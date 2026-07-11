using System;
using Server.Mobiles;
using Server.Utilities;

namespace Server.Items
{
	public class StrangePortal : Item
	{
		public int GateLocation;

		[CommandProperty(AccessLevel.Owner)]
		public int Gate_Location { get { return GateLocation; } set { GateLocation = value; InvalidateProperties(); } }

		[Constructable]
		public StrangePortal() : base(0x3D5E)
		{
			Movable = false;
			Light = LightType.Circle300;

			string sCalled = "a strange";
			switch( Utility.RandomMinMax( 0, 6 ) )
			{
				case 0: sCalled = "an odd"; break;
				case 1: sCalled = "an unusual"; break;
				case 2: sCalled = "a bizarre"; break;
				case 3: sCalled = "a curious"; break;
				case 4: sCalled = "a peculiar"; break;
				case 5: sCalled = "a strange"; break;
				case 6: sCalled = "a weird"; break;
			}

			Name = sCalled + " portal";
			Hue = Utility.RandomList( 0xB96, 0xB80, 0xB7F, 0xB79, 0xB77, 0xB71, 0xB70, 0xB64, 0xB63, 0, 0xB61, 0xB50, 0xB51, 0xB52, 0xB53, 0xB3D, 0xB17, 0xB09, 0xB0A, 0xB0B, 0xB0C, 0xB0F, 0xAFE, 0xAFF, 0xB00, 0xB01, 0xB02, 0xB03, 0xAF8, 0xABB, 0xABC );
		}

		public StrangePortal(Serial serial) : base(serial)
		{
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( m is PlayerMobile )
			{
				UseGate( m, GateLocation, null );
				Effects.PlaySound( m.Location, m.Map, 0x1FC );
			}

			return false;
		}

        public override void OnAfterSpawn()
        {
			base.OnAfterSpawn();

			int maxRetry = 10;
			int attempt = 0;
			while ( ++attempt < maxRetry )
			{
				if ( Land == Land.Lodoria ){ GateLocation = Utility.RandomMinMax( 0, 14 ); }
				else if ( Land == Land.Serpent ){ GateLocation = Utility.RandomMinMax( 27, 41 ); }
				else if ( Land == Land.IslesDread ){ GateLocation = Utility.RandomMinMax( 42, 47 ); }
				else if ( Land == Land.Savaged ){ GateLocation = Utility.RandomMinMax( 48, 56 ); }
				else if ( Land == Land.Underworld ){ GateLocation = Utility.RandomMinMax( 57, 64 ); }
				else GateLocation = Utility.RandomMinMax( 15, 26 );

				var result = GetGateLocation(GateLocation);
				if ( result != null )
				{
					if (!MySettings.S_PortalExits ) return;

					var loc = result.Item2;
					var map = result.Item1;
					if ( WorldUtilities.HasNearbyItem<Strange_Portal>( map, loc, 0, item => true ) )
						continue;

					var gate = new Strange_Portal
					{
						Gate_Location_X = X,
						Gate_Location_Y = Y,
						Gate_Location_Z = Z,
						Gate_Location_M = Map,
						Hue = Hue,
						Name = Name
					};
					gate.MoveToWorld( loc, map );
					return;
				}
			}

			Console.WriteLine( "[Strange Portal] Failed to find a valid output location on {1}", Land );
		}

