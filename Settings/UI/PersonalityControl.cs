using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeaseAI_CE.Scripting;

namespace Settings.UI
{
	public partial class PersonalityControl : UserControl
	{
		Personality p;
		Logger log;

		public PersonalityControl()
		{
			InitializeComponent();

			AssignPersonality(null);
		}

		public void AssignPersonality(Personality p)
		{
			log = new Logger();
			this.p = p;

			attach(".name", txtName);
			attach(".eye", comboEyeColor);
		}

		#region attach helper methods
		// text box
		private void attach(string key, TextBox txt)
		{
			key = VM.KeyClean(key);
			txt.TextChanged -= txtChanged;
			txt.Text = "";
			if (p == null)
				return;

			var v = p.GetVariable(key, log);
			txt.Tag = v;
			if (v.IsSet)
				txt.Text = (string)v.Value;

			if (v.Readonly)
				txt.Tag = null;
			else
				txt.TextChanged += txtChanged;
		}
		private void txtChanged(object sender, EventArgs e)
		{
			var txt = sender as TextBox;
			if (txt != null && txt.Tag is Variable)
				((Variable)txt.Tag).Value = txt.Text;
		}
		// combo box
		private void attach(string key, ComboBox combo)
		{
			key = VM.KeyClean(key);
			combo.TextChanged -= comboChanged;
			if (combo.Items.Count > 0)
				combo.Text = combo.Items[0].ToString();
			else
				combo.Text = "";
			Enabled = p != null;
			if (p == null)
				return;


			var v = p.GetVariable(key, log);
			combo.Tag = v;
			if (v.IsSet)
				combo.Text = (string)v.Value;

			if (v.Readonly)
				combo.Tag = null;
			else
				combo.TextChanged += comboChanged;
		}
		private void comboChanged(object sender, EventArgs e)
		{
			var combo = sender as ComboBox;
			if (combo != null && combo.Tag is Variable)
				((Variable)combo.Tag).Value = combo.Text;
		}
		#endregion
	}
}
