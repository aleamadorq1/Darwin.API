using System;
namespace Darwin.API.Dtos
{
    public class MaterialDto
    {
        public int MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public string Sku { get; set; }
        public string? Category { get; set; }
        public int CategoryId { get; set; }
        public string? Supplier { get; set; }
        public int SupplierId { get; set; }
        public double UnitPrice { get; set; }
        public string Uom { get; set; }
        public string TaxStatus { get; set; }
        public double? CifPrice { get; set; }
    }
}

