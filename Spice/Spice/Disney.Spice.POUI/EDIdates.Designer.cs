namespace Disney.Spice.POUI
{
    partial class EDIdates
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
            this.txtSCB = new System.Windows.Forms.TextBox();
            this.txtOOCL = new System.Windows.Forms.TextBox();
            this.txtAvery = new System.Windows.Forms.TextBox();
            this.lblSCB = new System.Windows.Forms.Label();
            this.lblOOCL = new System.Windows.Forms.Label();
            this.lblAvery = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtSCB
            // 
            this.txtSCB.Enabled = false;
            this.txtSCB.Location = new System.Drawing.Point(144, 45);
            this.txtSCB.Name = "txtSCB";
            this.txtSCB.Size = new System.Drawing.Size(100, 20);
            this.txtSCB.TabIndex = 0;
            // 
            // txtOOCL
            // 
            this.txtOOCL.Enabled = false;
            this.txtOOCL.Location = new System.Drawing.Point(144, 109);
            this.txtOOCL.Name = "txtOOCL";
            this.txtOOCL.Size = new System.Drawing.Size(100, 20);
            this.txtOOCL.TabIndex = 1;
            // 
            // txtAvery
            // 
            this.txtAvery.Enabled = false;
            this.txtAvery.Location = new System.Drawing.Point(144, 169);
            this.txtAvery.Name = "txtAvery";
            this.txtAvery.Size = new System.Drawing.Size(100, 20);
            this.txtAvery.TabIndex = 2;
            // 
            // lblSCB
            // 
            this.lblSCB.AutoSize = true;
            this.lblSCB.Location = new System.Drawing.Point(16, 48);
            this.lblSCB.Name = "lblSCB";
            this.lblSCB.Size = new System.Drawing.Size(91, 13);
            this.lblSCB.TabIndex = 3;
            this.lblSCB.Text = "Date Sent to SCB";
            // 
            // lblOOCL
            // 
            this.lblOOCL.AutoSize = true;
            this.lblOOCL.Location = new System.Drawing.Point(16, 112);
            this.lblOOCL.Name = "lblOOCL";
            this.lblOOCL.Size = new System.Drawing.Size(99, 13);
            this.lblOOCL.TabIndex = 4;
            this.lblOOCL.Text = "Date Sent to OOCL";
            // 
            // lblAvery
            // 
            this.lblAvery.AutoSize = true;
            this.lblAvery.Location = new System.Drawing.Point(16, 172);
            this.lblAvery.Name = "lblAvery";
            this.lblAvery.Size = new System.Drawing.Size(97, 13);
            this.lblAvery.TabIndex = 5;
            this.lblAvery.Text = "Date Sent to Avery";
            // 
            // EDIdates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 251);
            this.Controls.Add(this.lblAvery);
            this.Controls.Add(this.lblOOCL);
            this.Controls.Add(this.lblSCB);
            this.Controls.Add(this.txtAvery);
            this.Controls.Add(this.txtOOCL);
            this.Controls.Add(this.txtSCB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EDIdates";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EDI Dates";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSCB;
        private System.Windows.Forms.TextBox txtOOCL;
        private System.Windows.Forms.TextBox txtAvery;
        private System.Windows.Forms.Label lblSCB;
        private System.Windows.Forms.Label lblOOCL;
        private System.Windows.Forms.Label lblAvery;
    }
}