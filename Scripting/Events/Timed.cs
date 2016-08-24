using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaseAI_CE.Scripting.Events
{
	public class Timed : IEvent
	{
		DateTime end;
		TimeSpan time = TimeSpan.Zero;
		Action onFinished;
		bool firstTime = true;

		public Timed(DateTime end, Action onFinished = null)
		{
			this.end = end;
			this.onFinished = onFinished;
		}
		public Timed(TimeSpan time, bool waitToStart, Action onFinished = null)
		{
			if (waitToStart)
				this.time = time;
			else
				this.end = DateTime.Now + time;
			this.onFinished = onFinished;
		}

		public bool Finished()
		{
			if (firstTime)
			{
				firstTime = false;
				if (time != TimeSpan.Zero)
					end = DateTime.Now + time;
			}
			if (DateTime.Now < end)
				return false;
			onFinished?.Invoke();
			return true;
		}
	}
}
