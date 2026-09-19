using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // Asks which stored Timesheet to update when an employee has more than one for the
    // same WorkDate. Rare enough that a picker beats inventing a rule.
    public class TimesheetPickerForm : Form
    {
        private readonly List<Timesheet> timesheets;
        private readonly ListBox lstTimesheets;

        // Lays out the picker and lists each candidate Timesheet.
        public TimesheetPickerForm(List<Timesheet> timesheets)
        {
            this.timesheets = timesheets;

            Text = "Which timesheet?";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(420, 220);

            var lblPrompt = new Label();
            lblPrompt.AutoSize = false;
            lblPrompt.Location = new Point(12, 9);
            lblPrompt.Size = new Size(396, 32);
            lblPrompt.Text = "This employee already has more than one timesheet for this date. Choose the one to update.";

            lstTimesheets = new ListBox();
            lstTimesheets.Location = new Point(12, 45);
            lstTimesheets.Size = new Size(396, 121);
            lstTimesheets.IntegralHeight = false;

            foreach (Timesheet timesheet in timesheets)
            {
                lstTimesheets.Items.Add(Describe(timesheet));
            }

            if (lstTimesheets.Items.Count > 0)
            {
                lstTimesheets.SelectedIndex = 0;
            }

            var btnOk = new Button();
            btnOk.Text = "Update";
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(252, 180);
            btnOk.Size = new Size(75, 25);

            var btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(333, 180);
            btnCancel.Size = new Size(75, 25);

            Controls.Add(lblPrompt);
            Controls.Add(lstTimesheets);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        // The Timesheet the manager chose, or null if they cancelled.
        public Timesheet SelectedTimesheet
        {
            get
            {
                int index = lstTimesheets.SelectedIndex;
                if (index < 0 || index >= timesheets.Count)
                {
                    return null;
                }

                return timesheets[index];
            }
        }

        // Describes one candidate well enough to tell them apart.
        private static string Describe(Timesheet timesheet)
        {
            string notes = string.Empty;
            if (!string.IsNullOrWhiteSpace(timesheet.Notes))
            {
                notes = "  |  " + timesheet.Notes;
            }

            return string.Format(
                "#{0}  {1}  ({2}){3}",
                timesheet.TimesheetID,
                timesheet.DescribeWhen(),
                timesheet.DayType.ToDatabaseValue(),
                notes);
        }
    }
}
