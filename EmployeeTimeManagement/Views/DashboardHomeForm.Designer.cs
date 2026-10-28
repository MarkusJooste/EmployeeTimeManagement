namespace EmployeeTimeManagement.Views
{
    partial class DashboardHomeForm
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
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblStore = new System.Windows.Forms.Label();
            this.pnlEmployees = new System.Windows.Forms.Panel();
            this.pnlHours = new System.Windows.Forms.Panel();
            this.pnlLeave = new System.Windows.Forms.Panel();
            this.pnlAwol = new System.Windows.Forms.Panel();
            this.lblEmployeeCount = new System.Windows.Forms.Label();
            this.lblHoursWeek = new System.Windows.Forms.Label();
            this.lblLeaveToday = new System.Windows.Forms.Label();
            this.lblAwolWeek = new System.Windows.Forms.Label();
            this.pnlRenew = new System.Windows.Forms.Panel();
            this.lblRenewCount = new System.Windows.Forms.Label();
            this.lblRenewCaption = new System.Windows.Forms.Label();
            this.lblRenewNames = new System.Windows.Forms.Label();
            this.lblEmployeesCaption = new System.Windows.Forms.Label();
            this.lblHoursCaption = new System.Windows.Forms.Label();
            this.lblLeaveCaption = new System.Windows.Forms.Label();
            this.lblAwolCaption = new System.Windows.Forms.Label();
            this.tlpCards = new System.Windows.Forms.TableLayoutPanel();
            this.pnlEmployees.SuspendLayout();
            this.pnlHours.SuspendLayout();
            this.pnlLeave.SuspendLayout();
            this.pnlAwol.SuspendLayout();
            this.pnlRenew.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Location = new System.Drawing.Point(39, 21);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(62, 13);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "lblWelcome";
            // 
            // lblStore
            // 
            this.lblStore.AutoSize = true;
            this.lblStore.Location = new System.Drawing.Point(39, 52);
            this.lblStore.Name = "lblStore";
            this.lblStore.Size = new System.Drawing.Size(32, 13);
            this.lblStore.TabIndex = 1;
            this.lblStore.Text = "Store";
            // 
            // pnlEmployees
            // 
            this.pnlEmployees.BackColor = System.Drawing.Color.White;
            this.pnlEmployees.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlEmployees.Controls.Add(this.lblEmployeeCount);
            this.pnlEmployees.Controls.Add(this.lblEmployeesCaption);
            this.pnlEmployees.Location = new System.Drawing.Point(39, 75);
            this.pnlEmployees.Name = "pnlEmployees";
            this.pnlEmployees.Size = new System.Drawing.Size(148, 86);
            this.pnlEmployees.TabIndex = 2;
            // 
            // lblEmployeesCaption
            // 
            this.lblEmployeesCaption.AutoSize = true;
            this.lblEmployeesCaption.ForeColor = System.Drawing.Color.DimGray;
            this.lblEmployeesCaption.Location = new System.Drawing.Point(10, 8);
            this.lblEmployeesCaption.Name = "lblEmployeesCaption";
            this.lblEmployeesCaption.Size = new System.Drawing.Size(60, 13);
            this.lblEmployeesCaption.TabIndex = 0;
            this.lblEmployeesCaption.Text = "Employees";
            // 
            // lblEmployeeCount
            // 
            this.lblEmployeeCount.AutoSize = true;
            this.lblEmployeeCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmployeeCount.Location = new System.Drawing.Point(8, 28);
            this.lblEmployeeCount.Name = "lblEmployeeCount";
            this.lblEmployeeCount.Size = new System.Drawing.Size(26, 29);
            this.lblEmployeeCount.TabIndex = 1;
            this.lblEmployeeCount.Text = "0";
            // 
            // pnlHours
            // 
            this.pnlHours.BackColor = System.Drawing.Color.White;
            this.pnlHours.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHours.Controls.Add(this.lblHoursWeek);
            this.pnlHours.Controls.Add(this.lblHoursCaption);
            this.pnlHours.Location = new System.Drawing.Point(201, 75);
            this.pnlHours.Name = "pnlHours";
            this.pnlHours.Size = new System.Drawing.Size(148, 86);
            this.pnlHours.TabIndex = 3;
            // 
            // lblHoursCaption
            // 
            this.lblHoursCaption.AutoSize = true;
            this.lblHoursCaption.ForeColor = System.Drawing.Color.DimGray;
            this.lblHoursCaption.Location = new System.Drawing.Point(10, 8);
            this.lblHoursCaption.Name = "lblHoursCaption";
            this.lblHoursCaption.Size = new System.Drawing.Size(60, 13);
            this.lblHoursCaption.TabIndex = 0;
            this.lblHoursCaption.Text = "Hours this week";
            // 
            // lblHoursWeek
            // 
            this.lblHoursWeek.AutoSize = true;
            this.lblHoursWeek.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHoursWeek.Location = new System.Drawing.Point(8, 28);
            this.lblHoursWeek.Name = "lblHoursWeek";
            this.lblHoursWeek.Size = new System.Drawing.Size(26, 29);
            this.lblHoursWeek.TabIndex = 1;
            this.lblHoursWeek.Text = "0";
            // 
            // pnlLeave
            // 
            this.pnlLeave.BackColor = System.Drawing.Color.White;
            this.pnlLeave.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLeave.Controls.Add(this.lblLeaveToday);
            this.pnlLeave.Controls.Add(this.lblLeaveCaption);
            this.pnlLeave.Location = new System.Drawing.Point(363, 75);
            this.pnlLeave.Name = "pnlLeave";
            this.pnlLeave.Size = new System.Drawing.Size(148, 86);
            this.pnlLeave.TabIndex = 4;
            // 
            // lblLeaveCaption
            // 
            this.lblLeaveCaption.AutoSize = true;
            this.lblLeaveCaption.ForeColor = System.Drawing.Color.DimGray;
            this.lblLeaveCaption.Location = new System.Drawing.Point(10, 8);
            this.lblLeaveCaption.Name = "lblLeaveCaption";
            this.lblLeaveCaption.Size = new System.Drawing.Size(60, 13);
            this.lblLeaveCaption.TabIndex = 0;
            this.lblLeaveCaption.Text = "On leave today";
            // 
            // lblLeaveToday
            // 
            this.lblLeaveToday.AutoSize = true;
            this.lblLeaveToday.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeaveToday.Location = new System.Drawing.Point(8, 28);
            this.lblLeaveToday.Name = "lblLeaveToday";
            this.lblLeaveToday.Size = new System.Drawing.Size(26, 29);
            this.lblLeaveToday.TabIndex = 1;
            this.lblLeaveToday.Text = "0";
            // 
            // pnlAwol
            // 
            this.pnlAwol.BackColor = System.Drawing.Color.White;
            this.pnlAwol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAwol.Controls.Add(this.lblAwolWeek);
            this.pnlAwol.Controls.Add(this.lblAwolCaption);
            this.pnlAwol.Location = new System.Drawing.Point(525, 75);
            this.pnlAwol.Name = "pnlAwol";
            this.pnlAwol.Size = new System.Drawing.Size(148, 86);
            this.pnlAwol.TabIndex = 5;
            // 
            // lblAwolCaption
            // 
            this.lblAwolCaption.AutoSize = true;
            this.lblAwolCaption.ForeColor = System.Drawing.Color.DimGray;
            this.lblAwolCaption.Location = new System.Drawing.Point(10, 8);
            this.lblAwolCaption.Name = "lblAwolCaption";
            this.lblAwolCaption.Size = new System.Drawing.Size(60, 13);
            this.lblAwolCaption.TabIndex = 0;
            this.lblAwolCaption.Text = "AWOL this week";
            // 
            // lblAwolWeek
            // 
            this.lblAwolWeek.AutoSize = true;
            this.lblAwolWeek.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAwolWeek.Location = new System.Drawing.Point(8, 28);
            this.lblAwolWeek.Name = "lblAwolWeek";
            this.lblAwolWeek.Size = new System.Drawing.Size(26, 29);
            this.lblAwolWeek.TabIndex = 1;
            this.lblAwolWeek.Text = "0";
            //
            // pnlRenew
            //
            this.pnlRenew.BackColor = System.Drawing.Color.White;
            this.pnlRenew.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRenew.Controls.Add(this.lblRenewCount);
            this.pnlRenew.Controls.Add(this.lblRenewCaption);
            this.pnlRenew.Controls.Add(this.lblRenewNames);
            this.pnlRenew.Location = new System.Drawing.Point(687, 75);
            this.pnlRenew.Name = "pnlRenew";
            this.pnlRenew.Size = new System.Drawing.Size(148, 86);
            this.pnlRenew.TabIndex = 7;
            //
            // lblRenewCaption
            //
            this.lblRenewCaption.AutoSize = true;
            this.lblRenewCaption.ForeColor = System.Drawing.Color.DimGray;
            this.lblRenewCaption.Location = new System.Drawing.Point(10, 8);
            this.lblRenewCaption.Name = "lblRenewCaption";
            this.lblRenewCaption.Size = new System.Drawing.Size(60, 13);
            this.lblRenewCaption.TabIndex = 0;
            this.lblRenewCaption.Text = "Contracts to renew";
            //
            // lblRenewCount
            //
            this.lblRenewCount.AutoSize = true;
            this.lblRenewCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRenewCount.Location = new System.Drawing.Point(8, 28);
            this.lblRenewCount.Name = "lblRenewCount";
            this.lblRenewCount.Size = new System.Drawing.Size(26, 29);
            this.lblRenewCount.TabIndex = 1;
            this.lblRenewCount.Text = "0";
            //
            // lblRenewNames
            // Who the count is, filled in by DashboardHomeForm.
            this.lblRenewNames.AutoEllipsis = true;
            this.lblRenewNames.Location = new System.Drawing.Point(18, 64);
            this.lblRenewNames.Name = "lblRenewNames";
            this.lblRenewNames.Size = new System.Drawing.Size(124, 16);
            this.lblRenewNames.TabIndex = 2;
            // 
            // tlpCards
            // 
            // The dashboard's lists, three across and two down; DashboardHomeForm fills it.
            this.tlpCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCards.ColumnCount = 3;
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpCards.Location = new System.Drawing.Point(39, 174);
            this.tlpCards.Name = "tlpCards";
            this.tlpCards.RowCount = 2;
            this.tlpCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCards.Size = new System.Drawing.Size(808, 388);
            this.tlpCards.TabIndex = 6;
            // 
            // DashboardHomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 574);
            this.Controls.Add(this.tlpCards);
            this.Controls.Add(this.pnlRenew);
            this.Controls.Add(this.pnlAwol);
            this.Controls.Add(this.pnlLeave);
            this.Controls.Add(this.pnlHours);
            this.Controls.Add(this.pnlEmployees);
            this.Controls.Add(this.lblStore);
            this.Controls.Add(this.lblWelcome);
            this.Name = "DashboardHomeForm";
            this.Text = "DashboardHomeForm";
            this.pnlEmployees.ResumeLayout(false);
            this.pnlEmployees.PerformLayout();
            this.pnlHours.ResumeLayout(false);
            this.pnlHours.PerformLayout();
            this.pnlLeave.ResumeLayout(false);
            this.pnlLeave.PerformLayout();
            this.pnlAwol.ResumeLayout(false);
            this.pnlAwol.PerformLayout();
            this.pnlRenew.ResumeLayout(false);
            this.pnlRenew.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblStore;
        private System.Windows.Forms.Panel pnlEmployees;
        private System.Windows.Forms.Label lblEmployeeCount;
        private System.Windows.Forms.Panel pnlHours;
        private System.Windows.Forms.Label lblHoursWeek;
        private System.Windows.Forms.Panel pnlLeave;
        private System.Windows.Forms.Label lblLeaveToday;
        private System.Windows.Forms.Panel pnlAwol;
        private System.Windows.Forms.Label lblAwolWeek;
        private System.Windows.Forms.Label lblEmployeesCaption;
        private System.Windows.Forms.Label lblHoursCaption;
        private System.Windows.Forms.Label lblLeaveCaption;
        private System.Windows.Forms.Label lblAwolCaption;
        private System.Windows.Forms.Panel pnlRenew;
        private System.Windows.Forms.Label lblRenewCount;
        private System.Windows.Forms.Label lblRenewCaption;
        private System.Windows.Forms.Label lblRenewNames;
        private System.Windows.Forms.TableLayoutPanel tlpCards;
    }
}