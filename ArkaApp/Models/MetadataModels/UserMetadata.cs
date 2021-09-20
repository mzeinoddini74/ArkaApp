using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.MetadataModels
{
    public class UserMetadata
    {
        [Key]
        [ScaffoldColumn(false)]
        public int UserID { get; set; }

        [DisplayName("نام و نام خانوداگی")]
        [Display(Name = "نام و نام خانوداگی")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [RegularExpression("^[\u0600 -\u06FF ]+$", ErrorMessage = "لطفا {0} را فارسی وارد کنید.")]
        [StringLength(200, ErrorMessage = "{0}می تواند حداقل 5 و حداکثر 200 کاراکتر باشد.", MinimumLength = 5)]
        public string FullName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(200, ErrorMessage = "{0} حداکثر می تواند 200 کاراکتر باشد.")]
        [EmailAddress(ErrorMessage = "{0} را صحیح وارد کنید.")]
        [Display(Name = "آدرس پست الکترونیک")]
        [DisplayName("آدرس پست الکترونیک")]

        public string EmailAddress { get; set; }

        [DisplayName("شماره موبایل")]
        [Display(Name = "شماره موبایل")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [RegularExpression(@"^0?9[123]\d{8}$", ErrorMessage = "{0} را بدرستی وارد کنید.")]
        [StringLength(11, ErrorMessage = "{0} باید 11 کاراکتر باشد.", MinimumLength = 11)]
        public string MobileNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(200, ErrorMessage = "{0} حداکثر می تواند 200 کاراکتر باشد.")]
        [Display(Name = "نام کاربری")]
        [DisplayName("نام کاربری")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "رمزعبور")]
        [DisplayName("رمزعبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string SaltPassword { get; set; }

        [Display(Name = "تصویر")]
        [DisplayName("تصویر")]
        public string ProfilePicture { get; set; }
        public string AuthToken { get; set; }

        [Display(Name = "وضعیت")]
        [DisplayName("وضعیت")]
        public bool IsEnabled { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "نقش کاربر")]
        [DisplayName("نقش کاربر")]
        public int UserRoleID { get; set; }

        [Display(Name = "تاریخ ثبت")]
        [DisplayName("تاریخ ثبت")]
        public System.DateTime CreatedDate { get; set; }

        [Display(Name = "ثبت شده توسط")]
        [DisplayName("ثبت شده توسط")]
        public Nullable<int> CreatedBy { get; set; }

        [Display(Name = "تاریخ آخرین بروزرسانی")]
        [DisplayName("Name")]
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        [Display(Name = "بروزرسانی شده توسط")]
        [DisplayName("بروزرسانی شده توسط")]
        public Nullable<int> UpdatedBy { get; set; }
    }
}
namespace ArkaApp.Models.EntityModels
{
    [MetadataType(typeof(MetadataModels.UserMetadata))]
    public partial class User
    {

    }
}