		private static Tuple<Map, Point3D> GetGateLocation( int portal )
		{
			switch ( portal )
			{
				case 0: return Tuple.Create(Map.Lodor, new Point3D(5773, 2804, 0)); // the Crypts of Dracula
				case 1: return Tuple.Create(Map.Lodor, new Point3D(5353, 91, 15)); // the Mind Flayer City
				case 2: return Tuple.Create(Map.Lodor, new Point3D(5789, 2558, -30)); // Dungeon Covetous
				case 3: return Tuple.Create(Map.Lodor, new Point3D(5308, 680, 0)); // Dungeon Deceit
				case 4: return Tuple.Create(Map.Lodor, new Point3D(5185, 2442, 6)); // Dungeon Despise
				case 5: return Tuple.Create(Map.Lodor, new Point3D(5321, 799, 0)); // Dungeon Destard
				case 6: return Tuple.Create(Map.Lodor, new Point3D(5869, 1443, 0)); // the City of Embers
				case 7: return Tuple.Create(Map.Lodor, new Point3D(6038, 200, 22)); // Dungeon Hythloth
				case 8: return Tuple.Create(Map.Lodor, new Point3D(5728, 155, 1)); // the Frozen Hells
				case 9: return Tuple.Create(Map.Lodor, new Point3D(5783, 23, 0)); // Dungeon Shame
				case 10: return Tuple.Create(Map.Lodor, new Point3D(5174, 1703, 2)); // Terathan Keep
				case 11: return Tuple.Create(Map.Lodor, new Point3D(5247, 436, 0)); // the Halls of Undermountain
				case 12: return Tuple.Create(Map.Lodor, new Point3D(5859, 3427, 0)); // the Volcanic Cave
				case 13: return Tuple.Create(Map.Lodor, new Point3D(5443, 1398, 0)); // Dungeon Wrong
				case 14: return Tuple.Create(Map.Lodor, new Point3D(6035, 2574, 0)); // Stonegate Castle

				case 15: return Tuple.Create(Map.Sosaria, new Point3D(5854, 1756, 0)); // the Caverns of Poseidon
				case 16: return Tuple.Create(Map.Sosaria, new Point3D(5354, 923, 0)); // the Ancient Pyramid
				case 17: return Tuple.Create(Map.Sosaria, new Point3D(5965, 636, 0)); // Dungeon Exodus
				case 18: return Tuple.Create(Map.Sosaria, new Point3D(262, 3380, 0)); // the Cave of Banished Mages
				case 19: return Tuple.Create(Map.Sosaria, new Point3D(5981, 2154, 0)); // Dungeon Clues
				case 20: return Tuple.Create(Map.Sosaria, new Point3D(5550, 393, 0)); // Dardin's Pit
				case 21: return Tuple.Create(Map.Sosaria, new Point3D(5259, 262, 0)); // Dungeon Doom
				case 22: return Tuple.Create(Map.Sosaria, new Point3D(5526, 1228, 0)); // the Fires of Hell
				case 23: return Tuple.Create(Map.Sosaria, new Point3D(5587, 1602, 0)); // the Mines of Morinia
				case 24: return Tuple.Create(Map.Sosaria, new Point3D(5995, 423, 0)); // the Perinian Depths
				case 25: return Tuple.Create(Map.Sosaria, new Point3D(5638, 821, 0)); // the Dungeon of Time Awaits
				case 26: return Tuple.Create(Map.SavagedEmpire, new Point3D(100, 3389, 0)); // Forgotten Halls

				case 27: return Tuple.Create(Map.SerpentIsland, new Point3D(1955, 523, 0)); // the Ancient Prison
				case 28: return Tuple.Create(Map.SerpentIsland, new Point3D(2090, 863, 0)); // the Cave of Fire
				case 29: return Tuple.Create(Map.SerpentIsland, new Point3D(2440, 53, 2)); // the Cave of Souls
				case 30: return Tuple.Create(Map.SerpentIsland, new Point3D(2032, 76, 0)); // Dungeon Ankh
				case 31: return Tuple.Create(Map.SerpentIsland, new Point3D(1947, 216, 0)); // Dungeon Bane
				case 32: return Tuple.Create(Map.SerpentIsland, new Point3D(2189, 425, 0)); // Dungeon Hate
				case 33: return Tuple.Create(Map.SerpentIsland, new Point3D(2221, 816, 0)); // Dungeon Scorn
				case 34: return Tuple.Create(Map.SerpentIsland, new Point3D(1957, 710, 0)); // Dungeon Torment
				case 35: return Tuple.Create(Map.SerpentIsland, new Point3D(2361, 403, 0)); // Dungeon Vile
				case 36: return Tuple.Create(Map.SerpentIsland, new Point3D(2160, 173, 2)); // Dungeon Wicked
				case 37: return Tuple.Create(Map.SerpentIsland, new Point3D(2311, 912, 2)); // Dungeon Wrath
				case 38: return Tuple.Create(Map.SerpentIsland, new Point3D(2459, 880, 0)); // the Flooded Temple
				case 39: return Tuple.Create(Map.SerpentIsland, new Point3D(2064, 509, 0)); // the Gargoyle Crypts
				case 40: return Tuple.Create(Map.SerpentIsland, new Point3D(2457, 506, 0)); // the Serpent Sanctum
				case 41: return Tuple.Create(Map.SerpentIsland, new Point3D(2327, 183, 2)); // the Tomb of the Fallen Wizard

				case 42: return Tuple.Create(Map.SavagedEmpire, new Point3D(729, 2635, -28)); // the Blood Temple
				case 43: return Tuple.Create(Map.SavagedEmpire, new Point3D(323, 2836, 0)); // the Ice Queen Fortress
				case 44: return Tuple.Create(Map.SavagedEmpire, new Point3D(366, 3886, 0)); // the Scurvy Reef
				case 45: return Tuple.Create(Map.Underworld, new Point3D(1968, 1363, 61)); // the Glacial Scar
				case 46: return Tuple.Create(Map.Lodor, new Point3D(6142, 3660, -20)); // the Temple of Osirus
				case 47: return Tuple.Create(Map.Lodor, new Point3D(6021, 1968, 0)); // the Sanctum of Saltmarsh

				case 48: return Tuple.Create(Map.SavagedEmpire, new Point3D(774, 1984, -28)); // the Dungeon of the Mad Archmage
				case 49: return Tuple.Create(Map.SavagedEmpire, new Point3D(51, 2619, -28)); // the Tombs
				case 50: return Tuple.Create(Map.SavagedEmpire, new Point3D(342, 2296, -1)); // the Dungeon of the Lich King
				case 51: return Tuple.Create(Map.SavagedEmpire, new Point3D(1143, 2403, -28)); // the Halls of Ogrimar
				case 52: return Tuple.Create(Map.SavagedEmpire, new Point3D(692, 2319, -27)); // Dungeon Rock
				case 53: return Tuple.Create(Map.SavagedEmpire, new Point3D(647, 3860, 39)); // the Undersea Castle
				case 54: return Tuple.Create(Map.SavagedEmpire, new Point3D(231, 3650, 25)); // the Azure Castle
				case 55: return Tuple.Create(Map.SavagedEmpire, new Point3D(436, 3311, 20)); // the Tomb of Kazibal
				case 56: return Tuple.Create(Map.SavagedEmpire, new Point3D(670, 3357, 20)); // the Catacombs of Azerok

				case 57: return Tuple.Create(Map.Underworld, new Point3D(1851, 1233, -42)); // the Stygian Abyss
				case 58: return Tuple.Create(Map.Lodor, new Point3D(6413, 2004, -40)); // the Daemon's Crag
				case 59: return Tuple.Create(Map.Lodor, new Point3D(7003, 2437, -11)); // the Zealan Tombs
				case 60: return Tuple.Create(Map.Lodor, new Point3D(6368, 968, 25)); // the Hall of the Mountain King
				case 61: return Tuple.Create(Map.Lodor, new Point3D(6826, 1123, -92)); // Morgaelin's Inferno
				case 62: return Tuple.Create(Map.Lodor, new Point3D(5950, 1654, -5)); // the Depths of Carthax Lake
				case 63: return Tuple.Create(Map.Lodor, new Point3D(5989, 484, 1)); // Argentrock Castle
				case 64: return Tuple.Create(Map.Lodor, new Point3D(1125, 3684, 0)); // the Ancient Sky Ship
			}

			return null;
		}

