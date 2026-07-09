using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Timers
{
	public class RepeatableAction
	{
		private static readonly Dictionary<PlayerMobile, RepeatActionTimer> m_Timers = new Dictionary<PlayerMobile, RepeatActionTimer>();

		public static void Run(PlayerMobile m, Action action, Func<bool> predicate)
		{
			if (UnderEffect(m))
				StopTimer(m);

			var timer = m_Timers[m] = new RepeatActionTimer(m, action, predicate);
			timer.Start();
		}

		public static bool StopTimer(PlayerMobile m, Timer timer = null)
		{
			RepeatActionTimer t;
			if (!m_Timers.TryGetValue(m, out t)) return false;
			if (timer != null && t != timer) return false;

			t.Stop();
			m_Timers.Remove(m);

			return true;
		}

		public static bool UnderEffect(PlayerMobile m)
		{
			return m_Timers.ContainsKey(m);
		}

		private class RepeatActionTimer : Timer
		{
			private readonly Action m_Action;
			private readonly PlayerMobile m_Mobile;
			private readonly Func<bool> m_Predicate;

			public RepeatActionTimer(PlayerMobile m, Action action, Func<bool> predicate) : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
			{
				m_Action = action;
				m_Predicate = predicate;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				if (!m_Mobile.Alive || m_Mobile.Deleted || m_Mobile.NetState == null || !m_Predicate())
				{
					RepeatableAction.StopTimer(m_Mobile, this);
					return;
				}

				m_Action();
			}
		}
	}
}