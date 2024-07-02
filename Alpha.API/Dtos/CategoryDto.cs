﻿using System;
namespace Alpha.API.Dtos
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
