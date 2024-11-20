using System;

namespace Server.Engines.MLQuests
{
	public class QuestArea
	{
		private TextDefinition m_Name; // So we can add custom names, different from the Region name

		public TextDefinition Name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}

		public string RegionName { get; private set; }
		public Map ForceMap { get; private set; }

		public QuestArea(TextDefinition name, string region)
			: this(name, region, null)
		{
		}

		public QuestArea(TextDefinition name, string region, Map forceMap)
		{
			m_Name = name;
			RegionName = region;
			ForceMap = forceMap;

			if (MLQuestSystem.Debug)
				ValidationQueue<QuestArea>.Add(this);
		}

		public bool Contains(Mobile mob)
		{
			return Contains(mob.Region);
		}

		public bool Contains(Region reg)
		{
			if (reg == null || (ForceMap != null && reg.Map != ForceMap))
				return false;

			return reg.IsPartOf(RegionName);
		}

		// Debug method
		public void Validate()
		{
			bool found = false;

			foreach (Region r in Region.Regions)
			{
				if (r.Name == RegionName && (ForceMap == null || r.Map == ForceMap))
				{
					found = true;
					break;
				}
			}

			if (!found)
				Console.WriteLine("Warning: QuestArea region '{0}' does not exist (ForceMap = {1})", RegionName, (ForceMap == null) ? "-null-" : ForceMap.ToString());
		}
	}
}
