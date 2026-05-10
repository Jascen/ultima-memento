using Server.Engines.MobileEnhancement;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using System;

namespace Server.Spells.Song
{
	public class EnergyCarolSong : Song
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Energy Carol", "*plays an energy carol*",
				-1
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(0.5); } }
		public override double RequiredSkill { get { return 50.0; } }
		public override int RequiredMana { get { return 12; } }

		public EnergyCarolSong(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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
					var recipient = new EnergyCarolRecipient(Caster, friend, duration);
					Engine.Instance.AddEnhancement(friend, recipient);
				}

				sings = true;
			}

			BardFunctions.UseBardInstrument(BaseInstrument.GetInstrument(Caster), sings, Caster);
			FinishSequence();
		}

		private class EnergyCarolRecipient : TimeDependentRecipient<EnergyCarolSong>
		{
			private readonly Mobile Caster;
			private ResistanceMod m_Mod;

			public EnergyCarolRecipient(Mobile caster, Mobile targetMobile, TimeSpan duration) : base(targetMobile, duration)
			{
				Caster = caster;
			}

			protected override void RemoveInternal()
			{
				if (m_Mod == null) return;

				var m = TargetMobile;
				m.RemoveResistanceMod(m_Mod);
				m_Mod = null;

				BuffInfo.RemoveBuff(m, BuffIcon.EnergyCarol);
				m.SendMessage("The effect of {0} wears off.", m_Info.Name);
			}

			protected override bool TryApplyInternal()
			{
				var m = TargetMobile;
				var amount = MyServerSettings.PlayerLevelMod(MusicSkill(Caster) / 16, Caster);

				// Clamp creature resistance bonus to player max
				if (m is BaseCreature)
				{
					if ((amount + m.EnergyResistance) > MySettings.S_MaxResistance)
					{
						amount = MySettings.S_MaxResistance - m.EnergyResistance;
						if (amount < 1) return false;
					}
				}

				m.SendMessage("Your resistance to energy has increased.");
				m_Mod = new ResistanceMod(ResistanceType.Energy, +amount);
				m.AddResistanceMod(m_Mod);
				m.FixedParticles(0x373A, 10, 15, 5012, 0x14, 3, EffectLayer.Waist);

				string args = String.Format("{0}", amount);
				BuffInfo.RemoveBuff(m, BuffIcon.EnergyCarol);
				BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.EnergyCarol, 1063565, 1063566, Duration, null, args));

				return true;
			}
		}
	}
}