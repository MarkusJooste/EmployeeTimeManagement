using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeManagement.Models
{
    internal class EmployeeDetails
    {
        public int EmployeeID { get; set; }
        public string IDNumber { get; set; }
        public string SARSNumber { get; set; }
        public string MobileNumber { get; set; }
        public string MaritialStatus { get; set; }
        public short NumberOfDependents { get; set; }
        public DateTime BusinessDate { get; set; }
        public int CapturedBy { get; set; }
    }
}
