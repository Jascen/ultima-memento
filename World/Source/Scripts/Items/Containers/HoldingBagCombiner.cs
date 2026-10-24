using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;

namespace Server.Items
{
	// Dropping a bag or pack of holding onto a matching one offers to merge the two into the next size up:
	// bags 5+5 -> 10, 10+10 -> 20, 20+20 -> a 10 item pack of holding; packs 10+10 -> 20, 20+20 -> 30.
	public static class HoldingBagCombiner
	{
		// Returns true when the drop was a combine attempt and has been handled (the dropped bag goes back where it came from).
		public static bool TryCombine( Mobile from, Container target, Item dropped )
		{
			Container other = dropped as Container;

			if ( other == null || other == target || !Matches( target, other ) )
				return false;

			if ( !Accessible( from, target ) )
			{
				from.SendMessage( "Both bags must be in your backpack to combine them." );
				return true;
			}

			int slots = ResultSlots( target );

			if ( slots == 0 )
			{
				from.SendMessage( "Those bags cannot hold any more magic, and will not combine." );
				return true;
			}

			if ( target.Items.Count + other.Items.Count > slots )
			{
				SendTooFull( from, target, slots );
				return true;
			}

			string message = String.Format(
				"You feel the magic within the two {0} pulling together, hungering to become whole.<br><br>Let the enchantments meet, and they will fold into a single {1} that holds {2} items.<br><br>Everything inside both will be moved into the new one. {3} Do you want to combine them?",
				Describe( target, true ), ResultName( target ), slots, TextDefinition.GetColorizedText( "This cannot be undone.", HtmlColors.RED ) );

			from.SendGump( new ConfirmationGump( from, "Combine Bags of Holding", message, () => Combine( from, target, other ), null, 330 ) );
			return true;
		}

		private static void Combine( Mobile from, Container target, Container other )
		{
			if ( target.Deleted || other.Deleted || !Matches( target, other ) )
				return;

			if ( !Accessible( from, target ) || !Accessible( from, other ) )
			{
				from.SendMessage( "Both bags must be in your backpack to combine them." );
				return;
			}

			int slots = ResultSlots( target );

			if ( slots == 0 )
				return;

			if ( target.Items.Count + other.Items.Count > slots )
			{
				SendTooFull( from, target, slots );
				return;
			}

			Container combined = CreateResult( target );
			Container parent = (Container) target.Parent;

			if ( combined is SackOfHolding )
				((SackOfHolding) combined).SackOwner = from;

			if ( !( target is LargeBagofHolding ) )
				combined.Hue = target.Hue;

			if ( target.LootType == LootType.Blessed || other.LootType == LootType.Blessed )
				combined.LootType = LootType.Blessed;

			WeightReductionContainer bagA = target as WeightReductionContainer;
			WeightReductionContainer bagB = other as WeightReductionContainer;
			WeightReductionContainer bagResult = combined as WeightReductionContainer;

			if ( bagA != null && bagB != null && bagResult != null )
				bagResult.NextAccessTime = bagA.NextAccessTime > bagB.NextAccessTime ? bagA.NextAccessTime : bagB.NextAccessTime;

			combined.Location = target.Location;
			parent.AddItem( combined );

			MoveContents( other, combined );
			MoveContents( target, combined );

			target.Delete();
			other.Delete();

			from.SendMessage( "The two bags fold into one another, becoming a {0}.", Name( combined ) );
			from.PlaySound( 0x1F5 );
		}

		private static void MoveContents( Container source, Container destination )
		{
			List<Item> items = new List<Item>( source.Items );

			foreach ( Item item in items )
				destination.DropItem( item );
		}

		private static void SendTooFull( Mobile from, Container target, int slots )
		{
			from.SendMessage( "A magical force prevents the bags from combining, as the new {0} could only hold {1} items and could not contain everything inside them.", ResultName( target ), slots );
		}

		private static bool Accessible( Mobile from, Item bag )
		{
			return from.Backpack != null && bag.IsChildOf( from.Backpack );
		}

		private static bool Matches( Container a, Container b )
		{
			if ( a is SackOfHolding && b is SackOfHolding )
				return a.MaxItems == b.MaxItems;

			return ( a is SmallBagofHolding || a is MediumBagofHolding || a is LargeBagofHolding ) && a.GetType() == b.GetType();
		}

		// The capacity of what two of these combine into, or 0 when they are already the largest size.
		private static int ResultSlots( Container bag )
		{
			if ( bag is SmallBagofHolding )
				return 10;

			if ( bag is MediumBagofHolding )
				return 20;

			if ( bag is LargeBagofHolding )
				return 10; // becomes a pack of holding, which has no access delay

			if ( bag is SackOfHolding && bag.MaxItems < 30 )
				return bag.MaxItems + 10;

			return 0;
		}

		private static Container CreateResult( Container bag )
		{
			if ( bag is SmallBagofHolding )
				return new MediumBagofHolding();

			if ( bag is MediumBagofHolding )
				return new LargeBagofHolding();

			SackOfHolding pack = new SackOfHolding();
			pack.SetSize( ResultSlots( bag ) );
			return pack;
		}

		private static string ResultName( Container bag )
		{
			if ( bag is SmallBagofHolding )
				return "medium bag of holding";

			if ( bag is MediumBagofHolding )
				return "large bag of holding";

			return "pack of holding";
		}

		private static string Name( Container bag )
		{
			if ( bag is SackOfHolding )
				return String.Format( "pack of holding that holds {0} items", bag.MaxItems );

			return Describe( bag, false );
		}

		private static string Describe( Container bag, bool plural )
		{
			string s = plural ? "s" : "";

			if ( bag is SmallBagofHolding )
				return "small bag" + s + " of holding";

			if ( bag is MediumBagofHolding )
				return "medium bag" + s + " of holding";

			if ( bag is LargeBagofHolding )
				return "large bag" + s + " of holding";

			return String.Format( "pack{0} of holding ({1} items)", s, bag.MaxItems );
		}
	}
}
