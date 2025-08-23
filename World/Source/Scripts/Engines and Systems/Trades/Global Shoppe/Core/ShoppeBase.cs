using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.GlobalShoppe
{
    public abstract class ShoppeBase : Item
    {
        public ShoppeBase(Serial serial) : base(serial)
        {
        }

        protected ShoppeBase(int itemId) : base(itemId)
        {
        }

        public override bool Decays
        { get { return false; } }

        public abstract NpcGuild Guild { get; }

        protected abstract SkillName PrimarySkill { get; }
        protected abstract ShoppeType ShoppeType { get; }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            var context = GetOrCreateContext(from);
            if (0 < context.Gold)
            {
                list.Add(new CashOutEntry((PlayerMobile)from, context));
            }
        }

        public string GetRequirementsMessage(Mobile from, TradeSkillContext context)
        {
            var skill = from.Skills[PrimarySkill];
            if (ShoppeConstants.MIN_SKILL <= skill.Value)
            {
                if (context.FeePaid) return null;

                return string.Format("You must pay {0} gold coins to open this shoppe.", ShoppeConstants.SHOPPE_FEE);
            }

            return string.Format("You must have at least {0} skill in {1} to use this shoppe.", ShoppeConstants.MIN_SKILL, skill.Name);
        }

        public bool HasGoldCapacity(TradeSkillContext context, CustomerContext customer)
        {
            return context.Gold + customer.GoldReward <= ShoppeConstants.MAX_GOLD;
        }

        public override void OnDoubleClick(Mobile from)
        {
			OpenGump(from, true);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is Gold)
            {
                var context = GetOrCreateContext(from);
                if (!context.FeePaid && ShoppeConstants.SHOPPE_FEE <= dropped.Amount)
                {
                    context.FeePaid = true;
                    dropped.Amount -= ShoppeConstants.SHOPPE_FEE;
                    if (dropped.Amount < 1)
                        dropped.Delete();

                    OnDoubleClick(from);

                    return dropped.Deleted;
                }
            }

            return base.OnDragDrop(from, dropped);
        }

        public override bool OnDragLift(Mobile from)
        {
            // Entity is Visible so Ctrl+Shift can show it
            // Disallow moving it
            return false;
        }

		public void OpenGump(Mobile from, bool sendMessage)
		{
            if (!ShoppeEngine.Instance.IsEnabled)
            {
				if (sendMessage)
                	from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, false, "Shoppe system is currently disabled.");

                return;
            }

            if (!from.InRange(GetWorldLocation(), 2))
            {
				if (sendMessage)
                	from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.

                return;
            }

            var context = GetOrCreateContext(from);
            OpenGump(from, context);
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        protected string CreatePersonName()
        {
            return string.Format("{0} {1}", Utility.RandomBool() ? NameList.RandomName("female") : NameList.RandomName("male"), TavernPatrons.GetTitle());
        }

        protected abstract string CreateTask(TradeSkillContext context);

        protected IEnumerable<CraftItem> GetCraftItems(Mobile from, CraftSystem craftSystem)
        {
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
                        )
                        {
                            hasSkills = false;
                            break;
                        }
                    }

                    if (hasSkills) yield return craftItem;
                }
            }
        }

        protected abstract ShoppeGump GetGump(PlayerMobile from);

        protected TradeSkillContext GetOrCreateContext(Mobile from)
        {
            return ShoppeEngine.Instance.GetOrCreateShoppeContext(from, ShoppeType);
        }

        protected virtual void OpenGump(Mobile from, TradeSkillContext context)
        {
            from.CloseGump(typeof(ShoppeGump));
            from.SendGump(GetGump((PlayerMobile)from));
        }
    }

    public class CashOutEntry : ContextMenuEntry
    {
        private readonly TradeSkillContext m_Context;
        private readonly PlayerMobile m_From;

        public CashOutEntry(PlayerMobile from, TradeSkillContext context) : base(6113, 3)
        {
            m_From = from;
            m_Context = context;
        }

        public override void OnClick()
        {
            if (m_Context.Gold <= 0) return;

            double barter = (int)(m_From.Skills[SkillName.Mercantile].Value / 2);
            if (m_From.NpcGuild == NpcGuild.MerchantsGuild)
                barter += 25.0; // FOR GUILD MEMBERS

            barter /= 100;

            int bonus = (int)(m_Context.Gold * barter);

            int cash = m_Context.Gold;

            m_From.AddToBackpack(new BankCheck(cash));
            m_From.SendMessage("You now have a check for " + cash.ToString() + " gold.");
            m_Context.Gold = 0;
        }
    }
}