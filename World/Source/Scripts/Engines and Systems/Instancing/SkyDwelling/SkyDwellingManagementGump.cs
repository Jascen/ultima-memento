using System;
using Server;
using Knives.TownHouses;

namespace Server.Engines.Instancing
{
	// Owner-only control reached by double-clicking the owned sky-dwelling sign.
	// Sets the dwelling public (listed for anyone to visit from the climb chooser)
	// or private (owner + invited friends only).
	public class SkyDwellingManagementGump : GumpPlusLight
	{
		public SkyDwellingManagementGump( Mobile m ) : base( m, 100, 100 )
		{
		}

		private static SkyDwellingInstanceType Sky { get { return SkyDwellingInstanceType.Instance; } }

		protected override void BuildGump()
		{
			int width = 260;
			int y = 0;

			AddHtml( 0, y += 10, width, "<CENTER>Sky Dwelling Management" );

			if ( !Sky.OwnsDwelling( Owner ) )
			{
				AddHtml( 0, y += 28, width, "<CENTER>You do not own a sky dwelling." );
				AddBackgroundZero( 0, 0, width, y + 40, 0x1453 );
				return;
			}

			bool pub = Sky.IsPublic( Owner );

			AddHtml( 0, y += 26, width, String.Format( "<CENTER>Visibility: {0}", pub ? "Public" : "Private" ) );

			AddButton( 25, y += 32, pub ? 0xD2 : 0xD3, "Private", new GumpCallback( MakePrivate ) );
			AddHtml( 50, y, width - 55, "Private (you & invited friends)" );

			AddButton( 25, y += 26, pub ? 0xD3 : 0xD2, "Public", new GumpCallback( MakePublic ) );
			AddHtml( 50, y, width - 55, "Public (anyone may visit)" );

			AddHtml( 0, y += 30, width, "<CENTER>Invite friends with [skyfriend add" );

			AddBackgroundZero( 0, 0, width, y + 40, 0x1453 );
		}

		private void MakePublic()
		{
			if ( Sky.SetPublic( Owner, true ) )
				Owner.SendMessage( "Your sky dwelling is now public; anyone may visit." );
			NewGump();
		}

		private void MakePrivate()
		{
			if ( Sky.SetPublic( Owner, false ) )
				Owner.SendMessage( "Your sky dwelling is now private." );
			NewGump();
		}
	}
}
