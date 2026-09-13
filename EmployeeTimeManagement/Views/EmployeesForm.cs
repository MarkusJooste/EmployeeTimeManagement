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
    public partial class EmployeesForm : Form
    {
        private readonly EmployeeController employeeController;
        private List<EmployeeListItem> allEmployees = new List<EmployeeListItem>();

        // Every field the capture seam can refuse, by the name it refuses them under.
        private readonly Dictionary<string, EditorField> editorFields;

        // Pale red, so a field that stopped the save is obvious without shouting at the manager.
        private static readonly Color ErrorFieldColour = Color.FromArgb(255, 235, 238);

        // Every value the editor held when it opened, so Cancel can tell typing from an untouched form.
        private List<string> editorStateOnOpen = new List<string>();

        public EmployeesForm()
        {
            InitializeComponent();
            employeeController = new EmployeeController();
            editorFields = BuildEditorFields();
            cboMaritalStatus.Items.AddRange(MaritalStatuses.All());
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

        // Swaps the list away and opens the editor on a blank form for a new starter
        private void ShowEditorForNewEmployee()
        {
            lblEditorTitle.Text = "Add Employee";
            ClearEditor();
            editorStateOnOpen = DescribeEditorState();

            pnlList.Visible = false;
            pnlEditor.Visible = true;
            txtName.Focus();
        }

        // Swaps the editor away and returns to the list, which is left exactly as the manager had it
        private void ShowList()
        {
            pnlEditor.Visible = false;
            pnlList.Visible = true;
        }

        // Empties every field of the editor, leaving marital status unset and one blank family row to type into
        private void ClearEditor()
        {
            WalkInputs(
                pnlEditor,
                textBox => textBox.Text = string.Empty,
                comboBox => comboBox.SelectedIndex = -1,
                picker => picker.Value = DateTime.Today);

            dgvFamily.Rows.Clear();
            dgvFamily.Rows.Add();

            UpdateSpouseFields();
            ClearFieldErrors();
        }

        // Everything the editor currently holds, in control order, so it can be compared against the state it opened with.
        // The comparison is positional, so it is the walk that decides which fields count, not a list of them kept by hand.
        private List<string> DescribeEditorState()
        {
            // A cell still being typed into has not reached its row yet, so it would otherwise be missed.
            dgvFamily.EndEdit();

            var state = new List<string>();

            WalkInputs(
                pnlEditor,
                textBox => state.Add(textBox.Text),
                comboBox => state.Add(comboBox.SelectedIndex.ToString()),
                picker => state.Add(picker.Value.Date.ToString("yyyy-MM-dd")));

            foreach (DataGridViewRow row in dgvFamily.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    state.Add(cell.Value == null ? string.Empty : cell.Value.ToString());
                }
            }

            return state;
        }

        // Visits every input of one container and everything nested inside it, so that clearing the editor and
        // reading it back can never disagree about which controls a field added later counts among
        private static void WalkInputs(
            Control parent,
            Action<TextBox> onTextBox,
            Action<ComboBox> onComboBox,
            Action<DateTimePicker> onDatePicker)
        {
            foreach (Control child in parent.Controls)
            {
                TextBox textBox = child as TextBox;
                if (textBox != null)
                {
                    onTextBox(textBox);
                }

                ComboBox comboBox = child as ComboBox;
                if (comboBox != null)
                {
                    onComboBox(comboBox);
                }

                DateTimePicker picker = child as DateTimePicker;
                if (picker != null)
                {
                    onDatePicker(picker);
                }

                WalkInputs(child, onTextBox, onComboBox, onDatePicker);
            }
        }

        // Asks before throwing typing away, and says nothing when there is nothing to lose
        private bool ConfirmDiscard()
        {
            if (DescribeEditorState().SequenceEqual(editorStateOnOpen))
            {
                return true;
            }

            DialogResult result = MessageBox.Show(
                "The details captured on this screen have not been saved. Leaving the editor will discard them.",
                "Discard unsaved details?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            return result == DialogResult.Yes;
        }

        // Only a married employee has a spouse, so the two fields are open to typing then and cleared whenever they are not
        private void UpdateSpouseFields()
        {
            bool isMarried = MaritalStatuses.Married.Equals(cboMaritalStatus.SelectedItem as string);

            if (!isMarried)
            {
                txtSpouseName.Text = string.Empty;
                txtSpouseMobileNumber.Text = string.Empty;
            }

            txtSpouseName.Enabled = isMarried;
            txtSpouseMobileNumber.Enabled = isMarried;
        }

        // Validates the whole editor in one pass, checks nobody already holds the ID number,
        // and writes every table at once. Nothing is written until all of that passes.
        private void Save()
        {
            if (CurrentUser.StoreID == null || CurrentUser.ManagerID == null)
            {
                ShowRefusal("No manager is signed in");
                return;
            }

            // A cell still being typed into has not reached its row yet, so it would otherwise be missed.
            dgvFamily.EndEdit();
            ClearFieldErrors();

            EmployeeCaptureResult result = EmployeeCapture.Build(
                CurrentUser.StoreID.Value, CurrentUser.ManagerID.Value, ReadEditor());

            if (!result.IsValid)
            {
                ShowFieldErrors(result.Errors);
                return;
            }

            EmployeeIDNumberOwner owner;

            try
            {
                owner = employeeController.FindByIDNumber(result.Record.Employee.IDNumber);
            }
            catch (Exception ex)
            {
                ShowRefusal("Could not check this ID number: " + ex.Message);
                return;
            }

            EmployeeFieldError clash = EmployeeCapture.DescribeIDNumberClash(owner, CurrentUser.StoreID.Value);

            if (clash != null)
            {
                ShowFieldErrors(new List<EmployeeFieldError> { clash });
                return;
            }

            try
            {
                employeeController.Insert(result.Record);
            }
            catch (Exception ex)
            {
                ShowRefusal("Could not save this employee: " + ex.Message);
                return;
            }

            ShowList();

            // The search that was narrowing the list before is almost certainly the hunt that
            // ended in Add Employee, and it would hide the person just captured.
            txtSearch.Text = string.Empty;

            // Re-read rather than added to the list in memory, so the manager sees what was stored.
            LoadData();
            lblStatus.Text = "Saved " + result.Record.Employee.FullName;
        }

        // Reads every control of the editor into the raw shape the capture seam validates.
        private EmployeeCaptureInput ReadEditor()
        {
            var input = new EmployeeCaptureInput
            {
                Name = txtName.Text,
                Surname = txtSurname.Text,
                IDNumber = txtIDNumber.Text,
                SARSNumber = txtSARSNumber.Text,
                MobileNumber = txtMobileNumber.Text,
                MaritalStatus = cboMaritalStatus.SelectedItem as string,
                NumberOfDependents = txtDependents.Text,

                HouseFlatNumber = txtHouseFlatNumber.Text,
                ComplexFlatNumber = txtComplexFlatNumber.Text,
                StreetName = txtStreetName.Text,
                Town = txtTown.Text,
                PostalCode = txtPostalCode.Text,

                BankName = txtBankName.Text,
                AccountType = txtAccountType.Text,
                AccountNumber = txtAccountNumber.Text,
                BranchCode = txtBranchCode.Text,

                ContractType = txtContractType.Text,
                StartDate = dtpStartDate.Value.Date,
                Department = txtDepartment.Text,
                JobDescription = txtJobDescription.Text,
                HourlyRate = txtHourlyRate.Text,

                SpouseName = txtSpouseName.Text,
                SpouseMobileNumber = txtSpouseMobileNumber.Text
            };

            for (int index = 0; index < dgvFamily.Rows.Count; index++)
            {
                DataGridViewRow row = dgvFamily.Rows[index];

                input.FamilyMembers.Add(new FamilyMemberInput
                {
                    RowIndex = index,
                    Name = CellText(row, colFamilyName.Index),
                    MobileNumber = CellText(row, colFamilyMobileNumber.Index),
                    Relationship = CellText(row, colFamilyRelationship.Index)
                });
            }

            return input;
        }

        // Reads one family grid cell as text, treating a cell never typed into as empty.
        private static string CellText(DataGridViewRow row, int columnIndex)
        {
            object value = row.Cells[columnIndex].Value;
            return value == null ? string.Empty : value.ToString();
        }

        // Ties each field name the capture seam reports to the control it was typed into and
        // the words the manager knows it by, so a refused save can point at the right box.
        private Dictionary<string, EditorField> BuildEditorFields()
        {
            return new Dictionary<string, EditorField>
            {
                { "Name", new EditorField(txtName, "Name") },
                { "Surname", new EditorField(txtSurname, "Surname") },
                { "IDNumber", new EditorField(txtIDNumber, "ID number") },
                { "SARSNumber", new EditorField(txtSARSNumber, "SARS number") },
                { "MobileNumber", new EditorField(txtMobileNumber, "Mobile number") },
                { "MaritalStatus", new EditorField(cboMaritalStatus, "Marital status") },
                { "NumberOfDependents", new EditorField(txtDependents, "Dependents") },

                { "HouseFlatNumber", new EditorField(txtHouseFlatNumber, "House/flat number") },
                { "ComplexFlatNumber", new EditorField(txtComplexFlatNumber, "Complex/flat number") },
                { "StreetName", new EditorField(txtStreetName, "Street name") },
                { "Town", new EditorField(txtTown, "Town") },
                { "PostalCode", new EditorField(txtPostalCode, "Postal code") },

                { "BankName", new EditorField(txtBankName, "Bank name") },
                { "AccountType", new EditorField(txtAccountType, "Account type") },
                { "AccountNumber", new EditorField(txtAccountNumber, "Account number") },
                { "BranchCode", new EditorField(txtBranchCode, "Branch code") },

                { "ContractType", new EditorField(txtContractType, "Contract type") },
                { "StartDate", new EditorField(dtpStartDate, "Start date") },
                { "Department", new EditorField(txtDepartment, "Department") },
                { "JobDescription", new EditorField(txtJobDescription, "Job description") },
                { "HourlyRate", new EditorField(txtHourlyRate, "Hourly rate") },

                { "SpouseName", new EditorField(txtSpouseName, "Spouse name") },
                { "SpouseMobileNumber", new EditorField(txtSpouseMobileNumber, "Spouse mobile number") },

                { "FamilyMembers", new EditorField(dgvFamily, "Family contacts") }
            };
        }

        // Puts every blocking problem in front of the manager at once: each field marked, its
        // reason on hover, and a summary naming them, so Save is not a game of one at a time.
        private void ShowFieldErrors(List<EmployeeFieldError> errors)
        {
            ClearFieldErrors();

            var named = new List<string>();
            int marked = 0;

            foreach (EmployeeFieldError error in errors)
            {
                string displayName = MarkField(error);

                if (displayName == null)
                {
                    continue;
                }

                // Two problems on one family row are two fields but one place to look, so the
                // count and the list of names do not have to agree.
                marked++;

                if (!named.Contains(displayName))
                {
                    named.Add(displayName);
                }
            }

            ShowRefusal(string.Format(
                "{0} field(s) need fixing before this employee can be saved: {1}",
                marked,
                string.Join(", ", named)));
        }

        // Marks the one field a problem belongs to and returns what to call it in the summary,
        // or null when the seam named something this editor has no control for.
        private string MarkField(EmployeeFieldError error)
        {
            if (error.FamilyRowIndex.HasValue)
            {
                return MarkFamilyCell(error);
            }

            EditorField field;

            if (!editorFields.TryGetValue(error.Field, out field))
            {
                return null;
            }

            Colour(field.Input, ErrorFieldColour);
            tipEditorErrors.SetToolTip(field.Input, error.Message);

            return field.DisplayName;
        }

        // Marks the one cell of the family grid a problem belongs to. The mark goes on the cell
        // rather than the row header, which this grid does not show.
        private string MarkFamilyCell(EmployeeFieldError error)
        {
            int rowIndex = error.FamilyRowIndex.Value;

            if (rowIndex < 0 || rowIndex >= dgvFamily.Rows.Count)
            {
                return null;
            }

            DataGridViewColumn column = error.Field == "Relationship" ? colFamilyRelationship : colFamilyName;
            dgvFamily.Rows[rowIndex].Cells[column.Index].ErrorText = error.Message;

            return "Family contact " + (rowIndex + 1);
        }

        // Clears the marks a previous refused save left behind, so only current problems show.
        private void ClearFieldErrors()
        {
            foreach (EditorField field in editorFields.Values)
            {
                Colour(field.Input, SystemColors.Window);
                tipEditorErrors.SetToolTip(field.Input, string.Empty);
            }

            foreach (DataGridViewRow row in dgvFamily.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.ErrorText = string.Empty;
                }
            }

            lblEditorStatus.Text = string.Empty;
        }

        // Backs a field with a colour, for the kinds of input that show one. A date picker
        // ignores its BackColor, so the two that do not are left alone rather than lied to.
        private static void Colour(Control input, Color colour)
        {
            if (input is TextBox || input is ComboBox)
            {
                input.BackColor = colour;
            }
        }

        // Says why the employee was not saved. A save that works leaves for the list instead,
        // so everything this label ever shows is a refusal.
        private void ShowRefusal(string message)
        {
            lblEditorStatus.Text = message;
        }

        // A field of the editor as a refused save needs it: the control to mark, and what the manager calls it.
        private class EditorField
        {
            // Pairs a control with the words the manager knows it by.
            public EditorField(Control input, string displayName)
            {
                Input = input;
                DisplayName = displayName;
            }

            public Control Input { get; private set; }
            public string DisplayName { get; private set; }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ShowEditorForNewEmployee();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscard())
            {
                return;
            }

            ShowList();
        }

        private void cboMaritalStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSpouseFields();
        }

        private void btnAddFamilyRow_Click(object sender, EventArgs e)
        {
            dgvFamily.Rows.Add();
        }

        private void btnRemoveFamilyRow_Click(object sender, EventArgs e)
        {
            if (dgvFamily.CurrentRow == null)
            {
                return;
            }

            dgvFamily.Rows.Remove(dgvFamily.CurrentRow);
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
