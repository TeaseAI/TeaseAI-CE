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
	public partial class frmCombined : Form
	{
		public frmCombined()
		{
			InitializeComponent();
		}

		private void frmCombined_FormClosed(object sender, FormClosedEventArgs e)
		{
			Application.Exit();
		}
	}
}
