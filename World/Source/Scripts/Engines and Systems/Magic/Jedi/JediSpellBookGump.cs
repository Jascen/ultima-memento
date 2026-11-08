using System.Globalization;
using Server.Items; 
using Server.Misc; 
using Server.Network; 
using Server.Spells; 
using Server.Spells.Jedi;

namespace Server.Gumps 
{ 
	public class JediSpellbookGump : Gump 
	{
		private JediSpellbook m_Book; 
		private Map placer;
		private int xc;
		private int yc;

		public bool HasSpell(Mobile from, int spellID)
		{
			if ( m_Book.RootParentEntity == from )
				return (m_Book.HasSpell(spellID));
			else
				return false;
		}

		private const int CAST_SPELL_ACTION_BASE = 100;

		public struct JediHolocron
		{
			public readonly string Town;
			public readonly string World;
			public readonly string TrueName;

			public JediHolocron( string town, string world, string trueName )
			{
				Town = town;
				World = world;
				TrueName = trueName;
			}
		}

		public static JediHolocron GetHolocron( int spellID )
		{
			if ( spellID == ForceGrip.SpellInfo.SpellDefinition.SpellID ) return new JediHolocron( "the City of Britain", "Land of Sosaria", "Jacen Sollo" );
			if ( spellID == MindsEye.SpellInfo.SpellDefinition.SpellID ) return new JediHolocron( "the Town of Moon", "Land of Sosaria", "Kiadi Mundia" );
			if ( spellID == Mirage.SpellInfo.SpellDefinition.SpellID ) return new JediHolocron( "the Village of Grey", "Land of Sosaria", "Kip Fisto" );
			if ( spellID == ThrowSabre.SpellInfo.SpellDefinition.SpellID ) return new JediHolocron( "the City of Montor", "Land of Sosaria", "Marra Jade" );
			if ( spellID == Celerity.SpellInfo.SpellDefinition.SpellID ) return new JediHolocron( "the Town of Renika", "the Island of Umber Veil", "Numi Sunrider" );
			if ( spellID == PsychicAura.SpellInfo.SpellDefinition.SpellID ) return new JediHolocron( "the City of Elidor", "Land of Lodoria", "Plo Kune" );
			if ( spellID == Deflection.SpellInfo.SpellDefinition.SpellID ) return new JediHolocron( "the Village of Springvale", "Land of Lodoria", "Kyle Katran" );
			if ( spellID == SoothingTouch.SpellInfo.SpellDefinition.SpellID ) return new JediHolocron( "the Village of Islegem", "Land of Lodoria", "Kyp Duron" );
			if ( spellID == StasisField.SpellInfo.SpellDefinition.SpellID ) return new JediHolocron( "Greensky Village", "Land of Lodoria", "Ganer Rhysode" );
			if ( spellID == Replicate.SpellInfo.SpellDefinition.SpellID ) return new JediHolocron( "the City of Kuldara", "the Bottle World of Kuldar", "Coran Horn" );

			return default( JediHolocron );
		}

		public JediSpellbookGump( Mobile from, JediSpellbook book, int page ) : base( 25, 25 ) 
		{
			m_Book = book; 

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);
			AddImage(0, 0, 30521);

