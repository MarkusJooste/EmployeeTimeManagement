using EmployeeTimeManagement.Database;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // Asked once on a PC that has no database connection yet. Whoever installs the program
    // pastes the connection string; it is tried, then saved encrypted, and the store staff
    // never see or handle it.
    public class ConnectionSetupForm : Form
    {
        private readonly TextBox txtConnectionString;
        private readonly CheckBox chkShow;
        private readonly Label lblError;
        private readonly Button btnSave;

        public ConnectionSetupForm()
        {
            Text = "Database connection";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(460, 230);

            var lblPrompt = new Label();
            lblPrompt.AutoSize = false;
            lblPrompt.Location = new Point(12, 12);
            lblPrompt.Size = new Size(436, 48);
            lblPrompt.Text = "This PC is not connected to the database yet. Paste the connection string. "
                + "It is saved encrypted for this PC only and is not shown again.";

            txtConnectionString = new TextBox();
            txtConnectionString.Location = new Point(12, 66);
            txtConnectionString.Size = new Size(436, 20);
            txtConnectionString.UseSystemPasswordChar = true;

            chkShow = new CheckBox();
            chkShow.AutoSize = true;
            chkShow.Location = new Point(12, 94);
            chkShow.Text = "Show what I typed";
            chkShow.CheckedChanged += (sender, e) => txtConnectionString.UseSystemPasswordChar = !chkShow.Checked;

            lblError = new Label();
            lblError.AutoSize = false;
            lblError.Location = new Point(12, 120);
            lblError.Size = new Size(436, 60);
            lblError.ForeColor = Color.Firebrick;

            btnSave = new Button();
            btnSave.Text = "Test and save";
            btnSave.Location = new Point(262, 190);
            btnSave.Size = new Size(105, 28);
            btnSave.Click += btnSave_Click;

            var btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(373, 190);
            btnCancel.Size = new Size(75, 28);

            Controls.Add(lblPrompt);
            Controls.Add(txtConnectionString);
            Controls.Add(chkShow);
            Controls.Add(lblError);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);

            AcceptButton = btnSave;
            CancelButton = btnCancel;

            WimpyTheme.Apply(this);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string candidate = txtConnectionString.Text.Trim();

            if (candidate.Length == 0)
            {
                lblError.Text = "Paste the connection string first.";
                return;
            }

            lblError.Text = "Connecting...";
            btnSave.Enabled = false;
            Cursor = Cursors.WaitCursor;
            Refresh();

            string error;
            bool connected = DatabaseConnection.TryConnect(candidate, out error);

            Cursor = Cursors.Default;
            btnSave.Enabled = true;

            if (!connected)
            {
                lblError.Text = "Could not connect, so nothing was saved: " + error;
                return;
            }

            try
            {
                DatabaseConnection.Configure(candidate);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                lblError.Text = "Connected, but the connection could not be saved on this PC: " + ex.Message;
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
