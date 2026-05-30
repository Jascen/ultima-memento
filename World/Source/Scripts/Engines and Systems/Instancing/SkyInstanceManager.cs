using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Knives.TownHouses;

namespace Server.Engines.Instancing
{
	// True per-player map instancing for sky dwellings.
	//
	// Instead of carving squares out of a shared map, we register a pool of real
	// Map instances (see RegisterPoolMaps). Every pool map shares SerpentIsland's
	// mapID + fileIndex, so the client already has the terrain and renders it with
	// no extra .mul files -- exactly how Atlantis reuses Sosaria's files. They
	// differ only as separate item/mobile/region worlds, so two owners standing at
	// the same coordinates on different pool maps never see each other.
	//
	// The sky-home building (floors/walls) is baked into statics3.mul, so each
	// cloned map already shows the full structure -- only the decorative doors
	// (placed via Decoration/Monopoly/Serpent/sky_home.cfg) are absent. A dwelling's
	// own decorations are real, persistent Items: while the owner is inside they
	// live on an assigned pool map; when idle they are parked on Map.Internal and
	// the pool map is freed for reuse. That bounded pool (PoolSize live maps) backs
	// an unlimited number of owners.
	public static class SkyInstanceManager
	{
		// ----- Pool map configuration -----
		// Map.Maps[] is a 256-slot array; indices 0-31 and 0x7F/0xFF are reserved
		// for core use, so we register the pool well clear of those.
		public const int PoolBaseIndex = 40;
		public const int PoolSize      = 32;

		// Terrain reused by every pool map (SerpentIsland). mapID/fileIndex 3 means
		// the client renders map3 terrain it already ships with, including the baked
		// sky-home structures.
		public const int ClonedMapID    = 3;
		public const int ClonedFileIndex = 3;
		public const int MapWidth        = 2560;
		public const int MapHeight       = 2048;

		// Fixed dwelling footprint on every pool map: the arrival point of the
		// "climb" rope into the first sky home (Decoration/Monopoly/Sosaria/
		// sky_home.cfg teleports to here). Identical terrain on all pool maps means
		// this single coordinate works for everyone.
		public const int DwellingX = 1974;
		public const int DwellingY = 1977;
		public const int DwellingZ = 0;

		// Despawn (park) idle dwellings after this much inactivity. Tunable via [skydwelling unload N].
		public static TimeSpan UnloadAfter = TimeSpan.FromMinutes( 15 );
		public static readonly TimeSpan UnloadSweep = TimeSpan.FromMinutes( 1 );

		// Where players land when they leave a dwelling.
		public static Map ExitMap = Map.Sosaria;
		public static Point3D ExitPoint = new Point3D( 3884, 2879, 0 );

		// Price of the auto-placed purchase sign inside a fresh dwelling. Set this to
		// match the existing sky-home TownHouseSign; tunable at runtime via
		// [skyinstance price N.
		public static int DwellingPrice = 100000;

		private static readonly Dictionary<Serial, SkyInstance> _byOwner = new Dictionary<Serial, SkyInstance>();
		private static readonly Dictionary<int, SkyInstance> _liveMapToInstance = new Dictionary<int, SkyInstance>();
		private static readonly List<SkyInstanceRegion> _regions = new List<SkyInstanceRegion>();
		private static Timer _sweepTimer;

		public static IEnumerable<SkyInstance> AllInstances { get { return _byOwner.Values; } }
		public static int OwnerCount { get { return _byOwner.Count; } }
		public static int LiveCount { get { return _liveMapToInstance.Count; } }

		// ----- Startup -----

		public static void Configure()
		{
			EventSink.WorldLoad += new WorldLoadEventHandler( OnWorldLoad );
			EventSink.WorldSave += new WorldSaveEventHandler( OnWorldSave );
			EventSink.Login += new LoginEventHandler( OnLogin );
		}

		// A player who logged out (or was caught by a server restart) inside a
		// dwelling reappears on a now-parked, bare pool map. Re-home them: owners go
		// back into their own (freshly materialized) dwelling; anyone else is sent
		// to the exit point.
		private static void OnLogin( LoginEventArgs e )
		{
			Mobile m = e.Mobile;
			if ( m == null || !IsPoolMap( m.Map ) )
				return;

			if ( GetByOwner( m ) != null )
				SendOwnerToTheirInstance( m );
			else
			{
				BaseCreature.TeleportPets( m, ExitPoint, ExitMap, false );
				m.MoveToWorld( ExitPoint, ExitMap );
			}
		}

