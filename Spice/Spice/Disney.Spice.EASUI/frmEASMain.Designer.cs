namespace Disney.Spice.EASUI
{
    partial class frmEASMain
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.dgvDetails = new System.Windows.Forms.DataGridView();
            this.gbxFilters = new System.Windows.Forms.GroupBox();
            this.cbxDisplayAll = new System.Windows.Forms.CheckBox();
            this.btnApplyFilters = new System.Windows.Forms.Button();
            this.btnResetFilters = new System.Windows.Forms.Button();
            this.gbxDetails = new System.Windows.Forms.GroupBox();
            this.txtbxDetails = new System.Windows.Forms.TextBox();
            this.lblDetails = new System.Windows.Forms.Label();
            this.gbxRequestId = new System.Windows.Forms.GroupBox();
            this.txtbxTo = new System.Windows.Forms.TextBox();
            this.txtbxFrom = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gbxApprovalStatus = new System.Windows.Forms.GroupBox();
            this.chklbApprovalStatus = new System.Windows.Forms.CheckedListBox();
            this.gbxFunction = new System.Windows.Forms.GroupBox();
            this.chklbFunction = new System.Windows.Forms.CheckedListBox();
            this.gbxModule = new System.Windows.Forms.GroupBox();
            this.chklbModule = new System.Windows.Forms.CheckedListBox();
            this.cbxApplication = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).BeginInit();
            this.gbxFilters.SuspendLayout();
            this.gbxDetails.SuspendLayout();
            this.gbxRequestId.SuspendLayout();
            this.gbxApprovalStatus.SuspendLayout();
            this.gbxFunction.SuspendLayout();
            this.gbxModule.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(6, 490);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 0;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(768, 490);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // dgvDetails
            // 
            this.dgvDetails.AllowUserToAddRows = false;
            this.dgvDetails.AllowUserToDeleteRows = false;
            this.dgvDetails.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.dgvDetails.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetails.Location = new System.Drawing.Point(6, 248);
            this.dgvDetails.Name = "dgvDetails";
            this.dgvDetails.ReadOnly = true;
            this.dgvDetails.RowHeadersVisible = false;
            this.dgvDetails.Size = new System.Drawing.Size(836, 232);
            this.dgvDetails.TabIndex = 2;
            this.dgvDetails.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetails_CellContentDoubleClick);
            this.dgvDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvDetails_CellFormatting);
            // 
            // gbxFilters
            // 
            this.gbxFilters.Controls.Add(this.cbxDisplayAll);
            this.gbxFilters.Controls.Add(this.btnApplyFilters);
            this.gbxFilters.Controls.Add(this.btnResetFilters);
            this.gbxFilters.Controls.Add(this.gbxDetails);
            this.gbxFilters.Controls.Add(this.gbxRequestId);
            this.gbxFilters.Controls.Add(this.gbxApprovalStatus);
            this.gbxFilters.Controls.Add(this.gbxFunction);
            this.gbxFilters.Controls.Add(this.gbxModule);
            this.gbxFilters.Controls.Add(this.cbxApplication);
            this.gbxFilters.Controls.Add(this.label1);
            this.gbxFilters.Location = new System.Drawing.Point(6, 12);
            this.gbxFilters.Name = "gbxFilters";
            this.gbxFilters.Size = new System.Drawing.Size(836, 219);
            this.gbxFilters.TabIndex = 3;
            this.gbxFilters.TabStop = false;
            this.gbxFilters.Text = "Filters";
            // 
            // cbxDisplayAll
            // 
            this.cbxDisplayAll.AutoSize = true;
            this.cbxDisplayAll.Location = new System.Drawing.Point(480, 190);
            this.cbxDisplayAll.Name = "cbxDisplayAll";
            this.cbxDisplayAll.Size = new System.Drawing.Size(143, 17);
            this.cbxDisplayAll.TabIndex = 9;
            this.cbxDisplayAll.Text = "Display All (incl. Inactive)";
            this.cbxDisplayAll.UseVisualStyleBackColor = true;
            // 
            // btnApplyFilters
            // 
            this.btnApplyFilters.Location = new System.Drawing.Point(635, 184);
            this.btnApplyFilters.Name = "btnApplyFilters";
            this.btnApplyFilters.Size = new System.Drawing.Size(75, 23);
            this.btnApplyFilters.TabIndex = 8;
            this.btnApplyFilters.Text = "Apply Filters";
            this.btnApplyFilters.UseVisualStyleBackColor = true;
            this.btnApplyFilters.Click += new System.EventHandler(this.btnApplyFilters_Click);
            // 
            // btnResetFilters
            // 
            this.btnResetFilters.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnResetFilters.Location = new System.Drawing.Point(745, 184);
            this.btnResetFilters.Name = "btnResetFilters";
            this.btnResetFilters.Size = new System.Drawing.Size(75, 23);
            this.btnResetFilters.TabIndex = 7;
            this.btnResetFilters.Text = "Reset Filters";
            this.btnResetFilters.UseVisualStyleBackColor = true;
            this.btnResetFilters.Click += new System.EventHandler(this.btnResetFilters_Click);
            // 
            // gbxDetails
            // 
            this.gbxDetails.Controls.Add(this.txtbxDetails);
            this.gbxDetails.Controls.Add(this.lblDetails);
            this.gbxDetails.Location = new System.Drawing.Point(464, 133);
            this.gbxDetails.Name = "gbxDetails";
            this.gbxDetails.Size = new System.Drawing.Size(356, 44);
            this.gbxDetails.TabIndex = 6;
            this.gbxDetails.TabStop = false;
            // 
            // txtbxDetails
            // 
            this.txtbxDetails.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbxDetails.Location = new System.Drawing.Point(56, 15);
            this.txtbxDetails.Name = "txtbxDetails";
            this.txtbxDetails.Size = new System.Drawing.Size(294, 20);
            this.txtbxDetails.TabIndex = 1;
            // 
            // lblDetails
            // 
            this.lblDetails.AutoSize = true;
            this.lblDetails.Location = new System.Drawing.Point(11, 18);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(39, 13);
            this.lblDetails.TabIndex = 0;
            this.lblDetails.Text = "Details";
            // 
            // gbxRequestId
            // 
            this.gbxRequestId.Controls.Add(this.txtbxTo);
            this.gbxRequestId.Controls.Add(this.txtbxFrom);
            this.gbxRequestId.Controls.Add(this.label3);
            this.gbxRequestId.Controls.Add(this.label2);
            this.gbxRequestId.Location = new System.Drawing.Point(635, 20);
            this.gbxRequestId.Name = "gbxRequestId";
            this.gbxRequestId.Size = new System.Drawing.Size(185, 107);
            this.gbxRequestId.TabIndex = 5;
            this.gbxRequestId.TabStop = false;
            this.gbxRequestId.Text = "Request Id";
            // 
            // txtbxTo
            // 
            this.txtbxTo.Location = new System.Drawing.Point(54, 59);
            this.txtbxTo.MaxLength = 9;
            this.txtbxTo.Name = "txtbxTo";
            this.txtbxTo.Size = new System.Drawing.Size(86, 20);
            this.txtbxTo.TabIndex = 3;
            // 
            // txtbxFrom
            // 
            this.txtbxFrom.Location = new System.Drawing.Point(54, 31);
            this.txtbxFrom.MaxLength = 9;
            this.txtbxFrom.Name = "txtbxFrom";
            this.txtbxFrom.Size = new System.Drawing.Size(86, 20);
            this.txtbxFrom.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "To";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "From";
            // 
            // gbxApprovalStatus
            // 
            this.gbxApprovalStatus.Controls.Add(this.chklbApprovalStatus);
            this.gbxApprovalStatus.Location = new System.Drawing.Point(464, 19);
            this.gbxApprovalStatus.Name = "gbxApprovalStatus";
            this.gbxApprovalStatus.Size = new System.Drawing.Size(149, 108);
            this.gbxApprovalStatus.TabIndex = 4;
            this.gbxApprovalStatus.TabStop = false;
            this.gbxApprovalStatus.Text = "Approval Status";
            // 
            // chklbApprovalStatus
            // 
            this.chklbApprovalStatus.CheckOnClick = true;
            this.chklbApprovalStatus.FormattingEnabled = true;
            this.chklbApprovalStatus.Location = new System.Drawing.Point(6, 19);
            this.chklbApprovalStatus.Name = "chklbApprovalStatus";
            this.chklbApprovalStatus.Size = new System.Drawing.Size(137, 79);
            this.chklbApprovalStatus.TabIndex = 0;
            // 
            // gbxFunction
            // 
            this.gbxFunction.Controls.Add(this.chklbFunction);
            this.gbxFunction.Location = new System.Drawing.Point(200, 19);
            this.gbxFunction.Name = "gbxFunction";
            this.gbxFunction.Size = new System.Drawing.Size(258, 194);
            this.gbxFunction.TabIndex = 3;
            this.gbxFunction.TabStop = false;
            this.gbxFunction.Text = "Function";
            // 
            // chklbFunction
            // 
            this.chklbFunction.CheckOnClick = true;
            this.chklbFunction.FormattingEnabled = true;
            this.chklbFunction.Location = new System.Drawing.Point(6, 19);
            this.chklbFunction.Name = "chklbFunction";
            this.chklbFunction.Size = new System.Drawing.Size(246, 169);
            this.chklbFunction.TabIndex = 0;
            // 
            // gbxModule
            // 
            this.gbxModule.Controls.Add(this.chklbModule);
            this.gbxModule.Location = new System.Drawing.Point(9, 46);
            this.gbxModule.Name = "gbxModule";
            this.gbxModule.Size = new System.Drawing.Size(184, 167);
            this.gbxModule.TabIndex = 2;
            this.gbxModule.TabStop = false;
            this.gbxModule.Text = "Module";
            // 
            // chklbModule
            // 
            this.chklbModule.CheckOnClick = true;
            this.chklbModule.FormattingEnabled = true;
            this.chklbModule.Location = new System.Drawing.Point(6, 23);
            this.chklbModule.Name = "chklbModule";
            this.chklbModule.Size = new System.Drawing.Size(165, 139);
            this.chklbModule.TabIndex = 0;
            this.chklbModule.SelectedIndexChanged += new System.EventHandler(this.chklbModule_SelectedIndexChanged);
            // 
            // cbxApplication
            // 
            this.cbxApplication.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxApplication.FormattingEnabled = true;
            this.cbxApplication.Location = new System.Drawing.Point(65, 19);
            this.cbxApplication.Name = "cbxApplication";
            this.cbxApplication.Size = new System.Drawing.Size(128, 21);
            this.cbxApplication.TabIndex = 1;
            this.cbxApplication.SelectedIndexChanged += new System.EventHandler(this.cbxApplication_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Application";
            // 
            // frmEASMain
            // 
            this.AcceptButton = this.btnApplyFilters;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnResetFilters;
            this.ClientSize = new System.Drawing.Size(854, 529);
            this.Controls.Add(this.gbxFilters);
            this.Controls.Add(this.dgvDetails);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnHelp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEASMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SPICE - EAS - Approval Request Management - Approve/Reject";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).EndInit();
            this.gbxFilters.ResumeLayout(false);
            this.gbxFilters.PerformLayout();
            this.gbxDetails.ResumeLayout(false);
            this.gbxDetails.PerformLayout();
            this.gbxRequestId.ResumeLayout(false);
            this.gbxRequestId.PerformLayout();
            this.gbxApprovalStatus.ResumeLayout(false);
            this.gbxFunction.ResumeLayout(false);
            this.gbxModule.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.DataGridView dgvDetails;
        private System.Windows.Forms.GroupBox gbxFilters;
        private System.Windows.Forms.ComboBox cbxApplication;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbxModule;
        private System.Windows.Forms.CheckedListBox chklbModule;
        private System.Windows.Forms.GroupBox gbxFunction;
        private System.Windows.Forms.CheckedListBox chklbFunction;
        private System.Windows.Forms.GroupBox gbxApprovalStatus;
        private System.Windows.Forms.GroupBox gbxRequestId;
        private System.Windows.Forms.GroupBox gbxDetails;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.TextBox txtbxDetails;
        private System.Windows.Forms.CheckedListBox chklbApprovalStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtbxTo;
        private System.Windows.Forms.TextBox txtbxFrom;
        private System.Windows.Forms.Button btnResetFilters;
        private System.Windows.Forms.Button btnApplyFilters;
        private System.Windows.Forms.CheckBox cbxDisplayAll;
    }
}