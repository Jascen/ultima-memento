using Server.Commands;
using Server.Mobiles;

namespace Server.Items
{
    class PlayBarbaric
    {
        public static void Initialize()
        {
            CommandSystem.Register("barbaric", AccessLevel.Player, new CommandEventHandler(OnTogglePlayBarbaric));
        }

        [Usage("barbaric")]
        [Description("Enables or disables the barbaric play style.")]
        private static void OnTogglePlayBarbaric(CommandEventArgs e)
        {
            var m = e.Mobile as PlayerMobile;
			if (m == null) return;

			if ( m.Preferences.CharacterBarbaric == 1 && m.Female )
			{
				m.SendMessage(68, "You have enabled the barbaric play style with amazon fighter titles.");
				m.Preferences.CharacterBarbaric = 2;
			}
			else if ( m.Preferences.CharacterBarbaric > 0 )
			{
				m.SendMessage(38, "You have disabled the barbaric play style.");
				m.Preferences.CharacterBarbaric = 0;
				Server.Items.BarbaricSatchel.GetRidOf( m );
			}
			else
			{
				m.SendMessage(68, "You have enabled the barbaric play style.");
				m.Preferences.CharacterEvil = false;
				m.Preferences.CharacterOriental = false;
				m.Preferences.CharacterBarbaric = 1;
				Server.Items.BarbaricSatchel.GivePack( m );
			}
        }
    }
}
