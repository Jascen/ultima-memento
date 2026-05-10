using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Misc;
using Server.Engines.MLQuests;

namespace Server.Engines.Craft
{
	public enum ConsumeType
	{
		All, Half, None
	}

	public interface ICraftable
	{
		int OnCraft( int quality, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue );
	}

	public class CraftItem
	{
		private CraftResCol m_arCraftRes;
		private CraftSkillCol m_arCraftSkill;
		private Type m_Type;

		private string m_GroupNameString;
		private int m_GroupNameNumber;

		private string m_NameString;
		private int m_NameNumber;
		
		private int m_Mana;
		private int m_Hits;
		private int m_Stam;

		private bool m_UseAllRes;

		private bool m_NeedHeat;
		private bool m_NeedOven;
		private bool m_NeedMill;

		private bool m_ForceNonExceptional;

		public bool ForceNonExceptional
		{
			get { return m_ForceNonExceptional; }
			set { m_ForceNonExceptional = value; }
		}
	
		private Expansion m_RequiredExpansion;

		public Expansion RequiredExpansion
		{
			get { return m_RequiredExpansion; }
			set { m_RequiredExpansion = value; }
		}

		public Recipe Recipe { get; private set; }

		public void AddRecipe( int id, CraftSystem system )
		{
			if( Recipe != null )
			{
				Console.WriteLine( "Warning: Attempted add of recipe #{0} to the crafting of {1} in CraftSystem {2}.", id, this.m_Type.Name, system );
				return;
			}

			Recipe = new Recipe( id, system, this );
		}

		private static Dictionary<Type, int> _itemIds = new Dictionary<Type, int>();
		
		public static int ItemIDOf( Type type ) {
			int itemId;

			if ( !_itemIds.TryGetValue( type, out itemId ) )
			{
				if ( itemId == 0 ) {
					object[] attrs = type.GetCustomAttributes( typeof( CraftItemIDAttribute ), false );

					if ( attrs.Length > 0 ) {
						CraftItemIDAttribute craftItemID = ( CraftItemIDAttribute ) attrs[0];
						itemId = craftItemID.ItemID;
					}
				}

				if ( itemId == 0 ) {
					Item item = null;

					try { item = Activator.CreateInstance( type ) as Item; } catch { }

					if ( item != null ) {
						itemId = item.ItemID;
						item.Delete();
					}
				}

				_itemIds[type] = itemId;
			}

			return itemId;
		}

		public CraftItem( Type type, TextDefinition groupName, TextDefinition name )
		{
			m_arCraftRes = new CraftResCol();
			m_arCraftSkill = new CraftSkillCol();

			m_Type = type;

			m_GroupNameString = groupName;
			m_NameString = name;

			m_GroupNameNumber = groupName;
			m_NameNumber = name;
		}

		public void AddRes( Type type, TextDefinition name, int amount )
		{
			AddRes( type, name, amount, "" );
		}

		public void AddRes( Type type, TextDefinition name, int amount, TextDefinition message )
		{
			CraftRes craftRes = new CraftRes( type, name, amount, message );
			m_arCraftRes.Add( craftRes );
		}

		public void AddSkill( SkillName skillToMake, double minSkill, double maxSkill )
		{
			CraftSkill craftSkill = new CraftSkill( skillToMake, minSkill, maxSkill );
			m_arCraftSkill.Add( craftSkill );
		}

		public int Mana
		{
			get { return m_Mana; }
			set { m_Mana = value; }
		}

		public int Hits
		{
			get { return m_Hits; }
			set { m_Hits = value; }
		}

		public int Stam
		{
			get { return m_Stam; }
			set { m_Stam = value; }
		}

		public bool UseAllRes
		{
			get { return m_UseAllRes; }
			set { m_UseAllRes = value; }
		}

		public bool NeedHeat
		{
			get { return m_NeedHeat; }
			set { m_NeedHeat = value; }
		}

		public bool NeedOven
		{
			get { return m_NeedOven; }
			set { m_NeedOven = value; }
		}

		public bool NeedMill
		{
			get { return m_NeedMill; }
			set { m_NeedMill = value; }
		}
		
		public Type ItemType
		{
			get { return m_Type; }
		}

		public string GroupNameString
		{
			get { return m_GroupNameString; }
		}

		public int GroupNameNumber
		{
			get { return m_GroupNameNumber; }
		}

		public string NameString
		{
			get { return m_NameString; }
		}

		public int NameNumber
		{
			get { return m_NameNumber; }
		}

		public CraftResCol Resources
		{
			get { return m_arCraftRes; }
		}

		public CraftSkillCol Skills
		{
			get { return m_arCraftSkill; }
		}

		public bool ConsumeAttributes( Mobile from, ref object message, bool consume )
		{
			bool consumMana = false;
			bool consumHits = false;
			bool consumStam = false;

			if ( Hits > 0 && from.Hits < Hits )
			{
				message = "You lack the required hit points to make that.";
				return false;
			}
			else
			{
				consumHits = consume;
			}

			if ( Mana > 0 && from.Mana < Mana )
			{
				message = "You lack the required mana to make that.";
				return false;
			}
			else
			{
				consumMana = consume;
			}

			if ( Stam > 0 && from.Stam < Stam )
			{
				message = "You lack the required stamina to make that.";
				return false;
			}
			else
			{
				consumStam = consume;
			}

			if ( consumMana )
				from.Mana -= Mana;

			if ( consumHits )
				from.Hits -= Hits;

			if ( consumStam )
				from.Stam -= Stam;

			return true;
		}

