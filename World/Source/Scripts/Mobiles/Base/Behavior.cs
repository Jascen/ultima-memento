using MoveImpl=Server.Movement.MovementImpl;
using Server.ContextMenus;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Spells.Fifth;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Server.Spells.Second;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Spells.Magical;
using Server.Spells.Shinobi;
using Server.Spells;
using Server.Targeting;
using Server.Targets;
using Server;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System;
using Server.Gumps;

namespace Server
{
	public class SpeedInfo
	{
		// Should we use the new method of speeds?
		private static bool Enabled = true;

		private double m_ActiveSpeed;
		private double m_PassiveSpeed;
		private Type[] m_Types;

		public double ActiveSpeed
		{
			get{ return m_ActiveSpeed; }
			set{ m_ActiveSpeed = value; }
		}

		public double PassiveSpeed
		{
			get{ return m_PassiveSpeed; }
			set{ m_PassiveSpeed = value; }
		}

		public Type[] Types
		{
			get{ return m_Types; }
			set{ m_Types = value; }
		}

		public SpeedInfo( double activeSpeed, double passiveSpeed, Type[] types )
		{
			m_ActiveSpeed = activeSpeed;
			m_PassiveSpeed = passiveSpeed;
			m_Types = types;
		}

		public static bool Contains( object obj )
		{
			if ( !Enabled )
				return false;

			if ( m_Table == null )
				LoadTable();

			SpeedInfo sp = (SpeedInfo)m_Table[obj.GetType()];

			return ( sp != null );
		}

		public static bool GetSpeeds( object obj, ref double activeSpeed, ref double passiveSpeed )
		{
			if ( !Enabled )
				return false;

			if ( m_Table == null )
				LoadTable();

			SpeedInfo sp = (SpeedInfo)m_Table[obj.GetType()];

			if ( sp == null )
				return false;

			activeSpeed = sp.ActiveSpeed;
			passiveSpeed = sp.PassiveSpeed;

			return true;
		}

		private static void LoadTable()
		{
			m_Table = new Hashtable();

			for ( int i = 0; i < m_Speeds.Length; ++i )
			{
				SpeedInfo info = m_Speeds[i];
				Type[] types = info.Types;

				for ( int j = 0; j < types.Length; ++j )
					m_Table[types[j]] = info;
			}
		}

		private static Hashtable m_Table;

		private static SpeedInfo[] m_Speeds = new SpeedInfo[]
			{
				/* Slow */
				new SpeedInfo( 0.3, 0.6, new Type[]
				{
					typeof( AbysmalOgre ),
					typeof( AcidPuddle ),
					typeof( AncientEnt ),
					typeof( AncientFleshGolem ),
					typeof( FrankenFighter ),
					typeof( Mutant ),
					typeof( AntLion ),
					typeof( Snapper ),
					typeof( Turtle ),
					typeof( CrystalGoliath ),
					typeof( MetalBeetle ),
					typeof( AnyStatue ),
					typeof( AquaticGhoul ),
					typeof( ArcticEttin ),
					typeof( ArcticOgreLord ),
					typeof( BlackPudding ),
					typeof( GreenSlime ),
					typeof( Jellyfish ),
					typeof( BloodSpawn ),
					typeof( Bogling ),
					typeof( BogThing ),
					typeof( BoneGolem ),
					typeof( BoneKnight ),
					typeof( KhumashGor ),
					typeof( CaddelliteElemental ),
					typeof( CaddelliteGolem ),
					typeof( CarcassWorm ),
					typeof( Slitheran ),
					typeof( CrystalElemental ),
					typeof( DarkReaper ),
					typeof( DiseasedMummy ),
					typeof( DriftwoodElemental ),
					typeof( EarthElemental ),
					typeof( ElementalLordEarth ),
					typeof( ElementalCalledEarth ),
					typeof( Elephant ),
					typeof( Ent ),
					typeof( Ettin ),
					typeof( EttinShaman ),
					typeof( EvilEnt ),
					typeof( AncientReaper ),
					typeof( FleshGolem ),
					typeof( FrailSkeleton ),
					typeof( GargoyleBones ),
					typeof( FrostOoze ),
					typeof( FrostTroll ),
					typeof( FrozenCorpse ),
					typeof( Ghoul ),
					typeof( GiantSkeleton ),
					typeof( Golem ),
					typeof( ExcavationDroid ),
					typeof( GraveDustElemental ),
					typeof( GraveSeeker ),
					typeof( HeadlessOne ),
					typeof( IceGhoul ),
					typeof( KelpElemental ),
					typeof( LavaElemental ),
					typeof( LivingBronzeStatue ),
					typeof( LivingGoldStatue ),
					typeof( LivingIronStatue ),
					typeof( LivingJadeStatue ),
					typeof( LivingMarbleStatue ),
					typeof( LivingShadowIronStatue ),
					typeof( LivingSilverStatue ),
					typeof( LivingStoneStatue ),
					typeof( Lobstran ),
					typeof( MetalGolem ),
					typeof( CombatDroid ), 
					typeof( BattleDroid ), 
					typeof( MaintenanceDroid ), 
					typeof( SecurityDroid ),
					typeof( ServiceDroid ),
					typeof( Mouse ),
					typeof( MudElemental ),
					typeof( MudMan ),
					typeof( Mummy ),
					typeof( MummyLord ),
					typeof( Neanderthal ),
					typeof( Morlock ),
					typeof( Durgar ),
					typeof( Necromental ),
					typeof( Ogre ),
					typeof( OgreLord ),
					typeof( OgreMagi ),
					typeof( OilSlick ),
					typeof( Quagmire ),
					typeof( Rat ),
					typeof( Reaper ),
					typeof( RestlessSoul ),
					typeof( RottingCorpse ),
					typeof( RustGolem ),
					typeof( GolemFighter ),
					typeof( Robot ),
					typeof( SeaweedElemental ),
					typeof( SeaWeeder ),
					typeof( SeaZombie ),
					typeof( Sewerrat ),
					typeof( SicklyRat ),
					typeof( Skeleton ),
					typeof( SkeletonArcher ),
					typeof( SkinGolem ),
					typeof( Slime ),
					typeof( TheAncientTree ),
					typeof( TundraOgre ),
					typeof( UndeadGiant ),
					typeof( Viscera ),
					typeof( WalkingCorpse ),
					typeof( WalkingReaper ),
					typeof( Walrus ),
					typeof( WaterSpawn ),
					typeof( WaxSculpture ),
					typeof( WeedElemental ),
					typeof( WoodenGolem ),
					typeof( Xorn ),
					typeof( Serpentaur ),
					typeof( Zombie ),
					typeof( ZombieMage ),
					typeof( ZombieGargoyle ),
					typeof( ZombieGiant ),
					typeof( ZombieSpider )
				} ),
				/* Fast */
				new SpeedInfo( 0.2, 0.4, new Type[]
				{
					typeof( HookHorror ),
					typeof( Megalodon ),
					typeof( Lochasaur ),
					typeof( Shark ),
					typeof( GreatWhite ),
					typeof( CaveFisher ),
					typeof( AbyssCrawler ),
					typeof( AirElemental ),
					typeof( ElementalCalledAir ),
					typeof( ElementalLordAir ),
					typeof( AncientNightmareRiding ),
					typeof( Placeron ),
					typeof( AncientWyrm ),
					typeof( AnyElemental ),
					typeof( AnyGemElemental ),
					typeof( AnimatedRocks ), 
					typeof( GemElemental ), 
					typeof( Arachnar ),
					typeof( AshDragon ),
					typeof( Balron ),
					typeof( Archfiend ),
					typeof( TitanPyros ),
					typeof( BlackGateDemon ),
					typeof( ElementalSpiritAir ),
					typeof( ElementalSpiritEarth ),
					typeof( ElementalSpiritFire ),
					typeof( ElementalSpiritWater ),
					typeof( ElementalFiendAir ),
					typeof( ElementalFiendEarth ),
					typeof( ElementalFiendFire ),
					typeof( ElementalFiendWater ),
					typeof( DeathVortex ),
					typeof( BoneSlasher ),
					typeof( GasCloud ),
					typeof( BladeSpirits ),
					typeof( BloodDemigod ),
					typeof( BloodDemon ),
					typeof( BottleDragon ),
					typeof( CaddelliteDragon ),
					typeof( Cerberus ),
					typeof( Chimera ),
					typeof( CinderElemental ),
					typeof( CrystalDragon ),
					typeof( DarkUnicornRiding ),
					typeof( DarkWisp ),
					typeof( DeepSeaDevil ),
					typeof( Dracula ),
					typeof( DreadSpider ),
					typeof( AntaurKing ), 
					typeof( AntaurProgenitor ), 
					typeof( AntaurSoldier ), 
					typeof( AntaurWorker ), 
					typeof( Drider ),
					typeof( DriderWizard ),
					typeof( DustElemental ),
					typeof( Efreet ),
					typeof( EtherealWarrior ),
					typeof( EvilBladeSpirits ),
					typeof( FireNaga ),
					typeof( FireSalamander ),
					typeof( FrostSpider ),
					typeof( GiantBlackWidow ),
					typeof( Tarantula ),
					typeof( Devil ),
					typeof( Roc ),
					typeof( Ifreet ),
					typeof( Afreet ),
					typeof( YoungRoc ),
					typeof( Lich ),
					typeof( LionRiding ),
					typeof( SnowLion ),
					typeof( Kilrathi ),
					typeof( KilrathiGunner ),
					typeof( CragCat ),
					typeof( ManticoreRiding ),
					typeof( Marilith ),
					typeof( Medusa ),
					typeof( MonstrousSpider ),
					typeof( Naga ),
					typeof( Nightmare ),
					typeof( OphidianArchmage ),
					typeof( OphidianKnight ),
					typeof( OphidianMage ),
					typeof( OphidianMatriarch ),
					typeof( OphidianWarrior ),
					typeof( Serpentar ),
					typeof( SerpentarWizard ),
					typeof( Serpyn ), 
					typeof( SandSerpyn ), 
					typeof( SerpynChampion ), 
					typeof( SerpynSorceress ), 
					typeof( PoisonElemental ),
					typeof( LightningElemental ),
					typeof( Revenant ),
					typeof( RuneBeetle ),
					typeof( SabretoothTigerRiding ),
					typeof( SandSpider ),
					typeof( SandVortex ),
					typeof( SavageRider ),
					typeof( SavageShaman ),
					typeof( ShadowRecluse ),
					typeof( AlienSpider ),
					typeof( Alien ),
					typeof( AlienSmall ),
					typeof( Shaclaw ),
					typeof( PhaseSpider ),
					typeof( ShadowWisp ),
					typeof( SnowElemental ),
					typeof( SteamElemental ),
					typeof( StormCloud ),
					typeof( SummonedAirElemental ),
					typeof( SummonedAirElementalGreater ),
					typeof( Sunlyte ),
					typeof( TigerRiding ),
					typeof( SummonedTiger ),
					typeof( Typhoon ),
					typeof( Tyranasaur ),
					typeof( Vampire ),
					typeof( VampireLord ),
					typeof( VampirePrince ),
					typeof( VampireWoods ),
					typeof( WaterNaga ),
					typeof( WaterStrider ),
					typeof( WhiteTigerRiding ),
					typeof( Wyrms ),
					typeof( Wyrm ),
					typeof( Wisp ),
					typeof( SummonedCorpse ),
					typeof( Xurtzar )
				} ),
				/* Very Fast */
				new SpeedInfo( 0.175, 0.350, new Type[]
				{
					typeof( Cheetah ),
					typeof( EnergyVortex ),
					typeof( Pixie ),
					typeof( Sprite ),
					typeof( Faerie ),
					typeof( SilverSerpent ),
					typeof( VorpalBunny ),
					typeof( Leviathan ),
					typeof( FireBeetle ),
					typeof( EvilEnergyVortex ),
					typeof( GoldenSerpent ),
					typeof( Ostrich ),
					typeof( AxeBeakRiding ),
					typeof( RavenousRiding ),
					typeof( RaptorRiding ),
					typeof( Raptus ),
					typeof( Xenomorph ), 
					typeof( Xenomutant ), 
					typeof( ElectricalElemental )
				} ),
				/* Medium */
				new SpeedInfo( 0.25, 0.5, new Type[]
				{
					typeof( AbysmalDaemon ),
					typeof( AbyssGiant ),
					typeof( AgapiteElemental ),
					typeof( Alligator ),
					typeof( SwampGator ),
					typeof( Toraxen ),
					typeof( AncientCyclops ),
					typeof( AncientDrake ),
					typeof( AncientLich ),
					typeof( AncientSphinx ),
					typeof( RoyalSphinx ),
					typeof( Anhkheg ),
					typeof( Ape ),
					typeof( Archmage ),
					typeof( BabyDragon ),
					typeof( MysticalFox ),
					typeof( Bandit ),
					typeof( BasiliskRiding ),
					typeof( Basilosaurus ),
					typeof( Bat ),
					typeof( VampireBat ),
					typeof( AlbinoBat ),
					typeof( Stirge ),
					typeof( Beetle ),
					typeof( Beholder ),
					typeof( Berserker ),
					typeof( Adventurers ),
					typeof( SavageAlien ),
					typeof( BombWorshipper ),
					typeof( Syth ),
					typeof( Jedi ),
					typeof( Psionicist ),
					typeof( HenchmanFighter ),
					typeof( HenchmanArcher ),
					typeof( HenchmanWizard ),
					typeof( HenchmanMonster ),
					typeof( Bird ),
					typeof( BlackBear ),
					typeof( SabreclawCub ),
					typeof( BlackCat ),
					typeof( AsianDragon ),
					typeof( Angel ),
					typeof( Archangel ),
					typeof( ElementalSteed ),
					typeof( PrimevalFireDragon ), 
					typeof( PrimevalGreenDragon ), 
					typeof( PrimevalNightDragon ), 
					typeof( PrimevalRedDragon ), 
					typeof( PrimevalRoyalDragon ), 
					typeof( PrimevalRunicDragon ), 
					typeof( PrimevalSeaDragon ), 
					typeof( ReanimatedDragon ), 
					typeof( VampiricDragon ), 
					typeof( PrimevalAbysmalDragon ), 
					typeof( PrimevalAmberDragon ), 
					typeof( PrimevalBlackDragon ), 
					typeof( PrimevalDragon ), 
					typeof( PrimevalSilverDragon ), 
					typeof( PrimevalVolcanicDragon ), 
					typeof( PrimevalStygianDragon ), 
					typeof( BlackKnight ),
					typeof( BloodAssassin ),
					typeof( BloodElemental ),
					typeof( BloodLotus ),
					typeof( BloodSnake ),
					typeof( BloodWorm ),
					typeof( SlasherOfVoid ),
					typeof( Boar ),
					typeof( BoneDemon ),
					typeof( BoneMagi ),
					typeof( SkeletalGargoyle ),
					typeof( BoneSailor ),
					typeof( Brigand ),
					typeof( BronzeElemental ),
					typeof( BrownBear ),
					typeof( Bugbear ),
					typeof( Bull ),
					typeof( BullFrog ),
					typeof( Frog ),
					typeof( Toad ),
					typeof( Cat ),
					typeof( CaveBearRiding ),
					typeof( CaveLizard ),
					typeof( Stalker ),
					typeof( Centaur ),
					typeof( Chicken ),
					typeof( Turkey ),
					typeof( CopperElemental ),
					typeof( Corpser ),
					typeof( CorruptCentaur ),
					typeof( Cougar ),
					typeof( SabretoothCub ),
					typeof( Cow ),
					typeof( Crane ),
					typeof( Cyclops ),
					typeof( Daemon ),
					typeof( Fiend ),
					typeof( Dagon ),
					typeof( DarkHound ),
					typeof( DeadKnight ),
					typeof( DeadlyScorpion ),
					typeof( DeadWizard ),
					typeof( DeathBear ),
					typeof( DeathwatchBeetle ),
					typeof( DeathwatchBeetleHatchling ),
					typeof( DeathWolf ),
					typeof( NecroticHound ),
					typeof( DeepSeaGiant ),
					typeof( TitanLich ), 
					typeof( MummyGiant ), 
					typeof( Giant ), 
					typeof( HillGiant ), 
					typeof( HillGiantShaman ), 
					typeof( DeepSeaSerpent ),
					typeof( Jormungandr ),
					typeof( Cronosaurus ), 
					typeof( DeepWaterElemental ),
					typeof( ElementalLordWater ),
					typeof( DemiLich ),
					typeof( Demon ),
					typeof( DaemonTemplate ),
					typeof( DemonDog ),
					typeof( DemonOfTheSea ),
					typeof( DesertOstard ),
					typeof( DireBoar ),
					typeof( WolfDire ),
					typeof( Worg ),
					typeof( Jackalwitch ),
					typeof( DiseasedRat ),
					typeof( Dog ),
					typeof( Dolphin ),
					typeof( Dracolich ),
					typeof( SkeletonDragon ), 
					typeof( Dragons ),
					typeof( YoungDragon ),
					typeof( Dragoon ),
					typeof( RidingDragon ),
					typeof( DragonGolem ),
					typeof( DragonKing ),
					typeof( Dragonogre ),
					typeof( DragonTurtle ),
					typeof( Drake ),
					typeof( DrakkhenRed ),
					typeof( DrakkhenBlack ),
					typeof( AbysmalDrake ),
					typeof( Drakkul ),
					typeof( DrakkulChief ),
					typeof( DrakkulMage ),
					typeof( DullCopperElemental ),
					typeof( Eagle ),
					typeof( Penguin ),
					typeof( ElderBlackBearRiding ),
					typeof( ElderBrownBearRiding ),
					typeof( ElderDragon ),
					typeof( ElderGazer ),
					typeof( Seeker ),
					typeof( Watcher ),
					typeof( ElderPolarBearRiding ),
					typeof( ElderTitan ),
					typeof( ElfBerserker ),
					typeof( ElfBoatPirateArcher ),
					typeof( ElfBoatPirateBard ),
					typeof( ElfBoatPirateMage ),
					typeof( ElfMage ),
					typeof( ElfMinstrel ),
					typeof( ElfMonks ),
					typeof( ElfPirateCaptain ),
					typeof( ElfPirateCrew ),
					typeof( ElfPirateCrewBow ),
					typeof( ElfPirateCrewMage ),
					typeof( ElfRogue ),
					typeof( ElfBoatSailorArcher ),
					typeof( ElfBoatSailorBard ),
					typeof( ElfBoatSailorMage ),
					typeof( EnergyHydra ),
					typeof( EvilMage ),
					typeof( EvilMageLord ),
					typeof( BoatPirateArcher ),
					typeof( BoatPirateBard ),
					typeof( BoatPirateMage ),
					typeof( Executioner ),
					typeof( Exodus ),
					typeof( EyeOfTheDeep ),
					typeof( Ferret ),
					typeof( FireBat ),
					typeof( FireDemon ),
					typeof( FireElemental ),
					typeof( ElementalCalledFire ),
					typeof( ElementalLordFire ),
					typeof( FireGargoyle ),
					typeof( FireGiant ),
					typeof( FireMephit ),
					typeof( FireSteed ),
					typeof( IceSteed ),
					typeof( FireToad ),
					typeof( FireWyrmling ),
					typeof( FloatingEye ),
					typeof( ForestGiant ),
					typeof( ForestOstard ),
					typeof( Fox ),
					typeof( FrenziedOstard ),
					typeof( FrostGiant ),
					typeof( Fungal ),
					typeof( FungalMage ),
					typeof( CreepingFungus ),
					typeof( BullradonRiding ),
					typeof( Gargoyle ),
					typeof( StygianGargoyle ),
					typeof( StygianGargoyleLord ),
					typeof( GargoyleAmethyst ),
					typeof( AncientGargoyle ),
					typeof( MutantGargoyle ), 
					typeof( CosmicGargoyle ), 
					typeof( GargoyleEmerald ),
					typeof( GargoyleMarble ),
					typeof( GargoyleOnyx ),
					typeof( GargoyleRuby ),
					typeof( CodexGargoyleA ), 
					typeof( CodexGargoyleB ), 
					typeof( GargoyleSapphire ),
					typeof( GarnetElemental ),
					typeof( Gazer ),
					typeof( GhostGargoyle ),
					typeof( DemonicGhost ),
					typeof( Ghostly ),
					typeof( Shroud ),
					typeof( GhostPirate ),
					typeof( GhostWarrior ),
					typeof( GhostWizard ),
					typeof( GiantAdder ),
					typeof( GiantBat ),
					typeof( GiantCrab ),
					typeof( GiantEel ),
					typeof( GiantLamprey ),
					typeof( GiantLeech ),
					typeof( MarshWurm ),
					typeof( GiantLizard ),
					typeof( GiantRat ),
					typeof( GiantSerpent ),
					typeof( GiantSnake ),
					typeof( GiantSpider ),
					typeof( GiantSquid ),
					typeof( SandSquid ),
					typeof( GiantToad ),
					typeof( Gnoll ),
					typeof( Goat ),
					typeof( Goblin ),
					typeof( GoblinArcher ),
					typeof( GoldenElemental ),
					typeof( GolemController ),
					typeof( GorceratopsRiding ),
					typeof( GorgonRiding ),
					typeof( Gorilla ),
					typeof( Monkey ),
					typeof( Gorakong ), 
					typeof( Infected ),
					typeof( Grathek ),
					typeof( GreatHart ),
					typeof( Gazelle ),
					typeof( Moose ),
					typeof( Antelope ),
					typeof( GreyWolf ),
					typeof( GriffonRiding ),
					typeof( HippogriffRiding ),
					typeof( GrizzlyBearRiding ),
					typeof( Grum ),
					typeof( Ramadon ),
					typeof( GrundulVarg ),
					typeof( Harpy ),
					typeof( HarpyElder ),
					typeof( HarpyHen ),
					typeof( GiantHawk ),
					typeof( GiantRaven ),
					typeof( BloodGodTentacles ),
					typeof( Hawk ),
					typeof( HellCat ),
					typeof( HellHound ),
					typeof( HellBeast ),
					typeof( Hind ),
					typeof( Hobgoblin ),
					typeof( HordeMinion ),
					typeof( Horse ),
					typeof( ZebraRiding ),
					typeof( HugeLizard ),
					typeof( Hydra ),
					typeof( IcebergElemental ),
					typeof( IceColossus ),
					typeof( IceElemental ),
					typeof( IceDevil ),
					typeof( IceGolem ),
					typeof( IceSerpent ),
					typeof( IceSnake ),
					typeof( IceToad ),
					typeof( Iguana ),
					typeof( Imp ),
					typeof( IronBeetle ),
					typeof( IronCobra ),
					typeof( Jackal ),
					typeof( Hyena ),
					typeof( JackRabbit ),
					typeof( Weasel ),
					typeof( JadeSerpent ),
					typeof( Jaguar ),
					typeof( JungleGiant ),
					typeof( JungleViper ),
					typeof( Tortuga ),
					typeof( ForestElemental ),
					typeof( Kirin ),
					typeof( Kobold ),
					typeof( Lurker ),
					typeof( KoboldMage ),
					typeof( Kraken ),
					typeof( Calamari ),
					typeof( SeaHorses ),
					typeof( Krakoa ),
					typeof( Kull ),
					typeof( LargeSnake ),
					typeof( LargeSpider ),
					typeof( LavaLizard ),
					typeof( Lavapede ),
					typeof( LavaPuddle ),
					typeof( LavaSerpent ),
					typeof( LavaSnake ),
					typeof( LesserDemon ),
					typeof( LesserSeaSnake ),
					typeof( LichKing ),
					typeof( LichLord ),
					typeof( Nazghoul ),
					typeof( Lizardman ),
					typeof( Reptaur ),
					typeof( LizardmanArcher ),
					typeof( Llama ),
					typeof( Giraffe ),
					typeof( Locathah ),
					typeof( LostKnight ),
					typeof( LowerDemon ),
					typeof( MadDog ),
					typeof( MagmaElemental ),
					typeof( MeteorElemental ),
					typeof( Mantis ),
					typeof( MechanicalScorpion ),
					typeof( Meglasaur ),
					typeof( MindFlayer ),
					typeof( Minotaur ),
					typeof( MinotaurCaptain ),
					typeof( RottingMinotaur ),
					typeof( MinotaurScout ),
					typeof( MinotaurSmall ),
					typeof( MutantMinotaur ),
					typeof( Minstrel ),
					typeof( MLDryad ),
					typeof( Mongbat ),
					typeof( Monks ),
					typeof( MountainGiant ),
					typeof( MountainGoat ),
					typeof( Murk ),
					typeof( Native ),
					typeof( NativeArcher ),
					typeof( NativeWitchDoctor ),
					typeof( Neptar ),
					typeof( NeptarWizard ),
					typeof( ObsidianElemental ),
					typeof( WoodlandDevil ),
					typeof( Orc ),
					typeof( OrcBomber ),
					typeof( OrcCaptain ),
					typeof( Orx ),
					typeof( OrxWarrior ),
					typeof( OrcishLord ),
					typeof( OrcishMage ),
					typeof( Gnome ),
					typeof( GnomeMage ),
					typeof( GnomeWarrior ),
					typeof( OrkDemigod ),
					typeof( OrkMage ),
					typeof( OrkMonks ),
					typeof( OrkRogue ),
					typeof( OrkWarrior ),
					typeof( Owlbear ),
					typeof( Trollbear ),
					typeof( PackHorse ),
					typeof( PackLlama ),
					typeof( PandaRiding ),
					typeof( Panther ),
					typeof( Bobcat ),
					typeof( Phantom ),
					typeof( Pig ),
					typeof( PirateCaptain ),
					typeof( PirateCrew ),
					typeof( PirateCrewBow ),
					typeof( PirateCrewMage ),
					typeof( PirateLand ),
					typeof( PoisonBeetleRiding ),
					typeof( Skellot ),
					typeof( PoisonCloud ),
					typeof( PoisonFrog ),
					typeof( PolarBear ),
					typeof( PredatorHellCatRiding ),
					typeof( QuartzElemental ),
					typeof( Rabbit ),
					typeof( RadiationDragon ),
					typeof( RandomSerpent ),
					typeof( Ratman ),
					typeof( RatmanArcher ),
					typeof( RatmanMage ),
					typeof( RevenantLion ),
					typeof( RidableLlama ),
					typeof( Ridgeback ),
					typeof( Rogue ),
					typeof( Roper ),
					typeof( RottingSquid ),
					typeof( SabretoothBearRiding ),
					typeof( BoatSailorArcher ),
					typeof( BoatSailorBard ),
					typeof( BoatSailorMage ),
					typeof( MutantLizardman ),
					typeof( Sakleth ),
					typeof( SaklethArcher ),
					typeof( SaklethMage ),
					typeof( Reptalar ),
					typeof( ReptalarShaman ),
					typeof( ReptalarChieftain ),
					typeof( SandGiant ),
					typeof( Satan ),
					typeof( Satyr ),
					typeof( Savage ),
					typeof( SavageRidgeback ),
					typeof( Scorpion ),
					typeof( SeaDragon ),
					typeof( SeaDrake ),
					typeof( SeaGhost ),
					typeof( SeaGiant ),
					typeof( SeaHag ),
					typeof( SeaHagGreater ),
					typeof( SeaSerpent ),
					typeof( SeaSnake ),
					typeof( SeaTroll ),
					typeof( SewageElemental ),
					typeof( Shade ),
					typeof( ShadowDemon ),
					typeof( ShadowFiend ),
					typeof( ShadowHound ),
					typeof( ShadowIronElemental ),
					typeof( ShadowWyrm ),
					typeof( TitanStratos ),
					typeof( ShamanicCyclops ),
					typeof( Sheep ),
					typeof( SilverElemental ),
					typeof( SilverSteed ),
					typeof( SkeletalDragon ),
					typeof( SkeletalKnight ),
					typeof( SkeletalMage ),
					typeof( SkeletalMount ),
					typeof( SkeletalPirate ),
					typeof( SkeletalSamurai ),
					typeof( SkeletalWarrior ),
					typeof( SkeletalWizard ),
					typeof( Sleestax ),
					typeof( SlimeDevil ),
					typeof( Snake ),
					typeof( SnowHarpy ),
					typeof( SnowLeopard ),
					typeof( SoulReaper ),
					typeof( SoulSucker ),
					typeof( SoulWorm ),
					typeof( SpectralGargoyle ),
					typeof( Spectre ),
					typeof( Bodak ),
					typeof( SphinxRiding ),
					typeof( SpinelElemental ),
					typeof( Spirit ),
					typeof( Squirrel ),
					typeof( StarRubyElemental ),
					typeof( SapphireElemental ), 
					typeof( XormiteElemental ),
					typeof( DilithiumElemental ), 
					typeof( TrilithiumElemental ), 
					typeof( Brontosaur ),
					typeof( Stegosaurus ),
					typeof( Styguana ),
					typeof( StoneElemental ),
					typeof( StoneGargoyle ),
					typeof( GargoyleWarrior ), 
					typeof( StoneGiant ),
					typeof( TitanLithos ), 
					typeof( StoneHarpy ),
					typeof( StoneRoper ),
					typeof( StormGiant ),
					typeof( CloudGiant ), 
					typeof( StarGiant ), 
					typeof( StrangleVine ),
					typeof( Succubus ),
					typeof( SummonedDaemon ),
					typeof( SummonedEarthElemental ),
					typeof( SummonedFireElemental ),
					typeof( SummonedWaterElemental ),
					typeof( SummonedDaemonGreater ),
					typeof( SummonedEarthElementalGreater ),
					typeof( SummonedFireElementalGreater ),
					typeof( SummonedWaterElementalGreater ),
					typeof( Surtaz ),
					typeof( SwampDragon ),
					typeof( SwampDrakeRiding ),
					typeof( SwampTentacle ),
					typeof( SwampThing ),
					typeof( SwampTroll ),
					typeof( TerathanAvenger ),
					typeof( TerathanDrone ),
					typeof( TerathanMatriarch ),
					typeof( TerathanWarrior ),
					typeof( TimberWolf ),
					typeof( Titan ),
					typeof( Titanoboa ),
					typeof( TopazElemental ),
					typeof( ToxicElemental ),
					typeof( Tritun ),
					typeof( TritunMage ),
					typeof( Troll ),
					typeof( FrostTrollShaman ),
					typeof( TrollWitchDoctor ),
					typeof( TropicalBird ),
					typeof( SwampBird ),
					typeof( DesertBird ),
					typeof( GuardianWolf ),
					typeof( UmberHulk ),
					typeof( UndeadDruid ),
					typeof( Unicorn ),
					typeof( Pegasus ),
					typeof( Dreadhorn ),
					typeof( Urc ),
					typeof( UrcBowman ),
					typeof( UrcShaman ),
					typeof( Urk ),
					typeof( UrkShaman ),
					typeof( ValoriteElemental ),
					typeof( VeriteElemental ),
					typeof( VoidDragon ),
					typeof( VolcanicDragon ),
					typeof( Vordo ),
					typeof( Vrock ),
					typeof( Vulcrum ),
					typeof( WailingBanshee ),
					typeof( WaterBeetleRiding ),
					typeof( GlowBeetleRiding ),
					typeof( TigerBeetleRiding ),
					typeof( WaterElemental ),
					typeof( ElementalCalledWater ),
					typeof( WaterWeird ),
					typeof( WereWolf ),
					typeof( WhippingVine ),
					typeof( WhiteCat ),
					typeof( WhiteRabbit ),
					typeof( WhiteWolf ),
					typeof( WinterWolf ),
					typeof( Wight ),
					typeof( WolfMan ),
					typeof( Wraith ),
					typeof( Undead ),
					typeof( Wyverns ),
					typeof( AncientWyvern ),
					typeof( Wyvra ),
					typeof( Xatyr ),
					typeof( xDryad ),
					typeof( Yeti ),
					typeof( AbrozChieftain ),
					typeof( AbrozShaman ),
					typeof( AbrozWarrior ),
					typeof( ZombieDragon ),
					typeof( DragonGhost ), 
					typeof( ZornTheBlacksmith ),
					typeof( ZuluuArcher ),
					typeof( ZuluuNative ),
					typeof( ZuluuWitchDoctor )
				} )
			};
	}
}

namespace Server.Targets
{
	public class AIControlMobileTarget : Target
	{
		private List<BaseAI> m_List;
		private OrderType m_Order;

		public OrderType Order {
			get {
				return m_Order;
			}
		}

		public AIControlMobileTarget( BaseAI ai, OrderType order ) : base( -1, false, ( order == OrderType.Attack ? TargetFlags.Harmful : TargetFlags.None ) )
		{
			m_List = new List<BaseAI>();
			m_Order = order;

			AddAI( ai );
		}

		public void AddAI( BaseAI ai )
		{
			if ( !m_List.Contains( ai ) )
				m_List.Add( ai );
		}

		protected override void OnTarget( Mobile from, object o )
		{
			if ( o is Mobile ) {
				Mobile m = (Mobile)o;
				for ( int i = 0; i < m_List.Count; ++i )
					m_List[i].EndPickTarget( from, m, m_Order );
			}
		}
	}
}

namespace Server.Items
{
	public class HeldLight : BaseEquipableLight
	{
		public override int LitItemID{ get { return 0xA22; } }
		public override int UnlitItemID{ get { return 0xA22; } }

		[Constructable]
		public HeldLight() : base( 0xA22 )
		{
			Name = "lantern";
			Duration = TimeSpan.Zero;
			Burning = true;
			Light = LightType.Circle300;
			Weight = 2.0;
			LootType = LootType.Blessed;

			switch ( Utility.Random( 3 ) )
			{
				default:
				case 0: Name = "torch";		ItemID = 0xA12;		Light = LightType.Circle300;	break;
				case 1: Name = "candle";	ItemID = 0xA0F;		Light = LightType.Circle150;	break;
			}
		}

		public override bool DisplayLootType{ get{ return false; } }

		public HeldLight( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}

namespace Server.Misc
{
    class IntelligentAction
    {
		public static bool GetMyEnemies( Mobile m, Mobile me, bool checkDisguise )
		{
			bool enemy = true;
			bool trick = false;

			Region reg = Region.Find( me.Location, me.Map );

			if ( reg.IsPartOf( typeof( NecromancerRegion ) ) && ( GetPlayerInfo.EvilPlayer( m ) || m is BaseCreature ) )
				return false;

			if ( !(me.CanSee( m )) || !(me.InLOS( m )) )
				return false;

			if ( m.AccessLevel > AccessLevel.Player )
				return false;

			if ( m is BasePerson || m is BaseVendor || m is PlayerVendor || m is Citizens || m is PlayerBarkeeper )
				return false;

			if ( m is BaseCreature && ((BaseCreature)m).FightMode == FightMode.Evil )
				return false;

			if ( m.Region.IsPartOf( typeof( PublicRegion ) ) )
				return false;

			if ( m.Region.IsPartOf( typeof( StartRegion ) ) )
				return false;

			if ( m.Region.IsPartOf( typeof( SafeRegion ) ) )
				return false;

			if ( m.Region.IsPartOf( typeof( ProtectedRegion ) ) )
				return false;

			if ( m is PlayerMobile && !m.Criminal && m.Kills < 1 )
				enemy = false;

			if ( checkDisguise || (m.Region).Name == "the Castle of Knowledge" )
			{
				if ( DisguiseTimers.IsDisguised( m ) )
				{
					enemy = false;
					trick = true;
				}
				if( !m.CanBeginAction( typeof( PolymorphSpell ) ) )
				{
					enemy = false;
					trick = true;
				}
				if ( !m.CanBeginAction( typeof( IncognitoSpell ) ) )
				{
					enemy = false;
					trick = true;
				}
				if ( !m.CanBeginAction( typeof( Deception ) ) )
				{
					enemy = false;
					trick = true;
				}
			}

			if ( (m.Region).Name != "the Castle of Knowledge" && !trick && m is PlayerMobile && m.Karma <= -5000 && m.Skills[SkillName.Knightship].Base >= 50 && !m.Region.IsPartOf(typeof(UmbraRegion)) && !m.Region.IsPartOf(typeof(NecromancerRegion)) )
				enemy = true; // DEATH KNIGHTS ARE NOT WELCOME AFTER THIS POINT...EXCEPT IN UMBRA OR RAVENDARK

			if ( (m.Region).Name != "the Castle of Knowledge" && !trick && m is PlayerMobile && m.Karma <= -5000 && m.Skills[SkillName.Psychology].Base >= 50 && Server.Misc.GetPlayerInfo.isSyth(m, false) && !m.Region.IsPartOf(typeof(UmbraRegion)) && !m.Region.IsPartOf(typeof(NecromancerRegion)) )
				enemy = true; // SYTH ARE NOT WELCOME AFTER THIS POINT...EXCEPT IN UMBRA OR RAVENDARK

			if ( (m.Region).Name != "the Castle of Knowledge" && !trick && m is PlayerMobile && m.Karma < 2500 && m.Fame < 2500 && Server.Items.BaseRace.IsEvil( m ) && !m.Region.IsPartOf(typeof(UmbraRegion)) && !m.Region.IsPartOf(typeof(NecromancerRegion)) && !m.Region.IsPartOf(typeof(GargoyleRegion)) )
				enemy = true; // PLAYER CREATURES THAT ARE EVIL...EXCEPT IN UMBRA OR RAVENDARK

			if (m is BaseCreature)
			{
				BaseCreature c = (BaseCreature)m;
				if ( c.Controlled || c.FightMode == FightMode.Aggressor || c.FightMode == FightMode.None )
					enemy = false;
			}	

			return enemy;
		}

		public static int GetCreatureLevel( Mobile m )
		{
			int fame = m.Fame;
				if ( fame > 15000){ fame = 15000; }

			int karma = m.Karma;
				if ( karma < 0 ){ karma = m.Karma * -1; }
				if ( karma > 15000){ karma = 15000; }

			int skills = m.Skills.Total;
				if ( skills > 10000){ skills = 10000; }
				skills = (int)( 1.5 * skills );			// UP TO 15,000

			int stats = m.RawStr + m.RawDex + m.RawInt;
				if ( stats > 250){ stats = 250; }
				stats = 60 * stats;						// UP TO 15,000

			int level = (int)( ( fame + karma + skills + stats ) / 600 );
				level = (int)( ( level - 10 ) * 1.12 );

			if ( level < 1 ){ level = 1; }
			if ( level > 125 ){ level = 125; }

			return level;
		}

		public static bool IsDrow( Mobile m )
		{
			SlayerEntry drow = SlayerGroup.GetEntryByName( SlayerName.Fey );
			if ( drow.Slays(m) )
				return true;

			return false;
		}

		public static bool IsOrk( Mobile m )
		{
			SlayerEntry orc = SlayerGroup.GetEntryByName( SlayerName.OrcSlaying );
			if ( orc.Slays(m) )
				return true;

			return false;
		}

		public static void GiveTorch( Mobile m )
		{
			if ( m.FindItemOnLayer( Layer.TwoHanded ) == null )
			{
				if ( m.Region.IsPartOf( typeof( BardDungeonRegion ) ) || m.Region.IsPartOf( typeof( DungeonRegion ) ) || m.Region.IsPartOf( typeof( CaveRegion ) ) )
				{
					if ( Utility.RandomMinMax( 1, 3 ) != 1 )
					{
						m.AddItem( new HeldLight() );
					}
				}
			}
		}

		public static bool FameBasedEvent( Mobile m )
		{
			int events = (int)( m.Fame / 250 );

			if ( events <= 0 )
				return false;

			if ( events > 80 )
				events = 80;

			if ( events >= Utility.RandomMinMax( 1, 100 ) )
				return true;

			return false;
		}

		public static int FameBasedLevel( Mobile m )
		{
			int level = (int)( m.Fame / 3500 );

			if ( level <= 0 )
				return 1;

			if ( level > 6 )
				level = 6;

			return level;
		}

		public static void DoSpecialAbility( Mobile from, Mobile target )
		{
			if ( from == null || from.Deleted ) //sanity
				return;

			if ( Utility.RandomMinMax( 1, 20 ) == 1 && from.EmoteHue > 0 )
			{
				Map map = from.Map;

				if ( map == null )
					return;

				int monsters = 0;

				foreach ( Mobile m in from.GetMobilesInRange( 10 ) )
				{
					if ( from.EmoteHue == 1 )
					{
						if ( m is EvilBladeSpirits || m is Imp || m is Slime )
							++monsters;
					}
					else if ( from.EmoteHue == 2 )
					{
						if ( m is BloodWorm || m is BloodSnake || m is Viscera || m is BloodSpawn || m is GiantLeech )
							++monsters;
					}
					else if ( from.EmoteHue == 3 )
					{
						if ( m is LesserDemon || m is Imp || m is ShadowHound || m is Gargoyle || m is SoulWorm )
							++monsters;

						if ( m is LowerDemon )
						{
							++monsters;
							++monsters;
						}
					}
					else if ( from.EmoteHue == 4 )
					{
						if ( m is GarnetElemental || m is TopazElemental || m is QuartzElemental || m is SpinelElemental || m is StarRubyElemental || m is EarthElemental || m is AgapiteElemental || m is BronzeElemental || m is CopperElemental || m is DullCopperElemental || m is GoldenElemental || m is ShadowIronElemental || m is ValoriteElemental || m is VeriteElemental || m is WaterElemental )
							++monsters;

						if ( m is PoisonElemental || m is ToxicElemental || m is AirElemental || m is BloodElemental || m is FireElemental || m is ElectricalElemental )
						{
							++monsters;
							++monsters;
						}
					}
					else if ( from.EmoteHue == 5 )
					{
						if ( m is Bodak || m is BoneKnight || m is BoneMagi || m is Ghoul || m is Mummy || m is Shade || m is SkeletalKnight || m is SkeletalMage || m is Skeleton || m is Spectre || m is Wraith || m is Phantom || m is Zombie )
							++monsters;
					}
					else if ( from.EmoteHue == 6 )
					{
						if ( m is WeedElemental || m is WolfDire || m is DireBear || m is DireBoar )
							++monsters;
					}
					else if ( from.EmoteHue == 7 )
					{
						if ( m is SnowElemental || m is IceSerpent || m is WinterWolf || m is IceElemental || m is FrostOoze || m is FrostSpider || m is IceGolem || m is IceToad || m is IceSerpent )
							++monsters;
					}
					else if ( from.EmoteHue == 8 )
					{
						if ( m is FireDemon || m is LavaPuddle || m is CinderElemental || m is FireBat || m is FireElemental || m is FireMephit )
							++monsters;
					}
					else if ( from.EmoteHue == 9 )
					{
						if ( m is GiantSerpent || m is GiantAdder || m is JungleViper || m is LargeSnake || m is Snake )
							++monsters;

						if ( m is SilverSerpent )
						{
							++monsters;
							++monsters;
						}
					}
					else if ( from.EmoteHue == 10 )
					{
						if ( m is WaterWeird || m is Typhoon || m is WaterElemental || m is StormCloud || m is WaterSpawn )
							++monsters;
					}
					else if ( from.EmoteHue == 11 || from.EmoteHue == 505 )
					{
						if ( m is EvilIcyVortex || m is EvilPlagueVortex || m is EvilEnergyVortex || m is EvilBladeSpirits || m is EvilScorchingVortex )
							++monsters;
					}
					else if ( from.EmoteHue == 12 )
					{
						if ( m is WineElemental || m is ManureGolem || m is Fairy )
							++monsters;
					}
					else if ( from.EmoteHue == 13 )
					{
						if ( m is GhostWarrior || m is WalkingCorpse || m is Wight || m is Spirit || m is Phantom || m is FrailSkeleton || m is Zombie || m is Skeleton || m is SkeletalKnight || m is BoneKnight || m is SkeletalWarrior )
							++monsters;
					}
					else if ( from.EmoteHue == 14 )
					{
						if ( m is DeathBear || m is DeathWolf || m is DarkReaper )
							++monsters;
					}
					else if ( from.EmoteHue == 15 )
					{
						if ( m is Bat || m is Ghoul || m is Wraith || m is WalkingCorpse || m is VampireBat || m is Zombie )
							++monsters;
					}
					else if ( from.EmoteHue == 16 )
					{
						if ( m is EvilIcyVortex || m is IceBladeSpirits )
							++monsters;
					}
					else if ( from.EmoteHue == 17 )
					{
						if ( m is Scorpion || m is SandVortex || m is SandSpider || m is DustElemental || m is GiantAdder )
							++monsters;
					}
				}

				if ( monsters < 3 && from.Mana > 39 )
				{
					BaseCreature monster = new Imp();

					from.PlaySound( 0x216 );

					from.Mana = from.Mana - 40;

					int newmonsters = Utility.RandomMinMax( 1, 3 );

					for ( int i = 0; i < newmonsters; ++i )
					{
						if ( from.EmoteHue == 1 )
						{
							switch ( Utility.Random( 5 ) )
							{
								default:
								case 0: monster = new EvilBladeSpirits(); break;
								case 1: monster = new Imp(); break;
								case 2: monster = new Imp(); break;
								case 3: monster = new Slime(); break;
								case 4: monster = new Slime(); break;
							}
						}
						else if ( from.EmoteHue == 2 )
						{
							switch ( Utility.Random( 5 ) )
							{
								default:
								case 0: monster = new BloodWorm(); break;
								case 1: monster = new BloodSnake(); break;
								case 2: monster = new Viscera(); break;
								case 3: monster = new BloodSpawn(); break;
								case 4: monster = new GiantLeech(); break;
							}
						}
						else if ( from.EmoteHue == 3 )
						{
							switch ( Utility.Random( 11 ) )
							{
								default:
								case 0: monster = new LesserDemon(); break;
								case 1: monster = new LowerDemon(); break;
								case 2: monster = new LesserDemon(); break;
								case 3: monster = new LowerDemon(); break;
								case 4: monster = new Imp(); break;
								case 5: monster = new ShadowHound(); break;
								case 7: monster = new Gargoyle(); break;
								case 9: monster = new SoulWorm(); break;
							}
						}
						else if ( from.EmoteHue == 4 )
						{
							switch ( Utility.Random( 31 ) )
							{
								default:
								case 0: monster = new GarnetElemental(); break;
								case 1: monster = new TopazElemental(); break;
								case 2: monster = new QuartzElemental(); break;
								case 3: monster = new SpinelElemental(); break;
								case 4: monster = new StarRubyElemental(); break;
								case 5: monster = new GarnetElemental(); break;
								case 6: monster = new TopazElemental(); break;
								case 7: monster = new QuartzElemental(); break;
								case 8: monster = new SpinelElemental(); break;
								case 9: monster = new StarRubyElemental(); break;
								case 10: monster = new GarnetElemental(); break;
								case 11: monster = new TopazElemental(); break;
								case 12: monster = new QuartzElemental(); break;
								case 13: monster = new SpinelElemental(); break;
								case 14: monster = new StarRubyElemental(); break;
								case 15: monster = new EarthElemental(); break;
								case 16: monster = new AgapiteElemental(); break;
								case 17: monster = new BronzeElemental(); break;
								case 18: monster = new CopperElemental(); break;
								case 19: monster = new DullCopperElemental(); break;
								case 20: monster = new GoldenElemental(); break;
								case 21: monster = new ShadowIronElemental(); break;
								case 22: monster = new ValoriteElemental(); break;
								case 23: monster = new VeriteElemental(); break;
								case 24: monster = new PoisonElemental(); break;
								case 25: monster = new ToxicElemental(); break;
								case 26: monster = new WaterElemental(); break;
								case 27: monster = new AirElemental(); break;
								case 28: monster = new BloodElemental(); break;
								case 29: monster = new FireElemental(); break;
								case 30: monster = new ElectricalElemental(); break;
							}
						}
						else if ( from.EmoteHue == 5 )
						{
							switch ( Utility.Random( 14 ) )
							{
								default:
								case 0: monster = new BoneKnight(); break;
								case 1: monster = new BoneMagi(); break;
								case 2: monster = new Ghoul(); break;
								case 3: monster = new Ghostly(); break;
								case 4: monster = new Mummy(); break;
								case 5: monster = new Shade(); break;
								case 6: monster = new SkeletalKnight(); break;
								case 7: monster = new SkeletalMage(); break;
								case 8: monster = new Skeleton(); break;
								case 9: monster = new Spectre(); break;
								case 10: monster = new Wraith(); break;
								case 11: monster = new Phantom(); break;
								case 12: monster = new Zombie(); break;
								case 13: monster = new Bodak(); break;
							}
						}
						else if ( from.EmoteHue == 6 )
						{
							switch ( Utility.Random( 4 ) )
							{
								default:
								case 0: monster = new DireBear(); break;
								case 1: monster = new DireBoar(); break;
								case 2: monster = new WolfDire(); break;
								case 3: monster = new WeedElemental(); break;
							}
						}
						else if ( from.EmoteHue == 7 )
						{
							switch ( Utility.Random( 9 ) )
							{
								default:
								case 0: monster = new SnowElemental(); break;
								case 1: monster = new IceSerpent(); break;
								case 2: monster = new IceElemental(); break;
								case 3: monster = new FrostOoze(); break;
								case 4: monster = new FrostSpider(); break;
								case 5: monster = new IceGolem(); break;
								case 6: monster = new IceToad(); break;
								case 7: monster = new IceSerpent(); break;
								case 8: monster = new WinterWolf(); break;
							}
						}
						else if ( from.EmoteHue == 8 )
						{
							switch ( Utility.Random( 9 ) )
							{
								default:
								case 0: monster = new FireDemon(); break;
								case 1: monster = new LavaPuddle(); break;
								case 2: monster = new CinderElemental(); break;
								case 3: monster = new FireElemental(); break;
								case 4: monster = new FireBat(); break;
								case 5: monster = new FireBat(); break;
								case 6: monster = new LavaPuddle(); break;
								case 7: monster = new FireMephit(); break;
								case 8: monster = new FireMephit(); break;
							}
						}
						else if ( from.EmoteHue == 9 )
						{
							switch ( Utility.Random( 11 ) )
							{
								default:
								case 0: monster = new GiantSerpent(); break;
								case 1: monster = new GiantAdder(); break;
								case 2: monster = new JungleViper(); break;
								case 3: monster = new LargeSnake(); break;
								case 4: monster = new Snake(); break;
								case 5: monster = new GiantSerpent(); break;
								case 6: monster = new GiantAdder(); break;
								case 7: monster = new JungleViper(); break;
								case 8: monster = new LargeSnake(); break;
								case 9: monster = new Snake(); break;
								case 10: monster = new SilverSerpent(); break;
							}
						}
						else if ( from.EmoteHue == 10 )
						{
							switch ( Utility.Random( 5 ) )
							{
								default:
								case 0: monster = new WaterWeird(); break;
								case 1: monster = new Typhoon(); break;
								case 2: monster = new WaterElemental(); break;
								case 3: monster = new StormCloud(); break;
								case 4: monster = new WaterSpawn(); break;
							}
						}
						else if ( from.EmoteHue == 11 || from.EmoteHue == 505 )
						{
							switch ( Utility.Random( 5 ) )
							{
								default:
								case 0: monster = new EvilIcyVortex(); break;
								case 1: monster = new EvilPlagueVortex(); break;
								case 2: monster = new EvilEnergyVortex(); break;
								case 3: monster = new EvilScorchingVortex(); break;
								case 4: monster = new EvilBladeSpirits(); break;
							}
						}
						else if ( from.EmoteHue == 12 )
						{
							switch ( Utility.Random( 3 ) )
							{
								default:
								case 0: monster = new WineElemental(); break;
								case 1: monster = new ManureGolem(); break;
								case 2: monster = new Fairy(); break;
							}
						}
						else if ( from.EmoteHue == 13 )
						{
							int MaxMonster = 2;
							if ( from.Fame >= 23000 ){		MaxMonster = 10; }
							else if ( from.Fame >= 12000 ){	MaxMonster = 6; }

							switch ( Utility.RandomMinMax( 0, MaxMonster ) )
							{
								default:
								case 0: monster = new FrailSkeleton(); break;
								case 1: monster = new Phantom(); break;
								case 2: monster = new Skeleton(); break;
								case 3: monster = new Zombie(); break;
								case 4: monster = new GhostWarrior(); break;
								case 5: monster = new Wight(); break;
								case 6: monster = new SkeletalWarrior(); break;
								case 7: monster = new WalkingCorpse(); break;
								case 8: monster = new SkeletalKnight(); break;
								case 9: monster = new BoneKnight(); break;
								case 10: monster = new Spirit(); break;
							}
						}
						else if ( from.EmoteHue == 14 )
						{
							switch ( Utility.Random( 5 ) )
							{
								default:
								case 0: case 1:	monster = new DeathWolf(); break;
								case 2: case 3:	monster = new DeathBear(); break;
								case 4:			monster = new DarkReaper(); break;
							}
						}
						else if ( from.EmoteHue == 15 )
						{
							int MaxMonster = 1;
							if ( from.Fame >= 24000 ){		MaxMonster = 5; }
							else if ( from.Fame >= 10500 ){	MaxMonster = 3; }

							switch ( Utility.RandomMinMax( 0, MaxMonster ) )
							{
								default:
								case 0: monster = new Bat(); break;
								case 1: monster = new Zombie(); break;
								case 2: monster = new Wraith(); break;
								case 3: monster = new Ghoul(); break;
								case 4: monster = new VampireBat(); break;
								case 5: monster = new WalkingCorpse(); break;
							}
						}
						else if ( from.EmoteHue == 16 )
						{
							switch ( Utility.Random( 2 ) )
							{
								default:
								case 0: monster = new EvilIcyVortex(); break;
								case 1: monster = new IceBladeSpirits(); break;
							}
						}
						else if ( from.EmoteHue == 17 )
						{
							switch ( Utility.Random( 7 ) )
							{
								default:
								case 0: monster = new Scorpion(); break;
								case 1: monster = new SandVortex(); break;
								case 2: monster = new SandVortex(); break;
								case 3: monster = new DustElemental(); break;
								case 4: monster = new DustElemental(); break;
								case 5: monster = new SandSpider(); break;
								case 6: monster = new GiantAdder(); break;
							}
						}

						((BaseCreature)monster).Team = ((BaseCreature)from).Team;

						bool validLocation = false;
						Point3D loc = from.Location;

						for ( int j = 0; !validLocation && j < 10; ++j )
						{
							int x = from.X + Utility.Random( 3 ) - 1;
							int y = from.Y + Utility.Random( 3 ) - 1;
							int z = map.GetAverageZ( x, y );

							if ( validLocation = map.CanFit( x, y, from.Z, 16, false, false ) )
								loc = new Point3D( x, y, from.Z );
							else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
								loc = new Point3D( x, y, z );
						}

						monster.IsTempEnemy = true;
						monster.YellHue = from.Serial;
						monster.MoveToWorld( loc, map );
						monster.Combatant = target;
						Effects.SendLocationParticles( EffectItem.Create( monster.Location, monster.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
						monster.PlaySound( 0x1FE );
						OnCreatureSpawned( monster );
					}

					from.Say( "" + NameList.RandomName( "magic words" ) + " " + NameList.RandomName( "magic words" ) + " " + NameList.RandomName( "magic words" ) + "!" );
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void OnCreatureSpawned( Mobile summoned )
		{
			if ( summoned.Backpack != null )
			{
				List<Item> belongings = new List<Item>();
				foreach( Item i in summoned.Backpack.Items )
				{
					belongings.Add(i);
				}
				foreach ( Item stuff in belongings )
				{
					stuff.Delete();
				}
			}

			((BaseCreature)summoned).NameColor();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void GiveBasicWepShld( Mobile m )
		{
			int CanHaveShield = 1;

			Item weapon = new BattleAxe(); weapon.Delete();

			switch ( Utility.Random( 34 ) )
			{
				case 0: weapon = new BattleAxe(); CanHaveShield = 0; break;
				case 1: weapon = new VikingSword(); break;
				case 2: weapon = new Halberd(); CanHaveShield = 0; break;
				case 3: weapon = new DoubleAxe(); break;
				case 4: weapon = new ExecutionersAxe(); CanHaveShield = 0; break;
				case 5: weapon = new WarAxe(); break;
				case 6: weapon = new TwoHandedAxe(); CanHaveShield = 0; break;
				case 7: weapon = new Cutlass(); break;
				case 8: weapon = new Katana(); break;
				case 9: weapon = new Kryss(); break;
				case 10: weapon = new Broadsword(); break;
				case 11: weapon = new Longsword(); break;
				case 12: weapon = new ThinLongsword(); break;
				case 13: weapon = new Scimitar(); break;
				case 14: weapon = new BoneHarvester(); break;
				case 15: weapon = new CrescentBlade(); CanHaveShield = 0; break;
				case 16: weapon = new DoubleBladedStaff(); CanHaveShield = 0; break;
				case 17: weapon = new Pike(); CanHaveShield = 0; break;
				case 18: weapon = new Scythe(); CanHaveShield = 0; break;
				case 19: weapon = new Pitchfork(); CanHaveShield = 0; break;
				case 20: weapon = new ShortSpear(); CanHaveShield = 0; break;
				case 21: weapon = new Spear(); CanHaveShield = 0; break;
				case 22: weapon = new Club(); break;
				case 23: weapon = new HammerPick(); break;
				case 24: weapon = new Mace(); break;
				case 25: weapon = new Maul(); break;
				case 26: weapon = new WarHammer(); CanHaveShield = 0; break;
				case 27: weapon = new WarMace(); break;
				case 28: weapon = new Hammers(); break;
				case 29: weapon = new SpikedClub(); break;
				case 30: weapon = new Claymore(); CanHaveShield = 0; break;
				case 31: weapon = new Pitchforks(); CanHaveShield = 0; break;
				case 32: weapon = new ShortSword(); break;
				case 33: weapon = new Whips(); break;
			}

			m.AddItem( weapon );

			Item armor = new BronzeShield(); armor.Delete();

			if ( CanHaveShield == 1 && Utility.RandomMinMax( 1, 3 ) == 1 )
			{
				switch ( Utility.Random( 16 ) )
				{
					case 0: armor = new BronzeShield(); break;
					case 1: armor = new Buckler(); break;
					case 2: armor = new MetalKiteShield(); break;
					case 3: armor = new HeaterShield(); break;
					case 4: armor = new WoodenKiteShield(); break;
					case 5: armor = new MetalShield(); break;
					case 6: armor = new GuardsmanShield(); break;
					case 7: armor = new ElvenShield(); break;
					case 8: armor = new DarkShield(); break;
					case 9: armor = new CrestedShield(); break;
					case 10: armor = new ChampionShield(); break;
					case 11: armor = new JeweledShield(); break;
					case 12: armor = new ChaosShield(); break;
					case 13: armor = new OrderShield(); break;
					case 14: armor = new SunShield(); break;
					case 15: armor = new VirtueShield(); break;
				}
			}

			m.AddItem( armor );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void DressUpMerchants( Mobile from )
		{
			Server.Misc.MorphingTime.RemoveMyClothes( from );

			int pantsHue = Utility.RandomColor( Utility.RandomMinMax( 0, 12 ) );
			int legsHue = Utility.RandomList( 0, 0, Utility.RandomNeutralHue() );
			int shirtHue = Utility.RandomColor( Utility.RandomMinMax( 0, 12 ) );
			int robeHue = Utility.RandomColor( Utility.RandomMinMax( 0, 12 ) );
			int hatHue = Utility.RandomColor( Utility.RandomMinMax( 0, 12 ) );
			int cloakHue = Utility.RandomColor( Utility.RandomMinMax( 0, 12 ) );
			int robeType = Utility.RandomMinMax( 1, 14 );
			bool hasRobe = false;
			bool wizard = false;
			bool apron = false;
			bool player = false;
			bool gown = false;
			bool cloak = false;

			if ( from is Priest ){ 						hasRobe = true;		robeHue = 0x8D7;	}
			else if ( from is AssassinGuildmaster ){ 	hasRobe = true;		robeHue = 2411; 		robeType = 15;	}
			else if ( from is DruidGuildmaster ){ 		hasRobe = true;		robeHue = Utility.RandomGreenHue();	}
			else if ( from is Druid ){ 					hasRobe = true;		robeHue = Utility.RandomGreenHue();	}
			else if ( from is HealerGuildmaster ){ 		hasRobe = true;	}
			else if ( from is Healer ){ 				hasRobe = true;	}
			else if ( from is MageGuildmaster ){ 		hasRobe = true;		wizard = true;	}
			else if ( from is Mage ){ 					hasRobe = true;		wizard = true;	}
			else if ( from is Elementalist ){ 			hasRobe = true;		wizard = true;	robeHue = Server.Spells.Elementalism.ElementalSpell.ElementalHue( "any" ); }
			else if ( from is ElementalGuildmaster ){	hasRobe = true;		wizard = true;	robeHue = Server.Spells.Elementalism.ElementalSpell.ElementalHue( "any" ); }
			else if ( from is NecromancerGuildmaster ){ hasRobe = true;		robeHue = MorphingTime.GetRandomNecromancerHue();	wizard = true;	}
			else if ( from is Witches ){ 				hasRobe = true;		robeHue = MorphingTime.GetRandomNecromancerHue();	wizard = true;	}
			else if ( from is Undertaker ){ 			hasRobe = true;		robeHue = MorphingTime.GetRandomNecromancerHue();	wizard = true;	}
			else if ( from is Necromancer ){			hasRobe = true;		robeHue = MorphingTime.GetRandomNecromancerHue();	wizard = true;	}
			else if ( from is EvilHealer ){ 			hasRobe = true;		robeHue = MorphingTime.GetRandomNecromancerHue();	wizard = true;	}
			else if ( from is WanderingHealer ){ 		hasRobe = true;		wizard = true;	}
			else if ( from is WonderousDealer ){ 		hasRobe = true;		wizard = true;	}
			else if ( from is PowerDealer ){ 			hasRobe = true;		wizard = true;	}
			else if ( from is MythicalDealer ){ 		hasRobe = true;		wizard = true;	}
			else if ( from is LegendaryDealer ){ 		hasRobe = true;		wizard = true;	}
			else if ( from is ExaltedDealer ){ 			hasRobe = true;		wizard = true;	}
			else if ( from is Enchanter ){ 				hasRobe = true;		wizard = true;	}
			else if ( from is Sage ){ 					hasRobe = true;		wizard = true;	}
			else if ( from is Ranger ){ 				shirtHue = Utility.RandomGreenHue();	robeHue = Utility.RandomGreenHue(); }
			else if ( from is BlacksmithGuildmaster ){ 	apron = true; }
			else if ( from is Weaponsmith ){ 			apron = true; }
			else if ( from is IronWorker ){ 			apron = true; }
			else if ( from is Waiter ){ 				apron = true; }
			else if ( from is TavernKeeper ){ 			apron = true; }
			else if ( from is Blacksmith ){ 			apron = true; }
			else if ( from is CarpenterGuildmaster ){ 	apron = true; }
			else if ( from is Armorer ){ 				apron = true; }
			else if ( from is Barkeeper ){ 				apron = true; }
			else if ( from is Carpenter ){ 				apron = true; }
			else if ( from is CulinaryGuildmaster ){ 	apron = true; }
			else if ( from is Butcher ){ 				apron = true; }
			else if ( from is PlayerMobile ){ 			player = true; }

			if ( ( Utility.RandomMinMax( 1, 20 ) == 1 || hasRobe ) && !apron )
			{
				hasRobe = true;
				if ( ( from.Body == 0x191 || from.Body == 606 ) && Utility.RandomBool() && !player )
				{
					gown = true;
					switch ( Utility.RandomMinMax( 1, 3 ) )
					{
						case 1: from.AddItem( new PlainDress( robeHue ) ); break;
						case 2: from.AddItem( new GildedDress( robeHue ) ); break;
						case 3: from.AddItem( new FancyDress( robeHue ) ); break;
					}
				}
				else
				{
					switch ( robeType )
					{
						case 1: from.AddItem( new FancyRobe( robeHue ) ); break;
						case 2: from.AddItem( new GildedRobe( robeHue ) ); break;
						case 3: from.AddItem( new OrnateRobe( robeHue ) ); break;
						case 4: from.AddItem( new MagistrateRobe( robeHue ) ); break;
						case 5: from.AddItem( new RoyalRobe( robeHue ) ); break;
						case 6: from.AddItem( new ExquisiteRobe( robeHue ) ); break;
						case 7: from.AddItem( new ProphetRobe( robeHue ) ); break;
						case 8: from.AddItem( new ElegantRobe( robeHue ) ); break;
						case 9: from.AddItem( new FormalRobe( robeHue ) ); break;
						case 10: from.AddItem( new ArchmageRobe( robeHue ) ); break;
						case 11: from.AddItem( new PriestRobe( robeHue ) ); break;
						case 12: from.AddItem( new CultistRobe( robeHue ) ); break;
						case 13: from.AddItem( new SageRobe( robeHue ) ); break;
						case 14: from.AddItem( new ScholarRobe( robeHue ) ); break;
						case 15: from.AddItem( new AssassinRobe( robeHue ) ); break;
					}
				}
			}

			if ( !gown )
			{
				switch ( Utility.RandomMinMax( 1, 7 ) )
				{
					case 1: from.AddItem( new FancyShirt( shirtHue ) ); break;
					case 2: from.AddItem( new RoyalCoat( shirtHue ) ); break;
					case 3: from.AddItem( new RoyalShirt( shirtHue ) ); break;
					case 4: from.AddItem( new SquireShirt( shirtHue ) ); break;
					case 5: from.AddItem( new FormalCoat( shirtHue ) ); break;
					case 6: from.AddItem( new WizardShirt( shirtHue ) ); break;
					case 7: from.AddItem( new Shirt( shirtHue ) ); break;
				}

				if ( ( from.Body == 0x191 || from.Body == 606 ) && Utility.RandomBool() )
				{
					switch ( Utility.RandomMinMax( 1, 3 ) )
					{
						case 1: from.AddItem( new RoyalSkirt( pantsHue ) ); break;
						case 2: from.AddItem( new RoyalLongSkirt( pantsHue ) ); break;
						case 3: from.AddItem( new Skirt( pantsHue ) ); break;
					}
				}
				else
				{
					switch ( Utility.RandomMinMax( 1, 9 ) )
					{
						case 1: from.AddItem( new LongPants( legsHue ) ); break;
						case 2: from.AddItem( new SailorPants( legsHue ) ); break;
						case 3: from.AddItem( new PiratePants( legsHue ) ); break;
						case 4: from.AddItem( new ShortPants( legsHue ) ); break;
						case 5: from.AddItem( new LongPants( legsHue ) ); break;
						case 6: from.AddItem( new SailorPants( legsHue ) ); break;
						case 7: from.AddItem( new PiratePants( legsHue ) ); break;
						case 8: from.AddItem( new Kilt( pantsHue ) ); break;
						case 9: from.AddItem( new ShortPants( legsHue ) ); break;
					}
				}
			}

			switch ( Utility.RandomMinMax( 1, 10 ) )
			{
				case 1: from.AddItem( new Boots( Utility.RandomNeutralHue() ) ); break;
				case 2: from.AddItem( new BarbarianBoots( Utility.RandomNeutralHue() ) ); break;
				case 3: from.AddItem( new Boots( Utility.RandomNeutralHue() ) ); break;
				case 4: from.AddItem( new ThighBoots( Utility.RandomNeutralHue() ) ); break;
				case 5: from.AddItem( new Shoes( Utility.RandomNeutralHue() ) ); break;
				case 6: from.AddItem( new Sandals( Utility.RandomNeutralHue() ) ); break;
				case 7: from.AddItem( new ElvenBoots( Utility.RandomNeutralHue() ) ); break;
				case 8: from.AddItem( new Boots( Utility.RandomNeutralHue() ) ); break;
				case 9: from.AddItem( new Shoes( Utility.RandomNeutralHue() ) ); break;
				case 10: from.AddItem( new ElvenBoots( Utility.RandomNeutralHue() ) ); break;
			}

			if ( wizard )
			{
				if ( Utility.RandomBool() )
				{ 
					int myHat = Utility.RandomMinMax( 0, 4 );
					if ( from.Body == 605 ){ myHat = 1; }
					switch ( myHat )
					{
						case 0: from.AddItem( new ClothCowl( robeHue ) ); break;
						case 1: from.AddItem( new ClothHood( robeHue ) ); break;
						case 2: from.AddItem( new FancyHood( robeHue ) ); break;
						case 3: from.AddItem( new WizardHood( robeHue ) ); break;
						case 4: from.AddItem( new HoodedMantle( robeHue ) ); break;
					}
				}
				else
				{
					if ( ( from.Body == 0x191 || from.Body == 606 ) && Utility.RandomBool() )
					{
						from.AddItem( new WitchHat( robeHue ) );
					}
					else
					{
						from.AddItem( new WizardsHat( robeHue ) );
					}
				}
			}

			if ( player && Utility.RandomBool() )
			{
				from.AddItem( new Cloak( cloakHue ) );
				cloak = true;
			}

			if ( player && Utility.RandomBool() )
			{
				int cowlHue = hatHue;
				if ( hasRobe ){ cowlHue = robeHue; }
				else if ( !hasRobe && cloak ){ cowlHue = cloakHue; }

				switch ( Utility.RandomMinMax( 0, 29 ) )
				{
					case 0: from.AddItem( new ClothCowl( cowlHue ) ); break;
					case 1: from.AddItem( new ClothHood( cowlHue ) ); break;
					case 2: from.AddItem( new FancyHood( cowlHue ) ); break;
					case 3: from.AddItem( new WizardHood( cowlHue ) ); break;
					case 4: from.AddItem( new HoodedMantle( cowlHue ) ); break;
					case 5: from.AddItem( new FloppyHat( hatHue ) ); break;
					case 6: from.AddItem( new WideBrimHat( hatHue ) ); break;
					case 7: from.AddItem( new Cap( hatHue ) ); break;
					case 8: from.AddItem( new SkullCap( hatHue ) ); break;
					case 9: from.AddItem( new Bandana( hatHue ) ); break;
					case 10: from.AddItem( new TallStrawHat() ); break;
					case 11: from.AddItem( new StrawHat() ); break;
					case 12: from.AddItem( new WizardsHat( cowlHue ) ); break;
					case 13: from.AddItem( new WitchHat( cowlHue ) ); break;
					case 14: from.AddItem( new FeatheredHat( hatHue ) ); break;
				}
			}

			if ( apron && Utility.RandomBool() )
			{
				if ( apron && Utility.RandomBool() ){ 	from.AddItem( new FullApron( Utility.RandomNeutralHue() ) ); }
				else { 									from.AddItem( new HalfApron( Utility.RandomNeutralHue() ) ); }

			}
			from.ProcessClothing();
			from.ProcessHair();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void DressUpWizards( Mobile from, bool isDojo )
		{
			int clothHue = Utility.RandomWizardHue();
			int cloakHue = Utility.RandomWizardHue();
				if ( from is Citizens || Utility.RandomBool() )
				{
					clothHue = Utility.RandomColor( Utility.RandomMinMax( 0, 12 ) );
					cloakHue = Utility.RandomColor( Utility.RandomMinMax( 0, 12 ) );
				}

			if ( isDojo )
			{ 
				Item robe = new Robe();
					robe.Delete();
					switch ( Utility.RandomMinMax( 1, 5 ) )
					{
						case 1: robe = new VagabondRobe(); break;
						case 2: robe = new ShinobiRobe(); break;
						case 3: robe = new ProphetRobe(); break;
						case 4: robe = new ScholarRobe(); break;
						case 5: robe = new Robe(); break;
					}
					robe.Hue = clothHue; if ( IsDrow( from ) ){ robe.Hue = Utility.RandomDrowHue(); } else if ( IsOrk( from ) ){ robe.Hue = Utility.RandomOrkHue(); }
					from.AddItem( robe ); 
			}
			else if ( Utility.RandomBool() )
			{ 
				Item robe = new Robe();
					robe.Delete();

					int wear = Utility.RandomMinMax( 0, 20 );
					if ( from.Body == 605 || from.Body == 606 ){ wear = Utility.RandomMinMax( 0, 19 ); } // ELVES CANNOT WEAR SORCERER ROBES

					switch ( wear )
					{
						case 0: robe = new AssassinRobe(); break;
						case 1: robe = new FancyRobe(); break;
						case 2: robe = new GildedRobe(); break;
						case 3: robe = new OrnateRobe(); break;
						case 4: robe = new MagistrateRobe(); break;
						case 5: robe = new RoyalRobe(); break;
						case 6: robe = new NecromancerRobe(); break;
						case 7: robe = new SpiderRobe(); break;
						case 8: robe = new VagabondRobe(); break;
						case 9: robe = new ExquisiteRobe(); break;
						case 10: robe = new ProphetRobe(); break;
						case 11: robe = new ElegantRobe(); break;
						case 12: robe = new FormalRobe(); break;
						case 13: robe = new ArchmageRobe(); break;
						case 14: robe = new PriestRobe(); break;
						case 15: robe = new CultistRobe(); break;
						case 16: robe = new GildedDarkRobe(); break;
						case 17: robe = new GildedLightRobe(); break;
						case 18: robe = new SageRobe(); break;
						case 19: robe = new ScholarRobe(); break;
						case 20: robe = new SorcererRobe(); break;
					}
					robe.Hue = clothHue; if ( IsDrow( from ) ){ robe.Hue = Utility.RandomDrowHue(); } else if ( IsOrk( from ) ){ robe.Hue = Utility.RandomOrkHue(); }
					from.AddItem( robe ); 
			}
			else if ( from.Body == 0x190 || from.Body == 605 )
			{
				Item robe = new Robe();
					robe.Hue = clothHue; if ( IsDrow( from ) ){ robe.Hue = Utility.RandomDrowHue(); } else if ( IsOrk( from ) ){ robe.Hue = Utility.RandomOrkHue(); }
					from.AddItem( robe ); 
			}
			else
			{
				Item dress = new PlainDress();
					dress.Hue = clothHue; if ( IsDrow( from ) ){ dress.Hue = Utility.RandomDrowHue(); } else if ( IsOrk( from ) ){ dress.Hue = Utility.RandomOrkHue(); }
					from.AddItem( dress ); 
			}

			if ( Utility.Random( 5 ) == 1 )
			{ 
				Cloak cloak = new Cloak();
					cloak.Hue = cloakHue; if ( IsDrow( from ) ){ cloak.Hue = Utility.RandomDrowHue(); } else if ( IsOrk( from ) ){ cloak.Hue = Utility.RandomOrkHue(); }
					from.AddItem( cloak );
					clothHue = cloakHue;
			}

			if ( Utility.RandomBool() )
			{ 
				if ( isDojo )
				{ 
					Item hat = new WizardsHat( clothHue );

						switch ( Utility.RandomMinMax( 1, 4 ) )
						{
							case 1: hat.ItemID = 0x2798;	hat.Name = "kasa";		break;
							case 2: hat.ItemID = 0x2776;	hat.Name = "jingasa";	break;
							case 3: hat.ItemID = 0x2777;	hat.Name = "jingasa";	break;
							case 4: hat.ItemID = 0x2798;	hat.Name = "kasa";		break;
						}

						switch ( Utility.RandomMinMax( 1, 20 ) )
						{
							case 1: hat.ItemID = 0x5C11;	hat.Name = "hood";		break;
							case 2: hat.ItemID = 0x5C12;	hat.Name = "mask";		break;
							case 3: hat.ItemID = 0x5C13;	hat.Name = "cowl";		break;
						}

						from.AddItem( hat ); 
				}
				else if ( Utility.RandomBool() )
				{ 
					int myHat = Utility.RandomMinMax( 0, 4 );
					if ( from.Body == 605 ){ myHat = 1; }
					Item hat = new WizardsHat();
						hat.Delete();
						switch ( myHat )
						{
							case 0: hat = new ClothCowl( clothHue ); break;
							case 1: hat = new ClothHood( clothHue ); break;
							case 2: hat = new FancyHood( clothHue ); break;
							case 3: hat = new WizardHood( clothHue); break;
							case 4: hat = new HoodedMantle( clothHue); break;
						}
						from.AddItem( hat ); 
				}
				else if ( from.Body == 0x190 || from.Body == 605 )
				{
					WizardsHat hat = new WizardsHat( clothHue );
						if ( IsDrow( from ) ){ hat.Hue = Utility.RandomDrowHue(); } else if ( IsOrk( from ) ){ hat.Hue = Utility.RandomOrkHue(); }
						from.AddItem( hat );
				}
				else
				{
					WitchHat hat = new WitchHat( clothHue );
						if ( IsDrow( from ) ){ hat.Hue = Utility.RandomDrowHue(); } else if ( IsOrk( from ) ){ hat.Hue = Utility.RandomOrkHue(); }
						from.AddItem( hat );
				}
			}

			Item boots = new ThighBoots( );
				boots.Hue = Utility.RandomNeutralHue();
				boots.Name = "boots";
				if ( Utility.RandomBool() ){ boots.ItemID = 12228; }
				from.AddItem( boots );

			from.ProcessClothing();
			from.ProcessHair();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void DressUpBards( Mobile from )
		{
			int pantsHue = Utility.RandomColor( Utility.RandomMinMax( 0, 12 ) );
			int shirtHue = Utility.RandomColor( Utility.RandomMinMax( 0, 12 ) );
			int hatHue = Utility.RandomColor( Utility.RandomMinMax( 0, 12 ) );

			if ( IsDrow( from ) )
			{
				pantsHue = Utility.RandomDrowHue();
				shirtHue = Utility.RandomDrowHue();
				hatHue = Utility.RandomDrowHue();
			}
			else if ( IsOrk( from ) )
			{
				pantsHue = Utility.RandomOrkHue();
				shirtHue = Utility.RandomOrkHue();
				hatHue = Utility.RandomOrkHue();
			}

			if ( from.Female )
			{
				from.AddItem( new Skirt( pantsHue ) );
			}
			else
			{
				from.AddItem( new ShortPants( pantsHue ) );
			}

			from.AddItem( new Boots( Utility.RandomNeutralHue() ) );
			from.AddItem( new FancyShirt( shirtHue ));
			
			switch ( Utility.Random( 4 ))
			{
				case 0: from.AddItem( new FeatheredHat( hatHue ) ); break;
				case 1: from.AddItem( new FloppyHat( hatHue ) ); break;
				case 2: from.AddItem( new StrawHat( hatHue ) ); break;
				case 3: from.AddItem( new SkullCap( hatHue ) ); break;
			}

			from.ProcessClothing();
			from.ProcessHair();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void DressUpFighters( Mobile m, string race, bool isEnemy, bool isDojo, bool isTown )
		{
			int cloakColor = 0;
			if ( 1 == Utility.RandomMinMax( 1, 2 ) ){ cloakColor = Utility.RandomColor( 0 ); m.AddItem( new Cloak( cloakColor ) ); }

			int aHue = Utility.RandomList( 0x973, 0x966, 0x96D, 0x972, 0x8A5, 0x979, 0x89F, 0x8AB, 0x497, 0, Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue() );
			int lHue = Utility.RandomList( 0x66D, 0x8A8, 0x455, 0x851, 0x8FD, 0x8B0, 0x283, 0x227, 0x1C1, 0x8AC, 0x845, 0x851, 0x497, 0, Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue() );

				if ( IsDrow( m ) ){ aHue = Utility.RandomDrowHue(); }
				if ( IsDrow( m ) ){ lHue = Utility.RandomDrowHue(); }
				if ( IsOrk( m ) ){ aHue = Utility.RandomOrkHue(); }
				if ( IsOrk( m ) ){ lHue = Utility.RandomOrkHue(); }

			int iArmor = Utility.RandomMinMax( 1, 6 );
				if ( race == "undead " && iArmor == 4 ){ iArmor = 1; }
				if ( iArmor == 5 && !isEnemy && Utility.RandomMinMax( 1, 10 ) != 1 ){ iArmor = 1; }
				if ( isDojo ){ iArmor = 5; }

			int hHue = aHue;

			int myTitle = Utility.RandomMinMax( 0, 8 );
			int myHelm = Utility.RandomMinMax( 0, 10 );

			if ( iArmor == 1 )
			{
				Item cloth1 = new PlateArms();
					cloth1.ItemID = Utility.RandomList( 0x1410, 0x1417, 0x303, 0x304, 0x305, 0x306 );
					cloth1.Hue = aHue;
					m.AddItem( cloth1 );
				Item cloth2 = new PlateGorget();
					cloth2.Hue = aHue;
					m.AddItem( cloth2 );
				Item cloth3 = new PlateLegs();
					cloth3.Hue = aHue;
					m.AddItem( cloth3 );
				Item cloth4 = new PlateChest();
					cloth4.Hue = aHue;
					m.AddItem( cloth4 );

				if ( Utility.RandomBool() || race == "undead " )
				{
					Item glove = new PlateGloves();
						glove.Hue = aHue;
						m.AddItem( glove );
				}
			}
			else if ( iArmor == 2 )
			{
				Item cloth1 = new ChainChest();
					cloth1.Hue = aHue;
					m.AddItem( cloth1 );
				Item cloth2 = new ChainLegs();
					cloth2.Hue = aHue;
					m.AddItem( cloth2 );
				Item cloth3 = new RingmailArms();
					cloth3.Hue = aHue;
					m.AddItem( cloth3 );
				Item cloth4 = new PlateGorget();
					cloth4.Hue = aHue;
					m.AddItem( cloth4 );

				if ( Utility.RandomBool() || race == "undead " )
				{
					Item glove = new RingmailGloves();
						glove.Hue = aHue;
						m.AddItem( glove );
				}
			}
			else if ( iArmor == 3 )
			{
				hHue = lHue;
				Item cloth1 = new StuddedChest();
					cloth1.Hue = lHue;
					m.AddItem( cloth1 );
				Item cloth2 = new StuddedArms();
					cloth2.Hue = lHue;
					m.AddItem( cloth2 );
				Item cloth3 = new StuddedLegs();
					cloth3.Hue = lHue;
					m.AddItem( cloth3 );
				Item cloth4 = new StuddedGorget();
					cloth4.Hue = lHue;
					m.AddItem( cloth4 );

				if ( Utility.RandomBool() || race == "undead " )
				{
					Item glove = new StuddedGloves();
						glove.Hue = lHue;
						m.AddItem( glove );
				}
			}
			else if ( iArmor == 4 )
			{
				myTitle = Utility.RandomMinMax( 9, 13 );
				myHelm = Utility.RandomMinMax( 6, 10 );
				int mySkirt = Utility.RandomMinMax( 1, 3 );

				hHue = lHue;

				if ( mySkirt == 1 )
				{
					if ( Utility.RandomBool() )
					{
						Item cloth1 = new LoinCloth();
							cloth1.Hue = lHue;
							m.AddItem( cloth1 );
					}
					else
					{
						Item cloth1 = new RoyalLoinCloth();
							cloth1.Hue = lHue;
							m.AddItem( cloth1 );
					}
				}
				else if ( mySkirt == 2 )
				{
					if ( Utility.RandomBool() )
					{
						Item cloth1 = new PlateSkirt();
							cloth1.Hue = lHue;
							cloth1.Name = "platemail skirt";
							m.AddItem( cloth1 );
					}
					else
					{
						Item cloth2 = new LeatherSkirt();
							cloth2.Hue = lHue;
							m.AddItem( cloth2 );
					}
				}
				else 
				{
					if ( Utility.RandomBool() )
					{
						Item cloth1 = new ChainSkirt();
							cloth1.Hue = lHue;
							cloth1.Name = "chainmail skirt";
							m.AddItem( cloth1 );
					}
					else
					{
						Item cloth1 = new RingmailSkirt();
							cloth1.Hue = lHue;
							cloth1.Name = "ringmail skirt";
							m.AddItem( cloth1 );
					}
				}

				if ( m.Body == 0x191 || m.Body == 606 )
				{
					Item cloth3 = new LeatherBustierArms();
						cloth3.Hue = lHue;
						m.AddItem( cloth3 );
				}

				if ( Utility.RandomMinMax( 1, 4 ) == 2 )
				{
					Item glove = new LeatherGloves();
						glove.Hue = lHue;
						m.AddItem( glove );
				}
			}
			else if ( iArmor == 5 )
			{
				if ( m.Body.IsFemale ){ m.Name = NameList.RandomName( "tokuno female" ); }
				else { m.Name = NameList.RandomName( "tokuno male" ); }

				myTitle = Utility.RandomMinMax( 14, 15 );

				hHue = lHue;

				SamuraiTabi cloth1 = new SamuraiTabi( );
					cloth1.Hue = lHue;
					m.AddItem( cloth1 );

				LeatherHiroSode cloth2 = new LeatherHiroSode( );
					cloth2.Hue = lHue;
					m.AddItem( cloth2 );

				LeatherDo cloth3 = new LeatherDo( );
					cloth3.Hue = lHue;
					m.AddItem( cloth3 );

				if ( Utility.RandomBool() || race == "undead " )
				{
					Item glove = new LeatherGloves();
						glove.Hue = lHue;
						m.AddItem( glove );
				}

				switch ( Utility.Random( 4 ) )
				{
					case 0: LightPlateJingasa cloth4 = new LightPlateJingasa( );	cloth4.Hue = lHue;	m.AddItem( cloth4 ); break;
					case 1: ChainHatsuburi cloth5 = new ChainHatsuburi( );	cloth5.Hue = lHue;	m.AddItem( cloth5 ); break;
					case 2: DecorativePlateKabuto cloth6 = new DecorativePlateKabuto( );	cloth6.Hue = lHue;	m.AddItem( cloth6 ); break;
					case 3: LeatherJingasa cloth7 = new LeatherJingasa( );	cloth7.Hue = lHue;	m.AddItem( cloth7 ); break;
				}

				switch ( Utility.Random( 3 ) )
				{
					case 0: StuddedHaidate cloth8 = new StuddedHaidate( );	cloth8.Hue = lHue;	m.AddItem( cloth8 ); break;
					case 1: LeatherSuneate cloth9 = new LeatherSuneate( );	cloth9.Hue = lHue;	m.AddItem( cloth9 ); break;
					case 2: PlateSuneate cloth0 = new PlateSuneate( );	cloth0.Hue = lHue;	m.AddItem( cloth0 ); break;
				}
			}
			else
			{
				hHue = lHue;
				Item cloth1 = new LeatherArms();
					cloth1.Hue = lHue;
					m.AddItem( cloth1 );
				Item cloth2 = new LeatherChest();
					cloth2.Hue = lHue;
					m.AddItem( cloth2 );
				Item cloth3 = new LeatherGorget();
					cloth3.Hue = lHue;
					m.AddItem( cloth3 );
				Item cloth4 = new LeatherLegs();
					cloth4.Hue = lHue;
					m.AddItem( cloth4 );

				if ( Utility.RandomBool() || race == "undead " )
				{
					Item glove = new LeatherGloves();
						glove.Hue = lHue;
						m.AddItem( glove );
				}
			}

			int CanHaveHelm = 0;

			if ( iArmor == 4 && Utility.RandomBool() ) { m.AddItem( new BarbarianBoots( Utility.RandomNeutralHue() ) ); }
			else { m.AddItem( new Boots( Utility.RandomNeutralHue() ) ); }

			if ( Utility.RandomBool() && iArmor != 5 ){ CanHaveHelm = 1; }
			if ( race == "undead " && m.Hue != 0x430 ){ CanHaveHelm = 1; }
			if ( race == "undead " && m.Hue == 0x430 ){ CanHaveHelm = 0; }

			if ( CanHaveHelm > 0 )
			{
				if ( cloakColor > 0 && Utility.Random( 20 ) == 1 )
				{
					switch ( Utility.RandomMinMax( 1, 4 ) )
					{
						case 1: m.AddItem( new ClothCowl( cloakColor ) ); break;
						case 2: m.AddItem( new ClothHood( cloakColor ) ); break;
						case 3: m.AddItem( new FancyHood( cloakColor ) ); break;
						case 4: m.AddItem( new HoodedMantle( cloakColor ) ); break;
					}
				}
				else
				{
					switch ( myHelm )
					{
						case 0: Item helm1 = new PlateHelm(); helm1.Hue = hHue; m.AddItem( helm1 ); break;
						case 1: Item helm2 = new Bascinet();  helm2.Hue = hHue; m.AddItem( helm2 ); break;
						case 2: Item helm3 = new ChainCoif(); helm3.Hue = hHue; m.AddItem( helm3 ); break;
						case 3: Item helm4 = new CloseHelm(); helm4.Hue = hHue; m.AddItem( helm4 ); break;
						case 4: Item helm5 = new LeatherCap(); helm5.Hue = hHue; m.AddItem( helm5 ); break;
						case 5: Item helm6 = new Helmet(); helm6.Hue = hHue; m.AddItem( helm6 ); break;
						case 6: Item helm7 = new OrcHelm(); helm7.Hue = hHue; m.AddItem( helm7 ); break;
						case 7: Item helm8 = new NorseHelm(); helm8.Hue = hHue; m.AddItem( helm8 ); break;
						case 8: Item helm9 = new BoneHelm(); helm9.Hue = hHue; m.AddItem( helm9 ); break;
						case 9: Item helm10 = new OrcHelm(); helm10.Name = "great helm"; helm10.ItemID = 0x2645; helm10.Hue = hHue; m.AddItem( helm10 ); break;
						case 10: Item helm11 = new JewelryCirclet(); helm11.Hue = hHue; m.AddItem( helm11 ); break;
					}
				}
			}

			int CanHaveShield = 1;

			Item weapon = new Bow(); weapon.Delete();

			if ( isTown && Utility.RandomMinMax(1,20) == 1 )
			{
				CanHaveShield = 0; 
				switch ( Utility.Random( 8 ) )
				{
					case 0: weapon = new Bow(); break;
					case 1: weapon = new CompositeBow(); break;
					case 2: weapon = new Crossbow(); break;
					case 3: weapon = new ElvenCompositeLongbow(); break;
					case 4: weapon = new HeavyCrossbow(); break;
					case 5: weapon = new MagicalShortbow(); break;
					case 6: weapon = new RepeatingCrossbow(); break;
					case 7: weapon = new Yumi(); break;
				}
			}
			else if ( iArmor == 5 )
			{
				CanHaveShield = 0; 
				switch ( Utility.Random( 5 ) )
				{
					case 0: weapon = new NoDachi(); break;
					case 1: weapon = new Halberd(); break;
					case 2: weapon = new Wakizashi(); break;
					case 3: weapon = new Longsword(); break;
					case 4: weapon = new Bokuto(); break;
				}
			}
			else
			{
				switch ( Utility.Random( 31 ) )
				{
					case 0: weapon = new BattleAxe(); CanHaveShield = 0; break;
					case 1: weapon = new VikingSword(); break;
					case 2: weapon = new Halberd(); CanHaveShield = 0; break;
					case 3: weapon = new DoubleAxe(); break;
					case 4: weapon = new ExecutionersAxe(); CanHaveShield = 0; break;
					case 5: weapon = new WarAxe(); break;
					case 6: weapon = new TwoHandedAxe(); CanHaveShield = 0; break;
					case 7: weapon = new Cutlass(); break;
					case 8: weapon = new Katana(); break;
					case 9: weapon = new Kryss(); break;
					case 10: weapon = new Broadsword(); break;
					case 11: weapon = new Longsword(); break;
					case 12: weapon = new ThinLongsword(); break;
					case 13: weapon = new Scimitar(); break;
					case 14: weapon = new BoneHarvester(); break;
					case 15: weapon = new CrescentBlade(); CanHaveShield = 0; break;
					case 16: weapon = new DoubleBladedStaff(); CanHaveShield = 0; break;
					case 17: weapon = new Pike(); CanHaveShield = 0; break;
					case 18: weapon = new Scythe(); CanHaveShield = 0; break;
					case 19: weapon = new Pitchfork(); CanHaveShield = 0; break;
					case 20: weapon = new ShortSpear(); CanHaveShield = 0; break;
					case 21: weapon = new Spear(); CanHaveShield = 0; break;
					case 22: weapon = new Club(); break;
					case 23: weapon = new HammerPick(); break;
					case 24: weapon = new Mace(); break;
					case 25: weapon = new Maul(); break;
					case 26: weapon = new WarHammer(); CanHaveShield = 0; break;
					case 27: weapon = new WarMace(); break;
					case 28: weapon = new SpikedClub(); break;
					case 29: weapon = new Hammers(); break;
					case 30: weapon = new Whips(); break;
				}
			}

			m.AddItem( weapon );

			if ( CanHaveShield == 1 && Utility.RandomMinMax( 1, 3 ) == 1 )
			{
				switch ( Utility.Random( 16 ) )
				{
					case 0: m.AddItem( new BronzeShield() ); break;
					case 1: m.AddItem( new Buckler() ); break;
					case 2: m.AddItem( new MetalKiteShield() ); break;
					case 3: m.AddItem( new HeaterShield() ); break;
					case 4: m.AddItem( new WoodenKiteShield() ); break;
					case 5: m.AddItem( new MetalShield() ); break;
					case 6: m.AddItem( new ChaosShield() ); break;
					case 7: m.AddItem( new OrderShield() ); break;
					case 8: m.AddItem( new SunShield() ); break;
					case 9: m.AddItem( new VirtueShield() ); break;
					case 10: m.AddItem( new GuardsmanShield() ); break;
					case 11: m.AddItem( new ElvenShield() ); break;
					case 12: m.AddItem( new DarkShield() ); break;
					case 13: m.AddItem( new CrestedShield() ); break;
					case 14: m.AddItem( new ChampionShield() ); break;
					case 15: m.AddItem( new JeweledShield() ); break;
				}
			}

			switch ( myTitle )
			{
				case 0: m.Title = "the " + race + "fighter"; break;
				case 1: m.Title = "the " + race + "knight"; break;
				case 2: m.Title = "the " + race + "champion"; break;
				case 3: m.Title = "the " + race + "warrior"; break;
				case 4: m.Title = "the " + race + "soldier"; break;
				case 5: m.Title = "the " + race + "vanquisher"; break;
				case 6: m.Title = "the " + race + "battler"; break;
				case 7: m.Title = "the " + race + "gladiator"; break;
				case 8: m.Title = "the " + race + "mercenary"; break;
				case 9: m.Title = "the " + race + "nomad"; break;
				case 10: m.Title = "the " + race + "berserker"; break;
				case 11: m.Title = "the " + race + "barbarian"; if ( m.Female ){ m.Title = "the " + race + "amazon"; } break;
				case 12: m.Title = "the " + race + "pit fighter"; break;
				case 13: m.Title = "the " + race + "brute"; break;
				case 14: m.Title = "the " + race + "samurai"; break;
				case 15: m.Title = "the " + race + "ronin"; break;
			}

			m.ProcessClothing();
			m.ProcessHair();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void DressUpRogues( Mobile m, string race, bool isEnemy, bool isDojo, bool isTown )
		{
			BaseCreature bc = (BaseCreature)m;

			Region reg = Region.Find( bc.Home, bc.Map );

			int clothHue = Utility.RandomList( 0x973, 0x966, 0x96D, 0x972, 0x8A5, 0x979, 0x89F, 0x8AB, 0x66D, 0x8A8, 0x455, 0x851, 0x8FD, 0x8B0, 0x283, 0x227, 0x1C1, 0x8AC, 0x845, 0x851, 0x497, Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue() );
			int pantsHue = Utility.RandomList( 0x973, 0x966, 0x96D, 0x972, 0x8A5, 0x979, 0x89F, 0x8AB, 0x66D, 0x8A8, 0x455, 0x851, 0x8FD, 0x8B0, 0x283, 0x227, 0x1C1, 0x8AC, 0x845, 0x851, 0x497, Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue(), Utility.RandomMetalHue() );
				if ( IsDrow( m ) ){ clothHue = Utility.RandomDrowHue(); pantsHue = Utility.RandomDrowHue(); }
				if ( IsOrk( m ) ){ clothHue = Utility.RandomOrkHue(); pantsHue = Utility.RandomOrkHue(); }

			int iArmor = Utility.RandomMinMax( 1, 3 );

			int myTitle = Utility.RandomMinMax( 0, 9 );
			int myHelm = Utility.RandomMinMax( 0, 9 );

			if ( reg.IsPartOf( "the Ruins of the Black Blade" ) )
			{
				iArmor = 2;
			}
			else if ( reg.IsPartOf( "Stonegate Castle" ) )
			{
				iArmor = 2;
			}
			else if ( isDojo )
			{
				iArmor = 2;
			}
			else if ( m is Brigand )
			{
				iArmor = 3;
			}

			if ( iArmor == 1 )
			{
				m.AddItem( new Boots( Utility.RandomNeutralHue() ) );
				if ( 1 == Utility.RandomMinMax( 1, 2 ) ){ m.AddItem( new Cloak( Utility.RandomNeutralHue() ) ); }

				Item cloth1 = new LeatherChest();
					cloth1.Hue = clothHue;
					m.AddItem( cloth1 );
				Item cloth2 = new LeatherArms();
					cloth2.Hue = clothHue;
					m.AddItem( cloth2 );
				Item cloth3 = new LeatherLegs();
					cloth3.Hue = clothHue;
					m.AddItem( cloth3 );
				Item cloth4 = new LeatherGorget();
					cloth4.Hue = clothHue;
					m.AddItem( cloth4 );

				if ( Utility.RandomBool() )
				{
					Item glove = new LeatherGloves();
						glove.Hue = clothHue;
						m.AddItem( glove );
				}
			}
			else if ( iArmor == 2 )
			{
				m.YellHue = 1;
				string suit = "assassin";
				bool shinobi = false;
					if ( Utility.RandomMinMax(1,10) == 1 ){ shinobi = true; }

				m.EmoteHue = 1;

				int thief = Utility.RandomList(0,1,2);
					if ( !isEnemy ){ thief = Utility.RandomList(0,1); }

				int outfit = Utility.RandomMinMax( 0, 2 );
					if ( Utility.RandomMinMax( 1, 10 ) != 1 && outfit == 2 ){ outfit = Utility.RandomMinMax( 0, 1 ); }

				switch ( outfit )
				{
					case 0: m.Title = "the " + race + "assassin";	suit = "assassin"; m.YellHue = 2; break;
					case 1: m.Title = "the " + race + "hunter";		suit = "hunter"; break;
					case 2: m.Title = "the " + race + "ninja";		suit = "ninja"; myHelm = Utility.RandomList( 6, 10, 11, 12, 13 ); break;
				}

				if ( isDojo ){ m.Title = "the " + race + "ninja";	suit = "ninja"; myHelm = Utility.RandomList( 6, 10, 11, 12, 13 ); }

				myTitle = 100;

				if ( suit == "ninja" && m.Body.IsFemale ){ m.Name = NameList.RandomName( "tokuno female" ); }
				else if ( suit == "ninja" ){ m.Name = NameList.RandomName( "tokuno male" ); }

				if ( Utility.RandomBool() || suit == "ninja" )
				{
					Item jacket = new LeatherNinjaJacket( );
						if ( shinobi ){ jacket.Delete(); jacket = new OniwabanTunic(); }
						jacket.Name = suit + " tunic";
						jacket.Hue = clothHue;
						m.AddItem( jacket );
				}
				else
				{
					LeatherChest jacket = new LeatherChest( );
						jacket.Name = suit + " tunic";
						jacket.Hue = clothHue;
						m.AddItem( jacket );
				}

				if ( Utility.RandomBool() || suit == "ninja" )
				{
					Item pants = new LeatherNinjaPants( );
						if ( shinobi ){ pants.Delete(); pants = new OniwabanLeggings(); }
						pants.Name = suit + " leggings";
						pants.Hue = clothHue;
						m.AddItem( pants );
				}
				else
				{
					LeatherLegs pants = new LeatherLegs( );
						pants.Name = suit + " leggings";
						pants.Hue = clothHue;
						m.AddItem( pants );
				}

				if ( Utility.RandomBool() || suit == "ninja" )
				{
					Item gloves = new LeatherNinjaMitts( );
						if ( shinobi ){ gloves.Delete(); gloves = new OniwabanGloves(); }
						gloves.Name = suit + " gloves";
						gloves.Hue = clothHue;
						m.AddItem( gloves );
				}
				else
				{
					LeatherGloves gloves = new LeatherGloves( );
						gloves.Name = suit + " gloves";
						gloves.Hue = clothHue;
						m.AddItem( gloves );
				}

				if ( Utility.RandomBool() || suit == "ninja" )
				{
					Item boot = new NinjaTabi( );
						if ( shinobi ){ boot.Delete(); boot = new OniwabanBoots(); }
						boot.Name = suit + " boots";
						boot.Hue = clothHue;
						m.AddItem( boot );
				}
				else
				{
					Boots boot = new Boots( );
						boot.Name = suit + " boots";
						boot.Hue = clothHue;
						m.AddItem( boot );
				}
			}
			else
			{
				m.AddItem( new Boots( Utility.RandomNeutralHue() ) );
				if ( Utility.RandomBool() ){ m.AddItem( new Cloak( Utility.RandomNeutralHue() ) ); }

				Item cloth1;
					switch ( Utility.RandomMinMax( 0, 6 ) )
					{
						case 0: cloth1 = new RoyalCoat(); break;
						case 1: cloth1 = new RoyalShirt(); break;
						case 2: cloth1 = new RusticShirt(); break;
						case 3: cloth1 = new SquireShirt(); break;
						case 4: cloth1 = new FormalCoat(); break;
						case 5: cloth1 = new WizardShirt(); break;
						default: cloth1 = new FancyShirt(); break;
					}
					cloth1.Hue = clothHue;
					m.AddItem( cloth1 );

				if ( ( m.Body == 0x191 || m.Body == 606 ) && Utility.RandomBool() )
				{
					Item cloth2;
						switch ( Utility.RandomMinMax( 0, 2 ) )
						{
							case 0: cloth2 = new RoyalSkirt(); break;
							case 1: cloth2 = new RoyalLongSkirt(); break;
							default: cloth2 = new Skirt(); break;
						}
						cloth2.Hue = pantsHue;
						m.AddItem( cloth2 );
				}
				else
				{
					Item cloth2;
						switch ( Utility.RandomMinMax( 0, 3 ) )
						{
							case 0: cloth2 = new LongPants(); break;
							case 1: cloth2 = new SailorPants(); break;
							case 2: cloth2 = new PiratePants(); break;
							default: cloth2 = new ShortPants(); break;
						}
						cloth2.Hue = pantsHue;
						m.AddItem( cloth2 );
				}

				if ( Utility.RandomBool() )
				{
					Item glove = new LeatherGloves();
						glove.Hue = clothHue;
						m.AddItem( glove );
				}
			}

			switch ( myTitle )
			{
				case 0: m.Title = "the " + race + "thief"; break;
				case 1: m.Title = "the " + race + "rogue"; break;
				case 2: m.Title = "the " + race + "outlaw"; break;
				case 3: m.Title = "the " + race + "bandit"; break;
				case 4: m.Title = "the " + race + "pickpocket"; break;
				case 5: m.Title = "the " + race + "burglar"; break;
				case 6: m.Title = "the " + race + "robber"; break;
				case 7: m.Title = "the " + race + "criminal"; break;
				case 8: m.Title = "the " + race + "prowler"; break;
				case 9: m.Title = "the " + race + "pilferer"; break;
			}

			if ( m is Brigand ){ m.Title = "the " + race + "brigand"; }

			if ( Utility.RandomBool() || m.Title == "the " + race + "ninja" )
			{
				if ( m.Body == 605 && myHelm == 7 ){ myHelm = 8; }

				switch ( myHelm )
				{
					case 0: Item helm1 = new SkullCap(); 			helm1.Hue = clothHue; m.AddItem( helm1 ); break;
					case 1: Item helm2 = new FloppyHat(); 			helm2.Hue = clothHue; m.AddItem( helm2 ); break;
					case 2: Item helm3 = new LeatherCap(); 			helm3.Hue = clothHue; m.AddItem( helm3 ); break;
					case 3: Item helm4 = new FeatheredHat(); 		helm4.Hue = clothHue; m.AddItem( helm4 ); break;
					case 4: Item helm5 = new JewelryCirclet(); 		helm5.Hue = clothHue; m.AddItem( helm5 ); break;
					case 5: Item helm6 = new WideBrimHat(); 		helm6.Hue = clothHue; m.AddItem( helm6 ); break;
					case 6: Item helm7 = new LeatherNinjaHood(); 	helm7.Hue = clothHue; m.AddItem( helm7 ); break;
					case 7: Item helm8 = new ClothCowl(); 			helm8.Hue = clothHue; m.AddItem( helm8 ); break;
					case 8: Item helm9 = new ClothHood(); 			helm9.Hue = clothHue; m.AddItem( helm9 ); break;
					case 9: Item helm10 = new HoodedMantle(); 		helm10.Hue = clothHue; m.AddItem( helm10 ); break;
					case 10: Item helm11 = new ShinobiCowl(); 		helm11.Hue = clothHue; m.AddItem( helm11 ); break;
					case 11: Item helm12 = new ShinobiHood(); 		helm12.Hue = clothHue; m.AddItem( helm12 ); break;
					case 12: Item helm13 = new ShinobiMask(); 		helm13.Hue = clothHue; m.AddItem( helm13 ); break;
					case 13: Item helm14 = new OniwabanHood(); 		helm14.Hue = clothHue; m.AddItem( helm14 ); break;
				}
			}

			int CanHaveShield = 1;

			Item weapon = new Bow(); weapon.Delete();

			if ( isTown && Utility.RandomMinMax(1,20) == 1 )
			{
				CanHaveShield = 0; 
				switch ( Utility.Random( 8 ) )
				{
					case 0: weapon = new Bow(); break;
					case 1: weapon = new CompositeBow(); break;
					case 2: weapon = new Crossbow(); break;
					case 3: weapon = new ElvenCompositeLongbow(); break;
					case 4: weapon = new HeavyCrossbow(); break;
					case 5: weapon = new MagicalShortbow(); break;
					case 6: weapon = new RepeatingCrossbow(); break;
					case 7: weapon = new Yumi(); break;
				}
			}
			else if ( iArmor == 2 )
			{
				CanHaveShield = 0; 
				switch ( Utility.Random( 7 ) )
				{
					case 0: weapon = new Daisho(); break;
					case 1: weapon = new Nunchaku(); break;
					case 2: weapon = new Sai(); break;
					case 3: weapon = new Kama(); break;
					case 4: weapon = new NoDachi(); break;
					case 5: weapon = new Tekagi(); break;
					case 6: weapon = new Wakizashi(); break;
				}
			}
			else
			{
				switch ( Utility.Random( 19 ) )
				{
					case 0: weapon = new Cutlass(); break;
					case 1: weapon = new Katana(); break;
					case 2: weapon = new Kryss(); break;
					case 3: weapon = new Broadsword(); break;
					case 4: weapon = new Longsword(); break;
					case 5: weapon = new ThinLongsword(); break;
					case 6: weapon = new Scimitar(); break;
					case 7: weapon = new BoneHarvester(); break;
					case 8: weapon = new ShortSpear(); CanHaveShield = 0; break;
					case 9: weapon = new Club(); break;
					case 10: weapon = new Dagger(); break;
					case 11: weapon = new AssassinSpike(); break;
					case 12: weapon = new RuneBlade(); CanHaveShield = 0; break;
					case 13: weapon = new Leafblade(); break;
					case 14: weapon = new ElvenSpellblade(); break;
					case 15: weapon = new ElvenMachete(); break;
					case 16: weapon = new LargeKnife(); break;
					case 17: weapon = new ShortSword(); break;
					case 18: weapon = new Whips(); break;
				}
			}

			m.AddItem( weapon );

			if ( CanHaveShield == 1 && Utility.RandomMinMax( 1, 3 ) == 1 )
			{
				Item shield1 = new Buckler();
				shield1.Hue = clothHue; 
				m.AddItem( shield1 );
			}

			if ( reg.IsPartOf( "the Ruins of the Black Blade" ) )
			{
				MorphingTime.ColorMyClothes( m, 0x497, 0 );
				m.Title = "the dark assassin";
			}
			else if ( reg.IsPartOf( "Stonegate Castle" ) )
			{
				MorphingTime.ColorMyClothes( m, 0x541, 0 );
				m.Title = "the shadow assassin";
				m.Hue = 0x4001;
				m.HairItemID = 0;
				m.FacialHairItemID = 0;
				m.BaseSoundID = 0x482;
			}
			else
			{
				GiveAdventureGear( (BaseCreature)m );
			}

			m.ProcessClothing();
			m.ProcessHair();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void GiveAdventureGear( BaseCreature m )
		{
			if ( Utility.RandomMinMax( 1, 10 ) > 3 )
			{
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0: m.PackItem( new BreadLoaf( Utility.RandomMinMax( 1, 3 ) ) ); break;
					case 1: m.PackItem( new CheeseWheel( Utility.RandomMinMax( 1, 3 ) ) ); break;
					case 2: m.PackItem( new Ribs( Utility.RandomMinMax( 1, 3 ) ) ); break;
					case 3: m.PackItem( new Apple( Utility.RandomMinMax( 1, 3 ) ) ); break;
					case 4: m.PackItem( new CookedBird( Utility.RandomMinMax( 1, 3 ) ) ); break;
					case 5: m.PackItem( new LambLeg( Utility.RandomMinMax( 1, 3 ) ) ); break;
				}
			}
			if ( Utility.RandomMinMax( 1, 10 ) > 3 )
			{
				switch ( Utility.RandomMinMax( 0, 4 ) )
				{
					case 0: m.PackItem( new BeverageBottle( BeverageType.Ale ) ); break;
					case 1: m.PackItem( new BeverageBottle( BeverageType.Wine ) ); break;
					case 2: m.PackItem( new BeverageBottle( BeverageType.Liquor ) ); break;
					case 3: m.PackItem( new Jug( BeverageType.Cider ) ); break;
					case 4: m.PackItem( new Pitcher( BeverageType.Water ) ); break;
				}
			}
			if ( Utility.RandomMinMax( 1, 10 ) > 3 )
			{
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0: m.PackItem( new Torch() ); break;
					case 1: m.PackItem( new Candle() ); break;
					case 2: m.PackItem( new Lantern() ); break;
				}
			}
			if ( Utility.RandomMinMax( 1, 10 ) > 3 )
			{
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0: m.PackItem( new Bandage( Utility.RandomMinMax( 5, 15 ) ) ); break;
					case 1: LesserCurePotion pot1 = new LesserCurePotion(); pot1.Amount = Utility.RandomMinMax( 1, 2 ); m.PackItem( pot1 ); break;
					case 2: LesserHealPotion pot2 = new LesserHealPotion(); pot2.Amount = Utility.RandomMinMax( 1, 2 ); m.PackItem( pot2 ); break;
				}
			}

			if ( Utility.RandomMinMax( 1, 10 ) > 3 )
			{
				switch ( Utility.RandomMinMax( 0, 9 ) )
				{
					case 0: TenFootPole pole = new TenFootPole(); ResourceMods.SetRandomResource( false, true, pole, CraftResource.RegularWood, false, m ); m.PackItem( pole ); break;
					case 1: m.PackItem( new Lockpick() ); break;
					case 2: m.PackItem( new SkeletonsKey() ); break;
					case 3: m.PackItem( new Bottle( Utility.RandomMinMax( 1, 3 ) ) ); break;
					case 4: m.PackItem( new Pouch() ); break;
					case 5: m.PackItem( new Bag() ); break;
					case 6: m.PackItem( new Bedroll() ); break;
					case 7: m.PackItem( new Kindling( Utility.RandomMinMax( 1, 3 ) ) ); break;
					case 8: m.PackItem( new WritingBook() ); break;
					case 9: Spyglass spy = new Spyglass(); ResourceMods.SetRandomResource( false, true, spy, CraftResource.RegularWood, false, m ); m.PackItem( spy ); break;
				}
			}
			GiveTorch( m );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void BeforeMyDeath( Mobile from )
		{
			ArrayList targets = new ArrayList();

			foreach ( Mobile myPet in from.GetMobilesInRange( 30 ) )
			{
				if ( myPet is BaseCreature )
				{
					if ( ((BaseCreature)myPet).YellHue == from.Serial && ((BaseCreature)myPet).IsTempEnemy )
					{
						targets.Add( myPet );
					}
				}
			}

			for ( int i = 0; i < targets.Count; ++i )
			{
				Mobile pet = ( Mobile )targets[ i ];
				Effects.SendLocationParticles( EffectItem.Create( pet.Location, pet.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				pet.PlaySound( 0x1FE );
				pet.Delete();
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static bool HealThySelf( Mobile from )
		{
			if ( from.Mana > 20 && !Server.Items.MortalStrike.IsWounded( from ) && Utility.RandomMinMax( 1, 4 ) == 1  )
			{
				from.Mana = from.Mana - 20;
				from.Hits = from.HitsMax;
				from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				from.PlaySound( 0x202 );
				return true;
			}

			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void MakeAssassinNote( Mobile from )
		{
			bool MakeNote = false;

			if ( from.Title.Contains(" assassin") ){ MakeNote = true; }
			else if ( from.Title.Contains(" hunter") ){ MakeNote = true; }
			else if ( from.Title.Contains(" ninja") ){ MakeNote = true; }

			if ( Utility.RandomMinMax( 1, 10 ) > 1 ){ MakeNote = false; }

			if ( MakeNote )
			{
				Mobile killer = from.LastKiller;

				Region reg = Region.Find( from.Location, from.Map );
				
				if (killer is BaseCreature)
				{
					BaseCreature bc_killer = (BaseCreature)killer;
					if(bc_killer.Summoned)
					{
						if(bc_killer.SummonMaster != null)
							killer = bc_killer.SummonMaster;
					}
					else if(bc_killer.Controlled)
					{
						if(bc_killer.ControlMaster != null)
							killer=bc_killer.ControlMaster;
					}
					else if(bc_killer.BardProvoked)
					{
						if(bc_killer.BardMaster != null)
							killer=bc_killer.BardMaster;
					}
				}

				if ( killer is PlayerMobile )
				{
					int gold = (int)((killer.RawStr + killer.RawDex + killer.RawInt) / 12 ) + Utility.RandomMinMax( 0, 5 );
					string a = "him";
					string b = "he";
					string c = "his";

					if ( killer.Body == 0x191 )
					{
						a = "her";
						b = "she";
						c = "her";
					} 

					string ScrollText = from.Name + ",<br><br>You have been given a task by " + RandomThings.GetRandomSociety() + ". You are to find " + killer.Name + " and make sure you kill " + a + " while " + b + " is in " + Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) + ". When the deed is done, meet " + QuestCharacters.ParchmentWriter() + " in " + RandomThings.GetRandomCity() + " where you can collect your " + gold.ToString() + ",000 gold.<br><br> - " + QuestCharacters.ParchmentWriter();

					switch ( Utility.RandomMinMax( 0, 9 ) )
					{
						case 1:	ScrollText = "If we are going to carry out our plans, you need to kill " + killer.Name + " as we think " + b + " will become a problem for us. We heard from the " + RandomThings.GetRandomJob() + " in " + RandomThings.GetRandomCity() + " that " + b + " may be going to " + Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) + ". Wait for " + a + " there and strike when the time is right. If we do not see you return to " + RandomThings.GetRandomCity() + " soon, we will assume you failed.<br><br> - " + RandomThings.GetRandomSociety();
							break;
						case 2:	ScrollText = QuestCharacters.ParchmentWriter() + ",<br><br>We all know who stole the " + gold.ToString() + ",000 gold from me in " + RandomThings.GetRandomCity() + ". I will make a deal with you. You find out where " + from.Name + " is hiding and tell them that I will overlook this incident if they do something for me. They need to go to " + Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) + " and wait in the shadows for " + killer.Name + " to arrive. When " + b + " is spotted, they need to kill " + a + " before " + b + " finds what " + b + " is looking for. When the deed is done, the debt will be forgiven.<br><br> - " + QuestCharacters.ParchmentWriter();
							break;
						case 3:	ScrollText = from.Name + ",<br><br>The time is almost near, but there are some that fear of " + killer.Name + " causing a problem for us. We need to send " + a + " away from this life before " + b + " realizes what we are about to do. " + QuestCharacters.ParchmentWriter() + " has followed " + a + " to " + Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) + " so you can probably find " + a + " there. Do not fail, as you would not want to face the judgement of " + RandomThings.GetRandomSociety() + ".";
							break;
						case 4:	ScrollText = QuestCharacters.ParchmentWriter() + ",<br><br>I have another problem for you to take care of. A " + RandomThings.GetRandomJob() + " in " + RandomThings.GetRandomCity() + " is paying us " + gold.ToString() + ",000 gold to assassinate the one who killed their friend. I think we should send " + from.Name + " to deal with " + a + ", and " + b + " is known as " + killer.Name + ". I believe they headed in the direction of " + Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) + ". Have them wait for " + a + " there.<br><br> - " + QuestCharacters.ParchmentWriter();
							break;
						case 5:	ScrollText = from.Name + ",<br><br>I was in " + RandomThings.GetRandomCity() + " and I heard some whispers of a " + gold.ToString() + ",000 gold bounty on " + killer.Name + ". I also heard that " + RandomThings.GetRandomSociety() + " is the one offering the gold. I don't need to remind you that we have been trying to gain their trust so we can acquire " + Server.Misc.QuestCharacters.QuestItems( true ) + ". I paid the barkeep a few coins and found out where " + b + " might be. I will check out the place nearby. You head to " + Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) + " and look for " + a + " there. We will meet each other in " + RandomThings.GetRandomCity() + " in a few days. This could be the chance we were waiting for.<br><br> - " + QuestCharacters.ParchmentWriter();
							break;
						case 6:	ScrollText = from.Name + ",<br><br>You are to slay the one they call " + killer.Name + ". They have been a little too curious of what lies within " + Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) + ", and we do not need " + a + " getting in our way to find " + Server.Misc.QuestCharacters.QuestItems( true ) + ". If " + b + " finds it before we do, it could mean our very lives " + RandomThings.GetRandomSociety() + " will want. I will be going to " + RandomThings.GetRandomCity() + " to get some supplies, but I will return soon.<br><br> - " + QuestCharacters.ParchmentWriter();
							break;
						case 7:	ScrollText = from.Name + ",<br><br>Now that you eliminated " + QuestCharacters.ParchmentWriter() + ", it is time for your next target. This will be well worth your time as the fee being paid for this one is " + gold.ToString() + ",000 gold. They are known as " + killer.Name + ", and " + b + " has been talking about exploring " + Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) + ". If this is true, then " + QuestCharacters.ParchmentWriter() + " would have to hide " + Server.Misc.QuestCharacters.QuestItems( true ) + " elsewhere. You can guess by the gold being offered, that they would rather not resort to such actions. Take care of " + a + " and we will split the gold in " + RandomThings.GetRandomCity() + ".";
							break;
						case 8:	ScrollText = QuestCharacters.ParchmentWriter() + ",<br><br>" + killer.Name + " has meddled in our plans for the last time. " + QuestCharacters.ParchmentWriter() + " claims that " + b + " doesn't even know of " + c + " participation in my annoyance, but the time to act is now. I think we should give this mission to " + from.Name + ", as they have yet to fail us. Send " + from.Name + " to " + Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) + " and kill " + a + " before " + b + " leaves the area. Bring back " + Server.Misc.QuestCharacters.QuestItems( true ) + " if " + b + " is found with it.<br><br> - " + QuestCharacters.ParchmentWriter();
							break;
						case 9:	ScrollText = from.Name + ",<br><br>I left that magic item in a dungeon chest for safe keeping, but " + killer.Name + " ended up taking it! Find " + a + " and kill " + a + "! See if " + b + " still has it. If you find it, head to " + RandomThings.GetRandomCity() + " and give it to the " + RandomThings.GetRandomJob() + ". They'll know what to do with it.";
							break;
					}

					AssassinNote letter = new AssassinNote();
					letter.LetterMessage = ScrollText;
					((BaseCreature)from).PackItem( letter );
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void ChooseMonk( Mobile m, string race )
		{
			BaseCreature bc = (BaseCreature)m;

			Region reg = Region.Find( bc.Home, m.Map );

			m.Title = "the " + race + "monk";

			int level = 0;
			int color = 0;
			string named = race + "monk robe";

			if ( reg.IsPartOf( "Mangar's Tower" ) || reg.IsPartOf( "Mangar's Chamber" ) )
			{
				level = 5;
				color = 0xB89;
				named = "ivory monk robe";
				m.Title = "the ivory monk";
			}
			else if ( reg.IsPartOf( "Kylearan's Tower" ) )
			{
				level = 4;
				color = 0x125;
				named = "azure monk robe";
				m.Title = "the azure monk";
			}
			else if ( reg.IsPartOf( "Harkyn's Castle" ) )
			{
				level = 3;
				color = 0x8A;
				named = "scarlet monk robe";
				m.Title = "the scarlet monk";
			}
			else if ( reg.IsPartOf( "the Catacombs" ) )
			{
				level = 3;
				color = 0xB95;
				named = "jade monk robe";
				m.Title = "the jade monk";
			}
			else if ( reg.IsPartOf( "the Lower Catacombs" ) )
			{
				level = 2;
				color = 0xB95;
				named = "jade monk robe";
				m.Title = "the jade monk";
			}
			else if ( reg.IsPartOf( "the Tower of Brass" ) )
			{
				color = 2413;
				named = "brass monk robe";
				m.Title = "the brass monk";
			}
			else if ( reg.IsPartOf( "the Ruins of the Black Blade" ) )
			{
				color = 0x497;
				named = "dark monk robe";
				m.Title = "the dark monk";
			}
			else if ( reg.IsPartOf( "Stonegate Castle" ) )
			{
				color = 0x541;
				named = "shadow monk robe";
				m.Title = "the shadow monk";
				m.Hue = 0x4001;
				m.BaseSoundID = 0x482;
				m.HairItemID = 0;
				m.FacialHairItemID = 0;
			}

			int myBonus = 10;
			int myMinDmg = 8;
			int myMaxDmg = 12;
			int myResist = 5;

			int RandomRoll = Utility.RandomMinMax( 1, 6 );
			if ( level > 0 ){ RandomRoll = level; }

			switch ( RandomRoll )
			{
				case 1: myBonus = 10; myMinDmg = 8;  myMaxDmg = 12; myResist = 5;  break;
				case 2: myBonus = 20; myMinDmg = 9;  myMaxDmg = 13; myResist = 10; break;
				case 3: myBonus = 30; myMinDmg = 10; myMaxDmg = 14; myResist = 15; break;
				case 4: myBonus = 40; myMinDmg = 11; myMaxDmg = 15; myResist = 20; break;
				case 5: myBonus = 50; myMinDmg = 12; myMaxDmg = 16; myResist = 25; break;
				case 6: myBonus = 60; myMinDmg = 13; myMaxDmg = 17; myResist = 30; break;
			}

			m.RawStr = m.RawStr + myBonus;
			m.RawDex = m.RawDex + myBonus;
			m.RawInt = m.RawInt + myBonus;

			m.Hits = m.HitsMax;
			m.Mana = m.ManaMax;
			m.Stam = m.StamMax;

			bc.DamageMin = myMinDmg;
			bc.DamageMax = myMaxDmg;

			bc.SetDamageType( ResistanceType.Physical, 100 );

			bc.SetResistance( ResistanceType.Physical, ( 10 + myResist ) );
			bc.SetResistance( ResistanceType.Fire, myResist );
			bc.SetResistance( ResistanceType.Cold, myResist );
			bc.SetResistance( ResistanceType.Poison, myResist );
			bc.SetResistance( ResistanceType.Energy, myResist );

			bc.SetSkill( SkillName.Searching, ( 20.0 + myBonus ) );
			bc.SetSkill( SkillName.Anatomy, ( 50.0 + myBonus ) );
			bc.SetSkill( SkillName.MagicResist, ( 20.0 + myBonus ) );
			bc.SetSkill( SkillName.Bludgeoning, ( 50.0 + myBonus ) );
			bc.SetSkill( SkillName.Fencing, ( 50.0 + myBonus ) );
			bc.SetSkill( SkillName.FistFighting, ( 50.0 + myBonus ) );
			bc.SetSkill( SkillName.Swords, ( 50.0 + myBonus ) );
			bc.SetSkill( SkillName.Tactics, ( 50.0 + myBonus ) );

			m.Fame = m.Fame * myBonus;
			m.Karma = m.Karma * myBonus;

			m.VirtualArmor = myResist;

			List<Item> belongings = new List<Item>();
			foreach( Item i in m.Backpack.Items )
			{
				belongings.Add(i);
			}
			foreach ( Item stuff in belongings )
			{
				stuff.Delete();
			}

			bc.GenerateLoot( true );

			int clothHue = Utility.RandomColor( 0 );
				if ( color > 0 ){ clothHue = color; }
				if ( IsDrow( m ) ){ clothHue = Utility.RandomDrowHue(); }
				if ( IsOrk( m ) ){ clothHue = Utility.RandomOrkHue(); }

			if ( Utility.RandomBool() )
			{
				Item robe = new VagabondRobe();
					robe.Hue = clothHue;
					robe.Name = named;
					m.AddItem( robe ); 
			}
			else if ( m.Body == 0x190 || m.Body == 605 )
			{
				Item robe = new Robe();
					robe.Hue = clothHue;
					robe.Name = named;
					m.AddItem( robe ); 
			}
			else
			{
				Item dress = new PlainDress();
					dress.Hue = clothHue;
					dress.Name = named;
					m.AddItem( dress ); 
			}

			Item sandals = new Sandals();
				sandals.Hue = Utility.RandomNeutralHue();
				m.AddItem( sandals ); 

			if ( Utility.RandomBool() )
			{
				PugilistGlove glove = new PugilistGlove();
					glove.LootType = LootType.Blessed;
					glove.Movable = false;
					switch ( Utility.RandomMinMax( 1, 12 ) )
					{
						case 1: glove.WeaponAttributes.HitLeechHits = myBonus; break;
						case 2: glove.WeaponAttributes.HitLeechStam = myBonus; break;
						case 3: glove.WeaponAttributes.HitLeechMana = myBonus; break;
						case 4: glove.WeaponAttributes.HitMagicArrow = myBonus; break;
						case 5: glove.WeaponAttributes.HitHarm = myBonus; break;
						case 6: glove.WeaponAttributes.HitFireball = myBonus; break;
						case 7: glove.WeaponAttributes.HitLightning = myBonus; break;
						case 8: glove.WeaponAttributes.HitColdArea = myBonus; break;
						case 9: glove.WeaponAttributes.HitFireArea = myBonus; break;
						case 10: glove.WeaponAttributes.HitPoisonArea = myBonus; break;
						case 11: glove.WeaponAttributes.HitEnergyArea = myBonus; break;
						case 12: glove.WeaponAttributes.HitPhysicalArea = myBonus; break;
					}
					glove.Attributes.ReflectPhysical = myBonus;
					glove.Hue = Utility.RandomNeutralHue();
					glove.MinDamage = myMinDmg + 5;
					glove.MaxDamage = myMaxDmg + 5;
					glove.Name = "monk gloves";
					glove.Hue = m.Hue;
					m.AddItem( glove );
			}
			else
			{
				QuarterStaff ring = new QuarterStaff();
					ring.LootType = LootType.Blessed;
					ring.Movable = false;
					switch ( Utility.RandomMinMax( 1, 12 ) )
					{
						case 1: ring.WeaponAttributes.HitLeechHits = myBonus; break;
						case 2: ring.WeaponAttributes.HitLeechStam = myBonus; break;
						case 3: ring.WeaponAttributes.HitLeechMana = myBonus; break;
						case 4: ring.WeaponAttributes.HitMagicArrow = myBonus; break;
						case 5: ring.WeaponAttributes.HitHarm = myBonus; break;
						case 6: ring.WeaponAttributes.HitFireball = myBonus; break;
						case 7: ring.WeaponAttributes.HitLightning = myBonus; break;
						case 8: ring.WeaponAttributes.HitColdArea = myBonus; break;
						case 9: ring.WeaponAttributes.HitFireArea = myBonus; break;
						case 10: ring.WeaponAttributes.HitPoisonArea = myBonus; break;
						case 11: ring.WeaponAttributes.HitEnergyArea = myBonus; break;
						case 12: ring.WeaponAttributes.HitPhysicalArea = myBonus; break;
					}
					ring.Attributes.ReflectPhysical = myBonus;
					ring.Hue = Utility.RandomNeutralHue();
					ring.MinDamage = myMinDmg + 5;
					ring.MaxDamage = myMaxDmg + 5;
					ring.Name = "a monk staff";
					m.AddItem( ring );
			}

			Server.Misc.IntelligentAction.GiveAdventureGear( bc );
			m.ProcessClothing();
			m.ProcessHair();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void ChooseFighter( Mobile m, string race )
		{
			BaseCreature bc = (BaseCreature)m;

			Region reg = Region.Find( bc.Home, m.Map );

			int level = 0;
			int equip = 0;

			if ( reg.IsPartOf( "Mangar's Tower" ) || reg.IsPartOf( "Mangar's Chamber" ) )
			{
				level = 5;
				equip = 1;
			}
			else if ( reg.IsPartOf( "Kylearan's Tower" ) )
			{
				level = 4;
			}
			else if ( reg.IsPartOf( "Harkyn's Castle" ) || reg.IsPartOf( "the Catacombs" ) )
			{
				level = 3;
			}
			else if ( reg.IsPartOf( "the Lower Catacombs" ) || reg.IsPartOf( "the Sewers" ) )
			{
				level = 2;
			}
			else if ( reg.IsPartOf( "the Mines" ) || reg.IsPartOf( "the Cellar" ) )
			{
				level = 1;
			}
			else if ( reg.IsPartOf( "the Vault of the Black Knight" ) )
			{
				level = 4;
				equip = 2;
			}
			else if ( reg.IsPartOf( "the Tomb of Kazibal" ) )
			{
				equip = 3;
			}
			else if ( reg.IsPartOf( "Stonegate Castle" ) )
			{
				level = 4;
				equip = 4;
			}
			else if ( reg.IsPartOf( "the Azure Castle" ) )
			{
				level = 3;
				equip = 5;
			}

			int myBonus = 10;
			int myMinDmg = 8;
			int myMaxDmg = 18;
			int myResist = 5;

			int RandomRoll = Utility.RandomMinMax( 1, 6 );
			if ( level > 0 ){ RandomRoll = level; }

			switch ( RandomRoll )
			{
				case 1: myBonus = 10; myMinDmg = 8;  myMaxDmg = 18; myResist = 5;  break;
				case 2: myBonus = 20; myMinDmg = 9;  myMaxDmg = 19; myResist = 10; break;
				case 3: myBonus = 30; myMinDmg = 10; myMaxDmg = 20; myResist = 15; break;
				case 4: myBonus = 40; myMinDmg = 11; myMaxDmg = 21; myResist = 20; break;
				case 5: myBonus = 50; myMinDmg = 12; myMaxDmg = 22; myResist = 25; break;
				case 6: myBonus = 60; myMinDmg = 13; myMaxDmg = 23; myResist = 30; break;
			}

			m.RawStr = m.RawStr + myBonus;
			m.RawDex = m.RawDex + myBonus;
			m.RawInt = m.RawInt + myBonus;

			m.Hits = m.HitsMax;
			m.Mana = m.ManaMax;
			m.Stam = m.StamMax;

			bc.DamageMin = myMinDmg;
			bc.DamageMax = myMaxDmg;

			bc.SetDamageType( ResistanceType.Physical, 100 );

			bc.SetResistance( ResistanceType.Physical, ( 10 + myResist ) );
			bc.SetResistance( ResistanceType.Fire, myResist );
			bc.SetResistance( ResistanceType.Cold, myResist );
			bc.SetResistance( ResistanceType.Poison, myResist );
			bc.SetResistance( ResistanceType.Energy, myResist );

			bc.SetSkill( SkillName.Searching, ( 20.0 + myBonus ) );
			bc.SetSkill( SkillName.Anatomy, ( 50.0 + myBonus ) );
			bc.SetSkill( SkillName.MagicResist, ( 20.0 + myBonus ) );
			bc.SetSkill( SkillName.Bludgeoning, ( 50.0 + myBonus ) );
			bc.SetSkill( SkillName.Fencing, ( 50.0 + myBonus ) );
			bc.SetSkill( SkillName.FistFighting, ( 50.0 + myBonus ) );
			bc.SetSkill( SkillName.Swords, ( 50.0 + myBonus ) );
			bc.SetSkill( SkillName.Tactics, ( 50.0 + myBonus ) );

			m.Fame = m.Fame * myBonus;
			m.Karma = m.Karma * myBonus;

			m.VirtualArmor = myResist;

			List<Item> belongings = new List<Item>();
			foreach( Item i in m.Backpack.Items )
			{
				belongings.Add(i);
			}
			foreach ( Item stuff in belongings )
			{
				stuff.Delete();
			}

			bc.GenerateLoot( true );

			if ( equip == 1 ) // MANGAR'S TOWER
			{
				m.Hue = 0x497;
				m.Title = "of Mangar's guard";
				m.HairHue = 0x47E;
				m.FacialHairHue = 0x47E;

				BoneArms piece1 = new BoneArms( );
					piece1.Hue = 0x497;
					piece1.Name = "dark guard arms";
					m.AddItem( piece1 );

				BoneChest piece2 = new BoneChest( );
					piece2.Hue = 0x497;
					piece2.Name = "dark guard tunic";
					m.AddItem( piece2 );

				BoneGloves piece3 = new BoneGloves( );
					piece3.Hue = 0x497;
					piece3.Name = "dark guard gloves";
					m.AddItem( piece3 );

				BoneLegs piece4 = new BoneLegs( );
					piece4.Hue = 0x497;
					piece4.Name = "dark guard leggings";
					m.AddItem( piece4 );

				BoneHelm piece5 = new BoneHelm( );
					piece5.Hue = 0x497;
					piece5.Name = "dark guard helm";
					m.AddItem( piece5 );

				RoyalBoots piece6 = new RoyalBoots( );
					piece6.Hue = 0x497;
					piece6.Name = "dark guard boots";
					m.AddItem( piece6 );

				Halberd piece7 = new Halberd( );
					piece7.Hue = 0x497;
					piece7.Name = "dark guard halberd";
					m.AddItem( piece7 );
			}
			else if ( equip == 2 ) // BLACK KNIGHT'S VAULT
			{
				m.Title = "the black guard";
				m.AddItem( new Boots( ) );

				ChaosShield shield = new ChaosShield( );

				switch ( Utility.Random( 10 ) )
				{
					case 0: BattleAxe weapon1 = new BattleAxe( ); weapon1.Hue = 0x497; m.AddItem( weapon1 ); break;
					case 1: Halberd weapon2 = new Halberd( ); weapon2.Hue = 0x497; m.AddItem( weapon2 ); break;
					case 2: DoubleAxe weapon3 = new DoubleAxe( ); weapon3.Hue = 0x497; m.AddItem( weapon3 ); break;
					case 3: ExecutionersAxe weapon4 = new ExecutionersAxe( ); weapon4.Hue = 0x497; m.AddItem( weapon4 ); break;
					case 4: WarAxe weapon5 = new WarAxe( ); weapon5.Hue = 0x497; m.AddItem( weapon5 ); break;
					case 5: TwoHandedAxe weapon6 = new TwoHandedAxe( ); weapon6.Hue = 0x497; m.AddItem( weapon6 ); break;
					case 6: VikingSword weapon7 = new VikingSword( ); weapon7.Hue = 0x497; m.AddItem( weapon7 ); if ( Utility.Random( 3 ) == 1 ){ shield.Hue = 0x497; m.AddItem( shield ); } break;
					case 7: ThinLongsword weapon8 = new ThinLongsword( ); weapon8.Hue = 0x497; m.AddItem( weapon8 ); if ( Utility.Random( 3 ) == 1 ){ shield.Hue = 0x497; m.AddItem( shield ); } break;
					case 8: Longsword weapon9 = new Longsword( ); weapon9.Hue = 0x497; m.AddItem( weapon9 ); if ( Utility.Random( 3 ) == 1 ){ shield.Hue = 0x497; m.AddItem( shield ); } break;
					case 9: Broadsword weapon0 = new Broadsword( ); weapon0.Hue = 0x497; m.AddItem( weapon0 ); if ( Utility.Random( 3 ) == 1 ){ shield.Hue = 0x497; m.AddItem( shield ); } break;
				}

				Robe robe = new Robe( );
					robe.Name = "black guard robe";
					robe.Hue = 0x497;
					m.AddItem( robe );

				LeatherGloves gloves = new LeatherGloves( );
					gloves.Name = "black guard gloves";
					gloves.Hue = 0x497;
					m.AddItem( gloves );

				LeatherCap cap = new LeatherCap( );
					cap.Name = "black guard cap";
					cap.Hue = 0x497;
					m.AddItem( cap );
			}
			else if ( equip == 3 ) // TOMB OF KAZIBAL
			{
				m.Hue = 0x8420;
				m.Name = "a kaztec warrior";
				m.Title = null;
				m.HairItemID = 0;
				m.FacialHairItemID = 0;

				BoneArms piece1 = new BoneArms( );
					piece1.Hue = 0x83B;
					piece1.Name = "kaztec arms";
					m.AddItem( piece1 );

				BoneGloves piece3 = new BoneGloves( );
					piece3.Hue = 0x83B;
					piece3.Name = "kaztec gloves";
					m.AddItem( piece3 );

				BoneLegs piece4 = new BoneLegs( );
					piece4.Hue = 0x83B;
					piece4.Name = "kaztec leggings";
					m.AddItem( piece4 );

				TribalMask piece5 = new TribalMask( );
					piece5.Hue = 0x83B;
					piece5.Name = "kaztec mask";
					m.AddItem( piece5 );

				if ( m.Body == 0x191 || m.Body == 606 )
				{
					LeatherBustierArms piece2 = new LeatherBustierArms( );
						piece2.Hue = 0x83B;
						piece2.Name = "kaztec bustier";
						m.AddItem( piece2 );
				}

				if ( Utility.RandomBool() )
				{
					Item cloth1 = new LoinCloth();
						cloth1.Hue = 0x83B;
						cloth1.Name = "kaztec loin cloth";
						m.AddItem( cloth1 );
				}
				else
				{
					Item cloth2 = new LeatherSkirt();
						cloth2.Hue = 0x83B;
						cloth2.Name = "kaztec skirt";
						cloth2.Layer = Layer.Waist;
						m.AddItem( cloth2 );
				}

				Spear piece7 = new Spear( );
					piece7.Name = "kaztec spear";
					m.AddItem( piece7 );
			}
			else if ( equip == 4 ) // STONEGATE CASTLE
			{
				m.Title = "the shadow guard";
				m.Hue = 0x4001;
				m.BaseSoundID = 0x482;
				m.HairItemID = 0;
				m.FacialHairItemID = 0;

				GiveBasicWepShld( m );

				Item helm = new OrcHelm();
					helm.Name = "great helm";
					helm.ItemID = 0x2645;
					m.AddItem( helm );

				Item cloth1 = new PlateArms();
					cloth1.ItemID = Utility.RandomList( 0x1410, 0x1417, 0x303, 0x304, 0x305, 0x306 );
					m.AddItem( cloth1 );

				m.AddItem( new PlateGorget() );
				m.AddItem( new PlateLegs() );
				m.AddItem( new PlateChest() );
				m.AddItem( new PlateGloves() );

				MorphingTime.ColorMyClothes( m, 0x541, 0 );
			}
			else if ( equip == 5 ) // AZURE CASTLE
			{
				m.Title = "the azure guard";

				GiveBasicWepShld( m );

				Item helm = new OrcHelm();
					helm.Name = "great helm";
					helm.ItemID = 0x2645;
					m.AddItem( helm );

				Item cloth1 = new PlateArms();
					cloth1.ItemID = Utility.RandomList( 0x1410, 0x1417, 0x303, 0x304, 0x305, 0x306 );
					m.AddItem( cloth1 );

				m.AddItem( new PlateGorget() );
				m.AddItem( new PlateLegs() );
				m.AddItem( new PlateChest() );
				m.AddItem( new PlateGloves() );

				MorphingTime.ColorMyClothes( m, 0x538, 0 );
			}
			else if ( race == "undead " )
			{
				Server.Misc.IntelligentAction.DressUpFighters( m, race, true, false, false );
			}
			else 
			{
				Server.Misc.IntelligentAction.DressUpFighters( m, race, true, false, false );
				Server.Misc.IntelligentAction.GiveAdventureGear( bc );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void PunchStun( Mobile m )
		{
			if ( ( 100 - m.PhysicalResistance ) < Utility.RandomMinMax( 1, 100 ) && Utility.RandomMinMax( 1, 5 ) == 1 )
			{
				m.Paralyze(TimeSpan.FromSeconds(Math.Min(MySettings.S_paralyzeDuration, Utility.RandomMinMax(4, 12))));
				m.Warmode = false;
				m.LocalOverheadMessage( MessageType.Emote, 0x916, true, "You are hit with a stunning punch!" );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void PoisonVictim( Mobile m, Mobile from )
		{
			bool CanPoison = false;

			int itSicks = 1;
			if ( from.Fame >= 3000 ){ itSicks = 5; }
			else if ( from.Fame >= 2500 ){ itSicks = 4; }
			else if ( from.Fame >= 2000 ){ itSicks = 3; }
			else if ( from.Fame >= 1500 ){ itSicks = 2; }

			if ( from.Hue == 0x430 && ( from is Urk || from is UrkShaman ) )
			{
				CanPoison = true;
				itSicks = 1;
			}
			else if ( from.EmoteHue == 1 )
			{
				CanPoison = true;
			}

			if ( !(Server.Items.HiddenTrap.SavingThrow( m, "Poison", false, null )) && Utility.RandomMinMax( 1, 5 ) == 1 && CanPoison == true )
			{
				switch( Utility.RandomMinMax( 1, itSicks ) )
				{
					case 1: m.ApplyPoison( m, Poison.Lesser );	break;
					case 2: m.ApplyPoison( m, Poison.Regular );	break;
					case 3: m.ApplyPoison( m, Poison.Greater );	break;
					case 4: m.ApplyPoison( m, Poison.Deadly );	break;
					case 5: m.ApplyPoison( m, Poison.Lethal );	break;
				}

				Effects.SendLocationEffect( m.Location, m.Map, 0x11A8 - 2, 16, 3, 0, 0 );
				Effects.PlaySound( m, m.Map, 0x201 );
				m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "You have been poisoned!");
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void LeapToAttacker( Mobile from, Mobile m )
		{
			if ( from != null && from.Hits > 0 && Utility.RandomMinMax( 1, 5 ) == 1 )
			{
				Region myReg = Region.Find( from.Location, from.Map );
				Region foeReg = Region.Find( m.Location, m.Map );

				bool isNearby = false;
				foreach ( Mobile foe in from.GetMobilesInRange( 1 ) )
				{
					if ( foe == m )
					{
						isNearby = true;
					}
				}

				if ( isNearby == false && myReg == foeReg )
				{
					Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
					Effects.PlaySound( from, from.Map, 0x201 );
					from.Location = m.Location;
					from.Combatant = m;
					from.Warmode = true;
					Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
					Effects.PlaySound( from, from.Map, 0x201 );
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void HideStealMove( Mobile from, Mobile m )
		{
			HideFromOthers( m, 5 );

			if ( m != null && from != null && m.Hits > 0 && from is PlayerMobile && m.InRange( from, 2 ) && m.Map == from.Map )
			{
				if ( m.Skills[SkillName.Stealing].Value >= Utility.RandomMinMax( 1, 125 ) && m.Skills[SkillName.Snooping].Value >= Utility.RandomMinMax( 1, 100 ) )
				{
					if ( Utility.RandomMinMax( 1, 5 ) == 1 )
					{
						int c = 0;

						List<Item> belongings = new List<Item>();
						foreach( Item i in from.Backpack.Items )
						{
							if ( i.LootType != LootType.Blessed && i.TotalItems == 0 ){ belongings.Add(i); c++; }
						}

						int o = Utility.RandomMinMax( 0, c );

						foreach ( Item stuff in belongings )
						{
							o++;
							if ( c == o ){ ((BaseCreature)m).PackItem( stuff ); from.LocalOverheadMessage(MessageType.Emote, 0x916, true, m.Name + " stole something from you!"); }
						}
					}
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void HideFromOthers( Mobile m, int maxChance = 4 )
		{
			if ( m != null && m.Hits > 0 )
			{
				int hits = m.HitsMax / 5;
				if ( m.Skills[SkillName.Hiding].Value >= Utility.RandomMinMax( 1, 120 ) && m.Hits <= hits && Utility.RandomMinMax( 1, maxChance ) == 1 )
				{
					Map map = m.Map;
					bool validLocation = false;
					Point3D loc = m.Location;

					for ( int j = 0; !validLocation && j < 20; ++j )
					{
						int x = m.X + Utility.Random( 6 ) + 6;
							if ( Utility.RandomBool() ){ x = m.X - Utility.Random( 6 ) - 6; }

						int y = m.Y + Utility.Random( 6 ) + 6;
							if ( Utility.RandomBool() ){ y = m.Y - Utility.Random( 6 ) - 6; }

						int z = map.GetAverageZ( x, y );

						if ( validLocation = map.CanFit( x, y, m.Z, 16, false, false ) )
							loc = new Point3D( x, y, m.Z );
						else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
							loc = new Point3D( x, y, z );
					}
					
					Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
					Effects.PlaySound( m, m.Map, 0x201 );
					m.Combatant = null;
					m.Warmode = false;
					m.CantWalk = true;
					m.Location = loc;
					m.Hidden = true;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static bool IsHiding( Mobile m )
		{
			if ( m is BaseCreature && !((BaseCreature)m).Controlled )
			{
				if ( m is AntLion || m is PhaseSpider || m is ShadowRecluse || m is ElfRogue || m is Rogue || m is OrkRogue || m is Pixie || m is Sprite )
					return true;

				if ( Server.Spells.Sixth.InvisibilitySpell.HasTimer( m ) )
					return true;
			}
			return false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void CryOut( Mobile from )
		{
			if ( Utility.RandomMinMax( 1, 3 ) == 1 )
			{
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0: from.PlaySound( from.Female ? 0x14B : 0x154 ); break;
					case 1: from.PlaySound( from.Female ? 0x14C : 0x155 ); break;
					case 2: from.PlaySound( from.Female ? 0x14D : 0x156 ); break;
					case 3: from.PlaySound( from.Female ? 0x14E : 0x157 ); break;
					case 4: from.PlaySound( from.Female ? 0x14F : 0x158 ); break;
					case 5: from.PlaySound( from.Female ? 0x14F : 0x159 ); break;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void SayHey( Mobile from )
		{
			if ( Utility.RandomMinMax(1,5) != 1 )
			{
				if ( from.Female && ( from.Body == 0x191 || from.Body == 606 ) ){ from.PlaySound( 0x31D ); }
				else if ( !from.Female && ( from.Body == 0x190 || from.Body == 605 ) ){ from.PlaySound( 0x42D ); }
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void DropReagent( Mobile player, BaseCreature monster )
		{
			if ( player == null )
				return;

			if ( monster == null )
				return;

			SlayerEntry undead = SlayerGroup.GetEntryByName( SlayerName.Silver );
			SlayerEntry exorcism = SlayerGroup.GetEntryByName( SlayerName.Exorcism );
			SlayerEntry plants = SlayerGroup.GetEntryByName( SlayerName.WeedRuin );
			SlayerEntry gargoyle = SlayerGroup.GetEntryByName( SlayerName.GargoylesFoe );
			SlayerEntry poisoner = SlayerGroup.GetEntryByName( SlayerName.ElementalHealth );
			SlayerEntry rocks = SlayerGroup.GetEntryByName( SlayerName.EarthShatter );
			SlayerEntry flame = SlayerGroup.GetEntryByName( SlayerName.FlameDousing );
			SlayerEntry water = SlayerGroup.GetEntryByName( SlayerName.NeptunesBane );

			int DropThisMuch = Server.Misc.IntelligentAction.FameBasedLevel( monster );

			int amount = Utility.RandomMinMax( DropThisMuch, ( DropThisMuch * 3 ) );

			if ( undead.Slays(monster) )
			{
				monster.PackItem( new GraveDust( amount ) );
			}
			if ( gargoyle.Slays(monster) )
			{
				monster.PackItem( new GargoyleEar( Utility.RandomMinMax( 1, 2 ) ) );
			}
			if ( monster is PoisonElemental )
			{
				monster.PackItem( new NoxCrystal( amount ) );
			}
			if ( rocks.Slays(monster) )
			{
				monster.PackItem( new PigIron( amount ) );
			}
			if ( flame.Slays(monster) )
			{
				switch ( Utility.RandomMinMax(0,1) ) 
				{
					case 0: monster.PackItem( new Brimstone( amount ) ); break;
					case 1: monster.PackItem( new SulfurousAsh( amount ) ); break;
				}
			}
			if ( water.Slays(monster) )
			{
				monster.PackItem( new SeaSalt( amount ) );
			}
			if ( (monster.Name).Contains("beetle") )
			{
				monster.PackItem( new BeetleShell( 1 ) );
			}
			if ( (monster.Name).Contains("werewolf") || (monster.Name).Contains("wolf man") )
			{
				monster.PackItem( new WerewolfClaw( Utility.RandomMinMax(1,2) ) );
			}
			if ( (monster.Name).Contains("frog") || (monster.Name).Contains("toad") )
			{
				switch ( Utility.RandomMinMax( 0, 1 ) ) 
				{
					case 0: monster.PackItem( new EyeOfToad( Utility.RandomMinMax( 1, 2 ) ) ); break;
					case 1: monster.PackItem( new DriedToad( 1 ) ); break;
				}
			}
			if ( monster is Pixie || monster is Sprite || monster is Faerie )
			{
				switch ( Utility.RandomMinMax( 0, 2 ) ) 
				{
					case 0: monster.PackItem( new FairyEgg( Utility.RandomMinMax( 1, 2 ) ) ); break;
					case 1: monster.PackItem( new PixieSkull( 1 ) ); break;
					case 2: monster.PackItem( new ButterflyWings( Utility.RandomMinMax( 1, 2 ) ) ); break;
				}
			}
			if ( (monster.Name).Contains("spider") )
			{
				monster.PackItem( new SilverWidow( 1 ) );
			}
			if ( rocks.Slays(monster) )
			{
				monster.PackItem( new PigIron( amount ) );
			}
			if ( monster is BloodElemental || exorcism.Slays(monster) )
			{
				monster.PackItem( new DaemonBlood( amount ) );
			}
			if ( plants.Slays(monster) )
			{
				int pick = Utility.RandomMinMax( 0, 9 );
				switch ( pick ) 
				{
					case 0: monster.PackItem( new MandrakeRoot( amount ) ); break;
					case 1: monster.PackItem( new Nightshade( amount ) ); break;
					case 2: monster.PackItem( new SwampBerries( amount ) ); break;
					case 3: monster.PackItem( new RedLotus( amount ) ); break;
					case 4: monster.PackItem( new Ginseng( amount ) ); break;
					case 5: monster.PackItem( new Garlic( amount ) ); break;
					case 6: monster.PackItem( new BitterRoot( amount ) ); break;
					case 7: monster.PackItem( new VioletFungus( amount ) ); break;
					case 8: monster.PackItem( new BloodRose( amount ) ); break;
					case 9: monster.PackItem( new Wolfsbane( amount ) ); break;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void DropItem( BaseCreature from )
		{
			if ( MobileUtilities.TryGetKillingPlayer( from ) == null ) return;

			int killerLuck = MobileUtilities.GetLuckFromKiller( from );

			if (	from is BoneMagi || from is SkeletalMage || from is SkeletalWizard || from is Lich || from is Vordo || from is Nazghoul || from is LichLord || from is DemiLich || 
					from is AncientLich || from is Surtaz || from is LichKing || from is UndeadDruid )
			{
				if ( from.Fame > Utility.Random( 40000 ) )
				{
					EvilSkull skull = new EvilSkull();
						skull.Name = "the skull of " + from.Name;
							if ( from.Title != null && from.Title != "" ){ skull.Name = skull.Name + " " + from.Title; skull.Hue = from.Hue; }
						from.AddItem( skull );
				}
			}

			if ( from is Lich || from is Nazghoul || from is LichLord || from is AncientLich || from is Surtaz || from is LichKing || from is DemiLich || from is UndeadDruid )
			{
				string gear = "an old";
				int Magic = 1;
				int Mgear = 3;
				int MagicHue = Utility.RandomNeutralHue();
				int MgearHue = Utility.RandomNeutralHue();

				if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Utility.RandomMinMax( 1, 4 ) == 1 )
				{
					if ( from is Lich )
					{
						gear = "a lich";
						Magic = 1;
						Mgear = 3;
					}
					else if ( from is LichLord )
					{
						gear = "a lich lord";
						Magic = 3;
						Mgear = 5;
					}
					else if ( from is Nazghoul )
					{
						gear = "a nazghoul";
						Magic = 3;
						Mgear = 5;
					}
					else if ( from is AncientLich )
					{
						gear = "an ancient";
						Magic = 5;
						Mgear = 7;
					}
					else if ( from is Surtaz )
					{
						gear = "Surtaz's";
						Magic = 7;
						Mgear = 9;
					}
					else if ( from is LichKing )
					{
						gear = "a dreaded";
						Magic = 7;
						Mgear = 9;
						MagicHue = 1150;
					}
					else if ( from is DemiLich )
					{
						gear = "a demilich";
						MagicHue = from.Hunger;
						MgearHue = from.Thirst;
						Magic = 3;
						Mgear = 5;

						if ( from.Title == "the crypt thing" ){		gear = "a crypt";	Magic = 5; }
						else if ( from.Title == "the dark lich" ){	gear = "a dark";	Magic = 7; }
					}
					else if ( from is UndeadDruid )
					{
						gear = "a dark druid";
						Magic = 3;
						Mgear = 5;
						MagicHue = 0x497;
						MgearHue = 0x497;
					}

					switch( Utility.RandomMinMax( 0, 1 ) )
					{
						case 0: 
							NecromancerRobe robe = new NecromancerRobe(); if ( Utility.RandomBool() ){ robe.Resource = CraftResource.VileFabric; }
								robe.Name = gear + " robe";
								robe.Hue = MagicHue;
								robe.Attributes.CastRecovery = Magic;
								robe.Attributes.CastSpeed = Magic;
								robe.Attributes.LowerManaCost = 4 + Magic;
								robe.Attributes.LowerRegCost = 4 + Magic;
								robe.Attributes.SpellDamage = 2 + Magic;
								from.AddItem( robe );
							break;
						case 1: 
							QuarterStaff staff = new QuarterStaff(); if ( Utility.RandomBool() ){ staff.Resource = CraftResource.GhostTree; }
								staff.Name = gear + " staff";
								staff.ItemID = Utility.RandomList( 0xDF0, 0x13F8, 0xE89, 0x2D25 );
								staff.Hue = MgearHue;
								staff.WeaponAttributes.HitHarm = 5 * Mgear;
								staff.MaxHitPoints = 100;
								staff.HitPoints = 100;
								staff.MinDamage = staff.MinDamage + Mgear;
								staff.MaxDamage = staff.MaxDamage + Mgear;
								staff.SkillBonuses.SetValues( 0, SkillName.Bludgeoning, (2*Mgear) );
								((BaseCreature)from).PackItem( staff );
							break;
					}
				}
			}
			else if ( from is VampireLord || from is Vampire || from is VampirePrince || from is Dracula )
			{
				string gear = "a vampire";
				int Magic = 1;
				int Mgear = 3;
				int MagicHue = 0x497;
				int MgearHue = 0x485;

				if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Utility.RandomMinMax( 1, 4 ) == 1 )
				{
					if ( from is Vampire )
					{
						gear = "a vampire";
						Magic = 1;
						Mgear = 3;
					}
					else if ( from is VampireLord )
					{
						gear = "a vampire lord";
						Magic = 3;
						Mgear = 5;
					}
					else if ( from is VampirePrince )
					{
						gear = "a vampire prince";
						Magic = 5;
						Mgear = 7;
					}
					else if ( from is Dracula )
					{
						gear = "Dracula's";
						Magic = 7;
						Mgear = 9;
					}

					switch( Utility.RandomMinMax( 0, 1 ) )
					{
						case 0: 
							VampireRobe robe = new VampireRobe(); if ( Utility.RandomBool() ){ robe.Resource = CraftResource.VileFabric; }
								robe.Name = gear + " robe";
								robe.Hue = MagicHue;
								robe.Resistances.Cold = 3 + Mgear;
								robe.Attributes.DefendChance = 3 + Mgear;
								robe.Attributes.BonusStr = 1 + Mgear;
								robe.Attributes.NightSight = 1;
								robe.Attributes.RegenHits = 1 + Mgear;
								from.AddItem( robe );
							break;
						case 1: 
							Cloak cloak = new Cloak(); if ( Utility.RandomBool() ){ cloak.Resource = CraftResource.VileFabric; }
								cloak.Name = gear + " cloak";
								cloak.Hue = MgearHue;
								cloak.Resistances.Cold = 3 + Magic;
								cloak.Attributes.DefendChance = 3 + Magic;
								cloak.Attributes.BonusStr = 1 + Magic;
								cloak.Attributes.NightSight = 1;
								cloak.Attributes.RegenHits = 1 + Magic;
								from.AddItem( cloak );
							break;
					}
				}
			}
			else if ( from.EmoteHue == 11 && from.Title == "the mad archmage" )
			{
				switch( Utility.RandomMinMax( 0, 1 ) )
				{
					case 0: 
						Robe robe = new Robe( ); if ( Utility.RandomBool() ){ robe.Resource = CraftResource.MysteriousFabric; }
							robe.Hue = 0xA2A;
							robe.Name = "robe of the mad archmage";
							robe.Attributes.SpellDamage = 35;
							robe.Attributes.CastRecovery = 1;
							robe.Attributes.CastSpeed = 1;
							robe.Attributes.LowerManaCost = 30;
							robe.Attributes.LowerRegCost = 30;
							from.AddItem( robe );
						break;
					case 1: 
						WizardsHat hat = new WizardsHat( ); if ( Utility.RandomBool() ){ hat.Resource = CraftResource.MysteriousFabric; }
							hat.Hue = 0xA2A;
							hat.Name = "hat of the mad archmage";
							hat.Attributes.SpellDamage = 25;
							hat.Attributes.CastRecovery = 1;
							hat.Attributes.CastSpeed = 1;
							hat.Attributes.LowerManaCost = 20;
							hat.Attributes.LowerRegCost = 20;
							from.AddItem( hat );
						break;
				}
			}
			else if ( from.EmoteHue == 16 )
			{
				switch( Utility.RandomMinMax( 0, 1 ) )
				{
					case 0: 
						Robe robe = new Robe( ); if ( Utility.RandomBool() ){ robe.Resource = CraftResource.ArcticFabric; }
							robe.Hue = 0x482;
							robe.Name = "ice queen robe";
							robe.Attributes.RegenMana = 5;
							robe.Attributes.ReflectPhysical = 20;
							robe.Attributes.SpellDamage = 35;
							from.AddItem( robe );
						break;
					case 1: 
						WizardsHat hat = new WizardsHat( ); if ( Utility.RandomBool() ){ hat.Resource = CraftResource.ArcticFabric; }
							hat.Hue = 0x482;
							hat.Name = "ice queen hat";
							hat.Attributes.RegenMana = 3;
							hat.Attributes.ReflectPhysical = 10;
							hat.Attributes.SpellDamage = 15;
							from.AddItem( hat );
						break;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void BeforeMyBirth( Mobile from )
		{
			int PackGear = 1;

			Region reg = Region.Find( from.Location, from.Map );

			if ( reg.IsPartOf( "the Ice Queen Fortress" ) && from is EvilMageLord )
			{
				from.EmoteHue = 16;
				PackGear = 0;
			}
			else if ( from.EmoteHue == 11 ){} // EVIL MAGE LORD SKIP
			else if ( from is DeadWizard )
			{
				from.EmoteHue = 0;
				PackGear = 0;
			}
			else if ( reg.IsPartOf( "Stonegate Castle" ) && ( from is EvilMage || from is ElfMage || from is OrkMage ) && from.X >= 6326 && from.Y >= 2377 && from.X <= 6509 && from.Y <= 2505 )
			{
				from.EmoteHue = 6;
			}
			else if ( reg.IsPartOf( "Stonegate Castle" ) && ( from is EvilMage || from is ElfMage || from is OrkMage ) )
			{
				from.EmoteHue = 0;
				PackGear = 0;
			}
			else if ( reg.IsPartOf( "the Hidden Valley" ) && ( from is EvilMage || from is ElfMage || from is OrkMage ) )
			{
				from.EmoteHue = 6;
				PackGear = 0;
			}
			else if ( reg.IsPartOf( "the Azure Castle" ) && ( from is EvilMage || from is ElfMage || from is OrkMage ) )
			{
				from.EmoteHue = 0;
				PackGear = 0;
			}
			else if ( from is SkeletalGargoyle || from is BoneMagi || from is SkeletalMage || from is SkeletalWizard || from is Lich || from is Vordo || from is Nazghoul || from is LichLord || from is DemiLich || from is AncientLich || from is Surtaz || from is LichKing )
			{
				from.EmoteHue = 13;
				PackGear = 0;
			}
			else if ( from is UndeadDruid )
			{
				from.EmoteHue = 14;
				PackGear = 0;
			}
			else if ( from is VampirePrince || from is Vampire || from is VampireLord || from is Dracula )
			{
				from.EmoteHue = 15;
				PackGear = 0;
			}
			else if (	reg.IsPartOf( "the Ruins of the Black Blade" ) || 
						reg.IsPartOf( typeof( BardDungeonRegion ) ) )
			{
				from.EmoteHue = 0;
			}
			else if (	from.Map == Map.IslesDread || 
						reg.IsPartOf( "the Blood Temple" ) )
			{
				from.EmoteHue = 2;
				PackGear = 0;
			}
			else if (	reg.IsPartOf( "Dungeon Hythloth" ) )
			{
				from.EmoteHue = 3;
			}
			else if (
						( from.X >= 6362 && from.Y >= 3854 && from.X <= 6372 && from.Y <= 3864 && from.Map == Map.Lodor ) || 
						( from.X >= 6442 && from.Y >= 3821 && from.X <= 6452 && from.Y <= 3831 && from.Map == Map.Lodor ) || 
						reg.IsPartOf( "the Hall of the Mountain King" ) || 
						reg.IsPartOf( "Dungeon Shame" )
			)
			{
				from.EmoteHue = 4;
			}
			else if (
						( from.X >= 6312 && from.Y >= 3538 && from.X <= 6397 && from.Y <= 3628 && from.Map == Map.Sosaria ) || 
						( from.X >= 6266 && from.Y >= 469 && from.X <= 6276 && from.Y <= 479 && from.Map == Map.Lodor ) || 
						( from.X >= 6272 && from.Y >= 534 && from.X <= 6282 && from.Y <= 544 && from.Map == Map.Lodor ) || 
						( from.X >= 6309 && from.Y >= 578 && from.X <= 6319 && from.Y <= 588 && from.Map == Map.Lodor ) || 
						( from.X >= 6203 && from.Y >= 661 && from.X <= 6213 && from.Y <= 671 && from.Map == Map.Sosaria ) || 
						( from.X >= 6331 && from.Y >= 145 && from.X <= 6341 && from.Y <= 155 && from.Map == Map.Sosaria ) || 
						( from.X >= 6284 && from.Y >= 3598 && from.X <= 6294 && from.Y <= 3608 && from.Map == Map.Lodor ) || 
						( from.X >= 28 && from.Y >= 3294 && from.X <= 101 && from.Y <= 3329 && from.Map == Map.SavagedEmpire ) || 
						Server.Misc.Worlds.IsCrypt( from.Location, from.Map )
			)
			{
				from.EmoteHue = 5;
			}
			else if (

						( from.X >= 6590 && from.Y >= 373 && from.X <= 6629 && from.Y <= 465 && from.Map == Map.Lodor ) || 
						( Utility.RandomMinMax( 1, 4 ) > 1 && Server.Misc.Worlds.TestTile ( from.Map, from.X, from.Y, "forest" ) ) || 
						reg.IsPartOf( "the Valley of Dark Druids" ) )
			{
				from.EmoteHue = 6;
			}
			else if (
						( from is EvilMage || from is ElfMage || from is OrkMage ) && 
						(
							( from.X >= 6177 && from.Y >= 256 && from.X <= 6224 && from.Y <= 297 && from.Map == Map.Sosaria ) || 
							( from.X >= 6359 && from.Y >= 508 && from.X <= 6451 && from.Y <= 564 && from.Map == Map.Lodor ) || 
							Server.Misc.Worlds.TestTile ( from.Map, from.X, from.Y, "snow" ) || 
							Server.Misc.Worlds.IsIceDungeon( from.Location, from.Map )
						)
			)
			{
				from.EmoteHue = 7;
			}
			else if (
						( from.X >= 6184 && from.Y >= 496 && from.X <= 6208 && from.Y <= 520 && from.Map == Map.Sosaria ) || 
						( from.X >= 6314 && from.Y >= 250 && from.X <= 6339 && from.Y <= 285 && from.Map == Map.Sosaria ) || 
						( from.X >= 6459 && from.Y >= 460 && from.X <= 6481 && from.Y <= 477 && from.Map == Map.Sosaria ) || 
						( from.X >= 3094 && from.Y >= 3582 && from.X <= 3118 && from.Y <= 3602 && from.Map == Map.Lodor ) || 
						Server.Misc.Worlds.IsFireDungeon( from.Location, from.Map ) || 
						reg.IsPartOf( "the Tower of Brass" ) 
			)
			{
				from.EmoteHue = 8;
			}
			else if (
						( from.X >= 6289 && from.Y >= 119 && from.X <= 6299 && from.Y <= 129 && from.Map == Map.Sosaria ) || 
						( from.X >= 6312 && from.Y >= 125 && from.X <= 6326 && from.Y <= 133 && from.Map == Map.Sosaria ) || 
						Server.Misc.Worlds.TestTile ( from.Map, from.X, from.Y, "swamp" ) || 
						reg.IsPartOf( "the Temple of Osirus" ) || 
						reg.IsPartOf( "the Dragon's Maw" ) ||
						reg.IsPartOf( "Dungeon Destard" ) 
			)
			{
				from.EmoteHue = 9;
			}
			else if ( Server.Misc.Worlds.IsSeaDungeon( from.Location, from.Map ) || 
						Server.Misc.Worlds.TestOcean ( from.Map, from.X, from.Y, 15 ) )
			{
				from.EmoteHue = 10;
			}
			else if ( reg.IsPartOf( "the Tomb of Kazibal" ) )
			{
				from.EmoteHue = 17;
			}
			else // RANDOM MAGE
			{
				switch ( Utility.RandomMinMax( 0, 10 ) )
				{
					case 0: from.EmoteHue = 0; break; // Mages with no Summoning
					case 1: from.EmoteHue = 1; break; // Traditional Mages
					case 2: from.EmoteHue = 3; break; // Demonologists
					case 3: from.EmoteHue = 4; break; // Elementalists
					case 4: from.EmoteHue = 5; break; // Necromancers
					case 5: from.EmoteHue = 6; break; // Druids
					case 6: from.EmoteHue = 7; break; // Ice Wizards
					case 7: from.EmoteHue = 8; break; // Fire Wizards
					case 8: from.EmoteHue = 9; break; // Serpent Mages
					case 9: from.EmoteHue = 10; break; // Water Wizards
					case 10: from.EmoteHue = 12; break; // Insane Wizards
				}
			}

			if ( from.EmoteHue == 0 && reg.IsPartOf( "Stonegate Castle" ) )
			{
				from.Title = "the shadow priest";
				from.Hue = 0x4001;
				from.HairItemID = 0;
				from.FacialHairItemID = 0;

				for ( int i = 0; i < from.Items.Count; ++i )
				{
					Item item = from.Items[i];

					if ( item is BaseClothing || item is BaseArmor )
						item.Hue = 0x541;
				}
			}
			if ( reg.IsPartOf( "the Azure Castle" ) && ( from is OrkMage || from is DeadWizard ) )
			{
				from.Title = from.Title + " of azure";

				for ( int i = 0; i < from.Items.Count; ++i )
				{
					Item item = from.Items[i];

					if ( item is BaseClothing || item is BaseArmor )
						item.Hue = 0x538;
				}
			}
			else if ( from.EmoteHue == 2 )
			{
				from.Title = from.Title + " of blood";
				from.Hue = 0x4AA;
				from.HairHue = 0x96C;
				from.FacialHairHue = 0x96C;

				for ( int i = 0; i < from.Items.Count; ++i )
				{
					Item item = from.Items[i];

					if ( item is Hair || item is Beard )
						item.Hue = 0x96C;
					else if ( item is BaseClothing || item is BaseArmor )
						item.Hue = 0x84D;
				}
			}
			else if ( from.EmoteHue == 3 )
			{
				from.Title = from.Title + " of demons";
			}
			else if ( from.EmoteHue == 4 )
			{
				from.Title = from.Title + " of elements";
			}
			else if ( from.EmoteHue == 5 )
			{
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0: from.Title = from.Title + " of death"; break;
					case 1: from.Title = from.Title + " of the grave"; break;
					case 2: from.Title = from.Title + " of the dead"; break;
				}
				
				Server.Misc.MorphingTime.TurnToNecromancer( from );
			}
			else if ( from.EmoteHue == 6 )
			{
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0: from.Title = from.Title + " of the woods"; break;
					case 1: from.Title = from.Title + " of the forest"; break;
					case 2: from.Title = from.Title + " of the glade"; break;
				}

				for ( int i = 0; i < from.Items.Count; ++i )
				{
					Item item = from.Items[i];

					if ( item is BaseClothing || item is BaseArmor )
						item.Hue = Utility.RandomYellowHue();
				}
			}
			else if ( from.EmoteHue == 7 )
			{
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0: from.Title = from.Title + " of ice"; break;
					case 1: from.Title = from.Title + " of frost"; break;
					case 2: from.Title = from.Title + " of the snow"; break;
				}

				from.Hue = 0x83E8;
				from.HairHue = 0;
				from.FacialHairHue = 0;

				for ( int i = 0; i < from.Items.Count; ++i )
				{
					Item item = from.Items[i];

					if ( item is Hair || item is Beard )
						item.Hue = 0;
					else if ( item is BaseClothing || item is BaseArmor )
						item.Hue = Utility.RandomList( 0xB78, 0xB33, 0xB34, 0xB35, 0xB36, 0xB37, 0xAF3 );
				}
			}
			else if ( from.EmoteHue == 8 )
			{
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0: from.Title = from.Title + " of fire"; break;
					case 1: from.Title = from.Title + " of the flame"; break;
					case 2: from.Title = from.Title + " of flame"; break;
				}

				for ( int i = 0; i < from.Items.Count; ++i )
				{
					Item item = from.Items[i];

					if ( item is BaseClothing || item is BaseArmor )
						item.Hue = Utility.RandomList( 0xB73, 0xB71, 0xB17, 0xAFA, 0xAC8, 0x986 );
				}

				from.AddItem( new LighterSource() );
			}
			else if ( from.EmoteHue == 9 )
			{
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0: from.Title = from.Title + " of snakes"; break;
					case 1: from.Title = from.Title + " of venom"; break;
					case 2: from.Title = from.Title + " of serpents"; break;
				}

				int HairColor = Utility.RandomGreenHue();
				from.Hue = Utility.RandomGreenHue();
				from.HairHue = HairColor;
				from.FacialHairHue = HairColor;

				for ( int i = 0; i < from.Items.Count; ++i )
				{
					Item item = from.Items[i];

					if ( item is Hair || item is Beard )
						item.Hue = HairColor;
					else if ( item is BaseClothing || item is BaseArmor )
						item.Hue = Utility.RandomGreenHue();
				}
			}
			else if ( from.EmoteHue == 10 )
			{
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0: from.Title = from.Title + " of the sea"; break;
					case 1: from.Title = from.Title + " of the deep"; break;
					case 2: from.Title = from.Title + " of the lake"; break;
				}

				int HairColor = Utility.RandomBlueHue();
				from.Hue = Utility.RandomGreenHue();
				from.HairHue = HairColor;
				from.FacialHairHue = HairColor;

				for ( int i = 0; i < from.Items.Count; ++i )
				{
					Item item = from.Items[i];

					if ( item is Hair || item is Beard )
						item.Hue = HairColor;
					else if ( item is BaseClothing || item is BaseArmor )
						item.Hue = Utility.RandomColor( 2 );
				}
			}
			else if ( from.EmoteHue == 16 )
			{
				BaseCreature bsct = (BaseCreature)from;

				from.Body = 0x191; 
				from.Female = true;
				from.Name = NameList.RandomName( "female" );
				from.Title = "the ice queen";

				from.Hue = 0x47E;
				Utility.AssignRandomHair( from );
				from.HairHue = 0x47F;
				from.FacialHairItemID = 0;

				for ( int i = 0; i < from.Items.Count; ++i )
				{
					Item item = from.Items[i];

					if ( item is BaseClothing || item is BaseArmor )
					{
						item.Hue = 0x482;
						item.LootType = LootType.Blessed;
					}
					else if ( item is BaseWeapon )
						item.Delete();
				}

				((BaseCreature)from).PackItem( new GlacialStaff() );

				bsct.RawStr = 305;
				bsct.RawDex = 115;
				bsct.RawInt = 1045;

				bsct.Hits = bsct.HitsMax;
				bsct.Stam = bsct.StamMax;
				bsct.Mana = bsct.ManaMax;

				bsct.DamageMin = 15;
				bsct.DamageMax = 27;

				bsct.PhysicalDamage = 20;
				bsct.ColdDamage = 40;
				bsct.EnergyDamage = 40;

				bsct.PhysicalResistanceSeed = 50;
				bsct.FireResistSeed = 0;
				bsct.ColdResistSeed = 90;
				bsct.PoisonResistSeed = 50;
				bsct.EnergyResistSeed = 10;

				for( int i = 0; i < bsct.Skills.Length; i++ )
				{
					Skill skill = (Skill)bsct.Skills[i];

					if ( skill.Base > 0.0 )
						skill.Base = 125.0;
				}

				bsct.Fame = 23000;
				bsct.Karma = -23000;

				from.VirtualArmor = 60;
			}
			else if ( from.EmoteHue == 12 )
			{
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0: from.Title = from.Title + " of insanity"; break;
					case 1: from.Title = from.Title + " of dementia"; break;
					case 2: from.Title = from.Title + " of mania"; break;
					case 3: from.Title = from.Title + " of lunacy"; break;
					case 4: from.Title = from.Title + " of madness"; break;
					case 5: from.Title = from.Title + " of hysteria"; break;
				}

				for ( int i = 0; i < from.Items.Count; ++i )
				{
					Item item = from.Items[i];

					if ( item is BaseClothing || item is BaseArmor )
						item.Hue = Utility.RandomColor( 0 );
				}
			}
			else if ( from.EmoteHue == 17 )
			{
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0: from.Title = from.Title + " of the sands"; break;
					case 1: from.Title = from.Title + " of the desert"; break;
					case 2: from.Title = from.Title + " of the wastes"; break;
					case 3: from.Title = from.Title + " of the barrens"; break;
					case 4: from.Title = from.Title + " of the wasteland"; break;
					case 5: from.Title = from.Title + " of the badlands"; break;
				}

				for ( int i = 0; i < from.Items.Count; ++i )
				{
					Item item = from.Items[i];

					if ( item is BaseClothing || item is BaseArmor )
						item.Hue = 0x83B;
				}
			}

			if ( PackGear == 1 ){ Server.Misc.IntelligentAction.GiveAdventureGear( (BaseCreature)from ); }

			WizardStaff caster = new WizardStaff();
				if ( from.FindItemOnLayer( Layer.OneHanded ) != null ) { from.FindItemOnLayer( Layer.OneHanded ).Delete(); }
				if ( from.FindItemOnLayer( Layer.TwoHanded ) != null ) { from.FindItemOnLayer( Layer.TwoHanded ).Delete(); }
				((BaseCreature)from).SetSkill( SkillName.Marksmanship, from.Skills[SkillName.Magery].Value );
				if ( Utility.RandomBool() )
				{
					caster.Name = "staff";
					caster.ItemID = Utility.RandomList( 0xDF0, 0x13F8, 0xE89, 0x2D25, 0x26BC, 0x26C6, 0xDF2, 0xDF3, 0xDF4, 0xDF5, 0x269D, 0x269E );
					if ( caster.ItemID == 0x26BC || caster.ItemID == 0x26C6 ){ caster.Name = "scepter"; }
					if ( caster.ItemID == 0x269D || caster.ItemID == 0x269E ){ caster.Name = "skull scepter"; }
					if ( caster.ItemID == 0xDF2 || caster.ItemID == 0xDF3 || caster.ItemID == 0xDF4 || caster.ItemID == 0xDF5 ){ caster.Name = "magic wand"; }
				}
				else
				{
					caster.ItemID = 0x13C6;
					caster.Hue = from.Hue;
					caster.Name = "wizard gloves";
				}
				caster.LootType = LootType.Blessed;
				caster.Attributes.SpellChanneling = 1;
				caster.damageType = Utility.RandomMinMax( 0, 4 );
				if ( from.EmoteHue == 7 ){ caster.damageType = 2; } 		// Ice Wizards
				else if ( from.EmoteHue == 8 ){ caster.damageType = 1; } 	// Fire Wizards
				else if ( from.EmoteHue == 9 ){ caster.damageType = 4; } 	// Serpent Mages
				from.AddItem( caster );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void BurnAway( Mobile from )
		{
			from.PlaySound( 0x208 );

			Point3D fire1 = new Point3D( ( from.X-1 ), ( from.Y ), from.Z );
			Point3D fire2 = new Point3D( ( from.X+1 ), ( from.Y ), from.Z );
			Point3D fire3 = new Point3D( ( from.X-1 ), ( from.Y-1 ), from.Z );
			Point3D fire4 = new Point3D( ( from.X+1 ), ( from.Y-1 ), from.Z );
			Point3D fire5 = new Point3D( ( from.X ), ( from.Y-1 ), from.Z );
			Point3D fire6 = new Point3D( ( from.X-1 ), ( from.Y+1 ), from.Z );
			Point3D fire7 = new Point3D( ( from.X+1 ), ( from.Y+1 ), from.Z );
			Point3D fire8 = new Point3D( ( from.X ), ( from.Y+1 ), from.Z );
			Point3D fire9 = new Point3D( ( from.X ), ( from.Y ), from.Z );

			Effects.SendLocationEffect( fire1, from.Map, 0x3709, 30, 10 );
			Effects.SendLocationEffect( fire2, from.Map, 0x3709, 30, 10 );
			Effects.SendLocationEffect( fire3, from.Map, 0x3709, 30, 10 );
			Effects.SendLocationEffect( fire4, from.Map, 0x3709, 30, 10 );
			Effects.SendLocationEffect( fire5, from.Map, 0x3709, 30, 10 );
			Effects.SendLocationEffect( fire6, from.Map, 0x3709, 30, 10 );
			Effects.SendLocationEffect( fire7, from.Map, 0x3709, 30, 10 );
			Effects.SendLocationEffect( fire8, from.Map, 0x3709, 30, 10 );
			Effects.SendLocationEffect( fire9, from.Map, 0x3709, 30, 10 );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void FizzleSpell( Mobile m )
		{
			m.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502632 ); // The spell fizzles.

			if ( m.Player )
			{
				m.FixedParticles( 0x3735, 1, 30, 9503, EffectLayer.Waist );
				m.PlaySound( 0x5C );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void SaySomethingWhenAttacking( Mobile from, Mobile m )
		{
			if ( from.Name != "a sailor" && from.Name != "a pirate" && from.Name != "a follower" && m != null && from.EmoteHue != 505 && ((BaseCreature)from).ControlSlots != 666 && ((BaseCreature)from).GetMaster() == null && Utility.Random( 5 ) == 1 )
			{
				if ( from.SpeechHue < 1 ){ from.SpeechHue = Utility.RandomTalkHue(); }

				if ( m is BaseCreature )
					m = ((BaseCreature)m).GetMaster();

				if ( m is PlayerMobile )
				{
					if ( from is Exodus )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("I will vanquish your existence from all time!"); break;
							case 1: from.Say("" + m.Name + ", prepare to meet your end!"); break;
							case 2: from.Say("You cannot stop the destruction I will soon unleash!"); break;
							case 3: from.Say("My diligence will be your ultimate doom!"); break;
						};
					}
					else if ( from is FleshGolem || from is AncientFleshGolem )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("I am fearless, and therefore powerful!"); break;
							case 1: from.Say("I am a monster, cut off from all the world!"); break;
							case 2: from.Say("To be whole again, I must destroy you!"); break;
							case 3: from.Say("Fell the wrath of my master!"); break;
						};
					}
					else if ( from is BloodDemigod )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Foolish mortal!"); break;
							case 1: from.Say("I will summon your gore to crawl these halls!"); break;
							case 2: from.Say("Your life only feeds my own!"); break;
							case 3: from.Say("Let this be your final battle!"); break;
						};
					}
					else if ( from is Balron 
							|| from is Devil 
							|| from is BlackGateDemon 
							|| from is AbysmalDaemon 
							|| from is Xurtzar 
							|| from is TitanPyros 
							|| from is Marilith 
							|| from is Daemonic 
							|| from is Archfiend 
							|| from is Fiend 
							|| from is Daemon 
							|| from is DaemonTemplate 
							|| from is BloodDemon )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Your soul will be mine!"); break;
							case 1: from.Say("I will use your corpse to feed my minions!"); break;
							case 2: from.Say("Do you think you can slay one such as me?!"); break;
							case 3: from.Say("I look forward to torturing your soul, " + m.Name + "!"); break;
						};
					}
					else if ( from is DeepSeaDevil || from is DemonOfTheSea )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Your soul will soon be one with the deep!"); break;
							case 1: from.Say("You dare face the power of the sea?!"); break;
							case 2: from.Say("Are you ready to serve me in the depths, " + m.Name + "?!"); break;
							case 3: from.Say("I will drag your corpse into the sea!"); break;
						};
					}
					else if ( from is IceDevil )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Your soul will soon be encased in ice!"); break;
							case 1: from.Say("You dare face my glacial power?!"); break;
							case 2: from.Say("Are your bones cold yet, " + m.Name + "?!"); break;
							case 3: from.Say("I will freeze your blood and shatter your soul!"); break;
						};
					}
					else if ( from is Succubus )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Your blood smells sweet!"); break;
							case 1: from.Say("Are you ready to give yourself to me?!"); break;
							case 2: from.Say("Your life only feeds my own!"); break;
							case 3: from.Say("You will make me young again, " + m.Name + "!"); break;
						};
					}
					else if ( from is Satan )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Your soul will serve me well!"); break;
							case 1: from.Say("I will break a spirit such as yours!"); break;
							case 2: from.Say("Do you feel the power of hell on you?!"); break;
							case 3: from.Say("Your soul will be mine, " + m.Name + "!"); break;
						};
					}
					else if ( from is VampiricDragon )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("I will drain every breath of life from you!"); break;
							case 1: from.Say("I can smell the blood from your wounds!"); break;
							case 2: from.Say("Fool...I cannot kill what is dead?!"); break;
							case 3: from.Say("Your corpse will rise and serve me, " + m.Name + "!"); break;
						};
					}
					else if ( from is ShadowWyrm )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("I will take you from the light!"); break;
							case 1: from.Say("I can feel the darkness filling you!"); break;
							case 2: from.Say("Fool...you can never bring me to the light!"); break;
							case 3: from.Say("Your life will end in darkness, " + m.Name + "!"); break;
						};
					}
					else if ( from is AshDragon || from is VolcanicDragon )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("I will leave your corpse as ashes!"); break;
							case 1: from.Say("I can smell your burning flesh!"); break;
							case 2: from.Say("Fool...you cannot survive the flames!"); break;
							case 3: from.Say("Cinders will be all that is left of you, " + m.Name + "!"); break;
						};
					}
					else if ( from is BottleDragon
						 || from is CaddelliteDragon
						 || from is CrystalDragon
						 || from is DragonKing
						 || from is SlasherOfVoid
						 || from is ElderDragon
						 || from is RadiationDragon
						 || from is VoidDragon
						 || from is PrimevalAbysmalDragon
						 || from is PrimevalAmberDragon
						 || from is PrimevalBlackDragon
						 || from is PrimevalDragon
						 || from is PrimevalFireDragon
						 || from is PrimevalGreenDragon
						 || from is PrimevalNightDragon
						 || from is PrimevalRedDragon
						 || from is PrimevalRoyalDragon
						 || from is PrimevalRunicDragon
						 || from is PrimevalSeaDragon
						 || from is PrimevalSilverDragon
						 || from is PrimevalStygianDragon
						 || from is PrimevalVolcanicDragon
						 || from is AncientWyrm )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("I have slain those mightier than you, " + m.Name + "!"); break;
							case 1: from.Say("You will make me an excellent meal!"); break;
							case 2: from.Say("Many have died trying to take what is mine!"); break;
							case 3: from.Say("I will swallow you whole, " + m.Name + "!"); break;
						};
					}
					else if ( from is FireGargoyle
						 || from is Gargoyle
						 || from is GargoyleAmethyst
						 || from is GargoyleEmerald
						 || from is GargoyleOnyx
						 || from is GargoyleRuby
						 || from is GargoyleSapphire
						 || from is CodexGargoyleA
						 || from is CodexGargoyleB
						 || from is StoneGargoyle
						 || from is GargoyleWarrior
						 || from is StygianGargoyle
						 || from is StygianGargoyleLord
						 || from is AncientGargoyle
						 || from is MutantGargoyle
						 || from is CosmicGargoyle
						 || from is GargoyleMarble )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Esaeu lizz gia xes zes soth!"); break;
							case 1: from.Say("Dnadona qae zaaq esaeun doom!"); break;
							case 2: from.Say("I lizz raeq chq esaeu xaed za!"); break;
							case 3: from.Say("Dnadona qae gia, " + m.Name + "!"); break;
						};
					}
					else if ( from is ZornTheBlacksmith )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("You will never have the secrets of the ore!"); break;
							case 1: from.Say("You should leave before I crush you!"); break;
							case 2: from.Say("Feel the power of my hammer!"); break;
							case 3: from.Say("I will tell all on how I crushed " + m.Name + "!"); break;
						};
					}
					else if ( from is OrkDemigod )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Kneel before me you puny creature!"); break;
							case 1: from.Say("Feel the might of the orks!"); break;
							case 2: from.Say("I will become your new god, " + m.Name + "!"); break;
							case 3: from.Say("I have slain those more powerful than you!"); break;
						};
					}
					else if ( from is TrollWitchDoctor
						 || from is Troll
						 || from is SwampTroll
						 || from is SeaTroll
						 || from is FrostTrollShaman
						 || from is FrostTroll )
					{
						string organ = "spleen";
						switch ( Utility.Random( 4 ))		   
						{
							case 0: organ = "spleen"; break;
							case 1: organ = "heart"; break;
							case 2: organ = "liver"; break;
							case 3: organ = "guts"; break;
						};
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Me will be eating your " + organ + " me thinks!"); break;
							case 1: from.Say("Me see you living no longer!"); break;
							case 2: from.Say("You will be dead by me hand!"); break;
							case 3: from.Say("Me will be feasting on your bones soon!"); break;
						};
					}
					else if ( from is AncientEttin
						 || from is EttinShaman
						 || from is Ettin
						 || from is ArcticEttin )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("I smash you to pulp!"); break;
							case 1: from.Say("I will smash you into dirt!"); break;
							case 2: from.Say("You will make great feast for us!"); break;
							case 3: from.Say("You leave our land now!"); break;
						};
					}
					else if ( from is Titan
						 || from is ElderTitan
						 || from is CloudGiant
						 || from is StormGiant )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Feel the wrath of the storm!"); break;
							case 1: from.Say("I unleash the storms on you!"); break;
							case 2: from.Say("This will be your final battle, " + m.Name + "!"); break;
							case 3: from.Say("You think you can defeat me?!"); break;
						};
					}
					else if ( from is Dragonogre
						 || from is TundraOgre
						 || from is OgreMagi
						 || from is OgreLord
						 || from is Ogre
						 || from is ArcticOgreLord
						 || from is AbysmalOgre
						 || from is Neanderthal
						 || from is HillGiant
						 || from is HillGiantShaman )
					{
						string organ = "arm";
						switch ( Utility.Random( 4 ))		   
						{
							case 0: organ = "arms"; break;
							case 1: organ = "legs"; break;
							case 2: organ = "bones"; break;
							case 3: organ = "corpse"; break;
						};
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Me hit, you die!"); break;
							case 1: from.Say("You no match for me!"); break;
							case 2: from.Say("Me make soup with your " + organ + "!"); break;
							case 3: from.Say("You weak, me strong!"); break;
						};
					}
					else if ( from is IceGiant )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Feel the cold of glacial ice!"); break;
							case 1: from.Say("You are nothing but an insect to me!"); break;
							case 2: from.Say("" + m.Name + ", you dare face me!"); break;
							case 3: from.Say("Your frozen corpse will decorate my halls!"); break;
						};
					}
					else if ( from is LavaGiant )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Feel the fires of volcanic might!"); break;
							case 1: from.Say("You are nothing but an insect to me!"); break;
							case 2: from.Say("" + m.Name + ", you dare face me!"); break;
							case 3: from.Say("You will soon be nothing but ashes!"); break;
						};
					}
					else if ( from is DeepSeaGiant
						 || from is SeaGiant )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Feel the strength of the sea!"); break;
							case 1: from.Say("You will soon will rot below the waves!"); break;
							case 2: from.Say("" + m.Name + ", your bones will lie with the crabs!"); break;
							case 3: from.Say("You are no match for the gods of the sea!"); break;
						};
					}
					else if ( from is MountainGiant
						 || from is Giant
						 || from is AbyssGiant
						 || from is JungleGiant
						 || from is SandGiant
						 || from is StoneGiant
						 || from is FireGiant
						 || from is ForestGiant
						 || from is FrostGiant
						 || from is AncientCyclops
						 || from is ShamanicCyclops
						 || from is Cyclops )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("My foot will be the last thing you see!"); break;
							case 1: from.Say("I will crush you into the dirt!"); break;
							case 2: from.Say("" + m.Name + ", you will die!"); break;
							case 3: from.Say("I have defeated foes larger than you!"); break;
						};
					}
					else if ( from is TheAncientTree
						 || from is Ent
						 || from is EvilEnt
						 || from is AncientReaper
						 || from is AncientEnt )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("You...dare...fight...me?!"); break;
							case 1: from.Say("I...will...dispatch...of...you!"); break;
							case 2: from.Say("My...might...outweighs...yours!"); break;
							case 3: from.Say("You...will...die...in...this...fight!"); break;
						};
					}
					else if ( from is SwampThing )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Gri Gril Gestroy Groo!"); break;
							case 1: from.Say("Groo Gran Grever Gregreat Gre!"); break;
							case 2: from.Say("Grour Grones Gril Gray Grin Gry Grwamp!"); break;
							case 3: from.Say("Groo Grar Gro Gratch Gror Gre!"); break;
						};
					}
					else if ( from is Beholder )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("*blinks a red eye*"); break;
							case 1: from.Say("*blinks a blue eye*"); break;
							case 2: from.Say("*blinks a greed eye*"); break;
							case 3: from.Say("*blinks a yellow eye*"); break;
						};
					}
					else if ( from is Dracolich
						 || from is SkeletalDragon )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Your soul will make an excellent meal!"); break;
							case 1: from.Say("" + m.Name + ", do I frighten you?!"); break;
							case 2: from.Say("I have destroyed armies of things like you!"); break;
							case 3: from.Say("You dare invade my lair?!"); break;
						};
					}
					else if ( from is Vampire
						 || from is VampireLord
						 || from is VampirePrince
						 || from is VampireWoods )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("I can smell the blood from your wounds!"); break;
							case 1: from.Say("Look into my eyes..."); break;
							case 2: from.Say("Submit, and I will make it quick!"); break;
							case 3: from.Say("You think I have not faced mortals like you?!"); break;
						};
					}
					else if ( from is Dracula )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("" + m.Name + ", your blood will fill my glass tonight!"); break;
							case 1: from.Say("Look into my eyes, " + m.Name + "..."); break;
							case 2: from.Say("Your blood will decorate these walls!"); break;
							case 3: from.Say("You should be honored to be slain by me!"); break;
						};
					}
					else if ( from is Vordo )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("" + m.Name + ", you will join!"); break;
							case 1: from.Say("Look into my eyes, " + m.Name + "..."); break;
							case 2: from.Say("Your blood will decorate these walls!"); break;
							case 3: from.Say("You should be honored to be slain by me!"); break;
						};
					}
					else if ( from is AncientLich
						 || from is Lich
						 || from is LichKing
						 || from is TitanLich
						 || from is MummyGiant
						 || from is LichLord
						 || from is Nazghoul
						 || from is Surtaz
						 || from is UndeadDruid
						 || from is DemiLich )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("Feel the power of " + from.Name + "!"); break;
							case 1: from.Say("I will have a place for the bones of " + m.Name + "!"); break;
							case 2: from.Say("" + m.Name + ", you are a fool to face me!"); break;
							case 3: from.Say("My magic will decimate you!"); break;
						};
					}
					else if ( from is Executioner )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("" + m.Name + ", you are sentenced to death!"); break;
							case 1: from.Say("Your head will look good on the block!"); break;
							case 2: from.Say("My blade is eager to sever your head!"); break;
							case 3: from.Say("This will be your final fight!"); break;
						};
					}
					else if ( from is BlackKnight )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("" + m.Name + ", do you think you can defeat me?!"); break;
							case 1: from.Say("You will never gain entry to my vault!"); break;
							case 2: from.Say("Many have come here and all have perished!"); break;
							case 3: from.Say("Your treasure will help fill my vault!"); break;
						};
					}
					else if ( from is Archmage )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("" + m.Name + ", you have no hope against my power!"); break;
							case 1: from.Say("You will never leave this place alive!"); break;
							case 2: from.Say("You are no match for my magic!"); break;
							case 3: from.Say("All that have come here have perished!"); break;
						};
					}
					else if ( from is BombWorshipper || from is Psionicist )
					{
						switch ( Utility.Random( 9 ))		   
						{
							case 0: from.Say("I have converted others stonger than you, " + m.Name + "!"); break;
							case 1: from.Say("You will soon be one with the glow!"); break;
							case 2: from.Say("All will know that " + from.Name + " gave " + m.Name + " to the glow!"); break;
							case 3: from.Say("Maybe you should flee before it is too late!"); break;
							case 4: from.Say("Do you think you can beat me?!"); break;
							case 5: from.Say("No one desecrates the temple of the bomb!"); break;
							case 6: from.Say("Your life ends here!"); break;
							case 7: from.Say("Your life ends here, " + m.Name + "!"); break;
							case 8: from.Say("You will kneel before the bomb!"); break;
						};
					}
					else if ( from is Syth )
					{
						switch ( Utility.Random( 9 ))		   
						{
							case 0: from.Say("The Syth will be the last thing you see, " + m.Name + "!"); break;
							case 1: from.Say("You will submit to my dark power!"); break;
							case 2: from.Say("No one will find the bones of " + m.Name + "!"); break;
							case 3: from.Say("You should have fled but it is too late!"); break;
							case 4: from.Say("Do you think you can beat me?!"); break;
							case 5: from.Say("No one has faced a syth and lived!"); break;
							case 6: from.Say("Your life ends here!"); break;
							case 7: from.Say("Your life ends here, " + m.Name + "!"); break;
							case 8: from.Say("You will kneel before the Syth!"); break;
						};
					}
					else if ( from is ElfBerserker
						 || from is ElfRogue
						 || from is ElfMonks
						 || from is ElfMinstrel
						 || from is ElfMage
						 || from is BloodAssassin
						 || from is Berserker
						 || from is Bandit
						 || from is Rogue
						 || from is Monks
						 || from is Minstrel
						 || from is GolemController
						 || from is EvilMageLord
						 || from is EvilMage
						 || from is Brigand
						 || from is OrkMonks
						 || from is OrkMage
						 || from is OrkWarrior
						 || from is OrkRogue )
					{
						switch ( Utility.Random( 9 ))		   
						{
							case 0: from.Say("I have slain others better than you, " + m.Name + "!"); break;
							case 1: from.Say("Your riches will soon be mine!"); break;
							case 2: from.Say("All will know that " + from.Name + " defeated " + m.Name + "!"); break;
							case 3: from.Say("Maybe you should flee before it is too late!"); break;
							case 4: from.Say("Do you think you can best me?!"); break;
							case 5: from.Say("Let this be your final battle!"); break;
							case 6: from.Say("Your life ends here!"); break;
							case 7: from.Say("Your life ends here, " + m.Name + "!"); break;
							case 8: from.Say("All should fear " + from.Name + "!"); break;
						};
					}
					else if ( from is Adventurers || from is Jedi )
					{
						switch ( Utility.Random( 9 ))		   
						{
							case 0: from.Say("I have brought justice to others more vile than you, " + m.Name + "!"); break;
							case 1: from.Say("You will pay for your crimes!"); break;
							case 2: from.Say("All will know that " + from.Name + " brought " + m.Name + " to justice!"); break;
							case 3: from.Say("You should have fled this land long ago!"); break;
							case 4: from.Say("Do you think you can best me?!"); break;
							case 5: from.Say("Let this be your final battle!"); break;
							case 6: from.Say("Your life ends here!"); break;
							case 7: from.Say("Your life ends here, " + m.Name + "!"); break;
							case 8: from.Say("Your evil will be vanquished!"); break;
						};
					}
					else if ( Server.Mobiles.BasePirate.IsSailor( from ) )
					{
						switch ( Utility.Random( 9 ))		   
						{
							case 0: from.Say("" + m.Name + ", you will soon walk the plank!"); break;
							case 1: from.Say("I could beat you if I were three sheets to the wind!"); break;
							case 2: from.Say("I will splice the mainbrace over your corpse!"); break;
							case 3: from.Say("You will soon become shark bait!"); break;
							case 4: from.Say("You scurvy dog, do you think you can best me?!"); break;
							case 5: from.Say("I fought scallywags better than you!"); break;
							case 6: from.Say("No pray, no pay. Your riches will be mine!"); break;
							case 7: from.Say("You landlubber, prepare to die!"); break;
							case 8: from.Say("" + from.Name + ", you will soon feed the fish!"); break;
						};
					}
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void SaySomethingOnDeath( Mobile from, Mobile m )
		{
			if ( from.Name != "a sailor" && from.Name != "a pirate" && from.Name != "a follower" && m != null && from.EmoteHue != 505 && ((BaseCreature)from).ControlSlots != 666 && ((BaseCreature)from).GetMaster() == null && Utility.Random( 5 ) == 1 )
			{
				if ( from.SpeechHue < 1 ){ from.SpeechHue = Utility.RandomTalkHue(); }

				if ( m is BaseCreature )
					m = ((BaseCreature)m).GetMaster();

				if ( m is PlayerMobile )
				{
					if ( from is Exodus )
					{
						from.Say("You have not seen the last of me, " + m.Name + "!");
					}
					else if ( from is FleshGolem || from is AncientFleshGolem )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("No...I am eternal!"); break;
							case 1: from.Say("No...How can this be?!"); break;
							case 2: from.Say("Peace has finally found me..."); break;
							case 3: from.Say("I failed you my master..."); break;
						};
					}
					else if ( from is BloodDemigod )
					{
						from.Say("Some day your blood will fill these halls, " + m.Name + "!");
					}
					else if ( from is Balron 
							|| from is Devil 
							|| from is BlackGateDemon 
							|| from is AbysmalDaemon 
							|| from is Xurtzar 
							|| from is Marilith 
							|| from is Archfiend 
							|| from is Fiend 
							|| from is Daemon 
							|| from is BloodDemon )
					{
						switch ( Utility.Random( 5 ))		   
						{
							case 0: from.Say("No...I will not be vanquished!"); break;
							case 1: from.Say("I will return..."); break;
							case 2: from.Say("I hope the curses of hell fill your soul!"); break;
							case 3: from.Say("Death is only a distraction to me!"); break;
							case 4: from.Say("I will come for you, " + m.Name + "!"); break;
						};
					}
					else if ( from is DeepSeaDevil || from is DemonOfTheSea )
					{
						switch ( Utility.Random( 5 ))		   
						{
							case 0: from.Say("No...I will not be vanquished!"); break;
							case 1: from.Say("I will return..."); break;
							case 2: from.Say("I hope the blood of the sea drowns you!"); break;
							case 3: from.Say("Fool...I will rise again!"); break;
							case 4: from.Say("One day you will be taken by the sea, " + m.Name + "!"); break;
						};
					}
					else if ( from is IceDevil )
					{
						switch ( Utility.Random( 5 ))		   
						{
							case 0: from.Say("No...I will not be vanquished!"); break;
							case 1: from.Say("I will return..."); break;
							case 2: from.Say("The frost of death will find you!"); break;
							case 3: from.Say("Fool...you can never destroy me!"); break;
							case 4: from.Say("My cold heart will come for you, " + m.Name + "!"); break;
						};
					}
					else if ( from is Succubus )
					{
						from.Say("No...!");
					}
					else if ( from is Satan )
					{
						switch ( Utility.Random( 5 ))		   
						{
							case 0: from.Say("No...I cannot return to hell!"); break;
							case 1: from.Say("Hell will not be able to hold me..."); break;
							case 2: from.Say("" + m.Name + ", I will return for you!"); break;
							case 3: from.Say("Fool...I am eternal!"); break;
							case 4: from.Say("I will have your soul one day, " + m.Name + "!"); break;
						};
					}
					else if ( from is VampiricDragon )
					{
						switch ( Utility.Random( 2 ))		   
						{
							case 0: from.Say("No...you cannot do this!"); break;
							case 1: from.Say("Curse you, " + m.Name + "!"); break;
						};
					}
					else if ( from is ShadowWyrm )
					{
						switch ( Utility.Random( 2 ))		   
						{
							case 0: from.Say("No...you cannot have light without dark!"); break;
							case 1: from.Say("Let the shadows take you, " + m.Name + "!"); break;
						};
					}
					else if ( from is AshDragon || from is VolcanicDragon )
					{
						switch ( Utility.Random( 2 ))		   
						{
							case 0: from.Say("No...this cannot be how it ends!"); break;
							case 1: from.Say("Let the mountain fires take you, " + m.Name + "!"); break;
						};
					}
					else if ( from is BottleDragon
						 || from is CaddelliteDragon
						 || from is CrystalDragon
						 || from is DragonKing
						 || from is ElderDragon
						 || from is RadiationDragon
						 || from is VoidDragon
						 || from is PrimevalAbysmalDragon
						 || from is PrimevalAmberDragon
						 || from is PrimevalBlackDragon
						 || from is PrimevalDragon
						 || from is PrimevalFireDragon
						 || from is PrimevalGreenDragon
						 || from is PrimevalNightDragon
						 || from is PrimevalRedDragon
						 || from is PrimevalRoyalDragon
						 || from is PrimevalRunicDragon
						 || from is PrimevalSeaDragon
						 || from is PrimevalSilverDragon
						 || from is PrimevalStygianDragon
						 || from is PrimevalVolcanicDragon
						 || from is AncientWyrm )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("No...this cannot be the end!"); break;
							case 1: from.Say("How...can...this...be..."); break;
							case 2: from.Say("No, " + m.Name + "!"); break;
							case 3: from.Say("What is this madness?!"); break;
						};
					}
					else if ( from is FireGargoyle
						 || from is Gargoyle
						 || from is GargoyleAmethyst
						 || from is GargoyleEmerald
						 || from is GargoyleOnyx
						 || from is GargoyleRuby
						 || from is GargoyleSapphire
						 || from is StoneGargoyle
						 || from is StygianGargoyle
						 || from is StygianGargoyleLord
						 || from is AncientGargoyle
						 || from is GargoyleMarble )
					{
						switch ( Utility.Random( 2 ))		   
						{
							case 0: from.Say("Rae...sael yor yiz xa?"); break;
							case 1: from.Say("Zae zes hima ends sabbia!"); break;
						};
					}
					else if ( from is ZornTheBlacksmith )
					{
						switch ( Utility.Random( 2 ))		   
						{
							case 0: from.Say("No...you will never get the ore!"); break;
							case 1: from.Say("You will never find the caddellite!"); break;
						};
					}
					else if ( from is OrkDemigod )
					{
						switch ( Utility.Random( 3 ))		   
						{
							case 0: from.Say("You cannot defeat the power of gods..."); break;
							case 1: from.Say("" + m.Name + ", you have bested me in battle..."); break;
							case 2: from.Say("No..."); break;
						};
					}
					else if ( from is TrollWitchDoctor
						 || from is Troll
						 || from is SwampTroll
						 || from is SeaTroll
						 || from is FrostTrollShaman
						 || from is FrostTroll )
					{
						switch ( Utility.Random( 2 ))		   
						{
							case 0: from.Say("Me cannot lose!"); break;
							case 1: from.Say("Me curse you..."); break;
						};
					}
					else if ( from is AncientEttin
						 || from is EttinShaman
						 || from is Ettin
						 || from is ArcticEttin )
					{
						switch ( Utility.Random( 2 ))		   
						{
							case 0: from.Say("Arrrggghhh..."); break;
							case 1: from.Say("You...fight...good..."); break;
						};
					}
					else if ( from is Titan
						 || from is ElderTitan
						 || from is StormGiant )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("By what thunder is this?"); break;
							case 1: from.Say("No, it cannot be..."); break;
							case 2: from.Say("You may have won this day, " + m.Name + "...but..."); break;
							case 3: from.Say("Arrgghhh..."); break;
						};
					}
					else if ( from is Dragonogre
						 || from is TundraOgre
						 || from is OgreMagi
						 || from is OgreLord
						 || from is Ogre
						 || from is ArcticOgreLord
						 || from is AbysmalOgre
						 || from is Neanderthal
						 || from is HillGiant
						 || from is HillGiantShaman )
					{
						switch ( Utility.Random( 2 ))		   
						{
							case 0: from.Say("Me no feel good!"); break;
							case 1: from.Say("Arrgghhh..."); break;
						};
					}
					else if ( from is IceGiant )
					{
						switch ( Utility.Random( 3 ))		   
						{
							case 0: from.Say("By iceberg's might, how..."); break;
							case 1: from.Say("Arrgghhh..."); break;
							case 2: from.Say("No..."); break;
						};
					}
					else if ( from is LavaGiant )
					{
						switch ( Utility.Random( 3 ))		   
						{
							case 0: from.Say("By magma's might, how..."); break;
							case 1: from.Say("Arrgghhh..."); break;
							case 2: from.Say("No..."); break;
						};
					}
					else if ( from is DeepSeaGiant
						 || from is SeaGiant )
					{
						switch ( Utility.Random( 4 ))		   
						{
							case 0: from.Say("By Neptunes's might, how..."); break;
							case 1: from.Say("By Poseidon's wrath, how..."); break;
							case 2: from.Say("Arrgghhh..."); break;
							case 3: from.Say("No..."); break;
						};
					}
					else if ( from is MountainGiant
						 || from is Giant
						 || from is AbyssGiant
						 || from is JungleGiant
						 || from is SandGiant
						 || from is StoneGiant
						 || from is FireGiant
						 || from is ForestGiant
						 || from is FrostGiant
						 || from is AncientCyclops
						 || from is ShamanicCyclops
						 || from is Cyclops )
					{
						string called = "fly";
						switch ( Utility.Random( 4 ))		   
						{
							case 0: called = "fly"; break;
							case 1: called = "wretch"; break;
							case 2: called = "toad"; break;
							case 3: called = "thing"; break;
						};
						switch ( Utility.Random( 3 ))		   
						{
							case 0: from.Say("You puny " + called + ", how..."); break;
							case 1: from.Say("Arrgghhh..."); break;
							case 2: from.Say("No..."); break;
						};
					}
					else if ( from is TheAncientTree
						 || from is Ent
						 || from is EvilEnt
						 || from is AncientReaper
						 || from is AncientEnt )
					{
						switch ( Utility.Random( 2 ))		   
						{
							case 0: from.Say("How...did...you..."); break;
							case 1: from.Say("I...am...no...more..."); break;
						};
					}
					else if ( from is SwampThing )
					{
						switch ( Utility.Random( 2 ))		   
						{
							case 0: from.Say("Groo Grite Grood!"); break;
							case 1: from.Say("Grarrgh..."); break;
						};
					}
					else if ( from is Beholder )
					{
						// NOTHING
					}
					else if ( from is Dracolich
						 || from is SkeletalDragon )
					{
						switch ( Utility.Random( 3 ))		   
						{
							case 0: from.Say("My power is eternal!"); break;
							case 1: from.Say("" + m.Name + ", I will have my revenge..."); break;
							case 2: from.Say("No, how can this be?!"); break;
						};
					}
					else if ( from is AncientLich
						 || from is Lich
						 || from is LichKing
						 || from is LichLord
						 || from is Nazghoul
						 || from is TitanLich
						 || from is MummyGiant
						 || from is Surtaz
						 || from is UndeadDruid
						 || from is DemiLich )
					{
						switch ( Utility.Random( 3 ))		   
						{
							case 0: from.Say("My magic is eternal!"); break;
							case 1: from.Say("" + m.Name + ", I will have vengeance..."); break;
							case 2: from.Say("No...how can..."); break;
						};
					}
					else if ( from is Executioner
						 || from is BlackKnight
						 || from is Archmage
						 || from is ElfBerserker
						 || from is ElfRogue
						 || from is ElfMonks
						 || from is ElfMinstrel
						 || from is ElfMage
						 || from is BloodAssassin
						 || from is Berserker
						 || from is Adventurers
						 || from is Bandit
						 || from is Rogue
						 || from is Monks
						 || from is Minstrel
						 || from is GolemController
						 || from is EvilMageLord
						 || from is EvilMage
						 || from is Brigand
						 || from is OrkMonks
						 || from is OrkMage
						 || from is OrkWarrior
						 || from is OrkRogue
						 || Server.Mobiles.BasePirate.IsSailor( from ) )
					{
						switch ( Utility.Random( 5 ))		   
						{
							case 0: from.Say("No!"); break;
							case 1: from.Say("Argh!"); break;
							case 2: from.Say("Ahhh..."); break;
							case 3: from.Say("I...uh...uhhhhh..."); break;
							case 4: from.Say("Nooo..."); break;
						};
					}
				}
			}
		}
	}
}

namespace Server
{
	public class OppositionGroup
	{
		private Type[][] m_Types;

		public OppositionGroup( Type[][] types )
		{
			m_Types = types;
		}

		public bool IsEnemy( object from, object target )
		{
			int fromGroup = IndexOf( from );
			int targGroup = IndexOf( target );

			return ( fromGroup != -1 && targGroup != -1 && fromGroup != targGroup );
		}

		public int IndexOf( object obj )
		{
			if ( obj == null )
				return -1;

			Type type = obj.GetType();

			for ( int i = 0; i < m_Types.Length; ++i )
			{
				Type[] group = m_Types[i];

				bool contains = false;

				for ( int j = 0; !contains && j < group.Length; ++j )
					contains = ( type == group[j] );

				if ( contains )
					return i;
			}

			return -1;
		}

		private static OppositionGroup m_TerathansAndOphidians = new OppositionGroup( new Type[][]
			{
				new Type[]
				{
					typeof( WaterSpawn )
				},
				new Type[]
				{
					typeof( WaterSpawn )
				}
			} );

		public static OppositionGroup TerathansAndOphidians
		{
			get{ return m_TerathansAndOphidians; }
		}

		private static OppositionGroup m_SavagesAndOrcs = new OppositionGroup( new Type[][]
			{
				new Type[]
				{
					typeof( WaterSpawn )
				},
				new Type[]
				{
					typeof( WaterSpawn )
				}
			} );

		public static OppositionGroup SavagesAndOrcs
		{
			get{ return m_SavagesAndOrcs; }
		}
		
		private static OppositionGroup m_FeyAndUndead = new OppositionGroup( new Type[][]
			{
				new Type[]
				{
					typeof( WaterSpawn )
				},
				new Type[]
				{
					typeof( WaterSpawn )
				}
			} );

		public static OppositionGroup FeyAndUndead
		{
			get{ return m_FeyAndUndead; }
		}
	}
}

namespace Server.Mobiles
{
	public class AnimalAI : BaseAI
	{
		public AnimalAI(BaseCreature m) : base (m)
		{
		}

		public override bool DoActionWander()
		{
			// Old:
#if false
			if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, true, false, true))
			{
				m_Mobile.DebugSay( "There is something near, I go away" );
				Action = ActionType.Backoff;
			}
			else if ( m_Mobile.IsHurt() || m_Mobile.Combatant != null )
			{
				m_Mobile.DebugSay( "I am hurt or being attacked, I flee" );						
				Action = ActionType.Flee;
			}
			else
			{
				base.DoActionWander();
			}

			return true;
#endif

			// New, only flee @ 10%

			double hitPercent = (double)m_Mobile.Hits / m_Mobile.HitsMax;

			if ( !m_Mobile.Summoned && !m_Mobile.Controlled && hitPercent < 0.1 ) // Less than 10% health
			{
				m_Mobile.DebugSay( "I am low on health!" );
				Action = ActionType.Flee;
			}
			else if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionWander();
			}

			return true;
		}

		public override bool DoActionCombat()
		{
			Mobile combatant = m_Mobile.Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map )
			{
				m_Mobile.DebugSay( "My combatant is gone.." );

				Action = ActionType.Wander;

				return true;
			}

			if ( WalkMobileRange( combatant, 1, true, m_Mobile.RangeFight, m_Mobile.RangeFight ) )
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo( combatant );
			}
			else
			{
				if ( m_Mobile.GetDistanceToSqrt( combatant ) > m_Mobile.RangePerception + 1 )
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "I cannot find {0}", combatant.Name );

					Action = ActionType.Wander;

					return true;
				}
				else
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "I should be closer to {0}", combatant.Name );
				}
			}

			if ( !m_Mobile.Controlled && !m_Mobile.Summoned )
			{
				double hitPercent = (double)m_Mobile.Hits / m_Mobile.HitsMax;

				if ( hitPercent < 0.1 )
				{
					m_Mobile.DebugSay( "I am low on health!" );
					Action = ActionType.Flee;
				}
			}

			return true;
		}

		public override bool DoActionBackoff()
		{
			double hitPercent = (double)m_Mobile.Hits / m_Mobile.HitsMax;

			if ( !m_Mobile.Summoned && !m_Mobile.Controlled && hitPercent < 0.1 ) // Less than 10% health
			{
				Action = ActionType.Flee;
			}
			else
			{
				if (AcquireFocusMob(m_Mobile.RangePerception * 2, FightMode.Closest, true, false , true))
				{
					if ( WalkMobileRange(m_Mobile.FocusMob, 1, false, m_Mobile.RangePerception, m_Mobile.RangePerception * 2) )
					{
						m_Mobile.DebugSay( "Well, here I am safe" );
						Action = ActionType.Wander;
					}					
				}
				else
				{
					m_Mobile.DebugSay( "I have lost my focus, lets relax" );
					Action = ActionType.Wander;
				}
			}

			return true;
		}

		public override bool DoActionFlee()
		{
			AcquireFocusMob(m_Mobile.RangePerception * 2, m_Mobile.FightMode, true, false, true);

			if ( m_Mobile.FocusMob == null )
				m_Mobile.FocusMob = m_Mobile.Combatant;

			return base.DoActionFlee();
		}
	}
}

namespace Server.Mobiles
{
	public class ArcherAI : BaseAI
	{
		public ArcherAI(BaseCreature m) : base (m)
		{
		}

		public override bool DoActionWander()
		{
			m_Mobile.DebugSay( "I have no combatant" );

			if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0} and I will attack", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				return base.DoActionWander();
			}

			return true;
		}

		public override bool DoActionCombat()
		{
			if ( m_Mobile.Combatant == null || m_Mobile.Combatant.Deleted || !m_Mobile.Combatant.Alive || m_Mobile.Combatant.IsDeadBondedPet )
			{
				m_Mobile.DebugSay("My combatant is deleted");
				Action = ActionType.Guard;
				return true;
			}

			if ( (m_Mobile.LastMoveTime + TimeSpan.FromSeconds( 1.0 )) < DateTime.Now )
			{
				if (WalkMobileRange(m_Mobile.Combatant, 1, true, m_Mobile.RangeFight, m_Mobile.Weapon.MaxRange))
				{
					// Be sure to face the combatant
					m_Mobile.Direction = m_Mobile.GetDirectionTo(m_Mobile.Combatant.Location);
				}
				else
				{
					if ( m_Mobile.Combatant != null )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "I am still not in range of {0}", m_Mobile.Combatant.Name);

						if ( (int) m_Mobile.GetDistanceToSqrt( m_Mobile.Combatant ) > m_Mobile.RangePerception + 1 )
						{
							if ( m_Mobile.Debug )
								m_Mobile.DebugSay( "I have lost {0}", m_Mobile.Combatant.Name);

							m_Mobile.Combatant = null;
							Action = ActionType.Guard;
							return true;
						}
					}
				}
			}

			// When we have no ammo, we flee
			Container pack = m_Mobile.Backpack;

			if ( pack == null || pack.FindItemByType( typeof( Arrow ) ) == null )
			{
				Action = ActionType.Flee;
				return true;
			}

			
			// At 20% we should check if we must leave
			if ( m_Mobile.Hits < m_Mobile.HitsMax*20/100 )
			{
				bool bFlee = false;
				// if my current hits are more than my opponent, i don't care
				if ( m_Mobile.Combatant != null && m_Mobile.Hits < m_Mobile.Combatant.Hits)
				{
					int iDiff = m_Mobile.Combatant.Hits - m_Mobile.Hits;

					if ( Utility.Random(0, 100) > 10 + iDiff) // 10% to flee + the diff of hits
					{
						bFlee = true;
					}
				}
				else if ( m_Mobile.Combatant != null && m_Mobile.Hits >= m_Mobile.Combatant.Hits)
				{
					if ( Utility.Random(0, 100) > 10 ) // 10% to flee
					{
						bFlee = true;
					}
				}
						
				if (bFlee)
				{
					Action = ActionType.Flee; 
				}
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if ( AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionGuard();
			}

			return true;
		}
	}
}

namespace Server.Mobiles
{
	public enum AIType
	{
		AI_Use_Default,
		AI_Melee,
		AI_Animal,
		AI_Archer,
		AI_Healer,
		AI_Vendor,
		AI_Mage,
		AI_Berserk,
		AI_Predator,
		AI_Thief,
		AI_Citizen
	}

	public enum ActionType
	{
		Wander,
		Combat,
		Guard,
		Flee,
		Backoff,
		Interact
	}

	public abstract class BaseAI
	{
		public Timer m_Timer;
		protected ActionType m_Action;
		private DateTime m_NextStopGuard;

		public BaseCreature m_Mobile;

		public BaseAI( BaseCreature m )
		{
			m_Mobile = m;

			m_Timer = new AITimer( this );

			bool activate;

			if( !m.PlayerRangeSensitive )
				activate = true;
			else if( World.Loading )
				activate = false;
			else if( m.Map == null || m.Map == Map.Internal || !m.Map.GetSector( m ).Active )
				activate = false;
			else
				activate = true;

			if( activate )
				m_Timer.Start();

			Action = ActionType.Wander;
		}

		public ActionType Action
		{
			get
			{
				return m_Action;
			}
			set
			{
				m_Action = value;
				OnActionChanged();
			}
		}

		public virtual bool WasNamed( string speech )
		{
			string name = m_Mobile.Name;

			return (name != null && Insensitive.StartsWith( speech, name ));
		}

		private class InternalEntry : ContextMenuEntry
		{
			private Mobile m_From;
			private BaseCreature m_Mobile;
			private BaseAI m_AI;
			private OrderType m_Order;

			public InternalEntry( Mobile from, int number, int range, BaseCreature mobile, BaseAI ai, OrderType order )
				: base( number, range )
			{
				m_From = from;
				m_Mobile = mobile;
				m_AI = ai;
				m_Order = order;

				if( mobile.IsDeadPet && (order == OrderType.Guard || order == OrderType.Attack || order == OrderType.Transfer || order == OrderType.Drop) )
					Enabled = false;
			}

			public override void OnClick()
			{
				if( !m_Mobile.Deleted && m_Mobile.Controlled && m_From.CheckAlive() )
				{
					if( m_Mobile.IsDeadPet && (m_Order == OrderType.Guard || m_Order == OrderType.Attack || m_Order == OrderType.Transfer || m_Order == OrderType.Drop) )
						return;

					bool isOwner = (m_From == m_Mobile.ControlMaster);
					bool isFriend = (!isOwner && m_Mobile.IsPetFriend( m_From ));

					if( !isOwner && !isFriend )
						return;
					else if( isFriend && m_Order != OrderType.Follow && m_Order != OrderType.Stay && m_Order != OrderType.Stop )
						return;

					switch( m_Order )
					{
						case OrderType.Follow:
						case OrderType.Attack:
						case OrderType.Transfer:
						case OrderType.Friend:
						case OrderType.Unfriend:
						{
							if( m_Order == OrderType.Transfer && m_From.HasTrade )
								m_From.SendLocalizedMessage( 1010507 ); // You cannot transfer a pet with a trade pending
							else if( m_Order == OrderType.Friend && m_From.HasTrade )
								m_From.SendLocalizedMessage( 1070947 ); // You cannot friend a pet with a trade pending
							else
								m_AI.BeginPickTarget( m_From, m_Order );

							break;
						}
						case OrderType.Release:
						{
							if( m_Mobile.Summoned )
								goto default;
							else
								m_From.SendGump( new Gumps.ConfirmReleaseGump( m_From, m_Mobile ) );

							break;
						}
						default:
						{
							if( m_Mobile.CheckControlChance( m_From ) )
								m_Mobile.ControlOrder = m_Order;

							break;
						}
					}
				}
			}
		}

		public virtual void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if( from.Alive && m_Mobile.Controlled && from.InRange( m_Mobile, 14 ) )
			{
				if ( ( m_Mobile.Blessed || m_Mobile is AerialServant || m_Mobile is HenchmanFamiliar || m_Mobile is FrankenPorter || m_Mobile is PackBeast || m_Mobile is GolemPorter ) && ( from == m_Mobile.ControlMaster ) )
				{
					list.Add( new InternalEntry( from, 6108, 14, m_Mobile, this, OrderType.Follow ) ); // Command: Follow
					list.Add( new InternalEntry( from, 6112, 14, m_Mobile, this, OrderType.Stop ) );   // Command: Stop
					list.Add( new InternalEntry( from, 6114, 14, m_Mobile, this, OrderType.Stay ) );   // Command: Stay
					list.Add( new InternalEntry( from, 6118, 14, m_Mobile, this, OrderType.Release ) ); // Release
				}
				else if( from == m_Mobile.ControlMaster )
				{
					list.Add( new InternalEntry( from, 6107, 14, m_Mobile, this, OrderType.Guard ) );  // Command: Guard
					list.Add( new InternalEntry( from, 6108, 14, m_Mobile, this, OrderType.Follow ) ); // Command: Follow

					if( m_Mobile.CanDrop )
						list.Add( new InternalEntry( from, 6109, 14, m_Mobile, this, OrderType.Drop ) );   // Command: Drop

					list.Add( new InternalEntry( from, 6111, 14, m_Mobile, this, OrderType.Attack ) ); // Command: Kill

					list.Add( new InternalEntry( from, 6112, 14, m_Mobile, this, OrderType.Stop ) );   // Command: Stop
					list.Add( new InternalEntry( from, 6114, 14, m_Mobile, this, OrderType.Stay ) );   // Command: Stay

					if( !m_Mobile.Summoned && !( m_Mobile is GrizzledMare ) )
					{
						list.Add( new InternalEntry( from, 6110, 14, m_Mobile, this, OrderType.Friend ) ); // Add Friend
						list.Add( new InternalEntry( from, 6099, 14, m_Mobile, this, OrderType.Unfriend ) ); // Remove Friend
						list.Add( new InternalEntry( from, 6113, 14, m_Mobile, this, OrderType.Transfer ) ); // Transfer
					}

					list.Add( new InternalEntry( from, 6118, 14, m_Mobile, this, OrderType.Release ) ); // Release
				}
				else if( m_Mobile.IsPetFriend( from ) )
				{
					list.Add( new InternalEntry( from, 6108, 14, m_Mobile, this, OrderType.Follow ) ); // Command: Follow
					list.Add( new InternalEntry( from, 6112, 14, m_Mobile, this, OrderType.Stop ) );   // Command: Stop
					list.Add( new InternalEntry( from, 6114, 14, m_Mobile, this, OrderType.Stay ) );   // Command: Stay
				}
			}
		}

		public virtual void BeginPickTarget( Mobile from, OrderType order )
		{
			if( m_Mobile.Deleted || !m_Mobile.Controlled || !from.InRange( m_Mobile, 14 ) || from.Map != m_Mobile.Map )
				return;

			bool isOwner = (from == m_Mobile.ControlMaster);
			bool isFriend = (!isOwner && m_Mobile.IsPetFriend( from ));

			if( !isOwner && !isFriend )
				return;
			else if( isFriend && order != OrderType.Follow && order != OrderType.Stay && order != OrderType.Stop )
				return;

			if( from.Target == null )
			{
				if( order == OrderType.Transfer )
					from.SendLocalizedMessage( 502038 ); // Click on the person to transfer ownership to.
				else if( order == OrderType.Friend )
					from.SendLocalizedMessage( 502020 ); // Click on the player whom you wish to make a co-owner.
				else if( order == OrderType.Unfriend )
					from.SendLocalizedMessage( 1070948 ); // Click on the player whom you wish to remove as a co-owner.

				from.Target = new AIControlMobileTarget( this, order );
			}
			else if( from.Target is AIControlMobileTarget )
			{
				AIControlMobileTarget t = (AIControlMobileTarget)from.Target;

				if( t.Order == order )
					t.AddAI( this );
			}
		}

		public virtual void OnAggressiveAction( Mobile aggressor )
		{
			Mobile currentCombat = m_Mobile.Combatant;

			if( currentCombat != null && !aggressor.Hidden && currentCombat != aggressor && m_Mobile.GetDistanceToSqrt( currentCombat ) > m_Mobile.GetDistanceToSqrt( aggressor ) )
				m_Mobile.Combatant = aggressor;
		}

		public virtual void EndPickTarget( Mobile from, Mobile target, OrderType order )
		{
			if( m_Mobile.Deleted || !m_Mobile.Controlled || !from.InRange( m_Mobile, 14 ) || from.Map != m_Mobile.Map || !from.CheckAlive() )
				return;

			bool isOwner = (from == m_Mobile.ControlMaster);
			bool isFriend = (!isOwner && m_Mobile.IsPetFriend( from ));

			if( !isOwner && !isFriend )
				return;
			else if( isFriend && order != OrderType.Follow && order != OrderType.Stay && order != OrderType.Stop )
				return;

			if( order == OrderType.Attack )
			{
				if( from.Blessed || ( target is BaseCreature && ((BaseCreature)target).IsScaryToPets && m_Mobile.IsScaredOfScaryThings ) )
				{
					m_Mobile.SayTo( from, "Your pet refuses to attack this creature!" );
					return;
				}
			}

			if( m_Mobile.CheckControlChance( from ) )
			{
				m_Mobile.ControlTarget = target;
				m_Mobile.ControlOrder = order;
			}
		}

		public virtual bool HandlesOnSpeech( Mobile from )
		{
			if( from.AccessLevel >= AccessLevel.GameMaster )
				return true;

			if( from.Alive && m_Mobile.Controlled && m_Mobile.Commandable && (from == m_Mobile.ControlMaster || m_Mobile.IsPetFriend( from )) )
				return true;

			return (from.Alive && from.InRange( m_Mobile.Location, 3 ) && m_Mobile.IsHumanInTown());
		}

		private static SkillName[] m_KeywordTable = new SkillName[]
			{
				SkillName.Parry,
				SkillName.Healing,
				SkillName.Hiding,
				SkillName.Stealing,
				SkillName.Alchemy,
				SkillName.Druidism,
				SkillName.Mercantile,
				SkillName.ArmsLore,
				SkillName.Begging,
				SkillName.Blacksmith,
				SkillName.Bowcraft,
				SkillName.Peacemaking,
				SkillName.Camping,
				SkillName.Carpentry,
				SkillName.Cartography,
				SkillName.Cooking,
				SkillName.Searching,
				SkillName.Discordance,//??
				SkillName.Psychology,
				SkillName.Seafaring,
				SkillName.Provocation,
				SkillName.Lockpicking,
				SkillName.Magery,
				SkillName.MagicResist,
				SkillName.Tactics,
				SkillName.Snooping,
				SkillName.RemoveTrap,
				SkillName.Musicianship,
				SkillName.Poisoning,
				SkillName.Marksmanship,
				SkillName.Spiritualism,
				SkillName.Tailoring,
				SkillName.Taming,
				SkillName.Tasting,
				SkillName.Tinkering,
				SkillName.Veterinary,
				SkillName.Forensics,
				SkillName.Herding,
				SkillName.Tracking,
				SkillName.Stealth,
				SkillName.Inscribe,
				SkillName.Swords,
				SkillName.Bludgeoning,
				SkillName.Fencing,
				SkillName.FistFighting,
				SkillName.Lumberjacking,
				SkillName.Mining,
				SkillName.Meditation
			};

		public virtual void OnSpeech( SpeechEventArgs e )
		{
			if( e.Mobile.Alive && e.Mobile.InRange( m_Mobile.Location, 3 ) && m_Mobile.IsHumanInTown() )
			{
				if( e.HasKeyword( 0x9D ) && WasNamed( e.Speech ) ) // *move*
				{
					if( m_Mobile.Combatant != null )
					{
						// I am too busy fighting to deal with thee!
						m_Mobile.PublicOverheadMessage( MessageType.Regular, 0x3B2, 501482 );
					}
					else
					{
						// Excuse me?
						m_Mobile.PublicOverheadMessage( MessageType.Regular, 0x3B2, 501516 );
						WalkRandomInHome( 2, 2, 1 );
					}
				}
				else if( e.HasKeyword( 0x9E ) && WasNamed( e.Speech ) ) // *time*
				{
					if( m_Mobile.Combatant != null )
					{
						// I am too busy fighting to deal with thee!
						m_Mobile.PublicOverheadMessage( MessageType.Regular, 0x3B2, 501482 );
					}
					else
					{
						int generalNumber;
						string exactTime;

						Clock.GetTime( m_Mobile, out generalNumber, out exactTime );

						m_Mobile.PublicOverheadMessage( MessageType.Regular, 0x3B2, generalNumber );
					}
				}
				else if( e.HasKeyword( 0x6C ) && WasNamed( e.Speech ) ) // *train
				{
					if( m_Mobile.Combatant != null )
					{
						// I am too busy fighting to deal with thee!
						m_Mobile.PublicOverheadMessage( MessageType.Regular, 0x3B2, 501482 );
					}
					else
					{
						bool foundSomething = false;

						Skills ourSkills = m_Mobile.Skills;
						Skills theirSkills = e.Mobile.Skills;

						for( int i = 0; i < ourSkills.Length && i < theirSkills.Length; ++i )
						{
							Skill skill = ourSkills[i];
							Skill theirSkill = theirSkills[i];

							if( skill != null && theirSkill != null && skill.Base >= 60.0 && m_Mobile.CheckTeach( skill.SkillName, e.Mobile ) )
							{
								double toTeach = skill.Base / 3.0;

								if( toTeach > 42.0 )
									toTeach = 42.0;

								if( toTeach > theirSkill.Base )
								{
									int number = 1043059 + i;

									if( number > 1043107 )
										continue;

									if( !foundSomething )
										m_Mobile.Say( 1043058 ); // I can train the following:

									m_Mobile.Say( number );

									foundSomething = true;
								}
							}
						}

						if( !foundSomething )
							m_Mobile.Say( 501505 ); // Alas, I cannot teach thee anything.
					}
				}
				else
				{
					SkillName toTrain = (SkillName)(-1);

					for( int i = 0; toTrain == (SkillName)(-1) && i < e.Keywords.Length; ++i )
					{
						int keyword = e.Keywords[i];

						if( keyword == 0x154 )
						{
							toTrain = SkillName.Anatomy;
						}
						else if( keyword >= 0x6D && keyword <= 0x9C )
						{
							int index = keyword - 0x6D;

							if( index >= 0 && index < m_KeywordTable.Length )
								toTrain = m_KeywordTable[index];
						}
					}

					if( toTrain != (SkillName)(-1) && WasNamed( e.Speech ) )
					{
						if( m_Mobile.Combatant != null )
						{
							// I am too busy fighting to deal with thee!
							m_Mobile.PublicOverheadMessage( MessageType.Regular, 0x3B2, 501482 );
						}
						else
						{
							Skills skills = m_Mobile.Skills;
							Skill skill = skills[toTrain];

							if( skill == null || skill.Base < 60.0 || !m_Mobile.CheckTeach( toTrain, e.Mobile ) )
							{
								m_Mobile.Say( 501507 ); // 'Tis not something I can teach thee of.
							}
							else
							{
								m_Mobile.Teach( toTrain, e.Mobile, 0, false );
							}
						}
					}
				}
			}

			if( m_Mobile.Controlled && m_Mobile.Commandable )
			{
				m_Mobile.DebugSay( "Listening..." );

				bool isOwner = (e.Mobile == m_Mobile.ControlMaster);
				bool isFriend = (!isOwner && m_Mobile.IsPetFriend( e.Mobile ));

				if( e.Mobile.Alive && (isOwner || isFriend) )
				{
					m_Mobile.DebugSay( "It's from my master" );

					int[] keywords = e.Keywords;
					string speech = e.Speech;

					// First, check the all*
					for( int i = 0; i < keywords.Length; ++i )
					{
						int keyword = keywords[i];

						switch( keyword )
						{
							case 0x164: // all come
							{
								if( !isOwner )
									break;

								if( m_Mobile.CheckControlChance( e.Mobile ) )
								{
									m_Mobile.ControlTarget = null;
									m_Mobile.ControlOrder = OrderType.Come;
								}

								return;
							}
							case 0x165: // all follow
							{
								BeginPickTarget( e.Mobile, OrderType.Follow );
								return;
							}
							case 0x166: // all guard
							case 0x16B: // all guard me
							{
								if( !isOwner )
									break;

								if( m_Mobile.CheckControlChance( e.Mobile ) )
								{
									m_Mobile.ControlTarget = null;
									m_Mobile.ControlOrder = OrderType.Guard;
								}
								return;
							}
							case 0x167: // all stop
							{
								if( m_Mobile.CheckControlChance( e.Mobile ) )
								{
									m_Mobile.ControlTarget = null;
									m_Mobile.ControlOrder = OrderType.Stop;
								}
								return;
							}
							case 0x168: // all kill
							case 0x169: // all attack
							{
								if( !isOwner )
									break;

								BeginPickTarget( e.Mobile, OrderType.Attack );
								return;
							}
							case 0x16C: // all follow me
							{
								if( m_Mobile.CheckControlChance( e.Mobile ) )
								{
									m_Mobile.ControlTarget = e.Mobile;
									m_Mobile.ControlOrder = OrderType.Follow;
								}
								return;
							}
							case 0x170: // all stay
							{
								if( m_Mobile.CheckControlChance( e.Mobile ) )
								{
									m_Mobile.ControlTarget = null;
									m_Mobile.ControlOrder = OrderType.Stay;
								}
								return;
							}
						}
					}

					// No all*, so check *command
					for( int i = 0; i < keywords.Length; ++i )
					{
						int keyword = keywords[i];

						switch( keyword )
						{
							case 0x155: // *come
							{
								if( !isOwner )
									break;

								if( WasNamed( speech ) && m_Mobile.CheckControlChance( e.Mobile ) )
								{
									m_Mobile.ControlTarget = null;
									m_Mobile.ControlOrder = OrderType.Come;
								}

								return;
							}
							case 0x156: // *drop
							{
								if( !isOwner )
									break;

								if( !m_Mobile.IsDeadPet && !m_Mobile.Summoned && WasNamed( speech ) && m_Mobile.CheckControlChance( e.Mobile ) )
								{
									m_Mobile.ControlTarget = null;
									m_Mobile.ControlOrder = OrderType.Drop;
								}

								return;
							}
							case 0x15A: // *follow
							{
								if( WasNamed( speech ) && m_Mobile.CheckControlChance( e.Mobile ) )
									BeginPickTarget( e.Mobile, OrderType.Follow );

								return;
							}
							case 0x15B: // *friend
							{
								if( !isOwner )
									break;

								if( WasNamed( speech ) && m_Mobile.CheckControlChance( e.Mobile ) )
								{
									if( m_Mobile.Summoned  || ( m_Mobile is GrizzledMare ) )
										e.Mobile.SendLocalizedMessage( 1005481 ); // Summoned creatures are loyal only to their summoners.
									else if( e.Mobile.HasTrade )
										e.Mobile.SendLocalizedMessage( 1070947 ); // You cannot friend a pet with a trade pending
									else
										BeginPickTarget( e.Mobile, OrderType.Friend );
								}

								return;
							}
							case 0x15C: // *guard
							{
								if( !isOwner )
									break;

								if( !m_Mobile.IsDeadPet && WasNamed( speech ) && m_Mobile.CheckControlChance( e.Mobile ) )
								{
									m_Mobile.ControlTarget = null;
									m_Mobile.ControlOrder = OrderType.Guard;
								}

								return;
							}
							case 0x15D: // *kill
							case 0x15E: // *attack
							{
								if( !isOwner )
									break;

								if( !m_Mobile.IsDeadPet && WasNamed( speech ) && m_Mobile.CheckControlChance( e.Mobile ) )
									BeginPickTarget( e.Mobile, OrderType.Attack );

								return;
							}
							case 0x15F: // *patrol
							{
								if( !isOwner )
									break;

								if( WasNamed( speech ) && m_Mobile.CheckControlChance( e.Mobile ) )
								{
									m_Mobile.ControlTarget = null;
									m_Mobile.ControlOrder = OrderType.Patrol;
								}

								return;
							}
							case 0x161: // *stop
							{
								if( WasNamed( speech ) && m_Mobile.CheckControlChance( e.Mobile ) )
								{
									m_Mobile.ControlTarget = null;
									m_Mobile.ControlOrder = OrderType.Stop;
								}

								return;
							}
							case 0x163: // *follow me
							{
								if( WasNamed( speech ) && m_Mobile.CheckControlChance( e.Mobile ) )
								{
									m_Mobile.ControlTarget = e.Mobile;
									m_Mobile.ControlOrder = OrderType.Follow;
								}

								return;
							}
							case 0x16D: // *release
							{
								if( !isOwner )
									break;

								if( WasNamed( speech ) && m_Mobile.CheckControlChance( e.Mobile ) )
								{
									if( !m_Mobile.Summoned )
									{
										e.Mobile.SendGump( new Gumps.ConfirmReleaseGump( e.Mobile, m_Mobile ) );
									}
									else
									{
										m_Mobile.ControlTarget = null;
										m_Mobile.ControlOrder = OrderType.Release;
									}
								}

								return;
							}
							case 0x16E: // *transfer
							{
								if( !isOwner )
									break;

								if( !m_Mobile.IsDeadPet && WasNamed( speech ) && m_Mobile.CheckControlChance( e.Mobile ) )
								{
									if( m_Mobile.Summoned || ( m_Mobile is GrizzledMare ) )
										e.Mobile.SendLocalizedMessage( 1005487 ); // You cannot transfer ownership of a summoned creature.
									else if( e.Mobile.HasTrade )
										e.Mobile.SendLocalizedMessage( 1010507 ); // You cannot transfer a pet with a trade pending
									else
										BeginPickTarget( e.Mobile, OrderType.Transfer );
								}

								return;
							}
							case 0x16F: // *stay
							{
								if( WasNamed( speech ) && m_Mobile.CheckControlChance( e.Mobile ) )
								{
									m_Mobile.ControlTarget = null;
									m_Mobile.ControlOrder = OrderType.Stay;
								}

								return;
							}
						}
					}
				}
			}
			else
			{
				if( e.Mobile.AccessLevel >= AccessLevel.GameMaster )
				{
					m_Mobile.DebugSay( "It's from a GM" );

					if( m_Mobile.FindMyName( e.Speech, true ) )
					{
						string[] str = e.Speech.Split( ' ' );
						int i;

						for( i=0; i < str.Length; i++ )
						{
							string word = str[i];

							if( Insensitive.Equals( word, "obey" ) )
							{
								m_Mobile.SetControlMaster( e.Mobile );

								if( m_Mobile.Summoned )
									m_Mobile.SummonMaster = e.Mobile;

								return;
							}
						}
					}
				}
			}
		}

		public virtual bool Think()
		{
			if( m_Mobile.Deleted )
				return false;

			if( CheckFlee() )
				return true;

			switch( Action )
			{
				case ActionType.Wander:
				m_Mobile.OnActionWander();
				return DoActionWander();

				case ActionType.Combat:
				m_Mobile.OnActionCombat();
				return DoActionCombat();

				case ActionType.Guard:
				m_Mobile.OnActionGuard();
				return DoActionGuard();

				case ActionType.Flee:
				m_Mobile.OnActionFlee();
				return DoActionFlee();

				case ActionType.Interact:
				m_Mobile.OnActionInteract();
				return DoActionInteract();

				case ActionType.Backoff:
				m_Mobile.OnActionBackoff();
				return DoActionBackoff();

				default:
				return false;
			}
		}

		public virtual void OnActionChanged()
		{
			switch( Action )
			{
				case ActionType.Wander:
				m_Mobile.Warmode = false;
				m_Mobile.Combatant = null;
				m_Mobile.FocusMob = null;
				m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
				break;

				case ActionType.Combat:
				m_Mobile.Warmode = true;
				m_Mobile.FocusMob = null;
				m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
				break;

				case ActionType.Guard:
				m_Mobile.Warmode = true;
				m_Mobile.FocusMob = null;
				m_Mobile.Combatant = null;
				m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
				m_NextStopGuard = DateTime.Now + TimeSpan.FromSeconds( 10 );
				m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
				break;

				case ActionType.Flee:
				m_Mobile.Warmode = true;
				m_Mobile.FocusMob = null;
				m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
				break;

				case ActionType.Interact:
				m_Mobile.Warmode = false;
				m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
				break;

				case ActionType.Backoff:
				m_Mobile.Warmode = false;
				m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
				break;
			}
		}

		public virtual bool OnAtWayPoint()
		{
			return true;
		}

		public virtual bool DoActionWander()
		{
			if( m_Mobile is HouseVisitor )
			{
				m_Mobile.DebugSay( "I am resting here!" );
			}
			else if( CheckHerding() )
			{
				m_Mobile.DebugSay( "Praise the shepherd!" );
			}
			else if( m_Mobile.CurrentWayPoint != null )
			{
				WayPoint point = m_Mobile.CurrentWayPoint;
				if( (point.X != m_Mobile.Location.X || point.Y != m_Mobile.Location.Y) && point.Map == m_Mobile.Map && point.Parent == null && !point.Deleted )
				{
					m_Mobile.DebugSay( "I will move towards my waypoint." );
					DoMove( m_Mobile.GetDirectionTo( m_Mobile.CurrentWayPoint ) );
				}
				else if( OnAtWayPoint() )
				{
					m_Mobile.DebugSay( "I will go to the next waypoint" );
					m_Mobile.CurrentWayPoint = point.NextPoint;
					if( point.NextPoint != null && point.NextPoint.Deleted )
						m_Mobile.CurrentWayPoint = point.NextPoint = point.NextPoint.NextPoint;
				}
			}
			else if( m_Mobile.IsAnimatedDead )
			{
				// animated dead follow their master
				Mobile master = m_Mobile.SummonMaster;

				if( master != null && master.Map == m_Mobile.Map && master.InRange( m_Mobile, m_Mobile.RangePerception ) )
					MoveTo( master, false, 1 );
				else
					WalkRandomInHome( 2, 2, 1 );
			}
			else if( CheckMove() )
			{
				if( !m_Mobile.CheckIdle() )
					WalkRandomInHome( 2, 2, 1 );
			}

			if( m_Mobile.Combatant != null && !m_Mobile.Combatant.Deleted && m_Mobile.Combatant.Alive && !m_Mobile.Combatant.IsDeadBondedPet )
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo( m_Mobile.Combatant );
			}

			return true;
		}

		public virtual bool DoActionCombat()
		{
			if ( CheckHerding() )
			{
				m_Mobile.DebugSay( "Praise the shepherd!" );
			}
			else
			{
				Mobile c = m_Mobile.Combatant;

				if ( c == null || c.Deleted || c.Map != m_Mobile.Map || !c.Alive || c.IsDeadBondedPet )
					Action = ActionType.Wander;
				else
					m_Mobile.Direction = m_Mobile.GetDirectionTo( c );
			}

			return true;
		}

		public virtual bool DoActionGuard()
		{
			if ( CheckHerding() )
			{
				m_Mobile.DebugSay( "Praise the shepherd!" );
			}
			else if( DateTime.Now < m_NextStopGuard )
			{
				m_Mobile.DebugSay( "I am on guard" );
				//m_Mobile.Turn( Utility.Random(0, 2) - 1 );
			}
			else
			{
				m_Mobile.DebugSay( "I stopped being on guard" );
				Action = ActionType.Wander;
			}

			return true;
		}

		public virtual bool DoActionFlee()
		{
			Mobile from = m_Mobile.FocusMob;

			if( from == null || from.Deleted || from.Map != m_Mobile.Map )
			{
				m_Mobile.DebugSay( "I have lost him" );
				Action = ActionType.Guard;
				return true;
			}

			if( WalkMobileRange( from, 1, true, m_Mobile.RangePerception*2, m_Mobile.RangePerception*3 ) )
			{
				m_Mobile.DebugSay( "I have fled" );
				Action = ActionType.Guard;
				return true;
			}
			else
			{
				m_Mobile.DebugSay( "I am fleeing!" );
			}

			return true;
		}

		public virtual bool DoActionInteract()
		{
			return true;
		}

		public virtual bool DoActionBackoff()
		{
			return true;
		}

		public virtual bool Obey()
		{
			if( m_Mobile.Deleted )
				return false;

			switch( m_Mobile.ControlOrder )
			{
				case OrderType.None:
				return DoOrderNone();

				case OrderType.Come:
				return DoOrderCome();

				case OrderType.Drop:
				return DoOrderDrop();

				case OrderType.Friend:
				return DoOrderFriend();

				case OrderType.Unfriend:
				return DoOrderUnfriend();

				case OrderType.Guard:
				return DoOrderGuard();

				case OrderType.Attack:
				return DoOrderAttack();

				case OrderType.Patrol:
				return DoOrderPatrol();

				case OrderType.Release:
				return DoOrderRelease();

				case OrderType.Stay:
				return DoOrderStay();

				case OrderType.Stop:
				return DoOrderStop();

				case OrderType.Follow:
				return DoOrderFollow();

				case OrderType.Transfer:
				return DoOrderTransfer();

				default:
				return false;
			}
		}

		public virtual void OnCurrentOrderChanged()
		{
			if( m_Mobile.Deleted || m_Mobile.ControlMaster == null || m_Mobile.ControlMaster.Deleted )
				return;

			switch( m_Mobile.ControlOrder )
			{
				case OrderType.None:
				m_Mobile.ControlMaster.RevealingAction();
				m_Mobile.Home = m_Mobile.Location;
				m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
				m_Mobile.PlaySound( m_Mobile.GetIdleSound() );
				m_Mobile.Warmode = false;
				m_Mobile.Combatant = null;
				break;

				case OrderType.Come:
				m_Mobile.ControlMaster.RevealingAction();
				m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
				m_Mobile.PlaySound( m_Mobile.GetIdleSound() );
				m_Mobile.Warmode = false;
				m_Mobile.Combatant = null;
				break;

				case OrderType.Drop:
				m_Mobile.ControlMaster.RevealingAction();
				m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
				m_Mobile.PlaySound( m_Mobile.GetIdleSound() );
				m_Mobile.Warmode = true;
				m_Mobile.Combatant = null;
				break;

				case OrderType.Friend:
				case OrderType.Unfriend:
				m_Mobile.ControlMaster.RevealingAction();
				break;

				case OrderType.Guard:
				m_Mobile.ControlMaster.RevealingAction();
				m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
				m_Mobile.PlaySound( m_Mobile.GetIdleSound() );
				m_Mobile.Warmode = true;
				m_Mobile.Combatant = null;
				string petname = String.Format( "{0}", m_Mobile.Name );
				m_Mobile.ControlMaster.SendLocalizedMessage ( 1049671, petname );	//~1_PETNAME~ is now guarding you.
				break;

				case OrderType.Attack:
				m_Mobile.ControlMaster.RevealingAction();
				m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
				m_Mobile.PlaySound( m_Mobile.GetIdleSound() );

				m_Mobile.Warmode = true;
				m_Mobile.Combatant = null;
				break;

				case OrderType.Patrol:
				m_Mobile.ControlMaster.RevealingAction();
				m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
				m_Mobile.PlaySound( m_Mobile.GetIdleSound() );
				m_Mobile.Warmode = false;
				m_Mobile.Combatant = null;
				break;

				case OrderType.Release:
				m_Mobile.ControlMaster.RevealingAction();
				m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
				m_Mobile.PlaySound( m_Mobile.GetIdleSound() );
				m_Mobile.Warmode = false;
				m_Mobile.Combatant = null;
				break;

				case OrderType.Stay:
				m_Mobile.ControlMaster.RevealingAction();
				m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
				m_Mobile.PlaySound( m_Mobile.GetIdleSound() );
				m_Mobile.Warmode = false;
				m_Mobile.Combatant = null;
				break;

				case OrderType.Stop:
				m_Mobile.ControlMaster.RevealingAction();
				m_Mobile.Home = m_Mobile.Location;
				m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
				m_Mobile.PlaySound( m_Mobile.GetIdleSound() );
				m_Mobile.Warmode = false;
				m_Mobile.Combatant = null;
				break;

				case OrderType.Follow:
				m_Mobile.ControlMaster.RevealingAction();
				m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
				m_Mobile.PlaySound( m_Mobile.GetIdleSound() );

				m_Mobile.Warmode = false;
				m_Mobile.Combatant = null;
				break;

				case OrderType.Transfer:
				m_Mobile.ControlMaster.RevealingAction();
				m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
				m_Mobile.PlaySound( m_Mobile.GetIdleSound() );

				m_Mobile.Warmode = false;
				m_Mobile.Combatant = null;
				break;
			}
		}

		public virtual bool DoOrderNone()
		{
			m_Mobile.DebugSay( "I have no order" );

			WalkRandomInHome( 3, 2, 1 );

			if( m_Mobile.Combatant != null && !m_Mobile.Combatant.Deleted && m_Mobile.Combatant.Alive && !m_Mobile.Combatant.IsDeadBondedPet )
			{
				m_Mobile.Warmode = true;
				m_Mobile.Direction = m_Mobile.GetDirectionTo( m_Mobile.Combatant );
			}
			else
			{
				m_Mobile.Warmode = false;
			}

			return true;
		}

		public virtual bool DoOrderCome()
		{
			if( m_Mobile.ControlMaster != null && !m_Mobile.ControlMaster.Deleted )
			{
				int iCurrDist = (int)m_Mobile.GetDistanceToSqrt( m_Mobile.ControlMaster );

				if( iCurrDist > m_Mobile.RangePerception )
				{
					m_Mobile.DebugSay( "I have lost my master. I stay here" );
					m_Mobile.ControlTarget = null;
					m_Mobile.ControlOrder = OrderType.None;
				}
				else
				{
					m_Mobile.DebugSay( "My master told me come" );

					// Not exactly OSI style, but better than nothing.
					bool bRun = (iCurrDist > 5);

					if( WalkMobileRange( m_Mobile.ControlMaster, 1, bRun, 0, 1 ) )
					{
						if( m_Mobile.Combatant != null && !m_Mobile.Combatant.Deleted && m_Mobile.Combatant.Alive && !m_Mobile.Combatant.IsDeadBondedPet )
						{
							m_Mobile.Warmode = true;
							m_Mobile.Direction = m_Mobile.GetDirectionTo( m_Mobile.Combatant );
						}
						else
						{
							m_Mobile.Warmode = false;
						}
					}
				}
			}

			return true;
		}

		public virtual bool DoOrderDrop()
		{
			if( m_Mobile.IsDeadPet || !m_Mobile.CanDrop )
				return true;

			m_Mobile.DebugSay( "I drop my stuff for my master" );

			Container pack = m_Mobile.Backpack;

			if( pack != null )
			{
				List<Item> list = pack.Items;

				for( int i = list.Count - 1; i >= 0; --i )
					if( i < list.Count )
						list[i].MoveToWorld( m_Mobile.Location, m_Mobile.Map );
			}

			m_Mobile.ControlTarget = null;
			m_Mobile.ControlOrder = OrderType.None;

			return true;
		}

		public virtual bool CheckHerding()
		{
			IPoint2D target = m_Mobile.TargetLocation;

			if ( target == null )
				return false; // Creature is not being herded

			double distance = m_Mobile.GetDistanceToSqrt( target );

			if( distance < 1 || distance > 15 )
			{
				m_Mobile.TargetLocation = null;
				return false; // At the target or too far away
			}

			DoMove( m_Mobile.GetDirectionTo( target ) );

			return true;
		}

		public virtual bool DoOrderFollow()
		{
			if( CheckHerding() )
			{
				m_Mobile.DebugSay( "Praise the shepherd!" );
			}
			else if( m_Mobile.ControlTarget != null && !m_Mobile.ControlTarget.Deleted && m_Mobile.ControlTarget != m_Mobile )
			{
				int iCurrDist = (int)m_Mobile.GetDistanceToSqrt( m_Mobile.ControlTarget );

				if( iCurrDist > m_Mobile.RangePerception )
				{
					if ( m_Mobile.ControlTarget != null ){ m_Mobile.MoveToWorld( (m_Mobile.ControlTarget).Location, (m_Mobile.ControlTarget).Map ); }

					if( m_Mobile.Combatant != null && !m_Mobile.Combatant.Deleted && m_Mobile.Combatant.Alive && !m_Mobile.Combatant.IsDeadBondedPet )
					{
						m_Mobile.Warmode = true;
						m_Mobile.Direction = m_Mobile.GetDirectionTo( m_Mobile.Combatant );
					}
					else
					{
						m_Mobile.Warmode = false;
					}
				}
				else
				{
					m_Mobile.DebugSay( "My master told me to follow: {0}", m_Mobile.ControlTarget.Name );

					// Not exactly OSI style, but better than nothing.
					bool bRun = (iCurrDist > 5);

					if( WalkMobileRange( m_Mobile.ControlTarget, 1, bRun, 0, 1 ) )
					{
						if( m_Mobile.Combatant != null && !m_Mobile.Combatant.Deleted && m_Mobile.Combatant.Alive && !m_Mobile.Combatant.IsDeadBondedPet )
						{
							m_Mobile.Warmode = true;
							m_Mobile.Direction = m_Mobile.GetDirectionTo( m_Mobile.Combatant );
						}
						else
						{
							m_Mobile.Warmode = false;
							m_Mobile.CurrentSpeed = 0.1;
						}
					}
				}
			}
			else
			{
				m_Mobile.DebugSay( "I have nobody to follow" );
				m_Mobile.ControlTarget = null;
				m_Mobile.ControlOrder = OrderType.None;
			}

			return true;
		}

		public virtual bool DoOrderFriend()
		{
			Mobile from = m_Mobile.ControlMaster;
			Mobile to = m_Mobile.ControlTarget;

			if( from == null || to == null || from == to || from.Deleted || to.Deleted || !to.Player )
			{
				m_Mobile.PublicOverheadMessage( MessageType.Regular, 0x3B2, 502039 ); // *looks confused*
			}
			else
			{
				bool youngFrom = from is PlayerMobile ? ((PlayerMobile)from).Young : false;
				bool youngTo = to is PlayerMobile ? ((PlayerMobile)to).Young : false;

				if( youngFrom && !youngTo )
				{
					from.SendLocalizedMessage( 502040 ); // As a young player, you may not friend pets to older players.
				}
				else if( !youngFrom && youngTo )
				{
					from.SendLocalizedMessage( 502041 ); // As an older player, you may not friend pets to young players.
				}
				else if( from.CanBeBeneficial( to, true ) )
				{
					NetState fromState = from.NetState, toState = to.NetState;

					if( fromState != null && toState != null )
					{
						if( from.HasTrade )
						{
							from.SendLocalizedMessage( 1070947 ); // You cannot friend a pet with a trade pending
						}
						else if( to.HasTrade )
						{
							to.SendLocalizedMessage( 1070947 ); // You cannot friend a pet with a trade pending
						}
						else if( m_Mobile.IsPetFriend( to ) )
						{
							from.SendLocalizedMessage( 1049691 ); // That person is already a friend.
						}
						else if( !m_Mobile.AllowNewPetFriend )
						{
							from.SendLocalizedMessage( 1005482 ); // Your pet does not seem to be interested in making new friends right now.
						}
						else
						{
							// ~1_NAME~ will now accept movement commands from ~2_NAME~.
							from.SendLocalizedMessage( 1049676, String.Format( "{0}\t{1}", m_Mobile.Name, to.Name ) );

							/* ~1_NAME~ has granted you the ability to give orders to their pet ~2_PET_NAME~.
							 * This creature will now consider you as a friend.
							 */
							to.SendLocalizedMessage( 1043246, String.Format( "{0}\t{1}", from.Name, m_Mobile.Name ) );

							m_Mobile.AddPetFriend( to );

							m_Mobile.ControlTarget = to;
							m_Mobile.ControlOrder = OrderType.Follow;

							return true;
						}
					}
				}
			}

			m_Mobile.ControlTarget = from;
			m_Mobile.ControlOrder = OrderType.Follow;

			return true;
		}

		public virtual bool DoOrderUnfriend()
		{
			Mobile from = m_Mobile.ControlMaster;
			Mobile to = m_Mobile.ControlTarget;

			if( from == null || to == null || from == to || from.Deleted || to.Deleted || !to.Player )
			{
				m_Mobile.PublicOverheadMessage( MessageType.Regular, 0x3B2, 502039 ); // *looks confused*
			}
			else if( !m_Mobile.IsPetFriend( to ) )
			{
				from.SendLocalizedMessage( 1070953 ); // That person is not a friend.
			}
			else
			{
				// ~1_NAME~ will no longer accept movement commands from ~2_NAME~.
				from.SendLocalizedMessage( 1070951, String.Format( "{0}\t{1}", m_Mobile.Name, to.Name ) );

				/* ~1_NAME~ has no longer granted you the ability to give orders to their pet ~2_PET_NAME~.
				 * This creature will no longer consider you as a friend.
				 */
				to.SendLocalizedMessage( 1070952, String.Format( "{0}\t{1}", from.Name, m_Mobile.Name ) );

				m_Mobile.RemovePetFriend( to );
			}

			m_Mobile.ControlTarget = from;
			m_Mobile.ControlOrder = OrderType.Follow;

			return true;
		}

		public virtual bool DoOrderGuard()
		{
			if( m_Mobile.IsDeadPet )
				return true;

			Mobile controlMaster = m_Mobile.ControlMaster;

			if( controlMaster == null || controlMaster.Deleted )
				return true;

			Mobile combatant = m_Mobile.Combatant;

			List<AggressorInfo> aggressors = controlMaster.Aggressors;

			if( aggressors.Count > 0 )
			{
				for( int i = 0; i < aggressors.Count; ++i )
				{
					AggressorInfo info = aggressors[i];
					Mobile attacker = info.Attacker;

					if( attacker != null && !attacker.Deleted && attacker.GetDistanceToSqrt( m_Mobile ) <= m_Mobile.RangePerception )
					{
						if( combatant == null || attacker.GetDistanceToSqrt( controlMaster ) < combatant.GetDistanceToSqrt( controlMaster ) )
							combatant = attacker;
					}
				}
			}

			List<AggressorInfo> aggressed = controlMaster.Aggressed;

			if( aggressed.Count > 0 )
			{
				for ( int i = 0; i < aggressed.Count; ++i )
				{
					AggressorInfo info = aggressed[i];
					Mobile defender = info.Defender;

					if( defender != null && !defender.Deleted && defender.GetDistanceToSqrt( m_Mobile ) <= m_Mobile.RangePerception )
					{
						if( combatant == null || defender.GetDistanceToSqrt( controlMaster ) < combatant.GetDistanceToSqrt( controlMaster ) )
							combatant = defender;
					}
				}
			}

			if ( combatant == null && MySettings.S_FriendsGuardFriends )
			{
				foreach ( Mobile friend in m_Mobile.GetMobilesInRange( m_Mobile.RangePerception ) )
				{
					if ( friend is BaseCreature && ((BaseCreature)friend).Controlled && ((BaseCreature)friend).ControlMaster == controlMaster )
					{
						List<AggressorInfo> enemies = friend.Aggressors;

						if( enemies.Count > 0 )
						{
							for( int i = 0; i < enemies.Count; ++i )
							{
								AggressorInfo data = enemies[i];
								Mobile foe = data.Attacker;

								if( foe != null && !foe.Deleted && foe.GetDistanceToSqrt( m_Mobile ) <= m_Mobile.RangePerception )
								{
									if( combatant == null || foe.GetDistanceToSqrt( controlMaster ) < combatant.GetDistanceToSqrt( controlMaster ) )
										combatant = foe;
								}
							}
						}

						List<AggressorInfo> bullies = friend.Aggressed;

						if( bullies.Count > 0 )
						{
							for ( int i = 0; i < bullies.Count; ++i )
							{
								AggressorInfo info = bullies[i];
								Mobile defender = info.Defender;

								if( defender != null && !defender.Deleted && defender.GetDistanceToSqrt( m_Mobile ) <= m_Mobile.RangePerception )
								{
									if( combatant == null || defender.GetDistanceToSqrt( controlMaster ) < combatant.GetDistanceToSqrt( controlMaster ) )
										combatant = defender;
								}
							}
						}
					}
				}
			}

			if( combatant != null && combatant != m_Mobile && combatant != m_Mobile.ControlMaster && !combatant.Deleted && combatant.Alive && !combatant.IsDeadBondedPet && m_Mobile.CanSee( combatant ) && m_Mobile.CanBeHarmful( combatant, false ) && combatant.Map == m_Mobile.Map )
			{
				m_Mobile.DebugSay( "Guarding from target..." );

				m_Mobile.Combatant = combatant;
				m_Mobile.FocusMob = combatant;
				Action = ActionType.Combat;

				/*
				 * We need to call Think() here or spell casting monsters will not use
				 * spells when guarding because their target is never processed.
				 */
				Think();
			}
			else
			{
				m_Mobile.DebugSay( "Nothing to guard from" );

				m_Mobile.Warmode = false;
				m_Mobile.CurrentSpeed = 0.1;

				WalkMobileRange( controlMaster, 1, false, 0, 1 );
			}

			return true;
		}

		public virtual bool DoOrderAttack()
		{
			if( m_Mobile.IsDeadPet )
				return true;

			if( m_Mobile.ControlTarget == null || m_Mobile.ControlTarget.Deleted || m_Mobile.ControlTarget.Map != m_Mobile.Map || !m_Mobile.ControlTarget.Alive || m_Mobile.ControlTarget.IsDeadBondedPet )
			{
				m_Mobile.DebugSay( "I think he might be dead. He's not anywhere around here at least. That's cool. I'm glad he's dead." );

				m_Mobile.ControlTarget = m_Mobile.ControlMaster;
				m_Mobile.ControlOrder = OrderType.Follow;

				if( m_Mobile.FightMode == FightMode.Closest || m_Mobile.FightMode == FightMode.Aggressor )
				{
					Mobile newCombatant = null;
					double newScore = 0.0;

					foreach( Mobile aggr in m_Mobile.GetMobilesInRange( m_Mobile.RangePerception ) )
					{
						if( !m_Mobile.CanSee( aggr ) || aggr.Combatant != m_Mobile )
							continue;

						if( aggr.IsDeadBondedPet || !aggr.Alive )
							continue;

						double aggrScore = m_Mobile.GetFightModeRanking( aggr, FightMode.Closest, false );

						if( (newCombatant == null || aggrScore > newScore) && m_Mobile.InLOS( aggr ) )
						{
							newCombatant = aggr;
							newScore = aggrScore;
						}
					}

					if( newCombatant != null )
					{
						m_Mobile.ControlTarget = newCombatant;
						m_Mobile.ControlOrder = OrderType.Attack;
						m_Mobile.Combatant = newCombatant;
						m_Mobile.DebugSay( "But -that- is not dead. Here we go again..." );
						Think();
					}
				}
			}
			else
			{
				m_Mobile.DebugSay( "Attacking target..." );
				Think();
			}

			return true;
		}

		public virtual bool DoOrderPatrol()
		{
			m_Mobile.DebugSay( "This order is not yet coded" );
			return true;
		}

		public virtual bool DoOrderRelease()
		{
			if ( m_Mobile is HenchmanArcher || m_Mobile is HenchmanMonster || m_Mobile is HenchmanFighter || m_Mobile is HenchmanWizard )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				if ( item is HenchmanItem )
				{
					HenchmanItem henchItem = (HenchmanItem)item;
					if ( henchItem.HenchSerial == m_Mobile.Serial )
					{
						targets.Add( item );
					}
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					HenchmanItem henchThing = (HenchmanItem)item;
					henchThing.LootType = LootType.Regular;
					henchThing.HenchSerial = 0;
					henchThing.Visible = true;
					henchThing.HenchTimer = m_Mobile.Fame;
					henchThing.HenchBandages = m_Mobile.Hunger;
					henchThing.InvalidateProperties();
				}
			}
			else if( m_Mobile is HenchmanFamiliar )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				if ( item is HenchmanFamiliarItem )
				{
					HenchmanFamiliarItem henchItem = (HenchmanFamiliarItem)item;
					if ( henchItem.FamiliarSerial == m_Mobile.Serial )
					{
						targets.Add( item );
					}
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					HenchmanFamiliarItem henchThing = (HenchmanFamiliarItem)item;
					henchThing.LootType = LootType.Regular;
					henchThing.FamiliarSerial = 0;
					henchThing.FamiliarName = m_Mobile.Name;
					henchThing.FamiliarType = m_Mobile.Body;
					henchThing.Hue = m_Mobile.Hue;
					henchThing.Visible = true;
					henchThing.InvalidateProperties();
				}

				ArrayList bagitems = new ArrayList(m_Mobile.Backpack.Items);
				foreach (Item item in bagitems)
				{
					if ((item.Layer != Layer.Bank) && (item.Layer != Layer.Backpack) && (item.Layer != Layer.Hair) && (item.Layer != Layer.FacialHair) && (item.Layer != Layer.Mount))
					{
						item.MoveToWorld(m_Mobile.Location, m_Mobile.Map);
					}
				}
			}
			else if( m_Mobile is PackBeast )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				if ( item is PackBeastItem )
				{
					PackBeastItem henchItem = (PackBeastItem)item;
					if ( henchItem.PorterSerial == m_Mobile.Serial )
					{
						targets.Add( item );
					}
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					PackBeastItem henchThing = (PackBeastItem)item;
					henchThing.LootType = LootType.Regular;
					henchThing.PorterSerial = 0;
					henchThing.PorterName = m_Mobile.Name;
					henchThing.PorterType = m_Mobile.Body;
					henchThing.Hue = m_Mobile.Hue;
					henchThing.Visible = true;
					henchThing.InvalidateProperties();
				}

				ArrayList bagitems = new ArrayList(m_Mobile.Backpack.Items);
				foreach (Item item in bagitems)
				{
					if ((item.Layer != Layer.Bank) && (item.Layer != Layer.Backpack) && (item.Layer != Layer.Hair) && (item.Layer != Layer.FacialHair) && (item.Layer != Layer.Mount))
					{
						item.MoveToWorld(m_Mobile.Location, m_Mobile.Map);
					}
				}
			}
			else if( m_Mobile is FrankenPorter || m_Mobile is FrankenFighter )
			{
				Server.Items.FrankenPorterItem.Stash( m_Mobile );
			}
			else if( m_Mobile is GolemPorter || m_Mobile is GolemFighter )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				if ( item is GolemPorterItem )
				{
					GolemPorterItem henchItem = (GolemPorterItem)item;
					if ( henchItem.PorterSerial == m_Mobile.Serial )
					{
						targets.Add( item );
					}
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					GolemPorterItem henchThing = (GolemPorterItem)item;
					henchThing.LootType = LootType.Regular;
					henchThing.PorterSerial = 0;
					henchThing.PorterName = m_Mobile.Name;
					henchThing.Visible = true;
					henchThing.Hue = m_Mobile.Hue;
					m_Mobile.PlaySound( 0x665 );
					henchThing.InvalidateProperties();
				}

				if ( m_Mobile is GolemPorter )
				{
					ArrayList bagitems = new ArrayList(m_Mobile.Backpack.Items);
					foreach (Item item in bagitems)
					{
						if ((item.Layer != Layer.Bank) && (item.Layer != Layer.Backpack) && (item.Layer != Layer.Hair) && (item.Layer != Layer.FacialHair) && (item.Layer != Layer.Mount))
						{
							item.MoveToWorld(m_Mobile.Location, m_Mobile.Map);
						}
					}
				}
			}
			else if( m_Mobile is Robot )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				if ( item is RobotItem )
				{
					RobotItem robotItem = (RobotItem)item;
					if ( robotItem.RobotSerial == m_Mobile.Serial )
					{
						targets.Add( item );
					}
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					RobotItem robotThing = (RobotItem)item;
					robotThing.LootType = LootType.Regular;
					robotThing.RobotSerial = 0;
					robotThing.RobotName = m_Mobile.Name;
					robotThing.Visible = true;
					robotThing.Hue = m_Mobile.Hue;
					m_Mobile.PlaySound( 0x559 );
					robotThing.InvalidateProperties();
				}
			}
			else if ( !(Server.Mobiles.BaseCreature.AlwaysInvulnerable( m_Mobile )) && m_Mobile.Blessed )
			{
				m_Mobile.Blessed = false;
			}

			if ( MyServerSettings.FastFriends( m_Mobile ) )
			{
				Server.Misc.HenchmanFunctions.ForceSlow( m_Mobile );
			}

			m_Mobile.DebugSay( "I have been released" );

			m_Mobile.PlaySound( m_Mobile.GetAngerSound() );

			m_Mobile.SetControlMaster( null );
			m_Mobile.SummonMaster = null;

			m_Mobile.BondingBegin = DateTime.MinValue;
			m_Mobile.OwnerAbandonTime = DateTime.MinValue;
			m_Mobile.IsBonded = false;

			SpawnEntry se = m_Mobile.Spawner as SpawnEntry;
			if( se != null && se.HomeLocation != Point3D.Zero )
			{
				m_Mobile.Home = se.HomeLocation;
				m_Mobile.RangeHome = se.HomeRange;
			}

			if( m_Mobile.DeleteOnRelease || m_Mobile.IsDeadPet )
				m_Mobile.Delete();
			
			m_Mobile.BeginDeleteTimer();
			m_Mobile.DropBackpack();

			return true;
		}

		public virtual bool DoOrderStay()
		{
			if( CheckHerding() )
				m_Mobile.DebugSay( "Praise the shepherd!" );
			else
				m_Mobile.DebugSay( "My master told me to stay" );

			//m_Mobile.Direction = m_Mobile.GetDirectionTo( m_Mobile.ControlMaster );

			return true;
		}

		public virtual bool DoOrderStop()
		{
			if( m_Mobile.ControlMaster == null || m_Mobile.ControlMaster.Deleted )
				return true;

			m_Mobile.DebugSay( "My master told me to stop." );

			m_Mobile.Direction = m_Mobile.GetDirectionTo( m_Mobile.ControlMaster );
			m_Mobile.Home = m_Mobile.Location;

			m_Mobile.ControlTarget = null;

			if( Core.ML )
			{
				WalkRandomInHome( 3, 2, 1 );
			}
			else
			{
				m_Mobile.ControlOrder = OrderType.None;
			}

			return true;
		}

		private class TransferItem : Item
		{
			public static bool IsInCombat( BaseCreature creature )
			{
				return (creature != null && (creature.Aggressors.Count > 0 || creature.Aggressed.Count > 0));
			}

			private BaseCreature m_Creature;

			public TransferItem( BaseCreature creature )
				: base( ShrinkTable.Lookup( creature ) )
			{
				m_Creature = creature;

				Movable = false;

				if( !Core.AOS )
				{
					Name = creature.Name;
				}
				else if( this.ItemID == ShrinkTable.DefaultItemID || creature.GetType().IsDefined( typeof( FriendlyNameAttribute ), false ) )
					Name = FriendlyNameAttribute.GetFriendlyNameFor( creature.GetType() ).ToString();

				//(As Per OSI)No name.  Normally, set by the ItemID of the Shrink Item unless we either explicitly set it with an Attribute, or, no lookup found
				
				Hue = creature.Hue & 0x0FFF;
			}

			public TransferItem( Serial serial )
				: base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int)0 ); // version
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				Delete();
			}

			public override void GetProperties( ObjectPropertyList list )
			{
				base.GetProperties( list );

				list.Add( 1041603 ); // This item represents a pet currently in consideration for trade
				list.Add( 1041601, m_Creature.Name ); // Pet Name: ~1_val~

				if ( m_Creature.ControlMaster != null )
					list.Add( 1041602, m_Creature.ControlMaster.Name ); // Owner: ~1_val~
			}

			public override bool AllowSecureTrade( Mobile from, Mobile to, Mobile newOwner, bool accepted )
			{
				if( !base.AllowSecureTrade( from, to, newOwner, accepted ) )
					return false;

				if( Deleted || m_Creature == null || m_Creature.Deleted || m_Creature.ControlMaster != from || !from.CheckAlive() || !to.CheckAlive() )
					return false;

				if( from.Map != m_Creature.Map || !from.InRange( m_Creature, 14 ) )
					return false;

				bool youngFrom = from is PlayerMobile ? ((PlayerMobile)from).Young : false;
				bool youngTo = to is PlayerMobile ? ((PlayerMobile)to).Young : false;

				if( accepted && youngFrom && !youngTo )
				{
					from.SendLocalizedMessage( 502051 ); // As a young player, you may not transfer pets to older players.
				}
				else if( accepted && !youngFrom && youngTo )
				{
					from.SendLocalizedMessage( 502052 ); // As an older player, you may not transfer pets to young players.
				}
				else if( accepted && !m_Creature.CanBeControlledBy( to ) )
				{
					string args = String.Format( "{0}\t{1}\t ", to.Name, from.Name );

					from.SendLocalizedMessage( 1043248, args ); // The pet refuses to be transferred because it will not obey ~1_NAME~.~3_BLANK~
					to.SendLocalizedMessage( 1043249, args ); // The pet will not accept you as a master because it does not trust you.~3_BLANK~

					return false;
				}
				else if( accepted && !m_Creature.CanBeControlledBy( from ) )
				{
					string args = String.Format( "{0}\t{1}\t ", to.Name, from.Name );

					from.SendLocalizedMessage( 1043250, args ); // The pet refuses to be transferred because it will not obey you sufficiently.~3_BLANK~
					to.SendLocalizedMessage( 1043251, args ); // The pet will not accept you as a master because it does not trust ~2_NAME~.~3_BLANK~
				}
				else if( accepted && (to.Followers + m_Creature.ControlSlots) > to.FollowersMax )
				{
					to.SendLocalizedMessage( 1049607 ); // You have too many followers to control that creature.

					return false;
				}
				else if( accepted && IsInCombat( m_Creature ) )
				{
					from.SendMessage( "You may not transfer a pet that has recently been in combat." );
					to.SendMessage( "The pet may not be transfered to you because it has recently been in combat." );

					return false;
				}

				return true;
			}

			public override void OnSecureTrade( Mobile from, Mobile to, Mobile newOwner, bool accepted )
			{
				if( Deleted )
					return;

				Delete();

				if( m_Creature == null || m_Creature.Deleted || m_Creature.ControlMaster != from || !from.CheckAlive() || !to.CheckAlive() )
					return;

				if( from.Map != m_Creature.Map || !from.InRange( m_Creature, 14 ) )
					return;

				if( accepted )
				{
					if( m_Creature.SetControlMaster( to ) )
					{
						if( m_Creature.Summoned )
							m_Creature.SummonMaster = to;

						m_Creature.ControlTarget = to;
						m_Creature.ControlOrder = OrderType.Follow;

						m_Creature.BondingBegin = DateTime.MinValue;
						m_Creature.OwnerAbandonTime = DateTime.MinValue;
						m_Creature.IsBonded = false;
						m_Creature.Level = 1; // Reset level after trading!

						m_Creature.PlaySound( m_Creature.GetIdleSound() );

						string args = String.Format( "{0}\t{1}\t{2}", from.Name, m_Creature.Name, to.Name );

						from.SendLocalizedMessage( 1043253, args ); // You have transferred your pet to ~3_GETTER~.
						to.SendLocalizedMessage( 1043252, args ); // ~1_NAME~ has transferred the allegiance of ~2_PET_NAME~ to you.
					}
				}
			}
		}

		public virtual bool DoOrderTransfer()
		{
			if( m_Mobile.IsDeadPet )
				return true;

			Mobile from = m_Mobile.ControlMaster;
			Mobile to = m_Mobile.ControlTarget;

			if( from != to && from != null && !from.Deleted && to != null && !to.Deleted && to.Player )
			{
				m_Mobile.DebugSay( "Begin transfer with {0}", to.Name );

				bool youngFrom = from is PlayerMobile ? ((PlayerMobile)from).Young : false;
				bool youngTo = to is PlayerMobile ? ((PlayerMobile)to).Young : false;

				if( youngFrom && !youngTo )
				{
					from.SendLocalizedMessage( 502051 ); // As a young player, you may not transfer pets to older players.
				}
				else if( m_Mobile is HenchmanMonster || m_Mobile is HenchmanFighter || m_Mobile is HenchmanWizard || m_Mobile is HenchmanArcher )
				{
					from.SendMessage( "This is not some slave you can trade." );
					to.SendMessage( "This is not some slave you can take control of." );
				}
				else if( m_Mobile is AerialServant )
				{
					from.SendMessage( "You cannot give away an elemental." );
					to.SendMessage( "You cannot take another wizard's elemental." );
				}
				else if( m_Mobile is HenchmanFamiliar )
				{
					from.SendMessage( "You cannot give away a familiar." );
					to.SendMessage( "You cannot take another wizard's familiar." );
				}
				else if( m_Mobile is PackBeast )
				{
					from.SendMessage( "You cannot give away a mystical pack animal." );
					to.SendMessage( "You cannot take another's mystical pack animal." );
				}
				else if( m_Mobile is GolemPorter || m_Mobile is GolemFighter )
				{
					from.SendMessage( "You cannot give away a golem." );
					to.SendMessage( "You cannot take another's golem." );
				}
				else if( m_Mobile is FrankenPorter || m_Mobile is FrankenFighter )
				{
					from.SendMessage( "You cannot give away a reanimation." );
					to.SendMessage( "You cannot take another's reanimation." );
				}
				else if( !youngFrom && youngTo )
				{
					from.SendLocalizedMessage( 502052 ); // As an older player, you may not transfer pets to young players.
				}
				else if( !m_Mobile.CanBeControlledBy( to ) )
				{
					string args = String.Format( "{0}\t{1}\t ", to.Name, from.Name );

					from.SendLocalizedMessage( 1043248, args ); // The pet refuses to be transferred because it will not obey ~1_NAME~.~3_BLANK~
					to.SendLocalizedMessage( 1043249, args ); // The pet will not accept you as a master because it does not trust you.~3_BLANK~
				}
				else if( !m_Mobile.CanBeControlledBy( from ) )
				{
					string args = String.Format( "{0}\t{1}\t ", to.Name, from.Name );

					from.SendLocalizedMessage( 1043250, args ); // The pet refuses to be transferred because it will not obey you sufficiently.~3_BLANK~
					to.SendLocalizedMessage( 1043251, args ); // The pet will not accept you as a master because it does not trust ~2_NAME~.~3_BLANK~
				}
				else if( TransferItem.IsInCombat( m_Mobile ) )
				{
					from.SendMessage( "You may not transfer a pet that has recently been in combat." );
					to.SendMessage( "The pet may not be transferred to you because it has recently been in combat." );
				}
				else
				{
					NetState fromState = from.NetState, toState = to.NetState;

					if( fromState != null && toState != null )
					{
						if( from.HasTrade )
						{
							from.SendLocalizedMessage( 1010507 ); // You cannot transfer a pet with a trade pending
						}
						else if( to.HasTrade )
						{
							to.SendLocalizedMessage( 1010507 ); // You cannot transfer a pet with a trade pending
						}
						else
						{
							ConfirmationGump.PromptIfFalse(
								from,
								1 < m_Mobile.Level,
								() =>
									{
										Container c = fromState.AddTrade( toState );
										c.DropItem( new TransferItem( m_Mobile ) );
									},
								onConfirmed => new ConfirmationGump(from, "Confirm Transfer", "Transferring this pet will reduce the level to 1. Are you sure you want to transfer the pet?", onConfirmed)
							);
						}
					}
				}
			}

			m_Mobile.ControlTarget = null;
			m_Mobile.ControlOrder = OrderType.Stay;

			return true;
		}

		public virtual bool DoBardPacified()
		{
			if( DateTime.Now < m_Mobile.BardEndTime )
			{
				m_Mobile.DebugSay( "I am pacified, I wait" );
				m_Mobile.Combatant = null;
				m_Mobile.Warmode = false;

			}
			else
			{
				m_Mobile.DebugSay( "I'm not pacified any longer" );
				m_Mobile.BardPacified = false;
			}

			return true;
		}

		public virtual bool DoBardProvoked()
		{
			if( DateTime.Now >= m_Mobile.BardEndTime && (m_Mobile.BardMaster == null || m_Mobile.BardMaster.Deleted || m_Mobile.BardMaster.Map != m_Mobile.Map || m_Mobile.GetDistanceToSqrt( m_Mobile.BardMaster ) > m_Mobile.RangePerception) )
			{
				m_Mobile.DebugSay( "I have lost my provoker" );
				m_Mobile.BardProvoked = false;
				m_Mobile.BardMaster = null;
				m_Mobile.BardTarget = null;

				m_Mobile.Combatant = null;
				m_Mobile.Warmode = false;
			}
			else
			{
				if( m_Mobile.BardTarget == null || m_Mobile.BardTarget.Deleted || m_Mobile.BardTarget.Map != m_Mobile.Map || m_Mobile.GetDistanceToSqrt( m_Mobile.BardTarget ) > m_Mobile.RangePerception )
				{
					m_Mobile.DebugSay( "I have lost my provoke target" );
					m_Mobile.BardProvoked = false;
					m_Mobile.BardMaster = null;
					m_Mobile.BardTarget = null;

					m_Mobile.Combatant = null;
					m_Mobile.Warmode = false;
				}
				else
				{
					m_Mobile.Combatant = m_Mobile.BardTarget;
					m_Action = ActionType.Combat;

					m_Mobile.OnThink();
					Think();
				}
			}

			return true;
		}

		public virtual void WalkRandom( int iChanceToNotMove, int iChanceToDir, int iSteps )
		{
			if( m_Mobile.Deleted || m_Mobile.DisallowAllMoves )
				return;

			for( int i=0; i<iSteps; i++ )
			{
				if( Utility.Random( 8 * iChanceToNotMove ) <= 8 )
				{
					int iRndMove = Utility.Random( 0, 8 + (9*iChanceToDir) );

					switch( iRndMove )
					{
						case 0:
						DoMove( Direction.Up );
						break;
						case 1:
						DoMove( Direction.North );
						break;
						case 2:
						DoMove( Direction.Left );
						break;
						case 3:
						DoMove( Direction.West );
						break;
						case 5:
						DoMove( Direction.Down );
						break;
						case 6:
						DoMove( Direction.South );
						break;
						case 7:
						DoMove( Direction.Right );
						break;
						case 8:
						DoMove( Direction.East );
						break;
						default:
						DoMove( m_Mobile.Direction );
						break;
					}
				}
			}
		}

		public double TransformMoveDelay( double delay )
		{
			bool isPassive = (delay == m_Mobile.PassiveSpeed);
			bool isControlled = (m_Mobile.Controlled || m_Mobile.Summoned);

			if( delay == 0.2 )
				delay = 0.3;
			else if( delay == 0.25 )
				delay = 0.45;
			else if( delay == 0.3 )
				delay = 0.6;
			else if( delay == 0.4 )
				delay = 0.9;
			else if( delay == 0.5 )
				delay = 1.05;
			else if( delay == 0.6 )
				delay = 1.2;
			else if( delay == 0.8 )
				delay = 1.5;

			if( isPassive )
				delay += 0.2;

			if( !isControlled )
			{
				delay += 0.1;
			}
			else if( m_Mobile.Controlled )
			{
				if( m_Mobile.ControlOrder == OrderType.Follow && m_Mobile.ControlTarget == m_Mobile.ControlMaster )
					delay *= 0.5;

				delay -= 0.075;
			}

			if ( m_Mobile.ReduceSpeedWithDamage || m_Mobile.IsSubdued )
			{
				double offset = (double) m_Mobile.Hits / m_Mobile.HitsMax;

				if ( offset < 0.0 )
					offset = 0.0;
				else if ( offset > 1.0 )
					offset = 1.0;

				offset = 1.0 - offset;

				delay += ( offset * 0.8 );
			}

			if( delay < 0.0 )
				delay = 0.0;

			return delay;
		}

		private DateTime m_NextMove;

		public DateTime NextMove
		{
			get { return m_NextMove; }
			set { m_NextMove = value; }
		}

		public virtual bool CheckMove()
		{
			return (DateTime.Now >= m_NextMove);
		}

		public virtual bool DoMove( Direction d )
		{
			return DoMove( d, false );
		}

		public virtual bool DoMove( Direction d, bool badStateOk )
		{
			MoveResult res = DoMoveImpl( d );

			return (res == MoveResult.Success || res == MoveResult.SuccessAutoTurn || (badStateOk && res == MoveResult.BadState));
		}

		private static Queue m_Obstacles = new Queue();

		public virtual MoveResult DoMoveImpl( Direction d )
		{
			if( m_Mobile.Deleted || m_Mobile.Frozen || m_Mobile.Paralyzed || (m_Mobile.Spell != null && m_Mobile.Spell.IsCasting) || m_Mobile.DisallowAllMoves )
				return MoveResult.BadState;
			else if( !CheckMove() )
				return MoveResult.BadState;

			// This makes them always move one step, never any direction changes
			m_Mobile.Direction = d;

			TimeSpan delay = TimeSpan.FromSeconds( TransformMoveDelay( m_Mobile.CurrentSpeed ) );

			m_NextMove += delay;

			if( m_NextMove < DateTime.Now )
				m_NextMove = DateTime.Now;

			m_Mobile.Pushing = false;

			MoveImpl.IgnoreMovableImpassables = (m_Mobile.CanMoveOverObstacles && !m_Mobile.CanDestroyObstacles);

			if( (m_Mobile.Direction & Direction.Mask) != (d & Direction.Mask) )
			{
				bool v = m_Mobile.Move( d );

				MoveImpl.IgnoreMovableImpassables = false;
				return (v ? MoveResult.Success : MoveResult.Blocked);
			}
			else if( !m_Mobile.Move( d ) )
			{
				bool wasPushing = m_Mobile.Pushing;

				bool blocked = true;

				bool canOpenDoors = m_Mobile.CanOpenDoors;
				bool canDestroyObstacles = m_Mobile.CanDestroyObstacles;

				if( canOpenDoors || canDestroyObstacles )
				{
					m_Mobile.DebugSay( "My movement was blocked, I will try to clear some obstacles." );

					Map map = m_Mobile.Map;

					if( map != null )
					{
						int x = m_Mobile.X, y = m_Mobile.Y;
						Movement.Movement.Offset( d, ref x, ref y );

						int destroyables = 0;

						IPooledEnumerable eable = map.GetItemsInRange( new Point3D( x, y, m_Mobile.Location.Z ), 1 );

						foreach( Item item in eable )
						{
							if( canOpenDoors && item is BaseDoor && (item.Z + item.ItemData.Height) > m_Mobile.Z && (m_Mobile.Z + 16) > item.Z )
							{
								if( item.X != x || item.Y != y )
									continue;

								BaseDoor door = (BaseDoor)item;

								if( !door.Locked || !door.UseLocks() )
									m_Obstacles.Enqueue( door );

								if( !canDestroyObstacles )
									break;
							}
							else if( canDestroyObstacles && item.Movable && item.ItemData.Impassable && (item.Z + item.ItemData.Height) > m_Mobile.Z && (m_Mobile.Z + 16) > item.Z )
							{
								if( !m_Mobile.InRange( item.GetWorldLocation(), 1 ) )
									continue;

								m_Obstacles.Enqueue( item );
								++destroyables;
							}
						}

						eable.Free();

						if( destroyables > 0 )
							Effects.PlaySound( new Point3D( x, y, m_Mobile.Z ), m_Mobile.Map, 0x3B3 );

						if( m_Obstacles.Count > 0 )
							blocked = false; // retry movement

						while( m_Obstacles.Count > 0 )
						{
							Item item = (Item)m_Obstacles.Dequeue();

							if( item is BaseDoor )
							{
								m_Mobile.DebugSay( "Little do they expect, I've learned how to open doors. Didn't they read the script??" );
								m_Mobile.DebugSay( "*twist*" );

								((BaseDoor)item).Use( m_Mobile );
							}
							else
							{
								m_Mobile.DebugSay( "Ugabooga. I'm so big and tough I can destroy it: {0}", item.GetType().Name );

								if( item is Container )
								{
									Container cont = (Container)item;

									for( int i = 0; i < cont.Items.Count; ++i )
									{
										Item check = cont.Items[i];

										if( check.Movable && check.ItemData.Impassable && (item.Z + check.ItemData.Height) > m_Mobile.Z )
											m_Obstacles.Enqueue( check );
									}

									cont.Destroy();
								}
								else
								{
									item.Delete();
								}
							}
						}

						if( !blocked )
							blocked = !m_Mobile.Move( d );
					}
				}

				if( blocked )
				{
					int offset = (Utility.RandomDouble() >= 0.6 ? 1 : -1);

					for( int i = 0; i < 2; ++i )
					{
						m_Mobile.TurnInternal( offset );

						if( m_Mobile.Move( m_Mobile.Direction ) )
						{
							MoveImpl.IgnoreMovableImpassables = false;
							return MoveResult.SuccessAutoTurn;
						}
					}

					MoveImpl.IgnoreMovableImpassables = false;
					return (wasPushing ? MoveResult.BadState : MoveResult.Blocked);
				}
				else
				{
					MoveImpl.IgnoreMovableImpassables = false;
					return MoveResult.Success;
				}
			}

			MoveImpl.IgnoreMovableImpassables = false;
			return MoveResult.Success;
		}

		public virtual void WalkRandomInHome( int iChanceToNotMove, int iChanceToDir, int iSteps )
		{
			if( m_Mobile.Deleted || m_Mobile.DisallowAllMoves )
				return;

			if( m_Mobile.Home == Point3D.Zero )
			{
				if( m_Mobile.Spawner is SpawnEntry )
				{
					Region region = ((SpawnEntry)m_Mobile.Spawner).Region;

					if( m_Mobile.Region.AcceptsSpawnsFrom( region ) )
					{
						m_Mobile.WalkRegion = region;
						WalkRandom( iChanceToNotMove, iChanceToDir, iSteps );
						m_Mobile.WalkRegion = null;
					}
					else
					{
						if( region.GoLocation != Point3D.Zero && Utility.Random( 10 ) > 5 )
						{
							DoMove( m_Mobile.GetDirectionTo( region.GoLocation ) );
						}
						else
						{
							WalkRandom( iChanceToNotMove, iChanceToDir, 1 );
						}
					}
				}
				else
				{
					WalkRandom( iChanceToNotMove, iChanceToDir, iSteps );
				}
			}
			else
			{
				for( int i=0; i<iSteps; i++ )
				{
					if( m_Mobile.RangeHome != 0 )
					{
						int iCurrDist = (int)m_Mobile.GetDistanceToSqrt( m_Mobile.Home );

						if( iCurrDist < m_Mobile.RangeHome * 2 / 3 )
						{
							WalkRandom( iChanceToNotMove, iChanceToDir, 1 );
						}
						else if( iCurrDist > m_Mobile.RangeHome )
						{
							DoMove( m_Mobile.GetDirectionTo( m_Mobile.Home ) );
						}
						else
						{
							if( Utility.Random( 10 ) > 5 )
							{
								DoMove( m_Mobile.GetDirectionTo( m_Mobile.Home ) );
							}
							else
							{
								WalkRandom( iChanceToNotMove, iChanceToDir, 1 );
							}
						}
					}
					else
					{
						if( m_Mobile.Location != m_Mobile.Home )
						{
							DoMove( m_Mobile.GetDirectionTo( m_Mobile.Home ) );
						}
					}
				}
			}
		}

		public virtual bool CheckFlee()
		{
			if( m_Mobile.CheckFlee() )
			{
				Mobile combatant = m_Mobile.Combatant;

				if( combatant == null )
				{
					WalkRandom( 1, 2, 1 );
				}
				else
				{
					Direction d = combatant.GetDirectionTo( m_Mobile );

					d = (Direction)((int)d + Utility.RandomMinMax( -1, +1 ));

					m_Mobile.Direction = d;
					m_Mobile.Move( d );
				}

				return true;
			}

			return false;
		}

		protected PathFollower m_Path;

		public virtual void OnTeleported()
		{
			if( m_Path != null )
			{
				m_Mobile.DebugSay( "Teleported; repathing" );
				m_Path.ForceRepath();
			}
		}

		public virtual bool MoveTo( Mobile m, bool run, int range )
		{
			if( m_Mobile.Deleted || m_Mobile.DisallowAllMoves || m == null || m.Deleted )
				return false;

			if( m_Mobile.InRange( m, range ) )
			{
				m_Path = null;
				return true;
			}

			if( m_Path != null && m_Path.Goal == m )
			{
				if( m_Path.Follow( run, 1 ) )
				{
					m_Path = null;
					return true;
				}
			}
			else if( !DoMove( m_Mobile.GetDirectionTo( m ), true ) )
			{
				m_Path = new PathFollower( m_Mobile, m );
				m_Path.Mover = new MoveMethod( DoMoveImpl );

				if( m_Path.Follow( run, 1 ) )
				{
					m_Path = null;
					return true;
				}
			}
			else
			{
				m_Path = null;
				return true;
			}

			return false;
		}

		public static void MarchingOrder( Mobile m )
		{
			if ( m is PlayerMobile && m.Alive )
			{
				int cycle = 5;
				foreach ( Mobile friend in m.GetMobilesInRange( 12 ) )
				{
					if ( friend is BaseCreature && ((BaseCreature)friend).Controlled && ((BaseCreature)friend).ControlMaster == m )
					{
						if ( cycle > 9 ){ cycle = 5; } cycle++;
						friend.FollowersMax = cycle;
					}
				}
			}
		}

		/*
		 *  Walk at range distance from mobile
		 * 
		 *	iSteps : Number of steps
		 *	bRun   : Do we run
		 *	iWantDistMin : The minimum distance we want to be
		 *  iWantDistMax : The maximum distance we want to be
		 * 
		 */
		public virtual bool WalkMobileRange( Mobile m, int iSteps, bool bRun, int iWantDistMin, int iWantDistMax )
		{
			int foll = 1;
			if ( MySettings.S_FriendsAvoidHeels && m_Mobile.Controlled && !(m_Mobile.Backpack is StrongBackpack) && m_Mobile.ControlMaster == m )
			{
				if ( m_Mobile.FollowersMax < 6 ){ m_Mobile.FollowersMax = Utility.RandomMinMax( 6, 9 ); }
				iWantDistMin = m_Mobile.FollowersMax-5;
				iWantDistMax = iWantDistMin+1;
				foll = m_Mobile.FollowersMax-5;
			}

			if( m_Mobile.Deleted || m_Mobile.DisallowAllMoves )
				return false;

			if( m != null )
			{
				for( int i=0; i<iSteps; i++ )
				{
					// Get the curent distance
					int iCurrDist = (int)m_Mobile.GetDistanceToSqrt( m );

					if( iCurrDist < iWantDistMin || iCurrDist > iWantDistMax )
					{
						bool needCloser = (iCurrDist > iWantDistMax);
						bool needFurther = !needCloser;

						if( needCloser && m_Path != null && m_Path.Goal == m )
						{
							if( m_Path.Follow( bRun, foll ) )
								m_Path = null;
						}
						else
						{
							Direction dirTo;

							if( iCurrDist > iWantDistMax )
								dirTo = m_Mobile.GetDirectionTo( m );
							else
								dirTo = m.GetDirectionTo( m_Mobile );

							// Add the run flag
							if( bRun )
								dirTo = dirTo | Direction.Running;

							if( !DoMove( dirTo, true ) && needCloser )
							{
								m_Path = new PathFollower( m_Mobile, m );
								m_Path.Mover = new MoveMethod( DoMoveImpl );

								if( m_Path.Follow( bRun, foll ) )
									m_Path = null;
							}
							else
							{
								m_Path = null;
							}
						}
					}
					else
					{
						return true;
					}
				}

				// Get the curent distance
				int iNewDist = (int)m_Mobile.GetDistanceToSqrt( m );

				if( iNewDist >= iWantDistMin && iNewDist <= iWantDistMax )
					return true;
				else
					return false;
			}

			return false;
		}

		/*
		 * Here we check to acquire a target from our surronding
		 * 
		 *  iRange : The range
		 *  acqType : A type of acquire we want (closest, strongest, etc)
		 *  bPlayerOnly : Don't bother with other creatures or NPCs, want a player
		 * 
		 */
		public virtual bool AcquireFocusMob( int iRange, FightMode acqType, bool bPlayerOnly, bool bFacFriend, bool bFacFoe )
		{
			if( m_Mobile.Deleted )
				return false;

			if( m_Mobile.BardProvoked )
			{
				if( m_Mobile.BardTarget == null || m_Mobile.BardTarget.Deleted )
				{
					m_Mobile.FocusMob = null;
					return false;
				}
				else
				{
					m_Mobile.FocusMob = m_Mobile.BardTarget;
					return (m_Mobile.FocusMob != null);
				}
			}
			else if( m_Mobile.Controlled )
			{
				if( m_Mobile.ControlTarget == null || m_Mobile.ControlTarget.Deleted || m_Mobile.ControlTarget.Hidden || !m_Mobile.ControlTarget.Alive || m_Mobile.ControlTarget.IsDeadBondedPet || !m_Mobile.InRange( m_Mobile.ControlTarget, m_Mobile.RangePerception * 2 ) )
				{
					if ( m_Mobile.ControlTarget != null && m_Mobile.ControlTarget != m_Mobile.ControlMaster )
						m_Mobile.ControlTarget = null;

					m_Mobile.FocusMob = null;
					return false;
				}
				else
				{
					m_Mobile.FocusMob = m_Mobile.ControlTarget;
					return (m_Mobile.FocusMob != null);
				}
			}

			if( m_Mobile.ConstantFocus != null )
			{
				m_Mobile.DebugSay( "Acquired my constant focus" );
				m_Mobile.FocusMob = m_Mobile.ConstantFocus;
				return true;
			}

			if( acqType == FightMode.None )
			{
				m_Mobile.FocusMob = null;
				return false;
			}

			if ( acqType == FightMode.Aggressor && m_Mobile.Aggressors.Count == 0 && m_Mobile.Aggressed.Count == 0 )
			{
				m_Mobile.FocusMob = null;
				return false;
			}

			if( m_Mobile.NextReacquireTime > DateTime.Now )
			{
				m_Mobile.FocusMob = null;
				return false;
			}

			m_Mobile.NextReacquireTime = DateTime.Now + m_Mobile.ReacquireDelay;

			m_Mobile.DebugSay( "Acquiring..." );

			Map map = m_Mobile.Map;

			if( map != null )
			{
				Mobile newFocusMob = null;
				double val = double.MinValue;
				double theirVal;

				IPooledEnumerable eable = map.GetMobilesInRange( m_Mobile.Location, iRange );

				List<Mobile> mobiles = new List<Mobile>();
				foreach (Mobile m in eable)
				{
					mobiles.Add(m);
				}
				eable.Free();
				foreach (Mobile m in mobiles)
				{
					if ( m.Deleted || m.Blessed )
						continue;

					// Let's not target ourselves...
					if ( m == m_Mobile )
						continue;

					// Dead targets are invalid.
					if ( !m.Alive || m.IsDeadBondedPet )
						continue;

					// Staff members cannot be targeted.
					if ( m.AccessLevel > AccessLevel.Player )
						continue;

					// Does it have to be a player?
					if ( bPlayerOnly && !m.Player )
						continue;

					// Can't acquire a target we can't see.
					if ( !m_Mobile.CanSee( m ) )
						continue;

					if ( m_Mobile.Summoned && m_Mobile.SummonMaster != null )
					{
						// If this is a summon, it can't target its controller.
						if ( ( m == m_Mobile.SummonMaster ) || ( m is PlayerMobile ) )
							continue;

						if (m is BaseCreature)
						{
							BaseCreature c = (BaseCreature)m;
							if ( ( c.SummonMaster != null ) || ( c.ControlMaster != null ) )
							continue;
						}

						// It also must abide by harmful spell rules.
						//if ( !Server.Spells.SpellHelper.ValidIndirectTarget( m_Mobile.SummonMaster, m ) )
						//	continue;

						// Animated creatures cannot attack players directly.
						if ( m is PlayerMobile && m_Mobile.IsAnimatedDead )
							continue;
					}

					if ( bFacFriend && !m_Mobile.IsFriend( m ) )
						continue;
					
					if( acqType == FightMode.Aggressor || acqType == FightMode.Evil || acqType == FightMode.CharmMonster || acqType == FightMode.CharmAnimal )
					{
						// Only acquire this mobile if it attacked us, or if it's evil.
						bool bValid = false;

						for ( int a = 0; !bValid && a < m_Mobile.Aggressors.Count; ++a )
						{
							bValid = ( m_Mobile.Aggressors[a].Attacker == m );
							if ( bValid && acqType == FightMode.Evil ){ m.CriminalAction( false ); }
						}

						for ( int a = 0; !bValid && a < m_Mobile.Aggressed.Count; ++a )	
						{
							bValid = ( m_Mobile.Aggressed[a].Defender == m );
							if ( bValid && acqType == FightMode.Evil ){ m.CriminalAction( false ); }
						}

						if ( acqType == FightMode.Evil && !bValid )
						{
							if( m is BaseCreature && ((BaseCreature)m).Controlled && ((BaseCreature)m).ControlMaster != null )
							{
								bValid = ( ((BaseCreature)m).ControlMaster.Karma < -2499 || ((BaseCreature)m).ControlMaster.Criminal || ((BaseCreature)m).ControlMaster.Kills > 0 );
								if ( bValid ){ ((BaseCreature)m).ControlMaster.CriminalAction( false ); }
							}
							else if ( m is PlayerMobile )
							{
								bValid = ( m.Karma < -2499 || m.Criminal || m.Kills > 0 );
								if ( bValid ){ m.CriminalAction( false ); }
							}
							else if ( MySettings.S_Purple && m is BaseCreature && ((BaseCreature)m).ControlMaster == null )
							{
								// Suppress this behavior for champ spawn mobs
								bool isChampSpawnCombat = ((BaseCreature)m).IsEphemeral && m_Mobile.IsEphemeral && m.Region.IsPartOf( typeof( Server.Engines.CannedEvil.ChampionSpawnRegion ) );
								bValid = m.Karma < -2499 && !isChampSpawnCombat;
							}
						}
						else if ( ( acqType == FightMode.CharmMonster || acqType == FightMode.CharmAnimal ) && !bValid )
						{
							bValid = ( m.Karma < 0 && m is BaseCreature && !((BaseCreature)m).Controlled );
						}

						if ( !bValid )
							continue;
					} else {

						if ( bFacFoe && !m_Mobile.IsEnemy( m ) )
							continue;

						if ( bFacFoe && !bFacFriend && !m_Mobile.CanBeHarmful( m, false ) )
							continue;
					}

					theirVal = m_Mobile.GetFightModeRanking( m, acqType, bPlayerOnly );

					if( theirVal > val && m_Mobile.InLOS( m ) )
					{
						newFocusMob = m;
						val = theirVal;
					}
				}

				m_Mobile.FocusMob = newFocusMob;
			}

			return (m_Mobile.FocusMob != null);
		}

		public virtual void Searching()
		{
			if( m_Mobile.Deleted || m_Mobile.Map == null )
				return;

			m_Mobile.DebugSay( "Checking for hidden players" );

			double srcSkill = m_Mobile.Skills[SkillName.Searching].Value;

			if( srcSkill <= 0 )
				return;

			foreach( Mobile trg in m_Mobile.GetMobilesInRange( m_Mobile.RangePerception ) )
			{
				if( trg != m_Mobile && trg.Player && trg.Alive && trg.Hidden && trg.AccessLevel == AccessLevel.Player && m_Mobile.InLOS( trg ) )
				{
					m_Mobile.DebugSay( "Trying to detect {0}", trg.Name );

					double trgHiding = trg.Skills[SkillName.Hiding].Value / 2.9;
					double trgStealth = trg.Skills[SkillName.Stealth].Value / 1.8;

					double chance = srcSkill / 1.2 - Math.Min( trgHiding, trgStealth );

					if( chance < srcSkill / 10 )
						chance = srcSkill / 10;

					chance /= 100;

					if( chance > Utility.RandomDouble() )
					{
						trg.RevealingAction();
						trg.SendLocalizedMessage( 500814 ); // You have been revealed!
					}
				}
			}
		}

		public virtual void Deactivate()
		{
			if( m_Mobile.PlayerRangeSensitive )
			{
				m_Timer.Stop();

				SpawnEntry se = m_Mobile.Spawner as SpawnEntry;

				if( se != null && se.ReturnOnDeactivate && !m_Mobile.Controlled )
				{
					if( se.HomeLocation == Point3D.Zero )
					{
						if( !m_Mobile.Region.AcceptsSpawnsFrom( se.Region ) )
						{
							Timer.DelayCall( TimeSpan.Zero, new TimerCallback( ReturnToHome ) );
						}
					}
					else if( !m_Mobile.InRange( se.HomeLocation, se.HomeRange ) )
					{
						Timer.DelayCall( TimeSpan.Zero, new TimerCallback( ReturnToHome ) );
					}
				}
			}
		}

		private void ReturnToHome()
		{
			SpawnEntry se = m_Mobile.Spawner as SpawnEntry;

			if( se != null )
			{
				Point3D loc = se.RandomSpawnLocation( 16, !m_Mobile.CantWalk, m_Mobile.CanSwim );

				if( loc != Point3D.Zero )
				{
					m_Mobile.MoveToWorld( loc, se.Region.Map );
					return;
				}
			}
		}

		public virtual void Activate()
		{
			if( !m_Timer.Running )
			{
				m_Timer.Delay = TimeSpan.Zero;
				m_Timer.Start();
			}
		}

		/*
		 *  The mobile changed it speed, we must ajust the timer
		 */
		public virtual void OnCurrentSpeedChanged()
		{
			m_Timer.Stop();
			m_Timer.Delay = TimeSpan.FromSeconds( Utility.RandomDouble() );
			m_Timer.Interval = TimeSpan.FromSeconds( Math.Max( 0.0, m_Mobile.CurrentSpeed ) );
			m_Timer.Start();
		}

		private DateTime m_NextSearching;

		public virtual bool CanSearching { get { return m_Mobile.Skills[SkillName.Searching].Value > 0; } }

		/*
		 *  The Timer object
		 */
		private class AITimer : Timer
		{
			private BaseAI m_Owner;

			public AITimer( BaseAI owner )
				: base( TimeSpan.FromSeconds( Utility.RandomDouble() ), TimeSpan.FromSeconds( Math.Max( 0.0, owner.m_Mobile.CurrentSpeed ) ) )
			{
				m_Owner = owner;

				m_Owner.m_NextSearching = DateTime.Now;

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if( m_Owner.m_Mobile.Deleted )
				{
					Stop();
					return;
				}
				else if( m_Owner.m_Mobile.Map == null || m_Owner.m_Mobile.Map == Map.Internal )
				{
					return;
				}
				else if( m_Owner.m_Mobile.PlayerRangeSensitive )//have to check this in the timer....
				{
					Sector sect = m_Owner.m_Mobile.Map.GetSector( m_Owner.m_Mobile );
					if( !sect.Active )
					{
						m_Owner.Deactivate();
						return;
					}
				}

				m_Owner.m_Mobile.OnThink();

				if( m_Owner.m_Mobile.Deleted )
				{
					Stop();
					return;
				}
				else if( m_Owner.m_Mobile.Map == null || m_Owner.m_Mobile.Map == Map.Internal )
				{
					return;
				}

				if( m_Owner.m_Mobile.BardPacified )
				{
					m_Owner.DoBardPacified();
				}
				else if( m_Owner.m_Mobile.BardProvoked )
				{
					m_Owner.DoBardProvoked();
				}
				else
				{
					if( !m_Owner.m_Mobile.Controlled )
					{
						if( !m_Owner.Think() )
						{
							Stop();
							return;
						}
					}
					else
					{
						if( !m_Owner.Obey() )
						{
							Stop();
							return;
						}
					}
				}

				if( m_Owner.CanSearching && DateTime.Now > m_Owner.m_NextSearching )
				{
					m_Owner.Searching();

					// Not exactly OSI style, approximation.
					int delay = (15000 / m_Owner.m_Mobile.Int);

					if( delay > 60 )
						delay = 60;

					int min = delay * (9 / 10); // 13s at 1000 int, 33s at 400 int, 54s at <250 int
					int max = delay * (10 / 9); // 16s at 1000 int, 41s at 400 int, 66s at <250 int

					m_Owner.m_NextSearching = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( min, max ) );
				}
			}
		}
	}
}

namespace Server.Mobiles
{
	public class HealerAI : BaseAI
	{
		private static NeedDelegate m_Cure = new NeedDelegate( NeedCure );
		private static NeedDelegate m_GHeal = new NeedDelegate( NeedGHeal );
		private static NeedDelegate m_LHeal = new NeedDelegate( NeedLHeal );
		private static NeedDelegate[] m_ACure = new NeedDelegate[] { m_Cure };
		private static NeedDelegate[] m_AGHeal = new NeedDelegate[] { m_GHeal };
		private static NeedDelegate[] m_ALHeal = new NeedDelegate[] { m_LHeal };
		private static NeedDelegate[] m_All = new NeedDelegate[] { m_Cure, m_GHeal, m_LHeal };

		public HealerAI( BaseCreature m ) : base( m )
		{
		}

		public override bool Think()
		{
			if ( m_Mobile.Deleted )
				return false;

			Target targ = m_Mobile.Target;

			if ( targ != null )
			{
				if ( targ is CureSpell.InternalTarget )
				{
					ProcessTarget( targ, m_ACure );
				}
				else if ( targ is GreaterHealSpell.InternalTarget )
				{
					ProcessTarget( targ, m_AGHeal );
				}
				else if ( targ is HealSpell.InternalTarget )
				{
					ProcessTarget( targ, m_ALHeal );
				}
				else
				{
					targ.Cancel( m_Mobile, TargetCancelType.Canceled );
				}
			}
			else
			{
				Mobile toHelp = Find( m_All );

				if ( toHelp != null )
				{
					if ( NeedCure( toHelp ) )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "{0} needs a cure", toHelp.Name );

						if ( !(new CureSpell( m_Mobile, null )).Cast() )
							new CureSpell( m_Mobile, null ).Cast();
					}
					else if ( NeedGHeal( toHelp ) )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "{0} needs a greater heal", toHelp.Name );

						if ( !(new GreaterHealSpell( m_Mobile, null )).Cast() )
							new HealSpell( m_Mobile, null ).Cast();
					}
					else if ( NeedLHeal( toHelp ) )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "{0} needs a lesser heal", toHelp.Name );

						new HealSpell( m_Mobile, null ).Cast();
					}
				}
				else
				{
					if ( AcquireFocusMob( m_Mobile.RangePerception, FightMode.Weakest, false, true, false ) )
					{
						WalkMobileRange( m_Mobile.FocusMob, 1, false, 4, 7 );
					}
					else
					{
						WalkRandomInHome( 3, 2, 1 );
					}
				}
			}

			return true;
		}

		private delegate bool NeedDelegate( Mobile m );

		private void ProcessTarget( Target targ, NeedDelegate[] func )
		{
			Mobile toHelp = Find( func );

			if ( toHelp != null )
			{
				if ( targ.Range != -1 && !m_Mobile.InRange( toHelp, targ.Range ) )
				{
					DoMove( m_Mobile.GetDirectionTo( toHelp ) | Direction.Running );
				}
				else
				{
					targ.Invoke( m_Mobile, toHelp );
				}
			}
			else
			{
				targ.Cancel( m_Mobile, TargetCancelType.Canceled );
			}
		}

		private Mobile Find( params NeedDelegate[] funcs )
		{
			if ( m_Mobile.Deleted )
				return null;

			Map map = m_Mobile.Map;

			if ( map != null )
			{
				double prio = 0.0;
				Mobile found = null;

				foreach ( Mobile m in m_Mobile.GetMobilesInRange( m_Mobile.RangePerception ) )
				{
					if ( !m_Mobile.CanSee( m ) || !(m is BaseCreature) || ((BaseCreature)m).Team != m_Mobile.Team )
						continue;

					for ( int i = 0; i < funcs.Length; ++i )
					{
						if ( funcs[i]( m ) )
						{
							double val = -m_Mobile.GetDistanceToSqrt( m );

							if ( found == null || val > prio )
							{
								prio = val;
								found = m;
							}

							break;
						}
					}
				}

				return found;
			}

			return null;
		}

		private static bool NeedCure( Mobile m )
		{
			return m.Poisoned;
		}

		private static bool NeedGHeal( Mobile m )
		{
			return m.Hits < m.HitsMax - 40;
		}

		private static bool NeedLHeal( Mobile m )
		{
			return m.Hits < m.HitsMax - 10;
		}
	}
}

namespace Server.Mobiles
{
	public class MageAI : BaseAI
	{
		private DateTime m_NextCastTime;
		private DateTime m_NextHealTime;
		private DateTime m_NextAnimateTime = DateTime.Now;
		private double m_AnimateDelay = 5.0;
		private double m_AnimateFinish = 2.0;

		public MageAI( BaseCreature m ) : base( m )
		{
		}

		public override bool Think()
		{
			if( m_Mobile.Deleted )
				return false;

			if( ProcessTarget() )
				return true;
			else
				return base.Think();
		}

		public virtual bool SmartAI
		{
			get { return ( m_Mobile is BaseVendor ); }
		}

		private const double HealChance = 0.10; // 10% chance to heal at gm magery
		private const double TeleportChance = 0.05; // 5% chance to teleport at gm magery

		public virtual double ScaleByMagery( double v )
		{
			return m_Mobile.Skills[ SkillName.Magery ].Value * v * 0.01;
		}

		public override bool DoActionWander()
		{
			if( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if( m_Mobile.Debug )
					m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
				m_NextCastTime = DateTime.Now;
			}
			else if( SmartAI && m_Mobile.Mana < m_Mobile.ManaMax )
			{
				m_Mobile.DebugSay( "I am going to meditate" );

				m_Mobile.UseSkill( SkillName.Meditation );
			}
			else
			{
				m_Mobile.DebugSay( "I am wandering" );

				m_Mobile.Warmode = false;

				base.DoActionWander();

				if( ( Utility.RandomDouble() < .05 ) )
				{
					Spell spell = CheckCastHealingSpell();

					if( spell != null )
					{
						SlayerEntry wizard = SlayerGroup.GetEntryByName( SlayerName.WizardSlayer );
						if( DateTime.Now > m_NextAnimateTime && wizard.Slays(m_Mobile) && !m_Mobile.Mounted )
						{
							m_Mobile.PlaySound( m_Mobile.GetAngerSound());
							m_Mobile.Animate( 12, 5, 1, true, false, 0 );
							m_NextAnimateTime = DateTime.Now + TimeSpan.FromSeconds( m_AnimateDelay );
							NextMove = DateTime.Now + TimeSpan.FromSeconds( m_AnimateFinish );
						}
						spell.Cast();
					}
				}
			}

			return true;
		}

		private Spell CheckCastHealingSpell()
		{
			// If I'm poisoned, always attempt to cure.
			if( m_Mobile.Poisoned )
				return new CureSpell( m_Mobile, null );

			// Summoned creatures never heal themselves.
			if( m_Mobile.Summoned )
				return null;

			if( m_Mobile.Controlled && !(m_Mobile is HenchmanMonster) && !(m_Mobile is HenchmanArcher) && !(m_Mobile is HenchmanWizard) && !(m_Mobile is HenchmanFighter) )
			{
				if( DateTime.Now < m_NextHealTime )
					return null;
			}

			if( !SmartAI )
			{
				if( ScaleByMagery( HealChance ) < Utility.RandomDouble() )
					return null;
			}
			else
			{
				if( Utility.Random( 0, 4 + ( m_Mobile.Hits == 0 ? m_Mobile.HitsMax : ( m_Mobile.HitsMax / m_Mobile.Hits ) ) ) < 3 )
					return null;
			}

			Spell spell = null;

			if( m_Mobile.Hits < ( m_Mobile.HitsMax - 50 ) )
			{
				spell = new GreaterHealSpell( m_Mobile, null );

				if( spell == null )
					spell = new HealSpell( m_Mobile, null );
			}
			else if( m_Mobile.Hits < ( m_Mobile.HitsMax - 10 ) )
				spell = new HealSpell( m_Mobile, null );

			double delay;

			if( m_Mobile.Int >= 500 )
				delay = Utility.RandomMinMax( 7, 10 );
			else
				delay = Math.Sqrt( 600 - m_Mobile.Int );

			m_NextHealTime = DateTime.Now + TimeSpan.FromSeconds( delay );

			return spell;
		}

		public void RunTo( Mobile m )
		{
			if( !SmartAI )
			{
				if( !MoveTo( m, true, m_Mobile.RangeFight ) )
					OnFailedMove();

				return;
			}

			if( m.Paralyzed || m.Frozen )
			{
				if( m_Mobile.InRange( m, 1 ) )
					RunFrom( m );
				else if( !m_Mobile.InRange( m, m_Mobile.RangeFight > 2 ? m_Mobile.RangeFight : 2 ) && !MoveTo( m, true, 1 ) )
					OnFailedMove();
			}
			else
			{
				if( !m_Mobile.InRange( m, m_Mobile.RangeFight ) )
				{
					if( !MoveTo( m, true, 1 ) )
						OnFailedMove();
				}
				else if( m_Mobile.InRange( m, m_Mobile.RangeFight - 1 ) )
				{
					RunFrom( m );
				}
			}
		}

		public void RunFrom( Mobile m )
		{
			Run( ( m_Mobile.GetDirectionTo( m ) - 4 ) & Direction.Mask );
		}

		public void OnFailedMove()
		{
			if( !m_Mobile.DisallowAllMoves && !Server.Mobiles.BasePirate.IsSailor( m_Mobile ) && ( SmartAI ? Utility.Random( 4 ) == 0 : ScaleByMagery( TeleportChance ) > Utility.RandomDouble() ) )
			{
				if( m_Mobile.Target != null )
					m_Mobile.Target.Cancel( m_Mobile, TargetCancelType.Canceled );

				new TeleportSpell( m_Mobile, null ).Cast();

				m_Mobile.DebugSay( "I am stuck, I'm going to try teleporting away" );
			}
			else if( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if( m_Mobile.Debug )
					m_Mobile.DebugSay( "My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				m_Mobile.DebugSay( "I am stuck" );
			}
		}

		public void Run( Direction d )
		{
			if( ( m_Mobile.Spell != null && m_Mobile.Spell.IsCasting ) || m_Mobile.Paralyzed || m_Mobile.Frozen || m_Mobile.DisallowAllMoves )
				return;

			m_Mobile.Direction = d | Direction.Running;

			if( !DoMove( m_Mobile.Direction, true ) )
				OnFailedMove();
		}

		public virtual bool CanCastNecro()
		{
			return ( Utility.RandomBool() && ( m_Mobile is BaseCreature ) && ( (BaseCreature)m_Mobile ).IsNecromancer );
		}

		public virtual bool CanCastNecroBias( int bias )
		{
			return ( CanCastNecro() && Utility.Random( bias ) == 0 );
		}

		public virtual Spell GetRandomDamage()
		{
			return ( CanCastNecroBias( 2 ) ) ? GetRandomDamageNecroSpell() : GetRandomDamageSpell();
		}

		public virtual Spell GetRandomDamageNecroSpell()
		{
			if ( Utility.RandomMinMax(1,5) != 1 )
			{
				return new AttackSpells( m_Mobile, null );
			}
			else
			{
				int possibles = 3;

				if( myNecro >= 100 )
				{
					possibles = 5;
				}
				switch( Utility.Random( possibles ) )
				{
					default: m_Mobile.DebugSay( "Vengeful Spirit" ); return new VengefulSpiritSpell( m_Mobile, null );

					case 0: m_Mobile.DebugSay( "Pain Spike" ); return new PainSpikeSpell( m_Mobile, null );
					case 1: m_Mobile.DebugSay( "Poison Strike" ); return new PoisonStrikeSpell( m_Mobile, null );
					case 2: m_Mobile.DebugSay( "Strangle" ); return new StrangleSpell( m_Mobile, null );
					case 3: m_Mobile.DebugSay( "Wither" ); return new WitherSpell( m_Mobile, null );
				}
			}
		}

		public virtual Spell GetRandomDamageSpell()
		{
			if ( Utility.RandomMinMax(1,10) != 1 )
			{
				return new AttackSpells( m_Mobile, null );
			}
			else
			{
				int maxCircle = (int)( ( myMagery + 20.0 ) / ( 100.0 / 7.0 ) );

				if( maxCircle < 1 )
					maxCircle = 1;

				switch( Utility.Random( maxCircle * 2 ) )
				{
					case 0: return new AttackSpells( m_Mobile, null );
					case 1: return new AttackSpells( m_Mobile, null ); //return new MagicArrowSpell( m_Mobile, null );
					case 2: return new AttackSpells( m_Mobile, null );
					case 3: return new AttackSpells( m_Mobile, null ); //return new HarmSpell( m_Mobile, null );
					case 4: return new AttackSpells( m_Mobile, null );
					case 5: return new AttackSpells( m_Mobile, null ); //return new FireballSpell( m_Mobile, null );
					case 6: return new AttackSpells( m_Mobile, null );
					case 7: return new AttackSpells( m_Mobile, null ); //return new LightningSpell( m_Mobile, null );
					case 8: return new AttackSpells( m_Mobile, null );
					case 9: return new MindBlastSpell( m_Mobile, null );
					case 10: return new AttackSpells( m_Mobile, null ); //return new EnergyBoltSpell( m_Mobile, null );
					case 11: return new AttackSpells( m_Mobile, null ); //return new ExplosionSpell( m_Mobile, null );
					default: return new AttackSpells( m_Mobile, null ); //return new FlameStrikeSpell( m_Mobile, null );
				}
			}
		}

		public virtual Spell GetRandomCurse()
		{
			return ( CanCastNecro() ) ?  GetRandomNecroCurseSpell() :  GetRandomCurseSpell();
		}

		public virtual Spell GetRandomNecroCurseSpell()
		{
			switch( Utility.Random( 4 ) )
			{
				default:
				case 0: m_Mobile.DebugSay( "Blood Oath" ); return new BloodOathSpell( m_Mobile, null );
				case 1: m_Mobile.DebugSay( "Corpse Skin" ); return new CorpseSkinSpell( m_Mobile, null );
				case 2: m_Mobile.DebugSay( "Evil Omen" ); return new EvilOmenSpell( m_Mobile, null );
				case 3: m_Mobile.DebugSay( "Mind Rot" ); return new MindRotSpell( m_Mobile, null );
			}
		}

		public virtual Spell GetRandomCurseSpell()
		{
			if( Utility.Random( 4 ) == 3 )
			{
				if( myMagery >= 40.0 )
				{
					return new CurseSpell( m_Mobile, null );
				}
			}

			switch( Utility.Random( 3 ) )
			{
				default:
				case 0: return new WeakenSpell( m_Mobile, null );
				case 1: return new ClumsySpell( m_Mobile, null );
				case 2: return new FeeblemindSpell( m_Mobile, null );
			}
		}

		public virtual Spell GetRandomManaDrainSpell()
		{
			if( Utility.RandomBool() )
			{
				if( myMagery >= 80.0 )
					return new ManaVampireSpell( m_Mobile, null );
			}

			return new ManaDrainSpell( m_Mobile, null );
		}

		public virtual Spell DoDispel( Mobile toDispel )
		{
			if( !SmartAI )
			{
				if( m_Mobile.DispelChecks( toDispel, true ) )
					return new DispelSpell( m_Mobile, null );

				return ChooseSpell( toDispel );
			}

			Spell spell = CheckCastHealingSpell();

			if( spell == null )
			{
				if( !m_Mobile.DisallowAllMoves && Utility.Random( (int)m_Mobile.GetDistanceToSqrt( toDispel ) ) == 0 && !Server.Mobiles.BasePirate.IsSailor( m_Mobile ) )
					spell = new TeleportSpell( m_Mobile, null );
				else if( Utility.Random( 3 ) == 0 && !m_Mobile.InRange( toDispel, 3 ) && !toDispel.Paralyzed && !toDispel.Frozen )
					spell = new ParalyzeSpell( m_Mobile, null );
				else
					spell = ChooseSpell( toDispel );
			}

			return spell;
		}

		public virtual double myNecro { get { return m_Mobile.Skills[ SkillName.Magery ].Value; } }

		public virtual double myMagery { get { return m_Mobile.Skills[ SkillName.Magery ].Value; } }

		public virtual double mySpiritualism { get { return m_Mobile.Skills[ SkillName.Spiritualism ].Value; } }

		public virtual Spell ChooseSpell( Mobile c )
		{
			Spell spell = null;

			if( !SmartAI )
			{
				spell = CheckCastHealingSpell();

				if( spell != null )
					return spell;

				switch( Utility.Random( 16 ) )
				{
					case 0:
					case 1:	// Poison them
						{
							//m_Mobile.DebugSay( "Attempting to poison" );

							if( !c.Poisoned )
								spell = new PoisonSpell( m_Mobile, null );

							break;
						}
					case 2:	// Bless ourselves.
						{
							//m_Mobile.DebugSay( "Blessing myself" );

							spell = new BlessSpell( m_Mobile, null );
							break;
						}
					case 3:
					case 4: // Curse them.
						{
							//m_Mobile.DebugSay( "Attempting to curse" );

							spell = GetRandomCurse();
							break;
						}
					case 5:	// Paralyze them.
						{
							//m_Mobile.DebugSay( "Attempting to paralyze" );

							if( m_Mobile.Skills[ SkillName.Magery ].Value > 50.0 )
								spell = new ParalyzeSpell( m_Mobile, null );

							break;
						}
					case 6: // Drain mana
						{
							//m_Mobile.DebugSay( "Attempting to drain mana" );

							spell = GetRandomManaDrainSpell();
							break;
						}
					case 7:
						{
							//m_Mobile.DebugSay( "Attempting to Invis" );

							if( spell == null )
							{
								spell = new InvisibilitySpell( m_Mobile, null );
							}

							break;
						}

					default: // Damage them.
						{
							//m_Mobile.DebugSay( "Just doing damage" );

							spell = GetRandomDamage();
							break;
						}
				}

				return spell;
			}

			spell = CheckCastHealingSpell();

			if( spell != null )
				return spell;

			switch( Utility.RandomList( 0, 1, 1, 1, 2, 2 ) )
			{
				default:
				case 0: // Poison them
					{
						if( !c.Poisoned )
							spell = new PoisonSpell( m_Mobile, null );

						break;
					}
				case 1: // Deal some damage
					{
						spell = GetRandomDamageSpell();

						break;
					}
				case 2: // Set up a combo
					{
						if( m_Mobile.Mana < 40 && m_Mobile.Mana > 15 )
						{
							if( c.Paralyzed && !c.Poisoned )
							{
								m_Mobile.DebugSay( "I am going to meditate" );

								m_Mobile.UseSkill( SkillName.Meditation );
							}
							else if( !c.Poisoned )
							{
								spell = new ParalyzeSpell( m_Mobile, null );
							}
						}
						else if( m_Mobile.Mana > 60 )
						{
							if( Utility.Random( 2 ) == 0 && !c.Paralyzed && !c.Frozen && !c.Poisoned )
							{
								m_Combo = 0;
								spell = new ParalyzeSpell( m_Mobile, null );
							}
							else
							{
								m_Combo = 1;
								spell = new ExplosionSpell( m_Mobile, null );
							}
						}

						break;
					}
			}

			return spell;
		}

		protected int m_Combo = -1;

		public virtual Spell DoCombo( Mobile c )
		{
			Spell spell = null;

			if( m_Combo == 0 )
			{
				spell = new ExplosionSpell( m_Mobile, null );
				++m_Combo; // Move to next spell
			}
			else if( m_Combo == 1 )
			{
				spell = new WeakenSpell( m_Mobile, null );
				++m_Combo; // Move to next spell
			}
			else if( m_Combo == 2 )
			{
				if( !c.Poisoned )
					spell = new PoisonSpell( m_Mobile, null );

				++m_Combo; // Move to next spell
			}

			if( m_Combo == 3 && spell == null )
			{
				switch( Utility.Random( 3 ) )
				{
					default:
					case 0:
						{
							if( c.Int < c.Dex )
								spell = new FeeblemindSpell( m_Mobile, null );
							else
								spell = new ClumsySpell( m_Mobile, null );

							++m_Combo; // Move to next spell

							break;
						}
					case 1:
						{
							spell = new EnergyBoltSpell( m_Mobile, null );
							m_Combo = -1; // Reset combo state
							break;
						}
					case 2:
						{
							spell = new FlameStrikeSpell( m_Mobile, null );
							m_Combo = -1; // Reset combo state
							break;
						}
				}
			}
			else if( m_Combo == 4 && spell == null )
			{
				spell = new MindBlastSpell( m_Mobile, null );
				m_Combo = -1;
			}

			return spell;
		}

		private TimeSpan GetDelay()
		{
			double del = ScaleByMagery( 3.0 );
			double min = 6.0 - ( del * 0.75 );
			double max = 6.0 - ( del * 1.25 );

			return TimeSpan.FromSeconds( min + ( ( max - min ) * Utility.RandomDouble() ) );
		}

		public override bool DoActionCombat()
		{
			Mobile c = m_Mobile.Combatant;
			m_Mobile.Warmode = true;

			if( c == null || c.Deleted || !c.Alive || c.IsDeadBondedPet || !m_Mobile.CanSee( c ) || !m_Mobile.CanBeHarmful( c, false ) || c.Map != m_Mobile.Map )
			{
				// Our combatant is deleted, dead, hidden, or we cannot hurt them
				// Try to find another combatant

				if( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					if( m_Mobile.Debug )
						m_Mobile.DebugSay( "Something happened to my combatant, so I am going to fight {0}", m_Mobile.FocusMob.Name );

					m_Mobile.Combatant = c = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else
				{
					m_Mobile.DebugSay( "Something happened to my combatant, and nothing is around. I am on guard." );
					Action = ActionType.Guard;
					return true;
				}
			}

			if( !m_Mobile.InLOS( c ) )
			{
				m_Mobile.DebugSay( "I can't see my target" );

				if( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.DebugSay( "Nobody else is around" );
					m_Mobile.Combatant = c = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
			}

			if( SmartAI && !m_Mobile.StunReady && m_Mobile.Skills[ SkillName.FistFighting ].Value >= 80.0 && m_Mobile.Skills[ SkillName.Anatomy ].Value >= 80.0 )
				EventSink.InvokeStunRequest( new StunRequestEventArgs( m_Mobile ) );

			if( !m_Mobile.InRange( c, m_Mobile.RangePerception ) )
			{
				// They are somewhat far away, can we find something else?

				if( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.Combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else if( !m_Mobile.InRange( c, m_Mobile.RangePerception * 3 ) )
				{
					m_Mobile.Combatant = null;
				}

				c = m_Mobile.Combatant;

				if( c == null )
				{
					m_Mobile.DebugSay( "My combatant has fled, so I am on guard" );
					Action = ActionType.Guard;

					return true;
				}
			}

			if( !m_Mobile.Controlled && !m_Mobile.Summoned && !m_Mobile.IsParagon && !(m_Mobile is HenchmanArcher) && !(m_Mobile is HenchmanMonster) && !(m_Mobile is HenchmanWizard) && !(m_Mobile is HenchmanFighter) )
			{
				if( m_Mobile.Hits < m_Mobile.HitsMax * 20 / 100 )
				{
					// We are low on health, should we flee?

					bool flee = false;

					if( m_Mobile.Hits < c.Hits )
					{
						// We are more hurt than them

						int diff = c.Hits - m_Mobile.Hits;

						flee = ( Utility.Random( 0, 100 ) > ( 10 + diff ) ); // (10 + diff)% chance to flee
					}
					else
					{
						flee = Utility.Random( 0, 100 ) > 10; // 10% chance to flee
					}

					if( flee )
					{
						if( m_Mobile.Debug )
							m_Mobile.DebugSay( "I am going to flee from {0}", c.Name );

						Action = ActionType.Flee;
						return true;
					}
				}
			}

			if( m_Mobile.Spell == null && DateTime.Now > m_NextCastTime && m_Mobile.InRange( c, Core.ML ? 10 : 12 ) )
			{
				// We are ready to cast a spell

				Spell spell = null;
				Mobile toDispel = FindDispelTarget( true );

				if( m_Mobile.Poisoned ) // Top cast priority is cure
				{
					m_Mobile.DebugSay( "I am going to cure myself" );

					spell = new CureSpell( m_Mobile, null );
				}
				else if( toDispel != null ) // Something dispellable is attacking us
				{
					m_Mobile.DebugSay( "I am going to dispel {0}", toDispel );

					spell = DoDispel( toDispel );
				}
				else if( SmartAI && m_Combo != -1 ) // We are doing a spell combo
				{
					spell = DoCombo( c );
				}
				else
				{
					spell = ChooseSpell( c );
				}

				// Now we have a spell picked
				// Move first before casting

				if( SmartAI && toDispel != null )
				{
					if( m_Mobile.InRange( toDispel, 10 ) )
						RunFrom( toDispel );
					else if( !m_Mobile.InRange( toDispel, Core.ML ? 10 : 12 ) )
						RunTo( toDispel );
				}
				else
				{
					RunTo( c );
				}

				if( spell != null )
				{
					SlayerEntry wizard = SlayerGroup.GetEntryByName( SlayerName.WizardSlayer );
					if( DateTime.Now > m_NextAnimateTime && wizard.Slays(m_Mobile) && !m_Mobile.Mounted )
					{
						m_Mobile.PlaySound( m_Mobile.GetAngerSound());
						m_Mobile.Animate( 12, 5, 1, true, false, 0 );
						m_NextAnimateTime = DateTime.Now + TimeSpan.FromSeconds( m_AnimateDelay );
						NextMove = DateTime.Now + TimeSpan.FromSeconds( m_AnimateFinish );
					}
					spell.Cast();
				}

				TimeSpan delay;

				if( SmartAI || ( spell is DispelSpell ) )
					delay = TimeSpan.FromSeconds( m_Mobile.ActiveSpeed );
				else
					delay = GetDelay();

				m_NextCastTime = DateTime.Now;
			}
			else if( m_Mobile.Spell == null || !m_Mobile.Spell.IsCasting )
			{
				RunTo( c );
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				if( !m_Mobile.Controlled && !(m_Mobile is HenchmanArcher) && !(m_Mobile is HenchmanMonster) && !(m_Mobile is HenchmanWizard) && !(m_Mobile is HenchmanFighter) )
				{
					ProcessTarget();

					Spell spell = CheckCastHealingSpell();

					if( spell != null )
					{
						SlayerEntry wizard = SlayerGroup.GetEntryByName( SlayerName.WizardSlayer );
						if( DateTime.Now > m_NextAnimateTime && wizard.Slays(m_Mobile) && !m_Mobile.Mounted )
						{
							m_Mobile.PlaySound( m_Mobile.GetAngerSound());
							m_Mobile.Animate( 12, 5, 1, true, false, 0 );
							m_NextAnimateTime = DateTime.Now + TimeSpan.FromSeconds( m_AnimateDelay );
							NextMove = DateTime.Now + TimeSpan.FromSeconds( m_AnimateFinish );
						}
						spell.Cast();
					}
				}

				base.DoActionGuard();
			}

			return true;
		}

		public override bool DoActionFlee()
		{
			Mobile c = m_Mobile.Combatant;

			if( ( m_Mobile.Mana > 20 || m_Mobile.Mana == m_Mobile.ManaMax ) && m_Mobile.Hits > ( m_Mobile.HitsMax / 2 ) )
			{
				m_Mobile.DebugSay( "I am stronger now, my guard is up" );
				Action = ActionType.Guard;
			}
			else if( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if( m_Mobile.Debug )
					m_Mobile.DebugSay( "I am scared of {0}", m_Mobile.FocusMob.Name );

				RunFrom( m_Mobile.FocusMob );
				m_Mobile.FocusMob = null;

				if( m_Mobile.Poisoned && Utility.Random( 0, 5 ) == 0 )
					new CureSpell( m_Mobile, null ).Cast();
			}
			else
			{
				m_Mobile.DebugSay( "Area seems clear, but my guard is up" );

				Action = ActionType.Guard;
				m_Mobile.Warmode = true;
			}

			return true;
		}

		public Mobile FindDispelTarget( bool activeOnly )
		{
			if( m_Mobile.Deleted || m_Mobile.Int < 95 || CanDispel( m_Mobile ) || m_Mobile.AutoDispel )
				return null;

			if( activeOnly )
			{
				List<AggressorInfo> aggressed = m_Mobile.Aggressed;
				List<AggressorInfo> aggressors = m_Mobile.Aggressors;

				Mobile active = null;
				double activePrio = 0.0;

				Mobile comb = m_Mobile.Combatant;

				if( comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && m_Mobile.InRange( comb, Core.ML ? 10 : 12 ) && CanDispel( comb ) )
				{
					active = comb;
					activePrio = m_Mobile.GetDistanceToSqrt( comb );

					if( activePrio <= 2 )
						return active;
				}

				for( int i = 0; i < aggressed.Count; ++i )
				{
					AggressorInfo info = aggressed[ i ];
					Mobile m = info.Defender;

					if( m != comb && m.Combatant == m_Mobile && m_Mobile.InRange( m, Core.ML ? 10 : 12 ) && CanDispel( m ) )
					{
						double prio = m_Mobile.GetDistanceToSqrt( m );

						if( active == null || prio < activePrio )
						{
							active = m;
							activePrio = prio;

							if( activePrio <= 2 )
								return active;
						}
					}
				}

				for( int i = 0; i < aggressors.Count; ++i )
				{
					AggressorInfo info = aggressors[ i ];
					Mobile m = info.Attacker;

					if( m != comb && m.Combatant == m_Mobile && m_Mobile.InRange( m, Core.ML ? 10 : 12 ) && CanDispel( m ) )
					{
						double prio = m_Mobile.GetDistanceToSqrt( m );

						if( active == null || prio < activePrio )
						{
							active = m;
							activePrio = prio;

							if( activePrio <= 2 )
								return active;
						}
					}
				}

				return active;
			}
			else
			{
				Map map = m_Mobile.Map;

				if( map != null )
				{
					Mobile active = null, inactive = null;
					double actPrio = 0.0, inactPrio = 0.0;

					Mobile comb = m_Mobile.Combatant;

					if( comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && CanDispel( comb ) )
					{
						active = inactive = comb;
						actPrio = inactPrio = m_Mobile.GetDistanceToSqrt( comb );
					}

					foreach( Mobile m in m_Mobile.GetMobilesInRange( Core.ML ? 10 : 12 ) )
					{
						if( m != m_Mobile && CanDispel( m ) )
						{
							double prio = m_Mobile.GetDistanceToSqrt( m );

							if( !activeOnly && ( inactive == null || prio < inactPrio ) )
							{
								inactive = m;
								inactPrio = prio;
							}

							if( ( m_Mobile.Combatant == m || m.Combatant == m_Mobile ) && ( active == null || prio < actPrio ) )
							{
								active = m;
								actPrio = prio;
							}
						}
					}

					return active != null ? active : inactive;
				}
			}

			return null;
		}

		public bool CanDispel( Mobile m )
		{
			return ( m is BaseCreature && ( (BaseCreature)m ).Summoned && m_Mobile.CanBeHarmful( m, false ) && !( (BaseCreature)m ).IsAnimatedDead );
		}

		private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				 0, -1,
				 0,  1,
				 1, -1,
				 1,  0,
				 1,  1,

				-2, -2,
				-2, -1,
				-2,  0,
				-2,  1,
				-2,  2,
				-1, -2,
				-1,  2,
				 0, -2,
				 0,  2,
				 1, -2,
				 1,  2,
				 2, -2,
				 2, -1,
				 2,  0,
				 2,  1,
				 2,  2
			};

		private bool ProcessTarget()
		{
			Target targ = m_Mobile.Target;

			if( targ == null )
				return false;

			bool isDispel = ( targ is DispelSpell.InternalTarget );
			bool isParalyze = ( targ is ParalyzeSpell.InternalTarget );
			bool isTeleport = ( targ is TeleportSpell.InternalTarget );
			bool isInvisible = ( targ is InvisibilitySpell.InternalTarget );
			bool teleportAway = false;

			Mobile toTarget;

			if( isInvisible )
			{
				toTarget = m_Mobile;
			}
			else if( isDispel )
			{
				toTarget = FindDispelTarget( false );

				if( !SmartAI && toTarget != null )
					RunTo( toTarget );
				else if( toTarget != null && m_Mobile.InRange( toTarget, 10 ) )
					RunFrom( toTarget );
			}
			else if( SmartAI && ( isParalyze || isTeleport ) )
			{
				toTarget = FindDispelTarget( true );

				if( toTarget == null )
				{
					toTarget = m_Mobile.Combatant;

					if( toTarget != null )
						RunTo( toTarget );
				}
				else if( m_Mobile.InRange( toTarget, 10 ) )
				{
					RunFrom( toTarget );
					teleportAway = true;
				}
				else
				{
					teleportAway = true;
				}
			}
			else
			{
				toTarget = m_Mobile.Combatant;

				if( toTarget != null )
					RunTo( toTarget );
			}

			if( ( targ.Flags & TargetFlags.Harmful ) != 0 && toTarget != null )
			{
				if( ( targ.Range == -1 || m_Mobile.InRange( toTarget, targ.Range ) ) && m_Mobile.CanSee( toTarget ) && m_Mobile.InLOS( toTarget ) )
				{
					targ.Invoke( m_Mobile, toTarget );
				}
				else if( isDispel )
				{
					targ.Cancel( m_Mobile, TargetCancelType.Canceled );
				}
			}
			else if( ( targ.Flags & TargetFlags.Beneficial ) != 0 )
			{
				targ.Invoke( m_Mobile, m_Mobile );
			}
			else if( isTeleport && toTarget != null )
			{
				Map map = m_Mobile.Map;

				if( map == null )
				{
					targ.Cancel( m_Mobile, TargetCancelType.Canceled );
					return true;
				}

				int px, py;

				if( teleportAway )
				{
					int rx = m_Mobile.X - toTarget.X;
					int ry = m_Mobile.Y - toTarget.Y;

					double d = m_Mobile.GetDistanceToSqrt( toTarget );

					px = toTarget.X + (int)( rx * ( 10 / d ) );
					py = toTarget.Y + (int)( ry * ( 10 / d ) );
				}
				else
				{
					px = toTarget.X;
					py = toTarget.Y;
				}

				for( int i = 0; i < m_Offsets.Length; i += 2 )
				{
					int x = m_Offsets[ i ], y = m_Offsets[ i + 1 ];

					Point3D p = new Point3D( px + x, py + y, 0 );

					LandTarget lt = new LandTarget( p, map );

					if( ( targ.Range == -1 || m_Mobile.InRange( p, targ.Range ) ) && m_Mobile.InLOS( lt ) && map.CanSpawnMobile( px + x, py + y, lt.Z ) && !SpellHelper.CheckMulti( p, map ) )
					{
						targ.Invoke( m_Mobile, lt );
						return true;
					}
				}

				int teleRange = targ.Range;

				if( teleRange < 0 )
					teleRange = Core.ML ? 11 : 12;

				for( int i = 0; i < 10; ++i )
				{
					Point3D randomPoint = new Point3D( m_Mobile.X - teleRange + Utility.Random( teleRange * 2 + 1 ), m_Mobile.Y - teleRange + Utility.Random( teleRange * 2 + 1 ), 0 );

					LandTarget lt = new LandTarget( randomPoint, map );

					if( m_Mobile.InLOS( lt ) && map.CanSpawnMobile( lt.X, lt.Y, lt.Z ) && !SpellHelper.CheckMulti( randomPoint, map ) )
					{
						targ.Invoke( m_Mobile, new LandTarget( randomPoint, map ) );
						return true;
					}
				}

				targ.Cancel( m_Mobile, TargetCancelType.Canceled );
			}
			else
			{
				targ.Cancel( m_Mobile, TargetCancelType.Canceled );
			}

			return true;
		}
	}
}

namespace Server.Mobiles
{
	public class MeleeAI : BaseAI
	{
		public MeleeAI(BaseCreature m) : base (m)
		{
		}

		public override bool DoActionWander()
		{
			m_Mobile.DebugSay( "I have no combatant" );

			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionWander();
			}

			return true;
		}

		public override bool DoActionCombat()
		{
			Mobile combatant = m_Mobile.Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map || !combatant.Alive || combatant.IsDeadBondedPet )
			{
				m_Mobile.DebugSay( "My combatant is gone, so my guard is up" );

				Action = ActionType.Guard;

				return true;
			}

			if ( !m_Mobile.InRange( combatant, m_Mobile.RangePerception ) )
			{
				// They are somewhat far away, can we find something else?

				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.Combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else if ( !m_Mobile.InRange( combatant, m_Mobile.RangePerception * 3 ) )
				{
					m_Mobile.Combatant = null;
				}

				combatant = m_Mobile.Combatant;

				if ( combatant == null )
				{
					m_Mobile.DebugSay( "My combatant has fled, so I am on guard" );
					Action = ActionType.Guard;

					return true;
				}
			}

			/*if ( !m_Mobile.InLOS( combatant ) )
			{
				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.Combatant = combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
			}*/

			if ( MoveTo( combatant, true, m_Mobile.RangeFight ) )
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo( combatant );
			}
			else if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;

				return true;
			}
			else if ( m_Mobile.GetDistanceToSqrt( combatant ) > m_Mobile.RangePerception + 1 )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I cannot find {0}, so my guard is up", combatant.Name );

				Action = ActionType.Guard;

				return true;
			}
			else
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I should be closer to {0}", combatant.Name );
			}

			if ( !m_Mobile.Controlled && !m_Mobile.Summoned && !m_Mobile.IsParagon && !(m_Mobile is FrankenFighter) && !(m_Mobile is Robot) && !(m_Mobile is GolemFighter) && !(m_Mobile is HenchmanMonster) && !(m_Mobile is HenchmanArcher) && !(m_Mobile is HenchmanWizard) && !(m_Mobile is HenchmanFighter) )
			{
				if ( m_Mobile.Hits < m_Mobile.HitsMax * 20/100 )
				{
					// We are low on health, should we flee?

					bool flee = false;

					if ( m_Mobile.Hits < combatant.Hits )
					{
						// We are more hurt than them

						int diff = combatant.Hits - m_Mobile.Hits;

						flee = ( Utility.Random( 0, 100 ) < (10 + diff) ); // (10 + diff)% chance to flee
					}
					else
					{
						flee = Utility.Random( 0, 100 ) < 10; // 10% chance to flee
					}

					if ( flee )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "I am going to flee from {0}", combatant.Name );

						Action = ActionType.Flee;
					}
				}
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if ( AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionGuard();
			}

			return true;
		}

		public override bool DoActionFlee()
		{
			if ( m_Mobile.Hits > m_Mobile.HitsMax/2 )
			{
				m_Mobile.DebugSay( "I am stronger now, so I will continue fighting" );
				Action = ActionType.Combat;
			}
			else
			{
				m_Mobile.FocusMob = m_Mobile.Combatant;
				base.DoActionFlee();
			}

			return true;
		}
	}
}

namespace Server.Mobiles
{
	public class ThiefAI : BaseAI
	{
		public ThiefAI(BaseCreature m) : base (m)
		{
		}

		private Item m_toDisarm;
		public override bool DoActionWander()
		{
			m_Mobile.DebugSay( "I have no combatant" );

			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionWander();
			}

			return true;
		}

		public override bool DoActionCombat()
		{
			Mobile combatant = m_Mobile.Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map )
			{
				m_Mobile.DebugSay( "My combatant is gone, so my guard is up" );

				Action = ActionType.Guard;

				return true;
			}

			if ( WalkMobileRange( combatant, 1, true, m_Mobile.RangeFight, m_Mobile.RangeFight ) )
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo( combatant );
				if ( m_toDisarm == null )
					m_toDisarm = combatant.FindItemOnLayer( Layer.OneHanded );

				if ( m_toDisarm == null )
					m_toDisarm = combatant.FindItemOnLayer( Layer.TwoHanded );

				if ( m_toDisarm != null && m_toDisarm.IsChildOf( m_Mobile.Backpack ) )
				{
					m_toDisarm = combatant.FindItemOnLayer( Layer.OneHanded );
					if ( m_toDisarm == null )
						m_toDisarm = combatant.FindItemOnLayer( Layer.TwoHanded );
				}
				if ( !m_Mobile.DisarmReady && m_Mobile.Skills[SkillName.FistFighting].Value >= 80.0 && m_Mobile.Skills[SkillName.ArmsLore].Value >= 80.0 && m_toDisarm != null )
					EventSink.InvokeDisarmRequest( new DisarmRequestEventArgs( m_Mobile ) );

				if ( m_toDisarm != null && m_toDisarm.IsChildOf( combatant.Backpack ) && m_Mobile.NextSkillTime <= DateTime.Now && (m_toDisarm.LootType != LootType.Blessed && m_toDisarm.LootType != LootType.Newbied) )
				{
					m_Mobile.DebugSay( "Trying to steal from combatant." );
					m_Mobile.UseSkill( SkillName.Stealing );
					if ( m_Mobile.Target != null )
						m_Mobile.Target.Invoke( m_Mobile, m_toDisarm );
				}
				else if ( m_toDisarm == null && m_Mobile.NextSkillTime <= DateTime.Now )
				{
					Container cpack = combatant.Backpack;

					if ( cpack != null )
					{
						Item steal1 = cpack.FindItemByType( typeof ( Bandage ) );
						if ( steal1 != null ) 
						{
							m_Mobile.DebugSay( "Trying to steal from combatant." );
							m_Mobile.UseSkill( SkillName.Stealing );
							if ( m_Mobile.Target != null )
								m_Mobile.Target.Invoke( m_Mobile, steal1 );
						}
						Item steal2 = cpack.FindItemByType( typeof ( Nightshade ) );
						if ( steal2 != null ) 
						{
							m_Mobile.DebugSay( "Trying to steal from combatant." );
							m_Mobile.UseSkill( SkillName.Stealing );
							if ( m_Mobile.Target != null )
								m_Mobile.Target.Invoke( m_Mobile, steal2 );
						}
						Item steal3 = cpack.FindItemByType( typeof ( BlackPearl ) );
						if ( steal3 != null ) 
						{
							m_Mobile.DebugSay( "Trying to steal from combatant." );
							m_Mobile.UseSkill( SkillName.Stealing );
							if ( m_Mobile.Target != null )
								m_Mobile.Target.Invoke( m_Mobile, steal3 );
						}

						Item steal4 = cpack.FindItemByType( typeof ( MandrakeRoot ) );
						if ( steal4 != null ) 
						{
							m_Mobile.DebugSay( "Trying to steal from combatant." );
							m_Mobile.UseSkill( SkillName.Stealing );
							if ( m_Mobile.Target != null )
								m_Mobile.Target.Invoke( m_Mobile, steal4 );
						}

						Item steal5 = cpack.FindItemByType( typeof ( Spellbook ) );
						if ( steal5 != null ) 
						{
							m_Mobile.DebugSay( "Trying to steal from combatant." );
							m_Mobile.UseSkill( SkillName.Stealing );
							if ( m_Mobile.Target != null )
								m_Mobile.Target.Invoke( m_Mobile, steal5 );
						}

						Item steal6 = cpack.FindItemByType( typeof ( Runebook ) );
						if ( steal6 != null ) 
						{
							m_Mobile.DebugSay( "Trying to steal from combatant." );
							m_Mobile.UseSkill( SkillName.Stealing );
							if ( m_Mobile.Target != null )
								m_Mobile.Target.Invoke( m_Mobile, steal6 );
						}

						Item steal7 = cpack.FindItemByType( typeof ( BasePotion ) );
						if ( steal7 != null ) 
						{
							m_Mobile.DebugSay( "Trying to steal from combatant." );
							m_Mobile.UseSkill( SkillName.Stealing );
							if ( m_Mobile.Target != null )
								m_Mobile.Target.Invoke( m_Mobile, steal7 );
						}

						Item steal8 = cpack.FindItemByType( typeof ( SpellScroll ) );
						if ( steal8 != null ) 
						{
							m_Mobile.DebugSay( "Trying to steal from combatant." );
							m_Mobile.UseSkill( SkillName.Stealing );
							if ( m_Mobile.Target != null )
								m_Mobile.Target.Invoke( m_Mobile, steal8 );
						}

						Item steal9 = cpack.FindItemByType( typeof ( MagicalWand ) );
						if ( steal9 != null ) 
						{
							m_Mobile.DebugSay( "Trying to steal from combatant." );
							m_Mobile.UseSkill( SkillName.Stealing );
							if ( m_Mobile.Target != null )
								m_Mobile.Target.Invoke( m_Mobile, steal9 );
						}

						Item steal10 = cpack.FindItemByType( typeof ( Gold ) );
						if ( steal10 != null ) 
						{
							m_Mobile.DebugSay( "Trying to steal from combatant." );
							m_Mobile.UseSkill( SkillName.Stealing );
							if ( m_Mobile.Target != null )
								m_Mobile.Target.Invoke( m_Mobile, steal10 );
						}

						if (	steal1 == null && 
								steal2 == null && 
								steal3 == null && 
								steal4 == null && 
								steal5 == null && 
								steal6 == null && 
								steal7 == null && 
								steal8 == null && 
								steal9 == null && 
								steal10 == null )
						{
							m_Mobile.DebugSay( "I am going to flee from {0}", combatant.Name );

							Action = ActionType.Flee;
						}
					}
				}
			}
			else
			{
				m_Mobile.DebugSay( "I should be closer to {0}", combatant.Name );
			}

			if ( m_Mobile.Hits < m_Mobile.HitsMax * 20/100 && !m_Mobile.IsParagon )
			{
				// We are low on health, should we flee?

				bool flee = false;

				if ( m_Mobile.Hits < combatant.Hits )
				{
					// We are more hurt than them

					int diff = combatant.Hits - m_Mobile.Hits;

					flee = ( Utility.Random( 0, 100 ) > (10 + diff) ); // (10 + diff)% chance to flee
				}
				else
				{
					flee = Utility.Random( 0, 100 ) > 10; // 10% chance to flee
				}

				if ( flee )
				{
					m_Mobile.DebugSay( "I am going to flee from {0}", combatant.Name );

					Action = ActionType.Flee;
				}
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if ( AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionGuard();
			}

			return true;
		}

		public override bool DoActionFlee()
		{
			if ( m_Mobile.Hits > m_Mobile.HitsMax/2 )
			{
				m_Mobile.DebugSay( "I am stronger now, so I will continue fighting" );
				Action = ActionType.Combat;
			}
			else
			{
				m_Mobile.FocusMob = m_Mobile.Combatant;
				base.DoActionFlee();
			}

			return true;
		}
	}
}

namespace Server.Mobiles
{
	public class BerserkAI : BaseAI
	{
		public BerserkAI(BaseCreature m) : base (m)
		{
		}
	
		public override bool DoActionWander()
		{
			m_Mobile.DebugSay( "I have No Combatant" );
			
			if( AcquireFocusMob( m_Mobile.RangePerception, FightMode.Closest, false, true, true) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected " + m_Mobile.FocusMob.Name + " and I will attack" );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}			
			else
			{
				base.DoActionWander();
			}

			return true;			
		}
			
		public override bool DoActionCombat()
		{
			if( m_Mobile.Combatant == null || m_Mobile.Combatant.Deleted )
			{
				m_Mobile.DebugSay("My combatant is deleted");
				Action = ActionType.Guard;
				return true;
			}

			if( WalkMobileRange( m_Mobile.Combatant, 1, true, m_Mobile.RangeFight, m_Mobile.RangeFight ) )
			{
				// Be sure to face the combatant
				m_Mobile.Direction = m_Mobile.GetDirectionTo( m_Mobile.Combatant.Location );
			}
			else
			{
				if( m_Mobile.Combatant != null )
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay("I am still not in range of " + m_Mobile.Combatant.Name);

					if( (int) m_Mobile.GetDistanceToSqrt( m_Mobile.Combatant ) > m_Mobile.RangePerception + 1 )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "I have lost " + m_Mobile.Combatant.Name );

						Action = ActionType.Guard;
						return true;
					}
				}
			}
			
			return true;
		}
		
		public override bool DoActionGuard()
		{
			if ( AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, true, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionGuard();
			}

			return true;
		}
	}
}


namespace Server.Mobiles
{
	public class CitizenAI : BaseAI
	{
		public CitizenAI(BaseCreature m) : base (m)
		{
		}

		public override bool DoActionWander()
		{
			return false;
		}

		public override bool DoActionCombat()
		{
			return false;
		}

		public override bool DoActionBackoff()
		{
			return false;
		}

		public override bool DoActionFlee()
		{
			return false;
		}
	}
}

namespace Server.Mobiles
{
	public class PredatorAI : BaseAI
	{
		public PredatorAI(BaseCreature m) : base (m)
		{
		}

		public override bool DoActionWander()
		{
			if ( m_Mobile.Combatant != null )
			{
				m_Mobile.DebugSay( "I am hurt or being attacked, I kill him" );						
				Action = ActionType.Combat;
			}
			else if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, true, false, true))
			{
				m_Mobile.DebugSay( "There is something near, I go away" );
				Action = ActionType.Backoff;
			}
			else
			{
				base.DoActionWander();
			}

			return true;
		}

		public override bool DoActionCombat()
		{
			Mobile combatant = m_Mobile.Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map )
			{
				m_Mobile.DebugSay( "My combatant is gone, so my guard is up" );
				Action = ActionType.Wander;
				return true;
			}

			if ( WalkMobileRange( combatant, 1, true, m_Mobile.RangeFight, m_Mobile.RangeFight ) )
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo( combatant );
			}
			else
			{
				if ( m_Mobile.GetDistanceToSqrt( combatant ) > m_Mobile.RangePerception + 1 )
				{
					m_Mobile.DebugSay( "I cannot find {0}", combatant.Name );

					Action = ActionType.Wander;
					return true;
				}
				else
				{
					m_Mobile.DebugSay( "I should be closer to {0}", combatant.Name );
				}
			}

			return true;
		}

		public override bool DoActionBackoff()
		{
			if ( m_Mobile.IsHurt() || m_Mobile.Combatant != null )
			{
				Action = ActionType.Combat;
			}
			else
			{
				if (AcquireFocusMob(m_Mobile.RangePerception * 2, FightMode.Closest, true, false , true))
				{
					if ( WalkMobileRange(m_Mobile.FocusMob, 1, false, m_Mobile.RangePerception, m_Mobile.RangePerception * 2) )
					{
						m_Mobile.DebugSay( "Well, here I am safe" );
						Action = ActionType.Wander;
					}					
				}
				else
				{
					m_Mobile.DebugSay( "I have lost my focus, lets relax" );
					Action = ActionType.Wander;
				}
			}

			return true;
		}
	}
}

namespace Server.Mobiles
{
	public class VendorAI : BaseAI
	{
		public VendorAI(BaseCreature m) : base (m)
		{
		}

		public override bool DoActionWander()
		{
			m_Mobile.DebugSay( "I'm fine" );

			if ( m_Mobile.Combatant != null )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "{0} is attacking me", m_Mobile.Combatant.Name );

				m_Mobile.Say( Utility.RandomList( 1005305, 501603 ) );

				Action = ActionType.Flee;
			}
			else
			{
				if ( m_Mobile.FocusMob != null )
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "{0} has talked to me", m_Mobile.FocusMob.Name );

					Action = ActionType.Interact;
				}
				else
				{
					m_Mobile.Warmode = false;

					base.DoActionWander();
				}
			}

			return true;
		}

		public override bool DoActionInteract()
		{
			Mobile customer = m_Mobile.FocusMob;

			if ( m_Mobile.Combatant != null )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "{0} is attacking me", m_Mobile.Combatant.Name );

				m_Mobile.Say( Utility.RandomList( 1005305, 501603 ) );

				Action = ActionType.Flee;
				
				return true;
			}

			if ( customer == null || customer.Deleted || customer.Map != m_Mobile.Map )
			{
				m_Mobile.DebugSay( "My customer have disapeared" );
				m_Mobile.FocusMob = null;

				Action = ActionType.Wander;
			}
			else
			{
				if ( customer.InRange( m_Mobile, m_Mobile.RangeFight ) )
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "I am with {0}", customer.Name );

					m_Mobile.Direction = m_Mobile.GetDirectionTo( customer );
				}
				else
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "{0} is gone", customer.Name );

					m_Mobile.FocusMob = null;

					Action = ActionType.Wander;	
				}
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			m_Mobile.FocusMob = m_Mobile.Combatant;
			return base.DoActionGuard();
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			if ( from.InRange( m_Mobile, 4 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

		// Temporary 
		public override void OnSpeech( SpeechEventArgs e )
		{
			base.OnSpeech( e );
 
			Mobile from = e.Mobile;
 
			if ( m_Mobile is BaseVendor && from.InRange( m_Mobile, Core.AOS ? 2 : 4 ) && !e.Handled )
			{
				if ( e.HasKeyword( 0x14D ) ) // *vendor sell*
				{
					e.Handled = true;

					((BaseVendor)m_Mobile).VendorSell( from );
					m_Mobile.FocusMob = from;
				}
				else if ( e.HasKeyword( 0x3C ) )
				{
					e.Handled = true;

					((BaseVendor)m_Mobile).VendorBuy( from );
					m_Mobile.FocusMob = from;
				}
				else if ( WasNamed( e.Speech ) )
				{
					e.Handled = true;

					if ( e.HasKeyword( 0x177 ) ) // *sell*
						((BaseVendor)m_Mobile).VendorSell( from );
					else if ( e.HasKeyword( 0x171 ) ) // *buy*
						((BaseVendor)m_Mobile).VendorBuy( from );

					m_Mobile.FocusMob = from;
				}
			}
		}
	}
}