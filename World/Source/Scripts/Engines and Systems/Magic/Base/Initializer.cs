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
			Register( 200, typeof( Chivalry.CleanseByFireSpell ) );
			Register( 201, typeof( Chivalry.CloseWoundsSpell ) );
			Register( 202, typeof( Chivalry.ConsecrateWeaponSpell ) );
			Register( 203, typeof( Chivalry.DispelEvilSpell ) );
			Register( 204, typeof( Chivalry.DivineFurySpell ) );
			Register( 205, typeof( Chivalry.EnemyOfOneSpell ) );
			Register( 206, typeof( Chivalry.HolyLightSpell ) );
			Register( 207, typeof( Chivalry.NobleSacrificeSpell ) );
			Register( 208, typeof( Chivalry.RemoveCurseSpell ) );
			Register( 209, typeof( Chivalry.SacredJourneySpell ) );

			// Mystic abilities
			Register( 250, typeof( Mystic.AstralProjection ) );
			Register( 251, typeof( Mystic.AstralTravel ) );
			Register( 252, typeof( Mystic.CreateRobe ) );
			Register( 253, typeof( Mystic.GentleTouch ) );
			Register( 254, typeof( Mystic.Leap ) );
			Register( 255, typeof( Mystic.PsionicBlast ) );
			Register( 256, typeof( Mystic.PsychicWall ) );
			Register( 257, typeof( Mystic.PurityOfBody ) );
			Register( 258, typeof( Mystic.QuiveringPalm ) );
			Register( 259, typeof( Mystic.WindRunner ) );

			// Jester abilities
			Register( 260, typeof( Jester.CanOfSnakes ) );
			Register( 261, typeof( Jester.Clowns ) );
			Register( 262, typeof( Jester.FlowerPower ) );
			Register( 263, typeof( Jester.Hilarity ) );
			Register( 264, typeof( Jester.Insult ) );
			Register( 265, typeof( Jester.JumpAround ) );
			Register( 266, typeof( Jester.PoppingBalloon ) );
			Register( 267, typeof( Jester.RabbitInAHat ) );
			Register( 268, typeof( Jester.SeltzerBottle ) );
			Register( 269, typeof( Jester.SurpriseGift ) );

			// Syth abilities
			Register( 270, typeof( Syth.Psychokinesis ) );
			Register( 271, typeof( Syth.DeathGrip ) );
			Register( 272, typeof( Syth.Projection ) );
			Register( 273, typeof( Syth.ThrowSword ) );
			Register( 274, typeof( Syth.SythSpeed ) );
			Register( 275, typeof( Syth.SythLightning ) );
			Register( 276, typeof( Syth.Absorption ) );
			Register( 277, typeof( Syth.PsychicBlast ) );
			Register( 278, typeof( Syth.DrainLife ) );
			Register( 279, typeof( Syth.CloneBody ) );

			// Jedi abilities
			Register( 280, typeof( Jedi.ForceGrip ) );
			Register( 281, typeof( Jedi.MindsEye ) );
			Register( 282, typeof( Jedi.Mirage ) );
			Register( 283, typeof( Jedi.ThrowSabre ) );
			Register( 284, typeof( Jedi.Celerity ) );
			Register( 285, typeof( Jedi.PsychicAura ) );
			Register( 286, typeof( Jedi.Deflection ) );
			Register( 287, typeof( Jedi.SoothingTouch ) );
			Register( 288, typeof( Jedi.StasisField ) );
			Register( 289, typeof( Jedi.Replicate ) );

			// Shinobi abilities
			Register( 290, typeof( Shinobi.CheetahPaws ) );
			Register( 291, typeof( Shinobi.Deception ) );
			Register( 292, typeof( Shinobi.EagleEye ) );
			Register( 293, typeof( Shinobi.Espionage ) );
			Register( 294, typeof( Shinobi.FerretFlee ) );
			Register( 295, typeof( Shinobi.MonkeyLeap ) );
			Register( 296, typeof( Shinobi.MysticShuriken ) );
			Register( 297, typeof( Shinobi.TigerStrength ) );

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

			Register( 600, typeof( Research.ResearchAerialServant ) );
			Register( 601, typeof( Research.ResearchAirWalk ) );
			Register( 602, typeof( Research.ResearchAvalanche ) );
			Register( 603, typeof( Research.ResearchBanishDaemon ) );
			Register( 604, typeof( Research.ResearchCallDestruction ) );
			Register( 605, typeof( Research.ResearchCauseFear ) );
			Register( 606, typeof( Research.ResearchCharm ) );
			Register( 607, typeof( Research.ResearchClone ) );
			Register( 608, typeof( Research.ResearchConflagration ) );
			Register( 609, typeof( Research.ResearchConfusionBlast ) );
			Register( 610, typeof( Research.ResearchConjure ) );
			Register( 611, typeof( Research.ResearchCreateFire ) );
			Register( 612, typeof( Research.ResearchCreateGold ) );
			Register( 613, typeof( Research.ResearchCreateGolem ) );
			Register( 614, typeof( Research.ResearchDeathSpeak ) );
			Register( 615, typeof( Research.ResearchDeathVortex ) );
			Register( 616, typeof( Research.ResearchDevastation ) );
			Register( 617, typeof( Research.ResearchDivination ) );
			Register( 618, typeof( Research.ResearchEnchant ) );
			Register( 619, typeof( Research.ResearchEndureCold ) );
			Register( 620, typeof( Research.ResearchEndureHeat ) );
			Register( 621, typeof( Research.ResearchGasCloud ) );
			Register( 622, typeof( Research.ResearchEtherealTravel ) );
			Register( 623, typeof( Research.ResearchExplosion ) );
			Register( 624, typeof( Research.ResearchExtinguish ) );
			Register( 625, typeof( Research.ResearchFadefromSight ) );
			Register( 626, typeof( Research.ResearchFlameBolt ) );
			Register( 627, typeof( Research.ResearchFrostField ) );
			Register( 628, typeof( Research.ResearchFrostStrike ) );
			Register( 629, typeof( Research.ResearchGrantPeace ) );
			Register( 630, typeof( Research.ResearchHailStorm ) );
			Register( 631, typeof( Research.ResearchHealingTouch ) );
			Register( 632, typeof( Research.ResearchIcicle ) );
			Register( 633, typeof( Research.ResearchIgnite ) );
			Register( 634, typeof( Research.ResearchIntervention ) );
			Register( 635, typeof( Research.ResearchMagicSteed ) );
			Register( 636, typeof( Research.ResearchMaskofDeath ) );
			Register( 637, typeof( Research.ResearchMassDeath ) );
			Register( 638, typeof( Research.ResearchMassMight ) );
			Register( 639, typeof( Research.ResearchMassSleep ) );
			Register( 640, typeof( Research.ResearchMeteorShower ) );
			Register( 641, typeof( Research.ResearchOpenGround ) );
			Register( 642, typeof( Research.ResearchRestoration ) );
			Register( 643, typeof( Research.ResearchRingofFire ) );
			Register( 644, typeof( Research.ResearchRockFlesh ) );
			Register( 645, typeof( Research.ResearchSeeTruth ) );
			Register( 646, typeof( Research.ResearchSleep ) );
			Register( 647, typeof( Research.ResearchSleepField ) );
			Register( 648, typeof( Research.ResearchSneak ) );
			Register( 649, typeof( Research.ResearchSnowBall ) );
			Register( 650, typeof( Research.ResearchSummonAcidElemental ) );
			Register( 651, typeof( Research.ResearchSummonBloodElemental ) );
			Register( 652, typeof( Research.ResearchSummonCreature ) );
			Register( 653, typeof( Research.ResearchSummonDead ) );
			Register( 654, typeof( Research.ResearchSummonDevil ) );
			Register( 655, typeof( Research.ResearchSummonElectricalElemental ) );
			Register( 656, typeof( Research.ResearchSummonGemElemental ) );
			Register( 657, typeof( Research.ResearchSummonIceElemental ) );
			Register( 658, typeof( Research.ResearchSummonMudElemental ) );
			Register( 659, typeof( Research.ResearchSummonPoisonElemental ) );
			Register( 660, typeof( Research.ResearchSummonWeedElemental ) );
			Register( 661, typeof( Research.ResearchSwarm ) );
			Register( 662, typeof( Research.ResearchWithstandDeath ) );
			Register( 663, typeof( Research.ResearchWizardEye ) );

			// Misc Item Spells
			Register( 700, typeof( Magical.SummonSnakesSpell ) );
			Register( 701, typeof( Magical.SummonDragonSpell ) );
			Register( 702, typeof( Magical.ThorLightningSpell ) );
			Register( 703, typeof( Magical.AttackSpells ) );
			Register( 704, typeof( Magical.SummonSkeletonSpell ) );
			Register( 705, typeof( Magical.IdentifySpell ) );
			Register( 706, typeof( Magical.TravelSpell ) );

			// Death Knight Spells
			Register( 750, typeof( DeathKnight.BanishSpell ) );
			Register( 751, typeof( DeathKnight.DemonicTouchSpell ) );
			Register( 752, typeof( DeathKnight.DevilPactSpell ) );
			Register( 753, typeof( DeathKnight.GrimReaperSpell ) );
			Register( 754, typeof( DeathKnight.HagHandSpell ) );
			Register( 755, typeof( DeathKnight.HellfireSpell ) );
			Register( 756, typeof( DeathKnight.LucifersBoltSpell ) );
			Register( 757, typeof( DeathKnight.OrbOfOrcusSpell ) );
			Register( 758, typeof( DeathKnight.ShieldOfHateSpell ) );
			Register( 759, typeof( DeathKnight.SoulReaperSpell ) );
			Register( 760, typeof( DeathKnight.StrengthOfSteelSpell ) );
			Register( 761, typeof( DeathKnight.StrikeSpell ) );
			Register( 762, typeof( DeathKnight.SuccubusSkinSpell ) );
			Register( 763, typeof( DeathKnight.WrathSpell ) );

			// Holy Man Spells
			Register( 770, typeof( HolyMan.BanishEvilSpell ) );
			Register( 771, typeof( HolyMan.DampenSpiritSpell ) );
			Register( 772, typeof( HolyMan.EnchantSpell ) );
			Register( 773, typeof( HolyMan.HammerOfFaithSpell ) );
			Register( 774, typeof( HolyMan.HeavenlyLightSpell ) );
			Register( 775, typeof( HolyMan.NourishSpell ) );
			Register( 776, typeof( HolyMan.PurgeSpell ) );
			Register( 777, typeof( HolyMan.RebirthSpell ) );
			Register( 778, typeof( HolyMan.SacredBoonSpell ) );
			Register( 779, typeof( HolyMan.SanctifySpell ) );
			Register( 780, typeof( HolyMan.SeanceSpell ) );
			Register( 781, typeof( HolyMan.SmiteSpell ) );
			Register( 782, typeof( HolyMan.TouchOfLifeSpell ) );
			Register( 783, typeof( HolyMan.TrialByFireSpell ) );

			// Bard Songs
			Register( 351, typeof( Song.ArmysPaeonSong ) );
			Register( 352, typeof( Song.EnchantingEtudeSong ) );
			Register( 353, typeof( Song.EnergyCarolSong ) );
			Register( 354, typeof( Song.EnergyThrenodySong ) );
			Register( 355, typeof( Song.FireCarolSong ) );
			Register( 356, typeof( Song.FireThrenodySong ) );
			Register( 357, typeof( Song.FoeRequiemSong ) );
			Register( 358, typeof( Song.IceCarolSong ) );
			Register( 359, typeof( Song.IceThrenodySong ) );
			Register( 360, typeof( Song.KnightsMinneSong ) );
			Register( 361, typeof( Song.MagesBalladSong ) );
			Register( 362, typeof( Song.MagicFinaleSong ) );
			Register( 363, typeof( Song.PoisonCarolSong ) );
			Register( 364, typeof( Song.PoisonThrenodySong ) );
			Register( 365, typeof( Song.SheepfoeMamboSong ) );
			Register( 366, typeof( Song.SinewyEtudeSong ) );

			// Knightship abilities (dynamic spellbook - mirrors Chivalry spells at new IDs)
			Register( 2000, typeof( Chivalry.CleanseByFireSpell ) );
			Register( 2001, typeof( Chivalry.CloseWoundsSpell ) );
			Register( 2002, typeof( Chivalry.ConsecrateWeaponSpell ) );
			Register( 2003, typeof( Chivalry.DispelEvilSpell ) );
			Register( 2004, typeof( Chivalry.DivineFurySpell ) );
			Register( 2005, typeof( Chivalry.EnemyOfOneSpell ) );
			Register( 2006, typeof( Chivalry.HolyLightSpell ) );
			Register( 2007, typeof( Chivalry.NobleSacrificeSpell ) );
			Register( 2008, typeof( Chivalry.RemoveCurseSpell ) );
			Register( 2009, typeof( Chivalry.SacredJourneySpell ) );

			// Dynamic Elementalism (mirrors Elementalism spells at new IDs 2100-2131)
			Register( 2100, typeof( Elementalism.Elemental_Armor_Spell ) );
			Register( 2101, typeof( Elementalism.Elemental_Bolt_Spell ) );
			Register( 2102, typeof( Elementalism.Elemental_Mend_Spell ) );
			Register( 2103, typeof( Elementalism.Elemental_Sanctuary_Spell ) );
			Register( 2104, typeof( Elementalism.Elemental_Pain_Spell ) );
			Register( 2105, typeof( Elementalism.Elemental_Protection_Spell ) );
			Register( 2106, typeof( Elementalism.Elemental_Purge_Spell ) );
			Register( 2107, typeof( Elementalism.Elemental_Steed_Spell ) );
			Register( 2108, typeof( Elementalism.Elemental_Call_Spell ) );
			Register( 2109, typeof( Elementalism.Elemental_Force_Spell ) );
			Register( 2110, typeof( Elementalism.Elemental_Wall_Spell ) );
			Register( 2111, typeof( Elementalism.Elemental_Warp_Spell ) );
			Register( 2112, typeof( Elementalism.Elemental_Field_Spell ) );
			Register( 2113, typeof( Elementalism.Elemental_Restoration_Spell ) );
			Register( 2114, typeof( Elementalism.Elemental_Strike_Spell ) );
			Register( 2115, typeof( Elementalism.Elemental_Void_Spell ) );
			Register( 2116, typeof( Elementalism.Elemental_Blast_Spell ) );
			Register( 2117, typeof( Elementalism.Elemental_Echo_Spell ) );
			Register( 2118, typeof( Elementalism.Elemental_Fiend_Spell ) );
			Register( 2119, typeof( Elementalism.Elemental_Hold_Spell ) );
			Register( 2120, typeof( Elementalism.Elemental_Barrage_Spell ) );
			Register( 2121, typeof( Elementalism.Elemental_Rune_Spell ) );
			Register( 2122, typeof( Elementalism.Elemental_Storm_Spell ) );
			Register( 2123, typeof( Elementalism.Elemental_Summon_Spell ) );
			Register( 2124, typeof( Elementalism.Elemental_Devastation_Spell ) );
			Register( 2125, typeof( Elementalism.Elemental_Fall_Spell ) );
			Register( 2126, typeof( Elementalism.Elemental_Gate_Spell ) );
			Register( 2127, typeof( Elementalism.Elemental_Havoc_Spell ) );
			Register( 2128, typeof( Elementalism.Elemental_Apocalypse_Spell ) );
			Register( 2129, typeof( Elementalism.Elemental_Lord_Spell ) );
			Register( 2130, typeof( Elementalism.Elemental_Soul_Spell ) );
			Register( 2131, typeof( Elementalism.Elemental_Spirit_Spell ) );

			// Dynamic Song (mirrors Bard spells at new IDs 2200-2215)
			Register( 2200, typeof( Song.ArmysPaeonSong ) );
			Register( 2201, typeof( Song.EnchantingEtudeSong ) );
			Register( 2202, typeof( Song.EnergyCarolSong ) );
			Register( 2203, typeof( Song.EnergyThrenodySong ) );
			Register( 2204, typeof( Song.FireCarolSong ) );
			Register( 2205, typeof( Song.FireThrenodySong ) );
			Register( 2206, typeof( Song.FoeRequiemSong ) );
			Register( 2207, typeof( Song.IceCarolSong ) );
			Register( 2208, typeof( Song.IceThrenodySong ) );
			Register( 2209, typeof( Song.KnightsMinneSong ) );
			Register( 2210, typeof( Song.MagesBalladSong ) );
			Register( 2211, typeof( Song.MagicFinaleSong ) );
			Register( 2212, typeof( Song.PoisonCarolSong ) );
			Register( 2213, typeof( Song.PoisonThrenodySong ) );
			Register( 2214, typeof( Song.SheepfoeMamboSong ) );
			Register( 2215, typeof( Song.SinewyEtudeSong ) );

			// Dynamic Death Knight (mirrors Death Knight spells at new IDs 2300-2313)
			Register( 2300, typeof( DeathKnight.BanishSpell ) );
			Register( 2301, typeof( DeathKnight.DemonicTouchSpell ) );
			Register( 2302, typeof( DeathKnight.DevilPactSpell ) );
			Register( 2303, typeof( DeathKnight.GrimReaperSpell ) );
			Register( 2304, typeof( DeathKnight.HagHandSpell ) );
			Register( 2305, typeof( DeathKnight.HellfireSpell ) );
			Register( 2306, typeof( DeathKnight.LucifersBoltSpell ) );
			Register( 2307, typeof( DeathKnight.OrbOfOrcusSpell ) );
			Register( 2308, typeof( DeathKnight.ShieldOfHateSpell ) );
			Register( 2309, typeof( DeathKnight.SoulReaperSpell ) );
			Register( 2310, typeof( DeathKnight.StrengthOfSteelSpell ) );
			Register( 2311, typeof( DeathKnight.StrikeSpell ) );
			Register( 2312, typeof( DeathKnight.SuccubusSkinSpell ) );
			Register( 2313, typeof( DeathKnight.WrathSpell ) );

			// Dynamic Druidism (mirrors Herbalist spells at new IDs 2400-2415)
			Register( 2400, typeof( Herbalist.LureStoneSpell ) );
			Register( 2401, typeof( Herbalist.NaturesPassageSpell ) );
			Register( 2402, typeof( Herbalist.ShieldOfEarthSpell ) );
			Register( 2403, typeof( Herbalist.WoodlandProtectionSpell ) );
			Register( 2404, typeof( Herbalist.StoneCircleSpell ) );
			Register( 2405, typeof( Herbalist.GraspingRootsSpell ) );
			Register( 2406, typeof( Herbalist.DruidicRuneSpell ) );
			Register( 2407, typeof( Herbalist.HerbalHealingSpell ) );
			Register( 2408, typeof( Herbalist.BlendWithForestSpell ) );
			Register( 2409, typeof( Herbalist.FireflySpell ) );
			Register( 2410, typeof( Herbalist.MushroomGatewaySpell ) );
			Register( 2411, typeof( Herbalist.SwarmOfInsectsSpell ) );
			Register( 2412, typeof( Herbalist.ProtectiveFairySpell ) );
			Register( 2413, typeof( Herbalist.TreefellowSpell ) );
			Register( 2414, typeof( Herbalist.VolcanicEruptionSpell ) );
			Register( 2415, typeof( Herbalist.RestorativeSoilSpell ) );

			// Dynamic Holy Man (mirrors Holy Man spells at new IDs 2500-2513)
			Register( 2500, typeof( HolyMan.BanishEvilSpell ) );
			Register( 2501, typeof( HolyMan.DampenSpiritSpell ) );
			Register( 2502, typeof( HolyMan.EnchantSpell ) );
			Register( 2503, typeof( HolyMan.HammerOfFaithSpell ) );
			Register( 2504, typeof( HolyMan.HeavenlyLightSpell ) );
			Register( 2505, typeof( HolyMan.NourishSpell ) );
			Register( 2506, typeof( HolyMan.PurgeSpell ) );
			Register( 2507, typeof( HolyMan.RebirthSpell ) );
			Register( 2508, typeof( HolyMan.SacredBoonSpell ) );
			Register( 2509, typeof( HolyMan.SanctifySpell ) );
			Register( 2510, typeof( HolyMan.SeanceSpell ) );
			Register( 2511, typeof( HolyMan.SmiteSpell ) );
			Register( 2512, typeof( HolyMan.TouchOfLifeSpell ) );
			Register( 2513, typeof( HolyMan.TrialByFireSpell ) );

			// Dynamic Jedi (mirrors Jedi spells at new IDs 2600-2609)
			Register( 2600, typeof( Jedi.ForceGrip ) );
			Register( 2601, typeof( Jedi.MindsEye ) );
			Register( 2602, typeof( Jedi.Mirage ) );
			Register( 2603, typeof( Jedi.ThrowSabre ) );
			Register( 2604, typeof( Jedi.Celerity ) );
			Register( 2605, typeof( Jedi.PsychicAura ) );
			Register( 2606, typeof( Jedi.Deflection ) );
			Register( 2607, typeof( Jedi.SoothingTouch ) );
			Register( 2608, typeof( Jedi.StasisField ) );
			Register( 2609, typeof( Jedi.Replicate ) );

			// Dynamic Jester (mirrors Jester spells at new IDs 2700-2709)
			Register( 2700, typeof( Jester.CanOfSnakes ) );
			Register( 2701, typeof( Jester.Clowns ) );
			Register( 2702, typeof( Jester.FlowerPower ) );
			Register( 2703, typeof( Jester.Hilarity ) );
			Register( 2704, typeof( Jester.Insult ) );
			Register( 2705, typeof( Jester.JumpAround ) );
			Register( 2706, typeof( Jester.PoppingBalloon ) );
			Register( 2707, typeof( Jester.RabbitInAHat ) );
			Register( 2708, typeof( Jester.SeltzerBottle ) );
			Register( 2709, typeof( Jester.SurpriseGift ) );

			// Dynamic Research (mirrors Research spells at new IDs 2800-2863, ordered by school)
			// Conjuration
			Register( 2800, typeof( Research.ResearchConjure ) );
			Register( 2801, typeof( Research.ResearchExtinguish ) );
			Register( 2802, typeof( Research.ResearchClone ) );
			Register( 2803, typeof( Research.ResearchCreateGold ) );
			Register( 2804, typeof( Research.ResearchSwarm ) );
			Register( 2805, typeof( Research.ResearchMagicSteed ) );
			Register( 2806, typeof( Research.ResearchAerialServant ) );
			Register( 2807, typeof( Research.ResearchDeathVortex ) );
			// Death
			Register( 2808, typeof( Research.ResearchDeathSpeak ) );
			Register( 2809, typeof( Research.ResearchRockFlesh ) );
			Register( 2810, typeof( Research.ResearchGrantPeace ) );
			Register( 2811, typeof( Research.ResearchSummonDead ) );
			Register( 2812, typeof( Research.ResearchMaskofDeath ) );
			Register( 2813, typeof( Research.ResearchCreateGolem ) );
			Register( 2814, typeof( Research.ResearchOpenGround ) );
			Register( 2815, typeof( Research.ResearchWithstandDeath ) );
			// Enchanting
			Register( 2816, typeof( Research.ResearchSneak ) );
			Register( 2817, typeof( Research.ResearchMassMight ) );
			Register( 2818, typeof( Research.ResearchSleep ) );
			Register( 2819, typeof( Research.ResearchCauseFear ) );
			Register( 2820, typeof( Research.ResearchEnchant ) );
			Register( 2821, typeof( Research.ResearchSleepField ) );
			Register( 2822, typeof( Research.ResearchCharm ) );
			Register( 2823, typeof( Research.ResearchMassSleep ) );
			// Sorcery
			Register( 2824, typeof( Research.ResearchCreateFire ) );
			Register( 2825, typeof( Research.ResearchEndureCold ) );
			Register( 2826, typeof( Research.ResearchEndureHeat ) );
			Register( 2827, typeof( Research.ResearchIgnite ) );
			Register( 2828, typeof( Research.ResearchFlameBolt ) );
			Register( 2829, typeof( Research.ResearchConflagration ) );
			Register( 2830, typeof( Research.ResearchExplosion ) );
			Register( 2831, typeof( Research.ResearchRingofFire ) );
			// Summoning
			Register( 2832, typeof( Research.ResearchSummonElectricalElemental ) );
			Register( 2833, typeof( Research.ResearchSummonWeedElemental ) );
			Register( 2834, typeof( Research.ResearchSummonIceElemental ) );
			Register( 2835, typeof( Research.ResearchSummonMudElemental ) );
			Register( 2836, typeof( Research.ResearchSummonBloodElemental ) );
			Register( 2837, typeof( Research.ResearchSummonPoisonElemental ) );
			Register( 2838, typeof( Research.ResearchSummonGemElemental ) );
			Register( 2839, typeof( Research.ResearchSummonAcidElemental ) );
			// Thaumaturgy
			Register( 2840, typeof( Research.ResearchConfusionBlast ) );
			Register( 2841, typeof( Research.ResearchSummonCreature ) );
			Register( 2842, typeof( Research.ResearchEtherealTravel ) );
			Register( 2843, typeof( Research.ResearchBanishDaemon ) );
			Register( 2844, typeof( Research.ResearchCallDestruction ) );
			Register( 2845, typeof( Research.ResearchMeteorShower ) );
			Register( 2846, typeof( Research.ResearchSummonDevil ) );
			Register( 2847, typeof( Research.ResearchDevastation ) );
			// Theurgy
			Register( 2848, typeof( Research.ResearchSeeTruth ) );
			Register( 2849, typeof( Research.ResearchHealingTouch ) );
			Register( 2850, typeof( Research.ResearchWizardEye ) );
			Register( 2851, typeof( Research.ResearchFadefromSight ) );
			Register( 2852, typeof( Research.ResearchDivination ) );
			Register( 2853, typeof( Research.ResearchIntervention ) );
			Register( 2854, typeof( Research.ResearchAirWalk ) );
			Register( 2855, typeof( Research.ResearchRestoration ) );
			// Wizardry
			Register( 2856, typeof( Research.ResearchIcicle ) );
			Register( 2857, typeof( Research.ResearchSnowBall ) );
			Register( 2858, typeof( Research.ResearchFrostField ) );
			Register( 2859, typeof( Research.ResearchGasCloud ) );
			Register( 2860, typeof( Research.ResearchFrostStrike ) );
			Register( 2861, typeof( Research.ResearchHailStorm ) );
			Register( 2862, typeof( Research.ResearchAvalanche ) );
			Register( 2863, typeof( Research.ResearchMassDeath ) );

			// Dynamic Shinobi (mirrors Shinobi spells at new IDs 2900-2907)
			Register( 2900, typeof( Shinobi.CheetahPaws ) );
			Register( 2901, typeof( Shinobi.Deception ) );
			Register( 2902, typeof( Shinobi.EagleEye ) );
			Register( 2903, typeof( Shinobi.Espionage ) );
			Register( 2904, typeof( Shinobi.FerretFlee ) );
			Register( 2905, typeof( Shinobi.MonkeyLeap ) );
			Register( 2906, typeof( Shinobi.MysticShuriken ) );
			Register( 2907, typeof( Shinobi.TigerStrength ) );

			// Dynamic Syth (mirrors Syth spells at new IDs 3000-3009)
			Register( 3000, typeof( Syth.Psychokinesis ) );
			Register( 3001, typeof( Syth.DeathGrip ) );
			Register( 3002, typeof( Syth.Projection ) );
			Register( 3003, typeof( Syth.ThrowSword ) );
			Register( 3004, typeof( Syth.SythSpeed ) );
			Register( 3005, typeof( Syth.SythLightning ) );
			Register( 3006, typeof( Syth.Absorption ) );
			Register( 3007, typeof( Syth.PsychicBlast ) );
			Register( 3008, typeof( Syth.DrainLife ) );
			Register( 3009, typeof( Syth.CloneBody ) );

			// Dynamic Witch (mirrors Undead/Witch spells at new IDs 3100-3115)
			Register( 3100, typeof( Undead.SpectreShadowSpell ) );
			Register( 3101, typeof( Undead.ManaLeechSpell ) );
			Register( 3102, typeof( Undead.UndeadCurePoisonSpell ) );
			Register( 3103, typeof( Undead.HellsBrandSpell ) );
			Register( 3104, typeof( Undead.UndeadGraveyardGatewaySpell ) );
			Register( 3105, typeof( Undead.RetchedAirSpell ) );
			Register( 3106, typeof( Undead.UndeadEyesSpell ) );
			Register( 3107, typeof( Undead.UndeadWallOfSpikesSpell ) );
			Register( 3108, typeof( Undead.VampireGiftSpell ) );
			Register( 3109, typeof( Undead.UnBloodlessSpecPactSpell ) );
			Register( 3110, typeof( Undead.NecroPoisonSpell ) );
			Register( 3111, typeof( Undead.HellsGateSpell ) );
			Register( 3112, typeof( Undead.GhostlyImagesSpell ) );
			Register( 3113, typeof( Undead.GhostPhaseSpell ) );
			Register( 3114, typeof( Undead.NecroUnlockSpell ) );
			Register( 3115, typeof( Undead.PhantasmSpell ) );
		}

		public static void Register( int spellID, Type type )
		{
			SpellRegistry.Register( spellID, type );
		}
	}
}