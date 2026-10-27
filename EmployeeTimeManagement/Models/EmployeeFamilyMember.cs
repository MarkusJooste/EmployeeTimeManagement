using System;

namespace EmployeeTimeManagement.Models
{
    // Somebody to phone about an employee. One row of TBL_employee_family.
    public class EmployeeFamilyMember
    {
        public int? FamilyMemberID { get; set; }
        public int EmployeeID { get; set; }
        public string FamilyMemberName { get; set; }

        // The one nullable column among the three; a contact reachable only in person has none.
        public string MobileNumber { get; set; }

        public string Relationship { get; set; }
    }
}
