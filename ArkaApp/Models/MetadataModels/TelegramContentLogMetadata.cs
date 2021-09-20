using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.MetadataModels
{
    public class TelegramContentLogMetadata
    {
        [Key]
        [ScaffoldColumn(false)]
        public int TelegramContentLogID { get; set; }

        [Display(Name = "محتوا")]
        [DisplayName("محتوا")]
        public Nullable<int> ContentID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "لطفا فقط عدد وارد کنید.")]
        [Display(Name = "تعداد درخواستی")]
        [DisplayName("تعداد درخواستی")]
        public Nullable<long> Count { get; set; }

        [Display(Name = "موقعیت جغرافیایی")]
        [DisplayName("موقعیت جغرافیایی")]
        public string Geo { get; set; }


        [Display(Name = "منطقه")]
        [DisplayName("منطقه")]
        public string Regions { get; set; }

        [Display(Name = "نوع فعالیت گروه")]
        [DisplayName("نوع فعالیت گروه")]
        public string GroupTitle { get; set; }


        [Display(Name = "تاریخ ثبت")]
        [DisplayName("تاریخ ثبت")]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        [Display(Name = "وضعیت")]
        [DisplayName("وضعیت")]
        public Nullable<bool> IsFinished { get; set; }

        [Display(Name = "تاریخ پایان")]
        [DisplayName("تاریخ پایان")]
        public Nullable<System.DateTime> FinishedDate { get; set; }
    }
}
namespace ArkaApp.Models.EntityModels
{
    [MetadataType(typeof(MetadataModels.TelegramContentLogMetadata))]
    public partial class TelegramContentLog
    {

    }
}