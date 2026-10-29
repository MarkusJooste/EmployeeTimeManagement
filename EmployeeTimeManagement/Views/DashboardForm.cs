using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    public partial class DashboardForm : Form
    {
        // Tracks the current displayed view inside the dashboard
        private Form currentView;
        // Distunguishes between logout and exit
        private bool isLoggingOut = false;

        // Each sidebar link's shortcut, shown when the mouse rests on it.
        private readonly ToolTip navTips = new ToolTip();

        public DashboardForm()
        {
            InitializeComponent();
            ApplyTheme();
            AddKeyboardHints();
            LoadUserInformation();
            ConfigurePermissions();
            ShowHome();
            CurrentUser.AccessRevoked += HandleAccessRevoked;
        }

        // Closes the dashboard the same way Logout does, returning to the login screen.
        private void HandleAccessRevoked()
        {
            isLoggingOut = true;
            Close();
        }

        // The red sidebar and white header strip, after wimpy.co.za's red navigation bar.
        private void ApplyTheme()
        {
            BackColor = WimpyTheme.Page;
            pnlContent.BackColor = WimpyTheme.Page;
            pnlSidebar.BackColor = WimpyTheme.Red;
            pnlHeader.BackColor = WimpyTheme.Card;
            pnlHeaderLine.BackColor = WimpyTheme.BrightRed;

            lblAppName.Font = WimpyTheme.Heading(15F);
            lblAppName.ForeColor = Color.White;

            lblTitle.Font = WimpyTheme.Heading(16F);
            lblTitle.ForeColor = WimpyTheme.Text;

            lblCurrentUser.Font = WimpyTheme.BodyBold;
            lblCurrentUser.ForeColor = Color.White;
            lblStore.Font = WimpyTheme.Body;
            lblStore.ForeColor = Color.FromArgb(245, 200, 202);

            foreach (Button button in NavButtons())
            {
                WimpyTheme.StyleButton(button, ButtonRole.Nav);
            }

            // Outlined in white on the red, like the website's Order Online button.
            WimpyTheme.StyleButton(btnLogout, ButtonRole.Primary);
            btnLogout.BackColor = WimpyTheme.Red;
            btnLogout.FlatAppearance.BorderColor = Color.White;
            btnLogout.FlatAppearance.MouseOverBackColor = WimpyTheme.PressedRed;
            btnLogout.FlatAppearance.MouseDownBackColor = WimpyTheme.PressedRed;
        }

        private Button[] NavButtons()
        {
            return new[] { btnDashboard, btnEmployees, btnContracts, btnCaptureTimesheets, btnViewTimesheets, btnLeave, btnManagers };
        }

        // Ctrl+1 to Ctrl+7 open the sidebar's screens in order, and F1 lists every shortcut.
        // Both are shown where they will be seen: on the sidebar and in the header.
        private void AddKeyboardHints()
        {
            Button[] nav = NavButtons();
            for (int index = 0; index < nav.Length; index++)
            {
                navTips.SetToolTip(nav[index], nav[index].Text + " (Ctrl+" + (index + 1) + ")");
            }

            var hint = new Label
            {
                Text = "F1  Keyboard shortcuts",
                Dock = DockStyle.Right,
                Width = 170,
                Padding = new Padding(0, 0, 16, 0),
                TextAlign = ContentAlignment.MiddleRight,
                Font = WimpyTheme.Body,
                ForeColor = WimpyTheme.MutedText,
                Cursor = Cursors.Hand
            };
            hint.Click += (sender, e) => ShowShortcuts();
            pnlHeader.Controls.Add(hint);

            // Docked after the red line under the header, so the line still runs full width.
            hint.BringToFront();

            Disposed += (sender, e) => navTips.Dispose();
        }

        // Keys that work on every screen come first, then the screen's own, then moving
        // between fields. Each screen sees a key before this does, so a list or a drop-down
        // that uses a key itself still gets it.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Control focused = KeyboardNavigation.Focused(this);

            int screen = ScreenNumber(keyData);
            if (screen >= 0)
            {
                Button nav = NavButtons()[screen];
                if (nav.Visible && nav.Enabled)
                {
                    nav.PerformClick();
                }

                return true;
            }

            if (keyData == Keys.F1)
            {
                ShowShortcuts();
                return true;
            }

            if (keyData == (Keys.Control | Keys.F) && currentView != null && KeyboardNavigation.FocusSearch(currentView))
            {
                return true;
            }

            var shortcuts = currentView as IHasShortcuts;
            if (shortcuts != null && KeyboardNavigation.RunShortcut(shortcuts, keyData, focused))
            {
                return true;
            }

            if (KeyboardNavigation.Move(focused, keyData))
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Which sidebar screen Ctrl+1 to Ctrl+7 (on either row of number keys) opens, or -1.
        private int ScreenNumber(Keys keyData)
        {
            if ((keyData & Keys.Modifiers) != Keys.Control)
            {
                return -1;
            }

            Keys key = keyData & Keys.KeyCode;
            int number = key >= Keys.D1 && key <= Keys.D9 ? key - Keys.D1
                : key >= Keys.NumPad1 && key <= Keys.NumPad9 ? key - Keys.NumPad1
                : -1;

            return number < NavButtons().Length ? number : -1;
        }

        private void ShowShortcuts()
        {
            var screens = NavButtons()
                .Select((button, index) => new { button, index })
                .Where(nav => nav.button.Visible)
                .Select(nav => new KeyValuePair<string, string>("Ctrl+" + (nav.index + 1), nav.button.Text));

            var shortcuts = currentView as IHasShortcuts;
            var thisScreen = shortcuts == null
                ? Enumerable.Empty<KeyValuePair<string, string>>()
                : shortcuts.Shortcuts
                    .GroupBy(shortcut => KeyboardNavigation.Describe(shortcut.Keys) + "|" + shortcut.Explain())
                    .Select(group => group.First())
                    .Select(shortcut => new KeyValuePair<string, string>(KeyboardNavigation.Describe(shortcut.Keys), shortcut.Explain()));

            using (var dialog = new KeyboardShortcutsForm(lblTitle.Text, screens, thisScreen))
            {
                dialog.ShowDialog(this);
            }
        }

        private void LoadUserInformation()
        {
            if (CurrentUser.Manager == null)
            {
                return;
            }

            lblCurrentUser.Text = CurrentUser.Manager.ManagerName;
            lblStore.Text = CurrentUser.IsAdmin ? "All stores" : $"Store {CurrentUser.StoreID}";
        }

        private void ConfigurePermissions()
        {
            btnManagers.Visible = CurrentUser.IsAdmin;
        }

        private void ShowHome()
        {
            // Load the default view into the dashboard
            ShowView(() =>
            {
                var home = new DashboardHomeForm();
                home.OpenEmployeeRequested += OpenEmployee;
                return home;
            }, btnDashboard);
        }

        // Switches to the Employees screen with one employee selected, ready to update. Queued
        // because the request comes from a click on the dashboard, which ShowView disposes;
        // disposing a control inside its own Click handler can fail once the handler returns.
        private void OpenEmployee(int employeeID)
        {
            BeginInvoke(new Action(() =>
            {
                var employees = (EmployeesForm)ShowView(() => new EmployeesForm(), btnEmployees);
                employees.ShowEmployee(employeeID);
            }));
        }

        // Shows a screen in the content panel, marks its link in the sidebar and names it in the
        // header. The screen is created here rather than by the caller, because creating it is
        // what loads its data: the wait cursor goes up first, and the sidebar and header are
        // redrawn straight away so the click is seen to land while the screen loads.
        private Form ShowView(Func<Form> createView, Button navButton)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                foreach (Button button in NavButtons())
                {
                    WimpyTheme.SetNavSelected(button, button == navButton);
                }

                lblTitle.Text = navButton.Text;
                pnlSidebar.Update();
                pnlHeader.Update();

                Form view = createView();
                WimpyTheme.Apply(view);

                if (currentView != null)
                {
                    currentView.Close();
                    currentView.Dispose();
                }

                // Set up the new view to display inside the panel
                currentView = view;
                view.TopLevel = false;
                view.FormBorderStyle = FormBorderStyle.None;
                view.Dock = DockStyle.Fill;

                pnlContent.Controls.Clear();
                pnlContent.Controls.Add(view);
                view.Show();

                // Ready to type into, so a screen opened from the keyboard can be used at once.
                KeyboardNavigation.LetTabLeaveLists(view);
                KeyboardNavigation.FocusFirstField(view);

                return view;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            ShowHome();
        }

        private void btnEmployees_Click(object sender, EventArgs e)
        {
            ShowView(() => new EmployeesForm(), btnEmployees);
        }

        private void btnContracts_Click(object sender, EventArgs e)
        {
            ShowView(() => new ContractsForm(), btnContracts);
        }

        private void btnCaptureTimesheets_Click(object sender, EventArgs e)
        {
            ShowView(() => new CaptureTimesheetsForm(), btnCaptureTimesheets);
        }

        private void btnViewTimesheets_Click(object sender, EventArgs e)
        {
            ShowView(() => new ViewTimesheetsForm(), btnViewTimesheets);
        }

        private void btnLeave_Click(object sender, EventArgs e)
        {
            ShowView(() => new LeaveForm(), btnLeave);
        }

        private void btnManagers_Click(object sender, EventArgs e)
        {
            ShowView(() => new ManagersForm(), btnManagers);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }
            isLoggingOut = true;
            Close();
        }

        private void DashboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CurrentUser.AccessRevoked -= HandleAccessRevoked;

            // Removes current user session
            CurrentUser.Logout();
            // Exits the app if the user clicked the X
            if (!isLoggingOut)
            {
                Application.Exit();
            }
        }
    }
}
