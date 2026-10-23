using Server.Items; 
using Server.Network; 
using Server.Spells; 
using Server.Spells.Mystic; 

namespace Server.Gumps 
{ 
	public class MysticSpellbookGump : Gump 
	{
		private const int FIRST_ABILITY_PAGE = 2;

		private static int INFO_PAGE
		{ get { return FIRST_ABILITY_PAGE + MysticSpellProvider.SpellCount; } }

		private MysticSpellbook m_Book;
		private int m_Page;
		private Map m_Map;
		private int m_X;
		private int m_Y;

		public bool HasSpell(Mobile from, int spellID)
		{
			if ( m_Book.RootParentEntity == from )
				return (m_Book.HasSpell(spellID));
			else
				return false;
		}

		private static void GetWrit( MysticSpellbook book, int abilityIndex, out Map map, out int x, out int y, out string place, out string world, out string coord )
		{
			switch ( abilityIndex )
			{
				case 0: map = book.WritMap01; x = ( book.WritX101 + book.WritX201 ) / 2; y = ( book.WritY101 + book.WritY201 ) / 2; place = book.WritPlace01; world = book.WritWorld01; coord = book.WritCoord01; break;
				case 1: map = book.WritMap02; x = ( book.WritX102 + book.WritX202 ) / 2; y = ( book.WritY102 + book.WritY202 ) / 2; place = book.WritPlace02; world = book.WritWorld02; coord = book.WritCoord02; break;
				case 2: map = book.WritMap03; x = ( book.WritX103 + book.WritX203 ) / 2; y = ( book.WritY103 + book.WritY203 ) / 2; place = book.WritPlace03; world = book.WritWorld03; coord = book.WritCoord03; break;
				case 3: map = book.WritMap04; x = ( book.WritX104 + book.WritX204 ) / 2; y = ( book.WritY104 + book.WritY204 ) / 2; place = book.WritPlace04; world = book.WritWorld04; coord = book.WritCoord04; break;
				case 4: map = book.WritMap05; x = ( book.WritX105 + book.WritX205 ) / 2; y = ( book.WritY105 + book.WritY205 ) / 2; place = book.WritPlace05; world = book.WritWorld05; coord = book.WritCoord05; break;
				case 5: map = book.WritMap06; x = ( book.WritX106 + book.WritX206 ) / 2; y = ( book.WritY106 + book.WritY206 ) / 2; place = book.WritPlace06; world = book.WritWorld06; coord = book.WritCoord06; break;
				case 6: map = book.WritMap07; x = ( book.WritX107 + book.WritX207 ) / 2; y = ( book.WritY107 + book.WritY207 ) / 2; place = book.WritPlace07; world = book.WritWorld07; coord = book.WritCoord07; break;
				case 7: map = book.WritMap08; x = ( book.WritX108 + book.WritX208 ) / 2; y = ( book.WritY108 + book.WritY208 ) / 2; place = book.WritPlace08; world = book.WritWorld08; coord = book.WritCoord08; break;
				case 8: map = book.WritMap09; x = ( book.WritX109 + book.WritX209 ) / 2; y = ( book.WritY109 + book.WritY209 ) / 2; place = book.WritPlace09; world = book.WritWorld09; coord = book.WritCoord09; break;
				default: map = book.WritMap10; x = ( book.WritX110 + book.WritX210 ) / 2; y = ( book.WritY110 + book.WritY210 ) / 2; place = book.WritPlace10; world = book.WritWorld10; coord = book.WritCoord10; break;
			}
		}

