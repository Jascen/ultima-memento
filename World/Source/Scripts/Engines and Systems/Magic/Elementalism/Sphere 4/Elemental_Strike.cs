using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.Elementalism
{
	public class Elemental_Strike_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Strike", "Lovitura",
				239,
				9021
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		public Elemental_Strike_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return false; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				if ( SpellHelper.ResolveMagicDefense( (int)this.OneBasedCircle, ref source, ref m ) )
				{
					int nBenefit = (int)(Caster.Skills[CastSkill].Value / 5);

					double damage = GetNewAosDamage( 23, 1, 4, m ) + nBenefit;

					string elm = ElementalSpell.GetElement( Caster );

					if ( elm == "air" )
					{
						Effects.SendLocationEffect( m.Location, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB04-1, 0 );
						SpellHelper.Damage( this, m, damage, 50, 0, 0, 0, 50 );
					}
					else if ( elm == "earth" )
					{
						Effects.SendLocationEffect( m.Location, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB5E-1, 0 );
						SpellHelper.Damage( this, m, damage, 100, 0, 0, 0, 0 );
					}
					else if ( elm == "fire" )
					{
						Effects.SendLocationEffect( m.Location, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB71-1, 0 );
						SpellHelper.Damage( this, m, damage, 50, 50, 0, 0, 0 );
					}
					else if ( elm == "water" )
					{
						Effects.SendLocationEffect( m.Location, m.Map, Utility.RandomList( 0x384E, 0x3859 ), 85, 10, 0, 0 );
						SpellHelper.Damage( this, m, damage, 50, 0, 50, 0, 0 );
					}

					Effects.PlaySound( m.Location, m.Map, 0x656 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private Elemental_Strike_Spell m_Owner;

			public InternalTarget( Elemental_Strike_Spell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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