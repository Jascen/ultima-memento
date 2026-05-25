using Server.Items;
using Server.Mobiles;
using Server.Spells.Elementalism;

namespace Server.SpellBars
{
	public sealed class ElementalSpellSchool : ISpellSchool
	{
		public static readonly ElementalSpellSchool Instance = new ElementalSpellSchool();

		public int MaxSlots
		{ get { return 32; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Elemental; } }

		public int GetBackgroundImage(PlayerMobile from)
		{
			int art = 11161;
			int spel = from.CharacterElement;

			if (spel == 1)
				art = 11164;
			else if (spel == 2)
				art = 11163;
			else if (spel == 3)
				art = 11162;

			return art;
		}

		public int GetElementalBookType(PlayerMobile from)
		{
			int book = 0x6717;
			int spel = from.CharacterElement;

			if (spel == 1)
				book = 0x6713;
			else if (spel == 2)
				book = 0x6719;
			else if (spel == 3)
				book = 0x6715;

			return book;
		}

		public int GetIcon(PlayerMobile from, int slotIndex)
		{
			return ElementalSpell.SpellIcon(GetElementalBookType(from), GetRegistrySpellId(slotIndex));
		}

		public string GetName(int slotIndex)
		{
			int registryId = GetRegistrySpellId(slotIndex);
			return ElementalSpell.CommonInfo(registryId, 1);
		}

		public int GetRegistrySpellId(int slotIndex)
		{
			return 300 - 1 + slotIndex;
		}

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			Spellbook book = Spellbook.Find(from, registrySpellId);
			return book != null && book.HasSpell(registrySpellId);
		}
	}

	public sealed class SpellBarSetupGump_Elemental_1 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Elemental_1(PlayerMobile from, int origin) : base(SpellBarId.Elemental_1, from, origin)
		{
		}
	}

	public sealed class SpellBarSetupGump_Elemental_2 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Elemental_2(PlayerMobile from, int origin) : base(SpellBarId.Elemental_2, from, origin)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Elemental_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Elemental_1(PlayerMobile from) : base(SpellBarId.Elemental_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Elemental_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Elemental_2(PlayerMobile from) : base(SpellBarId.Elemental_2, from)
		{
		}
	}
}