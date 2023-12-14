using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
namespace Project.Models
{
    public class AppUserStore:UserStore<AppUser>
    {
        public AppUserStore(ShopDBContext dBContext) : base(dBContext) { }
    }
}