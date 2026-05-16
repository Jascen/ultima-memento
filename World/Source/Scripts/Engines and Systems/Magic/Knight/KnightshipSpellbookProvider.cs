using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells.Dynamic;

namespace Server.Spells.Chivalry
{
	public class KnightshipSpellbookProvider : IDynamicSpellbookProvider
	{
		public SpellbookType SpellbookType { get { return SpellbookType.Knightship; } }

		// Gump graphics matching the Chivalry spellbook style
		public ushort BookGraphic { get { return 0x2B01; } }
		public ushort MinimizedGraphic { get { return 0x2B04; } }

		public byte SpellsPerPageSide { get { return 5; } }
		public byte MaxDictionaryPages { get { return 2; } }

		public bool DisplayManaCost { get { return true; } }
		public bool DisplayMinSkill { get { return true; } }
		public bool DisplayPowerWords { get { return true; } }
		public string ManaCostLabel { get { return null; } }
		public string MinSkillLabel { get { return null; } }

		// Display tithing points on page 1 of the spellbook
		public string CustomPropertyTitle { get { return "Tithing Points"; } }
		public string CustomPropertyLabel { get { return "Available"; } }
		public string CustomPropertyName { get { return "TithingPoints"; } }

		public string[] GetPageNames()
		{
			return null;
		}