			if ( page == 1 )
			{
				AddImage(517, 40, 11435);

				AddHtml( 50, 40, 437, 20, @"<BODY><BASEFONT Color=#308EB3>DATACRON OF JEDI WISDOM</BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(443, 40, 3610, 3610, 2, GumpButtonType.Reply, 0);

				AddItem(43, 81, 16314);
				AddItem(60, 131, 9698);
				AddItem(51, 181, 0x3003, 0xB96);
				AddHtml( 110, 85, 70, 20, @"<BODY><BASEFONT Color=#308EB3>Skill:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 110, 135, 70, 20, @"<BODY><BASEFONT Color=#308EB3>Mana:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 110, 185, 70, 20, @"<BODY><BASEFONT Color=#308EB3>Crystals:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 210, 85, 55, 20, @"<BODY><BASEFONT Color=#00FF06>" + Server.Spells.Jedi.JediSpell.GetJediSkillMax( from)  + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 210, 135, 55, 20, @"<BODY><BASEFONT Color=#00FF06>" + from.ManaMax + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 210, 185, 55, 20, @"<BODY><BASEFONT Color=#00FF06>" + Server.Spells.Jedi.JediSpell.GetCrystals( from ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(305, 122, 0x2162);
				AddHtml( 360, 135, 70, 20, @"<BODY><BASEFONT Color=#308EB3>Power:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 460, 135, 55, 20, @"<BODY><BASEFONT Color=#00FF06>" + ((int)(Server.Spells.Jedi.JediSpell.GetJediDamage(from))) + "</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( Server.Misc.GetPlayerInfo.isJedi ( from, true ) ){ AddHtml( 310, 85, 183, 20, @"<BODY><BASEFONT Color=#00FF06>You Are One With The Jedi</BASEFONT></BODY>", (bool)false, (bool)false); }
				else { AddHtml( 310, 85, 183, 20, @"<BODY><BASEFONT Color=#308EB3>You Are Not A Jedi!</BASEFONT></BODY>", (bool)false, (bool)false); }

				AddHtml( 60, 236, 435, 60, @"<BODY><BASEFONT Color=#308EB3>Quick bars for these powers can be set up in the Magic Toolbars section of [toolbars</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 350, 316, 246, 20, @"<BODY><BASEFONT Color=#308EB3>Construct Laser Sword</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(305, 316, 4011, 4011, 6, GumpButtonType.Reply, 0);

				int index = 0;
				foreach ( SpellDefinition definition in JediSpellProvider.SpellDefinitions )
				{
					int x = index < 5 ? 0 : 370;
					int y = 380 + ( index % 5 ) * 50;
					index++;

					string wordColor = "#308EB3";
					if ( HasSpell( from, definition.SpellID ) ){ wordColor = "#00FF06"; AddButton(100 + x, y, 4005, 4005, CAST_SPELL_ACTION_BASE + definition.SpellID, GumpButtonType.Reply, 0); }
					else { AddImage(100 + x, y, 4005 ); }
					AddItem(60 + x, y - 3, 21564, 0xB3E);
					AddHtml( 145 + x, y, 154, 20, @"<BODY><BASEFONT Color=" + wordColor + ">" + definition.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(305 + x, y, 4011, 4011, definition.SpellID, GumpButtonType.Reply, 0);
				}
			}
			else if ( page == 2 )
			{
				AddHtml( 50, 40, 437, 20, @"<BODY><BASEFONT Color=#308EB3>DATACRON OF JEDI WISDOM</BASEFONT></BODY>", (bool)false, (bool)false);

				string lowreg = " There is a magical property on this world with lower reagent qualities. These are helpful for a Jedi in regards to syrcarak crystals, but only half as such as wizards are granted.";
					if ( MyServerSettings.LowerReg() < 1 )
						lowreg = "";

				AddHtml( 57, 77, 652, 528, @"<BODY><BASEFONT Color=#00FF06>You can hear the voice of Zoda the Jedi Master from within your own mind. He tells you that if you are worthy of the path, you can one day become a powerful Jedi yourself. A Jedi pursues their goals with their psychic abilities and their sword. Any other type of weapon is virtually useless to a Jedi. <br><br>He continues to explain that the knowledge this datacron contained was given by ten members of the Jedi order. You will need to find their resting places to obtain each of the Jedi powers once wielded by the Jedi. Each power is described within the datacron, and the location of where an aspiring Jedi may find the graves. Find the grave, speak the true name of the one resting there, and gain the wisdom of the Jedi.<br><br>To be one with the Jedi, you need to have positive karma. The greater your karma, the more light you possess and thus your powers will have a potential of greater effect. To use a Jedi power, one would need an equal amount of skill in both psychology and sword fighting. Tactics can help a Jedi not only fight better, but tactics (along with sword fighting) can also increase the effectiveness of Jedi powers. There are certain criteria to be a Jedi. They must always wield a sword and wear clothing or trinkets named after our order. These could be either trinkets, robes, hoods, cowls, or shields. Only one piece of clothing or trinket need be worn at a time. Without a datacron of their own, a Jedi cannot be. Since datacrons cannot be created on this world, you will have to embrace Zoda's. Keep it with you at all times or you will not be one with the Jedi. If you find clothing, or trinkets that you wish to keep, place them onto the datacron and it will transform the item into something appropriate for a Jedi.<br><br>A Jedi cannot rely solely on the power of the mind, as we need the energy from karan crystals to power our datacrons and give us the force we need to use our power. We could not find karan crystals on this world, as they are normally from the Jedi home world. The ship I came here on did have a quadrit catalizer, however, that can cystalize the molecular structure of gold into karan crystals. I left this device with Jacen in Britain, so he may be able to help you turn gold coins into these crystals. If you find this datacron many years beyond his passing, perhaps he had a child that also became a priest and may still be in Britain. It is up to you to seek them out if this is true, and see if you can give them gold coins they will turn into these crystals for you. We also discovered that demonic creatures have an element of karan crystals within their bones. This was by accident, when I slain a daemon terrorizing a family. It was a discovery that helped the Jedi to exist here, and you must know this if you pursue this life. If the device no longer exists, then you must vanquish the demons of the land and use the shards from their bones to undo the evil they wrought." + lowreg + "<br><br>If you want to have the grand title of 'Jedi', then make sure your skill title is set to that of psychology and that you also follow the Jedi ways. If you also pursue the passage of becoming a grandmaster in this world's noble acts of knightship, then you can be a 'Jedi Knight'.<br><br>Go forth, and bring peace to this world. Remain in the light and beware the darkness!<br><br>You can use these powers by a typed command, which allows you to make macros for using these if you want. Type [Spells_Jedi to see each of these commands.<br><br></BASEFONT></BODY>", (bool)false, (bool)true);
				AddButton(691, 38, 4017, 4017, 1, GumpButtonType.Reply, 0);
				AddItem(634, 34, 0x543D);
			}
			else if ( page == 6 )
			{
				AddHtml( 50, 40, 437, 20, @"<BODY><BASEFONT Color=#308EB3>DATACRON OF JEDI WISDOM</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(691, 38, 4017, 4017, 1, GumpButtonType.Reply, 0);
				AddImage(52, 78, 11435);

				AddItem(273, 80, 7153, 0x7A9);

				if ( m_Book.steel > 0 ){ AddHtml( 325, 81, 190, 20, @"<BODY><BASEFONT Color=#00FF06>You Have Durasteel</BASEFONT></BODY>", (bool)false, (bool)false); }
				else { AddHtml( 325, 81, 190, 20, @"<BODY><BASEFONT Color=#308EB3>You Need Durasteel!</BASEFONT></BODY>", (bool)false, (bool)false); }

				int jewel = 0xB38;
				if ( m_Book.gem > 0 ){ jewel = m_Book.gem; AddHtml( 325, 126, 190, 20, @"<BODY><BASEFONT Color=#00FF06>You Have A Gem</BASEFONT></BODY>", (bool)false, (bool)false); }
				else { AddHtml( 325, 126, 190, 20, @"<BODY><BASEFONT Color=#308EB3>You Need A Gem!</BASEFONT></BODY>", (bool)false, (bool)false); }
				AddItem(272, 131, 11421, jewel);

				AddHtml( 278, 221, 436, 382, @"<BODY><BASEFONT Color=#00FF06>When a Jedi has reached the level of grandmaster in psychology, sword fighting, and tactics they can construct their own laser sword. In order to do this, they will need to find an ordinary gem of their choice. These are gems like rubies, emeralds, or sapphires. The gem will set the color for the blade of the laser sword after it is constructed and emit the light needed for the blade, where this gem is put into this datacron. They will also need a metal durasteel ingot, which is not a metal born of this world. They can replace gems at any time before construction if they want to choose a different color for the blade. They will also need 10,000 gold in their pack for additional materials. The construction will require a Jedi to mediate deeply to power the creation of the blade, where 15,000 karma will be required. They will also need 15,000 points of fame. Both elements of the Jedi will be set to zero after the creation of the laser sword, where the Jedi will be greatly weakened in their powers until their wisdom grows strong again.<br><br>The laser sword will begin as something ordinary, but as long as it is used it will grow in power as victories are achieved over the many fearsome foes of the lands. This sword will never need to be repaired. If a Jedi meets an untimely end, they will have it in their possession when they return to the living. Certain traps that affect equipped items will have no adverse effects on this. Creatures, that attempt to ruin items, will fail in the attempt. If a Jedi is careless with the sword, and leave it lying about, then fate will speak for what may happen to it. This sword will gain levels as it achieves victory over a Jedi’s adversaries. When the sword gains a level, a Jedi can single click on it and select 'Enchant' to give the sword more power. Be careful adding powers, as one cannot change any attributes once they select them. A Jedi can use regular dye tubs on these, making them any color they choose if the gem they selected does not meet their expectations. They must also be in Zoda's tomb to construct this laser sword, where they will have their choice of a regular or a double bladed weapon.</BASEFONT></BODY>", (bool)false, (bool)true);

				AddItem(132, 416, 16314);
				AddItem(136, 525, 11499);

				Region reg = Region.Find( from.Location, from.Map );

				if ( !Server.Misc.GetPlayerInfo.isJedi ( from, false ) || from.Skills[SkillName.Psychology].Value < 100 || from.Skills[SkillName.Tactics].Value < 100 || from.Skills[SkillName.Swords].Value < 100 || from.Fame < 15000 || from.Karma < 15000 )
				{ 
					AddHtml( 281, 182, 430, 20, @"<BODY><BASEFONT Color=#308EB3>You lack the attributes as a Jedi to construct a laser sword.</BASEFONT></BODY>", (bool)false, (bool)false);
				}
				else if ( !( reg.IsPartOf( "the Tomb of Zoda the Jedi Master" ) ) )
				{ 
					AddHtml( 281, 182, 430, 20, @"<BODY><BASEFONT Color=#308EB3>You need to be at Zoda's tomb to construct a laser sword.</BASEFONT></BODY>", (bool)false, (bool)false);
				}
				else if ( m_Book.steel < 1 )
				{ 
					AddHtml( 281, 182, 430, 20, @"<BODY><BASEFONT Color=#308EB3>You need durasteel to construct a laser sword.</BASEFONT></BODY>", (bool)false, (bool)false);
				}
				else if ( m_Book.gem < 1 )
				{ 
					AddHtml( 281, 182, 430, 20, @"<BODY><BASEFONT Color=#308EB3>You need an ordinary gem to construct a laser sword.</BASEFONT></BODY>", (bool)false, (bool)false);
				}
				else if ( GetWealth( from ) < 10000 )
				{ 
					AddHtml( 281, 182, 430, 20, @"<BODY><BASEFONT Color=#308EB3>You need 10,000 gold to construct a laser sword.</BASEFONT></BODY>", (bool)false, (bool)false);
				}
				else
				{
					AddButton(97, 426, 4005, 4005, 7, GumpButtonType.Reply, 0);
					AddButton(97, 538, 4005, 4005, 8, GumpButtonType.Reply, 0);
				}
			}
			else if ( JediSpellProvider.GetDefinition( page ) != null )
			{
				SpellDefinition definition = JediSpellProvider.GetDefinition( page );
				JediHolocron holocron = GetHolocron( page );

				AddHtml( 50, 40, 437, 20, @"<BODY><BASEFONT Color=#308EB3>DATACRON OF JEDI WISDOM</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(691, 38, 4017, 4017, 1, GumpButtonType.Reply, 0);

				AddImage(510, 43, 11433);
				AddItem(307, 82, 21564, 0xB3E);
				AddHtml( 114, 83, 154, 20, @"<BODY><BASEFONT Color=#00FF06>" + definition.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 57, 286, 652, 186, @"<BODY><BASEFONT Color=#00FF06>" + definition.Description + "</BASEFONT></BODY>", (bool)false, (bool)false);

				int prev = m_Book.page - 1; if ( prev < JediSpellProvider.FirstSpellId ){ prev = JediSpellProvider.LastSpellId; }
				int next = m_Book.page + 1; if ( next > JediSpellProvider.LastSpellId ){ next = JediSpellProvider.FirstSpellId; }

				AddImage(56, 73, definition.IconGraphic, 2825);

				AddButton(45, 586, 4014, 4014, prev, GumpButtonType.Reply, 0);
				AddButton(691, 586, 4005, 4005, next, GumpButtonType.Reply, 0);

				AddItem(46, 132, 16314);
					AddHtml( 115, 135, 70, 20, @"<BODY><BASEFONT Color=#308EB3>Skill:</BASEFONT></BODY>", (bool)false, (bool)false);
						AddHtml( 210, 135, 54, 20, @"<BODY><BASEFONT Color=#00FF06>" + definition.MinSkill + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(63, 182, 9698);
					AddHtml( 115, 185, 70, 20, @"<BODY><BASEFONT Color=#308EB3>Mana:</BASEFONT></BODY>", (bool)false, (bool)false);
						AddHtml( 210, 185, 54, 20, @"<BODY><BASEFONT Color=#00FF06>" + definition.ManaCost + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(54, 232, 0x3003, 0xB96);
					AddHtml( 115, 235, 70, 20, @"<BODY><BASEFONT Color=#308EB3>Crystals:</BASEFONT></BODY>", (bool)false, (bool)false);
						AddHtml( 210, 235, 54, 20, @"<BODY><BASEFONT Color=#00FF06>" + definition.TithingCost + "</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( HasSpell( from, page ) ){ AddHtml( 352, 82, 154, 20, @"<BODY><BASEFONT Color=#00FF06>Learned</BASEFONT></BODY>", (bool)false, (bool)false); }
				else
				{
					AddHtml( 352, 82, 154, 20, @"<BODY><BASEFONT Color=#308EB3>Not Learned</BASEFONT></BODY>", (bool)false, (bool)false);

					string locale = Worlds.GetTown( 0, holocron.Town, out placer, out xc, out yc );

					string hidden = "You can find the Jedi Holocron of " + definition.Name + " at " + holocron.Town + " in the " + holocron.World + ".";
					hidden = hidden + " It is safely in the grave of a Jedi Master known as " + definition.PowerWords + ", and it can be revealed by speaking their true name of '" + holocron.TrueName + "'.<br><br>";
					hidden = hidden + locale;

					AddHtml( 57, 480, 664, 100, @"<BODY><BASEFONT Color=#308EB3>" + hidden + "</BASEFONT></BODY>", (bool)false, (bool)false);

					if ( Sextants.HasSextant( from ) && xc > 0 && !HasSpell( from, page ) )
						AddButton(104, 585, 10461, 10461, 98000+page, GumpButtonType.Reply, 0);
				}

				if ( Server.Misc.GetPlayerInfo.isJedi ( from, true ) )
				{
					if ( Server.Spells.Jedi.JediSpell.GetJediSkill( from, definition.MinSkill ) )
					{
						AddHtml( 352, 226, 300, 20, @"<BODY><BASEFONT Color=#00FF06>You Are One With The Jedi</BASEFONT></BODY>", (bool)false, (bool)false);
					}
					else
					{
						AddHtml( 352, 226, 300, 20, @"<BODY><BASEFONT Color=#308EB3>Your Jedi Powers Are Not Strong Enough!</BASEFONT></BODY>", (bool)false, (bool)false);
					}
				}
				else { AddHtml( 352, 226, 300, 20, @"<BODY><BASEFONT Color=#308EB3>You Are Not A Jedi!</BASEFONT></BODY>", (bool)false, (bool)false); }
			}
		}

		public static int GetWealth( Mobile from )
		{
			int wealth = 0;

			Container pack = from.Backpack;

			if ( pack != null )
			{
				Item[] gold = pack.FindItemsByType( typeof( Gold ) );

				for ( int i = 0; i < gold.Length; ++i )
					wealth += gold[i].Amount;
			}

			return wealth;
		}

		public static string SwordName( string item, Mobile from )
		{
			string OwnerName = from.Name;
			string sAdjective = CultureInfo.CurrentCulture.TextInfo.ToTitleCase( RandomThings.MagicItemAdj( "start", false, false, 0 ) );
			string name = item;

			if ( OwnerName.EndsWith( "s" ) )
			{
				OwnerName = OwnerName + "'";
			}
			else
			{
				OwnerName = OwnerName + "'s";
			}

			int FirstLast = 0;
			if ( Utility.RandomBool() ){ FirstLast = 1; }

			if ( FirstLast == 0 ) // FIRST COMES ADJECTIVE
			{
				name = "the " + sAdjective + " " + item + " of " + from.Name;
			}
			else // FIRST COMES OWNER
			{
				name = OwnerName + " " + sAdjective + " " + item;
			}

			return name;
		}

		public override void OnResponse( NetState state, RelayInfo info ) 
		{
			Mobile from = state.Mobile; 

			from.CloseGump( typeof( Sextants.MapGump ) );

			if ( info.ButtonID >= 98000 )
			{
				int pg = info.ButtonID - 98000;
				from.SendGump( new JediSpellbookGump( from, m_Book, pg ) );
				from.SendGump( new Sextants.MapGump( from, placer, xc, yc, null ) );
			}
			else if ( JediSpellProvider.GetDefinition( info.ButtonID ) != null )
			{
				m_Book.page = info.ButtonID;
				from.SendSound( 0x54B );
				int page = info.ButtonID;
				from.SendGump( new JediSpellbookGump( from, m_Book, page ) );
			}
			else if ( JediSpellProvider.GetDefinition( info.ButtonID - CAST_SPELL_ACTION_BASE ) != null )
			{
				m_Book.page = 1;
				JediSpellProvider.Cast( from, info.ButtonID - CAST_SPELL_ACTION_BASE );

				from.SendGump( new JediSpellbookGump( from, m_Book, 1 ) );
				from.SendSound( 0x54B );
			}
			else if ( info.ButtonID == 2 )
			{
				m_Book.page = 2;
				from.SendGump( new JediSpellbookGump( from, m_Book, 2 ) );
				from.SendSound( 0x54B );
			}
			else if ( info.ButtonID == 6 )
			{
				m_Book.page = 6;
				from.SendGump( new JediSpellbookGump( from, m_Book, 6 ) );
				from.SendSound( 0x54B );
			}
			else if ( info.ButtonID == 7 || info.ButtonID == 8 )
			{
				Container pack = from.Backpack;
				if (pack.ConsumeTotal(typeof(Gold), 10000))
				{
					Item sword = new LevelLaserSword();
						if ( info.ButtonID == 8 ){ sword.Delete(); sword = new LevelDoubleLaserSword(); }
					sword.Hue = m_Book.gem;
					from.Fame = 0;
					from.Karma = 0;
					m_Book.gem = 0;
					m_Book.steel = 0;
					sword.Name = SwordName( sword.Name, from );
					from.AddToBackpack ( sword );
					LoggingFunctions.LogCreatedJedi( from, sword.Name );
					from.SendMessage( "You can construct your own laser sword.");
					from.FixedParticles( 0x373A, 9, 32, 5030, 0xB41, 0, EffectLayer.Waist );
					from.PlaySound( 0x5C9 );
				}
			}
			else if ( m_Book.page > 1 )
			{
				m_Book.page = 1;
				from.SendGump( new JediSpellbookGump( from, m_Book, 1 ) );
				from.SendSound( 0x54B );
			}
			else
			{
				from.SendSound( 0x54D );
			}
		}
	}
}