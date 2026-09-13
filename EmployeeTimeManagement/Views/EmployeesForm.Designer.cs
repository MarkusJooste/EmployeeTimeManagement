namespace EmployeeTimeManagement.Views
{
    partial class EmployeesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlList = new System.Windows.Forms.Panel();
            this.dgvEmployees = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSurname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIDNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMobileNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colJobDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlListTop = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnTerminate = new System.Windows.Forms.Button();
            this.btnReactivate = new System.Windows.Forms.Button();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.chkShowFormer = new System.Windows.Forms.CheckBox();
            this.pnlEditor = new System.Windows.Forms.Panel();
            this.tlpEditor = new System.Windows.Forms.TableLayoutPanel();
            this.grpPersonal = new System.Windows.Forms.GroupBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblSurname = new System.Windows.Forms.Label();
            this.txtSurname = new System.Windows.Forms.TextBox();
            this.lblIDNumber = new System.Windows.Forms.Label();
            this.txtIDNumber = new System.Windows.Forms.TextBox();
            this.lblSARSNumber = new System.Windows.Forms.Label();
            this.txtSARSNumber = new System.Windows.Forms.TextBox();
            this.lblMobileNumber = new System.Windows.Forms.Label();
            this.txtMobileNumber = new System.Windows.Forms.TextBox();
            this.lblMaritalStatus = new System.Windows.Forms.Label();
            this.cboMaritalStatus = new System.Windows.Forms.ComboBox();
            this.lblDependents = new System.Windows.Forms.Label();
            this.txtDependents = new System.Windows.Forms.TextBox();
            this.tlpAddressBank = new System.Windows.Forms.TableLayoutPanel();
            this.grpAddress = new System.Windows.Forms.GroupBox();
            this.lblHouseFlatNumber = new System.Windows.Forms.Label();
            this.txtHouseFlatNumber = new System.Windows.Forms.TextBox();
            this.lblComplexFlatNumber = new System.Windows.Forms.Label();
            this.txtComplexFlatNumber = new System.Windows.Forms.TextBox();
            this.lblStreetName = new System.Windows.Forms.Label();
            this.txtStreetName = new System.Windows.Forms.TextBox();
            this.lblTown = new System.Windows.Forms.Label();
            this.txtTown = new System.Windows.Forms.TextBox();
            this.lblPostalCode = new System.Windows.Forms.Label();
            this.txtPostalCode = new System.Windows.Forms.TextBox();
            this.grpBank = new System.Windows.Forms.GroupBox();
            this.lblBankName = new System.Windows.Forms.Label();
            this.txtBankName = new System.Windows.Forms.TextBox();
            this.lblAccountType = new System.Windows.Forms.Label();
            this.txtAccountType = new System.Windows.Forms.TextBox();
            this.lblAccountNumber = new System.Windows.Forms.Label();
            this.txtAccountNumber = new System.Windows.Forms.TextBox();
            this.lblBranchCode = new System.Windows.Forms.Label();
            this.txtBranchCode = new System.Windows.Forms.TextBox();
            this.tlpContractSpouse = new System.Windows.Forms.TableLayoutPanel();
            this.grpContract = new System.Windows.Forms.GroupBox();
            this.lblContractType = new System.Windows.Forms.Label();
            this.txtContractType = new System.Windows.Forms.TextBox();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblDepartment = new System.Windows.Forms.Label();
            this.txtDepartment = new System.Windows.Forms.TextBox();
            this.lblJobDescription = new System.Windows.Forms.Label();
            this.txtJobDescription = new System.Windows.Forms.TextBox();
            this.lblHourlyRate = new System.Windows.Forms.Label();
            this.txtHourlyRate = new System.Windows.Forms.TextBox();
            this.grpSpouse = new System.Windows.Forms.GroupBox();
            this.lblSpouseName = new System.Windows.Forms.Label();
            this.txtSpouseName = new System.Windows.Forms.TextBox();
            this.lblSpouseMobileNumber = new System.Windows.Forms.Label();
            this.txtSpouseMobileNumber = new System.Windows.Forms.TextBox();
            this.grpFamily = new System.Windows.Forms.GroupBox();
            this.dgvFamily = new System.Windows.Forms.DataGridView();
            this.colFamilyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFamilyMobileNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFamilyRelationship = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFamilyButtons = new System.Windows.Forms.Panel();
            this.btnAddFamilyRow = new System.Windows.Forms.Button();
            this.btnRemoveFamilyRow = new System.Windows.Forms.Button();
            this.pnlEditorButtons = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblEditorTitle = new System.Windows.Forms.Label();
            this.lblEditorStatus = new System.Windows.Forms.Label();
            this.tipEditorErrors = new System.Windows.Forms.ToolTip(this.components);
            this.pnlList.SuspendLayout();
            this.pnlListTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
            this.pnlEditor.SuspendLayout();
            this.tlpEditor.SuspendLayout();
            this.grpPersonal.SuspendLayout();
            this.tlpAddressBank.SuspendLayout();
            this.grpAddress.SuspendLayout();
            this.grpBank.SuspendLayout();
            this.tlpContractSpouse.SuspendLayout();
            this.grpContract.SuspendLayout();
            this.grpSpouse.SuspendLayout();
            this.grpFamily.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamily)).BeginInit();
            this.pnlFamilyButtons.SuspendLayout();
            this.pnlEditorButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlList
            //
            this.pnlList.Controls.Add(this.dgvEmployees);
            this.pnlList.Controls.Add(this.lblStatus);
            this.pnlList.Controls.Add(this.pnlListTop);
            this.pnlList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlList.Location = new System.Drawing.Point(0, 0);
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new System.Drawing.Size(889, 574);
            this.pnlList.TabIndex = 0;
            //
            // dgvEmployees
            //
            this.dgvEmployees.AllowUserToAddRows = false;
            this.dgvEmployees.AllowUserToDeleteRows = false;
            this.dgvEmployees.AutoGenerateColumns = false;
            this.dgvEmployees.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEmployees.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEmployees.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colSurname,
            this.colIDNumber,
            this.colMobileNumber,
            this.colJobDescription,
            this.colStatus});
            this.dgvEmployees.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmployees.Location = new System.Drawing.Point(0, 80);
            this.dgvEmployees.MultiSelect = false;
            this.dgvEmployees.Name = "dgvEmployees";
            this.dgvEmployees.ReadOnly = true;
            this.dgvEmployees.RowHeadersVisible = false;
            this.dgvEmployees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployees.Size = new System.Drawing.Size(889, 471);
            this.dgvEmployees.TabIndex = 1;
            this.dgvEmployees.SelectionChanged += new System.EventHandler(this.dgvEmployees_SelectionChanged);
            //
            // colName
            //
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colSurname
            //
            this.colSurname.DataPropertyName = "Surname";
            this.colSurname.HeaderText = "Surname";
            this.colSurname.Name = "colSurname";
            this.colSurname.ReadOnly = true;
            this.colSurname.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colIDNumber
            //
            this.colIDNumber.DataPropertyName = "IDNumber";
            this.colIDNumber.HeaderText = "ID Number";
            this.colIDNumber.Name = "colIDNumber";
            this.colIDNumber.ReadOnly = true;
            this.colIDNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colMobileNumber
            //
            this.colMobileNumber.DataPropertyName = "MobileNumber";
            this.colMobileNumber.HeaderText = "Mobile";
            this.colMobileNumber.Name = "colMobileNumber";
            this.colMobileNumber.ReadOnly = true;
            this.colMobileNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colJobDescription
            //
            this.colJobDescription.DataPropertyName = "JobDescription";
            this.colJobDescription.HeaderText = "Job Description";
            this.colJobDescription.Name = "colJobDescription";
            this.colJobDescription.ReadOnly = true;
            this.colJobDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // lblStatus
            //
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Location = new System.Drawing.Point(0, 551);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(889, 23);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "lblStatus";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlListTop
            //
            this.pnlListTop.Controls.Add(this.chkShowFormer);
            this.pnlListTop.Controls.Add(this.txtSearch);
            this.pnlListTop.Controls.Add(this.lblSearch);
            this.pnlListTop.Controls.Add(this.btnReactivate);
            this.pnlListTop.Controls.Add(this.btnTerminate);
            this.pnlListTop.Controls.Add(this.btnUpdate);
            this.pnlListTop.Controls.Add(this.btnAdd);
            this.pnlListTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlListTop.Location = new System.Drawing.Point(0, 0);
            this.pnlListTop.Name = "pnlListTop";
            this.pnlListTop.Size = new System.Drawing.Size(889, 80);
            this.pnlListTop.TabIndex = 0;
            //
            // btnAdd
            //
            this.btnAdd.Location = new System.Drawing.Point(12, 9);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 25);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add Employee";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnUpdate
            //
            this.btnUpdate.Location = new System.Drawing.Point(118, 9);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 25);
            this.btnUpdate.TabIndex = 1;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            //
            // btnTerminate
            //
            this.btnTerminate.Location = new System.Drawing.Point(199, 9);
            this.btnTerminate.Name = "btnTerminate";
            this.btnTerminate.Size = new System.Drawing.Size(75, 25);
            this.btnTerminate.TabIndex = 2;
            this.btnTerminate.Text = "Terminate";
            this.btnTerminate.UseVisualStyleBackColor = true;
            //
            // btnReactivate
            //
            this.btnReactivate.Location = new System.Drawing.Point(280, 9);
            this.btnReactivate.Name = "btnReactivate";
            this.btnReactivate.Size = new System.Drawing.Size(75, 25);
            this.btnReactivate.TabIndex = 3;
            this.btnReactivate.Text = "Reactivate";
            this.btnReactivate.UseVisualStyleBackColor = true;
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(12, 51);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(87, 13);
            this.lblSearch.TabIndex = 4;
            this.lblSearch.Text = "Search employee";
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(105, 47);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(180, 20);
            this.txtSearch.TabIndex = 5;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            //
            // chkShowFormer
            //
            this.chkShowFormer.AutoSize = true;
            this.chkShowFormer.Location = new System.Drawing.Point(300, 49);
            this.chkShowFormer.Name = "chkShowFormer";
            this.chkShowFormer.Size = new System.Drawing.Size(139, 17);
            this.chkShowFormer.TabIndex = 6;
            this.chkShowFormer.Text = "Show former employees";
            this.chkShowFormer.UseVisualStyleBackColor = true;
            this.chkShowFormer.CheckedChanged += new System.EventHandler(this.chkShowFormer_CheckedChanged);
            //
            // pnlEditor
            //
            this.pnlEditor.Controls.Add(this.tlpEditor);
            this.pnlEditor.Controls.Add(this.pnlEditorButtons);
            this.pnlEditor.Controls.Add(this.lblEditorTitle);
            this.pnlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEditor.Location = new System.Drawing.Point(0, 0);
            this.pnlEditor.Name = "pnlEditor";
            this.pnlEditor.Size = new System.Drawing.Size(889, 574);
            this.pnlEditor.TabIndex = 1;
            this.pnlEditor.Visible = false;
            //
            // lblEditorTitle
            //
            this.lblEditorTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblEditorTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblEditorTitle.Location = new System.Drawing.Point(0, 0);
            this.lblEditorTitle.Name = "lblEditorTitle";
            this.lblEditorTitle.Padding = new System.Windows.Forms.Padding(9, 0, 0, 0);
            this.lblEditorTitle.Size = new System.Drawing.Size(889, 30);
            this.lblEditorTitle.TabIndex = 0;
            this.lblEditorTitle.Text = "Add Employee";
            this.lblEditorTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tlpEditor
            //
            this.tlpEditor.ColumnCount = 3;
            this.tlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpEditor.Controls.Add(this.grpPersonal, 0, 0);
            this.tlpEditor.Controls.Add(this.tlpAddressBank, 1, 0);
            this.tlpEditor.Controls.Add(this.tlpContractSpouse, 2, 0);
            this.tlpEditor.Controls.Add(this.grpFamily, 0, 1);
            this.tlpEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpEditor.Location = new System.Drawing.Point(0, 30);
            this.tlpEditor.Name = "tlpEditor";
            this.tlpEditor.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.tlpEditor.RowCount = 2;
            this.tlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.tlpEditor.SetColumnSpan(this.grpFamily, 3);
            this.tlpEditor.Size = new System.Drawing.Size(889, 500);
            this.tlpEditor.TabIndex = 1;
            //
            // grpPersonal
            //
            this.grpPersonal.Controls.Add(this.lblName);
            this.grpPersonal.Controls.Add(this.txtName);
            this.grpPersonal.Controls.Add(this.lblSurname);
            this.grpPersonal.Controls.Add(this.txtSurname);
            this.grpPersonal.Controls.Add(this.lblIDNumber);
            this.grpPersonal.Controls.Add(this.txtIDNumber);
            this.grpPersonal.Controls.Add(this.lblSARSNumber);
            this.grpPersonal.Controls.Add(this.txtSARSNumber);
            this.grpPersonal.Controls.Add(this.lblMobileNumber);
            this.grpPersonal.Controls.Add(this.txtMobileNumber);
            this.grpPersonal.Controls.Add(this.lblMaritalStatus);
            this.grpPersonal.Controls.Add(this.cboMaritalStatus);
            this.grpPersonal.Controls.Add(this.lblDependents);
            this.grpPersonal.Controls.Add(this.txtDependents);
            this.grpPersonal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPersonal.Location = new System.Drawing.Point(9, 3);
            this.grpPersonal.Name = "grpPersonal";
            this.grpPersonal.Size = new System.Drawing.Size(286, 324);
            this.grpPersonal.TabIndex = 0;
            this.grpPersonal.TabStop = false;
            this.grpPersonal.Text = "Personal";
            //
            // lblName
            //
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(10, 28);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            //
            // txtName
            //
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(115, 25);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(160, 20);
            this.txtName.TabIndex = 1;
            //
            // lblSurname
            //
            this.lblSurname.AutoSize = true;
            this.lblSurname.Location = new System.Drawing.Point(10, 57);
            this.lblSurname.Name = "lblSurname";
            this.lblSurname.Size = new System.Drawing.Size(49, 13);
            this.lblSurname.TabIndex = 2;
            this.lblSurname.Text = "Surname";
            //
            // txtSurname
            //
            this.txtSurname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSurname.Location = new System.Drawing.Point(115, 54);
            this.txtSurname.Name = "txtSurname";
            this.txtSurname.Size = new System.Drawing.Size(160, 20);
            this.txtSurname.TabIndex = 3;
            //
            // lblIDNumber
            //
            this.lblIDNumber.AutoSize = true;
            this.lblIDNumber.Location = new System.Drawing.Point(10, 86);
            this.lblIDNumber.Name = "lblIDNumber";
            this.lblIDNumber.Size = new System.Drawing.Size(57, 13);
            this.lblIDNumber.TabIndex = 4;
            this.lblIDNumber.Text = "ID Number";
            //
            // txtIDNumber
            //
            this.txtIDNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIDNumber.Location = new System.Drawing.Point(115, 83);
            this.txtIDNumber.MaxLength = 13;
            this.txtIDNumber.Name = "txtIDNumber";
            this.txtIDNumber.Size = new System.Drawing.Size(160, 20);
            this.txtIDNumber.TabIndex = 5;
            //
            // lblSARSNumber
            //
            this.lblSARSNumber.AutoSize = true;
            this.lblSARSNumber.Location = new System.Drawing.Point(10, 115);
            this.lblSARSNumber.Name = "lblSARSNumber";
            this.lblSARSNumber.Size = new System.Drawing.Size(101, 13);
            this.lblSARSNumber.TabIndex = 6;
            this.lblSARSNumber.Text = "SARS Number";
            //
            // txtSARSNumber
            //
            this.txtSARSNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSARSNumber.Location = new System.Drawing.Point(115, 112);
            this.txtSARSNumber.MaxLength = 20;
            this.txtSARSNumber.Name = "txtSARSNumber";
            this.txtSARSNumber.Size = new System.Drawing.Size(160, 20);
            this.txtSARSNumber.TabIndex = 7;
            //
            // lblMobileNumber
            //
            this.lblMobileNumber.AutoSize = true;
            this.lblMobileNumber.Location = new System.Drawing.Point(10, 144);
            this.lblMobileNumber.Name = "lblMobileNumber";
            this.lblMobileNumber.Size = new System.Drawing.Size(76, 13);
            this.lblMobileNumber.TabIndex = 8;
            this.lblMobileNumber.Text = "Mobile Number";
            //
            // txtMobileNumber
            //
            this.txtMobileNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMobileNumber.Location = new System.Drawing.Point(115, 141);
            this.txtMobileNumber.MaxLength = 20;
            this.txtMobileNumber.Name = "txtMobileNumber";
            this.txtMobileNumber.Size = new System.Drawing.Size(160, 20);
            this.txtMobileNumber.TabIndex = 9;
            //
            // lblMaritalStatus
            //
            this.lblMaritalStatus.AutoSize = true;
            this.lblMaritalStatus.Location = new System.Drawing.Point(10, 173);
            this.lblMaritalStatus.Name = "lblMaritalStatus";
            this.lblMaritalStatus.Size = new System.Drawing.Size(71, 13);
            this.lblMaritalStatus.TabIndex = 10;
            this.lblMaritalStatus.Text = "Marital Status";
            //
            // cboMaritalStatus
            //
            this.cboMaritalStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboMaritalStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMaritalStatus.FormattingEnabled = true;
            this.cboMaritalStatus.Location = new System.Drawing.Point(115, 170);
            this.cboMaritalStatus.Name = "cboMaritalStatus";
            this.cboMaritalStatus.Size = new System.Drawing.Size(160, 21);
            this.cboMaritalStatus.TabIndex = 11;
            this.cboMaritalStatus.SelectedIndexChanged += new System.EventHandler(this.cboMaritalStatus_SelectedIndexChanged);
            //
            // lblDependents
            //
            this.lblDependents.AutoSize = true;
            this.lblDependents.Location = new System.Drawing.Point(10, 202);
            this.lblDependents.Name = "lblDependents";
            this.lblDependents.Size = new System.Drawing.Size(103, 13);
            this.lblDependents.TabIndex = 12;
            this.lblDependents.Text = "Dependents";
            //
            // txtDependents
            //
            this.txtDependents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDependents.Location = new System.Drawing.Point(115, 199);
            this.txtDependents.Name = "txtDependents";
            this.txtDependents.Size = new System.Drawing.Size(160, 20);
            this.txtDependents.TabIndex = 13;
            //
            // tlpAddressBank
            //
            this.tlpAddressBank.ColumnCount = 1;
            this.tlpAddressBank.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAddressBank.Controls.Add(this.grpAddress, 0, 0);
            this.tlpAddressBank.Controls.Add(this.grpBank, 0, 1);
            this.tlpAddressBank.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAddressBank.Location = new System.Drawing.Point(301, 3);
            this.tlpAddressBank.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tlpAddressBank.Name = "tlpAddressBank";
            this.tlpAddressBank.RowCount = 2;
            this.tlpAddressBank.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 176F));
            this.tlpAddressBank.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAddressBank.Size = new System.Drawing.Size(286, 324);
            this.tlpAddressBank.TabIndex = 1;
            //
            // grpAddress
            //
            this.grpAddress.Controls.Add(this.lblHouseFlatNumber);
            this.grpAddress.Controls.Add(this.txtHouseFlatNumber);
            this.grpAddress.Controls.Add(this.lblComplexFlatNumber);
            this.grpAddress.Controls.Add(this.txtComplexFlatNumber);
            this.grpAddress.Controls.Add(this.lblStreetName);
            this.grpAddress.Controls.Add(this.txtStreetName);
            this.grpAddress.Controls.Add(this.lblTown);
            this.grpAddress.Controls.Add(this.txtTown);
            this.grpAddress.Controls.Add(this.lblPostalCode);
            this.grpAddress.Controls.Add(this.txtPostalCode);
            this.grpAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAddress.Location = new System.Drawing.Point(3, 3);
            this.grpAddress.Name = "grpAddress";
            this.grpAddress.Size = new System.Drawing.Size(280, 170);
            this.grpAddress.TabIndex = 0;
            this.grpAddress.TabStop = false;
            this.grpAddress.Text = "Address";
            //
            // lblHouseFlatNumber
            //
            this.lblHouseFlatNumber.AutoSize = true;
            this.lblHouseFlatNumber.Location = new System.Drawing.Point(10, 28);
            this.lblHouseFlatNumber.Name = "lblHouseFlatNumber";
            this.lblHouseFlatNumber.Size = new System.Drawing.Size(95, 13);
            this.lblHouseFlatNumber.TabIndex = 0;
            this.lblHouseFlatNumber.Text = "House/Flat Number";
            //
            // txtHouseFlatNumber
            //
            this.txtHouseFlatNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHouseFlatNumber.Location = new System.Drawing.Point(115, 25);
            this.txtHouseFlatNumber.MaxLength = 20;
            this.txtHouseFlatNumber.Name = "txtHouseFlatNumber";
            this.txtHouseFlatNumber.Size = new System.Drawing.Size(154, 20);
            this.txtHouseFlatNumber.TabIndex = 1;
            //
            // lblComplexFlatNumber
            //
            this.lblComplexFlatNumber.AutoSize = true;
            this.lblComplexFlatNumber.Location = new System.Drawing.Point(10, 57);
            this.lblComplexFlatNumber.Name = "lblComplexFlatNumber";
            this.lblComplexFlatNumber.Size = new System.Drawing.Size(104, 13);
            this.lblComplexFlatNumber.TabIndex = 2;
            this.lblComplexFlatNumber.Text = "Complex/Flat No.";
            //
            // txtComplexFlatNumber
            //
            this.txtComplexFlatNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtComplexFlatNumber.Location = new System.Drawing.Point(115, 54);
            this.txtComplexFlatNumber.MaxLength = 15;
            this.txtComplexFlatNumber.Name = "txtComplexFlatNumber";
            this.txtComplexFlatNumber.Size = new System.Drawing.Size(154, 20);
            this.txtComplexFlatNumber.TabIndex = 3;
            //
            // lblStreetName
            //
            this.lblStreetName.AutoSize = true;
            this.lblStreetName.Location = new System.Drawing.Point(10, 86);
            this.lblStreetName.Name = "lblStreetName";
            this.lblStreetName.Size = new System.Drawing.Size(66, 13);
            this.lblStreetName.TabIndex = 4;
            this.lblStreetName.Text = "Street Name";
            //
            // txtStreetName
            //
            this.txtStreetName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStreetName.Location = new System.Drawing.Point(115, 83);
            this.txtStreetName.MaxLength = 150;
            this.txtStreetName.Name = "txtStreetName";
            this.txtStreetName.Size = new System.Drawing.Size(154, 20);
            this.txtStreetName.TabIndex = 5;
            //
            // lblTown
            //
            this.lblTown.AutoSize = true;
            this.lblTown.Location = new System.Drawing.Point(10, 115);
            this.lblTown.Name = "lblTown";
            this.lblTown.Size = new System.Drawing.Size(33, 13);
            this.lblTown.TabIndex = 6;
            this.lblTown.Text = "Town";
            //
            // txtTown
            //
            this.txtTown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTown.Location = new System.Drawing.Point(115, 112);
            this.txtTown.MaxLength = 100;
            this.txtTown.Name = "txtTown";
            this.txtTown.Size = new System.Drawing.Size(154, 20);
            this.txtTown.TabIndex = 7;
            //
            // lblPostalCode
            //
            this.lblPostalCode.AutoSize = true;
            this.lblPostalCode.Location = new System.Drawing.Point(10, 144);
            this.lblPostalCode.Name = "lblPostalCode";
            this.lblPostalCode.Size = new System.Drawing.Size(63, 13);
            this.lblPostalCode.TabIndex = 8;
            this.lblPostalCode.Text = "Postal Code";
            //
            // txtPostalCode
            //
            this.txtPostalCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPostalCode.Location = new System.Drawing.Point(115, 141);
            this.txtPostalCode.MaxLength = 10;
            this.txtPostalCode.Name = "txtPostalCode";
            this.txtPostalCode.Size = new System.Drawing.Size(154, 20);
            this.txtPostalCode.TabIndex = 9;
            //
            // grpBank
            //
            this.grpBank.Controls.Add(this.lblBankName);
            this.grpBank.Controls.Add(this.txtBankName);
            this.grpBank.Controls.Add(this.lblAccountType);
            this.grpBank.Controls.Add(this.txtAccountType);
            this.grpBank.Controls.Add(this.lblAccountNumber);
            this.grpBank.Controls.Add(this.txtAccountNumber);
            this.grpBank.Controls.Add(this.lblBranchCode);
            this.grpBank.Controls.Add(this.txtBranchCode);
            this.grpBank.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBank.Location = new System.Drawing.Point(3, 179);
            this.grpBank.Name = "grpBank";
            this.grpBank.Size = new System.Drawing.Size(280, 142);
            this.grpBank.TabIndex = 1;
            this.grpBank.TabStop = false;
            this.grpBank.Text = "Bank";
            //
            // lblBankName
            //
            this.lblBankName.AutoSize = true;
            this.lblBankName.Location = new System.Drawing.Point(10, 28);
            this.lblBankName.Name = "lblBankName";
            this.lblBankName.Size = new System.Drawing.Size(62, 13);
            this.lblBankName.TabIndex = 0;
            this.lblBankName.Text = "Bank Name";
            //
            // txtBankName
            //
            this.txtBankName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBankName.Location = new System.Drawing.Point(115, 25);
            this.txtBankName.MaxLength = 100;
            this.txtBankName.Name = "txtBankName";
            this.txtBankName.Size = new System.Drawing.Size(154, 20);
            this.txtBankName.TabIndex = 1;
            //
            // lblAccountType
            //
            this.lblAccountType.AutoSize = true;
            this.lblAccountType.Location = new System.Drawing.Point(10, 57);
            this.lblAccountType.Name = "lblAccountType";
            this.lblAccountType.Size = new System.Drawing.Size(72, 13);
            this.lblAccountType.TabIndex = 2;
            this.lblAccountType.Text = "Account Type";
            //
            // txtAccountType
            //
            this.txtAccountType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAccountType.Location = new System.Drawing.Point(115, 54);
            this.txtAccountType.MaxLength = 30;
            this.txtAccountType.Name = "txtAccountType";
            this.txtAccountType.Size = new System.Drawing.Size(154, 20);
            this.txtAccountType.TabIndex = 3;
            //
            // lblAccountNumber
            //
            this.lblAccountNumber.AutoSize = true;
            this.lblAccountNumber.Location = new System.Drawing.Point(10, 86);
            this.lblAccountNumber.Name = "lblAccountNumber";
            this.lblAccountNumber.Size = new System.Drawing.Size(86, 13);
            this.lblAccountNumber.TabIndex = 4;
            this.lblAccountNumber.Text = "Account Number";
            //
            // txtAccountNumber
            //
            this.txtAccountNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAccountNumber.Location = new System.Drawing.Point(115, 83);
            this.txtAccountNumber.MaxLength = 30;
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.Size = new System.Drawing.Size(154, 20);
            this.txtAccountNumber.TabIndex = 5;
            //
            // lblBranchCode
            //
            this.lblBranchCode.AutoSize = true;
            this.lblBranchCode.Location = new System.Drawing.Point(10, 115);
            this.lblBranchCode.Name = "lblBranchCode";
            this.lblBranchCode.Size = new System.Drawing.Size(68, 13);
            this.lblBranchCode.TabIndex = 6;
            this.lblBranchCode.Text = "Branch Code";
            //
            // txtBranchCode
            //
            this.txtBranchCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBranchCode.Location = new System.Drawing.Point(115, 112);
            this.txtBranchCode.MaxLength = 10;
            this.txtBranchCode.Name = "txtBranchCode";
            this.txtBranchCode.Size = new System.Drawing.Size(154, 20);
            this.txtBranchCode.TabIndex = 7;
            //
            // tlpContractSpouse
            //
            this.tlpContractSpouse.ColumnCount = 1;
            this.tlpContractSpouse.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpContractSpouse.Controls.Add(this.grpContract, 0, 0);
            this.tlpContractSpouse.Controls.Add(this.grpSpouse, 0, 1);
            this.tlpContractSpouse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpContractSpouse.Location = new System.Drawing.Point(593, 3);
            this.tlpContractSpouse.Name = "tlpContractSpouse";
            this.tlpContractSpouse.RowCount = 2;
            this.tlpContractSpouse.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 176F));
            this.tlpContractSpouse.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpContractSpouse.Size = new System.Drawing.Size(287, 324);
            this.tlpContractSpouse.TabIndex = 2;
            //
            // grpContract
            //
            this.grpContract.Controls.Add(this.lblContractType);
            this.grpContract.Controls.Add(this.txtContractType);
            this.grpContract.Controls.Add(this.lblStartDate);
            this.grpContract.Controls.Add(this.dtpStartDate);
            this.grpContract.Controls.Add(this.lblDepartment);
            this.grpContract.Controls.Add(this.txtDepartment);
            this.grpContract.Controls.Add(this.lblJobDescription);
            this.grpContract.Controls.Add(this.txtJobDescription);
            this.grpContract.Controls.Add(this.lblHourlyRate);
            this.grpContract.Controls.Add(this.txtHourlyRate);
            this.grpContract.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpContract.Location = new System.Drawing.Point(3, 3);
            this.grpContract.Name = "grpContract";
            this.grpContract.Size = new System.Drawing.Size(281, 170);
            this.grpContract.TabIndex = 0;
            this.grpContract.TabStop = false;
            this.grpContract.Text = "Contract";
            //
            // lblContractType
            //
            this.lblContractType.AutoSize = true;
            this.lblContractType.Location = new System.Drawing.Point(10, 28);
            this.lblContractType.Name = "lblContractType";
            this.lblContractType.Size = new System.Drawing.Size(74, 13);
            this.lblContractType.TabIndex = 0;
            this.lblContractType.Text = "Contract Type";
            //
            // txtContractType
            //
            this.txtContractType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContractType.Location = new System.Drawing.Point(115, 25);
            this.txtContractType.MaxLength = 30;
            this.txtContractType.Name = "txtContractType";
            this.txtContractType.Size = new System.Drawing.Size(155, 20);
            this.txtContractType.TabIndex = 1;
            //
            // lblStartDate
            //
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(10, 57);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(54, 13);
            this.lblStartDate.TabIndex = 2;
            this.lblStartDate.Text = "Start Date";
            //
            // dtpStartDate
            //
            this.dtpStartDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(115, 54);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(155, 20);
            this.dtpStartDate.TabIndex = 3;
            //
            // lblDepartment
            //
            this.lblDepartment.AutoSize = true;
            this.lblDepartment.Location = new System.Drawing.Point(10, 86);
            this.lblDepartment.Name = "lblDepartment";
            this.lblDepartment.Size = new System.Drawing.Size(62, 13);
            this.lblDepartment.TabIndex = 4;
            this.lblDepartment.Text = "Department";
            //
            // txtDepartment
            //
            this.txtDepartment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDepartment.Location = new System.Drawing.Point(115, 83);
            this.txtDepartment.MaxLength = 100;
            this.txtDepartment.Name = "txtDepartment";
            this.txtDepartment.Size = new System.Drawing.Size(155, 20);
            this.txtDepartment.TabIndex = 5;
            //
            // lblJobDescription
            //
            this.lblJobDescription.AutoSize = true;
            this.lblJobDescription.Location = new System.Drawing.Point(10, 115);
            this.lblJobDescription.Name = "lblJobDescription";
            this.lblJobDescription.Size = new System.Drawing.Size(80, 13);
            this.lblJobDescription.TabIndex = 6;
            this.lblJobDescription.Text = "Job Description";
            //
            // txtJobDescription
            //
            this.txtJobDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJobDescription.Location = new System.Drawing.Point(115, 112);
            this.txtJobDescription.MaxLength = 100;
            this.txtJobDescription.Name = "txtJobDescription";
            this.txtJobDescription.Size = new System.Drawing.Size(155, 20);
            this.txtJobDescription.TabIndex = 7;
            //
            // lblHourlyRate
            //
            this.lblHourlyRate.AutoSize = true;
            this.lblHourlyRate.Location = new System.Drawing.Point(10, 144);
            this.lblHourlyRate.Name = "lblHourlyRate";
            this.lblHourlyRate.Size = new System.Drawing.Size(62, 13);
            this.lblHourlyRate.TabIndex = 8;
            this.lblHourlyRate.Text = "Hourly Rate";
            //
            // txtHourlyRate
            //
            this.txtHourlyRate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHourlyRate.Location = new System.Drawing.Point(115, 141);
            this.txtHourlyRate.Name = "txtHourlyRate";
            this.txtHourlyRate.Size = new System.Drawing.Size(155, 20);
            this.txtHourlyRate.TabIndex = 9;
            //
            // grpSpouse
            //
            this.grpSpouse.Controls.Add(this.lblSpouseName);
            this.grpSpouse.Controls.Add(this.txtSpouseName);
            this.grpSpouse.Controls.Add(this.lblSpouseMobileNumber);
            this.grpSpouse.Controls.Add(this.txtSpouseMobileNumber);
            this.grpSpouse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSpouse.Location = new System.Drawing.Point(3, 179);
            this.grpSpouse.Name = "grpSpouse";
            this.grpSpouse.Size = new System.Drawing.Size(281, 142);
            this.grpSpouse.TabIndex = 1;
            this.grpSpouse.TabStop = false;
            this.grpSpouse.Text = "Spouse";
            //
            // lblSpouseName
            //
            this.lblSpouseName.AutoSize = true;
            this.lblSpouseName.Location = new System.Drawing.Point(10, 28);
            this.lblSpouseName.Name = "lblSpouseName";
            this.lblSpouseName.Size = new System.Drawing.Size(74, 13);
            this.lblSpouseName.TabIndex = 0;
            this.lblSpouseName.Text = "Spouse Name";
            //
            // txtSpouseName
            //
            this.txtSpouseName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpouseName.Location = new System.Drawing.Point(115, 25);
            this.txtSpouseName.MaxLength = 200;
            this.txtSpouseName.Name = "txtSpouseName";
            this.txtSpouseName.Size = new System.Drawing.Size(155, 20);
            this.txtSpouseName.TabIndex = 1;
            //
            // lblSpouseMobileNumber
            //
            this.lblSpouseMobileNumber.AutoSize = true;
            this.lblSpouseMobileNumber.Location = new System.Drawing.Point(10, 57);
            this.lblSpouseMobileNumber.Name = "lblSpouseMobileNumber";
            this.lblSpouseMobileNumber.Size = new System.Drawing.Size(115, 13);
            this.lblSpouseMobileNumber.TabIndex = 2;
            this.lblSpouseMobileNumber.Text = "Spouse Mobile No.";
            //
            // txtSpouseMobileNumber
            //
            this.txtSpouseMobileNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpouseMobileNumber.Location = new System.Drawing.Point(115, 54);
            this.txtSpouseMobileNumber.MaxLength = 20;
            this.txtSpouseMobileNumber.Name = "txtSpouseMobileNumber";
            this.txtSpouseMobileNumber.Size = new System.Drawing.Size(155, 20);
            this.txtSpouseMobileNumber.TabIndex = 3;
            //
            // grpFamily
            //
            this.grpFamily.Controls.Add(this.dgvFamily);
            this.grpFamily.Controls.Add(this.pnlFamilyButtons);
            this.grpFamily.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpFamily.Location = new System.Drawing.Point(9, 333);
            this.grpFamily.Name = "grpFamily";
            this.grpFamily.Size = new System.Drawing.Size(871, 164);
            this.grpFamily.TabIndex = 3;
            this.grpFamily.TabStop = false;
            this.grpFamily.Text = "Family Contacts";
            //
            // dgvFamily
            //
            this.dgvFamily.AllowUserToAddRows = false;
            this.dgvFamily.AllowUserToDeleteRows = false;
            this.dgvFamily.AllowUserToResizeRows = false;
            this.dgvFamily.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFamily.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFamily.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFamilyName,
            this.colFamilyMobileNumber,
            this.colFamilyRelationship});
            this.dgvFamily.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFamily.Location = new System.Drawing.Point(3, 16);
            this.dgvFamily.MultiSelect = false;
            this.dgvFamily.Name = "dgvFamily";
            this.dgvFamily.RowHeadersVisible = false;
            this.dgvFamily.Size = new System.Drawing.Size(745, 145);
            this.dgvFamily.TabIndex = 0;
            //
            // colFamilyName
            //
            this.colFamilyName.HeaderText = "Name";
            this.colFamilyName.MaxInputLength = 200;
            this.colFamilyName.Name = "colFamilyName";
            this.colFamilyName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colFamilyMobileNumber
            //
            this.colFamilyMobileNumber.HeaderText = "Mobile Number";
            this.colFamilyMobileNumber.MaxInputLength = 20;
            this.colFamilyMobileNumber.Name = "colFamilyMobileNumber";
            this.colFamilyMobileNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colFamilyRelationship
            //
            this.colFamilyRelationship.HeaderText = "Relationship";
            this.colFamilyRelationship.MaxInputLength = 50;
            this.colFamilyRelationship.Name = "colFamilyRelationship";
            this.colFamilyRelationship.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // pnlFamilyButtons
            //
            this.pnlFamilyButtons.Controls.Add(this.btnAddFamilyRow);
            this.pnlFamilyButtons.Controls.Add(this.btnRemoveFamilyRow);
            this.pnlFamilyButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlFamilyButtons.Location = new System.Drawing.Point(748, 16);
            this.pnlFamilyButtons.Name = "pnlFamilyButtons";
            this.pnlFamilyButtons.Size = new System.Drawing.Size(120, 145);
            this.pnlFamilyButtons.TabIndex = 1;
            //
            // btnAddFamilyRow
            //
            this.btnAddFamilyRow.Location = new System.Drawing.Point(12, 12);
            this.btnAddFamilyRow.Name = "btnAddFamilyRow";
            this.btnAddFamilyRow.Size = new System.Drawing.Size(96, 25);
            this.btnAddFamilyRow.TabIndex = 0;
            this.btnAddFamilyRow.Text = "Add Row";
            this.btnAddFamilyRow.UseVisualStyleBackColor = true;
            this.btnAddFamilyRow.Click += new System.EventHandler(this.btnAddFamilyRow_Click);
            //
            // btnRemoveFamilyRow
            //
            this.btnRemoveFamilyRow.Location = new System.Drawing.Point(12, 43);
            this.btnRemoveFamilyRow.Name = "btnRemoveFamilyRow";
            this.btnRemoveFamilyRow.Size = new System.Drawing.Size(96, 25);
            this.btnRemoveFamilyRow.TabIndex = 1;
            this.btnRemoveFamilyRow.Text = "Remove Row";
            this.btnRemoveFamilyRow.UseVisualStyleBackColor = true;
            this.btnRemoveFamilyRow.Click += new System.EventHandler(this.btnRemoveFamilyRow_Click);
            //
            // pnlEditorButtons
            //
            this.pnlEditorButtons.Controls.Add(this.lblEditorStatus);
            this.pnlEditorButtons.Controls.Add(this.btnSave);
            this.pnlEditorButtons.Controls.Add(this.btnCancel);
            this.pnlEditorButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlEditorButtons.Location = new System.Drawing.Point(0, 530);
            this.pnlEditorButtons.Name = "pnlEditorButtons";
            this.pnlEditorButtons.Size = new System.Drawing.Size(889, 44);
            this.pnlEditorButtons.TabIndex = 2;
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(709, 9);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 26);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // lblEditorStatus
            //
            this.lblEditorStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEditorStatus.AutoEllipsis = true;
            this.lblEditorStatus.ForeColor = System.Drawing.Color.Firebrick;
            this.lblEditorStatus.Location = new System.Drawing.Point(9, 9);
            this.lblEditorStatus.Name = "lblEditorStatus";
            this.lblEditorStatus.Size = new System.Drawing.Size(694, 26);
            this.lblEditorStatus.TabIndex = 2;
            this.lblEditorStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(795, 9);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 26);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // EmployeesForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 574);
            this.Controls.Add(this.pnlEditor);
            this.Controls.Add(this.pnlList);
            this.Name = "EmployeesForm";
            this.Text = "EmployeesForm";
            this.pnlListTop.ResumeLayout(false);
            this.pnlListTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            this.pnlList.ResumeLayout(false);
            this.grpPersonal.ResumeLayout(false);
            this.grpPersonal.PerformLayout();
            this.grpAddress.ResumeLayout(false);
            this.grpAddress.PerformLayout();
            this.grpBank.ResumeLayout(false);
            this.grpBank.PerformLayout();
            this.tlpAddressBank.ResumeLayout(false);
            this.grpContract.ResumeLayout(false);
            this.grpContract.PerformLayout();
            this.grpSpouse.ResumeLayout(false);
            this.grpSpouse.PerformLayout();
            this.tlpContractSpouse.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamily)).EndInit();
            this.pnlFamilyButtons.ResumeLayout(false);
            this.grpFamily.ResumeLayout(false);
            this.tlpEditor.ResumeLayout(false);
            this.pnlEditorButtons.ResumeLayout(false);
            this.pnlEditor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlList;
        private System.Windows.Forms.Panel pnlListTop;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnTerminate;
        private System.Windows.Forms.Button btnReactivate;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.CheckBox chkShowFormer;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.DataGridView dgvEmployees;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSurname;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIDNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMobileNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colJobDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.Panel pnlEditor;
        private System.Windows.Forms.Label lblEditorTitle;
        private System.Windows.Forms.TableLayoutPanel tlpEditor;
        private System.Windows.Forms.GroupBox grpPersonal;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblSurname;
        private System.Windows.Forms.TextBox txtSurname;
        private System.Windows.Forms.Label lblIDNumber;
        private System.Windows.Forms.TextBox txtIDNumber;
        private System.Windows.Forms.Label lblSARSNumber;
        private System.Windows.Forms.TextBox txtSARSNumber;
        private System.Windows.Forms.Label lblMobileNumber;
        private System.Windows.Forms.TextBox txtMobileNumber;
        private System.Windows.Forms.Label lblMaritalStatus;
        private System.Windows.Forms.ComboBox cboMaritalStatus;
        private System.Windows.Forms.Label lblDependents;
        private System.Windows.Forms.TextBox txtDependents;
        private System.Windows.Forms.TableLayoutPanel tlpAddressBank;
        private System.Windows.Forms.GroupBox grpAddress;
        private System.Windows.Forms.Label lblHouseFlatNumber;
        private System.Windows.Forms.TextBox txtHouseFlatNumber;
        private System.Windows.Forms.Label lblComplexFlatNumber;
        private System.Windows.Forms.TextBox txtComplexFlatNumber;
        private System.Windows.Forms.Label lblStreetName;
        private System.Windows.Forms.TextBox txtStreetName;
        private System.Windows.Forms.Label lblTown;
        private System.Windows.Forms.TextBox txtTown;
        private System.Windows.Forms.Label lblPostalCode;
        private System.Windows.Forms.TextBox txtPostalCode;
        private System.Windows.Forms.GroupBox grpBank;
        private System.Windows.Forms.Label lblBankName;
        private System.Windows.Forms.TextBox txtBankName;
        private System.Windows.Forms.Label lblAccountType;
        private System.Windows.Forms.TextBox txtAccountType;
        private System.Windows.Forms.Label lblAccountNumber;
        private System.Windows.Forms.TextBox txtAccountNumber;
        private System.Windows.Forms.Label lblBranchCode;
        private System.Windows.Forms.TextBox txtBranchCode;
        private System.Windows.Forms.TableLayoutPanel tlpContractSpouse;
        private System.Windows.Forms.GroupBox grpContract;
        private System.Windows.Forms.Label lblContractType;
        private System.Windows.Forms.TextBox txtContractType;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lblDepartment;
        private System.Windows.Forms.TextBox txtDepartment;
        private System.Windows.Forms.Label lblJobDescription;
        private System.Windows.Forms.TextBox txtJobDescription;
        private System.Windows.Forms.Label lblHourlyRate;
        private System.Windows.Forms.TextBox txtHourlyRate;
        private System.Windows.Forms.GroupBox grpSpouse;
        private System.Windows.Forms.Label lblSpouseName;
        private System.Windows.Forms.TextBox txtSpouseName;
        private System.Windows.Forms.Label lblSpouseMobileNumber;
        private System.Windows.Forms.TextBox txtSpouseMobileNumber;
        private System.Windows.Forms.GroupBox grpFamily;
        private System.Windows.Forms.DataGridView dgvFamily;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFamilyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFamilyMobileNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFamilyRelationship;
        private System.Windows.Forms.Panel pnlFamilyButtons;
        private System.Windows.Forms.Button btnAddFamilyRow;
        private System.Windows.Forms.Button btnRemoveFamilyRow;
        private System.Windows.Forms.Panel pnlEditorButtons;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblEditorStatus;
        private System.Windows.Forms.ToolTip tipEditorErrors;
    }
}
