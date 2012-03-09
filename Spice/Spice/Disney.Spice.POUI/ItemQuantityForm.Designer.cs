namespace Disney.Spice.POUI
{
    partial class ItemQuantityForm
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
            this.grpBxItemQty = new System.Windows.Forms.GroupBox();
            this.lblDownQty = new System.Windows.Forms.Label();
            this.lblUpQty = new System.Windows.Forms.Label();
            this.rdBtnDown = new System.Windows.Forms.RadioButton();
            this.rdBtnUp = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpBxItemQty.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpBxItemQty
            // 
            this.grpBxItemQty.Controls.Add(this.lblDownQty);
            this.grpBxItemQty.Controls.Add(this.lblUpQty);
            this.grpBxItemQty.Controls.Add(this.rdBtnDown);
            this.grpBxItemQty.Controls.Add(this.rdBtnUp);
            this.grpBxItemQty.Location = new System.Drawing.Point(10, 6);
            this.grpBxItemQty.Name = "grpBxItemQty";
            this.grpBxItemQty.Size = new System.Drawing.Size(188, 76);
            this.grpBxItemQty.TabIndex = 0;
            this.grpBxItemQty.TabStop = false;
            this.grpBxItemQty.Text = "Confirm Quantity";
            // 
            // lblDownQty
            // 
            this.lblDownQty.AutoSize = true;
            this.lblDownQty.Location = new System.Drawing.Point(122, 52);
            this.lblDownQty.Name = "lblDownQty";
            this.lblDownQty.Size = new System.Drawing.Size(51, 13);
            this.lblDownQty.TabIndex = 3;
            this.lblDownQty.Text = "DownQty";
            this.lblDownQty.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblUpQty
            // 
            this.lblUpQty.AutoSize = true;
            this.lblUpQty.Location = new System.Drawing.Point(122, 28);
            this.lblUpQty.Name = "lblUpQty";
            this.lblUpQty.Size = new System.Drawing.Size(37, 13);
            this.lblUpQty.TabIndex = 2;
            this.lblUpQty.Text = "UpQty";
            this.lblUpQty.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // rdBtnDown
            // 
            this.rdBtnDown.AutoSize = true;
            this.rdBtnDown.Location = new System.Drawing.Point(16, 48);
            this.rdBtnDown.Name = "rdBtnDown";
            this.rdBtnDown.Size = new System.Drawing.Size(88, 17);
            this.rdBtnDown.TabIndex = 1;
            this.rdBtnDown.TabStop = true;
            this.rdBtnDown.Text = "Round Down";
            this.rdBtnDown.UseVisualStyleBackColor = true;
            this.rdBtnDown.CheckedChanged += new System.EventHandler(this.rdBtnDown_CheckedChanged);
            // 
            // rdBtnUp
            // 
            this.rdBtnUp.AutoSize = true;
            this.rdBtnUp.Location = new System.Drawing.Point(16, 25);
            this.rdBtnUp.Name = "rdBtnUp";
            this.rdBtnUp.Size = new System.Drawing.Size(74, 17);
            this.rdBtnUp.TabIndex = 0;
            this.rdBtnUp.TabStop = true;
            this.rdBtnUp.Text = "Round Up";
            this.rdBtnUp.UseVisualStyleBackColor = true;
            this.rdBtnUp.CheckedChanged += new System.EventHandler(this.rdBtnUp_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(24, 90);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(108, 91);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ItemQuantityForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(208, 122);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpBxItemQty);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ItemQuantityForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SPICE Item Quantity";
            this.grpBxItemQty.ResumeLayout(false);
            this.grpBxItemQty.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBxItemQty;
        private System.Windows.Forms.RadioButton rdBtnDown;
        private System.Windows.Forms.RadioButton rdBtnUp;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblDownQty;
        private System.Windows.Forms.Label lblUpQty;

    }
}