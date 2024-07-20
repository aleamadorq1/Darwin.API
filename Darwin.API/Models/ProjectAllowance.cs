namespace Darwin.API.Models;

public partial class ProjectAllowance
{
    public int ProjectAllowanceId { get; set; }

    public int ProjectLaborId { get; set; }

    public double Amount { get; set; }

    public double Quantity { get; set; }

    public DateTime LastModified { get; set; }

    public virtual ProjectLabor ProjectLabor { get; set; } = null!;
}
