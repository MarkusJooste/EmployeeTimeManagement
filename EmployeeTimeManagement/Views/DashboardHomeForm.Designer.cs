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
            this.lblHoursToday = new System.Windows.Forms.Label();
            this.lblLeaveToday = new System.Windows.Forms.Label();
            this.lblAwolToday = new System.Windows.Forms.Label();
            this.pnlEmployees.SuspendLayout();
            this.pnlHours.SuspendLayout();
            this.pnlLeave.SuspendLayout();
            this.pnlAwol.SuspendLayout();
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
            this.pnlEmployees.Controls.Add(this.lblEmployeeCount);
            this.pnlEmployees.Location = new System.Drawing.Point(42, 92);
            this.pnlEmployees.Name = "pnlEmployees";
            this.pnlEmployees.Size = new System.Drawing.Size(200, 100);
            this.pnlEmployees.TabIndex = 2;
            // 
            // pnlHours
            // 
            this.pnlHours.Controls.Add(this.lblHoursToday);
            this.pnlHours.Location = new System.Drawing.Point(319, 92);
            this.pnlHours.Name = "pnlHours";
            this.pnlHours.Size = new System.Drawing.Size(200, 100);
            this.pnlHours.TabIndex = 3;
            // 
            // pnlLeave
            // 
            this.pnlLeave.Controls.Add(this.lblLeaveToday);
            this.pnlLeave.Location = new System.Drawing.Point(42, 215);
            this.pnlLeave.Name = "pnlLeave";
            this.pnlLeave.Size = new System.Drawing.Size(200, 100);
            this.pnlLeave.TabIndex = 4;
            // 
            // pnlAwol
            // 
            this.pnlAwol.Controls.Add(this.lblAwolToday);
            this.pnlAwol.Location = new System.Drawing.Point(319, 215);
            this.pnlAwol.Name = "pnlAwol";
            this.pnlAwol.Size = new System.Drawing.Size(200, 100);
            this.pnlAwol.TabIndex = 5;
            // 
            // lblEmployeeCount
            // 
            this.lblEmployeeCount.AutoSize = true;
            this.lblEmployeeCount.Location = new System.Drawing.Point(65, 34);
            this.lblEmployeeCount.Name = "lblEmployeeCount";
            this.lblEmployeeCount.Size = new System.Drawing.Size(35, 13);
            this.lblEmployeeCount.TabIndex = 0;
            this.lblEmployeeCount.Text = "label1";
            // 
            // lblHoursToday
            // 
            this.lblHoursToday.AutoSize = true;
            this.lblHoursToday.Location = new System.Drawing.Point(94, 34);
            this.lblHoursToday.Name = "lblHoursToday";
            this.lblHoursToday.Size = new System.Drawing.Size(35, 13);
            this.lblHoursToday.TabIndex = 0;
            this.lblHoursToday.Text = "label1";
            // 
            // lblLeaveToday
            // 
            this.lblLeaveToday.AutoSize = true;
            this.lblLeaveToday.Location = new System.Drawing.Point(64, 35);
            this.lblLeaveToday.Name = "lblLeaveToday";
            this.lblLeaveToday.Size = new System.Drawing.Size(35, 13);
            this.lblLeaveToday.TabIndex = 0;
            this.lblLeaveToday.Text = "label1";
            // 
            // lblAwolToday
            // 
            this.lblAwolToday.AutoSize = true;
            this.lblAwolToday.Location = new System.Drawing.Point(94, 35);
            this.lblAwolToday.Name = "lblAwolToday";
            this.lblAwolToday.Size = new System.Drawing.Size(35, 13);
            this.lblAwolToday.TabIndex = 0;
            this.lblAwolToday.Text = "label1";
            // 
            // DashboardHomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblStore;
        private System.Windows.Forms.Panel pnlEmployees;
        private System.Windows.Forms.Label lblEmployeeCount;
        private System.Windows.Forms.Panel pnlHours;
        private System.Windows.Forms.Label lblHoursToday;
        private System.Windows.Forms.Panel pnlLeave;
        private System.Windows.Forms.Label lblLeaveToday;
        private System.Windows.Forms.Panel pnlAwol;
        private System.Windows.Forms.Label lblAwolToday;
    }
}