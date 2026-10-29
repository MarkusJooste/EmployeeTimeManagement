using EmployeeTimeManagement.Database;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // Asked on launch when the database connection is missing or failing: fix it now
    // (Retry, or Create when nothing is saved) or carry on without it.
    public class StartupChoiceForm : Form
    {
        public StartupChoice Choice { get; private set; }

        public StartupChoiceForm(StartupProblem problem, string detail)
        {
            Choice = StartupChoice.ContinueForNow;

            Text = "Database connection";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(460, 150);

            var lblDetail = new Label();
            lblDetail.AutoSize = false;
            lblDetail.Location = new Point(12, 12);
            lblDetail.Size = new Size(436, 90);
            lblDetail.Text = detail;

            StartupChoice primary = problem == StartupProblem.NotSetUp ? StartupChoice.Create : StartupChoice.Retry;

            var btnPrimary = new Button();
            btnPrimary.Text = primary == StartupChoice.Create ? "Create" : "Retry";
            btnPrimary.Location = new Point(222, 110);
            btnPrimary.Size = new Size(105, 28);
            btnPrimary.Click += (sender, e) =>
            {
                Choice = primary;
                DialogResult = DialogResult.OK;
            };

            var btnContinue = new Button();
            btnContinue.Text = "Continue for now";
            btnContinue.Location = new Point(333, 110);
            btnContinue.Size = new Size(115, 28);
            btnContinue.Click += (sender, e) =>
            {
                Choice = StartupChoice.ContinueForNow;
                DialogResult = DialogResult.Cancel;
            };

            Controls.Add(lblDetail);
            Controls.Add(btnPrimary);
            Controls.Add(btnContinue);

            AcceptButton = btnPrimary;
            CancelButton = btnContinue;

            WimpyTheme.Apply(this);
        }
    }
}
