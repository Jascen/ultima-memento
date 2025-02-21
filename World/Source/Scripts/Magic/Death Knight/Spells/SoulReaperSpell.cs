using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Spells.DeathKnight
{
	public class SoulReaperSpell : DeathKnightSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Soul Reaper", "Xaphan Spiritum",
				221,
				9032
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1 ); } }
		public override int RequiredTithing{ get{ return 63; } }
		public override double RequiredSkill{ get{ return 45.0; } }
		public override int RequiredMana{ get{ return 40; } }

		private static Hashtable m_Table = new Hashtable();

		public SoulReaperSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public static bool HasEffect( Mobile m )
		{
			return ( m_Table[m] != null );
		}

		public static void RemoveEffect( Mobile m )
		{
			Timer t = (Timer)m_Table[m];

			if ( t != null )
			{
				t.Stop();
				m_Table.Remove( m );
			} 
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Timer t = new InternalTimer( m );

				m_Table[m] = t;

				t.Start();

				m.FixedParticles( 0x374A, 10, 15, 5032, EffectLayer.Head );
				m.PlaySound( 0x1F8 );
				m.SendMessage( "You feel your soul weakening." );

				DrainSoulsInLantern( Caster, RequiredTithing );
			}

			FinishSequence();
		}

		private class InternalTimer : Timer
		{
			private Mobile m_Owner;
			private DateTime m_Expire;

			public InternalTimer( Mobile owner ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.5 ) )
			{
				m_Owner = owner;
				m_Expire = DateTime.Now + TimeSpan.FromSeconds( 30.0 );

				BuffInfo.RemoveBuff( m_Owner, BuffIcon.SoulReaper );
				BuffInfo.AddBuff( m_Owner, new BuffInfo( BuffIcon.SoulReaper, 1063555, TimeSpan.FromSeconds( 30.0 ), m_Owner ) );
			}

			protected override void OnTick()
			{
				if ( !m_Owner.CheckAlive() || DateTime.Now >= m_Expire )
				{
					Stop();
					m_Table.Remove( m_Owner );
					m_Owner.SendMessage( "Your soul begins to recover." );
				}
				else if ( m_Owner.Mana < 10 )
				{
					m_Owner.Mana = 0;
				}
				else
				{
					m_Owner.Mana = m_Owner.Mana - 10;
				}
			}
		}

		private class InternalTarget : Target
		{
			private SoulReaperSpell m_Owner;

			public InternalTarget( SoulReaperSpell owner ) : base( 12, false, TargetFlags.Harmful )
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
