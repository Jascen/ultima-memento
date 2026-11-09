using System;
using System.Collections;
using Server.Targeting;
using Server.Misc;
using Server.Items;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchConflagration : ResearchSpell
	{
		public int CirclePower = 6;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 608,
					IconGraphic = 11224,
					Name = "Conflagration",
					PowerWords = "Kal Vas Flam Corp Xen",
					Description = "Creates a huge inferno that can cause 2-16 points of fire damage per second to anyone within it.",
					ManaCost = 20,
					MinSkill = 35,
					TargetType = TargetFlags.None
				},
				215,
				9041,
				Reagent.SulfurousAsh,Reagent.Brimstone,Reagent.GargoyleEar
			);

		public ResearchConflagration( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.SendMessage( "Choose a focal point for this spell." );
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

				Effects.PlaySound( p, Caster.Map, 0x5CF );

				int itemID = Utility.RandomList(0x398C,0x3996);

				TimeSpan duration = TimeSpan.FromSeconds( (DamagingSkill( Caster )/1.6) );

				int damage = (int)(DamagingSkill( Caster )/3);
					if ( damage > 16 ){ damage = 16; }
					if ( damage < 2 ){ damage = 2; }

				int fires = 13;
				int x = p.X;
				int y = p.Y;
				Point3D loc = new Point3D( p.X, p.Y, p.Z );

				while ( fires > 0 )
				{
					if ( fires == 13 ){ loc = new Point3D( x, y-2, p.Z ); }
					else if ( fires == 12 ){ loc = new Point3D( x-1, y-1, p.Z ); }
					else if ( fires == 11 ){ loc = new Point3D( x, y-1, p.Z ); }
					else if ( fires == 10 ){ loc = new Point3D( x+1, y-1, p.Z ); }
					else if ( fires == 9 ){ loc = new Point3D( x-2, y, p.Z ); }
					else if ( fires == 8 ){ loc = new Point3D( x-1, y, p.Z ); }
					else if ( fires == 7 ){ loc = new Point3D( x+1, y, p.Z ); }
					else if ( fires == 6 ){ loc = new Point3D( x+2, y, p.Z ); }
					else if ( fires == 5 ){ loc = new Point3D( x-1, y+1, p.Z ); }
					else if ( fires == 4 ){ loc = new Point3D( x, y+1, p.Z ); }
					else if ( fires == 3 ){ loc = new Point3D( x+1, y+1, p.Z ); }
					else if ( fires == 2 ){ loc = new Point3D( x, y+2, p.Z ); }
					else { loc = new Point3D( x, y, p.Z ); }

					new FireFieldItem( 0x55B1, loc, Caster, Caster.Map, duration, 0, damage );

					fires--;
				}
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}

			FinishSequence();
		}

		[DispellableField]
		public class FireFieldItem : Item
		{
			private Timer m_Timer;
			private DateTime m_End;
			private Mobile m_Caster;
			private int m_Damage;

			public override bool BlocksFit{ get{ return true; } }

			public FireFieldItem( int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val ) : this( itemID, loc, caster, map, duration, val, 2 )
			{
			}

			public FireFieldItem( int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val, int damage ) : base( itemID )
			{
				bool canFit = SpellHelper.AdjustField( ref loc, map, 12, false );

				Name = "conflagration";
				Visible = false;
				Movable = false;
				Light = LightType.Circle300;
				Hue = Server.Misc.PlayerSettings.GetMySpellHue( false, caster, 0 );

				MoveToWorld( loc, map );

				m_Caster = caster;

				m_Damage = damage;

				m_End = DateTime.Now + duration;

				m_Timer = new InternalTimer( this, TimeSpan.FromSeconds( Math.Abs( val ) * 0.2 ), caster.InLOS( this ), canFit );
				m_Timer.Start();
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if ( m_Timer != null )
					m_Timer.Stop();
			}

			public FireFieldItem( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 2 ); // version

				writer.Write( m_Damage );
				writer.Write( m_Caster );
				writer.WriteDeltaTime( m_End );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				switch ( version )
				{
					case 2:
					{
						m_Damage = reader.ReadInt();
						goto case 1;
					}
					case 1:
					{
						m_Caster = reader.ReadMobile();

						goto case 0;
					}
					case 0:
					{
						m_End = reader.ReadDeltaTime();

						m_Timer = new InternalTimer( this, TimeSpan.Zero, true, true );
						m_Timer.Start();

						break;
					}
				}

				if( version < 2 )
					m_Damage = 2;
			}

			public override bool OnMoveOver( Mobile m )
			{
				if ( Visible && !Server.Spells.SpellHelper.isFriend( m_Caster, m ) )
				{
					if ( SpellHelper.CanRevealCaster( m ) )
						m_Caster.RevealingAction();
					
					m_Caster.DoHarmful( m );

					int damage = m_Damage;

					AOS.Damage( m, m_Caster, damage, 0, 100, 0, 0, 0 );
					m.PlaySound( 0x5CF );

					if ( m is BaseCreature )
						((BaseCreature) m).OnHarmfulSpell( m_Caster );
				}

				return true;
			}

			private class InternalTimer : Timer
			{
				private FireFieldItem m_Item;
				private bool m_InLOS, m_CanFit;

				private static Queue m_Queue = new Queue();

				public InternalTimer( FireFieldItem item, TimeSpan delay, bool inLOS, bool canFit ) : base( delay, TimeSpan.FromSeconds( 1.0 ) )
				{
					m_Item = item;
					m_InLOS = inLOS;
					m_CanFit = canFit;

					Priority = TimerPriority.FiftyMS;
				}

				protected override void OnTick()
				{
					if ( m_Item.Deleted )
						return;

					if ( !m_Item.Visible )
					{
						if ( m_InLOS && m_CanFit )
							m_Item.Visible = true;
						else
							m_Item.Delete();

						if ( !m_Item.Deleted )
						{
							m_Item.ProcessDelta();
							Effects.SendLocationParticles( EffectItem.Create( m_Item.Location, m_Item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, m_Item.m_Caster, 0 ), 0, 5029, 0 );
						}
					}
					else if ( DateTime.Now > m_Item.m_End )
					{
						m_Item.Delete();
						Stop();
					}
					else
					{
						Map map = m_Item.Map;
						Mobile caster = m_Item.m_Caster;

						if ( map != null && caster != null )
						{
							foreach ( Mobile m in m_Item.GetMobilesInRange( 0 ) )
							{
								if ( (m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z && !Server.Spells.SpellHelper.isFriend( caster, m ) )
									m_Queue.Enqueue( m );
							}

							while ( m_Queue.Count > 0 )
							{
								Mobile m = (Mobile)m_Queue.Dequeue();
								
								if ( SpellHelper.CanRevealCaster( m ) )
									caster.RevealingAction();

								caster.DoHarmful( m );

								int damage = m_Item.m_Damage;

								AOS.Damage( m, caster, damage, 0, 100, 0, 0, 0 );
								m.PlaySound( 0x5CF );

								if ( m is BaseCreature )
									((BaseCreature) m).OnHarmfulSpell( caster );
							}
						}
					}
				}
			}
		}

		private class InternalTarget : Target
		{
			private ResearchConflagration m_Owner;

			public InternalTarget( ResearchConflagration owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
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