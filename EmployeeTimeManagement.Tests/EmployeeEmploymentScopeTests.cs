using System;
using EmployeeTimeManagement.Models;

using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class EmployeeEmploymentScopeTests
    {
        private static readonly DateTime WorkDate = new DateTime(2026, 6, 15);

        private static Employee EmployeeWithContract(DateTime? startDate, DateTime? endDate)
        {
            return new Employee
            {
                ContractStartDate = startDate,
                ContractEndDate = endDate
            };
        }

        [Test]
        public void An_employee_with_no_contract_row_is_employed_on_any_date()
        {
            Employee employee = EmployeeWithContract(null, null);

            Assert.That(employee.IsEmployedOn(WorkDate), Is.True);
        }

        [Test]
        public void An_employee_whose_contract_started_before_the_work_date_and_has_not_ended_is_employed()
        {
            Employee employee = EmployeeWithContract(new DateTime(2026, 1, 1), null);

            Assert.That(employee.IsEmployedOn(WorkDate), Is.True);
        }

        [Test]
        public void An_employee_whose_contract_starts_after_the_work_date_is_not_employed()
        {
            Employee employee = EmployeeWithContract(new DateTime(2026, 7, 1), null);

            Assert.That(employee.IsEmployedOn(WorkDate), Is.False);
        }

        [Test]
        public void An_employee_terminated_after_the_work_date_is_still_employed()
        {
            Employee employee = EmployeeWithContract(new DateTime(2026, 1, 1), new DateTime(2026, 6, 20));

            Assert.That(employee.IsEmployedOn(WorkDate), Is.True);
        }

        [Test]
        public void An_employee_terminated_before_the_work_date_is_not_employed()
        {
            Employee employee = EmployeeWithContract(new DateTime(2026, 1, 1), new DateTime(2026, 3, 1));

            Assert.That(employee.IsEmployedOn(WorkDate), Is.False);
        }

        [Test]
        public void An_employee_terminated_on_the_work_date_is_still_employed()
        {
            Employee employee = EmployeeWithContract(new DateTime(2026, 1, 1), WorkDate);

            Assert.That(employee.IsEmployedOn(WorkDate), Is.True);
        }

        [Test]
        public void An_employee_whose_contract_starts_on_the_work_date_is_employed()
        {
            Employee employee = EmployeeWithContract(WorkDate, null);

            Assert.That(employee.IsEmployedOn(WorkDate), Is.True);
        }
    }
}
