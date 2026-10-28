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
            this.lblAppName = new System.Windows.Forms.Label();
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlHeaderLine = new System.Windows.Forms.Panel();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlSidebar.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.Controls.Add(this.lblAppName);
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
            this.pnlSidebar.Size = new System.Drawing.Size(210, 620);
            this.pnlSidebar.TabIndex = 0;
            // 
            // lblAppName
            // 
            this.lblAppName.Location = new System.Drawing.Point(18, 20);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(180, 60);
            this.lblAppName.TabIndex = 8;
            this.lblAppName.Text = "Employee Time\r\nManagement";
            // 
            // lblStore
            // 
            this.lblStore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStore.Location = new System.Drawing.Point(18, 546);
            this.lblStore.Name = "lblStore";
            this.lblStore.Size = new System.Drawing.Size(180, 18);
            this.lblStore.TabIndex = 2;
            this.lblStore.Text = "John Store";
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLogout.Location = new System.Drawing.Point(18, 574);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(174, 30);
            this.btnLogout.TabIndex = 7;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // lblCurrentUser
            // 
            this.lblCurrentUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCurrentUser.Location = new System.Drawing.Point(18, 524);
            this.lblCurrentUser.Name = "lblCurrentUser";
            this.lblCurrentUser.Size = new System.Drawing.Size(180, 20);
            this.lblCurrentUser.TabIndex = 1;
            this.lblCurrentUser.Text = "John User";
            // 
            // btnManagers
            // 
            this.btnManagers.Location = new System.Drawing.Point(0, 352);
            this.btnManagers.Name = "btnManagers";
            this.btnManagers.Size = new System.Drawing.Size(210, 42);
            this.btnManagers.TabIndex = 6;
            this.btnManagers.Text = "Managers";
            this.btnManagers.UseVisualStyleBackColor = true;
            this.btnManagers.Click += new System.EventHandler(this.btnManagers_Click);
            // 
            // btnLeave
            // 
            this.btnLeave.Location = new System.Drawing.Point(0, 310);
            this.btnLeave.Name = "btnLeave";
            this.btnLeave.Size = new System.Drawing.Size(210, 42);
            this.btnLeave.TabIndex = 5;
            this.btnLeave.Text = "Leave";
            this.btnLeave.UseVisualStyleBackColor = true;
            this.btnLeave.Click += new System.EventHandler(this.btnLeave_Click);
            // 
            // btnViewTimesheets
            // 
            this.btnViewTimesheets.Location = new System.Drawing.Point(0, 268);
            this.btnViewTimesheets.Name = "btnViewTimesheets";
            this.btnViewTimesheets.Size = new System.Drawing.Size(210, 42);
            this.btnViewTimesheets.TabIndex = 4;
            this.btnViewTimesheets.Text = "View Timesheets";
            this.btnViewTimesheets.UseVisualStyleBackColor = true;
            this.btnViewTimesheets.Click += new System.EventHandler(this.btnViewTimesheets_Click);
            // 
            // btnCaptureTimesheets
            // 
            this.btnCaptureTimesheets.Location = new System.Drawing.Point(0, 226);
            this.btnCaptureTimesheets.Name = "btnCaptureTimesheets";
            this.btnCaptureTimesheets.Size = new System.Drawing.Size(210, 42);
            this.btnCaptureTimesheets.TabIndex = 3;
            this.btnCaptureTimesheets.Text = "Capture Timesheets";
            this.btnCaptureTimesheets.UseVisualStyleBackColor = true;
            this.btnCaptureTimesheets.Click += new System.EventHandler(this.btnCaptureTimesheets_Click);
            // 
            // btnContracts
            // 
            this.btnContracts.Location = new System.Drawing.Point(0, 184);
            this.btnContracts.Name = "btnContracts";
            this.btnContracts.Size = new System.Drawing.Size(210, 42);
            this.btnContracts.TabIndex = 2;
            this.btnContracts.Text = "Contracts";
            this.btnContracts.UseVisualStyleBackColor = true;
            this.btnContracts.Click += new System.EventHandler(this.btnContracts_Click);
            // 
            // btnEmployees
            // 
            this.btnEmployees.Location = new System.Drawing.Point(0, 142);
            this.btnEmployees.Name = "btnEmployees";
            this.btnEmployees.Size = new System.Drawing.Size(210, 42);
            this.btnEmployees.TabIndex = 1;
            this.btnEmployees.Text = "Employees";
            this.btnEmployees.UseVisualStyleBackColor = true;
            this.btnEmployees.Click += new System.EventHandler(this.btnEmployees_Click);
            // 
            // btnDashboard
            // 
            this.btnDashboard.Location = new System.Drawing.Point(0, 100);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(210, 42);
            this.btnDashboard.TabIndex = 0;
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.UseVisualStyleBackColor = true;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.pnlHeaderLine);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(210, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(890, 50);
            this.pnlHeader.TabIndex = 2;
            // 
            // pnlHeaderLine
            // 
            this.pnlHeaderLine.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlHeaderLine.Location = new System.Drawing.Point(0, 47);
            this.pnlHeaderLine.Name = "pnlHeaderLine";
            this.pnlHeaderLine.Size = new System.Drawing.Size(890, 3);
            this.pnlHeaderLine.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(20, 11);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(144, 13);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Dashboard";
            // 
            // pnlContent
            // 
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(210, 50);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(890, 570);
            this.pnlContent.TabIndex = 1;
            // 
            // DashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 620);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlSidebar);
            this.Name = "DashboardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Employee Time Management";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DashboardForm_FormClosing);
            this.pnlSidebar.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlHeaderLine;
        private System.Windows.Forms.Label lblAppName;
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