		// Called from MapDefinitions.Configure(), right after the core maps are
		// registered, so the pool maps exist before Region.Load() and World.Load().
		// Items serialize their map by index (Map.Maps[ReadByte()]), so these
		// indices must be registered before any item deserialization.
		public static void RegisterPoolMaps()
		{
			for ( int i = 0; i < PoolSize; i++ )
			{
				int idx = PoolBaseIndex + i;
				if ( idx >= Map.Maps.Length || Map.Maps[idx] != null )
					continue;

				Map map = new Map( ClonedMapID, idx, ClonedFileIndex, MapWidth, MapHeight, 1,
					String.Format( "SkyDwelling{0}", i ), MapRules.LodorRules | MapRules.FreeMovement );

				// Mark it an instance of SerpentIsland so land/terrain/difficulty
				// logic treats it as that map (see Map.Logical / Lands.GetLand) while
				// it stays a distinct world for isolation.
				map.BaseMap = Map.SerpentIsland;

				Map.Maps[idx] = map;
				Map.AllMaps.Add( map );
			}
		}

		private static void OnWorldLoad()
		{
			LoadData();
			ResolveAfterLoad();
			EnsureRegions();
			int orphans = ReleaseDeadOwners();
			EnsureSweepTimer();

			Console.WriteLine( "[SkyDwelling] Ready: {0} owner(s), {1} live / {2} pool maps (idx {3}-{4}), unload after {5} min{6}.",
				_byOwner.Count, _liveMapToInstance.Count, PoolSize, PoolBaseIndex, PoolBaseIndex + PoolSize - 1,
				(int)UnloadAfter.TotalMinutes,
				orphans > 0 ? String.Format( ", freed {0} orphaned dwelling(s)", orphans ) : "" );
		}

		private static void OnWorldSave( WorldSaveEventArgs e )
		{
			// Refresh each live dwelling's item list so saved coordinates reflect
			// any furniture the owner rearranged while inside. The items themselves
			// persist normally with the world save (on a pool map or Internal).
			RefreshAllLiveItems();

			SaveData();
		}

		// Single pass over the world's items, bucketing each top-level item that
		// sits on a pool map into its dwelling. Cheaper than scanning World.Items
		// once per live dwelling.
		private static void RefreshAllLiveItems()
		{
			if ( _liveMapToInstance.Count == 0 )
				return;

			foreach ( SkyInstance inst in _liveMapToInstance.Values )
				inst.Items.Clear();

			foreach ( Item it in World.Items.Values )
			{
				if ( it == null || it.Deleted || it.Parent != null || it.Map == null )
					continue;

				SkyInstance inst;
				if ( _liveMapToInstance.TryGetValue( it.Map.MapIndex, out inst ) )
					inst.Items.Add( new DwellingItem( it, it.Location ) );
			}
		}

		private static void EnsureSweepTimer()
		{
			if ( _sweepTimer != null )
				return;

			_sweepTimer = Timer.DelayCall( UnloadSweep, UnloadSweep, new TimerCallback( SweepUnload ) );
		}

		private static void EnsureRegions()
		{
			if ( _regions.Count > 0 )
				return;

			for ( int i = 0; i < PoolSize; i++ )
			{
				Map map = Map.Maps[PoolBaseIndex + i];
				if ( map == null )
					continue;

				SkyInstanceRegion region = new SkyInstanceRegion( map );
				region.Register();
				_regions.Add( region );
			}
		}

		// ----- Pool map identity -----

		public static bool IsPoolMap( Map map )
		{
			return map != null && map.MapIndex >= PoolBaseIndex && map.MapIndex < PoolBaseIndex + PoolSize;
		}

		public static Point3D GetLandingPoint( Map map )
		{
			// The same spot the "climb" rope drops players into the first sky home;
			// the building floor there is baked into the shared static map.
			return new Point3D( DwellingX, DwellingY, DwellingZ );
		}

		// ----- Lookup / allocation -----

