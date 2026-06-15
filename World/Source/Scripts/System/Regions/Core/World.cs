using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Misc;
using Server.Items;
using Server.Network;
using Server.Commands;
using Server.Commands.Generic;
using Server.Mobiles;
using Server.Accounting;
using Server.Regions;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Fourth;
using Server.Spells.Magical;
using Server.Spells.Bushido;
using Server.Spells.Ninjitsu;
using Server.Spells.Necromancy;
using Server.Spells.Chivalry;
using Server.Spells.DeathKnight;
using Server.Spells.Herbalist;
using Server.Spells.Undead;
using Server.Spells.Mystic;
using Server.Spells.Research;
using Server.Spells.Elementalism;

namespace Server.Misc
{
    class Worlds
    {
		public static void EnteredTheLand( Mobile from )
		{
			if ( from is PlayerMobile )
			{
				string world = GetRegionName( from.Map, from.Location );

				bool runLog = false;

				if ( world == "the Land of Lodoria" ){ PlayerSettings.SetDiscovered( from, world, true ); runLog = true; }
				else if ( world == "the Land of Sosaria" )
				{
					if ( from.X >= 3546 && from.Y >= 3383 && from.X <= 3590 && from.Y <= 3428 ){ /* DO NOTHING IN TIME LORD CHAMBER */ }
					else { PlayerSettings.SetDiscovered( from, world, true ); runLog = true; }
				}
				else if ( world == "the Island of Umber Veil" ){ PlayerSettings.SetDiscovered( from, world, true ); runLog = true; }
				else if ( world == "the Land of Ambrosia" ){ PlayerSettings.SetDiscovered( from, world, true ); runLog = true; }
				else if ( world == "the Serpent Island" ){ PlayerSettings.SetDiscovered( from, world, true ); runLog = true; }
				else if ( world == "the Isles of Dread" ){ PlayerSettings.SetDiscovered( from, world, true ); runLog = true; }
				else if ( world == "the Savaged Empire" ){ PlayerSettings.SetDiscovered( from, world, true ); runLog = true; }
				else if ( world == "the Underworld" ){ PlayerSettings.SetDiscovered( from, world, true ); runLog = true; }
				else if ( world == "the Bottle World of Kuldar" ){ PlayerSettings.SetDiscovered( from, world, true ); runLog = true; }

				if ( runLog )
					LoggingFunctions.LogRegions( from, world, "enter" );
			}
		}

		public static bool isOrientalRegion( Mobile m )
		{
			if ( m is PlayerMobile )
				return false;

			if ( m is BaseCreature && ((BaseCreature)m).Controlled )
				return false;

			if ( m.Region.Name == "the Dojo" )
				return true;

			// DOJO IN LODOR CITY
			if ( m.Map == Map.Lodor && m.X >= 1888 && m.Y >= 2136 && m.X <= 1897 && m.Y <= 2150 )
				return true;

			// DOJO IN LODOR CITY
			if ( m.Map == Map.Lodor && m.X >= 1877 && m.Y >= 2145 && m.X <= 1891 && m.Y <= 2159 )
				return true;

			if ( m.Region.Name == "Shimazu's Dojo" )
				return true;

			return false;
		}

		public static bool isHauntedRegion( Mobile m )
		{
			if ( m.Map == Map.Sosaria )
			{
				if ( m.X >= 2612 && m.Y >= 3197 && m.X <= 2758 && m.Y <= 3373 )
					return true;
				if ( m.X >= 6981 && m.Y >= 6 && m.X <= 7150 && m.Y <= 166 )
					return true;
				if ( m.X >= 5211 && m.Y >= 1029 && m.X <= 5408 && m.Y <= 1307 )
					return true;
			}
			else if ( m.Map == Map.SavagedEmpire )
			{
				if ( m.X >= 251 && m.Y >= 2102 && m.X <= 554 && m.Y <= 2397 )
					return true;
				if ( m.X >= 0 && m.Y >= 2495 && m.X <= 180 && m.Y <= 2829 )
					return true;
			}
			else if ( m.Map == Map.Lodor )
			{
				if ( m.X >= 302 && m.Y >= 3466 && m.X <= 558 && m.Y <= 3804 )
					return true;
				if ( m.X >= 5123 && m.Y >= 519 && m.X <= 5351 && m.Y <= 760 )
					return true;
				if ( m.X >= 6746 && m.Y >= 1399 && m.X <= 7086 && m.Y <= 1728 )
					return true;
				if ( m.X >= 6472 && m.Y >= 1881 && m.X <= 6647 && m.Y <= 2042 )
					return true;
				if ( m.X >= 6450 && m.Y >= 3060 && m.X <= 7006 && m.Y <= 3466 )
					return true;
				if ( m.X >= 6548 && m.Y >= 3441 && m.X <= 7002 && m.Y <= 3897 )
					return true;
			}

			return false;
		}

		public static bool isSciFiRegion( Mobile m )
		{
			if ( m.Map != Map.SavagedEmpire )
				return false;

			if ( m.X >= 556 && m.Y >= 2816 && m.X <= 781 && m.Y <= 3123 )
				return true;

			if ( m.X >= 982 && m.Y >= 2907 && m.X <= 1233 && m.Y <= 3174 )
				return true;

			if ( m.X >= 875 && m.Y >= 3617 && m.X <= 1244 && m.Y <= 4054 )
				return true;

			return false;
		}

		public static bool NoApocalypse( Point3D p, Map map )
		{
			Region reg = Region.Find( p, map );

			if ( reg is WantedRegion || 
			reg is SavageRegion || 
			reg is VillageRegion || 
			reg is UnderHouseRegion || 
			reg is UmbraRegion || 
			reg is TownRegion || 
			reg is StartRegion || 
			reg is SkyHomeDwelling || 
			reg is SafeRegion || 
			reg is ProtectedRegion || 
			reg is PublicRegion || 
			reg is PirateRegion || 
			reg is BardTownRegion || 
			reg is DawnRegion || 
			reg is DungeonHomeRegion || 
			reg is GargoyleRegion || 
			reg is GuardedRegion || 
			reg is HouseRegion || 
			reg is LunaRegion || 
			reg is MazeRegion || 
			reg is MoonCore  )
				return true;

			return false;
		}

		public static string GetRegionName( Map map, Point3D location )
		{
			Region reg = Region.Find( location, map );

			string regionName = reg.Name;

			if ( ( reg.IsDefault || reg.Name == null || reg.Name == "" ) )
			{
				if ( map == Map.Lodor )
				{
					if ( location.X >= 0 && location.Y >= 0 && location.X <= 5118 && location.Y <= 4092 ){ regionName = "the Land of Lodoria"; }
				}
				else if ( map == Map.Sosaria )
				{
					if ( location.X >= 0 && location.Y >= 0 && location.X <= 5118 && location.Y <= 3125 ){ regionName = "the Land of Sosaria"; }
					else if ( location.X >= 699 && location.Y >= 3129 && location.X <= 2272 && location.Y <= 4095 ){ regionName = "the Island of Umber Veil"; }
					else if ( location.X >= 5122 && location.Y >= 3036 && location.X <= 6126 && location.Y <= 4095 ){ regionName = "the Land of Ambrosia"; }
					else if ( location.X >= 6127 && location.Y >= 828 && location.X <= 7167 && location.Y <= 2743 ){ regionName = "the Bottle World of Kuldar"; }
				}
				else if ( map == Map.Underworld )
				{
					if ( location.X >= 0 && location.Y >= 0 && location.X <= 1624 && location.Y <= 1600 ){ regionName = "the Underworld"; }
				}
				else if ( map == Map.SerpentIsland )
				{
					if ( location.X >= 0 && location.Y >= 0 && location.X <= 1874 && location.Y <= 2042 ){ regionName = "the Serpent Island"; }
				}
				else if ( map == Map.IslesDread )
				{
					if ( location.X >= 0 && location.Y >= 0 && location.X <= 1430 && location.Y <= 1430 ){ regionName = "the Isles of Dread"; }
				}
				else if ( map == Map.SavagedEmpire )
				{
					if ( location.X >= 0 && location.Y >= 0 && location.X <= 1168 && location.Y <= 1802 ){ regionName = "the Savaged Empire"; }
				}
			}

			return regionName;
		}

		public static bool IsMainRegion( string region )
		{
			if ( 	region == "the Land of Lodoria" || 
					region == "the Land of Sosaria" || 
					region == "the Island of Umber Veil" || 
					region == "the Land of Ambrosia" || 
					region == "the Bottle World of Kuldar" || 
					region == "the Underworld" || 
					region == "the Serpent Island" || 
					region == "the Isles of Dread" || 
					region == "the Savaged Empire" )
				return true;

			return false;
		}

		public static string GetMyRegion( Map map, Point3D location )
		{
			Region reg = Region.Find( location, map );
			return reg.Name;
		}

		public static string GetMyMapString( Map map )
		{
			string world = "Sosaria";

			if ( map == Map.Lodor ){ world = "Lodor"; }
			else if ( map == Map.Underworld ){ world = "Underworld"; }
			else if ( map == Map.SerpentIsland ){ world = "SerpentIsland"; }
			else if ( map == Map.IslesDread ){ world = "IslesDread"; }
			else if ( map == Map.SavagedEmpire ){ world = "SavagedEmpire"; }

			return world;
		}

		public static string GetMyMapDisplayName( Map map )
		{
			if ( map == Map.Sosaria ) return "Sosaria"; 
			if ( map == Map.Lodor ) return "Lodor"; 
			if ( map == Map.Underworld ) return "Underworld"; 
			if ( map == Map.SerpentIsland ) return "Serpent Island"; 
			if ( map == Map.IslesDread ) return "Isles of Dread"; 
			if ( map == Map.SavagedEmpire ) return "Savaged Empire"; 
			if ( map == Map.Atlantis ) return "Atlantis"; 

			return "Unknown";
		}

		public static Map GetMyDefaultMap( Land land )
		{
			Map map = Map.Sosaria;

			if ( land == Land.SkaraBrae ){ map = Map.Lodor; }
			else if ( land == Land.Lodoria ){ map = Map.Lodor; }
			else if ( land == Land.Serpent ){ map = Map.SerpentIsland; }
			else if ( land == Land.IslesDread ){ map = Map.IslesDread; }
			else if ( land == Land.Savaged ){ map = Map.SavagedEmpire; }
			else if ( land == Land.Underworld ){ map = Map.Underworld; }
			/// THE REST ARE ON SOSARIA ///

			return map;
		}

		public static Map GetPCDefaultMap( Mobile m )
		{
			Map map = Map.Sosaria;

			if ( m.Land == Land.SkaraBrae ){ map = Map.Lodor; }
			else if ( m.Land == Land.Lodoria ){ map = Map.Lodor; }
			else if ( m.Land == Land.Serpent ){ map = Map.SerpentIsland; }
			else if ( m.Land == Land.IslesDread ){ map = Map.IslesDread; }
			else if ( m.Land == Land.Savaged ){ map = Map.SavagedEmpire; }
			else if ( m.Land == Land.Underworld ){ map = Map.Underworld; }
			/// THE REST ARE ON SOSARIA ///

			return map;
		}

		public static Map GetMyDefaultTreasureMap( Land land )
		{
			Map map = Map.Sosaria;

			if ( land == Land.Lodoria ){ map = Map.Lodor; }
			else if ( land == Land.Serpent ){ map = Map.SerpentIsland; }
			else if ( land == Land.IslesDread ){ map = Map.IslesDread; }
			else if ( land == Land.Savaged ){ map = Map.SavagedEmpire; }
			else if ( land == Land.Underworld ){ map = Map.Underworld; }
			/// THE REST ARE ON SOSARIA ///

			return map;
		}

