using Server.Misc;
using Server.Mobiles;
using Server.Spells.Jester;

namespace Server.SpellBars
{
	public sealed class JesterSpellSchool : ISpellSchool
	{
		public static readonly JesterSpellSchool Instance = new JesterSpellSchool();

		public int MaxSlots
		{ get { return JesterSpellProvider.SpellCount; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Jester; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 10865; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{ return JesterSpellProvider.SpellDefinitions[slotIndex - 1].IconGraphic; }

		public string GetName(int slotIndex)
		{
			if (slotIndex < 1 || slotIndex > JesterSpellProvider.SpellCount)
				return string.Empty;

			return JesterSpellProvider.SpellDefinitions[slotIndex - 1].Name.String;
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return JesterSpellProvider.SpellDefinitions[slotIndex - 1].SpellID; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			return GetPlayerInfo.isJester(from) && JesterSpellProvider.HasSpell(from, registrySpellId);
		}
	}

	public sealed class SpellBarSetupGump_Jester_1 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Jester_1(PlayerMobile from, int origin) : base(SpellBarId.Jester_1, from, origin)
		{
		}
	}

	public sealed class SpellBarSetupGump_Jester_2 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Jester_2(PlayerMobile from, int origin) : base(SpellBarId.Jester_2, from, origin)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Jester_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Jester_1(PlayerMobile from) : base(SpellBarId.Jester_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Jester_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Jester_2(PlayerMobile from) : base(SpellBarId.Jester_2, from)
		{
		}
	}
}
