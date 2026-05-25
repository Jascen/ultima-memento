using System;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Misc;
using Server.Utilities;

namespace Server.Engines.Craft
{
    public class CraftGumpItem : Gump
	{
		private Mobile m_From;
		private CraftSystem m_CraftSystem;
		private CraftItem m_CraftItem;
		private BaseTool m_Tool;

		private const int LabelHue = 0x480; // 0x384
		private const int RedLabelHue = 0x20;

		private const string TextColor = "#FFFFFF";
		private const int LabelColor = 0x7FFF;
		private const int RedLabelColor = 0x6400;

		private const int GreyLabelColor = 0x3DEF;

		private int m_OtherCount;

		public CraftGumpItem( Mobile from, CraftSystem craftSystem, CraftItem craftItem, BaseTool tool ) : base( 40, 40 )
		{
			m_From = from;
			m_CraftSystem = craftSystem;
			m_CraftItem = craftItem;
			m_Tool = tool;

			const int INFO_WINDOW_WIDTH = 270;
			const int INFO_PANEL_START = 530;

			const int HORIZONTAL_LINE = 2700;
			const int VERTICAL_LINE = 2701;
			const int BORDER_WIDTH = 2;

			CraftContext context = craftSystem.GetContext( from );

			from.CloseGump( typeof( CraftGump ) );
			from.CloseGump( typeof( CraftGumpItem ) );
			if ( tool.Parent == from )
			{
				AddPage( 0 );
				AddImage(0, 0, craftSystem.GumpImage, Server.Misc.PlayerSettings.GetGumpHue( from ));

				AddImage(0, 0, 9595, 0);

				AddHtmlLocalized( 170, 40, 150, 20, 1044053, LabelColor, false, false ); // ITEM
				AddHtmlLocalized( 10, 192, 150, 22, 1044054, LabelColor, false, false ); // <CENTER>SKILLS</CENTER>
				AddHtmlLocalized( 10, 277, 150, 22, 1044055, LabelColor, false, false ); // <CENTER>MATERIALS</CENTER>
				AddHtmlLocalized( 10, 362, 150, 22, 1044056, LabelColor, false, false ); // <CENTER>OTHER</CENTER>

				if ( craftSystem.GumpTitleNumber > 0 )
					AddHtmlLocalized( 10, 12, 510, 20, craftSystem.GumpTitleNumber, LabelColor, false, false );
				else
					AddHtml( 10, 12, 510, 20, craftSystem.GumpTitleString, false, false );

				AddButton( 15, 405, 4014, 4016, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 50, 408, 150, 18, 1044150, LabelColor, false, false ); // BACK
					
				bool needsRecipe = craftItem.Recipe != null && from is PlayerMobile && !((PlayerMobile)from).HasRecipe( craftItem.Recipe );

				if ( craftItem.NameNumber > 0 )
					AddHtmlLocalized( 330, 40, 180, 18, craftItem.NameNumber, LabelColor, false, false );
				else
					AddLabel( 330, 40, LabelHue, craftItem.NameString );

				if ( craftItem.UseAllRes )
					AddHtmlLocalized( 170, 302 + (m_OtherCount++ * 20), 310, 18, 1048176, LabelColor, false, false ); // Makes as many as possible at once

				// DrawItem();
				Type type = m_CraftItem.ItemType;
				CraftSystem.SetDescription( m_CraftSystem.GetContext( m_From ), m_Tool, type, m_CraftSystem, m_CraftItem.NameString, m_From, m_CraftItem );
				m_ShowExceptionalChance = m_CraftItem.IsMarkable( type ) ;
				DrawSkill();
				DrawResource();

				// if ( craftSystem.ShowGumpInfo )
				// 	AddHtml( 538, 7, 254, 422, @"<BODY><BASEFONT Color=" + TextColor + ">" + context.Description + "</BASEFONT></BODY>", false, true);

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

					if ( m_Tool is IRunicTool && !TypeUtilities.IsEquipmentType( craftItem.ItemType ) )
					{
						AddHtml( x, y, INFO_WINDOW_WIDTH, 40, String.Format( "<BASEFONT COLOR={0}>No runic benefit</BASEFONT>", TextColor ), false, false );
					}
					else if ( needsRecipe )
					{
						AddHtml( x, y, INFO_WINDOW_WIDTH, 40, String.Format( "<BASEFONT COLOR={0}>You don't know this recipe</BASEFONT>", TextColor ), false, false );
					}
					else if ( CraftSystem.AllowManyCraft( m_Tool ) && !m_CraftItem.UseAllRes)
					{
						AddHtml( x, y, 100, 40, String.Format( "<BASEFONT COLOR={0}>Craft Amount:</BASEFONT>", TextColor ), false, false );
						CraftGump.AddCraftAmountButtons( this, x + 90, y - 3, 1 );
					}
					else
					{
						AddButton( INFO_PANEL_START + INFO_WINDOW_WIDTH - 85, y - 3, 4023, 4024, 1, GumpButtonType.Reply, 0 );
						AddHtmlLocalized( INFO_PANEL_START + INFO_WINDOW_WIDTH - 50, y, 50, 18, 1044132, LabelColor, false, false ); // MAKE LAST
					}
				}

				if( craftItem.RequiredExpansion != Expansion.None )
				{
					bool supportsEx = (from.NetState != null && from.NetState.SupportsExpansion( craftItem.RequiredExpansion ));
					TextDefinition.AddHtmlText( this, 170, 302 + (m_OtherCount++ * 20), 310, 18, RequiredExpansionMessage( craftItem.RequiredExpansion ), false, false, supportsEx ? LabelColor : RedLabelColor, supportsEx ? LabelHue : RedLabelHue );
				}
			}
		}

