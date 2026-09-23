namespace EmployeeTimeManagement.Views
{
    partial class EmployeeDetailsPanel
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
            this.pnlIdentity = new System.Windows.Forms.Panel();
            this.lblEmptyMessage = new System.Windows.Forms.Label();
            this.pnlIdentityDetails = new System.Windows.Forms.Panel();
            this.lblJobDepartment = new System.Windows.Forms.Label();
            this.pnlStatusTag = new System.Windows.Forms.Panel();
            this.lblStatusTagText = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.pnlInitials = new System.Windows.Forms.Panel();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnReactivate = new System.Windows.Forms.Button();
            this.btnTerminate = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.pnlFacts = new System.Windows.Forms.FlowLayoutPanel();
            this.factIDNumber = new EmployeeTimeManagement.Views.FactBox();
            this.factMobile = new EmployeeTimeManagement.Views.FactBox();
            this.factStartDate = new EmployeeTimeManagement.Views.FactBox();
            this.factJob = new EmployeeTimeManagement.Views.FactBox();
            this.factDepartment = new EmployeeTimeManagement.Views.FactBox();
            this.factContract = new EmployeeTimeManagement.Views.FactBox();
            this.pnlIdentity.SuspendLayout();
            this.pnlIdentityDetails.SuspendLayout();
            this.pnlStatusTag.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.pnlFacts.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlIdentity
            //
            this.pnlIdentity.Controls.Add(this.pnlIdentityDetails);
            this.pnlIdentity.Controls.Add(this.lblEmptyMessage);
            this.pnlIdentity.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlIdentity.Location = new System.Drawing.Point(0, 0);
            this.pnlIdentity.Name = "pnlIdentity";
            this.pnlIdentity.Padding = new System.Windows.Forms.Padding(16, 16, 16, 8);
            this.pnlIdentity.Size = new System.Drawing.Size(589, 110);
            this.pnlIdentity.TabIndex = 0;
            //
            // lblEmptyMessage
            //
            this.lblEmptyMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEmptyMessage.Location = new System.Drawing.Point(16, 16);
            this.lblEmptyMessage.Name = "lblEmptyMessage";
            this.lblEmptyMessage.Size = new System.Drawing.Size(557, 86);
            this.lblEmptyMessage.TabIndex = 1;
            this.lblEmptyMessage.Text = "Select an employee";
            this.lblEmptyMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlIdentityDetails
            //
            this.pnlIdentityDetails.Controls.Add(this.lblJobDepartment);
            this.pnlIdentityDetails.Controls.Add(this.pnlStatusTag);
            this.pnlIdentityDetails.Controls.Add(this.lblName);
            this.pnlIdentityDetails.Controls.Add(this.pnlInitials);
            this.pnlIdentityDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlIdentityDetails.Location = new System.Drawing.Point(16, 16);
            this.pnlIdentityDetails.Name = "pnlIdentityDetails";
            this.pnlIdentityDetails.Size = new System.Drawing.Size(557, 86);
            this.pnlIdentityDetails.TabIndex = 0;
            //
            // lblJobDepartment
            //
            this.lblJobDepartment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblJobDepartment.AutoEllipsis = true;
            this.lblJobDepartment.Location = new System.Drawing.Point(84, 66);
            this.lblJobDepartment.Name = "lblJobDepartment";
            this.lblJobDepartment.Size = new System.Drawing.Size(460, 20);
            this.lblJobDepartment.TabIndex = 3;
            this.lblJobDepartment.Text = "Job / Department";
            //
            // pnlStatusTag
            //
            this.pnlStatusTag.Controls.Add(this.lblStatusTagText);
            this.pnlStatusTag.Location = new System.Drawing.Point(84, 36);
            this.pnlStatusTag.Name = "pnlStatusTag";
            this.pnlStatusTag.Size = new System.Drawing.Size(72, 22);
            this.pnlStatusTag.TabIndex = 2;
            this.pnlStatusTag.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlStatusTag_Paint);
            //
            // lblStatusTagText
            //
            this.lblStatusTagText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatusTagText.Location = new System.Drawing.Point(0, 0);
            this.lblStatusTagText.Name = "lblStatusTagText";
            this.lblStatusTagText.Size = new System.Drawing.Size(72, 22);
            this.lblStatusTagText.TabIndex = 0;
            this.lblStatusTagText.Text = "Active";
            this.lblStatusTagText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblName
            //
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblName.AutoEllipsis = true;
            this.lblName.Location = new System.Drawing.Point(84, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(460, 32);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Employee name";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlInitials
            //
            this.pnlInitials.Location = new System.Drawing.Point(0, 4);
            this.pnlInitials.Name = "pnlInitials";
            this.pnlInitials.Size = new System.Drawing.Size(64, 64);
            this.pnlInitials.TabIndex = 0;
            this.pnlInitials.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlInitials_Paint);
            //
            // pnlActions
            //
            this.pnlActions.Controls.Add(this.btnReactivate);
            this.pnlActions.Controls.Add(this.btnTerminate);
            this.pnlActions.Controls.Add(this.btnUpdate);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlActions.Location = new System.Drawing.Point(0, 110);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Padding = new System.Windows.Forms.Padding(16, 4, 16, 8);
            this.pnlActions.Size = new System.Drawing.Size(589, 44);
            this.pnlActions.TabIndex = 1;
            //
            // btnUpdate
            //
            this.btnUpdate.Location = new System.Drawing.Point(16, 4);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(150, 32);
            this.btnUpdate.TabIndex = 0;
            this.btnUpdate.Text = "Update details";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            //
            // btnTerminate
            //
            this.btnTerminate.Location = new System.Drawing.Point(174, 4);
            this.btnTerminate.Name = "btnTerminate";
            this.btnTerminate.Size = new System.Drawing.Size(120, 32);
            this.btnTerminate.TabIndex = 1;
            this.btnTerminate.Text = "Terminate";
            this.btnTerminate.Click += new System.EventHandler(this.btnTerminate_Click);
            //
            // btnReactivate
            //
            this.btnReactivate.Location = new System.Drawing.Point(302, 4);
            this.btnReactivate.Name = "btnReactivate";
            this.btnReactivate.Size = new System.Drawing.Size(120, 32);
            this.btnReactivate.TabIndex = 2;
            this.btnReactivate.Text = "Reactivate";
            this.btnReactivate.Click += new System.EventHandler(this.btnReactivate_Click);
            //
            // pnlFacts
            //
            this.pnlFacts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFacts.Controls.Add(this.factIDNumber);
            this.pnlFacts.Controls.Add(this.factMobile);
            this.pnlFacts.Controls.Add(this.factStartDate);
            this.pnlFacts.Controls.Add(this.factJob);
            this.pnlFacts.Controls.Add(this.factDepartment);
            this.pnlFacts.Controls.Add(this.factContract);
            this.pnlFacts.Location = new System.Drawing.Point(0, 154);
            this.pnlFacts.Name = "pnlFacts";
            this.pnlFacts.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlFacts.Size = new System.Drawing.Size(589, 300);
            this.pnlFacts.TabIndex = 2;
            this.pnlFacts.WrapContents = true;
            //
            // factIDNumber
            //
            this.factIDNumber.Location = new System.Drawing.Point(12, 8);
            this.factIDNumber.Name = "factIDNumber";
            this.factIDNumber.Size = new System.Drawing.Size(170, 62);
            this.factIDNumber.TabIndex = 0;
            //
            // factMobile
            //
            this.factMobile.Location = new System.Drawing.Point(192, 8);
            this.factMobile.Name = "factMobile";
            this.factMobile.Size = new System.Drawing.Size(170, 62);
            this.factMobile.TabIndex = 1;
            //
            // factStartDate
            //
            this.factStartDate.Location = new System.Drawing.Point(372, 8);
            this.factStartDate.Name = "factStartDate";
            this.factStartDate.Size = new System.Drawing.Size(170, 62);
            this.factStartDate.TabIndex = 2;
            //
            // factJob
            //
            this.factJob.Location = new System.Drawing.Point(12, 80);
            this.factJob.Name = "factJob";
            this.factJob.Size = new System.Drawing.Size(170, 62);
            this.factJob.TabIndex = 3;
            //
            // factDepartment
            //
            this.factDepartment.Location = new System.Drawing.Point(192, 80);
            this.factDepartment.Name = "factDepartment";
            this.factDepartment.Size = new System.Drawing.Size(170, 62);
            this.factDepartment.TabIndex = 4;
            //
            // factContract
            //
            this.factContract.Location = new System.Drawing.Point(372, 80);
            this.factContract.Name = "factContract";
            this.factContract.Size = new System.Drawing.Size(170, 62);
            this.factContract.TabIndex = 5;
            //
            // EmployeeDetailsPanel
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlFacts);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlIdentity);
            this.Name = "EmployeeDetailsPanel";
            this.Size = new System.Drawing.Size(589, 482);
            this.pnlIdentity.ResumeLayout(false);
            this.pnlIdentityDetails.ResumeLayout(false);
            this.pnlStatusTag.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.pnlFacts.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlIdentity;
        private System.Windows.Forms.Label lblEmptyMessage;
        private System.Windows.Forms.Panel pnlIdentityDetails;
        private System.Windows.Forms.Panel pnlInitials;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Panel pnlStatusTag;
        private System.Windows.Forms.Label lblStatusTagText;
        private System.Windows.Forms.Label lblJobDepartment;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnTerminate;
        private System.Windows.Forms.Button btnReactivate;
        private System.Windows.Forms.FlowLayoutPanel pnlFacts;
        private EmployeeTimeManagement.Views.FactBox factIDNumber;
        private EmployeeTimeManagement.Views.FactBox factMobile;
        private EmployeeTimeManagement.Views.FactBox factStartDate;
        private EmployeeTimeManagement.Views.FactBox factJob;
        private EmployeeTimeManagement.Views.FactBox factDepartment;
        private EmployeeTimeManagement.Views.FactBox factContract;
    }
}
