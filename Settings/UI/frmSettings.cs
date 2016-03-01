using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TeaseAI_CE.Settings
{
	public partial class frmSettings : Form
	{
		AllSettings settings;

		public frmSettings(AllSettings settings)
		{
			this.settings = settings;
			InitializeComponent();
		}

		private void frmSettings_Load(object sender, EventArgs e)
		{
			flagShowTimestamps.Checked = settings.General.showTimestamps;
		}

		private void buttonSetDommeImageDir_Click(object sender, EventArgs e)
		{
			folderBrowserDommeDirectory.ShowDialog();
		}

		private void frmSettings_FormClosed(object sender, FormClosedEventArgs e)
		{
			settings.General.showTimestamps = flagShowTimestamps.Checked;
		}
	}
}
