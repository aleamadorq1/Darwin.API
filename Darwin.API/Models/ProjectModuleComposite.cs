using System;
using System.Collections.Generic;

namespace Darwin.API.Models;

public partial class ProjectModuleComposite
{
    public int ProjectModuleCompositeId { get; set; }

    public int ProjectId { get; set; }

    public int ModuleCompositeId { get; set; }

    public double Quantity { get; set; }

    public virtual ModulesComposite ModuleComposite { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
