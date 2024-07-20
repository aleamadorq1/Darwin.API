namespace Darwin.API.Models;

public partial class Organization
{
    public int OrganizationId { get; set; }

    public string OrganizationName { get; set; } = null!;

    public DateTime LastModified { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
