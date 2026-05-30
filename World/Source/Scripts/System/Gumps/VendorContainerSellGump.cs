using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Utilities;

namespace Server.Gumps
{
	public class VendorContainerSellGump : Gump
	{
		public const int DefaultCompactItemsPerPage = 10;
		public const int DefaultLargeItemsPerPage = 5;

		private const int CompactRowHeight = 30;
		private const int LargeRowHeight = 48;
		private const int ConfirmButton = 1;

		private readonly BaseVendor m_Vendor;
		private readonly Serial m_ContainerSerial;
		private readonly List<SellItemState> m_Items;

		public VendorContainerSellGump(BaseVendor vendor, Container container, List<SellItemState> items, int vendorGold, bool unlimitedGold, PlayerPreferenceContext preferences)
			: base(50, 50)
		{
			m_Vendor = vendor;
			m_ContainerSerial = container.Serial;
			m_Items = items;

			const int GUMP_WIDTH = 400;
			const int PADDING = 15;
			const int LIST_START_Y = PADDING + 50;
			const int FOOTER_HEIGHT = 130;
			const int LAST_COLUMN_X = GUMP_WIDTH - PADDING - 105;

			var showImages = preferences.VendorContainerSellShowItemImages;
			var itemsPerPage = showImages ? preferences.VendorContainerSellLargeItemsPerPage : preferences.VendorContainerSellCompactItemsPerPage;
			var rowHeight = showImages ? LargeRowHeight : CompactRowHeight;
			var selection = preferences.VendorContainerSellSelectionBehavior;

			int pageCount = Math.Max(1, (items.Count + itemsPerPage - 1) / itemsPerPage);

			int rowsThisPage = Math.Min(items.Count, itemsPerPage);
			int gumpHeight = LIST_START_Y + rowsThisPage * rowHeight + FOOTER_HEIGHT;
			var footerYStart = gumpHeight - FOOTER_HEIGHT;

			int listedGold = 0;
			var checkStateVendorGold = vendorGold;
			for (int page = 0; page < pageCount; page++)
			{
				AddPage(page + 1);

				AddBackground(0, 0, GUMP_WIDTH, gumpHeight, 2620);

				int startX = PADDING;
				int startY = LIST_START_Y;
				int x = startX;
				int y = PADDING;

				AddHtml(0, y, GUMP_WIDTH, 20, string.Format("<CENTER>{0}</CENTER>", TextDefinition.GetColorizedText("What would you like to sell?", HtmlColors.WHITE)), false, false);

				int start = page * itemsPerPage;
				int end = Math.Min(start + itemsPerPage, items.Count);

				for (int i = start; i < end; i++)
				{
					SellItemState state = items[i];
					Item item = state.Item;
					listedGold += state.Price;
					int row = i - start;

					x = startX;
					y = startY + row * rowHeight;

					var isChecked = GetDefaultCheckState(state, ref checkStateVendorGold, unlimitedGold, selection);

					AddCheck(x, y, 210, 211, isChecked, i);
					x += 30;

					if (showImages)
					{
						GumpUtilities.AddCenteredItemToGump(this, item.ItemID, x, y, 45, 40, item.Hue);
						AddItemProperty(item.Serial);
						x += 110;

						AddLabel(x, y + 8, 0x481, GetDisplayName(state));
						AddHtml(LAST_COLUMN_X, y + 8, 85, 20, TextDefinition.GetColorizedText(string.Format("{0:n0} gold", state.Price), HtmlColors.WHITE), false, false);
					}
					else
					{
						AddLabel(x, y, 0x481, GetDisplayName(state));
						AddHtml(LAST_COLUMN_X, y, 105, 20, TextDefinition.GetColorizedText(string.Format("{0:n0} gold", state.Price), HtmlColors.WHITE), false, false);
						AddItemProperty(item.Serial);
					}
				}

				y = footerYStart;

				if (pageCount > 1)
				{
					if (page > 0)
					{
						AddButton(PADDING, y, 4014, 4016, 0, GumpButtonType.Page, page);
						AddHtmlLocalized(PADDING + 35, y + 3, 100, 18, 1044044, 0x7FFF, false, false);
					}

					if (page < pageCount - 1)
					{
						AddButton(LAST_COLUMN_X, y, 4005, 4007, 0, GumpButtonType.Page, page + 2);
						AddHtmlLocalized(LAST_COLUMN_X + 35, y + 3, 100, 18, 1044045, 0x7FFF, false, false);
					}

					y += 30;
				}

				AddHtml(PADDING, y, GUMP_WIDTH - (PADDING * 2), 20, TextDefinition.GetColorizedText(string.Format("{0} has: {1:n0} gold", vendor.Name, vendorGold), HtmlColors.WHITE), false, false);

				y += 20;
				AddHtml(PADDING, y, GUMP_WIDTH - (PADDING * 2), 20, TextDefinition.GetColorizedText(string.Format("All your items are worth: {0:n0} gp", listedGold), HtmlColors.WHITE), false, false);

				y += 20;
				int pageNumber = page + 1;
				AddHtml((GUMP_WIDTH - 100) / 2, y + 3, 100, 20, TextDefinition.GetColorizedText(string.Format("Page {0} of {1}", pageNumber, pageCount), HtmlColors.WHITE), false, false);

				y += 30;
				AddButton(LAST_COLUMN_X, y, 4005, 4007, ConfirmButton, GumpButtonType.Reply, 0);
				AddHtml(LAST_COLUMN_X + 35, y + 3, 80, 20, TextDefinition.GetColorizedText("Sell Items", HtmlColors.WHITE), false, false);
			}
		}

