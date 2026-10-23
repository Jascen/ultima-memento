using Server.Items;
using System.Collections.Generic;
using System.Linq;

namespace Server.Spells.HolyMan
{
	public class HolyManSpellProvider
	{
		public static readonly List<SpellDefinition> SpellDefinitions = new List<SpellDefinition>
		{
			BanishEvilSpell.SpellInfo.SpellDefinition,
			DampenSpiritSpell.SpellInfo.SpellDefinition,
			EnchantSpell.SpellInfo.SpellDefinition,
			HammerOfFaithSpell.SpellInfo.SpellDefinition,
			HeavenlyLightSpell.SpellInfo.SpellDefinition,
			NourishSpell.SpellInfo.SpellDefinition,
			PurgeSpell.SpellInfo.SpellDefinition,
			RebirthSpell.SpellInfo.SpellDefinition,
			SacredBoonSpell.SpellInfo.SpellDefinition,
			SanctifySpell.SpellInfo.SpellDefinition,
			SeanceSpell.SpellInfo.SpellDefinition,
			SmiteSpell.SpellInfo.SpellDefinition,
			TouchOfLifeSpell.SpellInfo.SpellDefinition,
			TrialByFireSpell.SpellInfo.SpellDefinition,
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
			HolyManSpellbook book = Spellbook.Find(from, spellID, SpellbookType.HolyMan) as HolyManSpellbook;

			return book != null && book.owner == from && book.HasSpell(spellID);
		}
	}
}