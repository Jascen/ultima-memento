using Server.Gumps;
using System.Collections.Generic;
using Server.Network;
using System.Linq;
using System;
using Server;
using Server.Utilities;

namespace Scripts.Mythik.Systems.Achievements.Gumps
{
    class AchievementGump : Gump
    {
        public const int COLOR_HTML = 0xf7fbde; // RGB888
        public const int COLOR_LABEL = 1918; // Hue from files
        public const int COLOR_LOCALIZED = 0xf7db; // RGB565

        private const int PAGE_BUTTON_OFFSET = 1000;
        private const int CATEGORY_BUTTON_OFFSET = 5000;

        private readonly int m_curTotal;
        private int m_Category;
        private readonly Dictionary<int, AchieveData> m_curAchieves;
        private readonly Dictionary<int, AchieveData> m_viewerAchieves;
        private readonly string m_targetName;
        private readonly bool m_isOtherView;

        public AchievementGump(Dictionary<int, AchieveData> achieves, int total, int pageNumber = 1, int selectedCategoryId = -1, Dictionary<int, AchieveData> viewerAchieves = null, string targetName = null) : base(25, 25)
        {
            m_curAchieves = achieves;
            m_curTotal = total;
            m_Category = selectedCategoryId;
            m_viewerAchieves = viewerAchieves;
            m_targetName = targetName;
            m_isOtherView = viewerAchieves != null;
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 1053, 605, 3600); // Border
            AddImage(15, 14, 13000); // UO background
            AddImage(419, 23, 13001); // Achievements
            
            if (m_isOtherView)
            {
                TextDefinition.AddHtmlText(this, 262, 55, 500, 20, string.Format("<CENTER>{0}'s Feats</CENTER>", m_targetName), false, false, COLOR_LOCALIZED, COLOR_HTML);
            }
            
            TextDefinition.AddHtmlText(this, 850, 25, 180, 20, string.Format("<RIGHT>Achievement Points: {0}</RIGHT>", m_curTotal), false, false, COLOR_LOCALIZED, COLOR_HTML);

            var selectedCategory = AchievementSystem.Categories.FirstOrDefault(c => c.ID == selectedCategoryId);
            var categories = selectedCategory != null
                ? AchievementSystem.Categories.Where(cat =>
                    cat == selectedCategory // Match
                    || cat.ID == selectedCategory.Parent // Parent of selected category
                    || (0 < cat.Parent && cat.Parent == selectedCategory.ID) // Child of selected category
                    || (0 < cat.Parent && cat.Parent == selectedCategory.Parent) // Sibling of selected category
                )
                : AchievementSystem.Categories.Where(cat => cat.Parent == 0);
            var categoryList = categories.ToList();

            var addBackButton = selectedCategory != null; // TODO: Add visual
            if (addBackButton)
            {
                AddButton(27, 39, 4014, 4016, 1, GumpButtonType.Reply, 0);
                AddLabel(60, 42, 1153, "All Categories");
            }

            int i = 0;
            for (int index = 0; index < categoryList.Count; index++)
            {
                int x = 27;
                var category = categoryList[index];

                const int CATEGORY_WIDTH = 209;
                if (category.Parent == 0)
                {
					AddCategoryButton(x, 78 + (i * 56), CATEGORY_WIDTH, category.ID, selectedCategoryId);
                }
                else
                {
                    x += 20;
					AddCategoryButton(x, 78 + (i * 56), CATEGORY_WIDTH - 40, category.ID, selectedCategoryId);
                }

                TextDefinition.AddHtmlText(this, x + 37, 93 + (i * 56), CATEGORY_WIDTH, 16, category.Name, false, false, COLOR_LOCALIZED, COLOR_HTML);
                ++i;
            }

            const int ITEMS_PER_PAGE = 6;
            var achievements = selectedCategory != null
                ? AchievementSystem.Achievements.Where(ac => ac.CategoryID == selectedCategoryId
                    && (
                        !ac.HiddenTillComplete
                        || (achieves.ContainsKey(ac.ID) && achieves[ac.ID].IsComplete)
                    )
                    && (
                        ac.PreReq == null // No Pre-req
                        || (achieves.ContainsKey(ac.PreReq.ID) && achieves[ac.PreReq.ID].IsComplete) // Pre-req is complete
                    )
                )
                : AchievementSystem.Achievements
                    .Where(ac => achieves.ContainsKey(ac.ID) && achieves[ac.ID].IsComplete)
                    .OrderByDescending(ac => achieves[ac.ID].CompletedOn);

            if (m_isOtherView)
                achievements = achievements.Where(ac => achieves.ContainsKey(ac.ID) && achieves[ac.ID].IsComplete);

            var maxPages = (int)Math.Ceiling((double)achievements.Count() / ITEMS_PER_PAGE);
            int itemIndex = 0;
            foreach (var achievement in achievements.Skip((pageNumber - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE))
            {
                AchieveData data = null;
                achieves.TryGetValue(achievement.ID, out data);
                AddAchieve(achievement, itemIndex++, ITEMS_PER_PAGE, data);
            }

            // Add page buttons
            if (maxPages != 1)
            {
                AddButton(282, 549, 4014, 4015, PAGE_BUTTON_OFFSET + (pageNumber == 1 ? maxPages : pageNumber - 1), GumpButtonType.Reply, 0); // Previous
                AddLabel(615, 551, COLOR_LABEL, "Page " + pageNumber);
                AddButton(974, 549, 4005, 4006, PAGE_BUTTON_OFFSET + (pageNumber == maxPages ? 1 : pageNumber + 1), GumpButtonType.Reply, 0); // Next
            }
        }

