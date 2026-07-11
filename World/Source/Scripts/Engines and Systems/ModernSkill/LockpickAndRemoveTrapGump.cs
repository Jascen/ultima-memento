using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.SkillHandlers;
using Server.Timers;
using Server.Utilities;
using System;

namespace Server.ModernSkill
{
	public class LockpickAndRemoveTrapGump : Gump
	{
		private enum PageActions
		{
			None = 0,
			DoLockpick,
			DoRemoveTrap,
			DoDoubleclick,
			ToggleRetryLockpicking,
			ToggleRetryRemoveTrap,
		}

		private readonly bool m_ShowLockpickState;
		private readonly bool m_ShowTrapState;
		private readonly Item m_Target;

		public LockpickAndRemoveTrapGump(PlayerMobile player, Item target, bool showLockpickState, bool showTrapState) : base(100, 100)
		{
			m_Target = target;
			m_ShowLockpickState = showLockpickState;
			m_ShowTrapState = showTrapState;

			var gumpHeight = 120;
			if (IsPickable) gumpHeight += 80;
			if (IsTrappable) gumpHeight += 80;

			const int GUMP_WIDTH = 350;
			const int PADDING = 10;
			const int MAIN_WIDTH = GUMP_WIDTH - (4 * PADDING);
			const int START_X = 2 * PADDING;

			AddBackground(0, 0, GUMP_WIDTH, gumpHeight, 0x1453); // Tan box
			AddImageTiled(PADDING, PADDING, GUMP_WIDTH - (PADDING * 2), gumpHeight - (PADDING * 2), 2624); // Black box
			AddAlphaRegion(PADDING, PADDING, GUMP_WIDTH - (PADDING * 2), gumpHeight - (PADDING * 2));

			var currentX = START_X;
			var currentY = 20;

			TextDefinition.AddHtmlText(this, currentX, currentY, MAIN_WIDTH, 20, string.Format("<CENTER>{0}</CENTER>", target.Name), HtmlColors.MUSTARD);
			currentY += 20;

			const int BLACK_BOX_GOLD_TRIM = 5547;
			const int CHECKBOX_GRAPHIC_CHECKED = 4018;
			const int CHECKBOX_GRAPHIC_UNCHECKED = 3609;

			if (IsPickable)
			{
				currentY += 20;

				var showLockStatus = showLockpickState || 100 <= player.Skills[SkillName.Lockpicking].Value;
				if (showLockStatus && !Pickable.Locked && target is BaseContainer)
				{
					AddButton(currentX, currentY, BLACK_BOX_GOLD_TRIM, BLACK_BOX_GOLD_TRIM, (int)PageActions.DoDoubleclick, GumpButtonType.Reply, 0);
					TextDefinition.AddHtmlText(this, currentX + 2, currentY + 20, 60, 20, "<CENTER>Open</CENTER>", HtmlColors.MUSTARD);
				}
				else
				{
					const int LOCKPICKS_ITEM = 0x14FE;
					AddButton(currentX, currentY, BLACK_BOX_GOLD_TRIM, BLACK_BOX_GOLD_TRIM, (int)PageActions.DoLockpick, GumpButtonType.Reply, 0);
					GumpUtilities.AddCenteredItemToGump(this, LOCKPICKS_ITEM, currentX, currentY, 60, 60);
				}
				currentX += 60 + 10;

				currentY += 10;
				TextDefinition.AddHtmlText(this, currentX, currentY, MAIN_WIDTH, 20,
				string.Format("Lock Status: {0}", !showLockStatus
					? TextDefinition.GetColorizedText("???", HtmlColors.RED)
						: Pickable.Locked
						? TextDefinition.GetColorizedText("Locked", HtmlColors.RED)
						: TextDefinition.GetColorizedText("Unlocked", HtmlColors.COOL_GREEN)
				), HtmlColors.MUSTARD);

				currentY += 20;
				if (Pickable.Locked)
				{
					var difficultyText = Lockpick.GetDifficulty(player.Skills[SkillName.Lockpicking].Value, Pickable);
					TextDefinition.AddHtmlText(this, currentX, currentY, MAIN_WIDTH, 20, string.Format("Difficulty: {0}", difficultyText), HtmlColors.MUSTARD);
				}
			}

			if (IsTrappable)
			{
				if (IsPickable) currentY += 30;
				currentY += 20;

				var removeTrapSkill = player.Skills[SkillName.RemoveTrap].Value;
				var showTrapStatus = showTrapState || 100 <= removeTrapSkill;

				currentX = START_X;
				const int TRAPPING_TOOLS_ITEM = 0x1EBB;
				AddButton(currentX, currentY, BLACK_BOX_GOLD_TRIM, BLACK_BOX_GOLD_TRIM, (int)PageActions.DoRemoveTrap, GumpButtonType.Reply, 0);
				GumpUtilities.AddCenteredItemToGump(this, TRAPPING_TOOLS_ITEM, currentX, currentY, 60, 60);
				currentX += 60 + 10;

				currentY += 10;
				TextDefinition.AddHtmlText(this, currentX, currentY, MAIN_WIDTH, 20,
				string.Format("Trap Status: {0}", !showTrapStatus
					? TextDefinition.GetColorizedText("???", HtmlColors.RED)
					: Trap.IsActive
						? TextDefinition.GetColorizedText("Trapped", HtmlColors.RED)
						: TextDefinition.GetColorizedText("Safe", HtmlColors.COOL_GREEN)
				), HtmlColors.MUSTARD);

				currentY += 20;

				if (showTrapStatus && Trap.IsActive)
				{
					var difficultyText = RemoveTrap.GetDifficulty(removeTrapSkill, Trap);
					TextDefinition.AddHtmlText(this, currentX, currentY, MAIN_WIDTH, 20, string.Format("Difficulty: {0}", difficultyText), HtmlColors.MUSTARD);
				}
			}

			currentY += 30;

			if (IsPickable || IsTrappable)
			{
				currentY += 20;
				currentX = START_X;
				TextDefinition.AddHtmlText(this, currentX, currentY, 100, 20, "Auto-retry", HtmlColors.MUSTARD);
				currentY += 20;

				if (IsPickable)
				{
					int checkboxGraphic = player.Preferences.ModernLockpickingAutoRetryEnabled ? CHECKBOX_GRAPHIC_CHECKED : CHECKBOX_GRAPHIC_UNCHECKED;
					AddButton(currentX, currentY, checkboxGraphic, checkboxGraphic, (int)PageActions.ToggleRetryLockpicking, GumpButtonType.Reply, 0);
					TextDefinition.AddHtmlText(this, currentX + 30, currentY + 3, 100, 20, "Lockpicking", HtmlColors.MUSTARD);
					currentX += 120;
				}

				if (IsTrappable)
				{
					int checkboxGraphic = player.Preferences.ModernRemoveTrapsAutoRetryEnabled ? CHECKBOX_GRAPHIC_CHECKED : CHECKBOX_GRAPHIC_UNCHECKED;
					AddButton(currentX, currentY, checkboxGraphic, checkboxGraphic, (int)PageActions.ToggleRetryRemoveTrap, GumpButtonType.Reply, 0);
					TextDefinition.AddHtmlText(this, currentX + 30, currentY + 3, 100, 20, "Remove Trap", HtmlColors.MUSTARD);
				}
			}
		}

