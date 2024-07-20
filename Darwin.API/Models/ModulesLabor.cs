namespace Darwin.API.Models;

public partial class ModulesLabor
{
    public int ModuleLaborId { get; set; }

    public int ModuleId { get; set; }

    public int LaborId { get; set; }

    public double HoursRequired { get; set; }

    public DateTime LastModified { get; set; }

    public int Quantity { get; set; }

    public virtual Labor Labor { get; set; } = null!;

    public virtual Module Module { get; set; } = null!;
}
