using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.MetadataModels
{
    public class UserRoleMetadata
    {
        [Key]
        [ScaffoldColumn(false)]
        public int UserRoleID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(50, ErrorMessage = "{0} حداکثر می تواند 50 کاراکتر باشد.")]
        [Display(Name = "عنوان انگلیسی")]
        [DisplayName("عنوان انگلیسی")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(50, ErrorMessage = "{0} حداکثر می تواند 50 کاراکتر باشد.")]
        [RegularExpression("^[\u0600 -\u06FF ]+$", ErrorMessage = "لطفا {0} را فارسی وارد کنید.")]
        [Display(Name = "عنوان فارسی")]
        [DisplayName("عنوان فارسی")]
        public string TitleFa { get; set; }

        [Display(Name = "وضعیت")]
        [DisplayName("وضعیت")]
        public bool IsEnabled { get; set; }

        [Display(Name = "تاریخ ثبت")]
        [DisplayName("تاریخ ثبت")]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        [Display(Name = "ثبت شده توسط")]
        [DisplayName("ثبت شده توسط")]
        public int CreatedBy { get; set; }

        [Display(Name = "تاریخ آخرین بروزرسانی")]
        [DisplayName("تاریخ آخرین بروزرسانی")]
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        [Display(Name = "بروزرسانی شده توسط")]
        [DisplayName("بروزرسانی شده توسط")]
        public Nullable<int> UpdatedBy { get; set; }
    }
}
namespace ArkaApp.Models.EntityModels
{
    [MetadataType(typeof(MetadataModels.UserRoleMetadata))]
    public partial class UserRole
    {

    }
}