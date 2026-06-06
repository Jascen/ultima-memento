using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

namespace Server.Engines.Craft
{
	public enum CraftECA
	{
		ChanceMinusSixty,
		FiftyPercentChanceMinusTenPercent,
		ChanceMinusSixtyToFourtyFive
	}

	public abstract class CraftSystem
	{
		private int m_MinCraftEffect;
		private int m_MaxCraftEffect;
		private double m_Delay;
		private bool m_BreakDown;
		private bool m_Repair;
		private bool m_CanEnhance;
		public BaseTool m_Tools;

		private CraftItemCol m_CraftItems;
		private CraftGroupCol m_CraftGroups;
		private CraftSubResCol m_CraftSubRes;

		private List<int> m_Recipes;
		private List<int> m_RareRecipes;

		public int MinCraftEffect { get { return m_MinCraftEffect; } }
		public int MaxCraftEffect { get { return m_MaxCraftEffect; } }
		public double Delay { get { return m_Delay; } }

		public CraftItemCol CraftItems{ get { return m_CraftItems; } }
		public CraftGroupCol CraftGroups{ get { return m_CraftGroups; } }
		public CraftSubResCol CraftSubRes{ get { return m_CraftSubRes; } }
		
		public abstract SkillName MainSkill{ get; }

		public virtual int GumpImage{ get{ return 0; } }
		public virtual int GumpTitleNumber{ get{ return 0; } }
		public virtual string GumpTitleString{ get{ return ""; } }
		public virtual string CraftSystemTxt{ get{ return ""; } }
		public virtual bool ShowGumpInfo{ get{ return true; } }
		public virtual CraftResourceType BreakDownType{ get{ return CraftResourceType.Metal; } }
		public virtual CraftResourceType BreakDownTypeAlt{ get{ return CraftResourceType.None; } }

		public virtual CraftECA ECA{ get{ return CraftECA.ChanceMinusSixty; } }

		private Dictionary<Mobile, CraftContext> m_ContextTable = new Dictionary<Mobile, CraftContext>();

		public abstract double GetChanceAtMin( CraftItem item );

		public static bool AllowManyCraft( BaseTool tool )
		{
			if ( tool != null && ( tool is BaseRunicTool || tool is TomeOfWands ) )
				return false;

			return MySettings.S_CraftMany;
		}

		public static void SetCraftResource( CraftContext context, int index, CraftSubRes res )
		{
			if ( context != null && index > -1 && res != null )
			{
				context.LastResourceIndex = index;

				Type resType = res.ItemType;

				Item tempRes = (Item)Activator.CreateInstance(resType);

				context.CraftingResource = CraftResource.None;
				if ( tempRes is BaseIngot )
					context.CraftingResource = ((BaseIngot)tempRes).Resource;
				else if ( tempRes is BaseFabric )
					context.CraftingResource = ((BaseFabric)tempRes).Resource;
				else if ( tempRes is BaseScales )
					context.CraftingResource = ((BaseScales)tempRes).Resource;
				else if ( tempRes is BaseLeather )
					context.CraftingResource = ((BaseLeather)tempRes).Resource;
				else if ( tempRes is BaseWoodBoard )
					context.CraftingResource = ((BaseWoodBoard)tempRes).Resource;
				else if ( tempRes is BaseLog )
					context.CraftingResource = ((BaseLog)tempRes).Resource;
				else if ( tempRes is BaseHides )
					context.CraftingResource = ((BaseHides)tempRes).Resource;
				else if ( tempRes is BaseOre )
					context.CraftingResource = ((BaseOre)tempRes).Resource;
				else if ( tempRes is BaseBlocks )
					context.CraftingResource = ((BaseBlocks)tempRes).Resource;
				else if ( tempRes is BaseSkins )
					context.CraftingResource = ((BaseSkins)tempRes).Resource;
				else if ( tempRes is BaseSkeletal )
					context.CraftingResource = ((BaseSkeletal)tempRes).Resource;
				else if ( tempRes is BaseSpecial )
					context.CraftingResource = ((BaseSpecial)tempRes).Resource;

				tempRes.Delete();
			}
		}

