using System;
using System.IO;
using EmployeeTimeManagement.Database;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class ErrorLogTests
    {
        private const string Secret = "Server=db.example.com;Port=3306;Database=wimpy;User ID=app;Password=hunter2;";

        private string folder;
        private string logPath;
        private DateTime now;

        [SetUp]
        public void CreateFolder()
        {
            folder = Path.Combine(Path.GetTempPath(), "ErrorLogTests", Guid.NewGuid().ToString("N"));
            logPath = Path.Combine(folder, "logs", "errors.log");
            now = new DateTime(2026, 10, 29, 14, 30, 5);
        }

        [TearDown]
        public void DeleteFolder()
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
        }

        private ErrorLog Log(string version = "1.2.0.0", long maxBytes = 512 * 1024)
        {
            return new ErrorLog(Path.Combine(folder, "logs"), version, () => now, maxBytes);
        }

        private static Exception Thrown(string message)
        {
            try
            {
                throw new InvalidOperationException(message);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        [Test]
        public void An_error_is_written_with_time_version_context_and_exception()
        {
            Log().Write("Contracts", Thrown("boom"));

            string text = File.ReadAllText(logPath);
            Assert.That(text, Does.Contain("2026-10-29 14:30:05"));
            Assert.That(text, Does.Contain("1.2.0.0"));
            Assert.That(text, Does.Contain("Contracts"));
            Assert.That(text, Does.Contain("InvalidOperationException"));
            Assert.That(text, Does.Contain("boom"));
        }

        [Test]
        public void Entries_are_added_and_earlier_ones_kept()
        {
            Log().Write("First", Thrown("one"));
            Log().Write("Second", Thrown("two"));

            string text = File.ReadAllText(logPath);
            Assert.That(text, Does.Contain("one"));
            Assert.That(text, Does.Contain("two"));
        }

        [Test]
        public void The_log_survives_an_upgrade_to_a_new_version()
        {
            Log("1.0.0.0").Write("Old", Thrown("before upgrade"));
            Log("1.1.0.0").Write("New", Thrown("after upgrade"));

            string text = File.ReadAllText(logPath);
            Assert.That(text, Does.Contain("before upgrade"));
            Assert.That(text, Does.Contain("1.0.0.0"));
            Assert.That(text, Does.Contain("after upgrade"));
            Assert.That(text, Does.Contain("1.1.0.0"));
        }

        [Test]
        public void A_full_log_rolls_over_and_keeps_one_older_file()
        {
            ErrorLog log = Log(maxBytes: 600);

            for (int i = 0; i < 10; i++)
            {
                log.Write("Roll", Thrown("entry " + i));
            }

            Assert.That(File.Exists(Path.Combine(folder, "logs", "errors.1.log")), Is.True);
            Assert.That(new FileInfo(logPath).Length, Is.LessThan(2000));
            Assert.That(File.ReadAllText(logPath), Does.Contain("entry 9"));
        }

        [Test]
        public void Passwords_and_connection_strings_never_reach_the_log()
        {
            Log().Write("Connecting " + Secret, Thrown("Failed using " + Secret));

            string text = File.ReadAllText(logPath);
            Assert.That(text, Does.Not.Contain("hunter2"));
            Assert.That(text, Does.Not.Contain("db.example.com"));
            Assert.That(text, Does.Not.Contain("User ID=app"));
        }

        [Test]
        public void Diagnostics_carry_the_key_details_without_secrets()
        {
            string diagnostics = Log().BuildDiagnostics("Database connection",
                "Could not connect to the database: access denied for " + Secret);

            Assert.That(diagnostics, Does.Contain("1.2.0.0"));
            Assert.That(diagnostics, Does.Contain("2026-10-29 14:30:05"));
            Assert.That(diagnostics, Does.Contain("Database connection"));
            Assert.That(diagnostics, Does.Contain("Could not connect to the database"));
            Assert.That(diagnostics, Does.Not.Contain("hunter2"));
            Assert.That(diagnostics, Does.Not.Contain("db.example.com"));
        }

        [Test]
        public void Diagnostics_include_the_exception_when_there_is_one()
        {
            string diagnostics = Log().BuildDiagnostics("Contracts", "Could not print", Thrown("printer gone"));

            Assert.That(diagnostics, Does.Contain("InvalidOperationException"));
            Assert.That(diagnostics, Does.Contain("printer gone"));
        }

        [Test]
        public void A_log_that_cannot_be_written_does_not_throw()
        {
            // A file where the log folder should be.
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "logs"), "in the way");

            Assert.DoesNotThrow(() => Log().Write("Anything", Thrown("boom")));
        }
    }
}
