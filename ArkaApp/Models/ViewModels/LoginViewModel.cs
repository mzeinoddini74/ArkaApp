using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("نام کاربری")]
        [Display(Name = "نام کاربری")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(50, ErrorMessage = "{0} حداکثر می تواند 50 کاراکتر باشد.")]
        public string UserName { get; set; }

        [DisplayName("رمز عبور")]
        [Display(Name = "رمز عبور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "مرا به خاطر بسپار")]
        public bool Rememeber { get; set; }


    }
}