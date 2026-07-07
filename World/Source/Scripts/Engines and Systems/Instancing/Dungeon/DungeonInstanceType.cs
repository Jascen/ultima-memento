using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Misc;
using Server.Engines.PartySystem;
using Server.Regions;

namespace Server.Engines.Instancing
{
	// Transient, party-shared dungeon instances backed by real dungeon region data.
	// A configured entry item chooses the source dungeon and difficulty; this type
	// then assigns a clone map with matching terrain, registers a temporary copy of
	// the selected region, and clones that region's spawn entries into the instance.
	public class DungeonInstanceType : InstanceType
	{
		public static readonly DungeonInstanceType Instance = new DungeonInstanceType();

		public enum DungeonInstanceAvailability
		{
			Allowed,
			BadEntranceOrExit,
			Broken
		}

		private const int CoreBaseMapCount = 7;
		private const int ClonesPerBaseMap = 7;
		private const int RuntimeRegionPriorityOffset = 1000;
		private const int FunctionalItemMargin = 8;
		private const int TeleporterEndpointMargin = 4;
		private const int DungeonListingScanLimit = 85;
			private const string ReturnSavePath = "Saves/Instancing/DungeonReturns.bin";

			private static readonly Dictionary<int, List<DecorationTeleporterSource>> m_DecorationTeleportersByMap = new Dictionary<int, List<DecorationTeleporterSource>>();
		private static readonly string[] ExcludedListedDungeonNames = new string[]
		{
			"Stonegate Castle",
			"the Halls of Undermountain",
			"the Ice Fiend Lair"
		};

		private static readonly int[] BadEntranceOrExitDungeonIndexes = new int[]
		{
			75, 74, 76, 40, 25, 28, 30, 32, 6, 38, 36
		};

		private static readonly int[] BrokenDungeonIndexes = new int[]
		{
			81, 60, 62, 63, 64, 65, 66, 67, 70, 71, 77, 78, 79, 21, 22, 23, 26, 27, 29, 31, 39, 1, 10, 11, 13, 17, 18
		};

		private const string BadEntranceOrExitReason = "entrance location incorrect or exit tiles incorrect";
		private const string BrokenReason = "broken in general";
		public const int GoodDungeonGateHue = 0x455;
		public const int BadEntranceOrExitDungeonGateHue = 0x36;
		public const int BrokenDungeonGateHue = 0x25;
			private static readonly string[] EmptyStringArray = new string[0];

		private List<DungeonInstanceDefinition> m_Definitions;
		private readonly Dictionary<Serial, DungeonInstanceSettings> m_SettingsByOwner = new Dictionary<Serial, DungeonInstanceSettings>();
		private readonly Dictionary<int, DungeonInstanceSettings> m_SettingsByLiveMap = new Dictionary<int, DungeonInstanceSettings>();
		private readonly Dictionary<Serial, RuntimeDungeonState> m_Runtimes = new Dictionary<Serial, RuntimeDungeonState>();
		private readonly Dictionary<Serial, DungeonReturnInfo> m_ReturnsByPlayer = new Dictionary<Serial, DungeonReturnInfo>();
		private bool m_ReturnPersistenceConfigured;
		private int m_NextRuntimeSpawnId = -100000000;

		private DungeonInstanceType()
		{
		}

		public void Configure()
		{
			if ( m_ReturnPersistenceConfigured )
				return;

			m_ReturnPersistenceConfigured = true;
			EventSink.WorldLoad += new WorldLoadEventHandler( OnWorldLoad );
			EventSink.WorldSave += new WorldSaveEventHandler( OnWorldSave );
		}

			private void OnWorldLoad()
			{
				m_Definitions = null;
				m_DecorationTeleportersByMap.Clear();
				LoadReturnRecords();
			}

		private void OnWorldSave( WorldSaveEventArgs e )
		{
			SaveReturnRecords();
		}

		private void LoadReturnRecords()
		{
			m_ReturnsByPlayer.Clear();

			Persistence.Deserialize(
				ReturnSavePath,
				delegate( GenericReader reader )
				{
					if ( reader.End() )
						return;

					reader.ReadInt(); // version
					int count = reader.ReadInt();

					for ( int i = 0; i < count; i++ )
					{
						Serial player = (Serial)reader.ReadInt();
						Serial instanceKey = (Serial)reader.ReadInt();
						InstanceOwnerKind ownerKind = (InstanceOwnerKind)reader.ReadInt();
							Map returnMap = NormalizeExternalMap( reader.ReadMap() );
						Point3D returnPoint = reader.ReadPoint3D();
						DateTime lastTouched = reader.ReadDateTime();

						if ( returnMap == null )
							continue;

						m_ReturnsByPlayer[player] = new DungeonReturnInfo( instanceKey, ownerKind, returnMap, returnPoint, lastTouched );
					}
				} );

			PruneReturnRecords();
		}

		private void SaveReturnRecords()
		{
			PruneReturnRecords();

			Persistence.Serialize(
				ReturnSavePath,
				delegate( GenericWriter writer )
				{
					writer.Write( (int) 0 ); // version

					writer.Write( (int) m_ReturnsByPlayer.Count );
					foreach ( KeyValuePair<Serial, DungeonReturnInfo> kvp in m_ReturnsByPlayer )
					{
						DungeonReturnInfo info = kvp.Value;

						writer.Write( (int) kvp.Key );
						writer.Write( (int) info.InstanceKey );
						writer.Write( (int) info.OwnerKind );
						writer.Write( (Map) info.ReturnMap );
						writer.Write( (Point3D) info.ReturnPoint );
						writer.Write( (DateTime) info.LastTouched );
					}
				} );
		}

		private void PruneReturnRecords()
		{
			if ( m_ReturnsByPlayer.Count == 0 )
				return;

			List<Serial> remove = null;

			foreach ( KeyValuePair<Serial, DungeonReturnInfo> kvp in m_ReturnsByPlayer )
			{
				DungeonReturnInfo info = kvp.Value;
				if ( info == null || info.ReturnMap == null || World.FindMobile( kvp.Key ) == null )
				{
					if ( remove == null )
						remove = new List<Serial>();

					remove.Add( kvp.Key );
				}
			}

			if ( remove == null )
				return;

			for ( int i = 0; i < remove.Count; i++ )
				m_ReturnsByPlayer.Remove( remove[i] );
		}

		// Dungeon owns Map.Maps[] slots 72-120, clear of the sky pool (40-71) and
		// the reserved internal slot (127). The range is split into seven clones for
		// each core terrain map.
		public override string Key { get { return "Dungeon"; } }
		public override int PoolBaseIndex { get { return 72; } }
		public override int PoolSize { get { return CoreBaseMapCount * ClonesPerBaseMap; } }

		public override Map BaseMap { get { return Map.SerpentIsland; } }
		public override int MapWidth { get { return 2560; } }
		public override int MapHeight { get { return 2048; } }

		public override Point3D Landing { get { return new Point3D( 1974, 1977, 0 ); } }

		public override Map ExitMap { get { return Map.Sosaria; } }
		public override Point3D ExitPoint { get { return new Point3D( 3884, 2879, 0 ); } }

		public override string RegionName { get { return "Dungeon Instance"; } }

		// Transient: not saved, freed when empty.
		public override bool Persistent { get { return false; } }

		// Reclaim an emptied dungeon fairly quickly.
		public override TimeSpan UnloadAfter { get { return TimeSpan.FromMinutes( 5 ); } }

		// ----- Pool / terrain -----

		public override void RegisterMaps()
		{
			Map[] bases = GetCloneBaseMaps();

			for ( int b = 0; b < bases.Length; b++ )
			{
				Map baseMap = bases[b];
				if ( baseMap == null )
					continue;

				for ( int i = 0; i < ClonesPerBaseMap; i++ )
				{
					int idx = PoolBaseIndex + ( b * ClonesPerBaseMap ) + i;
					if ( idx >= Map.Maps.Length || Map.Maps[idx] != null )
						continue;

					Map map = new Map( baseMap.MapID, idx, GetFileIndexFor( baseMap ), baseMap.Width, baseMap.Height, 1,
						String.Format( "Dungeon{0}{1}", baseMap.Name, i ), Rules );

					map.BaseMap = baseMap;

					Map.Maps[idx] = map;
					Map.AllMaps.Add( map );
				}
			}
		}

		private static Map[] GetCloneBaseMaps()
		{
			return new Map[]
			{
				Map.Lodor,
				Map.Sosaria,
				Map.Underworld,
				Map.SerpentIsland,
				Map.IslesDread,
				Map.SavagedEmpire,
				Map.Atlantis
			};
		}

		private static int GetFileIndexFor( Map map )
		{
			if ( Object.ReferenceEquals( map, Map.Atlantis ) )
				return 1;

			return map.MapID;
		}

		private static bool IsCloneOf( Map map, Map baseMap )
		{
			return map != null && baseMap != null && Object.ReferenceEquals( map.BaseMap, baseMap );
		}

		public static Map NormalizeExternalMap( Map map )
		{
			if ( map != null && Instance.IsPoolMap( map ) && map.BaseMap != null )
				return map.BaseMap;

			return map;
		}

		// ----- Dungeon catalog -----

		public List<DungeonInstanceDefinition> Definitions
		{
			get { return GetDefinitions(); }
		}

		public int DefinitionCount
		{
			get { return GetDefinitions().Count; }
		}

		private List<DungeonInstanceDefinition> GetDefinitions()
		{
			if ( m_Definitions != null )
				return m_Definitions;

			List<DungeonInstanceDefinition> list = new List<DungeonInstanceDefinition>();
			HashSet<string> listedDungeonNames = BuildListedDungeonNames();

			foreach ( Region region in Region.Regions )
			{
				BaseRegion br = region as BaseRegion;
				if ( br == null || br.Map == null || br.Map == Map.Internal || br.Area == null || br.Area.Length == 0 )
					continue;

				if ( !IsCatalogRegionForDefinitionList( br ) || !IsListedDungeonDefinition( br, listedDungeonNames ) )
					continue;

				list.Add( new DungeonInstanceDefinition( br ) );
			}

			list.Sort( delegate( DungeonInstanceDefinition a, DungeonInstanceDefinition b )
			{
				int map = String.Compare( a.MapName, b.MapName, StringComparison.OrdinalIgnoreCase );
				if ( map != 0 )
					return map;

				return String.Compare( a.Name, b.Name, StringComparison.OrdinalIgnoreCase );
			} );

			for ( int i = 0; i < list.Count; i++ )
				list[i].Index = i;

			m_Definitions = list;
			return m_Definitions;
		}

		private static HashSet<string> BuildListedDungeonNames()
		{
			HashSet<string> names = new HashSet<string>( StringComparer.OrdinalIgnoreCase );

			for ( int i = 1; i <= DungeonListingScanLimit; i++ )
			{
				string world;
				string location;
				Map placer;
				int x;
				int y;
				string dungeon = Worlds.GetDungeonListing( i, out world, out location, out placer, out x, out y );

				AddListedDungeonName( names, dungeon );
			}

			return names;
		}

