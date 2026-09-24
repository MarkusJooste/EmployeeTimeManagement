using EmployeeTimeManagement.Database;
using EmployeeTimeManagement.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeManagement.Controllers
{
    internal class EmployeeCaptureController
    {
        public int CreateEmployee(
            Employee employee,
            EmployeeDetails details,
            EmployeeAddress address,
            EmployeeSpouse spouse,
            EmployeeBank bank,
            EmployeeContract contract)
        {
            using (var connection = DatabaseConnection.GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // -------------------------------------------------
                    // 1. Create the employee
                    // -------------------------------------------------

                    const string employeeQuery = @"
INSERT INTO TBL_employees
(
    StoreID,
    Name,
    Surname,
    IDNumber,
    SARSNumber,
    MobileNumber,
    MaritialStatus,
    NumberOfDependents,
    BusinessDate,
    CapturedBy
)
VALUES
(
    @StoreID,
    @Name,
    @Surname,
    @IDNumber,
    @SARSNumber,
    @MobileNumber,
    @MaritialStatus,
    @NumberOfDependents,
    @BusinessDate,
    @CapturedBy
);";

                    int employeeID;

                    using (var command = new MySqlCommand(
                        employeeQuery,
                        connection,
                        transaction))
                    {
                        command.Parameters.AddWithValue(
                            "@StoreID",
                            employee.StoreID);

                        command.Parameters.AddWithValue(
                            "@Name",
                            employee.Name);

                        command.Parameters.AddWithValue(
                            "@Surname",
                            employee.Surname);

                        command.Parameters.AddWithValue(
                            "@IDNumber",
                            details.IDNumber);

                        command.Parameters.AddWithValue(
                            "@SARSNumber",
                            string.IsNullOrWhiteSpace(details.SARSNumber)
                                ? DBNull.Value
                                : details.SARSNumber);

                        command.Parameters.AddWithValue(
                            "@MobileNumber",
                            details.MobileNumber);

                        command.Parameters.AddWithValue(
                            "@MaritialStatus",
                            details.MaritialStatus);

                        command.Parameters.AddWithValue(
                            "@NumberOfDependents",
                            details.NumberOfDependents);

                        command.Parameters.AddWithValue(
                            "@BusinessDate",
                            details.BusinessDate);

                        command.Parameters.AddWithValue(
                            "@CapturedBy",
                            details.CapturedBy);

                        command.ExecuteNonQuery();

                        employeeID = Convert.ToInt32(command.LastInsertedId);
                    }

                    // Make the generated EmployeeID available to
                    // all the related records.
                    details.EmployeeID = employeeID;
                    address.EmployeeID = employeeID;
                    spouse.EmployeeID = employeeID;
                    bank.EmployeeID = employeeID;
                    contract.EmployeeID = employeeID;

                    // -------------------------------------------------
                    // 2. Address
                    // -------------------------------------------------

                    const string addressQuery = @"
INSERT INTO TBL_employee_addresses
(
    EmployeeID,
    HouseFlatNumber,
    ComplexFlatNumber,
    StreetName,
    Town,
    PostalCode
)
VALUES
(
    @EmployeeID,
    @HouseFlatNumber,
    @ComplexFlatNumber,
    @StreetName,
    @Town,
    @PostalCode
);";

                    using (var command = new MySqlCommand(
                        addressQuery,
                        connection,
                        transaction))
                    {
                        command.Parameters.AddWithValue(
                            "@EmployeeID",
                            address.EmployeeID);

                        command.Parameters.AddWithValue(
                            "@HouseFlatNumber",
                            address.HouseFlatNumber);

                        command.Parameters.AddWithValue(
                            "@ComplexFlatNumber",
                            address.ComplexFlatNumber);

                        command.Parameters.AddWithValue(
                            "@StreetName",
                            address.StreetName);

                        command.Parameters.AddWithValue(
                            "@Town",
                            address.Town);

                        command.Parameters.AddWithValue(
                            "@PostalCode",
                            address.PostalCode);

                        command.ExecuteNonQuery();
                    }

                    // -------------------------------------------------
                    // 3. Spouse
                    // -------------------------------------------------

                    const string spouseQuery = @"
INSERT INTO TBL_employee_spouses
(
    EmployeeID,
    SpouseName,
    SpouseMobileNumber
)
VALUES
(
    @EmployeeID,
    @SpouseName,
    @SpouseMobileNumber
);";

                    using (var command = new MySqlCommand(
                        spouseQuery,
                        connection,
                        transaction))
                    {
                        command.Parameters.AddWithValue(
                            "@EmployeeID",
                            spouse.EmployeeID);

                        command.Parameters.AddWithValue(
                            "@SpouseName",
                            spouse.SpouseName);

                        command.Parameters.AddWithValue(
                            "@SpouseMobileNumber",
                            spouse.SpouseMobileNumber);

                        command.ExecuteNonQuery();
                    }

                    // -------------------------------------------------
                    // 4. Bank
                    // -------------------------------------------------

                    const string bankQuery = @"
INSERT INTO TBL_employee_bank
(
    EmployeeID,
    BankName,
    AccountType,
    AccountNumber,
    BranchCode
)
VALUES
(
    @EmployeeID,
    @BankName,
    @AccountType,
    @AccountNumber,
    @BranchCode
);";

                    using (var command = new MySqlCommand(
                        bankQuery,
                        connection,
                        transaction))
                    {
                        command.Parameters.AddWithValue(
                            "@EmployeeID",
                            bank.EmployeeID);

                        command.Parameters.AddWithValue(
                            "@BankName",
                            bank.BankName);

                        command.Parameters.AddWithValue(
                            "@AccountType",
                            bank.AccountType);

                        command.Parameters.AddWithValue(
                            "@AccountNumber",
                            bank.AccountNumber);

                        command.Parameters.AddWithValue(
                            "@BranchCode",
                            bank.BranchCode);

                        command.ExecuteNonQuery();
                    }

                    // -------------------------------------------------
                    // 5. Contract
                    // -------------------------------------------------

                    const string contractQuery = @"
INSERT INTO TBL_employee_contracts
(
    EmployeeID,
    ContractType,
    StartDate,
    EndDate,
    Department,
    JobDescription,
    ReasonForEnding,
    HourlyRate
)
VALUES
(
    @EmployeeID,
    @ContractType,
    @StartDate,
    @EndDate,
    @Department,
    @JobDescription,
    @ReasonForEnding,
    @HourlyRate
);";

                    using (var command = new MySqlCommand(
                        contractQuery,
                        connection,
                        transaction))
                    {
                        command.Parameters.AddWithValue(
                            "@EmployeeID",
                            contract.EmployeeID);

                        command.Parameters.AddWithValue(
                            "@ContractType",
                            contract.ContractType);

                        command.Parameters.AddWithValue(
                            "@StartDate",
                            contract.StartDate);

                        command.Parameters.AddWithValue(
                            "@EndDate",
                            contract.EndDate.HasValue
                                ? contract.EndDate.Value
                                : DBNull.Value);

                        command.Parameters.AddWithValue(
                            "@Department",
                            contract.Department);

                        command.Parameters.AddWithValue(
                            "@JobDescription",
                            contract.JobDescription);

                        command.Parameters.AddWithValue(
                            "@ReasonForEnding",
                            string.IsNullOrWhiteSpace(contract.ReasonForEnding)
                                ? ""
                                : contract.ReasonForEnding);

                        command.Parameters.AddWithValue(
                            "@HourlyRate",
                            contract.HourlyRate);

                        command.ExecuteNonQuery();
                    }

                    // Everything succeeded.
                    transaction.Commit();

                    return employeeID;
                }
                catch
                {
                    // If ANY insert failed, undo everything.
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public void AddFamilyMember(
            EmployeeFamilyMember familyMember)
        {
            const string query = @"
INSERT INTO TBL_employee_family
(
    EmployeeID,
    FamilyMemberName,
    MobileNumber,
    Relationship
)
VALUES
(
    @EmployeeID,
    @FamilyMemberName,
    @MobileNumber,
    @Relationship
);";

            using (var connection = DatabaseConnection.GetConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue(
                    "@EmployeeID",
                    familyMember.EmployeeID);

                command.Parameters.AddWithValue(
                    "@FamilyMemberName",
                    familyMember.FamilyMemberName);

                command.Parameters.AddWithValue(
                    "@MobileNumber",
                    string.IsNullOrWhiteSpace(familyMember.MobileNumber)
                        ? DBNull.Value
                        : familyMember.MobileNumber);

                command.Parameters.AddWithValue(
                    "@Relationship",
                    familyMember.Relationship);

                command.ExecuteNonQuery();
            }
        }
    }
}
