using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
        private static Task warmUp;

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
                connectionString = Tune(newConnectionString.Trim());
                loaded = true;
            }
        }

        // Opens and closes a connection, so setup can refuse a string that does not work.
        public static bool TryConnect(string candidate, out string error)
        {
            try
            {
                using (var connection = new MySqlConnection(Tune(candidate)))
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

        // Opens the first connection in the background, for the login screen to call while
        // the PIN is typed. A first connection costs about a second (network, encryption and
        // login handshakes); once it is in the pool, every later query reuses it.
        public static void WarmUp()
        {
            lock (LoadLock)
            {
                if (warmUp != null && !warmUp.IsCompleted)
                {
                    return;
                }

                warmUp = Task.Run(() =>
                {
                    try
                    {
                        using (OpenConnection())
                        {
                        }
                    }
                    catch (Exception ex)
                    {
                        // The next real query reports the problem; nothing is waiting on this.
                        System.Diagnostics.Trace.TraceWarning("Could not warm up the database connection: " + ex.Message);
                    }
                });
            }
        }

        public static MySqlConnection GetConnection()
        {
            // A warm-up still connecting will finish sooner than a second connection started
            // now, and leaves its connection in the pool for this one.
            Task pending;
            lock (LoadLock)
            {
                pending = warmUp;
            }

            if (pending != null && !pending.IsCompleted)
            {
                pending.Wait();
            }

            return OpenConnection();
        }

        private static MySqlConnection OpenConnection()
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

        // By default every connection taken from the pool is first reset on the server, which
        // costs a whole extra trip to it on every query. Nothing in this app keeps session
        // state between queries (transactions are always committed or rolled back), so the
        // reset is turned off. Idle connections are still dropped after 180 seconds, inside
        // the server's own 300-second timeout, so the pool never hands out a dead one.
        private static string Tune(string raw)
        {
            try
            {
                return new MySqlConnectionStringBuilder(raw) { ConnectionReset = false }.ConnectionString;
            }
            catch (ArgumentException)
            {
                // Left as typed, so opening it reports what is wrong with it.
                return raw;
            }
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
                        string saved = store.Load();
                        connectionString = saved == null ? null : Tune(saved);
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
