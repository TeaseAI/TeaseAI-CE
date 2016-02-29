using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace TeaseAI_CE.UI
{
	public partial class frmLoading : Form
	{
		public delegate void StatusDelegate(float percent, string message);
		public delegate bool LoadDelegate(StatusDelegate status);

		private delegate void FinishDelegate(bool result);

		private LoadDelegate load;
		private volatile bool loadResult = false;
		private Thread thread;

		public frmLoading(LoadDelegate load)
		{
			InitializeComponent();
			this.load = load;

			try
			{ Trace.Listeners.Add(new traceToStatus(Status)); }
			catch
			{ }

			thread = new Thread(() =>
			{
				finish(load(Status));
			});
		}

		private void frmLoading_Shown(object sender, EventArgs e)
		{
			thread.Start();
		}

		private void frmLoading_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (loadResult)
				DialogResult = DialogResult.OK;
			else
				DialogResult = DialogResult.Cancel;
		}

		private void finish(bool result)
		{
			if (InvokeRequired)
			{
				BeginInvoke((FinishDelegate)finish, result);
				return;
			}

			Thread.Sleep(100);
			try
			{
				if (thread.IsAlive)
					thread.Abort();
				thread = null;
			}
			catch { }

			loadResult = result;
			Close();
		}

		public void Status(float percent, string message)
		{
			if (InvokeRequired)
			{
				BeginInvoke((StatusDelegate)Status, percent, message);
				return;
			}

			if (percent != -1)
				progressBar.Value = precentToValue(percent, progressBar.Minimum, progressBar.Maximum);

			if (message != null && message.Length > 0)
			{
				lblStatus.Text = message;
			}
		}
		private static int precentToValue(float value, int min, int max)
		{
			value *= max;
			if (value < min)
				return min;
			if (value > max)
				return max;
			return (int)value;
		}


		private class traceToStatus : TraceListener
		{
			StatusDelegate status;
			StringBuilder buffer = new StringBuilder();
			public traceToStatus(StatusDelegate status)
			{
				this.status = status;
			}
			public override void Write(string message)
			{
				buffer.Append(message);
			}
			public override void WriteLine(string message)
			{
				if (buffer.Length > 0)
				{
					status(-1, buffer.ToString() + message);
					buffer.Clear();
				}
				else
					status(-1, message);
			}
		}
	}
}
