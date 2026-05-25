using Server.Misc;
using Server.Mobiles;

namespace Server.SpellBars
{
	public sealed class AncientSpellSchool : ISpellSchool
	{
		public static readonly AncientSpellSchool Instance = new AncientSpellSchool();

		public int MaxSlots
		{ get { return 64; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Ancient; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 11193; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{
			return int.Parse(Research.SpellInformation(slotIndex, 11));
		}

		public string GetName(int slotIndex)
		{
			return Research.SpellInformation(slotIndex, 2);
		}

		public int GetRegistrySpellId(int slotIndex)
		{
			return int.Parse(Research.SpellInformation(slotIndex, 12));
		}

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			for (int index = 1; index <= MaxSlots; index++)
			{
				if (int.Parse(Research.SpellInformation(index, 12)) != registrySpellId)
					continue;

				return ResearchSettings.HasSpell(from, index);
			}

			return false;
		}
	}

	public sealed class SpellBarSetupGump_Ancient_1 : SpellBarSetupPagedGump
	{
		public SpellBarSetupGump_Ancient_1(PlayerMobile from, int origin, int pageNumber = 1) : base(SpellBarId.Ancient_1, from, origin, pageNumber)
		{
		}
	}

	public sealed class SpellBarSetupGump_Ancient_2 : SpellBarSetupPagedGump
	{
		public SpellBarSetupGump_Ancient_2(PlayerMobile from, int origin, int pageNumber = 1) : base(SpellBarId.Ancient_2, from, origin, pageNumber)
		{
		}
	}

	public sealed class SpellBarSetupGump_Ancient_3 : SpellBarSetupPagedGump
	{
		public SpellBarSetupGump_Ancient_3(PlayerMobile from, int origin, int pageNumber = 1) : base(SpellBarId.Ancient_3, from, origin, pageNumber)
		{
		}
	}

	public sealed class SpellBarSetupGump_Ancient_4 : SpellBarSetupPagedGump
	{
		public SpellBarSetupGump_Ancient_4(PlayerMobile from, int origin, int pageNumber = 1) : base(SpellBarId.Ancient_4, from, origin, pageNumber)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Ancient_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Ancient_1(PlayerMobile from) : base(SpellBarId.Ancient_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Ancient_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Ancient_2(PlayerMobile from) : base(SpellBarId.Ancient_2, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Ancient_3 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Ancient_3(PlayerMobile from) : base(SpellBarId.Ancient_3, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Ancient_4 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Ancient_4(PlayerMobile from) : base(SpellBarId.Ancient_4, from)
		{
		}
	}
}