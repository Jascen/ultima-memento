using System;
using Server;
using System.Collections.Generic;
using System.Collections;
using Server.Mobiles;
using Server.Items;
using Server.Regions;
using Server.Network;
using Server.Multis;
using Server.Misc;
using Server.ContextMenus;
using Server.Gumps;
using Server.Commands;

namespace Server.Items 
{
	public enum MapRangerEffect
	{
		Charges
	}

	public class MapRanger : Item
	{
		public override string DefaultDescription{ get{ return "This is a trail map that shows a secret path to get to the location drawn on the map. You can only use these maps if you have at least an 80 in either skills of tracking or cartography. They can never be copied and they eventually wear out from use. If you double click the map while it is in your pack, you will follow the path of the map by yourself. No one will be able to follow you on your quick journey to this place. If you set the map down and double click it, then others will be able to use the map to go with you on your journey as they can double click the map left behind to follow you. The original map will be put back into your pack, while this map left behind will only remain for about 30 seconds so your comrades should make haste and follow. If a map goes to a world you have not discovered the way into on your own, then you will toss the map out as you cannot seem to find the path to this place."; } }

		private MapRangerEffect m_MapRangerEffect;
		private int m_Charges;
		private string m_MapDestination;
		private Point3D m_PointDest;
		private Map m_MapDest;

