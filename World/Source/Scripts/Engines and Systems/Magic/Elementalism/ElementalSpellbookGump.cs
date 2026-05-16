using System; 
using System.Collections; 
using Server; 
using Server.Items; 
using Server.Misc; 
using Server.Network; 
using Server.Spells; 
using Server.Spells.Elementalism; 
using Server.Prompts; 

namespace Server.Gumps 
{ 
	public class ElementalSpellbookGump : Gump
	{
		private ElementalSpellbook m_Book;

		public bool HasSpell(Mobile from, int spellID)
		{
			if ( m_Book.RootParentEntity == from )
				return (m_Book.HasSpell(spellID));
			else
				return false;
		}

		public ElementalSpellbookGump( Mobile from, ElementalSpellbook book, int page ) : base( 100, 100 )
		{
			m_Book = book;
			m_Book.EllyPage = page;

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			int hue = 2884;
			string fnt = ElementalSpell.FontColor( m_Book.ItemID );

			if ( m_Book.ItemID == 0x6713 ) // EARTH
			{
				hue = 2884;
			}
			else if ( m_Book.ItemID == 0x6715 ) // WATER
			{
				hue = 2876;
			}
			else if ( m_Book.ItemID == 0x6717 ) // AIR
			{
				hue = 2807;
			}
			else if ( m_Book.ItemID == 0x6719 ) // FIRE
			{
				hue = 2464;
			}

			AddPage(0);
			AddImage(0, 0, 11138, hue);
			AddImage(0, 0, 11152);
			AddImage(0, 0, 11147);

			int PriorPage = page - 1;
				if ( PriorPage < 1 ){ PriorPage = 37; }
			int NextPage = page + 1;
				if ( NextPage > 37 ){ NextPage = 1; }

			AddButton(24, 8, 11159, 11159, PriorPage, GumpButtonType.Reply, 0); 	// PAGE LEFT
			AddButton(295, 8, 11160, 11160, NextPage, GumpButtonType.Reply, 0); 	// PAGE RIGHT

			AddButton(40, 209, 2095, 2095, (500+page), GumpButtonType.Reply, 0);	// HELP

			if ( page == 1 ) // MAIN PAGE
			{
				AddHtml( 85, 12, 89, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>Elemental</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 207, 12, 89, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>Spellbook</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 34, 35, 133, 160, @"<BODY><BASEFONT Color=" + fnt + ">Known elemental spells can be cast by selecting the button next to each listed spell. A known spell will also have a red bookmark on its detail page you can click on to cast as well.</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 188, 35, 133, 160, @"<BODY><BASEFONT Color=" + fnt + ">Each spell has the power needed to cast, which is the amount of mana and stamina the caster needs. The bookmark below can be selected to view the information on how this magic works.</BASEFONT></BODY>", (bool)false, (bool)false);
			}
			else if ( page > 1 && page < 6 ) // SPELL LISTS
			{
				int id = 299;
				string seca = "I";
				string secb = "II";

				if ( page == 2 ){ 		id = 299; 	seca = "I";		secb = "II";	}
				else if ( page == 3 ){ 	id = 307; 	seca = "III";	secb = "IV";	}
				else if ( page == 4 ){ 	id = 315; 	seca = "V";		secb = "VI";	}
				else if ( page == 5 ){ 	id = 323; 	seca = "VII";	secb = "VIII";	}

				AddHtml( 59, 14, 100, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>Sphere " + seca + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 207, 14, 100, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>Sphere " + secb + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				id++;
				if ( HasSpell(from, id) ){ AddButton(35, 55, 2224, 2224, id, GumpButtonType.Reply, 0); }
				AddHtml( 60, 50, 102, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>" + ElementalSpell.CommonInfo( id, 1 ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(155, 55, 2104, 2104, (id-300+6), GumpButtonType.Reply, 0);

				id++;
				if ( HasSpell(from, id) ){ AddButton(35, 95, 2224, 2224, id, GumpButtonType.Reply, 0); }
				AddHtml( 60, 90, 102, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>" + ElementalSpell.CommonInfo( id, 1 ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(155, 95, 2104, 2104, (id-300+6), GumpButtonType.Reply, 0);

				id++;
				if ( HasSpell(from, id) ){ AddButton(35, 135, 2224, 2224, id, GumpButtonType.Reply, 0); }
				AddHtml( 60, 130, 102, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>" + ElementalSpell.CommonInfo( id, 1 ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(155, 135, 2104, 2104, (id-300+6), GumpButtonType.Reply, 0);

				id++;
				if ( HasSpell(from, id) ){ AddButton(35, 175, 2224, 2224, id, GumpButtonType.Reply, 0); }
				AddHtml( 60, 170, 102, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>" + ElementalSpell.CommonInfo( id, 1 ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(155, 175, 2104, 2104, (id-300+6), GumpButtonType.Reply, 0);

				id++;
				if ( HasSpell(from, id) ){ AddButton(190, 55, 2224, 2224, id, GumpButtonType.Reply, 0); }
				AddHtml( 215, 50, 102, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>" + ElementalSpell.CommonInfo( id, 1 ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(310, 55, 2104, 2104, (id-300+6), GumpButtonType.Reply, 0);

				id++;
				if ( HasSpell(from, id) ){ AddButton(190, 95, 2224, 2224, id, GumpButtonType.Reply, 0); }
				AddHtml( 215, 90, 102, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>" + ElementalSpell.CommonInfo( id, 1 ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(310, 95, 2104, 2104, (id-300+6), GumpButtonType.Reply, 0);

				id++;
				if ( HasSpell(from, id) ){ AddButton(190, 135, 2224, 2224, id, GumpButtonType.Reply, 0); }
				AddHtml( 215, 130, 102, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>" + ElementalSpell.CommonInfo( id, 1 ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(310, 135, 2104, 2104, (id-300+6), GumpButtonType.Reply, 0);

				id++;
				if ( HasSpell(from, id) ){ AddButton(190, 175, 2224, 2224, id, GumpButtonType.Reply, 0); }
				AddHtml( 215, 170, 102, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>" + ElementalSpell.CommonInfo( id, 1 ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(310, 175, 2104, 2104, (id-300+6), GumpButtonType.Reply, 0);
			}
			else // SPELL DESCRIPTIONS
			{
				int spell = 294 + page;
				string sphere = "I";
				int circle = 0;
				int skill = 0;
				int sect = 2;

				if ( spell >= 300 && spell <= 303 ){ 		sphere = "I"; 		circle = 1;		skill = 0;		sect = 2;	}
				else if ( spell >= 304 && spell <= 307 ){ 	sphere = "II"; 		circle = 2;		skill = 0;		sect = 2;	}
				else if ( spell >= 308 && spell <= 311 ){ 	sphere = "III"; 	circle = 3;		skill = 9;		sect = 3;	}
				else if ( spell >= 312 && spell <= 315 ){ 	sphere = "IV"; 		circle = 4;		skill = 23;		sect = 3;	}
				else if ( spell >= 316 && spell <= 319 ){ 	sphere = "V"; 		circle = 5;		skill = 38;		sect = 4;	}
				else if ( spell >= 320 && spell <= 323 ){ 	sphere = "VI"; 		circle = 6;		skill = 52;		sect = 4;	}
				else if ( spell >= 324 && spell <= 327 ){ 	sphere = "VII"; 	circle = 7;		skill = 66;		sect = 5;	}
				else if ( spell >= 328 && spell <= 331 ){ 	sphere = "VIII"; 	circle = 8;		skill = 80;		sect = 5;	}

				string power = (ElementalSpell.GetPower( circle-1 )).ToString();
				
				int spellIcon = ElementalSpell.SpellIcon( book.ItemID, spell );
				if ( HasSpell(from, spell) ){ AddButton(74, 86, spellIcon, spellIcon, spell, GumpButtonType.Reply, 0); }
				else AddImage( 74, 86, spellIcon );

				var spellName = ElementalSpell.CommonInfo( spell, 1 );

				AddHtml( 34, 13, 133, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG><CENTER>Elemental</CENTER></BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 34, 29, 133, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG><CENTER>" + spellName + "</CENTER></BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 26, 130, 144, 20, @"<BODY><BASEFONT Color=" + fnt + "><CENTER>[E" + spellName + "</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 34, 166, 100, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>Power:</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 139, 166, 38, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>" + power + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 34, 184, 100, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>Elementalism:</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 139, 184, 38, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>" + skill + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 190, 14, 100, 20, @"<BODY><BASEFONT Color=" + fnt + "><BIG>Sphere " + sphere + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 189, 38, 134, 159, @"<BODY><BASEFONT Color=" + fnt + ">" + ElementalSpell.DescriptionInfo( spell, m_Book.ItemID ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(35, 35, 2104, 2104, sect, GumpButtonType.Reply, 0);
			}
		}

		public override void OnResponse( NetState state, RelayInfo info ) 
		{
			Mobile from = state.Mobile; 
			from.SendSound( 0x55 );

			if ( info.ButtonID < 200 && info.ButtonID > 0 )
			{
				int page = info.ButtonID;
				if ( page < 1 ){ page = 37; }
				if ( page > 37 ){ page = 1; }
				from.SendGump( new ElementalSpellbookGump( from, m_Book, page ) );
			}
			else if ( info.ButtonID >= 500 )
			{
				from.SendGump( new ElementalSpellbookGump( from, m_Book, (info.ButtonID-500) ) );
				from.SendGump( new ElementalSpellHelp( from, m_Book, 1 ) );
			}
			else if ( info.ButtonID > 200 && HasSpell(from, info.ButtonID) )
			{
				if ( info.ButtonID == 300 ){ new Elemental_Armor_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 301 ){ new Elemental_Bolt_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 302 ){ new Elemental_Mend_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 303 ){ new Elemental_Sanctuary_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 304 ){ new Elemental_Pain_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 305 ){ new Elemental_Protection_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 306 ){ new Elemental_Purge_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 307 ){ new Elemental_Steed_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 308 ){ new Elemental_Call_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 309 ){ new Elemental_Force_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 310 ){ new Elemental_Wall_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 311 ){ new Elemental_Warp_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 312 ){ new Elemental_Field_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 313 ){ new Elemental_Restoration_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 314 ){ new Elemental_Strike_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 315 ){ new Elemental_Void_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 316 ){ new Elemental_Blast_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 317 ){ new Elemental_Echo_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 318 ){ new Elemental_Fiend_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 319 ){ new Elemental_Hold_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 320 ){ new Elemental_Barrage_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 321 ){ new Elemental_Rune_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 322 ){ new Elemental_Storm_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 323 ){ new Elemental_Summon_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 324 ){ new Elemental_Devastation_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 325 ){ new Elemental_Fall_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 326 ){ new Elemental_Gate_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 327 ){ new Elemental_Havoc_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 328 ){ new Elemental_Apocalypse_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 329 ){ new Elemental_Lord_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 330 ){ new Elemental_Soul_Spell( from, null ).Cast(); }
				else if ( info.ButtonID == 331 ){ new Elemental_Spirit_Spell( from, null ).Cast(); }

				from.SendGump( new ElementalSpellbookGump( from, m_Book, m_Book.EllyPage ) );
			}
		}
	}

	public class ElementalSpellHelp : Gump 
	{
		private Spellbook m_Book;

		public ElementalSpellHelp( Mobile from, Spellbook book, int page ) : base( 300, 200 )
		{
			m_Book = book;

			from.SendSound( 0x55 );

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			int img = 11143;
			int pic = 11148;
			string cat = "EARTH";
			string fnt = ElementalSpell.FontColor( m_Book.ItemID );

			if ( m_Book.ItemID == 0x6713 ) // EARTH
			{
				img = 11143;
				pic = 11148;
				cat = "EARTH";

			}
			else if ( m_Book.ItemID == 0x6715 ) // WATER
			{
				img = 11144;
				pic = 11149;
				cat = "WATER";
			}
			else if ( m_Book.ItemID == 0x6717 ) // AIR
			{
				img = 11145;
				pic = 11150;
				cat = "AIR";
			}
			else if ( m_Book.ItemID == 0x6719 ) // FIRE
			{
				img = 11146;
				pic = 11151;
				cat = "FIRE";
			}

			int btn = 4005;
			int pge = 2;
				if ( page == 2 ){ btn = 4014; pge = 1; }


			AddImage(0, 0, 7041, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddButton(608, 9, 4017, 4017, 0, GumpButtonType.Reply, 0);
			AddButton(567, 10, btn, btn, pge, GumpButtonType.Reply, 0);
			AddHtml( 12, 12, 530, 20, @"<BODY><BASEFONT Color=" + fnt + ">" + cat + " ELEMENTAL MAGIC</BASEFONT></BODY>", (bool)false, (bool)false);

			if ( page == 2 )
			{
				AddImage(14, 326, pic);
				AddHtml( 181, 118, 451, 360, @"<BODY><BASEFONT Color=" + fnt + ">[EArmor<BR>    Cast Elemental Armor<BR><BR>[EBolt<BR>    Cast Elemental Bolt<BR><BR>[EMend<BR>    Cast Elemental Mend<BR><BR>[ESanctuary<BR>    Cast Elemental Sanctuary<BR><BR>[EPain<BR>    Cast Elemental Pain<BR><BR>[EProtection<BR>    Cast Elemental Protection<BR><BR>[EPurge<BR>    Cast Elemental Purge<BR><BR>[ESteed<BR>    Cast Elemental Steed<BR><BR>[ECall<BR>    Cast Elemental Call<BR><BR>[EForce<BR>    Cast Elemental Force<BR><BR>[EWall<BR>    Cast Elemental Wall<BR><BR>[EWarp<BR>    Cast Elemental Warp<BR><BR>[EField<BR>    Cast Elemental Field<BR><BR>[ERestoration<BR>    Cast Elemental Restoration<BR><BR>[EStrike<BR>    Cast Elemental Strike<BR><BR>[EVoid<BR>    Cast Elemental Void<BR><BR>[EBlast<BR>    Cast Elemental Blast<BR><BR>[EEcho<BR>    Cast Elemental Echo<BR><BR>[EFiend<BR>    Cast Elemental Fiend<BR><BR>[EHold<BR>    Cast Elemental Hold<BR><BR>[EBarrage<BR>    Cast Elemental Barrage<BR><BR>[ERune<BR>    Cast Elemental Rune<BR><BR>[EStorm<BR>    Cast Elemental Storm<BR><BR>[ESummon<BR>    Cast Elemental Summon<BR><BR>[EDevastation<BR>    Cast Elemental Devastation<BR><BR>[EFall<BR>    Cast Elemental Fall<BR><BR>[EGate<BR>    Cast Elemental Gate<BR><BR>[EHavoc<BR>    Cast Elemental Havoc<BR><BR>[EApocalypse<BR>    Cast Elemental Apocalypse<BR><BR>[ELord<BR>    Cast Elemental Lord<BR><BR>[ESoul<BR>    Cast Elemental Soul<BR><BR>[ESpirit<BR>    Cast Elemental Spirit<BR><BR><BR><BR></BASEFONT></BODY>", (bool)false, (bool)true);
				AddHtml( 12, 44, 623, 60, @"<BODY><BASEFONT Color=" + fnt + ">Below are the [ commands you can either type to quickly cast a particular spell, or set a hot key to issue this command and cast the spell.</BASEFONT></BODY>", (bool)false, (bool)false);
			}
			else
			{
				AddImage(14, 365, img);
				string lowreg = "If you manage to find magical items that have a lower reagent quality, then the stamina required for spells will be reduced proportionally by that value. ";
					if ( MyServerSettings.LowerReg() < 1 )
						lowreg = "";

				AddHtml( 118, 47, 516, 433, @"<BODY><BASEFONT Color=" + fnt + ">Unlike other forms of magic, elementalism relies on both the intellect and physical condition of the spellcaster. No reagents are required, but casting these spells require an equal use of mana and stamina, referred to as 'power'. " + lowreg + "There are no supplement skills required for elementalists, like mages with their psychology or necromancers with their spiritualism. There is a guild you can join at the Lyceum, which is hidden on the Devil Guard mountains, with an illusionary spell. To get there, you need only cast the Sanctuary spell. This spell cannot be used everywhere, but as you learn more about elementalism you will be able to cast it in places like dungeons to quickly get to safety.<BR><BR>If you manage to visit the Lyceum, there are four shrines to the elements of Earth, Air, Water, and Fire. Each elementalist can only focus on one element at a time, and they can change their focus whenever they want. To do so, simply step up onto the shrine of your choice. If you want to change your clothing to match your focused element, simply speak the word 'culara'. Changing focus does not affect your spell library. This means that if you have 10 spells in your Earth Elemental Spellbook, and you change your focus to Water Elemental Magic, then your changed Water Elemental Spellbook will have the same 10 types of spells in it.<BR><BR>The only way to scribe elemental spells onto scrolls is by writing the magic words with daemon blood, and casting spells can be a tedious process, but it doesn't have to be. There is a secret that few elementalists know. You can have up to 2 custom spell bars that you can cast with. These spell bars keep your favorite spells at your fingertips, and allow you to cast them quicker. To learn the commands to access these secrets, you can find them by using the 'Help' button on the paperdoll.<BR><BR>This magic is said to have been forged by the Titans of the Elements, and many elementalists try to find their way into the Underworld to seek them out. What they desire are the spellbooks from the Titans, as they are very powerful. Most mages and elementalists have their spell casting often fail in the Underworld. Elementalists believe that equipping one of the Titan's spellbooks would allow them to explore the dark realm without this hinderance. Whether truth or falsehood, one can only try.<BR><BR>A word of warning in practicing other forms of magic, magery and necromancy cannot be bestowed upon the same elementalist. One knowing about these other forms of magic will cause any of the spells to fail when attempting to cast. This includes having items that enhance these types of skills. An aspiring spellcaster must choose a path and move toward that goal. You can either pursue elementalism, or you can learn about magery and necromancy. Elementalism knowledge even affects the forces of magic wands or runic magic. The pursuit of magic research is also something elementalists cannot achieve. If you find that you are losing interest in one of these schools of magic, and want to quickly pursue the other, then search for the Sorcerer Cave in Sosaria or the Conjurerer's Cave in Lodoria. They have crystals discovered centuries ago, that can help you forget what you learned in one area of magic so you can learn another.<BR><BR>Lastly, elemental magic is very sensitive to the forces surrounding the caster. The more armor you are wearing, the more likely your spells will fail. You either avoid wearing armor or find armor suited for spellcasters, that perhaps have a mage armor quality to them. Shields with spell channeling forces also help. Metal armor is the most obtrusive in this regard, where wooden armor is less detrimental. Leather armor is the least impactful, but finding quality clothing is the desired course.</BASEFONT></BODY>", (bool)false, (bool)true);
			}
		}

		public override void OnResponse( NetState state, RelayInfo info ) 
		{
			Mobile from = state.Mobile; 
			from.SendSound( 0x55 );

			if ( info.ButtonID > 0 )
			{
				int page = info.ButtonID;
				if ( page < 1 ){ page = 37; }
				if ( page > 37 ){ page = 1; }
				from.SendGump( new ElementalSpellHelp( from, m_Book, page ) );
			}
		}
	}
}