		public List<DynamicSpellDefinition> GetSpellDefinitions()
		{
			return new List<DynamicSpellDefinition>
			{
				new DynamicSpellDefinition
				{
					SpellID = 2000,
					IconGraphic = 0x5100,
					NameCliloc = 0,
					Name = "Cleanse By Fire",
					PowerWords = "Expor Flamus",
					Description = "Cures the target of poisons, but causes the caster to be burned by fire damage. The amount of fire damage is lessened if the caster has high Karma.",
					ManaCost = 10,
					MinSkill = 5,
					TargetType = 2, // Beneficial
					Reagents = 0,
					CustomReagents = null,
					Cooldown = 0,
					Page = 1
				},
				new DynamicSpellDefinition
				{
					SpellID = 2001,
					IconGraphic = 0x5101,
					NameCliloc = 0,
					Name = "Close Wounds",
					PowerWords = "Obsu Vulni",
					Description = "Heals the target for 7 to 39 points of damage. The caster's Karma affects the amount of damage healed.",
					ManaCost = 10,
					MinSkill = 0,
					TargetType = 2, // Beneficial
					Reagents = 0,
					CustomReagents = null,
					Cooldown = 0,
					Page = 1
				},
				new DynamicSpellDefinition
				{
					SpellID = 2002,
					IconGraphic = 0x5102,
					NameCliloc = 0,
					Name = "Consecrate Weapon",
					PowerWords = "Consecrus Arma",
					Description = "Temporarily enchants your equipped weapon to deal damage based on the target's weakest elemental resistance.",
					ManaCost = 10,
					MinSkill = 15,
					TargetType = 0, // Neutral (self-buff)
					Reagents = 0,
					CustomReagents = null,
					Cooldown = 0,
					Page = 1
				},
				new DynamicSpellDefinition
				{
					SpellID = 2003,
					IconGraphic = 0x5103,
					NameCliloc = 0,
					Name = "Dispel Evil",
					PowerWords = "Dispiro Malum",
					Description = "Attempts to dispel summoned creatures, cause evil creatures to flee, and damage necromancers in a wide area around the caster.",
					ManaCost = 10,
					MinSkill = 35,
					TargetType = 1, // Harmful
					Reagents = 0,
					CustomReagents = null,
					Cooldown = 0,
					Page = 1
				},
				new DynamicSpellDefinition
				{
					SpellID = 2004,
					IconGraphic = 0x5104,
					NameCliloc = 0,
					Name = "Divine Fury",
					PowerWords = "Divinum Furis",
					Description = "Restores your stamina to maximum and increases your weapon damage for a short duration. The duration is affected by your Karma.",
					ManaCost = 15,
					MinSkill = 25,
					TargetType = 0, // Neutral (self-buff)
					Reagents = 0,
					CustomReagents = null,
					Cooldown = 0,
					Page = 1
				},
				new DynamicSpellDefinition
				{
					SpellID = 2005,
					IconGraphic = 0x5105,
					NameCliloc = 0,
					Name = "Enemy of One",
					PowerWords = "Forul Solum",
					Description = "Increases damage dealt to a specific type of enemy for a moderate duration. The duration is affected by your Karma.",
					ManaCost = 20,
					MinSkill = 45,
					TargetType = 0, // Neutral (self-buff)
					Reagents = 0,
					CustomReagents = null,
					Cooldown = 0,
					Page = 1
				},
				new DynamicSpellDefinition
				{
					SpellID = 2006,
					IconGraphic = 0x5106,
					NameCliloc = 0,
					Name = "Holy Light",
					PowerWords = "Augus Luminos",
					Description = "Deals holy damage to all evil creatures within a small radius around the caster. Damage is affected by Karma.",
					ManaCost = 10,
					MinSkill = 55,
					TargetType = 1, // Harmful
					Reagents = 0,
					CustomReagents = null,
					Cooldown = 0,
					Page = 1
				},
				new DynamicSpellDefinition
				{
					SpellID = 2007,
					IconGraphic = 0x5107,
					NameCliloc = 0,
					Name = "Noble Sacrifice",
					PowerWords = "Dium Prostra",
					Description = "Attempts to resurrect, cure, and heal all allies within a small radius. The caster's health, mana, and stamina are reduced to 1 if any target is assisted.",
					ManaCost = 20,
					MinSkill = 65,
					TargetType = 2, // Beneficial
					Reagents = 0,
					CustomReagents = null,
					Cooldown = 0,
					Page = 1
				},
				new DynamicSpellDefinition
				{
					SpellID = 2008,
					IconGraphic = 0x5108,
					NameCliloc = 0,
					Name = "Remove Curse",
					PowerWords = "Extermo Vomica",
					Description = "Removes all curse debuffs from the target including clumsy, weaken, feeblemind, paralyze, evil omen, strangle, corpse skin, mortal strike, mind rot, and blood oath.",
					ManaCost = 20,
					MinSkill = 5,
					TargetType = 2, // Beneficial
					Reagents = 0,
					CustomReagents = null,
					Cooldown = 0,
					Page = 1
				},
				new DynamicSpellDefinition
				{
					SpellID = 2009,
					IconGraphic = 0x5109,
					NameCliloc = 0,
					Name = "Sacred Journey",
					PowerWords = "Sanctum Viatas",
					Description = "Teleports the caster to a location marked on a recall rune or runebook. Similar to the Recall spell.",
					ManaCost = 10,
					MinSkill = 15,
					TargetType = 0, // Neutral
					Reagents = 0,
					CustomReagents = null,
					Cooldown = 0,
					Page = 1
				}
			};
		}

		// Layout: use client defaults (0 = default)
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
		public List<DynamicInfoPage> GetInfoPages() { return null; }

		public ulong GetSpellBitmask( Mobile m, Spellbook spellbook )
		{
			if ( spellbook == null )
				return 0;

			return spellbook.Content;
		}

		// Auto-called by the server on startup via Initialize
		public static void Initialize()
		{
			Console.WriteLine( "[DynamicSpellbook] KnightshipSpellbookProvider.Initialize() called" );

			DynamicSpellbookManager.RegisterProvider(
				new KnightshipSpellbookProvider(),
				0x2252 // Same item art as Book of Chivalry
			);

			Console.WriteLine( "[DynamicSpellbook] KnightshipSpellbookProvider registered with item graphic 0x2252, type={0}", (int)SpellbookType.Knightship );
		}
	}
}
