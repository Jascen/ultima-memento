using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells.Dynamic;

namespace Server.Spells.Research
{
	public class DynamicResearchSpellbookProvider : IDynamicSpellbookProvider
	{
		public SpellbookType SpellbookType { get { return SpellbookType.DynamicResearch; } }
		public ushort BookGraphic { get { return 0x08AC; } }
		public ushort MinimizedGraphic { get { return 0x08BA; } }
		public byte SpellsPerPageSide { get { return 8; } }
		public byte MaxDictionaryPages { get { return 8; } }
		public bool DisplayManaCost { get { return true; } }
		public bool DisplayMinSkill { get { return true; } }
		public bool DisplayPowerWords { get { return false; } }
		public string ManaCostLabel { get { return null; } }
		public string MinSkillLabel { get { return null; } }
		public string CustomPropertyTitle { get { return null; } }
		public string CustomPropertyLabel { get { return null; } }
		public string CustomPropertyName { get { return null; } }
		public ushort BookHue { get { return 0; } }
		public ushort TextColor { get { return 0; } }
		public ushort SpellNameColor { get { return 0; } }
		public ushort TitleColor { get { return 0; } }
		public short ContentOffsetX { get { return 0; } }
		public short ContentOffsetY { get { return 0; } }
		public ushort PageTurnLeftGraphic { get { return 0; } }
		public ushort PageTurnRightGraphic { get { return 0; } }
		public short PageTurnLeftX { get { return 0; } }
		public short PageTurnLeftY { get { return 0; } }
		public short PageTurnRightX { get { return 0; } }
		public short PageTurnRightY { get { return 0; } }
		public ushort[] OverlayGraphics { get { return null; } }
		public List<DynamicInfoPage> GetInfoPages() { return null; }

		public string[] GetPageNames()
		{
			return new string[]
			{
				"Conjuration", "Death", "Enchanting", "Sorcery",
				"Summoning", "Thaumaturgy", "Theurgy", "Wizardry"
			};
		}

