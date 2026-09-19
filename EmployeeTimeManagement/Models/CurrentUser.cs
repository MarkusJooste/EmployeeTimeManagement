using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeManagement.Models
{
    internal class CurrentUser
    {
        public static Manager Manager { get; private set; }
        public static bool IsLoggedIn
        {
            get
            {
                return Manager != null;
            }
        }

        public static bool IsAdmin
        {
            get
            {
                if (Manager == null)
                {
                    return false;
                }
                return Manager.IsAdmin;
            }
        }

        public static int? ManagerID
        {
            get
            {
                if (Manager == null)
                {
                    return null;
                }
                return Manager.ManagerID;
            }
        }

        public static int? StoreID
        {
            get
            {
                if (Manager == null)
                {
                    return null;
                }
                return Manager.StoreID;
            }
        }

        public static void Login(Manager manager)
        {
            Manager = manager;
        }

        public static void Logout()
        {
            Manager = null;
        }
    }
}
