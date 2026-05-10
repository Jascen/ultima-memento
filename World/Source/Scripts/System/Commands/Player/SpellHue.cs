using Server.Commands;
using Server.Mobiles;

namespace Server.Items
{
    class SpellHue
    {
        public static void Initialize()
        {
            CommandSystem.Register("spellhue", AccessLevel.Player, new CommandEventHandler(OnSpellHueChange));
        }

        [Usage("spellhue [<name>]")]
        [Description("Changes the default color for magery spell effects.")]
        private static void OnSpellHueChange(CommandEventArgs e)
        {
            var from = e.Mobile as PlayerMobile;
			if (from == null) return;

			int hue = 0;

			if (e.Length >= 1){ hue = e.GetInt32(0); }

			from.SendMessage(68, "You have changed your magery spell effects color.");
			from.Preferences.MagerySpellHue = hue;
        }
    }
}
