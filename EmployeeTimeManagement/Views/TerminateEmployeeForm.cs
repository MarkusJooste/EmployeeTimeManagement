using System;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // Asks for the end date and reason Terminate writes to the employee's contract. There
    // is no fixed list of reasons, so the reason is free text rather than a dropdown.
    public class TerminateEmployeeForm : Form
    {
        private readonly DateTimePicker dtpEndDate;
        private readonly TextBox txtReason;

        public TerminateEmployeeForm(string employeeName)
        {
            Text = "Terminate employee";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(360, 210);

            var lblPrompt = new Label();
            lblPrompt.AutoSize = false;
            lblPrompt.Location = new Point(12, 9);
            lblPrompt.Size = new Size(336, 32);
            lblPrompt.Text = "Ending " + employeeName + "'s employment. When did they leave, and why?";

            var lblEndDate = new Label();
            lblEndDate.AutoSize = true;
            lblEndDate.Location = new Point(12, 54);
            lblEndDate.Text = "End date";

            dtpEndDate = new DateTimePicker();
            dtpEndDate.Format = DateTimePickerFormat.Short;
            dtpEndDate.Location = new Point(100, 50);
            dtpEndDate.Size = new Size(248, 20);
            dtpEndDate.Value = DateTime.Today;

            var lblReason = new Label();
            lblReason.AutoSize = true;
            lblReason.Location = new Point(12, 84);
            lblReason.Text = "Reason";

            txtReason = new TextBox();
            txtReason.Location = new Point(100, 81);
            txtReason.Size = new Size(248, 60);
            txtReason.Multiline = true;

            var btnOk = new Button();
            btnOk.Text = "Terminate";
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(192, 170);
            btnOk.Size = new Size(75, 25);

            var btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(273, 170);
            btnCancel.Size = new Size(75, 25);

            Controls.Add(lblPrompt);
            Controls.Add(lblEndDate);
            Controls.Add(dtpEndDate);
            Controls.Add(lblReason);
            Controls.Add(txtReason);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        // The end date the manager chose, defaulting to today.
        public DateTime EndDate
        {
            get { return dtpEndDate.Value.Date; }
        }

        // The free-text reason the manager typed, trimmed.
        public string Reason
        {
            get { return txtReason.Text.Trim(); }
        }
    }
}