		public MysticSpellbookGump( Mobile from, MysticSpellbook book, int page ) : base( 100, 100 ) 
		{
			m_Book = book;
			m_Page = page;

			string color = "#d6c382";

			bool showScrollBar = true;

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			int PriorPage = page - 1;
				if ( PriorPage < 1 ){ PriorPage = INFO_PAGE; }
			int NextPage = page + 1;

			AddImage(0, 0, 7005);
			AddImage(0, 0, 7006);
			AddImage(0, 0, 7024, 2736);
			AddButton(72, 45, 4014, 4014, PriorPage, GumpButtonType.Reply, 0);
			AddButton(590, 48, 4005, 4005, NextPage, GumpButtonType.Reply, 0);
			AddImage(83, 110, 7044);
			AddImage(380, 110, 7044);

			if ( page == 1 )
			{
				AddHtml( 110, 47, 177, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>MONK ABILITIES</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 404, 47, 177, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>MONK ABILITIES</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);

				int nHTMLx = 130;
				int nHTMLy = 105;

				int nBUTTONx = 75;
				int nBUTTONy = 93;

				int listed = 0;
				foreach ( SpellDefinition definition in MysticSpellProvider.SpellDefinitions )
				{
					if ( !this.HasSpell( from, definition.SpellID ) )
						continue;

					AddButton(nBUTTONx, nBUTTONy, definition.IconGraphic, definition.IconGraphic, definition.SpellID, GumpButtonType.Reply, 0);
					AddImage(nBUTTONx, nBUTTONy, definition.IconGraphic, 2422);
					AddHtml( nHTMLx, nHTMLy, 177, 20, @"<BODY><BASEFONT Color=" + color + ">" + definition.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);

					nHTMLy = nHTMLy + 65;
					nBUTTONy = nBUTTONy + 65;

					// Flip to the right page
					if ( ++listed == 5 )
					{
						nHTMLx = 432; nHTMLy = 105;
						nBUTTONx = 375; nBUTTONy = 93;
					}
				}
			}
			else if ( page >= FIRST_ABILITY_PAGE && page < INFO_PAGE )
			{
				int abilityIndex = page - FIRST_ABILITY_PAGE;
				SpellDefinition definition = MysticSpellProvider.SpellDefinitions[abilityIndex];

				string writPlace, writWorld, writCoord;
				GetWrit( book, abilityIndex, out m_Map, out m_X, out m_Y, out writPlace, out writWorld, out writCoord );

				string abil_info = "<br><br>To learn the secrets of this ability, you need to find the following location and open this book there to reach into your ki for enlightenment:<br><br>";
				abil_info += "Place: " + writPlace + "<br><br>";
				abil_info += "World: " + writWorld + "<br><br>";
				abil_info += "Location: " + writCoord + "<br>";
				abil_info += "<br>Make sure you bring a blank scroll with you, so you can write what you have learned. You can then place your writings within this book.<br>";

				AddHtml( 110, 47, 177, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>MONK ABILITIES</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 404, 47, 177, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>MONK ABILITIES</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);

				string know = "<BODY><BASEFONT Color=#f58a8a>Not Learned</BASEFONT></BODY>"; if ( this.HasSpell( from, definition.SpellID ) ){ know = "<BODY><BASEFONT Color=#8af599>Learned</BASEFONT></BODY>"; }

				string ismonk = "<BODY><BASEFONT Color=#f58a8a>You are not a Monk!</BASEFONT></BODY>";
				if ( Server.Misc.GetPlayerInfo.isMonk( from ) )
					ismonk = "<BODY><BASEFONT Color=#8af599>You are on the path...</BASEFONT></BODY>";

				AddHtml( 130, 105, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + definition.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);
				if ( this.HasSpell( from, definition.SpellID ) ){ abil_info = ""; showScrollBar = false; AddButton(78, 94, definition.IconGraphic, definition.IconGraphic, definition.SpellID, GumpButtonType.Reply, 0); }
				AddImage(78, 94, definition.IconGraphic, 2422);

				AddHtml( 75, 370, 253, 20, @"" + ismonk + "", (bool)false, (bool)false);

				AddHtml( 75, 336, 253, 20, @"" + know + "", (bool)false, (bool)false);

				AddHtml( 130, 160, 88, 20, @"<BODY><BASEFONT Color=" + color + ">Skill:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 225, 160, 88, 20, @"<BODY><BASEFONT Color=" + color + ">" + definition.MinSkill + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 130, 210, 88, 20, @"<BODY><BASEFONT Color=" + color + ">Mana:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 225, 210, 88, 20, @"<BODY><BASEFONT Color=" + color + ">" + definition.ManaCost + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 130, 260, 88, 20, @"<BODY><BASEFONT Color=" + color + ">Tithe:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 225, 260, 88, 20, @"<BODY><BASEFONT Color=" + color + ">" + definition.TithingCost + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 370, 82, 247, 309, @"<BODY><BASEFONT Color=" + color + ">" + MysticSpellProvider.GetDescription( definition ) + abil_info + "</BASEFONT></BODY>", (bool)false, (bool)showScrollBar);

				if ( !this.HasSpell( from, definition.SpellID ) && Sextants.HasSextant( from ) )
					AddButton(305, 52, 10461, 10461, 800, GumpButtonType.Reply, 0);
			}
			else if ( page == INFO_PAGE )
			{
				AddHtml( 110, 47, 177, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>MONK ABILITIES</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 404, 47, 177, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>MONK RUCKSACK</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 78, 83, 247, 309, @"<BODY><BASEFONT Color=" + color + ">Monks are an order of those that hone their body and spirit. To become a monk, one must become a natural grandmaster in both focus and mediation. Monks may not use any weapons nor use any type of armor, unless the armor is light or enough to allow the channeling of spells. Their innate abilities come from their skills in fist fighting, so they may make use of pugilist gloves. To perform any of the monk abilities, one must adhere to these rules. A monk is also not considered such unless they wear a mystical monk's robe that they themselves create by using the associated monk ability. Along with that, monks do not require the donning of this robe if they are to create such a robe. That is the only exception.<br><br>When you acquired this tome, you likely looked through the pages to see the various abilities a monk may learn. In order to learn the secrets of these abilities, you need to travel to the various locations and open this book there to reach into your ki for enlightenment. Make sure you bring a blank scroll with you, so you can write what you have learned. You can then place your writings within this book and use the abilities if your skill and mana allow it. Whenever one touches these tomes, it is bound to their individual ki unless it is already bound to another. This means you will be the only one able to open the book as it belongs to you. Your writings also share this quality, so when you learn about new abilities, the parchments belong to you. Anyone else that touches these parchments will cause the paper to crumble to dust.<br><br>As previously stated, monks can create their own robes and this is something every monk must seek to do quickly. Without wearing this robe, a monk cannot perform the abilities they have learned. A monk's ability level will determine the power of the robe created. When you create the robe, it will appear in your pack and it will have a number of points you can spend to enhance it. This allows you to tailor the robe to suit your style. To begin, single click the robe and select 'Status'. A menu will appear that you can choose which attributes you want the robe to have. Be careful, as you cannot change an attribute once you select it. The points you can spend is equal to the power of the robe. Only one of your robes may exist in the world at a time, so if you create another, any previous robes will vanish to the astral plane.<br><br>Monks seek to contribute to causes other than their own, so some monks seek to help those less fortunate, while more vile monks seek to help causes that dampen the good of the land. As such they must tithe gold in order to use their abilities. You can tithe gold at any shrine you can find by single clicking the shrine and choosing the appropriate option. Abilities require varying amounts of tithing points to use. This tome will show you how many points you have available, and this information can also be seen by pressing the 'Info' button on your character's paper doll.<br><br>To demonstrate your title of 'Monk', you should set your skill title to 'Fist Fighting'. As long as you follow the rules of monkhood, your title will remain as such. If you have an apprentice ability in either magical or necromantic arts, but live the life of a monk, then your title would be that of 'Mystic'. Adventurous monks can learn skills other than those monks must know, just make sure any other skills will bit hinder the life of a monk (do not learn sword fighting, for example, as swords are useless to monks). There are no other behavioral requirements to be a monk. Some are good, and some are evil. It is all up to you on the path you take.<br><br>You can have tool bars to quickly use these abilities, and although you can manage this in the 'Help' menu, below are commands you can type to use these tool bars:<br><br>Open the first ability bar editor:<br><br>[monkspell1<br><br>Open the second ability bar editor:<br><br>[monkspell2<br><br>Open the first ability bar:<br><br>[monktool1<br><br>Open the second ability bar:<br><br>[monktool2<br><br>Close the first ability bar:<br><br>[monkclose1<br><br>Close the second ability bar:<br><br>[monkclose2<br><br><br><br>You can also type [Spells_Mystic to see the commands that use these abilities, which can help when creating macros.<br><br></BASEFONT></BODY>", (bool)false, (bool)true);
				AddHtml( 370, 83, 247, 309, @"<BODY><BASEFONT Color=" + color + ">When you have reached the level of grandmaster monk or mystic, you can travel to the " + book.PackShrine + " in Ambrosia and use your ki to call forth a monk's rucksack from the astral plane. You will need a pearl in order to do this. When you step into the shrine, open this book and if you are worthy, the rucksack will appear. Be careful, however, as you can only have one rucksack at a time and any others you may have like this will vanish back to the astral plane and any items in it. These rucksacks allow a monk to carry 100 different items with virtually no weight to anything placed within the rucksack. You will be the only one able to open this particular rucksack, and if you lose your path of a grandmaster monk or mystic, you will not be able to open the rucksack. You cannot store your monk's robe or your tome in this bag.</BASEFONT></BODY>", (bool)false, (bool)true);
			}
		}

		public override void OnResponse( NetState state, RelayInfo info ) 
		{
			Mobile from = state.Mobile; 

			if ( info.ButtonID == 800 )
			{
				from.PlaySound( 0x249 );
				from.SendGump( new MysticSpellbookGump( from, m_Book, m_Page ) );
				from.CloseGump( typeof( Sextants.MapGump ) );
				from.SendGump( new Sextants.MapGump( from, m_Map, m_X, m_Y, null ) );
			}
			else if ( info.ButtonID < 200 && info.ButtonID > 0 )
			{
				from.SendSound( 0x55 );
				int page = info.ButtonID;
				if ( page < 1 ){ page = INFO_PAGE; }
				if ( page > INFO_PAGE ){ page = 1; }
				from.SendGump( new MysticSpellbookGump( from, m_Book, page ) );
				from.CloseGump( typeof( Sextants.MapGump ) );
			}
			else if ( MysticSpellProvider.GetDefinition( info.ButtonID ) != null )
			{
				MysticSpellProvider.Cast( from, info.ButtonID );

				from.SendGump( new MysticSpellbookGump( from, m_Book, 1 ) );
				from.CloseGump( typeof( Sextants.MapGump ) );
			}
			else
			{
				from.SendSound( 0x55 );
				from.CloseGump( typeof( Sextants.MapGump ) );
			}
		}
	}
}