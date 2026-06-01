using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Knives.TownHouses;

namespace Server.Engines.Instancing
{
	// The sky-dwelling system, expressed as a persistent-owned InstanceType.
	//
	// Every pool map shares SerpentIsland's mapID/fileIndex, so the client already
	// has the terrain and renders it with no extra .mul files. The building shell
	// (floors/walls) is baked into statics3.mul, so each cloned map already shows
	// the full structure -- only the decorative doors (placed via the original
	// sky_home.cfg) and the purchase sign are materialised per instance. A
	// dwelling's own decorations are real, persistent Items that park/restore with
	// it (see InstanceType).
	//
	// Sky-specific policy layered on the generic core:
	//   * Persistent: dwellings are saved and parked (not freed) when idle;
	//   * a "purchased" gate: an unpurchased dwelling is just a look-around visit
	//     and is freed when it goes idle, never saved (ShouldPersistOnIdle);
	//   * the legacy save path, so existing player dwellings still load.
	public class SkyDwellingInstanceType : InstanceType
	{
		public static readonly SkyDwellingInstanceType Instance = new SkyDwellingInstanceType();

		private SkyDwellingInstanceType()
		{
		}

		// ----- Pool / terrain configuration -----
		// Map.Maps[] indices 0-31 and 0x7F/0xFF are reserved; the pool sits well
		// clear of those. Sky owns 40-71.
		public override string Key { get { return "SkyDwelling"; } }
		public override int PoolBaseIndex { get { return 40; } }
		public override int PoolSize { get { return 32; } }

		public override Map BaseMap { get { return Map.SerpentIsland; } }
		public override int MapWidth { get { return 2560; } }
		public override int MapHeight { get { return 2048; } }

		// The arrival point of the "climb" rope into the first sky home; the building
		// floor there is baked into the shared static map.
		public const int DwellingX = 1974;
		public const int DwellingY = 1977;
		public const int DwellingZ = 0;
		public override Point3D Landing { get { return new Point3D( DwellingX, DwellingY, DwellingZ ); } }

		public override Map ExitMap { get { return Map.Sosaria; } }
		public override Point3D ExitPoint { get { return new Point3D( 3884, 2879, 0 ); } }

		public override string RegionName { get { return "Sky Dwelling"; } }

		public override bool Persistent { get { return true; } }

		// Keep existing player data: the original square-pool format was abandoned,
		// but the per-player mapping has always lived here.
		public override string SavePath { get { return "Saves/Instancing/SkyDwellings.bin"; } }

		// ----- Admin-tunable knobs -----

		private TimeSpan m_UnloadAfter = TimeSpan.FromMinutes( 15 );
		public override TimeSpan UnloadAfter { get { return m_UnloadAfter; } }
		public void SetUnloadAfter( TimeSpan ts ) { m_UnloadAfter = ts; }

		// Price of the auto-placed purchase sign inside a fresh dwelling. Set to
		// match the existing sky-home TownHouseSign; tunable at runtime.
		public int DwellingPrice = 100000;

		// ----- Purchase / ownership semantics -----

		// True only once the player has actually purchased their dwelling.
		public bool OwnsDwelling( Mobile m )
		{
			Instance inst = GetByOwner( m );
			return inst != null && inst.Purchased;
		}

		// Mark a dwelling purchased. Called by the in-instance SkyDwellingSign after
		// the buyer has been charged. Returns false if they already own it.
		public bool Purchase( Mobile owner )
		{
			if ( owner == null ) return false;

			Instance inst = GetOrCreate( owner );
			if ( inst.Purchased ) return false;

			inst.Purchased = true;
			inst.Touch();
			return true;
		}

		// An unpurchased dwelling is a transient look-around visit: free it when idle
		// rather than parking/saving it.
		protected override bool ShouldPersistOnIdle( Instance inst )
		{
			return inst.Purchased;
		}

		// ----- Friends / visiting -----

		public bool AddFriend( Mobile owner, Mobile friend )
		{
			return AddMember( owner, friend );
		}

		public bool RemoveFriend( Mobile owner, Mobile friend )
		{
			return RemoveMember( owner, friend );
		}

		public bool VisitFriendDwelling( Mobile from, Mobile owner )
		{
			if ( from == null || owner == null ) return false;

			Instance inst = GetByOwner( owner );
			if ( inst == null || !inst.Purchased )
			{
				from.SendMessage( "{0} has no sky dwelling.", owner.Name );
				return false;
			}
			if ( !IsMember( inst, from ) && from.AccessLevel < AccessLevel.GameMaster )
			{
				from.SendMessage( "{0} has not invited you to their sky dwelling.", owner.Name );
				return false;
			}

			if ( SendToInstance( from, inst ) )
			{
				from.SendMessage( "You arrive in {0}'s sky dwelling.", owner.Name );
				return true;
			}
			return false;
		}

		// ----- Contents -----

		// Footprint of the first sky dwelling on the real SerpentIsland -- the area we
		// copy doors / the sign from. Bounded to that dwelling (neighbours start near
		// x2220), so we don't drag in adjacent homes.
		private static readonly Rectangle2D FirstDwellingBounds = new Rectangle2D( 1950, 1910, 72, 92 );

		protected override void BuildContents( Instance inst, Map map )
		{
			// The building shell is baked into the shared static map; here we
			// replicate the original dwelling's doors and place the purchase/house
			// sign where the real one hangs. Everything created is tracked as an
			// instance item, so it parks/restores with the dwelling.
			Point3D signLoc = new Point3D( DwellingX, DwellingY - 1, DwellingZ ); // entrance fallback
			CloneOriginalStructure( inst, map, ref signLoc );

			if ( !inst.Purchased )
			{
				SkyDwellingSign sign = new SkyDwellingSign();
				sign.Price = DwellingPrice;
				sign.MoveToWorld( signLoc, map );
				inst.Items.Add( new InstanceItem( sign, signLoc ) );
			}
		}

		// Copy the original first sky dwelling's doors (and discover its sign spot)
		// from the real SerpentIsland into the instance, at identical coordinates.
		private void CloneOriginalStructure( Instance inst, Map map, ref Point3D signLoc )
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
						inst.Items.Add( new InstanceItem( nd, it.Location ) );
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
	}
}
