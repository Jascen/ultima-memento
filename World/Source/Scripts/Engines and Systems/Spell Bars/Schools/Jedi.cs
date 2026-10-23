using Server.Misc;
using Server.Mobiles;
using Server.Spells.Jedi;

namespace Server.SpellBars
{
	public sealed class JediSpellSchool : ISpellSchool
	{
		public static readonly JediSpellSchool Instance = new JediSpellSchool();

		public int MaxSlots
		{ get { return JediSpellProvider.SpellCount; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Jedi; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 11434; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{ return JediSpellProvider.SpellDefinitions[slotIndex - 1].IconGraphic; }

		public string GetName(int slotIndex)
		{
			if (slotIndex < 1 || slotIndex > JediSpellProvider.SpellCount)
				return string.Empty;

			return JediSpellProvider.SpellDefinitions[slotIndex - 1].Name.String;
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return JediSpellProvider.SpellDefinitions[slotIndex - 1].SpellID; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			return GetPlayerInfo.isJedi(from, true) && JediSpellProvider.HasSpell(from, registrySpellId);
		}
	}

	public sealed class SpellBarSetupGump_Jedi_1 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Jedi_1(PlayerMobile from, int origin) : base(SpellBarId.Jedi_1, from, origin)
		{
		}
	}

	public sealed class SpellBarSetupGump_Jedi_2 : SpellBarSetupGump
	{
		public SpellBarSetupGump_Jedi_2(PlayerMobile from, int origin) : base(SpellBarId.Jedi_2, from, origin)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Jedi_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Jedi_1(PlayerMobile from) : base(SpellBarId.Jedi_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Jedi_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Jedi_2(PlayerMobile from) : base(SpellBarId.Jedi_2, from)
		{
		}
	}
}
