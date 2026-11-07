using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Utilities;

namespace Server.Items
{
	public class LearnReagentsBook : Item, IShowableBook
	{
		[Constructable]
		public LearnReagentsBook( ) : base( 0x02DD )
		{
			Weight = 1.0;
			Name = "Scroll of Various Reagents";
			ItemID = Utility.RandomList( 0x02DD, 0x201A );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( "A Listing Of Reagents" );
		}

		public class LearnReagentsGump : Gump
		{
			public LearnReagentsGump( Mobile from ): base( 50, 50 )
			{
				string color = "#ddbc4b";

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 9546, Server.Misc.PlayerSettings.GetGumpHue( from ));

				AddHtml( 15, 15, 600, 20, @"<BODY><BASEFONT Color=" + color + ">INFORMATION ON VARIOUS REAGENTS</BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(867, 11, 4017, 4017, 0, GumpButtonType.Reply, 0);

				int i = 45;
				int o = 790;
				const int ROW_HEIGHT = 30;

				var tempList = new List<_Reagent>();
				
				AddHtml( 15, 60, 100, 20, @"<BODY><BASEFONT Color=" + color + ">COMMON</BASEFONT></BODY>", (bool)false, (bool)false);
				tempList.Add(new _Reagent(9839, "Black Pearl"));
				tempList.Add(new _Reagent(3963, "Bloodmoss"));
				tempList.Add(new _Reagent(3972, "Garlic"));
				tempList.Add(new _Reagent(3973, "Ginseng"));
				tempList.Add(new _Reagent(3974, "Mandrake Root"));
				tempList.Add(new _Reagent(3976, "Nightshade"));
				tempList.Add(new _Reagent(3981, "Spider Silk"));
				tempList.Add(new _Reagent(3980, "Sulfurous Ash"));
				for(var j = 0; j < tempList.Count; j++)
				{
					var reagent = tempList[j];
					var y = 90 + j * ROW_HEIGHT;
					GumpUtilities.AddRightAlignedItemToGump( this, reagent.ItemId, i - 5, y );
					AddHtml( i, y, 100, 20, string.Format("<BODY><BASEFONT Color={0}>{1}</BASEFONT></BODY>", color, reagent.Name), (bool)false, (bool)false);
				}

				tempList.Clear();
				AddHtml( 15, 360, 100, 20, @"<BODY><BASEFONT Color=" + color + ">NECROMANCY</BASEFONT></BODY>", (bool)false, (bool)false);
				tempList.Add(new _Reagent(3960, "Bat Wing"));
				tempList.Add(new _Reagent(3965, "Daemon Blood"));
				tempList.Add(new _Reagent(3983, "Grave Dust"));
				tempList.Add(new _Reagent(3982, "Nox Crystal"));
				tempList.Add(new _Reagent(3978, "Pig Iron"));
				for(var j = 0; j < tempList.Count; j++)
				{
					var reagent = tempList[j];
					var y = 390 + j * ROW_HEIGHT;
					GumpUtilities.AddRightAlignedItemToGump( this, reagent.ItemId, i - 5, y );
					AddHtml( i, y, 100, 20, string.Format("<BODY><BASEFONT Color={0}>{1}</BASEFONT></BODY>", color, reagent.Name), (bool)false, (bool)false);
				}

				tempList.Clear();
				AddHtml( 760, 60, 100, 20, @"<BODY><BASEFONT Color=" + color + ">HERBALIST</BASEFONT></BODY>", (bool)false, (bool)false);
				tempList.Add(new _Reagent(12280, "Beetle Shell"));
				tempList.Add(new _Reagent(12243, "Brimstone"));
				tempList.Add(new _Reagent(12290, "Butterfly Wings"));
				tempList.Add(new _Reagent(12250, "Eye of Toad"));
				tempList.Add(new _Reagent(12251, "Fairy Egg"));
				tempList.Add(new _Reagent(12291, "Moon Crystal"));
				tempList.Add(new _Reagent(12264, "Red Lotus"));
				tempList.Add(new _Reagent(12265, "Sea Salt"));
				tempList.Add(new _Reagent(12279, "Silver Widow"));
				tempList.Add(new _Reagent(12256, "Swamp Berries"));
				for(var j = 0; j < tempList.Count; j++)
				{
					var reagent = tempList[j];
					var y = 90 + j * ROW_HEIGHT;
					GumpUtilities.AddRightAlignedItemToGump( this, reagent.ItemId, o - 5, y );
					AddHtml( o, y, 100, 20, string.Format("<BODY><BASEFONT Color={0}>{1}</BASEFONT></BODY>", color, reagent.Name), (bool)false, (bool)false);
				}

				tempList.Clear();
				AddHtml( 760, 420, 100, 20, @"<BODY><BASEFONT Color=" + color + ">WITCHERY</BASEFONT></BODY>", (bool)false, (bool)false);
				tempList.Add(new _Reagent(25612, "Bitter Root"));
				tempList.Add(new _Reagent(25613, "Black Sand"));
				tempList.Add(new _Reagent(25614, "Blood Rose"));
				tempList.Add(new _Reagent(25615, "Dried Toad"));
				tempList.Add(new _Reagent(12249, "Gargoyle Ear"));
				tempList.Add(new _Reagent(25616, "Maggot"));
				tempList.Add(new _Reagent(25617, "Mummy Wrap"));
				tempList.Add(new _Reagent(12257, "Pixie Skull"));
				tempList.Add(new _Reagent(25618, "Violet Fungus"));
				tempList.Add(new _Reagent(25619, "Werewolf Claw"));
				tempList.Add(new _Reagent(25620, "Wolfsbane"));
				for(var j = 0; j < tempList.Count; j++)
				{
					var reagent = tempList[j];
					var y = 450 + j * ROW_HEIGHT;
					GumpUtilities.AddRightAlignedItemToGump( this, reagent.ItemId, o - 5, y );
					AddHtml( o, y, 100, 20, string.Format("<BODY><BASEFONT Color={0}>{1}</BASEFONT></BODY>", color, reagent.Name), (bool)false, (bool)false);
				}

				AddHtml( 237, 62, 434, 707, @"<BODY><BASEFONT Color=" + color + ">Mages, necromancers, witches, alchemists, and druids all use reagents of some type. This is a listing of the kinds of reagents you may find while traveling the world. This is not a complete list, as legends and rumors tell of other types that may exist. Reagents can be purchased from merchants, picked from some gardens, found on some creatures, or discovered with other treasure. One will commonly find reagents that may be used in their current trade, but if they have no such trade, then they may find any type that they can leave behind or sell.<br><br>Mages use the common reagents in the casting of spells, where necromancers use the necromancy reagents for their magic. Witches and druids use some of these reagents listed here as well.<br><br>You may find many reagents you need to identify. If you have practiced your tasting, you will able to discover what these are. Mages and necromancers simply carry the reagents with them to use as the cast spells. Alchemists use a mortar and pestle to create potions, while witches and druids use cauldrons.</BASEFONT></BODY>", (bool)false, (bool)false);
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x249 );
			}
		}

		public void ShowBookGump( Mobile from )
		{
			from.CloseGump( typeof( LearnReagentsGump ) );
			from.SendGump( new LearnReagentsGump( from ) );
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

		public LearnReagentsBook(Serial serial) : base(serial)
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

		private struct _Reagent
		{
			public readonly int ItemId;
			public readonly string Name;

			public _Reagent(int itemId, string name)
			{
				ItemId = itemId;
				Name = name;
			}
		}
	}
}