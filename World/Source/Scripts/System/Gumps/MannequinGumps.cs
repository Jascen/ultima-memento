using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	public class MannequinOwnerGump : Gump
	{
		private Mannequin m_Mannequin;

		public MannequinOwnerGump( Mannequin mannequin, Mobile from ) : base( 50, 50 )
		{
			m_Mannequin = mannequin;

			if ( from != null )
				mannequin.PauseFor( from );

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage( 0 );
			AddBackground( 0, 0, 280, 240, 0x1453 );

			AddImageTiled( 10, 10, 260, 20, 0xA40 );
			AddImageTiled( 10, 40, 260, 160, 0xA40 );
			AddImageTiled( 10, 210, 260, 20, 0xA40 );

			AddAlphaRegion( 10, 10, 260, 220 );

			AddHtml( 10, 12, 260, 18, "<CENTER><BASEFONT COLOR=#FFFFFF>MANNEQUIN MANAGEMENT</BASEFONT></CENTER>", false, false );

			int x = 20;
			int y = 50;
			int step = 25;

			AddButton( x, y, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y + 2, 200, 18, "<BASEFONT COLOR=#FFFFFF>Open Paperdoll</BASEFONT>", false, false );
			y += step;

			AddButton( x, y, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y + 2, 200, 18, "<BASEFONT COLOR=#FFFFFF>Swap Gear</BASEFONT>", false, false );
			y += step;

			AddButton( x, y, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y + 2, 200, 18, "<BASEFONT COLOR=#FFFFFF>Customize Appearance</BASEFONT>", false, false );
			y += step;

			AddButton( x, y, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y + 2, 200, 18, "<BASEFONT COLOR=#FFFFFF>Pack Up</BASEFONT>", false, false );
			y += step;

			AddCheck( x, y, 0xD2, 0xD3, mannequin.Roaming, 99 );
			AddHtml( x + 35, y + 2, 200, 18, "<BASEFONT COLOR=#FFFFFF>Roam House</BASEFONT>", false, false );

			AddButton( 20, 210, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 55, 212, 200, 18, 1060675, 0x7FFF, false, false ); // CLOSE
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( m_Mannequin == null || m_Mannequin.Deleted )
				return;

			if ( !m_Mannequin.CanManage( from ) )
				return;

			// Apply roaming-switch state regardless of which button was pressed.
			bool wantRoam = info.IsSwitched( 99 );
			if ( wantRoam != m_Mannequin.Roaming )
				m_Mannequin.Roaming = wantRoam;

			m_Mannequin.PauseFor( from );

			switch ( info.ButtonID )
			{
				case 1: // Open Paperdoll
				{
					m_Mannequin.DisplayPaperdollTo( from );
					break;
				}
				case 2: // Swap Gear
				{
					m_Mannequin.SwapGear( from );
					break;
				}
				case 3: // Customize Appearance (hair/beard styles + colors + skin + race)
				{
					from.SendGump( new NewPlayerVendorCustomizeGump( m_Mannequin ) );
					break;
				}
				case 4: // Pack Up
				{
					m_Mannequin.PackUp( from );
					break;
				}
			}
		}
	}

	public class MannequinRaceGump : Gump
	{
		private const int MaxMonsterPage = 172;
		private Mannequin m_Mannequin;
		private int m_Page;

		public MannequinRaceGump( Mannequin mannequin, int page, Mobile from ) : base( 50, 50 )
		{
			m_Mannequin = mannequin;

			if ( from != null )
				mannequin.PauseFor( from );

			// Clamp + skip invalid monster entries
			if ( page < 0 )
				page = 0;
			if ( page > MaxMonsterPage )
				page = MaxMonsterPage;

			m_Page = page;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage( 0 );
			AddBackground( 0, 0, 500, 400, 9300 );

			AddHtml( 10, 12, 480, 20, "<CENTER><BASEFONT COLOR=#FFFFFF>MANNEQUIN APPEARANCE</BASEFONT></CENTER>", false, false );

			BaseRace costume = null;
			string speciesName = "Human";
			int speciesGump = 50994; // human paperdoll image
			int confirmButton = 1000;

			if ( page > 0 )
			{
				costume = BaseRace.GetCostume( BaseRace.MonsterRaceIDBase + page );

				if ( costume != null )
				{
					speciesName = costume.Name;
					speciesGump = 50000 + costume.SpeciesGump;
					confirmButton = BaseRace.MonsterRaceIDBase + page;
				}
			}

			// Left text panel
			AddHtml( 40, 60, 200, 20, "<BASEFONT COLOR=#FFFFFF>Species:</BASEFONT>", false, false );
			AddHtml( 120, 60, 200, 20, "<BASEFONT COLOR=#FFFFFF>" + speciesName + "</BASEFONT>", false, false );

			// Paperdoll backdrop + preview image
			AddImage( 252, 32, 2001 ); // paperdoll container
			AddImage( 259, 51, speciesGump );
			AddImage( 253, 51, 50422 ); // backpack icon

			// Navigation
			AddButton( 40, 350, 4014, 4014, 1, GumpButtonType.Reply, 0 ); // Prev
			AddLabel( 75, 350, 0x480, "Prev" );

			AddButton( 200, 350, 4023, 4023, confirmButton, GumpButtonType.Reply, 0 ); // Apply
			AddLabel( 235, 350, 0x480, "Apply" );

			AddButton( 350, 350, 4005, 4005, 2, GumpButtonType.Reply, 0 ); // Next
			AddLabel( 385, 350, 0x480, "Next" );

			AddButton( 460, 12, 4017, 4017, 0, GumpButtonType.Reply, 0 ); // Close

			if ( costume != null )
				costume.Delete();
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( m_Mannequin == null || m_Mannequin.Deleted )
				return;

			if ( !m_Mannequin.CanManage( from ) )
				return;

			// Any response counts as interaction — refresh the pause window.
			m_Mannequin.PauseFor( from );

			int id = info.ButtonID;

			if ( id == 0 )
				return;

			if ( id == 1 )
			{
				int next = FindPrevValid( m_Page );
				from.SendGump( new MannequinRaceGump( m_Mannequin, next, from ) );
				return;
			}

			if ( id == 2 )
			{
				int next = FindNextValid( m_Page );
				from.SendGump( new MannequinRaceGump( m_Mannequin, next, from ) );
				return;
			}

			if ( id == 1000 )
			{
				m_Mannequin.RevertToHuman();
				return;
			}

			if ( id > BaseRace.MonsterRaceIDBase )
			{
				m_Mannequin.ApplyRace( id );
				return;
			}
		}

		private static int FindPrevValid( int page )
		{
			int p = page - 1;
			while ( p > 0 )
			{
				BaseRace c = BaseRace.GetCostume( BaseRace.MonsterRaceIDBase + p );
				if ( c != null && !String.IsNullOrEmpty( c.Name ) && c.SpeciesID > 0 )
				{
					c.Delete();
					return p;
				}
				if ( c != null )
					c.Delete();
				p--;
			}
			return 0;
		}

		private static int FindNextValid( int page )
		{
			int p = page + 1;
			while ( p <= MaxMonsterPage )
			{
				BaseRace c = BaseRace.GetCostume( BaseRace.MonsterRaceIDBase + p );
				if ( c != null && !String.IsNullOrEmpty( c.Name ) && c.SpeciesID > 0 )
				{
					c.Delete();
					return p;
				}
				if ( c != null )
					c.Delete();
				p++;
			}
			return MaxMonsterPage;
		}
	}
}
