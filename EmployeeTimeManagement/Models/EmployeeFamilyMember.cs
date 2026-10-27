using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeManagement.Models
{
    internal class EmployeeFamilyMember
    {
        public int EmployeeID { get; set; }
        public string FamilyMemberName { get; set; }

        // The one nullable column among the three; a contact reachable only in person has none.
        public string MobileNumber { get; set; }

        public string Relationship { get; set; }
    }
}
