using System;
using System.Collections.Generic;
using Server;

namespace Server.Engines.Instancing
{
	// Central registry and driver for every instanced-map system. It does not know
	// about sky dwellings, dungeons, or any specific type -- it just holds the list
	// of registered InstanceTypes and fans the shared, engine-level concerns out to
	// each of them:
	//
	//   * pool-map registration at the right point in startup (from MapDefinitions);
	//   * one idle-sweep timer ticking every type;
	//   * world save/load and login re-homing dispatched per type.
	//
	// To add a new instanced system, create an InstanceType subclass and add it to
	// EnsureRegistered below. Each type claims its own, non-overlapping Map.Maps[]
	// index range; RegisterTypes guards against accidental overlap.
	public static class InstanceManager
	{
		private static readonly List<InstanceType> _types = new List<InstanceType>();
		private static bool _registered;
		private static Timer _sweepTimer;

		// Sweep cadence is shared across all types; each type decides its own
		// UnloadAfter threshold.
		public static readonly TimeSpan SweepInterval = TimeSpan.FromMinutes( 1 );

		public static IEnumerable<InstanceType> Types { get { return _types; } }

		// The known instance systems. Constructed here (rather than relying on each
		// type's own Configure() running first) so pool-map registration is safe
		// regardless of Configure() ordering.
		private static void EnsureRegistered()
		{
			if ( _registered )
				return;

			_registered = true;

			Register( SkyDwellingInstanceType.Instance );
			Register( DungeonInstanceType.Instance );

			ValidateRanges();
		}

		private static void Register( InstanceType type )
		{
			if ( type != null && !_types.Contains( type ) )
				_types.Add( type );
		}

		// Catch overlapping pool ranges at startup rather than as mysterious item
		// corruption later (two types writing the same Map.Maps[] slot).
		private static void ValidateRanges()
		{
			for ( int a = 0; a < _types.Count; a++ )
			{
				InstanceType ta = _types[a];
				int aLo = ta.PoolBaseIndex, aHi = ta.PoolBaseIndex + ta.PoolSize;

				for ( int b = a + 1; b < _types.Count; b++ )
				{
					InstanceType tb = _types[b];
					int bLo = tb.PoolBaseIndex, bHi = tb.PoolBaseIndex + tb.PoolSize;

					if ( aLo < bHi && bLo < aHi )
						Console.WriteLine( "[Instancing] WARNING: pool ranges of '{0}' ({1}-{2}) and '{3}' ({4}-{5}) overlap!",
							ta.Key, aLo, aHi - 1, tb.Key, bLo, bHi - 1 );
				}
			}
		}

		// ----- Startup wiring -----

		public static void Configure()
		{
			EnsureRegistered();

			EventSink.WorldLoad += new WorldLoadEventHandler( OnWorldLoad );
			EventSink.WorldSave += new WorldSaveEventHandler( OnWorldSave );
			EventSink.Login += new LoginEventHandler( OnLogin );
		}

		// Called from MapDefinitions.Configure(), right after the core maps are
		// registered, so the pool maps exist before Region.Load() and World.Load().
		public static void RegisterAllPoolMaps()
		{
			EnsureRegistered();

			foreach ( InstanceType type in _types )
				type.RegisterMaps();
		}

		private static void OnWorldLoad()
		{
			EnsureRegistered();

			foreach ( InstanceType type in _types )
			{
				if ( type.Persistent )
				{
					type.Load();
					type.ResolveAfterLoad();
				}

				type.EnsureRegions();
				int orphans = type.ReleaseDeadOwners();

				Console.WriteLine( "[Instancing] {0} ready: {1} record(s), {2} live / {3} pool maps (idx {4}-{5}), unload after {6} min{7}.",
					type.Key, type.OwnerCount, type.LiveCount, type.PoolSize,
					type.PoolBaseIndex, type.PoolBaseIndex + type.PoolSize - 1,
					(int)type.UnloadAfter.TotalMinutes,
					orphans > 0 ? String.Format( ", freed {0} orphaned record(s)", orphans ) : "" );
			}

			EnsureSweepTimer();
		}

		private static void OnWorldSave( WorldSaveEventArgs e )
		{
			foreach ( InstanceType type in _types )
			{
				if ( !type.Persistent )
					continue;

				// Refresh saved coordinates to reflect any rearrangement done while
				// inside; the items themselves persist with the normal world save.
				type.RefreshAllLiveItems();
				type.Save();
			}
		}

		private static void OnLogin( LoginEventArgs e )
		{
			Mobile m = e.Mobile;
			if ( m == null )
				return;

			foreach ( InstanceType type in _types )
			{
				if ( type.IsPoolMap( m.Map ) )
				{
					type.OnLogin( m );
					return;
				}
			}
		}

		private static void EnsureSweepTimer()
		{
			if ( _sweepTimer != null )
				return;

			_sweepTimer = Timer.DelayCall( SweepInterval, SweepInterval, new TimerCallback( Sweep ) );
		}

		private static void Sweep()
		{
			foreach ( InstanceType type in _types )
				type.SweepUnload();
		}
	}
}
