namespace Darwin.API.Models;

public partial class Module
{
    public int ModuleId { get; set; }

    public string ModuleName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime? LastModified { get; set; }

    public int SystemId { get; set; }

    public virtual ICollection<ModuleCompositeDetail> ModuleCompositeDetails { get; set; } = new List<ModuleCompositeDetail>();

    public virtual ICollection<ModulesLabor> ModulesLabors { get; set; } = new List<ModulesLabor>();

    public virtual ICollection<ModulesMaterial> ModulesMaterials { get; set; } = new List<ModulesMaterial>();

    public virtual ICollection<ProjectLabor> ProjectLabors { get; set; } = new List<ProjectLabor>();

    public virtual ICollection<ProjectMaterial> ProjectMaterials { get; set; } = new List<ProjectMaterial>();

    public virtual ICollection<ProjectModule> ProjectModules { get; set; } = new List<ProjectModule>();

    public virtual System System { get; set; } = null!;
}
