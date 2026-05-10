using System;
using Server;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;

namespace Server.Items
{
    class ClassicPoison
    {
        public static void Initialize()
        {
            CommandSystem.Register("poisons", AccessLevel.Player, new CommandEventHandler(OnTogglePlayOriental));
        }

        [Usage("poisons")]
        [Description("Enables or disables the classic poisoning.")]
        private static void OnTogglePlayOriental(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

			if ( ((PlayerMobile)m).Preferences.ClassicPoisoning )
			{
				m.SendMessage(38, "Poisons are now set for precision with special weapon infectious strikes.");
				((PlayerMobile)m).Preferences.ClassicPoisoning = false;
			}
			else
			{
				m.SendMessage(68, "Poisons are now set with hits from one-handed slashing or piercing weapons.");
				((PlayerMobile)m).Preferences.ClassicPoisoning = true;
			}
        }
    }
}
