using System.Windows.Forms;
using EmployeeTimeManagement.Controllers;
using EmployeeTimeManagement.Models;

namespace EmployeeTimeManagement.Views
{
    // Shows the one message every view gives when WriteAccessGuard refuses a write, then ends the session so the dashboard returns to the login screen.
    internal static class AccessRevokedPrompt
    {
        public static void Show(AccessRevokedException ex)
        {
            MessageBox.Show(ex.Message, "Access removed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            CurrentUser.RevokeAccess();
        }
    }
}
