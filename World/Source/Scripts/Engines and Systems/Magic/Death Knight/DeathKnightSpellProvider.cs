using Server.Items;
using System.Collections.Generic;
using System.Linq;

namespace Server.Spells.DeathKnight
{
	public class DeathKnightSpellProvider
	{
		public static readonly List<SpellDefinition> SpellDefinitions = new List<SpellDefinition>
		{
			BanishSpell.SpellInfo.SpellDefinition,
			DemonicTouchSpell.SpellInfo.SpellDefinition,
			DevilPactSpell.SpellInfo.SpellDefinition,
			GrimReaperSpell.SpellInfo.SpellDefinition,
			HagHandSpell.SpellInfo.SpellDefinition,
			HellfireSpell.SpellInfo.SpellDefinition,
			LucifersBoltSpell.SpellInfo.SpellDefinition,
			OrbOfOrcusSpell.SpellInfo.SpellDefinition,
			ShieldOfHateSpell.SpellInfo.SpellDefinition,
			SoulReaperSpell.SpellInfo.SpellDefinition,
			StrengthOfSteelSpell.SpellInfo.SpellDefinition,
			StrikeSpell.SpellInfo.SpellDefinition,
			SuccubusSkinSpell.SpellInfo.SpellDefinition,
			WrathSpell.SpellInfo.SpellDefinition,
		};

		public static int FirstSpellId
		{ get { return SpellDefinitions[0].SpellID; } }

		public static int LastSpellId
		{ get { return SpellDefinitions[SpellDefinitions.Count - 1].SpellID; } }

		public static int SpellCount
		{ get { return SpellDefinitions.Count; } }

		public static void Cast(Mobile from, int spellID)
		{
			if (!Multis.DesignContext.Check(from)) return;

			if (HasSpell(from, spellID))
				SpellRegistry.NewSpell(spellID, from, null).Cast();
			else
				from.SendLocalizedMessage(500015);
		}

		public static SpellDefinition GetDefinition(int spellID)
		{
			var spellDefinition = SpellDefinitions.FirstOrDefault(definition => definition.SpellID == spellID);

			return spellDefinition;
		}

		public static bool HasSpell(Mobile from, int spellID)
		{
			DeathKnightSpellbook book = Spellbook.Find(from, spellID, SpellbookType.DeathKnight) as DeathKnightSpellbook;

			return book != null && book.Owner == from && book.HasSpell(spellID);
		}
	}
}