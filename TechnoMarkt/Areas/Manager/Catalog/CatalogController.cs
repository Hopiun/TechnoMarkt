using TechnoMarkt.Areas.Manager.Catalog.ViewModels;
using TechnoMarkt.Shared.Common.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechnoMarkt.Data;
using TechnoMarkt.Controllers;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Shared.Common;
using TechnoMarkt.Shared.Catalog.ViewModels;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;
using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Areas.Manager.Catalog
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class CatalogController : EmployeeCabinetController
    {
        private readonly IManagerItemsService _itemsService;
        private readonly IManagerCategoriesService _categoriesService;
        private readonly IDiscountsService _discountsService;
        private readonly ICategoriesReadService _categoriesReadService;
        private readonly IEventLogsService _eventLogsService;

        public CatalogController(
            AppDbContext context,
            IManagerItemsService itemsService,
            IManagerCategoriesService categoriesService,
            IDiscountsService discountsService,
            ICategoriesReadService categoriesReadService,
            IEventLogsService eventLogsService) : base(context)
        {
            _itemsService = itemsService;
            _categoriesService = categoriesService;
            _discountsService = discountsService;
            _categoriesReadService = categoriesReadService;
            _eventLogsService = eventLogsService;
        }

        #region Pages & Partial Pages

        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId = null, [FromQuery] ManagerItemsFilter? filter = null, [FromQuery] MonthYearFilter? topFilter = null)
        {
            filter ??= new ManagerItemsFilter();
            topFilter ??= new MonthYearFilter();

            filter.Suppliers = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "Name");

            var (itemsResult, totalCount) = await _itemsService.GetItemsAsync(categoryId, StoreId, filter);

            CatalogViewModel model = new CatalogViewModel()
            {
                TopFilter = topFilter,
                TopCategories = await _categoriesService.GetTopCategoriesAsync(StoreId, topFilter),
                TopItems = await _itemsService.GetTopItemsAsync(StoreId, topFilter),
                Categories = await _categoriesReadService.GetCategoriesAsync(),
                Items = new ItemsGridVM<ManagerItemsFilter>()
                {
                    Role = CurrentRole,
                    SelectedCategory = await _categoriesReadService.GetCategoryAsync(categoryId),
                    Filter = filter,
                    Items = itemsResult,
                    Pagination = new PaginationVM()
                    {
                        CurrentPage = filter.CurrentPage,
                        PageSize = filter.PageSize,
                        TotalCount = totalCount
                    }
                }
            };

            ViewBag.CategoryId = categoryId;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> FilterItems(int? categoryId, ManagerItemsFilter filter)
        {
            filter.Suppliers = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "Name");

            var (itemsResult, totalCount) = await _itemsService.GetItemsAsync(categoryId, StoreId, filter);

            ItemsGridVM<ManagerItemsFilter> model = new ItemsGridVM<ManagerItemsFilter>()
            {
                Role = CurrentRole,
                SelectedCategory = await _categoriesReadService.GetCategoryAsync(categoryId),
                Filter = filter,
                Items = itemsResult,
                Pagination = new PaginationVM()
                {
                    CurrentPage = filter.CurrentPage,
                    PageSize = filter.PageSize,
                    TotalCount = totalCount
                }
            };

            return PartialView("~/Areas/Manager/_Shared/Views/Shared/Catalog/_ItemsGrid.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> ItemDetails(int itemId)
        {
            ItemVM? itemDetails = await _itemsService.GetItemDetailsAsync(itemId, StoreId);
            if (itemDetails == null) return NotFound();

            return PartialView("~/Areas/Home/_Shared/Views/Shared/_ItemDetails.cshtml", itemDetails);
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            List<CategoryItem> categories = await _categoriesReadService.GetCategoriesAsync();
            return PartialView("~/Areas/Manager/_Shared/Views/Shared/Catalog/_Categories.cshtml", categories);
        }

        #endregion

        #region Items Management

        [HttpGet]
        public async Task<IActionResult> ItemForm(int? itemId)
        {
            ItemFormVM? form = await _itemsService.GetItemFormAsync(itemId);
            if (itemId.HasValue && form == null) return NotFound();

            return PartialView("~/Areas/Manager/_Shared/Views/Shared/Forms/_ItemForm.cshtml", form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(ItemFormVM form)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Помилка валідації: {errors}" });
            }

            var (succeeded, message) = await _itemsService.AddItemAsync(form, StoreId);
            if (succeeded)
            {
                var category = await _context.Categories.FindAsync(form.CategoryId);
                var brand = await _context.Brands.FindAsync(form.BrandId);
                var desc = $"Додано товар: '{form.Name}', Категорія: {category?.Name ?? "—"}, Бренд: {brand?.Name ?? "—"}, Ціна: {form.Price:N0}";
                await _eventLogsService.LogAsync("Create", "Item", null, desc);
            }
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(ItemFormVM form)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Помилка валідації: {errors}" });
            }

            var (succeeded, message) = await _itemsService.UpdateItemAsync(form);
            if (succeeded)
            {
                var category = await _context.Categories.FindAsync(form.CategoryId);
                var brand = await _context.Brands.FindAsync(form.BrandId);
                var desc = $"Оновлено товар (ID:{form.Id}): '{form.Name}', Категорія: {category?.Name ?? "—"}, Бренд: {brand?.Name ?? "—"}, Ціна: {form.Price:N0}";
                await _eventLogsService.LogAsync("Update", "Item", form.Id, desc);
            }
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem([FromForm] int itemId)
        {
            var item = await _context.Items.FindAsync(itemId);
            var (succeeded, message) = await _itemsService.DeleteItemAsync(itemId);
            if (succeeded)
                await _eventLogsService.LogAsync("Delete", "Item", itemId, $"Видалено товар: '{item?.Name ?? "ID:" + itemId}'");
            return Ok(new { success = succeeded, message });
        }

        #endregion

        #region Categories Management

        [HttpGet]
        public async Task<IActionResult> CategoryForm(int? categoryId)
        {
            CategoryFormVM? form = await _categoriesService.GetCategoryFormAsync(categoryId);
            if (categoryId.HasValue && form == null) return NotFound();

            return PartialView("~/Areas/Manager/_Shared/Views/Shared/Forms/_CategoryForm.cshtml", form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(CategoryFormVM form)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Помилка валідації: {errors}" });
            }

            var (succeeded, message) = await _categoriesService.AddCategoryAsync(form);
            if (succeeded)
            {
                var parent = form.ParentCategoryId.HasValue ? await _context.Categories.FindAsync(form.ParentCategoryId) : null;
                var desc = $"Додано категорію: '{form.Name}'" + (parent != null ? $" (батьківська: {parent.Name})" : "");
                await _eventLogsService.LogAsync("Create", "Category", null, desc);
            }
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(CategoryFormVM form)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Помилка валідації: {errors}" });
            }

            var (succeeded, message) = await _categoriesService.UpdateCategoryAsync(form);
            if (succeeded)
            {
                var parent = form.ParentCategoryId.HasValue ? await _context.Categories.FindAsync(form.ParentCategoryId) : null;
                var desc = $"Оновлено категорію (ID:{form.Id}): '{form.Name}'" + (parent != null ? $", батьківська: {parent.Name}" : "");
                await _eventLogsService.LogAsync("Update", "Category", form.Id, desc);
            }
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory([FromForm] int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            var (succeeded, message) = await _categoriesService.DeleteCategoryAsync(categoryId);
            if (succeeded)
                await _eventLogsService.LogAsync("Delete", "Category", categoryId, $"Видалено категорію: '{category?.Name ?? "ID:" + categoryId}'");
            return Ok(new { success = succeeded, message });
        }

        #endregion

        #region Discounts Management

        [HttpGet]
        public async Task<IActionResult> DiscountForm(int? itemId, int? categoryId)
        {
            DiscountFormVM? form = await _discountsService.GetDiscountFormAsync(itemId, categoryId);
            if (form == null) return NotFound();

            return PartialView("~/Areas/Manager/_Shared/Views/Shared/Forms/_DiscountForm.cshtml", form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDiscount(DiscountFormVM form)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Помилка валідації: {errors}" });
            }

            var (succeeded, message) = await _discountsService.AssignDiscountAsync(form);
            if (succeeded)
            {
                string target = form.ItemId.HasValue
                    ? await _context.Items.Where(i => i.ItemId == form.ItemId).Select(i => i.Name).FirstOrDefaultAsync() ?? $"товар ID:{form.ItemId}"
                    : await _context.Categories.Where(c => c.CategoryId == form.CategoryId).Select(c => c.Name).FirstOrDefaultAsync() ?? $"категорія ID:{form.CategoryId}";
                var desc = $"Призначено знижку {form.Percent}% на '{target}' з {form.DateFrom:dd.MM.yyyy} по {form.DateTo:dd.MM.yyyy}";
                await _eventLogsService.LogAsync("Create", "Discount", null, desc);
            }
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDiscount(DiscountFormVM form)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Помилка валідації: {errors}" });
            }

            var (succeeded, message) = await _discountsService.UpdateDiscountAsync(form);
            if (succeeded)
            {
                string target = form.ItemId.HasValue
                    ? await _context.Items.Where(i => i.ItemId == form.ItemId).Select(i => i.Name).FirstOrDefaultAsync() ?? $"товар ID:{form.ItemId}"
                    : await _context.Categories.Where(c => c.CategoryId == form.CategoryId).Select(c => c.Name).FirstOrDefaultAsync() ?? $"категорія ID:{form.CategoryId}";
                var desc = $"Оновлено знижку (ID:{form.Id}): {form.Percent}% на '{target}', з {form.DateFrom:dd.MM.yyyy} по {form.DateTo:dd.MM.yyyy}";
                await _eventLogsService.LogAsync("Update", "Discount", form.Id, desc);
            }
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        public async Task<IActionResult> RevokeDiscount([FromForm] int? itemId, [FromForm] int? categoryId)
        {
            string target = itemId.HasValue
                ? await _context.Items.Where(i => i.ItemId == itemId).Select(i => i.Name).FirstOrDefaultAsync() ?? $"товар ID:{itemId}"
                : categoryId.HasValue
                    ? await _context.Categories.Where(c => c.CategoryId == categoryId).Select(c => c.Name).FirstOrDefaultAsync() ?? $"категорія ID:{categoryId}"
                    : "—";
            var (succeeded, message) = await _discountsService.RevokeDiscountAsync(itemId, categoryId);
            if (succeeded)
                await _eventLogsService.LogAsync("Delete", "Discount", null, $"Скасовано знижку для '{target}'");
            return Ok(new { success = succeeded, message });
        }

        #endregion
    }
}









