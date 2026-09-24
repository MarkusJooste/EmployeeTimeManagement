using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeManagement.Models
{
    internal class EmployeeFamilyMember
    {
        public int FamilyMemberID { get; set; }
        public int EmployeeID { get; set; }
        public string FamilyMemberName { get; set; }
        public string MobileNumber { get; set; }
        public string Relationship { get; set; }
    }
}
