namespace Disney.Spice.POUI
{
    partial class MarketSelectionForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarketSelectionForm));
            this.lblMarketSelectionDesc = new System.Windows.Forms.Label();
            this.pctBxMarketLookup = new System.Windows.Forms.PictureBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblMarket = new System.Windows.Forms.Label();
            this.grpBoxMarketSelection = new System.Windows.Forms.GroupBox();
            this.txtMarketSelection = new System.Windows.Forms.TextBox();
            this.errMarketSelection = new System.Windows.Forms.ErrorProvider(this.components);
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pctBxMarketLookup)).BeginInit();
            this.grpBoxMarketSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errMarketSelection)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMarketSelectionDesc
            // 
            this.lblMarketSelectionDesc.AutoSize = true;
            this.lblMarketSelectionDesc.Location = new System.Drawing.Point(162, 50);
            this.lblMarketSelectionDesc.Name = "lblMarketSelectionDesc";
            this.lblMarketSelectionDesc.Size = new System.Drawing.Size(0, 13);
            this.lblMarketSelectionDesc.TabIndex = 2;
            // 
            // pctBxMarketLookup
            // 
            this.pctBxMarketLookup.Image = ((System.Drawing.Image)(resources.GetObject("pctBxMarketLookup.Image")));
            this.pctBxMarketLookup.Location = new System.Drawing.Point(136, 47);
            this.pctBxMarketLookup.Name = "pctBxMarketLookup";
            this.pctBxMarketLookup.Size = new System.Drawing.Size(20, 20);
            this.pctBxMarketLookup.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctBxMarketLookup.TabIndex = 3;
            this.pctBxMarketLookup.TabStop = false;
            this.pctBxMarketLookup.Click += new System.EventHandler(this.pctBxMarketLookup_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.CausesValidation = false;
            this.btnHelp.Location = new System.Drawing.Point(12, 127);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 7;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(172, 127);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(253, 127);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblMarket
            // 
            this.lblMarket.AutoSize = true;
            this.lblMarket.Location = new System.Drawing.Point(28, 50);
            this.lblMarket.Name = "lblMarket";
            this.lblMarket.Size = new System.Drawing.Size(40, 13);
            this.lblMarket.TabIndex = 1;
            this.lblMarket.Text = "Market";
            // 
            // grpBoxMarketSelection
            // 
            this.grpBoxMarketSelection.Controls.Add(this.pctBxMarketLookup);
            this.grpBoxMarketSelection.Controls.Add(this.lblMarketSelectionDesc);
            this.grpBoxMarketSelection.Controls.Add(this.lblMarket);
            this.grpBoxMarketSelection.Controls.Add(this.txtMarketSelection);
            this.grpBoxMarketSelection.Location = new System.Drawing.Point(12, 9);
            this.grpBoxMarketSelection.Name = "grpBoxMarketSelection";
            this.grpBoxMarketSelection.Size = new System.Drawing.Size(316, 112);
            this.grpBoxMarketSelection.TabIndex = 4;
            this.grpBoxMarketSelection.TabStop = false;
            this.grpBoxMarketSelection.Text = "Market Selection";
            // 
            // txtMarketSelection
            // 
            this.txtMarketSelection.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMarketSelection.Location = new System.Drawing.Point(85, 47);
            this.txtMarketSelection.MaxLength = 2;
            this.txtMarketSelection.Name = "txtMarketSelection";
            this.txtMarketSelection.Size = new System.Drawing.Size(45, 20);
            this.txtMarketSelection.TabIndex = 0;
            this.txtMarketSelection.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMarketSelection.Validating += new System.ComponentModel.CancelEventHandler(this.txtMarketSelection_Validating);
            // 
            // errMarketSelection
            // 
            this.errMarketSelection.ContainerControl = this;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Magnifying-Glass-256x256.png");
            // 
            // MarketSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(340, 159);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpBoxMarketSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MarketSelectionForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SPICE - PO Entry - Market Selection";
            ((System.ComponentModel.ISupportInitialize)(this.pctBxMarketLookup)).EndInit();
            this.grpBoxMarketSelection.ResumeLayout(false);
            this.grpBoxMarketSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errMarketSelection)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMarketSelectionDesc;
        private System.Windows.Forms.PictureBox pctBxMarketLookup;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblMarket;
        private System.Windows.Forms.GroupBox grpBoxMarketSelection;
        private System.Windows.Forms.TextBox txtMarketSelection;
        private System.Windows.Forms.ErrorProvider errMarketSelection;
        private System.Windows.Forms.ImageList imageList;
    }
}
