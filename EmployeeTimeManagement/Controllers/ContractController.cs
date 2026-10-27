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
        // the employee does not exist or belongs to a store the user may not see.
        public ContractSource GetContractSource(int employeeID, int? storeID)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                ContractSource source = ReadEmployee(connection, employeeID, storeID);
                if (source == null)
                {
                    return null;
                }

                source.Address = ReadAddress(connection, employeeID);
                source.Spouse = ReadSpouse(connection, employeeID);
                source.Contract = ReadLatestContract(connection, employeeID);
                source.FamilyMembers.AddRange(ReadFamily(connection, employeeID));
                source.IsLinkedManager = IsActiveManager(connection, employeeID);

                return source;
            }
        }

        private static ContractSource ReadEmployee(MySqlConnection connection, int employeeID, int? storeID)
        {
            const string query = @"SELECT e.EmployeeID, e.StoreID, e.Name, e.Surname, e.IDNumber, e.MobileNumber, s.StoreName
FROM TBL_employees e
LEFT JOIN TBL_stores s ON s.StoreID = e.StoreID
WHERE e.EmployeeID = @EmployeeID
  AND (@StoreID IS NULL OR e.StoreID = @StoreID);";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@EmployeeID", employeeID);
                command.Parameters.AddWithValue("@StoreID", storeID.HasValue ? (object)storeID.Value : DBNull.Value);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    var source = new ContractSource
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

                    return source;
                }
            }
        }

        // The most recently captured address.
        private static EmployeeAddress ReadAddress(MySqlConnection connection, int employeeID)
        {
            const string query = @"SELECT AddressID, EmployeeID, HouseFlatNumber, ComplexFlatNumber, StreetName, Town, PostalCode
FROM TBL_employee_addresses
WHERE EmployeeID = @EmployeeID
ORDER BY AddressID DESC
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
                        HouseFlatNumber = ReadString(reader, "HouseFlatNumber"),
                        ComplexFlatNumber = ReadString(reader, "ComplexFlatNumber"),
                        StreetName = ReadString(reader, "StreetName"),
                        Town = ReadString(reader, "Town"),
                        PostalCode = ReadString(reader, "PostalCode")
                    };
                }
            }
        }

        // The most recently captured spouse.
        private static EmployeeSpouse ReadSpouse(MySqlConnection connection, int employeeID)
        {
            const string query = @"SELECT SpouseID, SpouseName, SpouseMobileNumber
FROM TBL_employee_spouses
WHERE EmployeeID = @EmployeeID
ORDER BY SpouseID DESC
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
                        SpouseName = ReadString(reader, "SpouseName"),
                        SpouseMobileNumber = ReadString(reader, "SpouseMobileNumber")
                    };
                }
            }
        }

        // The contract with the latest start date, which is the one in force.
        private static EmployeeContract ReadLatestContract(MySqlConnection connection, int employeeID)
        {
            const string query = @"SELECT ContractID, ContractType, StartDate, EndDate, Department, JobDescription, ReasonForEnding, HourlyRate
FROM TBL_employee_contracts
WHERE EmployeeID = @EmployeeID
ORDER BY StartDate DESC, ContractID DESC
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
                }
            }
        }

        // Family members in the order they were captured.
        private static List<EmployeeFamilyMember> ReadFamily(MySqlConnection connection, int employeeID)
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
                            FamilyMemberName = ReadString(reader, "FamilyMemberName"),
                            MobileNumber = ReadString(reader, "MobileNumber"),
                            Relationship = ReadString(reader, "Relationship")
                        });
                    }
                }
            }

            return members;
        }

        // True when an active manager login is linked to this employee.
        private static bool IsActiveManager(MySqlConnection connection, int employeeID)
        {
            const string query = @"SELECT COUNT(*)
FROM TBL_managers
WHERE EmployeeID = @EmployeeID AND IsActiveManager = 1;";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@EmployeeID", employeeID);
                return Convert.ToInt64(command.ExecuteScalar()) > 0;
            }
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