		public static SkyInstance GetByOwner( Mobile owner )
		{
			if ( owner == null ) return null;
			SkyInstance inst;
			_byOwner.TryGetValue( owner.Serial, out inst );
			return inst;
		}

		// Make a dwelling occupy a pool map, building or restoring its decorations.
		// Returns false only if every pool map is in use by an online player.
		public static bool EnsureLive( SkyInstance inst )
		{
			if ( inst == null ) return false;
			if ( inst.IsLive ) return true;

			int idx = AcquireMapIndex();
			if ( idx < 0 ) return false;

			inst.LiveMapIndex = idx;
			_liveMapToInstance[idx] = inst;

			Map map = Map.Maps[idx];

			if ( !inst.Built )
			{
				BuildStarter( inst, map );
				inst.Built = true;
			}
			else
			{
				RestoreItems( inst, map );
			}

			return true;
		}

		private static int AcquireMapIndex()
		{
			for ( int i = 0; i < PoolSize; i++ )
			{
				int idx = PoolBaseIndex + i;
				if ( !_liveMapToInstance.ContainsKey( idx ) )
					return idx;
			}

			// Pool exhausted: evict the least-recently-used dwelling that has no
			// players inside, freeing its map.
			SkyInstance victim = null;
			foreach ( SkyInstance inst in _liveMapToInstance.Values )
			{
				if ( HasPlayersInside( inst ) )
					continue;
				if ( victim == null || inst.LastTouched < victim.LastTouched )
					victim = inst;
			}

			if ( victim == null )
				return -1; // all pool maps occupied by online players

			int freed = victim.LiveMapIndex;
			ReleaseIdle( victim );
			return freed;
		}

		// ----- Materialize / park decorations -----

		// Footprint of the first sky dwelling on the real SerpentIsland — the area we
		// copy doors / the sign from. Bounded to that dwelling (neighbours start near
		// x2220), so we don't drag in adjacent homes.
		private static readonly Rectangle2D FirstDwellingBounds = new Rectangle2D( 1950, 1910, 72, 92 );

		private static void BuildStarter( SkyInstance inst, Map map )
		{
			// The building shell is baked into the shared static map; here we
			// replicate the original dwelling's doors and place the purchase/house
			// sign where the real one hangs. Everything created is tracked as a
			// dwelling item, so it parks/restores with the instance.
			Point3D signLoc = new Point3D( DwellingX, DwellingY - 1, DwellingZ ); // entrance fallback
			CloneOriginalStructure( inst, map, ref signLoc );

			if ( !inst.Purchased )
			{
				SkyDwellingSign sign = new SkyDwellingSign();
				sign.Price = DwellingPrice;
				sign.MoveToWorld( signLoc, map );
				inst.Items.Add( new DwellingItem( sign, signLoc ) );
			}
		}

		// Copy the original first sky dwelling's doors (and discover its sign spot)
		// from the real SerpentIsland into the instance, at identical coordinates.
		private static void CloneOriginalStructure( SkyInstance inst, Map map, ref Point3D signLoc )
		{
			Map src = Map.SerpentIsland;
			if ( src == null )
				return;

			IPooledEnumerable eable = src.GetItemsInBounds( FirstDwellingBounds );
			try
			{
				foreach ( Item it in eable )
				{
					if ( it == null || it.Deleted )
						continue;

					if ( it is BaseDoor )
					{
						BaseDoor nd = CloneDoor( (BaseDoor)it );
						nd.MoveToWorld( it.Location, map );
						inst.Items.Add( new DwellingItem( nd, it.Location ) );
					}
					else if ( it is HouseSign || it is TownHouseSign )
					{
						signLoc = it.Location; // hang our sign where the real one is
					}
				}
			}
			finally
			{
				eable.Free();
			}
		}

		// Recreate a door of the same appearance/behaviour as the source. We copy the
		// graphic/sound/offset fields so any door type renders correctly, and leave it
		// unlocked so visitors can move through the template freely.
		private static BaseDoor CloneDoor( BaseDoor src )
		{
			BaseDoor d;
			if ( src is StrongWoodDoor )
				d = new StrongWoodDoor( DoorFacing.WestCW );
			else if ( src is MetalDoor )
				d = new MetalDoor( DoorFacing.WestCW );
			else
				d = new DarkWoodDoor( DoorFacing.WestCW );

			d.ItemID      = src.ItemID;
			d.ClosedID    = src.ClosedID;
			d.OpenedID    = src.OpenedID;
			d.OpenedSound = src.OpenedSound;
			d.ClosedSound = src.ClosedSound;
			d.Offset      = src.Offset;
			d.Locked      = false;
			return d;
		}

