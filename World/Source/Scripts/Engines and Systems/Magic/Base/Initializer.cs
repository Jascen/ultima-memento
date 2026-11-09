using System;
using Server;

namespace Server.Spells
{
	public class Initializer
	{
		public static void Initialize()
		{
			// First circle
			Register( 00, typeof( First.ClumsySpell ) );
			Register( 01, typeof( First.CreateFoodSpell ) );
			Register( 02, typeof( First.FeeblemindSpell ) );
			Register( 03, typeof( First.HealSpell ) );
			Register( 04, typeof( First.MagicArrowSpell ) );
			Register( 05, typeof( First.NightSightSpell ) );
			Register( 06, typeof( First.ReactiveArmorSpell ) );
			Register( 07, typeof( First.WeakenSpell ) );

			// Second circle
			Register( 08, typeof( Second.AgilitySpell ) );
			Register( 09, typeof( Second.CunningSpell ) );
			Register( 10, typeof( Second.CureSpell ) );
			Register( 11, typeof( Second.HarmSpell ) );
			Register( 12, typeof( Second.MagicTrapSpell ) );
			Register( 13, typeof( Second.RemoveTrapSpell ) );
			Register( 14, typeof( Second.ProtectionSpell ) );
			Register( 15, typeof( Second.StrengthSpell ) );

			// Third circle
			Register( 16, typeof( Third.BlessSpell ) );
			Register( 17, typeof( Third.FireballSpell ) );
			Register( 18, typeof( Third.MagicLockSpell ) );
			Register( 19, typeof( Third.PoisonSpell ) );
			Register( 20, typeof( Third.TelekinesisSpell ) );
			Register( 21, typeof( Third.TeleportSpell ) );
			Register( 22, typeof( Third.UnlockSpell ) );
			Register( 23, typeof( Third.WallOfStoneSpell ) );

			// Fourth circle
			Register( 24, typeof( Fourth.ArchCureSpell ) );
			Register( 25, typeof( Fourth.ArchProtectionSpell ) );
			Register( 26, typeof( Fourth.CurseSpell ) );
			Register( 27, typeof( Fourth.FireFieldSpell ) );
			Register( 28, typeof( Fourth.GreaterHealSpell ) );
			Register( 29, typeof( Fourth.LightningSpell ) );
			Register( 30, typeof( Fourth.ManaDrainSpell ) );
			Register( 31, typeof( Fourth.RecallSpell ) );

			// Fifth circle
			Register( 32, typeof( Fifth.BladeSpiritsSpell ) );
			Register( 33, typeof( Fifth.DispelFieldSpell ) );
			Register( 34, typeof( Fifth.IncognitoSpell ) );
			Register( 35, typeof( Fifth.MagicReflectSpell ) );
			Register( 36, typeof( Fifth.MindBlastSpell ) );
			Register( 37, typeof( Fifth.ParalyzeSpell ) );
			Register( 38, typeof( Fifth.PoisonFieldSpell ) );
			Register( 39, typeof( Fifth.SummonCreatureSpell ) );

			// Sixth circle
			Register( 40, typeof( Sixth.DispelSpell ) );
			Register( 41, typeof( Sixth.EnergyBoltSpell ) );
			Register( 42, typeof( Sixth.ExplosionSpell ) );
			Register( 43, typeof( Sixth.InvisibilitySpell ) );
			Register( 44, typeof( Sixth.MarkSpell ) );
			Register( 45, typeof( Sixth.MassCurseSpell ) );
			Register( 46, typeof( Sixth.ParalyzeFieldSpell ) );
			Register( 47, typeof( Sixth.RevealSpell ) );

			// Seventh circle
			Register( 48, typeof( Seventh.ChainLightningSpell ) );
			Register( 49, typeof( Seventh.EnergyFieldSpell ) );
			Register( 50, typeof( Seventh.FlameStrikeSpell ) );
			Register( 51, typeof( Seventh.GateTravelSpell ) );
			Register( 52, typeof( Seventh.ManaVampireSpell ) );
			Register( 53, typeof( Seventh.MassDispelSpell ) );
			Register( 54, typeof( Seventh.MeteorSwarmSpell ) );
			Register( 55, typeof( Seventh.PolymorphSpell ) );

			// Eighth circle
			Register( 56, typeof( Eighth.EarthquakeSpell ) );
			Register( 57, typeof( Eighth.EnergyVortexSpell ) );
			Register( 58, typeof( Eighth.ResurrectionSpell ) );
			Register( 59, typeof( Eighth.AirElementalSpell ) );
			Register( 60, typeof( Eighth.SummonDaemonSpell ) );
			Register( 61, typeof( Eighth.EarthElementalSpell ) );
			Register( 62, typeof( Eighth.FireElementalSpell ) );
			Register( 63, typeof( Eighth.WaterElementalSpell ) );

			// Necromancy spells
			Register( 100, typeof( Necromancy.AnimateDeadSpell ) );
			Register( 101, typeof( Necromancy.BloodOathSpell ) );
			Register( 102, typeof( Necromancy.CorpseSkinSpell ) );
			Register( 103, typeof( Necromancy.CurseWeaponSpell ) );
			Register( 104, typeof( Necromancy.EvilOmenSpell ) );
			Register( 105, typeof( Necromancy.HorrificBeastSpell ) );
			Register( 106, typeof( Necromancy.LichFormSpell ) );
			Register( 107, typeof( Necromancy.MindRotSpell ) );
			Register( 108, typeof( Necromancy.PainSpikeSpell ) );
			Register( 109, typeof( Necromancy.PoisonStrikeSpell ) );
			Register( 110, typeof( Necromancy.StrangleSpell ) );
			Register( 111, typeof( Necromancy.SummonFamiliarSpell ) );
			Register( 112, typeof( Necromancy.VampiricEmbraceSpell ) );
			Register( 113, typeof( Necromancy.VengefulSpiritSpell ) );
			Register( 114, typeof( Necromancy.WitherSpell ) );
			Register( 115, typeof( Necromancy.WraithFormSpell ) );
			Register( 116, typeof( Necromancy.ExorcismSpell ) );

			// Witch brews
			Register( 131, typeof( Undead.SpectreShadowSpell ) );
			Register( 132, typeof( Undead.ManaLeechSpell ) );
			Register( 133, typeof( Undead.UndeadCurePoisonSpell ) );
			Register( 134, typeof( Undead.HellsBrandSpell ) );
			Register( 135, typeof( Undead.UndeadGraveyardGatewaySpell ) );
			Register( 136, typeof( Undead.RetchedAirSpell ) );
			Register( 137, typeof( Undead.UndeadEyesSpell ) );
			Register( 138, typeof( Undead.UndeadWallOfSpikesSpell ) );
			Register( 139, typeof( Undead.VampireGiftSpell ) );
			Register( 140, typeof( Undead.UnBloodlessSpecPactSpell ) );
			Register( 141, typeof( Undead.NecroPoisonSpell ) );
			Register( 142, typeof( Undead.HellsGateSpell ) );
			Register( 143, typeof( Undead.GhostlyImagesSpell ) );
			Register( 144, typeof( Undead.GhostPhaseSpell ) );
			Register( 145, typeof( Undead.NecroUnlockSpell ) );
			Register( 146, typeof( Undead.PhantasmSpell ) );

			// Druid mixtures
			Register( 147, typeof( Herbalist.ShieldOfEarthSpell ) );
			Register( 148, typeof( Herbalist.WoodlandProtectionSpell ) );
			Register( 149, typeof( Herbalist.ProtectiveFairySpell ) );
			Register( 150, typeof( Herbalist.HerbalHealingSpell ) );
			Register( 151, typeof( Herbalist.GraspingRootsSpell ) );
			Register( 152, typeof( Herbalist.BlendWithForestSpell ) );
			Register( 153, typeof( Herbalist.SwarmOfInsectsSpell ) );
			Register( 154, typeof( Herbalist.VolcanicEruptionSpell ) );
			Register( 155, typeof( Herbalist.TreefellowSpell ) );
			Register( 156, typeof( Herbalist.StoneCircleSpell ) );
			Register( 157, typeof( Herbalist.DruidicRuneSpell ) );
			Register( 158, typeof( Herbalist.LureStoneSpell ) );
			Register( 159, typeof( Herbalist.NaturesPassageSpell ) );
			Register( 160, typeof( Herbalist.MushroomGatewaySpell ) );
			Register( 161, typeof( Herbalist.RestorativeSoilSpell ) );
			Register( 162, typeof( Herbalist.FireflySpell ) );

			// Elemental Spells
			Register( 300, typeof( Elementalism.Elemental_Armor_Spell ) );
			Register( 301, typeof( Elementalism.Elemental_Bolt_Spell ) );
			Register( 302, typeof( Elementalism.Elemental_Mend_Spell ) );
			Register( 303, typeof( Elementalism.Elemental_Sanctuary_Spell ) );
			Register( 304, typeof( Elementalism.Elemental_Pain_Spell ) );
			Register( 305, typeof( Elementalism.Elemental_Protection_Spell ) );
			Register( 306, typeof( Elementalism.Elemental_Purge_Spell ) );
			Register( 307, typeof( Elementalism.Elemental_Steed_Spell ) );
			Register( 308, typeof( Elementalism.Elemental_Call_Spell ) );
			Register( 309, typeof( Elementalism.Elemental_Force_Spell ) );
			Register( 310, typeof( Elementalism.Elemental_Wall_Spell ) );
			Register( 311, typeof( Elementalism.Elemental_Warp_Spell ) );
			Register( 312, typeof( Elementalism.Elemental_Field_Spell ) );
			Register( 313, typeof( Elementalism.Elemental_Restoration_Spell ) );
			Register( 314, typeof( Elementalism.Elemental_Strike_Spell ) );
			Register( 315, typeof( Elementalism.Elemental_Void_Spell ) );
			Register( 316, typeof( Elementalism.Elemental_Blast_Spell ) );
			Register( 317, typeof( Elementalism.Elemental_Echo_Spell ) );
			Register( 318, typeof( Elementalism.Elemental_Fiend_Spell ) );
			Register( 319, typeof( Elementalism.Elemental_Hold_Spell ) );
			Register( 320, typeof( Elementalism.Elemental_Barrage_Spell ) );
			Register( 321, typeof( Elementalism.Elemental_Rune_Spell ) );
			Register( 322, typeof( Elementalism.Elemental_Storm_Spell ) );
			Register( 323, typeof( Elementalism.Elemental_Summon_Spell ) );
			Register( 324, typeof( Elementalism.Elemental_Devastation_Spell ) );
			Register( 325, typeof( Elementalism.Elemental_Fall_Spell ) );
			Register( 326, typeof( Elementalism.Elemental_Gate_Spell ) );
			Register( 327, typeof( Elementalism.Elemental_Havoc_Spell ) );
			Register( 328, typeof( Elementalism.Elemental_Apocalypse_Spell ) );
			Register( 329, typeof( Elementalism.Elemental_Lord_Spell ) );
			Register( 330, typeof( Elementalism.Elemental_Soul_Spell ) );
			Register( 331, typeof( Elementalism.Elemental_Spirit_Spell ) );

			// Paladin abilities
			Register( Chivalry.CleanseByFireSpell.SpellInfo.SpellDefinition.SpellID, typeof( Chivalry.CleanseByFireSpell ) );
			Register( Chivalry.CloseWoundsSpell.SpellInfo.SpellDefinition.SpellID, typeof( Chivalry.CloseWoundsSpell ) );
			Register( Chivalry.ConsecrateWeaponSpell.SpellInfo.SpellDefinition.SpellID, typeof( Chivalry.ConsecrateWeaponSpell ) );
			Register( Chivalry.DispelEvilSpell.SpellInfo.SpellDefinition.SpellID, typeof( Chivalry.DispelEvilSpell ) );
			Register( Chivalry.DivineFurySpell.SpellInfo.SpellDefinition.SpellID, typeof( Chivalry.DivineFurySpell ) );
			Register( Chivalry.EnemyOfOneSpell.SpellInfo.SpellDefinition.SpellID, typeof( Chivalry.EnemyOfOneSpell ) );
			Register( Chivalry.HolyLightSpell.SpellInfo.SpellDefinition.SpellID, typeof( Chivalry.HolyLightSpell ) );
			Register( Chivalry.NobleSacrificeSpell.SpellInfo.SpellDefinition.SpellID, typeof( Chivalry.NobleSacrificeSpell ) );
			Register( Chivalry.RemoveCurseSpell.SpellInfo.SpellDefinition.SpellID, typeof( Chivalry.RemoveCurseSpell ) );
			Register( Chivalry.SacredJourneySpell.SpellInfo.SpellDefinition.SpellID, typeof( Chivalry.SacredJourneySpell ) );

			// Spell providers
			SpellProviderRegistry.Register( Mystic.MysticSpellProvider.SpellDefinitions );
			SpellProviderRegistry.Register( Jester.JesterSpellProvider.SpellDefinitions );
			SpellProviderRegistry.Register( Syth.SythSpellProvider.SpellDefinitions );
			SpellProviderRegistry.Register( Jedi.JediSpellProvider.SpellDefinitions );
			SpellProviderRegistry.Register( Shinobi.ShinobiSpellProvider.SpellDefinitions );
			SpellProviderRegistry.Register( DeathKnight.DeathKnightSpellProvider.SpellDefinitions );
			SpellProviderRegistry.Register( HolyMan.HolyManSpellProvider.SpellDefinitions );
			SpellProviderRegistry.Register( Song.BardSongProvider.SpellDefinitions );
			SpellProviderRegistry.Register( Research.ResearchSpellProvider.SpellDefinitions );

			// Samurai abilities
			Register( 400, typeof( Bushido.HonorableExecution ) );
			Register( 401, typeof( Bushido.Confidence ) );
			Register( 402, typeof( Bushido.Evasion ) );
			Register( 403, typeof( Bushido.CounterAttack ) );
			Register( 404, typeof( Bushido.LightningStrike ) );
			Register( 405, typeof( Bushido.MomentumStrike ) );

			// Ninja abilities
			Register( 500, typeof( Ninjitsu.FocusAttack ) );
			Register( 501, typeof( Ninjitsu.DeathStrike ) );
			Register( 502, typeof( Ninjitsu.AnimalForm ) );
			Register( 503, typeof( Ninjitsu.KiAttack ) );
			Register( 504, typeof( Ninjitsu.SurpriseAttack ) );
			Register( 505, typeof( Ninjitsu.Backstab ) );
			Register( 506, typeof( Ninjitsu.Shadowjump ) );
			Register( 507, typeof( Ninjitsu.MirrorImage ) );

			// Misc Item Spells
			Register( 700, typeof( Magical.SummonSnakesSpell ) );
			Register( 701, typeof( Magical.SummonDragonSpell ) );
			Register( 702, typeof( Magical.ThorLightningSpell ) );
			Register( 703, typeof( Magical.AttackSpells ) );
			Register( 704, typeof( Magical.SummonSkeletonSpell ) );
			Register( 705, typeof( Magical.IdentifySpell ) );
			Register( 706, typeof( Magical.TravelSpell ) );
		}

		public static void Register( int spellID, Type type )
		{
			SpellRegistry.Register( spellID, type );
		}
	}
}