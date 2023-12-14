using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Web.Mvc;
namespace Project.Models
{
    public class Specification
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Mã sản phẩm")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "varchar")]
        [StringLength(40)]
        public string ProductId { get; set; }//Mã sản phẩm
        [Column(TypeName = "nvarchar")]
        [Display(Name = "Màn hình")]
        public string Screen { get; set; }//Màn hình
        [Column(TypeName = "nvarchar")]
        [Display(Name = "Camera sau")]
        public string Camera { get; set; }//Camera sau
        [Column(TypeName = "nvarchar")]
        [Display(Name = "Camera Selfie")]
        public string CameraSelfie { get; set; }//Camera Selfile
        [Column(TypeName = "nvarchar")]
        [Display(Name = "Khối lượng")]
        public string Mass { get; set; }//khối lượng
        public int Ram { get; set; }
        public int Rom { get; set; }
        [Column(TypeName = "nvarchar")]
        [Display(Name = "Chất liệu")]
        public string Material { get; set; }//Chất liệu
        [Column(TypeName = "nvarchar")]
        [Display(Name = "CPU")]
        public string CPU { get; set; }//CPU
        [Column(TypeName = "nvarchar")]
        [Display(Name = "Dung lượng pin")]
        public string Battery { get; set; }//Pin
        [Column(TypeName = "nvarchar")]
        [Display(Name = "Xuất xứ")]
        public string Origin { get; set; }//Xuất xứ
        [ForeignKey("ProductId")]
        public Product product { get; set; }
    }
}