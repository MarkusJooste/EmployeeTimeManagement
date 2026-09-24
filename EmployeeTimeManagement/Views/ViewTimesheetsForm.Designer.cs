namespace EmployeeTimeManagement.Views
{
    partial class ViewTimesheetsForm
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
            this.lblPeriod = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.btnApplyRange = new System.Windows.Forms.Button();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.btnMonthly = new System.Windows.Forms.Button();
            this.btnWeekly = new System.Windows.Forms.Button();
            this.btnDaily = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.dgvTimesheets = new System.Windows.Forms.DataGridView();
            this.btnPrint = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimesheets)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.btnPrint);
            this.pnlTop.Controls.Add(this.lblPeriod);
            this.pnlTop.Controls.Add(this.txtSearch);
            this.pnlTop.Controls.Add(this.lblSearch);
            this.pnlTop.Controls.Add(this.btnApplyRange);
            this.pnlTop.Controls.Add(this.dtpTo);
            this.pnlTop.Controls.Add(this.dtpFrom);
            this.pnlTop.Controls.Add(this.btnMonthly);
            this.pnlTop.Controls.Add(this.btnWeekly);
            this.pnlTop.Controls.Add(this.btnDaily);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(800, 80);
            this.pnlTop.TabIndex = 0;
            // 
            // lblPeriod
            // 
            this.lblPeriod.AutoSize = true;
            this.lblPeriod.Location = new System.Drawing.Point(268, 15);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(47, 13);
            this.lblPeriod.TabIndex = 8;
            this.lblPeriod.Text = "lblPeriod";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(438, 45);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(180, 20);
            this.txtSearch.TabIndex = 7;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(345, 49);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(89, 13);
            this.lblSearch.TabIndex = 6;
            this.lblSearch.Text = "Search employee";
            // 
            // btnApplyRange
            // 
            this.btnApplyRange.Location = new System.Drawing.Point(244, 43);
            this.btnApplyRange.Name = "btnApplyRange";
            this.btnApplyRange.Size = new System.Drawing.Size(75, 25);
            this.btnApplyRange.TabIndex = 5;
            this.btnApplyRange.Text = "Apply";
            this.btnApplyRange.UseVisualStyleBackColor = true;
            this.btnApplyRange.Click += new System.EventHandler(this.btnApplyRange_Click);
            // 
            // dtpTo
            // 
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(128, 45);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(110, 20);
            this.dtpTo.TabIndex = 4;
            // 
            // dtpFrom
            // 
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(12, 45);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(110, 20);
            this.dtpFrom.TabIndex = 3;
            // 
            // btnMonthly
            // 
            this.btnMonthly.Location = new System.Drawing.Point(174, 9);
            this.btnMonthly.Name = "btnMonthly";
            this.btnMonthly.Size = new System.Drawing.Size(75, 25);
            this.btnMonthly.TabIndex = 2;
            this.btnMonthly.Text = "Monthly";
            this.btnMonthly.UseVisualStyleBackColor = true;
            this.btnMonthly.Click += new System.EventHandler(this.btnMonthly_Click);
            // 
            // btnWeekly
            // 
            this.btnWeekly.Location = new System.Drawing.Point(93, 9);
            this.btnWeekly.Name = "btnWeekly";
            this.btnWeekly.Size = new System.Drawing.Size(75, 25);
            this.btnWeekly.TabIndex = 1;
            this.btnWeekly.Text = "Weekly";
            this.btnWeekly.UseVisualStyleBackColor = true;
            this.btnWeekly.Click += new System.EventHandler(this.btnWeekly_Click);
            // 
            // btnDaily
            // 
            this.btnDaily.Location = new System.Drawing.Point(12, 9);
            this.btnDaily.Name = "btnDaily";
            this.btnDaily.Size = new System.Drawing.Size(75, 25);
            this.btnDaily.TabIndex = 0;
            this.btnDaily.Text = "Daily";
            this.btnDaily.UseVisualStyleBackColor = true;
            this.btnDaily.Click += new System.EventHandler(this.btnDaily_Click);
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
            // dgvTimesheets
            // 
            this.dgvTimesheets.AllowUserToAddRows = false;
            this.dgvTimesheets.AllowUserToDeleteRows = false;
            this.dgvTimesheets.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTimesheets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTimesheets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTimesheets.Location = new System.Drawing.Point(0, 80);
            this.dgvTimesheets.MultiSelect = false;
            this.dgvTimesheets.Name = "dgvTimesheets";
            this.dgvTimesheets.ReadOnly = true;
            this.dgvTimesheets.RowHeadersVisible = false;
            this.dgvTimesheets.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTimesheets.Size = new System.Drawing.Size(800, 347);
            this.dgvTimesheets.TabIndex = 1;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(713, 43);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 9;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // ViewTimesheetsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvTimesheets);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlTop);
            this.Name = "ViewTimesheetsForm";
            this.Text = "ViewTimesheetsForm";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimesheets)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnDaily;
        private System.Windows.Forms.Button btnWeekly;
        private System.Windows.Forms.Button btnMonthly;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Button btnApplyRange;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblPeriod;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.DataGridView dgvTimesheets;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSurname;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDaysWorked;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalHours;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSundayExtraHours;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHolidayExtraHours;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPayableHours;
        private System.Windows.Forms.Button btnPrint;
    }
}
