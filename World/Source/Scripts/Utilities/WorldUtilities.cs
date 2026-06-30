using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Utilities
{
	public static class WorldUtilities
	{
		public static void DeleteAllItems<T>(Func<T, bool> predicate) where T : Item
		{
			var toDelete = World.Items.Values
				.Where(item => item is T && predicate((T)item));
			if (toDelete.Any())
			{
				toDelete
					.ToList()
					.ForEach(item => item.Delete());
			}
		}

		public static T FirstOrDefault<T>(Func<T, bool> predicate) where T : Item
		{
			return World.Items.Values
				.Where(item => item is T && predicate((T)item))
				.FirstOrDefault() as T;
		}

		public static IEnumerable<T> ForEachItem<T>(Func<T, bool> predicate) where T : Item
		{
			var items = World.Items.Values
				.Where(item => item is T && predicate((T)item));
			if (!items.Any()) yield break;

			foreach (var item in items.ToList())
			{
				yield return (T)item;
			}
		}

		public static IEnumerable<T> ForEachMobile<T>(Func<T, bool> predicate) where T : Mobile
		{
			var mobiles = World.Mobiles.Values
				.Where(mobile => mobile is T && predicate((T)mobile));
			if (!mobiles.Any()) yield break;

			foreach (var item in mobiles.ToList())
			{
				yield return (T)item;
			}
		}

		/// <summary>
		/// Iterates over all items in the range of a mobile and returns the first item that matches the predicate.
		/// </summary>
		/// <returns>The first item that matches the predicate, otherwise null.</returns>
		public static Item ForEachNearbyItem(Mobile from, int range, Func<Item, bool> stopPredicate = null)
		{
			if (from.Map == null) return null;

			IPooledEnumerable eable = from.Map.GetItemsInRange(from.Location, range);

			foreach (Item item in eable)
			{
				if ((item.Z + 16) > from.Z && (from.Z + 16) > item.Z)
				{
					if (stopPredicate != null && stopPredicate(item))
					{
						eable.Free();
						return item;
					}
				}
			}

			eable.Free();
			return null;
		}

		/// <summary>
		/// Iterates over all static tiles in the range of a mobile and calls the stopPredicate for each static tile.
		/// </summary>
		/// <returns>True if the stopPredicate is true for any static tile, otherwise false.</returns>
		public static bool ForEachNearbyStatic(Mobile from, int range, Func<int, bool> stopPredicate)
		{
			if (from.Map == null) return false;

			for (int x = 0 - range; x <= range; ++x)
			{
				for (int y = 0 - range; y <= range; ++y)
				{
					int vx = from.X + x;
					int vy = from.Y + y;

					StaticTile[] tiles = from.Map.Tiles.GetStaticTiles(vx, vy, true);

					for (int i = 0; i < tiles.Length; ++i)
					{
						int z = tiles[i].Z;
						int id = tiles[i].ID;

						if ((z + 16) > from.Z && (from.Z + 16) > z)
						{
							if (stopPredicate != null && stopPredicate(id)) return true;
						}
					}
				}
			}

			return false;
		}

		public static T GetNearbyItem<T>(Mobile from, int range, Func<T, bool> predicate) where T : Item
		{
			return ForEachNearbyItem(from, range, item =>
			{
				return item is T && predicate((T)item);
			}) as T;
		}

		public static bool HasNearbyItem<T>(Mobile from, int range, Func<T, bool> predicate) where T : Item
		{
			return GetNearbyItem(from, range, predicate) != null;
		}

		public static bool HasNearbyStatic(Mobile from, int range, Func<int, bool> stopPredicate)
		{
			return ForEachNearbyStatic(from, range, stopPredicate);
		}
	}
}