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
            this.pnlHistory.Controls.Add(this.pnlHistoryTop);
            this.pnlHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHistory.Location = new System.Drawing.Point(320, 0);
            this.pnlHistory.Name = "pnlHistory";
            this.pnlHistory.Size = new System.Drawing.Size(569, 551);
            this.pnlHistory.TabIndex = 0;
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
            this.dgvHistory.Location = new System.Drawing.Point(0, 40);
            this.dgvHistory.MultiSelect = false;
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.RowHeadersVisible = false;
            this.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.Size = new System.Drawing.Size(569, 511);
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
    }
}
