using Server.Commands;
using Server.Spells;
using Server.Spells.Research;
using System.Collections.Generic;

namespace Server.Scripts.Commands
{
	public class ResearchCommands
	{
		private static List<Docs.DocCommandEntry> m_HelpEntries;

		// Commands that predate the spell names they cast
		private static readonly Dictionary<int, string> m_LegacyCommands = new Dictionary<int, string>
		{
			{ ResearchSummonCreature.SpellInfo.SpellDefinition.SpellID, "CastSpawnCreature" },
			{ ResearchDevastation.SpellInfo.SpellDefinition.SpellID, "CastDevastation" },
		};

		public static void Initialize()
		{
			m_HelpEntries = CastSpellCommand.RegisterAll(
				"Ancient",
				ResearchSpellProvider.SpellDefinitions,
				ResearchSpellProvider.Cast,
				definition =>
				{
					string legacyCommand;
					if (m_LegacyCommands.TryGetValue(definition.SpellID, out legacyCommand))
						return new string[] { legacyCommand };

					var spellNameNormalized = definition.Name.String.Replace(" ", "");
					return new string[] { "Cast" + spellNameNormalized };
				}
			);

			CommandSystem.Register("Spells_Ancient", AccessLevel.Player, new CommandEventHandler(SpellsAncient_OnCommand));
		}

		[Usage("Spells_Ancient")]
		[Description("Lists Ancient spell cast commands.")]
		private static void SpellsAncient_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new HelpInfo.CommandListGump(0, e.Mobile, m_HelpEntries));
		}
	}
}
