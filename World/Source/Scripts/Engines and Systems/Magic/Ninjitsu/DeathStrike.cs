using System;
using System.Collections;
using Server.Items;
using Server.SkillHandlers;

namespace Server.Spells.Ninjitsu
{
	public class DeathStrike : NinjaMove
	{
		private const int TRIGGER_STEPS = 3;

		public DeathStrike()
		{
		}

		public override int BaseMana { get { return 30; } }
		public override double RequiredSkill { get { return 85.0; } }

		public override TextDefinition AbilityMessage { get { return new TextDefinition( 1063091 ); } } // You prepare to hit your opponent with a Death Strike.

		public override double GetDamageScalar( Mobile attacker, Mobile defender )
		{
			return 0.5;
		}

		public override void OnHit( Mobile attacker, Mobile defender, int damage )
		{
			if( !Validate( attacker ) || !CheckMana( attacker, true ) )
				return;

			ClearCurrentMove( attacker );

			double ninjitsu = attacker.Skills[SkillName.Ninjitsu].Value;

			double chance;
			bool isRanged = false; // should be defined onHit method, what if the player hit and remove the weapon before process? ;)

			if ( attacker.Weapon is BaseRanged )
				isRanged = true;

			if( ninjitsu < 100 ) //This formula is an approximation from OSI data.  TODO: find correct formula
				chance = 30 + (ninjitsu - 85) * 2.2;
			else
				chance = 63 + (ninjitsu - 100) * 1.1;

			CheckGain( attacker );

			if( (chance / 100) < Utility.RandomDouble() )
			{
				attacker.SendLocalizedMessage( 1070779 ); // You missed your opponent with a Death Strike.
				return;
			}

			DeathStrikeInfo info;
			if( m_Table.Contains( defender ) )
			{
				defender.SendLocalizedMessage( 1063092 ); // Your opponent lands another Death Strike!

				info = (DeathStrikeInfo)m_Table[defender];

				if( info.m_Timer != null )
					info.m_Timer.Stop();

				m_Table.Remove( defender );
			}
			else
			{
				defender.SendLocalizedMessage( 1063093 ); // You have been hit by a Death Strike!  Move with caution!
			}

			attacker.SendLocalizedMessage( 1063094 ); // You inflict a Death Strike upon your opponent!

			defender.FixedParticles( 0x374A, 1, 17, 0x26BC, EffectLayer.Waist );
			attacker.PlaySound( attacker.Female ? 0x50D : 0x50E );

			info = new DeathStrikeInfo( defender, attacker, isRanged );
			info.m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( ProcessDeathStrike ), defender );

			m_Table[defender] = info;
		}

		private static Hashtable m_Table = new Hashtable();

		private class DeathStrikeInfo
		{
			public Mobile m_Target;
			public Mobile m_Attacker;
			public int m_Steps;
			public int m_DamageBonus;			public Timer m_Timer;
			public bool m_isRanged;

			public DeathStrikeInfo( Mobile target, Mobile attacker, bool isRanged )
			{
				m_Target = target;
				m_Attacker = attacker;
				m_isRanged = isRanged;
			}
		}

		public static void AddStep( Mobile m )
		{
			DeathStrikeInfo info = m_Table[m] as DeathStrikeInfo;

			if( info == null )
				return;

			if( ++info.m_Steps >= TRIGGER_STEPS )
				ProcessDeathStrike( m );
		}

		private static void ProcessDeathStrike( object state )
		{
			Mobile defender = (Mobile)state;

			DeathStrikeInfo info = m_Table[defender] as DeathStrikeInfo;

			if( info == null )	//sanity
				return;

			int maxDamage, damage = 0;

			double ninjitsu = info.m_Attacker.Skills[SkillName.Ninjitsu].Value;
			double stalkingBonus = Tracking.GetStalkingBonus( info.m_Attacker, info.m_Target );

			double scalar = ( info.m_Attacker.Skills[SkillName.Hiding].Value + info.m_Attacker.Skills[SkillName.Stealth].Value ) / 220;
			if ( scalar > 1 )
				scalar = 1;

			if ( info.m_Steps >= TRIGGER_STEPS )
				damage = (int)Math.Floor( ( ninjitsu ) * ( 0.3 + 0.7 * scalar ) + stalkingBonus );
			else
				damage = (int)Math.Floor( ( ninjitsu / 3 ) * ( 0.3 + 0.7 * scalar ) + stalkingBonus );

			if ( info.m_isRanged )
				damage /= 2;

			// Mortally wound target
			MortalStrike.BeginWound( info.m_Target, MortalStrike.NPCDuration );

			info.m_Target.Damage( damage, info.m_Attacker ); // Damage is direct.

			// Paralyze the target
			WeaponStrikes.AchillesStrike( info.m_Target, TimeSpan.FromSeconds( Utility.RandomMinMax(2, 4 ) ) );

			if( info.m_Timer != null )
				info.m_Timer.Stop();

			m_Table.Remove( info.m_Target );
		}
	}
}