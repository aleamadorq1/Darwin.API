using System;
using System.Collections.Generic;

namespace Darwin.API.Models;

public partial class ProjectAllowance
{
    public int AllowanceId { get; set; }

    public int ProjectId { get; set; }

    public int LaborId { get; set; }

    public double Amount { get; set; }

    public DateTime LastModified { get; set; }

    public virtual Labor Labor { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
