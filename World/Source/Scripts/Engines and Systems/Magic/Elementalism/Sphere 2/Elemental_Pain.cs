using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.Elementalism
{
	public class Elemental_Pain_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Pain", "Durere",
				212,
				9041
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public Elemental_Pain_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
				bool hitThrough = SpellHelper.ResolveMagicDefense( (int)this.OneBasedCircle, ref source, ref m );

				string elm = ElementalSpell.GetElement( Caster );
				Point3D loc = new Point3D( m.X+2, m.Y+2, m.Z+20 );

				if ( elm == "air" )
				{
					loc = new Point3D( m.X+2, m.Y+2, m.Z+15 );
					Effects.SendLocationEffect( loc, m.Map, 0x2007, 30, 10, 0xB62-1, 0 );
					m.PlaySound( 0x5C6 );
				}
				else if ( elm == "earth" )
				{
					loc = new Point3D( m.X+2, m.Y+2, m.Z+25 );
					Effects.SendLocationEffect( loc, m.Map, 0x384E, 30, 10, 0xB26-1, 0 );
					m.PlaySound( 0x308 );
				}
				else if ( elm == "fire" )
				{
					Effects.SendLocationEffect( loc, m.Map, 0x5475, 30, 10, 0xB71-1, 0 );
					m.PlaySound( 0x658 );
				}
				else if ( elm == "water" )
				{
					Effects.SendLocationEffect( loc, m.Map, 0x1A84, 30, 10, 0xB78-1, 0 );
					m.PlaySound( 0x027 );
				}

				if ( hitThrough )
				{
					int nBenefit = (int)(Caster.Skills[CastSkill].Value / 5);
					
					double damage = GetNewAosDamage( 17, 1, 5, m ) + nBenefit;

					if ( !m.InRange( Caster, 2 ) )
						damage *= 0.25; // 1/4 damage at > 2 tile range
					else if ( !m.InRange( Caster, 1 ) )
						damage *= 0.50; // 1/2 damage at 2 tile range

					if ( elm == "air" )
						SpellHelper.Damage( this, m, damage, 50, 0, 0, 0, 50 );
					else if ( elm == "earth" )
						SpellHelper.Damage( this, m, damage, 100, 0, 0, 0, 0 );
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
			private Elemental_Pain_Spell m_Owner;

			public InternalTarget( Elemental_Pain_Spell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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