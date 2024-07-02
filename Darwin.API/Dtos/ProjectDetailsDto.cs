namespace Darwin.API.Dtos
{
    public partial class ProjectDetailsDto
    {
        public int ProjectId { get; set; }
        public IList<ProjectLaborDto>? ProjectLabor { get; set; }

        public IList<ProjectModuleDto>? ProjectModules { get; set; }

        public IList<ProjectMaterialDto>? ProjectMaterials { get; set; }

        public IList<ProjectModuleCompositesDto>? ProjectModuleComposites { get; set; }

    }

}