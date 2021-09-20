//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ArkaApp.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class TelegramContentLog
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TelegramContentLog()
        {
            this.TelegramContentLogDetail = new HashSet<TelegramContentLogDetail>();
        }
    
        public int TelegramContentLogID { get; set; }
        public Nullable<int> ContentID { get; set; }
        public Nullable<long> Count { get; set; }
        public string Regions { get; set; }
        public string GroupTitle { get; set; }
        public string Geo { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<bool> IsFinished { get; set; }
        public Nullable<System.DateTime> FinishedDate { get; set; }
    
        public virtual Content Content { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TelegramContentLogDetail> TelegramContentLogDetail { get; set; }
    }
}