		public static void SetDescription( CraftContext context, BaseTool tool, Type type, CraftSystem system, string nameString, Mobile from, CraftItem cItem )
		{
			int typeHue = 0;

			if ( type != null && context != null )
			{
				context.ItemSelected = type;
				context.NameString = nameString;

				CraftResource resource = context.CraftingResource;

				if ( system is DefDraconic && !( resource >= CraftResource.RedScales && resource <= CraftResource.KraytScales ) )
				{
					resource = CraftResource.RedScales;
					context.CraftingResource = CraftResource.RedScales;
				}
				else if ( system is DefLapidary && !( resource >= CraftResource.AmethystBlock && resource <= CraftResource.CaddelliteBlock ) )
				{
					resource = CraftResource.AmethystBlock;
					context.CraftingResource = CraftResource.AmethystBlock;
				}
				else if ( system is DefStitching && !( resource >= CraftResource.DemonSkin && resource <= CraftResource.DeadSkin ) )
				{
					resource = CraftResource.DemonSkin;
					context.CraftingResource = CraftResource.DemonSkin;
				}

				if ( tool is BaseRunicTool )
					resource = ((BaseRunicTool)tool).Resource;

				Item descriptor = (Item)Activator.CreateInstance(type);
				ResourceMods.DefaultItemHue( descriptor );

				if ( descriptor.Catalog == Catalogs.Jewelry && descriptor is BaseTrinket )
				{
					if ( nameString.Contains("star sapphire") ){ ((BaseTrinket)descriptor).GemType = GemType.StarSapphire; }
					else if ( nameString.Contains("emerald") ){ ((BaseTrinket)descriptor).GemType = GemType.Emerald; }
					else if ( nameString.Contains("sapphire") ){ ((BaseTrinket)descriptor).GemType = GemType.Sapphire; }
					else if ( nameString.Contains("ruby") ){ ((BaseTrinket)descriptor).GemType = GemType.Ruby; }
					else if ( nameString.Contains("citrine") ){ ((BaseTrinket)descriptor).GemType = GemType.Citrine; }
					else if ( nameString.Contains("amethyst") ){ ((BaseTrinket)descriptor).GemType = GemType.Amethyst; }
					else if ( nameString.Contains("tourmaline") ){ ((BaseTrinket)descriptor).GemType = GemType.Tourmaline; }
					else if ( nameString.Contains("amber") ){ ((BaseTrinket)descriptor).GemType = GemType.Amber; }
					else if ( nameString.Contains("diamond") ){ ((BaseTrinket)descriptor).GemType = GemType.Diamond; }
					else if ( nameString.Contains("pearl") ){ ((BaseTrinket)descriptor).GemType = GemType.Pearl; }
				}
				else if ( descriptor.Catalog == Catalogs.Trinket && descriptor.NotIDSource == Identity.Wand && descriptor is BaseTrinket )
				{
					descriptor.Delete();
					descriptor = new MagicalWand( SpellItems.GetWand( nameString ) );
				}

				typeHue = descriptor.Hue;

				descriptor.Resource = resource;

				if ( typeHue < 1 || ( typeHue != descriptor.Hue && descriptor.Hue > 0 ) || system is DefDraconic || system is DefLapidary || system is DefStitching )
					typeHue = descriptor.Hue;

				string description = ItemProps.ItemProperties( descriptor, true );

				if ( tool is BaseRunicTool )
					description += "Potentially Magical";

				context.Description = description;
				context.ItemID = descriptor.ItemID;

				if ( cItem != null )
					system.OnMade( from, cItem );

				descriptor.Delete();
			}

			context.Hue = typeHue;
		}

		public static void CraftSound( Mobile m, int sound, BaseTool tool )
		{
			if ( m is PlayerMobile )
			{
				m.PlaySound( sound );
			}
		}

		public virtual bool RetainsColorFrom( CraftItem item, Type type )
		{
			return false;
		}

		public CraftContext GetContext( Mobile m )
		{
			if ( m == null )
				return null;

			if ( m.Deleted )
			{
				m_ContextTable.Remove( m );
				return null;
			}

			CraftContext c = null;
			m_ContextTable.TryGetValue( m, out c );

			if ( c == null )
				m_ContextTable[m] = c = new CraftContext();

			return c;
		}

		public void OnMade( Mobile m, CraftItem item )
		{
			CraftContext c = GetContext( m );

			if ( c != null )
				c.OnMade( item );
		}

		public BaseTool Tools
		{
			get { return m_Tools; }
			set { m_Tools = value; }
		}

		public bool BreakDown
		{
			get { return m_BreakDown; }
			set { m_BreakDown = value; }
		}

		public bool Repair
		{
			get{ return m_Repair; }
			set{ m_Repair = value; }
		}

		public bool CanEnhance
		{
			get{ return m_CanEnhance; }
			set{ m_CanEnhance = value; }
		}