		private TextDefinition RequiredExpansionMessage( Expansion expansion )
		{
			switch( expansion )
			{
				case Expansion.SE:
					return 1063363; // * Requires the "Samurai Empire" expansion
				case Expansion.ML:
					return 1072651; // * Requires the "Mondain's Legacy" expansion
				default:
					return String.Format( "* Requires the \"{0}\" expansion", ExpansionInfo.GetInfo( expansion ).Name );
			}
		}

		private bool m_ShowExceptionalChance;

		public void DrawItem()
		{
			Type type = m_CraftItem.ItemType;

			CraftSystem.SetDescription( m_CraftSystem.GetContext( m_From ), m_Tool, type, m_CraftSystem, m_CraftItem.NameString, m_From, m_CraftItem );

			AddItem( 20, 50, (m_CraftSystem.GetContext( m_From )).ItemID, (m_CraftSystem.GetContext( m_From )).Hue );

			m_ShowExceptionalChance = false;

			if ( m_CraftItem.IsMarkable( type ) )
			{
				//AddHtmlLocalized( 170, 302 + (m_OtherCount++ * 20), 310, 18, 1044059, LabelColor, false, false ); // This item may hold its maker's mark
				m_ShowExceptionalChance = true;
			}
		}

		public void DrawSkill()
		{
			for ( int i = 0; i < m_CraftItem.Skills.Count; i++ )
			{
				CraftSkill skill = m_CraftItem.Skills.GetAt( i );
				double minSkill = skill.MinSkill, maxSkill = skill.MaxSkill;

				if ( minSkill < 0 )
					minSkill = 0;

				AddHtmlLocalized( 170, 132 + (i * 20), 200, 18, 1044060 + (int)skill.SkillToMake, LabelColor, false, false );
				AddLabel( 430, 132 + (i * 20), LabelHue, String.Format( "{0:F1}", minSkill ) );
			}

			CraftSubResCol res = m_CraftSystem.CraftSubRes;
			int resIndex = -1;

			CraftContext context = m_CraftSystem.GetContext( m_From );

			if ( context != null )
				resIndex = context.LastResourceIndex;

			bool allRequiredSkills = true;
			double chance = m_CraftItem.GetSuccessChance( m_From, resIndex > -1 ? res.GetAt( resIndex ).ItemType : null, m_CraftSystem, false, ref allRequiredSkills );
			double excepChance = m_CraftItem.GetExceptionalChance( m_CraftSystem, chance, m_From );

			if ( chance < 0.0 )
				chance = 0.0;
			else if ( chance > 1.0 )
				chance = 1.0;

			AddHtmlLocalized( 170, 80, 250, 18, 1044057, LabelColor, false, false ); // Success Chance:
			AddLabel( 430, 80, LabelHue, String.Format( "{0:F1}%", chance * 100 ) );

			if ( m_ShowExceptionalChance )
			{
				if( excepChance < 0.0 )
					excepChance = 0.0;
				else if( excepChance > 1.0 )
					excepChance = 1.0;

				AddHtmlLocalized( 170, 100, 250, 18, 1044058, 32767, false, false ); // Exceptional Chance:
				AddLabel( 430, 100, LabelHue, String.Format( "{0:F1}%", excepChance * 100 ) );
			}
		}

