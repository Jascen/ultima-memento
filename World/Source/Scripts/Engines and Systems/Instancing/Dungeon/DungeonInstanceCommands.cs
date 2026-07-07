using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.Instancing
{
	public static class DungeonInstanceCommands
	{
		private const int SpawnAllColumns = 20;
		private const int SpawnAllRows = 10;

		private static DungeonInstanceType Dungeon { get { return DungeonInstanceType.Instance; } }

		public static void Configure()
		{
			Dungeon.Configure();

			CommandSystem.Register( "dungeon", AccessLevel.Player, new CommandEventHandler( OnEnterCommand ) );
			CommandSystem.Register( "leavedungeon", AccessLevel.Player, new CommandEventHandler( OnLeaveCommand ) );
			CommandSystem.Register( "dungeoninstance", AccessLevel.GameMaster, new CommandEventHandler( OnAdminCommand ) );
		}

		private static void OnEnterCommand( CommandEventArgs e )
		{
			Dungeon.SendOwnerToTheirInstance( e.Mobile );
		}

		private static void OnLeaveCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if ( !Dungeon.LeaveInstance( from ) )
				from.SendMessage( "You are not currently in a dungeon instance." );
		}

		private static void OnAdminCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

				if ( e.Arguments.Length == 0 )
				{
					from.SendMessage( "[dungeoninstance subcommands: status | list [filter] | placeentrance | spawnall | clearall" );
					return;
				}

			switch ( e.Arguments[0].ToLower() )
			{
				case "status":
					{
						from.SendMessage( "Dungeon instances: {0} record(s), {1} live / {2} pool maps, unload after {3} min.",
							Dungeon.OwnerCount, Dungeon.LiveCount, Dungeon.PoolSize,
							(int)Dungeon.UnloadAfter.TotalMinutes );
						break;
					}
				case "list":
					{
						string filter = e.Arguments.Length > 1 ? String.Join( " ", e.Arguments, 1, e.Arguments.Length - 1 ) : null;
						List<DungeonInstanceType.DungeonInstanceDefinition> defs = Dungeon.Definitions;
						int shown = 0;
						int matched = 0;

						for ( int i = 0; i < defs.Count; i++ )
						{
							DungeonInstanceType.DungeonInstanceDefinition def = defs[i];
							if ( filter != null && filter.Length > 0 &&
								def.DisplayName.IndexOf( filter, StringComparison.OrdinalIgnoreCase ) < 0 )
								continue;

							matched++;

							if ( shown < 40 )
							{
								if ( def.CanSpawnInstance )
									from.SendMessage( "{0}: {1} ({2} spawn entries) [{3}]", def.Index, def.DisplayName, def.SpawnCount, def.AvailabilityLabel );
								else
									from.SendMessage( "{0}: {1} ({2} spawn entries) [{3}: {4}]", def.Index, def.DisplayName, def.SpawnCount, def.AvailabilityLabel, def.InstanceBlockReason );

								shown++;
							}
						}

						if ( matched > shown )
							from.SendMessage( "Showing {0} of {1} matches. Use a filter to narrow the list.", shown, matched );
						else
							from.SendMessage( "Matched {0} dungeon(s).", matched );

						break;
					}
					case "placeentrance":
						{
							DungeonInstanceGate gate = new DungeonInstanceGate();
							gate.MoveToWorld( from.Location, from.Map );
							from.SendMessage( "Placed a dungeon instance gate here." );
							break;
						}
					case "spawnall":
						{
							from.SendMessage( "Target the northwest corner tile for a 20-column grouped dungeon instance gate test grid." );
							from.Target = new SpawnAllTarget();
							break;
						}
					case "clearall":
						{
							int count = Dungeon.ClearAllInstances();
							from.SendMessage( "Cleared {0} dungeon instance(s).", count );
							break;
						}
					default:
						from.SendMessage( "Unknown subcommand. Try: status, list, placeentrance, spawnall, clearall" );
						break;
				}
		}

		private class SpawnAllTarget : Target
		{
			public SpawnAllTarget() : base( -1, true, TargetFlags.None )
			{
				CheckLOS = false;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( from == null || from.Deleted )
					return;

				IPoint3D point = targeted as IPoint3D;
				if ( point == null )
				{
					from.SendMessage( "That is not a valid location." );
					return;
				}

				Map map = from.Map;
				if ( map == null || map == Map.Internal )
				{
					from.SendMessage( "You cannot place dungeon instance gates on this map." );
					return;
				}

				List<DungeonInstanceType.DungeonInstanceDefinition> defs = Dungeon.Definitions;
				if ( defs == null || defs.Count == 0 )
				{
					from.SendMessage( "No dungeon instance definitions are available." );
					return;
				}

				Point3D start = new Point3D( point );
				List<DungeonInstanceType.DungeonInstanceDefinition> good = new List<DungeonInstanceType.DungeonInstanceDefinition>();
				List<DungeonInstanceType.DungeonInstanceDefinition> badEntranceOrExit = new List<DungeonInstanceType.DungeonInstanceDefinition>();
				List<DungeonInstanceType.DungeonInstanceDefinition> broken = new List<DungeonInstanceType.DungeonInstanceDefinition>();

				for ( int i = 0; i < defs.Count; i++ )
				{
					DungeonInstanceType.DungeonInstanceDefinition def = defs[i];
					if ( def == null )
						continue;

					switch ( def.Availability )
					{
						case DungeonInstanceType.DungeonInstanceAvailability.BadEntranceOrExit:
							badEntranceOrExit.Add( def );
							break;
						case DungeonInstanceType.DungeonInstanceAvailability.Broken:
							broken.Add( def );
							break;
						default:
							good.Add( def );
							break;
					}
				}

				int row = 0;
				int goodPlaced = PlaceGroup( good, start, row, map );
				row += RowsFor( good.Count );

				if ( badEntranceOrExit.Count > 0 && row > 0 )
					row++;

				int badEntranceOrExitPlaced = PlaceGroup( badEntranceOrExit, start, row, map );
				row += RowsFor( badEntranceOrExit.Count );

				if ( broken.Count > 0 && row > 0 )
					row++;

				int brokenPlaced = PlaceGroup( broken, start, row, map );
				row += RowsFor( broken.Count );

				from.SendMessage( "Placed {0} good, {1} bad entrance/exit, and {2} broken dungeon instance gate(s).", goodPlaced, badEntranceOrExitPlaced, brokenPlaced );
				from.SendMessage( "Gate hues: good {0}, bad entrance/exit {1}, broken {2}.", DungeonInstanceType.GoodDungeonGateHue, DungeonInstanceType.BadEntranceOrExitDungeonGateHue, DungeonInstanceType.BrokenDungeonGateHue );

				if ( row > SpawnAllRows )
					from.SendMessage( "Grouped grid uses {0} row(s), which is larger than the old {1}-row footprint.", row, SpawnAllRows );
			}
			private static int PlaceGroup( List<DungeonInstanceType.DungeonInstanceDefinition> defs, Point3D start, int startRow, Map map )
			{
				if ( defs == null || defs.Count == 0 )
					return 0;

				for ( int i = 0; i < defs.Count; i++ )
				{
					DungeonInstanceType.DungeonInstanceDefinition def = defs[i];
					if ( def == null )
						continue;

					int x = start.X + ( i % SpawnAllColumns );
					int y = start.Y + startRow + ( i / SpawnAllColumns );

					DungeonInstanceGate gate = new DungeonInstanceGate();
					gate.DungeonIndex = def.Index;
					gate.Hue = DungeonInstanceType.GetAvailabilityHue( def.Availability );
					gate.MoveToWorld( new Point3D( x, y, start.Z ), map );
				}

				return defs.Count;
			}

			private static int RowsFor( int count )
			{
				if ( count <= 0 )
					return 0;

				return ( ( count - 1 ) / SpawnAllColumns ) + 1;
			}
		}
	}
}
