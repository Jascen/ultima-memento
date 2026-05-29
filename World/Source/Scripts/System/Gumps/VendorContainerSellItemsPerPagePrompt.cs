using Server.Mobiles;
using Server.Prompts;

namespace Server.Gumps
{
	public enum VendorContainerSellItemsPerPageMode
	{
		Compact,
		Large
	}

	public class VendorContainerSellItemsPerPagePrompt : Prompt
	{
		private readonly Mobile m_From;
		private readonly int m_ReturnPage;
		private readonly VendorContainerSellItemsPerPageMode m_Mode;
		public const int MinItemsPerPage = 1;
		public const int MaxItemsPerPage = 50;

		public static void Begin(Mobile from, VendorContainerSellItemsPerPageMode mode, int returnPage)
		{
			if (from == null) return;

			var label = mode == VendorContainerSellItemsPerPageMode.Compact ? "compact" : "large";
			from.SendMessage(string.Format("Enter {0} items per page ({1}-{2}, ESC to cancel):", label, MinItemsPerPage, MaxItemsPerPage));
			from.Prompt = new VendorContainerSellItemsPerPagePrompt(from, mode, returnPage);
		}

		private VendorContainerSellItemsPerPagePrompt(Mobile from, VendorContainerSellItemsPerPageMode mode, int returnPage)
		{
			m_From = from;
			m_Mode = mode;
			m_ReturnPage = returnPage;
		}

		public override void OnResponse(Mobile from, string text)
		{
			if (from != m_From) return;

			var player = from as PlayerMobile;
			if (player == null)
			{
				ReopenConfig(null);
				return;
			}

			text = text != null ? text.Trim() : string.Empty;

			int amount;
			if (!int.TryParse(text, out amount) || amount < MinItemsPerPage || amount > MaxItemsPerPage)
			{
				ReopenConfig(string.Format("Please enter a number between {0} and {1}.", MinItemsPerPage, MaxItemsPerPage));
				return;
			}

			if (m_Mode == VendorContainerSellItemsPerPageMode.Compact) player.Preferences.VendorContainerSellCompactItemsPerPage = amount;
			else player.Preferences.VendorContainerSellLargeItemsPerPage = amount;

			ReopenConfig(null);
		}

		public override void OnCancel(Mobile from)
		{
			if (from != m_From) return;

			ReopenConfig(null);
		}

		private void ReopenConfig(string notice)
		{
			if (notice != null)
				m_From.SendMessage(notice);

			m_From.CloseGump(typeof(VendorContainerSellConfigGump));
			m_From.SendGump(new VendorContainerSellConfigGump(m_From, m_ReturnPage));
		}
	}
}
