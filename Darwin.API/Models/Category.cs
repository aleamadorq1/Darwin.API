namespace Darwin.API.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public int? ParentCategoryId { get; set; }

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    public virtual Category? ParentCategory { get; set; }
}
