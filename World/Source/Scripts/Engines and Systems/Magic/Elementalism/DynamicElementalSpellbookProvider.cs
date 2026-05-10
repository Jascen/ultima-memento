using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Spells.Dynamic;

namespace Server.Spells.Elementalism
{
	public class DynamicElementalSpellbookProvider : IDynamicSpellbookProvider, IDynamicSpellbookBookmark
	{
		public SpellbookType SpellbookType { get { return SpellbookType.DynamicElementalism; } }

		// Custom elemental spellbook gump
		public ushort BookGraphic { get { return 11138; } }    // 0x2B82 - Elemental spellbook base
		public ushort MinimizedGraphic { get { return 0x08BA; } } // Minimized placeholder

		public byte SpellsPerPageSide { get { return 4; } }
		public byte MaxDictionaryPages { get { return 8; } }

		public bool DisplayManaCost { get { return true; } }
		public bool DisplayMinSkill { get { return true; } }
		public bool DisplayPowerWords { get { return false; } }
		public string ManaCostLabel { get { return "Power"; } }
		public string MinSkillLabel { get { return "Min. Skill"; } }

		public string CustomPropertyTitle { get { return null; } }
		public string CustomPropertyLabel { get { return null; } }
		public string CustomPropertyName { get { return null; } }

		// Layout: elemental spellbook gump (Air element default)
		public ushort BookHue { get { return 2807; } }         // Air element hue
		public ushort TextColor { get { return 0x0044; } }     // Body text color
		public ushort SpellNameColor { get { return 0x0044; } } // Clickable spell names
		public ushort TitleColor { get { return 0x0044; } }    // Titles (Index, sphere names)
		public short ContentOffsetX { get { return -22; } }  // Elemental book content area is ~22px left of Magery
		public short ContentOffsetY { get { return -5; } }   // Slight vertical adjustment
		public ushort PageTurnLeftGraphic { get { return 11159; } }   // 0x2B97
		public ushort PageTurnRightGraphic { get { return 11160; } }  // 0x2B98
		public short PageTurnLeftX { get { return 24; } }
		public short PageTurnLeftY { get { return 8; } }
		public short PageTurnRightX { get { return 295; } }
		public short PageTurnRightY { get { return 8; } }
		public ushort[] OverlayGraphics { get { return new ushort[] { 11152, 11147 }; } } // 0x2B90 (border), 0x2B8B (spine)

		public string[] GetPageNames()
		{
			return new string[]
			{
				"First Sphere",
				"Second Sphere",
				"Third Sphere",
				"Fourth Sphere",
				"Fifth Sphere",
				"Sixth Sphere",
				"Seventh Sphere",
				"Eighth Sphere"
			};
		}

