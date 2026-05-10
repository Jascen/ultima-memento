using System;
using System.Collections;
using System.Linq;

namespace Server.Items
{
	public enum CraftResourceType
	{
		None,
		Metal,
		Leather,
		Scales,
		Wood,
		Block,
		Skin,
		Special,
		Skeletal,
		Fabric
	}

	public class CraftAttributeInfo
	{
		private int m_WeaponFireDamage;
		private int m_WeaponColdDamage;
		private int m_WeaponPoisonDamage;
		private int m_WeaponEnergyDamage;
		private int m_WeaponChaosDamage;
		private int m_WeaponDirectDamage;
		private int m_WeaponDurability;
		private int m_WeaponLuck;
		private int m_WeaponGoldIncrease;
		private int m_WeaponLowerRequirements;

		private int m_ArmorPhysicalResist;
		private int m_ArmorFireResist;
		private int m_ArmorColdResist;
		private int m_ArmorPoisonResist;
		private int m_ArmorEnergyResist;
		private int m_ArmorDurability;
		private int m_ArmorLuck;
		private int m_ArmorGoldIncrease;
		private int m_ArmorLowerRequirements;

		public int WeaponFireDamage{ get{ return m_WeaponFireDamage; } set{ m_WeaponFireDamage = value; } }
		public int WeaponColdDamage{ get{ return m_WeaponColdDamage; } set{ m_WeaponColdDamage = value; } }
		public int WeaponPoisonDamage{ get{ return m_WeaponPoisonDamage; } set{ m_WeaponPoisonDamage = value; } }
		public int WeaponEnergyDamage{ get{ return m_WeaponEnergyDamage; } set{ m_WeaponEnergyDamage = value; } }
		public int WeaponChaosDamage{ get{ return m_WeaponChaosDamage; } set{ m_WeaponChaosDamage = value; } }
		public int WeaponDirectDamage{ get{ return m_WeaponDirectDamage; } set{ m_WeaponDirectDamage = value; } }
		public int WeaponDurability{ get{ return m_WeaponDurability; } set{ m_WeaponDurability = value; } }
		public int WeaponLuck{ get{ return m_WeaponLuck; } set{ m_WeaponLuck = value; } }
		public int WeaponGoldIncrease{ get{ return m_WeaponGoldIncrease; } set{ m_WeaponGoldIncrease = value; } }
		public int WeaponLowerRequirements{ get{ return m_WeaponLowerRequirements; } set{ m_WeaponLowerRequirements = value; } }

		public int ArmorPhysicalResist{ get{ return m_ArmorPhysicalResist; } set{ m_ArmorPhysicalResist = value; } }
		public int ArmorFireResist{ get{ return m_ArmorFireResist; } set{ m_ArmorFireResist = value; } }
		public int ArmorColdResist{ get{ return m_ArmorColdResist; } set{ m_ArmorColdResist = value; } }
		public int ArmorPoisonResist{ get{ return m_ArmorPoisonResist; } set{ m_ArmorPoisonResist = value; } }
		public int ArmorEnergyResist{ get{ return m_ArmorEnergyResist; } set{ m_ArmorEnergyResist = value; } }
		public int ArmorDurability{ get{ return m_ArmorDurability; } set{ m_ArmorDurability = value; } }
		public int ArmorLuck{ get{ return m_ArmorLuck; } set{ m_ArmorLuck = value; } }
		public int ArmorGoldIncrease{ get{ return m_ArmorGoldIncrease; } set{ m_ArmorGoldIncrease = value; } }
		public int ArmorLowerRequirements{ get{ return m_ArmorLowerRequirements; } set{ m_ArmorLowerRequirements = value; } }

		public static CraftAttributeInfo CraftAttInfo( int armorphy, int armorfir, int armorcld, int armorpsn, int armoregy, object spacer, int weapcold, int weapfire, int weapengy, int weappois, object spacer2, int durable, int lowreq, int luck )
		{
			CraftAttributeInfo var = new CraftAttributeInfo();

			var.ArmorPhysicalResist = 		armorphy;
			var.ArmorColdResist = 			armorcld;
			var.ArmorFireResist = 			armorfir;
			var.ArmorEnergyResist = 		armoregy;
			var.ArmorPoisonResist = 		armorpsn;
			var.ArmorDurability = 			durable;
			var.ArmorLowerRequirements = 	lowreq;
			var.ArmorLuck = 				luck;
			var.WeaponColdDamage = 			weapcold;
			var.WeaponFireDamage = 			weapfire;
			var.WeaponEnergyDamage = 		weapengy;
			var.WeaponPoisonDamage = 		weappois;
			var.WeaponDurability = 			durable;
			var.WeaponLowerRequirements = 	lowreq;
			var.WeaponLuck = 				luck;

			return var;
		}

		public CraftAttributeInfo()
		{
		}

		public static readonly CraftAttributeInfo Blank;
		public static readonly CraftAttributeInfo DullCopper, ShadowIron, Copper, Bronze, Golden, Agapite, Verite, Valorite, Nepturite, Obsidian, Steel, Brass, Mithril, Xormite, Dwarven, Agrinium, Beskar, Carbonite, Cortosis, Durasteel, Durite, Farium, Laminasteel, Neuranium, Phrik, Promethium, Quadranium, Songsteel, Titanium, Trimantium, Xonolite;
		public static readonly CraftAttributeInfo Horned, Barbed, Necrotic, Volcanic, Frozen, Spined, Goliath, Draconic, Hellish, Dinosaur, Alien, Adesote, Biomesh, Cerlin, Durafiber, Flexicris, Hypercloth, Nylar, Nylonite, Polyfiber, Syncloth, Thermoweave;
		public static readonly CraftAttributeInfo RedScales, YellowScales, BlackScales, GreenScales, WhiteScales, BlueScales, DinosaurScales, MetallicScales, BrazenScales, UmberScales, VioletScales, PlatinumScales, CadalyteScales, GornScales, TrandoshanScales, SilurianScales, KraytScales;
		public static readonly CraftAttributeInfo AshTree, CherryTree, EbonyTree, GoldenOakTree, HickoryTree, MahoganyTree, OakTree, PineTree, GhostTree, RosewoodTree, WalnutTree, PetrifiedTree, DriftwoodTree, ElvenTree, BorlTree, CosianTree, GreelTree, JaporTree, KyshyyykTree, LaroonTree, TeejTree, VeshokTree;
		public static readonly CraftAttributeInfo AmethystBlock, EmeraldBlock, GarnetBlock, IceBlock, JadeBlock, MarbleBlock, OnyxBlock, QuartzBlock, RubyBlock, SapphireBlock, SilverBlock, SpinelBlock, StarRubyBlock, TopazBlock, CaddelliteBlock;
		public static readonly CraftAttributeInfo DemonSkin, DragonSkin, NightmareSkin, SnakeSkin, TrollSkin, UnicornSkin, IcySkin, Seaweed, LavaSkin, DeadSkin;
		public static readonly CraftAttributeInfo SpectralSpec, DreadSpec, GhoulishSpec, WyrmSpec, HolySpec, BloodlessSpec, GildedSpec, DemilichSpec, WintrySpec, FireSpec, ColdSpec, PoisSpec, EngySpec, ExodusSpec, TurtleSpec;
		public static readonly CraftAttributeInfo DrowSkeletal, OrcSkeletal, ReptileSkeletal, OgreSkeletal, TrollSkeletal, GargoyleSkeletal, MinotaurSkeletal, LycanSkeletal, SharkSkeletal, ColossalSkeletal, MysticalSkeletal, VampireSkeletal, LichSkeletal, SphinxSkeletal, DevilSkeletal, DracoSkeletal, XenoSkeletal, AndorianSkeletal, CardassianSkeletal, MartianSkeletal, RodianSkeletal, TuskenSkeletal, TwilekSkeletal, XindiSkeletal, ZabrakSkeletal;
		public static readonly CraftAttributeInfo FurryFabric, WoolyFabric, SilkFabric, HauntedFabric, ArcticFabric, PyreFabric, VenomousFabric, MysteriousFabric, VileFabric, DivineFabric, FiendishFabric;

