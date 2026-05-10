using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Elementalism
{
	public class Elemental_Fiend_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Fiend", "Diavol", 
				266,
				9040,
				false
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public Elemental_Fiend_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override TimeSpan GetCastDelay()
		{
			return TimeSpan.FromTicks( base.GetCastDelay().Ticks * 2 );
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if( (Caster.Followers + 2) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			Map map = Caster.Map;

			SpellHelper.GetSurfaceTop( ref p );

			int nBenefit = 0;
			if ( Caster is PlayerMobile )
			{
				nBenefit = (int)(Caster.Skills[CastSkill].Value / 2);
			}

			if ( map == null || !map.CanSpawnMobile( p.X, p.Y, p.Z ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				TimeSpan duration = TimeSpan.FromSeconds( 120 + nBenefit );

				string elm = ElementalSpell.GetElement( Caster );

				if ( elm == "air" )
				{
					BaseCreature.Summon( new ElementalFiendAir(), false, Caster, new Point3D( p ), 0x658, duration );
				}
				else if ( elm == "earth" )
				{
					BaseCreature.Summon( new ElementalFiendEarth(), false, Caster, new Point3D( p ), 0x162, duration );
				}
				else if ( elm == "fire" )
				{
					BaseCreature.Summon( new ElementalFiendFire(), false, Caster, new Point3D( p ), 0x208, duration );
				}
				else if ( elm == "water" )
				{
					BaseCreature.Summon( new ElementalFiendWater(), false, Caster, new Point3D( p ), 0x025, duration );
				}

				Caster.SendMessage( "You can double click the summoned to dispel them." );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private Elemental_Fiend_Spell m_Owner;

			public InternalTarget( Elemental_Fiend_Spell owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is IPoint3D )
					m_Owner.Target( (IPoint3D)o );
			}

			protected override void OnTargetOutOfLOS( Mobile from, object o )
			{
				from.SendLocalizedMessage( 501943 ); // Target cannot be seen. Try again.
				from.Target = new InternalTarget( m_Owner );
				from.Target.BeginTimeout( from, TimeoutTime - DateTime.Now );
				m_Owner = null;
			}

			protected override void OnTargetFinish( Mobile from )
			{
				if ( m_Owner != null )
					m_Owner.FinishSequence();
			}
		}
	}
}