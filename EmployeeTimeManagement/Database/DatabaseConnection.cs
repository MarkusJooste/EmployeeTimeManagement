using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MySqlConnector;

namespace EmployeeTimeManagement.Database
{
    public static class DatabaseConnection
    {
        // Kept in ProgramData rather than next to the program, so installing an update over
        // the program folder does not lose it and each PC is only set up once.
        private static readonly string StorePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "EmployeeTimeManagement", "connection.dat");

        // Mixed into the encryption, so another program using DPAPI on the same PC cannot
        // decrypt the file just by calling it with no extra input.
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("EmployeeTimeManagement.ConnectionString.v1");

        private static readonly object LoadLock = new object();
        private static string connectionString;
        private static bool loaded;

        // Machine scope, so the app works for whichever Windows account the store PC uses.
        private static ConnectionStringStore CreateStore()
        {
            string envPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
            return new ConnectionStringStore(
                StorePath,
                envPath,
                plain => ProtectedData.Protect(plain, Entropy, DataProtectionScope.LocalMachine),
                secret => ProtectedData.Unprotect(secret, Entropy, DataProtectionScope.LocalMachine));
        }

        // Set when an imported .env could not be deleted, for the startup check to show.
        public static string ImportWarning { get; private set; }

        // Whether this PC has a connection string saved (or a .env to import).
        public static bool IsConfigured
        {
            get { return ConnectionString() != null; }
        }

        // Encrypts and saves a connection string typed in at first-run setup.
        public static void Configure(string newConnectionString)
        {
            lock (LoadLock)
            {
                CreateStore().Save(newConnectionString);
                connectionString = newConnectionString.Trim();
                loaded = true;
            }
        }

        // Opens and closes a connection, so setup can refuse a string that does not work.
        public static bool TryConnect(string candidate, out string error)
        {
            try
            {
                using (var connection = new MySqlConnection(candidate))
                {
                    connection.Open();
                }

                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static MySqlConnection GetConnection()
        {
            string current = ConnectionString();
            if (current == null)
            {
                throw new InvalidOperationException(
                    "This PC has no database connection set up. Restart the program to enter it.");
            }

            var connection = new MySqlConnection(current);

            connection.Open();
            return connection;
        }

        // Decrypted once and then kept in memory only.
        private static string ConnectionString()
        {
            lock (LoadLock)
            {
                if (!loaded)
                {
                    ConnectionStringStore store = CreateStore();

                    try
                    {
                        connectionString = store.Load();
                    }
                    catch (CryptographicException)
                    {
                        // Copied from another PC or damaged: only this machine's key opens
                        // it, so treat the PC as not set up and let setup replace the file.
                        connectionString = null;
                    }

                    ImportWarning = store.ImportWarning;
                    loaded = true;
                }

                return connectionString;
            }
        }
    }
}