		public static void UseGate( Mobile m, int portal, Item gate )
		{
			var result = GetGateLocation(portal);
			if ( result == null ) return;

			var map = result.Item1;
			var loc = result.Item2;

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

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
            writer.Write( GateLocation );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            GateLocation = reader.ReadInt();
		}
	}

	public class Strange_Portal : Item
	{
		public int GateLocation_X;
		public int GateLocation_Y;
		public int GateLocation_Z;
		public Map GateLocation_M;

		[CommandProperty(AccessLevel.Owner)]
		public int Gate_Location_X { get { return GateLocation_X; } set { GateLocation_X = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Gate_Location_Y { get { return GateLocation_Y; } set { GateLocation_Y = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Gate_Location_Z { get { return GateLocation_Z; } set { GateLocation_Z = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public Map Gate_Location_M { get { return GateLocation_M; } set { GateLocation_M = value; InvalidateProperties(); } }

		[Constructable]
		public Strange_Portal() : base(0x3D5E)
		{
			Movable = false;
			Light = LightType.Circle300;
			Name = "portal";
		}

		public Strange_Portal(Serial serial) : base(serial)
		{
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( m is PlayerMobile )
			{
				UseGate( m );
				Effects.PlaySound( m.Location, m.Map, 0x1FC );
			}

			return false;
		}

		public void UseGate( Mobile m )
		{
			Point3D loc = new Point3D(GateLocation_X, GateLocation_Y, GateLocation_Z);
			Map map = GateLocation_M;

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

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
            writer.Write( GateLocation_X );
            writer.Write( GateLocation_Y );
            writer.Write( GateLocation_Z );
            writer.Write( GateLocation_M );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            GateLocation_X = reader.ReadInt();
            GateLocation_Y = reader.ReadInt();
            GateLocation_Z = reader.ReadInt();
            GateLocation_M = reader.ReadMap();
		}
	}
}