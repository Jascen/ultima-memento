using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Spells.DeathKnight
{
	public class HellfireSpell : DeathKnightSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Demonic Hellfire", "Flam Infernum",
				242,
				9012
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1 ); } }
		public override int RequiredTithing{ get{ return 84; } }
		public override double RequiredSkill{ get{ return 70.0; } }
		public override int RequiredMana{ get{ return 52; } }

		public HellfireSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
			else if ( CheckHSequence( m ) )
			{
				m.FixedParticles( 0x3709, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
				m.PlaySound( 0x208 );
				m.SendMessage( "You feel your body being scorched by demonic hellfire!" );

				SpellHelper.Turn( Caster, m );

				double damage = GetKarmaPower( Caster ) / 5;

				SpellHelper.Damage( TimeSpan.Zero, m, Caster, damage, 0, 100, 0, 0, 0 );

				BeginBurn( m, Caster );
				DrainSoulsInLantern( Caster, RequiredTithing );
			}

			FinishSequence();
		}

		private static Hashtable m_Table = new Hashtable();

		public static bool IsBurning( Mobile m )
		{
			return m_Table.Contains( m );
		}

		public static void BeginBurn( Mobile m, Mobile from )
		{
			Timer t = (Timer)m_Table[m];

			if ( t != null )
				t.Stop();

			t = new InternalTimer( from, m );
			m_Table[m] = t;

			t.Start();

			m.YellowHealthbar = true;
		}

		public static void DoBurn( Mobile m, Mobile from, int level )
		{
			if ( m.Alive )
			{
				int damage = Utility.RandomMinMax( level, level * 2 );

				if ( !m.Player )
					damage *= 2;

				m.FixedParticles( 0x3709, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
				m.PlaySound( 0x208 );
				m.Damage( damage, m );
			}
			else
			{
				EndBurn( m, false );
			}
		}

		public static void EndBurn( Mobile m, bool message )
		{
			Timer t = (Timer)m_Table[m];

			if ( t == null )
				return;

			t.Stop();
			m_Table.Remove( m );
			m.YellowHealthbar = false;
			m.SendMessage( "The flames die out.." );
		}

		private class InternalTimer : Timer
		{
			private Mobile m_From;
			private Mobile m_Mobile;
			private int m_Count;

			public InternalTimer( Mobile from, Mobile m ) : base( TimeSpan.FromSeconds( 2.0 ), TimeSpan.FromSeconds( 2.0 ) )
			{
				m_From = from;
				m_Mobile = m;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				DoBurn( m_Mobile, m_From, 5 - m_Count );

				if ( ++m_Count == 5 )
					EndBurn( m_Mobile, true );
			}
		}

		private class InternalTarget : Target
		{
			private HellfireSpell m_Owner;

			public InternalTarget( HellfireSpell owner ) : base( 12, false, TargetFlags.Harmful )
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
