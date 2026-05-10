using Server.Engines.MobileEnhancement;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;
using System;

namespace Server.Spells.Song
{
	public class PoisonThrenodySong : Song
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Poison Threnody", "*plays a poison threnody*",
				-1
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(0.5); } }
		public override double RequiredSkill { get { return 70.0; } }
		public override int RequiredMana { get { return 25; } }

		public PoisonThrenodySong(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
		{
		}

		public virtual bool CheckSlayer(BaseInstrument instrument, Mobile defender)
		{
			SlayerEntry atkSlayer = SlayerGroup.GetEntryByName(instrument.Slayer);
			SlayerEntry atkSlayer2 = SlayerGroup.GetEntryByName(instrument.Slayer2);

			if (atkSlayer != null && atkSlayer.Slays(defender) || atkSlayer2 != null && atkSlayer2.Slays(defender))
				return true;

			return false;
		}

		public override void OnCast()
		{
			base.OnCast();

			Caster.Target = new InternalTarget(this);
		}

		public void Target(Mobile m)
		{
			bool sings = false;

			var instrument = BaseInstrument.GetInstrument(Caster);

			if (!Caster.CanSee(m))
			{
				Caster.SendLocalizedMessage(500237); // Target can not be seen.
			}
			else if (instrument == null || !Caster.InRange(instrument.GetWorldLocation(), 1))
			{
				Caster.SendMessage("Your instrument is missing! You can select another from your song book.");
			}
			else if (CheckHSequence(m))
			{
				sings = true;

				SpellHelper.Turn(Caster, m);

				m.FixedParticles(0x374A, 10, 30, 5013, 0x238, 2, EffectLayer.Waist);

				var musicSkill = MusicSkill(Caster);
				var durationSeconds = 0.24 * MusicSkill(Caster) + 30;
				int amount = musicSkill / 16;
				if (m is BaseCreature && CheckSlayer(instrument, m))
				{
					amount *= 2;
					durationSeconds *= 2;
				}

				var duration = TimeSpan.FromSeconds(Math.Min(120, durationSeconds));

				var recipient = new PoisonThrenodyRecipient(m, duration, amount);
				Engine.Instance.AddEnhancement(m, recipient);
			}

			BardFunctions.UseBardInstrument(instrument, sings, Caster);
			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private PoisonThrenodySong m_Owner;

			public InternalTarget(PoisonThrenodySong owner) : base(12, false, TargetFlags.Harmful)
			{
				m_Owner = owner;
			}

			protected override void OnTarget(Mobile from, object o)
			{
				if (o is Mobile)
					m_Owner.Target((Mobile)o);
			}

			protected override void OnTargetFinish(Mobile from)
			{
				m_Owner.FinishSequence();
			}
		}

		private class PoisonThrenodyRecipient : TimeDependentRecipient<PoisonThrenodySong>
		{
			private readonly int m_Amount;
			private ResistanceMod m_Mod;

			public PoisonThrenodyRecipient(Mobile targetMobile, TimeSpan duration, int amount) : base(targetMobile, duration)
			{
				m_Amount = amount;
			}

			protected override void RemoveInternal()
			{
				if (m_Mod == null) return;

				var m = TargetMobile;
				m.RemoveResistanceMod(m_Mod);
				m_Mod = null;

				BuffInfo.RemoveBuff(m, BuffIcon.PoisonThrenody);
				m.SendMessage("The effect of {0} wears off.", m_Info.Name);
			}

			protected override bool TryApplyInternal()
			{
				var m = TargetMobile;

				m.SendMessage("Your resistance to poison has decreased.");
				m_Mod = new ResistanceMod(ResistanceType.Poison, -m_Amount);
				m.AddResistanceMod(m_Mod);
				m.FixedParticles(0x374A, 10, 30, 5013, 0x238, 2, EffectLayer.Waist);

				string args = String.Format("{0}", m_Amount);
				BuffInfo.RemoveBuff(m, BuffIcon.PoisonThrenody);
				BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.PoisonThrenody, 1063583, 1063584, Duration, m, args, true));

				return true;
			}
		}
	}
}