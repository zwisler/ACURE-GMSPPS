using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GMSPPS.Models;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Xml;
using GMSPPS.Models;
using GMSPPS.Hubs;
using Microsoft.AspNet.SignalR;


namespace GMSPPS.Controllers
{
    /// <summary>
///  This class is a example.
/// </summary>
    public class GPMS_MissionController : ApiController
    {
        public GMSPPSEntities ctx;
        public GPMS_MissionController()
        {
            ctx = new GMSPPSEntities();

        }
        /// <summary> 
        /// Post a Mission to the Backend         
        /// </summary> 
        /// <param name="NewMission">Describe parameter.</param>

        /// <returns>Describe return value.</returns> 
        public async Task<int> Post(GPMS_MissionModel NewMission )
        {
            Trace.TraceInformation("Post MISSION erreicht");
            bool IsInRole = HttpContext.Current.User.IsInRole("GMSPPS_Provider");
            GPMS_MISSION DBMission = null;
            if (IsInRole)
            {
                Trace.TraceInformation("IST Provider");
                var user = HttpContext.Current.User.Identity.Name;
                // Provider Name
                GPMS_PROVIDER Provider = ctx.GPMS_PROVIDER.Where(x => x.Name == user).FirstOrDefault();
                //return ctx.GPMS_PROVIDER.Where(x => x.Name == user).FirstOrDefault().Name;
                Trace.TraceInformation($"Provider: {Provider.Name}");
                DBMission = ctx.GPMS_MISSION.Where(x => x.CostumMissionID == NewMission.CostumMissionID && x.GPMS_PROVIDER_ID == Provider.ID).FirstOrDefault();
                if (DBMission == null)
                {
                    Trace.TraceInformation("noch keine Mission");
                    DBMission = new GPMS_MISSION()
                    {
                        CostumMissionID = NewMission.CostumMissionID,
                        GPMS_PROVIDER_ID = Provider.ID,
                        Title = NewMission.Title,
                        Text = NewMission.Text,
                        LAT = NewMission.Positions.FirstOrDefault().LAT,
                        LON = NewMission.Positions.FirstOrDefault().LON,
                        GPMS_MISSION_STATE_ID = 1

                    };
                    ctx.GPMS_MISSION.Add(DBMission);
                    ctx.SaveChanges();
                    if (NewMission.URL == null || NewMission.URL == "") DBMission.CostumURL = $"{NewMission.DefaultUrl}Mission/Details/{DBMission.ID}";
                    else DBMission.CostumURL = NewMission.URL;
                }
                else
                {
                    DBMission.GPMS_MISSION_STATE_ID = 3;
                    DBMission.Text = NewMission.Text;
                    DBMission.LAT = NewMission.Positions.FirstOrDefault().LAT;
                    DBMission.LON = NewMission.Positions.FirstOrDefault().LON;
                    if (NewMission.URL == null || NewMission.URL == "") DBMission.CostumURL = $"{NewMission.DefaultUrl}Mission/Details/{DBMission.ID}";
                    else DBMission.CostumURL = NewMission.URL;
                    ctx.SaveChanges();
                }
                string[] userTag = new string[1];
                // TODO Achtung es sechin noch nicht alles mit dem Suscription zu stimmen behaben dann tresten
                //userTag[0] = "Provider:" + Provider.Name;
                Trace.TraceInformation($" Mission erstellt ID : {DBMission.ID} ");
                int typId = (int)Provider.GPMS_PROVIDER_TYP_ID;
                int missionId = DBMission.ID;
                // SiganalR Hub

                IHubContext MyChatHub = GlobalHost.ConnectionManager.GetHubContext<ProviderHub>();
                MyChatHub.Clients.Group(Provider.Name).StartMission(missionId, DBMission.CostumURL.ToString());
                Trace.TraceInformation($"Mission Start gesendet : {missionId}");
                // MS Service Buss
                Microsoft.ServiceBus.Notifications.NotificationOutcome outcome = null;
                HttpStatusCode ret = HttpStatusCode.InternalServerError;
                
                if(haveGoogleClients(Provider.ID))
                {
                    // Android
                     Trace.TraceInformation("gcm wird gesendet!");
                     var notif = "{ \"data\" : {\"Typ\":\"" + typId.ToString() + "\",\"ID\":\"" + missionId + "\",\"message\":\"" + DBMission.Title.ToString() + "\",\"url\":\"" + DBMission.CostumURL.ToString() +  "\"}}";
                   outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, userTag);

                }
                if (haveWindowsClients(Provider.ID))
                {
                    Trace.TraceInformation("wns  wird gesendet!");
                    // Windows 8.1 / Windows Phone 8.1

                    string RawToast = prepareRAWPayload(DBMission.CostumURL.ToString(), DBMission.ID.ToString(), typId.ToString());                   
                
                    Microsoft.ServiceBus.Notifications.WindowsNotification notification = new Microsoft.ServiceBus.Notifications.WindowsNotification(RawToast);
                    notification.Headers.Add("X-NotificationClass", "3"); // Required header for RAW
                    notification.Headers.Add("X-WNS-Type", "wns/raw"); // Required header for RAW
                    outcome = await Notifications.Instance.Hub.SendNotificationAsync(notification, userTag);

                }
                DBMission.GPMS_MISSION_STATE_ID = 2;
                ctx.SaveChanges();


            }
            ctx.SaveChanges();
            return DBMission.ID ;
        }
        // GET api/values/5 GPMS_ProviderTagsModel
        [Obsolete]
        public IQueryable<MissionViewModel> GetMission(int id)
        {
            var tags = from c in ctx.GPMS_MISSION
                       where c.ID == id
                       select new MissionViewModel()
                       {
                           CostumMissionID = c.CostumMissionID,
                           Text = c.Text,
                           Title = c.Title,
                           ProviderName =  c.GPMS_PROVIDER.Name,
                           ProviderLogo =  c.GPMS_PROVIDER.LOGO_URL,
                           ID = c.ID,
                           LAT = (double)c.LAT,
                           LON = (double)c.LON
                       };
            return tags;
        }
        private bool haveGoogleClients(int ProviderID)
        {

            //device Typ ID = 4 gcm Android
            var clientIds = ctx.GPMS_PROVIDER_CLIENTS.Where(x => x.GPMS_PROVIDER_ID == ProviderID).Select(y => y.GPMS_CLIENT_ID);
            return ctx.GPMS_CLIENT.Where(client => clientIds.Contains(client.ID)).Select(z => z.GPMS_DEVICE_TYPE_ID).Contains(4);
        }
        private bool haveWindowsClients(int ProviderID)
        {
            //device Typ ID = 1 wns
            var clientIds = ctx.GPMS_PROVIDER_CLIENTS.Where(x => x.GPMS_PROVIDER_ID == ProviderID).Select(y => y.GPMS_CLIENT_ID);
            return ctx.GPMS_CLIENT.Where(client => clientIds.Contains(client.ID)).Select(z => z.GPMS_DEVICE_TYPE_ID).Contains(1);
        }

