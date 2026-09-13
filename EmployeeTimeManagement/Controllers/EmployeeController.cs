using System;
using System.Collections.Generic;
using EmployeeTimeManagement.Database;
using EmployeeTimeManagement.Models;
using MySqlConnector;

namespace EmployeeTimeManagement.Controllers
{
    internal class EmployeeController
    {
        // Returns every employee at one store with their contract's job description and end date, ordered the way a manager reads a register.
        public List<EmployeeListItem> GetListByStore(int storeID)
        {
            // The schema permits several contracts per employee even though this application writes one, so only the latest is joined.
            const string query = @"SELECT e.EmployeeID,
       e.StoreID,
       e.Name,
       e.Surname,
       e.IDNumber,
       e.MobileNumber,
       c.JobDescription,
       c.EndDate
FROM TBL_employees e
LEFT JOIN TBL_employee_contracts c
       ON c.EmployeeID = e.EmployeeID
      AND c.ContractID = (SELECT MAX(latest.ContractID)
                          FROM TBL_employee_contracts latest
                          WHERE latest.EmployeeID = e.EmployeeID)
WHERE e.StoreID = @StoreID
ORDER BY e.Surname, e.Name;";

            var employees = new List<EmployeeListItem>();

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StoreID", storeID);

                    using (var reader = command.ExecuteReader())
                    {
                        int employeeIndex = reader.GetOrdinal("EmployeeID");
                        int storeIndex = reader.GetOrdinal("StoreID");
                        int nameIndex = reader.GetOrdinal("Name");
                        int surnameIndex = reader.GetOrdinal("Surname");
                        int idNumberIndex = reader.GetOrdinal("IDNumber");
                        int mobileIndex = reader.GetOrdinal("MobileNumber");
                        int jobIndex = reader.GetOrdinal("JobDescription");
                        int endDateIndex = reader.GetOrdinal("EndDate");

                        while (reader.Read())
                        {
                            var item = new EmployeeListItem();

                            item.EmployeeID = reader.GetInt32(employeeIndex);
                            item.StoreID = reader.GetInt32(storeIndex);
                            item.Name = ReadText(reader, nameIndex);
                            item.Surname = ReadText(reader, surnameIndex);
                            item.IDNumber = ReadText(reader, idNumberIndex);
                            item.MobileNumber = ReadText(reader, mobileIndex);
                            item.JobDescription = ReadText(reader, jobIndex);

                            // An employee with no contract row has no end date, which reads as still employed.
                            if (reader.IsDBNull(endDateIndex))
                            {
                                item.ContractEndDate = null;
                            }
                            else
                            {
                                item.ContractEndDate = reader.GetDateTime(endDateIndex);
                            }

                            employees.Add(item);
                        }

                        return employees;
                    }
                }
            }
        }

        // Reads a nullable text column as the empty string rather than null, so the grid and the search never see one.
        private static string ReadText(MySqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
            {
                return string.Empty;
            }

            return reader.GetString(index);
        }

        // Returns whoever already holds an ID number, anywhere in the database, or null when
        // nobody does. The unique index on IDNumber spans every store, so the answer can be
        // an employee this manager cannot see.
        public EmployeeIDNumberOwner FindByIDNumber(string idNumber)
        {
            const string query = @"SELECT StoreID, Name, Surname
FROM TBL_employees
WHERE IDNumber = @IDNumber
LIMIT 1;";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDNumber", idNumber);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        var owner = new EmployeeIDNumberOwner();

                        owner.StoreID = reader.GetInt32(reader.GetOrdinal("StoreID"));
                        owner.Name = ReadText(reader, reader.GetOrdinal("Name"));
                        owner.Surname = ReadText(reader, reader.GetOrdinal("Surname"));

                        return owner;
                    }
                }
            }
        }

        // Writes a new employee and every table hanging off them as one transaction, so a
        // failure part-way leaves nothing behind rather than an employee payroll cannot pay.
        public void Insert(EmployeeRecord record)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int employeeID = InsertEmployee(connection, transaction, record.Employee);

                        // The children hang off the identifier the database has just handed out.
                        record.Employee.EmployeeID = employeeID;
                        record.Address.EmployeeID = employeeID;
                        record.Bank.EmployeeID = employeeID;
                        record.Contract.EmployeeID = employeeID;

                        InsertAddress(connection, transaction, record.Address);
                        InsertBank(connection, transaction, record.Bank);
                        InsertContract(connection, transaction, record.Contract);

                        // Only a married employee has a spouse row to write.
                        if (record.Spouse != null)
                        {
                            record.Spouse.EmployeeID = employeeID;
                            InsertSpouse(connection, transaction, record.Spouse);
                        }

                        foreach (EmployeeFamilyMember member in record.FamilyMembers)
                        {
                            member.EmployeeID = employeeID;
                            InsertFamilyMember(connection, transaction, member);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Inserts the TBL_employees row and returns the identifier the five child tables hang off.
        private static int InsertEmployee(MySqlConnection connection, MySqlTransaction transaction, Employee employee)
        {
            const string query = @"INSERT INTO TBL_employees
    (StoreID, Name, Surname, IDNumber, SARSNumber, MobileNumber,
     MaritialStatus, NumberOfDependents, BusinessDate, CapturedBy)
VALUES
    (@StoreID, @Name, @Surname, @IDNumber, @SARSNumber, @MobileNumber,
     @MaritalStatus, @NumberOfDependents, @BusinessDate, @CapturedBy);";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@StoreID", employee.StoreID);
                command.Parameters.AddWithValue("@Name", employee.Name);
                command.Parameters.AddWithValue("@Surname", employee.Surname);
                command.Parameters.AddWithValue("@IDNumber", employee.IDNumber);
                command.Parameters.AddWithValue("@SARSNumber", ToParameter(employee.SARSNumber));
                command.Parameters.AddWithValue("@MobileNumber", employee.MobileNumber);

                // The column is misspelled in the schema; the property is not.
                command.Parameters.AddWithValue("@MaritalStatus", employee.MaritalStatus);

                command.Parameters.AddWithValue("@NumberOfDependents", employee.NumberOfDependents);
                command.Parameters.AddWithValue("@BusinessDate", employee.BusinessDate.Date);
                command.Parameters.AddWithValue("@CapturedBy", employee.CapturedBy);

                command.ExecuteNonQuery();

                return (int)command.LastInsertedId;
            }
        }

        // Inserts the employee's TBL_employee_addresses row.
        private static void InsertAddress(MySqlConnection connection, MySqlTransaction transaction, EmployeeAddress address)
        {
            const string query = @"INSERT INTO TBL_employee_addresses
    (EmployeeID, HouseFlatNumber, ComplexFlatNumber, StreetName, Town, PostalCode)
VALUES
    (@EmployeeID, @HouseFlatNumber, @ComplexFlatNumber, @StreetName, @Town, @PostalCode);";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@EmployeeID", address.EmployeeID);
                command.Parameters.AddWithValue("@HouseFlatNumber", address.HouseFlatNumber);
                command.Parameters.AddWithValue("@ComplexFlatNumber", address.ComplexFlatNumber);
                command.Parameters.AddWithValue("@StreetName", address.StreetName);
                command.Parameters.AddWithValue("@Town", address.Town);
                command.Parameters.AddWithValue("@PostalCode", address.PostalCode);

                command.ExecuteNonQuery();

                address.AddressID = (int)command.LastInsertedId;
            }
        }

        // Inserts the employee's TBL_employee_bank row, which is what makes them payable.
        private static void InsertBank(MySqlConnection connection, MySqlTransaction transaction, EmployeeBank bank)
        {
            const string query = @"INSERT INTO TBL_employee_bank
    (EmployeeID, BankName, AccountType, AccountNumber, BranchCode)
VALUES
    (@EmployeeID, @BankName, @AccountType, @AccountNumber, @BranchCode);";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@EmployeeID", bank.EmployeeID);
                command.Parameters.AddWithValue("@BankName", bank.BankName);
                command.Parameters.AddWithValue("@AccountType", bank.AccountType);
                command.Parameters.AddWithValue("@AccountNumber", bank.AccountNumber);
                command.Parameters.AddWithValue("@BranchCode", bank.BranchCode);

                command.ExecuteNonQuery();

                bank.BankID = (int)command.LastInsertedId;
            }
        }

        // Inserts the employee's TBL_employee_contracts row, live until Terminate ends it.
        private static void InsertContract(MySqlConnection connection, MySqlTransaction transaction, EmployeeContract contract)
        {
            const string query = @"INSERT INTO TBL_employee_contracts
    (EmployeeID, ContractType, StartDate, EndDate, Department, JobDescription, ReasonForEnding, HourlyRate)
VALUES
    (@EmployeeID, @ContractType, @StartDate, @EndDate, @Department, @JobDescription, @ReasonForEnding, @HourlyRate);";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@EmployeeID", contract.EmployeeID);
                command.Parameters.AddWithValue("@ContractType", contract.ContractType);
                command.Parameters.AddWithValue("@StartDate", contract.StartDate.Date);
                command.Parameters.AddWithValue("@EndDate", ToParameter(contract.EndDate));
                command.Parameters.AddWithValue("@Department", contract.Department);
                command.Parameters.AddWithValue("@JobDescription", contract.JobDescription);
                command.Parameters.AddWithValue("@ReasonForEnding", contract.ReasonForEnding);
                command.Parameters.AddWithValue("@HourlyRate", contract.HourlyRate);

                command.ExecuteNonQuery();

                contract.ContractID = (int)command.LastInsertedId;
            }
        }

        // Inserts the employee's TBL_employee_spouses row, written only for a married employee.
        private static void InsertSpouse(MySqlConnection connection, MySqlTransaction transaction, EmployeeSpouse spouse)
        {
            const string query = @"INSERT INTO TBL_employee_spouses
    (EmployeeID, SpouseName, SpouseMobileNumber)
VALUES
    (@EmployeeID, @SpouseName, @SpouseMobileNumber);";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@EmployeeID", spouse.EmployeeID);
                command.Parameters.AddWithValue("@SpouseName", spouse.SpouseName);
                command.Parameters.AddWithValue("@SpouseMobileNumber", spouse.SpouseMobileNumber);

                command.ExecuteNonQuery();

                spouse.SpouseID = (int)command.LastInsertedId;
            }
        }

        // Inserts one TBL_employee_family row: somebody to phone about this employee.
        private static void InsertFamilyMember(MySqlConnection connection, MySqlTransaction transaction, EmployeeFamilyMember member)
        {
            const string query = @"INSERT INTO TBL_employee_family
    (EmployeeID, FamilyMemberName, MobileNumber, Relationship)
VALUES
    (@EmployeeID, @FamilyMemberName, @MobileNumber, @Relationship);";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@EmployeeID", member.EmployeeID);
                command.Parameters.AddWithValue("@FamilyMemberName", member.FamilyMemberName);
                command.Parameters.AddWithValue("@MobileNumber", ToParameter(member.MobileNumber));
                command.Parameters.AddWithValue("@Relationship", member.Relationship);

                command.ExecuteNonQuery();

                member.FamilyMemberID = (int)command.LastInsertedId;
            }
        }

        // Converts an absent value to the DBNull the driver expects.
        private static object ToParameter(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            return value;
        }

        // Returns every employee at one store, ordered the way a manager reads a register.
        public List<Employee> GetByStore(int storeID)
        {
            const string query = @"SELECT EmployeeID, StoreID, Name, Surname
FROM TBL_employees
WHERE StoreID = @StoreID
ORDER BY Surname, Name;";

            var employees = new List<Employee>();

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StoreID", storeID);

                    using (var reader = command.ExecuteReader())
                    {
                        int employeeIndex = reader.GetOrdinal("EmployeeID");
                        int storeIndex = reader.GetOrdinal("StoreID");
                        int nameIndex = reader.GetOrdinal("Name");
                        int surnameIndex = reader.GetOrdinal("Surname");

                        while (reader.Read())
                        {
                            var employee = new Employee();

                            employee.EmployeeID = reader.GetInt32(employeeIndex);
                            employee.StoreID = reader.GetInt32(storeIndex);

                            if (reader.IsDBNull(nameIndex))
                            {
                                employee.Name = string.Empty;
                            }
                            else
                            {
                                employee.Name = reader.GetString(nameIndex);
                            }

                            if (reader.IsDBNull(surnameIndex))
                            {
                                employee.Surname = string.Empty;
                            }
                            else
                            {
                                employee.Surname = reader.GetString(surnameIndex);
                            }

                            employees.Add(employee);
                        }

                        return employees;
                    }
                }
            }
        }
    }
}
