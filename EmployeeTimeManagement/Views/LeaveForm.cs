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

        // The Leave Balances currently shown, so booking can check a new request against
        // them without recomputing from Timesheets and Absences a second time.
        private LeaveBalanceSummary currentBalances;

        // The Absence currently loaded into the booking editor for editing, or null while the
        // editor is booking a new one.
        private Absence editingAbsence;

        // Set while the grid's DataSource or selection is being changed programmatically, so
        // dgvHistory_SelectionChanged does not treat it as the manager picking a row to edit.
        private bool suppressSelectionEvent;

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

            // Set before LoadEmployees, which settles on its first row once the picker is
            // shown and so overwrites this with a real employee's history where the store
            // has any employees to show.
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

            ClearBalances();
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

            LoadBalances(employee);
        }

        // Recomputes every Leave Balance for the selected employee from their Timesheets and
        // the Absence history already loaded, so a fresh booking is reflected immediately.
        private void LoadBalances(EmployeeListItem employee)
        {
            if (CurrentUser.StoreID == null)
            {
                return;
            }

            try
            {
                List<DateTime> workedDates = leaveController.GetWorkedDates(employee.EmployeeID, CurrentUser.StoreID.Value);

                var request = new LeaveBalanceRequest
                {
                    AsOf = DateTime.Today,

                    // An employee with no contract row at all is still employed (Employee.IsEmployedOn)
                    // and can still have Timesheets, so falling back to today would wrongly cut every
                    // worked date out of accrual; MinValue imposes no cutoff at all instead.
                    ContractStartDate = employee.ContractStartDate ?? DateTime.MinValue,
                    OpeningBalanceAsAt = employee.OpeningBalanceAsAt,
                    OpeningPTODays = employee.OpeningPTODays,
                    WorkedDates = workedDates,
                    Absences = selectedEmployeeHistory
                };

                ShowBalances(LeaveBalanceCalculator.Calculate(request));
            }
            catch (Exception ex)
            {
                ClearBalances();
                lblStatus.Text = "Could not load this employee's Leave Balances: " + ex.Message;
            }
        }

        // Renders one computed LeaveBalanceSummary as the working-out a manager can defend,
        // not just the final answer.
        private void ShowBalances(LeaveBalanceSummary summary)
        {
            currentBalances = summary;

            PTOBalance pto = summary.PTO;

            lblPTOBalance.Text = string.Format(
                "PTO — Days Worked: {0}   Accrued: {1}   Opening Balance: {2}   Taken: {3}   Balance: {4}{5}",
                pto.DaysWorked, pto.DaysAccrued, pto.OpeningBalance, pto.DaysTaken, pto.Balance,
                pto.IsAtCap ? "   (At the 21-day cap — accrual paused)" : string.Empty);
            lblPTOBalance.ForeColor = pto.IsAtCap ? Color.Red : SystemColors.ControlText;

            lblSickBalance.Text = string.Format(
                "Sick — {0} of 30 days remaining, cycle ends {1:dd MMM yyyy}",
                summary.Sick.DaysRemaining, summary.Sick.CycleEndDate);

            lblMaternityBalance.Text = string.Format("Maternity — {0} days taken", summary.Maternity.DaysTaken);
            lblAWOLBalance.Text = string.Format("AWOL — {0} days taken", summary.AWOL.DaysTaken);
        }

        // Blanks the balances panel back to nothing, for no employee selected or a failed load.
        private void ClearBalances()
        {
            currentBalances = null;

            lblPTOBalance.Text = string.Empty;
            lblPTOBalance.ForeColor = SystemColors.ControlText;
            lblSickBalance.Text = string.Empty;
            lblMaternityBalance.Text = string.Empty;
            lblAWOLBalance.Text = string.Empty;
        }

        // Puts the booking editor back to its default state: today's date on both pickers,
        // no reason, the first bookable Leave Type, and no message left over from before. Also
        // drops the grid's selection, so no Absence stays half-loaded for editing.
        private void ResetBookingForm()
        {
            ResetBookingFields();

            lblBookingMessage.Text = string.Empty;
            lblBookingMessage.ForeColor = SystemColors.ControlText;

            suppressSelectionEvent = true;
            dgvHistory.ClearSelection();
            suppressSelectionEvent = false;
        }

        // Puts the booking editor's fields back to "book a new Absence" defaults, without
        // touching the grid selection or any message already shown - used when the AWOL lock
        // message needs the just-clicked row to stay visibly selected.
        private void ResetBookingFields()
        {
            editingAbsence = null;
            cboBookLeaveType.SelectedIndex = 0;
            dtpBookStartDate.Value = DateTime.Today;
            dtpBookEndDate.Value = DateTime.Today;
            txtBookReason.Text = string.Empty;
            txtBookOverrideReason.Text = string.Empty;
            btnBookAbsence.Text = "Book Absence";
            btnDeleteAbsence.Visible = false;
            btnNewAbsence.Visible = false;
            UpdateDayCountPreview();
        }

        // Loads one existing Absence into the booking editor, switching it into edit mode.
        private void EditAbsence(Absence absence)
        {
            editingAbsence = absence;

            cboBookLeaveType.SelectedItem = absence.LeaveType.ToDatabaseValue();
            dtpBookStartDate.Value = absence.StartDate.Date;
            dtpBookEndDate.Value = absence.EndDate.Date;
            txtBookReason.Text = absence.Reason ?? string.Empty;
            txtBookOverrideReason.Text = absence.OverrideReason ?? string.Empty;
            lblBookingMessage.Text = string.Empty;
            lblBookingMessage.ForeColor = SystemColors.ControlText;

            btnBookAbsence.Text = "Save Changes";
            btnDeleteAbsence.Visible = true;
            btnNewAbsence.Visible = true;

            UpdateDayCountPreview();
        }

        // AWOL rows are written from Capture Timesheets and would just be silently reversed by
        // its next save, so this screen explains that rather than loading one into the editor.
        private void ShowAwolLockedMessage()
        {
            ResetBookingFields();
            ShowBookingMessage("AWOL rows are captured from Capture Timesheets and cannot be edited or deleted here.", isError: false);
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

        // Validates and saves the Absence - inserting a new one, or updating editingAbsence in
        // place - writing nothing at all when it is refused.
        private void SaveAbsence()
        {
            EmployeeListItem employee = employeePicker.SelectedEmployee;

            if (employee == null || CurrentUser.ManagerID == null || CurrentUser.StoreID == null)
            {
                return;
            }

            string leaveType = cboBookLeaveType.SelectedItem as string;
            bool isEdit = editingAbsence != null;

            var request = new LeaveBookingRequest
            {
                EmployeeID = employee.EmployeeID,
                LeaveType = leaveType,
                StartDate = dtpBookStartDate.Value.Date,
                EndDate = dtpBookEndDate.Value.Date,
                Reason = txtBookReason.Text,
                ContractStartDate = employee.ContractStartDate,
                ContractEndDate = employee.ContractEndDate,
                ExistingAbsences = selectedEmployeeHistory,
                AvailableBalance = AvailableBalanceFor(leaveType, editingAbsence),
                OverrideReason = txtBookOverrideReason.Text,
                EditingLeaveID = isEdit ? editingAbsence.LeaveID : (int?)null
            };

            LeaveBookingResult result = LeaveBooking.Build(request);

            if (!result.IsValid)
            {
                ShowBookingMessage(result.Error, isError: true);

                if (result.RequiresOverride)
                {
                    txtBookOverrideReason.Focus();
                }

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
                if (isEdit)
                {
                    leaveController.Update(result.Absence, CurrentUser.StoreID.Value);
                }
                else
                {
                    leaveController.Insert(result.Absence, CurrentUser.ManagerID.Value);
                }
            }
            catch (AccessRevokedException ex)
            {
                AccessRevokedPrompt.Show(ex);
                return;
            }
            catch (Exception ex)
            {
                ShowBookingMessage("Could not save Absence: " + ex.Message, isError: true);
                return;
            }

            ShowHistoryFor(employee);
            ShowBookingMessage(isEdit ? "Absence updated." : "Absence booked.", isError: false);
        }

        // Deletes editingAbsence after the manager confirms, refusing an AWOL row as a second
        // line of defence even though the editor never loads one for editing in the first place.
        private void DeleteAbsence()
        {
            EmployeeListItem employee = employeePicker.SelectedEmployee;

            if (employee == null || editingAbsence == null || CurrentUser.StoreID == null)
            {
                return;
            }

            if (editingAbsence.LeaveType == LeaveType.AWOL)
            {
                return;
            }

            DialogResult confirmed = MessageBox.Show(
                string.Format(
                    "Delete this {0} Absence ({1:dd MMM yyyy} to {2:dd MMM yyyy})?",
                    editingAbsence.LeaveTypeDisplay, editingAbsence.StartDate, editingAbsence.EndDate),
                "Delete Absence", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmed != DialogResult.Yes)
            {
                return;
            }

            try
            {
                leaveController.Delete(editingAbsence.LeaveID, CurrentUser.StoreID.Value);
            }
            catch (AccessRevokedException ex)
            {
                AccessRevokedPrompt.Show(ex);
                return;
            }
            catch (Exception ex)
            {
                ShowBookingMessage("Could not delete Absence: " + ex.Message, isError: true);
                return;
            }

            ShowHistoryFor(employee);
            ShowBookingMessage("Absence deleted.", isError: false);
        }

        // The Leave Balance the over-balance check should compare this booking's cost against.
        // Null for a Leave Type with no cap to exceed, or while no balances are loaded yet. While
        // editing an Absence of the same Leave Type, its own current days are added back first,
        // since the loaded balance already has them deducted.
        private int? AvailableBalanceFor(string leaveType, Absence editing)
        {
            LeaveType parsed;
            if (currentBalances == null || !LeaveTypes.TryParseBookable(leaveType, out parsed))
            {
                return null;
            }

            int? available = currentBalances.AvailableBalance(parsed);

            if (available.HasValue && editing != null && editing.LeaveType == parsed)
            {
                available += editing.DayCount;
            }

            return available;
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

            suppressSelectionEvent = true;
            dgvHistory.DataSource = new BindingList<Absence>(matches.ToList());
            suppressSelectionEvent = false;
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

            // The row being edited may no longer be in view under the new filter, so the editor
            // drops back to booking a new Absence rather than saving into a row nobody can see.
            if (editingAbsence != null)
            {
                ResetBookingForm();
            }
        }

        private void dgvHistory_SelectionChanged(object sender, EventArgs e)
        {
            if (suppressSelectionEvent)
            {
                return;
            }

            var selected = dgvHistory.CurrentRow?.DataBoundItem as Absence;

            if (selected == null)
            {
                return;
            }

            if (selected.LeaveType == LeaveType.AWOL)
            {
                ShowAwolLockedMessage();
                return;
            }

            EditAbsence(selected);
        }

        private void dtpBookDate_ValueChanged(object sender, EventArgs e)
        {
            UpdateDayCountPreview();
        }

        private void btnBookAbsence_Click(object sender, EventArgs e)
        {
            SaveAbsence();
        }

        private void btnDeleteAbsence_Click(object sender, EventArgs e)
        {
            DeleteAbsence();
        }

        private void btnNewAbsence_Click(object sender, EventArgs e)
        {
            ResetBookingForm();
        }
    }
}
