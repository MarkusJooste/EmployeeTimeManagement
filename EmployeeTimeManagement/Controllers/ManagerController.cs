using System;
using System.Collections.Generic;
using EmployeeTimeManagement.Database;
using EmployeeTimeManagement.Models;
using MySqlConnector;

namespace EmployeeTimeManagement.Controllers
{
    internal class ManagerController
    {
        // Every row of TBL_managers at one store: Owners and Managers alike, active or not, so
        // ManagerAccess can decide who at that store may be promoted or demoted.
        public List<Manager> GetByStore(int storeID)
        {
            const string query = @"SELECT ManagerID, EmployeeID, ManagerName, StoreID, Password, IsActiveManager, IsAdmin, BusinessDate, CapturedBy
FROM TBL_managers
WHERE StoreID = @StoreID;";

            var managers = new List<Manager>();

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StoreID", storeID);

                    using (var reader = command.ExecuteReader())
                    {
                        int managerIdIndex = reader.GetOrdinal("ManagerID");
                        int employeeIdIndex = reader.GetOrdinal("EmployeeID");
                        int nameIndex = reader.GetOrdinal("ManagerName");
                        int storeIdIndex = reader.GetOrdinal("StoreID");
                        int pinIndex = reader.GetOrdinal("Password");
                        int activeIndex = reader.GetOrdinal("IsActiveManager");
                        int adminIndex = reader.GetOrdinal("IsAdmin");
                        int businessDateIndex = reader.GetOrdinal("BusinessDate");
                        int capturedByIndex = reader.GetOrdinal("CapturedBy");

                        while (reader.Read())
                        {
                            managers.Add(new Manager
                            {
                                ManagerID = reader.GetInt32(managerIdIndex),
                                EmployeeID = reader.IsDBNull(employeeIdIndex) ? (int?)null : reader.GetInt32(employeeIdIndex),
                                ManagerName = ReadText(reader, nameIndex),
                                StoreID = reader.IsDBNull(storeIdIndex) ? (int?)null : reader.GetInt32(storeIdIndex),
                                Pin = ReadText(reader, pinIndex),
                                IsActiveManager = reader.GetBoolean(activeIndex),
                                IsAdmin = reader.GetBoolean(adminIndex),
                                BusinessDate = reader.IsDBNull(businessDateIndex) ? (DateTime?)null : reader.GetDateTime(businessDateIndex),
                                CapturedBy = reader.IsDBNull(capturedByIndex) ? (int?)null : reader.GetInt32(capturedByIndex)
                            });
                        }
                    }
                }
            }

            return managers;
        }

        // Every row of TBL_managers across every store: PIN uniqueness spans every store, so
        // ManagerAccess needs every User, not just the promoting Owner's own.
        public List<Manager> GetAll()
        {
            const string query = @"SELECT ManagerID, EmployeeID, ManagerName, StoreID, Password, IsActiveManager, IsAdmin, BusinessDate, CapturedBy
FROM TBL_managers;";

            var managers = new List<Manager>();

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        int managerIdIndex = reader.GetOrdinal("ManagerID");
                        int employeeIdIndex = reader.GetOrdinal("EmployeeID");
                        int nameIndex = reader.GetOrdinal("ManagerName");
                        int storeIdIndex = reader.GetOrdinal("StoreID");
                        int pinIndex = reader.GetOrdinal("Password");
                        int activeIndex = reader.GetOrdinal("IsActiveManager");
                        int adminIndex = reader.GetOrdinal("IsAdmin");
                        int businessDateIndex = reader.GetOrdinal("BusinessDate");
                        int capturedByIndex = reader.GetOrdinal("CapturedBy");

                        while (reader.Read())
                        {
                            managers.Add(new Manager
                            {
                                ManagerID = reader.GetInt32(managerIdIndex),
                                EmployeeID = reader.IsDBNull(employeeIdIndex) ? (int?)null : reader.GetInt32(employeeIdIndex),
                                ManagerName = ReadText(reader, nameIndex),
                                StoreID = reader.IsDBNull(storeIdIndex) ? (int?)null : reader.GetInt32(storeIdIndex),
                                Pin = ReadText(reader, pinIndex),
                                IsActiveManager = reader.GetBoolean(activeIndex),
                                IsAdmin = reader.GetBoolean(adminIndex),
                                BusinessDate = reader.IsDBNull(businessDateIndex) ? (DateTime?)null : reader.GetDateTime(businessDateIndex),
                                CapturedBy = reader.IsDBNull(capturedByIndex) ? (int?)null : reader.GetInt32(capturedByIndex)
                            });
                        }
                    }
                }
            }

            return managers;
        }

        // Writes the row ManagerAccess.Promote produced: an insert for an Employee never
        // promoted before, or an update in place when a demoted row is being reconnected to,
        // so that row's ManagerID -- and everything already captured against it -- survives.
        public void Promote(Manager manager)
        {
            if (manager.ManagerID == 0)
            {
                Insert(manager);
            }
            else
            {
                Update(manager);
            }
        }

        private static void Insert(Manager manager)
        {
            const string query = @"INSERT INTO TBL_managers (EmployeeID, ManagerName, StoreID, Password, IsActiveManager, IsAdmin, BusinessDate, CapturedBy)
VALUES (@EmployeeID, @ManagerName, @StoreID, @Password, @IsActiveManager, @IsAdmin, @BusinessDate, @CapturedBy);";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ManagerName", string.Empty);
                    command.Parameters.AddWithValue("@EmployeeID", manager.EmployeeID);
                    command.Parameters.AddWithValue("@StoreID", manager.StoreID);
                    command.Parameters.AddWithValue("@Password", manager.Pin);
                    command.Parameters.AddWithValue("@IsActiveManager", manager.IsActiveManager);
                    command.Parameters.AddWithValue("@IsAdmin", manager.IsAdmin);
                    command.Parameters.AddWithValue("@BusinessDate", manager.BusinessDate);
                    command.Parameters.AddWithValue("@CapturedBy", manager.CapturedBy);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Reconnects a demoted row: everything Promote decided is rewritten, but the row's own
        // ManagerName -- never set by this app -- is left as it was.
        private static void Update(Manager manager)
        {
            const string query = @"UPDATE TBL_managers
SET EmployeeID = @EmployeeID, StoreID = @StoreID, Password = @Password, IsActiveManager = @IsActiveManager, IsAdmin = @IsAdmin, BusinessDate = @BusinessDate, CapturedBy = @CapturedBy
WHERE ManagerID = @ManagerID;";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ManagerID", manager.ManagerID);
                    command.Parameters.AddWithValue("@EmployeeID", manager.EmployeeID);
                    command.Parameters.AddWithValue("@StoreID", manager.StoreID);
                    command.Parameters.AddWithValue("@Password", manager.Pin);
                    command.Parameters.AddWithValue("@IsActiveManager", manager.IsActiveManager);
                    command.Parameters.AddWithValue("@IsAdmin", manager.IsAdmin);
                    command.Parameters.AddWithValue("@BusinessDate", manager.BusinessDate);
                    command.Parameters.AddWithValue("@CapturedBy", manager.CapturedBy);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Withdraws a login. ManagerAccess.Demote already copies EmployeeID, StoreID and Pin
        // across unchanged, so writing through the same Update the reconnect path in Promote
        // uses has no effect on them -- only IsActiveManager, BusinessDate and CapturedBy
        // actually move.
        public void Demote(Manager manager)
        {
            Update(manager);
        }

        private static string ReadText(MySqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
            {
                return string.Empty;
            }

            return reader.GetString(index);
        }
    }
}
