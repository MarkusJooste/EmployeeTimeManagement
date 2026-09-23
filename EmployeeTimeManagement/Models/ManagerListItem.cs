using System;

namespace EmployeeTimeManagement.Models
{
    // One row of the Managers list's right-hand side: an Active Manager as the Managers view
    // shows them, with the linked Employee's name and the promoting User's name resolved for
    // display. A Manager row itself carries neither.
    internal class ManagerListItem
    {
        public int ManagerID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Pin { get; set; }

        // Null for a Manager this app never promoted or demoted: an existing login the
        // migration carried forward without an audit trail to show.
        public DateTime? PromotedOn { get; set; }

        public string PromotedByName { get; set; }
    }
}
