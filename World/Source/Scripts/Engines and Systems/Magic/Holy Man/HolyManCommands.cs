using Server.Commands;
using Server.Spells;
using Server.Spells.HolyMan;
using System.Collections.Generic;

namespace Server.Scripts.Commands
{
	public class HolyManCommands
	{
		private static List<Docs.DocCommandEntry> m_HelpEntries;

		// Commands that predate the spell names they cast
		private static readonly Dictionary<int, string> m_LegacyCommands = new Dictionary<int, string>
		{
			{ HammerOfFaithSpell.SpellInfo.SpellDefinition.SpellID, "HMHammerFaith" },
			{ TouchOfLifeSpell.SpellInfo.SpellDefinition.SpellID, "HMTouchLife" },
			{ TrialByFireSpell.SpellInfo.SpellDefinition.SpellID, "HMTrialFire" },
		};

		public static void Initialize()
		{
			m_HelpEntries = CastSpellCommand.RegisterAll(
				"Holy Man",
				HolyManSpellProvider.SpellDefinitions,
				HolyManSpellProvider.Cast,
				definition =>
				{
					string legacyCommand;
					if (m_LegacyCommands.TryGetValue(definition.SpellID, out legacyCommand))
						return new string[] { legacyCommand };

					var spellNameNormalized = definition.Name.String.Replace(" ", "");
					return new string[] { string.Format("HM{0}", spellNameNormalized) };
				}
			);

			CommandSystem.Register("Spells_HolyMan", AccessLevel.Player, new CommandEventHandler(SpellsHolyMan_OnCommand));
		}

		[Usage("Spells_HolyMan")]
		[Description("Lists Holy Man spell cast commands.")]
		private static void SpellsHolyMan_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new HelpInfo.CommandListGump(0, e.Mobile, m_HelpEntries));
		}
	}
}
