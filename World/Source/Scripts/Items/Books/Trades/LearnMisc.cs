using Server.Gumps;
using Server.Network;
using Server.Misc;

namespace Server.Items
{
	public class LearnMiscBook : Item, IShowableBook
	{
		public override Catalogs DefaultCatalog{ get{ return Catalogs.Book; } }

		[Constructable]
		public LearnMiscBook( ) : base( 0x1C11 )
		{
			ItemID = RandomThings.GetRandomBookItemID();
			Hue = Utility.RandomColor(0);
			Weight = 1.0;
			Name = "Skinning & Carving";
		}

		public class LearnMiscGump : Gump
		{
			private Item m_Book;
			private Mobile m_Mobile;

			public LearnMiscGump( Mobile from, Item book ): base( 50, 50 )
			{
				m_Book = book;
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

				AddHtml( 106, 44, 215, 20, @"<BODY><BASEFONT Color=" + color + ">" + m_Book.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(76, 192, 7123);
				AddItem(74, 116, 2545);
				AddItem(359, 347, 7137, 0x512);
				AddItem(76, 320, 5981, 0x981);
				AddItem(70, 279, 9908, 0x806);
				AddItem(358, 140, 4199, 0xB80);
				AddItem(73, 153, 2489);
				AddItem(356, 84, 4216, 0x69E);
				AddItem(83, 86, 3921);
				AddItem(354, 304, 7144);
				AddItem(362, 197, 8536, 0x424);
				AddItem(82, 239, 8899, 0x43F);
				AddItem(351, 243, 6585, 0x5CE);
				AddItem(68, 365, 3576);

				string p1 = "Use a bladed item, like a dagger or knife, on a corpse by double clicking the item and selecting the corpse. If there is something to be carved from it, it will appear in their pack. You may get items such as meat, feathers, bones, scales, cloth, wool, hides, skins, rocks, stones, metal, or wood. The better your forensics skill, the more you can carve from a corpse. Any corpses that can be carved will indicate that as such when you hover over them.";

				string p2 = "Animals are the best source of meat, while feathers come from bird like creatures. Bone come from many different creatures, and scales come from reptiles. Cloth is rare to find, but ghosts often have it. Wool can come from sheep and hides can come from any tough skinned creature. Rocks and metals are often found on golems and stone elementals, while wood is often fond on ents and reapers.";

				AddHtml( 122, 80, 200, 300, @"<BODY><BASEFONT Color=" + color + ">" + p1 + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 415, 80, 200, 300, @"<BODY><BASEFONT Color=" + color + ">" + p2 + "</BASEFONT></BODY>", (bool)false, (bool)false);
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				m_Mobile.SendSound( 0x55 );
			}
		}

		public void ShowBookGump( Mobile from )
		{
			from.CloseGump( typeof( LearnMiscGump ) );
			from.SendGump( new LearnMiscGump( from, this ) );
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

		public LearnMiscBook(Serial serial) : base(serial)
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
				Name = "Skinning & Carving";
			}
		}
	}
}