		private static void RestoreItems( SkyInstance inst, Map map )
		{
			for ( int i = 0; i < inst.Items.Count; i++ )
			{
				DwellingItem di = inst.Items[i];
				if ( di.Item != null && !di.Item.Deleted )
					di.Item.MoveToWorld( di.Loc, map );
			}
		}

		// Rebuild the dwelling's item list from whatever currently sits on its pool
		// map (top-level items only). This captures anything the owner placed while
		// inside, with current positions.
		private static void CaptureItems( SkyInstance inst, Map map )
		{
			if ( map == null )
				return;

			inst.Items.Clear();

			foreach ( Item it in World.Items.Values )
			{
				if ( it == null || it.Deleted )
					continue;
				if ( it.Map != map || it.Parent != null )
					continue; // carried/contained items belong to their holder

				inst.Items.Add( new DwellingItem( it, it.Location ) );
			}
		}

		public static void Park( SkyInstance inst )
		{
			if ( inst == null || !inst.IsLive )
				return;

			CaptureItems( inst, inst.LiveMap );

			for ( int i = 0; i < inst.Items.Count; i++ )
			{
				DwellingItem di = inst.Items[i];
				if ( di.Item != null && !di.Item.Deleted )
					di.Item.Internalize();
			}

			_liveMapToInstance.Remove( inst.LiveMapIndex );
			inst.LiveMapIndex = -1;
		}

		// What to do with an idle dwelling: a purchased one is parked (kept); an
		// unpurchased one was just a look-around visit, so it is freed entirely.
		private static void ReleaseIdle( SkyInstance inst )
		{
			if ( inst.Purchased )
				Park( inst );
			else
				FreeDwelling( inst );
		}

		private static void SweepUnload()
		{
			DateTime now = DateTime.Now;
			List<SkyInstance> toRelease = null;

			foreach ( SkyInstance inst in _liveMapToInstance.Values )
			{
				if ( HasPlayersInside( inst ) )
				{
					inst.Touch();
					continue;
				}

				if ( ( now - inst.LastTouched ) >= UnloadAfter )
				{
					if ( toRelease == null ) toRelease = new List<SkyInstance>();
					toRelease.Add( inst );
				}
			}

			if ( toRelease != null )
			{
				foreach ( SkyInstance inst in toRelease )
					ReleaseIdle( inst );
			}

			ReleaseDeadOwners();
		}

		private static bool HasPlayersInside( SkyInstance inst )
		{
			if ( !inst.IsLive )
				return false;

			Map map = inst.LiveMap;
			foreach ( NetState ns in NetState.Instances )
			{
				Mobile m = ns.Mobile;
				if ( m != null && m.Map == map )
					return true;
			}
			return false;
		}

		// ----- Orphan cleanup -----

		public static int ReleaseDeadOwners()
		{
			List<SkyInstance> toRelease = null;
			foreach ( SkyInstance inst in _byOwner.Values )
			{
				if ( World.FindMobile( inst.OwnerSerial ) == null )
				{
					if ( toRelease == null ) toRelease = new List<SkyInstance>();
					toRelease.Add( inst );
				}
			}

			if ( toRelease == null ) return 0;

			foreach ( SkyInstance inst in toRelease )
				FreeDwelling( inst );

			return toRelease.Count;
		}

		// Permanently remove a dwelling and delete its decorations.
		public static void FreeDwelling( SkyInstance inst )
		{
			if ( inst.IsLive )
			{
				_liveMapToInstance.Remove( inst.LiveMapIndex );
				inst.LiveMapIndex = -1;
			}

			for ( int i = 0; i < inst.Items.Count; i++ )
			{
				DwellingItem di = inst.Items[i];
				if ( di.Item != null && !di.Item.Deleted )
					di.Item.Delete();
			}
			inst.Items.Clear();

			_byOwner.Remove( inst.OwnerSerial );

			Console.WriteLine( "[SkyDwelling] Freed dwelling (owner serial 0x{0:X} no longer exists).", (int)inst.OwnerSerial );
		}

