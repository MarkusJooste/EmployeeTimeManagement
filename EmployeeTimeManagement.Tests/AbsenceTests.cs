using System;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class AbsenceTests
    {
        [Test]
        public void ASingleDayAbsenceCostsOneDay()
        {
            var absence = Build(new DateTime(2026, 3, 10), new DateTime(2026, 3, 10));

            Assert.That(absence.DayCount, Is.EqualTo(1));
        }

        [Test]
        public void ADayCountIncludesBothEndsOfTheRange()
        {
            var absence = Build(new DateTime(2026, 3, 10), new DateTime(2026, 3, 14));

            Assert.That(absence.DayCount, Is.EqualTo(5));
        }

        [Test]
        public void ADayCountIncludesAnySundaysInTheRange()
        {
            // 9 March 2026 is a Monday, so this range spans one Sunday (15 March).
            var absence = Build(new DateTime(2026, 3, 9), new DateTime(2026, 3, 15));

            Assert.That(absence.DayCount, Is.EqualTo(7));
        }

        [Test]
        public void ADayCountIncludesAnyPublicHolidaysInTheRange()
        {
            // Human Rights Day, 21 March, falls inside this range and is not excluded.
            var absence = Build(new DateTime(2026, 3, 19), new DateTime(2026, 3, 23));

            Assert.That(absence.DayCount, Is.EqualTo(5));
        }

        [Test]
        public void AnAbsenceWithNoOverrideReasonIsNotMarkedOverridden()
        {
            var absence = Build(new DateTime(2026, 3, 10), new DateTime(2026, 3, 10));

            Assert.That(absence.OverriddenDisplay, Is.Empty);
        }

        [Test]
        public void AnAbsenceWithAnOverrideReasonIsMarkedOverridden()
        {
            var absence = Build(new DateTime(2026, 3, 10), new DateTime(2026, 3, 10));
            absence.OverrideReason = "Special arrangement with the owner";

            Assert.That(absence.OverriddenDisplay, Is.EqualTo("Overridden"));
        }

        private static Absence Build(DateTime startDate, DateTime endDate)
        {
            return new Absence
            {
                LeaveID = 1,
                EmployeeID = 1,
                LeaveType = LeaveType.PTO,
                StartDate = startDate,
                EndDate = endDate,
                Reason = "Annual leave"
            };
        }
    }
}