        //public async Task<HttpResponseMessage> Post(string pns, [FromBody]string message, string ProviderName, String URL)
        //{

        //    var user = HttpContext.Current.User.Identity.Name;
        //    string[] userTag = new string[1];
        //    userTag[0] = "Provider:" + ProviderName;
        //    //userTag[1] = "from:" + user;
        //    //userTag[2] = "NEWS" ;

        //    Microsoft.ServiceBus.Notifications.NotificationOutcome outcome = null;
        //    HttpStatusCode ret = HttpStatusCode.InternalServerError;
        //    // Dumm IDs nur zum Test der RowNotification
        //    int typId = 1;
        //    int missionId = 125;
        //    switch (pns.ToLower())
        //    {
        //        case "wns":
        //            // Windows 8.1 / Windows Phone 8.1

        //            string RawToast = prepareRAWPayload(URL, missionId.ToString(), typId.ToString());
        //            string toast = @"<toast duration=""long""><visual><binding template=""ToastText01""><text id=""1"">" +
        //                        "From " + user + ": " + message + @"</text></binding> </visual><audio src=""ms-winsoundevent:Notification.Looping.Alarm"" loop=""true""/></visual></toast>";
        //            Trace.TraceInformation("Tost : " + toast);
        //            Microsoft.ServiceBus.Notifications.WindowsNotification notification = new Microsoft.ServiceBus.Notifications.WindowsNotification(RawToast);
        //            notification.Headers.Add("X-NotificationClass", "3"); // Required header for RAW
        //            notification.Headers.Add("X-WNS-Type", "wns/raw"); // Required header for RAW
        //            outcome = await Notifications.Instance.Hub.SendNotificationAsync(notification, userTag);

        //            ////outcome = await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast, userTag);
        //            break;
        //        case "apns":
        //            // iOS
        //            var alert = "{\"aps\":{\"alert\":\"" + message + "\",\"url\":\"" + URL + "\"}}";
        //            outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
        //            break;
        //        case "gcm":
        //            // Android
        //            Trace.TraceInformation("gcm wird gesendet!");
        //            var notif = "{ \"data\" : {\"Typ\":\"" + "1" + "\",\"ID\":\"" + "123" + "\",\"message\":\"" + message + "\",\"url\":\"" + URL + "\"}}";
        //            outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, userTag);
        //            break;
        //    }

        //    if (outcome != null)
        //    {
        //        if (!((outcome.State == Microsoft.ServiceBus.Notifications.NotificationOutcomeState.Abandoned) ||
        //            (outcome.State == Microsoft.ServiceBus.Notifications.NotificationOutcomeState.Unknown)))
        //        {
        //            ret = HttpStatusCode.OK;
        //        }
        //    }

        //    return Request.CreateResponse(ret);
        //}
        private static string prepareRAWPayload(string url,  string id, string typid)
        {
            // Create encoding manually in order to prevent
            // creation of leading BOM (Byte Order Mark) xFEFF at start
            // of string created from the XML
            Encoding Utf8 = new UTF8Encoding(false); // Prevents creation of BOM
            MemoryStream stream = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = false,
                //   Encoding = Encoding.UTF8    !!NO-> adds Unicode BOM to start
                Encoding = Utf8,    // Use manually created UTF8 encoding
            };
            XmlWriter writer = XmlTextWriter.Create(stream, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Mission");

            writer.WriteStartElement("ID");
            writer.WriteValue(id);
            writer.WriteEndElement();

            writer.WriteStartElement("Typ");
            writer.WriteValue(typid);
            writer.WriteEndElement();

            writer.WriteStartElement("Url");
            writer.WriteValue(url);
            writer.WriteEndElement();          

            writer.WriteStartElement("LastUpdated");
            writer.WriteValue(DateTime.Now.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
