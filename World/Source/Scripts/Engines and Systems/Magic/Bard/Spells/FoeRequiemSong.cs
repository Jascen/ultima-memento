using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections;

namespace Server.Spells.Song
{
	public class FoeRequiemSong : Song
	{
		private static readonly Hashtable m_Table = new Hashtable();

		private static SpellInfo m_Info = new SpellInfo(
				"Foe Requiem", "*plays a foe requiem*",
				-1
			);

		public FoeRequiemSong(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
		{
		}

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(2); } }
		public override int RequiredMana { get { return 30; } }
		public override double RequiredSkill { get { return 80.0; } }
		public override bool UseDefaultInstrument { get { return false; } }

		public static bool HasEffect(Mobile m)
		{
			return m_Table[m] != null;
		}

		public static void RemoveEffect(Mobile m, Mobile messageRecipient = null)
		{
			Timer t = (Timer)m_Table[m];

			if (t != null)
			{
				t.Stop();
				m_Table.Remove(m);

				if (messageRecipient != null)
				{
					messageRecipient.SendMessage("The effect of {0} wears off.", m_Info.Name);
				}
			}
		}

		public override bool CheckCast()
		{
			if (!base.CheckCast())
				return false;

			if (GetEquippedInstrument() == null)
			{
				Caster.SendMessage("You must equip an instrument for this song!");
				return false;
			}

			return true;
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
			var instrument = GetEquippedInstrument();
			if (instrument == null)
			{
				Caster.SendMessage("You must equip an instrument for this song!");
				return;
			}

			if (m == null)
			{
				Caster.SendLocalizedMessage(500237); // Target can not be seen.
				FinishSequence();
			}
			else
			{
				bool sings = false;

				if (!Caster.CanSee(m))
				{
					Caster.SendLocalizedMessage(500237); // Target can not be seen.
					return;
				}
				else if (CheckHSequence(m))
				{
					// Each caster can only affect one target at a time.
					if (HasEffect(Caster))
					{
						RemoveEffect(Caster);
					}

					sings = true;

					SpellHelper.Turn(Caster, m);

					double damage = MusicSkill(Caster) / 10;
					if (m is BaseCreature && CheckSlayer(instrument, m)) damage *= 2;

					var i = 0;
					m_Table[Caster] = Timer.DelayCall(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), () =>
					{
						const int MAX_RANGE = 10;
						const int MAX_TICKS = 5;
						if (m == null || m.Deleted || !m.Alive || MAX_TICKS < ++i || !m.InRange(Caster, MAX_RANGE))
						{
							RemoveEffect(m, Caster);
							return;
						}

						var currentDamage = damage;
						if (!m.InRange(Caster, 3))
							currentDamage *= 0.50;
						else if (!m.InRange(Caster, 2))
							currentDamage *= 0.75;

						m.Damage((int)currentDamage, Caster);
						m.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Head);
					});

					Caster.MovingParticles(m, 0x379F, 7, 0, false, true, 3043, 4043, 0x211);
					m.PlaySound(0x1EA);
				}

				BardFunctions.UseBardInstrument(instrument, sings, Caster);
				FinishSequence();
			}
		}

		private BaseInstrument GetEquippedInstrument()
		{
			return Caster.Trinket as BaseInstrument;
		}

		private class InternalTarget : Target
		{
			private readonly FoeRequiemSong m_Owner;

			public InternalTarget(FoeRequiemSong owner) : base(12, false, TargetFlags.Harmful)
			{
				m_Owner = owner;
			}

			protected override void OnTarget(Mobile from, object o)
			{
				if (o is Mobile && o != null)
					m_Owner.Target((Mobile)o);
			}

			protected override void OnTargetFinish(Mobile from)
			{
				m_Owner.FinishSequence();
			}
		}
	}
}