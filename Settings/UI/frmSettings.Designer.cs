namespace TeaseAI_CE.Settings
{
    partial class frmSettings
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
            if(disposing && (components != null))
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.flagAllowMediaDelete = new System.Windows.Forms.CheckBox();
            this.flagSaveChatlog = new System.Windows.Forms.CheckBox();
            this.flagAuditScripts = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.numericSlideshowTime = new System.Windows.Forms.NumericUpDown();
            this.radioSlideshowTease = new System.Windows.Forms.RadioButton();
            this.radioSlideshowTime = new System.Windows.Forms.RadioButton();
            this.radioSlideshowManual = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonSetDommeImageDir = new System.Windows.Forms.Button();
            this.textDommeImageDir = new System.Windows.Forms.TextBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flagTypeInstantly = new System.Windows.Forms.CheckBox();
            this.flagShowNames = new System.Windows.Forms.CheckBox();
            this.flagShowTimestamps = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textDescription = new System.Windows.Forms.Label();
            this.folderBrowserDommeDirectory = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.textAge = new System.Windows.Forms.Label();
            this.textHairColor = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboCupSize = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSlideshowTime)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Location = new System.Drawing.Point(-5, -3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(789, 247);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(781, 221);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.textBox1);
            this.groupBox6.Location = new System.Drawing.Point(446, 108);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(213, 50);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Safeword";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 14);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(200, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "red";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.flagAllowMediaDelete);
            this.groupBox5.Controls.Add(this.flagSaveChatlog);
            this.groupBox5.Controls.Add(this.flagAuditScripts);
            this.groupBox5.Location = new System.Drawing.Point(446, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(213, 96);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "System";
            // 
            // flagAllowMediaDelete
            // 
            this.flagAllowMediaDelete.AutoSize = true;
            this.flagAllowMediaDelete.Location = new System.Drawing.Point(6, 66);
            this.flagAllowMediaDelete.Name = "flagAllowMediaDelete";
            this.flagAllowMediaDelete.Size = new System.Drawing.Size(197, 17);
            this.flagAllowMediaDelete.TabIndex = 2;
            this.flagAllowMediaDelete.Text = "Allow Domme to Delete Local Media";
            this.flagAllowMediaDelete.UseVisualStyleBackColor = true;
            // 
            // flagSaveChatlog
            // 
            this.flagSaveChatlog.AutoSize = true;
            this.flagSaveChatlog.Checked = true;
            this.flagSaveChatlog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.flagSaveChatlog.Location = new System.Drawing.Point(6, 43);
            this.flagSaveChatlog.Name = "flagSaveChatlog";
            this.flagSaveChatlog.Size = new System.Drawing.Size(110, 17);
            this.flagSaveChatlog.TabIndex = 1;
            this.flagSaveChatlog.Text = "Autosave Chatlog";
            this.flagSaveChatlog.UseVisualStyleBackColor = true;
            // 
            // flagAuditScripts
            // 
            this.flagAuditScripts.AutoSize = true;
            this.flagAuditScripts.Checked = true;
            this.flagAuditScripts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.flagAuditScripts.Location = new System.Drawing.Point(7, 20);
            this.flagAuditScripts.Name = "flagAuditScripts";
            this.flagAuditScripts.Size = new System.Drawing.Size(137, 17);
            this.flagAuditScripts.TabIndex = 0;
            this.flagAuditScripts.Text = "Audit Scripts on Startup";
            this.flagAuditScripts.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.numericSlideshowTime);
            this.groupBox4.Controls.Add(this.radioSlideshowTease);
            this.groupBox4.Controls.Add(this.radioSlideshowTime);
            this.groupBox4.Controls.Add(this.radioSlideshowManual);
            this.groupBox4.Location = new System.Drawing.Point(227, 108);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(213, 50);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Slideshow Options";
            // 
            // numericSlideshowTime
            // 
            this.numericSlideshowTime.Location = new System.Drawing.Point(93, 20);
            this.numericSlideshowTime.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numericSlideshowTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericSlideshowTime.Name = "numericSlideshowTime";
            this.numericSlideshowTime.Size = new System.Drawing.Size(40, 20);
            this.numericSlideshowTime.TabIndex = 3;
            this.numericSlideshowTime.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // radioSlideshowTease
            // 
            this.radioSlideshowTease.AutoSize = true;
            this.radioSlideshowTease.Checked = true;
            this.radioSlideshowTease.Location = new System.Drawing.Point(139, 20);
            this.radioSlideshowTease.Name = "radioSlideshowTease";
            this.radioSlideshowTease.Size = new System.Drawing.Size(55, 17);
            this.radioSlideshowTease.TabIndex = 2;
            this.radioSlideshowTease.TabStop = true;
            this.radioSlideshowTease.Text = "Tease";
            this.radioSlideshowTease.UseVisualStyleBackColor = true;
            // 
            // radioSlideshowTime
            // 
            this.radioSlideshowTime.AutoSize = true;
            this.radioSlideshowTime.Location = new System.Drawing.Point(73, 22);
            this.radioSlideshowTime.Name = "radioSlideshowTime";
            this.radioSlideshowTime.Size = new System.Drawing.Size(14, 13);
            this.radioSlideshowTime.TabIndex = 1;
            this.radioSlideshowTime.UseVisualStyleBackColor = true;
            // 
            // radioSlideshowManual
            // 
            this.radioSlideshowManual.AutoSize = true;
            this.radioSlideshowManual.Location = new System.Drawing.Point(7, 20);
            this.radioSlideshowManual.Name = "radioSlideshowManual";
            this.radioSlideshowManual.Size = new System.Drawing.Size(60, 17);
            this.radioSlideshowManual.TabIndex = 0;
            this.radioSlideshowManual.Text = "Manual";
            this.radioSlideshowManual.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonSetDommeImageDir);
            this.groupBox3.Controls.Add(this.textDommeImageDir);
            this.groupBox3.Controls.Add(this.checkBox3);
            this.groupBox3.Location = new System.Drawing.Point(227, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(213, 96);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Images";
            // 
            // buttonSetDommeImageDir
            // 
            this.buttonSetDommeImageDir.Location = new System.Drawing.Point(7, 43);
            this.buttonSetDommeImageDir.Name = "buttonSetDommeImageDir";
            this.buttonSetDommeImageDir.Size = new System.Drawing.Size(200, 21);
            this.buttonSetDommeImageDir.TabIndex = 2;
            this.buttonSetDommeImageDir.Text = "Set Domme Images Directory";
            this.buttonSetDommeImageDir.UseVisualStyleBackColor = true;
            this.buttonSetDommeImageDir.Click += new System.EventHandler(this.buttonSetDommeImageDir_Click);
            // 
            // textDommeImageDir
            // 
            this.textDommeImageDir.Location = new System.Drawing.Point(7, 66);
            this.textDommeImageDir.Name = "textDommeImageDir";
            this.textDommeImageDir.ReadOnly = true;
            this.textDommeImageDir.Size = new System.Drawing.Size(200, 20);
            this.textDommeImageDir.TabIndex = 1;
            this.textDommeImageDir.Text = "No path selected";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(7, 20);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(202, 17);
            this.checkBox3.TabIndex = 0;
            this.checkBox3.Text = "Display Slideshow Pictures Randomly";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flagTypeInstantly);
            this.groupBox1.Controls.Add(this.flagShowNames);
            this.groupBox1.Controls.Add(this.flagShowTimestamps);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(213, 96);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Chat window";
            // 
            // flagTypeInstantly
            // 
            this.flagTypeInstantly.AutoSize = true;
            this.flagTypeInstantly.Location = new System.Drawing.Point(7, 66);
            this.flagTypeInstantly.Name = "flagTypeInstantly";
            this.flagTypeInstantly.Size = new System.Drawing.Size(136, 17);
            this.flagTypeInstantly.TabIndex = 2;
            this.flagTypeInstantly.Text = "Domme Types Instantly";
            this.flagTypeInstantly.UseVisualStyleBackColor = true;
            // 
            // flagShowNames
            // 
            this.flagShowNames.AutoSize = true;
            this.flagShowNames.Checked = true;
            this.flagShowNames.CheckState = System.Windows.Forms.CheckState.Checked;
            this.flagShowNames.Location = new System.Drawing.Point(7, 43);
            this.flagShowNames.Name = "flagShowNames";
            this.flagShowNames.Size = new System.Drawing.Size(125, 17);
            this.flagShowNames.TabIndex = 1;
            this.flagShowNames.Text = "Always Show Names";
            this.flagShowNames.UseVisualStyleBackColor = true;
            // 
            // flagShowTimestamps
            // 
            this.flagShowTimestamps.AutoSize = true;
            this.flagShowTimestamps.Checked = true;
            this.flagShowTimestamps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.flagShowTimestamps.Location = new System.Drawing.Point(7, 20);
            this.flagShowTimestamps.Name = "flagShowTimestamps";
            this.flagShowTimestamps.Size = new System.Drawing.Size(112, 17);
            this.flagShowTimestamps.TabIndex = 0;
            this.flagShowTimestamps.Text = "Show Timestamps";
            this.flagShowTimestamps.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(781, 221);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Domme";
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(781, 221);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Sub";
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(781, 221);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Scripts";
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(781, 221);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Images";
            // 
            // tabPage6
            // 
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(781, 221);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Video";
            // 
            // tabPage7
            // 
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(781, 221);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "Ranges";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textDescription);
            this.groupBox2.Location = new System.Drawing.Point(1, 241);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(780, 65);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Description";
            // 
            // textDescription
            // 
            this.textDescription.AutoSize = true;
            this.textDescription.Location = new System.Drawing.Point(222, 25);
            this.textDescription.Name = "textDescription";
            this.textDescription.Size = new System.Drawing.Size(319, 13);
            this.textDescription.TabIndex = 0;
            this.textDescription.Text = "Hover over any setting in the menu for a more detailed description.";
            this.textDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // folderBrowserDommeDirectory
            // 
            this.folderBrowserDommeDirectory.ShowNewFolderButton = false;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.comboCupSize);
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.Controls.Add(this.label4);
            this.groupBox7.Controls.Add(this.textBox3);
            this.groupBox7.Controls.Add(this.label3);
            this.groupBox7.Controls.Add(this.textBox2);
            this.groupBox7.Controls.Add(this.label2);
            this.groupBox7.Controls.Add(this.textHairColor);
            this.groupBox7.Controls.Add(this.textAge);
            this.groupBox7.Controls.Add(this.label1);
            this.groupBox7.Controls.Add(this.dateTimePicker1);
            this.groupBox7.Location = new System.Drawing.Point(6, 6);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(200, 209);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Appearance";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(6, 34);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(188, 20);
            this.dateTimePicker1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Birthday";
            // 
            // textAge
            // 
            this.textAge.AutoSize = true;
            this.textAge.Location = new System.Drawing.Point(109, 16);
            this.textAge.Name = "textAge";
            this.textAge.Size = new System.Drawing.Size(32, 13);
            this.textAge.TabIndex = 2;
            this.textAge.Text = "Age: ";
            // 
            // textHairColor
            // 
            this.textHairColor.Location = new System.Drawing.Point(68, 58);
            this.textHairColor.Name = "textHairColor";
            this.textHairColor.Size = new System.Drawing.Size(126, 20);
            this.textHairColor.TabIndex = 3;
            this.textHairColor.Text = "blonde";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Hair Color:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Hair Length:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(68, 82);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(126, 20);
            this.textBox2.TabIndex = 5;
            this.textBox2.Text = "long";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Eye Color:";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(68, 107);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(126, 20);
            this.textBox3.TabIndex = 7;
            this.textBox3.Text = "green";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Cup Size:";
            // 
            // comboCupSize
            // 
            this.comboCupSize.BackColor = System.Drawing.SystemColors.Window;
            this.comboCupSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCupSize.FormattingEnabled = true;
            this.comboCupSize.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "DD",
            "DDD",
            "DDD+"});
            this.comboCupSize.Location = new System.Drawing.Point(68, 132);
            this.comboCupSize.Name = "comboCupSize";
            this.comboCupSize.Size = new System.Drawing.Size(121, 21);
            this.comboCupSize.TabIndex = 11;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 309);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.ShowIcon = false;
            this.Text = "Tease AI CE Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSlideshowTime)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox flagShowTimestamps;
        private System.Windows.Forms.CheckBox flagTypeInstantly;
        private System.Windows.Forms.CheckBox flagShowNames;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label textDescription;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.TextBox textDommeImageDir;
        private System.Windows.Forms.Button buttonSetDommeImageDir;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioSlideshowTease;
        private System.Windows.Forms.RadioButton radioSlideshowTime;
        private System.Windows.Forms.RadioButton radioSlideshowManual;
        private System.Windows.Forms.NumericUpDown numericSlideshowTime;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox flagAllowMediaDelete;
        private System.Windows.Forms.CheckBox flagSaveChatlog;
        private System.Windows.Forms.CheckBox flagAuditScripts;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDommeDirectory;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label textAge;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textHairColor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ComboBox comboCupSize;
    }
}