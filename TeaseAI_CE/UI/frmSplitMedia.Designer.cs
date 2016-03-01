namespace TeaseAI_CE.UI
{
	partial class frmSplitMedia
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
			this.media1 = new TeaseAI_CE.UI.Media();
			this.SuspendLayout();
			// 
			// media1
			// 
			this.media1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.media1.Location = new System.Drawing.Point(0, 0);
			this.media1.Name = "media1";
			this.media1.Size = new System.Drawing.Size(284, 261);
			this.media1.TabIndex = 0;
			// 
			// frmSplitMedia
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Controls.Add(this.media1);
			this.Name = "frmSplitMedia";
			this.Text = "TeaseAI CE - Media";
			this.ResumeLayout(false);

		}

		#endregion

		private Media media1;
	}
}