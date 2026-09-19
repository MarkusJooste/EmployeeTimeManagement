using System;

namespace EmployeeTimeManagement.Models
{
    // Mirrors the DayType enum on TBL_timesheets. Determines the premium hours an
    // employee earns on top of the hours they actually worked.
    public enum DayType
    {
        Normal,
        Sunday,
        PublicHoliday
    }

    public static class DayTypes
    {
        private const string NormalValue = "Normal";
        private const string SundayValue = "Sunday";
        private const string PublicHolidayValue = "Public Holiday";

        // Converts a DayType to the exact string stored in the TBL_timesheets enum column.
        public static string ToDatabaseValue(this DayType dayType)
        {
            switch (dayType)
            {
                case DayType.Normal:
                    return NormalValue;
                case DayType.Sunday:
                    return SundayValue;
                case DayType.PublicHoliday:
                    return PublicHolidayValue;
                default:
                    throw new ArgumentOutOfRangeException("dayType", dayType, "Unknown day type.");
            }
        }

        // Lists every day type as stored, for binding a dropdown without restating them.
        public static string[] AllDatabaseValues()
        {
            return new[] { NormalValue, SundayValue, PublicHolidayValue };
        }

        // Converts a string read from the TBL_timesheets enum column back to a DayType.
        public static DayType FromDatabaseValue(string value)
        {
            if (string.Equals(value, NormalValue, StringComparison.OrdinalIgnoreCase))
            {
                return DayType.Normal;
            }

            if (string.Equals(value, SundayValue, StringComparison.OrdinalIgnoreCase))
            {
                return DayType.Sunday;
            }

            if (string.Equals(value, PublicHolidayValue, StringComparison.OrdinalIgnoreCase))
            {
                return DayType.PublicHoliday;
            }

            throw new ArgumentException("Unknown day type value: " + value, "value");
        }
    }
}
