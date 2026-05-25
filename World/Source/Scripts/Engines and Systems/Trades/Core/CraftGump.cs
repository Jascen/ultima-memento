using System;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Utilities;

namespace Server.Engines.Craft
{
	public class CraftGump : Gump
	{
		private Mobile m_From;
		private CraftSystem m_CraftSystem;
		private BaseTool m_Tool;

		private CraftPage m_Page;

		private const int LabelHue = 0x480;
		private const string TextColor = "#FFFFFF";
		private const int LabelColor = 0x7FFF;
		private const int FontColor = 0xFFFFFF;
		private const int moveUp = 0;
		private const int moveDown = 0;

		private enum CraftPage
		{
			None,
			PickResource,
			PickResource2
		}

		public CraftGump( Mobile from, CraftSystem craftSystem, BaseTool tool, object notice ) : this( from, craftSystem, tool, notice, CraftPage.None )
		{
		}

		private CraftGump( Mobile from, CraftSystem craftSystem, BaseTool tool, object notice, CraftPage page ) : base( 40, 40 )
		{
			m_From = from;
			m_CraftSystem = craftSystem;
			m_Tool = tool;
				craftSystem.Tools = m_Tool;
			m_Page = page;

			const int LEFT_WINDOW_WIDTH = 510;
			const int INFO_WINDOW_WIDTH = 270;
			const int LEFT_WINDOW_START = 10;
			const int INFO_PANEL_START = 527;

			const int HORIZONTAL_LINE = 2700;
			const int VERTICAL_LINE = 2701;
			const int BORDER_WIDTH = 2;

			CraftContext context = craftSystem.GetContext( from );

			from.CloseGump( typeof( CraftGump ) );
			from.CloseGump( typeof( CraftGumpItem ) );

			bool needsRecipe = context.LastMade != null && context.LastMade.Recipe != null && from is PlayerMobile && !((PlayerMobile)from).HasRecipe( context.LastMade.Recipe );

			if ( tool.Parent == from )
			{
				AddPage( 0 );

				AddImage(0, 0, craftSystem.GumpImage, Server.Misc.PlayerSettings.GetGumpHue( from ));
				// AddImage(0, 0, 9594, 0);
				
				{
					int y = 30;
					
					AddImageTiled(10, y, LEFT_WINDOW_WIDTH, BORDER_WIDTH, HORIZONTAL_LINE); // Top border
					AddImageTiled(215, y, BORDER_WIDTH, 260, VERTICAL_LINE); // Categories - Right Border
					y += 260;

					AddImageTiled(10, y, LEFT_WINDOW_WIDTH, BORDER_WIDTH, HORIZONTAL_LINE); // Bottom border
					y += 45;

					AddImageTiled(10, y, LEFT_WINDOW_WIDTH, BORDER_WIDTH, HORIZONTAL_LINE); // Bottom border
				}

				if ( craftSystem.GumpTitleNumber > 0 )
					AddHtmlLocalized( 10, 12, 510, 20, craftSystem.GumpTitleNumber, LabelColor, false, false );
				else
					AddHtml( 10, 12, 510, 20, craftSystem.GumpTitleString, false, false );

				AddHtmlLocalized( 10, 37+moveDown, 200, 22, 1044010, LabelColor, false, false ); // <CENTER>CATEGORIES</CENTER>
				AddHtmlLocalized( 215, 37+moveDown, 305, 22, 1044011, LabelColor, false, false ); // <CENTER>SELECTIONS</CENTER>

				// Info Panel
				if ( craftSystem.ShowGumpInfo && context.ItemID > 0)
				{
					AddImageTiled(INFO_PANEL_START, 0, INFO_WINDOW_WIDTH, 437, 2702);

					int x = INFO_PANEL_START + 11;
					int y = 7;
					AddItem( x, y, context.ItemID, context.Hue );
					y += 100;

					AddHtml( x, y, 254, 280, @"<BODY><BASEFONT Color=" + TextColor + ">" + context.Description + "</BASEFONT></BODY>", false, true);
					y += 290;

					AddImageTiled(INFO_PANEL_START + 10, y, INFO_WINDOW_WIDTH - 15, BORDER_WIDTH, HORIZONTAL_LINE); // Top border -- Margin
					y += 10;

					if ( context.LastMade != null && tool is IRunicTool && !TypeUtilities.IsEquipmentType(context.LastMade.ItemType ) )
					{
						AddHtml( x, y, INFO_WINDOW_WIDTH, 40, String.Format( "<BASEFONT COLOR=#{0:X6}>No runic benefit</BASEFONT>", FontColor ), false, false );
					}
					else if ( needsRecipe )
					{
						AddHtml( x, y, INFO_WINDOW_WIDTH, 40, String.Format( "<BASEFONT COLOR=#{0:X6}>You don't know this recipe</BASEFONT>", FontColor ), false, false );
					}
					else if ( CraftSystem.AllowManyCraft( m_Tool ) && context.LastMade != null && !context.LastMade.UseAllRes)
					{
						AddHtml( x, y, 100, 40, String.Format( "<BASEFONT COLOR=#{0:X6}>Craft Amount:</BASEFONT>", FontColor ), false, false );
						AddCraftAmountButtons( this, x + 90, y - 3, GetButtonID( 6, 2 ) );
					}
					else if ( context.LastMade != null )
					{
						AddButton( INFO_PANEL_START + INFO_WINDOW_WIDTH - 85, y - 3, 4023, 4024, GetButtonID( 6, 2 ), GumpButtonType.Reply, 0 );
						AddHtmlLocalized( INFO_PANEL_START + INFO_WINDOW_WIDTH - 50, y, 50, 18, 1044132, LabelColor, false, false ); // MAKE LAST
					}
				}

				AddButton( LEFT_WINDOW_START, 402+moveUp, 4017, 4019, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 46, 405+moveUp, 150, 18, 1011441, LabelColor, false, false ); // EXIT

				// Break down option
				if ( craftSystem.BreakDown )
				{
					AddButton( LEFT_WINDOW_START, 342+moveUp, 4005, 4007, GetButtonID( 6, 1 ), GumpButtonType.Reply, 0 );
					AddHtmlLocalized( 46, 345+moveUp, 150, 18, 1044259, LabelColor, false, false ); // BREAK DOWN
				}
				// ****************************************

				// Repair option
				if ( craftSystem.Repair )
				{
					AddButton( LEFT_WINDOW_START + 138, 342+moveUp, 4005, 4007, GetButtonID( 6, 5 ), GumpButtonType.Reply, 0 );
					AddHtmlLocalized( 184, 345+moveUp, 150, 18, 1044260, LabelColor, false, false ); // REPAIR ITEM
				}
				// ****************************************

				// Enhance option
				if ( craftSystem.CanEnhance )
				{
					AddButton( LEFT_WINDOW_START + 138, 371+moveUp, 4005, 4007, GetButtonID( 6, 8 ), GumpButtonType.Reply, 0 );
					AddTooltip("Enhance an item with the selected resource");
					AddHtmlLocalized( 184, 374+moveUp, 150, 18, 1061001, LabelColor, false, false ); // ENHANCE ITEM
				}

				AddButton( LEFT_WINDOW_START + 282, 342+moveUp, 4005, 4007, GetButtonID( 6, 9 ), GumpButtonType.Reply, 0 );
				AddTooltip("A list of items you can craft to gain skill");
				AddHtml( LEFT_WINDOW_START + 318, 345+moveUp, 150, 18, string.Format("<BASEFONT COLOR=#{0:X6}>SKILL CRAFT LIST</BASEFONT>", FontColor ), false, false );

				// Set crafting container
				AddButton( LEFT_WINDOW_START + 282, 371+moveUp, 4005, 4007, GetButtonID( 6, 10 ), GumpButtonType.Reply, 0 );
				AddHtml( LEFT_WINDOW_START + 318, 374+moveUp, 200, 18, string.Format("<BASEFONT COLOR=#{0:X6}>SET CRAFTING CONTAINER</BASEFONT>", FontColor ), false, false );

				// ****************************************

				if ( notice is int && (int)notice > 0 )
					AddHtmlLocalized( LEFT_WINDOW_START, 295+moveUp, LEFT_WINDOW_WIDTH, 40, (int)notice, LabelColor, false, false );
				else if ( notice is string )
					AddHtml( LEFT_WINDOW_START, 295+moveUp, LEFT_WINDOW_WIDTH, 40, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", FontColor, notice ), false, false );
				else if ( m_Tool is BaseRunicTool )
				{
					string material = "This tool will create magical armor or weapons into " + CraftResources.GetName( ((BaseRunicTool)m_Tool).Resource ) + " material from any resource.";
					AddHtml( LEFT_WINDOW_START, 295+moveUp, LEFT_WINDOW_WIDTH, 40, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", FontColor, material ), false, false );
				}

				if ( craftSystem.CraftSubRes.Init )
				{
					string nameString = craftSystem.CraftSubRes.NameString;
					int nameNumber = craftSystem.CraftSubRes.NameNumber;

					int resIndex = ( context == null ? -1 : context.LastResourceIndex );

					Type resourceType = craftSystem.CraftSubRes.ResType;

					if ( resIndex > -1 )
					{
						CraftSubRes subResource = craftSystem.CraftSubRes.GetAt( resIndex );

						nameString = subResource.NameString;
						nameNumber = subResource.NameNumber;
						resourceType = subResource.ItemType;
					}

					int resourceCount = 0;

					if ( from.Backpack != null )
					{
						Item[] items = from.Backpack.FindItemsByType( resourceType, true );

						for ( int i = 0; i < items.Length; ++i )
							resourceCount += items[i].Amount;
					}

					AddButton( LEFT_WINDOW_START + 138, 402+moveUp, 4005, 4007, GetButtonID( 6, 0 ), GumpButtonType.Reply, 0 );

					if ( nameNumber > 0 )
						AddHtmlLocalized( 184, 405+moveUp, 250, 18, nameNumber, resourceCount.ToString(), LabelColor, false, false );
					else
						AddLabel( 184, 405+moveUp, LabelHue, String.Format( "{0} ({1} Available)", nameString, resourceCount ) );
				}

				CreateGroupList();

				if ( page == CraftPage.PickResource )
					CreateResList( false, from );
				else if ( page == CraftPage.PickResource2 )
					CreateResList( true, from );
				else if ( context != null && context.LastGroupIndex > -1 )
					CreateItemList( context.LastGroupIndex, from );
			}
		}

		private const int CRAFT_AMOUNT_ICON1 = 11316;
		private const int CRAFT_AMOUNT_ICON10 = 11317;
		private const int CRAFT_AMOUNT_ICON100 = 11318;

		public static void AddCraftAmountButtons( Gump g, int x, int y, int baseButtonId )
		{
			g.AddButton( x, y, CRAFT_AMOUNT_ICON1, CRAFT_AMOUNT_ICON1, baseButtonId, GumpButtonType.Reply, 0 );
			x += 17;
			x += 22;
			g.AddButton( x, y, CRAFT_AMOUNT_ICON10, CRAFT_AMOUNT_ICON10, 1000 + baseButtonId, GumpButtonType.Reply, 0 );
			x += 23;
			x += 22;
			g.AddButton( x, y, CRAFT_AMOUNT_ICON100, CRAFT_AMOUNT_ICON100, 2000 + baseButtonId, GumpButtonType.Reply, 0 );
			x += 31;
			x += 22;
			
			// Custom amount button
			int startX = x;
			g.AddButton( startX, y, CRAFT_AMOUNT_ICON10, CRAFT_AMOUNT_ICON10, 3000 + baseButtonId, GumpButtonType.Reply, 0 );
			g.AddTooltip( "Custom amount" );
			y += 3;
			g.AddImage(startX, y, 13003);
			g.AddImage(startX + 7, y, 13004);
			g.AddImage(startX + 14, y, 13004);
			g.AddImage(startX + 21, y, 13005);
			TextDefinition.AddHtmlText(g, startX + 1, y - 2, 28, 20, "<CENTER>xx</CENTER>", HtmlColors.WHITE);
		}

		public static void DoCraft( Mobile from, CraftSystem craftSystem, BaseTool tool, CraftItem item, int amount )
		{
			bool needsRecipe = item.Recipe != null && from is PlayerMobile && !((PlayerMobile)from).HasRecipe( item.Recipe );

			if ( needsRecipe )
				return;

			int num = craftSystem.CanCraft( from, tool, item.ItemType );

			CraftSystem.SetDescription( craftSystem.GetContext( from ), tool, item.ItemType, craftSystem, item.NameString, from, item );

			if ( num > 0 )
			{
				from.CloseGump( typeof( CraftGump ) );
				from.CloseGump( typeof( CraftGumpItem ) );
				from.SendGump( new CraftGump( from, craftSystem, tool, num ) );
			}
			else
			{
				Type type = null;

				CraftContext context = craftSystem.GetContext( from );

				if ( context != null )
				{
					CraftSubResCol res = craftSystem.CraftSubRes;
					int resIndex = context.LastResourceIndex;

					if ( resIndex >= 0 && resIndex < res.Count )
						type = res.GetAt( resIndex ).ItemType;
				}

				if ( 1 < amount )
					craftSystem.BulkCreateItem( from, item.ItemType, type, tool, item, amount );
				else
					craftSystem.CreateItem( from, item.ItemType, type, tool, item );
			}
		}

		public void CreateResList( bool opt, Mobile from )
		{
			const int RESOURCE_ITEMS_PER_PAGE = 8;
			CraftSubResCol res = m_CraftSystem.CraftSubRes;

			for ( int i = 0; i < res.Count; ++i )
			{
				int index = i % RESOURCE_ITEMS_PER_PAGE;

				CraftSubRes subResource = res.GetAt( i );

				if ( index == 0 )
				{
					// Prev page
					if ( i > 0 )
						AddButton( 485, 260+moveDown, 4005, 4007, 0, GumpButtonType.Page, (i / RESOURCE_ITEMS_PER_PAGE) + 1 );

					AddPage( (i / RESOURCE_ITEMS_PER_PAGE) + 1 );

					// Next page
					if ( i > 0 )
						AddButton( 455, 260+moveDown, 4014, 4015, 0, GumpButtonType.Page, i / RESOURCE_ITEMS_PER_PAGE );

					// Use Material Color vs Do Not Color
					CraftContext context = m_CraftSystem.GetContext( m_From );
					AddButton( 220, 260+moveDown, 4005, 4007, GetButtonID( 6, 4 ), GumpButtonType.Reply, 0 );
					AddHtmlLocalized( 255, 263+moveDown, 200, 18, (context == null || !context.DoNotColor) ? 1061591 : 1061590, LabelColor, false, false );

					TextDefinition.AddHtmlText(this, 255, 60, 100, 20, "Resource", HtmlColors.WHITE);
					TextDefinition.AddHtmlText(this, 255 + 172 + 56, 60, 35, 20, "<RIGHT>Skill</RIGHT>", HtmlColors.WHITE);
				}

				int resourceCount = 0;

				if ( from.Backpack != null )
				{
					Item[] items = from.Backpack.FindItemsByType( subResource.ItemType, true );

					for ( int j = 0; j < items.Length; ++j )
						resourceCount += items[j].Amount;
				}

				if ( m_From.Skills[m_CraftSystem.MainSkill].Value < subResource.RequiredSkill )
					AddImage( 220 + 9, 80+5+moveDown + (index * 20), 2092 ); // Lock
				else
					AddButton( 220, 80+moveDown + (index * 20), 4005, 4007, GetButtonID( 5, i ), GumpButtonType.Reply, 0 );

				if ( subResource.NameNumber > 0 )
					AddHtmlLocalized( 255, 82+moveDown + (index * 20), 250, 18, subResource.NameNumber, resourceCount.ToString(), LabelColor, false, false );
				else
					AddLabel( 255, 82+moveDown + ( index * 20 ), LabelHue, String.Format( "{0} ({1})", subResource.NameString, resourceCount ) );

				TextDefinition.AddHtmlText(this, 255 + 172 + 56, 82+moveDown + ( index * 20 ), 35, 20, string.Format("<RIGHT>{0:F1}</RIGHT>", subResource.RequiredSkill), HtmlColors.WHITE);
			}
		}

		public void CreateMakeLastList()
		{
			CraftContext context = m_CraftSystem.GetContext( m_From );

			if ( context == null )
				return;

			List<CraftItem> items = context.Items;

			if ( items.Count > 0 )
			{
				for ( int i = 0; i < items.Count; ++i )
				{
					int index = i % 10;

					CraftItem craftItem = items[i];

					if ( index == 0 )
					{
						if ( i > 0 )
						{
							AddButton( 370, 260+moveDown, 4005, 4007, 0, GumpButtonType.Page, (i / 10) + 1 );
							AddHtmlLocalized( 405, 263+moveDown, 100, 18, 1044045, LabelColor, false, false ); // NEXT PAGE
						}

						AddPage( (i / 10) + 1 );

						if ( i > 0 )
						{
							AddButton( 220, 260+moveDown, 4014, 4015, 0, GumpButtonType.Page, i / 10 );
							AddHtmlLocalized( 255, 263+moveDown, 100, 18, 1044044, LabelColor, false, false ); // PREV PAGE
						}
					}

					// if ( CraftSystem.AllowManyCraft( m_Tool ) )
					// {
					// 	AddButton( 220, 60+moveDown + (index * 20), 4011, 4012, GetButtonID( 4, i ), GumpButtonType.Reply, 0 ); // LAST 10 ITEM INFO BUTTON
					// 	AddButton( 411, 60+moveDown + (index * 20), 11316, 11316, GetButtonID( 3, i ), GumpButtonType.Reply, 0 );
					// 	AddButton( 441, 60+moveDown + (index * 20), 11317, 11317, 1000+GetButtonID( 3, i ), GumpButtonType.Reply, 0 );
					// 	AddButton( 476, 60+moveDown + (index * 20), 11318, 11318, 2000+GetButtonID( 3, i ), GumpButtonType.Reply, 0 );
					// }
					// else
					{
						AddButton( 220, 60+moveDown + (index * 20), 4005, 4007, GetButtonID( 3, i ), GumpButtonType.Reply, 0 ); // LAST 10 MAKE ITEM BUTTON
						AddButton( 480, 60+moveDown + (index * 20), 4011, 4012, GetButtonID( 4, i ), GumpButtonType.Reply, 0 ); // LAST 10 ITEM INFO BUTTON
					}

					if ( craftItem.NameNumber > 0 )
						AddHtmlLocalized( 255, 62+moveDown + (index * 20), 220, 18, craftItem.NameNumber, LabelColor, false, false );
					else
						AddLabel( 255, 62+moveDown + (index * 20), LabelHue, craftItem.NameString );
				}
			}
			else
			{
				// NOTE: This is not as OSI; it is an intentional difference

				AddHtmlLocalized( 230, 62+moveDown, 200, 22, 1044165, LabelColor, false, false ); // You haven't made anything yet.
			}
		}

		public void CreateItemList( int selectedGroup, Mobile from )
		{
			if ( selectedGroup == 501 ) // 501 : Last 10
			{
				CreateMakeLastList();
				return;
			}

			CraftGroupCol craftGroupCol = m_CraftSystem.CraftGroups;
			CraftGroup craftGroup = craftGroupCol.GetAt( selectedGroup );
			CraftItemCol craftItemCol = craftGroup.CraftItems;

			for ( int i = 0; i < craftItemCol.Count; ++i )
			{
				int index = i % 10;

				CraftItem craftItem = craftItemCol.GetAt( i );

				if ( index == 0 )
				{
					if ( i > 0 )
					{
						AddButton( 370, 260+moveDown, 4005, 4007, 0, GumpButtonType.Page, (i / 10) + 1 );
						AddHtmlLocalized( 405, 263+moveDown, 100, 18, 1044045, LabelColor, false, false ); // NEXT PAGE
					}

					AddPage( (i / 10) + 1 );

					if ( i > 0 )
					{
						AddButton( 220, 260+moveDown, 4014, 4015, 0, GumpButtonType.Page, i / 10 );
						AddHtmlLocalized( 255, 263+moveDown, 100, 18, 1044044, LabelColor, false, false ); // PREV PAGE
					}
				}

				bool needsRecipe = craftItem.Recipe != null && from is PlayerMobile && !((PlayerMobile)from).HasRecipe( craftItem.Recipe );

				if ( m_Tool is IRunicTool && !TypeUtilities.IsEquipmentType( craftItem.ItemType ) )
				{
					AddImage( 239, 66+moveDown + (index * 20), 2449 );
					AddTooltip("No runic benefit");
				}
				else if ( CraftSystem.AllowManyCraft( m_Tool ) && MySettings.S_CraftButtons )
				{
					AddButton( 220, 60+moveDown + (index * 20), 4011, 4012, GetButtonID( 2, i ), GumpButtonType.Reply, 0 ); // ITEM LIST INFO BUTTON
					if (0 < craftItem.NameNumber)
						AddTooltip(craftItem.NameNumber);
					else
						AddTooltip(craftItem.NameString);

					if ( !needsRecipe )
					{
						AddButton( 411, 60+moveDown + (index * 20), 13002, 13002, GetButtonID( 1, i ), GumpButtonType.Reply, 0 );
						AddButton( 441, 60+moveDown + (index * 20), 11317, 11317, 1000+GetButtonID( 1, i ), GumpButtonType.Reply, 0 );
						AddButton( 476, 60+moveDown + (index * 20), 11318, 11318, 2000+GetButtonID( 1, i ), GumpButtonType.Reply, 0 );
					}
				}
				else
				{
					if ( !needsRecipe )
					{
						AddButton( 230, 60+moveDown + (index * 20), 13002, 13002, GetButtonID( 1, i ), GumpButtonType.Reply, 0 ); // ITEM LIST MAKE BUTTON
						AddButton( 485, 60+moveDown + (index * 20), 4011, 4012, GetButtonID( 2, i ), GumpButtonType.Reply, 0 ); // ITEM LIST INFO BUTTON
					}
					else
					{
						if ( m_CraftSystem is DefInscription )
						{
							AddImage( 239, 66+moveDown + (index * 20), 2449 );
							AddTooltip("This cannot be crafted.");
						}
						else
						{
							AddImage( 239, 65+moveDown + (index * 20), 2092 );
							AddButton( 485, 60+moveDown + (index * 20), 4011, 4012, GetButtonID( 2, i ), GumpButtonType.Reply, 0 ); // ITEM LIST INFO BUTTON
						}
					}

					if (0 < craftItem.NameNumber)
						AddTooltip(craftItem.NameNumber);
					else
						AddTooltip(craftItem.NameString);
				}

				if ( craftItem.NameNumber > 0 )
					AddHtmlLocalized( 265, 62+moveDown + (index * 20), 220, 18, craftItem.NameNumber, LabelColor, false, false );
				else
					AddLabel( 265, 62+moveDown + (index * 20), LabelHue, craftItem.NameString );
			}
		}

		public int CreateGroupList()
		{
			CraftGroupCol craftGroupCol = m_CraftSystem.CraftGroups;

			AddButton( 15, 60+moveDown, 4005, 4007, GetButtonID( 6, 3 ), GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 50, 63+moveDown, 150, 18, 1044014, LabelColor, false, false ); // LAST TEN

			for ( int i = 0; i < craftGroupCol.Count; i++ )
			{
				CraftGroup craftGroup = craftGroupCol.GetAt( i );

				AddButton( 15, 80+moveDown + (i * 20), 4005, 4007, GetButtonID( 0, i ), GumpButtonType.Reply, 0 );

				if ( craftGroup.NameNumber > 0 )
					AddHtmlLocalized( 50, 82+moveDown + (i * 20), 150, 18, craftGroup.NameNumber, LabelColor, false, false );
				else
					AddLabel( 50, 82+moveDown + (i * 20), LabelHue, craftGroup.NameString );
			}

			return craftGroupCol.Count;
		}

		public static int GetButtonID( int type, int index )
		{
			return 1 + type + (index * 7);
		}

		public void CraftItem( CraftItem item, int amount )
		{
			DoCraft( m_From, m_CraftSystem, m_Tool, item, amount );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			int buttonID = info.ButtonID;

			if ( buttonID <= 0 )
			{
				m_From.CloseGump( typeof( CraftGump ) );
				m_From.CloseGump( typeof( CraftGumpItem ) );

				return; // Canceled
			}

			int toMake;
			if ( buttonID > 3000 && CraftSystem.AllowManyCraft( m_Tool ) )
			{
				int baseButtonId = buttonID - 3000;
				if ( baseButtonId == GetButtonID( 6, 2 ) )
				{
					CraftContext promptContext = m_CraftSystem.GetContext( m_From );
					if ( promptContext != null && promptContext.LastMade != null )
						CraftAmountPrompt.Begin( m_From, m_CraftSystem, m_Tool, promptContext.LastMade, CraftAmountSource.CraftGumpMakeLast );
				}

				return;
			}
			else if ( buttonID > 2000 && CraftSystem.AllowManyCraft( m_Tool ) )
			{
				buttonID = buttonID - 2000;
				toMake = 100;
			}
			else if ( buttonID > 1000 && CraftSystem.AllowManyCraft( m_Tool ) )
			{
				buttonID = buttonID - 1000;
				toMake = 10;
			}
			else
				toMake = 1;

			buttonID = buttonID - 1;
			int type = buttonID % 7;
			int index = buttonID / 7;

			CraftSystem system = m_CraftSystem;
			CraftGroupCol groups = system.CraftGroups;
			CraftContext context = system.GetContext( m_From );

			switch ( type )
			{
				case 0: // Show group
				{
					if ( context == null )
						break;

					if ( index >= 0 && index < groups.Count )
					{
						context.LastGroupIndex = index;
						m_From.SendGump( new CraftGump( m_From, system, m_Tool, null ) );
					}

					break;
				}
				case 1: // Create item
				{
					if ( context == null )
						break;

					int groupIndex = context.LastGroupIndex;

					if ( groupIndex >= 0 && groupIndex < groups.Count )
					{
						CraftGroup group = groups.GetAt( groupIndex );

						if ( index >= 0 && index < group.CraftItems.Count )
							CraftItem( group.CraftItems.GetAt( index ), toMake );
					}

					break;
				}
				case 2: // Item details
				{
					if ( context == null )
						break;

					int groupIndex = context.LastGroupIndex;

					if ( groupIndex >= 0 && groupIndex < groups.Count )
					{
						CraftGroup group = groups.GetAt( groupIndex );

						if ( index >= 0 && index < group.CraftItems.Count )
							m_From.SendGump( new CraftGumpItem( m_From, system, group.CraftItems.GetAt( index ), m_Tool ) );
					}

					break;
				}
				case 3: // Create item (last 10)
				{
					if ( context == null )
						break;

					List<CraftItem> lastTen = context.Items;

					if ( index >= 0 && index < lastTen.Count )
						CraftItem( lastTen[index], toMake );

					break;
				}
				case 4: // Item details (last 10)
				{
					if ( context == null )
						break;

					List<CraftItem> lastTen = context.Items;

					if ( index >= 0 && index < lastTen.Count )
						m_From.SendGump( new CraftGumpItem( m_From, system, lastTen[index], m_Tool ) );

					break;
				}
				case 5: // Resource selected
				{
					if ( m_Page == CraftPage.PickResource && index >= 0 && index < system.CraftSubRes.Count )
					{
						int groupIndex = ( context == null ? -1 : context.LastGroupIndex );

						CraftSubRes res = system.CraftSubRes.GetAt( index );

						if ( m_From.Skills[system.MainSkill].Value < res.RequiredSkill )
						{
							m_From.SendGump( new CraftGump( m_From, system, m_Tool, res.Message ) );
						}
						else
						{
							if ( context != null )
								context.LastResourceIndex = index;

							CraftSystem.SetCraftResource( context, index, res );

							if ( context.LastMade != null )
								CraftSystem.SetDescription( context, m_Tool, (context.LastMade).ItemType, system, (context.LastMade).NameString, m_From, context.LastMade );
							else if ( context.ItemSelected != null )
								CraftSystem.SetDescription( context, m_Tool, context.ItemSelected, system, context.NameString, m_From, null );

							m_From.SendGump( new CraftGump( m_From, system, m_Tool, null ) );
						}
					}

					break;
				}
				case 6: // Misc. buttons
				{
					switch ( index )
					{
						case 0: // Resource selection
						{
							if ( system.CraftSubRes.Init )
								m_From.SendGump( new CraftGump( m_From, system, m_Tool, null, CraftPage.PickResource ) );

							break;
						}
						case 1: // Break down item
						{
							if ( system.BreakDown )
								BreakDown.Do( m_From, system, m_Tool );

							break;
						}
						case 2: // Make last
						{
							if ( context == null )
								break;

							CraftItem item = context.LastMade;

							if ( item != null )
								CraftItem( item, toMake );
							else
								m_From.SendGump( new CraftGump( m_From, m_CraftSystem, m_Tool, 1044165, m_Page ) ); // You haven't made anything yet.

							break;
						}
						case 3: // Last 10
						{
							if ( context == null )
								break;

							context.LastGroupIndex = 501;
							m_From.SendGump( new CraftGump( m_From, system, m_Tool, null ) );

							break;
						}
						case 4: // Toggle use resource hue
						{
							if ( context == null )
								break;

							context.DoNotColor = !context.DoNotColor;

							m_From.SendGump( new CraftGump( m_From, m_CraftSystem, m_Tool, null, m_Page ) );

							break;
						}
						case 5: // Repair item
						{
							if ( system.Repair )
								Repair.Do( m_From, system, m_Tool );

							break;
						}
						case 8: // Enhance item
						{
							if ( system.CanEnhance )
								Enhance.BeginTarget( m_From, system, m_Tool );

							break;
						}
						case 9: // Open Skill Craft List
						{
							m_From.SendGump( new CraftGump( m_From, system, m_Tool, null ) );
							m_From.CloseGump( typeof( CraftItemSkillListGump ) );
							m_From.SendGump( new CraftItemSkillListGump( m_From, m_CraftSystem, m_Tool ) );
							break;
						}
						case 10: // Set crafting container
						{
							BaseContainer.ContainerSetTarget( m_From, 2 );
							context.AutomaticallyMarkQuestItems = !context.AutomaticallyMarkQuestItems;
							m_From.SendGump( new CraftGump( m_From, system, m_Tool, null ) );

							break;
						}
					}

					break;
				}
			}
		}
	}
}