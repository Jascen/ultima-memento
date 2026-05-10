using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Items;
using System.Linq;
using Server.Mobiles;

namespace Server.Engines.Craft
{
	public class CraftItemSkillListGump : Gump
	{
		private const int PAGE_BUTTON_OFFSET = 100;
		private const int SHOW_ITEM_INFO_OFFSET = 10000;

		private readonly Mobile m_From;
		private readonly CraftSystem m_CraftSystem;
		private readonly BaseTool m_Tool;
		private readonly List<CraftItem> m_CraftItems;
		private readonly int m_PageNumber;

		public CraftItemSkillListGump(Mobile from, CraftSystem craftSystem, BaseTool tool, List<CraftItem> craftItems, int pageNumber) : base(572, 40)
		{
			m_From = from;
			m_CraftSystem = craftSystem;
			m_Tool = tool;
			m_CraftItems = craftItems;
			m_PageNumber = pageNumber;

			const int HORIZONTAL_LINE = 2700;
			const int BORDER_WIDTH = 2;
			const int ITEM_HEIGHT = 30;
			const int INFO_WINDOW_WIDTH = 300;
			AddImageTiled(0, 0, INFO_WINDOW_WIDTH, 400, 2702);

			const int LOCK_COLUMN_X = 5;
			const int NAME_COLUMN_X = LOCK_COLUMN_X + 33;
			const int SKILL_COLUMN_WIDTH = NAME_COLUMN_X + 30;
			const int SKILL_COLUMN_X = INFO_WINDOW_WIDTH - SKILL_COLUMN_WIDTH;
			const int ITEM_START_Y = 2 * ITEM_HEIGHT;

			TextDefinition.AddHtmlText(this, LOCK_COLUMN_X, 30, INFO_WINDOW_WIDTH, 20, "Craft these items for skill gains", false, false, HtmlColors.OFFWHITE, HtmlColors.OFFWHITE);
			TextDefinition.AddHtmlText(this, SKILL_COLUMN_X, 10, SKILL_COLUMN_WIDTH, 40, "Max Skill", false, false, HtmlColors.OFFWHITE, HtmlColors.OFFWHITE);

			const int ITEMS_PER_PAGE = 10;
			var maxPages = (int)Math.Ceiling((double)m_CraftItems.Count / ITEMS_PER_PAGE);
			int lineIndex = 0;
			var toSkip = (pageNumber - 1) * ITEMS_PER_PAGE;
			foreach (var craftItem in m_CraftItems.Skip(toSkip).Take(ITEMS_PER_PAGE))
			{
				for (int k = 0; k < craftItem.Skills.Count; k++)
				{
					CraftSkill skill = craftItem.Skills.GetAt(k);
					if (skill.SkillToMake == craftSystem.MainSkill)
					{
						var y = ITEM_START_Y + (lineIndex * ITEM_HEIGHT);
						// Add border above item
						if (0 < lineIndex)  // Skip the first
							AddImageTiled(10, y - 7, INFO_WINDOW_WIDTH - 15, BORDER_WIDTH, HORIZONTAL_LINE);

						var name = craftItem.NameString == null ? CliLocTable.Lookup(craftItem.NameNumber) : craftItem.NameString;
						var hasRecipe = craftItem.Recipe == null || (from is PlayerMobile && ((PlayerMobile)from).HasRecipe( craftItem.Recipe ));

						if (!hasRecipe)
						{
							AddImage(LOCK_COLUMN_X + 8, y + 3, 2092); // Lock icon
							AddTooltip("You don't know this recipe");
						}
						else
						{
							AddButton(LOCK_COLUMN_X, y - 2, 4011, 4012, SHOW_ITEM_INFO_OFFSET + toSkip + lineIndex, GumpButtonType.Reply, 0); // Info icon
						}

						TextDefinition.AddHtmlText(this, NAME_COLUMN_X, y, SKILL_COLUMN_X - 20, 20, name, false, false, HtmlColors.OFFWHITE, HtmlColors.OFFWHITE);
						TextDefinition.AddHtmlText(this, SKILL_COLUMN_X, y, SKILL_COLUMN_WIDTH, 20, String.Format("{0:F1}", skill.MaxSkill), false, false, HtmlColors.OFFWHITE, HtmlColors.OFFWHITE);

						lineIndex++;
						break;
					}
				}
			}

			// Add page buttons
			if (maxPages != 1)
			{
				AddButton(NAME_COLUMN_X, 365, 4014, 4015, PAGE_BUTTON_OFFSET + (pageNumber == 1 ? maxPages : pageNumber - 1), GumpButtonType.Reply, 0); // Previous
				AddButton(SKILL_COLUMN_X, 365, 4005, 4006, PAGE_BUTTON_OFFSET + (pageNumber == maxPages ? 1 : pageNumber + 1), GumpButtonType.Reply, 0); // Next
			}
		}

		public CraftItemSkillListGump(Mobile from, CraftSystem craftSystem, BaseTool tool) : this(from, craftSystem, tool, CreateItemList(from, craftSystem), 1)
		{
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID == 0) return;

			if (SHOW_ITEM_INFO_OFFSET <= info.ButtonID)
			{
				var itemIndex = info.ButtonID - SHOW_ITEM_INFO_OFFSET;
				var item = m_CraftItems[itemIndex];

				sender.Mobile.SendGump(new CraftItemSkillListGump(m_From, m_CraftSystem, m_Tool, m_CraftItems, m_PageNumber));
				var itemInfoGump = new CraftGumpItem(sender.Mobile, m_CraftSystem, item, m_Tool);
				sender.Mobile.SendGump(itemInfoGump);
			}
			else if (PAGE_BUTTON_OFFSET <= info.ButtonID)
			{
				var pageNumber = info.ButtonID - PAGE_BUTTON_OFFSET;
				sender.Mobile.SendGump(new CraftItemSkillListGump(m_From, m_CraftSystem, m_Tool, m_CraftItems, pageNumber));
			}
		}

		public static List<CraftItem> CreateItemList(Mobile from, CraftSystem craftSystem)
		{
			var craftItems = new List<CraftItem>();
			for (int i = 0; i < craftSystem.CraftGroups.Count; i++)
			{
				var group = craftSystem.CraftGroups.GetAt(i);

				for (int j = 0; j < group.CraftItems.Count; j++)
				{
					var craftItem = group.CraftItems.GetAt(j);

					bool hasSkills = true;
					for (int k = 0; k < craftItem.Skills.Count; k++)
					{
						CraftSkill skill = craftItem.Skills.GetAt(k);
						if (
							from.Skills[skill.SkillToMake].Value < skill.MinSkill // Filter items you can't succeed on
							|| skill.MaxSkill <= from.Skills[skill.SkillToMake].Value // Filter items you can longer gain from
						)
						{
							hasSkills = false;
							break;
						}
					}

					if (hasSkills)
						craftItems.Add(craftItem);
				}
			}

			// Sort by max skill descending
			return craftItems.OrderByDescending(craftItem =>
			{
				for (int k = 0; k < craftItem.Skills.Count; k++)
				{
					CraftSkill skill = craftItem.Skills.GetAt(k);
					if (skill.SkillToMake == craftSystem.MainSkill) return skill.MaxSkill;
				}

				return 0;
			}).ToList();
		}
	}
}