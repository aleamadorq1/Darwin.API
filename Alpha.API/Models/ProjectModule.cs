using System;
using System.Collections.Generic;

namespace Alpha.API.Models;

public partial class ProjectModule
{
    public int ProjectModuleId { get; set; }

    public int ProjectId { get; set; }

    public int ModuleId { get; set; }

    public int Quantity { get; set; }

    public virtual Module Module { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
