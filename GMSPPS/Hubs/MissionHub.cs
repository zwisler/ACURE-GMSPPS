using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using System.Data;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GMSPPS.Hubs
{
   /// <summary>
   /// Mission Hub 
   /// </summary>
    [HubName("missionhub")]
    public class MissionHub : Hub
    {
        private static ConcurrentDictionary<string, List<int>> _mapping = new ConcurrentDictionary<string, List<int>>();
        /// <summary>
        /// Suscribe for a Mission by ID
        /// </summary>
        /// <param name="MissionID"> ID off Mission</param>
        public void subscribe(string MissionID)
        {
            
                    
            Trace.TraceInformation($"Subscribe erreicht id : {MissionID}" );
            Groups.Add(Context.ConnectionId, $"Mission#{MissionID}" );
        }
        /// <summary>
        /// Unuscribe for a Mission by ID
        /// </summary>
        /// <param name="MissionID">ID off Mission</param>
        public void unsubscribe(string MissionID)
        {
            Groups.Remove(Context.ConnectionId, $"Mission#{MissionID}");
        }
    }
}