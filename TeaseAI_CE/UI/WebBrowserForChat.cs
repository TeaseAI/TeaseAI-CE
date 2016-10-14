using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using TeaseAI_CE.Scripting;

namespace TeaseAI_CE.UI
{
	/// <summary>
	/// Some additions to allow the WebBrowser control to work better for chat history.
	/// </summary>
	public class WebBrowserForChat : Control
	{
		private TextBox textBox;
		//private WebBrowser browser;

		public WebBrowserForChat()
		{
			SuspendLayout();
			Dock = DockStyle.Fill;

			// ToDo : Finish browser functionality.
			// Need a consistant/clean way to add chat, show stuff like "__ is writing..", clear.
			// I am thinking loading base html/css/js from file, then using InvokeScript to push updates to the browser.
			//browser = new WebBrowser()
			//{
			//	Dock = DockStyle.Fill,
			//	AllowNavigation = false,
			//	AllowWebBrowserDrop = false,
			//	IsWebBrowserContextMenuEnabled = false,
			//	WebBrowserShortcutsEnabled = false,
			//};
			//Controls.Add(browser);

			// Remove:
			textBox = new TextBox()
			{
				Dock = DockStyle.Fill,
				Multiline = true,
				Text = "This history display is temporary.\n"
			};
			Controls.Add(textBox);


			ResumeLayout(false);
		}

		Personality lastPersona = null;
		public void Append(Personality p, string message)
		{
			message = System.Security.SecurityElement.Escape(message);

			textBox.AppendText("\n");
			//if (lastPersona != p)
			//{
			//	lastPersona = p;
			var name = System.Security.SecurityElement.Escape(p.Name);
			//	textBox.AppendText(name);
			//	textBox.AppendText(" Says:\n");
			//}
			//textBox.AppendText("    ");
			textBox.AppendText(name);
			textBox.AppendText(" Says: ");
			textBox.AppendText(message);

			textBox.Select(textBox.TextLength - 1, 0);
		}

		public void Clear()
		{
			textBox.Clear();
		}
	}
}
