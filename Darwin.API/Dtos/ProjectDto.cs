namespace Darwin.API.Dtos
{
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientId { get; set; }
        public string? ClientName { get; set; }
        public double TotalArea { get; set; }
        public int? TotalFloors { get; set; }
        public string Location { get; set; }
        public string LocationAddress { get; set; }
        public string LocationCoordinates { get; set; }
        public double ProfitMargin { get; set; }
        public int? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
