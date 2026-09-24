using System;
using System.Collections.Generic;

namespace EmployeeTimeManagement.Models
{
    // Everything Promote needs to decide whether an Employee may become a Manager.
    public class PromotionRequest
    {
        public int EmployeeID { get; set; }
        public int EmployeeStoreID { get; set; }
        public int OwnerStoreID { get; set; }

        // Null when the Employee is not terminated.
        public DateTime? ContractEndDate { get; set; }

        public string Pin { get; set; }
        public string PinConfirmation { get; set; }

        // Every existing User's row, since PIN uniqueness spans every store.
        public IEnumerable<Manager> ExistingManagers { get; set; } = new List<Manager>();

        // The demoted row to reconnect to, or null when this Employee has never been a Manager.
        public Manager ExistingManagerRow { get; set; }

        public int CapturedBy { get; set; }
        public DateTime BusinessDate { get; set; }
    }

    // The outcome of a promotion: the Manager row to write, or the rule that blocked it. A
    // blocked promotion carries no row at all, so nothing can accidentally be written from a
    // refused request.
    public class PromotionResult
    {
        private PromotionResult(Manager manager, string error)
        {
            Manager = manager;
            Error = error;
        }

        public Manager Manager { get; private set; }
        public string Error { get; private set; }

        public bool IsValid
        {
            get { return Error == null; }
        }

        public static PromotionResult Accept(Manager manager)
        {
            return new PromotionResult(manager, null);
        }

        public static PromotionResult Refuse(string error)
        {
            return new PromotionResult(null, error);
        }
    }

    // Everything Demote needs to decide whether a Manager's login may be withdrawn.
    public class DemotionRequest
    {
        // The row being demoted.
        public Manager Manager { get; set; }

        // Identifies the acting Owner's own row, so demoting it can be refused.
        public int ActingOwnerManagerID { get; set; }

        public int CapturedBy { get; set; }
        public DateTime BusinessDate { get; set; }
    }

    // The outcome of a demotion: the Manager row to write, or the rule that blocked it.
    public class DemotionResult
    {
        private DemotionResult(Manager manager, string error)
        {
            Manager = manager;
            Error = error;
        }

        public Manager Manager { get; private set; }
        public string Error { get; private set; }

        public bool IsValid
        {
            get { return Error == null; }
        }

        public static DemotionResult Accept(Manager manager)
        {
            return new DemotionResult(manager, null);
        }

        public static DemotionResult Refuse(string error)
        {
            return new DemotionResult(null, error);
        }
    }

    // Decides who may be promoted, who may be demoted, and what each produces. Holds every
    // rule so the Managers view itself stays a dumb collector, matching EmployeeCapture and
    // LeaveBooking.
    public static class ManagerAccess
    {
        private const int PinLength = 4;

        // The Employees an Owner may promote: at their own store, still employed, and not
        // already an Active Manager. An Employee whose only Manager row is demoted is not
        // excluded, so re-promoting them is always possible.
        public static IEnumerable<Employee> Promotable(IEnumerable<Employee> employees, IEnumerable<Manager> managers, int ownerStoreID)
        {
            var result = new List<Employee>();

            if (employees == null)
            {
                return result;
            }

            foreach (Employee employee in employees)
            {
                if (employee.StoreID != ownerStoreID)
                {
                    continue;
                }

                if (IsTerminated(employee.ContractEndDate, DateTime.Today))
                {
                    continue;
                }

                if (HasActiveManagerRow(managers, employee.EmployeeID))
                {
                    continue;
                }

                result.Add(employee);
            }

            return result;
        }

        // The Managers an Owner may demote: at their own store, active, and not an Owner.
        public static IEnumerable<Manager> Demotable(IEnumerable<Manager> managers, int ownerStoreID)
        {
            var result = new List<Manager>();

            if (managers == null)
            {
                return result;
            }

            foreach (Manager manager in managers)
            {
                if (manager.IsAdmin)
                {
                    continue;
                }

                if (!manager.IsActiveManager)
                {
                    continue;
                }

                if (manager.StoreID != ownerStoreID)
                {
                    continue;
                }

                result.Add(manager);
            }

            return result;
        }

