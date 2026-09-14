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
       c.StartDate,
       c.EndDate,
       c.OpeningPTODays,
       c.OpeningBalanceAsAt
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
                        int startDateIndex = reader.GetOrdinal("StartDate");
                        int endDateIndex = reader.GetOrdinal("EndDate");
                        int openingPTODaysIndex = reader.GetOrdinal("OpeningPTODays");
                        int openingBalanceAsAtIndex = reader.GetOrdinal("OpeningBalanceAsAt");

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

                            item.ContractStartDate = reader.IsDBNull(startDateIndex)
                                ? (DateTime?)null
                                : reader.GetDateTime(startDateIndex);

                            // An employee with no contract row has no end date, which reads as still employed.
                            if (reader.IsDBNull(endDateIndex))
                            {
                                item.ContractEndDate = null;
                            }
                            else
                            {
                                item.ContractEndDate = reader.GetDateTime(endDateIndex);
                            }

                            // An employee with no contract row has no Opening Balance to read, which reads as zero.
                            item.OpeningPTODays = reader.IsDBNull(openingPTODaysIndex) ? 0 : reader.GetInt32(openingPTODaysIndex);

                            item.OpeningBalanceAsAt = reader.IsDBNull(openingBalanceAsAtIndex)
                                ? (DateTime?)null
                                : reader.GetDateTime(openingBalanceAsAtIndex);

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
            const string query = @"SELECT EmployeeID, StoreID, Name, Surname
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

                        owner.EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID"));
                        owner.StoreID = reader.GetInt32(reader.GetOrdinal("StoreID"));
                        owner.Name = ReadText(reader, reader.GetOrdinal("Name"));
                        owner.Surname = ReadText(reader, reader.GetOrdinal("Surname"));

                        return owner;
                    }
                }
            }
        }

        // Reads everything on record for one employee, across all six tables, so the editor
        // can be prefilled for an update. A table an old employee predates comes back null
        // rather than failing, so a record captured before some detail was known can still be opened.
        public EmployeeRecord GetForEdit(int employeeID)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                Employee employee = ReadEmployee(connection, employeeID);

                if (employee == null)
                {
                    return null;
                }

                return new EmployeeRecord
                {
                    Employee = employee,
                    Address = ReadAddress(connection, employeeID),
                    Bank = ReadBank(connection, employeeID),
                    Contract = ReadContract(connection, employeeID),
                    Spouse = ReadSpouse(connection, employeeID),
                    FamilyMembers = ReadFamilyMembers(connection, employeeID)
                };
            }
        }

        // Reads the TBL_employees row an update starts from, or null if it no longer exists.
        private static Employee ReadEmployee(MySqlConnection connection, int employeeID)
        {
            const string query = @"SELECT EmployeeID, StoreID, Name, Surname, IDNumber, SARSNumber, MobileNumber,
       MaritialStatus, NumberOfDependents, BusinessDate, CapturedBy
FROM TBL_employees
WHERE EmployeeID = @EmployeeID
LIMIT 1;";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@EmployeeID", employeeID);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new Employee
                    {
                        EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                        StoreID = reader.GetInt32(reader.GetOrdinal("StoreID")),
                        Name = ReadText(reader, reader.GetOrdinal("Name")),
                        Surname = ReadText(reader, reader.GetOrdinal("Surname")),
                        IDNumber = ReadText(reader, reader.GetOrdinal("IDNumber")),
                        SARSNumber = ReadNullableText(reader, reader.GetOrdinal("SARSNumber")),
                        MobileNumber = ReadText(reader, reader.GetOrdinal("MobileNumber")),
                        MaritalStatus = ReadText(reader, reader.GetOrdinal("MaritialStatus")),
                        NumberOfDependents = reader.GetInt32(reader.GetOrdinal("NumberOfDependents")),
                        BusinessDate = reader.GetDateTime(reader.GetOrdinal("BusinessDate")),
                        CapturedBy = reader.GetInt32(reader.GetOrdinal("CapturedBy"))
                    };
                }
            }
        }

        // Reads the employee's address, or null when nothing has been captured yet.
        private static EmployeeAddress ReadAddress(MySqlConnection connection, int employeeID)
        {
            const string query = @"SELECT AddressID, HouseFlatNumber, ComplexFlatNumber, StreetName, Town, PostalCode
FROM TBL_employee_addresses
WHERE EmployeeID = @EmployeeID
LIMIT 1;";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@EmployeeID", employeeID);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new EmployeeAddress
                    {
                        AddressID = reader.GetInt32(reader.GetOrdinal("AddressID")),
                        EmployeeID = employeeID,
                        HouseFlatNumber = ReadText(reader, reader.GetOrdinal("HouseFlatNumber")),
                        ComplexFlatNumber = ReadText(reader, reader.GetOrdinal("ComplexFlatNumber")),
                        StreetName = ReadText(reader, reader.GetOrdinal("StreetName")),
                        Town = ReadText(reader, reader.GetOrdinal("Town")),
                        PostalCode = ReadText(reader, reader.GetOrdinal("PostalCode"))
                    };
                }
            }
        }

        // Reads the employee's bank details, or null when nothing has been captured yet.
        private static EmployeeBank ReadBank(MySqlConnection connection, int employeeID)
        {
            const string query = @"SELECT BankID, BankName, AccountType, AccountNumber, BranchCode
FROM TBL_employee_bank
WHERE EmployeeID = @EmployeeID
LIMIT 1;";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@EmployeeID", employeeID);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new EmployeeBank
                    {
                        BankID = reader.GetInt32(reader.GetOrdinal("BankID")),
                        EmployeeID = employeeID,
                        BankName = ReadText(reader, reader.GetOrdinal("BankName")),
                        AccountType = ReadText(reader, reader.GetOrdinal("AccountType")),
                        AccountNumber = ReadText(reader, reader.GetOrdinal("AccountNumber")),
                        BranchCode = ReadText(reader, reader.GetOrdinal("BranchCode"))
                    };
                }
            }
        }

        // Reads the employee's live contract, or null when nothing has been captured yet. The
        // schema permits several contracts per employee even though this application writes
        // one, so only the latest is read, matching GetListByStore.
        private static EmployeeContract ReadContract(MySqlConnection connection, int employeeID)
        {
            const string query = @"SELECT ContractID, ContractType, StartDate, EndDate, Department, JobDescription, ReasonForEnding, HourlyRate
FROM TBL_employee_contracts
WHERE EmployeeID = @EmployeeID
ORDER BY ContractID DESC
LIMIT 1;";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@EmployeeID", employeeID);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    int endDateIndex = reader.GetOrdinal("EndDate");

                    return new EmployeeContract
                    {
                        ContractID = reader.GetInt32(reader.GetOrdinal("ContractID")),
                        EmployeeID = employeeID,
                        ContractType = ReadText(reader, reader.GetOrdinal("ContractType")),
                        StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                        EndDate = reader.IsDBNull(endDateIndex) ? (DateTime?)null : reader.GetDateTime(endDateIndex),
                        Department = ReadText(reader, reader.GetOrdinal("Department")),
                        JobDescription = ReadText(reader, reader.GetOrdinal("JobDescription")),
                        ReasonForEnding = ReadText(reader, reader.GetOrdinal("ReasonForEnding")),
                        HourlyRate = reader.GetDecimal(reader.GetOrdinal("HourlyRate"))
                    };
                }
            }
        }

        // Reads the employee's spouse, or null when they have none on record. Read regardless
        // of the employee's current marital status, so a spouse row survives a manager
        // correcting marital status away from Married and back without losing it.
        private static EmployeeSpouse ReadSpouse(MySqlConnection connection, int employeeID)
        {
            const string query = @"SELECT SpouseID, SpouseName, SpouseMobileNumber
FROM TBL_employee_spouses
WHERE EmployeeID = @EmployeeID
LIMIT 1;";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@EmployeeID", employeeID);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new EmployeeSpouse
                    {
                        SpouseID = reader.GetInt32(reader.GetOrdinal("SpouseID")),
                        EmployeeID = employeeID,
                        SpouseName = ReadText(reader, reader.GetOrdinal("SpouseName")),
                        SpouseMobileNumber = ReadText(reader, reader.GetOrdinal("SpouseMobileNumber"))
                    };
                }
            }
        }

        // Reads every family contact on record for the employee.
        private static List<EmployeeFamilyMember> ReadFamilyMembers(MySqlConnection connection, int employeeID)
        {
            const string query = @"SELECT FamilyMemberID, FamilyMemberName, MobileNumber, Relationship
FROM TBL_employee_family
WHERE EmployeeID = @EmployeeID
ORDER BY FamilyMemberID;";

            var members = new List<EmployeeFamilyMember>();

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@EmployeeID", employeeID);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        members.Add(new EmployeeFamilyMember
                        {
                            FamilyMemberID = reader.GetInt32(reader.GetOrdinal("FamilyMemberID")),
                            EmployeeID = employeeID,
                            FamilyMemberName = ReadText(reader, reader.GetOrdinal("FamilyMemberName")),
                            MobileNumber = ReadNullableText(reader, reader.GetOrdinal("MobileNumber")),
                            Relationship = ReadText(reader, reader.GetOrdinal("Relationship"))
                        });
                    }
                }
            }

            return members;
        }

        // Reads a nullable text column as null, unlike ReadText, for fields whose absence
        // must round-trip back into the editor as genuinely empty rather than blank text.
        private static string ReadNullableText(MySqlDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? null : reader.GetString(index);
        }

        // Writes every change to an existing employee as one transaction: the employee row is
        // always updated, each child table is updated in place where a row exists and inserted
        // where it does not, and a family contact removed by the manager is deleted - the only
        // deletion this feature performs. End date and reason for ending are left untouched,
        // since they are off this form entirely.
        public void Update(EmployeeRecord record)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        UpdateEmployee(connection, transaction, record.Employee);

                        UpsertAddress(connection, transaction, record.Address);
                        UpsertBank(connection, transaction, record.Bank);
                        UpsertContract(connection, transaction, record.Contract);

                        // A spouse row is only ever written for a currently married employee;
                        // one belonging to somebody no longer married is left exactly as it was.
                        if (record.Spouse != null)
                        {
                            UpsertSpouse(connection, transaction, record.Spouse);
                        }

                        UpsertFamilyMembers(connection, transaction, record.Employee.EmployeeID, record.FamilyMembers);

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

        // Updates the TBL_employees row in place. StoreID and BusinessDate are left alone:
        // this screen has no way to move an employee between stores, and BusinessDate records
        // when the employee was originally captured, not when they were last edited. Only
        // CapturedBy is overwritten, so the row records the manager who made this change.
        private static void UpdateEmployee(MySqlConnection connection, MySqlTransaction transaction, Employee employee)
        {
            const string query = @"UPDATE TBL_employees SET
    Name = @Name, Surname = @Surname, IDNumber = @IDNumber, SARSNumber = @SARSNumber,
    MobileNumber = @MobileNumber, MaritialStatus = @MaritalStatus,
    NumberOfDependents = @NumberOfDependents, CapturedBy = @CapturedBy
WHERE EmployeeID = @EmployeeID;";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                command.Parameters.AddWithValue("@Name", employee.Name);
                command.Parameters.AddWithValue("@Surname", employee.Surname);
                command.Parameters.AddWithValue("@IDNumber", employee.IDNumber);
                command.Parameters.AddWithValue("@SARSNumber", ToParameter(employee.SARSNumber));
                command.Parameters.AddWithValue("@MobileNumber", employee.MobileNumber);
                command.Parameters.AddWithValue("@MaritalStatus", employee.MaritalStatus);
                command.Parameters.AddWithValue("@NumberOfDependents", employee.NumberOfDependents);
                command.Parameters.AddWithValue("@CapturedBy", employee.CapturedBy);

                command.ExecuteNonQuery();
            }
        }

        // Updates the address in place if one is on record, or inserts one if this is the
        // first time it has been captured.
        private static void UpsertAddress(MySqlConnection connection, MySqlTransaction transaction, EmployeeAddress address)
        {
            if (!address.AddressID.HasValue)
            {
                InsertAddress(connection, transaction, address);
                return;
            }

            const string query = @"UPDATE TBL_employee_addresses SET
    HouseFlatNumber = @HouseFlatNumber, ComplexFlatNumber = @ComplexFlatNumber,
    StreetName = @StreetName, Town = @Town, PostalCode = @PostalCode
WHERE AddressID = @AddressID;";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@AddressID", address.AddressID.Value);
                command.Parameters.AddWithValue("@HouseFlatNumber", address.HouseFlatNumber);
                command.Parameters.AddWithValue("@ComplexFlatNumber", address.ComplexFlatNumber);
                command.Parameters.AddWithValue("@StreetName", address.StreetName);
                command.Parameters.AddWithValue("@Town", address.Town);
                command.Parameters.AddWithValue("@PostalCode", address.PostalCode);

                command.ExecuteNonQuery();
            }
        }

        // Updates the bank details in place if any are on record, or inserts them if this is
        // the first time they have been captured.
        private static void UpsertBank(MySqlConnection connection, MySqlTransaction transaction, EmployeeBank bank)
        {
            if (!bank.BankID.HasValue)
            {
                InsertBank(connection, transaction, bank);
                return;
            }

            const string query = @"UPDATE TBL_employee_bank SET
    BankName = @BankName, AccountType = @AccountType, AccountNumber = @AccountNumber, BranchCode = @BranchCode
WHERE BankID = @BankID;";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@BankID", bank.BankID.Value);
                command.Parameters.AddWithValue("@BankName", bank.BankName);
                command.Parameters.AddWithValue("@AccountType", bank.AccountType);
                command.Parameters.AddWithValue("@AccountNumber", bank.AccountNumber);
                command.Parameters.AddWithValue("@BranchCode", bank.BranchCode);

                command.ExecuteNonQuery();
            }
        }

        // Updates the contract in place if one is on record, or inserts one if this is the
        // first time it has been captured. EndDate and ReasonForEnding are deliberately left
        // out of the SET list: they are off this form, and Terminate is what owns them.
        private static void UpsertContract(MySqlConnection connection, MySqlTransaction transaction, EmployeeContract contract)
        {
            if (!contract.ContractID.HasValue)
            {
                InsertContract(connection, transaction, contract);
                return;
            }

            const string query = @"UPDATE TBL_employee_contracts SET
    ContractType = @ContractType, StartDate = @StartDate, Department = @Department,
    JobDescription = @JobDescription, HourlyRate = @HourlyRate
WHERE ContractID = @ContractID;";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@ContractID", contract.ContractID.Value);
                command.Parameters.AddWithValue("@ContractType", contract.ContractType);
                command.Parameters.AddWithValue("@StartDate", contract.StartDate.Date);
                command.Parameters.AddWithValue("@Department", contract.Department);
                command.Parameters.AddWithValue("@JobDescription", contract.JobDescription);
                command.Parameters.AddWithValue("@HourlyRate", contract.HourlyRate);

                command.ExecuteNonQuery();
            }
        }

        // Updates the spouse in place if one is on record, or inserts one if this is the
        // first time a spouse has been captured for this employee.
        private static void UpsertSpouse(MySqlConnection connection, MySqlTransaction transaction, EmployeeSpouse spouse)
        {
            if (!spouse.SpouseID.HasValue)
            {
                InsertSpouse(connection, transaction, spouse);
                return;
            }

            const string query = @"UPDATE TBL_employee_spouses SET
    SpouseName = @SpouseName, SpouseMobileNumber = @SpouseMobileNumber
WHERE SpouseID = @SpouseID;";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@SpouseID", spouse.SpouseID.Value);
                command.Parameters.AddWithValue("@SpouseName", spouse.SpouseName);
                command.Parameters.AddWithValue("@SpouseMobileNumber", spouse.SpouseMobileNumber);

                command.ExecuteNonQuery();
            }
        }

        // Updates or inserts every family contact the manager kept, then deletes whichever
        // stored contacts are no longer among them - the only deletion this feature performs.
        private static void UpsertFamilyMembers(MySqlConnection connection, MySqlTransaction transaction, int employeeID, List<EmployeeFamilyMember> members)
        {
            var keptIDs = new List<int>();

            foreach (EmployeeFamilyMember member in members)
            {
                member.EmployeeID = employeeID;

                if (member.FamilyMemberID.HasValue)
                {
                    UpdateFamilyMember(connection, transaction, member);
                }
                else
                {
                    InsertFamilyMember(connection, transaction, member);
                }

                keptIDs.Add(member.FamilyMemberID.Value);
            }

            DeleteRemovedFamilyMembers(connection, transaction, employeeID, keptIDs);
        }

        // Updates one family contact already on record.
        private static void UpdateFamilyMember(MySqlConnection connection, MySqlTransaction transaction, EmployeeFamilyMember member)
        {
            const string query = @"UPDATE TBL_employee_family SET
    FamilyMemberName = @FamilyMemberName, MobileNumber = @MobileNumber, Relationship = @Relationship
WHERE FamilyMemberID = @FamilyMemberID;";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@FamilyMemberID", member.FamilyMemberID.Value);
                command.Parameters.AddWithValue("@FamilyMemberName", member.FamilyMemberName);
                command.Parameters.AddWithValue("@MobileNumber", ToParameter(member.MobileNumber));
                command.Parameters.AddWithValue("@Relationship", member.Relationship);

                command.ExecuteNonQuery();
            }
        }

        // Deletes whichever stored family contacts the manager did not keep - a manager
        // genuinely removing somebody who has moved away, the one deletion this feature performs.
        private static void DeleteRemovedFamilyMembers(MySqlConnection connection, MySqlTransaction transaction, int employeeID, List<int> keptIDs)
        {
            const string selectQuery = "SELECT FamilyMemberID FROM TBL_employee_family WHERE EmployeeID = @EmployeeID;";

            var storedIDs = new List<int>();

            using (var command = new MySqlCommand(selectQuery, connection, transaction))
            {
                command.Parameters.AddWithValue("@EmployeeID", employeeID);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        storedIDs.Add(reader.GetInt32(0));
                    }
                }
            }

            const string deleteQuery = "DELETE FROM TBL_employee_family WHERE FamilyMemberID = @FamilyMemberID;";

            foreach (int storedID in storedIDs)
            {
                if (keptIDs.Contains(storedID))
                {
                    continue;
                }

                using (var command = new MySqlCommand(deleteQuery, connection, transaction))
                {
                    command.Parameters.AddWithValue("@FamilyMemberID", storedID);
                    command.ExecuteNonQuery();
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

        // Ends one employee's contract: writes the end date and reason for leaving, and
        // touches no other table. Updates only the latest contract, matching ReadContract.
        // Returns whether a contract row existed to close: a no-op, not an error, when it
        // did not, so the caller can tell the manager nothing was actually there to end.
        public bool Terminate(int employeeID, DateTime endDate, string reason)
        {
            const string query = @"UPDATE TBL_employee_contracts
SET EndDate = @EndDate, ReasonForEnding = @ReasonForEnding
WHERE EmployeeID = @EmployeeID
ORDER BY ContractID DESC
LIMIT 1;";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);
                    command.Parameters.AddWithValue("@EndDate", endDate.Date);
                    command.Parameters.AddWithValue("@ReasonForEnding", reason ?? string.Empty);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Reverses Terminate: clears the end date and reason for leaving back to how a live
        // contract reads. Covers both a mistaken termination and a genuine rehire.
        public void Reactivate(int employeeID)
        {
            const string query = @"UPDATE TBL_employee_contracts
SET EndDate = NULL, ReasonForEnding = ''
WHERE EmployeeID = @EmployeeID
ORDER BY ContractID DESC
LIMIT 1;";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Returns every employee at one store, ordered the way a manager reads a register,
        // each carrying their latest contract's dates so the caller can date-scope who was
        // actually employed on a given work date. The schema permits several contracts per
        // employee even though this application writes one, so only the latest is joined,
        // matching GetListByStore.
        public List<Employee> GetByStore(int storeID)
        {
            const string query = @"SELECT e.EmployeeID,
       e.StoreID,
       e.Name,
       e.Surname,
       c.StartDate,
       c.EndDate
FROM TBL_employees e
LEFT JOIN TBL_employee_contracts c
       ON c.EmployeeID = e.EmployeeID
      AND c.ContractID = (SELECT MAX(latest.ContractID)
                          FROM TBL_employee_contracts latest
                          WHERE latest.EmployeeID = e.EmployeeID)
WHERE e.StoreID = @StoreID
ORDER BY e.Surname, e.Name;";

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
                        int startDateIndex = reader.GetOrdinal("StartDate");
                        int endDateIndex = reader.GetOrdinal("EndDate");

                        while (reader.Read())
                        {
                            var employee = new Employee();

                            employee.EmployeeID = reader.GetInt32(employeeIndex);
                            employee.StoreID = reader.GetInt32(storeIndex);
                            employee.Name = ReadText(reader, nameIndex);
                            employee.Surname = ReadText(reader, surnameIndex);

                            employee.ContractStartDate = reader.IsDBNull(startDateIndex)
                                ? (DateTime?)null
                                : reader.GetDateTime(startDateIndex);

                            employee.ContractEndDate = reader.IsDBNull(endDateIndex)
                                ? (DateTime?)null
                                : reader.GetDateTime(endDateIndex);

                            employees.Add(employee);
                        }

                        return employees;
                    }
                }
            }
        }
    }
}
