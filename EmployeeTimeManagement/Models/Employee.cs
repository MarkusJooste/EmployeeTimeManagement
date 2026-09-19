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

        // The latest contract's dates, null when the employee has no contract row at all.
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }

        // How the employee is shown in the capture grid's dropdown.
        public string FullName
        {
            get { return (Name + " " + Surname).Trim(); }
        }

        // Whether the employee was under contract on a given work date, for date-scoping the
        // capture dropdown. An employee with no contract row at all is always employed.
        public bool IsEmployedOn(DateTime workDate)
        {
            return IsEmployedOn(ContractStartDate, ContractEndDate, workDate);
        }

        // The same employment rule, usable by any caller that only has the contract dates
        // rather than a full Employee, so the rule is stated once and reused everywhere it applies.
        public static bool IsEmployedOn(DateTime? contractStartDate, DateTime? contractEndDate, DateTime workDate)
        {
            if (contractStartDate == null)
            {
                return true;
            }

            if (workDate.Date < contractStartDate.Value.Date)
            {
                return false;
            }

            return contractEndDate == null || workDate.Date <= contractEndDate.Value.Date;
        }
    }
}
