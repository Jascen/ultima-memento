using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Spells;
using Server.Targeting;
using System;

namespace Server.Utilities
{
	#region Models

	public enum SkeletonKeyTier
	{
		Regular,
		Master
	}

	public struct DoorUnlockOptions
	{
		public bool AllowDungeonDoors;
		public bool AllowSpaceshipDoors;
		public string AlreadyUnlockedMessage;
		public bool ConsumeTool;
		public int DungeonSound;
		public string NoEffectMessage;
		public Action<Mobile, Item> OnConsumed;
		public bool PlaySoundBeforeUnlock;
		public bool RevealingAction;
		public int SpaceshipSound;
		public bool UseLocalizedNotLockedMessage;
	}

	public struct SkeletonKeyMessages
	{
		public string FailTooHardCard;
		public string FailTooHardKey;
		public string RejectInvalidTarget;
		public string RejectNotContainer;
		public string RejectSelfOrHouse;
		public string RejectUnneeded;
		public string RejectVirtualContainer;
		public string RejectWrongContainer;
		public string SuccessCard;
		public string SuccessKey;
	}

	public struct SpellUnlockProfile
	{
		public string FailureMessage;
		public Func<Mobile, int> GetUnlockLevel;
		public string HouseDoorMessage;
		public int MaxLevel;
		public string ParagonMessage;
		public string PirateMessage;
		public bool RejectVirtualContainer;
		public string SecuredMessage;
		public string TreasureMapMessage;
		public bool UseLocalizedSecured;
		public bool UsePrivateFailureMessage;
		public string VirtualContainerMessage;
	}

	public struct TrapDissolverProfile
	{
		public string FailMessage;
		public string RejectUnused;
		public bool ReturnEmptyContainer;
		public int Sound;
		public string SuccessMessage;
		public string ToolDescription;
	}

	#endregion Models

	public static class UnlockUtilities
	{
		public const int CardSlotItemId = 0x3A75;
		public const int MAX_TMAP_LEVEL = 2;

		#region Profiles

		public static readonly TrapDissolverProfile AcidProfile = new TrapDissolverProfile
		{
			Sound = 0x231,
			ToolDescription = "This acid is to dissolve locks and traps on most chests.",
			SuccessMessage = "The acid seems to have eaten away at the mechanism inside.",
			FailMessage = "The acid seems to have done nothing to the mechanism inside.",
			RejectUnused = "You don't need to use acid on that.",
			ReturnEmptyContainer = true
		};

		public static readonly SpellUnlockProfile EspionageUnlockProfile = new SpellUnlockProfile
		{
			GetUnlockLevel = from =>
			{
				var level = (int)(from.Skills[SkillName.Ninjitsu].Value) + 20;
				if (level > 50) level = 50;
				return level;
			},
			MaxLevel = 50,
			HouseDoorMessage = "This ability is to unlock certain containers and other types of doors.",
			VirtualContainerMessage = "This key is to unlock almost any container.",
			SecuredMessage = "You cannot use this ability on a secure item!",
			UseLocalizedSecured = false,
			TreasureMapMessage = "A magical aura on this long lost treasure will always be too much for your abilities.",
			ParagonMessage = "A magical aura on this long lost treasure will always be too much for your abilities.",
			PirateMessage = "This seems to be protected from magic, but maybe an actual lock picker can get it open.",
			FailureMessage = "Your ability does not seem to have an effect on that lock!",
			RejectVirtualContainer = false,
			UsePrivateFailureMessage = false
		};

		public static readonly DoorUnlockOptions LockpickDoorOptions = new DoorUnlockOptions
		{
			AllowSpaceshipDoors = true,
			AllowDungeonDoors = true,
			ConsumeTool = false,
			RevealingAction = false,
			UseLocalizedNotLockedMessage = true,
			PlaySoundBeforeUnlock = true,
			AlreadyUnlockedMessage = null,
			NoEffectMessage = null,
			SpaceshipSound = 0x54B,
			DungeonSound = 0x241
		};

