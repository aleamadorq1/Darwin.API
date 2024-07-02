namespace Darwin.API.Dtos;

public class ProjectCostDetails
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}