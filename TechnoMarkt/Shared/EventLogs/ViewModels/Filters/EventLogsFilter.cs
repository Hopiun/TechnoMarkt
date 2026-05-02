using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.EventLogs.ViewModels.Filters
{
    public class EventLogsFilter
    {
        public string? Role { get; set; }
        public string? Action { get; set; }
        public string? Entity { get; set; }
        public string? SearchUser { get; set; }
    }
}