		#region Tables
		private static int[] m_HeatSources = new int[]
			{
				0x461, 0x48E, // Sandstone oven/fireplace
				0x92B, 0x96C, // Stone oven/fireplace
				0xDE3, 0xDE9, // Campfire
				0xFAC, // Firepit
				0x0FB1, 0x10DE, // Small Forge
				0x184A, 0x184C, // Heating stand (left)
				0x184E, 0x1850, // Heating stand (right)
				0x19AA, 0x19BB,	// Brazier
				0x197A, 0x19A9, // Large Forge 
				0x2DD8, // Elven Forge
				0x2DDB, 0x2DDC,	// Elven stove
				0x398C, 0x399F, // Fire field
				0x5321, 0x53A0, // Bonfire
				0x576A, 0x5771 // Firepit
			};

		private static int[] m_Ovens = new int[]
			{
				0x461, 0x46F, // Sandstone oven
				0x92B, 0x93F,  // Stone oven
				0x2DDB, 0x2DDC,	//Elven stove
				0x5363, 0x5367 // stove
			};

		private static int[] m_Mills = new int[]
			{
				0x1920, 0x1921, 0x1922, 0x1923, 0x1924, 0x1295, 0x1926, 0x1928,
				0x192C, 0x192D, 0x192E, 0x129F, 0x1930, 0x1931, 0x1932, 0x1934
			};

		private static Type[][] m_TypesTable = new Type[][]
			{
				new Type[]{ typeof( BaseLog ), typeof( BaseWoodBoard ) },
				new Type[]{ typeof( BaseScales ), typeof( BaseIngot ) },
				new Type[]{ typeof( BaseGranite ), typeof( BaseOre ) },
				new Type[]{ typeof( BaseSkins ), typeof( BaseBlocks ) },
				new Type[]{ typeof( BaseFabric ), typeof( BaseLeather ) },
				new Type[]{ typeof( BaseHides ), typeof( BaseSkeletal ) },
				new Type[]{ typeof( BlankMap ), typeof( BlankScroll ) },
				new Type[]{ typeof( CheeseWheel ), typeof( CheeseWedge ) },
				new Type[]{ typeof( Pumpkin ), typeof( SmallPumpkin ) },
				new Type[]{ typeof( WoodenBowlOfPeas ), typeof( PewterBowlOfPeas ) }
			};

		private static Type[] m_ColoredItemTable = new Type[]
			{
				typeof( BaseWeapon ), typeof( BaseArmor ), typeof( BaseClothing ),
				typeof( BaseTrinket ), typeof( DragonBardingDeed ),
				typeof( Spellbook ), typeof( Runebook ),
				typeof( ForkLeft ), typeof( ForkRight ),
				typeof( SpoonLeft ), typeof( SpoonRight ),
				typeof( KnifeLeft ), typeof( KnifeRight ),
				typeof( Plate ), typeof( BaseHarvestTool ),
				typeof( Goblet ), typeof( PewterMug ), typeof( SkullMug ),
				typeof( KeyRing ),
				typeof( Candelabra ), typeof( Scales ),
				typeof( Key ), typeof( Globe ), typeof( BaseBook ),
				typeof( Spyglass ), typeof( Lantern ),
				typeof( HeatingStand ), typeof( BaseTool ),
				typeof( TenFootPole ), typeof( HorseArmor ),
				typeof( BaseContainer ), typeof( DragonPedStatue )
			};

		private static Type[] m_ColoredResourceTable = new Type[]
			{
				typeof( BaseIngot ), typeof( BaseOre ),
				typeof( BaseLeather ), typeof( BaseHides ),
				typeof( BaseFabric ),
				typeof( BaseGranite ), typeof( BaseScales ),
				typeof( BaseLog ), typeof( BaseWoodBoard ),
				typeof( BaseBlocks ), typeof( BaseSkins ),
				typeof( BaseSpecial ), typeof( BaseSkeletal )
			};

		private static Type[] m_MarkableTable = new Type[]
				{
					typeof( BaseArmor ),
					typeof( BaseWeapon ),
					typeof( BaseClothing ),
					typeof( BaseInstrument ),
					typeof( BaseLight ),
					typeof( DragonBardingDeed ),
					typeof( BaseTool ),
					typeof( BaseHarvestTool ),
					typeof( FukiyaDarts ), typeof( Shuriken ),
					typeof( Spellbook ), typeof( Runebook ),
					typeof( BaseQuiver ),
					typeof( BaseTrinket )
				};
		#endregion

		public bool IsMarkable( Type type )
		{
			if( m_ForceNonExceptional )	//Don't even display the stuff for marking if it can't ever be exceptional.
				return false;

			for ( int i = 0; i < m_MarkableTable.Length; ++i )
			{
				if ( type == m_MarkableTable[i] || type.IsSubclassOf( m_MarkableTable[i] ) )
					return true;
			}

			return false;
		}

		public bool RetainsColorFrom( CraftSystem system, Type type )
		{
			if (DefTailoring.IsNonColorable(m_Type))
			{
				return false;
			}

			if ( system is DefWands )
				return false;

			if ( system.RetainsColorFrom( this, type ) )
				return true;

			bool inItemTable = false, inResourceTable = false;

			for ( int i = 0; !inItemTable && i < m_ColoredItemTable.Length; ++i )
				inItemTable = ( m_Type == m_ColoredItemTable[i] || m_Type.IsSubclassOf( m_ColoredItemTable[i] ) );

			for ( int i = 0; inItemTable && !inResourceTable && i < m_ColoredResourceTable.Length; ++i )
				inResourceTable = ( type == m_ColoredResourceTable[i] || type.IsSubclassOf( m_ColoredResourceTable[i] ) );

			return ( inItemTable && inResourceTable );
		}

