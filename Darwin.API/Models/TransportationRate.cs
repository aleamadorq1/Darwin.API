namespace Darwin.API.Models;

public partial class TransportationRate
{
    public int RateId { get; set; }

    public double RatePerKm { get; set; }

    public string Description { get; set; } = null!;

    public DateTime LastModified { get; set; }
}
