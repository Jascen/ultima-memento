using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Herbalist
{
   public class SwarmOfInsectsSpell : HerbalistSpell
   {
		private static SpellInfo m_Info = new SpellInfo( "", "", 239, 9021 );
		public override int HerbalistSpellCircle{ get{ return 4; } }
		public override double CastDelay{ get{ return 2.0; } }
		public override double RequiredSkill{ get{ return 65.0; } }
		public override int RequiredMana{ get{ return 0; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }

		public SwarmOfInsectsSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				if ( SpellHelper.ResolveMagicDefense( 7, ref source, ref m ) )
				{
					CheckResisted( m ); // Check magic resist for skill, but do not use return value

					m.FixedParticles( 0x91B, 1, 240, 9916, 0, 3, EffectLayer.Head );
					m.PlaySound( 0x1E5 );

					double damage = ((Caster.Skills[CastSkill].Value + m.Skills[DamageSkill].Value) / 10) + (int)(Server.Items.BasePotion.EnhancePotions( m )/2);

					if ( damage < 1 )
						damage = 1;

					if ( m_Table.Contains( m ) )
						damage /= 10;
					else
						new InternalTimer( m, damage * 0.5 ).Start();

					SpellHelper.Damage( this, m, damage );
				}
			}

			FinishSequence();
		}

		private static Hashtable m_Table = new Hashtable();

		private class InternalTimer : Timer
		{
			private Mobile m_Mobile;
			private int m_ToRestore;

			public InternalTimer( Mobile m, double toRestore ) : base( TimeSpan.FromSeconds( 20.0 ) )
			{
				Priority = TimerPriority.OneSecond;

				m_Mobile = m;
				m_ToRestore = (int)toRestore;

				m_Table[m] = this;
			}

			protected override void OnTick()
			{
				m_Table.Remove( m_Mobile );

				if ( m_Mobile.Alive )
				m_Mobile.Hits += m_ToRestore;
			}
		}

		private class InternalTarget : Target
		{
			private SwarmOfInsectsSpell m_Owner;

			public InternalTarget( SwarmOfInsectsSpell owner ) : base( 12, false, TargetFlags.Harmful )
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

		public override bool CheckResisted( Mobile target )
		{
			double n = GetResistPercent( target );

			n /= 100.0;

			if( n <= 0.0 )
				return false;

			if( n >= 1.0 )
				return true;

			int maxSkill = (1 + 7) * 10;
			maxSkill += (1 + (7 / 6)) * 25;

			if( target.Skills[SkillName.MagicResist].Value < maxSkill )
				target.CheckSkill( SkillName.MagicResist, 0.0, 120.0 );

			return (n >= Utility.RandomDouble());
		}

		public override double GetResistPercentForCircle( Mobile target )
		{
			double firstPercent = target.Skills[SkillName.MagicResist].Value / 5.0;
			double secondPercent = target.Skills[SkillName.MagicResist].Value - (((Caster.Skills[DamageSkill].Value - 20.0) / 5.0) + (1 + 7) * 5.0);

			return (firstPercent > secondPercent ? firstPercent : secondPercent) / 2.0; // Seems should be about half of what stratics says.
		}

		public override double GetResistPercent( Mobile target )
		{
			return GetResistPercentForCircle( target );
		}
   }
}