		private static void AddListedDungeonName( HashSet<string> names, string dungeon )
		{
			if ( String.IsNullOrEmpty( dungeon ) )
				return;

			dungeon = dungeon.Trim();
			if ( dungeon.Length == 0 )
				return;

			names.Add( dungeon );

			string normalized = NormalizeDungeonListingName( dungeon );
			if ( normalized != null && normalized.Length > 0 )
				names.Add( normalized );
		}

		private static bool IsCatalogRegionForDefinitionList( BaseRegion region )
		{
			return region is DungeonRegion || region is BardDungeonRegion || region is CaveRegion || region is DeadRegion;
		}

		private static bool IsListedDungeonDefinition( BaseRegion region, HashSet<string> listedDungeonNames )
		{
			if ( region == null || String.IsNullOrEmpty( region.Name ) || listedDungeonNames == null )
				return false;

			if ( IsExcludedListedDungeon( region.Name ) )
				return false;

			string normalized = NormalizeDungeonListingName( region.Name );
			return listedDungeonNames.Contains( region.Name )
				|| ( normalized != null && listedDungeonNames.Contains( normalized ) );
		}

		private static bool IsExcludedListedDungeon( string name )
		{
			if ( String.IsNullOrEmpty( name ) )
				return false;

			string normalized = NormalizeDungeonListingName( name );

			for ( int i = 0; i < ExcludedListedDungeonNames.Length; i++ )
			{
				string excluded = ExcludedListedDungeonNames[i];
				if ( String.Equals( name, excluded, StringComparison.OrdinalIgnoreCase ) )
					return true;

				if ( String.Equals( normalized, NormalizeDungeonListingName( excluded ), StringComparison.OrdinalIgnoreCase ) )
					return true;
			}

			return false;
		}

		private static string NormalizeDungeonListingName( string name )
		{
			if ( name == null )
				return null;

			name = name.Trim();

			if ( name.StartsWith( "the Dungeon of ", StringComparison.OrdinalIgnoreCase ) )
				return name.Substring( 4 );

			return name;
		}

		public static DungeonInstanceAvailability GetDefinitionAvailability( int index )
		{
			if ( ContainsIndex( BrokenDungeonIndexes, index ) )
				return DungeonInstanceAvailability.Broken;

			if ( ContainsIndex( BadEntranceOrExitDungeonIndexes, index ) )
				return DungeonInstanceAvailability.BadEntranceOrExit;

			return DungeonInstanceAvailability.Allowed;
		}

		public static bool IsDefinitionAvailable( int index )
		{
			return GetDefinitionAvailability( index ) == DungeonInstanceAvailability.Allowed;
		}

		public static string GetAvailabilityLabel( DungeonInstanceAvailability availability )
		{
			switch ( availability )
			{
				case DungeonInstanceAvailability.BadEntranceOrExit:
					return "Bad entrance/exit";
				case DungeonInstanceAvailability.Broken:
					return "Broken";
				default:
					return "Good";
			}
		}

		public static string GetAvailabilityReason( DungeonInstanceAvailability availability )
		{
			switch ( availability )
			{
				case DungeonInstanceAvailability.BadEntranceOrExit:
					return BadEntranceOrExitReason;
				case DungeonInstanceAvailability.Broken:
					return BrokenReason;
				default:
					return null;
			}
		}

		public static int GetAvailabilityHue( DungeonInstanceAvailability availability )
		{
			switch ( availability )
			{
				case DungeonInstanceAvailability.BadEntranceOrExit:
					return BadEntranceOrExitDungeonGateHue;
				case DungeonInstanceAvailability.Broken:
					return BrokenDungeonGateHue;
				default:
					return GoodDungeonGateHue;
			}
		}

		private static bool ContainsIndex( int[] indexes, int index )
		{
			if ( indexes == null )
				return false;

			for ( int i = 0; i < indexes.Length; i++ )
			{
				if ( indexes[i] == index )
					return true;
			}

			return false;
		}

		public DungeonInstanceDefinition GetDefinition( int index )
		{
			List<DungeonInstanceDefinition> defs = GetDefinitions();

			if ( index < 0 || index >= defs.Count )
				return null;

			return defs[index];
		}

		public DungeonInstanceDefinition FindDefinition( string value )
		{
			if ( value == null )
				return null;

			value = value.Trim();
			if ( value.Length == 0 )
				return null;

			int index;
			if ( Int32.TryParse( value, out index ) )
				return GetDefinition( index );

			List<DungeonInstanceDefinition> defs = GetDefinitions();

			for ( int i = 0; i < defs.Count; i++ )
			{
				DungeonInstanceDefinition def = defs[i];
				if ( String.Equals( def.Name, value, StringComparison.OrdinalIgnoreCase ) ||
					String.Equals( def.DisplayName, value, StringComparison.OrdinalIgnoreCase ) )
					return def;
			}

			for ( int i = 0; i < defs.Count; i++ )
			{
				DungeonInstanceDefinition def = defs[i];
				if ( def.DisplayName.IndexOf( value, StringComparison.OrdinalIgnoreCase ) >= 0 )
					return def;
			}

			return null;
		}

		public DungeonInstanceDefinition GetDefaultDefinition()
		{
			List<DungeonInstanceDefinition> defs = GetDefinitions();

			for ( int i = 0; i < defs.Count; i++ )
			{
				if ( defs[i].CanSpawnInstance && defs[i].SpawnCount > 0 )
					return defs[i];
			}

			for ( int i = 0; i < defs.Count; i++ )
			{
				if ( defs[i].CanSpawnInstance )
					return defs[i];
			}

			return defs.Count > 0 ? defs[0] : null;
		}

		public static Difficulty ClampDifficulty( Difficulty difficulty )
		{
			if ( difficulty < Difficulty.Easy )
				return Difficulty.Easy;

			if ( difficulty > Difficulty.Deadly )
				return Difficulty.Deadly;

			return difficulty;
		}

		// ----- Ownership model -----

		// Party members share one instance (keyed to the leader); solo players get a
		// private one (keyed to themselves).
		protected override Serial OwnerKey( Mobile m )
		{
			Party p = Party.Get( m );
			if ( p != null && p.Active && p.Leader != null )
				return p.Leader.Serial;

			return m.Serial;
		}

		public bool SendToConfiguredInstance( Mobile from, DungeonInstanceDefinition definition, Difficulty difficulty )
		{
			if ( from == null )
				return false;

			return SendToConfiguredInstance( from, OwnerKey( from ), definition, difficulty, null, Point3D.Zero, InstanceOwnerKind.Mobile );
		}

		public bool SendToConfiguredInstance( Mobile from, Serial instanceKey, DungeonInstanceDefinition definition, Difficulty difficulty )
		{
			return SendToConfiguredInstance( from, instanceKey, definition, difficulty, null, Point3D.Zero, InstanceOwnerKind.Mobile );
		}

		public bool SendToConfiguredInstance( Mobile from, Serial instanceKey, DungeonInstanceDefinition definition, Difficulty difficulty, Map returnMap, Point3D returnPoint )
		{
			return SendToConfiguredInstance( from, instanceKey, definition, difficulty, returnMap, returnPoint, InstanceOwnerKind.Mobile );
		}

		public bool SendToConfiguredInstance( Mobile from, Serial instanceKey, DungeonInstanceDefinition definition, Difficulty difficulty, Map returnMap, Point3D returnPoint, InstanceOwnerKind ownerKind )
		{
			if ( from == null )
				return false;

			if ( definition == null )
			{
				from.SendMessage( "This dungeon instance gate is not configured with a valid dungeon." );
				return false;
			}

			if ( !definition.CanSpawnInstance )
			{
				from.SendMessage( "This dungeon is temporarily disabled for instance spawning: {0}.", definition.InstanceBlockReason );
				return false;
			}

			difficulty = ClampDifficulty( difficulty );

			DungeonInstanceSettings requested = new DungeonInstanceSettings( definition, difficulty, NormalizeExternalMap( returnMap ), returnPoint );
			Instance existing = GetByKey( instanceKey );
			if ( existing != null )
				existing.OwnerKind = ownerKind;

			if ( existing != null )
			{
				DungeonInstanceSettings active = GetSettings( existing );
				bool changed = active == null || !active.Matches( requested );

				if ( changed )
				{
					if ( existing.IsLive && HasPlayersInside( existing ) )
					{
						if ( active != null && active.Definition != null )
							from.SendMessage( "This gate already has an active {0} instance; sending you there.", active.Definition.Name );

							return SendToInstanceAndRecordReturn( from, existing, requested );
					}

					FreeInstance( existing );
					existing = null;
				}
			}

			Instance inst = existing != null ? existing : GetOrCreateByKey( instanceKey, ownerKind );
			m_SettingsByOwner[inst.OwnerSerial] = requested;
			if ( inst.IsLive )
				m_SettingsByLiveMap[inst.LiveMapIndex] = requested;

			if ( !SendToInstanceAndRecordReturn( from, inst, requested ) )
			{
				FreeInstance( inst );
				return false;
			}

			return true;
		}

		private bool SendToInstanceAndRecordReturn( Mobile from, Instance inst, DungeonInstanceSettings settings )
		{
			if ( !SendToInstance( from, inst ) )
				return false;

			RegisterReturn( from, inst, settings );
			return true;
		}

			public bool CleanupInstance( Serial instanceKey )
			{
				Instance inst = GetByKey( instanceKey );
				if ( inst == null )
					return false;

			EjectPlayers( inst );
				FreeInstance( inst );
				return true;
			}

			public int ClearAllInstances()
			{
				List<Instance> instances = new List<Instance>( AllInstances );

				for ( int i = 0; i < instances.Count; i++ )
				{
					Instance inst = instances[i];
					EjectPlayers( inst );
					FreeInstance( inst );
				}

				return instances.Count;
			}

			private static void ClearTrackedInstanceItems( Instance inst )
			{
				if ( inst == null || inst.Items.Count == 0 )
					return;

				for ( int i = 0; i < inst.Items.Count; i++ )
				{
					InstanceItem item = inst.Items[i];
					if ( item != null && item.Item != null && !item.Item.Deleted )
						item.Item.Delete();
				}

				inst.Items.Clear();
			}

