using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class ContractStoreWordingTests
    {
        private static string Dots(int count)
        {
            return string.Concat(Enumerable.Repeat("…", count));
        }

        // The store spots of a contract, worded for the given store, on one page.
        private static FakeTemplate StorePage(string header, string business, string workplace, string town, string weekday)
        {
            return new FakeTemplate().Page(
                FakeTemplate.Ragged("WIMPY "),
                FakeTemplate.Ragged(header + " "),
                FakeTemplate.Ragged("wimpypaarlmall@gmail.com "),
                FakeTemplate.Ragged("currently known as " + business + " and the authority of the Employer"),
                FakeTemplate.Ragged("The premise situated at " + workplace + ", as well as any"),
                FakeTemplate.Ragged("Monday – Friday: "),
                FakeTemplate.Ragged(weekday + ". "),
                FakeTemplate.Ragged("These maybe amended as per policy of " + header + ". "),
                FakeTemplate.Ragged("SIGNED IN " + town + " ON THIS " + Dots(5) + " DAY OF " + Dots(11) + "..202........ "));
        }

        private static FakeTemplate PaarlPage()
        {
            return StorePage("Paarl Mall", "Wimpy Paarl Mall", "Shop 52, Paarl Mall", "PAARL", "07H30 – 18H15");
        }

        private static ContractData Data()
        {
            return new ContractData
            {
                Template = ContractTemplate.General,
                FullName = "Thandi Mokoena",
                StoreID = 2,
                SigningDate = new DateTime(2026, 10, 29)
            };
        }

        private static ContractFillPlan Plan(FakeTemplate template, Dictionary<string, string> wording)
        {
            return ContractBlanks.Plan(Data(), template, new FixedWidthMeasurer(), wording);
        }

        private static Dictionary<string, string> WellingtonWording()
        {
            return ContractStoreWording.Read(StorePage(
                "Market Square Wellington", "Wimpy Market Square", "Market Square Wellington", "WELLINGTON", "07H30 – 18H30"));
        }

        private static string TextOf(ContractFill fill)
        {
            return string.Join(" ", fill.Runs.Select(run => run.Text));
        }

        [Test]
        public void ReadsEachStoreSpotFromTheStoresOwnContract()
        {
            Dictionary<string, string> wording = ContractStoreWording.Read(PaarlPage());

            Assert.That(wording[ContractStoreWording.HeaderField], Is.EqualTo("Paarl Mall"));
            Assert.That(wording[ContractStoreWording.BusinessNameField], Is.EqualTo("Wimpy Paarl Mall"));
            Assert.That(wording[ContractStoreWording.WorkplaceField], Is.EqualTo("Shop 52, Paarl Mall"));
            Assert.That(wording[ContractStoreWording.WeekdayHoursField], Is.EqualTo("07H30 – 18H15"));
            Assert.That(wording[ContractStoreWording.PolicyField], Is.EqualTo("Paarl Mall"));
            Assert.That(wording[ContractStoreWording.TownField], Is.EqualTo("PAARL"));
        }

        [Test]
        public void WordingWrappedOntoTheNextLineIsReadAsOneLine()
        {
            var template = new FakeTemplate().Page(
                FakeTemplate.Ragged("currently known as Wimpy Market"),
                FakeTemplate.Ragged("Square and the authority of the Employer"));

            Assert.That(ContractStoreWording.Read(template)[ContractStoreWording.BusinessNameField], Is.EqualTo("Wimpy Market Square"));
        }

        [Test]
        public void TheOtherStoresWordingIsWrittenOverEachSpot()
        {
            ContractFillPlan plan = Plan(PaarlPage(), WellingtonWording());

            Assert.That(TextOf(plan.Fills.Single(fill => fill.Field == ContractStoreWording.HeaderField)), Is.EqualTo("Market Square Wellington"));
            Assert.That(TextOf(plan.Fills.Single(fill => fill.Field == ContractStoreWording.BusinessNameField)),
                Is.EqualTo("Wimpy Market Square and the authority of the Employer"));
            Assert.That(TextOf(plan.Fills.Single(fill => fill.Field == ContractStoreWording.WorkplaceField)),
                Is.EqualTo("Market Square Wellington, as well as any"));
            Assert.That(TextOf(plan.Fills.Single(fill => fill.Field == ContractStoreWording.WeekdayHoursField)), Is.EqualTo("07H30 – 18H30."));
            Assert.That(TextOf(plan.Fills.Single(fill => fill.Field == ContractStoreWording.PolicyField)),
                Is.EqualTo("Market Square Wellington."));
            // The page has none of the employee blanks, only the store spots.
            Assert.That(plan.NotFound.Intersect(ContractStoreWording.Fields), Is.Empty);
        }

        [Test]
        public void WordingIsWrittenWhereTheOldWordingStarted()
        {
            ContractFillPlan plan = Plan(PaarlPage(), WellingtonWording());
            ContractFill workplace = plan.Fills.Single(fill => fill.Field == ContractStoreWording.WorkplaceField);

            // After "The premise situated at ", on the workplace line.
            Assert.That(workplace.Runs[0].X, Is.EqualTo(56 + 24 * FakeTemplate.CharacterWidth));
            Assert.That(workplace.Runs[0].Baseline, Is.EqualTo(FakeTemplate.BaselineOf(4)));
            Assert.That(workplace.Cover.HasValue, Is.True);
        }

        [Test]
        public void TheSigningLineIsSignedInTheOtherStoresTown()
        {
            ContractFillPlan plan = Plan(PaarlPage(), WellingtonWording());
            ContractFill signing = plan.Fills.Single(fill => fill.Field == ContractBlanks.SigningDateField);

            Assert.That(TextOf(signing), Does.StartWith("WELLINGTON ON THIS 29th DAY OF October 2026"));
            Assert.That(signing.Runs.All(run => run.Bold), Is.True);
        }

        [Test]
        public void LongerWordingCarriesItsLastWordsOntoTheNextLine()
        {
            // The new name starts 138 points short of the right margin (564), so "Wimpy
            // Market Square" fits there and "Wellington" does not.
            string before = new string('x', 54) + " currently known as ";
            var template = new FakeTemplate().Page(
                FakeTemplate.Ragged(before + "Wimpy Paarl Mall"),
                FakeTemplate.Ragged("and the authority of the Employer"),
                FakeTemplate.Ragged("thereof."));

            var wording = new Dictionary<string, string>
            {
                { ContractStoreWording.BusinessNameField, "Wimpy Market Square Wellington" }
            };

            List<ContractFill> fills = Plan(template, wording).Fills
                .Where(fill => fill.Field == ContractStoreWording.BusinessNameField).ToList();

            Assert.That(fills.Count, Is.EqualTo(2));
            Assert.That(TextOf(fills[0]), Is.EqualTo("Wimpy Market Square"));
            Assert.That(TextOf(fills[1]), Is.EqualTo("Wellington and the authority of the Employer"));
            Assert.That(fills[1].Runs[0].X, Is.EqualTo(56));
            Assert.That(fills[1].Runs[0].Baseline, Is.EqualTo(FakeTemplate.BaselineOf(1)));
            Assert.That(fills[1].Runs.All(run => run.Size == 11), Is.True);
        }

        [Test]
        public void ShorterWordingOnAJustifiedLineTakesWordsUpFromTheNextLine()
        {
            // The new wording starts at 396, leaving room up to the margin (564) for
            // "Shop 52, Paarl Mall, as well as" but not "anything".
            var template = new FakeTemplate().Page(
                FakeTemplate.Justified("The premise situated at Market Sqaure Wellington, as well"),
                FakeTemplate.Ragged("as anything other premise."));

            var wording = new Dictionary<string, string> { { ContractStoreWording.WorkplaceField, "Shop 52, Paarl Mall" } };

            List<ContractFill> fills = Plan(template, wording).Fills
                .Where(fill => fill.Field == ContractStoreWording.WorkplaceField).ToList();

            Assert.That(fills.Select(TextOf), Is.EqualTo(new[] { "Shop 52, Paarl Mall, as well as", "anything other premise." }));

            // Still justified: the last word ends at the old right edge.
            ContractTextRun last = fills[0].Runs.Last();
            Assert.That(last.X + last.Text.Length * FakeTemplate.CharacterWidth, Is.EqualTo(FakeTemplate.JustifiedRight).Within(0.01));
        }

        [Test]
        public void ShorterWordingSpanningTwoLinesLeavesTheSecondWithoutTheOldWords()
        {
            var template = new FakeTemplate().Page(
                FakeTemplate.Ragged("currently known as Wimpy Market"),
                FakeTemplate.Ragged("Square and the authority of the Employer"));

            var wording = new Dictionary<string, string> { { ContractStoreWording.BusinessNameField, "Wimpy Paarl Mall" } };

            List<ContractFill> fills = Plan(template, wording).Fills
                .Where(fill => fill.Field == ContractStoreWording.BusinessNameField).ToList();

            // The rest of the sentence fits on the first line, so the second is painted over.
            string written = string.Join(" ", fills.Select(TextOf).Where(text => text.Length > 0));
            Assert.That(written, Is.EqualTo("Wimpy Paarl Mall and the authority of the Employer"));
            Assert.That(fills.Count, Is.EqualTo(2));
            Assert.That(fills[1].Cover.HasValue, Is.True);
        }

        [Test]
        public void WithoutAnotherStoresWordingNothingExtraIsWritten()
        {
            ContractFillPlan plan = Plan(PaarlPage(), null);

            Assert.That(plan.Fills.Any(fill => ContractStoreWording.Fields.Contains(fill.Field)), Is.False);
            Assert.That(TextOf(plan.Fills.Single(fill => fill.Field == ContractBlanks.SigningDateField)), Does.Not.Contain("WELLINGTON"));
        }

        [Test]
        public void TheSameWordingIsLeftAsItIs()
        {
            ContractFillPlan plan = Plan(PaarlPage(), ContractStoreWording.Read(PaarlPage()));

            Assert.That(plan.Fills.Any(fill => ContractStoreWording.Fields.Contains(fill.Field)), Is.False);
        }
    }
}
