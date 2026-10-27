using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    // A stand-in for a contract PDF: lines of text where every character is 5 points wide
    // and lines are 20 points apart, so positions can be worked out by hand.
    internal class FakeTemplate : IContractTemplateText
    {
        public const double CharacterWidth = 5;
        public const double FirstBaseline = 100;
        public const double LineSpacing = 20;
        public const double Width = 612;

        // Where a justified line's last character ends; within 2 points of the margin the
        // planner looks for (612 - 53 = 559).
        public const double JustifiedRight = 561;

        private readonly List<List<Line>> pages = new List<List<Line>>();

        public FakeTemplate Page(params Line[] lines)
        {
            pages.Add(lines.ToList());
            return this;
        }

        // A line starting at the left margin, ending short of the right one.
        public static Line Ragged(string text)
        {
            return new Line(text, 56);
        }

        // A line placed so its last character ends at the right margin.
        public static Line Justified(string text)
        {
            return new Line(text, JustifiedRight - text.TrimEnd().Length * CharacterWidth);
        }

        public static double BaselineOf(int line)
        {
            return FirstBaseline + line * LineSpacing;
        }

        public int PageCount
        {
            get { return pages.Count; }
        }

        public double PageWidth(int page)
        {
            return Width;
        }

        public string PageText(int page)
        {
            return string.Join("\r\n", pages[page].Select(line => line.Text));
        }

        public PdfBox? Bounds(int page, int start, int length)
        {
            PdfBox? union = null;
            int offset = 0;

            for (int lineNumber = 0; lineNumber < pages[page].Count; lineNumber++)
            {
                Line line = pages[page][lineNumber];

                for (int column = 0; column < line.Text.Length; column++)
                {
                    int index = offset + column;
                    if (index < start || index >= start + length)
                    {
                        continue;
                    }

                    double left = line.X + column * CharacterWidth;
                    double baseline = BaselineOf(lineNumber);
                    var box = new PdfBox(left, baseline - 8, left + CharacterWidth, baseline);
                    union = union.HasValue ? union.Value.Union(box) : box;
                }

                offset += line.Text.Length + 2;
            }

            return union;
        }

        public class Line
        {
            public Line(string text, double x)
            {
                Text = text;
                X = x;
            }

            public string Text { get; private set; }
            public double X { get; private set; }
        }
    }

    // Measures text at 5 points a character at 11 point, scaled for other sizes.
    internal class FixedWidthMeasurer : IContractTextMeasurer
    {
        public double Width(string text, bool bold, double size)
        {
            return text.Length * FakeTemplate.CharacterWidth * size / 11;
        }
    }

    [TestFixture]
    public class ContractBlanksTests
    {
        private const string Ellipsis = "…";

        private static string Dots(int count)
        {
            return string.Concat(Enumerable.Repeat(Ellipsis, count));
        }

        private static ContractData CompleteData(ContractTemplate template)
        {
            return new ContractData
            {
                Template = template,
                FullName = "Thandi Mokoena",
                IDNumber = "9001015009087",
                AddressLine1 = "12 Main Road",
                AddressLine2 = "Paarl 7646",
                TelephoneNumber = "0821234567",
                NextOfKinName = "Sipho Mokoena",
                NextOfKinRelationship = "Brother",
                NextOfKinNumber = "0837654321",
                CommencementDate = new DateTime(2026, 3, 1),
                Position = "Waiter",
                HourlyRate = 35.50m,
                StoreID = 1,
                StoreName = "Wimpy Paarl Mall",
                SigningDate = new DateTime(2026, 10, 23)
            };
        }

        private static FakeTemplate.Line[] PageOne()
        {
            return new[]
            {
                FakeTemplate.Ragged("Name:"),
                FakeTemplate.Ragged("ID Number:"),
                FakeTemplate.Ragged("Address:"),
                FakeTemplate.Ragged(""),
                FakeTemplate.Ragged("Tel. no:"),
                FakeTemplate.Ragged("Next of kin:"),
                FakeTemplate.Ragged("Relationship:"),
                FakeTemplate.Ragged("Contact No.:")
            };
        }

        private static FakeTemplate.Line SigningLine()
        {
            return FakeTemplate.Ragged("SIGNED IN PAARL ON THIS " + Dots(5) + " DAY OF " + Dots(11) + "..202........ ");
        }

        private static ContractFillPlan Plan(ContractData data, FakeTemplate template)
        {
            return ContractBlanks.Plan(data, template, new FixedWidthMeasurer());
        }

        private static List<ContractFill> FillsFor(ContractFillPlan plan, string field)
        {
            return plan.Fills.Where(fill => fill.Field == field).ToList();
        }

        private static string RunText(ContractFill fill)
        {
            return string.Join(" ", fill.Runs.Select(run => run.Text));
        }

        private static double RightEdge(ContractTextRun run)
        {
            return run.X + new FixedWidthMeasurer().Width(run.Text, run.Bold, run.Size);
        }

        [Test]
        public void PageOneValuesGoOnTheLineAnInchRightOfEachLabel()
        {
            var template = new FakeTemplate().Page(PageOne());

            ContractFillPlan plan = Plan(CompleteData(ContractTemplate.General), template);

            ContractTextRun name = FillsFor(plan, ContractBlanks.NameField).Single().Runs.Single();
            Assert.That(name.Text, Is.EqualTo("Thandi Mokoena"));
            Assert.That(name.X, Is.EqualTo(56 + 72));
            Assert.That(name.Baseline, Is.EqualTo(FakeTemplate.BaselineOf(0)));
            Assert.That(name.Bold, Is.False);
            Assert.That(FillsFor(plan, ContractBlanks.NameField).Single().Cover, Is.Null);

            ContractTextRun relationship = FillsFor(plan, ContractBlanks.RelationshipField).Single().Runs.Single();
            Assert.That(relationship.Text, Is.EqualTo("Brother"));
            Assert.That(relationship.Baseline, Is.EqualTo(FakeTemplate.BaselineOf(6)));
        }

        [Test]
        public void SecondAddressLineSitsHalfwayBetweenAddressAndTelephone()
        {
            var template = new FakeTemplate().Page(PageOne());

            List<ContractFill> address = FillsFor(Plan(CompleteData(ContractTemplate.General), template), ContractBlanks.AddressField);

            Assert.That(address.Select(fill => fill.Runs.Single().Text), Is.EqualTo(new[] { "12 Main Road", "Paarl 7646" }));
            Assert.That(address[1].Runs.Single().Baseline, Is.EqualTo((FakeTemplate.BaselineOf(2) + FakeTemplate.BaselineOf(4)) / 2));
        }

        [Test]
        public void LongValuesShrinkToFitTheLine()
        {
            var template = new FakeTemplate().Page(PageOne());
            ContractData data = CompleteData(ContractTemplate.General);
            data.FullName = new string('x', 70);

            ContractTextRun name = FillsFor(Plan(data, template), ContractBlanks.NameField).Single().Runs.Single();

            Assert.That(name.Size, Is.LessThan(11));
            Assert.That(RightEdge(name) - name.X, Is.LessThanOrEqualTo(288));
        }

        [Test]
        public void MissingValuesAreLeftForHandwriting()
        {
            var template = new FakeTemplate().Page(PageOne());
            ContractData data = CompleteData(ContractTemplate.General);
            data.IDNumber = null;
            data.AddressLine1 = null;
            data.AddressLine2 = " ";

            ContractFillPlan plan = Plan(data, template);

            Assert.That(FillsFor(plan, ContractBlanks.IDNumberField), Is.Empty);
            Assert.That(FillsFor(plan, ContractBlanks.AddressField), Is.Empty);
            Assert.That(plan.NotFound, Is.Empty.Or.All.Not.EqualTo(ContractBlanks.IDNumberField));
            Assert.That(ContractBlanks.MissingFields(data), Is.EqualTo(new[] { ContractBlanks.IDNumberField, ContractBlanks.AddressField }));
        }

        [Test]
        public void SigningLineIsSetAgainWithNormalSpacingInBold()
        {
            var template = new FakeTemplate().Page(SigningLine());

            ContractFill fill = FillsFor(Plan(CompleteData(ContractTemplate.General), template), ContractBlanks.SigningDateField).Single();

            Assert.That(RunText(fill), Is.EqualTo("23rd DAY OF October 2026"));
            Assert.That(fill.Runs.All(run => run.Bold), Is.True);

            // Starts where the day's dots did, after "SIGNED IN PAARL ON THIS ".
            Assert.That(fill.Runs[0].X, Is.EqualTo(56 + 24 * FakeTemplate.CharacterWidth));
            Assert.That(fill.Cover.HasValue, Is.True);
        }

        [Test]
        public void YearOutsideThePrintedDecadeReplacesThePrintedDigits()
        {
            var template = new FakeTemplate().Page(SigningLine());
            ContractData data = CompleteData(ContractTemplate.General);
            data.SigningDate = new DateTime(2031, 1, 2);

            ContractFill fill = FillsFor(Plan(data, template), ContractBlanks.SigningDateField).Single();

            Assert.That(RunText(fill), Is.EqualTo("2nd DAY OF January 2031"));
        }

        [Test]
        public void EverySigningLineIsFilled()
        {
            var template = new FakeTemplate()
                .Page(FakeTemplate.Ragged("Body text"))
                .Page(SigningLine(), FakeTemplate.Ragged(""), SigningLine());

            List<ContractFill> fills = FillsFor(Plan(CompleteData(ContractTemplate.General), template), ContractBlanks.SigningDateField);

            Assert.That(fills.Select(fill => fill.Page), Is.EqualTo(new[] { 1, 1 }));
        }

        [Test]
        public void JustifiedCommencementLineKeepsItsRightEdge()
        {
            string line = "The commencement date of employment is/was " + Dots(13) + " 20" + Dots(2) + ".  This ";
            var template = new FakeTemplate().Page(FakeTemplate.Justified(line));

            List<ContractFill> fills = FillsFor(Plan(CompleteData(ContractTemplate.General), template), ContractBlanks.CommencementField);

            // The date is written over its dots...
            Assert.That(fills[0].Runs.Single().Text, Is.EqualTo("1 March"));
            Assert.That(fills[0].Cover.HasValue, Is.True);

            // ...and "2026." to the end of the line is set again, still reaching the margin.
            ContractFill rest = fills[1];
            Assert.That(RunText(rest), Is.EqualTo("2026 . This"));
            Assert.That(rest.Runs[0].Bold, Is.True, "the printed 20 and the year are bold");
            Assert.That(rest.Runs[1].Bold, Is.False, "the full stop is not");
            Assert.That(RightEdge(rest.Runs.Last()), Is.EqualTo(FakeTemplate.JustifiedRight).Within(0.001));
        }

        [Test]
        public void PositionIsWrittenWithItsOwnSpacingOnAJustifiedLine()
        {
            string line = "The EMPLOYEE is appointed in the position of " + Dots(19) + "..  and ";
            var template = new FakeTemplate().Page(FakeTemplate.Justified(line));
            ContractData data = CompleteData(ContractTemplate.General);
            data.Position = "Assistant Manager";

            ContractFill fill = FillsFor(Plan(data, template), ContractBlanks.PositionField).Single();

            Assert.That(fill.Runs.Select(run => run.Text), Is.EqualTo(new[] { "Assistant Manager", "and" }));
            Assert.That(RightEdge(fill.Runs[1]), Is.EqualTo(FakeTemplate.JustifiedRight).Within(0.001));
        }

        [Test]
        public void GeneralRateIsWrittenAfterThePrintedR()
        {
            var template = new FakeTemplate().Page(
                FakeTemplate.Ragged("6.1.1 The EMPLOYEE will receive a basic remuneration of R......................... hour (or if"));

            ContractFill fill = FillsFor(Plan(CompleteData(ContractTemplate.General), template), ContractBlanks.HourlyRateField).Single();

            Assert.That(RunText(fill), Is.EqualTo("35.50 per hour (or if"));
            Assert.That(fill.Runs.Select(run => run.Text), Is.EqualTo(new[] { "35.50 per", "hour", "(or", "if" }));
            Assert.That(fill.Runs.Take(2).All(run => run.Bold), Is.True, "R... hour is bold in the template");
            Assert.That(fill.Runs.Skip(2).Any(run => run.Bold), Is.False);
        }

        [Test]
        public void ManagerSalaryReplacesTheExampleFigures()
        {
            var template = new FakeTemplate().Page(FakeTemplate.Justified(
                "6.1.1 The EMPLOYEE will receive a basic remuneration of R10 197 per month (R48.558 per hour "));
            ContractData data = CompleteData(ContractTemplate.Manager);
            data.HourlyRate = 48.56m;

            ContractFill fill = FillsFor(Plan(data, template), ContractBlanks.SalaryField).Single();

            Assert.That(fill.Runs.Select(run => run.Text), Is.EqualTo(new[] { "R10 197.60", "per", "month", "(R48.56", "per", "hour" }));
            Assert.That(fill.Runs[0].Bold, Is.True);
            Assert.That(fill.Runs.Skip(1).Any(run => run.Bold), Is.False);
            Assert.That(RightEdge(fill.Runs.Last()), Is.EqualTo(FakeTemplate.JustifiedRight).Within(0.001));
        }

        [Test]
        public void ManagerSalaryWithoutARateIsDottedForHandwriting()
        {
            var template = new FakeTemplate().Page(FakeTemplate.Justified(
                "6.1.1 The EMPLOYEE will receive a basic remuneration of R10 197 per month (R48.558 per hour "));
            ContractData data = CompleteData(ContractTemplate.Manager);
            data.HourlyRate = null;

            ContractFillPlan plan = Plan(data, template);
            string text = RunText(FillsFor(plan, ContractBlanks.SalaryField).Single());

            Assert.That(text, Does.StartWith("R" + Ellipsis));
            Assert.That(text, Does.Not.Contain("197"));
            Assert.That(ContractBlanks.MissingFields(data), Does.Contain(ContractBlanks.SalaryField));
        }

        [Test]
        public void ManagerCommencementIsCentredOnItsUnderlines()
        {
            string line = "The commencement date employment is/was the      day of          20    . If the";
            var template = new FakeTemplate().Page(FakeTemplate.Ragged(line));

            List<ContractFill> fills = FillsFor(Plan(CompleteData(ContractTemplate.Manager), template), ContractBlanks.CommencementField);

            Assert.That(fills.Select(fill => fill.Runs.Single().Text), Is.EqualTo(new[] { "1st", "March", "26" }));
            Assert.That(fills.All(fill => fill.Cover == null), Is.True, "underlines are left as they are");

            // "1st" is centred between "the" and "day".
            double theEnds = 56 + line.IndexOf("the ", StringComparison.Ordinal) * 5 + 15;
            double dayStarts = 56 + line.IndexOf("day", StringComparison.Ordinal) * 5;
            ContractTextRun day = fills[0].Runs.Single();
            Assert.That(day.X + 7.5, Is.EqualTo((theEnds + 1 + dayStarts - 1) / 2).Within(0.001));

            // The year carries straight on from the printed "20".
            double twentyEnds = 56 + (line.IndexOf("20", StringComparison.Ordinal) + 2) * 5;
            Assert.That(fills[2].Runs.Single().X, Is.EqualTo(twentyEnds + 0.6).Within(0.001));
        }

        [Test]
        public void BlanksMissingFromTheTemplateAreReported()
        {
            var template = new FakeTemplate().Page(FakeTemplate.Ragged("Some other document"));

            ContractFillPlan plan = Plan(CompleteData(ContractTemplate.General), template);

            Assert.That(plan.Fills, Is.Empty);
            Assert.That(plan.NotFound, Is.SupersetOf(new[]
            {
                ContractBlanks.NameField, ContractBlanks.CommencementField, ContractBlanks.PositionField,
                ContractBlanks.HourlyRateField, ContractBlanks.SigningDateField
            }));
        }

        [Test]
        public void ManagerContractAlwaysLeavesThePreviousEmployerDate()
        {
            Assert.That(ContractBlanks.MissingFields(CompleteData(ContractTemplate.Manager)),
                Is.EqualTo(new[] { ContractBlanks.PreviousEmployerField }));
            Assert.That(ContractBlanks.MissingFields(CompleteData(ContractTemplate.General)), Is.Empty);
        }
    }

    [TestFixture]
    public class ContractFormatTests
    {
        [TestCase(1, "1st")]
        [TestCase(2, "2nd")]
        [TestCase(3, "3rd")]
        [TestCase(4, "4th")]
        [TestCase(11, "11th")]
        [TestCase(12, "12th")]
        [TestCase(13, "13th")]
        [TestCase(21, "21st")]
        [TestCase(22, "22nd")]
        [TestCase(23, "23rd")]
        [TestCase(31, "31st")]
        public void OrdinalDays(int day, string expected)
        {
            Assert.That(ContractFormat.Ordinal(day), Is.EqualTo(expected));
        }

        [TestCase("35.5", "R35.50")]
        [TestCase("10197.6", "R10 197.60")]
        [TestCase("1234567.891", "R1 234 567.89")]
        public void RandUsesSpacesBetweenThousands(string value, string expected)
        {
            Assert.That(ContractFormat.Rand(decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture)), Is.EqualTo(expected));
        }

        [Test]
        public void MonthlySalaryDropsCentsOnlyWhenThereAreNone()
        {
            Assert.That(ContractFormat.RandDroppingZeroCents(10197m), Is.EqualTo("R10 197"));
            Assert.That(ContractFormat.RandDroppingZeroCents(10197.60m), Is.EqualTo("R10 197.60"));
        }

        [Test]
        public void MonthlySalaryIs210PaidHours()
        {
            // The client's own figures: R48.558 an hour is R10 197 a month.
            Assert.That(decimal.Round(ContractFormat.MonthlySalary(48.558m), 0), Is.EqualTo(10197m));
            Assert.That(ContractFormat.MonthlySalary(48.56m), Is.EqualTo(10197.60m));
        }

        [Test]
        public void MissingValuesFormatAsNull()
        {
            Assert.That(ContractFormat.Rand(null), Is.Null);
            Assert.That(ContractFormat.LongDate(null), Is.Null);
            Assert.That(ContractFormat.LongDate(new DateTime(2026, 3, 1)), Is.EqualTo("1 March 2026"));
        }
    }

    [TestFixture]
    public class ContractSourceTests
    {
        private static ContractSource Source(string position, bool linkedManager)
        {
            var source = new ContractSource
            {
                Employee = new Employee
                {
                    EmployeeID = 7, StoreID = 1, Name = "Thandi", Surname = "Mokoena",
                    IDNumber = "9001015009087", MobileNumber = "0821234567"
                },
                StoreName = "Wimpy Paarl Mall",
                IsLinkedManager = linkedManager
            };

            if (position != null)
            {
                source.Contract = new EmployeeContract
                {
                    EmployeeID = 7,
                    JobDescription = position,
                    StartDate = new DateTime(2026, 3, 1),
                    HourlyRate = 35.50m
                };
            }

            return source;
        }

        // The expected template is passed as a bool because the test method is public and
        // ContractTemplate is internal to the application.
        [TestCase("Waiter", false, false)]
        [TestCase("Assistant Manager", false, true)]
        [TestCase("MANAGER", false, true)]
        [TestCase("Waiter", true, true)]
        [TestCase(null, false, false)]
        public void SuggestsTheManagerTemplateForManagers(string position, bool linkedManager, bool expectManager)
        {
            ContractTemplate expected = expectManager ? ContractTemplate.Manager : ContractTemplate.General;
            Assert.That(Source(position, linkedManager).SuggestedTemplate(), Is.EqualTo(expected));
        }

        [Test]
        public void NextOfKinOffersFamilyThenSpouseThenBlank()
        {
            ContractSource source = Source("Waiter", false);
            source.FamilyMembers.Add(new EmployeeFamilyMember { FamilyMemberName = "Sipho Mokoena", Relationship = "Brother", MobileNumber = "0837654321" });
            source.FamilyMembers.Add(new EmployeeFamilyMember { FamilyMemberName = "  ", Relationship = "Cousin" });
            source.FamilyMembers.Add(new EmployeeFamilyMember { FamilyMemberName = "Lerato Mokoena", Relationship = "Mother" });
            source.Spouse = new EmployeeSpouse { SpouseName = "John Mokoena", SpouseMobileNumber = "0841112222" };

            List<NextOfKinOption> options = source.NextOfKinOptions();

            Assert.That(options.Select(option => option.ToString()), Is.EqualTo(new[]
            {
                "Sipho Mokoena (Brother)", "Lerato Mokoena (Mother)", "John Mokoena (Spouse)", "(Leave blank)"
            }));
            Assert.That(options[2].ContactNumber, Is.EqualTo("0841112222"));
        }

        [Test]
        public void WithNoFamilyOrSpouseTheOnlyOptionIsBlank()
        {
            Assert.That(Source("Waiter", false).NextOfKinOptions(), Is.EqualTo(new[] { NextOfKinOption.None }));
        }

        [Test]
        public void AddressSplitsIntoUnitAndStreetThenTownAndCode()
        {
            var address = new EmployeeAddress
            {
                ComplexFlatNumber = "Unit 4",
                HouseFlatNumber = "12",
                StreetName = "Main Road",
                Town = "Paarl",
                PostalCode = "7646"
            };

            string line1;
            string line2;
            ContractSource.FormatAddress(address, out line1, out line2);

            Assert.That(line1, Is.EqualTo("Unit 4, 12 Main Road"));
            Assert.That(line2, Is.EqualTo("Paarl 7646"));
        }

        [Test]
        public void AddressLeavesOutEmptyParts()
        {
            var address = new EmployeeAddress { ComplexFlatNumber = "", HouseFlatNumber = "12", StreetName = "Main Road", Town = "Paarl", PostalCode = " " };

            string line1;
            string line2;
            ContractSource.FormatAddress(address, out line1, out line2);

            Assert.That(line1, Is.EqualTo("12 Main Road"));
            Assert.That(line2, Is.EqualTo("Paarl"));
        }

        [Test]
        public void ContractDataCarriesTheStoredValues()
        {
            ContractSource source = Source("Waiter", false);
            var nextOfKin = new NextOfKinOption("Sipho Mokoena", "Brother", "0837654321");

            ContractData data = source.ToContractData(nextOfKin, ContractTemplate.General, new DateTime(2026, 10, 22, 15, 30, 0));

            Assert.That(data.FullName, Is.EqualTo("Thandi Mokoena"));
            Assert.That(data.IDNumber, Is.EqualTo("9001015009087"));
            Assert.That(data.TelephoneNumber, Is.EqualTo("0821234567"));
            Assert.That(data.NextOfKinRelationship, Is.EqualTo("Brother"));
            Assert.That(data.Position, Is.EqualTo("Waiter"));
            Assert.That(data.HourlyRate, Is.EqualTo(35.50m));
            Assert.That(data.CommencementDate, Is.EqualTo(new DateTime(2026, 3, 1)));
            Assert.That(data.SigningDate, Is.EqualTo(new DateTime(2026, 10, 22)));
            Assert.That(data.StoreID, Is.EqualTo(1));
            Assert.That(data.StoreName, Is.EqualTo("Wimpy Paarl Mall"));
        }

        [Test]
        public void ZeroHourlyRateIsTreatedAsNotCaptured()
        {
            ContractSource source = Source("Waiter", false);
            source.Contract.HourlyRate = 0m;

            ContractData data = source.ToContractData(null, ContractTemplate.General, DateTime.Today);

            Assert.That(data.HourlyRate, Is.Null);
            Assert.That(data.NextOfKinName, Is.Null);
        }
    }
}
