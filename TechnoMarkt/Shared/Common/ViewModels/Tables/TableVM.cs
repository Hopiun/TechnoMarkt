using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.Common.ViewModels.Tables
{
    public class TableVM<TRow, TFilter> where TFilter : new()
    {
        public string Role { get; set; } = string.Empty;
        public TFilter Filter { get; set; } = new TFilter();
        public List<TRow> Rows { get; set; } = [];
    }
}



