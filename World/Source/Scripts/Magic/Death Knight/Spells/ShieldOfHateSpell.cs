using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.DeathKnight
{
	public class ShieldOfHateSpell : DeathKnightSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Shield of Hate", "Bael Odi",
				236,
				9011
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1 ); } }
		public override int RequiredTithing{ get{ return 77; } }
		public override double RequiredSkill{ get{ return 60.0; } }
		public override int RequiredMana{ get{ return 48; } }

		public ShieldOfHateSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

			if ( m_Table.Contains( m ) )
			{
				Caster.LocalOverheadMessage( MessageType.Regular, 0x481, false, "That target is under the effect of that spell already." );
			}

			if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				ExpireTimer timer = (ExpireTimer)m_Table[m];

				if ( timer != null )
					timer.DoExpire();
				else
					m.SendMessage( "You feel hatred shielding you from physical harm." );
            		m.PlaySound( 0x5C0 );
					m.FixedParticles( 0x376A, 1, 29, 9961, 1152, 0, EffectLayer.Waist ); 

				TimeSpan duration = TimeSpan.FromSeconds( GetKarmaPower( Caster ) );

				ResistanceMod[] mods = new ResistanceMod[4]
					{
						new ResistanceMod( ResistanceType.Fire, +0 ),
						new ResistanceMod( ResistanceType.Poison, +0 ),
						new ResistanceMod( ResistanceType.Cold, +0 ),
						new ResistanceMod( ResistanceType.Physical, +100 )
					};

				timer = new ExpireTimer( m, mods, duration );
				timer.Start();

				BuffInfo.RemoveBuff( m, BuffIcon.ShieldOfHate );
				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.ShieldOfHate, 1063553, duration, m ) );

				DrainSoulsInLantern( Caster, RequiredTithing );

				m_Table[m] = timer;

				for ( int i = 0; i < mods.Length; ++i )
					m.AddResistanceMod( mods[i] );
			}

			FinishSequence();
		}

		private static Hashtable m_Table = new Hashtable();

		public static bool RemoveCurse( Mobile m )
		{
			ExpireTimer t = (ExpireTimer)m_Table[m];

			if ( t == null )
				return false;

			m.SendMessage( "The shield around you dissipates..." );
			m.PlaySound( 488 );
			t.DoExpire();
			return true;
		}

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		private class ExpireTimer : Timer
		{
			private Mobile m_Mobile;
			private ResistanceMod[] m_Mods;

			public ExpireTimer( Mobile m, ResistanceMod[] mods, TimeSpan delay ) : base( delay )
			{
				m_Mobile = m;
				m_Mods = mods;
			}

			public void DoExpire()
			{
				for ( int i = 0; i < m_Mods.Length; ++i )
					m_Mobile.RemoveResistanceMod( m_Mods[i] );

				Stop();
				BuffInfo.RemoveBuff( m_Mobile, BuffIcon.ShieldOfHate );
				m_Table.Remove( m_Mobile );
			}

			protected override void OnTick()
			{
				m_Mobile.SendMessage( "The shield around you dissipates..." );
				m_Mobile.PlaySound( 488 );
				DoExpire();
			}
		}

		private class InternalTarget : Target
		{
			private ShieldOfHateSpell m_Owner;

			public InternalTarget( ShieldOfHateSpell owner ) : base( 12, false, TargetFlags.Beneficial )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile) o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}
