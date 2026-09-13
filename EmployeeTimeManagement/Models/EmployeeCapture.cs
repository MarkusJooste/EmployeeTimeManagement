using System;
using System.Collections.Generic;
using System.Globalization;

namespace EmployeeTimeManagement.Models
{
    // One family row as the editor's grid holds it, before anything has been checked.
    public class FamilyMemberInput
    {
        // Identifies the stored row when the employee is being updated; null for a row just added.
        public int? FamilyMemberID { get; set; }

        public int RowIndex { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Relationship { get; set; }
    }

    // Everything the editor holds for one employee, as typed, before anything has been parsed.
    public class EmployeeCaptureInput
    {
        public EmployeeCaptureInput()
        {
            FamilyMembers = new List<FamilyMemberInput>();
        }

        // The identifiers of the stored rows, all null while adding a new employee.
        public int? EmployeeID { get; set; }
        public int? AddressID { get; set; }
        public int? BankID { get; set; }
        public int? ContractID { get; set; }
        public int? SpouseID { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string IDNumber { get; set; }
        public string SARSNumber { get; set; }
        public string MobileNumber { get; set; }
        public string MaritalStatus { get; set; }
        public string NumberOfDependents { get; set; }

        public string HouseFlatNumber { get; set; }
        public string ComplexFlatNumber { get; set; }
        public string StreetName { get; set; }
        public string Town { get; set; }
        public string PostalCode { get; set; }

        public string BankName { get; set; }
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }
        public string BranchCode { get; set; }

        public string ContractType { get; set; }
        public DateTime? StartDate { get; set; }
        public string Department { get; set; }
        public string JobDescription { get; set; }
        public string HourlyRate { get; set; }

        public string SpouseName { get; set; }
        public string SpouseMobileNumber { get; set; }

        public List<FamilyMemberInput> FamilyMembers { get; set; }
    }

    // A single blocking problem, tied to the field that caused it.
    public class EmployeeFieldError
    {
        // Ties a blocking problem to a field of the editor.
        public EmployeeFieldError(string field, string message)
            : this(field, message, null)
        {
        }

        // Ties a blocking problem to a field of one family grid row.
        public EmployeeFieldError(string field, string message, int? familyRowIndex)
        {
            Field = field;
            Message = message;
            FamilyRowIndex = familyRowIndex;
        }

        // Names the model property at fault, which is how the editor finds the control to highlight.
        public string Field { get; private set; }

        public string Message { get; private set; }

        // The family grid row the problem is on, or null when the field is not on that grid.
        public int? FamilyRowIndex { get; private set; }
    }

    // The outcome of validating a whole employee: what can be written, and what cannot.
    public class EmployeeCaptureResult
    {
        // Holds the record if it passed, and the problems that blocked it if it did not.
        public EmployeeCaptureResult(EmployeeRecord record, List<EmployeeFieldError> errors)
        {
            Record = record;
            Errors = errors;
        }

        // Null unless every rule passed, so a half-captured employee never reaches the database.
        public EmployeeRecord Record { get; private set; }

        public List<EmployeeFieldError> Errors { get; private set; }

        // True when every field passed, which is the only time anything may be saved.
        public bool IsValid
        {
            get { return Errors.Count == 0; }
        }
    }

    // Turns the employee editor's raw input into a persistable EmployeeRecord, or into
    // per-field errors describing why it cannot. Holds every parsing and validation rule
    // so the form itself stays a dumb collector.
    //
    // Whether an ID number is already taken is deliberately not decided here: that needs
    // the database, and belongs to the controller that saves.
    public static class EmployeeCapture
    {
        // Validates the whole employee at once, so every failing field is reported rather
        // than only the first. Produces no record at all unless everything passed.
        public static EmployeeCaptureResult Build(int storeID, int capturedBy, EmployeeCaptureInput input)
        {
            var errors = new List<EmployeeFieldError>();

            Employee employee = BuildEmployee(storeID, capturedBy, input, errors);
            EmployeeAddress address = BuildAddress(input, errors);
            EmployeeBank bank = BuildBank(input, errors);
            EmployeeContract contract = BuildContract(input, errors);
            EmployeeSpouse spouse = BuildSpouse(input, employee.MaritalStatus, errors);
            List<EmployeeFamilyMember> family = BuildFamily(input, errors);

            if (errors.Count > 0)
            {
                return new EmployeeCaptureResult(null, errors);
            }

            var record = new EmployeeRecord
            {
                Employee = employee,
                Address = address,
                Bank = bank,
                Contract = contract,
                Spouse = spouse,
                FamilyMembers = family
            };

            return new EmployeeCaptureResult(record, errors);
        }

