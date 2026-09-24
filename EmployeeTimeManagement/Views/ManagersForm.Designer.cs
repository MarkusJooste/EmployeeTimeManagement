namespace EmployeeTimeManagement.Views
{
    partial class ManagersForm
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
            this.pnlManagers = new System.Windows.Forms.Panel();
            this.lblManagersEmpty = new System.Windows.Forms.Label();
            this.dgvManagers = new System.Windows.Forms.DataGridView();
            this.colMgrName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMgrSurname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromotedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromotedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReveal = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lblManagersHeader = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnDemote = new System.Windows.Forms.Button();
            this.btnPromote = new System.Windows.Forms.Button();
            this.pnlEmployees = new System.Windows.Forms.Panel();
            this.lblEmployeesEmpty = new System.Windows.Forms.Label();
            this.dgvEmployees = new System.Windows.Forms.DataGridView();
            this.colEmpName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmpSurname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblEmployeesHeader = new System.Windows.Forms.Label();
            this.pnlManagers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvManagers)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.pnlEmployees.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
            this.SuspendLayout();
            //
            // pnlManagers
            //
            this.pnlManagers.Controls.Add(this.lblManagersEmpty);
            this.pnlManagers.Controls.Add(this.dgvManagers);
            this.pnlManagers.Controls.Add(this.lblManagersHeader);
            this.pnlManagers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlManagers.Location = new System.Drawing.Point(520, 0);
            this.pnlManagers.Name = "pnlManagers";
            this.pnlManagers.Size = new System.Drawing.Size(480, 520);
            this.pnlManagers.TabIndex = 2;
            //
            // lblManagersEmpty
            //
            this.lblManagersEmpty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblManagersEmpty.Location = new System.Drawing.Point(0, 30);
            this.lblManagersEmpty.Name = "lblManagersEmpty";
            this.lblManagersEmpty.Size = new System.Drawing.Size(480, 490);
            this.lblManagersEmpty.TabIndex = 2;
            this.lblManagersEmpty.Text = "No managers yet.";
            this.lblManagersEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // dgvManagers
            //
            this.dgvManagers.AllowUserToAddRows = false;
            this.dgvManagers.AllowUserToDeleteRows = false;
            this.dgvManagers.AutoGenerateColumns = false;
            this.dgvManagers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvManagers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvManagers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMgrName,
            this.colMgrSurname,
            this.colPin,
            this.colPromotedOn,
            this.colPromotedBy,
            this.colReveal});
            this.dgvManagers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvManagers.Location = new System.Drawing.Point(0, 30);
            this.dgvManagers.MultiSelect = false;
            this.dgvManagers.Name = "dgvManagers";
            this.dgvManagers.ReadOnly = true;
            this.dgvManagers.RowHeadersVisible = false;
            this.dgvManagers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvManagers.Size = new System.Drawing.Size(480, 490);
            this.dgvManagers.TabIndex = 1;
            this.dgvManagers.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvManagers_CellContentClick);
            this.dgvManagers.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvManagers_CellFormatting);
            this.dgvManagers.SelectionChanged += new System.EventHandler(this.dgvManagers_SelectionChanged);
            //
            // colMgrName
            //
            this.colMgrName.DataPropertyName = "Name";
            this.colMgrName.HeaderText = "Name";
            this.colMgrName.Name = "colMgrName";
            this.colMgrName.ReadOnly = true;
            this.colMgrName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colMgrSurname
            //
            this.colMgrSurname.DataPropertyName = "Surname";
            this.colMgrSurname.HeaderText = "Surname";
            this.colMgrSurname.Name = "colMgrSurname";
            this.colMgrSurname.ReadOnly = true;
            this.colMgrSurname.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colPin
            //
            this.colPin.DataPropertyName = "Pin";
            this.colPin.HeaderText = "PIN";
            this.colPin.Name = "colPin";
            this.colPin.ReadOnly = true;
            this.colPin.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colPromotedOn
            //
            this.colPromotedOn.DataPropertyName = "PromotedOn";
            this.colPromotedOn.DefaultCellStyle.Format = "dd MMM yyyy";
            this.colPromotedOn.HeaderText = "Promoted On";
            this.colPromotedOn.Name = "colPromotedOn";
            this.colPromotedOn.ReadOnly = true;
            this.colPromotedOn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colPromotedBy
            //
            this.colPromotedBy.DataPropertyName = "PromotedByName";
            this.colPromotedBy.HeaderText = "Promoted By";
            this.colPromotedBy.Name = "colPromotedBy";
            this.colPromotedBy.ReadOnly = true;
            this.colPromotedBy.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colReveal
            //
            this.colReveal.HeaderText = "";
            this.colReveal.Name = "colReveal";
            this.colReveal.ReadOnly = true;
            this.colReveal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colReveal.Text = "Reveal";
            this.colReveal.UseColumnTextForButtonValue = true;
            //
            // lblManagersHeader
            //
            this.lblManagersHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblManagersHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblManagersHeader.Location = new System.Drawing.Point(0, 0);
            this.lblManagersHeader.Name = "lblManagersHeader";
            this.lblManagersHeader.Padding = new System.Windows.Forms.Padding(4);
            this.lblManagersHeader.Size = new System.Drawing.Size(480, 30);
            this.lblManagersHeader.TabIndex = 0;
            this.lblManagersHeader.Text = "Managers";
            //
            // pnlButtons
            //
            this.pnlButtons.Controls.Add(this.btnDemote);
            this.pnlButtons.Controls.Add(this.btnPromote);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlButtons.Location = new System.Drawing.Point(380, 0);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(140, 520);
            this.pnlButtons.TabIndex = 1;
            //
            // btnDemote
            //
            this.btnDemote.Enabled = false;
            this.btnDemote.Location = new System.Drawing.Point(15, 260);
            this.btnDemote.Name = "btnDemote";
            this.btnDemote.Size = new System.Drawing.Size(110, 30);
            this.btnDemote.TabIndex = 1;
            this.btnDemote.Text = "< Demote";
            this.btnDemote.UseVisualStyleBackColor = true;
            this.btnDemote.Click += new System.EventHandler(this.btnDemote_Click);
            //
            // btnPromote
            //
            this.btnPromote.Enabled = false;
            this.btnPromote.Location = new System.Drawing.Point(15, 220);
            this.btnPromote.Name = "btnPromote";
            this.btnPromote.Size = new System.Drawing.Size(110, 30);
            this.btnPromote.TabIndex = 0;
            this.btnPromote.Text = "Promote >";
            this.btnPromote.UseVisualStyleBackColor = true;
            this.btnPromote.Click += new System.EventHandler(this.btnPromote_Click);
            //
            // pnlEmployees
            //
            this.pnlEmployees.Controls.Add(this.lblEmployeesEmpty);
            this.pnlEmployees.Controls.Add(this.dgvEmployees);
            this.pnlEmployees.Controls.Add(this.lblEmployeesHeader);
            this.pnlEmployees.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlEmployees.Location = new System.Drawing.Point(0, 0);
            this.pnlEmployees.Name = "pnlEmployees";
            this.pnlEmployees.Size = new System.Drawing.Size(380, 520);
            this.pnlEmployees.TabIndex = 0;
            //
            // lblEmployeesEmpty
            //
            this.lblEmployeesEmpty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEmployeesEmpty.Location = new System.Drawing.Point(0, 30);
            this.lblEmployeesEmpty.Name = "lblEmployeesEmpty";
            this.lblEmployeesEmpty.Size = new System.Drawing.Size(380, 490);
            this.lblEmployeesEmpty.TabIndex = 2;
            this.lblEmployeesEmpty.Text = "Every employee at this store is already a manager.";
            this.lblEmployeesEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // dgvEmployees
            //
            this.dgvEmployees.AllowUserToAddRows = false;
            this.dgvEmployees.AllowUserToDeleteRows = false;
            this.dgvEmployees.AutoGenerateColumns = false;
            this.dgvEmployees.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEmployees.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEmployees.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEmpName,
            this.colEmpSurname});
            this.dgvEmployees.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmployees.Location = new System.Drawing.Point(0, 30);
            this.dgvEmployees.MultiSelect = false;
            this.dgvEmployees.Name = "dgvEmployees";
            this.dgvEmployees.ReadOnly = true;
            this.dgvEmployees.RowHeadersVisible = false;
            this.dgvEmployees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployees.Size = new System.Drawing.Size(380, 490);
            this.dgvEmployees.TabIndex = 1;
            this.dgvEmployees.SelectionChanged += new System.EventHandler(this.dgvEmployees_SelectionChanged);
            //
            // colEmpName
            //
            this.colEmpName.DataPropertyName = "Name";
            this.colEmpName.HeaderText = "Name";
            this.colEmpName.Name = "colEmpName";
            this.colEmpName.ReadOnly = true;
            this.colEmpName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colEmpSurname
            //
            this.colEmpSurname.DataPropertyName = "Surname";
            this.colEmpSurname.HeaderText = "Surname";
            this.colEmpSurname.Name = "colEmpSurname";
            this.colEmpSurname.ReadOnly = true;
            this.colEmpSurname.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // lblEmployeesHeader
            //
            this.lblEmployeesHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblEmployeesHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblEmployeesHeader.Location = new System.Drawing.Point(0, 0);
            this.lblEmployeesHeader.Name = "lblEmployeesHeader";
            this.lblEmployeesHeader.Padding = new System.Windows.Forms.Padding(4);
            this.lblEmployeesHeader.Size = new System.Drawing.Size(380, 30);
            this.lblEmployeesHeader.TabIndex = 0;
            this.lblEmployeesHeader.Text = "Employees";
            //
            // ManagersForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 520);
            this.Controls.Add(this.pnlManagers);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlEmployees);
            this.Name = "ManagersForm";
            this.Text = "Managers";
            this.pnlManagers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvManagers)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.pnlEmployees.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlManagers;
        private System.Windows.Forms.Label lblManagersHeader;
        private System.Windows.Forms.DataGridView dgvManagers;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMgrName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMgrSurname;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPin;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromotedOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromotedBy;
        private System.Windows.Forms.DataGridViewButtonColumn colReveal;
        private System.Windows.Forms.Label lblManagersEmpty;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnDemote;
        private System.Windows.Forms.Button btnPromote;
        private System.Windows.Forms.Panel pnlEmployees;
        private System.Windows.Forms.Label lblEmployeesHeader;
        private System.Windows.Forms.DataGridView dgvEmployees;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmpName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmpSurname;
        private System.Windows.Forms.Label lblEmployeesEmpty;
    }
}
