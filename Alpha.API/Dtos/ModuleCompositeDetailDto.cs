﻿namespace Alpha.API.Dtos
{
    public class ModuleCompositeDetailDto
    {
        public int ModuleCompositeDetailId { get; set; }
        public int? ModuleId { get; set; }
        public string ModuleName { get; set; }
        public double Quantity { get; set; }
    }
}