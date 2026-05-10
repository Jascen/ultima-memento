using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Movement;
using Server.Network;
using Server.Gumps;
using Server.Misc;

namespace Server.Multis
{
	public enum BoatOrder
	{
		Move,
		Course,
		Single
	}

	// Smooth sailing - https://github.com/runuo/runuo/commit/110bd99e565441c2df33bacb3696bdffb9d70675
	public abstract class BaseBoat : BaseMulti
	{
		const string CLEAR_THE_DECK = "clear the deck";

		// THE LAST TWO INTEGERS ARE THE SEA WIDTH AND HEIGHT //
		private static Rectangle2D[] m_FelWrap = new Rectangle2D[]{ new Rectangle2D( 16, 16, 5120-32, 4096-32 ) };
		private static Rectangle2D[] m_TramWrap = new Rectangle2D[]{ new Rectangle2D( 16, 16, 5120-32, 3127-32 ) };
		private static Rectangle2D[] m_SerpentIslandWrap = new Rectangle2D[]{ new Rectangle2D( 16, 16, 1870-32, 2047-32 ) };
		private static Rectangle2D[] m_AmbrosiaWrap = new Rectangle2D[]{ new Rectangle2D( 5122+16, 3036+16, 1004-32, 1059-32 ) };
		private static Rectangle2D[] m_IslesDreadWrap = new Rectangle2D[]{ new Rectangle2D( 16, 16, 1447-32, 1447-32 ) };
		private static Rectangle2D[] m_BottleWrap = new Rectangle2D[]{ new Rectangle2D( 6127+16, 828+16, 1040-32, 1915-32 ) };
		private static Rectangle2D[] m_UmberWrap = new Rectangle2D[]{ new Rectangle2D( 699+16, 3129+16, 1573-32, 966-32 ) };

		private static TimeSpan BoatDecayDelay = TimeSpan.FromDays( MyServerSettings.BoatDecay() );

		public static BaseBoat FindBoatAt( IPoint2D loc, Map map )
		{
			Sector sector = map.GetSector( loc );

			for ( int i = 0; i < sector.Multis.Count; i++ )
			{
				BaseBoat boat = sector.Multis[i] as BaseBoat;

				if ( boat != null && boat.Contains( loc.X, loc.Y ) )
					return boat;
			}

			return null;
		}

		private Hold m_Hold;
		public BoatDoor m_BoatDoor;
		private TillerMan m_TillerMan;
		private Mobile m_Owner;

		private Direction m_Facing;

		private Direction m_Moving;
		private int m_Speed;
		private int m_ClientSpeed;

		private bool m_Anchored;
		private string m_ShipName;

		private BoatOrder m_Order;

		private MapItem m_MapItem;
		private int m_NextNavPoint;

		private Plank m_PPlank, m_SPlank;

		private DateTime m_DecayTime;

		private Timer m_TurnTimer;
		private Timer m_MoveTimer;