		// ----- Teleport entry points -----

		// True only once the player has actually purchased their dwelling.
		public static bool OwnsDwelling( Mobile m )
		{
			SkyInstance inst = GetByOwner( m );
			return inst != null && inst.Purchased;
		}

		private static SkyInstance GetOrCreate( Mobile owner )
		{
			if ( owner == null ) return null;

			SkyInstance inst = GetByOwner( owner );
			if ( inst != null ) return inst;

			inst = new SkyInstance();
			inst.OwnerSerial = owner.Serial;
			_byOwner[owner.Serial] = inst;
			return inst;
		}

		// Mark a dwelling purchased. Called by the in-instance SkyDwellingSign after
		// the buyer has been charged. Returns false if they already own it.
		public static bool Purchase( Mobile owner )
		{
			if ( owner == null ) return false;

			SkyInstance inst = GetOrCreate( owner );
			if ( inst.Purchased ) return false;

			inst.Purchased = true;
			inst.Touch();
			return true;
		}

		// Send a player into their own instance. Entry is ungated: the instance is
		// generated on demand, and a purchase sign is placed inside (see
		// BuildStarter) so the player can claim it. Unpurchased instances are
		// transient and cleaned up when idle.
		public static void SendOwnerToTheirInstance( Mobile owner )
		{
			if ( owner == null ) return;

			SkyInstance inst = GetOrCreate( owner );

			if ( !EnsureLive( inst ) )
			{
				owner.SendMessage( "All sky dwellings are currently in use; please try again shortly." );
				return;
			}

			inst.Touch();
			Map map = inst.LiveMap;
			Point3D landing = GetLandingPoint( map );

			BaseCreature.TeleportPets( owner, landing, map, false );
			owner.MoveToWorld( landing, map );
		}

		public static bool VisitFriendDwelling( Mobile from, Mobile owner )
		{
			if ( from == null || owner == null ) return false;

			SkyInstance inst = GetByOwner( owner );
			if ( inst == null || !inst.Purchased )
			{
				from.SendMessage( "{0} has no sky dwelling.", owner.Name );
				return false;
			}
			if ( !IsFriend( inst, from ) && from.AccessLevel < AccessLevel.GameMaster )
			{
				from.SendMessage( "{0} has not invited you to their sky dwelling.", owner.Name );
				return false;
			}

			if ( !EnsureLive( inst ) )
			{
				from.SendMessage( "All sky dwellings are currently in use; please try again shortly." );
				return false;
			}

			inst.Touch();
			Map map = inst.LiveMap;
			Point3D landing = GetLandingPoint( map );

			BaseCreature.TeleportPets( from, landing, map, false );
			from.MoveToWorld( landing, map );
			from.SendMessage( "You arrive in {0}'s sky dwelling.", owner.Name );
			return true;
		}

		public static bool LeaveDwelling( Mobile m )
		{
			if ( m == null || !IsPoolMap( m.Map ) )
				return false;

			BaseCreature.TeleportPets( m, ExitPoint, ExitMap, false );
			m.MoveToWorld( ExitPoint, ExitMap );
			return true;
		}

		// ----- Region hook -----

		public static void OnPlayerEnteredMap( int mapIndex, PlayerMobile pm )
		{
			SkyInstance inst;
			if ( _liveMapToInstance.TryGetValue( mapIndex, out inst ) )
				inst.Touch();
		}

		// ----- Friends -----

		public static bool IsFriend( SkyInstance inst, Mobile m )
		{
			if ( inst == null || m == null ) return false;
			if ( inst.OwnerSerial == m.Serial ) return true;
			for ( int i = 0; i < inst.Friends.Count; i++ )
				if ( inst.Friends[i] == m.Serial ) return true;
			return false;
		}

		public static bool AddFriend( Mobile owner, Mobile friend )
		{
			if ( owner == null || friend == null || owner == friend ) return false;

			SkyInstance inst = GetByOwner( owner );
			if ( inst == null ) return false; // must own a dwelling before inviting friends

			for ( int i = 0; i < inst.Friends.Count; i++ )
				if ( inst.Friends[i] == friend.Serial ) return false;

			inst.Friends.Add( friend.Serial );
			return true;
		}