			private void EjectPlayers( Instance inst )
			{
				if ( inst == null || !inst.IsLive )
					return;

				Map map = inst.LiveMap;
				if ( map == null )
					return;

				EjectPlayerCorpses( inst, map );

				List<Mobile> players = null;
				foreach ( NetState ns in NetState.Instances )
				{
					Mobile m = ns.Mobile;
					if ( m != null && m.Map == map )
					{
						if ( players == null )
							players = new List<Mobile>();

						players.Add( m );
					}
				}

				if ( players == null )
					return;

				for ( int i = 0; i < players.Count; i++ )
				{
					Mobile m = players[i];
					if ( m == null || m.Deleted || m.Map != map )
						continue;

					DungeonReturnInfo info = GetReturnInfo( m );
					Map exitMap = info != null && info.ReturnMap != null ? info.ReturnMap : GetReturnMap( inst );
					Point3D exitPoint = info != null && info.ReturnMap != null ? info.ReturnPoint : GetReturnPoint( inst );
					exitMap = NormalizeExternalMap( exitMap );

					BaseCreature.TeleportPets( m, exitPoint, exitMap, false );
					m.MoveToWorld( exitPoint, exitMap );
					ClearReturn( m );
				}
			}

		private void EjectPlayerCorpses( Instance inst, Map liveMap )
		{
			if ( inst == null || liveMap == null )
				return;

			List<Corpse> corpses = null;

			foreach ( Item item in World.Items.Values )
			{
				if ( item == null || item.Deleted || item.Map != liveMap || item.Parent != null )
					continue;

				Corpse corpse = item as Corpse;
				if ( corpse == null )
					continue;

				if ( corpses == null )
					corpses = new List<Corpse>();

				corpses.Add( corpse );
			}

			if ( corpses == null )
				return;

			for ( int i = 0; i < corpses.Count; i++ )
			{
				Corpse corpse = corpses[i];
				if ( corpse == null || corpse.Deleted || corpse.Map != liveMap )
					continue;

				Mobile owner = corpse.Owner;
				if ( owner != null && owner.Player && corpse.TotalItems > 0 )
				{
					Map returnMap;
					Point3D returnPoint;
					GetCorpseReturnLocation( inst, owner, out returnMap, out returnPoint );

					if ( returnMap != null )
					{
						RemoveTrackedInstanceItem( inst, corpse );
						corpse.MoveToWorld( returnPoint, returnMap );
						continue;
					}
				}

				corpse.Delete();
			}
		}

		private void GetCorpseReturnLocation( Instance inst, Mobile owner, out Map returnMap, out Point3D returnPoint )
		{
			DungeonReturnInfo info = GetReturnInfo( owner );

			if ( info != null && info.ReturnMap != null )
			{
				returnMap = NormalizeExternalMap( info.ReturnMap );
				returnPoint = info.ReturnPoint;
			}
			else
			{
				returnMap = GetReturnMap( inst );
				returnPoint = GetReturnPoint( inst );
			}

			if ( returnMap == null )
			{
				returnMap = ExitMap;
				returnPoint = ExitPoint;
			}
		}

		private static void RemoveTrackedInstanceItem( Instance inst, Item item )
		{
			if ( inst == null || item == null )
				return;

			for ( int i = inst.Items.Count - 1; i >= 0; i-- )
			{
				InstanceItem instanceItem = inst.Items[i];
				if ( instanceItem != null && Object.ReferenceEquals( instanceItem.Item, item ) )
					inst.Items.RemoveAt( i );
			}
		}

		public string GetInstanceStatus( Serial instanceKey )
		{
			Instance inst = GetByKey( instanceKey );
			if ( inst == null )
				return "None";

			DungeonInstanceSettings settings = GetSettings( inst );
			string dungeon = settings != null && settings.Definition != null ? settings.Definition.Name : "Unknown";

			if ( inst.IsLive )
				return String.Format( "Live: {0} on map {1}", dungeon, inst.LiveMapIndex );

			return String.Format( "Parked: {0}", dungeon );
		}

			public override bool LeaveInstance( Mobile m )
			{
				if ( m == null || !IsPoolMap( m.Map ) )
					return false;

				DungeonReturnInfo info = GetReturnInfo( m );
				Map exitMap = info != null && info.ReturnMap != null ? info.ReturnMap : GetExitMap( m );
				Point3D exitPoint = info != null && info.ReturnMap != null ? info.ReturnPoint : GetExitPoint( m );
				exitMap = NormalizeExternalMap( exitMap );

				BaseCreature.TeleportPets( m, exitPoint, exitMap, false );
				m.MoveToWorld( exitPoint, exitMap );
				ClearReturn( m );
				return true;
			}

		public override void OnLogin( Mobile m )
		{
			if ( m == null || !IsPoolMap( m.Map ) )
				return;

			DungeonReturnInfo info = GetReturnInfo( m );
			if ( info != null && info.OwnerKind == InstanceOwnerKind.PublicGateway )
			{
				Instance publicInst = GetByKey( info.InstanceKey );
				if ( publicInst != null && publicInst.IsPublicInstance && Object.ReferenceEquals( publicInst.LiveMap, m.Map ) )
				{
					publicInst.Touch();
					return;
				}

				BounceToReturn( m, info );
				ClearReturn( m );
				return;
			}

			Instance privateInst = GetByOwner( m );
			if ( privateInst != null )
			{
				SendToInstance( m, privateInst );
				return;
			}

			if ( info != null )
			{
				BounceToReturn( m, info );
				ClearReturn( m );
				return;
			}

			BounceToReturn( m, null );
		}

		protected override void OnReleaseDeadOwner( Instance inst )
		{
			EjectPlayers( inst );
		}

			protected override Map GetExitMap( Mobile m )
			{
				DungeonInstanceSettings settings = GetSettingsForMap( m != null ? m.Map : null );
				return NormalizeExternalMap( settings != null && settings.ReturnMap != null ? settings.ReturnMap : base.GetExitMap( m ) );
			}

		protected override Point3D GetExitPoint( Mobile m )
		{
			DungeonInstanceSettings settings = GetSettingsForMap( m != null ? m.Map : null );
			return settings != null && settings.ReturnMap != null ? settings.ReturnPoint : base.GetExitPoint( m );
		}

		private DungeonInstanceSettings GetSettingsForMap( Map map )
		{
			if ( map == null )
				return null;

			DungeonInstanceSettings settings;
			if ( m_SettingsByLiveMap.TryGetValue( map.MapIndex, out settings ) )
				return settings;

			return null;
		}

		private Map GetReturnMap( Instance inst )
		{
			DungeonInstanceSettings settings = GetSettings( inst );
			return NormalizeExternalMap( settings != null && settings.ReturnMap != null ? settings.ReturnMap : ExitMap );
		}

		private Point3D GetReturnPoint( Instance inst )
		{
			DungeonInstanceSettings settings = GetSettings( inst );
			return settings != null && settings.ReturnMap != null ? settings.ReturnPoint : ExitPoint;
		}

		private void RegisterReturn( Mobile from, Instance inst, DungeonInstanceSettings settings )
		{
			if ( from == null || inst == null || settings == null || settings.ReturnMap == null )
				return;

			Map returnMap = NormalizeExternalMap( settings.ReturnMap );
			if ( returnMap == null )
				return;

			m_ReturnsByPlayer[from.Serial] = new DungeonReturnInfo(
				inst.OwnerSerial,
				inst.OwnerKind,
				returnMap,
				settings.ReturnPoint,
				DateTime.Now );
		}

		private DungeonReturnInfo GetReturnInfo( Mobile m )
		{
			if ( m == null )
				return null;

			DungeonReturnInfo info;
			m_ReturnsByPlayer.TryGetValue( m.Serial, out info );
			return info;
		}

		private void ClearReturn( Mobile m )
		{
			if ( m != null )
				m_ReturnsByPlayer.Remove( m.Serial );
		}

		private void BounceToReturn( Mobile m, DungeonReturnInfo info )
		{
			if ( m == null )
				return;

			Map map = NormalizeExternalMap( info != null && info.ReturnMap != null ? info.ReturnMap : ExitMap );
			Point3D point = info != null && info.ReturnMap != null ? info.ReturnPoint : ExitPoint;

			BaseCreature.TeleportPets( m, point, map, false );
			m.MoveToWorld( point, map );
		}

		private DungeonInstanceSettings GetSettings( Instance inst )
		{
			if ( inst == null )
				return null;

			DungeonInstanceSettings settings;
			if ( m_SettingsByOwner.TryGetValue( inst.OwnerSerial, out settings ) && settings != null && settings.Definition != null )
				return settings;

			DungeonInstanceDefinition def = GetDefaultDefinition();
			if ( def == null )
				return null;

			settings = new DungeonInstanceSettings( def, Difficulty.Normal );
			m_SettingsByOwner[inst.OwnerSerial] = settings;
			return settings;
		}

		// ----- Live map selection / entry -----

		protected override int AcquireMapIndex( Instance inst )
		{
			DungeonInstanceSettings settings = GetSettings( inst );
			Map requiredBase = settings != null && settings.Definition != null ? settings.Definition.Map : BaseMap;

			for ( int i = 0; i < PoolSize; i++ )
			{
				int idx = PoolBaseIndex + i;
				if ( idx >= Map.Maps.Length )
					continue;

				Map map = Map.Maps[idx];
				if ( IsCloneOf( map, requiredBase ) && !IsMapIndexLive( idx ) )
					return idx;
			}

			Instance victim = null;
			foreach ( Instance live in LiveInstances )
			{
				if ( HasPlayersInside( live ) )
					continue;

				if ( !IsCloneOf( live.LiveMap, requiredBase ) )
					continue;

				if ( victim == null || live.LastTouched < victim.LastTouched )
					victim = live;
			}

			if ( victim == null )
				return -1;

			int freed = victim.LiveMapIndex;
			FreeInstance( victim );
			return freed;
		}

		protected override Point3D GetLandingPoint( Instance inst, Map map )
		{
			DungeonInstanceSettings settings = GetSettings( inst );
			if ( settings != null && settings.Definition != null )
				return settings.Definition.Landing;

			return base.GetLandingPoint( inst, map );
		}

		// ----- Contents -----

		protected override void BuildContents( Instance inst, Map map )
		{
			CleanupRuntime( inst.OwnerSerial );

				DungeonInstanceSettings settings = GetSettings( inst );
				if ( settings == null || settings.Definition == null || map == null )
					return;

				ClearTrackedInstanceItems( inst );
				ClearStrayMobiles( map );

				m_SettingsByLiveMap[map.MapIndex] = settings;

			RuntimeDungeonRegion region = new RuntimeDungeonRegion( this, inst, settings, map );
			region.Register();

			List<SpawnEntry> entries = new List<SpawnEntry>();
			SpawnEntry[] source = settings.Definition.Spawns;

			if ( source != null )
			{
				for ( int i = 0; i < source.Length; i++ )
				{
					SpawnEntry src = source[i];
					if ( src == null || src.Definition == null || src.Max <= 0 )
						continue;

					SpawnEntry entry = new SpawnEntry(
						NextRuntimeSpawnId(),
						region,
						src.HomeLocation,
						src.HomeRange,
						src.Direction,
						src.Definition,
						src.Max,
						src.MinSpawnTime,
						src.MaxSpawnTime );

					entries.Add( entry );
				}
			}

			region.Spawns = entries.ToArray();

			for ( int i = 0; i < entries.Count; i++ )
				entries[i].Respawn();

			CloneWorldContent( inst, map, settings.Definition );

			RuntimeDungeonState state = new RuntimeDungeonState( settings, region, entries.ToArray(), map, settings.Definition.Area );
			m_Runtimes[inst.OwnerSerial] = state;
			state.ApplyDifficulty();
			state.StartScaler();
		}

