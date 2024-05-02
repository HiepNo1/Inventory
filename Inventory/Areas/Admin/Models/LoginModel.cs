using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inventory.Areas.Admin.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage ="Vui lòng nhập user name")]
        public string UserName { set; get; }

        [Required(ErrorMessage = "Vui lòng nhập password")]
        public string PassWord { set; get; }

        public bool Remember { set; get; }
    }
}