using Server.Gumps;
using Server.Network;
using Server.Misc;

namespace Server.Items
{
	public class LearnTailorBook : Item, IShowableBook
	{
		public override Catalogs DefaultCatalog{ get{ return Catalogs.Book; } }

		[Constructable]
		public LearnTailorBook( ) : base( 0x1C11 )
		{
			ItemID = RandomThings.GetRandomBookItemID();
			Hue = Utility.RandomColor(0);
			Weight = 1.0;
			Name = "Tailoring the Cloth";
		}

		public class LearnTailorGump : Gump
		{
			private Item m_Book;
			private int m_Page;
			private Mobile m_Mobile;

			public LearnTailorGump( Mobile from, Item book, int page ): base( 50, 50 )
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
					int amt = 12;
					int itm = 0;

					int x = 75;
					int y = 75;
					CraftResource res = CraftResource.Fabric;

					int modX = 289;
					int modY = 36;

					while ( amt > 0 )
					{
						amt--; itm++;

						AddItem( x, y, 5987, CraftResources.GetHue( res ) );
						AddHtml( x+44, y, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + CraftResources.GetName( res ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

						y += modY;

						if ( itm == 9 ){ y = 75; x += modX; }

						res = (CraftResource)( (int)res + 1 );
					}
				}
				else
				{
					AddItem(368, 137, 6812);
					AddItem(378, 167, 21562);
					AddItem(84, 82, 19585, 0xB61);
					AddItem(368, 106, 3577);
					AddItem(76, 122, 5987);
					AddItem(370, 73, 3576);
					AddItem(76, 164, 3999);
					AddItem(354, 292, 4191);
					AddItem(360, 198, 4117);
					AddItem(367, 270, 4192);

					string tailoring = "Tailoring is the skill of taking cloth and making clothing. Using a sewing kit, you can use cloth and turn it into items like robes, pants, or hat. The better the cloth, the better the clothing you can create. The types of cloth one can find can be viewed on the next page. You can also use scissors on existing clothing, and if your skill is high enough, it will be turned into workable cloth. You can sheer sheep with a bladed weapon to get wool by double clicking the weapon and then selecting the sheep.";
					string cloth = "You can also find gardens that grow cotton and flax. You can gather these by using them or walking over them. The plants will be gathered in your pack. Once gathered, you can use them on a spinning wheel to make string. Once you have the string, you can use that on a loom to make cloth by using the string and then selecting the loom. You can also cut the cloth down into bandages if you need them.";

					AddHtml( 122, 80, 200, 310, @"<BODY><BASEFONT Color=" + color + ">" + tailoring + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 415, 80, 200, 310, @"<BODY><BASEFONT Color=" + color + ">" + cloth + "</BASEFONT></BODY>", (bool)false, (bool)false);
				}
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				if ( info.ButtonID > 0 )
				{
					m_Page = info.ButtonID;
					if ( m_Page >= 900 )
						m_Page = 2;
					else if ( m_Page > 2 )
						m_Page = 1;
					else
						m_Page = info.ButtonID;

					m_Mobile.SendGump( new LearnTailorGump( m_Mobile, m_Book, m_Page ) );
				}
				else
					m_Mobile.SendSound( 0x55 );
			}
		}

		public void ShowBookGump( Mobile from )
		{
			from.CloseGump( typeof( LearnTailorGump ) );
			from.SendGump( new LearnTailorGump( from, this, 1 ) );
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

		public LearnTailorBook(Serial serial) : base(serial)
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
				Name = "Tailoring the Cloth";
			}
		}
	}
}