		public bool Find( Mobile from, int[] itemIDs )
		{
			Map map = from.Map;

			if ( map == null )
				return false;

			IPooledEnumerable eable = map.GetItemsInRange( from.Location, 2 );

			foreach ( Item item in eable )
			{
				if ( (item.Z + 16) > from.Z && (from.Z + 16) > item.Z && Find( item.ItemID, itemIDs ) )
				{
					eable.Free();
					return true;
				}
			}

			eable.Free();

			for ( int x = -2; x <= 2; ++x )
			{
				for ( int y = -2; y <= 2; ++y )
				{
					int vx = from.X + x;
					int vy = from.Y + y;

					StaticTile[] tiles = map.Tiles.GetStaticTiles( vx, vy, true );

					for ( int i = 0; i < tiles.Length; ++i )
					{
						int z = tiles[i].Z;
						int id = tiles[i].ID;

						if ( (z + 16) > from.Z && (from.Z + 16) > z && Find( id, itemIDs ) )
							return true;
					}
				}
			}

			return false;
		}

		public bool Find( int itemID, int[] itemIDs )
		{
			bool contains = false;

			for ( int i = 0; !contains && i < itemIDs.Length; i += 2 )
				contains = ( itemID >= itemIDs[i] && itemID <= itemIDs[i + 1] );

			return contains;
		}

		public bool IsQuantityType( Type[][] types )
		{
			for ( int i = 0; i < types.Length; ++i )
			{
				Type[] check = types[i];

				for ( int j = 0; j < check.Length; ++j )
				{
					if ( typeof( IHasQuantity ).IsAssignableFrom( check[j] ) )
						return true;
				}
			}

			return false;
		}

		public int ConsumeQuantity( Container cont, Type[][] types, int[] amounts )
		{
			if ( types.Length != amounts.Length )
				throw new ArgumentException();

			Item[][] items = new Item[types.Length][];
			int[] totals = new int[types.Length];

			for ( int i = 0; i < types.Length; ++i )
			{
				items[i] = cont.FindItemsByType( types[i], true );

				for ( int j = 0; j < items[i].Length; ++j )
				{
					IHasQuantity hq = items[i][j] as IHasQuantity;

					if ( hq == null )
					{
						totals[i] += items[i][j].Amount;
					}
					else
					{
						if ( hq is BaseBeverage && ((BaseBeverage)hq).Content != BeverageType.Water )
							continue;

						totals[i] += hq.Quantity;
					}
				}

				if ( totals[i] < amounts[i] )
					return i;
			}

			for ( int i = 0; i < types.Length; ++i )
			{
				int need = amounts[i];

				for ( int j = 0; j < items[i].Length; ++j )
				{
					Item item = items[i][j];
					IHasQuantity hq = item as IHasQuantity;

					if ( hq == null )
					{
						int theirAmount = item.Amount;

						if ( theirAmount < need )
						{
							item.Delete();
							need -= theirAmount;
						}
						else
						{
							item.Consume( need );
							break;
						}
					}
					else
					{
						if ( hq is BaseBeverage && ((BaseBeverage)hq).Content != BeverageType.Water )
							continue;

						int theirAmount = hq.Quantity;

						if ( theirAmount < need )
						{
							hq.Quantity -= theirAmount;
							need -= theirAmount;
						}
						else
						{
							hq.Quantity -= need;
							break;
						}
					}
				}
			}

			return -1;
		}

		public int GetQuantity( Container cont, Type[] types )
		{
			Item[] items = cont.FindItemsByType( types, true );

			int amount = 0;

			for ( int i = 0; i < items.Length; ++i )
			{
				IHasQuantity hq = items[i] as IHasQuantity;

				if ( hq == null )
				{
					amount += items[i].Amount;
				}
				else
				{
					if ( hq is BaseBeverage && ((BaseBeverage)hq).Content != BeverageType.Water )
						continue;

					amount += hq.Quantity;
				}
			}

			return amount;
		}

		public bool ConsumeRes( Mobile from, Type typeRes, CraftSystem craftSystem, ref int resHue, ref int maxAmount, ConsumeType consumeType, ref object message )
		{
			return ConsumeRes( from, typeRes, craftSystem, ref resHue, ref maxAmount, consumeType, ref message, false );
		}

