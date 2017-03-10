using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using GMSPPS.Models;

namespace GMSPPS.Controllers
{
    //[Authorize]
    //[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
    public class VersionController : ApiController
    {
        private ApplicationUserManager _userManager;
        public GMSPPSEntities ctx;
        public VersionController()
        {
        }





        // GET api/Version
        public VersionViewModel Get(string device)
        {
            VersionViewModel android = new VersionViewModel() { Device =  "android", Version = 33 };
            VersionViewModel windows = new VersionViewModel() { Device = "windows", Version = 24 };
            List<VersionViewModel> Q = new List<VersionViewModel>();
            Q.Add(android);
            Q.Add(windows);           
            return Q.Where(x => x.Device == device).FirstOrDefault();
        }
    }
}
