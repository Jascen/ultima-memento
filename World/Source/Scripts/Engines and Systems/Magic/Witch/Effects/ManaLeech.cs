using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Undead
{
	public class ManaLeechSpell : UndeadSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "", "", 239, 9021 );
		public override double RequiredSkill{ get{ return 35.0; } }
		public override int RequiredMana{ get{ return 0; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }

		public ManaLeechSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( Caster.Poisoned ) { DoFizzle(); } // IF POISONED...NO GOOD
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				bool hitThrough = SpellHelper.ResolveMagicDefense( 7, ref source, ref m );

				m.PlaySound( 0x19C );
				m.FixedParticles( 0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot );

				if ( hitThrough )
				{
					if ( m.Spell != null )
						m.Spell.OnCasterHurt();

					m.Paralyzed = false;
					BuffInfo.CleanupIcons( m, true );

					int toDrain = 0;

						toDrain = (int)(GetDamageSkill( Caster ) - GetResistSkill( m )) + Server.Items.BasePotion.EnhancePotions( Caster );

						if ( !m.Player )
							toDrain /= 2;

						if ( toDrain < 0 )
							toDrain = 0;
						else if ( toDrain > m.Mana )
							toDrain = m.Mana;

					if ( toDrain > (Caster.ManaMax - Caster.Mana) )
						toDrain = Caster.ManaMax - Caster.Mana;

					m.Mana -= toDrain;
					Caster.Mana += toDrain + 25;
				}
			}

			FinishSequence();
		}

		public override double GetResistSkill( Mobile m )
		{
			int maxSkill = (1 + 7) * 10;
			maxSkill += (1 + (7 / 6)) * 25;

			if( m.Skills[SkillName.MagicResist].Value < maxSkill )
				m.CheckSkill( SkillName.MagicResist, 0.0, 120.0 );

			return m.Skills[SkillName.MagicResist].Value;
		}

		public virtual bool CheckResisted( Mobile target )
		{
			double n = GetResistPercent( target );

			n /= 100.0;

			if( n <= 0.0 )
				return false;

			if( n >= 1.0 )
				return true;

			int maxSkill = (1 + 7) * 10;
			maxSkill += (1 + (7 / 6)) * 25;

			if( target.Skills[SkillName.MagicResist].Value < maxSkill )
				target.CheckSkill( SkillName.MagicResist, 0.0, 120.0 );

			return (n >= Utility.RandomDouble());
		}

		public virtual double GetResistPercentForCircle( Mobile target )
		{
			double firstPercent = target.Skills[SkillName.MagicResist].Value / 5.0;
			double secondPercent = target.Skills[SkillName.MagicResist].Value - (((Caster.Skills[DamageSkill].Value - 20.0) / 5.0) + (1 + 7) * 5.0);

			return (firstPercent > secondPercent ? firstPercent : secondPercent) / 2.0; // Seems should be about half of what stratics says.
		}

		public virtual double GetResistPercent( Mobile target )
		{
			return GetResistPercentForCircle( target );
		}

		private class InternalTarget : Target
		{
			private ManaLeechSpell m_Owner;

			public InternalTarget( ManaLeechSpell owner ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}