		public static bool IsCrypt( Point3D p, Map map )
		{
			Region reg = Region.Find( p, map );
			
			if ( reg.IsPartOf( "the Crypt" ) || 
				reg.IsPartOf( "the Lodoria Catacombs" ) || 
				reg.IsPartOf( "the Crypts of Dracula" ) || 
				reg.IsPartOf( "the Castle of Dracula" ) || 
				reg.IsPartOf( "the Graveyard" ) || 
				reg.IsPartOf( "Ravendark Woods" ) || 
				reg.IsPartOf( "the Island of Dracula" ) || 
				reg.IsPartOf( "the Village of Ravendark" ) || 
				reg.IsPartOf( "the Black Magic Guild" ) || 
				reg.IsPartOf( "the Lodoria Cemetery" ) || 
				reg.IsPartOf( "the Lost Graveyard" ) || 
				reg.IsPartOf( "the Mausoleum" ) || 
				reg.IsPartOf( "the Dark Tombs" ) || 
				reg.IsPartOf( "the Kuldar Cemetery" ) || 
				reg.IsPartOf( "the Undercity of Umbra" ) || 
				reg.IsPartOf( "the Cave of Souls" ) || 
				reg.IsPartOf( "the Crypts of Kuldar" ) || 
				reg.IsPartOf( "the Zealan Graveyard" ) || 
				reg.IsPartOf( "Nightwood Fort" ) || 
				reg.IsPartOf( "the Zealan Tombs" ) || 
				reg.IsPartOf( "the Tombs" ) || 
				reg.IsPartOf( "the Dungeon of the Lich King" ) || 
				reg.IsPartOf( "the Tomb of Kazibal" ) || 
				reg.IsPartOf( "the Catacombs of Azerok" ) )
				return true;

			return false;
		}

		public static bool IsWaterSea( Mobile m )
		{
			if ( IsExploringSeaAreas( m ) )
				return true;

			if ( IsSeaDungeon( m.Location, m.Map ) )
				return true;

			if ( IsSeaTown( m.Location, m.Map ) )
				return true;

			return false;
		}

		public static bool IsSeaDungeon( Point3D p, Map map )
		{
			Region reg = Region.Find( p, map );

			if ( reg.IsPartOf( "the Depths of Carthax Lake" ) || 
			reg.IsPartOf( "the Storm Giant Lair" ) || 
			reg.IsPartOf( "the Island of the Storm Giant" ) || 
			reg.IsPartOf( "the Undersea Castle" ) || 
			reg.IsPartOf( "the Scurvy Reef" ) || 
			reg.IsPartOf( "the Caverns of Poseidon" ) || 
			reg.IsPartOf( "the Cavern of the Deep Ones" ) || 
			Server.Lands.GetLand( map, p, p.X, p.Y ) == Land.Atlantis || 
			reg.IsPartOf( "the Flooded Temple" ) )
				return true;

			return false;
		}

		public static bool IsSeaTown( Point3D p, Map map )
		{
			Region reg = Region.Find( p, map );

			if ( reg.IsPartOf( "Anchor Rock Docks" ) || 
			reg.IsPartOf( "Kraken Reef Docks" ) || 
			reg.IsPartOf( "Savage Sea Docks" ) || 
			reg.IsPartOf( "the Lankhmar Lighthouse" ) || 
			reg.IsPartOf( "Serpent Sail Docks" ) || 
			reg.IsPartOf( "the Forgotten Lighthouse" ) || 
			reg.IsPartOf( "the Port" ) )
				return true;

			return false;
		}

		public static bool IsFireDungeon( Point3D p, Map map )
		{
			Region reg = Region.Find( p, map );

			if ( reg.IsPartOf( "the Fires of Hell" ) || 
			reg.IsPartOf( "Morgaelin's Inferno" ) || 
			reg.IsPartOf( "the City of Embers" ) || 
			reg.IsPartOf( "the Cave of Fire" ) || 
			reg.IsPartOf( "Steamfire Cave" ) || 
			reg.IsPartOf( "the Volcanic Cave" ) )
				return true;

			return false;
		}

		public static bool IsOnSpaceship( Point3D p, Map map )
		{
			Region reg = Region.Find( p, map );

			if ( reg.IsPartOf( "the Ancient Crash Site" ) || 
			reg.IsPartOf( "the Ancient Sky Ship" ) )
				return true;

			return false;
		}

		public static bool IsIceDungeon( Point3D p, Map map )
		{
			Region reg = Region.Find( p, map );

			if ( reg.IsPartOf( "the Glacial Scar" ) || 
			reg.IsPartOf( "the Frozen Hells" ) || 
			reg.IsPartOf( "the Ratmen Cave" ) || 
			reg.IsPartOf( "the Ice Fiend Lair" ) || 
			reg.IsPartOf( "the Ice Queen Fortress" ) || 
			reg.IsPartOf( "the Frozen Dungeon" ) || 
			reg.IsPartOf( "Frostwall Caverns" ) )
				return true;

			return false;
		}

		public static bool IsExploringSeaAreas( Mobile m )
		{
			if ( IsOnBoat( m ) == true && BoatToCloseToTown( m ) == false )
				return true;

			Region reg = Region.Find( m.Location, m.Map );

			if ( reg.IsPartOf( "the Caverns of Poseidon" ) )
				return true;

			if ( reg.IsPartOf( "the Storm Giant Lair" ) )
				return true;

			if ( reg.IsPartOf( "the Cavern of the Deep Ones" ) )
				return true;

			if ( reg.IsPartOf( "the Island of the Storm Giant" ) )
				return true;

			if ( reg.IsPartOf( "the Island of Poseidon" ) )
				return true;

			if ( reg.IsPartOf( "the Buccaneer's Den" ) )
				return true;

			if ( reg.IsPartOf( "the Undersea Castle" ) )
				return true;

			if ( reg.IsPartOf( "the Depths of Carthax Lake" ) )
				return true;

			if ( reg.IsPartOf( "the Scurvy Reef" ) )
				return true;

			if ( reg.IsPartOf( "the Flooded Temple" ) )
				return true;

			if ( reg.IsPartOf( typeof( PirateRegion ) ) )
				return true;

			if ( m.Land == Land.Atlantis )
				return true;

			return false;
		}

		public static bool IsOnBoat( Mobile m )
		{
			if ( m.Z > -1 || m.Z < -3 )
				return false;

			int KeepSearching = 0;
			bool IsOnShip = false;

			foreach ( Item boatman in m.GetItemsInRange( 15 ) )
			{
				if ( KeepSearching != 1 )
				{
					if ( boatman is TillerMan )
					{
						IsOnShip = true;
						if ( IsOnShip == true ){ KeepSearching = 1; }
					}
				}
			}
			return IsOnShip;
		}

		public static bool ItemOnBoat( Item i )
		{
			if ( i.Z > -1 || i.Z < -3 )
				return false;

			int KeepSearching = 0;
			bool IsOnShip = false;

			foreach ( Item boatman in i.GetItemsInRange( 15 ) )
			{
				if ( KeepSearching != 1 )
				{
					if ( boatman is TillerMan )
					{
						IsOnShip = true;
						if ( IsOnShip == true ){ KeepSearching = 1; }
					}
				}
			}
			return IsOnShip;
		}

		public static bool BoatToCloseToTown( Mobile m )
		{
			foreach ( Mobile landlover in m.GetMobilesInRange( 50 ) )
			{
				if ( landlover is BaseVendor || landlover is BasePerson || landlover is BaseNPC )
				{
					return true;
				}
			}
			return false;
		}

		public static bool RegionAllowedTeleport( Map map, Point3D location, int x, int y )
		{
			Land land = Server.Lands.GetLand( map, location, x, y );
			Region reg = Region.Find( location, map );

			if ( reg.IsPartOf( typeof( DungeonRegion ) ) )
				return false;

			if ( land == Land.Kuldar )
				return false;

			if ( reg.IsPartOf( "the Time Lord Chamber" ) )
				return false;

			if ( land == Land.Underworld )
				return false;

			// The one true Ambrosia. All others (ex: dungeon home) are fake
			if ( land == Land.Ambrosia && map == Map.Sosaria )
				return false;

			if ( land == Land.SkaraBrae )
				return false;

			if ( reg.IsPartOf( "the Moon's Core" ) || reg.IsPartOf( "the Core of the Moon" ) || reg.IsPartOf( "Moonlight Cavern" ) )
				return false;

			if ( reg.IsPartOf( "the Camping Tent" ) )
				return false;

			if ( reg.IsPartOf( "the Dungeon Room" ) )
				return false;

			if ( reg.IsPartOf( "the Lyceum" ) )
				return false;

			if ( reg.IsPartOf( "the Island of Stonegate" ) )
				return false;

			if ( reg.IsPartOf( "the Painting of the Glade" ) )
				return false;

			if ( reg.IsPartOf( "the Island of the Black Knight" ) )
				return false;

			if ( reg.IsPartOf( "the Castle of the Black Knight" ) )
				return false;

			if ( reg.IsPartOf( "the Castle of the Black Knight" ) )
				return false;

			if ( reg.IsPartOf( typeof( GargoyleRegion ) ) )
				return false;

			if ( reg.IsPartOf( typeof( MazeRegion ) ) )
				return false;

			if ( reg.IsPartOf( typeof( PublicRegion ) ) )
				return false;

			if ( reg.IsPartOf( "the Island of Poseidon" ) )
				return false;

			if ( reg.IsPartOf( "the Village of Ravendark" ) )
				return false;

			if ( reg.IsPartOf( typeof( BargeDeadRegion ) ) )
				return false;

			return true;
		}

		public static bool AllowEscape( Mobile m, Map map, Point3D location, int x, int y )
		{
			bool canLeave = true;
			int mX = 0;
			int mY = 0;
			int mZ = 0;
			Map mWorld = null;

			string sPublicDoor = ((PlayerMobile)m).CharacterPublicDoor;
			if ( sPublicDoor != null )
			{
				if ( sPublicDoor.Length > 0 )
				{
					string[] sPublicDoors = sPublicDoor.Split('#');
					int nEntry = 1;
					foreach (string exits in sPublicDoors)
					{
						if ( nEntry == 1 ){ mX = Convert.ToInt32(exits); } // 6578
						else if ( nEntry == 2 ){ mY = Convert.ToInt32(exits); } // 2033
						else if ( nEntry == 3 ){ mZ = Convert.ToInt32(exits); } // 45
						else if ( nEntry == 4 ){ try { mWorld = Map.Parse( exits ); } catch{} if ( mWorld == null ){ mWorld = Map.Sosaria; } } // Lodor
						nEntry++;
					}

					location = new Point3D( mX, mY, mZ );
					map = mWorld;
					x = mX;
					y = mY;
				}
			}

			Land land = Server.Lands.GetLand( map, location, x, y );
			Region reg = Region.Find( location, map );

			if ( land == Land.Kuldar && !PlayerSettings.GetKeys( m, "VordoKey" ) )
				canLeave = false;

			if ( land == Land.SkaraBrae )
				canLeave = false;

			if ( reg.IsPartOf( "the Camping Tent" ) )
				canLeave = false;

			if ( reg.IsPartOf( "the Dungeon Room" ) )
				canLeave = false;

			if ( reg.IsPartOf( "the Lyceum" ) )
				canLeave = false;

			if ( reg.IsPartOf( "the Chasm" ) )
				canLeave = false;

			if ( reg.IsPartOf( "the Ship's Lower Deck" ) )
				canLeave = false;

			return canLeave;
		}

		public static bool IsAllowedSpell( Mobile m, ISpell s )
		{
			if ( m.Region.IsPartOf( "the Ship's Lower Deck" ) )
				return false;

			if (	s is GateTravelSpell || 
						s is MushroomGatewaySpell || 
						s is UndeadGraveyardGatewaySpell || 
						s is HellsGateSpell || 
						s is AstralTravel || 
						s is ResearchEtherealTravel || 
						s is NaturesPassageSpell || 
						s is RecallSpell || 
						s is TravelSpell || 
						s is Elemental_Void_Spell || 
						s is Elemental_Gate_Spell || 
						s is SacredJourneySpell )
			{
				return true;
			}

			return false;
		}

		public static bool RegionAllowedRecall( Map map, Point3D location, int x, int y )
		{
			Land land = Server.Lands.GetLand( map, location, x, y );
			Region reg = Region.Find( location, map );

			if ( land == Land.SkaraBrae )
				return false;

			if ( reg.IsPartOf( "Moonlight Cavern" ) )
				return false;

			if ( reg.IsPartOf( "the Village of Ravendark" ) )
				return false;

			return true;
		}

