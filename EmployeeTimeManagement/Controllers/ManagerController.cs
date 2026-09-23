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
