using Server.Items;
using Server.Mobiles;

namespace Server.SpellBars
{
	public sealed class BardSpellSchool : ISpellSchool
	{
		public static readonly BardSpellSchool Instance = new BardSpellSchool();

		private static readonly string[] SpellNames =
		{
			"Army's Paeon", "Enchanting Etude", "Energy Carol", "Energy Threnody", "Fire Carol", "Fire Threnody",
			"Foe Requiem", "Ice Carol", "Ice Threnody", "Knight's Minne", "Mage's Ballad", "Magic Finale",
			"Poison Carol", "Poison Threnody", "Shepherd's Dance", "Sinewy Etude"
		};

		public int MaxSlots
		{ get { return 16; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Bard; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 11165; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{
			if (11 < slotIndex)
				slotIndex += 1;

			return 1028 - 1 + slotIndex;
		}

		public string GetName(int slotIndex)
		{
			if (slotIndex < 1 || slotIndex > SpellNames.Length)
				return string.Empty;

			return SpellNames[slotIndex - 1];
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return 351 - 1 + slotIndex; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			Spellbook book = Spellbook.Find(from, registrySpellId);
			return book != null && book.HasSpell(registrySpellId);
		}
	}

	public sealed class SpellBarSetupGump_Bard_1 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Bard_1(PlayerMobile from, int origin) : base(SpellBarId.Bard_1, from, origin)
		{
		}
	}

	public sealed class SpellBarSetupGump_Bard_2 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Bard_2(PlayerMobile from, int origin) : base(SpellBarId.Bard_2, from, origin)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Bard_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Bard_1(PlayerMobile from) : base(SpellBarId.Bard_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Bard_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Bard_2(PlayerMobile from) : base(SpellBarId.Bard_2, from)
		{
		}
	}
}