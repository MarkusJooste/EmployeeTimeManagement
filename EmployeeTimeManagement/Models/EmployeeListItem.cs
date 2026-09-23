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
        public string Department { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string ReasonForEnding { get; set; }

        // PTO's Opening Balance, from the latest contract. Zero and null respectively when
        // the employee has no contract row at all.
        public int OpeningPTODays { get; set; }
        public DateTime? OpeningBalanceAsAt { get; set; }

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

        // The one or two letters the list draws in place of a photograph, with a placeholder
        // for an employee whose name and surname are both missing.
        public string Initials
        {
            get
            {
                string initials = FirstLetter(Name) + FirstLetter(Surname);

                return initials.Length == 0 ? "?" : initials;
            }
        }

        // The capital a name contributes to the initials, or nothing when it is missing.
        private static string FirstLetter(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value.Trim().Substring(0, 1).ToUpperInvariant();
        }
    }
}
