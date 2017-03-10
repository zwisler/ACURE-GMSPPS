using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GMSPPS;
using System.Security.Cryptography;
using System.Text;
using GMSPPS.Models;


//using System.Web.Http;
using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace GMSPPS.Controllers
{
    /// <summary>
    /// Dieser Controller wird für die ASP Seite Controller verwendet
    /// </summary>
    [Authorize]
    public class PROVIDERController : Controller
    {
        private ApplicationUserManager _userManager;
        private GMSPPSEntities db = new GMSPPSEntities();
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: GPMS_PROVIDER_test
        public ActionResult Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var gPMS_PROVIDER = db.GPMS_PROVIDER.Include(g => g.GPMS_PROVIDER_TYP).Where( x => x.UserName == user.Email);
            return View(gPMS_PROVIDER.ToList());
        }
        public ActionResult Wait(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GPMS_PROVIDER gPMS_PROVIDER = db.GPMS_PROVIDER.Find(id);
            if (gPMS_PROVIDER == null)
            {
                return HttpNotFound();
            }
            GPMS_PROVIDERSimpleModel myprovider = new GPMS_PROVIDERSimpleModel()                
                {
                Name = gPMS_PROVIDER.Name,
                Url = gPMS_PROVIDER.URL,
                Icon = gPMS_PROVIDER.LOGO_URL,
                ID = gPMS_PROVIDER.ID
            };
            return View(myprovider);
           
        }

        // GET: GPMS_PROVIDER_test/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GPMS_PROVIDER gPMS_PROVIDER = db.GPMS_PROVIDER.Find(id);
            if (gPMS_PROVIDER == null)
            {
                return HttpNotFound();
            }
            return View(gPMS_PROVIDER);
        }

        // GET: GPMS_PROVIDER_test/Create
        public ActionResult Create()
        {
            ViewBag.GPMS_PROVIDER_TYP_ID = new SelectList(db.GPMS_PROVIDER_TYP, "ID", "TYP");
            return View();
        }

        // POST: GPMS_PROVIDER_test/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,GPMS_PROVIDER_TYP_ID,Name,URL,LOGO_URL,Suscribe_Password,ConfirmPassword")] GPMS_PROVIDER gPMS_PROVIDER)
        {
           
             var user = UserManager.FindById(User.Identity.GetUserId());
            if (db.GPMS_PROVIDER.Where(x => x.Name.ToUpper() == gPMS_PROVIDER.Name.ToUpper()).Count() > 0)
            {
                ModelState.AddModelError("Name", $"Provider APP with the name {gPMS_PROVIDER.Name} already exist!");                
                ViewBag.GPMS_PROVIDER_TYP_ID = new SelectList(db.GPMS_PROVIDER_TYP, "ID", "TYP", gPMS_PROVIDER.GPMS_PROVIDER_TYP_ID);
                return View(gPMS_PROVIDER);
            }           

            if (ModelState.IsValid)
            {
                gPMS_PROVIDER.Email = user.Email;
                gPMS_PROVIDER.API_TOOKEN = GetHashString(gPMS_PROVIDER.Name);
                gPMS_PROVIDER.UserName = user.UserName;
                db.GPMS_PROVIDER.Add(gPMS_PROVIDER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GPMS_PROVIDER_TYP_ID = new SelectList(db.GPMS_PROVIDER_TYP, "ID", "TYP", gPMS_PROVIDER.GPMS_PROVIDER_TYP_ID);
            return View(gPMS_PROVIDER);
        }

        // GET: GPMS_PROVIDER_test/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GPMS_PROVIDER gPMS_PROVIDER = db.GPMS_PROVIDER.Find(id);
            if (gPMS_PROVIDER == null)
            {
                return HttpNotFound();
            }
            ViewBag.GPMS_PROVIDER_TYP_ID = new SelectList(db.GPMS_PROVIDER_TYP, "ID", "TYP", gPMS_PROVIDER.GPMS_PROVIDER_TYP_ID);
            return View(gPMS_PROVIDER);
        }

        // POST: GPMS_PROVIDER_test/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,GPMS_PROVIDER_TYP_ID,Name,URL,Email,LOGO_URL,Suscribe_Password,ConfirmPassword,UserName,API_TOOKEN")] GPMS_PROVIDER gPMS_PROVIDER)
        {
            if (ModelState.IsValid)
            {
                gPMS_PROVIDER.API_TOOKEN = GetHashString(gPMS_PROVIDER.Name);
                db.Entry(gPMS_PROVIDER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GPMS_PROVIDER_TYP_ID = new SelectList(db.GPMS_PROVIDER_TYP, "ID", "TYP", gPMS_PROVIDER.GPMS_PROVIDER_TYP_ID);
            return View(gPMS_PROVIDER);
        }

        // GET: GPMS_PROVIDER_test/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GPMS_PROVIDER gPMS_PROVIDER = db.GPMS_PROVIDER.Find(id);
            if (gPMS_PROVIDER == null)
            {
                return HttpNotFound();
            }
            return View(gPMS_PROVIDER);
        }

        // POST: GPMS_PROVIDER_test/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GPMS_PROVIDER gPMS_PROVIDER = db.GPMS_PROVIDER.Find(id);
            db.GPMS_PROVIDER.Remove(gPMS_PROVIDER);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();

        }
        private static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();  //or use SHA1.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
