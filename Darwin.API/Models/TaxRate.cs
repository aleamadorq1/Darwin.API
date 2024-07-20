namespace Darwin.API.Models;

public partial class TaxRate
{
    public int TaxRateId { get; set; }

    public double Rate { get; set; }

    public string Description { get; set; } = null!;

    public DateTime LastModified { get; set; }

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
