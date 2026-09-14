namespace EmployeeTimeManagement.Views
{
    partial class LeaveForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyleDate1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyleDate2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyleDayCount = new System.Windows.Forms.DataGridViewCellStyle();
            this.employeePicker = new EmployeeTimeManagement.Views.EmployeePickerControl();
            this.pnlHistory = new System.Windows.Forms.Panel();
            this.pnlBooking = new System.Windows.Forms.Panel();
            this.lblBookingMessage = new System.Windows.Forms.Label();
            this.btnBookAbsence = new System.Windows.Forms.Button();
            this.txtBookReason = new System.Windows.Forms.TextBox();
            this.lblBookReason = new System.Windows.Forms.Label();
            this.lblBookDayCount = new System.Windows.Forms.Label();
            this.dtpBookEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblBookEndDate = new System.Windows.Forms.Label();
            this.dtpBookStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblBookStartDate = new System.Windows.Forms.Label();
            this.cboBookLeaveType = new System.Windows.Forms.ComboBox();
            this.lblBookLeaveType = new System.Windows.Forms.Label();
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            this.colLeaveType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStartDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEndDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDayCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReason = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlHistoryTop = new System.Windows.Forms.Panel();
            this.cboLeaveType = new System.Windows.Forms.ComboBox();
            this.lblLeaveType = new System.Windows.Forms.Label();
            this.lblSelectedEmployee = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlHistory.SuspendLayout();
            this.pnlBooking.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.pnlHistoryTop.SuspendLayout();
            this.SuspendLayout();
            //
            // employeePicker
            //
            this.employeePicker.Dock = System.Windows.Forms.DockStyle.Left;
            this.employeePicker.Location = new System.Drawing.Point(0, 0);
            this.employeePicker.Name = "employeePicker";
            this.employeePicker.Size = new System.Drawing.Size(320, 551);
            this.employeePicker.TabIndex = 1;
            this.employeePicker.SelectionChanged += new System.EventHandler(this.employeePicker_SelectionChanged);
            this.employeePicker.FilterChanged += new System.EventHandler(this.employeePicker_FilterChanged);
            //
            // pnlHistory
            //
            this.pnlHistory.Controls.Add(this.dgvHistory);
            this.pnlHistory.Controls.Add(this.pnlBooking);
            this.pnlHistory.Controls.Add(this.pnlHistoryTop);
            this.pnlHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHistory.Location = new System.Drawing.Point(320, 0);
            this.pnlHistory.Name = "pnlHistory";
            this.pnlHistory.Size = new System.Drawing.Size(569, 551);
            this.pnlHistory.TabIndex = 0;
            //
            // pnlBooking
            //
            this.pnlBooking.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBooking.Controls.Add(this.lblBookLeaveType);
            this.pnlBooking.Controls.Add(this.cboBookLeaveType);
            this.pnlBooking.Controls.Add(this.lblBookStartDate);
            this.pnlBooking.Controls.Add(this.dtpBookStartDate);
            this.pnlBooking.Controls.Add(this.lblBookEndDate);
            this.pnlBooking.Controls.Add(this.dtpBookEndDate);
            this.pnlBooking.Controls.Add(this.lblBookDayCount);
            this.pnlBooking.Controls.Add(this.lblBookReason);
            this.pnlBooking.Controls.Add(this.txtBookReason);
            this.pnlBooking.Controls.Add(this.btnBookAbsence);
            this.pnlBooking.Controls.Add(this.lblBookingMessage);
            this.pnlBooking.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBooking.Enabled = false;
            this.pnlBooking.Location = new System.Drawing.Point(0, 40);
            this.pnlBooking.Name = "pnlBooking";
            this.pnlBooking.Size = new System.Drawing.Size(569, 72);
            this.pnlBooking.TabIndex = 2;
            //
            // lblBookLeaveType
            //
            this.lblBookLeaveType.AutoSize = true;
            this.lblBookLeaveType.Location = new System.Drawing.Point(8, 11);
            this.lblBookLeaveType.Name = "lblBookLeaveType";
            this.lblBookLeaveType.Size = new System.Drawing.Size(63, 13);
            this.lblBookLeaveType.TabIndex = 0;
            this.lblBookLeaveType.Text = "Leave Type";
            //
            // cboBookLeaveType
            //
            this.cboBookLeaveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBookLeaveType.FormattingEnabled = true;
            this.cboBookLeaveType.Location = new System.Drawing.Point(80, 8);
            this.cboBookLeaveType.Name = "cboBookLeaveType";
            this.cboBookLeaveType.Size = new System.Drawing.Size(100, 21);
            this.cboBookLeaveType.TabIndex = 1;
            //
            // lblBookStartDate
            //
            this.lblBookStartDate.AutoSize = true;
            this.lblBookStartDate.Location = new System.Drawing.Point(196, 11);
            this.lblBookStartDate.Name = "lblBookStartDate";
            this.lblBookStartDate.Size = new System.Drawing.Size(58, 13);
            this.lblBookStartDate.TabIndex = 2;
            this.lblBookStartDate.Text = "Start Date";
            //
            // dtpBookStartDate
            //
            this.dtpBookStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBookStartDate.Location = new System.Drawing.Point(260, 8);
            this.dtpBookStartDate.Name = "dtpBookStartDate";
            this.dtpBookStartDate.Size = new System.Drawing.Size(100, 20);
            this.dtpBookStartDate.TabIndex = 3;
            this.dtpBookStartDate.ValueChanged += new System.EventHandler(this.dtpBookDate_ValueChanged);
            //
            // lblBookEndDate
            //
            this.lblBookEndDate.AutoSize = true;
            this.lblBookEndDate.Location = new System.Drawing.Point(372, 11);
            this.lblBookEndDate.Name = "lblBookEndDate";
            this.lblBookEndDate.Size = new System.Drawing.Size(55, 13);
            this.lblBookEndDate.TabIndex = 4;
            this.lblBookEndDate.Text = "End Date";
            //
            // dtpBookEndDate
            //
            this.dtpBookEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBookEndDate.Location = new System.Drawing.Point(432, 8);
            this.dtpBookEndDate.Name = "dtpBookEndDate";
            this.dtpBookEndDate.Size = new System.Drawing.Size(100, 20);
            this.dtpBookEndDate.TabIndex = 5;
            this.dtpBookEndDate.ValueChanged += new System.EventHandler(this.dtpBookDate_ValueChanged);
            //
            // lblBookDayCount
            //
            this.lblBookDayCount.AutoSize = true;
            this.lblBookDayCount.Location = new System.Drawing.Point(8, 38);
            this.lblBookDayCount.Name = "lblBookDayCount";
            this.lblBookDayCount.Size = new System.Drawing.Size(43, 13);
            this.lblBookDayCount.TabIndex = 6;
            this.lblBookDayCount.Text = "1 day";
            //
            // lblBookReason
            //
            this.lblBookReason.AutoSize = true;
            this.lblBookReason.Location = new System.Drawing.Point(90, 38);
            this.lblBookReason.Name = "lblBookReason";
            this.lblBookReason.Size = new System.Drawing.Size(45, 13);
            this.lblBookReason.TabIndex = 7;
            this.lblBookReason.Text = "Reason";
            //
            // txtBookReason
            //
            this.txtBookReason.Location = new System.Drawing.Point(141, 35);
            this.txtBookReason.MaxLength = 255;
            this.txtBookReason.Name = "txtBookReason";
            this.txtBookReason.Size = new System.Drawing.Size(240, 20);
            this.txtBookReason.TabIndex = 8;
            //
            // btnBookAbsence
            //
            this.btnBookAbsence.Location = new System.Drawing.Point(392, 33);
            this.btnBookAbsence.Name = "btnBookAbsence";
            this.btnBookAbsence.Size = new System.Drawing.Size(100, 23);
            this.btnBookAbsence.TabIndex = 9;
            this.btnBookAbsence.Text = "Book Absence";
            this.btnBookAbsence.UseVisualStyleBackColor = true;
            this.btnBookAbsence.Click += new System.EventHandler(this.btnBookAbsence_Click);
            //
            // lblBookingMessage
            //
            this.lblBookingMessage.Location = new System.Drawing.Point(8, 58);
            this.lblBookingMessage.Name = "lblBookingMessage";
            this.lblBookingMessage.Size = new System.Drawing.Size(553, 13);
            this.lblBookingMessage.TabIndex = 10;
            //
            // dgvHistory
            //
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.AutoGenerateColumns = false;
            this.dgvHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLeaveType,
            this.colStartDate,
            this.colEndDate,
            this.colDayCount,
            this.colReason});
            this.dgvHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistory.Location = new System.Drawing.Point(0, 112);
            this.dgvHistory.MultiSelect = false;
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.RowHeadersVisible = false;
            this.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.Size = new System.Drawing.Size(569, 439);
            this.dgvHistory.TabIndex = 1;
            //
            // colLeaveType
            //
            this.colLeaveType.DataPropertyName = "LeaveTypeDisplay";
            this.colLeaveType.HeaderText = "Leave Type";
            this.colLeaveType.Name = "colLeaveType";
            this.colLeaveType.ReadOnly = true;
            this.colLeaveType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colStartDate
            //
            this.colStartDate.DataPropertyName = "StartDate";
            dataGridViewCellStyleDate1.Format = "dd MMM yyyy";
            this.colStartDate.DefaultCellStyle = dataGridViewCellStyleDate1;
            this.colStartDate.HeaderText = "Start Date";
            this.colStartDate.Name = "colStartDate";
            this.colStartDate.ReadOnly = true;
            this.colStartDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colEndDate
            //
            this.colEndDate.DataPropertyName = "EndDate";
            dataGridViewCellStyleDate2.Format = "dd MMM yyyy";
            this.colEndDate.DefaultCellStyle = dataGridViewCellStyleDate2;
            this.colEndDate.HeaderText = "End Date";
            this.colEndDate.Name = "colEndDate";
            this.colEndDate.ReadOnly = true;
            this.colEndDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colDayCount
            //
            this.colDayCount.DataPropertyName = "DayCount";
            dataGridViewCellStyleDayCount.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colDayCount.DefaultCellStyle = dataGridViewCellStyleDayCount;
            this.colDayCount.HeaderText = "Days";
            this.colDayCount.Name = "colDayCount";
            this.colDayCount.ReadOnly = true;
            this.colDayCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // colReason
            //
            this.colReason.DataPropertyName = "Reason";
            this.colReason.HeaderText = "Reason";
            this.colReason.Name = "colReason";
            this.colReason.ReadOnly = true;
            this.colReason.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // pnlHistoryTop
            //
            this.pnlHistoryTop.Controls.Add(this.cboLeaveType);
            this.pnlHistoryTop.Controls.Add(this.lblLeaveType);
            this.pnlHistoryTop.Controls.Add(this.lblSelectedEmployee);
            this.pnlHistoryTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHistoryTop.Location = new System.Drawing.Point(0, 0);
            this.pnlHistoryTop.Name = "pnlHistoryTop";
            this.pnlHistoryTop.Size = new System.Drawing.Size(569, 40);
            this.pnlHistoryTop.TabIndex = 0;
            //
            // cboLeaveType
            //
            this.cboLeaveType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLeaveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLeaveType.FormattingEnabled = true;
            this.cboLeaveType.Location = new System.Drawing.Point(437, 9);
            this.cboLeaveType.Name = "cboLeaveType";
            this.cboLeaveType.Size = new System.Drawing.Size(120, 21);
            this.cboLeaveType.TabIndex = 2;
            this.cboLeaveType.SelectedIndexChanged += new System.EventHandler(this.cboLeaveType_SelectedIndexChanged);
            //
            // lblLeaveType
            //
            this.lblLeaveType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLeaveType.AutoSize = true;
            this.lblLeaveType.Location = new System.Drawing.Point(360, 13);
            this.lblLeaveType.Name = "lblLeaveType";
            this.lblLeaveType.Size = new System.Drawing.Size(64, 13);
            this.lblLeaveType.TabIndex = 1;
            this.lblLeaveType.Text = "Leave Type";
            //
            // lblSelectedEmployee
            //
            this.lblSelectedEmployee.AutoSize = true;
            this.lblSelectedEmployee.Location = new System.Drawing.Point(12, 13);
            this.lblSelectedEmployee.Name = "lblSelectedEmployee";
            this.lblSelectedEmployee.Size = new System.Drawing.Size(89, 13);
            this.lblSelectedEmployee.TabIndex = 0;
            this.lblSelectedEmployee.Text = "lblSelectedEmployee";
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
            // LeaveForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 574);
            this.Controls.Add(this.pnlHistory);
            this.Controls.Add(this.employeePicker);
            this.Controls.Add(this.lblStatus);
            this.Name = "LeaveForm";
            this.Text = "LeaveForm";
            this.pnlHistory.ResumeLayout(false);
            this.pnlBooking.ResumeLayout(false);
            this.pnlBooking.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.pnlHistoryTop.ResumeLayout(false);
            this.pnlHistoryTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private EmployeeTimeManagement.Views.EmployeePickerControl employeePicker;
        private System.Windows.Forms.Panel pnlHistory;
        private System.Windows.Forms.DataGridView dgvHistory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLeaveType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStartDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEndDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDayCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReason;
        private System.Windows.Forms.Panel pnlHistoryTop;
        private System.Windows.Forms.ComboBox cboLeaveType;
        private System.Windows.Forms.Label lblLeaveType;
        private System.Windows.Forms.Label lblSelectedEmployee;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlBooking;
        private System.Windows.Forms.Label lblBookLeaveType;
        private System.Windows.Forms.ComboBox cboBookLeaveType;
        private System.Windows.Forms.Label lblBookStartDate;
        private System.Windows.Forms.DateTimePicker dtpBookStartDate;
        private System.Windows.Forms.Label lblBookEndDate;
        private System.Windows.Forms.DateTimePicker dtpBookEndDate;
        private System.Windows.Forms.Label lblBookDayCount;
        private System.Windows.Forms.Label lblBookReason;
        private System.Windows.Forms.TextBox txtBookReason;
        private System.Windows.Forms.Button btnBookAbsence;
        private System.Windows.Forms.Label lblBookingMessage;
    }
}
