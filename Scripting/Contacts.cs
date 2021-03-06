﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MyResources;
using TeaseAI_CE.Scripting.Events;
using System.Threading;

namespace TeaseAI_CE.Scripting
{
	/// <summary>
	/// Thread-safe list of personalities.
	/// </summary>
	public class Contacts
	{
		public readonly VM VM;
		private ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
		private List<Personality> list = new List<Personality>();

		public Contacts(VM vm)
		{
			VM = vm;
		}

		public bool Contains(Personality p)
		{
			try
			{
				locker.EnterReadLock();
				return list.Contains(p);
			}
			finally
			{ locker.ExitReadLock(); }
		}

		public void Require(Context sender, int count)
		{
			sender.RequiredPersonalities = count;

			addRandomToCount(sender, count);
		}

		public Personality Get(int i, Logger log)
		{
			try
			{
				locker.EnterReadLock();
				if (i < 0 || i >= list.Count)
					// ToDo : Error
					return null;

				return list[i];
			}
			finally
			{ locker.ExitReadLock(); }
		}

		public Personality GetActive()
		{
			try
			{
				locker.EnterReadLock();
				if (list.Count == 0)
					return null;
				return list[0];
			}
			finally
			{ locker.ExitReadLock(); }
		}

		public void Add(Personality p)
		{
			if (p.Enabled == false)
				// ToDo : Error
				return;

			if (!ReferenceEquals(p.VM, VM))
			{
				Logger.Log(null, Logger.Level.Error, "Tried to add a personality with a different VM then the controller!");
				return;
			}

			try
			{
				locker.EnterWriteLock();
				if (!list.Contains(p))
					list.Add(p);
			}
			finally
			{ locker.ExitWriteLock(); }
		}
		public void RemoveAt(Context sender, int i)
		{
			try
			{
				locker.EnterWriteLock();
				if (i > sender.RequiredPersonalities - 1)
					sender.Root.Log.Warning("RequiredContacts not properly setup!");
				if (i < 0 || i >= list.Count)
					// ToDo : Error
					return;
				list.RemoveAt(i);
			}
			finally
			{ locker.ExitWriteLock(); }
		}
		public void Remove(Context sender, Personality p)
		{
			try
			{
				locker.EnterWriteLock();
				if (!list.Remove(p))
					sender.Root.Log.Warning("Tried to remove a personality that was not contained.");
			}
			finally
			{ locker.ExitWriteLock(); }
		}

		public void Actvate(Context sender, int i)
		{
			try
			{
				locker.EnterWriteLock();
				if (i > sender.RequiredPersonalities - 1)
					sender.Root.Log.Warning("RequiredContacts not properly setup!");
				if (i < 0 || i >= list.Count)
					// ToDo : Error
					return;

				_swap(0, i);
			}
			finally
			{ locker.ExitWriteLock(); }
		}

		public void Actvate(Context sender, Personality p)
		{
			if (p.Enabled == false)
				// ToDo : Error
				return;
			if (!ReferenceEquals(p.VM, VM))
			{
				Logger.Log(null, Logger.Level.Error, "Tried to add a personality with a different VM then the controller!");
				return;
			}

			try
			{
				locker.EnterWriteLock();
				if (!list.Contains(p))
					list.Insert(0, p);
				else
					_swap(0, list.IndexOf(p));
			}
			finally
			{ locker.ExitWriteLock(); }
		}

		public void ActvateRandom()
		{
			try
			{
				locker.EnterWriteLock();
				if (list.Count < 2)
					return;

				var rnd = new Random();
				int i = rnd.Next(1, list.Count);

				_swap(0, i);
			}
			finally
			{ locker.ExitWriteLock(); }
		}

		private void _swap(int a, int b)
		{
			if (a == b)
				return;
			var tmp = list[a];
			list[a] = list[b];
			list[b] = tmp;
		}
		public void Swap()
		{
			try
			{
				locker.EnterWriteLock();
				if (list.Count > 1)
					_swap(0, 1);
			}
			finally
			{ locker.ExitWriteLock(); }
		}

		// ToDo 6: Add functions should return the personality as a variable.
		public void AddRandom(Context sender)
		{
			try
			{
				locker.EnterWriteLock();

				var ps = VM.GetPersonalities(false);
				var rnd = new Random();
				int maxTries = 100;
				for (int i = 0; i < maxTries; ++i)
				{
					var r = rnd.Next(0, ps.Length);
					if (!list.Contains(ps[r]))
					{
						list.Add(ps[r]);
						return;
					}
				}
				sender.Root.Log.Error("Unable to add a random personality!");

			}
			finally { locker.ExitWriteLock(); }
		}

		private void addRandomToCount(Context sender, int count)
		{
			try
			{
				locker.EnterWriteLock();

				if (list.Count >= count)
					return;

				var ps = VM.GetPersonalities(false);
				var rnd = new Random();
				int maxTries = 100 * count;
				for (int i = 0; i < maxTries && list.Count < count; ++i)
				{
					var r = rnd.Next(0, ps.Length);
					if (!list.Contains(ps[r]))
						list.Add(ps[r]);
				}
				if (list.Count != count)
					sender.Root.Log.Error("Unable to add a random personality!");

			}
			finally { locker.ExitWriteLock(); }
		}
	}
}
