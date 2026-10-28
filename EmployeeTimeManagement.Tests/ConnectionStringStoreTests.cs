using System;
using System.IO;
using System.Linq;
using System.Text;
using EmployeeTimeManagement.Database;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class ConnectionStringStoreTests
    {
        private const string Secret = "Server=db.example.com;Database=wimpy;User ID=app;Password=hunter2;";

        private string folder;
        private string storePath;
        private string envPath;

        [SetUp]
        public void CreateFolder()
        {
            folder = Path.Combine(Path.GetTempPath(), "ConnectionStringStoreTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            storePath = Path.Combine(folder, "data", "connection.dat");
            envPath = Path.Combine(folder, ".env");
        }

        [TearDown]
        public void DeleteFolder()
        {
            foreach (string file in Directory.GetFiles(folder, "*", SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(folder, true);
        }

        // Stands in for DPAPI: reversible, and nothing like the plain text.
        private static byte[] Scramble(byte[] bytes)
        {
            return bytes.Select(b => (byte)(b ^ 0x5A)).Reverse().ToArray();
        }

        private static byte[] Unscramble(byte[] bytes)
        {
            return bytes.Reverse().Select(b => (byte)(b ^ 0x5A)).ToArray();
        }

        private ConnectionStringStore Store()
        {
            return new ConnectionStringStore(storePath, envPath, Scramble, Unscramble);
        }

        [Test]
        public void ANewPCHasNothingToLoad()
        {
            Assert.That(Store().Load(), Is.Null);
        }

        [Test]
        public void AnEnvFileIsImportedEncryptedAndDeleted()
        {
            File.WriteAllText(envPath, "# Database connection string\r\n" + ConnectionStringStore.EnvKey + "=" + Secret + "\r\n");

            Assert.That(Store().Load(), Is.EqualTo(Secret));
            Assert.That(File.Exists(envPath), Is.False, "The plain-text .env was left behind.");
            Assert.That(File.ReadAllText(storePath), Does.Not.Contain("hunter2"));
        }

        [Test]
        public void TheImportedStringLoadsWithoutTheEnvFile()
        {
            File.WriteAllText(envPath, ConnectionStringStore.EnvKey + "=" + Secret);
            Store().Load();

            Assert.That(Store().Load(), Is.EqualTo(Secret));
        }

        [Test]
        public void ANewEnvFileReplacesTheSavedString()
        {
            Store().Save("Server=old;");
            File.WriteAllText(envPath, ConnectionStringStore.EnvKey + "=" + Secret);

            Assert.That(Store().Load(), Is.EqualTo(Secret));
            Assert.That(Store().Load(), Is.EqualTo(Secret));
        }

        [TestCase("MYSQL_CONNECTION_STRING=\"Server=a;Password=b;\"")]
        [TestCase("MYSQL_CONNECTION_STRING = 'Server=a;Password=b;'")]
        [TestCase("  MYSQL_CONNECTION_STRING=Server=a;Password=b;  ")]
        public void QuotesAndSpacesAroundTheValueAreDropped(string line)
        {
            File.WriteAllText(envPath, "OTHER=1\n#MYSQL_CONNECTION_STRING=commented\n" + line + "\n");

            Assert.That(Store().Load(), Is.EqualTo("Server=a;Password=b;"));
        }

        [Test]
        public void AnEnvFileWithNoValueIsLeftAlone()
        {
            File.WriteAllText(envPath, ConnectionStringStore.EnvKey + "=\n");

            Assert.That(Store().Load(), Is.Null);
            Assert.That(File.Exists(envPath), Is.True);
        }

        [Test]
        public void SavedSetupLoadsBack()
        {
            Store().Save("  " + Secret + "  ");

            Assert.That(Store().Load(), Is.EqualTo(Secret));
            Assert.That(Encoding.UTF8.GetString(File.ReadAllBytes(storePath)), Does.Not.Contain("hunter2"));
        }

        [Test]
        public void AnEmptyStringIsNotSaved()
        {
            Assert.Throws<ArgumentException>(() => Store().Save("   "));
            Assert.That(File.Exists(storePath), Is.False);
        }

        [Test]
        public void AnEnvFileThatCannotBeDeletedIsReported()
        {
            File.WriteAllText(envPath, ConnectionStringStore.EnvKey + "=" + Secret);
            File.SetAttributes(envPath, FileAttributes.ReadOnly);

            ConnectionStringStore store = Store();

            Assert.That(store.Load(), Is.EqualTo(Secret));
            Assert.That(store.ImportWarning, Does.Contain(envPath));
        }
    }
}
