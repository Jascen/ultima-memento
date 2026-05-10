using System;
using Server;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;

namespace Server.Items
{
    class AutoSheatheWeapon
    {
        public static class Config
        {
            public static bool SendOverheadMessage = false; // Should we send a overhead message to the player about the auto-sheathe?
            public static bool AllowPlayerToggle = true;   // Should we allow player to use a command to toggle the auto-sheathe?
        }

        private static Type[] ItemTypesToKeepEquiped = new Type[]
        {
			typeof(JewelryRing),
			typeof(LevelGoldRing),
			typeof(GiftGoldRing),
			typeof(BaseShield),
			typeof(PugilistGlove),
			typeof(PugilistGloves),
			typeof(ThrowingGloves),
			typeof(LevelPugilistGloves),
			typeof(LevelThrowingGloves),
			typeof(GiftPugilistGloves),
			typeof(GiftThrowingGloves),
			typeof(Spellbook)
        };

        private static Dictionary<int, Item> PlayerWeapons = new Dictionary<int, Item>();

        private static List<int> DisabledPlayers = new List<int>();

        public static void Initialize()
        {
            EventSink.Logout += new LogoutEventHandler(OnPlayerLogout);

            if (Config.AllowPlayerToggle)
                CommandSystem.Register("sheathe", AccessLevel.Player, new CommandEventHandler(OnToggleAutoSheathe));
        }

        [Usage("sheathe")]
        [Description("Enables or disables the weapon auto-sheathe feature.")]
        private static void OnToggleAutoSheathe(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

			if ( ((PlayerMobile)m).Preferences.CharacterSheath )
			{
				m.SendMessage(38, "You have disabled the weapon auto-sheathe feature.");
				((PlayerMobile)m).Preferences.CharacterSheath = false;
			}
			else
			{
				m.SendMessage(68, "You have enabled the weapon auto-sheathe feature.");
				((PlayerMobile)m).Preferences.CharacterSheath = true;
			}
        }

        private static void OnPlayerLogout(LogoutEventArgs args)
        {
            PlayerWeapons.Remove(args.Mobile.Serial.Value);
        }

        private static bool AllowedToKeep(Item item)
        {
            Type t = item.GetType();

            for (int i = 0; i < ItemTypesToKeepEquiped.Length; ++i)
                if (ItemTypesToKeepEquiped[i].IsAssignableFrom(t))
                    return true;

			if ( item is BaseEquipableLight )
				return true;

            return false;
        }

        public static void From(Mobile m)
        {
            if (m.Backpack == null)
                return;

            int key = m.Serial.Value;

            if ( Config.AllowPlayerToggle && !((PlayerMobile)m).Preferences.CharacterSheath )
                return;

            Item weapon = m.FindItemOnLayer(Layer.OneHanded);

            if (weapon == null || !weapon.Movable)
                weapon = m.FindItemOnLayer(Layer.TwoHanded);

            Item lastWeapon = null;

            if (PlayerWeapons.ContainsKey(key))
                lastWeapon = PlayerWeapons[key];

            if (m.Warmode)
            {
                if ((weapon == null || AllowedToKeep(weapon)) && lastWeapon != null && lastWeapon.IsChildOf(m.Backpack) && lastWeapon.Movable && lastWeapon.Visible && !lastWeapon.Deleted)
                {
                    m.EquipItem(lastWeapon);

                    if (Config.SendOverheadMessage)
                        m.LocalOverheadMessage(Network.MessageType.Emote, m.EmoteHue, false, "*Unsheathes Weapon*");
                }
            }
            else
            {
                if (weapon != null && !AllowedToKeep(weapon))
                {
                    m.Backpack.DropItem(weapon);
                    PlayerWeapons[key] = weapon;

                    if (Config.SendOverheadMessage)
                        m.LocalOverheadMessage(Network.MessageType.Emote, m.EmoteHue, false, "*Sheathes Weapon*");
                }
            }
        }
    }
}
