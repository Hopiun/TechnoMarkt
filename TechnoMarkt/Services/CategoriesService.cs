using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Extensions.Query;
using TechnoMarkt.Models;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Models.Utilities;
using TechnoMarkt.Shared.Catalog.ViewModels;
using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Services
{
    public class CategoriesService : ICategoriesReadService
    {
        private readonly AppDbContext _context;

        public CategoriesService(AppDbContext context) => _context = context;

        public async Task<List<CategoryItem>> GetCategoriesAsync()
        {
            IQueryable<Category> categories = _context.Categories
                .Include(category => category.InverseParentCategory);

            return await categories.ParentCategories().Select(category => new CategoryItem()
            {
                Id = category.CategoryId,
                Name = category.Name,
                IsSelected = false,
                SubCategories = category.InverseParentCategory.Select(sub => new CategoryItem()
                {
                    Id = sub.CategoryId,
                    Name = sub.Name,
                    IsSelected = false,
                    SubCategories = new List<CategoryItem>()
                }).ToList()
            }).ToListAsync();
        }

        public async Task<SelectedCategoryItem> GetCategoryAsync(int? categoryId)
        {
            if (!categoryId.HasValue) return null!;

            Category? category = await _context.Categories
                .Include(category => category.InverseParentCategory)
                .FirstOrDefaultAsync(category => category.CategoryId == categoryId.Value);

            if (category == null)
                return null!;

            List<int> categoryIds = new List<int> { category.CategoryId };

            bool hasSubCategories = category.InverseParentCategory != null && category.InverseParentCategory.Any();

            if (hasSubCategories)
                categoryIds.AddRange(category.InverseParentCategory.Select(category => category.CategoryId));

            int? itemCount = await _context.Items
                .Where(item => categoryIds.Contains(item.CategoryId))
                .CountAsync();

            Discount? activeDiscount = await _context.Discounts
                .InPeriod(DateRange.Today)
                .WithCategoryAsync(category.CategoryId);

            return new SelectedCategoryItem()
            {
                Id = category.CategoryId,
                Name = category.Name,
                ItemCount = itemCount ?? 0,
                Discount = activeDiscount != null ? new DiscountVM()
                {
                    Id = activeDiscount.DiscountId,
                    Percent = activeDiscount.Percent,
                    DateFrom = activeDiscount.DateFrom,
                    DateTo = activeDiscount.DateTo
                } : new DiscountVM()
            };
        }
    }
}


