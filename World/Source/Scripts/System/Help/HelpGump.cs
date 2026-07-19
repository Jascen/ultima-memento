using System;
using Server.Gumps;
using Server.Network;
using Server.Menus.Questions;
using Server.Multis;
using Server.Mobiles;
using Server.Regions;
using System.Collections;
using Server.Commands;
using Server.Misc;
using Server.Items;
using Server.Engines.MLQuests.Gumps;
using Scripts.Mythik.Systems.Achievements;
using Server.SpellBars;
using Server.ModernSkill;
using System.Linq;
using System.Text;

namespace Server.Engines.Help
{
	public class ContainedMenu : QuestionMenu
	{
		private Mobile m_From;

		public ContainedMenu( Mobile from ) : base( "You already have an open help request. We will have someone assist you as soon as possible.  What would you like to do?", new string[]{ "Leave my old help request like it is.", "Remove my help request from the queue." } )
		{
			m_From = from;
		}

		public override void OnCancel( NetState state )
		{
			m_From.SendLocalizedMessage( 1005306, "", 0x35 ); // Help request unchanged.
		}

		public override void OnResponse( NetState state, int index )
		{
			m_From.SendSound( 0x4A );
			if ( index == 0 )
			{
				m_From.SendLocalizedMessage( 1005306, "", 0x35 ); // Help request unchanged.
			}
			else if ( index == 1 )
			{
				PageEntry entry = PageQueue.GetEntry( m_From );

				if ( entry != null && entry.Handler == null )
				{
					m_From.SendLocalizedMessage( 1005307, "", 0x35 ); // Removed help request.
					entry.AddResponse( entry.Sender, "[Canceled]" );
					PageQueue.Remove( entry );
				}
				else
				{
					m_From.SendLocalizedMessage( 1005306, "", 0x35 ); // Help request unchanged.
				}
			}
		}
	}

	public class HelpGump : Gump
	{
		private const string TEXT_COLOR = "#ddbc4b"; // Yellowy
		private readonly int m_PageNumber;

		private enum PageActionType
		{
			None = 0,
			Close = 0,

			Do_Achievements,
			Do_CorpseClear,
			Do_CorpseSearch,
			Do_MoongateSearch,
			Do_StuckInWorld,
			Do_Toggle_AFK,

			MagicToolbar_Ancient_I_Close,
			MagicToolbar_Ancient_I_Config,
			MagicToolbar_Ancient_I_Open,
			MagicToolbar_Ancient_I_ToggleAutoOpen,
			MagicToolbar_Ancient_II_Close,
			MagicToolbar_Ancient_II_Config,
			MagicToolbar_Ancient_II_Open,
			MagicToolbar_Ancient_II_ToggleAutoOpen,
			MagicToolbar_Ancient_III_Close,
			MagicToolbar_Ancient_III_Config,
			MagicToolbar_Ancient_III_Open,
			MagicToolbar_Ancient_III_ToggleAutoOpen,
			MagicToolbar_Ancient_IV_Close,
			MagicToolbar_Ancient_IV_Config,
			MagicToolbar_Ancient_IV_Open,
			MagicToolbar_Ancient_IV_ToggleAutoOpen,
			MagicToolbar_Bard_I_Close,
			MagicToolbar_Bard_I_Config,
			MagicToolbar_Bard_I_Open,
			MagicToolbar_Bard_I_ToggleAutoOpen,
			MagicToolbar_Bard_II_Close,
			MagicToolbar_Bard_II_Config,
			MagicToolbar_Bard_II_Open,
			MagicToolbar_Bard_II_ToggleAutoOpen,
			MagicToolbar_DeathKnight_I_Close,
			MagicToolbar_DeathKnight_I_Config,
			MagicToolbar_DeathKnight_I_Open,
			MagicToolbar_DeathKnight_I_ToggleAutoOpen,
			MagicToolbar_DeathKnight_II_Close,
			MagicToolbar_DeathKnight_II_Config,
			MagicToolbar_DeathKnight_II_Open,
			MagicToolbar_DeathKnight_II_ToggleAutoOpen,
			MagicToolbar_Elemental_I_Close,
			MagicToolbar_Elemental_I_Config,
			MagicToolbar_Elemental_I_Open,
			MagicToolbar_Elemental_I_ToggleAutoOpen,
			MagicToolbar_Elemental_II_Close,
			MagicToolbar_Elemental_II_Config,
			MagicToolbar_Elemental_II_Open,
			MagicToolbar_Elemental_II_ToggleAutoOpen,
			MagicToolbar_Knight_I_Close,
			MagicToolbar_Knight_I_Config,
			MagicToolbar_Knight_I_Open,
			MagicToolbar_Knight_I_ToggleAutoOpen,
			MagicToolbar_Knight_II_Close,
			MagicToolbar_Knight_II_Config,
			MagicToolbar_Knight_II_Open,
			MagicToolbar_Knight_II_ToggleAutoOpen,
			MagicToolbar_Magery_I_Close,
			MagicToolbar_Magery_I_Config,
			MagicToolbar_Magery_I_Open,
			MagicToolbar_Magery_I_ToggleAutoOpen,
			MagicToolbar_Magery_II_Close,
			MagicToolbar_Magery_II_Config,
			MagicToolbar_Magery_II_Open,
			MagicToolbar_Magery_II_ToggleAutoOpen,
			MagicToolbar_Magery_III_Close,
			MagicToolbar_Magery_III_Config,
			MagicToolbar_Magery_III_Open,
			MagicToolbar_Magery_III_ToggleAutoOpen,
			MagicToolbar_Magery_IV_Close,
			MagicToolbar_Magery_IV_Config,
			MagicToolbar_Magery_IV_Open,
			MagicToolbar_Magery_IV_ToggleAutoOpen,
			MagicToolbar_Monk_I_Close,
			MagicToolbar_Monk_I_Config,
			MagicToolbar_Monk_I_Open,
			MagicToolbar_Monk_I_ToggleAutoOpen,
			MagicToolbar_Monk_II_Close,
			MagicToolbar_Monk_II_Config,
			MagicToolbar_Monk_II_Open,
			MagicToolbar_Monk_II_ToggleAutoOpen,
			MagicToolbar_Necromancer_I_Close,
			MagicToolbar_Necromancer_I_Config,
			MagicToolbar_Necromancer_I_Open,
			MagicToolbar_Necromancer_I_ToggleAutoOpen,
			MagicToolbar_Necromancer_II_Close,
			MagicToolbar_Necromancer_II_Config,
			MagicToolbar_Necromancer_II_Open,
			MagicToolbar_Necromancer_II_ToggleAutoOpen,
			MagicToolbar_Priest_I_Close,
			MagicToolbar_Priest_I_Config,
			MagicToolbar_Priest_I_Open,
			MagicToolbar_Priest_I_ToggleAutoOpen,
			MagicToolbar_Priest_II_Close,
			MagicToolbar_Priest_II_Config,
			MagicToolbar_Priest_II_Open,
			MagicToolbar_Priest_II_ToggleAutoOpen,

			Navigate_Actions,
			Navigate_Bars,
			Navigate_Gumps,
			Navigate_Changelog,
			Navigate_Library,
			Navigate_MagicToolbars,
			Navigate_Main,

			Setting_AutoAttack_Info,
			Setting_AutoAttack,
			Setting_AutoSheath_Info,
			Setting_AutoSheath,
			Setting_ClassicPoisoning_Info,
			Setting_ClassicPoisoning,
			Setting_ColorlessFabricBreakdown_Info,
			Setting_ColorlessFabricBreakdown,
			Setting_CreatureMagicFocus_Info,
			Setting_CreatureMagicFocus,
			Setting_CreatureSounds_Info,
			Setting_CreatureSounds,
			Setting_CreatureType_Info,
			Setting_CreatureType,
			Setting_CustomTitle_Info,
			Setting_CustomTitle,
			Setting_DoubleClickToIDItems_Info,
			Setting_DoubleClickToIDItems,
			Setting_DoubleClickToTalk_Info,
			Setting_DoubleClickToTalk,
			Setting_GumpImages_Info,
			Setting_GumpImages,
			Setting_LegacyCarve_Info,
			Setting_LegacyCarve,
			Setting_LootOptions_Info,
			Setting_LootOptions,
			Setting_MagerySpellColor_Black,
			Setting_MagerySpellColor_Blue,
			Setting_MagerySpellColor_Default,
			Setting_MagerySpellColor_Green,
			Setting_MagerySpellColor_Info,
			Setting_MagerySpellColor_Purple,
			Setting_MagerySpellColor_Red,
			Setting_MagerySpellColor_White,
			Setting_MagerySpellColor_Yellow,
			Setting_MessageColors_Info,
			Setting_MessageColors,
			Setting_ModernSkills_Info,
			Setting_ModernSkills,
			Setting_MusicPlaylist_Info,
			Setting_MusicPlaylist,
			Setting_MusicTone_Info,
			Setting_MusicTone,
			Setting_OrdinaryResources_Info,
			Setting_OrdinaryResources,
			Setting_Playstyle_Barbaric_Info,
			Setting_Playstyle_Barbaric,
			Setting_Playstyle_Evil_Info,
			Setting_Playstyle_Evil,
			Setting_Playstyle_Normal_Info,
			Setting_Playstyle_Normal,
			Setting_Playstyle_Oriental_Info,
			Setting_Playstyle_Oriental,
			Setting_PrivatePlay_Info,
			Setting_PrivatePlay,
			Setting_RemoveVendorGoldSafeguard_Info,
			Setting_RemoveVendorGoldSafeguard,
			Setting_SetCraftingContainer_Info,
			Setting_SetCraftingContainer,
			Setting_SetHarvestingContainer_Info,
			Setting_SetHarvestingContainer,
			Setting_SetLootContainer_Info,
			Setting_SetLootContainer,
			Setting_SingleAttemptID_Info,
			Setting_SingleAttemptID,
			Setting_SkillList_Info,
			Setting_SkillList,
			Setting_SkillTitle_Info,
			Setting_SkillTitle,
			Setting_SuppressVendorTooltips_Info,
			Setting_SuppressVendorTooltips,
			Setting_UseAncientSpellbook_Info,
			Setting_UseAncientSpellbook,
			Setting_VendorContainerSell_Info,
			Setting_VendorContainerSell,
			Setting_WeaponAbilityBar_Info,
			Setting_WeaponAbilityBar,
			Setting_WeaponAbilityNames_Info,
			Setting_WeaponAbilityNames,

			Show_Chat,
			Show_Conversations,
			Show_Emote,
			Show_MOTD,
			Show_Quests,
			Show_QuickBar,
			Show_QuickBarManage,
			Do_QuickBarAutoOpen,
			Show_RegBar,
			Show_CombatBar,
			Show_Settings,
			Show_SkillList,
			Show_Statistics,
			Show_WealthBar,
			Show_WeaponAbilities,
			ShowHelp_MagicToolbars,

			Changelog_PageBase = 9000,
		}

		public static void Initialize()
		{
			EventSink.HelpRequest += new HelpRequestEventHandler( EventSink_HelpRequest );
            CommandSystem.Register("toolbars", AccessLevel.Player, e => OpenHelpGumpPageCommand(e, (int)PageActionType.Navigate_MagicToolbars));
            CommandSystem.Register("help", AccessLevel.Player, e => OpenHelpGumpPageCommand(e, (int)PageActionType.Navigate_Main));
		}

        private static void OpenHelpGumpPageCommand(CommandEventArgs e, int pageNumber)
        {
			var player = e.Mobile as PlayerMobile;
			if (player == null) return;

			player.CloseGump(typeof(HelpGump));
			player.SendGump(new HelpGump(player, pageNumber));
        }

        private static void EventSink_HelpRequest( HelpRequestEventArgs e )
		{
			foreach ( Gump g in e.Mobile.NetState.Gumps )
			{
				if ( g is HelpGump )
					return;
			}

			if ( !PageQueue.CheckAllowedToPage( e.Mobile ) )
				return;

			if ( PageQueue.Contains( e.Mobile ) )
				e.Mobile.SendMenu( new ContainedMenu( e.Mobile ) );
			else
				e.Mobile.SendGump( new HelpGump( e.Mobile, (int)PageActionType.Navigate_Main ) );
		}

		private static bool IsYoung( Mobile m )
		{
			if ( m is PlayerMobile )
				return ((PlayerMobile)m).Young;

			return false;
		}

		public static bool CheckCombat( Mobile m )
		{
			for ( int i = 0; i < m.Aggressed.Count; ++i )
			{
				AggressorInfo info = m.Aggressed[i];

				if ( DateTime.Now - info.LastCombatTime < TimeSpan.FromSeconds( 30.0 ) )
					return true;
			}

			return false;
		}

