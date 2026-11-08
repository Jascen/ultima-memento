using Server.Items;
using System.Collections.Generic;
using System.Linq;

namespace Server.Spells.Mystic
{
	public class MysticSpellProvider
	{
		public static readonly List<SpellDefinition> SpellDefinitions = new List<SpellDefinition>
		{
			AstralProjection.SpellInfo.SpellDefinition,
			AstralTravel.SpellInfo.SpellDefinition,
			CreateRobe.SpellInfo.SpellDefinition,
			GentleTouch.SpellInfo.SpellDefinition,
			Leap.SpellInfo.SpellDefinition,
			PsionicBlast.SpellInfo.SpellDefinition,
			PsychicWall.SpellInfo.SpellDefinition,
			PurityOfBody.SpellInfo.SpellDefinition,
			QuiveringPalm.SpellInfo.SpellDefinition,
			WindRunner.SpellInfo.SpellDefinition,
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

		public static string GetDescription(SpellDefinition definition)
		{
			string description = definition.Description;

			if (definition.SpellID == WindRunner.SpellInfo.SpellDefinition.SpellID && MySettings.S_NoMountsInCertainRegions)
				description += " Be aware when exploring the land, that there are some areas you cannot use this ability in. These are areas such as dungeons, caves, and some indoor areas. If you enter such an area, this ability will be hindered.";

			return description;
		}

		public static bool HasSpell(Mobile from, int spellID)
		{
			MysticSpellbook book = Spellbook.Find(from, spellID, SpellbookType.Mystic) as MysticSpellbook;

			return book != null && book.owner == from && book.HasSpell(spellID);
		}
	}
}
