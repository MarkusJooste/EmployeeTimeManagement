using System;

namespace EmployeeTimeManagement.Models
{
    // The four marital statuses the application permits in the MaritialStatus column, which
    // the schema leaves as free text.
    public static class MaritalStatuses
    {
        public const string Single = "Single";
        public const string Married = "Married";
        public const string Divorced = "Divorced";
        public const string Widowed = "Widowed";

        // Lists every status, for binding the dropdown without restating them.
        public static string[] All()
        {
            return new[] { Single, Married, Divorced, Widowed };
        }

        // Matches a typed or stored status against the permitted four, accepting any casing
        // and returning the canonical spelling.
        public static bool TryParse(string value, out string status)
        {
            status = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            foreach (string candidate in All())
            {
                if (string.Equals(candidate, value.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    status = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}