		public List<DynamicSpellDefinition> GetSpellDefinitions()
		{
			// Icon bases per element: Air=11477, Earth=11509, Fire=11541, Water=11573
			// Using Earth (11509) as the default icon base; icons = 11509 + (spellID - 2100)

			return new List<DynamicSpellDefinition>
			{
				// === First Sphere (Circle 1) - Mana: 5, MinSkill: 0 ===
				Spell( 2100, 11509, "Elemental Armor", "Armura",
					"Increases your elemental resistance while reducing your other resistances. Active until deactivated by re-casting.",
					5, 0, 0, 1 ),
				Spell( 2101, 11510, "Elemental Bolt", "Sulita",
					"Shoots a magical bolt at a target, dealing elemental and physical damage.",
					5, 0, 1, 1 ),
				Spell( 2102, 11511, "Elemental Mend", "Vindeca",
					"Restores the target of a small amount of lost hit points.",
					5, 0, 2, 1 ),
				Spell( 2103, 11512, "Elemental Sanctuary", "Invata",
					"Transports the elementalist to the safety of the Lyceum. Can cast in dungeons at higher levels.",
					5, 0, 0, 1 ),

				// === Second Sphere (Circle 2) - Mana: 7, MinSkill: 0 ===
				Spell( 2104, 11513, "Elemental Pain", "Durere",
					"Affects the target with elemental energy, dealing damage. The closer the target, the more damage dealt.",
					7, 0, 1, 2 ),
				Spell( 2105, 11514, "Elemental Protection", "Proteja",
					"Prevents the caster from having their spells disrupted, but lowers physical and magic resistance. Toggle spell.",
					7, 0, 0, 2 ),
				Spell( 2106, 11515, "Elemental Purge", "Vindeca",
					"Attempts to cleanse poisons affecting the target.",
					7, 0, 2, 2 ),
				Spell( 2107, 11516, "Elemental Steed", "Faptura",
					"Summons an elemental mount that you can ride. The creature disappears after a set amount of time and requires a control slot.",
					7, 0, 0, 2 ),

				// === Third Sphere (Circle 3) - Mana: 10, MinSkill: 9 ===
				Spell( 2108, 11517, "Elemental Call", "Striga",
					"A lesser elemental is summoned to serve the caster. Disappears after a set amount of time and requires a control slot.",
					10, 9, 0, 3 ),
				Spell( 2109, 11518, "Elemental Force", "Forta",
					"Shoots a powerful elemental projectile at a target, dealing significant elemental damage.",
					10, 9, 1, 3 ),
				Spell( 2110, 11519, "Elemental Wall", "Perete",
					"Creates a temporary elemental wall that blocks movement.",
					10, 9, 0, 3 ),
				Spell( 2111, 11520, "Elemental Warp", "Urzeala",
					"Caster is transported to the target location.",
					10, 9, 0, 3 ),

				// === Fourth Sphere (Circle 4) - Mana: 14, MinSkill: 23 ===
				Spell( 2112, 11521, "Elemental Field", "Limite",
					"Creates a wall of elemental energy that deals damage to all who walk through it.",
					14, 23, 1, 4 ),
				Spell( 2113, 11522, "Elemental Restoration", "Restabili",
					"Restores the target of a medium amount of lost hit points.",
					14, 23, 2, 4 ),
				Spell( 2114, 11523, "Elemental Strike", "Lovitura",
					"Strikes the target with elemental energy from above, dealing physical and elemental damage.",
					14, 23, 1, 4 ),
				Spell( 2115, 11524, "Elemental Void", "Mutare",
					"Caster is transported to the location marked on a rune, along with their followers.",
					14, 23, 0, 4 ),

				// === Fifth Sphere (Circle 5) - Mana: 19, MinSkill: 38 ===
				Spell( 2116, 11525, "Elemental Blast", "Deteriora",
					"Makes a powerful elemental blast hit your target with significant damage, dependent on your elementalism and intelligence.",
					19, 38, 1, 5 ),
				Spell( 2117, 11526, "Elemental Echo", "Oglinda",
					"Harmful wizard spells cast at you will be reflected back toward the caster based on your elementalism. Requires a gemstone.",
					19, 38, 0, 5 ),
				Spell( 2118, 11527, "Elemental Fiend", "Diavol",
					"Conjures an elemental creature that attacks a target based off its combat strength and proximity. Requires 2 control slots.",
					19, 38, 0, 5 ),
				Spell( 2119, 11528, "Elemental Hold", "Temnita",
					"Elemental energy emerges to immobilize the target for a brief amount of time. Magic resistance affects the duration.",
					19, 38, 1, 5 ),

				// === Sixth Sphere (Circle 6) - Mana: 24, MinSkill: 52 ===
				Spell( 2120, 11529, "Elemental Barrage", "Baraj",
					"Launches a powerful elemental projectile at the target, dealing significant elemental damage.",
					24, 52, 1, 6 ),
				Spell( 2121, 11530, "Elemental Rune", "Marca",
					"Marks a rune to the elementalist's current location for use with teleportation spells.",
					24, 52, 0, 6 ),
				Spell( 2122, 11531, "Elemental Storm", "Furtuna",
					"Creates an elemental storm around the target, causing physical and elemental damage.",
					24, 52, 1, 6 ),
				Spell( 2123, 11532, "Elemental Summon", "Convoca",
					"A powerful elemental is summoned to serve the caster.",
					24, 52, 0, 6 ),

				// === Seventh Sphere (Circle 7) - Mana: 40, MinSkill: 66 ===
				Spell( 2124, 11533, "Elemental Devastation", "Devasta",
					"Calls down an elemental maelstrom, damaging nearby enemies with elemental damage.",
					40, 66, 1, 7 ),
				Spell( 2125, 11534, "Elemental Fall", "Toamna",
					"Brings down an elemental storm that affects all targets within a radius around the target location. Damage is split amongst targets.",
					40, 66, 1, 7 ),
				Spell( 2126, 11535, "Elemental Gate", "Poarta",
					"Targeting a marked rune opens a temporary portal to the rune's marked location.",
					40, 66, 0, 7 ),
				Spell( 2127, 11536, "Elemental Havoc", "Haotic",
					"Envelopes the target in elemental fury, causing a massive amount of physical and elemental damage.",
					40, 66, 1, 7 ),

				// === Eighth Sphere (Circle 8) - Mana: 50, MinSkill: 80 ===
				Spell( 2128, 11537, "Elemental Apocalypse", "Moarte",
					"Calls down an elemental cataclysm onto foes near the caster, causing devastating elemental damage.",
					50, 80, 1, 8 ),
				Spell( 2129, 11538, "Elemental Lord", "Dumnezeu",
					"A Lord of the Elements is called upon to assist the caster.",
					50, 80, 0, 8 ),
				Spell( 2130, 11539, "Elemental Soul", "Viata",
					"Resurrects another or summons a magical item to resurrect yourself at a later time.",
					50, 80, 2, 8 ),
				Spell( 2131, 11540, "Elemental Spirit", "Fantoma",
					"Summons an elemental spirit that attacks a target based off its intelligence and proximity. Requires 2 control slots.",
					50, 80, 0, 8 )
			};
		}

