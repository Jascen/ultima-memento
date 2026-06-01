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
				from.SendMessage( "[dungeoninstance subcommands: status | placeentrance" );
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
				case "placeentrance":
					{
						DungeonEntrance entrance = new DungeonEntrance();
						entrance.MoveToWorld( from.Location, from.Map );
						from.SendMessage( "Placed a dungeon entrance here." );
						break;
					}
				default:
					from.SendMessage( "Unknown subcommand. Try: status, placeentrance" );
					break;
			}
		}
	}
}
