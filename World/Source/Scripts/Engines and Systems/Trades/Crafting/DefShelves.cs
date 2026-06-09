using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Craft
{
	public class DefShelves : CraftSystem
	{
		public override SkillName MainSkill
		{
			get	{ return SkillName.Carpentry;	}
		}

        public override int GumpImage
        {
            get { return 9600; }
        }

		public override int GumpTitleNumber
		{
			get { return 1044004; } // <CENTER>CARPENTRY MENU</CENTER>
		}

		public override string CraftSystemTxt
		{
			get { return "Crafting: Shelving"; }
		}

		public override CraftResourceType BreakDownType
		{
			get { return CraftResourceType.Wood; }
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefShelves();

				return m_CraftSystem;
			}
		}

		public override double GetChanceAtMin( CraftItem item )
		{
			return 0.5; // 50%
		}

		private DefShelves() : base( 1, 1, 1.25 )// base( 1, 1, 3.0 )
		{
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
			else if ( !BaseTool.CheckAccessible( tool, from ) )
				return 1044263; // The tool must be on your person to use.

			return 0;
		}

		public override void PlayCraftEffect( Mobile from )
		{
			CraftSystem.CraftSound( from, 0x23D, m_Tools );
		}

		public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, CraftItem item )
		{
			if ( toolBroken )
				from.SendLocalizedMessage( 1044038 ); // You have worn out your tool

			if ( failed )
			{
				if ( lostMaterial )
					return 1044043; // You failed to create the item, and some of your materials are lost.
				else
					return 1044157; // You failed to create the item, but no materials were lost.
			}
			else
			{
				if ( quality == 0 )
					return 502785; // You were barely able to make this item.  It's quality is below average.
				else if ( quality == 2 )
					return 1044155; // You create an exceptional quality item.
				else
					return 1044154; // You create the item.
			}
		}

		public override void InitCraftList()
		{
			int index = -1;

			// 1
			AddCraft( typeof( FancyArmoire ),		"Armoires", 1044312, 51.5, 76.5,	typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( Armoire ),			"Armoires", 1022643, 51.5, 76.5,	typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( RedArmoire ),			"Armoires", 1030328, 90.0, 115.0,	typeof( Board ), 1015101, 40, 1044351 );
			AddCraft( typeof( ElegantArmoire ),		"Armoires", 1030330, 90.0, 115.0,	typeof( Board ), 1015101, 40, 1044351 );
			AddCraft( typeof( MapleArmoire ),		"Armoires", 1030332, 90.0, 115.0,	typeof( Board ), 1015101, 40, 1044351 );
			AddCraft( typeof( CherryArmoire ),		"Armoires", 1030334, 90.0, 115.0,	typeof( Board ), 1015101, 40, 1044351 );
			AddCraft( typeof( NewArmoireA ), 		"Armoires", "bamboo armoire", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmoireB ), 		"Armoires", "bamboo armoire", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmoireC ), 		"Armoires", "bamboo armoire", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmoireD ), 		"Armoires", "armoire", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmoireE ),		"Armoires", "empty armoire", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmoireF ), 		"Armoires", "open armoire", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmoireG ), 		"Armoires", "armoire", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmoireH ), 		"Armoires", "empty armoire", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmoireI ), 		"Armoires", "open armoire", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmoireJ ), 		"Armoires", "open armoire", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredArmoireA ),	"Armoires", "fancy amoire*", 51.5, 76.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( ColoredArmoireB ),	"Armoires", "tall fancy armoire*", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );

			// 2
			AddCraft( typeof( TallCabinet ),	"Cabinets", 1030261, 90.0, 115.0,	typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ShortCabinet ),	"Cabinets", 1030263, 90.0, 115.0,	typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredCabinetA ), "Cabinets", "book cabinet*", 51.5, 76.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( ColoredCabinetB ), "Cabinets", "dish cabinet*", 51.5, 76.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( ColoredCabinetC ), "Cabinets", "medium cabinet*", 51.5, 76.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( ColoredCabinetD ), "Cabinets", "narrow book cabinet*", 51.5, 76.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( ColoredCabinetE ), "Cabinets", "short cabinet*", 51.5, 76.5, typeof( Board ), 1015101, 25, 1044351 );
			AddCraft( typeof( ColoredCabinetF ), "Cabinets", "short elegant cabinet*", 51.5, 76.5, typeof( Board ), 1015101, 25, 1044351 );
			AddCraft( typeof( ColoredCabinetG ), "Cabinets", "short locker*", 51.5, 76.5, typeof( Board ), 1015101, 25, 1044351 );
			AddCraft( typeof( ColoredCabinetH ), "Cabinets", "storage cabinet*", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredCabinetI ), "Cabinets", "tall fancy cabinet*", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredCabinetJ ), "Cabinets", "tall medium cabinet*", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredCabinetK ), "Cabinets", "tall narrow cabinet*", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredCabinetL ), "Cabinets", "tall wide cabinet*", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredCabinetM ), "Cabinets", "tall wide locker*", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredCabinetN ), "Cabinets", "wide medium cabinet*", 51.5, 76.5, typeof( Board ), 1015101, 35, 1044351 );

			// 3
			AddCraft( typeof( WoodenBox ),				"Chests", 1023709,	21.0,  46.0,	typeof( Board ), 1015101, 10, 1044351 );
			AddCraft( typeof( WoodenChest ),			"Chests", 1023650,	73.6,  98.6,	typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( PlainWoodenChest ),		"Chests", 1030251, 90.0, 115.0,	typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( OrnateWoodenChest ),		"Chests", 1030253, 90.0, 115.0,	typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( GildedWoodenChest ),		"Chests", 1030255, 90.0, 115.0,	typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( WoodenFootLocker ),		"Chests", 1030257, 90.0, 115.0,	typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( FinishedWoodenChest ),	"Chests", 1030259, 90.0, 115.0,	typeof( Board ), 1015101, 30, 1044351 );
			index = AddCraft( typeof( WoodenCoffin ), 	"Chests", "coffin", 90.0, 115.0, typeof( Board ), 1015101, 40, 1044351 );
			AddSkill( index, SkillName.Forensics, 75.0, 80.0 );
			index = AddCraft( typeof( WoodenCasket ), 	"Chests", "casket", 90.0, 115.0, typeof( Board ), 1015101, 40, 1044351 );
			AddSkill( index, SkillName.Forensics, 75.0, 80.0 );
			
			index = AddCraft( typeof( PickBoxEasy ), "Chests", "locked box (easy)", 21.0,  46.0, typeof( GoldIngot ), 1027146, 2, 1044037 );
			AddSkill( index, SkillName.Tinkering, 30, 30 );
			AddRes( index, typeof( Board ), 1015101, 2, 1044351 );
			AddRes( index, typeof( ArcaneGem ), "arcane gem", 1, 1053098 );

			index = AddCraft( typeof( PickBoxNormal ), "Chests", "locked box (normal)", 21.0,  46.0, typeof( GoldIngot ), 1027146, 2, 1044037 );
			AddSkill( index, SkillName.Tinkering, 30, 30 );
			AddSkill( index, SkillName.Lockpicking, 20, 20 );
			AddRes( index, typeof( Board ), 1015101, 2, 1044351 );
			AddRes( index, typeof( ArcaneGem ), "arcane gem", 2, 1053098 );

			index = AddCraft( typeof( PickBoxDifficult ), "Chests", "locked box (difficult)", 21.0,  46.0, typeof( GoldIngot ), 1027146, 2, 1044037 );
			AddSkill( index, SkillName.Tinkering, 30, 30 );
			AddSkill( index, SkillName.Lockpicking, 30, 30 );
			AddRes( index, typeof( Board ), 1015101, 2, 1044351 );
			AddRes( index, typeof( ArcaneGem ), "arcane gem", 3, 1053098 );

			index = AddCraft( typeof( PickBoxChallenging ), "Chests", "locked box (challenging)", 31.0,  56.0, typeof( GoldIngot ), 1027146, 2, 1044037 );
			AddSkill( index, SkillName.Tinkering, 50, 50 );
			AddSkill( index, SkillName.Lockpicking, 40, 40 );
			AddRes( index, typeof( Board ), 1015101, 2, 1044351 );
			AddRes( index, typeof( ArcaneGem ), "arcane gem", 4, 1053098 );

			index = AddCraft( typeof( PickBoxHard ), "Chests", "locked box (hard)", 31.0,  56.0, typeof( GoldIngot ), 1027146, 2, 1044037 );
			AddSkill( index, SkillName.Tinkering, 50, 50 );
			AddSkill( index, SkillName.Lockpicking, 50, 50 );
			AddRes( index, typeof( Board ), 1015101, 2, 1044351 );
			AddRes( index, typeof( ArcaneGem ), "arcane gem", 10, 1053098 );

			index = AddCraft( typeof( PickBoxVeryHard ), "Chests", "locked box (very hard)", 41.0,  66.0, typeof( GoldIngot ), 1027146, 2, 1044037 );
			AddSkill( index, SkillName.Tinkering, 70, 70 );
			AddSkill( index, SkillName.Lockpicking, 60, 60 );
			AddRes( index, typeof( Board ), 1015101, 2, 1044351 );
			AddRes( index, typeof( PowerCrystal ), "power crystal", 1, 1053098 );

			index = AddCraft( typeof( PickBoxExtreme ), "Chests", "locked box (extreme)", 41.0,  66.0, typeof( GoldIngot ), 1027146, 2, 1044037 );
			AddSkill( index, SkillName.Tinkering, 70, 70 );
			AddSkill( index, SkillName.Lockpicking, 70, 70 );
			AddRes( index, typeof( Board ), 1015101, 2, 1044351 );
			AddRes( index, typeof( PowerCrystal ), "power crystal", 3, 1053098 );

			index = AddCraft( typeof( PickBoxMaster ), "Chests", "locked box (master)", 51.0,  76.0, typeof( GoldIngot ), 1027146, 2, 1044037 );
			AddSkill( index, SkillName.Tinkering, 90, 90 );
			AddSkill( index, SkillName.Lockpicking, 80, 80 );
			AddRes( index, typeof( Board ), 1015101, 2, 1044351 );
			AddRes( index, typeof( PowerCrystal ), "power crystal", 10, 1053098 );

			// 4
			AddCraft( typeof( SmallCrate ),			"Crates", 1044309,	10.0,  35.0,	typeof( Board ), 1015101, 8 , 1044351 );
			AddCraft( typeof( MediumCrate ),		"Crates", 1044310,	31.0,  56.0,	typeof( Board ), 1015101, 15, 1044351 );
			AddCraft( typeof( LargeCrate ),			"Crates", 1044311,	47.3,  72.3,	typeof( Board ), 1015101, 18, 1044351 );
			AddCraft( typeof( AdventurerCrate ), 	"Crates", "adventurer crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( AlchemyCrate ), 		"Crates", "alchemy crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( ArmsCrate ), 			"Crates", "arms crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( BakerCrate ), 		"Crates", "baker crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( BeekeeperCrate ), 	"Crates", "beekeeper crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( BlacksmithCrate ), 	"Crates", "blacksmith crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( BowyerCrate ), 		"Crates", "bowyer crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( ButcherCrate ), 		"Crates", "butcher crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( CarpenterCrate ), 	"Crates", "carpenter crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( FletcherCrate ), 		"Crates", "fletcher crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( HealerCrate ), 		"Crates", "healer crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( HugeCrate ), 			"Crates", "huge crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( JewelerCrate ), 		"Crates", "jeweler crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( LibrarianCrate ), 	"Crates", "librarian crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( MusicianCrate ), 		"Crates", "musician crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( NecromancerCrate ), 	"Crates", "necromancer crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( ProvisionerCrate ), 	"Crates", "provisioner crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( SailorCrate ), 		"Crates", "sailor crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( StableCrate ), 		"Crates", "stable crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( SupplyCrate ), 		"Crates", "supply crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( TailorCrate ), 		"Crates", "tailor crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( TavernCrate ), 		"Crates", "tavern crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( TinkerCrate ), 		"Crates", "tinker crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( TreasureCrate ), 		"Crates", "treasure crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );
			AddCraft( typeof( WizardryCrate ), 		"Crates", "wizardry crate", 47.3,  72.3, typeof( Board ), 1015101, 20, 1044351 );

			// 5
			AddCraft( typeof( NewDrawersA ), "Dressers", "dresser", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersB ), "Dressers", "open dresser", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersC ), "Dressers", "open dresser", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersD ), "Dressers", "open ", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersE ), "Dressers", "nightstand", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersF ), "Dressers", "dresser", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersG ), "Dressers", "open dresser", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersH ), "Dressers", "open dresser", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersI ), "Dressers", "open dresser", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersJ ), "Dressers", "nightstand", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( Drawer ), "Dressers", "dresser", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersL ), "Dressers", "open dresser", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersM ), "Dressers", "open dresser", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersN ), "Dressers", "open dresser", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( NewDrawersK ), "Dressers", "nightstand", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( ColoredDresserA ), "Dressers", "dresser*", 31.5, 56.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredDresserB ), "Dressers", "fancy dresser*", 31.5, 56.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredDresserC ), "Dressers", "medium dresser*", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( ColoredDresserD ), "Dressers", "medium dresser*", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( ColoredDresserE ), "Dressers", "short elegant dresser*", 31.5, 56.5, typeof( Board ), 1015101, 25, 1044351 );
			AddCraft( typeof( ColoredDresserF ), "Dressers", "short narrow dresser*", 31.5, 56.5, typeof( Board ), 1015101, 25, 1044351 );
			AddCraft( typeof( ColoredDresserG ), "Dressers", "short wide dresser*", 31.5, 56.5, typeof( Board ), 1015101, 27, 1044351 );
			AddCraft( typeof( ColoredDresserH ), "Dressers", "standing dresser*", 31.5, 56.5, typeof( Board ), 1015101, 27, 1044351 );
			AddCraft( typeof( ColoredDresserI ), "Dressers", "trinket dresser*", 31.5, 56.5, typeof( Board ), 1015101, 30, 1044351 );
			AddCraft( typeof( ColoredDresserJ ), "Dressers", "wide medium dresser*", 31.5, 56.5, typeof( Board ), 1015101, 35, 1044351 );

			// 6
			AddCraft( typeof( NewShelfB ), "Bamboo Shelves", "shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewShelfA ), "Bamboo Shelves", "shelf, small", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewWizardShelfC ), "Bamboo Shelves", "alchemy shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmorShelfB ), "Bamboo Shelves", "armor shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBakerShelfA ), "Bamboo Shelves", "baker shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfB ), "Bamboo Shelves", "book shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfC ), "Bamboo Shelves", "book shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfD ), "Bamboo Shelves", "book shelf, small", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBowyerShelfA ), "Bamboo Shelves", "bowyer shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTannerShelfB ), "Bamboo Shelves", "carpenter shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewClothShelfA ), "Bamboo Shelves", "cloth shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewClothShelfB ), "Bamboo Shelves", "cloth shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewShoeShelfA ), "Bamboo Shelves", "cobbler shelf, small", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewDrinkShelfA ), "Bamboo Shelves", "drink shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewHunterShelf ), "Bamboo Shelves", "hunter shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewKitchenShelfB ), "Bamboo Shelves", "kitchen shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewSorcererShelfA ), "Bamboo Shelves", "sorcerer shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTailorShelfA ), "Bamboo Shelves", "tailor shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTannerShelfA ), "Bamboo Shelves", "supply shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTavernShelfC ), "Bamboo Shelves", "tavern shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBlacksmithShelfB ), "Bamboo Shelves", "tinker shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );

			// 7
			AddCraft( typeof( NewShelfC ), "Redwood Shelves", "shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewShelfD ), "Redwood Shelves", "shelf, small", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewWizardShelfD ), "Redwood Shelves", "alchemy shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmorShelfC ), "Redwood Shelves", "armor shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBakerShelfC ), "Redwood Shelves", "baker shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfE ), "Redwood Shelves", "book shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfF ), "Redwood Shelves", "book shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfG ), "Redwood Shelves", "book shelf, small", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBowyerShelfB ), "Redwood Shelves", "bowyer shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewCarpenterShelfA ), "Redwood Shelves", "carpenter shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewClothShelfC ), "Redwood Shelves", "cloth shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewClothShelfD ), "Redwood Shelves", "cloth shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewShoeShelfB ), "Redwood Shelves", "cobbler shelf, small", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewDrinkShelfB ), "Redwood Shelves", "drink shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBakerShelfB ), "Redwood Shelves", "kitchen shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBlacksmithShelfC ), "Redwood Shelves", "smith shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewSorcererShelfB ), "Redwood Shelves", "sorcerer shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewSupplyShelfA ), "Redwood Shelves", "supply shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTailorShelfB ), "Redwood Shelves", "tailor shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTavernShelfD ), "Redwood Shelves", "tavern shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTinkerShelfA ), "Redwood Shelves", "tinker shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );

			// 8
			AddCraft( typeof( NewShelfF ), "Rustic Shelves", "shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewShelfE ), "Rustic Shelves", "shelf, small", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewWizardShelfE ), "Rustic Shelves", "alchemy shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmorShelfD ), "Rustic Shelves", "armor shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBakerShelfE ), "Rustic Shelves", "baker shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfH ), "Rustic Shelves", "book shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfI ), "Rustic Shelves", "book shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfJ ), "Rustic Shelves", "book shelf, small", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBowyerShelfC ), "Rustic Shelves", "bowyer shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewCarpenterShelfB ), "Rustic Shelves", "carpenter shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewClothShelfE ), "Rustic Shelves", "cloth shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewClothShelfF ), "Rustic Shelves", "cloth shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewShoeShelfC ), "Rustic Shelves", "cobbler shelf, small", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewDrinkShelfC ), "Rustic Shelves", "drink shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBakerShelfD ), "Rustic Shelves", "kitchen shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBlacksmithShelfD ), "Rustic Shelves", "smith shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewSorcererShelfC ), "Rustic Shelves", "sorcerer shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewSupplyShelfB ), "Rustic Shelves", "supply shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTailorShelfC ), "Rustic Shelves", "tailor shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTavernShelfE ), "Rustic Shelves", "tavern shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTinkerShelfB ), "Rustic Shelves", "tinker shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );

			// 9
			AddCraft( typeof( ColoredShelf1 ), "Stained Shelf", "alchemy shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelf2 ), "Stained Shelf", "armor shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelf3 ), "Stained Shelf", "baker shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelf4 ), "Stained Shelf", "barkeep shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelf5 ), "Stained Shelf", "book shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelf6 ), "Stained Shelf", "bowyer shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelf7 ), "Stained Shelf", "carpenter shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelf8 ), "Stained Shelf", "cloth shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfA ), "Stained Shelf", "cobbler shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfN ), "Stained Shelf", "cobbler shelf*", 41.5, 66.5, typeof( Board ), 1015101, 25, 1044351 );
			AddCraft( typeof( ColoredShelfB ), "Stained Shelf", "drink shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfC ), "Stained Shelf", "empty shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfD ), "Stained Shelf", "food shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfE ), "Stained Shelf", "kitchen shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfF ), "Stained Shelf", "library shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfG ), "Stained Shelf", "liquor shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfH ), "Stained Shelf", "necromancer shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfI ), "Stained Shelf", "plain cloth shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfJ ), "Stained Shelf", "provisions shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( SailorShelf ), "Stained Shelf", "sailor shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfK ), "Stained Shelf", "short book shelf*", 41.5, 66.5, typeof( Board ), 1015101, 25, 1044351 );
			AddCraft( typeof( ColoredShelfL ), "Stained Shelf", "short empty shelf*", 41.5, 66.5, typeof( Board ), 1015101, 25, 1044351 );
			AddCraft( typeof( ColoredShelfM ), "Stained Shelf", "short kitchen shelf*", 41.5, 66.5, typeof( Board ), 1015101, 25, 1044351 );
			AddCraft( typeof( ColoredShelfO ), "Stained Shelf", "smith shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfP ), "Stained Shelf", "storage shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfQ ), "Stained Shelf", "supply shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfR ), "Stained Shelf", "tailor shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfS ), "Stained Shelf", "tall supply shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfT ), "Stained Shelf", "tall wizard shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfU ), "Stained Shelf", "tamer shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfV ), "Stained Shelf", "tavern shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfW ), "Stained Shelf", "tinker shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfX ), "Stained Shelf", "tome shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfY ), "Stained Shelf", "weaver shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( ColoredShelfZ ), "Stained Shelf", "wizard shelf*", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );

			// 10
			AddCraft( typeof( EmptyBookcase ), "Wood Shelves", 1022718, 41.5, 66.5, typeof( Board ), 1015101, 25, 1044351 );
			AddCraft( typeof( FullBookcase ), "Wood Shelves", 1022711,	41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewShelfG ), "Wood Shelves", "shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewShelfH ), "Wood Shelves", "shelf, small", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewWizardShelfF ), "Wood Shelves", "alchemy shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmorShelfA ), "Wood Shelves", "armor shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewArmorShelfE ), "Wood Shelves", "armor shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBakerShelfG ), "Wood Shelves", "baker shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfK ), "Wood Shelves", "book shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfL ), "Wood Shelves", "book shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfM ), "Wood Shelves", "book shelf, small", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBookShelfA ), "Wood Shelves", "book shelf, tall", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBowyerShelfD ), "Wood Shelves", "bowyer shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewCarpenterShelfC ), "Wood Shelves", "carpenter shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewClothShelfG ), "Wood Shelves", "cloth shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewClothShelfH ), "Wood Shelves", "cloth shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewShoeShelfD ), "Wood Shelves", "cobbler shelf, small", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewDarkBookShelfA ), "Wood Shelves", "dark book shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewDarkBookShelfB ), "Wood Shelves", "dark book shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewDarkShelf ), "Wood Shelves", "dark shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewDrinkShelfD ), "Wood Shelves", "drink shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewHelmShelf ), "Wood Shelves", "helm shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBakerShelfF ), "Wood Shelves", "kitchen shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewKitchenShelfA ), "Wood Shelves", "kitchen shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewDrinkShelfE ), "Wood Shelves", "liquor shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewOldBookShelf ), "Wood Shelves", "old book shelf",	41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewPotionShelf ), "Wood Shelves", "potion shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewRuinedBookShelf ), "Wood Shelves", "ruined book shelf",	41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewBlacksmithShelfE ), "Wood Shelves", "smith shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewSorcererShelfD ), "Wood Shelves", "sorcerer shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewSupplyShelfC ), "Wood Shelves", "supply shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTailorShelfD ), "Wood Shelves", "tailor shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTavernShelfF ), "Wood Shelves", "tavern shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTinkerShelfC ), "Wood Shelves", "tinker shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewTortureShelf ), "Wood Shelves", "torture shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewWizardShelfB ), "Wood Shelves", "wizard shelf", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );
			AddCraft( typeof( NewWizardShelfA ), "Wood Shelves", "wizard shelf, tall", 41.5, 66.5, typeof( Board ), 1015101, 35, 1044351 );

			BreakDown = true;

			SetSubRes( typeof( Board ), CraftResources.GetClilocCraftName( CraftResource.RegularWood ) );

			// Add every material you want the player to be able to choose from
			// This will override the overridable material

			int cannot = 1079597; // You have no idea how to work this wood.

			AddSubRes( typeof( Board ),				CraftResources.GetClilocCraftName( CraftResource.RegularWood ), CraftResources.GetSkill( CraftResource.RegularWood ), CraftResources.GetClilocMaterialName( CraftResource.RegularWood ), cannot );
			AddSubRes( typeof( AshBoard ),			CraftResources.GetClilocCraftName( CraftResource.AshTree ), CraftResources.GetSkill( CraftResource.AshTree ), CraftResources.GetClilocMaterialName( CraftResource.AshTree ), cannot );
			AddSubRes( typeof( CherryBoard ),		CraftResources.GetClilocCraftName( CraftResource.CherryTree ), CraftResources.GetSkill( CraftResource.CherryTree ), CraftResources.GetClilocMaterialName( CraftResource.CherryTree ), cannot );
			AddSubRes( typeof( EbonyBoard ),		CraftResources.GetClilocCraftName( CraftResource.EbonyTree ), CraftResources.GetSkill( CraftResource.EbonyTree ), CraftResources.GetClilocMaterialName( CraftResource.EbonyTree ), cannot );
			AddSubRes( typeof( GoldenOakBoard ),	CraftResources.GetClilocCraftName( CraftResource.GoldenOakTree ), CraftResources.GetSkill( CraftResource.GoldenOakTree ), CraftResources.GetClilocMaterialName( CraftResource.GoldenOakTree ), cannot );
			AddSubRes( typeof( HickoryBoard ),		CraftResources.GetClilocCraftName( CraftResource.HickoryTree ), CraftResources.GetSkill( CraftResource.HickoryTree ), CraftResources.GetClilocMaterialName( CraftResource.HickoryTree ), cannot );
			AddSubRes( typeof( MahoganyBoard ),		CraftResources.GetClilocCraftName( CraftResource.MahoganyTree ), CraftResources.GetSkill( CraftResource.MahoganyTree ), CraftResources.GetClilocMaterialName( CraftResource.MahoganyTree ), cannot );
			AddSubRes( typeof( OakBoard ),			CraftResources.GetClilocCraftName( CraftResource.OakTree ), CraftResources.GetSkill( CraftResource.OakTree ), CraftResources.GetClilocMaterialName( CraftResource.OakTree ), cannot );
			AddSubRes( typeof( PineBoard ),			CraftResources.GetClilocCraftName( CraftResource.PineTree ), CraftResources.GetSkill( CraftResource.PineTree ), CraftResources.GetClilocMaterialName( CraftResource.PineTree ), cannot );
			AddSubRes( typeof( GhostBoard ),		CraftResources.GetClilocCraftName( CraftResource.GhostTree ), CraftResources.GetSkill( CraftResource.GhostTree ), CraftResources.GetClilocMaterialName( CraftResource.GhostTree ), cannot );
			AddSubRes( typeof( RosewoodBoard ),		CraftResources.GetClilocCraftName( CraftResource.RosewoodTree ), CraftResources.GetSkill( CraftResource.RosewoodTree ), CraftResources.GetClilocMaterialName( CraftResource.RosewoodTree ), cannot );
			AddSubRes( typeof( WalnutBoard ),		CraftResources.GetClilocCraftName( CraftResource.WalnutTree ), CraftResources.GetSkill( CraftResource.WalnutTree ), CraftResources.GetClilocMaterialName( CraftResource.WalnutTree ), cannot );
			AddSubRes( typeof( PetrifiedBoard ),	CraftResources.GetClilocCraftName( CraftResource.PetrifiedTree ), CraftResources.GetSkill( CraftResource.PetrifiedTree ), CraftResources.GetClilocMaterialName( CraftResource.PetrifiedTree ), cannot );
			AddSubRes( typeof( DriftwoodBoard ),	CraftResources.GetClilocCraftName( CraftResource.DriftwoodTree ), CraftResources.GetSkill( CraftResource.DriftwoodTree ), CraftResources.GetClilocMaterialName( CraftResource.DriftwoodTree ), cannot );
			AddSubRes( typeof( ElvenBoard ),		CraftResources.GetClilocCraftName( CraftResource.ElvenTree ), CraftResources.GetSkill( CraftResource.ElvenTree ), CraftResources.GetClilocMaterialName( CraftResource.ElvenTree ), cannot );
			// AddSubRes( typeof( BorlBoard ),			CraftResources.GetClilocCraftName( CraftResource.BorlTree ), CraftResources.GetSkill( CraftResource.BorlTree ), CraftResources.GetClilocMaterialName( CraftResource.BorlTree ), cannot );
			// AddSubRes( typeof( CosianBoard ),		CraftResources.GetClilocCraftName( CraftResource.CosianTree ), CraftResources.GetSkill( CraftResource.CosianTree ), CraftResources.GetClilocMaterialName( CraftResource.CosianTree ), cannot );
			// AddSubRes( typeof( GreelBoard ),		CraftResources.GetClilocCraftName( CraftResource.GreelTree ), CraftResources.GetSkill( CraftResource.GreelTree ), CraftResources.GetClilocMaterialName( CraftResource.GreelTree ), cannot );
			// AddSubRes( typeof( JaporBoard ),		CraftResources.GetClilocCraftName( CraftResource.JaporTree ), CraftResources.GetSkill( CraftResource.JaporTree ), CraftResources.GetClilocMaterialName( CraftResource.JaporTree ), cannot );
			// AddSubRes( typeof( KyshyyykBoard ),		CraftResources.GetClilocCraftName( CraftResource.KyshyyykTree ), CraftResources.GetSkill( CraftResource.KyshyyykTree ), CraftResources.GetClilocMaterialName( CraftResource.KyshyyykTree ), cannot );
			// AddSubRes( typeof( LaroonBoard ),		CraftResources.GetClilocCraftName( CraftResource.LaroonTree ), CraftResources.GetSkill( CraftResource.LaroonTree ), CraftResources.GetClilocMaterialName( CraftResource.LaroonTree ), cannot );
			// AddSubRes( typeof( TeejBoard ),			CraftResources.GetClilocCraftName( CraftResource.TeejTree ), CraftResources.GetSkill( CraftResource.TeejTree ), CraftResources.GetClilocMaterialName( CraftResource.TeejTree ), cannot );
			// AddSubRes( typeof( VeshokBoard ),		CraftResources.GetClilocCraftName( CraftResource.VeshokTree ), CraftResources.GetSkill( CraftResource.VeshokTree ), CraftResources.GetClilocMaterialName( CraftResource.VeshokTree ), cannot );
		}
	}
}