		static CraftAttributeInfo()
		{
			Blank = new CraftAttributeInfo();
			
			DullCopper 		= CraftAttInfo( 	2	,	1	,	1	,	1	,	1	,	null,	0	,	0	,	0	,	0	,	null,	75	,	35	,	0	 ); // 6
			ShadowIron	 	= CraftAttInfo( 	3	,	3	,	3	,	2	,	2	,	null,	0	,	0	,	0	,	0	,	null,	75	,	0	,	0	 ); // 13
			Copper	 	 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	2	,	null,	0	,	0	,	20	,	10	,	null,	25	,	0	,	0	 );
			Bronze	 	 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	0	,	40	,	0	,	0	,	null,	30	,	0	,	0	 );
			Golden	 	 	= CraftAttInfo( 	4	,	3	,	3	,	3	,	3	,	null,	0	,	0	,	0	,	0	,	null,	30	,	0	,	40	 );
			Agapite	 	 	= CraftAttInfo( 	4	,	4	,	3	,	3	,	3	,	null,	30	,	0	,	20	,	0	,	null,	25	,	0	,	0	 );
			Verite	 	 	= CraftAttInfo( 	4	,	4	,	4	,	3	,	3	,	null,	0	,	0	,	20	,	40	,	null,	25	,	0	,	0	 );
			Valorite	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	3	,	null,	20	,	10	,	20	,	10	,	null,	40	,	0	,	0	 );
			Nepturite	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	25	,	0	,	0	,	25	,	null,	40	,	0	,	0	 );
			Obsidian	 	= CraftAttInfo( 	5	,	4	,	4	,	4	,	4	,	null,	0	,	20	,	10	,	0	,	null,	40	,	0	,	0	 );
			Steel	 	 	= CraftAttInfo( 	5	,	5	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	50	,	0	,	0	 );
			Brass	 	 	= CraftAttInfo( 	5	,	5	,	5	,	4	,	4	,	null,	0	,	20	,	20	,	0	,	null,	55	,	0	,	0	 );
			Mithril	 	 	= CraftAttInfo( 	5	,	5	,	5	,	5	,	4	,	null,	0	,	0	,	30	,	0	,	null,	100	,	50	,	100	 );
			Xormite	 	 	= CraftAttInfo( 	5	,	5	,	5	,	5	,	5	,	null,	0	,	0	,	30	,	0	,	null,	0	,	25	,	0	 );
			Dwarven	 	 	= CraftAttInfo( 	6	,	5	,	5	,	5	,	5	,	null,	0	,	0	,	0	,	0	,	null,	100	,	10	,	0	 );
			//
			Agrinium	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	25	,	0	 );
			Beskar	 	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	10	,	0	,	0	,	null,	80	,	25	,	0	 );
			Carbonite	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	25	,	0	 );
			Cortosis	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	25	,	0	,	null,	80	,	25	,	0	 );
			Durasteel	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	25	,	0	 );
			Durite	 	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	25	,	0	 );
			Farium	 	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	35	,	0	,	0	,	null,	80	,	25	,	0	 );
			Laminasteel	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	25	,	null,	80	,	25	,	0	 );
			Neuranium	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	25	,	0	,	0	,	null,	80	,	25	,	0	 );
			Phrik	 	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	35	,	0	,	null,	80	,	25	,	0	 );
			Promethium	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	35	,	null,	80	,	25	,	0	 );
			Quadranium	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	10	,	10	,	10	,	10	,	null,	80	,	25	,	0	 );
			Songsteel	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	25	,	0	 );
			Titanium	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	25	,	0	 );
			Trimantium	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	50	,	0	,	0	,	0	,	null,	80	,	25	,	0	 );
			Xonolite	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	50	,	0	,	0	,	null,	80	,	25	,	0	 );
																																				
			RedScales	 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	0	,	25	,	0	,	0	,	null,	30	,	0	,	0	 );
			YellowScales 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	10	,	10	,	10	,	10	,	null,	30	,	0	,	30	 );
			BlackScales	 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	0	,	0	,	0	,	0	,	null,	30	,	0	,	0	 );
			GreenScales	 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	0	,	0	,	0	,	25	,	null,	30	,	0	,	0	 );
			WhiteScales	 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	25	,	0	,	0	,	0	,	null,	30	,	0	,	0	 );
			BlueScales	 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	15	,	0	,	0	,	15	,	null,	30	,	0	,	0	 );
			DinosaurScales 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	0	,	0	,	0	,	0	,	null,	30	,	0	,	0	 );
			MetallicScales 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	0	,	0	,	0	,	0	,	null,	30	,	0	,	0	 );
			BrazenScales 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	0	,	15	,	15	,	0	,	null,	30	,	0	,	0	 );
			UmberScales	 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	0	,	0	,	35	,	0	,	null,	30	,	0	,	0	 );
			VioletScales 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	0	,	0	,	25	,	0	,	null,	30	,	0	,	0	 );
			PlatinumScales 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	15	,	15	,	15	,	15	,	null,	30	,	0	,	50	 );
			CadalyteScales 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	0	,	0	,	50	,	0	,	null,	200	,	30	,	0	 );
			//
			GornScales	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	25	,	0	,	0	,	null,	100	,	10	,	0	 );
			TrandoshanScales= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	25	,	0	,	0	,	0	,	null,	100	,	10	,	0	 );
			SilurianScales	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	25	,	0	,	null,	100	,	10	,	0	 );
			KraytScales	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	25	,	null,	100	,	0	,	0	 );
																																				
			SpectralSpec	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	50	,	0	,	0	,	0	,	null,	20	,	0	,	0	 );
			DreadSpec	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	90	,	0	,	20	 );
			GhoulishSpec	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	10	,	10	,	10	,	10	,	null,	200	,	0	,	50	 );
			WyrmSpec	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	10	,	10	,	10	,	10	,	null,	200	,	0	,	50	 );
			HolySpec	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	35	,	10	,	35	,	10	,	null,	100	,	0	,	0	 );
			BloodlessSpec	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	70	,	20	,	0	 );
			GildedSpec	 	= CraftAttInfo( 	2	,	2	,	2	,	2	,	2	,	null,	0	,	0	,	0	,	0	,	null,	0	,	0	,	100	 );
			DemilichSpec	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	30	,	null,	200	,	0	,	0	 );
			WintrySpec	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	50	,	0	,	0	,	0	,	null,	70	,	0	,	0	 );
			FireSpec	 	= CraftAttInfo( 	0	,	17	,	0	,	0	,	0	,	null,	0	,	100	,	0	,	0	,	null,	25	,	10	,	0	 );
			ColdSpec	 	= CraftAttInfo( 	0	,	0	,	17	,	0	,	0	,	null,	100	,	0	,	0	,	0	,	null,	25	,	10	,	0	 );
			PoisSpec	 	= CraftAttInfo( 	0	,	0	,	0	,	17	,	0	,	null,	0	,	0	,	0	,	100	,	null,	25	,	10	,	0	 );
			EngySpec	 	= CraftAttInfo( 	0	,	0	,	0	,	0	,	17	,	null,	0	,	0	,	100	,	0	,	null,	25	,	10	,	0	 );
			ExodusSpec	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	0	,	0	,	0	 );
			TurtleSpec	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	120	,	30	,	0	 );
																																				
			Horned	 	 	= CraftAttInfo( 	3	,	2	,	0	,	0	,	0	,	null,	0	,	0	,	0	,	0	,	null,	10	,	0	,	0	 );
			Barbed	 	 	= CraftAttInfo( 	2	,	0	,	0	,	4	,	0	,	null,	0	,	0	,	0	,	70	,	null,	20	,	0	,	0	 );
			Necrotic	 	= CraftAttInfo( 	0	,	0	,	3	,	2	,	2	,	null,	0	,	0	,	0	,	50	,	null,	30	,	0	,	0	 );
			Volcanic	 	= CraftAttInfo( 	3	,	5	,	0	,	0	,	0	,	null,	0	,	50	,	0	,	0	,	null,	40	,	0	,	0	 );
			Frozen	 		= CraftAttInfo( 	0	,	2	,	5	,	0	,	2	,	null,	50	,	0	,	0	,	0	,	null,	50	,	0	,	0	 );
			Spined	 		= CraftAttInfo( 	0	,	4	,	4	,	2	,	0	,	null,	0	,	0	,	0	,	20	,	null,	50	,	0	,	40	 );
			Goliath	 		= CraftAttInfo( 	5	,	0	,	0	,	3	,	3	,	null,	0	,	0	,	25	,	0	,	null,	60	,	0	,	0	 );
			Draconic	 	= CraftAttInfo( 	0	,	5	,	3	,	2	,	2	,	null,	0	,	25	,	0	,	0	,	null,	70	,	0	,	0	 );
			Hellish	 		= CraftAttInfo( 	4	,	5	,	0	,	0	,	4	,	null,	0	,	50	,	0	,	0	,	null,	80	,	0	,	0	 );
			Dinosaur	 	= CraftAttInfo( 	5	,	0	,	5	,	4	,	0	,	null,	0	,	0	,	0	,	0	,	null,	100	,	0	,	0	 );
			Alien	 		= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	0	,	0	,	50	,	0	,	null,	100	,	0	,	0	 );
			//
			Adesote	 		= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	25	,	0	,	null,	80	,	50	,	0	 );
			Biomesh	 		= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	40	,	0	 );
			Cerlin	 		= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	25	,	0	,	0	,	0	,	null,	80	,	60	,	0	 );
			Durafiber	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	40	,	0	 );
			Flexicris	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	25	,	25	,	0	,	null,	80	,	50	,	0	 );
			Hypercloth	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	25	,	null,	80	,	60	,	0	 );
			Nylar	 		= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	50	,	0	,	0	,	null,	80	,	60	,	0	 );
			Nylonite	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	50	,	0	,	null,	80	,	50	,	0	 );
			Polyfiber	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	50	,	null,	80	,	50	,	0	 );
			Syncloth	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	40	,	0	 );
			Thermoweave	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	20	,	20	,	0	,	null,	80	,	50	,	0	 );
																																				
			AshTree	 		= CraftAttInfo( 	2	,	2	,	2	,	1	,	1	,	null,	5	,	5	,	5	,	5	,	null,	10	,	35	,	0	 ); // 8
			CherryTree	 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	2	,	null,	0	,	0	,	20	,	10	,	null,	25	,	0	,	0	 ); // 14
			EbonyTree	 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	20	,	0	,	0	,	0	,	null,	40	,	0	,	0	 );
			GoldenOakTree	= CraftAttInfo( 	4	,	3	,	3	,	3	,	3	,	null,	0	,	0	,	0	,	0	,	null,	20	,	0	,	40	 );
			HickoryTree	 	= CraftAttInfo( 	4	,	4	,	3	,	3	,	3	,	null,	0	,	0	,	0	,	0	,	null,	50	,	0	,	0	 );
			MahoganyTree	= CraftAttInfo( 	4	,	4	,	4	,	3	,	3	,	null,	0	,	0	,	20	,	10	,	null,	55	,	0	,	0	 );
			OakTree	 		= CraftAttInfo( 	4	,	4	,	4	,	4	,	3	,	null,	0	,	40	,	0	,	0	,	null,	55	,	0	,	0	 );
			PineTree	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	30	,	0	,	20	,	0	,	null,	60	,	0	,	0	 );
			GhostTree	 	= CraftAttInfo( 	5	,	4	,	4	,	4	,	4	,	null,	25	,	0	,	25	,	0	,	null,	60	,	0	,	0	 );
			RosewoodTree	= CraftAttInfo( 	5	,	5	,	4	,	4	,	4	,	null,	0	,	0	,	20	,	40	,	null,	60	,	0	,	0	 );
			WalnutTree	 	= CraftAttInfo( 	5	,	5	,	5	,	4	,	4	,	null,	20	,	10	,	20	,	10	,	null,	65	,	0	,	0	 );
			PetrifiedTree	= CraftAttInfo( 	5	,	5	,	5	,	5	,	4	,	null,	0	,	25	,	0	,	0	,	null,	70	,	0	,	0	 );
			DriftwoodTree	= CraftAttInfo( 	5	,	5	,	5	,	5	,	5	,	null,	10	,	10	,	10	,	20	,	null,	80	,	0	,	0	 );
			ElvenTree		= CraftAttInfo( 	6	,	5	,	5	,	5	,	5	,	null,	0	,	0	,	0	,	0	,	null,	100	,	0	,	100	 );
			//
			BorlTree		= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	0	,	0	 );
			CosianTree		= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	0	,	50	 );
			GreelTree		= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	0	,	0	 );
			JaporTree		= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	0	,	0	 );
			KyshyyykTree	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	100	,	0	,	0	 );
			LaroonTree	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	0	,	0	 );
			TeejTree	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	0	,	0	 );
			VeshokTree	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	0	,	null,	80	,	0	,	0	 );
																																				
			FurryFabric	 	= CraftAttInfo( 	1	,	1	,	1	,	1	,	1	,	null,	0	,	0	,	0	,	0	,	null,	0	,	25	,	0	 );
			WoolyFabric	 	= CraftAttInfo( 	2	,	1	,	1	,	1	,	1	,	null,	25	,	0	,	0	,	0	,	null,	0	,	25	,	0	 );
			SilkFabric	 	= CraftAttInfo( 	2	,	2	,	1	,	1	,	1	,	null,	0	,	0	,	0	,	25	,	null,	0	,	25	,	10	 );
			HauntedFabric	= CraftAttInfo( 	2	,	2	,	2	,	1	,	1	,	null,	0	,	0	,	25	,	0	,	null,	0	,	25	,	0	 );
			ArcticFabric	= CraftAttInfo( 	2	,	2	,	2	,	2	,	1	,	null,	50	,	0	,	0	,	0	,	null,	0	,	25	,	0	 );
			PyreFabric	 	= CraftAttInfo( 	2	,	2	,	2	,	2	,	2	,	null,	0	,	50	,	0	,	0	,	null,	0	,	25	,	0	 );
			VenomousFabric	= CraftAttInfo( 	3	,	2	,	2	,	2	,	2	,	null,	0	,	0	,	0	,	50	,	null,	0	,	25	,	0	 );
			MysteriousFabric= CraftAttInfo( 	3	,	3	,	2	,	2	,	2	,	null,	0	,	0	,	50	,	0	,	null,	0	,	25	,	20	 );
			VileFabric	 	= CraftAttInfo( 	3	,	3	,	3	,	2	,	2	,	null,	0	,	0	,	25	,	25	,	null,	0	,	25	,	0	 );
			DivineFabric	= CraftAttInfo( 	3	,	3	,	3	,	3	,	2	,	null,	10	,	10	,	25	,	0	,	null,	50	,	25	,	50	 );
			FiendishFabric	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	10	,	25	,	10	,	0	,	null,	50	,	25	,	0	 );
																																				
			AmethystBlock	= CraftAttInfo( 	3	,	3	,	2	,	2	,	2	,	null,	0	,	0	,	25	,	0	,	null,	100	,	0	,	10	 );
			EmeraldBlock	= CraftAttInfo( 	3	,	3	,	3	,	2	,	2	,	null,	0	,	0	,	0	,	25	,	null,	100	,	0	,	0	 );
			GarnetBlock	 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	2	,	null,	0	,	0	,	10	,	10	,	null,	100	,	0	,	5	 );
			IceBlock	 	= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	50	,	0	,	0	,	0	,	null,	100	,	0	,	0	 );
			JadeBlock	 	= CraftAttInfo( 	4	,	3	,	3	,	3	,	3	,	null,	0	,	10	,	0	,	20	,	null,	100	,	0	,	40	 );
			MarbleBlock	 	= CraftAttInfo( 	4	,	4	,	3	,	3	,	3	,	null,	0	,	0	,	0	,	0	,	null,	150	,	0	,	0	 );
			OnyxBlock	 	= CraftAttInfo( 	4	,	4	,	4	,	3	,	3	,	null,	20	,	20	,	20	,	20	,	null,	100	,	0	,	30	 );
			QuartzBlock	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	3	,	null,	0	,	25	,	25	,	0	,	null,	100	,	0	,	0	 );
			RubyBlock	 	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	60	,	0	,	0	,	null,	100	,	0	,	10	 );
			SapphireBlock	= CraftAttInfo( 	5	,	4	,	4	,	4	,	4	,	null,	0	,	30	,	0	,	0	,	null,	100	,	0	,	0	 );
			SilverBlock	 	= CraftAttInfo( 	5	,	5	,	4	,	4	,	4	,	null,	20	,	20	,	20	,	20	,	null,	100	,	0	,	20	 );
			SpinelBlock	 	= CraftAttInfo( 	5	,	5	,	5	,	4	,	4	,	null,	0	,	0	,	30	,	0	,	null,	100	,	0	,	0	 );
			StarRubyBlock	= CraftAttInfo( 	5	,	5	,	5	,	5	,	4	,	null,	0	,	15	,	15	,	0	,	null,	100	,	0	,	10	 );
			TopazBlock	 	= CraftAttInfo( 	5	,	5	,	5	,	5	,	5	,	null,	0	,	0	,	20	,	20	,	null,	100	,	0	,	0	 );
			CaddelliteBlock	= CraftAttInfo( 	6	,	5	,	5	,	5	,	5	,	null,	0	,	0	,	50	,	0	,	null,	200	,	0	,	0	 );
																																				
			DemonSkin	 	= CraftAttInfo( 	2	,	0	,	3	,	2	,	2	,	null,	0	,	50	,	0	,	0	,	null,	50	,	0	,	20	 );
			DragonSkin	 	= CraftAttInfo( 	2	,	2	,	2	,	2	,	2	,	null,	20	,	20	,	20	,	20	,	null,	50	,	0	,	0	 );
			NightmareSkin	= CraftAttInfo( 	2	,	0	,	3	,	0	,	3	,	null,	0	,	30	,	0	,	0	,	null,	40	,	0	,	0	 );
			SnakeSkin	 	= CraftAttInfo( 	4	,	0	,	0	,	4	,	0	,	null,	0	,	0	,	0	,	50	,	null,	60	,	0	,	0	 );
			TrollSkin	 	= CraftAttInfo( 	4	,	1	,	0	,	3	,	0	,	null,	0	,	0	,	0	,	0	,	null,	60	,	0	,	0	 );
			UnicornSkin	 	= CraftAttInfo( 	2	,	0	,	0	,	2	,	4	,	null,	0	,	0	,	50	,	0	,	null,	30	,	0	,	50	 );
			IcySkin	 		= CraftAttInfo( 	4	,	5	,	0	,	2	,	2	,	null,	50	,	0	,	0	,	0	,	null,	30	,	0	,	0	 );
			Seaweed	 		= CraftAttInfo( 	4	,	2	,	1	,	4	,	2	,	null,	0	,	0	,	0	,	25	,	null,	20	,	0	,	0	 );
			LavaSkin	 	= CraftAttInfo( 	4	,	0	,	5	,	2	,	2	,	null,	0	,	80	,	0	,	0	,	null,	40	,	0	,	0	 );
			DeadSkin	 	= CraftAttInfo( 	2	,	4	,	1	,	4	,	2	,	null,	0	,	0	,	0	,	60	,	null,	40	,	0	,	0	 );
																																				
			DrowSkeletal	= CraftAttInfo( 	2	,	2	,	2	,	2	,	2	,	null,	0	,	0	,	25	,	0	,	null,	5	,	0	,	5	 );
			OrcSkeletal	 	= CraftAttInfo( 	3	,	2	,	2	,	2	,	2	,	null,	0	,	0	,	0	,	0	,	null,	10	,	0	,	0	 );
			ReptileSkeletal	= CraftAttInfo( 	3	,	3	,	2	,	2	,	2	,	null,	0	,	0	,	0	,	25	,	null,	10	,	0	,	0	 );
			OgreSkeletal	= CraftAttInfo( 	3	,	3	,	3	,	2	,	2	,	null,	0	,	0	,	0	,	0	,	null,	20	,	0	,	0	 );
			TrollSkeletal	= CraftAttInfo( 	3	,	3	,	3	,	3	,	2	,	null,	0	,	0	,	0	,	0	,	null,	20	,	0	,	0	 );
			GargoyleSkeletal= CraftAttInfo( 	3	,	3	,	3	,	3	,	3	,	null,	0	,	50	,	0	,	0	,	null,	30	,	0	,	0	 );
			MinotaurSkeletal= CraftAttInfo( 	4	,	3	,	3	,	3	,	3	,	null,	0	,	0	,	0	,	0	,	null,	30	,	0	,	0	 );
			LycanSkeletal	= CraftAttInfo( 	4	,	4	,	3	,	3	,	3	,	null,	0	,	0	,	0	,	0	,	null,	40	,	0	,	0	 );
			SharkSkeletal	= CraftAttInfo( 	4	,	4	,	4	,	3	,	3	,	null,	25	,	0	,	0	,	0	,	null,	40	,	0	,	0	 );
			ColossalSkeletal= CraftAttInfo( 	4	,	4	,	4	,	4	,	3	,	null,	0	,	0	,	0	,	0	,	null,	40	,	0	,	0	 );
			MysticalSkeletal= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	50	,	0	,	null,	50	,	0	,	10	 );
			VampireSkeletal	= CraftAttInfo( 	5	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	25	,	25	,	null,	60	,	0	,	0	 );
			LichSkeletal	= CraftAttInfo( 	5	,	5	,	4	,	4	,	4	,	null,	25	,	0	,	0	,	25	,	null,	60	,	0	,	0	 );
			SphinxSkeletal	= CraftAttInfo( 	5	,	5	,	5	,	4	,	4	,	null,	15	,	15	,	15	,	15	,	null,	70	,	0	,	30	 );
			DevilSkeletal	= CraftAttInfo( 	5	,	5	,	5	,	5	,	4	,	null,	0	,	35	,	15	,	0	,	null,	70	,	0	,	50	 );
			DracoSkeletal	= CraftAttInfo( 	5	,	5	,	5	,	5	,	5	,	null,	20	,	20	,	20	,	20	,	null,	100	,	0	,	0	 );
			XenoSkeletal	= CraftAttInfo( 	6	,	5	,	5	,	5	,	5	,	null,	10	,	10	,	30	,	10	,	null,	80	,	0	,	0	 );
			//
			AndorianSkeletal= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	50	,	0	,	0	,	null,	80	,	35	,	0	 );
			CardassianSkeletal= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	10	,	10	,	10	,	10	,	null,	80	,	30	,	50	 );
			MartianSkeletal	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	0	,	50	,	null,	80	,	40	,	0	 );
			RodianSkeletal	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	0	,	25	,	25	,	null,	80	,	45	,	0	 );
			TuskenSkeletal	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	25	,	25	,	0	,	null,	80	,	30	,	0	 );
			TwilekSkeletal	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	15	,	15	,	15	,	0	,	null,	80	,	35	,	0	 );
			XindiSkeletal	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	10	,	10	,	10	,	10	,	null,	80	,	30	,	0	 );
			ZabrakSkeletal	= CraftAttInfo( 	4	,	4	,	4	,	4	,	4	,	null,	0	,	30	,	30	,	0	,	null,	80	,	40	,	0	 );
		}
	}

	public class CraftResourceInfo
	{
		private int m_Hue;
		private int m_Clr;
		private int m_Dmg;
		private int m_Arm;
		private double m_Gold;
		private double m_Skill;
		private int m_Uses;
		private int m_Weight;
		private int m_Bonus;
		private int m_Xtra;
		private	int m_WepArmor;
		private int m_CraftText;
		private int m_MaterialText;
		private int m_LowCaseText;
		private string m_Name;
		private CraftAttributeInfo m_AttributeInfo;
		private CraftResource m_Resource;
		private Type[] m_ResourceTypes;

		public int Hue{ get{ return m_Hue; } }
		public int Clr{ get{ return m_Clr; } }
		public int Dmg{ get{ return m_Dmg; } }
		public int Arm{ get{ return m_Arm; } }
		public double Gold{ get{ return m_Gold; } }
		public double Skill{ get{ return m_Skill; } }
		public int Uses{ get{ return m_Uses; } }
		public int Weight{ get{ return m_Weight; } }
		public int Bonus{ get{ return m_Bonus; } }
		public int Xtra{ get{ return m_Xtra; } }
		public int WepArmor{ get{ return m_WepArmor; } }
		public int CraftText{ get{ return m_CraftText; } }
		public int MaterialText{ get{ return m_MaterialText; } }
		public int LowCaseText{ get{ return m_LowCaseText; } }
		public string Name{ get{ return m_Name; } }
		public CraftAttributeInfo AttributeInfo{ get{ return m_AttributeInfo; } }
		public CraftResource Resource{ get{ return m_Resource; } }
		public Type[] ResourceTypes{ get{ return m_ResourceTypes; } }

		public CraftResourceInfo( int hue, int clr, int dmg, int ar, double gold, double skill, int uses, int weight, int bonus, int xtra, int weparm, int num1, int num2, int num3, string name, CraftAttributeInfo attributeInfo, CraftResource resource, params Type[] resourceTypes )
		{
			m_Hue = hue;			// Hue for items
			m_Clr = clr;			// Hue for creatures
			m_Dmg = dmg;			// Damage Mod
			m_Arm = ar;				// Armor Mod
			m_Gold = gold;			// Gold Mod
			m_Skill = skill;		// Skill Required
			m_Uses = uses;			// Instrument & Fishing Pole Uses
			m_Weight = weight;		// Ten Foot Pole Weight
			m_Bonus = bonus;		// Ten Foot Pole & Fishing Pole Effectiveness
			m_Xtra = xtra;			// Horse Barding Bonus & Spyglass bonus
			m_WepArmor = weparm;	// Indicates if a Weapon will get Half of the Armor Bonuses
			m_CraftText = num1;		// Text Like: GOLD (100)
			m_MaterialText = num2;	// Text Like: Gold Ingot
			m_LowCaseText = num3;	// Text Like: gold
			m_Name = name;
			m_AttributeInfo = attributeInfo;
			m_Resource = resource;
			m_ResourceTypes = resourceTypes;

			for ( int i = 0; i < resourceTypes.Length; ++i )
				CraftResources.RegisterType( resourceTypes[i], resource );
		}
	}

	public class CraftResources
	{
			//					   Item		NPC					Gold			Skill						Cliloc		Cliloc		Cliloc
			//					   Hue		Clr		Dmg		Arm	Xtra			Need	Use	Wgt	Bon	Xtr	WAr	CRFT 0		Mateial		LowCase		Name	Attribute	Resource	Begin Resource Types

			private static CraftResourceInfo[] m_MetalInfo = new CraftResourceInfo[]																														
			{																														
			new CraftResourceInfo( 0x000,	0x000,	0	,	0	,	1.00	,	0.0		,	0	,	0	,	0	,	0	,	0	,	1044022	,	1044036	,	1053109,	 "Iron",	CraftAttributeInfo.Blank,	CraftResource.Iron,typeof( IronIngot ),	typeof( IronOre ),	typeof( Granite ) ),	
			new CraftResourceInfo( 0x436, 	0x973,	1	,	1	,	1.25	,	65.0	,	10	,	2	,	3	,	1	,	0	,	1044023	,	1074916	,	1053108,	 "Dull Copper",	CraftAttributeInfo.DullCopper,	CraftResource.DullCopper,	typeof( DullCopperIngot ),	typeof( DullCopperOre ),	typeof( DullCopperGranite ) ),
			new CraftResourceInfo( 0x445, 	0x966,	1	,	2	,	1.50	,	70.0	,	20	,	4	,	6	,	2	,	0	,	1044024	,	1074917	,	1053107,	 "Shadow Iron",	CraftAttributeInfo.ShadowIron,	CraftResource.ShadowIron,	typeof( ShadowIronIngot ),	typeof( ShadowIronOre ),	typeof( ShadowIronGranite ) ),
			new CraftResourceInfo( 0x435, 	0x54E,	2	,	3	,	1.75	,	75.0	,	30	,	6	,	9	,	3	,	0	,	1044025	,	1074918	,	1053106,	 "Copper",	CraftAttributeInfo.Copper,	CraftResource.Copper,	typeof( CopperIngot ),	typeof( CopperOre ),	typeof( CopperGranite ) ),
			new CraftResourceInfo( 0x433, 	0x972,	2	,	4	,	2.00	,	80.0	,	40	,	8	,	12	,	4	,	0	,	1044026	,	1074919	,	1053105,	 "Bronze",	CraftAttributeInfo.Bronze,	CraftResource.Bronze,	typeof( BronzeIngot ),	typeof( BronzeOre ),	typeof( BronzeGranite ) ),
			new CraftResourceInfo( 0x43A, 	0x8A5,	2	,	5	,	2.25	,	85.0	,	50	,	10	,	15	,	5	,	0	,	1044027	,	1074920	,	1053104,	 "Gold",	CraftAttributeInfo.Golden,	CraftResource.Gold,typeof( GoldIngot ),	typeof( GoldOre ),	typeof( GoldGranite ) ),	
			new CraftResourceInfo( 0x424, 	0x979,	3	,	6	,	2.50	,	90.0	,	60	,	12	,	18	,	6	,	0	,	1044028	,	1074921	,	1053103,	 "Agapite",	CraftAttributeInfo.Agapite,	CraftResource.Agapite,	typeof( AgapiteIngot ),	typeof( AgapiteOre ),	typeof( AgapiteGranite ) ),
			new CraftResourceInfo( 0x44C, 	0x89F,	3	,	7	,	2.75	,	95.0	,	70	,	14	,	21	,	7	,	0	,	1044029	,	1074922	,	1053102,	 "Verite",	CraftAttributeInfo.Verite,	CraftResource.Verite,	typeof( VeriteIngot ),	typeof( VeriteOre ),	typeof( VeriteGranite ) ),
			new CraftResourceInfo( 0x44B, 	0x8AB,	3	,	8	,	3.00	,	99.0	,	80	,	16	,	24	,	8	,	0	,	1044030	,	1074923	,	1053101,	 "Valorite",	CraftAttributeInfo.Valorite,	CraftResource.Valorite,	typeof( ValoriteIngot ),	typeof( ValoriteOre ),	typeof( ValoriteGranite ) ),
			new CraftResourceInfo( 0x43F, 	0x847,	3	,	9	,	3.10	,	99.0	,	90	,	18	,	27	,	9	,	0	,	1036173	,	1036174	,	1036175,	 "Nepturite",	CraftAttributeInfo.Nepturite,	CraftResource.Nepturite,	typeof( NepturiteIngot ),	typeof( NepturiteOre ),	typeof( NepturiteGranite ) ),
			new CraftResourceInfo( 0x440, 	0x4AE,	3	,	9	,	3.10	,	105.0	,	100	,	20	,	30	,	10	,	0	,	1036162	,	1036164	,	1036165,	 "Obsidian",	CraftAttributeInfo.Obsidian,	CraftResource.Obsidian,	typeof( ObsidianIngot ),	typeof( ObsidianOre ),	typeof( ObsidianGranite ) ),
			new CraftResourceInfo( 0x449, 	0x42A,	4	,	10	,	3.25	,	105.0	,	110	,	22	,	33	,	11	,	0	,	1036144	,	1036145	,	1036146,	 "Steel",	CraftAttributeInfo.Steel,	CraftResource.Steel,	typeof( SteelIngot ) ),		
			new CraftResourceInfo( 0x432, 	0x7B7,	4	,	11	,	3.50	,	105.0	,	120	,	24	,	36	,	12	,	0	,	1036152	,	1036153	,	1036154,	 "Brass",	CraftAttributeInfo.Brass,	CraftResource.Brass,	typeof( BrassIngot ) ),		
			new CraftResourceInfo( 0x43E, 	0x482,	5	,	12	,	3.75	,	115.0	,	130	,	26	,	39	,	13	,	0	,	1036137	,	1036138	,	1036139,	 "Mithril",	CraftAttributeInfo.Mithril,	CraftResource.Mithril,	typeof( MithrilIngot ),	typeof( MithrilOre ),	typeof( MithrilGranite ) ),
			new CraftResourceInfo( 0x44D, 	0x4F6,	5	,	12	,	3.75	,	115.0	,	140	,	27	,	41	,	14	,	1	,	1034437	,	1034438	,	1034439,	 "Xormite",	CraftAttributeInfo.Xormite,	CraftResource.Xormite,	typeof( XormiteIngot ),	typeof( XormiteOre ),	typeof( XormiteGranite ) ),
			new CraftResourceInfo( 0x437, 	0x437,	6	,	14	,	4.50	,	125.0	,	160	,	28	,	42	,	15	,	1	,	1036181	,	1036182	,	1036183,	 "Dwarven",	CraftAttributeInfo.Dwarven,	CraftResource.Dwarven,	typeof( DwarvenIngot ),	typeof( DwarvenOre ),	typeof( DwarvenGranite ) ),
			new CraftResourceInfo( 0x8C1,	0x8C1,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1063982	,	1063983	,	1063981,	 "Agrinium",	CraftAttributeInfo.Agrinium,	CraftResource.Agrinium,	typeof( AgriniumIngot ) ),		
			new CraftResourceInfo( 0x6F8,	0x6F8,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1063986	,	1063987	,	1063985,	 "Beskar",	CraftAttributeInfo.Beskar,	CraftResource.Beskar,	typeof( BeskarIngot ) ),		
			new CraftResourceInfo( 0x829,	0x829,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1063990	,	1063991	,	1063989,	 "Carbonite",	CraftAttributeInfo.Carbonite,	CraftResource.Carbonite,	typeof( CarboniteIngot ) ),		
			new CraftResourceInfo( 0x82C,	0x82C,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1063994	,	1063995	,	1063993,	 "Cortosis",	CraftAttributeInfo.Cortosis,	CraftResource.Cortosis,	typeof( CortosisIngot ) ),		
			new CraftResourceInfo( 0x7A9,	0x7A9,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1063998	,	1063999	,	1063997,	 "Durasteel",	CraftAttributeInfo.Durasteel,	CraftResource.Durasteel,	typeof( DurasteelIngot ) ),		
			new CraftResourceInfo( 0x877,	0x877,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1064002	,	1064003	,	1064001,	 "Durite",	CraftAttributeInfo.Durite,	CraftResource.Durite,	typeof( DuriteIngot ) ),		
			new CraftResourceInfo( 0x775,	0x775,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1064006	,	1064007	,	1064005,	 "Farium",	CraftAttributeInfo.Farium,	CraftResource.Farium,	typeof( FariumIngot ) ),		
			new CraftResourceInfo( 0x77F,	0x77F,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1064010	,	1064011	,	1064009,	 "Laminasteel",	CraftAttributeInfo.Laminasteel,	CraftResource.Laminasteel,	typeof( LaminasteelIngot ) ),		
			new CraftResourceInfo( 0x870,	0x870,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1064014	,	1064015	,	1064013,	 "Neuranium",	CraftAttributeInfo.Neuranium,	CraftResource.Neuranium,	typeof( NeuraniumIngot ) ),		
			new CraftResourceInfo( 0xAF8,	0xAF8,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1064018	,	1064019	,	1064017,	 "Phrik",	CraftAttributeInfo.Phrik,	CraftResource.Phrik,	typeof( PhrikIngot ) ),		
			new CraftResourceInfo( 0x6F6,	0x6F6,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1064022	,	1064023	,	1064021,	 "Promethium",	CraftAttributeInfo.Promethium,	CraftResource.Promethium,	typeof( PromethiumIngot ) ),		
			new CraftResourceInfo( 0x705,	0x705,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1064026	,	1064027	,	1064025,	 "Quadranium",	CraftAttributeInfo.Quadranium,	CraftResource.Quadranium,	typeof( QuadraniumIngot ) ),		
			new CraftResourceInfo( 0xB42,	0xB42,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1064030	,	1064031	,	1064029,	 "Songsteel",	CraftAttributeInfo.Songsteel,	CraftResource.Songsteel,	typeof( SongsteelIngot ) ),		
			new CraftResourceInfo( 0xB70,	0xB70,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1064034	,	1064035	,	1064033,	 "Titanium",	CraftAttributeInfo.Titanium,	CraftResource.Titanium,	typeof( TitaniumIngot ) ),		
			new CraftResourceInfo( 0x8C3,	0x8C3,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1064038	,	1064039	,	1064037,	 "Trimantium",	CraftAttributeInfo.Trimantium,	CraftResource.Trimantium,	typeof( TrimantiumIngot ) ),		
			new CraftResourceInfo( 0x701,	0x701,	5	,	13	,	4.25	,	117.0	,	150	,	27	,	41	,	14	,	1	,	1064042	,	1064043	,	1064041,	 "Xonolite",	CraftAttributeInfo.Xonolite,	CraftResource.Xonolite,	typeof( XonoliteIngot ) )		
			};																														
			private static CraftResourceInfo[] m_ScaleInfo = new CraftResourceInfo[]																														
			{																														
			new CraftResourceInfo( 0x807,	0x66D,	2	,	5	,	2.40	,	45.0	,	90	,	20	,	32	,	10	,	1	,	1060875	,	1053129	,	1063788,	 "Crimson",	CraftAttributeInfo.RedScales,	CraftResource.RedScales,	typeof( RedScales ) ),		
			new CraftResourceInfo( 0x809,	0x8A8,	2	,	5	,	2.40	,	45.0	,	90	,	20	,	32	,	10	,	1	,	1060876	,	1053130	,	1053104,	 "Golden",	CraftAttributeInfo.YellowScales,	CraftResource.YellowScales,	typeof( YellowScales ) ),		
			new CraftResourceInfo( 0x803,	0x455,	2	,	5	,	2.40	,	45.0	,	90	,	20	,	32	,	10	,	1	,	1060877	,	1053131	,	1063790,	 "Dark",	CraftAttributeInfo.BlackScales,	CraftResource.BlackScales,	typeof( BlackScales ) ),		
			new CraftResourceInfo( 0x806,	0x851,	2	,	5	,	2.40	,	45.0	,	90	,	20	,	32	,	10	,	1	,	1060878	,	1053132	,	1063791,	 "Viridian",	CraftAttributeInfo.GreenScales,	CraftResource.GreenScales,	typeof( GreenScales ) ),		
			new CraftResourceInfo( 0x808,	0x8FD,	2	,	5	,	2.40	,	45.0	,	90	,	20	,	32	,	10	,	1	,	1060879	,	1053133	,	1063792,	 "Ivory",	CraftAttributeInfo.WhiteScales,	CraftResource.WhiteScales,	typeof( WhiteScales ) ),		
			new CraftResourceInfo( 0x804,	0x8B0,	2	,	5	,	2.40	,	45.0	,	90	,	20	,	32	,	10	,	1	,	1060880	,	1053134	,	1063793,	 "Azure",	CraftAttributeInfo.BlueScales,	CraftResource.BlueScales,	typeof( BlueScales ) ),		
			new CraftResourceInfo( 0x805,	0x805,	2	,	5	,	2.40	,	45.0	,	90	,	20	,	32	,	10	,	1	,	1054017	,	1054016	,	1063794,	 "Dinosaur",	CraftAttributeInfo.DinosaurScales,	CraftResource.DinosaurScales,	typeof( DinosaurScales ) ),		
			new CraftResourceInfo( 0xB80,	0xB80,	2	,	5	,	2.40	,	45.0	,	90	,	20	,	32	,	10	,	1	,	1054153	,	1054019	,	1063820,	 "Metallic",	CraftAttributeInfo.MetallicScales,	CraftResource.MetallicScales,	typeof( MetallicScales ) ),		
			new CraftResourceInfo( 0x436, 	0x973,	2	,	5	,	2.40	,	45.0	,	90	,	20	,	32	,	10	,	1	,	1054154	,	1054026	,	1063822,	 "Brazen",	CraftAttributeInfo.BrazenScales,	CraftResource.BrazenScales,	typeof( BrazenScales ) ),		
			new CraftResourceInfo( 0x435, 	0x54E,	2	,	5	,	2.40	,	45.0	,	90	,	20	,	32	,	10	,	1	,	1054155	,	1054027	,	1063824,	 "Umber",	CraftAttributeInfo.UmberScales,	CraftResource.UmberScales,	typeof( UmberScales ) ),		
			new CraftResourceInfo( 0x424, 	0x979,	2	,	5	,	2.40	,	45.0	,	90	,	20	,	32	,	10	,	1	,	1054156	,	1054028	,	1063826,	 "Violet",	CraftAttributeInfo.VioletScales,	CraftResource.VioletScales,	typeof( VioletScales ) ),		
			new CraftResourceInfo( 0x449, 	0x42A,	2	,	5	,	2.40	,	45.0	,	90	,	20	,	32	,	10	,	1	,	1054157	,	1054029	,	1063828,	 "Platinum",	CraftAttributeInfo.PlatinumScales,	CraftResource.PlatinumScales,	typeof( PlatinumScales ) ),		
			new CraftResourceInfo( 0x99D,	0x99D,	4	,	9	,	3.40	,	115.0	,	90	,	26	,	32	,	10	,	1	,	1054158	,	1060096	,	1063830,	 "Cadalyte",	CraftAttributeInfo.CadalyteScales,	CraftResource.CadalyteScales,	typeof( CadalyteScales ) ),		
			new CraftResourceInfo( 0x5D6,	0x5D6,	3	,	7	,	3.00	,	110.0	,	90	,	22	,	32	,	10	,	1	,	1064170	,	1064171	,	1064172,	 "Gorn",	CraftAttributeInfo.GornScales,	CraftResource.CadalyteScales,	typeof( GornScales ) ),		
			new CraftResourceInfo( 0x5D8,	0x5D8,	3	,	7	,	3.00	,	110.0	,	90	,	22	,	32	,	10	,	1	,	1064174	,	1064175	,	1064176,	 "Trandoshan",	CraftAttributeInfo.TrandoshanScales,	CraftResource.CadalyteScales,	typeof( TrandoshanScales ) ),		
			new CraftResourceInfo( 0x5D5,	0x5D5,	3	,	7	,	3.00	,	110.0	,	90	,	22	,	32	,	10	,	1	,	1064178	,	1064179	,	1064180,	 "Silurian",	CraftAttributeInfo.SilurianScales,	CraftResource.CadalyteScales,	typeof( SilurianScales ) ),		
			new CraftResourceInfo( 0x692,	0x692,	3	,	7	,	3.00	,	110.0	,	90	,	22	,	32	,	10	,	1	,	1064182	,	1064183	,	1064184,	 "Krayt",	CraftAttributeInfo.KraytScales,	CraftResource.CadalyteScales,	typeof( KraytScales ) )		
			};																														
			private static CraftResourceInfo[] m_SpecialInfo = new CraftResourceInfo[]																														
			{																														
			new CraftResourceInfo( 2859,	2859,	3	,	10	,	3.00	,	110.0	,	130	,	26	,	42	,	13	,	1	,	1064088	,	1064102	,	1063811,	 "Spectral",	CraftAttributeInfo.SpectralSpec,	CraftResource.SpectralSpec,	typeof( SpectralSpec ) ),		
			new CraftResourceInfo( 2860,	2860,	3	,	10	,	3.00	,	110.0	,	130	,	26	,	42	,	13	,	1	,	1064089	,	1064103	,	1063812,	 "Dread",	CraftAttributeInfo.DreadSpec,	CraftResource.DreadSpec,	typeof( DreadSpec ) ),		
			new CraftResourceInfo( 2937,	2937,	3	,	10	,	3.00	,	110.0	,	130	,	26	,	42	,	13	,	1	,	1064090	,	1064104	,	1063813,	 "Ghoulish",	CraftAttributeInfo.GhoulishSpec,	CraftResource.GhoulishSpec,	typeof( GhoulishSpec ) ),		
			new CraftResourceInfo( 2817,	2817,	3	,	10	,	3.00	,	110.0	,	130	,	26	,	42	,	13	,	1	,	1064091	,	1064105	,	1063814,	 "Wyrm",	CraftAttributeInfo.WyrmSpec,	CraftResource.WyrmSpec,	typeof( WyrmSpec ) ),		
			new CraftResourceInfo( 2882,	2882,	3	,	10	,	3.00	,	110.0	,	130	,	26	,	42	,	13	,	1	,	1064092	,	1064106	,	1063815,	 "Holy",	CraftAttributeInfo.HolySpec,	CraftResource.HolySpec,	typeof( HolySpec ) ),		
			new CraftResourceInfo( 1194,	1194,	3	,	10	,	3.00	,	110.0	,	130	,	26	,	42	,	13	,	1	,	1064093	,	1064107	,	1063816,	 "Bloodless",	CraftAttributeInfo.BloodlessSpec,	CraftResource.BloodlessSpec,	typeof( BloodlessSpec ) ),		
			new CraftResourceInfo( 2815,	2815,	3	,	10	,	3.00	,	110.0	,	130	,	26	,	42	,	13	,	1	,	1064094	,	1064108	,	1063817,	 "Gilded",	CraftAttributeInfo.GildedSpec,	CraftResource.GildedSpec,	typeof( GildedSpec ) ),		
			new CraftResourceInfo( 2858,	2858,	3	,	10	,	3.00	,	110.0	,	130	,	26	,	42	,	13	,	1	,	1064095	,	1064109	,	1063818,	 "Demilich",	CraftAttributeInfo.DemilichSpec,	CraftResource.DemilichSpec,	typeof( DemilichSpec ) ),		
			new CraftResourceInfo( 2867,	2867,	3	,	10	,	3.00	,	110.0	,	130	,	26	,	42	,	13	,	1	,	1064096	,	1064110	,	1063819,	 "Wintry",	CraftAttributeInfo.WintrySpec,	CraftResource.WintrySpec,	typeof( WintrySpec ) ),		
			new CraftResourceInfo( 0xB54,	0xB54,	3	,	6	,	1.60	,	80.0	,	30	,	6	,	9	,	3	,	0	,	1064097	,	1064111	,	1064077,	 "Fire",	CraftAttributeInfo.FireSpec,	CraftResource.FireSpec,	typeof( FireSpec ) ),		
			new CraftResourceInfo( 0xB57,	0xB57,	3	,	6	,	1.60	,	80.0	,	30	,	6	,	9	,	3	,	0	,	1064098	,	1064112	,	1064079,	 "Cold",	CraftAttributeInfo.ColdSpec,	CraftResource.ColdSpec,	typeof( ColdSpec ) ),		
			new CraftResourceInfo( 0xB51,	0xB51,	3	,	6	,	1.60	,	80.0	,	30	,	6	,	9	,	3	,	0	,	1064099	,	1064113	,	1064081,	 "Venom",	CraftAttributeInfo.PoisSpec,	CraftResource.PoisSpec,	typeof( PoisSpec ) ),		
			new CraftResourceInfo( 0xAFE,	0xAFE,	3	,	6	,	1.60	,	80.0	,	30	,	6	,	9	,	3	,	0	,	1064100	,	1064114	,	1064083,	 "Energy",	CraftAttributeInfo.EngySpec,	CraftResource.EngySpec,	typeof( EngySpec ) ),		
			new CraftResourceInfo( 1072,	1072,	4	,	16	,	4.20	,	120.0	,	150	,	27	,	41	,	14	,	1	,	1064101	,	1064115	,	1018194,	 "Exodus",	CraftAttributeInfo.ExodusSpec,	CraftResource.ExodusSpec,	typeof( ExodusSpec ) ),		
			new CraftResourceInfo( 0x9ED,	0x9ED,	3	,	10	,	3.00	,	110.0	,	130	,	26	,	42	,	13	,	0	,	1064116	,	1064117	,	1064119,	 "Turtle Shell",	CraftAttributeInfo.TurtleSpec,	CraftResource.TurtleSpec,	typeof( TurtleSpec ) )		
			};																														
			private static CraftResourceInfo[] m_LeatherInfo = new CraftResourceInfo[]																														
			{																														
			new CraftResourceInfo( 0x000, 	0x000,	0	,	0	,	1.00	,	0.0		,	0	,	0	,	0	,	0	,	0	,	1049150	,	1034455	,	1049353,	 "Normal",	CraftAttributeInfo.Blank,	CraftResource.RegularLeather,	typeof( Leather ),	typeof( Hides ) ),	
			new CraftResourceInfo( 0x69C, 	0x69C,	1	,	1	,	1.25	,	55.0	,	10	,	2	,	3	,	1	,	0	,	1049152	,	1034457	,	1061117,	 "Lizard",	CraftAttributeInfo.Horned,	CraftResource.HornedLeather,	typeof( HornedLeather ),	typeof( HornedHides ) ),	
			new CraftResourceInfo( 0x69E, 	0x69E,	1	,	2	,	1.50	,	60.0	,	20	,	4	,	6	,	2	,	0	,	1049153	,	1034458	,	1061116,	 "Serpent",	CraftAttributeInfo.Barbed,	CraftResource.BarbedLeather,	typeof( BarbedLeather ),	typeof( BarbedHides ) ),	
			new CraftResourceInfo( 0x69D, 	0x69D,	2	,	3	,	1.50	,	65.0	,	30	,	6	,	9	,	3	,	0	,	1034403	,	1034459	,	1034413,	 "Necrotic",	CraftAttributeInfo.Necrotic,	CraftResource.NecroticLeather,	typeof( NecroticLeather ),	typeof( NecroticHides ) ),	
			new CraftResourceInfo( 0x69F, 	0x69F,	2	,	4	,	2.00	,	70.0	,	40	,	8	,	12	,	4	,	0	,	1034414	,	1034460	,	1034424,	 "Volcanic",	CraftAttributeInfo.Volcanic,	CraftResource.VolcanicLeather,	typeof( VolcanicLeather ),	typeof( VolcanicHides ) ),	
			new CraftResourceInfo( 0x699, 	0x699,	2	,	5	,	1.75	,	75.0	,	50	,	10	,	15	,	5	,	0	,	1034425	,	1034461	,	1034435,	 "Frozen",	CraftAttributeInfo.Frozen,	CraftResource.FrozenLeather,	typeof( FrozenLeather ),	typeof( FrozenHides ) ),	
			new CraftResourceInfo( 0x696, 	0x696,	3	,	6	,	2.00	,	80.0	,	60	,	12	,	18	,	6	,	0	,	1049151	,	1034456	,	1061118,	 "Deep Sea",	CraftAttributeInfo.Spined,	CraftResource.SpinedLeather,	typeof( SpinedLeather ),	typeof( SpinedHides ) ),	
			new CraftResourceInfo( 0x69A, 	0x69A,	3	,	7	,	2.00	,	85.0	,	70	,	14	,	21	,	7	,	0	,	1034370	,	1034462	,	1034380,	 "Goliath",	CraftAttributeInfo.Goliath,	CraftResource.GoliathLeather,	typeof( GoliathLeather ),	typeof( GoliathHides ) ),	
			new CraftResourceInfo( 0x698, 	0x698,	3	,	8	,	2.25	,	90.0	,	80	,	16	,	24	,	8	,	0	,	1034381	,	1034463	,	1034391,	 "Draconic",	CraftAttributeInfo.Draconic,	CraftResource.DraconicLeather,	typeof( DraconicLeather ),	typeof( DraconicHides ) ),	
			new CraftResourceInfo( 0x69B, 	0x69B,	4	,	9	,	2.25	,	100.0	,	90	,	18	,	27	,	9	,	0	,	1034392	,	1034464	,	1034402,	 "Hellish",	CraftAttributeInfo.Hellish,	CraftResource.HellishLeather,	typeof( HellishLeather ),	typeof( HellishHides ) ),	
			new CraftResourceInfo( 0x697, 	0x697,	4	,	10	,	3.00	,	105.0	,	100	,	20	,	30	,	10	,	1	,	1036104	,	1034465	,	1036161,	 "Dinosaur",	CraftAttributeInfo.Dinosaur,	CraftResource.DinosaurLeather,	typeof( DinosaurLeather ),	typeof( DinosaurHides ) ),	
			new CraftResourceInfo( 0x695, 	0x695,	5	,	11	,	4.00	,	125.0	,	110	,	22	,	33	,	11	,	1	,	1034444	,	1034466	,	1034454,	 "Alien",	CraftAttributeInfo.Alien,	CraftResource.AlienLeather,	typeof( AlienLeather ),	typeof( AlienHides ) ),	
			new CraftResourceInfo( 0xAF8,	0xAF8,	5	,	12	,	3.75	,	110.0	,	120	,	24	,	36	,	12	,	1	,	1063938	,	1063939	,	1063937,	 "Adesote",	CraftAttributeInfo.Adesote,	CraftResource.Adesote,	typeof( AdesoteLeather ) ),		
			new CraftResourceInfo( 0x829,	0x829,	5	,	12	,	3.75	,	110.0	,	120	,	24	,	36	,	12	,	1	,	1063942	,	1063943	,	1063941,	 "Biomesh",	CraftAttributeInfo.Biomesh,	CraftResource.Biomesh,	typeof( BiomeshLeather ) ),		
			new CraftResourceInfo( 0xB57,	0xB57,	5	,	12	,	3.75	,	110.0	,	120	,	24	,	36	,	12	,	1	,	1063946	,	1063947	,	1063945,	 "Cerlin",	CraftAttributeInfo.Cerlin,	CraftResource.Cerlin,	typeof( CerlinLeather ) ),		
			new CraftResourceInfo( 0x8C1,	0x8C1,	5	,	12	,	3.75	,	110.0	,	120	,	24	,	36	,	12	,	1	,	1063950	,	1063951	,	1063949,	 "Durafiber",	CraftAttributeInfo.Durafiber,	CraftResource.Durafiber,	typeof( DurafiberLeather ) ),		
			new CraftResourceInfo( 0x705,	0x705,	5	,	12	,	3.75	,	110.0	,	120	,	24	,	36	,	12	,	1	,	1063954	,	1063955	,	1063953,	 "Flexicris",	CraftAttributeInfo.Flexicris,	CraftResource.Flexicris,	typeof( FlexicrisLeather ) ),		
			new CraftResourceInfo( 0x77F,	0x77F,	5	,	12	,	3.75	,	110.0	,	120	,	24	,	36	,	12	,	1	,	1063958	,	1063959	,	1063957,	 "Hypercloth",	CraftAttributeInfo.Hypercloth,	CraftResource.Hypercloth,	typeof( HyperclothLeather ) ),		
			new CraftResourceInfo( 0x701,	0x701,	5	,	12	,	3.75	,	110.0	,	120	,	24	,	36	,	12	,	1	,	1063962	,	1063963	,	1063961,	 "Nylar",	CraftAttributeInfo.Nylar,	CraftResource.Nylar,	typeof( NylarLeather ) ),		
			new CraftResourceInfo( 0x6F8,	0x6F8,	5	,	12	,	3.75	,	110.0	,	120	,	24	,	36	,	12	,	1	,	1063966	,	1063967	,	1063965,	 "Nylonite",	CraftAttributeInfo.Nylonite,	CraftResource.Nylonite,	typeof( NyloniteLeather ) ),		
			new CraftResourceInfo( 0x6F6,	0x6F6,	5	,	12	,	3.75	,	110.0	,	120	,	24	,	36	,	12	,	1	,	1063970	,	1063971	,	1063969,	 "Polyfiber",	CraftAttributeInfo.Polyfiber,	CraftResource.Polyfiber,	typeof( PolyfiberLeather ) ),		
			new CraftResourceInfo( 0x7A9,	0x7A9,	5	,	12	,	3.75	,	110.0	,	120	,	24	,	36	,	12	,	1	,	1063974	,	1063975	,	1063973,	 "Syncloth",	CraftAttributeInfo.Syncloth,	CraftResource.Syncloth,	typeof( SynclothLeather ) ),		
			new CraftResourceInfo( 0x775,	0x775,	5	,	12	,	3.75	,	110.0	,	120	,	24	,	36	,	12	,	1	,	1063978	,	1063979	,	1063977,	 "Thermoweave",	CraftAttributeInfo.Thermoweave,	CraftResource.Thermoweave,	typeof( ThermoweaveLeather ) )		
			};																														
			private static CraftResourceInfo[] m_WoodInfo = new CraftResourceInfo[]																														
			{																														
			new CraftResourceInfo( 0x000,	0x000,	0	,	0	,	1.00	,	0.0		,	0	,	0	,	0	,	0	,	0	,	1072643	,	1015101	,	1011542,	 "Normal",	CraftAttributeInfo.Blank,	CraftResource.RegularWood,	typeof( Board ),	typeof( Log ) ),	
			new CraftResourceInfo( 0x509,	0x509,	1	,	1	,	1.20	,	65.0	,	10	,	1	,	2	,	1	,	0	,	1095379	,	1095389	,	1095399,	 "Ash",	CraftAttributeInfo.AshTree,	CraftResource.AshTree,	typeof( AshBoard ),	typeof( AshLog ) ),	
			new CraftResourceInfo( 0x50A,	0x50A,	1	,	2	,	1.40	,	70.0	,	20	,	2	,	4	,	2	,	0	,	1095380	,	1095390	,	1095400,	 "Cherry",	CraftAttributeInfo.CherryTree,	CraftResource.CherryTree,	typeof( CherryBoard ),	typeof( CherryLog ) ),	
			new CraftResourceInfo( 0x50B,	0x50B,	1	,	3	,	1.60	,	75.0	,	30	,	3	,	6	,	3	,	0	,	1095381	,	1095391	,	1095401,	 "Ebony",	CraftAttributeInfo.EbonyTree,	CraftResource.EbonyTree,	typeof( EbonyBoard ),	typeof( EbonyLog ) ),	
			new CraftResourceInfo( 0x50E,	0x50E,	1	,	4	,	1.80	,	80.0	,	40	,	4	,	8	,	4	,	0	,	1095382	,	1095392	,	1095402,	 "Golden Oak",	CraftAttributeInfo.GoldenOakTree,	CraftResource.GoldenOakTree,	typeof( GoldenOakBoard ),	typeof( GoldenOakLog ) ),	
			new CraftResourceInfo( 0x508,	0x508,	2	,	5	,	2.00	,	85.0	,	50	,	5	,	10	,	5	,	0	,	1095383	,	1095393	,	1095403,	 "Hickory",	CraftAttributeInfo.HickoryTree,	CraftResource.HickoryTree,	typeof( HickoryBoard ),	typeof( HickoryLog ) ),	
			new CraftResourceInfo( 0x50F,	0x50F,	2	,	6	,	2.20	,	90.0	,	60	,	6	,	12	,	6	,	0	,	1095384	,	1095394	,	1095404,	 "Mahogany",	CraftAttributeInfo.MahoganyTree,	CraftResource.MahoganyTree,	typeof( MahoganyBoard ),	typeof( MahoganyLog ) ),	
			new CraftResourceInfo( 0x510,	0x510,	2	,	7	,	2.40	,	95.0	,	70	,	8	,	20	,	7	,	0	,	1095385	,	1095395	,	1095405,	 "Oak",	CraftAttributeInfo.OakTree,	CraftResource.OakTree,	typeof( OakBoard ),	typeof( OakLog ) ),	
			new CraftResourceInfo( 0x512,	0x512,	2	,	8	,	2.60	,	95.0	,	80	,	9	,	22	,	8	,	0	,	1095386	,	1095396	,	1095406,	 "Pine",	CraftAttributeInfo.PineTree,	CraftResource.PineTree,	typeof( PineBoard ),	typeof( PineLog ) ),	
			new CraftResourceInfo( 0x50D,	0x50D,	2	,	9	,	2.60	,	105.0	,	90	,	10	,	16	,	9	,	0	,	1095511	,	1095512	,	1095513,	 "Ghostwood",	CraftAttributeInfo.GhostTree,	CraftResource.GhostTree,	typeof( GhostBoard ),	typeof( GhostLog ) ),	
			new CraftResourceInfo( 0x513,	0x513,	2	,	10	,	2.80	,	100.0	,	100	,	11	,	24	,	10	,	0	,	1095387	,	1095397	,	1095407,	 "Rosewood",	CraftAttributeInfo.RosewoodTree,	CraftResource.RosewoodTree,	typeof( RosewoodBoard ),	typeof( RosewoodLog ) ),	
			new CraftResourceInfo( 0x514,	0x514,	3	,	11	,	3.00	,	100.0	,	110	,	12	,	26	,	11	,	0	,	1095388	,	1095398	,	1095408,	 "Walnut",	CraftAttributeInfo.WalnutTree,	CraftResource.WalnutTree,	typeof( WalnutBoard ),	typeof( WalnutLog ) ),	
			new CraftResourceInfo( 0x511,	0x511,	3	,	12	,	3.25	,	115.0	,	120	,	13	,	18	,	12	,	0	,	1095532	,	1095533	,	1095534,	 "Petrified",	CraftAttributeInfo.PetrifiedTree,	CraftResource.PetrifiedTree,	typeof( PetrifiedBoard ),	typeof( PetrifiedLog ) ),	
			new CraftResourceInfo( 0x507,	0x507,	3	,	13	,	2.40	,	115.0	,	130	,	7	,	14	,	13	,	0	,	1095409	,	1095410	,	1095510,	 "Driftwood",	CraftAttributeInfo.DriftwoodTree,	CraftResource.DriftwoodTree,	typeof( DriftwoodBoard ),	typeof( DriftwoodLog ) ),	
			new CraftResourceInfo( 0x50C,	0x50C,	4	,	14	,	3.40	,	125.0	,	140	,	14	,	33	,	14	,	1	,	1095535	,	1095536	,	1095537,	 "Elven",	CraftAttributeInfo.ElvenTree,	CraftResource.ElvenTree,	typeof( ElvenBoard ),	typeof( ElvenLog ) ),	
			new CraftResourceInfo( 0x775,	0x775,	4	,	15	,	3.50	,	115.0	,	150	,	13	,	30	,	15	,	1	,	1064046	,	1064047	,	1064045,	 "Borl",	CraftAttributeInfo.BorlTree,	CraftResource.BorlTree,	typeof( BorlBoard ) ),		
			new CraftResourceInfo( 0x77F,	0x77F,	4	,	15	,	3.50	,	115.0	,	150	,	13	,	30	,	15	,	1	,	1064050	,	1064051	,	1064049,	 "Cosian",	CraftAttributeInfo.CosianTree,	CraftResource.CosianTree,	typeof( CosianBoard ) ),		
			new CraftResourceInfo( 0x870,	0x870,	4	,	15	,	3.50	,	115.0	,	150	,	13	,	30	,	15	,	1	,	1064054	,	1064055	,	1064053,	 "Greel",	CraftAttributeInfo.GreelTree,	CraftResource.GreelTree,	typeof( GreelBoard ) ),		
			new CraftResourceInfo( 0x948,	0x948,	4	,	15	,	3.50	,	115.0	,	150	,	13	,	30	,	15	,	1	,	1064058	,	1064059	,	1064057,	 "Japor",	CraftAttributeInfo.JaporTree,	CraftResource.JaporTree,	typeof( JaporBoard ) ),		
			new CraftResourceInfo( 0x705,	0x705,	4	,	15	,	3.50	,	115.0	,	150	,	13	,	30	,	15	,	1	,	1064062	,	1064063	,	1064061,	 "Kyshyyyk",	CraftAttributeInfo.KyshyyykTree,	CraftResource.KyshyyykTree,	typeof( KyshyyykBoard ) ),		
			new CraftResourceInfo( 0x877,	0x877,	4	,	15	,	3.50	,	115.0	,	150	,	13	,	30	,	15	,	1	,	1064066	,	1064067	,	1064065,	 "Laroon",	CraftAttributeInfo.LaroonTree,	CraftResource.LaroonTree,	typeof( LaroonBoard ) ),		
			new CraftResourceInfo( 0x6F6,	0x6F6,	4	,	15	,	3.50	,	115.0	,	150	,	13	,	30	,	15	,	1	,	1064070	,	1064071	,	1064069,	 "Teej",	CraftAttributeInfo.TeejTree,	CraftResource.TeejTree,	typeof( TeejBoard ) ),		
			new CraftResourceInfo( 0x6F8,	0x6F8,	4	,	15	,	3.50	,	115.0	,	150	,	13	,	30	,	15	,	1	,	1064074	,	1064075	,	1064073,	 "Veshok",	CraftAttributeInfo.VeshokTree,	CraftResource.VeshokTree,	typeof( VeshokBoard ) )		
			};																														
			private static CraftResourceInfo[] m_FabricInfo = new CraftResourceInfo[]																														
			{																														
			new CraftResourceInfo( 0x000,	0x000,	0	,	0	,	1.00	,	0.0		,	0	,	0	,	0	,	0	,	0	,	1064120	,	1064121	,	1064123,	 "Normal",	CraftAttributeInfo.Blank,	CraftResource.Fabric,	typeof( Fabric ) ),		
			new CraftResourceInfo( 0x8BC,	0x8BC,	1	,	1	,	1.20	,	45.0	,	10	,	2	,	3	,	1	,	0	,	1064124	,	1064125	,	1064127,	 "Furry",	CraftAttributeInfo.FurryFabric,	CraftResource.FurryFabric,	typeof( FurryFabric ) ),		
			new CraftResourceInfo( 0x911,	0x911,	1	,	2	,	1.40	,	50.0	,	20	,	4	,	6	,	2	,	0	,	1064128	,	1064129	,	1064131,	 "Wooly",	CraftAttributeInfo.WoolyFabric,	CraftResource.WoolyFabric,	typeof( WoolyFabric ) ),		
			new CraftResourceInfo( 0xAFE,	0xAFE,	1	,	3	,	1.60	,	60.0	,	30	,	6	,	9	,	3	,	0	,	1064132	,	1064133	,	1064135,	 "Silk",	CraftAttributeInfo.SilkFabric,	CraftResource.SilkFabric,	typeof( SilkFabric ) ),		
			new CraftResourceInfo( 0xB3B,	0xB3B,	2	,	4	,	1.80	,	65.0	,	40	,	8	,	12	,	4	,	0	,	1064136	,	1064137	,	1064139,	 "Haunted",	CraftAttributeInfo.HauntedFabric,	CraftResource.HauntedFabric,	typeof( HauntedFabric ) ),		
			new CraftResourceInfo( 0x9A3,	0x9A3,	2	,	5	,	2.00	,	70.0	,	50	,	10	,	15	,	5	,	0	,	1064140	,	1064141	,	1064142,	 "Arctic",	CraftAttributeInfo.ArcticFabric,	CraftResource.ArcticFabric,	typeof( ArcticFabric ) ),		
			new CraftResourceInfo( 0x981,	0x981,	2	,	5	,	2.20	,	75.0	,	60	,	12	,	18	,	6	,	0	,	1064144	,	1064145	,	1064147,	 "Pyre",	CraftAttributeInfo.PyreFabric,	CraftResource.PyreFabric,	typeof( PyreFabric ) ),		
			new CraftResourceInfo( 0xB0C,	0xB0C,	2	,	5	,	2.40	,	75.0	,	70	,	14	,	21	,	7	,	0	,	1064148	,	1064149	,	1064151,	 "Venomous",	CraftAttributeInfo.VenomousFabric,	CraftResource.VenomousFabric,	typeof( VenomousFabric ) ),		
			new CraftResourceInfo( 0x8E4,	0x8E4,	3	,	6	,	2.60	,	80.0	,	80	,	16	,	24	,	8	,	0	,	1064152	,	1064153	,	1064155,	 "Mysterious",	CraftAttributeInfo.MysteriousFabric,	CraftResource.MysteriousFabric,	typeof( MysteriousFabric ) ),		
			new CraftResourceInfo( 0x7B1,	0x7B1,	3	,	7	,	2.60	,	90.0	,	90	,	18	,	27	,	9	,	0	,	1064156	,	1064157	,	1064159,	 "Vile",	CraftAttributeInfo.VileFabric,	CraftResource.VileFabric,	typeof( VileFabric ) ),		
			new CraftResourceInfo( 0x8D7,	0x8D7,	3	,	7	,	2.80	,	99.0	,	100	,	20	,	30	,	10	,	1	,	1064160	,	1064161	,	1064163,	 "Divine",	CraftAttributeInfo.DivineFabric,	CraftResource.DivineFabric,	typeof( DivineFabric ) ),		
			new CraftResourceInfo( 0x870,	0x870,	4	,	8	,	3.00	,	105.0	,	110	,	22	,	33	,	11	,	1	,	1064164	,	1064165	,	1064167,	 "Fiendish",	CraftAttributeInfo.FiendishFabric,	CraftResource.FiendishFabric,	typeof( FiendishFabric ) )		
			};																														
			private static CraftResourceInfo[] m_BlockInfo = new CraftResourceInfo[]																														
			{																														
			new CraftResourceInfo( 0x8D5,	0x8D5,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063690	,	1063706	,	1063707,	 "Amethyst",	CraftAttributeInfo.AmethystBlock,	CraftResource.AmethystBlock,	typeof( AmethystBlocks ),	typeof( AmethystStone ) ),	
			new CraftResourceInfo( 0x950,	0x950,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063691	,	1063709	,	1063710,	 "Emerald",	CraftAttributeInfo.EmeraldBlock,	CraftResource.EmeraldBlock,	typeof( EmeraldBlocks ),	typeof( EmeraldStone ) ),	
			new CraftResourceInfo( 0x4A2,	0x4A2,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063692	,	1063712	,	1063713,	 "Garnet",	CraftAttributeInfo.GarnetBlock,	CraftResource.GarnetBlock,	typeof( GarnetBlocks ),	typeof( GarnetStone ) ),	
			new CraftResourceInfo( 0x8E2,	0xAF3,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063693	,	1063715	,	1063716,	 "Ice",	CraftAttributeInfo.IceBlock,	CraftResource.IceBlock,	typeof( IceBlocks ),	typeof( IceStone ) ),	
			new CraftResourceInfo( 0xB0C,	0xB0C,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063694	,	1063718	,	1063719,	 "Jade",	CraftAttributeInfo.JadeBlock,	CraftResource.JadeBlock,	typeof( JadeBlocks ),	typeof( JadeStone ) ),	
			new CraftResourceInfo( 0xB3B,	0xB3B,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063695	,	1063721	,	1063722,	 "Marble",	CraftAttributeInfo.MarbleBlock,	CraftResource.MarbleBlock,	typeof( MarbleBlocks ),	typeof( MarbleStone ) ),	
			new CraftResourceInfo( 0xB5E,	0x839,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063696	,	1063724	,	1063725,	 "Onyx",	CraftAttributeInfo.OnyxBlock,	CraftResource.OnyxBlock,	typeof( OnyxBlocks ),	typeof( OnyxStone ) ),	
			new CraftResourceInfo( 0x869,	0x84D,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063697	,	1063727	,	1063728,	 "Quartz",	CraftAttributeInfo.QuartzBlock,	CraftResource.QuartzBlock,	typeof( QuartzBlocks ),	typeof( QuartzStone ) ),	
			new CraftResourceInfo( 0x982,	0x982,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063698	,	1063730	,	1063731,	 "Ruby",	CraftAttributeInfo.RubyBlock,	CraftResource.RubyBlock,	typeof( RubyBlocks ),	typeof( RubyStone ) ),	
			new CraftResourceInfo( 0x5CE,	0x5CE,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063699	,	1063733	,	1063734,	 "Sapphire",	CraftAttributeInfo.SapphireBlock,	CraftResource.SapphireBlock,	typeof( SapphireBlocks ),	typeof( SapphireStone ) ),	
			new CraftResourceInfo( 0xB2A,	0xB2A,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063700	,	1063736	,	1063737,	 "Silver",	CraftAttributeInfo.SilverBlock,	CraftResource.SilverBlock,	typeof( SilverBlocks ),	typeof( SilverStone ) ),	
			new CraftResourceInfo( 0x7CB,	0x6DF,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063701	,	1063739	,	1063740,	 "Spinel",	CraftAttributeInfo.SpinelBlock,	CraftResource.SpinelBlock,	typeof( SpinelBlocks ),	typeof( SpinelStone ) ),	
			new CraftResourceInfo( 0x7CA,	0x7CA,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063702	,	1063742	,	1063743,	 "Star Ruby",	CraftAttributeInfo.StarRubyBlock,	CraftResource.StarRubyBlock,	typeof( StarRubyBlocks ),	typeof( StarRubyStone ) ),	
			new CraftResourceInfo( 0x856,	0x883,	3	,	12	,	3.40	,	85.0	,	140	,	20	,	32	,	12	,	1	,	1063703	,	1063745	,	1063746,	 "Topaz",	CraftAttributeInfo.TopazBlock,	CraftResource.TopazBlock,	typeof( TopazBlocks ),	typeof( TopazStone ) ),	
			new CraftResourceInfo( 0x99D,	0x99D,	4	,	16	,	4.00	,	115.0	,	200	,	28	,	42	,	15	,	1	,	1063704	,	1063748	,	1063749,	 "Caddellite",	CraftAttributeInfo.CaddelliteBlock,	CraftResource.CaddelliteBlock,	typeof( CaddelliteBlocks ),	typeof( CaddelliteStone ) )	
			};																														
			private static CraftResourceInfo[] m_SkinInfo = new CraftResourceInfo[]																														
			{																														
			new CraftResourceInfo( 0xB1E,	0xB1E,	3	,	8	,	3.20	,	65.0	,	100	,	20	,	30	,	10	,	1	,	1063750	,	1063757	,	1063758,	 "Demon",	CraftAttributeInfo.DemonSkin,	CraftResource.DemonSkin,	typeof( DemonSkins ) ),		
			new CraftResourceInfo( 0x960,	0x960,	3	,	8	,	3.20	,	65.0	,	100	,	20	,	30	,	10	,	1	,	1063751	,	1063760	,	1063761,	 "Dragon",	CraftAttributeInfo.DragonSkin,	CraftResource.DragonSkin,	typeof( DragonSkins ) ),		
			new CraftResourceInfo( 0xB80,	0xB80,	3	,	8	,	3.20	,	65.0	,	100	,	20	,	30	,	10	,	1	,	1063752	,	1063763	,	1063764,	 "Nightmare",	CraftAttributeInfo.NightmareSkin,	CraftResource.NightmareSkin,	typeof( NightmareSkins ) ),		
			new CraftResourceInfo( 0xB79,	0xB79,	3	,	8	,	3.20	,	65.0	,	100	,	20	,	30	,	10	,	1	,	1063753	,	1063766	,	1063767,	 "Snake",	CraftAttributeInfo.SnakeSkin,	CraftResource.SnakeSkin,	typeof( SnakeSkins ) ),		
			new CraftResourceInfo( 0xB4C,	0xB4C,	3	,	8	,	3.20	,	65.0	,	100	,	20	,	30	,	10	,	1	,	1063754	,	1063769	,	1063770,	 "Troll",	CraftAttributeInfo.TrollSkin,	CraftResource.TrollSkin,	typeof( TrollSkins ) ),		
			new CraftResourceInfo( 0xBB4,	0xBB4,	3	,	8	,	3.20	,	65.0	,	100	,	20	,	30	,	10	,	1	,	1063755	,	1063772	,	1063773,	 "Unicorn",	CraftAttributeInfo.UnicornSkin,	CraftResource.UnicornSkin,	typeof( UnicornSkins ) ),		
			new CraftResourceInfo( 0xB7A,	0xB7A,	3	,	10	,	3.40	,	75.0	,	100	,	24	,	40	,	12	,	1	,	1064084	,	1063775	,	1063776,	 "Icy",	CraftAttributeInfo.IcySkin,	CraftResource.IcySkin,	typeof( IcySkins ) ),		
			new CraftResourceInfo( 0xB17,	0xB17,	3	,	10	,	3.40	,	75.0	,	100	,	24	,	40	,	12	,	1	,	1064085	,	1063778	,	1063779,	 "Lava",	CraftAttributeInfo.LavaSkin,	CraftResource.LavaSkin,	typeof( LavaSkins ) ),		
			new CraftResourceInfo( 0x98D,	0x98D,	3	,	10	,	3.40	,	75.0	,	100	,	24	,	40	,	12	,	1	,	1064086	,	1063781	,	1063782,	 "Seaweed",	CraftAttributeInfo.Seaweed,	CraftResource.Seaweed,	typeof( Seaweeds ) ),		
			new CraftResourceInfo( 0xB4A,	0xB4A,	3	,	10	,	3.40	,	75.0	,	100	,	24	,	40	,	12	,	1	,	1064087	,	1063784	,	1063785,	 "Dead",	CraftAttributeInfo.DeadSkin,	CraftResource.DeadSkin,	typeof( DeadSkins ) )		
			};																														
			private static CraftResourceInfo[] m_SkeletalInfo = new CraftResourceInfo[]																														
			{																														
			new CraftResourceInfo( 0x000,	0x000,	0	,	0	,	1.00	,	0.0		,	0	,	0	,	0	,	0	,	0	,	1063832	,	1063833	,	1063835,	 "Brittle",	CraftAttributeInfo.Blank,	CraftResource.BrittleSkeletal,	typeof( BrittleSkeletal ) ),		
			new CraftResourceInfo( 0x424,	0x424,	1	,	1	,	1.20	,	55.0	,	10	,	2	,	3	,	1	,	0	,	1063840	,	1063841	,	1063843,	 "Drow",	CraftAttributeInfo.DrowSkeletal,	CraftResource.DrowSkeletal,	typeof( DrowSkeletal ) ),		
			new CraftResourceInfo( 0x44C,	0x44C,	1	,	2	,	1.20	,	60.0	,	20	,	4	,	6	,	2	,	0	,	1063844	,	1063845	,	1063847,	 "Orc",	CraftAttributeInfo.OrcSkeletal,	CraftResource.OrcSkeletal,	typeof( OrcSkeletal ) ),		
			new CraftResourceInfo( 0x806,	0x806,	1	,	3	,	1.40	,	65.0	,	30	,	6	,	9	,	3	,	0	,	1063848	,	1063849	,	1063851,	 "Reptile",	CraftAttributeInfo.ReptileSkeletal,	CraftResource.ReptileSkeletal,	typeof( ReptileSkeletal ) ),		
			new CraftResourceInfo( 0x5B2,	0x5B2,	1	,	4	,	1.40	,	70.0	,	40	,	8	,	12	,	4	,	0	,	1063852	,	1063853	,	1063855,	 "Ogre",	CraftAttributeInfo.OgreSkeletal,	CraftResource.OgreSkeletal,	typeof( OgreSkeletal ) ),		
			new CraftResourceInfo( 0x961,	0x961,	1	,	4	,	1.40	,	70.0	,	40	,	8	,	12	,	4	,	0	,	1063856	,	1063857	,	1063859,	 "Troll",	CraftAttributeInfo.TrollSkeletal,	CraftResource.TrollSkeletal,	typeof( TrollSkeletal ) ),		
			new CraftResourceInfo( 0x807,	0x807,	2	,	5	,	1.60	,	75.0	,	50	,	10	,	15	,	5	,	0	,	1063860	,	1063861	,	1063863,	 "Gargoyle",	CraftAttributeInfo.GargoyleSkeletal,	CraftResource.GargoyleSkeletal,	typeof( GargoyleSkeletal ) ),		
			new CraftResourceInfo( 0x83F,	0x83F,	2	,	6	,	1.60	,	80.0	,	60	,	12	,	18	,	6	,	0	,	1063864	,	1063865	,	1063867,	 "Minotaur",	CraftAttributeInfo.MinotaurSkeletal,	CraftResource.MinotaurSkeletal,	typeof( MinotaurSkeletal ) ),		
			new CraftResourceInfo( 0x436,	0x436,	2	,	7	,	1.60	,	85.0	,	70	,	14	,	21	,	7	,	0	,	1063868	,	1063869	,	1063871,	 "Lycan",	CraftAttributeInfo.LycanSkeletal,	CraftResource.LycanSkeletal,	typeof( LycanSkeletal ) ),		
			new CraftResourceInfo( 0x43F,	0x43F,	2	,	8	,	1.80	,	90.0	,	80	,	16	,	24	,	8	,	0	,	1063872	,	1063873	,	1063875,	 "Shark",	CraftAttributeInfo.SharkSkeletal,	CraftResource.SharkSkeletal,	typeof( SharkSkeletal ) ),		
			new CraftResourceInfo( 0x69A,	0x69A,	2	,	9	,	1.80	,	91.0	,	90	,	18	,	27	,	9	,	0	,	1063876	,	1063877	,	1063879,	 "Colossal",	CraftAttributeInfo.ColossalSkeletal,	CraftResource.ColossalSkeletal,	typeof( ColossalSkeletal ) ),		
			new CraftResourceInfo( 0x809,	0x809,	3	,	10	,	2.00	,	93.0	,	100	,	20	,	30	,	10	,	0	,	1063880	,	1063881	,	1063883,	 "Mystical",	CraftAttributeInfo.MysticalSkeletal,	CraftResource.MysticalSkeletal,	typeof( MysticalSkeletal ) ),		
			new CraftResourceInfo( 0x803,	0x803,	3	,	11	,	2.00	,	95.0	,	110	,	21	,	31	,	11	,	0	,	1063884	,	1063885	,	1063887,	 "Vampire",	CraftAttributeInfo.VampireSkeletal,	CraftResource.VampireSkeletal,	typeof( VampireSkeletal ) ),		
			new CraftResourceInfo( 0x808,	0x808,	3	,	12	,	2.20	,	97.0	,	120	,	22	,	32	,	12	,	0	,	1063888	,	1063889	,	1063891,	 "Lich",	CraftAttributeInfo.LichSkeletal,	CraftResource.LichSkeletal,	typeof( LichSkeletal ) ),		
			new CraftResourceInfo( 0x804,	0x804,	3	,	13	,	2.20	,	99.0	,	130	,	23	,	33	,	13	,	1	,	1063892	,	1063893	,	1063895,	 "Sphinx",	CraftAttributeInfo.SphinxSkeletal,	CraftResource.SphinxSkeletal,	typeof( SphinxSkeletal ) ),		
			new CraftResourceInfo( 0x648,	0x648,	4	,	15	,	2.40	,	102.0	,	150	,	24	,	34	,	14	,	1	,	1063896	,	1063897	,	1063899,	 "Devil",	CraftAttributeInfo.DevilSkeletal,	CraftResource.DevilSkeletal,	typeof( DevilSkeletal ) ),		
			new CraftResourceInfo( 0x437,	0x698,	4	,	17	,	2.40	,	105.0	,	170	,	25	,	35	,	15	,	1	,	1063836	,	1063837	,	1063839,	 "Draco",	CraftAttributeInfo.DracoSkeletal,	CraftResource.DracoSkeletal,	typeof( DracoSkeletal ) ),		
			new CraftResourceInfo( 0x44D,	0x44D,	5	,	18	,	3.00	,	110.0	,	180	,	28	,	38	,	16	,	1	,	1063900	,	1063901	,	1063903,	 "Xeno",	CraftAttributeInfo.XenoSkeletal,	CraftResource.XenoSkeletal,	typeof( XenoSkeletal ) ),		
			new CraftResourceInfo( 0xB3D,	0xB3D,	4	,	14	,	3.10	,	100.1	,	140	,	26	,	37	,	16	,	1	,	1063906	,	1063907	,	1063905,	 "Andorian",	CraftAttributeInfo.AndorianSkeletal,	CraftResource.AndorianSkeletal,	typeof( AndorianSkeletal ) ),		
			new CraftResourceInfo( 0x986,	0x986,	4	,	14	,	3.10	,	100.1	,	140	,	26	,	37	,	16	,	1	,	1063910	,	1063911	,	1063909,	 "Cardassian",	CraftAttributeInfo.CardassianSkeletal,	CraftResource.CardassianSkeletal,	typeof( CardassianSkeletal ) ),		
			new CraftResourceInfo( 0x6F6,	0x6F6,	4	,	14	,	3.10	,	100.1	,	140	,	26	,	37	,	16	,	1	,	1063914	,	1063915	,	1063913,	 "Martian",	CraftAttributeInfo.MartianSkeletal,	CraftResource.MartianSkeletal,	typeof( MartianSkeletal ) ),		
			new CraftResourceInfo( 0x77F,	0x77F,	4	,	14	,	3.10	,	100.1	,	140	,	26	,	37	,	16	,	1	,	1063918	,	1063919	,	1063917,	 "Rodian",	CraftAttributeInfo.RodianSkeletal,	CraftResource.RodianSkeletal,	typeof( RodianSkeletal ) ),		
			new CraftResourceInfo( 0x775,	0x775,	4	,	14	,	3.10	,	100.1	,	140	,	26	,	37	,	16	,	1	,	1063922	,	1063923	,	1063921,	 "Tusken",	CraftAttributeInfo.TuskenSkeletal,	CraftResource.TuskenSkeletal,	typeof( TuskenSkeletal ) ),		
			new CraftResourceInfo( 0xAF8,	0xAF8,	4	,	14	,	3.10	,	100.1	,	140	,	26	,	37	,	16	,	1	,	1063926	,	1063927	,	1063925,	 "Twi'lek",	CraftAttributeInfo.TwilekSkeletal,	CraftResource.TwilekSkeletal,	typeof( TwilekSkeletal ) ),		
			new CraftResourceInfo( 0x877,	0x877,	4	,	14	,	3.10	,	100.1	,	140	,	26	,	37	,	16	,	1	,	1063930	,	1063931	,	1063929,	 "Xindi",	CraftAttributeInfo.XindiSkeletal,	CraftResource.XindiSkeletal,	typeof( XindiSkeletal ) ),		
			new CraftResourceInfo( 0xB01,	0xB01,	4	,	14	,	3.10	,	100.1	,	140	,	26	,	37	,	16	,	1	,	1063934	,	1063935	,	1063933,	 "Zabrak",	CraftAttributeInfo.ZabrakSkeletal,	CraftResource.ZabrakSkeletal,	typeof( ZabrakSkeletal ) )		
			};																														

		private static Hashtable m_TypeTable;

		public static void RegisterType( Type resourceType, CraftResource resource )
		{
			if ( m_TypeTable == null )
				m_TypeTable = new Hashtable();

			m_TypeTable[resourceType] = resource;
		}

		public static CraftResource GetFromType( Type resourceType )
		{
			if ( m_TypeTable == null )
				return CraftResource.None;

			object obj = m_TypeTable[resourceType];

			if ( !(obj is CraftResource) )
				return CraftResource.None;

			return (CraftResource)obj;
		}

		private static CraftResourceInfo[] GetResourceList( CraftResourceType resourceType )
		{
			switch ( resourceType )
			{
				case CraftResourceType.Metal: return m_MetalInfo;
				case CraftResourceType.Leather: return m_LeatherInfo;
				case CraftResourceType.Scales: return m_ScaleInfo;
				case CraftResourceType.Wood: return m_WoodInfo;
				case CraftResourceType.Block: return m_BlockInfo;
				case CraftResourceType.Skin: return m_SkinInfo;
				case CraftResourceType.Special: return m_SpecialInfo;
				case CraftResourceType.Skeletal: return m_SkeletalInfo;
				case CraftResourceType.Fabric: return m_FabricInfo;
			}
			
			return null;
		}

		public static CraftResourceInfo GetInfo( CraftResource resource )
		{
			CraftResourceInfo[] list = GetResourceList( GetType( resource ) );

			if ( list != null )
			{
				int index = GetIndex( resource );

				if ( index >= 0 && index < list.Length )
					return list[index];
			}

			return null;
		}

		public static CraftResource GetRandomNonBasicResource( CraftResourceType resourceType )
		{
			CraftResourceInfo[] list = GetResourceList( resourceType );
			if ( list == null ) return CraftResource.None;

			var basicType = GetStart( resourceType );

			return Utility.Random( list.Where(x => !IsVeryBasic(x.Resource) ).ToList() ).Resource;
		}

		public static bool IsVeryBasic( CraftResource resource )
		{
			switch ( resource )
			{
				case CraftResource.Iron:
				case CraftResource.RegularLeather:
				case CraftResource.RegularWood:
				case CraftResource.Fabric:
				case CraftResource.BrittleSkeletal:
				case CraftResource.RedScales:
				// case CraftResource.AmethystBlock:
				// case CraftResource.DemonSkin:
				// case CraftResource.SpectralSpec:
					return true;
			}

			return false;
		}

		public static CraftResourceType GetType( CraftResource resource )
		{
			if ( resource >= CraftResource.Iron && resource <= CraftResource.Xonolite )
				return CraftResourceType.Metal;

			if ( resource >= CraftResource.RegularLeather && resource <= CraftResource.Thermoweave )
				return CraftResourceType.Leather;

			if ( resource >= CraftResource.SpectralSpec && resource <= CraftResource.TurtleSpec )
				return CraftResourceType.Special;

			if ( resource >= CraftResource.RedScales && resource <= CraftResource.KraytScales )
				return CraftResourceType.Scales;

			if ( resource >= CraftResource.RegularWood && resource <= CraftResource.VeshokTree )
				return CraftResourceType.Wood;

			if ( resource >= CraftResource.AmethystBlock && resource <= CraftResource.CaddelliteBlock )
				return CraftResourceType.Block;

			if ( resource >= CraftResource.DemonSkin && resource <= CraftResource.DeadSkin )
				return CraftResourceType.Skin;

			if ( resource >= CraftResource.BrittleSkeletal && resource <= CraftResource.ZabrakSkeletal )
				return CraftResourceType.Skeletal;

			if ( resource >= CraftResource.Fabric && resource <= CraftResource.FiendishFabric )
				return CraftResourceType.Fabric;

			return CraftResourceType.None;
		}

		public static Density GetDensity( Item item )
		{
			var resourceType = GetType( item.Resource );

			if ( resourceType == CraftResourceType.Special )
			{
				if ( item.Resource == CraftResource.GildedSpec )
					return Density.Superior;
					
				return Density.Ultimate;
			}

			if ( resourceType == CraftResourceType.Fabric || ( item is BaseArmor && ((BaseArmor)item).MaterialType == ArmorMaterialType.Cloth ) )
				return Density.Weak;
			else if ( resourceType == CraftResourceType.Leather || ( item is BaseArmor && ((BaseArmor)item).MaterialType == ArmorMaterialType.Leather ) || ( item is BaseArmor && ((BaseArmor)item).MaterialType == ArmorMaterialType.Studded ) )
				return Density.Regular;
			else if ( resourceType == CraftResourceType.Skin )
				return Density.Regular;
			else if ( resourceType == CraftResourceType.Wood )
				return Density.Great;
			else if ( resourceType == CraftResourceType.Skeletal || ( item is BaseArmor && ((BaseArmor)item).MaterialType == ArmorMaterialType.Bone ) )
				return Density.Great;
			else if ( resourceType == CraftResourceType.Scales || ( item is BaseArmor && ((BaseArmor)item).MaterialType == ArmorMaterialType.Scaled ) )
				return Density.Greater;
			else if ( item is BaseArmor && ((BaseArmor)item).MaterialType == ArmorMaterialType.Plate )
			{
				if ( resourceType == CraftResourceType.Metal )
					return Density.Superior;
				else if ( resourceType == CraftResourceType.Block )
					return Density.Ultimate;
			}
			else if ( item is BaseArmor && ( ((BaseArmor)item).MaterialType == ArmorMaterialType.Chainmail || ((BaseArmor)item).MaterialType == ArmorMaterialType.Ringmail ) )
			{
				if ( resourceType == CraftResourceType.Metal )
					return Density.Greater;
				else if ( resourceType == CraftResourceType.Block )
					return Density.Superior;
			}
			else if ( resourceType == CraftResourceType.Metal )
				return Density.Greater;
			else if ( resourceType == CraftResourceType.Block )
				return Density.Superior;

			return Density.None;
		}

		public static Item GetRandomItemWithDurability( Mobile defender )
		{
			int subset;
			int attempt = 0;

			Item item = null;
			while ( attempt < 5 )
			{
				attempt++;
				double positionChance = Utility.RandomDouble();

				if( positionChance < 0.07 ) item = GetItemWithDurability( defender, Layer.Neck );
				if ( item != null)  break;

				if( positionChance < 0.14 ) item = GetItemWithDurability( defender, Layer.Gloves );
				if ( item != null ) break;

				if( positionChance < 0.21 ) item = GetItemWithDurability( defender, Layer.Arms );
				if ( item != null ) break;

				if( positionChance < 0.35 ) item = GetItemWithDurability( defender, Layer.Helm );
				if ( item != null ) break;

				if( positionChance < 0.49 )
				{
					subset = Utility.Random( 4 );
					if( subset == 0 ) item = GetItemWithDurability( defender, Layer.Pants );
					else if( subset == 1 ) item = GetItemWithDurability( defender, Layer.Waist );
					else if( subset == 2 ) item = GetItemWithDurability( defender, Layer.OuterLegs );
					else if( subset == 3 ) item = GetItemWithDurability( defender, Layer.InnerLegs );
					if ( item != null ) break;
				}

				if( positionChance < 0.56 ) item = GetItemWithDurability( defender, Layer.Shoes );
				if ( item != null ) break;

				if( positionChance < 0.63 ) item = GetItemWithDurability( defender, Layer.Cloak );
				if ( item != null ) break;

				if( positionChance < 0.70 ) item = GetItemWithDurability( defender, Layer.OuterTorso );
				if ( item != null ) break;

				if( positionChance < 0.14 ) item = GetItemWithDurability( defender, Layer.Gloves );
				if ( item != null ) break;

				subset = Utility.Random( 3 );
				if( subset == 0 ) item = GetItemWithDurability( defender, Layer.InnerTorso );
				else if( subset == 1 ) item = GetItemWithDurability( defender, Layer.MiddleTorso );
				else if( subset == 2 ) item = GetItemWithDurability( defender, Layer.Shirt );
				if ( item != null ) break;
			}

			return item;
		}

		private static Item GetItemWithDurability( Mobile mobile, Layer layer )
		{
			if (mobile == null) return null;

			Item item = mobile.FindItemOnLayer( layer );
			if (item == null || item.Density == Density.None) return null;

			return item;
		}

		public static string GetTradeItemName( CraftResource resource, bool sub, bool sub2 )
		{
			if ( resource >= CraftResource.Iron && resource <= CraftResource.Dwarven && sub && sub2 )
				return "granite";
			else if ( resource >= CraftResource.Iron && resource <= CraftResource.Dwarven && sub )
				return "ore";
			else if ( resource >= CraftResource.Iron && resource <= CraftResource.Dwarven )
				return "ingot";
			else if ( resource >= CraftResource.Agrinium && resource <= CraftResource.Xonolite )
				return "metal";
			else if ( resource >= CraftResource.AmethystBlock && resource <= CraftResource.CaddelliteBlock && sub )
				return "stone";
			else if ( resource >= CraftResource.AmethystBlock && resource <= CraftResource.CaddelliteBlock )
				return "block";
			else if ( resource >= CraftResource.RegularLeather && resource <= CraftResource.AlienLeather && sub )
				return "hide";
			else if ( resource >= CraftResource.RegularLeather && resource <= CraftResource.AlienLeather )
				return "leather";
			else if ( resource >= CraftResource.DemonSkin && resource <= CraftResource.DeadSkin )
				return "skin";
			else if ( resource >= CraftResource.Adesote && resource <= CraftResource.Thermoweave )
				return "material";
			else if ( resource >= CraftResource.RedScales && resource <= CraftResource.KraytScales )
				return "scale";
			else if ( resource >= CraftResource.RegularWood && resource <= CraftResource.ElvenTree && sub )
				return "log";
			else if ( resource >= CraftResource.RegularWood && resource <= CraftResource.ElvenTree )
				return "board";
			else if ( resource >= CraftResource.BorlTree && resource <= CraftResource.VeshokTree )
				return "timber";
			else if ( resource >= CraftResource.SpectralSpec && resource <= CraftResource.TurtleSpec )
				return "rune";
			else if ( resource >= CraftResource.BrittleSkeletal && resource <= CraftResource.ZabrakSkeletal )
				return "bone";
			else if ( resource >= CraftResource.Fabric && resource <= CraftResource.FiendishFabric )
				return "cloth";

			return null;
		}

		public static string GetTradeItemFullName( Item item, CraftResource resource, bool sub, bool sub2, string name )
		{
			string material = (CraftResources.GetName( resource )).ToLower();

			if ( Item.IsStandardResource( resource ) )
				material = "";

			string sufx = GetTradeItemName( resource, sub, sub2 );
				if ( name != null )
					sufx = name;

			if ( sufx != null && material != "" )
				material = material + " " + sufx;
			else if ( sufx != null )
				material = sufx;

			return material;
		}

		public static string GetResourceName( CraftResource resource )
		{
			return (CraftResources.GetName( resource )).ToLower();
		}

		public static CraftResource GetStart( CraftResourceType resourceType )
		{
			switch ( resourceType )
			{
				case CraftResourceType.Metal: return CraftResource.Iron;
				case CraftResourceType.Leather: return CraftResource.RegularLeather;
				case CraftResourceType.Scales: return CraftResource.RedScales;
				case CraftResourceType.Wood: return CraftResource.RegularWood;
				case CraftResourceType.Block: return CraftResource.AmethystBlock;
				case CraftResourceType.Skin: return CraftResource.DemonSkin;
				case CraftResourceType.Special: return CraftResource.SpectralSpec;
				case CraftResourceType.Skeletal: return CraftResource.BrittleSkeletal;
				case CraftResourceType.Fabric: return CraftResource.Fabric;
			}

			return CraftResource.None;
		}

		public static CraftResource GetStart( CraftResource resource )
		{
			return GetStart( GetType( resource ) );
		}

		public static int GetIndex( CraftResource resource )
		{
			CraftResource start = GetStart( resource );

			if ( start == CraftResource.None )
				return 0;

			return (int)(resource - start);
		}

		public static int GetClilocCraftName( CraftResource resource ) // RETURNS LIKE: GOLD (100)
		{
			CraftResourceInfo info = GetInfo( resource );

			if ( resource == CraftResource.None )
				return 0;

			return ( info == null ? 0 : info.CraftText );
		}

		public static int GetClilocMaterialName( CraftResource resource ) // RETURNS LIKE: Gold Ingots
		{
			CraftResourceInfo info = GetInfo( resource );

			if ( resource == CraftResource.None )
				return 0;

			return ( info == null ? 0 : info.MaterialText );
		}

		public static int GetClilocLowerCaseName( CraftResource resource ) // RETURNS LIKE: gold
		{
			CraftResourceInfo info = GetInfo( resource );

			// DO NOT RETURN A VALUE FOR REGULAR IRON, WOOD, OR LEATHER ... AND NONE.
			if ( resource == CraftResource.None || resource == CraftResource.Fabric || resource == CraftResource.Iron || resource == CraftResource.RegularLeather || resource == CraftResource.RegularWood || resource == CraftResource.BrittleSkeletal )
				return 0;

			return ( info == null ? 0 : info.LowCaseText );
		}

		public static int GetHue( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			return ( info == null ? 0 : info.Hue );
		}

		public static int GetClr( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			return ( info == null ? 0 : info.Clr );
		}

		public static string GetName( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			return ( info == null ? String.Empty : info.Name );
		}

		public static string GetPrefix( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			if ( info != null && info.Name != "Iron" && info.Name != "Normal" && info.Name != "Brittle" )
				return "" + info.Name + " ";

			return "";
		}

		public static int GetDmg( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			return ( info == null ? 0 : info.Dmg );
		}

		public static int GetArm( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			return ( info == null ? 0 : info.Arm );
		}

		public static double GetGold( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			return ( info == null ? 0.0 : info.Gold );
		}

		public static double GetSkill( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			return ( info == null ? 0.0 : info.Skill );
		}

		public static int GetUses( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			return ( info == null ? 0 : info.Uses );
		}

		public static int GetWeight( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			return ( info == null ? 0 : info.Weight );
		}

		public static int GetBonus( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			return ( info == null ? 0 : info.Bonus );
		}

		public static int GetXtra( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			return ( info == null ? 0 : info.Xtra );
		}

		public static int GetWeaponArmor( CraftResource resource )
		{
			CraftResourceInfo info = GetInfo( resource );

			return ( info == null ? 0 : info.WepArmor );
		}

		public static void GetAosMods( CraftResource resource, Item item, bool reduce )
		{
			switch(resource)
			{
				case CraftResource.Iron:
				case CraftResource.DullCopper:
				case CraftResource.ShadowIron:
				case CraftResource.Copper:
				case CraftResource.Bronze:
				case CraftResource.Gold:
				case CraftResource.Agapite:
				case CraftResource.Verite:
				case CraftResource.Valorite:
				case CraftResource.Nepturite:
				case CraftResource.Obsidian:
				case CraftResource.Steel:
				case CraftResource.Brass:
				case CraftResource.Mithril:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo());
					break;
				case CraftResource.Xormite:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						// AosAttribute_RegenMana = 3,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_ReflectPhysical = 2,
						// AosAttribute_EnhancePotions = 2,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_SelfRepair = 1,
						// AosWeaponAttribute_HitEnergyArea = 15,
						// AosArmorAttribute_SelfRepair = 1
					}); break;
				case CraftResource.Dwarven:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.OrcSlaying,
						Slayer = SlayerName.OgreTrashing,
						Skill5 = 99,
						Skill5Val = 2,
						// AosAttribute_DefendChance = 3,
						// AosWeaponAttribute_SelfRepair = 1,
						// AosArmorAttribute_SelfRepair = 1
					}); break;
				case CraftResource.Agrinium:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.EarthShatter,
						Skill5 = 99,
						Skill5Val = 2,
						// AosAttribute_DefendChance = 3,
						// AosAttribute_AttackChance = 3,
						// AosAttribute_WeaponDamage = 3,
						// AosWeaponAttribute_SelfRepair = 1,
						// AosWeaponAttribute_HitPhysicalArea = 10,
						// AosArmorAttribute_SelfRepair = 1
					}); break;
				case CraftResource.Beskar:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.OgreTrashing,
						Skill5 = 45,
						Skill5Val = 1,
						Skill4 = 43,
						Skill4Val = 1,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_ReflectPhysical = 2,
						// AosWeaponAttribute_HitLowerAttack = 10,
						// AosWeaponAttribute_HitLowerDefend = 10,
						// AosWeaponAttribute_HitHarm = 5,
						// AosWeaponAttribute_HitPhysicalArea = 10
					}); break;
				case CraftResource.Carbonite:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.TrollSlaughter,
						Skill5 = 2,
						Skill5Val = 1,
						Skill4 = 23,
						Skill4Val = 1,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_DefendChance = 3,
						// AosWeaponAttribute_HitLeechHits = 15,
						// AosWeaponAttribute_HitLeechStam = 15,
						// AosWeaponAttribute_HitDispel = 20,
						// AosWeaponAttribute_HitPhysicalArea = 10
					}); break;
				case CraftResource.Cortosis:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.WizardSlayer,
						Skill5 = 1,
						Skill5Val = 2,
						// AosAttribute_DefendChance = 3,
						// AosAttribute_AttackChance = 3,
						// AosAttribute_WeaponSpeed = 3,
						// AosAttribute_EnhancePotions = 2,
						// AosWeaponAttribute_HitEnergyArea = 10
					}); break;
				case CraftResource.Durasteel:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.GolemDestruction,
						Skill5 = 99,
						Skill5Val = 1,
						Skill4 = 48,
						Skill4Val = 1,
						// AosAttribute_DefendChance = 3,
						// AosAttribute_AttackChance = 3,
						// AosAttribute_WeaponDamage = 3,
						// AosAttribute_ReflectPhysical = 3,
						// AosWeaponAttribute_SelfRepair = 1,
						// AosWeaponAttribute_HitLowerAttack = 15,
						// AosWeaponAttribute_HitLowerDefend = 15,
						// AosWeaponAttribute_HitPhysicalArea = 10,
						// AosArmorAttribute_SelfRepair = 1
					}); break;
				case CraftResource.Durite:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.GiantKiller,
						Skill5 = 6,
						Skill5Val = 1,
						Skill4 = 8,
						Skill4Val = 1,
						// AosWeaponAttribute_SelfRepair = 1,
						// AosWeaponAttribute_HitLeechHits = 10,
						// AosWeaponAttribute_HitLeechStam = 10,
						// AosWeaponAttribute_HitPhysicalArea = 10,
						// AosArmorAttribute_SelfRepair = 1
					}); break;
				case CraftResource.Farium:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.WaterDissipation,
						Skill5 = 15,
						Skill5Val = 1,
						Skill4 = 42,
						Skill4Val = 1,
						// AosAttribute_BonusDex = 2,
						// AosWeaponAttribute_SelfRepair = 1,
						// AosWeaponAttribute_HitLowerAttack = 10,
						// AosWeaponAttribute_HitLowerDefend = 10,
						// AosWeaponAttribute_HitFireArea = 10
					}); break;
				case CraftResource.Laminasteel:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.WeedRuin,
						Skill5 = 3,
						Skill5Val = 1,
						Skill4 = 4,
						Skill4Val = 1,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_RegenStam = 1,
						// AosAttribute_RegenMana = 1,
						AosWeaponAttribute_HitPoisonArea = 10
					}); break;
				case CraftResource.Neuranium:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.DragonSlaying,
						Skill5 = 31,
						Skill5Val = 1,
						Skill4 = 33,
						Skill4Val = 1,
						// AosAttribute_SpellDamage = 1,
						// AosAttribute_CastSpeed = 1,
						// AosAttribute_LowerManaCost = 2,
						// AosAttribute_LowerRegCost = 3,
						// AosWeaponAttribute_HitFireArea = 10,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.Phrik:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Fey,
						Skill5 = 55,
						Skill5Val = 1,
						Skill4 = 21,
						Skill4Val = 1,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_BonusHits = 2,
						// AosAttribute_BonusStam = 2,
						// AosAttribute_WeaponSpeed = 3,
						// AosAttribute_SpellDamage = 1,
						// AosAttribute_CastRecovery = 1,
						// AosAttribute_LowerManaCost = 2,
						// AosAttribute_LowerRegCost = 3,
						// AosWeaponAttribute_HitEnergyArea = 10,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.Promethium:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.LizardmanSlaughter,
						Skill5 = 40,
						Skill5Val = 2,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_BonusHits = 3,
						// AosAttribute_WeaponDamage = 2,
						// AosWeaponAttribute_HitPoisonArea = 10
					}); break;
				case CraftResource.Quadranium:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.AvianHunter,
						Skill5 = 28,
						Skill5Val = 1,
						Skill4 = 27,
						Skill4Val = 1,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_BonusStam = 3,
						// AosWeaponAttribute_SelfRepair = 1,
						// AosWeaponAttribute_HitEnergyArea = 10
					}); break;
				case CraftResource.Songsteel:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Vacuum,
						Skill5 = 35,
						Skill5Val = 1,
						Skill4 = 39,
						Skill4Val = 1,
						Skill3 = 16,
						Skill3Val = 1,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_BonusStam = 3,
						// AosAttribute_WeaponSpeed = 3,
						// AosWeaponAttribute_HitEnergyArea = 10
					}); break;
				case CraftResource.Titanium:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.ArachnidDoom,
						Skill5 = 99,
						Skill5Val = 1,
						Skill4 = 48,
						Skill4Val = 1,
						// AosAttribute_DefendChance = 3,
						// AosAttribute_AttackChance = 3,
						// AosAttribute_WeaponDamage = 3,
						// AosWeaponAttribute_SelfRepair = 1,
						// AosWeaponAttribute_HitPhysicalArea = 10,
						// AosArmorAttribute_SelfRepair = 1
					}); break;
				case CraftResource.Trimantium:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.NeptunesBane,
						Skill5 = 19,
						Skill5Val = 2,
						// AosAttribute_BonusHits = 3,
						// AosAttribute_BonusStam = 3,
						// AosWeaponAttribute_HitColdArea = 10
					}); break;
				case CraftResource.Xonolite:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.SlimyScourge,
						Skill5 = 36,
						Skill5Val = 1,
						Skill4 = 44,
						Skill4Val = 1,
						// AosAttribute_SpellDamage = 1,
						// AosAttribute_CastRecovery = 1,
						// AosAttribute_LowerManaCost = 2,
						// AosAttribute_LowerRegCost = 3,
						// AosWeaponAttribute_HitFireArea = 10,
						// AosArmorAttribute_MageArmor = 1
					}); break;

				case CraftResource.RedScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_RegenHits = 2,
					}); break;
				case CraftResource.YellowScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_AttackChance = 2,
					}); break;
				case CraftResource.BlackScales: // Hiding + Stealth + Tactics
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 25,
						Skill4Val = 2,
						Skill3 = 46,
						Skill3Val = 2,
						AosAttribute_NightSight = 1,
					}); break;
				case CraftResource.GreenScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_RegenStam = 1,
						AosAttribute_BonusDex = 3,
					}); break;
				case CraftResource.WhiteScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.BlueScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_RegenMana = 2,
					}); break;
				case CraftResource.DinosaurScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{ // Vet, Herding, Taming, Tactics
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 4,
						Skill4Val = 1,
						Skill3 = 24,
						Skill3Val = 1,
						Skill2 = 53,
						Skill2Val = 2,
						AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.MetallicScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_BonusStr = 2,
						AosAttribute_BonusDex = 2,
						AosAttribute_BonusInt = 2,
					}); break;
				case CraftResource.BrazenScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_WeaponSpeed = 5,
					}); break;
				case CraftResource.UmberScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_CastSpeed = 1,
						AosArmorAttribute_MageArmor = 1,
					}); break;
				case CraftResource.VioletScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_SpellDamage = 2,
						AosArmorAttribute_MageArmor = 1,
					}); break;
				case CraftResource.PlatinumScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_CastRecovery = 1,
						AosArmorAttribute_MageArmor = 1,
					}); break;
				case CraftResource.CadalyteScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_WeaponDamage = 5,
						AosAttribute_WeaponSpeed = 5,
					}); break;
				case CraftResource.GornScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_WeaponDamage = 5,
						AosAttribute_BonusStr = 2,
						AosAttribute_BonusHits = 3,
					}); break;
				case CraftResource.TrandoshanScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_BonusDex = 2,
						AosAttribute_WeaponSpeed = 5,
					}); break;
				case CraftResource.SilurianScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_LowerManaCost = 3,
						AosAttribute_SpellDamage = 3,
						AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.KraytScales:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_WeaponDamage = 3,
						AosAttribute_WeaponSpeed = 5,
						AosArmorAttribute_MageArmor = 1
					}); break;

				case CraftResource.SpectralSpec:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Skill5 = 99,
						Skill5Val = 5,
						Skill4 = 36,
						Skill4Val = 5,
						Skill3 = 19,
						Skill3Val = 5,
						// AosAttribute_DefendChance = 3,
						// AosAttribute_AttackChance = 3,
						// AosAttribute_BonusStam = 2,
						// AosAttribute_ReflectPhysical = 3,
						// AosAttribute_EnhancePotions = 2,
						// AosWeaponAttribute_SelfRepair = 5,
						// AosWeaponAttribute_HitLeechHits = 10,
						// AosWeaponAttribute_HitColdArea = 20,
						// AosArmorAttribute_SelfRepair = 5
					}); break;
				case CraftResource.DreadSpec:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Skill5 = 99,
						Skill5Val = 5,
						Skill4 = 48,
						Skill4Val = 5,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_RegenStam = 1,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_AttackChance = 5,
						// AosAttribute_WeaponDamage = 7,
						// AosAttribute_ReflectPhysical = 2,
						// AosAttribute_NightSight = 1
					}); break;
				case CraftResource.GhoulishSpec:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Repond,
						Skill5 = 99,
						Skill5Val = 15,
						// AosAttribute_RegenHits = 2,
						// AosAttribute_RegenStam = 2,
						// AosAttribute_DefendChance = 2,
						// AosAttribute_WeaponDamage = 7,
						// AosAttribute_ReflectPhysical = 2,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_HitLeechHits = 10,
						// AosWeaponAttribute_HitLeechStam = 10
					}); break;
				case CraftResource.WyrmSpec:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.DragonSlaying,
						Skill5 = 99,
						Skill5Val = 15,
						// AosAttribute_DefendChance = 3,
						// AosAttribute_AttackChance = 7,
						// AosAttribute_WeaponDamage = 8,
						// AosAttribute_WeaponSpeed = 5,
						// AosWeaponAttribute_HitLowerAttack = 50,
						// AosWeaponAttribute_HitLowerDefend = 50
					}); break;
				case CraftResource.HolySpec:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Silver,
						Slayer = SlayerName.Exorcism,
						Skill5 = 99,
						Skill5Val = 20,
						// AosAttribute_RegenHits = 2,
						// AosAttribute_AttackChance = 9,
						// AosAttribute_WeaponDamage = 9,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_SelfRepair = 2,
						// AosWeaponAttribute_HitLeechHits = 5,
						// AosArmorAttribute_SelfRepair = 2
					}); break;
				case CraftResource.BloodlessSpec:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Repond,
						Skill5 = 36,
						Skill5Val = 5,
						Skill4 = 22,
						Skill4Val = 5,
						Skill3 = 44,
						Skill3Val = 5,
						// AosAttribute_BonusMana = 3,
						// AosAttribute_WeaponDamage = 8,
						// AosAttribute_LowerManaCost = 5,
						// AosAttribute_LowerRegCost = 5,
						// AosAttribute_SpellChanneling = 1,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_HitPoisonArea = 50,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.GildedSpec:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.AvianHunter,
						Slayer = SlayerName.AnimalHunter,
						Skill5 = 52, // Tracking
						Skill5Val = 5,
					}); break;
				case CraftResource.DemilichSpec:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Repond,
						Skill5 = 99,
						Skill5Val = 10,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_AttackChance = 9,
						// AosAttribute_SpellDamage = 2,
						// AosWeaponAttribute_HitLeechHits = 5,
						// AosWeaponAttribute_HitLowerDefend = 50,
						// AosWeaponAttribute_HitDispel = 20,
						// AosWeaponAttribute_HitPhysicalArea = 25
					}); break;
				case CraftResource.WintrySpec:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.FlameDousing,
						Skill5 = 99,
						Skill5Val = 5,
						// AosAttribute_WeaponDamage = 8,
						// AosAttribute_ReflectPhysical = 3,
						// AosAttribute_NightSight = 1
					}); break;
				case CraftResource.FireSpec: ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_BonusStr = 2
					}); break;
				case CraftResource.ColdSpec: ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_BonusInt = 2
					}); break;
				case CraftResource.PoisSpec: ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_BonusHits = 2,
						AosAttribute_BonusStam = 2
					}); break;
				case CraftResource.EngySpec: ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_BonusDex = 2
					}); break;
				case CraftResource.ExodusSpec:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.GolemDestruction,
						Slayer = SlayerName.BalronDamnation,
						Skill5 = 99,
						Skill5Val = 15,
						Skill4 = 32,
						Skill4Val = 10,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_RegenStam = 1,
						// AosAttribute_RegenMana = 1,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_AttackChance = 5,
						// AosAttribute_BonusStr = 2,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_BonusHits = 4,
						// AosAttribute_BonusStam = 3,
						// AosAttribute_WeaponDamage = 9,
						// AosAttribute_WeaponSpeed = 5,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_SelfRepair = 3,
						// AosWeaponAttribute_HitLightning = 20,
						// AosWeaponAttribute_HitDispel = 20,
						// AosWeaponAttribute_HitEnergyArea = 25,
						// AosArmorAttribute_SelfRepair = 3,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.TurtleSpec:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.NeptunesBane,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_ReflectPhysical = 3,
						// AosWeaponAttribute_HitFireball = 20,
						// AosWeaponAttribute_HitFireArea = 50,
						// AosArmorAttribute_SelfRepair = 5
					}); break;

				case CraftResource.RegularLeather:
				case CraftResource.HornedLeather:
				case CraftResource.BarbedLeather:
				case CraftResource.NecroticLeather:
				case CraftResource.VolcanicLeather:
				case CraftResource.FrozenLeather:
				case CraftResource.SpinedLeather:
				case CraftResource.GoliathLeather:
				case CraftResource.DraconicLeather:
				case CraftResource.HellishLeather:
				case CraftResource.DinosaurLeather:
				case CraftResource.AlienLeather:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo()); break;
				case CraftResource.Adesote:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Fey,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 17,
						Skill4Val = 2,
						// AosAttribute_RegenMana = 1,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_SpellChanneling = 1,
						// AosWeaponAttribute_HitEnergyArea = 5,
						// AosWeaponAttribute_HitPhysicalArea = 5,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.Biomesh:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.SpidersDeath,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 21,
						Skill4Val = 2,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_ReflectPhysical = 3,
						// AosAttribute_EnhancePotions = 7,
						// AosAttribute_SpellChanneling = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.Cerlin:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Vacuum,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 13,
						Skill4Val = 2,
						// AosAttribute_BonusHits = 2,
						// AosAttribute_BonusStam = 2,
						// AosAttribute_BonusMana = 2,
						// AosAttribute_SpellChanneling = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.Durafiber:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.OrcSlaying,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 23,
						Skill4Val = 2,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_CastSpeed = 1,
						// AosAttribute_LowerManaCost = 3,
						// AosAttribute_SpellChanneling = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.Flexicris:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.WizardSlayer,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 27,
						Skill4Val = 2,
						// AosAttribute_RegenStam = 1,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_CastRecovery = 5,
						// AosAttribute_LowerRegCost = 3,
						// AosAttribute_SpellChanneling = 1,
						// AosAttribute_NightSight = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.Hypercloth:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.SlimyScourge,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 2,
						Skill4Val = 2,
						// AosAttribute_BonusMana = 3,
						// AosAttribute_WeaponDamage = 5,
						// AosAttribute_SpellChanneling = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.Nylar:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.DaemonDismissal,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 50,
						Skill4Val = 2,
						// AosAttribute_BonusHits = 3,
						// AosAttribute_WeaponSpeed = 5,
						// AosAttribute_SpellChanneling = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.Nylonite:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.OgreTrashing,
						Skill5 = 25,
						Skill5Val = 2,
						Skill4 = 46,
						Skill4Val = 2,
						// AosAttribute_RegenStam = 1,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_SpellChanneling = 1,
						// AosAttribute_NightSight = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.Polyfiber:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.TrollSlaughter,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 52,
						Skill4Val = 2,
						// AosAttribute_RegenMana = 1,
						// AosAttribute_BonusStam = 3,
						// AosAttribute_SpellChanneling = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.Syncloth:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.EarthShatter,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 6,
						Skill4Val = 2,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_SpellDamage = 3,
						// AosAttribute_CastRecovery = 1,
						// AosAttribute_CastSpeed = 1,
						// AosAttribute_EnhancePotions = 5,
						// AosAttribute_SpellChanneling = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.Thermoweave:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.GolemDestruction,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 15,
						Skill4Val = 2,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_ReflectPhysical = 3,
						// AosAttribute_SpellChanneling = 1,
						// AosAttribute_NightSight = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;

				case CraftResource.RegularWood:
				case CraftResource.AshTree:
				case CraftResource.CherryTree:
				case CraftResource.EbonyTree:
				case CraftResource.GoldenOakTree:
				case CraftResource.HickoryTree:
				case CraftResource.MahoganyTree:
				case CraftResource.OakTree:
				case CraftResource.PineTree:
				case CraftResource.GhostTree:
				case CraftResource.RosewoodTree:
				case CraftResource.WalnutTree:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo()); break;
				case CraftResource.PetrifiedTree:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						AosAttribute_DefendChance = 3
					}); break;
				case CraftResource.DriftwoodTree:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Skill5 = 19,
						Skill5Val = 2
					}); break;
				case CraftResource.ElvenTree:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 52,
						Skill4Val = 2,
						Skill3 = 10,
						Skill3Val = 2,
						// AosAttribute_RegenMana = 1,
						// AosAttribute_AttackChance = 5,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_WeaponDamage = 7,
						// AosAttribute_WeaponSpeed = 7,
						// AosAttribute_EnhancePotions = 7,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_SelfRepair = 2,
						// AosWeaponAttribute_HitLowerDefend = 20,
						// AosArmorAttribute_SelfRepair = 2,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.BorlTree:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.EarthShatter,
						Skill5 = 15,
						Skill5Val = 2,
						// AosAttribute_RegenStam = 1,
						// AosAttribute_WeaponDamage = 5,
						// AosAttribute_EnhancePotions = 5,
						// AosAttribute_SpellChanneling = 1,
						// AosWeaponAttribute_HitFireball = 20
					}); break;
				case CraftResource.CosianTree:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.WeedRuin,
						Skill5 = 23,
						Skill5Val = 2,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_WeaponDamage = 5,
						// AosAttribute_SpellChanneling = 1,
						// AosWeaponAttribute_HitPoisonArea = 20
					}); break;
				case CraftResource.GreelTree:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.GargoylesFoe,
						Skill5 = 99,
						Skill5Val = 2,
						// AosAttribute_BonusHits = 3,
						// AosAttribute_WeaponDamage = 5,
						// AosAttribute_EnhancePotions = 7,
						// AosAttribute_SpellChanneling = 1,
						// AosWeaponAttribute_HitEnergyArea = 20
					}); break;
				case CraftResource.JaporTree:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Ophidian,
						Skill5 = 19,
						Skill5Val = 2,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_WeaponDamage = 6,
						// AosAttribute_WeaponSpeed = 5,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_HitColdArea = 20
					}); break;
				case CraftResource.KyshyyykTree:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.ScorpionsBane,
						Skill5 = 99,
						Skill5Val = 2,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_BonusHits = 3,
						// AosAttribute_WeaponDamage = 7,
						// AosAttribute_WeaponSpeed = 8,
						// AosAttribute_EnhancePotions = 10,
						// AosWeaponAttribute_HitPhysicalArea = 20
					}); break;
				case CraftResource.LaroonTree:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Terathan,
						Skill5 = 44,
						Skill5Val = 2,
						// AosAttribute_BonusMana = 3,
						// AosAttribute_WeaponDamage = 8,
						// AosAttribute_WeaponSpeed = 11,
						// AosAttribute_SpellDamage = 2,
						// AosAttribute_CastRecovery = 1,
						// AosAttribute_CastSpeed = 1,
						// AosAttribute_LowerManaCost = 3,
						// AosAttribute_LowerRegCost = 3,
						// AosWeaponAttribute_HitLightning = 20,
						// AosArmorAttribute_SelfRepair = 1
					}); break;
				case CraftResource.TeejTree:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.SnakesBane,
						Skill5 = 99,
						Skill5Val = 2,
						// AosAttribute_BonusHits = 2,
						// AosAttribute_BonusStam = 2,
						// AosAttribute_WeaponDamage = 9,
						// AosAttribute_WeaponSpeed = 12,
						// AosAttribute_ReflectPhysical = 3,
						// AosWeaponAttribute_HitLeechHits = 10,
						// AosWeaponAttribute_HitLeechStam = 10,
						// AosArmorAttribute_SelfRepair = 1
					}); break;
				case CraftResource.VeshokTree:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.SummerWind,
						Skill5 = 22,
						Skill5Val = 2,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_WeaponDamage = 10,
						// AosAttribute_WeaponSpeed = 13,
						// AosAttribute_EnhancePotions = 12,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_HitLeechStam = 10,
						// AosWeaponAttribute_HitLeechMana = 10,
						// AosArmorAttribute_SelfRepair = 2
					}); break;

				case CraftResource.Fabric:
				case CraftResource.FurryFabric:
				case CraftResource.WoolyFabric:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo()); break;
				case CraftResource.SilkFabric:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						// AosAttribute_BonusDex = 2,
						// AosAttribute_EnhancePotions = 2,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.HauntedFabric:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Skill5 = 44,
						Skill5Val = 2,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_EnhancePotions = 5,
						// AosAttribute_NightSight = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.ArcticFabric:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.FlameDousing,
						// AosAttribute_RegenStam = 1,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_EnhancePotions = 7,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.PyreFabric:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.WaterDissipation,
						// AosAttribute_RegenStam = 1,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_EnhancePotions = 7,
						// AosWeaponAttribute_HitFireball = 10,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.VenomousFabric:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.ElementalHealth,
						Skill5 = 40,
						Skill5Val = 2,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_EnhancePotions = 7,
						// AosWeaponAttribute_HitPoisonArea = 10,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.MysteriousFabric:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.WizardSlayer,
						Skill5 = 33,
						Skill5Val = 2,
						Skill4 = 32,
						Skill4Val = 2,
						// AosAttribute_RegenMana = 2,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_SpellDamage = 2,
						// AosAttribute_CastRecovery = 1,
						// AosAttribute_CastSpeed = 1,
						// AosAttribute_LowerRegCost = 3,
						// AosAttribute_ReflectPhysical = 2,
						// AosAttribute_EnhancePotions = 10,
						// AosAttribute_SpellChanneling = 1,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_HitLeechMana = 15,
						// AosWeaponAttribute_HitMagicArrow = 10,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.VileFabric:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Repond,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 36,
						Skill4Val = 2,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_RegenStam = 1,
						// AosAttribute_AttackChance = 8,
						// AosAttribute_BonusStr = 2,
						// AosAttribute_BonusStam = 3,
						// AosAttribute_WeaponDamage = 7,
						// AosAttribute_WeaponSpeed = 7,
						// AosAttribute_EnhancePotions = 10,
						// AosAttribute_SpellChanneling = 1,
						// AosWeaponAttribute_HitLeechHits = 15,
						// AosWeaponAttribute_HitHarm = 10,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.DivineFabric:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Silver,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 13,
						Skill4Val = 2,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_RegenMana = 1,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_AttackChance = 5,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_BonusHits = 3,
						// AosAttribute_ReflectPhysical = 3,
						// AosAttribute_EnhancePotions = 12,
						// AosAttribute_SpellChanneling = 1,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_SelfRepair = 2,
						// AosWeaponAttribute_HitLightning = 10,
						// AosArmorAttribute_SelfRepair = 2,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.FiendishFabric:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Exorcism,
						Skill5 = 99,
						Skill5Val = 4,
						Skill4 = 32,
						Skill4Val = 2,
						// AosAttribute_RegenStam = 1,
						// AosAttribute_RegenMana = 1,
						// AosAttribute_AttackChance = 5,
						// AosAttribute_BonusStr = 2,
						// AosAttribute_BonusInt = 2,
						// AosAttribute_BonusMana = 3,
						// AosAttribute_LowerManaCost = 3,
						// AosAttribute_EnhancePotions = 12,
						// AosAttribute_SpellChanneling = 1,
						// AosWeaponAttribute_HitLowerAttack = 15,
						// AosWeaponAttribute_HitFireball = 10,
						// AosWeaponAttribute_HitFireArea = 15,
						// AosArmorAttribute_MageArmor = 1
					}); break;

				case CraftResource.AmethystBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.GolemDestruction,
						Skill5 = 38,
						Skill5Val = 2,
						Skill4 = 48,
						Skill4Val = 2,
						// AosAttribute_DefendChance = 3,
						// AosAttribute_ReflectPhysical = 3,
						// AosWeaponAttribute_HitPhysicalArea = 80
					}); break;
				case CraftResource.EmeraldBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.WeedRuin,
						Skill5 = 40,
						Skill5Val = 2,
						// AosAttribute_RegenHits = 1,
						// AosAttribute_RegenStam = 1,
						// AosAttribute_RegenMana = 1,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_ReflectPhysical = 3
					}); break;
				case CraftResource.GarnetBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.ElementalHealth,
						// AosAttribute_RegenMana = 2,
						// AosAttribute_BonusInt = 2,
						// AosAttribute_SpellDamage = 2,
						// AosAttribute_CastRecovery = 1,
						// AosAttribute_CastSpeed = 1,
						// AosAttribute_LowerManaCost = 3,
						// AosAttribute_LowerRegCost = 3,
						// AosAttribute_SpellChanneling = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.IceBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.FlameDousing,
						// AosAttribute_RegenStam = 2,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_AttackChance = 5,
						// AosAttribute_ReflectPhysical = 2,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_HitColdArea = 25
					}); break;
				case CraftResource.JadeBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.ElementalHealth,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 9,
						Skill4Val = 2,
						// AosAttribute_RegenStam = 2,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_AttackChance = 5,
						// AosAttribute_BonusDex = 2,
						// AosAttribute_NightSight = 1
					}); break;
				case CraftResource.MarbleBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.EarthShatter,
						Slayer = SlayerName.GolemDestruction,
						Skill5 = 99,
						Skill5Val = 3,
						// AosAttribute_DefendChance = 7,
						// AosAttribute_BonusStr = 3,
						// AosAttribute_WeaponDamage = 8,
						// AosAttribute_ReflectPhysical = 2
					}); break;
				case CraftResource.OnyxBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.GiantKiller,
						Slayer = SlayerName.Repond,
						Skill5 = 36,
						Skill5Val = 2,
						// AosAttribute_RegenStam = 2,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_AttackChance = 5,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_BonusMana = 2,
						// AosAttribute_WeaponDamage = 7,
						// AosAttribute_SpellDamage = 2,
						// AosAttribute_CastRecovery = 1,
						// AosAttribute_CastSpeed = 1,
						// AosAttribute_LowerManaCost = 3,
						// AosAttribute_LowerRegCost = 3,
						// AosAttribute_SpellChanneling = 1,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_HitLeechMana = 5,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.QuartzBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.WaterDissipation,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_AttackChance = 8,
						// AosAttribute_BonusDex = 3,
						// AosAttribute_ReflectPhysical = 2
					}); break;
				case CraftResource.RubyBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.BloodDrinking,
						// AosAttribute_DefendChance = 7,
						// AosAttribute_AttackChance = 7,
						// AosAttribute_BonusStr = 2,
						// AosWeaponAttribute_HitFireball = 10,
						// AosWeaponAttribute_HitFireArea = 20
					}); break;
				case CraftResource.SapphireBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.AvianHunter,
						Skill5 = 17,
						Skill5Val = 1,
						Skill4 = 31,
						Skill4Val = 1,
						Skill3 = 32,
						Skill3Val = 3,
						Skill2 = 33,
						Skill2Val = 3,
						// AosAttribute_RegenMana = 2,
						// AosAttribute_BonusInt = 2,
						// AosAttribute_SpellDamage = 2,
						// AosAttribute_CastSpeed = 1,
						// AosAttribute_SpellChanneling = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.SilverBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Silver,
						Skill5 = 13,
						Skill5Val = 2,
						// AosAttribute_RegenHits = 2,
						// AosAttribute_RegenStam = 2,
						// AosAttribute_DefendChance = 7,
						// AosAttribute_AttackChance = 9,
						// AosAttribute_ReflectPhysical = 2,
						// AosAttribute_NightSight = 1
					}); break;
				case CraftResource.SpinelBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Fey,
						// AosAttribute_RegenMana = 2,
						// AosAttribute_WeaponDamage = 8,
						// AosAttribute_LowerManaCost = 3,
						// AosAttribute_LowerRegCost = 3,
						// AosAttribute_SpellChanneling = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.StarRubyBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.GargoylesFoe,
						Skill5 = 55,
						Skill5Val = 2,
						// AosAttribute_RegenHits = 2,
						// AosAttribute_DefendChance = 7,
						// AosAttribute_AttackChance = 8,
						// AosAttribute_BonusStr = 2,
						// AosAttribute_WeaponDamage = 8
					}); break;
				case CraftResource.TopazBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.ReptilianDeath,
						// AosAttribute_RegenHits = 2,
						// AosAttribute_RegenStam = 2,
						// AosAttribute_WeaponDamage = 7,
						// AosWeaponAttribute_SelfRepair = 2,
						// AosArmorAttribute_SelfRepair = 2
					}); break;
				case CraftResource.CaddelliteBlock:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.WizardSlayer,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 32,
						Skill4Val = 3,
						// AosAttribute_WeaponDamage = 9,
						// AosWeaponAttribute_SelfRepair = 2,
						// AosWeaponAttribute_HitLeechMana = 5,
						// AosWeaponAttribute_HitLightning = 10,
						// AosWeaponAttribute_HitEnergyArea = 30,
						// AosArmorAttribute_SelfRepair = 2
					}); break;

				case CraftResource.DemonSkin:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.DaemonDismissal,
						Skill5 = 32,
						Skill5Val = 2,
						// AosAttribute_RegenMana = 2,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_SpellDamage = 1,
						// AosAttribute_CastSpeed = 1,
						// AosAttribute_LowerManaCost = 3,
						// AosAttribute_LowerRegCost = 3,
						// AosAttribute_ReflectPhysical = 2,
						// AosAttribute_NightSight = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.DragonSkin:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.DragonSlaying,
						Skill5 = 99,
						Skill5Val = 2,
						// AosAttribute_RegenStam = 2,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_ReflectPhysical = 3,
						// AosWeaponAttribute_HitFireball = 20
					}); break;
				case CraftResource.NightmareSkin:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Skill5 = 99,
						Skill5Val = 2,
						// AosAttribute_BonusMana = 2,
						// AosAttribute_LowerManaCost = 2,
						// AosAttribute_SpellChanneling = 1,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_HitLowerAttack = 25,
						// AosWeaponAttribute_HitLowerDefend = 25,
						// AosWeaponAttribute_HitHarm = 20,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.SnakeSkin:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.SnakesBane,
						Skill5 = 40,
						Skill5Val = 2,
						// AosAttribute_DefendChance = 6,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_CastSpeed = 1,
						// AosWeaponAttribute_HitPoisonArea = 25,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.TrollSkin:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.TrollSlaughter,
						// AosAttribute_RegenHits = 2,
						// AosAttribute_BonusStr = 1,
						// AosWeaponAttribute_SelfRepair = 3,
						// AosArmorAttribute_SelfRepair = 3
					}); break;
				case CraftResource.UnicornSkin:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Skill5 = 99,
						Skill5Val = 2,
						// AosAttribute_RegenMana = 1,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_BonusMana = 2,
						// AosAttribute_SpellDamage = 2,
						// AosAttribute_CastRecovery = 1,
						// AosAttribute_CastSpeed = 1,
						// AosAttribute_LowerManaCost = 3,
						// AosAttribute_SpellChanneling = 1,
						// AosWeaponAttribute_HitLowerAttack = 25,
						// AosWeaponAttribute_HitLowerDefend = 25,
						// AosWeaponAttribute_HitMagicArrow = 30,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.IcySkin:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.FlameDousing,
						Skill5 = 99,
						Skill5Val = 2,
						// AosAttribute_RegenStam = 2,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_ReflectPhysical = 3,
						// AosWeaponAttribute_HitColdArea = 20
					}); break;
				case CraftResource.LavaSkin:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.WaterDissipation,
						Skill5 = 99,
						Skill5Val = 2,
						// AosAttribute_RegenStam = 2,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_ReflectPhysical = 3,
						// AosWeaponAttribute_HitFireArea = 20
					}); break;
				case CraftResource.Seaweed:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.NeptunesBane,
						Skill5 = 19,
						Skill5Val = 2,
						// AosAttribute_RegenHits = 2,
						// AosAttribute_LowerRegCost = 4,
						// AosAttribute_SpellChanneling = 1,
						// AosWeaponAttribute_HitLightning = 20,
						// AosWeaponAttribute_HitPoisonArea = 30,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.DeadSkin:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Silver,
						Skill5 = 36,
						Skill5Val = 2,
						Skill4 = 22,
						Skill4Val = 5,
						// AosAttribute_RegenStam = 2,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_ReflectPhysical = 2,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_SelfRepair = 2
					}); break;

				case CraftResource.BrittleSkeletal:
				case CraftResource.DrowSkeletal:
				case CraftResource.OrcSkeletal:
				case CraftResource.ReptileSkeletal:
				case CraftResource.OgreSkeletal:
				case CraftResource.TrollSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo()); break;
				case CraftResource.GargoyleSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Skill5 = 99,
						Skill5Val = 1
					}); break;
				case CraftResource.MinotaurSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Skill5 = 99,
						Skill5Val = 1
					}); break;
				case CraftResource.LycanSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.AnimalHunter,
						Skill5 = 99,
						Skill5Val = 2
					}); break;
				case CraftResource.SharkSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.NeptunesBane,
						Skill5 = 99,
						Skill5Val = 2
					}); break;
				case CraftResource.ColossalSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.GiantKiller,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 22,
						Skill4Val = 1
					}); break;
				case CraftResource.MysticalSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.WizardSlayer,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 22,
						Skill4Val = 1,
						// AosAttribute_RegenMana = 1,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_BonusMana = 3,
						// AosAttribute_SpellDamage = 2,
						// AosAttribute_CastRecovery = 1,
						// AosAttribute_CastSpeed = 1,
						// AosAttribute_LowerRegCost = 3,
						// AosAttribute_SpellChanneling = 1,
						// AosWeaponAttribute_HitMagicArrow = 15,
						// AosWeaponAttribute_HitDispel = 10,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.VampireSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Repond,
						Skill5 = 99,
						Skill5Val = 2,
						Skill4 = 22,
						Skill4Val = 1,
						// AosAttribute_RegenStam = 2,
						// AosAttribute_DefendChance = 5,
						// AosAttribute_BonusStam = 2,
						// AosAttribute_LowerManaCost = 3,
						// AosAttribute_ReflectPhysical = 2,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_HitLeechHits = 10,
						// AosWeaponAttribute_HitHarm = 10,
						// AosWeaponAttribute_HitColdArea = 15
					}); break;
				case CraftResource.LichSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Silver,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 22,
						Skill4Val = 1,
						// AosAttribute_RegenMana = 1,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_BonusMana = 3,
						// AosAttribute_SpellDamage = 2,
						// AosAttribute_CastRecovery = 1,
						// AosAttribute_CastSpeed = 1,
						// AosAttribute_LowerRegCost = 3,
						// AosAttribute_EnhancePotions = 2,
						// AosAttribute_SpellChanneling = 1,
						// AosWeaponAttribute_HitLeechMana = 5,
						// AosWeaponAttribute_HitDispel = 5,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.SphinxSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.GargoylesFoe,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 22,
						Skill4Val = 2,
						// AosAttribute_WeaponDamage = 3,
						// AosAttribute_LowerManaCost = 3,
						// AosAttribute_ReflectPhysical = 3,
						// AosWeaponAttribute_HitLowerAttack = 5,
						// AosWeaponAttribute_HitLowerDefend = 5,
						// AosWeaponAttribute_HitLightning = 10,
						// AosWeaponAttribute_HitPhysicalArea = 20,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.DevilSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.DaemonDismissal,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 22,
						Skill4Val = 2,
						// AosAttribute_RegenMana = 1,
						// AosAttribute_BonusInt = 2,
						// AosAttribute_BonusStam = 2,
						// AosAttribute_SpellDamage = 2,
						// AosAttribute_LowerRegCost = 5,
						// AosWeaponAttribute_SelfRepair = 1,
						// AosWeaponAttribute_HitFireArea = 15,
						// AosArmorAttribute_SelfRepair = 1,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.DracoSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.DragonSlaying,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 22,
						Skill4Val = 2,
						// AosAttribute_AttackChance = 5,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_BonusHits = 3,
						// AosAttribute_WeaponDamage = 5,
						// AosAttribute_WeaponSpeed = 3,
						// AosWeaponAttribute_HitFireball = 25,
						// AosWeaponAttribute_HitFireArea = 25
					}); break;
				case CraftResource.XenoSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.OrcSlaying,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 22,
						Skill4Val = 3,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_BonusStam = 2,
						// AosAttribute_WeaponDamage = 3,
						// AosAttribute_ReflectPhysical = 2,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_HitEnergyArea = 15,
						// AosArmorAttribute_SelfRepair = 1
					}); break;
				case CraftResource.AndorianSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.TrollSlaughter,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 22,
						Skill4Val = 2,
						// AosAttribute_EnhancePotions = 5,
						// AosWeaponAttribute_HitLowerAttack = 10,
						// AosWeaponAttribute_HitColdArea = 20
					}); break;
				case CraftResource.CardassianSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.OgreTrashing,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 22,
						Skill4Val = 2,
						// AosAttribute_DefendChance = 3,
						// AosAttribute_BonusMana = 2,
						// AosAttribute_WeaponDamage = 2,
						// AosAttribute_SpellChanneling = 1,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_SelfRepair = 1,
						// AosWeaponAttribute_HitFireArea = 10,
						// AosWeaponAttribute_HitEnergyArea = 10,
						// AosArmorAttribute_SelfRepair = 1
					}); break;
				case CraftResource.MartianSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.Terathan,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 22,
						Skill4Val = 2,
						// AosAttribute_RegenHits = 2,
						// AosAttribute_RegenStam = 2,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_NightSight = 1,
						// AosWeaponAttribute_HitPoisonArea = 20
					}); break;
				case CraftResource.RodianSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.SnakesBane,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 22,
						Skill4Val = 2,
						// AosAttribute_AttackChance = 3,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_ReflectPhysical = 2,
						// AosAttribute_EnhancePotions = 2,
						// AosWeaponAttribute_HitPoisonArea = 10,
						// AosWeaponAttribute_HitEnergyArea = 10,
						// AosArmorAttribute_MageArmor = 1
					}); break;
				case CraftResource.TuskenSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.LizardmanSlaughter,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 22,
						Skill4Val = 2,
						// AosAttribute_BonusHits = 2,
						// AosAttribute_WeaponDamage = 4,
						// AosWeaponAttribute_HitFireArea = 20,
						// AosWeaponAttribute_HitPhysicalArea = 25
					}); break;
				case CraftResource.TwilekSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.SpidersDeath,
						Skill5 = 35,
						Skill5Val = 3,
						Skill4 = 39,
						Skill4Val = 3,
						Skill3 = 16,
						Skill3Val = 2,
						// AosAttribute_DefendChance = 3,
						// AosAttribute_BonusStr = 1,
						// AosAttribute_SpellChanneling = 1,
						// AosWeaponAttribute_HitLightning = 5,
						// AosWeaponAttribute_HitEnergyArea = 20
					}); break;
				case CraftResource.XindiSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.ScorpionsBane,
						Skill5 = 99,
						Skill5Val = 4,
						Skill4 = 48,
						Skill4Val = 3,
						// AosAttribute_BonusInt = 1,
						// AosAttribute_ReflectPhysical = 2,
						// AosWeaponAttribute_HitColdArea = 10,
						// AosWeaponAttribute_HitPoisonArea = 10,
						// AosWeaponAttribute_HitEnergyArea = 10
					}); break;
				case CraftResource.ZabrakSkeletal:
					ResourceMods.ModifyItem(item, resource, reduce, new ResourceModInfo
					{
						Slayer2 = SlayerName.SlimyScourge,
						Skill5 = 99,
						Skill5Val = 3,
						Skill4 = 48,
						Skill4Val = 3,
						Skill3 = 38,
						Skill3Val = 2,
						// AosAttribute_AttackChance = 3,
						// AosAttribute_BonusDex = 1,
						// AosAttribute_BonusStam = 2,
						// AosAttribute_WeaponDamage = 3,
						// AosWeaponAttribute_SelfRepair = 2,
						// AosWeaponAttribute_HitLeechStam = 5,
						// AosWeaponAttribute_HitFireArea = 10,
						// AosWeaponAttribute_HitEnergyArea = 10,
						// AosArmorAttribute_SelfRepair = 2
					}); break;
			}
		}

		public static void GetGemMods( GemType resource, Item item, bool reduce )
		{
			switch(resource)
			{
				case GemType.Amber:
					ResourceMods.ModifyJewelry(item, resource, reduce, new GemModInfo
					{
						AosAttribute_RegenStam = 1, 
						AosAttribute_BonusDex = 1, 
						AosAttribute_EnhancePotions = 5,
					}); break;

				case GemType.Citrine:
					ResourceMods.ModifyJewelry(item, resource, reduce, new GemModInfo
					{
						AosAttribute_RegenStam = 1,
						AosAttribute_BonusDex = 1,
						AosAttribute_WeaponSpeed = 5,
					}); break;

				case GemType.Ruby:
					ResourceMods.ModifyJewelry(item, resource, reduce, new GemModInfo
					{
						AosAttribute_BonusStr = 1,
						AosAttribute_BonusHits = 3,
						AosAttribute_WeaponDamage = 2,
						AosAttribute_NightSight = 1,
					}); break;

				case GemType.Tourmaline:
					ResourceMods.ModifyJewelry(item, resource, reduce, new GemModInfo
					{
						AosAttribute_RegenStam = 1,
						AosAttribute_BonusDex = 1,
						AosAttribute_BonusStam = 3,
						AosAttribute_WeaponDamage = 2,
						AosAttribute_WeaponSpeed = 2,
					}); break;

				case GemType.Amethyst:
					ResourceMods.ModifyJewelry(item, resource, reduce, new GemModInfo
					{
						AosAttribute_RegenMana = 1,
						AosAttribute_BonusInt = 1,
						AosAttribute_BonusMana = 3,
						AosAttribute_SpellDamage = 2,
						AosAttribute_CastRecovery = 1,
						AosAttribute_LowerManaCost = 3,
					}); break;

				case GemType.Emerald:
					ResourceMods.ModifyJewelry(item, resource, reduce, new GemModInfo
					{
						AosAttribute_BonusStr = 1,
						AosAttribute_LowerRegCost = 5,
						AosAttribute_ReflectPhysical = 5,
					}); break;

				case GemType.Sapphire:
					ResourceMods.ModifyJewelry(item, resource, reduce, new GemModInfo
					{
						AosAttribute_RegenMana = 1,
						AosAttribute_BonusInt = 1,
						AosAttribute_BonusMana = 3,
						AosAttribute_LowerManaCost = 4,
						AosAttribute_LowerRegCost = 4,
					}); break;

				case GemType.StarSapphire:
					ResourceMods.ModifyJewelry(item, resource, reduce, new GemModInfo
					{
						AosAttribute_RegenMana = 1,
						AosAttribute_BonusInt = 1,
						AosAttribute_SpellDamage = 5,
						AosAttribute_CastRecovery = 1,
						AosAttribute_CastSpeed = 1,
						AosAttribute_EnhancePotions = 5,
						AosAttribute_NightSight = 1,
					}); break;

				case GemType.Diamond:
					ResourceMods.ModifyJewelry(item, resource, reduce, new GemModInfo
					{
						AosAttribute_DefendChance = 5,
						AosAttribute_BonusStr = 2,
						AosAttribute_BonusHits = 5,
						AosAttribute_NightSight = 1,
					}); break;

				case GemType.Pearl:
					ResourceMods.ModifyJewelry(item, resource, reduce, new GemModInfo
					{
						AosAttribute_RegenStam = 2,
						AosAttribute_RegenMana = 2,
						AosAttribute_SpellDamage = 5,
						AosAttribute_LowerManaCost = 10,
						AosAttribute_LowerRegCost = 10,
						AosAttribute_NightSight = 1,
					}); break;
				
				case GemType.None:
				default:
					break;
			}
		}

        public static int GetSkillCheckMultiplier(CraftResource resource)
        {
            switch (resource)
            {
                case CraftResource.None:
                    return 1;

                case CraftResource.Iron:
                    return 1;

                case CraftResource.DullCopper:
                case CraftResource.ShadowIron:
                case CraftResource.Copper:
                    return 2;

                case CraftResource.Bronze:
                case CraftResource.Gold:
                case CraftResource.Agapite:
                    return 3;

                case CraftResource.Verite:
                case CraftResource.Valorite:
                case CraftResource.Nepturite:
                case CraftResource.Obsidian:
                case CraftResource.Steel:
                case CraftResource.Brass:
                case CraftResource.Mithril:
                    return 4;

                case CraftResource.Xormite:
                case CraftResource.Dwarven:
                    return 5;

                case CraftResource.AmethystBlock:
                case CraftResource.EmeraldBlock:
                case CraftResource.GarnetBlock:
                case CraftResource.IceBlock:
                case CraftResource.JadeBlock:
                case CraftResource.MarbleBlock:
                case CraftResource.OnyxBlock:
                case CraftResource.QuartzBlock:
                case CraftResource.RubyBlock:
                case CraftResource.SapphireBlock:
                case CraftResource.SilverBlock:
                case CraftResource.SpinelBlock:
                case CraftResource.StarRubyBlock:
                case CraftResource.TopazBlock:
                case CraftResource.CaddelliteBlock:
                    return 3; // Guess

                case CraftResource.RegularLeather:
                    return 1;

                case CraftResource.HornedLeather:
                case CraftResource.FrozenLeather:
                case CraftResource.GoliathLeather:
                    return 2;

                case CraftResource.BarbedLeather:
                case CraftResource.NecroticLeather:
                case CraftResource.VolcanicLeather:
                case CraftResource.SpinedLeather:
                case CraftResource.DraconicLeather:
                case CraftResource.HellishLeather:
                    return 3;

                case CraftResource.DinosaurLeather:
                    return 4;

                case CraftResource.AlienLeather:
                    return 5;

                // Sci-Fi - Begin
                case CraftResource.Agrinium:
                case CraftResource.Beskar:
                case CraftResource.Carbonite:
                case CraftResource.Cortosis:
                case CraftResource.Durasteel:
                case CraftResource.Durite:
                case CraftResource.Farium:
                case CraftResource.Laminasteel:
                case CraftResource.Neuranium:
                case CraftResource.Phrik:
                case CraftResource.Promethium:
                case CraftResource.Quadranium:
                case CraftResource.Songsteel:
                case CraftResource.Titanium:
                case CraftResource.Trimantium:
                case CraftResource.Xonolite:

                case CraftResource.Adesote:
                case CraftResource.Biomesh:
                case CraftResource.Cerlin:
                case CraftResource.Durafiber:
                case CraftResource.Flexicris:
                case CraftResource.Hypercloth:
                case CraftResource.Nylar:
                case CraftResource.Nylonite:
                case CraftResource.Polyfiber:
                case CraftResource.Syncloth:
                case CraftResource.Thermoweave:

                case CraftResource.GornScales:
                case CraftResource.TrandoshanScales:
                case CraftResource.SilurianScales:
                case CraftResource.KraytScales:

                case CraftResource.BorlTree:
                case CraftResource.CosianTree:
                case CraftResource.GreelTree:
                case CraftResource.JaporTree:
                case CraftResource.KyshyyykTree:
                case CraftResource.LaroonTree:
                case CraftResource.TeejTree:
                case CraftResource.VeshokTree:

                case CraftResource.XenoSkeletal:
                case CraftResource.AndorianSkeletal:
                case CraftResource.CardassianSkeletal:
                case CraftResource.MartianSkeletal:
                case CraftResource.RodianSkeletal:
                case CraftResource.TuskenSkeletal:
                case CraftResource.TwilekSkeletal:
                case CraftResource.XindiSkeletal:
                case CraftResource.ZabrakSkeletal:
                    return 4;
                // Sci-Fi - End

                case CraftResource.DemonSkin:
                case CraftResource.DragonSkin:
                case CraftResource.NightmareSkin:
                case CraftResource.SnakeSkin:
                case CraftResource.TrollSkin:
                case CraftResource.UnicornSkin:
                case CraftResource.IcySkin:
                case CraftResource.LavaSkin:
                case CraftResource.Seaweed:
                case CraftResource.DeadSkin:
                    return 3; // Guess

                case CraftResource.RedScales:
                case CraftResource.YellowScales:
                case CraftResource.BlackScales:
                case CraftResource.GreenScales:
                case CraftResource.WhiteScales:
                case CraftResource.BlueScales:
                case CraftResource.DinosaurScales:
                    return 3; // Guess

                case CraftResource.MetallicScales:
                case CraftResource.BrazenScales:
                case CraftResource.UmberScales:
                case CraftResource.VioletScales:
                case CraftResource.PlatinumScales:
                case CraftResource.CadalyteScales:
                    return 3; // Guess

                case CraftResource.Fabric:
                    return 1;

                case CraftResource.FurryFabric:
                case CraftResource.WoolyFabric:
                case CraftResource.SilkFabric:
					return 2; // Guess

                case CraftResource.HauntedFabric:
                case CraftResource.ArcticFabric:
                case CraftResource.PyreFabric:
					return 3; // Guess

                case CraftResource.VenomousFabric:
                case CraftResource.MysteriousFabric:
                case CraftResource.VileFabric:
					return 4; // Guess

                case CraftResource.DivineFabric:
                case CraftResource.FiendishFabric:
                    return 5; // Guess

                case CraftResource.RegularWood:
                    return 1;

                case CraftResource.AshTree:
                case CraftResource.CherryTree:
                case CraftResource.EbonyTree:
                    return 2;

                case CraftResource.GoldenOakTree:
                case CraftResource.HickoryTree:
                case CraftResource.MahoganyTree:
                case CraftResource.DriftwoodTree: // 50% Drop Rate
                case CraftResource.PetrifiedTree: // 100% Drop rate
                    return 3;

                case CraftResource.OakTree:
                case CraftResource.PineTree:
                case CraftResource.GhostTree:
                case CraftResource.RosewoodTree:
                case CraftResource.WalnutTree:
                    return 4;

                case CraftResource.ElvenTree:
                    return 5;

                case CraftResource.BrittleSkeletal:
                    return 1;

                case CraftResource.DrowSkeletal:
                case CraftResource.OrcSkeletal:
                case CraftResource.ReptileSkeletal:
                case CraftResource.OgreSkeletal:
                case CraftResource.TrollSkeletal:
                case CraftResource.GargoyleSkeletal:
                case CraftResource.MinotaurSkeletal:
                case CraftResource.LycanSkeletal:
                case CraftResource.SharkSkeletal:
                case CraftResource.ColossalSkeletal:
                case CraftResource.MysticalSkeletal:
                case CraftResource.VampireSkeletal:
                case CraftResource.LichSkeletal:
                case CraftResource.SphinxSkeletal:
                case CraftResource.DevilSkeletal:
                case CraftResource.DracoSkeletal:
                    return 3; // Guess

                case CraftResource.SpectralSpec:
                case CraftResource.DreadSpec:
                case CraftResource.GhoulishSpec:
                case CraftResource.WyrmSpec:
                case CraftResource.HolySpec:
                case CraftResource.BloodlessSpec:
                case CraftResource.GildedSpec:
                case CraftResource.DemilichSpec:
                case CraftResource.WintrySpec:
                case CraftResource.FireSpec:
                case CraftResource.ColdSpec:
                case CraftResource.PoisSpec:
                case CraftResource.EngySpec:
                case CraftResource.ExodusSpec:
                case CraftResource.TurtleSpec:
                    return 3; // Guess

                default:
					return 0;
            }
        }
	}
}