		private static Type typeofBlankScroll = typeof( BlankScroll );
		private static Type typeofSpellScroll = typeof( SpellScroll );

		public void DrawResource()
		{
			bool retainedColor = false;

			CraftContext context = m_CraftSystem.GetContext( m_From );

			CraftSubResCol res = m_CraftSystem.CraftSubRes;
			int resIndex = -1;

			if ( context != null )
				resIndex = context.LastResourceIndex;

			bool cropScroll = ( m_CraftItem.Resources.Count > 1 )
				&& m_CraftItem.Resources.GetAt( m_CraftItem.Resources.Count - 1 ).ItemType == typeofBlankScroll
				&& typeofSpellScroll.IsAssignableFrom( m_CraftItem.ItemType );

			for ( int i = 0; i < m_CraftItem.Resources.Count - (cropScroll ? 1 : 0) && i < 4; i++ )
			{
				Type type;
				string nameString;
				int nameNumber;

				CraftRes craftResource = m_CraftItem.Resources.GetAt( i );

				type = craftResource.ItemType;
				nameString = craftResource.NameString;
				nameNumber = craftResource.NameNumber;
				
				// Resource Mutation
				if ( type == res.ResType && resIndex > -1 )
				{
					CraftSubRes subResource = res.GetAt( resIndex );

					type = subResource.ItemType;

					nameString = subResource.NameString;
					nameNumber = subResource.GenericNameNumber;

					if ( nameNumber <= 0 )
						nameNumber = subResource.NameNumber;
				}
				// ******************

				if ( !retainedColor && m_CraftItem.RetainsColorFrom( m_CraftSystem, type ) )
				{
					retainedColor = true;
					AddHtmlLocalized( 170, 302 + (m_OtherCount++ * 20), 310, 18, 1044152, LabelColor, false, false ); // * The item retains the color of this material
					AddLabel( 500, 219 + (i * 20), LabelHue, "*" );
				}

				if ( nameNumber > 0 )
					AddHtmlLocalized( 170, 219 + (i * 20), 310, 18, nameNumber, LabelColor, false, false );
				else
					AddLabel( 170, 219 + (i * 20), LabelHue, nameString );

				AddLabel( 430, 219 + (i * 20), LabelHue, craftResource.Amount.ToString() );
			}

			if ( cropScroll )
				AddHtmlLocalized( 170, 302 + (m_OtherCount++ * 20), 360, 18, 1044379, LabelColor, false, false ); // Inscribing scrolls also requires a blank scroll and mana.
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			// Back Button
			if ( info.ButtonID == 0 )
			{
				CraftGump craftGump = new CraftGump( m_From, m_CraftSystem, m_Tool, null );
				m_From.SendGump( craftGump );
			}
			else // Make Button
			{
				if ( info.ButtonID > 3000 && CraftSystem.AllowManyCraft( m_Tool ) )
				{
					CraftAmountPrompt.Begin( m_From, m_CraftSystem, m_Tool, m_CraftItem, CraftAmountSource.CraftGumpItem );
					return;
				}

				int toMake;

				if ( info.ButtonID > 2000 && CraftSystem.AllowManyCraft( m_Tool ) )
					toMake = 100;
				else if ( info.ButtonID > 1000 && CraftSystem.AllowManyCraft( m_Tool ) )
					toMake = 10;
				else
					toMake = 1;

				CraftGump.DoCraft( m_From, m_CraftSystem, m_Tool, m_CraftItem, toMake );
			}
		}
	}
}