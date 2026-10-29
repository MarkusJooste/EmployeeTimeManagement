using EmployeeTimeManagement.Database;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // The one error dialog: logs the failure, shows it, and offers Copy diagnostics so the
    // customer can send the details back. Used wherever the program reports an error.
    public class ErrorDialog : Form
    {
        // Logs and shows an error caught in a view.
        public static void Show(string title, string message, Exception exception)
        {
            ErrorLog.Default.Write(title + ": " + message, exception);
            ShowDialog(title, message, ErrorLog.Default.BuildDiagnostics(title, message, exception));
        }

        // Shows a problem that has no exception, such as a failed connection test. Logged too.
        public static void Show(string title, string message)
        {
            ErrorLog.Default.Write(title, message);
            ShowDialog(title, message, ErrorLog.Default.BuildDiagnostics(title, message));
        }

        private static void ShowDialog(string title, string message, string diagnostics)
        {
            using (var dialog = new ErrorDialog(title, message, diagnostics))
            {
                dialog.ShowDialog();
            }
        }

        // Adds a Copy diagnostics button to a dialog at the given spot.
        public static Button CopyDiagnosticsButton(string diagnostics, Point location)
        {
            var button = new Button();
            button.Text = "Copy diagnostics";
            button.Location = location;
            button.Size = new Size(120, 28);
            button.Click += (sender, e) =>
            {
                try
                {
                    Clipboard.SetText(diagnostics);
                    button.Text = "Copied";
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    // Another program is holding the clipboard.
                    button.Text = "Try again";
                }
            };
            return button;
        }

        private ErrorDialog(string title, string message, string diagnostics)
        {
            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(460, 150);

            var lblMessage = new Label();
            lblMessage.AutoSize = false;
            lblMessage.Location = new Point(12, 12);
            lblMessage.Size = new Size(436, 90);
            lblMessage.Text = message;

            Button btnCopy = CopyDiagnosticsButton(diagnostics, new Point(12, 110));

            var btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new Point(343, 110);
            btnOk.Size = new Size(105, 28);
            btnOk.DialogResult = DialogResult.OK;

            Controls.Add(lblMessage);
            Controls.Add(btnCopy);
            Controls.Add(btnOk);

            AcceptButton = btnOk;
            CancelButton = btnOk;

            WimpyTheme.Apply(this);
        }
    }
}
