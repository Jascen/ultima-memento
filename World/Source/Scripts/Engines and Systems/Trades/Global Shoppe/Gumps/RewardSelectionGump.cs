using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Utilities;

namespace Server.Engines.GlobalShoppe
{
    public class RewardSelectionGump : Gump
    {
        public enum RewardType
        {
            None = 0,
            Gold = 1,
            Points = 2,
            Reputation = 3
        }

        private enum Actions
        {
            Close = 0,
            SelectGold = 1,
            SelectPoints = 2,
            SelectReputation = 3,
            Claim = 10
        }

        private readonly Mobile m_From;
        private readonly ShoppeBase m_Shoppe;
        private readonly TradeSkillContext m_Context;
        private readonly IOrderContext m_Order;
        private readonly int m_OrderIndex;
        private readonly string m_Title;
        private readonly string m_ToolName;
        private readonly string m_ResourceName;
        private RewardType m_SelectedReward;

        public RewardSelectionGump(
            Mobile from,
            ShoppeBase shoppe,
            TradeSkillContext context,
            IOrderContext order,
            int orderIndex,
            string title,
            string toolName,
            string resourceName,
            RewardType selectedReward = RewardType.None
            ) : base(100, 100)
        {
            m_From = from;
            m_Shoppe = shoppe;
            m_Context = context;
            m_Order = order;
            m_OrderIndex = orderIndex;
            m_Title = title;
            m_ToolName = toolName;
            m_ResourceName = resourceName;
            m_SelectedReward = selectedReward;

            AddPage(0);

            AddBackground(0, 0, 400, 300, 0x1453);
            AddImageTiled(8, 8, 384, 284, 2624);
            AddAlphaRegion(8, 8, 384, 284);

            TextDefinition.AddHtmlText(this, 20, 20, 360, 25, "<CENTER>Order Completed</CENTER>", HtmlColors.MUSTARD);
            
            TextDefinition.AddHtmlText(this, 20, 50, 360, 40, 
                string.Format("<CENTER>You have successfully completed the {0} for {1}.</CENTER>",
                ((IOrderShoppe)m_Shoppe).GetDescription(order).Replace("Craft ", ""), order.Person), HtmlColors.BROWN);

            int y = 120;
            int boxWidth = 100;
            int totalWidth = boxWidth * 3;
            int startX = (400 - totalWidth) / 2;

            AddRewardOption(Actions.SelectReputation, RewardType.Reputation, startX + (boxWidth * 2) + (boxWidth / 2), y, 10283, m_Order.ReputationReward.ToString(), "Reputation");
            AddRewardOption(Actions.SelectGold, RewardType.Gold, startX + (boxWidth / 2), y, 3823, m_Order.GoldReward.ToString(), "Gold");
            AddRewardOption(Actions.SelectPoints, RewardType.Points, startX + boxWidth + (boxWidth / 2), y, 0x0EEC, m_Order.PointReward.ToString(), "Points");

            int buttonY = 250;
            int claimButtonX = 200;
            int cancelButtonX = 200 - 127;

            if (m_SelectedReward != RewardType.None)
            {
                AddButton(claimButtonX, buttonY, 4023, 4023, (int)Actions.Claim, GumpButtonType.Reply, 0);
                TextDefinition.AddHtmlText(this, claimButtonX + 35, buttonY + 3, 100, 20, "Claim Your Fee", HtmlColors.MUSTARD);
            }
            else
            {
                AddImage(claimButtonX, buttonY, 4020);
                TextDefinition.AddHtmlText(this, claimButtonX + 35, buttonY + 3, 100, 20, "Claim Your Fee", HtmlColors.GRAY);
            }

            AddButton(cancelButtonX, buttonY, 4020, 4020, (int)Actions.Close, GumpButtonType.Reply, 0);
            TextDefinition.AddHtmlText(this, cancelButtonX + 35, buttonY + 3, 60, 20, "Cancel", HtmlColors.MUSTARD);
        }

