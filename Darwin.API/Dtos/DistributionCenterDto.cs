using System;
namespace Darwin.API.Dtos
{
    public class DistributionCenterDto
    {
        public int DistributionCenterId { get; set; }

        public string Name { get; set; } = null!;

        public string Location { get; set; } = null!;

        public string LocationAddress { get; set; } = null!;

        public string LocationCoordinates { get; set; } = null!;

        public DateTime? LastModified { get; set; }

    }
}

