using Server.Items;
using Server.Mobiles;

namespace Server.SpellBars
{
	public sealed class NecromancySpellSchool : ISpellSchool
	{
		public static readonly NecromancySpellSchool Instance = new NecromancySpellSchool();

		private static readonly string[] SpellNames =
		{
			"Animate Dead", "Blood Oath", "Corpse Skin", "Curse Weapon", "Evil Omen", "Horrific Beast",
			"Lich Form", "Mind Rot", "Pain Spike", "Poison Strike", "Strangle", "Summon Familiar",
			"Vampiric Embrace", "Vengeful Spirit", "Wither", "Wraith Form", "Exorcism"
		};

		public int MaxSlots
		{ get { return 17; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Necromancy; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 11170; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{ return 20480 - 1 + slotIndex; }

		public string GetName(int slotIndex)
		{
			if (slotIndex < 1 || slotIndex > SpellNames.Length)
				return string.Empty;

			return SpellNames[slotIndex - 1];
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return 100 - 1 + slotIndex; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			Spellbook book = Spellbook.Find(from, registrySpellId);
			return book != null && book.HasSpell(registrySpellId);
		}
	}

	public sealed class SpellBarSetupGump_Necro_1 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Necro_1(PlayerMobile from, int origin) : base(SpellBarId.Necro_1, from, origin)
		{
		}
	}

	public sealed class SpellBarSetupGump_Necro_2 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Necro_2(PlayerMobile from, int origin) : base(SpellBarId.Necro_2, from, origin)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Necro_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Necro_1(PlayerMobile from) : base(SpellBarId.Necro_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Necro_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Necro_2(PlayerMobile from) : base(SpellBarId.Necro_2, from)
		{
		}
	}
}