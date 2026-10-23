using Server.Commands;
using Server.Spells;
using Server.Spells.Mystic;
using System.Collections.Generic;

namespace Server.Scripts.Commands
{
	public class CastMonkSpells
	{
		private static List<Docs.DocCommandEntry> m_HelpEntries;

		public static void Initialize()
		{
			m_HelpEntries = CastSpellCommand.RegisterAll(
				"Mystic",
				MysticSpellProvider.SpellDefinitions,
				MysticSpellProvider.Cast,
				definition =>
				{
					var spellNameNormalized = definition.Name.String.Replace(" ", "");
					return new string[] { spellNameNormalized };
				}
			);

			CommandSystem.Register("Spells_Mystic", AccessLevel.Player, new CommandEventHandler(SpellsMystic_OnCommand));
		}

		[Usage("Spells_Mystic")]
		[Description("Lists Mystic spell cast commands.")]
		private static void SpellsMystic_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new HelpInfo.CommandListGump(0, e.Mobile, m_HelpEntries));
		}
	}
}
