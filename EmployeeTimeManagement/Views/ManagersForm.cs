using EmployeeTimeManagement.Controllers;
using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // Lists who could have a login and who does, for an Owner to Promote or Demote. Holds no
    // rules of its own: both lists and every refusal come from ManagerAccess, matching
    // EmployeesForm's list-and-buttons shape. Demote gains behaviour in a later ticket.
    public partial class ManagersForm : Form
    {
        private const string MaskedPin = "****";

        private readonly EmployeeController employeeController;
        private readonly ManagerController managerController;

        // The ManagerID whose PIN is currently shown in the clear, or null while every row is
        // masked. Only ever one at a time, so answering "what is Jane's PIN" never puts every
        // login on screen.
        private int? revealedManagerID;

        public ManagersForm()
        {
            InitializeComponent();

            // The sidebar button is already hidden from a Manager, but that hides a door, not
            // the room behind it: this view is refused to anyone who reaches it another way.
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("Only an Owner can open Managers.", "Not permitted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Load += (sender, e) => Close();
                return;
            }

            employeeController = new EmployeeController();
            managerController = new ManagerController();

            LoadData();
        }

        private void LoadData()
        {
            revealedManagerID = null;

            if (CurrentUser.StoreID == null)
            {
                BindLists(new List<Employee>(), new List<ManagerListItem>());
                return;
            }

            int storeID = CurrentUser.StoreID.Value;

            List<Employee> employees;
            List<Manager> managers;

            try
            {
                employees = employeeController.GetByStore(storeID);
                managers = managerController.GetByStore(storeID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load Managers: " + ex.Message, "Managers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                BindLists(new List<Employee>(), new List<ManagerListItem>());
                return;
            }

            List<Employee> promotable = ManagerAccess.Promotable(employees, managers, storeID).ToList();
            List<ManagerListItem> promoted = BuildManagerListItems(ManagerAccess.Demotable(managers, storeID), employees, managers);

            BindLists(promotable, promoted);
        }

        // Resolves each Demotable row's display fields before binding, then sorts by surname
        // then name to match the Employees view: the in-memory join that follows breaks the
        // order the store-scoped query returned rows in.
        private static List<ManagerListItem> BuildManagerListItems(IEnumerable<Manager> demotable, List<Employee> employees, List<Manager> managers)
        {
            Dictionary<int, Employee> employeeByID = employees.ToDictionary(employee => employee.EmployeeID);
            Dictionary<int, Manager> managerByID = managers.ToDictionary(manager => manager.ManagerID);

            return demotable
                .Select(manager => BuildManagerListItem(manager, employeeByID, managerByID))
                .OrderBy(item => item.Surname, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(item => item.Name, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }

        // A Manager row carries no name of its own worth showing: Promote never sets ManagerName,
        // so the Employee it is linked to supplies Name and Surname, and CapturedBy is resolved
        // back to the User who last promoted or demoted this row.
        private static ManagerListItem BuildManagerListItem(Manager manager, Dictionary<int, Employee> employeeByID, Dictionary<int, Manager> managerByID)
        {
            Employee employee;
            employeeByID.TryGetValue(manager.EmployeeID ?? 0, out employee);

            Manager promotedBy = null;
            if (manager.CapturedBy.HasValue)
            {
                managerByID.TryGetValue(manager.CapturedBy.Value, out promotedBy);
            }

            return new ManagerListItem
            {
                ManagerID = manager.ManagerID,
                Name = employee == null ? string.Empty : employee.Name,
                Surname = employee == null ? string.Empty : employee.Surname,
                Pin = manager.Pin,
                PromotedOn = manager.BusinessDate,
                PromotedByName = promotedBy == null ? string.Empty : promotedBy.ManagerName
            };
        }

        private void BindLists(List<Employee> promotable, List<ManagerListItem> promoted)
        {
            dgvEmployees.DataSource = promotable;
            dgvManagers.DataSource = promoted;

            lblEmployeesEmpty.Visible = promotable.Count == 0;
            dgvEmployees.Visible = promotable.Count > 0;

            lblManagersEmpty.Visible = promoted.Count == 0;
            dgvManagers.Visible = promoted.Count > 0;

            UpdateButtonState();
        }

        // Promote and Demote act on one row, so they stay disabled while no row is selected in
        // the list they each act on.
        private void UpdateButtonState()
        {
            btnPromote.Enabled = dgvEmployees.Visible && dgvEmployees.CurrentRow != null;
            btnDemote.Enabled = dgvManagers.Visible && dgvManagers.CurrentRow != null;
        }

        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonState();
        }

        private void dgvManagers_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonState();
        }

        // Masks every PIN except the one row currently revealed.
        private void dgvManagers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvManagers.Columns[e.ColumnIndex].Name != colPin.Name)
            {
                return;
            }

            var row = dgvManagers.Rows[e.RowIndex].DataBoundItem as ManagerListItem;
            if (row == null)
            {
                return;
            }

            e.Value = revealedManagerID.HasValue && revealedManagerID.Value == row.ManagerID ? row.Pin : MaskedPin;
            e.FormattingApplied = true;
        }

        // Toggles one row's reveal: clicking the currently-revealed row's button masks it again;
        // clicking any other row's button reveals that one and masks every other row.
        private void dgvManagers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvManagers.Columns[e.ColumnIndex].Name != colReveal.Name)
            {
                return;
            }

            var row = dgvManagers.Rows[e.RowIndex].DataBoundItem as ManagerListItem;
            if (row == null)
            {
                return;
            }

            revealedManagerID = revealedManagerID.HasValue && revealedManagerID.Value == row.ManagerID
                ? (int?)null
                : row.ManagerID;

            dgvManagers.Refresh();
        }

        // Asks for a PIN, typed twice, then hands the decision to the seam. Nothing is written
        // until the seam accepts it, and every refusal is shown exactly as the seam gives it.
        private void btnPromote_Click(object sender, EventArgs e)
        {
            var selected = dgvEmployees.CurrentRow?.DataBoundItem as Employee;

            if (selected == null || CurrentUser.StoreID == null || CurrentUser.ManagerID == null)
            {
                return;
            }

            using (var dialog = new PromoteManagerForm(selected.FullName))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                List<Manager> allManagers;

                try
                {
                    allManagers = managerController.GetAll();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not check existing PINs: " + ex.Message, "Promote", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Manager demotedRow = allManagers.FirstOrDefault(manager => manager.EmployeeID == selected.EmployeeID);

                var request = new PromotionRequest
                {
                    EmployeeID = selected.EmployeeID,
                    EmployeeStoreID = selected.StoreID,
                    OwnerStoreID = CurrentUser.StoreID.Value,
                    ContractEndDate = selected.ContractEndDate,
                    Pin = dialog.Pin,
                    PinConfirmation = dialog.PinConfirmation,
                    ExistingManagers = allManagers,
                    ExistingManagerRow = demotedRow,
                    CapturedBy = CurrentUser.ManagerID.Value,
                    BusinessDate = DateTime.Today
                };

                PromotionResult result = ManagerAccess.Promote(request);

                if (!result.IsValid)
                {
                    MessageBox.Show(result.Error, "Promote", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    managerController.Promote(result.Manager);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not promote " + selected.FullName + ": " + ex.Message, "Promote", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoadData();

                MessageBox.Show("Promoted " + selected.FullName + " to manager.", "Promote", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Ticket 06 gives this a rule.
        private void btnDemote_Click(object sender, EventArgs e)
        {
        }
    }
}
