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
                    return (false, "РќРµРјРѕР¶Р»РёРІРѕ СЃС‚РІРѕСЂРёС‚Рё РєР°С‚РµРіРѕСЂС–СЋ 3-РіРѕ СЂС–РІРЅСЏ.");
                }
            }

            bool nameExists = await _context.Categories
                .AnyAsync(category => category.Name.ToLower() == newCategory.Name.ToLower() && category.ParentCategoryId == newCategory.ParentCategoryId);

            if (nameExists)
                return (false, "РљР°С‚РµРіРѕСЂС–СЏ Р· С‚Р°РєРѕСЋ РЅР°Р·РІРѕСЋ РІР¶Рµ С–СЃРЅСѓС” РЅР° С†СЊРѕРјСѓ СЂС–РІРЅС–.");

            try
            {
                Category category = new Category
                {
                    Name = newCategory.Name,
                    ParentCategoryId = newCategory.ParentCategoryId
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return (true, $"РљР°С‚РµРіРѕСЂС–СЋ '{category.Name}' СѓСЃРїС–С€РЅРѕ РґРѕРґР°РЅРѕ.");
            }
            catch (Exception)
            {
                return (false, "Р’РёРЅРёРєР»Р° РїРѕРјРёР»РєР° РїС–Рґ С‡Р°СЃ РґРѕРґР°РІР°РЅРЅСЏ РєР°С‚РµРіРѕСЂС–С—.");
            }
        }

        public async Task<(bool Succeeded, string NotificationMessage)> UpdateCategoryAsync(CategoryFormVM updatedCategory)
        {
            Category? category = await _context.Categories.FindAsync(updatedCategory.Id);
            if (category == null)
                return (false, $"РљР°С‚РµРіРѕСЂС–СЋ Р· ID:{updatedCategory.Id} РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

            if (updatedCategory.ParentCategoryId.HasValue && updatedCategory.ParentCategoryId != category.ParentCategoryId)
            {
                Category? newParent = await _context.Categories.FindAsync(updatedCategory.ParentCategoryId.Value);
                if (newParent != null && newParent.ParentCategoryId != null)
                {
                    return (false, "РќРµРјРѕР¶Р»РёРІРѕ РїРµСЂРµРјС–СЃС‚РёС‚Рё: СѓС‚РІРѕСЂРёС‚СЊСЃСЏ 3-Р№ СЂС–РІРµРЅСЊ РІРєР»Р°РґРµРЅРѕСЃС‚С–.");
                }
            }

            bool nameExists = await _context.Categories
                .AnyAsync(category => category.CategoryId != updatedCategory.Id &&
                               category.Name.ToLower() == updatedCategory.Name.ToLower() &&
                               category.ParentCategoryId == updatedCategory.ParentCategoryId);

            if (nameExists)
                return (false, "РљР°С‚РµРіРѕСЂС–СЏ Р· С‚Р°РєРѕСЋ РЅР°Р·РІРѕСЋ РІР¶Рµ С–СЃРЅСѓС” РЅР° С†СЊРѕРјСѓ СЂС–РІРЅС–.");

            try
            {
                category.Name = updatedCategory.Name;
                category.ParentCategoryId = updatedCategory.ParentCategoryId;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                return (true, $"РљР°С‚РµРіРѕСЂС–СЋ '{category.Name}' СѓСЃРїС–С€РЅРѕ РѕРЅРѕРІР»РµРЅРѕ.");
            }
            catch (Exception)
            {
                return (false, "Р’РёРЅРёРєР»Р° РїРѕРјРёР»РєР° РїС–Рґ С‡Р°СЃ РѕРЅРѕРІР»РµРЅРЅСЏ РєР°С‚РµРіРѕСЂС–С—.");
            }
        }

        public async Task<(bool Succeeded, string NotificationMessage)> DeleteCategoryAsync(int categoryId)
        {
            Category? category = await _context.Categories
                .Include(category => category.Items)
                .Include(category => category.InverseParentCategory)
                .FirstOrDefaultAsync(category => category.CategoryId == categoryId);

            if (category == null)
                return (false, $"РљР°С‚РµРіРѕСЂС–СЋ Р· ID:{categoryId} РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

            if (category.InverseParentCategory.Any())
                return (false, $"РќРµРјРѕР¶Р»РёРІРѕ РІРёРґР°Р»РёС‚Рё РєР°С‚РµРіРѕСЂС–СЋ '{category.Name}', РѕСЃРєС–Р»СЊРєРё РІРѕРЅР° РјР°С” РїС–РґРєР°С‚РµРіРѕСЂС–С—.");

            if (category.Items.Any())
                return (false, $"РќРµРјРѕР¶Р»РёРІРѕ РІРёРґР°Р»РёС‚Рё РєР°С‚РµРіРѕСЂС–СЋ '{category.Name}', РѕСЃРєС–Р»СЊРєРё РґРѕ РЅРµС— РїСЂРёРІ'СЏР·Р°РЅС– С‚РѕРІР°СЂРё.");

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return (true, $"РљР°С‚РµРіРѕСЂС–СЋ '{category.Name}' СѓСЃРїС–С€РЅРѕ РІРёРґР°Р»РµРЅРѕ.");
            }
            catch (Exception)
            {
                return (false, "Р’РёРЅРёРєР»Р° РїРѕРјРёР»РєР° РїС–Рґ С‡Р°СЃ РІРёРґР°Р»РµРЅРЅСЏ РєР°С‚РµРіРѕСЂС–С—.");
            }
        }
    }
}






