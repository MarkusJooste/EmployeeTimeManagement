using System;

namespace EmployeeTimeManagement.Models
{
    // An employee's spouse. One row of TBL_employee_spouses, written only for a married employee.
    public class EmployeeSpouse
    {
        public int? SpouseID { get; set; }
        public int EmployeeID { get; set; }
        public string SpouseName { get; set; }
        public string SpouseMobileNumber { get; set; }
    }
}
