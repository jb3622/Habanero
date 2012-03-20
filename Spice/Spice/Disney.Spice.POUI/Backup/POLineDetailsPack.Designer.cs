namespace Disney.Spice.POUI
{
    partial class POLineDetailsPack
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
            this.grpBxPackInfo = new System.Windows.Forms.GroupBox();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblVendorRef = new System.Windows.Forms.Label();
            this.lblVendorRefValue = new System.Windows.Forms.Label();
            this.lblDescriptionValue1 = new System.Windows.Forms.Label();
            this.lblDescriptionValue = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblVendor = new System.Windows.Forms.Label();
            this.lblVendorValue = new System.Windows.Forms.Label();
            this.lblClassName = new System.Windows.Forms.Label();
            this.lblClassNameValue = new System.Windows.Forms.Label();
            this.lblLongItemValue = new System.Windows.Forms.Label();
            this.lblLongItem = new System.Windows.Forms.Label();
            this.dtgrdVwPOLineDetailPack = new System.Windows.Forms.DataGridView();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpBxPackInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtgrdVwPOLineDetailPack)).BeginInit();
            this.SuspendLayout();
            // 
            // grpBxPackInfo
            // 
            this.grpBxPackInfo.Controls.Add(this.txtQuantity);
            this.grpBxPackInfo.Controls.Add(this.label1);
            this.grpBxPackInfo.Controls.Add(this.lblVendorRef);
            this.grpBxPackInfo.Controls.Add(this.lblVendorRefValue);
            this.grpBxPackInfo.Controls.Add(this.lblDescriptionValue1);
            this.grpBxPackInfo.Controls.Add(this.lblDescriptionValue);
            this.grpBxPackInfo.Controls.Add(this.lblDescription);
            this.grpBxPackInfo.Controls.Add(this.lblVendor);
            this.grpBxPackInfo.Controls.Add(this.lblVendorValue);
            this.grpBxPackInfo.Controls.Add(this.lblClassName);
            this.grpBxPackInfo.Controls.Add(this.lblClassNameValue);
            this.grpBxPackInfo.Controls.Add(this.lblLongItemValue);
            this.grpBxPackInfo.Controls.Add(this.lblLongItem);
            this.grpBxPackInfo.Location = new System.Drawing.Point(8, 7);
            this.grpBxPackInfo.Name = "grpBxPackInfo";
            this.grpBxPackInfo.Size = new System.Drawing.Size(791, 111);
            this.grpBxPackInfo.TabIndex = 0;
            this.grpBxPackInfo.TabStop = false;
            this.grpBxPackInfo.Enter += new System.EventHandler(this.grpBxPackInfo_Enter);
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(685, 70);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(93, 20);
            this.txtQuantity.TabIndex = 12;
            this.txtQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtQuantity.Validated += new System.EventHandler(this.txtQuantity_Validated);
            this.txtQuantity.Validating += new System.ComponentModel.CancelEventHandler(this.txtQuantity_Validating);
            this.txtQuantity.TextChanged += new System.EventHandler(this.txtQuantity_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(612, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Quantity";
            // 
            // lblVendorRef
            // 
            this.lblVendorRef.AutoSize = true;
            this.lblVendorRef.Location = new System.Drawing.Point(335, 73);
            this.lblVendorRef.Name = "lblVendorRef";
            this.lblVendorRef.Size = new System.Drawing.Size(94, 13);
            this.lblVendorRef.TabIndex = 10;
            this.lblVendorRef.Text = "Vendor Reference";
            // 
            // lblVendorRefValue
            // 
            this.lblVendorRefValue.AutoSize = true;
            this.lblVendorRefValue.Location = new System.Drawing.Point(436, 73);
            this.lblVendorRefValue.Name = "lblVendorRefValue";
            this.lblVendorRefValue.Size = new System.Drawing.Size(0, 13);
            this.lblVendorRefValue.TabIndex = 9;
            // 
            // lblDescriptionValue1
            // 
            this.lblDescriptionValue1.AutoSize = true;
            this.lblDescriptionValue1.Location = new System.Drawing.Point(436, 46);
            this.lblDescriptionValue1.Name = "lblDescriptionValue1";
            this.lblDescriptionValue1.Size = new System.Drawing.Size(0, 13);
            this.lblDescriptionValue1.TabIndex = 8;
            // 
            // lblDescriptionValue
            // 
            this.lblDescriptionValue.AutoSize = true;
            this.lblDescriptionValue.Location = new System.Drawing.Point(436, 27);
            this.lblDescriptionValue.Name = "lblDescriptionValue";
            this.lblDescriptionValue.Size = new System.Drawing.Size(0, 13);
            this.lblDescriptionValue.TabIndex = 7;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(335, 27);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(65, 13);
            this.lblDescription.TabIndex = 6;
            this.lblDescription.Text = "Descriptions";
            // 
            // lblVendor
            // 
            this.lblVendor.AutoSize = true;
            this.lblVendor.Location = new System.Drawing.Point(19, 73);
            this.lblVendor.Name = "lblVendor";
            this.lblVendor.Size = new System.Drawing.Size(41, 13);
            this.lblVendor.TabIndex = 5;
            this.lblVendor.Text = "Vendor";
            // 
            // lblVendorValue
            // 
            this.lblVendorValue.AutoSize = true;
            this.lblVendorValue.Location = new System.Drawing.Point(99, 73);
            this.lblVendorValue.Name = "lblVendorValue";
            this.lblVendorValue.Size = new System.Drawing.Size(0, 13);
            this.lblVendorValue.TabIndex = 4;
            // 
            // lblClassName
            // 
            this.lblClassName.AutoSize = true;
            this.lblClassName.Location = new System.Drawing.Point(19, 46);
            this.lblClassName.Name = "lblClassName";
            this.lblClassName.Size = new System.Drawing.Size(63, 13);
            this.lblClassName.TabIndex = 3;
            this.lblClassName.Text = "Class Name";
            // 
            // lblClassNameValue
            // 
            this.lblClassNameValue.AutoSize = true;
            this.lblClassNameValue.Location = new System.Drawing.Point(99, 46);
            this.lblClassNameValue.Name = "lblClassNameValue";
            this.lblClassNameValue.Size = new System.Drawing.Size(0, 13);
            this.lblClassNameValue.TabIndex = 2;
            // 
            // lblLongItemValue
            // 
            this.lblLongItemValue.AutoSize = true;
            this.lblLongItemValue.Location = new System.Drawing.Point(99, 20);
            this.lblLongItemValue.Name = "lblLongItemValue";
            this.lblLongItemValue.Size = new System.Drawing.Size(0, 13);
            this.lblLongItemValue.TabIndex = 1;
            // 
            // lblLongItem
            // 
            this.lblLongItem.AutoSize = true;
            this.lblLongItem.Location = new System.Drawing.Point(19, 20);
            this.lblLongItem.Name = "lblLongItem";
            this.lblLongItem.Size = new System.Drawing.Size(54, 13);
            this.lblLongItem.TabIndex = 0;
            this.lblLongItem.Text = "Long Item";
            // 
            // dtgrdVwPOLineDetailPack
            // 
            this.dtgrdVwPOLineDetailPack.AllowUserToAddRows = false;
            this.dtgrdVwPOLineDetailPack.AllowUserToDeleteRows = false;
            this.dtgrdVwPOLineDetailPack.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtgrdVwPOLineDetailPack.Location = new System.Drawing.Point(8, 126);
            this.dtgrdVwPOLineDetailPack.Name = "dtgrdVwPOLineDetailPack";
            this.dtgrdVwPOLineDetailPack.Size = new System.Drawing.Size(791, 243);
            this.dtgrdVwPOLineDetailPack.TabIndex = 1;
            this.dtgrdVwPOLineDetailPack.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtgrdVwPOLineDetailPack_CellDoubleClick);
            this.dtgrdVwPOLineDetailPack.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtgrdVwPOLineDetailPack_CellValidated);
            this.dtgrdVwPOLineDetailPack.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtgrdVwPOLineDetailPack_CellContentDoubleClick);
            this.dtgrdVwPOLineDetailPack.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dtgrdVwPOLineDetailPack_CellValidating);
            // 
            // btnHelp
            // 
            this.btnHelp.CausesValidation = false;
            this.btnHelp.Location = new System.Drawing.Point(8, 381);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(77, 25);
            this.btnHelp.TabIndex = 2;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(639, 381);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(77, 25);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point(722, 381);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(77, 25);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // POLineDetailsPack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 419);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.dtgrdVwPOLineDetailPack);
            this.Controls.Add(this.grpBxPackInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "POLineDetailsPack";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SPICE PO Entry - PO Line Details (Pack)";
            this.Load += new System.EventHandler(this.POLineDetailsPack_Load);
            this.grpBxPackInfo.ResumeLayout(false);
            this.grpBxPackInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtgrdVwPOLineDetailPack)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBxPackInfo;
        private System.Windows.Forms.Label lblClassName;
        private System.Windows.Forms.Label lblClassNameValue;
        private System.Windows.Forms.Label lblLongItemValue;
        private System.Windows.Forms.Label lblLongItem;
        private System.Windows.Forms.Label lblVendor;
        private System.Windows.Forms.Label lblVendorValue;
        private System.Windows.Forms.Label lblDescriptionValue;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblDescriptionValue1;
        private System.Windows.Forms.Label lblVendorRef;
        private System.Windows.Forms.Label lblVendorRefValue;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dtgrdVwPOLineDetailPack;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}