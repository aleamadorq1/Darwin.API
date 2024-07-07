using System;
namespace Darwin.API.Dtos
{
    public class ModuleDto
    {
        public int ModuleId { get; set; }
        public int ProjectId { get; set; }
        public string ModuleName { get; set; }
        public string? ModuleSystem { get; set; }
        public int SystemId { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        public DateTime? LastModified { get; set; }


        public List<ModuleMaterialsDto>? ModuleMaterials { get; set; }
        public List<ModulesLaborDto>? ModuleLabors { get; set; }
    }
}

