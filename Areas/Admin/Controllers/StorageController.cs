using Project.Filters;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Admin.Controllers
{
    [MyAuthenFilter]
    [AdminAuthorrization]
    public class StorageController : Controller
    {
        // GET: Admin/Storage
        private ShopDBContext DB = new ShopDBContext();
        public ActionResult Index()
        {
            var item = DB.Storages.ToList();
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Storage p)
        {
            ViewBag.Products = DB.Products.ToList();
            if (ModelState.IsValid)
            {
                DB.Storages.Add(p);
                DB.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(p);
        }
        public ActionResult Create()
        {
            ViewBag.Products = DB.Products.ToList();
            return View();
        }
    }
}