		public static bool IsPlayerInTheLand( Map map, Point3D location, int x, int y )
		{
			Land land = Server.Lands.GetLand( map, location, x, y );

			if ( land == Land.Luna && x >= 5801 && y >= 2716 && x <= 6125 && y <= 3034 )
				return true;
			else if ( land == Land.Sosaria && x >= 0 && y >= 0 && x <= 5119 && y <= 3127 )
				return true;
			else if ( land == Land.Lodoria && x >= 0 && y >= 0 && x <= 5120 && y <= 4095 )
				return true;
			else if ( land == Land.Serpent && x >= 0 && y >= 0 && x <= 1870 && y <= 2047 )
				return true;
			else if ( land == Land.IslesDread && x >= 0 && y >= 0 && x <= 1447 && y <= 1447 )
				return true;
			else if ( land == Land.Savaged && x >= 136 && y >= 8 && x <= 1160 && y <= 1792 )
				return true;
			else if ( land == Land.Ambrosia && x >= 5122 && y >= 3036 && x <= 6126 && y <= 4095 )
				return true;
			else if ( land == Land.UmberVeil && x >= 699 && y >= 3129 && x <= 2272 && y <= 4095 )
				return true;
			else if ( land == Land.Kuldar && x >= 6127 && y >= 828 && x <= 7168 && y <= 2742 )
				return true;
			else if ( land == Land.Underworld && x >= 0 && y >= 0 && x <= 1581 && y <= 1599 )
				return true;

			return false;
		}

		public static bool PlayersLeftInRegion( Mobile from, Region region )
		{
			bool occupied = false;

			foreach ( NetState state in NetState.Instances )
			{
				Mobile m = state.Mobile;

				if ( m != null /* && m.AccessLevel < AccessLevel.GameMaster */ && m != from && m.Region == region )
				{
					occupied = true;
				}
			}

			return occupied;
		}

		public static void MoveToRandomDungeon( Mobile m )
		{
			Point3D loc = new Point3D(0, 0, 0);
			Map map = Map.Sosaria;

			switch ( Utility.RandomMinMax( 0, 69 ) )
			{
				case 0: loc = new Point3D(5773, 2804, 0); map = Map.Lodor; break; // the Crypts of Dracula
				case 1: loc = new Point3D(5353, 91, 15); map = Map.Lodor; break; // the Mind Flayer City
				case 2: loc = new Point3D(5789, 2558, -30); map = Map.Lodor; break; // Dungeon Covetous
				case 3: loc = new Point3D(5308, 680, 0); map = Map.Lodor; break; // Dungeon Deceit
				case 4: loc = new Point3D(5185, 2442, 6); map = Map.Lodor; break; // Dungeon Despise
				case 5: loc = new Point3D(5321, 799, 0); map = Map.Lodor; break; // Dungeon Destard
				case 6: loc = new Point3D(5869, 1443, 0); map = Map.Lodor; break; // the City of Embers
				case 7: loc = new Point3D(6038, 200, 22); map = Map.Lodor; break; // Dungeon Hythloth
				case 8: loc = new Point3D(5728, 155, 1); map = Map.Lodor; break; // the Frozen Hells
				case 9: loc = new Point3D(5783, 23, 0); map = Map.Lodor; break; // Dungeon Shame
				case 10: loc = new Point3D(5174, 1703, 2); map = Map.Lodor; break; // Terathan Keep
				case 11: loc = new Point3D(5247, 436, 0); map = Map.Lodor; break; // the Halls of Undermountain
				case 12: loc = new Point3D(5859, 3427, 0); map = Map.Lodor; break; // the Volcanic Cave
				case 13: loc = new Point3D(5443, 1398, 0); map = Map.Lodor; break; // Dungeon Wrong
				case 14: loc = new Point3D(5854, 1756, 0); map = Map.Sosaria; break; // the Caverns of Poseidon
				case 15: loc = new Point3D(6387, 3754, -2); map = Map.Sosaria; break; // the Tower of Brass
				case 16: loc = new Point3D(3943, 3370, 0); map = Map.Sosaria; break; // the Mausoleum
				case 17: loc = new Point3D(6384, 490, 0); map = Map.Sosaria; break; // Vordo's Dungeon
				case 18: loc = new Point3D(7028, 3824, 5); map = Map.Sosaria; break; // the Cave of the Zuluu
				case 19: loc = new Point3D(4629, 3599, 0); map = Map.Sosaria; break; // the Dragon's Maw
				case 20: loc = new Point3D(5354, 923, 0); map = Map.Sosaria; break; // the Ancient Pyramid
				case 21: loc = new Point3D(5965, 636, 0); map = Map.Sosaria; break; // Dungeon Exodus
				case 22: loc = new Point3D(262, 3380, 0); map = Map.Sosaria; break; // the Cave of Banished Mages
				case 23: loc = new Point3D(5981, 2154, 0); map = Map.Sosaria; break; // Dungeon Clues
				case 24: loc = new Point3D(5550, 393, 0); map = Map.Sosaria; break; // Dardin's Pit
				case 25: loc = new Point3D(5259, 262, 0); map = Map.Sosaria; break; // Dungeon Doom
				case 26: loc = new Point3D(5526, 1228, 0); map = Map.Sosaria; break; // the Fires of Hell
				case 27: loc = new Point3D(5587, 1602, 0); map = Map.Sosaria; break; // the Mines of Morinia
				case 28: loc = new Point3D(5995, 423, 0); map = Map.Sosaria; break; // the Perinian Depths
				case 29: loc = new Point3D(5638, 821, 0); map = Map.Sosaria; break; // the Dungeon of Time Awaits
				case 30: loc = new Point3D(1955, 523, 0); map = Map.SerpentIsland; break; // the Ancient Prison
				case 31: loc = new Point3D(2090, 863, 0); map = Map.SerpentIsland; break; // the Cave of Fire
				case 32: loc = new Point3D(2440, 53, 2); map = Map.SerpentIsland; break; // the Cave of Souls
				case 33: loc = new Point3D(2032, 76, 0); map = Map.SerpentIsland; break; // Dungeon Ankh
				case 34: loc = new Point3D(1947, 216, 0); map = Map.SerpentIsland; break; // Dungeon Bane
				case 35: loc = new Point3D(2189, 425, 0); map = Map.SerpentIsland; break; // Dungeon Hate
				case 36: loc = new Point3D(2221, 816, 0); map = Map.SerpentIsland; break; // Dungeon Scorn
				case 37: loc = new Point3D(1957, 710, 0); map = Map.SerpentIsland; break; // Dungeon Torment
				case 38: loc = new Point3D(2361, 403, 0); map = Map.SerpentIsland; break; // Dungeon Vile
				case 39: loc = new Point3D(2160, 173, 2); map = Map.SerpentIsland; break; // Dungeon Wicked
				case 40: loc = new Point3D(2311, 912, 2); map = Map.SerpentIsland; break; // Dungeon Wrath
				case 41: loc = new Point3D(2459, 880, 0); map = Map.SerpentIsland; break; // the Flooded Temple
				case 42: loc = new Point3D(2064, 509, 0); map = Map.SerpentIsland; break; // the Gargoyle Crypts
				case 43: loc = new Point3D(2457, 506, 0); map = Map.SerpentIsland; break; // the Serpent Sanctum
				case 44: loc = new Point3D(2327, 183, 2); map = Map.SerpentIsland; break; // the Tomb of the Fallen Wizard
				case 45: loc = new Point3D(729, 2635, -28); map = Map.SavagedEmpire; break; // the Blood Temple
				case 46: loc = new Point3D(774, 1984, -28); map = Map.SavagedEmpire; break; // the Dungeon of the Mad Archmage
				case 47: loc = new Point3D(51, 2619, -28); map = Map.SavagedEmpire; break; // the Tombs
				case 48: loc = new Point3D(342, 2296, -1); map = Map.SavagedEmpire; break; // the Dungeon of the Lich King
				case 49: loc = new Point3D(323, 2836, 0); map = Map.SavagedEmpire; break; // the Ice Queen Fortress
				case 50: loc = new Point3D(1143, 2403, -28); map = Map.SavagedEmpire; break; // the Halls of Ogrimar
				case 51: loc = new Point3D(692, 2319, -27); map = Map.SavagedEmpire; break; // Dungeon Rock
				case 52: loc = new Point3D(100, 3389, 0); map = Map.SavagedEmpire; break; // Forgotten Halls
				case 53: loc = new Point3D(366, 3886, 0); map = Map.SavagedEmpire; break; // the Scurvy Reef
				case 54: loc = new Point3D(647, 3860, 39); map = Map.SavagedEmpire; break; // the Undersea Castle
				case 55: loc = new Point3D(231, 3650, 25); map = Map.SavagedEmpire; break; // the Azure Castle
				case 56: loc = new Point3D(436, 3311, 20); map = Map.SavagedEmpire; break; // the Tomb of Kazibal
				case 57: loc = new Point3D(670, 3357, 20); map = Map.SavagedEmpire; break; // the Catacombs of Azerok
				case 58: loc = new Point3D(6035, 2574, 0); map = Map.Lodor; break; // Stonegate Castle
				case 59: loc = new Point3D(1968, 1363, 61); map = Map.Underworld; break; // the Glacial Scar
				case 60: loc = new Point3D(6142, 3660, -20); map = Map.Lodor; break; // the Temple of Osirus
				case 61: loc = new Point3D(1851, 1233, -42); map = Map.Underworld; break; // the Stygian Abyss
				case 62: loc = new Point3D(6413, 2004, -40); map = Map.Lodor; break; // the Daemon's Crag
				case 63: loc = new Point3D(7003, 2437, -11); map = Map.Lodor; break; // the Zealan Tombs
				case 64: loc = new Point3D(6368, 968, 25); map = Map.Lodor; break; // the Hall of the Mountain King
				case 65: loc = new Point3D(6826, 1123, -92); map = Map.Lodor; break; // Morgaelin's Inferno
				case 66: loc = new Point3D(5950, 1654, -5); map = Map.Lodor; break; // the Depths of Carthax Lake
				case 67: loc = new Point3D(5989, 484, 1); map = Map.Lodor; break; // Argentrock Castle
				case 68: loc = new Point3D(6021, 1968, 0); map = Map.Lodor; break; // the Sanctum of Saltmarsh
				case 69: loc = new Point3D(1125, 3684, 0); map = Map.Lodor; break; // the Ancient Sky Ship
			}

			if ( m is PlayerMobile )
			{
				Server.Mobiles.BaseCreature.TeleportPets( m, loc, map );
				m.MoveToWorld( loc, map );
			}
			else if ( m is BaseCreature )
			{
				m.MoveToWorld( loc, map );
			}
		}

		public static void MoveToRandomOcean( Mobile m )
		{
			Point3D loc = new Point3D(20, 20, 0);
			Map map = Map.Sosaria;
			Land land = Land.Sosaria;

			switch ( Utility.RandomMinMax( 0, 8 ) )
			{
				case 0: land = Land.Kuldar;			map = Map.Sosaria;			break;
				case 1: land = Land.Ambrosia;		map = Map.Sosaria;			break;
				case 2: land = Land.UmberVeil;		map = Map.Sosaria;			break;
				case 3: land = Land.Lodoria;		map = Map.Lodor;			break;
				case 4: land = Land.Underworld;		map = Map.Underworld;		break;
				case 5: land = Land.Serpent;		map = Map.SerpentIsland;	break;
				case 6: land = Land.IslesDread;		map = Map.IslesDread;		break;
				case 7: land = Land.Savaged;		map = Map.SavagedEmpire;	break;
				case 8: land = Land.Sosaria;		map = Map.Sosaria;			break;
			}

			loc = GetRandomLocation( land, "ocean" );

			if ( m is PlayerMobile )
			{
				Server.Mobiles.BaseCreature.TeleportPets( m, loc, map );
				m.MoveToWorld( loc, map );
			}
			else if ( m is BaseCreature )
			{
				m.MoveToWorld( loc, map );
			}
		}

		public static Point3D GetRandomDungeonSpot( Map map )
		{
			Point3D loc = new Point3D(0, 0, 0);
			int aCount = 0;
			ArrayList targets = new ArrayList();
			foreach ( Item target in World.Items.Values )
			{
				if ( target is DungeonChest || target is DungeonChestSpawner && target.Map == map && Server.Difficult.GetDifficulty( target.Location, target.Map ) > 0 )
				{
					Region reg = Region.Find( target.Location, target.Map );
					if ( reg.IsPartOf( typeof( DungeonRegion ) ) )
					{
						targets.Add( target );
						aCount++;
					}
				}
			}
			aCount = Utility.RandomMinMax( 1, aCount );
			int xCount = 0;
			for ( int i = 0; i < targets.Count; ++i )
			{
				xCount++;

				if ( xCount == aCount )
				{
					Item finding = ( Item )targets[ i ];
					loc = finding.Location;
				}
			}
			return loc;
		}

