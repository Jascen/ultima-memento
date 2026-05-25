using Server.Items;
using Server.Mobiles;

namespace Server.SpellBars
{
	public sealed class DeathKnightSpellSchool : ISpellSchool
	{
		public static readonly DeathKnightSpellSchool Instance = new DeathKnightSpellSchool();

		private static readonly int[] SpellIcons =
		{
			0x5010, 0x5009, 0x5005, 0x402, 0x5002, 0x3E9, 0x5DC0, 0x1B, 0x3EE, 0x5006,
			0x2B, 0x12, 0x500C, 0x2E
		};

		private static readonly string[] SpellNames =
		{
			"Banish", "Demonic Touch", "Devil Pact", "Grim Reaper", "Hag Hand", "Hellfire",
			"Lucifer's Bolt", "Orb of Orcus", "Shield of Hate", "Soul Reaper", "Strength of Steel",
			"Strike", "Succubus Skin", "Wrath"
		};

		public int MaxSlots
		{ get { return 14; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.DeathKnight; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 11168; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{ return SpellIcons[slotIndex - 1]; }

		public string GetName(int slotIndex)
		{
			if (slotIndex < 1 || slotIndex > SpellNames.Length)
				return string.Empty;

			return SpellNames[slotIndex - 1];
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return 750 - 1 + slotIndex; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			Spellbook book = Spellbook.Find(from, registrySpellId);

			if (book is DeathKnightSpellbook && ((DeathKnightSpellbook)book).Owner != from)
				book = null;

			return book != null && book.HasSpell(registrySpellId);
		}
	}

	public sealed class SpellBarSetupGump_Death_1 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Death_1(PlayerMobile from, int origin) : base(SpellBarId.Death_1, from, origin)
		{
		}
	}

	public sealed class SpellBarSetupGump_Death_2 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Death_2(PlayerMobile from, int origin) : base(SpellBarId.Death_2, from, origin)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Death_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Death_1(PlayerMobile from) : base(SpellBarId.Death_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Death_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Death_2(PlayerMobile from) : base(SpellBarId.Death_2, from)
		{
		}
	}
}