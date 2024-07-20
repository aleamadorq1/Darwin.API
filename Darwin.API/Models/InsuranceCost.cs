namespace Darwin.API.Models;

public partial class InsuranceCost
{
    public int InsuranceCostId { get; set; }

    public double Cost { get; set; }

    public string Description { get; set; } = null!;

    public DateTime LastModified { get; set; }
}
