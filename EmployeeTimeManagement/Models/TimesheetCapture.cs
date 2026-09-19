using System;
using System.Collections.Generic;
using System.Globalization;

namespace EmployeeTimeManagement.Models
{
    // One row of raw input from the capture grid, before anything has been parsed.
    public class CaptureRow
    {
        public int RowIndex { get; set; }
        public int? EmployeeID { get; set; }
        public string Status { get; set; }
        public string DayType { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string Break1Start { get; set; }
        public string Break1End { get; set; }
        public string Break2Start { get; set; }
        public string Break2End { get; set; }
        public string Notes { get; set; }

        // Set when the manager chose to update a Timesheet that already exists.
        public int? ExistingTimesheetID { get; set; }
    }

    // A single blocking problem, tied to the grid row that caused it.
    public class RowError
    {
        // Ties a blocking problem to the grid row that caused it.
        public RowError(int rowIndex, string message)
        {
            RowIndex = rowIndex;
            Message = message;
        }

        public int RowIndex { get; private set; }
        public string Message { get; private set; }
    }

    // The outcome of validating a whole day: what can be written, and what cannot.
    public class CaptureResult
    {
        // Holds the Timesheets that passed and the problems that blocked the rest.
        public CaptureResult(List<Timesheet> valid, List<RowError> errors)
        {
            Valid = valid;
            Errors = errors;
        }

        public List<Timesheet> Valid { get; private set; }
        public List<RowError> Errors { get; private set; }

        // True when every row passed, which is the only time anything may be saved.
        public bool IsValid
        {
            get { return Errors.Count == 0; }
        }
    }

    // Turns the capture grid's raw input into persistable Timesheets, or into per-row
    // errors describing why it cannot. Holds every parsing and validation rule so the
    // form itself stays a dumb collector.
    public static class TimesheetCapture
    {
        // Validates and assembles a whole day of capture rows. Produces no Timesheets at
        // all unless every row is valid, so a save is all-or-nothing.
        public static CaptureResult Build(DateTime workDate, int capturedBy, IEnumerable<CaptureRow> rows)
        {
            var timesheets = new List<Timesheet>();
            var errors = new List<RowError>();
            var seenEmployees = new HashSet<int>();

            DayType defaultDayType = SouthAfricanHolidays.GetDayType(workDate);

            foreach (CaptureRow row in rows)
            {
                string error;
                Timesheet timesheet = BuildRow(row, workDate, capturedBy, defaultDayType, seenEmployees, out error);

                // The employee counts as seen even when the row failed for some other
                // reason, so a later duplicate is still caught.
                if (row.EmployeeID.HasValue)
                {
                    seenEmployees.Add(row.EmployeeID.Value);
                }

                if (error != null)
                {
                    errors.Add(new RowError(row.RowIndex, error));
                    continue;
                }

                timesheets.Add(timesheet);
            }

            if (errors.Count > 0)
            {
                timesheets.Clear();
            }

            return new CaptureResult(timesheets, errors);
        }

        // Validates one row, returning the Timesheet it describes or the reason it cannot.
        private static Timesheet BuildRow(
            CaptureRow row,
            DateTime workDate,
            int capturedBy,
            DayType defaultDayType,
            HashSet<int> seenEmployees,
            out string error)
        {
            error = null;

            if (!row.EmployeeID.HasValue)
            {
                error = "Choose an employee for this row.";
                return null;
            }

            if (seenEmployees.Contains(row.EmployeeID.Value))
            {
                error = "This employee is already on the grid; nobody can be added twice.";
                return null;
            }

            TimesheetStatus status;
            if (!TimesheetStatuses.TryParse(row.Status, out status))
            {
                error = "Choose a status for this row.";
                return null;
            }

            DayType dayType = defaultDayType;
            if (!string.IsNullOrWhiteSpace(row.DayType))
            {
                try
                {
                    dayType = DayTypes.FromDatabaseValue(row.DayType);
                }
                catch (ArgumentException)
                {
                    error = "Choose a day type for this row.";
                    return null;
                }
            }

            string notes = null;
            if (!string.IsNullOrWhiteSpace(row.Notes))
            {
                notes = row.Notes.Trim();
            }

            var timesheet = new Timesheet
            {
                TimesheetID = row.ExistingTimesheetID,
                EmployeeID = row.EmployeeID.Value,
                WorkDate = workDate.Date,
                DayType = dayType,
                Status = status,
                CapturedBy = capturedBy,
                BusinessDate = DateTime.Today,
                Notes = notes
            };

            // An employee who was not there has no times, so anything typed is discarded
            // rather than validated.
            if (status != TimesheetStatus.Worked)
            {
                return timesheet;
            }

            TimeSpan? timeIn, timeOut, break1Start, break1End, break2Start, break2End;

            if (!ParseField(row.TimeIn, "TimeIn", out timeIn, ref error)
                || !ParseField(row.TimeOut, "TimeOut", out timeOut, ref error)
                || !ParseField(row.Break1Start, "Break 1 start", out break1Start, ref error)
                || !ParseField(row.Break1End, "Break 1 end", out break1End, ref error)
                || !ParseField(row.Break2Start, "Break 2 start", out break2Start, ref error)
                || !ParseField(row.Break2End, "Break 2 end", out break2End, ref error))
            {
                return null;
            }

            error = ValidateWorkedShift(timeIn, timeOut, break1Start, break1End, break2Start, break2End);
            if (error != null)
            {
                return null;
            }

            timesheet.TimeIn = timeIn;
            timesheet.TimeOut = timeOut;
            timesheet.Break1Start = break1Start;
            timesheet.Break1End = break1End;
            timesheet.Break2Start = break2Start;
            timesheet.Break2End = break2End;

            return timesheet;
        }

