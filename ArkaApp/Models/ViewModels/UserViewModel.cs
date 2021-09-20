using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArkaApp.Models.ViewModels
{
    public class UserViewModel
    {
        public EntityModels.User User { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class UserEditViewModel
    {
        public int UserID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(200, ErrorMessage = "{0} حداکثر می تواند 200 کاراکتر باشد.")]
        [RegularExpression("^[\u0600 -\u06FF ]+$", ErrorMessage = "لطفا {0} را فارسی وارد کنید.")]
        [Display(Name = "نام و نام خانوادگی")]
        [DisplayName("نام و نام خانوادگی")]
        public string FullName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(200, ErrorMessage = "{0} حداکثر می تواند 200 کاراکتر باشد.")]
        [EmailAddress(ErrorMessage = "{0} را صحیح وارد کنید.")]
        [Display(Name = "آدرس پست الکترونیک")]
        [DisplayName("آدرس پست الکترونیک")]
        public string EmailAddress { get; set; }

        [Display(Name = "تصویر")]
        [DisplayName("تصویر")]
        public string ProfilePicture { get; set; }

        [Display(Name = "وضعیت")]
        [DisplayName("وضعیت")]
        public bool IsEnabled { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "نقش کاربر")]
        [DisplayName("نقش کاربر")]
        public int UserRoleID { get; set; }

        [DisplayName("شماره موبایل")]
        [Display(Name = "شماره موبایل")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [RegularExpression(@"^0?9[123]\d{8}$", ErrorMessage = "{0} را بدرستی وارد کنید.")]
        [StringLength(11, ErrorMessage = "{0} باید 11 کاراکتر باشد.", MinimumLength = 11)]
        public string MobileNumber { get; set; }

    }

    public class UserSuperEditViewModel
    {
        public int UserID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "رمزعبور")]
        [DisplayName("رمزعبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(200, ErrorMessage = "{0} حداکثر می تواند 200 کاراکتر باشد.")]
        [Display(Name = "نام کاربری")]
        [DisplayName("نام کاربری")]
        public string UserName { get; set; }
        public string ProfilePicture { get; set; }
        public string FullName { get; set; }

    }

    public class UserChangePasswordViewModel
    {
        public int UserID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "رمزعبور")]
        [DisplayName("رمزعبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "تکرار رمزعبور")]
        [DisplayName("تکرار رمزعبور")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="تکرار رمزعبور مطابقت ندارد.")]
        public string RePassword { get; set; }
    }

}