		public CraftSystem( int minCraftEffect, int maxCraftEffect, double delay )
		{
			m_MinCraftEffect = minCraftEffect;
			m_MaxCraftEffect = maxCraftEffect;
			m_Delay = delay;

			m_CraftItems = new CraftItemCol();
			m_CraftGroups = new CraftGroupCol();
			m_CraftSubRes = new CraftSubResCol();
			m_Recipes = new List<int>();
			m_RareRecipes = new List<int>();

			InitCraftList();
		}

		public virtual bool ConsumeOnFailure( Mobile from, Type resourceType, CraftItem craftItem )
		{
			return true;
		}

		public void BulkCreateItem( Mobile from, Type type, Type typeRes, BaseTool tool, CraftItem realCraftItem, int amount )
		{	
			// Verify if the type is in the list of the craftable item
			CraftItem craftItem = m_CraftItems.SearchFor( type );
			if ( craftItem != null )
			{
				var player = from as PlayerMobile;
				if ( player == null ) return;

				var timer = new BulkCraftTimer( realCraftItem, player, this, tool, typeRes, amount );
				BulkCraft.StartTimer(timer);
			}
		}

		public void CreateItem( Mobile from, Type type, Type typeRes, BaseTool tool, CraftItem realCraftItem )
		{	
			// Verify if the type is in the list of the craftable item
			CraftItem craftItem = m_CraftItems.SearchFor( type );
			if ( craftItem != null )
			{
				// The item is in the list, try to create it
				// Test code: items like sextant parts can be crafted either directly from ingots, or from different parts
				realCraftItem.Craft( from, this, typeRes, tool, null );
				BulkCraft.StopTimer(from as PlayerMobile);
				//craftItem.Craft( from, this, typeRes, tool );
			}
		}

		public IEnumerable<int> GetRecipes()
		{
			return m_Recipes;
		}

		public RecipeScroll GetRecipeScroll(PlayerMobile player, Type type)
		{
			CraftItem craftItem = m_CraftItems.SearchFor(type);
			if (craftItem == null || craftItem.Recipe == null)
			{
				Console.WriteLine("Failed to find Recipe ({0}) {1}", GetType(), type);

				return null;
			}

			return new RecipeScroll(craftItem.Recipe, player);
		}

		public Bag GetRecipeScrolls(PlayerMobile player, params Type[] types)
		{
			var bag = new Bag();

			// This is dirty, but I'm in a rush
			if (this is DefBlacksmithy)
				bag.Hue = Utility.RandomMetalHue();

			foreach(var type in types)
			{
				var scroll = GetRecipeScroll(player, type);
				if (scroll ==  null) continue;

				bag.AddItem(scroll);
			}

			return bag;
		}

		public Bag GetRandomRecipeScrolls(PlayerMobile player, int count, Func<Recipe, bool> predicate)
		{
			var candidates = m_Recipes.Where(id =>
			{
				var recipe = Recipe.Recipes[id];
				return predicate(recipe);
			}).ToList();

			return GetRandomRecipeScrolls(player, candidates, count);
		}

		public static Bag GetRandomRecipeScrolls(PlayerMobile player, List<int> candidates, int count)
		{
			var bag = new Bag();

			for (int i = 0; i < count; i++)
			{
				var id = PickRandomRecipe(candidates);
				if (id < 0) return bag;

				var recipe = new RecipeScroll(id, player);
				bag.AddItem(recipe);
				candidates.Remove(id);
			}

			return bag;
		}

		public int RandomRecipe()
		{
			return PickRandomRecipe(m_Recipes);
		}

		public int RandomRareRecipe()
        {
			return PickRandomRecipe(m_RareRecipes);
        }

		public static int PickRandomRecipe(List<int> recipes)
		{
			return recipes.Count != 0 ? recipes[Utility.Random( recipes.Count )] : -1;
		}

        public int AddCraft( Type typeItem, TextDefinition group, TextDefinition name, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount )
		{
			return AddCraft( -1, typeItem, group, name, MainSkill, minSkill, maxSkill, typeRes, nameRes, amount, "" );
		}

		public int AddCraft( Type typeItem, TextDefinition group, TextDefinition name, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount, TextDefinition message )
		{
			return AddCraft( -1, typeItem, group, name, MainSkill, minSkill, maxSkill, typeRes, nameRes, amount, message );
		}

		public int AddCraftRecipe( int recipeNumber, Type typeItem, TextDefinition group, TextDefinition name, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount, TextDefinition message )
		{
			return AddCraft( recipeNumber, typeItem, group, name, MainSkill, minSkill, maxSkill, typeRes, nameRes, amount, message );
		}

