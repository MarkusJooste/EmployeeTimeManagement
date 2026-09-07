using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeManagement.Models
{
    internal class TimesheetSummary
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int DaysWorked { get; set; }
        public decimal TotalHours { get; set; }
        public decimal SundayExtraHours { get; set; }
        public decimal HolidayExtraHours { get; set; }

        // Hours actually worked plus the Sunday and public holiday premium surplus
        public decimal PayableHours
        {
            get { return TotalHours + SundayExtraHours + HolidayExtraHours; }
        }
    }
}
