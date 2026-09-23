using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeTimeManagement.Models
{
    // Which standing of employee the picker is showing, named for the filter tab a User clicks.
    internal enum EmployeeScope
    {
        Active,
        Former,
        All
    }

    // What the picker shows once a scope and a search have narrowed the store's employees,
    // together with the three counts its filter tabs carry.
    internal class EmployeeListFilterResult
    {
        public List<EmployeeListItem> Visible { get; set; }
        public int ActiveCount { get; set; }
        public int FormerCount { get; set; }
        public int AllCount { get; set; }
    }

    // The picker's filtering rules, kept away from the UI so they can be tested on their own.
    internal static class EmployeeListFilter
    {
        // Narrows a store's employees to one scope and one search, keeping the order they came
        // in, and counts each scope before the search narrows anything, so the tab counts
        // describe the store rather than whatever is being typed.
        public static EmployeeListFilterResult Apply(IEnumerable<EmployeeListItem> employees, EmployeeScope scope, string searchText)
        {
            List<EmployeeListItem> all = employees == null
                ? new List<EmployeeListItem>()
                : employees.ToList();

            int activeCount = all.Count(employee => employee.IsActive);

            IEnumerable<EmployeeListItem> matches = all.Where(employee => IsInScope(employee, scope));

            string search = (searchText ?? string.Empty).Trim();

            if (search.Length > 0)
            {
                matches = matches.Where(employee => Matches(employee, search));
            }

            return new EmployeeListFilterResult
            {
                Visible = matches.ToList(),
                ActiveCount = activeCount,
                FormerCount = all.Count - activeCount,
                AllCount = all.Count
            };
        }

        // Whether an employee belongs to the chosen scope, following the list item's own
        // Active/Former rule so an end date of today already reads as Former.
        private static bool IsInScope(EmployeeListItem employee, EmployeeScope scope)
        {
            switch (scope)
            {
                case EmployeeScope.Active:
                    return employee.IsActive;

                case EmployeeScope.Former:
                    return !employee.IsActive;

                default:
                    return true;
            }
        }

        // Whether any of the details a User might search on carries what they typed.
        private static bool Matches(EmployeeListItem employee, string search)
        {
            return Contains(employee.Name, search)
                || Contains(employee.Surname, search)
                || Contains(employee.FullName, search)
                || Contains(employee.IDNumber, search)
                || Contains(employee.MobileNumber, search)
                || Contains(employee.JobDescription, search);
        }

        // Case-insensitive substring match that treats a missing value as no match.
        private static bool Contains(string value, string search)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return value.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}
