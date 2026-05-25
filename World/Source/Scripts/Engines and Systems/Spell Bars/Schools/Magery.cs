using Server.Items;
using Server.Mobiles;

namespace Server.SpellBars
{
	public sealed class MagerySpellSchool : ISpellSchool
	{
		public static readonly MagerySpellSchool Instance = new MagerySpellSchool();

		private static readonly string[] SpellNames =
		{
			"Clumsy", "Create Food", "Feeblemind", "Heal", "Magic Arrow", "Night Sight", "Reactive Armor", "Weaken",
			"Agility", "Cunning", "Cure", "Harm", "Magic Trap", "Remove Trap", "Protection", "Strength",
			"Bless", "Fireball", "Magic Lock", "Poison", "Telekinesis", "Teleport", "Unlock", "Wall Of Stone",
			"Arch Cure", "Arch Protection", "Curse", "Fire Field", "Greater Heal", "Lightning", "Mana Drain", "Recall",
			"Blade Spirits", "Dispel Field", "Incognito", "Magic Reflect", "Mind Blast", "Paralyze", "Poison Field", "Summon Creature",
			"Dispel", "Energy Bolt", "Explosion", "Invisibility", "Mark", "Mass Curse", "Paralyze Field", "Reveal",
			"Chain Lightning", "Energy Field", "Flame Strike", "Gate Travel", "Mana Vampire", "Mass Dispel", "Meteor Swarm", "Polymorph",
			"Earthquake", "Energy Vortex", "Resurrection", "Air Elemental", "Summon Daemon", "Earth Elemental", "Fire Elemental", "Water Elemental"
		};

		public int MaxSlots
		{ get { return 64; } }

		public SpellBarSchool School
		{ get { return SpellBarSchool.Magery; } }

		public int GetBackgroundImage(PlayerMobile from)
		{ return 11173; }

		public int GetIcon(PlayerMobile from, int slotIndex)
		{ return 2240 - 1 + slotIndex; }

		public string GetName(int slotIndex)
		{
			if (slotIndex < 1 || slotIndex > SpellNames.Length)
				return string.Empty;

			return SpellNames[slotIndex - 1];
		}

		public int GetRegistrySpellId(int slotIndex)
		{ return slotIndex - 1; }

		public bool HasSpell(PlayerMobile from, int registrySpellId)
		{
			Spellbook book = Spellbook.Find(from, registrySpellId);
			return book != null && book.HasSpell(registrySpellId);
		}
	}

	public sealed class SpellBarSetupGump_Mage_1 : SpellBarSetupPagedGump
	{
		public SpellBarSetupGump_Mage_1(PlayerMobile from, int origin, int pageNumber = 1) : base(SpellBarId.Mage_1, from, origin, pageNumber)
		{
		}
	}

	public sealed class SpellBarSetupGump_Mage_2 : SpellBarSetupPagedGump
	{
		public SpellBarSetupGump_Mage_2(PlayerMobile from, int origin, int pageNumber = 1) : base(SpellBarId.Mage_2, from, origin, pageNumber)
		{
		}
	}

	public sealed class SpellBarSetupGump_Mage_3 : SpellBarSetupPagedGump
	{
		public SpellBarSetupGump_Mage_3(PlayerMobile from, int origin, int pageNumber = 1) : base(SpellBarId.Mage_3, from, origin, pageNumber)
		{
		}
	}

	public sealed class SpellBarSetupGump_Mage_4 : SpellBarSetupPagedGump
	{
		public SpellBarSetupGump_Mage_4(PlayerMobile from, int origin, int pageNumber = 1) : base(SpellBarId.Mage_4, from, origin, pageNumber)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Mage_1 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Mage_1(PlayerMobile from) : base(SpellBarId.Mage_1, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Mage_2 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Mage_2(PlayerMobile from) : base(SpellBarId.Mage_2, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Mage_3 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Mage_3(PlayerMobile from) : base(SpellBarId.Mage_3, from)
		{
		}
	}

	public sealed class SpellBarToolbarGump_Mage_4 : SpellBarToolbarGump
	{
		public SpellBarToolbarGump_Mage_4(PlayerMobile from) : base(SpellBarId.Mage_4, from)
		{
		}
	}
}