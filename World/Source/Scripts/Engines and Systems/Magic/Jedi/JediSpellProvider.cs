using Server.Items;
using System.Collections.Generic;
using System.Linq;

namespace Server.Spells.Jedi
{
	public class JediSpellProvider
	{
		public static readonly List<SpellDefinition> SpellDefinitions = new List<SpellDefinition>
		{
			ForceGrip.SpellInfo.SpellDefinition,
			MindsEye.SpellInfo.SpellDefinition,
			Mirage.SpellInfo.SpellDefinition,
			ThrowSabre.SpellInfo.SpellDefinition,
			Celerity.SpellInfo.SpellDefinition,
			PsychicAura.SpellInfo.SpellDefinition,
			Deflection.SpellInfo.SpellDefinition,
			SoothingTouch.SpellInfo.SpellDefinition,
			StasisField.SpellInfo.SpellDefinition,
			Replicate.SpellInfo.SpellDefinition,
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
				from.SendLocalizedMessage(500015); // You do not have that spell!
		}

		public static SpellDefinition GetDefinition(int spellID)
		{
			var spellDefinition = SpellDefinitions.FirstOrDefault(definition => definition.SpellID == spellID);

			return spellDefinition;
		}

		public static bool HasSpell(Mobile from, int spellID)
		{
			JediSpellbook book = Spellbook.Find(from, spellID, SpellbookType.Jedi) as JediSpellbook;

			return book != null && book.owner == from && book.HasSpell(spellID);
		}
	}
}
