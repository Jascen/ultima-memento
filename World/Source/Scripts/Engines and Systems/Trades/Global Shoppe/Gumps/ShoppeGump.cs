using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.GlobalShoppe
{
    public class ShoppeGump : Gump
    {
        private enum Actions
        {
            Close = 0,
            Help = 1,
            AcceptBase = 50,
            RejectBase = 100,
            DoOrderBase = 150,
            CompleteOrderBase = 200,
            RejectOrderBase = 250,
        }

        private const int CARD_HEIGHT = 68;
        private const int CARD_WIDTH = 864;

        private readonly TradeSkillContext m_Context;
        private readonly PlayerMobile m_From;
        private readonly string m_ResourceName;
        private readonly ShoppeBase m_Shoppe;
        private readonly string m_Title;
        private readonly string m_ToolName;

        public ShoppeGump(
            PlayerMobile from,
            ShoppeBase shoppe,
            TradeSkillContext context,
            string title,
            string toolName,
            string resourceName,
            bool showHelp = false
            ) : base(25, 25)
        {
            m_From = from;
            m_Shoppe = shoppe;
            m_Context = context;
            m_Title = title;
            m_ToolName = toolName;
            m_ResourceName = resourceName;

            AddPage(0);

            AddImage(0, 0, 7028, Server.Misc.PlayerSettings.GetGumpHue(from));

            if (showHelp)
            {
                var mercrate = MySettings.S_MerchantCrates
                    ? "<br><br>If you want to earn more gold from your home, see the local provisioner and see if you can buy a merchant crate. These crates allow you to craft items, place them in the crate, and the Merchants Guild will pick up your wares after a set period of time. If you decide you want something back from the crate, make sure to take it out before the guild shows up."
                    : "";

                TextDefinition.AddHtmlText(this, 11, 11, 532, 20, "ABOUT SHOPPES", HtmlColors.BROWN);
                TextDefinition.AddHtmlText(this,
                    13, 44, 879, 360,
                    "The world is filled with opportunity, where adventurers seek the help of other in order to achieve their goals. With filled coin purses, they seek experts in various crafts to acquire their skills. Some would need armor repaired, maps deciphered, potions concocted, scrolls translated, clothing fixed, or many other things. The merchants, in the cities and villages, often cannot keep up with the demand of these requests. This provides opportunity for those that practice a trade and have their own home from which to conduct business. Seek out a tradesman and see if they have an option for you to have them build you a Shoppe of your own. These Shoppes usually demand you to part with 10,000 gold, but they can quickly pay for themselves if you are good at your craft. You may only have one type of each Shoppe at any given time. So if you are skilled in two different types of crafts, then you can have a Shoppe for each. You will be the only one to use the Shoppe, but you may give permission to others to transfer the gold out into a bank check for themselves. Shoppes require to be stocked with tools and resources, and the Shoppe will indicate what those are at the bottom. Simply drop such things onto your Shoppe to amass an inventory. When you drop tools onto your Shoppe, the number of tool uses will add to the Shoppe's tool count. A Shoppe may only hold 1,000 tools and 5,000 resources. After a set period of time, customers will make requests of you which you can fulfill or refuse. Each request will display the task, who it is for, the amount of tools needed, the amount of resources required, your chance to fulfill the request (based on the difficulty and your skill), and the amount of reputation your Shoppe will acquire if you are successful.<br><br>If you fail to perform a selected task, or refuse to do it, your Shoppe's reputation will drop by that same value you would have been rewarded with. Word of mouth travels fast in the land and you will have less prestigious work if your reputation is low. If you find yourself reaching the lows of becoming a murderer, your Shoppe will be useless as no one deals with murderers. Any gold earned will stay within the Shoppe until you single click the Shoppe and Transfer the funds out of it. Your Shoppe can have no more than 500,000 gold at a time, and you will not be able to conduct any more business in it until you withdraw the funds so it can amass more. The reputation for the Shoppe cannot go below 0, and it cannot go higher than 10,000. Again, the higher the reputation, the more lucrative work you will be asked to do. If you are a member of the associated crafting guild, your reputation will have a bonus toward it based on your crafting skill."
                    + mercrate,
                    false, true, HtmlColors.BROWN, HtmlColors.BROWN
                );
                AddButton(864, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

                int y = 420;

                AddItem(113, y, 10283); // Goblet
                TextDefinition.AddHtmlText(this, 153, y, 717, 20, "Shows your shoppe's tool count at the top, or tools needed for each individual contract.", HtmlColors.MUSTARD);
                y += 35;

                AddItem(102, y, 3823); // Gold
                TextDefinition.AddHtmlText(this, 153, y, 717, 20, "Shows your shoppe's gold at the top, or for each individual contract.", HtmlColors.BROWN);
                y += 35;

                AddItem(114, y, 10174); // Sack
                TextDefinition.AddHtmlText(this, 153, y, 717, 20, "Shows your shoppe's resource count at the top, or resources needed for each individual contract.", HtmlColors.BROWN);
                y += 35;

                AddItem(94, y, 3710); // Crate
                TextDefinition.AddHtmlText(this, 153, y, 717, 20, "Shows your shoppe's reputation at the top, or for each individual contract.", HtmlColors.MUSTARD);
                y += 35;

                AddItem(102, y, 10922); // Promotional Token
                TextDefinition.AddHtmlText(this, 153, y, 717, 20, "Shows your reputation bonus, if you are a member of the associated guild.", HtmlColors.BROWN);
                y += 35;

                AddItem(102, y, 4030); // Open Book
                TextDefinition.AddHtmlText(this, 153, y, 717, 20, "Shows your chance to successfully fulfill a contract.", HtmlColors.MUSTARD);
                y += 35;

                AddImage(108, y, 4023); // OK button
                TextDefinition.AddHtmlText(this, 153, y, 717, 40, "This will attempt to fulfill the contract. If you fail, you will lose materials and reputation. If you succeed, you will gain reputation and gold, as well as using up the appropriate materials.", HtmlColors.BROWN);
                y += 15;
                y += 35;

                AddImage(108, y, 4020); // Cancel button
                TextDefinition.AddHtmlText(this, 153, y, 717, 20, "This will refuse the request, but you will take a reduction in your shoppe's reputation by doing so.", HtmlColors.MUSTARD);
            }
            else
            {
                bool supportsCustomers = m_Shoppe is ICustomerShoppe;
                bool supportsOrders = m_Shoppe is IOrderShoppe;

                TextDefinition.AddHtmlText(this, 11, 11, 532, 20, title, HtmlColors.BROWN);

                AddButton(843, 9, 3610, 3610, (int)Actions.Help, GumpButtonType.Reply, 0); // Help

                // ------------------------------------------------------------------------------------

                int x_header = 162;
                int y_header_icon = 50;
                int y_header_number = y_header_icon + 31;
                int y_header_word = y_header_number + 22;

                AddItem(x_header + 39, y_header_icon, 10283); // Goblet
                TextDefinition.AddHtmlText(this, x_header, y_header_word, 100, 20, string.Format("<CENTER>{0}</CENTER>", "Reputation"), HtmlColors.BROWN);
                TextDefinition.AddHtmlText(this, x_header, y_header_number, 100, 20, string.Format("<CENTER>{0}</CENTER>", context.Reputation), HtmlColors.MUSTARD);
                x_header += 120;

                AddItem(x_header + 27, y_header_icon, 3823); // Gold
                TextDefinition.AddHtmlText(this, x_header, y_header_word, 100, 20, string.Format("<CENTER>{0}</CENTER>", "Gold"), HtmlColors.BROWN);
                TextDefinition.AddHtmlText(this, x_header, y_header_number, 100, 20, string.Format("<CENTER>{0}</CENTER>", context.Gold), HtmlColors.MUSTARD);
                x_header += 120;

                AddItem(x_header + 38, y_header_icon, 10174); // Sack
                TextDefinition.AddHtmlText(this, x_header, y_header_word, 100, 20, string.Format("<CENTER>{0}</CENTER>", toolName), HtmlColors.BROWN);
                TextDefinition.AddHtmlText(this, x_header, y_header_number, 100, 20, string.Format("<CENTER>{0}</CENTER>", context.Tools), HtmlColors.MUSTARD);
                x_header += 120;

                AddItem(x_header + 23, y_header_icon - 4, 3710); // Crate
                TextDefinition.AddHtmlText(this, x_header, y_header_word, 100, 20, string.Format("<CENTER>{0}</CENTER>", resourceName), HtmlColors.BROWN);
                TextDefinition.AddHtmlText(this, x_header, y_header_number, 100, 20, string.Format("<CENTER>{0}</CENTER>", context.Resources), HtmlColors.MUSTARD);
                x_header += 120;

                if (supportsCustomers)
                {
                    AddItem(x_header + 27, y_header_icon, 10922); // Promotional Token
                    TextDefinition.AddHtmlText(this, x_header, y_header_word, 100, 20, string.Format("<CENTER>{0}</CENTER>", "Guild Bonus"), HtmlColors.BROWN);
                    TextDefinition.AddHtmlText(this, x_header, y_header_number, 100, 20, string.Format("<CENTER>{0}</CENTER>", ((ICustomerShoppe)m_Shoppe).GetReputationBonus(from)), HtmlColors.MUSTARD);
                }

                // ------------------------------------------------------------------------------------

                int y = 130;

                var requirements = shoppe.GetRequirementsMessage(from, context);
                if (!string.IsNullOrWhiteSpace(requirements))
                {
                    AddTextCard(y, requirements);
                    return;
                }

                TextDefinition.AddHtmlText(this, 21, y, CARD_WIDTH, 20, string.Format("<RIGHT>{0}/{1} Customers</RIGHT>", context.Customers.Count, ShoppeConstants.MAX_CUSTOMERS), HtmlColors.MUSTARD);
                y += 20;

                if (supportsCustomers)
                {
                    int CUSTOMERS_PER_PAGE = supportsOrders ? 3 : 7;
                    for (int index = 0; index < CUSTOMERS_PER_PAGE; index++)
                    {
                        if (index < context.Customers.Count)
                        {
                            CustomerContext customer = context.Customers[index];
                            AddCustomerCard(from, (ICustomerShoppe)m_Shoppe, context, customer, index, ref y);
                        }
                        else
                        {
                            AddTextCard(y, "Awaiting Next Customer");
                            y += CARD_HEIGHT;
                            y += 12;
                        }
                    }
                }

                if (supportsOrders)
                {
                    y += 10;
                    TextDefinition.AddHtmlText(this, 21, y, CARD_WIDTH, 20, string.Format("<RIGHT>{0}/{1} Orders</RIGHT>", context.Orders.Count, ShoppeConstants.MAX_ORDERS), HtmlColors.MUSTARD);
                    y += 20;

                    const int ORDERS_PER_PAGE = 3;
                    for (int index = 0; index < ORDERS_PER_PAGE; index++)
                    {
                        if (index < context.Orders.Count)
                        {
                            IOrderContext order = context.Orders[index];
                            AddOrderCard((IOrderShoppe)m_Shoppe, order, index, ref y);
                        }
                        else
                        {
                            AddTextCard(y, "Awaiting Next Order");
                            y += CARD_HEIGHT;
                            y += 12;
                        }
                    }
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var buttonID = info.ButtonID;
            if (buttonID == 0) return;

            if ((int)Actions.RejectOrderBase <= buttonID) // Reject Order is higher
            {
                var index = buttonID - (int)Actions.RejectOrderBase;
                ((IOrderShoppe)m_Shoppe).RejectOrder(index, m_Context);
            }
            else if ((int)Actions.CompleteOrderBase <= buttonID) // Complete is higher
            {
                var index = buttonID - (int)Actions.CompleteOrderBase;
                ((IOrderShoppe)m_Shoppe).OpenRewardSelectionGump(index, sender.Mobile, m_Context);
                return;
            }
            else if ((int)Actions.DoOrderBase <= buttonID) // Do Order is higher
            {
                var index = buttonID - (int)Actions.DoOrderBase;
                ((IOrderShoppe)m_Shoppe).OpenOrderGump(index, sender.Mobile, m_Context);
                return;
            }
            else if ((int)Actions.RejectBase <= buttonID) // Reject is higher
            {
                var index = buttonID - (int)Actions.RejectBase;
                ((CustomerShoppe)m_Shoppe).RejectCustomer(index, m_Context);
            }
            else if ((int)Actions.AcceptBase <= buttonID) // Accept is higher
            {
                var index = buttonID - (int)Actions.AcceptBase;
                ((CustomerShoppe)m_Shoppe).AcceptCustomer(index, sender.Mobile, m_Context);
            }

            sender.Mobile.SendGump(new ShoppeGump(
                m_From,
                m_Shoppe,
                m_Context,
                m_Title,
                m_ToolName,
                m_ResourceName,
                buttonID == (int)Actions.Help
            ));
        }

        private void AddCard(int y)
        {
            AddBackground(20, y, CARD_WIDTH, CARD_HEIGHT + 5, 2620);
        }

        private void AddCustomerCard(
            PlayerMobile from,
            ICustomerShoppe shoppe,
            TradeSkillContext context,
            CustomerContext customer,
            int index,
            ref int y
            )
        {
            AddCard(y);
            y += 10; // Top padding

            // Flavor Text
            TextDefinition.AddHtmlText(this, 36, y, 319, 20, customer.Person, HtmlColors.MUSTARD);
            TextDefinition.AddHtmlText(this, 36, y + 20, 360, 40, customer.Description, false, false, HtmlColors.BROWN, HtmlColors.BROWN);

            // Accept/Decline
            const int x_card_action = 780;
            if (shoppe.CanAcceptCustomer(context, customer))
            {
                AddButton(x_card_action, y - 1, 4023, 4023, (int)Actions.AcceptBase + index, GumpButtonType.Reply, 0); // WILL FIX IT
                TextDefinition.AddHtmlText(this, x_card_action + 35, y - 1 + 3, 60, 20, "Accept", HtmlColors.MUSTARD);
            }
            else
            {
                var reason =
                    !shoppe.HasEnoughTools(context, customer)
                    ? "Tools"
                    : !shoppe.HasEnoughResources(context, customer)
                        ? "Resources"
                        : !shoppe.HasGoldCapacity(context, customer)
                            ? "Gold"
                            : "Unknown";
                TextDefinition.AddHtmlText(this, x_card_action + 35, y - 1 + 3, 60, 20, reason, HtmlColors.RED);
            }

            AddButton(x_card_action, y + 30, 4020, 4020, (int)Actions.RejectBase + index, GumpButtonType.Reply, 0); // WILL NOT FIX IT
            TextDefinition.AddHtmlText(this, x_card_action + 35, y + 30 + 3, 60, 20, "Decline", HtmlColors.MUSTARD);

            // Job Details
            y += 18;
            int x_card_icon = 400;
            AddItem(x_card_icon, y - 6, 10283);
            TextDefinition.AddHtmlText(this, x_card_icon + 24, y, 30, 20, customer.ReputationReward.ToString(), HtmlColors.MUSTARD);
            x_card_icon += 50;

            AddItem(x_card_icon, y - 4, 3823);
            TextDefinition.AddHtmlText(this, x_card_icon + 40, y, 50, 20, customer.GoldReward.ToString(), HtmlColors.MUSTARD);
            x_card_icon += 90;

            AddItem(x_card_icon, y - 2, 10174);
            TextDefinition.AddHtmlText(this, x_card_icon + 30, y, 30, 20, customer.ToolCost.ToString(), HtmlColors.MUSTARD);
            x_card_icon += 50;

            AddItem(x_card_icon, y - 6, 3710);
            TextDefinition.AddHtmlText(this, x_card_icon + 47, y, 30, 20, customer.ResourceCost.ToString(), HtmlColors.MUSTARD);
            x_card_icon += 82;

            AddItem(x_card_icon, y - 3, 4030);
            TextDefinition.AddHtmlText(this, x_card_icon + 40, y, 50, 20, string.Format("{0}%", shoppe.GetSuccessChance(from, customer.Difficulty)), HtmlColors.MUSTARD);

            y += 52;
        }

        private void AddOrderCard(
            IOrderShoppe shoppe,
            IOrderContext order,
            int index,
            ref int y
            )
        {
            AddCard(y);
            y += 10; // Top padding

            // Flavor Text
            TextDefinition.AddHtmlText(this, 36, y, 319, 20, order.Person, HtmlColors.MUSTARD);
            TextDefinition.AddHtmlText(this, 36, y + 20, 360, 40, shoppe.GetDescription(order), false, false, HtmlColors.BROWN, HtmlColors.BROWN);

            // Accept/Decline
            const int x_card_action = 780;
            if (order.IsComplete)
            {
                AddButton(x_card_action, y - 1, 4023, 4023, (int)Actions.CompleteOrderBase + index, GumpButtonType.Reply, 0); // ALREADY COMPLETED
                TextDefinition.AddHtmlText(this, x_card_action + 35, y - 1 + 3, 60, 20, "Complete", HtmlColors.MUSTARD);
            }
            else
            {
                AddButton(x_card_action, y - 1, 4023, 4023, (int)Actions.DoOrderBase + index, GumpButtonType.Reply, 0); // WILL FIX IT
                TextDefinition.AddHtmlText(this, x_card_action + 35, y - 1 + 3, 60, 20, "Add Items", HtmlColors.MUSTARD);

                AddButton(x_card_action, y + 30, 4020, 4020, (int)Actions.RejectOrderBase + index, GumpButtonType.Reply, 0); // WILL NOT FIX IT
                TextDefinition.AddHtmlText(this, x_card_action + 35, y + 30 + 3, 60, 20, "Decline", HtmlColors.MUSTARD);
            }

            // Job Details
            y += 18;
            int x_card_icon = 400;
            AddItem(x_card_icon, y - 6, 10283);
            TextDefinition.AddHtmlText(this, x_card_icon + 24, y, 30, 20, order.ReputationReward.ToString(), HtmlColors.MUSTARD);
            x_card_icon += 50;

            AddItem(x_card_icon, y - 4, 3823);
            TextDefinition.AddHtmlText(this, x_card_icon + 40, y, 50, 20, order.GoldReward.ToString(), HtmlColors.MUSTARD);
            x_card_icon += 90;

            AddItem(x_card_icon, y - 3, 0x0EEC, 0x44C); // 1072
            TextDefinition.AddHtmlText(this, x_card_icon + 45, y, 50, 20, order.PointReward.ToString(), HtmlColors.MUSTARD);

            y += 52;
        }

        private void AddTextCard(int y, string centeredText)
        {
            AddCard(y);
            TextDefinition.AddHtmlText(this, 21, y + 27, CARD_WIDTH, 20, string.Format("<CENTER>{0}</CENTER>", centeredText), HtmlColors.MUSTARD);
        }
    }
}