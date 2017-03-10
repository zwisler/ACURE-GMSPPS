using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using System.Net.Http;
using GMSPPS.Models;
using System.Diagnostics;
using Microsoft.ServiceBus.Notifications;
using Microsoft.ServiceBus.Messaging;

namespace GMSPPS.Controllers
{
   // [Authorize]
    public class GPMS_SuscribeController : ApiController
    {
        private NotificationHubClient hub;

        public GMSPPSEntities ctx;
        public GPMS_SuscribeController()
        {
            hub = Notifications.Instance.Hub;
            ctx = new GMSPPSEntities();
        }


        // GET api/values
        [Obsolete]
        public IQueryable<GPMS_PROVIDERBindingModel> GetProviders()
        {
           
            try
            {
                Trace.TraceInformation("Get Provider Subscibtion aufgerufen");
                string user = HttpContext.Current.User.Identity.Name;
                Trace.TraceInformation("User: " + user);
                if (user == null) return null;
                //TODO Tabele u User erweitern mit Google ID 
                // get The Client
                GPMS_CLIENT Client = ctx.GPMS_CLIENT.Where(x => x.Email == user).FirstOrDefault();
                Trace.TraceInformation("Client: " + Client.UserName);

                if (Client != null)
                {
                    var prov = from c in ctx.GPMS_PROVIDER
                               join ca in ctx.GPMS_PROVIDER_CLIENTS on c.ID equals ca.GPMS_PROVIDER_ID
                               where ca.GPMS_CLIENT_ID == Client.ID
                               select new GPMS_PROVIDERBindingModel()
                               {
                                   Name = c.Name,
                                   Url = c.URL,
                                   Icon = c.LOGO_URL,
                                   Typeint = (int)c.GPMS_PROVIDER_TYP_ID
                               };
                    return prov;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error by GPMS_SuscribeController: " + e);
                return null;
            }
        }
        [Obsolete]
        public async Task<string> Post(string handle = null)
        {
            string newRegistrationId = null;
            Trace.TraceInformation("Post im Register Controller wird aufgerufen" + handle);

            // make sure there are no existing registrations for this push handle (used for iOS and Android)
            if (handle != null)
            {
                // Code do Delete all Registartions
                //var all = await hub.GetAllRegistrationsAsync(100);
                //foreach (RegistrationDescription registration in all)
                //{
                //    await hub.DeleteRegistrationAsync(registration);
                //}

                    var registrations = await hub.GetRegistrationsByChannelAsync(handle, 100);

                foreach (RegistrationDescription registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                        Trace.TraceInformation("New ID : " + newRegistrationId);
                    }
                    else
                    {
                        Trace.TraceInformation("Delete Registration : " + registration);
                        await hub.DeleteRegistrationAsync(registration);
                    }
                }
            }

            if (newRegistrationId == null)
                newRegistrationId = await hub.CreateRegistrationIdAsync();
            var user = HttpContext.Current.User.Identity.Name;
            if (user != null)
            {
                // get The Client
                GPMS_CLIENT Client = ctx.GPMS_CLIENT.Where(x => x.Email == user).FirstOrDefault();
                if (Client != null)
                {
                    Client.RegistrationID = newRegistrationId;
                    ctx.SaveChanges();
                }
            }
            return newRegistrationId;
        }

