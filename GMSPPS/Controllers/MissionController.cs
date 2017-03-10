using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;

using System.Net;

using GMSPPS.Models;
using System.Security.Cryptography;
using System.Text;

namespace GMSPPS.Controllers
{
    public class MissionController : Controller
    {
        private GMSPPSEntities db = new GMSPPSEntities();
        // GET: Mission
        public ActionResult Index()
        {
            return View();
        }

        // GET: GPMS_PROVIDER_test/Details/5
        public ActionResult Details(int? id)
        {
           
            //var user = UserManager.FindById(User.Identity.GetUserId());
            //return new GetViewModel() { Email = user.Email };
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GPMS_MISSION mission = db.GPMS_MISSION.Find(id);            
            if (mission == null)
            {
                return HttpNotFound();
            }
            MissionViewModel missionvm = new MissionViewModel()
            {
                ID = mission.ID,
                CostumMissionID = mission.CostumMissionID,
                Text = mission.Text,
                Title = mission.Title,
                ProviderLogo = db.GPMS_PROVIDER.Where(x => x.ID == mission.GPMS_PROVIDER_ID).FirstOrDefault().LOGO_URL,
                ProviderName = db.GPMS_PROVIDER.Where(x => x.ID == mission.GPMS_PROVIDER_ID).FirstOrDefault().Name,
                LAT = (double)mission.LAT,
                LON= (double)mission.LON
            };
            return View(missionvm);
        }
    }
}