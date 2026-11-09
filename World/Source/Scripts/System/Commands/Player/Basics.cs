using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Commands;

namespace Server.Gumps
{
    public class BasicsGump : Gump
    {
		public int m_Origin;

        public BasicsGump( Mobile from, int origin ) : base( 50, 50 )
        {
			m_Origin = origin;
			string color = "#ddbc4b";
			from.SendSound( 0x4A ); 

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			AddImage(0, 0, 9546, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddHtml( 14, 14, 400, 20, @"<BODY><BASEFONT Color=" + color + ">BASICS</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(867, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);
			AddHtml( 17, 49, 875, 726, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.DynamicBook.BasicHelp() + "</BASEFONT></BODY>", (bool)false, (bool)true);
        }

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;
			from.SendSound( 0x4A ); 
			if ( m_Origin > 0 ){ from.SendGump( new Server.Engines.Help.HelpGump( from, m_Origin ) ); }
		}
    }
}