using Server.Misc;
using Server.Mobiles;
using Server.Spells.Syth;

namespace Server.SpellBars
{
	public sealed class SythSpellSchool : ISpellSchool
	{
		public static readonly SythSpellSchool Instance = new SythSpellSchool();

		public int MaxSlots
		{ get { return SythSpellProvider.SpellCount; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Syth; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 11427; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{ return SythSpellProvider.SpellDefinitions[slotIndex - 1].IconGraphic; }

		public string GetName(int slotIndex)
		{
			if (slotIndex < 1 || slotIndex > SythSpellProvider.SpellCount)
				return string.Empty;

			return SythSpellProvider.SpellDefinitions[slotIndex - 1].Name.String;
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return SythSpellProvider.SpellDefinitions[slotIndex - 1].SpellID; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			return GetPlayerInfo.isSyth(from, true) && SythSpellProvider.HasSpell(from, registrySpellId);
		}
	}

	public sealed class SpellBarSetupGump_Syth_1 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Syth_1(PlayerMobile from, int origin) : base(SpellBarId.Syth_1, from, origin)
		{
		}
	}

	public sealed class SpellBarSetupGump_Syth_2 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Syth_2(PlayerMobile from, int origin) : base(SpellBarId.Syth_2, from, origin)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Syth_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Syth_1(PlayerMobile from) : base(SpellBarId.Syth_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Syth_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Syth_2(PlayerMobile from) : base(SpellBarId.Syth_2, from)
		{
		}
	}
}
