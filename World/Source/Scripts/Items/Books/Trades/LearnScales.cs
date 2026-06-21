using Server.Gumps;
using Server.Network;
using Server.Misc;

namespace Server.Items
{
	public class LearnScalesBook : Item, IShowableBook
	{
		public override Catalogs DefaultCatalog{ get{ return Catalogs.Book; } }

		[Constructable]
		public LearnScalesBook( ) : base( 0x1C11 )
		{
			ItemID = RandomThings.GetRandomBookItemID();
			Hue = Utility.RandomColor(0);
			Weight = 1.0;
			Name = "Reptile Scale Crafts";
		}

		public class LearnScalesGump : Gump
		{
			private Item m_Book;
			private int m_Page;
			private Mobile m_Mobile;

			public LearnScalesGump( Mobile from, Item book, int page ): base( 50, 50 )
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
					int amt = 13;
					int itm = 0;

					int x = 75;
					int y = 75;
					CraftResource res = CraftResource.RedScales;

					int modX = 289;
					int modY = 36;

					while ( amt > 0 )
					{
						amt--; itm++;

						AddItem( x, y, 9908, CraftResources.GetHue( res ) );
						AddHtml( x+44, y, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + CraftResources.GetName( res ) + " Scales</BASEFONT></BODY>", (bool)false, (bool)false);

						y += modY;

						if ( itm == 9 ){ y = 75; x += modX; }

						res = (CraftResource)( (int)res + 1 );
					}
				}
				else
				{
					AddItem(75, 85, 26372, 0x99D);
					AddItem(361, 83, 4017);
					AddItem(73, 169, 9908, 0x99D);
					AddItem(82, 139, 3922);
					AddItem(364, 144, 4016);

					string craft = "Blacksmiths are able to use the hardened scales of reptiles, to make various types of armor and shields. These scales can vary in color and properties they enhance, for the items you can make from them. Due to the hardened nature of these scales, you would need an anvil and forge in order to heat them and hammer them into the shape required.";
					string scales = "Use a bladed item, like a dagger or knife, on a corpse by double clicking the item and then selecting the corpse. If there are reptile scales to be taken from it, they will appear in their pack. Different types of scales can be found on many creatures like lizards, dragons and dinosaurs. You can use these scales to make different types of armor and shields by using scaling tools. Some of the types of scales you can find are listed on the next page.";

					AddHtml( 122, 80, 200, 300, @"<BODY><BASEFONT Color=" + color + ">" + craft + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 415, 80, 200, 300, @"<BODY><BASEFONT Color=" + color + ">" + scales + "</BASEFONT></BODY>", (bool)false, (bool)false);
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

					m_Mobile.SendGump( new LearnScalesGump( m_Mobile, m_Book, m_Page ) );
				}
				else
					m_Mobile.SendSound( 0x55 );
			}
		}

		public void ShowBookGump( Mobile from )
		{
			from.CloseGump( typeof( LearnScalesGump ) );
			from.SendGump( new LearnScalesGump( from, this, 1 ) );
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

		public LearnScalesBook(Serial serial) : base(serial)
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
				Name = "Reptile Scale Crafts";
			}
		}
	}
}