		private void CloneWorldContent( Instance inst, Map map, DungeonInstanceDefinition definition )
		{
			List<Item> sources = null;

			foreach ( Item item in World.Items.Values )
			{
				if ( item == null || item.Deleted || item.Parent != null )
					continue;

				if ( !Object.ReferenceEquals( item.Map, definition.Map ) || !definition.ContainsFunctionalItem( item.Location ) )
					continue;

				if ( item is PremiumSpawner || item is Spawner )
				{
					if ( !definition.Contains( item.Location ) )
						continue;

					if ( sources == null )
						sources = new List<Item>();

					sources.Add( item );
				}
				else if ( ShouldCloneDoor( item as BaseDoor, definition ) )
				{
					if ( sources == null )
						sources = new List<Item>();

					sources.Add( item );
				}
				else if ( ShouldCloneTeleporter( item, definition ) )
				{
					if ( sources == null )
						sources = new List<Item>();

					sources.Add( item );
				}
			}

			Dictionary<BaseDoor, BaseDoor> doors = new Dictionary<BaseDoor, BaseDoor>();

			if ( sources != null )
			{
				for ( int i = 0; i < sources.Count; i++ )
				{
					Item source = sources[i];
					Item clone = CloneWorldItem( source );
					if ( clone == null )
						continue;

					RetargetClone( source, clone, map, definition );

					BaseDoor sourceDoor = source as BaseDoor;
					BaseDoor cloneDoor = clone as BaseDoor;
					if ( sourceDoor != null && cloneDoor != null )
						doors[sourceDoor] = cloneDoor;

					clone.MoveToWorld( source.Location, map );
					inst.Items.Add( new InstanceItem( clone, source.Location ) );
					StartClonedSpawner( source, clone );
				}
			}

			CloneDecoratedTeleporters( inst, map, definition );

			foreach ( KeyValuePair<BaseDoor, BaseDoor> kvp in doors )
			{
				BaseDoor sourceDoor = kvp.Key;
				BaseDoor cloneDoor = kvp.Value;
				BaseDoor cloneLink;

				if ( sourceDoor.Link != null && doors.TryGetValue( sourceDoor.Link, out cloneLink ) )
					cloneDoor.Link = cloneLink;
				else
					cloneDoor.Link = null;
				}
			}

			private void CloneDecoratedTeleporters( Instance inst, Map map, DungeonInstanceDefinition definition )
			{
				List<DecorationTeleporterSource> sources = GetDecorationTeleporterSources( definition.Map );
				if ( sources == null || sources.Count == 0 )
					return;

				for ( int i = 0; i < sources.Count; i++ )
				{
					DecorationTeleporterSource source = sources[i];

					if ( source == null || !definition.ContainsFunctionalItem( source.Location ) )
						continue;

					bool internalDestination = IsInternalDestination( source.SourceMap, source.DestinationMap, source.Destination, definition );

					Item clone;
					if ( internalDestination )
					{
						if ( HasTeleporterCloneAt( map, source ) )
							continue;

						clone = source.Construct();
						if ( clone == null )
							continue;

						RetargetClone( source.SourceMap, clone, map, definition );
					}
					else
					{
						if ( !ShouldCloneExitTeleporter( source.SourceMap, source.DestinationMap, source.Destination, definition ) )
							continue;

						if ( HasExitTeleporterCloneAt( map, source ) )
							continue;

						clone = source.ConstructExitTeleporter();
					}

					if ( clone == null )
						continue;

					clone.MoveToWorld( source.Location, map );
					inst.Items.Add( new InstanceItem( clone, source.Location ) );
				}
			}

			private static bool HasTeleporterCloneAt( Map map, DecorationTeleporterSource source )
			{
				IPooledEnumerable eable = map.GetItemsInRange( source.Location, 0 );
				try
				{
					foreach ( Item item in eable )
					{
						if ( item == null || item.Deleted || item.Parent != null )
							continue;

						if ( item.Z == source.Location.Z && item.ItemID == source.ItemID && item.GetType() == source.Type )
							return true;
					}
				}
				finally
				{
					eable.Free();
				}

				return false;
			}

			private static bool HasExitTeleporterCloneAt( Map map, DecorationTeleporterSource source )
			{
				IPooledEnumerable eable = map.GetItemsInRange( source.Location, 0 );
				try
				{
					foreach ( Item item in eable )
					{
						if ( item == null || item.Deleted || item.Parent != null )
							continue;

						if ( item.Z == source.Location.Z && item.ItemID == source.ItemID && item is DungeonInstanceExitTeleporter )
							return true;
					}
				}
				finally
				{
					eable.Free();
				}

				return false;
			}

			private static bool ShouldCloneTeleporter( Item item, DungeonInstanceDefinition definition )
			{
				Map destinationMap;
				Point3D destination;

				return TryGetTeleportDestination( item, out destinationMap, out destination )
					&& IsInternalDestination( item, destinationMap, destination, definition );
			}

		private static bool ShouldCloneDoor( BaseDoor door, DungeonInstanceDefinition definition )
		{
			if ( door == null )
				return false;

			ThruDoor thruDoor = door as ThruDoor;
			if ( thruDoor == null )
				return true;

			Map destinationMap;
			Point3D destination;
			if ( !TryGetTeleportDestination( thruDoor, out destinationMap, out destination ) )
				return true;

			return IsInternalDestination( thruDoor, destinationMap, destination, definition );
		}

		private static bool TryGetTeleportDestination( Item item, out Map destinationMap, out Point3D destination )
		{
			destinationMap = null;
			destination = Point3D.Zero;

			Teleporter teleporter = item as Teleporter;
			if ( teleporter != null )
			{
				destinationMap = teleporter.MapDest;
				destination = teleporter.PointDest;
				return destinationMap != null || destination != Point3D.Zero;
			}

			moongates gate = item as moongates;
			if ( gate != null )
			{
				destinationMap = gate.MapDest;
				destination = gate.PointDest;
				return destinationMap != null || destination != Point3D.Zero;
			}

			QuestTeleporter quest = item as QuestTeleporter;
			if ( quest != null )
			{
				destinationMap = quest.TeleporterMapDest;
				destination = quest.TeleporterPointDest;
				return destinationMap != null || destination != Point3D.Zero;
			}

			ThruDoor thruDoor = item as ThruDoor;
			if ( thruDoor != null )
			{
				destinationMap = thruDoor.MapDest;
				destination = thruDoor.PointDest;
				return destinationMap != null || destination != Point3D.Zero;
			}

			return false;
		}

		private static bool IsInternalDestination( Item source, Map destinationMap, Point3D destination, DungeonInstanceDefinition definition )
		{
			if ( source == null || definition == null || destination == Point3D.Zero )
				return false;

			return IsInternalDestination( source.Map, destinationMap, destination, definition );
		}

			private static bool IsInternalDestination( Map sourceMap, Map destinationMap, Point3D destination, DungeonInstanceDefinition definition )
			{
				if ( sourceMap == null || definition == null || destination == Point3D.Zero )
					return false;

				Map resolvedMap = ResolveDestinationMap( sourceMap, destinationMap );
				return resolvedMap == definition.Map && definition.ContainsInstanceDestination( resolvedMap, destination );
			}

			private static bool ShouldCloneExitTeleporter( Map sourceMap, Map destinationMap, Point3D destination, DungeonInstanceDefinition definition )
			{
				if ( sourceMap == null || definition == null || destination == Point3D.Zero )
					return false;

				Map resolvedMap = ResolveDestinationMap( sourceMap, destinationMap );
				return resolvedMap != definition.Map || !definition.ContainsInstanceDestination( resolvedMap, destination );
			}

			private static Map ResolveDestinationMap( Item source, Map destinationMap )
			{
				return ResolveDestinationMap( source != null ? source.Map : null, destinationMap );
			}

		private static Map ResolveDestinationMap( Map sourceMap, Map destinationMap )
		{
			if ( destinationMap == null || destinationMap == Map.Internal )
				return sourceMap;

			return destinationMap;
		}

		private static void RetargetClone( Item source, Item clone, Map map, DungeonInstanceDefinition definition )
		{
			Map destinationMap;
			Point3D destination;
			if ( !TryGetTeleportDestination( clone, out destinationMap, out destination ) )
				return;

			if ( !IsInternalDestination( source, destinationMap, destination, definition ) )
				return;

			SetTeleportDestinationMap( clone, map );
		}

		private static void RetargetClone( Map sourceMap, Item clone, Map map, DungeonInstanceDefinition definition )
		{
			Map destinationMap;
			Point3D destination;
			if ( !TryGetTeleportDestination( clone, out destinationMap, out destination ) )
				return;

			if ( !IsInternalDestination( sourceMap, destinationMap, destination, definition ) )
				return;

			SetTeleportDestinationMap( clone, map );
		}

			private static void SetTeleportDestinationMap( Item clone, Map map )
			{
				Teleporter teleporter = clone as Teleporter;
				if ( teleporter != null )
			{
				teleporter.MapDest = map;
				return;
			}

			moongates gate = clone as moongates;
			if ( gate != null )
			{
				gate.MapDest = map;
				return;
			}

			QuestTeleporter quest = clone as QuestTeleporter;
			if ( quest != null )
			{
				quest.TeleporterMapDest = map;
				return;
			}

			ThruDoor thruDoor = clone as ThruDoor;
				if ( thruDoor != null )
					thruDoor.MapDest = map;
			}

			private static List<DecorationTeleporterSource> GetDecorationTeleporterSources( Map map )
			{
				if ( map == null )
					return null;

				List<DecorationTeleporterSource> sources;
				if ( !m_DecorationTeleportersByMap.TryGetValue( map.MapIndex, out sources ) )
				{
					sources = LoadDecorationTeleporterSources( map );
					m_DecorationTeleportersByMap[map.MapIndex] = sources;
				}

				return sources;
			}

			private static List<DecorationTeleporterSource> LoadDecorationTeleporterSources( Map map )
			{
				List<DecorationTeleporterSource> sources = new List<DecorationTeleporterSource>();
				string fileName = GetDecorationFileName( map );
				if ( fileName == null )
					return sources;

				string path = Path.Combine( Core.BaseDirectory, Path.Combine( "Data/Decoration", fileName ) );
				if ( !File.Exists( path ) )
				{
					path = Path.Combine( "Data/Decoration", fileName );
					if ( !File.Exists( path ) )
						return sources;
				}

				try
				{
					using ( StreamReader reader = new StreamReader( path ) )
						ReadDecorationTeleporterSources( reader, map, sources );
				}
				catch
				{
				}

				return sources;
			}

