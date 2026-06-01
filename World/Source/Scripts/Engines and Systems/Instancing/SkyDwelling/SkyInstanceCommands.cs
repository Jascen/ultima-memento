using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.Instancing
{
	public static class SkyInstanceCommands
	{
		// Convenience handle to the sky-dwelling system.
		private static SkyDwellingInstanceType Sky { get { return SkyDwellingInstanceType.Instance; } }

		// Swap any already-placed plain climb teleporters that lead to the instanced
		// sky dwelling for the chooser version, so a live world picks up the new gump
		// without a full re-decoration. New worlds get it straight from the cfg.
		private static int InstallChooserRopes()
		{
			Point3D dest = new Point3D( 1974, 1977, 0 );

			List<KeywordTeleporter> targets = new List<KeywordTeleporter>();
			foreach ( Item it in World.Items.Values )
			{
				if ( it == null || it.Deleted ) continue;
				if ( it.GetType() != typeof( KeywordTeleporter ) ) continue; // not the subclass

				KeywordTeleporter kt = (KeywordTeleporter)it;
				if ( kt.MapDest == Map.SerpentIsland && kt.PointDest == dest )
					targets.Add( kt );
			}

			foreach ( KeywordTeleporter kt in targets )
			{
				SkyDwellingTeleporter rep = new SkyDwellingTeleporter();
				rep.Substring = kt.Substring;
				rep.Keyword   = kt.Keyword;
				rep.Range     = kt.Range;
				rep.PointDest = kt.PointDest;
				rep.MapDest   = kt.MapDest;
				rep.Active    = kt.Active;
				rep.ItemID    = kt.ItemID;
				rep.Hue       = kt.Hue;

				rep.MoveToWorld( kt.Location, kt.Map );
				kt.Delete();
			}

			return targets.Count;
		}

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
					Instance inst = Sky.GetByOwner( from );
					if ( inst == null || inst.Members.Count == 0 )
					{
						from.SendMessage( "You have not invited anyone to your sky dwelling." );
						return;
					}
					from.SendMessage( "Friends invited to your sky dwelling:" );
					for ( int i = 0; i < inst.Members.Count; i++ )
					{
						Mobile f = World.FindMobile( inst.Members[i] );
						from.SendMessage( " - {0}", f != null ? f.Name : String.Format( "(deleted, serial 0x{0:X})", (int)inst.Members[i] ) );
					}
					break;
				}
				case "add":
				{
					if ( !Sky.OwnsDwelling( from ) )
					{
						from.SendMessage( "You do not own a sky dwelling. Purchase one from a sky dwelling sign." );
						return;
					}
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
					if ( Sky.AddFriend( from, target ) )
						from.SendMessage( "{0} can now visit your sky dwelling.", target.Name );
					else
						from.SendMessage( "{0} is already invited.", target.Name );
				}
				else
				{
					if ( Sky.RemoveFriend( from, target ) )
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
				Sky.VisitFriendDwelling( from, target );
			}
		}

		private static void OnSkyDwellingCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			Sky.SendOwnerToTheirInstance( from );
		}

		private static void OnLeaveDwellingCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if ( !Sky.LeaveInstance( from ) )
				from.SendMessage( "You are not currently in a sky dwelling." );
		}

		private static void OnAdminCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( e.Arguments.Length == 0 )
			{
				from.SendMessage( "[skyinstance subcommands: status | price <gold> | unload <minutes> | park | grant | gotoplayer | placeportal | freedead | installrope" );
				return;
			}

			string sub = e.Arguments[0].ToLower();

			switch ( sub )
			{
				case "status":
					{
						from.SendMessage( "Sky Dwelling status: {0} record(s), {1} live / {2} pool maps (price {3}g, unload after {4} min).",
							Sky.OwnerCount,
							Sky.LiveCount,
							Sky.PoolSize,
							Sky.DwellingPrice,
							(int)Sky.UnloadAfter.TotalMinutes );
						break;
					}
				case "price":
					{
						if ( e.Arguments.Length < 2 )
						{
							from.SendMessage( "Usage: [skyinstance price <gold>" );
							return;
						}
						int gold;
						if ( !Int32.TryParse( e.Arguments[1], out gold ) || gold < 0 )
						{
							from.SendMessage( "Bad gold value." );
							return;
						}
						Sky.DwellingPrice = gold;
						from.SendMessage( "Sky dwellings now cost {0} gold.", gold );
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
						Sky.SetUnloadAfter( TimeSpan.FromMinutes( minutes ) );
						from.SendMessage( "Sky dwellings will now unload after {0} minutes of inactivity.", minutes );
						break;
					}
				case "park":
					{
						List<Instance> live = new List<Instance>( Sky.LiveInstances );
						int count = 0;
						foreach ( Instance inst in live )
						{
							if ( inst.IsLive )
							{
								Sky.Park( inst );
								count++;
							}
						}
						from.SendMessage( "Parked {0} live dwelling(s).", count );
						break;
					}
				case "grant":
					{
						from.SendMessage( "Target the player to grant a sky dwelling (bypasses purchase)." );
						from.Target = new GrantTarget();
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
						int freed = Sky.ReleaseDeadOwners();
						from.SendMessage( "Freed {0} orphaned dwelling(s).", freed );
						break;
					}
				case "installrope":
					{
						int swapped = InstallChooserRopes();
						from.SendMessage( "Replaced {0} climb teleporter(s) with the sky dwelling chooser.", swapped );
						break;
					}
				default:
					from.SendMessage( "Unknown subcommand. Try: status, unload, park, gotoplayer, placeportal, freedead, installrope" );
					break;
			}
		}

		private class GrantTarget : Target
		{
			public GrantTarget() : base( 12, false, TargetFlags.None ) { }

			protected override void OnTarget( Mobile from, object targeted )
			{
				PlayerMobile target = targeted as PlayerMobile;
				if ( target == null )
				{
					from.SendMessage( "That is not a player." );
					return;
				}

				if ( Sky.Purchase( target ) )
					from.SendMessage( "Granted a sky dwelling to {0}.", target.Name );
				else
					from.SendMessage( "{0} already owns a sky dwelling.", target.Name );
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

				// GameMasters bypass the friend check inside VisitFriendDwelling.
				Sky.VisitFriendDwelling( from, target );
			}
		}
	}
}
