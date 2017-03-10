using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GMSPPS;

namespace GMSPPS.Controllers
{
    public class PROVIDER_SUPPLYController : Controller
    {
        private GMSPPSEntities db = new GMSPPSEntities();

        // GET: PROVIDER_SUPPLY
        public ActionResult Index(int? id)
        {
            var gPMS_PROVIDER_SUPPLY = db.GPMS_PROVIDER_SUPPLY.Include(g => g.GPMS_PROVIDER).Where(x => x.GPMS_PROVIDER_ID == id);
            return View(gPMS_PROVIDER_SUPPLY.ToList());
        }

        // GET: PROVIDER_SUPPLY/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GPMS_PROVIDER_SUPPLY gPMS_PROVIDER_SUPPLY = db.GPMS_PROVIDER_SUPPLY.Find(id);
            if (gPMS_PROVIDER_SUPPLY == null)
            {
                return HttpNotFound();
            }
            return View(gPMS_PROVIDER_SUPPLY);
        }

        // GET: PROVIDER_SUPPLY/Create
        public ActionResult Create()
        {
            ViewBag.GPMS_PROVIDER_ID = new SelectList(db.GPMS_PROVIDER, "ID", "UserName");
            return View();
        }

        // POST: PROVIDER_SUPPLY/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,GPMS_PROVIDER_ID,SUPPLY")] GPMS_PROVIDER_SUPPLY gPMS_PROVIDER_SUPPLY)
        {
            if (ModelState.IsValid)
            {
                db.GPMS_PROVIDER_SUPPLY.Add(gPMS_PROVIDER_SUPPLY);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GPMS_PROVIDER_ID = new SelectList(db.GPMS_PROVIDER, "ID", "UserName", gPMS_PROVIDER_SUPPLY.GPMS_PROVIDER_ID);
            return View(gPMS_PROVIDER_SUPPLY);
        }

        // GET: PROVIDER_SUPPLY/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GPMS_PROVIDER_SUPPLY gPMS_PROVIDER_SUPPLY = db.GPMS_PROVIDER_SUPPLY.Find(id);
            if (gPMS_PROVIDER_SUPPLY == null)
            {
                return HttpNotFound();
            }
            ViewBag.GPMS_PROVIDER_ID = new SelectList(db.GPMS_PROVIDER, "ID", "UserName", gPMS_PROVIDER_SUPPLY.GPMS_PROVIDER_ID);
            return View(gPMS_PROVIDER_SUPPLY);
        }

        // POST: PROVIDER_SUPPLY/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,GPMS_PROVIDER_ID,SUPPLY")] GPMS_PROVIDER_SUPPLY gPMS_PROVIDER_SUPPLY)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gPMS_PROVIDER_SUPPLY).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GPMS_PROVIDER_ID = new SelectList(db.GPMS_PROVIDER, "ID", "UserName", gPMS_PROVIDER_SUPPLY.GPMS_PROVIDER_ID);
            return View(gPMS_PROVIDER_SUPPLY);
        }

        // GET: PROVIDER_SUPPLY/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GPMS_PROVIDER_SUPPLY gPMS_PROVIDER_SUPPLY = db.GPMS_PROVIDER_SUPPLY.Find(id);
            if (gPMS_PROVIDER_SUPPLY == null)
            {
                return HttpNotFound();
            }
            return View(gPMS_PROVIDER_SUPPLY);
        }

        // POST: PROVIDER_SUPPLY/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GPMS_PROVIDER_SUPPLY gPMS_PROVIDER_SUPPLY = db.GPMS_PROVIDER_SUPPLY.Find(id);
            db.GPMS_PROVIDER_SUPPLY.Remove(gPMS_PROVIDER_SUPPLY);
            db.SaveChanges();
            return RedirectToAction("Index");
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
