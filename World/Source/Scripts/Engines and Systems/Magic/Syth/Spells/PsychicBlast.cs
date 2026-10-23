using System;
using Server.Targeting;

namespace Server.Spells.Syth
{
	public class PsychicBlast : SythSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 277,
					IconGraphic = 0x59E2,
					Name = "Psychic Blast",
					PowerWords = "Wai Kusk",
					Description = "Summon your rage to perform a mental attack that deals an amount of energy damage based upon your power as a Syth. Elemental Resistances may reduce damage done by this attack. The damage dealt can be between 80-125 points.",
					ManaCost = 32,
					TithingCost = 48,
					MinSkill = 80,
					TargetType = TargetFlags.Harmful
				},
				-1,
				0
			);

		public int CirclePower = 6;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public PsychicBlast( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
			int min = (int)states[3];
			int max = (int)states[4];

			if ( caster.HarmfulCheck( defender ) )
			{
				AOS.Damage( target, caster, Utility.RandomMinMax( min, max ), 0, 0, 0, 0, 100 );

				Point3D boom = new Point3D( target.X+1, target.Y+2, target.Z+5);
				Effects.SendLocationEffect( boom, target.Map, 0x3822, 60, 10, 0xAF1, 0 );
				target.PlaySound( 0x658 );
			}
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

				int min = 26;
				int max = (int)( GetSythDamage( Caster ) / 3 );

				if ( max > 125 )
					max = 125;

				Timer.DelayCall( TimeSpan.FromSeconds( 0.1 ),
					new TimerStateCallback( AosDelay_Callback ),
					new object[]{ Caster, target, m, min, max } );
			}
		}

		public override double GetSlayerDamageScalar( Mobile target )
		{
			return 1.0; //This spell isn't affected by slayer spellbooks
		}

		private class InternalTarget : Target
		{
			private PsychicBlast m_Owner;

			public InternalTarget( PsychicBlast owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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