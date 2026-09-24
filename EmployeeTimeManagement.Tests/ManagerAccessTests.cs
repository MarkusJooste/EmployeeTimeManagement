using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class ManagerAccessTests
    {
        private const int ThisStore = 3;
        private const int AnotherStore = 8;

        private static PromotionRequest ValidPromotionRequest()
        {
            return new PromotionRequest
            {
                EmployeeID = 1,
                EmployeeStoreID = ThisStore,
                OwnerStoreID = ThisStore,
                ContractEndDate = null,
                Pin = "1234",
                PinConfirmation = "1234",
                ExistingManagers = new List<Manager>(),
                ExistingManagerRow = null,
                CapturedBy = 99,
                BusinessDate = new DateTime(2026, 3, 10)
            };
        }

        private static Manager ManagerAt(int storeID, bool isActive, bool isAdmin)
        {
            return new Manager
            {
                ManagerID = 42,
                EmployeeID = 7,
                ManagerName = "Thandi Mokoena",
                StoreID = storeID,
                Pin = "5555",
                IsActiveManager = isActive,
                IsAdmin = isAdmin
            };
        }

        // Promote

        [Test]
        public void AValidPromotionIsAccepted()
        {
            PromotionResult result = ManagerAccess.Promote(ValidPromotionRequest());

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Error, Is.Null);
            Assert.That(result.Manager, Is.Not.Null);
        }

        [Test]
        public void AValidPromotionReturnsARowCarryingTheEmployeesIDStoreAndPinActiveAndNotAdmin()
        {
            PromotionResult result = ManagerAccess.Promote(ValidPromotionRequest());

            Assert.That(result.Manager.EmployeeID, Is.EqualTo(1));
            Assert.That(result.Manager.StoreID, Is.EqualTo(ThisStore));
            Assert.That(result.Manager.Pin, Is.EqualTo("1234"));
            Assert.That(result.Manager.IsActiveManager, Is.True);
            Assert.That(result.Manager.IsAdmin, Is.False);
        }

        [Test]
        public void AnEmployeeAtAnotherStoreIsRefused()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.EmployeeStoreID = AnotherStore;

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ATerminatedEmployeeIsRefused()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.ContractEndDate = DateTime.Today.AddDays(-1);

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void AnEmployeeWhoseContractEndsTodayIsAccepted()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.ContractEndDate = DateTime.Today;

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void AnEmployeeWhoIsAlreadyAnActiveManagerIsRefused()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.ExistingManagers = new List<Manager>
            {
                new Manager { EmployeeID = request.EmployeeID, IsActiveManager = true, IsAdmin = false, Pin = "9999", ManagerName = "Existing" }
            };

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void AThreeDigitPinIsRefused()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.Pin = "123";
            request.PinConfirmation = "123";

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void AFiveDigitPinIsRefused()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.Pin = "12345";
            request.PinConfirmation = "12345";

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void AFourCharacterPinWithANonDigitIsRefused()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.Pin = "12a4";
            request.PinConfirmation = "12a4";

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void APinNotMatchingItsConfirmationIsRefused()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.Pin = "1234";
            request.PinConfirmation = "4321";

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void APinHeldByACurrentManagerIsRefusedAndNamesThem()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.ExistingManagers = new List<Manager>
            {
                new Manager { EmployeeID = 55, IsActiveManager = true, IsAdmin = false, Pin = "1234", ManagerName = "Thandi Mokoena" }
            };

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Error, Does.Contain("Thandi Mokoena"));
        }

        [Test]
        public void APinHeldByAFormerManagerIsRefusedAndNamesThemAsFormer()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.ExistingManagers = new List<Manager>
            {
                new Manager { EmployeeID = 55, IsActiveManager = false, IsAdmin = false, Pin = "1234", ManagerName = "Sipho Dlamini" }
            };

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Error, Does.Contain("Sipho Dlamini"));
            Assert.That(result.Error, Does.Contain("former"));
        }

        [Test]
        public void APinHeldByAnOwnerIsRefused()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.ExistingManagers = new List<Manager>
            {
                new Manager { EmployeeID = 55, IsActiveManager = true, IsAdmin = true, Pin = "1234", ManagerName = "Store Owner" }
            };

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ARePromotionTypingBackTheReconnectedRowsOwnFormerPinIsNotRefusedAsAClashAgainstItself()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.Pin = "5555";
            request.PinConfirmation = "5555";
            request.ExistingManagerRow = new Manager { ManagerID = 17, EmployeeID = 1, IsActiveManager = false, IsAdmin = false, Pin = "5555" };
            request.ExistingManagers = new List<Manager>
            {
                new Manager { ManagerID = 17, EmployeeID = 1, IsActiveManager = false, IsAdmin = false, Pin = "5555", ManagerName = "Same Person" }
            };

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void APromotionWithADemotedRowSuppliedReturnsThatRowsManagerID()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.ExistingManagerRow = new Manager { ManagerID = 17, EmployeeID = 1, IsActiveManager = false, IsAdmin = false, Pin = "5555" };

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.Manager.ManagerID, Is.EqualTo(17));
        }

        [Test]
        public void APromotionWithoutADemotedRowReturnsNoManagerID()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.ExistingManagerRow = null;

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.Manager.ManagerID, Is.EqualTo(0));
        }

        [Test]
        public void APromotionCarriesTheActingOwnerAsCapturedByAndTheGivenBusinessDate()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.CapturedBy = 123;
            request.BusinessDate = new DateTime(2026, 5, 1);

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.Manager.CapturedBy, Is.EqualTo(123));
            Assert.That(result.Manager.BusinessDate, Is.EqualTo(new DateTime(2026, 5, 1)));
        }

        [Test]
        public void ARefusedPromotionCarriesNoManagerRowAtAll()
        {
            PromotionRequest request = ValidPromotionRequest();
            request.EmployeeStoreID = AnotherStore;

            PromotionResult result = ManagerAccess.Promote(request);

            Assert.That(result.Manager, Is.Null);
        }

        // Demote

        private static DemotionRequest ValidDemotionRequest()
        {
            return new DemotionRequest
            {
                Manager = ManagerAt(ThisStore, isActive: true, isAdmin: false),
                ActingOwnerManagerID = 900,
                CapturedBy = 900,
                BusinessDate = new DateTime(2026, 3, 10)
            };
        }

        [Test]
        public void DemotingAManagerIsAccepted()
        {
            DemotionResult result = ManagerAccess.Demote(ValidDemotionRequest());

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Manager, Is.Not.Null);
            Assert.That(result.Manager.IsActiveManager, Is.False);
        }

        [Test]
        public void DemotingAnOwnerIsRefused()
        {
            DemotionRequest request = ValidDemotionRequest();
            request.Manager = ManagerAt(ThisStore, isActive: true, isAdmin: true);

            DemotionResult result = ManagerAccess.Demote(request);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void AnOwnerDemotingThemselvesIsRefused()
        {
            DemotionRequest request = ValidDemotionRequest();
            request.Manager = ManagerAt(ThisStore, isActive: true, isAdmin: true);
            request.Manager.ManagerID = request.ActingOwnerManagerID;

            DemotionResult result = ManagerAccess.Demote(request);

            Assert.That(result.IsValid, Is.False);
        }

        // Promotable

        private static Employee EmployeeAt(int employeeID, int storeID, DateTime? contractEndDate)
        {
            return new Employee
            {
                EmployeeID = employeeID,
                StoreID = storeID,
                Name = "Employee",
                Surname = employeeID.ToString(),
                ContractEndDate = contractEndDate
            };
        }

        [Test]
        public void PromotableExcludesOtherStores()
        {
            var employees = new List<Employee> { EmployeeAt(1, AnotherStore, null) };

            IEnumerable<Employee> result = ManagerAccess.Promotable(employees, new List<Manager>(), ThisStore);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void PromotableExcludesTerminatedEmployees()
        {
            var employees = new List<Employee> { EmployeeAt(1, ThisStore, DateTime.Today.AddDays(-1)) };

            IEnumerable<Employee> result = ManagerAccess.Promotable(employees, new List<Manager>(), ThisStore);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void PromotableExcludesActiveManagers()
        {
            var employees = new List<Employee> { EmployeeAt(1, ThisStore, null) };
            var managers = new List<Manager> { new Manager { EmployeeID = 1, IsActiveManager = true, IsAdmin = false } };

            IEnumerable<Employee> result = ManagerAccess.Promotable(employees, managers, ThisStore);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void PromotableIncludesAnEmployeeWhoseOnlyManagerRowIsDemoted()
        {
            var employees = new List<Employee> { EmployeeAt(1, ThisStore, null) };
            var managers = new List<Manager> { new Manager { EmployeeID = 1, IsActiveManager = false, IsAdmin = false } };

            IEnumerable<Employee> result = ManagerAccess.Promotable(employees, managers, ThisStore);

            Assert.That(result.Select(e => e.EmployeeID), Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void PromotableReturnsEmptyRatherThanNullWhenNothingQualifies()
        {
            IEnumerable<Employee> result = ManagerAccess.Promotable(new List<Employee>(), new List<Manager>(), ThisStore);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        // Demotable

        [Test]
        public void DemotableExcludesOwners()
        {
            var managers = new List<Manager> { ManagerAt(ThisStore, isActive: true, isAdmin: true) };

            IEnumerable<Manager> result = ManagerAccess.Demotable(managers, ThisStore);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void DemotableExcludesInactiveManagers()
        {
            var managers = new List<Manager> { ManagerAt(ThisStore, isActive: false, isAdmin: false) };

            IEnumerable<Manager> result = ManagerAccess.Demotable(managers, ThisStore);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void DemotableExcludesOtherStores()
        {
            var managers = new List<Manager> { ManagerAt(AnotherStore, isActive: true, isAdmin: false) };

            IEnumerable<Manager> result = ManagerAccess.Demotable(managers, ThisStore);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void DemotableReturnsEmptyRatherThanNullWhenNothingQualifies()
        {
            IEnumerable<Manager> result = ManagerAccess.Demotable(new List<Manager>(), ThisStore);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }
    }
}
