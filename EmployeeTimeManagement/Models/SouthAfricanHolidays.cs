using System;
using System.Collections.Generic;

namespace EmployeeTimeManagement.Models
{
    // Works out whether a date is a South African public holiday, and what DayType a
    // WorkDate therefore carries. Computed rather than stored, so no schema or
    // maintenance screen is needed; once-off declared holidays are handled by the
    // manager overriding DayType on the row instead.
    public static class SouthAfricanHolidays
    {
        private static readonly Dictionary<int, HashSet<DateTime>> HolidaysByYear = new Dictionary<int, HashSet<DateTime>>();
        private static readonly object CacheLock = new object();

        // Reports whether a date is a South African public holiday, including the
        // Monday observed when a holiday falls on a Sunday.
        public static bool IsPublicHoliday(DateTime date)
        {
            DateTime day = date.Date;
            return GetHolidays(day.Year).Contains(day);
        }

        // Reports the DayType of a WorkDate. A day that is both a Sunday and a public
        // holiday is a Public Holiday, because the holiday premium is the higher of the two.
        public static DayType GetDayType(DateTime date)
        {
            if (IsPublicHoliday(date))
            {
                return DayType.PublicHoliday;
            }

            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                return DayType.Sunday;
            }

            return DayType.Normal;
        }

        // Returns every public holiday in a year, computing and caching it on first use.
        private static HashSet<DateTime> GetHolidays(int year)
        {
            lock (CacheLock)
            {
                HashSet<DateTime> holidays;
                if (!HolidaysByYear.TryGetValue(year, out holidays))
                {
                    holidays = BuildHolidays(year);
                    HolidaysByYear.Add(year, holidays);
                }

                return holidays;
            }
        }

        // Builds a year's holidays: the twelve statutory days, then the Mondays observed
        // for any of them that fall on a Sunday.
        private static HashSet<DateTime> BuildHolidays(int year)
        {
            DateTime easter = GetEasterSunday(year);

            var statutory = new List<DateTime>
            {
                new DateTime(year, 1, 1),    // New Year's Day
                new DateTime(year, 3, 21),   // Human Rights Day
                easter.AddDays(-2),          // Good Friday
                easter.AddDays(1),           // Family Day
                new DateTime(year, 4, 27),   // Freedom Day
                new DateTime(year, 5, 1),    // Workers' Day
                new DateTime(year, 6, 16),   // Youth Day
                new DateTime(year, 8, 9),    // National Women's Day
                new DateTime(year, 9, 24),   // Heritage Day
                new DateTime(year, 12, 16),  // Day of Reconciliation
                new DateTime(year, 12, 25),  // Christmas Day
                new DateTime(year, 12, 26)   // Day of Goodwill
            };

            var holidays = new HashSet<DateTime>(statutory);

            // The Public Holidays Act makes the day after a Sunday holiday a holiday too.
            // Where that day is already one, the observed day moves to the next free day.
            foreach (DateTime holiday in statutory)
            {
                if (holiday.DayOfWeek != DayOfWeek.Sunday)
                {
                    continue;
                }

                DateTime observed = holiday.AddDays(1);
                while (holidays.Contains(observed))
                {
                    observed = observed.AddDays(1);
                }

                holidays.Add(observed);
            }

            return holidays;
        }

        // Calculates Easter Sunday using the anonymous Gregorian computus.
        private static DateTime GetEasterSunday(int year)
        {
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = ((19 * a) + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + (2 * e) + (2 * i) - h - k) % 7;
            int m = (a + (11 * h) + (22 * l)) / 451;
            int month = (h + l - (7 * m) + 114) / 31;
            int day = ((h + l - (7 * m) + 114) % 31) + 1;

            return new DateTime(year, month, day);
        }
    }
}
