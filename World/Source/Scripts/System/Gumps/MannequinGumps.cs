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

}
