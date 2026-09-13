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
            this.pnlList.SuspendLayout();
            this.pnlListTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
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
            this.pnlList.Size = new System.Drawing.Size(800, 450);
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
            this.dgvEmployees.Size = new System.Drawing.Size(800, 347);
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
            this.lblStatus.Location = new System.Drawing.Point(0, 427);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(800, 23);
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
            this.pnlListTop.Size = new System.Drawing.Size(800, 80);
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
            // EmployeesForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnlList);
            this.Name = "EmployeesForm";
            this.Text = "EmployeesForm";
            this.pnlListTop.ResumeLayout(false);
            this.pnlListTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            this.pnlList.ResumeLayout(false);
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
    }
}
