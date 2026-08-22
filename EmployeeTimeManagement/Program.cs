using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmployeeTimeManagement.Database;

namespace EmployeeTimeManagement
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    MessageBox.Show(
                        "Connected. Server version: " + connection.ServerVersion,
                        "Database connection test");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database connection test failed");
            }
        }
    }
}
