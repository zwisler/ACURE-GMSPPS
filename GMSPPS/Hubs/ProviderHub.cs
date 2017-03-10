using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using System.Data;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GMSPPS.Hubs
{
    [HubName("providerhub")]
    public class ProviderHub : Hub
    {
        private static ConcurrentDictionary<string, List<int>> _mapping = new ConcurrentDictionary<string, List<int>>();
        private GMSPPSEntities ctx = new GMSPPSEntities();       

        public void Subscribe(string Provider)
            
        {
            Trace.TraceInformation($"Subscribe provider: {Provider}");
            Groups.Add(Context.ConnectionId, Provider);
        }

        public void Unsubscribe(string Provider)
        {
            Trace.TraceInformation($"Unsubscribe provider: {Provider}");
            Groups.Remove(Context.ConnectionId, Provider);
        }
    }
}