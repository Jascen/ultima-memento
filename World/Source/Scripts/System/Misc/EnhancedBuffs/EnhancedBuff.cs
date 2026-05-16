using System;
using Server;

namespace Server
{
	public abstract class EnhancedBuff : BuffInfo
	{
		private Mobile m_Target;
		private DateTime m_AppliedTime;
		private int m_CustomTitleCliloc;
		private int m_CustomSecondaryCliloc;
		
		public Mobile Target { get { return m_Target; } }
		public DateTime AppliedTime { get { return m_AppliedTime; } }
		public new int TitleCliloc { get { return m_CustomTitleCliloc; } }
		public new int SecondaryCliloc { get { return m_CustomSecondaryCliloc; } }
		
		public EnhancedBuff( BuffIcon buffIcon, int titleCliloc, int secondaryCliloc, TimeSpan duration, Mobile target ) : base( buffIcon, titleCliloc, secondaryCliloc, duration, target )
		{
			m_Target = target;
			m_AppliedTime = DateTime.Now;
			m_CustomTitleCliloc = titleCliloc;
			m_CustomSecondaryCliloc = secondaryCliloc;
		}
		
		public TimeSpan RemainingTime
		{
			get
			{
				TimeSpan elapsed = DateTime.Now - m_AppliedTime;
				TimeSpan remaining = TimeLength - elapsed;
				return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
			}
		}
		
		public bool IsExpired
		{
			get { return RemainingTime <= TimeSpan.Zero; }
		}
	}
}