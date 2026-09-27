using System;

namespace Server.Gumps
{
	public class ConfirmationGump : Gump
	{
		private readonly Action _onConfirmed;
		private readonly Action _onDeclined;

		/// <summary>
		/// Immediately executes `onConfirmed` if `shouldSend` is `true`.
		/// Otherwise, sends a confirmation gump.
		/// </summary>
		public static void PromptIfFalse(Mobile from, bool shouldSend, Action onConfirmed, Func<Action, ConfirmationGump> gumpFactory)
		{
			if (shouldSend)
			{
				ConfirmationGump gump = gumpFactory(onConfirmed);
				from.SendGump(gump);
			}
			else
			{
				onConfirmed();
			}
		}

		public ConfirmationGump(Mobile player, string message, Action onConfirmed = null, Action onDeclined = null)
			: this(player, null, message, onConfirmed, onDeclined)
		{
		}

		public ConfirmationGump(Mobile player, string title, string message, Action onConfirmed = null, Action onDeclined = null, int height = 280) : base(25, 25)
		{
			player.CloseGump(typeof(ConfirmationGump));

			_onConfirmed = onConfirmed;
			_onDeclined = onDeclined;
			const int WIDTH = 475;
			int HEIGHT = height;
			const int PAGE_PADDING = 20;
			const int BODY_START = PAGE_PADDING;
			const int BODY_WIDTH = WIDTH - PAGE_PADDING * 2;

			AddPage(0);
			AddBackground(0, 0, WIDTH, HEIGHT, 2620);

			int y = PAGE_PADDING;
			if (!string.IsNullOrWhiteSpace(title))
			{
				AddBackground(0, 0, WIDTH, 50, 2620);
				TextDefinition.AddHtmlText(this, BODY_START, y, BODY_WIDTH, 50, string.Format("<CENTER>{0}</CENTER>", title), HtmlColors.WHITE);
				y += 80;
			}

			TextDefinition.AddHtmlText(this, BODY_START, y, BODY_WIDTH, HEIGHT - 110, message, HtmlColors.WHITE);

			int buttonY = HEIGHT - 50;
			int buttonX = 100;
			AddButton(buttonX, buttonY, 4005, 4005, 1, GumpButtonType.Reply, 0);
			TextDefinition.AddHtmlText(this, buttonX + 30, buttonY + 3, 58, 20, "Yes", HtmlColors.WHITE);

			buttonX = WIDTH - 145;
			AddButton(buttonX, buttonY, 4020, 4020, 2, GumpButtonType.Reply, 0);
			TextDefinition.AddHtmlText(this, buttonX + 30, buttonY + 3, 58, 20, "No", HtmlColors.WHITE);
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			if (info.ButtonID == 1)
			{
				if (_onConfirmed != null)
					_onConfirmed();
			}
			else
			{
				if (_onDeclined != null)
					_onDeclined();
			}
		}
	}
}