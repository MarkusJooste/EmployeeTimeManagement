using EmployeeTimeManagement.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using SharpDocument = PdfSharp.Pdf.PdfDocument;

namespace EmployeeTimeManagement.Views
{
    // Where each store's contract PDFs live: ContractTemplates\<StoreID>\General.pdf and
    // ContractTemplates\<StoreID>\Manager.pdf next to the program.
    internal static class ContractTemplateFiles
    {
        public const string FolderName = "ContractTemplates";

        public static string PathFor(int storeID, ContractTemplate template)
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                FolderName,
                storeID.ToString(),
                template + ".pdf");
        }

        // The path as it is shown to the user, relative to the program's folder.
        public static string DisplayPath(int storeID, ContractTemplate template)
        {
            return Path.Combine(FolderName, storeID.ToString(), template + ".pdf");
        }
    }

    // A contract filled in from a store's template PDF. The template's pages are kept as
    // they are; only the blanks are written on, so it matches the client's PDF exactly.
    internal class ContractPdf
    {
        private ContractPdf(byte[] bytes, ContractFillPlan plan)
        {
            Bytes = bytes;
            Plan = plan;
        }

        public byte[] Bytes { get; private set; }
        public ContractFillPlan Plan { get; private set; }

        public static ContractPdf Create(string templatePath, ContractData data)
        {
            byte[] template = File.ReadAllBytes(templatePath);

            ContractFillPlan plan;
            Dictionary<ContractFill, Color> coverColours;

            using (var measurer = new SharpTextMeasurer())
            using (var pdfium = PdfiumViewer.PdfDocument.Load(new MemoryStream(template)))
            {
                plan = ContractBlanks.Plan(data, new PdfiumTemplateText(pdfium), measurer);
                coverColours = SampleCoverColours(pdfium, plan);
            }

            byte[] filled = Write(template, plan, coverColours, data);
            return new ContractPdf(filled, plan);
        }

        // Draws the plan onto the template with PDFsharp and returns the new file.
        private static byte[] Write(byte[] template, ContractFillPlan plan, Dictionary<ContractFill, Color> coverColours, ContractData data)
        {
            using (SharpDocument document = PdfReader.Open(new MemoryStream(template), PdfDocumentOpenMode.Modify))
            {
                document.Info.Title = "Employment contract - " + (data.FullName ?? "employee");

                foreach (IGrouping<int, ContractFill> page in plan.Fills.GroupBy(fill => fill.Page))
                {
                    using (XGraphics graphics = XGraphics.FromPdfPage(document.Pages[page.Key], XGraphicsPdfPageOptions.Append))
                    {
                        foreach (ContractFill fill in page)
                        {
                            if (fill.Cover.HasValue)
                            {
                                PdfBox cover = fill.Cover.Value;
                                Color colour = coverColours[fill];
                                graphics.DrawRectangle(
                                    new XSolidBrush(XColor.FromArgb(colour.R, colour.G, colour.B)),
                                    cover.Left, cover.Top, cover.Width, cover.Height);
                            }

                            foreach (ContractTextRun run in fill.Runs)
                            {
                                graphics.DrawString(run.Text, SharpTextMeasurer.Font(run.Bold, run.Size), XBrushes.Black, run.X, run.Baseline);
                            }
                        }
                    }
                }

                using (var output = new MemoryStream())
                {
                    document.Save(output, false);
                    return output.ToArray();
                }
            }
        }

        // The colour behind each blank that is painted over, so a blank on a grey band
        // (the signing lines) is covered in grey rather than white. Taken as the most
        // common colour inside the area on the rendered page; the dots are a small part
        // of it.
        private static Dictionary<ContractFill, Color> SampleCoverColours(PdfiumViewer.PdfDocument pdfium, ContractFillPlan plan)
        {
            const float Scale = 2f;
            var colours = new Dictionary<ContractFill, Color>();

            foreach (IGrouping<int, ContractFill> page in plan.Fills.Where(fill => fill.Cover.HasValue).GroupBy(fill => fill.Page))
            {
                SizeF size = pdfium.PageSizes[page.Key];
                int width = (int)Math.Ceiling(size.Width * Scale);
                int height = (int)Math.Ceiling(size.Height * Scale);

                using (var image = (Bitmap)pdfium.Render(page.Key, width, height, 72 * Scale, 72 * Scale, PdfRenderFlags.Annotations))
                {
                    foreach (ContractFill fill in page)
                    {
                        colours[fill] = MostCommonColour(image, fill.Cover.Value, Scale);
                    }
                }
            }

            return colours;
        }

        private static Color MostCommonColour(Bitmap image, PdfBox area, float scale)
        {
            int left = Clamp((int)(area.Left * scale), image.Width);
            int right = Clamp((int)Math.Ceiling(area.Right * scale), image.Width);
            int top = Clamp((int)(area.Top * scale), image.Height);
            int bottom = Clamp((int)Math.Ceiling(area.Bottom * scale), image.Height);

            var counts = new Dictionary<int, int>();
            var rectangle = new Rectangle(left, top, Math.Max(1, right - left), Math.Max(1, bottom - top));
            BitmapData bits = image.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                var row = new int[rectangle.Width];
                for (int y = 0; y < rectangle.Height; y++)
                {
                    System.Runtime.InteropServices.Marshal.Copy(bits.Scan0 + y * bits.Stride, row, 0, row.Length);
                    foreach (int pixel in row)
                    {
                        int rgb = pixel & 0xFFFFFF;
                        int count;
                        counts.TryGetValue(rgb, out count);
                        counts[rgb] = count + 1;
                    }
                }
            }
            finally
            {
                image.UnlockBits(bits);
            }

            if (counts.Count == 0)
            {
                return Color.White;
            }

            int common = counts.OrderByDescending(pair => pair.Value).First().Key;
            return Color.FromArgb(common >> 16 & 0xFF, common >> 8 & 0xFF, common & 0xFF);
        }

        private static int Clamp(int value, int limit)
        {
            return Math.Max(0, Math.Min(limit - 1, value));
        }

        // Reads a template's text through PDFium. Character boxes are slow to fetch, so
        // only the ones the blank finder asks for are read.
        private class PdfiumTemplateText : IContractTemplateText
        {
            private readonly PdfiumViewer.PdfDocument document;
            private readonly Dictionary<int, string> pageTexts = new Dictionary<int, string>();

            public PdfiumTemplateText(PdfiumViewer.PdfDocument document)
            {
                this.document = document;
            }

            public int PageCount
            {
                get { return document.PageCount; }
            }

            public double PageWidth(int page)
            {
                return document.PageSizes[page].Width;
            }

            public string PageText(int page)
            {
                string text;
                if (!pageTexts.TryGetValue(page, out text))
                {
                    text = document.GetPdfText(page);
                    pageTexts[page] = text;
                }

                return text;
            }

            public PdfBox? Bounds(int page, int start, int length)
            {
                if (length <= 0)
                {
                    return null;
                }

                double pageHeight = document.PageSizes[page].Height;
                PdfBox? union = null;

                // PDFium measures up from the bottom of the page with a negative height;
                // turn that into a box measured down from the top.
                foreach (PdfRectangle rectangle in document.GetTextBounds(new PdfTextSpan(page, start, length)))
                {
                    RectangleF bounds = rectangle.Bounds;
                    double top = pageHeight - Math.Max(bounds.Top, bounds.Bottom);
                    double bottom = pageHeight - Math.Min(bounds.Top, bounds.Bottom);
                    var box = new PdfBox(bounds.Left, top, bounds.Right, bottom);

                    union = union.HasValue ? union.Value.Union(box) : box;
                }

                return union;
            }
        }

        // Measures text in the fonts it will be drawn in.
        private class SharpTextMeasurer : IContractTextMeasurer, IDisposable
        {
            private static readonly Dictionary<string, XFont> Fonts = new Dictionary<string, XFont>();

            private readonly XGraphics graphics = XGraphics.CreateMeasureContext(
                new XSize(612, 792), XGraphicsUnit.Point, XPageDirection.Downwards);

            // The contracts are set in Arial.
            public static XFont Font(bool bold, double size)
            {
                string key = (bold ? "b" : "r") + size.ToString(System.Globalization.CultureInfo.InvariantCulture);
                XFont font;

                lock (Fonts)
                {
                    if (!Fonts.TryGetValue(key, out font))
                    {
                        font = new XFont("Arial", size, bold ? XFontStyleEx.Bold : XFontStyleEx.Regular);
                        Fonts[key] = font;
                    }
                }

                return font;
            }

            public double Width(string text, bool bold, double size)
            {
                return graphics.MeasureString(text, Font(bold, size)).Width;
            }

            public void Dispose()
            {
                graphics.Dispose();
            }
        }
    }
}
