using System;
using System.Collections.Generic;

namespace Darwin.API.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string ContactInfo { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateTime? LastModified { get; set; }

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
