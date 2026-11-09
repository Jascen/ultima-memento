using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchSleep : ResearchSpell
	{
		public int CirclePower = 4;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 646,
					IconGraphic = 11213,
					Name = "Sleep",
					PowerWords = "In Zu Tym",
					Description = "This spell can put a foe to sleep from 10-60 seconds, but taking actions against them will likely wake them up from their slumber. This spell cannot affect supernatural creatures, constructs, golems, or elementals.",
					ManaCost = 15,
					MinSkill = 40,
					TargetType = TargetFlags.Harmful
				},
				236,
				9031,
				Reagent.FairyEgg,Reagent.MandrakeRoot,Reagent.SeaSalt
			);

		public ResearchSleep( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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