        // Checks every blocking rule in turn, so the first one an Employee fails is the one
        // the Owner sees.
        public static PromotionResult Promote(PromotionRequest request)
        {
            if (request.EmployeeStoreID != request.OwnerStoreID)
            {
                return PromotionResult.Refuse("This Employee does not belong to your store.");
            }

            if (IsTerminated(request.ContractEndDate, DateTime.Today))
            {
                return PromotionResult.Refuse("This Employee's contract has ended.");
            }

            if (HasActiveManagerRow(request.ExistingManagers, request.EmployeeID))
            {
                return PromotionResult.Refuse("This Employee is already an Active Manager.");
            }

            string pin = Trim(request.Pin);

            if (pin.Length != PinLength || !IsAllDigits(pin))
            {
                return PromotionResult.Refuse("A PIN is exactly four digits.");
            }

            if (pin != Trim(request.PinConfirmation))
            {
                return PromotionResult.Refuse("The PIN and its confirmation do not match.");
            }

            int? reconnectingManagerID = request.ExistingManagerRow == null ? (int?)null : request.ExistingManagerRow.ManagerID;
            Manager holder = FindByPin(request.ExistingManagers, pin, reconnectingManagerID);
            if (holder != null)
            {
                return PromotionResult.Refuse(DescribePinClash(holder));
            }

            var manager = new Manager
            {
                ManagerID = request.ExistingManagerRow == null ? 0 : request.ExistingManagerRow.ManagerID,
                EmployeeID = request.EmployeeID,
                StoreID = request.EmployeeStoreID,
                Pin = pin,
                IsActiveManager = true,
                IsAdmin = false,
                CapturedBy = request.CapturedBy,
                BusinessDate = request.BusinessDate
            };

            return PromotionResult.Accept(manager);
        }

        // Refuses to leave a store with nobody to administer it: an Owner cannot be demoted,
        // whether that Owner is somebody else or the acting Owner themselves.
        public static DemotionResult Demote(DemotionRequest request)
        {
            if (request.Manager.ManagerID == request.ActingOwnerManagerID)
            {
                return DemotionResult.Refuse("You cannot demote yourself.");
            }

            if (request.Manager.IsAdmin)
            {
                return DemotionResult.Refuse("An Owner cannot be demoted.");
            }

            var manager = new Manager
            {
                ManagerID = request.Manager.ManagerID,
                EmployeeID = request.Manager.EmployeeID,
                ManagerName = request.Manager.ManagerName,
                StoreID = request.Manager.StoreID,
                Pin = request.Manager.Pin,
                IsActiveManager = false,
                IsAdmin = false,
                CapturedBy = request.CapturedBy,
                BusinessDate = request.BusinessDate
            };

            return DemotionResult.Accept(manager);
        }

        // Names the PIN's current holder to the promoting Owner, who already sees every
        // Employee at their store, so a generic refusal would only leave them guessing at
        // free numbers. An Owner's identity is never disclosed this way.
        private static string DescribePinClash(Manager holder)
        {
            if (holder.IsAdmin)
            {
                return "This PIN is already in use.";
            }

            string status = holder.IsActiveManager ? "a current" : "a former";
            return "This PIN is already held by " + holder.ManagerName + ", " + status + " Manager.";
        }

        // Matches the rule the capture dropdown already applies: a contract with no end date,
        // or one that has not yet ended as of today, still counts as employed.
        private static bool IsTerminated(DateTime? contractEndDate, DateTime asOfDate)
        {
            return contractEndDate.HasValue && contractEndDate.Value.Date < asOfDate.Date;
        }

        private static bool HasActiveManagerRow(IEnumerable<Manager> managers, int employeeID)
        {
            if (managers == null)
            {
                return false;
            }

            foreach (Manager manager in managers)
            {
                if (manager.EmployeeID.HasValue && manager.EmployeeID.Value == employeeID && manager.IsActiveManager)
                {
                    return true;
                }
            }

            return false;
        }

        // Skips the row being reconnected to, if any, so a re-promoted Employee typing back
        // their own former PIN is never told that PIN is already theirs.
        private static Manager FindByPin(IEnumerable<Manager> managers, string pin, int? excludingManagerID)
        {
            if (managers == null)
            {
                return null;
            }

            foreach (Manager manager in managers)
            {
                if (excludingManagerID.HasValue && manager.ManagerID == excludingManagerID.Value)
                {
                    continue;
                }

                if (manager.Pin == pin)
                {
                    return manager;
                }
            }

            return null;
        }

        private static bool IsAllDigits(string text)
        {
            foreach (char character in text)
            {
                if (!char.IsDigit(character))
                {
                    return false;
                }
            }

            return text.Length > 0;
        }

        private static string Trim(string text)
        {
            return text == null ? string.Empty : text.Trim();
        }
    }
}
