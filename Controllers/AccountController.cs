﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.ViewModel;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Project.Models;
using Project.Filters;
using System.Configuration;

namespace Project.Controllers
{
    [MyAuthenFilter]
    public class AccountController : Controller
    {
        // GET: Account
        Project.Models.ShopDBContext db = new ShopDBContext();
        ShopDBContext DB = new ShopDBContext();
        [OverrideAuthentication]
        [OverrideExceptionFilters]
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
                    IsActive=true
                };

                IdentityResult identityResult = userManager.Create(user);
                if (identityResult.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Customer");
                    this.LoginUser(userManager, user);
                    
                }
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("New error", "Invalid data");
                return View();
            }

        }
        [NonAction]
        public void LoginUser(AppUserManager userManager,AppUser user)
        {
            var authenManager = HttpContext.GetOwinContext().Authentication;
            var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            authenManager.SignIn(new AuthenticationProperties(), userIdentity);
        }
        [OverrideAuthentication]
        [OverrideExceptionFilters]
        public ActionResult LogIn()
        {
            return View();
        }
        [OverrideAuthentication]
        [OverrideExceptionFilters]
        [HttpPost]
        public ActionResult LogIn(LoginVm vm)
        {
            var appDBContext = new ShopDBContext();
            var userStore = new AppUserStore(appDBContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.Find(vm.UserName, vm.Password);
            if (user != null)
            {
                if(user.IsActive)
                {
                    this.LoginUser(userManager, user);
                    if (userManager.IsInRole(user.Id,"Admin"))
                    {
                        return RedirectToAction("Index", "Home",new {area="Admin"});
                    }
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.Error = "Tài khoản của bạn đã bị khóa vui lòng liên hệ AD để biết thêm chi tiết";
                ModelState.AddModelError("myError", "Tài khoản của bạn đã bị khóa vui lòng liên hệ AD để biết thêm chi tiết");
                return View();
            }
            else
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
                ModelState.AddModelError("myError", "Invalid username and password");
                return View();
            }

        }
        [MyAuthenFilter]
        public ActionResult LogOut()
        {
            var authenManager = HttpContext.GetOwinContext().Authentication;
            authenManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
        [MyAuthenFilter]
        public new ActionResult Profile()
        {
            string curentUserID = User.Identity.GetUserId();
            AppUser curentUser = db.Users.Where(t=>t.Id== curentUserID).FirstOrDefault();
            List<Order> Orders = db.Orders.Where(t => t.IdUser==curentUserID).ToList();
            ViewBag.Orders = Orders;
            return View(curentUser);
        }
        [HttpPost]
        public ActionResult Edit(AppUser appUser)
        {
            AppUser User = db.Users.Where(t=>t.Id==appUser.Id).FirstOrDefault();
            User.FullName = appUser.FullName;
            User.Address = appUser.Address;
            User.City=appUser.City;
            User.PhoneNumber=appUser.PhoneNumber;
            User.BirthDay=appUser.BirthDay;
            User.Email=appUser.Email;
            User.Gender=appUser.Gender;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Profile", "Account");
            }
            return View(appUser);
        }
        [MyAuthenFilter]
        public ActionResult ShowCount()
        {
            string curentUserID = User.Identity.GetUserId();
            List<Models.Cart> Carts = db.Carts.Where(t => t.IdUser == curentUserID).ToList();
            if (Carts!=null)
            {
                return Json(new {Count=Carts.Count()},JsonRequestBehavior.AllowGet);
            }
            return Json(new {Count = 0}, JsonRequestBehavior.AllowGet);
        }
        [MyAuthenFilter]
        public ActionResult Cart()
        {
            string curentUserID = User.Identity.GetUserId();
            List<ShoppingCartItem> products=new List<ShoppingCartItem>();
            
            Product product = new Product();
            List<Models.Cart> Carts = db.Carts.Where(t => t.IdUser == curentUserID).ToList();
            double TongTien = 0;
            foreach (var item in Carts)
            {
                ShoppingCartItem x = new ShoppingCartItem();
                product = DB.Products.Find(item.IdProduct);
                x.ProductId = product.ProductId;
                x.Price =item.Price;
                x.Name =(string)product.Name;
                x.img = product.ImgeMain;
                x.Quantity = item.Quantity;
                x.TotalPrice = x.Quantity * x.Price;
                products.Add(x);
                TongTien += x.TotalPrice;
            }
            ViewBag.TongTien = TongTien;
            return View(products);
        }
        [HttpPost]
        [MyAuthenFilter]
        public ActionResult AddToCart(string id, int Quantity,float price)
        {
            string curentUserID = User.Identity.GetUserId();
            var code = new { Success = false, msg = "", code = -1, count = 0 };
            var Cart = db.Carts.FirstOrDefault(x => x.IdUser == curentUserID &&x.IdProduct==id);
            Cart Carts = new Cart();
            Carts.IdProduct = id;
            Carts.IdUser = curentUserID;
            Carts.Quantity = Quantity;
            Carts.Price = price;
            if (Cart == null)
            {
                db.Carts.Add(Carts);
                db.SaveChanges();
                code = new { Success = true, msg = "Them san pham vao gio hang thanh cong", code = 1, count = db.Carts.Count() };
            }
            else
            {
                Quantity +=  1;
                Cart.Quantity=Quantity;
                db.SaveChanges();
            }    
            return Json(code);
        }

        public ActionResult Partital_Item_Cart()
        {
            return PartialView();
        }    
        [HttpPost]
        public ActionResult Delete(string id)
        {
            string curentUserID = User.Identity.GetUserId();
            var code = new { Success = false, msg = "", code = -1, count = 0 };
            var Cart = db.Carts.Where(t=>t.IdUser==curentUserID);
            
            if (Cart!=null)
            {
                var checkProducts = db.Carts.FirstOrDefault(x => x.IdProduct == id);
                if (checkProducts!=null)
                {
                    db.Carts.Remove(checkProducts);
                    db.SaveChanges();
                    code = new { Success = true, msg = "", code = 1, count = db.Carts.Count() };
                }    
            }
            return Json(code);
        }
        public ActionResult ChangePasWord(string password)
        {
            var passwdHash = Crypto.HashPassword(password);
            string curentUserID = User.Identity.GetUserId();
            AppUser appUser = db.Users.Find(curentUserID);
            appUser.PasswordHash = passwdHash;
            DB.SaveChanges();
            return Json(appUser);
        }
       
    }
}