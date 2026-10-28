using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class DashboardChecksTests
    {
        // A Wednesday.
        private static readonly DateTime Today = new DateTime(2026, 10, 28);

        private static EmployeeOverview Build(int employeeID, DateTime? start = null, DateTime? end = null)
        {
            return new EmployeeOverview
            {
                EmployeeID = employeeID,
                StoreName = "Paarl Mall",
                Name = "Name" + employeeID,
                Surname = "Surname" + employeeID,
                IDNumber = "9001015009087",
                ContractStartDate = start,
                ContractEndDate = end,
                HourlyRate = 35.50m,
                HasAddress = true,
                HasNextOfKin = true
            };
        }

        private static List<int> IDs(IEnumerable<DashboardAlert> alerts)
        {
            return alerts.Select(alert => alert.EmployeeID).ToList();
        }

        [Test]
        public void EmployeeCountOnlyCountsThoseUnderContractToday()
        {
            var employees = new[]
            {
                Build(1),                                            // no contract row: employed
                Build(2, Today.AddYears(-1)),                        // open-ended
                Build(3, Today.AddDays(1)),                          // starts tomorrow
                Build(4, Today.AddYears(-1), Today.AddDays(-1)),     // left yesterday
                Build(5, Today.AddYears(-1), Today)                  // last day today
            };

            Assert.That(DashboardChecks.EmployedCount(Today, employees), Is.EqualTo(3));
        }

        // ------------------------------------------------------------------
        // Missed timesheets
        // ------------------------------------------------------------------

        private static List<CapturedDay> EveryDay(int employeeID, int fromDaysAgo, int toDaysAgo)
        {
            var days = new List<CapturedDay>();
            for (int daysAgo = fromDaysAgo; daysAgo >= toDaysAgo; daysAgo--)
            {
                days.Add(new CapturedDay(employeeID, Today.AddDays(-daysAgo)));
            }

            return days;
        }

        [Test]
        public void FullyCapturedWeekHasNoMissedDays()
        {
            List<DashboardAlert> alerts = DashboardChecks.MissedTimesheets(Today, new[] { Build(1) }, EveryDay(1, 7, 1));

            Assert.That(alerts, Is.Empty);
        }

        [Test]
        public void MissedDaysAreListedOldestFirst()
        {
            List<CapturedDay> captured = EveryDay(1, 7, 1).Where(day => day.WorkDate != Today.AddDays(-1) && day.WorkDate != Today.AddDays(-3)).ToList();

            DashboardAlert alert = DashboardChecks.MissedTimesheets(Today, new[] { Build(1) }, captured).Single();

            Assert.That(alert.Note, Is.EqualTo("2 days: Sun 25 Oct, Tue 27 Oct"));
        }

        [Test]
        public void OneMissedDayIsJustTheDate()
        {
            List<CapturedDay> captured = EveryDay(1, 7, 2);

            Assert.That(DashboardChecks.MissedTimesheets(Today, new[] { Build(1) }, captured).Single().Note, Is.EqualTo("Tue 27 Oct"));
        }

        [Test]
        public void TodayAndDaysBeforeTheLookBackAreIgnored()
        {
            // Nothing captured at all, but only yesterday back to 7 days ago count.
            DashboardAlert alert = DashboardChecks.MissedTimesheets(Today, new[] { Build(1) }, new CapturedDay[0]).Single();

            Assert.That(alert.Note, Does.StartWith("7 days: Wed 21 Oct"));
            Assert.That(alert.Note, Does.EndWith("Tue 27 Oct"));
        }

        [Test]
        public void DaysBeforeTheContractStartedAreNotMissed()
        {
            EmployeeOverview starter = Build(1, Today.AddDays(-2));

            DashboardAlert alert = DashboardChecks.MissedTimesheets(Today, new[] { starter }, new CapturedDay[0]).Single();

            Assert.That(alert.Note, Is.EqualTo("2 days: Mon 26 Oct, Tue 27 Oct"));
        }

        [Test]
        public void AnotherEmployeesTimesheetDoesNotCount()
        {
            List<DashboardAlert> alerts = DashboardChecks.MissedTimesheets(Today, new[] { Build(1), Build(2) }, EveryDay(2, 7, 1));

            Assert.That(IDs(alerts), Is.EqualTo(new[] { 1 }));
        }

        // ------------------------------------------------------------------
        // Probation and contracts ending
        // ------------------------------------------------------------------

        [Test]
        public void ProbationIsFourMonthsFromTheContractStart()
        {
            Assert.That(DashboardChecks.ProbationEnd(new DateTime(2026, 7, 1)), Is.EqualTo(new DateTime(2026, 11, 1)));
        }

        [TestCase(0, true)]
        [TestCase(28, true)]
        [TestCase(29, false)]
        [TestCase(-1, false)]
        public void ProbationEndingIsListedWithinTheWarningWindow(int daysUntilEnd, bool listed)
        {
            EmployeeOverview employee = Build(1, Today.AddDays(daysUntilEnd).AddMonths(-4));

            List<DashboardAlert> alerts = DashboardChecks.ProbationEnding(Today, new[] { employee });

            Assert.That(alerts.Count, Is.EqualTo(listed ? 1 : 0));
        }

        [Test]
        public void ProbationNoteSaysWhenItEnds()
        {
            EmployeeOverview employee = Build(1, new DateTime(2026, 7, 5));

            Assert.That(DashboardChecks.ProbationEnding(Today, new[] { employee }).Single().Note, Is.EqualTo("Ends Thu 5 Nov (8 days)"));
        }

        [Test]
        public void NoProbationWarningWithoutAContractOrAfterLeaving()
        {
            EmployeeOverview noContract = Build(1);
            EmployeeOverview left = Build(2, Today.AddDays(5).AddMonths(-4), Today.AddDays(-1));

            Assert.That(DashboardChecks.ProbationEnding(Today, new[] { noContract, left }), Is.Empty);
        }

        [TestCase(0, "Ends Wed 28 Oct (today)")]
        [TestCase(1, "Ends Thu 29 Oct (tomorrow)")]
        [TestCase(30, "Ends Fri 27 Nov (30 days)")]
        public void ContractsEndingWithinThirtyDaysAreListed(int daysUntilEnd, string note)
        {
            EmployeeOverview employee = Build(1, Today.AddYears(-1), Today.AddDays(daysUntilEnd));

            Assert.That(DashboardChecks.ContractsEnding(Today, new[] { employee }).Single().Note, Is.EqualTo(note));
        }

        [Test]
        public void ContractsEndingLaterOrAlreadyEndedAreNotListed()
        {
            EmployeeOverview later = Build(1, Today.AddYears(-1), Today.AddDays(31));
            EmployeeOverview ended = Build(2, Today.AddYears(-1), Today.AddDays(-1));
            EmployeeOverview open = Build(3, Today.AddYears(-1));

            Assert.That(DashboardChecks.ContractsEnding(Today, new[] { later, ended, open }), Is.Empty);
        }

        [Test]
        public void SoonestEndingIsFirst()
        {
            EmployeeOverview later = Build(1, Today.AddYears(-1), Today.AddDays(20));
            EmployeeOverview sooner = Build(2, Today.AddYears(-1), Today.AddDays(3));

            Assert.That(IDs(DashboardChecks.ContractsEnding(Today, new[] { later, sooner })), Is.EqualTo(new[] { 2, 1 }));
        }

        // ------------------------------------------------------------------
        // Long hours
        // ------------------------------------------------------------------

        [Test]
        public void PayWeekStartsOnSunday()
        {
            Assert.That(DashboardChecks.WeekStart(Today), Is.EqualTo(new DateTime(2026, 10, 25)));
            Assert.That(DashboardChecks.WeekStart(new DateTime(2026, 10, 25)), Is.EqualTo(new DateTime(2026, 10, 25)));
            Assert.That(DashboardChecks.WeekStart(new DateTime(2026, 10, 31)), Is.EqualTo(new DateTime(2026, 10, 25)));
        }

        [Test]
        public void LongHoursListsFortyAndUpMostHoursFirst()
        {
            var hours = new Dictionary<int, decimal> { { 1, 39.9m }, { 2, 40m }, { 3, 47.5m } };

            List<DashboardAlert> alerts = DashboardChecks.LongHours(new[] { Build(1), Build(2), Build(3), Build(4) }, hours);

            Assert.That(IDs(alerts), Is.EqualTo(new[] { 3, 2 }));
            Assert.That(alerts[0].Note, Is.EqualTo("47.5 h (over 45)"));
            Assert.That(alerts[1].Note, Is.EqualTo("40.0 h"));
        }

        [Test]
        public void ExactlyFortyFiveHoursIsNotOver()
        {
            var hours = new Dictionary<int, decimal> { { 1, 45m } };

            Assert.That(DashboardChecks.LongHours(new[] { Build(1) }, hours).Single().Note, Is.EqualTo("45.0 h"));
        }

        // ------------------------------------------------------------------
        // Incomplete records
        // ------------------------------------------------------------------

        [Test]
        public void ACompleteRecordIsNotListed()
        {
            Assert.That(DashboardChecks.IncompleteRecords(Today, new[] { Build(1) }), Is.Empty);
        }

        [Test]
        public void EverythingMissingIsNamed()
        {
            EmployeeOverview employee = Build(1);
            employee.IDNumber = " ";
            employee.HasAddress = false;
            employee.HasNextOfKin = false;
            employee.HourlyRate = null;

            Assert.That(DashboardChecks.IncompleteRecords(Today, new[] { employee }).Single().Note,
                Is.EqualTo("No ID number, address, next of kin, hourly rate"));
        }

        [Test]
        public void AZeroHourlyRateCountsAsMissing()
        {
            EmployeeOverview employee = Build(1);
            employee.HourlyRate = 0m;

            Assert.That(DashboardChecks.IncompleteRecords(Today, new[] { employee }).Single().Note, Is.EqualTo("No hourly rate"));
        }

        [Test]
        public void FormerEmployeesAreNotNaggedAbout()
        {
            EmployeeOverview left = Build(1, Today.AddYears(-1), Today.AddDays(-1));
            left.HasAddress = false;

            Assert.That(DashboardChecks.IncompleteRecords(Today, new[] { left }), Is.Empty);
        }

        [Test]
        public void ListsKeepEachStoreTogether()
        {
            EmployeeOverview wellington = Build(1);
            wellington.StoreName = "Wellington";
            wellington.HasAddress = false;
            EmployeeOverview paarl = Build(2);
            paarl.HasAddress = false;

            Assert.That(IDs(DashboardChecks.IncompleteRecords(Today, new[] { wellington, paarl })), Is.EqualTo(new[] { 2, 1 }));
        }
    }
}
