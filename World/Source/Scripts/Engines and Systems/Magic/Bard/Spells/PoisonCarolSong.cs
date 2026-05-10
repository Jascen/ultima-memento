using System;
using Server.Mobiles;
using Server.Misc;
using Server.Engines.MobileEnhancement;
using Server.Items;

namespace Server.Spells.Song
{
	public class PoisonCarolSong : Song
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Poison Carol", "*plays a poison carol*",
				-1
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(0.5); } }
		public override double RequiredSkill { get { return 50.0; } }
		public override int RequiredMana { get { return 12; } }

		public PoisonCarolSong(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
		{
		}

		public override void OnCast()
		{
			base.OnCast();

			bool sings = false;

			if (CheckSequence())
			{
				var durationSeconds = 0.24 * MusicSkill(Caster) + 30;
				var duration = TimeSpan.FromSeconds(durationSeconds);

				foreach (var friend in GetNearbyFriends())
				{
					var recipient = new PoisonCarolRecipient(Caster, friend, duration);
					Engine.Instance.AddEnhancement(friend, recipient);
				}

				sings = true;
			}

			BardFunctions.UseBardInstrument(BaseInstrument.GetInstrument(Caster), sings, Caster);
			FinishSequence();
		}

		private class PoisonCarolRecipient : TimeDependentRecipient<PoisonCarolSong>
		{
			private readonly Mobile Caster;
			private ResistanceMod m_Mod;

			public PoisonCarolRecipient(Mobile caster, Mobile targetMobile, TimeSpan duration) : base(targetMobile, duration)
			{
				Caster = caster;
			}

			protected override void RemoveInternal()
			{
				if (m_Mod == null) return;

				var m = TargetMobile;
				m.RemoveResistanceMod(m_Mod);
				m_Mod = null;

				BuffInfo.RemoveBuff(m, BuffIcon.PoisonCarol);
				m.SendMessage("The effect of {0} wears off.", m_Info.Name);
			}

			protected override bool TryApplyInternal()
			{
				var m = TargetMobile;
				var amount = MyServerSettings.PlayerLevelMod(MusicSkill(Caster) / 16, Caster);

				// Clamp creature resistance bonus to player max
				if (m is BaseCreature)
				{
					if ((amount + m.PoisonResistance) > MySettings.S_MaxResistance)
					{
						amount = MySettings.S_MaxResistance - m.PoisonResistance;
						if (amount < 1) return false;
					}
				}

				m.SendMessage("Your resistance to poison has increased.");
				m_Mod = new ResistanceMod(ResistanceType.Poison, +amount);
				m.AddResistanceMod(m_Mod);
				m.FixedParticles(0x373A, 10, 15, 5012, 0x238, 3, EffectLayer.Waist);

				string args = String.Format("{0}", amount);
				BuffInfo.RemoveBuff(m, BuffIcon.PoisonCarol);
				BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.PoisonCarol, 1063581, 1063582, Duration, null, args));

				return true;
			}
		}
	}
}