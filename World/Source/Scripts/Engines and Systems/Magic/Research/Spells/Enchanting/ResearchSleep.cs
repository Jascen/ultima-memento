using System;
using Server;
using System.Collections;
using Server.Network;
using System.Text;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchSleep : ResearchSpell
	{
		public override int spellIndex { get { return 19; } }
		public int CirclePower = 4;
		public static int spellID = 19;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.5 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 8 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 7 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Misc.Research.SpellInformation( spellID, 2 ),
				Server.Misc.Research.CapsCast( Server.Misc.Research.SpellInformation( spellID, 4 ) ),
				236,
				9031,
				Reagent.FairyEgg,Reagent.MandrakeRoot,Reagent.SeaSalt
			);

		public ResearchSleep( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.SendMessage( "Who do you want to put to sleep?" );
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			bool CanAffect = true;

			if ( m is BaseCreature )
			{
				SlayerEntry undead = SlayerGroup.GetEntryByName( SlayerName.Silver );
				SlayerEntry elly = SlayerGroup.GetEntryByName( SlayerName.ElementalBan );
				SlayerEntry golem = SlayerGroup.GetEntryByName( SlayerName.GolemDestruction );
				if (undead.Slays(m) || elly.Slays(m) || golem.Slays(m))
				{
					CanAffect = false;
				}
			}

			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( m.Frozen || m.Paralyzed )
			{
				Caster.SendLocalizedMessage( 1061923 ); // The target is already frozen.
			}
			else if ( !CanAffect )
			{
				Caster.SendMessage( "This spell cannot affect supernatural creatures, golems, constructs, or elementals." );
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				bool hitThrough = SpellHelper.ResolveMagicDefense( CirclePower, ref source, ref m );

				m.PlaySound( 0x657 );
				m.FixedParticles( 0x3039, 9, 32, 5008, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB72 ), 0, EffectLayer.Waist );

				if ( hitThrough )
				{
					TimeSpan duration = TimeSpan.FromSeconds( (DamagingSkill( Caster ) / 4) );

					m.Paralyze( duration );

					BuffInfo.RemoveBuff( m, BuffIcon.Sleep );
					BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.Sleep, 1063646, duration, m ) );

					new SleepyTimer( m, duration ).Start();
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

					HarmfulSpell( m );
				}
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private ResearchSleep m_Owner;

			public InternalTarget( ResearchSleep owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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

		public class SleepyTimer : Timer
		{
			private Mobile m_m;
			private DateTime m_Expire;
			private int m_Time;

			public SleepyTimer( Mobile sleeper, TimeSpan duration ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.1 ) )
			{
				m_m = sleeper;
				m_Expire = DateTime.Now + duration;
				m_Time = 0;
			}

			protected override void OnTick()
			{
				m_Time++;
				if ( m_Time > 30 )
				{
					m_Time = 0;
					Point3D zzz = new Point3D( m_m.X, m_m.Y+1, m_m.Z+20 );
					Effects.SendLocationParticles(EffectItem.Create(zzz, m_m.Map, EffectItem.DefaultDuration), 0x4B4E, 9, 32, 0xB71, 0, 5022, 0);
					if ( m_m.Female ){ m_m.PlaySound( 819 ); } else { m_m.PlaySound( 1093 ); }
				}

				if ( !m_m.Frozen && !m_m.Paralyzed )
				{
					Stop();
				}

				if ( DateTime.Now >= m_Expire )
				{
					Stop();
				}
			}
		}
	}
}