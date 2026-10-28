using System;
using System.Collections.Generic;
using EmployeeTimeManagement.Database;
using EmployeeTimeManagement.Models;
using MySqlConnector;

namespace EmployeeTimeManagement.Controllers
{
    // Read-only queries behind the dashboard's lists. A null storeID is the owner, who sees
    // every store; a manager passes their own. Each returns a Query, so the dashboard can send
    // them all to the server in one batch.
    internal class DashboardController
    {
        // Every employee with their latest contract and whether the records their contract
        // needs (address, next of kin) have been captured.
        public Query<List<EmployeeOverview>> EmployeeOverviewQuery(int? storeID)
        {
            const string query = @"SELECT e.EmployeeID,
       e.StoreID,
       s.StoreName,
       e.Name,
       e.Surname,
       e.IDNumber,
       c.StartDate,
       c.EndDate,
       c.ReasonForEnding,
       c.HourlyRate,
       EXISTS (SELECT 1
               FROM TBL_employee_addresses a
               WHERE a.EmployeeID = e.EmployeeID) AS HasAddress,
       (EXISTS (SELECT 1
                FROM TBL_employee_family f
                WHERE f.EmployeeID = e.EmployeeID
                  AND TRIM(f.FamilyMemberName) <> '')
        OR EXISTS (SELECT 1
                   FROM TBL_employee_spouses sp
                   WHERE sp.EmployeeID = e.EmployeeID
                     AND TRIM(sp.SpouseName) <> '')) AS HasNextOfKin
FROM TBL_employees e
LEFT JOIN TBL_stores s ON s.StoreID = e.StoreID
LEFT JOIN TBL_employee_contracts c
       ON c.EmployeeID = e.EmployeeID
      AND c.ContractID = (SELECT MAX(latest.ContractID)
                          FROM TBL_employee_contracts latest
                          WHERE latest.EmployeeID = e.EmployeeID)
WHERE (@StoreID IS NULL OR e.StoreID = @StoreID);";

            return new Query<List<EmployeeOverview>>(query, reader =>
            {
                var employees = new List<EmployeeOverview>();

                int employeeIDIndex = reader.GetOrdinal("EmployeeID");
                int storeIDIndex = reader.GetOrdinal("StoreID");
                int storeNameIndex = reader.GetOrdinal("StoreName");
                int nameIndex = reader.GetOrdinal("Name");
                int surnameIndex = reader.GetOrdinal("Surname");
                int idNumberIndex = reader.GetOrdinal("IDNumber");
                int startDateIndex = reader.GetOrdinal("StartDate");
                int endDateIndex = reader.GetOrdinal("EndDate");
                int reasonForEndingIndex = reader.GetOrdinal("ReasonForEnding");
                int hourlyRateIndex = reader.GetOrdinal("HourlyRate");
                int hasAddressIndex = reader.GetOrdinal("HasAddress");
                int hasNextOfKinIndex = reader.GetOrdinal("HasNextOfKin");

                while (reader.Read())
                {
                    employees.Add(new EmployeeOverview
                    {
                        EmployeeID = reader.GetInt32(employeeIDIndex),
                        StoreID = reader.GetInt32(storeIDIndex),
                        StoreName = ReadText(reader, storeNameIndex),
                        Name = ReadText(reader, nameIndex),
                        Surname = ReadText(reader, surnameIndex),
                        IDNumber = ReadText(reader, idNumberIndex),
                        ContractStartDate = reader.IsDBNull(startDateIndex) ? (DateTime?)null : reader.GetDateTime(startDateIndex),
                        ContractEndDate = reader.IsDBNull(endDateIndex) ? (DateTime?)null : reader.GetDateTime(endDateIndex),
                        ReasonForEnding = ReadText(reader, reasonForEndingIndex),
                        HourlyRate = reader.IsDBNull(hourlyRateIndex) ? (decimal?)null : reader.GetDecimal(hourlyRateIndex),

                        // EXISTS comes back as a 0/1 integer.
                        HasAddress = Convert.ToInt64(reader.GetValue(hasAddressIndex)) != 0,
                        HasNextOfKin = Convert.ToInt64(reader.GetValue(hasNextOfKinIndex)) != 0
                    });
                }

                return employees;
            }).With("@StoreID", storeID);
        }

        // Every employee and day between two dates that has a saved Timesheet, whatever its Status.
        public Query<List<CapturedDay>> CapturedDaysQuery(DateTime fromDate, DateTime toDate, int? storeID)
        {
            const string query = @"SELECT DISTINCT t.EmployeeID, t.WorkDate
FROM TBL_timesheets t
JOIN TBL_employees e ON e.EmployeeID = t.EmployeeID
WHERE t.WorkDate BETWEEN @FromDate AND @ToDate
  AND (@StoreID IS NULL OR e.StoreID = @StoreID);";

            return new Query<List<CapturedDay>>(query, reader =>
            {
                var days = new List<CapturedDay>();

                int employeeIDIndex = reader.GetOrdinal("EmployeeID");
                int workDateIndex = reader.GetOrdinal("WorkDate");

                while (reader.Read())
                {
                    days.Add(new CapturedDay(reader.GetInt32(employeeIDIndex), reader.GetDateTime(workDateIndex)));
                }

                return days;
            })
            .With("@FromDate", fromDate.Date)
            .With("@ToDate", toDate.Date)
            .With("@StoreID", storeID);
        }

        // How many Timesheets between two dates were saved as AWOL. Timesheets are where AWOL
        // is recorded; the AWOL Absences are mirrored from them (ADR-0002).
        public Query<int> AwolDaysQuery(DateTime fromDate, DateTime toDate, int? storeID)
        {
            const string query = @"SELECT COUNT(*)
FROM TBL_timesheets t
JOIN TBL_employees e ON e.EmployeeID = t.EmployeeID
WHERE t.WorkDate BETWEEN @FromDate AND @ToDate
  AND t.Status = @AWOL
  AND (@StoreID IS NULL OR e.StoreID = @StoreID);";

            return new Query<int>(query, QueryBatch.ReadInt)
                .With("@FromDate", fromDate.Date)
                .With("@ToDate", toDate.Date)
                .With("@AWOL", TimesheetStatus.AWOL.ToDatabaseValue())
                .With("@StoreID", storeID);
        }

        private static string ReadText(MySqlDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? string.Empty : reader.GetString(index);
        }
    }
}
