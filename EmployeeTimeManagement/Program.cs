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

            // Test the saved connection (importing any .env first) and let the user retry,
            // set it up, or carry on without it.
            StartupResult startup = DatabaseConnection.RunStartupFlow(
                (problem, detail) =>
                {
                    using (var choice = new StartupChoiceForm(problem, detail))
                    {
                        choice.ShowDialog();
                        return choice.Choice;
                    }
                },
                () =>
                {
                    using (var setup = new ConnectionSetupForm())
                    {
                        return setup.ShowDialog() == DialogResult.OK;
                    }
                });

            if (startup.ImportWarning != null)
            {
                MessageBox.Show(startup.ImportWarning, "Database connection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Application.Run(new LoginForm(startup.Connected ? null : startup.Reason));
        }
    }
}
