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
			CoreFunctions.AddTo(vm);
			vm.LoadScripts("scripts"); // Load all scritps from scripts folder.
			vm.ValidateScripts();
			// ToDo : At some point we will want to run setups.

			// Create a personality for testing.
			var persona = vm.CreatePersonality("Lisa");
			persona.RunSetup();
			var controller = vm.CreateController(persona);
			controller.Interval = 500;
			// Note: this will not be common use:
			var script = persona.GetVariable("script.test.welcome", new Logger("script.test")) as Variable<Script>;
			if (script != null && script.IsSet)
				controller.Add(script.Value);

			// test logger, for now errors show up in the output window.
			var log = new Logger("test");
			persona.GetVariable(".mood", log);
			persona.GetVariable("script.fake", log);


			bool split = MessageBox.Show("Yes for dual window, no for single window", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
			progressBar1.Value = progressBar1.Maximum;

			// show the main windows and get the glitter and chat controls.
			Glitter glitter;
			Chat chat;
			if (split)
			{
				var other = new frmSplitOther();
				var media = new frmSplitMedia();
				other.Show();
				media.Show();
				chat = other.Chat;
				glitter = other.Glitter;
			}
			else
			{
				var combined = new frmCombined();
				combined.Show();
				chat = combined.Chat;
				glitter = combined.Glitter;
			}

			// assign the output of the controller to go to the chat control.
			controller.OnOutput = chat.Message;

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
