using System;
using System.Collections.Generic;
using Server;
using Knives.TownHouses;

namespace Server.Engines.Instancing
{
	// Shown when a player climbs the sky-dwelling rope (see SkyDwellingTeleporter).
	// Lets them pick where to go:
	//   * their own dwelling, or -- if they have none -- an "available" dwelling
	//     they can look around and purchase;
	//   * dwellings they have been invited to (friend / co-owner);
	//   * any dwelling its owner has set Public.
	// Tapping a row teleports straight there.
	public class SkyDwellingChooserGump : GumpPlusLight
	{
		public SkyDwellingChooserGump( Mobile m ) : base( m, 100, 100 )
		{
		}

		private static SkyDwellingInstanceType Sky { get { return SkyDwellingInstanceType.Instance; } }

		protected override void BuildGump()
		{
			int width = 340;
			int y = 0;

			AddHtml( 0, y += 10, width, "<CENTER>Where would you like to go?" );

			// --- Your own dwelling, or an available one to buy ---
			AddHtml( 12, y += 28, width, "<BASEFONT COLOR=#FFD700>Your Sky Dwelling</BASEFONT>" );

			if ( Sky.OwnsDwelling( Owner ) )
			{
				AddButton( 20, y += 22, 0xFA5, 0xFA7, "own", new GumpCallback( EnterOwn ) );
				AddHtml( 55, y + 2, width - 60, String.Format( "Enter your sky dwelling{0}", Sky.IsPublic( Owner ) ? " (Public)" : "" ) );
			}
			else
			{
				AddButton( 20, y += 22, 0xFA5, 0xFA7, "available", new GumpCallback( EnterAvailable ) );
				AddHtml( 55, y + 2, width - 60, "Available Sky Dwelling -- visit & purchase" );
			}

			// --- Dwellings you have been invited to ---
			List<Instance> invited = Sky.InvitedInstances( Owner );
			if ( invited.Count > 0 )
			{
				AddHtml( 12, y += 30, width, "<BASEFONT COLOR=#88FF88>Dwellings You May Visit</BASEFONT>" );
				for ( int i = 0; i < invited.Count; i++ )
				{
					Instance inst = invited[i];
					AddButton( 20, y += 22, 0xFA5, 0xFA7, new GumpStateCallback( EnterInstance ), inst );
					AddHtml( 55, y + 2, width - 60, OwnerLabel( inst ) );
				}
			}

			// --- Public dwellings ---
			List<Instance> pub = Sky.PublicInstances( Owner );
			if ( pub.Count > 0 )
			{
				AddHtml( 12, y += 30, width, "<BASEFONT COLOR=#88CCFF>Public Sky Dwellings</BASEFONT>" );
				for ( int i = 0; i < pub.Count; i++ )
				{
					Instance inst = pub[i];
					AddButton( 20, y += 22, 0xFA5, 0xFA7, new GumpStateCallback( EnterInstance ), inst );
					AddHtml( 55, y + 2, width - 60, OwnerLabel( inst ) );
				}
			}

			AddBackgroundZero( 0, 0, width, y + 45, 0x1453 );
		}

		private static string OwnerLabel( Instance inst )
		{
			Mobile owner = World.FindMobile( inst.OwnerSerial );
			return owner != null ? String.Format( "{0}'s Sky Dwelling", owner.Name ) : "a sky dwelling";
		}

		private void EnterOwn()
		{
			Sky.SendOwnerToTheirInstance( Owner );
		}

		private void EnterAvailable()
		{
			// Generates a fresh, unpurchased instance with a purchase sign inside.
			Sky.SendOwnerToTheirInstance( Owner );
		}

		private void EnterInstance( object state )
		{
			Instance inst = state as Instance;
			if ( inst != null )
				Sky.EnterChosen( Owner, inst );
		}
	}
}
