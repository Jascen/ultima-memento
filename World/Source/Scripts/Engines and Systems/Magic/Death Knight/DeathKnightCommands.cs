
using Server.Spells;
using Server.Spells.DeathKnight;
using Server.Commands;
using System.Collections.Generic;

namespace Server.Scripts.Commands
{
	public class DeathKnightCommands
	{
		private static List<Docs.DocCommandEntry> m_HelpEntries;

		// Commands that predate the spell names they cast
		private static readonly Dictionary<int, string> m_LegacyCommands = new Dictionary<int, string>
		{
			{ HellfireSpell.SpellInfo.SpellDefinition.SpellID, "DKHellfire" },
			{ OrbOfOrcusSpell.SpellInfo.SpellDefinition.SpellID, "DKOrbOrcus" },
			{ ShieldOfHateSpell.SpellInfo.SpellDefinition.SpellID, "DKShieldHate" },
			{ StrengthOfSteelSpell.SpellInfo.SpellDefinition.SpellID, "DKStrengthSteel" },
		};

		public static void Initialize()
		{
			m_HelpEntries = CastSpellCommand.RegisterAll(
				"Death Knight",
				DeathKnightSpellProvider.SpellDefinitions,
				DeathKnightSpellProvider.Cast,
				definition =>
				{
					string legacyCommand;
					if (m_LegacyCommands.TryGetValue(definition.SpellID, out legacyCommand))
						return new string[] { legacyCommand };

					var spellNameNormalized = definition.Name.String.Replace(" ", "").Replace("'", "");
					return new string[] { string.Format("DK{0}", spellNameNormalized) };
				}
			);

			CommandSystem.Register("Spells_DeathKnight", AccessLevel.Player, new CommandEventHandler(SpellsDeathKnight_OnCommand));
		}

		[Usage("Spells_DeathKnight")]
		[Description("Lists Death Knight spell cast commands.")]
		private static void SpellsDeathKnight_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new HelpInfo.CommandListGump(0, e.Mobile, m_HelpEntries));
		}
	}
}
