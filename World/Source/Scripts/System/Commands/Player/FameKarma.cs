using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Commands;

namespace Server.Gumps
{
    public class FameKarma : Gump
    {
		public int m_Origin;

        public FameKarma( Mobile from, int origin ) : base( 50, 50 )
        {
			m_Origin = origin;
			string color = "#e87373";
			from.SendSound( 0x4A ); 

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			AddImage(0, 0, 9578, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddButton(859, 9, 4017, 4017, 0, GumpButtonType.Reply, 0);
			AddHtml( 12, 12, 576, 20, @"<BODY><BASEFONT Color=" + color + ">FAME AND KARMA</BASEFONT></BODY>", (bool)false, (bool)false);

			AddHtml( 20, 80, 137, 20, @"<BODY><BASEFONT Color=" + color + ">KARMA</BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 164, 50, 720, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>FAME</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);

			string col1 = "<BR><BR>";
			col1 = col1 + "10,000 & up<BR><BR>";
			col1 = col1 + "9,999 to 5,000<BR><BR>";
			col1 = col1 + "4,999 to 2,500<BR><BR>";
			col1 = col1 + "2,499 to 1,250<BR><BR>";
			col1 = col1 + "1,249 to 625<BR><BR>";
			col1 = col1 + "624 to -624<BR><BR>";
			col1 = col1 + "-625 to -1,249<BR><BR>";
			col1 = col1 + "-1,250 to -2,499<BR><BR>";
			col1 = col1 + "-2,500 to -4,999<BR><BR>";
			col1 = col1 + "-5,000 to -9,999<BR><BR>";
			col1 = col1 + "-10,000 & lower<BR><BR>";

			string col2 = "";
			col2 = col2 + "0 to 1,249<BR><BR>";
			col2 = col2 + "Trustworthy<BR><BR>";
			col2 = col2 + "Honest<BR><BR>";
			col2 = col2 + "Good<BR><BR>";
			col2 = col2 + "Kind<BR><BR>";
			col2 = col2 + "Fair<BR><BR>";
			col2 = col2 + "No Title<BR><BR>";
			col2 = col2 + "Rude<BR><BR>";
			col2 = col2 + "Unsavory<BR><BR>";
			col2 = col2 + "Scoundrel<BR><BR>";
			col2 = col2 + "Despicable<BR><BR>";
			col2 = col2 + "Outcast<BR><BR>";

			string col3 = "";
			col3 = col3 + "1,250 to 2,499<BR><BR>";
			col3 = col3 + "Estimable<BR><BR>";
			col3 = col3 + "Commendable<BR><BR>";
			col3 = col3 + "Honorable<BR><BR>";
			col3 = col3 + "Respectable<BR><BR>";
			col3 = col3 + "Upstanding<BR><BR>";
			col3 = col3 + "Notable<BR><BR>";
			col3 = col3 + "Disreputable<BR><BR>";
			col3 = col3 + "Dishonorable<BR><BR>";
			col3 = col3 + "Malicious<BR><BR>";
			col3 = col3 + "Dastardly<BR><BR>";
			col3 = col3 + "Wretched<BR><BR>";

			string col4 = "";
			col4 = col4 + "2500 to 4,999<BR><BR>";
			col4 = col4 + "Great<BR><BR>";
			col4 = col4 + "Famed<BR><BR>";
			col4 = col4 + "Admirable<BR><BR>";
			col4 = col4 + "Proper<BR><BR>";
			col4 = col4 + "Reputable<BR><BR>";
			col4 = col4 + "Prominent<BR><BR>";
			col4 = col4 + "Notorious<BR><BR>";
			col4 = col4 + "Ignoble<BR><BR>";
			col4 = col4 + "Vile<BR><BR>";
			col4 = col4 + "Wicked<BR><BR>";
			col4 = col4 + "Nefarious<BR><BR>";

			string col5 = "";
			col5 = col5 + "5,000 to 9,999<BR><BR>";
			col5 = col5 + "Glorious<BR><BR>";
			col5 = col5 + "Illustrious<BR><BR>";
			col5 = col5 + "Noble<BR><BR>";
			col5 = col5 + "Eminent<BR><BR>";
			col5 = col5 + "Distinguished<BR><BR>";
			col5 = col5 + "Renowned<BR><BR>";
			col5 = col5 + "Infamous<BR><BR>";
			col5 = col5 + "Sinister<BR><BR>";
			col5 = col5 + "Villainous<BR><BR>";
			col5 = col5 + "Evil<BR><BR>";
			col5 = col5 + "Dread<BR><BR>";

			string col6 = "";
			col6 = col6 + "10,000 & up<BR><BR>";
			col6 = col6 + "Glorious Lord<BR><BR>";
			col6 = col6 + "Illustrious Lord<BR><BR>";
			col6 = col6 + "Noble Lord<BR><BR>";
			col6 = col6 + "Eminent Lord<BR><BR>";
			col6 = col6 + "Distinguished Lord<BR><BR>";
			col6 = col6 + "Lord<BR><BR>";
			col6 = col6 + "Dishonored Lord<BR><BR>";
			col6 = col6 + "Sinister Lord<BR><BR>";
			col6 = col6 + "Dark Lord<BR><BR>";
			col6 = col6 + "Evil Lord<BR><BR>";
			col6 = col6 + "Dread Lord<BR><BR>";

			AddHtml( 20, 80, 144, 495, @"<BODY><BASEFONT Color=" + color + ">" + col1 + "</BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 164, 80, 144, 495, @"<BODY><BASEFONT Color=" + color + "><CENTER>" + col2 + "</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 308, 80, 144, 495, @"<BODY><BASEFONT Color=" + color + "><CENTER>" + col3 + "</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 452, 80, 144, 495, @"<BODY><BASEFONT Color=" + color + "><CENTER>" + col4 + "</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 596, 80, 144, 495, @"<BODY><BASEFONT Color=" + color + "><CENTER>" + col5 + "</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 740, 80, 144, 495, @"<BODY><BASEFONT Color=" + color + "><CENTER>" + col6 + "</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
        }

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;
			from.SendSound( 0x4A ); 
			if ( m_Origin > 0 ){ from.SendGump( new Server.Engines.Help.HelpGump( from, m_Origin ) ); }
		}
    }
}