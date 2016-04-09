using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyResources;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// Foundation for root level blocks, eg a script, or a list.
	/// </summary>
	public class BlockBase : Line, IKeyed
	{
		public enum Validation { NeverRan = 0, Running, Passed, Failed }
		public Validation Valid { get; private set; }

		public readonly Logger Log;
		public readonly GroupInfo Group;

		// ToDo : Question, How can we validate tags?
		//
		// ex script:
		// #(tag = "a")
		// #if(mood == happy)
		//     #(tag = "b)
		// #Image(tag)
		//
		// With something so simple, how would the validator know we needed both an image "a" and a image "b"?
		// We can't just check for image with both tags, because they could be sperate.
		// Currently it would just check for a image "b".
		// 
		// In validatation mode, we could add to the tag, then check if there is an image for every tag.
		// But that does not with when you use the AND operator. Because there may be a image for every tag, but there may not be any image for "a" AND "b".
		private HashSet<string> tags = null;

		public BlockBase(string key, Line[] lines, string[] tags, GroupInfo group, Logger log) : base(-1, key, lines)
		{
			Group = group;
			Log = log;
			Valid = Validation.NeverRan;

			if (tags != null && tags.Length > 0)
				this.tags = new HashSet<string>(tags);
		}

		/// <summary>
		/// Sets valid to running, or to passed/failed.
		/// </summary>
		/// <param name="running">If false, determin whether it passed.</param>
		public void SetValid(bool running)
		{
			if (running)
				Valid = Validation.Running;
			else
			{
				if (Log.ErrorCount == 0)
					Valid = Validation.Passed;
				else
					Valid = Validation.Failed;
			}
		}

		public bool HasTags { get { return tags != null && tags.Count > 0; } }
		public bool ContainsTag(string key)
		{
			if (tags == null)
				return false;
			return tags.Contains(key);
		}

		#region IKeyed
		public Variable Get(Key key, Logger log = null)
		{
			if (key.AtEnd)
				return new Variable<BlockBase>(this);
			Logger.LogF(log, Logger.Level.Error, StringsScripting.Formatted_Variable_not_found, key);
			return null;
		}
		#endregion
	}
}
