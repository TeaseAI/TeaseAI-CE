using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TeaseAI_CE.UI
{
	public partial class frmSplitMedia : Form
	{
		public frmSplitMedia()
		{
			InitializeComponent();
		}

		private void frmSplitMedia_FormClosed(object sender, FormClosedEventArgs e)
		{
			Application.Exit();
		}
	}
}
