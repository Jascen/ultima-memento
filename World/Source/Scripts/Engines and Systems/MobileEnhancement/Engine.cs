using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.MobileEnhancement
{
	/// <summary>
	/// Central engine for mobile enhancements (e.g. bard songs). Holds a context per mobile,
	/// runs a periodic validation timer after world load, and provides get-or-create context access.
	/// </summary>
	public class Engine
	{
		private static Engine m_Engine;

		/// <summary>One context per mobile; holds active enhancements and their validation schedule.</summary>
		private readonly Dictionary<Mobile, MobileContext> m_Context = new Dictionary<Mobile, MobileContext>();

		private Timer s_ValidationTimer;

		public static Engine Instance
		{
			get
			{
				if (m_Engine == null)
					m_Engine = new Engine();

				return m_Engine;
			}
		}

		/// <summary>Register for world load so the validation timer is (re)started when the world loads.</summary>
		public static void Configure()
		{
			EventSink.WorldLoad += Instance.OnWorldLoad;
		}

		public void AddEnhancement(Mobile mobile, IEnhancement enhancement)
		{
			var context = GetOrCreateContext(mobile);
			context.AddEnhancement(enhancement);
		}

		/// <summary>Returns the context for the mobile if one exists; otherwise returns the shared default (no-op) context.</summary>
		public MobileContext GetContextOrDefault(Mobile mobile)
		{
			MobileContext context;
			return mobile != null && m_Context.TryGetValue(mobile, out context) ? context : MobileContext.Default;
		}

		/// <summary>Returns the context for the mobile, creating and storing one if it does not exist.</summary>
		public MobileContext GetOrCreateContext(Mobile mobile)
		{
			MobileContext context;
			if (m_Context.TryGetValue(mobile, out context)) return context;

			return m_Context[mobile] = new MobileContext();
		}

		/// <summary>Stops any existing validation timer and starts a new one (every 1 second) to validate all contexts.</summary>
		private void OnWorldLoad()
		{
			if (s_ValidationTimer != null)
			{
				s_ValidationTimer.Stop();
				s_ValidationTimer = null;
			}

			s_ValidationTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), () =>
			{
				foreach (var kv in m_Context.ToList())
				{
					if (kv.Key.Deleted)
					{
						m_Context.Remove(kv.Key);
						continue;
					}

					kv.Value.Validate();
				}
			});
			s_ValidationTimer.Start();
		}
	}
}