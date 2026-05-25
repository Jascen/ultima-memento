using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.SpellBars
{
	public class SpellBarSetupPagedGump : SpellBarSetupGumpBase
	{
		const int PAGE_NEXT_BUTTON = 102;
		const int PAGE_PREVIOUS_BUTTON = 101;
		const int SPELLS_PER_PAGE = 32;

		protected readonly SpellBarId BarId;
		protected readonly ToolBarSpellBarConfiguration Configuration;
		protected readonly int PageNumber;
		protected readonly ISpellSchool School;

		public SpellBarSetupPagedGump(SpellBarId barId, PlayerMobile from, int origin, int pageNumber)
			: base(from, origin, SpellBarRegistry.GetDefinition(barId))
		{
			BarId = barId;
			PageNumber = pageNumber;
			School = SpellBarRegistry.GetDefinition(barId).SchoolInstance;
			Configuration = SpellBarRegistry.CreateConfiguration(from, barId);
		}

		public override bool ConfigureGump()
		{
			AddGlobalConfig(Configuration.IsVertical, Configuration.ShowName);

			int x = 75;
			int y = 135;
			int startIndex = PageNumber == 1 ? 0 : SPELLS_PER_PAGE;
			int endIndex = startIndex + SPELLS_PER_PAGE;

			for (int i = startIndex; i < endIndex && i < Configuration.MaxSlots;)
			{
				int spellIndex = i + 1;
				string spellName = School.GetName(spellIndex);

				if (string.IsNullOrWhiteSpace(spellName))
					break;

				bool isChecked = Configuration.IsSpellEnabled(spellIndex);
				AddSpell(isChecked, spellIndex, School.GetIcon(Player, spellIndex), spellName, ref i, ref x, ref y);
			}

			if (PageNumber == 1)
				AddNextPageButton(PAGE_NEXT_BUTTON);
			else
				AddPreviousPageButton(PAGE_PREVIOUS_BUTTON);

			return true;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			PlayerMobile from = (PlayerMobile)sender.Mobile;

			if (0 < info.ButtonID)
			{
				if (info.ButtonID == 90)
					Configuration.ToggleShowName();
				else if (info.ButtonID == 91)
					Configuration.ToggleIsVertical();
				else if (info.ButtonID == 92)
					InvokeCommand(OpenCommand, from);
				else if (info.ButtonID == 93)
					InvokeCommand(CloseCommand, from);
				else if (info.ButtonID < 90)
					Configuration.ToggleSpellEnabled(info.ButtonID);
				else if (info.ButtonID == PAGE_PREVIOUS_BUTTON)
				{
					ReopenGump(from, 1);
					return;
				}
				else if (info.ButtonID == PAGE_NEXT_BUTTON)
				{
					ReopenGump(from, 2);
					return;
				}
			}

			if (info.ButtonID < 1 && Origin > 0)
			{
				from.SendGump(new Server.Engines.Help.HelpGump(from, 7));
				from.SendSound(0x4A);
			}
			else if (info.ButtonID < 1)
			{
			}
			else
			{
				var gump = SpellBarRegistry.CreateSetupGump(BarId, from, Origin, PageNumber);
				if (gump != null && gump.ConfigureGump())
				{
					from.CloseGump(SpellBarRegistry.GetSetupGumpType(BarId));
					from.SendGump(gump);
				}

				from.SendSound(0x4A);
			}
		}

		protected void ReopenGump(PlayerMobile from, int pageNumber)
		{
			var gump = SpellBarRegistry.CreateSetupGump(BarId, from, Origin, pageNumber);
			if (gump == null || !gump.ConfigureGump()) return;

			from.CloseGump(SpellBarRegistry.GetSetupGumpType(BarId));
			from.SendGump(gump);
		}
	}
}