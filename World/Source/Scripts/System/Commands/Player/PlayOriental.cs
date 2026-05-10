using Server.Commands;
using Server.Mobiles;

namespace Server.Items
{
    class PlayOriental
    {
        public static void Initialize()
        {
            CommandSystem.Register("oriental", AccessLevel.Player, new CommandEventHandler(OnTogglePlayOriental));
        }

        [Usage("oriental")]
        [Description("Enables or disables the oriental play style.")]
        private static void OnTogglePlayOriental(CommandEventArgs e)
        {
            var m = e.Mobile as PlayerMobile;
			if (m == null) return;

			if ( m.Preferences.CharacterOriental )
			{
				m.SendMessage(38, "You have disabled the oriental play style.");
				m.Preferences.CharacterOriental = false;
			}
			else
			{
				m.SendMessage(68, "You have enabled the oriental play style.");
				m.Preferences.CharacterOriental = true;
				m.Preferences.CharacterEvil = false;
				m.Preferences.CharacterBarbaric = 0;
			}
        }
    }
}
