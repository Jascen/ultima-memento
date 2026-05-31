using Server;
using System;

namespace Harvest.Expedition
{
    public class HarvestExpedition
    {
        private int m_DurationHours;
        private DateTime m_ExpeditionStopTime;
        private StopTimer m_StopTimer;
        private HarvestExpeditionType m_ExpeditionType;

        public Mobile Owner { get; private set; }
        public bool IsComplete { get; private set; }
        public double PercentComplete { get; private set; }

        public void Start()
        {
            if (IsComplete) return;
            if (m_StopTimer != null) return;

            var now = DateTime.Now;

            if (m_ExpeditionStopTime < now)
            {
                Complete();
                return;
            }

            if (m_ExpeditionStopTime == DateTime.MinValue)
                m_ExpeditionStopTime = now.AddHours(m_DurationHours);

            m_StopTimer = new StopTimer(this, m_ExpeditionStopTime, TimeSpan.Zero);
        }

        public void Complete()
        {
            if (IsComplete) return;

            RemoveTimer();

            if (m_ExpeditionStopTime != DateTime.MinValue)
            {
                const int MINUTES_PER_HOUR = 60;
                var totalElapsedMinutes = Math.Max(0, (DateTime.Now - m_ExpeditionStopTime).TotalMinutes);
                PercentComplete = totalElapsedMinutes / m_DurationHours * MINUTES_PER_HOUR;
            }

            IsComplete = true;
        }

        private void RemoveTimer()
        {
            if (m_StopTimer == null) return;

            m_StopTimer.Stop();
            m_StopTimer = null;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0); //version
            writer.Write(Owner);
            writer.Write((int)m_ExpeditionType);
            writer.Write(m_DurationHours);
            writer.Write(m_ExpeditionStopTime);
            writer.Write(IsComplete);
            writer.Write(PercentComplete);
        }

        public HarvestExpedition(Mobile owner, HarvestExpeditionType expeditionType, int durationHours)
        {
            Owner = owner;
            m_ExpeditionType = expeditionType;
            m_DurationHours = durationHours;
        }

        public HarvestExpedition(GenericReader reader)
        {
            int version = reader.ReadInt();
            Owner = reader.ReadMobile();
            m_ExpeditionType = (HarvestExpeditionType)reader.ReadInt();
            m_DurationHours = reader.ReadInt();
            m_ExpeditionStopTime = reader.ReadDateTime();
            IsComplete = reader.ReadBool();
            PercentComplete = reader.ReadDouble();
        }

        private class StopTimer : Timer
        {
            public static readonly TimeSpan DefaultInterval = TimeSpan.FromHours(1);
            private readonly HarvestExpedition m_Expedition;
            private readonly DateTime m_StopTime;

            public StopTimer(HarvestExpedition expedition, DateTime stopTime, TimeSpan initialDelay) : base(initialDelay, TimeSpan.FromHours(1))
            {
                m_Expedition = expedition;
                m_StopTime = stopTime;
            }

            protected override void OnTick()
            {
                // 10% chance to fail early
                if (DateTime.Now < m_StopTime && 0.1 < Utility.RandomDouble()) return;

                m_Expedition.Complete();
            }
        }
    }
}