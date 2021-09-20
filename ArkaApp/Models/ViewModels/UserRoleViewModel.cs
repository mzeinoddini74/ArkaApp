using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.ViewModels
{
    public class UserRoleViewModel
    {
        public EntityModels.UserRole UserRole { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}