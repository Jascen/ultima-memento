using Server.Engines.MobileEnhancement;
using Server.Items;
using Server.Misc;
using System;

namespace Server.Spells.Song
{
	public class ArmysPaeonSong : Song
	{
		private static SpellInfo m_Info = new SpellInfo(
			"Army's Paeon", "*plays an army's paeon*",
			-1
			);

		public override bool BlocksMovement { get { return true; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(5); } }
		public override double RequiredSkill { get { return 55.0; } }
		public override int RequiredMana { get { return 15; } }

		public ArmysPaeonSong(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
		{
		}

		public override void OnCast()
		{
			base.OnCast();

			bool sings = false;

			if (CheckSequence())
			{
				var rawHits = 5 + (MusicSkill(Caster) / 120);
				var tickAmount = MyServerSettings.PlayerLevelMod(rawHits, Caster);
				int rounds = (int)(Caster.Skills[SkillName.Musicianship].Value * .16);
				var tickInterval = TimeSpan.FromSeconds(2);
				var duration = TimeSpan.FromSeconds(tickInterval.TotalSeconds * rounds);

				foreach (var friend in GetNearbyFriends())
				{
					var recipient = new ArmysPaeonRecipient(friend, tickAmount, tickInterval, duration);
					Engine.Instance.AddEnhancement(friend, recipient);
				}

				sings = true;
			}

			BardFunctions.UseBardInstrument(BaseInstrument.GetInstrument(Caster), sings, Caster);
			FinishSequence();
		}

		private class ArmysPaeonRecipient : TimeDependentRecipient<ArmysPaeonSong>
		{
			private readonly int m_TickAmount;
			private Timer m_Timer;

			public ArmysPaeonRecipient(Mobile targetMobile, int tickAmount, TimeSpan tickInterval, TimeSpan duration) : base(targetMobile, duration)
			{
				m_TickAmount = tickAmount;
			}

			protected override void RemoveInternal()
			{
				if (m_Timer != null)
				{
					m_Timer.Stop();
					m_Timer = null;
				}

				var m = TargetMobile;
				BuffInfo.RemoveBuff(m, BuffIcon.ArmysPaeon);
				m.SendMessage("The effect of {0} wears off.", m_Info.Name);
			}

			protected override bool TryApplyInternal()
			{
				var m = TargetMobile;
				m.SendMessage("Your wounds begin to heal.");
				m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), () =>
				{
					if (m == null || m.Deleted || !m.Alive || DateTime.Now >= AppliedAt + Duration)
					{
						m_Timer.Stop();
						return;
					}

					m.Hits = Math.Min(m.Hits + m_TickAmount, m.HitsMax);
				});

				BuffInfo.RemoveBuff(m, BuffIcon.ArmysPaeon);
				BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.ArmysPaeon, 1063561, 1063560, Duration, m));

				m.FixedParticles(0x376A, 9, 32, 5030, 0x21, 3, EffectLayer.Waist);

				return true;
			}
		}
	}
}