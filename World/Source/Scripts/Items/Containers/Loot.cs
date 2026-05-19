using System;
using System.IO;
using System.Reflection;
using Server;
using Server.Items;
using Server.Misc;
using Server.Engines.Mahjong;
using System.Collections.Generic;
using System.Collections;

namespace Server
{
	public class Loot
	{
		#region List definitions
		private static Type[] m_OrientWeaponTypes = new Type[]
			{
				typeof( Bokuto ),				typeof( Daisho ),				typeof( Kama ),
				typeof( Lajatang ),				typeof( NoDachi ),				typeof( Nunchaku ),
				typeof( Sai ),					typeof( Tekagi ),				typeof( Tessen ),
				typeof( Tetsubo ),				typeof( Wakizashi ), 			typeof( PugilistGloves ),
				typeof( RepeatingCrossbow ),	typeof( Katana ),
				typeof( QuarterStaff ),			typeof( Pike ),					typeof( BladedStaff ),
				typeof( Spear ),				typeof( Axe ),					typeof( ElvenMachete ),
				typeof( Scimitar ),				typeof( Leafblade ),			typeof( Longsword ),
				typeof( Dagger ),				typeof( WarMace )
			};

		public static Type[] OrientWeaponTypes{ get{ return m_OrientWeaponTypes; } }

		private static Type[] m_WeaponTypes = new Type[]
			{
				typeof( AssassinSpike ),	typeof( DoubleBladedStaff ),	typeof( Longsword ),		typeof( ShortSpear ),
				typeof( Axe ),				typeof( ElvenMachete ),			typeof( Mace ),				typeof( ShortSword ),
				typeof( Bardiche ),			typeof( ElvenSpellblade ),		typeof( Maul ),
				typeof( BattleAxe ),		typeof( ExecutionersAxe ),		typeof( OrnateAxe ),		typeof( Spear ),
				typeof( BlackStaff ),		typeof( GnarledStaff ),			typeof( Pike ),				typeof( SpikedClub ),
				typeof( BladedStaff ),		typeof( Halberd ),				typeof( Pitchfork ),		typeof( TwoHandedAxe ),
				typeof( BoneHarvester ),	typeof( HammerPick ),			typeof( Pitchforks ),		typeof( VikingSword ),
				typeof( Broadsword ),		typeof( Hammers ),				typeof( PugilistGlove ),	typeof( WarAxe ),
				typeof( ButcherKnife ),		typeof( Hammers ),				typeof( PugilistGloves ),	typeof( WarCleaver ),
				typeof( Claymore ),			typeof( Harpoon ),				typeof( QuarterStaff ),		typeof( WarFork ),
				typeof( Cleaver ),			typeof( Hatchet ),				typeof( RadiantScimitar ),	typeof( WarHammer ),
				typeof( Club ),				typeof( Katana ),				typeof( RoyalSword ),		typeof( WarMace ),
				typeof( CrescentBlade ),	typeof( Kryss ),				typeof( RuneBlade ),		typeof( Whips ),
				typeof( Cutlass ),			typeof( Lance ),				typeof( Scepter ),			typeof( WildStaff ),
				typeof( Dagger ),			typeof( LargeBattleAxe ),		typeof( Scimitar ),			typeof( WizardStaff ),
				typeof( DiamondMace ),		typeof( LargeKnife ),			typeof( Scythe ),			typeof( WizardStick ),
				typeof( DoubleAxe ),		typeof( Leafblade ),			typeof( ShepherdsCrook ),	typeof( WizardWand )
			};

		public static Type[] WeaponTypes{ get{ return m_WeaponTypes; } }

		private static Type[] m_SciFiWeaponTypes = new Type[]
			{
				typeof( LightSword ),		typeof( DoubleLaserSword )
			};

