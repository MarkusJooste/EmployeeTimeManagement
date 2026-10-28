using EmployeeTimeManagement.Controllers;
using EmployeeTimeManagement.Database;
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
    public partial class DashboardHomeForm : Form
    {
        // Asks the main window to open the Employees screen on one employee, so a lapsed
        // contract can be renewed or terminated from the dashboard.
        public event Action<int> OpenEmployeeRequested;

        // The longest-overdue lapsed contract, which a click on the renew tile opens.
        private DashboardAlert firstToRenew;

        private readonly ToolTip tileTips = new ToolTip { AutoPopDelay = 15000, InitialDelay = 300 };

        public DashboardHomeForm()
        {
            InitializeComponent();
            Disposed += (sender, e) => tileTips.Dispose();
            StyleTiles();
            LoadDashboardInformation();
        }

        // White tiles with a red edge and the figure in red, like the price tags on wimpy.co.za.
        private void StyleTiles()
        {
            WimpyTheme.Apply(this);
            lblWelcome.Font = WimpyTheme.Heading(16F);
            lblWelcome.ForeColor = WimpyTheme.Text;
            lblStore.ForeColor = WimpyTheme.MutedText;
            lblStore.Top = lblWelcome.Bottom + 4;

            foreach (Panel tile in new[] { pnlEmployees, pnlHours, pnlLeave, pnlAwol, pnlRenew })
            {
                tile.BorderStyle = BorderStyle.None;
                tile.BackColor = WimpyTheme.Card;
                tile.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 5, BackColor = WimpyTheme.BrightRed });
            }

            foreach (Label caption in new[] { lblEmployeesCaption, lblHoursCaption, lblLeaveCaption, lblAwolCaption, lblRenewCaption })
            {
                caption.Text = caption.Text.ToUpperInvariant();
                caption.Font = WimpyTheme.BodyBold;
                caption.ForeColor = WimpyTheme.MutedText;
                caption.Location = new Point(18, 10);
            }

            foreach (Label value in new[] { lblEmployeeCount, lblHoursWeek, lblLeaveToday, lblAwolWeek, lblRenewCount })
            {
                value.Font = WimpyTheme.Heading(20F);
                value.ForeColor = WimpyTheme.Red;
                value.Location = new Point(15, 28);
            }

            lblRenewNames.Font = WimpyTheme.Body;
            lblRenewNames.ForeColor = WimpyTheme.Text;
            lblRenewNames.Text = string.Empty;

            // The whole tile is the click target, not just the number.
            foreach (Control part in new Control[] { pnlRenew, lblRenewCaption, lblRenewCount, lblRenewNames })
            {
                part.Click += (sender, e) => OpenFirstToRenew();
            }
        }

        // Names who the renew tile counts, lists them all with their end dates on hover, and
        // makes the tile open the longest overdue in the Employees screen.
        private void ShowRenewals(List<DashboardAlert> toRenew)
        {
            lblRenewCount.Text = toRenew.Count.ToString();
            lblRenewNames.Text = DashboardChecks.NamesSummary(toRenew);
            firstToRenew = toRenew.FirstOrDefault();

            Cursor cursor = firstToRenew == null ? Cursors.Default : Cursors.Hand;
            string tip = null;

            if (firstToRenew != null)
            {
                tip = string.Join(Environment.NewLine, toRenew.Select(alert => alert.EmployeeName + ": " + alert.Note))
                    + Environment.NewLine + Environment.NewLine
                    + "Click to open " + firstToRenew.EmployeeName + " in Employees.";
            }

            foreach (Control part in new Control[] { pnlRenew, lblRenewCaption, lblRenewCount, lblRenewNames })
            {
                part.Cursor = cursor;
                tileTips.SetToolTip(part, tip);
            }
        }

        private void OpenFirstToRenew()
        {
            if (firstToRenew != null && OpenEmployeeRequested != null)
            {
                OpenEmployeeRequested(firstToRenew.EmployeeID);
            }
        }

        private void LoadDashboardInformation()
        {
            if (CurrentUser.Manager == null)
            {
                return;
            }

            lblWelcome.Text = $"Welcome back, {CurrentUser.Manager.ManagerName}";

            lblStore.Text = CurrentUser.IsAdmin ? "All stores" : $"Store {CurrentUser.StoreID}";

            // The tiles have no status label of their own, so each starts as a dash and keeps
            // it if its figure cannot be loaded, rather than silently reading as a zero.
            lblEmployeeCount.Text = "—";
            lblHoursWeek.Text = "—";
            lblLeaveToday.Text = "—";
            lblAwolWeek.Text = "—";
            lblRenewCount.Text = "—";

            LoadCards();
        }

        // Fills a tile from a query, leaving its dash if the query fails.
        private static void FillTile(Label tile, string name, Func<string> load)
        {
            try
            {
                tile.Text = load();
            }
            catch (Exception ex)
            {
                tile.Text = "—";
                System.Diagnostics.Trace.TraceError("Could not load the " + name + " tile: " + ex.Message);
            }
        }

        // Fills the six lists of people who need attention. Managers see their own store;
        // the owner sees every store, with a Store column to tell them apart. All six reads go
        // to the database in one batch, since every trip to the server is slow; each list and
        // tile still reports its own failure, so one failed query does not blank the rest.
        private void LoadCards()
        {
            bool showStore = CurrentUser.IsAdmin;

            var today = new DashboardListCard("Timesheets not captured today", showStore);
            var missed = new DashboardListCard("Missed timesheets (last " + DashboardChecks.MissedDaysLookBack + " days)", showStore);
            var longHours = new DashboardListCard("Long hours this week", showStore);
            var probation = new DashboardListCard("Probation ending soon", showStore);
            var contracts = new DashboardListCard("Contracts ended or ending", showStore);
            var incomplete = new DashboardListCard("Incomplete employee records", showStore);

            tlpCards.Controls.Add(today, 0, 0);
            tlpCards.Controls.Add(missed, 1, 0);
            tlpCards.Controls.Add(longHours, 2, 0);
            tlpCards.Controls.Add(probation, 0, 1);
            tlpCards.Controls.Add(contracts, 1, 1);
            tlpCards.Controls.Add(incomplete, 2, 1);

            var cards = new[] { today, missed, longHours, probation, contracts, incomplete };

            if (!CurrentUser.IsAdmin && CurrentUser.StoreID == null)
            {
                foreach (DashboardListCard card in cards)
                {
                    card.ShowMessage("no store assigned");
                }

                return;
            }

            int? storeID = CurrentUser.IsAdmin ? null : CurrentUser.StoreID;
            DateTime date = DateTime.Today;
            DateTime weekStart = DashboardChecks.WeekStart(date);

            // The employee overview feeds most of the dashboard, so it goes first: the server
            // stops at a failing statement, and everything before it still arrives.
            var dashboard = new DashboardController();
            var timesheets = new TimesheetController();
            Query<List<EmployeeOverview>> overviewQuery = dashboard.EmployeeOverviewQuery(storeID);
            Query<List<TimesheetSummary>> weekHoursQuery = timesheets.SummaryQuery(weekStart, date, storeID);
            Query<List<EmployeeDayStatus>> dayStatusQuery = timesheets.DayStatusQuery(date, storeID);
            Query<List<CapturedDay>> capturedQuery = dashboard.CapturedDaysQuery(
                date.AddDays(-DashboardChecks.MissedDaysLookBack), date.AddDays(-1), storeID);
            Query<int> onLeaveQuery = new LeaveController().OnLeaveTodayQuery(storeID, date);
            Query<int> awolQuery = dashboard.AwolDaysQuery(weekStart, date, storeID);

            QueryBatch.Run(overviewQuery, weekHoursQuery, dayStatusQuery, capturedQuery, onLeaveQuery, awolQuery);

            FillTile(lblLeaveToday, "on leave today", () => onLeaveQuery.Result.ToString());
            FillTile(lblAwolWeek, "AWOL this week", () => awolQuery.Result.ToString());

            LoadTodayCard(today, date, dayStatusQuery);

            // Hours worked this pay week, for the Hours tile and the Long hours list.
            List<TimesheetSummary> weekHours = null;
            Exception weekHoursError = null;
            try
            {
                weekHours = weekHoursQuery.Result;
                lblHoursWeek.Text = weekHours.Sum(summary => summary.TotalHours).ToString("0.#");
            }
            catch (Exception ex)
            {
                weekHoursError = ex;
                System.Diagnostics.Trace.TraceError("Could not load the hours this week tile: " + ex.Message);
            }

            List<EmployeeOverview> employees;
            try
            {
                employees = overviewQuery.Result;
            }
            catch (Exception ex)
            {
                foreach (DashboardListCard card in new[] { missed, longHours, probation, contracts, incomplete })
                {
                    card.ShowError(ex.Message);
                }

                return;
            }

            lblEmployeeCount.Text = DashboardChecks.EmployedCount(date, employees).ToString();

            probation.ShowAlerts(DashboardChecks.ProbationEnding(date, employees), "No probation ending in the next "
                + DashboardChecks.ProbationWarningDays + " days");
            // Lapsed fixed-term contracts first: those people are already working without one.
            List<DashboardAlert> toRenew = DashboardChecks.ContractsToRenew(date, employees);
            ShowRenewals(toRenew);
            contracts.ShowAlerts(toRenew.Concat(DashboardChecks.ContractsEnding(date, employees)).ToList(),
                "No contracts ended or ending in the next " + DashboardChecks.ContractEndingWarningDays + " days");
            incomplete.ShowAlerts(DashboardChecks.IncompleteRecords(date, employees), "Every employee record is complete");

            try
            {
                List<CapturedDay> captured = capturedQuery.Result;
                missed.ShowAlerts(DashboardChecks.MissedTimesheets(date, employees, captured),
                    "No missed timesheets in the last " + DashboardChecks.MissedDaysLookBack + " days");
            }
            catch (Exception ex)
            {
                missed.ShowError(ex.Message);
            }

            if (weekHours == null)
            {
                longHours.ShowError(weekHoursError.Message);
                return;
            }

            var hours = new Dictionary<int, decimal>();
            foreach (TimesheetSummary summary in weekHours)
            {
                hours[summary.EmployeeID] = summary.TotalHours;
            }

            longHours.ShowAlerts(DashboardChecks.LongHours(employees, hours),
                "Nobody near " + DashboardChecks.NormalWeeklyHours.ToString("0") + " hours this week");
        }

        // Everyone employed today whose timesheet has not been saved yet.
        private static void LoadTodayCard(DashboardListCard card, DateTime date, Query<List<EmployeeDayStatus>> dayStatus)
        {
            try
            {
                var alerts = new List<DashboardAlert>();

                foreach (EmployeeDayStatus employee in TimesheetCompletion.MissingOn(date, dayStatus.Result))
                {
                    // A booked leave day only needs the pre-filled Leave row saved, so say so.
                    alerts.Add(new DashboardAlert(employee.FullName, employee.StoreName, employee.IsOnLeave ? "On leave" : string.Empty));
                }

                card.ShowAlerts(alerts, "All timesheets captured for today");
            }
            catch (Exception ex)
            {
                card.ShowError(ex.Message);
            }
        }
    }
}
