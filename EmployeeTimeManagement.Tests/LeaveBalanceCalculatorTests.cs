using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class LeaveBalanceCalculatorTests
    {
        private static readonly DateTime ContractStart = new DateTime(2020, 1, 1);

        private static LeaveBalanceRequest EmptyRequest()
        {
            return new LeaveBalanceRequest
            {
                AsOf = new DateTime(2026, 3, 1),
                ContractStartDate = ContractStart,
                OpeningBalanceAsAt = null,
                OpeningPTODays = 0,
                WorkedDates = new List<DateTime>(),
                Absences = new List<Absence>()
            };
        }

        private static List<DateTime> WorkedDays(DateTime start, int count)
        {
            return Enumerable.Range(0, count).Select(i => start.AddDays(i)).ToList();
        }

        private static Absence Pto(DateTime start, DateTime end)
        {
            return new Absence { LeaveType = LeaveType.PTO, StartDate = start, EndDate = end };
        }

        private static Absence Sick(DateTime start, DateTime end)
        {
            return new Absence { LeaveType = LeaveType.Sick, StartDate = start, EndDate = end };
        }

        [Test]
        public void AnEmployeeWithNoTimesheetsNoAbsencesAndNoContractDataReadsAsZero()
        {
            LeaveBalanceSummary summary = LeaveBalanceCalculator.Calculate(EmptyRequest());

            Assert.That(summary.PTO.DaysWorked, Is.EqualTo(0));
            Assert.That(summary.PTO.DaysAccrued, Is.EqualTo(0));
            Assert.That(summary.PTO.OpeningBalance, Is.EqualTo(0));
            Assert.That(summary.PTO.DaysTaken, Is.EqualTo(0));
            Assert.That(summary.PTO.Balance, Is.EqualTo(0));
            Assert.That(summary.PTO.IsAtCap, Is.False);
            Assert.That(summary.Sick.DaysRemaining, Is.EqualTo(30));
            Assert.That(summary.Maternity.DaysTaken, Is.EqualTo(0));
            Assert.That(summary.AWOL.DaysTaken, Is.EqualTo(0));
        }

        [Test]
        public void SeventeenDaysWorkedAccruesOneDayOfPTO()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.WorkedDates = WorkedDays(new DateTime(2026, 1, 1), 17);

            PTOBalance pto = LeaveBalanceCalculator.Calculate(request).PTO;

            Assert.That(pto.DaysWorked, Is.EqualTo(17));
            Assert.That(pto.DaysAccrued, Is.EqualTo(1));
            Assert.That(pto.Balance, Is.EqualTo(1));
        }

        [Test]
        public void SixteenDaysWorkedAccruesNothingYet()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.WorkedDates = WorkedDays(new DateTime(2026, 1, 1), 16);

            PTOBalance pto = LeaveBalanceCalculator.Calculate(request).PTO;

            Assert.That(pto.DaysAccrued, Is.EqualTo(0));
            Assert.That(pto.Balance, Is.EqualTo(0));
        }

        [Test]
        public void HoursAndDayTypeAreIrrelevantOnlyDistinctWorkedDatesCount()
        {
            LeaveBalanceRequest request = EmptyRequest();
            var oneDate = new DateTime(2026, 1, 1);
            request.WorkedDates = Enumerable.Repeat(oneDate, 17).ToList();

            PTOBalance pto = LeaveBalanceCalculator.Calculate(request).PTO;

            Assert.That(pto.DaysWorked, Is.EqualTo(1));
            Assert.That(pto.DaysAccrued, Is.EqualTo(0));
        }

        [Test]
        public void TimesheetsBeforeTheOpeningBalanceDateAreIgnoredByAccrual()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.OpeningBalanceAsAt = new DateTime(2026, 1, 10);
            List<DateTime> worked = WorkedDays(new DateTime(2026, 1, 1), 17);
            request.WorkedDates = worked;

            PTOBalance pto = LeaveBalanceCalculator.Calculate(request).PTO;

            // Only 2026-01-10 .. 2026-01-17 (8 days) fall on or after the cutoff.
            Assert.That(pto.DaysWorked, Is.EqualTo(8));
            Assert.That(pto.DaysAccrued, Is.EqualTo(0));
        }

        [Test]
        public void OpeningBalanceDateDefaultsToContractStartDateWhenNull()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.ContractStartDate = new DateTime(2026, 1, 10);
            request.OpeningBalanceAsAt = null;
            request.WorkedDates = WorkedDays(new DateTime(2026, 1, 1), 17);

            PTOBalance pto = LeaveBalanceCalculator.Calculate(request).PTO;

            Assert.That(pto.DaysWorked, Is.EqualTo(8));
        }

        [Test]
        public void AnEmployeeWithNoContractRowStillAccruesFromEveryWorkedDate()
        {
            // A caller with no contract row falls back to DateTime.MinValue rather than today,
            // since an employee with no contract can still have real Timesheets behind them
            // (Employee.IsEmployedOn treats no contract as always employed) and today's date
            // would otherwise cut every one of those worked dates out of accrual.
            LeaveBalanceRequest request = EmptyRequest();
            request.ContractStartDate = DateTime.MinValue;
            request.OpeningBalanceAsAt = null;
            request.WorkedDates = WorkedDays(new DateTime(2026, 1, 1), 17);

            PTOBalance pto = LeaveBalanceCalculator.Calculate(request).PTO;

            Assert.That(pto.DaysWorked, Is.EqualTo(17));
            Assert.That(pto.DaysAccrued, Is.EqualTo(1));
        }

        [Test]
        public void OpeningBalanceFeedsDirectlyIntoTheBalance()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.OpeningPTODays = 5;

            PTOBalance pto = LeaveBalanceCalculator.Calculate(request).PTO;

            Assert.That(pto.OpeningBalance, Is.EqualTo(5));
            Assert.That(pto.Balance, Is.EqualTo(5));
        }

        [Test]
        public void DaysTakenReduceTheBalance()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.OpeningPTODays = 10;
            request.Absences = new List<Absence> { Pto(new DateTime(2026, 2, 1), new DateTime(2026, 2, 3)) };

            PTOBalance pto = LeaveBalanceCalculator.Calculate(request).PTO;

            Assert.That(pto.DaysTaken, Is.EqualTo(3));
            Assert.That(pto.Balance, Is.EqualTo(7));
        }

        [Test]
        public void DaysTakenPastTheOpeningBalanceLeaveItNegativeRatherThanClampedAtZero()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.OpeningPTODays = 2;
            request.Absences = new List<Absence> { Pto(new DateTime(2026, 2, 1), new DateTime(2026, 2, 5)) };

            PTOBalance pto = LeaveBalanceCalculator.Calculate(request).PTO;

            Assert.That(pto.DaysTaken, Is.EqualTo(5));
            Assert.That(pto.Balance, Is.EqualTo(-3));
        }

        [Test]
        public void AvailableBalanceReadsPTOAndSickFromTheirOwnBalances()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.OpeningPTODays = 5;
            request.Absences = new List<Absence> { Sick(new DateTime(2026, 2, 1), new DateTime(2026, 2, 2)) };

            LeaveBalanceSummary summary = LeaveBalanceCalculator.Calculate(request);

            Assert.That(summary.AvailableBalance(LeaveType.PTO), Is.EqualTo(5));
            Assert.That(summary.AvailableBalance(LeaveType.Sick), Is.EqualTo(28));
        }

        [Test]
        public void AvailableBalanceIsNullForLeaveTypesWithNoBalanceToExceed()
        {
            LeaveBalanceSummary summary = LeaveBalanceCalculator.Calculate(EmptyRequest());

            Assert.That(summary.AvailableBalance(LeaveType.Maternity), Is.Null);
            Assert.That(summary.AvailableBalance(LeaveType.AWOL), Is.Null);
        }

        [Test]
        public void AccrualIsCappedAtTwentyOneDays()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.OpeningPTODays = 21;
            request.WorkedDates = WorkedDays(new DateTime(2026, 1, 1), 17);

            PTOBalance pto = LeaveBalanceCalculator.Calculate(request).PTO;

            Assert.That(pto.Balance, Is.EqualTo(21));
            Assert.That(pto.DaysAccrued, Is.EqualTo(0));
            Assert.That(pto.IsAtCap, Is.True);
        }

        [Test]
        public void AccrualHaltsAtTheCapAndResumesOnceLeaveIsTaken()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.OpeningPTODays = 21;

            // 17 more days worked while capped earn nothing; a 3-day Absence taken in the
            // middle of them frees room, and the days worked after it earn one more day.
            var worked = new List<DateTime>();
            worked.AddRange(WorkedDays(new DateTime(2026, 1, 1), 8));
            worked.AddRange(WorkedDays(new DateTime(2026, 2, 1), 9));
            request.WorkedDates = worked;
            request.Absences = new List<Absence> { Pto(new DateTime(2026, 1, 20), new DateTime(2026, 1, 22)) };

            PTOBalance pto = LeaveBalanceCalculator.Calculate(request).PTO;

            // Balance drops to 18 after the 3-day Absence, then 17 worked days accrue one day back.
            Assert.That(pto.DaysAccrued, Is.EqualTo(1));
            Assert.That(pto.Balance, Is.EqualTo(19));
            Assert.That(pto.IsAtCap, Is.False);
        }

        [Test]
        public void SickShowsThirtyDaysRemainingWhenNoneAreTaken()
        {
            LeaveBalanceRequest request = EmptyRequest();

            SickBalance sick = LeaveBalanceCalculator.Calculate(request).Sick;

            Assert.That(sick.DaysRemaining, Is.EqualTo(30));
        }

        [Test]
        public void SickDaysTakenInTheCurrentCycleReduceTheBalance()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.AsOf = new DateTime(2021, 6, 1);
            request.Absences = new List<Absence> { Sick(new DateTime(2021, 5, 1), new DateTime(2021, 5, 5)) };

            SickBalance sick = LeaveBalanceCalculator.Calculate(request).Sick;

            Assert.That(sick.DaysRemaining, Is.EqualTo(25));
        }

        [Test]
        public void SickDaysTakenInAPreviousCycleDoNotReduceTheCurrentBalance()
        {
            LeaveBalanceRequest request = EmptyRequest();
            // Cycle 1 runs 2020-01-01 .. 2022-12-31; cycle 2 runs 2023-01-01 .. 2025-12-31.
            request.AsOf = new DateTime(2023, 3, 1);
            request.Absences = new List<Absence> { Sick(new DateTime(2021, 5, 1), new DateTime(2021, 5, 10)) };

            SickBalance sick = LeaveBalanceCalculator.Calculate(request).Sick;

            Assert.That(sick.DaysRemaining, Is.EqualTo(30));
        }

        [Test]
        public void TheSickLeaveCycleEndDateIsThirtySixMonthsFromContractStart()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.AsOf = new DateTime(2021, 6, 1);

            SickBalance sick = LeaveBalanceCalculator.Calculate(request).Sick;

            Assert.That(sick.CycleEndDate, Is.EqualTo(new DateTime(2022, 12, 31)));
        }

        [Test]
        public void TheSickLeaveCycleRollsOverEveryThirtySixMonths()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.AsOf = new DateTime(2023, 3, 1);

            SickBalance sick = LeaveBalanceCalculator.Calculate(request).Sick;

            Assert.That(sick.CycleEndDate, Is.EqualTo(new DateTime(2025, 12, 31)));
        }

        [Test]
        public void MaternityShowsATallyOfDaysTakenWithNoBalanceConcept()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.Absences = new List<Absence>
            {
                new Absence { LeaveType = LeaveType.Maternity, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 4, 10) }
            };

            LeaveTally maternity = LeaveBalanceCalculator.Calculate(request).Maternity;

            Assert.That(maternity.DaysTaken, Is.EqualTo(100));
        }

        [Test]
        public void AwolShowsATallyOfDaysTaken()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.Absences = new List<Absence>
            {
                new Absence { LeaveType = LeaveType.AWOL, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 1, 1) },
                new Absence { LeaveType = LeaveType.AWOL, StartDate = new DateTime(2025, 2, 1), EndDate = new DateTime(2025, 2, 2) }
            };

            LeaveTally awol = LeaveBalanceCalculator.Calculate(request).AWOL;

            Assert.That(awol.DaysTaken, Is.EqualTo(3));
        }

        [Test]
        public void PtoAndSickAbsencesDoNotAffectEachOthersFigures()
        {
            LeaveBalanceRequest request = EmptyRequest();
            request.OpeningPTODays = 10;
            request.Absences = new List<Absence>
            {
                Pto(new DateTime(2026, 1, 5), new DateTime(2026, 1, 6)),
                Sick(new DateTime(2026, 1, 10), new DateTime(2026, 1, 10))
            };

            LeaveBalanceSummary summary = LeaveBalanceCalculator.Calculate(request);

            Assert.That(summary.PTO.DaysTaken, Is.EqualTo(2));
            Assert.That(summary.PTO.Balance, Is.EqualTo(8));
            Assert.That(summary.Sick.DaysRemaining, Is.EqualTo(29));
        }
    }
}
