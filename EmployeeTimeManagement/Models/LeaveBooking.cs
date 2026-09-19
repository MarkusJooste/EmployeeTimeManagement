using System;
using System.Collections.Generic;

namespace EmployeeTimeManagement.Models
{
    // Everything the Book an Absence editor collects, before validation.
    public class LeaveBookingRequest
    {
        public int EmployeeID { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }

        // The Leave Balance available for this Leave Type before this booking. Null for a
        // Leave Type with no balance to exceed (Maternity, AWOL), so no over-balance check applies.
        public int? AvailableBalance { get; set; }

        // The manager's justification for booking past the Leave Balance. Only ever stored
        // on the Absence when the booking actually needed it to be accepted.
        public string OverrideReason { get; set; }

        // Every other Absence already on record for this employee, any Leave Type included,
        // so a day cannot end up double-booked regardless of which balance it would draw from.
        public IEnumerable<Absence> ExistingAbsences { get; set; } = new List<Absence>();

        // The LeaveID of the Absence being edited, so the overlap check ignores that Absence's
        // own current dates and the result carries this ID for LeaveController.Update to find it
        // by. Null for a new booking.
        public int? EditingLeaveID { get; set; }
    }

    // The outcome of validating a booking: the Absence it describes, or the rule that
    // blocked it. A blocked booking carries no Absence at all, so nothing can accidentally
    // be written from a refused request.
    public class LeaveBookingResult
    {
        private LeaveBookingResult(Absence absence, string error, string warning, bool requiresOverride)
        {
            Absence = absence;
            Error = error;
            Warning = warning;
            RequiresOverride = requiresOverride;
        }

        public Absence Absence { get; }
        public string Error { get; }

        // Set when the booking is allowed but worth a manager's second look before it is saved.
        public string Warning { get; }

        // Set when this refusal is specifically the over-balance rule, so the caller knows an
        // Override reason - not just a corrected request - is what turns it into an Accept.
        public bool RequiresOverride { get; }

        public bool IsValid
        {
            get { return Error == null; }
        }

        public static LeaveBookingResult Refuse(string error)
        {
            return new LeaveBookingResult(null, error, null, false);
        }

        public static LeaveBookingResult RefuseOverBalance(string error)
        {
            return new LeaveBookingResult(null, error, null, true);
        }

        public static LeaveBookingResult Accept(Absence absence, string warning)
        {
            return new LeaveBookingResult(absence, null, warning, false);
        }
    }

    // Turns a Book an Absence request into a persistable Absence, or the rule that blocks
    // it. Holds every booking rule so the form itself stays a dumb collector, matching
    // TimesheetCapture.
    public static class LeaveBooking
    {
        // The BCEA maternity leave period this app can compare against, used only to warn,
        // not to block: overshooting it is legitimate and common.
        private const int MaternityWarningMonths = 4;

        public static LeaveBookingResult Build(LeaveBookingRequest request)
        {
            if (IsExplicitlyAwol(request.LeaveType))
            {
                return LeaveBookingResult.Refuse("AWOL cannot be booked here; it is captured from Capture Timesheets.");
            }

            LeaveType leaveType;
            if (!LeaveTypes.TryParseBookable(request.LeaveType, out leaveType))
            {
                return LeaveBookingResult.Refuse("Choose a Leave Type.");
            }

            DateTime startDate = request.StartDate.Date;
            DateTime endDate = request.EndDate.Date;

            if (endDate < startDate)
            {
                return LeaveBookingResult.Refuse("The end date cannot be before the start date.");
            }

            if (!IsWithinEmployment(request, startDate, endDate))
            {
                return LeaveBookingResult.Refuse("These dates fall outside the employee's employment.");
            }

            Absence overlap = FindOverlap(request.ExistingAbsences, startDate, endDate, request.EditingLeaveID);
            if (overlap != null)
            {
                return LeaveBookingResult.Refuse(string.Format(
                    "These dates overlap an existing {0} Absence ({1:dd MMM yyyy} to {2:dd MMM yyyy}).",
                    overlap.LeaveTypeDisplay, overlap.StartDate, overlap.EndDate));
            }

            int dayCount = (endDate - startDate).Days + 1;
            string overrideReason = string.IsNullOrWhiteSpace(request.OverrideReason) ? null : request.OverrideReason.Trim();

            if (request.AvailableBalance.HasValue && dayCount > request.AvailableBalance.Value)
            {
                if (overrideReason == null)
                {
                    return LeaveBookingResult.RefuseOverBalance(string.Format(
                        "This booking needs {0} days but only {1} are available. Add an Override reason to book it anyway.",
                        dayCount, request.AvailableBalance.Value));
                }
            }
            else
            {
                // An Override reason typed for a booking that turns out not to need one is not
                // an override at all, so it is never carried onto the Absence.
                overrideReason = null;
            }

            string reason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim();

            var absence = new Absence
            {
                LeaveID = request.EditingLeaveID ?? 0,
                EmployeeID = request.EmployeeID,
                LeaveType = leaveType,
                StartDate = startDate,
                EndDate = endDate,
                Reason = reason,
                OverrideReason = overrideReason
            };

            string warning = null;
            if (leaveType == LeaveType.Maternity && endDate > startDate.AddMonths(MaternityWarningMonths))
            {
                warning = string.Format(
                    "This maternity Absence is longer than {0} months. Continue?", MaternityWarningMonths);
            }

            return LeaveBookingResult.Accept(absence, warning);
        }

        // Names AWOL specifically as the blocker, rather than folding it into the generic
        // "choose a Leave Type" refusal, since the issue calls out AWOL as its own rule.
        private static bool IsExplicitlyAwol(string value)
        {
            LeaveType parsed;
            try
            {
                parsed = LeaveTypes.FromDatabaseValue(value);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return parsed == LeaveType.AWOL;
        }

        // Applies the same employment rule Capture Timesheets already uses, checked at both
        // ends of the range: a contract that covers the start and the end covers everything
        // between them, since the app only ever has one open contract per employee.
        private static bool IsWithinEmployment(LeaveBookingRequest request, DateTime startDate, DateTime endDate)
        {
            return Employee.IsEmployedOn(request.ContractStartDate, request.ContractEndDate, startDate)
                && Employee.IsEmployedOn(request.ContractStartDate, request.ContractEndDate, endDate);
        }

        // Returns the first existing Absence the requested range overlaps, or null when it
        // is clear. Leave Type plays no part: a day cannot belong to two Absences at once.
        // Skips editingLeaveID's own row, so an edit never overlaps its own prior dates.
        private static Absence FindOverlap(IEnumerable<Absence> existingAbsences, DateTime startDate, DateTime endDate, int? editingLeaveID)
        {
            if (existingAbsences == null)
            {
                return null;
            }

            foreach (Absence existing in existingAbsences)
            {
                if (editingLeaveID.HasValue && existing.LeaveID == editingLeaveID.Value)
                {
                    continue;
                }

                if (startDate <= existing.EndDate.Date && endDate >= existing.StartDate.Date)
                {
                    return existing;
                }
            }

            return null;
        }
    }
}