        private void AddRewardOption(Actions action, RewardType rewardType, int x, int y, int itemId, string amount, string label)
        {
            bool isSelected = m_SelectedReward == rewardType;
            
            int boxWidth = 80;
            int boxHeight = 100;
            int boxX = x - (boxWidth / 2);
            int boxY = y - 15;

            for (int tileY = 0; tileY < 4; tileY++)
            {
                for (int tileX = 0; tileX < 3; tileX++)
                {
                    int tilePosX = boxX + 2 + (tileX * 23);
                    int tilePosY = boxY + 2 + (tileY * 22);
                    AddButton(tilePosX, tilePosY, 0x9C, 0x9C, (int)action, GumpButtonType.Reply, 0);
                }
            }

            AddBackground(boxX, boxY, boxWidth, boxHeight, 0xA3C);

            int iconX = x - 22;
            int iconY = y + 5;

            //Reputation icon is 21 wide, so needs a custom offset.
            if (itemId == 10283)
            {
                iconX = x - 11;
            }

            if (itemId == 0x0EEC)
            {
                AddItem(iconX, iconY, itemId, 0x44C); // 1072
            }
            else
            {
                AddItem(iconX, iconY, itemId);
            }
            
            TextDefinition.AddHtmlText(this, boxX, iconY + 35, boxWidth, 20, 
                string.Format("<CENTER>{0}</CENTER>", amount), 
                isSelected ? HtmlColors.WHITE : HtmlColors.MUSTARD);
            
            TextDefinition.AddHtmlText(this, boxX, iconY + 55, boxWidth, 20, 
                string.Format("<CENTER>{0}</CENTER>", label), 
                isSelected ? HtmlColors.WHITE : HtmlColors.BROWN);


            //Checkbox must be reversed if isSelected
            if (isSelected)
            {
                AddButton(x - 15, iconY + 85, 0xFB1, 0xE19, (int)action, GumpButtonType.Reply, 0);
            }
            else
            {
                AddButton(x - 15, iconY + 85, 0xE19, 0xFB1, (int)action, GumpButtonType.Reply, 0);
            }

        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var buttonID = info.ButtonID;
            
            switch ((Actions)buttonID)
            {
                case Actions.Close:
                    sender.Mobile.SendGump(new ShoppeGump(
                        (PlayerMobile)m_From,
                        m_Shoppe,
                        m_Context,
                        m_Title,
                        m_ToolName,
                        m_ResourceName
                    ));
                    break;

                case Actions.SelectGold:
                case Actions.SelectPoints:
                case Actions.SelectReputation:
                    RewardType newSelection = (RewardType)buttonID;
                    sender.Mobile.SendGump(new RewardSelectionGump(
                        m_From,
                        m_Shoppe,
                        m_Context,
                        m_Order,
                        m_OrderIndex,
                        m_Title,
                        m_ToolName,
                        m_ResourceName,
                        newSelection
                    ));
                    break;

                case Actions.Claim:
                    if (m_SelectedReward != RewardType.None)
                    {
                        CompleteOrderWithSelectedReward();
                        sender.Mobile.SendGump(new ShoppeGump(
                            (PlayerMobile)m_From,
                            m_Shoppe,
                            m_Context,
                            m_Title,
                            m_ToolName,
                            m_ResourceName
                        ));
                    }
                    break;
            }
        }

        private void CompleteOrderWithSelectedReward()
        {
            switch (m_SelectedReward)
            {
                case RewardType.Gold:
                    m_Context.Gold += m_Order.GoldReward;
                    break;
                case RewardType.Points:
                    m_Context.Points += m_Order.PointReward;
                    break;
                case RewardType.Reputation:
                    m_Context.Reputation = System.Math.Min(ShoppeConstants.MAX_REPUTATION, m_Context.Reputation + m_Order.ReputationReward);
                    break;
            }

            SkillUtilities.DoSkillChecks(m_From, SkillName.Mercantile, 3);
            m_Context.Orders.Remove(m_Order);

            m_From.PlaySound(0x32); // Dropgem1
        }
    }
}