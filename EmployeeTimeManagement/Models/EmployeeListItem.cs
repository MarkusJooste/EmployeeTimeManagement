using System;

namespace EmployeeTimeManagement.Models
{
    // One row of the Employees list: the identifying details a manager scans, plus the contract facts the list shows.
    internal class EmployeeListItem
    {
        public int EmployeeID { get; set; }
        public int StoreID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IDNumber { get; set; }
        public string MobileNumber { get; set; }
        public string JobDescription { get; set; }
        public DateTime? ContractEndDate { get; set; }

        // How the employee is shown in status messages and confirmation dialogs.
        public string FullName
        {
            get { return (Name + " " + Surname).Trim(); }
        }

        // An employee is still employed while their contract is open or ends after today;
        // someone with no contract at all counts as employed. An end date of today or
        // earlier reads as Former immediately, so Terminate's default of today takes effect
        // without a day's delay.
        public bool IsActive
        {
            get { return ContractEndDate == null || ContractEndDate.Value.Date > DateTime.Today; }
        }

        // How the employee's standing is shown in the Status column.
        public string Status
        {
            get { return IsActive ? "Active" : "Former"; }
        }
    }
}
