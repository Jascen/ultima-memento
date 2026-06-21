using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.Elementalism
{
	public class Elemental_Blast_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Blast", "Deteriora",
				218,
				9002
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public Elemental_Blast_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
			m_Info.LeftHandEffect = m_Info.RightHandEffect = 9002;
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		private void AosDelay_Callback( object state )
		{
			object[] states = (object[])state;
			Mobile caster = (Mobile)states[0];
			Mobile target = (Mobile)states[1];
			Mobile defender = (Mobile)states[2];
			int damage = (int)states[3];
			bool applyEffect = (bool)states[4];

			string elm = ElementalSpell.GetElement( caster );
			Point3D loc = new Point3D( 0, 0, 0 );

			if ( elm == "air" )
			{
				loc = new Point3D( target.X+2, target.Y+2, target.Z+20 );
				Effects.SendLocationEffect( loc, target.Map, 0x5547, 30, 10, 0, 0 );
				target.PlaySound( 0x211 );
			}
			else if ( elm == "earth" )
			{
				loc = new Point3D( target.X+2, target.Y+2, target.Z+15 );
				Effects.SendLocationEffect( loc, target.Map, 0x3822, 30, 10, 0xAC0-1, 0 );
				target.PlaySound( 0x11C );
			}
			else if ( elm == "fire" )
			{
				loc = new Point3D( target.X+2, target.Y+2, target.Z+10 );
				Effects.SendLocationEffect( loc, target.Map, 0x36B0, 30, 10, 0, 0 );
				target.PlaySound( 0x11B );
			}
			else if ( elm == "water" )
			{
				loc = new Point3D( target.X+2, target.Y+2, target.Z+20 );
				Effects.SendLocationEffect( loc, target.Map, 0x5558, 30, 10, 0, 0 );
				target.PlaySound( 0x026 );
			}

			if ( applyEffect && caster.HarmfulCheck( defender ) )
			{
				if ( elm == "air" )
					SpellHelper.Damage( this, target, Utility.RandomMinMax( damage, damage + 4 ), 0, 0, 0, 0, 100 );
				else if ( elm == "earth" )
					SpellHelper.Damage( this, target, Utility.RandomMinMax( damage, damage + 4 ), 100, 0, 0, 0, 0 );
				else if ( elm == "fire" )
					SpellHelper.Damage( this, target, Utility.RandomMinMax( damage, damage + 4 ), 0, 100, 0, 0, 0 );
				else if ( elm == "water" )
				{
					AddWater( target );
					SpellHelper.Damage( this, target, Utility.RandomMinMax( damage, damage + 4 ), 0, 0, 100, 0, 0 );
				}
			}
		}

		public override bool DelayedDamage{ get{ return !Core.AOS; } }

		public void Target( Mobile m )
		{
			int nBenefit = 0;
			if ( Caster is PlayerMobile )
			{
				nBenefit = (int)(Caster.Skills[CastSkill].Value / 5);
			}

			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile from = Caster, target = m;

				SpellHelper.Turn( from, target );

				bool hitThrough = SpellHelper.ResolveMagicDefense( (int)this.OneBasedCircle, ref from, ref target );

				int damage = (int)((Caster.Skills[CastSkill].Value + Caster.Int) / 5);
			
				if ( damage > 60 )
					damage = 60;

				damage = damage + nBenefit;

				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ),
					new TimerStateCallback( AosDelay_Callback ),
					new object[]{ Caster, target, m, damage, hitThrough } );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private Elemental_Blast_Spell m_Owner;

			public InternalTarget( Elemental_Blast_Spell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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