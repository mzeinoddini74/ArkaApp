using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.MetadataModels
{
    public class FollowerLogMetadata
    {
        [Key]
        [ScaffoldColumn(false)]
        public int FollowerLogID { get; set; }

        [Display(Name = "حساب")]
        [DisplayName("حساب")]
        public Nullable<int> AccountID { get; set; }

        [Display(Name = "لطفا استان(ها) را انتخاب کنید.")]
        [DisplayName("لطفا استان(ها) را انتخاب کنید.")]
        public string Geo { get; set; }

        [Display(Name = "اولویت")]
        [DisplayName("اولویت")]
        public Nullable<short> Priority { get; set; }

        [Display(Name = "جنسیت")]
        [DisplayName("جنسیت")]
        public Nullable<short> Gender { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage ="لطفا فقط عدد وارد کنید.")]
        [Display(Name = "تعداد درخواستی")]
        [DisplayName("تعداد درخواستی")]
        public Nullable<long> Count { get; set; }

        [Display(Name = "تعداد قبلی")]
        [DisplayName("تعداد قبلی")]
        public Nullable<long> PreviousCount { get; set; }

        [Display(Name = "تاریخ ثبت")]
        [DisplayName("تاریخ ثبت")]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        [Display(Name = "آیا فرایند پایان یافته است؟")]
        [DisplayName("آیا فرایند پایان یافته است؟")]
        public Nullable<bool> IsFinished { get; set; }

    }
}
namespace ArkaApp.Models.EntityModels
{
    [MetadataType(typeof(MetadataModels.FollowerLogMetadata))]
    public partial class FollowerLog
    {

    }
}