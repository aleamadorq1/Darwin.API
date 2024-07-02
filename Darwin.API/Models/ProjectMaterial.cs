using System;
using System.Collections.Generic;

namespace Darwin.API.Models;

public partial class ProjectMaterial
{
    public int ProjectMaterialId { get; set; }

    public int ProjectId { get; set; }

    public int MaterialId { get; set; }

    public double UnitPrice { get; set; }

    public string TaxStatus { get; set; } = null!;

    public double CifPrice { get; set; }

    public DateTime LastModified { get; set; }

    public int Quantity { get; set; }

    public int? ModuleId { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual Module? Module { get; set; }

    public virtual Project Project { get; set; } = null!;
}
