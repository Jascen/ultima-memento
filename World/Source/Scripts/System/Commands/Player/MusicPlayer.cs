using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Misc;
using Server.Commands;
using Server.Commands.Generic;
using Server.Gumps;

namespace Server.Gumps 
{
    class Musical
    {
        public static void Initialize()
        {
            CommandSystem.Register("musical", AccessLevel.Player, new CommandEventHandler(OnTogglePrivateTime));
        }

        [Usage("musical")]
        [Description("Enables or disables the type of music played in dungeons.")]
        private static void OnTogglePrivateTime(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

			string tunes = ((PlayerMobile)m).Preferences.CharMusical;

            if ( tunes == "Forest" )
            {
				((PlayerMobile)m).Preferences.CharMusical = "Dungeon";
                m.SendMessage(68, "Your dungeon music preference has been set to normal.");
            }
            else
            {
				((PlayerMobile)m).Preferences.CharMusical = "Forest";
                m.SendMessage(68, "Your dungeon music preference has been set to casual.");
            }
        }
    }
}