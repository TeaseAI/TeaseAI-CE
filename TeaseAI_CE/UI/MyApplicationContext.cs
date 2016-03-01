using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using TeaseAI_CE.Scripting;

namespace TeaseAI_CE.UI
{
	class MyApplicationContext : ApplicationContext
	{
		internal static MyApplicationContext Instance { get; private set; }

		private List<Form> forms = new List<Form>();

		private Settings.AllSettings settings;
		private VM vm;

		internal MyApplicationContext()
		{
			Instance = this;
			Application.ApplicationExit += onApplicationExit;

			var loading = new frmLoading(load);
			if (loading.ShowDialog() == DialogResult.Cancel)
				ExitThread();

			foreach (Form f in forms)
				f.Show();

			vm.Start();
		}

		internal void ShowSettings()
		{
			bool running = vm.IsRunning;
			if (running)
				vm.Stop();

			var frm = new Settings.frmSettings(settings);
			frm.ShowDialog();

			if (running)
				vm.Start();
		}

		private bool load(frmLoading.StatusDelegate status)
		{
			status(0, "Loading settings");
			settings = Settings.AllSettings.Load();
			if (settings == null)
				return false;

			status(10, "Creating scripting VM");
			vm = new VM();
			status(15, "Adding functions");
			CoreFunctions.AddTo(vm);
			status(20, "Loading scripts");
			vm.LoadScripts("scripts"); // Load all scritps from scripts folder.
			status(50, "Validating scripts");
			vm.ValidateScripts();
			// ToDo : At some point we will want to run setups.

			status(70, "Creating personalities");
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

			status(90, "Loading UI");
			bool split = false; //MessageBox.Show("Yes for dual window, no for single window", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button2) == DialogResult.Yes;

			// show the main windows and get the glitter and chat controls.
			Glitter glitter;
			Chat chat;
			if (split)
			{
				var other = new frmSplitOther();
				var media = new frmSplitMedia();
				forms.Add(other);
				forms.Add(media);
				other.FormClosed += formClosed;
				media.FormClosed += formClosed;

				chat = other.Chat;
				glitter = other.Glitter;
			}
			else
			{
				var combined = new frmCombined();
				combined.FormClosed += formClosed;

				forms.Add(combined);
				chat = combined.Chat;
				glitter = combined.Glitter;
			}

			// assign the output of the controller to go to the chat control.
			controller.OnOutput = chat.Message;

			status(100, "Displaying UI");
			return true;
		}

		private void formClosed(object sender, FormClosedEventArgs e)
		{
			if (vm != null)
				vm.Stop();

			Application.Exit();
		}

		private void onApplicationExit(object sender, EventArgs e)
		{
			if (settings != null)
				settings.Save();
		}
	}
}
