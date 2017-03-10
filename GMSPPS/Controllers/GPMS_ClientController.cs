using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.ServiceBus.Notifications;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Web;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using GMSPPS.Models;
using Citaurus.WebClientTool;



namespace GMSPPS.Controllers
{
    [Authorize]
    public class GPMS_ClientController : ApiController
    {
        public GMSPPSEntities  ctx;
        //private NotificationHubClient hub;
        private ApplicationUserManager _userManager;

        public GPMS_ClientController()
        {
            ctx = new GMSPPSEntities();
            //hub = Notifications.Instance.Hub;
        }
        //public MeController(ApplicationUserManager userManager)
        //{
        //    UserManager = userManager;
        //}

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

      

       

       

        // GET api/values
     

         //GET api/values
        //public IQueryable<GPMS_PROVIDERBindingModel> GetClients()
        //{
            
        //    //GoggleToken token = new GoggleToken();
        //    //dynamic ret = token.GetTokeninfo(T);
        //    //string mail = ret.email;
        //    //string name = ret.name;
        //    //string GoggleID = ret.sub;
        //    // var user = UserManager.FindById(User.Identity.GetUserId());
        //    //var prov = from c in ctx.GPMS_PROVIDER
        //    //           // where c.Email == user.Email
        //    //           select new GPMS_PROVIDERBindingModel()
        //    //           {
        //    //               Name = c.Name,
        //    //               Url = c.URL,
        //    //              // Typeint = (int)c.GPMS_PROVIDER_TYP_ID
        //    //           };

           
        //    var prov = from c in ctx.GPMS_PROVIDER
        //               // where c.Email == user.Email
        //               select new GPMS_PROVIDERBindingModel()
        //               {
        //                   Name = c.Name,
        //                   Url = c.URL,
        //                   // Typeint = (int)c.GPMS_PROVIDER_TYP_ID
        //               };

        //    return prov;
                
        //}

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
