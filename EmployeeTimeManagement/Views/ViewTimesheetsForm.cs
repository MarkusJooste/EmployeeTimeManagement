using EmployeeTimeManagement.Controllers;
using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    public partial class ViewTimesheetsForm : Form, IHasShortcuts
    {

        // Ctrl+D, Ctrl+W and Ctrl+M pick the period; Ctrl+P prints the chosen employees'
        // sign-off sheets and Ctrl+Shift+P the store summary. Listed on the F1 shortcuts list.
        IEnumerable<Shortcut> IHasShortcuts.Shortcuts
        {
            get
            {
                return new[]
                {
                    new Shortcut(Keys.Control | Keys.D, btnDaily, null, "Today"),
                    new Shortcut(Keys.Control | Keys.W, btnWeekly, null, "This week"),
                    new Shortcut(Keys.Control | Keys.M, btnMonthly, null, "This month"),
                    new Shortcut(Keys.Control | Keys.P, btnSignOff, null, "Print sign-off sheets for the chosen employees"),
                    new Shortcut(Keys.Control | Keys.Shift | Keys.P, btnPrint, null, "Store summary PDF")
                };
            }
        }
        private readonly TimesheetController timesheetController;
        private readonly ReportController reportController;
        private List<TimesheetSummary> allRows = new List<TimesheetSummary>();

        // The rows in the grid after the search, which both PDFs print from.
        private List<TimesheetSummary> shownRows = new List<TimesheetSummary>();
        private DateTime fromDate;
        private DateTime toDate;

        private readonly ToolTip buttonTips = new ToolTip();

        public ViewTimesheetsForm()
        {
            InitializeComponent();
            timesheetController = new TimesheetController();
            reportController = new ReportController();

            // The sign-off sheet is what managers print most, so it is the red button.
            btnPrint.Tag = ButtonRole.Secondary;
            buttonTips.SetToolTip(btnSignOff,
                "A page per selected employee to sign that their hours are correct."
                + Environment.NewLine + "Ctrl+click to pick more than one, or Ctrl+A for everyone.");
            buttonTips.SetToolTip(btnPrint, "One page with the totals for everyone in the list.");
            Disposed += (sender, e) => buttonTips.Dispose();

            dgvTimesheets.SelectionChanged += (sender, e) => UpdateSignOffButton();

            ShowWeekly();
        }

        // Loads today only
        private void ShowDaily()
        {
            HighlightPeriod(btnDaily);
            SetPeriod(DateTime.Today, DateTime.Today);
        }

        // Loads the Monday to Sunday week that contains today
        private void ShowWeekly()
        {
            HighlightPeriod(btnWeekly);

            DateTime today = DateTime.Today;
            // DayOfWeek starts on Sunday, so Sunday is 6 days after the week's Monday
            int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;
            DateTime monday = today.AddDays(-daysSinceMonday);

            SetPeriod(monday, monday.AddDays(6));
        }

        // Loads the first to the last day of the current month
        private void ShowMonthly()
        {
            HighlightPeriod(btnMonthly);

            DateTime today = DateTime.Today;
            DateTime first = new DateTime(today.Year, today.Month, 1);

            SetPeriod(first, first.AddMonths(1).AddDays(-1));
        }

        // Fills the active period button red, or outlines every button when a custom range is used
        private void HighlightPeriod(Button active)
        {
            foreach (Button button in new[] { btnDaily, btnWeekly, btnMonthly })
            {
                WimpyTheme.SetToggleSelected(button, button == active);
            }
        }

        // Sets the period being displayed and reloads the grid
        private void SetPeriod(DateTime from, DateTime to)
        {
            fromDate = from.Date;
            toDate = to.Date;

            lblPeriod.Text = $"{fromDate:dd MMM yyyy} - {toDate:dd MMM yyyy}";
            dtpFrom.Value = fromDate;
            dtpTo.Value = toDate;

            LoadData();
        }

        private void btnDaily_Click(object sender, EventArgs e)
        {
            ShowDaily();
        }

        private void btnWeekly_Click(object sender, EventArgs e)
        {
            ShowWeekly();
        }

        private void btnMonthly_Click(object sender, EventArgs e)
        {
            ShowMonthly();
        }

        private void btnApplyRange_Click(object sender, EventArgs e)
        {
            DateTime from = dtpFrom.Value.Date;
            DateTime to = dtpTo.Value.Date;

            if (from > to)
            {
                lblStatus.Text = "Start date must be before end date";
                return;
            }

            HighlightPeriod(null);
            SetPeriod(from, to);
        }

        // Fetches the summary rows for the current period and binds them to the grid
        private void LoadData()
        {
            if (CurrentUser.StoreID == null)
            {
                allRows = new List<TimesheetSummary>();
                shownRows = allRows;
                dgvTimesheets.DataSource = null;
                lblStatus.Text = "No store assigned to this user";
                UpdateSignOffButton();
                return;
            }

            try
            {
                allRows = timesheetController.GetSummary(fromDate, toDate, CurrentUser.StoreID.Value);
            }
            catch (Exception ex)
            {
                allRows = new List<TimesheetSummary>();
                shownRows = allRows;
                dgvTimesheets.DataSource = null;
                lblStatus.Text = "Could not load timesheets: " + ex.Message;
                UpdateSignOffButton();
                return;
            }

            ApplyFilter();
        }

        // Narrows the loaded rows by the search text without re-querying the database
        private void ApplyFilter()
        {
            string search = txtSearch.Text.Trim();

            if (search.Length == 0)
            {
                shownRows = allRows;
            }
            else
            {
                shownRows = allRows
                    .Where(row => row.Name.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) >= 0
                               || row.Surname.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    .ToList();
            }

            dgvTimesheets.DataSource = new BindingList<TimesheetSummary>(shownRows);
            ShowTotals(shownRows);
            UpdateSignOffButton();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        // Summarises the displayed rows in the status label
        private void ShowTotals(List<TimesheetSummary> rows)
        {
            if (rows.Count == 0)
            {
                lblStatus.Text = "No timesheet records for this period";
                return;
            }

            decimal hours = rows.Sum(row => row.TotalHours);
            decimal payable = rows.Sum(row => row.PayableHours);

            lblStatus.Text = $"{rows.Count} employees · {hours:N2} hours · {payable:N2} payable";
        }

        // The employees picked in the grid, top to bottom as they are listed.
        private List<TimesheetSummary> SelectedEmployees()
        {
            return dgvTimesheets.SelectedRows
                .Cast<DataGridViewRow>()
                .OrderBy(row => row.Index)
                .Select(row => row.DataBoundItem as TimesheetSummary)
                .Where(row => row != null)
                .ToList();
        }

        // Says how many sheets the button will print, so a multi-select is not a surprise.
        private void UpdateSignOffButton()
        {
            int count = SelectedEmployees().Count;

            btnSignOff.Enabled = count > 0;
            btnSignOff.Text = count > 1 ? $"Print sign-off sheets ({count})" : "Print sign-off sheet";
        }

        private void btnSignOff_Click(object sender, EventArgs e)
        {
            List<TimesheetSummary> selected = SelectedEmployees();

            if (selected.Count == 0)
            {
                MessageBox.Show("Select the employee whose sign-off sheet you want to print.", "No Employee Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string filePath;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                List<SignOffSheet> sheets = timesheetController.GetSignOffSheets(
                    fromDate, toDate, CurrentUser.StoreID, selected.Select(row => row.EmployeeID).ToList());

                if (sheets.Count == 0)
                {
                    MessageBox.Show("Those employees could not be found in your store.", "No Data",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                filePath = reportController.GenerateSignOffSheets(fromDate, toDate, sheets);
            }
            catch (IOException ex)
            {
                ShowPdfError(ex, true);
                return;
            }
            catch (Exception ex)
            {
                ShowPdfError(ex, false);
                return;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            string what = selected.Count == 1
                ? "The sign-off sheet for " + selected[0].Name + " " + selected[0].Surname + " has been created."
                : "The sign-off sheets for " + selected.Count + " employees have been created, one page each.";

            OfferToOpen(filePath, what);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (shownRows.Count == 0)
            {
                string reason = allRows.Count == 0
                    ? "There are no timesheet records to include in the report."
                    : "There are no employees matching the current search.";

                MessageBox.Show(reason, "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string filePath;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                filePath = reportController.GenerateTimesheetReport(fromDate, toDate, shownRows);
            }
            catch (IOException ex)
            {
                ShowPdfError(ex, true);
                return;
            }
            catch (Exception ex)
            {
                ShowPdfError(ex, false);
                return;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            OfferToOpen(filePath, "The store summary has been created.");
        }

        // Saving over a PDF that is still open in a reader fails, which is the usual cause
        // of a file error here, so that case says what to do about it.
        private static void ShowPdfError(Exception ex, bool fileProblem)
        {
            string message = fileProblem
                ? "Could not save the PDF. If an earlier copy is open in a PDF reader, close it and try again.\n\n" + ex.Message
                : "Could not create the PDF:\n\n" + ex.Message;

            MessageBox.Show(message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void OfferToOpen(string filePath, string what)
        {
            DialogResult result = MessageBox.Show(what + "\n\nSaved to:\n" + filePath + "\n\nWould you like to open it to print?",
                "PDF Created", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
            }
        }
    }
}
