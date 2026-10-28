using System;
using System.IO;
using System.Text;

namespace EmployeeTimeManagement.Database
{
    // Keeps the database connection string on each PC encrypted, never as readable text.
    //
    // The string is encrypted with Windows DPAPI for the machine, so the saved file is
    // unreadable in Notepad and useless copied to another computer. It cannot stop someone
    // running their own code on this PC, which is why the database login it holds should
    // only be allowed what the app needs.
    //
    // A .env file next to the program (MYSQL_CONNECTION_STRING=...) is imported once:
    // encrypted, saved, and deleted, so the plain text does not stay behind. Dropping a new
    // .env in is also how the connection string is changed later.
    public class ConnectionStringStore
    {
        public const string EnvKey = "MYSQL_CONNECTION_STRING";

        private readonly string storePath;
        private readonly string envPath;
        private readonly Func<byte[], byte[]> protect;
        private readonly Func<byte[], byte[]> unprotect;

        // protect and unprotect are the encryption, passed in so tests can stand in for DPAPI.
        public ConnectionStringStore(string storePath, string envPath, Func<byte[], byte[]> protect, Func<byte[], byte[]> unprotect)
        {
            this.storePath = storePath;
            this.envPath = envPath;
            this.protect = protect;
            this.unprotect = unprotect;
        }

        // Set when a .env was imported but could not be deleted afterwards, so the caller
        // can warn that the plain text is still on disk.
        public string ImportWarning { get; private set; }

        // The connection string, importing a .env first if there is one. Null when this PC
        // has not been set up yet.
        public string Load()
        {
            ImportWarning = null;

            string imported = ReadEnv();
            if (imported != null)
            {
                Save(imported);

                try
                {
                    File.Delete(envPath);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    ImportWarning = "The connection string was saved encrypted, but the .env file at " + envPath
                        + " could not be deleted (" + ex.Message + "). Delete it by hand so the password is not left readable.";
                }

                return imported;
            }

            if (!File.Exists(storePath))
            {
                return null;
            }

            byte[] decrypted = unprotect(File.ReadAllBytes(storePath));
            return Encoding.UTF8.GetString(decrypted);
        }

        public void Save(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("The connection string is empty.", "connectionString");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(storePath));
            File.WriteAllBytes(storePath, protect(Encoding.UTF8.GetBytes(connectionString.Trim())));
        }

        // The MYSQL_CONNECTION_STRING value from the .env, or null when there is no file or
        // no value in it. Comment lines are skipped and surrounding quotes removed.
        private string ReadEnv()
        {
            if (envPath == null || !File.Exists(envPath))
            {
                return null;
            }

            foreach (string rawLine in File.ReadAllLines(envPath))
            {
                string line = rawLine.Trim();

                if (line.StartsWith("#") || !line.StartsWith(EnvKey))
                {
                    continue;
                }

                string rest = line.Substring(EnvKey.Length).TrimStart();
                if (!rest.StartsWith("="))
                {
                    continue;
                }

                string value = rest.Substring(1).Trim();
                if (value.Length >= 2 && (value[0] == '"' || value[0] == '\'') && value[value.Length - 1] == value[0])
                {
                    value = value.Substring(1, value.Length - 2);
                }

                return value.Length == 0 ? null : value;
            }

            return null;
        }
    }
}
