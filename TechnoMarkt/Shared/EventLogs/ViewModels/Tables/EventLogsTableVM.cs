using Microsoft.AspNetCore.Mvc.Rendering;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;

namespace TechnoMarkt.Shared.EventLogs.ViewModels.Tables
{
    public class EventLogsTableVM : TableVM<EventLogsTableRow, EventLogsFilter>
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public SelectList RoleOptions { get; set; } = new SelectList(new[]
        {
            new SelectListItem { Text = "Administrator", Value = "Administrator" },
            new SelectListItem { Text = "Manager", Value = "Manager" },
            new SelectListItem { Text = "Operator", Value = "Operator" },
            new SelectListItem { Text = "Client", Value = "Client" }
        }, "Value", "Text");

        public SelectList ActionOptions { get; set; } = new SelectList(new[]
        {
            new SelectListItem { Text = "Create", Value = "Create" },
            new SelectListItem { Text = "Update", Value = "Update" },
            new SelectListItem { Text = "Delete", Value = "Delete" },
            new SelectListItem { Text = "Login", Value = "Login" },
            new SelectListItem { Text = "Logout", Value = "Logout" },
            new SelectListItem { Text = "Backup", Value = "Backup" },
            new SelectListItem { Text = "Restore", Value = "Restore" }
        }, "Value", "Text");

        public SelectList EntityOptions { get; set; } = new SelectList(new[]
        {
            new SelectListItem { Text = "Item", Value = "Item" },
            new SelectListItem { Text = "Order", Value = "Order" },
            new SelectListItem { Text = "OrderLine", Value = "OrderLine" },
            new SelectListItem { Text = "Payment", Value = "Payment" },
            new SelectListItem { Text = "PaymentMethod", Value = "PaymentMethod" },
            new SelectListItem { Text = "Store", Value = "Store" },
            new SelectListItem { Text = "Warehouse", Value = "Warehouse" },
            new SelectListItem { Text = "Client", Value = "Client" },
            new SelectListItem { Text = "Employee", Value = "Employee" },
            new SelectListItem { Text = "Discount", Value = "Discount" },
            new SelectListItem { Text = "Category", Value = "Category" },
            new SelectListItem { Text = "Brand", Value = "Brand" },
            new SelectListItem { Text = "Supplier", Value = "Supplier" },
            new SelectListItem { Text = "City", Value = "City" },
            new SelectListItem { Text = "Country", Value = "Country" },
            new SelectListItem { Text = "Review", Value = "Review" },
            new SelectListItem { Text = "AppSettings", Value = "AppSettings" },
            new SelectListItem { Text = "EventLog", Value = "EventLog" },
            new SelectListItem { Text = "Backup", Value = "Backup" }
        }, "Value", "Text");
    }

    public class EventLogsTableRow
    {
        public int LogId { get; set; }
        public string? UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? Role { get; set; }
        public string Action { get; set; } = null!;
        public string Entity { get; set; } = null!;
        public int? EntityId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}



