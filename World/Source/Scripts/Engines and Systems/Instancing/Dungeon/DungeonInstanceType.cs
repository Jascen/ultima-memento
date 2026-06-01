using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.PartySystem;

namespace Server.Engines.Instancing
{
	// A scaffold instanced dungeon, demonstrating the *transient* lifecycle on the
	// same core that backs the persistent sky dwellings. Nothing here is meant to be
	// a finished dungeon -- it exists to prove the abstraction supports both
	// configurable ownership models the project asked for:
	//
	//   * Party-shared: a player in an active party keys their instance to the party
	//     leader (OwnerKey), so the whole party drops into one shared copy.
	//   * Per-player: a solo player keys to their own serial and gets a private copy.
	//
	// Because Persistent is false, instances are never saved and are freed entirely
	// when they empty out (ShouldPersistOnIdle -> false via the base). Freeing also
	// clears the spawned creatures (OnFreeInstance), so each fresh entry rebuilds a
	// clean dungeon (BuildContents).
	//
	// A real dungeon would point BaseMap at dungeon terrain and give Landing a spot
	// in it; this scaffold reuses SerpentIsland (like the sky pool) so it renders
	// with no new .mul files and lands on a known-walkable tile.
	public class DungeonInstanceType : InstanceType
	{
		public static readonly DungeonInstanceType Instance = new DungeonInstanceType();

		private DungeonInstanceType()
		{
		}

		// Dungeon owns Map.Maps[] slots 72-87, clear of the sky pool (40-71).
		public override string Key { get { return "Dungeon"; } }
		public override int PoolBaseIndex { get { return 72; } }
		public override int PoolSize { get { return 16; } }

		public override Map BaseMap { get { return Map.SerpentIsland; } }
		public override int MapWidth { get { return 2560; } }
		public override int MapHeight { get { return 2048; } }

		// Placeholder landing: a known-walkable tile on the shared terrain. Replace
		// with a real dungeon entry point when this points at dungeon terrain.
		public override Point3D Landing { get { return new Point3D( 1974, 1977, 0 ); } }

		public override Map ExitMap { get { return Map.Sosaria; } }
		public override Point3D ExitPoint { get { return new Point3D( 3884, 2879, 0 ); } }

		public override string RegionName { get { return "Dungeon Instance"; } }

		// Transient: not saved, freed when empty.
		public override bool Persistent { get { return false; } }

		// Reclaim an emptied dungeon fairly quickly.
		public override TimeSpan UnloadAfter { get { return TimeSpan.FromMinutes( 5 ); } }

		// ----- Ownership model -----

		// Party members share one instance (keyed to the leader); solo players get a
		// private one (keyed to themselves). This single override is what gives us
		// both party-shared and per-player transient instances.
		protected override Serial OwnerKey( Mobile m )
		{
			Party p = Party.Get( m );
			if ( p != null && p.Active && p.Leader != null )
				return p.Leader.Serial;

			return m.Serial;
		}

		// ----- Contents -----

		// Where this instance's spawned creatures are tracked so we can clear them
		// when the instance is freed. Keyed by the instance's owner serial, which is
		// stable for the instance's lifetime.
		private readonly Dictionary<Serial, List<Mobile>> m_Spawns = new Dictionary<Serial, List<Mobile>>();

		protected override void BuildContents( Instance inst, Map map )
		{
			List<Mobile> spawned = new List<Mobile>();

			// A scattering of weak undead near the landing -- just enough to show the
			// instance is populated independently per copy.
			Point3D c = Landing;
			SpawnAt( spawned, new Skeleton(), map, new Point3D( c.X + 2, c.Y, c.Z ) );
			SpawnAt( spawned, new Skeleton(), map, new Point3D( c.X - 2, c.Y, c.Z ) );
			SpawnAt( spawned, new Zombie(), map, new Point3D( c.X, c.Y + 2, c.Z ) );

			m_Spawns[inst.OwnerSerial] = spawned;
		}

		private static void SpawnAt( List<Mobile> track, BaseCreature bc, Map map, Point3D loc )
		{
			bc.MoveToWorld( loc, map );
			bc.Home = loc;
			bc.RangeHome = 8;
			track.Add( bc );
		}

		// Free this instance's spawned creatures along with the instance.
		protected override void OnFreeInstance( Instance inst, Map liveMap )
		{
			List<Mobile> spawned;
			if ( m_Spawns.TryGetValue( inst.OwnerSerial, out spawned ) )
			{
				for ( int i = 0; i < spawned.Count; i++ )
				{
					Mobile m = spawned[i];
					if ( m != null && !m.Deleted )
						m.Delete();
				}
				m_Spawns.Remove( inst.OwnerSerial );
			}

			// Belt-and-braces: clear any stray non-player mobiles left on the freed
			// map (e.g. summons or creatures that wandered the footprint).
			if ( liveMap != null )
			{
				int half = RegionHalfExtent;
				Rectangle2D bounds = new Rectangle2D( Landing.X - half, Landing.Y - half, half * 2, half * 2 );

				List<Mobile> strays = null;
				IPooledEnumerable eable = liveMap.GetMobilesInBounds( bounds );
				try
				{
					foreach ( Mobile m in eable )
					{
						if ( m is BaseCreature && !m.Player )
						{
							if ( strays == null ) strays = new List<Mobile>();
							strays.Add( m );
						}
					}
				}
				finally
				{
					eable.Free();
				}

				if ( strays != null )
				{
					for ( int i = 0; i < strays.Count; i++ )
						strays[i].Delete();
				}
			}
		}
	}
}
