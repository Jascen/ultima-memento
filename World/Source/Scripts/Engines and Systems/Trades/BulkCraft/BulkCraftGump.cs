using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Craft
{
	public class BulkCraftGump : Gump
	{
		private readonly int GUMP_HEIGHT = 160;
		private readonly int GUMP_WIDTH = 170;
		private const int PADDING = 10;

		private readonly PlayerMobile m_Player;
		private readonly BulkCraftContext m_Context;

		public BulkCraftGump(PlayerMobile player, BulkCraftContext context, bool isComplete = false) : base(25, 25)
		{
			m_Player = player;
			m_Context = context;

			if (context.LastMessage != null)
			{
				GUMP_WIDTH += 40;
				GUMP_HEIGHT += 30;
			}

			AddBackground(0, 0, GUMP_WIDTH + PADDING * 2, GUMP_HEIGHT + PADDING * 2, 2620); // Slate background gold trim

			int x = PADDING;
			int y = PADDING;

			if (context.Paused)
				TextDefinition.AddHtmlText(this, x, y - 4, GUMP_WIDTH, 20, "Session is paused...", false, false, HtmlColors.RED, HtmlColors.RED);
			else if (isComplete)
				TextDefinition.AddHtmlText(this, x, y - 4, GUMP_WIDTH, 20, "Crafting session complete...", false, false, HtmlColors.RED, HtmlColors.RED);
			else if ( context.LastMessage != null )
			{
				int noticeNumber = context.LastMessage is int && (int)context.LastMessage > 0 ? (int)context.LastMessage : 0;
				string noticeText = noticeNumber > 0 ? CliLocTable.Lookup(noticeNumber) : (string)context.LastMessage;
				if (!string.IsNullOrWhiteSpace(noticeText))
					TextDefinition.AddHtmlText(this, x, y - 4, GUMP_WIDTH, 60, noticeText, false, false, HtmlColors.RED, HtmlColors.RED);
				else if (noticeNumber > 0)
					TextDefinition.AddHtmlText(this, x, y - 4, GUMP_WIDTH, 60, noticeNumber, false, false, LabelColors.PALE_RED, HtmlColors.PALE_RED);
				y += 30;
			}
			else
				TextDefinition.AddHtmlText(this, x, y - 4, GUMP_WIDTH, 20, string.Format("Crafting '{0}' items...", context.Amount), false, false, LabelColors.OFFWHITE, HtmlColors.OFFWHITE);
			y += 30;

			// Add progress bar
			const int PROGRESS_WIDTH = 96;
			AddImageTiled(x, y, PROGRESS_WIDTH, 9, 9750); // Gray progress
			var step = (double)PROGRESS_WIDTH / Math.Max(context.Amount, context.Fail + context.Success);
			AddProgressBar(x, y, context.Fail + context.Success, step, true);
			AddProgressBar(x, y, context.Fail, step, false);
			y += 20;

			AddLabelWithValue(x, y, "Fail:", context.Fail);
			y += 20;

			AddLabelWithValue(x, y, "Success:", context.Success);
			y += 20;

			AddLabelWithValue(x, y, "Exceptional:", context.Exceptional);
			y += 20;

			if (isComplete)
			{
				AddButtonWithLabel(x, y, "Close", 4);
			}
			else
			{
				if (context.Paused)
					AddButtonWithLabel(x, y, "Resume", 2);
				else
					AddButtonWithLabel(x, y, "Pause", 3);

				y += 28;
				AddButtonWithLabel(x, y, "Stop", 4);
			}
		}

		private void AddLabelWithValue(int x, int y, string label, int value)
		{
			TextDefinition.AddHtmlText(this, x, y - 4, 100, 16, label, false, false, LabelColors.OFFWHITE, HtmlColors.OFFWHITE);
			TextDefinition.AddHtmlText(this, x + 100, y - 4, GUMP_WIDTH - 100, 16, string.Format("<RIGHT>{0}</RIGHT>", value), false, false, LabelColors.OFFWHITE, HtmlColors.OFFWHITE);
		}

		private void AddButtonWithLabel(int x, int y, string text, int buttonID)
		{
			const int RIGHT_CURVED_ARROW = 4005;
			TextDefinition.AddHtmlText(this, x + 35, y + 3, 100, 16, text, false, false, LabelColors.OFFWHITE, HtmlColors.OFFWHITE);
			AddButton(x, y, RIGHT_CURVED_ARROW, RIGHT_CURVED_ARROW, buttonID, GumpButtonType.Reply, 0);
		}

		private void AddProgressBar(int x, int y, int currentValue, double step, bool isGood)
		{
			if (0 < currentValue)
			{
				if (isGood)
					AddImageTiled(x, y, (int)(currentValue * step), 9, 9752); // Green progress
				else
					AddImageTiled(x, y, (int)(currentValue * step), 9, 40); // Red progress
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID < 1) return;

			switch (info.ButtonID)
			{
				case 1: // Stop showing gump
					m_Context.Suppressed = true;
					break;

				case 2: // Resume
					m_Context.Paused = false;
					break;

				case 3: // Pause
					m_Context.Paused = true;
					m_Player.CloseGump(typeof(BulkCraftGump));
					m_Player.SendGump(new BulkCraftGump(m_Player, m_Context, false));
					break;

				case 4: // Stop
					m_Context.Suppressed = true;
					m_Player.SendMessage("You crafted {0} of {1} items ({2} failed, {3} exceptional).", m_Context.Success, m_Context.Current, m_Context.Fail, m_Context.Exceptional);
					BulkCraft.StopTimer(m_Player);
					break;
			}
		}
	}
}