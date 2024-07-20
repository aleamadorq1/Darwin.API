namespace Darwin.API.Models;

public partial class System
{
    public int SystemId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
}
