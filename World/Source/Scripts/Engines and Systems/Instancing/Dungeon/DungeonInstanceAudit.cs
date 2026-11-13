using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.Instancing
{
	// Staff tool: builds each dungeon as an instance, flood-fills it from the landing following teleporters, and
	// reports escapes and exits. DungeonAudit.json lists the tile and step reaching each teleporter and edge.
	public static class DungeonInstanceAudit
	{
		private const int MaxVisited = 400000;
		private const int SampleCount = 6;
		private static readonly Serial AuditKey = (Serial)0x7FFFFFF0;

		private static readonly Direction[] Directions = new Direction[]
		{
			Direction.North, Direction.Right, Direction.East, Direction.Down,
			Direction.South, Direction.Left, Direction.West, Direction.Up
		};

		public static void Run( Mobile probe, string filter )
		{
			List<DungeonInstanceType.DungeonInstanceDefinition> defs = DungeonInstanceType.Instance.Definitions;
			string path = Path.Combine( Core.BaseDirectory, "DungeonAudit.txt" );
			string jsonPath = Path.Combine( Core.BaseDirectory, "DungeonAudit.json" );
			int audited = 0, clean = 0;

			bool oldIgnoreDoors = Server.Movement.MovementImpl.AlwaysIgnoreDoors;
			Server.Movement.MovementImpl.AlwaysIgnoreDoors = true;

			try
			{
				using ( StreamWriter writer = new StreamWriter( path ) )
				using ( StreamWriter json = new StreamWriter( jsonPath ) )
				{
					json.WriteLine( "[" );
					bool first = true;

					for ( int i = 0; i < defs.Count; i++ )
					{
						DungeonInstanceType.DungeonInstanceDefinition def = defs[i];
						if ( !Matches( def, filter ) )
							continue;

						if ( !first )
							json.WriteLine( "," );
						first = false;

						bool ok = AuditDefinition( probe, def, writer, json );
						audited++;
						if ( ok )
							clean++;
					}

					json.WriteLine( "]" );
				}
			}
			finally
			{
				Server.Movement.MovementImpl.AlwaysIgnoreDoors = oldIgnoreDoors;
			}

			probe.SendMessage( "Audited {0} dungeon(s): {1} passed (no escape, an exit reachable). Report: {2}", audited, clean, path );
		}

		private static bool Matches( DungeonInstanceType.DungeonInstanceDefinition def, string filter )
		{
			if ( filter == null || filter.Length == 0 || filter == "all" )
				return true;

			int index;
			if ( Int32.TryParse( filter, out index ) )
				return def.Index == index;

			return def.DisplayName.IndexOf( filter, StringComparison.OrdinalIgnoreCase ) >= 0;
		}

		private static bool AuditDefinition( Mobile probe, DungeonInstanceType.DungeonInstanceDefinition def, StreamWriter w, StreamWriter json )
		{
			w.WriteLine( "=== [{0}] {1} | {2} | spawns {3}", def.Index, def.DisplayName, def.AvailabilityLabel, def.SpawnCount );
			w.WriteLine( "  landing {0} in-area={1} standable={2}{3}", def.Landing, def.Contains( def.Landing ), def.Map.CanFit( def.Landing.X, def.Landing.Y, def.Landing.Z, 16, false, false, true ),
				def.WalkOutExits ? " | exits on foot" : "" );
			ReportRealExits( def, w );

			if ( def.ConnectedRegions != null )
			{
				for ( int i = 0; i < def.ConnectedRegions.Length; i++ )
				{
					BaseRegion r = def.ConnectedRegions[i];
					w.WriteLine( "  region {0} '{1}' prio {2} rects {3}", r.GetType().Name, r.Name, r.Priority, r.Area.Length );
				}
			}

			Instance inst = DungeonInstanceType.Instance.BuildAuditInstance( def, AuditKey );
			if ( inst == null || inst.LiveMap == null )
			{
				w.WriteLine( "  !! could not build instance" );
				json.Write( "{{\"index\":{0},\"name\":{1},\"error\":true}}", def.Index, Quote( def.DisplayName ) );
				return false;
			}

			Map map = inst.LiveMap;
			bool clean;

			try
			{
				clean = Walk( probe, def, map, w, json );
				ReportCopiedContent( def, map, w );
			}
			finally
			{
				DungeonInstanceType.Instance.FreeInstance( inst );
			}

			w.WriteLine();
			w.Flush();
			return clean;
		}

		private sealed class Link
		{
			public Item Item;
			public Map DestMap;
			public Point3D Dest;
		}

		private sealed class Step
		{
			public Point3D From;
			public Direction Dir;
		}

		private static bool Walk( Mobile probe, DungeonInstanceType.DungeonInstanceDefinition def, Map map, StreamWriter w, StreamWriter json )
		{
			Dictionary<long, Step> steps = new Dictionary<long, Step>();
			List<string> plan = new List<string>();
			List<string> edges = new List<string>();
			Dictionary<int, List<Link>> links = CollectLinks( map );
			Map baseMap = def.Map;

			Queue<Point3D> queue = new Queue<Point3D>();
			HashSet<long> visited = new HashSet<long>();
			HashSet<Item> reachedLinks = new HashSet<Item>();
			List<string> walkLeaks = new List<string>();
			HashSet<long> leakTiles = new HashSet<long>();
			List<string> voidTeleports = new List<string>();
			List<string> exits = new List<string>();
			List<string> realWorld = new List<string>();

			Enqueue( queue, visited, def.Landing );

			while ( queue.Count > 0 && visited.Count < MaxVisited )
			{
				Point3D p = queue.Dequeue();

				List<Link> here;
				if ( links.TryGetValue( Key2D( p.X, p.Y ), out here ) )
				{
					for ( int i = 0; i < here.Count; i++ )
					{
						Link link = here[i];
						if ( link.Item.Z < p.Z - 5 || link.Item.Z > p.Z + 15 || !reachedLinks.Add( link.Item ) )
							continue;

						plan.Add( PlanEntry( link, p, steps, map ) );

						if ( link.Item is DungeonInstanceExitTeleporter )
						{
							exits.Add( link.Item.Location.ToString() );
						}
						else if ( Object.ReferenceEquals( link.DestMap, map ) )
						{
							if ( !def.Contains( link.Dest ) )
								voidTeleports.Add( String.Format( "{0} {1} -> {2} ({3})", link.Item.GetType().Name, link.Item.Location, link.Dest, DescribeRegion( link.Dest, baseMap ) ) );

							Enqueue( queue, visited, link.Dest );
						}
						else
						{
							realWorld.Add( String.Format( "{0} {1} -> {2} {3}", link.Item.GetType().Name, link.Item.Location, link.DestMap, link.Dest ) );
						}
					}
				}

				for ( int d = 0; d < Directions.Length; d++ )
				{
					int newZ;
					if ( !Server.Movement.Movement.CheckMovement( probe, map, p, Directions[d], out newZ ) )
						continue;

					int x = p.X, y = p.Y;
					Server.Movement.Movement.Offset( Directions[d], ref x, ref y );
					Point3D next = new Point3D( x, y, newZ );

					if ( !def.Contains( next ) )
					{
						if ( leakTiles.Add( Key2D( x, y ) ) )
						{
							if ( walkLeaks.Count < SampleCount )
								walkLeaks.Add( String.Format( "{0} -> {1} ({2})", p, next, DescribeRegion( next, baseMap ) ) );

							edges.Add( String.Format( "{{\"x\":{0},\"y\":{1},\"z\":{2},\"dir\":\"{3}\"}}", p.X, p.Y, p.Z, Directions[d] ) );
						}

						continue;
					}

					if ( Enqueue( queue, visited, next ) )
					{
						Step step = new Step();
						step.From = p;
						step.Dir = Directions[d];
						steps[Key3D( next )] = step;
					}
				}
			}

			int unreached = 0;
			foreach ( List<Link> list in links.Values )
				for ( int i = 0; i < list.Count; i++ )
					if ( !reachedLinks.Contains( list[i].Item ) )
						unreached++;

			bool truncated = visited.Count >= MaxVisited;
			w.WriteLine( "  walked {0} tiles{1}; teleporters reached {2}, unreached {3}; exits reached {4}",
				visited.Count, truncated ? " (TRUNCATED)" : "", reachedLinks.Count, unreached, exits.Count );

			json.Write( "{{\"index\":{0},\"name\":{1},\"walkOut\":{2},\"landing\":[{3},{4},{5}],\"links\":[{6}],\"edges\":[{7}]}}",
				def.Index, Quote( def.DisplayName ), def.WalkOutExits ? "true" : "false", def.Landing.X, def.Landing.Y, def.Landing.Z,
				String.Join( ",", plan.ToArray() ), String.Join( ",", edges.ToArray() ) );

			WriteList( w, "EXIT", exits );
			WriteList( w, "EDGE (walkable but outside the dungeon, refused in game: " + leakTiles.Count + " tiles)", walkLeaks );
			WriteList( w, "TELEPORT-OUT (lands outside area on the clone)", voidTeleports );
			WriteList( w, "REAL-WORLD (teleports straight to a real map)", realWorld );

			return voidTeleports.Count == 0 && realWorld.Count == 0 && ( exits.Count > 0 || ( def.WalkOutExits && leakTiles.Count > 0 ) );
		}

		private static Dictionary<int, List<Link>> CollectLinks( Map map )
		{
			Dictionary<int, List<Link>> links = new Dictionary<int, List<Link>>();

			foreach ( Item item in World.Items.Values )
			{
				if ( item == null || item.Deleted || item.Parent != null || !Object.ReferenceEquals( item.Map, map ) )
					continue;

				Link link = new Link();
				link.Item = item;

				if ( !( item is DungeonInstanceExitTeleporter ) )
				{
					Map destMap;
					Point3D dest;
					if ( !DungeonInstanceType.TryGetTeleportDestination( item, out destMap, out dest ) || dest == Point3D.Zero )
						continue;

					link.DestMap = destMap == null || destMap == Map.Internal ? map : destMap;
					link.Dest = dest;
				}

				int key = Key2D( item.X, item.Y );
				List<Link> list;
				if ( !links.TryGetValue( key, out list ) )
					links[key] = list = new List<Link>();

				list.Add( link );
			}

			return links;
		}

		// Fixed items by type, real vs instance, to show anything the copy misses.
		private static void ReportCopiedContent( DungeonInstanceType.DungeonInstanceDefinition def, Map map, StreamWriter w )
		{
			Dictionary<string, int> real = new Dictionary<string, int>();
			Dictionary<string, int> copied = new Dictionary<string, int>();
			int realTotal = 0, copiedTotal = 0;

			foreach ( Item item in World.Items.Values )
			{
				if ( item == null || item.Deleted || item.Parent != null || item.Movable || !def.Contains( item.Location ) )
					continue;

				if ( Object.ReferenceEquals( item.Map, def.Map ) && item.Spawner == null && !( item is AddonComponent ) )
				{
					Count( real, item.GetType().Name );
					realTotal++;
				}
				else if ( Object.ReferenceEquals( item.Map, map ) && !( item is AddonComponent ) )
				{
					Count( copied, item.GetType().Name );
					copiedTotal++;
				}
			}

			List<string> missing = new List<string>();
			foreach ( KeyValuePair<string, int> kvp in real )
			{
				int have;
				copied.TryGetValue( kvp.Key, out have );
				if ( have < kvp.Value )
					missing.Add( String.Format( "{0} {1}/{2}", kvp.Key, have, kvp.Value ) );
			}

			w.WriteLine( "  fixed items: real {0}, instance {1}{2}", realTotal, copiedTotal,
				missing.Count > 0 ? "; short: " + String.Join( ", ", missing.ToArray() ) : "" );
		}

		private static void Count( Dictionary<string, int> counts, string key )
		{
			int count;
			counts.TryGetValue( key, out count );
			counts[key] = count + 1;
		}

		// Real destinations of the teleporters the instance turns into exits.
		private static void ReportRealExits( DungeonInstanceType.DungeonInstanceDefinition def, StreamWriter w )
		{
			List<string> lines = new List<string>();

			foreach ( Item item in World.Items.Values )
			{
				if ( item == null || item.Deleted || item.Parent != null || !Object.ReferenceEquals( item.Map, def.Map ) || !def.Contains( item.Location ) )
					continue;

				Map destMap;
				Point3D dest;
				if ( !DungeonInstanceType.TryGetTeleportDestination( item, out destMap, out dest ) || dest == Point3D.Zero )
					continue;

				Map resolved = destMap == null || destMap == Map.Internal ? def.Map : destMap;
				if ( Object.ReferenceEquals( resolved, def.Map ) && def.Contains( dest ) )
					continue;

				lines.Add( String.Format( "{0} {1} -> {2} {3} ({4})", item.GetType().Name, item.Location, resolved, dest, DescribeRegion( dest, resolved ) ) );
			}

			WriteList( w, "LEADS OUT (becomes an exit)", lines );
		}

		private static string DescribeRegion( Point3D p, Map baseMap )
		{
			Region r = Region.Find( p, baseMap );
			if ( r == null || r.IsDefault )
				return "no region";

			return String.Format( "{0} '{1}'", r.GetType().Name, r.Name );
		}

		private static void WriteList( StreamWriter w, string label, List<string> items )
		{
			if ( items.Count == 0 )
				return;

			w.WriteLine( "  {0}: {1}", label, items.Count );
			for ( int i = 0; i < items.Count && i < 40; i++ )
				w.WriteLine( "    {0}", items[i] );
		}

		private static bool Enqueue( Queue<Point3D> queue, HashSet<long> visited, Point3D p )
		{
			if ( !visited.Add( Key3D( p ) ) )
				return false;

			queue.Enqueue( p );
			return true;
		}

		private static long Key3D( Point3D p )
		{
			return ( (long)p.X << 24 ) | ( (long)p.Y << 8 ) | (long)( p.Z + 128 );
		}

		private static string PlanEntry( Link link, Point3D at, Dictionary<long, Step> steps, Map map )
		{
			Item item = link.Item;
			string kind = item is DungeonInstanceExitTeleporter ? "exit" : Object.ReferenceEquals( link.DestMap, map ) ? "internal" : "realworld";

			Step step;
			string approach = steps.TryGetValue( Key3D( at ), out step )
				? String.Format( "\"ax\":{0},\"ay\":{1},\"az\":{2},\"dir\":\"{3}\"", step.From.X, step.From.Y, step.From.Z, step.Dir )
				: "\"ax\":null";

			KeywordTeleporter keyword = item as KeywordTeleporter;

			return String.Format( "{{\"kind\":\"{0}\",\"type\":\"{1}\",\"x\":{2},\"y\":{3},\"z\":{4},\"tx\":{5},\"ty\":{6},\"tz\":{7},\"dx\":{8},\"dy\":{9},\"dz\":{10},\"keyword\":{11},{12}}}",
				kind, item.GetType().Name, item.X, item.Y, item.Z, at.X, at.Y, at.Z, link.Dest.X, link.Dest.Y, link.Dest.Z,
				keyword != null ? Quote( keyword.Substring ) : "null", approach );
		}

		private static string Quote( string s )
		{
			return s == null ? "null" : "\"" + s.Replace( "\\", "\\\\" ).Replace( "\"", "\\\"" ) + "\"";
		}

		private static int Key2D( int x, int y )
		{
			return ( x << 13 ) | y;
		}
	}
}
