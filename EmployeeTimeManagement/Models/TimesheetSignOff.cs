using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EmployeeTimeManagement.Models
{
    // One line on a sign-off sheet: a captured Timesheet, or a day with nothing captured.
    // A day with more than one Timesheet gets a line for each, with the date on the first.
    public class SignOffLine
    {
        public DateTime Date { get; set; }
        public bool ShowDate { get; set; }
        public string Day { get; set; }
        public string Status { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string Breaks { get; set; }
        public decimal? Hours { get; set; }
        public string Notes { get; set; }

        // Nothing was captured for this day, so the employee should check it too.
        public bool IsMissing { get; set; }
    }

    // Everything one employee signs to confirm the hours recorded for a period.
    public class SignOffSheet
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string StoreName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<SignOffLine> Lines { get; } = new List<SignOffLine>();

        public int DaysWorked { get; set; }
        public decimal TotalHours { get; set; }
        public decimal SundayExtraHours { get; set; }
        public decimal HolidayExtraHours { get; set; }
        public int LeaveDays { get; set; }
        public int OffDays { get; set; }
        public int AwolDays { get; set; }
        public int MissingDays { get; set; }

        // Hours worked plus the Sunday and public holiday premiums, as on View Timesheets.
        public decimal PayableHours
        {
            get { return TotalHours + SundayExtraHours + HolidayExtraHours; }
        }
    }

    public static class TimesheetSignOff
    {
        // The same premiums the View Timesheets summary adds: half again for a Sunday,
        // double time for a public holiday.
        public const decimal SundayPremium = 0.5m;
        public const decimal HolidayPremium = 1.0m;

        // Builds the sheet for one employee, listing every day of the period so that a day
        // with nothing captured is in front of the employee when they sign. A day after today
        // is left blank rather than called missing, since it cannot have been captured yet.
        public static SignOffSheet Build(
            int employeeID,
            string employeeName,
            string storeName,
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<Timesheet> timesheets,
            DateTime today)
        {
            var sheet = new SignOffSheet
            {
                EmployeeID = employeeID,
                EmployeeName = employeeName,
                StoreName = storeName,
                FromDate = fromDate.Date,
                ToDate = toDate.Date
            };

            ILookup<DateTime, Timesheet> byDate = timesheets
                .Where(timesheet => timesheet.EmployeeID == employeeID)
                .ToLookup(timesheet => timesheet.WorkDate.Date);

            var workedDates = new HashSet<DateTime>();

            for (DateTime date = sheet.FromDate; date <= sheet.ToDate; date = date.AddDays(1))
            {
                List<Timesheet> onDate = byDate[date]
                    .OrderBy(timesheet => timesheet.TimeIn ?? TimeSpan.MaxValue)
                    .ThenBy(timesheet => timesheet.TimesheetID ?? 0)
                    .ToList();

                if (onDate.Count == 0 && date > today.Date)
                {
                    sheet.Lines.Add(new SignOffLine { Date = date, ShowDate = true, Status = string.Empty });
                    continue;
                }

                if (onDate.Count == 0)
                {
                    sheet.MissingDays++;
                    sheet.Lines.Add(new SignOffLine
                    {
                        Date = date,
                        ShowDate = true,
                        Status = "Not captured",
                        IsMissing = true
                    });
                    continue;
                }

                bool first = true;
                foreach (Timesheet timesheet in onDate)
                {
                    SignOffLine line = LineFor(timesheet);
                    line.ShowDate = first;
                    sheet.Lines.Add(line);
                    first = false;

                    if (line.Hours.HasValue)
                    {
                        workedDates.Add(date);
                        sheet.TotalHours += line.Hours.Value;

                        if (timesheet.DayType == DayType.Sunday)
                        {
                            sheet.SundayExtraHours += line.Hours.Value * SundayPremium;
                        }
                        else if (timesheet.DayType == DayType.PublicHoliday)
                        {
                            sheet.HolidayExtraHours += line.Hours.Value * HolidayPremium;
                        }
                    }
                }

                // A day counts once towards each status, however many rows it has.
                if (onDate.Any(timesheet => timesheet.Status == TimesheetStatus.Leave))
                {
                    sheet.LeaveDays++;
                }

                if (onDate.Any(timesheet => timesheet.Status == TimesheetStatus.Off))
                {
                    sheet.OffDays++;
                }

                if (onDate.Any(timesheet => timesheet.Status == TimesheetStatus.AWOL))
                {
                    sheet.AwolDays++;
                }
            }

            sheet.DaysWorked = workedDates.Count;
            return sheet;
        }

        // Hours between clocking in and out less both breaks, never below zero. The same sum
        // the View Timesheets summary does in SQL, so the sheet agrees with the grid.
        public static decimal? WorkedHours(Timesheet timesheet)
        {
            if (timesheet.Status != TimesheetStatus.Worked || !timesheet.TimeIn.HasValue || !timesheet.TimeOut.HasValue)
            {
                return null;
            }

            TimeSpan worked = timesheet.TimeOut.Value - timesheet.TimeIn.Value
                - BreakLength(timesheet.Break1Start, timesheet.Break1End)
                - BreakLength(timesheet.Break2Start, timesheet.Break2End);

            if (worked < TimeSpan.Zero)
            {
                return 0m;
            }

            return (decimal)worked.TotalSeconds / 3600m;
        }

        // The breaks as they were captured, for example "10:00-10:15, 13:00-13:30".
        public static string DescribeBreaks(Timesheet timesheet)
        {
            var breaks = new List<string>();

            foreach (TimeSpan?[] pair in new[]
            {
                new[] { timesheet.Break1Start, timesheet.Break1End },
                new[] { timesheet.Break2Start, timesheet.Break2End }
            })
            {
                if (pair[0].HasValue && pair[1].HasValue)
                {
                    breaks.Add(TimesheetCapture.FormatTime(pair[0]) + "-" + TimesheetCapture.FormatTime(pair[1]));
                }
            }

            return string.Join(", ", breaks);
        }

        // Hours as a clock reading, which is how employees check them: 7.75 is "7:45".
        public static string FormatHours(decimal hours)
        {
            int minutes = (int)Math.Round(hours * 60m, MidpointRounding.AwayFromZero);
            return string.Format(CultureInfo.InvariantCulture, "{0}:{1:00}", minutes / 60, minutes % 60);
        }

        private static SignOffLine LineFor(Timesheet timesheet)
        {
            var line = new SignOffLine
            {
                Date = timesheet.WorkDate.Date,
                Day = DayLabel(timesheet.DayType),
                Status = timesheet.Status.ToDatabaseValue(),
                Notes = timesheet.Notes == null ? string.Empty : timesheet.Notes.Trim(),
                TimeIn = string.Empty,
                TimeOut = string.Empty,
                Breaks = string.Empty
            };

            if (timesheet.Status == TimesheetStatus.Worked)
            {
                line.TimeIn = TimesheetCapture.FormatTime(timesheet.TimeIn);
                line.TimeOut = TimesheetCapture.FormatTime(timesheet.TimeOut);
                line.Breaks = DescribeBreaks(timesheet);
                line.Hours = WorkedHours(timesheet);
            }

            return line;
        }

        // Only the days that pay a premium are named; an ordinary day is left blank.
        private static string DayLabel(DayType dayType)
        {
            switch (dayType)
            {
                case DayType.Sunday:
                    return "Sunday";
                case DayType.PublicHoliday:
                    return "Public holiday";
                default:
                    return string.Empty;
            }
        }

        private static TimeSpan BreakLength(TimeSpan? start, TimeSpan? end)
        {
            if (!start.HasValue || !end.HasValue)
            {
                return TimeSpan.Zero;
            }

            return end.Value - start.Value;
        }
    }
}
