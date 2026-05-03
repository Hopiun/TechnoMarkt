using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.Common.Models;

public class KpiItem
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Color { get; set; } = "bg-indigo-500";
    public string Icon { get; set; } = "fa-chart-simple";
}



