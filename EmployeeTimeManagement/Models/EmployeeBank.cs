using System;

namespace EmployeeTimeManagement.Models
{
    // Where an employee is paid. One row of TBL_employee_bank.
    public class EmployeeBank
    {
        public int? BankID { get; set; }
        public int EmployeeID { get; set; }
        public string BankName { get; set; }
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }
        public string BranchCode { get; set; }
    }
}
