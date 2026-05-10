using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells.Dynamic;

namespace Server.Spells.Jester
{
	public class DynamicJesterSpellbookProvider : IDynamicSpellbookProvider
	{
		public SpellbookType SpellbookType { get { return SpellbookType.DynamicJester; } }
		public ushort BookGraphic { get { return 0x2B01; } }
		public ushort MinimizedGraphic { get { return 0x2B04; } }
		public byte SpellsPerPageSide { get { return 5; } }
		public byte MaxDictionaryPages { get { return 2; } }
		public bool DisplayManaCost { get { return true; } }
		public bool DisplayMinSkill { get { return true; } }
		public bool DisplayPowerWords { get { return false; } }
		public string ManaCostLabel { get { return "Prank Points"; } }
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
				Spell( 2700, 20749, "Can of Snakes", "", 40, 10, 0 ),
				Spell( 2701, 20751, "Clowns", "", 25, 10, 2 ),
				Spell( 2702, 20748, "Flower Power", "", 20, 10, 1 ),
				Spell( 2703, 20750, "Hilarity", "", 50, 10, 1 ),
				Spell( 2704, 20747, "Insult", "", 60, 10, 1 ),
				Spell( 2705, 20754, "Jump Around", "", 20, 10, 2 ),
				Spell( 2706, 20746, "Popping Balloon", "", 20, 10, 1 ),
				Spell( 2707, 20753, "Rabbit in a Hat", "", 30, 10, 0 ),
				Spell( 2708, 20755, "Seltzer Bottle", "", 20, 10, 1 ),
				Spell( 2709, 20752, "Surprise Gift", "", 20, 10, 1 )
			};
		}

		private static DynamicSpellDefinition Spell( ushort id, ushort icon, string name, string words, byte mana, byte skill, byte target )
		{
			return new DynamicSpellDefinition { SpellID = id, IconGraphic = icon, Name = name, PowerWords = words, ManaCost = mana, MinSkill = skill, TargetType = target };
		}

		public ulong GetSpellBitmask( Mobile m, Spellbook spellbook ) { return spellbook != null ? spellbook.Content : 0; }

		public static void Initialize()
		{
			DynamicSpellbookManager.RegisterProvider( new DynamicJesterSpellbookProvider(), 0x1E3F );
		}
	}
}
