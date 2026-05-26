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
		// Pool lives in the unplayed region of SavagedEmpire (map is 1280x4096,
		// playable bounds end at y=1792 — so y=2048+ is free space).
		public static Map InstanceMap { get { return Map.SavagedEmpire; } }

		public const int PoolOriginX = 32;
		public const int PoolOriginY = 2048;
		public const int SlotWidth   = 64;
		public const int SlotHeight  = 64;
		public const int GridColumns = 16;
		public const int GridRows    = 32;
		public const int MaxSlots    = GridColumns * GridRows; // 512
		public const int LandingZ    = 10;
		public const int FloorItemId = 0x495; // wood floor tile
		public const int PlatformRadius = 5; // 11x11 walkable platform

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
			EnsureSweepTimer();
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
			SkyInstanceRegion region = new SkyInstanceRegion( inst.Id, InstanceMap, GetSlotRect( inst.Id ) );
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
			// A small permanent walkable platform so Recall's CanSpawnMobile check passes
			// even when the dwelling has not been lazy-loaded yet. The bulk of the
			// dwelling (walls, furniture, etc.) is the materialized layer above.
			Point3D landing = GetLandingPoint( inst.Id );

			if ( inst.Floor == null || inst.Floor.Deleted )
			{
				Static center = new Static( FloorItemId );
				center.MoveToWorld( landing, InstanceMap );
				inst.Floor = center;
			}

			// The platform around the center is also permanent (cheap, 121 tiles per
			// allocated slot, only allocated slots pay this cost).
			for ( int dx = -PlatformRadius; dx <= PlatformRadius; dx++ )
			{
				for ( int dy = -PlatformRadius; dy <= PlatformRadius; dy++ )
				{
					if ( dx == 0 && dy == 0 ) continue;
					Point3D p = new Point3D( landing.X + dx, landing.Y + dy, landing.Z );
					if ( PlatformTileExists( p ) ) continue;
					Static tile = new Static( FloorItemId );
					tile.MoveToWorld( p, InstanceMap );
				}
			}
		}

		private static bool PlatformTileExists( Point3D p )
		{
			IPooledEnumerable eable = InstanceMap.GetItemsInRange( p, 0 );
			try
			{
				foreach ( Item it in eable )
				{
					if ( it is Static && it.Z == p.Z ) return true;
				}
			}
			finally
			{
				eable.Free();
			}
			return false;
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

			// A simple identifying signpost near the landing. The dwelling "contents"
			// is intentionally minimal for now; this is the lazy-loaded layer that the
			// unload sweep deletes.
			Static sign = new Static( 0x1F28 ); // a glowing rune-like marker
			sign.Name = String.Format( "Sky Dwelling #{0}", inst.Id );
			sign.MoveToWorld( new Point3D( landing.X, landing.Y - 1, landing.Z ), map );
			inst.TempItems.Add( sign );

			// A couple of decorative torches at the corners.
			AddTempItem( inst, 0xA12, new Point3D( landing.X - 3, landing.Y - 3, landing.Z ), map );
			AddTempItem( inst, 0xA12, new Point3D( landing.X + 3, landing.Y - 3, landing.Z ), map );
			AddTempItem( inst, 0xA12, new Point3D( landing.X - 3, landing.Y + 3, landing.Z ), map );
			AddTempItem( inst, 0xA12, new Point3D( landing.X + 3, landing.Y + 3, landing.Z ), map );

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
		}

		private static bool HasPlayersInside( SkyInstance inst )
		{
			Map map = InstanceMap;
			Rectangle2D rect = GetSlotRect( inst.Id );

			foreach ( NetState ns in NetState.Instances )
			{
				Mobile m = ns.Mobile;
				if ( m == null ) continue;
				if ( m.Map != map ) continue;
				if ( rect.Contains( m.Location ) ) return true;
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
			int slotId = GetSlotIdFromLocation( map, loc.X, loc.Y );
			if ( slotId < 0 ) return;

			SkyInstance inst = GetById( slotId );
			if ( inst == null ) return;

			inst.Touch();
			if ( !inst.Loaded )
				Materialize( inst );
		}

		// ----- Teleport helper -----

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

		// ----- Persistence -----

		private const string SavePath = "Saves/Instancing/SkyInstances.bin";

		private static void SaveData()
		{
			Persistence.Serialize(
				SavePath,
				delegate( GenericWriter writer )
				{
					writer.Write( (int) 0 ); // version

					writer.Write( (int) _byId.Count );
					foreach ( SkyInstance inst in _byId.Values )
					{
						writer.Write( (int) inst.Id );
						writer.Write( (int) (int)inst.OwnerSerial );
						writer.Write( (Item) inst.Floor );
						writer.Write( (DateTime) inst.LastTouched );
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
						_byId[id] = inst;
						_ownerToSlot[ownerSerial] = id;
					}
				} );
		}
	}
}
