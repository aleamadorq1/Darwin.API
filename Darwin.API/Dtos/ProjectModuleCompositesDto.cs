namespace Darwin.API.Dtos
{
    public partial class ProjectModuleCompositesDto
    {
        public int ProjectModuleCompositeId { get; set; }
        public int ProjectId { get; set; }
        public int ModuleCompositeId { get; set; }
        public string? CompositeName { get; set; } = null!;

        public int Quantity { get; set; }

        public List<ModuleCompositeDetailDto>? CompositeDetails { get; set; }

    }

}