		public static readonly SpellUnlockProfile MageryUnlockProfile = new SpellUnlockProfile
		{
			GetUnlockLevel = from =>
			{
				var level = (int)(Spell.ItemSkillValue(from, SkillName.Magery, false)) + 20;
				if (level > 50) level = 50;
				return level;
			},
			MaxLevel = 50,
			HouseDoorMessage = "This spell is to unlock certain containers and other types of doors.",
			VirtualContainerMessage = "This key is to unlock almost any container.",
			SecuredMessage = null,
			UseLocalizedSecured = true,
			TreasureMapMessage = "A magical aura on this long lost treasure seems to negate your spell.",
			ParagonMessage = "A magical aura on this long lost treasure seems to negate your spell.",
			PirateMessage = "This seems to be protected from magic, but maybe a thief can get it open.",
			FailureMessage = null,
			RejectVirtualContainer = false,
			UsePrivateFailureMessage = false
		};

		public static readonly SpellUnlockProfile NecroUnlockProfile = new SpellUnlockProfile
		{
			GetUnlockLevel = from =>
			{
				var level = (int)(from.Skills[SkillName.Necromancy].Value) + BasePotion.EnhancePotions(from);
				if (level > 90) level = 90;
				return level;
			},
			MaxLevel = 90,
			HouseDoorMessage = null,
			VirtualContainerMessage = null,
			SecuredMessage = "You cannot use this on a secure item.",
			UseLocalizedSecured = false,
			TreasureMapMessage = null,
			ParagonMessage = null,
			PirateMessage = null,
			FailureMessage = "This does not seem to work on that lock.",
			RejectVirtualContainer = true,
			UsePrivateFailureMessage = true
		};

		public static readonly TrapDissolverProfile PlasmaTorchProfile = new TrapDissolverProfile
		{
			Sound = 0x227,
			ToolDescription = "This torch is to melt locks and traps on most chests.",
			SuccessMessage = "The torch seems to have melted the mechanism inside.",
			FailMessage = "The torch seems to have done nothing to the mechanism inside.",
			RejectUnused = "You don't need to use torch on that.",
			ReturnEmptyContainer = false
		};

		public static readonly DoorUnlockOptions SkeletonKeyDoorOptions = new DoorUnlockOptions
		{
			AllowSpaceshipDoors = true,
			AllowDungeonDoors = true,
			ConsumeTool = true,
			RevealingAction = true,
			UseLocalizedNotLockedMessage = false,
			PlaySoundBeforeUnlock = false,
			AlreadyUnlockedMessage = "That does not need to be unlocked.",
			NoEffectMessage = "That does not need to be unlocked.",
			SpaceshipSound = 0x54B,
			DungeonSound = 0x241
		};

		public static readonly SkeletonKeyMessages SkeletonKeyMessagesMagic = new SkeletonKeyMessages
		{
			RejectSelfOrHouse = "This key is to unlock almost any container.",
			RejectVirtualContainer = "This key is to unlock almost any container.",
			RejectInvalidTarget = "This key is to unlock any container.",
			RejectNotContainer = "That is not a container.",
			RejectWrongContainer = "This key is to unlock any container.",
			FailTooHardKey = null,
			FailTooHardCard = null,
			SuccessCard = "You swipe the key card to open the lock.",
			SuccessKey = "The key opens the lock.",
			RejectUnneeded = "You don't need to use this key on that."
		};

		public static readonly SkeletonKeyMessages SkeletonKeyMessagesMaster = new SkeletonKeyMessages
		{
			RejectSelfOrHouse = "This key is to unlock almost any container.",
			RejectVirtualContainer = "This key is to unlock almost any container.",
			RejectInvalidTarget = "This key is to unlock any container.",
			RejectNotContainer = "That is not a container.",
			RejectWrongContainer = "This key is to unlock any container.",
			FailTooHardKey = null,
			FailTooHardCard = null,
			SuccessCard = "You swipe the key card to open the lock, but also wearing it out from further use.",
			SuccessKey = "The key opens the lock, wearing the key out from further use.",
			RejectUnneeded = "You don't need to use this key on that."
		};

		public static readonly SkeletonKeyMessages SkeletonKeyMessagesRegular = new SkeletonKeyMessages
		{
			RejectSelfOrHouse = "This key is to unlock certain containers.",
			RejectVirtualContainer = "This key is to unlock almost any container.",
			RejectInvalidTarget = "This key is to unlock certain containers.",
			RejectNotContainer = "That is not a container.",
			RejectWrongContainer = "This key is to unlock certain containers.",
			FailTooHardKey = "The lock seems too complicated for this key.",
			FailTooHardCard = "The lock seems too secure for this key card.",
			SuccessCard = "You swipe the key card to open the lock, but also wearing it out from further use.",
			SuccessKey = "The key opens the lock, wearing the key out from further use.",
			RejectUnneeded = "You don't need to use this key on that."
		};

