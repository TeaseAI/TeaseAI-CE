namespace TeaseAI_CE.UI
{
	partial class Chat
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
			this.txtUser = new System.Windows.Forms.TextBox();
			this.chatHistory = new TeaseAI_CE.UI.WebBrowserForChat();
			this.SuspendLayout();
			// 
			// txtUser
			// 
			this.txtUser.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txtUser.Location = new System.Drawing.Point(0, 130);
			this.txtUser.Name = "txtUser";
			this.txtUser.Size = new System.Drawing.Size(572, 20);
			this.txtUser.TabIndex = 0;
			this.txtUser.TextChanged += new System.EventHandler(this.txtUser_TextChanged);
			this.txtUser.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUser_KeyDown);
			// 
			// chatHistory
			// 
			this.chatHistory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chatHistory.Location = new System.Drawing.Point(0, 0);
			this.chatHistory.Name = "chatHistory";
			this.chatHistory.Size = new System.Drawing.Size(572, 130);
			this.chatHistory.TabIndex = 1;
			this.chatHistory.Text = "webBrowserForChat1";
			// 
			// Chat
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.chatHistory);
			this.Controls.Add(this.txtUser);
			this.Name = "Chat";
			this.Size = new System.Drawing.Size(572, 150);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtUser;
		private WebBrowserForChat chatHistory;
	}
}
