namespace TechnoMarkt.Models.Utilities
{
    public class DateRange
    {
        public DateTime From { get; }
        public DateTime To { get; }

        public DateRange(DateTime from, DateTime to)
        {
            if (from > to) throw new ArgumentException("Початкова дата (From) не може бути більше кінцевої (To)");

            From = from;
            To = to;
        }

        public static DateRange Today => new DateRange(
            DateTime.Today,
            DateTime.Today.AddDays(1).AddTicks(-1));

        public static DateRange ThisWeek
        {
            get
            {
                DateTime now = DateTime.Today;

                int daysFromMonday = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
                DateTime startOfWeek = now.AddDays(-1 * daysFromMonday);
                return new DateRange(startOfWeek, startOfWeek.AddDays(7).AddTicks(-1));
            }
        }

        public static DateRange ThisMonth
        {
            get
            {
                DateTime now = DateTime.Today;
                DateTime start = new DateTime(now.Year, now.Month, 1);
                return new DateRange(start, start.AddMonths(1).AddTicks(-1));
            }
        }

        public static DateRange ThisYear
        {
            get
            {
                DateTime start = new DateTime(DateTime.Today.Year, 1, 1);
                return new DateRange(start, start.AddYears(1).AddTicks(-1));
            }
        }

        public static DateRange FromMonthYear(int month, int year) => new DateRange(
            new DateTime(year, month, 1),
            new DateTime(year, month, DateTime.DaysInMonth(year, month)));
    }
}
