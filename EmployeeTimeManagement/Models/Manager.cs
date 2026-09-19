using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeManagement.Models
{
    internal class Manager
    {
        public int ManagerID { get; set; }
        public string ManagerName { get; set; }
        public int? StoreID { get; set; }
        public bool IsAdmin { get; set; }
    }
}