        // Assembles the TBL_employees row: the personal details and the two audit fields.
        private static Employee BuildEmployee(int storeID, int capturedBy, EmployeeCaptureInput input, List<EmployeeFieldError> errors)
        {
            var employee = new Employee
            {
                EmployeeID = EmployeeIDOrNew(input),
                StoreID = storeID,
                CapturedBy = capturedBy,
                BusinessDate = DateTime.Today,
                Name = Required(input.Name, "Name", "A name is required.", errors),
                Surname = Required(input.Surname, "Surname", "A surname is required.", errors),
                MobileNumber = Required(input.MobileNumber, "MobileNumber", "A mobile number is required.", errors),
                SARSNumber = Optional(input.SARSNumber)
            };

            employee.IDNumber = ValidateIDNumber(input.IDNumber, errors);
            employee.MaritalStatus = ValidateMaritalStatus(input.MaritalStatus, errors);
            employee.NumberOfDependents = ValidateDependents(input.NumberOfDependents, errors);

            return employee;
        }

        // Assembles the TBL_employee_addresses row. The complex or flat number is the one
        // field left blank without complaint, because a standalone house has none.
        private static EmployeeAddress BuildAddress(EmployeeCaptureInput input, List<EmployeeFieldError> errors)
        {
            return new EmployeeAddress
            {
                AddressID = input.AddressID,
                EmployeeID = EmployeeIDOrNew(input),
                HouseFlatNumber = Required(input.HouseFlatNumber, "HouseFlatNumber", "A house or flat number is required.", errors),
                ComplexFlatNumber = Trim(input.ComplexFlatNumber),
                StreetName = Required(input.StreetName, "StreetName", "A street name is required.", errors),
                Town = Required(input.Town, "Town", "A town is required.", errors),
                PostalCode = Required(input.PostalCode, "PostalCode", "A postal code is required.", errors)
            };
        }

        // Assembles the TBL_employee_bank row, where the two numeric fields must be digits
        // only so payroll can use them as typed.
        private static EmployeeBank BuildBank(EmployeeCaptureInput input, List<EmployeeFieldError> errors)
        {
            var bank = new EmployeeBank
            {
                BankID = input.BankID,
                EmployeeID = EmployeeIDOrNew(input),
                BankName = Required(input.BankName, "BankName", "A bank name is required.", errors),
                AccountType = Required(input.AccountType, "AccountType", "An account type is required.", errors),
                AccountNumber = Required(input.AccountNumber, "AccountNumber", "An account number is required.", errors),
                BranchCode = Required(input.BranchCode, "BranchCode", "A branch code is required.", errors)
            };

            RequireDigits(bank.AccountNumber, "AccountNumber", "An account number may hold digits only.", errors);
            RequireDigits(bank.BranchCode, "BranchCode", "A branch code may hold digits only.", errors);

            return bank;
        }

        // Assembles the TBL_employee_contracts row. A contract captured here is live, so it
        // ends nowhere and carries no reason; Terminate is what fills those in.
        private static EmployeeContract BuildContract(EmployeeCaptureInput input, List<EmployeeFieldError> errors)
        {
            var contract = new EmployeeContract
            {
                ContractID = input.ContractID,
                EmployeeID = EmployeeIDOrNew(input),
                ContractType = Required(input.ContractType, "ContractType", "A contract type is required.", errors),
                Department = Required(input.Department, "Department", "A department is required.", errors),
                JobDescription = Required(input.JobDescription, "JobDescription", "A job description is required.", errors),
                EndDate = null,
                ReasonForEnding = string.Empty,
                HourlyRate = ValidateHourlyRate(input.HourlyRate, errors)
            };

            if (!input.StartDate.HasValue)
            {
                errors.Add(new EmployeeFieldError("StartDate", "A start date is required."));
            }
            else
            {
                contract.StartDate = input.StartDate.Value.Date;
            }

            return contract;
        }

