using Server.Network;
using Server.Gumps;
using Server.Mobiles;
using Server.Utilities;

namespace Server.Items
{
	public class CarpetBuild : Item
	{
		[Constructable]
		public CarpetBuild() : base( 0x1A97 )
		{
			Weight = 2.0;
			Hue = 0x95B;
			Name = "The Carpet of Aladdin";

			if ( Weight > 1.0 )
			{
				Weight = 1.0;
				HaveGold = 0;
				HaveCloth = 0;
			}
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);

			int needGold = 20000 - HaveGold;
				if ( needGold < 0 ){ needGold = 0; }
			int needCloth = 10000 - HaveCloth;
				if ( needCloth < 0 ){ needCloth = 0; }

			int carpetDone = needGold + needCloth;

			if ( carpetDone > 0 )
			{
				list.Add( 1070722, "Drop The Items Needed On This Book" );
				list.Add( 1049644, "Need " + needGold.ToString() + " Gold Coins, " + needCloth.ToString() + " Cloth" );
			}
			else
			{
				list.Add( 1070722, "Read the Book to Conjure" );
			}
        }

		public override void OnDoubleClick( Mobile from )
		{
			int needGold = 20000 - HaveGold;
			int needCloth = 10000 - HaveCloth;

			int carpetDone = needGold + needCloth;

			if ( carpetDone > 0 )
			{
				from.SendMessage( "You need to gather more items before you can conjure this!" );
				from.SendSound( 0x4A );
				from.CloseGump( typeof( RugGump ) );
				from.SendGump( new RugGump( from, HaveGold, HaveCloth ) );
			}
			else
			{
				int builder = 0;

				foreach ( Mobile m in this.GetMobilesInRange( 20 ) )
				{
					if ( m is Mage || m is Witches || m is Necromancer || m is MageGuildmaster || m is NecromancerGuildmaster )
						++builder;
				}

				if ( builder < 1 )
				{
					from.SendMessage( "You need to be near a wizard to conjure that!" );
					from.SendSound( 0x4A );
					from.CloseGump( typeof( RugGump ) );
					from.SendGump( new RugGump( from, HaveGold, HaveCloth ) );
				}
				else
				{
					from.SendMessage( "You read the book and it transforms into a magic carpet." );
					from.PlaySound( 0x243 );
					from.AddToBackpack ( new Multis.MagicCarpetADeed() );
					this.Delete();
				}
			}
		}

		private class RugGump : Gump
		{
			public RugGump( Mobile from, int gold, int cloth ) : base( 50, 50 )
			{
				from.SendSound( 0x4A ); 
				string color = "#9ab9cb";

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				if ( gold > 20000 ){ gold = 20000; }
				if ( cloth > 10000 ){ cloth = 10000; }

				AddPage(0);

				AddImage(0, 0, 7008, Server.Misc.PlayerSettings.GetGumpHue( from ));
				AddButton(400, 11, 4017, 4017, 0, GumpButtonType.Reply, 0);
				AddHtml( 12, 14, 357, 20, @"<BODY><BASEFONT Color=" + color + ">THE CARPET OF ALADDIN</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 13, 43, 415, 165, @"<BODY><BASEFONT Color=" + color + ">This book tells the tale of Aladdin's magic carpet. Within these pages, you have learned the secrets to have your own magic carpet conjured. Centuries ago, legends tell of Aladdin gliding over the seas on his magic carpet. If you want to have such an item created for yourself, you will need to collect 10,000 cloth and then read the magic words from this book while near a local wizard as you will surely need their help. For their services, the wizard will require 20,000 gold. To collect these items, place them on this book as you acquire them. Once you collect the needed cloth and gold, find a local wizard and read the book while with them. The book should then transform into a magic carpet.<br><br>Magic carpets work much like sailing vessels, except they can be launched from any shore and they do not have a lower deck to rest. Perhaps the book, Skulls and Shackles, will help you as the navigation and maintenance is the same for both sailing vessels and magic carpets. Unlike ships with planks to board, magic carpets have extended cloth that you can board and disembark from. You will also have a magic key for which to protect your carpet from unwanted visitors. They do not have a hold like a ship, but instead a magic bag that you can store items in. They do not have a tillerman, but instead a magic lamp to follow your commands. If you ever want to change the decorative design of your carpet, simply give it to a tailor and they will change it to one of nine designs. You can simply use the carpet to see the preview of what the design looks like after it is altered. If you named your carpet prior to alteration, you will have to rename it if you gave it a name previously. If you want to change the color of your carpet, you can simply dye it a different color.</BASEFONT></BODY>", (bool)false, (bool)true);
				AddHtml( 15, 221, 200, 20, @"<BODY><BASEFONT Color=" + color + ">Gold: " + gold + "/20000</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 227, 221, 200, 20, @"<BODY><BASEFONT Color=" + color + ">Cloth: " + cloth + "/10000</BASEFONT></BODY>", (bool)false, (bool)false);
			}

			public override void OnResponse(NetState state, RelayInfo info)
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x4A ); 
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{          		
			int needGold = 20000 - HaveGold;
			int needCloth = 10000 - HaveCloth;

			if ( from != null )
			{
				if ( dropped is Gold && needGold > 0 )
				{
					var iAmount = ItemUtilities.ConsumeClamped(dropped, needGold);
					HaveGold += iAmount;
					from.SendMessage( "You added " + iAmount.ToString() + " gold." );
					this.InvalidateProperties();

					return dropped.Deleted;
				}
				else if ( dropped is BaseFabric && needCloth > 0 )
				{
					var iAmount = ItemUtilities.ConsumeClamped(dropped, needCloth);
					HaveCloth += iAmount;
					from.SendMessage( "You added " + iAmount.ToString() + " cloth." );
					this.InvalidateProperties();

					return dropped.Deleted;
				}
			}

			return false;
		}

		public CarpetBuild( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version

			writer.Write( HaveGold );
			writer.Write( HaveCloth );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			HaveGold = reader.ReadInt();
			HaveCloth = reader.ReadInt();
		}

		public int HaveGold;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveGold { get{ return HaveGold; } set{ HaveGold = value; } }

		public int HaveCloth;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveCloth { get{ return HaveCloth; } set{ HaveCloth = value; } }
	}
}