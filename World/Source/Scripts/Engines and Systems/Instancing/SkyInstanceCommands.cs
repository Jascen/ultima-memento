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
			CommandSystem.Register( "skyfriend", AccessLevel.Player, new CommandEventHandler( OnSkyFriendCommand ) );
			CommandSystem.Register( "skyvisit", AccessLevel.Player, new CommandEventHandler( OnSkyVisitCommand ) );
			CommandSystem.Register( "skyinstance", AccessLevel.GameMaster, new CommandEventHandler( OnAdminCommand ) );
		}

		private static void OnSkyFriendCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( e.Arguments.Length == 0 )
			{
				from.SendMessage( "Usage: [skyfriend add | remove | list" );
				return;
			}

			string sub = e.Arguments[0].ToLower();
			switch ( sub )
			{
				case "list":
				{
					SkyInstance inst = SkyInstanceManager.GetByOwner( from );
					if ( inst == null || inst.Friends.Count == 0 )
					{
						from.SendMessage( "You have not invited anyone to your sky dwelling." );
						return;
					}
					from.SendMessage( "Friends invited to your sky dwelling:" );
					for ( int i = 0; i < inst.Friends.Count; i++ )
					{
						Mobile f = World.FindMobile( inst.Friends[i] );
						from.SendMessage( " - {0}", f != null ? f.Name : String.Format( "(deleted, serial 0x{0:X})", (int)inst.Friends[i] ) );
					}
					break;
				}
				case "add":
				{
					from.SendMessage( "Target the player to invite to your sky dwelling." );
					from.Target = new FriendTarget( true );
					break;
				}
				case "remove":
				{
					from.SendMessage( "Target the player to un-invite from your sky dwelling." );
					from.Target = new FriendTarget( false );
					break;
				}
				default:
					from.SendMessage( "Usage: [skyfriend add | remove | list" );
					break;
			}
		}

		private static void OnSkyVisitCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.SendMessage( "Target the player whose sky dwelling you want to visit." );
			from.Target = new SkyVisitTarget();
		}

		private class FriendTarget : Target
		{
			private readonly bool m_Add;
			public FriendTarget( bool add ) : base( 12, false, TargetFlags.None )
			{
				m_Add = add;
			}
			protected override void OnTarget( Mobile from, object o )
			{
				PlayerMobile target = o as PlayerMobile;
				if ( target == null ) { from.SendMessage( "That is not a player." ); return; }
				if ( target == from ) { from.SendMessage( "You cannot {0} yourself.", m_Add ? "invite" : "un-invite" ); return; }

				if ( m_Add )
				{
					if ( SkyInstanceManager.AddFriend( from, target ) )
						from.SendMessage( "{0} can now visit your sky dwelling.", target.Name );
					else
						from.SendMessage( "{0} is already invited.", target.Name );
				}
				else
				{
					if ( SkyInstanceManager.RemoveFriend( from, target ) )
						from.SendMessage( "{0} can no longer visit your sky dwelling.", target.Name );
					else
						from.SendMessage( "{0} was not on your invite list.", target.Name );
				}
			}
		}

		private class SkyVisitTarget : Target
		{
			public SkyVisitTarget() : base( 12, false, TargetFlags.None ) { }
			protected override void OnTarget( Mobile from, object o )
			{
				PlayerMobile target = o as PlayerMobile;
				if ( target == null ) { from.SendMessage( "That is not a player." ); return; }
				SkyInstanceManager.VisitFriendDwelling( from, target );
			}
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
				from.SendMessage( "[skyinstance subcommands: status | unload <minutes> | despawn | gotoplayer | placeportal | freedead" );
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
				case "placeportal":
					{
						SkyDwellingPortal portal = new SkyDwellingPortal();
						portal.MoveToWorld( from.Location, from.Map );
						from.SendMessage( "Placed a sky dwelling portal here." );
						break;
					}
				case "freedead":
					{
						int freed = SkyInstanceManager.ReleaseDeadOwners();
						from.SendMessage( "Freed {0} orphaned slot(s).", freed );
						break;
					}
				default:
					from.SendMessage( "Unknown subcommand. Try: status, unload, despawn, gotoplayer, placeportal, freedead" );
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
