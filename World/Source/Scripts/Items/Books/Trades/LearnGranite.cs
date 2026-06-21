using Server.Gumps;
using Server.Misc;
using Server.Network;

namespace Server.Items
{
	public class LearnGraniteBook : Item, IShowableBook
	{
		public override Catalogs DefaultCatalog{ get{ return Catalogs.Book; } }

		[Constructable]
		public LearnGraniteBook( ) : base( 0x1C11 )
		{
			ItemID = RandomThings.GetRandomBookItemID();
			Hue = Utility.RandomColor(0);
			Weight = 1.0;
			Name = "Sand & Stone Crafts";
		}

		public class LearnGraniteGump : Gump
		{
			private Item m_Book;
			private int m_Page;
			private Mobile m_Mobile;

			public LearnGraniteGump( Mobile from, Item book, int page ): base( 50, 50 )
			{
				m_Book = book;
				m_Page = page;
				m_Mobile = from;

				string color = "#CEAA87";
				m_Mobile.SendSound( 0x55 );

				Closable = true;
				Disposable = true;
				Dragable = true;
				Resizable = false;

				AddPage(0);

				AddImage(0, 0, 7005, book.Hue);
				AddImage(0, 0, 7006);
				AddImage(0, 0, 7024, 2789);

				int prevPage = page - 1; if ( prevPage < 1 ){ prevPage = 900; }
				int nextPage = page + 1;

				AddHtml( 106, 44, 215, 20, @"<BODY><BASEFONT Color=" + color + ">" + m_Book.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(71, 41, 4014, 4014, prevPage, GumpButtonType.Reply, 0);
				AddButton(596, 41, 4005, 4005, nextPage, GumpButtonType.Reply, 0);

				if ( m_Page == 2 )
				{
					int amt = 16;
					int itm = 0;

					int x = 75;
					int y = 75;
					CraftResource res = CraftResource.Iron;

					int modX = 289;
					int modY = 36;

					while ( amt > 0 )
					{
						amt--; itm++;

						AddItem( x, y, 8536, CraftResources.GetClr( res ) );
						AddHtml( x+44, y, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + CraftResources.GetPrefix( res ) + "Granite</BASEFONT></BODY>", (bool)false, (bool)false);

						y += modY;

						if ( itm == 9 ){ y = 75; x += modX; }

						res = (CraftResource)( (int)res + 1 );
					}
				}
				else
				{
					AddItem(75, 85, 12656);
					AddItem(80, 256, 8536);
					AddItem(361, 83, 11128);
					AddItem(75, 131, 4030);
					AddItem(60, 203, 3897);
					AddItem(362, 258, 2859);
					AddItem(75, 169, 3717);
					AddItem(357, 138, 4084);
					AddItem(348, 211, 3897);
					AddItem(363, 177, 3717);

					string rock = "Mining is the skill one needs to find granite from caves and mountains. With this, carpenters can make stone furniture and statues using a mallet and chisel. You simply need to get a pick axe or a shovel, double-click it, and then target a mountain side or caven floor. You must single click the tool and set it for stone gathering. The many types of granite are listed on the next page. You need to first learn how to dig for it, and craft them, by finding books on the subjects.";
					string sand = "Mining is also the skill one needs to find sand on beaches and desert sands. With this sand, alchemists can make items such as bottles and jars. You simply need to get a pick axe or a shovel, double-click it, and then target a the sand at your feet. You must single click the tool and set it for sand gathering. Sand comes in piles and you can use a blow pipe to glass items, You need to first learn how to dig for it, and craft them, by finding books on the subjects.";

					AddHtml( 122, 80, 200, 300, @"<BODY><BASEFONT Color=" + color + ">" + rock + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 415, 80, 200, 300, @"<BODY><BASEFONT Color=" + color + ">" + sand + "</BASEFONT></BODY>", (bool)false, (bool)false);
				}
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
					m_Page = info.ButtonID;
				if ( info.ButtonID > 0 )
				{
					if ( m_Page >= 900 )
						m_Page = 2;
					else if ( m_Page > 2 )
						m_Page = 1;
					else
						m_Page = info.ButtonID;

					m_Mobile.SendGump( new LearnGraniteGump( m_Mobile, m_Book, m_Page ) );
				}
				else
					m_Mobile.SendSound( 0x55 );
			}
		}

		public void ShowBookGump( Mobile from )
		{
			from.CloseGump( typeof( LearnGraniteGump ) );
			from.SendGump( new LearnGraniteGump( from, this, 1 ) );
		}

		public override void OnDoubleClick( Mobile e )
		{
			if ( !e.InRange(GetWorldLocation(), 2 ) )
			{
				e.SendMessage("That is too far away to read.");
				return;
			}

			ShowBookGump( e );
			Server.Gumps.MyLibrary.readBook( this, e );
		}

		public LearnGraniteBook(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			if ( ItemID == 0x02DD || ItemID == 0x201A )
			{
				ItemID = RandomThings.GetRandomBookItemID();
				Hue = Utility.RandomColor(0);
				Name = "Sand & Stone Crafts";
			}
		}
	}
}