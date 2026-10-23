using Server.Items; 
using Server.Misc; 
using Server.Network; 
using Server.Spells; 
using Server.Spells.DeathKnight; 

namespace Server.Gumps 
{ 
	public class DeathKnightSpellbookGump : Gump 
	{
		private const int CAST_SPELL_ACTION_BASE = 10000;

		private DeathKnightSpellbook m_Book;

		private Map m_Map_1;
		private int m_X_1;
		private int m_Y_1;

		private Map m_Map_2;
		private int m_X_2;
		private int m_Y_2;

		public bool HasSpell( Mobile from, int spellID )
		{
			if ( m_Book.RootParentEntity == from )
				return (m_Book.HasSpell(spellID));
			else
				return false;
		}

		public DeathKnightSpellbookGump( Mobile from, DeathKnightSpellbook book, int page ) : base( 100, 100 ) 
		{
			from.PlaySound( 0x55 );
			m_Book = book;

			m_Map_1 = Map.Internal;
			m_X_1 = 0;
			m_Y_1 = 0;
			m_Map_2 = Map.Internal;
			m_X_2 = 0;
			m_Y_2 = 0;

			string color = "#df5e5e";

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			AddImage(0, 0, 7005, 2873);
			AddImage(0, 0, 7006);
			AddImage(0, 0, 7024, 2736);
			AddImage(69, 53, 7046);
			AddImage(373, 53, 7046);

			int PriorPage = page - 1;
				if ( PriorPage < 1 ){ PriorPage = 9; }
			int NextPage = page + 1;
				if ( NextPage > 9 ){ NextPage = 1; }

			AddButton(72, 45, 4014, 4014, PriorPage, GumpButtonType.Reply, 0);
			AddButton(590, 48, 4005, 4005, NextPage, GumpButtonType.Reply, 0);

			AddHtml( 107, 46, 186, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>DEATH MAGIC</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 398, 48, 186, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>DEATH MAGIC</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);

			if ( page == 1 )
			{
				const int MAX_SPELLS_PER_PAGE = 7;
				
				int x = 84;
				int y = 95;
				int o = 95;
				int v = 45;

				int index = 0;
				foreach (var definition in DeathKnightSpellProvider.SpellDefinitions)
				{
					if ( !HasSpell( from, definition.SpellID ) ) continue;

					AddHtml( x+30, y, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + definition.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(x, y-4, 7050, 7050, CAST_SPELL_ACTION_BASE + definition.SpellID, GumpButtonType.Reply, 0);
					y=y+v;

					// Flip to the next page
					if ( ++index == MAX_SPELLS_PER_PAGE ){ x = 382; y = o; }
				}
			}
			else if ( page == 9 )
			{
				string lowreg = "Magic from lower reagent properties can affect the amount of souls needed to invoke the magic. ";
					if ( MyServerSettings.LowerReg() < 1 )
						lowreg = "";

				var info = "In order to learn the ways of the Death Knight, you must master the art of Knightship while spreading evil deeds throughout the land, avoiding Karmic influences. One must seek out the 14 Disciple Knights of Kas, and learn the power they each mastered. Find their resting places, speak their names, and claim their skulls which contains the knowledge they had. Placing the skulls onto this book will increase its spell potential, but be quick about it. Anyone that calls forth their skull will cause it to appear no matter where it is in the land, taking it from another that may possess it. You will need the power of souls to use such magic. Find humanoid creatures like brigands, orcs, titans, goblins, or trolls...those that carry gold, and slay them while holding the lantern in your left hand. Although their gold will turn to dust, your lantern will increase in power that will drain as you use this magic. You do not need to hold the lantern while unleashing this power, but only when collecting souls. The lantern does not need to be in your possession either, as death magic will claim the souls from the lantern wherever it is. " + lowreg + "Although most magic relies on your Knightship skill alone, there are also some elements that will have greater effect the lower your Karma is. Go forth Death Knight, and bring our order back to this world. Beware, Death Knight. Powerful Death Knights are often not tolerated in the city streets and may be attacked on site.";

				AddHtml( 78, 80, 250, 314, @"<BODY><BASEFONT Color=" + color + ">" + info + "</BASEFONT></BODY>", (bool)false, (bool)true);

				info = "Quick bars for these spells can be set up in the Magic Toolbars section of [toolbars<BR><BR>You can use these spells by a typed command, which allows you to make macros for using these if you want. Type [Spells_DeathKnight to see each of these commands.";

				AddHtml( 366, 80, 250, 314, @"<BODY><BASEFONT Color=" + color + ">" + info + "</BASEFONT></BODY>", (bool)false, (bool)true);
			}
			else
			{
				m_Map_1 = Map.Internal;
				var haveLeftSpell = false;
				SpellDefinition leftSpell = null;
				string leftSpellLocation = null;

				m_Map_2 = Map.Internal;
				var haveRightSpell = false;
				SpellDefinition rightSpell = null;
				string rightSpellLocation = null;

				if ( page == 2 )
				{
					leftSpell = BanishSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetAreaEntrance( 0, "the Ancient Pyramid", Map.Sosaria, out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Saint Kargoth<BR>Land of Sosaria: Ancient Pyramid<BR>" + leftSpellGrave;
					}

					rightSpell = DemonicTouchSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetAreaEntrance( 0, "Dungeon Clues", Map.Sosaria, out m_Map_1, out m_X_1, out m_Y_1 );
						rightSpellLocation = "Lord Monduiz Dephaar<BR>Land of Sosaria: Dungeon Clues<BR>" + rightSpellGrave;
					}
				}
				else if ( page == 3 )
				{
					leftSpell = DevilPactSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetAreaEntrance( 0, "Dungeon Doom", Map.Sosaria, out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Lady Kath of Naelex<BR>Land of Sosaria: Dungeon Doom<BR>" + leftSpellGrave;
					}

					rightSpell = GrimReaperSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetAreaEntrance( 0, "the Fires of Hell", Map.Sosaria, out m_Map_1, out m_X_1, out m_Y_1 );
						rightSpellLocation = "Prince Myrhal of Rax<BR>Land of Sosaria: Fires of Hell<BR>" + rightSpellGrave;
					}
				}
				else if ( page == 4 )
				{
					leftSpell = HagHandSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetAreaEntrance( 0, "Dungeon Exodus", Map.Sosaria, out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Sir Maeril of Naelax<BR>Land of Sosaria: Dungeon Exodus<BR>" + leftSpellGrave;
					}

					rightSpell = HellfireSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetAreaEntrance( 0, "the City of the Dead", Map.Sosaria, out m_Map_1, out m_X_1, out m_Y_1 );
						rightSpellLocation = "Sir Farian of Lirtham<BR>Land of Ambrosia: City of the Dead<BR>" + rightSpellGrave;
					}
				}
				else if ( page == 5 )
				{
					leftSpell = LucifersBoltSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetAreaEntrance( 0, "the Mausoleum", Map.Sosaria, out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Lord Androma of Gara<BR>Island of Umber Veil: the Mausoleum<BR>" + leftSpellGrave;
					}

					rightSpell = OrbOfOrcusSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetAreaEntrance( 0, "Dungeon Despise", Map.Lodor, out m_Map_1, out m_X_1, out m_Y_1 );
						rightSpellLocation = "Sir Oslan Knarren<BR>Land of Lodoria: Dungeon Despise<BR>" + rightSpellGrave;
					}
				}
				else if ( page == 6 )
				{
					leftSpell = ShieldOfHateSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetAreaEntrance( 0, "Dungeon Deceit", Map.Lodor, out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Sir Rezinar of Haxx<BR>Land of Lodoria: Dungeon Deceit<BR>" + leftSpellGrave;
					}

					rightSpell = SoulReaperSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetAreaEntrance( 0, "Dungeon Wrong", Map.Lodor, out m_Map_1, out m_X_1, out m_Y_1 );
						rightSpellLocation = "Lord Thyrian of Naelax<BR>Land of Lodoria: Dungeon Wrong<BR>" + rightSpellGrave;
					}
				}
				else if ( page == 7 )
				{
					leftSpell = StrengthOfSteelSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetAreaEntrance( 0, "the Lodoria Catacombs", Map.Lodor, out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Sir Minar of Darmen<BR>Land of Lodoria: Lodoria Catacombs<BR>" + leftSpellGrave;
					}

					rightSpell = StrikeSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetAreaEntrance( 0, "Dungeon Shame", Map.Lodor, out m_Map_1, out m_X_1, out m_Y_1 );
						rightSpellLocation = "Duke Urkar of Torquann<BR>Land of Lodoria: Dungeon Shame<BR>" + rightSpellGrave;
					}
				}
				else if ( page == 8 )
				{
					leftSpell = SuccubusSkinSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetAreaEntrance( 0, "the City of Embers", Map.Lodor, out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Sir Luren the Boar<BR>Land of Lodoria: the City of Embers<BR>" + leftSpellGrave;
					}

					rightSpell = WrathSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetAreaEntrance( 0, "Dungeon Hythloth", Map.Lodor, out m_Map_1, out m_X_1, out m_Y_1 );
						rightSpellLocation = "Lord Khayven of Rax<BR>Land of Lodoria: Dungeon Hythloth<BR>" + rightSpellGrave;
					}
				}

				if ( leftSpell != null )
				{
					AddImage(73, 78, 7052);
					AddImage(75, 80, leftSpell.IconGraphic, 2405);
					AddHtml( 129, 93, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + leftSpell.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 134, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Souls:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 196, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + leftSpell.TithingCost + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 134, 160, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Skill:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 196, 160, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + leftSpell.MinSkill + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 134, 190, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Mana:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 196, 190, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + leftSpell.ManaCost + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 78, 220, 250, 175, @"<BODY><BASEFONT Color=" + color + ">" + (leftSpellLocation ?? "") + "<br><br>Mantra: " + leftSpell.PowerWords + "<BR><BR>" + leftSpell.Description + "</BASEFONT></BODY>", (bool)false, (bool)false);
				}

				if ( rightSpell != null )
				{
					AddImage(360, 78, 7052);
					AddImage(362, 80, rightSpell.IconGraphic, 2405);
					AddHtml( 417, 93, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + rightSpell.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 422, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Souls:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 484, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + rightSpell.TithingCost + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 422, 160, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Skill:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 484, 160, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + rightSpell.MinSkill + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 422, 190, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Mana:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 484, 190, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + rightSpell.ManaCost + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 366, 220, 250, 175, @"<BODY><BASEFONT Color=" + color + ">" + (rightSpellLocation ?? "") + "<br><br>Mantra: " + rightSpell.PowerWords + "<BR><BR>" + rightSpell.Description + "</BASEFONT></BODY>", (bool)false, (bool)false);
				}

				if ( Sextants.HasSextant( from ) && m_X_1 > 0 && !haveLeftSpell )
					AddButton(73, 368, 10461, 10461, 98000+page, GumpButtonType.Reply, 0);

				if ( Sextants.HasSextant( from ) && m_X_2 > 0 && !haveRightSpell )
					AddButton(592, 368, 10461, 10461, 99000+page, GumpButtonType.Reply, 0);
			}
		}

		public override void OnResponse( NetState state, RelayInfo info ) 
		{
			Mobile from = state.Mobile; 

			from.CloseGump( typeof( Sextants.MapGump ) );

			if ( info.ButtonID >= 99000 )
			{
				int pg = info.ButtonID - 99000;
				from.SendGump( new DeathKnightSpellbookGump( from, m_Book, pg ) );
				from.SendGump( new Sextants.MapGump( from, m_Map_2, m_X_2, m_Y_2, null ) );
			}
			else if ( info.ButtonID >= 98000 )
			{
				int pg = info.ButtonID - 98000;
				from.SendGump( new DeathKnightSpellbookGump( from, m_Book, pg ) );
				from.SendGump( new Sextants.MapGump( from, m_Map_1, m_X_1, m_Y_1, null ) );
			}
			else if ( info.ButtonID < CAST_SPELL_ACTION_BASE && info.ButtonID > 0 )
			{
				from.SendSound( 0x55 );
				int page = info.ButtonID;
				if ( page < 1 ){ page = 9; }
				if ( page > 9 ){ page = 1; }
				from.SendGump( new DeathKnightSpellbookGump( from, m_Book, page ) );
			}
			else
			{
				int spellID = info.ButtonID - CAST_SPELL_ACTION_BASE;
				if ( HasSpell( from, spellID ) )
				{
					DeathKnightSpellProvider.Cast(from, spellID);

					from.SendGump( new DeathKnightSpellbookGump( from, m_Book, 1 ) );
				}
				else
					from.SendSound( 0x55 );
			}
		}
	}
}