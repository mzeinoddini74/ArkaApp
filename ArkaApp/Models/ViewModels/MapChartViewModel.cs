using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.ViewModels
{
    public class MapChartViewModel
    {
        public Models.EntityModels.Province Province { get; set; }
        public long Count { get; set; }
    }
}