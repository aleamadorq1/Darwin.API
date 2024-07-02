using System;
using System.Collections.Generic;

namespace Darwin.API.Models;

public partial class Material
{
    public int MaterialId { get; set; }

    public string Sku { get; set; } = null!;

    public string MaterialName { get; set; } = null!;

    public int CategoryId { get; set; }

    public double UnitPrice { get; set; }

    public string TaxStatus { get; set; } = null!;

    public int SupplierId { get; set; }

    public double? CifPrice { get; set; }

    public string Uom { get; set; } = null!;

    public DateTime? LastModified { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ModulesMaterial> ModulesMaterials { get; set; } = new List<ModulesMaterial>();

    public virtual ICollection<ProjectMaterial> ProjectMaterials { get; set; } = new List<ProjectMaterial>();

    public virtual Supplier Supplier { get; set; } = null!;
}
