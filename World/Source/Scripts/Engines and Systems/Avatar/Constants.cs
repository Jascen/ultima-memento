namespace Server.Engines.Avatar
{
	public class Constants
	{
		public const int RIVAL_BONUS_MAX_POINTS = 50 * 1000;
		public const int RIVAL_BONUS_PERCENT = 50;
		public const int SKILL_CAP_BASE = 3000;
		public const int TITAN_SKILL_BONUS = 2000;

		#region Shop Constants

		public const int IMPROVED_TEMPLATE_MAX_COUNT = 5;
		public const int POINT_GAIN_RATE_MAX_LEVEL = 100;
		public const int POINT_GAIN_RATE_PER_LEVEL = 1;
		public const int RECORDED_SKILL_CAP_INTERVAL = 5;
		public const int RECORDED_SKILL_CAP_MAX_AMOUNT = 125;
		public const int RECORDED_SKILL_CAP_MAX_LEVEL = (RECORDED_SKILL_CAP_MAX_AMOUNT - RECORDED_SKILL_CAP_MIN_AMOUNT) / RECORDED_SKILL_CAP_INTERVAL;
		public const int RECORDED_SKILL_CAP_MIN_AMOUNT = 30;
		public const int SAFETY_DEPOSIT_BOX_MAX_LEVEL = 10;
		public const int SKILL_CAP_MAX_LEVEL = 70;
		public const int SKILL_CAP_PER_LEVEL = 10;
		public const int SKILL_GAIN_RATE_MAX_LEVEL = 10;
		public const int SKILL_GAIN_RATE_PER_LEVEL = 5;
		public const int STAT_CAP_MAX_LEVEL = 150;
		public const int STAT_CAP_PER_LEVEL = 1;

		#endregion Shop Constants
	}
}