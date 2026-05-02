using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.Common.Models;

public class NavigationItem
{
    public string Title { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public string CurrentController { get; set; } = string.Empty;

    public bool IsActive => string.Equals(Controller, CurrentController, StringComparison.OrdinalIgnoreCase);
}



