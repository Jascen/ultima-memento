namespace Server.Engines.GlobalShoppe
{
    public interface IOrderShoppe
    {
        void CompleteOrder(int index, Mobile from, TradeSkillContext context, RewardType selectedReward);

        string GetDescription(IOrderContext order);

        void OpenOrderGump(int index, Mobile from, TradeSkillContext context);

        void OpenRewardSelectionGump(int index, Mobile from, TradeSkillContext context);

        void RejectOrder(int index, TradeSkillContext context);
    }
}