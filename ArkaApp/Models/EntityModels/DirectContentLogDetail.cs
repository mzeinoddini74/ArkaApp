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
    
    public partial class DirectContentLogDetail
    {
        public int DirectContentLogDetailID { get; set; }
        public Nullable<int> DirectContentLogID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<long> Count { get; set; }
    
        public virtual DirectContentLog DirectContentLog { get; set; }
    }
}