			private static string GetDecorationFileName( Map map )
			{
				if ( Object.ReferenceEquals( map, Map.Lodor ) )
					return "Lodor.cfg";

				if ( Object.ReferenceEquals( map, Map.Sosaria ) )
					return "Sosaria.cfg";

				if ( Object.ReferenceEquals( map, Map.Underworld ) )
					return "Underworld.cfg";

				if ( Object.ReferenceEquals( map, Map.SerpentIsland ) )
					return "SerpentIsland.cfg";

				if ( Object.ReferenceEquals( map, Map.IslesDread ) )
					return "IslesOfDread.cfg";

				if ( Object.ReferenceEquals( map, Map.SavagedEmpire ) )
					return "SavagedEmpire.cfg";

				if ( Object.ReferenceEquals( map, Map.Atlantis ) )
					return "Atlantis.cfg";

				return null;
			}

			private static void ReadDecorationTeleporterSources( StreamReader reader, Map sourceMap, List<DecorationTeleporterSource> sources )
			{
				string line;
				while ( TryReadDecorationHeader( reader, out line ) )
				{
					string typeName;
					int itemID;
					string[] parameters;
					bool useEntries = false;
					Type type = null;

					if ( TryParseDecorationHeader( line, out typeName, out itemID, out parameters ) )
					{
						type = ScriptCompiler.FindTypeByName( typeName, true );
						useEntries = type != null && IsDecorationTeleporterType( type ) && HasDecorationParameter( parameters, "PointDest" );
					}

					while ( (line = reader.ReadLine()) != null )
					{
						line = line.Trim();

						if ( line.Length == 0 )
							break;

						if ( line.StartsWith( "#" ) )
							continue;

						if ( !useEntries )
							continue;

						Point3D location;
						if ( !TryParseDecorationLocation( line, out location ) )
							continue;

						DecorationTeleporterSource source = new DecorationTeleporterSource( sourceMap, type, itemID, parameters, location );
						if ( source.Destination != Point3D.Zero )
							sources.Add( source );
					}
				}
			}

			private static bool TryReadDecorationHeader( StreamReader reader, out string line )
			{
				while ( (line = reader.ReadLine()) != null )
				{
					line = line.Trim();

					if ( line.Length > 0 && !line.StartsWith( "#" ) )
						return true;
				}

				return false;
			}

			private static bool TryParseDecorationHeader( string line, out string typeName, out int itemID, out string[] parameters )
			{
				typeName = null;
				itemID = 0;
				parameters = EmptyStringArray;

				int typeEnd = line.IndexOf( ' ' );
				if ( typeEnd <= 0 )
					return false;

				typeName = line.Substring( 0, typeEnd ).Trim();

				string rest = line.Substring( typeEnd + 1 ).Trim();
				int paramStart = rest.IndexOf( '(' );

				if ( paramStart >= 0 )
				{
					itemID = Utility.ToInt32( rest.Substring( 0, paramStart ).Trim() );

					string paramText = rest.Substring( paramStart + 1 ).Trim();
					if ( paramText.EndsWith( ")" ) )
						paramText = paramText.Substring( 0, paramText.Length - 1 );

					parameters = SplitDecorationParameters( paramText );
				}
				else
				{
					itemID = Utility.ToInt32( rest );
				}

				return typeName.Length > 0;
			}

			private static string[] SplitDecorationParameters( string paramText )
			{
				if ( paramText == null || paramText.Length == 0 )
					return EmptyStringArray;

				string[] parameters = paramText.Split( ';' );
				for ( int i = 0; i < parameters.Length; i++ )
					parameters[i] = parameters[i].Trim();

				return parameters;
			}

			private static bool TryParseDecorationLocation( string line, out Point3D location )
			{
				location = Point3D.Zero;

				string[] parts = line.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
				if ( parts.Length < 3 )
					return false;

				try
				{
					location = new Point3D( Utility.ToInt32( parts[0] ), Utility.ToInt32( parts[1] ), Utility.ToInt32( parts[2] ) );
					return true;
				}
				catch
				{
					return false;
				}
			}

			private static bool IsDecorationTeleporterType( Type type )
			{
				return typeof( Teleporter ).IsAssignableFrom( type )
					|| typeof( moongates ).IsAssignableFrom( type )
					|| typeof( QuestTeleporter ).IsAssignableFrom( type );
			}

			private static bool HasDecorationParameter( string[] parameters, string parameterName )
			{
				for ( int i = 0; i < parameters.Length; i++ )
				{
					string name;
					string value;
					if ( TrySplitDecorationParameter( parameters[i], out name, out value ) && String.Equals( name, parameterName, StringComparison.OrdinalIgnoreCase ) )
						return true;
				}

				return false;
			}

			private static bool TrySplitDecorationParameter( string parameter, out string name, out string value )
			{
				name = null;
				value = null;

				if ( parameter == null )
					return false;

				int index = parameter.IndexOf( '=' );
				if ( index < 0 )
				{
					name = parameter.Trim();
					return name.Length > 0;
				}

				name = parameter.Substring( 0, index ).Trim();
				value = parameter.Substring( index + 1 ).Trim();

				return name.Length > 0;
			}

			private static Map ParseDecorationMap( string value )
			{
				if ( value == null || value.Length == 0 )
					return null;

				try
				{
					return Map.Parse( value );
				}
				catch
				{
					return null;
				}
			}

			private static Point3D ParseDecorationPoint( string value )
			{
				if ( value == null || value.Length == 0 )
					return Point3D.Zero;

				try
				{
					return Point3D.Parse( value );
				}
				catch
				{
					return Point3D.Zero;
				}
			}

		private static Item CloneItem( Item source )
		{
			Type t = source.GetType();
			System.Reflection.ConstructorInfo c = t.GetConstructor( Type.EmptyTypes );

			if ( c != null )
			{
				try
				{
					object o = c.Invoke( null );
					Item newItem = o as Item;
					if ( newItem != null )
					{
						Server.Commands.Dupe.CopyProperties( newItem, source );
						return newItem;
					}
				}
				catch { }
			}
			return null;
		}

		private static Item CloneWorldItem( Item source )
		{
			BaseDoor door = source as BaseDoor;
			if ( door != null )
				return CloneDoor( door );

			if ( source is Teleporter || source is moongates || source is QuestTeleporter )
				return CloneItem( source );

			PremiumSpawner premium = source as PremiumSpawner;
			if ( premium != null )
			{
				PremiumSpawner clone = new PremiumSpawner(
					premium.Count,
					premium.CountA,
					premium.CountB,
					premium.CountC,
					premium.CountD,
					premium.CountE,
					premium.SpawnID,
					premium.MinDelay,
					premium.MaxDelay,
					premium.Team,
					premium.HomeRange,
					premium.WalkingRange,
					CopyStrings( premium.CreaturesName ),
					CopyStrings( premium.SubSpawnerA ),
					CopyStrings( premium.SubSpawnerB ),
					CopyStrings( premium.SubSpawnerC ),
					CopyStrings( premium.SubSpawnerD ),
					CopyStrings( premium.SubSpawnerE ) );

				clone.Group = premium.Group;
				clone.Running = false;
				return clone;
			}

			Spawner classic = source as Spawner;
			if ( classic != null )
			{
				Spawner clone = new Spawner(
					classic.Count,
					classic.MinDelay,
					classic.MaxDelay,
					classic.Team,
					classic.HomeRange,
					CopyStrings( classic.SpawnNames ) );

				clone.WalkingRange = classic.WalkingRange;
				clone.Group = classic.Group;
				clone.Running = false;
				return clone;
			}

			return null;
		}

		private static Item CloneDoor( BaseDoor source )
		{
			Item clone = CloneItem( source );
			if ( clone != null )
				return clone;

			System.Reflection.ConstructorInfo c = source.GetType().GetConstructor( new Type[] { typeof( DoorFacing ) } );
			if ( c == null )
				return null;

			try
			{
				object o = c.Invoke( new object[] { GuessDoorFacing( source ) } );
				BaseDoor door = o as BaseDoor;
				if ( door == null )
					return null;

				Server.Commands.Dupe.CopyProperties( door, source );
				return door;
			}
			catch
			{
				return null;
			}
		}

		private static DoorFacing GuessDoorFacing( BaseDoor door )
		{
			Array values = Enum.GetValues( typeof( DoorFacing ) );
			for ( int i = 0; i < values.Length; i++ )
			{
				DoorFacing facing = (DoorFacing)values.GetValue( i );
				if ( BaseDoor.GetOffset( facing ) == door.Offset )
					return facing;
			}

			return DoorFacing.WestCW;
		}

		private static List<string> CopyStrings( List<string> source )
		{
			return source != null ? new List<string>( source ) : new List<string>();
		}

		private static void StartClonedSpawner( Item source, Item clone )
		{
			PremiumSpawner clonePremium = clone as PremiumSpawner;
			if ( clonePremium != null )
			{
				clonePremium.Running = true;
				clonePremium.Respawn();
				return;
			}

			Spawner cloneClassic = clone as Spawner;
			if ( cloneClassic != null )
			{
				cloneClassic.Running = true;
				cloneClassic.Respawn();
			}
		}

		private int NextRuntimeSpawnId()
		{
			while ( SpawnEntry.Table.Contains( m_NextRuntimeSpawnId ) )
				m_NextRuntimeSpawnId--;

			return m_NextRuntimeSpawnId--;
		}

		protected override void OnFreeInstance( Instance inst, Map liveMap )
		{
			EjectPlayerCorpses( inst, liveMap );

			CleanupRuntime( inst.OwnerSerial );

			m_SettingsByOwner.Remove( inst.OwnerSerial );

			if ( liveMap != null )
			{
				m_SettingsByLiveMap.Remove( liveMap.MapIndex );
				ClearStrayMobiles( liveMap );
			}
		}

		private void CleanupRuntime( Serial owner )
		{
			RuntimeDungeonState state;
			if ( m_Runtimes.TryGetValue( owner, out state ) )
			{
				state.Delete();
				m_Runtimes.Remove( owner );
			}
		}

		private static void ClearStrayMobiles( Map liveMap )
		{
			List<Mobile> strays = null;

			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m == null || m.Deleted || m.Map != liveMap )
					continue;

				if ( m is BaseCreature && !m.Player )
				{
					if ( strays == null )
						strays = new List<Mobile>();

					strays.Add( m );
				}
			}

