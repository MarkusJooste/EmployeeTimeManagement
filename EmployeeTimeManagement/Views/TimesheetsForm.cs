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
            ShowCurrentWeek();
        }

        // Loads the Monday to Sunday week that contains today
        private void ShowCurrentWeek()
        {
            DateTime today = DateTime.Today;
            // DayOfWeek starts on Sunday, so Sunday is 6 days after the week's Monday
            int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;
            DateTime monday = today.AddDays(-daysSinceMonday);

            SetPeriod(monday, monday.AddDays(6));
        }

        // Sets the period being displayed and reloads the grid
        private void SetPeriod(DateTime from, DateTime to)
        {
            fromDate = from.Date;
            toDate = to.Date;

            lblPeriod.Text = $"{fromDate:dd MMM yyyy} - {toDate:dd MMM yyyy}";

            LoadData();
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
