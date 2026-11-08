using Server.Items;
using System.Collections.Generic;
using System.Linq;

namespace Server.Spells.Syth
{
	public class SythSpellProvider
	{
		public static readonly List<SpellDefinition> SpellDefinitions = new List<SpellDefinition>
		{
			Psychokinesis.SpellInfo.SpellDefinition,
			DeathGrip.SpellInfo.SpellDefinition,
			Projection.SpellInfo.SpellDefinition,
			ThrowSword.SpellInfo.SpellDefinition,
			SythSpeed.SpellInfo.SpellDefinition,
			SythLightning.SpellInfo.SpellDefinition,
			Absorption.SpellInfo.SpellDefinition,
			PsychicBlast.SpellInfo.SpellDefinition,
			DrainLife.SpellInfo.SpellDefinition,
			CloneBody.SpellInfo.SpellDefinition,
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
			SythSpellbook book = Spellbook.Find(from, spellID, SpellbookType.Syth) as SythSpellbook;

			return book != null && book.owner == from && book.HasSpell(spellID);
		}
	}
}
