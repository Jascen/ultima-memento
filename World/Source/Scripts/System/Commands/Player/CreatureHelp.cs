using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Commands;

namespace Server.Gumps
{
    public class CreatureHelpGump : Gump
    {
		public int m_Origin;

        public CreatureHelpGump( Mobile from, int origin ) : base( 75, 75 )
        {
			m_Origin = origin;
			string color = "#9dca8b";
			string hilit = "#e1e0a1";
			from.SendSound( 0x4A ); 

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			AddImage(0, 0, 7038, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddButton(1133, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);
			AddHtml( 13, 13, 711, 20, @"<BODY><BASEFONT Color=" + color + ">CREATURE HELP</BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 12, 49, 600, 553, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.RacePotions.RaceHelp( m_Origin ) + "</BASEFONT></BODY>", (bool)false, (bool)true);
			AddImage(620, 50, 7037);
			AddHtml( 636, 420, 506, 190, @"<BODY><BASEFONT Color=" + hilit + ">" + Server.Items.RacePotions.RaceEquipment() + "</BASEFONT></BODY>", (bool)false, (bool)false);
        }

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;
			from.SendSound( 0x4A ); 
			if ( m_Origin > 0 ){ from.SendGump( new Server.Engines.Help.HelpGump( from, m_Origin ) ); }
		}
    }
}