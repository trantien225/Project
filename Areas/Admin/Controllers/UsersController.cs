using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using Project.Models;
using Project.ViewModel;

namespace Project.Areas.Admin.Controllers
{
    
    public class UsersController : Controller
    {
        // GET: Admin/User
        ShopDBContext DB=new ShopDBContext();

        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;
            var Users =DB.Users.OrderByDescending(x => x.UserName).ToList();         
            return View(Users.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Detail(string Id)
        {
            var User=DB.Users.Find(Id);
            ViewBag.Roles=DB.Roles.ToList();
            return View(User);
        }
        [HttpPost]
        public ActionResult Edit(AppUser app)
        {
            AppUser User = DB.Users.Where(t=>t.Id==app.Id).FirstOrDefault();
            User.FullName=app.FullName;
            User.Email=app.Email;
            User.IsActive=app.IsActive;
            DB.SaveChanges();
            return RedirectToAction("Index", "Users");
        }
        [HttpPost]
        public ActionResult IsActive(string id)
        {
            var item = DB.Users.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                DB.Entry(item).State = System.Data.Entity.EntityState.Modified;
                DB.SaveChanges();
                return Json(new { success = true, isAcive = item.IsActive });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult Remove(string id)
        {
            var item = DB.Users.Find(id);
            if (item!=null)
            {
                DB.Users.Remove(item);
                DB.SaveChanges();
                return Json(new { success = true }) ;
            }
            return Json(new { success = false });
        }
        public ActionResult Add()
        {
            return View();
        }
        public ActionResult Regisster()
        {
            return View();
        }
        [HttpPost]
        [OverrideAuthentication]
        [OverrideExceptionFilters]
        public ActionResult Regisster(RegisterVM vm)
        {
            if (ModelState.IsValid)
            {
                var appDBContext = new ShopDBContext();
                var userStore = new AppUserStore(appDBContext);
                var userManager = new AppUserManager(userStore);
                var passwdHash = Crypto.HashPassword(vm.Password);
                var user = new AppUser()
                {
                    Email = vm.Email,
                    UserName = vm.UserName,
                    PasswordHash = passwdHash,
                    City = vm.City,
                    BirthDay = vm.DateOfBirth,
                    FullName = vm.FullName,
                    Address = vm.Address,
                    PhoneNumber = vm.Mobile,
                    IsActive = true
                };

                IdentityResult identityResult = userManager.Create(user);
                if (identityResult.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");

                }
                return View();
            }
            else
            {
                ModelState.AddModelError("New error", "Invalid data");
                return View();
            }

        }
    }
}