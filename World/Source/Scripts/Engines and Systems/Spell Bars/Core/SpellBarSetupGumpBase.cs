using Server.Commands;
using Server.Gumps;
using Server.Mobiles;

namespace Server.SpellBars
{
	public abstract class SpellBarSetupGumpBase : Gump
	{
		public readonly PlayerMobile Player;

		protected readonly string CloseCommand;
		protected readonly string OpenCommand;
		protected readonly int Origin;
		protected readonly string Title;

		protected SpellBarSetupGumpBase(PlayerMobile from, int origin, ISpellBarDescriptor descriptor)
			: base(12, 50)
		{
			Player = from;
			Origin = origin;
			Title = descriptor.Title;
			OpenCommand = descriptor.ToolCommand;
			CloseCommand = descriptor.CloseCommand;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
		}

		public abstract bool ConfigureGump();

		protected void AddGlobalConfig(bool useVerticalBar, bool showSpellNamesWhenVertical)
		{
			AddImage(38, 0, 9578, Server.Misc.PlayerSettings.GetGumpHue(Player));
			AddButton(897, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);
			AddLabel(52, 14, LabelColors.OFFWHITE, Title);

			int useHorizontalBarGraphic = !useVerticalBar ? 4018 : 3609;
			int useVerticalBarGraphic = useVerticalBar ? 4018 : 3609;
			int showSpellNamesWhenVerticalGraphic = showSpellNamesWhenVertical ? 4018 : 3609;

			AddButton(75, 52, useHorizontalBarGraphic, useHorizontalBarGraphic, 91, GumpButtonType.Reply, 0);
			AddLabel(115, 55, LabelColors.OFFWHITE, "Horizontal Bar");

			AddButton(75, 82, useVerticalBarGraphic, useVerticalBarGraphic, 91, GumpButtonType.Reply, 0);
			AddLabel(115, 85, LabelColors.OFFWHITE, "Vertical Bar");

			AddButton(225, 82, showSpellNamesWhenVerticalGraphic, showSpellNamesWhenVerticalGraphic, 90, GumpButtonType.Reply, 0);
			AddLabel(265, 85, LabelColors.OFFWHITE, "Show Spell Names When Vertical");

			AddButton(500, 52, 4005, 4007, 92, GumpButtonType.Reply, 0);
			AddLabel(540, 55, LabelColors.OFFWHITE, "Open Toolbar");

			AddButton(500, 82, 4020, 4020, 93, GumpButtonType.Reply, 0);
			AddLabel(540, 85, LabelColors.OFFWHITE, "Close Toolbar");
		}

		protected void AddNextPageButton(int buttonId)
		{
			AddButton(897, 569, 4005, 4007, buttonId, GumpButtonType.Reply, 0); // Next Page
		}

		protected void AddPreviousPageButton(int buttonId)
		{
			AddButton(50, 569, 4014, 4015, buttonId, GumpButtonType.Reply, 0); // Previous Page
		}

		protected void AddSpell(bool isChecked, int buttonId, int spellGraphicId, string spellName, ref int i, ref int x, ref int y)
		{
			int checkboxGraphic = isChecked ? 4018 : 3609;
			AddButton(x, y + 10, checkboxGraphic, checkboxGraphic, buttonId, GumpButtonType.Reply, 0);
			AddButton(x + 40, y, spellGraphicId, spellGraphicId, buttonId, GumpButtonType.Reply, 0);
			AddLabel(x + 100, y + 12, LabelColors.OFFWHITE, spellName);

			// Next column
			if (++i % 8 == 0)
			{
				x += 210;
				y = 135;
			}
			else
			{
				// Next row
				y += 50;
			}
		}

		protected void InvokeCommand(string c, Mobile from)
		{
			CommandSystem.Handle(from, string.Format("{0}{1}", CommandSystem.Prefix, c));
		}
	}
}