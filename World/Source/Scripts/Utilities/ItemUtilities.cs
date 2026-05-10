using Server.Items;
using Server.Mobiles;
using Server.Multis;
using System;
using System.Collections.Generic;

namespace Server.Utilities
{
	public static class ItemUtilities
	{
		public static IEnumerable<Item> AddStacks(int totalAmount, Func<Item> createItem)
		{
			const int MAX_STACK_SIZE = 60000;
			int amountRemaining = totalAmount;

			while (0 < amountRemaining)
			{
				Item item = createItem();
				if (MAX_STACK_SIZE < amountRemaining)
				{
					item.Amount = MAX_STACK_SIZE;
					amountRemaining -= MAX_STACK_SIZE;

					yield return item;
				}
				else
				{
					item.Amount = amountRemaining;
					yield return item;
					yield break;
				}
			}
		}

		public static bool HasItemOwnershipRights(Mobile from, Item item)
		{
			if (AccessLevel.Player < from.AccessLevel) return true;

			if (item.RootParentEntity != null)
			{
				var parent = item.RootParentEntity;
				if (parent == from) return true;
				if (parent == from.FindBankNoCreate()) return true;
				if (parent is BaseCreature && ((BaseCreature)parent).ControlMaster == from) return true;
				if (parent is MysticPack && ((MysticPack)parent).Owner == from) return true;
				if (parent is MovingBox && ((MovingBox)parent).Owner == from) return true;
			}

			var house = BaseHouse.FindHouseAt(item);
			if (house != null && house.IsOwner(from)) return true;

			return false;
		}

		public static bool HasItemOwnershipRights(Mobile from, Item item, bool sendMessage)
		{
			var success = HasItemOwnershipRights(from, item);
			if (success) return true;

			if (sendMessage)
				from.SendMessage("You may only Organize containers that belong to you.");

			return false;
		}

		public static bool IsExceptional(Item item)
		{
			if (item == null) return false;

			if (item is BaseWeapon) return ((BaseWeapon)item).Quality == WeaponQuality.Exceptional;
			if (item is BaseArmor) return ((BaseArmor)item).Quality == ArmorQuality.Exceptional;
			if (item is BaseClothing) return ((BaseClothing)item).Quality == ClothingQuality.Exceptional;
			if (item is BaseInstrument) return ((BaseInstrument)item).Quality == InstrumentQuality.Exceptional;
			if (item is BaseInstrument) return ((BaseInstrument)item).Quality == InstrumentQuality.Exceptional;
			if (item is BaseTool) return ((BaseTool)item).Quality == ToolQuality.Exceptional;
			if (item is BaseHarvestTool) return ((BaseHarvestTool)item).Quality == ToolQuality.Exceptional;

			return false;
		}

		public static void SortItems(Container container, IEnumerable<Item> items, int columnWidth, int rowHeight)
		{
			var countainerBounds = container.Bounds;
			var minX = countainerBounds.X;
			var minY = countainerBounds.Y;
			var maxX = minX + countainerBounds.Width;
			var maxY = minY + countainerBounds.Height;
			// Console.WriteLine("MinX: {0}, MinY: {1}, MaxX: {2}, MaxY: {3}", minX, minY, maxX, maxY);

			var column = 0;
			var row = 0;
			foreach (var item in items)
			{
				var x = minX + column * columnWidth;
				if (maxX < x)
				{
					x = minX;
					column = 0; // Reset to Left

					row++; // Increase Y
				}

				var y = minY + row * rowHeight;
				if (maxY < y)
				{
					// Shift to avoid overlap
					minX += columnWidth / 2;
					x = minX;
					column = 0;

					y = minY;
					row = 0; // Reset to Top
				}

				if (item.Parent != container)
					container.DropItem(item);

				item.Location = new Point3D(x, y, 0);
				// Console.WriteLine(string.Format("ItemID: {0}, Name: {1}, Weight: {2} at ({3}, {4})", item.ItemID, item.Name, 0 < item.TotalWeight ? item.TotalWeight : item.PileWeight, x, y));

				column++;
			}
		}
	}
}