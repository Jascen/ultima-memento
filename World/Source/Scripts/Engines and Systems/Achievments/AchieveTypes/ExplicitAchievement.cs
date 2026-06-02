using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Scripts.Mythik.Systems.Achievements
{
	public enum ExplicitAchievementType
	{
		ExoticTaste,
	}

	public class ExplicitAchievement : BaseAchievement
	{
		public static Dictionary<ExplicitAchievementType, ExplicitAchievement> Achievements = new Dictionary<ExplicitAchievementType, ExplicitAchievement>();

		public ExplicitAchievement(int id, int catid, int itemIcon, bool hiddenTillComplete, BaseAchievement prereq, string title, string desc, short rewardPoints, params Type[] rewards)
			: base(id, catid, itemIcon, hiddenTillComplete, prereq, title, desc, rewardPoints, 1, rewards)
		{
			HideDesc = true;
			CompletionTotal = 1;
		}

		public static void TryAward(ExplicitAchievementType type, PlayerMobile m)
		{
			if (m == null) return;

			ExplicitAchievement achievement;
			if (!Achievements.TryGetValue(type, out achievement)) return;

			AchievementSystem.SetAchievementStatus(m, achievement, 1);
		}
	}
}
