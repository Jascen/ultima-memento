using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Instancing
{
	// Base class for every instanced-map system. It owns the generic machinery and
	// leaves the policy to subclasses:
	//
	//   * a bounded pool of real Map instances cloning one base map's terrain, so
	//     the client renders existing .mul terrain with no extra files (exactly how
	//     Atlantis reuses Sosaria's files);
	//   * slot allocation with LRU eviction of idle instances;
	//   * a live -> park/free lifecycle, materialising and restoring real Items;
	//   * an idle sweep, login re-homing, and (for persistent types) save/load.
	//
	// Subclasses choose the terrain, the pool size/range, what to build inside, who
	// may enter, and whether instances persist (kept and parked when idle) or are
	// transient (freed when empty). InstanceManager registers and drives them.
	public abstract class InstanceType
	{
		// ----- Pool / terrain configuration (subclass-supplied) -----

		// Stable identifier; used in console output, region names and the default
		// save path. Keep it short and file-safe.
		public abstract string Key { get; }

		// First Map.Maps[] slot owned by this type, and how many. Map.Maps[] has 256
		// slots; 0-31 and 0x7F/0xFF are reserved for core use. Each type must claim a
		// range that does not overlap any other type (see InstanceManager).
		public abstract int PoolBaseIndex { get; }
		public abstract int PoolSize { get; }

		// The real map whose terrain (and baked statics) every pool map mirrors. Pool
		// maps reuse its mapID/fileIndex so the client renders it with no new files.
		public abstract Map BaseMap { get; }

		// Dimensions of the cloned terrain. Must match BaseMap's dimensions.
		public abstract int MapWidth { get; }
		public abstract int MapHeight { get; }

		// Arrival point inside the instance (instance-local; identical on every pool
		// map because they share terrain), and where players are sent on leaving.
		public abstract Point3D Landing { get; }
		public abstract Map ExitMap { get; }
		public abstract Point3D ExitPoint { get; }

		// mapID/fileIndex default to the base map's, which is what makes the client
		// reuse existing terrain. fileIndex defaults to mapID (true for the maps we
		// clone, e.g. SerpentIsland = 3/3); override if a base map's files differ.
		public virtual int ClonedMapID { get { return BaseMap.MapID; } }
		public virtual int ClonedFileIndex { get { return ClonedMapID; } }

		public virtual MapRules Rules { get { return MapRules.LodorRules | MapRules.FreeMovement; } }
		public virtual string RegionName { get { return Key; } }

		// Despawn (park or free) idle instances after this much inactivity.
		public virtual TimeSpan UnloadAfter { get { return TimeSpan.FromMinutes( 15 ); } }

		// Persistent types are saved across restarts and parked (kept) when idle;
		// transient types are not saved and are freed entirely when they empty out.
		public virtual bool Persistent { get { return false; } }

		// A bounded box around the landing footprint rather than the whole map:
		// region registration force-creates every Sector it covers, so a full-map
		// region would allocate hundreds of thousands of sectors per pool map. The
		// instance and its visitors only ever occupy this small area.
		public virtual int RegionHalfExtent { get { return 64; } }

		public virtual Rectangle3D RegionFootprint()
		{
			return new Rectangle3D(
				new Point3D( Landing.X - RegionHalfExtent, Landing.Y - RegionHalfExtent, sbyte.MinValue ),
				new Point3D( Landing.X + RegionHalfExtent, Landing.Y + RegionHalfExtent, sbyte.MaxValue ) );
		}

		// Where the save file lives for persistent types. Defaults to a per-key file;
		// override to preserve a legacy path.
		public virtual string SavePath { get { return String.Format( "Saves/Instancing/{0}.bin", Key ); } }

		// ----- Runtime state -----

		private readonly Dictionary<Serial, Instance> _byOwner = new Dictionary<Serial, Instance>();
		private readonly Dictionary<int, Instance> _liveMapToInstance = new Dictionary<int, Instance>();
		private readonly List<InstanceRegion> _regions = new List<InstanceRegion>();

		public IEnumerable<Instance> AllInstances { get { return _byOwner.Values; } }
		public IEnumerable<Instance> LiveInstances { get { return _liveMapToInstance.Values; } }
		public int OwnerCount { get { return _byOwner.Count; } }
		public int LiveCount { get { return _liveMapToInstance.Count; } }

		// ----- Map pool -----

		// Called from MapDefinitions.Configure (via InstanceManager), before
		// Region.Load() and World.Load(). Items serialize their map by index
		// (Map.Maps[ReadByte()]), so these indices must exist before any item
		// deserialization.
		public virtual void RegisterMaps()
		{
			Map baseMap = BaseMap;
			if ( baseMap == null )
				return;

			for ( int i = 0; i < PoolSize; i++ )
			{
				int idx = PoolBaseIndex + i;
				if ( idx >= Map.Maps.Length || Map.Maps[idx] != null )
					continue;

				Map map = new Map( ClonedMapID, idx, ClonedFileIndex, MapWidth, MapHeight, 1,
					String.Format( "{0}{1}", Key, i ), Rules );

				// Record the base map this clone mirrors. Map's == / IEquatable<Map>
				// reads BaseMap, so land/terrain/difficulty logic that checks
				// ( map == Map.SerpentIsland ) treats this instance as that map, while
				// it stays a distinct world for isolation.
				map.BaseMap = baseMap;

				Map.Maps[idx] = map;
				Map.AllMaps.Add( map );
			}
		}

		public void EnsureRegions()
		{
			if ( _regions.Count > 0 )
				return;

			for ( int i = 0; i < PoolSize; i++ )
			{
				int idx = PoolBaseIndex + i;
				if ( idx >= Map.Maps.Length )
					continue;

				Map map = Map.Maps[idx];
				if ( map == null )
					continue;

				InstanceRegion region = new InstanceRegion( this, map );
				region.Register();
				_regions.Add( region );
			}
		}

		public bool IsPoolMap( Map map )
		{
			return map != null && map.MapIndex >= PoolBaseIndex && map.MapIndex < PoolBaseIndex + PoolSize;
		}

		public virtual Point3D GetLandingPoint( Map map )
		{
			return Landing;
		}

		// ----- Identity / lookup -----

		// The key under which a mobile's instance is stored. Owner-keyed by default;
		// shared (party) types override to a leader serial so members resolve to the
		// same instance.
		protected virtual Serial OwnerKey( Mobile m )
		{
			return m.Serial;
		}

		public Instance GetByOwner( Mobile owner )
		{
			if ( owner == null ) return null;
			Instance inst;
			_byOwner.TryGetValue( OwnerKey( owner ), out inst );
			return inst;
		}

		protected Instance GetByKey( Serial key )
		{
			Instance inst;
			_byOwner.TryGetValue( key, out inst );
			return inst;
		}

		public Instance GetOrCreate( Mobile owner )
		{
			if ( owner == null ) return null;

			Instance inst = GetByOwner( owner );
			if ( inst != null ) return inst;

			inst = new Instance( this );
			inst.OwnerSerial = OwnerKey( owner );
			_byOwner[inst.OwnerSerial] = inst;
			return inst;
		}

		// ----- Membership / access -----

		public bool IsMember( Instance inst, Mobile m )
		{
			if ( inst == null || m == null ) return false;
			if ( inst.OwnerSerial == OwnerKey( m ) ) return true;
			for ( int i = 0; i < inst.Members.Count; i++ )
				if ( inst.Members[i] == m.Serial ) return true;
			return false;
		}

		public bool AddMember( Mobile owner, Mobile member )
		{
			if ( owner == null || member == null || owner == member ) return false;

			Instance inst = GetByOwner( owner );
			if ( inst == null ) return false;

			for ( int i = 0; i < inst.Members.Count; i++ )
				if ( inst.Members[i] == member.Serial ) return false;

			inst.Members.Add( member.Serial );
			return true;
		}

		public bool RemoveMember( Mobile owner, Mobile member )
		{
			if ( owner == null || member == null ) return false;
			Instance inst = GetByOwner( owner );
			if ( inst == null ) return false;

			for ( int i = 0; i < inst.Members.Count; i++ )
			{
				if ( inst.Members[i] == member.Serial )
				{
					inst.Members.RemoveAt( i );
					return true;
				}
			}
			return false;
		}

		// May this mobile enter the given instance? Owner and members always may;
		// staff bypass. Subclasses tighten/loosen as needed.
		public virtual bool CanEnter( Mobile from, Instance inst )
		{
			if ( from == null || inst == null ) return false;
			if ( from.AccessLevel >= AccessLevel.GameMaster ) return true;
			return IsMember( inst, from );
		}

		// ----- Materialize / park / free -----

		// Make an instance occupy a pool map, building or restoring its contents.
		// Returns false only if every pool map is in use by an online player.
		public bool EnsureLive( Instance inst )
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
				BuildContents( inst, map );
				inst.Built = true;
			}
			else
			{
				RestoreContents( inst, map );
			}

			return true;
		}

		private int AcquireMapIndex()
		{
			for ( int i = 0; i < PoolSize; i++ )
			{
				int idx = PoolBaseIndex + i;
				if ( !_liveMapToInstance.ContainsKey( idx ) )
					return idx;
			}

			// Pool exhausted: evict the least-recently-used instance that has no
			// players inside, freeing its map.
			Instance victim = null;
			foreach ( Instance inst in _liveMapToInstance.Values )
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

		// Populate a freshly-built instance: doors and a purchase sign, monster
		// spawns, whatever the system needs. Everything created should be tracked as
		// an InstanceItem on the instance so it parks/restores (or is freed) with it.
		protected abstract void BuildContents( Instance inst, Map map );

		// Restore a previously-parked instance's contents onto a pool map. The
		// default moves the stored Items back to their saved coordinates, which is
		// what persistent types want; transient types never park, so they never hit
		// this.
		protected virtual void RestoreContents( Instance inst, Map map )
		{
			for ( int i = 0; i < inst.Items.Count; i++ )
			{
				InstanceItem di = inst.Items[i];
				if ( di.Item != null && !di.Item.Deleted )
					di.Item.MoveToWorld( di.Loc, map );
			}
		}

		// Rebuild an instance's item list from whatever currently sits on its pool
		// map (top-level items only). Captures anything placed while inside, with
		// current positions; carried/contained items belong to their holder.
		protected void CaptureItems( Instance inst, Map map )
		{
			if ( map == null )
				return;

			inst.Items.Clear();

			foreach ( Item it in World.Items.Values )
			{
				if ( it == null || it.Deleted )
					continue;
				if ( it.Map != map || it.Parent != null )
					continue;

				inst.Items.Add( new InstanceItem( it, it.Location ) );
			}
		}

		// Park (keep) a live instance: capture its items, move them to Internal, and
		// free the pool map for reuse.
		public void Park( Instance inst )
		{
			if ( inst == null || !inst.IsLive )
				return;

			CaptureItems( inst, inst.LiveMap );

			for ( int i = 0; i < inst.Items.Count; i++ )
			{
				InstanceItem di = inst.Items[i];
				if ( di.Item != null && !di.Item.Deleted )
					di.Item.Internalize();
			}

			_liveMapToInstance.Remove( inst.LiveMapIndex );
			inst.LiveMapIndex = -1;
		}

		// Permanently remove an instance and delete its contents.
		public void FreeInstance( Instance inst )
		{
			if ( inst == null )
				return;

			Map liveMap = inst.LiveMap;

			if ( inst.IsLive )
			{
				_liveMapToInstance.Remove( inst.LiveMapIndex );
				inst.LiveMapIndex = -1;
			}

			// Let the type tear down anything not tracked as an InstanceItem (e.g. a
			// dungeon's spawned creatures) while we still know which map it was on.
			OnFreeInstance( inst, liveMap );

			for ( int i = 0; i < inst.Items.Count; i++ )
			{
				InstanceItem di = inst.Items[i];
				if ( di.Item != null && !di.Item.Deleted )
					di.Item.Delete();
			}
			inst.Items.Clear();

			_byOwner.Remove( inst.OwnerSerial );
		}

		// Hook for tearing down per-instance content that is not tracked as an
		// InstanceItem -- most notably spawned mobiles. Called from FreeInstance with
		// the map the instance was last live on (null if it was already parked).
		// Default does nothing; Items are deleted by FreeInstance regardless.
		protected virtual void OnFreeInstance( Instance inst, Map liveMap )
		{
		}

		// Whether an instance should be kept (parked) or freed when it goes idle.
		// Persistent types keep by default; sky dwellings additionally drop a still
		// unpurchased (look-around) instance. Transient types always free.
		protected virtual bool ShouldPersistOnIdle( Instance inst )
		{
			return Persistent;
		}

		private void ReleaseIdle( Instance inst )
		{
			if ( ShouldPersistOnIdle( inst ) )
				Park( inst );
			else
				FreeInstance( inst );
		}

		// ----- Sweep / orphan cleanup -----

		public void SweepUnload()
		{
			DateTime now = DateTime.Now;
			List<Instance> toRelease = null;

			foreach ( Instance inst in _liveMapToInstance.Values )
			{
				if ( HasPlayersInside( inst ) )
				{
					inst.Touch();
					continue;
				}

				if ( ( now - inst.LastTouched ) >= UnloadAfter )
				{
					if ( toRelease == null ) toRelease = new List<Instance>();
					toRelease.Add( inst );
				}
			}

			if ( toRelease != null )
			{
				foreach ( Instance inst in toRelease )
					ReleaseIdle( inst );
			}

			ReleaseDeadOwners();
		}

		protected bool HasPlayersInside( Instance inst )
		{
			if ( inst == null || !inst.IsLive )
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

		// Free instances whose owner no longer exists. For party/transient types the
		// owner key may be a leader who logged out -- those are reclaimed by the
		// idle sweep instead, so only purge when the keyed mobile is truly gone.
		public int ReleaseDeadOwners()
		{
			List<Instance> toRelease = null;
			foreach ( Instance inst in _byOwner.Values )
			{
				if ( World.FindMobile( inst.OwnerSerial ) == null )
				{
					if ( toRelease == null ) toRelease = new List<Instance>();
					toRelease.Add( inst );
				}
			}

			if ( toRelease == null ) return 0;

			foreach ( Instance inst in toRelease )
				FreeInstance( inst );

			return toRelease.Count;
		}

		// ----- Entry / exit -----

		// Send a mobile into a specific instance, materialising it on demand.
		public bool SendToInstance( Mobile from, Instance inst )
		{
			if ( from == null || inst == null )
				return false;

			if ( !EnsureLive( inst ) )
			{
				from.SendMessage( "All instances are currently in use; please try again shortly." );
				return false;
			}

			inst.Touch();
			Map map = inst.LiveMap;
			Point3D landing = GetLandingPoint( map );

			BaseCreature.TeleportPets( from, landing, map, false );
			from.MoveToWorld( landing, map );
			return true;
		}

		// Send a player into their own instance (created on demand).
		public bool SendOwnerToTheirInstance( Mobile owner )
		{
			if ( owner == null ) return false;
			return SendToInstance( owner, GetOrCreate( owner ) );
		}

		public bool LeaveInstance( Mobile m )
		{
			if ( m == null || !IsPoolMap( m.Map ) )
				return false;

			BaseCreature.TeleportPets( m, ExitPoint, ExitMap, false );
			m.MoveToWorld( ExitPoint, ExitMap );
			return true;
		}

		// A player who logged out (or was caught by a restart) inside an instance
		// reappears on a now-parked, bare pool map. Re-home them: an owner goes back
		// into their (freshly materialized) instance; anyone else is sent to the exit.
		public virtual void OnLogin( Mobile m )
		{
			if ( m == null || !IsPoolMap( m.Map ) )
				return;

			Instance inst = GetByOwner( m );
			if ( inst != null )
				SendToInstance( m, inst );
			else
			{
				BaseCreature.TeleportPets( m, ExitPoint, ExitMap, false );
				m.MoveToWorld( ExitPoint, ExitMap );
			}
		}

		// Region hook: keep the live instance from being swept while occupied.
		public void OnPlayerEnteredMap( int mapIndex, PlayerMobile pm )
		{
			Instance inst;
			if ( _liveMapToInstance.TryGetValue( mapIndex, out inst ) )
				inst.Touch();
		}

		// ----- Persistence (persistent types only) -----

		// Refresh each live instance's item list so saved coordinates reflect any
		// rearrangement done while inside. The items themselves persist normally with
		// the world save (on a pool map or Internal).
		public void RefreshAllLiveItems()
		{
			if ( _liveMapToInstance.Count == 0 )
				return;

			foreach ( Instance inst in _liveMapToInstance.Values )
				inst.Items.Clear();

			foreach ( Item it in World.Items.Values )
			{
				if ( it == null || it.Deleted || it.Parent != null || it.Map == null )
					continue;

				Instance inst;
				if ( _liveMapToInstance.TryGetValue( it.Map.MapIndex, out inst ) )
					inst.Items.Add( new InstanceItem( it, it.Location ) );
			}
		}

		public void Save()
		{
			Persistence.Serialize(
				SavePath,
				delegate( GenericWriter writer )
				{
					writer.Write( (int) 2 ); // version

					writer.Write( (int) _byOwner.Count );
					foreach ( Instance inst in _byOwner.Values )
					{
						writer.Write( (int) inst.OwnerSerial );
						writer.Write( (DateTime) inst.LastTouched );
						writer.Write( (bool) inst.Built );
						writer.Write( (bool) inst.Purchased );
						writer.Write( (bool) inst.Public );
						writer.Write( (int) inst.LiveMapIndex );

						writer.Write( (int) inst.Members.Count );
						for ( int i = 0; i < inst.Members.Count; i++ )
							writer.Write( (int) inst.Members[i] );

						writer.Write( (int) inst.Items.Count );
						for ( int i = 0; i < inst.Items.Count; i++ )
						{
							writer.Write( (Item) inst.Items[i].Item );
							writer.Write( (Point3D) inst.Items[i].Loc );
						}
					}
				} );
		}

		public void Load()
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
						Instance inst = new Instance( this );
						inst.OwnerSerial = (Serial)reader.ReadInt();
						inst.LastTouched = reader.ReadDateTime();
						inst.Built = reader.ReadBool();
						if ( version >= 1 )
							inst.Purchased = reader.ReadBool();
						if ( version >= 2 )
							inst.Public = reader.ReadBool();
						inst.LiveMapIndex = reader.ReadInt();

						int memberCount = reader.ReadInt();
						for ( int j = 0; j < memberCount; j++ )
							inst.Members.Add( (Serial)reader.ReadInt() );

						int itemCount = reader.ReadInt();
						for ( int j = 0; j < itemCount; j++ )
						{
							Item it = reader.ReadItem();
							Point3D loc = reader.ReadPoint3D();
							if ( it != null && !it.Deleted )
								inst.Items.Add( new InstanceItem( it, loc ) );
						}

						_byOwner[inst.OwnerSerial] = inst;
					}
				} );
		}

		// After world load every instance starts parked. One that was live at save
		// time has its items sitting on the pool map (they deserialized there); move
		// them to Internal so we boot into a clean all-parked state. Instances that
		// should not persist when idle (e.g. an unpurchased look-around dwelling)
		// were transient, so free them outright.
		public void ResolveAfterLoad()
		{
			List<Instance> toFree = null;

			foreach ( Instance inst in _byOwner.Values )
			{
				inst.Items.RemoveAll( delegate( InstanceItem di ) { return di.Item == null || di.Item.Deleted; } );

				if ( !ShouldPersistOnIdle( inst ) )
				{
					if ( toFree == null ) toFree = new List<Instance>();
					toFree.Add( inst );
					continue;
				}

				if ( inst.LiveMapIndex >= 0 )
				{
					for ( int i = 0; i < inst.Items.Count; i++ )
					{
						InstanceItem di = inst.Items[i];
						if ( di.Item != null && !di.Item.Deleted )
							di.Item.Internalize();
					}
					inst.LiveMapIndex = -1;
				}
			}

			if ( toFree != null )
			{
				foreach ( Instance inst in toFree )
					FreeInstance( inst );
			}
		}
	}
}
