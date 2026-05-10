using System;

namespace Server.Engines.Avatar
{
	public class ActionReward : IReward
	{
		private ActionReward()
		{
		}

		public bool CanSelect { get; set; }
		public int Cost { get; private set; }
		public string Description { get; private set; }
		public int Graphic { get; private set; }
		public bool IsComplete { get; set; }
		public string Name { get; private set; }
		public Action OnSelect { get; set; }
		public string PrequisiteTooltip { get; set; }
		public bool Static { get; set; }

		public static ActionReward Create(int cost, int graphic, string name, string description, Action onSelect)
		{
			return Create(false, cost, graphic, name, description, onSelect);
		}

		public static ActionReward Create(bool isComplete, int cost, int graphic, string name, string description, Action onSelect)
		{
			return new ActionReward
			{
				IsComplete = isComplete,
				Graphic = graphic,
				Name = name,
				Description = description,
				Cost = cost,
				CanSelect = true,
				OnSelect = onSelect,
				Static = false,
			};
		}

		public ActionReward AsStatic()
		{
			Static = true;
			return this;
		}

		public ActionReward WithPrereq(bool hasPrereq, string text)
		{
			if (!hasPrereq)
				PrequisiteTooltip = text;

			return this;
		}
	}
}