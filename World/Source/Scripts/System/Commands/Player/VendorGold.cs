using Server.Mobiles;

namespace Server.Commands
{
    public class VendorGold
    {
        public static void Initialize()
        {
            if (!MySettings.S_RichMerchants && !MySettings.S_UseRemainingGold)
                CommandSystem.Register("VendorGold", AccessLevel.Player, new CommandEventHandler(OnTogglePlayBarbaric));
        }

        [Usage("VendorGold")]
        [Description("Enables or disables the vendor gold safe guard during sales.")]
        private static void OnTogglePlayBarbaric(CommandEventArgs e)
        {
            var player = e.Mobile as PlayerMobile;
            if (player == null) return;

            player.Preferences.IgnoreVendorGoldSafeguard = !player.Preferences.IgnoreVendorGoldSafeguard;

            var message = player.Preferences.IgnoreVendorGoldSafeguard
                ? "Safeguard disabled. Vendors will no longer stop sales they cannot afford."
                : "Vendors will now stop sales if they cannot afford it.";
            player.SendMessage(68, message);
        }
    }
}
