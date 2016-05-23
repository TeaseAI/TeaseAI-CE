using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting
{
	public class Key
	{
		private static readonly char[] keySeparator = { '.' };
		private static readonly string keySeparatorStr = ".";
		private string[] keys;
		private int index;
		public Key(Logger log, params string[] str)
		{
			keys = new string[str.Length];
			for (int i = 0; i < keys.Length; ++i)
				keys[i] = VM.KeyClean(str[i], log);
			index = 0;
		}
		public Key(string str, Logger log = null)
		{
			keys = VM.KeyClean(str, log).Split(keySeparator);
			index = 0;
			if (keys.Length > 0)
			{
				if (keys[0] == "")
					keys[0] = "self";
			}
		}
		public Key(string str, Context context)
		{
			str = VM.KeyClean(str, context.Root.Log);
			keys = str.Split(keySeparator);
			index = 0;

			if (keys.Length > 0)
			{
				if (keys[0] == "")
					keys[0] = "self";
				else if (keys[0] == "script" || keys[0] == "list")
				{
					if (keys[1] == "")
						keys[1] = context.Root.Group.Key;
				}
			}
		}

		public string Next()
		{
			if (index >= keys.Length)
				return "";
			return keys[index++];
		}
		public bool NextIf(string key)
		{
			if (Peek == key && index < keys.Length)
			{
				++index;
				return true;
			}
			return false;
		}

		public string Peek
		{
			get
			{
				if (index >= keys.Length)
					return "";
				return keys[index];
			}
		}

		public bool AtEnd
		{
			get { return index >= keys.Length; }
		}

		public void Reset()
		{
			index = 0;
		}

		public override string ToString()
		{
			return string.Join(keySeparatorStr, keys);
		}

		public static Key operator ++(Key key)
		{
			if (key.index <= keySeparator.Length)
				++key.index;
			return key;
		}
		public static Key operator --(Key key)
		{
			if (key.index > 0)
				--key.index;
			return key;
		}
	}
}
