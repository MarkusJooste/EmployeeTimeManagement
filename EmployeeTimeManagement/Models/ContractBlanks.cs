using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EmployeeTimeManagement.Models
{
    // A rectangle on a PDF page, in points, measured from the page's top-left corner.
    internal struct PdfBox
    {
        public PdfBox(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public double Left { get; private set; }
        public double Top { get; private set; }
        public double Right { get; private set; }
        public double Bottom { get; private set; }

        public double Width
        {
            get { return Right - Left; }
        }

        public double Height
        {
            get { return Bottom - Top; }
        }

        public PdfBox Union(PdfBox other)
        {
            return new PdfBox(
                Math.Min(Left, other.Left),
                Math.Min(Top, other.Top),
                Math.Max(Right, other.Right),
                Math.Max(Bottom, other.Bottom));
        }
    }

    // The text of a contract template PDF and where its characters sit on the page.
    internal interface IContractTemplateText
    {
        int PageCount { get; }

        double PageWidth(int page);

        // The page's text, with "\r\n" between lines.
        string PageText(int page);

        // The box around characters start to start + length - 1 of PageText(page), or null
        // when none of them has a box.
        PdfBox? Bounds(int page, int start, int length);
    }

    // Measures text as it will be drawn into the PDF.
    internal interface IContractTextMeasurer
    {
        double Width(string text, bool bold, double size);

        // How far the character's ink starts to the right of where it is drawn from. A
        // character's box in the template starts at its ink, so this finds where it was drawn.
        double LeftBearing(char character, bool bold, double size);
    }

    // One piece of text to draw. X and Baseline are in points from the top-left corner.
    internal class ContractTextRun
    {
        public ContractTextRun(double x, double baseline, string text, bool bold, double size)
        {
            X = x;
            Baseline = baseline;
            Text = text;
            Bold = bold;
            Size = size;
        }

        public double X { get; private set; }
        public double Baseline { get; private set; }
        public string Text { get; private set; }
        public bool Bold { get; private set; }
        public double Size { get; private set; }
    }

    // What to do to one blank: paint over the template's dots (when Cover is set) and
    // draw the value.
    internal class ContractFill
    {
        public ContractFill(string field, int page, PdfBox? cover, List<ContractTextRun> runs)
        {
            Field = field;
            Page = page;
            Cover = cover;
            Runs = runs;
        }

        public string Field { get; private set; }
        public int Page { get; private set; }
        public PdfBox? Cover { get; private set; }
        public List<ContractTextRun> Runs { get; private set; }
    }

    internal class ContractFillPlan
    {
        public ContractFillPlan()
        {
            Fills = new List<ContractFill>();
            NotFound = new List<string>();
        }

        public List<ContractFill> Fills { get; private set; }

        // Blanks the template should have but that could not be found in it, so they were
        // not filled. Usually means the PDF is a different version of the contract.
        public List<string> NotFound { get; private set; }
    }

    // How values are written into the contract.
    internal static class ContractFormat
    {
        public const decimal ManagerMonthlyPaidHours = 210m;

        private static readonly NumberFormatInfo RandFormat = new NumberFormatInfo
        {
            NumberDecimalSeparator = ".",
            NumberGroupSeparator = " ",
            NumberGroupSizes = new[] { 3 }
        };

        public static string Amount(decimal value)
        {
            return value.ToString("#,0.00", RandFormat);
        }

        public static string Rand(decimal? value)
        {
            if (!value.HasValue)
            {
                return null;
            }

            return "R" + Amount(value.Value);
        }

        // The contract prints the monthly salary without cents.
        public static string RandDroppingZeroCents(decimal value)
        {
            if (value == decimal.Truncate(value))
            {
                return "R" + value.ToString("#,0", RandFormat);
            }

            return "R" + Amount(value);
        }

        public static decimal MonthlySalary(decimal hourlyRate)
        {
            return decimal.Round(hourlyRate * ManagerMonthlyPaidHours, 2, MidpointRounding.AwayFromZero);
        }

        // "1 March 2026", or null when there is no date.
        public static string LongDate(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
        }

        public static string MonthName(DateTime date)
        {
            return date.ToString("MMMM", CultureInfo.InvariantCulture);
        }

        // 1 -> "1st", 22 -> "22nd", 13 -> "13th".
        public static string Ordinal(int day)
        {
            int lastTwo = day % 100;
            if (lastTwo >= 11 && lastTwo <= 13)
            {
                return day + "th";
            }

            switch (day % 10)
            {
                case 1:
                    return day + "st";
                case 2:
                    return day + "nd";
                case 3:
                    return day + "rd";
                default:
                    return day + "th";
            }
        }
    }

    // Finds the blanks in the client's contract PDFs and works out what to write in each.
    // Blanks are found by the wording around them rather than by fixed positions, so a
    // store's own copy of a contract still works when its pages break differently.
    internal static class ContractBlanks
    {
        public const string NameField = "Name";
        public const string IDNumberField = "ID number";
        public const string AddressField = "Address";
        public const string TelephoneField = "Tel. no";
        public const string NextOfKinField = "Next of kin";
        public const string RelationshipField = "Relationship";
        public const string ContactNumberField = "Contact no.";
        public const string CommencementField = "Commencement date (2.1)";
        public const string PreviousEmployerField = "Start date with previous employer (2.1)";
        public const string PositionField = "Position (3.1)";
        public const string HourlyRateField = "Hourly rate (6.1.1)";
        public const string SalaryField = "Monthly salary and hourly rate (6.1.1)";
        public const string SigningDateField = "Signing date";

        // The labels on page 1 sit in one column and their lines start one inch (a Word
        // tab stop) to the right of it, running 4 inches.
        private const double LabelToLine = 72;
        private const double LabelLineLength = 288;

        // The contracts are set in 11 point Arial. Values are shrunk to fit a blank, but
        // never below SmallestSize.
        private const double FontSize = 11;
        private const double SmallestSize = 7;

        // Word's justified lines end 51 points from the right of the page; ragged lines stop
        // short of that. A line ending within 2 points of it was justified.
        private const double JustifiedRightMargin = 53;

        // Where Word's justified lines end, from the right of the page: the furthest a line
        // carrying another store's longer wording may run before words move to the next line.
        private const double TextRightMargin = 48;

        // Characters that sit on the baseline, used to find a line's baseline.
        private const string BaselineCharacters = "ABCDEFGHIKLMNORSTUVWXZ0123456789abcdefhiklmnorstuvwxz.…";

        private const string Dots = "…[….]*";

        // A "bold" group marks text the template prints in bold, so it stays bold if the
        // line is rewritten.
        private static readonly Regex GeneralCommencement = new Regex(
            @"is/was\s*(?<date>" + Dots + @")(?<bold>\s*(?<century>20)(?<year>…+))");

        private static readonly Regex ManagerCommencement = new Regex(
            @"is/was the(?<day> +)day of(?<month> +)(?<century>20)(?<year> +)\.");

        private static readonly Regex Position = new Regex(
            @"position of\s*(?<position>" + Dots + ")");

        private static readonly Regex GeneralRate = new Regex(
            @"remuneration of\s*(?<bold>R(?<rate>\.+)\s*hour)");

        private static readonly Regex ManagerSalary = new Regex(
            @"remuneration of\s*(?<monthly>R\s?\d+(?:[  ]\d{3})*(?:[.,]\d+)?)\s*per month\s*\(R(?<hourly>\d+(?:[.,]\d+)?)\s*per\s+hour");

        private static readonly Regex Signing = new Regex(
            @"(?<bold>SIGNED IN (?<town>.*?) ON THIS\s*(?<day>" + Dots + @")\s*DAY OF\s*(?<month>…[….]*?)(?<decade>202)(?<year>\.+))");

        // The blanks that will be left for handwriting because there is no data for them.
        public static List<string> MissingFields(ContractData data)
        {
            var missing = new List<string>();

            AddIfBlank(missing, NameField, data.FullName);
            AddIfBlank(missing, IDNumberField, data.IDNumber);

            if (IsBlank(data.AddressLine1) && IsBlank(data.AddressLine2))
            {
                missing.Add(AddressField);
            }

            AddIfBlank(missing, TelephoneField, data.TelephoneNumber);
            AddIfBlank(missing, NextOfKinField, data.NextOfKinName);
            AddIfBlank(missing, RelationshipField, data.NextOfKinRelationship);
            AddIfBlank(missing, ContactNumberField, data.NextOfKinNumber);

            if (!data.CommencementDate.HasValue)
            {
                missing.Add(CommencementField);
            }

            if (data.Template == ContractTemplate.Manager)
            {
                // Not stored anywhere, so always written in by hand.
                missing.Add(PreviousEmployerField);

                if (!data.HourlyRate.HasValue)
                {
                    missing.Add(SalaryField);
                }
            }
            else
            {
                AddIfBlank(missing, PositionField, data.Position);

                if (!data.HourlyRate.HasValue)
                {
                    missing.Add(HourlyRateField);
                }
            }

            return missing;
        }

        // Works out every change to make to the template for this employee. storeWording is
        // the employee's store's own wording (ContractStoreWording.Read) when the template is
        // another store's PDF; where it differs it is written over the template's.
        public static ContractFillPlan Plan(ContractData data, IContractTemplateText template, IContractTextMeasurer measurer,
            IDictionary<string, string> storeWording = null)
        {
            return new Planner(data, template, measurer, storeWording).Run();
        }

        private static bool IsBlank(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        private static void AddIfBlank(List<string> missing, string field, string value)
        {
            if (IsBlank(value))
            {
                missing.Add(field);
            }
        }

        // How a blank is drawn in the template.
        private enum BlankStyle
        {
            // A row of dots, painted over and written on from the left.
            Dotted,

            // Underlined spaces, written on centred and left as they are.
            Underlined
        }

        private class Planner
        {
            private readonly ContractData data;
            private readonly IContractTemplateText template;
            private readonly IContractTextMeasurer measurer;
            private readonly ContractFillPlan plan = new ContractFillPlan();
            private readonly IDictionary<string, string> storeWording;

            public Planner(ContractData data, IContractTemplateText template, IContractTextMeasurer measurer,
                IDictionary<string, string> storeWording)
            {
                this.data = data;
                this.template = template;
                this.measurer = measurer;
                this.storeWording = storeWording ?? new Dictionary<string, string>();
            }

            public ContractFillPlan Run()
            {
                FillPageOne();

                if (data.Template == ContractTemplate.Manager)
                {
                    FillManagerCommencement();
                    FillManagerSalary();
                }
                else
                {
                    FillGeneralCommencement();
                    FillPosition();
                    FillGeneralRate();
                }

                FillSigningLines();
                SwapStoreWording();
                return plan;
            }

            // Page 1: the employee's details on the lines beside each label

            private void FillPageOne()
            {
                FillLabelLine(NameField, @"Name:", data.FullName);
                FillLabelLine(IDNumberField, @"ID Number:", data.IDNumber);

                double addressX;
                double addressBaseline;
                bool hasAddress = FindLabel(@"Address:", out addressX, out addressBaseline);

                double telephoneX;
                double telephoneBaseline;
                bool hasTelephone = FindLabel(@"Tel\. no:", out telephoneX, out telephoneBaseline);

                if (hasAddress)
                {
                    AddLabelValue(AddressField, addressX, addressBaseline, data.AddressLine1);

                    // The second address line has no label of its own; it sits halfway
                    // between the Address and Tel. no lines.
                    if (hasTelephone)
                    {
                        AddLabelValue(AddressField, addressX, (addressBaseline + telephoneBaseline) / 2, data.AddressLine2);
                    }
                }
                else
                {
                    plan.NotFound.Add(AddressField);
                }

                if (hasTelephone)
                {
                    AddLabelValue(TelephoneField, telephoneX, telephoneBaseline, data.TelephoneNumber);
                }
                else
                {
                    plan.NotFound.Add(TelephoneField);
                }

                FillLabelLine(NextOfKinField, @"Next of kin:", data.NextOfKinName);
                FillLabelLine(RelationshipField, @"Relationship:", data.NextOfKinRelationship);
                FillLabelLine(ContactNumberField, @"Contact No\.:", data.NextOfKinNumber);
            }

            private void FillLabelLine(string field, string labelPattern, string value)
            {
                double x;
                double baseline;
                if (!FindLabel(labelPattern, out x, out baseline))
                {
                    plan.NotFound.Add(field);
                    return;
                }

                AddLabelValue(field, x, baseline, value);
            }

            private void AddLabelValue(string field, double x, double baseline, string value)
            {
                if (IsBlank(value))
                {
                    return;
                }

                AddText(field, 0, null, x, baseline, value.Trim(), false, LabelLineLength, false);
            }

            // Finds a label on page 1 and returns where its line starts and its baseline.
            private bool FindLabel(string pattern, out double lineX, out double baseline)
            {
                lineX = 0;
                baseline = 0;

                if (template.PageCount == 0)
                {
                    return false;
                }

                Match match = new Regex(pattern).Match(template.PageText(0));
                if (!match.Success)
                {
                    return false;
                }

                PdfBox? label = template.Bounds(0, match.Index, 1);
                if (!label.HasValue)
                {
                    return false;
                }

                lineX = label.Value.Left + LabelToLine;
                baseline = Baseline(0, match.Index, match.Length);
                return true;
            }

            // 2.1 Commencement date

            // "The commencement date of employment is/was 1 March 2026."
            private void FillGeneralCommencement()
            {
                int page;
                Match match = FindFirst(GeneralCommencement, out page);
                if (match == null)
                {
                    plan.NotFound.Add(CommencementField);
                    return;
                }

                if (!data.CommencementDate.HasValue)
                {
                    return;
                }

                DateTime date = data.CommencementDate.Value;

                FillLine(CommencementField, page, match, BlankStyle.Dotted,
                    new Replacement(match.Groups["date"], date.Day + " " + ContractFormat.MonthName(date), false),
                    YearReplacement(match.Groups["century"], match.Groups["year"], date.Year, true));
            }

            // "is/was the 1st day of March 2026."
            private void FillManagerCommencement()
            {
                int page;
                Match match = FindFirst(ManagerCommencement, out page);
                if (match == null)
                {
                    plan.NotFound.Add(CommencementField);
                    return;
                }

                if (!data.CommencementDate.HasValue)
                {
                    return;
                }

                DateTime date = data.CommencementDate.Value;

                FillLine(CommencementField, page, match, BlankStyle.Underlined,
                    new Replacement(match.Groups["day"], ContractFormat.Ordinal(date.Day), false),
                    new Replacement(match.Groups["month"], ContractFormat.MonthName(date), false),
                    YearReplacement(match.Groups["century"], match.Groups["year"], date.Year, false));
            }

            // 3.1 Position and 6.1.1 Remuneration

            private void FillPosition()
            {
                FillSimpleBlank(PositionField, Position, "position", data.Position, false);
            }

            private void FillGeneralRate()
            {
                string value = data.HourlyRate.HasValue
                    ? ContractFormat.Amount(data.HourlyRate.Value) + " per"
                    : null;

                FillSimpleBlank(HourlyRateField, GeneralRate, "rate", value, true);
            }

            private void FillSimpleBlank(string field, Regex pattern, string group, string value, bool bold)
            {
                int page;
                Match match = FindFirst(pattern, out page);
                if (match == null)
                {
                    plan.NotFound.Add(field);
                    return;
                }

                if (IsBlank(value))
                {
                    return;
                }

                FillLine(field, page, match, BlankStyle.Dotted, new Replacement(match.Groups[group], value.Trim(), bold));
            }

            private void FillManagerSalary()
            {
                int page;
                Match match = FindFirst(ManagerSalary, out page);
                if (match == null)
                {
                    plan.NotFound.Add(SalaryField);
                    return;
                }

                string monthly;
                string hourly;
                if (data.HourlyRate.HasValue)
                {
                    monthly = ContractFormat.RandDroppingZeroCents(ContractFormat.MonthlySalary(data.HourlyRate.Value));
                    hourly = ContractFormat.Amount(data.HourlyRate.Value);
                }
                else
                {
                    monthly = "R……………";
                    hourly = "…………";
                }

                Group monthlyGroup = match.Groups["monthly"];
                Group hourlyGroup = match.Groups["hourly"];
                string text = template.PageText(page);

                // The figures may land on two lines in another store's copy, so each line
                // is rewritten separately.
                int monthlyLineEnd = LineEnd(text, monthlyGroup.Index);
                var firstLine = new List<Replacement> { new Replacement(monthlyGroup, monthly, true) };

                // Only the monthly figure is bold in the template, and it is replaced.
                var noBold = new List<Group>();

                if (hourlyGroup.Index < monthlyLineEnd)
                {
                    firstLine.Add(new Replacement(hourlyGroup, hourly, false));
                    RewriteToLineEnd(SalaryField, page, monthlyGroup.Index, firstLine, noBold);
                }
                else
                {
                    RewriteToLineEnd(SalaryField, page, monthlyGroup.Index, firstLine, noBold);
                    RewriteToLineEnd(SalaryField, page, hourlyGroup.Index,
                        new List<Replacement> { new Replacement(hourlyGroup, hourly, false) }, noBold);
                }
            }


            // "SIGNED IN ON THIS 23rd DAY OF October 2026"


            private void FillSigningLines()
            {
                DateTime date = data.SigningDate;
                bool found = false;

                for (int page = 0; page < template.PageCount; page++)
                {
                    foreach (Match match in Signing.Matches(template.PageText(page)))
                    {
                        found = true;

                        // The month's dots run straight into the printed "202", hence the space.
                        var replacements = new List<Replacement>
                        {
                            new Replacement(match.Groups["day"], ContractFormat.Ordinal(date.Day), true),
                            new Replacement(match.Groups["month"], ContractFormat.MonthName(date) + " ", true),
                            YearReplacement(match.Groups["decade"], match.Groups["year"], date.Year, true)
                        };

                        // Another store's PDF is signed in that store's town.
                        string town;
                        if (storeWording.TryGetValue(ContractStoreWording.TownField, out town)
                            && town != ContractStoreWording.Tidy(match.Groups["town"].Value))
                        {
                            replacements.Insert(0, new Replacement(match.Groups["town"], town, true));
                        }

                        FillLine(SigningDateField, page, match, BlankStyle.Dotted, replacements.ToArray());
                    }
                }

                if (!found)
                {
                    plan.NotFound.Add(SigningDateField);
                }
            }

            // Another store's PDF: this store's name, workplace and hours written over that
            // store's. The signing town is changed with the signing lines above.

            private void SwapStoreWording()
            {
                foreach (KeyValuePair<string, string> wording in storeWording)
                {
                    if (wording.Key == ContractStoreWording.TownField)
                    {
                        continue;
                    }

                    int page;
                    Match match = FindFirst(ContractStoreWording.PatternFor(wording.Key), out page);
                    if (match == null)
                    {
                        plan.NotFound.Add(wording.Key);
                        continue;
                    }

                    Group old = match.Groups["value"];
                    if (ContractStoreWording.Tidy(old.Value) != wording.Value)
                    {
                        ReplaceWrapped(wording.Key, page, old.Index, old.Length, wording.Value);
                    }
                }
            }

            // Writes new wording over old, which may run onto the next line. The lines are
            // wrapped again from there as Word would: words that no longer fit move to the start
            // of the next line, and a justified line left short takes words up from the next,
            // for as many lines as it takes. A line with nowhere to pass words on is set smaller
            // instead. Lines Word justified stay justified.
            private void ReplaceWrapped(string field, int page, int start, int length, string newText)
            {
                string text = template.PageText(page);
                int oldEnd = LineEnd(text, start + length - 1);

                // The words still to set: the new wording, then whatever followed the old on
                // its last line.
                string carry = newText + text.Substring(start + length, oldEnd - (start + length));
                int lineStart = start;

                // Whether carry already holds the next line's words.
                bool hasNextLine = false;

                while (true)
                {
                    int lineEnd = LineEnd(text, lineStart);

                    // A later line still holds some of the old wording, so it is rewritten
                    // whatever happens to this one.
                    bool oldContinues = lineEnd < oldEnd;

                    int nextStart = NextLineStart(text, lineEnd);
                    bool canCarry = oldContinues || nextStart >= 0;

                    // A line Word wrapped fills up again with words from the next line, which
                    // is the rest of its paragraph.
                    if (!oldContinues && !hasNextLine && nextStart >= 0 && WrapsInto(page, lineStart, lineEnd, nextStart))
                    {
                        carry += " " + text.Substring(nextStart, LineEnd(text, nextStart) - nextStart);
                        hasNextLine = true;
                    }

                    List<string> words = carry.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    int fit;
                    if (!SetWrappedLine(field, page, lineStart, lineEnd, words, canCarry, out fit))
                    {
                        return;
                    }

                    List<string> rest = words.Skip(fit).ToList();

                    if (oldContinues || hasNextLine)
                    {
                        // The next line's words are in rest, or were all taken up; either
                        // way it is set again.
                        carry = string.Join(" ", rest);
                        lineStart = nextStart;
                        hasNextLine = false;
                        continue;
                    }

                    if (rest.Count == 0)
                    {
                        return;
                    }

                    int nextEnd = LineEnd(text, nextStart);
                    carry = string.Join(" ", rest) + " " + text.Substring(nextStart, nextEnd - nextStart);
                    lineStart = nextStart;
                }
            }

            // Sets as many of the words as fit on the line from lineStart, painting over what
            // was there. When the line cannot pass words on, all of them are set, smaller if
            // need be. Returns false, noting the field, if the line cannot be found.
            private bool SetWrappedLine(string field, int page, int lineStart, int lineEnd, List<string> words, bool canCarry, out int fit)
            {
                string text = template.PageText(page);
                int end = lineEnd;
                while (end > lineStart && char.IsWhiteSpace(text[end - 1]))
                {
                    end--;
                }

                fit = 0;
                PdfBox? found = template.Bounds(page, lineStart, end - lineStart);
                if (!found.HasValue)
                {
                    plan.NotFound.Add(field);
                    return false;
                }

                PdfBox line = found.Value;
                double baseline = Baseline(page, lineStart, end - lineStart);
                bool justified = IsJustified(page, lineStart);
                double rightEdge = template.PageWidth(page) - TextRightMargin;

                // A justified line's spaces may close up a little to take one more word, as
                // the spaces before the new wording are already spread and cannot be.
                double spaceWidth = measurer.Width(" ", false, FontSize) * (justified ? 0.8 : 1);

                double used = 0;
                while (fit < words.Count)
                {
                    double width = measurer.Width(words[fit], false, FontSize) + (fit > 0 ? spaceWidth : 0);
                    if (line.Left + used + width > rightEdge)
                    {
                        break;
                    }

                    used += width;
                    fit++;
                }

                if (!canCarry || (fit == 0 && words.Count > 0))
                {
                    fit = words.Count;
                }

                List<string> set = words.Take(fit).ToList();

                double size = FontSize;
                double wordsWidth = set.Sum(word => measurer.Width(word, false, size));
                while (size > SmallestSize
                    && line.Left + wordsWidth + measurer.Width(" ", false, size) * Math.Max(0, set.Count - 1) > rightEdge)
                {
                    size -= 0.5;
                    wordsWidth = set.Sum(word => measurer.Width(word, false, size));
                }

                double gap = measurer.Width(" ", false, size);
                if (justified && set.Count > 1)
                {
                    gap = Math.Max(gap * 0.5, (line.Right - line.Left - wordsWidth) / (set.Count - 1));
                }

                // Drawn from where the old first character was drawn from, so the new line
                // starts level with the lines above and below it.
                double origin = line.Left - measurer.LeftBearing(text[lineStart], false, FontSize);

                var runs = new List<ContractTextRun>();
                double x = origin;
                foreach (string word in set)
                {
                    runs.Add(new ContractTextRun(x, baseline, word, false, size));
                    x += measurer.Width(word, false, size) + gap;
                }

                PdfBox cover = CoverFor(line, baseline);
                double newRight = x - gap;
                cover = new PdfBox(cover.Left, cover.Top, Math.Max(cover.Right, newRight + 0.5), cover.Bottom);

                plan.Fills.Add(new ContractFill(field, page, cover, runs));
                return true;
            }

            // Drawing values into blanks

            // Years are printed partly in the template ("20……" or "202……"). Only the
            // missing digits are written, unless the year does not start with the printed
            // ones, in which case the printed digits are replaced too.
            private static Replacement YearReplacement(Group printed, Group blank, int year, bool bold)
            {
                string full = year.ToString(CultureInfo.InvariantCulture);

                if (full.StartsWith(printed.Value, StringComparison.Ordinal))
                {
                    return new Replacement(blank.Index, blank.Length, full.Substring(printed.Value.Length), bold);
                }

                return new Replacement(printed.Index, blank.Index + blank.Length - printed.Index, full, bold);
            }

            // Writes the values into a sentence's blanks. On a line Word left ragged the rest
            // of the line is set again, so it reads as if the values had been typed in. A
            // justified line would need words pulled up from the next line, which would move
            // the page breaks, so values are written over their own dots, and only the words
            // from the last blank on are spread to the margin again.
            private void FillLine(string field, int page, Match match, BlankStyle style, params Replacement[] replacements)
            {
                string text = template.PageText(page);
                var boldGroups = new List<Group> { match.Groups["bold"] };

                foreach (IGrouping<int, Replacement> line in replacements.GroupBy(r => LineEnd(text, r.Index)))
                {
                    List<Replacement> ordered = line.OrderBy(r => r.Index).ToList();

                    if (style == BlankStyle.Underlined)
                    {
                        foreach (Replacement replacement in ordered)
                        {
                            FillUnderline(field, page, replacement);
                        }

                        continue;
                    }

                    if (!IsJustified(page, ordered[0].Index))
                    {
                        RewriteToLineEnd(field, page, ordered[0].Index, ordered, boldGroups);
                        continue;
                    }

                    Replacement last = ordered[ordered.Count - 1];
                    for (int index = 0; index < ordered.Count - 1; index++)
                    {
                        FillDots(field, page, ordered[index]);
                    }

                    // Start from the beginning of the last blank's word, so the "20" printed
                    // before a year moves with it.
                    int wordStart = last.Index;
                    while (wordStart > 0 && !char.IsWhiteSpace(text[wordStart - 1]))
                    {
                        wordStart--;
                    }

                    RewriteToLineEnd(field, page, wordStart, new List<Replacement> { last }, boldGroups);
                }
            }

            // Paints over a row of dots and writes the value from where they started.
            private void FillDots(string field, int page, Replacement replacement)
            {
                PdfBox? found = template.Bounds(page, replacement.Index, replacement.Length);
                if (!found.HasValue)
                {
                    plan.NotFound.Add(field);
                    return;
                }

                PdfBox box = found.Value;
                double baseline = LineBaseline(page, replacement.Index);
                AddText(field, page, CoverFor(box, baseline), box.Left, baseline, replacement.Text, replacement.Bold, box.Width, false);
            }

            // Writes a value centred on an underlined blank, which runs from the end of the
            // word before it to the start of the word after it. (The underlined spaces are
            // spread out by Word, so their own boxes are narrower than the line.)
            private void FillUnderline(string field, int page, Replacement replacement)
            {
                PdfBox? before = template.Bounds(page, replacement.Index - 1, 1);
                PdfBox? after = template.Bounds(page, replacement.Index + replacement.Length, 1);
                if (!before.HasValue || !after.HasValue || after.Value.Left <= before.Value.Right)
                {
                    plan.NotFound.Add(field);
                    return;
                }

                // The rest of a year carries on straight after the printed "20".
                bool continuesNumber = char.IsDigit(template.PageText(page)[replacement.Index - 1]);

                double left = before.Value.Right + (continuesNumber ? 0.6 : 1);
                double width = after.Value.Left - 1 - left;
                double baseline = LineBaseline(page, replacement.Index);
                AddText(field, page, null, left, baseline, replacement.Text, replacement.Bold, width, !continuesNumber);
            }

            private void AddText(string field, int page, PdfBox? cover, double x, double baseline, string text,
                bool bold, double maxWidth, bool centre)
            {
                double size = FontSize;
                double width = measurer.Width(text, bold, size);

                while (width > maxWidth && size > SmallestSize)
                {
                    size -= 0.5;
                    width = measurer.Width(text, bold, size);
                }

                if (centre && width < maxWidth)
                {
                    x += (maxWidth - width) / 2;
                }

                var runs = new List<ContractTextRun> { new ContractTextRun(x, baseline, text, bold, size) };
                plan.Fills.Add(new ContractFill(field, page, cover, runs));
            }

            // Paints over the rest of the line from start, then writes it again with the
            // replacements made. Text inside boldGroups stays bold. Justified lines keep their
            // right edge, as Word set them.
            private void RewriteToLineEnd(string field, int page, int start, List<Replacement> replacements, List<Group> boldGroups)
            {
                string text = template.PageText(page);
                int end = LineEnd(text, start);

                while (end > start && char.IsWhiteSpace(text[end - 1]))
                {
                    end--;
                }

                PdfBox? found = template.Bounds(page, start, end - start);
                if (!found.HasValue)
                {
                    plan.NotFound.Add(field);
                    return;
                }

                PdfBox line = found.Value;
                double baseline = Baseline(page, start, end - start);

                List<Word> words = SplitWords(text, start, end, replacements, boldGroups);
                if (words.Count == 0)
                {
                    return;
                }

                // A justified line has its new words spread to the same right edge.
                bool justified = IsJustified(page, start);

                double size = FontSize;
                double wordsWidth;
                double spaceWidth;
                double gap;

                while (true)
                {
                    wordsWidth = 0;
                    foreach (Word word in words)
                    {
                        wordsWidth += word.Width(measurer, size);
                    }

                    spaceWidth = measurer.Width(" ", false, size);
                    int gaps = words.Count - 1;
                    gap = spaceWidth;

                    if (justified && gaps > 0)
                    {
                        gap = (line.Width - wordsWidth) / gaps;
                    }

                    bool fits = justified
                        ? gap >= spaceWidth * 0.5
                        : wordsWidth + spaceWidth * gaps <= line.Width + spaceWidth * 4;

                    if (fits || size <= SmallestSize)
                    {
                        break;
                    }

                    size -= 0.5;
                }

                var runs = new List<ContractTextRun>();
                double x = line.Left;

                foreach (Word word in words)
                {
                    foreach (Word.Part part in word.Parts)
                    {
                        runs.Add(new ContractTextRun(x, baseline, part.Text, part.Bold, size));
                        x += measurer.Width(part.Text, part.Bold, size);
                    }

                    x += gap;
                }

                PdfBox cover = CoverFor(line, baseline);
                if (!justified)
                {
                    // The new text may run past the old, so cover to where it ends.
                    double newRight = line.Left + wordsWidth + gap * (words.Count - 1);
                    cover = new PdfBox(cover.Left, cover.Top, Math.Max(cover.Right, newRight + 0.5), cover.Bottom);
                }

                plan.Fills.Add(new ContractFill(field, page, cover, runs));
            }

            // The words of text[start..end) with the replacements made. A replaced value
            // takes the replacement's weight; the rest is bold inside boldGroups. The spaces
            // inside a value are kept as they are, so a justified line is spread between the
            // template's words rather than inside "Assistant Manager".
            private static List<Word> SplitWords(string text, int start, int end, List<Replacement> replacements, List<Group> boldGroups)
            {
                var characters = new StringBuilder();
                var bold = new List<bool>();
                var breakable = new List<bool>();
                int index = start;

                while (index < end)
                {
                    Replacement replacement = replacements.Find(r => r.Index == index);
                    if (replacement != null)
                    {
                        string value = replacement.Text.TrimEnd();
                        characters.Append(value);
                        for (int count = 0; count < value.Length; count++)
                        {
                            bold.Add(replacement.Bold);
                            breakable.Add(false);
                        }

                        // A value's trailing space still separates it from what follows.
                        if (value.Length < replacement.Text.Length)
                        {
                            characters.Append(' ');
                            bold.Add(false);
                            breakable.Add(true);
                        }

                        index += replacement.Length;
                        continue;
                    }

                    int templateIndex = index;
                    characters.Append(text[index]);
                    bold.Add(boldGroups.Exists(group => group.Success && templateIndex >= group.Index && templateIndex < group.Index + group.Length));
                    breakable.Add(true);
                    index++;
                }

                var words = new List<Word>();
                Word current = null;

                for (int position = 0; position < characters.Length; position++)
                {
                    if (char.IsWhiteSpace(characters[position]) && breakable[position])
                    {
                        current = null;
                        continue;
                    }

                    if (current == null)
                    {
                        current = new Word();
                        words.Add(current);
                    }

                    current.Append(characters[position], bold[position]);
                }

                return words;
            }

            // Finding things in the template

            private Match FindFirst(Regex pattern, out int page)
            {
                for (page = 0; page < template.PageCount; page++)
                {
                    Match match = pattern.Match(template.PageText(page));
                    if (match.Success)
                    {
                        return match;
                    }
                }

                page = -1;
                return null;
            }

            // The baseline of the line holding text[start..start+length): the bottom of
            // its first character that sits on the baseline.
            private double Baseline(int page, int start, int length)
            {
                string text = template.PageText(page);

                for (int index = start; index < start + length && index < text.Length; index++)
                {
                    if (BaselineCharacters.IndexOf(text[index]) < 0)
                    {
                        continue;
                    }

                    PdfBox? box = template.Bounds(page, index, 1);
                    if (box.HasValue)
                    {
                        return box.Value.Bottom;
                    }
                }

                PdfBox? whole = template.Bounds(page, start, length);
                return whole.HasValue ? whole.Value.Bottom : 0;
            }

            private double LineBaseline(int page, int index)
            {
                string text = template.PageText(page);
                int start = LineStart(text, index);
                return Baseline(page, start, LineEnd(text, index) - start);
            }

            // Whether Word justified the line holding text[index]: its last character
            // reaches the right margin.
            private bool IsJustified(int page, int index)
            {
                string text = template.PageText(page);
                int last = LineEnd(text, index) - 1;

                while (last > index && char.IsWhiteSpace(text[last]))
                {
                    last--;
                }

                PdfBox? box = template.Bounds(page, last, 1);
                return box.HasValue && box.Value.Right >= template.PageWidth(page) - JustifiedRightMargin;
            }

            // Where the line holding text[index] starts.
            private static int LineStart(string text, int index)
            {
                int newline = index == 0 ? -1 : text.LastIndexOfAny(new[] { '\r', '\n' }, index - 1);
                return newline + 1;
            }

            // Whether Word broke the line only because the next line's first word did not fit
            // on it, so the two are one paragraph. A line that ends a paragraph would have had
            // room for it.
            private bool WrapsInto(int page, int lineStart, int lineEnd, int nextStart)
            {
                string text = template.PageText(page);
                int last = lineEnd - 1;
                while (last > lineStart && char.IsWhiteSpace(text[last]))
                {
                    last--;
                }

                PdfBox? lastBox = template.Bounds(page, last, 1);
                if (!lastBox.HasValue)
                {
                    return false;
                }

                int wordEnd = nextStart;
                while (wordEnd < text.Length && !char.IsWhiteSpace(text[wordEnd]))
                {
                    wordEnd++;
                }

                // The box ends at the ink, a little short of where Word measured the line to, so
                // a word that would only just have fitted is taken as not fitting.
                const double Leeway = 4;

                double needed = measurer.Width(" " + text.Substring(nextStart, wordEnd - nextStart), false, FontSize);
                return lastBox.Value.Right + needed > template.PageWidth(page) - TextRightMargin - Leeway;
            }

            // Where the first text of the line after the one ending at lineEnd starts, or -1
            // when that line is blank or there is none: wording is not carried over a gap.
            private static int NextLineStart(string text, int lineEnd)
            {
                int index = lineEnd;
                if (index < text.Length && text[index] == '\r')
                {
                    index++;
                }

                if (index < text.Length && text[index] == '\n')
                {
                    index++;
                }

                int nextEnd = LineEnd(text, index);
                while (index < nextEnd && char.IsWhiteSpace(text[index]))
                {
                    index++;
                }

                return index < nextEnd ? index : -1;
            }

            // Where the line holding text[index] ends.
            private static int LineEnd(string text, int index)
            {
                int end = text.IndexOfAny(new[] { '\r', '\n' }, index);
                return end < 0 ? text.Length : end;
            }

            // The area to paint over a blank: its width, and the height of an 11 point line
            // around its baseline, so the dots disappear without touching the lines above
            // and below.
            private static PdfBox CoverFor(PdfBox box, double baseline)
            {
                return new PdfBox(box.Left - 0.3, baseline - FontSize * 0.8, box.Right + 0.3, baseline + FontSize * 0.25);
            }
        }

        // Text in the template to write over, and what to write instead.
        private class Replacement
        {
            public Replacement(Group group, string text, bool bold)
                : this(group.Index, group.Length, text, bold)
            {
            }

            public Replacement(int index, int length, string text, bool bold)
            {
                Index = index;
                Length = length;
                Text = text;
                Bold = bold;
            }

            public int Index { get; private set; }
            public int Length { get; private set; }
            public string Text { get; private set; }
            public bool Bold { get; private set; }
        }

        // A word on a rewritten line, in parts where its weight changes ("2026" bold
        // followed by a regular ".").
        private class Word
        {
            public Word()
            {
                Parts = new List<Part>();
            }

            public List<Part> Parts { get; private set; }

            public void Append(char character, bool bold)
            {
                Part last = Parts.Count == 0 ? null : Parts[Parts.Count - 1];
                if (last == null || last.Bold != bold)
                {
                    last = new Part(bold);
                    Parts.Add(last);
                }

                last.Text += character;
            }

            public double Width(IContractTextMeasurer measurer, double size)
            {
                double width = 0;
                foreach (Part part in Parts)
                {
                    width += measurer.Width(part.Text, part.Bold, size);
                }

                return width;
            }

            public class Part
            {
                public Part(bool bold)
                {
                    Bold = bold;
                    Text = "";
                }

                public bool Bold { get; private set; }
                public string Text { get; set; }
            }
        }
    }
}
