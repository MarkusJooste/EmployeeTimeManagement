using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace EmployeeTimeManagement.Database
{
    // A small local error log, and the "Copy diagnostics" text built from the same details.
    //
    // The log lives in ProgramData next to the saved connection, outside the program folder,
    // so installing an update does not touch it. When it passes a size it is rolled over to a
    // single older file, so it never grows without limit. Nothing is sent anywhere; the
    // customer copies the diagnostics and sends them back by hand.
    //
    // Anything that looks like a database login is blanked before it is written or copied.
    public class ErrorLog
    {
        private const string TimeFormat = "yyyy-MM-dd HH:mm:ss";
        private const string Blanked = "(hidden)";

        // Connection string settings, whole value up to the next ';'. Covers the password and
        // the server, user and database that identify the hosted database.
        private static readonly Regex ConnectionSetting = new Regex(
            @"\b(password|pwd|server|host|data source|user id|uid|user|database|initial catalog|port)\s*=\s*[^;\r\n]*;?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly string folder;
        private readonly string version;
        private readonly Func<DateTime> clock;
        private readonly long maxBytes;
        private readonly object writeLock = new object();

        public ErrorLog(string folder, string version, Func<DateTime> clock, long maxBytes)
        {
            this.folder = folder;
            this.version = version;
            this.clock = clock;
            this.maxBytes = maxBytes;
        }

        // The program's own log: ProgramData\EmployeeTimeManagement\logs, stamped with the
        // build version from AssemblyInfo.cs.
        public static ErrorLog Default { get; } = new ErrorLog(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "EmployeeTimeManagement", "logs"),
            Assembly.GetExecutingAssembly().GetName().Version.ToString(),
            () => DateTime.Now,
            512 * 1024);

        // Records a failure. Logging must never be the thing that breaks the program, so a
        // log that cannot be written is silently skipped.
        public void Write(string context, Exception exception)
        {
            Append(Describe(context, exception == null ? null : exception.ToString()));
        }

        // Records a problem that has a message but no exception, such as a failed connection test.
        public void Write(string context, string detail)
        {
            Append(Describe(context, detail));
        }

        // The text for the Copy diagnostics button.
        public string BuildDiagnostics(string context, string detail, Exception exception = null)
        {
            var text = new StringBuilder();
            text.AppendLine("Employee Time Management diagnostics");
            text.AppendLine("Version: " + version);
            text.AppendLine("Time: " + clock().ToString(TimeFormat));
            text.AppendLine("Windows: " + Environment.OSVersion.VersionString);
            text.AppendLine("Where: " + Redact(context));
            text.AppendLine("Detail: " + Redact(detail));

            if (exception != null)
            {
                text.AppendLine();
                text.AppendLine(Redact(exception.ToString()));
            }

            return text.ToString();
        }

        public static string Redact(string text)
        {
            if (text == null)
            {
                return null;
            }

            // MySQL's "Access denied for user 'name'@'host'" carries the login without any '='.
            text = Regex.Replace(text, @"'[^']*'@'[^']*'", "'" + Blanked + "'");
            return ConnectionSetting.Replace(text, Blanked + " ");
        }

        private string Describe(string context, string detail)
        {
            var entry = new StringBuilder();
            entry.AppendLine("[" + clock().ToString(TimeFormat) + "] v" + version + " " + Redact(context));

            if (!string.IsNullOrEmpty(detail))
            {
                entry.AppendLine(Redact(detail));
            }

            entry.AppendLine();
            return entry.ToString();
        }

        private void Append(string entry)
        {
            try
            {
                lock (writeLock)
                {
                    Directory.CreateDirectory(folder);

                    string path = Path.Combine(folder, "errors.log");
                    if (File.Exists(path) && new FileInfo(path).Length >= maxBytes)
                    {
                        string older = Path.Combine(folder, "errors.1.log");
                        File.Delete(older);
                        File.Move(path, older);
                    }

                    File.AppendAllText(path, entry, Encoding.UTF8);
                }
            }
            catch (Exception)
            {
                // Nowhere to write it; the dialog still shows the problem to the user.
            }
        }
    }
}
