using System;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // Asks for the last working day and the reason, then asks once, in the same dialog,
    // whether to end the named employee's contract on that date - so Terminate takes one
    // confirmation rather than a second Yes/No after this one. There is no fixed list of
    // reasons, so the reason is free text, and an empty one is refused without closing.
    public class TerminateEmployeeForm : Form
    {
        private readonly string employeeName;
        private readonly DateTimePicker dtpEndDate;
        private readonly TextBox txtReason;
        private readonly Label lblReasonError;
        private readonly Label lblConfirm;
        private readonly Button btnCancel;
        private readonly Button btnTerminate;

        public TerminateEmployeeForm(string employeeName)
        {
            this.employeeName = employeeName;

            Text = "Terminate employee";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            BackColor = Theme.Cream;
            ClientSize = new Size(380, 268);

            var lblEndDateCaption = new Label();
            lblEndDateCaption.AutoSize = true;
            lblEndDateCaption.Location = new Point(16, 18);
            lblEndDateCaption.Text = "Last working day";
            lblEndDateCaption.Font = Theme.BodyFont;
            lblEndDateCaption.ForeColor = Theme.Ink;

            dtpEndDate = new DateTimePicker();
            dtpEndDate.Format = DateTimePickerFormat.Short;
            dtpEndDate.Location = new Point(16, 40);
            dtpEndDate.Size = new Size(348, 24);
            dtpEndDate.Value = DateTime.Today;
            dtpEndDate.ValueChanged += (sender, e) => UpdateConfirmText();

            var lblReasonCaption = new Label();
            lblReasonCaption.AutoSize = true;
            lblReasonCaption.Location = new Point(16, 76);
            lblReasonCaption.Text = "Reason";
            lblReasonCaption.Font = Theme.BodyFont;
            lblReasonCaption.ForeColor = Theme.Ink;

            txtReason = new TextBox();
            txtReason.Location = new Point(16, 98);
            txtReason.Size = new Size(348, 60);
            txtReason.Multiline = true;
            txtReason.Font = Theme.BodyFont;
            txtReason.TextChanged += (sender, e) => ClearReasonError();

            lblReasonError = new Label();
            lblReasonError.AutoSize = false;
            lblReasonError.Location = new Point(16, 160);
            lblReasonError.Size = new Size(348, 18);
            lblReasonError.Text = "A reason is needed to end this employment.";
            lblReasonError.Font = Theme.SmallFont;
            lblReasonError.ForeColor = Theme.ErrorText;
            lblReasonError.Visible = false;

            lblConfirm = new Label();
            lblConfirm.AutoSize = false;
            lblConfirm.Location = new Point(16, 182);
            lblConfirm.Size = new Size(348, 40);
            lblConfirm.Font = Theme.BodyFont;
            lblConfirm.ForeColor = Theme.Ink;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(116, 228);
            btnCancel.Size = new Size(90, 32);

            btnTerminate = new Button();
            btnTerminate.Text = "Terminate employee";
            btnTerminate.Location = new Point(214, 228);
            btnTerminate.Size = new Size(150, 32);
            btnTerminate.Click += btnTerminate_Click;

            Controls.Add(lblEndDateCaption);
            Controls.Add(dtpEndDate);
            Controls.Add(lblReasonCaption);
            Controls.Add(txtReason);
            Controls.Add(lblReasonError);
            Controls.Add(lblConfirm);
            Controls.Add(btnCancel);
            Controls.Add(btnTerminate);

            Theme.StyleButton(btnCancel, ButtonRole.Ghost);
            Theme.StyleButton(btnTerminate, ButtonRole.DangerSolid);

            AcceptButton = btnTerminate;
            CancelButton = btnCancel;

            UpdateConfirmText();
            Theme.FocusSafeChoice(this, btnCancel);
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

        // Names who is leaving and when, kept in step with the date picker.
        private void UpdateConfirmText()
        {
            lblConfirm.Text = "End " + employeeName + "'s employment on " + dtpEndDate.Value.Date.ToString("yyyy-MM-dd") + "?";
        }

        private void ClearReasonError()
        {
            lblReasonError.Visible = false;
            txtReason.BackColor = SystemColors.Window;
        }

        private void ShowReasonError()
        {
            lblReasonError.Visible = true;
            txtReason.BackColor = Theme.ErrorField;
            txtReason.Focus();
        }

        // Refuses an empty reason in place rather than closing, since a reason is set by the
        // DialogResult only once one has been typed.
        private void btnTerminate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReason.Text))
            {
                ShowReasonError();
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
