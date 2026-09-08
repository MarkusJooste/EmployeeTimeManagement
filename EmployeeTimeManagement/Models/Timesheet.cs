using System;

namespace EmployeeTimeManagement.Models
{
    // Mirrors the Status enum on TBL_timesheets. Only 'Worked' rows carry times.
    public enum TimesheetStatus
    {
        Worked,
        Off,
        Leave,
        AWOL
    }

    // One employee's record for one calendar day: their attendance status, and if they
    // worked, their in/out times and breaks. One row of TBL_timesheets.
    public class Timesheet
    {
        public int? TimesheetID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime WorkDate { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public TimeSpan? Break1Start { get; set; }
        public TimeSpan? Break1End { get; set; }
        public TimeSpan? Break2Start { get; set; }
        public TimeSpan? Break2End { get; set; }
        public DayType DayType { get; set; }
        public TimesheetStatus Status { get; set; }
        public int CapturedBy { get; set; }
        public DateTime BusinessDate { get; set; }
        public string Notes { get; set; }

        // True when this replaces a stored Timesheet rather than creating a new one.
        public bool IsUpdate
        {
            get { return TimesheetID.HasValue; }
        }

        // Describes when the employee was at work, or why they were not, for the dialogs
        // that ask the manager to tell one Timesheet from another.
        public string DescribeWhen()
        {
            if (Status != TimesheetStatus.Worked)
            {
                return Status.ToDatabaseValue();
            }

            return TimesheetCapture.FormatTime(TimeIn) + " - " + TimesheetCapture.FormatTime(TimeOut);
        }
    }

    public static class TimesheetStatuses
    {
        // Converts a status to the exact string stored in the TBL_timesheets enum column.
        // The enum names were chosen to match those values exactly.
        public static string ToDatabaseValue(this TimesheetStatus status)
        {
            return status.ToString();
        }

        // Lists every status as stored, for binding a dropdown without restating them.
        public static string[] AllDatabaseValues()
        {
            var values = (TimesheetStatus[])Enum.GetValues(typeof(TimesheetStatus));
            var names = new string[values.Length];

            for (int index = 0; index < values.Length; index++)
            {
                names[index] = values[index].ToDatabaseValue();
            }

            return names;
        }

        // Converts a string read from the TBL_timesheets enum column back to a status.
        public static TimesheetStatus FromDatabaseValue(string value)
        {
            TimesheetStatus status;
            if (!TryParse(value, out status))
            {
                throw new ArgumentException("Unknown status value: " + value, "value");
            }

            return status;
        }

        // Parses a status name, accepting any casing, without throwing on bad input.
        public static bool TryParse(string value, out TimesheetStatus status)
        {
            status = TimesheetStatus.Worked;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            foreach (TimesheetStatus candidate in (TimesheetStatus[])Enum.GetValues(typeof(TimesheetStatus)))
            {
                if (string.Equals(candidate.ToString(), value.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    status = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}
