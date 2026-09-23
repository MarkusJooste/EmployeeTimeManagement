using EmployeeTimeManagement.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // The right-hand side of the split Employees list: one selected employee's identity,
    // actions and facts, or an empty state when nobody is selected. It shows one list item
    // it is given and nothing more - no data access and no validation - so the Employees
    // view stays the one place that knows how to load or change an employee.
    public partial class EmployeeDetailsPanel : UserControl
    {
        public EmployeeDetailsPanel()
        {
            InitializeComponent();
            ApplyTheme();
            ShowEmployee(null);
        }

        // Fired when Update details, Terminate or Reactivate is clicked. What each means is
        // the caller's business; this control only knows when one was clicked and whether
        // that made sense for the employee it is showing.
        public event EventHandler UpdateClicked;
        public event EventHandler TerminateClicked;
        public event EventHandler ReactivateClicked;

        // Whether Update details is enabled right now, so a double-click or Enter elsewhere
        // can be refused the same way the button itself refuses it.
        internal bool UpdateEnabled
        {
            get { return btnUpdate.Enabled; }
        }

        // Dresses the panel in the theme: a cream page, a dark Update button and outlined
        // Terminate/Reactivate buttons, matching the roles the same three actions use
        // everywhere else in this view.
        private void ApplyTheme()
        {
            BackColor = Theme.Cream;
            pnlIdentity.BackColor = Theme.Cream;
            pnlIdentityDetails.BackColor = Theme.Cream;
            pnlActions.BackColor = Theme.Cream;
            pnlFacts.BackColor = Theme.Cream;

            lblEmptyMessage.Font = Theme.HeadingFont;
            lblEmptyMessage.ForeColor = Theme.InkMuted;

            lblName.Font = Theme.TitleFont;
            lblName.ForeColor = Theme.Ink;
            lblJobDepartment.Font = Theme.BodyFont;
            lblJobDepartment.ForeColor = Theme.InkMuted;
            lblStatusTagText.Font = Theme.SmallFont;

            Theme.PrefixIcon(btnUpdate, "\u270E");
            Theme.PrefixIcon(btnTerminate, "\u2716");
            Theme.PrefixIcon(btnReactivate, "\u21BB");

            Theme.StyleButton(btnUpdate, ButtonRole.Dark);
            Theme.StyleButton(btnTerminate, ButtonRole.Danger);
            Theme.StyleButton(btnReactivate, ButtonRole.Ok);

            factIDNumber.Caption = "ID number";
            factMobile.Caption = "Mobile";
            factStartDate.Caption = "Start date";
            factJob.Caption = "Job";
            factDepartment.Caption = "Department";
            factContract.Caption = "Contract";
        }

        // Shows one employee's details, or the empty state when none is given. The action
        // buttons' enable rules live here too, worked out from the same employee the rest
        // of the panel draws from.
        internal void ShowEmployee(EmployeeListItem selected)
        {
            bool hasSelection = selected != null;

            pnlIdentityDetails.Visible = hasSelection;
            lblEmptyMessage.Visible = !hasSelection;
            pnlFacts.Visible = hasSelection;

            btnUpdate.Enabled = hasSelection;
            btnTerminate.Enabled = hasSelection && selected.IsActive;
            btnReactivate.Enabled = hasSelection && !selected.IsActive;

            if (!hasSelection)
            {
                return;
            }

            lblName.Text = selected.FullName;
            lblJobDepartment.Text = DescribeJobAndDepartment(selected);
            ShowStatusTag(selected);

            pnlInitials.Tag = selected;
            pnlInitials.Invalidate();

            factIDNumber.Value = Fact(selected.IDNumber);
            factMobile.Value = Fact(selected.MobileNumber);
            factStartDate.Value = FactDate(selected.ContractStartDate);
            factJob.Value = Fact(selected.JobDescription);
            factDepartment.Value = Fact(selected.Department);
            factContract.Value = DescribeContractEnding(selected);
        }

        // The job and department together, on one line, with whichever half is missing left out.
        private static string DescribeJobAndDepartment(EmployeeListItem selected)
        {
            string job = string.IsNullOrWhiteSpace(selected.JobDescription) ? null : selected.JobDescription.Trim();
            string department = string.IsNullOrWhiteSpace(selected.Department) ? null : selected.Department.Trim();

            if (job != null && department != null)
            {
                // The separator is written as an escape, so this file stays plain ASCII, the
                // way the icon glyphs above do.
                return job + "  \u00B7  " + department;
            }

            return job ?? department ?? "No job on record";
        }

        // A Former employee's contract fact box shows when and why it ended; an Active
        // employee's reads as open, so the box's shape does not jump between the two.
        private static string DescribeContractEnding(EmployeeListItem selected)
        {
            if (selected.IsActive)
            {
                return "Open";
            }

            string reason = string.IsNullOrWhiteSpace(selected.ReasonForEnding)
                ? "No reason on record"
                : selected.ReasonForEnding.Trim();

            return "Ended " + FactDate(selected.ContractEndDate) + "\r\n" + reason;
        }

        private static string FactDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "-";
        }

        // A missing fact reads as a dash rather than a blank box.
        private static string Fact(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        private void ShowStatusTag(EmployeeListItem selected)
        {
            lblStatusTagText.Text = selected.Status;

            Color colour = selected.IsActive ? Theme.Green : Theme.InkMuted;
            lblStatusTagText.ForeColor = colour;
            pnlStatusTag.Tag = colour;
            pnlStatusTag.Invalidate();
        }

        // Draws the large initials circle: the same shape as a picker row's, at a size that
        // fits the identity block a User confirms before acting.
        private void pnlInitials_Paint(object sender, PaintEventArgs e)
        {
            EmployeeListItem selected = pnlInitials.Tag as EmployeeListItem;

            if (selected == null)
            {
                return;
            }

            SmoothingMode was = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = new Rectangle(0, 0, pnlInitials.Width, pnlInitials.Height);

            using (var fill = new SolidBrush(selected.IsActive ? Theme.Charcoal : Theme.DisabledBack))
            {
                e.Graphics.FillEllipse(fill, bounds);
            }

            e.Graphics.SmoothingMode = was;

            TextRenderer.DrawText(
                e.Graphics,
                selected.Initials,
                Theme.TitleFont,
                bounds,
                selected.IsActive ? Theme.OnDark : Theme.DisabledFore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        // Draws the outline around the status tag, coloured the same way a picker row colours it.
        private void pnlStatusTag_Paint(object sender, PaintEventArgs e)
        {
            Color colour = pnlStatusTag.Tag is Color ? (Color)pnlStatusTag.Tag : Theme.InkMuted;

            using (var border = new Pen(colour))
            {
                e.Graphics.DrawRectangle(border, 0, 0, pnlStatusTag.Width - 1, pnlStatusTag.Height - 1);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (UpdateClicked != null)
            {
                UpdateClicked(this, EventArgs.Empty);
            }
        }

        private void btnTerminate_Click(object sender, EventArgs e)
        {
            if (TerminateClicked != null)
            {
                TerminateClicked(this, EventArgs.Empty);
            }
        }

        private void btnReactivate_Click(object sender, EventArgs e)
        {
            if (ReactivateClicked != null)
            {
                ReactivateClicked(this, EventArgs.Empty);
            }
        }
    }
}
