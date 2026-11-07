using System;
using Server.Gumps;

namespace Server.Utilities
{
	public static class GumpUtilities
	{
		public static void AddCenteredItemToGump(Gump gump, int itemId, int x, int y, int slotWidth, int slotHeight, int hue = 0)
		{
			var halfGraphicSlotWidth = slotWidth / 2;
			var halfGraphicSlotHeight = slotHeight / 2;
			AddCenteredItemToGump(gump, itemId, x + halfGraphicSlotWidth, y + halfGraphicSlotHeight, hue);
		}

		public static void AddCenteredItemToGump(Gump gump, int itemId, int x, int y, int hue = 0)
		{
			if (ItemBounds.Table.Length < itemId)
			{
				Console.WriteLine("Attempted to look up item bounds for itemId {0}, but ItemBounds.Table.Length is {1}", itemId, ItemBounds.Table.Length);
				return;
			}

			Rectangle2D bounds = ItemBounds.Table[itemId];
			gump.AddItem(
				x - bounds.X - (bounds.Width / 2),
				y - bounds.Y - (bounds.Height / 2),
				itemId,
				hue
			);
		}

		public static void AddRightAlignedItemToGump(Gump gump, int itemId, int x, int y, int hue = 0)
		{
			if (ItemBounds.Table.Length < itemId)
			{
				Console.WriteLine("Attempted to look up item bounds for itemId {0}, but ItemBounds.Table.Length is {1}", itemId, ItemBounds.Table.Length);
				return;
			}

			Rectangle2D bounds = ItemBounds.Table[itemId];
			gump.AddItem(
				x - bounds.X - bounds.Width,
				y - bounds.Y,
				itemId,
				hue
			);
		}
	}
}