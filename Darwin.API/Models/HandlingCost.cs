namespace Darwin.API.Models;

public partial class HandlingCost
{
    public int HandlingCostId { get; set; }

    public double Cost { get; set; }

    public string Description { get; set; } = null!;

    public DateTime LastModified { get; set; }

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
