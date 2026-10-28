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
            ApplyTheme();
            LoadUserInformation();
            ConfigurePermissions();
            ShowHome();
            CurrentUser.AccessRevoked += HandleAccessRevoked;
        }

        // Closes the dashboard the same way Logout does, returning to the login screen.
        private void HandleAccessRevoked()
        {
            isLoggingOut = true;
            Close();
        }

        // The red sidebar and white header strip, after wimpy.co.za's red navigation bar.
        private void ApplyTheme()
        {
            BackColor = WimpyTheme.Page;
            pnlContent.BackColor = WimpyTheme.Page;
            pnlSidebar.BackColor = WimpyTheme.Red;
            pnlHeader.BackColor = WimpyTheme.Card;
            pnlHeaderLine.BackColor = WimpyTheme.BrightRed;

            lblAppName.Font = WimpyTheme.Heading(15F);
            lblAppName.ForeColor = Color.White;

            lblTitle.Font = WimpyTheme.Heading(16F);
            lblTitle.ForeColor = WimpyTheme.Text;

            lblCurrentUser.Font = WimpyTheme.BodyBold;
            lblCurrentUser.ForeColor = Color.White;
            lblStore.Font = WimpyTheme.Body;
            lblStore.ForeColor = Color.FromArgb(245, 200, 202);

            foreach (Button button in NavButtons())
            {
                WimpyTheme.StyleButton(button, ButtonRole.Nav);
            }

            // Outlined in white on the red, like the website's Order Online button.
            WimpyTheme.StyleButton(btnLogout, ButtonRole.Primary);
            btnLogout.BackColor = WimpyTheme.Red;
            btnLogout.FlatAppearance.BorderColor = Color.White;
            btnLogout.FlatAppearance.MouseOverBackColor = WimpyTheme.PressedRed;
            btnLogout.FlatAppearance.MouseDownBackColor = WimpyTheme.PressedRed;
        }

        private Button[] NavButtons()
        {
            return new[] { btnDashboard, btnEmployees, btnContracts, btnCaptureTimesheets, btnViewTimesheets, btnLeave, btnManagers };
        }

        private void LoadUserInformation()
        {
            if (CurrentUser.Manager == null)
            {
                return;
            }

            lblCurrentUser.Text = CurrentUser.Manager.ManagerName;
            lblStore.Text = CurrentUser.IsAdmin ? "All stores" : $"Store {CurrentUser.StoreID}";
        }

        private void ConfigurePermissions()
        {
            btnManagers.Visible = CurrentUser.IsAdmin;
        }

        private void ShowHome()
        {
            // Load the default view into the dashboard
            var home = new DashboardHomeForm();
            home.OpenEmployeeRequested += OpenEmployee;
            ShowView(home, btnDashboard);
        }

        // Switches to the Employees screen with one employee selected, ready to update. Queued
        // because the request comes from a click on the dashboard, which ShowView disposes;
        // disposing a control inside its own Click handler can fail once the handler returns.
        private void OpenEmployee(int employeeID)
        {
            BeginInvoke(new Action(() =>
            {
                var employees = new EmployeesForm();
                ShowView(employees, btnEmployees);
                employees.ShowEmployee(employeeID);
            }));
        }

        // Shows a screen in the content panel, marks its link in the sidebar and names it in the header.
        private void ShowView(Form view, Button navButton)
        {
            foreach (Button button in NavButtons())
            {
                WimpyTheme.SetNavSelected(button, button == navButton);
            }

            lblTitle.Text = navButton.Text;
            WimpyTheme.Apply(view);

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
            ShowView(new EmployeesForm(), btnEmployees);
        }

        private void btnContracts_Click(object sender, EventArgs e)
        {
            ShowView(new ContractsForm(), btnContracts);
        }

        private void btnCaptureTimesheets_Click(object sender, EventArgs e)
        {
            ShowView(new CaptureTimesheetsForm(), btnCaptureTimesheets);
        }

        private void btnViewTimesheets_Click(object sender, EventArgs e)
        {
            ShowView(new ViewTimesheetsForm(), btnViewTimesheets);
        }

        private void btnLeave_Click(object sender, EventArgs e)
        {
            ShowView(new LeaveForm(), btnLeave);
        }

        private void btnManagers_Click(object sender, EventArgs e)
        {
            ShowView(new ManagersForm(), btnManagers);
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
            CurrentUser.AccessRevoked -= HandleAccessRevoked;

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
