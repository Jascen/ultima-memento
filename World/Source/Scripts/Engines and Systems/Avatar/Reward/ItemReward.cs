using System;
using System.Globalization;

namespace Server.Engines.Avatar
{
	public class ItemReward : IReward
	{
		private static readonly TextInfo m_TextInfo = new CultureInfo("en-US", false).TextInfo;

		private ItemReward()
		{
		}

		public bool CanSelect { get; set; }
		public int Cost { get; private set; }
		public string Description { get; private set; }
		public int Graphic { get; private set; }
		public string Name { get; private set; }
		public Func<Item> OnSelect { get; set; }
		public bool Static { get; set; }

		public static ItemReward Create<T>(int cost, bool canSelect, Func<T> onSelect, int amount = 0, string name = null, string description = null, int graphicOverride = AvatarShopGump.BLANK_ITEM_ID) where T : Item, new()
		{
			var itemSnapshot = ItemSnapshotCache.GetOrCreate(typeof(T));

			if (string.IsNullOrEmpty(name)) name = m_TextInfo.ToTitleCase(itemSnapshot.Name);
			if (0 < amount) name = string.Format("{0} ({1})", name, amount);

			if (string.IsNullOrEmpty(description)) description = itemSnapshot.DefaultDescription;

			return Create(cost, graphicOverride != AvatarShopGump.BLANK_ITEM_ID ? graphicOverride : itemSnapshot.ItemID, name, description, canSelect, onSelect);
		}

		public static ItemReward Create(int cost, int graphic, string name, string description, bool canSelect, Func<Item> onSelect)
		{
			return new ItemReward
			{
				Graphic = graphic,
				Name = name,
				Description = description,
				Cost = cost,
				CanSelect = canSelect,
				OnSelect = onSelect,
				Static = false,
			};
		}

		public ItemReward AsStatic()
		{
			Static = true;
			return this;
		}

		public ItemReward WithDescription(string description)
		{
			Description = description;
			return this;
		}

		public ItemReward WithName(string name)
		{
			Name = name;
			return this;
		}
	}
}