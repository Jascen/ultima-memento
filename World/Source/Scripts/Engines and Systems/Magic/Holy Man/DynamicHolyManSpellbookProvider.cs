using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells.Dynamic;

namespace Server.Spells.HolyMan
{
	public class DynamicHolyManSpellbookProvider : IDynamicSpellbookProvider
	{
		public SpellbookType SpellbookType { get { return SpellbookType.DynamicHolyMan; } }
		public ushort BookGraphic { get { return 0x2B01; } }
		public ushort MinimizedGraphic { get { return 0x2B04; } }
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
				Spell( 2500, 0x965, "Banish Evil", "", 30, 60, 1 ),
				Spell( 2501, 0x966, "Dampen Spirit", "", 35, 70, 1 ),
				Spell( 2502, 0x967, "Enchant", "", 45, 90, 2 ),
				Spell( 2503, 0x968, "Hammer of Faith", "", 25, 50, 2 ),
				Spell( 2504, 0x969, "Heavenly Light", "", 5, 10, 0 ),
				Spell( 2505, 0x96A, "Nourish", "", 5, 10, 2 ),
				Spell( 2506, 0x96B, "Purge", "", 20, 40, 2 ),
				Spell( 2507, 0x96C, "Rebirth", "", 40, 80, 2 ),
				Spell( 2508, 0x96E, "Sacred Boon", "", 10, 20, 2 ),
				Spell( 2509, 0x96D, "Sanctify", "", 15, 30, 2 ),
				Spell( 2510, 0x96F, "Seance", "", 30, 60, 0 ),
				Spell( 2511, 0x970, "Smite", "", 20, 40, 1 ),
				Spell( 2512, 0x971, "Touch of Life", "", 10, 20, 2 ),
				Spell( 2513, 0x972, "Trial by Fire", "", 15, 30, 0 )
			};
		}

		private static DynamicSpellDefinition Spell( ushort id, ushort icon, string name, string words, byte mana, byte skill, byte target )
		{
			return new DynamicSpellDefinition { SpellID = id, IconGraphic = icon, Name = name, PowerWords = words, ManaCost = mana, MinSkill = skill, TargetType = target };
		}

		public ulong GetSpellBitmask( Mobile m, Spellbook spellbook ) { return spellbook != null ? spellbook.Content : 0; }

		public static void Initialize()
		{
			DynamicSpellbookManager.RegisterProvider( new DynamicHolyManSpellbookProvider(), 0x672B );
		}
	}
}
