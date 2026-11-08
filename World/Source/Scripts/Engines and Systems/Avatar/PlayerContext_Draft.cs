using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Engines.Avatar
{
	public partial class PlayerContext
	{
		private HashSet<SkillName> _draftBannedSkills;
		private HashSet<SkillName> _draftedSkills;
		private bool _draftModeEnabled;

		public IReadOnlyCollection<SkillName> DraftBannedSkills
		{ get { return _draftBannedSkills ?? (IReadOnlyCollection<SkillName>)Array.Empty<SkillName>(); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int DraftCurrentExperienceRequired
		{
			get
			{
				var requiredExperience = 0;
				for (var i = 1; i < DraftLevel; i++)
				{
					requiredExperience += GetDraftLevelExperience(PrestigeLevel, i);
				}

				return requiredExperience;
			}
		}

		public IReadOnlyCollection<SkillName> DraftedSkills
		{ get { return _draftedSkills ?? (IReadOnlyCollection<SkillName>)Array.Empty<SkillName>(); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int DraftExperienceToNextPick
		{
			get
			{
				if (Constants.DRAFT_MAX_LEVEL <= DraftLevel) return 0;

				var requiredExperience = DraftCurrentExperienceRequired;
				var draftLevel = DraftLevel;
				for (var i = 0; i < LevelsToNextPick; i++)
				{
					requiredExperience += GetDraftLevelExperience(PrestigeLevel, draftLevel + i);
				}

				return requiredExperience - DraftTotalExperienceGained;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int DraftLevel
		{
			get
			{
				var level = 1;
				var currentExperience = DraftTotalExperienceGained;
				do
				{
					currentExperience -= GetDraftLevelExperience(PrestigeLevel, level);
					if (currentExperience < 0) break;

					level++;
				} while (0 < currentExperience);

				return Math.Min(level, Constants.DRAFT_MAX_LEVEL);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool DraftModeEnabled
		{ get { return _draftModeEnabled; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int DraftPicksAvailable
		{ get { return Constants.DRAFT_START_PICK_AMOUNT + (DraftLevel / Constants.DRAFT_LEVELS_PER_PICK); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int DraftPicksSpent
		{ get { return _draftedSkills.Count; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int DraftTotalExperienceGained
		{ get { return GrandTotalPoints; } }

		private int LevelsToNextPick
		{ get { return Constants.DRAFT_LEVELS_PER_PICK - (DraftLevel % Constants.DRAFT_LEVELS_PER_PICK); } }

		public void AddDraftBannedSkill(SkillName skill)
		{
			if (!DraftModeEnabled) return;

			_draftBannedSkills.Add(skill);
		}

		public void AddDraftedSkill(SkillName skill)
		{
			if (!DraftModeEnabled) return;

			_draftedSkills.Add(skill);
		}

		public bool HasPrerequisiteSkills(SkillName skill)
		{
			// If draft mode is not enabled, all skills are available
			if (!DraftModeEnabled) return true;

			switch (skill)
			{
				// Crafting skills
				case SkillName.Blacksmith: return IsSkillDrafted(SkillName.Mining);
				case SkillName.Bowcraft: return IsSkillDrafted(SkillName.Lumberjacking);
				case SkillName.Carpentry: return IsSkillDrafted(SkillName.Lumberjacking);
				case SkillName.Tinkering: return IsSkillDrafted(SkillName.Mining);
				case SkillName.Alchemy: return IsSkillDrafted(SkillName.Cooking);
				case SkillName.Cooking:
				case SkillName.Inscribe:
				case SkillName.Tailoring:
					return true;

				// Gathering skills
				case SkillName.Forensics:
				case SkillName.Lumberjacking:
				case SkillName.Mining:
					return true;

				case SkillName.Stealth: return IsSkillDrafted(SkillName.Hiding);

				default:
					return true;
			}
		}

		public bool IsSkillDrafted(SkillName skill)
		{
			// If draft mode is not enabled, all skills are available
			if (!DraftModeEnabled) return true;

			return _draftedSkills != null && _draftedSkills.Contains(skill);
		}

		public bool IsSmartSkill(SkillName skillName)
		{
			if (IsSkillDrafted(skillName) || !HasPrerequisiteSkills(skillName)) return false;

			// TODO: Skip skills that have not been trained
			// if (skill.Base <= 0) continue;

			switch (skillName)
			{
				case SkillName.Alchemy:
				case SkillName.Blacksmith:
				case SkillName.Bowcraft:
				case SkillName.Carpentry:
				case SkillName.Cooking:
				case SkillName.Inscribe:
				case SkillName.Tailoring:
				case SkillName.Tinkering:
					// Limit - Already has a Crafting skill
					if (IsAnySkillDraftedExcept(
						skillName,
						SkillName.Alchemy,
						SkillName.Blacksmith,
						SkillName.Bowcraft,
						SkillName.Carpentry,
						SkillName.Cooking,
						SkillName.Inscribe,
						SkillName.Tailoring,
						SkillName.Tinkering
					)) return false;
					break;

				case SkillName.Forensics:
				case SkillName.Lumberjacking:
				case SkillName.Mining:
					// Limit - Already has a Gathering skill
					if (IsAnySkillDraftedExcept(
						skillName,
						SkillName.Forensics,
						SkillName.Lumberjacking,
						SkillName.Mining
					)) return false;
					break;

				case SkillName.Anatomy:
					if (!IsAnySkillDraftedExcept( // Combat skills
						skillName,
						SkillName.Anatomy,
						SkillName.Healing,
						SkillName.Knightship,
						SkillName.Bludgeoning,
						SkillName.Fencing,
						SkillName.FistFighting,
						SkillName.Marksmanship,
						SkillName.Swords
					)) return false;
					break;

				case SkillName.Druidism:
				case SkillName.Herding:
				case SkillName.Veterinary:
					// Make sure we have a Tamer skill
					if (!IsAnySkillDraftedExcept(
						skillName,
						SkillName.Druidism,
						SkillName.Herding,
						SkillName.Taming,
						SkillName.Veterinary
					)) return false;
					break;

				case SkillName.ArmsLore:
				case SkillName.Mercantile:
				case SkillName.Tasting:
					// Limit - Already has an ID skill
					if (IsAnySkillDraftedExcept(
						skillName,
						SkillName.ArmsLore,
						SkillName.Mercantile,
						SkillName.Tasting
					)) return false;

					if (skillName == SkillName.ArmsLore)
					{
						// Make sure we have a relevant Crafting skill
						if (!IsAnySkillDrafted(
							// SkillName.Alchemy,
							SkillName.Blacksmith,
							SkillName.Bowcraft,
							SkillName.Carpentry,
							// SkillName.Cooking,
							// SkillName.Inscribe,
							SkillName.Tailoring,
							SkillName.Tinkering
						)) return false;
					}
					break;

				case SkillName.Bushido:
				case SkillName.Knightship:
				case SkillName.Ninjitsu:
				case SkillName.Tactics:
					// Make sure we have a Weapon skill
					if (!IsAnySkillDrafted(
						SkillName.Bludgeoning,
						SkillName.Fencing,
						SkillName.FistFighting,
						// SkillName.Knightship,
						SkillName.Marksmanship,
						SkillName.Swords
					)) return false;
					break;

				case SkillName.Camping:
					// Make sure we have a relevant Tamer skill
					if (!IsAnySkillDraftedExcept(
						skillName,
						SkillName.Druidism,
						SkillName.Herding,
						SkillName.Taming,
						SkillName.Veterinary
					)) return false;
					break;

				case SkillName.Discordance:
				case SkillName.Peacemaking:
				case SkillName.Provocation:
					// Make sure we have a Bard skill
					if (!IsAnySkillDraftedExcept(
						skillName,
						SkillName.Discordance,
						SkillName.Musicianship,
						SkillName.Peacemaking,
						SkillName.Provocation
					)) return false;
					break;

				case SkillName.Elementalism:
				case SkillName.Magery:
				case SkillName.Necromancy:
					// Limit - Already have a Casting skill available
					if (IsAnySkillDraftedExcept(
						skillName,
						SkillName.Elementalism,
						SkillName.Magery,
						SkillName.Necromancy
					)) return false;
					break;

				case SkillName.Focus:
				case SkillName.Meditation:
					if (
						!IsAnySkillDraftedExcept( // Mana skills
							skillName,
							SkillName.Bushido,
							SkillName.Elementalism,
							SkillName.Knightship,
							SkillName.Magery,
							SkillName.Necromancy,
							SkillName.Ninjitsu,
							SkillName.FistFighting // Monk
					)) return false;
					break;

				case SkillName.Hiding:
				case SkillName.Lockpicking:
				case SkillName.RemoveTrap:
				case SkillName.Searching:
				case SkillName.Snooping:
				case SkillName.Stealing:
				case SkillName.Stealth:
					// Make sure we have a Thief skill
					if (!IsAnySkillDraftedExcept(
						skillName,
						SkillName.Hiding,
						SkillName.Ninjitsu,
						SkillName.Lockpicking,
						SkillName.RemoveTrap,
						SkillName.Searching,
						SkillName.Snooping,
						SkillName.Stealing,
						SkillName.Stealth
					)) return false;
					break;

				case SkillName.Bludgeoning:
				case SkillName.Fencing:
				case SkillName.FistFighting:
				case SkillName.Marksmanship:
				case SkillName.Swords:
					// Limit - Already has a Combat skill
					if (IsAnySkillDraftedExcept(
						skillName,
						// SkillName.Knightship,
						SkillName.Bludgeoning,
						SkillName.Fencing,
						SkillName.FistFighting,
						SkillName.Marksmanship,
						SkillName.Swords
					)) return false;
					break;

				case SkillName.Parry:
					// Make sure we have a (melee) Combat skill
					if (!IsAnySkillDraftedExcept(
						skillName,
						SkillName.Bludgeoning,
						SkillName.Fencing,
						SkillName.FistFighting,
						// SkillName.Marksmanship,
						SkillName.Swords
					)) return false;
					break;

				case SkillName.Psychology:
					if (
						!HasAllSkillsButThis(skillName, SkillName.Begging, SkillName.Psychology) // Jester
						&& !IsSkillDrafted(SkillName.Magery) // Mage
						&& !IsSkillDrafted(SkillName.Swords) // Jedi/Syth
					) return false;
					break;

				case SkillName.Spiritualism:
					if (
						!HasAllSkillsButThis(skillName, SkillName.Healing, SkillName.Spiritualism) // Holy Man
					) return false;
					break;

				case SkillName.Cartography:
				case SkillName.MagicResist:
				case SkillName.Poisoning:
				case SkillName.Seafaring:
				case SkillName.Tracking:
					// No smart roll option
					return false;

				case SkillName.Begging: // Starts Jester
				case SkillName.Healing: // Starts Holy Man
				case SkillName.Musicianship:
				case SkillName.Taming:
					// Always possible
					break;

				case SkillName.Mysticism:
				case SkillName.Imbuing:
				case SkillName.Throwing:
				default:
					Console.WriteLine("[Avatar] Unexpected skill during draft mode smart roll: {0}", skillName);
					return false;
			}

			return true;
		}

		public void RemoveDraftBannedSkill(SkillName skill)
		{
			if (!DraftModeEnabled) return;

			_draftBannedSkills.Remove(skill);
		}

		public void SetDraftModeEnabled(PlayerMobile player, bool enabled)
		{
			if (enabled)
			{
				_draftedSkills = new HashSet<SkillName>();

				// Maintain bans
				if (_draftBannedSkills == null) _draftBannedSkills = new HashSet<SkillName>();

				ClearRewardCache(Categories.Draft);
				player.SendMessage("Draft mode enabled. All skills have been reset to 0 and Locked.");
			}
			else
			{
				_draftedSkills = null;
				_draftBannedSkills = null;
				player.SendMessage("Draft mode disabled. All skills have been reset to 0 and Unlocked.");
			}

			// Zero out all skills
			for (var i = 0; i < player.Skills.Length; i++)
			{
				var skill = player.Skills[i];
				skill.SetLockNoRelay(!enabled ? SkillLock.Up : SkillLock.Locked);
				skill.CanGain = false;
				skill.Base = 0;
			}

			player.Send(new SkillUpdate(player.Skills));

			_draftModeEnabled = enabled;
		}

		private int GetDraftLevelExperience(int prestigeLevel, int currentLevel)
		{
			const double LEVEL_GROWTH = 0.08;
			// const double LEVEL_GROWTH = 0.48;
			const double PRESTIGE_GROWTH = 0.25;
			const int BASE_REQUIREMENT = 500;

			var required = BASE_REQUIREMENT;
			if (0 < prestigeLevel) required += (int)(1 + (prestigeLevel * PRESTIGE_GROWTH));

			var levelMultiplier = 1 + (LEVEL_GROWTH * (currentLevel - 1));

			return (int)(required * levelMultiplier);
		}

		private bool HasAllSkillsButThis(SkillName excludedSkill, params SkillName[] skills)
		{
			foreach (var skill in skills)
			{
				if (skill == excludedSkill) continue;
				if (!IsSkillDrafted(skill)) return false;
			}

			return true;
		}

		private bool IsAnySkillDrafted(params SkillName[] skills)
		{
			foreach (var skill in skills)
			{
				if (IsSkillDrafted(skill)) return true;
			}

			return false;
		}

		private bool IsAnySkillDraftedExcept(SkillName excludedSkill, params SkillName[] skills)
		{
			foreach (var skill in skills)
			{
				if (skill == excludedSkill) continue;
				if (IsSkillDrafted(skill)) return true;
			}

			return false;
		}
	}
}