using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells.Dynamic;

namespace Server.Spells.Jedi
{
	public class DynamicJediSpellbookProvider : IDynamicSpellbookProvider
	{
		public SpellbookType SpellbookType { get { return SpellbookType.DynamicJedi; } }
		public ushort BookGraphic { get { return 0x2B01; } }
		public ushort MinimizedGraphic { get { return 0x2B04; } }
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
				Spell( 2600, 11244, "Force Grip", "", 5, 10, 0 ),
				Spell( 2601, 11243, "Mind's Eye", "", 8, 20, 0 ),
				Spell( 2602, 11201, "Mirage", "", 12, 30, 0 ),
				Spell( 2603, 11215, "Throw Sabre", "", 16, 40, 1 ),
				Spell( 2604, 11249, "Celerity", "", 20, 50, 2 ),
				Spell( 2605, 11237, "Psychic Aura", "", 24, 20, 2 ),
				Spell( 2606, 11204, "Deflection", "", 28, 70, 2 ),
				Spell( 2607, 11213, "Soothing Touch", "", 32, 10, 2 ),
				Spell( 2608, 11253, "Stasis Field", "", 36, 50, 1 ),
				Spell( 2609, 11218, "Replicate", "", 40, 100, 2 )
			};
		}

		private static DynamicSpellDefinition Spell( ushort id, ushort icon, string name, string words, byte mana, byte skill, byte target )
		{
			return new DynamicSpellDefinition { SpellID = id, IconGraphic = icon, Name = name, PowerWords = words, ManaCost = mana, MinSkill = skill, TargetType = target };
		}

		public ulong GetSpellBitmask( Mobile m, Spellbook spellbook ) { return spellbook != null ? spellbook.Content : 0; }

		public static void Initialize()
		{
			DynamicSpellbookManager.RegisterProvider( new DynamicJediSpellbookProvider(), 0x543D );
		}
	}
}
