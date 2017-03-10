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
using System.Diagnostics;
using GMSPPS.Models;
using GMSPPS.Hubs;
using Microsoft.AspNet.SignalR;


namespace GMSPPS.Controllers
{
    // [Authorize]
    public class GPMS_StatusController : ApiController 
    {
        private ChatHub SignalIR;
        public GMSPPSEntities ctx;
        public GPMS_StatusController()
        {
            ctx = new GMSPPSEntities();
           
        }




        // GET api/values
        [Obsolete]
        public IQueryable<GPMS_PROVIDERBindingModel> GetState()
        {
            var user = HttpContext.Current.User.Identity.Name;
            if (user == null) return null;
            //TODO Tabele u User erweitern mit Google ID 
            // get The Client
            GPMS_CLIENT Client = ctx.GPMS_CLIENT.Where(x => x.Email == user).FirstOrDefault();
            
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
        [Obsolete]
        public async Task<HttpResponseMessage> Post(int State, double LAT, double LON)
        {
            Trace.TraceInformation("POST Status lat: " + LAT + " LON: " + LON);
            var user =  HttpContext.Current.User.Identity.Name;
            Trace.TraceInformation("POST User: " + user);
            if (user == null) return null;
            try
            {
                // get The Client
                Trace.TraceInformation("Get The Client");
                GPMS_CLIENT Client = ctx.GPMS_CLIENT.Where(x => x.Email == user).FirstOrDefault();
                
                List<String> Providers = ctx.GPMS_PROVIDER.Where(x => ctx.GPMS_PROVIDER_CLIENTS.Where(y => y.GPMS_CLIENT_ID == Client.ID).Select(z => z.GPMS_PROVIDER_ID).Contains(x.ID)).Select(Pro => Pro.Name).ToList();
                //x.GPMS_PROVIDER_CLIENTS.Any(acc => Ids.Contains(acc.ID))
                Trace.TraceInformation("Client is " + Client.UserName);
                // signal IR Test using GMSPPS.Hubs;
               
                // Important: .Resolve is an extension method inside SignalR.Infrastructure namespace.
                //IHubContext MyChatHub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                IHubContext MyChatHub = GlobalHost.ConnectionManager.GetHubContext<ProviderHub>();
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MissionHub>();
                context.Clients.All.addNewMessageToPage(Client.UserName, "The Pos is exakt Lat: " + LAT + " LON: " + LON + " Time UTC: " + DateTime.Now.ToString() + " Status: " + State);
                context.Clients.All.addPin(LAT, LON, Client.UserName , State);
                //MyChatHub.Clients.All.addPin(LAT, LON, Client.UserName, State);
                Trace.TraceInformation("Provider senden " + Providers[0]);
                MyChatHub.Clients.Groups(Providers).addPin(LAT, LON, Client.UserName, State);




                if (Client != null)
                {

                    GPMS_CLIENT_STATE Status = ctx.GPMS_CLIENT_STATE.Where(x => x.GPMS_CLIENT_ID == Client.ID).FirstOrDefault();



                    if (Status == null)
                    {
                        //Anlegen Erster Eintrag!!
                        Trace.TraceInformation("Client ID: " + Client.ID);
                        Trace.TraceInformation("State ID: " + State);
                        Trace.TraceInformation("Time UTC: " + DateTime.Now.ToString());
                        GPMS_CLIENT_STATE NewStatus = new GPMS_CLIENT_STATE()
                        {
                            GPMS_CLIENT_ID = Client.ID,                            
                            POS = System.Data.Entity.Spatial.DbGeography.FromText(String.Format("POINT({0} {1})", LAT.ToString(), LON.ToString()), 4326),
                            LAT = LAT,
                            LON = LON,
                            GPMS_STATE_ID = State,
                            DATETIME = DateTime.Now
                        };

                        ctx.GPMS_CLIENT_STATE.Add(NewStatus);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        //  Update 
                        Status.POS = System.Data.Entity.Spatial.DbGeography.FromText(String.Format("POINT({0} {1})", LAT.ToString(), LON.ToString()), 4326);
                        Status.LAT = LAT;
                        Status.LON = LON;
                        Status.GPMS_STATE_ID = State;
                        Status.DATETIME = DateTime.Now;
                        ctx.SaveChanges();
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch( Exception ex)
            {
                Trace.TraceError("POST Status Error:" + ex.InnerException.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
