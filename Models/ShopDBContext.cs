using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Project.Models;
using Project.ViewModel;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Project.Models
{
    public class ShopDBContext : IdentityDbContext<AppUser>
    {
        public ShopDBContext() : base("MyConnectionString") { }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<ImgeProduct> Imges { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<PropertieProduct> Properties { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public List<GetReportProductByYear> GetReportProductByYear(int year)
        {
            string sql = "EXEC GetReportProductByYear @Year";
            var parameters = new SqlParameter[]
            {
            new SqlParameter("@Year", year)

            };
            return Database.SqlQuery<GetReportProductByYear>(sql, parameters).ToList();
        }

    }
}