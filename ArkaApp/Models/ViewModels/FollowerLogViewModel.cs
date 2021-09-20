using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.ViewModels
{
    public class FollowerLogViewModel
    {
        public EntityModels.FollowerLog Log { get; set; }
        public List<EntityModels.Province> Provinces { get; set; }
        public List<EntityModels.Province> Lakes { get; set; }
        public List<EntityModels.Province> Islands { get; set; }
        public List<EntityModels.Province> Seas { get; set; }

    }
}