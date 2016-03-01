using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeaseAI_CE.Scripting;

namespace TeaseAI_CE.Settings
{
	public partial class frmSettings : Form
	{
		AllSettings settings;
		VM vm;

		public frmSettings(AllSettings settings, VM vm)
		{
			this.settings = settings;
			this.vm = vm;
			InitializeComponent();
		}

		private void frmSettings_Load(object sender, EventArgs e)
		{
			flagShowTimestamps.Checked = settings.General.showTimestamps;


			tabPersonalities_load();

		}

		private void frmSettings_FormClosed(object sender, FormClosedEventArgs e)
		{
			settings.General.showTimestamps = flagShowTimestamps.Checked;
		}

		#region tab Personalities
		private void tabPersonalities_load()
		{
			var lst = PersonalitiesListBox;
			var ps = vm.GetPersonalities();

			lst.SuspendLayout();
			lst.Items.Clear();
			foreach (var p in ps)
				if (p != null)
					lst.Items.Add(p, p.EnabledUser);
			lst.ResumeLayout(true);
			if (lst.Items.Count > 0)
				lst.SelectedIndex = 0;
			else
				personalityControl.AssignPersonality(null);
		}

		private void PersonalitiesListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			var p = PersonalitiesListBox.SelectedItem as Personality;
			if (p == null)
				return;
			p.EnabledUser = e.NewValue == CheckState.Checked;
		}

		private void PersonalitiesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			personalityControl.AssignPersonality(PersonalitiesListBox.SelectedItem as Personality);
		}
		#endregion

		#region Tab Domme
		private void buttonSetDommeImageDir_Click(object sender, EventArgs e)
		{
			folderBrowserDommeDirectory.ShowDialog();
		}
		#endregion

	}
}
