using TechnoMarkt.Data;
using TechnoMarkt.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;

namespace TechnoMarkt.Controllers
{
    [Authorize]
    public abstract class EmployeeCabinetController : Controller
    {
        protected readonly AppDbContext _context;

        protected int StoreId => int.Parse(User.FindFirst("StoreId")?.Value);
        protected int EmployeeId => int.Parse(User.FindFirst("EmployeeId")?.Value);
        protected string CurrentRole => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        public EmployeeCabinetController(AppDbContext context) => _context = context;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Employee? employee = _context.Employees
            .FirstOrDefault(employee => employee.EmployeeId == EmployeeId);

            if (employee != null)
            {
                var store = _context.Stores.Find(employee.StoreId);
                var city = store != null ? _context.Cities.Find(store.CityId) : null;

                ViewData["FullName"] = $"{employee.LastName} {employee.FirstName}";
                ViewData["Role"] = employee.Role.ToString();
                ViewData["StoreId"] = employee.StoreId;
                ViewData["Street"] = store?.Address ?? string.Empty;
                ViewData["City"] = city?.Name ?? string.Empty;
            }

            base.OnActionExecuting(context);
        }

    }
}
