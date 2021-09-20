using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.MetadataModels
{
    public class ContentMetadata
    {
        [Key]
        [ScaffoldColumn(false)]

        public int ContentID { get; set; }


        [Display(Name = "حساب")]
        [DisplayName("حساب")]

        public Nullable<int> AccountID { get; set; }


        [Display(Name = "توضیحات")]
        [DisplayName("توضیحات")]

        public string Description { get; set; }


        [Display(Name = "فایل")]
        [DisplayName("فایل")]

        public string URL { get; set; }


        [Display(Name = "تاریخ ثبت")]
        [DisplayName("تاریخ ثبت")]
        public System.DateTime CreatedDate { get; set; }

    }
}
namespace ArkaApp.Models.EntityModels
{
    [MetadataType(typeof(MetadataModels.ContentMetadata))]
    public partial class Content
    {

    }
}