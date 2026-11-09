using System; 
using System.Collections; 
using Server; 
using Server.Items; 
using Server.Misc; 
using Server.Network; 
using Server.Spells; 
using Server.Spells.Research; 
using Server.Prompts; 

namespace Server.Gumps 
{ 
	public class AncientSpellbookGump : Gump 
	{
		private AncientSpellbook m_Book; 
		private int m_Page;

		public bool HasSpell(Mobile from, int spellID)
		{
			if ( m_Book.RootParentEntity == from )
				return (m_Book.HasSpell(spellID));
			else
				return false;
		}

		public AncientSpellbookGump( Mobile from, AncientSpellbook book, int page ) : base( 100, 100 ) 
		{
			if ( book.Owner != from )
				return;

			from.PlaySound( 0x55 );
			m_Book = book;
			m_Page = page;
			string color = "#ffff00";

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			AddImage(0, 0, 7066, m_Book.Hue-1);
			AddImage(0, 0, 7006, 2988);
			AddImage(0, 0, 7024, 2736);
			AddImage(83, 110, 11279, 2381);
			AddImage(380, 110, 11279, 2381);

			if ( book.paper > 10 )
				AddImage(652, 42, 11192);
			else
				AddImage(652, 42, 11190);

			if ( book.quill > 10 )
				AddImage(652, 108, 11192);
			else
				AddImage(652, 108, 11190);

			if ( book.paper > 9999 )
				AddImage(665, 42, 11191);

			if ( book.quill > 9999 )
				AddImage(665, 108, 11191);

			AddItem(654, 66, 3636);
			AddItem(666, 124, 8273);

			AddHtml( 690, 67, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + book.paper + "</BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 690, 132, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + book.quill + "</BASEFONT></BODY>", (bool)false, (bool)false);

			int PriorPage = page - 1;
				if ( PriorPage < 1 ){ PriorPage = 37; }
			int NextPage = page + 1;
				if ( NextPage > 37 ){ NextPage = 1; }

			string info = "";

			AddButton(72, 45, 4014, 4014, PriorPage, GumpButtonType.Reply, 0);
			AddButton(590, 48, 4005, 4005, NextPage, GumpButtonType.Reply, 0);

			AddHtml( 107, 46, 186, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>" + spellHeader( page, 1 ) + "</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
				if ( page < 5 )
					AddImage(300, 48, spellImage( page, 1 ));
			AddHtml( 398, 48, 186, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>" + spellHeader( page, 2 ) + "</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
				if ( page < 5 )
					AddImage(362, 48, spellImage( page, 2 ));

			if ( page > 0 && page < 5 )
			{
				int SpellsOnPage = 16;
				int SafetyCatch = 0;
				int spellListing = 1;

				int x = 84;
				int y = 95;
				int o = 95;
				int v = 40;

				while ( SpellsOnPage > 0 )
				{
					SafetyCatch++;
					SpellsOnPage--;

					spellListing = spellList( page, SafetyCatch );
					AddHtml( x+30, y, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + Research.SpellInformation( spellListing, 2 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

					if ( HasSpell( from, Int32.Parse( Research.SpellInformation( spellListing, 12 ) ) ) )
						AddButton(x, y+2, 2117, 2117, Int32.Parse( Research.SpellInformation( spellListing, 12 ) ), GumpButtonType.Reply, 0);
					else
						AddImage(x, y+2, 2117, 2266);

					AddButton(x+200, y, 4011, 4011, spellPage( SafetyCatch ), GumpButtonType.Reply, 0);
					AddImage(x+200, y, 4011, 2266);

					y=y+v;

					if ( SafetyCatch == 8 ){ x = 382; y = o; }

					if ( SafetyCatch > 16 ){ SpellsOnPage = 0; }
				}
			}
			else if ( page == 37 )
			{
				info = "This book will be yours alone to use and you can only have one book at any given time. Creating a new book will cause this book to crumple to dust. Using ancient spellbooks requires the caster to carry reagents with them. The book pages may also turn to dust when spells are cast, so the wizard will need to keep extra pages (blank scrolls) within the book. They will also need to keep quills (scribe pens) set aside as well. Simply place these items on this book in order to maintain a supply of each. If you run out, you will not be able to cast any spells until you acquire more. You might find that you don't consume any scrolls or quills when reagent lowering attributes exist. These books can be equipped like other spellbooks.<BR><BR>" + Server.Misc.ResearchSettings.BagOrBook() + "<BR><BR>Spells are cast with those skilled in either magery or necromancy, whichever is higher. The effectiveness of the spells is dependent on the combination of magery, necromancy, spiritualism, and psychology. If you are simply skilled in only a couple of these skills, then the spells will have only an average effect. It is those that pursue all four of the skills of wizardry, that will gain the most benefit. When ancient spells are performed, it helps a researcher practice inscription, magery, necromancy, spiritualism, and psychology at the same time. This is why ancient spell research interests archmages, as they have achieved the level of grandmaster in both areas of magic. Some ancient magic has similarities to spells used today, as is to be expected that some of the knowledge survived the ages. So very few spells will be similar to current magery spells, and even fewer spells that are similar to modern necromancer spells. Although they are similar, the ancient spell usually proves to be much more powerful.";

				AddHtml( 78, 80, 250, 314, @"<BODY><BASEFONT Color=" + color + ">" + info + "</BASEFONT></BODY>", (bool)false, (bool)true);

				info = "Magic Toolbars: Here are the commands you can use (include the bracket) to manage magic toolbars that might help you play better.<BR><BR>[ancient - Switches between using the book or bag.<BR><BR>[archspell1 - Opens the 1st ancient spell bar editor.<BR><BR>[archspell2 - Opens the 2nd ancient spell bar editor.<BR><BR>[archspell3 - Opens the 3rd ancient spell bar editor.<BR><BR>[archspell4 - Opens the 4th ancient spell bar editor.<BR><BR>[archtool1 - Opens the 1st ancient spell bar.<BR><BR>[archtool2 - Opens the 2nd ancient spell bar.<BR><BR>[archtool3 - Opens the 3rd ancient spell bar.<BR><BR>[archtool4 - Opens the 4th ancient spell bar.<BR><BR>[archclose1 - Closes the 1st ancient spell bar.<BR><BR>[archclose2 - Closes the 2nd ancient spell bar.<BR><BR>[archclose3 - Closes the 3rd ancient spell bar.<BR><BR>[archclose4 - Closes the 4th ancient spell bar.<BR><BR>Below are the [ commands you can either type to quickly cast a particular spell, or set a hot key to issue this command and cast the spell." + Server.Misc.ResearchSettings.AncientKeywords() + "";

				AddHtml( 366, 80, 250, 314, @"<BODY><BASEFONT Color=" + color + ">" + info + "</BASEFONT></BODY>", (bool)false, (bool)true);
			}
			else
			{
				// --------------------------------------------------------------------------------

				int spellName = spellSection( page );
				string description = "";
				bool spellNav = bool.Parse( Research.SpellInformation( spellName, 13 ) );

				// --------------------------------------------------------------------------------

				AddImage(75, 80, Int32.Parse( Research.SpellInformation( spellName, 11 ) ));

				if ( HasSpell( from, Int32.Parse( Research.SpellInformation( spellName, 12 ) ) ) )
					AddButton(80, 132, 2117, 2117, Int32.Parse( Research.SpellInformation( spellName, 12 ) ), GumpButtonType.Reply, 0);
				else
					AddImage(80, 132, 2117, 2266);

				AddImage(285, 85, Int32.Parse( Research.SpellInformation( spellName, 10 ) ));

				AddHtml( 129, 93, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + Research.SpellInformation( spellName, 2 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 134, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Skill:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 196, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + Research.SpellInformation( spellName, 8 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 236, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Mana:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 298, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + Research.SpellInformation( spellName, 7 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 78, 160, 250, 60, @"<BODY><BASEFONT Color=" + color + ">Reagents: " + Research.SpellInformation( spellName, 5 ) + ".</BASEFONT></BODY>", (bool)false, (bool)false);

				description = Research.SpellInformation( spellName, 6 );

				if ( description.Contains("That means that the spell scroll will always crumble to dust when cast") )
					description = description.Replace("That means that the spell scroll will always crumble to dust when cast", "That means that the spell will always consume the reagents when cast");

				else if ( description.Contains("spell is powerful enough that the scroll will always crumble to dust when cast") )
					description = description.Replace("spell is powerful enough that the scroll will always crumble to dust when cast", "spell is powerful enough that it will always consume the reagents when cast");

				AddHtml( 78, 210, 250, 185, @"<BODY><BASEFONT Color=" + color + ">" + description + "</BASEFONT></BODY>", (bool)false, spellNav);

				AddButton(75, 421, 2094, 2094, spellTofC( page ), GumpButtonType.Reply, 0);

				// --------------------------------------------------------------------------------

				spellName = spellName + 8;
				spellNav = bool.Parse( Research.SpellInformation( spellName, 13 ) );

				// --------------------------------------------------------------------------------

				AddImage(362, 80, Int32.Parse( Research.SpellInformation( spellName, 11 ) ));

				if ( HasSpell( from, Int32.Parse( Research.SpellInformation( spellName, 12 ) ) ) )
					AddButton(367, 132, 2117, 2117, Int32.Parse( Research.SpellInformation( spellName, 12 ) ), GumpButtonType.Reply, 0);
				else
					AddImage(367, 132, 2117, 2266);

				AddImage(572, 85, Int32.Parse( Research.SpellInformation( spellName, 10 ) ));

				AddHtml( 417, 93, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + Research.SpellInformation( spellName, 2 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 422, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Skill:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 484, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + Research.SpellInformation( spellName, 8 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 524, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Mana:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 586, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + Research.SpellInformation( spellName, 7 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 366, 160, 250, 60, @"<BODY><BASEFONT Color=" + color + ">Reagents: " + Research.SpellInformation( spellName, 5 ) + ".</BASEFONT></BODY>", (bool)false, (bool)false);

				description = Research.SpellInformation( spellName, 6 );

				if ( description.Contains("That means that the spell scroll will always crumble to dust when cast") )
					description = description.Replace("That means that the spell scroll will always crumble to dust when cast", "That means that the spell will always consume the reagents when cast");

				else if ( description.Contains("spell is powerful enough that the scroll will always crumble to dust when cast") )
					description = description.Replace("spell is powerful enough that the scroll will always crumble to dust when cast", "spell is powerful enough that it will always consume the reagents when cast");

				AddHtml( 366, 210, 250, 185, @"<BODY><BASEFONT Color=" + color + ">" + description + "</BASEFONT></BODY>", (bool)false, spellNav);

				// --------------------------------------------------------------------------------
			}
		}

		public string spellHeader( int page, int entry )
		{
			if ( ( page == 1 && entry == 1 ) || ( page >= 5 && page <= 8 ) )
				return "CONJURATION MAGIC";

			else if ( ( page == 1 && entry == 2 ) || ( page >= 9 && page <= 12 ) )
				return "DEATH MAGIC";

			else if ( ( page == 2 && entry == 1 ) || ( page >= 13 && page <= 16 ) )
				return "ENCHANTING MAGIC";

			else if ( ( page == 2 && entry == 2 ) || ( page >= 17 && page <= 20 ) )
				return "SORCERY MAGIC";

			else if ( ( page == 3 && entry == 1 ) || ( page >= 21 && page <= 24 ) )
				return "SUMMONING MAGIC";

			else if ( ( page == 3 && entry == 2 ) || ( page >= 25 && page <= 28 ) )
				return "THAUMATURGY MAGIC";

			else if ( ( page == 4 && entry == 1 ) || ( page >= 29 && page <= 32 ) )
				return "THEURGY MAGIC";

			else if ( ( page == 4 && entry == 2 ) || ( page >= 33 && page <= 36 ) )
				return "WIZARDRY MAGIC";

			return "ANCIENT MAGIC";
		}

		public int spellImage( int page, int entry )
		{
			if ( page == 1 && entry == 1 )
				return 11260;

			else if ( page == 1 && entry == 2 )
				return 11259;

			else if ( page == 2 && entry == 1 )
				return 11268;

			else if ( page == 2 && entry == 2 )
				return 11267;

			else if ( page == 3 && entry == 1 )
				return 11262;

			else if ( page == 3 && entry == 2 )
				return 11266;

			else if ( page == 4 && entry == 1 )
				return 11263;

			else if ( page == 4 && entry == 2 )
				return 11261;

			return 11260;
		}

		public int spellPage( int entry )
		{
			if ( m_Page == 1 )
			{
				if ( entry == 1 || entry == 2 ){ 			return 5; }
				else if ( entry == 3 || entry == 4 ){ 		return 6; }
				else if ( entry == 5 || entry == 6 ){ 		return 7; }
				else if ( entry == 7 || entry == 8 ){ 		return 8; }
				else if ( entry == 9 || entry == 10 ){ 		return 9; }
				else if ( entry == 11 || entry == 12 ){ 	return 10; }
				else if ( entry == 13 || entry == 14 ){ 	return 11; }
				else { 										return 12; }
			}
			else if ( m_Page == 2 )
			{
				if ( entry == 1 || entry == 2 ){ 			return 13; }
				else if ( entry == 3 || entry == 4 ){ 		return 14; }
				else if ( entry == 5 || entry == 6 ){ 		return 15; }
				else if ( entry == 7 || entry == 8 ){ 		return 16; }
				else if ( entry == 9 || entry == 10 ){ 		return 17; }
				else if ( entry == 11 || entry == 12 ){ 	return 18; }
				else if ( entry == 13 || entry == 14 ){ 	return 19; }
				else { 										return 20; }
			}
			else if ( m_Page == 3 )
			{
				if ( entry == 1 || entry == 2 ){ 			return 21; }
				else if ( entry == 3 || entry == 4 ){ 		return 22; }
				else if ( entry == 5 || entry == 6 ){ 		return 23; }
				else if ( entry == 7 || entry == 8 ){ 		return 24; }
				else if ( entry == 9 || entry == 10 ){ 		return 25; }
				else if ( entry == 11 || entry == 12 ){ 	return 26; }
				else if ( entry == 13 || entry == 14 ){ 	return 27; }
				else { 										return 28; }
			}
			else if ( m_Page == 4 )
			{
				if ( entry == 1 || entry == 2 ){ 			return 29; }
				else if ( entry == 3 || entry == 4 ){ 		return 30; }
				else if ( entry == 5 || entry == 6 ){ 		return 31; }
				else if ( entry == 7 || entry == 8 ){ 		return 32; }
				else if ( entry == 9 || entry == 10 ){ 		return 33; }
				else if ( entry == 11 || entry == 12 ){ 	return 34; }
				else if ( entry == 13 || entry == 14 ){ 	return 35; }
				else { 										return 36; }
			}

			return 1;
		}

		public int spellSection( int page )
		{
			if ( page < 6 ){ 		return 1; }		else if ( page == 6 ){  return 17; }	else if ( page == 7 ){  return 33; }	else if ( page == 8 ){  return 49; }
			else if ( page == 9 ){  return 2; }		else if ( page == 10 ){ return 18; }	else if ( page == 11 ){ return 34; }	else if ( page == 12 ){ return 50; }
			else if ( page == 13 ){ return 3; }		else if ( page == 14 ){ return 19; }	else if ( page == 15 ){ return 35; }	else if ( page == 16 ){ return 51; }
			else if ( page == 17 ){ return 4; }		else if ( page == 18 ){ return 20; }	else if ( page == 19 ){ return 36; }	else if ( page == 20 ){ return 52; }
			else if ( page == 21 ){ return 5; }		else if ( page == 22 ){ return 21; }	else if ( page == 23 ){ return 37; }	else if ( page == 24 ){ return 53; }
			else if ( page == 25 ){ return 6; }		else if ( page == 26 ){ return 22; }	else if ( page == 27 ){ return 38; }	else if ( page == 28 ){ return 54; }
			else if ( page == 29 ){ return 7; }		else if ( page == 30 ){ return 23; }	else if ( page == 31 ){ return 39; }	else if ( page == 32 ){ return 55; }
			else if ( page == 33 ){ return 8; }		else if ( page == 34 ){ return 24; }	else if ( page == 35 ){ return 40; }	else if ( page == 36 ){ return 56; }

			return 1;
		}

		public int spellTofC( int page )
		{
			if ( page > 4 && page < 13 )
				return 1; 
			else if ( page > 12 && page < 21 )
				return 2; 
			else if ( page > 20 && page < 29 )
				return 3; 

			return 4;
		}

		public int spellList( int page, int entry )
		{
			if ( page == 1 && entry == 1 ){ return 1; }
			else if ( page == 1 && entry == 2 ){ return 9; }
			else if ( page == 1 && entry == 3 ){ return 17; }
			else if ( page == 1 && entry == 4 ){ return 25; }
			else if ( page == 1 && entry == 5 ){ return 33; }
			else if ( page == 1 && entry == 6 ){ return 41; }
			else if ( page == 1 && entry == 7 ){ return 49; }
			else if ( page == 1 && entry == 8 ){ return 57; }
			else if ( page == 1 && entry == 9 ){ return 2; }
			else if ( page == 1 && entry == 10 ){ return 10; }
			else if ( page == 1 && entry == 11 ){ return 18; }
			else if ( page == 1 && entry == 12 ){ return 26; }
			else if ( page == 1 && entry == 13 ){ return 34; }
			else if ( page == 1 && entry == 14 ){ return 42; }
			else if ( page == 1 && entry == 15 ){ return 50; }
			else if ( page == 1 && entry == 16 ){ return 58; }
			else if ( page == 2 && entry == 1 ){ return 3; }
			else if ( page == 2 && entry == 2 ){ return 11; }
			else if ( page == 2 && entry == 3 ){ return 19; }
			else if ( page == 2 && entry == 4 ){ return 27; }
			else if ( page == 2 && entry == 5 ){ return 35; }
			else if ( page == 2 && entry == 6 ){ return 43; }
			else if ( page == 2 && entry == 7 ){ return 51; }
			else if ( page == 2 && entry == 8 ){ return 59; }
			else if ( page == 2 && entry == 9 ){ return 4; }
			else if ( page == 2 && entry == 10 ){ return 12; }
			else if ( page == 2 && entry == 11 ){ return 20; }
			else if ( page == 2 && entry == 12 ){ return 28; }
			else if ( page == 2 && entry == 13 ){ return 36; }
			else if ( page == 2 && entry == 14 ){ return 44; }
			else if ( page == 2 && entry == 15 ){ return 52; }
			else if ( page == 2 && entry == 16 ){ return 60; }
			else if ( page == 3 && entry == 1 ){ return 5; }
			else if ( page == 3 && entry == 2 ){ return 13; }
			else if ( page == 3 && entry == 3 ){ return 21; }
			else if ( page == 3 && entry == 4 ){ return 29; }
			else if ( page == 3 && entry == 5 ){ return 37; }
			else if ( page == 3 && entry == 6 ){ return 45; }
			else if ( page == 3 && entry == 7 ){ return 53; }
			else if ( page == 3 && entry == 8 ){ return 61; }
			else if ( page == 3 && entry == 9 ){ return 6; }
			else if ( page == 3 && entry == 10 ){ return 14; }
			else if ( page == 3 && entry == 11 ){ return 22; }
			else if ( page == 3 && entry == 12 ){ return 30; }
			else if ( page == 3 && entry == 13 ){ return 38; }
			else if ( page == 3 && entry == 14 ){ return 46; }
			else if ( page == 3 && entry == 15 ){ return 54; }
			else if ( page == 3 && entry == 16 ){ return 62; }
			else if ( page == 4 && entry == 1 ){ return 7; }
			else if ( page == 4 && entry == 2 ){ return 15; }
			else if ( page == 4 && entry == 3 ){ return 23; }
			else if ( page == 4 && entry == 4 ){ return 31; }
			else if ( page == 4 && entry == 5 ){ return 39; }
			else if ( page == 4 && entry == 6 ){ return 47; }
			else if ( page == 4 && entry == 7 ){ return 55; }
			else if ( page == 4 && entry == 8 ){ return 63; }
			else if ( page == 4 && entry == 9 ){ return 8; }
			else if ( page == 4 && entry == 10 ){ return 16; }
			else if ( page == 4 && entry == 11 ){ return 24; }
			else if ( page == 4 && entry == 12 ){ return 32; }
			else if ( page == 4 && entry == 13 ){ return 40; }
			else if ( page == 4 && entry == 14 ){ return 48; }
			else if ( page == 4 && entry == 15 ){ return 56; }
			else if ( page == 4 && entry == 16 ){ return 64; }

			return 1;
		}

		public override void OnResponse( NetState state, RelayInfo info ) 
		{
			Mobile from = state.Mobile; 

			if ( info.ButtonID < 100 && info.ButtonID > 0 )
			{
				from.SendSound( 0x55 );
				int page = info.ButtonID;
				if ( page < 1 ){ page = 37; }
				if ( page > 37 ){ page = 1; }
				from.CloseGump( typeof( AncientSpellbookGump ) );
				from.SendGump( new AncientSpellbookGump( from, m_Book, page ) );
			}
			else if ( info.ButtonID >= ResearchSpellProvider.FirstSpellId && info.ButtonID <= ResearchSpellProvider.LastSpellId && HasSpell(from, info.ButtonID) )
			{
				Spell spell = SpellRegistry.NewSpell( info.ButtonID, from, m_Book );

				if ( spell != null )
					spell.Cast();
				else
					from.SendLocalizedMessage( 502345 ); // This spell has been temporarily disabled.

				from.CloseGump( typeof( AncientSpellbookGump ) );
				from.SendGump( new AncientSpellbookGump( from, m_Book, m_Page ) );
			}
			else
				from.PlaySound( 0x55 );
		}
	}
}