using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeTimeManagement.Models
{
    // Everything the Leave Balances panel needs to compute one employee's balances, gathered
    // from Timesheets and Absences up front so the calculator itself touches no database.
    public class LeaveBalanceRequest
    {
        public DateTime AsOf { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime? OpeningBalanceAsAt { get; set; }
        public int OpeningPTODays { get; set; }

        // Every date with a Timesheet whose Status is 'Worked'. Hours and Day Type play no
        // part, and a date may repeat in the source data but is only ever counted once.
        public IEnumerable<DateTime> WorkedDates { get; set; } = new List<DateTime>();

        // Every Absence on record for the employee, any Leave Type, since balances are
        // calculated from Absence ranges alone (ADR-0001).
        public IEnumerable<Absence> Absences { get; set; } = new List<Absence>();
    }

    // PTO shown as the arithmetic that produced it, not just the answer, so the balance can
    // be defended to the person it belongs to.
    public class PTOBalance
    {
        public int DaysWorked { get; set; }
        public int DaysAccrued { get; set; }
        public int OpeningBalance { get; set; }
        public int DaysTaken { get; set; }
        public int Balance { get; set; }
        public bool IsAtCap { get; set; }
    }

    public class SickBalance
    {
        public int DaysRemaining { get; set; }
        public DateTime CycleEndDate { get; set; }
    }

    // A running total with no balance behind it, for a Leave Type that does not accrue.
    public class LeaveTally
    {
        public int DaysTaken { get; set; }
    }

    public class LeaveBalanceSummary
    {
        public PTOBalance PTO { get; set; }
        public SickBalance Sick { get; set; }
        public LeaveTally Maternity { get; set; }
        public LeaveTally AWOL { get; set; }

        // The Leave Balance a booking of this Leave Type would draw from, or null when the
        // Leave Type carries no balance to exceed - Maternity and AWOL are tallies only.
        public int? AvailableBalance(LeaveType leaveType)
        {
            switch (leaveType)
            {
                case LeaveType.PTO:
                    return PTO.Balance;
                case LeaveType.Sick:
                    return Sick.DaysRemaining;
                default:
                    return null;
            }
        }
    }

    // Turns Timesheets and Absences into every Leave Balance the Leave Balances panel shows,
    // matching the accrual, cap and cycle rules in ADR-0003.
    public static class LeaveBalanceCalculator
    {
        private const int DaysWorkedPerAccrual = 17;
        private const int PTOCap = 21;
        private const int SickDaysPerCycle = 30;
        private const int SickCycleMonths = 36;

        public static LeaveBalanceSummary Calculate(LeaveBalanceRequest request)
        {
            List<Absence> absences = (request.Absences ?? new List<Absence>()).ToList();

            return new LeaveBalanceSummary
            {
                PTO = CalculatePTO(request, absences),
                Sick = CalculateSick(request, absences),
                Maternity = CalculateTally(absences, LeaveType.Maternity),
                AWOL = CalculateTally(absences, LeaveType.AWOL)
            };
        }

        // Replays worked days and PTO taken in date order, so accrual genuinely halts while
        // the balance sits at the cap and resumes only once a later Absence frees room under it.
        private static PTOBalance CalculatePTO(LeaveBalanceRequest request, List<Absence> absences)
        {
            DateTime cutoff = (request.OpeningBalanceAsAt ?? request.ContractStartDate).Date;

            List<DateTime> workedDates = (request.WorkedDates ?? new List<DateTime>())
                .Select(date => date.Date)
                .Distinct()
                .Where(date => date >= cutoff)
                .ToList();

            List<Absence> pto = absences.Where(absence => absence.LeaveType == LeaveType.PTO).ToList();
            int daysTaken = pto.Sum(absence => absence.DayCount);

            List<LedgerEvent> events = BuildLedger(workedDates, pto);

            int balance = request.OpeningPTODays;
            int workedSinceAccrual = 0;
            int daysAccrued = 0;

            foreach (LedgerEvent ledgerEvent in events)
            {
                if (ledgerEvent.IsWorkedDay)
                {
                    workedSinceAccrual++;

                    if (workedSinceAccrual == DaysWorkedPerAccrual)
                    {
                        workedSinceAccrual = 0;

                        if (balance < PTOCap)
                        {
                            balance++;
                            daysAccrued++;
                        }
                    }
                }
                else
                {
                    balance -= ledgerEvent.DaysTaken;
                }
            }

            return new PTOBalance
            {
                DaysWorked = workedDates.Count,
                DaysAccrued = daysAccrued,
                OpeningBalance = request.OpeningPTODays,
                DaysTaken = daysTaken,
                Balance = balance,
                IsAtCap = balance >= PTOCap
            };
        }

        // One day worked, or one Absence's worth of days taken, ordered so the simulation can
        // walk them chronologically. Ties put a taken Absence before a worked day, so a day
        // that both frees the cap and completes an accrual cycle frees it first.
        private struct LedgerEvent
        {
            public DateTime Date;
            public bool IsWorkedDay;
            public int DaysTaken;
        }

        private static List<LedgerEvent> BuildLedger(List<DateTime> workedDates, List<Absence> pto)
        {
            var events = new List<LedgerEvent>();

            foreach (DateTime date in workedDates)
            {
                events.Add(new LedgerEvent { Date = date, IsWorkedDay = true });
            }

            foreach (Absence absence in pto)
            {
                events.Add(new LedgerEvent { Date = absence.StartDate.Date, IsWorkedDay = false, DaysTaken = absence.DayCount });
            }

            return events
                .OrderBy(evt => evt.Date)
                .ThenBy(evt => evt.IsWorkedDay)
                .ToList();
        }

        // The current Sick Leave Cycle: 36 months from the contract start date, repeating,
        // so a cycle that has already rolled over is found without ever reaching into the future.
        private static SickBalance CalculateSick(LeaveBalanceRequest request, List<Absence> absences)
        {
            DateTime cycleStart = request.ContractStartDate.Date;
            DateTime asOf = request.AsOf.Date;

            while (cycleStart.AddMonths(SickCycleMonths) <= asOf)
            {
                cycleStart = cycleStart.AddMonths(SickCycleMonths);
            }

            DateTime cycleEnd = cycleStart.AddMonths(SickCycleMonths).AddDays(-1);

            int daysTakenThisCycle = absences
                .Where(absence => absence.LeaveType == LeaveType.Sick)
                .Where(absence => absence.StartDate.Date >= cycleStart && absence.StartDate.Date <= cycleEnd)
                .Sum(absence => absence.DayCount);

            return new SickBalance
            {
                DaysRemaining = SickDaysPerCycle - daysTakenThisCycle,
                CycleEndDate = cycleEnd
            };
        }

        // Maternity and AWOL: a lifetime total of days taken, since neither accrues or resets.
        private static LeaveTally CalculateTally(List<Absence> absences, LeaveType leaveType)
        {
            return new LeaveTally
            {
                DaysTaken = absences.Where(absence => absence.LeaveType == leaveType).Sum(absence => absence.DayCount)
            };
        }
    }
}