		public List<DynamicSpellDefinition> GetSpellDefinitions()
		{
			return new List<DynamicSpellDefinition>
			{
				// Conjuration
				Spell( 2800, 11195, "Conjure", "", 10, 15, 0, 1 ),
				Spell( 2801, 11196, "Extinguish", "", 20, 35, 1, 1 ),
				Spell( 2802, 11197, "Clone", "", 25, 45, 0, 1 ),
				Spell( 2803, 11198, "Create Gold", "", 35, 55, 0, 1 ),
				Spell( 2804, 11199, "Swarm", "", 15, 40, 1, 1 ),
				Spell( 2805, 11200, "Magic Steed", "", 30, 50, 0, 1 ),
				Spell( 2806, 11201, "Aerial Servant", "", 50, 80, 0, 1 ),
				Spell( 2807, 11202, "Death Vortex", "", 60, 90, 1, 1 ),
				// Death
				Spell( 2808, 11203, "Death Speak", "", 5, 10, 0, 2 ),
				Spell( 2809, 11204, "Rock Flesh", "", 10, 15, 2, 2 ),
				Spell( 2810, 11205, "Grant Peace", "", 35, 75, 1, 2 ),
				Spell( 2811, 11206, "Animate Bones", "", 40, 70, 0, 2 ),
				Spell( 2812, 11207, "Mask of Death", "", 70, 90, 2, 2 ),
				Spell( 2813, 11208, "Create Golem", "", 40, 70, 0, 2 ),
				Spell( 2814, 11209, "Open Ground", "", 65, 85, 1, 2 ),
				Spell( 2815, 11210, "Withstand Death", "", 70, 90, 2, 2 ),
				// Enchanting
				Spell( 2816, 11211, "Sneak", "", 10, 10, 2, 3 ),
				Spell( 2817, 11212, "Mass Might", "", 99, 66, 2, 3 ),
				Spell( 2818, 11213, "Sleep", "", 15, 40, 1, 3 ),
				Spell( 2819, 11214, "Cause Fear", "", 35, 45, 1, 3 ),
				Spell( 2820, 11215, "Enchant", "", 45, 75, 2, 3 ),
				Spell( 2821, 11216, "Sleep Field", "", 30, 60, 1, 3 ),
				Spell( 2822, 11217, "Charm", "", 60, 82, 2, 3 ),
				Spell( 2823, 11218, "Mass Sleep", "", 50, 85, 1, 3 ),
				// Sorcery
				Spell( 2824, 11219, "Create Fire", "", 5, 15, 1, 4 ),
				Spell( 2825, 11220, "Endure Cold", "", 15, 20, 2, 4 ),
				Spell( 2826, 11221, "Endure Heat", "", 15, 20, 2, 4 ),
				Spell( 2827, 11222, "Ignite", "", 30, 40, 1, 4 ),
				Spell( 2828, 11223, "Flame Bolt", "", 15, 30, 1, 4 ),
				Spell( 2829, 11224, "Conflagration", "", 20, 35, 1, 4 ),
				Spell( 2830, 11225, "Explosion", "", 30, 60, 1, 4 ),
				Spell( 2831, 11226, "Ring of Fire", "", 55, 85, 1, 4 ),
				// Summoning
				Spell( 2832, 11227, "Electrical Elemental", "", 40, 70, 0, 5 ),
				Spell( 2833, 11228, "Weed Elemental", "", 40, 70, 0, 5 ),
				Spell( 2834, 11229, "Ice Elemental", "", 40, 70, 0, 5 ),
				Spell( 2835, 11230, "Mud Elemental", "", 40, 70, 0, 5 ),
				Spell( 2836, 11231, "Blood Elemental", "", 50, 90, 0, 5 ),
				Spell( 2837, 11232, "Poison Elemental", "", 50, 86, 0, 5 ),
				Spell( 2838, 11233, "Gem Elemental", "", 50, 80, 0, 5 ),
				Spell( 2839, 11234, "Acid Elemental", "", 50, 82, 0, 5 ),
				// Thaumaturgy
				Spell( 2840, 11235, "Confusion Blast", "", 15, 40, 1, 6 ),
				Spell( 2841, 11236, "Spawn Creatures", "", 20, 45, 0, 6 ),
				Spell( 2842, 11237, "Ethereal Travel", "", 20, 35, 2, 6 ),
				Spell( 2843, 11238, "Banish Daemon", "", 40, 80, 1, 6 ),
				Spell( 2844, 11239, "Call Destruction", "", 25, 40, 1, 6 ),
				Spell( 2845, 11240, "Meteor Shower", "", 40, 70, 1, 6 ),
				Spell( 2846, 11241, "Invoke Devil", "", 60, 95, 0, 6 ),
				Spell( 2847, 11242, "Armageddon", "", 80, 100, 1, 6 ),
				// Theurgy
				Spell( 2848, 11243, "See Truth", "", 60, 20, 0, 7 ),
				Spell( 2849, 11244, "Healing Touch", "", 15, 30, 2, 7 ),
				Spell( 2850, 11245, "Wizard Eye", "", 30, 50, 0, 7 ),
				Spell( 2851, 11246, "Fade from Sight", "", 15, 50, 2, 7 ),
				Spell( 2852, 11247, "Divination", "", 30, 50, 0, 7 ),
				Spell( 2853, 11248, "Intervention", "", 25, 50, 2, 7 ),
				Spell( 2854, 11249, "Air Walk", "", 55, 65, 2, 7 ),
				Spell( 2855, 11250, "Restoration", "", 50, 80, 2, 7 ),
				// Wizardry
				Spell( 2856, 11251, "Icicle", "", 10, 15, 1, 8 ),
				Spell( 2857, 11252, "Snow Ball", "", 10, 10, 1, 8 ),
				Spell( 2858, 11253, "Frost Field", "", 15, 30, 1, 8 ),
				Spell( 2859, 11254, "Gas Cloud", "", 25, 45, 1, 8 ),
				Spell( 2860, 11255, "Frost Strike", "", 40, 67, 1, 8 ),
				Spell( 2861, 11256, "Hail Storm", "", 25, 55, 1, 8 ),
				Spell( 2862, 11257, "Avalanche", "", 40, 70, 1, 8 ),
				Spell( 2863, 11258, "Mass Death", "", 55, 90, 1, 8 )
			};
		}

		private static DynamicSpellDefinition Spell( ushort id, ushort icon, string name, string words, byte mana, byte skill, byte target, byte page )
		{
			return new DynamicSpellDefinition { SpellID = id, IconGraphic = icon, Name = name, PowerWords = words, ManaCost = mana, MinSkill = skill, TargetType = target, Page = page };
		}

		public ulong GetSpellBitmask( Mobile m, Spellbook spellbook ) { return spellbook != null ? spellbook.Content : 0; }

		public static void Initialize()
		{
			DynamicSpellbookManager.RegisterProvider( new DynamicResearchSpellbookProvider(), 0x65EC );
		}
	}
}
