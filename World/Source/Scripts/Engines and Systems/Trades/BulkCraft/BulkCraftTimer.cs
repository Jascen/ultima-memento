using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Craft
{
	public class BulkCraftTimer : Timer
	{
		public readonly PlayerMobile Player;

		private readonly BulkCraftContext m_Context;
		private readonly CraftSystem m_CraftSystem;
		private readonly CraftItem m_CraftItem;
		private readonly BaseTool m_Tool;
		private readonly Type m_TypeRes;

		public BulkCraftTimer(CraftItem craftItem, PlayerMobile player, CraftSystem craftSystem, BaseTool tool, Type typeRes, int amount) : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
		{
			Player = player;
			m_CraftItem = craftItem;
			m_CraftSystem = craftSystem;
			m_Tool = tool;
			m_TypeRes = typeRes;
			m_Context = new BulkCraftContext { Amount = amount };
		}

		protected override void OnTick()
		{
			int last = m_Context.Current;

			if (!m_Context.Paused && m_CraftItem.Craft(Player, m_CraftSystem, m_TypeRes, m_Tool, m_Context))
				Player.EndAction(typeof(CraftSystem));

			bool isComplete = m_Context.Amount <= m_Context.Success;
			if (isComplete) // Session has completed
				Stop();
			else if (m_Tool == null || m_Tool.Parent != Player)
				Cancel();

			if (!m_Context.Paused && !m_Context.Suppressed && !m_Context.Cancelled && last != m_Context.Current)
				RefreshGump(isComplete);
		}

		public void Pause()
		{
			m_Context.Paused = true;
			RefreshGump(false);
		}

		public void Cancel()
		{
			Stop();
			m_Context.Cancelled = true;

			if (!m_Context.Suppressed)
				RefreshGump(true);
		}

		private void RefreshGump(bool isComplete)
		{
			Player.CloseGump(typeof(BulkCraftGump));
			Player.SendGump(new BulkCraftGump(Player, m_Context, isComplete));
		}
	}
}