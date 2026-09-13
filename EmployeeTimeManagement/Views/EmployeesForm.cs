using EmployeeTimeManagement.Controllers;
using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    public partial class EmployeesForm : Form
    {
        private readonly EmployeeController employeeController;
        private List<EmployeeListItem> allEmployees = new List<EmployeeListItem>();

        public EmployeesForm()
        {
            InitializeComponent();
            employeeController = new EmployeeController();
            LoadData();
        }

        // Fetches the logged-in manager's store from the database and binds it to the grid
        private void LoadData()
        {
            if (CurrentUser.StoreID == null)
            {
                allEmployees = new List<EmployeeListItem>();
                dgvEmployees.DataSource = null;
                lblStatus.Text = "No store assigned to this user";
                DisableAllButtons();
                return;
            }

            try
            {
                allEmployees = employeeController.GetListByStore(CurrentUser.StoreID.Value);
            }
            catch (Exception ex)
            {
                allEmployees = new List<EmployeeListItem>();
                dgvEmployees.DataSource = null;
                lblStatus.Text = "Could not load employees: " + ex.Message;
                DisableAllButtons();
                return;
            }

            ApplyFilter();
        }

        // Narrows the loaded employees by the search text and the former-employee checkbox without re-querying the database
        private void ApplyFilter()
        {
            IEnumerable<EmployeeListItem> matches = allEmployees;

            if (!chkShowFormer.Checked)
            {
                matches = matches.Where(employee => employee.IsActive);
            }

            string search = txtSearch.Text.Trim();

            if (search.Length > 0)
            {
                matches = matches.Where(employee => Contains(employee.Name, search)
                                                 || Contains(employee.Surname, search)
                                                 || Contains(employee.IDNumber, search));
            }

            List<EmployeeListItem> filtered = matches.ToList();

            dgvEmployees.DataSource = new BindingList<EmployeeListItem>(filtered);
            ShowCount(filtered);
            UpdateButtonState();
        }

        // Case-insensitive substring match that treats a missing value as no match
        private static bool Contains(string value, string search)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return value.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        // Summarises the displayed rows in the status label
        private void ShowCount(List<EmployeeListItem> employees)
        {
            if (employees.Count == 0)
            {
                lblStatus.Text = "No employees to show";
                return;
            }

            lblStatus.Text = employees.Count == 1 ? "1 employee" : $"{employees.Count} employees";
        }

        // The employee whose row is selected, or null when nothing is selected
        private EmployeeListItem SelectedEmployee
        {
            get
            {
                if (dgvEmployees.CurrentRow == null || dgvEmployees.CurrentRow.Index < 0)
                {
                    return null;
                }

                return dgvEmployees.CurrentRow.DataBoundItem as EmployeeListItem;
            }
        }

        // Update, Terminate and Reactivate act on one person, so they stay disabled while no row is selected
        private void UpdateButtonState()
        {
            bool hasSelection = SelectedEmployee != null;

            btnUpdate.Enabled = hasSelection;
            btnTerminate.Enabled = hasSelection;
            btnReactivate.Enabled = hasSelection;
        }

        // Nothing on this screen is safe to press when there is no store to act on
        private void DisableAllButtons()
        {
            btnAdd.Enabled = false;
            btnUpdate.Enabled = false;
            btnTerminate.Enabled = false;
            btnReactivate.Enabled = false;
            txtSearch.Enabled = false;
            chkShowFormer.Enabled = false;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void chkShowFormer_CheckedChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonState();
        }
    }
}
