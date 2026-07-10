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
		public static Item ForEachNearbyItem(Map map, Point3D location, int range, Func<Item, bool> stopPredicate = null)
		{
			if (map == null || map == Map.Internal) return null;

			IPooledEnumerable eable = map.GetItemsInRange(location, range);

			foreach (Item item in eable)
			{
				if ((item.Z + 16) > location.Z && (location.Z + 16) > item.Z)
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

		public static Item ForEachNearbyItem(Mobile mobile, int range, Func<Item, bool> stopPredicate = null)
		{
			return ForEachNearbyItem(mobile.Map, mobile.Location, range, stopPredicate);
		}

		/// <summary>
		/// Iterates over all mobiles in the range of a mobile and returns the first mobile that matches the predicate.
		/// </summary>
		/// <returns>The first mobile that matches the predicate, otherwise null.</returns>
		public static Mobile ForEachNearbyMobile(Map map, Point3D location, int range, Func<Mobile, bool> stopPredicate = null)
		{
			if (map == null || map == Map.Internal) return null;

			IPooledEnumerable eable = map.GetMobilesInRange(location, range);

			foreach (Mobile mobile in eable)
			{
				if (stopPredicate != null && stopPredicate(mobile))
				{
					eable.Free();
					return mobile;
				}
			}

			eable.Free();
			return null;
		}

		public static Mobile ForEachNearbyMobile(Mobile mobile, int range, Func<Mobile, bool> stopPredicate)
		{
			return ForEachNearbyMobile(mobile.Map, mobile.Location, range, stopPredicate);
		}

		/// <summary>
		/// Iterates over all static tiles in the range of a mobile and calls the stopPredicate for each static tile.
		/// </summary>
		/// <returns>True if the stopPredicate is true for any static tile, otherwise false.</returns>
		public static bool ForEachNearbyStatic(Map map, IPoint3D location, int range, Func<int, bool> stopPredicate)
		{
			if (map == null || map == Map.Internal) return false;

			for (int x = 0 - range; x <= range; ++x)
			{
				for (int y = 0 - range; y <= range; ++y)
				{
					int vx = location.X + x;
					int vy = location.Y + y;

					StaticTile[] tiles = map.Tiles.GetStaticTiles(vx, vy, true);

					for (int i = 0; i < tiles.Length; ++i)
					{
						int z = tiles[i].Z;
						int id = tiles[i].ID;

						if ((z + 16) > location.Z && (location.Z + 16) > z)
						{
							if (stopPredicate != null && stopPredicate(id)) return true;
						}
					}
				}
			}

			return false;
		}

		public static bool ForEachNearbyStatic(Mobile mobile, int range, Func<int, bool> stopPredicate)
		{
			return ForEachNearbyStatic(mobile.Map, mobile.Location, range, stopPredicate);
		}

		public static IEnumerable<T> GetAllNearbyItems<T>(Mobile from, int range, Func<T, bool> predicate) where T : Item
		{
			return GetAllNearbyItems(from.Map, from.Location, range, predicate);
		}

		/// <summary>
		/// Iterates over all items in the range of a location and returns all items that match the predicate.
		/// </summary>
		public static IEnumerable<T> GetAllNearbyItems<T>(Map map, Point3D location, int range, Func<T, bool> predicate) where T : Item
		{
			if (map == null || map == Map.Internal) yield break;

			List<T> items = null;
			IPooledEnumerable eable = map.GetItemsInRange(location, range);
			foreach (Item item in eable)
			{
				// TODO: Z+16 is probably fine...
				if ((item.Z + 16) > location.Z && (location.Z + 16) > item.Z)
				{
					if (item is T && predicate((T)item))
					{
						if (items == null) items = new List<T>();
						items.Add((T)item);
					}
				}
			}
			eable.Free();

			if (items == null) yield break;

			foreach (var item in items)
			{
				yield return item;
			}
		}

		public static T GetNearbyItem<T>(Map map, Point3D location, int range, Func<T, bool> predicate) where T : Item
		{
			return ForEachNearbyItem(map, location, range, item =>
			{
				return item is T && predicate((T)item);
			}) as T;
		}

		public static T GetNearbyItem<T>(Mobile mobile, int range, Func<T, bool> predicate) where T : Item
		{
			return GetNearbyItem(mobile.Map, mobile.Location, range, predicate);
		}

		public static T GetNearbyMobile<T>(Map map, Point3D location, int range, Func<T, bool> predicate) where T : Mobile
		{
			return ForEachNearbyMobile(map, location, range, mobile =>
			{
				return mobile is T && predicate((T)mobile);
			}) as T;
		}

		public static T GetNearbyMobile<T>(Mobile mobile, int range, Func<T, bool> predicate) where T : Mobile
		{
			return GetNearbyMobile(mobile.Map, mobile.Location, range, predicate);
		}

		public static bool HasNearbyItem<T>(Map map, Point3D location, int range, Func<T, bool> predicate) where T : Item
		{
			return GetNearbyItem(map, location, range, predicate) != null;
		}

		public static bool HasNearbyItem<T>(Mobile mobile, int range, Func<T, bool> predicate) where T : Item
		{
			return GetNearbyItem(mobile, range, predicate) != null;
		}

		public static bool HasNearbyMobile<T>(Map map, Point3D location, int range, Func<T, bool> predicate) where T : Mobile
		{
			return GetNearbyMobile(map, location, range, predicate) != null;
		}

		public static bool HasNearbyMobile<T>(Mobile mobile, int range, Func<T, bool> predicate) where T : Mobile
		{
			return GetNearbyMobile(mobile, range, predicate) != null;
		}

		public static bool HasNearbyStatic(Map map, IPoint3D location, int range, Func<int, bool> stopPredicate)
		{
			return ForEachNearbyStatic(map, location, range, stopPredicate);
		}

		public static bool HasNearbyStatic(Mobile mobile, int range, Func<int, bool> stopPredicate)
		{
			return ForEachNearbyStatic(mobile.Map, mobile.Location, range, stopPredicate);
		}
	}
}