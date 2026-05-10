using System;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using System.Linq;

namespace Server.Engines.Craft
{
	public class DefBlacksmithy : CraftSystem
	{
		public override SkillName MainSkill
		{
			get	{ return SkillName.Blacksmith; }
		}

        public override int GumpImage
        {
            get { return 9603; }
        }

		public override int GumpTitleNumber
		{
			get { return 1044002; } // <CENTER>BLACKSMITHY MENU</CENTER>
		}

        public override bool ShowGumpInfo
        {
            get { return true; }
        }

		public override string CraftSystemTxt
		{
			get { return "Crafting: Smithing"; }
		}

		public override CraftResourceType BreakDownType
		{
			get { return CraftResourceType.Metal; }
		}

		private static DefBlacksmithy m_CraftSystem;

		public static DefBlacksmithy CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefBlacksmithy();

				return m_CraftSystem;
			}
		}

		public override CraftECA ECA{ get{ return CraftECA.ChanceMinusSixtyToFourtyFive; } }

		public override double GetChanceAtMin( CraftItem item )
		{
			return 0.0; // 0%
		}

		private DefBlacksmithy() : base( 1, 1, 1.25 )
		{
			/*
			
			base( MinCraftEffect, MaxCraftEffect, Delay )
			
			MinCraftEffect	: The minimum number of time the mobile will play the craft effect
			MaxCraftEffect	: The maximum number of time the mobile will play the craft effect
			Delay			: The delay between each craft effect
			
			Example: (3, 6, 1.7) would make the mobile do the PlayCraftEffect override
			function between 3 and 6 time, with a 1.7 second delay each time.
			
			*/ 
		}

		private static Type typeofAnvil = typeof( AnvilAttribute );
		private static Type typeofForge = typeof( ForgeAttribute );

		public static bool IsForge( object obj )
		{
			if ( Core.ML && obj is Mobile && ((Mobile)obj).IsDeadBondedPet )
				return false;

			if ( obj.GetType().IsDefined( typeof( ForgeAttribute ), false ) )
				return true;

			int itemID = 0;

			if ( obj is Item )
				itemID = ((Item)obj).ItemID;
			else if ( obj is StaticTarget )
				itemID = ((StaticTarget)obj).ItemID;

			if ( itemID >= 6896 && itemID <= 6898 )
			{
				if ( obj is FireGiantForge )
				{
					FireGiantForge kettle = (FireGiantForge)obj;
					Server.Items.FireGiantForge.ConsumeCharge( kettle );
					return true;
				}
			}

			return ( itemID == 4017 || (itemID >= 0x10DE && itemID <= 0x10E0) || (itemID >= 6522 && itemID <= 6569) || (itemID >= 0x544B && itemID <= 0x544E) );
		}

		public static void CheckAnvilAndForge( Mobile from, int range, out bool anvil, out bool forge )
		{
			anvil = false;
			forge = false;

			Map map = from.Map;

			if ( map == null )
				return;

			IPooledEnumerable eable = map.GetItemsInRange( from.Location, range );

			foreach ( Item item in eable )
			{
				Type type = item.GetType();

				bool isAnvil = ( type.IsDefined( typeofAnvil, false ) || item.ItemID == 4015 || item.ItemID == 4016 || item.ItemID == 0x2DD5 || item.ItemID == 0x2DD6 || item.ItemID == 0x2B55 || item.ItemID == 0x2B57 || item.ItemID == 0x64ED|| item.ItemID == 0x64EE|| item.ItemID == 0x64EF|| item.ItemID == 0x64F0|| item.ItemID == 0x64F1|| item.ItemID == 0x64F2|| item.ItemID == 0x64F3|| item.ItemID == 0x64F4|| item.ItemID == 0x64F5|| item.ItemID == 0x64F6|| item.ItemID == 0x64F7|| item.ItemID == 0x64F8|| item.ItemID == 0x64F9|| item.ItemID == 0x64FA|| item.ItemID == 0x64FB|| item.ItemID == 0x64FC|| item.ItemID == 0x64FD|| item.ItemID == 0x64FE|| item.ItemID == 0x64FF|| item.ItemID == 0x6500|| item.ItemID == 0x6501|| item.ItemID == 0x6502|| item.ItemID == 0x6503|| item.ItemID == 0x6504|| item.ItemID == 0x6505|| item.ItemID == 0x6506|| item.ItemID == 0x6507|| item.ItemID == 0x6508|| item.ItemID == 0x6509|| item.ItemID == 0x650A|| item.ItemID == 0x650B|| item.ItemID == 0x650C|| item.ItemID == 0x650D|| item.ItemID == 0x650E|| item.ItemID == 0x650F|| item.ItemID == 0x6510|| item.ItemID == 0x6511|| item.ItemID == 0x6512|| item.ItemID == 0x6513|| item.ItemID == 0x6514|| item.ItemID == 0x6515|| item.ItemID == 0x6516|| item.ItemID == 0x6517|| item.ItemID == 0x6518 );
				bool isForge = ( type.IsDefined( typeofForge, false ) || item.ItemID == 4017 || (item.ItemID >= 0x10DE && item.ItemID <= 0x10E0) || (item.ItemID >= 6522 && item.ItemID <= 6569) || item.ItemID == 0x2DD8 || (item.ItemID >= 0x544B && item.ItemID <= 0x544E) );

				if ( isAnvil || isForge )
				{
					if ( (from.Z + 16) < item.Z || (item.Z + 16) < from.Z || !from.InLOS( item ) )
						continue;

					anvil = anvil || isAnvil;
					forge = forge || isForge;

					if ( anvil && forge )
						break;
				}
			}

			eable.Free();

			for ( int x = -range; (!anvil || !forge) && x <= range; ++x )
			{
				for ( int y = -range; (!anvil || !forge) && y <= range; ++y )
				{
					StaticTile[] tiles = map.Tiles.GetStaticTiles( from.X+x, from.Y+y, true );

					for ( int i = 0; (!anvil || !forge) && i < tiles.Length; ++i )
					{
						int id = tiles[i].ID;

						bool isAnvil = ( id == 4015 || id == 4016 || id == 0x2DD5 || id == 0x2DD6 || id == 0x2B55 || id == 0x2B57 || id == 0x64ED || id == 0x64EE || id == 0x64EF || id == 0x64F0 || id == 0x64F1 || id == 0x64F2 || id == 0x64F3 || id == 0x64F4 || id == 0x64F5 || id == 0x64F6 || id == 0x64F7 || id == 0x64F8 || id == 0x64F9 || id == 0x64FA || id == 0x64FB || id == 0x64FC || id == 0x64FD || id == 0x64FE || id == 0x64FF || id == 0x6500 || id == 0x6501 || id == 0x6502 || id == 0x6503 || id == 0x6504 || id == 0x6505 || id == 0x6506 || id == 0x6507 || id == 0x6508 || id == 0x6509 || id == 0x650A || id == 0x650B || id == 0x650C || id == 0x650D || id == 0x650E || id == 0x650F || id == 0x6510 || id == 0x6511 || id == 0x6512 || id == 0x6513 || id == 0x6514 || id == 0x6515 || id == 0x6516 || id == 0x6517 || id == 0x6518 );
						bool isForge = ( id == 4017 || (id >= 0x10DE && id <= 0x10E0) || (id >= 6522 && id <= 6569) || id == 0x2DD8 || (id >= 0x544B && id <= 0x544E) );

						if ( isAnvil || isForge )
						{
							if ( (from.Z + 16) < tiles[i].Z || (tiles[i].Z + 16) < from.Z || !from.InLOS( new Point3D( from.X+x, from.Y+y, tiles[i].Z + (tiles[i].Height/2) + 1 ) ) )
								continue;

							anvil = anvil || isAnvil;
							forge = forge || isForge;
						}
					}
				}
			}
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if ( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
			else if ( !BaseTool.CheckTool( tool, from ) )
				return 1048146; // If you have a tool equipped, you must use that tool.
			else if ( !BaseTool.CheckAccessible( tool, from ) )
				return 1044263; // The tool must be on your person to use.

			bool anvil, forge;
			CheckAnvilAndForge( from, 2, out anvil, out forge );

			if ( anvil && forge )
				return 0;

			return 1044267; // You must be near an anvil and a forge to smith items.
		}

		public override void PlayCraftEffect( Mobile from )
		{
			from.Animate( 11, 5, 1, true, false, 0 );
			CraftSound( from, 0x541, m_Tools );
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
			/*
			Synthax for a SIMPLE craft item
			AddCraft( ObjectType, Group, MinSkill, MaxSkill, ResourceType, Amount, Message )
			
			ObjectType		: The type of the object you want to add to the build list.
			Group			: The group in wich the object will be showed in the craft menu.
			MinSkill		: The minimum of skill value
			MaxSkill		: The maximum of skill value
			ResourceType	: The type of the resource the mobile need to create the item
			Amount			: The amount of the ResourceType it need to create the item
			Message			: String or Int for Localized.  The message that will be sent to the mobile, if the specified resource is missing.
			
			Synthax for a COMPLEXE craft item.  A complexe item is an item that need either more than
			only one skill, or more than only one resource.
			
			Coming soon....
			*/

			#region Chainmail
			AddCraftRecipe( 1, typeof( ChainCoif ), "Chain/Ringmail", 1025051, 14.5, 64.5, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraftRecipe( 2, typeof( ChainLegs ), "Chain/Ringmail", 1025054, 36.7, 86.7, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraftRecipe( 3, typeof( ChainChest ), "Chain/Ringmail", 1025055, 39.1, 89.1, typeof( IronIngot ), 1044036, 20, 1044037 );
			AddCraftRecipe( 4, typeof( ChainSkirt ), "Chain/Ringmail", "chainmail skirt", 36.7, 86.7, typeof( IronIngot ), 1044036, 18, 1044037 );
			#endregion

			#region Ringmail
			AddCraftRecipe( 5, typeof( RingmailGloves ), "Chain/Ringmail", 1025099, 12.0, 62.0, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraftRecipe( 6, typeof( RingmailLegs ), "Chain/Ringmail", 1025104, 19.4, 69.4, typeof( IronIngot ), 1044036, 16, 1044037 );
			AddCraftRecipe( 7, typeof( RingmailArms ), "Chain/Ringmail", 1025103, 16.9, 66.9, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraftRecipe( 8, typeof( RingmailChest ), "Chain/Ringmail", 1025100, 21.9, 71.9, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraftRecipe( 9, typeof( RingmailSkirt ), "Chain/Ringmail", "ringmail skirt", 19.4, 69.4, typeof( IronIngot ), 1044036, 16, 1044037 );
			#endregion

			int index = -1;

			#region Platemail
			AddCraftRecipe( 10, typeof( PlateArms ), "Platemail", 1025136, 66.3, 116.3, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraftRecipe( 11, typeof( PlateGloves ), "Platemail", 1025140, 58.9, 108.9, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 12, typeof( PlateGorget ), "Platemail", 1025139, 56.4, 106.4, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraftRecipe( 13, typeof( PlateLegs ), "Platemail", 1025137, 68.8, 118.8, typeof( IronIngot ), 1044036, 20, 1044037 );
			AddCraftRecipe( 14, typeof( PlateSkirt ), "Platemail", "platemail skirt", 68.8, 118.8, typeof( IronIngot ), 1044036, 20, 1044037 );
			AddCraftRecipe( 15, typeof( PlateChest ), "Platemail", 1046431, 75.0, 125.0, typeof( IronIngot ), 1044036, 25, 1044037 );
			AddCraftRecipe( 16, typeof( FemalePlateChest ), "Platemail", 1046430, 44.1, 94.1, typeof( IronIngot ), 1044036, 20, 1044037 );

			AddCraftRecipe( 17, typeof( DragonBardingDeed ), "Platemail", 1053012, 72.5, 122.5, typeof( IronIngot ), 1044036, 750, 1044037 );
			AddCraftRecipe( 18, typeof( HorseArmor ), "Platemail", "horse barding", 72.5, 122.5, typeof( IronIngot ), 1044036, 650, 1044037 );

			AddCraftRecipe( 19, typeof( PlateMempo ), "Platemail", 1030180, 80.0, 130.0, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraftRecipe( 20, typeof( PlateDo ), "Platemail", 1030184, 80.0, 130.0, typeof( IronIngot ), 1044036, 28, 1044037 );
			AddCraftRecipe( 21, typeof( PlateHiroSode ), "Platemail", 1030187, 80.0, 130.0, typeof( IronIngot ), 1044036, 16, 1044037 );
			AddCraftRecipe( 22, typeof( PlateSuneate ), "Platemail", 1030195, 65.0, 115.0, typeof( IronIngot ), 1044036, 20, 1044037 );
			AddCraftRecipe( 23, typeof( PlateHaidate ), "Platemail", 1030200, 65.0, 115.0, typeof( IronIngot ), 1044036, 20, 1044037 );

			#endregion

			#region Royal
			AddCraftRecipe( 24, typeof( RoyalBoots ), "Royal Armor", "royal boots", 88.9, 118.9, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 25, typeof( RoyalGloves ), "Royal Armor", "royal bracers", 88.9, 118.9, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 26, typeof( RoyalGorget ), "Royal Armor", "royal gorget", 86.4, 116.4, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraftRecipe( 27, typeof( RoyalHelm ), "Royal Armor", "royal helm", 92.6, 122.6, typeof( IronIngot ), 1044036, 15, 1044037 );
			AddCraftRecipe( 28, typeof( RoyalsLegs ), "Royal Armor", "royal leggings", 96.8, 125.0, typeof( IronIngot ), 1044036, 20, 1044037 );
			AddCraftRecipe( 29, typeof( RoyalArms ), "Royal Armor", "royal mantle", 96.3, 125.0, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraftRecipe( 30, typeof( RoyalChest ), "Royal Armor", "royal tunic", 98.0, 125.0, typeof( IronIngot ), 1044036, 25, 1044037 );
			#endregion

			#region Helmets
			AddCraft( typeof( Bascinet ), "Helmets", 1025132, 8.3, 58.3, typeof( IronIngot ), 1044036, 15, 1044037 );
			AddCraft( typeof( CloseHelm ), "Helmets", 1025128, 37.9, 87.9, typeof( IronIngot ), 1044036, 15, 1044037 );
			AddCraft( typeof( Helmet ), "Helmets", 1025130, 37.9, 87.9, typeof( IronIngot ), 1044036, 15, 1044037 );
			AddCraft( typeof( NorseHelm ), "Helmets", 1025134, 37.9, 87.9, typeof( IronIngot ), 1044036, 15, 1044037 );
			AddCraft( typeof( PlateHelm ), "Helmets", 1025138, 62.6, 112.6, typeof( IronIngot ), 1044036, 15, 1044037 );
			AddCraft( typeof( DreadHelm ), "Helmets", "dread helm", 62.6, 112.6, typeof( IronIngot ), 1044036, 15, 1044037 );
			
			if( Core.SE )
			{
				AddCraft( typeof( ChainHatsuburi ), "Helmets", 1030175, 30.0, 80.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				AddCraft( typeof( PlateHatsuburi ), "Helmets", 1030176, 45.0, 95.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				AddCraft( typeof( HeavyPlateJingasa ), "Helmets", 1030178, 45.0, 95.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				AddCraft( typeof( LightPlateJingasa ), "Helmets", 1030188, 45.0, 95.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				AddCraft( typeof( SmallPlateJingasa ), "Helmets", 1030191, 45.0, 95.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				AddCraft( typeof( DecorativePlateKabuto ), "Helmets", 1030179, 90.0, 140.0, typeof( IronIngot ), 1044036, 25, 1044037 );
				AddCraft( typeof( PlateBattleKabuto ), "Helmets", 1030192, 90.0, 140.0, typeof( IronIngot ), 1044036, 25, 1044037 );
				AddCraft( typeof( StandardPlateKabuto ), "Helmets", 1030196, 90.0, 140.0, typeof( IronIngot ), 1044036, 25, 1044037 );
			}
			#endregion

			#region Shields
			AddCraft( typeof( Buckler ), "Shields", 1027027, -25.0, 25.0, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraft( typeof( BronzeShield ), "Shields", "large shield", -15.2, 34.8, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraft( typeof( WoodenKiteShield ), "Shields", 1027032, -15.2, 34.8, typeof( IronIngot ), 1044036, 8, 1044037 );
			AddCraft( typeof( MetalShield ), "Shields", 1027035, -10.2, 39.8, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraft( typeof( MetalKiteShield ), "Shields", 1027028, 4.6, 54.6, typeof( IronIngot ), 1044036, 16, 1044037 );
			AddCraft( typeof( HeaterShield ), "Shields", 1027030, 24.3, 74.3, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraft( typeof( ChampionShield ), "Shields", "champion shield", 54.3, 84.3, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraft( typeof( CrestedShield ), "Shields", "crested shield", 54.3, 84.3, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraft( typeof( DarkShield ), "Shields", "dark shield", 54.3, 84.3, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraft( typeof( ElvenShield ), "Shields", "elven shield", 54.3, 84.3, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraft( typeof( GuardsmanShield ), "Shields", "guardsman shield", 54.3, 84.3, typeof( IronIngot ), 1044036, 18, 1044037 );
			index = AddCraft( typeof( JeweledShield ), "Shields", "jeweled shield", 54.3, 84.3, typeof( IronIngot ), 1044036, 18, 1044037 );
				AddRes( index, typeof( StarSapphire ), 1023855, 1, 1044037 );
			AddCraft( typeof( RoyalShield ), "Shields", "royal shield", 54.3, 84.3, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraft( typeof( ChaosShield ), "Shields", "chaos shield", 85.0, 135.0, typeof( IronIngot ), 1044036, 25, 1044037 );
			AddCraft( typeof( OrderShield ), "Shields", "order shield", 85.0, 135.0, typeof( IronIngot ), 1044036, 25, 1044037 );
			AddCraft( typeof( SunShield ), "Shields", "sun shield", 85.0, 135.0, typeof( IronIngot ), 1044036, 25, 1044037 );
			AddCraft( typeof( VirtueShield ), "Shields", "virtue shield", 85.0, 135.0, typeof( IronIngot ), 1044036, 25, 1044037 );
			#endregion

			#region Bladed
			AddCraftRecipe( 61, typeof( AssassinSpike ), "Bladed", "assassin dagger", 10.0, 49.6, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraftRecipe( 62, typeof( ElvenSpellblade ), "Bladed", "assassin sword", 44.1, 94.1, typeof( IronIngot ), 1044036, 8, 1044037 );
			AddCraftRecipe( 63, typeof( VikingSword ), "Bladed", "barbarian sword", 24.3, 74.3, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraftRecipe( 64, typeof( Broadsword ), "Bladed", 1023934, 35.4, 85.4, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraftRecipe( 65, typeof( Claymore ), "Bladed", "claymore", 34.3, 84.3, typeof( IronIngot ), 1044036, 16, 1044037 );
			AddCraftRecipe( 66, typeof( CrescentBlade ), "Bladed", 1029921, 45.0, 95.0, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraftRecipe( 67, typeof( Cutlass ), "Bladed", 1025185, 24.3, 74.3, typeof( IronIngot ), 1044036, 8, 1044037 );
			AddCraftRecipe( 68, typeof( Dagger ), "Bladed", 1023921, -0.4, 49.6, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraftRecipe( 69, typeof( RadiantScimitar ), "Bladed", "falchion", 35.4, 85.4, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraftRecipe( 70, typeof( Katana ),"Bladed", 1025119, 44.1, 94.1, typeof( IronIngot ), 1044036, 8, 1044037 );
			AddCraftRecipe( 71, typeof( Kryss ), "Bladed", 1025121, 36.7, 86.7, typeof( IronIngot ), 1044036, 8, 1044037 );
			AddCraftRecipe( 72, typeof( LargeKnife ), "Bladed", "large knife", -0.4, 49.6, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraftRecipe( 73, typeof( Longsword ), "Bladed", 1023937, 28.0, 78.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 74, typeof( ElvenMachete ), "Bladed", "machete", 33.0, 83.0, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraftRecipe( 75, typeof( ShortSpear ), "Bladed", "rapier", 45.3, 95.3, typeof( IronIngot ), 1044036, 6, 1044037 );
			AddCraftRecipe( 76, typeof( RoyalSword ), "Bladed", "royal sword", 54.3, 84.3, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraftRecipe( 77, typeof( ShortSword ), "Bladed", "short sword", 33.0, 83.0, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraftRecipe( 78, typeof( BoneHarvester ), "Bladed", "sickle", 33.0, 83.0, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraftRecipe( 79, typeof( Scimitar ), "Bladed", 1025046, 31.7, 81.7, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraftRecipe( 80, typeof( RuneBlade ), "Bladed", "war blades", 28.0, 78.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 81, typeof( WarCleaver ), "Bladed", "war cleaver", 10.0, 49.6, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraftRecipe( 82, typeof( Leafblade ), "Bladed", "war dagger", 20.0, 59.6, typeof( IronIngot ), 1044036, 5, 1044037 );
			AddCraftRecipe( 83, typeof( NoDachi ), "Bladed", 1030221, 75.0, 125.0, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraftRecipe( 84, typeof( Wakizashi ), "Bladed", 1030223, 50.0, 100.0, typeof( IronIngot ), 1044036, 8, 1044037 );
			AddCraftRecipe( 85, typeof( Lajatang ), "Bladed", 1030226, 80.0, 130.0, typeof( IronIngot ), 1044036, 25, 1044037 );
			AddCraftRecipe( 86, typeof( Daisho ), "Bladed", 1030228, 60.0, 110.0, typeof( IronIngot ), 1044036, 15, 1044037 );
			AddCraftRecipe( 87, typeof( Tekagi ), "Bladed", 1030230, 55.0, 105.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 88, typeof( Shuriken ), "Bladed", 1030231, 45.0, 95.0, typeof( IronIngot ), 1044036, 5, 1044037 );
			AddCraftRecipe( 89, typeof( Kama ), "Bladed", 1030232, 40.0, 90.0, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraftRecipe( 90, typeof( Sai ), "Bladed", 1030234, 50.0, 100.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			#endregion

			#region Axes
			AddCraftRecipe( 91, typeof( Axe ), "Axes", 1023913, 34.2, 84.2, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraftRecipe( 92, typeof( BattleAxe ), "Axes", 1023911, 30.5, 80.5, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraftRecipe( 93, typeof( DoubleAxe ), "Axes", 1023915, 29.3, 79.3, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 94, typeof( ExecutionersAxe ), "Axes", "great axe", 34.2, 84.2, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraftRecipe( 95, typeof( LargeBattleAxe ), "Axes", 1025115, 28.0, 78.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 96, typeof( TwoHandedAxe ), "Axes", 1025187, 33.0, 83.0, typeof( IronIngot ), 1044036, 16, 1044037 );
			AddCraftRecipe( 97, typeof( WarAxe ), "Axes", 1025040, 39.1, 89.1, typeof( IronIngot ), 1044036, 16, 1044037 );
			AddCraftRecipe( 98, typeof( OrnateAxe ), "Axes", "barbarian axe", 24.3, 74.3, typeof( IronIngot ), 1044036, 16, 1044037 );
			#endregion

			#region Pole Arms
			AddCraftRecipe( 99, typeof( Bardiche ), "Polearms", 1023917, 31.7, 81.7, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraftRecipe( 100, typeof( BladedStaff ), "Polearms", 1029917, 40.0, 90.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 101, typeof( DoubleBladedStaff ), "Polearms", 1029919, 45.0, 95.0, typeof( IronIngot ), 1044036, 16, 1044037 );
			AddCraftRecipe( 102, typeof( Halberd ), "Polearms", 1025183, 39.1, 89.1, typeof( IronIngot ), 1044036, 20, 1044037 );
			AddCraftRecipe( 103, typeof( Harpoon ), "Polearms", "harpoon", 30.0, 70.0, typeof( IronIngot ), 1015101, 12, 1044351 );
			AddCraftRecipe( 104, typeof( Lance ), "Polearms", 1029920, 48.0, 98.0, typeof( IronIngot ), 1044036, 20, 1044037 );
			AddCraftRecipe( 105, typeof( Pike ), "Polearms", 1029918, 47.0, 97.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 106, typeof( Pitchforks ), "Polearms", "pitchfork", 45.0, 95.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 107, typeof( Scythe ), "Polearms", 1029914, 39.0, 89.0, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraftRecipe( 108, typeof( Spear ), "Polearms", 1023938, 49.0, 99.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 109, typeof( Pitchfork ), "Polearms", "trident", 45.0, 95.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraftRecipe( 110, typeof( WarFork ), "Polearms", 1025125, 42.9, 92.9, typeof( IronIngot ), 1044036, 12, 1044037 );
			#endregion

			#region Bashing
			AddCraftRecipe( 111, typeof( DiamondMace ), "Bashing", "battle mace", 28.0, 78.0, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraftRecipe( 112, typeof( Hammers ), "Bashing", "hammer", 14.5, 64.5, typeof( IronIngot ), 1044036, 6, 1044037 );
			AddCraftRecipe( 113, typeof( HammerPick ), "Bashing", 1025181, 34.2, 84.2, typeof( IronIngot ), 1044036, 16, 1044037 );
			AddCraftRecipe( 114, typeof( Mace ), "Bashing", 1023932, 14.5, 64.5, typeof( IronIngot ), 1044036, 6, 1044037 );
			AddCraftRecipe( 115, typeof( Maul ), "Bashing", 1025179, 19.4, 69.4, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraftRecipe( 116, typeof( Scepter ), "Bashing", 1029916, 21.4, 71.4, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraftRecipe( 117, typeof( SpikedClub ), "Bashing", "spiked club", 14.5, 64.5, typeof( IronIngot ), 1044036, 6, 1044037 );
			AddCraftRecipe( 118, typeof( WarMace ), "Bashing", 1025127, 28.0, 78.0, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraftRecipe( 119, typeof( WarHammer ), "Bashing", 1025177, 34.2, 84.2, typeof( IronIngot ), 1044036, 16, 1044037 );
			index = AddCraftRecipe( 120, typeof( Tessen ), "Bashing", 1030222, 85.0, 135.0, typeof( IronIngot ), 1044036, 16, 1044037 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Fabric ), 1044286, 10, 1044287 );

			#endregion
			
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

		public bool NeedsWeaponRecipe(PlayerMobile player)
		{
			return GetRecipes().Any(id =>
				{
					var recipe = Recipe.Recipes[id];

					return !player.HasRecipe(id) && IsWeaponRecipe(recipe);
				}
			);
		}

		public bool IsWeaponRecipe(Recipe recipe)
		{
			return typeof(BaseWeapon).IsAssignableFrom(recipe.CraftItem.ItemType) || recipe.CraftItem.ItemType == typeof(Shuriken);
		}
	}

	public class ForgeAttribute : Attribute
	{
		public ForgeAttribute()
		{
		}
	}

	public class AnvilAttribute : Attribute
	{
		public AnvilAttribute()
		{
		}
	}
}