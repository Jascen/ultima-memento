/***************************************************************************
 *                                  Map.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id$
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Server.Items;
using Server.Network;
using Server.Targeting;
using System.Linq;

namespace Server
{
    public class Difficult
    {
		public static int GetDifficultyBounded( IEntity entity )
		{
			return Math.Max(0, Server.Difficult.GetDifficulty( entity.Location, entity.Map ));
		}

		public static int GetDifficulty( Point3D loc, Map map ) // THESE ARE DUNGEON DIFFICULTY LEVELS FROM 0 (NEWBIE) TO 1 (NORMAL) UP TO 5 (EPIC)
		{
			Land land = Lands.GetLand( map, loc, loc.X, loc.Y );

			int Heat = -5;

			Region reg = Region.Find( loc, map );

			if ( map == Map.Lodor )
			{
				if ( reg.IsPartOf( "the Lodoria Sewers" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Lizardman Cave" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Ratmen Cave" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Crypt" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "Dungeon Wrong" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Volcanic Cave" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "Terathan Keep" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "Dungeon Shame" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Ice Fiend Lair" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Frozen Hells" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "Dungeon Hythloth" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Mind Flayer City" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the City of Embers" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "Dungeon Destard" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "Dungeon Despise" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "Dungeon Deceit" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "Dungeon Covetous" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the Lodoria Catacombs" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Halls of Undermountain" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the Vault of the Black Knight" ) ){ Heat = 3; } // -- IN SERPENT ISLAND
				else if ( reg.IsPartOf( "the Crypts of Dracula" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the Castle of Dracula" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "Stonegate Castle" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Ancient Elven Mine" ) ){ Heat = 3; }

				else if ( reg.IsPartOf( "Morgaelin's Inferno" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Zealan Tombs" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Temple of Osirus" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "Argentrock Castle" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Daemon's Crag" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Hall of the Mountain King" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Depths of Carthax Lake" ) ){ Heat = 4; }

				else if ( reg.IsPartOf( "the Montor Sewers" ) ){ Heat = 0; }

				else if ( reg.IsPartOf( "Mangar's Tower" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "Mangar's Chamber" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "Kylearan's Tower" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "Harkyn's Castle" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Catacombs" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Lower Catacombs" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Sewers" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Cellar" ) ){ Heat = 0; }

				else if ( reg.IsPartOf( "the Sanctum of Saltmarsh" ) ){ Heat = 3; }
			}
			else if ( map == Map.Sosaria )
			{
				if ( reg.IsPartOf( "the Ancient Pyramid" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Mausoleum" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "Dungeon Clues" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "Dardin's Pit" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "Frostwall Caverns" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "Dungeon Doom" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "Dungeon Exodus" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Fires of Hell" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Frozen Dungeon" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Mines of Morinia" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Perinian Depths" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Ratmen Lair" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Dungeon of Time Awaits" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "Castle Exodus" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Cave of Banished Mages" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the City of the Dead" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Dragon's Maw" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Cave of the Zuluu" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Tower of Brass" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Caverns of Poseidon" ) ){ Heat = 1; }

				else if ( reg.IsPartOf( "the Accursed Maze" ) ){ Heat = -1; }
				else if ( reg.IsPartOf( "the Chamber of Bane" ) ){ Heat = -1; }
				else if ( reg.IsPartOf( "Coldhall Depths" ) ){ Heat = -1; }
				else if ( reg.IsPartOf( "the Dark Sanctum" ) ){ Heat = -1; }
				else if ( reg.IsPartOf( "the Forgotten Tombs" ) ){ Heat = -1; }
				else if ( reg.IsPartOf( "the Magma Vaults" ) ){ Heat = -1; }
				else if ( reg.IsPartOf( "the Dark Tombs" ) ){ Heat = -1; }
				else if ( reg.IsPartOf( "the Shrouded Grave" ) ){ Heat = -1; }

				else if ( reg.IsPartOf( "the Ruins of the Black Blade" ) ){ Heat = 2; }

				else if ( reg.IsPartOf( "Steamfire Cave" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Kuldara Sewers" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Crypts of Kuldar" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "Vordo's Castle" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "Vordo's Dungeon" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "Vordo's Castle Grounds" ) ){ Heat = 3; }
			}
			else if ( map == Map.SerpentIsland )
			{
				if ( reg.IsPartOf( "the Ancient Prison" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Cave of Fire" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Cave of Souls" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "Dungeon Ankh" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "Dungeon Bane" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "Dungeon Hate" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "Dungeon Scorn" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "Dungeon Torment" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "Dungeon Vile" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "Dungeon Wicked" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "Dungeon Wrath" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the Flooded Temple" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Gargoyle Crypts" ) ){ Heat = 0; }
				else if ( reg.IsPartOf( "the Serpent Sanctum" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Tomb of the Fallen Wizard" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the Castle of the Black Knight" ) ){ Heat = 3; }
			}
			else if ( map == Map.IslesDread )
			{
				if ( reg.IsPartOf( "the Altar of the Blood God" ) ){ Heat = 2; }
			}
			else if ( map == Map.Underworld )
			{
				if ( loc.X > 1655 && loc.Y < 1065 )
				{
					// THIS IS THE DUNGEON HOME REGION
				}
				else
				{
					if ( reg.IsPartOf( "the Glacial Scar" ) ){ Heat = 2; }
					else if ( reg.IsPartOf( "the Stygian Abyss" ) ){ Heat = 4; }
					else if ( land == Land.Underworld ){ Heat = 3; }
				}
			}
			else if ( map == Map.SavagedEmpire )
			{
				if ( reg.IsPartOf( "the Blood Temple" ) ){ Heat = 2; } // -- IN ISLES OF DREAD
				else if ( reg.IsPartOf( "the Tombs" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Corrupt Pass" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Crypt" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the Great Pyramid" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Altar of the Dragon King" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Ice Queen Fortress" ) ){ Heat = 2; } // -- IN ISLES OF DREAD
				else if ( reg.IsPartOf( "the Dungeon of the Lich King" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Dungeon of the Mad Archmage" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Halls of Ogrimar" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Ratmen Mines" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "Dungeon Rock" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Sakkhra Tunnel" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the Spider Cave" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the Storm Giant Lair" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Cave of the Ancient Wyrm" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the Isle of the Lich" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Castle of the Mad Archmage" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Mage Mansion" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Island of the Storm Giant" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Orc Fort" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the Hedge Maze" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the Pixie Cave" ) ){ Heat = 1; }
				else if ( reg.IsPartOf( "the Forgotten Halls" ) ){ Heat = 2; }
				else if ( reg.IsPartOf( "the Undersea Castle" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Tomb of Kazibal" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Catacombs of Azerok" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Azure Castle" ) ){ Heat = 3; }
				else if ( reg.IsPartOf( "the Scurvy Reef" ) ){ Heat = 2; }

				else if ( reg.IsPartOf( "the Ancient Crash Site" ) ){ Heat = 4; }
				else if ( reg.IsPartOf( "the Ancient Sky Ship" ) ){ Heat = 4; }
			}
			else if ( map == Map.Atlantis )
			{
				if ( reg.IsPartOf( "the Erase" ) ){ Heat = 0; }
				else { Heat = 4; }
			}

			return Heat;
		}
	}

	public enum Land
	{
		None = 0,
		Ambrosia,
		Atlantis,
		IslesDread,
		Kuldar,
		Lodoria,
		Luna,
		Savaged,
		Serpent,
		SkaraBrae,
		Sosaria,
		UmberVeil,
		Underworld
	}

    public class Lands
    {
        public static string LandNameShort(Land land)
        {
            switch (land)
            {
                case Land.Ambrosia: return "Ambrosia";
                case Land.Atlantis: return "Atlantis";
                case Land.IslesDread: return "Isles of Dread";
                case Land.Kuldar: return "Kuldar";
                case Land.Lodoria: return "Lodoria";
                case Land.Luna: return "Luna";
                case Land.Savaged: return "Savaged Empire";
                case Land.Serpent: return "Serpent Island";
                case Land.SkaraBrae: return "Skara Brae";
                case Land.Sosaria: return "Sosaria";
                case Land.UmberVeil: return "Umber Veil";
                case Land.Underworld: return "Underworld";
                default: throw new Exception("Unsupported land type: " + land.ToString());
            }
        }

		public static string LandName( Land land )
		{
			if ( land == Land.Ambrosia )
				return "the Land of Ambrosia";
			else if ( land == Land.Atlantis )
				return "the Land of Atlantis";
			else if ( land == Land.IslesDread )
				return "the Isles of Dread";
			else if ( land == Land.Kuldar )
				return "the Bottle World of Kuldar";
			else if ( land == Land.Lodoria )
				return "the Land of Lodoria";
			else if ( land == Land.Luna )
				return "the Moon of Luna";
			else if ( land == Land.Savaged )
				return "the Savaged Empire";
			else if ( land == Land.Serpent )
				return "the Serpent Island";
			else if ( land == Land.SkaraBrae )
				return "the Town of Skara Brae";
			else if ( land == Land.Sosaria )
				return "the Land of Sosaria";
			else if ( land == Land.UmberVeil )
				return "the Island of Umber Veil";
			else if ( land == Land.Underworld )
				return "the Underworld";

			return "the Land of Sosaria";
		}

		public static Land LandRef( string land )
		{
			if ( land == "the Land of Ambrosia" )
				return Land.Ambrosia;
			else if ( land == "the Land of Atlantis" )
				return Land.Atlantis;
			else if ( land == "the Isles of Dread" )
				return Land.IslesDread;
			else if ( land == "the Bottle World of Kuldar" )
				return Land.Kuldar;
			else if ( land == "the Land of Lodoria" )
				return Land.Lodoria;
			else if ( land == "the Moon of Luna" )
				return Land.Luna;
			else if ( land == "the Savaged Empire" )
				return Land.Savaged;
			else if ( land == "the Serpent Island" )
				return Land.Serpent;
			else if ( land == "the Town of Skara Brae" )
				return Land.SkaraBrae;
			else if ( land == "the Land of Sosaria" )
				return Land.Sosaria;
			else if ( land == "the Island of Umber Veil" )
				return Land.UmberVeil;
			else if ( land == "the Underworld" )
				return Land.Underworld;

			return Land.None;
		}

		public static Map GetMap( Land land )
		{
			switch (land)
			{
				case Land.Kuldar:
				case Land.Ambrosia:
				case Land.Luna:
				case Land.Sosaria:
				case Land.UmberVeil:
					return Map.Sosaria;

				case Land.Atlantis: return Map.Atlantis;
				case Land.IslesDread: return Map.IslesDread;
				case Land.Lodoria: return Map.Lodor;
				case Land.Savaged: return Map.SavagedEmpire;
				case Land.Serpent: return Map.SerpentIsland;
				case Land.SkaraBrae: return Map.Lodor;
				case Land.Underworld: return Map.Underworld;
			}

			return Map.Sosaria;
		}

		public static Land GetLand( IEntity entity )
		{
			return GetLand(entity.Map, entity.Location, entity.Location.X, entity.Location.Y);
		}

		public static Land GetLand( Map map, Point3D location, int x, int y )
		{
			Region reg = Region.Find( location, map );
			Land land = Land.Sosaria;

			if ( map == Map.Sosaria && x > 5774 && y > 2694 && x < 6123 && y < 3074 ){ land = Land.Luna; }
			else if ( map == Map.Sosaria && ( reg.IsPartOf( "Moonlight Cavern" ) || 
												reg.IsPartOf( "the Core of the Moon" ) || 
												reg.IsPartOf( "the Moon's Core" ) ) ){ land = Land.Luna; }

			else if ( map == Map.Sosaria && x > 5125 && y > 3038 && x < 6124 && y < 4093 ){ land = Land.Ambrosia; }
			else if ( map == Map.Sosaria && x > 3229 && y > 3870 && x < 3344 && y < 3946 ){ land = Land.Ambrosia; }
			else if ( map == Map.Sosaria && ( reg.IsPartOf( "the Dragon's Maw" ) || 
												reg.IsPartOf( "the Cave of the Zuluu" ) || 
												reg.IsPartOf( "the Arena of The Zuluu" ) ) ){ land = Land.Ambrosia; }

			else if ( map == Map.Sosaria && x > 2931 && y > 3675 && x < 2999 && y < 3722 ){ land = Land.UmberVeil; }
			else if ( map == Map.Sosaria && x > 699 && y > 3129 && x < 2272 && y < 4095 ){ land = Land.UmberVeil; }
			else if ( map == Map.Sosaria && reg.IsPartOf( "the Mausoleum" ) ){ land = Land.UmberVeil; }
			else if ( map == Map.Sosaria && reg.IsPartOf( "the Tower of Brass" ) ){ land = Land.UmberVeil; }

			else if ( map == Map.Sosaria && x > 6127 && y > 828 && x < 7168 && y < 2736 ){ land = Land.Kuldar; }
			else if ( map == Map.Sosaria && ( reg.IsPartOf( "Highrock Mine" ) || 
												reg.IsPartOf( "Waterfall Cavern" ) || 
												reg.IsPartOf( "the Crumbling Cave" ) || 
												reg.IsPartOf( "Steamfire Cave" ) || 
												reg.IsPartOf( "the Valley of Dark Druids" ) || 
												reg.IsPartOf( "Vordo's Castle Grounds" ) || 
												reg.IsPartOf( "the Kuldara Sewers" ) || 
												reg.IsPartOf( "the Crypts of Kuldar" ) || 
												reg.IsPartOf( "Vordo's Castle" ) || 
												reg.IsPartOf( "Vordo's Dungeon" ) ) ){ land = Land.Kuldar; }

			else if ( map == Map.Lodor && ( reg.IsPartOf( "Morgaelin's Inferno" ) || 
												reg.IsPartOf( "the Zealan Tombs" ) || 
												reg.IsPartOf( "Argentrock Castle" ) || 
												reg.IsPartOf( "the Daemon's Crag" ) || 
												reg.IsPartOf( "the Hall of the Mountain King" ) || 
												reg.IsPartOf( "the Depths of Carthax Lake" ) ) ){ land = Land.Underworld; }

			else if ( map == Map.Sosaria && reg.IsPartOf( "the Chamber of Corruption" ) ){ land = Land.Underworld; }

			else if ( map == Map.SavagedEmpire && ( reg.IsPartOf( "the Ancient Crash Site" ) || 
												reg.IsPartOf( "the Obsidian Fortress" ) || 
												reg.IsPartOf( "the Ancient Sky Ship" ) ) ){ land = Land.Underworld; }

			else if ( map == Map.Underworld && ( reg.IsPartOf( "the Glacial Scar" ) ) ){ land = Land.IslesDread; }

			else if ( map == Map.Lodor && ( reg.IsPartOf( "the Temple of Osirus" ) || reg.IsPartOf( "the Sanctum of Saltmarsh" ) ) ){ land = Land.IslesDread; }

			else if ( map == Map.Lodor && ( reg.IsPartOf( "the Town of Skara Brae" ) || 
											reg.IsPartOf( "Mangar's Tower" ) || 
											reg.IsPartOf( "Mangar's Chamber" ) || 
											reg.IsPartOf( "Kylearan's Tower" ) || 
											reg.IsPartOf( "Harkyn's Castle" ) || 
											reg.IsPartOf( "the Catacombs" ) || 
											reg.IsPartOf( "the Lower Catacombs" ) || 
											reg.IsPartOf( "the Sewers" ) || 
											reg.IsPartOf( "the Mines" ) || 
											reg.IsPartOf( "the Cellar" ) ) ){ land = Land.SkaraBrae; }

			else if ( map == Map.Lodor && reg.IsPartOf( "the Montor Sewers" ) ){ land = Land.Sosaria; }
			else if ( map == Map.Lodor && !reg.IsPartOf( "the Vault of the Black Knight" ) ){ land = Land.Lodoria; }

			else if ( map == Map.SerpentIsland || reg.IsPartOf( "the Vault of the Black Knight" ) ){ land = Land.Serpent; }

			else if (
						map == Map.SavagedEmpire && 
						( reg.IsPartOf( "the Cimmeran Mines" ) || 
						reg.IsPartOf( "the Ice Queen Fortress" ) || 
						reg.IsPartOf( "the Scurvy Reef" ) || 
						reg.IsPartOf( "the Blood Temple" ) ) ){ land = Land.IslesDread; }

			else if ( map == Map.SavagedEmpire && reg.IsPartOf( "the Forgotten Halls" ) ){ land = Land.Sosaria; }

			// SKY CASTLES
			else if ( map == Map.SerpentIsland && ( x > 1949 ) && ( y > 1393 ) && ( x < 2061 ) && ( y < 1486 ) ){ land = Land.Sosaria; }
			else if ( map == Map.SerpentIsland && ( x > 2150 ) && ( y > 1401 ) && ( x < 2270 ) && ( y < 1513 ) ){ land = Land.Lodoria; }
			else if ( map == Map.SerpentIsland && ( x > 2375 ) && ( y > 1398 ) && ( x < 2442 ) && ( y < 1467 ) ){ land = Land.Lodoria; }
			else if ( map == Map.SerpentIsland && ( x > 2401 ) && ( y > 1635 ) && ( x < 2468 ) && ( y < 1703 ) ){ land = Land.Serpent; }
			else if ( map == Map.SerpentIsland && ( x > 2408 ) && ( y > 1896 ) && ( x < 2517 ) && ( y < 2005 ) ){ land = Land.Savaged; }
			else if ( map == Map.SerpentIsland && ( x > 2181 ) && ( y > 1889 ) && ( x < 2275 ) && ( y < 2003 ) ){ land = Land.IslesDread; }
			else if ( map == Map.SerpentIsland && ( x > 1930 ) && ( y > 1890 ) && ( x < 2022 ) && ( y < 1997 ) ){ land = Land.Sosaria; }

			else if ( map == Map.SerpentIsland ){ land = Land.Serpent; }

			// DUNGEON HOMES
			else if ( map == Map.Underworld && ( x > 1644 ) && ( y > 35 ) && ( x < 1818 ) && ( y < 163 ) ){ land = Land.Lodoria; }
			else if ( map == Map.Underworld && ( x > 1864 ) && ( y > 32 ) && ( x < 2041 ) && ( y < 162 ) ){ land = Land.Savaged; }
			else if ( map == Map.Underworld && ( x > 2098 ) && ( y > 27 ) && ( x < 2272 ) && ( y < 156 ) ){ land = Land.Savaged; }
			else if ( map == Map.Underworld && ( x > 1647 ) && ( y > 184 ) && ( x < 1810 ) && ( y < 305 ) ){ land = Land.Serpent; }
			else if ( map == Map.Underworld && ( x > 1877 ) && ( y > 187 ) && ( x < 2033 ) && ( y < 302 ) ){ land = Land.UmberVeil; }
			else if ( map == Map.Underworld && ( x > 2108 ) && ( y > 190 ) && ( x < 2269 ) && ( y < 305 ) ){ land = Land.Serpent; }
			else if ( map == Map.Underworld && ( x > 1656 ) && ( y > 335 ) && ( x < 1807 ) && ( y < 443 ) ){ land = Land.Sosaria; }
			else if ( map == Map.Underworld && ( x > 1880 ) && ( y > 338 ) && ( x < 2031 ) && ( y < 445 ) ){ land = Land.Lodoria; }
			else if ( map == Map.Underworld && ( x > 2111 ) && ( y > 335 ) && ( x < 2266 ) && ( y < 446 ) ){ land = Land.IslesDread; }
			else if ( map == Map.Underworld && ( x > 1657 ) && ( y > 496 ) && ( x < 1807 ) && ( y < 606 ) ){ land = Land.Sosaria; }
			else if ( map == Map.Underworld && ( x > 1879 ) && ( y > 498 ) && ( x < 2031 ) && ( y < 605 ) ){ land = Land.Savaged; }
			else if ( map == Map.Underworld && ( x > 2115 ) && ( y > 499 ) && ( x < 2263 ) && ( y < 605 ) ){ land = Land.Lodoria; }
			else if ( map == Map.Underworld && ( x > 1657 ) && ( y > 641 ) && ( x < 1808 ) && ( y < 748 ) ){ land = Land.Lodoria; }
			else if ( map == Map.Underworld && ( x > 1883 ) && ( y > 640 ) && ( x < 2033 ) && ( y < 745 ) ){ land = Land.Savaged; }
			else if ( map == Map.Underworld && ( x > 2113 ) && ( y > 641 ) && ( x < 2266 ) && ( y < 747 ) ){ land = Land.IslesDread; }
			else if ( map == Map.Underworld && ( x > 1657 ) && ( y > 795 ) && ( x < 1811 ) && ( y < 898 ) ){ land = Land.Serpent; }
			else if ( map == Map.Underworld && ( x > 1883 ) && ( y > 794 ) && ( x < 2034 ) && ( y < 902 ) ){ land = Land.Lodoria; }
			else if ( map == Map.Underworld && ( x > 2112 ) && ( y > 794 ) && ( x < 2267 ) && ( y < 898 ) ){ land = Land.IslesDread; }
			else if ( map == Map.Underworld && ( x > 1659 ) && ( y > 953 ) && ( x < 1809 ) && ( y < 1059 ) ){ land = Land.Ambrosia; }
			else if ( map == Map.Underworld && ( x > 1881 ) && ( y > 954 ) && ( x < 2034 ) && ( y < 1059 ) ){ land = Land.Savaged; }
			else if ( map == Map.Underworld && ( x > 2113 ) && ( y > 952 ) && ( x < 2268 ) && ( y < 1056 ) ){ land = Land.Savaged; }

			else if ( map == Map.Lodor ){ land = Land.Lodoria; }
			else if ( map == Map.Sosaria ){ land = Land.Sosaria; }
			else if ( map == Map.Underworld ){ land = Land.Underworld; }
			else if ( map == Map.SerpentIsland ){ land = Land.Serpent; }
			else if ( map == Map.IslesDread ){ land = Land.IslesDread; }
			else if ( map == Map.SavagedEmpire ){ land = Land.Savaged; }
			else if ( map == Map.Atlantis ){ land = Land.Atlantis; }

			if ( map == Map.SerpentIsland && reg.IsPartOf( "Sosaria Prison" ) ){ land = Land.Sosaria; }
			else if ( map == Map.SerpentIsland && reg.IsPartOf( "Lodoria Prison" ) ){ land = Land.Lodoria; }
			else if ( map == Map.SerpentIsland && reg.IsPartOf( "Renika Prison" ) ){ land = Land.UmberVeil; }
			else if ( map == Map.SerpentIsland && reg.IsPartOf( "Kuldara Prison" ) ){ land = Land.Kuldar; }
			else if ( map == Map.SerpentIsland && reg.IsPartOf( "Ork Prison" ) ){ land = Land.Savaged; }
			else if ( map == Map.SerpentIsland && reg.IsPartOf( "Furnace Prison" ) ){ land = Land.Serpent; }
			else if ( map == Map.SerpentIsland && reg.IsPartOf( "Cimmeran Prison" ) ){ land = Land.IslesDread; }

			return land;
		}
	}

	public enum Terrain
	{
		None = 0,
		Building = 1,
		Town,
		Water,
		Cave,
		Dirt,
		Forest,
		Grass,
		Jungle,
		Rock,
		Sand,
		Swamp,
		Snow
	}

    public class Terrains
    {
		public static Terrain GetTerrain( Map map, Point3D location, int x, int y )
		{
			if ( map == Map.Internal || x < 0 || y < 0 )
				return Terrain.None;

			LandTile LTile = map.Tiles.GetLandTile( x, y );

			int[] WaterTerrain = { 0x00A8, 0x00A9, 0x00AA, 0x00AB, 0x0136, 0x0138 };
			int[] CaveTerrain = { 0x024A, 0x024B, 0x024C, 0x024D, 0x024E, 0x024F, 0x0250, 0x0251, 0x0252, 0x0253, 0x0254, 0x0255, 0x0256, 0x0257, 0x0258, 0x0259, 0x025A, 0x025B, 0x025C, 0x025D, 0x025E, 0x025F, 0x0260, 0x0261, 0x0262, 0x0263, 0x0264, 0x0265, 0x0266, 0x0267, 0x0268, 0x0269, 0x026A, 0x026B, 0x026C, 0x026D, 0x02BC, 0x02BD, 0x02BE, 0x02BF, 0x02C0, 0x02C1, 0x02C2, 0x02C3, 0x02C4, 0x02C5, 0x02C6, 0x02C7, 0x02C8, 0x02C9, 0x02CA, 0x02CB, 0x0245, 0x0246, 0x0247, 0x0248, 0x0249, 0x063B, 0x063C, 0x063D, 0x063E };
			int[] DirtTerrain = { 0x008D, 0x008E, 0x008F, 0x0090, 0x0091, 0x0092, 0x0093, 0x0094, 0x0095, 0x0096, 0x0097, 0x0098, 0x0099, 0x009A, 0x009B, 0x009C, 0x009D, 0x009E, 0x009F, 0x00A0, 0x00A1, 0x00A2, 0x00A3, 0x00A4, 0x00A5, 0x00A6, 0x00A7, 0x00DC, 0x00DD, 0x00DE, 0x00DF, 0x00E0, 0x00E1, 0x00E2, 0x00E3, 0x02D0, 0x02D1, 0x02D2, 0x02D3, 0x02D4, 0x02D5, 0x02D6, 0x02D7, 0x02E5, 0x02E6, 0x02E7, 0x02E8, 0x02E9, 0x02EA, 0x02EB, 0x02EC, 0x02ED, 0x02EE, 0x02EF, 0x02F0, 0x02F1, 0x02F2, 0x02F3, 0x02F4, 0x02F5, 0x02F6, 0x02F7, 0x02F8, 0x02F9, 0x02FA, 0x02FB, 0x02FC, 0x02FD, 0x02FE, 0x02FF, 0x0303, 0x0304, 0x0305, 0x0306, 0x0307, 0x0308, 0x0309, 0x030A, 0x030B, 0x030C, 0x030D, 0x030E, 0x030F, 0x0310, 0x0311, 0x0312, 0x0313, 0x0314, 0x0315, 0x0316, 0x0317, 0x0318, 0x0319, 0x031A, 0x031B, 0x031C, 0x031D, 0x031E, 0x031F, 0x06F4, 0x0777, 0x0778, 0x0779, 0x077A, 0x077B, 0x077C, 0x077D, 0x077E, 0x077F, 0x0780, 0x0781, 0x0782, 0x0783, 0x0784, 0x0785, 0x0786, 0x0787, 0x0788, 0x0789, 0x078A, 0x078B, 0x078C, 0x078D, 0x078E, 0x078F, 0x0790, 0x0791, 0x0071, 0x0072, 0x0073, 0x0074, 0x0075, 0x0076, 0x0077, 0x0078, 0x0079, 0x007A, 0x007B, 0x007C, 0x0082, 0x0083, 0x0085, 0x0086, 0x0087, 0x0088, 0x0089, 0x008A, 0x008B, 0x008C, 0x00E8, 0x00E9, 0x00EA, 0x00EB, 0x0141, 0x0142, 0x0143, 0x0144, 0x014C, 0x014D, 0x014E, 0x014F, 0x0169, 0x016A, 0x016B, 0x016C, 0x016D, 0x016E, 0x016F, 0x0170, 0x0171, 0x0172, 0x0173, 0x0174, 0x01DC, 0x01DD, 0x01DE, 0x01DF, 0x01E0, 0x01E1, 0x01E2, 0x01E3, 0x01E4, 0x01E5, 0x01E6, 0x01E7, 0x01EC, 0x01ED, 0x01EE, 0x01EF, 0x0272, 0x0273, 0x0274, 0x0275, 0x027E, 0x027F, 0x0280, 0x0281, 0x032C, 0x032D, 0x032E, 0x032F, 0x033D, 0x033E, 0x033F, 0x0340, 0x0345, 0x0346, 0x0347, 0x0348, 0x0349, 0x034A, 0x034B, 0x034C, 0x0355, 0x0356, 0x0357, 0x0358, 0x0367, 0x0368, 0x0369, 0x036A, 0x036B, 0x036C, 0x036D, 0x036E, 0x0377, 0x0378, 0x0379, 0x037A, 0x038D, 0x038E, 0x038F, 0x0390, 0x0395, 0x0396, 0x0397, 0x0398, 0x0399, 0x039A, 0x039B, 0x039C, 0x03A5, 0x03A6, 0x03A7, 0x03A8, 0x03F6, 0x03F7, 0x03F9, 0x03FA, 0x03FB, 0x03FC, 0x03FD, 0x03FE, 0x03FF, 0x0400, 0x0401, 0x0402, 0x0403, 0x0404, 0x0405, 0x0547, 0x0548, 0x0549, 0x054A, 0x054B, 0x054C, 0x054D, 0x054E, 0x0553, 0x0554, 0x0555, 0x0556, 0x0597, 0x0598, 0x0599, 0x059A, 0x059B, 0x059C, 0x059D, 0x059E, 0x0623, 0x0624, 0x0625, 0x0626, 0x0627, 0x0628, 0x0629, 0x062A, 0x062B, 0x062C, 0x062D, 0x062E, 0x062F, 0x0630, 0x0631, 0x0632, 0x0633, 0x0634, 0x0635, 0x0636, 0x0637, 0x0638, 0x0639, 0x063A, 0x06F3, 0x06F5, 0x06F6, 0x06F7, 0x06F8, 0x06F9, 0x06FA };
			int[] ForestTerrain = { 0x00ED, 0x00EE, 0x00EF, 0x3AF0, 0x3AF1, 0x3AF2, 0x3AF3, 0x3AF4, 0x3AF5, 0x3AF6, 0x3AF7, 0x3AF8, 0x00C4, 0x00C5, 0x00C6, 0x00C7, 0x00C8, 0x00C9, 0x00CA, 0x00CB, 0x00CC, 0x00CD, 0x00CE, 0x00CF, 0x00D0, 0x00D1, 0x00D2, 0x00D3, 0x00D4, 0x00D5, 0x00D6, 0x00D7, 0x00F0, 0x00F1, 0x00F2, 0x00F3, 0x00F8, 0x00F9, 0x00FA, 0x00FB, 0x015D, 0x015E, 0x015F, 0x0160, 0x0161, 0x0162, 0x0163, 0x0164, 0x0165, 0x0166, 0x0167, 0x0168, 0x0324, 0x0325, 0x0326, 0x0327, 0x0328, 0x0329, 0x032A, 0x032B, 0x054F, 0x0550, 0x0551, 0x0552, 0x05F1, 0x05F2, 0x05F3, 0x05F4, 0x05F9, 0x05FA, 0x05FB, 0x05FC, 0x05FD, 0x05FE, 0x05FF, 0x0600, 0x0601, 0x0602, 0x0603, 0x0604, 0x0611, 0x0612, 0x0613, 0x0614, 0x0653, 0x0654, 0x0655, 0x0656, 0x065B, 0x065C, 0x065D, 0x065E, 0x065F, 0x0660, 0x0661, 0x0662, 0x066B, 0x066C, 0x066D, 0x066E, 0x06AF, 0x06B0, 0x06B1, 0x06B2, 0x06B3, 0x06B4, 0x06BB, 0x06BC, 0x06BD, 0x06BE, 0x0709, 0x070A, 0x070B, 0x070C, 0x0715, 0x0716, 0x0717, 0x0718, 0x0719, 0x071A, 0x071B, 0x071C };
			int[] GrassTerrain = { 0x0231, 0x0232, 0x0233, 0x0234, 0x0239, 0x023A, 0x023B, 0x023C, 0x023D, 0x023E, 0x023F, 0x0240, 0x0241, 0x06D2, 0x06D3, 0x06D4, 0x06D5, 0x06D6, 0x06D7, 0x06D8, 0x06D9, 0x0003, 0x0004, 0x0005, 0x0006, 0x003B, 0x003C, 0x003D, 0x003E, 0x007D, 0x007E, 0x007F, 0x00C0, 0x00C1, 0x00C2, 0x00C3, 0x00D8, 0x00D9, 0x00DA, 0x00DB, 0x01A4, 0x01A5, 0x01A6, 0x01A7, 0x0242, 0x0243, 0x036F, 0x0370, 0x0371, 0x0372, 0x0373, 0x0374, 0x0375, 0x0376, 0x037B, 0x037C, 0x037D, 0x037E, 0x03BF, 0x03C0, 0x03C1, 0x03C2, 0x03C3, 0x03C4, 0x03C5, 0x03C6, 0x03CB, 0x03CC, 0x03CD, 0x03CE, 0x0579, 0x057A, 0x057B, 0x057C, 0x057D, 0x057E, 0x057F, 0x0580, 0x058B, 0x058C, 0x05D7, 0x05D8, 0x05D9, 0x05DA, 0x05DB, 0x05DC, 0x05DD, 0x05DE, 0x05E3, 0x05E4, 0x05E5, 0x05E6, 0x067D, 0x067E, 0x067F, 0x0680, 0x0681, 0x0682, 0x0683, 0x0684, 0x0689, 0x068A, 0x068B, 0x068C, 0x0695, 0x0696, 0x0697, 0x0698, 0x0699, 0x069A, 0x069B, 0x069C, 0x06A1, 0x06A2, 0x06A3, 0x06A4, 0x06B5, 0x06B6, 0x06B7, 0x06B8, 0x06B9, 0x06BA, 0x06BF, 0x06C0, 0x06C1, 0x06C2, 0x06DE, 0x06DF, 0x06E0, 0x06E1 };
			int[] JungleTerrain = { 0x00EC, 0x00FC, 0x00FD, 0x00FE, 0x00FF, 0x072A, 0x00AC, 0x00AD, 0x00AE, 0x00AF, 0x00B0, 0x00B3, 0x00B6, 0x00B9, 0x00BC, 0x00BD, 0x00BE, 0x00BF, 0x0100, 0x0101, 0x0102, 0x0103, 0x0108, 0x0109, 0x010A, 0x010B, 0x01F0, 0x01F1, 0x01F2, 0x01F3, 0x026E, 0x026F, 0x0270, 0x0271, 0x0276, 0x0277, 0x0278, 0x0279, 0x027A, 0x027B, 0x027C, 0x027D, 0x0286, 0x0287, 0x0288, 0x0289, 0x0292, 0x0293, 0x0294, 0x0295, 0x0581, 0x0582, 0x0583, 0x0584, 0x0585, 0x0586, 0x0587, 0x0588, 0x0589, 0x058A, 0x058D, 0x058E, 0x058F, 0x0590, 0x059F, 0x05A0, 0x05A1, 0x05A2, 0x05A3, 0x05A4, 0x05A5, 0x05A6, 0x05B3, 0x05B4, 0x05B5, 0x05B6, 0x05B7, 0x05B8, 0x05B9, 0x05BA, 0x05F5, 0x05F6, 0x05F7, 0x05F8, 0x0605, 0x0606, 0x0607, 0x0608, 0x0609, 0x060A, 0x060B, 0x060C, 0x060D, 0x060E, 0x060F, 0x0610, 0x0615, 0x0616, 0x0617, 0x0618, 0x0727, 0x0728, 0x0729, 0x0733, 0x0734, 0x0735, 0x0736, 0x0737, 0x0738, 0x0739, 0x073A };
			int[] RockTerrain = { 0x00E4, 0x00E5, 0x00E6, 0x00E7, 0x00F4, 0x00F5, 0x00F6, 0x00F7, 0x0104, 0x0105, 0x0106, 0x0107, 0x0110, 0x0111, 0x0112, 0x0113, 0x0122, 0x0123, 0x0124, 0x0125, 0x01D3, 0x01D4, 0x01D5, 0x01D6, 0x01D7, 0x01D8, 0x01D9, 0x01DA, 0x021F, 0x0220, 0x0221, 0x0222, 0x0223, 0x0224, 0x0225, 0x0226, 0x0227, 0x0228, 0x0229, 0x022A, 0x022B, 0x022C, 0x022D, 0x022E, 0x022F, 0x0230, 0x0235, 0x0236, 0x0237, 0x0238, 0x06CD, 0x06CE, 0x06CF, 0x06D0, 0x06D1, 0x06DA, 0x06DB, 0x06DC, 0x06DD, 0x06EB, 0x06EC, 0x06ED, 0x06EE, 0x06EF, 0x06F0, 0x06F1, 0x06F2, 0x06FB, 0x06FC, 0x06FD, 0x06FE, 0x070E, 0x070F, 0x0710, 0x0711, 0x0712, 0x0713, 0x0714, 0x071D, 0x071E, 0x071F, 0x0720, 0x072B, 0x072C, 0x072D, 0x072E, 0x072F, 0x0730, 0x0731, 0x0732, 0x073B, 0x073C, 0x073D, 0x073E, 0x0749, 0x074A, 0x074B, 0x074C, 0x074D, 0x074E, 0x074F, 0x0750, 0x0759, 0x075A, 0x075B, 0x075C, 0x09EC, 0x09ED, 0x09EE, 0x09EF, 0x09F0, 0x09F1, 0x09F2, 0x09F3, 0x09F4, 0x09F5, 0x09F6, 0x09F7, 0x09F8, 0x09F9, 0x09FA, 0x09FB, 0x09FC, 0x09FD, 0x09FE, 0x09FF, 0x0A00, 0x0A01, 0x0A02, 0x0A03, 0x3F39, 0x3F3A, 0x3F3B, 0x3F3C, 0x3F3D, 0x3F3E, 0x3F3F, 0x3F40, 0x3F41, 0x3F42, 0x3F43, 0x3F44, 0x3F45, 0x3F46, 0x3F47, 0x3F48, 0x3F49, 0x3F4A, 0x3F4B, 0x3F4C, 0x3F4D, 0x3F4E, 0x3F4F, 0x3F50, 0x3F51, 0x3F52, 0x3F53, 0x3F54, 0x3F55, 0x3F56, 0x3F57, 0x3F58, 0x3F59, 0x3F5A, 0x3F5B, 0x3F5C, 0x3F5D, 0x3F5E, 0x3F5F, 0x3F60, 0x3F61, 0x3F62, 0x3F63, 0x3F64, 0x3F65, 0x3F66, 0x3F67, 0x3F68, 0x3F82, 0x3F83, 0x3F84, 0x3F85, 0x3F86, 0x3F87, 0x3F88, 0x3F89, 0x3F8A, 0x3F8B, 0x3F8C, 0x3F8D, 0x3F8E, 0x3F8F, 0x3F92, 0x3F93, 0x3F94, 0x3F95, 0x3F96, 0x3F97, 0x3F98, 0x3F99, 0x3F9A, 0x3F9B, 0x3F9C, 0x3F9D, 0x3F9E, 0x3F9F, 0x3FA0, 0x3FA1, 0x3FA2, 0x3FA3, 0x3FA4, 0x3FA5, 0x3FA6, 0x3FA7, 0x3FA8, 0x3FA9, 0x3FAA, 0x3FAB, 0x3FAC, 0x3FAD, 0x3FAE, 0x3FAF, 0x3FB0, 0x3FB1, 0x3FB2, 0x3FB3, 0x3FB4, 0x3FB5, 0x3FB6, 0x3FB7, 0x3FB8, 0x3FB9, 0x3FBA, 0x3FBB, 0x3FBC, 0x3FBD, 0x3FBE, 0x3FBF, 0x3FC0, 0x3FC1, 0x3FC2, 0x3FC3, 0x3FC4, 0x3FC5, 0x3FC6, 0x3FC7, 0x3FC8, 0x3FC9, 0x3FCA, 0x3FCB, 0x3FCC, 0x3FCD, 0x3FCE, 0x3FCF, 0x0436, 0x0437, 0x0438, 0x0439, 0x043A, 0x043B, 0x043C, 0x043D, 0x043E, 0x043F, 0x0440, 0x0441, 0x0442, 0x0443, 0x0444, 0x0445 };
			int[] SandTerrain = { 0x001A, 0x001B, 0x001C, 0x001D, 0x001E, 0x001F, 0x0020, 0x0021, 0x0022, 0x0023, 0x0024, 0x0025, 0x0026, 0x0027, 0x0028, 0x0029, 0x002A, 0x002B, 0x002C, 0x002D, 0x002E, 0x002F, 0x0030, 0x0031, 0x0032, 0x0044, 0x0045, 0x0046, 0x0047, 0x0048, 0x0049, 0x004A, 0x004B, 0x0126, 0x0127, 0x0128, 0x0129, 0x01B9, 0x01BA, 0x01BB, 0x01BC, 0x01BD, 0x01BE, 0x01BF, 0x01C0, 0x01C1, 0x01C2, 0x01C3, 0x01C4, 0x01C5, 0x01C6, 0x01C7, 0x01C8, 0x01C9, 0x01CA, 0x01CB, 0x01CC, 0x01CD, 0x01CE, 0x01CF, 0x01D0, 0x01D1, 0x0016, 0x0017, 0x0018, 0x0019, 0x0033, 0x0034, 0x0035, 0x0036, 0x0037, 0x0038, 0x0039, 0x003A, 0x011E, 0x011F, 0x0120, 0x0121, 0x012A, 0x012B, 0x012C, 0x012D, 0x01A8, 0x01A9, 0x01AA, 0x01AB, 0x0282, 0x0283, 0x0284, 0x0285, 0x028A, 0x028B, 0x028C, 0x028D, 0x028E, 0x028F, 0x0290, 0x0291, 0x0335, 0x0336, 0x0337, 0x0338, 0x0339, 0x033A, 0x033B, 0x033C, 0x0341, 0x0342, 0x0343, 0x0344, 0x034D, 0x034E, 0x034F, 0x0350, 0x0351, 0x0352, 0x0353, 0x0354, 0x0359, 0x035A, 0x035B, 0x035C, 0x03B7, 0x03B8, 0x03B9, 0x03BA, 0x03BB, 0x03BC, 0x03BD, 0x03BE, 0x03C7, 0x03C8, 0x03C9, 0x03CA, 0x05A7, 0x05A8, 0x05A9, 0x05AA, 0x05AB, 0x05AC, 0x05AD, 0x05AE, 0x05AF, 0x05B0, 0x05B1, 0x05B2, 0x064B, 0x064C, 0x064D, 0x064E, 0x064F, 0x0650, 0x0651, 0x0652, 0x0657, 0x0658, 0x0659, 0x065A, 0x0663, 0x0664, 0x0665, 0x0666, 0x0667, 0x0668, 0x0669, 0x066A, 0x066F, 0x0670, 0x0671, 0x0672 };
			int[] SwampTerrain = { 0x3D65, 0x3D66, 0x3D67, 0x3D68, 0x3D69, 0x3D6A, 0x3D6B, 0x3D6C, 0x3D6D, 0x3D6E, 0x3D6F, 0x3D70, 0x3D71, 0x3D72, 0x3D73, 0x3D74, 0x3D75, 0x3D76, 0x3D77, 0x3D78, 0x3D79, 0x3D7A, 0x3D7B, 0x3D7C, 0x3D7D, 0x3D7E, 0x3D7F, 0x3D80, 0x3D81, 0x3D82, 0x3D83, 0x3D84, 0x3D85, 0x3D86, 0x3D87, 0x3D88, 0x3D89, 0x3D8A, 0x3D8B, 0x3D8C, 0x3D8D, 0x3D8E, 0x3D8F, 0x3D90, 0x3D91, 0x3D92, 0x3D93, 0x3D94, 0x3D95, 0x3D96, 0x3D97, 0x3D98, 0x3D99, 0x3D9A, 0x3D9B, 0x3D9C, 0x3D9D, 0x3D9E, 0x3D9F, 0x3DA0, 0x3DA1, 0x3DA2, 0x3DA3, 0x3DA4, 0x3DA5, 0x3DA6, 0x3DA7, 0x3DA8, 0x3DA9, 0x3DAA, 0x3DAB, 0x3DAC, 0x3DAD, 0x3DAE, 0x3DAF, 0x3DB0, 0x3DB1, 0x3DB2, 0x3DB3, 0x3DB4, 0x3DB5, 0x3DB6, 0x3DB7, 0x3DB8, 0x3DB9, 0x3DBA, 0x3DBB, 0x3DBC, 0x3DBD, 0x3DBE, 0x3DBF, 0x3DC0, 0x3DC1, 0x3DC2, 0x3DC3, 0x3DC4, 0x3DC5, 0x3DC6, 0x3DC7, 0x3DC8, 0x3DC9, 0x3DCA, 0x3DCB, 0x3DCC, 0x3DCD, 0x3DCE, 0x3DCF, 0x3DD0, 0x3DD1, 0x3DD2, 0x3DD3, 0x3DD4, 0x3DD5, 0x3DD6, 0x3DD7, 0x3DD8, 0x3DD9, 0x3DDA, 0x3DDB, 0x3DDC, 0x3DDD, 0x3DDE, 0x3DDF, 0x3DE0, 0x3DE1, 0x3DE2, 0x3DE3, 0x3DE4, 0x3DE5, 0x3DE6, 0x3DE7, 0x3DE8, 0x3DE9, 0x3DEA, 0x3DEB, 0x3DEC, 0x3DED, 0x3DEE, 0x3DEF, 0x3DF0, 0x3DF1 };
			int[] SnowTerrain = { 0x010C, 0x010D, 0x010E, 0x010F, 0x0114, 0x0115, 0x0116, 0x0117, 0x017C, 0x017D, 0x017E, 0x017F, 0x0180, 0x0181, 0x0182, 0x0183, 0x0184, 0x0185, 0x0186, 0x0187, 0x0188, 0x0189, 0x018A, 0x0755, 0x0756, 0x0757, 0x0758, 0x076D, 0x076E, 0x076F, 0x0770, 0x0771, 0x0772, 0x0773, 0x011A, 0x011B, 0x011C, 0x011D, 0x012E, 0x012F, 0x0130, 0x0131, 0x0179, 0x017A, 0x017B, 0x0385, 0x0386, 0x0387, 0x0388, 0x0389, 0x038A, 0x038B, 0x038C, 0x0391, 0x0392, 0x0393, 0x0394, 0x039D, 0x039E, 0x039F, 0x03A0, 0x03A1, 0x03A2, 0x03A3, 0x03A4, 0x03A9, 0x03AA, 0x03AB, 0x03AC, 0x05BF, 0x05C0, 0x05C1, 0x05C2, 0x05C3, 0x05C4, 0x05C5, 0x05C6, 0x05C7, 0x05C8, 0x05C9, 0x05CA, 0x05CB, 0x05CC, 0x05CD, 0x05CE, 0x05CF, 0x05D0, 0x05D1, 0x05D2, 0x05D3, 0x05D4, 0x05D5, 0x05D6, 0x05DF, 0x05E0, 0x05E1, 0x05E2, 0x0745, 0x0746, 0x0747, 0x0748, 0x0751, 0x0752, 0x0753, 0x0754, 0x075D, 0x075E, 0x075F, 0x0760 };

			// YEW FOREST PATCH
			bool yew = false;
			if ( map == Map.Sosaria && JungleTerrain.Contains( LTile.ID ) )
			{
				if (
					( x >= 2089 && y >= 841 && x <= 2207 && y <= 1001 ) || 
					( x >= 2162 && y >= 679 && x <= 2358 && y <= 1077 ) || 
					( x >= 2335 && y >= 660 && x <= 2621 && y <= 1117 ) || 
					( x >= 2610 && y >= 718 && x <= 2747 && y <= 1025 )
				)
					yew = true;
			}

			if ( InBuilding( map, x, y, location.Z ) )
				return Terrain.Building;
			else if ( WaterTerrain.Contains( LTile.ID ) )
				return Terrain.Water;
			else if ( CaveTerrain.Contains( LTile.ID ) )
				return Terrain.Cave;
			else if ( DirtTerrain.Contains( LTile.ID ) )
				return Terrain.Dirt;
			else if ( ForestTerrain.Contains( LTile.ID ) || yew )
				return Terrain.Forest;
			else if ( GrassTerrain.Contains( LTile.ID ) )
				return Terrain.Grass;
			else if ( JungleTerrain.Contains( LTile.ID ) )
				return Terrain.Jungle;
			else if ( RockTerrain.Contains( LTile.ID ) )
				return Terrain.Rock;
			else if ( SandTerrain.Contains( LTile.ID ) )
				return Terrain.Sand;
			else if ( SwampTerrain.Contains( LTile.ID ) )
				return Terrain.Swamp;
			else if ( SnowTerrain.Contains( LTile.ID ) )
				return Terrain.Snow;

			return Terrain.None;
        }

		public static Terrain TestAround ( Terrain ground, Map map, int x, int y, int distance )
		{
			Point3D p = new Point3D( 9000, 9000, 0 );
			Terrain terrain = Terrain.None;
			int i = 8;
			int g = 0;

			while ( i > 0 )
			{
				i--;
				g = Utility.Random(8);

				switch ( g )
				{
					case 0: terrain = GetTerrain( map, p, x-distance, y-distance ); break;
					case 1: terrain = GetTerrain( map, p, x, y-distance ); break;
					case 2: terrain = GetTerrain( map, p, x+distance, y-distance ); break;
					case 3: terrain = GetTerrain( map, p, x-distance, y ); break;
					case 4: terrain = GetTerrain( map, p, x+distance, y ); break;
					case 5: terrain = GetTerrain( map, p, x-distance, y+distance ); break;
					case 6: terrain = GetTerrain( map, p, x, y+distance ); break;
					case 7: terrain = GetTerrain( map, p, x+distance, y+distance ); break;
				}

				if ( terrain != Terrain.Water && terrain != Terrain.Cave && terrain != Terrain.Dirt && terrain != Terrain.Rock )

				return terrain;
			}

			return ground;
		}

		public static bool InBuilding( Map map, int x, int y, int z )
		{
			bool indoors = false;

			if ( map == Map.Atlantis )
			{
				//indoors = true; 
			}
			else if ( map == Map.Sosaria )
			{
				// BRITAIN
				if ( x >= 2971 && y >= 991 && x <= 2985 && y <= 998 ){ indoors = true; }
				else if ( x >= 2950 && y >= 990 && x <= 2965 && y <= 998 ){ indoors = true; }
				else if ( x >= 2939 && y >= 985 && x <= 2944 && y <= 990 ){ indoors = true; }
				else if ( x >= 2940 && y >= 1000 && x <= 2950 && y <= 1016 ){ indoors = true; }
				else if ( x >= 2970 && y >= 1008 && x <= 2985 && y <= 1018 ){ indoors = true; }
				else if ( x >= 2970 && y >= 1022 && x <= 2985 && y <= 1031 ){ indoors = true; }
				else if ( x >= 2967 && y >= 1033 && x <= 2985 && y <= 1053 ){ indoors = true; }
				else if ( x >= 2941 && y >= 1020 && x <= 2950 && y <= 1027 ){ indoors = true; }
				else if ( x >= 2941 && y >= 1027 && x <= 2954 && y <= 1036 ){ indoors = true; }
				else if ( x >= 2956 && y >= 1058 && x <= 2967 && y <= 1068 ){ indoors = true; }
				else if ( x >= 2986 && y >= 1095 && x <= 2995 && y <= 1108 ){ indoors = true; }
				else if ( x >= 3005 && y >= 1103 && x <= 3016 && y <= 1110 ){ indoors = true; }
				else if ( x >= 3020 && y >= 1104 && x <= 3030 && y <= 1110 ){ indoors = true; }
				else if ( x >= 3033 && y >= 1094 && x <= 3039 && y <= 1105 ){ indoors = true; }
				else if ( x >= 3039 && y >= 1045 && x <= 3046 && y <= 1056 ){ indoors = true; }
				else if ( x >= 3020 && y >= 1034 && x <= 3030 && y <= 1042 ){ indoors = true; }
				else if ( x >= 3020 && y >= 1024 && x <= 3030 && y <= 1032 ){ indoors = true; }
				else if ( x >= 3021 && y >= 1007 && x <= 3030 && y <= 1021 ){ indoors = true; }
				else if ( x >= 3004 && y >= 1006 && x <= 3014 && y <= 1018 ){ indoors = true; }
				else if ( x >= 2955 && y >= 893 && x <= 2967 && y <= 904 ){ indoors = true; }
				else if ( x >= 2983 && y >= 895 && x <= 2999 && y <= 898 ){ indoors = true; }
				else if ( x >= 3015 && y >= 893 && x <= 3027 && y <= 904 ){ indoors = true; }
				else if ( x >= 3015 && y >= 957 && x <= 3027 && y <= 968 ){ indoors = true; }
				else if ( x >= 2985 && y >= 963 && x <= 2997 && y <= 967 ){ indoors = true; }
				else if ( x >= 2955 && y >= 957 && x <= 2967 && y <= 968 ){ indoors = true; }
				else if ( x >= 2959 && y >= 898 && x <= 3023 && y <= 963 ){ indoors = true; }
				else if ( x >= 3062 && y >= 963 && x <= 3067 && y <= 967 ){ indoors = true; }
				else if ( x >= 3044 && y >= 1015 && x <= 3055 && y <= 1026 ){ indoors = true; }
				else if ( x >= 3051 && y >= 1026 && x <= 3055 && y <= 1057 ){ indoors = true; }
				else if ( x >= 3051 && y >= 1064 && x <= 3055 && y <= 1127 ){ indoors = true; }
				else if ( x >= 3003 && y >= 1123 && x <= 3055 && y <= 1127 ){ indoors = true; }
				else if ( x >= 2929 && y >= 1123 && x <= 2996 && y <= 1127 ){ indoors = true; }
				else if ( x >= 2929 && y >= 1065 && x <= 2933 && y <= 1127 ){ indoors = true; }
				else if ( x >= 2929 && y >= 986 && x <= 2938 && y <= 993 ){ indoors = true; }
				else if ( x >= 2929 && y >= 986 && x <= 2933 && y <= 1060 ){ indoors = true; }
				else if ( x >= 2947 && y >= 1083 && x <= 2952 && y <= 1088 ){ indoors = true; }
				else if ( x >= 2994 && y >= 1123 && x <= 3005 && y <= 1127 && z >= 20 ){ indoors = true; }
				else if ( x >= 2929 && y >= 1053 && x <= 2933 && y <= 1072 && z >= 20 ){ indoors = true; }
				else if ( x >= 3051 && y >= 1053 && x <= 3055 && y <= 1071 && z >= 20 ){ indoors = true; }
				else if ( x >= 2631 && y >= 3221 && x <= 2748 && y <= 3346 ){ indoors = true; }
				// MONTOR
				else if ( x >= 3070 && y >= 2571 && x <= 3085 && y <= 2580 ){ indoors = true; }
				else if ( x >= 3093 && y >= 2574 && x <= 3098 && y <= 2579 ){ indoors = true; }
				else if ( x >= 3103 && y >= 2581 && x <= 3114 && y <= 2588 ){ indoors = true; }
				else if ( x >= 3131 && y >= 2581 && x <= 3143 && y <= 2588 ){ indoors = true; }
				else if ( x >= 3111 && y >= 2568 && x <= 3134 && y <= 2577 ){ indoors = true; }
				else if ( x >= 3117 && y >= 2577 && x <= 3128 && y <= 2588 ){ indoors = true; }
				else if ( x >= 3163 && y >= 2572 && x <= 3177 && y <= 2580 ){ indoors = true; }
				else if ( x >= 3143 && y >= 2597 && x <= 3160 && y <= 2605 ){ indoors = true; }
				else if ( x >= 3085 && y >= 2597 && x <= 3094 && y <= 2605 ){ indoors = true; }
				else if ( x >= 3070 && y >= 2625 && x <= 3085 && y <= 2635 ){ indoors = true; }
				else if ( x >= 3074 && y >= 2638 && x <= 3085 && y <= 2650 ){ indoors = true; }
				else if ( x >= 3142 && y >= 2615 && x <= 3161 && y <= 2623 ){ indoors = true; }
				else if ( x >= 3143 && y >= 2619 && x <= 3151 && y <= 2632 ){ indoors = true; }
				else if ( x >= 3142 && y >= 2635 && x <= 3149 && y <= 2649 ){ indoors = true; }
				else if ( x >= 3151 && y >= 2634 && x <= 3159 && y <= 2647 ){ indoors = true; }
				else if ( x >= 3196 && y >= 2615 && x <= 3204 && y <= 2626 ){ indoors = true; }
				else if ( x >= 3236 && y >= 2586 && x <= 3243 && y <= 2595 ){ indoors = true; }
				else if ( x >= 3260 && y >= 2577 && x <= 3272 && y <= 2585 ){ indoors = true; }
				else if ( x >= 3283 && y >= 2573 && x <= 3295 && y <= 2580 ){ indoors = true; }
				else if ( x >= 3300 && y >= 2568 && x <= 3324 && y <= 2580 ){ indoors = true; }
				else if ( x >= 3328 && y >= 2573 && x <= 3342 && y <= 2580 ){ indoors = true; }
				else if ( x >= 3356 && y >= 2576 && x <= 3369 && y <= 2585 ){ indoors = true; }
				else if ( x >= 3340 && y >= 2594 && x <= 3356 && y <= 2603 ){ indoors = true; }
				else if ( x >= 3317 && y >= 2594 && x <= 3333 && y <= 2603 ){ indoors = true; }
				else if ( x >= 3355 && y >= 2638 && x <= 3370 && y <= 2647 ){ indoors = true; }
				else if ( x >= 3306 && y >= 2642 && x <= 3321 && y <= 2651 ){ indoors = true; }
				else if ( x >= 3259 && y >= 2638 && x <= 3272 && y <= 2647 ){ indoors = true; }
				else if ( x >= 3262 && y >= 2650 && x <= 3272 && y <= 2657 ){ indoors = true; }
				else if ( x >= 3174 && y >= 2600 && x <= 3181 && y <= 2608 ){ indoors = true; }
				else if ( x >= 3181 && y >= 2602 && x <= 3193 && y <= 2606 ){ indoors = true; }
				else if ( x >= 3174 && y >= 2614 && x <= 3181 && y <= 2622 ){ indoors = true; }
				else if ( x >= 3181 && y >= 2616 && x <= 3193 && y <= 2620 ){ indoors = true; }
				else if ( x >= 3189 && y >= 2616 && x <= 3193 && y <= 2663 ){ indoors = true; }
				else if ( x >= 3068 && y >= 2613 && x <= 3075 && y <= 2621 ){ indoors = true; }
				else if ( x >= 3057 && y >= 2615 && x <= 3068 && y <= 2619 ){ indoors = true; }
				else if ( x >= 3057 && y >= 2615 && x <= 3061 && y <= 2659 ){ indoors = true; }
				else if ( x >= 3057 && y >= 2659 && x <= 3193 && y <= 2663 ){ indoors = true; }
				else if ( x >= 3068 && y >= 2600 && x <= 3075 && y <= 2608 ){ indoors = true; }
				else if ( x >= 3057 && y >= 2602 && x <= 3068 && y <= 2606 ){ indoors = true; }
				else if ( x >= 3057 && y >= 2561 && x <= 3061 && y <= 2606 ){ indoors = true; }
				else if ( x >= 3057 && y >= 2561 && x <= 3193 && y <= 2565 ){ indoors = true; }
				else if ( x >= 3189 && y >= 2561 && x <= 3193 && y <= 2606 ){ indoors = true; }
				else if ( x >= 3181 && y >= 2602 && x <= 3193 && y <= 2606 ){ indoors = true; }
				// KULDAR
				else if ( x >= 6627 && y >= 1827 && x <= 6634 && y <= 1834 ){ indoors = true; }
				else if ( x >= 6627 && y >= 1843 && x <= 6635 && y <= 1851 ){ indoors = true; }
				else if ( x >= 6643 && y >= 1843 && x <= 6651 && y <= 1851 ){ indoors = true; }
				else if ( x >= 6632 && y >= 1831 && x <= 6647 && y <= 1847 ){ indoors = true; }
				else if ( x >= 6703 && y >= 1815 && x <= 6711 && y <= 1823 ){ indoors = true; }
				else if ( x >= 6708 && y >= 1848 && x <= 6719 && y <= 1863 ){ indoors = true; }
				else if ( x >= 6720 && y >= 1856 && x <= 6727 && y <= 1863 ){ indoors = true; }
				else if ( x >= 6743 && y >= 1823 && x <= 6759 && y <= 1831 ){ indoors = true; }
				else if ( x >= 6768 && y >= 1824 && x <= 6779 && y <= 1831 ){ indoors = true; }
				else if ( x >= 6787 && y >= 1816 && x <= 6795 && y <= 1827 ){ indoors = true; }
				else if ( x >= 6767 && y >= 1839 && x <= 6779 && y <= 1847 ){ indoors = true; }
				else if ( x >= 6767 && y >= 1839 && x <= 6775 && y <= 1855 ){ indoors = true; }
				else if ( x >= 6744 && y >= 1808 && x <= 6767 && y <= 1815 ){ indoors = true; }
				else if ( x >= 6761 && y >= 1787 && x <= 6773 && y <= 1795 ){ indoors = true; }
				else if ( x >= 6761 && y >= 1787 && x <= 6769 && y <= 1799 ){ indoors = true; }
				else if ( x >= 6746 && y >= 1782 && x <= 6753 && y <= 1797 ){ indoors = true; }
				else if ( x >= 6759 && y >= 1767 && x <= 6783 && y <= 1779 ){ indoors = true; }
				else if ( x >= 6792 && y >= 1760 && x <= 6803 && y <= 1767 ){ indoors = true; }
				else if ( x >= 6792 && y >= 1760 && x <= 6799 && y <= 1771 ){ indoors = true; }
				else if ( x >= 6735 && y >= 1763 && x <= 6750 && y <= 1773 ){ indoors = true; }
				else if ( x >= 6731 && y >= 1749 && x <= 6747 && y <= 1757 ){ indoors = true; }
				else if ( x >= 6717 && y >= 1750 && x <= 6727 && y <= 1769 ){ indoors = true; }
				else if ( x >= 6703 && y >= 1717 && x <= 6711 && y <= 1725 && z < 35 ){ indoors = true; }
				else if ( x >= 6703 && y >= 1733 && x <= 6711 && y <= 1741 && z < 35 ){ indoors = true; }
				else if ( x >= 6719 && y >= 1737 && x <= 6723 && y <= 1743 && z < 35 ){ indoors = true; }
				else if ( x >= 6720 && y >= 1743 && x <= 6749 && y <= 1747 && z < 35 ){ indoors = true; }
				else if ( x >= 6749 && y >= 1739 && x <= 6757 && y <= 1751 && z < 35 ){ indoors = true; }
				else if ( x >= 6785 && y >= 1739 && x <= 6793 && y <= 1751 && z < 35 ){ indoors = true; }
				else if ( x >= 6793 && y >= 1743 && x <= 6805 && y <= 1747 && z < 35 ){ indoors = true; }
				else if ( x >= 6805 && y >= 1739 && x <= 6813 && y <= 1751 && z < 35 ){ indoors = true; }
				else if ( x >= 6793 && y >= 1687 && x <= 6801 && y <= 1696 && z < 15 ){ indoors = true; }
				else if ( x >= 6797 && y >= 1695 && x <= 6801 && y <= 1707 && z < 15 ){ indoors = true; }
				else if ( x >= 6798 && y >= 1707 && x <= 6809 && y <= 1711 && z < 15 ){ indoors = true; }
				else if ( x >= 6805 && y >= 1708 && x <= 6809 && y <= 1713 && z < 15 ){ indoors = true; }
				else if ( x >= 6805 && y >= 1713 && x <= 6813 && y <= 1721 && z < 15 ){ indoors = true; }
				else if ( x >= 6802 && y >= 1695 && x <= 6809 && y <= 1705 ){ indoors = true; }
				else if ( x >= 6800 && y >= 1668 && x <= 6811 && y <= 1679 ){ indoors = true; }
				else if ( x >= 6803 && y >= 1663 && x <= 6811 && y <= 1679 ){ indoors = true; }
				else if ( x >= 6791 && y >= 1616 && x <= 6807 && y <= 1631 ){ indoors = true; }
				else if ( x >= 6779 && y >= 1647 && x <= 6795 && y <= 1663 ){ indoors = true; }
				else if ( x >= 6732 && y >= 1652 && x <= 6771 && y <= 1671 ){ indoors = true; }
				else if ( x >= 6711 && y >= 1647 && x <= 6723 && y <= 1655 ){ indoors = true; }
				else if ( x >= 6711 && y >= 1647 && x <= 6717 && y <= 1663 ){ indoors = true; }
				else if ( x >= 6711 && y >= 1679 && x <= 6727 && y <= 1687 ){ indoors = true; }
				else if ( x >= 6724 && y >= 1696 && x <= 6735 && y <= 1703 ){ indoors = true; }
				else if ( x >= 6735 && y >= 1679 && x <= 6755 && y <= 1687 ){ indoors = true; }
				else if ( x >= 6743 && y >= 1679 && x <= 6755 && y <= 1703 ){ indoors = true; }
				else if ( x >= 6723 && y >= 1711 && x <= 6735 && y <= 1719 ){ indoors = true; }
				else if ( x >= 6703 && y >= 1691 && x <= 6715 && y <= 1715 ){ indoors = true; }
				else if ( x >= 6743 && y >= 1711 && x <= 6751 && y <= 1723 ){ indoors = true; }
				else if ( x >= 6743 && y >= 1711 && x <= 6755 && y <= 1719 ){ indoors = true; }
				else if ( x >= 6764 && y >= 1696 && x <= 6779 && y <= 1703 ){ indoors = true; }
				else if ( x >= 6776 && y >= 1684 && x <= 6783 && y <= 1694 ){ indoors = true; }
				else if ( x >= 6764 && y >= 1680 && x <= 6774 && y <= 1687 ){ indoors = true; }
				else if ( x >= 6764 && y >= 1712 && x <= 6775 && y <= 1719 ){ indoors = true; }
				// RENIKA
				else if ( x >= 1370 && y >= 3633 && x <= 1417 && y <= 3661 ){ indoors = true; }
				else if ( x >= 1414 && y >= 3637 && x <= 1421 && y <= 3644 ){ indoors = true; }
				else if ( x >= 1414 && y >= 3661 && x <= 1421 && y <= 3668 ){ indoors = true; }
				else if ( x >= 1388 && y >= 3661 && x <= 1395 && y <= 3668 ){ indoors = true; }
				else if ( x >= 1398 && y >= 3662 && x <= 1411 && y <= 3667 ){ indoors = true; }
				else if ( x >= 1387 && y >= 3777 && x <= 1403 && y <= 3789 ){ indoors = true; }
				else if ( x >= 1425 && y >= 3757 && x <= 1436 && y <= 3764 ){ indoors = true; }
				else if ( x >= 1439 && y >= 3752 && x <= 1445 && y <= 3764 ){ indoors = true; }
				else if ( x >= 1419 && y >= 3767 && x <= 1427 && y <= 3777 ){ indoors = true; }
				else if ( x >= 1418 && y >= 3779 && x <= 1427 && y <= 3791 ){ indoors = true; }
				else if ( x >= 1454 && y >= 3766 && x <= 1464 && y <= 3773 ){ indoors = true; }
				else if ( x >= 1468 && y >= 3766 && x <= 1479 && y <= 3773 ){ indoors = true; }
				else if ( x >= 1434 && y >= 3791 && x <= 1445 && y <= 3799 ){ indoors = true; }
				else if ( x >= 1438 && y >= 3802 && x <= 1445 && y <= 3809 ){ indoors = true; }
				else if ( x >= 1454 && y >= 3781 && x <= 1476 && y <= 3789 ){ indoors = true; }
				else if ( x >= 1453 && y >= 3802 && x <= 1460 && y <= 3809 ){ indoors = true; }
				// BANDIT CAMP
				else if ( x >= 3005 && y >= 383 && x <= 3009 && y <= 388 ){ indoors = true; }
				else if ( x >= 3005 && y >= 370 && x <= 3014 && y <= 377 ){ indoors = true; }
				// DARK TOWER
				else if ( x >= 3504 && y >= 2277 && x <= 3512 && y <= 2289 ){ indoors = true; }
				else if ( x >= 3503 && y >= 2278 && x <= 3513 && y <= 2288 ){ indoors = true; }
				else if ( x >= 3502 && y >= 2279 && x <= 3514 && y <= 2287 ){ indoors = true; }
				// DARK FORTRESS
				else if ( x >= 3779 && y >= 1846 && x <= 3786 && y <= 1857 ){ indoors = true; }
				else if ( x >= 3786 && y >= 1846 && x <= 3797 && y <= 1861 ){ indoors = true; }
				else if ( x >= 3797 && y >= 1848 && x <= 3812 && y <= 1862 ){ indoors = true; }
				else if ( x >= 3804 && y >= 1846 && x <= 3812 && y <= 1864 ){ indoors = true; }
				else if ( x >= 3786 && y >= 1854 && x <= 3807 && y <= 1861 ){ indoors = true; }
				else if ( x >= 3787 && y >= 1861 && x <= 3812 && y <= 1876 ){ indoors = true; }
				else if ( x >= 3786 && y >= 1864 && x <= 3812 && y <= 1876 ){ indoors = true; }
				else if ( x >= 3794 && y >= 1851 && x <= 3809 && y <= 1873 ){ indoors = true; }
				// LIGHTHOUSE
				else if ( x >= 3191 && y >= 507 && x <= 3200 && y <= 520 ){ indoors = true; }
				// MANGARS TOWER
				else if ( x >= 2823 && y >= 1870 && x <= 2834 && y <= 1878 ){ indoors = true; }
				// PIRATE ISLE
				else if ( x >= 1823 && y >= 2219 && x <= 1830 && y <= 2226 ){ indoors = true; }
				else if ( x >= 1807 && y >= 2219 && x <= 1814 && y <= 2226 ){ indoors = true; }
				else if ( x >= 1823 && y >= 2235 && x <= 1830 && y <= 2242 ){ indoors = true; }
				else if ( x >= 1803 && y >= 2254 && x <= 1807 && y <= 2258 ){ indoors = true; }
				// DAWN
				else if ( x >= 5903 && y >= 2875 && x <= 5910 && y <= 2881 ){ indoors = true; }
				else if ( x >= 5903 && y >= 2842 && x <= 5910 && y <= 2849 ){ indoors = true; }
				else if ( x >= 5903 && y >= 2848 && x <= 5905 && y <= 2875 ){ indoors = true; }
				else if ( x >= 5908 && y >= 2842 && x <= 6014 && y <= 2844 ){ indoors = true; }
				else if ( x >= 6014 && y >= 2842 && x <= 6021 && y <= 2849 ){ indoors = true; }
				else if ( x >= 6019 && y >= 2849 && x <= 6021 && y <= 2882 ){ indoors = true; }
				else if ( x >= 6019 && y >= 2887 && x <= 6021 && y <= 2903 ){ indoors = true; }
				else if ( x >= 5989 && y >= 2901 && x <= 6021 && y <= 2903 ){ indoors = true; }
				else if ( x >= 5989 && y >= 2901 && x <= 5991 && y <= 2924 ){ indoors = true; }
				else if ( x >= 5903 && y >= 2922 && x <= 5991 && y <= 2924 ){ indoors = true; }
				else if ( x >= 5903 && y >= 2887 && x <= 5910 && y <= 2893 ){ indoors = true; }
				else if ( x >= 5903 && y >= 2887 && x <= 5905 && y <= 2922 ){ indoors = true; }
				else if ( x >= 6019 && y >= 2881 && x <= 6021 && y <= 2889 && z > 20 ){ indoors = true; }
				else if ( x >= 6002 && y >= 2884 && x <= 6010 && y <= 2896 ){ indoors = true; }
				else if ( x >= 6011 && y >= 2888 && x <= 6014 && y <= 2896 ){ indoors = true; }
				else if ( x >= 6003 && y >= 2866 && x <= 6009 && y <= 2872 ){ indoors = true; }
				else if ( x >= 6009 && y >= 2867 && x <= 6016 && y <= 2879 ){ indoors = true; }
				else if ( x >= 6003 && y >= 2854 && x <= 6010 && y <= 2860 ){ indoors = true; }
				else if ( x >= 5981 && y >= 2857 && x <= 5993 && y <= 2864 ){ indoors = true; }
				else if ( x >= 5964 && y >= 2852 && x <= 5977 && y <= 2861 ){ indoors = true; }
				else if ( x >= 5945 && y >= 2848 && x <= 5959 && y <= 2855 ){ indoors = true; }
				else if ( x >= 5930 && y >= 2851 && x <= 5943 && y <= 2858 ){ indoors = true; }
				else if ( x >= 5920 && y >= 2872 && x <= 5937 && y <= 2876 ){ indoors = true; }
				else if ( x >= 5933 && y >= 2866 && x <= 5937 && y <= 2876 ){ indoors = true; }
				else if ( x >= 5913 && y >= 2886 && x <= 5922 && y <= 2900 ){ indoors = true; }
				else if ( x >= 5927 && y >= 2888 && x <= 5938 && y <= 2896 ){ indoors = true; }
				else if ( x >= 5928 && y >= 2904 && x <= 5938 && y <= 2911 ){ indoors = true; }
				else if ( x >= 5943 && y >= 2901 && x <= 5949 && y <= 2910 ){ indoors = true; }
				else if ( x >= 5951 && y >= 2901 && x <= 5958 && y <= 2910 ){ indoors = true; }
				else if ( x >= 5963 && y >= 2903 && x <= 5971 && y <= 2913 ){ indoors = true; }
				else if ( x >= 5975 && y >= 2905 && x <= 5985 && y <= 2913 ){ indoors = true; }
				// YEW
				else if ( x >= 2347 && y >= 874 && x <= 2355 && y <= 879 ){ indoors = true; }
				else if ( x >= 2397 && y >= 859 && x <= 2407 && y <= 865 ){ indoors = true; }
				else if ( x >= 2387 && y >= 888 && x <= 2400 && y <= 898 ){ indoors = true; }
				else if ( x >= 2410 && y >= 858 && x <= 2422 && y <= 865 ){ indoors = true; }
				else if ( x >= 2419 && y >= 887 && x <= 2424 && y <= 896 ){ indoors = true; }
				else if ( x >= 2432 && y >= 847 && x <= 2438 && y <= 857 ){ indoors = true; }
				else if ( x >= 2442 && y >= 847 && x <= 2448 && y <= 857 ){ indoors = true; }
				else if ( x >= 2454 && y >= 835 && x <= 2465 && y <= 845 ){ indoors = true; }
				else if ( x >= 2452 && y >= 865 && x <= 2464 && y <= 871 ){ indoors = true; }
				else if ( x >= 2492 && y >= 879 && x <= 2502 && y <= 886 ){ indoors = true; }
				else if ( x >= 2475 && y >= 888 && x <= 2485 && y <= 895 ){ indoors = true; }
				else if ( x >= 2509 && y >= 856 && x <= 2514 && y <= 866 ){ indoors = true; }
				// PORT
				else if ( x >= 7063 && y >= 703 && x <= 7087 && y <= 719 ){ indoors = true; }
				else if ( x >= 7031 && y >= 695 && x <= 7047 && y <= 703 ){ indoors = true; }
				else if ( x >= 7039 && y >= 679 && x <= 7063 && y <= 687 ){ indoors = true; }
				else if ( x >= 7047 && y >= 671 && x <= 7063 && y <= 687 ){ indoors = true; }
				else if ( x >= 7007 && y >= 687 && x <= 7015 && y <= 703 ){ indoors = true; }
				else if ( x >= 7007 && y >= 688 && x <= 7023 && y <= 695 ){ indoors = true; }
				else if ( x >= 6981 && y >= 693 && x <= 7000 && y <= 714 ){ indoors = true; }
				else if ( x >= 6998 && y >= 671 && x <= 7007 && y <= 679 ){ indoors = true; }
				else if ( x >= 6991 && y >= 662 && x <= 7000 && y <= 670 ){ indoors = true; }
				else if ( x >= 7015 && y >= 663 && x <= 7031 && y <= 679 ){ indoors = true; }
				else if ( x >= 7026 && y >= 663 && x <= 7039 && y <= 671 ){ indoors = true; }
				else if ( x >= 7047 && y >= 655 && x <= 7071 && y <= 663 ){ indoors = true; }
				else if ( x >= 7055 && y >= 647 && x <= 7063 && y <= 655 ){ indoors = true; }
				else if ( x >= 7071 && y >= 640 && x <= 7079 && y <= 647 ){ indoors = true; }
				else if ( x >= 7079 && y >= 640 && x <= 7087 && y <= 655 ){ indoors = true; }
				else if ( x >= 7055 && y >= 615 && x <= 7071 && y <= 631 ){ indoors = true; }
				else if ( x >= 7055 && y >= 626 && x <= 7063 && y <= 639 ){ indoors = true; }
				else if ( x >= 7079 && y >= 615 && x <= 7087 && y <= 623 ){ indoors = true; }
				else if ( x >= 7079 && y >= 624 && x <= 7095 && y <= 631 ){ indoors = true; }
				else if ( x >= 7071 && y >= 597 && x <= 7085 && y <= 609 ){ indoors = true; }
				else if ( x >= 7085 && y >= 589 && x <= 7102 && y <= 609 ){ indoors = true; }
				else if ( x >= 7047 && y >= 598 && x <= 7055 && y <= 607 ){ indoors = true; }
				else if ( x >= 7039 && y >= 616 && x <= 7047 && y <= 631 ){ indoors = true; }
				else if ( x >= 7015 && y >= 623 && x <= 7023 && y <= 639 ){ indoors = true; }
				else if ( x >= 7015 && y >= 623 && x <= 7031 && y <= 631 ){ indoors = true; }
				else if ( x >= 7015 && y >= 599 && x <= 7031 && y <= 615 ){ indoors = true; }
				else if ( x >= 6998 && y >= 632 && x <= 7007 && y <= 639 ){ indoors = true; }
				else if ( x >= 6998 && y >= 607 && x <= 7007 && y <= 615 ){ indoors = true; }
				else if ( x >= 6975 && y >= 608 && x <= 6983 && y <= 623 ){ indoors = true; }
				else if ( x >= 6975 && y >= 608 && x <= 6991 && y <= 615 ){ indoors = true; }
				else if ( x >= 6975 && y >= 632 && x <= 6991 && y <= 639 ){ indoors = true; }
				else if ( x >= 6960 && y >= 646 && x <= 6971 && y <= 659 ){ indoors = true; }
				else if ( x >= 6998 && y >= 631 && x <= 7007 && y <= 639 ){ indoors = true; }
				else if ( x >= 7031 && y >= 639 && x <= 7047 && y <= 647 ){ indoors = true; }
				else if ( x >= 7031 && y >= 648 && x <= 7039 && y <= 655 ){ indoors = true; }
				else if ( x >= 7007 && y >= 648 && x <= 7023 && y <= 655 ){ indoors = true; }
				// DEATH GULCH
				else if ( x >= 3696 && y >= 1557 && x <= 3704 && y <= 1569 ){ indoors = true; }
				else if ( x >= 3759 && y >= 1565 && x <= 3765 && y <= 1571 ){ indoors = true; }
				else if ( x >= 3765 && y >= 1562 && x <= 3778 && y <= 1572 ){ indoors = true; }
				else if ( x >= 3762 && y >= 1527 && x <= 3772 && y <= 1533 ){ indoors = true; }
				else if ( x >= 3778 && y >= 1494 && x <= 3783 && y <= 1499 ){ indoors = true; }
				else if ( x >= 3783 && y >= 1495 && x <= 3789 && y <= 1504 ){ indoors = true; }
				else if ( x >= 3689 && y >= 1514 && x <= 3692 && y <= 1523 ){ indoors = true; }
				else if ( x >= 3689 && y >= 1514 && x <= 3695 && y <= 1517 ){ indoors = true; }
				// DEVIL GUARD
				else if ( x >= 1560 && y >= 1398 && x <= 1569 && y <= 1403 ){ indoors = true; }
				else if ( x >= 6588 && y >= 3193 && x <= 6595 && y <= 3199 ){ indoors = true; }
				else if ( x >= 6588 && y >= 3193 && x <= 6591 && y <= 3201 ){ indoors = true; }
				else if ( x >= 6608 && y >= 3194 && x <= 6619 && y <= 3202 ){ indoors = true; }
				else if ( x >= 1599 && y >= 1463 && x <= 1609 && y <= 1471 ){ indoors = true; }
				else if ( x >= 1605 && y >= 1448 && x <= 1617 && y <= 1458 ){ indoors = true; }
				else if ( x >= 1620 && y >= 1449 && x <= 1631 && y <= 1458 ){ indoors = true; }
				else if ( x >= 1663 && y >= 1444 && x <= 1673 && y <= 1458 ){ indoors = true; }
				else if ( x >= 1676 && y >= 1449 && x <= 1689 && y <= 1458 ){ indoors = true; }
				else if ( x >= 1711 && y >= 1505 && x <= 1729 && y <= 1517 ){ indoors = true; }
				else if ( x >= 1732 && y >= 1505 && x <= 1739 && y <= 1517 ){ indoors = true; }
				else if ( x >= 1714 && y >= 1527 && x <= 1720 && y <= 1536 ){ indoors = true; }
				else if ( x >= 1729 && y >= 1526 && x <= 1738 && y <= 1535 ){ indoors = true; }
				else if ( x >= 1633 && y >= 1507 && x <= 1648 && y <= 1523 ){ indoors = true; }
				// FAWN
				else if ( x >= 2076 && y >= 247 && x <= 2089 && y <= 257 ){ indoors = true; }
				else if ( x >= 2061 && y >= 262 && x <= 2074 && y <= 270 ){ indoors = true; }
				else if ( x >= 2094 && y >= 247 && x <= 2107 && y <= 258 ){ indoors = true; }
				else if ( x >= 2088 && y >= 291 && x <= 2100 && y <= 306 ){ indoors = true; }
				else if ( x >= 2113 && y >= 247 && x <= 2119 && y <= 257 ){ indoors = true; }
				else if ( x >= 2122 && y >= 246 && x <= 2130 && y <= 264 ){ indoors = true; }
				else if ( x >= 2088 && y >= 291 && x <= 2100 && y <= 306 ){ indoors = true; }
				else if ( x >= 2103 && y >= 291 && x <= 2111 && y <= 306 ){ indoors = true; }
				else if ( x >= 2103 && y >= 291 && x <= 2117 && y <= 298 ){ indoors = true; }
				else if ( x >= 2171 && y >= 248 && x <= 2182 && y <= 254 ){ indoors = true; }
				else if ( x >= 2159 && y >= 254 && x <= 2169 && y <= 268 ){ indoors = true; }
				else if ( x >= 2159 && y >= 282 && x <= 2167 && y <= 294 ){ indoors = true; }
				else if ( x >= 2141 && y >= 298 && x <= 2154 && y <= 305 ){ indoors = true; }
				else if ( x >= 2158 && y >= 298 && x <= 2173 && y <= 305 ){ indoors = true; }
				else if ( x >= 2180 && y >= 268 && x <= 2187 && y <= 279 ){ indoors = true; }
				else if ( x >= 2180 && y >= 268 && x <= 2195 && y <= 274 ){ indoors = true; }
				else if ( x >= 2195 && y >= 268 && x <= 2204 && y <= 284 ){ indoors = true; }
				// GLACIAL COAST
				else if ( x >= 4738 && y >= 1151 && x <= 4746 && y <= 1161 ){ indoors = true; }
				else if ( x >= 4738 && y >= 1163 && x <= 4746 && y <= 1174 ){ indoors = true; }
				else if ( x >= 4717 && y >= 1170 && x <= 4723 && y <= 1177 ){ indoors = true; }
				else if ( x >= 4723 && y >= 1170 && x <= 4730 && y <= 1183 ){ indoors = true; }
				else if ( x >= 4751 && y >= 1167 && x <= 4763 && y <= 1174 ){ indoors = true; }
				else if ( x >= 4734 && y >= 1180 && x <= 4746 && y <= 1188 ){ indoors = true; }
				else if ( x >= 4738 && y >= 1190 && x <= 4746 && y <= 1200 ){ indoors = true; }
				else if ( x >= 4751 && y >= 1179 && x <= 4762 && y <= 1194 ){ indoors = true; }
				else if ( x >= 4767 && y >= 1179 && x <= 4789 && y <= 1186 ){ indoors = true; }
				// GREY
				else if ( x >= 838 && y >= 2014 && x <= 845 && y <= 2028 ){ indoors = true; }
				else if ( x >= 831 && y >= 2051 && x <= 837 && y <= 2061 ){ indoors = true; }
				else if ( x >= 834 && y >= 2066 && x <= 840 && y <= 2078 ){ indoors = true; }
				else if ( x >= 865 && y >= 2048 && x <= 876 && y <= 2055 ){ indoors = true; }
				else if ( x >= 885 && y >= 2048 && x <= 891 && y <= 2057 ){ indoors = true; }
				else if ( x >= 895 && y >= 2048 && x <= 901 && y <= 2057 ){ indoors = true; }
				else if ( x >= 904 && y >= 2042 && x <= 916 && y <= 2051 ){ indoors = true; }
				else if ( x >= 920 && y >= 2048 && x <= 929 && y <= 2057 ){ indoors = true; }
				else if ( x >= 887 && y >= 2067 && x <= 897 && y <= 2073 ){ indoors = true; }
				else if ( x >= 887 && y >= 2076 && x <= 897 && y <= 2083 ){ indoors = true; }
				else if ( x >= 921 && y >= 2072 && x <= 927 && y <= 2080 ){ indoors = true; }
				else if ( x >= 921 && y >= 2083 && x <= 927 && y <= 2091 ){ indoors = true; }
				else if ( x >= 915 && y >= 2094 && x <= 924 && y <= 2102 ){ indoors = true; }
				else if ( x >= 905 && y >= 2112 && x <= 912 && y <= 2124 ){ indoors = true; }
				// ICELAD VILLAGE
				else if ( x >= 4322 && y >= 1143 && x <= 4329 && y <= 1152 ){ indoors = true; }
				else if ( x >= 4315 && y >= 1158 && x <= 4321 && y <= 1168 ){ indoors = true; }
				else if ( x >= 4327 && y >= 1156 && x <= 4337 && y <= 1162 ){ indoors = true; }
				else if ( x >= 4319 && y >= 1172 && x <= 4325 && y <= 1183 ){ indoors = true; }
				else if ( x >= 4325 && y >= 1176 && x <= 4329 && y <= 1183 ){ indoors = true; }
				else if ( x >= 4306 && y >= 1170 && x <= 4315 && y <= 1177 ){ indoors = true; }
				// MOUNTAIN CREST
				else if ( x >= 4498 && y >= 1250 && x <= 4513 && y <= 1256 ){ indoors = true; }
				else if ( x >= 4516 && y >= 1245 && x <= 4523 && y <= 1256 ){ indoors = true; }
				else if ( x >= 4526 && y >= 1248 && x <= 4538 && y <= 1256 ){ indoors = true; }
				else if ( x >= 4540 && y >= 1252 && x <= 4547 && y <= 1256 ){ indoors = true; }
				else if ( x >= 4529 && y >= 1278 && x <= 4537 && y <= 1287 ){ indoors = true; }
				else if ( x >= 4517 && y >= 1278 && x <= 4527 && y <= 1283 ){ indoors = true; }
				else if ( x >= 4517 && y >= 1278 && x <= 4523 && y <= 1287 ){ indoors = true; }
				else if ( x >= 4514 && y >= 1264 && x <= 4528 && y <= 1272 ){ indoors = true; }
				else if ( x >= 4503 && y >= 1263 && x <= 4511 && y <= 1272 ){ indoors = true; }
				else if ( x >= 4501 && y >= 1277 && x <= 4511 && y <= 1283 ){ indoors = true; }
				else if ( x >= 4501 && y >= 1285 && x <= 4511 && y <= 1291 ){ indoors = true; }
				// HOMES
				else if ( x >= 963 && y >= 640 && x <= 971 && y <= 652 ){ indoors = true; }
				else if ( x >= 941 && y >= 629 && x <= 954 && y <= 636 ){ indoors = true; }
				else if ( x >= 979 && y >= 673 && x <= 992 && y <= 681 ){ indoors = true; }
				else if ( x >= 1003 && y >= 654 && x <= 1015 && y <= 661 ){ indoors = true; }
				else if ( x >= 909 && y >= 767 && x <= 915 && y <= 779 ){ indoors = true; }
				else if ( x >= 907 && y >= 787 && x <= 913 && y <= 798 ){ indoors = true; }
				else if ( x >= 2991 && y >= 1267 && x <= 3001 && y <= 1274 ){ indoors = true; }
				else if ( x >= 2998 && y >= 1274 && x <= 3001 && y <= 1279 ){ indoors = true; }
				else if ( x >= 2961 && y >= 1196 && x <= 2969 && y <= 1208 ){ indoors = true; }
				else if ( x >= 2939 && y >= 1185 && x <= 2952 && y <= 1192 ){ indoors = true; }
				else if ( x >= 2813 && y >= 999 && x <= 2819 && y <= 1010 ){ indoors = true; }
				else if ( x >= 2816 && y >= 980 && x <= 2821 && y <= 991 ){ indoors = true; }
				else if ( x >= 2765 && y >= 918 && x <= 2771 && y <= 929 ){ indoors = true; }
				else if ( x >= 2774 && y >= 900 && x <= 2786 && y <= 912 ){ indoors = true; }
				else if ( x >= 2777 && y >= 590 && x <= 2784 && y <= 602 ){ indoors = true; }
				else if ( x >= 2791 && y >= 588 && x <= 2802 && y <= 594 ){ indoors = true; }
				else if ( x >= 2688 && y >= 626 && x <= 2701 && y <= 633 ){ indoors = true; }
				else if ( x >= 2678 && y >= 593 && x <= 2686 && y <= 611 ){ indoors = true; }
				else if ( x >= 2642 && y >= 509 && x <= 2654 && y <= 516 ){ indoors = true; }
				else if ( x >= 2618 && y >= 512 && x <= 2628 && y <= 527 ){ indoors = true; }
				else if ( x >= 2613 && y >= 521 && x <= 2628 && y <= 527 ){ indoors = true; }
				// MOON
				else if ( x >= 791 && y >= 679 && x <= 806 && y <= 687 ){ indoors = true; }
				else if ( x >= 784 && y >= 697 && x <= 791 && y <= 707 ){ indoors = true; }
				else if ( x >= 803 && y >= 695 && x <= 814 && y <= 700 ){ indoors = true; }
				else if ( x >= 810 && y >= 714 && x <= 822 && y <= 722 ){ indoors = true; }
				else if ( x >= 826 && y >= 715 && x <= 837 && y <= 722 ){ indoors = true; }
				else if ( x >= 801 && y >= 741 && x <= 808 && y <= 753 ){ indoors = true; }
				else if ( x >= 802 && y >= 757 && x <= 808 && y <= 768 ){ indoors = true; }
				else if ( x >= 826 && y >= 715 && x <= 837 && y <= 722 ){ indoors = true; }
				else if ( x >= 854 && y >= 711 && x <= 860 && y <= 722 ){ indoors = true; }
				else if ( x >= 842 && y >= 732 && x <= 849 && y <= 744 ){ indoors = true; }
				else if ( x >= 837 && y >= 738 && x <= 849 && y <= 744 ){ indoors = true; }
				else if ( x >= 843 && y >= 682 && x <= 850 && y <= 691 ){ indoors = true; }
				else if ( x >= 836 && y >= 700 && x <= 849 && y <= 706 ){ indoors = true; }
			}
			else if ( map == Map.IslesDread )
			{
				// ISLES OF DREAD
				if ( x >= 6764 && y >= 1712 && x <= 6775 && y <= 1719 ){ indoors = true; }
				else if ( x >= 1223 && y >= 175 && x <= 1239 && y <= 183 ){ indoors = true; }
				else if ( x >= 1236 && y >= 175 && x <= 1257 && y <= 186 ){ indoors = true; }
				else if ( x >= 1255 && y >= 173 && x <= 1264 && y <= 182 ){ indoors = true; }
				else if ( x >= 1256 && y >= 175 && x <= 1261 && y <= 186 ){ indoors = true; }
				else if ( x >= 245 && y >= 1149 && x <= 265 && y <= 1169 ){ indoors = true; }
				else if ( x >= 248 && y >= 1184 && x <= 263 && y <= 1199 ){ indoors = true; }
				else if ( x >= 279 && y >= 1167 && x <= 287 && y <= 1183 ){ indoors = true; }
				else if ( x >= 283 && y >= 1168 && x <= 295 && y <= 1175 ){ indoors = true; }
				else if ( x >= 312 && y >= 1184 && x <= 319 && y <= 1199 ){ indoors = true; }
				else if ( x >= 304 && y >= 1192 && x <= 319 && y <= 1199 ){ indoors = true; }
				else if ( x >= 381 && y >= 1205 && x <= 401 && y <= 1225 ){ indoors = true; }
				else if ( x >= 325 && y >= 1165 && x <= 345 && y <= 1185 ){ indoors = true; }
				else if ( x >= 333 && y >= 1144 && x <= 337 && y <= 1168 && z >= 13 ){ indoors = true; }
				else if ( x >= 325 && y >= 1125 && x <= 343 && y <= 1145 ){ indoors = true; }
				else if ( x >= 384 && y >= 1138 && x <= 397 && y <= 1151 ){ indoors = true; }
				else if ( x >= 253 && y >= 1093 && x <= 273 && y <= 1113 ){ indoors = true; }
				else if ( x >= 343 && y >= 1063 && x <= 367 && y <= 1071 ){ indoors = true; }
				else if ( x >= 310 && y >= 1056 && x <= 329 && y <= 1073 ){ indoors = true; }
				else if ( x >= 309 && y >= 1029 && x <= 343 && y <= 1055 ){ indoors = true; }
				else if ( x >= 343 && y >= 1029 && x <= 374 && y <= 1047 ){ indoors = true; }
				else if ( x >= 364 && y >= 1029 && x <= 415 && y <= 1055 ){ indoors = true; }
				else if ( x >= 399 && y >= 1054 && x <= 415 && y <= 1057 ){ indoors = true; }
				else if ( x >= 413 && y >= 1029 && x <= 430 && y <= 1031 ){ indoors = true; }
				else if ( x >= 429 && y >= 1021 && x <= 446 && y <= 1041 ){ indoors = true; }
				else if ( x >= 429 && y >= 1021 && x <= 446 && y <= 1039 ){ indoors = true; }
				else if ( x >= 440 && y >= 1039 && x <= 446 && y <= 1041 ){ indoors = true; }
				else if ( x >= 424 && y >= 1032 && x <= 431 && y <= 1039 ){ indoors = true; }
				else if ( x >= 440 && y >= 1040 && x <= 441 && y <= 1081 ){ indoors = true; }
				else if ( x >= 440 && y >= 1070 && x <= 441 && y <= 1161 ){ indoors = true; }
				else if ( x >= 415 && y >= 1159 && x <= 441 && y <= 1161 ){ indoors = true; }
				else if ( x >= 415 && y >= 1159 && x <= 417 && y <= 1169 ){ indoors = true; }
				else if ( x >= 397 && y >= 1167 && x <= 417 && y <= 1169 ){ indoors = true; }
				else if ( x >= 399 && y >= 1149 && x <= 417 && y <= 1151 ){ indoors = true; }
				else if ( x >= 397 && y >= 1135 && x <= 399 && y <= 1169 ){ indoors = true; }
				else if ( x >= 375 && y >= 1135 && x <= 399 && y <= 1137 ){ indoors = true; }
				else if ( x >= 365 && y >= 1072 && x <= 367 && y <= 1087 ){ indoors = true; }
				else if ( x >= 333 && y >= 1085 && x <= 367 && y <= 1087 ){ indoors = true; }
				else if ( x >= 333 && y >= 1085 && x <= 351 && y <= 1103 ){ indoors = true; }
				else if ( x >= 334 && y >= 1103 && x <= 343 && y <= 1105 ){ indoors = true; }
				else if ( x >= 341 && y >= 1104 && x <= 351 && y <= 1119 ){ indoors = true; }
				else if ( x >= 342 && y >= 1114 && x <= 343 && y <= 1125 ){ indoors = true; }
				else if ( x >= 367 && y >= 1111 && x <= 375 && y <= 1127 ){ indoors = true; }
				else if ( x >= 383 && y >= 1111 && x <= 399 && y <= 1127 ){ indoors = true; }
				else if ( x >= 408 && y >= 1112 && x <= 415 && y <= 1119 ){ indoors = true; }
				else if ( x >= 424 && y >= 1104 && x <= 431 && y <= 1111 ){ indoors = true; }
				else if ( x >= 423 && y >= 1071 && x <= 439 && y <= 1095 ){ indoors = true; }
				else if ( x >= 424 && y >= 1048 && x <= 431 && y <= 1063 ){ indoors = true; }
				else if ( x >= 376 && y >= 1072 && x <= 391 && y <= 1079 ){ indoors = true; }
			}
			else if ( map == Map.Lodor )
			{
				// SKARA BRAE
				if ( x >= 7001 && y >= 184 && x <= 7023 && y <= 206 ){ indoors = true; }
				else if ( x >= 7005 && y >= 180 && x <= 7046 && y <= 184 ){ indoors = true; }
				else if ( x >= 7043 && y >= 180 && x <= 7047 && y <= 266 ){ indoors = true; }
				else if ( x >= 7000 && y >= 262 && x <= 7047 && y <= 266 ){ indoors = true; }
				else if ( x >= 7000 && y >= 262 && x <= 7004 && y <= 311 ){ indoors = true; }
				else if ( x >= 6861 && y >= 307 && x <= 7004 && y <= 311 ){ indoors = true; }
				else if ( x >= 6861 && y >= 131 && x <= 6886 && y <= 158 ){ indoors = true; }
				else if ( x >= 6861 && y >= 155 && x <= 6865 && y <= 311 ){ indoors = true; }
				else if ( x >= 6861 && y >= 131 && x <= 7001 && y <= 135 ){ indoors = true; }
				else if ( x >= 7001 && y >= 131 && x <= 7013 && y <= 184 ){ indoors = true; }
				else if ( x >= 6938 && y >= 213 && x <= 6979 && y <= 238 ){ indoors = true; }
				else if ( x >= 6947 && y >= 282 && x <= 6957 && y <= 288 ){ indoors = true; }
				else if ( x >= 6913 && y >= 273 && x <= 6922 && y <= 284 ){ indoors = true; }
				else if ( x >= 6912 && y >= 237 && x <= 6922 && y <= 256 ){ indoors = true; }
				else if ( x >= 6906 && y >= 224 && x <= 6922 && y <= 233 ){ indoors = true; }
				else if ( x >= 6883 && y >= 224 && x <= 6897 && y <= 233 ){ indoors = true; }
				else if ( x >= 6905 && y >= 190 && x <= 6922 && y <= 202 ){ indoors = true; }
				else if ( x >= 6897 && y >= 150 && x <= 6905 && y <= 162 ){ indoors = true; }
				else if ( x >= 6912 && y >= 159 && x <= 6922 && y <= 178 ){ indoors = true; }
				else if ( x >= 6940 && y >= 146 && x <= 6957 && y <= 155 ){ indoors = true; }
				else if ( x >= 6958 && y >= 147 && x <= 6960 && y <= 150 ){ indoors = true; }
				else if ( x >= 6954 && y >= 177 && x <= 6964 && y <= 197 ){ indoors = true; }
				else if ( x >= 6940 && y >= 187 && x <= 6964 && y <= 197 ){ indoors = true; }
				else if ( x >= 6985 && y >= 181 && x <= 6996 && y <= 197 ){ indoors = true; }
				// HOUSES
				else if ( x >= 5221 && y >= 1188 && x <= 5226 && y <= 1194 ){ indoors = true; }
				else if ( x >= 5230 && y >= 1188 && x <= 5235 && y <= 1194 ){ indoors = true; }
				else if ( x >= 5209 && y >= 1211 && x <= 5225 && y <= 1223 ){ indoors = true; }
				else if ( x >= 5226 && y >= 1213 && x <= 5230 && y <= 1220 ){ indoors = true; }
				else if ( x >= 5231 && y >= 1214 && x <= 5232 && y <= 1219 ){ indoors = true; }
				else if ( x >= 5248 && y >= 1228 && x <= 5254 && y <= 1238 ){ indoors = true; }
				else if ( x >= 5242 && y >= 1231 && x <= 5248 && y <= 1241 ){ indoors = true; }
				else if ( x >= 5246 && y >= 1231 && x <= 5249 && y <= 1238 ){ indoors = true; }
				else if ( x >= 2153 && y >= 2769 && x <= 2169 && y <= 2788 ){ indoors = true; }
				else if ( x >= 2123 && y >= 2799 && x <= 2132 && y <= 2806 ){ indoors = true; }
				else if ( x >= 2142 && y >= 2792 && x <= 2149 && y <= 2801 ){ indoors = true; }
				else if ( x >= 2149 && y >= 2739 && x <= 2158 && y <= 2746 ){ indoors = true; }
				else if ( x >= 2140 && y >= 2749 && x <= 2147 && y <= 2758 ){ indoors = true; }
				else if ( x >= 1149 && y >= 2882 && x <= 1160 && y <= 2889 ){ indoors = true; }
				else if ( x >= 1154 && y >= 2876 && x <= 1156 && y <= 2883 ){ indoors = true; }
				else if ( x >= 1153 && y >= 2877 && x <= 1157 && y <= 2880 ){ indoors = true; }
				else if ( x >= 2885 && y >= 1097 && x <= 2890 && y <= 1105 ){ indoors = true; }
				else if ( x >= 2891 && y >= 1096 && x <= 2898 && y <= 1114 ){ indoors = true; }
				else if ( x >= 2863 && y >= 1103 && x <= 2868 && y <= 1112 ){ indoors = true; }
				else if ( x >= 2762 && y >= 1228 && x <= 2769 && y <= 1242 ){ indoors = true; }
				else if ( x >= 1860 && y >= 2393 && x <= 1878 && y <= 2403 ){ indoors = true; }
				else if ( x >= 2087 && y >= 2419 && x <= 2093 && y <= 2427 ){ indoors = true; }
				else if ( x >= 2095 && y >= 2420 && x <= 2106 && y <= 2427 ){ indoors = true; }
				else if ( x >= 2101 && y >= 2427 && x <= 2106 && y <= 2434 ){ indoors = true; }
				else if ( x >= 2063 && y >= 2037 && x <= 2069 && y <= 2046 ){ indoors = true; }
				else if ( x >= 2106 && y >= 2047 && x <= 2112 && y <= 2061 ){ indoors = true; }
				else if ( x >= 2099 && y >= 2054 && x <= 2112 && y <= 2061 ){ indoors = true; }
				// DUSK
				else if ( x >= 2659 && y >= 3169 && x <= 2672 && y <= 3176 ){ indoors = true; }
				else if ( x >= 2679 && y >= 3169 && x <= 2687 && y <= 3185 ){ indoors = true; }
				else if ( x >= 2680 && y >= 3179 && x <= 2696 && y <= 3187 ){ indoors = true; }
				else if ( x >= 2687 && y >= 3170 && x <= 2696 && y <= 3179 ){ indoors = true; }
				else if ( x >= 2700 && y >= 3181 && x <= 2708 && y <= 3196 ){ indoors = true; }
				else if ( x >= 2665 && y >= 3184 && x <= 2672 && y <= 3196 ){ indoors = true; }
				else if ( x >= 2659 && y >= 3202 && x <= 2672 && y <= 3210 ){ indoors = true; }
				else if ( x >= 2654 && y >= 3230 && x <= 2671 && y <= 3238 ){ indoors = true; }
				else if ( x >= 2640 && y >= 3193 && x <= 2646 && y <= 3205 ){ indoors = true; }
				else if ( x >= 2640 && y >= 3220 && x <= 2646 && y <= 3235 ){ indoors = true; }
				else if ( x >= 2667 && y >= 3247 && x <= 2670 && y <= 3250 ){ indoors = true; }
				else if ( x >= 2680 && y >= 3247 && x <= 2683 && y <= 3250 ){ indoors = true; }
				else if ( x >= 2735 && y >= 3192 && x <= 2738 && y <= 3195 ){ indoors = true; }
				else if ( x >= 2735 && y >= 3205 && x <= 2738 && y <= 3208 ){ indoors = true; }
				else if ( x >= 2630 && y >= 3171 && x <= 2633 && y <= 3174 ){ indoors = true; }
				else if ( x >= 2642 && y >= 3161 && x <= 2645 && y <= 3164 ){ indoors = true; }
				// ELIDOR
				else if ( x >= 2952 && y >= 1277 && x <= 2956 && y <= 1281 ){ indoors = true; }
				else if ( x >= 2952 && y >= 1244 && x <= 2956 && y <= 1248 ){ indoors = true; }
				else if ( x >= 2940 && y >= 1244 && x <= 2944 && y <= 1248 ){ indoors = true; }
				else if ( x >= 2970 && y >= 1329 && x <= 2974 && y <= 1333 ){ indoors = true; }
				else if ( x >= 2970 && y >= 1363 && x <= 2974 && y <= 1367 ){ indoors = true; }
				else if ( x >= 2970 && y >= 1375 && x <= 2974 && y <= 1379 ){ indoors = true; }
				else if ( x >= 2884 && y >= 1385 && x <= 2888 && y <= 1389 ){ indoors = true; }
				else if ( x >= 2884 && y >= 1373 && x <= 2888 && y <= 1377 ){ indoors = true; }
				else if ( x >= 2930 && y >= 1250 && x <= 2944 && y <= 1260 ){ indoors = true; }
				else if ( x >= 2905 && y >= 1256 && x <= 2912 && y <= 1267 ){ indoors = true; }
				else if ( x >= 2906 && y >= 1263 && x <= 2918 && y <= 1269 ){ indoors = true; }
				else if ( x >= 2920 && y >= 1257 && x <= 2928 && y <= 1269 ){ indoors = true; }
				else if ( x >= 2890 && y >= 1258 && x <= 2902 && y <= 1269 ){ indoors = true; }
				else if ( x >= 2880 && y >= 1257 && x <= 2888 && y <= 1278 ){ indoors = true; }
				else if ( x >= 2876 && y >= 1260 && x <= 2882 && y <= 1266 ){ indoors = true; }
				else if ( x >= 2877 && y >= 1279 && x <= 2888 && y <= 1296 ){ indoors = true; }
				else if ( x >= 2897 && y >= 1278 && x <= 2910 && y <= 1291 ){ indoors = true; }
				else if ( x >= 2935 && y >= 1278 && x <= 2945 && y <= 1292 ){ indoors = true; }
				else if ( x >= 2963 && y >= 1306 && x <= 2969 && y <= 1314 ){ indoors = true; }
				else if ( x >= 2902 && y >= 1298 && x <= 2914 && y <= 1305 ){ indoors = true; }
				else if ( x >= 2914 && y >= 1301 && x <= 2920 && y <= 1304 ){ indoors = true; }
				else if ( x >= 2917 && y >= 1297 && x <= 2941 && y <= 1300 ){ indoors = true; }
				else if ( x >= 2917 && y >= 1316 && x <= 2920 && y <= 1319 ){ indoors = true; }
				else if ( x >= 2938 && y >= 1316 && x <= 2941 && y <= 1319 ){ indoors = true; }
				else if ( x >= 2920 && y >= 1298 && x <= 2938 && y <= 1318 ){ indoors = true; }
				else if ( x >= 2897 && y >= 1313 && x <= 2912 && y <= 1323 ){ indoors = true; }
				else if ( x >= 2873 && y >= 1300 && x <= 2888 && y <= 1308 ){ indoors = true; }
				else if ( x >= 2873 && y >= 1300 && x <= 2883 && y <= 1318 ){ indoors = true; }
				else if ( x >= 2874 && y >= 1318 && x <= 2888 && y <= 1326 ){ indoors = true; }
				else if ( x >= 2881 && y >= 1331 && x <= 2888 && y <= 1346 ){ indoors = true; }
				else if ( x >= 2918 && y >= 1331 && x <= 2925 && y <= 1337 ){ indoors = true; }
				else if ( x >= 2934 && y >= 1332 && x <= 2949 && y <= 1341 ){ indoors = true; }
				else if ( x >= 2954 && y >= 1332 && x <= 2963 && y <= 1352 ){ indoors = true; }
				else if ( x >= 2949 && y >= 1355 && x <= 2963 && y <= 1368 ){ indoors = true; }
				else if ( x >= 2935 && y >= 1349 && x <= 2942 && y <= 1360 ){ indoors = true; }
				else if ( x >= 2914 && y >= 1361 && x <= 2925 && y <= 1368 ){ indoors = true; }
				else if ( x >= 2898 && y >= 1358 && x <= 2910 && y <= 1368 ){ indoors = true; }
				else if ( x >= 2881 && y >= 1353 && x <= 2888 && y <= 1368 ){ indoors = true; }
				else if ( x >= 2865 && y >= 1370 && x <= 2877 && y <= 1377 ){ indoors = true; }
				else if ( x >= 2896 && y >= 1383 && x <= 2904 && y <= 1397 ){ indoors = true; }
				else if ( x >= 2904 && y >= 1389 && x <= 2912 && y <= 1397 ){ indoors = true; }
				else if ( x >= 2915 && y >= 1390 && x <= 2925 && y <= 1397 ){ indoors = true; }
				else if ( x >= 2942 && y >= 1384 && x <= 2963 && y <= 1390 ){ indoors = true; }
				else if ( x >= 2950 && y >= 1390 && x <= 2963 && y <= 1397 ){ indoors = true; }
				// GLACIAL HILLS
				else if ( x >= 3657 && y >= 466 && x <= 3661 && y <= 470 ){ indoors = true; }
				else if ( x >= 3671 && y >= 466 && x <= 3675 && y <= 470 ){ indoors = true; }
				else if ( x >= 3733 && y >= 381 && x <= 3737 && y <= 385 ){ indoors = true; }
				else if ( x >= 3733 && y >= 395 && x <= 3737 && y <= 399 ){ indoors = true; }
				else if ( x >= 3619 && y >= 395 && x <= 3623 && y <= 399 ){ indoors = true; }
				else if ( x >= 3619 && y >= 381 && x <= 3623 && y <= 385 ){ indoors = true; }
				else if ( x >= 3662 && y >= 350 && x <= 3666 && y <= 354 ){ indoors = true; }
				else if ( x >= 3648 && y >= 350 && x <= 3652 && y <= 354 ){ indoors = true; }
				else if ( x >= 3697 && y >= 272 && x <= 3701 && y <= 276 ){ indoors = true; }
				else if ( x >= 3711 && y >= 271 && x <= 3725 && y <= 278 ){ indoors = true; }
				else if ( x >= 3629 && y >= 377 && x <= 3643 && y <= 386 ){ indoors = true; }
				else if ( x >= 3648 && y >= 372 && x <= 3659 && y <= 386 ){ indoors = true; }
				else if ( x >= 3688 && y >= 376 && x <= 3701 && y <= 386 ){ indoors = true; }
				else if ( x >= 3712 && y >= 394 && x <= 3720 && y <= 406 ){ indoors = true; }
				else if ( x >= 3695 && y >= 394 && x <= 3708 && y <= 402 ){ indoors = true; }
				else if ( x >= 3678 && y >= 394 && x <= 3688 && y <= 401 ){ indoors = true; }
				else if ( x >= 3627 && y >= 395 && x <= 3633 && y <= 406 ){ indoors = true; }
				else if ( x >= 3621 && y >= 410 && x <= 3632 && y <= 426 ){ indoors = true; }
				else if ( x >= 3624 && y >= 431 && x <= 3632 && y <= 444 ){ indoors = true; }
				else if ( x >= 3624 && y >= 439 && x <= 3637 && y <= 446 ){ indoors = true; }
				else if ( x >= 3639 && y >= 423 && x <= 3650 && y <= 431 ){ indoors = true; }
				else if ( x >= 3653 && y >= 418 && x <= 3662 && y <= 431 ){ indoors = true; }
				else if ( x >= 3647 && y >= 438 && x <= 3662 && y <= 457 ){ indoors = true; }
				else if ( x >= 3670 && y >= 404 && x <= 3678 && y <= 412 ){ indoors = true; }
				else if ( x >= 3670 && y >= 422 && x <= 3678 && y <= 430 ){ indoors = true; }
				else if ( x >= 3669 && y >= 440 && x <= 3684 && y <= 451 ){ indoors = true; }
				else if ( x >= 3695 && y >= 421 && x <= 3706 && y <= 431 ){ indoors = true; }
				else if ( x >= 3689 && y >= 440 && x <= 3704 && y <= 451 ){ indoors = true; }
				else if ( x >= 3709 && y >= 421 && x <= 3720 && y <= 431 ){ indoors = true; }
				else if ( x >= 3703 && y >= 411 && x <= 3720 && y <= 418 ){ indoors = true; }
				// GREENSKY
				else if ( x >= 4236 && y >= 2964 && x <= 4246 && y <= 2970 ){ indoors = true; }
				else if ( x >= 4226 && y >= 2985 && x <= 4239 && y <= 2992 ){ indoors = true; }
				else if ( x >= 4233 && y >= 2985 && x <= 4239 && y <= 2998 ){ indoors = true; }
				else if ( x >= 4213 && y >= 3010 && x <= 4219 && y <= 3018 ){ indoors = true; }
				// ISLEGEM
				else if ( x >= 2802 && y >= 2257 && x <= 2808 && y <= 2263 ){ indoors = true; }
				else if ( x >= 2807 && y >= 2210 && x <= 2814 && y <= 2222 ){ indoors = true; }
				else if ( x >= 2817 && y >= 2203 && x <= 2827 && y <= 2208 ){ indoors = true; }
				else if ( x >= 2829 && y >= 2202 && x <= 2835 && y <= 2208 ){ indoors = true; }
				else if ( x >= 2820 && y >= 2223 && x <= 2829 && y <= 2236 ){ indoors = true; }
				else if ( x >= 2814 && y >= 2240 && x <= 2829 && y <= 2247 ){ indoors = true; }
				else if ( x >= 2842 && y >= 2241 && x <= 2852 && y <= 2248 ){ indoors = true; }
				else if ( x >= 2854 && y >= 2242 && x <= 2860 && y <= 2248 ){ indoors = true; }
				// WHISPER
				else if ( x >= 886 && y >= 962 && x <= 893 && y <= 968 ){ indoors = true; }
				else if ( x >= 878 && y >= 973 && x <= 885 && y <= 979 ){ indoors = true; }
				else if ( x >= 901 && y >= 962 && x <= 907 && y <= 969 ){ indoors = true; }
				else if ( x >= 894 && y >= 982 && x <= 904 && y <= 985 ){ indoors = true; }
				else if ( x >= 895 && y >= 980 && x <= 902 && y <= 985 ){ indoors = true; }
				else if ( x >= 897 && y >= 979 && x <= 900 && y <= 980 ){ indoors = true; }
				else if ( x >= 895 && y >= 985 && x <= 902 && y <= 987 ){ indoors = true; }
				else if ( x >= 897 && y >= 987 && x <= 901 && y <= 989 ){ indoors = true; }
				else if ( x >= 902 && y >= 985 && x <= 903 && y <= 988 ){ indoors = true; }
				else if ( x >= 904 && y >= 983 && x <= 906 && y <= 987 ){ indoors = true; }
				else if ( x >= 888 && y >= 935 && x <= 902 && y <= 943 ){ indoors = true; }
				else if ( x >= 902 && y >= 935 && x <= 904 && y <= 940 ){ indoors = true; }
				else if ( x >= 881 && y >= 918 && x <= 888 && y <= 924 ){ indoors = true; }
				else if ( x >= 902 && y >= 890 && x <= 913 && y <= 895 ){ indoors = true; }
				else if ( x >= 904 && y >= 889 && x <= 913 && y <= 895 ){ indoors = true; }
				else if ( x >= 887 && y >= 898 && x <= 900 && y <= 904 ){ indoors = true; }
				else if ( x >= 868 && y >= 912 && x <= 874 && y <= 920 ){ indoors = true; }
				else if ( x >= 869 && y >= 910 && x <= 874 && y <= 916 ){ indoors = true; }
				else if ( x >= 858 && y >= 925 && x <= 864 && y <= 933 ){ indoors = true; }
				else if ( x >= 864 && y >= 932 && x <= 869 && y <= 933 ){ indoors = true; }
				else if ( x >= 853 && y >= 938 && x <= 870 && y <= 957 ){ indoors = true; }
				else if ( x >= 807 && y >= 920 && x <= 845 && y <= 965 ){ indoors = true; }
				else if ( x >= 840 && y >= 945 && x <= 853 && y <= 953 ){ indoors = true; }
				// STARGUIDE
				else if ( x >= 2373 && y >= 3165 && x <= 2376 && y <= 3168 ){ indoors = true; }
				else if ( x >= 2299 && y >= 3165 && x <= 2302 && y <= 3168 ){ indoors = true; }
				else if ( x >= 2299 && y >= 3151 && x <= 2302 && y <= 3154 ){ indoors = true; }
				else if ( x >= 2325 && y >= 3129 && x <= 2330 && y <= 3138 ){ indoors = true; }
				else if ( x >= 2338 && y >= 3129 && x <= 2344 && y <= 3138 ){ indoors = true; }
				else if ( x >= 2340 && y >= 3132 && x <= 2351 && y <= 3138 ){ indoors = true; }
				else if ( x >= 2338 && y >= 3146 && x <= 2344 && y <= 3155 ){ indoors = true; }
				else if ( x >= 2353 && y >= 3164 && x <= 2358 && y <= 3173 ){ indoors = true; }
				else if ( x >= 2359 && y >= 3164 && x <= 2365 && y <= 3173 ){ indoors = true; }
				else if ( x >= 2364 && y >= 3150 && x <= 2372 && y <= 3155 ){ indoors = true; }
				else if ( x >= 2321 && y >= 3164 && x <= 2330 && y <= 3170 ){ indoors = true; }
				else if ( x >= 2321 && y >= 3149 && x <= 2330 && y <= 3155 ){ indoors = true; }
				else if ( x >= 2299 && y >= 3133 && x <= 2303 && y <= 3138 ){ indoors = true; }
				else if ( x >= 2306 && y >= 3127 && x <= 2310 && y <= 3132 ){ indoors = true; }
				// SPRINGVALE
				else if ( x >= 4247 && y >= 1490 && x <= 4250 && y <= 1493 ){ indoors = true; }
				else if ( x >= 4261 && y >= 1490 && x <= 4264 && y <= 1493 ){ indoors = true; }
				else if ( x >= 4291 && y >= 1490 && x <= 4294 && y <= 1493 ){ indoors = true; }
				else if ( x >= 4291 && y >= 1476 && x <= 4294 && y <= 1479 ){ indoors = true; }
				else if ( x >= 4170 && y >= 1487 && x <= 4173 && y <= 1490 ){ indoors = true; }
				else if ( x >= 4186 && y >= 1406 && x <= 4189 && y <= 1409 ){ indoors = true; }
				else if ( x >= 4172 && y >= 1406 && x <= 4175 && y <= 1409 ){ indoors = true; }
				else if ( x >= 4168 && y >= 1425 && x <= 4177 && y <= 1441 ){ indoors = true; }
				else if ( x >= 4167 && y >= 1469 && x <= 4177 && y <= 1481 ){ indoors = true; }
				else if ( x >= 4184 && y >= 1472 && x <= 4197 && y <= 1481 ){ indoors = true; }
				else if ( x >= 4186 && y >= 1445 && x <= 4198 && y <= 1453 ){ indoors = true; }
				else if ( x >= 4184 && y >= 1420 && x <= 4197 && y <= 1429 ){ indoors = true; }
				else if ( x >= 4203 && y >= 1417 && x <= 4212 && y <= 1429 ){ indoors = true; }
				else if ( x >= 4201 && y >= 1441 && x <= 4213 && y <= 1453 ){ indoors = true; }
				else if ( x >= 4206 && y >= 1448 && x <= 4214 && y <= 1454 ){ indoors = true; }
				else if ( x >= 4217 && y >= 1443 && x <= 4237 && y <= 1453 ){ indoors = true; }
				else if ( x >= 4238 && y >= 1416 && x <= 4257 && y <= 1429 ){ indoors = true; }
				else if ( x >= 4242 && y >= 1436 && x <= 4252 && y <= 1451 ){ indoors = true; }
				else if ( x >= 4243 && y >= 1465 && x <= 4252 && y <= 1481 ){ indoors = true; }
				else if ( x >= 4227 && y >= 1471 && x <= 4240 && y <= 1481 ){ indoors = true; }
				else if ( x >= 4210 && y >= 1461 && x <= 4219 && y <= 1481 ){ indoors = true; }
				// RAVENDARK
				else if ( x >= 6759 && y >= 3631 && x <= 6786 && y <= 3647 ){ indoors = true; }
				else if ( x >= 6813 && y >= 3653 && x <= 6819 && y <= 3659 ){ indoors = true; }
				else if ( x >= 6826 && y >= 3653 && x <= 6832 && y <= 3659 ){ indoors = true; }
				else if ( x >= 6813 && y >= 3668 && x <= 6819 && y <= 3674 ){ indoors = true; }
				else if ( x >= 6826 && y >= 3668 && x <= 6832 && y <= 3674 ){ indoors = true; }
				else if ( x >= 6814 && y >= 3659 && x <= 6831 && y <= 3670 ){ indoors = true; }
				else if ( x >= 6831 && y >= 3662 && x <= 6832 && y <= 3665 ){ indoors = true; }
				else if ( x >= 6815 && y >= 3677 && x <= 6818 && y <= 3681 ){ indoors = true; }
				else if ( x >= 6815 && y >= 3683 && x <= 6819 && y <= 3689 ){ indoors = true; }
				else if ( x >= 6816 && y >= 3669 && x <= 6838 && y <= 3695 ){ indoors = true; }
				else if ( x >= 6816 && y >= 3688 && x <= 6824 && y <= 3689 ){ indoors = true; }
				else if ( x >= 6816 && y >= 3690 && x <= 6819 && y <= 3708 ){ indoors = true; }
				else if ( x >= 6817 && y >= 3691 && x <= 6829 && y <= 3706 ){ indoors = true; }
				else if ( x >= 6822 && y >= 3706 && x <= 6825 && y <= 3708 ){ indoors = true; }
				else if ( x >= 6828 && y >= 3705 && x <= 6831 && y <= 3708 ){ indoors = true; }
				else if ( x >= 6824 && y >= 3690 && x <= 6830 && y <= 3705 ){ indoors = true; }
				else if ( x >= 6829 && y >= 3699 && x <= 6831 && y <= 3702 ){ indoors = true; }
				else if ( x >= 6836 && y >= 3693 && x <= 6839 && y <= 3696 ){ indoors = true; }
				else if ( x >= 6828 && y >= 3694 && x <= 6833 && y <= 3696 ){ indoors = true; }
				else if ( x >= 6832 && y >= 3693 && x <= 6836 && y <= 3695 ){ indoors = true; }
				else if ( x >= 6836 && y >= 3682 && x <= 6838 && y <= 3693 ){ indoors = true; }
				else if ( x >= 6837 && y >= 3687 && x <= 6839 && y <= 3690 ){ indoors = true; }
				else if ( x >= 6837 && y >= 3681 && x <= 6839 && y <= 3684 ){ indoors = true; }
				else if ( x >= 6837 && y >= 3675 && x <= 6840 && y <= 3680 ){ indoors = true; }
				else if ( x >= 6825 && y >= 3718 && x <= 6832 && y <= 3722 ){ indoors = true; }
				else if ( x >= 6786 && y >= 3732 && x <= 6793 && y <= 3738 ){ indoors = true; }
				else if ( x >= 6752 && y >= 3753 && x <= 6758 && y <= 3759 ){ indoors = true; }
				else if ( x >= 6750 && y >= 3741 && x <= 6758 && y <= 3747 ){ indoors = true; }
				else if ( x >= 6738 && y >= 3745 && x <= 6745 && y <= 3755 ){ indoors = true; }
				else if ( x >= 6703 && y >= 3750 && x <= 6710 && y <= 3755 ){ indoors = true; }
				else if ( x >= 6705 && y >= 3742 && x <= 6710 && y <= 3751 ){ indoors = true; }
				else if ( x >= 6707 && y >= 3741 && x <= 6714 && y <= 3750 ){ indoors = true; }
				else if ( x >= 6707 && y >= 3742 && x <= 6716 && y <= 3749 ){ indoors = true; }
				else if ( x >= 6736 && y >= 3692 && x <= 6746 && y <= 3707 ){ indoors = true; }
				else if ( x >= 6728 && y >= 3708 && x <= 6735 && y <= 3716 ){ indoors = true; }
				else if ( x >= 6735 && y >= 3708 && x <= 6740 && y <= 3714 ){ indoors = true; }
				else if ( x >= 6763 && y >= 3669 && x <= 6771 && y <= 3676 ){ indoors = true; }
				else if ( x >= 6769 && y >= 3670 && x <= 6773 && y <= 3673 ){ indoors = true; }
				else if ( x >= 6749 && y >= 3691 && x <= 6758 && y <= 3699 ){ indoors = true; }
				else if ( x >= 6758 && y >= 3699 && x <= 6764 && y <= 3707 ){ indoors = true; }
				else if ( x >= 6755 && y >= 3700 && x <= 6761 && y <= 3707 ){ indoors = true; }
				else if ( x >= 6752 && y >= 3707 && x <= 6761 && y <= 3716 ){ indoors = true; }
				else if ( x >= 6755 && y >= 3717 && x <= 6764 && y <= 3725 ){ indoors = true; }
				else if ( x >= 6788 && y >= 3690 && x <= 6807 && y <= 3702 ){ indoors = true; }
				else if ( x >= 6796 && y >= 3701 && x <= 6803 && y <= 3709 ){ indoors = true; }
				else if ( x >= 6775 && y >= 3709 && x <= 6787 && y <= 3723 ){ indoors = true; }
				else if ( x >= 6788 && y >= 3713 && x <= 6789 && y <= 3719 ){ indoors = true; }
				else if ( x >= 6819 && y >= 3655 && x <= 6830 && y <= 3663 ){ indoors = true; }
				else if ( x >= 6819 && y >= 3655 && x <= 6830 && y <= 3663 ){ indoors = true; }
				else if ( x >= 6735 && y >= 3776 && x <= 6743 && y <= 3786 ){ indoors = true; }
				else if ( x >= 6736 && y >= 3786 && x <= 6742 && y <= 3791 ){ indoors = true; }
				// PORTSHINE
				else if ( x >= 817 && y >= 1990 && x <= 825 && y <= 1997 ){ indoors = true; }
				else if ( x >= 818 && y >= 1995 && x <= 823 && y <= 2002 ){ indoors = true; }
				else if ( x >= 824 && y >= 1989 && x <= 830 && y <= 1995 ){ indoors = true; }
				else if ( x >= 837 && y >= 1984 && x <= 842 && y <= 1995 ){ indoors = true; }
				else if ( x >= 841 && y >= 1989 && x <= 846 && y <= 1995 ){ indoors = true; }
				else if ( x >= 849 && y >= 2017 && x <= 859 && y <= 2023 ){ indoors = true; }
				else if ( x >= 832 && y >= 2030 && x <= 842 && y <= 2036 ){ indoors = true; }
				// LODORIA CASTLE
				else if ( x >= 1763 && y >= 2199 && x <= 1794 && y <= 2224 ){ indoors = true; }
				else if ( x >= 1791 && y >= 2199 && x <= 1798 && y <= 2206 ){ indoors = true; }
				else if ( x >= 1791 && y >= 2224 && x <= 1798 && y <= 2231 ){ indoors = true; }
				// LODORIA VILLAGE
				else if ( x >= 2019 && y >= 2159 && x <= 2026 && y <= 2165 ){ indoors = true; }
				else if ( x >= 2040 && y >= 2168 && x <= 2047 && y <= 2183 ){ indoors = true; }
				else if ( x >= 2049 && y >= 2155 && x <= 2056 && y <= 2166 ){ indoors = true; }
				else if ( x >= 2056 && y >= 2159 && x <= 2064 && y <= 2166 ){ indoors = true; }
				else if ( x >= 2072 && y >= 2159 && x <= 2084 && y <= 2166 ){ indoors = true; }
				else if ( x >= 2086 && y >= 2144 && x <= 2093 && y <= 2156 ){ indoors = true; }
				else if ( x >= 2100 && y >= 2150 && x <= 2112 && y <= 2156 ){ indoors = true; }
				else if ( x >= 2100 && y >= 2164 && x <= 2107 && y <= 2183 ){ indoors = true; }
				else if ( x >= 2071 && y >= 2176 && x <= 2084 && y <= 2183 ){ indoors = true; }
				else if ( x >= 2072 && y >= 2191 && x <= 2084 && y <= 2197 ){ indoors = true; }
				else if ( x >= 2077 && y >= 2202 && x <= 2084 && y <= 2212 ){ indoors = true; }
				else if ( x >= 2058 && y >= 2206 && x <= 2071 && y <= 2212 ){ indoors = true; }
				else if ( x >= 2040 && y >= 2169 && x <= 2047 && y <= 2183 ){ indoors = true; }
				else if ( x >= 2054 && y >= 2177 && x <= 2067 && y <= 2183 ){ indoors = true; }
				else if ( x >= 2038 && y >= 2247 && x <= 2045 && y <= 2253 ){ indoors = true; }
				else if ( x >= 1975 && y >= 2223 && x <= 2001 && y <= 2247 ){ indoors = true; }
				else if ( x >= 1983 && y >= 2201 && x <= 2020 && y <= 2247 ){ indoors = true; }
				else if ( x >= 2014 && y >= 2244 && x <= 2020 && y <= 2249 ){ indoors = true; }
				else if ( x >= 2015 && y >= 2216 && x <= 2032 && y <= 2240 ){ indoors = true; }
				else if ( x >= 2025 && y >= 2222 && x <= 2038 && y <= 2230 ){ indoors = true; }
				else if ( x >= 2008 && y >= 2201 && x <= 2032 && y <= 2225 ){ indoors = true; }
				// LODORIA CITY
				else if ( x >= 1937 && y >= 2130 && x <= 1945 && y <= 2145 ){ indoors = true; }
				else if ( x >= 1903 && y >= 2136 && x <= 1907 && y <= 2140 ){ indoors = true; }
				else if ( x >= 1901 && y >= 2144 && x <= 1908 && y <= 2158 ){ indoors = true; }
				else if ( x >= 1894 && y >= 2151 && x <= 1908 && y <= 2158 ){ indoors = true; }
				else if ( x >= 1916 && y >= 2158 && x <= 1920 && y <= 2162 ){ indoors = true; }
				else if ( x >= 1888 && y >= 2136 && x <= 1898 && y <= 2150 ){ indoors = true; }
				else if ( x >= 1877 && y >= 2146 && x <= 1890 && y <= 2158 ){ indoors = true; }
				else if ( x >= 1851 && y >= 2161 && x <= 1859 && y <= 2178 ){ indoors = true; }
				else if ( x >= 1850 && y >= 2185 && x <= 1854 && y <= 2189 ){ indoors = true; }
				else if ( x >= 1845 && y >= 2196 && x <= 1858 && y <= 2203 ){ indoors = true; }
				else if ( x >= 1870 && y >= 2182 && x <= 1879 && y <= 2193 ){ indoors = true; }
				else if ( x >= 1863 && y >= 2197 && x <= 1878 && y <= 2203 ){ indoors = true; }
				else if ( x >= 1884 && y >= 2177 && x <= 1894 && y <= 2183 ){ indoors = true; }
				else if ( x >= 1899 && y >= 2176 && x <= 1908 && y <= 2183 ){ indoors = true; }
				else if ( x >= 1885 && y >= 2196 && x <= 1897 && y <= 2204 ){ indoors = true; }
				else if ( x >= 1901 && y >= 2192 && x <= 1908 && y <= 2203 ){ indoors = true; }
				else if ( x >= 1916 && y >= 2177 && x <= 1930 && y <= 2184 ){ indoors = true; }
				else if ( x >= 1920 && y >= 2191 && x <= 1935 && y <= 2197 ){ indoors = true; }
				else if ( x >= 1929 && y >= 2198 && x <= 1935 && y <= 2205 ){ indoors = true; }
				else if ( x >= 1927 && y >= 2223 && x <= 1936 && y <= 2235 ){ indoors = true; }
				else if ( x >= 1943 && y >= 2213 && x <= 1949 && y <= 2234 ){ indoors = true; }
				else if ( x >= 1914 && y >= 2241 && x <= 1920 && y <= 2255 ){ indoors = true; }
				else if ( x >= 1923 && y >= 2247 && x <= 1936 && y <= 2256 ){ indoors = true; }
				else if ( x >= 1885 && y >= 2273 && x <= 1896 && y <= 2279 ){ indoors = true; }
				else if ( x >= 1899 && y >= 2266 && x <= 1906 && y <= 2279 ){ indoors = true; }
				else if ( x >= 1927 && y >= 2223 && x <= 1936 && y <= 2235 ){ indoors = true; }
				else if ( x >= 1902 && y >= 2227 && x <= 1917 && y <= 2235 ){ indoors = true; }
				else if ( x >= 1898 && y >= 2241 && x <= 1906 && y <= 2257 ){ indoors = true; }
				else if ( x >= 1890 && y >= 2249 && x <= 1906 && y <= 2257 ){ indoors = true; }
				else if ( x >= 1891 && y >= 2219 && x <= 1899 && y <= 2235 ){ indoors = true; }
				else if ( x >= 1885 && y >= 2227 && x <= 1899 && y <= 2235 ){ indoors = true; }
				else if ( x >= 1863 && y >= 2242 && x <= 1878 && y <= 2258 ){ indoors = true; }
				else if ( x >= 1871 && y >= 2267 && x <= 1878 && y <= 2281 ){ indoors = true; }
				else if ( x >= 1832 && y >= 2237 && x <= 1838 && y <= 2254 ){ indoors = true; }
				else if ( x >= 1846 && y >= 2227 && x <= 1858 && y <= 2234 ){ indoors = true; }
				else if ( x >= 1843 && y >= 2289 && x <= 1948 && y <= 2291 && z >= 18 ){ indoors = true; }
				else if ( x >= 1855 && y >= 2120 && x <= 1958 && y <= 2123 && z >= 18 ){ indoors = true; }
				else if ( x >= 1956 && y >= 2119 && x <= 1959 && y <= 2256 && z >= 18 ){ indoors = true; }
				else if ( x >= 1945 && y >= 2253 && x <= 1959 && y <= 2256 && z >= 18 ){ indoors = true; }
				else if ( x >= 1945 && y >= 2253 && x <= 1947 && y <= 2292 && z >= 18 ){ indoors = true; }
			}
			else if ( map == Map.SerpentIsland )
			{
				if ( x >= 2259 && y >= 1655 && x <= 2267 && y <= 1666 ){ indoors = true; }
				else if ( x >= 2255 && y >= 1661 && x <= 2267 && y <= 1666 ){ indoors = true; }
				else if ( x >= 2286 && y >= 1654 && x <= 2297 && y <= 1666 ){ indoors = true; }
				else if ( x >= 2268 && y >= 1673 && x <= 2278 && y <= 1680 ){ indoors = true; }
				else if ( x >= 2268 && y >= 1673 && x <= 2272 && y <= 1690 ){ indoors = true; }
				else if ( x >= 2294 && y >= 1675 && x <= 2305 && y <= 1685 ){ indoors = true; }
				else if ( x >= 2270 && y >= 1697 && x <= 2279 && y <= 1706 ){ indoors = true; }
				else if ( x >= 2280 && y >= 1713 && x <= 2294 && y <= 1722 ){ indoors = true; }
				else if ( x >= 2274 && y >= 1732 && x <= 2288 && y <= 1741 ){ indoors = true; }
				else if ( x >= 2304 && y >= 1704 && x <= 2316 && y <= 1717 ){ indoors = true; }
				else if ( x >= 2270 && y >= 1697 && x <= 2279 && y <= 1706 ){ indoors = true; }
				else if ( x >= 2248 && y >= 1697 && x <= 2263 && y <= 1706 ){ indoors = true; }
				else if ( x >= 2228 && y >= 1697 && x <= 2241 && y <= 1707 ){ indoors = true; }
				else if ( x >= 2235 && y >= 1713 && x <= 2248 && y <= 1725 ){ indoors = true; }
				else if ( x >= 2244 && y >= 1734 && x <= 2258 && y <= 1742 ){ indoors = true; }
				else if ( x >= 2228 && y >= 1735 && x <= 2236 && y <= 1744 ){ indoors = true; }
				else if ( x >= 2190 && y >= 1662 && x <= 2212 && y <= 1676 ){ indoors = true; }
			}
			else if ( map == Map.SavagedEmpire )
			{
				if ( x >= 767 && y >= 311 && x <= 799 && y <= 327 ){ indoors = true; }
				else if ( x >= 776 && y >= 327 && x <= 791 && y <= 335 ){ indoors = true; }
				else if ( x >= 212 && y >= 1657 && x <= 219 && y <= 1667 ){ indoors = true; }
				else if ( x >= 238 && y >= 1705 && x <= 247 && y <= 1711 ){ indoors = true; }
				else if ( x >= 228 && y >= 1710 && x <= 239 && y <= 1721 ){ indoors = true; }
				else if ( x >= 245 && y >= 1717 && x <= 253 && y <= 1731 ){ indoors = true; }
				else if ( x >= 306 && y >= 1699 && x <= 319 && y <= 1704 ){ indoors = true; }
				else if ( x >= 306 && y >= 1699 && x <= 311 && y <= 1707 ){ indoors = true; }
				else if ( x >= 287 && y >= 1643 && x <= 297 && y <= 1654 ){ indoors = true; }
				else if ( x >= 273 && y >= 1634 && x <= 286 && y <= 1652 ){ indoors = true; }
				else if ( x >= 285 && y >= 1638 && x <= 294 && y <= 1643 ){ indoors = true; }
				else if ( x >= 670 && y >= 847 && x <= 685 && y <= 858 ){ indoors = true; }
				else if ( x >= 773 && y >= 854 && x <= 803 && y <= 877 ){ indoors = true; }
				else if ( x >= 758 && y >= 893 && x <= 777 && y <= 904 ){ indoors = true; }
				else if ( x >= 758 && y >= 893 && x <= 765 && y <= 912 ){ indoors = true; }
				else if ( x >= 740 && y >= 888 && x <= 752 && y <= 891 ){ indoors = true; }
				else if ( x >= 739 && y >= 891 && x <= 755 && y <= 903 ){ indoors = true; }
				else if ( x >= 740 && y >= 900 && x <= 751 && y <= 906 ){ indoors = true; }
				else if ( x >= 724 && y >= 900 && x <= 734 && y <= 911 ){ indoors = true; }
				else if ( x >= 723 && y >= 908 && x <= 734 && y <= 911 ){ indoors = true; }
				else if ( x >= 747 && y >= 919 && x <= 755 && y <= 931 ){ indoors = true; }
				else if ( x >= 703 && y >= 908 && x <= 718 && y <= 919 ){ indoors = true; }
				else if ( x >= 747 && y >= 919 && x <= 755 && y <= 931 ){ indoors = true; }
				else if ( x >= 739 && y >= 944 && x <= 747 && y <= 952 ){ indoors = true; }
				else if ( x >= 766 && y >= 972 && x <= 773 && y <= 980 ){ indoors = true; }
				else if ( x >= 749 && y >= 962 && x <= 758 && y <= 968 ){ indoors = true; }
				else if ( x >= 749 && y >= 966 && x <= 755 && y <= 975 ){ indoors = true; }
				else if ( x >= 808 && y >= 991 && x <= 817 && y <= 1001 ){ indoors = true; }
				else if ( x >= 809 && y >= 998 && x <= 832 && y <= 1010 ){ indoors = true; }
				else if ( x >= 824 && y >= 990 && x <= 832 && y <= 1002 ){ indoors = true; }
				else if ( x >= 812 && y >= 969 && x <= 831 && y <= 980 ){ indoors = true; }
				else if ( x >= 815 && y >= 981 && x <= 828 && y <= 984 ){ indoors = true; }
				else if ( x >= 712 && y >= 979 && x <= 718 && y <= 995 ){ indoors = true; }
				else if ( x >= 710 && y >= 983 && x <= 718 && y <= 991 ){ indoors = true; }
				else if ( x >= 706 && y >= 988 && x <= 710 && y <= 991 ){ indoors = true; }
				else if ( x >= 707 && y >= 992 && x <= 714 && y <= 995 ){ indoors = true; }
				else if ( x >= 681 && y >= 986 && x <= 693 && y <= 994 ){ indoors = true; }
				else if ( x >= 215 && y >= 1649 && x <= 236 && y <= 1653 && z > 55 ){ indoors = true; }
				else if ( x >= 324 && y >= 1649 && x <= 328 && y <= 1740 && z > 55 ){ indoors = true; }
				else if ( x >= 288 && y >= 1759 && x <= 313 && y <= 1764 && z > 55 ){ indoors = true; }
				else if ( x >= 1045 && y >= 421 && x <= 1063 && y <= 440 ){ indoors = true; }
			}

            return indoors;
        }
	}

	[Flags]
	public enum MapRules
	{
		None					= 0x0000,
		Internal				= 0x0001, // Internal map (used for dragging, commodity deeds, etc)
		FreeMovement			= 0x0002, // Anyone can move over anyone else without taking stamina loss
		BeneficialRestrictions	= 0x0004, // Disallow performing beneficial actions on criminals/murderers
		HarmfulRestrictions		= 0x0008, // Disallow performing harmful actions on innocents
		SosariaRules			= FreeMovement | BeneficialRestrictions | HarmfulRestrictions,
		LodorRules			= None
	}

	public interface IPooledEnumerable : IEnumerable
	{
		void Free();
	}

	public interface IPooledEnumerator : IEnumerator
	{
		IPooledEnumerable Enumerable{ get; set; }
		void Free();
	}

	[Parsable]
	//[CustomEnum( new string[]{ "Lodor", "Sosaria", "Underworld", "SerpentIsland", "IslesDread", "SavagedEmpire", "Internal" } )]
	public sealed class Map : IComparable, IComparable<Map>
	{
		public const int SectorSize = 16;
		public const int SectorShift = 4;
		public static int SectorActiveRange = 2;

		private static Map[] m_Maps = new Map[0x100];

		public static Map[] Maps { get { return m_Maps; } }

		public static Map Lodor { get { return m_Maps[0]; } }
		public static Map Sosaria { get { return m_Maps[1]; } }
		public static Map Underworld { get { return m_Maps[2]; } }
		public static Map SerpentIsland { get { return m_Maps[3]; } }
		public static Map IslesDread { get { return m_Maps[4]; } }
		public static Map SavagedEmpire { get { return m_Maps[5]; } }
		public static Map Atlantis { get { return m_Maps[6]; } }
		public static Map Internal { get { return m_Maps[0x7F]; } }

		private static List<Map> m_AllMaps = new List<Map>();

		public static List<Map> AllMaps { get { return m_AllMaps; } }

		private int m_MapID, m_MapIndex, m_FileIndex;

		private int m_Width, m_Height;
		private int m_SectorsWidth, m_SectorsHeight;
		private int m_Season;
		private Dictionary<string, Region> m_Regions;
		private Region m_DefaultRegion;

		public int Season { get { return m_Season; } set { m_Season = value; } }

		private string m_Name;
		private MapRules m_Rules;
		private Sector[][] m_Sectors;
		private Sector m_InvalidSector;

		private TileMatrix m_Tiles;

		private static string[] m_MapNames;
		private static Map[] m_MapValues;

		public static string[] GetMapNames()
		{
			CheckNamesAndValues();
			return m_MapNames;
		}

		public static Map[] GetMapValues()
		{
			CheckNamesAndValues();
			return m_MapValues;
		}

		public static Map Parse( string value )
		{
			CheckNamesAndValues();

			for ( int i = 0; i < m_MapNames.Length; ++i )
			{
				if ( Insensitive.Equals( m_MapNames[i], value ) )
					return m_MapValues[i];
			}

			int index;

			if( int.TryParse( value, out index ) )
			{
				if( index >= 0 && index < m_Maps.Length && m_Maps[index] != null )
					return m_Maps[index];
			}

			throw new ArgumentException( "Invalid map name" );
		}

		private static void CheckNamesAndValues()
		{
			if ( m_MapNames != null && m_MapNames.Length == m_AllMaps.Count )
				return;

			m_MapNames = new string[m_AllMaps.Count];
			m_MapValues = new Map[m_AllMaps.Count];

			for ( int i = 0; i < m_AllMaps.Count; ++i )
			{
				Map map = m_AllMaps[i];

				m_MapNames[i] = map.Name;
				m_MapValues[i] = map;
			}
		}

		public override string ToString()
		{
			return m_Name;
		}

		public int GetAverageZ( int x, int y )
		{
			int z = 0, avg = 0, top = 0;

			GetAverageZ( x, y, ref z, ref avg, ref top );

			return avg;
		}

		public void GetAverageZ( int x, int y, ref int z, ref int avg, ref int top )
		{
			int zTop = Tiles.GetLandTile( x, y ).Z;
			int zLeft = Tiles.GetLandTile( x, y + 1 ).Z;
			int zRight = Tiles.GetLandTile( x + 1, y ).Z;
			int zBottom = Tiles.GetLandTile( x + 1, y + 1 ).Z;

			z = zTop;
			if ( zLeft < z )
				z = zLeft;
			if ( zRight < z )
				z = zRight;
			if ( zBottom < z )
				z = zBottom;

			top = zTop;
			if ( zLeft > top )
				top = zLeft;
			if ( zRight > top )
				top = zRight;
			if ( zBottom > top )
				top = zBottom;

			if ( Math.Abs( zTop - zBottom ) > Math.Abs( zLeft - zRight ) )
				avg = FloorAverage( zLeft, zRight );
			else
				avg = FloorAverage( zTop, zBottom );
		}

		private static int FloorAverage( int a, int b )
		{
			int v = a + b;

			if ( v < 0 )
				--v;

			return ( v / 2 );
		}

		#region Get*InRange/Bounds
		public IPooledEnumerable GetObjectsInRange( Point3D p )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( ObjectEnumerator.Instantiate( this, new Rectangle2D( p.m_X - 18, p.m_Y - 18, 37, 37 ) ) );
		}

		public IPooledEnumerable GetObjectsInRange( Point3D p, int range )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( ObjectEnumerator.Instantiate( this, new Rectangle2D( p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1 ) ) );
		}

		public IPooledEnumerable GetObjectsInBounds( Rectangle2D bounds )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( ObjectEnumerator.Instantiate( this, bounds ) );
		}

		public IPooledEnumerable GetClientsInRange( Point3D p )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( TypedEnumerator.Instantiate( this, new Rectangle2D( p.m_X - 18, p.m_Y - 18, 37, 37 ), SectorEnumeratorType.Clients ) );
		}

		public IPooledEnumerable GetClientsInRange( Point3D p, int range )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( TypedEnumerator.Instantiate( this, new Rectangle2D( p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1 ), SectorEnumeratorType.Clients ) );
		}

		public IPooledEnumerable GetClientsInBounds( Rectangle2D bounds )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( TypedEnumerator.Instantiate( this, bounds, SectorEnumeratorType.Clients ) );
		}

		public IPooledEnumerable GetItemsInRange( Point3D p )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( TypedEnumerator.Instantiate( this, new Rectangle2D( p.m_X - 18, p.m_Y - 18, 37, 37 ), SectorEnumeratorType.Items ) );
		}

		public IPooledEnumerable GetItemsInRange( Point3D p, int range )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( TypedEnumerator.Instantiate( this, new Rectangle2D( p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1 ), SectorEnumeratorType.Items ) );
		}

		public IPooledEnumerable GetItemsInBounds( Rectangle2D bounds )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( TypedEnumerator.Instantiate( this, bounds, SectorEnumeratorType.Items ) );
		}

		public IPooledEnumerable GetMobilesInRange( Point3D p )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( TypedEnumerator.Instantiate( this, new Rectangle2D( p.m_X - 18, p.m_Y - 18, 37, 37 ), SectorEnumeratorType.Mobiles ) );
		}

		public IPooledEnumerable GetMobilesInRange( Point3D p, int range )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( TypedEnumerator.Instantiate( this, new Rectangle2D( p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1 ), SectorEnumeratorType.Mobiles ) );
		}

		public IPooledEnumerable GetMobilesInBounds( Rectangle2D bounds )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( TypedEnumerator.Instantiate( this, bounds, SectorEnumeratorType.Mobiles ) );
		}
		#endregion

		public IPooledEnumerable GetMultiTilesAt( int x, int y )
		{
			if ( this == Map.Internal )
				return NullEnumerable.Instance;

			Sector sector = GetSector( x, y );

			if ( sector.Multis.Count == 0 )
				return NullEnumerable.Instance;

			return PooledEnumerable.Instantiate( MultiTileEnumerator.Instantiate( sector, new Point2D( x, y ) ) );
		}

		#region CanFit
		public bool CanFit( Point3D p, int height, bool checkBlocksFit )
		{
			return CanFit( p.m_X, p.m_Y, p.m_Z, height, checkBlocksFit, true, true );
		}

		public bool CanFit( Point3D p, int height, bool checkBlocksFit, bool checkMobiles )
		{
			return CanFit( p.m_X, p.m_Y, p.m_Z, height, checkBlocksFit, checkMobiles, true );
		}

		public bool CanFit( Point2D p, int z, int height, bool checkBlocksFit )
		{
			return CanFit( p.m_X, p.m_Y, z, height, checkBlocksFit, true, true );
		}

		public bool CanFit( Point3D p, int height )
		{
			return CanFit( p.m_X, p.m_Y, p.m_Z, height, false, true, true );
		}

		public bool CanFit( Point2D p, int z, int height )
		{
			return CanFit( p.m_X, p.m_Y, z, height, false, true, true );
		}

		public bool CanFit( int x, int y, int z, int height )
		{
			return CanFit( x, y, z, height, false, true, true );
		}

		public bool CanFit( int x, int y, int z, int height, bool checksBlocksFit )
		{
			return CanFit( x, y, z, height, checksBlocksFit, true, true );
		}

		public bool CanFit( int x, int y, int z, int height, bool checkBlocksFit, bool checkMobiles )
		{
			return CanFit( x, y, z, height, checkBlocksFit, checkMobiles, true );
		}

		public bool CanFit( int x, int y, int z, int height, bool checkBlocksFit, bool checkMobiles, bool requireSurface )
		{
			if ( this == Map.Internal )
				return false;

			if ( x < 0 || y < 0 || x >= m_Width || y >= m_Height )
				return false;

			bool hasSurface = false;

			LandTile lt = Tiles.GetLandTile( x, y );
			int lowZ = 0, avgZ = 0, topZ = 0;

			GetAverageZ( x, y, ref lowZ, ref avgZ, ref topZ );
			TileFlag landFlags = TileData.LandTable[lt.ID & TileData.MaxLandValue].Flags;

			if ( ( landFlags & TileFlag.Impassable ) != 0 && avgZ > z && ( z + height ) > lowZ )
				return false;
			else if ( ( landFlags & TileFlag.Impassable ) == 0 && z == avgZ && !lt.Ignored )
				hasSurface = true;

			StaticTile[] staticTiles = Tiles.GetStaticTiles( x, y, true );

			bool surface, impassable;

			for ( int i = 0; i < staticTiles.Length; ++i )
			{
				ItemData id = TileData.ItemTable[staticTiles[i].ID & TileData.MaxItemValue];
				surface = id.Surface;
				impassable = id.Impassable;

				if ( ( surface || impassable ) && ( staticTiles[i].Z + id.CalcHeight ) > z && ( z + height ) > staticTiles[i].Z )
					return false;
				else if ( surface && !impassable && z == ( staticTiles[i].Z + id.CalcHeight ) )
					hasSurface = true;
			}

			Sector sector = GetSector( x, y );
			List<Item> items  = sector.Items;
			List<Mobile> mobs = sector.Mobiles;

			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = items[i];

				if ( !(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue && item.AtWorldPoint( x, y ) )
				{
					ItemData id = item.ItemData;
					surface = id.Surface;
					impassable = id.Impassable;

					if ( ( surface || impassable || ( checkBlocksFit && item.BlocksFit ) ) && ( item.Z + id.CalcHeight ) > z && ( z + height ) > item.Z )
						return false;
					else if ( surface && !impassable && !item.Movable && z == ( item.Z + id.CalcHeight ) )
						hasSurface = true;
				}
			}

			if ( checkMobiles )
			{
				for ( int i = 0; i < mobs.Count; ++i )
				{
					Mobile m = mobs[i];

					if ( m.Location.m_X == x && m.Location.m_Y == y && ( m.AccessLevel == AccessLevel.Player || !m.Hidden ) )
						if ( ( m.Z + 16 ) > z && ( z + height ) > m.Z )
							return false;
				}
			}

			return !requireSurface || hasSurface;
		}

		#endregion

		#region CanSpawnMobile
		public bool CanSpawnMobile( Point3D p )
		{
			return CanSpawnMobile( p.m_X, p.m_Y, p.m_Z );
		}

		public bool CanSpawnMobile( Point2D p, int z )
		{
			return CanSpawnMobile( p.m_X, p.m_Y, z );
		}

		public bool CanSpawnMobile( int x, int y, int z )
		{
			return CanSpawnMobile( x, y, z, true );
		}

		public bool CanSpawnMobile( int x, int y, int z, bool checkMobiles )
		{
			if ( !Region.Find( new Point3D( x, y, z ), this ).AllowSpawn() )
				return false;

			return CanFit( x, y, z, 16, false, checkMobiles );
		}
		#endregion

		private class ZComparer : IComparer<Item>
		{
			public static readonly ZComparer Default = new ZComparer();

			public int Compare( Item x, Item y )
			{
				return x.Z.CompareTo( y.Z );
			}
		}

		public void FixColumn( int x, int y )
		{
			LandTile landTile = Tiles.GetLandTile( x, y );

			int landZ = 0, landAvg = 0, landTop = 0;
			GetAverageZ( x, y, ref landZ, ref landAvg, ref landTop );

			StaticTile[] tiles = Tiles.GetStaticTiles( x, y, true );

			List<Item> items = new List<Item>();

			IPooledEnumerable eable = GetItemsInRange( new Point3D( x, y, 0 ), 0 );

			foreach ( Item item in eable )
			{
				if ( !(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue )
				{
					items.Add( item );

					if ( items.Count > 100 )
						break;
				}
			}

			eable.Free();

			if ( items.Count > 100 )
				return;

			items.Sort( ZComparer.Default );

			for ( int i = 0; i < items.Count; ++i )
			{
				Item toFix = items[i];

				if ( !toFix.Movable )
					continue;

				int z = int.MinValue;
				int currentZ = toFix.Z;

				if ( !landTile.Ignored && landAvg <= currentZ )
					z = landAvg;

				for ( int j = 0; j < tiles.Length; ++j )
				{
					StaticTile tile = tiles[j];
					ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

					int checkZ = tile.Z;
					int checkTop = checkZ + id.CalcHeight;

					if ( checkTop == checkZ && !id.Surface )
						++checkTop;

					if ( checkTop > z && checkTop <= currentZ )
						z = checkTop;
				}

				for ( int j = 0; j < items.Count; ++j )
				{
					if ( j == i )
						continue;

					Item item = items[j];
					ItemData id = item.ItemData;

					int checkZ = item.Z;
					int checkTop = checkZ + id.CalcHeight;

					if ( checkTop == checkZ && !id.Surface )
						++checkTop;

					if ( checkTop > z && checkTop <= currentZ )
						z = checkTop;
				}

				if ( z != int.MinValue )
					toFix.Location = new Point3D( toFix.X, toFix.Y, z );
			}
		}

		/* This could be probably be re-implemented if necessary (perhaps via an ITile interface?).
		public List<Tile> GetTilesAt( Point2D p, bool items, bool land, bool statics )
		{
			List<Tile> list = new List<Tile>();

			if ( this == Map.Internal )
				return list;

			if ( land )
				list.Add( Tiles.GetLandTile( p.m_X, p.m_Y ) );

			if ( statics )
				list.AddRange( Tiles.GetStaticTiles( p.m_X, p.m_Y, true ) );

			if ( items )
			{
				Sector sector = GetSector( p );

				foreach ( Item item in sector.Items )
					if ( item.AtWorldPoint( p.m_X, p.m_Y ) )
						list.Add( new StaticTile( (ushort)item.ItemID, (sbyte) item.Z ) );
			}

			return list;
		}
		*/

		/// <summary>
		/// Gets the highest surface that is lower than <paramref name="p"/>.
		/// </summary>
		/// <param name="p">The reference point.</param>
		/// <returns>A surface <typeparamref name="Tile"/> or <typeparamref name="Item"/>.</returns>
		public object GetTopSurface( Point3D p )
		{
			if ( this == Map.Internal )
				return null;

			object surface = null;
			int surfaceZ = int.MinValue;


			LandTile lt = Tiles.GetLandTile( p.X, p.Y );

			if ( !lt.Ignored )
			{
				int avgZ = GetAverageZ( p.X, p.Y );

				if ( avgZ <= p.Z )
				{
					surface = lt;
					surfaceZ = avgZ;

					if ( surfaceZ == p.Z )
						return surface;
				}
			}


			StaticTile[] staticTiles = Tiles.GetStaticTiles( p.X, p.Y, true );

			for ( int i = 0; i < staticTiles.Length; i++ )
			{
				StaticTile tile = staticTiles[i];
				ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				if ( id.Surface || ( id.Flags & TileFlag.Wet ) != 0 )
				{
					int tileZ = tile.Z + id.CalcHeight;

					if ( tileZ > surfaceZ && tileZ <= p.Z )
					{
						surface = tile;
						surfaceZ = tileZ;

						if ( surfaceZ == p.Z )
							return surface;
					}
				}
			}


			Sector sector = GetSector( p.X, p.Y );

			for ( int i = 0; i < sector.Items.Count; i++ )
			{
				Item item = sector.Items[i];

				if ( !(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue && item.AtWorldPoint( p.X, p.Y ) && !item.Movable )
				{
					ItemData id = item.ItemData;

					if ( id.Surface || ( id.Flags & TileFlag.Wet ) != 0 )
					{
						int itemZ = item.Z + id.CalcHeight;

						if ( itemZ > surfaceZ && itemZ <= p.Z )
						{
							surface = item;
							surfaceZ = itemZ;

							if ( surfaceZ == p.Z )
								return surface;
						}
					}
				}
			}


			return surface;
		}

		/// <summary>
		/// Gets the highest entity for a point
		/// </summary>
		public object GetHighestEntity( int x, int y )
		{
			if ( this == Map.Internal )
				return null;

			object surface = null;
			int surfaceZ = int.MinValue;

			LandTile lt = Tiles.GetLandTile( x, y );

			if ( !lt.Ignored )
			{
				int avgZ = GetAverageZ( x, y );
				surfaceZ = avgZ;
			}

			StaticTile[] staticTiles = Tiles.GetStaticTiles( x, y, true );

			for ( int i = 0; i < staticTiles.Length; i++ )
			{
				StaticTile tile = staticTiles[i];
				ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				if ( id.Surface || ( id.Flags & TileFlag.Wet ) != 0 )
				{
					int tileZ = tile.Z + id.CalcHeight;
					if (tileZ < surfaceZ) continue;

					surfaceZ = tileZ;
					surface = tile;
				}
			}

			Sector sector = GetSector( x, y );

			for ( int i = 0; i < sector.Items.Count; i++ )
			{
				Item item = sector.Items[i];

				if ( !(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue && item.AtWorldPoint( x, y ) && !item.Movable )
				{
					ItemData id = item.ItemData;

					if ( id.Surface || ( id.Flags & TileFlag.Wet ) != 0 )
					{
						int itemZ = item.Z + id.CalcHeight;
						if (itemZ < surfaceZ) continue;
						
						surfaceZ = itemZ;
						surface = item;
					}
				}
			}

			return surface ?? lt;
		}

		/// <summary>
		/// Gets the highest Z value for a point
		/// </summary>
		public int GetHighestPoint( int x, int y )
		{
			if ( this == Map.Internal )
				return 0;

			int surfaceZ = int.MinValue;

			LandTile lt = Tiles.GetLandTile( x, y );

			if ( !lt.Ignored )
			{
				int avgZ = GetAverageZ( x, y );
				surfaceZ = avgZ;
			}

			StaticTile[] staticTiles = Tiles.GetStaticTiles( x, y, true );

			for ( int i = 0; i < staticTiles.Length; i++ )
			{
				StaticTile tile = staticTiles[i];
				ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				if ( id.Surface || ( id.Flags & TileFlag.Wet ) != 0 )
				{
					int tileZ = tile.Z + id.CalcHeight;
					if (tileZ < surfaceZ) continue;

					surfaceZ = tileZ;
				}
			}


			Sector sector = GetSector( x, y );

			for ( int i = 0; i < sector.Items.Count; i++ )
			{
				Item item = sector.Items[i];

				if ( !(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue && item.AtWorldPoint( x, y ) && !item.Movable )
				{
					ItemData id = item.ItemData;

					if ( id.Surface || ( id.Flags & TileFlag.Wet ) != 0 )
					{
						int itemZ = item.Z + id.CalcHeight;
						if (itemZ < surfaceZ) continue;
						
						surfaceZ = itemZ;
					}
				}
			}


			return surfaceZ;
		}

		public void Bound( int x, int y, out int newX, out int newY )
		{
			if ( x < 0 )
				newX = 0;
			else if ( x >= m_Width )
				newX = m_Width - 1;
			else
				newX = x;

			if ( y < 0 )
				newY = 0;
			else if ( y >= m_Height )
				newY = m_Height - 1;
			else
				newY = y;
		}

		public Point2D Bound( Point2D p )
		{
			int x = p.m_X, y = p.m_Y;

			if ( x < 0 )
				x = 0;
			else if ( x >= m_Width )
				x = m_Width - 1;

			if ( y < 0 )
				y = 0;
			else if ( y >= m_Height )
				y = m_Height - 1;

			return new Point2D( x, y );
		}

		public Map( int mapID, int mapIndex, int fileIndex, int width, int height, int season, string name, MapRules rules )
		{
			m_MapID = mapID;
			m_MapIndex = mapIndex;
			m_FileIndex = fileIndex;
			m_Width = width;
			m_Height = height;
			m_Season = season;
			m_Name = name;
			m_Rules = rules;
			m_Regions = new Dictionary<string, Region>( StringComparer.OrdinalIgnoreCase );
			m_InvalidSector = new Sector( 0, 0, this );
			m_SectorsWidth = width >> SectorShift;
			m_SectorsHeight = height >> SectorShift;
			m_Sectors = new Sector[m_SectorsWidth][];
		}

		#region GetSector
		public Sector GetSector( Point3D p )
		{
			return InternalGetSector( p.m_X >> SectorShift, p.m_Y >> SectorShift );
		}

		public Sector GetSector( Point2D p )
		{
			return InternalGetSector( p.m_X >> SectorShift, p.m_Y >> SectorShift );
		}

		public Sector GetSector( IPoint2D p )
		{
			return InternalGetSector( p.X >> SectorShift, p.Y >> SectorShift );
		}

		public Sector GetSector( int x, int y )
		{
			return InternalGetSector( x >> SectorShift, y >> SectorShift );
		}

		public Sector GetRealSector( int x, int y )
		{
			return InternalGetSector( x, y );
		}

		private Sector InternalGetSector( int x, int y )
		{
			if ( x >= 0 && x < m_SectorsWidth && y >= 0 && y < m_SectorsHeight )
			{
				Sector[] xSectors = m_Sectors[x];

				if ( xSectors == null )
					m_Sectors[x] = xSectors = new Sector[m_SectorsHeight];

				Sector sec = xSectors[y];

				if ( sec == null )
					xSectors[y] = sec = new Sector( x, y, this );

				return sec;
			}
			else
			{
				return m_InvalidSector;
			}
		}
		#endregion

		public void ActivateSectors( int cx, int cy )
		{
			for ( int x = cx - SectorActiveRange; x <= cx + SectorActiveRange; ++x )
			{
				for ( int y = cy - SectorActiveRange; y <= cy + SectorActiveRange; ++y )
				{
					Sector sect = GetRealSector( x, y );
					if ( sect != m_InvalidSector )
						sect.Activate();
				}
			}
		}

		public void DeactivateSectors( int cx, int cy )
		{
			for ( int x = cx - SectorActiveRange; x <= cx + SectorActiveRange; ++x )
			{
				for ( int y = cy - SectorActiveRange; y <= cy + SectorActiveRange; ++y )
				{
					Sector sect = GetRealSector( x, y );
					if ( sect != m_InvalidSector && !PlayersInRange( sect, SectorActiveRange ) )
						sect.Deactivate();
				}
			}
		}

		private bool PlayersInRange( Sector sect, int range )
		{
			for ( int x = sect.X - range; x <= sect.X + range; ++x )
			{
				for ( int y = sect.Y - range; y <= sect.Y + range; ++y )
				{
					Sector check = GetRealSector( x, y );
					if ( check != m_InvalidSector && check.Players.Count > 0 )
						return true;
				}
			}

			return false;
		}

		public void OnClientChange( NetState oldState, NetState newState, Mobile m )
		{
			if ( this == Map.Internal )
				return;

			GetSector( m ).OnClientChange( oldState, newState );
		}

		public void OnEnter( Mobile m )
		{
			if ( this == Map.Internal )
				return;

			Sector sector = GetSector( m );

			sector.OnEnter( m );
		}

		public void OnEnter( Item item )
		{
			if ( this == Map.Internal )
				return;

			GetSector( item ).OnEnter( item );

			if ( item is BaseMulti )
			{
				BaseMulti m = (BaseMulti)item;
				MultiComponentList mcl = m.Components;

				Sector start = GetMultiMinSector( item.Location, mcl );
				Sector end = GetMultiMaxSector( item.Location, mcl );

				AddMulti( m, start, end );
			}
		}

		public void OnLeave( Mobile m )
		{
			if ( this == Map.Internal )
				return;

			Sector sector = GetSector( m );

			sector.OnLeave( m );
		}

		public void OnLeave( Item item )
		{
			if ( this == Map.Internal )
				return;

			GetSector( item ).OnLeave( item );

			if ( item is BaseMulti )
			{
				BaseMulti m = (BaseMulti)item;
				MultiComponentList mcl = m.Components;

				Sector start = GetMultiMinSector( item.Location, mcl );
				Sector end = GetMultiMaxSector( item.Location, mcl );

				RemoveMulti( m, start, end );
			}
		}

		public void RemoveMulti( BaseMulti m, Sector start, Sector end )
		{
			if ( this == Map.Internal )
				return;

			for ( int x = start.X; x <= end.X; ++x )
				for ( int y = start.Y; y <= end.Y; ++y )
					InternalGetSector( x, y ).OnMultiLeave( m );
		}

		public void AddMulti( BaseMulti m, Sector start, Sector end )
		{
			if ( this == Map.Internal )
				return;

			for ( int x = start.X; x <= end.X; ++x )
				for ( int y = start.Y; y <= end.Y; ++y )
					InternalGetSector( x, y ).OnMultiEnter( m );
		}

		public Sector GetMultiMinSector( Point3D loc, MultiComponentList mcl )
		{
			return GetSector( Bound( new Point2D( loc.m_X + mcl.Min.m_X, loc.m_Y + mcl.Min.m_Y ) ) );
		}

		public Sector GetMultiMaxSector( Point3D loc, MultiComponentList mcl )
		{
			return GetSector( Bound( new Point2D( loc.m_X + mcl.Max.m_X, loc.m_Y + mcl.Max.m_Y ) ) );
		}

		public void OnMove( Point3D oldLocation, Mobile m )
		{
			if ( this == Map.Internal )
				return;

			Sector oldSector = GetSector( oldLocation );
			Sector newSector = GetSector( m.Location );

			if ( oldSector != newSector )
			{
				oldSector.OnLeave( m );
				newSector.OnEnter( m );
			}
		}

		public void OnMove( Point3D oldLocation, Item item )
		{
			if ( this == Map.Internal )
				return;

			Sector oldSector = GetSector( oldLocation );
			Sector newSector = GetSector( item.Location );

			if ( oldSector != newSector )
			{
				oldSector.OnLeave( item );
				newSector.OnEnter( item );
			}

			if ( item is BaseMulti )
			{
				BaseMulti m = (BaseMulti)item;
				MultiComponentList mcl = m.Components;

				Sector start = GetMultiMinSector( item.Location, mcl );
				Sector end = GetMultiMaxSector( item.Location, mcl );

				Sector oldStart = GetMultiMinSector( oldLocation, mcl );
				Sector oldEnd = GetMultiMaxSector( oldLocation, mcl );

				if ( oldStart != start || oldEnd != end )
				{
					RemoveMulti( m, oldStart, oldEnd );
					AddMulti( m, start, end );
				}
			}
		}

		public TileMatrix Tiles
		{
			get
			{
				if ( m_Tiles == null )
					m_Tiles = new TileMatrix( this, m_FileIndex, m_MapID, m_Width, m_Height );

				return m_Tiles;
			}
		}

		public int MapID
		{
			get
			{
				return m_MapID;
			}
		}

		public int MapIndex
		{
			get
			{
				return m_MapIndex;
			}
		}

		public int Width
		{
			get
			{
				return m_Width;
			}
		}

		public int Height
		{
			get
			{
				return m_Height;
			}
		}

		public Dictionary<string, Region> Regions
		{
			get
			{
				return m_Regions;
			}
		}

		public void RegisterRegion( Region reg )
		{
			string regName = reg.Name;

			if ( regName != null )
			{
				if ( m_Regions.ContainsKey( regName ) )
					Console.WriteLine( "Warning: Duplicate region name '{0}' for map '{1}'", regName, this.Name );
				else
					m_Regions[regName] = reg;
			}
		}

		public void UnregisterRegion( Region reg )
		{
			string regName = reg.Name;

			if ( regName != null )
				m_Regions.Remove( regName );
		}

		public Region DefaultRegion
		{
			get
			{
				if ( m_DefaultRegion == null )
					m_DefaultRegion = new Region( null, this, 0, new Rectangle3D[0] );

				return m_DefaultRegion;
			}
			set
			{
				m_DefaultRegion = value;
			}
		}

		public MapRules Rules
		{
			get
			{
				return m_Rules;
			}
			set
			{
				m_Rules = value;
			}
		}

		public Sector InvalidSector
		{
			get
			{
				return m_InvalidSector;
			}
		}

		public string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}

		#region Enumerables
		public class NullEnumerable : IPooledEnumerable
		{
			private InternalEnumerator m_Enumerator;

			public static readonly NullEnumerable Instance = new NullEnumerable();

			private NullEnumerable()
			{
				m_Enumerator = new InternalEnumerator();
			}

			public IEnumerator GetEnumerator()
			{
				return m_Enumerator;
			}

			public void Free()
			{
			}

			private class InternalEnumerator : IEnumerator
			{
				public void Reset()
				{
				}

				public object Current
				{
					get
					{
						return null;
					}
				}

				public bool MoveNext()
				{
					return false;
				}
			}
		}

		private class PooledEnumerable : IPooledEnumerable, IDisposable
		{
			private IPooledEnumerator m_Enumerator;

			private static Queue<PooledEnumerable> m_InstancePool = new Queue<PooledEnumerable>();
			private static int m_Depth = 0;

			public static PooledEnumerable Instantiate( IPooledEnumerator etor )
			{
				++m_Depth;

				if ( m_Depth >= 5 )
					Console.WriteLine( "Warning: Make sure to call .Free() on pooled enumerables." );

				PooledEnumerable e;

				if ( m_InstancePool.Count > 0 )
				{
					e = m_InstancePool.Dequeue();
					e.m_Enumerator = etor;
				}
				else
				{
					e = new PooledEnumerable( etor );
				}

				etor.Enumerable = e;

				return e;
			}

			private PooledEnumerable( IPooledEnumerator etor )
			{
				m_Enumerator = etor;
			}

			public IEnumerator GetEnumerator()
			{
				if ( m_Enumerator == null )
					throw new ObjectDisposedException( "PooledEnumerable", "GetEnumerator() called after Free()" );

				return m_Enumerator;
			}

			public void Free()
			{
				if ( m_Enumerator != null )
				{
					m_InstancePool.Enqueue( this );

					m_Enumerator.Free();
					m_Enumerator = null;

					--m_Depth;
				}
			}

			public void Dispose()
			{
				Free();
			}
		}
		#endregion

		#region Enumerators
		private enum SectorEnumeratorType
		{
			Mobiles,
			Items,
			Clients
		}

		private class TypedEnumerator : IPooledEnumerator, IDisposable
		{
			private IPooledEnumerable m_Enumerable;

			public IPooledEnumerable Enumerable
			{
				get { return m_Enumerable; }
				set { m_Enumerable = value; }
			}

			private Map m_Map;
			private Rectangle2D m_Bounds;
			private SectorEnumerator m_Enumerator;
			private SectorEnumeratorType m_Type;
			private object m_Current;

			private static Queue<TypedEnumerator> m_InstancePool = new Queue<TypedEnumerator>();

			public static TypedEnumerator Instantiate( Map map, Rectangle2D bounds, SectorEnumeratorType type )
			{
				TypedEnumerator e;

				if ( m_InstancePool.Count > 0 )
				{
					e = m_InstancePool.Dequeue();

					e.m_Map = map;
					e.m_Bounds = bounds;
					e.m_Type = type;

					e.Reset();
				}
				else
				{
					e = new TypedEnumerator( map, bounds, type );
				}

				return e;
			}

			public void Free()
			{
				if ( m_Map == null )
					return;

				m_InstancePool.Enqueue( this );

				m_Map = null;

				if ( m_Enumerator != null )
				{
					m_Enumerator.Free();
					m_Enumerator = null;
				}

				if ( m_Enumerable != null )
					m_Enumerable.Free();
			}

			public TypedEnumerator( Map map, Rectangle2D bounds, SectorEnumeratorType type )
			{
				m_Map = map;
				m_Bounds = bounds;
				m_Type = type;

				Reset();
			}

			public object Current
			{
				get
				{
					return m_Current;
				}
			}

			public bool MoveNext()
			{
				while ( true )
				{
					if ( m_Enumerator.MoveNext() )
					{
						object o;

						try
						{
							o = m_Enumerator.Current;
						}
						catch
						{
							continue;
						}

						if ( o is Mobile )
						{
							Mobile m = (Mobile) o;

							if ( !m.Deleted && m_Bounds.Contains( m.Location ) )
							{
								m_Current = o;
								return true;
							}
						}
						else if ( o is Item )
						{
							Item item = (Item) o;

							if ( !item.Deleted && item.Parent == null && m_Bounds.Contains( item.Location ) )
							{
								m_Current = o;
								return true;
							}
						}
						else if ( o is NetState )
						{
							Mobile m = ( (NetState) o ).Mobile;

							if ( m != null && !m.Deleted && m_Bounds.Contains( m.Location ) )
							{
								m_Current = o;
								return true;
							}
						}
					}
					else
					{
						m_Current = null;

						m_Enumerator.Free();
						m_Enumerator = null;

						return false;
					}
				}
			}

			public void Reset()
			{
				m_Current = null;

				if ( m_Enumerator != null )
					m_Enumerator.Free();

				m_Enumerator = SectorEnumerator.Instantiate( m_Map, m_Bounds, m_Type );//new SectorEnumerator( m_Map, m_Origin, m_Type, m_Range );
			}

			public void Dispose()
			{
				Free();
			}
		}

		private class MultiTileEnumerator : IPooledEnumerator, IDisposable
		{
			private IPooledEnumerable m_Enumerable;

			public IPooledEnumerable Enumerable
			{
				get { return m_Enumerable; }
				set { m_Enumerable = value; }
			}

			private List<BaseMulti> m_List;
			private Point2D m_Location;
			private object m_Current;
			private int m_Index;

			private static Queue<MultiTileEnumerator> m_InstancePool = new Queue<MultiTileEnumerator>();

			public static MultiTileEnumerator Instantiate( Sector sector, Point2D loc )
			{
				MultiTileEnumerator e;

				if ( m_InstancePool.Count > 0 )
				{
					e = m_InstancePool.Dequeue();

					e.m_List = sector.Multis;
					e.m_Location = loc;

					e.Reset();
				}
				else
				{
					e = new MultiTileEnumerator( sector, loc );
				}

				return e;
			}

			private MultiTileEnumerator( Sector sector, Point2D loc )
			{
				m_List = sector.Multis;
				m_Location = loc;

				Reset();
			}

			public object Current
			{
				get
				{
					return m_Current;
				}
			}

			public bool MoveNext()
			{
				while ( ++m_Index < m_List.Count )
				{
					BaseMulti m = m_List[m_Index];

					if ( m != null && !m.Deleted )
					{
						MultiComponentList list = m.Components;

						int xOffset = m_Location.m_X - ( m.Location.m_X + list.Min.m_X );
						int yOffset = m_Location.m_Y - ( m.Location.m_Y + list.Min.m_Y );

						if ( xOffset >= 0 && xOffset < list.Width && yOffset >= 0 && yOffset < list.Height )
						{
							StaticTile[] tiles = list.Tiles[xOffset][yOffset];

							if ( tiles.Length > 0 )
							{
								// TODO: How to avoid this copy?
								StaticTile[] copy = new StaticTile[tiles.Length];

								for ( int i = 0; i < copy.Length; ++i )
								{
									copy[i] = tiles[i];
									copy[i].Z += m.Z;
								}

								m_Current = copy;
								return true;
							}
						}
					}
				}

				return false;
			}

			public void Free()
			{
				if ( m_List == null )
					return;

				m_InstancePool.Enqueue( this );

				m_List = null;

				if ( m_Enumerable != null )
					m_Enumerable.Free();
			}

			public void Reset()
			{
				m_Current = null;
				m_Index = -1;
			}

			public void Dispose()
			{
				Free();
			}
		}

		private class ObjectEnumerator : IPooledEnumerator, IDisposable
		{
			private IPooledEnumerable m_Enumerable;

			public IPooledEnumerable Enumerable
			{
				get { return m_Enumerable; }
				set { m_Enumerable = value; }
			}

			private Map m_Map;
			private Rectangle2D m_Bounds;
			private SectorEnumerator m_Enumerator;
			private int m_Stage; // 0 = items, 1 = mobiles
			private object m_Current;

			private static Queue<ObjectEnumerator> m_InstancePool = new Queue<ObjectEnumerator>();

			public static ObjectEnumerator Instantiate( Map map, Rectangle2D bounds )
			{
				ObjectEnumerator e;

				if ( m_InstancePool.Count > 0 )
				{
					e = m_InstancePool.Dequeue();

					e.m_Map = map;
					e.m_Bounds = bounds;

					e.Reset();
				}
				else
				{
					e = new ObjectEnumerator( map, bounds );
				}

				return e;
			}

			public void Free()
			{
				if ( m_Map == null )
					return;

				m_InstancePool.Enqueue( this );

				m_Map = null;

				if ( m_Enumerator != null )
				{
					m_Enumerator.Free();
					m_Enumerator = null;
				}

				if ( m_Enumerable != null )
					m_Enumerable.Free();
			}

			private ObjectEnumerator( Map map, Rectangle2D bounds )
			{
				m_Map = map;
				m_Bounds = bounds;

				Reset();
			}

			public object Current
			{
				get
				{
					return m_Current;
				}
			}

			public bool MoveNext()
			{
				while ( true )
				{
					if ( m_Enumerator.MoveNext() )
					{
						object o;

						try
						{
							o = m_Enumerator.Current;
						}
						catch
						{
							continue;
						}

						if ( o is Mobile )
						{
							Mobile m = (Mobile) o;

							if ( m_Bounds.Contains( m.Location ) )
							{
								m_Current = o;
								return true;
							}
						}
						else if ( o is Item )
						{
							Item item = (Item) o;

							if ( item.Parent == null && m_Bounds.Contains( item.Location ) )
							{
								m_Current = o;
								return true;
							}
						}
					}
					else if ( m_Stage == 0 )
					{
						m_Enumerator.Free();
						m_Enumerator = SectorEnumerator.Instantiate( m_Map, m_Bounds, SectorEnumeratorType.Mobiles );

						m_Current = null;
						m_Stage = 1;
					}
					else
					{
						m_Enumerator.Free();
						m_Enumerator = null;

						m_Current = null;
						m_Stage = -1;

						return false;
					}
				}
			}

			public void Reset()
			{
				m_Stage = 0;

				m_Current = null;

				if ( m_Enumerator != null )
					m_Enumerator.Free();

				m_Enumerator = SectorEnumerator.Instantiate( m_Map, m_Bounds, SectorEnumeratorType.Items );
			}

			public void Dispose()
			{
				Free();
			}
		}

		private class SectorEnumerator : IPooledEnumerator, IDisposable
		{
			private IPooledEnumerable m_Enumerable;

			public IPooledEnumerable Enumerable
			{
				get { return m_Enumerable; }
				set { m_Enumerable = value; }
			}

			private Map m_Map;
			private Rectangle2D m_Bounds;

			private int m_xSector, m_ySector;
			private int m_xSectorStart, m_ySectorStart;
			private int m_xSectorEnd, m_ySectorEnd;
			private IList m_CurrentList;
			private int m_CurrentIndex;
			private SectorEnumeratorType m_Type;

			private static Queue<SectorEnumerator> m_InstancePool = new Queue<SectorEnumerator>();

			public static SectorEnumerator Instantiate( Map map, Rectangle2D bounds, SectorEnumeratorType type )
			{
				SectorEnumerator e;

				if ( m_InstancePool.Count > 0 )
				{
					e = m_InstancePool.Dequeue();

					e.m_Map = map;
					e.m_Bounds = bounds;
					e.m_Type = type;

					e.Reset();
				}
				else
				{
					e = new SectorEnumerator( map, bounds, type );
				}

				return e;
			}

			public void Free()
			{
				if ( m_Map == null )
					return;

				m_InstancePool.Enqueue( this );

				m_Map = null;

				if ( m_Enumerable != null )
					m_Enumerable.Free();
			}

			private SectorEnumerator( Map map, Rectangle2D bounds, SectorEnumeratorType type )
			{
				m_Map = map;
				m_Bounds = bounds;
				m_Type = type;

				Reset();
			}

			private IList GetListForSector( Sector sector )
			{
				switch ( m_Type )
				{
					case SectorEnumeratorType.Clients:
						return sector.Clients;
					case SectorEnumeratorType.Mobiles:
						return sector.Mobiles;
					case SectorEnumeratorType.Items:
						return sector.Items;
					default:
						throw new Exception( "Invalid SectorEnumeratorType" );
				}
			}

			public object Current
			{
				get
				{
					return m_CurrentList[m_CurrentIndex];
					/*try
					{
						return m_CurrentList[m_CurrentIndex];
					}
					catch
					{
						Console.WriteLine( "Warning: Object removed during enumeration. May not be recoverable" );

						m_CurrentIndex = -1;
						m_CurrentList = GetListForSector( m_Map.InternalGetSector( m_xSector, m_ySector ) );

						if ( MoveNext() )
						{
							return Current;
						}
						else
						{
							throw new Exception( "Object disposed during enumeration. Was not recoverable." );
						}
					}*/
				}
			}

			public bool MoveNext()
			{
				while ( true )
				{
					++m_CurrentIndex;

					if ( m_CurrentIndex == m_CurrentList.Count )
					{
						++m_ySector;

						if ( m_ySector > m_ySectorEnd )
						{
							m_ySector = m_ySectorStart;
							++m_xSector;

							if ( m_xSector > m_xSectorEnd )
							{
								m_CurrentIndex = -1;
								m_CurrentList = null;

								return false;
							}
						}

						m_CurrentIndex = -1;
						m_CurrentList = GetListForSector( m_Map.InternalGetSector( m_xSector, m_ySector ) );//m_Map.m_Sectors[m_xSector][m_ySector] );
					}
					else
					{
						return true;
					}
				}
			}

			public void Reset()
			{
				m_Map.Bound( m_Bounds.Start.m_X, m_Bounds.Start.m_Y, out m_xSectorStart, out m_ySectorStart );
				m_Map.Bound( m_Bounds.End.m_X - 1, m_Bounds.End.m_Y - 1, out m_xSectorEnd, out m_ySectorEnd );

				m_xSector = m_xSectorStart >>= Map.SectorShift;
				m_ySector = m_ySectorStart >>= Map.SectorShift;

				m_xSectorEnd >>= Map.SectorShift;
				m_ySectorEnd >>= Map.SectorShift;

				m_CurrentIndex = -1;
				m_CurrentList = GetListForSector( m_Map.InternalGetSector( m_xSector, m_ySector ) );
			}

			public void Dispose()
			{
				Free();
			}
		}
		#endregion

		public Point3D GetPoint( object o, bool eye )
		{
			Point3D p;

			if( o is Mobile )
			{
				p = ((Mobile)o).Location;
				p.Z += 14;//eye ? 15 : 10;
			}
			else if( o is Item )
			{
				p = ((Item)o).GetWorldLocation();
				p.Z += (((Item)o).ItemData.Height / 2) + 1;
			}
			else if( o is Point3D )
			{
				p = (Point3D)o;
			}
			else if( o is LandTarget )
			{
				p = ((LandTarget)o).Location;

				int low = 0, avg = 0, top = 0;
				GetAverageZ( p.X, p.Y, ref low, ref avg, ref top );

				p.Z = top + 1;
			}
			else if( o is StaticTarget )
			{
				StaticTarget st = (StaticTarget)o;
				ItemData id = TileData.ItemTable[st.ItemID & TileData.MaxItemValue];

				p = new Point3D( st.X, st.Y, st.Z - id.CalcHeight + (id.Height / 2) + 1 );
			}
			else if( o is IPoint3D )
			{
				p = new Point3D( (IPoint3D)o );
			}
			else
			{
				Console.WriteLine( "Warning: Invalid object ({0}) in line of sight", o );
				p = Point3D.Zero;
			}

			return p;
		}

		#region Line Of Sight
		private static int m_MaxLOSDistance = 25;

		public static int MaxLOSDistance
		{
			get { return m_MaxLOSDistance; }
			set { m_MaxLOSDistance = value; }
		}

		public bool LineOfSight( Point3D org, Point3D dest )
		{
			if( this == Map.Internal )
				return false;

			if( !Utility.InRange( org, dest, m_MaxLOSDistance ) )
				return false;

			Point3D start = org;
			Point3D end = dest;

			if( org.X > dest.X || (org.X == dest.X && org.Y > dest.Y) || (org.X == dest.X && org.Y == dest.Y && org.Z > dest.Z) )
			{
				Point3D swap = org;
				org = dest;
				dest = swap;
			}

			double rise, run, zslp;
			double sq3d;
			double x, y, z;
			int xd, yd, zd;
			int ix, iy, iz;
			int height;
			bool found;
			Point3D p;
			Point3DList path = m_PathList;
			TileFlag flags;

			if( org == dest )
				return true;

			if( path.Count > 0 )
				path.Clear();

			xd = dest.m_X - org.m_X;
			yd = dest.m_Y - org.m_Y;
			zd = dest.m_Z - org.m_Z;
			zslp = Math.Sqrt( xd * xd + yd * yd );
			if( zd != 0 )
				sq3d = Math.Sqrt( zslp * zslp + zd * zd );
			else
				sq3d = zslp;

			rise = ((float)yd) / sq3d;
			run = ((float)xd) / sq3d;
			zslp = ((float)zd) / sq3d;

			y = org.m_Y;
			z = org.m_Z;
			x = org.m_X;
			while( Utility.NumberBetween( x, dest.m_X, org.m_X, 0.5 ) && Utility.NumberBetween( y, dest.m_Y, org.m_Y, 0.5 ) && Utility.NumberBetween( z, dest.m_Z, org.m_Z, 0.5 ) )
			{
				ix = (int)Math.Round( x );
				iy = (int)Math.Round( y );
				iz = (int)Math.Round( z );
				if( path.Count > 0 )
				{
					p = path.Last;

					if( p.m_X != ix || p.m_Y != iy || p.m_Z != iz )
						path.Add( ix, iy, iz );
				}
				else
				{
					path.Add( ix, iy, iz );
				}
				x += run;
				y += rise;
				z += zslp;
			}

			if( path.Count == 0 )
				return true;//<--should never happen, but to be safe.

			p = path.Last;

			if( p != dest )
				path.Add( dest );

			Point3D pTop = org, pBottom = dest;
			Utility.FixPoints( ref pTop, ref pBottom );

			int pathCount = path.Count;

			for( int i = 0; i < pathCount; ++i )
			{
				Point3D point = path[i];

				LandTile landTile = Tiles.GetLandTile( point.X, point.Y );
				int landZ = 0, landAvg = 0, landTop = 0;
				GetAverageZ( point.m_X, point.m_Y, ref landZ, ref landAvg, ref landTop );

				if( landZ <= point.m_Z && landTop >= point.m_Z && (point.m_X != end.m_X || point.m_Y != end.m_Y || landZ > end.m_Z || landTop < end.m_Z) && !landTile.Ignored )
					return false;

				/* --Do land tiles need to be checked?  There is never land between two people, always statics.--
				LandTile landTile = Tiles.GetLandTile( point.X, point.Y );
				if ( landTile.Z-1 >= point.Z && landTile.Z+1 <= point.Z && (TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags & TileFlag.Impassable) != 0 )
					return false;
				*/

				StaticTile[] statics = Tiles.GetStaticTiles( point.m_X, point.m_Y, true );

				bool contains = false;
				int ltID = landTile.ID;

				for( int j = 0; !contains && j < m_InvalidLandTiles.Length; ++j )
					contains = (ltID == m_InvalidLandTiles[j]);

				if( contains && statics.Length == 0 )
				{
					IPooledEnumerable eable = GetItemsInRange( point, 0 );

					foreach( Item item in eable )
					{
						if( item.Visible )
							contains = false;

						if( !contains )
							break;
					}

					eable.Free();

					if( contains )
						return false;
				}

				for( int j = 0; j < statics.Length; ++j )
				{
					StaticTile t = statics[j];

					ItemData id = TileData.ItemTable[t.ID & TileData.MaxItemValue];

					flags = id.Flags;
					height = id.CalcHeight;

					if( t.Z <= point.Z && t.Z + height >= point.Z && (flags & (TileFlag.Window | TileFlag.NoShoot)) != 0 )
					{
						if( point.m_X == end.m_X && point.m_Y == end.m_Y && t.Z <= end.m_Z && t.Z + height >= end.m_Z )
							continue;

						return false;
					}

					/*if ( t.Z <= point.Z && t.Z+height >= point.Z && (flags&TileFlag.Window)==0 && (flags&TileFlag.NoShoot)!=0
						&& ( (flags&TileFlag.Wall)!=0 || (flags&TileFlag.Roof)!=0 || (((flags&TileFlag.Surface)!=0 && zd != 0)) ) )*/
					/*{
						//Console.WriteLine( "LoS: Blocked by Static \"{0}\" Z:{1} T:{3} P:{2} F:x{4:X}", TileData.ItemTable[t.ID&TileData.MaxItemValue].Name, t.Z, point, t.Z+height, flags );
						//Console.WriteLine( "if ( {0} && {1} && {2} && ( {3} || {4} || {5} || ({6} && {7} && {8}) ) )", t.Z <= point.Z, t.Z+height >= point.Z, (flags&TileFlag.Window)==0, (flags&TileFlag.Impassable)!=0, (flags&TileFlag.Wall)!=0, (flags&TileFlag.Roof)!=0, (flags&TileFlag.Surface)!=0, t.Z != dest.Z, zd != 0 ) ;
						return false;
					}*/
				}
			}

			Rectangle2D rect = new Rectangle2D( pTop.m_X, pTop.m_Y, (pBottom.m_X - pTop.m_X) + 1, (pBottom.m_Y - pTop.m_Y) + 1 );

			IPooledEnumerable area = GetItemsInBounds( rect );

			foreach( Item i in area )
			{
				if( !i.Visible )
					continue;

				if( i is BaseMulti || i.ItemID > TileData.MaxItemValue )
					continue;

				ItemData id = i.ItemData;
				flags = id.Flags;

				if( (flags & (TileFlag.Window | TileFlag.NoShoot)) == 0 )
					continue;

				height = id.CalcHeight;

				found = false;

				int count = path.Count;

				for( int j = 0; j < count; ++j )
				{
					Point3D point = path[j];
					Point3D loc = i.Location;

					//if ( t.Z <= point.Z && t.Z+height >= point.Z && ( height != 0 || ( t.Z == dest.Z && zd != 0 ) ) )
					if( loc.m_X == point.m_X && loc.m_Y == point.m_Y &&
						loc.m_Z <= point.m_Z && loc.m_Z + height >= point.m_Z )
					{
						if( loc.m_X == end.m_X && loc.m_Y == end.m_Y && loc.m_Z <= end.m_Z && loc.m_Z + height >= end.m_Z )
							continue;

						found = true;
						break;
					}
				}

				if( !found )
					continue;

				area.Free();
				return false;

				/*if ( (flags & (TileFlag.Impassable | TileFlag.Surface | TileFlag.Roof)) != 0 )

				//flags = TileData.ItemTable[i.ItemID&TileData.MaxItemValue].Flags;
				//if ( (flags&TileFlag.Window)==0 && (flags&TileFlag.NoShoot)!=0 && ( (flags&TileFlag.Wall)!=0 || (flags&TileFlag.Roof)!=0 || (((flags&TileFlag.Surface)!=0 && zd != 0)) ) )
				{
					//height = TileData.ItemTable[i.ItemID&TileData.MaxItemValue].Height;
					//Console.WriteLine( "LoS: Blocked by ITEM \"{0}\" P:{1} T:{2} F:x{3:X}", TileData.ItemTable[i.ItemID&TileData.MaxItemValue].Name, i.Location, i.Location.Z+height, flags );
					area.Free();
					return false;
				}*/
			}

			area.Free();

			return true;
		}

		public bool LineOfSight( object from, object dest )
		{
			if ( from == dest || ( from is Mobile && ( (Mobile) from ).AccessLevel > AccessLevel.Player ) )
				return true;
			else if ( dest is Item && from is Mobile && ( (Item) dest ).RootParent == from )
				return true;

			return LineOfSight( GetPoint( from, true ), GetPoint( dest, false ) );
		}

		public bool LineOfSight( Mobile from, Point3D target )
		{
			if ( from.AccessLevel > AccessLevel.Player )
				return true;

			Point3D eye = from.Location;

			eye.Z += 14;

			return LineOfSight( eye, target );
		}

		public bool LineOfSight( Mobile from, Mobile to )
		{
			if ( from == to || from.AccessLevel > AccessLevel.Player )
				return true;

			Point3D eye = from.Location;
			Point3D target = to.Location;

			eye.Z += 14;
			target.Z += 14;//10;

			return LineOfSight( eye, target );
		}
		#endregion

		private static int[] m_InvalidLandTiles = new int[] { 0x244 };

		public static int[] InvalidLandTiles
		{
			get { return m_InvalidLandTiles; }
			set { m_InvalidLandTiles = value; }
		}

		private static Point3DList m_PathList = new Point3DList();
		public int CompareTo( Map other )
		{
			if ( other == null )
				return -1;

			return m_MapID.CompareTo( other.m_MapID );
		}

		public int CompareTo( object other )
		{
			if ( other == null || other is Map )
				return this.CompareTo( other );

			throw new ArgumentException();
		}
	}
}