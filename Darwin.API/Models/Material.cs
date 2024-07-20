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

    public int TaxRateId { get; set; }

    public int SupplierId { get; set; }

    public int HandlingCostId { get; set; }

    public double CifPrice { get; set; }

    public string Uom { get; set; } = null!;

    public DateTime LastModified { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual HandlingCost HandlingCost { get; set; } = null!;

    public virtual ICollection<ModulesMaterial> ModulesMaterials { get; set; } = new List<ModulesMaterial>();

    public virtual ICollection<ProjectMaterial> ProjectMaterials { get; set; } = new List<ProjectMaterial>();

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual TaxRate TaxRate { get; set; } = null!;
}
