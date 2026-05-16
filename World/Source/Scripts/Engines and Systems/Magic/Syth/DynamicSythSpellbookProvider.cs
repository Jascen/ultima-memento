using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells.Dynamic;

namespace Server.Spells.Syth
{
	public class DynamicSythSpellbookProvider : IDynamicSpellbookProvider
	{
		public SpellbookType SpellbookType { get { return SpellbookType.DynamicSyth; } }
		public ushort BookGraphic { get { return 0x2B00; } }
		public ushort MinimizedGraphic { get { return 0x2B03; } }
		public byte SpellsPerPageSide { get { return 5; } }
		public byte MaxDictionaryPages { get { return 2; } }
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
		public string[] GetPageNames() { return null; }
		public List<DynamicInfoPage> GetInfoPages() { return null; }

		public List<DynamicSpellDefinition> GetSpellDefinitions()
		{
			return new List<DynamicSpellDefinition>
			{
				Spell( 3000, 1038, "Psychokinesis", "", 5, 10, 0 ),
				Spell( 3001, 1002, "Death Grip", "", 8, 20, 1 ),
				Spell( 3002, 21287, "Projection", "", 12, 30, 0 ),
				Spell( 3003, 1005, "Throw Sword", "", 16, 40, 1 ),
				Spell( 3004, 1043, "Speed", "", 20, 50, 2 ),
				Spell( 3005, 1010, "Syth Lightning", "", 24, 60, 1 ),
				Spell( 3006, 23015, "Absorption", "", 28, 70, 2 ),
				Spell( 3007, 23010, "Psychic Blast", "", 32, 80, 1 ),
				Spell( 3008, 1026, "Drain Life", "", 36, 90, 1 ),
				Spell( 3009, 2261, "Clone", "", 40, 100, 2 )
			};
		}

		private static DynamicSpellDefinition Spell( ushort id, ushort icon, string name, string words, byte mana, byte skill, byte target )
		{
			return new DynamicSpellDefinition { SpellID = id, IconGraphic = icon, Name = name, PowerWords = words, ManaCost = mana, MinSkill = skill, TargetType = target };
		}

		public ulong GetSpellBitmask( Mobile m, Spellbook spellbook ) { return spellbook != null ? spellbook.Content : 0; }

		public static void Initialize()
		{
			DynamicSpellbookManager.RegisterProvider( new DynamicSythSpellbookProvider(), 0x4CE0 );
		}
	}
}
