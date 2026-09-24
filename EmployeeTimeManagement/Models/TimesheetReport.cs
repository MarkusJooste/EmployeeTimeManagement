using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeManagement.Models
{
    internal class TimesheetReport
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<TimesheetSummary> Rows { get; set; } =
            new List<TimesheetSummary>();

        public decimal TotalHours
        {
            get { return Rows.Sum(row => row.TotalHours); }
        }

        public decimal TotalSundayExtraHours
        {
            get { return Rows.Sum(row => row.SundayExtraHours); }
        }

        public decimal TotalHolidayExtraHours
        {
            get { return Rows.Sum(row => row.HolidayExtraHours); }
        }

        public decimal TotalPayableHours
        {
            get { return Rows.Sum(row => row.PayableHours); }
        }

        public int TotalDaysWorked
        {
            get { return Rows.Sum(row => row.DaysWorked); }
        }

        public decimal TotalPay
        {
            get { return TotalPayableHours; }
        }
    }
}
