using System;
using System.Collections;
using Server.Targeting;
using Server.Misc;
using Server.Items;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchFrostField : ResearchSpell
	{
		public int CirclePower = 4;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.25 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 627,
					IconGraphic = 11253,
					Name = "Frost Field",
					PowerWords = "In Ex Des",
					Description = "Creates a wall of frost that can cause 2-16 points of cold damage per second to anyone within it.",
					ManaCost = 15,
					MinSkill = 30,
					TargetType = TargetFlags.None
				},
				236,
				9031,
				Reagent.MoonCrystal,Reagent.EyeOfToad,Reagent.SpidersSilk
			);

		public ResearchFrostField( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
				{
					eastToWest = false;
				}
				else if ( rx >= 0 )
				{
					eastToWest = true;
				}
				else if ( ry >= 0 )
				{
					eastToWest = true;
				}
				else
				{
					eastToWest = false;
				}

				Effects.PlaySound( p, Caster.Map, 0x64F );

				int itemID = eastToWest ? 0x3946 : 0x3956;

				TimeSpan duration = TimeSpan.FromSeconds( (DamagingSkill( Caster )/1.6) );

				int damage = (int)(DamagingSkill( Caster )/3);
					if ( damage > 16 ){ damage = 16; }
					if ( damage < 2 ){ damage = 2; }

				for ( int i = -2; i <= 2; ++i )
				{
					Point3D loc = new Point3D( eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z );

					new FrostFieldItem( itemID, loc, Caster, Caster.Map, duration, i, damage );
				}
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}

			FinishSequence();
		}

		[DispellableField]
		public class FrostFieldItem : Item
		{
			private Timer m_Timer;
			private DateTime m_End;
			private Mobile m_Caster;
			private int m_Damage;

			public override bool BlocksFit{ get{ return true; } }

			public FrostFieldItem( int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val ) : this( itemID, loc, caster, map, duration, val, 2 )
			{
			}

			public FrostFieldItem( int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val, int damage ) : base( itemID )
			{
				bool canFit = SpellHelper.AdjustField( ref loc, map, 12, false );

				Name = "field of frost";
				Visible = false;
				Movable = false;
				Light = LightType.Circle300;
				Hue = Server.Misc.PlayerSettings.GetMySpellHue( false, caster, 0xB77 );

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

			public FrostFieldItem( Serial serial ) : base( serial )
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
			}

			public override bool OnMoveOver( Mobile m )
			{
				if ( Visible && !Server.Spells.SpellHelper.isFriend( m_Caster, m ) )
				{
					if ( SpellHelper.CanRevealCaster( m ) )
						m_Caster.RevealingAction();
					
					m_Caster.DoHarmful( m );

					int damage = m_Damage;

					AOS.Damage( m, m_Caster, damage, 0, 0, 100, 0, 0 );
					m.PlaySound( 0x5C7 );

					if ( m is BaseCreature )
						((BaseCreature) m).OnHarmfulSpell( m_Caster );
				}

				return true;
			}

			private class InternalTimer : Timer
			{
				private FrostFieldItem m_Item;
				private bool m_InLOS, m_CanFit;

				private static Queue m_Queue = new Queue();

				public InternalTimer( FrostFieldItem item, TimeSpan delay, bool inLOS, bool canFit ) : base( delay, TimeSpan.FromSeconds( 1.0 ) )
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
							Effects.SendLocationParticles( EffectItem.Create( m_Item.Location, m_Item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, m_Item.m_Caster, 0xB77 ), 0, 5029, 0 );
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

								AOS.Damage( m, caster, damage, 0, 0, 100, 0, 0 );
								m.PlaySound( 0x5C7 );

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
			private ResearchFrostField m_Owner;

			public InternalTarget( ResearchFrostField owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
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