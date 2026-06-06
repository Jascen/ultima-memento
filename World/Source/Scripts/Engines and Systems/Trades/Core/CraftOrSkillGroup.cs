using System.Linq;

namespace Server.Engines.Craft
{
	public class CraftOrSkillGroup
	{
		private readonly double m_MaxSkill;
		private readonly double m_MinSkill;
		private readonly SkillName[] m_Skills;

		public CraftOrSkillGroup(double minSkill, double maxSkill, SkillName[] skills)
		{
			m_MinSkill = minSkill;
			m_MaxSkill = maxSkill;
			m_Skills = skills;
		}

		public double MaxSkill
		{
			get { return m_MaxSkill; }
		}

		public double MinSkill
		{
			get { return m_MinSkill; }
		}

		public SkillName[] Skills
		{
			get { return m_Skills; }
		}

		public string GetDisplayName()
		{
			if (m_Skills.Length == 0)
				return "";

			var last = m_Skills.Length - 1;
			var skills = m_Skills.Select(skill => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(SkillInfo.Table[(int)skill].Name));
			var values = string.Join(", or ", string.Join(", ", skills.Take(last)), skills.Last());

			return values;
		}

		public bool MeetsRequirement(Mobile from, bool checkGainRange)
		{
			for (int i = 0; i < m_Skills.Length; i++)
			{
				var val = from.Skills[m_Skills[i]].Value;

				if (val < m_MinSkill)
					continue;

				if (checkGainRange && val >= m_MaxSkill)
					continue;

				return true;
			}

			return false;
		}
	}

	public class CraftOrSkillGroupCol : System.Collections.CollectionBase
	{
		public CraftOrSkillGroupCol()
		{
		}

		public void Add(CraftOrSkillGroup group)
		{
			List.Add(group);
		}

		public CraftOrSkillGroup GetAt(int index)
		{
			return (CraftOrSkillGroup)List[index];
		}
	}
}