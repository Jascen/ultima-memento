using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.Elementalism
{
	public class Elemental_Barrage_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Barrage", "Baraj",
				230,
				9022
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public Elemental_Barrage_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

				SpellHelper.Turn( Caster, m );

				bool hitThrough = SpellHelper.ResolveMagicDefense( (int)this.OneBasedCircle, ref source, ref m );

				string elm = ElementalSpell.GetElement( Caster );

				if ( elm == "air" )
				{
					source.MovingParticles( m, 0x379F, 7, 0, false, false, 0, 0, 0, 0, 0, 0 );
					source.PlaySound( 0x20A );
				}
				else if ( elm == "earth" )
				{
					source.MovingParticles( m, 0x46E6, 7, 0, false, false, 0, 0, 0, 0, 0, 0 );
					source.PlaySound( 0x34F );
				}
				else if ( elm == "fire" )
				{
					source.MovingParticles( m, 0x36D4, 7, 0, false, false, 0, 0, 0, 0, 0, 0 );
					source.PlaySound( 0x349 );
				}
				else if ( elm == "water" )
				{
					source.MovingParticles( m, 0x46E9, 7, 0, false, false, 0, 0, 0, 0, 0, 0 );
					source.PlaySound( 0x364 );
				}

				if ( hitThrough )
				{
					int nBenefit = (int)(source.Skills[CastSkill].Value / 5);

					double damage = GetNewAosDamage( 40, 1, 5, m ) + nBenefit;

					if ( elm == "air" )
						SpellHelper.Damage( this, m, damage, 0, 0, 0, 0, 100 );
					else if ( elm == "earth" )
						SpellHelper.Damage( this, m, damage, 0, 0, 0, 100, 0 );
					else if ( elm == "fire" )
						SpellHelper.Damage( this, m, damage, 0, 100, 0, 0, 0 );
					else if ( elm == "water" )
					{
						AddWater( m );
						SpellHelper.Damage( this, m, damage, 0, 0, 100, 0, 0 );
					}
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private Elemental_Barrage_Spell m_Owner;

			public InternalTarget( Elemental_Barrage_Spell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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