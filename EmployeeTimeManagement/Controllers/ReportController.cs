using EmployeeTimeManagement.Models;
using EmployeeTimeManagement.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EmployeeTimeManagement.Controllers
{
    internal class ReportController
    {
        public string GenerateTimesheetReport(
            DateTime fromDate,
            DateTime toDate,
            System.Collections.Generic.List<TimesheetSummary> rows)
        {
            string filePath = GetReportFilePath(fromDate, toDate);

            var report = new TimesheetReport
            {
                FromDate = fromDate.Date,
                ToDate = toDate.Date,
                Rows = rows
            };

            TimesheetSummaryPdf.Save(report, filePath);

            return filePath;
        }

        // Saves one sign-off sheet per employee into a single PDF and returns its path. One
        // employee's file is named after them, so it can be found again for that person.
        public string GenerateSignOffSheets(
            DateTime fromDate,
            DateTime toDate,
            IList<SignOffSheet> sheets)
        {
            string who = sheets.Count == 1
                ? sheets[0].EmployeeName
                : $"{sheets.Count} employees";

            string filePath = GetReportFilePath(
                $"Timesheet Sign-off {who} {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}.pdf");

            TimesheetSignOffPdf.Save(sheets, filePath);

            return filePath;
        }

        private static string GetReportFilePath(
            DateTime fromDate,
            DateTime toDate)
        {
            return GetReportFilePath(
                $"Store Timesheet Summary {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}.pdf");
        }

        private static string GetReportFilePath(string fileName)
        {
            string documentsPath =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string reportsFolder =
                Path.Combine(documentsPath, "EmployeeTimeManagement Reports");

            Directory.CreateDirectory(reportsFolder);

            // A name can hold characters Windows does not allow in a file name.
            foreach (char invalid in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalid, '_');
            }

            return Path.Combine(reportsFolder, fileName);
        }
    }
}
