using System;
using System.IO;
using System.Security.Cryptography;

namespace EmployeeTimeManagement.Database
{
    public enum StartupProblem
    {
        // A connection string is saved (or was in a .env) but the database test failed.
        ConnectionFailed,

        // This PC has no connection string at all.
        NotSetUp
    }

    public enum StartupChoice
    {
        Retry,
        Create,
        ContinueForNow
    }

    public delegate bool ConnectionTest(string connectionString, out string error);

    public class StartupResult
    {
        public bool Connected { get; set; }

        // Why there is no connection, for the not-connected screen. Null when connected.
        public string Reason { get; set; }

        // Set when an imported .env could not be deleted, so the caller can warn.
        public string ImportWarning { get; set; }
    }

    // What the program does on launch about the database connection: import any .env,
    // test the saved connection, and let the user retry, create it, or carry on without.
    //
    // The connection test, the user prompt and the setup form are passed in, so tests can
    // stand in for the database and the person.
    public class StartupConnectionFlow
    {
        private readonly ConnectionStringStore store;
        private readonly ConnectionTest test;
        private readonly Func<StartupProblem, string, StartupChoice> prompt;
        private readonly Func<bool> create;

        // create shows the setup form and returns whether it saved a working connection.
        public StartupConnectionFlow(
            ConnectionStringStore store,
            ConnectionTest test,
            Func<StartupProblem, string, StartupChoice> prompt,
            Func<bool> create)
        {
            this.store = store;
            this.test = test;
            this.prompt = prompt;
            this.create = create;
        }

        public StartupResult Run()
        {
            var result = new StartupResult();

            while (true)
            {
                string loadError = null;
                string connectionString = null;

                try
                {
                    connectionString = store.Load();
                }
                catch (CryptographicException)
                {
                    // Copied from another PC or damaged: only this machine's key opens it,
                    // so treat the PC as not set up and let setup replace the file.
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    loadError = "The saved database connection could not be read or written: " + ex.Message;
                }

                result.ImportWarning = store.ImportWarning ?? result.ImportWarning;

                StartupProblem problem;
                string detail;

                if (connectionString == null)
                {
                    problem = StartupProblem.NotSetUp;
                    detail = loadError ?? "This PC is not connected to the database yet.";
                }
                else
                {
                    string error;
                    if (test(connectionString, out error))
                    {
                        result.Connected = true;
                        return result;
                    }

                    problem = StartupProblem.ConnectionFailed;
                    detail = "Could not connect to the database: " + error;
                }

                StartupChoice choice = prompt(problem, detail);

                if (choice == StartupChoice.ContinueForNow)
                {
                    result.Reason = detail;
                    return result;
                }

                if (choice == StartupChoice.Create && create())
                {
                    result.Connected = true;
                    return result;
                }

                // Retry, or a cancelled setup form: look and ask again.
            }
        }
    }
}
