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

		public MannequinOwnerGump( Mannequin mannequin ) : base( 100, 100 )
		{
			m_Mannequin = mannequin;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage( 0 );
			AddBackground( 0, 0, 350, 300, 0x1453 );

			AddHtml( 10, 12, 330, 20, "<CENTER><BASEFONT COLOR=#FFFFFF>MANNEQUIN MANAGEMENT</BASEFONT></CENTER>", false, false );

			int x = 40;
			int y = 50;
			int step = 30;

			AddButton( x, y, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddLabel( x + 35, y, 0x480, "Open Paperdoll" );
			y += step;

			AddButton( x, y, 4005, 4007, 2, GumpButtonType.Reply, 0 );
			AddLabel( x + 35, y, 0x480, "Swap Gear" );
			y += step;

			AddButton( x, y, 4005, 4007, 3, GumpButtonType.Reply, 0 );
			AddLabel( x + 35, y, 0x480, "Change Race" );
			y += step;

			AddButton( x, y, 4005, 4007, 4, GumpButtonType.Reply, 0 );
			AddLabel( x + 35, y, 0x480, mannequin.IsFemale ? "Switch to Male" : "Switch to Female" );
			y += step;

			AddButton( x, y, 4005, 4007, 5, GumpButtonType.Reply, 0 );
			AddLabel( x + 35, y, 0x480, "Customize Appearance" );
			y += step;

			AddButton( x, y, 4005, 4007, 6, GumpButtonType.Reply, 0 );
			AddLabel( x + 35, y, 0x480, "Pack Up" );
			y += step;

			AddButton( x, y, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddLabel( x + 35, y, 0x480, "Close" );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( m_Mannequin == null || m_Mannequin.Deleted )
				return;

			if ( !m_Mannequin.CanManage( from ) )
				return;

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
				case 3: // Change Race
				{
					from.CloseGump( typeof( MannequinRaceGump ) );
					from.SendGump( new MannequinRaceGump( m_Mannequin, 0 ) );
					break;
				}
				case 4: // Toggle Male/Female
				{
					m_Mannequin.ToggleFemale( from );
					from.CloseGump( typeof( MannequinOwnerGump ) );
					from.SendGump( new MannequinOwnerGump( m_Mannequin ) );
					break;
				}
				case 5: // Customize Appearance
				{
					from.SendGump( new PlayerVendorCustomizeGump( m_Mannequin, from ) );
					break;
				}
				case 6: // Pack Up
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

		public MannequinRaceGump( Mannequin mannequin, int page ) : base( 50, 50 )
		{
			m_Mannequin = mannequin;

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
				costume = BaseRace.GetCostume( 80000 + page );

				if ( costume != null )
				{
					speciesName = costume.Name;
					speciesGump = 50000 + costume.SpeciesGump;
					confirmButton = 80000 + page;
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

			int id = info.ButtonID;

			if ( id == 0 )
				return;

			if ( id == 1 )
			{
				int next = FindPrevValid( m_Page );
				from.SendGump( new MannequinRaceGump( m_Mannequin, next ) );
				return;
			}

			if ( id == 2 )
			{
				int next = FindNextValid( m_Page );
				from.SendGump( new MannequinRaceGump( m_Mannequin, next ) );
				return;
			}

			if ( id == 1000 )
			{
				m_Mannequin.RevertToHuman();
				return;
			}

			if ( id > 80000 )
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
				BaseRace c = BaseRace.GetCostume( 80000 + p );
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
				BaseRace c = BaseRace.GetCostume( 80000 + p );
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
