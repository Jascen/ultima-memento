using System;
using Server.Targeting;
using Server.Network;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Spells.Undead
{
	public class RetchedAirSpell : UndeadSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "", "", 239, 9021 );
		public override double RequiredSkill{ get{ return 30.0; } }
		public override int RequiredMana{ get{ return 0; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }

		
		public RetchedAirSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile source = Caster;

				SpellHelper.Turn( source, m );

				if ( SpellHelper.ResolveMagicDefense( 3, ref source, ref m ) )
				{
					double damage;

					damage = GetNewAosDamage( 19, 1, 5, m );

					m.FixedParticles( 0x376A, 9, 10, 5029, 38, 0, EffectLayer.LeftFoot );
					m.PlaySound( 0x5C6 );

					SpellHelper.Damage( this, m, damage, 0, 100, 0, 0, 0 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private RetchedAirSpell m_Owner;

			public InternalTarget( RetchedAirSpell owner ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}