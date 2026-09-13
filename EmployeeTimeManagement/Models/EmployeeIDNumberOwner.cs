using System;

namespace EmployeeTimeManagement.Models
{
    // Who already holds an ID number, which the database alone can say. Enough of them to
    // name the person when they work at the manager's own store, and no more: an employee
    // at another store is never identified to a manager who cannot see them.
    public class EmployeeIDNumberOwner
    {
        public int StoreID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        // How the holder is named back to the manager who collided with them.
        public string FullName
        {
            get { return (Name + " " + Surname).Trim(); }
        }
    }
}
