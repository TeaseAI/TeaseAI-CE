using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using TeaseAI_CE.Scripting;
using MyResources;

namespace TeaseAI_CE.UI
{
	class MyApplicationContext : ApplicationContext
	{
		internal static MyApplicationContext Instance { get; private set; }

		private TraceToFileListener traceToFile;

		private List<Form> forms = new List<Form>();

		private Settings.AllSettings settings;
		private VM vm;

		internal bool Fail { get; private set; } = false;

		internal MyApplicationContext()
		{
			Instance = this;
			traceToFile = new TraceToFileListener();
			Trace.Listeners.Add(traceToFile);
			Application.ApplicationExit += onApplicationExit;

			var loading = new frmLoading(load);
			if (loading.ShowDialog() == DialogResult.Cancel)
			{
				ExitThread();
				Fail = true;
				return;
			}

			foreach (Form f in forms)
				f.Show();

			vm.Start();
		}

		internal void ToggleMainWindows()
		{
			PauseVM();

			settings.Windows.Split = !settings.Windows.Split;
			// ToDo 9: Implment control hot-swaping, so a restart is not required.
			MessageBox.Show("Please restart the applcation for changes to take effect.");

			ResumeVM();
		}

		internal void ShowSettings()
		{
			PauseVM();

			var frm = new Settings.frmSettings(settings, vm);
			settings.Windows.Settings.Attach(frm);
			frm.ShowDialog();

			ResumeVM();
		}

		private bool vmWasRunning;
		internal void PauseVM()
		{
			vmWasRunning = vm.IsRunning;
			if (vmWasRunning)
				vm.Stop();
		}
		internal void ResumeVM()
		{
			if (vmWasRunning)
				vm.Start();
		}

		private bool load(frmLoading.StatusDelegate status)
		{
			status(0, Strings.Status_Load_Settings);
			settings = Settings.AllSettings.Load();
			if (settings == null)
			{
				settings = new Settings.AllSettings();
				Thread.Sleep(4000);
			}

			status(10, Strings.Status_Creating_VM);
			vm = new VM();
			status(15, Strings.Status_Add_Functions);
			CoreFunctions.AddTo(vm);
			status(20, Strings.Status_Load_Scripts);
			vm.LoadFromDirectory("scripts"); // Load all scritps from scripts folder.
			status(40, Strings.Status_Load_Personalities);
			vm.LoadFromDirectory(settings.Personalities.Path);

			Personality player;
			if (!vm.TryGetPersonality("player", out player))
				player = vm.CreatePersonality("Player");
			// Create a personality for testing.
			Personality persona;
			if (!vm.TryGetPersonality("Lisa", out persona))
				persona = vm.CreatePersonality("Lisa");

			status(70, Strings.Status_Validate);
			vm.Validate();

			// ToDo : At some point we will want to run all setups.
			persona.RunSetup();

			var controller = vm.CreateController(persona);
			controller.Interval = 500;
			var fakelog = new Logger("fakelog");
			controller.Add(vm.QueryScript(persona.Get(new Key(".startquery", fakelog), fakelog), fakelog));


			status(90, Strings.Status_Load_UI);
			bool split = settings.Windows.Split;

			// show the main windows and get the glitter and chat controls.
			Glitter glitter;
			Chat chat;
			if (split)
			{
				var other = new frmSplitOther();
				var media = new frmSplitMedia();
				settings.Windows.SplitOther.Attach(other);
				settings.Windows.SplitMedia.Attach(media);
				addMainForm(other);
				addMainForm(media);

				chat = other.Chat;
				glitter = other.Glitter;
			}
			else
			{
				var combined = new frmCombined();
				settings.Windows.Combined.Attach(combined);
				addMainForm(combined);

				chat = combined.Chat;
				glitter = combined.Glitter;
			}

			// assign the output of the controller to go to the chat control.
			controller.OnOutput = chat.Message;
			// just dump all chat input to the controller.
			chat.OnInput = (string text) => { controller.Input(player, text); };

			status(100, Strings.Status_Display_UI);
			return true;
		}
		private void addMainForm(Form f)
		{
			forms.Add(f);
			f.FormClosed += formClosed;
		}

		private void formClosed(object sender, FormClosedEventArgs e)
		{
			if (vm != null)
				vm.Stop();

			Application.Exit();
		}

		private void onApplicationExit(object sender, EventArgs e)
		{
			// save personalities.
			SavePersonalities();

			// save settings
			if (settings != null)
				settings.Save();

			traceToFile.Dispose();
		}

		public void SavePersonalities()
		{
			string path = settings.Personalities.Path;
			try
			{
				path = Path.GetFullPath(path);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(path + ": " + ex.Message);
				settings.Personalities.Path = path = Path.GetFullPath("personalities");
			}

			Directory.CreateDirectory(path);
			var personalities = vm.GetPersonalities();

			// remove personality files that nolonger have a personality.
			var files = Directory.GetFiles(path, "*.vtscript", SearchOption.TopDirectoryOnly);
			foreach (var file in files)
			{
				var key = VM.KeyClean(Path.GetFileNameWithoutExtension(file));
				bool found = false;
				foreach (var p in personalities)
				{
					if (p.ID == key)
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					try
					{
						File.Delete(file);
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex.Message);
					}
				}
			}

			// save all personalities.
			foreach (var p in personalities)
			{
				using (var stream = new StreamWriter(Path.Combine(path, p.ID + ".vtscript")))
					stream.Write(p.WriteVariablesToString());
			}
		}
	}
}
