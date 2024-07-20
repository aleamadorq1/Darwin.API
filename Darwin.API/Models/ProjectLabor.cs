namespace Darwin.API.Models;

public partial class ProjectLabor
{
    public int ProjectLaborId { get; set; }

    public int ProjectId { get; set; }

    public int LaborId { get; set; }

    public double HourlyRate { get; set; }

    public DateTime LastModified { get; set; }

    public int Quantity { get; set; }

    public int? ModuleId { get; set; }

    public double? HoursRequired { get; set; }

    public virtual Labor Labor { get; set; } = null!;

    public virtual Module? Module { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ProjectAllowance? ProjectAllowance { get; set; }
}
