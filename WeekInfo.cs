using System.Globalization;

public sealed record WeekInfo (int Week, int Year, DateOnly Thursday, bool IsCurrentWeek);

static class WeekCalculator
    {
       public static WeekInfo WeekOf(DateOnly date)
        {
            var dt = date.ToDateTime(TimeOnly.MinValue);
            var week = ISOWeek.GetWeekOfYear(dt);
            var year = ISOWeek.GetYear(dt);
            var thursday = DateOnly.FromDateTime(ISOWeek.ToDateTime(year, week, DayOfWeek.Thursday));
            var today = DateOnly.FromDateTime(DateTime.Today);
            var thursdayOfToday = DateOnly.FromDateTime(ISOWeek.ToDateTime(today.Year, ISOWeek.GetWeekOfYear(today.ToDateTime(TimeOnly.MinValue)), DayOfWeek.Thursday));
            return new WeekInfo(week, year, thursday, thursday == thursdayOfToday);
        }

        public static List<WeekInfo> Range(DateOnly start, int count)
        {
            var result = new List<WeekInfo> ();
            for (var i = 0; i < count; i++)
            {
                var week = WeekOf(start.AddDays(i * 7));
                result.Add(week);
            }
            return result;
        }
    }