namespace Darwin.API.Dtos
{
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public required string ProjectName { get; set; }
        public required string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientId { get; set; }
        public string? ClientName { get; set; }
        public double TotalArea { get; set; }
        public int? TotalFloors { get; set; }
        public int DistributionCenterId { get; set; }
        public required string Location { get; set; }
        public required string LocationAddress { get; set; }
        public required string LocationCoordinates { get; set; }
        public double ProfitMargin { get; set; }
        public int? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
