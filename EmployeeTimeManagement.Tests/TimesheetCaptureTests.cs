using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class TimesheetCaptureTests
    {
        private const int CapturedBy = 7;

        // An ordinary Monday, so DayType defaults to Normal unless a test says otherwise.
        private static readonly DateTime WorkDate = new DateTime(2025, 9, 8);

        private static CaptureRow ValidRow(int employeeId = 1)
        {
            return new CaptureRow
            {
                RowIndex = 0,
                EmployeeID = employeeId,
                Status = "Worked",
                TimeIn = "08:00",
                TimeOut = "17:00"
            };
        }

        private static CaptureResult Build(params CaptureRow[] rows)
        {
            return TimesheetCapture.Build(WorkDate, CapturedBy, rows);
        }

        private static string ErrorFor(CaptureResult result, int rowIndex)
        {
            RowError error = result.Errors.FirstOrDefault(e => e.RowIndex == rowIndex);
            return error == null ? null : error.Message;
        }

        // --- Happy path ---

        [Test]
        public void AValidRowProducesAPersistableTimesheet()
        {
            CaptureResult result = Build(ValidRow());

            Assert.That(result.IsValid, Is.True, ErrorFor(result, 0));
            Assert.That(result.Valid, Has.Count.EqualTo(1));

            Timesheet timesheet = result.Valid[0];
            Assert.That(timesheet.EmployeeID, Is.EqualTo(1));
            Assert.That(timesheet.WorkDate, Is.EqualTo(WorkDate));
            Assert.That(timesheet.TimeIn, Is.EqualTo(new TimeSpan(8, 0, 0)));
            Assert.That(timesheet.TimeOut, Is.EqualTo(new TimeSpan(17, 0, 0)));
            Assert.That(timesheet.Status, Is.EqualTo(TimesheetStatus.Worked));
            Assert.That(timesheet.CapturedBy, Is.EqualTo(CapturedBy));
            Assert.That(timesheet.BusinessDate, Is.EqualTo(DateTime.Today));
        }

        [Test]
        public void SeveralValidRowsAllProduceTimesheets()
        {
            CaptureRow first = ValidRow(1);
            CaptureRow second = ValidRow(2);
            second.RowIndex = 1;

            CaptureResult result = Build(first, second);

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Valid.Select(t => t.EmployeeID), Is.EquivalentTo(new[] { 1, 2 }));
        }

        [Test]
        public void AShiftWithNoBreaksIsValid()
        {
            Assert.That(Build(ValidRow()).IsValid, Is.True);
        }

        [Test]
        public void AShiftWithOnlyAFirstBreakIsValid()
        {
            CaptureRow row = ValidRow();
            row.Break1Start = "12:00";
            row.Break1End = "12:30";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.True, ErrorFor(result, 0));
            Assert.That(result.Valid[0].Break1Start, Is.EqualTo(new TimeSpan(12, 0, 0)));
            Assert.That(result.Valid[0].Break2Start, Is.Null);
        }

        [Test]
        public void AShiftWithBothBreaksIsValid()
        {
            CaptureRow row = ValidRow();
            row.Break1Start = "10:00";
            row.Break1End = "10:15";
            row.Break2Start = "14:00";
            row.Break2End = "14:30";

            Assert.That(Build(row).IsValid, Is.True);
        }

        // --- Lenient time parsing ---

        [TestCase("8")]
        [TestCase("08")]
        [TestCase("800")]
        [TestCase("0800")]
        [TestCase("8:00")]
        [TestCase("08:00")]
        public void TimesAreAcceptedInSeveralFormats(string text)
        {
            CaptureRow row = ValidRow();
            row.TimeIn = text;

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.True, ErrorFor(result, 0));
            Assert.That(result.Valid[0].TimeIn, Is.EqualTo(new TimeSpan(8, 0, 0)));
        }

        [TestCase("8:45", 8, 45)]
        [TestCase("845", 8, 45)]
        [TestCase("0845", 8, 45)]
        [TestCase("13:05", 13, 5)]
        [TestCase("1305", 13, 5)]
        public void TimesWithMinutesParseCorrectly(string text, int hours, int minutes)
        {
            CaptureRow row = ValidRow();
            row.TimeIn = text;
            row.TimeOut = "23:00";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.True, ErrorFor(result, 0));
            Assert.That(result.Valid[0].TimeIn, Is.EqualTo(new TimeSpan(hours, minutes, 0)));
        }

        [TestCase("half eight")]
        [TestCase("25:00")]
        [TestCase("08:70")]
        [TestCase("99999")]
        [TestCase("-1")]
        public void UnparseableTimesAreARowError(string text)
        {
            CaptureRow row = ValidRow();
            row.TimeIn = text;

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 0), Does.Contain("TimeIn"));
        }

        [Test]
        public void TimesAreNormalisedForDisplay()
        {
            TimeSpan? parsed;
            Assert.That(TimesheetCapture.TryParseTime("8", out parsed), Is.True);
            Assert.That(TimesheetCapture.FormatTime(parsed), Is.EqualTo("08:00"));
        }

        // --- Day Type ---

        [Test]
        public void DayTypeDefaultsFromTheWorkDate()
        {
            Assert.That(Build(ValidRow()).Valid[0].DayType, Is.EqualTo(DayType.Normal));
        }

        [Test]
        public void DayTypeDefaultsToSundayOnASunday()
        {
            CaptureResult result = TimesheetCapture.Build(new DateTime(2025, 9, 7), CapturedBy, new[] { ValidRow() });

            Assert.That(result.Valid[0].DayType, Is.EqualTo(DayType.Sunday));
        }

        [Test]
        public void DayTypeDefaultsToPublicHolidayOnAHoliday()
        {
            CaptureResult result = TimesheetCapture.Build(new DateTime(2025, 6, 16), CapturedBy, new[] { ValidRow() });

            Assert.That(result.Valid[0].DayType, Is.EqualTo(DayType.PublicHoliday));
        }

        [Test]
        public void APerRowDayTypeOverrideWins()
        {
            CaptureRow row = ValidRow();
            row.DayType = "Public Holiday";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.True, ErrorFor(result, 0));
            Assert.That(result.Valid[0].DayType, Is.EqualTo(DayType.PublicHoliday));
        }

        // --- Status drives whether times are kept ---

        [TestCase("Off")]
        [TestCase("Leave")]
        [TestCase("AWOL")]
        public void ANonWorkedStatusDiscardsEveryTime(string status)
        {
            CaptureRow row = ValidRow();
            row.Status = status;
            row.Break1Start = "12:00";
            row.Break1End = "12:30";
            row.Break2Start = "15:00";
            row.Break2End = "15:15";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.True, ErrorFor(result, 0));

            Timesheet timesheet = result.Valid[0];
            Assert.That(timesheet.TimeIn, Is.Null);
            Assert.That(timesheet.TimeOut, Is.Null);
            Assert.That(timesheet.Break1Start, Is.Null);
            Assert.That(timesheet.Break1End, Is.Null);
            Assert.That(timesheet.Break2Start, Is.Null);
            Assert.That(timesheet.Break2End, Is.Null);
        }

        [Test]
        public void ANonWorkedStatusIgnoresUnparseableTimes()
        {
            CaptureRow row = ValidRow();
            row.Status = "Off";
            row.TimeIn = "nonsense";

            Assert.That(Build(row).IsValid, Is.True);
        }

        // --- Validation rules ---

        [Test]
        public void ARowWithNoEmployeeIsARowError()
        {
            CaptureRow row = ValidRow();
            row.EmployeeID = null;

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 0), Does.Contain("employee").IgnoreCase);
        }

        [Test]
        public void WorkedWithoutATimeInIsARowError()
        {
            CaptureRow row = ValidRow();
            row.TimeIn = "";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 0), Does.Contain("TimeIn"));
        }

        [Test]
        public void WorkedWithoutATimeOutIsARowError()
        {
            CaptureRow row = ValidRow();
            row.TimeOut = "";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 0), Does.Contain("TimeOut"));
        }

        [Test]
        public void TimeOutBeforeTimeInIsARowError()
        {
            CaptureRow row = ValidRow();
            row.TimeIn = "17:00";
            row.TimeOut = "08:00";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 0), Does.Contain("after"));
        }

        [Test]
        public void TimeOutEqualToTimeInIsARowError()
        {
            CaptureRow row = ValidRow();
            row.TimeOut = "08:00";

            Assert.That(Build(row).IsValid, Is.False);
        }

        [Test]
        public void ABreakWithOnlyAStartIsARowError()
        {
            CaptureRow row = ValidRow();
            row.Break1Start = "12:00";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 0), Does.Contain("Break 1"));
        }

        [Test]
        public void ABreakWithOnlyAnEndIsARowError()
        {
            CaptureRow row = ValidRow();
            row.Break2End = "15:00";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 0), Does.Contain("Break 2"));
        }

        [Test]
        public void ABreakEndingBeforeItStartsIsARowError()
        {
            CaptureRow row = ValidRow();
            row.Break1Start = "12:30";
            row.Break1End = "12:00";

            Assert.That(Build(row).IsValid, Is.False);
        }

        [Test]
        public void ABreakStartingBeforeTheShiftIsARowError()
        {
            CaptureRow row = ValidRow();
            row.Break1Start = "07:00";
            row.Break1End = "07:30";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 0), Does.Contain("inside"));
        }

        [Test]
        public void ABreakEndingAfterTheShiftIsARowError()
        {
            CaptureRow row = ValidRow();
            row.Break1Start = "16:45";
            row.Break1End = "17:30";

            Assert.That(Build(row).IsValid, Is.False);
        }

        [Test]
        public void ASecondBreakStartingBeforeTheFirstEndsIsARowError()
        {
            CaptureRow row = ValidRow();
            row.Break1Start = "12:00";
            row.Break1End = "12:30";
            row.Break2Start = "12:15";
            row.Break2End = "12:45";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 0), Does.Contain("Break 2"));
        }

        [Test]
        public void ASecondBreakWithoutAFirstIsARowError()
        {
            CaptureRow row = ValidRow();
            row.Break2Start = "14:00";
            row.Break2End = "14:30";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 0), Does.Contain("Break 1"));
        }

        [Test]
        public void ASecondBreakStartingExactlyWhenTheFirstEndsIsARowError()
        {
            // Back-to-back breaks are really one longer break, and the rule says Break 2
            // must start after Break 1 ends.
            CaptureRow row = ValidRow();
            row.Break1Start = "12:00";
            row.Break1End = "12:30";
            row.Break2Start = "12:30";
            row.Break2End = "13:00";

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 0), Does.Contain("Break 2"));
        }

        [Test]
        public void ADuplicateEmployeeIsCaughtEvenWhenTheFirstRowIsInvalid()
        {
            // The first row failing for an unrelated reason must not let the same
            // employee through on a later row.
            CaptureRow first = ValidRow(1);
            first.TimeOut = "";

            CaptureRow second = ValidRow(1);
            second.RowIndex = 1;

            CaptureResult result = Build(first, second);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 1), Does.Contain("twice").IgnoreCase);
        }

        [Test]
        public void TheSameEmployeeTwiceIsARowError()
        {
            CaptureRow first = ValidRow(1);
            CaptureRow second = ValidRow(1);
            second.RowIndex = 1;

            CaptureResult result = Build(first, second);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ErrorFor(result, 1), Does.Contain("twice").IgnoreCase);
        }

        [Test]
        public void EachBadRowReportsItsOwnError()
        {
            CaptureRow first = ValidRow(1);
            first.TimeOut = "";

            CaptureRow second = ValidRow(2);
            second.RowIndex = 1;
            second.TimeIn = "nonsense";

            CaptureResult result = Build(first, second);

            Assert.That(result.Errors, Has.Count.EqualTo(2));
            Assert.That(ErrorFor(result, 0), Is.Not.Null);
            Assert.That(ErrorFor(result, 1), Is.Not.Null);
        }

        [Test]
        public void NoTimesheetsAreProducedWhenAnyRowIsInvalid()
        {
            CaptureRow good = ValidRow(1);
            CaptureRow bad = ValidRow(2);
            bad.RowIndex = 1;
            bad.TimeOut = "";

            CaptureResult result = Build(good, bad);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Valid, Is.Empty);
        }

        // --- Updates ---

        [Test]
        public void ARowMarkedAsAnUpdateCarriesItsExistingIdentity()
        {
            CaptureRow row = ValidRow();
            row.ExistingTimesheetID = 42;

            CaptureResult result = Build(row);

            Assert.That(result.IsValid, Is.True, ErrorFor(result, 0));
            Assert.That(result.Valid[0].TimesheetID, Is.EqualTo(42));
            Assert.That(result.Valid[0].IsUpdate, Is.True);
        }

        [Test]
        public void ANewRowHasNoIdentity()
        {
            CaptureResult result = Build(ValidRow());

            Assert.That(result.Valid[0].TimesheetID, Is.Null);
            Assert.That(result.Valid[0].IsUpdate, Is.False);
        }

        [Test]
        public void AWorkedTimesheetDescribesItsHours()
        {
            Timesheet timesheet = Build(ValidRow()).Valid[0];

            Assert.That(timesheet.DescribeWhen(), Is.EqualTo("08:00 - 17:00"));
        }

        [Test]
        public void AnAbsentTimesheetDescribesItsStatusInsteadOfHours()
        {
            CaptureRow row = ValidRow();
            row.Status = "AWOL";

            Timesheet timesheet = Build(row).Valid[0];

            Assert.That(timesheet.DescribeWhen(), Is.EqualTo("AWOL"));
        }

        [Test]
        public void NoRowsProducesNothingAndNoErrors()
        {
            CaptureResult result = Build();

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Valid, Is.Empty);
        }

        [Test]
        public void NotesAreCarriedThrough()
        {
            CaptureRow row = ValidRow();
            row.Notes = "Covered the late shift";

            Assert.That(Build(row).Valid[0].Notes, Is.EqualTo("Covered the late shift"));
        }

        [Test]
        public void BlankNotesBecomeNull()
        {
            CaptureRow row = ValidRow();
            row.Notes = "   ";

            Assert.That(Build(row).Valid[0].Notes, Is.Null);
        }
    }
}