		#endregion Profiles

		public static void ApplyContainerUnlock(LockableContainer cont, Mobile picker, bool setPicker)
		{
			if (cont.LockLevel == -255)
			{
				cont.LockLevel = cont.RequiredSkill - 10;
				if (cont.LockLevel == 0)
					cont.LockLevel = -1;
			}

			if (setPicker)
				cont.Picker = picker;
		}

		public static void ApplySpellContainerUnlock(LockableContainer cont)
		{
			if (cont.LockLevel == -255)
				cont.LockLevel = cont.RequiredSkill - 10;
		}

		public static void BeginSkeletonKeyUnlock(Mobile from, Item tool, SkeletonKeyTier tier, SkeletonKeyMessages messages)
		{
			from.Target = new SkeletonKeyUnlockTarget(tool, tier, messages);
		}

		public static void BeginTrapDissolverUnlock(Mobile from, Item tool, TrapDissolverProfile profile, Func<Mobile, object, Item, bool> extraHandler = null)
		{
			from.Target = new TrapDissolverUnlockTarget(tool, profile, extraHandler);
		}

		public static bool TryDissolveContainer(Mobile from, object targeted, Item tool, TrapDissolverProfile profile)
		{
			var o = (ILockable)targeted;
			var cont = o as LockableContainer;
			TrapableContainer trapCont;
			trapCont = o as TrapableContainer;

			if (cont == null || trapCont == null)
			{
				from.SendMessage("That is not a container.");
				return true;
			}

			if (!o.Locked && trapCont.TrapType == TrapType.None)
			{
				from.SendMessage(profile.RejectUnused);
				return true;
			}

			if (o is BaseDoor && !((BaseDoor)o).UseLocks())
			{
				from.SendMessage(profile.ToolDescription);
				return true;
			}

			if (IsInvalidTreasureMapChest(targeted))
			{
				from.SendMessage(profile.FailMessage);
				ConsumeDissolverTool(from, tool, profile);
				return true;
			}

			if (100 >= cont.RequiredSkill)
			{
				DissolveContainerSuccess(from, o, cont, trapCont, targeted, tool, profile);
				return true;
			}

			if (trapCont.TrapType != TrapType.None && trapCont.TrapLevel > 0)
			{
				ClearTrap(trapCont);
				from.SendMessage(profile.SuccessMessage);
				from.RevealingAction();
				from.PlaySound(profile.Sound);
				ConsumeDissolverTool(from, tool, profile);
				return true;
			}

			from.SendMessage(profile.FailMessage);
			ConsumeDissolverTool(from, tool, profile);
			return true;
		}

		public static bool TryMeltHead(Mobile from, object targeted, Item tool)
		{
			var head = targeted as Head;
			if (head == null)
				return false;

			if (head.ItemID == 7584 || head.ItemID == 7393)
			{
				from.RevealingAction();
				from.PlaySound(AcidProfile.Sound);
				ConsumeDissolverTool(from, tool, AcidProfile);
				head.ItemID = 0x1AE0;
				if (head.Name != null && head.Name.Contains(" head "))
					head.Name = head.Name.Replace(" head ", " skull ");
				from.SendMessage("The acid melts the skin away, leaving only a skull.");
			}
			else
			{
				from.SendMessage("Someone already used acid to melt the skin away.");
			}

			return true;
		}

		public static bool TrySpellUnlock(Mobile from, object target, SpellUnlockProfile profile)
		{
			if (target is Mobile)
			{
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503101); // That did not need to be unlocked.
				return true;
			}

			if (target is BaseHouseDoor)
			{
				if (profile.HouseDoorMessage != null)
					from.SendMessage(profile.HouseDoorMessage);
				else
					from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503101);
				return true;
			}

			if (target is Item && ((Item)target).VirtualContainer)
			{
				if (profile.VirtualContainerMessage != null)
					from.SendMessage(profile.VirtualContainerMessage);
				else
					from.SendLocalizedMessage(501666);
				return true;
			}

