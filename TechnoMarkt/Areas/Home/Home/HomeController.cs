using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Areas.Home.Home.ViewModels;
using TechnoMarkt.Data;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Models;

namespace TechnoMarkt.Areas.Home.Home
{
    [Area("Home")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly AppDbContext _context;
        private readonly IEventLogsService _eventLogsService;

        public HomeController(IHomeService homeService, AppDbContext context, IEventLogsService eventLogsService)
        {
            _homeService = homeService;
            _context = context;
            _eventLogsService = eventLogsService;
        }

        public async Task<IActionResult> Index()
        {
            var cities = await _context.Cities
                .Where(c => _context.Stores.Any(s => s.CityId == c.CityId))
                .ToListAsync();

            int? selectedCityId = null;
            if (Request.Cookies["StoreId"] != null && int.TryParse(Request.Cookies["StoreId"], out int storeId))
            {
                selectedCityId = _context.Stores.Find(storeId)?.CityId;
            }

            var stores = selectedCityId.HasValue
                ? await _context.Stores.Where(s => s.CityId == selectedCityId.Value).ToListAsync()
                : new List<Store>();

            var model = new HomeViewModel
            {
                Cities = new SelectList(
                    cities.Select(c => new SelectListItem { Value = c.CityId.ToString(), Text = c.Name }),
                    "Value", "Text"
                ),
                SelectedCityId = selectedCityId,
                Stores = new SelectList(
                    stores.Select(s => new SelectListItem { Value = s.StoreId.ToString(), Text = s.Address }),
                    "Value", "Text"
                ),
                SelectedStoreId = int.TryParse(Request.Cookies["StoreId"], out int selectedStore) ? selectedStore : (int?)null,
                Categories = await _homeService.GetMainCategoriesAsync(),
                TopItems   = await _homeService.GetTopItemsAsync(),
            };

            if (User.Identity is { IsAuthenticated: true })
            {
                model.ClientName   = $"{User.FindFirst("LastName")?.Value} {User.FindFirst("FirstName")?.Value}";
                model.ClientEmail  = User.Identity.Name;
                model.ClientWallet = User.FindFirst("WalletBalance")?.Value ?? "0.00";
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetStoresByCity(int cityId)
        {
            var stores = await _context.Stores
                .Where(s => s.CityId == cityId)
                .Select(s => new { id = s.StoreId, address = s.Address })
                .ToListAsync();

            return Json(stores);
        }

        [HttpPost]
        public IActionResult SetCity(int cityId, string returnUrl = "/Home/Home/Index")
        {
            var store = _context.Stores.FirstOrDefault(s => s.CityId == cityId);
            if (store != null)
            {
                Response.Cookies.Append("StoreId", store.StoreId.ToString(),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30), IsEssential = true });

                var city = _context.Cities.Find(cityId);
                var user = User.Identity?.Name ?? "Відвідувач";
                _eventLogsService.LogAsync("Update", "Store", store.StoreId,
                    $"{user} обрав магазин: {store.Address}, місто {city?.Name ?? "—" + cityId}");
            }
            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        public IActionResult SetStore([FromBody] int storeId)
        {
            Response.Cookies.Append("StoreId", storeId.ToString(),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30), IsEssential = true });

            var store = _context.Stores.Find(storeId);
            var user = User.Identity?.Name ?? "Відвідувач";
            _eventLogsService.LogAsync("Update", "Store", storeId,
                $"{user} змінив магазин (ID:{storeId})");

            return Ok();
        }
    }
}




