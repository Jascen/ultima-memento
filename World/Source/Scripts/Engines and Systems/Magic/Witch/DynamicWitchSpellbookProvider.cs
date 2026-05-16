using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells.Dynamic;

namespace Server.Spells.Undead
{
	public class DynamicWitchSpellbookProvider : IDynamicSpellbookProvider
	{
		public SpellbookType SpellbookType { get { return SpellbookType.DynamicWitch; } }
		public ushort BookGraphic { get { return 0x2B00; } }
		public ushort MinimizedGraphic { get { return 0x2B03; } }
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
				Spell( 3100, 11460, "Eyes of the Dead", "", 0, 10, 2 ),
				Spell( 3101, 11468, "Tomb Raiding", "", 0, 15, 0 ),
				Spell( 3102, 11458, "Disease", "", 0, 20, 1 ),
				Spell( 3103, 11465, "Phantasm", "", 0, 25, 2 ),
				Spell( 3104, 11466, "Retched Air", "", 0, 30, 1 ),
				Spell( 3105, 11464, "Lich Leech", "", 0, 35, 1 ),
				Spell( 3106, 11470, "Wall of Spikes", "", 0, 40, 1 ),
				Spell( 3107, 11459, "Disease Curing", "", 0, 45, 2 ),
				Spell( 3108, 11456, "Blood Pact", "", 0, 50, 2 ),
				Spell( 3109, 11467, "Spectre Shadow", "", 0, 55, 2 ),
				Spell( 3110, 11461, "Ghost Phase", "", 0, 60, 2 ),
				Spell( 3111, 11457, "Demonic Fire", "", 0, 65, 1 ),
				Spell( 3112, 11462, "Ghostly Images", "", 0, 70, 2 ),
				Spell( 3113, 11463, "Hellish Branding", "", 0, 75, 0 ),
				Spell( 3114, 11455, "Black Gate", "", 0, 80, 0 ),
				Spell( 3115, 11469, "Vampire Blood", "", 0, 85, 2 )
			};
		}

		private static DynamicSpellDefinition Spell( ushort id, ushort icon, string name, string words, byte mana, byte skill, byte target )
		{
			return new DynamicSpellDefinition { SpellID = id, IconGraphic = icon, Name = name, PowerWords = words, ManaCost = mana, MinSkill = skill, TargetType = target };
		}

		public ulong GetSpellBitmask( Mobile m, Spellbook spellbook ) { return spellbook != null ? spellbook.Content : 0; }

		public static void Initialize()
		{
			DynamicSpellbookManager.RegisterProvider( new DynamicWitchSpellbookProvider(), 0x5776 );
		}
	}
}
