using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BioForeignKit.Models;

namespace BioForeignKit.Controllers
{
    [Authorize(Roles = "Doktor,Koordinatör,Admin")]
    public class NationalityController : Controller
    {
        private BioForeignKİTDbEntities db = new BioForeignKİTDbEntities();

        // GET: Nationality
        public ActionResult Index()
        {
            return View(db.Nationalities.ToList());
        }

        // GET: Nationality/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Nationality/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nationality,RegisterDate,IsActive")] Nationalities nationalities)
        {
            if (ModelState.IsValid)
            {
                nationalities.RegisterDate = DateTime.Now;
                nationalities.IsActive = true;
                db.Nationalities.Add(nationalities);
                db.SaveChanges();
                TempData["successNationalityRegister"] = "*Uyruk kaydı başarıyla tamamlanmıştır.";
                return RedirectToAction("Index");
            }
            return View(nationalities);
        }

        // GET: Nationality/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Nationality");
            }
            Nationalities nationalities = db.Nationalities.Find(id);
            if (nationalities == null)
            {
                return RedirectToAction("Error", "Account");
            }
            return View(nationalities);
        }

        // POST: Nationality/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nationality,RegisterDate,IsActive")] Nationalities nationalities)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nationalities).State = EntityState.Modified;
                db.SaveChanges();
                TempData["successNationalityEdit"] = "*Uyruk kaydı başarıyla güncellenmiştir.";
                return RedirectToAction("Index");
            }
            return View(nationalities);
        }
      

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        // GET: Nationality/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Nationalities nationalities = db.Nationalities.Find(id);
        //    if (nationalities == null)
        //    {
        //        return RedirectToAction("Error", "Account");
        //    }
        //    return View(nationalities);
        //}
        // GET: Nationality/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {

        //        return RedirectToAction("Index", "Nationality");
        //    }
        //    Nationalities nationalities = db.Nationalities.Find(id);
        //    if (nationalities == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(nationalities);
        //}

        //// POST: Nationality/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Nationalities nationalities = db.Nationalities.Find(id);
        //    db.Nationalities.Remove(nationalities);
        //    db.SaveChanges();
        //    TempData["successNationalityDelete"] = "*Uyruk kaydı başarıyla silinmiştir.";
        //    return RedirectToAction("Index");
        //}
    }
}
