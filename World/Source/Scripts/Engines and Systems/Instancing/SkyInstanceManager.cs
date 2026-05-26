using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Instancing
{
	public static class SkyInstanceManager
	{
		// ----- Slot pool configuration -----
		// Pool sits at Z=100 above SavagedEmpire's lower half. The map's playable
		// area uses Z 0-30; our Z range (80-128) sits well above it, so our
		// SkyInstanceRegion (priority 110) wins at Z>=80 without disturbing the
		// dungeons and caves that overlap our X/Y at ground level.
		public static Map InstanceMap { get { return Map.SavagedEmpire; } }

		public const int PoolOriginX = 32;
		public const int PoolOriginY = 2048;
		public const int SlotWidth   = 64;
		public const int SlotHeight  = 64;
		public const int GridColumns = 16;
		public const int GridRows    = 32;
		public const int MaxSlots    = GridColumns * GridRows; // 512
		public const int LandingZ    = 100;
		public const int RegionZMin  = 80;
		public const int RegionZMax  = 128;   // sbyte.MaxValue + 1
		public const int RegionPriority = 110; // beats the Hedge Maze (95) and other high-priority overlays
		public const int FloorItemId = 0x495; // marble paver — walkable, used elsewhere as MarblePavers
		public const int PlatformRadius = 5;  // 11x11 walkable platform

		// Despawn temp items after this much inactivity. Tunable via [skydwelling unload N].
		public static TimeSpan UnloadAfter = TimeSpan.FromMinutes( 15 );
		public static readonly TimeSpan UnloadSweep = TimeSpan.FromMinutes( 1 );

		private static readonly Dictionary<int, SkyInstance> _byId = new Dictionary<int, SkyInstance>();
		private static readonly Dictionary<Serial, int> _ownerToSlot = new Dictionary<Serial, int>();
		private static Timer _sweepTimer;

		public static IEnumerable<SkyInstance> AllInstances { get { return _byId.Values; } }
		public static int AllocatedCount { get { return _byId.Count; } }

		public static void Configure()
		{
			EventSink.WorldLoad += new WorldLoadEventHandler( OnWorldLoad );
			EventSink.WorldSave += new WorldSaveEventHandler( OnWorldSave );
		}

		private static void OnWorldLoad()
		{
			LoadData();
			RehydrateAll();
			int orphans = ReleaseDeadOwners();
			EnsureSweepTimer();

			Console.WriteLine( "[SkyInstance] Ready: {0} allocated / {1} capacity on {2} (Z {3}-{4}, unload after {5} min){6}.",
				_byId.Count, MaxSlots, InstanceMap != null ? InstanceMap.Name : "?",
				RegionZMin, RegionZMax, (int)UnloadAfter.TotalMinutes,
				orphans > 0 ? String.Format( ", freed {0} orphaned slot(s)", orphans ) : "" );
		}

		private static void OnWorldSave( WorldSaveEventArgs e )
		{
			SaveData();
		}

		private static void EnsureSweepTimer()
		{
			if ( _sweepTimer != null )
				return;

			_sweepTimer = Timer.DelayCall( UnloadSweep, UnloadSweep, new TimerCallback( SweepUnload ) );
		}

		// ----- Slot geometry -----

		public static Rectangle2D GetSlotRect( int slotId )
		{
			int col = slotId % GridColumns;
			int row = slotId / GridColumns;
			return new Rectangle2D( PoolOriginX + col * SlotWidth, PoolOriginY + row * SlotHeight, SlotWidth, SlotHeight );
		}

		public static Point3D GetLandingPoint( int slotId )
		{
			int col = slotId % GridColumns;
			int row = slotId / GridColumns;
			return new Point3D( PoolOriginX + col * SlotWidth + SlotWidth / 2, PoolOriginY + row * SlotHeight + SlotHeight / 2, LandingZ );
		}

		public static int GetSlotIdFromLocation( Map map, int x, int y )
		{
			if ( map != InstanceMap ) return -1;
			if ( x < PoolOriginX || y < PoolOriginY ) return -1;
			int col = (x - PoolOriginX) / SlotWidth;
			int row = (y - PoolOriginY) / SlotHeight;
			if ( col < 0 || col >= GridColumns || row < 0 || row >= GridRows ) return -1;
			return row * GridColumns + col;
		}

		public static string RegionNameFor( int slotId )
		{
			return String.Format( "Sky Dwelling #{0}", slotId );
		}

		// ----- Allocation -----

		public static SkyInstance GetByOwner( Mobile owner )
		{
			if ( owner == null ) return null;
			int slotId;
			if ( !_ownerToSlot.TryGetValue( owner.Serial, out slotId ) ) return null;
			SkyInstance inst;
			_byId.TryGetValue( slotId, out inst );
			return inst;
		}

		public static SkyInstance GetById( int slotId )
		{
			SkyInstance inst;
			_byId.TryGetValue( slotId, out inst );
			return inst;
		}

		public static SkyInstance GetOrCreate( Mobile owner )
		{
			if ( owner == null ) return null;

			SkyInstance existing = GetByOwner( owner );
			if ( existing != null ) return existing;

			int slotId = AllocateFreeSlotId();
			if ( slotId < 0 )
			{
				Console.WriteLine( "[SkyInstance] Slot pool exhausted; cannot allocate for {0}.", owner.Name );
				return null;
			}

			SkyInstance inst = new SkyInstance( slotId );
			inst.OwnerSerial = owner.Serial;
			_byId[slotId] = inst;
			_ownerToSlot[owner.Serial] = slotId;

			EnsureFloor( inst );
			EnsureRegion( inst );

			return inst;
		}

		private static int AllocateFreeSlotId()
		{
			for ( int i = 0; i < MaxSlots; i++ )
			{
				if ( !_byId.ContainsKey( i ) ) return i;
			}
			return -1;
		}

		// ----- Region / floor (permanent infrastructure) -----

		private static void EnsureRegion( SkyInstance inst )
		{
			if ( inst.Region != null ) return;

			Rectangle2D xy = GetSlotRect( inst.Id );
			Rectangle3D rect3 = new Rectangle3D(
				new Point3D( xy.Start.X, xy.Start.Y, RegionZMin ),
				new Point3D( xy.End.X,   xy.End.Y,   RegionZMax )
			);

			SkyInstanceRegion region = new SkyInstanceRegion( inst.Id, InstanceMap, rect3 );
			region.RuneName = GetRuneNameFor( inst );
			region.Register();
			inst.Region = region;
		}

		public static string GetRuneNameFor( SkyInstance inst )
		{
			Mobile owner = inst.FindOwner();
			if ( owner != null && !String.IsNullOrEmpty( owner.Name ) )
				return String.Format( "the sky dwelling of {0}", owner.Name );
			return String.Format( "a sky dwelling (#{0})", inst.Id );
		}

		private static void EnsureFloor( SkyInstance inst )
		{
			// A small permanent walkable platform so Recall's CanSpawnMobile check
			// passes even when the dwelling's lazy decoration is unloaded. The bulk
			// of the dwelling (walls, furniture, etc.) is the materialized layer
			// above and gets despawned by the idle sweep.
			Point3D landing = GetLandingPoint( inst.Id );
			bool isNew = ( inst.Floor == null || inst.Floor.Deleted );
			if ( !isNew ) return; // floor + surrounding platform already in World.Items

			Static center = new Static( FloorItemId );
			center.MoveToWorld( landing, InstanceMap );
			inst.Floor = center;

			for ( int dx = -PlatformRadius; dx <= PlatformRadius; dx++ )
			{
				for ( int dy = -PlatformRadius; dy <= PlatformRadius; dy++ )
				{
					if ( dx == 0 && dy == 0 ) continue;
					Static tile = new Static( FloorItemId );
					tile.MoveToWorld( new Point3D( landing.X + dx, landing.Y + dy, landing.Z ), InstanceMap );
				}
			}
		}

		private static void RehydrateAll()
		{
			foreach ( SkyInstance inst in _byId.Values )
			{
				EnsureFloor( inst );
				EnsureRegion( inst );
			}
		}

		// ----- Lazy materialization / despawn -----

		public static void Materialize( SkyInstance inst )
		{
			if ( inst.Loaded ) return;

			Point3D landing = GetLandingPoint( inst.Id );
			Map map = InstanceMap;
			int x = landing.X, y = landing.Y, z = landing.Z;

			Mobile owner = inst.FindOwner();
			string ownerLabel = (owner != null && !String.IsNullOrEmpty( owner.Name )) ? owner.Name : String.Format( "#{0}", inst.Id );

			// Owner sign on the north edge.
			Static sign = new Static( 0xBD2 ); // brass house sign — same graphic the housing system uses
			sign.Name = String.Format( "{0}'s sky dwelling", ownerLabel );
			sign.MoveToWorld( new Point3D( x, y - 5, z ), map );
			inst.TempItems.Add( sign );

			// Lanterns at the four corners of the platform.
			AddTempItem( inst, 0xA22, new Point3D( x - 4, y - 4, z ), map );
			AddTempItem( inst, 0xA22, new Point3D( x + 4, y - 4, z ), map );
			AddTempItem( inst, 0xA22, new Point3D( x - 4, y + 4, z ), map );
			AddTempItem( inst, 0xA22, new Point3D( x + 4, y + 4, z ), map );

			// Bed along the west wall.
			AddTempItem( inst, 0xA7B, new Point3D( x - 4, y - 1, z ), map ); // bed head
			AddTempItem( inst, 0xA7C, new Point3D( x - 4, y,     z ), map ); // bed foot

			// Bookcase against the east wall.
			AddTempItem( inst, 0xA9D, new Point3D( x + 4, y - 2, z ), map );

			// Small table + chair south of the landing (the landing tile stays clear).
			AddTempItem( inst, 0xB7D, new Point3D( x,     y + 2, z ), map ); // oak table
			AddTempItem( inst, 0xB30, new Point3D( x + 1, y + 2, z ), map ); // chair
			AddTempItem( inst, 0xB30, new Point3D( x - 1, y + 2, z ), map ); // chair

			inst.Loaded = true;
			inst.Touch();
		}

		private static void AddTempItem( SkyInstance inst, int itemId, Point3D loc, Map map )
		{
			Static s = new Static( itemId );
			s.MoveToWorld( loc, map );
			inst.TempItems.Add( s );
		}

		public static void Despawn( SkyInstance inst )
		{
			if ( !inst.Loaded ) return;

			for ( int i = 0; i < inst.TempItems.Count; i++ )
			{
				Item item = inst.TempItems[i];
				if ( item != null && !item.Deleted )
					item.Delete();
			}
			inst.TempItems.Clear();
			inst.Loaded = false;
		}

		private static void SweepUnload()
		{
			DateTime now = DateTime.Now;
			List<SkyInstance> toUnload = null;

			foreach ( SkyInstance inst in _byId.Values )
			{
				if ( !inst.Loaded ) continue;

				if ( HasPlayersInside( inst ) )
				{
					inst.Touch();
					continue;
				}

				if ( (now - inst.LastTouched) >= UnloadAfter )
				{
					if ( toUnload == null ) toUnload = new List<SkyInstance>();
					toUnload.Add( inst );
				}
			}

			if ( toUnload != null )
			{
				foreach ( SkyInstance inst in toUnload )
					Despawn( inst );
			}

			ReleaseDeadOwners();
		}

		// ----- Orphaned slot cleanup -----

		public static int ReleaseDeadOwners()
		{
			List<SkyInstance> toRelease = null;
			foreach ( SkyInstance inst in _byId.Values )
			{
				if ( World.FindMobile( inst.OwnerSerial ) == null )
				{
					if ( toRelease == null ) toRelease = new List<SkyInstance>();
					toRelease.Add( inst );
				}
			}

			if ( toRelease == null ) return 0;

			foreach ( SkyInstance inst in toRelease )
				FreeSlot( inst );

			return toRelease.Count;
		}

		public static void FreeSlot( SkyInstance inst )
		{
			Despawn( inst );

			// Delete all permanent floor tiles in the slot rect at our Z.
			Rectangle2D xy = GetSlotRect( inst.Id );
			List<Item> toDelete = new List<Item>();
			IPooledEnumerable eable = InstanceMap.GetItemsInBounds( xy );
			try
			{
				foreach ( Item it in eable )
				{
					if ( it is Static && it.ItemID == FloorItemId && it.Z == LandingZ )
						toDelete.Add( it );
				}
			}
			finally
			{
				eable.Free();
			}
			for ( int i = 0; i < toDelete.Count; i++ )
				toDelete[i].Delete();

			if ( inst.Region != null )
			{
				inst.Region.Unregister();
				inst.Region = null;
			}

			_ownerToSlot.Remove( inst.OwnerSerial );
			_byId.Remove( inst.Id );

			Console.WriteLine( "[SkyInstance] Freed slot {0} (owner serial 0x{1:X} no longer exists).", inst.Id, (int)inst.OwnerSerial );
		}

		// ----- Friend management -----

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
			if ( owner == null || friend == null ) return false;
			if ( owner == friend ) return false;

			SkyInstance inst = GetOrCreate( owner );
			if ( inst == null ) return false;

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

		public static bool VisitFriendDwelling( Mobile from, Mobile owner )
		{
			if ( from == null || owner == null ) return false;
			SkyInstance inst = GetByOwner( owner );
			if ( inst == null )
			{
				from.SendMessage( "{0} has no sky dwelling.", owner.Name );
				return false;
			}
			if ( !IsFriend( inst, from ) && from.AccessLevel < AccessLevel.GameMaster )
			{
				from.SendMessage( "{0} has not invited you to their sky dwelling.", owner.Name );
				return false;
			}

			Point3D landing = GetLandingPoint( inst.Id );
			Server.Mobiles.BaseCreature.TeleportPets( from, landing, InstanceMap, false );
			from.MoveToWorld( landing, InstanceMap );
			from.SendMessage( "You arrive in {0}'s sky dwelling.", owner.Name );
			return true;
		}

		private static bool HasPlayersInside( SkyInstance inst )
		{
			foreach ( NetState ns in NetState.Instances )
			{
				Mobile m = ns.Mobile;
				if ( m == null ) continue;
				SkyInstanceRegion sir = m.Region as SkyInstanceRegion;
				if ( sir != null && sir.InstanceId == inst.Id ) return true;
			}
			return false;
		}

		// ----- Region event hooks -----

		public static void OnPlayerEntered( int slotId, PlayerMobile pm )
		{
			SkyInstance inst;
			if ( !_byId.TryGetValue( slotId, out inst ) ) return;

			inst.Touch();
			if ( !inst.Loaded )
				Materialize( inst );
		}

		public static void OnPlayerExited( int slotId, PlayerMobile pm )
		{
			SkyInstance inst;
			if ( !_byId.TryGetValue( slotId, out inst ) ) return;

			inst.Touch(); // reset clock when last player leaves; sweep handles eviction
		}

		// ----- Pre-arrival materialization hook -----

		public static void PrepareForArrival( Map map, Point3D loc )
		{
			if ( map != InstanceMap ) return;
			if ( loc.Z < RegionZMin || loc.Z >= RegionZMax ) return; // not in our Z layer
			int slotId = GetSlotIdFromLocation( map, loc.X, loc.Y );
			if ( slotId < 0 ) return;

			SkyInstance inst = GetById( slotId );
			if ( inst == null ) return;

			inst.Touch();
			if ( !inst.Loaded )
				Materialize( inst );
		}

		// ----- Teleport helpers -----

		// Default ground-level fallback when leaving a dwelling: same landing the
		// existing shared Sky Home uses on Sosaria.
		public static Map ExitMap = Map.Sosaria;
		public static Point3D ExitPoint = new Point3D( 3884, 2879, 0 );

		public static void SendOwnerToTheirInstance( Mobile owner )
		{
			if ( owner == null ) return;

			SkyInstance inst = GetOrCreate( owner );
			if ( inst == null )
			{
				owner.SendMessage( "There are no sky dwellings available." );
				return;
			}

			if ( !inst.Loaded )
				Materialize( inst );

			Point3D landing = GetLandingPoint( inst.Id );
			Server.Mobiles.BaseCreature.TeleportPets( owner, landing, InstanceMap, false );
			owner.MoveToWorld( landing, InstanceMap );
		}

		public static bool LeaveDwelling( Mobile m )
		{
			if ( m == null ) return false;
			if ( !( m.Region is SkyInstanceRegion ) ) return false;

			Server.Mobiles.BaseCreature.TeleportPets( m, ExitPoint, ExitMap, false );
			m.MoveToWorld( ExitPoint, ExitMap );
			return true;
		}

		// ----- Persistence -----

		private const string SavePath = "Saves/Instancing/SkyInstances.bin";

		private static void SaveData()
		{
			Persistence.Serialize(
				SavePath,
				delegate( GenericWriter writer )
				{
					writer.Write( (int) 2 ); // version

					writer.Write( (int) _byId.Count );
					foreach ( SkyInstance inst in _byId.Values )
					{
						writer.Write( (int) inst.Id );
						writer.Write( (int) inst.OwnerSerial );
						writer.Write( (Item) inst.Floor );
						writer.Write( (DateTime) inst.LastTouched );
						writer.Write( (bool) inst.Loaded );
						writer.Write( (int) inst.TempItems.Count );
						for ( int i = 0; i < inst.TempItems.Count; i++ )
							writer.Write( (Item) inst.TempItems[i] );
						writer.Write( (int) inst.Friends.Count );
						for ( int i = 0; i < inst.Friends.Count; i++ )
							writer.Write( (int) inst.Friends[i] );
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
						int id = reader.ReadInt();
						Serial ownerSerial = (Serial)reader.ReadInt();
						Item floor = reader.ReadItem();
						DateTime touched = reader.ReadDateTime();

						SkyInstance inst = new SkyInstance( id );
						inst.OwnerSerial = ownerSerial;
						inst.Floor = floor;
						inst.LastTouched = touched;

						if ( version >= 1 )
						{
							bool loaded = reader.ReadBool();
							int itemCount = reader.ReadInt();
							for ( int j = 0; j < itemCount; j++ )
							{
								Item it = reader.ReadItem();
								if ( it != null && !it.Deleted )
									inst.TempItems.Add( it );
							}
							inst.Loaded = loaded && inst.TempItems.Count > 0;
						}

						if ( version >= 2 )
						{
							int friendCount = reader.ReadInt();
							for ( int j = 0; j < friendCount; j++ )
								inst.Friends.Add( (Serial)reader.ReadInt() );
						}

						_byId[id] = inst;
						_ownerToSlot[ownerSerial] = id;
					}
				} );
		}
	}
}
