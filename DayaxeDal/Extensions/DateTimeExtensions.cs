using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TimeZoneConverter;

namespace DayaxeDal.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToLosAngerlesTime(this DateTime currentTime)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(currentTime, TimeZoneInfo.Utc.Id, "Pacific Standard Time");
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        public static List<DateTime> GetDates(this DateTime current)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(current.Year, current.Month))  // Days: 1, 2 ... 31 etc.
                             .Select(day => new DateTime(current.Year, current.Month, day)) // Map each day to a date
                             .ToList(); // Load dates into a list
        }

        public static List<DateTime> GetDates(this DateTime current, int toDate)
        {
            var to = current.Date.AddDays(toDate);
            return Enumerable.Range(1, DateTime.DaysInMonth(current.Year, current.Month))  // Days: 1, 2 ... 31 etc.
                             .Select(day => new DateTime(current.Year, current.Month, day)) // Map each day to a date
                             .Where(d => current.Date <= d.Date && d.Date <= to.Date)
                             .ToList(); // Load dates into a list
        }

        public static int GetIsoWeek(this DateTime currentTime)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(currentTime,
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }

        public static IEnumerable<DateTime> DateRange(this DateTime fromDate, DateTime toDate)
        {
            return Enumerable.Range(0, toDate.Subtract(fromDate).Days + 1)
                .Select(d => fromDate.AddDays(d));
        }

        public static long GetTimestamp(this DateTime currentDate)
        {
            return (Int32)(currentDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static DateTime ToLosAngerlesTimeWithTimeZone(this DateTime currentDate, string destinationTimeZoneId)
        {
            if (string.IsNullOrEmpty(destinationTimeZoneId))
            {
                return currentDate.ToLosAngerlesTime();
            }

            string tz = TZConvert.IanaToWindows(destinationTimeZoneId);
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(currentDate, TimeZoneInfo.Utc.Id, tz);
        }
    }
}
