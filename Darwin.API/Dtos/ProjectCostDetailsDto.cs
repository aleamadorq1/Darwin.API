namespace Darwin.API.Dtos;

public class ProjectCostDetailsDto
{
    public int ProjectId { get; set; }
    
    public decimal TotalCost { get; set; }
    public double ProfitMargin { get; set; }
    public List<ProjectModuleCompositesDto> ModulesComposite { get; set; }
    public List<ProjectModuleDto> Modules { get; set; }
    public ProjectModuleDto? ParentLessCosts { get; set; }
}