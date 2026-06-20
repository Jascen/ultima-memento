
using System;
using Server.Items;
using Server.Network;

namespace Server.SkillHandlers
{
	public class Parrying
	{
		public const int DEXTERITY_PENALTY_THRESHOLD = 80;

		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Parry].Callback = new SkillUseCallback(OnUse);
		}

		public static TimeSpan OnUse(Mobile m)
		{
			var shield = m.FindItemOnLayer(Layer.TwoHanded) as BaseShield;
			if (shield == null)
			{
				m.SendMessage("You can only parry with a shield!");

				return TimeSpan.Zero;
			}

			var skill = m.Skills[SkillName.Parry];

			if (skill.Base < skill.Cap)
				m.CheckSkill(SkillName.Parry, 0.5);

			// Start with 1/3 of player's skill
			var value = skill.Value / 3;

			// Apply a reduction for players with < 125 skill
			const int SKILL_PENALTY_THRESHOLD = 125;
			if (skill.Fixed < SKILL_PENALTY_THRESHOLD && m.Dex < DEXTERITY_PENALTY_THRESHOLD)
				value *= (float)m.Dex / DEXTERITY_PENALTY_THRESHOLD;

			// Guarantee at least 1 point is provided
			var amount = (int)Math.Ceiling(value);

			m.MagicDamageAbsorb += amount;
			m.MeleeDamageAbsorb += amount;

			m.RevealingAction();
			m.PrivateOverheadMessage(MessageType.Regular, 1153, false, "You raise your shield in preparation.", m.NetState);

			Timer.DelayCall(TimeSpan.FromSeconds(3), () =>
			{
				if (0 < m.MagicDamageAbsorb || 0 < m.MeleeDamageAbsorb)
				{
					if (0 < m.MagicDamageAbsorb) m.MagicDamageAbsorb = Math.Max(0, m.MagicDamageAbsorb - amount);
					if (0 < m.MeleeDamageAbsorb) m.MeleeDamageAbsorb = Math.Max(0, m.MeleeDamageAbsorb - amount);
				}

				m.PrivateOverheadMessage(MessageType.Regular, 1153, false, "You relax your stance.", m.NetState);
			});

			return TimeSpan.FromSeconds(5);
		}
	}
}