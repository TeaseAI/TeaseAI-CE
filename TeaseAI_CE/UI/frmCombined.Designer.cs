namespace TeaseAI_CE.UI
{
	partial class frmCombined
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.Glitter = new TeaseAI_CE.UI.Glitter();
			this.media1 = new TeaseAI_CE.UI.Media();
			this.Chat = new TeaseAI_CE.UI.Chat();
			this.header1 = new TeaseAI_CE.UI.Header();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.Glitter);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(724, 521);
			this.splitContainer1.SplitterDistance = 165;
			this.splitContainer1.TabIndex = 2;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.media1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.Chat);
			this.splitContainer2.Size = new System.Drawing.Size(555, 521);
			this.splitContainer2.SplitterDistance = 359;
			this.splitContainer2.TabIndex = 3;
			// 
			// Glitter
			// 
			this.Glitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Glitter.Location = new System.Drawing.Point(0, 0);
			this.Glitter.Name = "Glitter";
			this.Glitter.Size = new System.Drawing.Size(165, 521);
			this.Glitter.TabIndex = 0;
			// 
			// media1
			// 
			this.media1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.media1.Location = new System.Drawing.Point(0, 0);
			this.media1.Name = "media1";
			this.media1.Size = new System.Drawing.Size(555, 359);
			this.media1.TabIndex = 0;
			// 
			// Chat
			// 
			this.Chat.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Chat.Location = new System.Drawing.Point(0, 0);
			this.Chat.Name = "Chat";
			this.Chat.Size = new System.Drawing.Size(555, 158);
			this.Chat.TabIndex = 0;
			// 
			// header1
			// 
			this.header1.AutoSize = true;
			this.header1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.header1.Dock = System.Windows.Forms.DockStyle.Top;
			this.header1.Location = new System.Drawing.Point(0, 0);
			this.header1.MinimumSize = new System.Drawing.Size(512, 24);
			this.header1.Name = "header1";
			this.header1.Size = new System.Drawing.Size(724, 24);
			this.header1.TabIndex = 3;
			// 
			// frmCombined
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(724, 545);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.header1);
			this.Name = "frmCombined";
			this.Text = "TeaseAI CE";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCombined_FormClosed);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private Media media1;
		internal Chat Chat;
		internal Glitter Glitter;
		private Header header1;
	}
}