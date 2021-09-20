using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.MetadataModels
{
    public class ExploreLogMetadata
    {
        [Key]
        [ScaffoldColumn(false)]
        public int ExploreLogID { get; set; }

        [Display(Name = "پست")]
        [DisplayName("پست")]
        public int PostID { get; set; }

        [Display(Name = "تاریخ ثبت")]
        [DisplayName("تاریخ ثبت")]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "تاریخ شروع")]
        [DisplayName("تاریخ شروع")]

        public string FromDateFa { get; set; }

        [Display(Name = "تاریخ شروع میلادی")]
        [DisplayName("تاریخ شروع میلادی")]
        public Nullable<System.DateTime> FromDateEn { get; set; }

        [Display(Name = "تعداد روز")]
        [DisplayName("تعداد روز")]
        public Nullable<int> DateCount { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "تاریخ پایان")]
        [DisplayName("تاریخ پایان")]
        public string EndDateFa { get; set; }


        [Display(Name = "تاریخ پایان میلادی")]
        [DisplayName("تاریخ پایان میلادی")]
        public Nullable<System.DateTime> EndDateEn { get; set; }
    }
}
namespace ArkaApp.Models.EntityModels
{
    [MetadataType(typeof(MetadataModels.ExploreLogMetadata))]
    public partial class ExploreLog
    {

    }
}