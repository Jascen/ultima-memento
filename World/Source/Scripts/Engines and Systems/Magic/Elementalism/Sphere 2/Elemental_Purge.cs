using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Elementalism
{
	public class Elemental_Purge_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Purge", "Purifica",
				212,
				9061
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public Elemental_Purge_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Poison p = m.Poison;

				if ( p != null )
				{
					int chanceToCure = 10000 + (int)(Caster.Skills[CastSkill].Value * 75) - ((p.Level + 1) * (Core.AOS ? (p.Level < 4 ? 3300 : 3100) : 1750));
					chanceToCure /= 100;

					if ( chanceToCure > Utility.Random( 100 ) )
					{
						if ( m.CurePoison( Caster ) )
						{
							if ( Caster != m )
								Caster.SendLocalizedMessage( 1010058 ); // You have cured the target of all poisons!

							m.SendLocalizedMessage( 1010059 ); // You have been cured of all poisons.
						}
					}
					else
					{
						m.SendLocalizedMessage( 1010060 ); // You have failed to cure your target!
					}
				}

				string elm = ElementalSpell.GetElement( Caster );

				if ( elm == "air" )
				{
					m.FixedParticles( 0x5590, 10, 15, 5012, 0, 0, EffectLayer.Waist );
					m.PlaySound( 0x5C7 );
				}
				else if ( elm == "earth" )
				{
					m.FixedParticles( 0x54F4, 10, 15, 5012, 0, 0, EffectLayer.Waist );
					m.PlaySound( 0x657 );
				}
				else if ( elm == "fire" )
				{
					m.FixedParticles( 0x3F29, 10, 15, 5012, 0, 0, EffectLayer.Waist );
					m.PlaySound( 0x5CA );
				}
				else if ( elm == "water" )
				{
					m.FixedParticles( 0x1A84, 10, 15, 5012, 0x97F-1, 0, EffectLayer.Waist );
					m.PlaySound( 0x026 );
					AddWater( m );
				}
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private Elemental_Purge_Spell m_Owner;

			public InternalTarget( Elemental_Purge_Spell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Beneficial )
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