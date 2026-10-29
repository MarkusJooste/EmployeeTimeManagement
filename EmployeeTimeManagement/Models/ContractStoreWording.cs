using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EmployeeTimeManagement.Models
{
    // The places a contract names its store: the heading on page 1, the business named in the
    // preamble, the workplace, the trading hours and the town it is signed in. They are read
    // from a store's own contract PDF, so another store's PDF of a type this store has not
    // supplied yet can be printed with this store's wording written over its own.
    internal static class ContractStoreWording
    {
        public const string HeaderField = "Store name under WIMPY (page 1)";
        public const string BusinessNameField = "Business name (preamble)";
        public const string WorkplaceField = "Workplace (1)";
        public const string WeekdayHoursField = "Monday to Friday hours (5.1.2)";
        public const string SaturdayHoursField = "Saturday hours (5.1.2)";
        public const string SundayHoursField = "Sunday hours (5.1.2)";
        public const string PolicyField = "Store named for the hours policy (5.1.2)";
        public const string TownField = "Signing town";

        private const string Hours = @"(?<value>\d\dH\d\d\s*[–-]\s*\d\dH\d\d)";

        // Each pattern's "value" group is the store's own wording. The wording may wrap onto
        // the next line, so the patterns allow for line breaks inside it.
        private static readonly Dictionary<string, Regex> Patterns = new Dictionary<string, Regex>
        {
            { HeaderField, new Regex(@"WIMPY[ ]*\r?\n(?<value>[^\r\n]*?\S)[ ]*\r?\n[^\r\n@]+@") },
            { BusinessNameField, new Regex(@"currently known as\s+(?<value>\S.*?)\s+and the authority", RegexOptions.Singleline) },
            { WorkplaceField, new Regex(@"situated at\s+(?<value>\S.*?),\s+as well", RegexOptions.Singleline) },
            { WeekdayHoursField, new Regex(@"Monday\s*[–-]\s*Friday:\s*" + Hours) },
            { SaturdayHoursField, new Regex(@"Saturday:\s*" + Hours) },
            { SundayHoursField, new Regex(@"Sunday:\s*" + Hours) },
            { PolicyField, new Regex(@"as per policy of\s+(?<value>\S.*?)\.(?:\s|$)", RegexOptions.Singleline) },
            { TownField, new Regex(@"SIGNED IN\s+(?<value>\S.*?)\s+ON THIS") }
        };

        // Every field, in the order they appear in the contract.
        public static IEnumerable<string> Fields
        {
            get { return Patterns.Keys; }
        }

        public static Regex PatternFor(string field)
        {
            return Patterns[field];
        }

        // The store's wording for each field found in its contract. A field the contract does
        // not have is left out, and its spot in the other store's PDF is left as it is.
        public static Dictionary<string, string> Read(IContractTemplateText template)
        {
            var wording = new Dictionary<string, string>();

            foreach (KeyValuePair<string, Regex> pattern in Patterns)
            {
                for (int page = 0; page < template.PageCount; page++)
                {
                    Match match = pattern.Value.Match(template.PageText(page));
                    if (match.Success)
                    {
                        wording[pattern.Key] = Tidy(match.Groups["value"].Value);
                        break;
                    }
                }
            }

            return wording;
        }

        // Wording as one line with single spaces, as it reads when wrapped onto two.
        public static string Tidy(string value)
        {
            return Regex.Replace(value, @"\s+", " ").Trim();
        }
    }
}
