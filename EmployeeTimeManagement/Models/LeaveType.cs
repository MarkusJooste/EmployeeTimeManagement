using System;

namespace EmployeeTimeManagement.Models
{
    // Mirrors the LeaveType enum on TBL_leave.
    public enum LeaveType
    {
        PTO,
        Sick,
        Maternity,
        AWOL
    }

    public static class LeaveTypes
    {
        private const string PTOValue = "PTO";
        private const string SickValue = "SICK";
        private const string MaternityValue = "MATERNITY";
        private const string AWOLValue = "AWOL";

        // Converts a LeaveType to the exact string stored in the TBL_leave enum column.
        public static string ToDatabaseValue(this LeaveType leaveType)
        {
            switch (leaveType)
            {
                case LeaveType.PTO:
                    return PTOValue;
                case LeaveType.Sick:
                    return SickValue;
                case LeaveType.Maternity:
                    return MaternityValue;
                case LeaveType.AWOL:
                    return AWOLValue;
                default:
                    throw new ArgumentOutOfRangeException("leaveType", leaveType, "Unknown leave type.");
            }
        }

        // Lists every leave type as stored, for binding a dropdown without restating them.
        public static string[] AllDatabaseValues()
        {
            return new[] { PTOValue, SickValue, MaternityValue, AWOLValue };
        }

        // Lists the Leave Types a manager can book. AWOL is captured only from Capture
        // Timesheets, so it is deliberately left out.
        public static string[] BookableValues()
        {
            return new[] { PTOValue, SickValue, MaternityValue };
        }

        // Parses one of the three bookable Leave Type values, refusing AWOL and anything unrecognised.
        public static bool TryParseBookable(string value, out LeaveType leaveType)
        {
            leaveType = default(LeaveType);

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            LeaveType parsed;
            try
            {
                parsed = FromDatabaseValue(value);
            }
            catch (ArgumentException)
            {
                return false;
            }

            if (parsed == LeaveType.AWOL)
            {
                return false;
            }

            leaveType = parsed;
            return true;
        }

        // Converts a string read from the TBL_leave enum column back to a LeaveType.
        public static LeaveType FromDatabaseValue(string value)
        {
            if (string.Equals(value, PTOValue, StringComparison.OrdinalIgnoreCase))
            {
                return LeaveType.PTO;
            }

            if (string.Equals(value, SickValue, StringComparison.OrdinalIgnoreCase))
            {
                return LeaveType.Sick;
            }

            if (string.Equals(value, MaternityValue, StringComparison.OrdinalIgnoreCase))
            {
                return LeaveType.Maternity;
            }

            if (string.Equals(value, AWOLValue, StringComparison.OrdinalIgnoreCase))
            {
                return LeaveType.AWOL;
            }

            throw new ArgumentException("Unknown leave type value: " + value, "value");
        }
    }
}
