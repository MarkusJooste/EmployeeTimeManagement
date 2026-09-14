using System;
using System.Collections.Generic;
using EmployeeTimeManagement.Database;
using EmployeeTimeManagement.Models;
using MySqlConnector;

namespace EmployeeTimeManagement.Controllers
{
    internal class LeaveController
    {
        // Returns every Absence booked for one employee, scoped to the manager's store by
        // joining through the employee, newest first, covering the whole employment rather
        // than one cycle.
        public List<Absence> GetHistoryForEmployee(int employeeID, int storeID)
        {
            const string query = @"SELECT l.LeaveID, l.EmployeeID, l.LeaveType, l.StartDate, l.EndDate, l.Reason, l.OverrideReason
FROM TBL_leave l
JOIN TBL_employees e ON e.EmployeeID = l.EmployeeID
WHERE l.EmployeeID = @EmployeeID
  AND e.StoreID = @StoreID
ORDER BY l.StartDate DESC, l.LeaveID DESC;";

            var absences = new List<Absence>();

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);
                    command.Parameters.AddWithValue("@StoreID", storeID);

                    using (var reader = command.ExecuteReader())
                    {
                        int leaveIDIndex = reader.GetOrdinal("LeaveID");
                        int employeeIDIndex = reader.GetOrdinal("EmployeeID");
                        int leaveTypeIndex = reader.GetOrdinal("LeaveType");
                        int startDateIndex = reader.GetOrdinal("StartDate");
                        int endDateIndex = reader.GetOrdinal("EndDate");
                        int reasonIndex = reader.GetOrdinal("Reason");
                        int overrideReasonIndex = reader.GetOrdinal("OverrideReason");

                        while (reader.Read())
                        {
                            absences.Add(new Absence
                            {
                                LeaveID = reader.GetInt32(leaveIDIndex),
                                EmployeeID = reader.GetInt32(employeeIDIndex),
                                LeaveType = LeaveTypes.FromDatabaseValue(reader.GetString(leaveTypeIndex)),
                                StartDate = reader.GetDateTime(startDateIndex),
                                EndDate = reader.GetDateTime(endDateIndex),
                                Reason = ReadNullableText(reader, reasonIndex),
                                OverrideReason = ReadNullableText(reader, overrideReasonIndex)
                            });
                        }
                    }
                }
            }

            return absences;
        }

        // Reads a nullable text column as null, so an Absence with no reason round-trips as genuinely empty.
        private static string ReadNullableText(MySqlDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? null : reader.GetString(index);
        }

        // Writes one booked Absence. Only called with an Absence LeaveBooking has already
        // validated, so a refused booking never reaches this far.
        public void Insert(Absence absence, int capturedBy)
        {
            const string query = @"INSERT INTO TBL_leave
    (EmployeeID, LeaveType, StartDate, EndDate, Reason, OverrideReason, BusinessDate, CapturedBy)
VALUES
    (@EmployeeID, @LeaveType, @StartDate, @EndDate, @Reason, @OverrideReason, @BusinessDate, @CapturedBy);";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", absence.EmployeeID);
                    command.Parameters.AddWithValue("@LeaveType", absence.LeaveType.ToDatabaseValue());
                    command.Parameters.AddWithValue("@StartDate", absence.StartDate.Date);
                    command.Parameters.AddWithValue("@EndDate", absence.EndDate.Date);
                    command.Parameters.AddWithValue("@Reason", ToParameter(absence.Reason));
                    command.Parameters.AddWithValue("@OverrideReason", ToParameter(absence.OverrideReason));
                    command.Parameters.AddWithValue("@BusinessDate", DateTime.Today);
                    command.Parameters.AddWithValue("@CapturedBy", capturedBy);

                    command.ExecuteNonQuery();

                    absence.LeaveID = (int)command.LastInsertedId;
                }
            }
        }

        // Counts employees at one store with an Absence, of any Leave Type, covering today -
        // the same question the Leave view answers for one employee at a time, so the
        // dashboard's tile and this screen can never disagree.
        public int CountOnLeaveToday(int storeID, DateTime today)
        {
            const string query = @"SELECT COUNT(DISTINCT l.EmployeeID)
FROM TBL_leave l
JOIN TBL_employees e ON e.EmployeeID = l.EmployeeID
WHERE e.StoreID = @StoreID
  AND @Today BETWEEN l.StartDate AND l.EndDate;";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StoreID", storeID);
                    command.Parameters.AddWithValue("@Today", today.Date);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        // Converts an absent value to the DBNull the driver expects.
        private static object ToParameter(object value)
        {
            return value ?? DBNull.Value;
        }
    }
}
