namespace EmployeeTimeManagement.Views
{
    partial class EmployeePickerControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearchHint = new System.Windows.Forms.Label();
            this.pnlTabs = new System.Windows.Forms.FlowLayoutPanel();
            this.btnScopeActive = new System.Windows.Forms.Button();
            this.btnScopeFormer = new System.Windows.Forms.Button();
            this.btnScopeAll = new System.Windows.Forms.Button();
            this.dgvEmployees = new System.Windows.Forms.DataGridView();
            this.colEmployee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblEmpty = new System.Windows.Forms.Label();
            this.pnlTop.SuspendLayout();
            this.pnlTabs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
            this.SuspendLayout();
            //
            // pnlTop
            //
            this.pnlTop.Controls.Add(this.txtSearch);
            this.pnlTop.Controls.Add(this.lblSearchHint);
            this.pnlTop.Controls.Add(this.pnlTabs);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.pnlTop.Size = new System.Drawing.Size(889, 100);
            this.pnlTop.TabIndex = 0;
            //
            // txtSearch
            //
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSearch.Location = new System.Drawing.Point(12, 62);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(865, 20);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            //
            // lblSearchHint
            //
            this.lblSearchHint.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSearchHint.Location = new System.Drawing.Point(12, 46);
            this.lblSearchHint.Name = "lblSearchHint";
            this.lblSearchHint.Size = new System.Drawing.Size(865, 16);
            this.lblSearchHint.TabIndex = 2;
            this.lblSearchHint.Text = "Search name, ID number, mobile or job";
            //
            // pnlTabs
            //
            this.pnlTabs.Controls.Add(this.btnScopeActive);
            this.pnlTabs.Controls.Add(this.btnScopeFormer);
            this.pnlTabs.Controls.Add(this.btnScopeAll);
            this.pnlTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTabs.Location = new System.Drawing.Point(12, 10);
            this.pnlTabs.Name = "pnlTabs";
            this.pnlTabs.Size = new System.Drawing.Size(865, 34);
            this.pnlTabs.TabIndex = 0;
            this.pnlTabs.WrapContents = false;
            //
            // btnScopeActive
            //
            this.btnScopeActive.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnScopeActive.Name = "btnScopeActive";
            this.btnScopeActive.Size = new System.Drawing.Size(92, 28);
            this.btnScopeActive.TabIndex = 0;
            this.btnScopeActive.Text = "Active";
            this.btnScopeActive.Click += new System.EventHandler(this.btnScopeActive_Click);
            //
            // btnScopeFormer
            //
            this.btnScopeFormer.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnScopeFormer.Name = "btnScopeFormer";
            this.btnScopeFormer.Size = new System.Drawing.Size(92, 28);
            this.btnScopeFormer.TabIndex = 1;
            this.btnScopeFormer.Text = "Former";
            this.btnScopeFormer.Click += new System.EventHandler(this.btnScopeFormer_Click);
            //
            // btnScopeAll
            //
            this.btnScopeAll.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnScopeAll.Name = "btnScopeAll";
            this.btnScopeAll.Size = new System.Drawing.Size(92, 28);
            this.btnScopeAll.TabIndex = 2;
            this.btnScopeAll.Text = "All";
            this.btnScopeAll.Click += new System.EventHandler(this.btnScopeAll_Click);
            //
            // dgvEmployees
            //
            this.dgvEmployees.AllowUserToAddRows = false;
            this.dgvEmployees.AllowUserToDeleteRows = false;
            this.dgvEmployees.AllowUserToResizeColumns = false;
            this.dgvEmployees.AllowUserToResizeRows = false;
            this.dgvEmployees.AutoGenerateColumns = false;
            this.dgvEmployees.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEmployees.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvEmployees.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvEmployees.ColumnHeadersVisible = false;
            this.dgvEmployees.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEmployee});
            this.dgvEmployees.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmployees.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvEmployees.Location = new System.Drawing.Point(0, 100);
            this.dgvEmployees.MultiSelect = false;
            this.dgvEmployees.Name = "dgvEmployees";
            this.dgvEmployees.ReadOnly = true;
            this.dgvEmployees.RowHeadersVisible = false;
            this.dgvEmployees.RowTemplate.Height = 52;
            this.dgvEmployees.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvEmployees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployees.Size = new System.Drawing.Size(889, 451);
            this.dgvEmployees.TabIndex = 1;
            this.dgvEmployees.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEmployees_CellDoubleClick);
            this.dgvEmployees.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvEmployees_CellPainting);
            this.dgvEmployees.SelectionChanged += new System.EventHandler(this.dgvEmployees_SelectionChanged);
            this.dgvEmployees.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvEmployees_KeyDown);
            //
            // colEmployee
            //
            this.colEmployee.DataPropertyName = "FullName";
            this.colEmployee.HeaderText = "Employee";
            this.colEmployee.Name = "colEmployee";
            this.colEmployee.ReadOnly = true;
            this.colEmployee.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // lblEmpty
            //
            this.lblEmpty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEmpty.Location = new System.Drawing.Point(0, 100);
            this.lblEmpty.Name = "lblEmpty";
            this.lblEmpty.Padding = new System.Windows.Forms.Padding(16);
            this.lblEmpty.Size = new System.Drawing.Size(889, 451);
            this.lblEmpty.TabIndex = 2;
            this.lblEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEmpty.Visible = false;
            //
            // EmployeePickerControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblEmpty);
            this.Controls.Add(this.dgvEmployees);
            this.Controls.Add(this.pnlTop);
            this.Name = "EmployeePickerControl";
            this.Size = new System.Drawing.Size(889, 551);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlTabs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.FlowLayoutPanel pnlTabs;
        private System.Windows.Forms.Button btnScopeActive;
        private System.Windows.Forms.Button btnScopeFormer;
        private System.Windows.Forms.Button btnScopeAll;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearchHint;
        private System.Windows.Forms.DataGridView dgvEmployees;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmployee;
        private System.Windows.Forms.Label lblEmpty;
    }
}
