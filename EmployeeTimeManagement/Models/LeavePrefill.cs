using System;
using System.Collections.Generic;

namespace EmployeeTimeManagement.Models
{
    // Decides which employees Capture Timesheets should pre-fill Status 'Leave' for when a
    // day is opened, per issue 09. Takes the day's Absences and the EmployeeIDs already
    // holding a saved Timesheet rather than looking either up itself, so the decision needs
    // no database and no form.
    public static class LeavePrefill
    {
        public static List<int> EmployeeIDsToPrefill(
            DateTime workDate, IEnumerable<Absence> absences, IEnumerable<int> employeeIDsWithSavedTimesheet)
        {
            var saved = new HashSet<int>(employeeIDsWithSavedTimesheet);
            var seen = new HashSet<int>();
            var result = new List<int>();
            DateTime day = workDate.Date;

            foreach (Absence absence in absences)
            {
                // AWOL is derived from Timesheets, so letting it flow back would close a
                // loop where the app feeds itself its own output (ADR-0002).
                if (absence.LeaveType == LeaveType.AWOL)
                {
                    continue;
                }

                if (day < absence.StartDate.Date || day > absence.EndDate.Date)
                {
                    continue;
                }

                if (saved.Contains(absence.EmployeeID) || !seen.Add(absence.EmployeeID))
                {
                    continue;
                }

                result.Add(absence.EmployeeID);
            }

            return result;
        }
    }
}
