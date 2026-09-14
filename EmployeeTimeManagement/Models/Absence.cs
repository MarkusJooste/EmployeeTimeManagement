using System;

namespace EmployeeTimeManagement.Models
{
    // One row of TBL_leave: an employee's Leave Type, date range, reason and Override reason.
    public class Absence
    {
        public int LeaveID { get; set; }
        public int EmployeeID { get; set; }
        public LeaveType LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string OverrideReason { get; set; }

        // Every calendar day in the range, inclusive of both ends, Sundays and public holidays included.
        public int DayCount
        {
            get { return (EndDate.Date - StartDate.Date).Days + 1; }
        }

        // How the Leave Type reads in the history grid, as the exact value stored in the
        // database, so it matches what the Leave Type filter's dropdown shows.
        public string LeaveTypeDisplay
        {
            get { return LeaveType.ToDatabaseValue(); }
        }
    }
}
