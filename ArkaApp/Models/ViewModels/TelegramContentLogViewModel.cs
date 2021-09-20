using System.Collections.Generic;

namespace ArkaApp.Models.ViewModels
{
    public class TelegramContentLogViewModel
    {
        public EntityModels.TelegramContentLog Log { get; set; }
        public List<EntityModels.Province> Provinces { get; set; }
        public List<EntityModels.Province> Lakes { get; set; }
        public List<EntityModels.Province> Islands { get; set; }
        public List<EntityModels.Province> Seas { get; set; }
    }
}