using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Engines.MLQuests.Objectives;
using Server.Engines.MLQuests.Rewards;

namespace Server.Engines.MLQuests.Gumps
{
	public enum ButtonPosition : byte
	{
		Left,
		Right
	}

	public enum ButtonGraphic : ushort
	{
		Invalid,
		Accept		= 0x2EE0,
		Clear		= 0x2EE3,
		Close		= 0x2EE6,
		Continue	= 0x2EE9,
		Okay		= 0x2EEC,
		Previous	= 0x2EEF,
		Refuse		= 0x2EF2,
		Resign		= 0x2EF5
	}

	public abstract class BaseQuestGump : Gump
	{
		public const int COLOR_HTML = 0xf7fbde; // RGB888
		public const int COLOR_LABEL = 1918; // Hue from files
		public const int COLOR_LOCALIZED = 0xf7db; // RGB565
		public const int COLOR_TITLE_LOCALIZED = 0xffa0; // RGB565

		private struct ButtonInfo
		{
			private ButtonPosition m_Position;
			private ButtonGraphic m_Graphic;
			private int m_ButtonID;

			public ButtonPosition Position { get { return m_Position; } }
			public ButtonGraphic Graphic { get { return m_Graphic; } }
			public int ButtonID { get { return m_ButtonID; } }

			public ButtonInfo( ButtonPosition position, ButtonGraphic graphic, int buttonID )
			{
				m_Position = position;
				m_Graphic = graphic;
				m_ButtonID = buttonID;
			}
		}

		private int m_Page;
		private int m_MaxPages;
		private int m_Label;
		private string m_Title;
		private List<ButtonInfo> m_Buttons;
		// RunUO optimized version
		public BaseQuestGump( int label )
			: base( 75, 25 )
		{
			m_Page = 0;
			m_MaxPages = 0;
			m_Label = label;
			m_Title = null;
			m_Buttons = new List<ButtonInfo>( 2 );

			Closable = false;

			AddPage( 0 );
			
			AddImage( 50, 20, 0x1452 );
			AddImage( 90, 33, 0x232D ); // Medallion top left
			AddHtmlLocalized( 130, 45, 270, 16, label, 0xFFFFFF, false, false );
			AddImageTiled( 130, 65, 175, 1, 0x238D ); // Tiny Underline
		}

		public void BuildPage()
		{
			AddPage( ++m_Page );

			if ( m_Page > 1 )
				AddButton( 130, 400, (int)ButtonGraphic.Previous, (int)ButtonGraphic.Previous + 2, 0, GumpButtonType.Page, m_Page - 1 );

			if ( m_Page < m_MaxPages )
				AddButton( 325, 400, (int)ButtonGraphic.Continue, (int)ButtonGraphic.Continue + 2, 0, GumpButtonType.Page, m_Page + 1 );

			foreach ( ButtonInfo button in m_Buttons )
				AddButton( button.Position == ButtonPosition.Left ? 95 : 363, 425, (int)button.Graphic, (int)button.Graphic + 2, button.ButtonID, GumpButtonType.Reply, 0 );

			if ( m_Title != null )
				AddHtmlLocalized( 130, 68, 220, 48, 1114513, m_Title, COLOR_TITLE_LOCALIZED, false, false ); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
		}

		public void SetPageCount( int maxPages )
		{
			m_MaxPages = maxPages;
		}

		public void SetTitle( TextDefinition def )
		{
			if ( def.Number > 0 )
				m_Title = String.Format( "#{0}", def.Number ); // OSI does "@@#{0}" instead, why? KR client related?
			else
				m_Title = def.String;
		}

		public void RegisterButton( ButtonPosition position, ButtonGraphic graphic, int buttonID )
		{
			m_Buttons.Add( new ButtonInfo( position, graphic, buttonID ) );
		}

		#region Elaborate Formatting Shortcuts

		public void AddDescription( MLQuest quest )
		{
			AddHtmlLocalized( 98, 110, 312, 16, ( quest.IsChainTriggered || quest.NextQuest != null ) ? 1075024 : 1072202, COLOR_TITLE_LOCALIZED, false, false ); // Description [(quest chain)]
			TextDefinition.AddHtmlText( this, 98, 126, 342, 260, quest.Description, false, true, COLOR_HTML, COLOR_HTML );
		}