        // Assembles the TBL_employee_spouses row, which exists only for a married employee.
        // Spouse details supplied by anybody else are rejected rather than quietly dropped.
        private static EmployeeSpouse BuildSpouse(EmployeeCaptureInput input, string maritalStatus, List<EmployeeFieldError> errors)
        {
            string name = Trim(input.SpouseName);
            string mobile = Trim(input.SpouseMobileNumber);
            bool isMarried = maritalStatus == MaritalStatuses.Married;

            if (!isMarried)
            {
                if (name.Length > 0)
                {
                    errors.Add(new EmployeeFieldError("SpouseName", "Only a married employee has a spouse."));
                }

                if (mobile.Length > 0)
                {
                    errors.Add(new EmployeeFieldError("SpouseMobileNumber", "Only a married employee has a spouse."));
                }

                return null;
            }

            if (name.Length == 0)
            {
                errors.Add(new EmployeeFieldError("SpouseName", "A married employee needs a spouse name."));
            }

            if (mobile.Length == 0)
            {
                errors.Add(new EmployeeFieldError("SpouseMobileNumber", "A married employee needs a spouse mobile number."));
            }

            return new EmployeeSpouse
            {
                SpouseID = input.SpouseID,
                EmployeeID = EmployeeIDOrNew(input),
                SpouseName = name,
                SpouseMobileNumber = mobile
            };
        }

        // Assembles the TBL_employee_family rows. There must be somebody to phone, so an
        // employee with no family row at all is rejected.
        private static List<EmployeeFamilyMember> BuildFamily(EmployeeCaptureInput input, List<EmployeeFieldError> errors)
        {
            var members = new List<EmployeeFamilyMember>();

            if (input.FamilyMembers == null || input.FamilyMembers.Count == 0)
            {
                errors.Add(new EmployeeFieldError("FamilyMembers", "At least one family contact is required."));
                return members;
            }

            foreach (FamilyMemberInput row in input.FamilyMembers)
            {
                var member = new EmployeeFamilyMember
                {
                    FamilyMemberID = row.FamilyMemberID,
                    EmployeeID = EmployeeIDOrNew(input),
                    FamilyMemberName = Trim(row.Name),
                    Relationship = Trim(row.Relationship),
                    MobileNumber = Optional(row.MobileNumber)
                };

                if (member.FamilyMemberName.Length == 0)
                {
                    errors.Add(new EmployeeFieldError("FamilyMemberName", "A family contact needs a name.", row.RowIndex));
                }

                if (member.Relationship.Length == 0)
                {
                    errors.Add(new EmployeeFieldError("Relationship", "A family contact needs a relationship.", row.RowIndex));
                }

                members.Add(member);
            }

            return members;
        }

        // Checks an ID number as a South African one: thirteen digits, a real date of birth
        // in the first six, and a check digit that agrees with the rest.
        private static string ValidateIDNumber(string text, List<EmployeeFieldError> errors)
        {
            string idNumber = Trim(text);

            if (idNumber.Length == 0)
            {
                errors.Add(new EmployeeFieldError("IDNumber", "An ID number is required."));
                return idNumber;
            }

            if (idNumber.Length != 13 || !IsAllDigits(idNumber))
            {
                errors.Add(new EmployeeFieldError("IDNumber", "An ID number is exactly thirteen digits."));
                return idNumber;
            }

            if (!HasRealDateOfBirth(idNumber))
            {
                errors.Add(new EmployeeFieldError("IDNumber", "The first six digits are not a real date of birth."));
                return idNumber;
            }

            if (!PassesLuhn(idNumber))
            {
                errors.Add(new EmployeeFieldError("IDNumber", "This ID number's check digit is wrong; a digit has probably been mistyped."));
            }

            return idNumber;
        }

        // Reads the YYMMDD at the front of an ID number as a date. The century is not stored,
        // so a date counts as real when it works in either the 1900s or the 2000s.
        private static bool HasRealDateOfBirth(string idNumber)
        {
            int year = int.Parse(idNumber.Substring(0, 2), CultureInfo.InvariantCulture);
            int month = int.Parse(idNumber.Substring(2, 2), CultureInfo.InvariantCulture);
            int day = int.Parse(idNumber.Substring(4, 2), CultureInfo.InvariantCulture);

            return IsRealDate(1900 + year, month, day) || IsRealDate(2000 + year, month, day);
        }