			if ( strays != null )
			{
				for ( int i = 0; i < strays.Count; i++ )
					strays[i].Delete();
			}
		}

		public sealed class DungeonInstanceDefinition
		{
			private int m_Index;
			private readonly string m_Name;
			private readonly string m_DisplayName;
			private readonly Map m_Map;
			private readonly string m_MapName;
			private readonly Point3D m_Landing;
			private readonly Rectangle3D[] m_Area;
			private readonly int m_Priority;
			private readonly SpawnZLevel m_SpawnZLevel;
			private readonly SpawnEntry[] m_Spawns;

			public int Index { get { return m_Index; } set { m_Index = value; } }
			public string Name { get { return m_Name; } }
			public string DisplayName { get { return m_DisplayName; } }
			public Map Map { get { return m_Map; } }
			public string MapName { get { return m_MapName; } }
			public Point3D Landing { get { return m_Landing; } }
			public Rectangle3D[] Area { get { return m_Area; } }
			public int Priority { get { return m_Priority; } }
			public SpawnZLevel SpawnZLevel { get { return m_SpawnZLevel; } }
			public SpawnEntry[] Spawns { get { return m_Spawns; } }
			public int SpawnCount { get { return m_Spawns == null ? 0 : m_Spawns.Length; } }
			public DungeonInstanceAvailability Availability { get { return DungeonInstanceType.GetDefinitionAvailability( m_Index ); } }
			public string AvailabilityLabel { get { return DungeonInstanceType.GetAvailabilityLabel( Availability ); } }
			public string InstanceBlockReason { get { return DungeonInstanceType.GetAvailabilityReason( Availability ); } }
			public bool CanSpawnInstance { get { return Availability == DungeonInstanceAvailability.Allowed; } }

			public bool Contains( Point3D loc )
			{
				return Contains( m_Area, loc );
			}

			public bool ContainsFunctionalItem( Point3D loc )
			{
				return Contains( m_Area, loc, FunctionalItemMargin );
			}

			public bool ContainsInstanceDestination( Map map, Point3D loc )
			{
				if ( !Object.ReferenceEquals( map, m_Map ) )
					return false;

				if ( Contains( loc ) )
					return true;

				if ( !ContainsFunctionalItem( loc ) )
					return false;

				return FindCloneableRegion( map, loc ) != null;
			}

				public DungeonInstanceDefinition( BaseRegion region )
				{
					m_Name = region.Name != null ? region.Name : region.ToString();
					m_Map = region.Map;
					m_MapName = m_Map != null && m_Map.Name != null ? m_Map.Name : "Unknown";
					m_DisplayName = String.Format( "{0}: {1}", m_MapName, m_Name );
					m_Landing = GetLandingFor( region );
					BaseRegion[] connectedRegions;
					m_Area = BuildCloneArea( region, out connectedRegions );
					m_Priority = region.Priority;
					m_SpawnZLevel = region.SpawnZLevel;
					m_Spawns = BuildSpawnsForConnectedRegions( connectedRegions );
				}

				private static Rectangle3D[] BuildCloneArea( BaseRegion root, out BaseRegion[] connectedRegions )
				{
					List<Rectangle3D> areas = new List<Rectangle3D>();
					List<BaseRegion> connected = new List<BaseRegion>();
					AddAreas( areas, root.Area );
					connected.Add( root );

				bool changed = true;
				int pass = 0;

					while ( changed && pass++ < 16 )
					{
						changed = false;
						changed |= AddTeleporterConnectedAreas( root, areas, connected );
						changed |= AddDecorationTeleporterConnectedAreas( root, areas, connected );

						foreach ( Item item in World.Items.Values )
						{
						if ( item == null || item.Deleted || item.Parent != null )
							continue;

						if ( !Object.ReferenceEquals( item.Map, root.Map ) || !Contains( areas, item.Location, FunctionalItemMargin ) )
							continue;

						Map destinationMap;
						Point3D destination;
						if ( !TryGetTeleportDestination( item, out destinationMap, out destination ) || destination == Point3D.Zero )
							continue;

						bool implicitSameMap = destinationMap == null || destinationMap == Map.Internal;
						destinationMap = ResolveDestinationMap( item, destinationMap );

						if ( !Object.ReferenceEquals( destinationMap, root.Map ) )
							continue;

						BaseRegion destinationRegion = FindCloneableRegion( destinationMap, destination );
						if ( ShouldAddConnectedRegion( root, destinationRegion ) )
							changed |= AddConnectedRegion( root, destinationRegion, connected, areas );
						else if ( implicitSameMap )
							changed |= AddTeleporterEndpointArea( areas, destination );
						}
					}

					connectedRegions = connected.ToArray();
					return areas.ToArray();
				}

			private static bool AddTeleporterConnectedAreas( BaseRegion root, List<Rectangle3D> areas, List<BaseRegion> connected )
			{
				bool changed = false;

				foreach ( Item item in World.Items.Values )
				{
					if ( item == null || item.Deleted || item.Parent != null || !Object.ReferenceEquals( item.Map, root.Map ) )
						continue;

					Map destinationMap;
					Point3D destination;
					if ( !TryGetTeleportDestination( item, out destinationMap, out destination ) || destination == Point3D.Zero )
						continue;

					bool implicitSameMap = destinationMap == null || destinationMap == Map.Internal;
					destinationMap = ResolveDestinationMap( item, destinationMap );

					if ( !Object.ReferenceEquals( destinationMap, root.Map ) )
						continue;

					BaseRegion sourceRegion = FindCloneableRegion( item.Map, item.Location );
					if ( sourceRegion == null || !ContainsRegion( connected, sourceRegion ) )
						continue;

					BaseRegion destinationRegion = FindCloneableRegion( destinationMap, destination );
					if ( ShouldAddConnectedRegion( root, destinationRegion ) )
						changed |= AddConnectedRegion( root, destinationRegion, connected, areas );
					else if ( implicitSameMap )
						changed |= AddTeleporterEndpointArea( areas, destination );
				}

					return changed;
				}

				private static bool AddDecorationTeleporterConnectedAreas( BaseRegion root, List<Rectangle3D> areas, List<BaseRegion> connected )
				{
					bool changed = false;
					List<DecorationTeleporterSource> sources = GetDecorationTeleporterSources( root.Map );

					if ( sources == null || sources.Count == 0 )
						return false;

					for ( int i = 0; i < sources.Count; i++ )
					{
						DecorationTeleporterSource source = sources[i];
						if ( source == null || !Object.ReferenceEquals( source.SourceMap, root.Map ) )
							continue;

						if ( !Contains( areas, source.Location, FunctionalItemMargin ) )
							continue;

						bool implicitSameMap = source.DestinationMap == null || source.DestinationMap == Map.Internal;
						Map destinationMap = ResolveDestinationMap( source.SourceMap, source.DestinationMap );

						if ( !Object.ReferenceEquals( destinationMap, root.Map ) )
							continue;

						BaseRegion destinationRegion = FindCloneableRegion( destinationMap, source.Destination );
						if ( ShouldAddConnectedRegion( root, destinationRegion ) )
							changed |= AddConnectedRegion( root, destinationRegion, connected, areas );
						else if ( implicitSameMap )
							changed |= AddTeleporterEndpointArea( areas, source.Destination );
					}

					return changed;
				}

				private static bool AddTeleporterEndpointArea( List<Rectangle3D> areas, Point3D destination )
				{
					Rectangle3D area = new Rectangle3D(
						new Point3D( destination.X - TeleporterEndpointMargin, destination.Y - TeleporterEndpointMargin, Region.MinZ ),
					new Point3D( destination.X + TeleporterEndpointMargin + 1, destination.Y + TeleporterEndpointMargin + 1, Region.MaxZ ) );

				if ( ContainsArea( areas, area ) )
					return false;

				areas.Add( area );
				return true;
			}

				private static SpawnEntry[] BuildSpawnsForConnectedRegions( BaseRegion[] connectedRegions )
				{
					List<SpawnEntry> spawns = new List<SpawnEntry>();
					HashSet<int> seen = new HashSet<int>();

					if ( connectedRegions == null )
						return spawns.ToArray();

					for ( int r = 0; r < connectedRegions.Length; r++ )
					{
						BaseRegion br = connectedRegions[r];
						if ( br == null || !IsDungeonCatalogRegion( br ) )
							continue;

						SpawnEntry[] entries = br.Spawns;
						if ( entries == null )
							continue;

						for ( int i = 0; i < entries.Length; i++ )
						{
							SpawnEntry entry = entries[i];
							if ( entry != null && seen.Add( entry.ID ) )
								spawns.Add( entry );
						}
					}

					return spawns.ToArray();
				}

			private static bool ShouldAddConnectedRegion( BaseRegion root, BaseRegion destinationRegion )
			{
				if ( root == null || destinationRegion == null )
					return false;

				if ( Object.ReferenceEquals( root, destinationRegion ) )
					return true;

				if ( destinationRegion.Priority < root.Priority && AreasIntersect( root.Area, destinationRegion.Area ) )
					return false;

				return true;
			}

			private static bool AddConnectedRegion( BaseRegion root, BaseRegion region, List<BaseRegion> connected, List<Rectangle3D> areas )
			{
				if ( !ShouldAddConnectedRegion( root, region ) )
					return false;

				bool changed = false;

				if ( !ContainsRegion( connected, region ) )
				{
					connected.Add( region );
					changed = true;
				}

				return AddAreas( areas, region.Area ) || changed;
			}

			private static bool ContainsRegion( List<BaseRegion> regions, BaseRegion region )
			{
				for ( int i = 0; i < regions.Count; i++ )
				{
					if ( Object.ReferenceEquals( regions[i], region ) )
						return true;
				}

				return false;
			}

			private static BaseRegion FindCloneableRegion( Map map, Point3D location )
			{
				BaseRegion best = null;

				foreach ( Region region in Region.Regions )
				{
					BaseRegion br = region as BaseRegion;
					if ( br == null || !Object.ReferenceEquals( br.Map, map ) || br.Area == null || br.Area.Length == 0 )
						continue;

					if ( !IsCloneableConnectedRegion( br ) || !Contains( br.Area, location ) )
						continue;

					if ( best == null || br.Priority > best.Priority )
						best = br;
				}

				return best;
			}

			private static bool IsDungeonCatalogRegion( BaseRegion region )
			{
				return region is DungeonRegion || region is BardDungeonRegion || region is CaveRegion || region is DeadRegion;
			}

			private static bool IsCloneableConnectedRegion( BaseRegion region )
			{
				return IsDungeonCatalogRegion( region )
					|| region is PublicRegion
					|| region is UnderHouseRegion
					|| region is SafeRegion;
			}

			private static bool AddAreas( List<Rectangle3D> areas, Rectangle3D[] add )
			{
				bool changed = false;

				if ( add == null )
					return false;

				for ( int i = 0; i < add.Length; i++ )
				{
					if ( ContainsArea( areas, add[i] ) )
						continue;

					areas.Add( add[i] );
					changed = true;
				}

				return changed;
			}

			private static bool ContainsArea( List<Rectangle3D> areas, Rectangle3D area )
			{
				for ( int i = 0; i < areas.Count; i++ )
				{
					Rectangle3D existing = areas[i];
					if ( existing.Start == area.Start && existing.End == area.End )
						return true;
				}

				return false;
			}

			private static bool Contains( List<Rectangle3D> areas, Point3D location )
			{
				return Contains( areas, location, 0 );
			}

			private static bool Contains( List<Rectangle3D> areas, Point3D location, int margin )
			{
				for ( int i = 0; i < areas.Count; i++ )
				{
					if ( Contains( areas[i], location, margin ) )
						return true;
				}

				return false;
			}

			private static bool Contains( Rectangle3D[] areas, Point3D location )
			{
				return Contains( areas, location, 0 );
			}

			private static bool Contains( Rectangle3D[] areas, Point3D location, int margin )
			{
				for ( int i = 0; i < areas.Length; i++ )
				{
					if ( Contains( areas[i], location, margin ) )
						return true;
				}

				return false;
			}

			private static bool Contains( Rectangle3D area, Point3D location, int margin )
			{
				if ( margin <= 0 )
					return area.Contains( location );

				return location.X >= area.Start.X - margin && location.X < area.End.X + margin
					&& location.Y >= area.Start.Y - margin && location.Y < area.End.Y + margin
					&& location.Z >= area.Start.Z - margin && location.Z < area.End.Z + margin;
			}

			private static bool AreasIntersect( Rectangle3D[] a, Rectangle3D[] b )
			{
				if ( a == null || b == null )
					return false;

				for ( int i = 0; i < a.Length; i++ )
				{
					for ( int j = 0; j < b.Length; j++ )
					{
						if ( RectsIntersect2D( a[i], b[j] ) )
							return true;
					}
				}

				return false;
			}

			private static bool RectsIntersect2D( Rectangle3D a, Rectangle3D b )
			{
				return a.Start.X < b.End.X && a.End.X > b.Start.X
					&& a.Start.Y < b.End.Y && a.End.Y > b.Start.Y;
			}

			private static Point3D GetLandingFor( BaseRegion region )
			{
				DungeonRegion dungeon = region as DungeonRegion;
				if ( dungeon != null && dungeon.EntranceMap == region.Map && dungeon.EntranceLocation != Point3D.Zero )
					return dungeon.EntranceLocation;

				if ( region.GoLocation != Point3D.Zero )
					return region.GoLocation;

				// 1. Try to find an overworld teleporter that drops players into this region
				foreach ( Item item in World.Items.Values )
				{
					Teleporter teleporter = item as Teleporter;
					if ( teleporter != null && teleporter.MapDest == region.Map && region.Contains( teleporter.PointDest ) )
						return teleporter.PointDest;
				}

				// 2. Fallback: find any teleporter inside this region (like an exit teleporter)
				foreach ( Item item in World.Items.Values )
				{
					Teleporter teleporter = item as Teleporter;
					if ( teleporter != null && teleporter.Map == region.Map && region.Contains( teleporter.Location ) )
						return teleporter.Location;
				}

				Rectangle3D rect = region.Area[0];
				int x = rect.Start.X + ( rect.Width / 2 );
				int y = rect.Start.Y + ( rect.Height / 2 );
				int z = region.Map != null ? region.Map.GetAverageZ( x, y ) : rect.Start.Z;

				return new Point3D( x, y, z );
				}
			}

			private sealed class DecorationTeleporterSource
			{
				public readonly Map SourceMap;
				public readonly Type Type;
				public readonly int ItemID;
				public readonly Point3D Location;
				public readonly Map DestinationMap;
				public readonly Point3D Destination;
				private readonly string[] m_Parameters;

				public DecorationTeleporterSource( Map sourceMap, Type type, int itemID, string[] parameters, Point3D location )
				{
					SourceMap = sourceMap;
					Type = type;
					ItemID = itemID;
					Location = location;
					m_Parameters = parameters != null ? parameters : EmptyStringArray;

					Map destinationMap;
					Point3D destination;
					ReadDestination( m_Parameters, out destinationMap, out destination );

					DestinationMap = destinationMap;
					Destination = destination;
				}

				public Item Construct()
				{
					Item item;

					try
					{
						item = (Item) Activator.CreateInstance( Type );
					}
					catch
					{
						return null;
					}

					if ( item == null )
						return null;

					if ( ItemID > 0 )
						item.ItemID = ItemID;

					for ( int i = 0; i < m_Parameters.Length; i++ )
					{
						try
						{
							ApplyParameter( item, m_Parameters[i] );
						}
						catch
						{
						}
					}

					item.Movable = false;
					item.Weight = -2;

					return item;
				}

				public Item ConstructExitTeleporter()
				{
					return new DungeonInstanceExitTeleporter( ItemID );
				}

				private static void ReadDestination( string[] parameters, out Map destinationMap, out Point3D destination )
				{
					destinationMap = null;
					destination = Point3D.Zero;

					for ( int i = 0; i < parameters.Length; i++ )
					{
						string name;
						string value;
						if ( !TrySplitDecorationParameter( parameters[i], out name, out value ) )
							continue;

						if ( String.Equals( name, "PointDest", StringComparison.OrdinalIgnoreCase ) )
							destination = ParseDecorationPoint( value );
						else if ( String.Equals( name, "MapDest", StringComparison.OrdinalIgnoreCase ) )
							destinationMap = ParseDecorationMap( value );
					}
				}

				private static void ApplyParameter( Item item, string parameter )
				{
					string name;
					string value;
					if ( !TrySplitDecorationParameter( parameter, out name, out value ) )
						return;

					QuestTransporter transporter = item as QuestTransporter;
					if ( transporter != null && ApplyQuestTransporterParameter( transporter, name, value ) )
						return;

					SkillTeleporter skill = item as SkillTeleporter;
					if ( skill != null && ApplySkillTeleporterParameter( skill, name, value ) )
						return;

					KeywordTeleporter keyword = item as KeywordTeleporter;
					if ( keyword != null && ApplyKeywordTeleporterParameter( keyword, name, value ) )
						return;

					Teleporter teleporter = item as Teleporter;
					if ( teleporter != null && ApplyTeleporterParameter( teleporter, name, value ) )
						return;

					moongates gate = item as moongates;
					if ( gate != null && ApplyMoongateParameter( gate, name, value ) )
						return;

					QuestTeleporter quest = item as QuestTeleporter;
					if ( quest != null && ApplyQuestTeleporterParameter( quest, name, value ) )
						return;

					ApplyItemParameter( item, name, value );
				}

				private static bool ApplyQuestTransporterParameter( QuestTransporter transporter, string name, string value )
				{
					if ( String.Equals( name, "TeleportName", StringComparison.OrdinalIgnoreCase ) )
					{
						transporter.TeleportName = value;
						return true;
					}

					if ( String.Equals( name, "Required", StringComparison.OrdinalIgnoreCase ) )
					{
						transporter.Required = value;
						return true;
					}

					if ( String.Equals( name, "MessageString", StringComparison.OrdinalIgnoreCase ) )
					{
						transporter.MessageString = value;
						return true;
					}

					return false;
				}

				private static bool ApplySkillTeleporterParameter( SkillTeleporter skill, string name, string value )
				{
					if ( String.Equals( name, "Skill", StringComparison.OrdinalIgnoreCase ) )
					{
						skill.Skill = (SkillName) Enum.Parse( typeof( SkillName ), value, true );
						return true;
					}

					if ( String.Equals( name, "RequiredFixedPoint", StringComparison.OrdinalIgnoreCase ) )
					{
						skill.Required = Utility.ToInt32( value ) * 0.01;
						return true;
					}

					if ( String.Equals( name, "Required", StringComparison.OrdinalIgnoreCase ) )
					{
						skill.Required = Utility.ToDouble( value );
						return true;
					}

					if ( String.Equals( name, "MessageString", StringComparison.OrdinalIgnoreCase ) )
					{
						skill.MessageString = value;
						return true;
					}

					if ( String.Equals( name, "MessageNumber", StringComparison.OrdinalIgnoreCase ) )
					{
						skill.MessageNumber = Utility.ToInt32( value );
						return true;
					}

					return false;
				}

				private static bool ApplyKeywordTeleporterParameter( KeywordTeleporter keyword, string name, string value )
				{
					if ( String.Equals( name, "Substring", StringComparison.OrdinalIgnoreCase ) )
					{
						keyword.Substring = value;
						return true;
					}

					if ( String.Equals( name, "Keyword", StringComparison.OrdinalIgnoreCase ) )
					{
						keyword.Keyword = Utility.ToInt32( value );
						return true;
					}

					if ( String.Equals( name, "Range", StringComparison.OrdinalIgnoreCase ) )
					{
						keyword.Range = Utility.ToInt32( value );
						return true;
					}

					return false;
				}

				private static bool ApplyTeleporterParameter( Teleporter teleporter, string name, string value )
				{
					if ( String.Equals( name, "PointDest", StringComparison.OrdinalIgnoreCase ) )
					{
						teleporter.PointDest = ParseDecorationPoint( value );
						return true;
					}

					if ( String.Equals( name, "MapDest", StringComparison.OrdinalIgnoreCase ) )
					{
						teleporter.MapDest = ParseDecorationMap( value );
						return true;
					}

					if ( String.Equals( name, "Creatures", StringComparison.OrdinalIgnoreCase ) )
					{
						teleporter.Creatures = Utility.ToBoolean( value );
						return true;
					}

					if ( String.Equals( name, "SourceEffect", StringComparison.OrdinalIgnoreCase ) )
					{
						teleporter.SourceEffect = Utility.ToBoolean( value );
						return true;
					}

					if ( String.Equals( name, "DestEffect", StringComparison.OrdinalIgnoreCase ) )
					{
						teleporter.DestEffect = Utility.ToBoolean( value );
						return true;
					}

					if ( String.Equals( name, "SoundID", StringComparison.OrdinalIgnoreCase ) )
					{
						teleporter.SoundID = Utility.ToInt32( value );
						return true;
					}

					if ( String.Equals( name, "Delay", StringComparison.OrdinalIgnoreCase ) )
					{
						teleporter.Delay = TimeSpan.Parse( value );
						return true;
					}

					if ( String.Equals( name, "Active", StringComparison.OrdinalIgnoreCase ) )
					{
						teleporter.Active = Utility.ToBoolean( value );
						return true;
					}

					if ( String.Equals( name, "CombatCheck", StringComparison.OrdinalIgnoreCase ) )
					{
						teleporter.CombatCheck = Utility.ToBoolean( value );
						return true;
					}

					return false;
				}

				private static bool ApplyMoongateParameter( moongates gate, string name, string value )
				{
					if ( String.Equals( name, "PointDest", StringComparison.OrdinalIgnoreCase ) )
					{
						gate.PointDest = ParseDecorationPoint( value );
						return true;
					}

					if ( String.Equals( name, "MapDest", StringComparison.OrdinalIgnoreCase ) )
					{
						gate.MapDest = ParseDecorationMap( value );
						return true;
					}

					if ( String.Equals( name, "Creatures", StringComparison.OrdinalIgnoreCase ) )
					{
						gate.Creatures = Utility.ToBoolean( value );
						return true;
					}

					if ( String.Equals( name, "SourceEffect", StringComparison.OrdinalIgnoreCase ) )
					{
						gate.SourceEffect = Utility.ToBoolean( value );
						return true;
					}

					if ( String.Equals( name, "DestEffect", StringComparison.OrdinalIgnoreCase ) )
					{
						gate.DestEffect = Utility.ToBoolean( value );
						return true;
					}

					if ( String.Equals( name, "SoundID", StringComparison.OrdinalIgnoreCase ) )
					{
						gate.SoundID = Utility.ToInt32( value );
						return true;
					}

					if ( String.Equals( name, "Delay", StringComparison.OrdinalIgnoreCase ) )
					{
						gate.Delay = TimeSpan.Parse( value );
						return true;
					}

					if ( String.Equals( name, "Active", StringComparison.OrdinalIgnoreCase ) )
					{
						gate.Active = Utility.ToBoolean( value );
						return true;
					}

					if ( String.Equals( name, "CombatCheck", StringComparison.OrdinalIgnoreCase ) )
					{
						gate.CombatCheck = Utility.ToBoolean( value );
						return true;
					}

					return false;
				}

				private static bool ApplyQuestTeleporterParameter( QuestTeleporter quest, string name, string value )
				{
					if ( String.Equals( name, "PointDest", StringComparison.OrdinalIgnoreCase ) )
					{
						quest.TeleporterPointDest = ParseDecorationPoint( value );
						return true;
					}

					if ( String.Equals( name, "MapDest", StringComparison.OrdinalIgnoreCase ) )
					{
						quest.TeleporterMapDest = ParseDecorationMap( value );
						return true;
					}

					if ( String.Equals( name, "TeleporterOpen", StringComparison.OrdinalIgnoreCase ) || String.Equals( name, "Teleporter_Open", StringComparison.OrdinalIgnoreCase ) )
					{
						quest.TeleporterOpen = Utility.ToInt32( value );
						return true;
					}

					if ( String.Equals( name, "TeleporterSound", StringComparison.OrdinalIgnoreCase ) || String.Equals( name, "Teleporter_Sound", StringComparison.OrdinalIgnoreCase ) )
					{
						quest.TeleporterSound = Utility.ToInt32( value );
						return true;
					}

					if ( String.Equals( name, "TeleporterItem", StringComparison.OrdinalIgnoreCase ) || String.Equals( name, "Teleporter_Item", StringComparison.OrdinalIgnoreCase ) )
					{
						quest.TeleporterItem = Utility.ToInt32( value );
						return true;
					}

					if ( String.Equals( name, "TeleporterMessage", StringComparison.OrdinalIgnoreCase ) || String.Equals( name, "Teleporter_Message", StringComparison.OrdinalIgnoreCase ) )
					{
						quest.TeleporterMessage = value;
						return true;
					}

					if ( String.Equals( name, "TeleporterFail", StringComparison.OrdinalIgnoreCase ) || String.Equals( name, "Teleporter_Fail", StringComparison.OrdinalIgnoreCase ) )
					{
						quest.TeleporterFail = value;
						return true;
					}

					if ( String.Equals( name, "TeleporterQuest", StringComparison.OrdinalIgnoreCase ) || String.Equals( name, "Teleporter_Quest", StringComparison.OrdinalIgnoreCase ) )
					{
						quest.TeleporterQuest = value;
						return true;
					}

					if ( String.Equals( name, "TeleporterLock", StringComparison.OrdinalIgnoreCase ) || String.Equals( name, "Teleporter_Lock", StringComparison.OrdinalIgnoreCase ) )
					{
						quest.TeleporterLock = value;
						return true;
					}

					if ( String.Equals( name, "TeleporterLockMsg", StringComparison.OrdinalIgnoreCase ) || String.Equals( name, "Teleporter_LockMsg", StringComparison.OrdinalIgnoreCase ) )
					{
						quest.TeleporterLockMsg = value;
						return true;
					}

					return false;
				}

				private static void ApplyItemParameter( Item item, string name, string value )
				{
					if ( String.Equals( name, "Light", StringComparison.OrdinalIgnoreCase ) )
					{
						item.Light = (LightType) Enum.Parse( typeof( LightType ), value, true );
					}
					else if ( String.Equals( name, "Hue", StringComparison.OrdinalIgnoreCase ) )
					{
						item.Hue = Utility.ToInt32( value );
					}
					else if ( String.Equals( name, "Name", StringComparison.OrdinalIgnoreCase ) )
					{
						item.Name = value;
					}
					else if ( String.Equals( name, "Visible", StringComparison.OrdinalIgnoreCase ) )
					{
						item.Visible = Utility.ToBoolean( value );
					}
					else if ( String.Equals( name, "Movable", StringComparison.OrdinalIgnoreCase ) )
					{
						item.Movable = Utility.ToBoolean( value );
					}
					else if ( String.Equals( name, "Amount", StringComparison.OrdinalIgnoreCase ) )
					{
						item.Amount = Utility.ToInt32( value );
					}
				}
			}

			private sealed class DungeonInstanceSettings
			{
				public readonly DungeonInstanceDefinition Definition;
			public readonly Difficulty Difficulty;
			public readonly Map ReturnMap;
			public readonly Point3D ReturnPoint;

			public DungeonInstanceSettings( DungeonInstanceDefinition definition, Difficulty difficulty )
				: this( definition, difficulty, null, Point3D.Zero )
			{
			}

			public DungeonInstanceSettings( DungeonInstanceDefinition definition, Difficulty difficulty, Map returnMap, Point3D returnPoint )
			{
				Definition = definition;
				Difficulty = ClampDifficulty( difficulty );
				ReturnMap = NormalizeExternalMap( returnMap );
				ReturnPoint = returnPoint;
			}

			public bool Matches( DungeonInstanceSettings other )
			{
				return other != null && Object.ReferenceEquals( Definition, other.Definition ) && Difficulty == other.Difficulty;
			}
		}

		private sealed class DungeonReturnInfo
		{
			public readonly Serial InstanceKey;
			public readonly InstanceOwnerKind OwnerKind;
			public readonly Map ReturnMap;
			public readonly Point3D ReturnPoint;
			public readonly DateTime LastTouched;

			public DungeonReturnInfo( Serial instanceKey, InstanceOwnerKind ownerKind, Map returnMap, Point3D returnPoint, DateTime lastTouched )
			{
				InstanceKey = instanceKey;
				OwnerKind = ownerKind;
				ReturnMap = returnMap;
				ReturnPoint = returnPoint;
				LastTouched = lastTouched;
			}
		}

		private sealed class RuntimeDungeonRegion : BaseRegion
		{
			private readonly DungeonInstanceType m_Type;
			private readonly int m_MapIndex;

			public RuntimeDungeonRegion( DungeonInstanceType type, Instance inst, DungeonInstanceSettings settings, Map map )
				: base(
					String.Format( "{0} Instance {1}", settings.Definition.Name, (int)inst.OwnerSerial ),
					map,
					settings.Definition.Priority + RuntimeRegionPriorityOffset,
					settings.Definition.Area )
			{
				m_Type = type;
				m_MapIndex = map.MapIndex;
				SpawnZLevel = settings.Definition.SpawnZLevel;
				RuneName = settings.Definition.Name;
				GoLocation = settings.Definition.Landing;
			}

			public override bool YoungProtected { get { return false; } }

			public override bool AllowHousing( Mobile from, Point3D p )
			{
				return false;
			}

			public override void OnEnter( Mobile m )
			{
				base.OnEnter( m );

				PlayerMobile pm = m as PlayerMobile;
				if ( pm != null )
					m_Type.OnPlayerEnteredMap( m_MapIndex, pm );
			}
		}

		private sealed class RuntimeDungeonState
		{
			private readonly DungeonInstanceSettings m_Settings;
			private readonly RuntimeDungeonRegion m_Region;
			private readonly SpawnEntry[] m_Entries;
			private readonly Map m_Map;
			private readonly Rectangle3D[] m_Area;
			private readonly HashSet<Serial> m_Scaled = new HashSet<Serial>();
			private readonly List<Mobile> m_Mobiles = new List<Mobile>();
			private Timer m_Timer;

			public RuntimeDungeonState( DungeonInstanceSettings settings, RuntimeDungeonRegion region, SpawnEntry[] entries, Map map, Rectangle3D[] area )
			{
				m_Settings = settings;
				m_Region = region;
				m_Entries = entries;
				m_Map = map;
				m_Area = area;
			}

			public void StartScaler()
			{
				if ( m_Timer != null )
					return;

				m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), TimeSpan.FromSeconds( 5.0 ), new TimerCallback( ApplyDifficulty ) );
			}

			public void ApplyDifficulty()
			{
				for ( int i = 0; i < m_Entries.Length; i++ )
				{
					SpawnEntry entry = m_Entries[i];
					if ( entry == null )
						continue;

					List<ISpawnable> spawned = entry.SpawnedObjects;
					for ( int j = 0; j < spawned.Count; j++ )
					{
						Mobile m = spawned[j] as Mobile;
						if ( m == null || m.Deleted )
							continue;

						ScaleMobile( m );
					}
				}

				ScaleMobilesInArea();
			}

			private void ScaleMobilesInArea()
			{
				if ( m_Map == null || m_Area == null )
					return;

				for ( int i = 0; i < m_Area.Length; i++ )
				{
					Rectangle3D rect = m_Area[i];
					Rectangle2D bounds = new Rectangle2D( rect.Start.X, rect.Start.Y, rect.Width, rect.Height );

					IPooledEnumerable eable = m_Map.GetMobilesInBounds( bounds );
					try
					{
						foreach ( Mobile m in eable )
							ScaleMobile( m );
					}
					finally
					{
						eable.Free();
					}
				}
			}

			private void ScaleMobile( Mobile m )
			{
				if ( m == null || m.Deleted || m.Player )
					return;

				BaseCreature bc = m as BaseCreature;
				if ( bc == null || bc.Controlled || bc.Summoned || bc.ControlMaster != null || bc.SummonMaster != null )
					return;

				if ( !m_Mobiles.Contains( m ) )
					m_Mobiles.Add( m );

				if ( m_Scaled.Contains( bc.Serial ) )
					return;

				BaseCreature.BeefUp( bc, m_Settings.Difficulty, false );
				m_Scaled.Add( bc.Serial );
			}

			public void Delete()
			{
				if ( m_Timer != null )
				{
					m_Timer.Stop();
					m_Timer = null;
				}

				if ( m_Region != null && m_Region.Registered )
					m_Region.Unregister();
				else
				{
					for ( int i = 0; i < m_Entries.Length; i++ )
					{
						if ( m_Entries[i] != null )
							m_Entries[i].Delete();
					}
				}

				for ( int i = 0; i < m_Mobiles.Count; i++ )
				{
					Mobile m = m_Mobiles[i];
					if ( m != null && !m.Deleted )
						m.Delete();
				}

				m_Mobiles.Clear();
				m_Scaled.Clear();
			}
		}
	}
}