        // PUT api/register/5
        // This creates or updates a registration (with provided channelURI) at the specified id
        [Obsolete]
        public async Task<HttpResponseMessage> Put(string id, DeviceRegistration deviceUpdate)
        {
            try
            {
                if (deviceUpdate.Tags.Count() > 0)
            {
                Trace.TraceInformation($"PUT im Register Controller ID: { id }  PRID: {deviceUpdate.ProviderID} mit Key {deviceUpdate.Tags[0]}");
            }
            else
            {
                Trace.TraceInformation($"PUT im Register Controller ID: { id }  PRID: {deviceUpdate.ProviderID} ");
            }

            RegistrationDescription registration = null;
            switch (deviceUpdate.Platform)
            {
                case "mpns":
                    Trace.TraceInformation("mnps Device Update: ");
                    registration = new MpnsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "wns":
                    Trace.TraceInformation("Windows Device Update: wns ");
                    registration = new WindowsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "apns":
                    Trace.TraceInformation("Apple Device Update: ");
                    registration = new AppleRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "gcm":
                    Trace.TraceInformation("Google Device Update: ");
                    registration = new GcmRegistrationDescription(deviceUpdate.Handle);
                    break;
                default:
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            registration.RegistrationId = id;
            var username = HttpContext.Current.User.Identity.Name;
            registration.Tags = new HashSet<string>(deviceUpdate.Tags);
            if (username != null)
            {
                Trace.TraceInformation("User OK");
                // get The Client
                GPMS_CLIENT Client = ctx.GPMS_CLIENT.Where(x => x.Email == username).FirstOrDefault();
                if (Client != null)
                {
                    Trace.TraceInformation("Client OK");
                    Client.RegistrationID = id;
                    ctx.SaveChanges();

                    registration.Tags.Add($"username:{username}");
                    Trace.TraceInformation($"UserName on reg : {username}");
                    GPMS_PROVIDER Provider = ctx.GPMS_PROVIDER.Where(x => x.ID == deviceUpdate.ProviderID).FirstOrDefault();
                    if (Provider != null  )
                    {
                        Trace.TraceInformation("Provider OK");
                        if (Provider.Suscribe_Password != deviceUpdate.Key )
                        {
                            Trace.TraceInformation("Passwort falsch");
                            return Request.CreateResponse(HttpStatusCode.Unauthorized);
                        }
                        registration.Tags.Add($"Provider:{Provider.Name}");
                        GPMS_PROVIDER_CLIENTS PROVIDER_CLIENT = ctx.GPMS_PROVIDER_CLIENTS.Where(x => x.GPMS_PROVIDER_ID == Provider.ID && x.GPMS_CLIENT_ID == Client.ID).FirstOrDefault();
                        if (PROVIDER_CLIENT == null)
                        {
                            GPMS_PROVIDER_CLIENTS ProClient = new GPMS_PROVIDER_CLIENTS()
                            {
                                GPMS_CLIENT_ID = Client.ID,
                                GPMS_PROVIDER_ID = Provider.ID
                            };
                            ctx.GPMS_PROVIDER_CLIENTS.Add(ProClient);
                            ctx.SaveChanges();
                        }
                        if (deviceUpdate.Tags.Count() > 0)
                        {
                            foreach (String tag in deviceUpdate.Tags)
                            {
                                Trace.TraceInformation($"Tag gelesen: {tag}");
                                GPMS_PROVIDER_SUPPLY GPMSTag = ctx.GPMS_PROVIDER_SUPPLY.Where(x => x.GPMS_PROVIDER_ID == Provider.ID && x.SUPPLY == tag).FirstOrDefault();
                                if(GPMSTag != null)
                                {
                                    Trace.TraceInformation($"Tag ist subly: {tag} + {GPMSTag.SUPPLY}");
                                    GPMS_CLIENT_SUSCRIPT s = ctx.GPMS_CLIENT_SUSCRIPT.Where(x => x.GPMS_PROVIDER_SUPPLY_ID == GPMSTag.ID && x.GPMS_CLIENT_ID == Client.ID).FirstOrDefault();
                                    if(s == null)
                                    {
                                            Trace.TraceInformation($"Tag wird gespeichert: {tag} ");
                                            s = new GPMS_CLIENT_SUSCRIPT()
                                        {
                                            GPMS_CLIENT_ID = Client.ID,
                                            GPMS_PROVIDER_SUPPLY_ID = GPMSTag.ID                                           
                                            
                                        };
                                        ctx.GPMS_CLIENT_SUSCRIPT.Add(s);
                                        ctx.SaveChanges();
                                    }
                                }                                
                            }
                        }


                    }
                }
            }
            

            // add check if user is allowed to add these tags
            try
            {
                await hub.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (MessagingException e)
            {
                Trace.TraceError("Error by REG on Device: " + deviceUpdate.Platform + " :: " + e);
                ReturnGoneIfHubResponseIsGone(e);
            }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error: " + " :: " + e);

            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/register/5
        [Obsolete]
        public async Task<HttpResponseMessage> Delete(string id)
        {
            await hub.DeleteRegistrationAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        {
            var webex = e.InnerException as WebException;
            if (webex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = (HttpWebResponse)webex.Response;
                if (response.StatusCode == HttpStatusCode.Gone)
                    throw new HttpRequestException(HttpStatusCode.Gone.ToString());
            }
        }


    }
}
