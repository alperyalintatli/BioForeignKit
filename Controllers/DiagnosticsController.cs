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
    public class DiagnosticsController : Controller
    {
        private BioForeignKİTDbEntities db = new BioForeignKİTDbEntities();

        // GET: Diagnostics
        public ActionResult Index()
        {
            return View(db.Diagnostics.ToList());
        }

        // GET: Diagnostics/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Diagnostics");
            }
            Diagnostics diagnostics = db.Diagnostics.Find(id);
            if (diagnostics == null)
            {
                return RedirectToAction("Error", "Account");
            }
            return View(diagnostics);
        }

        // GET: Diagnostics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Diagnostics/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DiagnosticName,Description,RegisterDate,IsActive")] DiagnosticViewModel diagnostics)
        {
            if (ModelState.IsValid)
            {
                var diagnostic = new Diagnostics { Description = diagnostics.Description, DiagnosticName = diagnostics.DiagnosticName, RegisterDate = DateTime.Now, IsActive=true };
                db.Diagnostics.Add(diagnostic);
                db.SaveChanges();
                TempData["successDiagnosticAdd"] = "*Tanı kaydı başarıyla tamamlanmıştır.";
                return RedirectToAction("Index");
            }
            return View(diagnostics);
        }

        // GET: Diagnostics/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Diagnostics");
            }
            Diagnostics diagnostics = db.Diagnostics.Find(id);
            if (diagnostics == null)
            {
                return RedirectToAction("Error", "Account");
            }
            return View(diagnostics);
        }

        // POST: Diagnostics/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DiagnosticName,Description,RegisterDate,IsActive")] Diagnostics diagnostics)
        {
            if (ModelState.IsValid)
            {
                db.Entry(diagnostics).State = EntityState.Modified;
                db.SaveChanges();
                TempData["successDiagnosticEdit"] = "*Tanı kaydı başarıyla güncellenmiştir.";
                return RedirectToAction("Index");
            }
            return View(diagnostics);
        }

        // GET: Diagnostics/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction("Index", "Diagnostics");
        //    }
        //    Diagnostics diagnostics = db.Diagnostics.Find(id);
        //    if (diagnostics == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(diagnostics);
        //}

        //// POST: Diagnostics/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Diagnostics diagnostics = db.Diagnostics.Find(id);
        //    db.Diagnostics.Remove(diagnostics);
        //    db.SaveChanges();
        //    TempData["successDiagnosticDelete"] = "*Tanı kaydı başarıyla silinmiştir.";
        //    return RedirectToAction("Index");
        //}

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
