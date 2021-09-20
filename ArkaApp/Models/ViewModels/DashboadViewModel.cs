using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.ViewModels
{
    public class DashboadViewModel
    {
        public int AccountID { get; set; }
        public long FollowerCount { get; set; }
        public long PostCount { get; set; }
        public long Avg { get; set; }
        public List<MapChartViewModel> MapChart { get; set; }
        public List<EntityModels.Province> Provinces { get; set; }
    }
}