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
	public class ResearchCharm : ResearchSpell
	{
		public override int spellIndex { get { return 51; } }
		public int CirclePower = 7;
		public static int spellID = 51;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.5 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 8 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 7 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Misc.Research.SpellInformation( spellID, 2 ),
				Server.Misc.Research.CapsCast( Server.Misc.Research.SpellInformation( spellID, 4 ) ),
				236,
				9031,
				Reagent.EnchantedSeaweed,Reagent.FairyEgg,Reagent.PixieSkull
			);

		public ResearchCharm( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.SendMessage( "Who do you want to charm?" );
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
			else if ( m is PlayerMobile )
			{
				m.Paralyze( TimeSpan.FromSeconds( (DamagingSkill( Caster ) / 4) ) );
				m.SendMessage( "You are charmed." );
				BuffInfo.RemoveBuff( m, BuffIcon.Charm );
				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.Charm, 1063686, TimeSpan.FromSeconds( (DamagingSkill( Caster ) / 4) ), m ) );
			}
			else if ( !CanAffect )
			{
				Caster.SendMessage( "You cannot charm supernatural creatures, golems, constructs, or elementals." );
			}
			else if ( m is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)m;

				int pFame = (int)( DamagingSkill( Caster ) ) * 60;
				int mFame = m.Fame;

				if ( bc.ControlMaster != null )
				{
					Caster.SendMessage( "That is already controlled by another." );
				}
				else if ( bc.FightMode != FightMode.Closest && bc.FightMode != FightMode.Aggressor )
				{
					Caster.SendMessage( "These charms will not work very well on that." );
				}
				else if ( mFame > pFame )
				{
					Caster.SendMessage( "That creature is too powerful for you to charm." );
				}
				else if ( CheckHSequence( m ) )
				{
					SpellHelper.Turn( Caster, m );

					Mobile source = Caster;
					if ( SpellHelper.ResolveMagicDefense( CirclePower, ref source, ref m ) )
					{
						TimeSpan duration = TimeSpan.FromSeconds( (DamagingSkill( Caster ) / 4) );

						if ( bc.FightMode == FightMode.Closest ){ bc.FightMode = FightMode.CharmMonster; }
						else if ( bc.FightMode == FightMode.Aggressor ){ bc.FightMode = FightMode.CharmAnimal; }

						m.PlaySound( 0x20B );

						m.FixedParticles( 0x3039, 9, 32, 5008, 0x48F, 0, EffectLayer.Waist );

						new CharmTimer( m, duration ).Start();
						Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

						HarmfulSpell( m );
					}
				}

				FinishSequence();
			}
			else
			{
				Caster.SendMessage( "This spell cannot affect that." );
			}
		}

		public class InternalTarget : Target
		{
			private ResearchCharm m_Owner;

			public InternalTarget( ResearchCharm owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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

		public class CharmTimer : Timer
		{
			private Mobile m_m;
			private DateTime m_Expire;
			private int m_Time;

			public CharmTimer( Mobile charmed, TimeSpan duration ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.1 ) )
			{
				m_m = charmed;
				m_Expire = DateTime.Now + duration;
				m_Time = 0;
			}

			protected override void OnTick()
			{
				m_Time++;
				if ( m_Time > 60 )
				{
					m_Time = 0;
					Point3D charm = new Point3D( m_m.X+1, m_m.Y+1, m_m.Z+10 );
					Effects.SendLocationParticles(EffectItem.Create(charm, m_m.Map, EffectItem.DefaultDuration), 0x3039, 9, 32, 0x48F, 0, 5022, 0);
				}

				if ( DateTime.Now >= m_Expire )
				{
					BaseCreature bc = (BaseCreature)m_m;
					if ( bc.FightMode == FightMode.CharmMonster ){ bc.FightMode = FightMode.Closest; }
					else if ( bc.FightMode == FightMode.CharmAnimal ){ bc.FightMode = FightMode.Aggressor; }
					Stop();
				}
			}
		}
	}
}