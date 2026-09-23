using System;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // One themed Yes/No dialog for every confirmation this view asks besides Terminate, which
    // asks its own question alongside the fields it collects. The confirming button carries the
    // colour that names what confirming does; Cancel stays plain and starts with focus, so an
    // Enter pressed out of habit cancels rather than acts.
    public class ConfirmDialog : Form
    {
        private readonly Button btnCancel;
        private readonly Button btnConfirm;

        public ConfirmDialog(string title, string message, string confirmLabel, ButtonRole confirmRole)
        {
            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            BackColor = Theme.Cream;
            ClientSize = new Size(360, 156);

            var lblMessage = new Label();
            lblMessage.AutoSize = false;
            lblMessage.Location = new Point(16, 16);
            lblMessage.Size = new Size(328, 76);
            lblMessage.Text = message;
            lblMessage.Font = Theme.BodyFont;
            lblMessage.ForeColor = Theme.Ink;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(126, 108);
            btnCancel.Size = new Size(100, 32);

            btnConfirm = new Button();
            btnConfirm.Text = confirmLabel;
            btnConfirm.DialogResult = DialogResult.OK;
            btnConfirm.Location = new Point(234, 108);
            btnConfirm.Size = new Size(110, 32);

            Controls.Add(lblMessage);
            Controls.Add(btnCancel);
            Controls.Add(btnConfirm);

            Theme.StyleButton(btnCancel, ButtonRole.Ghost);
            Theme.StyleButton(btnConfirm, confirmRole);

            AcceptButton = btnConfirm;
            CancelButton = btnCancel;

            Theme.FocusSafeChoice(this, btnCancel);
        }

        // Shows a themed Yes/No dialog and reports whether the confirming button was chosen.
        public static bool Show(IWin32Window owner, string title, string message, string confirmLabel, ButtonRole confirmRole)
        {
            using (var dialog = new ConfirmDialog(title, message, confirmLabel, confirmRole))
            {
                return dialog.ShowDialog(owner) == DialogResult.OK;
            }
        }
    }
}
