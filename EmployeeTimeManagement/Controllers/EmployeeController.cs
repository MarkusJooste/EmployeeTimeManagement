using System;
using System.Collections.Generic;
using EmployeeTimeManagement.Database;
using EmployeeTimeManagement.Models;
using MySqlConnector;

namespace EmployeeTimeManagement.Controllers
{
    internal class EmployeeController
    {
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
