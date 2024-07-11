using System;
using System.Collections.Generic;

namespace Darwin.API.Models;

public partial class ModuleAllowance
{
    public int ModuleAllowanceId { get; set; }

    public int ModuleId { get; set; }

    public int LaborId { get; set; }

    public double AllowanceAmount { get; set; }

    public DateTime? LastModified { get; set; }

    //public virtual Labor Labor { get; set; } = null!;

    public virtual Module Module { get; set; } = null!;
}