		public bool ConsumeRes( Mobile from, Type typeRes, CraftSystem craftSystem, ref int resHue, ref int maxAmount, ConsumeType consumeType, ref object message, bool isFailure )
		{
			Container ourPack = from.Backpack;

			if ( ourPack == null )
				return false;

			if ( m_NeedHeat && !Find( from, m_HeatSources ) )
			{
				message = 1044487; // You must be near a fire source to cook.
				return false;
			}

			if ( m_NeedOven && !Find( from, m_Ovens ) )
			{
				message = 1044493; // You must be near an oven to bake that.
				return false;
			}

			if ( m_NeedMill && !Find( from, m_Mills ) )
			{
				message = 1044491; // You must be near a flour mill to do that.
				return false;
			}

			Type[][] types = new Type[m_arCraftRes.Count][];
			int[] amounts = new int[m_arCraftRes.Count];

			maxAmount = int.MaxValue;

			CraftSubResCol resCol = craftSystem.CraftSubRes;

			for ( int i = 0; i < types.Length; ++i )
			{
				CraftRes craftRes = m_arCraftRes.GetAt( i );
				Type baseType = craftRes.ItemType;

				// Resource Mutation
				if ( (baseType == resCol.ResType) && ( typeRes != null ) )
				{
					baseType = typeRes;

					CraftSubRes subResource = resCol.SearchFor( baseType );

					if ( subResource != null && from.Skills[craftSystem.MainSkill].Value < subResource.RequiredSkill )
					{
						message = subResource.Message;
						return false;
					}
				}
				// ******************

				for ( int j = 0; types[i] == null && j < m_TypesTable.Length; ++j )
				{
					if ( m_TypesTable[j][0] == baseType )
						types[i] = m_TypesTable[j];
				}

				if ( types[i] == null )
					types[i] = new Type[]{ baseType };

				amounts[i] = craftRes.Amount;

				// For stackable items that can be crafted more than one at a time
				if ( UseAllRes )
				{
					int tempAmount = ourPack.GetAmount( types[i] );
					tempAmount /= amounts[i];
					if ( tempAmount < maxAmount )
					{
						maxAmount = tempAmount;

						if ( maxAmount == 0 )
						{
							CraftRes res = m_arCraftRes.GetAt( i );

							if ( res.MessageNumber > 0 )
								message = res.MessageNumber;
							else if ( !String.IsNullOrEmpty( res.MessageString ) )
								message = res.MessageString;
							else
								message = 502925; // You don't have the resources required to make that item.

							return false;
						}
					}
				}
				// ****************************

				if ( isFailure && !craftSystem.ConsumeOnFailure( from, types[i][0], this ) )
					amounts[i] = 0;
			}

			// We adjust the amount of each resource to consume the max posible
			if ( UseAllRes )
			{
				for ( int i = 0; i < amounts.Length; ++i )
					amounts[i] *= maxAmount;
			}
			else
				maxAmount = -1;

			Item consumeExtra = null;

			int index = 0;

			// Consume ALL
			if ( consumeType == ConsumeType.All )
			{
				m_ResHue = 0; m_ResAmount = 0; m_System = craftSystem;

				if ( IsQuantityType( types ) )
					index = ConsumeQuantity( ourPack, types, amounts );
				else
					index = ourPack.ConsumeTotalGrouped( types, amounts, true, new OnItemConsumed( OnResourceConsumed ), new CheckItemGroup( CheckHueGrouping ) );

				resHue = m_ResHue;
			}

			// Consume Half ( for use all resource craft type )
			else if ( consumeType == ConsumeType.Half )
			{
				for ( int i = 0; i < amounts.Length; i++ )
				{
					amounts[i] /= 2;

					if ( amounts[i] < 1 )
						amounts[i] = 1;
				}

				m_ResHue = 0; m_ResAmount = 0; m_System = craftSystem;

				if ( IsQuantityType( types ) )
					index = ConsumeQuantity( ourPack, types, amounts );
				else
					index = ourPack.ConsumeTotalGrouped( types, amounts, true, new OnItemConsumed( OnResourceConsumed ), new CheckItemGroup( CheckHueGrouping ) );

				resHue = m_ResHue;
			}

			else // ConstumeType.None ( it's basicaly used to know if the crafter has enough resource before starting the process )
			{
				index = -1;

				if ( IsQuantityType( types ) )
				{
					for ( int i = 0; i < types.Length; i++ )
					{
						if ( GetQuantity( ourPack, types[i] ) < amounts[i] )
						{
							index = i;
							break;
						}
					}
				}
				else
				{
					for ( int i = 0; i < types.Length; i++ )
					{
						if ( ourPack.GetBestGroupAmount( types[i], true, new CheckItemGroup( CheckHueGrouping ) ) < amounts[i] )
						{
							index = i;
							break;
						}
					}
				}
			}

			if ( index == -1 )
			{
				if ( consumeType != ConsumeType.None )
					if ( consumeExtra != null )
						consumeExtra.Delete();

				return true;
			}
			else
			{
				CraftRes res = m_arCraftRes.GetAt( index );

				if ( res.MessageNumber > 0 )
					message = res.MessageNumber;
				else if ( res.MessageString != null && res.MessageString != String.Empty )
					message = res.MessageString;
				else
					message = 502925; // You don't have the resources required to make that item.

				return false;
			}
		}

		private int m_ResHue;
		private int m_ResAmount;
		private CraftSystem m_System;

		private void AttemptCompleted( Mobile from, CraftSystem craftSystem, BaseTool tool, BulkCraftContext bulkCraftContext, object message, bool canSendGump = true )
		{
			if ( bulkCraftContext != null )
			{
				bulkCraftContext.LastMessage = message;
				return;
			}

			if ( canSendGump ) from.SendGump( new CraftGump( from, craftSystem, tool, message ) );
			else if ( message is int && (int)message > 0 ) from.SendLocalizedMessage( (int)message );
			else if ( message is string ) from.SendMessage( (string)message );
		}

		private void OnResourceConsumed( Item item, int amount )
		{
			if ( !RetainsColorFrom( m_System, item.GetType() ) )
				return;

			if ( amount >= m_ResAmount )
			{
				m_ResHue = item.Hue;
				m_ResAmount = amount;
			}
		}

		private int CheckHueGrouping( Item a, Item b )
		{
			return b.Hue.CompareTo( a.Hue );
		}

