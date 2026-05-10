using System;
using Server.Misc;
using Server.Mobiles;

namespace Server
{
    public class Announce
    {
        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(World_Login);
            EventSink.Logout += new LogoutEventHandler(World_Logout);
            EventSink.Disconnected += new DisconnectedEventHandler(World_Leave);
            EventSink.PlayerDeath += new PlayerDeathEventHandler(OnDeath);
        }

        private static void World_Login(LoginEventArgs args)
        {
            Mobile m = args.Mobile;
			PlayerMobile pm = (PlayerMobile)m;

			if ( m.Hue >= 33770 ){ m.Hue = m.Hue - 32768; }

			m.RaceBody();

			if ( pm.Preferences.GumpHue > 0 && m.RecordSkinColor == 0 )
			{
				m.RecordsHair( true );

				pm.Preferences.WeaponBarOpen = true;
				pm.Preferences.GumpHue = pm.RecordSkinColor = 1;
			}

			if ( m.RecordSkinColor >= 33770 ){ m.RecordSkinColor = m.RecordSkinColor - 32768; m.Hue = m.RecordSkinColor; }

			m.RecordFeatures( false );
			m.Stam = m.StamMax;

			if ( !MySettings.S_AllowCustomTitles ){ m.Title = null; }

			LoggingFunctions.LogAccess( m, "login" );

			if ( m.Region.GetLogoutDelay( m ) == TimeSpan.Zero && !m.Poisoned ){ m.Hits = 1000; m.Stam = 1000; m.Mana = 1000; } // FULLY REST UP ON LOGIN
        }

        private static void World_Leave(DisconnectedEventArgs args)
        {
			if ( MySettings.S_SaveOnCharacterLogout ){ World.Save( true, false ); }
        }

        private static void World_Logout(LogoutEventArgs args)
        {
            Mobile m = args.Mobile;
			LoggingFunctions.LogAccess( m, "logout" );
        }
		
        public static void OnDeath(PlayerDeathEventArgs args)
        {
            Mobile m = args.Mobile;
			GhostHelper.OnGhostWalking( m );
        }
    }
}