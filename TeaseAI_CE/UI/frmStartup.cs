using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeaseAI_CE.Scripting;

namespace TeaseAI_CE.UI
{
	public partial class frmStartup : Form
	{
		private VM vm;

		public frmStartup()
		{
			InitializeComponent();
		}

		// ToDo : Threaded loading with progress.
		private void frmStartup_Shown(object sender, EventArgs e)
		{
			vm = new VM();
			vm.LoadScripts("scripts");

			var persona = vm.CreatePersonality();
			var controller = vm.CreateController(persona);
			controller.Interval = 2000;
			controller.Script = vm.GetScript("script.welcome");


			bool split = MessageBox.Show("Yes for dual window, no for single window", "", MessageBoxButtons.YesNo) == DialogResult.Yes;
			progressBar1.Value = progressBar1.Maximum;
			if (split)
			{
				var other = new frmSplitOther();
				var media = new frmSplitMedia();
				other.Show();
				media.Show();

				controller.OnOutput = other.Chat.Message;
			}
			else
			{
				var combined = new frmCombined();
				combined.Show();

				controller.OnOutput = combined.Chat.Message;
			}

			vm.Start();

			Hide();
		}

		private void frmStartup_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (vm != null)
				vm.Stop();
		}
	}
}
