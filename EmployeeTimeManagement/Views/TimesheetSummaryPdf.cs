using EmployeeTimeManagement.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static EmployeeTimeManagement.Views.ReportPdf;

namespace EmployeeTimeManagement.Views
{
    // The store's hours for a period on one sheet, for payroll: a line per employee with
    // their hours and premiums, the store's totals, and a line for the manager who checked it.
    // Drawn in the same style as the sign-off sheets, which back up each line.
    internal static class TimesheetSummaryPdf
    {
        private const double RowHeight = 15;

        // The totals box and the manager's sign-off, which are kept together on one page.
        private const double ClosingHeight = 150;

        private static readonly string[] Headers =
            { "Employee", "Days", "Hours worked", "Sunday extra", "Holiday extra", "Payable hours" };
        private static readonly double[] Widths = { 0, 50, 80, 80, 80, 85 };
        private static readonly ISet<int> NumberColumns = new HashSet<int> { 1, 2, 3, 4, 5 };

        public static void Save(TimesheetReport report, string filePath)
        {
            using (var document = new PdfDocument())
            {
                document.Info.Title = "Store timesheet summary";
                new Writer(document, report).Write();
                document.Save(filePath);
            }
        }

        private sealed class Writer
        {
            private readonly PdfDocument document;
            private readonly TimesheetReport report;
            private readonly List<PdfPage> pages = new List<PdfPage>();
            private readonly double[] widths = (double[])Widths.Clone();
            private PdfPage page;
            private XGraphics graphics;
            private double y;

            public Writer(PdfDocument document, TimesheetReport report)
            {
                this.document = document;
                this.report = report;
            }

            private double Right
            {
                get { return page.Width.Point - Margin; }
            }

            private double Bottom
            {
                get { return page.Height.Point - Margin - FooterHeight; }
            }

            public void Write()
            {
                NewPage(false);
                widths[0] = Right - Margin - widths.Skip(1).Sum();
                DrawTableHeader();

                bool shaded = false;
                foreach (TimesheetSummary row in report.Rows)
                {
                    if (y + RowHeight > Bottom)
                    {
                        NewPage(true);
                        DrawTableHeader();
                    }

                    shaded = !shaded;
                    DrawRow(row, shaded);
                }

                // The totals line stays under the last employee rather than starting a page.
                if (y + RowHeight + 2 > Bottom)
                {
                    NewPage(true);
                    DrawTableHeader();
                }

                DrawTotalsLine();

                if (y + 16 + ClosingHeight > Bottom)
                {
                    NewPage(true);
                }

                y += 16;
                DrawTotals();
                DrawSignOff();

                graphics.Dispose();
                NumberPages(pages, "Store timesheet summary   ·   " + report.StoreName);
            }

            private void NewPage(bool continued)
            {
                if (graphics != null)
                {
                    graphics.Dispose();
                }

                page = AddPage(document, "STORE TIMESHEET SUMMARY", report.StoreName, out graphics, out y);
                pages.Add(page);

                string period = string.Format(CultureInfo.InvariantCulture, "{0} to {1}",
                    LongDate(report.FromDate), LongDate(report.ToDate));

                if (continued)
                {
                    graphics.DrawString(period + " (continued)", BoldFont, XBrushes.Black, Margin, y);
                    y += 8;
                    return;
                }

                graphics.DrawString(period, NameFont, XBrushes.Black, Margin, y);
                y += 14;

                string count = report.Rows.Count == 1 ? "1 employee" : report.Rows.Count + " employees";
                graphics.DrawString(count + " with hours captured in this period", BodyFont, new XSolidBrush(Charcoal), Margin, y);
                y += 12;

                graphics.DrawString("Hours worked are after breaks. Sunday hours earn half extra (time and a half); "
                    + "public holiday hours a whole extra (double time).", SmallFont, new XSolidBrush(Muted), Margin, y);
                y += 9;
            }

            private void DrawTableHeader()
            {
                graphics.DrawRectangle(new XSolidBrush(Charcoal), Margin, y, Right - Margin, RowHeight + 2);
                DrawCells(graphics, Headers, widths, NumberColumns, HeadFont, XBrushes.White, y, RowHeight + 2);
                y += RowHeight + 2;
            }

