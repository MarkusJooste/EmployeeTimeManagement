using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class TimesheetSignOffTests
    {
        // Monday 2 to Sunday 8 November 2026.
        private static readonly DateTime Monday = new DateTime(2026, 11, 2);
        private static readonly DateTime Sunday = new DateTime(2026, 11, 8);

        private static Timesheet Worked(DateTime date, string timeIn, string timeOut,
            string break1Start = null, string break1End = null, DayType dayType = DayType.Normal, int employeeID = 1)
        {
            return new Timesheet
            {
                TimesheetID = date.Day,
                EmployeeID = employeeID,
                WorkDate = date,
                TimeIn = TimeSpan.Parse(timeIn),
                TimeOut = TimeSpan.Parse(timeOut),
                Break1Start = break1Start == null ? (TimeSpan?)null : TimeSpan.Parse(break1Start),
                Break1End = break1End == null ? (TimeSpan?)null : TimeSpan.Parse(break1End),
                DayType = dayType,
                Status = TimesheetStatus.Worked
            };
        }

        private static Timesheet Absent(DateTime date, TimesheetStatus status, string notes = null)
        {
            return new Timesheet { EmployeeID = 1, WorkDate = date, Status = status, Notes = notes };
        }

        private static SignOffSheet Build(params Timesheet[] timesheets)
        {
            return TimesheetSignOff.Build(1, "Thandi Mokoena", "Paarl Mall", Monday, Sunday, timesheets, Sunday);
        }

        [Test]
        public void EveryDayOfThePeriodIsListedEvenWithNothingCaptured()
        {
            SignOffSheet sheet = Build(Worked(Monday, "08:00", "16:00"));

            Assert.That(sheet.Lines.Select(line => line.Date),
                Is.EqualTo(Enumerable.Range(0, 7).Select(day => Monday.AddDays(day))));
            Assert.That(sheet.Lines.Count(line => line.IsMissing), Is.EqualTo(6));
            Assert.That(sheet.MissingDays, Is.EqualTo(6));
            Assert.That(sheet.Lines[1].Status, Is.EqualTo("Not captured"));
        }

        [Test]
        public void DaysAfterTodayAreBlankRatherThanMissing()
        {
            // Printed on the Wednesday of the week.
            SignOffSheet sheet = TimesheetSignOff.Build(1, "Thandi Mokoena", "Paarl Mall", Monday, Sunday,
                new[] { Worked(Monday, "08:00", "16:00") }, Monday.AddDays(2));

            Assert.That(sheet.Lines.Count, Is.EqualTo(7));
            Assert.That(sheet.MissingDays, Is.EqualTo(2));
            Assert.That(sheet.Lines.Skip(3).All(line => !line.IsMissing && line.Status.Length == 0), Is.True);
        }

        [Test]
        public void HoursAreTimeAtWorkLessBreaks()
        {
            SignOffSheet sheet = Build(Worked(Monday, "08:00", "16:30", "12:00", "12:45"));

            Assert.That(sheet.Lines[0].Hours, Is.EqualTo(7.75m));
            Assert.That(sheet.Lines[0].TimeIn, Is.EqualTo("08:00"));
            Assert.That(sheet.Lines[0].TimeOut, Is.EqualTo("16:30"));
            Assert.That(sheet.Lines[0].Breaks, Is.EqualTo("12:00-12:45"));
            Assert.That(sheet.TotalHours, Is.EqualTo(7.75m));
            Assert.That(sheet.DaysWorked, Is.EqualTo(1));
        }

        [Test]
        public void BothBreaksAreDescribedAndTakenOff()
        {
            Timesheet timesheet = Worked(Monday, "08:00", "17:00", "10:00", "10:15");
            timesheet.Break2Start = TimeSpan.Parse("13:00");
            timesheet.Break2End = TimeSpan.Parse("13:30");

            Assert.That(TimesheetSignOff.DescribeBreaks(timesheet), Is.EqualTo("10:00-10:15, 13:00-13:30"));
            Assert.That(TimesheetSignOff.WorkedHours(timesheet), Is.EqualTo(8.25m));
        }

        [Test]
        public void SundaysAndPublicHolidaysEarnTheSamePremiumsAsTheSummary()
        {
            SignOffSheet sheet = Build(
                Worked(Monday, "08:00", "16:00"),
                Worked(Monday.AddDays(1), "08:00", "12:00", dayType: DayType.PublicHoliday),
                Worked(Sunday, "09:00", "13:00", dayType: DayType.Sunday));

            Assert.That(sheet.TotalHours, Is.EqualTo(16m));
            Assert.That(sheet.SundayExtraHours, Is.EqualTo(2m));
            Assert.That(sheet.HolidayExtraHours, Is.EqualTo(4m));
            Assert.That(sheet.PayableHours, Is.EqualTo(22m));
            Assert.That(sheet.Lines.Last().Day, Is.EqualTo("Sunday"));
            Assert.That(sheet.Lines[1].Day, Is.EqualTo("Public holiday"));
            Assert.That(sheet.Lines[0].Day, Is.Empty);
        }

        [Test]
        public void LeaveOffAndAwolDaysAreCountedWithoutHours()
        {
            SignOffSheet sheet = Build(
                Absent(Monday, TimesheetStatus.Leave, "Annual leave"),
                Absent(Monday.AddDays(1), TimesheetStatus.Off),
                Absent(Monday.AddDays(2), TimesheetStatus.AWOL));

            Assert.That(sheet.LeaveDays, Is.EqualTo(1));
            Assert.That(sheet.OffDays, Is.EqualTo(1));
            Assert.That(sheet.AwolDays, Is.EqualTo(1));
            Assert.That(sheet.DaysWorked, Is.EqualTo(0));
            Assert.That(sheet.TotalHours, Is.EqualTo(0m));
            Assert.That(sheet.Lines[0].Hours, Is.Null);
            Assert.That(sheet.Lines[0].Status, Is.EqualTo("Leave"));
            Assert.That(sheet.Lines[0].Notes, Is.EqualTo("Annual leave"));
        }

        [Test]
        public void TwoShiftsOnOneDayAreBothListedButCountAsOneDayWorked()
        {
            Timesheet morning = Worked(Monday, "07:00", "11:00");
            Timesheet evening = Worked(Monday, "17:00", "21:00");
            morning.TimesheetID = 10;
            evening.TimesheetID = 11;

            // Given out of order, to show the sheet sorts them by the time they started.
            SignOffSheet sheet = Build(evening, morning);
            List<SignOffLine> monday = sheet.Lines.Where(line => line.Date == Monday).ToList();

            Assert.That(monday.Select(line => line.TimeIn), Is.EqualTo(new[] { "07:00", "17:00" }));
            Assert.That(monday.Select(line => line.ShowDate), Is.EqualTo(new[] { true, false }));
            Assert.That(sheet.DaysWorked, Is.EqualTo(1));
            Assert.That(sheet.TotalHours, Is.EqualTo(8m));
        }

        [Test]
        public void AnotherEmployeesTimesheetsAreIgnored()
        {
            SignOffSheet sheet = Build(
                Worked(Monday, "08:00", "16:00"),
                Worked(Monday.AddDays(1), "08:00", "16:00", employeeID: 2));

            Assert.That(sheet.TotalHours, Is.EqualTo(8m));
            Assert.That(sheet.Lines[1].IsMissing, Is.True);
        }

        [Test]
        public void ShiftWithBreaksLongerThanTheShiftCountsAsZeroNotNegative()
        {
            Assert.That(TimesheetSignOff.WorkedHours(Worked(Monday, "08:00", "09:00", "08:00", "10:00")), Is.EqualTo(0m));
        }

        [Test]
        public void HoursFormatAsAClockReading()
        {
            Assert.That(TimesheetSignOff.FormatHours(7.75m), Is.EqualTo("7:45"));
            Assert.That(TimesheetSignOff.FormatHours(38m), Is.EqualTo("38:00"));
            Assert.That(TimesheetSignOff.FormatHours(0.5m), Is.EqualTo("0:30"));
        }
    }
}
