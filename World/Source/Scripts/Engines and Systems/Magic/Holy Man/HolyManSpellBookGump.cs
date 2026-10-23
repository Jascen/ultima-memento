using Server.Items; 
using Server.Misc; 
using Server.Network;
using Server.Spells;
using Server.Spells.HolyMan; 

namespace Server.Gumps 
{ 
	public class HolyManSpellbookGump : Gump 
	{
		private const int CAST_SPELL_ACTION_BASE = 10000;

		private HolyManSpellbook m_Book; 

		private Map m_Map_1;
		private int m_X_1;
		private int m_Y_1;

		private Map m_Map_2;
		private int m_X_2;
		private int m_Y_2;

		public bool HasSpell(Mobile from, int spellID)
		{
			if ( m_Book.RootParentEntity == from )
				return (m_Book.HasSpell(spellID));
			else
				return false;
		}

		public HolyManSpellbookGump( Mobile from, HolyManSpellbook book, int page ) : base( 100, 100 ) 
		{
			from.PlaySound( 0x55 );
			m_Book = book;
			string color = "#dddddd";

			m_Map_1 = Map.Internal;
			m_X_1 = 0;
			m_Y_1 = 0;
			m_Map_2 = Map.Internal;
			m_X_2 = 0;
			m_Y_2 = 0;

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			AddImage(0, 0, 7005, 2995);
			AddImage(0, 0, 7006);
			AddImage(0, 0, 7024, 2736);
			AddImage(131, 125, 7051);
			AddImage(431, 125, 7051);

			int PriorPage = page - 1;
				if ( PriorPage < 1 ){ PriorPage = 9; }
			int NextPage = page + 1;
				if ( NextPage > 9 ){ NextPage = 1; }

			string info = "";

			AddButton(72, 45, 4014, 4014, PriorPage, GumpButtonType.Reply, 0);
			AddButton(590, 48, 4005, 4005, NextPage, GumpButtonType.Reply, 0);

			AddHtml( 107, 46, 186, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>PRAYER BOOK</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 398, 48, 186, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>PRAYER BOOK</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);

			if ( page == 1 )
			{
				const int MAX_SPELLS_PER_PAGE = 7;

				int x = 84;
				int y = 95;
				int o = 95;
				int v = 45;

				int index = 0;
				foreach (var definition in HolyManSpellProvider.SpellDefinitions)
				{
					if ( !HasSpell( from, definition.SpellID ) ) continue;

					AddHtml( x+30, y, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + definition.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(x, y-4, 7049, 7049, CAST_SPELL_ACTION_BASE + definition.SpellID, GumpButtonType.Reply, 0);
					y=y+v;

					// Flip to the next page
					if ( ++index == MAX_SPELLS_PER_PAGE ){ x = 382; y = o; }
				}
			}
			else if ( page == 9 )
			{
				string lowreg = "Magic from lower reagent properties can affect the amount of piety needed to invoke the prayer. ";
					if ( MyServerSettings.LowerReg() < 1 )
						lowreg = "";

				info = "In order to learn the ways of the light, you must pursue proficiency in healing and spiritualism. One must seek out the graves of 14 priests, which are spread throughout the lands. Find their resting places, speak their mantra, and claim theirholy symbols which contains the power granted from the gods. Placing the symbols onto this book will add the prayer, but be quick about it. Anyone that calls forth their symbols will cause it to appear no matter where it is in the land, taking it from another that may possess it. You will need to banish evil to use such prayers. Find creatures like demons and the undead...those that carry gold, and slay them while holding the symbol where trinkets go. Although their gold will vanish, your symbol will increase in piety that will deplete as you use these prayers. You do not need to hold the symbol while praying, but only when dispatching such evil. The symbol does not need to be in your possession either, as prayers will use the piety wherever it is. " + lowreg + "Although most prayers rely on your Spiritualism skill alone, there are also some elements that will have greater effect based on your Healing skill. Go forth Priest, and rid the world of evil.";

				AddHtml( 78, 80, 250, 314, @"<BODY><BASEFONT Color=" + color + ">" + info + "</BASEFONT></BODY>", (bool)false, (bool)true);

				info = "Quick bars for these prayers can be set up in the Magic Toolbars section of [toolbars<BR><BR>You can use these prayers by a typed command, which allows you to make macros for using these if you want. Type [Spells_HolyMan to see each of these commands.";

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
					leftSpell = BanishEvilSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetTown( 0, "the Village of Springvale", out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Patriarch Morden rests south of the Village of Springvale<br>" + leftSpellGrave;
					}

					rightSpell = DampenSpiritSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetTown( 0, "the Village of Whisper", out m_Map_2, out m_X_2, out m_Y_2 );
						rightSpellLocation = "Archbishop Halyrn rests by the Village of Whisper<br>" + rightSpellGrave;
					}
				}
				else if ( page == 3 )
				{
					leftSpell = EnchantSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetTown( 0, "the City of Kuldara", out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Bishop Leantre rests in the Kuldar Cemetery<br>" + leftSpellGrave;
					}

					rightSpell = HammerOfFaithSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetTown( 0, "the City of Elidor", out m_Map_2, out m_X_2, out m_Y_2 );
						rightSpellLocation = "Deacon Wilems rests in the City of Elidor<br>" + rightSpellGrave;
					}
				}
				else if ( page == 4 )
				{
					leftSpell = HeavenlyLightSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetTown( 0, "the City of Britain", out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Drumat the Apostle rests by the City of Britain<br>" + leftSpellGrave;
					}

					rightSpell = NourishSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetTown( 0, "the Town of Moon", out m_Map_2, out m_X_2, out m_Y_2 );
						rightSpellLocation = "Vincent the Priest rests by the Town of Moon<br>" + rightSpellGrave;
					}
				}
				else if ( page == 5 )
				{
					leftSpell = PurgeSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetTown( 0, "the Town of Renika", out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Abigayl the Preacher rests near the Church of the Divine in the Town of Renika<br>" + leftSpellGrave;
					}

					rightSpell = RebirthSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetTown( 0, "Greensky Village", out m_Map_2, out m_X_2, out m_Y_2 );
						rightSpellLocation = "Cardinal Greggs rests near the Greensky Village<br>" + rightSpellGrave;
					}
				}
				else if ( page == 6 )
				{
					leftSpell = SacredBoonSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetTown( 0, "the Village of Grey", out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Father Michal rests by the Village of Grey<br>" + leftSpellGrave;
					}

					rightSpell = SanctifySpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetTown( 0, "the City of Montor", out m_Map_2, out m_X_2, out m_Y_2 );
						rightSpellLocation = "Sister Tiana rests south of the City of Montor<br>" + rightSpellGrave;
					}
				}
				else if ( page == 7 )
				{
					leftSpell = SeanceSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetTown( 0, "the Village of Islegem", out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Brother Kurklan rests near the Village of Islegem<br>" + leftSpellGrave;
					}

					rightSpell = SmiteSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetTown( 0, "the City of Lodoria", out m_Map_2, out m_X_2, out m_Y_2 );
						rightSpellLocation = "Edwin the Pope rests in the Lodoria Cemetery<br>" + rightSpellGrave;
					}
				}
				else if ( page == 8 )
				{
					leftSpell = TouchOfLifeSpell.SpellInfo.SpellDefinition;
					haveLeftSpell = HasSpell( from, leftSpell.SpellID );
					if ( !haveLeftSpell )
					{
						var leftSpellGrave = Worlds.GetTown( 0, "the Town of Devil Guard", out m_Map_1, out m_X_1, out m_Y_1 );
						leftSpellLocation = "Xephyn the Monk rests near the Town of Devil Guard<br>" + leftSpellGrave;
					}

					rightSpell = TrialByFireSpell.SpellInfo.SpellDefinition;
					haveRightSpell = HasSpell( from, rightSpell.SpellID );
					if ( !haveRightSpell )
					{
						var rightSpellGrave = Worlds.GetTown( 0, "the Village of Fawn", out m_Map_2, out m_X_2, out m_Y_2 );
						rightSpellLocation = "Chancellor Davis rests on an island near the Village of Fawn<br>" + rightSpellGrave;
					}
				}

				if ( leftSpell != null )
				{
					AddImage(75, 80, leftSpell.IconGraphic, 1071);
					AddHtml( 129, 93, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + leftSpell.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 134, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Piety:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 196, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + leftSpell.TithingCost + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 134, 160, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Skill:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 196, 160, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + leftSpell.MinSkill + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 134, 190, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Mana:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 196, 190, 57, 20, @"<BODY><BASEFONT Color=" + color + ">" + leftSpell.ManaCost + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 78, 220, 250, 175, @"<BODY><BASEFONT Color=" + color + ">" + (leftSpellLocation ?? "") + "<br><br>Mantra: " + leftSpell.PowerWords + "<BR><BR>" + leftSpell.Description + "</BASEFONT></BODY>", (bool)false, (bool)false);
				}

				if ( rightSpell != null )
				{
					AddImage(362, 80, rightSpell.IconGraphic, 1071);
					AddHtml( 417, 93, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + rightSpell.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 422, 130, 57, 20, @"<BODY><BASEFONT Color=" + color + ">Piety:</BASEFONT></BODY>", (bool)false, (bool)false);
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
				from.SendGump( new HolyManSpellbookGump( from, m_Book, pg ) );
				from.SendGump( new Sextants.MapGump( from, m_Map_2, m_X_2, m_Y_2, null ) );
			}
			else if ( info.ButtonID >= 98000 )
			{
				int pg = info.ButtonID - 98000;
				from.SendGump( new HolyManSpellbookGump( from, m_Book, pg ) );
				from.SendGump( new Sextants.MapGump( from, m_Map_1, m_X_1, m_Y_1, null ) );
			}
			else if ( info.ButtonID < CAST_SPELL_ACTION_BASE && info.ButtonID > 0 )
			{
				from.SendSound( 0x55 );
				int page = info.ButtonID;
				if ( page < 1 ){ page = 9; }
				if ( page > 9 ){ page = 1; }
				from.SendGump( new HolyManSpellbookGump( from, m_Book, page ) );
			}
			else
			{
				int spellID = info.ButtonID - CAST_SPELL_ACTION_BASE;
				if ( HasSpell( from, spellID ) )
				{
					HolyManSpellProvider.Cast(from, spellID);

					from.SendGump( new HolyManSpellbookGump( from, m_Book, 1 ) );
				}
				else
					from.PlaySound( 0x55 );
			}
		}
	}
}