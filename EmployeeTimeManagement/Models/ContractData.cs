using System;
using System.Collections.Generic;

namespace EmployeeTimeManagement.Models
{
    // The two contracts the client uses. They share most clauses; the Manager one has
    // its own duties, salary, tips and commencement wording.
    internal enum ContractTemplate
    {
        General,
        Manager
    }

    internal static class ContractTemplates
    {
        // The name shown in the template dropdown and used in the PDF file name.
        public static string DisplayName(this ContractTemplate template)
        {
            switch (template)
            {
                case ContractTemplate.General:
                    return "General staff";
                case ContractTemplate.Manager:
                    return "Manager";
                default:
                    throw new ArgumentOutOfRangeException("template", template, "Unknown contract template.");
            }
        }
    }

    // A person who can be named as next of kin on the contract.
    internal class NextOfKinOption
    {
        public NextOfKinOption(string name, string relationship, string contactNumber)
        {
            Name = name;
            Relationship = relationship;
            ContactNumber = contactNumber;
        }

        // Leaves the three next-of-kin blanks empty for handwriting.
        public static readonly NextOfKinOption None = new NextOfKinOption(null, null, null);

        public string Name { get; private set; }
        public string Relationship { get; private set; }
        public string ContactNumber { get; private set; }

        // How the option appears in the dropdown.
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return "(Leave blank)";
            }

            if (string.IsNullOrWhiteSpace(Relationship))
            {
                return Name;
            }

            return Name + " (" + Relationship + ")";
        }
    }

    // Every value that goes into a contract's blanks. A null value is printed as a
    // dotted line so it can be completed by hand.
    internal class ContractData
    {
        public ContractTemplate Template { get; set; }

        public string FullName { get; set; }
        public string IDNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string TelephoneNumber { get; set; }

        public string NextOfKinName { get; set; }
        public string NextOfKinRelationship { get; set; }
        public string NextOfKinNumber { get; set; }

        public DateTime? CommencementDate { get; set; }
        public string Position { get; set; }
        public decimal? HourlyRate { get; set; }

        // The store whose copy of the contract is filled in.
        public int StoreID { get; set; }
        public string StoreName { get; set; }

        // The date printed in the "SIGNED IN ... ON THIS ... DAY OF" lines.
        public DateTime SigningDate { get; set; }
    }

    // Everything stored about one employee that a contract can use, as read from the
    // database by ContractController.
    internal class ContractSource
    {
        public ContractSource()
        {
            FamilyMembers = new List<EmployeeFamilyMember>();
        }

        public Employee Employee { get; set; }
        public string StoreName { get; set; }

        // Null when nothing was captured.
        public EmployeeAddress Address { get; set; }
        public EmployeeSpouse Spouse { get; set; }
        public EmployeeContract Contract { get; set; }

        public List<EmployeeFamilyMember> FamilyMembers { get; private set; }

        // True when an active row in TBL_managers points at this employee.
        public bool IsLinkedManager { get; set; }

        // The template to offer first: Manager for anyone who is a manager in the system
        // or whose position says so, General for everyone else.
        public ContractTemplate SuggestedTemplate()
        {
            if (IsLinkedManager)
            {
                return ContractTemplate.Manager;
            }

            if (Contract != null
                && Contract.JobDescription != null
                && Contract.JobDescription.IndexOf("manager", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ContractTemplate.Manager;
            }

            return ContractTemplate.General;
        }

        // The people who can be named as next of kin: captured family members first, in
        // the order captured, then the spouse, then the option to leave it blank.
        public List<NextOfKinOption> NextOfKinOptions()
        {
            var options = new List<NextOfKinOption>();

            foreach (EmployeeFamilyMember member in FamilyMembers)
            {
                if (string.IsNullOrWhiteSpace(member.FamilyMemberName))
                {
                    continue;
                }

                options.Add(new NextOfKinOption(member.FamilyMemberName, member.Relationship, member.MobileNumber));
            }

            if (Spouse != null && !string.IsNullOrWhiteSpace(Spouse.SpouseName))
            {
                options.Add(new NextOfKinOption(Spouse.SpouseName, "Spouse", Spouse.SpouseMobileNumber));
            }

            options.Add(NextOfKinOption.None);
            return options;
        }

        // Assembles the values for the blanks from the stored data and the choices made
        // on the Contracts screen.
        public ContractData ToContractData(
            NextOfKinOption nextOfKin,
            ContractTemplate template,
            DateTime signingDate)
        {
            if (nextOfKin == null)
            {
                nextOfKin = NextOfKinOption.None;
            }

            var data = new ContractData
            {
                Template = template,
                FullName = Employee == null ? null : Employee.FullName,
                IDNumber = Employee == null ? null : Employee.IDNumber,
                TelephoneNumber = Employee == null ? null : Employee.MobileNumber,
                NextOfKinName = nextOfKin.Name,
                NextOfKinRelationship = nextOfKin.Relationship,
                NextOfKinNumber = nextOfKin.ContactNumber,
                StoreID = Employee == null ? 0 : Employee.StoreID,
                StoreName = StoreName,
                SigningDate = signingDate.Date
            };

            string line1;
            string line2;
            FormatAddress(Address, out line1, out line2);
            data.AddressLine1 = line1;
            data.AddressLine2 = line2;

            if (Contract != null)
            {
                data.CommencementDate = Contract.StartDate;
                data.Position = Contract.JobDescription;

                // A zero rate is treated as not captured, so the blank is left for the
                // hourly rate to be written in rather than printing R0.00.
                if (Contract.HourlyRate > 0)
                {
                    data.HourlyRate = Contract.HourlyRate;
                }
            }

            return data;
        }

        // Splits an address into the two lines the contract has room for: the unit and
        // street, then the town and postal code. Empty parts are left out.
        public static void FormatAddress(EmployeeAddress address, out string line1, out string line2)
        {
            line1 = null;
            line2 = null;

            if (address == null)
            {
                return;
            }

            string street = JoinNonEmpty(" ", address.HouseFlatNumber, address.StreetName);
            line1 = JoinNonEmpty(", ", address.ComplexFlatNumber, street);
            line2 = JoinNonEmpty(" ", address.Town, address.PostalCode);
        }

        // Joins the non-blank parts, or returns null when every part is blank.
        private static string JoinNonEmpty(string separator, params string[] parts)
        {
            var kept = new List<string>();

            foreach (string part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part))
                {
                    kept.Add(part.Trim());
                }
            }

            if (kept.Count == 0)
            {
                return null;
            }

            return string.Join(separator, kept);
        }
    }
}
