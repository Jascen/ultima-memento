using Server.Items;
using Server.Prompts;

namespace Server.Engines.Craft
{
	public enum CraftAmountSource
	{
		CraftGumpMakeLast,
		CraftGumpItem
	}

	public class CraftAmountPrompt : Prompt
	{
		private const string INVALID_AMOUNT_MESSAGE = "Please pick a number between 1 and 10,000";

		private readonly Mobile m_From;
		private readonly CraftSystem m_CraftSystem;
		private readonly BaseTool m_Tool;
		private readonly CraftItem m_CraftItem;
		private readonly CraftAmountSource m_Source;

		public static void Begin(Mobile from, CraftSystem craftSystem, BaseTool tool, CraftItem craftItem, CraftAmountSource source)
		{
			if (from == null || craftSystem == null || tool == null || craftItem == null) return;

			from.SendMessage("Enter how many to craft (1-10000, ESC to cancel):");
			from.Prompt = new CraftAmountPrompt(from, craftSystem, tool, craftItem, source);
		}

		private CraftAmountPrompt(Mobile from, CraftSystem craftSystem, BaseTool tool, CraftItem craftItem, CraftAmountSource source)
		{
			m_From = from;
			m_CraftSystem = craftSystem;
			m_Tool = tool;
			m_CraftItem = craftItem;
			m_Source = source;
		}

		public override void OnResponse(Mobile from, string text)
		{
			if (from != m_From) return;

			text = text != null ? text.Trim() : string.Empty;

			int amount;

			if (!int.TryParse(text, out amount) || amount < 1 || amount > 10000)
			{
				ReopenGump(INVALID_AMOUNT_MESSAGE);
				return;
			}

			CraftGump.DoCraft(m_From, m_CraftSystem, m_Tool, m_CraftItem, amount);
		}

		public override void OnCancel(Mobile from)
		{
			if (from != m_From) return;

			ReopenGump(null);
		}

		private void ReopenGump(string notice)
		{
			if (m_Source == CraftAmountSource.CraftGumpItem)
			{
				if (notice != null)
					m_From.SendMessage(notice);

				m_From.SendGump(new CraftGumpItem(m_From, m_CraftSystem, m_CraftItem, m_Tool));
			}
			else
			{
				m_From.SendGump(new CraftGump(m_From, m_CraftSystem, m_Tool, notice));
			}
		}
	}
}
