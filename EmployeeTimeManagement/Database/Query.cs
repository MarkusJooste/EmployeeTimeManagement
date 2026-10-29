using System;
using System.Collections.Generic;
using MySqlConnector;

namespace EmployeeTimeManagement.Database
{
    // One read query: its SQL, its parameters, and how to turn its rows into a result. It can
    // run on its own, or with others in a QueryBatch so several reads cost one trip to the
    // database server instead of one each. The server is far enough away that every trip is
    // noticeable, so a screen that reads several things at once should batch them.
    public interface IBatchedQuery
    {
        MySqlBatchCommand CreateCommand();
        void Read(MySqlDataReader reader);
        void Fail(Exception error);
    }

    public sealed class Query<T> : IBatchedQuery
    {
        private readonly string sql;
        private readonly Func<MySqlDataReader, T> read;
        private readonly List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();

        private bool hasResult;
        private T result;
        private Exception error;

        public Query(string sql, Func<MySqlDataReader, T> read)
        {
            this.sql = sql;
            this.read = read;
        }

        // Adds a parameter; null is sent as SQL NULL.
        public Query<T> With(string name, object value)
        {
            parameters.Add(new KeyValuePair<string, object>(name, value ?? DBNull.Value));
            return this;
        }

        // The rows read, once the query has run. Throws what went wrong if it failed, so each
        // caller can report its own failure while the rest of a batch carries on.
        public T Result
        {
            get
            {
                if (error != null)
                {
                    throw error;
                }

                if (!hasResult)
                {
                    throw new InvalidOperationException("The query has not been run.");
                }

                return result;
            }
        }

        // Runs this query alone, on a connection of its own.
        public T Run()
        {
            QueryBatch.Run(this);
            return Result;
        }

        MySqlBatchCommand IBatchedQuery.CreateCommand()
        {
            // A query run again starts from nothing rather than keeping the last result.
            hasResult = false;
            error = null;

            var command = new MySqlBatchCommand(sql);
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            return command;
        }

        void IBatchedQuery.Read(MySqlDataReader reader)
        {
            result = read(reader);
            hasResult = true;
        }

        void IBatchedQuery.Fail(Exception failure)
        {
            error = failure;
        }
    }

    public static class QueryBatch
    {
        // Sends every query to the server together and reads their results back in order. If
        // one fails, the ones before it keep their results; it and the ones after it fail with
        // its error, because the server stops at the first statement that fails.
        public static void Run(params IBatchedQuery[] queries)
        {
            int next = 0;

            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                using (var batch = new MySqlBatch(connection))
                {
                    foreach (IBatchedQuery query in queries)
                    {
                        batch.BatchCommands.Add(query.CreateCommand());
                    }

                    using (MySqlDataReader reader = batch.ExecuteReader())
                    {
                        for (; next < queries.Length; next++)
                        {
                            if (next > 0)
                            {
                                reader.NextResult();
                            }

                            queries[next].Read(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                for (int failed = next; failed < queries.Length; failed++)
                {
                    queries[failed].Fail(ex);
                }
            }
        }

        // Reads a single-value result, such as a COUNT(*).
        public static int ReadInt(MySqlDataReader reader)
        {
            return reader.Read() && !reader.IsDBNull(0) ? Convert.ToInt32(reader.GetValue(0)) : 0;
        }
    }
}