		public int AddCraft( Type typeItem, TextDefinition group, TextDefinition name, SkillName skillToMake, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount )
		{
			return AddCraft( -1, typeItem, group, name, skillToMake, minSkill, maxSkill, typeRes, nameRes, amount, "" );
		}

		public int AddCraft( int recipeNumber, Type typeItem, TextDefinition group, TextDefinition name, SkillName skillToMake, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount, TextDefinition message )
		{
			CraftItem craftItem = new CraftItem( typeItem, group, name );
			craftItem.AddRes( typeRes, nameRes, amount, message );
			craftItem.AddSkill( skillToMake, minSkill, maxSkill );

			DoGroup( group, craftItem );
			var index = m_CraftItems.Add( craftItem );
			if (0 < recipeNumber)
			{
				int recipeStartIndex;
				if (!Recipe.RecipeIndexStartMap.TryGetValue(GetType(), out recipeStartIndex)) throw new Exception(string.Format("Failed to get recipe index for {0}", GetType()));

				AddRecipe(index, recipeStartIndex + recipeNumber );
			}
			
			return index;
		}

		private void DoGroup( TextDefinition groupName, CraftItem craftItem )
		{
			int index = m_CraftGroups.SearchFor( groupName );

			if ( index == -1)
			{
				CraftGroup craftGroup = new CraftGroup( groupName );
				craftGroup.AddCraftItem( craftItem );
				m_CraftGroups.Add( craftGroup );
			}
			else
			{
				m_CraftGroups.GetAt( index ).AddCraftItem( craftItem );
			}
		}

		public void SetManaReq( int index, int mana )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.Mana = mana;
		}

		public void SetStamReq( int index, int stam )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.Stam = stam;
		}

		public void SetHitsReq( int index, int hits )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.Hits = hits;
		}
		
		public void SetUseAllRes( int index, bool useAll )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.UseAllRes = useAll;
		}

		public void SetNeedHeat( int index, bool needHeat )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.NeedHeat = needHeat;
		}

		public void SetNeedOven( int index, bool needOven )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.NeedOven = needOven;
		}

		public void SetNeedMill( int index, bool needMill )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.NeedMill = needMill;
		}

		public void SetNeededExpansion( int index, Expansion expansion )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.RequiredExpansion = expansion;
		}

		public void AddRes( int index, Type type, TextDefinition name, int amount )
		{
			AddRes( index, type, name, amount, "" );
		}

		public void AddRes( int index, Type type, TextDefinition name, int amount, TextDefinition message )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.AddRes( type, name, amount, message );
		}

		public void AddSkill( int index, SkillName skillToMake, double minSkill, double maxSkill )
		{
			CraftItem craftItem = m_CraftItems.GetAt(index);
			craftItem.AddSkill(skillToMake, minSkill, maxSkill);
		}

		public void AddOrSkill( int index, double minSkill, double maxSkill, params SkillName[] skills )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.AddOrSkillGroup( minSkill, maxSkill, skills );
		}

		private void AddRecipeBase( int index, int id )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.AddRecipe( id, this );
		}

		public void AddRecipe( int index, int id )
		{
			AddRecipeBase( index, id );
			m_Recipes.Add( id );
		}

		public void AddRareRecipe( int index, int id )
		{
			AddRecipeBase( index, id );
			m_RareRecipes.Add( id );
		}

		public void AddQuestRecipe( int index, int id )
		{
			AddRecipeBase( index, id );
		}

		public void ForceNonExceptional( int index )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.ForceNonExceptional = true;
		}

		public void SetSubRes( Type type, string name )
		{
			m_CraftSubRes.ResType = type;
			m_CraftSubRes.NameString = name;
			m_CraftSubRes.Init = true;
		}

		public void SetSubRes( Type type, int name )
		{
			m_CraftSubRes.ResType = type;
			m_CraftSubRes.NameNumber = name;
			m_CraftSubRes.Init = true;
		}

		public void AddSubRes( Type type, int name, double reqSkill, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, message );
			m_CraftSubRes.Add( craftSubRes );
		}

		public void AddSubRes( Type type, int name, double reqSkill, int genericName, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, genericName, message );
			m_CraftSubRes.Add( craftSubRes );
		}

		public void AddSubRes( Type type, string name, double reqSkill, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, message );
			m_CraftSubRes.Add( craftSubRes );
		}

		public abstract void InitCraftList();

		public abstract void PlayCraftEffect( Mobile from );
		public abstract int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, CraftItem item );

		public abstract int CanCraft( Mobile from, BaseTool tool, Type itemType );
	}
}