using EmployeeTimeManagement.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace EmployeeTimeManagement.Views
{
    // The look the timesheet PDFs share: the red rule and title at the top of each page, the
    // charcoal table headings, and the printed date and page numbers at the bottom.
    internal static class ReportPdf
    {
        public const double Margin = 36;
        public const double FooterHeight = 18;

        public static readonly XColor Red = XColor.FromArgb(190, 6, 19);
        public static readonly XColor Charcoal = XColor.FromArgb(45, 45, 45);
        public static readonly XColor Muted = XColor.FromArgb(108, 112, 117);
        public static readonly XColor Line = XColor.FromArgb(200, 197, 193);
        public static readonly XColor Shade = XColor.FromArgb(246, 245, 243);
        public static readonly XColor Tint = XColor.FromArgb(252, 232, 233);

        public static readonly XFont TitleFont = new XFont("Arial", 15, XFontStyleEx.Bold);
        public static readonly XFont NameFont = new XFont("Arial", 13, XFontStyleEx.Bold);
        public static readonly XFont HeadFont = new XFont("Arial", 8, XFontStyleEx.Bold);
        public static readonly XFont BodyFont = new XFont("Arial", 8.5, XFontStyleEx.Regular);
        public static readonly XFont BoldFont = new XFont("Arial", 8.5, XFontStyleEx.Bold);
        public static readonly XFont ItalicFont = new XFont("Arial", 8.5, XFontStyleEx.Italic);
        public static readonly XFont SmallFont = new XFont("Arial", 7.5, XFontStyleEx.Regular);

        // Adds an A4 portrait page with the red rule, the title and the store name drawn on
        // it, and returns where the content below them starts.
        public static PdfPage AddPage(PdfDocument document, string title, string storeName, out XGraphics graphics, out double y)
        {
            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            page.Orientation = PdfSharp.PageOrientation.Portrait;

            graphics = XGraphics.FromPdfPage(page);
            double right = page.Width.Point - Margin;
            y = Margin;

            graphics.DrawRectangle(new XSolidBrush(Red), Margin, y, right - Margin, 4);
            y += 20;

            graphics.DrawString(title, TitleFont, new XSolidBrush(Red), Margin, y);
            graphics.DrawString(storeName ?? string.Empty, BoldFont, new XSolidBrush(Charcoal),
                new XRect(Margin, y - 11, right - Margin, 14), XStringFormats.TopRight);
            y += 19;

            return page;
        }

        // Writes what the pages are and when they were printed along the bottom of each, with
        // "Page n of m" counted over just these pages.
        public static void NumberPages(IList<PdfPage> pages, string description)
        {
            string footerText = description + "   ·   Printed "
                + DateTime.Now.ToString("dd MMM yyyy HH:mm", CultureInfo.InvariantCulture);

            for (int index = 0; index < pages.Count; index++)
            {
                using (XGraphics footer = XGraphics.FromPdfPage(pages[index]))
                {
                    double width = pages[index].Width.Point;
                    double top = pages[index].Height.Point - Margin - 8;
                    var brush = new XSolidBrush(Muted);

                    footer.DrawString(footerText, SmallFont, brush, Margin, top);
                    footer.DrawString(string.Format(CultureInfo.InvariantCulture, "Page {0} of {1}", index + 1, pages.Count),
                        SmallFont, brush, new XRect(Margin, top - 8, width - Margin * 2, 10), XStringFormats.TopRight);
                }
            }
        }

        // Shortens text that does not fit its column, ending it with an ellipsis.
        public static string Fit(XGraphics graphics, string text, XFont font, double width)
        {
            if (text.Length == 0 || graphics.MeasureString(text, font).Width <= width)
            {
                return text;
            }

            for (int length = text.Length - 1; length > 0; length--)
            {
                string shorter = text.Substring(0, length).TrimEnd() + "…";
                if (graphics.MeasureString(shorter, font).Width <= width)
                {
                    return shorter;
                }
            }

            return string.Empty;
        }

        // A row of table cells across the given column widths. Columns listed in rightAligned
        // are numbers, lined up on the right so they add up by eye.
        public static void DrawCells(XGraphics graphics, string[] cells, double[] widths, ISet<int> rightAligned,
            XFont font, XBrush brush, double y, double height)
        {
            double x = Margin;

            for (int index = 0; index < cells.Length; index++)
            {
                var rect = new XRect(x + 4, y, widths[index] - 8, height);
                string text = Fit(graphics, cells[index], font, rect.Width);

                graphics.DrawString(text, font, brush, rect,
                    rightAligned.Contains(index) ? XStringFormats.CenterRight : XStringFormats.CenterLeft);
                x += widths[index];
            }
        }

        // A line to sign or write on, with what goes there written small underneath.
        public static void SignatureLine(XGraphics graphics, double x, double y, double width, string label)
        {
            graphics.DrawLine(new XPen(Charcoal, 0.75), x, y, x + width, y);
            graphics.DrawString(label, SmallFont, new XSolidBrush(Muted), x, y + 10);
        }

        // One label and value in a totals box.
        public static void TotalRow(XGraphics graphics, double x, double y, string label, string value, bool bold)
        {
            graphics.DrawString(label, BodyFont, new XSolidBrush(Charcoal), x, y);
            graphics.DrawString(value, bold ? BoldFont : BodyFont, XBrushes.Black, x + 160, y);
        }

        public static string LongDate(DateTime date)
        {
            return date.ToString("ddd d MMM yyyy", CultureInfo.InvariantCulture);
        }

        // Decimal hours, as on View Timesheets, with the clock reading beside them.
        public static string Hours(decimal hours)
        {
            return hours.ToString("0.00", CultureInfo.InvariantCulture) + "  (" + TimesheetSignOff.FormatHours(hours) + ")";
        }
    }
}