		public static Type[] SciFiWeaponTypes{ get{ return m_SciFiWeaponTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_OrientRangedWeaponTypes = new Type[]
			{
				typeof( Yumi ),					typeof( Yumi ),					typeof( Yumi ),
				typeof( Crossbow ),				typeof( ElvenCompositeLongbow ),
				typeof( ThrowingGloves ),		typeof( Shuriken )
			};

		public static Type[] OrientRangedWeaponTypes{ get{ return m_OrientRangedWeaponTypes; } }

		private static Type[] m_RangedWeaponTypes = new Type[]
			{
				typeof( Bow ),						typeof( Crossbow ),				typeof( HeavyCrossbow ),
				typeof( ThrowingGloves ),			typeof( CompositeBow ),			typeof( RepeatingCrossbow ),
				typeof( ElvenCompositeLongbow ),	typeof( MagicalShortbow )	
			};

		public static Type[] RangedWeaponTypes{ get{ return m_RangedWeaponTypes; } }

		private static Type[] m_SciFiGunTypes = new Type[]
			{
				typeof( KilrathiHeavyGun ),		typeof( KilrathiGun )
			};

		public static Type[] SciFiGunTypes{ get{ return m_SciFiGunTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_OrientArmorTypes = new Type[]
			{
				typeof( ChainHatsuburi ),		typeof( LeatherDo ),				typeof( LeatherHaidate ),
				typeof( LeatherHiroSode ),		typeof( LeatherJingasa ),			typeof( LeatherMempo ),
				typeof( LeatherNinjaHood ),		typeof( LeatherNinjaJacket ),		typeof( LeatherNinjaMitts ),
				typeof( LeatherNinjaPants ),	typeof( LeatherSuneate ),			typeof( DecorativePlateKabuto ),
				typeof( HeavyPlateJingasa ),	typeof( LightPlateJingasa ),		typeof( PlateBattleKabuto ),
				typeof( PlateDo ),				typeof( PlateHaidate ),				typeof( PlateHatsuburi ),
				typeof( PlateHiroSode ),		typeof( PlateMempo ),				typeof( PlateSuneate ),
				typeof( SmallPlateJingasa ),	typeof( StandardPlateKabuto ),		typeof( StuddedDo ),
				typeof( StuddedHaidate ),		typeof( StuddedHiroSode ),			typeof( StuddedMempo ),
				typeof( StuddedSuneate ),		typeof( OniwabanHood ),				typeof( OniwabanLeggings ),
				typeof( ShinobiCowl ),			typeof( ShinobiHood ),				typeof( ShinobiMask ),
				typeof( ShinobiRobe ),			typeof( OniwabanTunic ),			typeof( OniwabanBoots ),
				typeof( OniwabanGloves ),		typeof( ScalyBoots ),				typeof( DrakboneHelm )
			};

		public static Type[] OrientArmorTypes{ get{ return m_OrientArmorTypes; } }

		private static Type[] m_ArmorTypes = new Type[]
			{
				typeof( Bascinet ),				typeof( LeatherSandals ),			typeof( RingmailArms ),			
				typeof( BoneArms ),				typeof( LeatherShoes ),				typeof( RingmailChest ),			
				typeof( BoneChest ),			typeof( LeatherShorts ),			typeof( RingmailGloves ),			
				typeof( BoneGloves ),			typeof( LeatherSkirt ),				typeof( RingmailLegs ),			
				typeof( BoneHelm ),				typeof( LeatherSoftBoots ),			typeof( RoyalArms ),			
				typeof( BoneLegs ),				typeof( LeatherThighBoots ),		typeof( RoyalBoots ),			
				typeof( ChainChest ),			typeof( RoyalChest ),				typeof( WolfCap ),
				typeof( ChainCoif ),			typeof( DreadHelm ),				typeof( DeerCap ),
				typeof( ChainLegs ),			typeof( RoyalGloves ),				typeof( BearCap ),
				typeof( CloseHelm ),			typeof( RoyalGorget ),				typeof( StagCap ),
				typeof( FemaleLeatherChest ),	typeof( RoyalHelm ),			
				typeof( FemalePlateChest ),		typeof( RoyalsLegs ),				typeof( PlateSkirt ),
				typeof( FemaleStuddedChest ),	typeof( StuddedArms ),				typeof( ChainSkirt ),
				typeof( Helmet ),				typeof( StuddedBustierArms ),		typeof( RingmailSkirt ),
				typeof( LeatherArms ),			typeof( StuddedChest ),				typeof( StuddedSkirt ),
				typeof( LeatherBoots ),			typeof( StuddedGloves ),
				typeof( LeatherBustierArms ),	typeof( NorseHelm ),				typeof( StuddedGorget ),			
				typeof( LeatherCap ),			typeof( OrcHelm ),					typeof( StuddedLegs ),			
				typeof( LeatherChest ),			typeof( PlateArms ),				typeof( WoodenPlateArms ),			
				typeof( LeatherCloak ),			typeof( PlateChest ),				typeof( WoodenPlateChest ),			
				typeof( LeatherGloves ),		typeof( PlateGloves ),				typeof( WoodenPlateGloves ),			
				typeof( LeatherGorget ),		typeof( PlateGorget ),				typeof( WoodenPlateGorget ),			
				typeof( LeatherLegs ),			typeof( PlateHelm ),				typeof( WoodenPlateHelm ),			
				typeof( LeatherRobe ),			typeof( PlateLegs ),				typeof( WoodenPlateLegs ),
				typeof( BoneSkirt ),			typeof( HideChest ),
				typeof( SavageArms ), 			typeof( SavageChest ), 				typeof( SavageGloves ),
				typeof( SavageHelm ), 			typeof( SavageLegs ),				typeof( StuddedHideChest ),
				typeof( DragonChest ),			typeof( DragonGloves ),				typeof( DragonLegs ),
				typeof( ScaledLegs ),			typeof( ScaledArms ),				typeof( ScaledChest ),
				typeof( ScaledGloves ),			typeof( ScaledGorget ),				typeof( ScaledHelm ),
				typeof( ScalyArms ),			typeof( ScalyLegs ),				typeof( ScalyHelm ),
				typeof( ScalyGorget ),			typeof( ScalyGloves ),				typeof( ScalyChest )
			};

		public static Type[] ArmorTypes{ get{ return m_ArmorTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_ShieldTypes = new Type[]
			{
				typeof( BronzeShield ),			typeof( Buckler ),				typeof( HeaterShield ),
				typeof( MetalShield ),			typeof( MetalKiteShield ),		typeof( WoodenKiteShield ),
				typeof( WoodenShield ),			typeof( SunShield ),			typeof( VirtueShield ),
				typeof( ChaosShield ),			typeof( OrderShield ),			typeof( RoyalShield ),
				typeof( GuardsmanShield ),		typeof( ElvenShield ),			typeof( DarkShield ),
				typeof( CrestedShield ),		typeof( ChampionShield ),		typeof( JeweledShield ),
				typeof( ScalemailShield ),		typeof( ScaledShield )
			};

		public static Type[] ShieldTypes{ get{ return m_ShieldTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_OrientClothingTypes = new Type[]
			{
				typeof( ClothNinjaJacket ),		typeof( FemaleKimono ),			typeof( Hakama ),
				typeof( HakamaShita ),			typeof( JinBaori ),				typeof( Kamishimo ),
				typeof( MaleKimono ),			typeof( NinjaTabi ),			typeof( Obi ),
				typeof( SamuraiTabi ),			typeof( TattsukeHakama ),		typeof( Waraji ),
				typeof( LeatherNinjaBelt )
			};

		public static Type[] OrientClothingTypes{ get{ return m_OrientClothingTypes; } }

		private static Type[] m_ClothingTypes = new Type[]
			{
				typeof( FullApron ),			typeof( JesterSuit ),			typeof( BarbarianBoots ),	typeof( Cloak ),			typeof( ArchmageRobe ),
				typeof( BodySash ),				typeof( Doublet ),				typeof( Boots ),			typeof( Cloak ),			typeof( AssassinRobe ),
				typeof( LoinCloth ),			typeof( FancyShirt ),			typeof( Sandals ),			typeof( Cloak ),			typeof( ChaosRobe ),
				typeof( HalfApron ),			typeof( RusticVest ),			typeof( Shoes ),			typeof( Cloak ),			typeof( CultistRobe ),
				typeof( Kilt ),					typeof( Tunic ),				typeof( ThighBoots ),		typeof( Cloak ),			typeof( DragonRobe ),
				typeof( Belt ),					typeof( Shirt ),				typeof( BarbarianBoots ),	typeof( Cloak ),			typeof( ElegantRobe ),
				typeof( Skirt ),				typeof( Surcoat ),				typeof( Boots ),			typeof( Cloak ),			typeof( ExquisiteRobe ),
				typeof( LongPants ),			typeof( RoyalCoat ),			typeof( Sandals ),			typeof( Cloak ),			typeof( FancyDress ),
				typeof( ShortPants ),			typeof( RoyalShirt ),			typeof( Shoes ),			typeof( Cloak ),			typeof( FancyRobe ),
				typeof( PiratePants ),			typeof( FormalShirt ),			typeof( ThighBoots ),		typeof( Cloak ),			typeof( FoolsCoat ),
				typeof( SailorPants ),			typeof( RusticShirt ),			typeof( BarbarianBoots ),	typeof( Cloak ),			typeof( FormalRobe ),
				typeof( RoyalLongSkirt ),		typeof( SquireShirt ),			typeof( Boots ),			typeof( Cloak ),			typeof( GildedDarkRobe ),
				typeof( RoyalSkirt ),			typeof( FormalCoat ),			typeof( Sandals ),			typeof( Cloak ),			typeof( GildedDress ),
				typeof( RoyalLoinCloth ),		typeof( WizardShirt ),			typeof( Shoes ),			typeof( Cloak ),			typeof( GildedLightRobe ),
				typeof( ElvenBoots ),			typeof( BeggarVest ),			typeof( ThighBoots ),		typeof( Cloak ),			typeof( GildedRobe ),
				typeof( JesterShoes ),			typeof( RoyalVest ),			typeof( BarbarianBoots ),	typeof( HikingBoots ),			typeof( JesterGarb ),
				typeof( FullApron ),			typeof( JesterSuit ),			typeof( Boots ),			typeof( Cloak ),			typeof( JokerRobe ),
				typeof( BodySash ),				typeof( Doublet ),				typeof( Sandals ),			typeof( Cloak ),			typeof( MagistrateRobe ),
				typeof( LoinCloth ),			typeof( FancyShirt ),			typeof( Shoes ),			typeof( Cloak ),			typeof( NecromancerRobe ),
				typeof( HalfApron ),			typeof( RusticVest ),			typeof( ThighBoots ),		typeof( Cloak ),			typeof( OrnateRobe ),
				typeof( Kilt ),					typeof( Tunic ),				typeof( BarbarianBoots ),	typeof( HikingBoots ),			typeof( PirateCoat ),
				typeof( Belt ),					typeof( Shirt ),				typeof( Boots ),			typeof( Cloak ),			typeof( PlainDress ),
				typeof( Skirt ),				typeof( Surcoat ),				typeof( Sandals ),			typeof( Cloak ),			typeof( PriestRobe ),
				typeof( LongPants ),			typeof( RoyalCoat ),			typeof( Shoes ),			typeof( Cloak ),			typeof( ProphetRobe ),
				typeof( ShortPants ),			typeof( RoyalShirt ),			typeof( ThighBoots ),		typeof( Cloak ),			typeof( Robe ),
				typeof( PiratePants ),			typeof( FormalShirt ),			typeof( BarbarianBoots ),	typeof( HikingBoots ),			typeof( RoyalRobe ),
				typeof( SailorPants ),			typeof( RusticShirt ),			typeof( Boots ),			typeof( RoyalCape ),		typeof( SageRobe ),
				typeof( RoyalLongSkirt ),		typeof( SquireShirt ),			typeof( Sandals ),			typeof( RoyalCape ),		typeof( ScholarRobe ),
				typeof( RoyalSkirt ),			typeof( FormalCoat ),			typeof( Shoes ),			typeof( RoyalCape ),		typeof( SorcererRobe ),
				typeof( RoyalLoinCloth ),		typeof( WizardShirt ),			typeof( ThighBoots ),		typeof( RoyalCape ),		typeof( SpiderRobe ),
				typeof( ElvenBoots ),			typeof( BeggarVest ),			typeof( Boots ),			typeof( RoyalCape ),		typeof( VagabondRobe),
				typeof( JesterShoes ),			typeof( RoyalVest ),			typeof( ThighBoots ),		typeof( RoyalCape ),		typeof( VampireRobe )
			};
		public static Type[] ClothingTypes{ get{ return m_ClothingTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_OrientHatTypes = new Type[]
			{
				typeof( ClothNinjaHood ),		typeof( Kasa ), 				typeof( Bandana )
			};

		public static Type[] OrientHatTypes{ get{ return m_OrientHatTypes; } }

		private static Type[] m_HatTypes = new Type[]
			{
				typeof( SkullCap ),			typeof( Bandana ),		typeof( FloppyHat ),
				typeof( Cap ),				typeof( WideBrimHat ),	typeof( StrawHat ),
				typeof( TallStrawHat ),		typeof( WizardsHat ),	typeof( Bonnet ),
				typeof( WitchHat ),			typeof( ClothCowl ),	typeof( ClothHood ),
				typeof( FeatheredHat ),		typeof( TricorneHat ),	typeof( JesterHat ),
				typeof( PirateHat ),		typeof( JokerHat ),		typeof( FancyHood ),
				typeof( DeadMask ),			typeof( WizardHood ),	typeof( HoodedMantle ),
				typeof( ReaperHood ),		typeof( ReaperCowl )
			};

		public static Type[] HatTypes{ get{ return m_HatTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_OrientSpellbooks = new Type[]
			{
				typeof( BookOfBushido ),		typeof( BookOfNinjitsu ),			typeof( MysticSpellbook )
			};

		public static Type[] OrientSpellbooks{ get{ return m_OrientSpellbooks; } }

		private static Type[] m_Spellbooks = new Type[]
			{
				typeof( SongBook ),				typeof( Spellbook ),				typeof( MysticSpellbook ),
				typeof( BookOfNinjitsu ),		typeof( BookOfBushido ),			typeof( NecromancerSpellbook ),
				typeof( BookOfChivalry ),		typeof( ElementalSpellbook )	
			};

		public static Type[] Spellbooks{ get{ return m_Spellbooks; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_FoodsTypes = new Type[]
			{
				typeof( ChickenLeg ),			typeof( BreadLoaf ),			typeof( Apple ),
				typeof( ChickenLeg ),			typeof( BreadLoaf ),			typeof( Banana ),
				typeof( ChickenLeg ),			typeof( BreadLoaf ),			typeof( Cabbage ),
				typeof( CookedBird ),			typeof( BreadLoaf ),			typeof( Cantaloupe ),
				typeof( CookedBird ),			typeof( CheeseWedge ),			typeof( Carrot ),
				typeof( CookedBird ),			typeof( CheeseWedge ),			typeof( Grapes ),
				typeof( FishSteak ),			typeof( CheeseWedge ),			typeof( GreenGourd ),
				typeof( FishSteak ),			typeof( CheeseWedge ),			typeof( HoneydewMelon ),
				typeof( FishSteak ),			typeof( CheeseWheel ),			typeof( Lemon ),
				typeof( Ham ),					typeof( CheeseWheel ),			typeof( Lettuce ),
				typeof( LambLeg ),				typeof( CheeseWheel ),			typeof( Lime ),
				typeof( LambLeg ),				typeof( CheeseWheel ),			typeof( Onion ),
				typeof( Ribs ),					typeof( FrenchBread ),			typeof( Peach ),
				typeof( Ribs ),					typeof( FrenchBread ),			typeof( Pear ),
				typeof( Ribs ),					typeof( FrenchBread ),			typeof( Pumpkin ),
				typeof( Sausage ),				typeof( FrenchBread ),			typeof( Squash ),
				typeof( Sausage ),				typeof( Muffins ),				typeof( Watermelon ),
				typeof( Sausage ),				typeof( Muffins ),				typeof( YellowGourd )
			};

		public static Type[] FoodsTypes{ get{ return m_FoodsTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_GemTypes = new Type[]
			{
				typeof( Amber ),				typeof( Amethyst ),				typeof( Citrine ),
				typeof( Diamond ),				typeof( Emerald ),				typeof( Ruby ),
				typeof( Sapphire ),				typeof( StarSapphire ),			typeof( Tourmaline )
			};

		public static Type[] GemTypes{ get{ return m_GemTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_JewelryTypes = new Type[]
			{
				typeof( JewelryRing ),			typeof( JewelryNecklace ),		typeof( JewelryEarrings ),	
				typeof( JewelryBracelet ),		typeof( JewelryCirclet ),		typeof( TrinketTalisman ),
				typeof( TrinketCandle ),		typeof( TrinketTorch ),			typeof( TrinketLantern )
			};

		public static Type[] JewelryTypes{ get{ return m_JewelryTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_RegTypes = new Type[]
			{
				typeof( BlackPearl ),			typeof( Bloodmoss ),			typeof( Garlic ),
				typeof( Ginseng ),				typeof( MandrakeRoot ),			typeof( Nightshade ),
				typeof( SulfurousAsh ),			typeof( SpidersSilk )
			};

		public static Type[] RegTypes{ get{ return m_RegTypes; } }

		private static Type[] m_WitchRegTypes = new Type[]
			{
				typeof( BlackSand ),			typeof( BloodRose ),			typeof( DriedToad ),
				typeof( Maggot ),				typeof( MummyWrap ),			typeof( VioletFungus ),
				typeof( WerewolfClaw ),			typeof( Wolfsbane ),			typeof( BitterRoot ),
				typeof( BatWing ),				typeof( DaemonBlood ),			typeof( PigIron ),
				typeof( NoxCrystal ),			typeof( GraveDust ),			typeof( BlackPearl ),
				typeof( Bloodmoss ),			typeof( Brimstone ),			typeof( EyeOfToad ),
				typeof( GargoyleEar ),			typeof( BeetleShell ),			typeof( MoonCrystal ),
				typeof( PixieSkull ),			typeof( RedLotus ),				typeof( SilverWidow ),
				typeof( SwampBerries )
			};

		public static Type[] WitchRegTypes{ get{ return m_WitchRegTypes; } }

		private static Type[] m_DruidRegTypes = new Type[]
			{
				typeof( BlackPearl ),			typeof( Bloodmoss ),			typeof( Garlic ),
				typeof( Ginseng ),				typeof( MandrakeRoot ),			typeof( Nightshade ),
				typeof( SpidersSilk ),			typeof( SulfurousAsh ),			typeof( Brimstone ),
				typeof( ButterflyWings ),		typeof( EyeOfToad ),			typeof( FairyEgg ),
				typeof( BeetleShell ),			typeof( MoonCrystal ),			typeof( RedLotus ),
				typeof( SeaSalt ),				typeof( SilverWidow ),			typeof( SwampBerries )
			};

		public static Type[] DruidRegTypes{ get{ return m_DruidRegTypes; } }

		private static Type[] m_NecroRegTypes = new Type[]
			{
				typeof( BatWing ),				typeof( GraveDust ),			typeof( DaemonBlood ),
				typeof( NoxCrystal ),			typeof( PigIron )
			};

		public static Type[] NecroRegTypes{ get{ return m_NecroRegTypes; } }

		private static Type[] m_MixerRegTypes = new Type[]
			{
				typeof( EyeOfToad ),			typeof( FairyEgg ),				typeof( GargoyleEar ),
				typeof( BeetleShell ),			typeof( MoonCrystal ),			typeof( PixieSkull ),
				typeof( RedLotus ),				typeof( SeaSalt ),				typeof( SilverWidow ),
				typeof( SwampBerries ),			typeof( Brimstone ),			typeof( ButterflyWings )
			};

		public static Type[] MixerRegTypes{ get{ return m_MixerRegTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_LowPotionTypes = new Type[]
			{
				typeof( AgilityPotion ),		typeof( LesserExplosionPotion ),	typeof( LesserManaPotion ),		typeof( LesserRejuvenatePotion ),
				typeof( ConflagrationPotion ),	typeof( FrostbitePotion ),			typeof( NightSightPotion ),		typeof( StrengthPotion ),
				typeof( ConfusionBlastPotion ),	typeof( LesserHealPotion ),			typeof( LesserPoisonPotion ),
				typeof( LesserCurePotion ),		typeof( LesserInvisibilityPotion ),	typeof( RefreshPotion )
			};

		public static Type[] LowPotionTypes{ get{ return m_LowPotionTypes; } }

		private static Type[] m_MedPotionTypes = new Type[]
			{
				typeof( AgilityPotion ),		typeof( FrostbitePotion ),			typeof( PoisonPotion ),
				typeof( ConflagrationPotion ),	typeof( HealPotion ),				typeof( RefreshPotion ),		typeof( GreaterPoisonPotion ),	typeof( SilverSerpentVenom ),
				typeof( ConfusionBlastPotion ),	typeof( InvisibilityPotion ),		typeof( RejuvenatePotion ),		typeof( RepairPotion ),
				typeof( CurePotion ),			typeof( ManaPotion ),				typeof( StrengthPotion ),
				typeof( ExplosionPotion ),		typeof( NightSightPotion ),			typeof( BottleOfAcid )
			};

		public static Type[] MedPotionTypes{ get{ return m_MedPotionTypes; } }

		private static Type[] m_HighPotionTypes = new Type[]
			{
				typeof( GreaterAgilityPotion ),			typeof( GreaterInvisibilityPotion ),	typeof( AutoResPotion ),		typeof( InvulnerabilityPotion ),
				typeof( GreaterConflagrationPotion ),	typeof( GreaterManaPotion ),			typeof( BottleOfAcid ),			typeof( PotionOfDexterity ),
				typeof( GreaterConfusionBlastPotion ),	typeof( NightSightPotion ),				typeof( DurabilityPotion ),		typeof( PotionOfMight ),
				typeof( GreaterCurePotion ),			typeof( GenderPotion ),					typeof( DeadlyPoisonPotion ),	typeof( PotionOfWisdom ),
				typeof( GreaterExplosionPotion ),		typeof( TotalRefreshPotion ),			typeof( LethalPoisonPotion ),	typeof( ResurrectPotion ),
				typeof( GreaterFrostbitePotion ),		typeof( GreaterRejuvenatePotion ),		typeof( GoldenSerpentVenom ),	typeof( SuperPotion ),
				typeof( GreaterHealPotion ),			typeof( GreaterStrengthPotion ),		typeof( TransmutationPotion ),	typeof( RepairPotion )
			};

		public static Type[] HighPotionTypes{ get{ return m_HighPotionTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_LowScrollTypes = new Type[]
			{
				typeof( ClumsyScroll ),			typeof( CurseWeaponScroll ),	typeof( ArmysPaeonScroll ),			typeof( Elemental_Armor_Scroll ),
				typeof( CreateFoodScroll ),		typeof( BloodOathScroll ),		typeof( MagesBalladScroll ),		typeof( Elemental_Bolt_Scroll ),
				typeof( FeeblemindScroll ),		typeof( CorpseSkinScroll ),		typeof( EnchantingEtudeScroll ),	typeof( Elemental_Mend_Scroll ),
				typeof( HealScroll ),			typeof( EvilOmenScroll ),		typeof( SheepfoeMamboScroll ),		typeof( Elemental_Sanctuary_Scroll ),
				typeof( MagicArrowScroll ),		typeof( PainSpikeScroll ),		typeof( SinewyEtudeScroll ),		typeof( Elemental_Pain_Scroll ),
				typeof( NightSightScroll ),		typeof( WraithFormScroll ),		typeof( FireThrenodyScroll ),		typeof( Elemental_Protection_Scroll ),
				typeof( ReactiveArmorScroll ),	typeof( MindRotScroll ),		typeof( IceThrenodyScroll ),		typeof( Elemental_Purge_Scroll ),
				typeof( WeakenScroll ),			typeof( SummonFamiliarScroll ),	typeof( PoisonThrenodyScroll ),		typeof( Elemental_Steed_Scroll ),
				typeof( AgilityScroll ),		typeof( CurseWeaponScroll ),	typeof( ArmysPaeonScroll ),			typeof( Elemental_Call_Scroll ),
				typeof( CunningScroll ),		typeof( BloodOathScroll ),		typeof( MagesBalladScroll ),		typeof( Elemental_Force_Scroll ),
				typeof( CureScroll ),			typeof( CorpseSkinScroll ),		typeof( EnchantingEtudeScroll ),	typeof( Elemental_Wall_Scroll ),
				typeof( HarmScroll ),			typeof( EvilOmenScroll ),		typeof( SheepfoeMamboScroll ),		typeof( Elemental_Warp_Scroll ),
				typeof( MagicTrapScroll ),		typeof( PainSpikeScroll ),		typeof( SinewyEtudeScroll ),		typeof( Elemental_Armor_Scroll ),
				typeof( MagicUnTrapScroll ),	typeof( WraithFormScroll ),		typeof( FireThrenodyScroll ),		typeof( Elemental_Bolt_Scroll ),
				typeof( ProtectionScroll ),		typeof( MindRotScroll ),		typeof( IceThrenodyScroll ),		typeof( Elemental_Mend_Scroll ),
				typeof( StrengthScroll ),		typeof( SummonFamiliarScroll ),	typeof( PoisonThrenodyScroll ),		typeof( Elemental_Sanctuary_Scroll ),
				typeof( BlessScroll ),			typeof( CurseWeaponScroll ),	typeof( ArmysPaeonScroll ),			typeof( Elemental_Pain_Scroll ),
				typeof( FireballScroll ),		typeof( BloodOathScroll ),		typeof( MagesBalladScroll ),		typeof( Elemental_Protection_Scroll ),
				typeof( MagicLockScroll ),		typeof( CorpseSkinScroll ),		typeof( EnchantingEtudeScroll ),	typeof( Elemental_Purge_Scroll ),
				typeof( PoisonScroll ),			typeof( EvilOmenScroll ),		typeof( SheepfoeMamboScroll ),		typeof( Elemental_Steed_Scroll ),
				typeof( TelekinisisScroll ),	typeof( PainSpikeScroll ),		typeof( SinewyEtudeScroll ),		typeof( Elemental_Call_Scroll ),
				typeof( TeleportScroll ),		typeof( WraithFormScroll ),		typeof( FireThrenodyScroll ),		typeof( Elemental_Force_Scroll ),
				typeof( UnlockScroll ),			typeof( MindRotScroll ),		typeof( IceThrenodyScroll ),		typeof( Elemental_Wall_Scroll ),
				typeof( WallOfStoneScroll ),	typeof( SummonFamiliarScroll ),	typeof( PoisonThrenodyScroll ),		typeof( Elemental_Warp_Scroll )
			};

		public static Type[] LowScrollTypes{ get{ return m_LowScrollTypes; } }

		private static Type[] m_MedScrollTypes = new Type[]
			{
				typeof( ArchCureScroll ),		typeof( AnimateDeadScroll ),	typeof( MagicFinaleScroll ),		typeof( Elemental_Field_Scroll ),
				typeof( ArchProtectionScroll ),	typeof( HorrificBeastScroll ),	typeof( EnergyCarolScroll ),		typeof( Elemental_Restoration_Scroll ),
				typeof( CurseScroll ),			typeof( PoisonStrikeScroll ),	typeof( EnergyThrenodyScroll ),		typeof( Elemental_Strike_Scroll ),
				typeof( FireFieldScroll ),		typeof( WitherScroll ),			typeof( FireCarolScroll ),			typeof( Elemental_Void_Scroll ),
				typeof( GreaterHealScroll ),	typeof( StrangleScroll ),		typeof( IceCarolScroll ),			typeof( Elemental_Blast_Scroll ),
				typeof( LightningScroll ),		typeof( AnimateDeadScroll ),	typeof( KnightsMinneScroll ),		typeof( Elemental_Echo_Scroll ),
				typeof( ManaDrainScroll ),		typeof( HorrificBeastScroll ),	typeof( PoisonCarolScroll ),		typeof( Elemental_Fiend_Scroll ),
				typeof( RecallScroll ),			typeof( PoisonStrikeScroll ),	typeof( MagicFinaleScroll ),		typeof( Elemental_Hold_Scroll ),
				typeof( BladeSpiritsScroll ),	typeof( WitherScroll ),			typeof( EnergyCarolScroll ),		typeof( Elemental_Barrage_Scroll ),
				typeof( DispelFieldScroll ),	typeof( StrangleScroll ),		typeof( EnergyThrenodyScroll ),		typeof( Elemental_Rune_Scroll ),
				typeof( IncognitoScroll ),		typeof( AnimateDeadScroll ),	typeof( FireCarolScroll ),			typeof( Elemental_Storm_Scroll ),
				typeof( MagicReflectScroll ),	typeof( HorrificBeastScroll ),	typeof( IceCarolScroll ),			typeof( Elemental_Summon_Scroll ),
				typeof( MindBlastScroll ),		typeof( PoisonStrikeScroll ),	typeof( KnightsMinneScroll ),		typeof( Elemental_Field_Scroll ),
				typeof( ParalyzeScroll ),		typeof( WitherScroll ),			typeof( PoisonCarolScroll ),		typeof( Elemental_Restoration_Scroll ),
				typeof( PoisonFieldScroll ),	typeof( StrangleScroll ),		typeof( MagicFinaleScroll ),		typeof( Elemental_Strike_Scroll ),
				typeof( SummonCreatureScroll ),	typeof( AnimateDeadScroll ),	typeof( EnergyCarolScroll ),		typeof( Elemental_Void_Scroll ),
				typeof( DispelScroll ),			typeof( HorrificBeastScroll ),	typeof( EnergyThrenodyScroll ),		typeof( Elemental_Blast_Scroll ),
				typeof( EnergyBoltScroll ),		typeof( PoisonStrikeScroll ),	typeof( FireCarolScroll ),			typeof( Elemental_Echo_Scroll ),
				typeof( ExplosionScroll ),		typeof( WitherScroll ),			typeof( IceCarolScroll ),			typeof( Elemental_Fiend_Scroll ),
				typeof( InvisibilityScroll ),	typeof( StrangleScroll ),		typeof( KnightsMinneScroll ),		typeof( Elemental_Hold_Scroll ),
				typeof( MarkScroll ),											typeof( PoisonCarolScroll ),		typeof( Elemental_Barrage_Scroll ),
				typeof( MassCurseScroll ),																			typeof( Elemental_Rune_Scroll ),
				typeof( ParalyzeFieldScroll ),																		typeof( Elemental_Storm_Scroll ),
				typeof( RevealScroll ),																				typeof( Elemental_Summon_Scroll )
			};

		public static Type[] MedScrollTypes{ get{ return m_MedScrollTypes; } }

		private static Type[] m_HighScrollTypes = new Type[]
			{
				typeof( ChainLightningScroll ),			typeof( LichFormScroll ),			typeof( MagicFinaleScroll ),	typeof( Elemental_Devastation_Scroll ),
				typeof( EnergyFieldScroll ),			typeof( ExorcismScroll ),			typeof( EnergyCarolScroll ),	typeof( Elemental_Fall_Scroll ),
				typeof( FlamestrikeScroll ),			typeof( VengefulSpiritScroll ),		typeof( EnergyThrenodyScroll ),	typeof( Elemental_Gate_Scroll ),
				typeof( GateTravelScroll ),				typeof( VampiricEmbraceScroll ),	typeof( FireCarolScroll ),		typeof( Elemental_Havoc_Scroll ),
				typeof( ManaVampireScroll ),			typeof( LichFormScroll ),			typeof( IceCarolScroll ),		typeof( Elemental_Apocalypse_Scroll ),
				typeof( MassDispelScroll ),				typeof( ExorcismScroll ),			typeof( KnightsMinneScroll ),	typeof( Elemental_Lord_Scroll ),
				typeof( MeteorSwarmScroll ),			typeof( VengefulSpiritScroll ),		typeof( PoisonCarolScroll ),	typeof( Elemental_Soul_Scroll ),
				typeof( PolymorphScroll ),				typeof( VampiricEmbraceScroll ),	typeof( FoeRequiemScroll ),		typeof( Elemental_Spirit_Scroll ),
				typeof( EarthquakeScroll ),				typeof( LichFormScroll ),			typeof( MagicFinaleScroll ),	typeof( Elemental_Devastation_Scroll ),
				typeof( EnergyVortexScroll ),			typeof( ExorcismScroll ),			typeof( EnergyCarolScroll ),	typeof( Elemental_Fall_Scroll ),
				typeof( ResurrectionScroll ),			typeof( VengefulSpiritScroll ),		typeof( EnergyThrenodyScroll ),	typeof( Elemental_Gate_Scroll ),
				typeof( SummonAirElementalScroll ),		typeof( VampiricEmbraceScroll ),	typeof( FireCarolScroll ),		typeof( Elemental_Havoc_Scroll ),
				typeof( SummonDaemonScroll ),			typeof( LichFormScroll ),			typeof( IceCarolScroll ),		typeof( Elemental_Apocalypse_Scroll ),
				typeof( SummonEarthElementalScroll ),	typeof( ExorcismScroll ),			typeof( KnightsMinneScroll ),	typeof( Elemental_Lord_Scroll ),
				typeof( SummonFireElementalScroll ),	typeof( VengefulSpiritScroll ),		typeof( PoisonCarolScroll ),	typeof( Elemental_Soul_Scroll ),
				typeof( SummonWaterElementalScroll ),	typeof( VampiricEmbraceScroll ),	typeof( FoeRequiemScroll ),		typeof( Elemental_Spirit_Scroll )
			};

		public static Type[] HighScrollTypes{ get{ return m_HighScrollTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_InstrumentTypes = new Type[]
			{
				typeof( Drums ),				typeof( LapHarp ),
				typeof( Lute ),					typeof( TambourineTassel ),
				typeof( BambooFlute ),			typeof( Trumpet ),
				typeof( Tambourine )
			};

		public static Type[] InstrumentTypes{ get{ return m_InstrumentTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_QuiverTypes = new Type[]
		{
			typeof( MagicQuiver )
		};

		public static Type[] QuiverTypes{ get{ return m_QuiverTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_StatueTypes = new Type[]
		{
			typeof( StatueSouth ),			typeof( StatueSouth2 ),			typeof( StatueNorth ),
			typeof( StatueWest ),			typeof( StatueEast ),			typeof( StatueEast2 ),
			typeof( StatueSouthEast ),		typeof( BustSouth ),			typeof( BustEast )
		};

		public static Type[] StatueTypes{ get{ return m_StatueTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_ArtyTypes = new Type[]
			{
				typeof( Artifact_AbysmalGloves ), 	typeof( Artifact_ArmsOfTheHarrower ), 	typeof( Artifact_CandleEnergy ), 	typeof( Artifact_DivineLeggings ), 	typeof( Artifact_GeishasObi ), 	typeof( Artifact_HolyKnightsArmPlates ), 	typeof( Artifact_JesterHatofChuckles ), 	typeof( Artifact_MidnightBracers ), 	typeof( Artifact_RamusNecromanticScalpel ), 	typeof( Artifact_ShadowDancerTunic ), 	typeof( Artifact_TotemGloves ), 	typeof( Artifact_QuiverOfRage ),
				typeof( Artifact_AchillesShield ), 	typeof( Artifact_ArmsOfToxicity ), 	typeof( Artifact_CandleFire ), 	typeof( Artifact_DivineTunic ), 	typeof( Artifact_GiantBlackjack ), 	typeof( Artifact_HolyKnightsBreastplate ), 	typeof( Artifact_JinBaoriOfGoodFortune ), 	typeof( Artifact_MidnightGloves ), 	typeof( Artifact_ResilientBracer ), 	typeof( Artifact_ShaMontorrossbow ), 	typeof( Artifact_TotemGorget ), 	typeof( Artifact_RobeofStratos ),
				typeof( Artifact_AchillesSpear ), 	typeof( Artifact_AuraOfShadows ), 	typeof( Artifact_CandleNecromancer ), 	typeof( Artifact_DjinnisRing ), 	typeof( Artifact_GladiatorsCollar ), 	typeof( Artifact_HolyKnightsGloves ), 	typeof( Artifact_KamiNarisIndestructableDoubleAxe ), 	typeof( Artifact_MidnightHelm ), 	typeof( Artifact_Retort ), 	typeof( Artifact_ShardThrasher ), 	typeof( Artifact_TotemLeggings ), 	typeof( Artifact_BootsofHydros ),
				typeof( Artifact_AcidProofRobe ), 	typeof( Artifact_AxeOfTheHeavens ), 	typeof( Artifact_CandlePoison ), 	typeof( Artifact_DreadPirateHat ), 	typeof( Artifact_GlassSword ), 	typeof( Artifact_HolyKnightsGorget ), 	typeof( Artifact_KodiakBearMask ), 	typeof( Artifact_MidnightLegs ), 	typeof( Artifact_RighteousAnger ), 	typeof( Artifact_ShieldOfInvulnerability ), 	typeof( Artifact_TotemOfVoid ), 	typeof( Artifact_BootsofLithos ),
				typeof( Artifact_Aegis ), 	typeof( Artifact_AxeoftheMinotaur ), 	typeof( Artifact_CandleWizard ), 	typeof( Artifact_DupresCollar ), 	typeof( Artifact_GlovesOfAegis ), 	typeof( Artifact_HolyKnightsLegging ), 	typeof( Artifact_LegacyOfTheDreadLord ), 	typeof( Artifact_MidnightTunic ), 	typeof( Artifact_RingOfHealth ), 	typeof( Artifact_ShimmeringTalisman ), 	typeof( Artifact_TotemTunic ), 	typeof( Artifact_BootsofPyros ),
				typeof( Artifact_AegisOfGrace ), 	typeof( Artifact_BeggarsRobe ), 	typeof( Artifact_CapOfFortune ), 	typeof( Artifact_DupresShield ), 	typeof( Artifact_GlovesOfCorruption ), 	typeof( Artifact_HolyKnightsPlateHelm ), 	typeof( Artifact_LeggingsOfAegis ), 	typeof( Artifact_MinersPickaxe ), 	typeof( Artifact_RingOfProtection ), 	typeof( Artifact_ShroudOfDeciet ), 	typeof( Artifact_TownGuardsHalberd ), 	typeof( Artifact_BootsofStratos ),
				typeof( Artifact_AilricsLongbow ), 	typeof( Artifact_BelmontWhip ), 	typeof( Artifact_CapOfTheFallenKing ), 	typeof( Artifact_EarringsOfHealth ), 	typeof( Artifact_GlovesOfDexterity ), 	typeof( Artifact_HolySword ), 	typeof( Artifact_LeggingsOfBane ), 	typeof( Artifact_NightsKiss ), 	typeof( Artifact_RingOfTheElements ), 	typeof( Artifact_SilvanisFeywoodBow ), 	typeof( Artifact_TunicOfAegis ), 	typeof( Artifact_MantleofHydros ),
				typeof( Artifact_AlchemistsBauble ), 	typeof( Artifact_BeltofHercules ), 	typeof( Artifact_CaptainJohnsHat ), 	typeof( Artifact_EarringsOfTheElements ), 	typeof( Artifact_GlovesOfFortune ), 	typeof( Artifact_HoodedShroudOfShadows ), 	typeof( Artifact_LeggingsOfDeceit ), 	typeof( Artifact_NordicVikingSword ), 	typeof( Artifact_RingOfTheMagician ), 	typeof( Artifact_SinbadsSword ), 	typeof( Artifact_TunicOfBane ), 	typeof( Artifact_MantleofLithos ),
				typeof( Artifact_ANecromancerShroud ), 	typeof( Artifact_BladeDance ), 	typeof( Artifact_CaptainQuacklebushsCutlass ), 	typeof( Artifact_EarringsOfTheMagician ), 	typeof( Artifact_GlovesOfInsight ), 	typeof( Artifact_HornOfKingTriton ), 	typeof( Artifact_LeggingsOfEmbers ), 	typeof( Artifact_NoxBow ), 	typeof( Artifact_RingOfTheVile ), 	typeof( Artifact_SongWovenMantle ), 	typeof( Artifact_TunicOfFire ), 	typeof( Artifact_MantleofPyros ),
				typeof( Artifact_AngelicEmbrace ), 	typeof( Artifact_BladeOfInsanity ), 	typeof( Artifact_CavortingClub ), 	typeof( Artifact_EarringsOfTheVile ), 	typeof( Artifact_GlovesOfRegeneration ), 	typeof( Artifact_HuntersArms ), 	typeof( Artifact_LeggingsOfEnlightenment ), 	typeof( Artifact_NoxNightlight ), 	typeof( Artifact_RobeOfTeleportation ), 	typeof( Artifact_SoulSeeker ), 	typeof( Artifact_TunicOfTheFallenKing ), 	typeof( Artifact_MantleofStratos ),
				typeof( Artifact_AngeroftheGods ), 	typeof( Artifact_BladeOfTheRighteous ), 	typeof( Artifact_CircletOfTheSorceress ), 	typeof( Artifact_EmbroideredOakLeafCloak ), 	typeof( Artifact_GlovesOfTheFallenKing ), 	typeof( Artifact_HuntersGloves ), 	typeof( Artifact_LeggingsOfFire ), 	typeof( Artifact_NoxRangersHeavyCrossbow ), 	typeof( Artifact_RobeOfTheEclipse ), 	typeof( Artifact_SpellWovenBritches ), 	typeof( Artifact_TunicOfTheHarrower ), 	typeof( Artifact_RobeofHydros ),
				typeof( Artifact_Annihilation ), 	typeof( Artifact_BlazeOfDeath ), 	typeof( Artifact_CoifOfBane ), 	typeof( Artifact_EnchantedTitanLegBone ), 	typeof( Artifact_GlovesOfTheHarrower ), 	typeof( Artifact_HuntersGorget ), 	typeof( Artifact_LegsOfFortune ), 	typeof( Artifact_OblivionsNeedle ), 	typeof( Artifact_RobeOfTheEquinox ), 	typeof( Artifact_SpiritOfTheTotem ), 	typeof( Artifact_VampiresRobe ), 	typeof( Artifact_RobeofLithos ),
				typeof( Artifact_ArcaneArms ), 	typeof( Artifact_BlightGrippedLongbow ), 	typeof( Artifact_CoifOfFire ), 	typeof( Artifact_EssenceOfBattle ), 	typeof( Artifact_GlovesOfThePugilist ), 	typeof( Artifact_HuntersHeaddress ), 	typeof( Artifact_LegsOfInsight ), 	typeof( Artifact_OrcChieftainHelm ), 	typeof( Artifact_RobeOfTreason ), 	typeof( Artifact_SprintersSandals ), 	typeof( Artifact_VampiricDaisho ), 	typeof( Artifact_RobeofPyros ),
				typeof( Artifact_ArcaneCap ), 	typeof( Artifact_BloodwoodSpirit ), 	typeof( Artifact_ColdBlood ), 	typeof( Artifact_EternalFlame ), 	typeof( Artifact_GorgetOfAegis ), 	typeof( Artifact_HuntersLeggings ), 	typeof( Artifact_LegsOfNobility ), 	typeof( Artifact_OrcishVisage ), 	typeof( Artifact_RobinHoodsBow ), 	typeof( Artifact_StaffOfPower ), 	typeof( Artifact_VioletCourage ), 	typeof( Artifact_PyrosGrimoire ),
				typeof( Artifact_ArcaneGloves ), 	typeof( Artifact_BoneCrusher ), 	typeof( Artifact_ColdForgedBlade ), 	typeof( Artifact_EvilMageGloves ), 	typeof( Artifact_GorgetOfFortune ), 	typeof( Artifact_HuntersTunic ), 	typeof( Artifact_LegsOfTheFallenKing ), 	typeof( Artifact_OrnamentOfTheMagician ), 	typeof( Artifact_RobinHoodsFeatheredHat ), 	typeof( Artifact_StaffofSnakes ), 	typeof( Artifact_VoiceOfTheFallenKing ), 	typeof( Artifact_StratosManual ),
				typeof( Artifact_ArcaneGorget ), 	typeof( Artifact_Bonesmasher ), 	typeof( Artifact_ConansHelm ), 	typeof( Artifact_Excalibur ), 	typeof( Artifact_GorgetOfInsight ), 	typeof( Artifact_Indecency ), 	typeof( Artifact_LegsOfTheHarrower ), 	typeof( Artifact_OrnateCrownOfTheHarrower ), 	typeof( Artifact_RodOfResurrection ), 	typeof( Artifact_StaffOfTheMagi ), 	typeof( Artifact_WarriorsClasp ), 	typeof( Artifact_HydrosLexicon ),
				typeof( Artifact_ArcaneLeggings ), 	typeof( Artifact_BookOfKnowledge ), 	typeof( Artifact_ConansLoinCloth ), 	typeof( Artifact_FalseGodsScepter ), 	typeof( Artifact_GrayMouserCloak ), 	typeof( Artifact_InquisitorsArms ), 	typeof( Artifact_LieutenantOfTheBritannianRoyalGuard ), 	typeof( Artifact_OssianGrimoire ), 	typeof( Artifact_RoyalArchersBow ), 	typeof( Artifact_StitchersMittens ), 	typeof( Artifact_WildfireBow ), 	typeof( Artifact_LithosTome ),
				typeof( Artifact_ArcaneShield ), 	typeof( Artifact_Boomstick ), 	typeof( Artifact_ConansSword ), 	typeof( Artifact_FangOfRactus ), 	typeof( Artifact_GrimReapersLantern ), 	typeof( Artifact_InquisitorsGorget ), 	typeof( Artifact_LongShot ), 	typeof( Artifact_OverseerSunderedBlade ), 	typeof( Artifact_RoyalGuardsChestplate ), 	typeof( Artifact_Stormbringer ), 	typeof( Artifact_Windsong ), 	
				typeof( Artifact_ArcaneTunic ), 	typeof( Artifact_BootsofHermes ), 	typeof( Artifact_CrimsonCincture ), 	typeof( Artifact_FesteringWound ), 	typeof( Artifact_GrimReapersMask ), 	typeof( Artifact_InquisitorsHelm ), 	typeof( Artifact_LuckyEarrings ), 	typeof( Artifact_Pacify ), 	typeof( Artifact_RoyalGuardsGorget ), 	typeof( Artifact_Subdue ), 	typeof( Artifact_WizardsPants ), 	
				typeof( Artifact_ArcanicRobe ), 	typeof( Artifact_BowOfTheJukaKing ), 	typeof( Artifact_CrownOfTalKeesh ), 	typeof( Artifact_FeyLeggings ), 	typeof( Artifact_GrimReapersRobe ), 	typeof( Artifact_InquisitorsLeggings ), 	typeof( Artifact_LuckyNecklace ), 	typeof( Artifact_PadsOfTheCuSidhe ), 	typeof( Artifact_RoyalGuardSurvivalKnife ), 	typeof( Artifact_SwiftStrike ), 	typeof( Artifact_WrathOfTheDryad ), 	
				typeof( Artifact_ArcticBeacon ), 	typeof( Artifact_BowofthePhoenix ), 	typeof( Artifact_DaggerOfVenom ), 	typeof( Artifact_FleshRipper ), 	typeof( Artifact_GrimReapersScythe ), 	typeof( Artifact_InquisitorsResolution ), 	typeof( Artifact_LuminousRuneBlade ), 	typeof( Artifact_PendantOfTheMagi ), 	typeof( Artifact_RuneCarvingKnife ), 	typeof( Artifact_TalonBite ), 	typeof( Artifact_YashimotosHatsuburi ), 	
				typeof( Artifact_ArcticDeathDealer ), 	typeof( Artifact_BraceletOfHealth ), 	typeof( Artifact_DarkGuardiansChest ), 	typeof( Artifact_Fortifiedarms ), 	typeof( Artifact_GuantletsOfAnger ), 	typeof( Artifact_InquisitorsTunic ), 	typeof( Artifact_LunaLance ), 	typeof( Artifact_Pestilence ), 	typeof( Artifact_SamaritanRobe ), 	typeof( Artifact_TheBeserkersMaul ), 	typeof( Artifact_ZyronicClaw ), 	
				typeof( Artifact_ArmorOfFortune ), 	typeof( Artifact_BraceletOfTheElements ), 	typeof( Artifact_DarkLordsPitchfork ), 	typeof( Artifact_FortunateBlades ), 	typeof( Artifact_HammerofThor ), 	typeof( Artifact_IronwoodCrown ), 	typeof( Artifact_MadmansHatchet ), 	typeof( Artifact_PhantomStaff ), 	typeof( Artifact_SamuraiHelm ), 	typeof( Artifact_TheDragonSlayer ), 	typeof( Artifact_GwennosHarp ), 	
				typeof( Artifact_ArmorOfInsight ), 	typeof( Artifact_BraceletOfTheVile ), 	typeof( Artifact_DarkNeck ), 	typeof( Artifact_Frostbringer ), 	typeof( Artifact_HatOfTheMagi ), 	typeof( Artifact_JackalsArms ), 	typeof( Artifact_MagesBand ), 	typeof( Artifact_PixieSwatter ), 	typeof( Artifact_SerpentsFang ), 	typeof( Artifact_TheDryadBow ), 	typeof( Artifact_IolosLute ), 	
				typeof( Artifact_ArmorOfNobility ), 	typeof( Artifact_BrambleCoat ), 	typeof( Artifact_DeathsMask ), 	typeof( Artifact_FurCapeOfTheSorceress ), 	typeof( Artifact_HeartOfTheLion ), 	typeof( Artifact_JackalsCollar ), 	typeof( Artifact_MagiciansIllusion ), 	typeof( Artifact_PolarBearBoots ), 	typeof( Artifact_ShadowBlade ), 	typeof( Artifact_TheNightReaper ), 	typeof( Artifact_QuiverOfBlight ), 	
				typeof( Artifact_ArmsOfAegis ), 	typeof( Artifact_BraveKnightOfTheBritannia ), 	typeof( Artifact_DetectiveBoots ), 	typeof( Artifact_Fury ), 	typeof( Artifact_HellForgedArms ), 	typeof( Artifact_JackalsGloves ), 	typeof( Artifact_MagiciansMempo ), 	typeof( Artifact_PolarBearCape ), 	typeof( Artifact_ShadowDancerArms ), 	typeof( Artifact_TheRobeOfBritanniaAri ), 	typeof( Artifact_QuiverOfElements ), 	
				typeof( Artifact_ArmsOfFortune ), 	typeof( Artifact_BreathOfTheDead ), 	typeof( Artifact_DivineArms ), 	typeof( Artifact_GandalfsHat ), 	typeof( Artifact_HelmOfAegis ), 	typeof( Artifact_JackalsHelm ), 	typeof( Artifact_MarbleShield ), 	typeof( Artifact_PolarBearMask ), 	typeof( Artifact_ShadowDancerCap ), 	typeof( Artifact_TheTaskmaster ), 	typeof( Artifact_QuiverOfFire ), 	
				typeof( Artifact_ArmsOfInsight ), 	typeof( Artifact_BurglarsBandana ), 	typeof( Artifact_DivineCountenance ), 	typeof( Artifact_GandalfsRobe ), 	typeof( Artifact_HelmOfBrilliance ), 	typeof( Artifact_JackalsLeggings ), 	typeof( Artifact_MauloftheBeast ), 	typeof( Artifact_PowerSurge ), 	typeof( Artifact_ShadowDancerGloves ), 	typeof( Artifact_TitansHammer ), 	typeof( Artifact_QuiverOfIce ), 	
				typeof( Artifact_ArmsOfNobility ), 	typeof( Artifact_Calm ), 	typeof( Artifact_DivineGloves ), 	typeof( Artifact_GandalfsStaff ), 	typeof( Artifact_HelmOfInsight ), 	typeof( Artifact_JackalsTunic ), 	typeof( Artifact_MaulOfTheTitans ), 	typeof( Artifact_Quell ), 	typeof( Artifact_ShadowDancerGorget ), 	typeof( Artifact_TorchOfTrapFinding ), 	typeof( Artifact_QuiverOfInfinity ), 	
				typeof( Artifact_ArmsOfTheFallenKing ), 	typeof( Artifact_CandleCold ), 	typeof( Artifact_DivineGorget ), 	typeof( Artifact_GauntletsOfNobility ), 	typeof( Artifact_HelmOfSwiftness ), 	typeof( Artifact_JadeScimitar ), 	typeof( Artifact_MelisandesCorrodedHatchet ), 	typeof( Artifact_RaedsGlory ), 	typeof( Artifact_ShadowDancerLeggings ), 	typeof( Artifact_TotemArms ), 	typeof( Artifact_QuiverOfLightning )
			};
		public static Type[] ArtyTypes{ get{ return m_ArtyTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_RelicTypes = new Type[]
			{
				typeof( DDRelicCoins ),
				typeof( DDRelicClock1 ),			typeof( DDRelicClock2 ),				typeof( DDRelicClock3 ),
				typeof( DDRelicLight2 ), 			typeof( DDRelicLight1 ), 				typeof( DDRelicLight3 ),
				typeof( DDRelicVase ),				typeof( DDRelicPainting ),				typeof( DDRelicArts ),
				typeof( DDRelicStatue ), 			typeof( DDRelicRugAddonDeed ),			typeof( DDRelicWeapon ),
				typeof( DDRelicArmor ),				typeof( DDRelicJewels ),				typeof( DDRelicInstrument ),
				typeof( DDRelicScrolls ),			typeof( DDRelicCloth ),					typeof( DDRelicFur ),
				typeof( DDRelicDrink ),				typeof( DDRelicReagent ),				typeof( DDRelicOrbs ),
				typeof( DDRelicVase ),				typeof( DDRelicPainting ),				typeof( DDRelicArts ),
				typeof( DDRelicStatue ), 			typeof( DDRelicRugAddonDeed ),			typeof( DDRelicWeapon ),
				typeof( DDRelicArmor ),				typeof( DDRelicJewels ),				typeof( DDRelicInstrument ),
				typeof( DDRelicScrolls ),			typeof( DDRelicCloth ),					typeof( DDRelicFur ),
				typeof( DDRelicDrink ),				typeof( DDRelicReagent ),				typeof( DDRelicOrbs ),
				typeof( DDRelicVase ),				typeof( DDRelicPainting ),				typeof( DDRelicArts ),
				typeof( DDRelicStatue ), 			typeof( DDRelicRugAddonDeed ),			typeof( DDRelicWeapon ),
				typeof( DDRelicArmor ),				typeof( DDRelicJewels ),				typeof( DDRelicInstrument ),
				typeof( DDRelicScrolls ),			typeof( DDRelicCloth ),					typeof( DDRelicFur ),
				typeof( DDRelicDrink ),				typeof( DDRelicReagent ),				typeof( DDRelicOrbs ),
				typeof( DDRelicBearRugsAddonDeed ), typeof( DDRelicLeather ),				typeof( DDRelicAlchemy ),
				typeof( DDRelicBook ),				typeof( DDRelicBook ),					typeof( DDRelicBook ),
				typeof( DDRelicTablet ),			typeof( DDRelicGem ),					typeof( DDRelicBanner )
			};
		public static Type[] RelicTypes{ get{ return m_RelicTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_SeaTypes = new Type[]
			{
				typeof( AdmiralsHeartyRum ),
				typeof( ShipModelOfTheHMSCape ),
				typeof( SeahorseStatuette ),
				typeof( GhostShipAnchor ),
				typeof( AquariumEastAddonDeed ),
				typeof( LightHouseAddonDeed ),
				typeof( MarlinEastAddonDeed ),
				typeof( MarlinSouthAddonDeed ),
				typeof( DolphinSouthSmallAddonDeed ),
				typeof( SkullEastLargeAddonDeed ),
				typeof( SkullEastSmallAddonDeed ),
				typeof( SkullSouthLargeAddonDeed ),
				typeof( SkullSouthSmallAddonDeed ),
				typeof( DolphinSouthLargeAddonDeed ),
				typeof( DolphinEastLargeAddonDeed ),
				typeof( AquariumSouthAddonDeed ),
				typeof( DolphinEastSmallAddonDeed ),
				typeof( SeaShell )
			};
		public static Type[] SeaTypes{ get{ return m_SeaTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_SArtyTypes = new Type[]
			{
				typeof( GoldBricks ),				typeof( BedOfNailsDeed ),			typeof( DecoGinsengRoot ),		typeof( DecoRoseOfTrinsic ),
				typeof( PhillipsWoodenSteed ),		typeof( BoneCouchDeed ),			typeof( DecoGinsengRoot2 ),		typeof( DecoRoseOfTrinsic2 ),
				typeof( BoneTableDeed ),			typeof( DecoMandrake ),				typeof( DecoRoseOfTrinsic3 ),	typeof( SackOfHolding ),
				typeof( SoulStone ),				typeof( BoneThroneDeed ),			typeof( DecoMandrake2 ),		typeof( BrokenChair ),
				typeof( RedSoulstone ),				typeof( CreepyPortraitDeed ),		typeof( DecoMandrake3 ),		typeof( EmptyJar ),
				typeof( BlueSoulstone ),			typeof( DisturbingPortraitDeed ),	typeof( DecoMandrakeRoot ),		typeof( DecoFullJar ),
				typeof( HaunterMirrorDeed ),		typeof( DecoMandrakeRoot2 ),		typeof( HalfEmptyJar ),			typeof( AwesomeDisturbingPortraitDeed ),
				typeof( HangingSkeletonDeed ),		typeof( DirtPatch ),				typeof( DecoNightshade ),		typeof( DecoCrystalBall ),
				typeof( FlamingHeadDeed ),			typeof( EvilIdolSkull ),			typeof( DecoNightshade2 ),		typeof( DecoMagicalCrystal ),
				typeof( RewardBrazierDeed ),		typeof( WallBlood ),				typeof( DecoNightshade3 ),		typeof( DecoSpittoon ),
				typeof( BloodyPentagramDeed ),		typeof( SkullPole ),				typeof( DecoObsidian ),			typeof( DecoDeckOfTarot ),
				typeof( MonsterStatueDeed ),		typeof( DecoPumice ),				typeof( DecoDeckOfTarot2 ),		typeof( CandelabraOfSouls ),
				typeof( WeaponEngravingTool ),		typeof( DecoStatueDeed ),			typeof( DecoWyrmsHeart ),		typeof( DecoTarot ),
				typeof( IronMaidenDeed ),			typeof( GrizzledMareStatuette ),	typeof( DecoArrowShafts ),		typeof( DecoTarot2 ),
				typeof( StoneStatueDeed ),			typeof( TormentedChains ),			typeof( CrossbowBolts ),		typeof( DecoTarot3 ),
				typeof( MountedPixieWhiteDeed ),	typeof( DecoBlackmoor ),			typeof( EmptyToolKit ),			typeof( DecoTarot4 ),
				typeof( MountedPixieBlueDeed ),		typeof( DecoBloodspawn ),			typeof( EmptyToolKit2 ),		typeof( DecoTarot5 ),
				typeof( MountedPixieGreenDeed ),	typeof( DecoBrimstone ),			typeof( Lockpicks ),			typeof( DecoTarot6 ),
				typeof( MountedPixieLimeDeed ),		typeof( DecoDragonsBlood ),			typeof( ToolKit ),				typeof( DecoTarot7 ),
				typeof( MountedPixieOrangeDeed ),	typeof( DecoDragonsBlood2 ),		typeof( UnfinishedBarrel ),		typeof( Cards ),
				typeof( SacrificialAltarDeed ),		typeof( DecoEyeOfNewt ),			typeof( DecoRock2 ),			typeof( Cards2 ),
				typeof( UnsettlingPortraitDeed ),	typeof( DecoGarlic ),				typeof( DecoRocks ),			typeof( Cards3 ),
				typeof( GuillotineDeed ),			typeof( DecoGarlic2 ),				typeof( DecoRocks2 ),			typeof( Cards4 ),
				typeof( WindSpirit ),				typeof( DecoGarlicBulb ),			typeof( DecoRock ),				typeof( DecoCards5 ),
				typeof( SuitOfGoldArmorDeed ),		typeof( DecoGarlicBulb2 ),			typeof( DecoFlower ),			typeof( PlayingCards ),
				typeof( SuitOfSilverArmorDeed ),	typeof( DecoGinseng ),				typeof( DecoFlower2 ),			typeof( PlayingCards2 ),
				typeof( WoodenCoffinDeed ),			typeof( DecoGinseng2 ),				typeof( JokeBook ),				typeof( HorseArmor ),
				typeof( Dice4 ),					typeof( Dice6 ),					typeof( Dice8 ),				typeof( Dice10 ),
				typeof( Dice12 ),					typeof( Dice20 ),					typeof( MonsterManual ),		typeof( PlayersHandbook ),
				typeof( DungeonMastersGuide ),		typeof( GygaxStatue ), 				typeof( DragonOrbStatue ),		typeof( WizardsStatue ),
				typeof( PandorasBox ),				typeof( ColoringBook ),				typeof( EverlastingBottle ),	typeof( EverlastingLoaf ),
				typeof( GemOfSeeing ),				typeof( SlayerDeed ),				typeof( LuckyHorseShoes ),		typeof( FireHorn ),
				typeof( SmallBagofHolding ),		typeof( MediumBagofHolding ),		typeof( LargeBagofHolding ),	typeof( BagOfHolding ),
				typeof( DruidMirror ),				typeof( SpecialJars ),				typeof( EvilItems )
			};
		public static Type[] SArtyTypes{ get{ return m_SArtyTypes; } }

		private static Type[] m_SArtyOrientTypes = new Type[]
			{
				typeof( WhiteHangingLantern ),	typeof( ShojiLantern ),			typeof( RoundPaperLantern ),				
				typeof( RedHangingLantern ),	typeof( PaperLantern ),			typeof( TowerLantern ),				
				typeof( OrigamiPaper ),			typeof( WindChimes ),			typeof( FancyWindChimes ),				
				typeof( BambooScreen ),			typeof( ShojiScreen ),			typeof( OrientBasket1 ),				
				typeof( OrientBasket2 ),		typeof( OrientBasket3 ),		typeof( OrientBasket4 ),				
				typeof( OrientBasket5 ),		typeof( OrientalItems )
			};

		public static Type[] SArtyOrientTypes{ get{ return m_SArtyOrientTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_ProvisionsTypes = new Type[]
			{
				typeof( Backpack ),				typeof( Bag ),					typeof( SmallCrate ),
				typeof( Bandage ),				typeof( Kindling ),				typeof( Torch ),
				typeof( Candle ),				typeof( Lantern ),				typeof( Arrow ),
				typeof( Bolt ),					typeof( GrapplingHook ),		typeof( SmallTent ),
				typeof( Lockpick ),				typeof( SpoolOfThread ),		typeof( Bedroll ),
				typeof( ThrowingWeapon ),		typeof( MageEye ),				typeof( HarpoonRope ),
				typeof( CampersTent ),			typeof( TenFootPole ),			typeof( Spyglass ),
				typeof( WoodenBox )
			};
		public static Type[] ProvisionsTypes{ get{ return m_ProvisionsTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_RareItemTypes = new Type[]
			{
				typeof( SkeletonsKey ),			typeof( PaintCanvas ),					typeof( MagicalDyes ),
				typeof( HeavySharpeningStone ),	typeof( ConsecratedSharpeningStone ),	typeof( MyCircusTentEastAddonDeed ),
				typeof( ManyArrows100 ),		typeof( ManyBolts100 ),					typeof( MyTentSouthAddonDeed ),
				typeof( RoughSharpeningStone ),	typeof( DenseSharpeningStone ),			typeof( ElementalSharpeningStone ),
				typeof( ArtifactManual ),		typeof( light_dragon_brazier ), 		typeof( MoonStone ),
				typeof( CrystallineJar ),		typeof( TrophyBase ),					typeof( DockingLantern ),
				typeof( BoatBuild ),			typeof( TrapKit ),						typeof( MalletStake ), 
				typeof( CandleLarge ),			typeof( Candelabra ),					typeof( CandelabraStand ),
				typeof( HeavyWeightingStone ),	typeof( ConsecratedWeightingStone ),	typeof( MagicalWand ),
				typeof( HairDyeBottle ),		typeof( GothicCandelabraA ),			typeof( GothicCandelabraB ),
				typeof( RoughWeightingStone ),	typeof( DenseWeightingStone ),			typeof( ElementalWeightingStone ),
				typeof( RareAnvil ),			typeof( MasterSkeletonsKey ),			typeof( InvulnerabilityPotion ),
				typeof( ArmsBarrel ),			typeof( AlternateRealityMap ),			typeof( UnusualDyes ),
				typeof( NecromancerBarrel ),	typeof( CarpetBuild ),					typeof( DwarvenForge ),
				typeof( SmallHollowBook ),		typeof( LargeHollowBook ),				typeof( MagicSkeltonsKey ),
				typeof( HeavyBowString ),		typeof( ConsecratedBowString ),			typeof( MagicalWand ),
				typeof( SlaversNet ),			typeof( BrokenArmoireDeed ),			typeof( BrokenVanityDeed ),
				typeof( RoughBowString ),		typeof( DenseBowString ),				typeof( ElementalBowString ),
				typeof( BrokenBookcaseDeed ),	typeof( StandingBrokenChairDeed ),		typeof( BrokenCoveredChairDeed ),
				typeof( MountingBase ),			typeof( StuffingBasket ),				typeof( BrokenFallenChairDeed ),
				typeof( MagicPigment ),			typeof( RoseEastLargeAddonDeed ),		typeof( RoseEastSmallAddonDeed ),
				typeof( TelescopeAddonDeed ),	typeof( RoseSouthLargeAddonDeed ),		typeof( ECrystalThroneDeed ),
				typeof( ECrystalTableDeed ),	typeof( ECrystalSupplicantStatueDeed ),	typeof( RoseSouthSmallAddonDeed ),
				typeof( ECrystalBullDeed ),		typeof( ECrystalBrazierDeed ),			typeof( ECrystalRunnerStatueDeed ),
				typeof( ECrystalAltarDeed ),	typeof( ECrystalBeggarStatueDeed ),		typeof( GolemManual ),
				typeof( BrokenBedDeed ),		typeof( Runebook ),						typeof( RecallRune),
				typeof( SummonPrison ), 		typeof( BrokenChestOfDrawersDeed ),
			};

		public static Type[] RareItemTypes{ get{ return m_RareItemTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_CraftsTypes = new Type[]
			{
				typeof( Board ),				typeof( IronIngot ),			typeof( Leather ),				typeof( Fabric ),
				typeof( Jar ),					typeof( Bottle ),				typeof( BlankScroll ),			typeof( Feather ),
				typeof( Shaft ),				typeof( BrittleSkeletal ),		typeof( AmethystBlocks ),		typeof( Sand ),
				typeof( DemonSkins ),			typeof( RedScales ),			typeof( Log ),					typeof( IronOre ),
				typeof( Hides )
			};

		public static Type[] CraftsTypes{ get{ return m_CraftsTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_RuneMagic = new Type[]
			{
				typeof( MagicRuneBag ),		
				typeof( An ),				typeof( Bet ),			typeof( Corp ),			typeof( Des ),			typeof( Ex ),		
				typeof( Flam ),				typeof( Grav ),			typeof( Hur ),			typeof( In ),			typeof( Jux ),			typeof( Kal ),		
				typeof( Lor ),				typeof( Mani ),			typeof( Nox ),			typeof( Ort ),			typeof( Por ),			typeof( Quas ),		
				typeof( Rel ),				typeof( Sanct ),		typeof( Tym ),			typeof( Uus ),			typeof( Vas ),			typeof( Wis ),		
				typeof( Xen ),				typeof( Ylem ),			typeof( Zu )	
			};
		public static Type[] RuneMagic{ get{ return m_RuneMagic; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_Songs = new Type[]
			{
				typeof( ArmysPaeonScroll ),			typeof( EnchantingEtudeScroll ),		typeof( EnergyCarolScroll ),
				typeof( EnergyThrenodyScroll ),		typeof( FireCarolScroll ),				typeof( FireThrenodyScroll ),
				typeof( FoeRequiemScroll ),			typeof( IceCarolScroll ),				typeof( IceThrenodyScroll ),
				typeof( KnightsMinneScroll ),		typeof( MagesBalladScroll ),			typeof( MagicFinaleScroll ),
				typeof( PoisonCarolScroll ),		typeof( PoisonThrenodyScroll ),			typeof( SheepfoeMamboScroll ),
				typeof( SinewyEtudeScroll )
			};
		public static Type[] Songs{ get{ return m_Songs; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_Books = new Type[]
			{
				typeof( SomeRandomNote ),			typeof( SomeRandomNote ), 					typeof( SomeRandomNote ),
				typeof( ScrollClue ),				typeof( ScrollClue ),						typeof( ScrollClue ),
				typeof( LoreBook ),					typeof( LoreBook ),							typeof( LoreBook ),
				typeof( LoreBook ),					typeof( LoreBook ),							typeof( LoreBook ),
				typeof( PlaceMap ),					typeof( PlaceMap ),							typeof( PlaceMap ),
				typeof( WritingBook ),				typeof( WritingBook ),						typeof( WritingBook ),
				typeof( WritingBook ),				typeof( WritingBook ),						typeof( WritingBook ),
				typeof( WritingBook ),				typeof( WritingBook ),						typeof( WritingBook ),
				typeof( MapRanger ),				typeof( GoldenRangers ),					typeof( MapWorld ),
				typeof( BookDruidBrewing ),			typeof( BookWitchBrewing ),					typeof( LearnWoodBook ),
				typeof( LearnTraps ),				typeof( LearnTitles ),						typeof( LearnTailorBook ),
				typeof( LearnStealingBook ),		typeof( LearnScalesBook ),					typeof( LearnReagentsBook ),
				typeof( LearnMiscBook ),			typeof( LearnMetalBook ),					typeof( LearnLeatherBook ),
				typeof( LearnGraniteBook ),			typeof( AlchemicalMixtures ),				typeof( BookOfPoisons ),
				typeof( WorkShoppes ),				typeof( SwordsAndShackles ),				typeof( QuestTake ),
				typeof( DDRelicBook ),				typeof( JokeBook ),							typeof( SmallHollowBook ),
				typeof( LargeHollowBook ),			typeof( AlchemicalElixirs ),				typeof( RuneJournal )
			};
		public static Type[] Books{ get{ return m_Books; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_ToolTypes = new Type[]
			{
				typeof( Pickaxe ),				typeof( Scissors ),				typeof( Dyes ),
				typeof( DyeTub ),				typeof( FletcherTools ),		typeof( Spade ),
				typeof( MapmakersPen ),			typeof( MortarPestle ),			typeof( FishingPole ),
				typeof( CarpenterTools ),		typeof( ScribesPen ),			typeof( LeatherworkingTools ),
				typeof( UndertakerKit ),		typeof( CulinarySet ),			typeof( TinkerTools ),
				typeof( SewingKit ),			typeof( ScalingTools ),			typeof( SmithHammer ),
				typeof( GraveSpade ),			typeof( DruidCauldron ),		typeof( WitchCauldron ),
				typeof( Hatchet ),				typeof( InteriorDecorator ),	typeof( HousePlacementTool ), 
				typeof( PolishBoneBrush ),		typeof( WoodworkingTools ),		typeof( Monocle ),
				typeof( WaxingPot ),			typeof( SkinningKnifeTool ),
			};
		public static Type[] ToolTypes{ get{ return m_ToolTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_JunkTypes = new Type[]
			{
				typeof( RustyJunk ),			typeof( RustyJunk ),			typeof( RustyJunk ),		typeof( Basket ),
				typeof( MetalBox ),				typeof( Pouch ),
				typeof( Chessboard ),			typeof( CheckerBoard ),			typeof( Backgammon ),
				typeof( Dices ),				typeof( tarotpoker ),			typeof( MahjongGame ),		typeof( TarotDeck ),
				typeof( Beeswax ),				typeof( OilCloth ),				typeof( Scales ),
				typeof( Axle ),					typeof( AxleGears ),			typeof( ClockFrame ),
				typeof( ClockParts ),			typeof( Gears ),				typeof( SextantParts ),
				typeof( Springs ),				typeof( Fork ),					typeof( ForkLeft ),
				typeof( ForkRight ),			typeof( Spoon ),				typeof( SpoonLeft ),
				typeof( SpoonRight ),			typeof( Knife ),				typeof( KnifeLeft ),
				typeof( KnifeRight ),			typeof( Plate ),
				typeof( CeramicMug ),			typeof( PewterMug ),			typeof( Goblet ),			typeof( SkullMug ),
				typeof( GlassMug ),				typeof( Pitcher ),				typeof( WritingBook ), 
				typeof( Candle ),				typeof( CandleLarge ),			typeof( CandleLong ), 
				typeof( CandleShort ),			typeof( CandleSkull ),			typeof( WritingBook ),		typeof( Pillows ),
				typeof( Brazier ),				typeof( BrazierTall ),			typeof( WallTorch ),		typeof( ColoredWallTorch )
			};
		public static Type[] JunkTypes{ get{ return m_JunkTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_SciFiItemTypes = new Type[]
			{
				typeof( SciFiJunk ),			typeof( SciFiJunk ),				typeof( SciFiJunk ),
				typeof( SciFiJunk ),			typeof( SciFiJunk ),				typeof( SciFiJunk ),
				typeof( SciFiJunk ),			typeof( SciFiJunk ),				typeof( SciFiJunk ),
				typeof( SciFiJunk ),			typeof( SciFiJunk ),				typeof( SciFiJunk ),
				typeof( SciFiJunk ),			typeof( SciFiJunk ),				typeof( SciFiJunk ),
				typeof( SciFiJunk ),			typeof( SciFiJunk ),				typeof( SciFiJunk ),
				typeof( SciFiJunk ),			typeof( SciFiJunk ),				typeof( SciFiJunk ),
				typeof( SciFiJunk ),			typeof( SciFiJunk ),				typeof( SciFiJunk ),
				typeof( SciFiJunk ),			typeof( SciFiJunk ),				typeof( SciFiJunk ),
				typeof( SciFiJunk ),			typeof( SciFiJunk ),				typeof( SciFiJunk ),

				typeof( DataPad ),				typeof( DataPad ),					typeof( DataPad ),

				typeof( Bandage ),				typeof( SkeletonsKey ),				typeof( MasterSkeletonsKey ),
				typeof( Lockpick ), 			typeof( Jar ), 						typeof( Bottle ),
				typeof( Fork ), 				typeof( ForkLeft ),
				typeof( ForkRight ), 			typeof( Spoon ), 					typeof( SpoonLeft ),
				typeof( SpoonRight ), 			typeof( Knife ), 					typeof( KnifeLeft ),
				typeof( KnifeRight ), 			typeof( Plate ), 					typeof( GlassMug ),
				typeof( Pillows ),				typeof( Bedroll ),
				typeof( SmallTent ),			typeof( CampersTent ),				typeof( PlasmaTorch ),

				typeof( Krystal ),				typeof( Spyglass ),					typeof( ArtifactManual ),
				typeof( LandmineSetup ),		typeof( PlasmaGrenade),				typeof( DataPad ),
				typeof( PuzzleCube ),			typeof( DuctTape ),					typeof( PortableSmelter ),
				typeof( FirstAidKit ),			typeof( ThermalDetonator ),			typeof( Chainsaw ),

				typeof( RobotBatteries ),		typeof( RobotSheetMetal ),			typeof( RobotOil ),
				typeof( RobotGears ),			typeof( RobotEngineParts ),			typeof( RobotCircuitBoard ),
				typeof( RobotBolt ),			typeof( RobotTransistor ),			typeof( MaterialLiquifier )
			};

		public static Type[] SciFiItemTypes{ get{ return m_SciFiItemTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		private static Type[] m_HorrorTypes = new Type[]
			{
				typeof( GuillotineDeed ),					typeof( IronMaidenDeed ),			typeof( ScarecrowDeed ),	
				typeof( WoodenCoffinDeed ),					typeof( UnsettlingPortraitDeed ),	typeof( BoneCouchDeed ),	
				typeof( AwesomeDisturbingPortraitDeed ),	typeof( BoneTableDeed ),			typeof( BoneThroneDeed ),	
				typeof( CreepyPortraitDeed ),				typeof( DisturbingPortraitDeed ),	typeof( HaunterMirrorDeed ),	
				typeof( SacrificialAltarDeed ),				typeof( BedOfNailsDeed ),			typeof( ESpikeColumnDeed ),	
				typeof( ESpikePostEastDeed ),				typeof( ESpikePostSouthDeed ),		typeof( EObsidianPillarDeed ),	
				typeof( EObsidianRockDeed ),				typeof( EShadowAltarDeed ),			typeof( EShadowBannerDeed ),	
				typeof( EShadowFirePitDeed ),				typeof( EShadowFirePitCrossDeed ),	typeof( EShadowPillarDeed ),	
				typeof( BloodyPentagramDeed ),				typeof( EvilItems ),				typeof( EvilItems ),
				typeof( EvilItems ),						typeof( EvilItems ),				typeof( EvilItems ),
				typeof( BrokenArmoire ),					typeof( BrokenBookcase ),			typeof( BrokenDrawer ),
				typeof( NecromancerBanner ),				typeof( NecromancerTable ),			typeof( BloodPentagramDeed ),
				typeof( BloodyTableAddonDeed ),				typeof( DeadBodyEWDeed ),			typeof( DeadBodyNSDeed ),
				typeof( EvilFireplaceSouthFaceAddonDeed ),	typeof( HalloweenBlood ),			typeof( HalloweenBonePileDeed ),
				typeof( HalloweenChopper ),					typeof( HalloweenColumn ),			typeof( HalloweenGrave1 ),
				typeof( HalloweenGrave2 ),					typeof( HalloweenGrave3 ),			typeof( HalloweenMaiden ),
				typeof( HalloweenPylon ),					typeof( HalloweenPylonFire ),		typeof( HalloweenShrineChaosDeed ),
				typeof( HalloweenSkullPole ),				typeof( HalloweenStoneColumn ),		typeof( HalloweenStoneSpike ),
				typeof( HalloweenStoneSpike2 ),				typeof( HalloweenTortSkel ),		typeof( halloween_coffin_eastAddonDeed ),
				typeof( halloween_block_eastAddonDeed ),	typeof( LargeDyingPlant ),			typeof( DyingPlant ),
				typeof( PumpkinScarecrow ),					typeof( GrimWarning ),				typeof( SkullsOnPike ),
				typeof( BlackCatStatue ),					typeof( RuinedTapestry ),			typeof( HalloweenWeb ),
				typeof( halloween_shackles ),				typeof( halloween_ruined_bookcase ),typeof( halloween_covered_chair ),
				typeof( halloween_HauntedMirror1 ),			typeof( halloween_HauntedMirror2 ),	typeof( halloween_devil_face ),
				typeof( BurningScarecrowA ),				typeof( EerieGhost ),				typeof( TrainingDaemonEastDeed ),
				typeof( DaemonDartBoardEastDeed )
			};
		public static Type[] HorrorTypes{ get{ return m_HorrorTypes; } }

		// ------------------------------------------------------------------------------------------------------------------------------------------

		#endregion

		#region Accessors

		public static Item RandomHorror()
		{
			return Construct( m_HorrorTypes );
		}

		public static Item RandomCrafts()
		{
			return Construct( m_CraftsTypes );
		}

		public static Item RandomProvisions()
		{
			return Construct( m_ProvisionsTypes );
		}

		public static Item RandomRuneMagic()
		{
			return Construct( m_RuneMagic );
		}

		public static Item RandomBooks( int level )
		{
			Item item = Construct( m_Books );

			if ( ( item is ScrollClue || item is SomeRandomNote ) && Utility.RandomBool() )
			{
				item.Delete();

				level = (int)(level/2);
				if ( level < 0 )
					level = 1;

				Map map = Map.Sosaria;
				switch( Utility.Random( 6 ) )
				{
					case 0: map = Map.Sosaria; break;
					case 1: map = Map.Lodor; break;
					case 2: map = Map.SerpentIsland; break;
					case 3: map = Map.IslesDread; break;
					case 4: map = Map.SavagedEmpire; break;
					case 5: map = Map.Underworld; break;
				}

				Point3D loc = new Point3D( 200, 200, 0 );
				item = new TreasureMap( level, map, loc, 200, 200 );
			}

			return item;
		}

		public static Item RandomTools()
		{
			return Construct( m_ToolTypes );
		}

		public static Item RandomJunk()
		{
			return Construct( m_JunkTypes );
		}

		public static Item RandomSciFiItems()
		{
			Item item = Construct( m_SciFiItemTypes );
			item.Technology = true;
			return item;
		}

		public static Item RandomItem( Mobile m, int level )
		{
			Item item = null;

			if ( level == -10 ) // VENDOR BAGS
			{
				switch ( Utility.RandomMinMax( 0, 6 ) ) 
				{
					case 0: item = RandomFoods( false, true ); break;
					case 1: item = RandomPossibleReagent(); break;
					case 2: item = RandomProvisions(); break;
					case 3: item = RandomJunk(); break;
					case 4: item = RandomTools(); break;
					case 5: item = RandomCrafts(); break;
					case 6: item = RandomRelic( m ); break;
				}

				if ( item.Stackable )
					item.Amount = item.DefaultCatalog == Catalogs.Gem ? Utility.RandomMinMax( 2, 4 ) : Utility.RandomMinMax( 1, 10 );

				if ( item == null || item.Weight > 10 || item.TotalWeight > 10 )
				{
					item.Delete();
					item = new Dagger();
				}
			}
			else
			{
				int var = Worlds.isSciFiRegion( m ) ? Utility.RandomMinMax( 0, 4 ): Utility.RandomMinMax( 1, 7 );
				switch ( var ) 
				{
					case 0: item = RandomSciFi(); break;
					case 1: if ( Utility.RandomBool() ){ item = RandomFoods( false, false ); } else { item = RandomPossibleReagent(); } break;

					case 2:
					case 3: item = RandomRelic( m ); break;

					case 4: item = RandomBooks( Utility.Random(4)+1 ); break;
					case 5: item = RandomProvisions(); break;
					case 6: if ( Utility.RandomBool() ){ item = RandomJunk(); } else { item = RandomCoins( m ); } break;
					case 7: if ( Utility.RandomBool() ){ item = RandomTools(); } else { item = RandomCrafts(); } break;
				}
			}

			if ( item == null )
				item = RandomCoins( m );

			return item;
		}

		public static Item RandomTreasure( Mobile m, int level )
		{
			Item item = null;

			int var = Utility.RandomMinMax( 2, 11 );
			if ( Worlds.isSciFiRegion( m ) )
				var = Utility.RandomMinMax( 0, 3 );

			switch ( var ) 
			{
				case 0: item = RandomSciFi(); break;
				case 1: item = RandomSciFi(); break;
				case 2: item = RandomFoods( false, false ); break;
				case 3: item = RandomPossibleReagent(); break;
				case 4: item = RandomTools(); break;
				case 5: item = RandomStatue(); break;
				case 6: item = RandomRelic( m ); break;
				case 7: item = RandomRelic( m ); break;
				case 8: item = RandomRare( level, m ); break;
				case 9: item = RandomJunk(); break;
				case 10: item = RandomJunk(); break;
				case 11: item = RandomJunk(); break;
			}

			if ( item == null )
				item = RandomCoins( m );

			return item;
		}

		public static BaseClothing RandomClothing()
		{
			return RandomClothing( false );
		}

		public static BaseClothing RandomClothing( bool playOrient )
		{
			BaseClothing item = null;

			if ( playOrient )
				item = Construct( m_OrientClothingTypes, m_ClothingTypes ) as BaseClothing;
			else
				item = Construct( m_ClothingTypes ) as BaseClothing;

			if ( !MyServerSettings.MonstersAllowed() && item is HikingBoots )
			{
				item.Delete();
				item = new ThighBoots();
			}

			return item;
		}

		public static BaseHat RandomHats()
		{
			return RandomHats( false );
		}

		public static BaseHat RandomHats( bool playOrient )
		{
			if ( playOrient )
				return Construct( m_OrientHatTypes, m_HatTypes ) as BaseHat;

			return Construct( m_HatTypes ) as BaseHat;
		}

		public static BaseWeapon RandomRangedWeapon()
		{
			return RandomRangedWeapon( false );
		}

		public static BaseWeapon RandomRangedWeapon( bool playOrient )
		{
			if ( playOrient )
				return Construct( m_OrientRangedWeaponTypes, m_RangedWeaponTypes ) as BaseWeapon;

			return Construct( m_RangedWeaponTypes ) as BaseWeapon;
		}

		public static BaseWeapon RandomSciFiGun()
		{
			return Construct( m_SciFiGunTypes ) as BaseWeapon;
		}

		public static BaseWeapon RandomWeapon()
		{
			return RandomWeapon( false );
		}

		public static BaseWeapon RandomWeapon( bool playOrient )
		{
			if ( playOrient )
				return Construct( m_OrientWeaponTypes, m_WeaponTypes ) as BaseWeapon;

			return Construct( m_WeaponTypes ) as BaseWeapon;
		}

		public static BaseWeapon RandomSciFiWeapon()
		{
			return Construct( m_SciFiWeaponTypes ) as BaseWeapon;
		}

		public static BaseTrinket RandomJewelry()
		{
			return Construct( m_JewelryTypes ) as BaseTrinket;
		}

		public static BaseArmor RandomArmor()
		{
			return RandomArmor( false );
		}

		public static BaseArmor RandomArmor( bool playOrient )
		{
			return playOrient
				? Construct(m_OrientArmorTypes, m_ArmorTypes) as BaseArmor
				: Construct(m_ArmorTypes) as BaseArmor;
		}

		public static Spellbook RandomSpellbook()
		{
			return RandomSpellbook( false );
		}

		public static Spellbook RandomSpellbook( bool playOrient )
		{
			if ( playOrient )
				return Construct( m_OrientSpellbooks, m_Spellbooks ) as Spellbook;

			return Construct( m_Spellbooks ) as Spellbook;
		}

		public static BaseHat RandomHat()
		{
			return RandomHat( false );
		}

		public static BaseHat RandomHat( bool playOrient )
		{
			if ( playOrient )
				return Construct( m_OrientHatTypes, m_HatTypes ) as BaseHat;

			return Construct( m_HatTypes ) as BaseHat;
		}

		public static BaseShield RandomShield()
		{
			return Construct( m_ShieldTypes ) as BaseShield;
		}

		public static BaseArmor RandomArmorOrShield()
		{
			return RandomArmorOrShield( false );
		}

		public static BaseArmor RandomArmorOrShield( bool playOrient )
		{
			if ( playOrient )
				return Construct( m_OrientArmorTypes, m_ArmorTypes, m_ShieldTypes ) as BaseArmor;

			return Construct( m_ArmorTypes, m_ShieldTypes ) as BaseArmor;
		}

		public static Item RandomMagicalItem()
		{
			return RandomMagicalItem( false );
		}

		public static Item RandomMagicalItem( bool playOrient )
		{
			if ( playOrient )
				return Construct( m_WeaponTypes, m_RangedWeaponTypes, m_ArmorTypes, m_HatTypes, m_ClothingTypes, m_Spellbooks, m_OrientWeaponTypes, m_OrientRangedWeaponTypes, m_OrientArmorTypes, m_OrientHatTypes, m_ShieldTypes, m_JewelryTypes, m_OrientClothingTypes, m_OrientSpellbooks, m_InstrumentTypes, m_QuiverTypes );

			return Construct( m_WeaponTypes, m_RangedWeaponTypes, m_ArmorTypes, m_HatTypes, m_ShieldTypes, m_JewelryTypes, m_ClothingTypes, m_Spellbooks, m_InstrumentTypes, m_QuiverTypes );
		}

		public static Item RandomFoods( bool foodOnly, bool cleanOnly )
		{
			Item item = null;

			if ( Utility.RandomBool() && !foodOnly )
			{
				item = RandomDrink();
				if ( item is BaseBeverage )
				{
					BaseBeverage drink = (BaseBeverage)item;
					if ( Utility.Random(20) == 0 && !cleanOnly )
						drink.Poison = Food.PoisonLevel();
				}
			}
			else
			{
				item = Construct( m_FoodsTypes );
				if ( item is Food )
				{
					Food food = (Food)item;
					if ( Utility.Random(20) == 0 && !cleanOnly )
						food.Poison = Food.PoisonLevel();
				}
			}

			return item;
		}

		public static Item RandomDrink()
		{
			Item drink = null;

			switch ( Utility.Random( 18 ) ) 
			{
				case 0: drink = new BeverageBottle( BeverageType.Milk ); break;
				case 1: drink = new BeverageBottle( BeverageType.Wine ); break;
				case 2: drink = new BeverageBottle( BeverageType.Cider ); break;
				case 3: drink = new BeverageBottle( BeverageType.Ale ); break;
				case 4: drink = new BeverageBottle( BeverageType.Water ); break;
				case 5: drink = new BeverageBottle( BeverageType.Liquor ); break;
				case 6: drink = new Jug( BeverageType.Milk ); break;
				case 7: drink = new Jug( BeverageType.Wine ); break;
				case 8: drink = new Jug( BeverageType.Cider ); break;
				case 9: drink = new Jug( BeverageType.Ale ); break;
				case 10: drink = new Jug( BeverageType.Water ); break;
				case 11: drink = new Jug( BeverageType.Liquor ); break;
				case 12: drink = new Pitcher( BeverageType.Milk ); break;
				case 13: drink = new Pitcher( BeverageType.Ale ); break;
				case 14: drink = new Pitcher( BeverageType.Cider ); break;
				case 15: drink = new Pitcher( BeverageType.Liquor ); break;
				case 16: drink = new Pitcher( BeverageType.Wine ); break;
				case 17: drink = new Pitcher( BeverageType.Water ); break;
			}

			return drink;
		}

		public static Item RandomGem()
		{
			return Construct( m_GemTypes );
		}

		public static Item RandomArty()
		{
			return Construct( m_ArtyTypes );
		}

		public static bool isBag( Item i )
		{
			if ( i is SmallBagofHolding )
				return true;
			else if ( i is MediumBagofHolding )
				return true;
			else if ( i is LargeBagofHolding )
				return true;
			else if ( i is BagOfHolding )
				return true;

			return false;
		}

		public static Item RandomSArty( bool playOrient, Mobile m )
		{
			Item i = null;

			if ( playOrient && Utility.RandomBool() )
				i = Construct( m_SArtyOrientTypes, m_SArtyTypes );
			else
				i = Construct( m_SArtyTypes );
				
			if ( m == null && Utility.RandomBool() && isBag( i ) )
			{
				i.Delete();
				i = Construct( m_SArtyTypes );
			}

			return i;
		}

		public static Item RandomRare( int level, Mobile from )
		{
			level = LootPackChange.ScaleLevel( level );

			Item item = Construct( m_RareItemTypes );

			if ( ( item is SmallHollowBook || item is LargeHollowBook ) && Utility.RandomBool() )
			{
				int filler = (int)(level/2)+1; if (filler < 1){ filler = 1; }
				int fillup = filler + 2;
				int rich = level * 7;
				int iRich = rich + 7;
				
				Container iBook = (Container)item;
				int booking = 0;

				int booked = Utility.RandomMinMax( filler,fillup );
				for ( int b = 0; b < booked; ++b )
				{
					int buk = 1; if ( !Worlds.isSciFiRegion( from ) ){ buk = 0; }
					booking = Utility.RandomMinMax( buk, 3 );

					if ( (level * 9) > Utility.Random( 100 ) )
					{
						if ( booking == 0 )
							iBook.DropItem( Loot.RandomScroll( level ) );
						else if ( booking == 1 )
							iBook.DropItem( RandomPotion( level, true ) );
						else if ( booking == 2 )
							{ Item wand = new MagicalWand(Utility.RandomMinMax(1,4)); iBook.DropItem( wand ); }
						else
							iBook.DropItem( Loot.RandomGem() );
					}
				}
			}

			return item;
		}

		public static Item RandomRelic( Mobile m )
		{
			if ( Worlds.IsWaterSea( m ) )
				return Construct( m_SeaTypes, m_RelicTypes );
			else if ( GetPlayerInfo.OrientalPlay( m ) )
				return Construct( m_SArtyOrientTypes, m_RelicTypes );
			else if ( GetPlayerInfo.EvilPlay( m ) )
				return Construct( m_HorrorTypes, m_RelicTypes );

			return Construct( m_RelicTypes );
		}

		public static Item RandomSea()
		{
			return Construct( m_SeaTypes );
		}

		public static Item RandomSciFi()
		{
			return Construct( m_SciFiItemTypes );
		}

		public static Item RandomReagent()
		{
			return Construct( m_RegTypes );
		}

		public static Item RandomDruidReagent()
		{
			return Construct( m_DruidRegTypes );
		}

		public static Item RandomWitchReagent()
		{
			return Construct( m_WitchRegTypes );
		}

		public static Item RandomNecromancyReagent()
		{
			return Construct( m_NecroRegTypes );
		}

		public static Item RandomMixerReagent()
		{
			return Construct( m_MixerRegTypes );
		}

		public static Item RandomPossibleReagent()
		{
			return Construct( m_RegTypes, m_WitchRegTypes, m_NecroRegTypes, m_MixerRegTypes );
		}

		public static Item RandomCoins( Mobile m )
		{
			Item item = null;

			if ( m != null && Worlds.isSciFiRegion( m ) )
				item = new DDXormite( Utility.RandomMinMax( 5, 50 ) );
			else if ( m != null && m.Land == Land.Underworld )
				item = new DDJewels( Utility.RandomMinMax( 5, 50 ) );
			else if ( m != null && (Region.Find( m.Location, m.Map )).IsPartOf( "the Mines of Morinia" ) )
				item = new Crystals( Utility.RandomMinMax( 5, 20 ) );
			else if ( Utility.RandomBool() )
				item = new Gold( Utility.RandomMinMax( 10, 100 ) );
			else if ( Utility.RandomBool() )
				item = new DDSilver( Utility.RandomMinMax( 10, 500 ) );
			else
				item = new DDCopper( Utility.RandomMinMax( 10, 1000 ) );

			if ( item.Amount > 65000 )
				item.Amount = 65000;

			return item;
		}

		public static Item RandomPotion( int level, bool rnd )
		{
			if ( rnd )
				level = Utility.RandomMinMax( 1, level );

			if ( level > 8 )
				level = 3;
			else if ( level > 4 )
				level = 2;
			else
				level = 1;

			if ( level == 1 )
				return Construct( m_LowPotionTypes );
			else if ( level == 2 )
				return Construct( m_MedPotionTypes );

			return Construct( m_HighPotionTypes );
		}

		public static BaseInstrument RandomInstrument()
		{
			return Construct( m_InstrumentTypes ) as BaseInstrument;
		}

		public static Item RandomStatue()
		{
			return Construct( m_StatueTypes );
		}

		public static BaseQuiver RandomQuiver()
		{
			return Construct( m_QuiverTypes ) as BaseQuiver;
		}

		public static Item RandomScroll( int level )
		{
			if ( Utility.Random(100) == 0 )
				return Construct( m_RuneMagic );

			if ( level > 8 )
				level = Utility.RandomList( 1, 2, 2, 3, 3, 3, 3, 3 );
			else if ( level > 4 )
				level = Utility.RandomList( 1, 2, 2, 2 );
			else
				level = 1;

			if ( level == 1 )
				return Construct( m_LowScrollTypes );
			else if ( level == 2 )
				return Construct( m_MedScrollTypes );

			return Construct( m_HighScrollTypes );
		}

		public static Item RandomSong()
		{
			return Construct( m_Songs );
		}

		#endregion

		#region Construction methods
		public static Item Construct( Type type )
		{
			try
			{
				return Activator.CreateInstance( type ) as Item;
			}
			catch
			{
				return null;
			}
		}

		public static Item Construct( Type[] types )
		{
			if ( types.Length > 0 )
				return Construct( types, Utility.Random( types.Length ) );

			return null;
		}

		public static Item Construct( Type[] types, int index )
		{
			if ( index >= 0 && index < types.Length )
				return Construct( types[index] );

			return null;
		}

		public static Item Construct( params Type[][] types )
		{
			int totalLength = 0;

			for ( int i = 0; i < types.Length; ++i )
				totalLength += types[i].Length;

			if ( totalLength > 0 )
			{
				int index = Utility.Random( totalLength );

				for ( int i = 0; i < types.Length; ++i )
				{
					if ( index >= 0 && index < types[i].Length )
						return Construct( types[i][index] );

					index -= types[i].Length;
				}
			}

			return null;
		}
		#endregion
	}
}