		public double GetExceptionalChance( CraftSystem system, double chance, Mobile from )
		{
			if( m_ForceNonExceptional )
				return 0.0;

			double bonus = 0.0;

			if ( from.Skills[ system.MainSkill ].Value >= 125.0 ){ bonus = 0.030; }
			else if ( from.Skills[ system.MainSkill ].Value >= 124.0 ){ bonus = 0.029; }
			else if ( from.Skills[ system.MainSkill ].Value >= 123.0 ){ bonus = 0.028; }
			else if ( from.Skills[ system.MainSkill ].Value >= 122.0 ){ bonus = 0.027; }
			else if ( from.Skills[ system.MainSkill ].Value >= 121.0 ){ bonus = 0.026; }
			else if ( from.Skills[ system.MainSkill ].Value >= 120.0 ){ bonus = 0.025; }
			else if ( from.Skills[ system.MainSkill ].Value >= 119.0 ){ bonus = 0.024; }
			else if ( from.Skills[ system.MainSkill ].Value >= 118.0 ){ bonus = 0.023; }
			else if ( from.Skills[ system.MainSkill ].Value >= 117.0 ){ bonus = 0.022; }
			else if ( from.Skills[ system.MainSkill ].Value >= 116.0 ){ bonus = 0.021; }
			else if ( from.Skills[ system.MainSkill ].Value >= 115.0 ){ bonus = 0.020; }
			else if ( from.Skills[ system.MainSkill ].Value >= 114.0 ){ bonus = 0.019; }
			else if ( from.Skills[ system.MainSkill ].Value >= 113.0 ){ bonus = 0.018; }
			else if ( from.Skills[ system.MainSkill ].Value >= 112.0 ){ bonus = 0.017; }
			else if ( from.Skills[ system.MainSkill ].Value >= 111.0 ){ bonus = 0.016; }
			else if ( from.Skills[ system.MainSkill ].Value >= 110.0 ){ bonus = 0.015; }
			else if ( from.Skills[ system.MainSkill ].Value >= 109.0 ){ bonus = 0.014; }
			else if ( from.Skills[ system.MainSkill ].Value >= 108.0 ){ bonus = 0.013; }
			else if ( from.Skills[ system.MainSkill ].Value >= 107.0 ){ bonus = 0.012; }
			else if ( from.Skills[ system.MainSkill ].Value >= 106.0 ){ bonus = 0.011; }
			else if ( from.Skills[ system.MainSkill ].Value >= 105.0 ){ bonus = 0.010; }
			else if ( from.Skills[ system.MainSkill ].Value >= 104.0 ){ bonus = 0.009; }
			else if ( from.Skills[ system.MainSkill ].Value >= 103.0 ){ bonus = 0.008; }
			else if ( from.Skills[ system.MainSkill ].Value >= 102.0 ){ bonus = 0.007; }
			else if ( from.Skills[ system.MainSkill ].Value >= 101.0 ){ bonus = 0.006; }
			else if ( from.Skills[ system.MainSkill ].Value >= 100.1 ){ bonus = 0.005; }

			if ( bonus >= chance && from.Skills[ system.MainSkill ].Value >= 100.1 && chance > 0.0 )
				bonus = 0.001;

			switch ( system.ECA )
			{
				default:
				case CraftECA.ChanceMinusSixty: chance -= 0.6; break;
				case CraftECA.FiftyPercentChanceMinusTenPercent: chance = chance * 0.5 - 0.1; break;
				case CraftECA.ChanceMinusSixtyToFourtyFive:
				{
					double offset = 0.60 - ((from.Skills[system.MainSkill].Value - 95.0) * 0.03);

					if ( offset < 0.45 )
						offset = 0.45;
					else if ( offset > 0.60 )
						offset = 0.60;

					chance -= offset;
					break;
				}
			}

			if ( chance < 0.0 )
				chance = 0.0;

			chance = chance + bonus;

			return chance;
		}

		public bool CheckSkills( Mobile from, Type typeRes, CraftSystem craftSystem, ref int quality, ref bool allRequiredSkills )
		{
			return CheckSkills( from, typeRes, craftSystem, ref quality, ref allRequiredSkills, true );
		}

		public bool CheckSkills( Mobile from, Type typeRes, CraftSystem craftSystem, ref int quality, ref bool allRequiredSkills, bool gainSkills )
		{
			double chance = GetSuccessChance( from, typeRes, craftSystem, gainSkills, ref allRequiredSkills );

			if ( GetExceptionalChance( craftSystem, chance, from ) > Utility.RandomDouble() )
				quality = 2;

			return ( chance > Utility.RandomDouble() );
		}

		public double GetSuccessChance( Mobile from, Type typeRes, CraftSystem craftSystem, bool gainSkills, ref bool allRequiredSkills )
		{
			double minMainSkill = 0.0;
			double maxMainSkill = 0.0;
			double valMainSkill = 0.0;

			allRequiredSkills = true;

			for ( int i = 0; i < m_arCraftSkill.Count; i++)
			{
				CraftSkill craftSkill = m_arCraftSkill.GetAt(i);

				double minSkill = craftSkill.MinSkill;
				double maxSkill = craftSkill.MaxSkill;
				double valSkill = from.Skills[craftSkill.SkillToMake].Value;

				if ( valSkill < minSkill )
					allRequiredSkills = false;

				if ( craftSkill.SkillToMake == craftSystem.MainSkill )
				{
					minMainSkill = minSkill;
					maxMainSkill = maxSkill;
					valMainSkill = valSkill;
				}

				if ( gainSkills ) // This is a passive check. Success chance is entirely dependent on the main skill
				{
					Type resourceType = typeRes;
					if ( resourceType == null )
						resourceType = Resources.GetAt( 0 ).ItemType;

					CraftResource thisResource = CraftResources.GetFromType( resourceType );
					int checks = CraftResources.GetSkillCheckMultiplier( thisResource );
					if (0 < checks)
						from.CheckSkill( craftSkill.SkillToMake, minSkill, maxSkill );

					if (1 < checks)
					{
						double before = from.Skills[craftSkill.SkillToMake].Base;

						for (i = 1; i < checks; i++)
							from.CheckSkill( craftSkill.SkillToMake, minSkill, maxSkill );
						
						// Notify if bonus attempts resulted in a skill gain
						if ( before < from.Skills[craftSkill.SkillToMake].Base )
						{
							var resourceName = CraftResources.GetName( thisResource );
							from.PlaySound( from.Female ? 778 : 1049 );
							from.Say( "*ah!*" );
							from.SendMessage( "Working with {0} has challenged your abilities.", resourceName );
						}
					}
				}
			}

			double chance;

			if ( allRequiredSkills )
				chance = craftSystem.GetChanceAtMin( this ) + ((valMainSkill - minMainSkill) / (maxMainSkill - minMainSkill) * (1.0 - craftSystem.GetChanceAtMin( this )));
			else
				chance = 0.0;

			if ( allRequiredSkills && valMainSkill == maxMainSkill )
				chance = 1.0;

			return chance;
		}

