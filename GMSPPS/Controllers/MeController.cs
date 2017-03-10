
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
   // [Obsolete]
    public class MeController : ApiController
    {
        private ApplicationUserManager _userManager;
        public GMSPPSEntities ctx;
        public MeController()
        {
        }
        [Obsolete]
        public MeController(ApplicationUserManager userManager)
        {
            ctx = new GMSPPSEntities();
            UserManager = userManager;
        }

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

        // GET api/Me
        [Obsolete]
        public GetViewModel Get()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            return new GetViewModel() { Email = user.Email };
        }
    }
}