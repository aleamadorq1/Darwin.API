using System;
using System.Collections.Generic;

namespace Alpha.API.Models;

public partial class ModulesComposite
{
    public int ModuleCompositeId { get; set; }

    public string CompositeName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<ModuleCompositeDetail> ModuleCompositeDetails { get; set; } = new List<ModuleCompositeDetail>();

    public virtual ICollection<ProjectModuleComposite> ProjectModuleComposites { get; set; } = new List<ProjectModuleComposite>();
}
