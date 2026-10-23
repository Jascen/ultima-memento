using Server.Commands;
using Server.Spells;
using Server.Spells.Syth;
using System.Collections.Generic;

namespace Server.Scripts.Commands
{
	public class CastSythSpells
	{
		private static List<Docs.DocCommandEntry> m_HelpEntries;

		// Commands that predate the spell names they cast
		private static readonly Dictionary<int, string> m_LegacyCommands = new Dictionary<int, string>
		{
			{ SythSpeed.SpellInfo.SpellDefinition.SpellID, "SythSpeed" },
			{ CloneBody.SpellInfo.SpellDefinition.SpellID, "CloneBody" },
		};

		public static void Initialize()
		{
			m_HelpEntries = CastSpellCommand.RegisterAll(
				"Syth",
				SythSpellProvider.SpellDefinitions,
				SythSpellProvider.Cast,
				definition =>
				{
					string legacyCommand;
					if (m_LegacyCommands.TryGetValue(definition.SpellID, out legacyCommand))
						return new string[] { legacyCommand };

					var spellNameNormalized = definition.Name.String.Replace(" ", "");
					return new string[] { spellNameNormalized };
				}
			);

			CommandSystem.Register("Spells_Syth", AccessLevel.Player, new CommandEventHandler(SpellsSyth_OnCommand));
		}

		[Usage("Spells_Syth")]
		[Description("Lists Syth spell cast commands.")]
		private static void SpellsSyth_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new HelpInfo.CommandListGump(0, e.Mobile, m_HelpEntries));
		}
	}
}
