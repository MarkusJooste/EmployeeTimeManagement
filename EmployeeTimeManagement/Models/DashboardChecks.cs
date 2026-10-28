using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EmployeeTimeManagement.Models
{
    // What the dashboard knows about one employee: who they are, their latest contract,
    // and whether the records a contract needs have been captured.
    public class EmployeeOverview
    {
        public int EmployeeID { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IDNumber { get; set; }

        // The latest contract, all null when the employee has no contract row at all.
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public decimal? HourlyRate { get; set; }

        public bool HasAddress { get; set; }

        // A named family member or spouse, either of whom can be put down as next of kin.
        public bool HasNextOfKin { get; set; }

        public string FullName
        {
            get { return (Name + " " + Surname).Trim(); }
        }

        public bool IsEmployedOn(DateTime date)
        {
            return Employee.IsEmployedOn(ContractStartDate, ContractEndDate, date);
        }
    }

    // A saved Timesheet's employee and day, used to find days nobody captured.
    public class CapturedDay
    {
        public CapturedDay(int employeeID, DateTime workDate)
        {
            EmployeeID = employeeID;
            WorkDate = workDate.Date;
        }

        public int EmployeeID { get; private set; }
        public DateTime WorkDate { get; private set; }
    }

    // One line in a dashboard list: who, at which store, and what needs attention.
    public class DashboardAlert
    {
        public DashboardAlert(EmployeeOverview employee, string note, DateTime sortDate)
        {
            EmployeeID = employee.EmployeeID;
            EmployeeName = employee.FullName;
            StoreName = employee.StoreName;
            Surname = employee.Surname;
            Note = note;
            SortDate = sortDate;
        }

        // A line for a list that does not come from an EmployeeOverview.
        public DashboardAlert(string employeeName, string storeName, string note)
        {
            EmployeeName = employeeName;
            StoreName = storeName;
            Surname = string.Empty;
            Note = note;
        }

        public int EmployeeID { get; private set; }
        public string EmployeeName { get; private set; }
        public string StoreName { get; private set; }
        public string Note { get; private set; }

        // Used to put the most urgent first.
        public DateTime SortDate { get; private set; }

        internal string Surname { get; private set; }
    }

    // The dashboard's checks. Each takes what was read from the database and returns the
    // employees that need the manager's attention, most urgent first.
    public static class DashboardChecks
    {
        // How many days before today the missed-timesheet list looks back.
        public const int MissedDaysLookBack = 7;

        // The contract's probation period (clause 2.3), and how early to warn it is ending.
        public const int ProbationMonths = 4;
        public const int ProbationWarningDays = 28;

        public const int ContractEndingWarningDays = 30;

        // The contract allows 45 normal hours a week (clause 5.1.1). The list starts a little
        // below that, so there is time to adjust the roster before it becomes overtime.
        public const decimal NormalWeeklyHours = 45m;
        public const decimal LongHoursWarning = 40m;

        // The pay week runs Sunday to Saturday (clause 6.1.1 of the general contract).
        public static DateTime WeekStart(DateTime date)
        {
            return date.Date.AddDays(-(int)date.DayOfWeek);
        }

        // How many employees are under contract on a day, for the Employees tile.
        public static int EmployedCount(DateTime date, IEnumerable<EmployeeOverview> employees)
        {
            return employees.Count(employee => employee.IsEmployedOn(date));
        }

        // Employees with no saved Timesheet on a day they were employed, from MissedDaysLookBack
        // days ago up to yesterday. Today is left out; it has its own list and is usually
        // still being worked.
        public static List<DashboardAlert> MissedTimesheets(
            DateTime today, IEnumerable<EmployeeOverview> employees, IEnumerable<CapturedDay> captured)
        {
            var capturedKeys = new HashSet<string>(captured.Select(day => Key(day.EmployeeID, day.WorkDate)));
            var alerts = new List<DashboardAlert>();

            foreach (EmployeeOverview employee in employees)
            {
                var missed = new List<DateTime>();

                for (int daysAgo = MissedDaysLookBack; daysAgo >= 1; daysAgo--)
                {
                    DateTime day = today.Date.AddDays(-daysAgo);
                    if (employee.IsEmployedOn(day) && !capturedKeys.Contains(Key(employee.EmployeeID, day)))
                    {
                        missed.Add(day);
                    }
                }

                if (missed.Count == 0)
                {
                    continue;
                }

                string days = string.Join(", ", missed.Select(ShortDate));
                string note = missed.Count == 1 ? days : missed.Count + " days: " + days;
                alerts.Add(new DashboardAlert(employee, note, missed[0]));
            }

            return Sorted(alerts);
        }

        // Employees whose probation ends between today and ProbationWarningDays from now.
        public static List<DashboardAlert> ProbationEnding(DateTime today, IEnumerable<EmployeeOverview> employees)
        {
            var alerts = new List<DashboardAlert>();

            foreach (EmployeeOverview employee in employees)
            {
                if (!employee.ContractStartDate.HasValue || !employee.IsEmployedOn(today))
                {
                    continue;
                }

                DateTime ends = ProbationEnd(employee.ContractStartDate.Value);
                if (ends >= today.Date && ends <= today.Date.AddDays(ProbationWarningDays))
                {
                    alerts.Add(new DashboardAlert(employee, "Ends " + ShortDate(ends) + DaysAway(today, ends), ends));
                }
            }

            return Sorted(alerts);
        }

        public static DateTime ProbationEnd(DateTime contractStart)
        {
            return contractStart.Date.AddMonths(ProbationMonths);
        }

        // Employees whose contract ends between today and ContractEndingWarningDays from now.
        public static List<DashboardAlert> ContractsEnding(DateTime today, IEnumerable<EmployeeOverview> employees)
        {
            var alerts = new List<DashboardAlert>();

            foreach (EmployeeOverview employee in employees)
            {
                if (!employee.ContractEndDate.HasValue)
                {
                    continue;
                }

                DateTime ends = employee.ContractEndDate.Value.Date;
                if (ends >= today.Date && ends <= today.Date.AddDays(ContractEndingWarningDays))
                {
                    alerts.Add(new DashboardAlert(employee, "Ends " + ShortDate(ends) + DaysAway(today, ends), ends));
                }
            }

            return Sorted(alerts);
        }

        // Employees who have worked LongHoursWarning hours or more this pay week, most
        // hours first. hoursThisWeek maps EmployeeID to hours worked net of breaks.
        public static List<DashboardAlert> LongHours(
            IEnumerable<EmployeeOverview> employees, IDictionary<int, decimal> hoursThisWeek)
        {
            var alerts = new List<KeyValuePair<decimal, DashboardAlert>>();

            foreach (EmployeeOverview employee in employees)
            {
                decimal hours;
                if (!hoursThisWeek.TryGetValue(employee.EmployeeID, out hours) || hours < LongHoursWarning)
                {
                    continue;
                }

                string note = hours.ToString("0.0", CultureInfo.InvariantCulture) + " h";
                if (hours > NormalWeeklyHours)
                {
                    note += " (over " + NormalWeeklyHours.ToString("0", CultureInfo.InvariantCulture) + ")";
                }

                alerts.Add(new KeyValuePair<decimal, DashboardAlert>(hours, new DashboardAlert(employee, note, DateTime.MinValue)));
            }

            return alerts
                .OrderByDescending(pair => pair.Key)
                .ThenBy(pair => pair.Value.Surname, StringComparer.OrdinalIgnoreCase)
                .Select(pair => pair.Value)
                .ToList();
        }

        // Employees employed today whose record is missing something their contract needs.
        public static List<DashboardAlert> IncompleteRecords(DateTime today, IEnumerable<EmployeeOverview> employees)
        {
            var alerts = new List<DashboardAlert>();

            foreach (EmployeeOverview employee in employees)
            {
                if (!employee.IsEmployedOn(today))
                {
                    continue;
                }

                var missing = new List<string>();

                if (string.IsNullOrWhiteSpace(employee.IDNumber))
                {
                    missing.Add("ID number");
                }

                if (!employee.HasAddress)
                {
                    missing.Add("address");
                }

                if (!employee.HasNextOfKin)
                {
                    missing.Add("next of kin");
                }

                if (!employee.HourlyRate.HasValue || employee.HourlyRate.Value <= 0)
                {
                    missing.Add("hourly rate");
                }

                if (missing.Count > 0)
                {
                    alerts.Add(new DashboardAlert(employee, "No " + string.Join(", ", missing), DateTime.MinValue));
                }
            }

            return Sorted(alerts);
        }

        // "Tue 27 Oct".
        public static string ShortDate(DateTime date)
        {
            return date.ToString("ddd d MMM", CultureInfo.InvariantCulture);
        }

        private static string DaysAway(DateTime today, DateTime date)
        {
            int days = (date.Date - today.Date).Days;
            if (days == 0)
            {
                return " (today)";
            }

            return days == 1 ? " (tomorrow)" : " (" + days + " days)";
        }

        // Earliest first, then by store and name, so the owner's list keeps each store together.
        private static List<DashboardAlert> Sorted(List<DashboardAlert> alerts)
        {
            return alerts
                .OrderBy(alert => alert.SortDate)
                .ThenBy(alert => alert.StoreName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(alert => alert.Surname, StringComparer.OrdinalIgnoreCase)
                .ThenBy(alert => alert.EmployeeName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string Key(int employeeID, DateTime day)
        {
            return employeeID + "|" + day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
