namespace Darwin.API.Models;

public partial class DistributionCenter
{
    public int DistributionCenterId { get; set; }

    public string Name { get; set; } = null!;

    public string LocationAddress { get; set; } = null!;

    public string LocationCoordinates { get; set; } = null!;

    public DateTime LastModified { get; set; }

    public string Location { get; set; } = null!;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
