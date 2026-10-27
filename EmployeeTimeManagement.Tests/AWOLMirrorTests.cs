using System;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class AWOLMirrorTests
    {
        private static readonly DateTime WorkDate = new DateTime(2026, 3, 10);

        private static Timesheet BuildTimesheet(TimesheetStatus status, string notes = null, int employeeID = 1)
        {
            return new Timesheet
            {
                EmployeeID = employeeID,
                WorkDate = WorkDate,
                Status = status,
                Notes = notes
            };
        }

        [Test]
        public void AnAwolTimesheetWithNoExistingMirrorIsInserted()
        {
            AWOLMirrorAction action = AWOLMirror.Decide(BuildTimesheet(TimesheetStatus.AWOL), null);

            Assert.That(action.Type, Is.EqualTo(AWOLMirrorActionType.Insert));
            Assert.That(action.Absence.EmployeeID, Is.EqualTo(1));
            Assert.That(action.Absence.LeaveType, Is.EqualTo(LeaveType.AWOL));
            Assert.That(action.Absence.StartDate, Is.EqualTo(WorkDate));
            Assert.That(action.Absence.EndDate, Is.EqualTo(WorkDate));
        }

        [Test]
        public void TheInsertedAbsenceIsASingleDaySpanningJustTheWorkDate()
        {
            AWOLMirrorAction action = AWOLMirror.Decide(BuildTimesheet(TimesheetStatus.AWOL), null);

            Assert.That(action.Absence.StartDate, Is.EqualTo(action.Absence.EndDate));
            Assert.That(action.Absence.DayCount, Is.EqualTo(1));
        }

        [Test]
        public void TheTimesheetsNotesBecomeTheAbsencesReason()
        {
            AWOLMirrorAction action = AWOLMirror.Decide(BuildTimesheet(TimesheetStatus.AWOL, "No call, no show"), null);

            Assert.That(action.Absence.Reason, Is.EqualTo("No call, no show"));
        }

        [Test]
        public void EmptyNotesProduceNoReasonRatherThanPlaceholderText()
        {
            AWOLMirrorAction action = AWOLMirror.Decide(BuildTimesheet(TimesheetStatus.AWOL, "   "), null);

            Assert.That(action.Absence.Reason, Is.Null);
        }

        [Test]
        public void MissingNotesProduceNoReason()
        {
            AWOLMirrorAction action = AWOLMirror.Decide(BuildTimesheet(TimesheetStatus.AWOL, null), null);

            Assert.That(action.Absence.Reason, Is.Null);
        }

        [Test]
        public void AnAwolTimesheetWithAnExistingMirrorIsUpdatedRatherThanInserted()
        {
            AWOLMirrorAction action = AWOLMirror.Decide(BuildTimesheet(TimesheetStatus.AWOL, "Updated reason"), 42);

            Assert.That(action.Type, Is.EqualTo(AWOLMirrorActionType.Update));
            Assert.That(action.LeaveID, Is.EqualTo(42));
            Assert.That(action.Absence.Reason, Is.EqualTo("Updated reason"));
        }

        [Test]
        public void ChangingStatusAwayFromAwolDeletesAnExistingMirror()
        {
            AWOLMirrorAction action = AWOLMirror.Decide(BuildTimesheet(TimesheetStatus.Worked), 42);

            Assert.That(action.Type, Is.EqualTo(AWOLMirrorActionType.Delete));
            Assert.That(action.LeaveID, Is.EqualTo(42));
            Assert.That(action.Absence, Is.Null);
        }

        [Test]
        public void ADayThatWasNeverAwolProducesNeitherAWriteNorARemoval()
        {
            AWOLMirrorAction action = AWOLMirror.Decide(BuildTimesheet(TimesheetStatus.Off), null);

            Assert.That(action.Type, Is.EqualTo(AWOLMirrorActionType.None));
            Assert.That(action.Absence, Is.Null);
            Assert.That(action.LeaveID, Is.Null);
        }

        [Test]
        public void ALeaveStatusWithNoExistingMirrorAlsoProducesNoAction()
        {
            AWOLMirrorAction action = AWOLMirror.Decide(BuildTimesheet(TimesheetStatus.Leave), null);

            Assert.That(action.Type, Is.EqualTo(AWOLMirrorActionType.None));
        }

        [Test]
        public void SeveralAwolEmployeesOnOneDayEachProduceTheirOwnInsert()
        {
            AWOLMirrorAction first = AWOLMirror.Decide(BuildTimesheet(TimesheetStatus.AWOL, employeeID: 1), null);
            AWOLMirrorAction second = AWOLMirror.Decide(BuildTimesheet(TimesheetStatus.AWOL, employeeID: 2), null);

            Assert.That(first.Absence.EmployeeID, Is.EqualTo(1));
            Assert.That(second.Absence.EmployeeID, Is.EqualTo(2));
            Assert.That(first.Type, Is.EqualTo(AWOLMirrorActionType.Insert));
            Assert.That(second.Type, Is.EqualTo(AWOLMirrorActionType.Insert));
        }
    }
}
