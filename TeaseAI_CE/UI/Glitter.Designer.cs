namespace TeaseAI_CE.UI
{
	partial class Glitter
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
			this.webBrowserForChat1 = new TeaseAI_CE.UI.WebBrowserForChat();
			this.SuspendLayout();
			// 
			// webBrowserForChat1
			// 
			this.webBrowserForChat1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowserForChat1.Location = new System.Drawing.Point(0, 0);
			this.webBrowserForChat1.Name = "webBrowserForChat1";
			this.webBrowserForChat1.Size = new System.Drawing.Size(253, 396);
			this.webBrowserForChat1.TabIndex = 0;
			this.webBrowserForChat1.Text = "webBrowserForChat1";
			// 
			// Glitter
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.webBrowserForChat1);
			this.Name = "Glitter";
			this.Size = new System.Drawing.Size(253, 396);
			this.ResumeLayout(false);

		}

		#endregion

		private WebBrowserForChat webBrowserForChat1;
	}
}
