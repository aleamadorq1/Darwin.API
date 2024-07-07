﻿using System;
namespace Darwin.API.Dtos
{
    public class ModulesLaborDto
    {
        public int ModuleLaborId { get; set; }
        public int ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public int LaborId { get; set; }
        public string? LaborType { get; set; }
        public double? HoursRequired { get; set; }
        public double? HourlyRate { get; set; }
    }
}

