namespace Alpha.API.Dtos
{
    public class ProjectLaborDto
    {
        public int ProjectLaborId { get; set; }

        public int ProjectId { get; set; }

        public int LaborId { get; set; }

        public double HourlyRate { get; set; }

        public int Quantity { get; set; }

        public int? ModuleId { get; set; }

    }

}