using System;
using System.Collections.Generic;

namespace EmployeeTimeManagement.Models
{
    // An employee together with everything hanging off them, which is the unit the editor saves in one transaction.
    public class EmployeeRecord
    {
        public EmployeeRecord()
        {
            FamilyMembers = new List<EmployeeFamilyMember>();
        }

        public Employee Employee { get; set; }
        public EmployeeAddress Address { get; set; }
        public EmployeeBank Bank { get; set; }
        public EmployeeContract Contract { get; set; }

        // Null unless the employee is married, because two empty strings would look like a real spouse.
        public EmployeeSpouse Spouse { get; set; }

        public List<EmployeeFamilyMember> FamilyMembers { get; set; }
    }
}