			if (target is BaseDoor)
			{
				var door = (BaseDoor)target;
				if (DoorType.IsDungeonDoor(door))
				{
					if (!door.Locked)
						from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503101);
					else
					{
						door.Locked = false;
						DoorType.UnlockDoors(door);
					}
				}
				else
					from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503101);

				return true;
			}

			var cont = target as LockableContainer;
			if (cont == null)
			{
				from.SendLocalizedMessage(501666); // You can't unlock that!
				return true;
			}

			if (BaseHouse.CheckSecured(cont))
			{
				if (profile.UseLocalizedSecured)
					from.SendLocalizedMessage(503098);
				else
					from.SendMessage(profile.SecuredMessage);
				return true;
			}

			if (!cont.Locked)
			{
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503101);
				return true;
			}

			if (cont.LockLevel == 0)
			{
				from.SendLocalizedMessage(501666);
				return true;
			}

			if (profile.RejectVirtualContainer && cont.VirtualContainer)
			{
				from.SendLocalizedMessage(501666);
				return true;
			}

			if (cont is ParagonChest && profile.ParagonMessage != null)
			{
				from.SendMessage(profile.ParagonMessage);
				return true;
			}

			if (cont is PirateChest && profile.PirateMessage != null)
			{
				from.SendMessage(profile.PirateMessage);
				return true;
			}

			var level = profile.GetUnlockLevel(from);
			var canUnlock = level >= cont.RequiredSkill;

			if (IsInvalidTreasureMapChest(cont))
			{
				if (profile.FailureMessage != null)
					from.SendMessage(profile.FailureMessage);
				else
					from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503099);
				return true;
			}

			if (canUnlock)
			{
				cont.Locked = false;
				ApplySpellContainerUnlock(cont);
			}
			else if (profile.UsePrivateFailureMessage)
				from.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, profile.FailureMessage, from.NetState);
			else if (profile.FailureMessage != null)
				from.SendMessage(profile.FailureMessage);
			else
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503099); // My spell does not seem to have an effect on that lock.

			return true;
		}

		public static bool TryUnlockDoor(Mobile from, BaseDoor door, Item tool, DoorUnlockOptions opts)
		{
			var toolItemId = tool != null ? tool.ItemID : 0;
			var isCard = toolItemId == CardSlotItemId;
			var isSpaceship = DoorType.IsSpaceshipDoor(door);
			var isDungeon = DoorType.IsDungeonDoor(door);

			if (isSpaceship && !isCard)
			{
				from.SendMessage("This doesn't have a key hole, but it does have a card slot.");
				return true;
			}

			if (!isSpaceship && isCard)
			{
				from.SendMessage("This doesn't have a card slot, but it does have a key hole.");
				return true;
			}

			if (opts.AllowSpaceshipDoors && isSpaceship && isCard)
			{
				if (!door.Locked)
				{
					SendAlreadyUnlocked(from, opts);
					return true;
				}

				if (opts.PlaySoundBeforeUnlock)
					from.PlaySound(opts.SpaceshipSound);

				door.Locked = false;
				DoorType.UnlockDoors(door);

				if (opts.RevealingAction)
					from.RevealingAction();

				if (!opts.PlaySoundBeforeUnlock)
					from.PlaySound(opts.SpaceshipSound);

				FinishDoorConsume(from, tool, opts);
				return true;
			}

			if (opts.AllowDungeonDoors && isDungeon && !isCard)
			{
				if (!door.Locked)
				{
					SendAlreadyUnlocked(from, opts);
					return true;
				}

				if (opts.PlaySoundBeforeUnlock)
					from.PlaySound(opts.DungeonSound);

				door.Locked = false;
				DoorType.UnlockDoors(door);

				if (opts.RevealingAction)
					from.RevealingAction();

				if (!opts.PlaySoundBeforeUnlock)
					from.PlaySound(opts.DungeonSound);

				FinishDoorConsume(from, tool, opts);
				return true;
			}

			if (opts.UseLocalizedNotLockedMessage)
				from.SendLocalizedMessage(502069); // This does not appear to be locked
			else if (opts.NoEffectMessage != null)
				from.SendMessage(opts.NoEffectMessage);

			return true;
		}

		public static bool TryUnlockSkeletonContainer(Mobile from, object targeted, Item tool, SkeletonKeyTier tier, SkeletonKeyMessages messages)
		{
			var o = (ILockable)targeted;
			var cont = o as LockableContainer;
			if (cont == null)
			{
				from.SendMessage(messages.RejectNotContainer);
				return true;
			}

			if (BaseHouse.CheckSecured(cont))
			{
				from.SendLocalizedMessage(503098); // You cannot cast this on a secure item.
				return true;
			}

			if (!cont.Locked)
			{
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503101); // That did not need to be unlocked.
				return true;
			}

			if (cont.LockLevel == 0)
			{
				from.SendLocalizedMessage(501666); // You can't unlock that!
				return true;
			}

			var isCard = tool.ItemID == CardSlotItemId;
			var catalog = cont.Catalog;

			if (catalog == Catalogs.SciFi && o.Locked && !isCard)
			{
				from.SendMessage("This doesn't have a key hole, but it does have a card slot.");
				return true;
			}

			if (catalog != Catalogs.SciFi && o.Locked && isCard)
			{
				from.SendMessage("This doesn't have a card slot, but it does have a key hole.");
				return true;
			}

			if (catalog == Catalogs.SciFi && o.Locked && isCard)
			{
				if (tier == SkeletonKeyTier.Regular)
				{
					var neededMod = Difficult.GetDifficulty(from.Location, from.Map) * 5;
					if (neededMod < 1) neededMod = 0;
					var neededSkill = 51 + neededMod;

					if (cont.RequiredSkill >= neededSkill)
					{
						from.SendMessage(messages.FailTooHardCard);
						return true;
					}
				}

				UnlockSkeletonContainer(from, o, cont, messages.SuccessCard, 0x54B, tool);
				return true;
			}

			if (o.Locked && !isCard)
			{
				if (o is BaseDoor && !((BaseDoor)o).UseLocks())
				{
					from.SendMessage(messages.RejectWrongContainer);
					return true;
				}

				if (tier == SkeletonKeyTier.Regular)
				{
					if (cont.RequiredSkill >= 51 || IsInvalidTreasureMapChest(targeted) || targeted is PirateChest || targeted is ParagonChest)
					{
						from.SendMessage(messages.FailTooHardKey);
						return true;
					}
				}

				UnlockSkeletonContainer(from, o, cont, messages.SuccessKey, 0x241, tool);
				return true;
			}

			from.SendMessage(messages.RejectUnneeded);
			return true;
		}

		private static void ClearTrap(TrapableContainer cont)
		{
			if (cont.TrapType != TrapType.None)
				cont.TrapType = TrapType.None;
		}

		private static void ConsumeDissolverTool(Mobile from, Item tool, TrapDissolverProfile profile)
		{
			if (profile.ReturnEmptyContainer)
				ReturnEmptyAcidContainer(from, tool);
			tool.Consume();
		}

		private static void DissolveContainerSuccess(Mobile from, ILockable o, LockableContainer cont, TrapableContainer trapCont, object targeted, Item tool, TrapDissolverProfile profile)
		{
			o.Locked = false;
			ApplyContainerUnlock(cont, from, true);
			ClearTrap(trapCont);

			if (targeted is Item)
				from.SendMessage(profile.SuccessMessage);

			from.RevealingAction();
			from.PlaySound(profile.Sound);
			ConsumeDissolverTool(from, tool, profile);
		}

		private static void FinishDoorConsume(Mobile from, Item tool, DoorUnlockOptions opts)
		{
			if (opts.ConsumeTool && tool != null)
			{
				if (opts.OnConsumed != null)
					opts.OnConsumed(from, tool);
				tool.Consume();
			}
		}

		private static void HandleSkeletonKeyTarget(Mobile from, object targeted, Item tool, SkeletonKeyTier tier, SkeletonKeyMessages messages)
		{
			if (!tool.IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
				return;
			}

			if (targeted == tool)
			{
				from.SendMessage(messages.RejectSelfOrHouse);
				return;
			}

			if (targeted is BaseHouseDoor)
			{
				from.SendMessage(messages.RejectSelfOrHouse);
				return;
			}

			if (targeted is Item && ((Item)targeted).VirtualContainer)
			{
				from.SendMessage(messages.RejectVirtualContainer);
				return;
			}

			if (targeted is BaseDoor)
			{
				TryUnlockDoor(from, (BaseDoor)targeted, tool, SkeletonKeyDoorOptions);
				return;
			}

			if (targeted is ILockable)
			{
				TryUnlockSkeletonContainer(from, targeted, tool, tier, messages);
				return;
			}

			from.SendMessage(messages.RejectInvalidTarget);
		}

		private static void HandleTrapDissolverTarget(Mobile from, object targeted, Item tool, TrapDissolverProfile profile, Func<Mobile, object, Item, bool> extraHandler)
		{
			if (!tool.IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
				return;
			}

			if (extraHandler != null && extraHandler(from, targeted, tool))
				return;

			if (targeted == tool)
			{
				from.SendMessage(profile.ToolDescription);
				return;
			}

			if (targeted is BaseHouseDoor)
			{
				from.SendMessage(profile.ToolDescription);
				return;
			}

			if (targeted is Item && ((Item)targeted).VirtualContainer)
			{
				from.SendMessage("This key is to unlock almost any container.");
				return;
			}

			if (targeted is BaseDoor)
			{
				var door = (BaseDoor)targeted;
				var opts = new DoorUnlockOptions
				{
					AllowSpaceshipDoors = false,
					AllowDungeonDoors = true,
					ConsumeTool = true,
					RevealingAction = true,
					UseLocalizedNotLockedMessage = false,
					PlaySoundBeforeUnlock = false,
					AlreadyUnlockedMessage = "That does not need to be unlocked.",
					NoEffectMessage = "That does not need to be unlocked.",
					SpaceshipSound = profile.Sound,
					DungeonSound = profile.Sound,
					OnConsumed = profile.ReturnEmptyContainer ? (Action<Mobile, Item>)ReturnEmptyAcidContainer : null
				};

				TryUnlockDoor(from, door, tool, opts);
				return;
			}

			if (targeted is ILockable)
			{
				TryDissolveContainer(from, targeted, tool, profile);
				return;
			}

			from.SendMessage(profile.ToolDescription);
		}

		private static bool IsInvalidTreasureMapChest(object chest)
		{
			return chest is TreasureMapChest && MAX_TMAP_LEVEL < ((TreasureMapChest)chest).Level;
		}

		private static void ReturnEmptyAcidContainer(Mobile from, Item tool)
		{
			if (tool.ItemID == 0x1007)
				from.AddToBackpack(new Jar());
			else
				from.AddToBackpack(new Bottle());
		}

		private static void SendAlreadyUnlocked(Mobile from, DoorUnlockOptions opts)
		{
			if (opts.UseLocalizedNotLockedMessage)
				from.SendLocalizedMessage(502069);
			else if (opts.AlreadyUnlockedMessage != null)
				from.SendMessage(opts.AlreadyUnlockedMessage);
		}

		private static void UnlockSkeletonContainer(Mobile from, ILockable o, LockableContainer cont, string successMessage, int sound, Item tool)
		{
			o.Locked = false;
			ApplyContainerUnlock(cont, from, true);

			if (successMessage != null)
				from.SendMessage(successMessage);

			from.RevealingAction();
			from.PlaySound(sound);
			tool.Consume();
		}

		private class SkeletonKeyUnlockTarget : Target
		{
			private readonly SkeletonKeyMessages m_Messages;
			private readonly SkeletonKeyTier m_Tier;
			private readonly Item m_Tool;

			public SkeletonKeyUnlockTarget(Item tool, SkeletonKeyTier tier, SkeletonKeyMessages messages) : base(2, false, TargetFlags.None)
			{
				m_Tool = tool;
				m_Tier = tier;
				m_Messages = messages;
				CheckLOS = true;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				HandleSkeletonKeyTarget(from, targeted, m_Tool, m_Tier, m_Messages);
			}
		}

		private class TrapDissolverUnlockTarget : Target
		{
			private readonly Func<Mobile, object, Item, bool> m_ExtraHandler;
			private readonly TrapDissolverProfile m_Profile;
			private readonly Item m_Tool;

			public TrapDissolverUnlockTarget(Item tool, TrapDissolverProfile profile, Func<Mobile, object, Item, bool> extraHandler) : base(2, false, TargetFlags.None)
			{
				m_Tool = tool;
				m_Profile = profile;
				m_ExtraHandler = extraHandler;
				CheckLOS = true;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				HandleTrapDissolverTarget(from, targeted, m_Tool, m_Profile, m_ExtraHandler);
			}
		}
	}
}