using System;
using Server.Targeting;
using Server.Items;

namespace Server.Engines.Craft
{
	public enum EnhanceResult
	{
		None,
		NotInBackpack,
		BadItem,
		BadResource,
		AlreadyEnhanced,
		Success,
		Failure,
		Broken,
		NoResources,
		NoSkill
	}

	public class Enhance
	{
		public static EnhanceResult Invoke( Mobile from, CraftSystem craftSystem, BaseTool tool, Item item, CraftResource resource, Type resType, ref object resMessage )
		{
			if ( item == null )
				return EnhanceResult.BadItem;

			if ( !item.IsChildOf( from.Backpack ) )
				return EnhanceResult.NotInBackpack;

			if ( !item.ResourceCanChange() )
				return EnhanceResult.BadItem;

			if ( item is IArcaneEquip )
			{
				IArcaneEquip eq = (IArcaneEquip)item;
				if ( eq.IsArcane )
					return EnhanceResult.BadItem;
			}

			if ( Item.IsStandardResource( resource ) )
				return EnhanceResult.BadResource;

			if ( !Item.IsStandardResource( item.Resource ) )
				return EnhanceResult.AlreadyEnhanced;
			
			var itemType = item.GetType();
			int num = craftSystem.CanCraft( from, tool, itemType );
			
			if ( num > 0 )
			{
				if ( num > 0 )
				{
					resMessage = num;
					return EnhanceResult.None;
				}
			}

			CraftItem craftItem = craftSystem.CraftItems.SearchFor( itemType );
			if ( craftItem == null )
			{
				itemType = TryGetFallbackItemType(itemType);
				if (itemType != null)
					craftItem = craftSystem.CraftItems.SearchFor( itemType );
			}

			if ( craftItem == null || craftItem.Resources.Count == 0 )
				return EnhanceResult.BadItem;

			bool allRequiredSkills = false;
			if( craftItem.GetSuccessChance( from, resType, craftSystem, false, ref allRequiredSkills ) <= 0.0 )
				return EnhanceResult.NoSkill;

			CraftResourceInfo info = CraftResources.GetInfo( resource );

			if ( info == null || info.ResourceTypes.Length == 0 )
				return EnhanceResult.BadResource;

			CraftAttributeInfo attributes = info.AttributeInfo;

			if ( attributes == null )
				return EnhanceResult.BadResource;

			int resHue = 0, maxAmount = 0;

			if ( !craftItem.ConsumeRes( from, resType, craftSystem, ref resHue, ref maxAmount, ConsumeType.None, ref resMessage ) )
				return EnhanceResult.NoResources;

			var gloves = from.FindItemOnLayer(Layer.OneHanded) as AncientCraftingGloves;
			if (gloves != null)
				gloves.ConsumeCharge(from, craftSystem);

			int playerSkill = (int)from.Skills[craftSystem.MainSkill].Value;
			int materialSkill = (int)CraftResources.GetSkill( resource );
			int densityMod = (int)(item.Density) * 5;
			int breakMod = 20 + ( (int)(item.Density) * 10 );

			playerSkill = playerSkill - materialSkill + densityMod;

			EnhanceResult res = EnhanceResult.Success;

			if ( playerSkill < Utility.Random(100) )
			{
				res = EnhanceResult.Failure;

				if ( Utility.Random(100) > breakMod )
					res = EnhanceResult.Broken;
			}

			switch ( res )
			{
				case EnhanceResult.Broken:
				{
					if ( !craftItem.ConsumeRes( from, resType, craftSystem, ref resHue, ref maxAmount, ConsumeType.Half, ref resMessage ) )
						return EnhanceResult.NoResources;

					item.Delete();
					break;
				}
				case EnhanceResult.Success:
				{
					if ( !craftItem.ConsumeRes( from, resType, craftSystem, ref resHue, ref maxAmount, ConsumeType.All, ref resMessage ) )
						return EnhanceResult.NoResources;

					craftSystem.PlayCraftEffect( from );
					item.Resource = resource;

					break;
				}
				case EnhanceResult.Failure:
				{
					if ( !craftItem.ConsumeRes( from, resType, craftSystem, ref resHue, ref maxAmount, ConsumeType.Half, ref resMessage ) )
						return EnhanceResult.NoResources;

					break;
				}
			}

			return res;
		}

