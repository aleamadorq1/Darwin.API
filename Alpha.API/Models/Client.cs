using System;
using System.Collections.Generic;

namespace Alpha.API.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public string ClientName { get; set; } = null!;

    public string? ContactInfo { get; set; }

    public string? Address { get; set; }

    public DateTime? LastModified { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
