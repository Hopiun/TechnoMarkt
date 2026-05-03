using TechnoMarkt.Areas.Administrator.Employees.ViewModels.Forms;
using TechnoMarkt.Areas.Administrator.Employees.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Controllers;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common.ViewModels.Forms;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;

namespace TechnoMarkt.Areas.Administrator.Employees
{
    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class EmployeesController : EmployeeCabinetController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEventLogsService _eventLogsService;

        public EmployeesController(
            AppDbContext context,
            IEmployeeService employeeService,
            IEventLogsService eventLogsService) : base(context)
        {
            _employeeService = employeeService;
            _eventLogsService = eventLogsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(EmployeeFilter? filter)
        {
            filter ??= new EmployeeFilter();

            var employees = await _employeeService.GetEmployeesAsync(StoreId, filter);
            var kpi = await _employeeService.GetKpiAsync(StoreId);
            var avgSalary = await _employeeService.GetAverageSalaryByRoleAsync(StoreId);
            var operatorStats = await _employeeService.GetOperatorOrderStatsAsync(StoreId);
            var storeForm = await _employeeService.GetStoreFormAsync(StoreId);

            var model = new EmployeesViewModel
            {
                Kpi = kpi,
                AverageSalaryByRole = avgSalary,
                OperatorOrderStats = operatorStats,
                StoreEditForm = storeForm,
                EmployeeFilter = filter,
                Employees = employees
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(EmployeeFilter filter)
        {
            var employees = await _employeeService.GetEmployeesAsync(StoreId, filter);
            return PartialView("_EmployeeTable", employees);
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeForm(int? employeeId)
        {
            if (employeeId.HasValue)
            {
                var form = await _employeeService.GetEmployeeFormAsync(employeeId.Value);
                if (form == null) return NotFound();
                return PartialView("_EmployeeForm", form);
            }

            var newForm = new EmployeeFormVM
            {
                Action = FormAction.Add,
                StoreId = StoreId,
                Role = EmployeeRole.Operator,
                Status = EmployeeStatus.Present
            };
            return PartialView("_EmployeeForm", newForm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeFormVM form)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Помилка валідації: {errors}" });
            }

            if (form.Password != form.ConfirmPassword)
            {
                return BadRequest(new { success = false, message = "Паролі не співпадають" });
            }

            var (succeeded, message) = await _employeeService.AddEmployeeAsync(StoreId, form);
            if (succeeded)
            {
                await _eventLogsService.LogAsync("Create", "Employee", null,
                    $"Додано працівника: {form.FirstName} {form.LastName}, Роль: {form.Role}, Магазин ID: {StoreId}, Зарплата: {form.Salary:N0}");
            }
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(EmployeeFormVM form)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Помилка валідації: {errors}" });
            }

            var (succeeded, message) = await _employeeService.UpdateEmployeeAsync(form.Id!.Value, form);
            if (succeeded)
            {
                await _eventLogsService.LogAsync("Update", "Employee", form.Id.Value,
                    $"Оновлено дані працівника (ID:{form.Id}): {form.FirstName} {form.LastName}, Роль: {form.Role}, Статус: {form.Status}, Зарплата: {form.Salary:N0}");
            }
            return Ok(new { success = succeeded, message });
        }

        [HttpGet]
        public async Task<IActionResult> StoreForm()
        {
            var form = await _employeeService.GetStoreFormAsync(StoreId);
            if (form == null) return NotFound();
            return PartialView("_StoreForm", form);
        }

        [HttpPost]
        public async Task<IActionResult> EditStore(StoreFormVM form)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Помилка валідації: {errors}" });
            }

            var (succeeded, message) = await _employeeService.UpdateStoreAsync(StoreId, form);
            if (succeeded)
            {
                await _eventLogsService.LogAsync("Update", "Store", StoreId,
                    $"Оновлено дані магазину (ID:{StoreId}): {form.Address}");
            }
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateEmployee([FromForm] int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            var (succeeded, message) = await _employeeService.DeactivateEmployeeAsync(employeeId, EmployeeId);
            if (succeeded)
            {
                await _eventLogsService.LogAsync("Delete", "Employee", employeeId,
                    $"Деактивовано працівника: {employee?.FirstName} {employee?.LastName} (ID:{employeeId})");
            }
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        public async Task<IActionResult> ReactivateEmployee([FromForm] int employeeId)
        {
            var (succeeded, message) = await _employeeService.ReactivateEmployeeAsync(employeeId);
            if (succeeded)
            {
                await _eventLogsService.LogAsync("Update", "Employee", employeeId,
                    $"Реактивовано працівника (ID:{employeeId})");
            }
            return Ok(new { success = succeeded, message });
        }
    }
}