		private static DynamicSpellDefinition Spell( ushort spellID, ushort iconGraphic, string name, string powerWords,
			string description, byte manaCost, byte minSkill, byte targetType, byte page )
		{
			return new DynamicSpellDefinition
			{
				SpellID = spellID,
				IconGraphic = iconGraphic,
				NameCliloc = 0,
				Name = name,
				PowerWords = powerWords,
				Description = description,
				ManaCost = manaCost,
				MinSkill = minSkill,
				TargetType = targetType,
				Reagents = 0,
				CustomReagents = null,
				Cooldown = 0,
				Page = page
			};
		}

		public List<DynamicInfoPage> GetInfoPages()
		{
			return new List<DynamicInfoPage>
			{
				new DynamicInfoPage
				{
					Title = "Elemental",
					Body = "Unlike other forms of magic, elementalism relies on both the intellect and physical conditions of the spellcaster. No reagents are required. Casting requires mana and stamina, referred to as 'power'."
				},
				new DynamicInfoPage
				{
					Title = "Spellbook",
					Body = "If you manage to find magical items that have lwoer reagent quality, then the stamin required for spells will be reduced proportionally by that value."
				}
			};
		}

		public ulong GetSpellBitmask( Mobile m, Spellbook spellbook )
		{
			if ( spellbook == null )
				return 0;

			return spellbook.Content;
		}

		// === IDynamicSpellbookBookmark ===
		// Mirrors the original ElementalSpellbookGump's bottom-left bookmark (graphic 2095) which
		// opened the ElementalSpellHelp gump. We use ServerCallback so the server decides what to
		// show — keeping the door open for context-aware help (per-element art, per-mobile state).
		public bool HasBookmark { get { return true; } }
		public ushort BookmarkGraphic { get { return 2095; } }
		public ushort BookmarkPressedGraphic { get { return 2095; } }
		public short BookmarkX { get { return 40; } }
		public short BookmarkY { get { return 209; } }
		public ushort BookmarkHue { get { return 0; } }
		public byte BookmarkPage { get { return 0; } }                                       // show on every page
		public byte BookmarkActionType { get { return DynamicBookmarkAction.ServerCallback; } }
		public uint BookmarkAction { get { return 1; } }                                     // token: "open help"
		public string BookmarkTooltip { get { return "Elemental Magic Help"; } }

		public void OnBookmarkPressed( Mobile from, Spellbook spellbook, uint token )
		{
			if ( spellbook == null )
				return;

			// token 1 = open the same help gump as the original elemental spellbook bookmark.
			// Reserve other tokens for future bookmark actions (e.g. focus selector).
			if ( token == 1 )
			{
				from.CloseGump( typeof( ElementalSpellHelp ) );
				from.SendGump( new ElementalSpellHelp( from, spellbook, 1 ) );
			}
		}

		public static void Initialize()
		{
			Console.WriteLine( "[DynamicSpellbook] DynamicElementalSpellbookProvider.Initialize() called" );

			DynamicSpellbookManager.RegisterProvider(
				new DynamicElementalSpellbookProvider(),
				0x6713 // Earth (default graphic for registration)
			);

			Console.WriteLine( "[DynamicSpellbook] DynamicElementalSpellbookProvider registered with item graphic 0x6713, type={0}", (int)SpellbookType.DynamicElementalism );
		}
	}
}
