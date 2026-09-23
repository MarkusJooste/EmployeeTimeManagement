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
            this.pnlSplit = new System.Windows.Forms.Panel();
            this.employeeDetails = new EmployeeTimeManagement.Views.EmployeeDetailsPanel();
            this.employeePicker = new EmployeeTimeManagement.Views.EmployeePickerControl();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblViewTitle = new System.Windows.Forms.Label();
            this.lblStore = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.pnlEditor = new System.Windows.Forms.Panel();
            this.pnlSectionBody = new System.Windows.Forms.Panel();
            this.pnlSectionContent = new System.Windows.Forms.Panel();
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
            this.lblOpeningPTODays = new System.Windows.Forms.Label();
            this.txtOpeningPTODays = new System.Windows.Forms.TextBox();
            this.lblOpeningBalanceAsAt = new System.Windows.Forms.Label();
            this.dtpOpeningBalanceAsAt = new System.Windows.Forms.DateTimePicker();
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
            this.pnlSectionList = new System.Windows.Forms.Panel();
            this.btnSectionFamily = new System.Windows.Forms.Button();
            this.btnSectionSpouse = new System.Windows.Forms.Button();
            this.btnSectionContract = new System.Windows.Forms.Button();
            this.btnSectionBank = new System.Windows.Forms.Button();
            this.btnSectionAddress = new System.Windows.Forms.Button();
            this.btnSectionPersonal = new System.Windows.Forms.Button();
            this.btnBackToList = new System.Windows.Forms.Button();
            this.pnlEditorButtons = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblEditorTitle = new System.Windows.Forms.Label();
            this.lblEditorStatus = new System.Windows.Forms.Label();
            this.tipEditorErrors = new System.Windows.Forms.ToolTip(this.components);
            this.pnlList.SuspendLayout();
            this.pnlSplit.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlEditor.SuspendLayout();
            this.pnlSectionBody.SuspendLayout();
            this.pnlSectionContent.SuspendLayout();
            this.grpPersonal.SuspendLayout();
            this.grpAddress.SuspendLayout();
            this.grpBank.SuspendLayout();
            this.grpContract.SuspendLayout();
            this.grpSpouse.SuspendLayout();
            this.grpFamily.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamily)).BeginInit();
            this.pnlFamilyButtons.SuspendLayout();
            this.pnlSectionList.SuspendLayout();
            this.pnlEditorButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlList
            //
            this.pnlList.Controls.Add(this.pnlSplit);
            this.pnlList.Controls.Add(this.lblStatus);
            this.pnlList.Controls.Add(this.pnlHeader);
            this.pnlList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlList.Location = new System.Drawing.Point(0, 0);
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new System.Drawing.Size(889, 574);
            this.pnlList.TabIndex = 0;
            //
            // pnlSplit
            //
            this.pnlSplit.Controls.Add(this.employeeDetails);
            this.pnlSplit.Controls.Add(this.employeePicker);
            this.pnlSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSplit.Location = new System.Drawing.Point(0, 64);
            this.pnlSplit.Name = "pnlSplit";
            this.pnlSplit.Size = new System.Drawing.Size(889, 482);
            this.pnlSplit.TabIndex = 1;
            //
            // employeePicker
            //
            this.employeePicker.Dock = System.Windows.Forms.DockStyle.Left;
            this.employeePicker.Location = new System.Drawing.Point(0, 0);
            this.employeePicker.Name = "employeePicker";
            this.employeePicker.Size = new System.Drawing.Size(300, 482);
            this.employeePicker.TabIndex = 0;
            this.employeePicker.SelectionChanged += new System.EventHandler(this.employeePicker_SelectionChanged);
            this.employeePicker.FilterChanged += new System.EventHandler(this.employeePicker_FilterChanged);
            this.employeePicker.EmployeeActivated += new System.EventHandler(this.employeePicker_EmployeeActivated);
            //
            // employeeDetails
            //
            this.employeeDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.employeeDetails.Location = new System.Drawing.Point(300, 0);
            this.employeeDetails.Name = "employeeDetails";
            this.employeeDetails.Size = new System.Drawing.Size(589, 482);
            this.employeeDetails.TabIndex = 1;
            this.employeeDetails.UpdateClicked += new System.EventHandler(this.btnUpdate_Click);
            this.employeeDetails.TerminateClicked += new System.EventHandler(this.btnTerminate_Click);
            this.employeeDetails.ReactivateClicked += new System.EventHandler(this.btnReactivate_Click);
            //
            // lblStatus
            //
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Location = new System.Drawing.Point(0, 546);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.lblStatus.Size = new System.Drawing.Size(889, 28);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "lblStatus";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlHeader
            //
            this.pnlHeader.Controls.Add(this.lblViewTitle);
            this.pnlHeader.Controls.Add(this.lblStore);
            this.pnlHeader.Controls.Add(this.btnAdd);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(889, 64);
            this.pnlHeader.TabIndex = 0;
            //
            // lblViewTitle
            //
            this.lblViewTitle.AutoSize = true;
            this.lblViewTitle.Location = new System.Drawing.Point(16, 9);
            this.lblViewTitle.Name = "lblViewTitle";
            this.lblViewTitle.Size = new System.Drawing.Size(83, 25);
            this.lblViewTitle.TabIndex = 0;
            this.lblViewTitle.Text = "Employees";
            //
            // lblStore
            //
            this.lblStore.AutoSize = true;
            this.lblStore.Location = new System.Drawing.Point(18, 38);
            this.lblStore.Name = "lblStore";
            this.lblStore.Size = new System.Drawing.Size(45, 15);
            this.lblStore.TabIndex = 1;
            this.lblStore.Text = "Store";
            //
            // btnAdd
            //
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(725, 16);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(148, 32);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add employee";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // pnlEditor
            //
            this.pnlEditor.Controls.Add(this.pnlSectionBody);
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
            this.lblEditorTitle.Location = new System.Drawing.Point(0, 0);
            this.lblEditorTitle.Name = "lblEditorTitle";
            this.lblEditorTitle.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblEditorTitle.Size = new System.Drawing.Size(889, 30);
            this.lblEditorTitle.TabIndex = 0;
            this.lblEditorTitle.Text = "Add Employee";
            this.lblEditorTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlSectionBody
            //
            this.pnlSectionBody.Controls.Add(this.pnlSectionContent);
            this.pnlSectionBody.Controls.Add(this.pnlSectionList);
            this.pnlSectionBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSectionBody.Location = new System.Drawing.Point(0, 30);
            this.pnlSectionBody.Name = "pnlSectionBody";
            this.pnlSectionBody.Size = new System.Drawing.Size(889, 500);
            this.pnlSectionBody.TabIndex = 1;
            //
            // pnlSectionContent
            //
            this.pnlSectionContent.Controls.Add(this.grpPersonal);
            this.pnlSectionContent.Controls.Add(this.grpAddress);
            this.pnlSectionContent.Controls.Add(this.grpBank);
            this.pnlSectionContent.Controls.Add(this.grpContract);
            this.pnlSectionContent.Controls.Add(this.grpSpouse);
            this.pnlSectionContent.Controls.Add(this.grpFamily);
            this.pnlSectionContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSectionContent.Location = new System.Drawing.Point(200, 0);
            this.pnlSectionContent.Name = "pnlSectionContent";
            this.pnlSectionContent.Padding = new System.Windows.Forms.Padding(12);
            this.pnlSectionContent.Size = new System.Drawing.Size(689, 500);
            this.pnlSectionContent.TabIndex = 1;
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
            this.grpPersonal.Location = new System.Drawing.Point(12, 12);
            this.grpPersonal.Name = "grpPersonal";
            this.grpPersonal.Size = new System.Drawing.Size(665, 476);
            this.grpPersonal.TabIndex = 0;
            this.grpPersonal.TabStop = false;
            this.grpPersonal.Text = "Personal";
            //
            // lblName
            //
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(16, 32);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(41, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name *";
            //
            // txtName
            //
            this.txtName.Location = new System.Drawing.Point(142, 29);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 20);
            this.txtName.TabIndex = 1;
            //
            // lblSurname
            //
            this.lblSurname.AutoSize = true;
            this.lblSurname.Location = new System.Drawing.Point(366, 32);
            this.lblSurname.Name = "lblSurname";
            this.lblSurname.Size = new System.Drawing.Size(55, 13);
            this.lblSurname.TabIndex = 2;
            this.lblSurname.Text = "Surname *";
            //
            // txtSurname
            //
            this.txtSurname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSurname.Location = new System.Drawing.Point(492, 29);
            this.txtSurname.Name = "txtSurname";
            this.txtSurname.Size = new System.Drawing.Size(157, 20);
            this.txtSurname.TabIndex = 3;
            //
            // lblIDNumber
            //
            this.lblIDNumber.AutoSize = true;
            this.lblIDNumber.Location = new System.Drawing.Point(16, 66);
            this.lblIDNumber.Name = "lblIDNumber";
            this.lblIDNumber.Size = new System.Drawing.Size(63, 13);
            this.lblIDNumber.TabIndex = 4;
            this.lblIDNumber.Text = "ID Number *";
            //
            // txtIDNumber
            //
            this.txtIDNumber.Location = new System.Drawing.Point(142, 63);
            this.txtIDNumber.MaxLength = 13;
            this.txtIDNumber.Name = "txtIDNumber";
            this.txtIDNumber.Size = new System.Drawing.Size(200, 20);
            this.txtIDNumber.TabIndex = 5;
            //
            // lblSARSNumber
            //
            this.lblSARSNumber.AutoSize = true;
            this.lblSARSNumber.Location = new System.Drawing.Point(366, 66);
            this.lblSARSNumber.Name = "lblSARSNumber";
            this.lblSARSNumber.Size = new System.Drawing.Size(101, 13);
            this.lblSARSNumber.TabIndex = 6;
            this.lblSARSNumber.Text = "SARS Number";
            //
            // txtSARSNumber
            //
            this.txtSARSNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSARSNumber.Location = new System.Drawing.Point(492, 63);
            this.txtSARSNumber.MaxLength = 20;
            this.txtSARSNumber.Name = "txtSARSNumber";
            this.txtSARSNumber.Size = new System.Drawing.Size(157, 20);
            this.txtSARSNumber.TabIndex = 7;
            //
            // lblMobileNumber
            //
            this.lblMobileNumber.AutoSize = true;
            this.lblMobileNumber.Location = new System.Drawing.Point(16, 100);
            this.lblMobileNumber.Name = "lblMobileNumber";
            this.lblMobileNumber.Size = new System.Drawing.Size(84, 13);
            this.lblMobileNumber.TabIndex = 8;
            this.lblMobileNumber.Text = "Mobile Number *";
            //
            // txtMobileNumber
            //
            this.txtMobileNumber.Location = new System.Drawing.Point(142, 97);
            this.txtMobileNumber.MaxLength = 20;
            this.txtMobileNumber.Name = "txtMobileNumber";
            this.txtMobileNumber.Size = new System.Drawing.Size(200, 20);
            this.txtMobileNumber.TabIndex = 9;
            //
            // lblMaritalStatus
            //
            this.lblMaritalStatus.AutoSize = true;
            this.lblMaritalStatus.Location = new System.Drawing.Point(366, 100);
            this.lblMaritalStatus.Name = "lblMaritalStatus";
            this.lblMaritalStatus.Size = new System.Drawing.Size(79, 13);
            this.lblMaritalStatus.TabIndex = 10;
            this.lblMaritalStatus.Text = "Marital Status *";
            //
            // cboMaritalStatus
            //
            this.cboMaritalStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboMaritalStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMaritalStatus.FormattingEnabled = true;
            this.cboMaritalStatus.Location = new System.Drawing.Point(492, 97);
            this.cboMaritalStatus.Name = "cboMaritalStatus";
            this.cboMaritalStatus.Size = new System.Drawing.Size(157, 21);
            this.cboMaritalStatus.TabIndex = 11;
            this.cboMaritalStatus.SelectedIndexChanged += new System.EventHandler(this.cboMaritalStatus_SelectedIndexChanged);
            //
            // lblDependents
            //
            this.lblDependents.AutoSize = true;
            this.lblDependents.Location = new System.Drawing.Point(16, 134);
            this.lblDependents.Name = "lblDependents";
            this.lblDependents.Size = new System.Drawing.Size(78, 13);
            this.lblDependents.TabIndex = 12;
            this.lblDependents.Text = "Dependents *";
            //
            // txtDependents
            //
            this.txtDependents.Location = new System.Drawing.Point(142, 131);
            this.txtDependents.Name = "txtDependents";
            this.txtDependents.Size = new System.Drawing.Size(200, 20);
            this.txtDependents.TabIndex = 13;
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
            this.grpAddress.Location = new System.Drawing.Point(12, 12);
            this.grpAddress.Name = "grpAddress";
            this.grpAddress.Size = new System.Drawing.Size(665, 476);
            this.grpAddress.TabIndex = 0;
            this.grpAddress.TabStop = false;
            this.grpAddress.Text = "Address";
            this.grpAddress.Visible = false;
            //
            // lblHouseFlatNumber
            //
            this.lblHouseFlatNumber.AutoSize = true;
            this.lblHouseFlatNumber.Location = new System.Drawing.Point(16, 32);
            this.lblHouseFlatNumber.Name = "lblHouseFlatNumber";
            this.lblHouseFlatNumber.Size = new System.Drawing.Size(114, 13);
            this.lblHouseFlatNumber.TabIndex = 0;
            this.lblHouseFlatNumber.Text = "House/Flat Number *";
            //
            // txtHouseFlatNumber
            //
            this.txtHouseFlatNumber.Location = new System.Drawing.Point(142, 29);
            this.txtHouseFlatNumber.MaxLength = 20;
            this.txtHouseFlatNumber.Name = "txtHouseFlatNumber";
            this.txtHouseFlatNumber.Size = new System.Drawing.Size(200, 20);
            this.txtHouseFlatNumber.TabIndex = 1;
            //
            // lblComplexFlatNumber
            //
            this.lblComplexFlatNumber.AutoSize = true;
            this.lblComplexFlatNumber.Location = new System.Drawing.Point(366, 32);
            this.lblComplexFlatNumber.Name = "lblComplexFlatNumber";
            this.lblComplexFlatNumber.Size = new System.Drawing.Size(104, 13);
            this.lblComplexFlatNumber.TabIndex = 2;
            this.lblComplexFlatNumber.Text = "Complex/Flat No.";
            //
            // txtComplexFlatNumber
            //
            this.txtComplexFlatNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtComplexFlatNumber.Location = new System.Drawing.Point(492, 29);
            this.txtComplexFlatNumber.MaxLength = 15;
            this.txtComplexFlatNumber.Name = "txtComplexFlatNumber";
            this.txtComplexFlatNumber.Size = new System.Drawing.Size(157, 20);
            this.txtComplexFlatNumber.TabIndex = 3;
            //
            // lblStreetName
            //
            this.lblStreetName.AutoSize = true;
            this.lblStreetName.Location = new System.Drawing.Point(16, 66);
            this.lblStreetName.Name = "lblStreetName";
            this.lblStreetName.Size = new System.Drawing.Size(74, 13);
            this.lblStreetName.TabIndex = 4;
            this.lblStreetName.Text = "Street Name *";
            //
            // txtStreetName
            //
            this.txtStreetName.Location = new System.Drawing.Point(142, 63);
            this.txtStreetName.MaxLength = 150;
            this.txtStreetName.Name = "txtStreetName";
            this.txtStreetName.Size = new System.Drawing.Size(200, 20);
            this.txtStreetName.TabIndex = 5;
            //
            // lblTown
            //
            this.lblTown.AutoSize = true;
            this.lblTown.Location = new System.Drawing.Point(366, 66);
            this.lblTown.Name = "lblTown";
            this.lblTown.Size = new System.Drawing.Size(41, 13);
            this.lblTown.TabIndex = 6;
            this.lblTown.Text = "Town *";
            //
            // txtTown
            //
            this.txtTown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTown.Location = new System.Drawing.Point(492, 63);
            this.txtTown.MaxLength = 100;
            this.txtTown.Name = "txtTown";
            this.txtTown.Size = new System.Drawing.Size(157, 20);
            this.txtTown.TabIndex = 7;
            //
            // lblPostalCode
            //
            this.lblPostalCode.AutoSize = true;
            this.lblPostalCode.Location = new System.Drawing.Point(16, 100);
            this.lblPostalCode.Name = "lblPostalCode";
            this.lblPostalCode.Size = new System.Drawing.Size(71, 13);
            this.lblPostalCode.TabIndex = 8;
            this.lblPostalCode.Text = "Postal Code *";
            //
            // txtPostalCode
            //
            this.txtPostalCode.Location = new System.Drawing.Point(142, 97);
            this.txtPostalCode.MaxLength = 10;
            this.txtPostalCode.Name = "txtPostalCode";
            this.txtPostalCode.Size = new System.Drawing.Size(200, 20);
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
            this.grpBank.Location = new System.Drawing.Point(12, 12);
            this.grpBank.Name = "grpBank";
            this.grpBank.Size = new System.Drawing.Size(665, 476);
            this.grpBank.TabIndex = 0;
            this.grpBank.TabStop = false;
            this.grpBank.Text = "Bank";
            this.grpBank.Visible = false;
            //
            // lblBankName
            //
            this.lblBankName.AutoSize = true;
            this.lblBankName.Location = new System.Drawing.Point(16, 32);
            this.lblBankName.Name = "lblBankName";
            this.lblBankName.Size = new System.Drawing.Size(70, 13);
            this.lblBankName.TabIndex = 0;
            this.lblBankName.Text = "Bank Name *";
            //
            // txtBankName
            //
            this.txtBankName.Location = new System.Drawing.Point(142, 29);
            this.txtBankName.MaxLength = 100;
            this.txtBankName.Name = "txtBankName";
            this.txtBankName.Size = new System.Drawing.Size(200, 20);
            this.txtBankName.TabIndex = 1;
            //
            // lblAccountType
            //
            this.lblAccountType.AutoSize = true;
            this.lblAccountType.Location = new System.Drawing.Point(366, 32);
            this.lblAccountType.Name = "lblAccountType";
            this.lblAccountType.Size = new System.Drawing.Size(80, 13);
            this.lblAccountType.TabIndex = 2;
            this.lblAccountType.Text = "Account Type *";
            //
            // txtAccountType
            //
            this.txtAccountType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAccountType.Location = new System.Drawing.Point(492, 29);
            this.txtAccountType.MaxLength = 30;
            this.txtAccountType.Name = "txtAccountType";
            this.txtAccountType.Size = new System.Drawing.Size(157, 20);
            this.txtAccountType.TabIndex = 3;
            //
            // lblAccountNumber
            //
            this.lblAccountNumber.AutoSize = true;
            this.lblAccountNumber.Location = new System.Drawing.Point(16, 66);
            this.lblAccountNumber.Name = "lblAccountNumber";
            this.lblAccountNumber.Size = new System.Drawing.Size(94, 13);
            this.lblAccountNumber.TabIndex = 4;
            this.lblAccountNumber.Text = "Account Number *";
            //
            // txtAccountNumber
            //
            this.txtAccountNumber.Location = new System.Drawing.Point(142, 63);
            this.txtAccountNumber.MaxLength = 30;
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.Size = new System.Drawing.Size(200, 20);
            this.txtAccountNumber.TabIndex = 5;
            //
            // lblBranchCode
            //
            this.lblBranchCode.AutoSize = true;
            this.lblBranchCode.Location = new System.Drawing.Point(366, 66);
            this.lblBranchCode.Name = "lblBranchCode";
            this.lblBranchCode.Size = new System.Drawing.Size(76, 13);
            this.lblBranchCode.TabIndex = 6;
            this.lblBranchCode.Text = "Branch Code *";
            //
            // txtBranchCode
            //
            this.txtBranchCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBranchCode.Location = new System.Drawing.Point(492, 63);
            this.txtBranchCode.MaxLength = 10;
            this.txtBranchCode.Name = "txtBranchCode";
            this.txtBranchCode.Size = new System.Drawing.Size(157, 20);
            this.txtBranchCode.TabIndex = 7;
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
            this.grpContract.Controls.Add(this.lblOpeningPTODays);
            this.grpContract.Controls.Add(this.txtOpeningPTODays);
            this.grpContract.Controls.Add(this.lblOpeningBalanceAsAt);
            this.grpContract.Controls.Add(this.dtpOpeningBalanceAsAt);
            this.grpContract.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpContract.Location = new System.Drawing.Point(12, 12);
            this.grpContract.Name = "grpContract";
            this.grpContract.Size = new System.Drawing.Size(665, 476);
            this.grpContract.TabIndex = 0;
            this.grpContract.TabStop = false;
            this.grpContract.Text = "Contract";
            this.grpContract.Visible = false;
            //
            // lblContractType
            //
            this.lblContractType.AutoSize = true;
            this.lblContractType.Location = new System.Drawing.Point(16, 32);
            this.lblContractType.Name = "lblContractType";
            this.lblContractType.Size = new System.Drawing.Size(88, 13);
            this.lblContractType.TabIndex = 0;
            this.lblContractType.Text = "Contract Type *";
            //
            // txtContractType
            //
            this.txtContractType.Location = new System.Drawing.Point(142, 29);
            this.txtContractType.MaxLength = 30;
            this.txtContractType.Name = "txtContractType";
            this.txtContractType.Size = new System.Drawing.Size(200, 20);
            this.txtContractType.TabIndex = 1;
            //
            // lblStartDate
            //
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(366, 32);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(68, 13);
            this.lblStartDate.TabIndex = 2;
            this.lblStartDate.Text = "Start Date *";
            //
            // dtpStartDate
            //
            this.dtpStartDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(492, 29);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(157, 20);
            this.dtpStartDate.TabIndex = 3;
            //
            // lblDepartment
            //
            this.lblDepartment.AutoSize = true;
            this.lblDepartment.Location = new System.Drawing.Point(16, 66);
            this.lblDepartment.Name = "lblDepartment";
            this.lblDepartment.Size = new System.Drawing.Size(76, 13);
            this.lblDepartment.TabIndex = 4;
            this.lblDepartment.Text = "Department *";
            //
            // txtDepartment
            //
            this.txtDepartment.Location = new System.Drawing.Point(142, 63);
            this.txtDepartment.MaxLength = 100;
            this.txtDepartment.Name = "txtDepartment";
            this.txtDepartment.Size = new System.Drawing.Size(200, 20);
            this.txtDepartment.TabIndex = 5;
            //
            // lblJobDescription
            //
            this.lblJobDescription.AutoSize = true;
            this.lblJobDescription.Location = new System.Drawing.Point(366, 66);
            this.lblJobDescription.Name = "lblJobDescription";
            this.lblJobDescription.Size = new System.Drawing.Size(94, 13);
            this.lblJobDescription.TabIndex = 6;
            this.lblJobDescription.Text = "Job Description *";
            //
            // txtJobDescription
            //
            this.txtJobDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJobDescription.Location = new System.Drawing.Point(492, 63);
            this.txtJobDescription.MaxLength = 100;
            this.txtJobDescription.Name = "txtJobDescription";
            this.txtJobDescription.Size = new System.Drawing.Size(157, 20);
            this.txtJobDescription.TabIndex = 7;
            //
            // lblHourlyRate
            //
            this.lblHourlyRate.AutoSize = true;
            this.lblHourlyRate.Location = new System.Drawing.Point(16, 100);
            this.lblHourlyRate.Name = "lblHourlyRate";
            this.lblHourlyRate.Size = new System.Drawing.Size(70, 13);
            this.lblHourlyRate.TabIndex = 8;
            this.lblHourlyRate.Text = "Hourly Rate *";
            //
            // txtHourlyRate
            //
            this.txtHourlyRate.Location = new System.Drawing.Point(142, 97);
            this.txtHourlyRate.Name = "txtHourlyRate";
            this.txtHourlyRate.Size = new System.Drawing.Size(200, 20);
            this.txtHourlyRate.TabIndex = 9;
            //
            // lblOpeningPTODays
            //
            this.lblOpeningPTODays.AutoSize = true;
            this.lblOpeningPTODays.Location = new System.Drawing.Point(366, 100);
            this.lblOpeningPTODays.Name = "lblOpeningPTODays";
            this.lblOpeningPTODays.Size = new System.Drawing.Size(126, 13);
            this.lblOpeningPTODays.TabIndex = 10;
            this.lblOpeningPTODays.Text = "Opening Balance (days)";
            //
            // txtOpeningPTODays
            //
            this.txtOpeningPTODays.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOpeningPTODays.Location = new System.Drawing.Point(492, 97);
            this.txtOpeningPTODays.Name = "txtOpeningPTODays";
            this.txtOpeningPTODays.Size = new System.Drawing.Size(157, 20);
            this.txtOpeningPTODays.TabIndex = 11;
            //
            // lblOpeningBalanceAsAt
            //
            this.lblOpeningBalanceAsAt.AutoSize = true;
            this.lblOpeningBalanceAsAt.Location = new System.Drawing.Point(16, 134);
            this.lblOpeningBalanceAsAt.Name = "lblOpeningBalanceAsAt";
            this.lblOpeningBalanceAsAt.Size = new System.Drawing.Size(126, 13);
            this.lblOpeningBalanceAsAt.TabIndex = 12;
            this.lblOpeningBalanceAsAt.Text = "Opening Balance As At";
            //
            // dtpOpeningBalanceAsAt
            //
            this.dtpOpeningBalanceAsAt.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOpeningBalanceAsAt.Location = new System.Drawing.Point(142, 131);
            this.dtpOpeningBalanceAsAt.Name = "dtpOpeningBalanceAsAt";
            this.dtpOpeningBalanceAsAt.ShowCheckBox = true;
            this.dtpOpeningBalanceAsAt.Size = new System.Drawing.Size(200, 20);
            this.dtpOpeningBalanceAsAt.TabIndex = 13;
            //
            // grpSpouse
            //
            this.grpSpouse.Controls.Add(this.lblSpouseName);
            this.grpSpouse.Controls.Add(this.txtSpouseName);
            this.grpSpouse.Controls.Add(this.lblSpouseMobileNumber);
            this.grpSpouse.Controls.Add(this.txtSpouseMobileNumber);
            this.grpSpouse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSpouse.Location = new System.Drawing.Point(12, 12);
            this.grpSpouse.Name = "grpSpouse";
            this.grpSpouse.Size = new System.Drawing.Size(665, 476);
            this.grpSpouse.TabIndex = 0;
            this.grpSpouse.TabStop = false;
            this.grpSpouse.Text = "Spouse";
            this.grpSpouse.Visible = false;
            //
            // lblSpouseName
            //
            this.lblSpouseName.AutoSize = true;
            this.lblSpouseName.Location = new System.Drawing.Point(16, 32);
            this.lblSpouseName.Name = "lblSpouseName";
            this.lblSpouseName.Size = new System.Drawing.Size(82, 13);
            this.lblSpouseName.TabIndex = 0;
            this.lblSpouseName.Text = "Spouse Name *";
            //
            // txtSpouseName
            //
            this.txtSpouseName.Location = new System.Drawing.Point(142, 29);
            this.txtSpouseName.MaxLength = 200;
            this.txtSpouseName.Name = "txtSpouseName";
            this.txtSpouseName.Size = new System.Drawing.Size(200, 20);
            this.txtSpouseName.TabIndex = 1;
            //
            // lblSpouseMobileNumber
            //
            this.lblSpouseMobileNumber.AutoSize = true;
            this.lblSpouseMobileNumber.Location = new System.Drawing.Point(366, 32);
            this.lblSpouseMobileNumber.Name = "lblSpouseMobileNumber";
            this.lblSpouseMobileNumber.Size = new System.Drawing.Size(123, 13);
            this.lblSpouseMobileNumber.TabIndex = 2;
            this.lblSpouseMobileNumber.Text = "Spouse Mobile No. *";
            //
            // txtSpouseMobileNumber
            //
            this.txtSpouseMobileNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpouseMobileNumber.Location = new System.Drawing.Point(492, 29);
            this.txtSpouseMobileNumber.MaxLength = 20;
            this.txtSpouseMobileNumber.Name = "txtSpouseMobileNumber";
            this.txtSpouseMobileNumber.Size = new System.Drawing.Size(157, 20);
            this.txtSpouseMobileNumber.TabIndex = 3;
            //
            // grpFamily
            //
            this.grpFamily.Controls.Add(this.dgvFamily);
            this.grpFamily.Controls.Add(this.pnlFamilyButtons);
            this.grpFamily.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpFamily.Location = new System.Drawing.Point(12, 12);
            this.grpFamily.Name = "grpFamily";
            this.grpFamily.Size = new System.Drawing.Size(665, 476);
            this.grpFamily.TabIndex = 0;
            this.grpFamily.TabStop = false;
            this.grpFamily.Text = "Family Contacts";
            this.grpFamily.Visible = false;
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
            this.dgvFamily.Size = new System.Drawing.Size(539, 457);
            this.dgvFamily.TabIndex = 0;
            //
            // colFamilyName
            //
            this.colFamilyName.HeaderText = "Name *";
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
            this.colFamilyRelationship.HeaderText = "Relationship *";
            this.colFamilyRelationship.MaxInputLength = 50;
            this.colFamilyRelationship.Name = "colFamilyRelationship";
            this.colFamilyRelationship.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // pnlFamilyButtons
            //
            this.pnlFamilyButtons.Controls.Add(this.btnAddFamilyRow);
            this.pnlFamilyButtons.Controls.Add(this.btnRemoveFamilyRow);
            this.pnlFamilyButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlFamilyButtons.Location = new System.Drawing.Point(542, 16);
            this.pnlFamilyButtons.Name = "pnlFamilyButtons";
            this.pnlFamilyButtons.Size = new System.Drawing.Size(120, 457);
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
            // pnlSectionList
            //
            this.pnlSectionList.Controls.Add(this.btnSectionFamily);
            this.pnlSectionList.Controls.Add(this.btnSectionSpouse);
            this.pnlSectionList.Controls.Add(this.btnSectionContract);
            this.pnlSectionList.Controls.Add(this.btnSectionBank);
            this.pnlSectionList.Controls.Add(this.btnSectionAddress);
            this.pnlSectionList.Controls.Add(this.btnSectionPersonal);
            this.pnlSectionList.Controls.Add(this.btnBackToList);
            this.pnlSectionList.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSectionList.Location = new System.Drawing.Point(0, 0);
            this.pnlSectionList.Name = "pnlSectionList";
            this.pnlSectionList.Size = new System.Drawing.Size(200, 500);
            this.pnlSectionList.TabIndex = 0;
            //
            // btnBackToList
            //
            this.btnBackToList.Location = new System.Drawing.Point(0, 16);
            this.btnBackToList.Name = "btnBackToList";
            this.btnBackToList.Size = new System.Drawing.Size(200, 32);
            this.btnBackToList.TabIndex = 0;
            this.btnBackToList.Text = "Back to list";
            this.btnBackToList.Click += new System.EventHandler(this.btnBackToList_Click);
            //
            // btnSectionPersonal
            //
            this.btnSectionPersonal.Location = new System.Drawing.Point(0, 64);
            this.btnSectionPersonal.Name = "btnSectionPersonal";
            this.btnSectionPersonal.Size = new System.Drawing.Size(200, 40);
            this.btnSectionPersonal.TabIndex = 1;
            this.btnSectionPersonal.Text = "Personal";
            this.btnSectionPersonal.Click += new System.EventHandler(this.btnSectionPersonal_Click);
            //
            // btnSectionAddress
            //
            this.btnSectionAddress.Location = new System.Drawing.Point(0, 104);
            this.btnSectionAddress.Name = "btnSectionAddress";
            this.btnSectionAddress.Size = new System.Drawing.Size(200, 40);
            this.btnSectionAddress.TabIndex = 2;
            this.btnSectionAddress.Text = "Address";
            this.btnSectionAddress.Click += new System.EventHandler(this.btnSectionAddress_Click);
            //
            // btnSectionBank
            //
            this.btnSectionBank.Location = new System.Drawing.Point(0, 144);
            this.btnSectionBank.Name = "btnSectionBank";
            this.btnSectionBank.Size = new System.Drawing.Size(200, 40);
            this.btnSectionBank.TabIndex = 3;
            this.btnSectionBank.Text = "Bank";
            this.btnSectionBank.Click += new System.EventHandler(this.btnSectionBank_Click);
            //
            // btnSectionContract
            //
            this.btnSectionContract.Location = new System.Drawing.Point(0, 184);
            this.btnSectionContract.Name = "btnSectionContract";
            this.btnSectionContract.Size = new System.Drawing.Size(200, 40);
            this.btnSectionContract.TabIndex = 4;
            this.btnSectionContract.Text = "Contract";
            this.btnSectionContract.Click += new System.EventHandler(this.btnSectionContract_Click);
            //
            // btnSectionSpouse
            //
            this.btnSectionSpouse.Location = new System.Drawing.Point(0, 224);
            this.btnSectionSpouse.Name = "btnSectionSpouse";
            this.btnSectionSpouse.Size = new System.Drawing.Size(200, 40);
            this.btnSectionSpouse.TabIndex = 5;
            this.btnSectionSpouse.Text = "Spouse";
            this.btnSectionSpouse.Click += new System.EventHandler(this.btnSectionSpouse_Click);
            //
            // btnSectionFamily
            //
            this.btnSectionFamily.Location = new System.Drawing.Point(0, 264);
            this.btnSectionFamily.Name = "btnSectionFamily";
            this.btnSectionFamily.Size = new System.Drawing.Size(200, 40);
            this.btnSectionFamily.TabIndex = 6;
            this.btnSectionFamily.Text = "Family contacts";
            this.btnSectionFamily.Click += new System.EventHandler(this.btnSectionFamily_Click);
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
            this.pnlSplit.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlList.ResumeLayout(false);
            this.grpPersonal.ResumeLayout(false);
            this.grpPersonal.PerformLayout();
            this.grpAddress.ResumeLayout(false);
            this.grpAddress.PerformLayout();
            this.grpBank.ResumeLayout(false);
            this.grpBank.PerformLayout();
            this.grpContract.ResumeLayout(false);
            this.grpContract.PerformLayout();
            this.grpSpouse.ResumeLayout(false);
            this.grpSpouse.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamily)).EndInit();
            this.pnlFamilyButtons.ResumeLayout(false);
            this.grpFamily.ResumeLayout(false);
            this.pnlSectionContent.ResumeLayout(false);
            this.pnlSectionList.ResumeLayout(false);
            this.pnlSectionBody.ResumeLayout(false);
            this.pnlEditorButtons.ResumeLayout(false);
            this.pnlEditor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlList;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblViewTitle;
        private System.Windows.Forms.Label lblStore;
        private System.Windows.Forms.Panel pnlSplit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label lblStatus;
        private EmployeeTimeManagement.Views.EmployeePickerControl employeePicker;
        private EmployeeTimeManagement.Views.EmployeeDetailsPanel employeeDetails;
        private System.Windows.Forms.Panel pnlEditor;
        private System.Windows.Forms.Label lblEditorTitle;
        private System.Windows.Forms.Panel pnlSectionBody;
        private System.Windows.Forms.Panel pnlSectionContent;
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
        private System.Windows.Forms.Label lblOpeningPTODays;
        private System.Windows.Forms.TextBox txtOpeningPTODays;
        private System.Windows.Forms.Label lblOpeningBalanceAsAt;
        private System.Windows.Forms.DateTimePicker dtpOpeningBalanceAsAt;
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
        private System.Windows.Forms.Panel pnlSectionList;
        private System.Windows.Forms.Button btnBackToList;
        private System.Windows.Forms.Button btnSectionPersonal;
        private System.Windows.Forms.Button btnSectionAddress;
        private System.Windows.Forms.Button btnSectionBank;
        private System.Windows.Forms.Button btnSectionContract;
        private System.Windows.Forms.Button btnSectionSpouse;
        private System.Windows.Forms.Button btnSectionFamily;
        private System.Windows.Forms.Panel pnlEditorButtons;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblEditorStatus;
        private System.Windows.Forms.ToolTip tipEditorErrors;
    }
}
