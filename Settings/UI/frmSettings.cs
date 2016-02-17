using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeaseAI_CE.Serialization;
using System.IO;

namespace TeaseAI_CE.Settings
{
    public partial class frmSettings : Form
    {
        AllSettings allSettings = new AllSettings();

        public frmSettings()
        {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            FileInfo settingsFile = new FileInfo("Settings.xml");
            if(settingsFile.Exists)
            {
            }
            else
            {
                Serializer.SerializeAsXML<AllSettings>(allSettings, settingsFile);
            }

            comboCupSize.SelectedIndex = (int)Domme.CupSize.C;
        }

        private void buttonSetDommeImageDir_Click(object sender, EventArgs e)
        {
            folderBrowserDommeDirectory.ShowDialog();
        }
    }
}
