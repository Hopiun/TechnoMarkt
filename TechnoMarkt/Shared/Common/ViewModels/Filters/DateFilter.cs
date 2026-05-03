using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.Common.ViewModels.Filters
{
    public class MonthYearFilter
    {
        public int Month { get; set; } = DateTime.Now.Month;
        public int Year { get; set; } = DateTime.Now.Year;
    }

    public class DateRangeFilter
    {
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }

        public bool DateRangeSpecified => DateTo != null;

        public bool IsDateRangeValid => DateFrom <= DateTo;
    }
}



