using Server.Mobiles;
using Server.Spells.Mystic;

namespace Server.SpellBars
{
	public sealed class MonkSpellSchool : ISpellSchool
	{
		public static readonly MonkSpellSchool Instance = new MonkSpellSchool();

		public int MaxSlots
		{ get { return MysticSpellProvider.SpellCount; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Monk; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 11169; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{ return MysticSpellProvider.SpellDefinitions[slotIndex - 1].IconGraphic; }

		public string GetName(int slotIndex)
		{
			if (slotIndex < 1 || slotIndex > MysticSpellProvider.SpellCount)
				return string.Empty;

			return MysticSpellProvider.SpellDefinitions[slotIndex - 1].Name.String;
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return MysticSpellProvider.SpellDefinitions[slotIndex - 1].SpellID; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			return MysticSpellProvider.HasSpell(from, registrySpellId);
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