using Server.Items;
using Server.Mobiles;

namespace Server.SpellBars
{
	public sealed class PriestSpellSchool : ISpellSchool
	{
		public static readonly PriestSpellSchool Instance = new PriestSpellSchool();

		private static readonly int[] SpellIcons =
		{
			0x965, 0x966, 0x967, 0x968, 0x969, 0x96A, 0x96B, 0x96C, 0x96E, 0x96D, 0x96F, 0x970, 0x971, 0x972
		};

		private static readonly string[] SpellNames =
		{
			"Banish", "Dampen Spirit", "Enchant", "Hammer of Faith", "Heavenly Light", "Nourish",
			"Purge", "Rebirth", "Sacred Boon", "Sactify", "Seance", "Smite", "Touch of Life", "Trial by Fire"
		};

		public int MaxSlots
		{ get { return 14; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Priest; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 11171; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{ return SpellIcons[slotIndex - 1]; }

		public string GetName(int slotIndex)
		{
			if (slotIndex < 1 || slotIndex > SpellNames.Length)
				return string.Empty;

			return SpellNames[slotIndex - 1];
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return 770 - 1 + slotIndex; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			Spellbook book = Spellbook.Find(from, registrySpellId);

			if (book is HolyManSpellbook && ((HolyManSpellbook)book).owner != from)
				book = null;

			return book != null && book.HasSpell(registrySpellId);
		}
	}

	public sealed class SpellBarSetupGump_Priest_1 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Priest_1(PlayerMobile from, int origin) : base(SpellBarId.Priest_1, from, origin)
		{
		}
	}

	public sealed class SpellBarSetupGump_Priest_2 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Priest_2(PlayerMobile from, int origin) : base(SpellBarId.Priest_2, from, origin)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Priest_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Priest_1(PlayerMobile from) : base(SpellBarId.Priest_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Priest_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Priest_2(PlayerMobile from) : base(SpellBarId.Priest_2, from)
		{
		}
	}
}