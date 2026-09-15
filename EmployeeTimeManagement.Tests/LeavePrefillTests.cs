using System;
using System.Collections.Generic;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class LeavePrefillTests
    {
        private static readonly DateTime WorkDate = new DateTime(2026, 3, 10);

        private static Absence Build(int employeeID, LeaveType leaveType, DateTime start, DateTime end)
        {
            return new Absence
            {
                EmployeeID = employeeID,
                LeaveType = leaveType,
                StartDate = start,
                EndDate = end
            };
        }

        [Test]
        public void AnEmployeeWhoseAbsenceCoversTheDayIsPrefilled()
        {
            var absences = new List<Absence> { Build(1, LeaveType.PTO, WorkDate, WorkDate) };

            List<int> result = LeavePrefill.EmployeeIDsToPrefill(WorkDate, absences, new int[0]);

            Assert.That(result, Is.EquivalentTo(new[] { 1 }));
        }

        [Test]
        public void AnEmployeeWithNoAbsenceCoveringTheDayIsUnaffected()
        {
            var absences = new List<Absence>
            {
                Build(1, LeaveType.PTO, WorkDate.AddDays(1), WorkDate.AddDays(5))
            };

            List<int> result = LeavePrefill.EmployeeIDsToPrefill(WorkDate, absences, new int[0]);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void AnEmployeeWithASavedTimesheetIsNeverPrefilled()
        {
            var absences = new List<Absence> { Build(1, LeaveType.PTO, WorkDate, WorkDate) };

            List<int> result = LeavePrefill.EmployeeIDsToPrefill(WorkDate, absences, new[] { 1 });

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void AnAwolAbsenceNeverPrefillsAnything()
        {
            var absences = new List<Absence> { Build(1, LeaveType.AWOL, WorkDate, WorkDate) };

            List<int> result = LeavePrefill.EmployeeIDsToPrefill(WorkDate, absences, new int[0]);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void NoAbsencesAtAllPrefillsNothing()
        {
            List<int> result = LeavePrefill.EmployeeIDsToPrefill(WorkDate, new List<Absence>(), new int[0]);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void AMultiDayAbsenceCoversEveryDayInItsRangeInclusive()
        {
            var absences = new List<Absence>
            {
                Build(1, LeaveType.Sick, WorkDate.AddDays(-2), WorkDate.AddDays(2))
            };

            List<int> result = LeavePrefill.EmployeeIDsToPrefill(WorkDate, absences, new int[0]);

            Assert.That(result, Is.EquivalentTo(new[] { 1 }));
        }

        [Test]
        public void EachEmployeeAppearsAtMostOnceEvenWithOverlappingAbsences()
        {
            var absences = new List<Absence>
            {
                Build(1, LeaveType.PTO, WorkDate.AddDays(-1), WorkDate.AddDays(1)),
                Build(1, LeaveType.Sick, WorkDate, WorkDate)
            };

            List<int> result = LeavePrefill.EmployeeIDsToPrefill(WorkDate, absences, new int[0]);

            Assert.That(result, Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void SeveralEmployeesOnLeaveTheSameDayAreAllPrefilled()
        {
            var absences = new List<Absence>
            {
                Build(1, LeaveType.PTO, WorkDate, WorkDate),
                Build(2, LeaveType.Maternity, WorkDate, WorkDate)
            };

            List<int> result = LeavePrefill.EmployeeIDsToPrefill(WorkDate, absences, new int[0]);

            Assert.That(result, Is.EquivalentTo(new[] { 1, 2 }));
        }
    }
}
