namespace Darwin.API.Models
{
    public class TaxRateDto
    {
        public int TaxRateId { get; set; }
        public double Rate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}