		private bool IsPickable
		{ get { return CheckIsPickable(Pickable); } }

		private bool IsTrappable
		{ get { return CheckIsTrappable(Trap); } }

		private ILockpickable Pickable
		{ get { return m_Target as ILockpickable; } }

		private ITrap Trap
		{ get { return m_Target as ITrap; } }

		public static bool TryLockpick(PlayerMobile from, Item target)
		{
			if (!TryShow(from, target)) return false;

			DoLockpick(from, target, false);
			return true;
		}

		public static bool TryRemoveTrap(PlayerMobile from, Item target, bool force)
		{
			if (!TryShow(from, target)) return false;

			DoRemoveTrap(from, target, false, force);
			return true;
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			var player = state.Mobile as PlayerMobile;
			if (player == null) return;

			var pageAction = (PageActions)info.ButtonID;
			switch (pageAction)
			{
				case PageActions.None:
					return;

				case PageActions.DoLockpick:
					DoLockpick(player, m_Target, m_ShowTrapState);
					break;

				case PageActions.DoRemoveTrap:
					DoRemoveTrap(player, m_Target, m_ShowLockpickState);
					break;

				case PageActions.DoDoubleclick:
					m_Target.OnDoubleClick(player);
					break;

				case PageActions.ToggleRetryLockpicking:
					player.Preferences.ModernLockpickingAutoRetryEnabled = !player.Preferences.ModernLockpickingAutoRetryEnabled;
					player.SendGump(new LockpickAndRemoveTrapGump(player, m_Target, m_ShowLockpickState, m_ShowTrapState));
					break;

				case PageActions.ToggleRetryRemoveTrap:
					player.Preferences.ModernRemoveTrapsAutoRetryEnabled = !player.Preferences.ModernRemoveTrapsAutoRetryEnabled;
					player.SendGump(new LockpickAndRemoveTrapGump(player, m_Target, m_ShowLockpickState, m_ShowTrapState));
					break;
			}
		}

