using System;
using System.Collections.Generic;

namespace Alpha.API.Models;

public partial class ModulesMaterial
{
    public int ModuleMaterialId { get; set; }

    public int ModuleId { get; set; }

    public int MaterialId { get; set; }

    public double Quantity { get; set; }

    public DateTime LastModified { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual Module Module { get; set; } = null!;
}
