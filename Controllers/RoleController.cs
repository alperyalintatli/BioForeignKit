using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BioForeignKit.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BioForeignKit.Controllers
{
    [Authorize(Roles = "Doktor,Koordinatör,Admin")]
    public class RoleController : Controller
    {
        private RoleManager<ApplicationRole> roleManager;
        public RoleController()
        {
            roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()));
        }
        // GET: Role
        public ActionResult Index()
        {
            var model = roleManager.Roles.ToList();
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole { Name = model.Name };
                var result= roleManager.Create(role);
                if (result.Succeeded)
                {
                    TempData["successRoleCreate"] = "*Rol kaydı başarıyla yapılmıştır.";
                    return RedirectToAction("Index", "Role");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                        return View(model);
                    }
                }

            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var model = roleManager.FindById(id);
            if (model != null&&model.Name!="Hasta" && model.Name != "Koordinatör" && model.Name != "Doktor" && model.Name != "Admin")
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Error", "Account");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id,string name)
        {
            if (name!=null && name!="")
            {
                if (ModelState.IsValid)
                {
                    var model = roleManager.FindById(id);
                    if (model != null)
                    {
                        model.Name = name;
                        roleManager.Update(model);
                        TempData["successRoleEdit"] = "*Rol kaydı başarıyla güncellenmiştir.";
                        return RedirectToAction("Index", "Role");
                    }
                    else
                    {
                        return RedirectToAction("Error", "Account");
                    }
                }
            }
             return View(name);
        }

        //public ActionResult Delete(string id)
        //{
        //    var model = roleManager.FindById(id);
        //    if (model != null)
        //    {  return View(model);
        //    }
        //    return RedirectToAction("Index", "Role");
 
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteRole(string id)
        //{
        //    var model = roleManager.FindById(id);
        //    if (ModelState.IsValid)
        //        {
        //            if (model != null)
        //            {
        //            roleManager.Delete(model);
        //            TempData["successRoleDelete"] = "*Rol kaydı başarıyla silinmiştir.";
        //            return RedirectToAction("Index", "Role");
        //            }
        //        }

        //    ViewBag.deleteProblem = "*Öğe silinemedi.Tekrar deneyin.";
        //    return RedirectToAction("Delete","Role",model);
        //}
    }
}