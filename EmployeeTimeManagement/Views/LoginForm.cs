using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmployeeTimeManagement.Controllers;
using EmployeeTimeManagement.Models;

namespace EmployeeTimeManagement.Views
{
    public partial class LoginForm : Form
    {
        private readonly LoginController loginController;
        public LoginForm()
        {
            InitializeComponent();
            loginController = new LoginController();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string pin = txtPin.Text.Trim();
            // Validate PIN length and digits
            if (pin.Length != 4 || !pin.All(char.IsDigit))
            {
                lblError.Text = "Please enter a valid 4-digit PIN";
                return;
            }

            // Call the login controller
            Manager manager = loginController.Login(pin);

            // If login failed
            if (manager == null)
            {
                lblError.Text = "Invalid PIN";
                txtPin.Clear();
                txtPin.Focus();
                return;
            }

            // Successful login
            OpenDashboard(manager);
            CurrentUser.Login(manager);
        }

        private void OpenDashboard(Manager manager)
        {
            MessageBox.Show($"Welcome, {CurrentUser.Manager.ManagerName}!","Login Successful",MessageBoxButtons.OK,MessageBoxIcon.Information);
            // TODO: Dashboard
        }
    }
}
