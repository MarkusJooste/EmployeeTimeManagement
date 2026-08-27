using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTimeManagement.Database;
using EmployeeTimeManagement.Models;
using MySqlConnector;

namespace EmployeeTimeManagement.Controllers
{
    internal class LoginController
    {
        public Manager Login(string pin)
        {
            const string query = @"SELECT ManagerID, ManagerName, StoreID, IsAdmin FROM TBL_managers WHERE Password = @Password;";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Password", pin);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        var manager = new Manager();

                        // ManagerID
                        manager.ManagerID = reader.GetInt32(reader.GetOrdinal("ManagerID"));

                        // ManagerName
                        int nameIndex = reader.GetOrdinal("ManagerName");
                        if (reader.IsDBNull(nameIndex))
                        {
                            manager.ManagerName = string.Empty;
                        }
                        else
                        {
                            manager.ManagerName = reader.GetString(nameIndex);
                        }

                        // StoreID
                        int storeIndex = reader.GetOrdinal("StoreID");
                        if (reader.IsDBNull(storeIndex))
                        {
                            manager.StoreID = null;
                        }
                        else
                        {
                            manager.StoreID = reader.GetInt32(storeIndex);
                        }

                        // IsAdmin
                        manager.IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"));
                        return manager;
                    }
                }
            }
        }
    }

}
