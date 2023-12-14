using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;
using Microsoft.AspNet.Identity;
using System.Configuration;
using Project.ViewModel;
namespace Project.Controllers
{
    public class OrdersController : Controller
    {
        ShopDBContext db=new ShopDBContext();
        // GET: Order
        [HttpPost]
        public ActionResult Order(float price,RegisterVM req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            Order Orderr = new Order();
            string curentUserID = User.Identity.GetUserId();
            List<Cart> Carts = db.Carts.Where(t => t.IdUser == curentUserID).ToList();
            //var code = new { Success = false};
            if (Carts!=null && Carts.Any())
            {
                Orderr.DateBooking = DateTime.Now;
                Orderr.IdUser = curentUserID;
                Orderr.Total = price;
                Orderr.Status = 0;
                if (Orderr.Total<500000)
                {
                    Orderr.Ship = 30000;
                }
                Orderr.TotalPrice = price+Orderr.Ship;
                db.Orders.Add(Orderr);
                db.SaveChanges();
                foreach (var item in Carts)
                {
                    OrderDetail Detail = new OrderDetail();
                    double ThanhTien = item.Quantity * item.Price;
                    Detail.IdOrder = Orderr.Id;
                    Detail.IdProductt = item.IdProduct;
                    Detail.Count = item.Quantity;
                    Detail.Price = item.Price;
                    Detail.Thanhtien = ThanhTien;
                    db.OrderDetails.Add(Detail);
                    db.Carts.Remove(item);
                    code = new { Success = true, Code = req.TypePayment=2, Url = "" };
                    //var url = "";
                    if (req.TypePayment == 2)
                    {
                        var url = UrlPayment(req.TypePaymentVN, Orderr.Id);
                        code = new { Success = true, Code = req.TypePayment, Url = url };
                    }
                }                
                db.SaveChanges();
                //code = new { Success = true};
            }
            return Json(code);
        }
        public ActionResult OrderSucces()
        {
            return View();
        }
        public ActionResult OrderDetail(int id)
        {
            ViewBag.Order=db.Orders.Where(t=>t.Id==id).FirstOrDefault();
            List<OrderDetail> OrderDetails=db.OrderDetails.Where(t=>t.IdOrder == id).ToList(); 
            return View(OrderDetails);
        }

        //vnpay
        #region Thanh toán vnpay
        public string UrlPayment(int TypePaymentVN, int orderCode)
        {
            var urlPayment = "";
            var order = db.Orders.FirstOrDefault(x => x.Id == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalPrice * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            //vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Id);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Id); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion  
    }
}