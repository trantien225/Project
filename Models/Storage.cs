using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class Storage
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Mã Sản Phẩm")]
        public string ProductId { get; set; }
        [Display(Name = "Số lượng")]
        public string Quantity { get; set; }
        [Display(Name = "Giá nhập")]
        [DisplayFormat(DataFormatString = "{0:0,0 đ}")]
        public Nullable<double> Price { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yy}")]
        [Display(Name = "Ngày nhập sản phẩm")]
        public Nullable<System.DateTime> DateCreate { get; set; }
        public string Unit { get; set; }
        [ForeignKey("ProductId")]
        public Product product { get; set; }
    }
}