using Server.Commands;
using Server.Mobiles;

namespace Server.SpellBars
{
	public class AncientBook
	{
		[Usage("ancient")]
		[Description("Switches ancient magic between book or bag.")]
		public static void AncientBook_OnCommand(CommandEventArgs e)
		{
			Mobile m = e.Mobile;
			var player = m as PlayerMobile;
			if (player == null) return;

			if (!player.Preferences.UsingAncientBook)
			{
				player.Preferences.UsingAncientBook = true;
				m.SendMessage(38, "You are now using the ancient spellbook.");
			}
			else
			{
				player.Preferences.UsingAncientBook = false;
				m.SendMessage(68, "You are now using the research bag.");
			}
		}

		public static void Initialize()
		{
			CommandSystem.Register("ancient", AccessLevel.Player, new CommandEventHandler(AncientBook_OnCommand));
		}

		public static void Register(string command, AccessLevel access, CommandEventHandler handler)
		{
			CommandSystem.Register(command, access, handler);
		}
	}
}