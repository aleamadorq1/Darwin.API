namespace Alpha.API.Dtos
{
    public class ProjectMaterialDto
    {
        public int? ProjectMaterialId { get; set; }

        public int ProjectId { get; set; }

        public int MaterialId { get; set; }

        public double UnitPrice { get; set; }

        public string TaxStatus { get; set; } = null!;

        public double CifPrice { get; set; }

        public DateTime? LastModified { get; set; }

        public int Quantity { get; set; }

        public int? ModuleId { get; set; }

    }

}