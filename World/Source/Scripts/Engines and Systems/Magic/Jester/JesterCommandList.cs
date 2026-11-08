using Server.Commands;
using Server.Spells;
using Server.Spells.Jester;
using System.Collections.Generic;

namespace Server.Scripts.Commands
{
	public class CastJesterSpells
	{
		private static List<Docs.DocCommandEntry> m_HelpEntries;

		public static void Initialize()
		{
			m_HelpEntries = CastSpellCommand.RegisterAll(
				"Jester",
				JesterSpellProvider.SpellDefinitions,
				JesterSpellProvider.Cast,
				definition =>
				{
					var spellNameNormalized = definition.Name.String.Replace(" ", "");
					return new string[] { spellNameNormalized };
				}
			);

			CommandSystem.Register("Spells_Jester", AccessLevel.Player, new CommandEventHandler(SpellsJester_OnCommand));
		}

		[Usage("Spells_Jester")]
		[Description("Lists Jester spell cast commands.")]
		private static void SpellsJester_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new HelpInfo.CommandListGump(0, e.Mobile, m_HelpEntries));
		}
	}
}
