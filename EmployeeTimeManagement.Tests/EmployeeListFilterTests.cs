using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeTimeManagement.Models;

using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class EmployeeListFilterTests
    {
        // One employee to filter, carrying only the details a search can match on.
        private static EmployeeListItem Employee(
            int employeeID,
            string name,
            string surname,
            DateTime? contractEndDate = null,
            string idNumber = null,
            string mobileNumber = null,
            string jobDescription = null)
        {
            return new EmployeeListItem
            {
                EmployeeID = employeeID,
                Name = name,
                Surname = surname,
                ContractEndDate = contractEndDate,
                IDNumber = idNumber,
                MobileNumber = mobileNumber,
                JobDescription = jobDescription
            };
        }

        // Two still employed, one who left in 2025, in an order the filter must not disturb.
        private static List<EmployeeListItem> MixedStore()
        {
            return new List<EmployeeListItem>
            {
                Employee(1, "Thabo", "Nkosi", idNumber: "8001015800081", mobileNumber: "0821234567", jobDescription: "Cashier"),
                Employee(2, "Sarah", "Botha", new DateTime(2025, 3, 1), idNumber: "9002026900082", mobileNumber: "0739876543", jobDescription: "Baker"),
                Employee(3, "Pieter", "Venter", idNumber: "7503037500083", mobileNumber: "0845551212", jobDescription: "Driver")
            };
        }

        // Who is visible, as the identifiers a test can state an expectation about in order.
        private static int[] IdsOf(EmployeeListFilterResult result)
        {
            return result.Visible.Select(employee => employee.EmployeeID).ToArray();
        }

        [Test]
        public void The_active_scope_shows_only_employees_who_are_still_employed()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.Active, string.Empty);

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 1, 3 }));
        }

        [Test]
        public void The_former_scope_shows_only_employees_whose_contract_has_ended()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.Former, string.Empty);

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 2 }));
        }

        [Test]
        public void The_all_scope_shows_every_employee()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.All, string.Empty);

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void The_counts_describe_the_whole_store_whichever_scope_is_chosen()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.Former, string.Empty);

            Assert.Multiple(() =>
            {
                Assert.That(result.ActiveCount, Is.EqualTo(2));
                Assert.That(result.FormerCount, Is.EqualTo(1));
                Assert.That(result.AllCount, Is.EqualTo(3));
            });
        }

        [Test]
        public void The_counts_ignore_the_search_text()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.All, "Thabo");

            Assert.Multiple(() =>
            {
                Assert.That(IdsOf(result), Is.EqualTo(new[] { 1 }));
                Assert.That(result.ActiveCount, Is.EqualTo(2));
                Assert.That(result.FormerCount, Is.EqualTo(1));
                Assert.That(result.AllCount, Is.EqualTo(3));
            });
        }

        [Test]
        public void An_empty_search_shows_the_whole_scope()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.All, "   ");

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void Search_matches_a_name()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.All, "Pieter");

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 3 }));
        }

        [Test]
        public void Search_matches_a_surname()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.All, "Botha");

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 2 }));
        }

        [Test]
        public void Search_matches_a_name_and_surname_typed_together()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.All, "Thabo Nkosi");

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void Search_matches_an_ID_number()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.All, "7503037500083");

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 3 }));
        }

        [Test]
        public void Search_matches_a_mobile_number()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.All, "0739876543");

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 2 }));
        }

        [Test]
        public void Search_matches_a_job_description()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.All, "Driver");

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 3 }));
        }

        [Test]
        public void Search_ignores_upper_and_lower_case()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.All, "cASHIER");

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void Search_trims_the_whitespace_around_what_was_typed()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.All, "  Botha  ");

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 2 }));
        }

        [Test]
        public void A_missing_field_never_matches_a_search()
        {
            var employees = new List<EmployeeListItem>
            {
                Employee(1, "Thabo", "Nkosi", idNumber: null, mobileNumber: null, jobDescription: null)
            };

            EmployeeListFilterResult result = EmployeeListFilter.Apply(employees, EmployeeScope.All, "Cashier");

            Assert.That(result.Visible, Is.Empty);
        }

        [Test]
        public void A_missing_name_and_surname_never_match_a_search()
        {
            var employees = new List<EmployeeListItem>
            {
                Employee(1, null, null, idNumber: "8001015800081", mobileNumber: null, jobDescription: null)
            };

            EmployeeListFilterResult result = EmployeeListFilter.Apply(employees, EmployeeScope.All, "Nkosi");

            Assert.That(result.Visible, Is.Empty);
        }

        [Test]
        public void An_employee_with_only_a_name_is_still_found_by_it()
        {
            var employees = new List<EmployeeListItem>
            {
                Employee(1, "Thabo", null)
            };

            EmployeeListFilterResult result = EmployeeListFilter.Apply(employees, EmployeeScope.All, "thabo");

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void A_search_nobody_matches_shows_nothing_but_still_counts_the_store()
        {
            EmployeeListFilterResult result = EmployeeListFilter.Apply(MixedStore(), EmployeeScope.All, "Mokoena");

            Assert.Multiple(() =>
            {
                Assert.That(result.Visible, Is.Empty);
                Assert.That(result.AllCount, Is.EqualTo(3));
            });
        }

        [Test]
        public void The_visible_employees_keep_the_order_they_came_in()
        {
            var employees = new List<EmployeeListItem>
            {
                Employee(3, "Zanele", "Adams"),
                Employee(1, "Ayanda", "Zulu"),
                Employee(2, "Mandla", "Khumalo")
            };

            EmployeeListFilterResult result = EmployeeListFilter.Apply(employees, EmployeeScope.All, string.Empty);

            Assert.That(IdsOf(result), Is.EqualTo(new[] { 3, 1, 2 }));
        }

        [Test]
        public void An_employee_whose_contract_ends_today_counts_as_former()
        {
            var employees = new List<EmployeeListItem>
            {
                Employee(1, "Thabo", "Nkosi", DateTime.Today)
            };

            EmployeeListFilterResult result = EmployeeListFilter.Apply(employees, EmployeeScope.Former, string.Empty);

            Assert.Multiple(() =>
            {
                Assert.That(IdsOf(result), Is.EqualTo(new[] { 1 }));
                Assert.That(result.FormerCount, Is.EqualTo(1));
                Assert.That(result.ActiveCount, Is.EqualTo(0));
            });
        }
    }
}