		private static bool GetDefaultCheckState(SellItemState item, ref int vendorGold, bool unlimitedGold, VendorContainerSellSelectionBehavior selection)
		{
			switch (selection)
			{
				case VendorContainerSellSelectionBehavior.All: return true;
				case VendorContainerSellSelectionBehavior.None: return false;
				case VendorContainerSellSelectionBehavior.AsManyAsPossible:
				default:
					{
						if (unlimitedGold) return true;
						if (vendorGold < item.Price) return false;

						vendorGold -= item.Price;
						return true;
					}
			}
		}

		private static string GetDisplayName(SellItemState state)
		{
			Item item = state.Item;
			string name = item.Name;

			if (string.IsNullOrWhiteSpace(name))
				name = state.Name;

			if (item.Amount > 1)
				name = item.Amount + " " + name;

			return name;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			if (from == null) return;
			if (info.ButtonID != ConfirmButton) return;
			if (m_Vendor == null || m_Vendor.Deleted || !from.CheckAlive()) return;
			if (!m_Vendor.IsActiveBuyer || !m_Vendor.CheckVendorAccess(from)) return;
			if (!Utility.RangeCheck(m_Vendor.Location, from.Location, 10)) return;

			Container container = World.FindItem(m_ContainerSerial) as Container;
			if (container == null || container.Deleted || container.RootParent != from) return;

			IShopSellInfo[] sellInfo = m_Vendor.GetSellInfo();
			int barter = m_Vendor.GetSellBarter(from);
			List<SellItemResponse> sellList = new List<SellItemResponse>();

			for (int i = 0; i < m_Items.Count; i++)
			{
				if (!info.IsSwitched(i)) continue;

				Item item = m_Items[i].Item;
				if (item == null || item.Deleted || !item.IsChildOf(container)) continue;

				SellItemState sellState;
				if (!m_Vendor.TryGetSellState(item, from, barter, sellInfo, out sellState)) continue;

				sellList.Add(new SellItemResponse(item, item.Amount));
			}

			if (sellList.Count > 0)
				m_Vendor.OnSellItems(from, sellList);
		}
	}
}
