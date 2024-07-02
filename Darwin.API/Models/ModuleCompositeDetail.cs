using System;
using System.Collections.Generic;

namespace Darwin.API.Models;

public partial class ModuleCompositeDetail
{
    public int ModuleCompositeDetailId { get; set; }

    public int? ModuleCompositeId { get; set; }

    public int? ModuleId { get; set; }

    public double Quantity { get; set; }

    public virtual Module? Module { get; set; }

    public virtual ModulesComposite? ModuleComposite { get; set; }
}
