using Server.Gumps;
using Server.Misc;
using Server.Network;

namespace Server.Items
{
	public class LearnWoodBook : Item, IShowableBook
	{
		public override Catalogs DefaultCatalog{ get{ return Catalogs.Book; } }

		[Constructable]
		public LearnWoodBook( ) : base( 0x1C11 )
		{
			ItemID = RandomThings.GetRandomBookItemID();
			Hue = Utility.RandomColor(0);
			Weight = 1.0;
			Name = "Wooden Carvings";
		}

		public class LearnWoodBookGump : Gump
		{
			private Item m_Book;
			private int m_Page;
			private Mobile m_Mobile;

			public LearnWoodBookGump( Mobile from, Item book, int page ): base( 50, 50 )
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
					int amt = 15;
					int itm = 0;

					int x = 75;
					int y = 75;
					CraftResource res = CraftResource.RegularWood;

					int modX = 289;
					int modY = 36;

					while ( amt > 0 )
					{
						amt--; itm++;

						AddItem( x, y, 7128, CraftResources.GetHue( res ) );
						AddHtml( x+44, y, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + CraftResources.GetName( res ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

						y += modY;

						if ( itm == 9 ){ y = 75; x += modX; }

						res = (CraftResource)( (int)res + 1 );
					}
				}
				else
				{
					AddItem(65, 86, 3907);
					AddItem(72, 175, 7128);
					AddItem(72, 125, 7137);
					AddItem(95, 314, 1928);
					AddItem(84, 326, 1928);
					AddItem(58, 296, 4533);
					AddItem(76, 219, 20851);
					AddItem(76, 275, 26361, 0xB61);
					AddItem(352, 300, 5042);
					AddItem(361, 245, 3904);
					AddItem(355, 85, 2473);
					AddItem(366, 174, 2903);
					AddItem(359, 134, 7034);

					string wood = "Lumberjacking allows you to us an axe to gather wood. Double click the axe, and then select a tree to begin chopping. Once you get some logs, you need to cut them into boards so you can use them for crafting. To do this, double click the logs and select a saw mill. These mills are commonly found in carpenter shops. Then you can begin crafting with a carpentry tool, or bowcrafting with bowyer tools.";

					string carve = "Bowcrafters can use boards to make arrows, bows, and crossbows. A carpenter can make shelves with woodworking tools, while they can use carpenter tools to make furniture, weapons, and armor. Molding wood into armor usually requires special oils from living trees. You can also use wood to make kindling for camping, or bark to make paper. Scribes can then take the bark and make scrolls from it.";

					AddHtml( 122, 80, 200, 300, @"<BODY><BASEFONT Color=" + color + ">" + wood + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 415, 80, 200, 300, @"<BODY><BASEFONT Color=" + color + ">" + carve + "</BASEFONT></BODY>", (bool)false, (bool)false);
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

					m_Mobile.SendGump( new LearnWoodBookGump( m_Mobile, m_Book, m_Page ) );
				}
				else
					m_Mobile.SendSound( 0x55 );
			}
		}

		public void ShowBookGump( Mobile from )
		{
			from.CloseGump( typeof( LearnWoodBookGump ) );
			from.SendGump( new LearnWoodBookGump( from, this, 1 ) );
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

		public LearnWoodBook(Serial serial) : base(serial)
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
				Name = "Wooden Carvings";
			}
		}
	}
}