		[CommandProperty( AccessLevel.GameMaster )]
		public Hold Hold{ get{ return m_Hold; } set{ m_Hold = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public BoatDoor BoatDoor{ get{ return m_BoatDoor; } set{ m_BoatDoor = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public TillerMan TillerMan{ get{ return m_TillerMan; } set{ m_TillerMan = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Plank PPlank{ get{ return m_PPlank; } set{ m_PPlank = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Plank SPlank{ get{ return m_SPlank; } set{ m_SPlank = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner{ get{ return m_Owner; } set{ m_Owner = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Direction Facing{ get{ return m_Facing; } set{ SetFacing( value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Direction Moving{ get{ return m_Moving; } set{ m_Moving = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsMoving{ get{ return ( m_MoveTimer != null ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Speed{ get{ return m_Speed; } set{ m_Speed = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Anchored{ get{ return m_Anchored; } set{ m_Anchored = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public string ShipName{ get{ return m_ShipName; } set{ m_ShipName = value; if ( m_TillerMan != null ) m_TillerMan.InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public BoatOrder Order{ get{ return m_Order; } set{ m_Order = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public MapItem MapItem{ get{ return m_MapItem; } set{ m_MapItem = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int NextNavPoint{ get{ return m_NextNavPoint; } set{ m_NextNavPoint = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime TimeOfDecay{ get{ return m_DecayTime; } set{ m_DecayTime = value; if ( m_TillerMan != null ) m_TillerMan.InvalidateProperties(); } }

		public int Status
		{
			get
			{
				DateTime start = TimeOfDecay - BoatDecayDelay;

				if ( DateTime.Now - start < TimeSpan.FromHours( 1.0 ) )
					return translateText( this, 1043010 ); // This structure is like new.

				if ( DateTime.Now - start < TimeSpan.FromDays( 2.0 ) )
					return translateText( this, 1043011 ); // This structure is slightly worn.

				if ( DateTime.Now - start < TimeSpan.FromDays( 3.0 ) )
					return translateText( this, 1043012 ); // This structure is somewhat worn.

				if ( DateTime.Now - start < TimeSpan.FromDays( 4.0 ) )
					return translateText( this, 1043013 ); // This structure is fairly worn.

				if ( DateTime.Now - start < TimeSpan.FromDays( 5.0 ) )
					return translateText( this, 1043014 ); // This structure is greatly worn.

				return translateText( this, 1043015 ); // This structure is in danger of collapsing.
			}
		}

		public virtual int NorthID{ get{ return 0; } }
		public virtual int  EastID{ get{ return 0; } }
		public virtual int SouthID{ get{ return 0; } }
		public virtual int  WestID{ get{ return 0; } }

		public virtual int HoldDistance{ get{ return 0; } }
		public virtual int BoatDoorDistance{ get{ return -2; } }
		public virtual int TillerManDistance{ get{ return 0; } }
		public virtual Point2D StarboardOffset{ get{ return Point2D.Zero; } }
		public virtual Point2D PortOffset{ get{ return Point2D.Zero; } }
		public virtual Point3D MarkOffset{ get{ return Point3D.Zero; } }

		public virtual BaseDockedBoat DockedBoat{ get{ return null; } }

		private static List<BaseBoat> m_Instances = new List<BaseBoat>();

		public static List<BaseBoat> Boats{ get{ return m_Instances; } }

		public BaseBoat() : base( 0x0 )
		{
			m_DecayTime = DateTime.Now + BoatDecayDelay;

			m_TillerMan = new TillerMan( this );
			m_Hold = new Hold( this );

			m_BoatDoor = null;
			if ( !isCarpet( this ) ){ m_BoatDoor = new BoatDoor( this ); }

			m_PPlank = new Plank( this, PlankSide.Port, 0 );
			m_SPlank = new Plank( this, PlankSide.Starboard, 0 );

			m_PPlank.MoveToWorld( new Point3D( X + PortOffset.X, Y + PortOffset.Y, Z ), Map );
			m_SPlank.MoveToWorld( new Point3D( X + StarboardOffset.X, Y + StarboardOffset.Y, Z ), Map );

			Facing = Direction.North;

			m_NextNavPoint = -1;

			Movable = false;

			m_Instances.Add( this );
		}

		public static bool isCarpet( BaseBoat rug )
		{
			if (	rug is MagicCarpetA || 
					rug is MagicCarpetB || 
					rug is MagicCarpetC || 
					rug is MagicCarpetD || 
					rug is MagicCarpetE || 
					rug is MagicCarpetF || 
					rug is MagicCarpetG || 
					rug is MagicCarpetH || 
					rug is MagicCarpetI )
				return true;

			return false;
		}

		public static bool isRolledCarpet( Item rug )
		{
			if (	rug is MagicCarpetADeed || rug is MagicDockedCarpetA || 
					rug is MagicCarpetBDeed || rug is MagicDockedCarpetB || 
					rug is MagicCarpetCDeed || rug is MagicDockedCarpetC || 
					rug is MagicCarpetDDeed || rug is MagicDockedCarpetD || 
					rug is MagicCarpetEDeed || rug is MagicDockedCarpetE || 
					rug is MagicCarpetFDeed || rug is MagicDockedCarpetF || 
					rug is MagicCarpetGDeed || rug is MagicDockedCarpetG || 
					rug is MagicCarpetHDeed || rug is MagicDockedCarpetH || 
					rug is MagicCarpetIDeed || rug is MagicDockedCarpetI )
				return true;

			return false;
		}

		public static bool isRug( int item )
		{
			if ( item >= 0xBB && item <= 0xCC )
				return true;

			return false;
		}

		public static int translateText( BaseBoat boat, int text )
		{
			if ( isCarpet( boat ) )
			{
				if ( text == 1042884 ){ text = 1041532; }
				else if ( text == 1042885 ){ text = 1041533; }
				else if ( text == 502490 ){ text = 1041534; }
				else if ( text == 502491 ){ text = 1041535; }
				else if ( text == 501419 ){ text = 1041556; }
				else if ( text == 501423 ){ text = 1041550; }
				else if ( text == 501424 ){ text = 1041557; }
				else if ( text == 501429 ){ text = 1041540; }
				else if ( text == 501443 ){ text = 1041551; }
				else if ( text == 501444 ){ text = 1041537; }
				else if ( text == 501446 ){ text = 1041539; }
				else if ( text == 501447 ){ text = 1041538; }
				else if ( text == 501455 ){ text = 1041536; }
				else if ( text == 502513 ){ text = 1041552; }
				else if ( text == 502514 ){ text = 1041553; }
				else if ( text == 502515 ){ text = 1041555; }
				else if ( text == 502526 ){ text = 1041545; }
				else if ( text == 502531 ){ text = 1041543; }
				else if ( text == 502534 ){ text = 1041544; }
				else if ( text == 502575 ){ text = 1041547; }
				else if ( text == 502576 ){ text = 1041548; }
				else if ( text == 502577 ){ text = 1041549; }
				else if ( text == 502580 ){ text = 1041541; }
				else if ( text == 1007168 ){ text = 1041563; }
				else if ( text == 1007169 ){ text = 1041563; }
				else if ( text == 1007170 ){ text = 1041563; }
				else if ( text == 1007171 ){ text = 1041563; }
				else if ( text == 1007172 ){ text = 1041563; }
				else if ( text == 1042551 ){ text = 1041554; }
				else if ( text == 1042874 ){ text = 1041560; }
				else if ( text == 1042875 ){ text = 1041561; }
				else if ( text == 1042876 ){ text = 1041564; }
				else if ( text == 1042877 ){ text = 1041564; }
				else if ( text == 1042878 ){ text = 1041564; }
				else if ( text == 1042879 ){ text = 1041564; }
				else if ( text == 1042880 ){ text = 1041542; }
				else if ( text == 1042881 ){ text = 1041558; }
				else if ( text == 1042882 ){ text = 1041546; }
				else if ( text == 1042883 ){ text = 1041562; }
				else if ( text == 1042885 ){ text = 1041559; }
				else if ( text == 1043010 ){ text = 1041565; }
				else if ( text == 1043011 ){ text = 1041566; }
				else if ( text == 1043012 ){ text = 1041567; }
				else if ( text == 1043013 ){ text = 1041568; }
				else if ( text == 1043014 ){ text = 1041569; }
				else if ( text == 1043015 ){ text = 1041570; }
				else if ( text == 502484 ){ text = 1041571; }
				else if ( text == 502485 ){ text = 1041572; }
				else if ( text == 502483 ){ text = 1041573; }
				else if ( text == 502494 ){ text = 1041574; }
				else if ( text == 1010570 ){ text = 1041575; }
				else if ( text == 502495 ){ text = 1041576; }
				else if ( text == 502496 ){ text = 1041577; }
				else if ( text == 502497 ){ text = 1041578; }
			}

			return text;
		}

		public BaseBoat( Serial serial ) : base( serial )
		{
		}

		public Point3D GetRotatedLocation( int x, int y )
		{
			Point3D p = new Point3D( X + x, Y + y, Z );

			return Rotate( p, (int)m_Facing / 2 );
		}

		public void UpdateComponents()
		{
			if ( m_PPlank != null )
			{
				m_PPlank.MoveToWorld( GetRotatedLocation( PortOffset.X, PortOffset.Y ), Map );
				m_PPlank.SetFacing( m_Facing );
			}

			if ( m_SPlank != null )
			{
				m_SPlank.MoveToWorld( GetRotatedLocation( StarboardOffset.X, StarboardOffset.Y ), Map );
				m_SPlank.SetFacing( m_Facing );
			}

			int xOffset = 0, yOffset = 0;
			Movement.Movement.Offset( m_Facing, ref xOffset, ref yOffset );

			if ( m_TillerMan != null )
			{
				m_TillerMan.Location = new Point3D( X + (xOffset * TillerManDistance) + (m_Facing == Direction.North ? 1 : 0), Y + (yOffset * TillerManDistance), m_TillerMan.Z );
				m_TillerMan.SetFacing( m_Facing );
				m_TillerMan.InvalidateProperties();
			}

			if ( m_Hold != null )
			{
				m_Hold.Location = new Point3D( X + (xOffset * HoldDistance), Y + (yOffset * HoldDistance), m_Hold.Z );
				m_Hold.SetFacing( m_Facing );
			}

			if ( m_BoatDoor != null )
			{
				m_BoatDoor.Location = new Point3D( X + (xOffset * BoatDoorDistance), Y + (yOffset * BoatDoorDistance), m_BoatDoor.Z );
				m_BoatDoor.SetFacing( m_Facing );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 );

			writer.Write( (Item) m_MapItem );
			writer.Write( (int) m_NextNavPoint );

			writer.Write( (int) m_Facing );

			writer.WriteDeltaTime( m_DecayTime );

			writer.Write( m_Owner );
			writer.Write( m_PPlank );
			writer.Write( m_SPlank );
			writer.Write( m_TillerMan );
			writer.Write( m_Hold );
			writer.Write( m_BoatDoor );
			writer.Write( m_Anchored );
			writer.Write( m_ShipName );

			CheckDecay();
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:
				{
					m_MapItem = (MapItem) reader.ReadItem();
					m_NextNavPoint = reader.ReadInt();

					goto case 2;
				}
				case 2:
				{
					m_Facing = (Direction)reader.ReadInt();

					goto case 1;
				}
				case 1:
				{
					m_DecayTime = reader.ReadDeltaTime();

					goto case 0;
				}
				case 0:
				{
					if ( version < 3 )
						m_NextNavPoint = -1;

					if ( version < 2 )
					{
						if ( ItemID == NorthID )
							m_Facing = Direction.North;
						else if ( ItemID == SouthID )
							m_Facing = Direction.South;
						else if ( ItemID == EastID )
							m_Facing = Direction.East;
						else if ( ItemID == WestID )
							m_Facing = Direction.West;
					}

					m_Owner = reader.ReadMobile();
					m_PPlank = reader.ReadItem() as Plank;
					m_SPlank = reader.ReadItem() as Plank;
					m_TillerMan = reader.ReadItem() as TillerMan;
					m_Hold = reader.ReadItem() as Hold;
					m_BoatDoor = reader.ReadItem() as BoatDoor;
					m_Anchored = reader.ReadBool();
					m_ShipName = reader.ReadString();

					if ( version < 1)
						Refresh();

					break;
				}
			}

			m_Instances.Add( this );
		}

		public void RemoveKeys( Mobile m )
		{
			uint keyValue = 0;

			if ( m_PPlank != null )
				keyValue = m_PPlank.KeyValue;

			if ( keyValue == 0 && m_SPlank != null )
				keyValue = m_SPlank.KeyValue;

			Key.RemoveKeys( m, keyValue );
		}

		public uint CreateKeys( Mobile m )
		{
			uint value = Key.RandomValue();

			Key packKey = new Key( KeyType.Gold, value, this );
			Key bankKey = new Key( KeyType.Gold, value, this );

			packKey.MaxRange = 10;
			bankKey.MaxRange = 10;

			packKey.Name = "a ship key";
			bankKey.Name = "a ship key";

			if ( isCarpet( this ) )
			{
				packKey.Name = "a magic key";
				bankKey.Name = "a magic key";
				packKey.ItemID = 0x1012;
				bankKey.ItemID = 0x1012;
			}

			BankBox box = m.BankBox;

			if ( !box.TryDropItem( m, bankKey, false ) )
				bankKey.Delete();
			else
				m.LocalOverheadMessage( MessageType.Regular, 0x3B2, translateText( this, 502484 ) ); // A ship's key is now in my safety deposit box.

			if ( m.AddToBackpack( packKey ) )
				m.LocalOverheadMessage( MessageType.Regular, 0x3B2, translateText( this, 502485 ) ); // A ship's key is now in my backpack.
			else
				m.LocalOverheadMessage( MessageType.Regular, 0x3B2, translateText( this, 502483 ) ); // A ship's key is now at my feet.

			return value;
		}

		public override void OnAfterDelete()
		{
			if ( m_TillerMan != null )
				m_TillerMan.Delete();

			if ( m_Hold != null )
				m_Hold.Delete();

			if ( m_BoatDoor != null )
				m_BoatDoor.Delete();

			if ( m_PPlank != null )
				m_PPlank.Delete();

			if ( m_SPlank != null )
				m_SPlank.Delete();

			if ( m_TurnTimer != null )
				m_TurnTimer.Stop();

			if ( m_MoveTimer != null )
				m_MoveTimer.Stop();

			m_Instances.Remove( this );
		}

		public static SunkenShip CreateSunkenShip( BaseCreature from )
		{
			PlayerMobile killer = MobileUtilities.TryGetKillingPlayer( from );
			int level = ((int)(Server.Misc.IntelligentAction.GetCreatureLevel( from )/25)+1);
			if ( killer != null )
			{
				int mod = (int)( killer.Skills[SkillName.Seafaring].Value / 25 );
				if ( mod > 0 ){ level = level + mod; }
			}

			SunkenShip ShipWreck = new SunkenShip( level );

			Point3D splash = new Point3D( ( from.X-1 ), ( from.Y ), -5 );
				Effects.SendLocationEffect( splash, from.Map, 0x23B2, 16 );
			splash = new Point3D( ( from.X+1 ), ( from.Y ), -5 );
				Effects.SendLocationEffect( splash, from.Map, 0x23B2, 16 );
			splash = new Point3D( ( from.X ), ( from.Y-1 ), -5 );
				Effects.SendLocationEffect( splash, from.Map, 0x23B2, 16 );
			splash = new Point3D( ( from.X ), ( from.Y+1 ), -5 );
				Effects.SendLocationEffect( splash, from.Map, 0x23B2, 16 );
			splash = new Point3D( from.X, from.Y, -5);
				Effects.SendLocationEffect( splash, from.Map, 0x23B2, 16 );

			level = (int)(level/3); if ( level < 1 && Utility.RandomBool() ){ level = 1; }
			int cycle = Utility.RandomMinMax( 0, level );
			int relics = Utility.RandomMinMax( 0, level );

			string shipName = "";
			if ( from is BaseSailor ){ ShipWreck.Name = "sunken boat"; if ( cycle > 1 ){ cycle = 1; } } else { shipName = RandomThings.GetRandomShipName( "", 0 ); }

			while ( cycle > 0 )
			{
				cycle--;
				Cargo cargo = new Cargo();
				cargo.CargoKarma = -(int)(cargo.CargoValue/10);
				cargo.CargoShip = shipName;
					if ( from.Karma < 0 ){ cargo.CargoKarma = (int)(cargo.CargoValue/5); }
				ShipWreck.DropItem( cargo );
			}

			if ( Utility.RandomMinMax( 1, 4 ) == 1 ){ Item i1 = new RawFishSteak( Utility.RandomMinMax( 1, 8 ) ); 	ShipWreck.DropItem( i1 ); }
			if ( Utility.RandomMinMax( 1, 4 ) == 1 ){ Item i2 = new Fish( Utility.RandomMinMax( 1, 8 ) ); 			ShipWreck.DropItem( i2 ); }
			if ( Utility.RandomMinMax( 1, 4 ) == 1 ){ Item i3 = new FishingPole() { Resource = CraftResources.GetRandomNonBasicResource( CraftResourceType.Wood ) }; ShipWreck.DropItem( i3 ); }
			if ( Utility.RandomMinMax( 1, 4 ) == 1 ){ Item i4 = new NewFish(); 										ShipWreck.DropItem( i4 ); }
			if ( Utility.RandomMinMax( 1, 4 ) == 1 ){ Item i5 = new Sextant(); 										ShipWreck.DropItem( i5 ); }
			if ( Utility.RandomMinMax( 1, 10 ) == 1 )
			{
				HighSeasRelic goods = new HighSeasRelic();
				goods.CoinPrice = goods.CoinPrice + (int)(from.RawStatTotal/3);
				ShipWreck.DropItem(goods);
			}

			if (killer != null )
			{
				while ( relics > 0 )
				{
					relics--;
					Item relic = Loot.RandomRelic( killer );
					ShipWreck.DropItem( relic );
				}

				int killerLuck = MobileUtilities.GetLuckFromKiller( from );
				if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Utility.RandomMinMax( 1, 4 ) == 1 )
				{
					LootChest MyChest = new LootChest( level );
					MyChest.Name = "Chest Plundered from " + from.Name + " " + from.Title;
						if ( from.Karma > 0 ){ MyChest.Name = "Chest Seized from " + from.Name + " " + from.Title; }
					ShipWreck.DropItem( MyChest );
				}
				else if ( from is BasePirate && GetPlayerInfo.LuckyKiller( killerLuck ) && Utility.RandomBool() && killer.Karma > 0 && from.Karma < 0 )
				{
					PirateBounty bounty = new PirateBounty();
					bounty.BountyWho = from.Name + " "  + from.Title;
					ShipWreck.DropItem( bounty );
				}
			}

			ShipWreck.MoveToWorld( splash, from.Map );

			return ShipWreck;
		}

		public static void SinkShip( BaseBoat boat, Mobile captain )
		{
			if( boat != null )
			{
				ArrayList crew = new ArrayList();
				foreach ( Mobile sailors in World.Mobiles.Values )
				{
					if ( sailors.EmoteHue == boat.Serial && sailors != captain && ( sailors is BasePirate || sailors.Name == "a follower" || sailors.Name == "a sailor" || sailors.Name == "a pirate" ) )
					{
						crew.Add( sailors );
					}
				}
				for ( int i = 0; i < crew.Count; ++i )
				{
					Mobile sailors = ( Mobile )crew[ i ];
					sailors.Delete();
				}

				int splash = 30;
				int radius = 15;
					if ( boat is TinyBoat ){ splash = 6; radius = 2; }

				Effects.PlaySound( boat.Location, boat.Map, 0x026 );

				int x = 0;
				int y = 0;

				while ( splash > 0 )
				{
					x = boat.X + Utility.RandomMinMax( -radius, radius );
					y = boat.Y + Utility.RandomMinMax( -radius, radius );
					Point3D wave = new Point3D( x, y, boat.Z);

					if ( Utility.RandomBool() )
					{
						Effects.SendLocationEffect( wave, boat.Map, 0x23B2, 16 );
					}
					else
					{
						Effects.SendLocationEffect( wave, boat.Map, 0x352D, 16, 4 );
					}

					splash--;
				}

				boat.Delete();
            }
		}

		public static void BuildShip( BaseBoat boat, Mobile m )
		{
			if( boat != null )
			{
				Point3D loc = m.Location;
				loc = new Point3D( m.X, m.Y-1, -5 );
				m.Z = BoatDeckZ( boat );
				boat.MoveToWorld(loc, m.Map);
				m.EmoteHue = boat.Serial;
            }
		}

		public override void OnLocationChange( Point3D old )
		{
			if ( IsNPCBoat( this ) )
			{
				switch ( Utility.RandomMinMax( 1, 4 ) )
				{
					case 1: SetFacing ( Direction.North ); break;
					case 2: SetFacing ( Direction.East ); break;
					case 3: SetFacing ( Direction.South ); break;
					case 4: SetFacing ( Direction.West ); break;
				}
				if ( this is TinyBoat ){ Hue = 0x5BE; }

				PPlank.Visible = false;		PPlank.ItemID = 0x0F7A;
				SPlank.Visible = false;		SPlank.ItemID = 0x0F7A;
				TillerMan.Visible = false;	TillerMan.ItemID = 0x0F7A;
				Hold.Visible = false;		Hold.ItemID = 0x0F7A;
				BoatDoor.Visible = false;	BoatDoor.ItemID = 0x0F7A;		BoatDoor.Name = "enemy ship";	BoatDoor.Z = BoatDeckZ( this );
			}
			else
			{
				if ( m_TillerMan != null )
					m_TillerMan.Location = new Point3D( X + (m_TillerMan.X - old.X), Y + (m_TillerMan.Y - old.Y), Z + (m_TillerMan.Z - old.Z ) );

				if ( m_Hold != null )
					m_Hold.Location = new Point3D( X + (m_Hold.X - old.X), Y + (m_Hold.Y - old.Y), Z + (m_Hold.Z - old.Z ) );

				if ( m_BoatDoor != null )
					m_BoatDoor.Location = new Point3D( X + (m_BoatDoor.X - old.X), Y + (m_BoatDoor.Y - old.Y), Z + (m_BoatDoor.Z - old.Z ) );

				if ( m_PPlank != null )
					m_PPlank.Location = new Point3D( X + (m_PPlank.X - old.X), Y + (m_PPlank.Y - old.Y), Z + (m_PPlank.Z - old.Z ) );

				if ( m_SPlank != null )
					m_SPlank.Location = new Point3D( X + (m_SPlank.X - old.X), Y + (m_SPlank.Y - old.Y), Z + (m_SPlank.Z - old.Z ) );
			}
		}

		public static void ClearShip()
		{
			ArrayList targets = new ArrayList();
			foreach ( Item boat in World.Items.Values )
			{
				if ( IsNPCBoat( boat ) || boat is VesselsNS || boat is VesselsEW || boat is ShipNS || boat is ShipEW || boat is WreckNS || boat is WreckEW )
				{
					targets.Add( boat );
				}
			}
			for ( int i = 0; i < targets.Count; ++i )
			{
				Item ship = ( Item )targets[ i ];
				if ( ship is VesselsNS || ship is VesselsEW || ship is ShipNS || ship is ShipEW || ship is WreckNS || ship is WreckEW )
				{
					ship.Delete();
				}
				else
				{
					SinkShip( (BaseBoat)ship, null );
				}
			}

			bool keepMe = false;
			ArrayList crew = new ArrayList();
			foreach ( Mobile sailors in World.Mobiles.Values )
			{
				if ( sailors.EmoteHue > 10000 )
				{
					keepMe = false;
					foreach ( Item i in sailors.GetItemsInRange( 20 ) )
					{
						if ( i is BaseBoat && i.Serial == sailors.EmoteHue )
							keepMe = true;
					}

					if ( !keepMe )
						crew.Add( sailors );
				}
			}
			for ( int i = 0; i < crew.Count; ++i )
			{
				Mobile sailors = ( Mobile )crew[ i ];
				sailors.EmoteHue = 0;
				sailors.Delete();
			}
		}

		public static bool IsNPCBoat( Item boat )
		{
			if ( boat is TinyBoat || 
				boat is GalleonBarbarian || 
				boat is GalleonRoyal || 
				boat is GalleonExotic || 
				boat is GalleonLarge || 
				boat is GalleonWreckedBarbarian || 
				boat is GalleonWreckedRoyal || 
				boat is GalleonWreckedExotic || 
				boat is GalleonWreckedLarge || 
				boat is GalleonRuinedBarbarian || 
				boat is GalleonRuinedRoyal || 
				boat is GalleonRuinedExotic )
			{
				return true;
			}

			return false;
		}

		public static bool IsInPortTown( Mobile m )
		{
			Region reg = Region.Find( m.Location, m.Map );

			if ( Server.Misc.Worlds.IsSeaTown( m.Location, m.Map ) )
				return true;

			string sPublicDoor = "";

			sPublicDoor = ((PlayerMobile)m).CharacterPublicDoor;

			if ( sPublicDoor != null )
			{
				int mX = 0;
				int mY = 0;
				int mZ = 0;
				Map mWorld = null;
				string[] sPublicDoors = sPublicDoor.Split('#');
				int nEntry = 1;
				foreach (string exits in sPublicDoors)
				{
					if ( nEntry == 1 ){ mX = Convert.ToInt32(exits); }
					else if ( nEntry == 2 ){ mY = Convert.ToInt32(exits); }
					else if ( nEntry == 3 ){ mZ = Convert.ToInt32(exits); }
					else if ( nEntry == 4 ){ try { mWorld = Map.Parse( exits ); } catch{} if ( mWorld == null ){ mWorld = Map.Sosaria; } }
					nEntry++;
				}

				Point3D loc = new Point3D( mX, mY, mZ );
				reg = Region.Find( loc, mWorld );

				if ( Server.Misc.Worlds.IsSeaTown( m.Location, m.Map ) )
					return true;
			}

			return false;
		}

		public static int BoatDeckZ( Item boat )
		{
			int z = -2;
			if ( boat is GalleonBarbarian ){ z = 9; }
			else if ( boat is GalleonRoyal ){ z = 11; }
			else if ( boat is GalleonExotic ){ z = 9; }
			else if ( boat is GalleonLarge ){ z = 13; }
			else if ( boat is GalleonWreckedBarbarian ){ z = 9; }
			else if ( boat is GalleonWreckedRoyal ){ z = 11; }
			else if ( boat is GalleonWreckedExotic ){ z = 9; }
			else if ( boat is GalleonWreckedLarge ){ z = 13; }
			else if ( boat is GalleonRuinedBarbarian ){ z = 9; }
			else if ( boat is GalleonRuinedRoyal ){ z = 11; }
			else if ( boat is GalleonRuinedExotic ){ z = 9; }

			return z;
		}

		public static bool IsNearOtherShip( Mobile from )
		{
			int obstacle = 0;
			foreach ( Mobile m in from.GetMobilesInRange( 20 ) )
			{
				if ( m is BaseCreature && m != from && m.YellHue != from.YellHue && m.EmoteHue != m.EmoteHue )
				{
					BaseCreature bc = (BaseCreature)m;
					if ( bc.ControlMaster == null && !bc.CanSwim )
						++obstacle;
				}
			}

			if ( obstacle > 0 )
				return true;

			return false;
		}

		public static Point3D GetPirateShip( BaseCreature bc )
		{
			Point3D loc = new Point3D( 0, 0, 0 );

			if ( bc.ControlMaster == null && bc.EmoteHue > 0 )
			{
				foreach ( Item i in bc.GetItemsInRange( 10 ) )
				{
					if ( i is BoatDoor && i.Name == "enemy ship" )
					{
						loc = new Point3D( i.X, i.Y, i.Z );
					}
				}
			}
			return loc;
		}

		public override void OnMapChange()
		{
			if ( m_TillerMan != null )
				m_TillerMan.Map = Map;

			if ( m_Hold != null )
				m_Hold.Map = Map;

			if ( m_BoatDoor != null )
				m_BoatDoor.Map = Map;

			if ( m_PPlank != null )
				m_PPlank.Map = Map;

			if ( m_SPlank != null )
				m_SPlank.Map = Map;
		}

		public bool CanCommand( Mobile m )
		{
			return true;
		}

		public Point3D GetMarkedLocation()
		{
			Point3D p = new Point3D( X + MarkOffset.X, Y + MarkOffset.Y, Z + MarkOffset.Z );

			return Rotate( p, (int)m_Facing / 2 );
		}

		public bool CheckKey( uint keyValue )
		{
			if ( m_SPlank != null && m_SPlank.KeyValue == keyValue )
				return true;

			if ( m_PPlank != null && m_PPlank.KeyValue == keyValue )
				return true;

			return false;
		}

		/*
		 * Intervals:
		 *       drift forward
		 * fast | 0.25|   0.25
		 * slow | 0.50|   0.50
		 *
		 * Speed:
		 *       drift forward
		 * fast |  0x4|    0x4
		 * slow |  0x3|    0x3
		 *
		 * Tiles (per interval):
		 *       drift forward
		 * fast |    1|      1
		 * slow |    1|      1
		 *
		 * 'walking' in piloting mode has a 1s interval, speed 0x2
		 */

		// Legacy clients will continue to see the old-style movement, but speeds are adjusted
		private int m_SlowSpeedDelay = 500;
		private int m_FastSpeedDelay = 250;

		[CommandProperty( AccessLevel.GameMaster )]
		public int SlowSpeedDelay { get { return m_SlowSpeedDelay; } set { m_SlowSpeedDelay = Math.Max(0, value); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int FastSpeedDelay { get { return m_FastSpeedDelay; } set { m_FastSpeedDelay = Math.Max(0, value); } }

		private static bool NewBoatMovement = true;

		private static int SlowSpeed = 1;
		private static int FastSpeed = NewBoatMovement ? 1 : 3;

		private TimeSpan GetSlowInterval() { return TimeSpan.FromMilliseconds( NewBoatMovement ? SlowSpeedDelay : 750 ); }
		private TimeSpan GetFastInterval() { return TimeSpan.FromMilliseconds( NewBoatMovement ? FastSpeedDelay : 750 ); }
		private TimeSpan GetSlowDriftInterval() { return TimeSpan.FromMilliseconds( NewBoatMovement ? SlowSpeedDelay : 1500 ); }
		private TimeSpan GetFastDriftInterval() { return TimeSpan.FromMilliseconds( NewBoatMovement ? FastSpeedDelay : 750 ); }
		private static int SlowDriftSpeed = 1;
		private static int FastDriftSpeed = 1;

		private static Direction Forward = Direction.North;
		private static Direction ForwardLeft = Direction.Up;
		private static Direction ForwardRight = Direction.Right;
		private static Direction Backward = Direction.South;
		private static Direction BackwardLeft = Direction.Left;
		private static Direction BackwardRight = Direction.Down;
		private static Direction Left = Direction.West;
		private static Direction Right = Direction.East;
		private static Direction Port = Left;
		private static Direction Starboard = Right;

		private bool m_Decaying;

		public void Refresh()
		{
			m_DecayTime = DateTime.Now + BoatDecayDelay;

			if( m_TillerMan != null )
				m_TillerMan.InvalidateProperties();
		}

		private class DecayTimer : Timer
		{
			private BaseBoat m_Boat;
			private int m_Count;

			public DecayTimer( BaseBoat boat ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 5.0 ) )
			{
				m_Boat = boat;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				if ( m_Count == 5 )
				{
					m_Boat.Delete();
					Stop();
				}
				else
				{
					m_Boat.Location = new Point3D( m_Boat.X, m_Boat.Y, m_Boat.Z - 1 );

					if ( m_Boat.TillerMan != null )
						m_Boat.TillerMan.Say( BaseBoat.translateText( m_Boat, (1007168 + m_Count) ) );

					++m_Count;
				}
			}
		}

		public bool CheckDecay()
		{
			if ( m_Decaying )
				return true;

			if ( !IsMoving && DateTime.Now >= m_DecayTime )
			{
				new DecayTimer( this ).Start();

				m_Decaying = true;

				return true;
			}

			return false;
		}

		public bool LowerAnchor( bool message )
		{
			if ( CheckDecay() )
				return false;

			if ( m_Anchored )
			{
				if ( message && m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 501445 ) ); // Ar, the anchor was already dropped sir.

				return false;
			}

			StopMove( false );

			m_Anchored = true;

			if ( message && m_TillerMan != null )
				m_TillerMan.Say( BaseBoat.translateText( this, 501444 ) ); // Ar, anchor dropped sir.

			return true;
		}

		public bool RaiseAnchor( bool message )
		{
			if ( CheckDecay() )
				return false;

			if ( !m_Anchored )
			{
				if ( message && m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 501447 ) ); // Ar, the anchor has not been dropped sir.

				return false;
			}

			m_Anchored = false;

			if ( message && m_TillerMan != null )
				m_TillerMan.Say( BaseBoat.translateText( this, 501446 ) ); // Ar, anchor raised sir.

			return true;
		}

		public bool StartMove( Direction dir, bool fast )
		{
			if ( CheckDecay() )
				return false;

			bool drift = ( dir != Forward && dir != ForwardLeft && dir != ForwardRight );
			TimeSpan interval = (fast ? (drift ? GetFastDriftInterval() : GetFastInterval()) : (drift ? GetSlowDriftInterval() : GetSlowInterval()));
			int speed = (fast ? (drift ? FastDriftSpeed : FastSpeed) : (drift ? SlowDriftSpeed : SlowSpeed));
			int clientSpeed = fast ? 0x4 : 0x3;

			if ( StartMove( dir, speed, clientSpeed, interval, false, true ) )
			{
				if ( m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 501429 ) ); // Aye aye sir.

				return true;
			}

			return false;
		}

		public bool OneMove( Direction dir )
		{
			if ( CheckDecay() )
				return false;

			bool drift = ( dir != Forward );
			TimeSpan interval = drift ? GetFastDriftInterval() : GetFastInterval();
			int speed = drift ? FastDriftSpeed : FastSpeed;

			if ( StartMove( dir, speed, 0x1, interval, true, true ) )
			{
				if ( m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 501429 ) ); // Aye aye sir.

				return true;
			}

			return false;
		}

		public void BeginRename( Mobile from )
		{
			if ( CheckDecay() )
				return;

			if ( from.AccessLevel < AccessLevel.GameMaster && from != m_Owner )
			{
				if ( m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, ( Utility.Random( 1042876, 4 ) ) ) ); // Arr, don't do that! | Arr, leave me alone! | Arr, watch what thour'rt doing, matey! | Arr! Do that again and I�ll throw ye overhead!

				return;
			}

			if ( m_TillerMan != null )
				m_TillerMan.Say( BaseBoat.translateText( this, 502580 ) ); // What dost thou wish to name thy ship?

			from.Prompt = new RenameBoatPrompt( this );
		}

		public void EndRename( Mobile from, string newName )
		{
			if ( Deleted || CheckDecay() )
				return;

			if ( from.AccessLevel < AccessLevel.GameMaster && from != m_Owner )
			{
				if ( m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 1042880 ) ); // Arr! Only the owner of the ship may change its name!

				return;
			}
			else if ( !from.Alive )
			{
				if ( m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 502582 ) ); // You appear to be dead.

				return;
			}

			newName = newName.Trim();

			if ( newName.Length == 0 )
				newName = null;

			Rename( newName );
		}

		public enum DryDockResult{ Valid, Dead, NoKey, NotAnchored, Mobiles, Items, Hold, Decaying }

		public DryDockResult CheckDryDock( Mobile from )
		{
			if ( CheckDecay() )
				return DryDockResult.Decaying;

			if ( !from.Alive )
				return DryDockResult.Dead;

			Container pack = from.Backpack;
			Container bank = from.BankBox;
			if ( (m_SPlank == null || !Key.ContainsKey( pack, m_SPlank.KeyValue )) && (m_PPlank == null || ( !Key.ContainsKey( pack, m_PPlank.KeyValue ) && !Key.ContainsKey( bank, m_PPlank.KeyValue ) ) ) )
				return DryDockResult.NoKey;

			if ( !m_Anchored )
				return DryDockResult.NotAnchored;

			if ( m_Hold != null && m_Hold.Items.Count > 0 )
				return DryDockResult.Hold;

			Map map = Map;

			if ( map == null || map == Map.Internal )
				return DryDockResult.Items;

			List<IEntity> ents = GetMovingEntities();

			if ( ents.Count >= 1 )
				return ( ents[0] is Mobile ) ? DryDockResult.Mobiles : DryDockResult.Items;

			return DryDockResult.Valid;
		}

		public void BeginDryDock( Mobile from, int hue )
		{
			if ( CheckDecay() )
				return;

			DryDockResult result = CheckDryDock( from );

			if ( result == DryDockResult.Dead )
				from.SendLocalizedMessage( 502493 ); // You appear to be dead.
			else if ( result == DryDockResult.NoKey )
				from.SendLocalizedMessage( translateText( this, 502494 ) ); // You must have a key to the ship to dock the boat.
			else if ( result == DryDockResult.NotAnchored )
				from.SendLocalizedMessage( translateText( this, 1010570 ) ); // You must lower the anchor to dock the boat.
			else if ( result == DryDockResult.Mobiles )
				from.SendLocalizedMessage( translateText( this, 502495 ) ); // You cannot dock the ship with beings on board!
			else if ( result == DryDockResult.Items )
				from.SendLocalizedMessage( translateText( this, 502496 ) ); // You cannot dock the ship with a cluttered deck.
			else if ( result == DryDockResult.Hold )
				from.SendLocalizedMessage( translateText( this, 502497 ) ); // Make sure your hold is empty, and try again!
			else if ( result == DryDockResult.Valid )
				from.SendGump( new ConfirmDryDockGump( from, this, hue ) );
		}

		public void EndDryDock( Mobile from, int hue )
		{
			if ( Deleted || CheckDecay() )
				return;

			from.CloseGump( typeof( TillerManGump ) );

			DryDockResult result = CheckDryDock( from );

			if ( result == DryDockResult.Dead )
				from.SendLocalizedMessage( 502493 ); // You appear to be dead.
			else if ( result == DryDockResult.NoKey )
				from.SendLocalizedMessage( 502494 ); // You must have a key to the ship to dock the boat.
			else if ( result == DryDockResult.NotAnchored )
				from.SendLocalizedMessage( 1010570 ); // You must lower the anchor to dock the boat.
			else if ( result == DryDockResult.Mobiles )
				from.SendLocalizedMessage( 502495 ); // You cannot dock the ship with beings on board!
			else if ( result == DryDockResult.Items )
				from.SendLocalizedMessage( 502496 ); // You cannot dock the ship with a cluttered deck.
			else if ( result == DryDockResult.Hold )
				from.SendLocalizedMessage( 502497 ); // Make sure your hold is empty, and try again!

			if ( result != DryDockResult.Valid )
				return;

			BaseDockedBoat boat = DockedBoat;
			boat.Hue = hue;

			if ( boat == null )
				return;

			// Magic carpets don't have a Lower Deck
			if ( !BaseBoat.isCarpet( this ) )
			{
				foreach ( Mobile stow in World.Mobiles.Values )
				if ( stow is PlayerMobile && stow.Region.Name == "the Ship's Lower Deck" )
				{
					string sCabinDoor = ((PlayerMobile)from).CharacterBoatDoor;

					string sWorld = "";
					string sSerial = "";
					string sCode = "";

					if ( sCabinDoor != null )
					{
						string[] doors = sCabinDoor.Split('#');
						int nEntry = 1;
						foreach (string doorz in doors)
						{
							if ( nEntry == 1 ){ sSerial = doorz; }
							else if ( nEntry == 2 ){ sCode = doorz; }
							else if ( nEntry == 3 ){ sWorld = doorz; }

							nEntry++;
						}
					}

					if ( m_BoatDoor.Serial.ToString() == sSerial && m_BoatDoor.BoatCode == sCode )
					{
						DeckDoor.CabinDoor( stow, from.Location, from.Map );
					}
				}
			}

			RemoveKeys( from );

			from.AddToBackpack( boat );
			if ( BaseBoat.isCarpet( this ) ){ from.PlaySound( 0x1FD ); } else { from.PlaySound( 0x026 ); }
			Delete();
		}

		public void SetName( SpeechEventArgs e )
		{
			if ( CheckDecay() )
				return;

			if ( e.Mobile.AccessLevel < AccessLevel.GameMaster && e.Mobile != m_Owner )
			{
				if ( m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 1042880 ) ); // Arr! Only the owner of the ship may change its name!

				return;
			}
			else if ( !e.Mobile.Alive )
			{
				if ( m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 502582 ) ); // You appear to be dead.

				return;
			}

			if ( e.Speech.Length > 8 )
			{
				string newName = e.Speech.Substring( 8 ).Trim();

				if ( newName.Length == 0 )
					newName = null;

				Rename( newName );
			}
		}

		public void Rename( string newName )
		{
			if ( CheckDecay() )
				return;

			if ( newName != null && newName.Length > 40 )
				newName = newName.Substring( 0, 40 );

			if ( m_ShipName == newName )
			{
				if ( m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 502531 ) ); // Yes, sir.

				return;
			}

			ShipName = newName;

			if ( m_TillerMan != null && m_ShipName != null )
				m_TillerMan.Say( BaseBoat.translateText( this, 1042885 ), m_ShipName ); // This ship is now called the ~1_NEW_SHIP_NAME~.
			else if ( m_TillerMan != null )
				m_TillerMan.Say( BaseBoat.translateText( this, 502534 ) ); // This ship now has no name.
		}

		public void RemoveName( Mobile m )
		{
			if ( CheckDecay() )
				return;

			if ( m.AccessLevel < AccessLevel.GameMaster && m != m_Owner )
			{
				if ( m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 1042880 ) ); // Arr! Only the owner of the ship may change its name!

				return;
			}
			else if ( !m.Alive )
			{
				if ( m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 502582 ) ); // You appear to be dead.

				return;
			}

			if ( m_ShipName == null )
			{
				if ( m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 502526 ) ); // Ar, this ship has no name.

				return;
			}

			ShipName = null;

			if ( m_TillerMan != null )
				m_TillerMan.Say( BaseBoat.translateText( this, 502534 ) ); // This ship now has no name.
		}

		public void GiveName( Mobile m )
		{
			if ( m_TillerMan == null || CheckDecay() )
				return;

			if ( m_ShipName == null )
				m_TillerMan.Say( BaseBoat.translateText( this, 502526 ) ); // Ar, this ship has no name.
			else
				m_TillerMan.Say( BaseBoat.translateText( this, 1042881 ), m_ShipName ); // This is the ~1_BOAT_NAME~.
		}

		public void GiveNavPoint()
		{
			if ( TillerMan == null || CheckDecay() )
				return;

			if ( NextNavPoint < 0 )
				TillerMan.Say( BaseBoat.translateText( this, 1042882 ) ); // I have no current nav point.
			else
				TillerMan.Say( BaseBoat.translateText( this, 1042883 ), (NextNavPoint + 1).ToString() ); // My current destination navpoint is nav ~1_NAV_POINT_NUM~.
		}

		public void AssociateMap( Item paper )
		{
			if ( CheckDecay() )
				return;


			MapItem map = null;
			if ( paper is MapItem )
				map = (MapItem)paper;

			if ( paper is BlankScroll || paper is BlankMap )
			{
				if ( TillerMan != null )
					TillerMan.Say( BaseBoat.translateText( this, 502575 ) ); // Ar, that is not a map, tis but a blank piece of paper!
			}
			else if ( map != null && map.Pins.Count == 0 )
			{
				if ( TillerMan != null )
					TillerMan.Say( BaseBoat.translateText( this, 502576 ) ); // Arrrr, this map has no course on it!
			}
			else if ( map != null )
			{
				StopMove( false );

				MapItem = map;
				NextNavPoint = -1;

				if ( TillerMan != null )
					TillerMan.Say( BaseBoat.translateText( this, 502577 ) ); // A map!
			}
		}

		public bool StartCourse( string navPoint, bool single, bool message )
		{
			int number = -1;

			int start = -1;
			for ( int i = 0; i < navPoint.Length; i++ )
			{
				if ( Char.IsDigit( navPoint[i] ) )
				{
					start = i;
					break;
				}
			}

			if ( start != -1 )
			{
				string sNumber = navPoint.Substring( start );

                if ( !int.TryParse( sNumber, out number ) )
                    number = -1;

				if ( number != -1 )
				{
					number--;

					if ( MapItem == null || number < 0 || number >= MapItem.Pins.Count )
					{
						number = -1;
					}
				}
			}

			if ( number == -1 )
			{
				if ( message && TillerMan != null )
					TillerMan.Say( BaseBoat.translateText( this, 1042551 ) ); // I don't see that navpoint, sir.

				return false;
			}

			NextNavPoint = number;
			return StartCourse( single, message );
		}

		public bool StartCourse( bool single, bool message )
		{
			if ( CheckDecay() )
				return false;

			if ( Anchored )
			{
				if ( message && TillerMan != null )
					TillerMan.Say( BaseBoat.translateText( this, 501419 ) ); // Ar, the anchor is down sir!

				return false;
			}
			else if ( MapItem == null || MapItem.Deleted )
			{
				if ( message && TillerMan != null )
					TillerMan.Say( BaseBoat.translateText( this, 502513 ) ); // I have seen no map, sir.

				return false;
			}
			else if ( this.Map != MapItem.Map || !this.Contains( MapItem.GetWorldLocation() ) )
			{
				if ( message && TillerMan != null )
					TillerMan.Say( BaseBoat.translateText( this, 502514 ) ); // The map is too far away from me, sir.

				return false;
			}
			else if ( NextNavPoint < 0 || NextNavPoint >= MapItem.Pins.Count )
			{
				if ( message && TillerMan != null )
					TillerMan.Say( BaseBoat.translateText( this, 1042551 ) ); // I don't see that navpoint, sir.

				return false;
			}

			Speed = FastSpeed;
			Order = single ? BoatOrder.Single : BoatOrder.Course;

			if ( m_MoveTimer != null )
				m_MoveTimer.Stop();

			m_MoveTimer = new MoveTimer( this, GetFastInterval(), false );
			m_MoveTimer.Start();

			if ( message && TillerMan != null )
				TillerMan.Say( BaseBoat.translateText( this, 501429 ) ); // Aye aye sir.

			return true;
		}

		public override bool HandlesOnSpeech{ get{ return true; } }

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( CheckDecay() )
				return;

			Mobile from = e.Mobile;

			bool handled = false;
			if ( CanCommand( from ) && Contains( from ) )
			{
				for ( int i = 0; i < e.Keywords.Length; ++i )
				{
					int keyword = e.Keywords[i];

					if ( keyword >= 0x42 && keyword <= 0x6B )
					{
						switch ( keyword )
						{
							case 0x42: SetName( e ); break;
							case 0x43: RemoveName( e.Mobile ); break;
							case 0x44: GiveName( e.Mobile ); break;
							case 0x45: StartMove( Forward, true ); break;
							case 0x46: StartMove( Backward, true ); break;
							case 0x47: StartMove( Left, true ); break;
							case 0x48: StartMove( Right, true ); break;
							case 0x4B: StartMove( ForwardLeft, true ); break;
							case 0x4C: StartMove( ForwardRight, true ); break;
							case 0x4D: StartMove( BackwardLeft, true ); break;
							case 0x4E: StartMove( BackwardRight, true ); break;
							case 0x4F: StopMove( true ); break;
							case 0x50: StartMove( Left, false ); break;
							case 0x51: StartMove( Right, false ); break;
							case 0x52: StartMove( Forward, false ); break;
							case 0x53: StartMove( Backward, false ); break;
							case 0x54: StartMove( ForwardLeft, false ); break;
							case 0x55: StartMove( ForwardRight, false ); break;
							case 0x56: StartMove( BackwardRight, false ); break;
							case 0x57: StartMove( BackwardLeft, false ); break;
							case 0x58: OneMove( Left ); break;
							case 0x59: OneMove( Right ); break;
							case 0x5A: OneMove( Forward ); break;
							case 0x5B: OneMove( Backward ); break;
							case 0x5C: OneMove( ForwardLeft ); break;
							case 0x5D: OneMove( ForwardRight ); break;
							case 0x5E: OneMove( BackwardRight ); break;
							case 0x5F: OneMove( BackwardLeft ); break;
							case 0x49: case 0x65: StartTurn(  2, true ); break; // turn right
							case 0x4A: case 0x66: StartTurn( -2, true ); break; // turn left
							case 0x67: StartTurn( -4, true ); break; // turn around, come about
							case 0x68: StartMove( Forward, true ); break;
							case 0x69: StopMove( true ); break;
							case 0x6A: LowerAnchor( true ); break;
							case 0x6B: RaiseAnchor( true ); break;
							case 0x60: GiveNavPoint(); break; // nav
							case 0x61: NextNavPoint = 0; StartCourse( false, true ); break; // start
							case 0x62: StartCourse( false, true ); break; // continue
							case 0x63: StartCourse( e.Speech, false, true ); break; // goto*
							case 0x64: StartCourse( e.Speech, true, true ); break; // single*
						}

						handled = true;
						break;
					}
				}

				if (!handled && Insensitive.Contains(e.Speech, CLEAR_THE_DECK))
				{
					bool foundItem = false;
					foreach(var entity in GetMovingEntities())
					{
						if (entity is Mobile)
						{
							var mobile = (Mobile)entity;
							if (mobile.Hidden) mobile.Hidden = false;
						}

						if (entity is Item)
						{
							if (entity is Corpse)
							{
								entity.Delete();
							}
							else
							{
								foundItem = true;
							}
						}
					}

					if (foundItem)
					{
						m_TillerMan.Say("That item might be precious sir!");
					}

					handled = true;
				}
			}

			if (handled) return;

			if (from is PlayerMobile && Utility.Random( 4 ) == 1 && Contains(e.Mobile))
			{
				m_TillerMan.Say(string.Format("I can always '{0}' sir!", CLEAR_THE_DECK));
			}
		}

		public bool StartTurn( int offset, bool message )
		{
			if ( CheckDecay() )
				return false;

			if ( m_Anchored )
			{
				if ( message )
					m_TillerMan.Say( BaseBoat.translateText( this, 501419 ) ); // Ar, the anchor is down sir!

				return false;
			}
			else
			{
				if ( m_MoveTimer != null && this.Order != BoatOrder.Move )
				{
					m_MoveTimer.Stop();
					m_MoveTimer = null;
				}

				if ( m_TurnTimer != null )
					m_TurnTimer.Stop();

				m_TurnTimer = new TurnTimer( this, offset );
				m_TurnTimer.Start();

				if ( message && TillerMan != null )
					TillerMan.Say( BaseBoat.translateText( this, 501429 ) ); // Aye aye sir.

				return true;
			}
		}

		public bool Turn( int offset, bool message )
		{
			if ( m_TurnTimer != null )
			{
				m_TurnTimer.Stop();
				m_TurnTimer = null;
			}

			if ( CheckDecay() )
				return false;

			if ( m_Anchored )
			{
				if ( message )
					m_TillerMan.Say( BaseBoat.translateText( this, 501419 ) ); // Ar, the anchor is down sir!

				return false;
			}
			else if ( SetFacing( (Direction)(((int)m_Facing + offset) & 0x7) ) )
			{
				return true;
			}
			else
			{
				if ( message )
					m_TillerMan.Say( BaseBoat.translateText( this, 501423 ) ); // Ar, can't turn sir.

				return false;
			}
		}

		private class TurnTimer : Timer
		{
			private BaseBoat m_Boat;
			private int m_Offset;

			public TurnTimer( BaseBoat boat, int offset ) : base( TimeSpan.FromSeconds( 0.5 ) )
			{
				m_Boat = boat;
				m_Offset = offset;

				Priority = TimerPriority.TenMS;
			}

			protected override void OnTick()
			{
				if ( !m_Boat.Deleted )
					m_Boat.Turn( m_Offset, true );
			}
		}

		public bool StartMove( Direction dir, int speed, int clientSpeed, TimeSpan interval, bool single, bool message )
		{
			if ( CheckDecay() )
				return false;

			if ( m_Anchored )
			{
				if ( message && m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 501419 ) ); // Ar, the anchor is down sir!

				return false;
			}

			m_Moving = dir;
			m_Speed = speed;
			m_ClientSpeed = clientSpeed;
			m_Order = BoatOrder.Move;

			if ( m_MoveTimer != null )
				m_MoveTimer.Stop();

			m_MoveTimer = new MoveTimer( this, interval, single );
			m_MoveTimer.Start();

			return true;
		}

		public bool StopMove( bool message )
		{
			if ( CheckDecay() )
				return false;

			if ( m_MoveTimer == null )
			{
				if ( message && m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 501443 ) ); // Er, the ship is not moving sir.

				return false;
			}

			m_Moving = Direction.North;
			m_Speed = 0;
			m_ClientSpeed = 0;
			m_MoveTimer.Stop();
			m_MoveTimer = null;

			if ( message && m_TillerMan != null )
				m_TillerMan.Say( BaseBoat.translateText( this, 501429 ) ); // Aye aye sir.

			return true;
		}

		public bool CanFit( Point3D p, Map map, int itemID )
		{
			if ( map == null || map == Map.Internal || Deleted || CheckDecay() )
				return false;

			MultiComponentList newComponents = MultiData.GetComponents( itemID );

			for ( int x = 0; x < newComponents.Width; ++x )
			{
				for ( int y = 0; y < newComponents.Height; ++y )
				{
					int tx = p.X + newComponents.Min.X + x;
					int ty = p.Y + newComponents.Min.Y + y;

					if ( newComponents.Tiles[x][y].Length == 0 || Contains( tx, ty ) )
						continue;

					LandTile landTile = map.Tiles.GetLandTile( tx, ty );
					StaticTile[] tiles = map.Tiles.GetStaticTiles( tx, ty, true );

					bool hasWater = false;

					if ( landTile.Z == p.Z && Server.Misc.Worlds.IsWaterTile( landTile.ID, 0 ) )
						hasWater = true;

					for ( int i = 0; i < tiles.Length; ++i )
					{
						StaticTile tile = tiles[i];

						if ( tile.Z == p.Z && Server.Misc.Worlds.IsWaterTile( tile.ID, 0 ) )
							hasWater = true;
						else if ( tile.Z >= p.Z && !Server.Misc.Worlds.IsWaterTile( tile.ID, 0 ) )
							return false;
					}

					if ( !hasWater )
						return false;
				}
			}

			IPooledEnumerable eable = map.GetItemsInBounds( new Rectangle2D( p.X + newComponents.Min.X, p.Y + newComponents.Min.Y, newComponents.Width, newComponents.Height ) );

			foreach ( Item item in eable )
			{
				if ( item is BaseMulti || item.ItemID > TileData.MaxItemValue || item.Z < p.Z || !item.Visible || item is Corpse )
					continue;

				int x = item.X - p.X + newComponents.Min.X;
				int y = item.Y - p.Y + newComponents.Min.Y;

				if ( x >= 0 && x < newComponents.Width && y >= 0 && y < newComponents.Height && newComponents.Tiles[x][y].Length == 0 )
					continue;
				else if ( Contains( item ) )
					continue;

				eable.Free();
				return false;
			}

			eable.Free();

			return true;
		}

		public Point3D Rotate( Point3D p, int count )
		{
			int rx = p.X - Location.X;
			int ry = p.Y - Location.Y;

			for ( int i = 0; i < count; ++i )
			{
				int temp = rx;
				rx = -ry;
				ry = temp;
			}

			return new Point3D( Location.X + rx, Location.Y + ry, p.Z );
		}

		public override bool Contains( int x, int y )
		{
			if ( base.Contains( x, y ) )
				return true;

			if ( m_TillerMan != null && x == m_TillerMan.X && y == m_TillerMan.Y )
				return true;

			if ( m_Hold != null && x == m_Hold.X && y == m_Hold.Y )
				return true;

			if ( m_BoatDoor != null && x == m_BoatDoor.X && y == m_BoatDoor.Y )
				return true;

			if ( m_PPlank != null && x == m_PPlank.X && y == m_PPlank.Y )
				return true;

			if ( m_SPlank != null && x == m_SPlank.X && y == m_SPlank.Y )
				return true;

			return false;
		}

		public static bool IsValidLocation( Point3D p, Map map )
		{
			Rectangle2D[] wrap = GetWrapFor( map, p, p.X, p.Y );

			for ( int i = 0; i < wrap.Length; ++i )
			{
				if ( wrap[i].Contains( p ) )
					return true;
			}

			return false;
		}

		public static Rectangle2D[] GetWrapFor( Map m, Point3D location, int x, int y )
		{
			Land land = Server.Lands.GetLand( m, location, x, y );

			if ( land == Land.Lodoria )
				return m_FelWrap;
			else if ( land == Land.Serpent )
				return m_SerpentIslandWrap;
			else if ( land == Land.Ambrosia )
				return m_AmbrosiaWrap;
			else if ( land == Land.IslesDread )
				return m_IslesDreadWrap;
			else if ( land == Land.Kuldar )
				return m_BottleWrap;
			else if ( land == Land.UmberVeil )
				return m_UmberWrap;
			else
				return m_TramWrap;
		}

		public Direction GetMovementFor( int x, int y, out int maxSpeed )
		{
			int dx = x - this.X;
			int dy = y - this.Y;

			int adx = Math.Abs( dx );
			int ady = Math.Abs( dy );

			Direction dir = Utility.GetDirection( this, new Point2D( x, y ) );
			int iDir = (int) dir;

			// Compute the maximum distance we can travel without going too far away
			if ( iDir % 2 == 0 ) // North, East, South and West
				maxSpeed = Math.Abs( adx - ady );
			else // Right, Down, Left and Up
				maxSpeed = Math.Min( adx, ady );

			return (Direction) ((iDir - (int)Facing) & 0x7);
		}

		public bool DoMovement( bool message )
		{
			Direction dir;
			int speed, clientSpeed;

			if ( this.Order == BoatOrder.Move )
			{
				dir = m_Moving;
				speed = m_Speed;
				clientSpeed = m_ClientSpeed;
			}
			else if ( MapItem == null || MapItem.Deleted )
			{
				if ( message && TillerMan != null )
					TillerMan.Say( BaseBoat.translateText( this, 502513 ) ); // I have seen no map, sir.

				return false;
			}
			else if ( this.Map != MapItem.Map || !this.Contains( MapItem.GetWorldLocation() ) )
			{
				if ( message && TillerMan != null )
					TillerMan.Say( BaseBoat.translateText( this, 502514 ) ); // The map is too far away from me, sir.

				return false;
			}
			else if ( NextNavPoint < 0 || NextNavPoint >= MapItem.Pins.Count )
			{
				if ( message && TillerMan != null )
					TillerMan.Say( BaseBoat.translateText( this, 1042551 ) ); // I don't see that navpoint, sir.

				return false;
			}
			else
			{
				Point2D dest = (Point2D) MapItem.Pins[NextNavPoint];

				int x, y;
				MapItem.ConvertToWorld( dest.X, dest.Y, out x, out y );

				int maxSpeed;
				dir = GetMovementFor( x, y, out maxSpeed );

				if ( maxSpeed == 0 )
				{
					if ( message && this.Order == BoatOrder.Single && TillerMan != null )
						TillerMan.Say( BaseBoat.translateText( this, 1042874 ), (NextNavPoint + 1).ToString() ); // We have arrived at nav point ~1_POINT_NUM~ , sir.

					if ( NextNavPoint + 1 < MapItem.Pins.Count )
					{
						NextNavPoint++;

						if ( this.Order == BoatOrder.Course )
						{
							if ( message && TillerMan != null )
								TillerMan.Say( BaseBoat.translateText( this, 1042875 ), (NextNavPoint + 1).ToString() ); // Heading to nav point ~1_POINT_NUM~, sir.

							return true;
						}

						return false;
					}
					else
					{
						NextNavPoint = -1;

						if ( message && this.Order == BoatOrder.Course && TillerMan != null )
							TillerMan.Say( BaseBoat.translateText( this, 502515 ) ); // The course is completed, sir.

						return false;
					}
				}

				if ( dir == Left || dir == BackwardLeft || dir == Backward )
					return Turn( -2, true );
				else if ( dir == Right || dir == BackwardRight )
					return Turn( 2, true );

				speed = Math.Min( this.Speed, maxSpeed );
				clientSpeed = 0x4;
			}

			return Move( dir, speed, clientSpeed, true );
		}

		public bool Move( Direction dir, int speed, int clientSpeed, bool message )
		{
			Map map = Map;

			if ( map == null || Deleted || CheckDecay() )
				return false;

			if ( m_Anchored )
			{
				if ( message && m_TillerMan != null )
					m_TillerMan.Say( BaseBoat.translateText( this, 501419 ) ); // Ar, the anchor is down sir!

				return false;
			}

			int rx = 0, ry = 0;
			Direction d = (Direction)(((int)m_Facing + (int)dir) & 0x7);
			Movement.Movement.Offset( d, ref rx, ref ry );

			for ( int i = 1; i <= speed; ++i )
			{
				if ( !CanFit( new Point3D( X + (i * rx), Y + (i * ry), Z ), Map, ItemID ) )
				{
					if ( i == 1 )
					{
						if ( message && m_TillerMan != null )
							m_TillerMan.Say( BaseBoat.translateText( this, 501424 ) ); // Ar, we've stopped sir.

						return false;
					}

					speed = i - 1;
					break;
				}
			}

			int xOffset = speed*rx;
			int yOffset = speed*ry;

			int newX = X + xOffset;
			int newY = Y + yOffset;

			Rectangle2D[] wrap = GetWrapFor( map, Location, X, Y );

			for ( int i = 0; i < wrap.Length; ++i )
			{
				Rectangle2D rect = wrap[i];

				if ( rect.Contains( new Point2D( X, Y ) ) && !rect.Contains( new Point2D( newX, newY ) ) )
				{
					if ( newX < rect.X )
						newX = rect.X + rect.Width - 1;
					else if ( newX >= rect.X + rect.Width )
						newX = rect.X;

					if ( newY < rect.Y )
						newY = rect.Y + rect.Height - 1;
					else if ( newY >= rect.Y + rect.Height )
						newY = rect.Y;

					for ( int j = 1; j <= speed; ++j )
					{
						if ( !CanFit( new Point3D( newX + (j * rx), newY + (j * ry), Z ), Map, ItemID ) )
						{
							if ( message && m_TillerMan != null )
								m_TillerMan.Say( BaseBoat.translateText( this, 501424 ) ); // Ar, we've stopped sir.

							return false;
						}
					}

					xOffset = newX - X;
					yOffset = newY - Y;
				}
			}

			if ( !NewBoatMovement || Math.Abs( xOffset ) > 1 || Math.Abs( yOffset ) > 1 )
			{
				Teleport( xOffset, yOffset, 0 );
			}
			else
			{
				List<IEntity> toMove = GetMovingEntities();

				SafeAdd( m_TillerMan, toMove );
				SafeAdd( m_Hold, toMove );
				SafeAdd( m_PPlank, toMove );
				SafeAdd( m_SPlank, toMove );

				// Packet must be sent before actual locations are changed
				foreach ( NetState ns in Map.GetClientsInRange( Location, GetMaxUpdateRange() ) )
				{
					Mobile m = ns.Mobile;

					if ( ns.HighSeas && m.CanSee( this ) && m.InRange( Location, GetUpdateRange( m ) ) )
						ns.Send( new MoveBoatHS( m, this, d, clientSpeed, toMove, xOffset, yOffset ) );
				}

				foreach ( IEntity e in toMove )
				{
					if ( e is Item )
					{
						Item item = (Item)e;

						item.NoMoveHS = true;

						if ( item is Corpse ) // This makes it look like it changes it's location because the boat moves
							item.Location = new Point3D( item.X, item.Y, item.Z );
						else if ( !( item is TillerMan || item is Hold || item is Plank ) )
							item.Location = new Point3D( item.X + xOffset, item.Y + yOffset, item.Z );
					}
					else if ( e is Mobile )
					{
						Mobile m = (Mobile)e;

						m.NoMoveHS = true;
						m.Location = new Point3D( m.X + xOffset, m.Y + yOffset, m.Z );
					}
				}

				NoMoveHS = true;
				Location = new Point3D( X + xOffset, Y + yOffset, Z );

				foreach ( IEntity e in toMove )
				{
					if ( e is Item )
						((Item)e).NoMoveHS = false;
					else if ( e is Mobile )
						((Mobile)e).NoMoveHS = false;
				}

				NoMoveHS = false;
			}

			return true;
		}

		private static void SafeAdd( Item item, List<IEntity> toMove )
		{
			if ( item != null )
				toMove.Add( item );
		}

		public void Teleport( int xOffset, int yOffset, int zOffset )
		{
			List<IEntity> toMove = GetMovingEntities();

			for ( int i = 0; i < toMove.Count; ++i )
			{
				IEntity e = toMove[i];

				if ( e is Item )
				{
					Item item = (Item)e;

					item.Location = new Point3D( item.X + xOffset, item.Y + yOffset, item.Z + zOffset );
				}
				else if ( e is Mobile )
				{
					Mobile m = (Mobile)e;

					m.Location = new Point3D( m.X + xOffset, m.Y + yOffset, m.Z + zOffset );
				}
			}

			Location = new Point3D( X + xOffset, Y + yOffset, Z + zOffset );
		}

		public List<IEntity> GetMovingEntities()
		{
			List<IEntity> list = new List<IEntity>();

			Map map = Map;

			if ( map == null || map == Map.Internal )
				return list;

			MultiComponentList mcl = Components;

			foreach ( object o in map.GetObjectsInBounds( new Rectangle2D( X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height ) ) )
			{
				if ( o == this || o is TillerMan || o is Hold || o is Plank || o is BoatDoor )
					continue;

				if ( o is Item )
				{
					Item item = (Item)o;
					if ( !item.Movable && false == (item is Corpse) )
						continue;

					if ( Contains( item ) && item.Visible && item.Z >= Z )
						list.Add( item );
				}
				else if ( o is Mobile )
				{
					Mobile m = (Mobile)o;

					if ( Contains( m ) )
						list.Add( m );
				}
			}

			return list;
		}

		public bool SetFacing( Direction facing )
		{
			if ( Parent != null || this.Map == null )
				return false;

			if ( CheckDecay() )
				return false;

			if ( Map != Map.Internal )
			{
				switch ( facing )
				{
					case Direction.North: if ( !CanFit( Location, Map, NorthID ) ) return false; break;
					case Direction.East:  if ( !CanFit( Location, Map,  EastID ) ) return false; break;
					case Direction.South: if ( !CanFit( Location, Map, SouthID ) ) return false; break;
					case Direction.West:  if ( !CanFit( Location, Map,  WestID ) ) return false; break;
				}
			}

			Direction old = m_Facing;

			m_Facing = facing;

			if ( m_TillerMan != null )
				m_TillerMan.SetFacing( facing );

			if ( m_Hold != null )
				m_Hold.SetFacing( facing );

			if ( m_BoatDoor != null )
				m_BoatDoor.SetFacing( facing );

			if ( m_PPlank != null )
				m_PPlank.SetFacing( facing );

			if ( m_SPlank != null )
				m_SPlank.SetFacing( facing );

			List<IEntity> toMove = GetMovingEntities();

			toMove.Add( m_PPlank );
			toMove.Add( m_SPlank );

			int xOffset = 0, yOffset = 0;
			Movement.Movement.Offset( facing, ref xOffset, ref yOffset );

			if ( m_TillerMan != null )
				m_TillerMan.Location = new Point3D( X + (xOffset * TillerManDistance) + (facing == Direction.North ? 1 : 0), Y + (yOffset * TillerManDistance), m_TillerMan.Z );

			if ( m_Hold != null )
				m_Hold.Location = new Point3D( X + (xOffset * HoldDistance), Y + (yOffset * HoldDistance), m_Hold.Z );

			if ( m_BoatDoor != null )
				m_BoatDoor.Location = new Point3D( X + (xOffset * BoatDoorDistance), Y + (yOffset * BoatDoorDistance), m_BoatDoor.Z );

			int count = (int)(m_Facing - old) & 0x7;
			count /= 2;

			for ( int i = 0; i < toMove.Count; ++i )
			{
				IEntity e = toMove[i];

				if ( e is Item )
				{
					Item item = (Item)e;

					item.Location = Rotate( item.Location, count );
				}
				else if ( e is Mobile )
				{
					Mobile m = (Mobile)e;

					m.Direction = ( m.Direction - old + facing ) & Direction.Mask;
					m.Location = Rotate( m.Location, count );
				}
			}

			switch ( facing )
			{
				case Direction.North: ItemID = NorthID; break;
				case Direction.East:  ItemID =  EastID; break;
				case Direction.South: ItemID = SouthID; break;
				case Direction.West:  ItemID =  WestID; break;
			}

			return true;
		}

		private class MoveTimer : Timer
		{
			private BaseBoat m_Boat;

			public MoveTimer( BaseBoat boat, TimeSpan interval, bool single ) : base( interval, interval, single ? 1 : 0 )
			{
				m_Boat = boat;
				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				if ( !m_Boat.DoMovement( true ) )
					m_Boat.StopMove( false );
			}
		}

		public static void UpdateAllComponents()
		{
			for ( int i = m_Instances.Count - 1; i >= 0; --i )
				m_Instances[i].UpdateComponents();
		}

		public static void Initialize()
		{
			new UpdateAllTimer().Start();
			EventSink.WorldSave += new WorldSaveEventHandler( EventSink_WorldSave );
		}

		private static void EventSink_WorldSave( WorldSaveEventArgs e )
		{
			new UpdateAllTimer().Start();
		}

		public class UpdateAllTimer : Timer
		{
			public UpdateAllTimer() : base( TimeSpan.FromSeconds( 1.0 ) )
			{
			}

			protected override void OnTick()
			{
				UpdateAllComponents();
			}
		}

		#region High Seas

		/*
		 * OSI sends the 0xF7 packet instead, holding 0xF3 packets
		 * for every entity on the boat. Though, the regular 0xF3
		 * packets are still being sent as well as entities come
		 * into sight. Do we really need it?
		 */
		/*
		protected override Packet GetWorldPacketFor( NetState state )
		{
			if ( NewBoatMovement && state.HighSeas )
				return new DisplayBoatHS( state.Mobile, this );
			else
				return base.GetWorldPacketFor( state );
		}
		*/

		public sealed class MoveBoatHS : Packet
		{
			public MoveBoatHS( Mobile beholder, BaseBoat boat, Direction d, int speed, List<IEntity> ents, int xOffset, int yOffset )
				: base( 0xF6 )
			{
				EnsureCapacity( 3 + 15 + ents.Count * 10 );

				m_Stream.Write( (int) boat.Serial );
				m_Stream.Write( (byte) speed );
				m_Stream.Write( (byte) d );
				m_Stream.Write( (byte) boat.Facing );
				m_Stream.Write( (short) ( boat.X + xOffset ) );
				m_Stream.Write( (short) ( boat.Y + yOffset ) );
				m_Stream.Write( (short) boat.Z );
				m_Stream.Write( (short) 0 ); // count placeholder

				int count = 0;

				foreach ( IEntity ent in ents )
				{
					if ( !beholder.CanSee( ent ) )
						continue;

					m_Stream.Write( (int) ent.Serial );
					m_Stream.Write( (short) ( ent.X + xOffset ) );
					m_Stream.Write( (short) ( ent.Y + yOffset ) );
					m_Stream.Write( (short) ent.Z );
					++count;
				}

				m_Stream.Seek( 16, System.IO.SeekOrigin.Begin );
				m_Stream.Write( (short) count );
			}
		}

		public sealed class DisplayBoatHS : Packet
		{
			public DisplayBoatHS( Mobile beholder, BaseBoat boat )
				: base( 0xF7 )
			{
				List<IEntity> ents = boat.GetMovingEntities();

				SafeAdd( boat.TillerMan, ents );
				SafeAdd( boat.Hold, ents );
				SafeAdd( boat.PPlank, ents );
				SafeAdd( boat.SPlank, ents );

				ents.Add( boat );

				EnsureCapacity( 3 + 2 + ents.Count * 26 );

				m_Stream.Write( (short) 0 ); // count placeholder

				int count = 0;

				foreach ( IEntity ent in ents )
				{
					if ( !beholder.CanSee( ent ) )
						continue;

					// Embedded WorldItemHS packets
					m_Stream.Write( (byte) 0xF3 );
					m_Stream.Write( (short) 0x1 );

					if ( ent is BaseMulti )
					{
						BaseMulti bm = (BaseMulti)ent;

						m_Stream.Write( (byte) 0x02 );
						m_Stream.Write( (int) bm.Serial );
						// TODO: Mask no longer needed, merge with Item case?
						m_Stream.Write( (ushort) ( bm.ItemID & 0x3FFF ) );
						m_Stream.Write( (byte) 0 );

						m_Stream.Write( (short) bm.Amount );
						m_Stream.Write( (short) bm.Amount );

						m_Stream.Write( (short) ( bm.X & 0x7FFF ) );
						m_Stream.Write( (short) ( bm.Y & 0x3FFF ) );
						m_Stream.Write( (sbyte) bm.Z );

						m_Stream.Write( (byte) bm.Light );
						m_Stream.Write( (short) bm.Hue );
						m_Stream.Write( (byte) bm.GetPacketFlags() );
					}
					else if ( ent is Mobile )
					{
						Mobile m = (Mobile)ent;

						m_Stream.Write( (byte) 0x01 );
						m_Stream.Write( (int) m.Serial );
						m_Stream.Write( (short) m.Body );
						m_Stream.Write( (byte) 0 );

						m_Stream.Write( (short) 1 );
						m_Stream.Write( (short) 1 );

						m_Stream.Write( (short) ( m.X & 0x7FFF ) );
						m_Stream.Write( (short) ( m.Y & 0x3FFF ) );
						m_Stream.Write( (sbyte) m.Z );

						m_Stream.Write( (byte) m.Direction );
						m_Stream.Write( (short) m.Hue );
						m_Stream.Write( (byte) m.GetPacketFlags() );
					}
					else if ( ent is Item )
					{
						Item item = (Item)ent;

						m_Stream.Write( (byte) 0x00 );
						m_Stream.Write( (int) item.Serial );
						m_Stream.Write( (ushort) ( item.ItemID & 0xFFFF ) );
						m_Stream.Write( (byte) 0 );

						m_Stream.Write( (short) item.Amount );
						m_Stream.Write( (short) item.Amount );

						m_Stream.Write( (short) ( item.X & 0x7FFF ) );
						m_Stream.Write( (short) ( item.Y & 0x3FFF ) );
						m_Stream.Write( (sbyte) item.Z );

						m_Stream.Write( (byte) item.Light );
						m_Stream.Write( (short) item.Hue );
						m_Stream.Write( (byte) item.GetPacketFlags() );
					}

					m_Stream.Write( (short) 0x00 );
					++count;
				}

				m_Stream.Seek( 3, System.IO.SeekOrigin.Begin );
				m_Stream.Write( (short) count );
			}
		}

		#endregion
	}
}
