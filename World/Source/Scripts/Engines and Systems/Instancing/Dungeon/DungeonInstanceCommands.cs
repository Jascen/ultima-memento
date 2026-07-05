using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Engines.Instancing
{
	public static class DungeonInstanceCommands
	{
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
					from.SendMessage( "[dungeoninstance subcommands: status | list [filter] | placeentrance | clearall" );
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
								from.SendMessage( "{0}: {1} ({2} spawn entries)", def.Index, def.DisplayName, def.SpawnCount );
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
					case "clearall":
						{
							int count = Dungeon.ClearAllInstances();
							from.SendMessage( "Cleared {0} dungeon instance(s).", count );
							break;
						}
					default:
						from.SendMessage( "Unknown subcommand. Try: status, list, placeentrance, clearall" );
						break;
				}
		}
	}
}
