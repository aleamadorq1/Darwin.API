using System;
namespace Darwin.API.Dtos
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public int? ParentCategoryId { get; set; }
    }
}

