using System;
namespace Darwin.API.Dtos
{
    public class ProjectLaborDto
    {
        public int ModuleLaborId { get; set; }
        public int ProjectLaborId { get; set; }
        public int ProjectId { get; set; }
        public int? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public int LaborId { get; set; }
        public string? LaborType { get; set; }
        public double Quantity { get; set; }
        public double? HourlyRate { get; set; }
        public double? AllowanceAmount { get; set; }

        public double? AllowanceQuantity { get; set; }
    }
}

