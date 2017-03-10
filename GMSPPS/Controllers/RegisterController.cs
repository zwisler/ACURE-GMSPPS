using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.ServiceBus.Notifications;
using GMSPPS.Models;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Web;
using System.Diagnostics;

namespace GMSPPS.Controllers
{
    public class RegisterController : ApiController
    {
        private NotificationHubClient hub;
        public GMSPPSEntities ctx;

        public RegisterController()
        {
            hub = Notifications.Instance.Hub;
            ctx = new GMSPPSEntities();
        }
        [Obsolete]
        public async Task<string> Post(string handle = null)
        {
            string newRegistrationId = null;
            Trace.TraceInformation("Post im Register Controller wird aufgerufen" + handle );

            // make sure there are no existing registrations for this push handle (used for iOS and Android)
            if (handle != null)
            {
                var registrations = await hub.GetRegistrationsByChannelAsync(handle, 100);

                foreach (RegistrationDescription registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                        Trace.TraceInformation("New ID : " + newRegistrationId );
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
                    Trace.TraceInformation("Windows Device Update: wns " );
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

            if (username != null)
            {
                // get The Client
                GPMS_CLIENT Client = ctx.GPMS_CLIENT.Where(x => x.Email == username).FirstOrDefault();
                if (Client != null)
                {
                    Client.RegistrationID = id;
                    ctx.SaveChanges();
                }
            }

            // add check if user is allowed to add these tags
            registration.Tags = new HashSet<string>(deviceUpdate.Tags);
            registration.Tags.Add("username:" + username);
            registration.Tags.Add("Provider:Doit4you");
            Trace.TraceInformation("UserName on reg : " + username );

            try
            {
                await hub.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (MessagingException e)
            {
                Trace.TraceError("Error by REG on Device: " + deviceUpdate.Platform + " :: " + e);
                ReturnGoneIfHubResponseIsGone(e);
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