		public static void CheckResult( ref EnhanceResult res, int chance )
		{
			if ( res != EnhanceResult.Success )
				return; // we've already failed..

			int random = Utility.Random( 100 );

			if ( 10 > random )
				res = EnhanceResult.Failure;
			else if ( chance > random )
				res = EnhanceResult.Broken;
		}

		public static void BeginTarget( Mobile from, CraftSystem craftSystem, BaseTool tool )
		{
			CraftContext context = craftSystem.GetContext( from );

			if ( context == null )
				return;

			int lastRes = context.LastResourceIndex;
			CraftSubResCol subRes = craftSystem.CraftSubRes;

			if ( lastRes >= 0 && lastRes < subRes.Count )
			{
				CraftSubRes res = subRes.GetAt( lastRes );

				if ( from.Skills[craftSystem.MainSkill].Value < res.RequiredSkill )
				{
					from.SendGump( new CraftGump( from, craftSystem, tool, res.Message ) );
				}
				else
				{
					CraftResource resource = CraftResources.GetFromType( res.ItemType );

					if ( resource != CraftResource.None )
					{
						from.Target = new InternalTarget( craftSystem, tool, res.ItemType, resource );
						from.SendLocalizedMessage( 1061004 ); // Target an item to enhance with the properties of your selected material.
					}
					else
					{
						from.SendGump( new CraftGump( from, craftSystem, tool, 1061010 ) ); // You must select a special material in order to enhance an item with its properties.
					}
				}
			}
			else
			{
				from.SendGump( new CraftGump( from, craftSystem, tool, 1061010 ) ); // You must select a special material in order to enhance an item with its properties.
			}

		}

		private class InternalTarget : Target
		{
			private CraftSystem m_CraftSystem;
			private BaseTool m_Tool;
			private Type m_ResourceType;
			private CraftResource m_Resource;

			public InternalTarget( CraftSystem craftSystem, BaseTool tool, Type resourceType, CraftResource resource ) :  base ( 2, false, TargetFlags.None )
			{
				m_CraftSystem = craftSystem;
				m_Tool = tool;
				m_ResourceType = resourceType;
				m_Resource = resource;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item )
				{
					object message = null;
					EnhanceResult res = Enhance.Invoke( from, m_CraftSystem, m_Tool, (Item)targeted, m_Resource, m_ResourceType, ref message );

					switch ( res )
					{
						case EnhanceResult.NotInBackpack: message = 1061005; break; // The item must be in your backpack to enhance it.
						case EnhanceResult.AlreadyEnhanced: message = 1061012; break; // This item is already enhanced with the properties of a special material.
						case EnhanceResult.BadItem: message = 1061011; break; // You cannot enhance this type of item with the properties of the selected special material.
						case EnhanceResult.BadResource: message = 1061010; break; // You must select a special material in order to enhance an item with its properties.
						case EnhanceResult.Broken: message = 1061080; break; // You attempt to enhance the item, but fail catastrophically. The item is lost.
						case EnhanceResult.Failure: message = 1061082; break; // You attempt to enhance the item, but fail. Some material is lost in the process.
						case EnhanceResult.Success: message = 1061008; break; // You enhance the item with the properties of the special material.
						case EnhanceResult.NoSkill: message = 1044153; break; // You don't have the required skills to attempt this item.
					}

					from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, message ) );
				}
			}
		}

		private static Type TryGetFallbackItemType(Type itemType)
		{
			if (itemType == typeof(RangerArms)) return typeof(StuddedArms);
			if (itemType == typeof(RangerChest)) return typeof(StuddedChest);
			if (itemType == typeof(RangerGloves)) return typeof(StuddedGloves);
			if (itemType == typeof(RangerGorget)) return typeof(StuddedGorget);
			if (itemType == typeof(RangerLegs)) return typeof(StuddedLegs);

			if (itemType == typeof(HideChest)) return typeof(LeatherChest);
			
			return null;
		}
	}
}