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

        // Every value the editor held when it opened, so Cancel can tell typing from an untouched form.
        private List<string> editorStateOnOpen = new List<string>();

        public EmployeesForm()
        {
            InitializeComponent();
            employeeController = new EmployeeController();
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ShowEditorForNewEmployee();
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
