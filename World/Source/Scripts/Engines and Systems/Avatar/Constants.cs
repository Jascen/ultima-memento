namespace Server.Engines.Avatar
{
	public class Constants
	{
		public const string AVATAR_GYPSY_TEXT = @"You may choose the path of the Avatar, where each life is but one step upon a greater Ascent. If you draw this card, your story begins at this gypsy encampment in the forest, where fate has gathered you before you are cast into Sosaria. Here you are weak and nearly bereft of skill, yet you carry a blessed tome called The Avatar's Ascent.<br><br>Before you leave this sanctuary, choose a Template to guide your rebirth, and consider your Ascensions carefully. Ascensions are permanent gifts to your lineage that may preserve knowledge between lives, unlock greater potential, or soften the harsh truth that awaits you outside these lanterns. Unless an Ascension says otherwise, everything you own is lost when you die, and you cannot be resurrected as others are.<br><br>Death will end your current life and return you here to begin anew, while the coins you farmed are banked for your next preparations. Your house alone may endure, passed to whoever inherits your name. Once you step beyond the encampment, you may read your tome but you may purchase nothing more from it until fate brings you back.<br><br>This is a challenging journey of self-discovery. Each run is temporary; each death is a lesson. Those who endure may raise their stat and skill ceilings, master Primary and Secondary skills from past lives, and someday claim vengeance against the faction that wronged their family. Will you begin the Avatar's Ascent?";

		public const int RIVAL_BONUS_MAX_POINTS = 50 * 1000;
		public const int RIVAL_BONUS_PERCENT = 50;
		public const int SKILL_CAP_BASE = 3000;
		public const int TITAN_SKILL_BONUS = 2000;

		#region Shop Constants

		public const int BOAT_SPEED_MAX_LEVEL = Multis.BaseBoat.MAX_SPEED_BOOSTS;
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