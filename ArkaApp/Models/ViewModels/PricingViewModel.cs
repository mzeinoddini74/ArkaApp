using System.Collections.Generic;

namespace ArkaApp.Models.ViewModels
{
    public class PricingViewModel
    {
        public Models.EntityModels.Pricing Pricing { get; set; }
        public List<Models.EntityModels.PricingDetail> Details { get; set; }
        public decimal Total { get; set; }
    }
}