		[CommandProperty( AccessLevel.GameMaster )]
		public MapRangerEffect Effect { get{ return m_MapRangerEffect; } set{ m_MapRangerEffect = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges { get{ return m_Charges; } set{ m_Charges = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string MapDestination { get { return m_MapDestination; } set { m_MapDestination = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D PointDest { get { return m_PointDest; } set { m_PointDest = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Map MapDest { get { return m_MapDest; } set { m_MapDest = value; InvalidateProperties(); } }

		[Constructable]
		public MapRanger() : base( 0x14EB )
		{
			Weight = 1.0; 
			Charges = 10;
			ItemID = Utility.RandomList( 0x14EB, 0x14EC );
			MapSetup( this );
			Name = "Trail Map to " + m_MapDestination;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, Server.Lands.LandName( Server.Lands.GetLand( m_MapDest, m_PointDest, m_PointDest.X, m_PointDest.Y ) ) );
			list.Add( 1049644, "Use To Get To Locations Quicker");
        } 
		
		public override void OnDoubleClick( Mobile from )
		{
			Land GoToWorld = Server.Lands.GetLand( m_MapDest, m_PointDest, m_PointDest.X, m_PointDest.Y );

			bool CanUseMap = false;

			if ( GoToWorld == Land.Sosaria ){ CanUseMap = true; }
			else if ( GoToWorld == Land.UmberVeil && PlayerSettings.GetDiscovered( from, Land.UmberVeil ) ){ CanUseMap = true; }
			else if ( GoToWorld == Land.Ambrosia && PlayerSettings.GetDiscovered( from, Land.Ambrosia ) ){ CanUseMap = true; }
			else if ( GoToWorld == Land.Lodoria && PlayerSettings.GetDiscovered( from, Land.Lodoria ) ){ CanUseMap = true; }
			else if ( GoToWorld == Land.Serpent && PlayerSettings.GetDiscovered( from, Land.Serpent ) ){ CanUseMap = true; }
			else if ( GoToWorld == Land.IslesDread && PlayerSettings.GetDiscovered( from, Land.IslesDread ) ){ CanUseMap = true; }
			else if ( GoToWorld == Land.Savaged && PlayerSettings.GetDiscovered( from, Land.Savaged ) ){ CanUseMap = true; }
			else if ( GoToWorld == Land.Kuldar && PlayerSettings.GetDiscovered( from, Land.Kuldar ) ){ CanUseMap = true; }

			if ( from.Skills[SkillName.Tracking].Value < 80 && from.Skills[SkillName.Cartography].Value < 80 )
			{
				from.SendMessage( "You must be a adept ranger or cartographer to use this map." );
				return;
			}
			else if (	from.Region.IsPartOf( typeof( BardTownRegion ) ) || 
						from.Region.IsPartOf( typeof( BardDungeonRegion ) ) )
			{
				from.SendMessage( "This won't lead you out of this place." ); 
				return;
			}
			else if (	!Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) ) && 
						!from.Region.IsPartOf( typeof( OutDoorRegion ) ) && 
						!from.Region.IsPartOf( typeof( OutDoorBadRegion ) ) && 
						!from.Region.IsPartOf( typeof( VillageRegion ) ) && 
						!from.Region.IsPartOf( typeof( PublicRegion ) ) )
			{
				from.SendMessage( "You can only use this map outdoors." ); 
				return;
			}
			else if ( CanUseMap == false )
			{
				from.SendMessage( "Not knowing how to get to this world, you throw the map away." );
				this.Delete();
				return;
			}
			else if ( IsChildOf( from.Backpack ) && Charges > 0 ) 
			{
				ConsumeCharge( from );
				MapTeleport( from, m_PointDest, m_MapDest );
				return;
			}
			else if ( from.InRange( this.GetWorldLocation(), 3 ) && Charges > 0 )
			{
				ConsumeCharge( from );
				InternalItem builtMap = new InternalItem();
				builtMap.Name = this.Name;
				builtMap.ItemID = this.ItemID;
				MapRangerDoor rangerDoor = (MapRangerDoor)builtMap;
				rangerDoor.m_PointDest = m_PointDest;
				rangerDoor.m_MapDest = m_MapDest;
				builtMap.MoveToWorld( this.Location, this.Map );
				from.AddToBackpack( this );
				MapTeleport( from, m_PointDest, m_MapDest );
			}
			else if ( !from.InRange( this.GetWorldLocation(), 3 ) && Charges > 0 )
			{
				from.SendLocalizedMessage( 502138 ); // That is too far away for you to use
				return;
			}
			else
			{
				from.SendMessage( "This map is too worn from over use, and is no longer of any good." );
				this.Delete();
				return;
			}
		}

		public static void MapTeleport( Mobile m, Point3D loc, Map map )
		{
			BaseCreature.TeleportPets( m, loc, map, false );
			m.MoveToWorld ( loc, map );
			m.PlaySound( 0x249 );
		}

		public void ConsumeCharge( Mobile from )
		{
			--Charges;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1060584, "{0}\t{1}", m_Charges.ToString(), "Uses" );
		}

		private class InternalItem : MapRangerDoor
		{
			public InternalItem()
			{
				InternalTimer t = new InternalTimer( this );
				t.Start();
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
				Delete();
			}

			private class InternalTimer : Timer
			{
				private Item m_Item;

				public InternalTimer( Item item ) : base( TimeSpan.FromSeconds( 30.0 ) )
				{
					Priority = TimerPriority.OneSecond;
					m_Item = item;
				}

				protected override void OnTick()
				{
					m_Item.Delete();
				}
			}
		}

		public static void MapSetup( MapRanger paper )
		{
			if ( Utility.RandomMinMax( 1, 3 ) > 1 )
			{
				switch ( Utility.Random( 71 ) )
				{
					case 0: paper.m_MapDestination = "the City of the Dead"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(5828, 3263, 0); break;
					case 1: paper.m_MapDestination = "the Mausoleum"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(1529, 3599, 0); break;
					case 2: paper.m_MapDestination = "the Valley of Dark Druids"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(6763, 1423, 2); break;
					case 3: paper.m_MapDestination = "Vordo's Castle"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(6708, 1729, 25); break;
					case 4: paper.m_MapDestination = "the Crypts of Kuldar"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(6668, 1568, 10); break;
					case 5: paper.m_MapDestination = "the Kuldara Sewers"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(6790, 1745, 24); break;
					case 6: paper.m_MapDestination = "the Ancient Pyramid"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(1162, 472, 0); break;
					case 7: paper.m_MapDestination = "Dungeon Exodus"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(877, 2702, 0); break;
					case 8: paper.m_MapDestination = "the Cave of Banished Mages"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(3798, 1879, 2); break;
					case 9: paper.m_MapDestination = "Dungeon Clues"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(3760, 2038, 0); break;
					case 10: paper.m_MapDestination = "Dardin's Pit"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(3006, 446, 0); break;
					case 11: paper.m_MapDestination = "Dungeon Doom"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(1628, 2561, 0); break;
					case 12: paper.m_MapDestination = "the Fires of Hell"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(3345, 1647, 0); break;
					case 13: paper.m_MapDestination = "the Mines of Morinia"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(1022, 1369, 2); break;
					case 14: paper.m_MapDestination = "the Perinian Depths"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(3619, 456, 0); break;
					case 15: paper.m_MapDestination = "the Dungeon of Time Awaits"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(3831, 1494, 0); break;
					case 16: paper.m_MapDestination = "the Pirate Cave"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(1842, 2211, 0); break;
					case 17: paper.m_MapDestination = "the Vault of the Black Knight"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(1581, 202, 0); break;
					case 18: paper.m_MapDestination = "the Undersea Pass"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(1179, 1931, 0); break;
					case 19: paper.m_MapDestination = "the Castle of Dracula"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(466, 3794, 0); break;
					case 20: paper.m_MapDestination = "the Crypts of Dracula"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(466, 3794, 0); break;
					case 21: paper.m_MapDestination = "the Lodoria Catacombs"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(1869, 2378, 0); break;
					case 22: paper.m_MapDestination = "Dungeon Covetous"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(4019, 2436, 2); break;
					case 23: paper.m_MapDestination = "Dungeon Deceit"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(2523, 757, 1); break;
					case 24: paper.m_MapDestination = "Dungeon Despise"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(1278, 1852, 0); break;
					case 25: paper.m_MapDestination = "Dungeon Destard"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(749, 630, 0); break;
					case 26: paper.m_MapDestination = "the City of Embers"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(3196, 3318, 0); break;
					case 27: paper.m_MapDestination = "Dungeon Hythloth"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(1634, 2805, 0); break;
					case 28: paper.m_MapDestination = "the Frozen Hells"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(3769, 1092, 0); break;
					case 29: paper.m_MapDestination = "the Ice Fiend Lair"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(3769, 1092, 0); break;
					case 30: paper.m_MapDestination = "the Halls of Undermountain"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(959, 2669, 5); break;
					case 31: paper.m_MapDestination = "Dungeon Shame"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(1405, 2338, 0); break;
					case 32: paper.m_MapDestination = "Terathan Keep"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(624, 2403, 2); break;
					case 33: paper.m_MapDestination = "the Volcanic Cave"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(3105, 3594, 0); break;
					case 34: paper.m_MapDestination = "Dungeon Wrong"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(2252, 854, 1); break;
					case 35: paper.m_MapDestination = "the Blood Temple"; paper.m_MapDest = Map.IslesDread; paper.m_PointDest = new Point3D(1258, 1231, 0); break;
					case 36: paper.m_MapDestination = "the Ice Queen Fortress"; paper.m_MapDest = Map.IslesDread; paper.m_PointDest = new Point3D(319, 324, 5); break;
					case 37: paper.m_MapDestination = "Dungeon of the Mad Archmage"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(464, 851, -60); break;
					case 38: paper.m_MapDestination = "Dungeon of the Lich King"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(922, 1772, 26); break;
					case 39: paper.m_MapDestination = "the Halls of Ogrimar"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(1107, 1380, 17); break;
					case 40: paper.m_MapDestination = "the Ratmen Mines"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(157, 1369, 32); break;
					case 41: paper.m_MapDestination = "Dungeon Rock"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(1092, 1038, 0); break;
					case 42: paper.m_MapDestination = "the Storm Giant Lair"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(283, 466, 14); break;
					case 43: paper.m_MapDestination = "the Corrupt Pass"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(155, 1125, 60); break;
					case 44: paper.m_MapDestination = "the Tombs"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(222, 1361, 0); break;
					case 45: paper.m_MapDestination = "the Ancient Prison"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(748, 846, 1); break;
					case 46: paper.m_MapDestination = "the Cave of Fire"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(561, 1143, 0); break;
					case 47: paper.m_MapDestination = "the Cave of Souls"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(121, 1475, 0); break;
					case 48: paper.m_MapDestination = "Dungeon Ankh"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(465, 1435, 2); break;
					case 49: paper.m_MapDestination = "Dungeon Bane"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(310, 761, 2); break;
					case 50: paper.m_MapDestination = "Dungeon Hate"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(1459, 1220, 0); break;
					case 51: paper.m_MapDestination = "Dungeon Scorn"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(1463, 873, 2); break;
					case 52: paper.m_MapDestination = "Dungeon Torment"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(1690, 1225, 0); break;
					case 53: paper.m_MapDestination = "Dungeon Vile"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(1554, 991, 2); break;
					case 54: paper.m_MapDestination = "Dungeon Wicked"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(733, 260, 0); break;
					case 55: paper.m_MapDestination = "Dungeon Wrath"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(1803, 918, 0); break;
					case 56: paper.m_MapDestination = "the Flooded Temple"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(1069, 952, 2); break;
					case 57: paper.m_MapDestination = "the Gargoyle Crypts"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(1267, 936, 0); break;
					case 58: paper.m_MapDestination = "the Serpent Sanctum"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(1093, 1609, 0); break;
					case 59: paper.m_MapDestination = "the Tomb of the Fallen Wizard"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(1056, 1338, 0); break;
					case 60: paper.m_MapDestination = "the Scurvy Reef"; paper.m_MapDest = Map.IslesDread; paper.m_PointDest = new Point3D(713, 493, 1); break;
					case 61: paper.m_MapDestination = "the Glacial Scar"; paper.m_MapDest = Map.IslesDread; paper.m_PointDest = new Point3D(238, 171, 0); break;
					case 62: paper.m_MapDestination = "the Temple of Osirus"; paper.m_MapDest = Map.IslesDread; paper.m_PointDest = new Point3D(601, 819, 20); break;
					case 63: paper.m_MapDestination = "the Forgotten Halls"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(3015, 944, 0); break;
					case 64: paper.m_MapDestination = "Stonegate Castle"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(1355, 404, 0); break;
					case 65: paper.m_MapDestination = "the Ancient Elven Mine"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(1179, 1931, 0); break;
					case 66: paper.m_MapDestination = "the Undersea Castle"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(283, 409, 20); break;
					case 67: paper.m_MapDestination = "the Azure Castle"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(774, 612, 15); break;
					case 68: paper.m_MapDestination = "the Tomb of Kazibal"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(368, 298, 57); break;
					case 69: paper.m_MapDestination = "the Catacombs of Azerok"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(1056, 424, 38); break;
					case 70: paper.m_MapDestination = "the Sanctum of Saltmarsh"; paper.m_MapDest = Map.IslesDread; paper.m_PointDest = new Point3D(926, 874, 0); break;
				}
			}
			else
			{
				switch ( Utility.Random( 24 ) )
				{
					case 0: paper.m_MapDestination = "the Druid's Glade"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(3910, 3971, 0); break;
					case 1: paper.m_MapDestination = "the Town of Moon"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(860, 678, 0); break;
					case 2: paper.m_MapDestination = "the Village of Fawn"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(2069, 255, 0); break;
					case 3: paper.m_MapDestination = "the Village of Yew"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(2530, 872, 2); break;
					case 4: paper.m_MapDestination = "the City of Britain"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(2942, 1092, 0); break;
					case 5: paper.m_MapDestination = "the Village of Grey"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(866, 2082, 0); break;
					case 6: paper.m_MapDestination = "the City of Montor"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(3198, 2590, 0); break;
					case 7: paper.m_MapDestination = "the Town of Renika"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(1471, 3761, 0); break;
					case 8: paper.m_MapDestination = "the Bottle World of Kuldar"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(6675, 1820, 15); break;
					case 9: paper.m_MapDestination = "the Village of Kurak"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(807, 857, -1); break;
					case 10: paper.m_MapDestination = "the Village of Barako"; paper.m_MapDest = Map.SavagedEmpire; paper.m_PointDest = new Point3D(196, 1698, 37); break;
					case 11: paper.m_MapDestination = "the Cimmeran Hold"; paper.m_MapDest = Map.IslesDread; paper.m_PointDest = new Point3D(299, 1068, 15); break;
					case 12: paper.m_MapDestination = "the Serpent Island"; paper.m_MapDest = Map.SerpentIsland; paper.m_PointDest = new Point3D(799, 1364, 2); break;
					case 13: paper.m_MapDestination = "the Village of Whisper"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(912, 995, 1); break;
					case 14: paper.m_MapDestination = "the Town of Glacial Hills"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(3650, 417, 2); break;
					case 15: paper.m_MapDestination = "the Village of Springvale"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(4258, 1402, 0); break;
					case 16: paper.m_MapDestination = "the City of Elidor"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(2876, 1358, 0); break;
					case 17: paper.m_MapDestination = "the Village of Portshine"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(814, 1991, 1); break;
					case 18: paper.m_MapDestination = "the Lodoria City Park"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(2011, 2229, 0); break;
					case 19: paper.m_MapDestination = "the Village of Islegem"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(2803, 2251, 0); break;
					case 20: paper.m_MapDestination = "Greensky Village"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(4228, 2958, 0); break;
					case 21: paper.m_MapDestination = "the Port of Starguide"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(2318, 3130, 0); break;
					case 22: paper.m_MapDestination = "the Port of Dusk"; paper.m_MapDest = Map.Lodor; paper.m_PointDest = new Point3D(2633, 3206, 0); break;
					case 23: paper.m_MapDestination = "the Stone Wall Inn"; paper.m_MapDest = Map.Sosaria; paper.m_PointDest = new Point3D(1833, 755, 0); break;
				}
			}
		}

		public MapRanger( Serial serial ) : base( serial )
		{ 
		} 
		
		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 );
			writer.Write( (int) m_MapRangerEffect );
			writer.Write( (int) m_Charges );
            writer.Write( (string) m_MapDestination );
			writer.Write( m_PointDest );
			writer.Write( m_MapDest );
		} 
		
		public override void Deserialize(GenericReader reader) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt();
			switch ( version )
			{
				case 0:
				{
					m_MapRangerEffect = (MapRangerEffect)reader.ReadInt();
					m_Charges = (int)reader.ReadInt();
					break;
				}
			}
            m_MapDestination = reader.ReadString();
			m_PointDest = reader.ReadPoint3D();
			m_MapDest = reader.ReadMap();
		}
	}
	///////////////////////////////////////////////////////////////////////////////////////////////
	public class MapRangerDoor : Item
	{
		public Point3D m_PointDest;
		public Map m_MapDest;

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D PointDest
		{
			get { return m_PointDest; }
			set { m_PointDest = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Map MapDest
		{
			get { return m_MapDest; }
			set { m_MapDest = value; InvalidateProperties(); }
		}

		[Constructable]
		public MapRangerDoor() : base( 0x22C4 )
		{
			Name = "trail map";
			Weight = 1.0;
			Movable = false;
		}

		public void DoMapRangerDoor( Mobile m )
		{
			if ( m is PlayerMobile )
			{
				MapTeleport( m, m_PointDest, m_MapDest );
			}
		}

		public override void OnDoubleClick( Mobile m )
		{
			if ( m.InRange( this.GetWorldLocation(), 2 ) )
			{
				DoMapRangerDoor( m );
			}
			else
			{
				m.SendLocalizedMessage( 502138 ); // That is too far away for you to use
			}
		}

        public override void OnDoubleClickDead( Mobile m )
        {
			if ( m.InRange( this.GetWorldLocation(), 2 ) )
			{
				DoMapRangerDoor( m );
			}
			else
			{
				m.SendLocalizedMessage( 502138 ); // That is too far away for you to use
			}
        }

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Double-Click To Follow The Path");
        } 

		public static void MapTeleport( Mobile m, Point3D loc, Map map )
		{
			BaseCreature.TeleportPets( m, loc, map, false );
			m.MoveToWorld ( loc, map );
			m.PlaySound( 0x249 );
		}

		public MapRangerDoor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write( m_PointDest );
			writer.Write( m_MapDest );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_PointDest = reader.ReadPoint3D();
			m_MapDest = reader.ReadMap();
		}
	}
}