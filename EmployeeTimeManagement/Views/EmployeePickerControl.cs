using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // The employee list, its Active/Former/All filter tabs and its search box, extracted from
    // the Employees view so a second screen can reuse the same look and the same filtering
    // rules. The rules themselves live in EmployeeListFilter; this control only shows them.
    public partial class EmployeePickerControl : UserControl
    {
        // The initials circle, and the room the row leaves around it.
        private const int CircleSize = 34;
        private const int EdgePadding = 12;
        private const int SelectedBarWidth = 4;
        private const int TagHeight = 18;
        private const int TagPadding = 8;

        private List<EmployeeListItem> allEmployees = new List<EmployeeListItem>();
        private EmployeeScope scope = EmployeeScope.Active;

        // Rebinding the grid makes it select a row of its own accord, so the selection this
        // control reports is settled first and announced once, rather than at every step.
        private bool rebinding;

        // Who was reported last, within one set of loaded employees. The grid raises its own
        // selection event several times while it settles on a row, and a listener that
        // reloads an employee's history should not pay for each of them, so the same person
        // is never announced twice running. Cleared whenever fresh rows arrive.
        private int? reportedSelection;

        // Fired whenever the selected row changes.
        public event EventHandler SelectionChanged;

        // Fired whenever the visible rows change: a new employee list, a search keystroke, or a filter tab.
        public event EventHandler FilterChanged;

        // Fired when a User opens a row, by double-clicking it or pressing Enter on it.
        public event EventHandler EmployeeActivated;

        public EmployeePickerControl()
        {
            InitializeComponent();
            ApplyTheme();
            ApplyFilter();
        }

        // Dresses the picker in the theme: pale tabs over a cream strip, a white list, and
        // rows this control paints itself, so no colour is named outside the theme.
        private void ApplyTheme()
        {
            BackColor = Theme.Cream;
            pnlTop.BackColor = Theme.Cream;
            pnlTabs.BackColor = Theme.Cream;

            Theme.StyleTab(btnScopeActive);
            Theme.StyleTab(btnScopeFormer);
            Theme.StyleTab(btnScopeAll);

            // The box searches more than a name now, so the caption above it says so rather
            // than leaving a User to guess which details are worth typing.
            lblSearchHint.Font = Theme.SmallFont;
            lblSearchHint.ForeColor = Theme.InkMuted;

            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Font = Theme.BodyFont;
            txtSearch.BackColor = Theme.Surface;
            txtSearch.ForeColor = Theme.Ink;

            lblEmpty.Font = Theme.BodyFont;
            lblEmpty.ForeColor = Theme.InkMuted;
            lblEmpty.BackColor = Theme.Surface;

            dgvEmployees.BackgroundColor = Theme.Surface;
            dgvEmployees.GridColor = Theme.Border;
            dgvEmployees.DefaultCellStyle.BackColor = Theme.Surface;
            dgvEmployees.DefaultCellStyle.SelectionBackColor = Theme.Cream;
            dgvEmployees.DefaultCellStyle.SelectionForeColor = Theme.Ink;
        }

        // The rows currently on screen, after the filter tab and the search text narrow them.
        internal List<EmployeeListItem> VisibleEmployees { get; private set; } = new List<EmployeeListItem>();

        // The search text, exposed so a caller can clear it the way a completed save does.
        public string SearchText
        {
            get { return txtSearch.Text; }
            set { txtSearch.Text = value; }
        }

        // Which standing the list is showing. Opens on Active, so people who left years ago
        // do not clutter the daily work.
        internal EmployeeScope Scope
        {
            get { return scope; }

            set
            {
                if (scope == value)
                {
                    return;
                }

                scope = value;
                ApplyFilter();
            }
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

        // Replaces the full list of employees to choose from and reapplies the current filter tab and search.
        internal void SetEmployees(List<EmployeeListItem> employees)
        {
            allEmployees = employees ?? new List<EmployeeListItem>();

            // These are freshly read rows, so the same person is a different set of details
            // from the ones listeners were last told about, and has to be announced again.
            reportedSelection = null;

            ApplyFilter();
        }

        // Narrows the loaded employees by the filter tab and the search text without re-querying
        // the database, keeping the same person selected when they are still on screen and
        // clearing the selection when they are not, so nobody acts on a row they cannot see.
        private void ApplyFilter()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(allEmployees, scope, txtSearch.Text);

            VisibleEmployees = result.Visible;

            ShowCounts(result);
            ShowEmptyMessage();

            int? wasSelected = SelectedEmployeeID();

            rebinding = true;

            try
            {
                dgvEmployees.DataSource = new BindingList<EmployeeListItem>(VisibleEmployees);
                RestoreSelection(wasSelected);
            }
            finally
            {
                rebinding = false;
            }

            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }

            AnnounceSelection();
        }

        // Tells listeners who is selected, but only when that is somebody other than the
        // person they were last told about.
        private void AnnounceSelection()
        {
            int? selected = SelectedEmployeeID();

            if (selected == reportedSelection)
            {
                return;
            }

            reportedSelection = selected;

            if (SelectionChanged != null)
            {
                SelectionChanged(this, EventArgs.Empty);
            }
        }

        // Who is selected, as the identifier that survives a reload of the same people.
        private int? SelectedEmployeeID()
        {
            EmployeeListItem selected = SelectedEmployee;

            return selected == null ? (int?)null : selected.EmployeeID;
        }

        // Puts the selection back on the same employee after a rebind, or clears it when the
        // new filter or search no longer shows them.
        private void RestoreSelection(int? employeeID)
        {
            int row = employeeID == null ? -1 : VisibleEmployees.FindIndex(employee => employee.EmployeeID == employeeID.Value);

            if (row < 0)
            {
                dgvEmployees.CurrentCell = null;
                dgvEmployees.ClearSelection();
                return;
            }

            dgvEmployees.CurrentCell = dgvEmployees.Rows[row].Cells[0];
        }

        // Marks the chosen tab and gives each one the size of its group, counted before the
        // search narrowed anything so the counts describe the store.
        private void ShowCounts(EmployeeListFilterResult result)
        {
            btnScopeActive.Text = "Active (" + Written(result.ActiveCount) + ")";
            btnScopeFormer.Text = "Former (" + Written(result.FormerCount) + ")";
            btnScopeAll.Text = "All (" + Written(result.AllCount) + ")";

            Theme.SelectTab(btnScopeActive, scope == EmployeeScope.Active);
            Theme.SelectTab(btnScopeFormer, scope == EmployeeScope.Former);
            Theme.SelectTab(btnScopeAll, scope == EmployeeScope.All);
        }

        // A count as it is written on a tab, kept off the machine's own number formatting.
        private static string Written(int count)
        {
            return count.ToString(CultureInfo.InvariantCulture);
        }

        // Explains an empty list, so a User reads a result rather than wondering about a fault.
        // The message takes the grid's place rather than sitting over it, since two controls
        // filling the same space leave the second one nothing to be drawn in.
        private void ShowEmptyMessage()
        {
            bool empty = VisibleEmployees.Count == 0;

            lblEmpty.Text = empty ? DescribeEmptyList() : string.Empty;
            lblEmpty.Visible = empty;
            dgvEmployees.Visible = !empty;
        }

        // Names the reason the list is empty: a search that found nobody, or a group with
        // nobody in it.
        private string DescribeEmptyList()
        {
            string search = txtSearch.Text.Trim();

            if (search.Length > 0)
            {
                return "No employee matches \"" + search + "\".";
            }

            switch (scope)
            {
                case EmployeeScope.Active:
                    return "No active employees.";

                case EmployeeScope.Former:
                    return "No former employees.";

                default:
                    return "No employees to show.";
            }
        }

        // Draws one employee as a single row: an initials circle, the full name, their job and
        // start date, and the tag that says where they stand.
        private void dgvEmployees_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            EmployeeListItem employee = dgvEmployees.Rows[e.RowIndex].DataBoundItem as EmployeeListItem;

            if (employee == null)
            {
                return;
            }

            bool selected = (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected;

            DrawRow(e.Graphics, e.CellBounds, employee, selected);

            e.Handled = true;
        }

        // Paints the whole row. A Former employee is muted throughout, so current staff stand
        // out while the All tab is showing both.
        private static void DrawRow(Graphics graphics, Rectangle bounds, EmployeeListItem employee, bool selected)
        {
            bool active = employee.IsActive;

            using (var background = new SolidBrush(selected ? Theme.Cream : Theme.Surface))
            {
                graphics.FillRectangle(background, bounds);
            }

            using (var separator = new Pen(Theme.Border))
            {
                graphics.DrawLine(separator, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }

            if (selected)
            {
                using (var bar = new SolidBrush(Theme.Mustard))
                {
                    graphics.FillRectangle(bar, bounds.Left, bounds.Top, SelectedBarWidth, bounds.Height);
                }
            }

            var circle = new Rectangle(
                bounds.Left + EdgePadding + SelectedBarWidth,
                bounds.Top + ((bounds.Height - CircleSize) / 2),
                CircleSize,
                CircleSize);

            DrawInitials(graphics, circle, employee, active);

            Rectangle tag = DrawStandingTag(graphics, bounds, employee.Status, active);

            int textLeft = circle.Right + 10;
            int textWidth = tag.Left - 8 - textLeft;

            if (textWidth <= 0)
            {
                return;
            }

            const TextFormatFlags Flags = TextFormatFlags.Left
                | TextFormatFlags.VerticalCenter
                | TextFormatFlags.EndEllipsis
                | TextFormatFlags.NoPrefix;

            var nameBounds = new Rectangle(textLeft, bounds.Top + 8, textWidth, 19);
            var detailBounds = new Rectangle(textLeft, bounds.Top + 27, textWidth, 17);

            TextRenderer.DrawText(graphics, employee.FullName, Theme.ButtonFont, nameBounds, active ? Theme.Ink : Theme.InkMuted, Flags);
            TextRenderer.DrawText(graphics, DescribeRole(employee), Theme.SmallFont, detailBounds, Theme.InkMuted, Flags);
        }

        // Fills the initials circle. Anti-aliasing is turned on for the circle alone, since
        // the text around it reads more crisply without it.
        private static void DrawInitials(Graphics graphics, Rectangle circle, EmployeeListItem employee, bool active)
        {
            SmoothingMode was = graphics.SmoothingMode;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (var fill = new SolidBrush(active ? Theme.Charcoal : Theme.DisabledBack))
            {
                graphics.FillEllipse(fill, circle);
            }

            graphics.SmoothingMode = was;

            TextRenderer.DrawText(
                graphics,
                employee.Initials,
                Theme.ButtonFont,
                circle,
                active ? Theme.OnDark : Theme.DisabledFore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        // Draws the Active or Former tag against the row's right edge and reports the room it
        // took, so the name beside it can be cut rather than run underneath.
        private static Rectangle DrawStandingTag(Graphics graphics, Rectangle bounds, string standing, bool active)
        {
            Size text = TextRenderer.MeasureText(graphics, standing, Theme.SmallFont, Size.Empty, TextFormatFlags.NoPrefix);

            var tag = new Rectangle(
                bounds.Right - EdgePadding - text.Width - (TagPadding * 2),
                bounds.Top + ((bounds.Height - TagHeight) / 2),
                text.Width + (TagPadding * 2),
                TagHeight);

            Color colour = active ? Theme.Green : Theme.InkMuted;

            using (var border = new Pen(colour))
            {
                graphics.DrawRectangle(border, tag);
            }

            TextRenderer.DrawText(
                graphics,
                standing,
                Theme.SmallFont,
                tag,
                colour,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            return tag;
        }

        // The second line of a row: what the employee does, and since when.
        private static string DescribeRole(EmployeeListItem employee)
        {
            string job = string.IsNullOrWhiteSpace(employee.JobDescription)
                ? "No job on record"
                : employee.JobDescription.Trim();

            if (employee.ContractStartDate == null)
            {
                return job;
            }

            // The separator is written as an escape, so this file stays plain ASCII and is
            // beyond the reach of a code page, the way the icon glyphs elsewhere are.
            return job + "  \u00B7  since " + employee.ContractStartDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        // Opens the selected employee, the gesture the Employees view turns into Update.
        private void ActivateSelected()
        {
            if (SelectedEmployee == null || EmployeeActivated == null)
            {
                return;
            }

            EmployeeActivated(this, EventArgs.Empty);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void btnScopeActive_Click(object sender, EventArgs e)
        {
            Scope = EmployeeScope.Active;
        }

        private void btnScopeFormer_Click(object sender, EventArgs e)
        {
            Scope = EmployeeScope.Former;
        }

        private void btnScopeAll_Click(object sender, EventArgs e)
        {
            Scope = EmployeeScope.All;
        }

        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            if (rebinding)
            {
                return;
            }

            AnnounceSelection();
        }

        private void dgvEmployees_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            ActivateSelected();
        }

        // Enter opens the row the keyboard is on, instead of the grid's own move to the next row.
        private void dgvEmployees_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;

            ActivateSelected();
        }
    }
}
