using System;

namespace EmployeeTimeManagement.Models
{
    // A person employed at a store, as far as timesheet capture is concerned.
    internal class Employee
    {
        public int EmployeeID { get; set; }
        public int StoreID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        // How the employee is shown in the capture grid's dropdown.
        public string FullName
        {
            get { return (Name + " " + Surname).Trim(); }
        }
    }
}