		private static bool CheckIsPickable(ILockpickable pickable)
		{ return pickable != null; }

		private static bool CheckIsTrappable(ITrap trap)
		{ return trap != null; }

		private static void DoLockpick(PlayerMobile player, Item target, bool showTrapState)
		{
			var pickable = target as ILockpickable;
			if (pickable == null) return;

			var trap = target as ITrap;
			var isTrappable = CheckIsTrappable(trap);

			var pick = TryGetLockpick(player, target);
			var keepRunning = true;
			bool isRunning = false;
			RepeatableAction.Run(player,
			() =>
				{
					if (isRunning) return;

					keepRunning = player.Preferences.ModernLockpickingAutoRetryEnabled && pickable.Locked;
					if (!pickable.Locked) return;

					isRunning = true;
					player.PrivateOverheadMessage(MessageType.Regular, 1150, false, "You begin to pick the lock.", player.NetState);
					Lockpick.DoEffect(player, pick, target, () =>
					{
						isRunning = false;
						if (pick.Deleted)
						{
							pick = TryGetLockpick(player, target);
							if (pick == null) return;
						}
					});
				},
			() =>
			{
				if (pick == null) return false;
				if (player.Map != pick.Map || player.Map != target.Map) return false;
				if (!player.InRange(target.GetWorldLocation(), 2) || !player.InRange(pick.GetWorldLocation(), 2)) return false;
				if (!Lockpick.CanDoEffect(player, pick, target, pickable.Locked) || !keepRunning)
				{
					if (!pickable.Locked && showTrapState && (!isTrappable || !trap.IsActive))
						target.OnDoubleClick(player);
					else
						player.SendGump(new LockpickAndRemoveTrapGump(player, target, true, showTrapState));

					return false;
				}

				if (target is IAutoLockingContainer)
				{
					if (player.Skills[SkillName.Lockpicking].Value < pickable.MaxLockLevel) return true;

					player.PrivateOverheadMessage(MessageType.Regular, 1150, false, "That wasn't even a challenge.", player.NetState);
					return false;
				}

				return true;
			});
		}

		private static void DoRemoveTrap(PlayerMobile player, Item target, bool showLockpickState, bool force = false)
		{
			var trap = target as ITrap;
			var isTrappable = CheckIsTrappable(trap);
			if (!isTrappable && !force) return;

			var keepRunning = true;
			RepeatableAction.Run(player, () =>
			{
				if (DateTime.Now < player.NextSkillTime) return;
				if (!Skills.CanUseSkill(player, (int)SkillName.RemoveTrap)) return;

				keepRunning = player.Preferences.ModernRemoveTrapsAutoRetryEnabled && trap.IsActive;
				if (!trap.IsActive) return;

				if (Skills.TryExecuteSkillCallback(player))
				{
					player.PrivateOverheadMessage(MessageType.Regular, 1150, false, "You begin to remove the trap.", player.NetState);
					RemoveTrap.DoEffect(player, target);
				}
				else
				{
					player.SendSkillMessage();
				}
			},
			() =>
			{
				if (player.Map != target.Map) return false;
				if (!player.InRange(target.GetWorldLocation(), 2)) return false;
				if (!RemoveTrap.CanDoEffect(player, target) || !keepRunning)
				{			
					var pickable = target as ILockpickable;
					if (pickable != null && !pickable.Locked && showLockpickState && (!isTrappable || !trap.IsActive))
						target.OnDoubleClick(player);
					else
						player.SendGump(new LockpickAndRemoveTrapGump(player, target, showLockpickState, true));
					return false;
				}

				return true;
			});
		}

		private static Lockpick TryGetLockpick(PlayerMobile player, Item targeted)
		{
			var pick = player.Backpack.FindItemByType<Lockpick>(lockpick => Lockpick.ValidateLockpickType(lockpick, targeted));
			if (pick == null)
			{
				player.SendMessage("You don't have a lockpick that can be used on this container.");
				return null;
			}

			return pick;
		}

		private static bool TryShow(PlayerMobile from, Item target)
		{
			from.CloseGump(typeof(LockpickAndRemoveTrapGump));

			var trap = target as ITrap;
			var isTrappable = CheckIsTrappable(trap);
			var pickable = target as ILockpickable;
			var isPickable = CheckIsPickable(pickable);
			if (!isTrappable && !isPickable) return false;

			return true;
		}
	}
}