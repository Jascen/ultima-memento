using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Spells.Elementalism
{
	public class Elemental_Force_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Force", "Forta",
				203,
				9041
			);

		public override SpellCircle Circle { get { return SpellCircle.Third; } }

		public Elemental_Force_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

				bool hitThrough = SpellHelper.ResolveMagicDefense( (int)this.OneBasedCircle, ref source, ref m );

				string elm = ElementalSpell.GetElement( Caster );
				if ( elm == "air" )
				{
					source.MovingParticles( m, 0x3818, 7, 0, false, false, 0, 0, 0, 0, 0, 0 );
					source.PlaySound( 0x20A );
				}
				else if ( elm == "earth" )
				{
					source.MovingParticles( m, 0x1363, 7, 0, false, false, 0, 0, 0, 0, 0, 0 );
					source.PlaySound( 0x65A );
				}
				else if ( elm == "fire" )
				{
					source.MovingParticles( m, 0x36D4, 7, 0, false, false, 0, 0, 0, 0, 0, 0 );
					source.PlaySound( Core.AOS ? 0x15E : 0x44B );
				}
				else if ( elm == "water" )
				{
					source.MovingParticles( m, 0x5691, 7, 0, false, false, 0, 0, 0, 0, 0, 0 );
					source.PlaySound( 0x026 );
				}

				if ( hitThrough )
				{
					int nBenefit = (int)(Caster.Skills[CastSkill].Value / 5);
					double damage = GetNewAosDamage( 19, 1, 5, m ) + nBenefit;

					if ( elm == "air" )
						SpellHelper.Damage( this, m, damage, 25, 0, 0, 0, 75 );
					else if ( elm == "earth" )
						SpellHelper.Damage( this, m, damage, 100, 0, 0, 0, 0 );
					else if ( elm == "fire" )
						SpellHelper.Damage( this, m, damage, 25, 75, 0, 0, 0 );
					else if ( elm == "water" )
					{
						AddWater( m );
						SpellHelper.Damage( this, m, damage, 25, 0, 75, 0, 0 );
					}
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private Elemental_Force_Spell m_Owner;

			public InternalTarget( Elemental_Force_Spell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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