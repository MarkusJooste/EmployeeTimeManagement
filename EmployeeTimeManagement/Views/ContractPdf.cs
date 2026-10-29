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

        // The PDF to fill in for a store: its own, or while the client has not supplied that
        // one, another store's PDF of the same type, with this store's name, hours and town
        // read from this store's other contract and written over the other store's. Null
        // when there is neither.
        public static ContractTemplateSource Find(int storeID, ContractTemplate template)
        {
            string own = PathFor(storeID, template);
            if (File.Exists(own))
            {
                return new ContractTemplateSource(storeID, template, storeID, null);
            }

            ContractTemplate other = template == ContractTemplate.General ? ContractTemplate.Manager : ContractTemplate.General;
            if (!File.Exists(PathFor(storeID, other)))
            {
                return null;
            }

            string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FolderName);
            if (!Directory.Exists(root))
            {
                return null;
            }

            foreach (int otherStore in Directory.GetDirectories(root)
                .Select(folder => Path.GetFileName(folder))
                .Select(name => { int id; return int.TryParse(name, out id) ? id : -1; })
                .Where(id => id > 0 && id != storeID)
                .OrderBy(id => id))
            {
                if (File.Exists(PathFor(otherStore, template)))
                {
                    return new ContractTemplateSource(storeID, template, otherStore, other);
                }
            }

            return null;
        }
    }

    // Which PDF a contract is filled in from, and where its store's wording comes from when
    // that PDF is another store's.
    internal class ContractTemplateSource
    {
        public ContractTemplateSource(int storeID, ContractTemplate template, int templateStoreID, ContractTemplate? wordingTemplate)
        {
            StoreID = storeID;
            Template = template;
            TemplateStoreID = templateStoreID;
            WordingTemplate = wordingTemplate;
        }

        public int StoreID { get; private set; }
        public ContractTemplate Template { get; private set; }
        public int TemplateStoreID { get; private set; }

        // This store's other contract, which its wording is read from; null when the PDF is
        // the store's own.
        public ContractTemplate? WordingTemplate { get; private set; }

        public bool IsBorrowed
        {
            get { return WordingTemplate.HasValue; }
        }

        public string TemplatePath
        {
            get { return ContractTemplateFiles.PathFor(TemplateStoreID, Template); }
        }

        public string WordingPath
        {
            get { return IsBorrowed ? ContractTemplateFiles.PathFor(StoreID, WordingTemplate.Value) : null; }
        }

        public string Describe()
        {
            string template = ContractTemplateFiles.DisplayPath(TemplateStoreID, Template);
            if (!IsBorrowed)
            {
                return template;
            }

            return template + " with this store's details from "
                + ContractTemplateFiles.DisplayPath(StoreID, WordingTemplate.Value)
                + " (" + ContractTemplateFiles.DisplayPath(StoreID, Template) + " not supplied yet)";
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

        public static ContractPdf Create(ContractTemplateSource source, ContractData data)
        {
            byte[] template = File.ReadAllBytes(source.TemplatePath);

            Dictionary<string, string> storeWording = null;
            if (source.IsBorrowed)
            {
                using (var own = PdfiumViewer.PdfDocument.Load(new MemoryStream(File.ReadAllBytes(source.WordingPath))))
                {
                    storeWording = ContractStoreWording.Read(new PdfiumTemplateText(own));
                }
            }

            ContractFillPlan plan;
            Dictionary<ContractFill, Color> coverColours;

            using (var measurer = new SharpTextMeasurer())
            using (var pdfium = PdfiumViewer.PdfDocument.Load(new MemoryStream(template)))
            {
                plan = ContractBlanks.Plan(data, new PdfiumTemplateText(pdfium), measurer, storeWording);
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

            // Windows' "A" width for the character: the gap between where it is drawn from
            // and where its ink starts. Measured on a 1000 pixel Arial and scaled.
            public double LeftBearing(char character, bool bold, double size)
            {
                const float EmPixels = 1000;

                using (var font = new Font("Arial", EmPixels, bold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel))
                using (Graphics screen = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hdc = screen.GetHdc();
                    IntPtr hfont = font.ToHfont();

                    try
                    {
                        IntPtr previous = NativeMethods.SelectObject(hdc, hfont);
                        var widths = new NativeMethods.ABCFLOAT[1];
                        bool measured = NativeMethods.GetCharABCWidthsFloat(hdc, character, character, widths);
                        NativeMethods.SelectObject(hdc, previous);

                        return measured ? widths[0].abcfA / EmPixels * size : 0;
                    }
                    finally
                    {
                        NativeMethods.DeleteObject(hfont);
                        screen.ReleaseHdc(hdc);
                    }
                }
            }

            public void Dispose()
            {
                graphics.Dispose();
            }
        }

        private static class NativeMethods
        {
            [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
            public struct ABCFLOAT
            {
                public float abcfA;
                public float abcfB;
                public float abcfC;
            }

            [System.Runtime.InteropServices.DllImport("gdi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            public static extern bool GetCharABCWidthsFloat(IntPtr hdc, uint first, uint last, [System.Runtime.InteropServices.Out] ABCFLOAT[] widths);

            [System.Runtime.InteropServices.DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr gdiObject);

            [System.Runtime.InteropServices.DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr gdiObject);
        }
    }
}