		public HelpGump( Mobile mobile, int page, int secondaryPageNumber = 0 ) : base( 50, 50 )
		{
			if ( false == ( mobile is PlayerMobile ) ) return;

			m_PageNumber = page;
			var from = (PlayerMobile)mobile;
			string HelpText = MyHelp();
			string color = TEXT_COLOR;

			from.SendSound( 0x4A ); 

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			int r = 40;
			int e = 30;

			AddImage(0, 0, 9548, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddHtml( 12, 12, 300, 20, @"<BODY><BASEFONT Color=" + color + ">HELP OPTIONS</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(967, 10, 4017, 4017, (int)PageActionType.Close, GumpButtonType.Reply, 0);

			const int NAVIGATION_START_X = 15;
			const int NAVIGATION_ITEM_WIDTH = 150;

			int nav_x = NAVIGATION_START_X;
			
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			AddAction(nav_x, r, from, "Main", PageActionType.Navigate_Main, NAVIGATION_ITEM_WIDTH);
			r += e;
			if ( page == (int)PageActionType.Navigate_Main ){ AddHtml( 252, 71, 739, 630, @"<BODY><BASEFONT Color=" + color + ">" + HelpText + "</BASEFONT></BODY>", (bool)false, (bool)true); }

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			AddAction(nav_x, r, from, "Actions", PageActionType.Navigate_Actions, NAVIGATION_ITEM_WIDTH);
			r += e;
			if ( page == (int)PageActionType.Navigate_Actions )
			{
				const int SECTION_START_X = 245;
				const int BAR_BORDER_HEIGHT = 4;

				int rowY = 40;
				int rowHeight = 40;
				AddGeneralActionRowHeader(SECTION_START_X, rowY, "Common Actions");

				rowY += rowHeight;
				rowY -= BAR_BORDER_HEIGHT;
				AddGeneralActionRow(SECTION_START_X, rowY, "Clear My Corpses", PageActionType.Do_CorpseClear);

				rowY += rowHeight;
				rowY -= BAR_BORDER_HEIGHT;
				AddGeneralActionRow(SECTION_START_X, rowY, "Find My Corpse", PageActionType.Do_CorpseSearch);

				rowY += rowHeight;
				rowY -= BAR_BORDER_HEIGHT;
				AddGeneralActionRow(SECTION_START_X, rowY, "Find Nearby Moongates", PageActionType.Do_MoongateSearch);

				rowY += rowHeight;
				rowY -= BAR_BORDER_HEIGHT;
				AddGeneralActionRow(SECTION_START_X, rowY, "Toggle AFK Mode", PageActionType.Do_Toggle_AFK);
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			AddAction(nav_x, r, from, "Bars", PageActionType.Navigate_Bars, NAVIGATION_ITEM_WIDTH);
			r += e;

			if ( page == (int)PageActionType.Navigate_Bars )
			{
				const int SECTION_START_X = 245;

				int rowY = 40;
				int rowHeight = 40;
				AddMagicToolbarRowHeader(SECTION_START_X, rowY);

				rowY += rowHeight;
				AddToolbarRowOnlyOpen(SECTION_START_X, rowY, "Combat Bar", PageActionType.Show_CombatBar);

				rowY += rowHeight;
				AddToolbarRowOnlyOpen(SECTION_START_X, rowY, "Reagent Bar", PageActionType.Show_RegBar);

				rowY += rowHeight;
				var isAutoOpen = PlayerSettings.GetQuickConfig( from, QuickConfig.KEY_AUTO_OPEN );
				AddMagicToolbarRow(SECTION_START_X, rowY, "Quick Bar", PageActionType.Show_QuickBarManage, PageActionType.Show_QuickBar, PageActionType.None, PageActionType.Do_QuickBarAutoOpen, isAutoOpen);

				rowY += rowHeight;
				AddToolbarRowOnlyOpen(SECTION_START_X, rowY, "Weapon Abilities Bar", PageActionType.Show_WeaponAbilities);
				
				rowY += rowHeight;
				AddToolbarRowOnlyOpen(SECTION_START_X, rowY, "Wealth Bar", PageActionType.Show_WealthBar);
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			AddAction(nav_x, r, from, "Gumps", PageActionType.Navigate_Gumps, NAVIGATION_ITEM_WIDTH);
			r += e;

			if ( page == (int)PageActionType.Navigate_Gumps )
			{
				const int SECTION_START_X = 245;
				const int BAR_BORDER_HEIGHT = 4;

				int rowY = 40;
				int rowHeight = 40;
				AddGeneralActionRowHeader(SECTION_START_X, rowY, "Common Gumps");

				rowY += rowHeight;
				AddGeneralActionRow(SECTION_START_X, rowY, "Achievements", PageActionType.Do_Achievements);

				rowY += rowHeight;
				rowY -= BAR_BORDER_HEIGHT;
				AddGeneralActionRow(SECTION_START_X, rowY, "Chat", PageActionType.Show_Chat);

				rowY += rowHeight;
				rowY -= BAR_BORDER_HEIGHT;
				AddGeneralActionRow(SECTION_START_X, rowY, "Emote List", PageActionType.Show_Emote);

				rowY += rowHeight;
				rowY -= BAR_BORDER_HEIGHT;
				AddGeneralActionRow(SECTION_START_X, rowY, "Library", PageActionType.Navigate_Library);

				rowY += rowHeight;
				rowY -= BAR_BORDER_HEIGHT;
				AddGeneralActionRow(SECTION_START_X, rowY, "Message of the Day", PageActionType.Show_MOTD);

				rowY += rowHeight;
				rowY -= BAR_BORDER_HEIGHT;
				AddGeneralActionRow(SECTION_START_X, rowY, "NPC Conversations", PageActionType.Show_Conversations);

				rowY += rowHeight;
				rowY -= BAR_BORDER_HEIGHT;
				AddGeneralActionRow(SECTION_START_X, rowY, "Skill List", PageActionType.Show_SkillList);
				
				rowY += rowHeight;
				rowY -= BAR_BORDER_HEIGHT;
				AddGeneralActionRow(SECTION_START_X, rowY, "Statistics", PageActionType.Show_Statistics);
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			AddAction(nav_x, r, from, "Magic Toolbars", PageActionType.Navigate_MagicToolbars, NAVIGATION_ITEM_WIDTH);
			r += e;
			if ( page == (int)PageActionType.Navigate_MagicToolbars )
			{
				var hasBarding = 0 < from.Skills[SkillName.Musicianship].Value;
				var hasElementalism = 0 < from.Skills[SkillName.Elementalism].Value;
				var hasHolyMan = Spellbook.FindHolyMan(from) != null;
				var hasKnightship = 0 < from.Skills[SkillName.Knightship].Value;
				var hasNegativeKarma = from.Karma < 0;
				var hasMagery = 0 < from.Skills[SkillName.Magery].Value;
				var hasMonk = Server.Misc.GetPlayerInfo.isMonk(from);
				var hasNecromancy = 0 < from.Skills[SkillName.Necromancy].Value;
				var hasResearch = Server.Misc.ResearchSettings.ResearchMaterials(from) != null || ResearchSettings.BookCaster( from );

				const int SECTION_START_X = 245;
				const int BAR_BORDER_HEIGHT = 4;

				int barS = 40;
				int barM = 30;

				AddButton(904, 10, 3610, 3610, (int)PageActionType.ShowHelp_MagicToolbars, GumpButtonType.Reply, 0);

				AddMagicToolbarRowHeader(SECTION_START_X, barS);
				barS += barM;
				var autoOpenSpellBarIds = from.SpellBars.GetAllAutoOpenSpellBarIds().ToHashSet();

				if (hasResearch)
				{
					AddMagicToolbarRow(SECTION_START_X, barS, "Ancient Spell Bar", PageActionType.MagicToolbar_Ancient_I_Config, PageActionType.MagicToolbar_Ancient_I_Open, PageActionType.MagicToolbar_Ancient_I_Close, PageActionType.MagicToolbar_Ancient_I_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Ancient_1));
					barS += barM;

					AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_Ancient_II_Config, PageActionType.MagicToolbar_Ancient_II_Open, PageActionType.MagicToolbar_Ancient_II_Close, PageActionType.MagicToolbar_Ancient_II_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Ancient_2));
					barS += barM;

					AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_Ancient_III_Config, PageActionType.MagicToolbar_Ancient_III_Open, PageActionType.MagicToolbar_Ancient_III_Close, PageActionType.MagicToolbar_Ancient_III_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Ancient_3));
					barS += barM;

					AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_Ancient_IV_Config, PageActionType.MagicToolbar_Ancient_IV_Open, PageActionType.MagicToolbar_Ancient_IV_Close, PageActionType.MagicToolbar_Ancient_IV_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Ancient_4));
					barS += barM;
				}

				if (hasBarding)
				{
					barS -= BAR_BORDER_HEIGHT;
					AddMagicToolbarRow(SECTION_START_X, barS, "Bard Songs Bar", PageActionType.MagicToolbar_Bard_I_Config, PageActionType.MagicToolbar_Bard_I_Open, PageActionType.MagicToolbar_Bard_I_Close, PageActionType.MagicToolbar_Bard_I_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Bard_1), true);
					barS += barM;

					AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_Bard_II_Config, PageActionType.MagicToolbar_Bard_II_Open, PageActionType.MagicToolbar_Bard_II_Close, PageActionType.MagicToolbar_Bard_II_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Bard_2));
					barS += barM;
				}

				if (hasKnightship)
				{
					barS -= BAR_BORDER_HEIGHT;
					AddMagicToolbarRow(SECTION_START_X, barS, "Knight Spell Bar", PageActionType.MagicToolbar_Knight_I_Config, PageActionType.MagicToolbar_Knight_I_Open, PageActionType.MagicToolbar_Knight_I_Close, PageActionType.MagicToolbar_Knight_I_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Knight_1), true);
					barS += barM;

					AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_Knight_II_Config, PageActionType.MagicToolbar_Knight_II_Open, PageActionType.MagicToolbar_Knight_II_Close, PageActionType.MagicToolbar_Knight_II_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Knight_2));
					barS += barM;

					if (hasNegativeKarma)
					{
						barS -= BAR_BORDER_HEIGHT;
						AddMagicToolbarRow(SECTION_START_X, barS, "Death Knight Spell Bar", PageActionType.MagicToolbar_DeathKnight_I_Config, PageActionType.MagicToolbar_DeathKnight_I_Open, PageActionType.MagicToolbar_DeathKnight_I_Close, PageActionType.MagicToolbar_DeathKnight_I_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Death_1), true);
						barS += barM;

						AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_DeathKnight_II_Config, PageActionType.MagicToolbar_DeathKnight_II_Open, PageActionType.MagicToolbar_DeathKnight_II_Close, PageActionType.MagicToolbar_DeathKnight_II_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Death_2));
						barS += barM;
					}
				}

				if (hasElementalism)
				{
					barS -= BAR_BORDER_HEIGHT;
					AddMagicToolbarRow(SECTION_START_X, barS, "Elemental Spell Bar", PageActionType.MagicToolbar_Elemental_I_Config, PageActionType.MagicToolbar_Elemental_I_Open, PageActionType.MagicToolbar_Elemental_I_Close, PageActionType.MagicToolbar_Elemental_I_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Elemental_1), true);
					barS += barM;

					AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_Elemental_II_Config, PageActionType.MagicToolbar_Elemental_II_Open, PageActionType.MagicToolbar_Elemental_II_Close, PageActionType.MagicToolbar_Elemental_II_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Elemental_2));
					barS += barM;
				}

				if (hasMagery)
				{
					barS -= BAR_BORDER_HEIGHT;
					AddMagicToolbarRow(SECTION_START_X, barS, "Magery Spell Bar", PageActionType.MagicToolbar_Magery_I_Config, PageActionType.MagicToolbar_Magery_I_Open, PageActionType.MagicToolbar_Magery_I_Close, PageActionType.MagicToolbar_Magery_I_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Mage_1), true);
					barS += barM;

					AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_Magery_II_Config, PageActionType.MagicToolbar_Magery_II_Open, PageActionType.MagicToolbar_Magery_II_Close, PageActionType.MagicToolbar_Magery_II_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Mage_2));
					barS += barM;

					AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_Magery_III_Config, PageActionType.MagicToolbar_Magery_III_Open, PageActionType.MagicToolbar_Magery_III_Close, PageActionType.MagicToolbar_Magery_III_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Mage_3));
					barS += barM;

					AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_Magery_IV_Config, PageActionType.MagicToolbar_Magery_IV_Open, PageActionType.MagicToolbar_Magery_IV_Close, PageActionType.MagicToolbar_Magery_IV_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Mage_4));
					barS += barM;
				}

				if (hasMonk)
				{
					barS -= BAR_BORDER_HEIGHT;
					AddMagicToolbarRow(SECTION_START_X, barS, "Monk Ability Bar", PageActionType.MagicToolbar_Monk_I_Config, PageActionType.MagicToolbar_Monk_I_Open, PageActionType.MagicToolbar_Monk_I_Close, PageActionType.MagicToolbar_Monk_I_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Monk_1), true);
					barS += barM;

					AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_Monk_II_Config, PageActionType.MagicToolbar_Monk_II_Open, PageActionType.MagicToolbar_Monk_II_Close, PageActionType.MagicToolbar_Monk_II_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Monk_2));
					barS += barM;
				}

				if (hasNecromancy)
				{
					barS -= BAR_BORDER_HEIGHT;
					AddMagicToolbarRow(SECTION_START_X, barS, "Necromancer Spell Bar", PageActionType.MagicToolbar_Necromancer_I_Config, PageActionType.MagicToolbar_Necromancer_I_Open, PageActionType.MagicToolbar_Necromancer_I_Close, PageActionType.MagicToolbar_Necromancer_I_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Necro_1), true);
					barS += barM;

					AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_Necromancer_II_Config, PageActionType.MagicToolbar_Necromancer_II_Open, PageActionType.MagicToolbar_Necromancer_II_Close, PageActionType.MagicToolbar_Necromancer_II_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Necro_2));
					barS += barM;
				}

				if (hasHolyMan)
				{
					barS -= BAR_BORDER_HEIGHT;
					AddMagicToolbarRow(SECTION_START_X, barS, "Priest Prayer Bar", PageActionType.MagicToolbar_Priest_I_Config, PageActionType.MagicToolbar_Priest_I_Open, PageActionType.MagicToolbar_Priest_I_Close, PageActionType.MagicToolbar_Priest_I_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Priest_1), true);
					barS += barM;

					AddMagicToolbarRow(SECTION_START_X, barS, null, PageActionType.MagicToolbar_Priest_II_Config, PageActionType.MagicToolbar_Priest_II_Open, PageActionType.MagicToolbar_Priest_II_Close, PageActionType.MagicToolbar_Priest_II_ToggleAutoOpen, autoOpenSpellBarIds.Contains(SpellBarId.Priest_2));
					barS += barM;
				}
			}


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			AddAction(nav_x, r, from, "Quests", PageActionType.Show_Quests, NAVIGATION_ITEM_WIDTH);
			r += e;
			if ( page == (int)PageActionType.Show_Quests ){ AddHtml( 252, 71, 739, 630, @"<BODY><BASEFONT Color=" + color + ">Throughout your journey, you may come across particular events that appear in your quest log. They may be a simple achievement of finding a strange land, or they may reference an item you must find. Quests are handled in a 'virtual' manner. What this means is that any achievements are real, but any references to items found are not. If your quest log states that you found an ebony key, you will not have an ebony key in your backpack...but you will 'virtually' have the item. The quest will keep track of this fact for you. Because of this, you will never lose that ebony key and it remains unique to your character's questing. The quest knows you found it and have it. You may be tasked to find an item in a dungeon. When there is an indication you found it, it will be 'virtually' in your possession. You will often hear a sound of victory when a quest event is reached, along with a message about it. You still may miss it, however. So check your quest log from time to time. One way to get quests is to visit taverns or inns. If you see a bulletin board called 'Seeking Brave Adventurers', single click on it to begin your life questing for fame and fortune.<BR><BR>" + MyQuests( from ) + "<BR><BR></BASEFONT></BODY>", (bool)false, (bool)true); }

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			AddAction(nav_x, r, from, "Settings", PageActionType.Show_Settings, NAVIGATION_ITEM_WIDTH);
			r += e;

			if ( page == (int)PageActionType.Show_Settings )
			{
				const int SECTION_START_X = 225;
				const int SETTING_START_X = SECTION_START_X + 20;
				const int SETTING_SECTION_WIDTH = 725;

				int g = 40;
				int j = 30;
				int xm = 245;
				int xo = 700;
				int xr = 0;
				int xs = SECTION_START_X;

				// Section - Settings
				AddHtml( SECTION_START_X, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Settings</BASEFONT></BODY>", (bool)false, (bool)false);
				g += j;

				xs = SETTING_START_X;
				AddSetting(xs, g, from, "Auto Attack", PageActionType.Setting_AutoAttack, PageActionType.Setting_AutoAttack_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Auto Sheath", PageActionType.Setting_AutoSheath, PageActionType.Setting_AutoSheath_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Classic Poisoning", PageActionType.Setting_ClassicPoisoning, PageActionType.Setting_ClassicPoisoning_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				if ( from.RaceID > 0 && Server.Items.BaseRace.GetMonsterMage( from.RaceID ) )
				{
					string magic = "Default";
					if ( from.RaceMagicSchool == 1 ){ magic = "Magery"; }
					else if ( from.RaceMagicSchool == 2 ){ magic = "Necromancy"; }
					else if ( from.RaceMagicSchool == 3 ){ magic = "Elementalism"; }

					var inTavern = from.Region.Name == "the Tavern";
					AddSetting(xs, g, from, "Creature Magic (" + magic + ")", PageActionType.Setting_CreatureMagicFocus, PageActionType.Setting_CreatureMagicFocus_Info, inTavern);
					if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }
				}

				if ( from.RaceID > 0 )
				{
					AddSetting(xs, g, from, "Creature Sounds", PageActionType.Setting_CreatureSounds, PageActionType.Setting_CreatureSounds_Info);
					if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }
				}

				if ( from.RaceID > 0 
					&& (
					(from.Region).Name == "the Tavern" ||
					( from.Map == Map.Sosaria && from.X >= 6982 && from.Y >= 694 && from.X <= 6999 && from.Y <= 713 )
				))
				{
					AddSetting(xs, g, from, "Creature Type", PageActionType.Setting_CreatureType, PageActionType.Setting_CreatureType_Info);
					if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }
				}

				if ( MySettings.S_AllowCustomTitles )
				{
					AddSetting(xs, g, from, "Custom Title", PageActionType.Setting_CustomTitle, PageActionType.Setting_CustomTitle_Info);
					if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }
				}

				AddSetting(xs, g, from, "Gump Images", PageActionType.Setting_GumpImages, PageActionType.Setting_GumpImages_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Loot Options", PageActionType.Setting_LootOptions, PageActionType.Setting_LootOptions_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Message Colors", PageActionType.Setting_MessageColors, PageActionType.Setting_MessageColors_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Music Playlist", PageActionType.Setting_MusicPlaylist, PageActionType.Setting_MusicPlaylist_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Music Tone", PageActionType.Setting_MusicTone, PageActionType.Setting_MusicTone_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Private Play", PageActionType.Setting_PrivatePlay, PageActionType.Setting_PrivatePlay_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Skill Title", PageActionType.Setting_SkillTitle, PageActionType.Setting_SkillTitle_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				string skillLocks = "Skill List (Show Up)"; 
				if ( from.SkillDisplay == 1 ){ skillLocks = "Skill List (Show Up and Locked)"; }
				AddSetting(xs, g, from, skillLocks, PageActionType.Setting_SkillList, PageActionType.Setting_SkillList_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Weapon Ability Bar", PageActionType.Setting_WeaponAbilityBar, PageActionType.Setting_WeaponAbilityBar_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Set Crafting Container", PageActionType.Setting_SetCraftingContainer, PageActionType.Setting_SetCraftingContainer_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Weapon Ability Names", PageActionType.Setting_WeaponAbilityNames, PageActionType.Setting_WeaponAbilityNames_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Set Harvesting Container", PageActionType.Setting_SetHarvestingContainer, PageActionType.Setting_SetHarvestingContainer_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Use Ancient Spellbook", PageActionType.Setting_UseAncientSpellbook, PageActionType.Setting_UseAncientSpellbook_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Set Loot Container", PageActionType.Setting_SetLootContainer, PageActionType.Setting_SetLootContainer_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Double Click to ID Items", PageActionType.Setting_DoubleClickToIDItems, PageActionType.Setting_DoubleClickToIDItems_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Double Click Talk", PageActionType.Setting_DoubleClickToTalk, PageActionType.Setting_DoubleClickToTalk_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Single ID Attempt", PageActionType.Setting_SingleAttemptID, PageActionType.Setting_SingleAttemptID_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Colorless Fabric Breakdown", PageActionType.Setting_ColorlessFabricBreakdown, PageActionType.Setting_ColorlessFabricBreakdown_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Ordinary Resources", PageActionType.Setting_OrdinaryResources, PageActionType.Setting_OrdinaryResources_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Remove Vendor Gold Safeguard", PageActionType.Setting_RemoveVendorGoldSafeguard, PageActionType.Setting_RemoveVendorGoldSafeguard_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Suppress Vendor Tooltips", PageActionType.Setting_SuppressVendorTooltips, PageActionType.Setting_SuppressVendorTooltips_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Container Sell", PageActionType.Setting_VendorContainerSell, PageActionType.Setting_VendorContainerSell_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Modern Skills", PageActionType.Setting_ModernSkills, PageActionType.Setting_ModernSkills_Info);
				if ( xr == 1 ){ g += j; xr=0; xs=xm; } else { xr=1; xs=xo; }

				AddSetting(xs, g, from, "Legacy Carve", PageActionType.Setting_LegacyCarve, PageActionType.Setting_LegacyCarve_Info);
				// Last setting, don't add a row

				// Section - Play Styles
				const int PLAYSTYLE_OPTIONS_PER_ROW = 4;
				const int PLAYSTYLE_OPTION_WIDTH = 125;
				const int PLAYSTYLE_OPTION_WIDTH_TOTAL = PLAYSTYLE_OPTION_WIDTH * PLAYSTYLE_OPTIONS_PER_ROW;
				const int PLAYSTYLE_PADDING_LEFT = (int)( (double)( SETTING_SECTION_WIDTH - PLAYSTYLE_OPTION_WIDTH_TOTAL ) / PLAYSTYLE_OPTIONS_PER_ROW );

				g += (int)(1.5 * j);
				xs = SECTION_START_X;
				AddHtml( xs, g, 316, 20, @"<BODY><BASEFONT Color=" + color + ">Play Styles</BASEFONT></BODY>", (bool)false, (bool)false);
				g += j;

				xs = SETTING_START_X;
				AddSetting(xs, g, from, "Normal", PageActionType.Setting_Playstyle_Normal, PageActionType.Setting_Playstyle_Normal_Info);
				xs += PLAYSTYLE_OPTION_WIDTH + PLAYSTYLE_PADDING_LEFT;

				AddSetting(xs, g, from, "Evil", PageActionType.Setting_Playstyle_Evil, PageActionType.Setting_Playstyle_Evil_Info);
				xs += PLAYSTYLE_OPTION_WIDTH + PLAYSTYLE_PADDING_LEFT;

				AddSetting(xs, g, from, "Oriental", PageActionType.Setting_Playstyle_Oriental, PageActionType.Setting_Playstyle_Oriental_Info);
				xs += PLAYSTYLE_OPTION_WIDTH + PLAYSTYLE_PADDING_LEFT;

				string barbaricStyle = !from.Female ? "Barbaric" : "Barbaric (Amazon)";
				AddSetting(xs, g, from, barbaricStyle, PageActionType.Setting_Playstyle_Barbaric, PageActionType.Setting_Playstyle_Barbaric_Info);

				// Section - Magery Spell Color
				const int MAGERY_SPELL_COLOR_OPTIONS_PER_ROW = 4;
				const int MAGERY_SPELL_COLOR_OPTION_WIDTH = 90;
				const int MAGERY_SPELL_COLOR_OPTION_WIDTH_TOTAL = MAGERY_SPELL_COLOR_OPTION_WIDTH * MAGERY_SPELL_COLOR_OPTIONS_PER_ROW;
				const int MAGERY_SPELL_COLOR_PADDING_LEFT = (int)( (double)( SETTING_SECTION_WIDTH - MAGERY_SPELL_COLOR_OPTION_WIDTH_TOTAL ) / MAGERY_SPELL_COLOR_OPTIONS_PER_ROW );

				g += (int)(1.5 * j);
				xs = SECTION_START_X;
				AddHtml( xs, g + 3, 110, 20, @"<BODY><BASEFONT Color=" + color + ">Magery Spell Color</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(xs + 124, g, 4011, 4011, (int)PageActionType.Setting_MagerySpellColor_Info, GumpButtonType.Reply, 0);

				g += j;
				xs = SETTING_START_X;
				AddAction(xs, g, from, "Default", PageActionType.Setting_MagerySpellColor_Default, MAGERY_SPELL_COLOR_OPTION_WIDTH);
				xs += MAGERY_SPELL_COLOR_OPTION_WIDTH + MAGERY_SPELL_COLOR_PADDING_LEFT;
				AddAction(xs, g, from, "Black", PageActionType.Setting_MagerySpellColor_Black, MAGERY_SPELL_COLOR_OPTION_WIDTH);
				xs += MAGERY_SPELL_COLOR_OPTION_WIDTH + MAGERY_SPELL_COLOR_PADDING_LEFT;
				AddAction(xs, g, from, "Blue", PageActionType.Setting_MagerySpellColor_Blue, MAGERY_SPELL_COLOR_OPTION_WIDTH);
				xs += MAGERY_SPELL_COLOR_OPTION_WIDTH + MAGERY_SPELL_COLOR_PADDING_LEFT;
				AddAction(xs, g, from, "Green", PageActionType.Setting_MagerySpellColor_Green, MAGERY_SPELL_COLOR_OPTION_WIDTH);
				xs += MAGERY_SPELL_COLOR_OPTION_WIDTH + MAGERY_SPELL_COLOR_PADDING_LEFT;

				g += j;
				xs = SETTING_START_X;
				AddAction(xs, g, from, "Purple", PageActionType.Setting_MagerySpellColor_Purple, MAGERY_SPELL_COLOR_OPTION_WIDTH);
				xs += MAGERY_SPELL_COLOR_OPTION_WIDTH + MAGERY_SPELL_COLOR_PADDING_LEFT;
				AddAction(xs, g, from, "Red", PageActionType.Setting_MagerySpellColor_Red, MAGERY_SPELL_COLOR_OPTION_WIDTH);
				xs += MAGERY_SPELL_COLOR_OPTION_WIDTH + MAGERY_SPELL_COLOR_PADDING_LEFT;
				AddAction(xs, g, from, "White", PageActionType.Setting_MagerySpellColor_White, MAGERY_SPELL_COLOR_OPTION_WIDTH);
				xs += MAGERY_SPELL_COLOR_OPTION_WIDTH + MAGERY_SPELL_COLOR_PADDING_LEFT;
				AddAction(xs, g, from, "Yellow", PageActionType.Setting_MagerySpellColor_Yellow, MAGERY_SPELL_COLOR_OPTION_WIDTH);
				xs += MAGERY_SPELL_COLOR_OPTION_WIDTH + MAGERY_SPELL_COLOR_PADDING_LEFT;
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			bool house = from.Region is HouseRegion && ((HouseRegion)from.Region).House.IsOwner(from);
			if ( from.Region.GetLogoutDelay( from ) != TimeSpan.Zero && house == false && !( from.Region is SkyHomeDwelling ) && !( from.Region is PrisonArea ) && !( from.Region is DungeonHomeRegion ) && !( from.Region is GargoyleRegion ) && !( from.Region is SafeRegion ) )
			{
				AddAction(nav_x, r, from, "Stuck in World", PageActionType.Do_StuckInWorld, NAVIGATION_ITEM_WIDTH);
				r += e;
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			AddAction(nav_x, r, from, "Change log", PageActionType.Navigate_Changelog, NAVIGATION_ITEM_WIDTH);
			r += e;
			if ( page == (int)PageActionType.Navigate_Changelog )
				AddChangelogPanel( color, secondaryPageNumber );
		}

        public void InvokeCommand( string c, Mobile from )
        {
            CommandSystem.Handle(from, String.Format("{0}{1}", CommandSystem.Prefix, c));
        }

		private void TryConfigureSpellBar(SpellBarSetupGumpBase gump)
		{
			if (gump.ConfigureGump())
			{
				gump.Player.CloseGump(gump.GetType());
				gump.Player.SendGump(gump);
			}
		}

		private void ToggleAutoOpenSpellBar( PlayerMobile from, SpellBarId id )
		{
			from.SpellBars.GetState(id).OpenOnLogin = !from.SpellBars.GetState(id).OpenOnLogin;
		}

		private void AddSetting(int x, int y, PlayerMobile from, string name, PageActionType actionType, PageActionType infoType, bool addActionButton = true)
		{
			if (addActionButton)
			{
				int isSelected = GetActionIcon(from, actionType);
				AddButton(x, y, isSelected, isSelected, (int)actionType, GumpButtonType.Reply, 0);
			}

			AddButton(x+40, y, 4011, 4011, (int)infoType, GumpButtonType.Reply, 0);
			AddHtml( x+80, y + 3, 316, 20, @"<BODY><BASEFONT Color=" + TEXT_COLOR + ">" + name + "</BASEFONT></BODY>", (bool)false, (bool)false);
		}

		private void AddAction(int x, int y, PlayerMobile from, string name, PageActionType actionType, int width = 100)
		{
			int isSelected = GetActionIcon(from, actionType);
			AddButton(x, y, isSelected, isSelected, (int)actionType, GumpButtonType.Reply, 0);
			AddHtml( x+40, y + 3, width, 20, @"<BODY><BASEFONT Color=" + TEXT_COLOR + ">" + name + "</BASEFONT></BODY>", (bool)false, (bool)false);
		}

		private void AddChangelogPanel( string color, int changelogPage )
		{
			var releaseCount = ChangeLog.RELEASE_COUNT;
			var releaseIndex = changelogPage;

			if ( releaseIndex < 0 ) releaseIndex = 0;
			else if ( releaseIndex >= releaseCount ) releaseIndex = releaseCount - 1;

			var releaseText = ChangeLog.GetRelease( releaseIndex );
			AddHtml( 252, 71, 739, 600, string.Format( @"<BODY><BASEFONT Color={0}>{1}</BASEFONT></BODY>", color, releaseText ), false, true );

			const int FOOTER_Y = 675;

			if ( releaseIndex > 0 )
			{
				AddButton( 252, FOOTER_Y, 4014, 4016, (int)PageActionType.Changelog_PageBase + ( releaseIndex - 1 ), GumpButtonType.Reply, 0 );
				AddHtml( 287, FOOTER_Y + 3, 80, 20, string.Format( @"<BODY><BASEFONT Color={0}>Prev</BASEFONT></BODY>", color ), false, false );
			}

			AddHtml( 500, FOOTER_Y + 3, 240, 20, string.Format( @"<BODY><BASEFONT Color={0}><CENTER>Release {1} of {2}</CENTER></BASEFONT></BODY>", color, releaseIndex + 1, releaseCount ), false, false );

			if ( releaseIndex < releaseCount - 1 )
			{
				AddButton( 920, FOOTER_Y, 4005, 4007, (int)PageActionType.Changelog_PageBase + ( releaseIndex + 1 ), GumpButtonType.Reply, 0 );
				AddHtml( 955, FOOTER_Y + 3, 80, 20, string.Format( @"<BODY><BASEFONT Color={0}>Next</BASEFONT></BODY>", color ), false, false );
			}
		}

		private void AddGeneralActionRowHeader(int x, int y, string label)
		{
			const int HORIZONTAL_LINE = 2700;
			const int BORDER_WIDTH = 2;

			AddImageTiled(x + 2, y + 25, 740, BORDER_WIDTH, HORIZONTAL_LINE);

			x += 10;
			AddHtml(x, y + 3, 200, 20, string.Format(@"<BODY><BASEFONT Color={0}>{1}</BASEFONT></BODY>", TEXT_COLOR, label), false, false);
		}

		private void AddGeneralActionRow(int x, int y, string name, PageActionType action, bool addTopSeparator = false)
		{
			const int RIGHT_ARROW = 4005;

			if (addTopSeparator)
			{
				const int HORIZONTAL_LINE = 2700;
				const int BORDER_WIDTH = 1;
				AddImageTiled(x + 2, y, 740, BORDER_WIDTH, HORIZONTAL_LINE);
				y += 4;
			}

			x += 20;
			AddButton(x, y, RIGHT_ARROW, RIGHT_ARROW, (int)action, GumpButtonType.Reply, 0);
			AddHtml(x + 40, y + 3, 200, 20, @"<BODY><BASEFONT Color=" + TEXT_COLOR + ">" + name + "</BASEFONT></BODY>", false, false);
		}

		private void AddMagicToolbarRowHeader(int x, int y)
		{
			const int HORIZONTAL_LINE = 2700;
			const int BORDER_WIDTH = 2;

			AddImageTiled(x + 2, y + 25, 740, BORDER_WIDTH, HORIZONTAL_LINE);

			x += 10;
			AddHtml(x, y + 3, 200, 20, @"<BODY><BASEFONT Color=" + TEXT_COLOR + ">Type</BASEFONT></BODY>", false, false);

			x += 215;
			AddHtml(x, y + 3, 100, 20, @"<BODY><BASEFONT Color=" + TEXT_COLOR + ">Open</BASEFONT></BODY>", false, false);

			x += 50;
			AddHtml(x, y + 3, 100, 20, @"<BODY><BASEFONT Color=" + TEXT_COLOR + ">Close</BASEFONT></BODY>", false, false);

			x += 50;
			AddHtml(x, y + 3, 100, 20, @"<BODY><BASEFONT Color=" + TEXT_COLOR + ">Manage</BASEFONT></BODY>", false, false);

			x += 75;
			AddHtml(x, y + 3, 100, 20, @"<BODY><BASEFONT Color=" + TEXT_COLOR + ">Open on Login</BASEFONT></BODY>", false, false);
		}

		private void AddToolbarRowOnlyOpen(int x, int y, string name, PageActionType open, bool addTopSeparator = false)
		{
			AddMagicToolbarRow(x, y, name, PageActionType.None, open, PageActionType.None, PageActionType.None, false, addTopSeparator);
		}

		private void AddMagicToolbarRow(int x, int y, string name, PageActionType config, PageActionType open, PageActionType close, PageActionType toggleAutoOpen, bool isAutoOpen, bool addTopSeparator = false)
		{
			const int RIGHT_ARROW = 4005;
			const int CANCEL_ICON = 4020;
			const int PAGE_ICON = 4011;
			const int CHECKED_BOX = 4018;
			const int UNCHECKED_BOX = 3609;

			if (addTopSeparator)
			{
				const int HORIZONTAL_LINE = 2700;
				const int BORDER_WIDTH = 1;
				AddImageTiled(x + 2, y, 740, BORDER_WIDTH, HORIZONTAL_LINE);
				y += 4;
			}

			x += 20;
			if (!string.IsNullOrWhiteSpace(name))
				AddHtml(x, y + 3, 200, 20, @"<BODY><BASEFONT Color=" + TEXT_COLOR + ">" + name + "</BASEFONT></BODY>", false, false);

			// Open
			x += 205;
			AddButton(x, y, RIGHT_ARROW, RIGHT_ARROW, (int)open, GumpButtonType.Reply, 0);

			// Close
			x += 50;
			if (close != PageActionType.None)
				AddButton(x, y, CANCEL_ICON, CANCEL_ICON, (int)close, GumpButtonType.Reply, 0);

			// Manage
			x += 50;
			if (config != PageActionType.None)
				AddButton(x, y, PAGE_ICON, PAGE_ICON, (int)config, GumpButtonType.Reply, 0);

			// Open on Login
			x += 75;
			if (toggleAutoOpen != PageActionType.None)
			{
				int isAutoOpenSelected = isAutoOpen ? CHECKED_BOX : UNCHECKED_BOX;
				AddButton(x, y, isAutoOpenSelected, isAutoOpenSelected, (int)toggleAutoOpen, GumpButtonType.Reply, 0);
			}
		}

		private int GetActionIcon( PlayerMobile from, PageActionType actionType )
		{
			const int RIGHT_ARROW = 4005;
			const int CANCEL_ICON = 4020;
			const int PAGE_ICON = 4011;
			const int CHECKED_BOX = 4018;
			const int UNCHECKED_BOX = 3609;

			switch ( actionType )
			{
				case PageActionType.Navigate_Main:
				case PageActionType.Navigate_Actions:
				case PageActionType.Navigate_Gumps:
				case PageActionType.Navigate_Bars:
				case PageActionType.Navigate_MagicToolbars:
				case PageActionType.Navigate_Changelog:
				case PageActionType.Show_Quests:
				case PageActionType.Show_Settings:
					return m_PageNumber == (int)actionType ? CHECKED_BOX : UNCHECKED_BOX;

				case PageActionType.Do_Achievements:
				case PageActionType.Show_Chat:
				case PageActionType.Show_Conversations:
				case PageActionType.Do_CorpseClear:
				case PageActionType.Do_CorpseSearch:
				case PageActionType.Show_Emote:
				case PageActionType.Navigate_Library:
				case PageActionType.Do_MoongateSearch:
				case PageActionType.Show_MOTD:
				case PageActionType.Show_QuickBar:
				case PageActionType.Show_RegBar:
				case PageActionType.Show_CombatBar:
				case PageActionType.Show_SkillList:
				case PageActionType.Show_Statistics:
				case PageActionType.Do_StuckInWorld:
				case PageActionType.Show_WealthBar:
				case PageActionType.Show_WeaponAbilities:

				case PageActionType.Setting_CreatureMagicFocus:
				case PageActionType.Setting_CreatureType:
				case PageActionType.Setting_CustomTitle:
				case PageActionType.Setting_LootOptions:
				case PageActionType.Setting_MusicPlaylist:
				case PageActionType.Setting_SkillTitle:
				case PageActionType.Setting_SetCraftingContainer:
				case PageActionType.Setting_SetHarvestingContainer:
				case PageActionType.Setting_SetLootContainer:
				case PageActionType.Setting_VendorContainerSell:
				case PageActionType.Setting_ModernSkills:
					return RIGHT_ARROW;
			}
			
			return IsActive(from, actionType) ? CHECKED_BOX : UNCHECKED_BOX;
		}

		private bool IsActive( PlayerMobile from, PageActionType actionType )
		{
			switch (actionType)
			{
				case PageActionType.Do_Toggle_AFK: return Server.Commands.AFK.m_AFK.Contains( from.Serial.Value );

				case PageActionType.Setting_AutoAttack: return !from.NoAutoAttack;
				case PageActionType.Setting_AutoSheath: return from.Preferences.CharacterSheath;
				case PageActionType.Setting_ClassicPoisoning: return from.Preferences.ClassicPoisoning;
				case PageActionType.Setting_CreatureSounds: return from.RaceMakeSounds;
				case PageActionType.Setting_GumpImages: return from.Preferences.GumpHue > 0;
				case PageActionType.Setting_MessageColors: return from.RainbowMsg;
				case PageActionType.Setting_MusicTone: return from.Preferences.CharMusical == "Forest";
				case PageActionType.Setting_PrivatePlay: return !from.PublicInfo;
				case PageActionType.Setting_WeaponAbilityBar: return from.Preferences.WeaponBarOpen;
				case PageActionType.Setting_WeaponAbilityNames: return from.Preferences.CharacterWepAbNames;
				case PageActionType.Setting_UseAncientSpellbook: return ResearchSettings.BookCaster( from );
				case PageActionType.Setting_DoubleClickToIDItems: return from.Preferences.DoubleClickID;
				case PageActionType.Setting_DoubleClickToTalk: return from.Preferences.DoubleClickToTalk;
				case PageActionType.Setting_OrdinaryResources: return from.HarvestOrdinary;
				case PageActionType.Setting_RemoveVendorGoldSafeguard: return from.Preferences.IgnoreVendorGoldSafeguard;
				case PageActionType.Setting_SuppressVendorTooltips: return from.Preferences.SuppressVendorTooltip;
				case PageActionType.Setting_SingleAttemptID: return from.Preferences.SingleAttemptID;
				case PageActionType.Setting_ColorlessFabricBreakdown: return from.Preferences.ColorlessFabricBreakdown;
				case PageActionType.Setting_LegacyCarve: return from.Preferences.LegacyCarve;

				case PageActionType.Setting_Playstyle_Normal: return !from.Preferences.CharacterEvil && !from.Preferences.CharacterOriental && from.Preferences.CharacterBarbaric == 0;
				case PageActionType.Setting_Playstyle_Evil: return from.Preferences.CharacterEvil;
				case PageActionType.Setting_Playstyle_Oriental: return from.Preferences.CharacterOriental;
				case PageActionType.Setting_Playstyle_Barbaric: return from.Preferences.CharacterBarbaric == 1 || from.Preferences.CharacterBarbaric == 2;

				case PageActionType.Setting_MagerySpellColor_White: return from.Preferences.MagerySpellHue == 0x47E;
				case PageActionType.Setting_MagerySpellColor_Black: return from.Preferences.MagerySpellHue == 0x94E;
				case PageActionType.Setting_MagerySpellColor_Blue: return from.Preferences.MagerySpellHue == 0x48D;
				case PageActionType.Setting_MagerySpellColor_Red: return from.Preferences.MagerySpellHue == 0x48E;
				case PageActionType.Setting_MagerySpellColor_Green: return from.Preferences.MagerySpellHue == 0x48F;
				case PageActionType.Setting_MagerySpellColor_Purple: return from.Preferences.MagerySpellHue == 0x490;
				case PageActionType.Setting_MagerySpellColor_Yellow: return from.Preferences.MagerySpellHue == 0x491;
				case PageActionType.Setting_MagerySpellColor_Default: return from.Preferences.MagerySpellHue == 0;

				case PageActionType.Setting_SkillList: return true;
			}

			return false;
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			PlayerMobile from = state.Mobile as PlayerMobile;
			if ( from == null ) return;

			int pressed = info.ButtonID;
			PageActionType actionType = (PageActionType)info.ButtonID;

			from.SendSound( 0x4A ); 

			from.CloseGump( typeof(Server.Engines.Help.HelpGump) );

			if ( pressed >= (int)PageActionType.Changelog_PageBase && pressed < (int)PageActionType.Changelog_PageBase + ChangeLog.RELEASE_COUNT )
			{
				var releaseIndex = pressed - (int)PageActionType.Changelog_PageBase;
				from.SendGump( new HelpGump( from, (int)PageActionType.Navigate_Changelog, releaseIndex ) );
				return;
			}

			if ( ShowHelpInfoWindow( from, actionType ) ) return;

			bool reopenPage = false;
			switch ( actionType )
			{
				case PageActionType.Close:
				{
					//from.SendLocalizedMessage( 501235, "", 0x35 ); // Help request aborted.
					break;
				}
				case PageActionType.Navigate_Main:
				case PageActionType.Navigate_Actions:
				case PageActionType.Navigate_Gumps:
				case PageActionType.Navigate_Bars:
				{
					from.SendGump( new Server.Engines.Help.HelpGump( from, (int)actionType ) );
					break;
				}
				case PageActionType.Do_Achievements:
				{
					reopenPage = true;
					AchievementSystem.OpenGump( from, from );
					break;
				}
				case PageActionType.Do_Toggle_AFK:
				{
					reopenPage = true;
					InvokeCommand( "afk", from );
					break;
				}
				case PageActionType.Show_Chat:
				{
					reopenPage = true;
					InvokeCommand( "c", from );
					break;
				}
				case PageActionType.Do_CorpseClear:
				{
					reopenPage = true;
					InvokeCommand( "corpseclear", from );
					break;
				}
				case PageActionType.Do_CorpseSearch:
				{
					reopenPage = true;
					InvokeCommand( "corpse", from );
					break;
				}
				case PageActionType.Show_Emote:
				{
					reopenPage = true;
					InvokeCommand( "emote", from );
					break;
				}
				case PageActionType.Navigate_MagicToolbars:
				{
					from.SendGump( new Server.Engines.Help.HelpGump( from, (int)PageActionType.Navigate_MagicToolbars ) );
					break;
				}
				case PageActionType.Do_MoongateSearch:
				{
					reopenPage = true;
					InvokeCommand( "magicgate", from );
					break;
				}
				case PageActionType.Show_MOTD:
				{
					reopenPage = true;
					from.CloseGump( typeof( Joeku.MOTD.MOTD_Gump ) );
					Joeku.MOTD.MOTD_Utility.SendGump( from, false, 0, 0 );
					break;
				}
				case PageActionType.Show_Quests:
				{
					from.SendGump( new Server.Engines.Help.HelpGump( from, (int)PageActionType.Show_Quests ) );
					from.SendGump(new QuestLogGump((PlayerMobile)from));
					break;
				}
				case PageActionType.Show_QuickBar:
				{
					reopenPage = true;
					from.CloseGump( typeof( QuickBar ) );
					from.SendGump( new QuickBar( from ) );
					break;
				}
				case PageActionType.Show_QuickBarManage:
				{
					from.CloseGump( typeof( QuickConfig ) );
					from.SendGump( new QuickConfig( from ) );
					break;
				}
				case PageActionType.Do_QuickBarAutoOpen:
				{
					reopenPage = true;
					PlayerSettings.SetQuickConfig( from, QuickConfig.KEY_AUTO_OPEN );
					break;
				}
				case PageActionType.Show_RegBar:
				{
					reopenPage = true;
					from.CloseGump( typeof( RegBar ) );
					from.SendGump( new RegBar( from ) );
					break;
				}
				case PageActionType.Show_CombatBar:
				{
					reopenPage = true;
					from.CloseGump( typeof( CombatBar.CombatBarGump ) );
					from.SendGump( new CombatBar.CombatBarGump( from ) );
					break;
				}
				case PageActionType.Show_Settings:
				{
					from.SendGump( new HelpGump( from, (int)PageActionType.Show_Settings ) );
					break;
				}
				case PageActionType.Navigate_Library:
				{
					from.CloseGump( typeof( MyLibrary ) );
					from.SendSound( 0x4A ); 
					from.SendGump( new MyLibrary( from, (int)PageActionType.Navigate_Gumps ) );
					break;
				}
				case PageActionType.Show_Statistics:
				{
					from.CloseGump( typeof( Server.Statistics.StatisticsGump ) );
					from.SendGump( new Server.Statistics.StatisticsGump( from, (int)PageActionType.Navigate_Gumps ) );
					break;
				}
				case PageActionType.Do_StuckInWorld:
				{
					BaseHouse house = BaseHouse.FindHouseAt( from );

					if ( house != null && house.IsAosRules )
					{
						from.Location = house.BanLocation;
					}
					else if ( from.Region.IsPartOf( typeof( Server.Regions.Jail ) ) )
					{
						from.SendLocalizedMessage( 1041530, "", 0x35 ); // You'll need a better jailbreak plan then that!
					}
					else if ( from.CanUseStuckMenu() && from.Region.CanUseStuckMenu( from ) && !CheckCombat( from ) && !from.Frozen && !from.Criminal && (Core.AOS || from.Kills < 5) )
					{
						StuckMenu menu = new StuckMenu( from, from, true );

						menu.BeginClose();

						from.SendGump( menu );
					}

					break;
				}
				case PageActionType.Show_WeaponAbilities:
				{
					reopenPage = true;
					InvokeCommand( "sad", from );
					break;
				}
				case PageActionType.Show_WealthBar:
				{
					reopenPage = true;
					from.CloseGump( typeof( WealthBar ) );
					from.SendGump( new WealthBar( from ) );
					break;
				}
				case PageActionType.Show_Conversations:
				{
					from.CloseGump( typeof( MyChat ) );
					from.SendSound( 0x4A ); 
					from.SendGump( new MyChat( from, (int)PageActionType.Navigate_Gumps ) );
					break;
				}
				case PageActionType.Navigate_Changelog:
				{
					from.SendGump( new Server.Engines.Help.HelpGump( from, (int)PageActionType.Navigate_Changelog ) );
					break;
				}

				case PageActionType.Setting_OrdinaryResources:
				{
					reopenPage = true;
					if ( from.HarvestOrdinary )
						from.HarvestOrdinary = false;
					else
						from.HarvestOrdinary = true;
					break;
				}
				case PageActionType.Setting_SetLootContainer:
				case PageActionType.Setting_SetCraftingContainer:
				case PageActionType.Setting_SetHarvestingContainer:
				{
					reopenPage = true;
					int box;
					switch(actionType)
					{
						case PageActionType.Setting_SetLootContainer: box = 1; break;
						case PageActionType.Setting_SetCraftingContainer: box = 2; break;
						case PageActionType.Setting_SetHarvestingContainer: box = 3; break;
						default: Console.WriteLine("[HelpGump] Invalid container type: {0}", pressed); return;
					}

					BaseContainer.ContainerSetTarget( from, box );
					break;
				}
				case PageActionType.Setting_WeaponAbilityNames:
				{
					reopenPage = true;
					from.Preferences.CharacterWepAbNames = !from.Preferences.CharacterWepAbNames;
					break;
				}
				case PageActionType.Setting_AutoSheath:
				{
					reopenPage = true;
					from.Preferences.CharacterSheath = !from.Preferences.CharacterSheath;
					break;
				}
				case PageActionType.Setting_MusicTone:
				{
					reopenPage = true;
					string tunes = from.Preferences.CharMusical;

					if ( tunes == "Forest" )
					{
						from.Preferences.CharMusical = "Dungeon";
					}
					else
					{
						from.Preferences.CharMusical = "Forest";
					}
					break;
				}
				case PageActionType.Setting_PrivatePlay:
				{
					reopenPage = true;
					PlayerMobile pm = (PlayerMobile)from;

					if ( pm.PublicInfo == false )
					{
						pm.PublicInfo = true;
					}
					else
					{
						pm.PublicInfo = false;
					}
					break;
				}
				case PageActionType.Setting_LootOptions:
				{
					from.CloseGump( typeof( LootChoices ) );
					from.SendGump( new LootChoices( from, 1 ) );
					break;
				}
				case PageActionType.Setting_SkillTitle:
				{
					from.CloseGump( typeof( SkillTitleGump ) );
					from.SendGump( new SkillTitleGump( from ) );
					break;
				}
				case PageActionType.Setting_UseAncientSpellbook:
				{
					reopenPage = true;
					if ( !ResearchSettings.BookCaster( from ) )
					{
						((PlayerMobile)from).Preferences.UsingAncientBook = true;
					}
					else
					{
						((PlayerMobile)from).Preferences.UsingAncientBook = false;
					}
					break;
				}
				case PageActionType.Setting_SkillList:
				{
					reopenPage = true;
					if ( ((PlayerMobile)from).SkillDisplay > 0 ){ ((PlayerMobile)from).SkillDisplay = 0; } else { ((PlayerMobile)from).SkillDisplay = 1; }
					Server.Gumps.SkillListingGump.RefreshSkillList( from );
					break;
				}
				case PageActionType.Show_SkillList:
				{
					reopenPage = true;
					Server.Gumps.SkillListingGump.OpenSkillList( from );
					break;
				}
				case PageActionType.Setting_GumpImages:
				{
					reopenPage = true;
					int gump = ((PlayerMobile)from).Preferences.GumpHue;

					if ( gump > 0 )
					{
						((PlayerMobile)from).Preferences.GumpHue = 0;
					}
					else
					{
						((PlayerMobile)from).Preferences.GumpHue = 1;
					}
					break;
				}
				case PageActionType.Setting_WeaponAbilityBar:
				{
					reopenPage = true;
					from.Preferences.WeaponBarOpen = !from.Preferences.WeaponBarOpen;
					break;
				}
				case PageActionType.Setting_CreatureMagicFocus:
				{
					reopenPage = true;
					if ( from.RaceMagicSchool == 0 )
							from.RaceMagicSchool = 1;
					else if ( from.RaceMagicSchool == 1 )
							from.RaceMagicSchool = 2;
					else if ( from.RaceMagicSchool == 2 )
							from.RaceMagicSchool = 3;
					else
							from.RaceMagicSchool = 0;

					if ( from.FindItemOnLayer( Layer.Special ) != null && from.RaceID > 0 )
					{
						if ( from.FindItemOnLayer( Layer.Special ) is BaseRace )
							Server.Items.BaseRace.SetMonsterMagic( from, (BaseRace)(from.FindItemOnLayer( Layer.Special )) );
					}
					break;
				}
				case PageActionType.Setting_CreatureType:
				{
					from.RaceSection = 1;
					from.SendGump( new Server.Items.RacePotions.RacePotionsGump( from, 1, true ) );
					break;
				}
				case PageActionType.Setting_CreatureSounds:
				{
					reopenPage = true;
					if ( !from.RaceMakeSounds )
							from.RaceMakeSounds = true;
					else
							from.RaceMakeSounds = false;
					break;
				}
				case PageActionType.Setting_Playstyle_Normal:
				{
					reopenPage = true;
					((PlayerMobile)from).Preferences.CharacterEvil = false;
					((PlayerMobile)from).Preferences.CharacterOriental = false;
					((PlayerMobile)from).Preferences.CharacterBarbaric = 0;
					Server.Items.BarbaricSatchel.GetRidOf( from );
					break;
				}
				case PageActionType.Setting_Playstyle_Evil:
				{
					reopenPage = true;
					((PlayerMobile)from).Preferences.CharacterEvil = true;
					((PlayerMobile)from).Preferences.CharacterOriental = false;
					((PlayerMobile)from).Preferences.CharacterBarbaric = 0;
					Server.Items.BarbaricSatchel.GetRidOf( from );
					break;
				}
				case PageActionType.Setting_Playstyle_Oriental:
				{
					reopenPage = true;
					((PlayerMobile)from).Preferences.CharacterEvil = false;
					((PlayerMobile)from).Preferences.CharacterOriental = true;
					((PlayerMobile)from).Preferences.CharacterBarbaric = 0;
					Server.Items.BarbaricSatchel.GetRidOf( from );
					break;
				}
				case PageActionType.Setting_MessageColors:
				{
					reopenPage = true;
					if ( from.RainbowMsg )
					{
						from.RainbowMsg = false;
					}
					else
					{
						from.RainbowMsg = true;
					}
					break;
				}
				case PageActionType.Setting_AutoAttack:
				{
					reopenPage = true;
					if ( from.NoAutoAttack )
					{
						from.NoAutoAttack = false;
					}
					else
					{
						from.NoAutoAttack = true;
					}
					break;
				}
				case PageActionType.Setting_Playstyle_Barbaric:
				{
					reopenPage = true;
					if ( ((PlayerMobile)from).Preferences.CharacterBarbaric == 1 && from.Female )
					{
						((PlayerMobile)from).Preferences.CharacterBarbaric = 2;
					}
					else if ( ((PlayerMobile)from).Preferences.CharacterBarbaric > 0 )
					{
						((PlayerMobile)from).Preferences.CharacterBarbaric = 0;
						Server.Items.BarbaricSatchel.GetRidOf( from );
					}
					else
					{
						((PlayerMobile)from).Preferences.CharacterEvil = false;
						((PlayerMobile)from).Preferences.CharacterOriental = false;
						((PlayerMobile)from).Preferences.CharacterBarbaric = 1;
						Server.Items.BarbaricSatchel.GivePack( from );
					}
					break;
				}
				case PageActionType.Setting_ClassicPoisoning:
				{
					reopenPage = true;
					from.Preferences.ClassicPoisoning = !from.Preferences.ClassicPoisoning;
					break;
				}
				case PageActionType.Setting_MusicPlaylist:
				{
					from.CloseGump( typeof( MusicPlaylist ) );
					from.SendGump( new MusicPlaylist( from, 1 ) );
					break;
				}
				case PageActionType.Setting_DoubleClickToIDItems:
				{
					reopenPage = true;
					from.Preferences.DoubleClickID = !from.Preferences.DoubleClickID;
					break;
				}
				case PageActionType.Setting_DoubleClickToTalk:
				{
					reopenPage = true;
					from.Preferences.DoubleClickToTalk = !from.Preferences.DoubleClickToTalk;
					break;
				}
				case PageActionType.Setting_RemoveVendorGoldSafeguard:
				{
					reopenPage = true;
					from.Preferences.IgnoreVendorGoldSafeguard = !from.Preferences.IgnoreVendorGoldSafeguard;
					break;
				}
				case PageActionType.Setting_SuppressVendorTooltips:
				{
					reopenPage = true;
					from.Preferences.SuppressVendorTooltip = !from.Preferences.SuppressVendorTooltip;
					break;
				}
				case PageActionType.Setting_VendorContainerSell:
				{
					reopenPage = true;
					from.CloseGump( typeof( VendorContainerSellConfigGump ) );
					break;
				}
				case PageActionType.Setting_ModernSkills:
				{
					reopenPage = true;
					from.CloseGump( typeof( ModernSkillSetupGump ) );
					break;
				}
				case PageActionType.Setting_SingleAttemptID:
				{
					reopenPage = true;
					from.Preferences.SingleAttemptID = !from.Preferences.SingleAttemptID;
					break;
				}
				case PageActionType.Setting_ColorlessFabricBreakdown:
				{
					reopenPage = true;
					from.Preferences.ColorlessFabricBreakdown = !from.Preferences.ColorlessFabricBreakdown;
					break;
				}

				case PageActionType.MagicToolbar_Ancient_I_Close: reopenPage = true; InvokeCommand( "archclose1", from ); break;
				case PageActionType.MagicToolbar_Ancient_I_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Ancient_1, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Ancient_I_Open: reopenPage = true; InvokeCommand( "archtool1", from ); break;
				case PageActionType.MagicToolbar_Ancient_I_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Ancient_1 ); break;
				case PageActionType.MagicToolbar_Ancient_II_Close: reopenPage = true; InvokeCommand( "archclose2", from ); break;
				case PageActionType.MagicToolbar_Ancient_II_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Ancient_2, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Ancient_II_Open: reopenPage = true; InvokeCommand( "archtool2", from ); break;
				case PageActionType.MagicToolbar_Ancient_II_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Ancient_2 ); break;
				case PageActionType.MagicToolbar_Ancient_III_Close: reopenPage = true; InvokeCommand( "archclose3", from ); break;
				case PageActionType.MagicToolbar_Ancient_III_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Ancient_3, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Ancient_III_Open: reopenPage = true; InvokeCommand( "archtool3", from ); break;
				case PageActionType.MagicToolbar_Ancient_III_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Ancient_3 ); break;
				case PageActionType.MagicToolbar_Ancient_IV_Close: reopenPage = true; InvokeCommand( "archclose4", from ); break;
				case PageActionType.MagicToolbar_Ancient_IV_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Ancient_4, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Ancient_IV_Open: reopenPage = true; InvokeCommand( "archtool4", from ); break;
				case PageActionType.MagicToolbar_Ancient_IV_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Ancient_4 ); break;
				case PageActionType.MagicToolbar_Bard_I_Close: reopenPage = true; InvokeCommand( "bardclose1", from ); break;
				case PageActionType.MagicToolbar_Bard_I_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Bard_1, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Bard_I_Open: reopenPage = true; InvokeCommand( "bardtool1", from ); break;
				case PageActionType.MagicToolbar_Bard_I_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Bard_1 ); break;
				case PageActionType.MagicToolbar_Bard_II_Close: reopenPage = true; InvokeCommand( "bardclose2", from ); break;
				case PageActionType.MagicToolbar_Bard_II_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Bard_2, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Bard_II_Open: reopenPage = true; InvokeCommand( "bardtool2", from ); break;
				case PageActionType.MagicToolbar_Bard_II_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Bard_2 ); break;
				case PageActionType.MagicToolbar_DeathKnight_I_Close: reopenPage = true; InvokeCommand( "deathclose1", from ); break;
				case PageActionType.MagicToolbar_DeathKnight_I_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Death_1, from, 1 ) ); break;
				case PageActionType.MagicToolbar_DeathKnight_I_Open: reopenPage = true; InvokeCommand( "deathtool1", from ); break;
				case PageActionType.MagicToolbar_DeathKnight_I_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Death_1 ); break;
				case PageActionType.MagicToolbar_DeathKnight_II_Close: reopenPage = true; InvokeCommand( "deathclose2", from ); break;
				case PageActionType.MagicToolbar_DeathKnight_II_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Death_2, from, 1 ) ); break;
				case PageActionType.MagicToolbar_DeathKnight_II_Open: reopenPage = true; InvokeCommand( "deathtool2", from ); break;
				case PageActionType.MagicToolbar_DeathKnight_II_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Death_2 ); break;
				case PageActionType.MagicToolbar_Elemental_I_Close: reopenPage = true; InvokeCommand( "elementclose1", from ); break;
				case PageActionType.MagicToolbar_Elemental_I_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Elemental_1, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Elemental_I_Open: reopenPage = true; InvokeCommand( "elementtool1", from ); break;
				case PageActionType.MagicToolbar_Elemental_I_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Elemental_1 ); break;
				case PageActionType.MagicToolbar_Elemental_II_Close: reopenPage = true; InvokeCommand( "elementclose2", from ); break;
				case PageActionType.MagicToolbar_Elemental_II_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Elemental_2, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Elemental_II_Open: reopenPage = true; InvokeCommand( "elementtool2", from ); break;
				case PageActionType.MagicToolbar_Elemental_II_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Elemental_2 ); break;
				case PageActionType.MagicToolbar_Knight_I_Close: reopenPage = true; InvokeCommand( "knightclose1", from ); break;
				case PageActionType.MagicToolbar_Knight_I_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Knight_1, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Knight_I_Open: reopenPage = true; InvokeCommand( "knighttool1", from ); break;
				case PageActionType.MagicToolbar_Knight_I_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Knight_1 ); break;
				case PageActionType.MagicToolbar_Knight_II_Close: reopenPage = true; InvokeCommand( "knightclose2", from ); break;
				case PageActionType.MagicToolbar_Knight_II_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Knight_2, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Knight_II_Open: reopenPage = true; InvokeCommand( "knighttool2", from ); break;
				case PageActionType.MagicToolbar_Knight_II_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Knight_2 ); break;
				case PageActionType.MagicToolbar_Magery_I_Close: reopenPage = true; InvokeCommand( "mageclose1", from ); break;
				case PageActionType.MagicToolbar_Magery_I_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Mage_1, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Magery_I_Open: reopenPage = true; InvokeCommand( "magetool1", from ); break;
				case PageActionType.MagicToolbar_Magery_I_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Mage_1 ); break;
				case PageActionType.MagicToolbar_Magery_II_Close: reopenPage = true; InvokeCommand( "mageclose2", from ); break;
				case PageActionType.MagicToolbar_Magery_II_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Mage_2, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Magery_II_Open: reopenPage = true; InvokeCommand( "magetool2", from ); break;
				case PageActionType.MagicToolbar_Magery_II_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Mage_2 ); break;
				case PageActionType.MagicToolbar_Magery_III_Close: reopenPage = true; InvokeCommand( "mageclose3", from ); break;
				case PageActionType.MagicToolbar_Magery_III_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Mage_3, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Magery_III_Open: reopenPage = true; InvokeCommand( "magetool3", from ); break;
				case PageActionType.MagicToolbar_Magery_III_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Mage_3 ); break;
				case PageActionType.MagicToolbar_Magery_IV_Close: reopenPage = true; InvokeCommand( "mageclose4", from ); break;
				case PageActionType.MagicToolbar_Magery_IV_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Mage_4, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Magery_IV_Open: reopenPage = true; InvokeCommand( "magetool4", from ); break;
				case PageActionType.MagicToolbar_Magery_IV_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Mage_4 ); break;
				case PageActionType.MagicToolbar_Monk_I_Close: reopenPage = true; InvokeCommand( "monkclose1", from ); break;
				case PageActionType.MagicToolbar_Monk_I_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Monk_1, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Monk_I_Open: reopenPage = true; InvokeCommand( "monktool1", from ); break;
				case PageActionType.MagicToolbar_Monk_I_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Monk_1 ); break;
				case PageActionType.MagicToolbar_Monk_II_Close: reopenPage = true; InvokeCommand( "monkclose2", from ); break;
				case PageActionType.MagicToolbar_Monk_II_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Monk_2, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Monk_II_Open: reopenPage = true; InvokeCommand( "monktool2", from ); break;
				case PageActionType.MagicToolbar_Monk_II_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Monk_2 ); break;
				case PageActionType.MagicToolbar_Necromancer_I_Close: reopenPage = true; InvokeCommand( "necroclose1", from ); break;
				case PageActionType.MagicToolbar_Necromancer_I_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Necro_1, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Necromancer_I_Open: reopenPage = true; InvokeCommand( "necrotool1", from ); break;
				case PageActionType.MagicToolbar_Necromancer_I_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Necro_1 ); break;
				case PageActionType.MagicToolbar_Necromancer_II_Close: reopenPage = true; InvokeCommand( "necroclose2", from ); break;
				case PageActionType.MagicToolbar_Necromancer_II_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Necro_2, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Necromancer_II_Open: reopenPage = true; InvokeCommand( "necrotool2", from ); break;
				case PageActionType.MagicToolbar_Necromancer_II_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Necro_2 ); break;
				case PageActionType.MagicToolbar_Priest_I_Close: reopenPage = true; InvokeCommand( "holyclose1", from ); break;
				case PageActionType.MagicToolbar_Priest_I_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Priest_1, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Priest_I_Open: reopenPage = true; InvokeCommand( "holytool1", from ); break;
				case PageActionType.MagicToolbar_Priest_I_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Priest_1 ); break;
				case PageActionType.MagicToolbar_Priest_II_Close: reopenPage = true; InvokeCommand( "holyclose2", from ); break;
				case PageActionType.MagicToolbar_Priest_II_Config: TryConfigureSpellBar( SpellBarRegistry.CreateSetupGump( SpellBarId.Priest_2, from, 1 ) ); break;
				case PageActionType.MagicToolbar_Priest_II_Open: reopenPage = true; InvokeCommand( "holytool2", from ); break;
				case PageActionType.MagicToolbar_Priest_II_ToggleAutoOpen: reopenPage = true; ToggleAutoOpenSpellBar( from, SpellBarId.Priest_2 ); break;

				case PageActionType.Setting_MagerySpellColor_White:
				{
					reopenPage = true;
					from.Preferences.MagerySpellHue = 0x47E;
					break;
				}
				case PageActionType.Setting_MagerySpellColor_Black:
				{
					reopenPage = true;
					from.Preferences.MagerySpellHue = 0x94E;
					break;
				}
				case PageActionType.Setting_MagerySpellColor_Blue:
				{
					reopenPage = true;
					from.Preferences.MagerySpellHue = 0x48D;
					break;
				}
				case PageActionType.Setting_MagerySpellColor_Red:
				{
					reopenPage = true;
					from.Preferences.MagerySpellHue = 0x48E;
					break;
				}
				case PageActionType.Setting_MagerySpellColor_Green:
				{
					reopenPage = true;
					from.Preferences.MagerySpellHue = 0x48F;
					break;
				}
				case PageActionType.Setting_MagerySpellColor_Purple:
				{
					reopenPage = true;
					from.Preferences.MagerySpellHue = 0x490;
					break;
				}
				case PageActionType.Setting_MagerySpellColor_Yellow:
				{
					reopenPage = true;
					from.Preferences.MagerySpellHue = 0x491;
					break;
				}
				case PageActionType.Setting_MagerySpellColor_Default:
				{
					reopenPage = true;
					from.Preferences.MagerySpellHue = 0;
					break;
				}

				case PageActionType.Setting_CustomTitle:
				{
					from.SendGump( new CustomTitleGump( from ) );
					break;
				}
			}
			
			if ( reopenPage )
				from.SendGump( new Server.Engines.Help.HelpGump( from, m_PageNumber ) );
		}

		public static string MyQuests( Mobile from )
        {
			PlayerMobile pm = (PlayerMobile)from;

			string sQuests = "Below is a brief list of current quests, along with achievements in specific discoveries. These are owned quests, which are specific to your character. Other quests (like messages in a bottle, treasure maps, or scribbled notes) are not listed here.<br><br>";

			string ContractQuest = PlayerSettings.GetQuestInfo( from, "StandardQuest" );
			if ( PlayerSettings.GetQuestState( from, "StandardQuest" ) ){ string sAdventurer = StandardQuestFunctions.QuestStatus( from ); sQuests = sQuests + "-" + sAdventurer + ".<br><br>"; }

			string ContractKiller = PlayerSettings.GetQuestInfo( from, "AssassinQuest" );
			if ( PlayerSettings.GetQuestState( from, "AssassinQuest" ) ){ string sAssassin = AssassinFunctions.QuestStatus( from ); sQuests = sQuests + "-" + sAssassin + ".<br><br>"; }

			string ContractSailor = PlayerSettings.GetQuestInfo( from, "FishingQuest" );
			if ( PlayerSettings.GetQuestState( from, "FishingQuest" ) ){ string sSailor = FishingQuestFunctions.QuestStatus( from ); sQuests = sQuests + "-" + sSailor + ".<br><br>"; }

			sQuests = sQuests + OtherQuests( from );

			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleMadGodName" ) ){ sQuests = sQuests + "-Learned about the Mad God Tarjan.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleCatacombKey" ) ){ sQuests = sQuests + "-The priest from the Mad God Temple gave me the key to the Catacombs.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleSpectreEye" ) ){ sQuests = sQuests + "-Found a mysterious eye from the Catacombs.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleHarkynKey" ) ){ sQuests = sQuests + "-Found a key with a symbol of a dragon on it.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleDragonKey" ) ){ sQuests = sQuests + "-Found a rusty key from around a gray dragon's neck.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleCrystalSword" ) ){ sQuests = sQuests + "-Found a crystal sword.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleSilverSquare" ) ){ sQuests = sQuests + "-Found a silver square.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleKylearanKey" ) ){ sQuests = sQuests + "-Found a key with a symbol of a unicorn on it.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleBedroomKey" ) ){ sQuests = sQuests + "-Found a key with a symbol of a tree on it.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleSilverTriangle" ) ){ sQuests = sQuests + "-Found a silver triangle.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleCrystalGolem" ) ){ sQuests = sQuests + "-Destroyed the crystal golem and found a golden key.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleEbonyKey" ) ){ sQuests = sQuests + "-Kylearan gave me an ebony key with a demon symbol on it.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleSilverCircle" ) ){ sQuests = sQuests + "-Found a silver circle.<br><br>"; }
			if ( PlayerSettings.GetBardsTaleQuest( from, "BardsTaleWin" ) && ((PlayerMobile)from).Fugitive != 1 ){ sQuests = sQuests + "-Defeated the evil wizard Mangar and escaped Skara Brae.<br><br>"; }

			if ( PlayerSettings.GetKeys( from, "UndermountainKey" ) ){ sQuests = sQuests + "-Found a key made of dwarven steel.<br><br>"; }
			if ( PlayerSettings.GetKeys( from, "BlackKnightKey" ) ){ sQuests = sQuests + "-Found the Black Knight's key.<br><br>"; }
			if ( PlayerSettings.GetKeys( from, "SkullGate" ) ){ sQuests = sQuests + "-Discovered the secret of Skull Gate.<br>   One is in the Undercity of Umbra in Sosaria.<br>   The other is in the Ravendark Woods.<br><br>"; }
			if ( PlayerSettings.GetKeys( from, "SerpentPillars" ) ){ sQuests = sQuests + "-Discovered the secret of the Serpent Pillars.<br>   Sosaria: 86° 41'S, 124° 39'E<br>   Lodoria: 35° 36'S, 65° 2'E<br><br>"; }
			if ( PlayerSettings.GetKeys( from, "RangerOutpost" ) ){ sQuests = sQuests + "-Discovered the Ranger Outpost.<br><br>"; }
			if ( PlayerSettings.GetKeys( from, "DragonRiding" ) ){ sQuests = sQuests + "-Learned the secrets of riding draconic creatures.<br><br>"; }

			if ( PlayerSettings.GetDiscovered( from, Land.Sosaria ) ){ sQuests = sQuests + "-Discovered the World of Sosaria.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, Land.UmberVeil ) ){ sQuests = sQuests + "-Discovered Umber Veil.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, Land.Ambrosia ) ){ sQuests = sQuests + "-Discovered Ambrosia.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, Land.Lodoria ) ){ sQuests = sQuests + "-Discovered the Elven World of Lodoria.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, Land.Serpent ) ){ sQuests = sQuests + "-Discovered the Serpent Island.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, Land.IslesDread ) ){ sQuests = sQuests + "-Discovered the Isles of Dread.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, Land.Savaged ) ){ sQuests = sQuests + "-Discovered the Valley of the Savaged Empire.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, Land.Kuldar ) ){ sQuests = sQuests + "-Discovered the Bottle World of Kuldar.<br><br>"; }
			if ( PlayerSettings.GetDiscovered( from, Land.Underworld ) ){ sQuests = sQuests + "-Discovered the Underworld.<br><br>"; }

			return "Quests For " + from.Name + "<br><br>" + sQuests;
        }

		public static string OtherQuests( Mobile from )
        {
			string quests = "";

			ArrayList targets = new ArrayList();
			foreach ( Item item in World.Items.Values )
			{
				if ( item is ThiefNote )
				{
					if ( ((ThiefNote)item).NoteOwner == from )
					{
						if ( Server.Items.ThiefNote.ThiefAllowed( from ) == null )
						{
							quests = quests + "-" + ((ThiefNote)item).NoteStory + "<br><br>";
						}
						else
						{
							quests = quests + "-You have a secret note instructing you to steal something, but you will take a break from thieving and read it in about " + Server.Items.ThiefNote.ThiefAllowed( from ) + " minutes.<br><br>";
						}
					}
				}
				else if ( item is CourierMail )
				{
					if ( ((CourierMail)item).Owner == from )
					{
						quests = quests + "-You need to find " + ((CourierMail)item).SearchItem + " for " + ((CourierMail)item).ForWho + ". They said in their letter that you should search in " + ((CourierMail)item).SearchDungeon + " in " + ((CourierMail)item).SearchWorld + ".<br><br>";
					}
				}
				else if ( item is SearchPage )
				{
					if ( ((SearchPage)item).Owner == from )
					{
						quests = quests + "-You want to find " + ((SearchPage)item).SearchItem + " in " + ((SearchPage)item).SearchDungeon + " in " + ((SearchPage)item).SearchWorld + ".<br><br>";
					}
				}
				else if ( item is SummonPrison )
				{
					if ( ((SummonPrison)item).owner == from )
					{
						quests = quests + "-You currently have " + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(((SummonPrison)item).Prisoner.ToLower()) + " in a Magical Prison.<br><br>";
					}
				}
				else if ( item is FrankenJournal )
				{
					if ( ((FrankenJournal)item).JournalOwner == from )
					{
						int parts = 0;
						if ( ((FrankenJournal)item).HasArmRight > 0 ){ parts++; }
						if ( ((FrankenJournal)item).HasArmLeft > 0 ){ parts++; }
						if ( ((FrankenJournal)item).HasLegRight > 0 ){ parts++; }
						if ( ((FrankenJournal)item).HasLegLeft > 0 ){ parts++; }
						if ( ((FrankenJournal)item).HasTorso > 0 ){ parts++; }
						if ( ((FrankenJournal)item).HasHead > 0 ){ parts++; }

						quests = quests + "-You currently have " + parts + " out of 6 body parts needed to create a flesh golem.<br><br>";
					}
				}
				else if ( item is RuneBox )
				{
					if ( ((RuneBox)item).RuneBoxOwner == from )
					{
						int runes = 0;
						if ( ((RuneBox)item).HasCompassion > 0 ){ runes++; }
						if ( ((RuneBox)item).HasHonesty > 0 ){ runes++; }
						if ( ((RuneBox)item).HasHonor > 0 ){ runes++; }
						if ( ((RuneBox)item).HasHumility > 0 ){ runes++; }
						if ( ((RuneBox)item).HasJustice > 0 ){ runes++; }
						if ( ((RuneBox)item).HasSacrifice > 0 ){ runes++; }
						if ( ((RuneBox)item).HasSpirituality > 0 ){ runes++; }
						if ( ((RuneBox)item).HasValor > 0 ){ runes++; }

						quests = quests + "-You currently have " + runes + " out of 8 runes of virtue.<br><br>";
					}
				}
				else if ( item is SearchPage )
				{
					if ( ((SearchPage)item).Owner == from )
					{
						quests = quests + "-You are on a quest to obtain the " + ((SearchPage)item).SearchItem + ".<br><br>";
					}
				}
				else if ( item is VortexCube )
				{
					if ( ((VortexCube)item).CubeOwner == from )
					{
						VortexCube cube = (VortexCube)item;
						quests = quests + "-You are searching for the Codex of Ultimate Wisdom.<br>";

						if ( cube.HasConvexLense > 0 ){ quests = quests + "   -You have the Convex Lense.<br>"; }
						if ( cube.HasConcaveLense > 0 ){ quests = quests + "   -You have the Concave Lense.<br>"; }

						if ( cube.HasKeyLaw > 0 ){ quests = quests + "   -You have the Key of Law.<br>"; }
						if ( cube.HasKeyBalance > 0 ){ quests = quests + "   -You have the Key of Balance.<br>"; }
						if ( cube.HasKeyChaos > 0 ){ quests = quests + "   -You have the Key of Chaos.<br>"; }

						if ( cube.HasCrystalRed > 0 ){ quests = quests + "   -You have the Red Void Crystal.<br>"; }
						if ( cube.HasCrystalBlue > 0 ){ quests = quests + "   -You have the Blue Void Crystal.<br>"; }
						if ( cube.HasCrystalGreen > 0 ){ quests = quests + "   -You have the Green Void Crystal.<br>"; }
						if ( cube.HasCrystalYellow > 0 ){ quests = quests + "   -You have the Yellow Void Crystal.<br>"; }
						if ( cube.HasCrystalWhite > 0 ){ quests = quests + "   -You have the White Void Crystal.<br>"; }
						if ( cube.HasCrystalPurple > 0 ){ quests = quests + "   -You have the Purple Void Crystal.<br>"; }

						quests = quests + "<br>";
					}
				}
				else if ( item is ObeliskTip )
				{
					if ( ((ObeliskTip)item).ObeliskOwner == from )
					{
						ObeliskTip obelisk = (ObeliskTip)item;
						quests = quests + "-You are trying to become a Titan of Ether.<br>";
						quests = quests + "   -You have the Obelisk Tip.<br>"; 

						if ( obelisk.WonAir > 0 ){ quests = quests + "   -You have defeated Stratos, the Titan of Air.<br>"; }
						else if ( obelisk.HasAir > 0 ){ quests = quests + "   -You have the Breath of Air.<br>"; }
						if ( obelisk.WonFire > 0 ){ quests = quests + "   -You have defeated Pyros, the Titan of Fire.<br>"; }
						else if ( obelisk.HasFire > 0 ){ quests = quests + "   -You have the Tongue of Flame.<br>"; }
						if ( obelisk.WonEarth > 0 ){ quests = quests + "   -You have defeated Lithos, the Titan of Earth.<br>"; }
						else if ( obelisk.HasEarth > 0 ){ quests = quests + "   -You have the Heart of Earth.<br>"; }
						if ( obelisk.WonWater > 0 ){ quests = quests + "   -You have defeated Hydros, the Titan of Water.<br>"; }
						else if ( obelisk.HasWater > 0 ){ quests = quests + "   -You have the Tear of the Seas.<br>"; }

						quests = quests + "<br>";
					}
				}
				else if ( item is MuseumBook )
				{
					if ( ((MuseumBook)item).ArtOwner == from )
					{
						quests = quests + "-You have found " + MuseumBook.GetTotal( (MuseumBook)item ) + " out of 60 antiques for the museum.<br><br>";
					}
				}
				else if ( item is RuneBox )
				{
					if ( ((RuneBox)item).RuneBoxOwner == from )
					{
						int runes = ((RuneBox)item).HasCompassion + ((RuneBox)item).HasHonesty + ((RuneBox)item).HasHonor + ((RuneBox)item).HasHumility + ((RuneBox)item).HasJustice + ((RuneBox)item).HasSacrifice + ((RuneBox)item).HasSpirituality + ((RuneBox)item).HasValor;
						quests = quests + "-You have found " + runes + " out of 8 runes of virtue.<br><br>";
					}
				}
				else if ( item is QuestTome )
				{
					if ( ((QuestTome)item).QuestTomeOwner == from )
					{
						quests = quests + "-You are on a quest to find " + ((QuestTome)item).GoalItem4 + ".<br><br>";
					}
				}
			}

			if ( 	from.Backpack.FindItemByType( typeof ( ScalesOfEthicality ) ) != null || 
					from.Backpack.FindItemByType( typeof ( OrbOfLogic ) ) != null || 
					from.Backpack.FindItemByType( typeof ( LanternOfDiscipline ) ) != null || 
					from.Backpack.FindItemByType( typeof ( BlackrockSerpentOrder ) ) != null || 
					from.Backpack.FindItemByType( typeof ( BlackrockSerpentChaos ) ) != null || 
					from.Backpack.FindItemByType( typeof ( BlackrockSerpentBalance ) ) != null )
			{
				quests = quests + "-You are on a quest to bring the Serpents back into balance.<br><br>";
			}

			if ( 	from.Backpack.FindItemByType( typeof ( ShardOfFalsehood ) ) != null || 
					from.Backpack.FindItemByType( typeof ( ShardOfCowardice ) ) != null || 
					from.Backpack.FindItemByType( typeof ( ShardOfHatred ) ) != null || 
					from.Backpack.FindItemByType( typeof ( CandleOfLove ) ) != null || 
					from.Backpack.FindItemByType( typeof ( BookOfTruth ) ) != null || 
					from.Backpack.FindItemByType( typeof ( BellOfCourage ) ) != null )
			{
				quests = quests + "-You are on a quest to destroy the Shadowlords and construct a Gem of Immortality.<br><br>";
			}
			else if ( from.Backpack.FindItemByType( typeof ( GemImmortality ) ) != null )
			{
				quests = quests + "-You have constructed a Gem of Immortality.<br><br>";
			}

			if ( PlayerSettings.GetKeys( from, "Museums" ) )
			{
				quests = quests + "-You have found all of the antiques for the Museum.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Gygax" ) )
			{
				quests = quests + "-You have obtained the Statue of Gygax.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Virtues" ) )
			{
				quests = quests + "-You have cleansed all of the Runes of Virtue.<br><br>";
			}
			else if ( PlayerSettings.GetKeys( from, "Corrupt" ) )
			{
				quests = quests + "-You have corrupted all of the Runes of Virtue.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Exodus" ) )
			{
				quests = quests + "-You have destroyed the Core of Exodus.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "BlackGateDemon" ) )
			{
				quests = quests + "-You have defeated the Black Gate Demon and found a portal to the Ethereal Plane.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Jormungandr" ) )
			{
				quests = quests + "-You have defeated the legendary serpent known as Jormungandr.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Dracula" ) )
			{
				quests = quests + "-You have destroyed Dracula, the ruler of all vampires.<br><br>";
			}
			if ( 	from.Backpack.FindItemByType( typeof ( StaffPartVenom ) ) != null || 
					from.Backpack.FindItemByType( typeof ( StaffPartCaddellite ) ) != null || 
					from.Backpack.FindItemByType( typeof ( StaffPartFire ) ) != null || 
					from.Backpack.FindItemByType( typeof ( StaffPartLight ) ) != null || 
					from.Backpack.FindItemByType( typeof ( StaffPartEnergy ) ) != null )
			{
				quests = quests + "-You are seeking to assemble the Staff of Ultimate Power.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Arachnar" ) )
			{
				quests = quests + "-You have defeated Arachnar, the guardian of the staff.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Surtaz" ) )
			{
				quests = quests + "-You have defeated Surtaz, the guardian of the staff.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Vordinax" ) )
			{
				quests = quests + "-You have defeated Vordinax, the guardian of the staff.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Vulcrum" ) )
			{
				quests = quests + "-You have defeated Vulcrum, the guardian of the staff.<br><br>";
			}
			if ( PlayerSettings.GetKeys( from, "Xurtzar" ) )
			{
				quests = quests + "-You have defeated Xurtzar, the guardian of the staff.<br><br>";
			}

			return quests;
		}

		public static string MyHelp()
        {
			string HelpText = "If you are looking for help exploring this world, you can learn about almost anything within the game world you travel. Some merchants sell scrolls or books that will explain how some skills can be performed, resources gathered, and even how elements of the world can be manipulated. A sage often sells many tomes of useful information on skills, weapon abilities, or various types of magics available. If you are totally new to this game, buy yourself a Guide to Adventure book from a sage if you lost the one you started with. This book explains how to navigate and play the game. You will also learn some things about how the world behaves such as merchant interactions, how to use items, and what to do when your character dies. Talk to the townsfolk to learn whatever you can. On this screen there are many options, information, and settings that can assist in your journey. Many of the options here have keyboard commands that are listed below. Make sure to check out the 'Info' section on your character's paperdoll as it has some vital information about your character.<br><br>"
				+ "Common Commands: Below are the commands you can use for various things in the game.<br><br>"
				+ "[abilitynames - Turns on/off the special weapon ability names next to the appropriate icons.<br><br>"
				+ "[afk - Turns on/off the notification to others that you are away from keyboard.<br><br>"
				+ "[ancient - Turns on/off whether you are using magic from the research bag or the ancient spellbook.<br><br>"
				+ "[autoattack - Turns on/off whether you auto attack when attacked.<br><br>"
				+ "[bandother - Bandage other command.<br><br>"
				+ "[bandself - Bandage self command.<br><br>"
				+ "[barbaric - Turns on/off the barbaric flavor the game provides (see end).<br><br>"
				+ "[c - Initiates the chat system.<br><br>"
				+ "[corpse - Helps one find their remains.<br><br>"
				+ "[corpseclear - Removes your corpse from a ship's deck.<br><br>"
				+ "[e - Opens the emote mini window.<br><br>"
				+ "[emote - Opens the emote window.<br><br>"
				+ "[evil - Turns on/off the evil flavor the game provides (see end).<br><br>"
				+ "[feats - Opens the Achievements window.<br><br>"
				+ "[loot - Automatically take certain items from common dungeon chests or corpses and put them in your backpack. The unknown items are those that will need identification, but you may decide to take them anyway. The reagent options have a few categories. Magery and necromancer reagents are those used specifically by those characters, where witches brew reagents mainly fall into the necromancer category. Alchemic reagents are those that fall outside the category of magery and necromancer reagents, and only alchemists use them. Herbalist reagents are useful in druidic herbalism.<br><br>"
				+ "[magicgate - Helps one find the nearest magical gate.<br><br>"
				+ "[motd - Opens the message of the day.<br><br>"
				+ "[oriental - Turns on/off the oriental flavor the game provides (see end).<br><br>"
				+ "[password - Change your account password.<br><br>"
				+ "[poisons - This changes how poisoned weapons work, which can be for either precise control with special weapon infectious strikes (default) or with hits of a one-handed slashing or piercing weapon.<br><br>"
				+ "[private - Turns on/off detailed messages of your journey for the town crier and local citizen chatter.<br><br>"
				+ "[quests - Opens a scroll to show certain quest events.<br><br>"
				+ "[quickbar - Opens a small, vertical bar with common game functions for easier use.<br><br>"
				+ "[sad - Opens the weapon's special abilities.<br><br>"
				+ "[set1 - Sets your weapon's first ability to active.<br>"
				+ "[set2 - Sets your weapon's second ability to active.<br>"
				+ "[set3 - Sets your weapon's third ability to active.<br>"
				+ "[set4 - Sets your weapon's fourth ability to active.<br>"
				+ "[set5 - Sets your weapon's fifth ability to active.<br><br>"
				+ "[sheathe - Turns on/off the feature to sheathe your weapon when not in battle.<br><br>"
				+ "[skill - Shows you what each skill is used for.<br><br>"
				+ "[skilllist - Displays a more condensed list of skills you have set to 'up' and perhaps 'locked'.<br><br>"
				+ "[spellhue ## - This command, following by a color reference hue number, will change all of your magery spell effects to that color. A value of '1' will normally render as '0' so avoid that setting as it will not produce the result you may want.<br><br>"
				+ "[statistics - Shows you some statistics of the server.<br><br>"
				+ "[wealth - Opens a small, horizontal bar showing your gold value for the various forms of currency and gold in your bank and backpack. Currency are items you would have a banker convert to gold for you (silver, copper, xormite, jewels, and crystals). If you put these items in your bank, you can update the values on the wealth bar by right clicking on it.<br><br>"

				+ "<br><br>"

				+ "Area Difficulty Levels: When you enter many dangerous areas, there will be a message to you that you entered a particular area. There may be a level of difficulty shown in parenthesis, that will give you an indication on the difficulty of the area. Below are the descriptions for each level.<br><br>"
				+ " - Easy (Not much of a challenge)<br>"
				+ " - Normal (An average level of challenge)<br>"
				+ " - Difficult (A tad more difficult)<br>"
				+ " - Challenging (You will probably run away alot)<br>"
				+ " - Hard (You will probably die alot)<br>"
				+ " - Deadly (I dare you)<br>"
				// + " - Epic (For Titans of Ether)<br>"

				+ "<br><br>"

				+ "Skill Titles: You can set your default title for your character. Although you may be a Grandmaster Driven, you may want your title to reflect your Apprentice Wizard title instead. This is how you set it...<br><br>"
				+ "Type the '[SkillName' command followed by the name of the skill you want to set as your default. Make sure you surround the skill name in quotes and all lowercase. Example...<br>"
				+ "  [SkillName \"taming\"<br><br>"
				+ "If you want the game to manage your character's title, simply use the same command with a skill name of \"clear\".<br><br>"

				+ "<br><br>"

				+ "Reagent Bars: Below are the commands you can use to watch your reagent quantities as you cast spells or create potions. These are customizable bars that will show the quantities of the reagents you are carrying. These will show updated quantities of reagents whenever you cast a spell or make a potion that uses them. Otherwise you can make a macro to these commands and use them to refresh the amounts manually.<br><br>"
				+ "[regbar - Opens the reagent bar.<br><br>"
				+ "[regclose - Closes the reagent bar.<br><br>"

				+ "<br><br>"

				+ "Music: There is many different pieces of classic music in the game, and they play depending on areas you visit. Some of the music is from the original game, but there are some pieces from older games. There are also some pieces from computer games in the 1990's, but they really fit the theme when traveling the land. You can choose to listen to them, or change the music you are listening to when exploring the world. Keep in mind that when you change the music, and you enter a new area, the default music for that area will play and you may have to change your music again. Also keep in mind that your game client will want to play the song for a few seconds before allowing a switch of new music. You can use the below command to open a window that allows you to choose a song to play. Almost all of them play in a loop, where there are three that do not and are marked with an asterisk. There are two pages of songs to choose from so use the top arrow to go back and forth to each screen. When your music begins to play, then press the OKAY button to exit the screen. Although an unnecessary function, it does give you some control over the music in the game.<br><br>"
				+ "[music - Opens the music playlist and player.<br><br>"
				+ "The below command will simply toggle your music preference to play a different set of music in the dungeons. When turned on, it will play music you normally hear when traveling the land, instead of the music commonly played in dungeons.<br><br>"
				+ "[musical - Sets the default dungeon music.<br><br>"

				+ "<br><br>"

				+ "Evil Style: There is an evil element to the game that some want to participate in. With classes such as Necromancers, some players may want to travel a world with this flavor added. This particular setting allows you to toggle between regular and evil flavors. When in the evil mode, some of the treasure you will find will often have a name that fits in the evil style. When you stay within negative karma, skill titles will change for you as well, but not all. Look over the book of skill titles (found within the game world) to see which titles will change based on karma. Some of the relics you will find may also have this style, to perhaps decorate a home in this fashion. This option can be turned off and on at any time. You can only have one type of play style active at any one time.<br><br>"
				+ "[evil - Turns on/off the evil flavor the game provides.<br><br>"

				+ "<br><br>"

				+ "Oriental Style: There is an oriental element to the game that most do not want to participate in. With classes such as Ninja and Samurai, some players may want to travel a world with this flavor added. This particular setting allows you to toggle between fantasy and oriental. When in the oriental mode, half of the treasure you will find will be of Chinese or Japanese historical origins. These types of items will most times be named to match the style. Items that once belonged to someone, will often have a name that fits in the oriental style. Some of the skill titles will change for you as well, but not all. Look over the book of skill titles (found within the game world) to see which titles will change based on this play style. Some of the relics and artwork you will find will also have this style, to perhaps decorate a home in this fashion. This option can be turned off and on at any time. You can only have one type of play style active at any one time.<br><br>"
				+ "[oriental - Turns on/off the oriental flavor the game provides.<br><br>"

				+ "<br><br>"

				+ "Barbaric Style: The default game does not lend itself to a sword and sorcery experience. This means that it is not the most optimal play experience to be a loin cloth wearing barbarian that roams the land with a huge axe. Characters generally get as much equipment as they can in order to maximize their rate of survivability. This particular play style can help in this regard. Choosing to play in this style will have a satchel appear in your main pack. You cannot store anything in this satchel, as its purpose is to change certain pieces of equipment you place into it. It will change shields, hats, helms, tunics, sleeves, leggings, boots, gorgets, gloves, necklaces, cloaks, and robes. When these items get changed, they will become something that appears differently but behave in the same way the previous item did. These different items can be equipped but may not appear on your character. Also note that when you wear robes, they cover your character's tunics and sleeves. Wearing a sword and sorcery robe will do the same thing so you will have to remove the robe in order to get to the sleeves and/or tunic. This play style has their own set of skill titles for many skills as well. If you are playing a female character, pressing the button further will convert any 'Barbarian' titles to 'Amazon'. You can open your satchel to learn more about this play style. This option can be turned off and on at any time. You can only have one type of play style active at any one time.<br><br>"
				+ "[barbaric - Turns on/off the barbaric flavor the game provides.<br><br>"

			+ "";

			return HelpText;
		}

		private bool ShowHelpInfoWindow( Mobile from, PageActionType page )
		{
			bool scrollbar = true;
			string title;
			string info;
			PageActionType returnPage = PageActionType.Show_Settings;

			switch ( page )
			{
				case PageActionType.Setting_MusicTone_Info:
				{
					scrollbar = false;
					title = "Music Tone";
					info = "This option will simply toggle your music preference to play a different set of music in the dungeons. When turned on, it will play music you normally hear when traveling the land, instead of the music commonly played in dungeons.";
					break;
				}

				case PageActionType.Setting_MusicPlaylist_Info:
				{
					title = "Music Playlist";
					info = "This gives you a complete list of the in-game music. You can select the music you like and those choices will randomly play as you go from region to region. To listen to a song for review, select the blue gem icon. Note that the client has a delay time when you can start another song so selecting the blue gem may not respond if you started a song too soon before that. Wait for a few seconds and try clicking the blue gem again to see if that song starts to play. Playlists are disabled by default, so if you want your playlist to function, make sure to enable it.";
					break;
				}

				case PageActionType.Setting_PrivatePlay_Info:
				{
					scrollbar = false;
					title = "Private Play";
					info = "This option turns on or off the detailed messages of your journey for the town crier and local citizen chatter. This keeps your activities private so others will not see where you are traveling the world.";
					break;
				}

				case PageActionType.Setting_LootOptions_Info:
				{
					title = "Loot Options";
					info = "This lets you select from a list of categories, where they will automatically take those types of items from common dungeon chests or corpses and put them in your backpack. If you select coins, you will take wealth in the form of currency or gold nuggets. If you take gems and jewels, this will consist of gems, gemstones, jewelry, jewels, and crystals. The unknown items are those that will need identification, but you may decide to take them anyway. The reagent options have a few categories. Magery and necromancer reagents are those used specifically by those characters, where witches brew reagents fall into the necromancer category. Alchemic reagents are those that fall outside the category of magery and necromancer reagents, and only alchemists use them. Herbalist reagents are useful druidic herbalism.";
					break;
				}

				case PageActionType.Setting_ClassicPoisoning_Info:
				{
					title = "Classic Poisoning";
					info = "There are two methods that assassins use to handle poisoned weapons. One is the simple method of soaking the blade and having it poison whenever it strikes their opponent. With this method, known as classic poisoning, there is little control on the dosage given but it is easier to maneuver. When this option is turned off, it has the newer and more tactical method, where only certain weapons can be poisoned and the assassin can control when the poison is administered with the hit. Although the tactical method requires more thought, it does have the potential to allow an assassin to poison certain arrows, for example. The choice of methods can be switched at any time, but only one method can be in use at a given time.";
					break;
				}

				case PageActionType.Setting_SkillTitle_Info:
				{
					title = "Skill Title";
					info = "When you don't set your skill title here, the game will take your highest skill and make that into your character's title. Choosing a skill here will force your title to that profession. So if you always want to be known as a wizard, then select the 'Magery' option (for example). You can let the game manage this at any time by setting it back to 'Auto Title'. Be warned when choosing a skill, if you have zero skill points in it, you will be titled 'the Village Idiot'. If you get at least 0.1, you will at least be 'Aspiring'.";
					break;
				}

				case PageActionType.Setting_MessageColors_Info:
				{
					scrollbar = false;
					title = "Message Color";
					info = "By default, most of the messages appearing on the lower left of the screen are gray in color. Enabling this option will change those messages to have a random color whenenver a new message appears. This feature can help some more easily see such messages and the varying colors can also help distinguish individual messages that may be scrolling by.";
					break;
				}

				case PageActionType.Setting_AutoAttack_Info:
				{
					scrollbar = false;
					title = "Auto Attack";
					info = "By default, when you are attacked you will automatically attack back. If you want to instead decide when or if you want to attack back, you can turn this option off. This can be helpful if you do not want to kill innocents by accident, or you are trying to tame an angry creature.";
					break;
				}

				case PageActionType.Setting_Playstyle_Normal_Info:
				{
					title = "Play Style - Normal";
					info = "This is the default play style for the " + MySettings.S_ServerName + ". It is designed for a classic fantasy world experience for the players. There are two other play styles available, evil and oriental. Play styles do not change the mechanics of the game playing experience, but it does change the flavor of the treasure you find and the henchman you hire. For example, you can set your play style to an 'evil' style of play. What happens is you will find treasure geared toward that play style. Where you would normally find a blue 'mace of might', the evil style would have you find a black 'mace of ghostly death'. They are simply a way to tweak your character's experience in the game.";
					break;
				}

				case PageActionType.Setting_Playstyle_Evil_Info:
				{
					title = "Play Style - Evil";
					info = "There is an evil element to the game that some want to participate in. With classes such as Necromancers, some players may want to travel a world with this flavor added. This particular setting allows you to toggle between regular and evil flavors. When in the evil mode, some of the treasure you will find will often have a name that fits in the evil style. When you stay within negative karma, skill titles will change for you as well, but not all. Look over the book of skill titles (found within the game world) to see which titles will change based on karma. Some of the relics you will find may also have this style, to perhaps decorate a home in this fashion. This option can be turned off and on at any time. You can only have one type of play style active at any one time.<br><br>"
						+ "[evil - Turns on/off the evil flavor the game provides.";
					break;
				}

				case PageActionType.Setting_Playstyle_Oriental_Info:
				{
					title = "Play Style - Oriental";
					info = "There is an oriental element to the game that most do not want to participate in. With classes such as Ninja and Samurai, some players may want to travel a world with this flavor added. This particular setting allows you to toggle between fantasy and oriental. When in the oriental mode, much of the treasure you will find will be of Chinese or Japanese historical origins. These types of items will most times be named to match the style. Items that once belonged to someone, will often have a name that fits in the oriental style. Some of the skill titles will change for you as well, but not all. Look over the book of skill titles (found within the game world) to see which titles will change based on this play style. Some of the relics and artwork you will find will also have this style, to perhaps decorate a home in this fashion. This option can be turned off and on at any time. You can only have one type of play style active at any one time.";
					break;
				}

				case PageActionType.ShowHelp_MagicToolbars:
				{
					returnPage = PageActionType.Navigate_MagicToolbars;
					title = "Magic Toolbars";
					var builder = new StringBuilder();
					builder.Append("These toolbars can be configured for all areas of magical-style spells in the game. Each school of magic has two separate toolbars you can customize, except for magery which has four available. The large number of spells for magery benefit from the extra two toolbars. These toolbars allow you to select spells that you like to cast often, and set whether the bar will appear vertical or horizontal. If you choose to have the toolbar appear vertical, you have the additional option of showing the spell names next to the icons. These toolbars can be moved around and you need only single click the appropriate icon to cast the spell. If you have spells selected for a toolbar, but lack the spell in your spellbook, the icon will not appear when you open the toolbar. These toolbars cannot be closed by normal means, to avoid the chance you close them by accident when in combat. You can either use the command button available in the 'Help' section, or the appropriate typed keyboard command.");
					builder.Append("<br><br>");

					builder.Append("Ancient Spell Bars may be opened with:<br>");
					builder.Append("[archtool1<br>");
					builder.Append("[archtool2<br>");
					builder.Append("[archtool3<br>");
					builder.Append("[archtool4<br>");
					builder.Append("Ancient Spell Bars may be closed with:<br>");
					builder.Append("[archclose1<br>");
					builder.Append("[archclose2<br>");
					builder.Append("[archclose3<br>");
					builder.Append("[archclose4<br>");
					builder.Append("<br>");

					builder.Append("Bard Song Bars may be opened with:<br>");
					builder.Append("[bardtool1<br>");
					builder.Append("[bardtool2<br>");
					builder.Append("Bard Song Bars may be closed with:<br>");
					builder.Append("[bardclose1<br>");
					builder.Append("[bardclose2<br>");
					builder.Append("<br>");

					builder.Append("Death Knight Spell Bars may be opened with:<br>");
					builder.Append("[deathtool1<br>");
					builder.Append("[deathtool2<br>");
					builder.Append("Death Knight Spell Bars may be closed with:<br>");
					builder.Append("[deathclose1<br>");
					builder.Append("[deathclose2<br>");
					builder.Append("<br>");

					builder.Append("Elemental Spell Bars may be opened with:<br>");
					builder.Append("[elementtool1<br>");
					builder.Append("[elementtool2<br>");
					builder.Append("Elemental Spell Bars may be closed with:<br>");
					builder.Append("[elementclose1<br>");
					builder.Append("[elementclose2<br>");
					builder.Append("<br>");

					builder.Append("Priest Prayer Bars may be opened with:<br>");
					builder.Append("[holytool1<br>");
					builder.Append("[holytool2<br>");
					builder.Append("Priest Prayer Bars may be closed with:<br>");
					builder.Append("[holyclose1<br>");
					builder.Append("[holyclose2<br>");
					builder.Append("<br>");

					builder.Append("Knight Spell Bars may be opened with:<br>");
					builder.Append("[knighttool1<br>");
					builder.Append("[knighttool2<br>");
					builder.Append("Knight Spell Bars may be closed with:<br>");
					builder.Append("[knightclose1<br>");
					builder.Append("[knightclose2<br>");
					builder.Append("<br>");

					builder.Append("Mage Spell Bars may be opened with:<br>");
					builder.Append("[magetool1<br>");
					builder.Append("[magetool2<br>");
					builder.Append("[magetool3<br>");
					builder.Append("[magetool4<br>");
					builder.Append("Mage Spell Bars may be closed with:<br>");
					builder.Append("[mageclose1<br>");
					builder.Append("[mageclose2<br>");
					builder.Append("[mageclose3<br>");
					builder.Append("[mageclose4<br>");
					builder.Append("<br>");

					builder.Append("Monk Ability Bars may be opened with:<br>");
					builder.Append("[monktool1<br>");
					builder.Append("[monktool2<br>");
					builder.Append("Monk Ability Bars may be closed with:<br>");
					builder.Append("[monkclose1<br>");
					builder.Append("[monkclose2<br>");
					builder.Append("<br>");

					builder.Append("Necromancer Spell Bars may be opened with:<br>");
					builder.Append("[necrotool1<br>");
					builder.Append("[necrotool2<br>");
					builder.Append("Necromancer Spell Bars may be closed with:<br>");
					builder.Append("[necroclose1<br>");
					builder.Append("[necroclose2<br>");
					info = builder.ToString();
					break;
				}

				case PageActionType.Setting_MagerySpellColor_Info:
				{
					scrollbar = false;
					title = "Magery Spell Color";
					info = "You can change the color for all of your magery spell effects here. There are a limited amount of choices given here. Once set, your spells will be that color for every effect. If you want to set it back to normal, then select the 'Default' option. You can also use the '[spellhue' command followed by a number of any color you want to set it to.";
					break;
				}

				case PageActionType.Setting_CustomTitle_Info:
				{
					scrollbar = false;
					title = "Custom Title";
					info = "This allows you to enter a custom title for your character, instead of relying on the game to assign you one based on your best skill or the skill you choose to have represent you. To clear out a custom title you may have set with this option, enter the word of 'clear' to remove it.";
					break;
				}

				case PageActionType.Setting_WeaponAbilityNames_Info:
				{
					scrollbar = false;
					title = "Weapon Ability Names";
					info = "When you get good enough with tactics and a weapon type, you will get special abilities that they can perform. These usually appear as simple icons you can select to do the action, but this option will turn on or off the special weapon ability names next to the appropriate icons.";
					break;
				}

				case PageActionType.Setting_AutoSheath_Info:
				{
					scrollbar = false;
					title = "Auto Sheath";
					info = "This option turns on or off the feature to sheathe your weapon when not in battle. When you put your character back into war mode, they will draw the weapon.";
					break;
				}

				case PageActionType.Setting_GumpImages_Info:
				{
					scrollbar = false;
					title = "Gump Images";
					info = "Many window gumps have a faded image in the background. Turning this off will have those windows only be black in color, with no background image.";
					break;
				}

				case PageActionType.Setting_WeaponAbilityBar_Info:
				{
					scrollbar = false;
					title = "Weapon Ability Bar";
					info = "This option turns on or off the auto-opening of the weapon ability icon bar, meaning you will have to do it manually if you turn it off.";
					break;
				}

				case PageActionType.Setting_CreatureMagicFocus_Info:
				{
					scrollbar = false;
					title = "Creature Magic";
					info = "You must be in the Tavern to change this setting.<br><br>Some creatures have a natural ability for magic. This setting lets you change which school of magic you want to focus on: magery, necromancy, or elementalism. This allows magery or necromancy creatures to move their focus into elementalism, or to switch between magery and necromancy.";
					break;
				}

				case PageActionType.Setting_CreatureType_Info:
				{
					scrollbar = false;
					title = "Creature Type";
					info = "Some creature species has more than one option for appearance. This setting lets you change to another of that species if another appearance is available. You can also turn yourself into a human if you choose. If you become human, you will remain that way forever.";
					break;
				}

				case PageActionType.Setting_CreatureSounds_Info:
				{
					scrollbar = false;
					title = "Creature Sounds";
					info = "Since you are a creature, you sometimes make sounds when attacking or getting hurt from attacks. You can turn these sounds on or off here.";
					break;
				}

				case PageActionType.Setting_UseAncientSpellbook_Info:
				{
					scrollbar = false;
					title = "Ancient Spellbook";
					info = "If you begin researching the 64 ancient spells that were long forgotten, enabling this setting means you will be casting such magic from a book instead of using your research bag. If you have this enabled, you will need reagents to cast spells and the spells being cast must be in your book. Disabling this checks your research bag to see if you have the spell prepared ahead of time.";
					break;
				}

				case PageActionType.Setting_SetCraftingContainer_Info:
				{
					scrollbar = false;
					title = "Set Crafting Container";
					info = "This allows you to set a container, where items will go when you are creating them through crafting. The container must be in your main pack in order to collect the items, and not within another container.";
					break;
				}

				case PageActionType.Setting_SetHarvestingContainer_Info:
				{
					scrollbar = false;
					title = "Set Harvesting Container";
					info = "This allows you to set a container, where items will go when you are harvesting for items. These are items you get from activities like mining, lumberjacking, and fishing. The container must be in your main pack in order to collect the items, and not within another container.";
					break;
				}

				case PageActionType.Setting_SetLootContainer_Info:
				{
					scrollbar = false;
					title = "Set Loot Container";
					info = "This allows you to set a container, where items will go that you configured in the Loot Options setting. The container must be in your main pack in order to collect the items, and not within another container.";
					break;
				}

				case PageActionType.Setting_OrdinaryResources_Info:
				{
					scrollbar = false;
					title = "Ordinary Resources";
					info = "Turning this setting on will have your character only harvest or gather ordinary resources like wood, leather, granite, iron and bones. This means you will not be collecting higher resourced items when skinning, mining, or lumberjacking.";
					break;
				}

				case PageActionType.Setting_DoubleClickToIDItems_Info:
				{
					scrollbar = false;
					title = "Double Click to ID Items";
					info = "Enabling this will allow your character to try and identify items by double clicking them.<BR><BR>NOTE: if you are using any third party software, that tries to open all of your containers, then that third party software will try to identify these items without your consent.";
					break;
				}

				case PageActionType.Setting_DoubleClickToTalk_Info:
				{
					scrollbar = false;
					title = "Double Click Talk";
					info = "When enabled, double clicking a citizen or other talkative NPC (the ones with a 'Talk' option in their context menu - found at meeting spots, town guards, innkeepers, couriers, and epic characters) will open their conversation directly instead of opening their paperdoll. NPCs without anything to say still show their paperdoll as normal.";
					break;
				}

				case PageActionType.Setting_Playstyle_Barbaric_Info:
				{
					title = "Play Style - Barbaric";
					info = "The default game does not lend itself to a sword and sorcery experience. This means that it is not the most optimal play experience to be a loin cloth wearing barbarian that roams the land with a huge axe. Characters generally get as much equipment as they can in order to maximize their rate of survivability. This particular play style can help in this regard. Choosing to play in this style will have a satchel appear in your main pack. You cannot store anything in this satchel, as its purpose is to change certain pieces of equipment you place into it. It will change shields, hats, helms, tunics, sleeves, leggings, boots, gorgets, gloves, necklaces, cloaks, and robes. When these items get changed, they will become something that appears differently but behave in the same way the previous item did. These different items can be equipped but may not appear on your character. Also note that when you wear robes, they cover your character's tunics and sleeves. Wearing a sword and sorcery robe will do the same thing so you will have to remove the robe in order to get to the sleeves and/or tunic. This play style has their own set of skill titles for many skills as well. If you are playing a female character, pressing the button further will convert any 'Barbarian' titles to 'Amazon'. You can open your satchel to learn more about this play style. This option can be turned off and on at any time. You can only have one type of play style active at any one time.";
					break;
				}

				case PageActionType.Setting_SkillList_Info:
				{
					title = "Skill Lists";
					info = "Skill lists are an alternative to the normal skill lists you can get from clicking the appropriate button on the paper doll. Although you still need to use that for skill management (up, down, lock), skill lists have a more condensed appearance for when you play the game. In order for skills to appear in this alternate list, they have to either be set to 'up', or they can be set to 'locked'. The 'locked' skills will only display in this list if you change your settings here to reflect that. The list does not refresh in real time, but it will often refresh itself to show your skill status in both real and enhanced values. Any skill that appears in orange indicates a skill that you have locked. You can open this list with the '[skilllist' command, or the appropriate button on the main screen.";
					break;
				}

				case PageActionType.Setting_RemoveVendorGoldSafeguard_Info:
				{
					title = "Remove Vendor Gold Safeguard";
					info = "Command: [VendorGold<br><br>When enabled, vendors will no longer stop sales if they cannot afford it. Instead, vendors will take all the items and give you their remaining gold.";
					break;
				}
				
				case PageActionType.Setting_SuppressVendorTooltips_Info:
				{
					title = "Suppress Vendor Tooltips";
					info = "Command: [SuppressTooltips<br><br>When enabled, vendor tooltips will not be sent to the client. This can be helpful for players who use a touch screen. Warning: Players usually have to re-log to re-query synchronize with this after changing it.";
					break;
				}

				case PageActionType.Setting_VendorContainerSell_Info:
				{
					title = "Container Sell";
					info = "Configure the gump shown when you drop a container on a vendor to sell its contents. If the new gump is disabled, sellable items from the container are sent through the classic vendor sell list instead.";
					break;
				}

				case PageActionType.Setting_ModernSkills_Info:
				{
					title = "Modern Skill Settings";
					info = "Configure the gump shown to configure modernization of skills.";
					break;
				}

				case PageActionType.Setting_SingleAttemptID_Info:
				{
					title = "Single Attempt ID";
					info = "When enabled, a single attempt to identify an item will use all available attempts.";
					break;
				}

				case PageActionType.Setting_ColorlessFabricBreakdown_Info:
				{
					title = "Colorless Fabric Breakdown";
					info = "When enabled, the fabric color will be stripped when it is broken down.";
					break;
				}

				case PageActionType.Setting_LegacyCarve_Info:
				{
					title = "Legacy Carve";
					info = "When enabled, carving humanoid corpses will turn them into bones. Warning: Enabling this setting may cause an awkward looting experience after carving.";
					break;
				}

				default:
					return false;
			}
			
			from.SendGump( new InfoHelpGump( from, title, info, scrollbar, () => from.SendGump( new HelpGump( from, (int)returnPage ) ) ) );

			return true;
		}
	}
}
