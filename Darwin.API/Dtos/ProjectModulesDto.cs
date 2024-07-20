namespace Darwin.API.Dtos
{
    public class ProjectModuleDto
    {
        public int? ModuleId { get; set; }
        public int ProjectModuleId { get; set;}
        public int ProjectId { get; set; }
        public string? ModuleName { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public double? Total { get; set; }
        public string? SystemName { get; set; }


        public List<ProjectMaterialDto>? ModuleMaterials { get; set; }
        public List<ProjectLaborDto>? ModuleLabors { get; set; }
    }
}

