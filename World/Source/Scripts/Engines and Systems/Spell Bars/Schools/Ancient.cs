using Server.Mobiles;
using Server.Spells;
using Server.Spells.Research;

namespace Server.SpellBars
{
	public sealed class AncientSpellSchool : ISpellSchool
	{
		public static readonly AncientSpellSchool Instance = new AncientSpellSchool();

		public int MaxSlots
		{ get { return ResearchSpellProvider.SpellCount; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Ancient; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 11193; }

		// Slots follow research order (circle, then school) rather than spell ID order
		public int GetIcon(PlayerMobile from, int slotIndex)
		{ return ResearchSpellProvider.GetDefinitionByResearchIndex(slotIndex).IconGraphic; }

		public string GetName(int slotIndex)
		{
			SpellDefinition definition = ResearchSpellProvider.GetDefinitionByResearchIndex(slotIndex);

			return definition == null ? string.Empty : definition.Name.String;
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return ResearchSpellProvider.GetDefinitionByResearchIndex(slotIndex).SpellID; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			return ResearchSpellProvider.HasSpell(from, registrySpellId);
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