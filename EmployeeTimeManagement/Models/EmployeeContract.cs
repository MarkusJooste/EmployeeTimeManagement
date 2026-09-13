using System;

namespace EmployeeTimeManagement.Models
{
    // The terms an employee works under, and the rate payroll pays them. One row of TBL_employee_contracts.
    public class EmployeeContract
    {
        public int? ContractID { get; set; }
        public int EmployeeID { get; set; }
        public string ContractType { get; set; }
        public DateTime StartDate { get; set; }

        // Null while the employee is still employed; set by Terminate, cleared by Reactivate.
        public DateTime? EndDate { get; set; }

        public string Department { get; set; }
        public string JobDescription { get; set; }

        // The empty string while the contract is live, because the column cannot hold null.
        public string ReasonForEnding { get; set; }

        public decimal HourlyRate { get; set; }
    }
}
