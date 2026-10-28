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
using EmployeeTimeManagement.Views;

namespace EmployeeTimeManagement.Views
{
    public partial class LoginForm : Form
    {
        private readonly LoginController loginController;
        public LoginForm()
        {
            InitializeComponent();
            ApplyTheme();
            loginController = new LoginController();
        }

        // Red like the website's header, with the white button it uses on red.
        private void ApplyTheme()
        {
            WimpyTheme.Apply(this);
            BackColor = WimpyTheme.Red;
            Text = "Employee Time Management";

            lblTitle.ForeColor = Color.White;
            lblTitle.Left = (ClientSize.Width - lblTitle.Width) / 2;
            lblTitle.Top = 80;

            lblPin.ForeColor = Color.White;
            lblPin.Font = WimpyTheme.BodyBold;
            lblPin.Text = "Enter your 4-digit PIN";
            lblPin.Left = (ClientSize.Width - lblPin.Width) / 2;
            lblPin.Top = txtPin.Top - lblPin.Height - 8;

            txtPin.Font = WimpyTheme.Heading(14F);
            txtPin.TextAlign = HorizontalAlignment.Center;
            txtPin.Width = 160;
            txtPin.Left = (ClientSize.Width - txtPin.Width) / 2;
            lblPin.Top = txtPin.Top - lblPin.Height - 8;

            WimpyTheme.StyleButton(btnLogin, ButtonRole.Secondary);
            btnLogin.Font = WimpyTheme.Heading(12F);
            btnLogin.FlatAppearance.BorderColor = Color.White;
            btnLogin.Height = 40;
            btnLogin.Width = txtPin.Width;
            btnLogin.Left = txtPin.Left;
            btnLogin.Top = txtPin.Bottom + 16;

            // Centred across the form, since its text changes length.
            lblError.AutoSize = false;
            lblError.Left = 0;
            lblError.Width = ClientSize.Width;
            lblError.Top = btnLogin.Bottom + 16;
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.ForeColor = Color.White;
            lblError.Font = WimpyTheme.BodyBold;
            lblError.Text = string.Empty;
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
            CurrentUser.Login(manager);
            OpenDashboard(manager);
        }

        private void OpenDashboard(Manager manager)
        {
            DashboardForm dashboardForm = new DashboardForm();

            Hide();
            dashboardForm.FormClosed += DashboardForm_FormClosed;
            dashboardForm.Show();
        }

        private void DashboardForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // When the dashboard form is closed it will show the login form again
            Show();
            txtPin.Clear();
            txtPin.Focus();
        }
    }
}
