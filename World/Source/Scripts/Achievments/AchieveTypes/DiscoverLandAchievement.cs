using Server;
using System;

namespace Scripts.Mythik.Systems.Achievements
{
    public class DiscoverLandAchievement : BaseAchievement
    {
        private readonly Land m_Land;

        public DiscoverLandAchievement(int id, int catid, int itemIcon, bool hiddenTillComplete, BaseAchievement prereq, string title, string desc, short rewardPoints, Land land, params Type[] rewards)
            : base(id, catid, itemIcon, hiddenTillComplete, prereq, title, desc, rewardPoints, 1, rewards)
        {
            HideDesc = true;
            m_Land = land;
            CompletionTotal = 1;
            CustomEventSink.LandChanged += EventSink_OnLandChanged;
        }

        private void EventSink_OnLandChanged(LandChangedArgs e)
        {
            if (e == null || e.Mobile == null) return;

            if (e.NewLand == m_Land)
            {
                AchievementSystem.SetAchievementStatus(e.Mobile, this, 1);
            }
        }
    }
}
