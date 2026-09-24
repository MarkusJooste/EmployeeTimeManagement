using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeManagement.Models
{
    internal class EmployeeBank
    {
        public int BankID { get; set; }
        public int EmployeeID { get; set; }
        public string BankName { get; set; }
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }
        public string BranchCode { get; set; }
    }
}
