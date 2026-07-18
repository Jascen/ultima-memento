using System;

namespace Server.SpellBars
{
	public static partial class SpellBarRegistry
	{
		public static Type GetSetupGumpType(SpellBarId id)
		{
			switch (id)
			{
				case SpellBarId.Ancient_1: return typeof(SpellBarSetupGump_Ancient_1);
				case SpellBarId.Ancient_2: return typeof(SpellBarSetupGump_Ancient_2);
				case SpellBarId.Ancient_3: return typeof(SpellBarSetupGump_Ancient_3);
				case SpellBarId.Ancient_4: return typeof(SpellBarSetupGump_Ancient_4);
				case SpellBarId.Bard_1: return typeof(SpellBarSetupGump_Bard_1);
				case SpellBarId.Bard_2: return typeof(SpellBarSetupGump_Bard_2);
				case SpellBarId.Death_1: return typeof(SpellBarSetupGump_Death_1);
				case SpellBarId.Death_2: return typeof(SpellBarSetupGump_Death_2);
				case SpellBarId.Elemental_1: return typeof(SpellBarSetupGump_Elemental_1);
				case SpellBarId.Elemental_2: return typeof(SpellBarSetupGump_Elemental_2);
				case SpellBarId.Knight_1: return typeof(SpellBarSetupGump_Knight_1);
				case SpellBarId.Knight_2: return typeof(SpellBarSetupGump_Knight_2);
				case SpellBarId.Mage_1: return typeof(SpellBarSetupGump_Mage_1);
				case SpellBarId.Mage_2: return typeof(SpellBarSetupGump_Mage_2);
				case SpellBarId.Mage_3: return typeof(SpellBarSetupGump_Mage_3);
				case SpellBarId.Mage_4: return typeof(SpellBarSetupGump_Mage_4);
				case SpellBarId.Monk_1: return typeof(SpellBarSetupGump_Monk_1);
				case SpellBarId.Monk_2: return typeof(SpellBarSetupGump_Monk_2);
				case SpellBarId.Necro_1: return typeof(SpellBarSetupGump_Necro_1);
				case SpellBarId.Necro_2: return typeof(SpellBarSetupGump_Necro_2);
				case SpellBarId.Priest_1: return typeof(SpellBarSetupGump_Priest_1);
				case SpellBarId.Priest_2: return typeof(SpellBarSetupGump_Priest_2);
				default: throw new ArgumentOutOfRangeException("id");
			}
		}

		public static Type GetToolbarGumpType(SpellBarId id)
		{
			switch (id)
			{
				case SpellBarId.Ancient_1: return typeof(SpellBarToolbarGump_Ancient_1);
				case SpellBarId.Ancient_2: return typeof(SpellBarToolbarGump_Ancient_2);
				case SpellBarId.Ancient_3: return typeof(SpellBarToolbarGump_Ancient_3);
				case SpellBarId.Ancient_4: return typeof(SpellBarToolbarGump_Ancient_4);
				case SpellBarId.Bard_1: return typeof(SpellBarToolbarGump_Bard_1);
				case SpellBarId.Bard_2: return typeof(SpellBarToolbarGump_Bard_2);
				case SpellBarId.Death_1: return typeof(SpellBarToolbarGump_Death_1);
				case SpellBarId.Death_2: return typeof(SpellBarToolbarGump_Death_2);
				case SpellBarId.Elemental_1: return typeof(SpellBarToolbarGump_Elemental_1);
				case SpellBarId.Elemental_2: return typeof(SpellBarToolbarGump_Elemental_2);
				case SpellBarId.Knight_1: return typeof(SpellBarToolbarGump_Knight_1);
				case SpellBarId.Knight_2: return typeof(SpellBarToolbarGump_Knight_2);
				case SpellBarId.Mage_1: return typeof(SpellBarToolbarGump_Mage_1);
				case SpellBarId.Mage_2: return typeof(SpellBarToolbarGump_Mage_2);
				case SpellBarId.Mage_3: return typeof(SpellBarToolbarGump_Mage_3);
				case SpellBarId.Mage_4: return typeof(SpellBarToolbarGump_Mage_4);
				case SpellBarId.Monk_1: return typeof(SpellBarToolbarGump_Monk_1);
				case SpellBarId.Monk_2: return typeof(SpellBarToolbarGump_Monk_2);
				case SpellBarId.Necro_1: return typeof(SpellBarToolbarGump_Necro_1);
				case SpellBarId.Necro_2: return typeof(SpellBarToolbarGump_Necro_2);
				case SpellBarId.Priest_1: return typeof(SpellBarToolbarGump_Priest_1);
				case SpellBarId.Priest_2: return typeof(SpellBarToolbarGump_Priest_2);
				default: throw new ArgumentOutOfRangeException("id");
			}
		}