		public bool Craft( Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, BulkCraftContext bulkCraftContext )
		{
			if ( from.BeginAction( typeof( CraftSystem ) ) )
			{
				bool allRequiredSkills = true;
				double chance = GetSuccessChance( from, typeRes, craftSystem, false, ref allRequiredSkills );

				if ( allRequiredSkills && chance >= 0.0 )
				{
					int badCraft = craftSystem.CanCraft( from, tool, m_Type );

					if( badCraft <= 0 )
					{
						int resHue = 0;
						int maxAmount = 0;
						object message = null;

						if( ConsumeRes( from, typeRes, craftSystem, ref resHue, ref maxAmount, ConsumeType.None, ref message ) )
						{
							message = null;

							if( ConsumeAttributes( from, ref message, false ) )
							{
								CraftContext context = craftSystem.GetContext( from );

								if( context != null )
									context.OnMade( this );

								int iMin = craftSystem.MinCraftEffect;
								int iMax = (craftSystem.MaxCraftEffect - iMin) + 1;
								int iRandom = Utility.Random( iMax );
								iRandom += iMin + 1;

								if ( bulkCraftContext != null )
									RunCommand( from, craftSystem, this, typeRes, tool, bulkCraftContext );
								else 
									new InternalTimer( from, craftSystem, this, typeRes, tool, iRandom ).Start();
								
								return true;
							}
							else
							{
								from.EndAction( typeof( CraftSystem ) );
								AttemptCompleted( from, craftSystem, tool, bulkCraftContext, message );
							}
						}
						else
						{
							from.EndAction( typeof( CraftSystem ) );
							AttemptCompleted( from, craftSystem, tool, bulkCraftContext, message );
						}
					}
					else
					{
						from.EndAction( typeof( CraftSystem ) );
						AttemptCompleted( from, craftSystem, tool, bulkCraftContext, badCraft );
					}
				}
				else
				{
					from.EndAction( typeof( CraftSystem ) );
					AttemptCompleted( from, craftSystem, tool, bulkCraftContext, 1044153 ); // You don't have the required skills to attempt this item.
				}
			}
			else
			{
				from.SendLocalizedMessage( 500119 ); // You must wait to perform another action
			}

			return false;
		}

		private object RequiredExpansionMessage( Expansion expansion )	//Eventually convert to TextDefinition, but that requires that we convert all the gumps to ues it too.  Not that it wouldn't be a bad idea.
		{
			switch( expansion )
			{
				case Expansion.SE:
					return 1063307; // The "Samurai Empire" expansion is required to attempt this item.
				case Expansion.ML:
					return 1072650; // The "Mondain's Legacy" expansion is required to attempt this item.
				default:
					return String.Format( "The \"{0}\" expansion is required to attempt this item.", ExpansionInfo.GetInfo( expansion ).Name );
			}
		}

		private bool IsExceptional(Item item)
		{
			if (item == null) return false;

			return (item is BaseArmor && ((BaseArmor)item).Quality == ArmorQuality.Exceptional)
				|| (item is BaseWeapon && ((BaseWeapon)item).Quality == WeaponQuality.Exceptional)
				|| (item is BaseClothing && ((BaseClothing)item).Quality == ClothingQuality.Exceptional)
				|| (item is BaseTrinket && ((BaseTrinket)item).Quality == TrinketQuality.Exceptional)
				|| (item is BaseInstrument && ((BaseInstrument)item).Quality == InstrumentQuality.Exceptional);
		}

