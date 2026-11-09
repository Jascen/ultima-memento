using System;
using Server.Targeting;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchSleepField : ResearchSpell
	{
		public int CirclePower = 6;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.75 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 647,
					IconGraphic = 11216,
					Name = "Sleep Field",
					PowerWords = "In Zu Grav Tym",
					Description = "This spell will create a field that will put entering foes to sleep for 10-60 seconds, but taking actions against them will likely wake them up from their slumber. This spell cannot affect supernatural creatures, constructs, golems, or elementals.",
					ManaCost = 30,
					MinSkill = 60,
					TargetType = TargetFlags.None
				},
				230,
				9012,
				Reagent.FairyEgg,Reagent.MandrakeRoot,Reagent.SeaSalt,Reagent.GraveDust
			);

		public ResearchSleepField( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.SendMessage( "Where do you want the field to materialize?" );
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				SpellHelper.GetSurfaceTop( ref p );

				int dx = Caster.Location.X - p.X;
				int dy = Caster.Location.Y - p.Y;
				int rx = (dx - dy) * 44;
				int ry = (dx + dy) * 44;

				bool eastToWest;

				if ( rx >= 0 && ry >= 0 )
					eastToWest = false;
				else if ( rx >= 0 )
					eastToWest = true;
				else if ( ry >= 0 )
					eastToWest = true;
				else
					eastToWest = false;

				Effects.PlaySound( p, Caster.Map, 0x651 );
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

				int itemID = eastToWest ? 0x376A : 0x376A;

				TimeSpan duration = TimeSpan.FromSeconds( 3.0 + ( (DamagingSkill( Caster )/2) / 3.0) );

				for ( int i = -2; i <= 2; ++i )
				{
					Point3D loc = new Point3D( eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z );
					bool canFit = SpellHelper.AdjustField( ref loc, Caster.Map, 12, false );

					if ( !canFit )
						continue;

					Item item = new InternalItem( Caster, itemID, loc, Caster.Map, duration );
					item.ProcessDelta();

					Effects.SendLocationParticles( EffectItem.Create( loc, Caster.Map, EffectItem.DefaultDuration ), 0x376A, 9, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, 5048, 0 );
				}
			}

			FinishSequence();
		}

		[DispellableField]
		public class InternalItem : Item
		{
			private Timer m_Timer;
			private Mobile m_Caster;
			private DateTime m_End;

			public override bool BlocksFit{ get{ return true; } }

			public InternalItem( Mobile caster, int itemID, Point3D loc, Map map, TimeSpan duration ) : base( itemID )
			{
				Visible = false;
				Movable = false;
				Hue = Server.Misc.PlayerSettings.GetMySpellHue( false, caster, 0 );

				MoveToWorld( loc, map );

				if ( caster.InLOS( this ) )
					Visible = true;
				else
					Delete();

				if ( Deleted )
					return;

				m_Caster = caster;

				m_Timer = new InternalTimer( this, duration );
				m_Timer.Start();

				m_End = DateTime.Now + duration;
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if ( m_Timer != null )
					m_Timer.Stop();
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 0 ); // version

				writer.Write( m_Caster );
				writer.WriteDeltaTime( m_End );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				switch ( version )
				{
					case 0:
					{
						m_Caster = reader.ReadMobile();
						m_End = reader.ReadDeltaTime();

						m_Timer = new InternalTimer( this, m_End - DateTime.Now );
						m_Timer.Start();

						break;
					}
				}
			}

			public override bool OnMoveOver( Mobile m )
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

				if ( Visible && m_Caster != null && (!Core.AOS || m != m_Caster) && SpellHelper.ValidIndirectTarget( m_Caster, m ) && m_Caster.CanBeHarmful( m, false ) && CanAffect )
				{
					if ( SpellHelper.CanRevealCaster( m ) )
						m_Caster.RevealingAction();

					m_Caster.DoHarmful( m );

					TimeSpan duration = TimeSpan.FromSeconds( (DamagingSkill( m_Caster ) / 4) );

					m.Paralyze( duration );

					BuffInfo.RemoveBuff( m, BuffIcon.SleepField );
					BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.SleepField, 1063648, duration, m ) );

					m.PlaySound( 0x657 );

					m.FixedParticles( 0x3039, 9, 32, 5008, Server.Misc.PlayerSettings.GetMySpellHue( true, m_Caster, 0xB72 ), 0, EffectLayer.Waist );

					new SleepyTimer( m, duration ).Start();
				}

				return true;
			}

			private class InternalTimer : Timer
			{
				private Item m_Item;

				public InternalTimer( Item item, TimeSpan duration ) : base( duration )
				{
					Priority = TimerPriority.OneSecond;
					m_Item = item;
				}

				protected override void OnTick()
				{
					m_Item.Delete();
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

		private class InternalTarget : Target
		{
			private ResearchSleepField m_Owner;

			public InternalTarget( ResearchSleepField owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is IPoint3D )
					m_Owner.Target( (IPoint3D)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}