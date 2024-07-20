namespace Darwin.API.Dtos
{
    public class ModuleMaterialsDto
    {
        public int ModuleMaterialId { get; set; }
        public int? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public int MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public double Quantity { get; set; }
    }
}