            private void DrawRow(TimesheetSummary row, bool shaded)
            {
                graphics.DrawRectangle(new XSolidBrush(shaded ? Shade : XColors.White), Margin, y, Right - Margin, RowHeight);

                string[] cells =
                {
                    (row.Name + " " + row.Surname).Trim(),
                    row.DaysWorked.ToString(CultureInfo.InvariantCulture),
                    Number(row.TotalHours),
                    Number(row.SundayExtraHours),
                    Number(row.HolidayExtraHours),
                    Number(row.PayableHours)
                };

                DrawCells(graphics, cells, widths, NumberColumns, BodyFont, XBrushes.Black, y, RowHeight);
                graphics.DrawLine(new XPen(Line, 0.5), Margin, y + RowHeight, Right, y + RowHeight);
                y += RowHeight;
            }

            // The column totals, in bold under a heavier rule, so the table adds up on its own.
            private void DrawTotalsLine()
            {
                graphics.DrawRectangle(new XSolidBrush(Tint), Margin, y, Right - Margin, RowHeight + 2);
                graphics.DrawLine(new XPen(Charcoal, 1), Margin, y, Right, y);

                string[] cells =
                {
                    "Total",
                    report.TotalDaysWorked.ToString(CultureInfo.InvariantCulture),
                    Number(report.TotalHours),
                    Number(report.TotalSundayExtraHours),
                    Number(report.TotalHolidayExtraHours),
                    Number(report.TotalPayableHours)
                };

                DrawCells(graphics, cells, widths, NumberColumns, BoldFont, XBrushes.Black, y, RowHeight + 2);
                y += RowHeight + 2;
            }

            private void DrawTotals()
            {
                double top = y;
                double width = Right - Margin;
                graphics.DrawRectangle(new XPen(Line, 0.75), new XSolidBrush(Shade), Margin, top, width, 72);

                y = top + 15;
                graphics.DrawString("STORE TOTALS FOR THE PERIOD", HeadFont, new XSolidBrush(Red), Margin + 10, y);
                y += 13;

                // Hours down the left, as on the sign-off sheets; the head count on the right.
                double left = Margin + 10;
                double half = Margin + width / 2 + 10;
                TotalRow(graphics, left, y, "Hours worked", Hours(report.TotalHours), true);
                TotalRow(graphics, half, y, "Employees", report.Rows.Count.ToString(CultureInfo.InvariantCulture), false);
                y += 12;
                TotalRow(graphics, left, y, "Sunday extra (time and a half)", Hours(report.TotalSundayExtraHours), false);
                TotalRow(graphics, half, y, "Days worked", report.TotalDaysWorked.ToString(CultureInfo.InvariantCulture), false);
                y += 12;
                TotalRow(graphics, left, y, "Public holiday extra (double time)", Hours(report.TotalHolidayExtraHours), false);
                y += 12;
                TotalRow(graphics, left, y, "Payable hours", Hours(report.TotalPayableHours), true);

                y = top + 72 + 21;
            }

            private void DrawSignOff()
            {
                graphics.DrawString("I have checked these hours against the employees' signed sign-off sheets.",
                    BodyFont, XBrushes.Black, Margin, y);
                y += 34;

                const double labelWidth = 72;
                const double gap = 16;
                const double dateWidth = 100;
                double left = Margin + labelWidth;
                double dateX = Right - dateWidth;
                double nameWidth = (dateX - gap - left - gap) / 2;

                graphics.DrawString("MANAGER", HeadFont, new XSolidBrush(Red), Margin, y - 2);
                SignatureLine(graphics, left, y, nameWidth, "Manager name");
                SignatureLine(graphics, left + nameWidth + gap, y, nameWidth, "Manager signature");
                SignatureLine(graphics, dateX, y, dateWidth, "Date");
            }
        }

        private static string Number(decimal hours)
        {
            return hours.ToString("0.00", CultureInfo.InvariantCulture);
        }
    }
}
