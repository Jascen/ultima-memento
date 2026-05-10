using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells.Dynamic;

namespace Server.Spells.Herbalist
{
	public class DynamicDruidismSpellbookProvider : IDynamicSpellbookProvider
	{
		public SpellbookType SpellbookType { get { return SpellbookType.DynamicDruidism; } }
		public ushort BookGraphic { get { return 0x2B01; } }
		public ushort MinimizedGraphic { get { return 0x2B04; } }
		public byte SpellsPerPageSide { get { return 8; } }
		public byte MaxDictionaryPages { get { return 2; } }
		public bool DisplayManaCost { get { return false; } }
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
		public string[] GetPageNames() { return null; }
		public List<DynamicInfoPage> GetInfoPages() { return null; }

		public List<DynamicSpellDefinition> GetSpellDefinitions()
		{
			return new List<DynamicSpellDefinition>
			{
				Spell( 2400, 11446, "Lure Stone", "", 0, 10, 0 ),
				Spell( 2401, 11449, "Nature's Passage", "", 0, 15, 0 ),
				Spell( 2402, 11450, "Shield of Earth", "", 0, 20, 0 ),
				Spell( 2403, 11454, "Woodland Protection", "", 0, 25, 2 ),
				Spell( 2404, 11451, "Stone Rising", "", 0, 30, 0 ),
				Spell( 2405, 11443, "Grasping Roots", "", 0, 35, 1 ),
				Spell( 2406, 11439, "Druidic Marking", "", 0, 40, 0 ),
				Spell( 2407, 11444, "Herbal Healing", "", 0, 45, 2 ),
				Spell( 2408, 11442, "Forest Blending", "", 0, 50, 2 ),
				Spell( 2409, 11445, "Jar of Fireflies", "", 0, 55, 0 ),
				Spell( 2410, 11448, "Mushroom Gateway", "", 0, 60, 0 ),
				Spell( 2411, 11441, "Jar of Insects", "", 0, 65, 1 ),
				Spell( 2412, 11440, "Fairy in a Jar", "", 0, 70, 2 ),
				Spell( 2413, 11452, "Treant Fertilizer", "", 0, 75, 2 ),
				Spell( 2414, 11453, "Volcanic Fluid", "", 0, 80, 1 ),
				Spell( 2415, 11447, "Magical Mud", "", 0, 85, 2 )
			};
		}

		private static DynamicSpellDefinition Spell( ushort id, ushort icon, string name, string words, byte mana, byte skill, byte target )
		{
			return new DynamicSpellDefinition { SpellID = id, IconGraphic = icon, Name = name, PowerWords = words, ManaCost = mana, MinSkill = skill, TargetType = target };
		}

		public ulong GetSpellBitmask( Mobile m, Spellbook spellbook ) { return spellbook != null ? spellbook.Content : 0; }

		public static void Initialize()
		{
			DynamicSpellbookManager.RegisterProvider( new DynamicDruidismSpellbookProvider(), 0x5688 );
		}
	}
}
