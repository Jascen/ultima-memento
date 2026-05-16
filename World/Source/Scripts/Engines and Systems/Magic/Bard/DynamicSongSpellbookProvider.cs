using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells.Dynamic;

namespace Server.Spells.Song
{
	public class DynamicSongSpellbookProvider : IDynamicSpellbookProvider
	{
		public SpellbookType SpellbookType { get { return SpellbookType.DynamicSong; } }
		public ushort BookGraphic { get { return 0x2B01; } }
		public ushort MinimizedGraphic { get { return 0x2B04; } }
		public byte SpellsPerPageSide { get { return 8; } }
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
				Spell( 2200, 0x404, "Army's Paeon", "", 15, 55, 2 ),
				Spell( 2201, 0x405, "Enchanting Etude", "", 60, 20, 2 ),
				Spell( 2202, 0x406, "Energy Carol", "", 50, 12, 2 ),
				Spell( 2203, 0x407, "Energy Threnody", "", 70, 25, 1 ),
				Spell( 2204, 0x408, "Fire Carol", "", 50, 12, 2 ),
				Spell( 2205, 0x409, "Fire Threnody", "", 70, 25, 1 ),
				Spell( 2206, 0x40A, "Foe Requiem", "", 80, 30, 1 ),
				Spell( 2207, 0x40B, "Ice Carol", "", 50, 12, 2 ),
				Spell( 2208, 0x40C, "Ice Threnody", "", 70, 25, 1 ),
				Spell( 2209, 0x40D, "Knight's Minne", "", 50, 12, 2 ),
				Spell( 2210, 0x40E, "Mage's Ballad", "", 55, 15, 2 ),
				Spell( 2211, 0x410, "Magic Finale", "", 90, 35, 1 ),
				Spell( 2212, 0x411, "Poison Carol", "", 50, 12, 2 ),
				Spell( 2213, 0x412, "Poison Threnody", "", 70, 25, 1 ),
				Spell( 2214, 0x413, "Shepherd's Dance", "", 60, 20, 2 ),
				Spell( 2215, 0x414, "Sinewy Etude", "", 60, 20, 2 )
			};
		}

		private static DynamicSpellDefinition Spell( ushort id, ushort icon, string name, string words, byte mana, byte skill, byte target )
		{
			return new DynamicSpellDefinition { SpellID = id, IconGraphic = icon, Name = name, PowerWords = words, ManaCost = mana, MinSkill = skill, TargetType = target };
		}

		public ulong GetSpellBitmask( Mobile m, Spellbook spellbook ) { return spellbook != null ? spellbook.Content : 0; }

		public static void Initialize()
		{
			DynamicSpellbookManager.RegisterProvider( new DynamicSongSpellbookProvider(), 0x671B );
		}
	}
}
