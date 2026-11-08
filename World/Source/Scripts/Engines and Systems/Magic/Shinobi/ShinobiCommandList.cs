using Server.Commands;
using Server.Spells;
using Server.Spells.Shinobi;
using System.Collections.Generic;

namespace Server.Scripts.Commands
{
	public class CastShinobiSpells
	{
		private static List<Docs.DocCommandEntry> m_HelpEntries;

		public static void Initialize()
		{
			m_HelpEntries = CastSpellCommand.RegisterAll(
				"Shinobi",
				ShinobiSpellProvider.SpellDefinitions,
				ShinobiSpellProvider.Cast,
				definition =>
				{
					var spellNameNormalized = definition.Name.String.Replace(" ", "");
					return new string[] { spellNameNormalized };
				}
			);

			CommandSystem.Register("Spells_Shinobi", AccessLevel.Player, new CommandEventHandler(SpellsShinobi_OnCommand));
		}

		[Usage("Spells_Shinobi")]
		[Description("Lists Shinobi spell cast commands.")]
		private static void SpellsShinobi_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new HelpInfo.CommandListGump(0, e.Mobile, m_HelpEntries));
		}
	}
}
