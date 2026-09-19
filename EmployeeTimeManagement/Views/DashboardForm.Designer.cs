namespace EmployeeTimeManagement.Views
{
    partial class DashboardForm
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
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.lblStore = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblCurrentUser = new System.Windows.Forms.Label();
            this.btnManagers = new System.Windows.Forms.Button();
            this.btnLeave = new System.Windows.Forms.Button();
            this.btnViewTimesheets = new System.Windows.Forms.Button();
            this.btnCaptureTimesheets = new System.Windows.Forms.Button();
            this.btnContracts = new System.Windows.Forms.Button();
            this.btnEmployees = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlSidebar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.Controls.Add(this.lblStore);
            this.pnlSidebar.Controls.Add(this.btnLogout);
            this.pnlSidebar.Controls.Add(this.lblCurrentUser);
            this.pnlSidebar.Controls.Add(this.btnManagers);
            this.pnlSidebar.Controls.Add(this.btnLeave);
            this.pnlSidebar.Controls.Add(this.btnViewTimesheets);
            this.pnlSidebar.Controls.Add(this.btnCaptureTimesheets);
            this.pnlSidebar.Controls.Add(this.btnContracts);
            this.pnlSidebar.Controls.Add(this.btnEmployees);
            this.pnlSidebar.Controls.Add(this.btnDashboard);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(193, 620);
            this.pnlSidebar.TabIndex = 0;
            // 
            // lblStore
            // 
            this.lblStore.AutoSize = true;
            this.lblStore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStore.Location = new System.Drawing.Point(10, 586);
            this.lblStore.Name = "lblStore";
            this.lblStore.Size = new System.Drawing.Size(58, 13);
            this.lblStore.TabIndex = 2;
            this.lblStore.Text = "John Store";
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLogout.Location = new System.Drawing.Point(20, 495);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(150, 25);
            this.btnLogout.TabIndex = 7;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // lblCurrentUser
            // 
            this.lblCurrentUser.AutoSize = true;
            this.lblCurrentUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCurrentUser.Location = new System.Drawing.Point(12, 551);
            this.lblCurrentUser.Name = "lblCurrentUser";
            this.lblCurrentUser.Size = new System.Drawing.Size(55, 13);
            this.lblCurrentUser.TabIndex = 1;
            this.lblCurrentUser.Text = "John User";
            // 
            // btnManagers
            // 
            this.btnManagers.Location = new System.Drawing.Point(20, 296);
            this.btnManagers.Name = "btnManagers";
            this.btnManagers.Size = new System.Drawing.Size(150, 25);
            this.btnManagers.TabIndex = 6;
            this.btnManagers.Text = "Managers";
            this.btnManagers.UseVisualStyleBackColor = true;
            this.btnManagers.Click += new System.EventHandler(this.btnManagers_Click);
            // 
            // btnLeave
            // 
            this.btnLeave.Location = new System.Drawing.Point(20, 267);
            this.btnLeave.Name = "btnLeave";
            this.btnLeave.Size = new System.Drawing.Size(150, 25);
            this.btnLeave.TabIndex = 5;
            this.btnLeave.Text = "Leave";
            this.btnLeave.UseVisualStyleBackColor = true;
            this.btnLeave.Click += new System.EventHandler(this.btnLeave_Click);
            // 
            // btnViewTimesheets
            // 
            this.btnViewTimesheets.Location = new System.Drawing.Point(20, 238);
            this.btnViewTimesheets.Name = "btnViewTimesheets";
            this.btnViewTimesheets.Size = new System.Drawing.Size(150, 25);
            this.btnViewTimesheets.TabIndex = 4;
            this.btnViewTimesheets.Text = "View Timesheets";
            this.btnViewTimesheets.UseVisualStyleBackColor = true;
            this.btnViewTimesheets.Click += new System.EventHandler(this.btnViewTimesheets_Click);
            // 
            // btnCaptureTimesheets
            // 
            this.btnCaptureTimesheets.Location = new System.Drawing.Point(20, 209);
            this.btnCaptureTimesheets.Name = "btnCaptureTimesheets";
            this.btnCaptureTimesheets.Size = new System.Drawing.Size(150, 25);
            this.btnCaptureTimesheets.TabIndex = 3;
            this.btnCaptureTimesheets.Text = "Capture Timesheets";
            this.btnCaptureTimesheets.UseVisualStyleBackColor = true;
            this.btnCaptureTimesheets.Click += new System.EventHandler(this.btnCaptureTimesheets_Click);
            // 
            // btnContracts
            // 
            this.btnContracts.Location = new System.Drawing.Point(20, 180);
            this.btnContracts.Name = "btnContracts";
            this.btnContracts.Size = new System.Drawing.Size(150, 25);
            this.btnContracts.TabIndex = 2;
            this.btnContracts.Text = "Contracts";
            this.btnContracts.UseVisualStyleBackColor = true;
            this.btnContracts.Click += new System.EventHandler(this.btnContracts_Click);
            // 
            // btnEmployees
            // 
            this.btnEmployees.Location = new System.Drawing.Point(20, 151);
            this.btnEmployees.Name = "btnEmployees";
            this.btnEmployees.Size = new System.Drawing.Size(150, 25);
            this.btnEmployees.TabIndex = 1;
            this.btnEmployees.Text = "Employees";
            this.btnEmployees.UseVisualStyleBackColor = true;
            this.btnEmployees.Click += new System.EventHandler(this.btnEmployees_Click);
            // 
            // btnDashboard
            // 
            this.btnDashboard.Location = new System.Drawing.Point(20, 122);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(150, 25);
            this.btnDashboard.TabIndex = 0;
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.UseVisualStyleBackColor = true;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlContent.Location = new System.Drawing.Point(199, 34);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(889, 574);
            this.pnlContent.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(208, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(144, 13);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Employee Time Management";
            // 
            // DashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 620);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlSidebar);
            this.Controls.Add(this.lblTitle);
            this.Name = "DashboardForm";
            this.Text = "DashboardForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DashboardForm_FormClosing);
            this.pnlSidebar.ResumeLayout(false);
            this.pnlSidebar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Label lblStore;
        private System.Windows.Forms.Label lblCurrentUser;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnManagers;
        private System.Windows.Forms.Button btnLeave;
        private System.Windows.Forms.Button btnViewTimesheets;
        private System.Windows.Forms.Button btnCaptureTimesheets;
        private System.Windows.Forms.Button btnContracts;
        private System.Windows.Forms.Button btnEmployees;
        private System.Windows.Forms.Button btnDashboard;
    }
}