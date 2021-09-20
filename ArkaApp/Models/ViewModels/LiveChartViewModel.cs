using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.ViewModels
{
    public class LiveChartViewModel
    {
        public List<LineChartViewModel> Chart { get; set; }
        public bool IsActive { get; set; }
    }

}