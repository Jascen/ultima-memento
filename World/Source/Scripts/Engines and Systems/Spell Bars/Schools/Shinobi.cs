using Server.Mobiles;
using Server.Spells.Shinobi;

namespace Server.SpellBars
{
	public sealed class ShinobiSpellSchool : ISpellSchool
	{
		public static readonly ShinobiSpellSchool Instance = new ShinobiSpellSchool();

		public int MaxSlots
		{ get { return ShinobiSpellProvider.SpellCount; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Shinobi; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 10880; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{ return ShinobiSpellProvider.SpellDefinitions[slotIndex - 1].IconGraphic; }

		public string GetName(int slotIndex)
		{
			if (slotIndex < 1 || slotIndex > ShinobiSpellProvider.SpellCount)
				return string.Empty;

			return ShinobiSpellProvider.SpellDefinitions[slotIndex - 1].Name.String;
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return ShinobiSpellProvider.SpellDefinitions[slotIndex - 1].SpellID; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			return ShinobiSpellProvider.HasSpell(from, registrySpellId);
		}
	}

	public sealed class SpellBarSetupGump_Shinobi_1 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Shinobi_1(PlayerMobile from, int origin) : base(SpellBarId.Shinobi_1, from, origin)
		{
		}
	}

	public sealed class SpellBarSetupGump_Shinobi_2 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Shinobi_2(PlayerMobile from, int origin) : base(SpellBarId.Shinobi_2, from, origin)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Shinobi_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Shinobi_1(PlayerMobile from) : base(SpellBarId.Shinobi_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Shinobi_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Shinobi_2(PlayerMobile from) : base(SpellBarId.Shinobi_2, from)
		{
		}
	}
}
