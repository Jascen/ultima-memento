using System;
using Server;
using Server.Gumps;
using Server.ContextMenus;
using System.Collections;
using System.Collections.Generic;
using Server.Network;

namespace Server.Items
{
    public class SackOfHolding : LargeSack
    {
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public Mobile SackOwner;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Sack_Owner { get{ return SackOwner; } set{ SackOwner = value; } }

		private int m_MaxWeightDefault = 100000;
		public override int DefaultMaxWeight{ get{ return m_MaxWeightDefault; } }

		public override bool DisplaysContent{ get{ return false; } }
		public override bool DisplayWeight{ get{ return false; } }

		[Constructable]
		public SackOfHolding() : base()
		{
			Weight = 1.0;
			MaxItems = 10;
			Name = "pack of holding";
			Hue = Utility.RandomColor(0);

			switch( Utility.RandomMinMax( 0, 5 ) )
			{
				case 0: Weight = 1.0;	MaxItems = 10;		ItemID = Utility.RandomList( 0x658D, 0x658E );	break;
				case 1: Weight = 1.0;	MaxItems = 10;		ItemID = Utility.RandomList( 0x658D, 0x658E );	break;
				case 2: Weight = 1.0;	MaxItems = 10;		ItemID = Utility.RandomList( 0x658D, 0x658E );	break;
				case 3: Weight = 2.0;	MaxItems = 20;		ItemID = Utility.RandomList( 0x6568, 0x6569 );	break;
				case 4: Weight = 2.0;	MaxItems = 20;		ItemID = Utility.RandomList( 0x6568, 0x6569 );	break;
				case 5: Weight = 3.0;	MaxItems = 30;		ItemID = Utility.RandomList( 0x6568, 0x6569 );	break;
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1073841, "{0}\t{1}\t{2}", TotalItems, MaxItems, TotalWeight ); // Contents: ~1_COUNT~/~2_MAXCOUNT~ items, ~3_WEIGHT~ stones
		}

		public void SetSize( int slots )
		{
			Weight = slots / 10;
			MaxItems = slots;
			ItemID = slots >= 20 ? Utility.RandomList( 0x6568, 0x6569 ) : Utility.RandomList( 0x658D, 0x658E );
		}

		public override bool OnDragDropInto( Mobile from, Item dropped, Point3D p )
        {
			if ( HoldingBagCombiner.TryCombine( from, this, dropped ) )
				return false;

			if ( dropped is Container )
			{
                from.SendMessage("You cannot store containers in this bag.");
                return false;
			}

            return base.OnDragDropInto(from, dropped, p);
        }

		public override bool OnDragDrop( Mobile from, Item dropped )
        {
			if ( HoldingBagCombiner.TryCombine( from, this, dropped ) )
				return false;

			if ( dropped is Container )
			{
                from.SendMessage("You cannot store containers in this bag.");
                return false;
			}

            return base.OnDragDrop(from, dropped);
        }

		public class BagGump : Gump
		{
			public BagGump( Mobile from, Container bag ): base( 50, 50 )
			{
				string color = "#b7765d";
				from.SendSound( 0x4A );

				int hold = bag.MaxItems;
				string sText = "This magical bag can hold almost an infinite amount of weight, but it can only hold " + hold + " separate items. Items stacked onto each other count as a single item in this regard. Other containers cannot be placed within the bag. Now that you have read this information about the bag, you can now open the bag as you normally would. To read this information in the future, single click the bag and choose the Look At menu option. Placing items in this bag can be tricky, so be aware of these issues. Items placed into the bag will only be magically affected after being placed within it. This means if your main backpack can only hold 500 stones, and this bag of holding is within your main backpack, then placing 600 stones of weight into this magical bag will not work. You would instead need to place a lesser amount of weight into the bag so the bag can magically reduce that weight. Then you can place a few more items within it. Another method of placing large piles of weight (iron ore for example) into the bag, is to set the bag on the ground and then place items in it.";

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 9547, Server.Misc.PlayerSettings.GetGumpHue( from ));
				AddHtml( 11, 11, 200, 20, @"<BODY><BASEFONT Color=" + color + ">BAG OF HOLDING</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 13, 44, 582, 473, @"<BODY><BASEFONT Color=" + color + ">" + sText + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(568, 9, 4017, 4017, 0, GumpButtonType.Reply, 0);
			}

			public override void OnResponse(NetState state, RelayInfo info)
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x4A ); 
			}
		}

		public class BagMenu : ContextMenuEntry
		{
			private SackOfHolding i_SackOfHolding;
			private Mobile m_From;

			public BagMenu( Mobile from, SackOfHolding bag ) : base( 6121, 1 )
			{
				m_From = from;
				i_SackOfHolding = bag;
			}

			public override void OnClick() 
			{
				m_From.CloseGump( typeof( BagGump ) );
				m_From.SendGump( new BagGump( m_From, i_SackOfHolding ) );
				m_From.PlaySound( 0x048 );
			}
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list ) 
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive )
				list.Add( new BagMenu( from, this ) );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( SackOwner == from )
			{
				Open( from );
				from.CloseGump( typeof( BagGump ) );
			}
			else
			{
				SackOwner = from;
				from.CloseGump( typeof( BagGump ) );
				from.SendGump( new BagGump( from, this ) );
				from.PlaySound( 0x048 );
			}
		}

		public override int GetTotal(TotalType type)
        {
			if (type != TotalType.Weight)
				return base.GetTotal(type);
			else
			{
				return (int)(TotalItemWeights() * (0.0));
			}
        }

		public override void UpdateTotal(Item sender, TotalType type, int delta)
        {
            if (type != TotalType.Weight)
                base.UpdateTotal(sender, type, delta);
            else
                base.UpdateTotal(sender, type, (int)(delta * (0.0)));
        }

		private double TotalItemWeights()
        {
			double weight = 0.0;

			foreach (Item item in Items)
				weight += (item.Weight * (double)(item.Amount));

			return weight;
        }

		public SackOfHolding( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write( (Mobile)SackOwner);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			SackOwner = reader.ReadMobile();
			Name = "pack of holding";
			if ( Weight == 3.0 ){ MaxItems = 30; if ( ItemID != 0x6568 || ItemID != 0x6569 ){ ItemID = Utility.RandomList( 0x6568, 0x6569 ); } }
			else if ( Weight == 2.0 ){ MaxItems = 20; if ( ItemID != 0x6568 || ItemID != 0x6569 ){ ItemID = Utility.RandomList( 0x6568, 0x6569 ); } }
			else { MaxItems = 10; if ( ItemID != 0x658D || ItemID != 0x658E ){ ItemID = Utility.RandomList( 0x658D, 0x658E ); } }
			m_MaxWeightDefault = 100000;
		}
	}
}