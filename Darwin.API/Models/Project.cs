using System;
using System.Collections.Generic;

namespace Darwin.API.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int ClientId { get; set; }

    public double TotalArea { get; set; }

    public int? TotalFloors { get; set; }

    public string Location { get; set; } = null!;

    public double ProfitMargin { get; set; }

    public int? OrganizationId { get; set; }

    public DateTime LastModified { get; set; }

    public string LocationAddress { get; set; } = null!;

    public string LocationCoordinates { get; set; } = null!;

    public int? DistributionCenterId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual DistributionCenter? DistributionCenter { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<ProjectLabor> ProjectLabors { get; set; } = new List<ProjectLabor>();

    public virtual ICollection<ProjectMaterial> ProjectMaterials { get; set; } = new List<ProjectMaterial>();

    public virtual ICollection<ProjectModuleComposite> ProjectModuleComposites { get; set; } = new List<ProjectModuleComposite>();

    public virtual ICollection<ProjectModule> ProjectModules { get; set; } = new List<ProjectModule>();
}
