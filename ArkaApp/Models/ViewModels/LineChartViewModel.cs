using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Models.ViewModels
{
    public class LineChartViewModel
    {
        public string label { get; set; }
        public long value { get; set; }

        public LineChartViewModel(string label, long value)
        {
            this.label = label;
            this.value = value;
        }
        public LineChartViewModel()
        {
        }
    }

}