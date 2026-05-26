using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.Instancing
{
	public static class SkyInstanceCommands
	{
		public static void Configure()
		{
			CommandSystem.Register( "skydwelling", AccessLevel.Player, new CommandEventHandler( OnSkyDwellingCommand ) );
			CommandSystem.Register( "leavedwelling", AccessLevel.Player, new CommandEventHandler( OnLeaveDwellingCommand ) );
			CommandSystem.Register( "skyinstance", AccessLevel.GameMaster, new CommandEventHandler( OnAdminCommand ) );
		}

		private static void OnSkyDwellingCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			SkyInstanceManager.SendOwnerToTheirInstance( from );
		}

		private static void OnLeaveDwellingCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if ( !SkyInstanceManager.LeaveDwelling( from ) )
				from.SendMessage( "You are not currently in a sky dwelling." );
		}

		private static void OnAdminCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( e.Arguments.Length == 0 )
			{
				from.SendMessage( "[skyinstance subcommands: status | unload <minutes> | despawn | gotoplayer" );
				return;
			}

			string sub = e.Arguments[0].ToLower();

			switch ( sub )
			{
				case "status":
					{
						from.SendMessage( "Sky Instance status: {0} allocated / {1} capacity (unload after {2} min).",
							SkyInstanceManager.AllocatedCount,
							SkyInstanceManager.MaxSlots,
							(int)SkyInstanceManager.UnloadAfter.TotalMinutes );
						int loaded = 0;
						foreach ( SkyInstance inst in SkyInstanceManager.AllInstances )
							if ( inst.Loaded ) loaded++;
						from.SendMessage( "Currently materialized: {0}.", loaded );
						break;
					}
				case "unload":
					{
						if ( e.Arguments.Length < 2 )
						{
							from.SendMessage( "Usage: [skyinstance unload <minutes>" );
							return;
						}
						int minutes;
						if ( !Int32.TryParse( e.Arguments[1], out minutes ) || minutes < 1 )
						{
							from.SendMessage( "Bad minutes value." );
							return;
						}
						SkyInstanceManager.UnloadAfter = TimeSpan.FromMinutes( minutes );
						from.SendMessage( "Sky dwellings will now unload after {0} minutes of inactivity.", minutes );
						break;
					}
				case "despawn":
					{
						int count = 0;
						foreach ( SkyInstance inst in SkyInstanceManager.AllInstances )
						{
							if ( inst.Loaded )
							{
								SkyInstanceManager.Despawn( inst );
								count++;
							}
						}
						from.SendMessage( "Force-despawned {0} loaded instances.", count );
						break;
					}
				case "gotoplayer":
					{
						from.SendMessage( "Target the player whose dwelling you want to visit." );
						from.Target = new VisitTarget();
						break;
					}
				default:
					from.SendMessage( "Unknown subcommand. Try: status, unload, despawn, gotoplayer" );
					break;
			}
		}

		private class VisitTarget : Target
		{
			public VisitTarget() : base( 12, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				PlayerMobile target = targeted as PlayerMobile;
				if ( target == null )
				{
					from.SendMessage( "That is not a player." );
					return;
				}

				SkyInstance inst = SkyInstanceManager.GetOrCreate( target );
				if ( inst == null )
				{
					from.SendMessage( "Could not allocate a dwelling for that player." );
					return;
				}

				if ( !inst.Loaded )
					SkyInstanceManager.Materialize( inst );

				Server.Mobiles.BaseCreature.TeleportPets( from, SkyInstanceManager.GetLandingPoint( inst.Id ), SkyInstanceManager.InstanceMap, false );
				from.MoveToWorld( SkyInstanceManager.GetLandingPoint( inst.Id ), SkyInstanceManager.InstanceMap );
				from.SendMessage( "You arrive in {0}'s sky dwelling (#{1}).", target.Name, inst.Id );
			}
		}
	}
}