		public void CompleteCraft( int quality, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CustomCraft customCraft, BulkCraftContext bulkCraftContext )
		{
			if ( bulkCraftContext != null)
				bulkCraftContext.Current++;
				
			CraftContext context = craftSystem.GetContext( from );
			var canSendGump = tool != null && !tool.Deleted && tool.UsesRemaining > 0;

			int badCraft = craftSystem.CanCraft( from, tool, m_Type );
			if ( badCraft > 0 )
			{
				AttemptCompleted( from, craftSystem, tool, bulkCraftContext, badCraft, canSendGump );

				return;
			}

			int checkResHue = 0, checkMaxAmount = 0;
			object checkMessage = null;

			// Not enough resource to craft it
			if ( !ConsumeRes( from, typeRes, craftSystem, ref checkResHue, ref checkMaxAmount, ConsumeType.None, ref checkMessage ) )
			{
				AttemptCompleted( from, craftSystem, tool, bulkCraftContext, badCraft, canSendGump );

				return;
			}
			else if ( !ConsumeAttributes( from, ref checkMessage, false ) )
			{
				AttemptCompleted( from, craftSystem, tool, bulkCraftContext, badCraft, canSendGump );

				return;
			}

			bool toolBroken = false;

			int ignored = 1;
			int endquality = 1;

			bool allRequiredSkills = true;

			if ( CheckSkills( from, typeRes, craftSystem, ref ignored, ref allRequiredSkills ) )
			{
				// Resource
				int resHue = 0;
				int maxAmount = 0;

				object message = null;

				// Not enough resource to craft it
				if ( !ConsumeRes( from, typeRes, craftSystem, ref resHue, ref maxAmount, ConsumeType.All, ref message ) )
				{
					AttemptCompleted( from, craftSystem, tool, bulkCraftContext, message, canSendGump );

					return;
				}
				else if ( !ConsumeAttributes( from, ref message, true ) )
				{
					AttemptCompleted( from, craftSystem, tool, bulkCraftContext, badCraft, canSendGump );
					return;
				}

				tool.UsesRemaining--;

				var gloves = from.FindItemOnLayer(Layer.OneHanded) as AncientCraftingGloves;
				if (gloves != null)
					gloves.ConsumeCharge(from, craftSystem);

				if ( tool.UsesRemaining < 1 )
				{
					toolBroken = true;
					tool.Delete();
					BulkCraft.Cancel((PlayerMobile)from);
				}

				int num = 0;

				Item item;
				
				if ( customCraft != null )
					item = customCraft.CompleteCraft( out num );
				else if ( typeof( MapItem ).IsAssignableFrom( ItemType ) && ( !Worlds.IsPlayerInTheLand( from.Map, from.Location, from.X, from.Y ) ) )
				{
					item = new IndecipherableMap();
					from.SendMessage( "You cannot seem to create a map of this area." );
				}
				else
					item = Activator.CreateInstance( ItemType ) as Item;

				if ( item != null )
				{
					item.Built = true;
					item.BuiltBy = from;

					if( item is ICraftable )
						endquality = ((ICraftable)item).OnCraft( quality, from, craftSystem, typeRes, tool, this, resHue );
					else if ( item.Hue == 0 )
						item.Hue = resHue;

					if ( tool is IRunicTool )
					{
						var runic = (IRunicTool)tool;
						BaseRunicTool.ApplyAttributes(from, item, runic.RunicMinAttributes, runic.RunicMaxAttributes, runic.RunicMinIntensity, runic.RunicMaxIntensity);
					}
					else if (tool is IRunicWhenExceptional && IsExceptional(item)) // Exceptional applies a runic
					{
						BaseRunicTool.ApplyAttributesTo(item, 1, 5, 20);
					}

					if ( maxAmount > 0 )
					{
						if ( !item.Stackable && item is IUsesRemaining )
							((IUsesRemaining)item).UsesRemaining *= maxAmount;
						else
							item.Amount = maxAmount;
					}

                    EventSink.InvokeCraftSuccess(new CraftSuccessArgs(from, item, tool is Item ? (Item)tool : null));

					if ( item is Kindling || item is BarkFragment || item is Shaft )
						item.Amount *= 2;
					else if ( item is BlankScroll )
						item.Amount *= 10;

					if ( ( item.Resource == CraftResource.None || item.Resource == CraftResource.None ) && ( item is WoodenPlateLegs || item is WoodenPlateGloves || item is WoodenPlateGorget || item is WoodenPlateArms || item is WoodenPlateChest || item is WoodenPlateHelm ) )
					{
						item.Resource = CraftResource.RegularWood;
						item.Hue = 0x840;
					}
					else if ( item is MagicalWand )
					{
						string nameString = context.NameString;
						item.Delete();
						item = new MagicalWand( SpellItems.GetWand( nameString ) );
					}
					else if ( item is BaseContainer || item is BaseBook || item is BaseLight || item is Spyglass || item is Runebook || item is Spellbook || item is TenFootPole || item is DragonPedStatue || item is HorseArmor || item is PotionKeg || item is TrapKit )
					{
						Type resourceType = typeRes;
						if ( resourceType == null )
							resourceType = Resources.GetAt( 0 ).ItemType;

						CraftResource thisResource = CraftResources.GetFromType( resourceType );
						item.Resource = thisResource;
					}
					else if ( item.Catalog == Catalogs.Stone )
					{
						string material = "Granite";
						string maker = from.Name;

						Type resourceType = typeRes;
						if ( resourceType == null )
							resourceType = Resources.GetAt( 0 ).ItemType;

						CraftResource thisResource = CraftResources.GetFromType( resourceType );
						item.Hue = CraftResources.GetClr( thisResource );
						item.Resource = thisResource;
						item.Built = true;
						item.BuiltBy = from;

						if ( item is BaseStatue )
						{
							((BaseStatue)item).Crafter = maker;
							((BaseStatue)item).MadeOf = material;
						}

						if ( item is BaseStatueDeed )
							Server.Items.Statues.SetStatue( (BaseStatueDeed)item, (int)item.Weight, item.Hue, material, maker, item.Name, true, from, thisResource );
					}
					else if ( item is ShortMusicStand || 
						item is Scales || 
						item is Key || 
						item is Globe || 
						item is WindChimes || 
						item is FancyWindChimes || 
						item is TallMusicStand || 
						item is Easle || 
						item is ShojiScreen || 
						item is BambooScreen || 
						item is FootStool || 
						item is Stool || 
						item is BambooChair || 
						item is WoodenChair || 
						item is FancyWoodenChairCushion || 
						item is WoodenChairCushion || 
						item is WoodenBench || 
						item is WoodenThrone || 
						item is Throne || 
						item is Nightstand || 
						item is WritingTable || 
						item is YewWoodTable || 
						item is CounterWood || 
						item is CounterWooden || 
						item is CounterRustic || 
						item is LargeTable || 
						item is ElegantLowTable || 
						item is PlainLowTable || 
						item is CandleLarge || 
						item is Candelabra || 
						item is CandelabraStand )
					{
						Type resourceType = typeRes;
						if ( resourceType == null )
							resourceType = Resources.GetAt( 0 ).ItemType;

						CraftResource thisResource = CraftResources.GetFromType( resourceType );

						item.Hue = CraftResources.GetClr( thisResource );
					}

					if ( item is Spear ){ item.ItemID = 0xF62; }
					else if ( item is Club ){ item.ItemID = 0x13B4; }
					else if ( item is Cleaver ){ item.ItemID = 0xEC3; }

					BaseContainer.PutStuffInContainer( from, 2, item );

					if (context.AutomaticallyMarkQuestItems && tool != null && (tool is BaseRunicTool) == false && from is PlayerMobile)
						MLQuestSystem.MarkQuestItem((PlayerMobile) from, item);

					if( from.AccessLevel > AccessLevel.Player )
						CommandLogging.WriteLine( from, "Crafting {0} with craft system {1}", CommandLogging.Format( item ), craftSystem.GetType().Name );
				}

				if ( num == 0 )
					num = craftSystem.PlayEndingEffect( from, false, true, toolBroken, endquality, this );
				
				if ( bulkCraftContext != null )
                {
                    bulkCraftContext.LastMessage = null;
                    bulkCraftContext.Success++;
					if ( IsExceptional(item) )
						bulkCraftContext.Exceptional++;
				}
				else
					AttemptCompleted( from, craftSystem, tool, bulkCraftContext, num, canSendGump );
			}
			else if ( !allRequiredSkills )
			{
				// You don't have the required skills to attempt this item.
				AttemptCompleted( from, craftSystem, tool, bulkCraftContext, 1044153, canSendGump );
			}
			else
			{
				ConsumeType consumeType = ( UseAllRes ? ConsumeType.Half : ConsumeType.All );
				int resHue = 0;
				int maxAmount = 0;

				object message = null;

				// Not enough resource to craft it
				if ( !ConsumeRes( from, typeRes, craftSystem, ref resHue, ref maxAmount, consumeType, ref message, true ) )
				{
					AttemptCompleted( from, craftSystem, tool, bulkCraftContext, message, canSendGump );

					return;
				}

				if ( tool.UsesRemaining < 1 )
				{
					toolBroken = true;
					tool.Delete();
					BulkCraft.Cancel((PlayerMobile)from);
				}

				// SkillCheck failed.
				int num = craftSystem.PlayEndingEffect( from, true, true, toolBroken, endquality, this );

				if ( bulkCraftContext != null)
				{
					bulkCraftContext.Fail++;
					return;
				}
			
				AttemptCompleted( from, craftSystem, tool, bulkCraftContext, num, canSendGump );
			}
		}

