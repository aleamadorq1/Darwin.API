using System;
using System.Collections.Generic;

namespace Alpha.API.Models;

public partial class Labor
{
    public int LaborId { get; set; }

    public string LaborType { get; set; } = null!;

    public double HourlyRate { get; set; }

    public string Description { get; set; } = null!;

    public DateTime? LastModified { get; set; }

    public virtual ICollection<ModulesLabor> ModulesLabors { get; set; } = new List<ModulesLabor>();

    public virtual ICollection<ProjectAllowance> ProjectAllowances { get; set; } = new List<ProjectAllowance>();

    public virtual ICollection<ProjectLabor> ProjectLabors { get; set; } = new List<ProjectLabor>();
}
