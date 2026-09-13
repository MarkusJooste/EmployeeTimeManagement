using System;

namespace EmployeeTimeManagement.Models
{
    // A person employed at a store. One row of TBL_employees.
    public class Employee
    {
        public int EmployeeID { get; set; }
        public int StoreID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IDNumber { get; set; }

        // The one optional column on the table; null when the employee has no SARS number on file.
        public string SARSNumber { get; set; }

        public string MobileNumber { get; set; }

        // Stored in the MaritialStatus column, whose name is misspelled in the schema.
        public string MaritalStatus { get; set; }

        public int NumberOfDependents { get; set; }
        public DateTime BusinessDate { get; set; }
        public int CapturedBy { get; set; }

        // How the employee is shown in the capture grid's dropdown.
        public string FullName
        {
            get { return (Name + " " + Surname).Trim(); }
        }
    }
}
