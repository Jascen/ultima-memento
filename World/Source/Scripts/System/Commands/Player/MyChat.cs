using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Menus;
using Server.Menus.Questions;
using Server.Accounting;
using Server.Multis;
using Server.Mobiles;
using Server.Regions;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Misc;
using Server.Items;
using System.Globalization;

namespace Server.Gumps 
{
    public class MyTalk : Gump
    {
		public MyTalk ( Mobile from, int source ) : base ( 80, 80 )
		{
            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			Mobile person = new Mage();
			from.SendSound( 0x4A );

			string text = Server.Gumps.GypsyTarotGump.GypsySpeech( from );

			if ( source > 0 )
				text = Server.Misc.SpeechFunctions.SpeechText( person, from, MyChat.talkInfo( source, 2 ) );

			person.Delete();

			string color = "#ddbc4b";

			AddPage(0);

			AddImage(0, 0, 9547, PlayerSettings.GetGumpHue( from ));

			AddHtml( 10, 10, 300, 20, @"<BODY><BASEFONT Color=" + color + ">CONVERSATION - " + MyChat.talkInfo( source, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(568, 9, 4017, 4017, 0, GumpButtonType.Reply, 0);
			AddHtml( 16, 44, 576, 470, @"<BODY><BASEFONT Color=" + color + ">" + text + "</BASEFONT></BODY>", (bool)false, (bool)true);
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;
			from.SendSound( 0x4A );
		}
	}

    public class MyChat : Gump
    {
		public int m_Origin;

		public MyChat ( Mobile from, int source ) : base ( 50, 50 )
		{
			m_Origin = source;

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			string color = "#a7bad2";

			AddPage(0);

			AddImage(0, 0, 7018, PlayerSettings.GetGumpHue( from ));

			AddHtml( 12, 12, 200, 20, @"<BODY><BASEFONT Color=" + color + ">CONVERSATIONS</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(879, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

			int x = 20;
			int y = 50;

			int i = 350;

			int d = 35;

			int rows = 0;

			AddButton(x, y, 4008, 4008, 400, GumpButtonType.Reply, 0);
			AddHtml( x+38, y, 200, 20, @"<BODY><BASEFONT Color=" + color + ">Welcome</BASEFONT></BODY>", (bool)false, (bool)false);
			y=y+d;
			rows++;

			string keys = PlayerSettings.ValChatConfig( from );

			if ( keys.Length > 0 )
			{
				string[] configures = keys.Split('#');
				int entry = 1;

				foreach (string key in configures)
				{
					if ( key == "1" )
					{
						if ( rows == 16 || rows == 32 ){ x = x+i; y = 50; }

						AddButton(x, y, 4008, 4008, entry, GumpButtonType.Reply, 0);
						AddHtml( x+38, y, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + talkInfo( entry, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
						y=y+d;
						rows++;
					}
					entry++;
				}
			}
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			from.CloseGump( typeof( MyChat ) );
			int button = info.ButtonID;

			if ( button > 0 )
			{
				from.SendGump( new MyChat( from, m_Origin ) );

				if ( button == 400 ) // WELCOME
				{
					from.CloseGump( typeof( MyTalk ) ); from.SendGump( new MyTalk( from, 0 ) );
				}
				else // CONVERSATIONS
				{
					from.CloseGump( typeof( MyTalk ) ); from.SendGump( new MyTalk( from, button ) );
				}
			}
			else if ( m_Origin > 0 ){ from.SendGump( new Server.Engines.Help.HelpGump( from, m_Origin ) ); }
			else { from.SendSound( 0x4A ); }
		}

		public static string talkInfo( int val, int part )
		{
			string txt = "";
			string spk = "";

			switch ( val )
			{
				case 0: txt = "Welcome"; spk = "Welcome"; break;
				case 1: txt = "Alchemist"; spk = "Alchemist"; break;
				case 2: txt = "Architect"; spk = "Architect"; break;
				case 3: txt = "Armorer"; spk = "Armorer"; break;
				case 4: txt = "Assassin"; spk = "Assassin"; break;
				case 5: txt = "Banker"; spk = "Banker"; break;
				case 6: txt = "Bard"; spk = "Bard"; break;
				case 7: txt = "Blacksmith"; spk = "Blacksmith"; break;
				case 8: txt = "Bowyer"; spk = "Bowyer"; break;
				case 9: txt = "Cook"; spk = "Cook"; break;
				case 10: txt = "Courier"; spk = "Courier"; break;
				case 11: txt = "Death Knight Demon"; spk = "DeathKnight"; break;
				case 12: txt = "Devon"; spk = "Devon"; break;
				case 13: txt = "Druid"; spk = "Druid"; break;
				case 14: txt = "Elementalism"; spk = "Elementalism"; break;
				case 15: txt = "Farmer"; spk = "Farmer"; break;
				case 16: txt = "Undertaker"; spk = "Frankenstein"; break;
				case 17: txt = "Furtrader"; spk = "Furtrader"; break;
				case 18: txt = "Arez the God of Legends"; spk = "GodOfLegends"; break;
				case 19: txt = "Guard"; spk = "Guard"; break;
				case 20: txt = "Gypsy"; spk = "Gypsy"; break;
				case 21: txt = "Healer"; spk = "Healer"; break;
				case 22: txt = "Herbalist"; spk = "Herbalist"; break;
				case 23: txt = "Jedi"; spk = "Jedi"; break;
				case 24: txt = "Jester"; spk = "Jester"; break;
				case 25: txt = "Knight"; spk = "Knight"; break;
				case 26: txt = "Leather Worker"; spk = "LeatherWorker"; break;
				case 27: txt = "Mage"; spk = "Mage"; break;
				case 28: txt = "Mapmaker"; spk = "Mapmaker"; break;
				case 29: txt = "Monk"; spk = "Monk"; break;
				case 30: txt = "Grounds Keeper"; spk = "NecroGreeter"; break;
				case 31: txt = "Necromancer"; spk = "Necromancer"; break;
				case 32: txt = "Painter"; spk = "Painter"; break;
				case 33: txt = "Stablemaster"; spk = "Pets"; break;
				case 34: txt = "Teacher of Knowledge"; spk = "Powerscroll"; break;
				case 35: txt = "Provisioner"; spk = "Provisioner"; break;
				case 36: txt = "Ranger"; spk = "Ranger"; break;
				case 37: txt = "Sage"; spk = "Sage"; break;
				case 38: txt = "Scribe"; spk = "Scribe"; break;
				case 39: txt = "Shipwright"; spk = "Shipwright"; break;
				case 40: txt = "Stonecrafter"; spk = "Stonecrafter"; break;
				case 41: txt = "Tailor"; spk = "Tailor"; break;
				case 42: txt = "Tanner"; spk = "Tanner"; break;
				case 43: txt = "Tavern"; spk = "Tavern"; break;
				case 44: txt = "Thief"; spk = "Thief"; break;
				case 45: txt = "Art Collector"; spk = "Variety"; break;
				case 46: txt = "Weaponsmith"; spk = "Weaponsmith"; break;
				case 47: txt = "Xardok"; spk = "Xardok"; break;
			}

			if ( part == 1 )
				return txt;
			else if ( part == 2 )
				return spk;

			return txt;
		}

		public static void speechText( string chat, Mobile m )
		{
			bool effect = false;
			int num = 0;

			if ( chat == "Alchemist" ){ num = 1; }
			else if ( chat == "Architect" ){ num = 2; }
			else if ( chat == "Armorer" ){ num = 3; }
			else if ( chat == "Assassin" ){ num = 4; }
			else if ( chat == "Banker" ){ num = 5; }
			else if ( chat == "Bard" ){ num = 6; }
			else if ( chat == "Blacksmith" ){ num = 7; }
			else if ( chat == "Bowyer" ){ num = 8; }
			else if ( chat == "Cook" ){ num = 9; }
			else if ( chat == "Courier" ){ num = 10; }
			else if ( chat == "DeathKnight" ){ num = 11; }
			else if ( chat == "Devon" ){ num = 12; }
			else if ( chat == "Druid" ){ num = 13; }
			else if ( chat == "Elementalism" ){ num = 14; }
			else if ( chat == "Farmer" ){ num = 15; }
			else if ( chat == "Frankenstein" ){ num = 16; }
			else if ( chat == "Furtrader" ){ num = 17; }
			else if ( chat == "GodOfLegends" ){ num = 18; }
			else if ( chat == "Guard" ){ num = 19; }
			else if ( chat == "Gypsy" ){ num = 20; }
			else if ( chat == "Healer" ){ num = 21; }
			else if ( chat == "Herbalist" ){ num = 22; }
			else if ( chat == "Jedi" ){ num = 23; }
			else if ( chat == "Jester" ){ num = 24; }
			else if ( chat == "Knight" ){ num = 25; }
			else if ( chat == "LeatherWorker" ){ num = 26; }
			else if ( chat == "Mage" ){ num = 27; }
			else if ( chat == "Mapmaker" ){ num = 28; }
			else if ( chat == "Monk" ){ num = 29; }
			else if ( chat == "NecroGreeter" ){ num = 30; }
			else if ( chat == "Necromancer" ){ num = 31; }
			else if ( chat == "Painter" ){ num = 32; }
			else if ( chat == "Pets" ){ num = 33; }
			else if ( chat == "Powerscroll" ){ num = 34; }
			else if ( chat == "Provisioner" ){ num = 35; }
			else if ( chat == "Ranger" ){ num = 36; }
			else if ( chat == "Sage" ){ num = 37; }
			else if ( chat == "Scribe" ){ num = 38; }
			else if ( chat == "Shipwright" ){ num = 39; }
			else if ( chat == "Stonecrafter" ){ num = 40; }
			else if ( chat == "Tailor" ){ num = 41; }
			else if ( chat == "Tanner" ){ num = 42; }
			else if ( chat == "Tavern" ){ num = 43; }
			else if ( chat == "Thief" ){ num = 44; }
			else if ( chat == "Variety" ){ num = 45; }
			else if ( chat == "Weaponsmith" ){ num = 46; }
			else if ( chat == "Xardok" ){ num = 47; }

			if ( num > 0 )
			{
				if ( !PlayerSettings.GetChatConfig( m, num ) )
				{
					PlayerSettings.SetChatConfig( m, num );
					effect = true;
				}
			}

			if ( effect )
			{
				Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 0, 0, 5024, 0 );
				m.SendSound( 0x65C );
				m.SendMessage( "The " + talkInfo( num, 1 ) + " conversation has been memorized." );
			}

		}
	}
}