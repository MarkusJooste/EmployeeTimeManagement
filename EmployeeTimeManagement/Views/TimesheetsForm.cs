using EmployeeTimeManagement.Controllers;
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
    public partial class TimesheetsForm : Form
    {
        private readonly TimesheetController timesheetController;
        private List<TimesheetSummary> allRows = new List<TimesheetSummary>();
        private DateTime fromDate;
        private DateTime toDate;

        public TimesheetsForm()
        {
            InitializeComponent();
            timesheetController = new TimesheetController();
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

        // Bolds the active period button, or clears every button when a custom range is used
        private void HighlightPeriod(Button active)
        {
            foreach (Button button in new[] { btnDaily, btnWeekly, btnMonthly })
            {
                bool isActive = button == active;
                button.Font = new Font(button.Font, isActive ? FontStyle.Bold : FontStyle.Regular);
                button.BackColor = isActive ? SystemColors.Highlight : SystemColors.Control;
                button.ForeColor = isActive ? SystemColors.HighlightText : SystemColors.ControlText;
                button.UseVisualStyleBackColor = !isActive;
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
                dgvTimesheets.DataSource = null;
                lblStatus.Text = "No store assigned to this user";
                return;
            }

            try
            {
                allRows = timesheetController.GetSummary(fromDate, toDate, CurrentUser.StoreID.Value);
            }
            catch (Exception ex)
            {
                allRows = new List<TimesheetSummary>();
                dgvTimesheets.DataSource = null;
                lblStatus.Text = "Could not load timesheets: " + ex.Message;
                return;
            }

            dgvTimesheets.DataSource = new BindingList<TimesheetSummary>(allRows);
            ShowTotals(allRows);
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
    }
}
