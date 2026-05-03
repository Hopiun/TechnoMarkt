using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;

namespace TechnoMarkt.Areas.Administrator.Settings.ViewModels
{
    public class SettingsViewModel
    {
        public EventLogsTableVM EventLogsTable { get; set; } = new();
        public Dictionary<string, string> AppSettings { get; set; } = new();
    }
}



