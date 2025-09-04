using Server;
using Server.Mobiles;
using Server.Targeting;

namespace Scripts.Mythik.Systems.Achievements
{
    public class FeatsTarget : Target
    {
        public FeatsTarget() : base(-1, false, TargetFlags.None)
        {
        }

        protected override void OnTarget(Mobile from, object o)
        {
            Mobile target = o as Mobile;
            if (target != null)
            {
                PlayerMobile targetPlayer = target as PlayerMobile;
                if (targetPlayer == null)
                {
                    from.SendMessage("Only players can have feats!");
                    return;
                }

                if (target == from)
                {
                    AchievementSystem.OpenGump(from, from);
                }
                else
                {
                    PlayerMobile fromPlayer = from as PlayerMobile;
                    if (fromPlayer != null)
                        AchievementSystem.OpenOtherGump(fromPlayer, targetPlayer);
                }
            }
            else
            {
                from.SendMessage("You can only target players to view their feats.");
            }
        }
    }
}