using Server.Commands;
using Server.Spells;
using Server.Spells.Jedi;
using System.Collections.Generic;

namespace Server.Scripts.Commands
{
	public class CastJediSpells
	{
		private static List<Docs.DocCommandEntry> m_HelpEntries;

		public static void Initialize()
		{
			m_HelpEntries = CastSpellCommand.RegisterAll(
				"Jedi",
				JediSpellProvider.SpellDefinitions,
				JediSpellProvider.Cast,
				definition =>
				{
					var spellNameNormalized = definition.Name.String.Replace(" ", "").Replace("'", "");
					return new string[] { spellNameNormalized };
				}
			);

			CommandSystem.Register("Spells_Jedi", AccessLevel.Player, new CommandEventHandler(SpellsJedi_OnCommand));
		}

		[Usage("Spells_Jedi")]
		[Description("Lists Jedi spell cast commands.")]
		private static void SpellsJedi_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new HelpInfo.CommandListGump(0, e.Mobile, m_HelpEntries));
		}
	}
}
