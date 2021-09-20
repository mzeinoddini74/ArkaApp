using System.Collections.Generic;

namespace ArkaApp.Models.ViewModels
{
    public class DirectContentLogViewModel
    {
        public EntityModels.DirectContentLog Log { get; set; }
        public int ContentID { get; set; }
        public List<EntityModels.Province> Provinces { get; set; }
        public List<EntityModels.Province> Lakes { get; set; }
        public List<EntityModels.Province> Islands { get; set; }
        public List<EntityModels.Province> Seas { get; set; }
    }
}