        public static string GetAreaEntrance( int exact, string zone, Map map, out Map place, out int x, out int y )
        {
			// THIS RETURNS THE COORDINATES AND MAP OF THE DUNGEON ENTRANCE

			Point3D loc = new Point3D(0, 0, 0);

			if ( exact >= 1 && exact <= 24 || exact == 90 || exact == 91 ){ map = Map.Sosaria; }
			else if ( exact >= 25 && exact <= 44 || exact == 86 ){ map = Map.Lodor; }
			else if ( exact >= 45 && exact <= 56 || exact == 87 || exact == 88 || exact == 89 ){ map = Map.SavagedEmpire; }
			else if ( exact >= 57 && exact <= 71 ){ map = Map.SerpentIsland; }
			else if ( exact == 72 || exact == 73 || exact == 74 || exact == 85 ){ map = Map.SavagedEmpire; }
			else if ( exact == 75 || exact == 82 ){ map = Map.Underworld; }
			else if ( exact >= 76 && exact <= 84 ){ map = Map.Lodor; }

			if ( ( exact == 1 || zone == "the City of the Dead" ) && map == Map.Sosaria ){ loc = new Point3D(5828, 3263, 0); zone = "the City of the Dead"; }
			else if ( ( exact == 2 || zone == "the Mausoleum" ) && map == Map.Sosaria ){ loc = new Point3D(1529, 3599, 0); zone = "the Mausoleum"; }
			else if ( ( exact == 3 || zone == "the Valley of Dark Druids" ) && map == Map.Sosaria ){ loc = new Point3D(6763, 1423, 2); zone = "the Valley of Dark Druids"; }
			else if ( ( exact == 4 || zone == "Vordo's Castle" ) && map == Map.Sosaria ){ loc = new Point3D(6708, 1729, 25); zone = "Vordo's Castle"; }
			else if ( ( exact == 5 || zone == "Vordo's Dungeon" ) && map == Map.Sosaria ){ loc = new Point3D(6708, 1729, 25); zone = "Vordo's Dungeon"; }
			else if ( ( exact == 6 || zone == "the Crypts of Kuldar" ) && map == Map.Sosaria ){ loc = new Point3D(6668, 1568, 10); zone = "the Crypts of Kuldar"; }
			else if ( ( exact == 7 || zone == "the Kuldara Sewers" ) && map == Map.Sosaria ){ loc = new Point3D(6790, 1745, 24); zone = "the Kuldara Sewers"; }
			else if ( ( exact == 8 || zone == "the Ancient Pyramid" ) && map == Map.Sosaria ){ loc = new Point3D(1162, 472, 0); zone = "the Ancient Pyramid"; }
			else if ( ( exact == 9 || zone == "Dungeon Exodus" ) && map == Map.Sosaria ){ loc = new Point3D(877, 2702, 0); zone = "Dungeon Exodus"; }
			else if ( ( exact == 10 || zone == "the Cave of Banished Mages" ) && map == Map.Sosaria ){ loc = new Point3D(3798, 1879, 2); zone = "the Cave of Banished Mages"; }
			else if ( ( exact == 11 || zone == "Dungeon Clues" ) && map == Map.Sosaria ){ loc = new Point3D(3760, 2038, 0); zone = "Dungeon Clues"; }
			else if ( ( exact == 12 || zone == "Dardin's Pit" ) && map == Map.Sosaria ){ loc = new Point3D(3006, 446, 0); zone = "Dardin's Pit"; }
			else if ( ( exact == 13 || zone == "Dungeon Doom" ) && map == Map.Sosaria ){ loc = new Point3D(1628, 2561, 0); zone = "Dungeon Doom"; }
			else if ( ( exact == 14 || zone == "the Fires of Hell" ) && map == Map.Sosaria ){ loc = new Point3D(3345, 1647, 0); zone = "the Fires of Hell"; }
			else if ( ( exact == 15 || zone == "the Mines of Morinia" ) && map == Map.Sosaria ){ loc = new Point3D(1022, 1369, 2); zone = "the Mines of Morinia"; }
			else if ( ( exact == 16 || zone == "the Perinian Depths" ) && map == Map.Sosaria ){ loc = new Point3D(3619, 456, 0); zone = "the Perinian Depths"; }
			else if ( ( exact == 17 || zone == "the Dungeon of Time Awaits" ) && map == Map.Sosaria ){ loc = new Point3D(3831, 1494, 0); zone = "the Dungeon of Time Awaits"; }
			else if ( ( exact == 18 || zone == "the Pirate Cave" ) && map == Map.Sosaria ){ loc = new Point3D(1842, 2211, 0); zone = "the Pirate Cave"; }
			else if ( ( exact == 19 || zone == "the Dragon's Maw" ) && map == Map.Sosaria ){ loc = new Point3D(5315, 3430, 2); zone = "the Dragon's Maw"; }
			else if ( ( exact == 20 || zone == "the Cave of the Zuluu" ) && map == Map.Sosaria ){ loc = new Point3D(5901, 3999, 0); zone = "the Cave of the Zuluu"; }
			else if ( ( exact == 21 || zone == "the Ratmen Lair" ) && map == Map.Sosaria ){ loc = new Point3D(1303, 1458, 0); zone = "the Ratmen Lair"; }
			else if ( ( exact == 22 || zone == "the Caverns of Poseidon" ) && map == Map.Sosaria ){ loc = new Point3D(198, 2295, 12); zone = "the Caverns of Poseidon"; }
			else if ( ( exact == 23 || zone == "the Tower of Brass" ) && map == Map.Sosaria ){ loc = new Point3D(1593, 3376, 15); zone = "the Tower of Brass"; }
			else if ( ( exact == 24 || zone == "the Forgotten Halls" ) && map == Map.Sosaria ){ loc = new Point3D(3015, 944, 0); zone = "the Forgotten Halls"; }

			else if ( ( exact == 25 || zone == "the Vault of the Black Knight" ) && map == Map.Lodor ){ loc = new Point3D(1581, 202, 0); map = Map.SerpentIsland; zone = "the Vault of the Black Knight"; }
			else if ( ( exact == 26 || zone == "the Undersea Pass" ) && map == Map.Lodor ){ loc = new Point3D(1179, 1931, 0); zone = "the Undersea Pass"; }
			else if ( ( exact == 27 || zone == "the Castle of Dracula" ) && map == Map.Lodor ){ loc = new Point3D(466, 3794, 0); zone = "the Castle of Dracula"; }
			else if ( ( exact == 28 || zone == "the Crypts of Dracula" ) && map == Map.Lodor ){ loc = new Point3D(466, 3794, 0); zone = "the Crypts of Dracula"; }
			else if ( ( exact == 29 || zone == "the Lodoria Catacombs" ) && map == Map.Lodor ){ loc = new Point3D(1869, 2378, 0); zone = "the Lodoria Catacombs"; }
			else if ( ( exact == 30 || zone == "Dungeon Covetous" ) && map == Map.Lodor ){ loc = new Point3D(4019, 2436, 2); zone = "Dungeon Covetous"; }
			else if ( ( exact == 31 || zone == "Dungeon Deceit" ) && map == Map.Lodor ){ loc = new Point3D(2523, 757, 1); zone = "Dungeon Deceit"; }
			else if ( ( exact == 32 || zone == "Dungeon Despise" ) && map == Map.Lodor ){ loc = new Point3D(1278, 1852, 0); zone = "Dungeon Despise"; }
			else if ( ( exact == 33 || zone == "Dungeon Destard" ) && map == Map.Lodor ){ loc = new Point3D(749, 630, 0); zone = "Dungeon Destard"; }
			else if ( ( exact == 34 || zone == "the City of Embers" ) && map == Map.Lodor ){ loc = new Point3D(3196, 3318, 0); zone = "the City of Embers"; }
			else if ( ( exact == 35 || zone == "Dungeon Hythloth" ) && map == Map.Lodor ){ loc = new Point3D(1634, 2805, 0); zone = "Dungeon Hythloth"; }
			else if ( ( exact == 36 || zone == "the Frozen Hells" ) && map == Map.Lodor ){ loc = new Point3D(3769, 1092, 0); zone = "the Frozen Hells"; }
			else if ( ( exact == 37 || zone == "the Ice Fiend Lair" ) && map == Map.Lodor ){ loc = new Point3D(3769, 1092, 0); zone = "the Ice Fiend Lair"; }
			else if ( ( exact == 38 || zone == "the Halls of Undermountain" ) && map == Map.Lodor ){ loc = new Point3D(959, 2669, 5); zone = "the Halls of Undermountain"; }
			else if ( ( exact == 39 || zone == "Dungeon Shame" ) && map == Map.Lodor ){ loc = new Point3D(1405, 2338, 0); zone = "Dungeon Shame"; }
			else if ( ( exact == 40 || zone == "Terathan Keep" ) && map == Map.Lodor ){ loc = new Point3D(624, 2403, 2); zone = "Terathan Keep"; }
			else if ( ( exact == 41 || zone == "the Volcanic Cave" ) && map == Map.Lodor ){ loc = new Point3D(3105, 3594, 0); zone = "the Volcanic Cave"; }
			else if ( ( exact == 42 || zone == "Dungeon Wrong" ) && map == Map.Lodor ){ loc = new Point3D(2252, 854, 1); zone = "Dungeon Wrong"; }
			else if ( ( exact == 43 || zone == "Stonegate Castle" ) && map == Map.Lodor ){ loc = new Point3D(1355, 404, 0); zone = "Stonegate Castle"; }
			else if ( ( exact == 44 || zone == "the Ancient Elven Mine" ) && map == Map.Lodor ){ loc = new Point3D(1179, 1931, 0); zone = "the Ancient Elven Mine"; }

			else if ( ( exact == 45 || zone == "the Dungeon of the Mad Archmage" ) && map == Map.SavagedEmpire ){ loc = new Point3D(464, 851, -60); zone = "the Dungeon of the Mad Archmage"; }
			else if ( ( exact == 46 || zone == "the Dungeon of the Lich King" ) && map == Map.SavagedEmpire ){ loc = new Point3D(922, 1772, 26); zone = "the Dungeon of the Lich King"; }
			else if ( ( exact == 47 || zone == "the Halls of Ogrimar" ) && map == Map.SavagedEmpire ){ loc = new Point3D(1107, 1380, 17); zone = "the Halls of Ogrimar"; }
			else if ( ( exact == 48 || zone == "the Ratmen Mines" ) && map == Map.SavagedEmpire ){ loc = new Point3D(157, 1369, 32); zone = "the Ratmen Mines"; }
			else if ( ( exact == 49 || zone == "Dungeon Rock" ) && map == Map.SavagedEmpire ){ loc = new Point3D(1092, 1038, 0); zone = "Dungeon Rock"; }
			else if ( ( exact == 50 || zone == "the Storm Giant Lair" ) && map == Map.SavagedEmpire ){ loc = new Point3D(283, 466, 14); zone = "the Storm Giant Lair"; }
			else if ( ( exact == 51 || zone == "the Corrupt Pass" ) && map == Map.SavagedEmpire ){ loc = new Point3D(155, 1125, 60); zone = "the Corrupt Pass"; }
			else if ( ( exact == 52 || zone == "the Tombs" ) && map == Map.SavagedEmpire ){ loc = new Point3D(222, 1361, 0); zone = "the Tombs"; }
			else if ( ( exact == 53 || zone == "the Undersea Castle" ) && map == Map.SavagedEmpire ){ loc = new Point3D(283, 409, 20); zone = "the Undersea Castle"; }
			else if ( ( exact == 54 || zone == "the Azure Castle" ) && map == Map.SavagedEmpire ){ loc = new Point3D(774, 612, 15); zone = "the Azure Castle"; }
			else if ( ( exact == 55 || zone == "the Tomb of Kazibal" ) && map == Map.SavagedEmpire ){ loc = new Point3D(368, 298, 57); zone = "the Tomb of Kazibal"; }
			else if ( ( exact == 56 || zone == "the Catacombs of Azerok" ) && map == Map.SavagedEmpire ){ loc = new Point3D(1056, 424, 38); zone = "the Catacombs of Azerok"; }

			else if ( ( exact == 57 || zone == "the Ancient Prison" ) && map == Map.SerpentIsland ){ loc = new Point3D(748, 846, 1); zone = "the Ancient Prison"; }
			else if ( ( exact == 58 || zone == "the Cave of Fire" ) && map == Map.SerpentIsland ){ loc = new Point3D(561, 1143, 0); zone = "the Cave of Fire"; }
			else if ( ( exact == 59 || zone == "the Cave of Souls" ) && map == Map.SerpentIsland ){ loc = new Point3D(121, 1475, 0); zone = "the Cave of Souls"; }
			else if ( ( exact == 60 || zone == "Dungeon Ankh" ) && map == Map.SerpentIsland ){ loc = new Point3D(465, 1435, 2); zone = "Dungeon Ankh"; }
			else if ( ( exact == 61 || zone == "Dungeon Bane" ) && map == Map.SerpentIsland ){ loc = new Point3D(310, 761, 2); zone = "Dungeon Bane"; }
			else if ( ( exact == 62 || zone == "Dungeon Hate" ) && map == Map.SerpentIsland ){ loc = new Point3D(1459, 1220, 0); zone = "Dungeon Hate"; }
			else if ( ( exact == 63 || zone == "Dungeon Scorn" ) && map == Map.SerpentIsland ){ loc = new Point3D(1463, 873, 2); zone = "Dungeon Scorn"; }
			else if ( ( exact == 64 || zone == "Dungeon Torment" ) && map == Map.SerpentIsland ){ loc = new Point3D(1690, 1225, 0); zone = "Dungeon Torment"; }
			else if ( ( exact == 65 || zone == "Dungeon Vile" ) && map == Map.SerpentIsland ){ loc = new Point3D(1554, 991, 2); zone = "Dungeon Vile"; }
			else if ( ( exact == 66 || zone == "Dungeon Wicked" ) && map == Map.SerpentIsland ){ loc = new Point3D(733, 260, 0); zone = "Dungeon Wicked"; }
			else if ( ( exact == 67 || zone == "Dungeon Wrath" ) && map == Map.SerpentIsland ){ loc = new Point3D(1803, 918, 0); zone = "Dungeon Wrath"; }
			else if ( ( exact == 68 || zone == "the Flooded Temple" ) && map == Map.SerpentIsland ){ loc = new Point3D(1069, 952, 2); zone = "the Flooded Temple"; }
			else if ( ( exact == 69 || zone == "the Gargoyle Crypts" ) && map == Map.SerpentIsland ){ loc = new Point3D(1267, 936, 0); zone = "the Gargoyle Crypts"; }
			else if ( ( exact == 70 || zone == "the Serpent Sanctum" ) && map == Map.SerpentIsland ){ loc = new Point3D(1093, 1609, 0); zone = "the Serpent Sanctum"; }
			else if ( ( exact == 71 || zone == "the Tomb of the Fallen Wizard" ) && map == Map.SerpentIsland ){ loc = new Point3D(1056, 1338, 0); zone = "the Tomb of the Fallen Wizard"; }

			else if ( ( exact == 72 || zone == "the Blood Temple" ) && map == Map.SavagedEmpire ){ loc = new Point3D(1258, 1231, 0); map = Map.IslesDread; zone = "the Blood Temple"; }
			else if ( ( exact == 73 || zone == "the Ice Queen Fortress" ) && map == Map.SavagedEmpire ){ loc = new Point3D(319, 324, 5); map = Map.IslesDread; zone = "the Ice Queen Fortress"; }
			else if ( ( exact == 74 || zone == "the Scurvy Reef" ) && map == Map.SavagedEmpire ){ loc = new Point3D(713, 493, 1); map = Map.IslesDread; zone = "the Scurvy Reef"; }
			else if ( ( exact == 75 || zone == "the Glacial Scar" ) && map == Map.Underworld ){ loc = new Point3D(238, 171, 0); map = Map.IslesDread; zone = "the Glacial Scar"; }
			else if ( ( exact == 76 || zone == "the Temple of Osirus" ) && map == Map.Lodor ){ loc = new Point3D(601, 819, 20); map = Map.IslesDread; zone = "the Temple of Osirus"; }
			else if ( ( exact == 77 || zone == "the Sanctum of Saltmarsh" ) && map == Map.Lodor ){ loc = new Point3D(926, 874, 0); map = Map.IslesDread; zone = "the Sanctum of Saltmarsh"; }

			else if ( ( exact == 78 || zone == "Morgaelin's Inferno" ) && map == Map.Lodor ){ loc = new Point3D(1459, 100, 0); map = Map.Underworld; zone = "Morgaelin's Inferno"; }
			else if ( ( exact == 79 || zone == "the Zealan Tombs" ) && map == Map.Lodor ){ loc = new Point3D(1094, 1229, 0); map = Map.Underworld; zone = "the Zealan Tombs"; }
			else if ( ( exact == 80 || zone == "Argentrock Castle" ) && map == Map.Lodor ){ loc = new Point3D(103, 999, 36); map = Map.Underworld; zone = "Argentrock Castle"; }
			else if ( ( exact == 81 || zone == "the Daemon's Crag" ) && map == Map.Lodor ){ loc = new Point3D(1481, 835, 0); map = Map.Underworld; zone = "the Daemon's Crag"; }
			else if ( ( exact == 82 || zone == "the Stygian Abyss" ) && map == Map.Underworld ){ loc = new Point3D(824, 907, 0); zone = "the Stygian Abyss"; }
			else if ( ( exact == 83 || zone == "the Hall of the Mountain King" ) && map == Map.Lodor ){ loc = new Point3D(130, 102, 0); map = Map.Underworld; zone = "the Hall of the Mountain King"; }
			else if ( ( exact == 84 || zone == "the Depths of Carthax Lake" ) && map == Map.Lodor ){ loc = new Point3D(118, 1582, 0); map = Map.Underworld; zone = "the Depths of Carthax Lake"; }
			else if ( ( exact == 85 || zone == "the Ancient Sky Ship" ) && map == Map.SavagedEmpire ){ loc = new Point3D(66, 561, 0); map = Map.Underworld; zone = "the Ancient Sky Ship"; }

			else if ( ( exact == 86 || zone == "the Lizardman Cave" ) && map == Map.Lodor ){ loc = new Point3D(960, 2667, 0); map = Map.Lodor; zone = "the Lizardman Cave"; }
			else if ( ( exact == 87 || zone == "the Cave of the Ancient Wyrm" ) && map == Map.SavagedEmpire ){ loc = new Point3D(774, 611, 0); map = Map.SavagedEmpire; zone = "the Cave of the Ancient Wyrm"; }
			else if ( ( exact == 88 || zone == "the Great Pyramid" ) && map == Map.SavagedEmpire ){ loc = new Point3D(1131, 1550, 0); map = Map.SavagedEmpire; zone = "the Great Pyramid"; }
			else if ( ( exact == 89 || zone == "the Spider Cave" ) && map == Map.SavagedEmpire ){ loc = new Point3D(1129, 1551, 0); map = Map.SavagedEmpire; zone = "the Spider Cave"; }
			else if ( ( exact == 90 || zone == "the Ruins of the Black Blade" ) && map == Map.Sosaria ){ loc = new Point3D(4504, 1892, 0); map = Map.Sosaria; zone = "the Ruins of the Black Blade"; }
			else if ( ( exact == 91 || zone == "the Montor Sewers" ) && map == Map.Sosaria ){ loc = new Point3D(3058, 2565, 0); map = Map.Sosaria; zone = "the Montor Sewers"; }

			place = map;
			x = loc.X;
			y = loc.Y;

			string my_location = "";

			int xLong = 0, yLat = 0;
			int xMins = 0, yMins = 0;
			bool xEast = false, ySouth = false;

			if ( Sextant.Format( loc, map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
				my_location = String.Format( "{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W" );

			if ( exact > 0 )
				return zone;

            return my_location;
        }

        public static string GetTown( int exact, string zone, out Map place, out int x, out int y )
        {
			// THIS RETURNS THE COORDINATES AND MAP OF THE DUNGEON ENTRANCE

			Point3D loc = new Point3D(0, 0, 0);
			Map map = Map.Internal;

			if ( exact == 1 || zone == "the City of Britain" ){ loc = new Point3D(2999, 1063, 0); map = Map.Sosaria; zone = "the City of Britain"; }
			else if ( exact == 2 || zone == "the City of Kuldara" ){ loc = new Point3D(6773, 1739, 20); map = Map.Sosaria; zone = "the City of Kuldara"; }
			else if ( exact == 3 || zone == "the Cimmeran Hold" ){ loc = new Point3D(384, 1086, 15); map = Map.IslesDread; zone = "the Cimmeran Hold"; }
			else if ( exact == 4 || zone == "the Fort of Tenebrae" ){ loc = new Point3D(756, 384, 0); map = Map.Underworld; zone = "the Fort of Tenebrae"; }
			else if ( exact == 5 || zone == "the Town of Renika" ){ loc = new Point3D(1449, 3787, 0); map = Map.Sosaria; zone = "the Town of Renika"; }
			else if ( exact == 6 || zone == "the City of Furnace" ){ loc = new Point3D(804, 1121, 43); map = Map.SerpentIsland; zone = "the City of Furnace"; }
			else if ( exact == 7 || zone == "the Village of Barako" ){ loc = new Point3D(285, 1698, 37); map = Map.SavagedEmpire; zone = "the Village of Barako"; }
			else if ( exact == 8 || zone == "the Village of Kurak" ){ loc = new Point3D(741, 914, -1); map = Map.SavagedEmpire; zone = "the Village of Kurak"; }
			else if ( exact == 9 || zone == "Death Gulch" ){ loc = new Point3D(3717, 1501, 0); map = Map.Sosaria; zone = "Death Gulch"; }
			else if ( exact == 10 || zone == "the Town of Devil Guard" ){ loc = new Point3D(1652, 1557, 2); map = Map.Sosaria; zone = "the Town of Devil Guard"; }
			else if ( exact == 11 || zone == "the Village of Fawn" ){ loc = new Point3D(2124, 276, 0); map = Map.Sosaria; zone = "the Village of Fawn"; }
			else if ( exact == 12 || zone == "Glacial Coast Village" ){ loc = new Point3D(4762, 1177, 2); map = Map.Sosaria; zone = "Glacial Coast Village"; }
			else if ( exact == 13 || zone == "the Village of Grey" ){ loc = new Point3D(902, 2063, 0); map = Map.Sosaria; zone = "the Village of Grey"; }
			else if ( exact == 14 || zone == "Iceclad Fisherman's Village" ){ loc = new Point3D(4326, 1169, 2); map = Map.Sosaria; zone = "Iceclad Fisherman's Village"; }
			else if ( exact == 15 || zone == "the City of Montor" ){ loc = new Point3D(3223, 2606, 1); map = Map.Sosaria; zone = "the City of Montor"; }
			else if ( exact == 16 || zone == "the Town of Moon" ){ loc = new Point3D(806, 728, 0); map = Map.Sosaria; zone = "the Town of Moon"; }
			else if ( exact == 17 || zone == "the Town of Mountain Crest" ){ loc = new Point3D(4514, 1276, 2); map = Map.Sosaria; zone = "the Town of Mountain Crest"; }
			else if ( exact == 18 || zone == "the Village of Yew" ){ loc = new Point3D(2433, 873, 2); map = Map.Sosaria; zone = "the Village of Yew"; }
			else if ( exact == 19 || zone == "the Port of Dusk" ){ loc = new Point3D(2675, 3202, 0); map = Map.Lodor; zone = "the Port of Dusk"; }
			else if ( exact == 20 || zone == "the City of Elidor" ){ loc = new Point3D(2930, 1327, 0); map = Map.Lodor; zone = "the City of Elidor"; }
			else if ( exact == 21 || zone == "the Town of Glacial Hills" ){ loc = new Point3D(3677, 419, 0); map = Map.Lodor; zone = "the Town of Glacial Hills"; }
			else if ( exact == 22 || zone == "Greensky Village" ){ loc = new Point3D(4228, 2996, 0); map = Map.Lodor; zone = "Greensky Village"; }
			else if ( exact == 23 || zone == "the Village of Islegem" ){ loc = new Point3D(2828, 2249, 1); map = Map.Lodor; zone = "the Village of Islegem"; }
			else if ( exact == 24 || zone == "the City of Lodoria" ){ loc = new Point3D(1912, 2209, 0); map = Map.Lodor; zone = "the City of Lodoria"; }
			else if ( exact == 25 || zone == "the Village of Portshine" ){ loc = new Point3D(845, 2026, 0); map = Map.Lodor; zone = "the Village of Portshine"; }
			else if ( exact == 26 || zone == "the Village of Springvale" ){ loc = new Point3D(4223, 1459, 0); map = Map.Lodor; zone = "the Village of Springvale"; }
			else if ( exact == 27 || zone == "the Port of Starguide" ){ loc = new Point3D(2335, 3160, 0); map = Map.Lodor; zone = "the Port of Starguide"; }
			else if ( exact == 28 || zone == "the Village of Whisper" ){ loc = new Point3D(891, 947, 0); map = Map.Lodor; zone = "the Village of Whisper"; }

			place = map;
			x = loc.X;
			y = loc.Y;

			string my_location = "";

			int xLong = 0, yLat = 0;
			int xMins = 0, yMins = 0;
			bool xEast = false, ySouth = false;

			if ( Sextant.Format( loc, map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
				my_location = String.Format( "{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W" );

			if ( exact > 0 )
				return zone;

            return my_location;
        }

        public static string GetDungeonListing( int i, out string world, out string location, out Map placer, out int xc, out int yc )
        {
			string dungeon = null;
			switch( i )
			{
				case 1: dungeon = "Dardin's Pit"; break;
				case 2: dungeon = "Dungeon Clues"; break;
				case 3: dungeon = "Dungeon Doom"; break;
				case 4: dungeon = "Dungeon Exodus"; break;
				case 5: dungeon = "the Pirate Cave"; break;
				case 6: dungeon = "the Ancient Pyramid"; break;
				case 7: dungeon = "the Cave of Banished Mages"; break;
				case 8: dungeon = "the Caverns of Poseidon"; break;
				case 9: dungeon = "the Dungeon of Time Awaits"; break;
				case 10: dungeon = "the Fires of Hell"; break;
				case 11: dungeon = "the Forgotten Halls"; break;
				case 12: dungeon = "the Mines of Morinia"; break;
				case 13: dungeon = "the Perinian Depths"; break;
				case 14: dungeon = "the Ratmen Lair"; break;
				case 15: dungeon = "the Cave of the Zuluu"; break;
				case 16: dungeon = "the City of the Dead"; break;
				case 17: dungeon = "the Dragon's Maw"; break;
				case 18: dungeon = "the Mausoleum"; break;
				case 19: dungeon = "the Tower of Brass"; break;
				case 20: dungeon = "Dungeon Covetous"; break;
				case 21: dungeon = "Dungeon Deceit"; break;
				case 22: dungeon = "Dungeon Despise"; break;
				case 23: dungeon = "Dungeon Destard"; break;
				case 24: dungeon = "Dungeon Hythloth"; break;
				case 25: dungeon = "Dungeon Shame"; break;
				case 26: dungeon = "Dungeon Wrong"; break;
				case 27: dungeon = "Stonegate Castle"; break;
				case 28: dungeon = "Terathan Keep"; break;
				case 29: dungeon = "the Ancient Elven Mine"; break;
				case 30: dungeon = "the Castle of Dracula"; break;
				case 31: dungeon = "the City of Embers"; break;
				case 32: dungeon = "the Crypts of Dracula"; break;
				case 33: dungeon = "the Frozen Hells"; break;
				case 34: dungeon = "the Halls of Undermountain"; break;
				case 35: dungeon = "the Ice Fiend Lair"; break;
				case 36: dungeon = "the Lodoria Catacombs"; break;
				case 37: dungeon = "the Undersea Pass"; break;
				case 38: dungeon = "the Volcanic Cave"; break;
				case 39: dungeon = "Dungeon Ankh"; break;
				case 40: dungeon = "Dungeon Bane"; break;
				case 41: dungeon = "Dungeon Hate"; break;
				case 42: dungeon = "Dungeon Scorn"; break;
				case 43: dungeon = "Dungeon Torment"; break;
				case 44: dungeon = "Dungeon Vile"; break;
				case 45: dungeon = "Dungeon Wicked"; break;
				case 46: dungeon = "Dungeon Wrath"; break;
				case 47: dungeon = "the Ancient Prison"; break;
				case 48: dungeon = "the Cave of Fire"; break;
				case 49: dungeon = "the Cave of Souls"; break;
				case 50: dungeon = "the Flooded Temple"; break;
				case 51: dungeon = "the Gargoyle Crypts"; break;
				case 52: dungeon = "the Serpent Sanctum"; break;
				case 53: dungeon = "the Tomb of the Fallen Wizard"; break;
				case 54: dungeon = "the Vault of the Black Knight"; break;
				case 55: dungeon = "the Blood Temple"; break;
				case 56: dungeon = "the Glacial Scar"; break;
				case 57: dungeon = "the Ice Queen Fortress"; break;
				case 58: dungeon = "the Sanctum of Saltmarsh"; break;
				case 59: dungeon = "the Scurvy Reef"; break;
				case 60: dungeon = "the Temple of Osirus"; break;
				case 61: dungeon = "Dungeon of the Lich King"; break;
				case 62: dungeon = "Dungeon of the Mad Archmage"; break;
				case 63: dungeon = "Dungeon Rock"; break;
				case 64: dungeon = "the Azure Castle"; break;
				case 65: dungeon = "the Catacombs of Azerok"; break;
				case 66: dungeon = "the Corrupt Pass"; break;
				case 67: dungeon = "the Halls of Ogrimar"; break;
				case 68: dungeon = "the Ratmen Mines"; break;
				case 69: dungeon = "the Storm Giant Lair"; break;
				case 70: dungeon = "the Tomb of Kazibal"; break;
				case 71: dungeon = "the Tombs"; break;
				case 72: dungeon = "the Undersea Castle"; break;
				case 73: dungeon = "the Crypts of Kuldar"; break;
				case 74: dungeon = "the Kuldara Sewers"; break;
				case 75: dungeon = "the Valley of Dark Druids"; break;
				case 76: dungeon = "Vordo's Castle"; break;
				case 77: dungeon = "Vordo's Dungeon"; break;
				case 78: dungeon = "Argentrock Castle"; break;
				case 79: dungeon = "the Ancient Sky Ship"; break;
				case 80: dungeon = "Morgaelin's Inferno"; break;
				case 81: dungeon = "the Daemon's Crag"; break;
				case 82: dungeon = "the Depths of Carthax Lake"; break;
				case 83: dungeon = "the Hall of the Mountain King"; break;
				case 84: dungeon = "the Stygian Abyss"; break;
				case 85: dungeon = "the Zealan Tombs"; break;
			}

        	return GetDungeonListing( dungeon, out world, out location, out placer, out xc, out yc );
        }

        public static string GetDungeonListing( string dungeon, out string world, out string location, out Map placer, out int xc, out int yc )
        {
			// THIS RETURNS AN ALPHABETICAL LIST (BY WORLD) OF DUNGEONS & LOCATIONS

			world = "";
			location = "";
			placer = Map.Internal;
			xc = 0;
			yc = 0;

			switch(dungeon)
			{
				// Sosaria
				case "Dardin's Pit": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "Dungeon Clues": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "Dungeon Doom": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "Dungeon Exodus": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "the Pirate Cave": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "the Ancient Pyramid": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "the Cave of Banished Mages": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "the Caverns of Poseidon": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "the Dungeon of Time Awaits": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "the Fires of Hell": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "the Forgotten Halls": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "the Mines of Morinia": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "the Perinian Depths": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "the Ratmen Lair": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "the Montor Sewers": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;
				case "the Ruins of the Black Blade": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Sosaria"; break;

				// Ambrosia
				case "the Cave of the Zuluu": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Ambrosia"; break;
				case "the City of the Dead": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Ambrosia"; break;
				case "the Dragon's Maw": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Ambrosia"; break;

				// Umber Veil
				case "the Mausoleum": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Umber Veil"; break;
				case "the Tower of Brass": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Umber Veil"; break;

				// Lodoria
				case "Dungeon Covetous": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "Dungeon Deceit": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "Dungeon Despise": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "Dungeon Destard": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "Dungeon Hythloth": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "Dungeon Shame": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "Dungeon Wrong": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "Stonegate Castle": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "Terathan Keep": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "the Ancient Elven Mine": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "the Castle of Dracula": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "the City of Embers": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "the Crypts of Dracula": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "the Frozen Hells": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "the Halls of Undermountain": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "the Ice Fiend Lair": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "the Lodoria Catacombs": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "the Undersea Pass": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "the Volcanic Cave": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;
				case "the Lizardman Cave": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Lodoria"; break;

				// Serpent Island
				case "Dungeon Ankh": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "Dungeon Bane": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "Dungeon Hate": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "Dungeon Scorn": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "Dungeon Torment": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "Dungeon Vile": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "Dungeon Wicked": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "Dungeon Wrath": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "the Ancient Prison": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "the Cave of Fire": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "the Cave of Souls": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "the Flooded Temple": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "the Gargoyle Crypts": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "the Serpent Sanctum": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "the Tomb of the Fallen Wizard": location = GetAreaEntrance( 0, dungeon, Map.SerpentIsland, out placer, out xc, out yc ); world = "Serpent Island"; break;
				case "the Vault of the Black Knight": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Serpent Island"; break;

				// Isles of Dread
				case "the Blood Temple": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Isles of Dread"; break;
				case "the Glacial Scar": location = GetAreaEntrance( 0, dungeon, Map.Underworld, out placer, out xc, out yc ); world = "Isles of Dread"; break;
				case "the Ice Queen Fortress": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Isles of Dread"; break;
				case "the Sanctum of Saltmarsh": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Isles of Dread"; break;
				case "the Scurvy Reef": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Isles of Dread"; break;
				case "the Temple of Osirus": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Isles of Dread"; break;

				// Savaged Empire
				case "the Dungeon of the Lich King": 
				case "Dungeon of the Lich King": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Dungeon of the Mad Archmage": 
				case "Dungeon of the Mad Archmage": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "Dungeon Rock": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Azure Castle": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Catacombs of Azerok": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Corrupt Pass": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Halls of Ogrimar": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Ratmen Mines": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Storm Giant Lair": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Tomb of Kazibal": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Tombs": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Undersea Castle": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Cave of the Ancient Wyrm": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Great Pyramid": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;
				case "the Spider Cave": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Savaged Empire"; break;

				// Kuldar
				case "the Crypts of Kuldar": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Kuldar"; break;
				case "the Kuldara Sewers": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Kuldar"; break;
				case "the Valley of Dark Druids": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Kuldar"; break;
				case "Vordo's Castle": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Kuldar"; break;
				case "Vordo's Dungeon": location = GetAreaEntrance( 0, dungeon, Map.Sosaria, out placer, out xc, out yc ); world = "Kuldar"; break;

				// Underworld
				case "Argentrock Castle": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Underworld"; break;
				case "the Ancient Sky Ship": location = GetAreaEntrance( 0, dungeon, Map.SavagedEmpire, out placer, out xc, out yc ); world = "Underworld"; break;
				case "Morgaelin's Inferno": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Underworld"; break;
				case "the Daemon's Crag": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Underworld"; break;
				case "the Depths of Carthax Lake": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Underworld"; break;
				case "the Hall of the Mountain King": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Underworld"; break;
				case "the Stygian Abyss": location = GetAreaEntrance( 0, dungeon, Map.Underworld, out placer, out xc, out yc ); world = "Underworld"; break;
				case "the Zealan Tombs": location = GetAreaEntrance( 0, dungeon, Map.Lodor, out placer, out xc, out yc ); world = "Underworld"; break;
			}

            return dungeon;
        }

		public static Point3D GetRandomLocation( Land land, string scape )
		{
            bool LandOk = false;
			Point3D loc = new Point3D(0, 0, 0);
			Point3D failover = new Point3D(0, 0, 0);
			Point3D testLocation = new Point3D(0, 0, 0);

			Map tl = Map.Sosaria;
            int tx = 0;
			int ty = 0;
			int tz = 0;
			int tm = 0;
			int r = 0;
			int swrapx = 0;
			int swrapy = 0;

			if ( scape != "land" ){ swrapx = 26; swrapy = 26; }

            while ( tm < 1 )
            {
                if (land == Land.Kuldar)
                {
					tl = Map.Sosaria;
                    tx = Utility.RandomMinMax( 6166+swrapx, 7204-swrapx );
                    ty = Utility.RandomMinMax( 829+swrapy, 2741-swrapy );
                    tz = tl.GetAverageZ(tx, ty);
					if ( scape == "land" ){ failover = new Point3D(6722, 1338, 0); } else { failover = new Point3D(6348, 1096, -5); }
                }
                else if (land == Land.Ambrosia)
                {
					tl = Map.Sosaria;
                    tx = Utility.RandomMinMax( 5160+swrapx, 6163-swrapx );
                    ty = Utility.RandomMinMax( 3036+swrapy, 4095-swrapy );
                    tz = tl.GetAverageZ(tx, ty);
					if ( scape == "land" ){ failover = new Point3D(5599, 3523, 22); } else { failover = new Point3D(5512, 3232, -5); }
                }
                else if (land == Land.UmberVeil)
                {
					tl = Map.Sosaria;
                    tx = Utility.RandomMinMax( 737+swrapx, 2310-swrapx );
                    ty = Utility.RandomMinMax( 3130+swrapy, 4095-swrapy );
                    tz = tl.GetAverageZ(tx, ty);
					if ( scape == "land" ){ failover = new Point3D(1766, 3638, 22); } else { failover = new Point3D(880, 3796, -5); }
                }
                else if (land == Land.Luna)
                {
					tl = Map.Sosaria;
                    tx = Utility.RandomMinMax( 5856+swrapx, 6164-swrapx );
                    ty = Utility.RandomMinMax( 2740+swrapy, 3018-swrapy );
                    tz = tl.GetAverageZ(tx, ty);
					if ( scape == "land" ){ failover = new Point3D(5902, 2793, 0); } else { failover = new Point3D(112, 1816, -5); }
                }
                else if (land == Land.SkaraBrae)
                {
					tl = Map.Lodor;
                    tx = Utility.RandomMinMax( 6925, 6929 );
                    ty = Utility.RandomMinMax( 133, 309 );
                    tz = tl.GetAverageZ(tx, ty);
					failover = new Point3D(6925, 218, 0);
                }
                else if (land == Land.Lodoria)
                {
					tl = Map.Lodor;
                    tx = Utility.RandomMinMax( 0+swrapx, 5157-swrapx );
                    ty = Utility.RandomMinMax( 0+swrapy, 4095-swrapy );
                    tz = tl.GetAverageZ(tx, ty);
					if ( scape == "land" ){ failover = new Point3D(1050, 2236, 0); } else { failover = new Point3D(3470, 2504, -5); }
                }
                else if (land == Land.Underworld)
                {
					tl = Map.Underworld;
                    tx = Utility.RandomMinMax( 50+swrapx, 1660-swrapx );
                    ty = Utility.RandomMinMax( 10+swrapy, 1600-swrapy );
                    tz = tl.GetAverageZ(tx, ty);
					if ( scape == "land" ){ failover = new Point3D(1433, 855, 0); } else { failover = new Point3D(547, 1441, -5); }
                }
                else if (land == Land.Serpent)
                {
					tl = Map.SerpentIsland;
                    tx = Utility.RandomMinMax( 0+swrapx, 1908-swrapx );
                    ty = Utility.RandomMinMax( 0+swrapy, 2047-swrapy );
                    tz = tl.GetAverageZ(tx, ty);
					if ( scape == "land" ){ failover = new Point3D(286, 1392, 2); } else { failover = new Point3D(1605, 536, -5); }
                }
                else if (land == Land.IslesDread)
                {
					tl = Map.IslesDread;
                    tx = Utility.RandomMinMax( 0+swrapx, 1446-swrapx );
                    ty = Utility.RandomMinMax( 0+swrapy, 1446-swrapy );
                    tz = tl.GetAverageZ(tx, ty);
					if ( scape == "land" ){ failover = new Point3D(1176, 816, 0); } else { failover = new Point3D(626, 643, -5); }
                }
                else if (land == Land.Savaged)
                {
					tl = Map.SavagedEmpire;
                    tx = Utility.RandomMinMax( 170+swrapx, 1200-swrapx );
                    ty = Utility.RandomMinMax( 10+swrapy, 1795-swrapy );
                    tz = tl.GetAverageZ(tx, ty);
					if ( scape == "land" ){ failover = new Point3D(653, 1269, -2); } else { failover = new Point3D(320, 638, -5); }
                }
                else if (land == Land.Sosaria)
                {
					tl = Map.Sosaria;
                    tx = Utility.RandomMinMax( 0+swrapx, 5158-swrapx );
                    ty = Utility.RandomMinMax( 0+swrapy, 3128-swrapy );
                    tz = tl.GetAverageZ(tx, ty);
					if ( scape == "land" ){ failover = new Point3D(2575, 1680, 20); } else { failover = new Point3D(112, 1816, -5); }
                }

                LandTile t = tl.Tiles.GetLandTile(tx, ty);

				if ( scape == "land" )
				{
					LandOk = Utility.PassableTile ( t.ID, "any" );

					Mobile mSp = new Rat();
					mSp.Name = "locator";
					mSp.MoveToWorld(new Point3D(tx, ty, tz), tl);
					Region RatReg = mSp.Region;
					mSp.Delete();
					testLocation = new Point3D(tx, ty, tz);

					if (LandOk && tl.CanSpawnMobile(tx, ty, tz) && ( Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( tl, testLocation ) ) || RatReg.IsPartOf(typeof(Regions.BardTownRegion)) ) )
					{
						loc = new Point3D(tx, ty, tz);
						tm = 1;
					}
				}
				else // GET WATER TILES
				{
					if ( Server.Misc.Worlds.IsWaterTile( t.ID, 0 ) && Server.Misc.Worlds.TestOcean ( tl, tx, ty, 2 ) ) { LandOk = true; }

					Point3D locale = new Point3D(tx, ty, tz);
					Region reg = Region.Find( locale, tl );

					if ( tz != -5 ){ LandOk = false; }

					if ( LandOk && Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( tl, locale ) ) )
					{
						loc = new Point3D(tx, ty, tz);
						tm = 1;
					}
				}

				r++; // SAFETY CATCH
				if ( r > 1000 && tm != 1)
                {
                    loc = failover;
					tm = 1;
                }
            }
            return loc;
        }

		public static bool InBuilding( Mobile m )
		{
			if ( !MySettings.S_NoMountBuilding && !(m is TownGuards) )
				return false;

			// A TWEAK FOR ONE GUARD WHO SOMETIMES MOUNTS BECAUSE THEY SPAWN AT Z OF 0
			if ( m is TownGuards && m.Map == Map.Sosaria & ((BaseCreature)m).Home.X == 2999 && ((BaseCreature)m).Home.Y == 1124 )
				return true;

            return Server.Terrains.InBuilding( m.Map, m.X, m.Y, m.Z );
        }

		public static bool IsMassSpawnZone( Map map, int x, int y )
		{
			if (
				( x >= 0 && y >= 0 && x <= 6 && y <= 6 && map == Map.Lodor ) || 
				( x >= 0 && y >= 0 && x <= 6 && y <= 6 && map == Map.Sosaria ) || 
				( x >= 0 && y >= 0 && x <= 6 && y <= 6 && map == Map.SerpentIsland ) || 
				( x >= 0 && y >= 0 && x <= 6 && y <= 6 && map == Map.IslesDread ) || 
				( x >= 1125 && y >= 298 && x <= 1131 && y <= 305 && map == Map.SavagedEmpire ) || 
				( x >= 5457 && y >= 3300 && x <= 5459 && y <= 3302 && map == Map.Sosaria ) || 
				( x >= 608 && y >= 4090 && x <= 704 && y <= 4096 && map == Map.Sosaria ) || 
				( x >= 6126 && y >= 827 && x <= 6132 && y <= 833 && map == Map.Sosaria ) || 
				( x >= 2 && y >= 2 && x <= 5 && y <= 5 && map == Map.Underworld )
				)
				return true;

			return false;
		}

		public static Point3D GetBoatWater( int x, int y, int z, Map map, int range )
		{
			Point3D loc = new Point3D(x, y, z);
			Map tm = map;
			int tx = Utility.RandomMinMax( x+range, x-range );
			int ty = Utility.RandomMinMax( y+range, y-range );
			int tz = -5;
            loc = new Point3D(tx, ty, tz);

            return loc;
        }

		public static bool IsWaterTile ( int id, int harvest )
		{
			if ( harvest == 0 && ( id==0x00A8 || id==0x00A9 || id==0x00AA || id==0x00AB || id==0x0136 || id==0x0137 || id==0x1559 || id==0x1796 || id==0x1797 || id==0x1798 || id==0x1799 || id==0x179A || id==0x179B || id==0x179C || id==0x179D || id==0x179E || id==0x179F || id==0x17A0 || id==0x17A1 || id==0x17A2 || id==0x17A3 || id==0x17A4 || id==0x17A5 || id==0x17A6 || id==0x17A7 || id==0x17A8 || id==0x17A9 || id==0x17AA || id==0x17AB || id==0x17AC || id==0x17AD || id==0x17AE || id==0x17AF || id==0x17B0 || id==0x17B1 || id==0x17B2 || id==0x17BB || id==0x17BC || id==0x346E || id==0x346F || id==0x3470 || id==0x3471 || id==0x3472 || id==0x3473 || id==0x3474 || id==0x3475 || id==0x3476 || id==0x3477 || id==0x3478 || id==0x3479 || id==0x347A || id==0x347B || id==0x347C || id==0x347D || id==0x347E || id==0x347F || id==0x3480 || id==0x3481 || id==0x3482 || id==0x3483 || id==0x3484 || id==0x3485 || id==0x3494 || id==0x3495 || id==0x3496 || id==0x3497 || id==0x3498 || id==0x349A || id==0x349B || id==0x349C || id==0x349D || id==0x349E || id==0x34A0 || id==0x34A1 || id==0x34A2 || id==0x34A3 || id==0x34A4 || id==0x34A6 || id==0x34A7 || id==0x34A8 || id==0x34A9 || id==0x34AA || id==0x34AB || id==0x34B8 || id==0x34B9 || id==0x34BA || id==0x34BB || id==0x34BD || id==0x34BE || id==0x34BF || id==0x34C0 || id==0x34C2 || id==0x34C3 || id==0x34C4 || id==0x34C5 || id==0x34C7 || id==0x34C8 || id==0x34C9 || id==0x34CA || id==0x34D2 || id==0x3529 || id==0x352A || id==0x352B || id==0x352C || id==0x3531 || id==0x3532 || id==0x3533 || id==0x3534 || id==0x3535 || id==0x3536 || id==0x3537 || id==0x3538 || id==0x353D || id==0x353E || id==0x353F || id==0x3540 || id==0x3541 || id==0x55F0 || id==0x55F1 || id==0x55F2 || id==0x55F3 || id==0x55F4 || id==0x55F5 || id==0x55F6 || id==0x55F7 || id==0x55F8 || id==0x55F9 || id==0x55FA || id==0x55FB || id==0x55FC || id==0x55FD || id==0x55FE || id==0x55FF || id==0x5600 || id==0x5601 || id==0x5602 || id==0x5603 || id==0x5604 || id==0x5605 || id==0x5606 || id==0x5607 || id==0x5608 || id==0x5609 || id==0x560A || id==0x560B || id==0x560C || id==0x560D || id==0x560E || id==0x560F || id==0x5610 || id==0x5611 || id==0x5612 || id==0x5613 || id==0x5614 || id==0x5615 || id==0x5616 || id==0x5617 || id==0x5618 || id==0x5619 || id==0x561A || id==0x561B || id==0x561C || id==0x561D || id==0x561E || id==0x561F || id==0x5620 || id==0x5621 || id==0x5622 || id==0x5623 || id==0x5624 || id==0x5633 || id==0x5634 || id==0x5635 || id==0x5636 || id==0x5637 || id==0x5638 || id==0x5639 || id==0x563A || id==0x563B || id==0x563C || id==0x563D || id==0x563F || id==0x5640 || id==0x5641 || id==0x5642 || id==0x5643 || id==0x5644 || id==0x5645 || id==0x5646 || id==0x5647 || id==0x5648 || id==0x5649 || id==0x564A || id==0x5657 || id==0x5658 || id==0x5659 || id==0x565A || id==0x565B || id==0x565C || id==0x565D || id==0x565E || id==0x565F || id==0x5660 || id==0x5661 || id==0x5662 || id==0x5663 || id==0x5664 || id==0x5665 || id==0x5666 || id==0x5667 || id==0x5668 || id==0x5669 || id==0x566A || id==0x566B || id==0x566C || id==0x566D || id==0x566E || id==0x566F ) )
				return true;

			else if ( id>=0x5536 && id<= 0x553F )
				return true;

			else if ( harvest == 1 && ( id==0x40A8 || id==0x40A9 || id==0x40AA || id==0x40AB || id==0x4136 || id==0x4137 || id==0x5559 || id==0x5796 || id==0x5797 || id==0x5798 || id==0x5799 || id==0x579A || id==0x579B || id==0x579C || id==0x579D || id==0x579E || id==0x579F || id==0x57A0 || id==0x57A1 || id==0x57A2 || id==0x57A3 || id==0x57A4 || id==0x57A5 || id==0x57A6 || id==0x57A7 || id==0x57A8 || id==0x57A9 || id==0x57AA || id==0x57AB || id==0x57AC || id==0x57AD || id==0x57AE || id==0x57AF || id==0x57B0 || id==0x57B1 || id==0x57B2 || id==0x57BB || id==0x57BC || id==0x746E || id==0x746F || id==0x7470 || id==0x7471 || id==0x7472 || id==0x7473 || id==0x7474 || id==0x7475 || id==0x7476 || id==0x7477 || id==0x7478 || id==0x7479 || id==0x747A || id==0x747B || id==0x747C || id==0x747D || id==0x747E || id==0x747F || id==0x7480 || id==0x7481 || id==0x7482 || id==0x7483 || id==0x7484 || id==0x7485 || id==0x7494 || id==0x7495 || id==0x7496 || id==0x7497 || id==0x7498 || id==0x749A || id==0x749B || id==0x749C || id==0x749D || id==0x749E || id==0x74A0 || id==0x74A1 || id==0x74A2 || id==0x74A3 || id==0x74A4 || id==0x74A6 || id==0x74A7 || id==0x74A8 || id==0x74A9 || id==0x74AA || id==0x74AB || id==0x74B8 || id==0x74B9 || id==0x74BA || id==0x74BB || id==0x74BD || id==0x74BE || id==0x74BF || id==0x74C0 || id==0x74C2 || id==0x74C3 || id==0x74C4 || id==0x74C5 || id==0x74C7 || id==0x74C8 || id==0x74C9 || id==0x74CA || id==0x74D2 || id==0x7529 || id==0x752A || id==0x752B || id==0x752C || id==0x7531 || id==0x7532 || id==0x7533 || id==0x7534 || id==0x7535 || id==0x7536 || id==0x7537 || id==0x7538 || id==0x753D || id==0x753E || id==0x753F || id==0x7540 || id==0x7541 || id==0x95F0 || id==0x95F1 || id==0x95F2 || id==0x95F3 || id==0x95F4 || id==0x95F5 || id==0x95F6 || id==0x95F7 || id==0x95F8 || id==0x95F9 || id==0x95FA || id==0x95FB || id==0x95FC || id==0x95FD || id==0x95FE || id==0x95FF || id==0x9600 || id==0x9601 || id==0x9602 || id==0x9603 || id==0x9604 || id==0x9605 || id==0x9606 || id==0x9607 || id==0x9608 || id==0x9609 || id==0x960A || id==0x960B || id==0x960C || id==0x960D || id==0x960E || id==0x960F || id==0x9610 || id==0x9611 || id==0x9612 || id==0x9613 || id==0x9614 || id==0x9615 || id==0x9616 || id==0x9617 || id==0x9618 || id==0x9619 || id==0x961A || id==0x961B || id==0x961C || id==0x961D || id==0x961E || id==0x961F || id==0x9620 || id==0x9621 || id==0x9622 || id==0x9623 || id==0x9624 || id==0x9633 || id==0x9634 || id==0x9635 || id==0x9636 || id==0x9637 || id==0x9638 || id==0x9639 || id==0x963A || id==0x963B || id==0x963C || id==0x963D || id==0x963F || id==0x9640 || id==0x9641 || id==0x9642 || id==0x9643 || id==0x9644 || id==0x9645 || id==0x9646 || id==0x9647 || id==0x9648 || id==0x9649 || id==0x964A || id==0x9657 || id==0x9658 || id==0x9659 || id==0x965A || id==0x965B || id==0x965C || id==0x965D || id==0x965E || id==0x965F || id==0x9660 || id==0x9661 || id==0x9662 || id==0x9663 || id==0x9664 || id==0x9665 || id==0x9666 || id==0x9667 || id==0x9668 || id==0x9669 || id==0x966A || id==0x966B || id==0x966C || id==0x966D || id==0x966E || id==0x966F ) )
				return true;

			return false;
		}

		public static bool TestTile ( Map map, int x, int y, string category )
		{
			Region reg = Region.Find( new Point3D( x, y, 0 ), map );
				if ( reg.IsPartOf( typeof( DungeonRegion ) ) ){ return false; }

			int results = 0;

			LandTile landTile1 = map.Tiles.GetLandTile( x-1, y-1 );
			LandTile landTile2 = map.Tiles.GetLandTile( x, y-1 );
			LandTile landTile3 = map.Tiles.GetLandTile( x+1, y-1 );
			LandTile landTile4 = map.Tiles.GetLandTile( x-1, y );
			LandTile landTile5 = map.Tiles.GetLandTile( x, y );
			LandTile landTile6 = map.Tiles.GetLandTile( x+1, y );
			LandTile landTile7 = map.Tiles.GetLandTile( x-1, y+1 );
			LandTile landTile8 = map.Tiles.GetLandTile( x, y+1 );
			LandTile landTile9 = map.Tiles.GetLandTile( x+1, y+1 );

			// YEW FOREST PATCH
			if ( map == Map.Sosaria && category == "forest" )
			{
				if (
					( x >= 2089 && y >= 841 && x <= 2207 && y <= 1001 ) || 
					( x >= 2162 && y >= 679 && x <= 2358 && y <= 1077 ) || 
					( x >= 2335 && y >= 660 && x <= 2621 && y <= 1117 ) || 
					( x >= 2610 && y >= 718 && x <= 2747 && y <= 1025 )
				)
					category = "jungle";
			}

			if ( Utility.PassableTile ( landTile1.ID, category ) ){ results ++; }
				if ( Utility.BlockedTile ( landTile1.ID, category ) ){ results ++; }
			if ( Utility.PassableTile ( landTile2.ID, category ) ){ results ++; }
				if ( Utility.BlockedTile ( landTile2.ID, category ) ){ results ++; }
			if ( Utility.PassableTile ( landTile3.ID, category ) ){ results ++; }
				if ( Utility.BlockedTile ( landTile3.ID, category ) ){ results ++; }
			if ( Utility.PassableTile ( landTile4.ID, category ) ){ results ++; }
				if ( Utility.BlockedTile ( landTile4.ID, category ) ){ results ++; }
			if ( Utility.PassableTile ( landTile5.ID, category ) ){ results ++; }
				if ( Utility.BlockedTile ( landTile5.ID, category ) ){ results ++; }
			if ( Utility.PassableTile ( landTile6.ID, category ) ){ results ++; }
				if ( Utility.BlockedTile ( landTile6.ID, category ) ){ results ++; }
			if ( Utility.PassableTile ( landTile7.ID, category ) ){ results ++; }
				if ( Utility.BlockedTile ( landTile7.ID, category ) ){ results ++; }
			if ( Utility.PassableTile ( landTile8.ID, category ) ){ results ++; }
				if ( Utility.BlockedTile ( landTile8.ID, category ) ){ results ++; }
			if ( Utility.PassableTile ( landTile9.ID, category ) ){ results ++; }
				if ( Utility.BlockedTile ( landTile9.ID, category ) ){ results ++; }

			if ( results > 4 )
				return true;

			return false;
		}

		public static bool TestMountain ( Map map, int x, int y, int distance )
		{
			Region reg = Region.Find( new Point3D( x, y, 0 ), map );
				if ( reg.IsPartOf( typeof( DungeonRegion ) ) ){ return false; }

			int results = 0;

			LandTile landRock1 = map.Tiles.GetLandTile( x-distance, y-distance );
			LandTile landRock2 = map.Tiles.GetLandTile( x, y-distance );
			LandTile landRock3 = map.Tiles.GetLandTile( x+distance, y-distance );
			LandTile landRock4 = map.Tiles.GetLandTile( x-distance, y );
			LandTile landRock5 = map.Tiles.GetLandTile( x+distance, y );
			LandTile landRock6 = map.Tiles.GetLandTile( x-distance, y+distance );
			LandTile landRock7 = map.Tiles.GetLandTile( x, y+distance );
			LandTile landRock8 = map.Tiles.GetLandTile( x+distance, y+distance );

			if ( Utility.BlockedTile ( landRock1.ID, "rock" ) ){ results ++; }
			if ( Utility.BlockedTile ( landRock2.ID, "rock" ) ){ results ++; }
			if ( Utility.BlockedTile ( landRock3.ID, "rock" ) ){ results ++; }
			if ( Utility.BlockedTile ( landRock4.ID, "rock" ) ){ results ++; }
			if ( Utility.BlockedTile ( landRock5.ID, "rock" ) ){ results ++; }
			if ( Utility.BlockedTile ( landRock6.ID, "rock" ) ){ results ++; }
			if ( Utility.BlockedTile ( landRock7.ID, "rock" ) ){ results ++; }
			if ( Utility.BlockedTile ( landRock8.ID, "rock" ) ){ results ++; }

			if ( results > 0 )
				return true;

			return false;
		}

		public static bool TestOcean( Map map, int x, int y, int distance )
		{
			Region reg = Region.Find( new Point3D( x, y, 0 ), map );
				if ( reg.IsPartOf( typeof( DungeonRegion ) ) ){ return false; }

			int results = 0;

			LandTile seaTile1 = map.Tiles.GetLandTile( x-distance, y-distance );
			LandTile seaTile2 = map.Tiles.GetLandTile( x, y-distance );
			LandTile seaTile3 = map.Tiles.GetLandTile( x+distance, y-distance );
			LandTile seaTile4 = map.Tiles.GetLandTile( x-distance, y );
			LandTile seaTile5 = map.Tiles.GetLandTile( x+distance, y );
			LandTile seaTile6 = map.Tiles.GetLandTile( x-distance, y+distance );
			LandTile seaTile7 = map.Tiles.GetLandTile( x, y+distance );
			LandTile seaTile8 = map.Tiles.GetLandTile( x+distance, y+distance );

			if ( Utility.BlockedTile ( seaTile1.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile2.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile3.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile4.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile5.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile6.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile7.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile8.ID, "water" ) ){ results ++; }

			if ( results > 0 )
				return true;

			return false;
		}

		public static bool TestShore( Map map, int x, int y, int distance )
		{
			int results = 0;

			LandTile seaTile1 = map.Tiles.GetLandTile( x-distance, y-distance );
			LandTile seaTile2 = map.Tiles.GetLandTile( x, y-distance );
			LandTile seaTile3 = map.Tiles.GetLandTile( x+distance, y-distance );
			LandTile seaTile4 = map.Tiles.GetLandTile( x-distance, y );
			LandTile seaTile5 = map.Tiles.GetLandTile( x+distance, y );
			LandTile seaTile6 = map.Tiles.GetLandTile( x-distance, y+distance );
			LandTile seaTile7 = map.Tiles.GetLandTile( x, y+distance );
			LandTile seaTile8 = map.Tiles.GetLandTile( x+distance, y+distance );

			if ( Utility.BlockedTile ( seaTile1.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile2.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile3.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile4.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile5.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile6.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile7.ID, "water" ) ){ results ++; }
			if ( Utility.BlockedTile ( seaTile8.ID, "water" ) ){ results ++; }

			if ( results > 7 )
				return false;

			return true;
		}
	}

    class WhereWorld
    {
		public static void Initialize()
		{
            CommandSystem.Register( "world", AccessLevel.Administrator, new CommandEventHandler( WhereWorld_OnCommand ) );
		}
		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "world" )]
		[Description( "Tells you what world you are in." )]
		public static void WhereWorld_OnCommand( CommandEventArgs e )
        {
			(e.Mobile).SendMessage( "You are currently in " + Server.Lands.LandName( (e.Mobile).Land ) + "." );
		}
	}
}