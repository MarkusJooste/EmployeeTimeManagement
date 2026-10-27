using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // The employee list, its search box and its "Show former employees" toggle, extracted from
    // the Employees view so a second screen can reuse the same look and the same filtering rules.
    public partial class EmployeePickerControl : UserControl
    {
        private List<EmployeeListItem> allEmployees = new List<EmployeeListItem>();

        // Fired whenever the selected row changes.
        public event EventHandler SelectionChanged;

        // Fired whenever the visible rows change: a new employee list, a search keystroke, or the toggle.
        public event EventHandler FilterChanged;

        public EmployeePickerControl()
        {
            InitializeComponent();
        }

        // The rows currently on screen, after the search text and former-employee toggle narrow them.
        internal List<EmployeeListItem> VisibleEmployees { get; private set; } = new List<EmployeeListItem>();

        // The search text, exposed so a caller can clear it the way a completed save does.
        public string SearchText
        {
            get { return txtSearch.Text; }
            set { txtSearch.Text = value; }
        }

        // The employee whose row is selected, or null when nothing is selected.
        internal EmployeeListItem SelectedEmployee
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

        // Replaces the full list of employees to choose from and reapplies the current search and toggle state.
        internal void SetEmployees(List<EmployeeListItem> employees)
        {
            allEmployees = employees ?? new List<EmployeeListItem>();
            ApplyFilter();
        }

        // Narrows the loaded employees by the search text and the former-employee checkbox without re-querying the database.
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

            VisibleEmployees = matches.ToList();

            dgvEmployees.DataSource = new BindingList<EmployeeListItem>(VisibleEmployees);

            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
        }

        // Case-insensitive substring match that treats a missing value as no match.
        private static bool Contains(string value, string search)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return value.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) >= 0;
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
            if (SelectionChanged != null)
            {
                SelectionChanged(this, EventArgs.Empty);
            }
        }
    }
}
