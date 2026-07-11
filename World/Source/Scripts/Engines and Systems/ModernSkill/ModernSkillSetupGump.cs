using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.ModernSkill
{
	public class ModernSkillSetupGump : Gump
	{
		private enum PageActions
		{
			Close = 0,
			ModernLockpickingEnabled = 1,
			ModernRemoveTrapEnabled,
			ModernLockpickingAutoRetryEnabled,
			ModernRemoveTrapsAutoRetryEnabled,
		}

		private const int GUMP_HEIGHT = 200;
		private const int GUMP_WIDTH = 450;
		private const int MAX_CONTENT_WIDTH = GUMP_WIDTH - (4 * PADDING);
		private const int PADDING = 10;

		private readonly int m_ReturnPage;

		public ModernSkillSetupGump(PlayerMobile player, int returnPage) : base(50, 50)
		{
			m_ReturnPage = returnPage;

			PlayerPreferenceContext prefs = player.Preferences;

			Closable = true;
			Disposable = true;
			Dragable = true;

			const int SETTING_INDENT = 35;

			int x = 2 * PADDING;
			int y = PADDING;

			AddPage(0);

			// AddImage(0, 0, 9580, PlayerSettings.GetGumpHue(from));
			AddBackground(0, 0, GUMP_WIDTH, GUMP_HEIGHT, 0x1453); // Tan box
			AddImageTiled(PADDING, PADDING, GUMP_WIDTH - (PADDING * 2), GUMP_HEIGHT - (PADDING * 2), 2624); // Black box
			AddAlphaRegion(PADDING, PADDING, GUMP_WIDTH - (PADDING * 2), GUMP_HEIGHT - (PADDING * 2));

			AddHtml(0, y, GUMP_WIDTH, 20, string.Format("<CENTER>{0}</CENTER>", TextDefinition.GetColorizedText("MODERN SKILL SETTINGS", HtmlColors.WHITE)), false, false);
			y += 40;

			AddToggleRow(x, y, (int)PageActions.ModernLockpickingEnabled, prefs.ModernLockpickingEnabled, "Lockpicks: Show gump when targeting a locked container");
			y += 30;
			AddToggleRow(x + SETTING_INDENT, y, (int)PageActions.ModernLockpickingAutoRetryEnabled, prefs.ModernLockpickingAutoRetryEnabled, "Auto-retry");
			y += 30;

			AddToggleRow(x, y, (int)PageActions.ModernRemoveTrapEnabled, prefs.ModernRemoveTrapEnabled, "Remove Trap: Show gump when targeting a trap or container");
			y += 30;
			AddToggleRow(x + SETTING_INDENT, y, (int)PageActions.ModernRemoveTrapsAutoRetryEnabled, prefs.ModernRemoveTrapsAutoRetryEnabled, "Auto-retry");
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			var player = sender.Mobile as PlayerMobile;
			if (player == null) return;

			player.SendSound(0x4A);
			if (info.ButtonID < 1)
			{
				ReturnToHelp(player);
				return;
			}

			PlayerPreferenceContext prefs = player.Preferences;
			switch ((PageActions)info.ButtonID)
			{
				case PageActions.ModernLockpickingEnabled:
					prefs.ModernLockpickingEnabled = !prefs.ModernLockpickingEnabled;
					break;

				case PageActions.ModernLockpickingAutoRetryEnabled:
					prefs.ModernLockpickingAutoRetryEnabled = !prefs.ModernLockpickingAutoRetryEnabled;
					break;

				case PageActions.ModernRemoveTrapEnabled:
					prefs.ModernRemoveTrapEnabled = !prefs.ModernRemoveTrapEnabled;
					break;

				case PageActions.ModernRemoveTrapsAutoRetryEnabled:
					prefs.ModernRemoveTrapsAutoRetryEnabled = !prefs.ModernRemoveTrapsAutoRetryEnabled;
					break;

				default:
					return;
			}

			player.CloseGump(typeof(ModernSkillSetupGump));
			player.SendGump(new ModernSkillSetupGump(player, m_ReturnPage));
		}

		private void AddToggleRow(int x, int y, int buttonId, bool on, string label)
		{
			int graphic = on ? 4018 : 3609;
			AddButton(x, y, graphic, graphic, buttonId, GumpButtonType.Reply, 0);
			AddHtml(x + 35, y + 3, MAX_CONTENT_WIDTH - 35, 20, TextDefinition.GetColorizedText(label, HtmlColors.WHITE), false, false);
		}

		private void ReturnToHelp(PlayerMobile player)
		{
			player.CloseGump(typeof(Engines.Help.HelpGump));
			player.SendGump(new Engines.Help.HelpGump(player, m_ReturnPage));
		}
	}
}