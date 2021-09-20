using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.MetadataModels
{
    public class AccountMetadata
    {
        [Key]
        [ScaffoldColumn(false)]

        public int AccountID { get; set; }
        [Display(Name = "کاربر")]
        [DisplayName("کاربر")]
        public int UserID { get; set; }

        [Display(Name = "تصویر")]
        [DisplayName("تصویر")]
        public string ProfilePicture { get; set; }

        [Display(Name = "کد حساب")]
        [DisplayName("کد حساب")]
        public string AccountPK { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(500, ErrorMessage = "{0} حداکثر می تواند 500 کاراکتر باشد.")]
        [Display(Name = "نام کاربری")]
        [DisplayName("نام کاربری")]
        public string AccountUserName { get; set; }

        [Display(Name = "وضعیت")]
        [DisplayName("وضعیت")]
        public bool IsEnabled { get; set; }


        [Display(Name = "حساب پیش فرض باشد؟")]
        [DisplayName("حساب پیش فرض باشد؟")]
        public Nullable<bool> IsDefault { get; set; } 

        [Display(Name = "تاریخ ثبت")]
        [DisplayName("تاریخ ثبت")]
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
namespace ArkaApp.Models.EntityModels
{
    [MetadataType(typeof(MetadataModels.AccountMetadata))]
    public partial class Account
    {

    }
}