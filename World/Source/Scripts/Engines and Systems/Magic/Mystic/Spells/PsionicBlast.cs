using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.Mystic
{
	public class PsionicBlast : MysticSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Psionic Blast", "Lum Om Summ Cah",
				269,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override int RequiredTithing{ get{ return 15; } }
		public override double RequiredSkill{ get{ return 30.0; } }
		public override int RequiredMana{ get{ return 35; } }
		public override int MysticSpellCircle{ get{ return 5; } }

		public PsionicBlast( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
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

			Point3D boom = new Point3D( target.X+1, target.Y+2, target.Z+5);
			Effects.SendLocationEffect( boom, target.Map, 0x3822, 60, 10, 0xB74, 0 );
			target.PlaySound( 0x658 );

			if ( applyEffect && caster.HarmfulCheck( defender ) )
				SpellHelper.Damage( this, target, Utility.RandomMinMax( damage, damage + 4 ), 0, 0, 0, 0, 100 );
		}

		public override bool DelayedDamage{ get{ return !Core.AOS; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile from = Caster, target = m;

				SpellHelper.Turn( from, target );

				bool hitThrough = SpellHelper.ResolveMagicDefense( 5, ref from, ref target );

				int damage = (int)((Caster.Skills[SkillName.FistFighting].Value + Caster.Int) / 4);
			
				if ( damage > 60 )
					damage = 60;

				Timer.DelayCall( TimeSpan.FromSeconds( 0.1 ),
					new TimerStateCallback( AosDelay_Callback ),
					new object[]{ Caster, target, m, damage, hitThrough } );
			}

			FinishSequence();
		}

		public override double GetSlayerDamageScalar( Mobile target )
		{
			return 1.0; //This spell isn't affected by slayer spellbooks
		}

		private class InternalTarget : Target
		{
			private PsionicBlast m_Owner;

			public InternalTarget( PsionicBlast owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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