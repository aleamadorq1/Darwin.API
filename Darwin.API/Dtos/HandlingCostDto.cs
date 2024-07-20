namespace Darwin.API.Models
{
    public class HandlingCostDto
    {
        public int HandlingCostId { get; set; }
        public double Cost { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}