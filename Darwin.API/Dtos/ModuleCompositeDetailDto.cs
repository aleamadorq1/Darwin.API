using System.Reflection;

namespace Darwin.API.Dtos
{
    public class ModuleCompositeDetailDto
    {
        public int ModuleCompositeDetailId { get; set; }
        public int? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public double Quantity { get; set; }
        public ProjectModuleDto? Module { get; set; }
    }
}
