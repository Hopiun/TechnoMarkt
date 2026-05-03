using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Areas.Manager.Catalog.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechnoMarkt.Data;
using TechnoMarkt.Models;

using TechnoMarkt.Models.Utilities;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;

namespace TechnoMarkt.Areas.Manager.Catalog
{
    public class ManagerCategoriesService : IManagerCategoriesService
    {
        private readonly AppDbContext _context;

        public ManagerCategoriesService(AppDbContext context) => _context = context;

        public async Task<List<CategorySalesDto>> GetTopCategoriesAsync(int storeId, MonthYearFilter filter)
        {
            DateRange month = DateRange.FromMonthYear(filter.Month, filter.Year);

            var rawData = await _context.OrderLines.ByStore(storeId).InPeriod(month).Completed()
                .Select(orderLine => new
                {
                    CategoryId = orderLine.Item.Category.ParentCategoryId ?? orderLine.Item.CategoryId,
                    CategoryName = orderLine.Item.Category.ParentCategoryId != null
                        ? orderLine.Item.Category.ParentCategory.Name
                        : orderLine.Item.Category.Name,
                    orderLine.Quantity,
                    orderLine.PriceAtMoment,
                    orderLine.OrderId
                })
                .ToListAsync();

            return rawData
                .GroupBy(x => new { x.CategoryId, x.CategoryName })
                .Select(group => new CategorySalesDto
                (
                    group.Key.CategoryId,
                    group.Key.CategoryName,
                    group.Sum(x => x.Quantity * x.PriceAtMoment),
                    group.Select(x => x.OrderId).Distinct().Count()
                ))
                .OrderByDescending(x => x.TotalSales)
                .ToList();
        }

        public async Task<CategoryFormVM?> GetCategoryFormAsync(int? categoryId)
        {
            List<SelectListItem> topLevelCategories = await _context.Categories
                .Where(category => category.ParentCategoryId == null)
                .Select(category => new SelectListItem()
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.Name
                })
                .ToListAsync();

            if (!categoryId.HasValue)
            {
                return new CategoryFormVM()
                {
                    Action = TechnoMarkt.Shared.Common.ViewModels.Forms.FormAction.Add,
                    ParentCategories = new SelectList(topLevelCategories, "Value", "Text")
                };
            }

            Category? category = await _context.Categories.FindAsync(categoryId);
            if (category == null) return null;

            return new CategoryFormVM
            {
                Action = TechnoMarkt.Shared.Common.ViewModels.Forms.FormAction.Edit,
                Id = category.CategoryId,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategories = new SelectList(topLevelCategories, "Value", "Text", category.ParentCategoryId)
            };
        }

        public async Task<(bool Succeeded, string NotificationMessage)> AddCategoryAsync(CategoryFormVM newCategory)
        {
            if (newCategory.ParentCategoryId.HasValue)
            {
                Category? parent = await _context.Categories.FindAsync(newCategory.ParentCategoryId.Value);
                if (parent != null && parent.ParentCategoryId != null)
                {
                    return (false, "Неможливо створити категорію 3-го рівня.");
                }
            }

            bool nameExists = await _context.Categories
                .AnyAsync(category => category.Name.ToLower() == newCategory.Name.ToLower() && category.ParentCategoryId == newCategory.ParentCategoryId);

            if (nameExists)
                return (false, "Категорія з такою назвою вже існує на цьому рівні.");

            try
            {
                Category category = new Category
                {
                    Name = newCategory.Name,
                    ParentCategoryId = newCategory.ParentCategoryId
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return (true, $"Категорію '{category.Name}' успішно додано.");
            }
            catch (Exception)
            {
                return (false, "Виникла помилка під час додавання категорії.");
            }
        }

        public async Task<(bool Succeeded, string NotificationMessage)> UpdateCategoryAsync(CategoryFormVM updatedCategory)
        {
            Category? category = await _context.Categories.FindAsync(updatedCategory.Id);
            if (category == null)
                return (false, $"Категорію з ID:{updatedCategory.Id} не знайдено.");

            if (updatedCategory.ParentCategoryId.HasValue && updatedCategory.ParentCategoryId != category.ParentCategoryId)
            {
                Category? newParent = await _context.Categories.FindAsync(updatedCategory.ParentCategoryId.Value);
                if (newParent != null && newParent.ParentCategoryId != null)
                {
                    return (false, "Неможливо перемістити: утвориться 3-й рівень вкладеності.");
                }
            }

            bool nameExists = await _context.Categories
                .AnyAsync(category => category.CategoryId != updatedCategory.Id &&
                               category.Name.ToLower() == updatedCategory.Name.ToLower() &&
                               category.ParentCategoryId == updatedCategory.ParentCategoryId);

            if (nameExists)
                return (false, "Категорія з такою назвою вже існує на цьому рівні.");

            try
            {
                category.Name = updatedCategory.Name;
                category.ParentCategoryId = updatedCategory.ParentCategoryId;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                return (true, $"Категорію '{category.Name}' успішно оновлено.");
            }
            catch (Exception)
            {
                return (false, "Виникла помилка під час оновлення категорії.");
            }
        }

        public async Task<(bool Succeeded, string NotificationMessage)> DeleteCategoryAsync(int categoryId)
        {
            Category? category = await _context.Categories
                .Include(category => category.Items)
                .Include(category => category.InverseParentCategory)
                .FirstOrDefaultAsync(category => category.CategoryId == categoryId);

            if (category == null)
                return (false, $"Категорію з ID:{categoryId} не знайдено.");

            if (category.InverseParentCategory.Any())
                return (false, $"Неможливо видалити категорію '{category.Name}', оскільки вона має підкатегорії.");

            if (category.Items.Any())
                return (false, $"Неможливо видалити категорію '{category.Name}', оскільки до неї прив'язані товари.");

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return (true, $"Категорію '{category.Name}' успішно видалено.");
            }
            catch (Exception)
            {
                return (false, "Виникла помилка під час видалення категорії.");
            }
        }
    }
}




