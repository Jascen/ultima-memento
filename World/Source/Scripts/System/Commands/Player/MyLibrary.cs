using System;
using Server.Network;
using Server.Mobiles;
using Server.Misc;
using Server.Items;

namespace Server.Gumps 
{
    public class MyLibrary : Gump
    {
		public int m_Origin;

		public MyLibrary ( Mobile from, int source ) : base ( 50, 50 )
		{
			m_Origin = source;

			if ( from.AccessLevel >= AccessLevel.GameMaster )
				((PlayerMobile)from).Preferences.MyLibrary = "1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#";

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			string color = "#ddbc4b";
			string mains = "#bc9090";

			AddPage(0);

			AddImage(0, 0, 9546, PlayerSettings.GetGumpHue( from ));

			AddHtml( 12, 12, 200, 20, @"<BODY><BASEFONT Color=" + color + ">LIBRARY</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(869, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

			int x = 16;
			int y = 52;

			int i = 235;

			int d = 30;

			int rows = 0;

			AddButton(x, y, 4011, 4011, 400, GumpButtonType.Reply, 0);
			AddHtml( x+38, y, 200, 20, @"<BODY><BASEFONT Color=" + mains + ">Basics</BASEFONT></BODY>", (bool)false, (bool)false);
			y=y+d;
			rows++;

			if ( from.RaceID > 0 )
			{
				AddButton(x, y, 4011, 4011, 401, GumpButtonType.Reply, 0);
				AddHtml( x+38, y, 200, 20, @"<BODY><BASEFONT Color=" + mains + ">Creature Help</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+d;
				rows++;
			}

			AddButton(x, y, 4011, 4011, 402, GumpButtonType.Reply, 0);
			AddHtml( x+38, y, 200, 20, @"<BODY><BASEFONT Color=" + mains + ">Fame & Karma</BASEFONT></BODY>", (bool)false, (bool)false);
			y=y+d;
			rows++;

			AddButton(x, y, 4011, 4011, 403, GumpButtonType.Reply, 0);
			AddHtml( x+38, y, 200, 20, @"<BODY><BASEFONT Color=" + mains + ">Item Properties</BASEFONT></BODY>", (bool)false, (bool)false);
			y=y+d;
			rows++;

			AddButton(x, y, 4011, 4011, 404, GumpButtonType.Reply, 0);
			AddHtml( x+38, y, 200, 20, @"<BODY><BASEFONT Color=" + mains + ">Skills</BASEFONT></BODY>", (bool)false, (bool)false);
			y=y+d;
			rows++;

			AddButton(x, y, 4011, 4011, 405, GumpButtonType.Reply, 0);
			AddHtml( x+38, y, 200, 20, @"<BODY><BASEFONT Color=" + mains + ">Weapon Abilities</BASEFONT></BODY>", (bool)false, (bool)false);
			y=y+d;
			rows++;

			string keys = PlayerSettings.ValLibraryConfig( from );

			if ( keys.Length > 0 )
			{
				string[] configures = keys.Split('#');
				int entry = 1;

                foreach (string key in configures)
                {
					if ( rows == 24 || rows == 48 || rows == 72 ){ x = x+i; y = 52; }

                    bool discovered = key == "1";
                    if ( discovered ) AddButton(x, y, 4011, 4011, entry, GumpButtonType.Reply, 0);
                    string title = discovered ? bookInfo( entry, 1 ) : "---------------";
                    AddHtml( x+38, y, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + title + "</BASEFONT></BODY>", (bool)false, (bool)false);
                    y=y+d;
                    rows++;
                    entry++;
                }
			}
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			from.CloseGump( typeof( MyLibrary ) );
			int button = info.ButtonID;

			if ( button > 0 )
			{
				int refer = Int32.Parse( bookInfo( button, 2 ) );
				string book = bookInfo( button, 0 );

				from.SendGump( new MyLibrary( from, m_Origin ) );

				if ( button >= 400 ) // BUILT IN HELP
				{
					if ( button == 400 ){ from.CloseGump( typeof( BeginnerBookGump ) ); from.SendGump( new BeginnerBookGump( from, 1 ) ); }
					else if ( button == 401 ){ from.CloseGump( typeof( CreatureHelpGump ) ); from.SendGump( new CreatureHelpGump( from, 0 ) ); }
					else if ( button == 402 ){ from.CloseGump( typeof( FameKarma ) ); from.SendGump( new FameKarma( from, 0 ) ); }
					else if ( button == 403 ){ from.CloseGump( typeof( ItemPropsGump ) ); from.SendGump( new ItemPropsGump( from, 0 ) ); }
					else if ( button == 404 ){ from.CloseGump( typeof( NewSkillsGump ) ); from.SendGump( new NewSkillsGump( from, 0 ) ); }
					else { from.CloseGump( typeof( WeaponAbilityBook.AbilityBookGump ) ); from.SendGump( new WeaponAbilityBook.AbilityBookGump( from ) ); }
				}
				else if ( refer >= 300 ) // SKULLS & SHACKLES
				{
					Item item = null;
					Type itemType = ScriptCompiler.FindTypeByName( book );
					item = (Item)Activator.CreateInstance(itemType);
					item.Weight = -50.0;
					item.OnDoubleClick(from);
					item.Delete();
				}
				else if ( refer >= 200 ) // SCROLLS
				{
					Item item = null;
					Type itemType = ScriptCompiler.FindTypeByName( book );
					item = (Item)Activator.CreateInstance(itemType);
					item.Weight = -50.0;
					item.OnDoubleClick(from);
					item.Delete();
				}
				else if ( refer >= 100 ) // DYNAMIC BOOKS
				{
					Item item = null;
					Type itemType = ScriptCompiler.FindTypeByName( book );
					item = (Item)Activator.CreateInstance(itemType);
					item.Weight = -50.0;
					item.OnDoubleClick(from);
					item.Delete();
				}
				else // LORE BOOKS
				{
					Item item = null;
					Type itemType = ScriptCompiler.FindTypeByName( book );
					item = (Item)Activator.CreateInstance(itemType);
					item.Weight = -50.0;
					if ( item is LoreBook ){ LoreBook lore = (LoreBook)item; lore.writeBook( refer ); }
					item.OnDoubleClick(from);
					item.Delete();
				}
			}
			else if ( m_Origin > 0 ){ from.SendGump( new Server.Engines.Help.HelpGump( from, 1 ) ); }
			else { from.SendSound( 0x4A ); }
		}

		public static string bookInfo( int val, int part )
		{
			string item = "";
			string title = "";
			int id = 0;

			switch ( val+1 )
			{
				case 2: item = "LoreBook"; title = "Akalabeth's Tale"; id = 0; break;
				case 3: item = "AlchemicalElixirs"; title = "Alchemical Elixirs"; id = 115; break;
				case 4: item = "AlchemicalMixtures"; title = "Alchemical Mixtures"; id = 116; break;
				case 5: item = "LoreBook"; title = "Antiquities"; id = 45; break;
				case 6: item = "LearnStealingBook"; title = "The Art of Thievery"; id = 202; break;
				case 7: item = "LoreBook"; title = "The Balance Vol I of II"; id = 2; break;
				case 8: item = "LoreBook"; title = "The Balance Vol II of II"; id = 3; break;
				case 9: item = "LoreBook"; title = "The Bard's Tale"; id = 32; break;
				case 10: item = "BookofDeadClue"; title = "Barge of the Dead"; id = 104; break;
				case 11: item = "LoreBook"; title = "The Black Gate Demon"; id = 4; break;
				case 12: item = "LoreBook"; title = "The Blue Ore"; id = 5; break;
				case 13: item = "BookBottleCity"; title = "The Bottle City"; id = 103; break;
				case 14: item = "LoreBook"; title = "Castles Above"; id = 27; break;
				case 15: item = "LoreBook"; title = "The Cruel Game"; id = 18; break;
				case 16: item = "LoreBook"; title = "Crystal Flasks"; id = 6; break;
				case 17: item = "LoreBook"; title = "The Curse of Mangar"; id = 22; break;
				case 18: item = "LoreBook"; title = "The Curse of the Island"; id = 7; break;
				case 19: item = "LoreBook"; title = "The Dark Age"; id = 8; break;
				case 20: item = "LoreBook"; title = "The Dark Core"; id = 9; break;
				case 21: item = "LoreBook"; title = "The Darkness Within"; id = 12; break;
				case 22: item = "LoreBook"; title = "Death Dealing"; id = 33; break;
				case 23: item = "LoreBook"; title = "The Death Knights"; id = 11; break;
				case 24: item = "LoreBook"; title = "Death to Pirates"; id = 10; break;
				case 25: item = "LoreBook"; title = "The Demon Shard"; id = 42; break;
				case 26: item = "LoreBook"; title = "The Destruction of Exodus"; id = 13; break;
				case 27: item = "LodorBook"; title = "Diary on Lodoria"; id = 109; break;
				case 28: item = "LoreBook"; title = "The Dragon's Egg"; id = 37; break;
				case 29: item = "LoreBook"; title = "The Elemental Titans"; id = 36; break;
				case 30: item = "CBookElvesandOrks"; title = "Elves and Orks"; id = 106; break;
				case 31: item = "LoreBook"; title = "The Fall of Mondain"; id = 15; break;
				case 32: item = "LoreBook"; title = "Forging the Fire"; id = 16; break;
				case 33: item = "LoreBook"; title = "Forgotten Dungeons"; id = 17; break;
				case 34: item = "LillyBook"; title = "Gargoyle Secrets"; id = 111; break;
				case 35: item = "LoreBook"; title = "Gem of Immortality"; id = 25; break;
				case 36: item = "LoreBook"; title = "The Gods of Men"; id = 26; break;
				case 37: item = "GoldenRangers"; title = "The Golden Rangers"; id = 114; break;
				case 38: item = "LearnTraps"; title = "Hidden Traps"; id = 112; break;
				case 39: item = "LoreBook"; title = "The Ice Queen"; id = 19; break;
				case 40: item = "LoreBook"; title = "The Jedi Order"; id = 46; break;
				case 41: item = "FamiliarClue"; title = "Journal on Familiars"; id = 108; break;
				case 42: item = "LoreBook"; title = "The Knight Who Fell"; id = 14; break;
				case 43: item = "LearnLeatherBook"; title = "Leather & Bone Crafts"; id = 207; break;
				case 44: item = "GreyJournal"; title = "Legend of the Sky Castle"; id = 119; break;
				case 45: item = "LoreBook"; title = "The Lost Land"; id = 1; break;
				case 46: item = "CBookTheLostTribeofSosaria"; title = "Lost Tribe of Sosaria"; id = 110; break;
				case 47: item = "LoreBook"; title = "Luck of the Rogue"; id = 20; break;
				case 48: item = "LoreBook"; title = "Magic in the Moon"; id = 38; break;
				case 49: item = "LoreBook"; title = "The Maze of Wonder"; id = 39; break;
				case 50: item = "LearnMetalBook"; title = "Metal Smithing & Tinkering"; id = 206; break;
				case 51: item = "LoreBook"; title = "The Orb of the Abyss"; id = 34; break;
				case 52: item = "LoreBook"; title = "The Pass of the Gods"; id = 40; break;
				case 53: item = "LoreBook"; title = "Rangers of Lodoria"; id = 24; break;
				case 54: item = "LearnReagentsBook"; title = "Reagents"; id = 204; break;
				case 55: item = "LearnScalesBook"; title = "Reptile Scale Crafts"; id = 203; break;
				case 56: item = "LoreBook"; title = "The Rule of One"; id = 44; break;
				case 57: item = "RuneJournal"; title = "Rune Magic"; id = 120; break;
				case 58: item = "LearnGraniteBook"; title = "Sand & Stone Crafts"; id = 208; break;
				case 59: item = "LearnMiscBook"; title = "Skinning & Carving"; id = 205; break;
				case 60: item = "SwordsAndShackles"; title = "Skulls and Shackles"; id = 300; break;
				case 61: item = "LoreBook"; title = "Staff of Five Parts"; id = 28; break;
				case 62: item = "LoreBook"; title = "The Story of Exodus"; id = 29; break;
				case 63: item = "LoreBook"; title = "The Story of Minax"; id = 30; break;
				case 64: item = "LoreBook"; title = "The Story of Mondain"; id = 31; break;
				case 65: item = "LoreBook"; title = "The Syth Order"; id = 43; break;
				case 66: item = "LearnTailorBook"; title = "Tailoring the Cloth"; id = 201; break;
				case 67: item = "LoreBook"; title = "Tattered Journal"; id = 21; break;
				case 68: item = "TendrinsJournal"; title = "Tendrin's Journal"; id = 100; break;
				case 69: item = "LoreBook"; title = "The Times of Minax"; id = 23; break;
				case 70: item = "LearnTitles"; title = "Titles of the Skilled"; id = 113; break;
				case 71: item = "CBookTombofDurmas"; title = "Tomb of Durmas"; id = 105; break;
				case 72: item = "LoreBook"; title = "The Underworld Gate"; id = 35; break;
				case 73: item = "LoreBook"; title = "Valley of Corruption"; id = 41; break;
				case 74: item = "BookOfPoisons"; title = "Venom and Poisons"; id = 117; break;
				case 75: item = "MagestykcClueBook"; title = "Wizards in Exile"; id = 107; break;
				case 76: item = "LearnWoodBook"; title = "Wooden Carvings"; id = 200; break;
				case 77: item = "WorkShoppes"; title = "Work Shoppes"; id = 118; break;
			}

			if ( part == 1 )
				return title;
			else if ( part == 2 )
				return "" + id + "";

			return item;
		}

		public static void readBook ( Item book, Mobile m )
		{
			bool effect = false;
			int num = 0;

			if ( book.Name == "Akalabeth's Tale" ){ num = 1; }
			else if ( book.Name == "Alchemical Elixirs" ){ num = 2; }
			else if ( book.Name == "Alchemical Mixtures" ){ num = 3; }
			else if ( book.Name == "Antiquities" ){ num = 4; }
			else if ( book.Name == "The Art of Thievery" ){ num = 5; }
			else if ( book.Name == "The Balance Vol I of II" ){ num = 6; }
			else if ( book.Name == "The Balance Vol II of II" ){ num = 7; }
			else if ( book.Name == "The Bard's Tale" ){ num = 8; }
			else if ( book.Name == "Barge of the Dead" ){ num = 9; }
			else if ( book.Name == "The Black Gate Demon" ){ num = 10; }
			else if ( book.Name == "The Blue Ore" ){ num = 11; }
			else if ( book.Name == "The Bottle City" ){ num = 12; }
			else if ( book.Name == "Castles Above" ){ num = 13; }
			else if ( book.Name == "The Cruel Game" ){ num = 14; }
			else if ( book.Name == "Crystal Flasks" ){ num = 15; }
			else if ( book.Name == "The Curse of Mangar" ){ num = 16; }
			else if ( book.Name == "The Curse of the Island" ){ num = 17; }
			else if ( book.Name == "The Dark Age" ){ num = 18; }
			else if ( book.Name == "The Dark Core" ){ num = 19; }
			else if ( book.Name == "The Darkness Within" ){ num = 20; }
			else if ( book.Name == "Death Dealing" ){ num = 21; }
			else if ( book.Name == "The Death Knights" ){ num = 22; }
			else if ( book.Name == "Death to Pirates" ){ num = 23; }
			else if ( book.Name == "The Demon Shard" ){ num = 24; }
			else if ( book.Name == "The Destruction of Exodus" ){ num = 25; }
			else if ( book is LodorBook ){ num = 26; }
			else if ( book.Name == "The Dragon's Egg" ){ num = 27; }
			else if ( book.Name == "The Elemental Titans" ){ num = 28; }
			else if ( book.Name == "Elves and Orks" ){ num = 29; }
			else if ( book.Name == "The Fall of Mondain" ){ num = 30; }
			else if ( book.Name == "Forging the Fire" ){ num = 31; }
			else if ( book.Name == "Forgotten Dungeons" ){ num = 32; }
			else if ( book.Name == "Gargoyle Secrets" ){ num = 33; }
			else if ( book.Name == "Gem of Immortality" ){ num = 34; }
			else if ( book.Name == "The Gods of Men" ){ num = 35; }
			else if ( book.Name == "The Golden Rangers" ){ num = 36; }
			else if ( book.Name == "Hidden Traps" ){ num = 37; }
			else if ( book.Name == "The Ice Queen" ){ num = 38; }
			else if ( book.Name == "The Jedi Order" ){ num = 39; }
			else if ( book.Name == "Journal on Familiars" ){ num = 40; }
			else if ( book.Name == "The Knight Who Fell" ){ num = 41; }
			else if ( book.Name == "Scroll of Various Leather" ){ num = 42; }
			else if ( book.Name == "Legend of the Sky Castle" ){ num = 43; }
			else if ( book.Name == "The Lost Land" ){ num = 44; }
			else if ( book.Name == "Lost Tribe of Sosaria" ){ num = 45; }
			else if ( book.Name == "Luck of the Rogue" ){ num = 46; }
			else if ( book.Name == "Magic in the Moon" ){ num = 47; }
			else if ( book.Name == "The Maze of Wonder" ){ num = 48; }
			else if ( book.Name == "Scroll of Various Metals" ){ num = 49; }
			else if ( book.Name == "The Orb of the Abyss" ){ num = 50; }
			else if ( book.Name == "The Pass of the Gods" ){ num = 51; }
			else if ( book.Name == "Rangers of Lodoria" ){ num = 52; }
			else if ( book.Name == "Scroll of Various Reagents" ){ num = 53; }
			else if ( book.Name == "Scroll of Reptile Scales" ){ num = 54; }
			else if ( book.Name == "The Rule of One" ){ num = 55; }
			else if ( book.Name == "Rune Magic" ){ num = 56; }
			else if ( book.Name == "Scroll of Sand and Stone" ){ num = 57; }
			else if ( book.Name == "Scroll of Skinning Creatures" ){ num = 58; }
			else if ( book.Name == "Skulls and Shackles" ){ num = 59; }
			else if ( book.Name == "Staff of Five Parts" ){ num = 60; }
			else if ( book.Name == "The Story of Exodus" ){ num = 61; }
			else if ( book.Name == "The Story of Minax" ){ num = 62; }
			else if ( book.Name == "The Story of Mondain" ){ num = 63; }
			else if ( book.Name == "The Syth Order" ){ num = 64; }
			else if ( book.Name == "Scroll of Tailoring" ){ num = 65; }
			else if ( book.Name == "A Tattered Journal" ){ num = 66; }
			else if ( book.Name == "Tendrin's Journal" ){ num = 67; }
			else if ( book.Name == "The Times of Minax" ){ num = 68; }
			else if ( book.Name == "Titles of the Skilled" ){ num = 69; }
			else if ( book.Name == "Tomb of Durmas" ){ num = 70; }
			else if ( book.Name == "The Underworld Gate" ){ num = 71; }
			else if ( book.Name == "Valley of Corruption" ){ num = 72; }
			else if ( book.Name == "Venom and Poisons" ){ num = 73; }
			else if ( book.Name == "Wizards in Exile" ){ num = 74; }
			else if ( book.Name == "Scroll of Various Wood" ){ num = 75; }
			else if ( book.Name == "Work Shoppes" ){ num = 76; }

			if ( num > 0 )
			{
				if ( !PlayerSettings.GetLibraryConfig( m, num ) )
				{
					PlayerSettings.SetLibraryConfig( m, num );
					effect = true;
				}
			}

			if ( effect )
			{
				Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 0, 0, 5024, 0 );
				m.SendSound( 0x65C );
				m.SendMessage( book.Name + " has been added to your library." );
				if (book.Movable && (book is GoldenRangers) == false) // Needed for quest
					book.Delete();
			}

		}
	}
}