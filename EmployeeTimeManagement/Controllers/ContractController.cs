using System;
using System.Collections.Generic;
using EmployeeTimeManagement.Database;
using EmployeeTimeManagement.Models;
using MySqlConnector;

namespace EmployeeTimeManagement.Controllers
{
    // One row of the Contracts screen's employee list.
    internal class ContractListItem
    {
        public int EmployeeID { get; set; }
        public int StoreID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string StoreName { get; set; }
        public string Position { get; set; }
        public DateTime? StartDate { get; set; }
    }

    // Reads what an employment contract needs. Read-only: nothing here writes to the
    // database. Every query takes the store the user may see, so a manager can only
    // ever load employees of their own store; null means every store (the owner).
    internal class ContractController
    {
        // Each employee with the position and start date of their latest contract.
        public List<ContractListItem> GetEmployees(int? storeID)
        {
            const string query = @"SELECT e.EmployeeID, e.StoreID, e.Name, e.Surname, s.StoreName,
       c.JobDescription, c.StartDate
FROM TBL_employees e
LEFT JOIN TBL_stores s ON s.StoreID = e.StoreID
LEFT JOIN TBL_employee_contracts c ON c.ContractID = (
    SELECT c2.ContractID
    FROM TBL_employee_contracts c2
    WHERE c2.EmployeeID = e.EmployeeID
    ORDER BY c2.StartDate DESC, c2.ContractID DESC
    LIMIT 1)
WHERE (@StoreID IS NULL OR e.StoreID = @StoreID)
ORDER BY e.Surname, e.Name;";

            var items = new List<ContractListItem>();

            using (var connection = DatabaseConnection.GetConnection())
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StoreID", storeID.HasValue ? (object)storeID.Value : DBNull.Value);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new ContractListItem
                        {
                            EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                            StoreID = reader.GetInt32(reader.GetOrdinal("StoreID")),
                            Name = ReadString(reader, "Name"),
                            Surname = ReadString(reader, "Surname"),
                            StoreName = ReadString(reader, "StoreName"),
                            Position = ReadString(reader, "JobDescription"),
                            StartDate = ReadDate(reader, "StartDate")
                        });
                    }
                }
            }

            return items;
        }

        // Everything stored about one employee that the contract can use, or null when
        // the employee does not exist or belongs to a store the user may not see. The six
        // reads go to the database in one batch, since each trip to the server is slow.
        public ContractSource GetContractSource(int employeeID, int? storeID)
        {
            Query<ContractSource> employee = EmployeeQuery(employeeID, storeID);
            Query<EmployeeAddress> address = AddressQuery(employeeID, storeID);
            Query<EmployeeSpouse> spouse = SpouseQuery(employeeID, storeID);
            Query<EmployeeContract> contract = LatestContractQuery(employeeID, storeID);
            Query<List<EmployeeFamilyMember>> family = FamilyQuery(employeeID, storeID);
            Query<bool> isManager = ActiveManagerQuery(employeeID, storeID);

            QueryBatch.Run(employee, address, spouse, contract, family, isManager);

            ContractSource source = employee.Result;
            if (source == null)
            {
                return null;
            }

            source.Address = address.Result;
            source.Spouse = spouse.Result;
            source.Contract = contract.Result;
            source.FamilyMembers.AddRange(family.Result);
            source.IsLinkedManager = isManager.Result;

            return source;
        }

        // Added to the queries below, which are batched with the employee's own read rather
        // than run after it: each one still only returns rows for an employee in a store the
        // user may see.
        private const string InVisibleStore = @"EXISTS (SELECT 1
              FROM TBL_employees visible
              WHERE visible.EmployeeID = @EmployeeID
                AND (@StoreID IS NULL OR visible.StoreID = @StoreID))";

        private static Query<ContractSource> EmployeeQuery(int employeeID, int? storeID)
        {
            const string query = @"SELECT e.EmployeeID, e.StoreID, e.Name, e.Surname, e.IDNumber, e.MobileNumber, s.StoreName
FROM TBL_employees e
LEFT JOIN TBL_stores s ON s.StoreID = e.StoreID
WHERE e.EmployeeID = @EmployeeID
  AND (@StoreID IS NULL OR e.StoreID = @StoreID);";

            return new Query<ContractSource>(query, reader =>
            {
                if (!reader.Read())
                {
                    return null;
                }

                return new ContractSource
                {
                    Employee = new Employee
                    {
                        EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                        StoreID = reader.GetInt32(reader.GetOrdinal("StoreID")),
                        Name = ReadString(reader, "Name") ?? string.Empty,
                        Surname = ReadString(reader, "Surname") ?? string.Empty,
                        IDNumber = ReadString(reader, "IDNumber"),
                        MobileNumber = ReadString(reader, "MobileNumber")
                    },
                    StoreName = ReadString(reader, "StoreName")
                };
            }).With("@EmployeeID", employeeID).With("@StoreID", storeID);
        }

        // The most recently captured address.
        private static Query<EmployeeAddress> AddressQuery(int employeeID, int? storeID)
        {
            const string query = @"SELECT AddressID, EmployeeID, HouseFlatNumber, ComplexFlatNumber, StreetName, Town, PostalCode
FROM TBL_employee_addresses
WHERE EmployeeID = @EmployeeID AND " + InVisibleStore + @"
ORDER BY AddressID DESC
LIMIT 1;";

            return new Query<EmployeeAddress>(query, reader =>
            {
                if (!reader.Read())
                {
                    return null;
                }

                return new EmployeeAddress
                {
                    AddressID = reader.GetInt32(reader.GetOrdinal("AddressID")),
                    EmployeeID = employeeID,
                    HouseFlatNumber = ReadString(reader, "HouseFlatNumber"),
                    ComplexFlatNumber = ReadString(reader, "ComplexFlatNumber"),
                    StreetName = ReadString(reader, "StreetName"),
                    Town = ReadString(reader, "Town"),
                    PostalCode = ReadString(reader, "PostalCode")
                };
            }).With("@EmployeeID", employeeID).With("@StoreID", storeID);
        }

        // The most recently captured spouse.
        private static Query<EmployeeSpouse> SpouseQuery(int employeeID, int? storeID)
        {
            const string query = @"SELECT SpouseID, SpouseName, SpouseMobileNumber
FROM TBL_employee_spouses
WHERE EmployeeID = @EmployeeID AND " + InVisibleStore + @"
ORDER BY SpouseID DESC
LIMIT 1;";

            return new Query<EmployeeSpouse>(query, reader =>
            {
                if (!reader.Read())
                {
                    return null;
                }

                return new EmployeeSpouse
                {
                    SpouseID = reader.GetInt32(reader.GetOrdinal("SpouseID")),
                    EmployeeID = employeeID,
                    SpouseName = ReadString(reader, "SpouseName"),
                    SpouseMobileNumber = ReadString(reader, "SpouseMobileNumber")
                };
            }).With("@EmployeeID", employeeID).With("@StoreID", storeID);
        }

        // The contract with the latest start date, which is the one in force.
        private static Query<EmployeeContract> LatestContractQuery(int employeeID, int? storeID)
        {
            const string query = @"SELECT ContractID, ContractType, StartDate, EndDate, Department, JobDescription, ReasonForEnding, HourlyRate
FROM TBL_employee_contracts
WHERE EmployeeID = @EmployeeID AND " + InVisibleStore + @"
ORDER BY StartDate DESC, ContractID DESC
LIMIT 1;";

            return new Query<EmployeeContract>(query, reader =>
            {
                if (!reader.Read())
                {
                    return null;
                }

                return new EmployeeContract
                {
                    ContractID = reader.GetInt32(reader.GetOrdinal("ContractID")),
                    EmployeeID = employeeID,
                    ContractType = ReadString(reader, "ContractType"),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = ReadDate(reader, "EndDate"),
                    Department = ReadString(reader, "Department"),
                    JobDescription = ReadString(reader, "JobDescription"),
                    ReasonForEnding = ReadString(reader, "ReasonForEnding"),
                    HourlyRate = reader.GetDecimal(reader.GetOrdinal("HourlyRate"))
                };
            }).With("@EmployeeID", employeeID).With("@StoreID", storeID);
        }

        // Family members in the order they were captured.
        private static Query<List<EmployeeFamilyMember>> FamilyQuery(int employeeID, int? storeID)
        {
            const string query = @"SELECT FamilyMemberID, FamilyMemberName, MobileNumber, Relationship
FROM TBL_employee_family
WHERE EmployeeID = @EmployeeID AND " + InVisibleStore + @"
ORDER BY FamilyMemberID;";

            return new Query<List<EmployeeFamilyMember>>(query, reader =>
            {
                var members = new List<EmployeeFamilyMember>();

                while (reader.Read())
                {
                    members.Add(new EmployeeFamilyMember
                    {
                        FamilyMemberID = reader.GetInt32(reader.GetOrdinal("FamilyMemberID")),
                        EmployeeID = employeeID,
                        FamilyMemberName = ReadString(reader, "FamilyMemberName"),
                        MobileNumber = ReadString(reader, "MobileNumber"),
                        Relationship = ReadString(reader, "Relationship")
                    });
                }

                return members;
            }).With("@EmployeeID", employeeID).With("@StoreID", storeID);
        }

        // True when an active manager login is linked to this employee.
        private static Query<bool> ActiveManagerQuery(int employeeID, int? storeID)
        {
            const string query = @"SELECT COUNT(*)
FROM TBL_managers
WHERE EmployeeID = @EmployeeID AND IsActiveManager = 1 AND " + InVisibleStore + ";";

            return new Query<bool>(query, reader => QueryBatch.ReadInt(reader) > 0)
                .With("@EmployeeID", employeeID)
                .With("@StoreID", storeID);
        }

        private static string ReadString(MySqlDataReader reader, string column)
        {
            int index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? null : reader.GetString(index);
        }

        private static DateTime? ReadDate(MySqlDataReader reader, string column)
        {
            int index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? (DateTime?)null : reader.GetDateTime(index);
        }
    }
}