		private void RunCommand( Mobile m_From, CraftSystem m_CraftSystem, CraftItem m_CraftItem, Type m_TypeRes, BaseTool m_Tool, BulkCraftContext bulkCraftContext )
		{
			m_From.DisruptiveAction();
			m_CraftSystem.PlayCraftEffect( m_From );
			m_From.EndAction( typeof( CraftSystem ) );

			bool canSendGump = m_Tool != null && !m_Tool.Deleted && m_Tool.UsesRemaining > 0;

			int badCraft = m_CraftSystem.CanCraft( m_From, m_Tool, m_CraftItem.m_Type );

			if ( badCraft > 0 )
			{
				AttemptCompleted( m_From, m_CraftSystem, m_Tool, bulkCraftContext, badCraft, canSendGump );

				return;
			}

			int quality = 1;
			bool allRequiredSkills = true;

			m_CraftItem.CheckSkills( m_From, m_TypeRes, m_CraftSystem, ref quality, ref allRequiredSkills, false );

			CraftContext context = m_CraftSystem.GetContext( m_From );

			if ( context == null )
				return;

			if ( typeof( CustomCraft ).IsAssignableFrom( m_CraftItem.ItemType ) )
			{
				CustomCraft cc = null;

				try{ cc = Activator.CreateInstance( m_CraftItem.ItemType, new object[] { m_From, m_CraftItem, m_CraftSystem, m_TypeRes, m_Tool, quality } ) as CustomCraft; }
				catch{}

				if ( cc != null )
					cc.EndCraftAction();

				return;
			}

			m_CraftItem.CompleteCraft( quality, m_From, m_CraftSystem, m_TypeRes, m_Tool, null, bulkCraftContext );
		}

		private class InternalTimer : Timer
		{
			private Mobile m_From;
			private int m_iCount;
			private int m_iCountMax;
			private CraftItem m_CraftItem;
			private CraftSystem m_CraftSystem;
			private Type m_TypeRes;
			private BaseTool m_Tool;

			public InternalTimer( Mobile from, CraftSystem craftSystem, CraftItem craftItem, Type typeRes, BaseTool tool, int iCountMax ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( craftSystem.Delay ), iCountMax )
			{
				m_From = from;
				m_CraftItem = craftItem;
				m_iCount = 0;
				m_iCountMax = iCountMax;
				m_CraftSystem = craftSystem;
				m_TypeRes = typeRes;
				m_Tool = tool;
			}

			protected override void OnTick()
			{
				m_iCount++;

				m_From.DisruptiveAction();

				if ( m_iCount < m_iCountMax )
				{
					m_CraftSystem.PlayCraftEffect( m_From );
				}
				else
				{
					m_From.EndAction( typeof( CraftSystem ) );

					int badCraft = m_CraftSystem.CanCraft( m_From, m_Tool, m_CraftItem.m_Type );

					if ( badCraft > 0 )
					{
						if ( m_Tool != null && !m_Tool.Deleted && m_Tool.UsesRemaining > 0 )
							m_From.SendGump( new CraftGump( m_From, m_CraftSystem, m_Tool, badCraft ) );
						else
							m_From.SendLocalizedMessage( badCraft );

						return;
					}

					int quality = 1;
					bool allRequiredSkills = true;

					m_CraftItem.CheckSkills( m_From, m_TypeRes, m_CraftSystem, ref quality, ref allRequiredSkills, false );

					CraftContext context = m_CraftSystem.GetContext( m_From );

					if ( context == null )
						return;

					if ( typeof( CustomCraft ).IsAssignableFrom( m_CraftItem.ItemType ) )
					{
						CustomCraft cc = null;

						try{ cc = Activator.CreateInstance( m_CraftItem.ItemType, new object[] { m_From, m_CraftItem, m_CraftSystem, m_TypeRes, m_Tool, quality } ) as CustomCraft; }
						catch{}

						if ( cc != null )
							cc.EndCraftAction();

						return;
					}

					m_CraftItem.CompleteCraft( quality, m_From, m_CraftSystem, m_TypeRes, m_Tool, null, null);
				}
			}
		}
	}
}
