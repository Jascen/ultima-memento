using Server.Commands;
using Server.Mobiles;

namespace Server.Items
{
    class PlayEvil
    {
        public static void Initialize()
        {
            CommandSystem.Register("evil", AccessLevel.Player, new CommandEventHandler(OnTogglePlayOriental));
        }

        [Usage("evil")]
        [Description("Enables or disables the evil play style.")]
        private static void OnTogglePlayOriental(CommandEventArgs e)
        {
            var m = e.Mobile as PlayerMobile;
			if (m == null) return;

			if ( m.Preferences.CharacterEvil )
			{
				m.SendMessage(38, "You have disabled the evil play style.");
				m.Preferences.CharacterEvil = false;
			}
			else
			{
				m.SendMessage(68, "You have enabled the evil play style.");
				m.Preferences.CharacterEvil = true;
				m.Preferences.CharacterOriental = false;
				m.Preferences.CharacterBarbaric = 0;
			}
        }
    }
}
