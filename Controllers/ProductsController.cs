using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;
using Project.Filters;

namespace Project.Controllers
{
    public class ProductsController : Controller
    {
        ShopDBContext DB = new ShopDBContext();

        // GET: Products
        [MyActionFilters]
        [OutputCache(Duration = 1000)]//Thời gian để chờ load lại trang
        public ActionResult Index(string Search = "", int? Sort = 0, int? Brand = 0, int? CaregoryID = 0, int page = 1, float Price = 0)
        {
            List<Product> Products = DB.Products.Where(t => t.Name.Contains(Search)).ToList();
            ViewBag.Search = Search;
            ViewBag.Price = Price;
            switch (Sort)
            {
                case 1:
                    Products = Products.OrderBy(T => T.Price).ToList();
                    break;
                case 2:
                    Products = Products.OrderByDescending(T => T.Price).ToList();
                    break;
                case 3:
                    Products = Products.OrderBy(T => T.Name).ToList();
                    break;
                case 4:
                    Products = Products.OrderByDescending(T => T.Name).ToList();
                    break;
                case 5:
                    Products = Products.OrderByDescending(T => T.DateCreate).ToList();
                    break;
                case 6:
                    Products = Products.OrderBy(T => T.DateCreate).ToList();
                    break;
                case 7:
                    Products = Products.OrderByDescending(T => T.TotalSold).ToList();
                    break;
                case 8:
                    Products = Products.OrderByDescending(T => T.TotalSold).ToList();
                    break;
            }

            if (CaregoryID != 0)
            {
                Products = Products.Where(t => t.ProductType.CategoryID == CaregoryID).ToList();
            }
            if (Brand != 0)
            {
                Products = Products.Where(t => t.BrandID == Brand).ToList();
            }
            if (Price != 0)
            {
                Products = Products.Where(t => t.Price == Price).ToList();
            }
            //Paging
            ViewBag.Sort = Sort;
            int NoOfrecordPerPage = 12;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Products.Count) / Convert.ToDouble(NoOfrecordPerPage)));
            int NoOfRecordToSkip = (page - 1) * NoOfrecordPerPage;
            ViewBag.Page = page;
            ViewBag.NoOfPages = NoOfPages;
            Products = Products.Skip(NoOfRecordToSkip).Take(NoOfrecordPerPage).ToList();
            return View(Products);
        }
        [OutputCache(Duration = 1000)]//Thời gian để chờ load lại trang

        public ActionResult Detail(string id)
        {
            Product product = DB.Products.Find(id);
            //Specification specification=DB.Specifications.Find(id);
            List<Specification> specifications = DB.Specifications.Where(t => t.ProductId == id).ToList();
            ViewBag.AllImg = DB.Imges.Where(t => t.ProductId == id).ToList();
            if (product != null)
            {
                if (product.Promotion > 0)
                {
                    ViewBag.Price = product.Price * (1 - product.Promotion * 0.01);
                }
                else
                {
                    ViewBag.Price = product.Price;
                }
            }
            ViewBag.ProductsSame = DB.Products.Where(t => t.ProductType.Id == product.ProductTypeID).ToList();
            ViewBag.Property = DB.Properties.Where(t => t.ProductId == id).ToList();
            //return View(product);
            var viewModel = new ProductSpecificationViewModel
            {
                Product = product,
                Specification = specifications
            };

            // Trả về view và chuyển model là đối tượng ProductSpecificationViewModel.
            return View(viewModel);
        }

        [ChildActionOnly]
        public ActionResult DisPlaySingleProduct(Product p)
        {
            return PartialView("Product", p);
        }
        [HttpPost]

        public ActionResult FilterProductsByPrice(string priceRange)
        {
            var filteredProducts = GetFilteredProductsByPrice(priceRange);

            // Kiểm tra xem danh sách sản phẩm sau khi lọc có trống không
            if (!filteredProducts.Any())
            {
                // Nếu danh sách trống, trả về PartialView chứa thông báo
                return PartialView("NoProductsFound");
            }

            float parsedMinPrice, parsedMaxPrice;

            // Assuming priceRange is in the format "min-max"
            string[] priceRangeArray = priceRange.Split('-');

            if (priceRangeArray.Length == 2 &&
                float.TryParse(priceRangeArray[0], out parsedMinPrice) &&
                float.TryParse(priceRangeArray[1], out parsedMaxPrice))
            {
                List<Product> products_price = DB.Products
                    .Where(t => t.Price >= parsedMinPrice && t.Price <= parsedMaxPrice)
                    .ToList();

                // Assuming PartialView "Product" expects a List<Product> as its model
                return PartialView("Product", products_price);
            }
            else
            {
                // Handle the case where priceRange is not in the expected format
                return PartialView("InvalidInput");
            }
        }

        private IQueryable<Product> GetFilteredProductsByPrice(string priceRange)
        {
            if (!string.IsNullOrEmpty(priceRange))
            {
                var priceBounds = priceRange.Split('-').Select(int.Parse).ToArray();

                if (priceBounds.Length == 2)
                {
                    // Sử dụng biến trung gian
                    var lowerBound = priceBounds[0];
                    var upperBound = priceBounds[1];

                    return DB.Products.Where(p => p.Price >= lowerBound && p.Price < upperBound);
                }
            }
            return DB.Products;
        }
    }
}