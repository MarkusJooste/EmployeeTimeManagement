using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    public partial class DashboardForm : Form
    {
        // Tracks the current displayed view inside the dashboard
        private Form currentView;
        // Distunguishes between logout and exit
        private bool isLoggingOut = false;
        public DashboardForm()
        {
            InitializeComponent();
            LoadUserInformation();
            ConfigurePermissions();
            ShowHome();
        }

        private void LoadUserInformation()
        {
            if (CurrentUser.Manager == null)
            {
                return;
            }

            lblCurrentUser.Text = CurrentUser.Manager.ManagerName;
            lblStore.Text = $"Store ID: {CurrentUser.StoreID}";
        }

        private void ConfigurePermissions()
        {
            btnManagers.Visible = CurrentUser.IsAdmin;
        }

        private void ShowHome()
        {
            // Load the default view into the dashboard
            ShowView(new DashboardHomeForm());
        }

        private void ShowView(Form view)
        {
            if (currentView != null)
            {
                currentView.Close();
                currentView.Dispose();
            }

            // Set up the new view to display inside the panel
            currentView = view;
            view.TopLevel = false;
            view.FormBorderStyle = FormBorderStyle.None;
            view.Dock = DockStyle.Fill;

            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(view);
            view.Show();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            ShowHome();
        }

        private void btnEmployees_Click(object sender, EventArgs e)
        {
            //ShowView(new EmployeeForm))); Use this for when the form is created
        }

        private void btnContracts_Click(object sender, EventArgs e)
        {

        }

        private void btnTimesheet_Click(object sender, EventArgs e)
        {

        }

        private void btnReports_Click(object sender, EventArgs e)
        {

        }

        private void btnLeave_Click(object sender, EventArgs e)
        {

        }

        private void btnManagers_Click(object sender, EventArgs e)
        {

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }
            isLoggingOut = true;
            Close();
        }

        private void DashboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Removes current user session
            CurrentUser.Logout();
            // Exits the app if the user clicked the X
            if (!isLoggingOut)
            {
                Application.Exit();
            }
        }
    }
}
