using Server.Items;
using System.Collections.Generic;
using System.Linq;

namespace Server.Spells.Shinobi
{
	public class ShinobiSpellProvider
	{
		public static readonly List<SpellDefinition> SpellDefinitions = new List<SpellDefinition>
		{
			CheetahPaws.SpellInfo.SpellDefinition,
			Deception.SpellInfo.SpellDefinition,
			EagleEye.SpellInfo.SpellDefinition,
			Espionage.SpellInfo.SpellDefinition,
			FerretFlee.SpellInfo.SpellDefinition,
			MonkeyLeap.SpellInfo.SpellDefinition,
			MysticShuriken.SpellInfo.SpellDefinition,
			TigerStrength.SpellInfo.SpellDefinition,
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
				from.SendMessage("You did not learn that ability.");
		}

		public static SpellDefinition GetDefinition(int spellID)
		{
			var spellDefinition = SpellDefinitions.FirstOrDefault(definition => definition.SpellID == spellID);

			return spellDefinition;
		}

		public static bool HasSpell(Mobile from, int spellID)
		{
			if (GetDefinition(spellID) == null || from.Backpack == null) return false;

			foreach (Item item in from.Backpack.FindItemsByType(typeof(ShinobiScroll), true))
			{
				if (ShinobiScroll.HasAbility(from, spellID, (ShinobiScroll)item))
					return true;
			}

			return false;
		}
	}
}
