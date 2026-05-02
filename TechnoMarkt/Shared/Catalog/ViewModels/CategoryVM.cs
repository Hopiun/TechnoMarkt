using TechnoMarkt.Models;
using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Shared.Catalog.ViewModels
{
    public class CategoryCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
    public class CategoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; } = false;
        public List<CategoryItem>? SubCategories { get; set; } = [];

        public bool HasSubCategories => SubCategories.Count > 0;
        public bool IsMainCategory => !HasSubCategories;
    }

    public class SelectedCategoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public DiscountVM Discount { get; set; } = new();
    }
}


