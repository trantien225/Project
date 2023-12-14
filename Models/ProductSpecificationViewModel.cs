using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class ProductSpecificationViewModel
    {
        public Product Product { get; set; }
        public List<Specification> Specification { get; set; }
    }
}