		private static SpellBarDefinition[] BuildDefinitions()
		{
			return new[]
			{
				Bar(SpellBarId.Ancient_1, SpellBarSchool.Ancient, AncientSpellSchool.Instance, 1, "archtool1", "archclose1", "SPELL BAR - ANCIENT", true),
				Bar(SpellBarId.Ancient_2, SpellBarSchool.Ancient, AncientSpellSchool.Instance, 2, "archtool2", "archclose2", "SPELL BAR - ANCIENT", true),
				Bar(SpellBarId.Ancient_3, SpellBarSchool.Ancient, AncientSpellSchool.Instance, 3, "archtool3", "archclose3", "SPELL BAR - ANCIENT", true),
				Bar(SpellBarId.Ancient_4, SpellBarSchool.Ancient, AncientSpellSchool.Instance, 4, "archtool4", "archclose4", "SPELL BAR - ANCIENT", true),
				Bar(SpellBarId.Bard_1, SpellBarSchool.Bard, BardSpellSchool.Instance, 1, "bardtool1", "bardclose1", "SPELL BAR - BARD", false),
				Bar(SpellBarId.Bard_2, SpellBarSchool.Bard, BardSpellSchool.Instance, 2, "bardtool2", "bardclose2", "SPELL BAR - BARD", false),
				Bar(SpellBarId.Death_1, SpellBarSchool.DeathKnight, DeathKnightSpellSchool.Instance, 1, "deathtool1", "deathclose1", "SPELL BAR - DEATH KNIGHT", false),
				Bar(SpellBarId.Death_2, SpellBarSchool.DeathKnight, DeathKnightSpellSchool.Instance, 2, "deathtool2", "deathclose2", "SPELL BAR - DEATH KNIGHT", false),
				Bar(SpellBarId.Elemental_1, SpellBarSchool.Elemental, ElementalSpellSchool.Instance, 1, "elementtool1", "elementclose1", "SPELL BAR - ELEMENTALIST", false),
				Bar(SpellBarId.Elemental_2, SpellBarSchool.Elemental, ElementalSpellSchool.Instance, 2, "elementtool2", "elementclose2", "SPELL BAR - ELEMENTALIST", false),
				Bar(SpellBarId.Knight_1, SpellBarSchool.Knight, KnightSpellSchool.Instance, 1, "knighttool1", "knightclose1", "SPELL BAR - KNIGHT", false),
				Bar(SpellBarId.Knight_2, SpellBarSchool.Knight, KnightSpellSchool.Instance, 2, "knighttool2", "knightclose2", "SPELL BAR - KNIGHT", false),
				Bar(SpellBarId.Mage_1, SpellBarSchool.Magery, MagerySpellSchool.Instance, 1, "magetool1", "mageclose1", "SPELL BAR - MAGERY", true),
				Bar(SpellBarId.Mage_2, SpellBarSchool.Magery, MagerySpellSchool.Instance, 2, "magetool2", "mageclose2", "SPELL BAR - MAGERY", true),
				Bar(SpellBarId.Mage_3, SpellBarSchool.Magery, MagerySpellSchool.Instance, 3, "magetool3", "mageclose3", "SPELL BAR - MAGERY", true),
				Bar(SpellBarId.Mage_4, SpellBarSchool.Magery, MagerySpellSchool.Instance, 4, "magetool4", "mageclose4", "SPELL BAR - MAGERY", true),
				Bar(SpellBarId.Monk_1, SpellBarSchool.Monk, MonkSpellSchool.Instance, 1, "monktool1", "monkclose1", "SPELL BAR - MONK", false),
				Bar(SpellBarId.Monk_2, SpellBarSchool.Monk, MonkSpellSchool.Instance, 2, "monktool2", "monkclose2", "SPELL BAR - MONK", false),
				Bar(SpellBarId.Necro_1, SpellBarSchool.Necromancy, NecromancySpellSchool.Instance, 1, "necrotool1", "necrospell1", "SPELL BAR - NECROMANCER", false),
				Bar(SpellBarId.Necro_2, SpellBarSchool.Necromancy, NecromancySpellSchool.Instance, 2, "necrotool2", "necrospell2", "SPELL BAR - NECROMANCER", false),
				Bar(SpellBarId.Priest_1, SpellBarSchool.Priest, PriestSpellSchool.Instance, 1, "holytool1", "holyclose1", "SPELL BAR - PRIEST", false),
				Bar(SpellBarId.Priest_2, SpellBarSchool.Priest, PriestSpellSchool.Instance, 2, "holytool2", "holyclose2", "SPELL BAR - PRIEST", false),
			};
		}
	}
}