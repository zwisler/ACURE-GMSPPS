using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace GMSPPS
{
    class SignalRChat : Hub
    {
        public void GreetAll()
        {
            
            Clients.All.acceptGreet("Good morning! The time is " + DateTime.Now.ToString());
        }
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }

    }
}
