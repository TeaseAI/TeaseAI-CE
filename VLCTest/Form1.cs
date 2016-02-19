using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vlc.DotNet.Core;
using Vlc.DotNet.Forms;

namespace VLCTest
{
    public partial class Form1 : Form
    {
        VlcControl vlcControl1;
        Random rng = new Random();
        
        public Form1()
        {
            InitializeComponent();

            this.vlcControl1 = new Vlc.DotNet.Forms.VlcControl();
            ((System.ComponentModel.ISupportInitialize)(this.vlcControl1)).BeginInit();
            // 
            // vlcControl1
            // 
            this.vlcControl1.BackColor = System.Drawing.Color.Black;
            this.vlcControl1.Location = new System.Drawing.Point(12, 112);
            this.vlcControl1.Name = "vlcControl1";
            this.vlcControl1.Size = new System.Drawing.Size(653, 344);
            this.vlcControl1.Spu = -1;
            this.vlcControl1.TabIndex = 1;
            this.vlcControl1.Text = "vlcControl1";
            ///####################################
            /// CRUCIAL PART: This needs to point to a local "install" of vlc player version 1.1.11 or lower
            ///####################################
            this.vlcControl1.VlcLibDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\vlc");
            this.vlcControl1.VlcMediaplayerOptions = null;

            this.Controls.Add(this.vlcControl1);

            ((System.ComponentModel.ISupportInitialize)(this.vlcControl1)).EndInit();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                vlcControl1.SetMedia(new FileInfo(ofd.FileName));
                vlcControl1.Play();
            }
        }
    }
}
