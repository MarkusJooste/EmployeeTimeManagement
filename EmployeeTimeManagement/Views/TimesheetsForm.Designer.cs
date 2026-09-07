namespace EmployeeTimeManagement.Views
{
    partial class TimesheetsForm
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
            System.Windows.Forms.DataGridViewCellStyle totalHoursCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle sundayExtraCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle holidayExtraCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle payableCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnDaily = new System.Windows.Forms.Button();
            this.btnWeekly = new System.Windows.Forms.Button();
            this.btnMonthly = new System.Windows.Forms.Button();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.btnApplyRange = new System.Windows.Forms.Button();
            this.lblPeriod = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.dgvTimesheets = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSurname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDaysWorked = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalHours = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSundayExtraHours = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHolidayExtraHours = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPayableHours = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimesheets)).BeginInit();
            this.SuspendLayout();
            //
            // pnlTop
            //
            this.pnlTop.Controls.Add(this.lblPeriod);
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
            // dtpFrom
            //
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(12, 45);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(110, 20);
            this.dtpFrom.TabIndex = 3;
            //
            // dtpTo
            //
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(128, 45);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(110, 20);
            this.dtpTo.TabIndex = 4;
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
            // lblPeriod
            //
            this.lblPeriod.AutoSize = true;
            this.lblPeriod.Location = new System.Drawing.Point(268, 15);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(48, 13);
            this.lblPeriod.TabIndex = 6;
            this.lblPeriod.Text = "lblPeriod";
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
            this.dgvTimesheets.AutoGenerateColumns = false;
            this.dgvTimesheets.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTimesheets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTimesheets.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colSurname,
            this.colDaysWorked,
            this.colTotalHours,
            this.colSundayExtraHours,
            this.colHolidayExtraHours,
            this.colPayableHours});
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
            // colDaysWorked
            //
            this.colDaysWorked.DataPropertyName = "DaysWorked";
            this.colDaysWorked.HeaderText = "Days";
            this.colDaysWorked.Name = "colDaysWorked";
            this.colDaysWorked.ReadOnly = true;
            //
            // colTotalHours
            //
            totalHoursCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            totalHoursCellStyle.Format = "N2";
            this.colTotalHours.DefaultCellStyle = totalHoursCellStyle;
            this.colTotalHours.DataPropertyName = "TotalHours";
            this.colTotalHours.HeaderText = "Hours";
            this.colTotalHours.Name = "colTotalHours";
            this.colTotalHours.ReadOnly = true;
            //
            // colSundayExtraHours
            //
            sundayExtraCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            sundayExtraCellStyle.Format = "N2";
            this.colSundayExtraHours.DefaultCellStyle = sundayExtraCellStyle;
            this.colSundayExtraHours.DataPropertyName = "SundayExtraHours";
            this.colSundayExtraHours.HeaderText = "Sunday Extra";
            this.colSundayExtraHours.Name = "colSundayExtraHours";
            this.colSundayExtraHours.ReadOnly = true;
            //
            // colHolidayExtraHours
            //
            holidayExtraCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            holidayExtraCellStyle.Format = "N2";
            this.colHolidayExtraHours.DefaultCellStyle = holidayExtraCellStyle;
            this.colHolidayExtraHours.DataPropertyName = "HolidayExtraHours";
            this.colHolidayExtraHours.HeaderText = "Holiday Extra";
            this.colHolidayExtraHours.Name = "colHolidayExtraHours";
            this.colHolidayExtraHours.ReadOnly = true;
            //
            // colPayableHours
            //
            payableCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            payableCellStyle.Format = "N2";
            this.colPayableHours.DefaultCellStyle = payableCellStyle;
            this.colPayableHours.DataPropertyName = "PayableHours";
            this.colPayableHours.HeaderText = "Payable";
            this.colPayableHours.Name = "colPayableHours";
            this.colPayableHours.ReadOnly = true;
            //
            // TimesheetsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvTimesheets);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlTop);
            this.Name = "TimesheetsForm";
            this.Text = "TimesheetsForm";
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
    }
}
