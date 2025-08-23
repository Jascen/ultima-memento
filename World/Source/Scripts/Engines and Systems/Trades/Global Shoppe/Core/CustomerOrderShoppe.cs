using System;
using System.Collections.Generic;
using Server.Utilities;

namespace Server.Engines.GlobalShoppe
{
	[SkipSerializeReq]
    public abstract class CustomerOrderShoppe<TOrderContext> : CustomerShoppe, IOrderShoppe
        where TOrderContext : class, IOrderContext
    {
        protected CustomerOrderShoppe(Serial serial) : base(serial)
        {
        }

        protected CustomerOrderShoppe(int itemId) : base(itemId)
        {
        }

        public void CompleteOrder(int index, Mobile from, TradeSkillContext context, RewardType selectedReward)
        {
            if (context.Orders.Count <= index) return;

            var order = context.Orders[index];
            if (!order.IsComplete) return;

            switch (selectedReward)
            {
                case RewardType.Gold:
                    context.Gold += order.GoldReward;
                    break;
                case RewardType.Points:
                    context.Points += order.PointReward;
                    break;
                case RewardType.Reputation:
                    context.Reputation = Math.Min(ShoppeConstants.MAX_REPUTATION, context.Reputation + order.ReputationReward);
                    break;
            }

			SkillUtilities.DoSkillChecks(from, SkillName.Mercantile, 3);
            context.Orders.Remove(order);

            from.PlaySound(0x32); // Dropgem1
        }

        public void OpenRewardSelectionGump(int index, Mobile from, TradeSkillContext context)
        {
            if (context.Orders.Count <= index) return;

            var order = context.Orders[index];
            if (!order.IsComplete) return;

            from.CloseGump(typeof(RewardSelectionGump));
            from.SendGump(new RewardSelectionGump(from, this, context, order, index));
        }

        public string GetDescription(IOrderContext order)
        {
            var typed = order as TOrderContext;
            if (typed == null) return "invalid_order";

            return GetDescription(typed);
        }

        public void OpenOrderGump(int index, Mobile from, TradeSkillContext context)
        {
            if (context.Orders.Count <= index) return;

            var order = context.Orders[index];
            if (order.IsComplete) return;

            from.CloseGump(typeof(OrderGump));
            from.SendGump(new OrderGump(from, order));
        }

        public void RejectOrder(int index, TradeSkillContext context)
        {
            if (context.Orders.Count <= index) return;

            var order = context.Orders[index];
            context.Reputation = Math.Max(0, context.Reputation - order.ReputationReward);

            context.Orders.Remove(order);
        }

        protected abstract IEnumerable<TOrderContext> CreateOrders(Mobile from, TradeSkillContext context, int amount);

        protected abstract string GetDescription(TOrderContext order);

        protected override void OpenGump(Mobile from, TradeSkillContext context)
        {
            if (context.FeePaid && context.CanRefreshOrders)
            {
                var count = ShoppeConstants.MAX_ORDERS - context.Orders.Count;
                foreach (var order in CreateOrders(from, context, count))
                {
                    context.Orders.Add(order);
                }

                context.CanRefreshOrders = false;
                context.NextOrderRefresh = DateTime.UtcNow.Add(ShoppeConstants.ORDER_REFRESH_DELAY);
            }

            base.OpenGump(from, context);
        }
    }
}