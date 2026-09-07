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
    }
}
