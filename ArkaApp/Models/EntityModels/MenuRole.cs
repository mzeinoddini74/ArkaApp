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
    
    public partial class MenuRole
    {
        public int MenuRoleID { get; set; }
        public int MenuID { get; set; }
        public int UserRoleID { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    
        public virtual Menu Menu { get; set; }
        public virtual UserRole UserRole { get; set; }
    }
}
