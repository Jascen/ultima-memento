using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.Elementalism
{
	public class Elemental_Havoc_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Havoc", "Haotic",
				245,
				9042
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public Elemental_Havoc_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				bool hitThrough = SpellHelper.ResolveMagicDefense( (int)this.OneBasedCircle, ref source, ref m );

				string elm = ElementalSpell.GetElement( Caster );

				if ( elm == "air" )
				{
					Point3D blast6w = new Point3D( m.X+2, m.Y+2, m.Z+15 );
					Effects.SendLocationEffect( blast6w, m.Map, 0x5492, 30, 10, 0, 0 );
					m.PlaySound( 0x64F );
				}
				else if ( elm == "earth" )
				{
					Point3D blast6w = new Point3D( m.X+2, m.Y+2, m.Z+15 );
					Effects.SendLocationEffect( blast6w, m.Map, 0x554F, 30, 10, 0, 0 );
					blast6w = new Point3D( m.X, m.Y, m.Z+5 );
					Effects.SendLocationEffect( blast6w, m.Map, 0x554F, 30, 10, 0, 0 );
					m.PlaySound( 0x5CC );
				}
				else if ( elm == "fire" )
				{
					Point3D blast6w = new Point3D( m.X+1, m.Y, m.Z );
					Effects.SendLocationEffect( blast6w, m.Map, 0x3709, 30, 10, 0, 0 );
					blast6w = new Point3D( m.X, m.Y+1 , m.Z );
					Effects.SendLocationEffect( blast6w, m.Map, 0x3709, 30, 10, 0, 0 );
					blast6w = new Point3D( m.X+2, m.Y+2, m.Z+15 );
					Effects.SendLocationEffect( blast6w, m.Map, 0x3709, 30, 10, 0, 0 );
					m.PlaySound( 0x208 );
				}
				else if ( elm == "water" )
				{
					Point3D blast6w = new Point3D( m.X+2, m.Y+2, m.Z+12 );
					Effects.SendLocationEffect( blast6w, m.Map, 0x559A, 30, 10, 0, 0 );
					m.PlaySound( 0x64B );
				}

				if ( hitThrough )
				{
					int nBenefit = (int)(Caster.Skills[CastSkill].Value / 5);

					double damage = GetNewAosDamage( 48, 1, 5, m ) + nBenefit;

					if ( elm == "air" )
						SpellHelper.Damage( this, m, damage, 30, 0, 0, 0, 70 );
					else if ( elm == "earth" )
						SpellHelper.Damage( this, m, damage, 30, 0, 0, 70, 0 );
					else if ( elm == "fire" )
						SpellHelper.Damage( this, m, damage, 30, 70, 0, 0, 0 );
					else if ( elm == "water" )
					{
						AddWater( m );
						SpellHelper.Damage( this, m, damage, 30, 0, 70, 0, 0 );
					}
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private Elemental_Havoc_Spell m_Owner;

			public InternalTarget( Elemental_Havoc_Spell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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