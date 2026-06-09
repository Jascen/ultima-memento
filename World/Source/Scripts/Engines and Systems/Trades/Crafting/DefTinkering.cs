using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using System.Globalization;

namespace Server.Engines.Craft
{
	public class DefTinkering : CraftSystem
	{
		public override SkillName MainSkill
		{
			get	{ return SkillName.Tinkering; }
		}

        public override int GumpImage
        {
            get { return 9603; }
        }

		public override int GumpTitleNumber
		{
			get { return 1044007; } // <CENTER>TINKERING MENU</CENTER>
		}

        public override bool ShowGumpInfo
        {
            get { return true; }
        }

		public override CraftResourceType BreakDownType
		{
			get { return CraftResourceType.Metal; }
		}

		public override string CraftSystemTxt
		{
			get { return "Crafting: Tinkering"; }
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefTinkering();

				return m_CraftSystem;
			}
		}

		private DefTinkering() : base( 1, 1, 1.25 )// base( 1, 1, 3.0 )
		{
		}

		public override double GetChanceAtMin( CraftItem item )
		{
			if ( item.NameNumber == 1044258 || item.NameNumber == 1046445 ) // potion keg
				return 0.5; // 50%

			return 0.0; // 0%
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
			CraftSystem.CraftSound( from, 0x542, m_Tools );
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

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			#region Multi-Component Items

			index = AddCraft( typeof( AxleGears ), 1044051, 1024177, 0.0, 0.0, typeof( Axle ), 1044169, 1, 1044253 );
			AddRes( index, typeof( Gears ), 1044254, 1, 1044253 );

			index = AddCraft( typeof( ClockParts ), 1044051, 1024175, 0.0, 0.0, typeof( AxleGears ), 1044170, 1, 1044253 );
			AddRes( index, typeof( Springs ), 1044171, 1, 1044253 );

			index = AddCraft( typeof( SextantParts ), 1044051, 1024185, 0.0, 0.0, typeof( AxleGears ), 1044170, 1, 1044253 );
			AddRes( index, typeof( Hinge ), 1044172, 1, 1044253 );

			index = AddCraft( typeof( ClockRight ), 1044051, 1044257, 0.0, 0.0, typeof( ClockFrame ), 1044174, 1, 1044253 );
			AddRes( index, typeof( ClockParts ), 1044173, 1, 1044253 );

			index = AddCraft( typeof( ClockLeft ), 1044051, 1044256, 0.0, 0.0, typeof( ClockFrame ), 1044174, 1, 1044253 );
			AddRes( index, typeof( ClockParts ), 1044173, 1, 1044253 );

			AddCraft( typeof( Sextant ), 1044051, 1024183, 0.0, 0.0, typeof( SextantParts ), 1044175, 1, 1044253 );

			index = AddCraft( typeof( Bola ), 1044051, 1046441, 60.0, 80.0, typeof( BolaBall ), 1046440, 4, 1042613 );
			AddRes( index, typeof( Leather ), 1044462, 3, 1044463 );

			index = AddCraft( typeof( PotionKeg ), 1044051, 1044258, 75.0, 100.0, typeof( Keg ), 1044255, 1, 1044253 );
			AddRes( index, typeof( Bottle ), 1044250, 10, 1044253 );
			AddRes( index, typeof( BarrelLid ), 1044251, 1, 1044253 );
			AddRes( index, typeof( BarrelTap ), 1044252, 1, 1044253 );

			#endregion

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			#region Misc

			index = AddCraft( typeof( CandleLarge ), 1044050, 1022598, 45.0, 85.0, typeof( IronIngot ), 1044036, 2, 1044037 );
				AddRes( index, typeof( Beeswax ), 1025154, 1, 1053098 );
			index = AddCraft( typeof( Candelabra ), 1044050, 1022599, 55.0, 95.0, typeof( IronIngot ), 1044036, 4, 1044037 );
				AddRes( index, typeof( Beeswax ), 1025154, 3, 1053098 );
			index = AddCraft( typeof( CandelabraStand ), 1044050, 1022599, 65.0, 105.0, typeof( IronIngot ), 1044036, 8, 1044037 );
				AddRes( index, typeof( Beeswax ), 1025154, 3, 1053098 );

			AddCraft( typeof( Globe ), 1044050, 1024167, 55.0, 105.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( HeatingStand ), 1044050, 1026217, 60.0, 110.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Key ), 1044050, 1024112, 20.0, 70.0, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraft( typeof( KeyRing ), 1044050, 1024113, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( Lantern ), 1044050, 1022597, 30.0, 80.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( Spyglass ), 1044050, "spyglass", 60.0, 110.0, typeof( IronIngot ), 1044036, 4, 1044037 );

			index = AddCraft( typeof( WallTorch ), 1044050, "wall torch", 55.0, 105.0, typeof( IronIngot ), 1044036, 5, 1044037 );
				AddRes( index, typeof( Torch ), 1011410, 1, 1053098 );

			index = AddCraft( typeof( ColoredWallTorch ), 1044050, "colored wall torch", 85.0, 125.0, typeof( IronIngot ), 1044036, 5, 1044037 );
				AddRes( index, typeof( Torch ), 1011410, 1, 1053098 );

			index = AddCraft( typeof( ShojiLantern ), 1044050, 1029404, 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
				AddRes( index, typeof( Board ), 1015101, 5, 1044351 );

			index = AddCraft( typeof( PaperLantern ), 1044050, 1029406, 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
				AddRes( index, typeof( Board ), 1015101, 5, 1044351 );

			index = AddCraft( typeof( RoundPaperLantern ), 1044050, 1029418, 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
				AddRes( index, typeof( Board ), 1015101, 5, 1044351 );

			AddCraft( typeof( Scales ), 1044050, 1026225, 60.0, 110.0, typeof( IronIngot ), 1044036, 4, 1044037 );

			index = AddCraft( typeof( ThrowingWeapon ), 1044050, "throwing weapons", 0.0, 40.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			index = AddCraft( typeof( ThrowingWeapon ), 1044050, "a batch of throwing weapons", 40.0, 40.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			SetUseAllRes( index, true );

			index = AddCraft( typeof( Trumpet ), 1044050, "trumpet", 57.8, 82.8, typeof( IronIngot ), 1044036, 20, 1044037 );
			AddSkill( index, SkillName.Musicianship, 45.0, 50.0 );
			AddRes( index, typeof( Fabric ), 1044286, 5, 1044287 );

			AddCraft( typeof( WindChimes ), 1044050, 1030290, 80.0, 130.0, typeof( IronIngot ), 1044036, 15, 1044037 );

			AddCraft( typeof( FancyWindChimes ), 1044050, 1030291, 80.0, 130.0, typeof( IronIngot ), 1044036, 15, 1044037 );

			#endregion

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			#region Jewelry

			int JewelTypes = 11;

			Type gemType = null;
			string gemName = "";
			int gemSkill = 0;

			while ( JewelTypes > 0 )
			{
				if ( JewelTypes == 10 ){			gemSkill = 5;		gemType = typeof( Amber );				gemName = "amber ";			}
				else if ( JewelTypes == 9 ){		gemSkill = 25;		gemType = typeof( Amethyst );			gemName = "amethyst ";		}
				else if ( JewelTypes == 8 ){		gemSkill = 10;		gemType = typeof( Citrine );			gemName = "citrine ";		}
				else if ( JewelTypes == 7 ){		gemSkill = 45;		gemType = typeof( Diamond );			gemName = "diamond ";		}
				else if ( JewelTypes == 6 ){		gemSkill = 30;		gemType = typeof( Emerald );			gemName = "emerald ";		}
				else if ( JewelTypes == 5 ){		gemSkill = 50;		gemType = typeof( Oyster );				gemName = "pearl ";			}
				else if ( JewelTypes == 4 ){		gemSkill = 15;		gemType = typeof( Ruby );				gemName = "ruby ";			}
				else if ( JewelTypes == 3 ){		gemSkill = 35;		gemType = typeof( Sapphire );			gemName = "sapphire ";		}
				else if ( JewelTypes == 2 ){		gemSkill = 40;		gemType = typeof( StarSapphire );		gemName = "star sapphire ";	}
				else if ( JewelTypes == 1 ){		gemSkill = 20;		gemType = typeof( Tourmaline );			gemName = "tourmaline ";	}

				TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

				index = AddCraft( typeof( JewelryBracelet ), 1044049, gemName + "bracelet", 40.0+gemSkill, 90.0+gemSkill, typeof( IronIngot ), 1044036, 2, 1044037 );
				if ( gemType != null ){ AddRes( index, gemType, cultInfo.ToTitleCase(gemName), 1, 1044240 ); }

				index = AddCraft( typeof( JewelryCirclet ), 1044049, gemName + "circlet", 40.0+gemSkill, 90.0+gemSkill, typeof( IronIngot ), 1044036, 3, 1044037 );
				if ( gemType != null ){ AddRes( index, gemType, cultInfo.ToTitleCase(gemName), 1, 1044240 ); }

				index = AddCraft( typeof( JewelryEarrings ), 1044049, gemName + "earrings", 40.0+gemSkill, 90.0+gemSkill, typeof( IronIngot ), 1044036, 2, 1044037 );
				if ( gemType != null ){ AddRes( index, gemType, cultInfo.ToTitleCase(gemName), 1, 1044240 ); }

				index = AddCraft( typeof( JewelryNecklace ), 1044049, gemName + "necklace", 40.0+gemSkill, 90.0+gemSkill, typeof( IronIngot ), 1044036, 3, 1044037 );
				if ( gemType != null ){ AddRes( index, gemType, cultInfo.ToTitleCase(gemName), 1, 1044240 ); }

				index = AddCraft( typeof( JewelryRing ), 1044049, gemName + "ring", 40.0+gemSkill, 90.0+gemSkill, typeof( IronIngot ), 1044036, 2, 1044037 );
				if ( gemType != null ){ AddRes( index, gemType, cultInfo.ToTitleCase(gemName), 1, 1044240 ); }

				JewelTypes--;
			}

			#endregion

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			#region Parts

			AddCraft( typeof( BarrelHoops ), 1044047, 1024321, -15.0, 35.0, typeof( IronIngot ), 1044036, 5, 1044037 );
			AddCraft( typeof( BarrelTap ), 1044047, 1024100, 35.0, 85.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( BolaBall ), 1044047, 1023699, 45.0, 95.0, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraft( typeof( ClockParts ), 1044047, 1024175, 25.0, 75.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( Gears ), 1044047, 1024179, 5.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( Hinge ), 1044047, 1024181, 5.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( SextantParts ), 1044047, 1024185, 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Springs ), 1044047, 1024189, 5.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );

			index = AddCraft( typeof( PowerCrystal ), 1044047, "power crystal", 45.0, 60.0, typeof( GoldIngot ), 1027146, 2, 1044037 );
			AddOrSkill( index, 40.0, 50.0, SkillName.Elementalism, SkillName.Magery, SkillName.Necromancy );
			AddRes( index, typeof( ArcaneGem ), "arcane gem", 3, 1053098 );

			#endregion

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			#region Tools

			AddCraft( typeof( CarpenterTools ), 1044046, "carpenter tools", 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( CulinarySet ), 1044046, "culinary set", 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( DruidCauldron ), 1044046, "druid's cauldron", 20.0, 70.0, typeof( IronIngot ), 1044036, 5, 1044037 );
			AddCraft( typeof( FletcherTools ), 1044046, 1044166, 35.0, 85.0, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraft( typeof( GraveSpade ), 1044046, "grave shovel", 35.0, 85.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Hatchet ), 1044046, 1023907, 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Lockpick ), 1044046, 1025371, 45.0, 95.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( MapmakersPen ), 1044046, 1044167, 25.0, 75.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( MortarPestle ), 1044046, 1023739, 20.0, 70.0, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraft( typeof( Pickaxe ), 1044046, 1023718, 40.0, 90.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( ScalingTools ), 1044046, "scaling tools", 35.0, 85.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Scissors ), 1044046, 1023998, 5.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( ScribesPen ), 1044046, 1044168, 25.0, 75.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( SewingKit ), 1044046, 1023997, 10.0, 70.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( Spade ), 1044046, 1023898, 40.0, 90.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( SkinningKnifeTool ), 1044046, "skinning knife", 15.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( SmithHammer ), 1044046, 1025091, 40.0, 90.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( LeatherworkingTools ), 1044046, "tanning tools", 10.0, 70.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( TinkerTools ), 1044046, 1044164, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( TrapKit ), 1044046, "trapping tools", 75.0, 110.0, typeof( IronIngot ), 1044036, 32, 1044037 );
			AddCraft( typeof( WaxingPot ), 1044046, "wax crafting pot", 20.0, 60.0, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraft( typeof( WitchCauldron ), 1044046, "witch's cauldron", 20.0, 70.0, typeof( IronIngot ), 1044036, 5, 1044037 );
			AddCraft( typeof( WoodworkingTools ), 1044046, "woodworking tools", 30.0, 80.0, typeof( IronIngot ), 1044036, 2, 1044037 );

			#endregion

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			#region Utensils

			AddCraft( typeof( ButcherKnife ), 1044048, 1025110, 25.0, 75.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( SpoonLeft ), 1044048, 1044158, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( SpoonRight ), 1044048, 1044159, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( Plate ), 1044048, 1022519, 0.0, 50.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( ForkLeft ), 1044048, 1044160, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( ForkRight ), 1044048, 1044161, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( Cleaver ), 1044048, 1023778, 20.0, 70.0, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraft( typeof( KnifeLeft ), 1044048, 1044162, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( KnifeRight ), 1044048, 1044163, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( Goblet ), 1044048, 1022458, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( PewterMug ), 1044048, 1024097, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( SkullMug ), 1044048, 1024091, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );

			#endregion

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			#region Wooden Items

			AddCraft( typeof( ClockFrame ), 1044042, 1024173, 0.0, 50.0, typeof( Board ), 1015101, 6, 1044351 );
			AddCraft( typeof( Axle ), 1044042, 1024187, -25.0, 25.0, typeof( Board ), 1015101, 2, 1044351 );

			index = AddCraft( typeof( SawMillSouthAddonDeed ), 1044042, "saw mill (south)", 60.0, 120.0, typeof( Granite ), 1044514, 80, 1044513 );
			AddSkill( index, SkillName.Lumberjacking, 75.0, 80.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 10, 1044037 );

			index = AddCraft( typeof( SawMillEastAddonDeed ), 1044042, "saw mill (east)", 60.0, 120.0, typeof( Granite ), 1044514, 80, 1044513 );
			AddSkill( index, SkillName.Lumberjacking, 75.0, 80.0 );
			AddRes( index, typeof( IronIngot ), 1044036, 10, 1044037 );

			index = AddCraft( typeof( Nunchaku ), 1044042, 1030158, 70.0, 120.0, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddRes( index, typeof( Board ), 1015101, 8, 1044351 );
			SetNeededExpansion( index, Expansion.SE );

			#endregion

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	
			// Set the overridable material
			SetSubRes( typeof( IronIngot ), CraftResources.GetClilocCraftName( CraftResource.Iron ) );

			int cannot = 1079593; // You have no idea how to work this metal.

			// Add every material you want the player to be able to choose from
			// This will override the overridable material
			AddSubRes( typeof( IronIngot ),			CraftResources.GetClilocCraftName( CraftResource.Iron ), CraftResources.GetSkill( CraftResource.Iron ), CraftResources.GetClilocMaterialName( CraftResource.Iron ), cannot );
			AddSubRes( typeof( DullCopperIngot ),	CraftResources.GetClilocCraftName( CraftResource.DullCopper ), CraftResources.GetSkill( CraftResource.DullCopper ), CraftResources.GetClilocMaterialName( CraftResource.DullCopper ), cannot );
			AddSubRes( typeof( ShadowIronIngot ),	CraftResources.GetClilocCraftName( CraftResource.ShadowIron ), CraftResources.GetSkill( CraftResource.ShadowIron ), CraftResources.GetClilocMaterialName( CraftResource.ShadowIron ), cannot );
			AddSubRes( typeof( CopperIngot ),		CraftResources.GetClilocCraftName( CraftResource.Copper ), CraftResources.GetSkill( CraftResource.Copper ), CraftResources.GetClilocMaterialName( CraftResource.Copper ), cannot );
			AddSubRes( typeof( BronzeIngot ),		CraftResources.GetClilocCraftName( CraftResource.Bronze ), CraftResources.GetSkill( CraftResource.Bronze ), CraftResources.GetClilocMaterialName( CraftResource.Bronze ), cannot );
			AddSubRes( typeof( GoldIngot ),			CraftResources.GetClilocCraftName( CraftResource.Gold ), CraftResources.GetSkill( CraftResource.Gold ), CraftResources.GetClilocMaterialName( CraftResource.Gold ), cannot );
			AddSubRes( typeof( AgapiteIngot ),		CraftResources.GetClilocCraftName( CraftResource.Agapite ), CraftResources.GetSkill( CraftResource.Agapite ), CraftResources.GetClilocMaterialName( CraftResource.Agapite ), cannot );
			AddSubRes( typeof( VeriteIngot ),		CraftResources.GetClilocCraftName( CraftResource.Verite ), CraftResources.GetSkill( CraftResource.Verite ), CraftResources.GetClilocMaterialName( CraftResource.Verite ), cannot );
			AddSubRes( typeof( ValoriteIngot ),		CraftResources.GetClilocCraftName( CraftResource.Valorite ), CraftResources.GetSkill( CraftResource.Valorite ), CraftResources.GetClilocMaterialName( CraftResource.Valorite ), cannot );
			AddSubRes( typeof( NepturiteIngot ),	CraftResources.GetClilocCraftName( CraftResource.Nepturite ), CraftResources.GetSkill( CraftResource.Nepturite ), CraftResources.GetClilocMaterialName( CraftResource.Nepturite ), cannot );
			AddSubRes( typeof( ObsidianIngot ),		CraftResources.GetClilocCraftName( CraftResource.Obsidian ), CraftResources.GetSkill( CraftResource.Obsidian ), CraftResources.GetClilocMaterialName( CraftResource.Obsidian ), cannot );
			AddSubRes( typeof( SteelIngot ),		CraftResources.GetClilocCraftName( CraftResource.Steel ), CraftResources.GetSkill( CraftResource.Steel ), CraftResources.GetClilocMaterialName( CraftResource.Steel ), cannot );
			AddSubRes( typeof( BrassIngot ),		CraftResources.GetClilocCraftName( CraftResource.Brass ), CraftResources.GetSkill( CraftResource.Brass ), CraftResources.GetClilocMaterialName( CraftResource.Brass ), cannot );
			AddSubRes( typeof( MithrilIngot ),		CraftResources.GetClilocCraftName( CraftResource.Mithril ), CraftResources.GetSkill( CraftResource.Mithril ), CraftResources.GetClilocMaterialName( CraftResource.Mithril ), cannot );
			AddSubRes( typeof( XormiteIngot ),		CraftResources.GetClilocCraftName( CraftResource.Xormite ), CraftResources.GetSkill( CraftResource.Xormite ), CraftResources.GetClilocMaterialName( CraftResource.Xormite ), cannot );
			AddSubRes( typeof( DwarvenIngot ),		CraftResources.GetClilocCraftName( CraftResource.Dwarven ), CraftResources.GetSkill( CraftResource.Dwarven ), CraftResources.GetClilocMaterialName( CraftResource.Dwarven ), cannot );
			// AddSubRes( typeof( AgriniumIngot ),		CraftResources.GetClilocCraftName( CraftResource.Agrinium ), CraftResources.GetSkill( CraftResource.Agrinium ), CraftResources.GetClilocMaterialName( CraftResource.Agrinium ), cannot );
			// AddSubRes( typeof( BeskarIngot ),		CraftResources.GetClilocCraftName( CraftResource.Beskar ), CraftResources.GetSkill( CraftResource.Beskar ), CraftResources.GetClilocMaterialName( CraftResource.Beskar ), cannot );
			// AddSubRes( typeof( CarboniteIngot ),	CraftResources.GetClilocCraftName( CraftResource.Carbonite ), CraftResources.GetSkill( CraftResource.Carbonite ), CraftResources.GetClilocMaterialName( CraftResource.Carbonite ), cannot );
			// AddSubRes( typeof( CortosisIngot ),		CraftResources.GetClilocCraftName( CraftResource.Cortosis ), CraftResources.GetSkill( CraftResource.Cortosis ), CraftResources.GetClilocMaterialName( CraftResource.Cortosis ), cannot );
			// AddSubRes( typeof( DurasteelIngot ),	CraftResources.GetClilocCraftName( CraftResource.Durasteel ), CraftResources.GetSkill( CraftResource.Durasteel ), CraftResources.GetClilocMaterialName( CraftResource.Durasteel ), cannot );
			// AddSubRes( typeof( DuriteIngot ),		CraftResources.GetClilocCraftName( CraftResource.Durite ), CraftResources.GetSkill( CraftResource.Durite ), CraftResources.GetClilocMaterialName( CraftResource.Durite ), cannot );
			// AddSubRes( typeof( FariumIngot ),		CraftResources.GetClilocCraftName( CraftResource.Farium ), CraftResources.GetSkill( CraftResource.Farium ), CraftResources.GetClilocMaterialName( CraftResource.Farium ), cannot );
			// AddSubRes( typeof( LaminasteelIngot ),	CraftResources.GetClilocCraftName( CraftResource.Laminasteel ), CraftResources.GetSkill( CraftResource.Laminasteel ), CraftResources.GetClilocMaterialName( CraftResource.Laminasteel ), cannot );
			// AddSubRes( typeof( NeuraniumIngot ),	CraftResources.GetClilocCraftName( CraftResource.Neuranium ), CraftResources.GetSkill( CraftResource.Neuranium ), CraftResources.GetClilocMaterialName( CraftResource.Neuranium ), cannot );
			// AddSubRes( typeof( PhrikIngot ),		CraftResources.GetClilocCraftName( CraftResource.Phrik ), CraftResources.GetSkill( CraftResource.Phrik ), CraftResources.GetClilocMaterialName( CraftResource.Phrik ), cannot );
			// AddSubRes( typeof( PromethiumIngot ),	CraftResources.GetClilocCraftName( CraftResource.Promethium ), CraftResources.GetSkill( CraftResource.Promethium ), CraftResources.GetClilocMaterialName( CraftResource.Promethium ), cannot );
			// AddSubRes( typeof( QuadraniumIngot ),	CraftResources.GetClilocCraftName( CraftResource.Quadranium ), CraftResources.GetSkill( CraftResource.Quadranium ), CraftResources.GetClilocMaterialName( CraftResource.Quadranium ), cannot );
			// AddSubRes( typeof( SongsteelIngot ),	CraftResources.GetClilocCraftName( CraftResource.Songsteel ), CraftResources.GetSkill( CraftResource.Songsteel ), CraftResources.GetClilocMaterialName( CraftResource.Songsteel ), cannot );
			// AddSubRes( typeof( TitaniumIngot ),		CraftResources.GetClilocCraftName( CraftResource.Titanium ), CraftResources.GetSkill( CraftResource.Titanium ), CraftResources.GetClilocMaterialName( CraftResource.Titanium ), cannot );
			// AddSubRes( typeof( TrimantiumIngot ),	CraftResources.GetClilocCraftName( CraftResource.Trimantium ), CraftResources.GetSkill( CraftResource.Trimantium ), CraftResources.GetClilocMaterialName( CraftResource.Trimantium ), cannot );
			// AddSubRes( typeof( XonoliteIngot ),		CraftResources.GetClilocCraftName( CraftResource.Xonolite ), CraftResources.GetSkill( CraftResource.Xonolite ), CraftResources.GetClilocMaterialName( CraftResource.Xonolite ), cannot );

			BreakDown = true;
			Repair = true;
			CanEnhance = true;
		}
	}

	public abstract class TrapCraft : CustomCraft
	{
		private LockableContainer m_Container;

		public LockableContainer Container{ get{ return m_Container; } }

		public abstract TrapType TrapType{ get; }

		public TrapCraft( Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality ) : base( from, craftItem, craftSystem, typeRes, tool, quality )
		{
		}

		private int Verify( LockableContainer container )
		{
			if ( container == null || container.KeyValue == 0 )
				return 1005638; // You can only trap lockable chests.
			if ( From.Map != container.Map || !From.InRange( container.GetWorldLocation(), 2 ) )
				return 500446; // That is too far away.
			if ( !container.Movable )
				return 502944; // You cannot trap this item because it is locked down.
			if ( !container.IsAccessibleTo( From ) )
				return 502946; // That belongs to someone else.
			if ( container.Locked )
				return 502943; // You can only trap an unlocked object.
			if ( container.TrapType != TrapType.None )
				return 502945; // You can only place one trap on an object at a time.

			return 0;
		}

		private bool Acquire( object target, out int message )
		{
			LockableContainer container = target as LockableContainer;

			message = Verify( container );

			if ( message > 0 )
			{
				return false;
			}
			else
			{
				m_Container = container;
				return true;
			}
		}

		public override void EndCraftAction()
		{
			From.SendLocalizedMessage( 502921 ); // What would you like to set a trap on?
			From.Target = new ContainerTarget( this );
		}

		private class ContainerTarget : Target
		{
			private TrapCraft m_TrapCraft;

			public ContainerTarget( TrapCraft trapCraft ) : base( -1, false, TargetFlags.None )
			{
				m_TrapCraft = trapCraft;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				int message;

				if ( m_TrapCraft.Acquire( targeted, out message ) )
					m_TrapCraft.CraftItem.CompleteCraft( m_TrapCraft.Quality, m_TrapCraft.From, m_TrapCraft.CraftSystem, m_TrapCraft.TypeRes, m_TrapCraft.Tool, m_TrapCraft, null );
				else
					Failure( message );
			}

			protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
			{
				if ( cancelType == TargetCancelType.Canceled )
					Failure( 0 );
			}

			private void Failure( int message )
			{
				Mobile from = m_TrapCraft.From;
				BaseTool tool = m_TrapCraft.Tool;

				if ( tool != null && !tool.Deleted && tool.UsesRemaining > 0 )
					from.SendGump( new CraftGump( from, m_TrapCraft.CraftSystem, tool, message ) );
				else if ( message > 0 )
					from.SendLocalizedMessage( message );
			}
		}

		public override Item CompleteCraft( out int message )
		{
			message = Verify( this.Container );

			if ( message == 0 )
			{
				int trapLevel = (int)(From.Skills.Tinkering.Value / 10);

				Container.TrapType = this.TrapType;
				Container.TrapPower = trapLevel * 9;
				Container.TrapLevel = trapLevel;
				Container.TrapOnLockpick = true;

				message = 1005639; // Trap is disabled until you lock the chest.
			}

			return null;
		}
	}

	[CraftItemID( 0x1BFC )]
	public class DartTrapCraft : TrapCraft
	{
		public override TrapType TrapType{ get{ return TrapType.DartTrap; } }

		public DartTrapCraft( Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality ) : base( from, craftItem, craftSystem, typeRes, tool, quality )
		{
		}
	}

	[CraftItemID( 0x113E )]
	public class PoisonTrapCraft : TrapCraft
	{
		public override TrapType TrapType{ get{ return TrapType.PoisonTrap; } }

		public PoisonTrapCraft( Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality ) : base( from, craftItem, craftSystem, typeRes, tool, quality )
		{
		}
	}

	[CraftItemID( 0x370C )]
	public class ExplosionTrapCraft : TrapCraft
	{
		public override TrapType TrapType{ get{ return TrapType.ExplosionTrap; } }

		public ExplosionTrapCraft( Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality ) : base( from, craftItem, craftSystem, typeRes, tool, quality )
		{
		}
	}
}