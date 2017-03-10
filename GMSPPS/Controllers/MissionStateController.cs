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
    public class MissionStateController : ApiController
    {
        public GMSPPSEntities ctx;

        public MissionStateController()
        {

            ctx = new GMSPPSEntities();
        }
        // PUT api/register/5
        // This creates or updates a registration (with provided channelURI) at the specified id
        public async Task<HttpResponseMessage> Post(int Mission, int state)
        {
            Trace.TraceInformation($"POST Mission Status : {state.ToString()} mit mission: {Mission.ToString()} " );
            var user = HttpContext.Current.User.Identity.Name;
            Trace.TraceInformation("POST User: " + user);
            if (user == null) return null;
            try
            {
                // get The Client
                Trace.TraceInformation("Get The Client");
                GPMS_CLIENT Client = ctx.GPMS_CLIENT.Where(x => x.Email == user).FirstOrDefault();
                GPMS_MISSION myMission = ctx.GPMS_MISSION.Where(x => x.ID == Mission).FirstOrDefault();
                GPMS_MISSION_CLIENT_ACC ClientState = ctx.GPMS_MISSION_CLIENT_ACC.Where(x => x.GPMS_MISSION_ID == Mission && x.GPMS_CLIENT_ID == Client.ID).FirstOrDefault();
                if (ClientState == null)
                {
                    // ToDo Lat und Lon erweitern
                    ClientState = new GPMS_MISSION_CLIENT_ACC()
                    {

                        DATETIME = DateTime.Now,
                        GPMS_CLIENT_ID = Client.ID,
                        GPMS_MISSION_ACC_ID = state,
                        GPMS_MISSION_ID = Mission
                    };
                    ctx.GPMS_MISSION_CLIENT_ACC.Add(ClientState);                    
                }
                else
                {
                    ClientState.DATETIME = DateTime.Now;
                    ClientState.GPMS_MISSION_ACC_ID = state;
                }
                ctx.SaveChanges();
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MissionHub>();
                context.Clients.Group($"Mission#{Mission}").ClientAkk(Client.UserName , state, ClientState.GPMS_MISSION_ID, myMission.CostumMissionID);
                Trace.TraceInformation($"Mission Akk gesendet : {Client.UserName}");


                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Trace.TraceError("POST Status Error:" + ex.InnerException.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
