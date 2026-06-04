using System;
using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchMassSleep : ResearchSpell
	{
		public override int spellIndex { get { return 59; } }
		public override bool alwaysConsume { get{ return bool.Parse( Server.Misc.Research.SpellInformation( spellIndex, 14 )); } }
		public int CirclePower = 8;
		public static int spellID = 59;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 8 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 7 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Misc.Research.SpellInformation( spellID, 2 ),
				Server.Misc.Research.CapsCast( Server.Misc.Research.SpellInformation( spellID, 4 ) ),
				233,
				9042,
				Reagent.FairyEgg,Reagent.MandrakeRoot,Reagent.EnchantedSeaweed,Reagent.LichDust
			);

		public ResearchMassSleep( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.SendMessage( "Choose a focal point for this spell." );
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				if ( p is Item )
					p = ((Item)p).GetWorldLocation();

				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 8 );

					bool CanAffect = true;
					SlayerEntry undead = SlayerGroup.GetEntryByName( SlayerName.Silver );
					SlayerEntry elly = SlayerGroup.GetEntryByName( SlayerName.ElementalBan );
					SlayerEntry golem = SlayerGroup.GetEntryByName( SlayerName.GolemDestruction );

					foreach ( Mobile m in eable )
					{
						CanAffect = true;

						Mobile pet = m;
						if ( m is BaseCreature )
							pet = ((BaseCreature)m).GetMaster();

						if ( m is BaseCreature )
						{
							if (undead.Slays(m) || elly.Slays(m) || golem.Slays(m))
							{
								CanAffect = false;
							}
						}

						if ( Caster.Region == m.Region && Caster != m && Caster != pet && Caster.InLOS( m ) && m.Blessed == false && Caster.CanBeHarmful( m, true ) && !m.Paralyzed && CanAffect )
						{
							targets.Add( m );
						}
					}

					eable.Free();
				}

				if ( targets.Count > 0 )
				{
					Caster.PlaySound( 0x651 );
					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = targets[i];

						SpellHelper.Turn( Caster, m );

						if ( SpellHelper.ResolveMagicAbsorption( CirclePower, m ) )
						{
							TimeSpan duration = TimeSpan.FromSeconds( (DamagingSkill( Caster ) / 4) );

							m.Paralyze( duration );

							BuffInfo.RemoveBuff( m, BuffIcon.MassSleep );
							BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.MassSleep, 1063650, duration, m ) );

							new SleepyTimer( m, duration, Caster ).Start();

							HarmfulSpell( m );
						}
					}
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchMassSleep m_Owner;

			public InternalTarget( ResearchMassSleep owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if ( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}

		public class SleepyTimer : Timer
		{
			private Mobile m_m;
			private Mobile m_Caster;
			private DateTime m_Expire;
			private int m_Time;

			public SleepyTimer( Mobile sleeper, TimeSpan duration, Mobile caster ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.1 ) )
			{
				m_m = sleeper;
				m_Expire = DateTime.Now + duration;
				m_Time = 0;
				m_Caster = caster;
			}

			protected override void OnTick()
			{
				if ( m_Time < 1 )
				{
					m_m.FixedParticles( 0x3039, 9, 32, 5008, Server.Misc.PlayerSettings.GetMySpellHue( true, m_Caster, 0xB72 ), 0, EffectLayer.Waist );
				}
				m_Time++;
				if ( m_Time > 30 )
				{
					m_Time = 1;
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