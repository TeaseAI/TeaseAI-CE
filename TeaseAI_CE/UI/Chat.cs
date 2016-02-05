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
		public Chat()
		{
			InitializeComponent();
		}

		public delegate void MessageDelegate(string text);
		public void Message(string text)
		{
			if (InvokeRequired)
			{
				Invoke(new MessageDelegate(Message), text);
			}
			else
			{
				txtHistory.Text += Environment.NewLine + text;
			}
		}
	}
}