        // Reports whether a year, month and day name a day that exists.
        private static bool IsRealDate(int year, int month, int day)
        {
            if (month < 1 || month > 12)
            {
                return false;
            }

            return day >= 1 && day <= DateTime.DaysInMonth(year, month);
        }

        // Applies the Luhn check the last digit of a South African ID number encodes.
        private static bool PassesLuhn(string idNumber)
        {
            int total = 0;
            bool doubled = false;

            for (int index = idNumber.Length - 1; index >= 0; index--)
            {
                int digit = idNumber[index] - '0';

                if (doubled)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }

                total += digit;
                doubled = !doubled;
            }

            return total % 10 == 0;
        }

        // Matches the typed marital status against the permitted four.
        private static string ValidateMaritalStatus(string text, List<EmployeeFieldError> errors)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                errors.Add(new EmployeeFieldError("MaritalStatus", "A marital status is required."));
                return string.Empty;
            }

            string status;
            if (!MaritalStatuses.TryParse(text, out status))
            {
                errors.Add(new EmployeeFieldError("MaritalStatus", "Marital status must be Single, Married, Divorced or Widowed."));
                return string.Empty;
            }

            return status;
        }

        // Reads the number of dependents, which counts who the employee supports and so
        // cannot be negative, and need not match how many family contacts were captured.
        private static int ValidateDependents(string text, List<EmployeeFieldError> errors)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                errors.Add(new EmployeeFieldError("NumberOfDependents", "A number of dependents is required; enter 0 if there are none."));
                return 0;
            }

            int dependents;
            if (!int.TryParse(text.Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out dependents))
            {
                errors.Add(new EmployeeFieldError("NumberOfDependents", "The number of dependents must be a whole number."));
                return 0;
            }

            if (dependents < 0)
            {
                errors.Add(new EmployeeFieldError("NumberOfDependents", "The number of dependents cannot be negative."));
                return 0;
            }

            return dependents;
        }

        // Reads the hourly rate, which payroll multiplies the captured hours by and so must
        // be worth something.
        private static decimal ValidateHourlyRate(string text, List<EmployeeFieldError> errors)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                errors.Add(new EmployeeFieldError("HourlyRate", "An hourly rate is required."));
                return 0m;
            }

            decimal rate;
            if (!decimal.TryParse(text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out rate))
            {
                errors.Add(new EmployeeFieldError("HourlyRate", "The hourly rate is not an amount we understand. Try 45.50."));
                return 0m;
            }

            if (rate <= 0m)
            {
                errors.Add(new EmployeeFieldError("HourlyRate", "The hourly rate must be more than zero."));
                return 0m;
            }

            return rate;
        }

        // Reads the employee's identifier, which is zero for somebody being added because
        // the database has not yet given them one.
        private static int EmployeeIDOrNew(EmployeeCaptureInput input)
        {
            return input.EmployeeID.HasValue ? input.EmployeeID.Value : 0;
        }

        // Trims a required field and records the given problem when nothing is left.
        private static string Required(string text, string field, string message, List<EmployeeFieldError> errors)
        {
            string value = Trim(text);

            if (value.Length == 0)
            {
                errors.Add(new EmployeeFieldError(field, message));
            }

            return value;
        }

        // Records a problem when a field that must be digits holds anything else. A field
        // already reported as missing is left alone, so one mistake raises one error.
        private static void RequireDigits(string value, string field, string message, List<EmployeeFieldError> errors)
        {
            if (value.Length == 0 || IsAllDigits(value))
            {
                return;
            }

            errors.Add(new EmployeeFieldError(field, message));
        }

        // Reads an optional field as null rather than the empty string, so the nullable
        // columns hold NULL rather than something that reads as an answer.
        private static string Optional(string text)
        {
            string value = Trim(text);
            return value.Length == 0 ? null : value;
        }

        // Trims a field, treating a null the editor never filled in as blank.
        private static string Trim(string text)
        {
            return text == null ? string.Empty : text.Trim();
        }

        // Reports whether every character is a digit, with no signs, spaces or separators.
        private static bool IsAllDigits(string text)
        {
            foreach (char character in text)
            {
                if (!char.IsDigit(character))
                {
                    return false;
                }
            }

            return text.Length > 0;
        }
    }
}
