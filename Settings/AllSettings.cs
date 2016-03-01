using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TeaseAI_CE.Serialization;

namespace TeaseAI_CE.Settings
{
	[Serializable]
	public class AllSettings
	{
		private const string filename = "settings.xml";

		public Windows Windows { get; set; } = new Windows();
		public General General { get; set; } = new General();
		public Domme Domme { get; set; } = new Domme();
		public Sub Sub { get; set; } = new Sub();
		public Images Images { get; set; } = new Images();
		public Videos Videos { get; set; } = new Videos();
		public Ranges Ranges { get; set; } = new Ranges();

		public static AllSettings Load()
		{
			try
			{
				FileInfo file = new FileInfo(filename);
				if (file.Exists)
					return Serializer.DeserializeFromXML<AllSettings>(file);
				else
					return new AllSettings();
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Error loading setttings: " + ex.Message);
				return null;
			}
		}
		public bool Save()
		{
			try
			{
				FileInfo file = new FileInfo(filename);
				Serializer.SerializeAsXML(this, file);
				return true;
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Error saving setttings: " + ex.Message);
				return false;
			}
		}
	}
}
