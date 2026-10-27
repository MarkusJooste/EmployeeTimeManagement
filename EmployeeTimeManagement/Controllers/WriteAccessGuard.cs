using System;
using EmployeeTimeManagement.Models;

namespace EmployeeTimeManagement.Controllers
{
    // Thrown by WriteAccessGuard.EnsureActive when the write is refused because the logged-in User's login is no longer active.
    internal class AccessRevokedException : Exception
    {
        public AccessRevokedException() : base("Your access has been removed.")
        {
        }
    }

    // The one check every write goes through before it touches the database: whether the logged-in User's login is still active.
    internal static class WriteAccessGuard
    {
        public static void EnsureActive()
        {
            int? managerID = CurrentUser.ManagerID;

            if (managerID == null || !new ManagerController().IsActive(managerID.Value))
            {
                throw new AccessRevokedException();
            }
        }
    }
}
