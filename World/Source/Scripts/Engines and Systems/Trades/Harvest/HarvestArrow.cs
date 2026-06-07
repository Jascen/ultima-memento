using System;

namespace Server.Engines.Harvest
{
	public class HarvestArrow : QuestArrow
	{
		private readonly Timer m_Timer;

		public HarvestArrow(Mobile from, IPoint3D target) : base(from, target)
		{
			m_Timer = new HarvestArrowTimer(from, this);
			m_Timer.Start();
		}

		public override void OnClick(bool rightClick)
		{
			if (rightClick)
			{
				Stop();
			}
		}

		public override void OnStop()
		{
			m_Timer.Stop();
		}
	}

	public class HarvestArrowTimer : Timer
	{
		private readonly Mobile m_From;
		private readonly int m_LastX, m_LastY;
		private readonly HarvestArrow m_Arrow;

		public HarvestArrowTimer(Mobile from, HarvestArrow arrow) : base(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(1))
		{
			m_From = from;
			m_LastX = from.X;
			m_LastY = from.Y;
			m_Arrow = arrow;
		}

		protected override void OnTick()
		{
			if (!m_Arrow.Running)
			{
				Stop();
				return;
			}
			
			if (m_From.NetState == null 
				|| !m_From.Alive 
				|| m_From.Deleted
				|| m_LastX != m_From.X || m_LastY != m_From.Y)
			{
				m_Arrow.Stop();
				Stop();
				return;
			}
		}
	}
}