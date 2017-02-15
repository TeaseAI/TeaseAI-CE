using System;
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

		public Personality Get(int i)
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
				// ToDo : Error
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
		public void RemoveAt(int i)
		{
			try
			{
				locker.EnterWriteLock();
				list.RemoveAt(i);
			}
			finally
			{ locker.ExitWriteLock(); }
		}
		public void Remove(Personality p)
		{
			try
			{
				locker.EnterWriteLock();
				list.Remove(p);
			}
			finally
			{ locker.ExitWriteLock(); }
		}

		public void Actvate(int i)
		{
			try
			{
				locker.EnterWriteLock();
				if (i <= 0 || i >= list.Count)
					// ToDo : Error
					return;

				_swap(0, i);
			}
			finally
			{ locker.ExitWriteLock(); }
		}

		public void Actvate(Personality p)
		{
			if (p.Enabled == false)
				// ToDo : Error
				return;

			try
			{
				locker.EnterWriteLock();
				if (!list.Contains(p))
				{
					if (!ReferenceEquals(p.VM, VM))
					{
						// ToDo : Error
						Logger.Log(null, Logger.Level.Error, "Tried to add a personality with a different VM then the controller!");
						return;
					}
					list.Insert(0, p);
				}
				else
					_swap(0, list.IndexOf(p));
			}
			finally
			{ locker.ExitWriteLock(); }
		}
		private void _swap(int a, int b)
		{
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

		public void AddRandom()
		{
			try
			{
				locker.EnterWriteLock();

				var ps = VM.GetPersonalities();
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
				// ToDo: Error, unable to add a random personality!

			}
			finally { locker.ExitWriteLock(); }
		}

		public void AddRandomMultiple(Context sender, int count)
		{
			try
			{
				locker.EnterWriteLock();

				if (list.Count >= count)
					return;

				var ps = VM.GetPersonalities();
				var rnd = new Random();
				int maxTries = 100 * count;
				for (int i = 0; i < maxTries && list.Count < count; ++i)
				{
					var r = rnd.Next(0, ps.Length);
					if (!list.Contains(ps[r]))
						list.Add(ps[r]);
				}
				// ToDo: Error, unable to add a random personality!

			}
			finally { locker.ExitWriteLock(); }
		}
	}
}
