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
