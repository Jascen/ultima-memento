using System;
using Server.Network;

namespace Server.Gumps
{
	public class BoardGump : Gump
	{
		private readonly Action _onAccept;
		private readonly Action _onReject;

		private const int WIDTH = 648;
		private const int HEIGHT = 435;
		private const int PAGE_PADDING = 20;
		private const int BODY_START = PAGE_PADDING;
		private const int BODY_WIDTH = WIDTH - PAGE_PADDING * 2;

		public BoardGump(Mobile from, string title, string txt, string color, bool bars) : this(from, title, txt, color, bars, null, null, null, null)
		{
		}

		public BoardGump(Mobile from, string title, string txt, string color, bool bars, string acceptText, Action onAccept, string rejectText, Action onReject) : this(from, acceptText, onAccept, rejectText, onReject)
		{
			AddHtml(11, 12, 562, 20, @"<BODY><BASEFONT Color=" + color + ">" + title + "</BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml(12, 44, 623, 328, @"<BODY><BASEFONT Color=" + color + ">" + txt + "</BASEFONT></BODY>", (bool)false, (bool)bars);
		}

		private BoardGump(Mobile from, string acceptText, Action onAccept, string rejectText, Action onReject) : base(100, 100)
		{
			from.SendSound(0x59);

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);

			AddImage(0, 0, 9541, Server.Misc.PlayerSettings.GetGumpHue(from));
			AddButton(609, 8, 4017, 4017, 0, GumpButtonType.Reply, 0);

			int buttonY = HEIGHT - 50;
			int buttonX = PAGE_PADDING;

			if (!string.IsNullOrWhiteSpace(rejectText))
			{
				var maxWidth = WIDTH - buttonX - 30;
				AddButton(buttonX, buttonY, 4020, 4020, 2, GumpButtonType.Reply, 0);
				TextDefinition.AddHtmlText(this, buttonX + 30, buttonY + 3, maxWidth, 20, rejectText, HtmlColors.WHITE);
				_onReject = onReject;

				// Set other button location
				buttonX = WIDTH - 145;
			}

			if (!string.IsNullOrWhiteSpace(acceptText))
			{
				var maxWidth = WIDTH - buttonX - 30;
				AddButton(buttonX, buttonY, 4005, 4005, 1, GumpButtonType.Reply, 0);
				TextDefinition.AddHtmlText(this, buttonX + 30, buttonY + 3, maxWidth, 20, acceptText, HtmlColors.WHITE);
				_onAccept = onAccept;
			}
		}

		public BoardGump(Mobile player, string title, string message, string acceptText, Action onAccept, string rejectText, Action onReject) : this(player, acceptText, onAccept, rejectText, onReject)
		{
			player.CloseGump(typeof(BoardGump));

			int y = PAGE_PADDING;
			if (!string.IsNullOrWhiteSpace(title))
			{
				AddBackground(0, 0, WIDTH, 50, 2620);
				TextDefinition.AddHtmlText(this, BODY_START, y, BODY_WIDTH, 50, string.Format("<CENTER>{0}</CENTER>", title), HtmlColors.WHITE);
				y += 80;
			}

			TextDefinition.AddHtmlText(this, BODY_START, y, BODY_WIDTH, 170, message, HtmlColors.WHITE);
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID == 0) return;

			Mobile from = state.Mobile;
			from.SendSound(0x59);

			if (info.ButtonID == 1)
			{
				if (_onAccept != null)
					_onAccept();
			}
			else
			{
				if (_onReject != null)
					_onReject();
			}
		}
	}
}