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
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.lblStatus = new System.Windows.Forms.Label();
			this.labelTemp = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.BackColor = System.Drawing.Color.Black;
			this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.progressBar.Location = new System.Drawing.Point(0, 219);
			this.progressBar.Maximum = 1000;
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(624, 23);
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar.TabIndex = 2;
			// 
			// lblStatus
			// 
			this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.lblStatus.Location = new System.Drawing.Point(0, 196);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(624, 23);
			this.lblStatus.TabIndex = 4;
			this.lblStatus.Text = "Status: ";
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelTemp
			// 
			this.labelTemp.BackColor = System.Drawing.Color.Black;
			this.labelTemp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTemp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.labelTemp.Location = new System.Drawing.Point(0, 0);
			this.labelTemp.Name = "labelTemp";
			this.labelTemp.Size = new System.Drawing.Size(624, 196);
			this.labelTemp.TabIndex = 3;
			this.labelTemp.Text = "Loading...";
			this.labelTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// frmLoading
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(624, 242);
			this.Controls.Add(this.labelTemp);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.progressBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "frmLoading";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "TeaseAI CE - Loading...";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLoading_FormClosing);
			this.Shown += new System.EventHandler(this.frmLoading_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Label labelTemp;
	}
}