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
    // The sheet an employee signs to confirm their hours for a period: every day with its
    // times, the totals, a declaration and signature lines for the employee and a manager.
    // Each employee starts on a new A4 page, so the pages can be handed out separately.
    internal static class TimesheetSignOffPdf
    {
        private const double RowHeight = 13;

        // Totals, declaration and signatures, which are kept together on one page. With
        // the rows above, a whole month of single shifts fits on one page.
        private const double ClosingHeight = 230;

        private static readonly string[] Headers = { "Date", "Day", "Status", "In", "Out", "Breaks", "Hours", "Notes" };
        private static readonly double[] Widths = { 60, 64, 62, 34, 34, 122, 38, 0 };
        private static readonly ISet<int> NumberColumns = new HashSet<int> { 6 };

        public static void Save(IList<SignOffSheet> sheets, string filePath)
        {
            using (var document = new PdfDocument())
            {
                document.Info.Title = sheets.Count == 1
                    ? "Timesheet sign-off: " + sheets[0].EmployeeName
                    : "Timesheet sign-off sheets";

                foreach (SignOffSheet sheet in sheets)
                {
                    new Writer(document, sheet).Write();
                }

                document.Save(filePath);
            }
        }

        // Draws one employee's pages, then numbers them once it knows how many there are.
        private sealed class Writer
        {
            private readonly PdfDocument document;
            private readonly SignOffSheet sheet;
            private readonly List<PdfPage> pages = new List<PdfPage>();
            private readonly double[] widths = (double[])Widths.Clone();
            private PdfPage page;
            private XGraphics graphics;
            private double y;

            public Writer(PdfDocument document, SignOffSheet sheet)
            {
                this.document = document;
                this.sheet = sheet;
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
                widths[widths.Length - 1] = Right - Margin - widths.Take(widths.Length - 1).Sum();
                DrawTableHeader();

                bool shaded = false;
                foreach (SignOffLine line in sheet.Lines)
                {
                    if (y + RowHeight > Bottom)
                    {
                        NewPage(true);
                        DrawTableHeader();
                    }

                    // Shading follows the date, so the lines of one day stay together.
                    if (line.ShowDate)
                    {
                        shaded = !shaded;
                    }

                    DrawLine(line, shaded);
                }

                if (y + 12 + ClosingHeight > Bottom)
                {
                    NewPage(true);
                }

                y += 12;
                DrawTotals();
                DrawDeclaration();

                graphics.Dispose();
                NumberPages(pages, sheet.EmployeeName);
            }

            private void NewPage(bool continued)
            {
                if (graphics != null)
                {
                    graphics.Dispose();
                }

                page = AddPage(document, "TIMESHEET SIGN-OFF", sheet.StoreName, out graphics, out y);
                pages.Add(page);

                if (continued)
                {
                    graphics.DrawString(sheet.EmployeeName + " (continued)", BoldFont, XBrushes.Black, Margin, y);
                    y += 8;
                    return;
                }

                graphics.DrawString(sheet.EmployeeName, NameFont, XBrushes.Black, Margin, y);
                y += 14;

                string period = string.Format(CultureInfo.InvariantCulture, "Period: {0} to {1}   ·   Employee no. {2}",
                    LongDate(sheet.FromDate), LongDate(sheet.ToDate), sheet.EmployeeID);
                graphics.DrawString(period, BodyFont, new XSolidBrush(Charcoal), Margin, y);
                y += 12;

                graphics.DrawString("Please check every day below. Days marked \"Not captured\" have no timesheet.",
                    SmallFont, new XSolidBrush(Muted), Margin, y);
                y += 9;
            }

            private void DrawTableHeader()
            {
                graphics.DrawRectangle(new XSolidBrush(Charcoal), Margin, y, Right - Margin, RowHeight + 2);
                DrawCells(graphics, Headers, widths, NumberColumns, HeadFont, XBrushes.White, y, RowHeight + 2);
                y += RowHeight + 2;
            }

            private void DrawLine(SignOffLine line, bool shaded)
            {
                // Sundays and public holidays are tinted, since they pay extra.
                XColor fill = !string.IsNullOrEmpty(line.Day) ? Tint : shaded ? Shade : XColors.White;
                graphics.DrawRectangle(new XSolidBrush(fill), Margin, y, Right - Margin, RowHeight);

                string[] cells =
                {
                    line.ShowDate ? line.Date.ToString("ddd dd MMM", CultureInfo.InvariantCulture) : string.Empty,
                    line.Day ?? string.Empty,
                    line.Status,
                    line.TimeIn ?? string.Empty,
                    line.TimeOut ?? string.Empty,
                    line.Breaks ?? string.Empty,
                    line.Hours.HasValue ? line.Hours.Value.ToString("0.00", CultureInfo.InvariantCulture) : string.Empty,
                    line.Notes ?? string.Empty
                };

                XFont font = line.IsMissing ? ItalicFont : BodyFont;
                XBrush brush = line.IsMissing ? new XSolidBrush(Muted) : XBrushes.Black;
                DrawCells(graphics, cells, widths, NumberColumns, font, brush, y, RowHeight);

                graphics.DrawLine(new XPen(Line, 0.5), Margin, y + RowHeight, Right, y + RowHeight);
                y += RowHeight;
            }

            private void DrawTotals()
            {
                double top = y;
                double width = Right - Margin;
                graphics.DrawRectangle(new XPen(Line, 0.75), new XSolidBrush(Shade), Margin, top, width, 72);

                y = top + 15;
                graphics.DrawString("TOTALS FOR THE PERIOD", HeadFont, new XSolidBrush(Red), Margin + 10, y);
                y += 13;

                // Hours down the left, days down the right. Sunday and public holiday extra are
                // apart, since a Sunday hour earns half extra and a public holiday hour a whole one.
                double left = Margin + 10;
                double half = Margin + width / 2 + 10;
                TotalRow(graphics, left, y, "Hours worked", Hours(sheet.TotalHours), true);
                TotalRow(graphics, half, y, "Days worked", sheet.DaysWorked.ToString(CultureInfo.InvariantCulture), false);
                y += 12;
                TotalRow(graphics, left, y, "Sunday extra (time and a half)", Hours(sheet.SundayExtraHours), false);
                TotalRow(graphics, half, y, "Leave days", sheet.LeaveDays.ToString(CultureInfo.InvariantCulture), false);
                y += 12;
                TotalRow(graphics, left, y, "Public holiday extra (double time)", Hours(sheet.HolidayExtraHours), false);
                TotalRow(graphics, half, y, "Days off / AWOL", sheet.OffDays + " / " + sheet.AwolDays, false);
                y += 12;
                TotalRow(graphics, left, y, "Payable hours", Hours(sheet.PayableHours), true);
                TotalRow(graphics, half, y, "Days not captured", sheet.MissingDays.ToString(CultureInfo.InvariantCulture), sheet.MissingDays > 0);

                y = top + 72 + 21;
            }

            private void DrawDeclaration()
            {
                var text = new PdfSharp.Drawing.Layout.XTextFormatter(graphics);
                string declaration = string.Format(CultureInfo.InvariantCulture,
                    "I, {0}, confirm that the days, times and hours shown above are a true and complete record of the "
                    + "hours I worked from {1} to {2}.",
                    sheet.EmployeeName, LongDate(sheet.FromDate), LongDate(sheet.ToDate));
                text.DrawString(declaration, BodyFont, XBrushes.Black, new XRect(Margin, y - 9, Right - Margin, 24));
                y += 25;

                graphics.DrawString("Anything wrong? Write the date and the correct times here before signing:",
                    SmallFont, new XSolidBrush(Muted), Margin, y);
                y += 19;
                graphics.DrawLine(new XPen(Line, 0.75), Margin, y, Right, y);
                y += 19;
                graphics.DrawLine(new XPen(Line, 0.75), Margin, y, Right, y);
                y += 30;

                // One row each, with the date lines lined up: the employee signs, then the
                // manager who checked the sheet with them.
                const double labelWidth = 72;
                const double gap = 16;
                const double dateWidth = 100;
                double left = Margin + labelWidth;
                double dateX = Right - dateWidth;
                double nameWidth = (dateX - gap - left - gap) / 2;

                graphics.DrawString("EMPLOYEE", HeadFont, new XSolidBrush(Red), Margin, y - 2);
                SignatureLine(graphics, left, y, dateX - gap - left, "Employee signature");
                SignatureLine(graphics, dateX, y, dateWidth, "Date");
                y += 34;

                graphics.DrawString("MANAGER", HeadFont, new XSolidBrush(Red), Margin, y - 2);
                SignatureLine(graphics, left, y, nameWidth, "Manager name");
                SignatureLine(graphics, left + nameWidth + gap, y, nameWidth, "Manager signature");
                SignatureLine(graphics, dateX, y, dateWidth, "Date");
            }
        }
    }
}
