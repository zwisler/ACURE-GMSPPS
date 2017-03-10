using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using GMSPPS.Models;





//using System.Security.Claims;
//httpContext
using System.Web;



namespace GMSPPS.Controllers
{
    
  //  [Authorize]
   // [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
    [RoutePrefix("GPMS_PROVIDER")]
    public class GPMS_PROVIDERController : ApiController
    {
        public ApplicationUserManager UserManager
        {
            get
            {
                
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                ctx = new GMSPPSEntities();
                _userManager = value;
            }
        }
        public GPMS_PROVIDERController(ApplicationUserManager userManager)
        {
            ctx = new GMSPPSEntities();
            UserManager = userManager;
        }
        public GPMS_PROVIDERController()
        {
            ctx = new GMSPPSEntities();
        }
        private ApplicationUserManager _userManager;
        public GMSPPSEntities ctx;


        // GET api/values
        [Obsolete]
        public IQueryable<GPMS_PROVIDERBindingModel> Get()
        {
            // var user = UserManager.FindById(User.Identity.GetUserId());
             var prov = from c in ctx.GPMS_PROVIDER
                       // where c.Email == user.Email
                        select new GPMS_PROVIDERBindingModel()
                         {
                          ID = c.ID,
                          Name = c.Name,
                          Url =  c.URL,
                          Icon = c.LOGO_URL,
                          Typeint =  (int)c.GPMS_PROVIDER_TYP_ID
                      };
                      

             return prov;
        }

        // GET api/values/5 GPMS_ProviderTagsModel
        [Obsolete]
        [ActionName("TAGS")]       
        public IQueryable<GPMS_ProviderTagsModel> GetTags(int id)
        {
            var tags = from c in ctx.GPMS_PROVIDER_SUPPLY
                       where c.GPMS_PROVIDER_ID == id
                       select new GPMS_ProviderTagsModel()
                       {
                           ID = c.ID,
                           ProviderID = (int)c.GPMS_PROVIDER_ID,
                           Name = c.SUPPLY
                       };

            return tags;
        }

        // POST api/values
        [Obsolete]
        public void Post(GPMS_PROVIDERBindingModel Provider)
        {
            
            var user = UserManager.FindById(User.Identity.GetUserId());
            GPMS_PROVIDER myProvider = ctx.GPMS_PROVIDER.Where(x => x.Name == Provider.Name).FirstOrDefault();
            if (myProvider == null)
            {
                //ProviderType Type = (ProviderType)Enum.Parse(typeof(ProviderType), Provider.Type, true);
                GPMS_PROVIDER Prov = new GPMS_PROVIDER()
                {
                  Name =   Provider.Name,
                  URL = Provider.Url,
                  Email = user.Email,
                  UserName = user.UserName,
                  GPMS_PROVIDER_TYP_ID = (int)Provider.Typeint
                };
                ctx.GPMS_PROVIDER.Add(Prov);
                ctx.SaveChanges();
            }
            
        }

        // PUT api/values/5
        [Obsolete]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [Obsolete]
        public void Delete(int id)
        {
        }
    }
}