        // Parses one time cell, naming the field in the error so the manager knows which.
        private static bool ParseField(string text, string fieldName, out TimeSpan? value, ref string error)
        {
            if (TryParseTime(text, out value))
            {
                return true;
            }

            error = fieldName + " is not a time we understand. Try 08:00, 8:00, 0800 or 8.";
            return false;
        }

        // Applies every rule that governs a shift somebody actually worked.
        private static string ValidateWorkedShift(
            TimeSpan? timeIn,
            TimeSpan? timeOut,
            TimeSpan? break1Start,
            TimeSpan? break1End,
            TimeSpan? break2Start,
            TimeSpan? break2End)
        {
            if (!timeIn.HasValue)
            {
                return "TimeIn is required when the status is Worked.";
            }

            if (!timeOut.HasValue)
            {
                return "TimeOut is required when the status is Worked.";
            }

            // No shift at this store crosses midnight, so this holds unconditionally.
            if (timeOut.Value <= timeIn.Value)
            {
                return "TimeOut must be after TimeIn.";
            }

            string breakError = ValidateBreak("Break 1", break1Start, break1End, timeIn.Value, timeOut.Value);
            if (breakError != null)
            {
                return breakError;
            }

            breakError = ValidateBreak("Break 2", break2Start, break2End, timeIn.Value, timeOut.Value);
            if (breakError != null)
            {
                return breakError;
            }

            bool hasBreak1 = break1Start.HasValue;
            bool hasBreak2 = break2Start.HasValue;

            if (hasBreak2 && !hasBreak1)
            {
                return "Break 1 must be filled in before Break 2.";
            }

            if (hasBreak1 && hasBreak2 && break2Start.Value <= break1End.Value)
            {
                return "Break 2 must start after Break 1 ends.";
            }

            return null;
        }

        // Checks one break: both ends or neither, ordered, and inside the shift.
        private static string ValidateBreak(string name, TimeSpan? start, TimeSpan? end, TimeSpan timeIn, TimeSpan timeOut)
        {
            if (!start.HasValue && !end.HasValue)
            {
                return null;
            }

            if (!start.HasValue || !end.HasValue)
            {
                return name + " needs both a start and an end, or neither.";
            }

            if (end.Value <= start.Value)
            {
                return name + " must end after it starts.";
            }

            if (start.Value < timeIn || end.Value > timeOut)
            {
                return name + " must fall inside the shift.";
            }

            return null;
        }

        // Parses a time typed loosely: 8, 08, 800, 0800, 8:00 and 08:00 all mean 08:00.
        // Blank input is a valid absence rather than a failure.
        public static bool TryParseTime(string text, out TimeSpan? value)
        {
            value = null;

            if (string.IsNullOrWhiteSpace(text))
            {
                return true;
            }

            string trimmed = text.Trim();
            int hours;
            int minutes;

            int separator = trimmed.IndexOf(':');
            if (separator >= 0)
            {
                if (!TryParseNumber(trimmed.Substring(0, separator), out hours)
                    || !TryParseNumber(trimmed.Substring(separator + 1), out minutes))
                {
                    return false;
                }
            }
            else
            {
                int digits;
                if (!TryParseNumber(trimmed, out digits))
                {
                    return false;
                }

                switch (trimmed.Length)
                {
                    case 1:
                    case 2:
                        hours = digits;
                        minutes = 0;
                        break;
                    case 3:
                    case 4:
                        hours = digits / 100;
                        minutes = digits % 100;
                        break;
                    default:
                        return false;
                }
            }

            if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
            {
                return false;
            }

            value = new TimeSpan(hours, minutes, 0);
            return true;
        }

        // Parses a run of digits, rejecting signs, spaces and anything else.
        private static bool TryParseNumber(string text, out int number)
        {
            number = 0;

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            foreach (char character in text)
            {
                if (!char.IsDigit(character))
                {
                    return false;
                }
            }

            return int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out number);
        }

        // Renders a parsed time back as the canonical HH:mm the grid displays.
        public static string FormatTime(TimeSpan? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}", value.Value.Hours, value.Value.Minutes);
        }
    }
}
