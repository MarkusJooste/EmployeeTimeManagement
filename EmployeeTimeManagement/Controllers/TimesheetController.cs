using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTimeManagement.Database;
using EmployeeTimeManagement.Models;
using MySqlConnector;

namespace EmployeeTimeManagement.Controllers
{
    internal class TimesheetController
    {
        // Returns per-employee hours worked between two dates for one store, net of breaks, with Sunday and public-holiday premium hours broken out.
        public List<TimesheetSummary> GetSummary(DateTime fromDate, DateTime toDate, int storeID)
        {
            const string query = @"SELECT a.EmployeeID,
       MAX(e.Name)              AS Name,
       MAX(e.Surname)           AS Surname,
       MAX(a.DaysWorked)        AS DaysWorked,
       MAX(a.TotalHours)        AS TotalHours,
       MAX(a.SundayExtraHours)  AS SundayExtraHours,
       MAX(a.HolidayExtraHours) AS HolidayExtraHours
FROM (
    SELECT r.EmployeeID,
           COUNT(DISTINCT r.WorkDate) AS DaysWorked,
           SUM(r.WorkedSeconds) / 3600 AS TotalHours,
           SUM(CASE WHEN r.DayType = 'Sunday'         THEN r.WorkedSeconds ELSE 0 END) / 3600 * 0.5 AS SundayExtraHours,
           SUM(CASE WHEN r.DayType = 'Public Holiday' THEN r.WorkedSeconds ELSE 0 END) / 3600 * 1.0 AS HolidayExtraHours
    FROM (
        SELECT EmployeeID,
               WorkDate,
               DayType,
               GREATEST(
                   TIME_TO_SEC(TIMEDIFF(TimeOut, TimeIn))
                   - IFNULL(TIME_TO_SEC(TIMEDIFF(Break1End, Break1Start)), 0)
                   - IFNULL(TIME_TO_SEC(TIMEDIFF(Break2End, Break2Start)), 0), 0) AS WorkedSeconds
        FROM TBL_timesheets
        WHERE WorkDate BETWEEN @FromDate AND @ToDate
          AND Status = 'Worked'
          AND TimeIn IS NOT NULL
          AND TimeOut IS NOT NULL
    ) r
    GROUP BY r.EmployeeID
) a
INNER JOIN TBL_employees e ON e.EmployeeID = a.EmployeeID
WHERE e.StoreID = @StoreID
GROUP BY a.EmployeeID
ORDER BY MAX(e.Surname), MAX(e.Name);";

            var summaries = new List<TimesheetSummary>();

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    command.Parameters.AddWithValue("@ToDate", toDate.Date);
                    command.Parameters.AddWithValue("@StoreID", storeID);

                    using (var reader = command.ExecuteReader())
                    {
                        int employeeIndex = reader.GetOrdinal("EmployeeID");
                        int nameIndex = reader.GetOrdinal("Name");
                        int surnameIndex = reader.GetOrdinal("Surname");
                        int daysIndex = reader.GetOrdinal("DaysWorked");
                        int totalIndex = reader.GetOrdinal("TotalHours");
                        int sundayIndex = reader.GetOrdinal("SundayExtraHours");
                        int holidayIndex = reader.GetOrdinal("HolidayExtraHours");

                        while (reader.Read())
                        {
                            var summary = new TimesheetSummary();

                            // EmployeeID
                            summary.EmployeeID = reader.GetInt32(employeeIndex);

                            // Name
                            if (reader.IsDBNull(nameIndex))
                            {
                                summary.Name = string.Empty;
                            }
                            else
                            {
                                summary.Name = reader.GetString(nameIndex);
                            }

                            // Surname
                            if (reader.IsDBNull(surnameIndex))
                            {
                                summary.Surname = string.Empty;
                            }
                            else
                            {
                                summary.Surname = reader.GetString(surnameIndex);
                            }

                            // DaysWorked
                            if (reader.IsDBNull(daysIndex))
                            {
                                summary.DaysWorked = 0;
                            }
                            else
                            {
                                summary.DaysWorked = reader.GetInt32(daysIndex);
                            }

                            // TotalHours
                            if (reader.IsDBNull(totalIndex))
                            {
                                summary.TotalHours = 0;
                            }
                            else
                            {
                                summary.TotalHours = reader.GetDecimal(totalIndex);
                            }

                            // SundayExtraHours
                            if (reader.IsDBNull(sundayIndex))
                            {
                                summary.SundayExtraHours = 0;
                            }
                            else
                            {
                                summary.SundayExtraHours = reader.GetDecimal(sundayIndex);
                            }

                            // HolidayExtraHours
                            if (reader.IsDBNull(holidayIndex))
                            {
                                summary.HolidayExtraHours = 0;
                            }
                            else
                            {
                                summary.HolidayExtraHours = reader.GetDecimal(holidayIndex);
                            }

                            summaries.Add(summary);
                        }

                        return summaries;
                    }
                }
            }
        }

        // Returns every Timesheet already stored for one employee on one WorkDate. More
        // than one is possible: the table has no unique key on employee and date.
        public List<Timesheet> GetByEmployeeAndDate(int employeeID, DateTime workDate)
        {
            const string query = @"SELECT TimesheetID, EmployeeID, WorkDate, TimeIn, TimeOut,
       Break1Start, Break1End, Break2Start, Break2End,
       DayType, Status, CapturedBy, BusinessDate, Notes
FROM TBL_timesheets
WHERE EmployeeID = @EmployeeID AND WorkDate = @WorkDate
ORDER BY TimesheetID;";

            var timesheets = new List<Timesheet>();

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);
                    command.Parameters.AddWithValue("@WorkDate", workDate.Date);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            timesheets.Add(ReadTimesheet(reader));
                        }

                        return timesheets;
                    }
                }
            }
        }

        // Writes a whole day of Timesheets in one transaction, so a failure part way
        // through leaves nothing behind.
        public void Save(IEnumerable<Timesheet> timesheets)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (Timesheet timesheet in timesheets)
                        {
                            if (timesheet.IsUpdate)
                            {
                                Update(connection, transaction, timesheet);
                            }
                            else
                            {
                                Insert(connection, transaction, timesheet);
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Inserts a new Timesheet.
        private static void Insert(MySqlConnection connection, MySqlTransaction transaction, Timesheet timesheet)
        {
            const string query = @"INSERT INTO TBL_timesheets
    (EmployeeID, WorkDate, TimeIn, TimeOut, Break1Start, Break1End, Break2Start, Break2End,
     DayType, Status, CapturedBy, BusinessDate, Notes)
VALUES
    (@EmployeeID, @WorkDate, @TimeIn, @TimeOut, @Break1Start, @Break1End, @Break2Start, @Break2End,
     @DayType, @Status, @CapturedBy, @BusinessDate, @Notes);";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                AddTimesheetParameters(command, timesheet);
                command.ExecuteNonQuery();

                // Keeping the new identity means saving the same row again updates it
                // rather than inserting a duplicate.
                timesheet.TimesheetID = (int)command.LastInsertedId;
            }
        }

        // Replaces a stored Timesheet, taking over CapturedBy so it records who last wrote it.
        private static void Update(MySqlConnection connection, MySqlTransaction transaction, Timesheet timesheet)
        {
            const string query = @"UPDATE TBL_timesheets
SET WorkDate = @WorkDate,
    TimeIn = @TimeIn,
    TimeOut = @TimeOut,
    Break1Start = @Break1Start,
    Break1End = @Break1End,
    Break2Start = @Break2Start,
    Break2End = @Break2End,
    DayType = @DayType,
    Status = @Status,
    CapturedBy = @CapturedBy,
    BusinessDate = @BusinessDate,
    Notes = @Notes
WHERE TimesheetID = @TimesheetID AND EmployeeID = @EmployeeID;";

            using (var command = new MySqlCommand(query, connection, transaction))
            {
                AddTimesheetParameters(command, timesheet);
                command.Parameters.AddWithValue("@TimesheetID", timesheet.TimesheetID.Value);
                command.ExecuteNonQuery();
            }
        }

        // Binds the columns shared by the insert and the update.
        private static void AddTimesheetParameters(MySqlCommand command, Timesheet timesheet)
        {
            command.Parameters.AddWithValue("@EmployeeID", timesheet.EmployeeID);
            command.Parameters.AddWithValue("@WorkDate", timesheet.WorkDate.Date);
            command.Parameters.AddWithValue("@TimeIn", ToParameter(timesheet.TimeIn));
            command.Parameters.AddWithValue("@TimeOut", ToParameter(timesheet.TimeOut));
            command.Parameters.AddWithValue("@Break1Start", ToParameter(timesheet.Break1Start));
            command.Parameters.AddWithValue("@Break1End", ToParameter(timesheet.Break1End));
            command.Parameters.AddWithValue("@Break2Start", ToParameter(timesheet.Break2Start));
            command.Parameters.AddWithValue("@Break2End", ToParameter(timesheet.Break2End));
            command.Parameters.AddWithValue("@DayType", timesheet.DayType.ToDatabaseValue());
            command.Parameters.AddWithValue("@Status", timesheet.Status.ToDatabaseValue());
            command.Parameters.AddWithValue("@CapturedBy", timesheet.CapturedBy);
            command.Parameters.AddWithValue("@BusinessDate", timesheet.BusinessDate.Date);
            command.Parameters.AddWithValue("@Notes", ToParameter(timesheet.Notes));
        }

        // Converts an absent value to the DBNull the driver expects.
        private static object ToParameter(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            return value;
        }

        // Reads one TBL_timesheets row into a Timesheet.
        private static Timesheet ReadTimesheet(MySqlDataReader reader)
        {
            var timesheet = new Timesheet();

            timesheet.TimesheetID = reader.GetInt32(reader.GetOrdinal("TimesheetID"));
            timesheet.EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID"));
            timesheet.WorkDate = reader.GetDateTime(reader.GetOrdinal("WorkDate"));
            timesheet.TimeIn = ReadTime(reader, "TimeIn");
            timesheet.TimeOut = ReadTime(reader, "TimeOut");
            timesheet.Break1Start = ReadTime(reader, "Break1Start");
            timesheet.Break1End = ReadTime(reader, "Break1End");
            timesheet.Break2Start = ReadTime(reader, "Break2Start");
            timesheet.Break2End = ReadTime(reader, "Break2End");
            timesheet.DayType = DayTypes.FromDatabaseValue(reader.GetString(reader.GetOrdinal("DayType")));
            timesheet.Status = TimesheetStatuses.FromDatabaseValue(reader.GetString(reader.GetOrdinal("Status")));
            timesheet.CapturedBy = reader.GetInt32(reader.GetOrdinal("CapturedBy"));
            timesheet.BusinessDate = reader.GetDateTime(reader.GetOrdinal("BusinessDate"));

            int notesIndex = reader.GetOrdinal("Notes");
            if (reader.IsDBNull(notesIndex))
            {
                timesheet.Notes = null;
            }
            else
            {
                timesheet.Notes = reader.GetString(notesIndex);
            }

            return timesheet;
        }

        // Reads a nullable time column.
        private static TimeSpan? ReadTime(MySqlDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(index))
            {
                return null;
            }

            return reader.GetTimeSpan(index);
        }
    }
}
