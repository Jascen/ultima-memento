using Server.Items;
using Server.Mobiles;

namespace Server.SpellBars
{
	public sealed class KnightSpellSchool : ISpellSchool
	{
		public static readonly KnightSpellSchool Instance = new KnightSpellSchool();

		private static readonly string[] SpellNames =
		{
			"Cleanse by Fire", "Close Wounds", "Consecrate Weapon", "Dispel Evil", "Divine Fury",
			"Enemy of One", "Holy Light", "Noble Sacrifice", "Remove Curse", "Sacred Journey"
		};

		public int MaxSlots
		{ get { return 10; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Knight; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 11167; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{ return 20736 - 1 + slotIndex; }

		public string GetName(int slotIndex)
		{
			if (slotIndex < 1 || slotIndex > SpellNames.Length)
				return string.Empty;

			return SpellNames[slotIndex - 1];
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return 200 - 1 + slotIndex; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			Spellbook book = Spellbook.Find(from, registrySpellId);
			return book != null && book.HasSpell(registrySpellId);
		}
	}

	public sealed class SpellBarSetupGump_Knight_1 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Knight_1(PlayerMobile from, int origin) : base(SpellBarId.Knight_1, from, origin)
		{
		}
	}

	public sealed class SpellBarSetupGump_Knight_2 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Knight_2(PlayerMobile from, int origin) : base(SpellBarId.Knight_2, from, origin)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Knight_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Knight_1(PlayerMobile from) : base(SpellBarId.Knight_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Knight_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Knight_2(PlayerMobile from) : base(SpellBarId.Knight_2, from)
		{
		}
	}
}