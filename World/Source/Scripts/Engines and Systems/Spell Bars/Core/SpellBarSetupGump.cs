using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.SpellBars
{
	public class SpellBarSetupGump : SpellBarSetupGumpBase
	{
		const int SPELL_TOGGLE_OFFSET = 100;

		protected readonly SpellBarId BarId;
		protected readonly ToolBarSpellBarConfiguration Configuration;
		protected readonly ISpellSchool School;

		public SpellBarSetupGump(SpellBarId barId, PlayerMobile from, int origin)
			: base(from, origin, SpellBarRegistry.GetDefinition(barId))
		{
			BarId = barId;
			School = SpellBarRegistry.GetDefinition(barId).SchoolInstance;
			Configuration = SpellBarRegistry.CreateConfiguration(from, barId);
		}

		public override bool ConfigureGump()
		{
			AddGlobalConfig(Configuration.IsVertical, Configuration.ShowName);

			int x = 75;
			int y = 135;

			for (int i = 0; i < Configuration.MaxSlots;)
			{
				int spellIndex = i + 1;
				string spellName = School.GetName(spellIndex);

				if (string.IsNullOrWhiteSpace(spellName))
					break;

				bool isChecked = Configuration.IsSpellEnabled(spellIndex);
				AddSpell(isChecked, SPELL_TOGGLE_OFFSET + spellIndex, School.GetIcon(Player, spellIndex), spellName, ref i, ref x, ref y);
			}

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
				else if (SPELL_TOGGLE_OFFSET <= info.ButtonID)
					Configuration.ToggleSpellEnabled(info.ButtonID - SPELL_TOGGLE_OFFSET);
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
				var gump = SpellBarRegistry.CreateSetupGump(BarId, from, Origin);
				if (gump != null && gump.ConfigureGump())
				{
					from.CloseGump(SpellBarRegistry.GetSetupGumpType(BarId));
					from.SendGump(gump);
				}

				from.SendSound(0x4A);
			}
		}
	}
}