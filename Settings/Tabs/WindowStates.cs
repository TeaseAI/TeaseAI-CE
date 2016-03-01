using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace TeaseAI_CE.Settings
{
	[Serializable]
	public class Windows
	{
		public bool Split = false;

		public Window Settings = new Window();

		public Window Combined = new Window();
		public Window SplitMedia = new Window();
		public Window SplitOther = new Window();
	}

	[Serializable]
	public class Window
	{
		public Size Size = Size.Empty;
		public Point Location;
		public bool BorderLess;
		public FormWindowState State;

		public void Attach(Form f)
		{
			Apply(f);
			f.FormClosing += (object sender, FormClosingEventArgs e) =>
			{
				Get(f);
			};
		}
		public virtual void Get(Form f)
		{
			State = f.WindowState;
			if (State == FormWindowState.Normal)
			{
				Size = f.Size;
				Location = f.Location;
			}
			BorderLess = f.FormBorderStyle == FormBorderStyle.None;
		}
		public virtual void Apply(Form f)
		{
			if (!Size.IsEmpty)
			{
				f.Size = Size;
				f.StartPosition = FormStartPosition.Manual;
				f.Location = Location;
			}
			f.WindowState = State;
			if (BorderLess)
				f.FormBorderStyle = FormBorderStyle.None;
		}
	}
}
