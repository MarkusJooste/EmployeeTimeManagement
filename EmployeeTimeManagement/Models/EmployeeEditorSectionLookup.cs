using System;
using System.Collections.Generic;

namespace EmployeeTimeManagement.Models
{
    // The editor's sections, in the order they sit on the section list. A refused save always
    // walks them in this order, never in the order the capture seam happened to report errors.
    public enum EmployeeEditorSection
    {
        Personal,
        Address,
        Bank,
        Contract,
        Spouse,
        Family
    }

    // Maps a capture seam field error to the editor section that holds it, so a refused save
    // can mark every section with a problem and land on the first one, in section-list order.
    // Kept away from the UI so the mapping can be tested without a form.
    public static class EmployeeEditorSectionLookup
    {
        private static readonly Dictionary<string, EmployeeEditorSection> FieldSections = new Dictionary<string, EmployeeEditorSection>
        {
            { "Name", EmployeeEditorSection.Personal },
            { "Surname", EmployeeEditorSection.Personal },
            { "IDNumber", EmployeeEditorSection.Personal },
            { "SARSNumber", EmployeeEditorSection.Personal },
            { "MobileNumber", EmployeeEditorSection.Personal },
            { "MaritalStatus", EmployeeEditorSection.Personal },
            { "NumberOfDependents", EmployeeEditorSection.Personal },

            { "HouseFlatNumber", EmployeeEditorSection.Address },
            { "ComplexFlatNumber", EmployeeEditorSection.Address },
            { "StreetName", EmployeeEditorSection.Address },
            { "Town", EmployeeEditorSection.Address },
            { "PostalCode", EmployeeEditorSection.Address },

            { "BankName", EmployeeEditorSection.Bank },
            { "AccountType", EmployeeEditorSection.Bank },
            { "AccountNumber", EmployeeEditorSection.Bank },
            { "BranchCode", EmployeeEditorSection.Bank },

            { "ContractType", EmployeeEditorSection.Contract },
            { "StartDate", EmployeeEditorSection.Contract },
            { "Department", EmployeeEditorSection.Contract },
            { "JobDescription", EmployeeEditorSection.Contract },
            { "HourlyRate", EmployeeEditorSection.Contract },
            { "OpeningPTODays", EmployeeEditorSection.Contract },

            { "SpouseName", EmployeeEditorSection.Spouse },
            { "SpouseMobileNumber", EmployeeEditorSection.Spouse },

            { "FamilyMembers", EmployeeEditorSection.Family }
        };

        // The one section a single error belongs to. A family row error is always Family
        // contacts, whatever its field name; anything else falls back to the field-name table,
        // and a field the table does not know maps to no section at all.
        public static EmployeeEditorSection? SectionFor(EmployeeFieldError error)
        {
            if (error.FamilyRowIndex.HasValue)
            {
                return EmployeeEditorSection.Family;
            }

            EmployeeEditorSection section;
            return FieldSections.TryGetValue(error.Field, out section) ? section : (EmployeeEditorSection?)null;
        }

        // Every section that holds at least one error, in section-list order rather than the
        // order the errors arrived in, so the section list can mark all of them at once.
        public static List<EmployeeEditorSection> SectionsWithErrors(IEnumerable<EmployeeFieldError> errors)
        {
            var found = new HashSet<EmployeeEditorSection>();

            foreach (EmployeeFieldError error in errors)
            {
                EmployeeEditorSection? section = SectionFor(error);

                if (section.HasValue)
                {
                    found.Add(section.Value);
                }
            }

            var ordered = new List<EmployeeEditorSection>();

            foreach (EmployeeEditorSection section in AllSections())
            {
                if (found.Contains(section))
                {
                    ordered.Add(section);
                }
            }

            return ordered;
        }

        // The first section with an error, in section-list order, or null when nothing errored.
        public static EmployeeEditorSection? FirstSectionWithErrors(IEnumerable<EmployeeFieldError> errors)
        {
            List<EmployeeEditorSection> sections = SectionsWithErrors(errors);
            return sections.Count == 0 ? (EmployeeEditorSection?)null : sections[0];
        }

        // Every section, in the order the section list shows them. The enum's declaration
        // order is that order, so there is nothing here for a new section to fall out of sync with.
        private static IEnumerable<EmployeeEditorSection> AllSections()
        {
            return (EmployeeEditorSection[])Enum.GetValues(typeof(EmployeeEditorSection));
        }
    }
}
