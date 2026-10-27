using EmployeeTimeManagement.Controllers;
using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // Fills in an employee's employment contract on their store's copy of the client's
    // contract PDF, with every blank the system has data for written in, ready to
    // preview, print or save.
    public partial class ContractsForm : Form
    {
        private readonly ContractController contractController;

        private List<ContractListItem> allEmployees = new List<ContractListItem>();
        private ContractSource selected;

        // Set while the dropdowns are being filled, so their change events do not rebuild
        // the summary once per item.
        private bool populating;

        public ContractsForm()
        {
            InitializeComponent();
            contractController = new ContractController();

            cboTemplate.Items.Add(new TemplateChoice(ContractTemplate.General));
            cboTemplate.Items.Add(new TemplateChoice(ContractTemplate.Manager));

            ShowSelection(null);
            LoadEmployees();
        }

        // The store whose employees this user may see. Managers see their own store only;
        // the owner (admin) sees every store.
        private static int? VisibleStoreID
        {
            get { return CurrentUser.IsAdmin ? null : CurrentUser.StoreID; }
        }

        private static bool MaySeeEmployees
        {
            get { return CurrentUser.IsAdmin || CurrentUser.StoreID.HasValue; }
        }

        // ------------------------------------------------------------------
        // Loading
        // ------------------------------------------------------------------

        private void LoadEmployees()
        {
            if (!MaySeeEmployees)
            {
                lblStatus.Text = "No store assigned to this user";
                return;
            }

            try
            {
                allEmployees = contractController.GetEmployees(VisibleStoreID);
            }
            catch (Exception ex)
            {
                allEmployees = new List<ContractListItem>();
                lblStatus.Text = "Could not load employees: " + ex.Message;
                return;
            }

            // Only the owner sees more than one store, so only they need the Store column.
            colStore.Visible = CurrentUser.IsAdmin;
            ApplyFilter();
        }

        // Narrows the list by the search text without going back to the database.
        private void ApplyFilter()
        {
            string search = txtSearch.Text.Trim();
            List<ContractListItem> rows = allEmployees;

            if (search.Length > 0)
            {
                rows = allEmployees
                    .Where(item => Contains(item.Name, search) || Contains(item.Surname, search)
                        || Contains((item.Name + " " + item.Surname), search))
                    .ToList();
            }

            dgvEmployees.DataSource = rows;
            lblStatus.Text = rows.Count == 1 ? "1 employee" : rows.Count + " employees";

            LoadSelectedEmployee();
        }

        private static bool Contains(string value, string search)
        {
            return value != null && value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // Reads everything the contract needs for the highlighted employee.
        private void LoadSelectedEmployee()
        {
            ContractListItem item = dgvEmployees.CurrentRow == null
                ? null
                : dgvEmployees.CurrentRow.DataBoundItem as ContractListItem;

            if (item == null)
            {
                ShowSelection(null);
                return;
            }

            if (selected != null && selected.Employee.EmployeeID == item.EmployeeID)
            {
                return;
            }

            ContractSource source;
            try
            {
                source = contractController.GetContractSource(item.EmployeeID, VisibleStoreID);
            }
            catch (Exception ex)
            {
                ShowSelection(null);
                lblStatus.Text = "Could not load " + item.Name + " " + item.Surname + ": " + ex.Message;
                return;
            }

            if (source == null)
            {
                ShowSelection(null);
                lblStatus.Text = "That employee could not be found.";
                return;
            }

            ShowSelection(source);
        }

        // Fills the dropdowns for an employee (or clears them) and refreshes the summary.
        private void ShowSelection(ContractSource source)
        {
            selected = source;
            populating = true;

            try
            {
                cboNextOfKin.Items.Clear();

                if (source == null)
                {
                    cboTemplate.SelectedIndex = -1;
                }
                else
                {
                    foreach (NextOfKinOption option in source.NextOfKinOptions())
                    {
                        cboNextOfKin.Items.Add(option);
                    }

                    cboNextOfKin.SelectedIndex = 0;
                    SelectTemplate(source.SuggestedTemplate());
                }
            }
            finally
            {
                populating = false;
            }

            bool hasSelection = source != null;
            cboTemplate.Enabled = hasSelection;
            cboNextOfKin.Enabled = hasSelection;
            btnPreview.Enabled = hasSelection;
            btnPrint.Enabled = hasSelection;
            btnSavePdf.Enabled = hasSelection;

            RefreshSummary();
        }

        private void SelectTemplate(ContractTemplate template)
        {
            for (int index = 0; index < cboTemplate.Items.Count; index++)
            {
                if (((TemplateChoice)cboTemplate.Items[index]).Template == template)
                {
                    cboTemplate.SelectedIndex = index;
                    return;
                }
            }
        }


        // Building the contract

        private ContractData BuildData()
        {
            var templateChoice = cboTemplate.SelectedItem as TemplateChoice;
            ContractTemplate template = templateChoice == null ? selected.SuggestedTemplate() : templateChoice.Template;
            var nextOfKin = cboNextOfKin.SelectedItem as NextOfKinOption;

            return selected.ToContractData(nextOfKin, template, DateTime.Today);
        }

        // Shows what each blank will contain, and lists the blanks that will be left for
        // handwriting, so gaps in the employee's record are visible before printing.
        private void RefreshSummary()
        {
            if (selected == null)
            {
                txtSummary.Text = "Select an employee.";
                return;
            }

            ContractData data = BuildData();
            string templatePath = ContractTemplateFiles.DisplayPath(data.StoreID, data.Template);

            var text = new StringBuilder();
            AppendLine(text, "Contract PDF", File.Exists(ContractTemplateFiles.PathFor(data.StoreID, data.Template))
                ? templatePath
                : templatePath + " - MISSING, add this store's PDF there");
            AppendLine(text, "Name", data.FullName);
            AppendLine(text, "ID number", data.IDNumber);
            AppendLine(text, "Address", JoinLines(data.AddressLine1, data.AddressLine2));
            AppendLine(text, "Tel. no", data.TelephoneNumber);
            AppendLine(text, "Next of kin", data.NextOfKinName);
            AppendLine(text, "Relationship", data.NextOfKinRelationship);
            AppendLine(text, "Contact no.", data.NextOfKinNumber);
            AppendLine(text, "Commencement", ContractFormat.LongDate(data.CommencementDate));
            AppendLine(text, "Position", data.Template == ContractTemplate.Manager ? "Manager" : data.Position);
            AppendLine(text, "Hourly rate", ContractFormat.Rand(data.HourlyRate));

            if (data.Template == ContractTemplate.Manager && data.HourlyRate.HasValue)
            {
                AppendLine(text, "Monthly salary", ContractFormat.Rand(ContractFormat.MonthlySalary(data.HourlyRate.Value)));
            }

            AppendLine(text, "Store", data.StoreName);
            AppendLine(text, "Signing date", ContractFormat.LongDate(data.SigningDate));

            List<string> missing = ContractBlanks.MissingFields(data);

            text.AppendLine();
            if (missing.Count == 0)
            {
                text.AppendLine("Every blank will be filled in.");
            }
            else
            {
                text.AppendLine("Left blank to fill in by hand:");
                foreach (string field in missing)
                {
                    text.AppendLine("  - " + field);
                }
            }

            txtSummary.Text = text.ToString();
        }

        private static void AppendLine(StringBuilder text, string label, string value)
        {
            text.AppendLine(label + ": " + (string.IsNullOrWhiteSpace(value) ? "(blank)" : value));
        }

        private static string JoinLines(string first, string second)
        {
            if (string.IsNullOrWhiteSpace(first))
            {
                return second;
            }

            if (string.IsNullOrWhiteSpace(second))
            {
                return first;
            }

            return first + ", " + second;
        }

        // Output

        // Fills in the selected employee's contract. Returns null, having told the user why,
        // when the store has no PDF for this contract or it could not be filled in.
        private ContractPdf CreateContract(out ContractData data)
        {
            data = BuildData();
            string path = ContractTemplateFiles.PathFor(data.StoreID, data.Template);

            if (!File.Exists(path))
            {
                MessageBox.Show(
                    "There is no " + data.Template.DisplayName().ToLowerInvariant() + " contract PDF for "
                    + (data.StoreName ?? "store " + data.StoreID) + " yet.\n\n"
                    + "Save the store's contract PDF as:\n" + path,
                    "Contracts", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            ContractPdf contract;
            try
            {
                Cursor = Cursors.WaitCursor;
                contract = ContractPdf.Create(path, data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not fill in the contract: " + ex.Message, "Contracts", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            if (contract.Plan.NotFound.Count > 0)
            {
                MessageBox.Show(
                    "These blanks could not be found in " + ContractTemplateFiles.DisplayPath(data.StoreID, data.Template)
                    + ", so they were not filled in:\n\n  - " + string.Join("\n  - ", contract.Plan.NotFound.Distinct())
                    + "\n\nCheck that the PDF is the client's contract, or fill these in by hand.",
                    "Contracts", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return contract;
        }

        private static string ContractTitle(ContractData data)
        {
            return data.Template.DisplayName() + " contract - " + (data.FullName ?? "employee");
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (selected == null)
            {
                return;
            }

            ContractData data;
            ContractPdf contract = CreateContract(out data);
            if (contract == null)
            {
                return;
            }

            try
            {
                using (var preview = new ContractPreviewForm(contract.Bytes, ContractTitle(data)))
                {
                    preview.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not preview the contract: " + ex.Message, "Contracts", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (selected == null)
            {
                return;
            }

            ContractData data;
            ContractPdf contract = CreateContract(out data);
            if (contract == null)
            {
                return;
            }

            try
            {
                using (var pdf = PdfiumViewer.PdfDocument.Load(new MemoryStream(contract.Bytes)))
                using (PrintDocument printDocument = pdf.CreatePrintDocument())
                using (var dialog = new PrintDialog())
                {
                    printDocument.DocumentName = ContractTitle(data);
                    dialog.Document = printDocument;
                    dialog.UseEXDialog = true;
                    dialog.AllowSomePages = true;

                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    printDocument.Print();
                    lblStatus.Text = "Sent to " + printDocument.PrinterSettings.PrinterName;
                }
            }
            catch (InvalidPrinterException)
            {
                MessageBox.Show(
                    "No printer is installed on this computer. Install a printer and try again, "
                    + "or use Save as PDF... instead.",
                    "Contracts", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not print the contract: " + ex.Message, "Contracts", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSavePdf_Click(object sender, EventArgs e)
        {
            if (selected == null)
            {
                return;
            }

            ContractData data;
            ContractPdf contract = CreateContract(out data);
            if (contract == null)
            {
                return;
            }

            string path;
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "PDF files (*.pdf)|*.pdf";
                dialog.DefaultExt = "pdf";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;
                dialog.FileName = SafeFileName(ContractTitle(data)) + ".pdf";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                path = dialog.FileName;
            }

            try
            {
                File.WriteAllBytes(path, contract.Bytes);
                lblStatus.Text = "Saved " + Path.GetFileName(path) + " to " + Path.GetDirectoryName(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save the PDF: " + ex.Message, "Save as PDF", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Replaces characters Windows does not allow in file names.
        private static string SafeFileName(string name)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            var safe = new StringBuilder(name.Length);

            foreach (char character in name)
            {
                safe.Append(Array.IndexOf(invalid, character) >= 0 ? '_' : character);
            }

            return safe.ToString();
        }

        // Events

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            LoadSelectedEmployee();
        }

        // The grid only picks its first row once it is on screen, which can be after the
        // list was bound, so load the highlighted employee again when binding finishes.
        private void dgvEmployees_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            LoadSelectedEmployee();
        }

        private void cboTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!populating)
            {
                RefreshSummary();
            }
        }

        private void cboNextOfKin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!populating)
            {
                RefreshSummary();
            }
        }

        // A template as shown in the dropdown.
        private class TemplateChoice
        {
            public TemplateChoice(ContractTemplate template)
            {
                Template = template;
            }

            public ContractTemplate Template { get; private set; }

            public override string ToString()
            {
                return Template.DisplayName();
            }
        }
    }
}
