using System;
using System.Collections.Generic;

namespace Server.Engines.Avatar
{
	public class ItemSnapshotCache
	{
		private static Dictionary<Type, ItemSnapshot> _cache;

		public static ItemSnapshot GetOrCreate(string typeName)
		{
			Type itemType = ScriptCompiler.FindTypeByName(typeName);
			if (itemType == null) { return null; }

			return GetOrCreate(itemType);
		}

		public static ItemSnapshot GetOrCreate(Type type)
		{
			if (_cache == null) _cache = new Dictionary<Type, ItemSnapshot>();

			Item item = null;
			try
			{
				if (_cache.ContainsKey(type)) return _cache[type];

				item = (Item)Activator.CreateInstance(type);
				if (item == null) return null;

				// Automagic sync prioritizes Item Name over Cliloc Name
				if (item.NameWasSynced)
				{
					var clilocName = CliLocTable.Lookup(item.LabelNumber);
					if (!string.IsNullOrWhiteSpace(clilocName)) item.Name = clilocName;
				}

				return _cache[type] = new ItemSnapshot
				{
					Name = item.Name.ToLower(),
					ItemID = item.ItemID,
					DefaultDescription = item.DefaultDescription,
				};
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to create Item for Avatar Item Cache. {0}", e.Message);
				return null;
			}
			finally
			{
				if (item != null)
				{
					item.Delete();
				}
			}
		}
	}
}