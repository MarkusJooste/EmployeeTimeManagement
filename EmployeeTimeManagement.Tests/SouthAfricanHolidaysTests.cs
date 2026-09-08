using System;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class SouthAfricanHolidaysTests
    {
        // 2025 is the reference year throughout. Note that Freedom Day 2025 falls on a
        // Sunday, which the observance tests below rely on.
        [TestCase(2025, 1, 1, TestName = "New Year's Day")]
        [TestCase(2025, 3, 21, TestName = "Human Rights Day")]
        [TestCase(2025, 4, 27, TestName = "Freedom Day")]
        [TestCase(2025, 5, 1, TestName = "Workers' Day")]
        [TestCase(2025, 6, 16, TestName = "Youth Day")]
        [TestCase(2025, 8, 9, TestName = "National Women's Day")]
        [TestCase(2025, 9, 24, TestName = "Heritage Day")]
        [TestCase(2025, 12, 16, TestName = "Day of Reconciliation")]
        [TestCase(2025, 12, 25, TestName = "Christmas Day")]
        [TestCase(2025, 12, 26, TestName = "Day of Goodwill")]
        public void FixedDateHolidaysAreRecognised(int year, int month, int day)
        {
            Assert.That(SouthAfricanHolidays.IsPublicHoliday(new DateTime(year, month, day)), Is.True);
        }

        [TestCase(2024, 3, 29, TestName = "Good Friday 2024")]
        [TestCase(2025, 4, 18, TestName = "Good Friday 2025")]
        [TestCase(2026, 4, 3, TestName = "Good Friday 2026")]
        [TestCase(2027, 3, 26, TestName = "Good Friday 2027 (early Easter)")]
        [TestCase(2038, 4, 23, TestName = "Good Friday 2038 (late Easter)")]
        public void GoodFridayIsRecognised(int year, int month, int day)
        {
            Assert.That(SouthAfricanHolidays.IsPublicHoliday(new DateTime(year, month, day)), Is.True);
        }

        [TestCase(2024, 4, 1, TestName = "Family Day 2024")]
        [TestCase(2025, 4, 21, TestName = "Family Day 2025")]
        [TestCase(2026, 4, 6, TestName = "Family Day 2026")]
        [TestCase(2027, 3, 29, TestName = "Family Day 2027 (early Easter)")]
        [TestCase(2038, 4, 26, TestName = "Family Day 2038 (late Easter)")]
        public void FamilyDayIsRecognised(int year, int month, int day)
        {
            Assert.That(SouthAfricanHolidays.IsPublicHoliday(new DateTime(year, month, day)), Is.True);
        }

        [Test]
        public void MondayAfterAHolidayFallingOnASundayIsAlsoAHoliday()
        {
            // 27 April 2025 (Freedom Day) is a Sunday, so 28 April 2025 is a public holiday.
            Assert.That(new DateTime(2025, 4, 27).DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
            Assert.That(SouthAfricanHolidays.IsPublicHoliday(new DateTime(2025, 4, 28)), Is.True);
        }

        [Test]
        public void MondayAfterAHolidayFallingOnASaturdayIsNotAHoliday()
        {
            // 16 June 2029 is a Saturday; the observance rule covers Sundays only.
            Assert.That(new DateTime(2029, 6, 16).DayOfWeek, Is.EqualTo(DayOfWeek.Saturday));
            Assert.That(SouthAfricanHolidays.IsPublicHoliday(new DateTime(2029, 6, 18)), Is.False);
        }

        [Test]
        public void ChristmasOnASundayMakesTheTwentySeventhAHoliday()
        {
            // 25 December 2022 was a Sunday. The 26th is already the Day of Goodwill,
            // so the observed holiday lands on the 27th.
            Assert.That(new DateTime(2022, 12, 25).DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
            Assert.That(SouthAfricanHolidays.IsPublicHoliday(new DateTime(2022, 12, 26)), Is.True);
            Assert.That(SouthAfricanHolidays.IsPublicHoliday(new DateTime(2022, 12, 27)), Is.True);
        }

        [Test]
        public void OrdinaryWeekdayIsNotAHoliday()
        {
            Assert.That(SouthAfricanHolidays.IsPublicHoliday(new DateTime(2025, 9, 8)), Is.False);
        }

        [Test]
        public void OrdinarySundayIsNotAHoliday()
        {
            DateTime date = new DateTime(2025, 9, 7);
            Assert.That(date.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
            Assert.That(SouthAfricanHolidays.IsPublicHoliday(date), Is.False);
        }

        [Test]
        public void OrdinaryWeekdayIsANormalDayType()
        {
            Assert.That(SouthAfricanHolidays.GetDayType(new DateTime(2025, 9, 8)), Is.EqualTo(DayType.Normal));
        }

        [Test]
        public void OrdinarySundayIsASundayDayType()
        {
            Assert.That(SouthAfricanHolidays.GetDayType(new DateTime(2025, 9, 7)), Is.EqualTo(DayType.Sunday));
        }

        [Test]
        public void WeekdayHolidayIsAPublicHolidayDayType()
        {
            Assert.That(SouthAfricanHolidays.GetDayType(new DateTime(2025, 6, 16)), Is.EqualTo(DayType.PublicHoliday));
        }

        [Test]
        public void HolidayFallingOnASundayIsAPublicHolidayNotASunday()
        {
            // Freedom Day 2025 falls on a Sunday. The holiday premium is the higher of
            // the two, so Public Holiday wins.
            Assert.That(SouthAfricanHolidays.GetDayType(new DateTime(2025, 4, 27)), Is.EqualTo(DayType.PublicHoliday));
        }

        [Test]
        public void ObservedMondayAfterASundayHolidayIsAPublicHolidayDayType()
        {
            Assert.That(SouthAfricanHolidays.GetDayType(new DateTime(2025, 4, 28)), Is.EqualTo(DayType.PublicHoliday));
        }
    }
}
