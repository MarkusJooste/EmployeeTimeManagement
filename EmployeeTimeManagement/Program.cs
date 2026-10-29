using EmployeeTimeManagement.Database;
using EmployeeTimeManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            // A new PC is set up once with the database connection, saved encrypted.
            if (!DatabaseConnection.IsConfigured)
            {
                using (var setup = new ConnectionSetupForm())
                {
                    if (setup.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                }
            }

            if (DatabaseConnection.ImportWarning != null)
            {
                MessageBox.Show(DatabaseConnection.ImportWarning, "Database connection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Application.Run(new LoginForm());
        }
    }
}
