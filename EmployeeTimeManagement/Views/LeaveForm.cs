using EmployeeTimeManagement.Controllers;
using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

            cboBookLeaveType.Items.AddRange(LeaveTypes.BookableValues());
            cboBookLeaveType.SelectedIndex = 0;

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

            pnlBooking.Enabled = false;
            ResetBookingForm();
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

            pnlBooking.Enabled = true;
            ResetBookingForm();
        }

        // Puts the booking editor back to its default state: today's date on both pickers,
        // no reason, the first bookable Leave Type, and no message left over from before.
        private void ResetBookingForm()
        {
            cboBookLeaveType.SelectedIndex = 0;
            dtpBookStartDate.Value = DateTime.Today;
            dtpBookEndDate.Value = DateTime.Today;
            txtBookReason.Text = string.Empty;
            lblBookingMessage.Text = string.Empty;
            lblBookingMessage.ForeColor = SystemColors.ControlText;
            UpdateDayCountPreview();
        }

        // Shows how many calendar days the currently chosen range costs, before anything
        // is saved, so the deduction it implies is never a surprise.
        private void UpdateDayCountPreview()
        {
            int days = (dtpBookEndDate.Value.Date - dtpBookStartDate.Value.Date).Days + 1;

            lblBookDayCount.Text = days >= 1
                ? days + (days == 1 ? " day" : " days")
                : "End date is before the start date";
        }

        // Validates and books the Absence, writing nothing at all when it is refused.
        private void BookAbsence()
        {
            EmployeeListItem employee = employeePicker.SelectedEmployee;

            if (employee == null || CurrentUser.ManagerID == null)
            {
                return;
            }

            var request = new LeaveBookingRequest
            {
                EmployeeID = employee.EmployeeID,
                LeaveType = cboBookLeaveType.SelectedItem as string,
                StartDate = dtpBookStartDate.Value.Date,
                EndDate = dtpBookEndDate.Value.Date,
                Reason = txtBookReason.Text,
                ContractStartDate = employee.ContractStartDate,
                ContractEndDate = employee.ContractEndDate,
                ExistingAbsences = selectedEmployeeHistory
            };

            LeaveBookingResult result = LeaveBooking.Build(request);

            if (!result.IsValid)
            {
                ShowBookingMessage(result.Error, isError: true);
                return;
            }

            if (result.Warning != null)
            {
                DialogResult confirmed = MessageBox.Show(
                    result.Warning, "Long Absence", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (confirmed != DialogResult.OK)
                {
                    return;
                }
            }

            try
            {
                leaveController.Insert(result.Absence, CurrentUser.ManagerID.Value);
            }
            catch (Exception ex)
            {
                ShowBookingMessage("Could not book Absence: " + ex.Message, isError: true);
                return;
            }

            ShowHistoryFor(employee);
            ShowBookingMessage("Absence booked.", isError: false);
        }

        // Shows a booking outcome in red for a refusal or failure, or the default colour otherwise.
        private void ShowBookingMessage(string message, bool isError)
        {
            lblBookingMessage.Text = message;
            lblBookingMessage.ForeColor = isError ? Color.Red : SystemColors.ControlText;
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

        private void dtpBookDate_ValueChanged(object sender, EventArgs e)
        {
            UpdateDayCountPreview();
        }

        private void btnBookAbsence_Click(object sender, EventArgs e)
        {
            BookAbsence();
        }
    }
}
