using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class EmployeeCaptureTests
    {
        private const int StoreID = 3;
        private const int CapturedBy = 7;

        // Thirteen digits, born 1990-01-01, and passes the Luhn check.
        private const string ValidIDNumber = "9001015009086";

        private static EmployeeCaptureInput SingleEmployee()
        {
            return new EmployeeCaptureInput
            {
                Name = "Thandi",
                Surname = "Mokoena",
                IDNumber = ValidIDNumber,
                SARSNumber = "1234567890",
                MobileNumber = "0821234567",
                MaritalStatus = "Single",
                NumberOfDependents = "0",

                HouseFlatNumber = "12",
                ComplexFlatNumber = "",
                StreetName = "Long Street",
                Town = "Cape Town",
                PostalCode = "8001",

                BankName = "Capitec",
                AccountType = "Savings",
                AccountNumber = "1234567890",
                BranchCode = "470010",

                ContractType = "Permanent",
                StartDate = new DateTime(2025, 1, 6),
                Department = "Kitchen",
                JobDescription = "Cook",
                HourlyRate = "45.50",

                SpouseName = "",
                SpouseMobileNumber = "",

                FamilyMembers = new List<FamilyMemberInput>
                {
                    new FamilyMemberInput { RowIndex = 0, Name = "Sipho Mokoena", MobileNumber = "0839876543", Relationship = "Brother" }
                }
            };
        }

        private static EmployeeCaptureInput MarriedEmployee()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.MaritalStatus = "Married";
            input.SpouseName = "Lerato Mokoena";
            input.SpouseMobileNumber = "0845551234";
            return input;
        }

        private static EmployeeCaptureResult Build(EmployeeCaptureInput input)
        {
            return EmployeeCapture.Build(StoreID, CapturedBy, input);
        }

        private static bool HasErrorFor(EmployeeCaptureResult result, string field)
        {
            return result.Errors.Any(e => e.Field == field);
        }

        // Names every failing field, so a failing test says which rule fired rather than just "not valid".
        private static string FailingFields(EmployeeCaptureResult result)
        {
            return string.Join(", ", result.Errors.Select(e => e.Field + ": " + e.Message).ToArray());
        }

        // --- Happy path ---

        [Test]
        public void ACompleteMarriedEmployeeProducesARecordWithASpouse()
        {
            EmployeeCaptureResult result = Build(MarriedEmployee());

            Assert.That(result.IsValid, Is.True, FailingFields(result));

            EmployeeRecord record = result.Record;
            Assert.That(record.Employee.Name, Is.EqualTo("Thandi"));
            Assert.That(record.Employee.Surname, Is.EqualTo("Mokoena"));
            Assert.That(record.Employee.IDNumber, Is.EqualTo(ValidIDNumber));
            Assert.That(record.Employee.SARSNumber, Is.EqualTo("1234567890"));
            Assert.That(record.Employee.MobileNumber, Is.EqualTo("0821234567"));
            Assert.That(record.Employee.MaritalStatus, Is.EqualTo("Married"));
            Assert.That(record.Employee.NumberOfDependents, Is.EqualTo(0));
            Assert.That(record.Employee.StoreID, Is.EqualTo(StoreID));
            Assert.That(record.Employee.CapturedBy, Is.EqualTo(CapturedBy));
            Assert.That(record.Employee.BusinessDate, Is.EqualTo(DateTime.Today));

            Assert.That(record.Spouse, Is.Not.Null);
            Assert.That(record.Spouse.SpouseName, Is.EqualTo("Lerato Mokoena"));
            Assert.That(record.Spouse.SpouseMobileNumber, Is.EqualTo("0845551234"));
        }

        [Test]
        public void ACompleteSingleEmployeeProducesARecordWithNoSpouse()
        {
            EmployeeCaptureResult result = Build(SingleEmployee());

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Spouse, Is.Null);
        }

        [Test]
        public void ACompleteRecordCarriesTheAddressBankAndContract()
        {
            EmployeeCaptureResult result = Build(SingleEmployee());

            Assert.That(result.IsValid, Is.True, FailingFields(result));

            Assert.That(result.Record.Address.HouseFlatNumber, Is.EqualTo("12"));
            Assert.That(result.Record.Address.ComplexFlatNumber, Is.EqualTo(string.Empty));
            Assert.That(result.Record.Address.StreetName, Is.EqualTo("Long Street"));
            Assert.That(result.Record.Address.Town, Is.EqualTo("Cape Town"));
            Assert.That(result.Record.Address.PostalCode, Is.EqualTo("8001"));

            Assert.That(result.Record.Bank.BankName, Is.EqualTo("Capitec"));
            Assert.That(result.Record.Bank.AccountType, Is.EqualTo("Savings"));
            Assert.That(result.Record.Bank.AccountNumber, Is.EqualTo("1234567890"));
            Assert.That(result.Record.Bank.BranchCode, Is.EqualTo("470010"));

            Assert.That(result.Record.Contract.ContractType, Is.EqualTo("Permanent"));
            Assert.That(result.Record.Contract.StartDate, Is.EqualTo(new DateTime(2025, 1, 6)));
            Assert.That(result.Record.Contract.Department, Is.EqualTo("Kitchen"));
            Assert.That(result.Record.Contract.JobDescription, Is.EqualTo("Cook"));
            Assert.That(result.Record.Contract.HourlyRate, Is.EqualTo(45.50m));

            // A live contract has not ended, and the reason column cannot hold null.
            Assert.That(result.Record.Contract.EndDate, Is.Null);
            Assert.That(result.Record.Contract.ReasonForEnding, Is.EqualTo(string.Empty));
        }

        [Test]
        public void AnInvalidRecordIsNotProducedAtAll()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.Surname = "";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Record, Is.Null);
        }

        // --- Spouse ---

        [Test]
        public void ASpouseSuppliedForAnUnmarriedEmployeeIsRejected()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.SpouseName = "Lerato Mokoena";
            input.SpouseMobileNumber = "0845551234";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(HasErrorFor(result, "SpouseName"), Is.True, FailingFields(result));
        }

        [Test]
        public void AMarriedEmployeeWithNoSpouseDetailsIsRejected()
        {
            EmployeeCaptureInput input = MarriedEmployee();
            input.SpouseName = "";
            input.SpouseMobileNumber = "";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(HasErrorFor(result, "SpouseName"), Is.True, FailingFields(result));
            Assert.That(HasErrorFor(result, "SpouseMobileNumber"), Is.True, FailingFields(result));
        }

        [Test]
        public void AMarriedEmployeeWithNoSpouseMobileNumberIsRejected()
        {
            EmployeeCaptureInput input = MarriedEmployee();
            input.SpouseMobileNumber = "";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(HasErrorFor(result, "SpouseMobileNumber"), Is.True, FailingFields(result));
        }

        // --- Required fields, one case per field ---

        [TestCase("Name")]
        [TestCase("Surname")]
        [TestCase("IDNumber")]
        [TestCase("MobileNumber")]
        [TestCase("MaritalStatus")]
        [TestCase("NumberOfDependents")]
        [TestCase("HouseFlatNumber")]
        [TestCase("StreetName")]
        [TestCase("Town")]
        [TestCase("PostalCode")]
        [TestCase("BankName")]
        [TestCase("AccountType")]
        [TestCase("AccountNumber")]
        [TestCase("BranchCode")]
        [TestCase("ContractType")]
        [TestCase("Department")]
        [TestCase("JobDescription")]
        [TestCase("HourlyRate")]
        public void ARequiredFieldLeftBlankIsRejected(string field)
        {
            EmployeeCaptureInput input = SingleEmployee();
            Blank(input, field);

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False, field + " was accepted while blank.");
            Assert.That(HasErrorFor(result, field), Is.True, FailingFields(result));
        }

        [Test]
        public void AMissingStartDateIsRejected()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.StartDate = null;

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(HasErrorFor(result, "StartDate"), Is.True, FailingFields(result));
        }

        [Test]
        public void AnEmptyComplexFlatNumberIsAccepted()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.ComplexFlatNumber = "";

            Assert.That(Build(input).IsValid, Is.True);
        }

        [Test]
        public void AComplexFlatNumberIsKeptWhenSupplied()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.ComplexFlatNumber = "Unit 4B";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Address.ComplexFlatNumber, Is.EqualTo("Unit 4B"));
        }

        [Test]
        public void AMissingSARSNumberIsAccepted()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.SARSNumber = "";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Employee.SARSNumber, Is.Null);
        }

        // --- Family members ---

        [Test]
        public void ZeroFamilyMembersIsRejected()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.FamilyMembers = new List<FamilyMemberInput>();

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(HasErrorFor(result, "FamilyMembers"), Is.True, FailingFields(result));
        }

        [Test]
        public void OneFamilyMemberIsAccepted()
        {
            EmployeeCaptureResult result = Build(SingleEmployee());

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.FamilyMembers, Has.Count.EqualTo(1));
            Assert.That(result.Record.FamilyMembers[0].FamilyMemberName, Is.EqualTo("Sipho Mokoena"));
            Assert.That(result.Record.FamilyMembers[0].Relationship, Is.EqualTo("Brother"));
        }

        [Test]
        public void SeveralFamilyMembersAreAccepted()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.FamilyMembers.Add(new FamilyMemberInput { RowIndex = 1, Name = "Nomsa Mokoena", MobileNumber = "0827778888", Relationship = "Mother" });
            input.FamilyMembers.Add(new FamilyMemberInput { RowIndex = 2, Name = "Jabu Mokoena", MobileNumber = "", Relationship = "Father" });

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.FamilyMembers, Has.Count.EqualTo(3));
        }

        [Test]
        public void AFamilyMemberWithNoRelationshipIsRejected()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.FamilyMembers[0].Relationship = "";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(HasErrorFor(result, "Relationship"), Is.True, FailingFields(result));
        }

        [Test]
        public void AFamilyMemberWithNoNameIsRejected()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.FamilyMembers[0].Name = "";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(HasErrorFor(result, "FamilyMemberName"), Is.True, FailingFields(result));
        }

        [Test]
        public void AFamilyMemberWithNoMobileNumberIsAccepted()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.FamilyMembers[0].MobileNumber = "";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.FamilyMembers[0].MobileNumber, Is.Null);
        }

        [Test]
        public void AFailingFamilyMemberNamesItsOwnRow()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.FamilyMembers.Add(new FamilyMemberInput { RowIndex = 1, Name = "Nomsa Mokoena", MobileNumber = "", Relationship = "" });

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.Field == "Relationship" && e.FamilyRowIndex == 1), Is.True, FailingFields(result));
        }

        // --- ID number ---

        [Test]
        public void ACorrectIDNumberIsAccepted()
        {
            Assert.That(Build(SingleEmployee()).IsValid, Is.True);
        }

        [TestCase("900101500908", TestName = "TwelveDigitsIsRejected")]
        [TestCase("90010150090861", TestName = "FourteenDigitsIsRejected")]
        [TestCase("90010150090A6", TestName = "NonDigitCharactersAreRejected")]
        [TestCase("9013015009081", TestName = "MonthThirteenIsRejected")]
        [TestCase("9002315009081", TestName = "ThirtyFirstOfFebruaryIsRejected")]
        [TestCase("9001015009087", TestName = "AWrongCheckDigitIsRejected")]
        public void AnUnusableIDNumberIsRejected(string idNumber)
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.IDNumber = idNumber;

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False, idNumber + " was accepted.");
            Assert.That(HasErrorFor(result, "IDNumber"), Is.True, FailingFields(result));
        }

        [Test]
        public void ALeapDayOfBirthIsAcceptedWhenOnlyOneCenturyMakesItReal()
        {
            // 29 February in year 00: no such day in 1900, but there is one in 2000.
            EmployeeCaptureInput input = SingleEmployee();
            input.IDNumber = "0002295009084";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
        }

        // --- Marital status ---

        [TestCase("Single")]
        [TestCase("Married")]
        [TestCase("Divorced")]
        [TestCase("Widowed")]
        public void EveryPermittedMaritalStatusIsAccepted(string status)
        {
            EmployeeCaptureInput input = status == "Married" ? MarriedEmployee() : SingleEmployee();
            input.MaritalStatus = status;

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Employee.MaritalStatus, Is.EqualTo(status));
        }

        [Test]
        public void AMaritalStatusStoredInAnotherCasingIsReadAsThePermittedOne()
        {
            EmployeeCaptureInput input = MarriedEmployee();
            input.MaritalStatus = "married";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Employee.MaritalStatus, Is.EqualTo("Married"));
            Assert.That(result.Record.Spouse, Is.Not.Null);
        }

        [TestCase("Separated")]
        [TestCase("0")]
        public void AMaritalStatusOutsideTheFourPermittedIsRejected(string status)
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.MaritalStatus = status;

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(HasErrorFor(result, "MaritalStatus"), Is.True, FailingFields(result));
        }

        // --- Dependents ---

        [Test]
        public void ZeroDependentsIsAccepted()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.NumberOfDependents = "0";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Employee.NumberOfDependents, Is.EqualTo(0));
        }

        [TestCase("-1")]
        [TestCase("two")]
        [TestCase("1.5")]
        public void AnUnusableNumberOfDependentsIsRejected(string dependents)
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.NumberOfDependents = dependents;

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False, dependents + " was accepted.");
            Assert.That(HasErrorFor(result, "NumberOfDependents"), Is.True, FailingFields(result));
        }

        // --- Hourly rate ---

        [TestCase("0")]
        [TestCase("0.00")]
        [TestCase("-5")]
        [TestCase("free")]
        public void AnUnusableHourlyRateIsRejected(string rate)
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.HourlyRate = rate;

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False, rate + " was accepted.");
            Assert.That(HasErrorFor(result, "HourlyRate"), Is.True, FailingFields(result));
        }

        // --- Bank digits ---

        [TestCase("12345A7890")]
        [TestCase("1234 567890")]
        public void AnAccountNumberThatIsNotAllDigitsIsRejected(string accountNumber)
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.AccountNumber = accountNumber;

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False, accountNumber + " was accepted.");
            Assert.That(HasErrorFor(result, "AccountNumber"), Is.True, FailingFields(result));
        }

        [Test]
        public void ABranchCodeThatIsNotAllDigitsIsRejected()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.BranchCode = "4700X0";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(HasErrorFor(result, "BranchCode"), Is.True, FailingFields(result));
        }

        // --- Several problems at once ---

        [Test]
        public void EveryInvalidFieldReportsItsOwnError()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.Name = "";
            input.IDNumber = "123";
            input.BranchCode = "abc";
            input.HourlyRate = "0";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(HasErrorFor(result, "Name"), Is.True, FailingFields(result));
            Assert.That(HasErrorFor(result, "IDNumber"), Is.True, FailingFields(result));
            Assert.That(HasErrorFor(result, "BranchCode"), Is.True, FailingFields(result));
            Assert.That(HasErrorFor(result, "HourlyRate"), Is.True, FailingFields(result));
        }

        [Test]
        public void EveryErrorCarriesAMessageNamingTheProblem()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.Name = "";
            input.Town = "";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors.All(e => !string.IsNullOrWhiteSpace(e.Message)), Is.True);
        }

        // --- Whitespace ---

        [Test]
        public void FieldsAreTrimmedBeforeTheyAreStored()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.Name = "  Thandi  ";
            input.Surname = "  Mokoena  ";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Employee.Name, Is.EqualTo("Thandi"));
            Assert.That(result.Record.Employee.Surname, Is.EqualTo("Mokoena"));
        }

        [Test]
        public void AFieldHoldingOnlySpacesCountsAsBlank()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.Town = "   ";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(HasErrorFor(result, "Town"), Is.True, FailingFields(result));
        }

        // --- Opening Balance ---

        [Test]
        public void OpeningPTODaysDefaultsToZeroWhenLeftBlank()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.OpeningPTODays = "";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Contract.OpeningPTODays, Is.EqualTo(0));
        }

        [Test]
        public void OpeningPTODaysIsCapturedWhenSupplied()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.OpeningPTODays = "12";

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Contract.OpeningPTODays, Is.EqualTo(12));
        }

        [Test]
        public void AnOpeningPTODaysAtTheCapIsAccepted()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.OpeningPTODays = LeaveBalanceCalculator.PTOCap.ToString(CultureInfo.InvariantCulture);

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Contract.OpeningPTODays, Is.EqualTo(LeaveBalanceCalculator.PTOCap));
        }

        [Test]
        public void AnOpeningPTODaysAboveTheCapIsRejected()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.OpeningPTODays = (LeaveBalanceCalculator.PTOCap + 1).ToString(CultureInfo.InvariantCulture);

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False);
            Assert.That(HasErrorFor(result, "OpeningPTODays"), Is.True, FailingFields(result));
        }

        [TestCase("-1")]
        [TestCase("two")]
        [TestCase("1.5")]
        public void AnUnusableOpeningPTODaysIsRejected(string days)
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.OpeningPTODays = days;

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.False, days + " was accepted.");
            Assert.That(HasErrorFor(result, "OpeningPTODays"), Is.True, FailingFields(result));
        }

        [Test]
        public void OpeningBalanceAsAtIsNullWhenNotSupplied()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.OpeningBalanceAsAt = null;

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Contract.OpeningBalanceAsAt, Is.Null);
        }

        [Test]
        public void OpeningBalanceAsAtIsCapturedWhenSupplied()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.OpeningBalanceAsAt = new DateTime(2022, 3, 1);

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Contract.OpeningBalanceAsAt, Is.EqualTo(new DateTime(2022, 3, 1)));
        }

        // --- Update ---

        [Test]
        public void AnExistingEmployeeKeepsTheirIdentifiersOnTheRecord()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.EmployeeID = 42;
            input.AddressID = 5;
            input.BankID = 6;
            input.ContractID = 7;

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Employee.EmployeeID, Is.EqualTo(42));
            Assert.That(result.Record.Address.AddressID, Is.EqualTo(5));
            Assert.That(result.Record.Bank.BankID, Is.EqualTo(6));
            Assert.That(result.Record.Contract.ContractID, Is.EqualTo(7));
        }

        [Test]
        public void AFamilyContactAlreadyOnRecordKeepsTheirRowIdentifier()
        {
            EmployeeCaptureInput input = SingleEmployee();
            input.FamilyMembers[0].FamilyMemberID = 11;
            input.FamilyMembers.Add(new FamilyMemberInput { RowIndex = 1, Name = "Nomsa Mokoena", MobileNumber = "", Relationship = "Mother" });

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.FamilyMembers[0].FamilyMemberID, Is.EqualTo(11));

            // A contact just added has no row of their own yet.
            Assert.That(result.Record.FamilyMembers[1].FamilyMemberID, Is.Null);
        }

        [Test]
        public void AMarriedEmployeeKeepsAnExistingSpouseRowIdentifier()
        {
            EmployeeCaptureInput input = MarriedEmployee();
            input.SpouseID = 8;

            EmployeeCaptureResult result = Build(input);

            Assert.That(result.IsValid, Is.True, FailingFields(result));
            Assert.That(result.Record.Spouse.SpouseID, Is.EqualTo(8));
        }

        // Blanks the one field a required-field test is about, leaving everything else valid.
        private static void Blank(EmployeeCaptureInput input, string field)
        {
            switch (field)
            {
                case "Name": input.Name = ""; break;
                case "Surname": input.Surname = ""; break;
                case "IDNumber": input.IDNumber = ""; break;
                case "MobileNumber": input.MobileNumber = ""; break;
                case "MaritalStatus": input.MaritalStatus = ""; break;
                case "NumberOfDependents": input.NumberOfDependents = ""; break;
                case "HouseFlatNumber": input.HouseFlatNumber = ""; break;
                case "StreetName": input.StreetName = ""; break;
                case "Town": input.Town = ""; break;
                case "PostalCode": input.PostalCode = ""; break;
                case "BankName": input.BankName = ""; break;
                case "AccountType": input.AccountType = ""; break;
                case "AccountNumber": input.AccountNumber = ""; break;
                case "BranchCode": input.BranchCode = ""; break;
                case "ContractType": input.ContractType = ""; break;
                case "Department": input.Department = ""; break;
                case "JobDescription": input.JobDescription = ""; break;
                case "HourlyRate": input.HourlyRate = ""; break;
                default: throw new ArgumentException("No such field: " + field, "field");
            }
        }
    }
}
