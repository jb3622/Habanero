namespace Disney.Menu
{
	partial class MessageBox
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.checkBox = new System.Windows.Forms.CheckBox();
			this.label = new System.Windows.Forms.Label();
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// checkBox
			// 
			this.checkBox.AutoSize = true;
			this.checkBox.Location = new System.Drawing.Point(0, 0);
			this.checkBox.Name = "checkBox";
			this.checkBox.Size = new System.Drawing.Size(104, 24);
			this.checkBox.TabIndex = 0;
			this.checkBox.Text = "checkBox1";
			this.checkBox.UseVisualStyleBackColor = true;
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Location = new System.Drawing.Point(0, 0);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(100, 23);
			this.label.TabIndex = 0;
			this.label.Text = "label1";

		}

		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.CheckBox checkBox;
		private System.Windows.Forms.Label label;
	}
}