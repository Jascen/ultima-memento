using Server.Mobiles;
using Server.Misc;
using Server.Network;

namespace Server.Gumps
{
	public enum VendorContainerSellSelectionBehavior
	{
		AsManyAsPossible = 0,
		All = 1,
		None = 2
	}

	public class VendorContainerSellConfigGump : Gump
	{
		public const int DefaultVendorContainerSellCompactItemsPerPage = 8;
		public const int DefaultVendorContainerSellLargeItemsPerPage = 5;
		public const int MinVendorContainerSellItemsPerPage = 1;
		public const int MaxVendorContainerSellItemsPerPage = 50;

		private const int EnableButton = 1;
		private const int ShowImagesButton = 2;
		private const int CompactItemsPerPageButton = 3;
		private const int LargeItemsPerPageButton = 4;
		private const int SelectionAsManyButton = 10;
		private const int SelectionAllButton = 11;
		private const int SelectionNoneButton = 12;

		private readonly Mobile m_From;
		private readonly int m_ReturnPage;

		public VendorContainerSellConfigGump(Mobile from, int returnPage) : base(50, 50)
		{
			m_From = from;
			m_ReturnPage = returnPage;

			PlayerMobile pm = from as PlayerMobile;
			PlayerPreferenceContext prefs = pm != null ? pm.Preferences : null;

			var enabled = prefs == null || prefs.VendorContainerSellEnabled;
			var showImages = prefs != null && prefs.VendorContainerSellShowItemImages;
			var compactPerPage = prefs != null ? prefs.VendorContainerSellCompactItemsPerPage : DefaultVendorContainerSellCompactItemsPerPage;
			var largePerPage = prefs != null ? prefs.VendorContainerSellLargeItemsPerPage : DefaultVendorContainerSellLargeItemsPerPage;
			var selection = prefs != null ? prefs.VendorContainerSellSelectionBehavior : VendorContainerSellSelectionBehavior.AsManyAsPossible;

			Closable = true;
			Disposable = true;
			Dragable = true;

			const int GUMP_WIDTH = 425;
			const int PADDING = 10;

			int x = PADDING;
			int y = PADDING;

			AddPage(0);

			AddImage(0, 0, 9580, PlayerSettings.GetGumpHue(from));
			AddHtml(0, y, GUMP_WIDTH, 20, string.Format("<CENTER>{0}</CENTER>", TextDefinition.GetColorizedText("CONTAINER SELL SETTINGS", HtmlColors.LIGHT_GOLD)), false, false);

			y += 40;
			AddToggleRow(x, y, EnableButton, enabled, "Use new gump");

			x += 2 * PADDING;
			y += 30;
			AddToggleRow(x, y, ShowImagesButton, showImages, "Show item images");

			y += 30;
			var imagesPerPageButtonId = showImages ? LargeItemsPerPageButton : CompactItemsPerPageButton;
			var imageCountPerPage = showImages ? largePerPage : compactPerPage;
			AddButton(x, y, 4005, 4007, imagesPerPageButtonId, GumpButtonType.Reply, 0);
			AddHtml(x + 35, y + 3, GUMP_WIDTH - (PADDING * 2) - 35, 20, TextDefinition.GetColorizedText(string.Format("{0} items per page", imageCountPerPage), HtmlColors.LIGHT_GOLD), false, false);

			y += 30;
			AddHtml(x, y, GUMP_WIDTH - (PADDING * 2), 20, TextDefinition.GetColorizedText("Default selection behavior", HtmlColors.LIGHT_GOLD), false, false);

			x += 2 * PADDING;
			y += 20;
			AddSelectionRow(x, y, SelectionAsManyButton, selection == VendorContainerSellSelectionBehavior.AsManyAsPossible, "Up to vendor limit");

			y += 30;
			AddSelectionRow(x, y, SelectionAllButton, selection == VendorContainerSellSelectionBehavior.All, "All items");

			y += 30;
			AddSelectionRow(x, y, SelectionNoneButton, selection == VendorContainerSellSelectionBehavior.None, "No items");
		}

		private void AddToggleRow(int x, int y, int buttonId, bool on, string label)
		{
			int graphic = on ? 4018 : 3609;
			AddButton(x, y, graphic, graphic, buttonId, GumpButtonType.Reply, 0);
			AddHtml(x + 35, y + 3, 320, 20, TextDefinition.GetColorizedText(label, HtmlColors.LIGHT_GOLD), false, false);
		}

		private void AddSelectionRow(int x, int y, int buttonId, bool selected, string label)
		{
			int graphic = selected ? 4018 : 3609;
			AddButton(x, y, graphic, graphic, buttonId, GumpButtonType.Reply, 0);
			AddHtml(x + 35, y + 3, 320, 20, TextDefinition.GetColorizedText(label, HtmlColors.LIGHT_GOLD), false, false);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;
			if (from == null || from != m_From) return;

			from.SendSound(0x4A);

			if (info.ButtonID < 1)
			{
				ReturnToHelp();
				return;
			}

			PlayerMobile pm = from as PlayerMobile;

			if (pm == null)
			{
				ReturnToHelp();
				return;
			}

			PlayerPreferenceContext prefs = pm.Preferences;

			switch (info.ButtonID)
			{
				case EnableButton:
					prefs.VendorContainerSellEnabled = !prefs.VendorContainerSellEnabled;
					break;

				case ShowImagesButton:
					prefs.VendorContainerSellShowItemImages = !prefs.VendorContainerSellShowItemImages;
					break;

				case CompactItemsPerPageButton:
					VendorContainerSellItemsPerPagePrompt.Begin(from, VendorContainerSellItemsPerPageMode.Compact, m_ReturnPage);
					return;

				case LargeItemsPerPageButton:
					VendorContainerSellItemsPerPagePrompt.Begin(from, VendorContainerSellItemsPerPageMode.Large, m_ReturnPage);
					return;

				case SelectionAsManyButton:
					prefs.VendorContainerSellSelectionBehavior = VendorContainerSellSelectionBehavior.AsManyAsPossible;
					break;

				case SelectionAllButton:
					prefs.VendorContainerSellSelectionBehavior = VendorContainerSellSelectionBehavior.All;
					break;

				case SelectionNoneButton:
					prefs.VendorContainerSellSelectionBehavior = VendorContainerSellSelectionBehavior.None;
					break;

				default:
					return;
			}

			from.CloseGump(typeof(VendorContainerSellConfigGump));
			from.SendGump(new VendorContainerSellConfigGump(from, m_ReturnPage));
		}

		private void ReturnToHelp()
		{
			m_From.CloseGump(typeof(Engines.Help.HelpGump));
			m_From.SendGump(new Engines.Help.HelpGump(m_From, m_ReturnPage));
		}
	}
}
