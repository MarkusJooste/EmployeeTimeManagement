using EmployeeTimeManagement.Controllers;
using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    public partial class CaptureTimesheetsForm : Form
    {
        private const string ColumnEmployee = "colEmployee";
        private const string ColumnStatus = "colStatus";
        private const string ColumnDayType = "colDayType";
        private const string ColumnNotes = "colNotes";
        private const string ColumnTimeIn = "colTimeIn";
        private const string ColumnTimeOut = "colTimeOut";
        private const string ColumnBreak1Start = "colBreak1Start";
        private const string ColumnBreak1End = "colBreak1End";
        private const string ColumnBreak2Start = "colBreak2Start";
        private const string ColumnBreak2End = "colBreak2End";

        private static readonly string[] TimeColumns =
        {
            ColumnTimeIn, ColumnTimeOut, ColumnBreak1Start, ColumnBreak1End, ColumnBreak2Start, ColumnBreak2End
        };

        private static readonly Color SavedRowColour = Color.FromArgb(232, 245, 233);
        private static readonly Color LockedCellColour = SystemColors.Control;

        private readonly EmployeeController employeeController;
        private readonly TimesheetController timesheetController;

        private List<Employee> employees = new List<Employee>();

        // Guards against grid events re-entering while cells are being written by code.
        private bool isSettingCells;

        // Guards against the date picker's own ValueChanged firing while it is put back.
        private bool isRevertingDate;

        // The date the rows currently on the grid belong to.
        private DateTime currentWorkDate;

        public CaptureTimesheetsForm()
        {
            InitializeComponent();

            employeeController = new EmployeeController();
            timesheetController = new TimesheetController();

            BuildColumns();
            WireGridEvents();

            currentWorkDate = DateTime.Today;
            dtpWorkDate.Value = currentWorkDate;
            ShowDayTypeForDate();
            LoadEmployees();
        }

        // Builds the ten capture columns. Done in code so the employee dropdown can be
        // bound to the store's employees at run time.
        private void BuildColumns()
        {
            dgvCapture.AutoGenerateColumns = false;
            dgvCapture.AllowUserToAddRows = true;
            dgvCapture.AllowUserToDeleteRows = true;
            dgvCapture.EditMode = DataGridViewEditMode.EditOnEnter;

            var employeeColumn = new DataGridViewComboBoxColumn();
            employeeColumn.Name = ColumnEmployee;
            employeeColumn.HeaderText = "Employee";
            employeeColumn.DisplayMember = "FullName";
            employeeColumn.ValueMember = "EmployeeID";
            employeeColumn.Width = 140;
            employeeColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            dgvCapture.Columns.Add(employeeColumn);

            var statusColumn = new DataGridViewComboBoxColumn();
            statusColumn.Name = ColumnStatus;
            statusColumn.HeaderText = "Status";
            statusColumn.Width = 75;
            statusColumn.Items.AddRange(TimesheetStatuses.AllDatabaseValues());
            dgvCapture.Columns.Add(statusColumn);

            var dayTypeColumn = new DataGridViewComboBoxColumn();
            dayTypeColumn.Name = ColumnDayType;
            dayTypeColumn.HeaderText = "Day type";
            dayTypeColumn.Width = 100;
            dayTypeColumn.Items.AddRange(DayTypes.AllDatabaseValues());
            dgvCapture.Columns.Add(dayTypeColumn);

            AddTimeColumn(ColumnTimeIn, "In");
            AddTimeColumn(ColumnTimeOut, "Out");
            AddTimeColumn(ColumnBreak1Start, "Break 1 from");
            AddTimeColumn(ColumnBreak1End, "Break 1 to");
            AddTimeColumn(ColumnBreak2Start, "Break 2 from");
            AddTimeColumn(ColumnBreak2End, "Break 2 to");

            var notesColumn = new DataGridViewTextBoxColumn();
            notesColumn.Name = ColumnNotes;
            notesColumn.HeaderText = "Notes";
            notesColumn.Width = 150;
            notesColumn.MaxInputLength = 255;
            dgvCapture.Columns.Add(notesColumn);
        }

        // Adds one of the six free-text time columns.
        private void AddTimeColumn(string name, string header)
        {
            var column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = header;
            column.Width = 65;
            column.MaxInputLength = 5;
            dgvCapture.Columns.Add(column);
        }

        // Subscribes to the grid events that drive defaulting, conflict checks and normalising.
        private void WireGridEvents()
        {
            dgvCapture.DefaultValuesNeeded += dgvCapture_DefaultValuesNeeded;
            dgvCapture.CellValueChanged += dgvCapture_CellValueChanged;
            dgvCapture.CurrentCellDirtyStateChanged += dgvCapture_CurrentCellDirtyStateChanged;
            dgvCapture.CellEndEdit += dgvCapture_CellEndEdit;
        }

        // Fetches the store's employees, or explains why capture is unavailable.
        private void LoadEmployees()
        {
            if (CurrentUser.StoreID == null)
            {
                DisableCapture("No store assigned to this manager");
                return;
            }

            try
            {
                employees = employeeController.GetByStore(CurrentUser.StoreID.Value);
            }
            catch (Exception ex)
            {
                DisableCapture("Could not load employees: " + ex.Message);
                return;
            }

            if (employees.Count == 0)
            {
                DisableCapture("No employees at this store");
                return;
            }

            var employeeColumn = (DataGridViewComboBoxColumn)dgvCapture.Columns[ColumnEmployee];
            employeeColumn.DataSource = employees;
        }

        // Puts the view into a read-only state with an explanation.
        private void DisableCapture(string message)
        {
            dgvCapture.AllowUserToAddRows = false;
            dgvCapture.Enabled = false;
            btnSave.Enabled = false;
            lblStatus.Text = message;
        }

        // Shows the day type the chosen date implies, before any per-row override.
        private void ShowDayTypeForDate()
        {
            DayType dayType = SouthAfricanHolidays.GetDayType(dtpWorkDate.Value.Date);
            lblDayType.Text = dayType.ToDatabaseValue();
        }

        // Seeds a newly added row with the sensible defaults for this date.
        private void dgvCapture_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[ColumnStatus].Value = TimesheetStatus.Worked.ToDatabaseValue();
            e.Row.Cells[ColumnDayType].Value = SouthAfricanHolidays.GetDayType(dtpWorkDate.Value.Date).ToDatabaseValue();
            e.Row.Tag = new CaptureRowState();
        }

        // Commits combo box edits immediately so the conflict check fires on selection
        // rather than when the cell is left.
        private void dgvCapture_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvCapture.IsCurrentCellDirty && dgvCapture.CurrentCell is DataGridViewComboBoxCell)
            {
                dgvCapture.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        // Reacts to the two cells that change the rest of the row: employee and status.
        private void dgvCapture_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isSettingCells || e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row = dgvCapture.Rows[e.RowIndex];
            string columnName = dgvCapture.Columns[e.ColumnIndex].Name;

            if (columnName == ColumnEmployee)
            {
                HandleEmployeeChosen(row);
            }
            else if (columnName == ColumnStatus)
            {
                ApplyStatusToRow(row);
            }
        }

        // Normalises a time cell so the grid always shows the canonical HH:mm.
        private void dgvCapture_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (isSettingCells || e.RowIndex < 0)
            {
                return;
            }

            if (Array.IndexOf(TimeColumns, dgvCapture.Columns[e.ColumnIndex].Name) < 0)
            {
                return;
            }

            DataGridViewCell cell = dgvCapture.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string text = cell.Value as string;

            TimeSpan? parsed;
            if (TimesheetCapture.TryParseTime(text, out parsed) && parsed.HasValue)
            {
                isSettingCells = true;
                cell.Value = TimesheetCapture.FormatTime(parsed);
                isSettingCells = false;
            }
        }

        // Checks the chosen employee against what is already stored for this date, and
        // offers to update rather than silently creating a second Timesheet.
        private void HandleEmployeeChosen(DataGridViewRow row)
        {
            var state = EnsureState(row);
            state.ExistingTimesheetID = null;

            object value = row.Cells[ColumnEmployee].Value;
            if (value == null)
            {
                return;
            }

            int employeeID = Convert.ToInt32(value);

            // Catch a repeat within this session before going near the database, so the
            // manager is told at the moment they choose rather than at save time.
            if (IsAlreadyOnGrid(row, employeeID))
            {
                MessageBox.Show(
                    EmployeeName(employeeID) + " is already on this grid. Nobody can be captured twice on one date.",
                    "Already added",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                RemoveRow(row);
                return;
            }

            List<Timesheet> existing;
            try
            {
                existing = timesheetController.GetByEmployeeAndDate(employeeID, dtpWorkDate.Value.Date);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Could not check for existing timesheets: " + ex.Message;
                return;
            }

            if (existing.Count == 0)
            {
                return;
            }

            Timesheet chosen = ChooseExisting(existing);
            if (chosen == null)
            {
                RemoveRow(row);
                return;
            }

            state.ExistingTimesheetID = chosen.TimesheetID;
            FillRowFrom(row, chosen);
        }

        // Reports whether another row on the grid already holds this employee.
        private bool IsAlreadyOnGrid(DataGridViewRow candidate, int employeeID)
        {
            foreach (DataGridViewRow row in dgvCapture.Rows)
            {
                if (row.IsNewRow || row.Index == candidate.Index)
                {
                    continue;
                }

                object value = row.Cells[ColumnEmployee].Value;
                if (value != null && Convert.ToInt32(value) == employeeID)
                {
                    return true;
                }
            }

            return false;
        }

        // Takes a row off the grid entirely. Blanking it would leave an employee-less row
        // behind that blocks the next save.
        private void RemoveRow(DataGridViewRow row)
        {
            if (row.IsNewRow)
            {
                return;
            }

            isSettingCells = true;
            try
            {
                dgvCapture.Rows.Remove(row);
            }
            finally
            {
                isSettingCells = false;
            }
        }

        // Asks the manager whether to update the stored Timesheet, and which one when
        // the employee somehow has more than one for this date.
        private Timesheet ChooseExisting(List<Timesheet> existing)
        {
            if (existing.Count > 1)
            {
                using (var picker = new TimesheetPickerForm(existing))
                {
                    if (picker.ShowDialog(this) != DialogResult.OK)
                    {
                        return null;
                    }

                    return picker.SelectedTimesheet;
                }
            }

            Timesheet only = existing[0];
            string message = string.Format(
                "{0} already has a timesheet for {1:dd MMM yyyy} ({2}).{3}{3}Update it?",
                EmployeeName(only.EmployeeID),
                only.WorkDate,
                only.DescribeWhen(),
                Environment.NewLine);

            DialogResult result = MessageBox.Show(
                message, "Timesheet already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return null;
            }

            return only;
        }

        // Names an employee for a message box, falling back when the list has moved on.
        private string EmployeeName(int employeeID)
        {
            Employee employee = employees.FirstOrDefault(e => e.EmployeeID == employeeID);

            if (employee == null)
            {
                return "This employee";
            }

            return employee.FullName;
        }

        // Prefills a row from a stored Timesheet so correcting one field does not wipe
        // everything else on the record.
        private void FillRowFrom(DataGridViewRow row, Timesheet timesheet)
        {
            isSettingCells = true;
            try
            {
                row.Cells[ColumnStatus].Value = timesheet.Status.ToDatabaseValue();
                row.Cells[ColumnDayType].Value = timesheet.DayType.ToDatabaseValue();
                row.Cells[ColumnTimeIn].Value = TimesheetCapture.FormatTime(timesheet.TimeIn);
                row.Cells[ColumnTimeOut].Value = TimesheetCapture.FormatTime(timesheet.TimeOut);
                row.Cells[ColumnBreak1Start].Value = TimesheetCapture.FormatTime(timesheet.Break1Start);
                row.Cells[ColumnBreak1End].Value = TimesheetCapture.FormatTime(timesheet.Break1End);
                row.Cells[ColumnBreak2Start].Value = TimesheetCapture.FormatTime(timesheet.Break2Start);
                row.Cells[ColumnBreak2End].Value = TimesheetCapture.FormatTime(timesheet.Break2End);
                row.Cells[ColumnNotes].Value = timesheet.Notes;
            }
            finally
            {
                isSettingCells = false;
            }

            ApplyStatusToRow(row);
            row.HeaderCell.Value = "U";
            row.HeaderCell.ToolTipText = "Updates an existing timesheet";
        }

        // Locks and greys a row's time cells when the employee was not at work, so hours
        // cannot be recorded against an absence.
        private void ApplyStatusToRow(DataGridViewRow row)
        {
            var status = row.Cells[ColumnStatus].Value as string;
            bool worked = string.IsNullOrEmpty(status)
                          || string.Equals(status, TimesheetStatus.Worked.ToDatabaseValue(), StringComparison.OrdinalIgnoreCase);

            isSettingCells = true;
            try
            {
                foreach (string columnName in TimeColumns)
                {
                    DataGridViewCell cell = row.Cells[columnName];
                    cell.ReadOnly = !worked;

                    if (worked)
                    {
                        cell.Style.BackColor = Color.Empty;
                    }
                    else
                    {
                        cell.Style.BackColor = LockedCellColour;
                    }

                    if (!worked)
                    {
                        cell.Value = null;
                    }
                }
            }
            finally
            {
                isSettingCells = false;
            }
        }

        // Returns the row's state, attaching one if the row does not have it yet.
        private static CaptureRowState EnsureState(DataGridViewRow row)
        {
            var state = row.Tag as CaptureRowState;
            if (state == null)
            {
                state = new CaptureRowState();
                row.Tag = state;
            }

            return state;
        }

        // Warns before a date change throws away work that has not been saved, and puts
        // the picker back if the manager decides against it.
        private void dtpWorkDate_ValueChanged(object sender, EventArgs e)
        {
            if (isRevertingDate)
            {
                return;
            }

            if (HasUnsavedRows())
            {
                DialogResult result = MessageBox.Show(
                    "There are timesheets on this screen that have not been saved. Changing the date will discard them.",
                    "Discard unsaved timesheets?",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.OK)
                {
                    // The picker has already moved, so cancelling has to move it back or
                    // the rows on screen would save against the wrong WorkDate.
                    RevertDate();
                    return;
                }
            }

            dgvCapture.Rows.Clear();
            lblStatus.Text = string.Empty;
            currentWorkDate = dtpWorkDate.Value.Date;
            ShowDayTypeForDate();
        }

        // Puts the date picker back to the date the grid was captured against.
        private void RevertDate()
        {
            isRevertingDate = true;
            try
            {
                dtpWorkDate.Value = currentWorkDate;
            }
            finally
            {
                isRevertingDate = false;
            }
        }

        // Reports whether any row on the grid has not yet been written to the database.
        private bool HasUnsavedRows()
        {
            foreach (DataGridViewRow row in dgvCapture.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                var state = row.Tag as CaptureRowState;
                if (state == null || !state.IsSaved)
                {
                    return true;
                }
            }

            return false;
        }

        // Saves the day when the manager asks.
        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        // Validates every row, then writes the whole day at once.
        private void Save()
        {
            if (CurrentUser.ManagerID == null)
            {
                lblStatus.Text = "No manager is signed in";
                return;
            }

            dgvCapture.EndEdit();
            ClearRowErrors();

            List<DataGridViewRow> rows = CapturableRows();
            if (rows.Count == 0)
            {
                lblStatus.Text = "Nothing to save";
                return;
            }

            List<CaptureRow> captureRows = rows.Select(ToCaptureRow).ToList();

            CaptureResult result = TimesheetCapture.Build(
                dtpWorkDate.Value.Date, CurrentUser.ManagerID.Value, captureRows);

            if (!result.IsValid)
            {
                ShowRowErrors(rows, result);
                return;
            }

            try
            {
                timesheetController.Save(result.Valid);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Could not save timesheets: " + ex.Message;
                return;
            }

            MarkSaved(rows, result.Valid);
            lblStatus.Text = string.Format("Saved {0} timesheet(s) for {1:dd MMM yyyy}", result.Valid.Count, dtpWorkDate.Value.Date);
        }

        // Returns the rows holding real input, skipping the grid's trailing blank row.
        private List<DataGridViewRow> CapturableRows()
        {
            return dgvCapture.Rows.Cast<DataGridViewRow>().Where(row => !row.IsNewRow).ToList();
        }

        // Reads one grid row into the raw shape the capture module validates.
        private CaptureRow ToCaptureRow(DataGridViewRow row, int index)
        {
            var state = EnsureState(row);

            int? employeeID = null;
            if (row.Cells[ColumnEmployee].Value != null)
            {
                employeeID = Convert.ToInt32(row.Cells[ColumnEmployee].Value);
            }

            return new CaptureRow
            {
                RowIndex = index,
                EmployeeID = employeeID,
                Status = row.Cells[ColumnStatus].Value as string,
                DayType = row.Cells[ColumnDayType].Value as string,
                TimeIn = row.Cells[ColumnTimeIn].Value as string,
                TimeOut = row.Cells[ColumnTimeOut].Value as string,
                Break1Start = row.Cells[ColumnBreak1Start].Value as string,
                Break1End = row.Cells[ColumnBreak1End].Value as string,
                Break2Start = row.Cells[ColumnBreak2Start].Value as string,
                Break2End = row.Cells[ColumnBreak2End].Value as string,
                Notes = row.Cells[ColumnNotes].Value as string,
                ExistingTimesheetID = state.ExistingTimesheetID
            };
        }

        // Clears the error markers left by a previous save attempt.
        private void ClearRowErrors()
        {
            foreach (DataGridViewRow row in dgvCapture.Rows)
            {
                row.ErrorText = string.Empty;
            }
        }

        // Puts each blocking problem against the row that caused it.
        private void ShowRowErrors(List<DataGridViewRow> rows, CaptureResult result)
        {
            foreach (RowError error in result.Errors)
            {
                if (error.RowIndex >= 0 && error.RowIndex < rows.Count)
                {
                    rows[error.RowIndex].ErrorText = error.Message;
                }
            }

            lblStatus.Text = string.Format(
                "{0} row(s) need fixing before this day can be saved", result.Errors.Count);
        }

        // Marks written rows so the manager can check them against the paper register,
        // and keeps their new identity so saving again updates instead of duplicating.
        private void MarkSaved(List<DataGridViewRow> rows, List<Timesheet> saved)
        {
            for (int index = 0; index < rows.Count && index < saved.Count; index++)
            {
                DataGridViewRow row = rows[index];
                var state = EnsureState(row);

                state.IsSaved = true;
                state.ExistingTimesheetID = saved[index].TimesheetID;

                row.DefaultCellStyle.BackColor = SavedRowColour;
                row.HeaderCell.Value = "S";
                row.HeaderCell.ToolTipText = "Saved";
            }
        }

        // What the form remembers about a row beyond the values in its cells.
        private class CaptureRowState
        {
            public int? ExistingTimesheetID { get; set; }
            public bool IsSaved { get; set; }
        }
    }
}
