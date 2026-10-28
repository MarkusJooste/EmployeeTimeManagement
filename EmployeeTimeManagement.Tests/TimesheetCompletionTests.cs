using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class TimesheetCompletionTests
    {
        private static readonly DateTime WorkDate = new DateTime(2026, 10, 28);

        private static EmployeeDayStatus Build(int employeeID, string name, bool hasTimesheet,
            DateTime? contractStart = null, DateTime? contractEnd = null, string store = "Paarl Mall")
        {
            return new EmployeeDayStatus
            {
                EmployeeID = employeeID,
                StoreName = store,
                Name = name,
                Surname = "Surname" + employeeID,
                HasTimesheet = hasTimesheet,
                ContractStartDate = contractStart,
                ContractEndDate = contractEnd
            };
        }

        private static List<int> MissingIDs(params EmployeeDayStatus[] employees)
        {
            return TimesheetCompletion.MissingOn(WorkDate, employees).Select(e => e.EmployeeID).ToList();
        }

        [Test]
        public void AnEmployeeWithoutATimesheetIsMissing()
        {
            Assert.That(MissingIDs(Build(1, "Thandi", false)), Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void AnyCapturedTimesheetCountsWhateverItsStatus()
        {
            Assert.That(MissingIDs(Build(1, "Thandi", true)), Is.Empty);
        }

        [Test]
        public void AnEmployeeNotYetStartedIsNotMissing()
        {
            Assert.That(MissingIDs(Build(1, "Thandi", false, WorkDate.AddDays(1))), Is.Empty);
        }

        [Test]
        public void AnEmployeeWhoseContractEndedIsNotMissing()
        {
            Assert.That(MissingIDs(Build(1, "Thandi", false, WorkDate.AddYears(-1), WorkDate.AddDays(-1))), Is.Empty);
        }

        [Test]
        public void ContractStartAndEndDaysCountAsEmployed()
        {
            Assert.That(MissingIDs(
                Build(1, "Starts today", false, WorkDate),
                Build(2, "Ends today", false, WorkDate.AddYears(-1), WorkDate)), Is.EquivalentTo(new[] { 1, 2 }));
        }

        [Test]
        public void AnEmployeeWithNoContractRowIsTreatedAsEmployed()
        {
            Assert.That(MissingIDs(Build(1, "Thandi", false, null, null)), Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void AnEmployeeOnLeaveStillNeedsTheirTimesheetSaved()
        {
            EmployeeDayStatus onLeave = Build(1, "Thandi", false);
            onLeave.IsOnLeave = true;

            Assert.That(MissingIDs(onLeave), Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void ListedByStoreThenSurnameThenName()
        {
            var employees = new[]
            {
                new EmployeeDayStatus { EmployeeID = 1, StoreName = "Wellington", Name = "Anna", Surname = "Botha" },
                new EmployeeDayStatus { EmployeeID = 2, StoreName = "Paarl Mall", Name = "Zola", Surname = "Mokoena" },
                new EmployeeDayStatus { EmployeeID = 3, StoreName = "Paarl Mall", Name = "Ben", Surname = "Adams" },
                new EmployeeDayStatus { EmployeeID = 4, StoreName = "Paarl Mall", Name = "Amy", Surname = "Adams" }
            };

            List<int> order = TimesheetCompletion.MissingOn(WorkDate, employees).Select(e => e.EmployeeID).ToList();

            Assert.That(order, Is.EqualTo(new[] { 4, 3, 2, 1 }));
        }
    }
}
