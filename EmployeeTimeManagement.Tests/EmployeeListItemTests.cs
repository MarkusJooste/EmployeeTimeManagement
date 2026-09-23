using EmployeeTimeManagement.Models;

using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class EmployeeListItemTests
    {
        // One employee, named only, since the initials read nothing else.
        private static EmployeeListItem Employee(string name, string surname)
        {
            return new EmployeeListItem { Name = name, Surname = surname };
        }

        [Test]
        public void Initials_are_the_first_letter_of_the_name_and_the_surname()
        {
            Assert.That(Employee("Thabo", "Nkosi").Initials, Is.EqualTo("TN"));
        }

        [Test]
        public void Initials_are_capitals_whatever_case_the_name_was_captured_in()
        {
            Assert.That(Employee("thabo", "nkosi").Initials, Is.EqualTo("TN"));
        }

        [Test]
        public void Initials_ignore_the_whitespace_around_a_name()
        {
            Assert.That(Employee("  Thabo  ", "  Nkosi  ").Initials, Is.EqualTo("TN"));
        }

        [Test]
        public void An_employee_with_no_surname_on_record_gets_one_initial()
        {
            Assert.That(Employee("Thabo", null).Initials, Is.EqualTo("T"));
        }

        [Test]
        public void An_employee_with_no_name_at_all_gets_a_placeholder()
        {
            Assert.That(Employee(null, "   ").Initials, Is.EqualTo("?"));
        }
    }
}
