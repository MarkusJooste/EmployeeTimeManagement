namespace EmployeeTimeManagement.Views
{
    partial class CaptureTimesheetsForm
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
            this.lblWorkDate = new System.Windows.Forms.Label();
            this.dtpWorkDate = new System.Windows.Forms.DateTimePicker();
            this.lblDayType = new System.Windows.Forms.Label();
            this.dgvCapture = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCapture)).BeginInit();
            this.SuspendLayout();
            //
            // lblWorkDate
            //
            this.lblWorkDate.AutoSize = true;
            this.lblWorkDate.Location = new System.Drawing.Point(12, 15);
            this.lblWorkDate.Name = "lblWorkDate";
            this.lblWorkDate.Size = new System.Drawing.Size(56, 13);
            this.lblWorkDate.TabIndex = 0;
            this.lblWorkDate.Text = "Work date";
            //
            // dtpWorkDate
            //
            this.dtpWorkDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpWorkDate.Location = new System.Drawing.Point(78, 11);
            this.dtpWorkDate.Name = "dtpWorkDate";
            this.dtpWorkDate.Size = new System.Drawing.Size(120, 20);
            this.dtpWorkDate.TabIndex = 1;
            this.dtpWorkDate.ValueChanged += new System.EventHandler(this.dtpWorkDate_ValueChanged);
            //
            // lblDayType
            //
            this.lblDayType.AutoSize = true;
            this.lblDayType.Location = new System.Drawing.Point(213, 15);
            this.lblDayType.Name = "lblDayType";
            this.lblDayType.Size = new System.Drawing.Size(40, 13);
            this.lblDayType.TabIndex = 2;
            this.lblDayType.Text = "Normal";
            //
            // dgvCapture
            //
            this.dgvCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCapture.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCapture.Location = new System.Drawing.Point(12, 45);
            this.dgvCapture.Name = "dgvCapture";
            this.dgvCapture.RowHeadersWidth = 25;
            this.dgvCapture.Size = new System.Drawing.Size(876, 468);
            this.dgvCapture.TabIndex = 3;
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(813, 522);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 528);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 5;
            //
            // CaptureTimesheetsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 560);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvCapture);
            this.Controls.Add(this.lblDayType);
            this.Controls.Add(this.dtpWorkDate);
            this.Controls.Add(this.lblWorkDate);
            this.Name = "CaptureTimesheetsForm";
            this.Text = "CaptureTimesheetsForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCapture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWorkDate;
        private System.Windows.Forms.DateTimePicker dtpWorkDate;
        private System.Windows.Forms.Label lblDayType;
        private System.Windows.Forms.DataGridView dgvCapture;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblStatus;
    }
}
