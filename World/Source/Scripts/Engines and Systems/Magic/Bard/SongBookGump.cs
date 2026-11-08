using Server.Items; 
using Server.Network; 
using Server.Spells.Song; 
 

namespace Server.Gumps 
{ 
	public class SongBookGump : Gump
	{
		private const int CAST_SPELL_ACTION_BASE = 10000;

		private readonly SongBook m_Book;

		public bool HasSpell( Mobile from, int spellID )
		{
			if ( m_Book.RootParentEntity == from )
				return (m_Book.HasSpell(spellID));
			else
				return false;
		}
       
		public SongBookGump( Mobile from, SongBook book, int page ) : base( 100, 100 ) 
		{ 
			m_Book = book;
			string color = "#d6c382";
			from.PlaySound( 0x55 );

			this.Closable=true;
			this.Disposable=false;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);
			AddImage(0, 0, 7005, book.Hue-1);
			AddImage(0, 0, 7006);
			AddImage(0, 0, 7024, 2736);
			AddImage(125, 130, 7047);
			AddImage(436, 130, 7047);

			int PriorPage = page - 1;
				if ( PriorPage < 1 ){ PriorPage = 9; }
			int NextPage = page + 1;
				if ( NextPage > 9 ){ NextPage = 1; }

			AddButton(72, 45, 4014, 4014, PriorPage, GumpButtonType.Reply, 0);
			AddButton(590, 48, 4005, 4005, NextPage, GumpButtonType.Reply, 0);

			if ( page == 1 )
			{
				AddHtml( 107, 46, 186, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>BARDIC SONGS</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);

				int x = 95;
				int y = 100;
				int c = 0;
				foreach (var definition in BardSongProvider.SpellDefinitions)
				{
					if ( !HasSpell( from, definition.SpellID ) ) continue;

					AddButton(x-5, y-5, 7048, 7048, CAST_SPELL_ACTION_BASE + definition.SpellID, GumpButtonType.Reply, 0);
					AddHtml( x+25, y, 148, 20, @"<BODY><BASEFONT Color=" + color + ">" + definition.Name.String + "</BASEFONT></BODY>", (bool)false, (bool)false);
					y += 38;
					if ( ++c == 8 ){ x = 415; y = 100; }
				}
			}
			else
			{
				AddHtml( 107, 46, 186, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>BARDIC SONGS</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 398, 48, 186, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>BARDIC SONGS</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);

				var firstDefinitionIndex = (page - 2) * 2;
				var definitionIndex = 0;
				foreach (var definition in BardSongProvider.SpellDefinitions)
				{
					if ( definitionIndex < firstDefinitionIndex )
					{
						definitionIndex++;
						continue;
					}

					if ( definitionIndex >= firstDefinitionIndex + 2 ) break;

					var offset = (definitionIndex - firstDefinitionIndex) * 295;
					var command = definition.Name.String.Replace( " ", "" ).Replace( "'", "" );

					AddImage( 75 + offset, 80, definition.IconGraphic );
					AddHtml( 134 + offset, 90, 177, 20, string.Format( @"<BODY><BASEFONT Color={0}>{1}</BASEFONT></BODY>", color, definition.Name.String ), false, false );
					AddHtml( 135 + offset, 125, 56, 20, string.Format( @"<BODY><BASEFONT Color={0}>Skill:</BASEFONT></BODY>", color ), false, false );
					AddHtml( 199 + offset, 125, 56, 20, string.Format( @"<BODY><BASEFONT Color={0}>{1}</BASEFONT></BODY>", color, definition.MinSkill ), false, false );
					AddHtml( 134 + offset, 155, 56, 20, string.Format( @"<BODY><BASEFONT Color={0}>Mana:</BASEFONT></BODY>", color ), false, false );
					AddHtml( 198 + offset, 155, 56, 20, string.Format( @"<BODY><BASEFONT Color={0}>{1}</BASEFONT></BODY>", color, definition.ManaCost ), false, false );
					AddHtml( 95 + offset, 215, 189, 20, string.Format( @"<BODY><BASEFONT Color={0}>[{1}</BASEFONT></BODY>", color, command ), false, false );
					AddHtml( 76 + offset, 250, 247, 143, string.Format( @"<BODY><BASEFONT Color={0}>{1}</BASEFONT></BODY>", color, definition.Description.String ), false, false );
					AddHtml( 77 + offset, 190, 189, 20, string.Format( @"<BODY><BASEFONT Color={0}>Keyboard Command:</BASEFONT></BODY>", color ), false, false );

					definitionIndex++;
				}
			}
		}
       
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile; 

			if ( info.ButtonID < CAST_SPELL_ACTION_BASE && info.ButtonID > 0 )
			{
				from.SendSound( 0x55 );
				int page = info.ButtonID;
				if ( page < 1 ){ page = 9; }
				if ( page > 9 ){ page = 1; }
				from.SendGump( new SongBookGump( from, m_Book, page ) );
			}
			else
			{
				int spellID = info.ButtonID - CAST_SPELL_ACTION_BASE;
				if ( HasSpell( from, spellID ) )
				{
					BardSongProvider.Cast(from, spellID);

					from.SendGump( new SongBookGump( from, m_Book, 1 ) );
				}
				else
					from.PlaySound( 0x55 );
			}
		}
	} 
}
