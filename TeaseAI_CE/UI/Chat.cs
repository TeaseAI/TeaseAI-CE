using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TeaseAI_CE.UI
{
	public partial class Chat : UserControl
	{
		public delegate void MessageDelegate(Scripting.Personality p, string text);
		public delegate void InputDelegate(string text);

		public InputDelegate OnInput;

		public Chat()
		{
			InitializeComponent();
		}

		public void Message(Scripting.Personality p, string text)
		{
			if (InvokeRequired)
			{
				Invoke(new MessageDelegate(Message), p, text);
			}
			else
			{
				chatHistory.Append(p.Name, text);
			}
		}

		private void txtUser_TextChanged(object sender, EventArgs e)
		{
		}

		private void txtUser_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				if (OnInput != null)
					OnInput(txtUser.Text);

				e.SuppressKeyPress = true;
				txtUser.Text = "";
			}
		}
	}
}
