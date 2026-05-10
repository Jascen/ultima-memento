using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Temptation
{
	public class TemptationGump : Gump
	{
		private enum ActionButtonType
		{
			Decline = -3,
			Accept = -2,
			Help = -1,
			Close = 0,
			I_can_take_it,
			Strongest_Avenger,
			Famine,
			Puzzle_master,
			This_is_just_a_tribute,
			Deathwish,
		}

		private const int BORDER_WIDTH = 0;
		private const int GUMP_HEIGHT = 458;
		private const int GUMP_WIDTH = 708;
		private const int HALF_SECTION_INDENT = SECTION_INDENT / 2;
		private const int LABEL_PADDING_BOTTOM = 20;
		private const int SECTION_INDENT = 20;

		private readonly PlayerMobile m_Requester;
		private readonly PlayerMobile m_Target;
		private readonly PlayerContext m_Context;
		private readonly Action m_OnAccept;
		private readonly Action m_OnDecline;

		public TemptationGump(PlayerMobile from, PlayerContext context, PlayerMobile requester) : this(from, context, requester, null, null)
		{
		}

		public TemptationGump(PlayerMobile from, PlayerContext context, PlayerMobile requester, Action onAccept, Action onDecline) : base(25, 25)
		{
			from.CloseGump(typeof(TemptationGump));
			requester.CloseGump(typeof(TemptationGump));

			m_Target = from;
			m_Requester = requester;
			m_Context = context;
			m_OnAccept = onAccept;
			m_OnDecline = onDecline;

			// Add the image multiple times to decrease the transparency
			AddImage(BORDER_WIDTH, BORDER_WIDTH, 7055, 2999);
			AddImage(BORDER_WIDTH, BORDER_WIDTH, 7055, 2999);
			AddAlphaRegion(BORDER_WIDTH, BORDER_WIDTH, GUMP_WIDTH, GUMP_HEIGHT);
			AddImage(BORDER_WIDTH, BORDER_WIDTH, 7055, Server.Misc.PlayerSettings.GetGumpHue(from));
			AddImage(BORDER_WIDTH, BORDER_WIDTH, 7055, Server.Misc.PlayerSettings.GetGumpHue(from));
			AddAlphaRegion(BORDER_WIDTH, BORDER_WIDTH, GUMP_WIDTH, GUMP_HEIGHT);

			AddButton(GUMP_WIDTH - 69, 10, 3610, 3610, (int)ActionButtonType.Help, GumpButtonType.Reply, 0);

			const int SECTION_LABEL_WIDTH = GUMP_WIDTH - HALF_SECTION_INDENT;
			var y = HALF_SECTION_INDENT;

			var canEdit = m_Requester.AccessLevel >= AccessLevel.GameMaster
				|| (m_Target == m_Requester && m_Target.Region != null && m_Target.Region.Name == "the Forest");
			var title = m_Target == m_Requester && canEdit ? "Choose your Temptations" : string.Format("{0}'s Temptations", m_Target.Name);

			int x = SECTION_INDENT;
			if (!canEdit)
			{
				const int LOCK_ICON = 2092;
				AddImage(HALF_SECTION_INDENT, y + 4, LOCK_ICON);
				AddTooltip("Setting can only be changed in the Gypsy Forest");

				x += 5;
				TextDefinition.AddHtmlText(this, x, y, SECTION_LABEL_WIDTH, 20, title, HtmlColors.RED);
				y += 30;
				x += HALF_SECTION_INDENT;
			}
			else
			{
				TextDefinition.AddHtmlText(this, x, y, SECTION_LABEL_WIDTH, 20, title, HtmlColors.RED);
				y += 30;
				x += HALF_SECTION_INDENT;

				TextDefinition.AddHtmlText(this, x, y, GUMP_WIDTH - x - 10, 60, "Once you've been tempted, you may never go back. Choose wisely.", HtmlColors.RED);
				y += 30;
			}


			if (!canEdit && context.Flags == TemptationFlags.None)
				TextDefinition.AddHtmlText(this, x, y, SECTION_LABEL_WIDTH, 20, string.Format("{0} was not tempted.", m_Target.Name), HtmlColors.RED);
			else
			{
				AddOptions(x, ref y, canEdit, context);
			}

			if (canEdit)
			{
				const int CANCEL_BUTTON = 0xFB4;
				const int OK_BUTTON = 0xFB7;

				y = GUMP_HEIGHT - 34;

				x = GUMP_WIDTH - 330;
				AddButton(x, y, CANCEL_BUTTON, CANCEL_BUTTON, (int)ActionButtonType.Decline, GumpButtonType.Reply, 0);
				TextDefinition.AddHtmlText(this, x + 35, y + 3, SECTION_LABEL_WIDTH, 20, "No thank you", HtmlColors.RED);

				if (m_Context.Flags != TemptationFlags.None)
				{
					x = GUMP_WIDTH - 163;
					AddButton(x, y, OK_BUTTON, OK_BUTTON, (int)ActionButtonType.Accept, GumpButtonType.Reply, 0);
					TextDefinition.AddHtmlText(this, x + 35, y + 3, SECTION_LABEL_WIDTH, 20, "I've been Tempted!", HtmlColors.RED);
				}
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID == 0) return;

			// Make sure we create a context if necessary
			switch ((ActionButtonType)info.ButtonID)
			{
				case ActionButtonType.Decline:
					{
						var context = TemptationEngine.Instance.GetOrCreateContext(m_Target);
						context.Flags = TemptationFlags.None;
						TemptationEngine.Instance.ApplyContext(m_Target, context);
						if (m_OnDecline != null)
							m_OnDecline();
						return;
					}

				case ActionButtonType.Accept:
					{
						var context = TemptationEngine.Instance.GetOrCreateContext(m_Target);
						context.Flags = m_Context.Flags;
						TemptationEngine.Instance.ApplyContext(m_Target, context);
						if (m_OnAccept != null)
							m_OnAccept();
						return;
					}

				case ActionButtonType.Help:
					{
						var from = sender.Mobile;
						from.SendGump(new InfoHelpGump(
							from, "Temptations",
							"Command: [Temptations<br><br>Temptations will add or change basic gameplay mechanics for your character. You may be tempted by the powerful benefits, but you should take note of the significant drawbacks. Once you've been tempted, you may never go back. Choose wisely.", true,
							() => m_Requester.SendGump(new TemptationGump(m_Target, m_Context, m_Requester, m_OnAccept, m_OnDecline))
							)
						);
						return;
					}

				case ActionButtonType.I_can_take_it:
					m_Context.IsBerserk = !m_Context.IsBerserk;
					break;

				case ActionButtonType.Deathwish:
					m_Context.HasPermanentDeath = !m_Context.HasPermanentDeath;
					break;

				case ActionButtonType.Strongest_Avenger:
					m_Context.IncreaseMobDifficulty = !m_Context.IncreaseMobDifficulty;
					break;

				case ActionButtonType.Puzzle_master:
					m_Context.CanUsePuzzleboxes = !m_Context.CanUsePuzzleboxes;
					break;

				case ActionButtonType.Famine:
				case ActionButtonType.This_is_just_a_tribute:
					Console.WriteLine("[Temptation] Selected a flag that is not yet implemented: {0}", (ActionButtonType)info.ButtonID);
					return;

				case ActionButtonType.Close:
				default:
					return;
			}

			m_Requester.SendGump(new TemptationGump(m_Target, m_Context, m_Requester, m_OnAccept, m_OnDecline));
		}

		private void AddOption(int x, ref int y, ActionButtonType actionButtonType, bool canEdit, PlayerContext context)
		{
			const int X_BUTTON = 0xFB1;
			const int BLANK_BUTTON = 0xE19;
			var isSelected = GetIsSelected(actionButtonType, context);
			if (!canEdit && !isSelected) return;

			var button = isSelected ? X_BUTTON : BLANK_BUTTON;
			if (canEdit)
			{
				AddButton(x, y, button, button, (int)actionButtonType, GumpButtonType.Reply, 0);
				x += 35;
			}

			var width = GUMP_WIDTH - x;
			TextDefinition.AddHtmlText(this, x, y + 2, width, 20, GetName(actionButtonType), HtmlColors.RED);
			y += LABEL_PADDING_BOTTOM;

			var description = GetDescription(actionButtonType);
			var height = description.Split('<').Length * 20;
			TextDefinition.AddHtmlText(this, x, y + 2, width, height, description, HtmlColors.PALE_RED);
			y += height + 10;
		}

		private void AddOptions(int x, ref int y, bool canEdit, PlayerContext context)
		{
			AddOption(x, ref y, ActionButtonType.Puzzle_master, canEdit, context);
			// AddOption(x, ref y, ActionButtonType.Famine, canEdit, context); // Incomplete
			AddOption(x, ref y, ActionButtonType.I_can_take_it, canEdit, context);
			AddOption(x, ref y, ActionButtonType.Strongest_Avenger, canEdit, context);
			// AddOption(x, ref y, ActionButtonType.This_is_just_a_tribute, canEdit, context); // Incomplete

			if (!m_Target.Avatar.Active || !canEdit)
				AddOption(x, ref y, ActionButtonType.Deathwish, canEdit, context);
		}

		private string GetDescription(ActionButtonType actionButtonType)
		{
			switch (actionButtonType)
			{
				case ActionButtonType.I_can_take_it:
					return "+ You do 10% more damage"
					+ "<br>x You take 20% more damage";

				case ActionButtonType.Strongest_Avenger:
					return "+ You learn how to wear pants"
					+ "<br>x Enemies cast spells faster";

				case ActionButtonType.Famine:
					return "+ Your stat and skill gain rate is impacted by your Hunger"
					+ "<br>x Hunger and thirst decay twice as fast";

				case ActionButtonType.Puzzle_master:
					return "+ You learn how to solve puzzle boxes"
					+ "<br>x Reduced skill cap bonuses for Fugitives (+200) and Titan of Ether quest (+200)"
					+ "<br>x Magical attributes from monster racial bonuses now act as Minimums. Resist/Stat bonuses still stack.";

				case ActionButtonType.This_is_just_a_tribute: return "- Tribute quests rewards are changed";
				case ActionButtonType.Deathwish:
					return "+ You gain stats and skills faster"
					+ "<br>- An old sword"
					+ "<br>x You may never be resurrected";

				default: return string.Empty;
			}
		}

		private bool GetIsSelected(ActionButtonType actionButtonType, PlayerContext context)
		{
			switch (actionButtonType)
			{
				case ActionButtonType.I_can_take_it: return context.Flags.HasFlag(TemptationFlags.I_can_take_it);
				case ActionButtonType.Strongest_Avenger: return context.Flags.HasFlag(TemptationFlags.Strongest_Avenger);
				case ActionButtonType.Famine: return context.Flags.HasFlag(TemptationFlags.Famine);
				case ActionButtonType.Puzzle_master: return context.Flags.HasFlag(TemptationFlags.Puzzle_master);
				case ActionButtonType.This_is_just_a_tribute: return context.Flags.HasFlag(TemptationFlags.This_is_just_a_tribute);
				case ActionButtonType.Deathwish: return context.Flags.HasFlag(TemptationFlags.Deathwish);
				default: return false;
			}
		}

		private string GetName(ActionButtonType actionButtonType)
		{
			switch (actionButtonType)
			{
				case ActionButtonType.I_can_take_it: return "I can take it";
				case ActionButtonType.Strongest_Avenger: return "Strongest Avenger";
				case ActionButtonType.Famine: return "Red warrior needs food, badly!";
				case ActionButtonType.Puzzle_master: return "Puzzle master";
				case ActionButtonType.This_is_just_a_tribute: return "This is just a tribute";
				case ActionButtonType.Deathwish: return "It's dangerous to go alone! Take this.";
				default: return string.Empty;
			}
		}
	}
}