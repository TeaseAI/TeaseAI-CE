namespace TeaseAI_CE.UI
{
	partial class frmLoading
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLoading));
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.lblStatus = new System.Windows.Forms.Label();
			this.labelTemp = new System.Windows.Forms.Label();
			this.lblSubStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.BackColor = System.Drawing.Color.Black;
			resources.ApplyResources(this.progressBar, "progressBar");
			this.progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.progressBar.Maximum = 1000;
			this.progressBar.Name = "progressBar";
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			// 
			// lblStatus
			// 
			resources.ApplyResources(this.lblStatus, "lblStatus");
			this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.lblStatus.Name = "lblStatus";
			// 
			// labelTemp
			// 
			this.labelTemp.BackColor = System.Drawing.Color.Black;
			resources.ApplyResources(this.labelTemp, "labelTemp");
			this.labelTemp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.labelTemp.Name = "labelTemp";
			// 
			// lblSubStatus
			// 
			resources.ApplyResources(this.lblSubStatus, "lblSubStatus");
			this.lblSubStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.lblSubStatus.Name = "lblSubStatus";
			// 
			// frmLoading
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.Add(this.labelTemp);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.lblSubStatus);
			this.Controls.Add(this.progressBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "frmLoading";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLoading_FormClosing);
			this.Shown += new System.EventHandler(this.frmLoading_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Label labelTemp;
		private System.Windows.Forms.Label lblSubStatus;
	}
}