using System;
using Server.Targeting;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Misc;

namespace Server.Spells.Research
{
	public class ResearchOpenGround : ResearchSpell
	{
		public override bool alwaysConsume { get{ return bool.Parse( Server.Misc.Research.SpellInformation( spellIndex, 14 )); } }
		public int CirclePower = 7;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.00 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 641,
					IconGraphic = 11209,
					Name = "Open Ground",
					PowerWords = "Des Por Ylem",
					Description = "Opens a huge hole in the ground that can cause anyone that goes near it to fall in. Creatures may fall to their deaths and never be seen again, while advenuterers will fall into the huge chasm below. If an adventurer has comrades, familiars, summons, or tamed beasts that stumble into the chasm, then the adventurer will fall in after them. This spell is powerful enough that the scroll will always crumble to dust when cast. The caster may lose some karma if they attempt this.",
					ManaCost = 65,
					MinSkill = 85,
					TargetType = TargetFlags.None
				},
				212,
				9001,
				Reagent.Brimstone,Reagent.GraveDust,Reagent.PigIron,Reagent.MandrakeRoot
			);

		public ResearchOpenGround( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.SendMessage( "Where do you want the rip the ground open?" );
			Caster.Target = new InternalTarget( this );
		}

		public void MTarget( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				int pits = 0;

				foreach ( Item m in Caster.GetItemsInRange( 10 ) )
				{
					if ( m is OpenGround )
						++pits;
				}

				if ( pits > 0 )
				{
					Caster.SendMessage( "There is already an open chasm nearby!" );
				}
				else if ( Server.Misc.Worlds.NoApocalypse( Caster.Location, Caster.Map ) )
				{
					Caster.SendMessage( "You don't think it is wise to open a chasm in this area." ); 
					return;
				}
				else
				{
					int cycle = 25;

					SpellHelper.Turn( Caster, p );
					SpellHelper.GetSurfaceTop( ref p );

					int x = p.X+2;
					int y = p.Y+2;
					int z = p.Z;

					int bX = 0;
					int bY = 0;

					Point3D loc = new Point3D( 0,0,0 );
					OpenGround piT = null;
					Point3D blast = new Point3D( ( p.X+2 ), ( p.Y+2 ), p.Z+30 );
					Point3D dirt1 = new Point3D( ( p.X+2 ), ( p.Y+2 ), p.Z+30 );
					Point3D dirt2 = new Point3D( ( p.X+5 ), ( p.Y+1 ), p.Z+30 );
					Point3D dirt3 = new Point3D( ( p.X+2 ), ( p.Y+5 ), p.Z+30 );

					while ( cycle > 0 )
					{
						if ( cycle == 25 ){ bX = x + 0; bY = y + 0; }
						else if ( cycle == 24 ){ bX = x + 0; bY = y + -3; }
						else if ( cycle == 23 ){ bX = x + 0; bY = y + -4; }
						else if ( cycle == 22 ){ bX = x + 0; bY = y + -5; }
						else if ( cycle == 21 ){ bX = x + -1; bY = y + -2; }
						else if ( cycle == 20 ){ bX = x + -1; bY = y + -3; }
						else if ( cycle == 19 ){ bX = x + -1; bY = y + -4; }
						else if ( cycle == 18 ){ bX = x + -1; bY = y + -5; }
						else if ( cycle == 17 ){ bX = x + -2; bY = y + -1; }
						else if ( cycle == 16 ){ bX = x + -2; bY = y + -2; }
						else if ( cycle == 15 ){ bX = x + -2; bY = y + -3; }
						else if ( cycle == 14 ){ bX = x + -2; bY = y + -4; }
						else if ( cycle == 13 ){ bX = x + -2; bY = y + -5; }
						else if ( cycle == 12 ){ bX = x + -3; bY = y + 0; }
						else if ( cycle == 11 ){ bX = x + -3; bY = y + -1; }
						else if ( cycle == 10 ){ bX = x + -3; bY = y + -2; }
						else if ( cycle == 9 ){ bX = x + -3; bY = y + -3; }
						else if ( cycle == 8 ){ bX = x + -3; bY = y + -4; }
						else if ( cycle == 7 ){ bX = x + -4; bY = y + 0; }
						else if ( cycle == 6 ){ bX = x + -4; bY = y + -1; }
						else if ( cycle == 5 ){ bX = x + -4; bY = y + -2; }
						else if ( cycle == 4 ){ bX = x + -4; bY = y + -3; }
						else if ( cycle == 3 ){ bX = x + -5; bY = y + 0; }
						else if ( cycle == 2 ){ bX = x + -5; bY = y + -1; }
						else { bX = x + -5; bY = y + -2; }

						loc = new Point3D( bX, bY, z );
						piT = new OpenGround( Caster );

						if ( cycle == 25 ){ piT.ItemID = Utility.RandomList( 0x4CC8, 0x4CC9 ); piT.Visible = true; }

						piT.Map = Caster.Map; 
						piT.Location = loc;

						cycle--;
					}
					Effects.SendLocationEffect( blast, Caster.Map, 0x2A4E, 30, 10, 0, 0 );
					Effects.PlaySound( blast, Caster.Map, 0x664 );

					Effects.SendLocationEffect( dirt1, Caster.Map, 0x23B2, 30, 10, 0xABF, 0 );
					Effects.SendLocationEffect( dirt2, Caster.Map, 0x23B2, 30, 10, 0xABF, 0 );
					Effects.SendLocationEffect( dirt3, Caster.Map, 0x23B2, 30, 10, 0xABF, 0 );

					Caster.PlaySound( 0x029 );
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

					KarmaMod( Caster, ((int)RequiredSkill+RequiredMana) );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchOpenGround m_Owner;

			public InternalTarget( ResearchOpenGround owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is IPoint3D )
				{
					m_Owner.MTarget( (IPoint3D)o );
				}
				else
				{
					from.SendMessage( "The spell doesn't seem to do anything there." );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}

namespace Server.Items
{
	public class OpenGround : Item
	{
		public Mobile owner;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner { get{ return owner; } set{ owner = value; } }

		private DateTime m_DecayTime;
		private Timer m_DecayTimer;

		public virtual TimeSpan DecayDelay{ get{ return TimeSpan.FromSeconds( 60.0 ); } } // HOW LONG UNTIL THE TRAP DECAYS IN SECONDS

		[Constructable]
		public OpenGround( Mobile source ) : base( 0x1B72 )
		{
			Movable = false;
			Visible = false;
			Name = "chasm";
			RefreshDecay( true );
			owner = source;
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( ItemID == 0x4CC8 || ItemID == 0x4CC9 )
			{
				return true;
			}
			else
			{
				if ( m is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)m;
					if ( bc.GetMaster() != null )
					{
						Mobile pc = bc.GetMaster();
						if ( pc != null )
						{
							MoveToPit( pc, owner );
						}
						else
						{
							Effects.PlaySound( m.Location, m.Map, Utility.RandomList( 0x5D2,0x5D3 ) );
							m.Delete();
						}
					}
					else
					{
						if ( owner is PlayerMobile && owner != null )
						{
							owner.Criminal = true;
						}
						else if ( m is BaseVendor || m is BasePerson )
						{
							owner.Criminal = true;
							owner.Kills = owner.Kills + 1;
						}
						Effects.PlaySound( m.Location, m.Map, Utility.RandomList( 0x5D2,0x5D3 ) );
						m.Delete();
					}
				}
				else if ( m is PlayerMobile )
				{
					MoveToPit( m, owner );
				}
				return false;
			}
		}

		public static void MoveToPit( Mobile m, Mobile owner )
		{
			string sX = m.X.ToString();
			string sY = m.Y.ToString();
			string sZ = m.Z.ToString();
			string sMap = Worlds.GetMyMapString( m.Map );
			string sZone = Server.Lands.LandName( Server.Lands.GetLand( m.Map, m.Location, m.X, m.Y ) );

			((PlayerMobile)m).CharacterPublicDoor = sX + "#" + sY + "#" + sZ + "#" + sMap + "#" + sZone;

			Effects.PlaySound( m.Location, m.Map, Utility.RandomList( 0x5D2,0x5D3 ) );
			Point3D p = new Point3D( 2602, 3688, 100 );
			Point3D b = new Point3D( 2602, 3688, 0 );
			Map map = Map.Sosaria;

			Effects.PlaySound( m.Location, m.Map, Utility.RandomList( 0x5D2,0x5D3 ) );
			owner.DoHarmful( m );
			Server.Mobiles.BaseCreature.TeleportPets( m, b, map );
			m.MoveToWorld( p, map );
			m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "You fall into a deep pit!");
		}

		public OpenGround(Serial serial) : base(serial)
		{
		}

		public virtual void RefreshDecay( bool setDecayTime )
		{
			if( Deleted )
				return;
			if( m_DecayTimer != null )
				m_DecayTimer.Stop();
			if( setDecayTime )
				m_DecayTime = DateTime.Now + DecayDelay;

			TimeSpan ts = m_DecayTime - DateTime.Now;

			if ( ts < TimeSpan.FromMinutes( 1.0 ) )
				ts = TimeSpan.FromMinutes( 1.0 );

			m_DecayTimer = Timer.DelayCall( ts, new TimerCallback( Delete ) );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write( m_DecayTime );
			writer.Write( (Mobile)owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			switch ( version )
			{
				case 0:
				{
					m_DecayTime = reader.ReadDateTime();
					RefreshDecay( false );
					break;
				}
			}
			owner = reader.ReadMobile();
		}
	}
}