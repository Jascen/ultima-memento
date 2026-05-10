using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Server.Engines.GlobalShoppe
{
	public class ShoppeRewardGump : Gump
	{
		private const int CARD_HEIGHT = 68;
		private const int CARD_WIDTH = 864 - NAVIGATION_WIDTH;
		private const int CATEGORY_WIDTH = NAVIGATION_WIDTH - 28 - 20;
		private const int NAVIGATION_WIDTH = 152 + 20 + 20;

		private static readonly List<ShoppeType> m_Categories = new List<ShoppeType>
		{
			ShoppeType.Alchemist,
			ShoppeType.Baker,
			ShoppeType.Blacksmith,
			ShoppeType.Bowyer,
			ShoppeType.Carpentry,
            // ShoppeType.Cartography, // Can't get Points atm
            ShoppeType.Herbalist,
			ShoppeType.Librarian,
			ShoppeType.Mortician,
			ShoppeType.Tailor,
			ShoppeType.Tinker,
		};

		private static readonly Dictionary<ShoppeType, List<_Reward>> m_Rewards = new Dictionary<ShoppeType, List<_Reward>>
		{
			{
				ShoppeType.Alchemist, new List<_Reward>
				{
					_Reward.Create(1000, () => { return new MortarPestle(1000); }, uses: 1000),
					_Reward.Create(1000, () => { return new RepairPotion(); }),
					_Reward.Create(2500, () => { return new DurabilityPotion(); }),
					_Reward.Create(2500, () => { return new MagicalDyes(); }, uses: 1, description: "A vial of a rare hue, usable on a single item."),
					_Reward.Create(10000, () => { return new AncientCraftingGloves(SkillName.Alchemy, 5, 10); }, uses: 10, name: "Ancient Alchemy Gloves (+5 skill)"),
					_Reward.Create(20000, () => { return new SoulstoneFragment(1); }, uses: 1, description: "Extract a single skill. Binds to account."),

					_Reward.Create(100000, () => { return new Artifact_BootsofHermes(); }, description: "Allows players to run at mounted speed."),
				}
			},
			{
				ShoppeType.Baker, new List<_Reward>
				{
					_Reward.Create(1000, () => { return new CulinarySet(1000); }, uses: 1000),
					_Reward.Create(10000, () => { return new AncientCraftingGloves(SkillName.Cooking, 5, 10); }, uses: 10, name: "Ancient Baker Gloves (+5 skill)"),
					_Reward.Create(50000, () => { return new EverlastingBottle(); }, description: "A magical bottle that refills itself when you drink from it."),
					_Reward.Create(50000, () => { return new EverlastingLoaf(); }, description: "A magical loaf that reforms itself when you eat it."),

					_Reward.Create(100000, () => { return new Artifact_BootsofHermes(); }, description: "Allows players to run at mounted speed."),
				}
			},
			{
				ShoppeType.Blacksmith, new List<_Reward>
				{
					_Reward.Create(1000, () => { return new SmithHammer(1000); }, uses: 1000),
					_Reward.Create(5000, () => { return new MetalDyeTubTemp(20); }, uses: 20, description: "Apply dye to metal items."),
					_Reward.Create(10000, () => { return new AncientCraftingGloves(SkillName.Blacksmith, 5, 10); }, uses: 10, name: "Ancient Blacksmith Gloves (+5 skill)"),

					_Reward.Create(5000, () => { return new SmithHammerRunicI(15); }, uses: 15, name: "Runic (I)", description: "Runic tool; crafted items have 1 magical property."),
					_Reward.Create(20000, () => { return new SmithHammerRunicII(10); }, uses: 10, name: "Runic (II)", description: "Runic tool; crafted items have 2 magical properties."),
					_Reward.Create(50000, () => { return new SmithHammerRunicIII(5); }, uses: 5, name: "Runic (III)", description: "Runic tool; crafted items have 3 magical properties."),

					_Reward.Create(100000, () => { return new Artifact_BootsofHermes(); }, description: "Allows players to run at mounted speed."),
				}
			},
			{
				ShoppeType.Bowyer, new List<_Reward>
				{
					_Reward.Create(1000, () => { return new FletcherTools(1000); }, uses: 1000),
					_Reward.Create(5000, () => { return new ArboristTool(50); }, uses: 50, name: "Arborist Tool"),
					_Reward.Create(5000, () => { return new WoodDyeTubTemp(20); }, uses: 20, description: "Apply dye to wooden items."),
					_Reward.Create(10000, () => { return new AncientCraftingGloves(SkillName.Bowcraft, 5, 10); }, uses: 10, name: "Ancient Bowyer Gloves (+5 skill)"),

					_Reward.Create(5000, () => { return new FletcherToolsRunicI(15); }, uses: 15, name: "Runic (I)", description: "Runic tool; crafted items have 1 magical property."),
					_Reward.Create(20000, () => { return new FletcherToolsRunicII(10); }, uses: 10, name: "Runic (II)", description: "Runic tool; crafted items have 2 magical properties."),
					_Reward.Create(50000, () => { return new FletcherToolsRunicIII(5); }, uses: 5, name: "Runic (III)", description: "Runic tool; crafted items have 3 magical properties."),

					_Reward.Create(100000, () => { return new Artifact_BootsofHermes(); }, description: "Allows players to run at mounted speed."),
				}
			},
			{
				ShoppeType.Carpentry, new List<_Reward> {
					_Reward.Create(1000, () => { return new CarpenterTools(1000); }, uses: 1000),
					_Reward.Create(5000, () => { return new ArboristTool(50); }, uses: 50, name: "Arborist Tool"),
					_Reward.Create(5000, () => { return new WoodDyeTubTemp(20); }, uses: 20, description: "Apply dye to wooden items."),
					_Reward.Create(10000, () => { return new AncientCraftingGloves(SkillName.Carpentry, 5, 10); }, uses: 10, name: "Ancient Carpenter Gloves (+5 skill)"),

					_Reward.Create(5000, () => { return new CarpenterToolsRunicI(15); }, uses: 15, name: "Runic (I)", description: "Runic tool; crafted items have 1 magical property."),
					_Reward.Create(20000, () => { return new CarpenterToolsRunicII(10); }, uses: 10, name: "Runic (II)", description: "Runic tool; crafted items have 2 magical properties."),
					_Reward.Create(50000, () => { return new CarpenterToolsRunicIII(5); }, uses: 5, name: "Runic (III)", description: "Runic tool; crafted items have 3 magical properties."),

					_Reward.Create(100000, () => { return new Artifact_BootsofHermes(); }, description: "Allows players to run at mounted speed."),
				}
			},
			{ // Warning: Not in use
				ShoppeType.Cartography, new List<_Reward>
				{
					_Reward.Create(1000, () => { return new MapmakersPen(1000); }, uses: 1000),
					_Reward.Create(10000, () => { return new AncientCraftingGloves(SkillName.Cartography, 5, 10); }, uses: 10, name: "Ancient Cartographer Gloves (+5 skill)"),

					_Reward.Create(100000, () => { return new Artifact_BootsofHermes(); }, description: "Allows players to run at mounted speed."),
				}
			},
			{
				ShoppeType.Herbalist, new List<_Reward>
				{
					_Reward.Create(1000, () => { return new DruidCauldron(1000); }, uses: 1000),
					_Reward.Create(1000, () => { return new RepairPotion(); }),
					_Reward.Create(2500, () => { return new DurabilityPotion(); }),
					_Reward.Create(2500, () => { return new MagicalDyes(); }, uses: 1, description: "A vial of a rare hue, usable on a single item."),
					_Reward.Create(10000, () => { return new AncientCraftingGloves(SkillName.Druidism, 5, 10); }, uses: 10, name: "Ancient Herbalist Gloves (+5 skill)"),
					_Reward.Create(20000, () => { return new SoulstoneFragment(1); }, uses: 1, description: "Extract a single skill. Binds to account."),

					_Reward.Create(100000, () => { return new Artifact_BootsofHermes(); }, description: "Allows players to run at mounted speed."),
				}
			},
			{
				ShoppeType.Librarian, new List<_Reward>
				{
					_Reward.Create(1000, () => { return new ScribesPen(1000); }, uses: 1000),
					_Reward.Create(2000, () => { return new Monocle(250); }, uses: 250),
					_Reward.Create(10000, () => { return new AncientCraftingGloves(SkillName.Inscribe, 5, 10); }, uses: 10, name: "Ancient Librarian Gloves (+5 skill)"),

					_Reward.Create(5000, () => { return new ScribesPenRunicI(15); }, uses: 15, name: "Runic (I)", description: "Runic tool; crafted items have 1 magical property."),
					_Reward.Create(20000, () => { return new ScribesPenRunicII(10); }, uses: 10, name: "Runic (II)", description: "Runic tool; crafted items have 2 magical properties."),
					_Reward.Create(50000, () => { return new ScribesPenRunicIII(5); }, uses: 5, name: "Runic (III)", description: "Runic tool; crafted items have 3 magical properties."),

					_Reward.Create(100000, () => { return new Artifact_BootsofHermes(); }, description: "Allows players to run at mounted speed."),
				}
			},
			{
				ShoppeType.Mortician, new List<_Reward>
				{
					_Reward.Create(1000, () => { return new WitchCauldron(1000); }, uses: 1000),
					_Reward.Create(2500, () => { return new DurabilityPotion(); }),
					_Reward.Create(10000, () => { return new AncientCraftingGloves(SkillName.Forensics, 5, 10); }, uses: 10, name: "Ancient Mortician Gloves (+5 skill)"),

					_Reward.Create(5000, () => { return new UndertakerKitRunicI(15); }, uses: 15, name: "Runic (I)", description: "Runic tool; crafted items have 1 magical property."),
					_Reward.Create(20000, () => { return new UndertakerKitRunicII(10); }, uses: 10, name: "Runic (II)", description: "Runic tool; crafted items have 2 magical properties."),
					_Reward.Create(50000, () => { return new UndertakerKitRunicIII(5); }, uses: 5, name: "Runic (III)", description: "Runic tool; crafted items have 3 magical properties."),

					_Reward.Create(100000, () => { return new Artifact_BootsofHermes(); }, description: "Allows players to run at mounted speed."),
				}
			},
			{
				ShoppeType.Tailor, new List<_Reward>
				{
					_Reward.Create(1000, () => { return new SewingKit(1000); }, uses: 1000),
					_Reward.Create(1000, () => { return new LeatherworkingTools(1000); }, uses: 1000),
					_Reward.Create(5000, () => { return new AdvancedSkinningKnife(20, 100); }, uses: 100, name: "Advanced Skinning Knife", description: "Increases carving yields by 20%."),
					_Reward.Create(5000, () => { return new LeatherDyeTubTemp(20); }, uses: 20, description: "Apply dye to leather items."),
					_Reward.Create(10000, () => { return new AncientCraftingGloves(SkillName.Tailoring, 5, 10); }, uses: 10, name: "Ancient Tailor Gloves (+5 skill)"),

					_Reward.Create(5000, () => { return new SewingKitRunicI(15); }, uses: 15, name: "Runic (I)", description: "Runic tool; crafted items have 1 magical property."),
					_Reward.Create(20000, () => { return new SewingKitRunicII(10); }, uses: 10, name: "Runic (II)", description: "Runic tool; crafted items have 2 magical properties."),
					_Reward.Create(50000, () => { return new SewingKitRunicIII(5); }, uses: 5, name: "Runic (III)", description: "Runic tool; crafted items have 3 magical properties."),

					_Reward.Create(5000, () => { return new LeatherworkingToolsRunicI(15); }, uses: 15, name: "Runic (I)", description: "Runic tool; crafted items have 1 magical property."),
					_Reward.Create(20000, () => { return new LeatherworkingToolsRunicII(10); }, uses: 10, name: "Runic (II)", description: "Runic tool; crafted items have 2 magical properties."),
					_Reward.Create(50000, () => { return new LeatherworkingToolsRunicIII(5); }, uses: 5, name: "Runic (III)", description: "Runic tool; crafted items have 3 magical properties."),

					_Reward.Create(100000, () => { return new Artifact_BootsofHermes(); }, description: "Allows players to run at mounted speed."),
				}
			},
			{
				ShoppeType.Tinker, new List<_Reward>
				{
					_Reward.Create(1000, () => { return new TinkerTools(1000); }, uses: 1000),
					_Reward.Create(5000, () => { return new MetalDyeTubTemp(20); }, uses: 20, description: "Apply dye to metal items."),
					_Reward.Create(10000, () => { return new AncientCraftingGloves(SkillName.Tinkering, 5, 10); }, uses: 10, name: "Ancient Tinker Gloves (+5 skill)"),
					_Reward.Create(50000, () => { return new HueVacuumTube(); }, uses: 1),

					_Reward.Create(5000, () => { return new TinkerToolsRunicI(15); }, uses: 15, name: "Runic (I)", description: "Runic tool; crafted items have 1 magical property."),
					_Reward.Create(20000, () => { return new TinkerToolsRunicII(10); }, uses: 10, name: "Runic (II)", description: "Runic tool; crafted items have 2 magical properties."),
					_Reward.Create(50000, () => { return new TinkerToolsRunicIII(5); }, uses: 5, name: "Runic (III)", description: "Runic tool; crafted items have 3 magical properties."),

					_Reward.Create(100000, () => { return new Artifact_BootsofHermes(); }, description: "Allows players to run at mounted speed."),
				}
			},
		};

		private readonly PlayerContext m_Context;
		private readonly PlayerMobile m_From;
		private readonly Action m_onGumpClose;
		private readonly int m_PageNumber;
		private readonly ShoppeType m_SelectedShoppeType;

		public ShoppeRewardGump(PlayerMobile from, ShoppeType selectedShoppeType = (ShoppeType)(-1), int pageNumber = 1, Action onGumpClose = null) : base(25, 25)
		{
			m_Context = ShoppeEngine.Instance.GetOrCreateContext(from);
			m_From = from;
			m_onGumpClose = onGumpClose;
			m_PageNumber = pageNumber;
			m_SelectedShoppeType = selectedShoppeType;

			AddPage(0);

			AddImage(0, 0, 7028, Server.Misc.PlayerSettings.GetGumpHue(from));

			const int GUMP_WIDTH = 904;
			const int GUMP_HEIGHT = 729;
			TextDefinition.AddHtmlText(this, 11, 11, GUMP_WIDTH, 20, "<CENTER>Shoppe Reward Selection</CENTER>", HtmlColors.BROWN);


			AddShoppeTypeList(selectedShoppeType, 27, 48);

			List<_Reward> rewards;
			if (m_Rewards.TryGetValue(selectedShoppeType, out rewards))
			{
				int ITEMS_PER_PAGE = 8;
				int y = 48;
				var skip = (pageNumber - 1) * ITEMS_PER_PAGE;
				int itemIndex = skip;
				foreach (var reward in rewards.Skip(skip).Take(ITEMS_PER_PAGE))
				{
					AddCard(m_Context[selectedShoppeType].Points, reward.Graphic, reward.Name, reward.Description, reward.Cost, itemIndex, y);

					y += CARD_HEIGHT;
					y += 10;
					++itemIndex;
				}

				y = GUMP_HEIGHT - 33;

				// Prev page
				if (0 < skip)
					AddButton(NAVIGATION_WIDTH + 20, y, 4014, 4015, (int)Actions.PageBase + (pageNumber - 1), GumpButtonType.Reply, 0);

				// Next page
				var remain = rewards.Count - skip - ITEMS_PER_PAGE;
				if (0 < remain)
					AddButton(GUMP_WIDTH - 47, y, 4005, 4007, (int)Actions.PageBase + (pageNumber + 1), GumpButtonType.Reply, 0);
			}
		}

		private enum Actions
		{
			Close = 0,

			// Help = 1,
			SelectShoppeTypeBase = 10,

			PurchaseBase = 50,
			PageBase = 300,
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			var buttonID = info.ButtonID;
			if (buttonID == 0)
			{
				if (m_onGumpClose != null)
					m_onGumpClose();
				return;
			}

			var player = sender.Mobile as PlayerMobile;
			if (player == null) return;

			if ((int)Actions.PageBase <= buttonID) // Page is higher
			{
				var page = buttonID - (int)Actions.PageBase;
				sender.Mobile.SendGump(new ShoppeRewardGump(m_From, m_SelectedShoppeType, page, m_onGumpClose));
				return;
			}
			else if ((int)Actions.PurchaseBase <= buttonID) // Purchase is higher
			{
				var index = buttonID - (int)Actions.PurchaseBase;
				var reward = m_Rewards[m_SelectedShoppeType][index];
				var context = m_Context[m_SelectedShoppeType];
				if (context.Points < reward.Cost)
				{
					player.SendMessage("You do not have enough points to purchase this reward.");
				}
				else
				{
					var item = reward.OnSelect();
					if (item != null)
					{
						context.Points -= reward.Cost;
						player.AddToBackpack(item);
						player.SendMessage("You have purchased the {0} for {1} points.", reward.Name, reward.Cost);
					}
				}

				sender.Mobile.SendGump(new ShoppeRewardGump(m_From, m_SelectedShoppeType, m_PageNumber, m_onGumpClose));
			}
			else if ((int)Actions.SelectShoppeTypeBase <= buttonID) // Select Shoppe Type is higher
			{
				var selectedShoppeType = (ShoppeType)(buttonID - (int)Actions.SelectShoppeTypeBase);
				sender.Mobile.SendGump(new ShoppeRewardGump(m_From, selectedShoppeType, 1, m_onGumpClose));
				return;
			}
		}

		private void AddCard(
			int points,
			int itemId,
			string name,
			string description,
			int cost,
			int index,
			int y
			)
		{
			const int START_X = NAVIGATION_WIDTH + 20;
			const int GRAPHIC_SLOT_WIDTH = 73;
			const int GRAPHIC_SLOT_HEIGHT = 68;
			const int DESCRIPTION_START = START_X + GRAPHIC_SLOT_WIDTH + 10;
			const int PURCHASE_WIDTH = 130;
			const int DESCRIPTION_WIDTH = CARD_WIDTH - GRAPHIC_SLOT_WIDTH + 10 - PURCHASE_WIDTH;

			int x = START_X;

			AddBackground(x, y, CARD_WIDTH, CARD_HEIGHT + 5, 2620);
			AddBackground(x, y, GRAPHIC_SLOT_WIDTH, CARD_HEIGHT + 5, 2620);

			// Item image
			if (itemId > 0)
				GumpUtilities.AddCenteredItemToGump(this, itemId, x, y, GRAPHIC_SLOT_WIDTH, GRAPHIC_SLOT_HEIGHT);

			// Item text
			x = DESCRIPTION_START;
			y += 5; // Top padding
			const int LAZY_AMOUNT = 30; // Arbitrary value to account for left padding
			TextDefinition.AddHtmlText(this, x, y, DESCRIPTION_WIDTH - LAZY_AMOUNT, 20, name, HtmlColors.MUSTARD);
			TextDefinition.AddHtmlText(this, x + 10, y + 20, DESCRIPTION_WIDTH - LAZY_AMOUNT, 40, TextDefinition.GetColorizedText(description, HtmlColors.BROWN), false, true);

			x = START_X + CARD_WIDTH - PURCHASE_WIDTH;
			y += 6;

			// Purchase section
			const int GRAPHIC_WIDTH = 55;
			const int GRAPHIC_HEIGHT = 22;
			const int POINTS_ITEM = 0x0EEC;
			var canAfford = cost <= points;
			var pointsColor = canAfford ? HtmlColors.MUSTARD : HtmlColors.RED;
			GumpUtilities.AddCenteredItemToGump(this, POINTS_ITEM, x, y, GRAPHIC_WIDTH, GRAPHIC_HEIGHT, 0x44C);
			TextDefinition.AddHtmlText(this, x + GRAPHIC_WIDTH, y + 2, 50, 20, cost.ToString("n0"), pointsColor);

			if (canAfford)
			{
				y += 30;

				AddButton(x + 13, y - 1, 4023, 4023, (int)Actions.PurchaseBase + index, GumpButtonType.Reply, 0); // OK
				TextDefinition.AddHtmlText(this, x + GRAPHIC_WIDTH, y + 2, 60, 20, "Purchase", HtmlColors.MUSTARD);
			}
		}

		private void AddShoppeTypeList(ShoppeType selectedShoppeType, int x, int y)
		{
			int i = 0;
			foreach (var shoppeType in m_Categories)
			{
				var isSelected = selectedShoppeType == shoppeType;
				const int CARD_HEIGHT = 54;
				const int TOP_PADDING = 6;
				const int HEIGHT_PER_ITEM = CARD_HEIGHT + TOP_PADDING; // Extra top padding
				const int LEFT_PADDING = 36;

				if (!isSelected)
				{
					const int HIDDEN_BUTTON_ID = 0x1; // 44x44 px
					const int HIDDEN_BUTTON_WIDTH = 44;

					int buttonId = (int)Actions.SelectShoppeTypeBase + (int)shoppeType;
					AddButton(x, y + TOP_PADDING + (i * HEIGHT_PER_ITEM), HIDDEN_BUTTON_ID, HIDDEN_BUTTON_ID, buttonId, GumpButtonType.Reply, 0);
					AddButton(x + HIDDEN_BUTTON_WIDTH, y + TOP_PADDING + (i * HEIGHT_PER_ITEM), HIDDEN_BUTTON_ID, HIDDEN_BUTTON_ID, buttonId, GumpButtonType.Reply, 0);
					AddButton(x + HIDDEN_BUTTON_WIDTH + HIDDEN_BUTTON_WIDTH, y + TOP_PADDING + (i * HEIGHT_PER_ITEM), HIDDEN_BUTTON_ID, HIDDEN_BUTTON_ID, buttonId, GumpButtonType.Reply, 0);
				}

				AddBackground(x, y + (i * HEIGHT_PER_ITEM), CATEGORY_WIDTH, HEIGHT_PER_ITEM - 6, 2620);

				var gemGraphic = isSelected ? 1210 : 1209;
				var gemHue = isSelected ? 1152 : 0;
				AddImage(x + 17, y + 10 + (i * HEIGHT_PER_ITEM), gemGraphic, gemHue);

				var color = isSelected ? HtmlColors.MUSTARD : HtmlColors.BROWN;
				TextDefinition.AddHtmlText(this, x + LEFT_PADDING, y + 7 + (i * HEIGHT_PER_ITEM), CATEGORY_WIDTH - LEFT_PADDING, 16, shoppeType.ToString(), color);
				TextDefinition.AddHtmlText(this, x + LEFT_PADDING, y + 27 + (i * HEIGHT_PER_ITEM), CATEGORY_WIDTH - LEFT_PADDING, 16, string.Format("{0:n0} points", m_Context[shoppeType].Points), HtmlColors.MUSTARD);
				++i;
			}
		}

		private class _Reward
		{
			private static readonly TextInfo m_TextInfo = new CultureInfo("en-US", false).TextInfo;

			private _Reward()
			{
			}

			public int Cost { get; set; }
			public string Description { get; set; }
			public int Graphic { get; set; }
			public string Name { get; set; }
			public Func<Item> OnSelect { get; set; }

			public static _Reward Create<T>(int cost, Func<T> onSelect, int uses = 0, string name = null, string description = null) where T : Item, new()
			{
				var itemSnapshot = ShoppeItemCache.GetOrCreate(typeof(T));

				if (string.IsNullOrEmpty(name)) name = m_TextInfo.ToTitleCase(itemSnapshot.Name);
				if (0 < uses) name = string.Format("{0} ({1} use{2})", name, uses, uses == 1 ? "" : "s");

				if (string.IsNullOrEmpty(description)) description = itemSnapshot.DefaultDescription;

				return Create(itemSnapshot.ItemID, name, description, cost, onSelect);
			}

			private static _Reward Create(int graphic, string name, string description, int cost, Func<Item> onSelect)
			{
				return new _Reward
				{
					Graphic = graphic,
					Name = name,
					Description = description,
					Cost = cost,
					OnSelect = onSelect,
				};
			}
		}
	}
}