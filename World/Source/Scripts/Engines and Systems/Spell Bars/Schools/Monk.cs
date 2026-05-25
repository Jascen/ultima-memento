using Server.Items;
using Server.Mobiles;

namespace Server.SpellBars
{
	public sealed class MonkSpellSchool : ISpellSchool
	{
		public static readonly MonkSpellSchool Instance = new MonkSpellSchool();

		private static readonly int[] SpellIcons =
		{
			0x500E, 0x410, 0x15, 0x971, 0x4B2, 0x5DC2, 0x1A, 0x96D, 0x5001, 0x19
		};

		private static readonly string[] SpellNames =
		{
			"Astral Projection", "Astral Travel", "Create Robe", "Gentle Touch", "Leap",
			"Psionic Blast", "Psychic Wall", "Purity of Body", "Quivering Palm", "Wind Runner"
		};

		public int MaxSlots
		{ get { return 10; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Monk; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 11169; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{ return SpellIcons[slotIndex - 1]; }

		public string GetName(int slotIndex)
		{
			if (slotIndex < 1 || slotIndex > SpellNames.Length)
				return string.Empty;

			return SpellNames[slotIndex - 1];
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return 250 - 1 + slotIndex; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			Spellbook book = Spellbook.Find(from, registrySpellId);

			if (book is MysticSpellbook && ((MysticSpellbook)book).owner != from)
				book = null;

			return book != null && book.HasSpell(registrySpellId);
		}
	}

	public sealed class SpellBarSetupGump_Monk_1 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Monk_1(PlayerMobile from, int origin) : base(SpellBarId.Monk_1, from, origin)
		{
		}
	}

	public sealed class SpellBarSetupGump_Monk_2 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Monk_2(PlayerMobile from, int origin) : base(SpellBarId.Monk_2, from, origin)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Monk_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Monk_1(PlayerMobile from) : base(SpellBarId.Monk_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Monk_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Monk_2(PlayerMobile from) : base(SpellBarId.Monk_2, from)
		{
		}
	}
}