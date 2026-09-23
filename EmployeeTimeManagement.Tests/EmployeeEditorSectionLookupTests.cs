using System.Collections.Generic;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class EmployeeEditorSectionLookupTests
    {
        private static EmployeeFieldError FieldError(string field)
        {
            return new EmployeeFieldError(field, "message");
        }

        [TestCase("Name", EmployeeEditorSection.Personal)]
        [TestCase("Surname", EmployeeEditorSection.Personal)]
        [TestCase("IDNumber", EmployeeEditorSection.Personal)]
        [TestCase("SARSNumber", EmployeeEditorSection.Personal)]
        [TestCase("MobileNumber", EmployeeEditorSection.Personal)]
        [TestCase("MaritalStatus", EmployeeEditorSection.Personal)]
        [TestCase("NumberOfDependents", EmployeeEditorSection.Personal)]
        [TestCase("HouseFlatNumber", EmployeeEditorSection.Address)]
        [TestCase("ComplexFlatNumber", EmployeeEditorSection.Address)]
        [TestCase("StreetName", EmployeeEditorSection.Address)]
        [TestCase("Town", EmployeeEditorSection.Address)]
        [TestCase("PostalCode", EmployeeEditorSection.Address)]
        [TestCase("BankName", EmployeeEditorSection.Bank)]
        [TestCase("AccountType", EmployeeEditorSection.Bank)]
        [TestCase("AccountNumber", EmployeeEditorSection.Bank)]
        [TestCase("BranchCode", EmployeeEditorSection.Bank)]
        [TestCase("ContractType", EmployeeEditorSection.Contract)]
        [TestCase("StartDate", EmployeeEditorSection.Contract)]
        [TestCase("Department", EmployeeEditorSection.Contract)]
        [TestCase("JobDescription", EmployeeEditorSection.Contract)]
        [TestCase("HourlyRate", EmployeeEditorSection.Contract)]
        [TestCase("OpeningPTODays", EmployeeEditorSection.Contract)]
        [TestCase("SpouseName", EmployeeEditorSection.Spouse)]
        [TestCase("SpouseMobileNumber", EmployeeEditorSection.Spouse)]
        [TestCase("FamilyMembers", EmployeeEditorSection.Family)]
        public void Each_field_maps_to_its_section(string field, EmployeeEditorSection expected)
        {
            EmployeeEditorSection? section = EmployeeEditorSectionLookup.SectionFor(FieldError(field));

            Assert.That(section, Is.EqualTo(expected));
        }

        [Test]
        public void A_family_row_error_maps_to_family_contacts_whatever_its_field_name()
        {
            var error = new EmployeeFieldError("Relationship", "A family contact needs a relationship.", 0);

            EmployeeEditorSection? section = EmployeeEditorSectionLookup.SectionFor(error);

            Assert.That(section, Is.EqualTo(EmployeeEditorSection.Family));
        }

        [Test]
        public void An_unknown_field_maps_to_no_section()
        {
            EmployeeEditorSection? section = EmployeeEditorSectionLookup.SectionFor(FieldError("SomethingUnheardOf"));

            Assert.That(section, Is.Null);
        }

        [Test]
        public void No_errors_picks_no_section()
        {
            EmployeeEditorSection? section = EmployeeEditorSectionLookup.FirstSectionWithErrors(new List<EmployeeFieldError>());

            Assert.That(section, Is.Null);
        }

        [Test]
        public void The_first_section_with_errors_follows_section_list_order_not_error_order()
        {
            var errors = new List<EmployeeFieldError>
            {
                FieldError("HourlyRate"),
                FieldError("BankName"),
                FieldError("Name")
            };

            EmployeeEditorSection? section = EmployeeEditorSectionLookup.FirstSectionWithErrors(errors);

            Assert.That(section, Is.EqualTo(EmployeeEditorSection.Personal));
        }

        [Test]
        public void Sections_with_errors_lists_every_affected_section_once_in_section_list_order()
        {
            var errors = new List<EmployeeFieldError>
            {
                FieldError("HourlyRate"),
                FieldError("Department"),
                FieldError("Name"),
                new EmployeeFieldError("Relationship", "message", 0)
            };

            List<EmployeeEditorSection> sections = EmployeeEditorSectionLookup.SectionsWithErrors(errors);

            Assert.That(sections, Is.EqualTo(new[]
            {
                EmployeeEditorSection.Personal,
                EmployeeEditorSection.Contract,
                EmployeeEditorSection.Family
            }));
        }

        [Test]
        public void An_unknown_field_among_others_is_ignored_when_picking_the_first_section()
        {
            var errors = new List<EmployeeFieldError>
            {
                FieldError("SomethingUnheardOf"),
                FieldError("BranchCode")
            };

            EmployeeEditorSection? section = EmployeeEditorSectionLookup.FirstSectionWithErrors(errors);

            Assert.That(section, Is.EqualTo(EmployeeEditorSection.Bank));
        }
    }
}
