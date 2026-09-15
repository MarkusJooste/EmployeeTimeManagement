using System;

namespace EmployeeTimeManagement.Models
{
    // What should happen to a Timesheet's mirrored AWOL Absence.
    public enum AWOLMirrorActionType
    {
        None,
        Insert,
        Update,
        Delete
    }

    // The outcome of reconciling one Timesheet against its mirrored AWOL Absence: write it,
    // update its Reason, remove it, or leave it alone.
    public class AWOLMirrorAction
    {
        private AWOLMirrorAction(AWOLMirrorActionType type, Absence absence, int? leaveID)
        {
            Type = type;
            Absence = absence;
            LeaveID = leaveID;
        }

        public AWOLMirrorActionType Type { get; }

        // Set for Insert and Update: the Absence to write.
        public Absence Absence { get; }

        // Set for Update and Delete: the mirrored row this action applies to.
        public int? LeaveID { get; }

        public static AWOLMirrorAction None()
        {
            return new AWOLMirrorAction(AWOLMirrorActionType.None, null, null);
        }

        public static AWOLMirrorAction Insert(Absence absence)
        {
            return new AWOLMirrorAction(AWOLMirrorActionType.Insert, absence, null);
        }

        public static AWOLMirrorAction Update(int leaveID, Absence absence)
        {
            return new AWOLMirrorAction(AWOLMirrorActionType.Update, absence, leaveID);
        }

        public static AWOLMirrorAction Delete(int leaveID)
        {
            return new AWOLMirrorAction(AWOLMirrorActionType.Delete, null, leaveID);
        }
    }

    // Decides how a saved Timesheet's mirrored AWOL Absence should change, per ADR-0002.
    // Takes the mirror's current LeaveID (if any) rather than looking it up itself, so the
    // decision needs no database and no form.
    public static class AWOLMirror
    {
        public static AWOLMirrorAction Decide(Timesheet timesheet, int? existingMirrorLeaveID)
        {
            if (timesheet.Status != TimesheetStatus.AWOL)
            {
                return existingMirrorLeaveID.HasValue
                    ? AWOLMirrorAction.Delete(existingMirrorLeaveID.Value)
                    : AWOLMirrorAction.None();
            }

            DateTime day = timesheet.WorkDate.Date;
            string reason = string.IsNullOrWhiteSpace(timesheet.Notes) ? null : timesheet.Notes.Trim();

            var absence = new Absence
            {
                EmployeeID = timesheet.EmployeeID,
                LeaveType = LeaveType.AWOL,
                StartDate = day,
                EndDate = day,
                Reason = reason
            };

            if (existingMirrorLeaveID.HasValue)
            {
                return AWOLMirrorAction.Update(existingMirrorLeaveID.Value, absence);
            }

            return AWOLMirrorAction.Insert(absence);
        }
    }
}
