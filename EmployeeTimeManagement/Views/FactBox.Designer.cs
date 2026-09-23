namespace EmployeeTimeManagement.Views
{
    partial class FactBox
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
            this.components = new System.ComponentModel.Container();
            this.lblCaption = new System.Windows.Forms.Label();
            this.lblValue = new System.Windows.Forms.Label();
            this.tipValue = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            //
            // lblCaption
            //
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new System.Drawing.Point(10, 8);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(46, 13);
            this.lblCaption.TabIndex = 0;
            this.lblCaption.Text = "Caption";
            //
            // lblValue
            //
            this.lblValue.AutoEllipsis = true;
            this.lblValue.AutoSize = false;
            this.lblValue.Location = new System.Drawing.Point(10, 24);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(150, 34);
            this.lblValue.TabIndex = 1;
            this.lblValue.Text = "Value";
            //
            // FactBox
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.lblCaption);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 10, 10);
            this.Name = "FactBox";
            this.Size = new System.Drawing.Size(170, 62);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.ToolTip tipValue;
    }
}
