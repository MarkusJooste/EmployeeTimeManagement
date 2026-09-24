using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeManagement.Models
{
    internal class EmployeeContract
    {
        public int ContractID { get; set; }
        public int EmployeeID { get; set; }
        public string ContractType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Department { get; set; }
        public string JobDescription { get; set; }
        public string ReasonForEnding { get; set; }
        public decimal HourlyRate { get; set; }
    }
}
