using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace EmployeeTimeManagement.Database
{
    public static class DatabaseConnection
    {
        private static bool _envLoaded;

        public static MySqlConnection GetConnection()
        {
            if (!_envLoaded)
            {
                string envPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
                DotNetEnv.Env.Load(envPath);
                _envLoaded = true;
            }

            string connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "MYSQL_CONNECTION_STRING is not set. Add it to a .env file next to the application.");
            }

            var connection = new MySqlConnection(connectionString);

            connection.Open();
            return connection;
        }
    }
}
