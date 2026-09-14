using EmployeeTimeManagement.Controllers;
using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    public partial class LeaveForm : Form
    {
        // Shown as the first item of the Leave Type filter, standing for no filter at all.
        private const string AllLeaveTypesLabel = "All Leave Types";

        private readonly EmployeeController employeeController;
        private readonly LeaveController leaveController;

        // Every Absence loaded for the currently selected employee, before the Leave Type filter narrows it.
        private List<Absence> selectedEmployeeHistory = new List<Absence>();

        public LeaveForm()
        {
            InitializeComponent();

            employeeController = new EmployeeController();
            leaveController = new LeaveController();

            cboLeaveType.Items.Add(AllLeaveTypesLabel);
            cboLeaveType.Items.AddRange(LeaveTypes.AllDatabaseValues());
            cboLeaveType.SelectedIndex = 0;

            // Set before LoadEmployees, which selects its first row as a side effect of
            // binding the grid and so immediately overwrites this with a real employee's
            // history where the store has any employees to show.
            ShowNoSelection();
            LoadEmployees();
        }

        // Fetches the logged-in manager's store from the database and hands it to the employee picker.
        private void LoadEmployees()
        {
            if (CurrentUser.StoreID == null)
            {
                employeePicker.SetEmployees(new List<EmployeeListItem>());
                lblStatus.Text = "No store assigned to this user";
                employeePicker.Enabled = false;
                return;
            }

            List<EmployeeListItem> employees;

            try
            {
                employees = employeeController.GetListByStore(CurrentUser.StoreID.Value);
            }
            catch (Exception ex)
            {
                employeePicker.SetEmployees(new List<EmployeeListItem>());
                lblStatus.Text = "Could not load employees: " + ex.Message;
                employeePicker.Enabled = false;
                return;
            }

            employeePicker.SetEmployees(employees);
        }

        // Summarises the displayed rows in the status label, matching the Employees view.
        private void ShowCount(List<EmployeeListItem> employees)
        {
            if (employees.Count == 0)
            {
                lblStatus.Text = "No employees to show";
                return;
            }

            lblStatus.Text = employees.Count == 1 ? "1 employee" : $"{employees.Count} employees";
        }

        // Clears the history panel back to its no-selection state.
        private void ShowNoSelection()
        {
            selectedEmployeeHistory = new List<Absence>();
            lblSelectedEmployee.Text = "Select an employee to see their Absence history";
            cboLeaveType.Enabled = false;
            dgvHistory.DataSource = null;
        }

        // Re-reads this employee's whole Absence history and shows it, newest first.
        private void ShowHistoryFor(EmployeeListItem employee)
        {
            if (CurrentUser.StoreID == null)
            {
                return;
            }

            lblSelectedEmployee.Text = "Absence history: " + employee.FullName;
            cboLeaveType.Enabled = true;

            try
            {
                selectedEmployeeHistory = leaveController.GetHistoryForEmployee(employee.EmployeeID, CurrentUser.StoreID.Value);
            }
            catch (Exception ex)
            {
                selectedEmployeeHistory = new List<Absence>();
                lblStatus.Text = "Could not load this employee's Absence history: " + ex.Message;
            }

            ApplyLeaveTypeFilter();
        }

        // Narrows the loaded history by the Leave Type filter without re-querying the database.
        private void ApplyLeaveTypeFilter()
        {
            IEnumerable<Absence> matches = selectedEmployeeHistory;

            string selected = cboLeaveType.SelectedItem as string;

            if (!string.IsNullOrEmpty(selected) && selected != AllLeaveTypesLabel)
            {
                LeaveType type = LeaveTypes.FromDatabaseValue(selected);
                matches = matches.Where(absence => absence.LeaveType == type);
            }

            dgvHistory.DataSource = new BindingList<Absence>(matches.ToList());
        }

        private void employeePicker_SelectionChanged(object sender, EventArgs e)
        {
            EmployeeListItem selected = employeePicker.SelectedEmployee;

            if (selected == null)
            {
                ShowNoSelection();
                return;
            }

            ShowHistoryFor(selected);
        }

        private void employeePicker_FilterChanged(object sender, EventArgs e)
        {
            ShowCount(employeePicker.VisibleEmployees);
        }

        private void cboLeaveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyLeaveTypeFilter();
        }
    }
}
