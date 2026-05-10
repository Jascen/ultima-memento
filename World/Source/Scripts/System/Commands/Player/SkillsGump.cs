using Server.Network;
using Server.Mobiles;
using Server.Misc;

namespace Server.Gumps 
{
    public class SkillTitleGump : Gump
    {
        public SkillTitleGump ( Mobile from ) : base ( 50, 50 )
        {
			string color = "#ddbc4b";
			int display = 60;
			int line = 0;

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			AddImage(0, 0, 9548, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddHtml( 12, 12, 300, 20, @"<BODY><BASEFONT Color=" + color + ">CHOOSE THE SKILL YOU WANT TO BE TITLED FROM</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(967, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);
			AddHtml( 396, 12, 559, 20, @"<BODY><BASEFONT Color=" + color + ">" + from.Name + " the " + GetPlayerInfo.GetSkillTitle( from ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

			var player = (PlayerMobile)from;

			while ( display > 0 )
			{
				display--;
				line++;

				GetLine( line, player );
			}
        }
    
		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( info.ButtonID == 99 ){ ((PlayerMobile)from).CharacterSkill = 0; from.SendSound( 0x4A ); }
			else if ( info.ButtonID > 0 ){ ((PlayerMobile)from).CharacterSkill = info.ButtonID; from.SendSound( 0x4A ); }

			if ( info.ButtonID > 0 ){ from.SendGump( new SkillTitleGump( from ) ); }
			else { from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) ); }
		}

		public void GetLine( int val, PlayerMobile player )
		{
			string color = "#ddbc4b";
			int skl = 0;
			string txt = "";
			int btn = 3609;

			if ( val == 1 ){ skl = 1; txt = "Alchemy"; }
			else if ( val == 2 ){ skl = 2; txt = "Anatomy"; }
			else if ( val == 3 ){ skl = 6; txt = "Arms Lore"; }
			else if ( val == 4 ){ skl = 7; txt = "Begging"; }
			else if ( val == 5 ){ skl = 8; txt = "Blacksmithing"; }
			else if ( val == 6 ){ skl = 30; txt = "Bludgeoning"; }
			else if ( val == 7 ){ skl = 20; txt = "Bowcrafting"; }
			else if ( val == 8 ){ skl = 9; txt = "Bushido"; }
			else if ( val == 9 ){ skl = 10; txt = "Camping"; }
			else if ( val == 10 ){ skl = 11; txt = "Carpentry"; }
			else if ( val == 11 ){ skl = 12; txt = "Cartography"; }
			else if ( val == 12 ){ skl = 14; txt = "Cooking"; }
			else if ( val == 13 ){ skl = 16; txt = "Discordance"; }
			else if ( val == 14 ){ skl = 3; txt = "Druidism"; }
			else if ( val == 15 ){ skl = 55; txt = "Elementalism"; }
			else if ( val == 16 ){ skl = 18; txt = "Fencing"; }
			else if ( val == 17 ){ skl = 54; txt = "Fist Fighting"; }
			else if ( val == 18 ){ skl = 21; txt = "Focus"; }
			else if ( val == 19 ){ skl = 22; txt = "Forensics"; }
			else if ( val == 20 ){ skl = 23; txt = "Healing"; }
			else if ( val == 21 ){ skl = 24; txt = "Herding"; }
			else if ( val == 22 ){ skl = 25; txt = "Hiding"; }
			else if ( val == 23 ){ skl = 26; txt = "Inscription"; }
			else if ( val == 24 ){ skl = 13; txt = "Knightship"; }
			else if ( val == 25 ){ skl = 28; txt = "Lockpicking"; }
			else if ( val == 26 ){ skl = 29; txt = "Lumberjacking"; }
			else if ( val == 27 ){ skl = 31; txt = "Magery"; }
			else if ( val == 28 ){ skl = 32; txt = "Magic Resistance"; }
			else if ( val == 29 ){ skl = 5; txt = "Marksmanship"; }
			else if ( val == 30 ){ skl = 33; txt = "Meditation"; }
			else if ( val == 31 ){ skl = 27; txt = "Mercantile"; }
			else if ( val == 32 ){ skl = 34; txt = "Mining"; }
			else if ( val == 33 ){ skl = 35; txt = "Musicianship"; }
			else if ( val == 34 ){ skl = 36; txt = "Necromancy"; }
			else if ( val == 35 ){ skl = 37; txt = "Ninjitsu"; }
			else if ( val == 36 ){ skl = 38; txt = "Parrying"; }
			else if ( val == 37 ){ skl = 39; txt = "Peacemaking"; }
			else if ( val == 38 ){ skl = 40; txt = "Poisoning"; }
			else if ( val == 39 ){ skl = 41; txt = "Provocation"; }
			else if ( val == 40 ){ skl = 17; txt = "Psychology"; }
			else if ( val == 41 ){ skl = 42; txt = "Remove Trap"; }
			else if ( val == 42 ){ skl = 19; txt = "Seafaring"; }
			else if ( val == 43 ){ skl = 15; txt = "Searching"; }
			else if ( val == 44 ){ skl = 43; txt = "Snooping"; }
			else if ( val == 45 ){ skl = 44; txt = "Spiritualism"; }
			else if ( val == 46 ){ skl = 45; txt = "Stealing"; }
			else if ( val == 47 ){ skl = 46; txt = "Stealth"; }
			else if ( val == 48 ){ skl = 47; txt = "Swordsmanship"; }
			else if ( val == 49 ){ skl = 48; txt = "Tactics"; }
			else if ( val == 50 ){ skl = 49; txt = "Tailoring"; }
			else if ( val == 51 ){ skl = 4; txt = "Taming"; }
			else if ( val == 52 ){ skl = 50; txt = "Tasting"; }
			else if ( val == 53 ){ skl = 51; txt = "Tinkering"; }
			else if ( val == 54 ){ skl = 52; txt = "Tracking"; }
			else if ( val == 55 ){ skl = 53; txt = "Veterinary"; }

			else if ( val == 56 ){ skl = 0; txt = "Auto Title"; }

			else if ( val == 57 && player.IsTitanOfEther ){ skl = 66; txt = "Titan of Ether"; }

			if ( txt != "" )
			{
				int x; int y;

				if ( val < 24 ){ x = 15; y = 25 + (val*28); }
				else if ( val < 47 ){ x = 365; y = 25 + ((val-23)*28); }
				else { x = 700; y = 25 + ((val-46)*28); }

				if ( player.CharacterSkill == skl ){ btn = 4018; } else { btn = 3609; }
				if ( skl == 0 ){ skl = 99; y = y + 28;}
				if ( skl == 66 ){ y = y + 28;}

				AddButton(x, y, btn, btn, skl, GumpButtonType.Reply, 0);
				AddHtml( x+50, y, 252, 20, @"<BODY><BASEFONT Color=" + color + ">" + txt + "</BASEFONT></BODY>", (bool)false, (bool)false);
			}
		}
    }
}