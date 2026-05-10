using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells.Dynamic;

namespace Server.Spells.Shinobi
{
	public class DynamicShinobiSpellbookProvider : IDynamicSpellbookProvider
	{
		public SpellbookType SpellbookType { get { return SpellbookType.DynamicShinobi; } }
		public ushort BookGraphic { get { return 0x2B06; } }
		public ushort MinimizedGraphic { get { return 0x2B08; } }
		public byte SpellsPerPageSide { get { return 4; } }
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
				Spell( 2900, 10876, "Cheetah Paws", "", 60, 80, 2 ),
				Spell( 2901, 10871, "Deception", "", 15, 30, 2 ),
				Spell( 2902, 10872, "Eagle Eye", "", 50, 70, 2 ),
				Spell( 2903, 10873, "Espionage", "", 10, 20, 2 ),
				Spell( 2904, 10874, "Ferret Flee", "", 30, 50, 2 ),
				Spell( 2905, 10875, "Monkey Leap", "", 20, 40, 2 ),
				Spell( 2906, 10877, "Mystic Shuriken", "", 40, 60, 1 ),
				Spell( 2907, 10878, "Tiger Strength", "", 70, 90, 2 )
			};
		}

		private static DynamicSpellDefinition Spell( ushort id, ushort icon, string name, string words, byte mana, byte skill, byte target )
		{
			return new DynamicSpellDefinition { SpellID = id, IconGraphic = icon, Name = name, PowerWords = words, ManaCost = mana, MinSkill = skill, TargetType = target };
		}

		public ulong GetSpellBitmask( Mobile m, Spellbook spellbook ) { return spellbook != null ? spellbook.Content : 0; }

		public static void Initialize()
		{
			DynamicSpellbookManager.RegisterProvider( new DynamicShinobiSpellbookProvider(), 0x5C15 );
		}
	}
}
