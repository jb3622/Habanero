namespace Disney.Spice.POUI
{
    partial class DropShipMatrixPO
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
            this.dtgrdViewDropShipMatrix = new System.Windows.Forms.DataGridView();
            this.LongItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QtyAssigned = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStore1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dtgrdViewDropShipMatrix)).BeginInit();
            this.SuspendLayout();
            // 
            // dtgrdViewDropShipMatrix
            // 
            this.dtgrdViewDropShipMatrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtgrdViewDropShipMatrix.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LongItem,
            this.TotalQty,
            this.QtyAssigned,
            this.colStore1});
            this.dtgrdViewDropShipMatrix.Location = new System.Drawing.Point(13, 30);
            this.dtgrdViewDropShipMatrix.Name = "dtgrdViewDropShipMatrix";
            this.dtgrdViewDropShipMatrix.Size = new System.Drawing.Size(660, 150);
            this.dtgrdViewDropShipMatrix.TabIndex = 0;
            this.dtgrdViewDropShipMatrix.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtgrdViewDropShipMatrix_CellLeave);
            // 
            // LongItem
            // 
            this.LongItem.HeaderText = "LongItem";
            this.LongItem.Name = "LongItem";
            // 
            // TotalQty
            // 
            this.TotalQty.HeaderText = "Total Quantity";
            this.TotalQty.Name = "TotalQty";
            // 
            // QtyAssigned
            // 
            this.QtyAssigned.HeaderText = "Qty Assigned";
            this.QtyAssigned.Name = "QtyAssigned";
            // 
            // colStore1
            // 
            this.colStore1.HeaderText = "741-CRIBBS CAUSEWAY";
            this.colStore1.Name = "colStore1";
            // 
            // btnHelp
            // 
            this.btnHelp.CausesValidation = false;
            this.btnHelp.Location = new System.Drawing.Point(13, 238);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 65;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point(598, 238);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 66;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(517, 238);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 67;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // DropShipMatrixPO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 273);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.dtgrdViewDropShipMatrix);
            this.Name = "DropShipMatrixPO";
            this.Text = "SPICE -PO Entry- Drop Ship (Matrix)  PO";
            ((System.ComponentModel.ISupportInitialize)(this.dtgrdViewDropShipMatrix)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dtgrdViewDropShipMatrix;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn LongItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn QtyAssigned;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStore1;
    }
}