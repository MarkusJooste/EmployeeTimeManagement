using System;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // Asks for the new Manager's PIN, typed twice. The digits are shown as typed rather than
    // masked: the Owner has to read the PIN back to the Manager standing in front of them, so
    // hiding four digits they are about to say out loud is friction for nothing.
    public class PromoteManagerForm : Form
    {
        private readonly TextBox txtPin;
        private readonly TextBox txtPinConfirmation;

        public PromoteManagerForm(string employeeName)
        {
            Text = "Promote to manager";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(360, 170);

            var lblPrompt = new Label();
            lblPrompt.AutoSize = false;
            lblPrompt.Location = new Point(12, 9);
            lblPrompt.Size = new Size(336, 32);
            lblPrompt.Text = "Choose a PIN for " + employeeName + ". Type it twice.";

            var lblPin = new Label();
            lblPin.AutoSize = true;
            lblPin.Location = new Point(12, 54);
            lblPin.Text = "PIN";

            txtPin = new TextBox();
            txtPin.Location = new Point(100, 50);
            txtPin.Size = new Size(80, 20);
            txtPin.MaxLength = 4;

            var lblPinConfirmation = new Label();
            lblPinConfirmation.AutoSize = true;
            lblPinConfirmation.Location = new Point(12, 84);
            lblPinConfirmation.Text = "Confirm PIN";

            txtPinConfirmation = new TextBox();
            txtPinConfirmation.Location = new Point(100, 80);
            txtPinConfirmation.Size = new Size(80, 20);
            txtPinConfirmation.MaxLength = 4;

            var btnOk = new Button();
            btnOk.Text = "Promote";
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(192, 130);
            btnOk.Size = new Size(75, 25);

            var btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(273, 130);
            btnCancel.Size = new Size(75, 25);

            Controls.Add(lblPrompt);
            Controls.Add(lblPin);
            Controls.Add(txtPin);
            Controls.Add(lblPinConfirmation);
            Controls.Add(txtPinConfirmation);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        // The PIN the Owner typed, trimmed. Left to the seam to say whether it is valid.
        public string Pin
        {
            get { return txtPin.Text.Trim(); }
        }

        public string PinConfirmation
        {
            get { return txtPinConfirmation.Text.Trim(); }
        }
    }
}
