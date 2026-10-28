using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeTimeManagement.Models
{
    // One employee's timesheet state for a single day, as the dashboard reads it.
    public class EmployeeDayStatus
    {
        public int EmployeeID { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        // The latest contract's dates, null when the employee has no contract row at all.
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }

        // Whether any Timesheet is saved for the day, whatever its Status.
        public bool HasTimesheet { get; set; }

        // Whether booked leave (not AWOL) covers the day. Leave still needs its Timesheet
        // saved; Capture Timesheets pre-fills the row but the manager has to save it.
        public bool IsOnLeave { get; set; }

        public string FullName
        {
            get { return (Name + " " + Surname).Trim(); }
        }
    }

    // Works out whose timesheet has not been captured for a day.
    public static class TimesheetCompletion
    {
        // Everyone employed on the day who has no saved Timesheet for it, by store and then
        // the way a manager reads a register. Uses the same employment rule as Capture
        // Timesheets, so the dashboard never asks for someone the capture screen won't offer.
        public static List<EmployeeDayStatus> MissingOn(DateTime workDate, IEnumerable<EmployeeDayStatus> employees)
        {
            return employees
                .Where(employee => !employee.HasTimesheet)
                .Where(employee => Employee.IsEmployedOn(employee.ContractStartDate, employee.ContractEndDate, workDate))
                .OrderBy(employee => employee.StoreName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(employee => employee.Surname, StringComparer.OrdinalIgnoreCase)
                .ThenBy(employee => employee.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
