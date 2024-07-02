using System.Collections.Generic;

namespace Alpha.API.Dtos
{
    public class ModulesCompositeDto
    {
        public int ModuleCompositeId { get; set; }
        public string CompositeName { get; set; }
        public string Description { get; set; }
        public List<ModuleCompositeDetailDto> ModuleCompositeDetails { get; set; }
    }
}
