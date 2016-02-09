namespace TeaseAI_CE.UI
{
	partial class frmStartup
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
			this.labelTemp = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.lblStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelTemp
			// 
			this.labelTemp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTemp.Location = new System.Drawing.Point(0, 0);
			this.labelTemp.Name = "labelTemp";
			this.labelTemp.Size = new System.Drawing.Size(612, 252);
			this.labelTemp.TabIndex = 0;
			this.labelTemp.Text = "Logo and loading text here";
			this.labelTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// progressBar1
			// 
			this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.progressBar1.Location = new System.Drawing.Point(0, 229);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(612, 23);
			this.progressBar1.TabIndex = 1;
			// 
			// lblStatus
			// 
			this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblStatus.Location = new System.Drawing.Point(0, 206);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(612, 23);
			this.lblStatus.TabIndex = 2;
			this.lblStatus.Text = "Status: ";
			this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// frmStartup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(612, 252);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.labelTemp);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmStartup";
			this.Text = "TeaseAI CE - Startup";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmStartup_FormClosed);
			this.Shown += new System.EventHandler(this.frmStartup_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelTemp;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label lblStatus;
	}
}