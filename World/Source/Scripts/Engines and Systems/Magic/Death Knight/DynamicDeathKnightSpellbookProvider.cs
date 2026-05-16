using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells.Dynamic;

namespace Server.Spells.DeathKnight
{
	public class DynamicDeathKnightSpellbookProvider : IDynamicSpellbookProvider
	{
		public SpellbookType SpellbookType { get { return SpellbookType.DynamicDeathKnight; } }
		public ushort BookGraphic { get { return 0x2B00; } }
		public ushort MinimizedGraphic { get { return 0x2B03; } }
		public byte SpellsPerPageSide { get { return 7; } }
		public byte MaxDictionaryPages { get { return 2; } }
		public bool DisplayManaCost { get { return true; } }
		public bool DisplayMinSkill { get { return true; } }
		public bool DisplayPowerWords { get { return true; } }
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
				Spell( 2300, 0x5010, "Banish", "", 36, 40, 1 ),
				Spell( 2301, 0x5009, "Demonic Touch", "", 16, 15, 2 ),
				Spell( 2302, 0x5005, "Devil Pact", "", 60, 90, 2 ),
				Spell( 2303, 0x402, "Grim Reaper", "", 28, 30, 1 ),
				Spell( 2304, 0x5002, "Hag Hand", "", 8, 5, 0 ),
				Spell( 2305, 0x3E9, "Hellfire", "", 52, 70, 1 ),
				Spell( 2306, 0x5DC0, "Lucifer's Bolt", "", 24, 25, 1 ),
				Spell( 2307, 0x1B, "Orb of Orcus", "", 56, 80, 0 ),
				Spell( 2308, 0x3EE, "Shield of Hate", "", 48, 60, 2 ),
				Spell( 2309, 0x5006, "Soul Reaper", "", 40, 45, 1 ),
				Spell( 2310, 0x2B, "Strength of Steel", "", 20, 20, 2 ),
				Spell( 2311, 0x12, "Strike", "", 12, 10, 1 ),
				Spell( 2312, 0x500C, "Succubus Skin", "", 32, 35, 2 ),
				Spell( 2313, 0x2E, "Wrath", "", 44, 50, 1 )
			};
		}

		private static DynamicSpellDefinition Spell( ushort id, ushort icon, string name, string words, byte mana, byte skill, byte target )
		{
			return new DynamicSpellDefinition { SpellID = id, IconGraphic = icon, Name = name, PowerWords = words, ManaCost = mana, MinSkill = skill, TargetType = target };
		}

		public ulong GetSpellBitmask( Mobile m, Spellbook spellbook ) { return spellbook != null ? spellbook.Content : 0; }

		public static void Initialize()
		{
			DynamicSpellbookManager.RegisterProvider( new DynamicDeathKnightSpellbookProvider(), 0x6721 );
		}
	}
}
