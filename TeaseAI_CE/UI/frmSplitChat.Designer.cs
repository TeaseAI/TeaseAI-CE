namespace TeaseAI_CE.UI
{
	partial class frmSplitChat
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
			this.Chat = new TeaseAI_CE.UI.Chat();
			this.SuspendLayout();
			// 
			// Chat
			// 
			this.Chat.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Chat.Location = new System.Drawing.Point(0, 0);
			this.Chat.Name = "Chat";
			this.Chat.Size = new System.Drawing.Size(753, 340);
			this.Chat.TabIndex = 0;
			// 
			// frmSplitChat
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(753, 340);
			this.Controls.Add(this.Chat);
			this.Name = "frmSplitChat";
			this.Text = "TeaseAI CE - Chat";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSplitChat_FormClosed);
			this.ResumeLayout(false);

		}

		#endregion

		internal Chat Chat;
	}
}