using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class EmployeeIDNumberClashTests
    {
        private const int ThisStore = 3;
        private const int AnotherStore = 8;

        private static EmployeeIDNumberOwner OwnerAt(int storeID)
        {
            return new EmployeeIDNumberOwner
            {
                StoreID = storeID,
                Name = "Thandi",
                Surname = "Mokoena"
            };
        }

        [Test]
        public void An_id_number_nobody_holds_does_not_clash()
        {
            Assert.That(EmployeeCapture.DescribeIDNumberClash(null, ThisStore), Is.Null);
        }

        [Test]
        public void An_id_number_held_at_this_store_is_refused()
        {
            EmployeeFieldError error = EmployeeCapture.DescribeIDNumberClash(OwnerAt(ThisStore), ThisStore);

            Assert.That(error, Is.Not.Null);
            Assert.That(error.Field, Is.EqualTo("IDNumber"));
        }

        [Test]
        public void An_id_number_held_at_this_store_names_the_employee_holding_it()
        {
            EmployeeFieldError error = EmployeeCapture.DescribeIDNumberClash(OwnerAt(ThisStore), ThisStore);

            Assert.That(error.Message, Does.Contain("Thandi"));
            Assert.That(error.Message, Does.Contain("Mokoena"));
        }

        [Test]
        public void An_id_number_held_at_another_store_is_refused()
        {
            EmployeeFieldError error = EmployeeCapture.DescribeIDNumberClash(OwnerAt(AnotherStore), ThisStore);

            Assert.That(error, Is.Not.Null);
            Assert.That(error.Field, Is.EqualTo("IDNumber"));
        }

        [Test]
        public void An_id_number_held_at_another_store_names_neither_the_employee_nor_the_store()
        {
            EmployeeFieldError error = EmployeeCapture.DescribeIDNumberClash(OwnerAt(AnotherStore), ThisStore);

            Assert.That(error.Message, Does.Not.Contain("Thandi"));
            Assert.That(error.Message, Does.Not.Contain("Mokoena"));
            Assert.That(error.Message, Does.Not.Contain(AnotherStore.ToString()));
        }

        [Test]
        public void An_id_number_held_at_another_store_still_says_it_is_taken()
        {
            EmployeeFieldError error = EmployeeCapture.DescribeIDNumberClash(OwnerAt(AnotherStore), ThisStore);

            Assert.That(error.Message, Does.Contain("another store"));
        }
    }
}
