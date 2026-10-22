using EmployeeTimeManagement.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
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

            GeneratePdf(report, filePath);

            return filePath;
        }

        private static string GetReportFilePath(
            DateTime fromDate,
            DateTime toDate)
        {
            string documentsPath =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string reportsFolder =
                Path.Combine(documentsPath, "EmployeeTimeManagement Reports");

            Directory.CreateDirectory(reportsFolder);

            string fileName =
                $"Timesheet Report {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}.pdf";

            return Path.Combine(reportsFolder, fileName);
        }

        private static void GeneratePdf(
            TimesheetReport report,
            string filePath)
        {
            using (var document = new PdfDocument())
            {
                document.Info.Title = "Employee Timesheet Report";
                document.Info.Subject =
                    $"Timesheet report {report.FromDate:dd MMM yyyy} - {report.ToDate:dd MMM yyyy}";

                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                page.Orientation = PdfSharp.PageOrientation.Landscape;

                XGraphics graphics = XGraphics.FromPdfPage(page);

                XFont titleFont =
                    new XFont("Arial", 18, XFontStyleEx.Bold);

                XFont headingFont =
                    new XFont("Arial", 11, XFontStyleEx.Bold);

                XFont normalFont =
                    new XFont("Arial", 9, XFontStyleEx.Regular);

                XFont smallFont =
                    new XFont("Arial", 8, XFontStyleEx.Regular);

                double margin = 40;
                double y = margin;

                // Header

                graphics.DrawString(
                    "EMPLOYEE TIMESHEET REPORT",
                    titleFont,
                    XBrushes.Black,
                    new XPoint(margin, y));

                y += 30;

                graphics.DrawString(
                    $"Period: {report.FromDate:dd MMM yyyy} - {report.ToDate:dd MMM yyyy}",
                    normalFont,
                    XBrushes.Black,
                    new XPoint(margin, y));

                y += 16;

                graphics.DrawString(
                    $"Generated: {DateTime.Now:dd MMM yyyy HH:mm}",
                    normalFont,
                    XBrushes.Black,
                    new XPoint(margin, y));

                y += 30;

                // Table

                double[] columnWidths =
                {
                    150, // Employee
                    65,  // Days
                    85,  // Total Hours
                    95,  // Sunday
                    105, // Public Holiday
                    95,  // Payable
                    95   // Pay
                };

                string[] headers =
                {
                    "Employee",
                    "Days",
                    "Total Hours",
                    "Sunday Extra",
                    "Holiday Extra",
                    "Payable Hours",
                    "Pay"
                };

                double tableX = margin;
                double rowHeight = 24;

                DrawTableHeader(
                    graphics,
                    headers,
                    columnWidths,
                    tableX,
                    y,
                    rowHeight,
                    headingFont);

                y += rowHeight;

                foreach (TimesheetSummary row in report.Rows)
                {
                    if (y + rowHeight > page.Height - 130)
                    {
                        graphics.Dispose();

                        page = document.AddPage();
                        page.Size = PdfSharp.PageSize.A4;
                        page.Orientation = PdfSharp.PageOrientation.Landscape;

                        graphics = XGraphics.FromPdfPage(page);

                        y = margin;

                        graphics.DrawString(
                            "EMPLOYEE TIMESHEET REPORT",
                            headingFont,
                            XBrushes.Black,
                            new XPoint(margin, y));

                        y += 25;

                        DrawTableHeader(
                            graphics,
                            headers,
                            columnWidths,
                            tableX,
                            y,
                            rowHeight,
                            headingFont);

                        y += rowHeight;
                    }

                    string[] values =
                    {
                        $"{row.Name} {row.Surname}",
                        row.DaysWorked.ToString(),
                        row.TotalHours.ToString("N2"),
                        row.SundayExtraHours.ToString("N2"),
                        row.HolidayExtraHours.ToString("N2"),
                        row.PayableHours.ToString("N2"),
                        row.PayableHours.ToString("N2")
                    };

                    DrawTableRow(
                        graphics,
                        values,
                        columnWidths,
                        tableX,
                        y,
                        rowHeight,
                        normalFont);

                    y += rowHeight;
                }

                // Totals

                y += 10;

                if (y + 100 > page.Height)
                {
                    graphics.Dispose();

                    page = document.AddPage();
                    page.Size = PdfSharp.PageSize.A4;
                    page.Orientation = PdfSharp.PageOrientation.Landscape;

                    graphics = XGraphics.FromPdfPage(page);

                    y = margin;
                }

                graphics.DrawLine(
                    XPens.Black,
                    margin,
                    y,
                    page.Width - margin,
                    y);

                y += 22;

                graphics.DrawString(
                    "REPORT TOTALS",
                    headingFont,
                    XBrushes.Black,
                    new XPoint(margin, y));

                y += 20;

                graphics.DrawString(
                    $"Employees: {report.Rows.Count}",
                    normalFont,
                    XBrushes.Black,
                    new XPoint(margin, y));

                y += 16;

                graphics.DrawString(
                    $"Total days worked: {report.TotalDaysWorked}",
                    normalFont,
                    XBrushes.Black,
                    new XPoint(margin, y));

                y += 16;

                graphics.DrawString(
                    $"Total hours: {report.TotalHours:N2}",
                    normalFont,
                    XBrushes.Black,
                    new XPoint(margin, y));

                y += 16;

                graphics.DrawString(
                    $"Sunday extra hours: {report.TotalSundayExtraHours:N2}",
                    normalFont,
                    XBrushes.Black,
                    new XPoint(margin, y));

                y += 16;

                graphics.DrawString(
                    $"Public holiday extra hours: {report.TotalHolidayExtraHours:N2}",
                    normalFont,
                    XBrushes.Black,
                    new XPoint(margin, y));

                y += 16;

                graphics.DrawString(
                    $"Total payable hours: {report.TotalPayableHours:N2}",
                    headingFont,
                    XBrushes.Black,
                    new XPoint(margin, y));

                y += 16;

                graphics.DrawString(
                    $"Total pay: {report.TotalPay:N2}",
                    headingFont,
                    XBrushes.Black,
                    new XPoint(margin, y));

                y += 30;

                graphics.Dispose();

                document.Save(filePath);
            }
        }

        private static void DrawTableHeader(
            XGraphics graphics,
            string[] headers,
            double[] widths,
            double x,
            double y,
            double height,
            XFont font)
        {
            double currentX = x;

            for (int i = 0; i < headers.Length; i++)
            {
                XRect rect =
                    new XRect(currentX, y, widths[i], height);

                graphics.DrawRectangle(
                    XBrushes.LightGray,
                    rect);

                graphics.DrawRectangle(
                    XPens.Black,
                    rect);

                graphics.DrawString(
                    headers[i],
                    font,
                    XBrushes.Black,
                    rect,
                    XStringFormats.Center);

                currentX += widths[i];
            }
        }

        private static void DrawTableRow(
            XGraphics graphics,
            string[] values,
            double[] widths,
            double x,
            double y,
            double height,
            XFont font)
        {
            double currentX = x;

            for (int i = 0; i < values.Length; i++)
            {
                XRect rect =
                    new XRect(currentX, y, widths[i], height);

                graphics.DrawRectangle(
                    XPens.Black,
                    rect);

                XStringFormat format =
                    i == 0
                        ? XStringFormats.CenterLeft
                        : XStringFormats.Center;

                graphics.DrawString(
                    values[i],
                    font,
                    XBrushes.Black,
                    rect,
                    format);

                currentX += widths[i];
            }
        }
    }
}