        private void AddAchieve(BaseAchievement ac, int i, int itemsPerPage, AchieveData acheiveData)
        {
            const int CARD_HEIGHT = 68;
            const int CARD_Y_OFFSET = 10;
            const int CARD_GAP = 11;
            const int HEIGHT_PER_CARD = CARD_HEIGHT + CARD_GAP;

            int index = i % itemsPerPage; // Item index

            var isComplete = acheiveData != null && acheiveData.IsComplete;

            var title = isComplete || !ac.HideTitle ? ac.Title : "???";
            string description;
            if (m_isOtherView)
            {
                bool viewerHasAchievement = m_viewerAchieves != null && 
                                          m_viewerAchieves.ContainsKey(ac.ID) && 
                                          m_viewerAchieves[ac.ID].IsComplete;
                
                description = viewerHasAchievement && isComplete ? ac.Desc : "";

            }
            else
            {
                description = isComplete || !ac.HideDesc ? ac.Desc : ac.HiddenDesc;
            }
            
            AddBackground(277, CARD_HEIGHT + CARD_Y_OFFSET + (index * HEIGHT_PER_CARD), 727, 73, 3600);
            AddLabel(350, 15 + CARD_HEIGHT + CARD_Y_OFFSET + (index * HEIGHT_PER_CARD), 49, title);
            if (ac.ItemIcon > 0)
            {
                int y = 35 + CARD_HEIGHT + CARD_Y_OFFSET + (index * HEIGHT_PER_CARD);
				GumpUtilities.AddCenteredItemToGump(this, ac.ItemIcon, 321, y);
            }

            if (!isComplete && 1 < ac.CompletionTotal)
            {
                var progress = acheiveData != null ? acheiveData.Progress : 0;
                AddImageTiled(890, 84 + CARD_Y_OFFSET + (index * HEIGHT_PER_CARD), 95, 9, 9750); // Gray progress

                if (acheiveData != null)
                {
                    var step = 95.0 / ac.CompletionTotal;

                    if (0 < progress)
                        AddImageTiled(890, 84 + CARD_Y_OFFSET + (index * HEIGHT_PER_CARD), (int)(progress * step), 9, 9752); // Green progress
                }

                TextDefinition.AddHtmlText(this, 806, 23 + CARD_HEIGHT + CARD_Y_OFFSET + (index * HEIGHT_PER_CARD), 185, 16, string.Format("<RIGHT>{0} / {1}</RIGHT>", progress, ac.CompletionTotal), false, false, COLOR_LOCALIZED, COLOR_HTML);
            }

            if (!string.IsNullOrEmpty(description))
                TextDefinition.AddHtmlText(this, 355, 34 + CARD_HEIGHT + CARD_Y_OFFSET + (index * HEIGHT_PER_CARD), 613, 16, description, false, false, COLOR_LOCALIZED, COLOR_HTML);

            if (acheiveData != null && acheiveData.IsComplete)
                TextDefinition.AddHtmlText(this, 806, 12 + CARD_HEIGHT + CARD_Y_OFFSET + (index * HEIGHT_PER_CARD), 185, 16, string.Format("<RIGHT>Completed {0}</RIGHT>", acheiveData.CompletedOn.ToShortDateString()), false, false, COLOR_LOCALIZED, 0x148506);
        }

		private void AddCategoryButton(int x, int y, int width, int categoryID, int selectedCategoryId)
		{
			const int bgID = 3600;
			const int BUTTON_WIDTH = 29;
			const int BUTTON_GRAPHIC = 2151;

			if (categoryID != selectedCategoryId)
			{
				for (var i = 0; x + (i * BUTTON_WIDTH) <= width; i++)
				{
					AddButton(x + (i * BUTTON_WIDTH), y + 10, BUTTON_GRAPHIC, BUTTON_GRAPHIC, 5000 + categoryID, GumpButtonType.Reply, 0);
				}
			}

			AddBackground(x, y, width, 50, bgID);

			if (categoryID == selectedCategoryId)
			{
				AddImage(x + 17, y + 18, 1210, 1152);
			}
			else
			{
				AddImage(x + 17, y + 18, 1210);
			}
		}

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0) return;

            switch (info.ButtonID)
            {
                case 0: // Close
                    return;

                case 1: // All Categories
                    sender.Mobile.SendGump(new AchievementGump(m_curAchieves, m_curTotal, 1, -1, m_viewerAchieves, m_targetName));
                    break;

                default:
                    if (CATEGORY_BUTTON_OFFSET <= info.ButtonID)
                    {
                        var category = info.ButtonID - CATEGORY_BUTTON_OFFSET;
                        sender.Mobile.SendGump(new AchievementGump(m_curAchieves, m_curTotal, 1, category, m_viewerAchieves, m_targetName));
                    }
                    else if (PAGE_BUTTON_OFFSET <= info.ButtonID)
                    {
                        var pageNumber = info.ButtonID - PAGE_BUTTON_OFFSET;
                        sender.Mobile.SendGump(new AchievementGump(m_curAchieves, m_curTotal, pageNumber, m_Category, m_viewerAchieves, m_targetName));
                    }

                    break;
            }
        }
    }
}

