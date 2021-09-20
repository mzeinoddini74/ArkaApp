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
    
    public partial class DirectContentLog
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DirectContentLog()
        {
            this.DirectContentLogDetail = new HashSet<DirectContentLogDetail>();
        }
    
        public int DirectContentLogID { get; set; }
        public Nullable<int> ContentID { get; set; }
        public string Geo { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<bool> IsFinished { get; set; }
        public Nullable<System.DateTime> FinishedDate { get; set; }
        public Nullable<short> Gender { get; set; }
        public Nullable<long> Count { get; set; }
    
        public virtual Content Content { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DirectContentLogDetail> DirectContentLogDetail { get; set; }
    }
}
