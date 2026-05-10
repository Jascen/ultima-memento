using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Avatar
{
	public class AvatarShopGump : Gump
	{
		public const int BLANK_ITEM_ID = 0;
		public const int COST_FREE = -1;
		public const int FAT_BOTTLE_ITEM_ID = 0x1FD9;
		public const int GOLD_STACK_ITEM_ID = 0x0EEF;
		public const int COST_NO_BUY = 0;
		public const int NO_ITEM_ID = -1;
		private const int CARD_HEIGHT = 68;
		private const int CARD_WIDTH = 864 - NAVIGATION_WIDTH;
		private const int CATEGORY_WIDTH = NAVIGATION_WIDTH - 28 - 20;
		private const int GIANT_COIN_ITEM_ID = 0x4FAD;
		private const int NAVIGATION_WIDTH = 152 + 20 + 20;

		private static readonly List<Categories> m_Categories = new List<Categories>
		{
			Categories.Information,
			Categories.Ascensions,
			Categories.Templates,
			Categories.PrimaryBoosts,
			Categories.SecondaryBoosts,
			Categories.FullSkillArchive,
			// Categories.Items,
		};

		private readonly PlayerContext m_Context;
		private readonly PlayerMobile m_From;
		private readonly bool m_InGypsyEncampment;
		private readonly Action m_onGumpClose;
		private readonly int m_PageNumber;
		private readonly List<IReward> m_Rewards;
		private readonly Categories m_SelectedCategory;

		public AvatarShopGump(PlayerMobile from, Categories selectedCategory = Categories.Information, int pageNumber = 1, Action onGumpClose = null) : base(25, 25)
		{
			m_Context = AvatarEngine.Instance.GetOrCreateContext(from);
			m_From = from;
			m_onGumpClose = onGumpClose;
			m_PageNumber = pageNumber;
			m_SelectedCategory = selectedCategory;
			m_InGypsyEncampment = InGypsyEncampment(from);

			AddPage(0);

			const int BACKGROUND_WIDTH = 904;
			const int BACKGROUND_HEIGHT = 729;
			AddBackground(0, 0, BACKGROUND_WIDTH, BACKGROUND_HEIGHT, 2620);

			const int GUMP_WIDTH = BACKGROUND_WIDTH;
			const int GUMP_HEIGHT = BACKGROUND_HEIGHT;
			TextDefinition.AddHtmlText(this, 11, 11, GUMP_WIDTH, 20, "<CENTER>The Avatar's Ascent</CENTER>", HtmlColors.COOL_BLUE);

			AddCategoryList(selectedCategory, 27, 48);

			var y = 48;

			if (selectedCategory == Categories.Information)
			{
				AddKeyValuePairsCard(NAVIGATION_WIDTH + 20, y);
				y += CARD_HEIGHT;
				y += 10;

				AddInformationCard(NO_ITEM_ID, "An Avatar is Born", "You have begun the Avatar's Ascent. This is a challenging journey of self-discovery and growth.", y);
				y += CARD_HEIGHT;
				y += 10;

				AddInformationCard(NO_ITEM_ID, "Everything is Temporary", "Unless explicitly stated, everything your character owns will be deleted when your character dies. The only exception to this is your house, which will be inherited by your new character.", y);
				y += CARD_HEIGHT;
				y += 10;

				AddInformationCard(NO_ITEM_ID, "Meta Progression", string.Format(
					"{0} are permanent and may be purchased for a fee. {0} can enable permanence between runs, unlock new features, accelerate progression, or even raise the ceiling of your character.",
					TextDefinition.GetColorizedText("Ascensions", HtmlColors.ORANGE)
				), y);
				y += CARD_HEIGHT;
				y += 10;

				AddInformationCard(NO_ITEM_ID, "Your Template", string.Format(
					"Your Avatar is weak when they are reborn. It is recommended that you select a {0} each run. The Erudian Ascensions will allw you to recover skills using the {1} and {2} skill pages.",
					TextDefinition.GetColorizedText("Template", HtmlColors.ORANGE),
					TextDefinition.GetColorizedText("Primary", HtmlColors.ORANGE),
					TextDefinition.GetColorizedText("Secondary", HtmlColors.ORANGE)
				), y);
				y += CARD_HEIGHT;
				y += 10;

				if (m_Context.HasRivalFaction)
				{
					AddInformationCard(NO_ITEM_ID, "Enemy Faction", string.Format("Your family has been wronged by {0}. You will receive a bonus each time you kill monsters of this faction. Once you have avenged your family, you will no longer receive the bonus.", TextDefinition.GetColorizedText(m_Context.RivalFactionName, HtmlColors.ORANGE)), y);
					y += CARD_HEIGHT;
					y += 10;
				}

				AddInformationCard(NO_ITEM_ID, "Opportunity Only Knocks Once...", string.Format(
					"Your vessel is frail; once you leave the {0} you may not purchase anything from this gump.",
					TextDefinition.GetColorizedText("Gypsy Encampment", HtmlColors.ORANGE)
				), y);
				y += CARD_HEIGHT;
				y += 10;

				return;
			}

			if (pageNumber == 1)
			{
				switch (m_SelectedCategory)
				{
					case Categories.Ascensions:
						{
							AddInformationCard(BLANK_ITEM_ID, "Ascensions - Unlock Permanent Enhancements", "Ascensions are a way to apply permanent changes to your lineage. Some of these changes may allow you to persist knowledge between runs while others may simply make you stronger.", y, addBackground: false);
							break;
						}

					case Categories.Templates:
						{
							AddInformationCard(BLANK_ITEM_ID, "Templates - Select Your Beginning", "A template can provide a fixed combination of skills, stats, and/or items to aid with your run. The templates available to you will change each run.", y, addBackground: false);
							break;
						}

					case Categories.PrimaryBoosts:
					case Categories.SecondaryBoosts:
						{
							var isPrimary = m_SelectedCategory == Categories.PrimaryBoosts;
							AddInformationCard(
								BLANK_ITEM_ID,
								isPrimary ? "Available Primary Skills" : "Available Secondary Skills",
								string.Format(
									"Only half the {0} skills that you've ever become proficient in ({1} skill) are available for selection. As long as you have capacity, selecting a skill will immediately raise it to the displayed value.",
									isPrimary ? "Primary" : "Secondary",
									Constants.RECORDED_SKILL_CAP_MIN_AMOUNT
									),
								y,
								addBackground: false
							);
							break;
						}

					case Categories.FullSkillArchive:
						{
							AddInformationCard(
								BLANK_ITEM_ID,
								"Skill Archive",
								string.Format(
									"Review the highest you've ever reached in each skill. Once you become proficient ({0} skill), you may be able to restore the skill to that value for free.",
									Constants.RECORDED_SKILL_CAP_MIN_AMOUNT
									),
								y,
								addBackground: false
							);
							break;
						}

					case Categories.Items:
						{
							AddInformationCard(BLANK_ITEM_ID, "Items - Purchase Temporary Conveniences", "Items can be purchased to assist you with your next run. Be wary of how much you invest, as these items are lost upon death.", y, addBackground: false);
							break;
						}

					case Categories.Information:
					default:
						break;
				}
			}

			var rewards = RewardFactory.CreateRewards(m_From, selectedCategory, m_Context);
			if (rewards == null || rewards.Count == 0) return;

			if (m_Context.RewardCache == null) m_Context.RewardCache = new Dictionary<Categories, List<int>>();

			List<int> randomRewardIndexes;
			if (!m_Context.RewardCache.TryGetValue(selectedCategory, out randomRewardIndexes))
			{
				randomRewardIndexes = new List<int>();

				switch (selectedCategory)
				{
					case Categories.Ascensions:
					case Categories.FullSkillArchive:
						{
							randomRewardIndexes.AddRange(
								rewards
								.Select((reward, index) => index)
							);
							break;
						}

					case Categories.Templates:
					case Categories.PrimaryBoosts:
					case Categories.SecondaryBoosts:
					case Categories.Items:
						{
							if (rewards.Any(reward => reward.Static))
							{
								randomRewardIndexes.AddRange(
									rewards
									.Where(reward => reward.Static)
									.Select(reward => rewards.FindIndex(r => r == reward))
								);
							}

							if (rewards.Any(reward => !reward.Static))
							{
								var nonStaticRewards = rewards.Where(reward => !reward.Static).ToList();
								var rewardsToPick = nonStaticRewards.Count / 2;
								if (0 < rewardsToPick)
								{
									randomRewardIndexes.AddRange(
										nonStaticRewards
										.OrderBy(r => Utility.RandomMinMax(0, 100))
										.Take(rewardsToPick)
										.Select(reward => rewards.FindIndex(r => r == reward))
									);
								}
							}
							break;
						}

					case Categories.Information:
					default:
						break;
				}

				m_Context.RewardCache[selectedCategory] = randomRewardIndexes;
			}

			if (randomRewardIndexes == null) return;

			var queryable = randomRewardIndexes
				.Select(index => index < rewards.Count ? rewards[index] : null)
				.Where(reward => reward != null);
			if (selectedCategory != Categories.Ascensions && selectedCategory != Categories.FullSkillArchive) queryable = queryable.OrderBy(reward => reward.Name);

			var randomRewards = queryable.ToList();
			if (randomRewards.Count < 1) return;

			const int ITEMS_PER_PAGE = 8;
			var toTake = ITEMS_PER_PAGE;

			var skip = (pageNumber - 1) * toTake;

			// All pages except the Information page have a description card
			var hasInfoCard = selectedCategory != Categories.Information;
			if (hasInfoCard)
			{
				if (pageNumber == 1) toTake -= 1;
				else skip -= 1;
			}

			if (pageNumber == 1)
			{
				y += CARD_HEIGHT;
				y += 10;
			}

			m_Rewards = new List<IReward>();
			var itemIndex = 0;
			foreach (var reward in randomRewards.Skip(skip).Take(toTake))
			{
				m_Rewards.Add(reward);

				var tooltip = reward is ActionReward ? ((ActionReward)reward).PrequisiteTooltip : null;
				var canPurchase = m_InGypsyEncampment && reward.CanSelect && string.IsNullOrWhiteSpace(tooltip);
				var cost = reward is ActionReward && ((ActionReward)reward).IsComplete ? COST_NO_BUY : reward.Cost;

				AddCard(m_Context.PointsSaved, reward.Graphic, reward.Name, reward.Description, canPurchase, cost, itemIndex, y, tooltip);

				y += CARD_HEIGHT;
				y += 10;
				++itemIndex;
			}

			y = GUMP_HEIGHT - 33;

			// Prev page
			if (0 < skip)
				AddButton(NAVIGATION_WIDTH + 20, y, 4014, 4015, (int)_Actions.PageBase + (pageNumber - 1), GumpButtonType.Reply, 0);

			// Next page
			var remain = randomRewards.Count - skip - ITEMS_PER_PAGE;
			if (0 < remain)
				AddButton(GUMP_WIDTH - 47, y, 4005, 4007, (int)_Actions.PageBase + (pageNumber + 1), GumpButtonType.Reply, 0);
		}

		private enum _Actions
		{
			Close = 0,
			SelectCategoryBase = 10,
			PurchaseBase = 50,
			PageBase = 500,
		}

		public static bool InGypsyEncampment(PlayerMobile from)
		{
			return from.Region != null && from.Region.Name == "the Forest";
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

			if ((int)_Actions.PageBase <= buttonID) // Page is higher
			{
				var page = buttonID - (int)_Actions.PageBase;
				sender.Mobile.SendGump(new AvatarShopGump(m_From, m_SelectedCategory, page, m_onGumpClose));
				return;
			}
			else if ((int)_Actions.PurchaseBase <= buttonID) // Purchase is higher
			{
				var index = buttonID - (int)_Actions.PurchaseBase;
				var reward = m_Rewards[index];
				var cost = Math.Max(0, reward.Cost);
				if (m_Context.PointsSaved < cost)
				{
					player.SendMessage("You do not have enough coins to purchase this.");
				}
				else
				{
					var itemReward = reward as ItemReward;
					if (itemReward != null)
					{
						var item = itemReward.OnSelect();
						if (item != null)
						{
							player.SendMessage("You have purchased '{0}' for '{1:n0}' coins.", reward.Name, cost);
							m_Context.PointsSaved -= cost;
							player.AddToBackpack(item);
						}
					}
					else
					{
						var actionReward = reward as ActionReward;
						if (actionReward != null)
						{
							if (m_SelectedCategory == Categories.Templates)
							{
								player.SendMessage("You have selected a template.");
								Timer.DelayCall(TimeSpan.FromSeconds(0.25), () => actionReward.OnSelect());
							}
							else
							{
								player.SendMessage("You have purchased '{0}' for '{1:n0}' coins.", reward.Name, cost);
								m_Context.PointsSaved -= cost;
								actionReward.OnSelect();
								AvatarEngine.Instance.ApplyContext(player, player.Avatar);
							}
						}
					}
				}

				sender.Mobile.SendGump(new AvatarShopGump(m_From, m_SelectedCategory, m_PageNumber, m_onGumpClose));
			}
			else if ((int)_Actions.SelectCategoryBase <= buttonID) // Select Category is higher
			{
				var selectedCategory = (Categories)(buttonID - (int)_Actions.SelectCategoryBase);
				sender.Mobile.SendGump(new AvatarShopGump(m_From, selectedCategory, 1, m_onGumpClose));
				return;
			}
		}

		private void AddCard(
			int points,
			int itemId,
			string name,
			string description,
			bool canPurchase,
			int purchaseCost,
			int index,
			int y,
			string tooltip = null,
			bool scrollable = false,
			bool addBackground = true
			)
		{
			const int START_X = NAVIGATION_WIDTH + 20;
			const int GRAPHIC_SLOT_WIDTH = 73;
			const int GRAPHIC_SLOT_HEIGHT = 68;
			const int PURCHASE_WIDTH = 130;

			int x = START_X;
			int cost = purchaseCost;
			if (cost == COST_NO_BUY) cost = int.MinValue;
			if (cost == COST_FREE) cost = 0;

			if (addBackground)
				AddBackground(x, y, CARD_WIDTH, CARD_HEIGHT + 5, 2620);

			// Item image
			if (itemId > BLANK_ITEM_ID)
			{
				AddBackground(x, y, GRAPHIC_SLOT_WIDTH, CARD_HEIGHT + 5, 2620);
				if (itemId > NO_ITEM_ID)
					GumpUtilities.AddCenteredItemToGump(this, itemId, x, y, GRAPHIC_SLOT_WIDTH, GRAPHIC_SLOT_HEIGHT);
				x += GRAPHIC_SLOT_WIDTH;
			}

			// Item text
			x += 10;
			y += 5; // Top padding

			int DESCRIPTION_WIDTH = CARD_WIDTH - (x - START_X);
			if (0 <= cost) DESCRIPTION_WIDTH -= PURCHASE_WIDTH;

			const int LAZY_AMOUNT = 30; // Arbitrary value to account for left padding
			TextDefinition.AddHtmlText(this, x, y, DESCRIPTION_WIDTH - LAZY_AMOUNT, 20, name, HtmlColors.ORANGE);
			TextDefinition.AddHtmlText(this, x + 10, y + 20, DESCRIPTION_WIDTH - LAZY_AMOUNT, 40, TextDefinition.GetColorizedText(description, HtmlColors.COOL_BLUE), false, scrollable);

			if (0 <= cost)
			{
				x = START_X + CARD_WIDTH - PURCHASE_WIDTH;
				y += 6;

				// Purchase section
				const int GRAPHIC_WIDTH = 55;
				var canAfford = cost <= points;
				if (0 < cost)
				{
					var pointsColor = canAfford ? HtmlColors.ORANGE : HtmlColors.RED;
					TextDefinition.AddHtmlText(this, x + GRAPHIC_WIDTH, y + 2, 80, 20, cost.ToString("n0"), pointsColor);
				}

				if (canAfford)
				{
					y += 30;

					if (canPurchase)
						AddButton(x + 13, y - 1, 4023, 4023, (int)_Actions.PurchaseBase + index, GumpButtonType.Reply, 0); // OK
					else
					{
						AddImage(x + 21, y + 3, 2092); // Lock icon
						if (!string.IsNullOrWhiteSpace(tooltip))
							AddTooltip(tooltip);
					}

					string purchaseText;
					switch (m_SelectedCategory)
					{
						case Categories.Templates:
							purchaseText = "Select";
							break;

						case Categories.PrimaryBoosts:
						case Categories.SecondaryBoosts:
							purchaseText = "Teach Me";
							break;

						case Categories.Ascensions:
							purchaseText = "Unlock";
							break;

						case Categories.Items:
						case Categories.Information:
						default:
							purchaseText = "Purchase";
							break;
					}

					TextDefinition.AddHtmlText(this, x + GRAPHIC_WIDTH, y + 2, 60, 20, purchaseText, HtmlColors.ORANGE);
				}
			}
		}

		private void AddCategoryList(Categories selectedCategory, int x, int y)
		{
			// Show current coins
			{
				var firstRowY = y + 20 - 3;
				var secondRowY = firstRowY + 20;

				AddBackground(x, y, CATEGORY_WIDTH, CARD_HEIGHT + 5, 2620);
				GumpUtilities.AddCenteredItemToGump(this, GIANT_COIN_ITEM_ID, x + 10, y, 40, CARD_HEIGHT + 5);
				AddTooltip("Coins are earned by killing monsters and completing combat-based quests.");
				TextDefinition.AddHtmlText(this, x + 60, firstRowY, CATEGORY_WIDTH - 20, 40, string.Format("{0}", (m_From.Avatar.PointsSaved + m_From.Avatar.PointsFarmed).ToString("n0")), HtmlColors.ORANGE);
				TextDefinition.AddHtmlText(this, x + 60, secondRowY, CATEGORY_WIDTH - 20, 40, "Coins", HtmlColors.COOL_BLUE);
				y += CARD_HEIGHT + 10;
			}

			int i = 0;
			foreach (var category in m_Categories)
			{
				var isSelected = selectedCategory == category;
				const int CATEGORY_CARD_HEIGHT = 34;
				const int TOP_PADDING = 6;
				const int HEIGHT_PER_ITEM = CATEGORY_CARD_HEIGHT + TOP_PADDING; // Extra top padding
				const int LEFT_PADDING = 36;

				if (!isSelected)
				{
					const int HIDDEN_BUTTON_ID = 1150;
					const int HIDDEN_BUTTON_WIDTH = 28;

					int buttonId = (int)_Actions.SelectCategoryBase + (int)category;
					AddButton(x, y + TOP_PADDING + (i * HEIGHT_PER_ITEM), HIDDEN_BUTTON_ID, HIDDEN_BUTTON_ID, buttonId, GumpButtonType.Reply, 0);
					AddButton(x + HIDDEN_BUTTON_WIDTH, y + TOP_PADDING + (i * HEIGHT_PER_ITEM), HIDDEN_BUTTON_ID, HIDDEN_BUTTON_ID, buttonId, GumpButtonType.Reply, 0);
					AddButton(x + HIDDEN_BUTTON_WIDTH + HIDDEN_BUTTON_WIDTH, y + TOP_PADDING + (i * HEIGHT_PER_ITEM), HIDDEN_BUTTON_ID, HIDDEN_BUTTON_ID, buttonId, GumpButtonType.Reply, 0);
				}

				AddBackground(x, y + (i * HEIGHT_PER_ITEM), CATEGORY_WIDTH, HEIGHT_PER_ITEM - 6, 2620);

				var gemGraphic = isSelected ? 1210 : 1209;
				var gemHue = isSelected ? 1152 : 0;
				AddImage(x + 17, y + 10 + (i * HEIGHT_PER_ITEM), gemGraphic, gemHue);

				var color = isSelected ? HtmlColors.ORANGE : HtmlColors.COOL_BLUE;

				string categoryName;
				switch (category)
				{
					case Categories.PrimaryBoosts:
						categoryName = "Primary Skills";
						break;

					case Categories.SecondaryBoosts:
						categoryName = "Secondary Skills";
						break;

					case Categories.FullSkillArchive:
						categoryName = "Skill Archive";
						break;

					case Categories.Information:
					case Categories.Ascensions:
					case Categories.Templates:
					case Categories.Items:
					default:
						categoryName = category.ToString();
						break;
				}

				TextDefinition.AddHtmlText(this, x + LEFT_PADDING, y + 7 + (i * HEIGHT_PER_ITEM), CATEGORY_WIDTH - LEFT_PADDING, 16, categoryName, color);
				++i;
			}
		}

		private void AddInformationCard(
			int itemId,
			string name,
			string description,
			int y,
			string tooltip = null,
			bool scrollable = false,
			bool addBackground = true
		)
		{
			AddCard(0, itemId, name, description, false, 0, 0, y, tooltip, scrollable, addBackground);
		}

		private void AddKeyValuePairsCard(int x, int y)
		{
			AddBackground(x, y, CARD_WIDTH, CARD_HEIGHT + 5, 2620);

			const int COUNT = 3;
			const int WIDTH_AVAILABLE = CARD_WIDTH;
			var cardWidth = Math.Min(100, WIDTH_AVAILABLE / COUNT);

			var space = cardWidth + (int)((double)cardWidth / (COUNT - 1));
			x += (WIDTH_AVAILABLE - (space * (COUNT - 1)) - cardWidth) / 2; // Split any excess due to card width limits

			var firstRowY = y + 20;
			var secondRowY = firstRowY + 20;

			TextDefinition.AddHtmlText(this, x, firstRowY, cardWidth, 40, string.Format("<CENTER>{0}</CENTER>", m_From.StatCap), HtmlColors.ORANGE);
			TextDefinition.AddHtmlText(this, x, secondRowY, cardWidth, 40, "<CENTER>Stat Cap</CENTER>", HtmlColors.COOL_BLUE);

			x += space;
			TextDefinition.AddHtmlText(this, x, firstRowY, cardWidth, 40, string.Format("<CENTER>{0}</CENTER>", (m_From.SkillsCap / 10).ToString("n0")), HtmlColors.ORANGE);
			TextDefinition.AddHtmlText(this, x, secondRowY, cardWidth, 40, "<CENTER>Skill Cap</CENTER>", HtmlColors.COOL_BLUE);

			x += space;
			TextDefinition.AddHtmlText(this, x, firstRowY, cardWidth, 40, string.Format("<CENTER>{0}</CENTER>", Math.Min(Constants.RIVAL_BONUS_MAX_POINTS, m_Context.RivalBonusPoints).ToString("n0")), HtmlColors.ORANGE);
			TextDefinition.AddHtmlText(this, x, secondRowY, cardWidth, 40, "<CENTER>Faction Bonus</CENTER>", HtmlColors.COOL_BLUE);
			AddTooltip("The amount of coins you have received for killing your enemy faction.");
		}
	}
}