		public static bool RemoveFriend( Mobile owner, Mobile friend )
		{
			if ( owner == null || friend == null ) return false;
			SkyInstance inst = GetByOwner( owner );
			if ( inst == null ) return false;

			for ( int i = 0; i < inst.Friends.Count; i++ )
			{
				if ( inst.Friends[i] == friend.Serial )
				{
					inst.Friends.RemoveAt( i );
					return true;
				}
			}
			return false;
		}

		// ----- Persistence -----
		// New format / new path: the old square-pool save (SkyInstances.bin) is
		// deliberately abandoned. Decoration Items persist via the normal world
		// save; here we only persist the owner->dwelling mapping plus each item's
		// dwelling-local coordinate so we can restore it onto a pool map on demand.

		private const string SavePath = "Saves/Instancing/SkyDwellings.bin";

		private static void SaveData()
		{
			Persistence.Serialize(
				SavePath,
				delegate( GenericWriter writer )
				{
					writer.Write( (int) 1 ); // version

					writer.Write( (int) _byOwner.Count );
					foreach ( SkyInstance inst in _byOwner.Values )
					{
						writer.Write( (int) inst.OwnerSerial );
						writer.Write( (DateTime) inst.LastTouched );
						writer.Write( (bool) inst.Built );
						writer.Write( (bool) inst.Purchased );
						writer.Write( (int) inst.LiveMapIndex );

						writer.Write( (int) inst.Friends.Count );
						for ( int i = 0; i < inst.Friends.Count; i++ )
							writer.Write( (int) inst.Friends[i] );

						writer.Write( (int) inst.Items.Count );
						for ( int i = 0; i < inst.Items.Count; i++ )
						{
							writer.Write( (Item) inst.Items[i].Item );
							writer.Write( (Point3D) inst.Items[i].Loc );
						}
					}
				} );
		}

		private static void LoadData()
		{
			Persistence.Deserialize(
				SavePath,
				delegate( GenericReader reader )
				{
					if ( reader.End() )
						return;

					int version = reader.ReadInt();
					int count = reader.ReadInt();

					for ( int i = 0; i < count; i++ )
					{
						SkyInstance inst = new SkyInstance();
						inst.OwnerSerial = (Serial)reader.ReadInt();
						inst.LastTouched = reader.ReadDateTime();
						inst.Built = reader.ReadBool();
						if ( version >= 1 )
							inst.Purchased = reader.ReadBool();
						inst.LiveMapIndex = reader.ReadInt();

						int friendCount = reader.ReadInt();
						for ( int j = 0; j < friendCount; j++ )
							inst.Friends.Add( (Serial)reader.ReadInt() );

						int itemCount = reader.ReadInt();
						for ( int j = 0; j < itemCount; j++ )
						{
							Item it = reader.ReadItem();
							Point3D loc = reader.ReadPoint3D();
							if ( it != null && !it.Deleted )
								inst.Items.Add( new DwellingItem( it, loc ) );
						}

						_byOwner[inst.OwnerSerial] = inst;
					}
				} );
		}

		// After world load every dwelling starts parked. A dwelling that was live at
		// save time has its items sitting on the pool map (they deserialized there);
		// move them to Internal so we boot into a clean all-parked state. Unpurchased
		// dwellings were transient visits, so free them outright (deleting the stray
		// purchase sign and any items they had on the pool map).
		private static void ResolveAfterLoad()
		{
			List<SkyInstance> toFree = null;

			foreach ( SkyInstance inst in _byOwner.Values )
			{
				inst.Items.RemoveAll( delegate( DwellingItem di ) { return di.Item == null || di.Item.Deleted; } );

				if ( !inst.Purchased )
				{
					if ( toFree == null ) toFree = new List<SkyInstance>();
					toFree.Add( inst );
					continue;
				}

				if ( inst.LiveMapIndex >= 0 )
				{
					for ( int i = 0; i < inst.Items.Count; i++ )
					{
						DwellingItem di = inst.Items[i];
						if ( di.Item != null && !di.Item.Deleted )
							di.Item.Internalize();
					}
					inst.LiveMapIndex = -1;
				}
			}

			if ( toFree != null )
			{
				foreach ( SkyInstance inst in toFree )
					FreeDwelling( inst );
			}
		}
	}
}
