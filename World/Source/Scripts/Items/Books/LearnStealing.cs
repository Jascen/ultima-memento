using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class LearnStealingBook : Item, IShowableBook
	{
		[Constructable]
		public LearnStealingBook( ) : base( 0x02DD )
		{
			Weight = 1.0;
			Name = "The Art of Thievery";
			ItemID = Utility.RandomList( 0x02DD, 0x201A );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( "What To Steal For Better Profit" );
		}

		public class LearnStealingGump : Gump
		{
			public LearnStealingGump( Mobile from ): base( 50, 50 )
			{
				string color = "#ddbc4b";

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 9547, Server.Misc.PlayerSettings.GetGumpHue( from ));

				AddHtml( 15, 15, 398, 20, @"<BODY><BASEFONT Color=" + color + ">THE ART OF THIEVERY</BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(567, 11, 4017, 4017, 0, GumpButtonType.Reply, 0);

				AddHtml( 14, 50, 579, 388, @"<BODY><BASEFONT Color=" + color + ">For those skilled in the art of snooping and stealing, the search for ancient artifacts can be a profitable venture. Searching some of the crypts, tombs, and dungeons...you may find pedestals with ornately crafted boxes and bags that might contain something of great value. It may be a rare item, a fine piece of art, or an ancient weapon. The finely crafted bags and boxes can be kept for oneself, or they may be sold to a thief in the guild where they will gladly pay some gold for each one. These are highly collectible and they have guild contacts to resell them to royalty, art dealers, or collectors. When you come across these pedestals, and there is an item upon it, double click it to attempt to steal the item. If you are not well trained in snooping, you may set off a deadly trap. Having a good trap removing skill may avoid the effects of such traps. Once the trap is avoided, then your skill in stealing will be put to the test. If you succeed at getting the item, look inside and claim your prize.<br><br>Many people in town are looking for rare artifacts, and may pay handsomely for them.<br><br>There are also footlockers, chests, bags, and boxes that contain treasure in these places. You can attempt to steal these containers. Make sure to take what you want from them before stealing them, as you will empty the container on your escape. A thief in the guild may also pay money for these containers by selling it to them, as they are also collectible to others and they may fetch a good price. If you want to take one of these dungeon containers, use your stealing skill and then target the container. Maybe you will be quick enough.<br><br>Although you can also seek gold by picking the pockets of merchants, you can also steal gold from their coffers. You can snoop the coffers to see how much gold is in it, and then you can use your stealing skill on the coffer to try and take the gold. This may practice your skill, but it is a tricky maneuver if you are caught. You can steal coins and such from other creatures by standing next to them and attacking them, where you may automatically steal such items when giving the attack.</BASEFONT></BODY>", (bool)false, (bool)true);

				AddItem(554, 449, 4643);
				AddItem(19, 457, 13042);
				AddItem(554, 447, 3702);
				AddItem(388, 484, 5373);
				AddItem(18, 459, 3712);
				AddItem(370, 461, 7183);
				AddItem(202, 458, 13111);
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x249 );
			}
		}

		public void ShowBookGump( Mobile from )
		{
			from.CloseGump( typeof( LearnStealingGump ) );
			from.SendGump( new LearnStealingGump( from ) );
		}

		public override void OnDoubleClick( Mobile e )
		{
			if ( !e.InRange(GetWorldLocation(), 2 ) )
			{
				e.SendMessage("That is too far away to read.");
				return;
			}

			ShowBookGump( e );
			e.PlaySound( 0x249 );
			Server.Gumps.MyLibrary.readBook ( this, e );
		}

		public LearnStealingBook(Serial serial) : base(serial)
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
		}
	}
}