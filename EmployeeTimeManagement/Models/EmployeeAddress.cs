using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeManagement.Models
{
    internal class EmployeeAddress
    {
        public int AddressID { get; set; }
        public int EmployeeID { get; set; }
        public string HouseFlatNumber { get; set; }
        public string ComplexFlatNumber { get; set; }
        public string StreetName { get; set; }
        public string Town { get; set; }
        public string PostalCode { get; set; }
    }
}
