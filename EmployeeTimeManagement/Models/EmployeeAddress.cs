using System;

namespace EmployeeTimeManagement.Models
{
    // Where an employee lives. One row of TBL_employee_addresses.
    public class EmployeeAddress
    {
        public int? AddressID { get; set; }
        public int EmployeeID { get; set; }
        public string HouseFlatNumber { get; set; }

        // Empty for a standalone house, which has no complex or flat number.
        public string ComplexFlatNumber { get; set; }

        public string StreetName { get; set; }
        public string Town { get; set; }
        public string PostalCode { get; set; }
    }
}
