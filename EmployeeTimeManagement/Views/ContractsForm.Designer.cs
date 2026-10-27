namespace EmployeeTimeManagement.Views
{
    partial class ContractsForm
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblHeading = new System.Windows.Forms.Label();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.lblTemplate = new System.Windows.Forms.Label();
            this.cboTemplate = new System.Windows.Forms.ComboBox();
            this.lblNextOfKin = new System.Windows.Forms.Label();
            this.cboNextOfKin = new System.Windows.Forms.ComboBox();
            this.lblSummary = new System.Windows.Forms.Label();
            this.txtSummary = new System.Windows.Forms.TextBox();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnSavePdf = new System.Windows.Forms.Button();
            this.dgvEmployees = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSurname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStartDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlTop.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
            this.SuspendLayout();
            //
            // pnlTop
            //
            this.pnlTop.Controls.Add(this.lblHeading);
            this.pnlTop.Controls.Add(this.lblSearch);
            this.pnlTop.Controls.Add(this.txtSearch);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(800, 50);
            this.pnlTop.TabIndex = 0;
            //
            // lblHeading
            //
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.Location = new System.Drawing.Point(12, 14);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(165, 20);
            this.lblHeading.TabIndex = 0;
            this.lblHeading.Text = "Employee Contracts";
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(220, 19);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(87, 13);
            this.lblSearch.TabIndex = 1;
            this.lblSearch.Text = "Search employee";
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(313, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(180, 20);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            //
            // pnlOptions
            //
            this.pnlOptions.Controls.Add(this.lblTemplate);
            this.pnlOptions.Controls.Add(this.cboTemplate);
            this.pnlOptions.Controls.Add(this.lblNextOfKin);
            this.pnlOptions.Controls.Add(this.cboNextOfKin);
            this.pnlOptions.Controls.Add(this.lblSummary);
            this.pnlOptions.Controls.Add(this.txtSummary);
            this.pnlOptions.Controls.Add(this.btnPreview);
            this.pnlOptions.Controls.Add(this.btnPrint);
            this.pnlOptions.Controls.Add(this.btnSavePdf);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlOptions.Location = new System.Drawing.Point(500, 50);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(300, 377);
            this.pnlOptions.TabIndex = 2;
            //
            // lblTemplate
            //
            this.lblTemplate.AutoSize = true;
            this.lblTemplate.Location = new System.Drawing.Point(12, 8);
            this.lblTemplate.Name = "lblTemplate";
            this.lblTemplate.Size = new System.Drawing.Size(92, 13);
            this.lblTemplate.TabIndex = 0;
            this.lblTemplate.Text = "Contract template";
            //
            // cboTemplate
            //
            this.cboTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTemplate.FormattingEnabled = true;
            this.cboTemplate.Location = new System.Drawing.Point(12, 25);
            this.cboTemplate.Name = "cboTemplate";
            this.cboTemplate.Size = new System.Drawing.Size(276, 21);
            this.cboTemplate.TabIndex = 1;
            this.cboTemplate.SelectedIndexChanged += new System.EventHandler(this.cboTemplate_SelectedIndexChanged);
            //
            // lblNextOfKin
            //
            this.lblNextOfKin.AutoSize = true;
            this.lblNextOfKin.Location = new System.Drawing.Point(12, 55);
            this.lblNextOfKin.Name = "lblNextOfKin";
            this.lblNextOfKin.Size = new System.Drawing.Size(59, 13);
            this.lblNextOfKin.TabIndex = 2;
            this.lblNextOfKin.Text = "Next of kin";
            //
            // cboNextOfKin
            //
            this.cboNextOfKin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboNextOfKin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNextOfKin.FormattingEnabled = true;
            this.cboNextOfKin.Location = new System.Drawing.Point(12, 72);
            this.cboNextOfKin.Name = "cboNextOfKin";
            this.cboNextOfKin.Size = new System.Drawing.Size(276, 21);
            this.cboNextOfKin.TabIndex = 3;
            this.cboNextOfKin.SelectedIndexChanged += new System.EventHandler(this.cboNextOfKin_SelectedIndexChanged);
            //
            // lblSummary
            //
            this.lblSummary.AutoSize = true;
            this.lblSummary.Location = new System.Drawing.Point(12, 102);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(124, 13);
            this.lblSummary.TabIndex = 4;
            this.lblSummary.Text = "What will be filled in";
            //
            // txtSummary
            //
            this.txtSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSummary.BackColor = System.Drawing.SystemColors.Window;
            this.txtSummary.Location = new System.Drawing.Point(12, 119);
            this.txtSummary.Multiline = true;
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ReadOnly = true;
            this.txtSummary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSummary.Size = new System.Drawing.Size(276, 205);
            this.txtSummary.TabIndex = 5;
            this.txtSummary.TabStop = false;
            //
            // btnPreview
            //
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPreview.Location = new System.Drawing.Point(12, 334);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(88, 30);
            this.btnPreview.TabIndex = 6;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            //
            // btnPrint
            //
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrint.Location = new System.Drawing.Point(106, 334);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(88, 30);
            this.btnPrint.TabIndex = 7;
            this.btnPrint.Text = "Print...";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            //
            // btnSavePdf
            //
            this.btnSavePdf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSavePdf.Location = new System.Drawing.Point(200, 334);
            this.btnSavePdf.Name = "btnSavePdf";
            this.btnSavePdf.Size = new System.Drawing.Size(88, 30);
            this.btnSavePdf.TabIndex = 8;
            this.btnSavePdf.Text = "Save as PDF...";
            this.btnSavePdf.UseVisualStyleBackColor = true;
            this.btnSavePdf.Click += new System.EventHandler(this.btnSavePdf_Click);
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
            this.colStore,
            this.colPosition,
            this.colStartDate});
            this.dgvEmployees.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmployees.Location = new System.Drawing.Point(0, 50);
            this.dgvEmployees.MultiSelect = false;
            this.dgvEmployees.Name = "dgvEmployees";
            this.dgvEmployees.ReadOnly = true;
            this.dgvEmployees.RowHeadersVisible = false;
            this.dgvEmployees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployees.Size = new System.Drawing.Size(500, 377);
            this.dgvEmployees.TabIndex = 1;
            this.dgvEmployees.SelectionChanged += new System.EventHandler(this.dgvEmployees_SelectionChanged);
            this.dgvEmployees.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvEmployees_DataBindingComplete);
            //
            // colName
            //
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            //
            // colSurname
            //
            this.colSurname.DataPropertyName = "Surname";
            this.colSurname.HeaderText = "Surname";
            this.colSurname.Name = "colSurname";
            this.colSurname.ReadOnly = true;
            //
            // colStore
            //
            this.colStore.DataPropertyName = "StoreName";
            this.colStore.HeaderText = "Store";
            this.colStore.Name = "colStore";
            this.colStore.ReadOnly = true;
            //
            // colPosition
            //
            this.colPosition.DataPropertyName = "Position";
            this.colPosition.HeaderText = "Position";
            this.colPosition.Name = "colPosition";
            this.colPosition.ReadOnly = true;
            //
            // colStartDate
            //
            this.colStartDate.DataPropertyName = "StartDate";
            this.colStartDate.DefaultCellStyle.Format = "dd MMM yyyy";
            this.colStartDate.HeaderText = "Start date";
            this.colStartDate.Name = "colStartDate";
            this.colStartDate.ReadOnly = true;
            //
            // lblStatus
            //
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Location = new System.Drawing.Point(0, 427);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblStatus.Size = new System.Drawing.Size(800, 23);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ContractsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvEmployees);
            this.Controls.Add(this.pnlOptions);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.lblStatus);
            this.Name = "ContractsForm";
            this.Text = "Contracts";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.Label lblTemplate;
        private System.Windows.Forms.ComboBox cboTemplate;
        private System.Windows.Forms.Label lblNextOfKin;
        private System.Windows.Forms.ComboBox cboNextOfKin;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.TextBox txtSummary;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnSavePdf;
        private System.Windows.Forms.DataGridView dgvEmployees;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSurname;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStore;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStartDate;
        private System.Windows.Forms.Label lblStatus;
    }
}
