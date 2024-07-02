using System;
namespace Darwin.API.Dtos
{
	public class LaborIndexDto
	{
        public long LaborId { get; set; }

        public string? LaborType { get; set; }

        public double? HourlyRate { get; set; }

        public string? Description { get; set; }

        public DateTime? LastModified { get; set; }
    }
}

