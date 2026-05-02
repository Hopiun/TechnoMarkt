using TechnoMarkt.Areas.Home.Catalog.ViewModels;
using TechnoMarkt.Areas.Manager.Catalog;
using TechnoMarkt.Shared.Common.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechnoMarkt.Data;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common;
using TechnoMarkt.Shared.Catalog.ViewModels;
using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Areas.Home.Catalog
{
    [Area("Home")]
    [AllowAnonymous]
    public class CatalogController : Controller
    {
        private readonly IItemsReadService<HomeItemsFilter> _itemsService;
        private readonly ICategoriesReadService _categoriesReadService;
        private readonly AppDbContext _context;

        public CatalogController(
            IItemsReadService<HomeItemsFilter> itemsService,
            ICategoriesReadService categoriesReadService,
            AppDbContext context)
        {
            _itemsService = itemsService;
            _categoriesReadService = categoriesReadService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId, [FromQuery] HomeItemsFilter? filter)
        {
            filter ??= new HomeItemsFilter();

            int storeId = GetStoreId();

            var (itemsResult, totalCount) = await _itemsService.GetItemsAsync(categoryId, storeId, filter);

            var cities = await _context.Cities
                .Where(c => _context.Stores.Any(s => s.CityId == c.РЎityId))
                .ToListAsync();

            int? selectedCityId = null;
            int? selectedStoreId = null;

            if (Request.Cookies["StoreId"] != null && int.TryParse(Request.Cookies["StoreId"], out int cookieStoreId))
            {
                selectedStoreId = cookieStoreId;
                selectedCityId = _context.Stores.Find(cookieStoreId)?.CityId;
            }
            selectedCityId ??= cities.FirstOrDefault()?.РЎityId;

            var stores = selectedCityId.HasValue
                ? await _context.Stores.Where(s => s.CityId == selectedCityId.Value).ToListAsync()
                : new List<Store>();

            var brands = await _context.Brands.ToListAsync();

            var model = new HomeCatalogViewModel()
            {
                Cities = new SelectList(cities.Select(c => new SelectListItem { Value = c.РЎityId.ToString(), Text = c.Name }), "Value", "Text"),
                SelectedCityId = selectedCityId,
                Stores = new SelectList(stores.Select(s => new SelectListItem { Value = s.StoreId.ToString(), Text = s.Address }), "Value", "Text"),
                SelectedStoreId = selectedStoreId,
                Brands = new SelectList(brands.OrderBy(b => b.Name).Select(b => new SelectListItem { Value = b.BrandId.ToString(), Text = b.Name }), "Value", "Text"),
                Categories = await _categoriesReadService.GetCategoriesAsync(),
                Items = new ItemsGridVM<HomeItemsFilter>()
                {
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
        public async Task<IActionResult> FilterItems(int? categoryId, [FromQuery] HomeItemsFilter? filter)
        {
            filter ??= new HomeItemsFilter();

            int storeId = GetStoreId();

            var (itemsResult, totalCount) = await _itemsService.GetItemsAsync(categoryId, storeId, filter);

            ItemsGridVM<HomeItemsFilter> model = new ItemsGridVM<HomeItemsFilter>()
            {
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

            return PartialView("_ItemsGrid", model);
        }

        [HttpGet]
        public async Task<IActionResult> ItemDetails(int itemId)
        {
            int storeId = GetStoreId();

            ItemVM? itemDetails = await _itemsService.GetItemDetailsAsync(itemId, storeId);
            if (itemDetails == null) return NotFound();

            return PartialView("~/Areas/Home/_Shared/Views/Shared/_ItemDetails.cshtml", itemDetails);
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            List<CategoryItem> categories = await _categoriesReadService.GetCategoriesAsync();
            return PartialView("_Categories", categories);
        }

        private int GetStoreId()
        {
            if (Request.Cookies["StoreId"] != null && int.TryParse(Request.Cookies["StoreId"], out int storeId))
                return storeId;

            return _context.Stores.FirstOrDefault()?.StoreId ?? 0;
        }
    }
}








