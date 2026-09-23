using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeManagement.Models
{
    // A User: an Owner or a Manager. One row of TBL_managers, a table named before Owners
    // existed, so its name and its ManagerID key both cover Owners too.
    public class Manager
    {
        public int ManagerID { get; set; }

        // The Employee this login belongs to; null for an Owner, who need not be an Employee.
        public int? EmployeeID { get; set; }

        public string ManagerName { get; set; }
        public int? StoreID { get; set; }

        // The four digits that are the whole credential. No two Users may share one.
        public string Pin { get; set; }

        // Whether this login still works. Cleared by Demote; the only thing login checks.
        public bool IsActiveManager { get; set; }

        public bool IsAdmin { get; set; }

        // Overwritten on every promotion or demotion, so it records the most recent one
        // rather than the first. Null for a row the migration added the column to but this
        // app has never written, since nothing was backfilled.
        public DateTime? BusinessDate { get; set; }

        public int? CapturedBy { get; set; }
    }
}
