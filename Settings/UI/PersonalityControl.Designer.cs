namespace TeaseAI_CE.Settings.UI
{
	partial class PersonalityControl
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
			this.txtName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboEyeColor = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtID = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnSetId = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(64, 41);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(100, 20);
			this.txtName.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Name:";
			// 
			// comboEyeColor
			// 
			this.comboEyeColor.FormattingEnabled = true;
			this.comboEyeColor.Items.AddRange(new object[] {
			"Brown",
			"Blue",
			"Green",
			"Hazel",
			"Gray"});
			this.comboEyeColor.Location = new System.Drawing.Point(64, 65);
			this.comboEyeColor.Name = "comboEyeColor";
			this.comboEyeColor.Size = new System.Drawing.Size(100, 21);
			this.comboEyeColor.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Eye Color:";
			// 
			// txtID
			// 
			this.txtID.Location = new System.Drawing.Point(64, 14);
			this.txtID.Name = "txtID";
			this.txtID.Size = new System.Drawing.Size(100, 20);
			this.txtID.TabIndex = 0;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(21, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "ID:";
			// 
			// btnSetId
			// 
			this.btnSetId.Location = new System.Drawing.Point(170, 13);
			this.btnSetId.Name = "btnSetId";
			this.btnSetId.Size = new System.Drawing.Size(36, 20);
			this.btnSetId.TabIndex = 4;
			this.btnSetId.Text = "set";
			this.btnSetId.UseVisualStyleBackColor = true;
			this.btnSetId.Click += new System.EventHandler(this.btnSetId_Click);
			// 
			// PersonalityControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnSetId);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboEyeColor);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtID);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtName);
			this.Name = "PersonalityControl";
			this.Size = new System.Drawing.Size(500, 176);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboEyeColor;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtID;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnSetId;
	}
}