		public void AddObjectives( MLQuest quest )
		{
			AddHtmlLocalized( 98, 140, 312, 16, 1049073, COLOR_TITLE_LOCALIZED, false, false ); // Objective:
            AddLabel(98, 156, COLOR_LABEL, ( quest.ObjectiveType == ObjectiveType.All ) ? "All of the following" : "Only one of the following");

			int y = 172;

			foreach ( BaseObjective objective in quest.Objectives )
			{
				objective.WriteToGump( this, ref y );

				if ( objective.IsTimed )
				{
					if ( objective is CollectObjective )
						y -= 16;

					BaseObjectiveInstance.WriteTimeRemaining( this, ref y, objective.Duration );
				}
			}
		}

		public void AddObjectivesProgress( MLQuestInstance instance )
		{
			MLQuest quest = instance.Quest;

			AddHtmlLocalized( 98, 140, 312, 16, 1049073, COLOR_TITLE_LOCALIZED, false, false ); // Objective:
            AddLabel(98, 156, COLOR_LABEL, ( quest.ObjectiveType == ObjectiveType.All ) ? "All of the following" : "Only one of the following");

			int y = 172;

			foreach ( BaseObjectiveInstance objInstance in instance.Objectives )
				objInstance.WriteToGump( this, ref y );
		}

		public void AddRewardsPage( MLQuest quest ) // For the quest log/offer gumps
		{
			AddHtmlLocalized( 98, 140, 312, 16, 1072201, COLOR_TITLE_LOCALIZED, false, false ); // Reward

			int y = 162;

			if ( quest.Rewards.Count > 1 )
			{
				// TODO: Is this what this is for? Does "Only one of the following" occur?
				AddHtmlLocalized( 98, 156, 312, 16, 1072208, COLOR_LOCALIZED, false, false ); // All of the following
				y += 16;
			}

			AddRewards( quest, 105, y, 16 );
		}

		public void AddRewards( MLQuest quest ) // For the claim rewards gump
		{
			int y = 146;

			if ( quest.Rewards.Count > 1 )
			{
				// TODO: Is this what this is for? Does "Only one of the following" occur?
				AddHtmlLocalized( 100, 140, 312, 16, 1072208, COLOR_LOCALIZED, false, false ); // All of the following
				y += 16;
			}

			AddRewards( quest, 107, y, 26 );
		}

		public void AddRewards( MLQuest quest, int x, int y, int spacing )
		{
			int xReward = x + 28;

			foreach ( BaseReward reward in quest.Rewards )
			{
				AddImage( x, y + 1, 0x4B9 );
				reward.WriteToGump( this, xReward, ref y );
				y += spacing;
			}
		}

		public void AddConversation( TextDefinition text )
		{
			TextDefinition.AddHtmlText( this, 98, 110, 342, 210, text, false, true, COLOR_HTML, COLOR_HTML );
		}

		#endregion

		/* OSI gump IDs:
		 * 800 - QuestOfferGump
		 * 801 - QuestCancelConfirmGump
		 * 802 - ?? (gets closed by Toggle Quest Item)
		 * 803 - QuestRewardGump
		 * 804 - ?? (gets closed by Toggle Quest Item)
		 * 805 - QuestLogGump
		 * 806 - QuestConversationGump (refuse / in progress)
		 * 807 - ?? (gets closed by Toggle Quest Item and most quest gumps)
		 * 808 - InfoNPCGump
		 * 809 - QuestLogDetailedGump
		 * 810 - QuestReportBackGump
		 */
		public static void CloseOtherGumps( PlayerMobile pm )
		{
			pm.CloseGump( typeof( InfoNPCGump ) );
			pm.CloseGump( typeof( QuestRewardGump ) );
			pm.CloseGump( typeof( QuestConversationGump ) );
			pm.CloseGump( typeof( QuestReportBackGump ) );
			//pm.CloseGump( typeof( UnknownGump807 ) );
			pm.CloseGump( typeof( QuestCancelConfirmGump ) );
		}
	}
}
