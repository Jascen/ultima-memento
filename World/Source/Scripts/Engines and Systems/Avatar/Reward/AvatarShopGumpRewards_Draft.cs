using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Avatar
{
	public partial class RewardFactory
	{
		public static List<IReward> CreateDraftRewards(PlayerMobile from, PlayerContext context, bool isInGypsyEncampment)
		{
			if (!context.DraftModeEnabled) return new List<IReward>();

			if (context.SelectedTemplate == AvatarStarterTemplates.None)
			{
				return new List<IReward>
				{
					ActionReward.Create(
						false,
						AvatarShopGump.COST_NO_BUY,
						AvatarShopGump.NO_ITEM_ID,
						"No Template Selected",
						"You have not selected a template. Please select a template to continue.",
						() => { }
					).AsStatic()
				};
			}

			if (context.DraftPicksSpent >= context.DraftPicksAvailable)
			{
				var noMorePicksReward = new List<IReward>();

				if (context.DraftLevel == 1)
				{
					noMorePicksReward.Add(
						ActionReward.Create(
							false,
							AvatarShopGump.COST_FREE,
							AvatarShopGump.NO_ITEM_ID,
							"My template is unplayable",
							"If you believe your template is unplayable, you may restart the drafting process.",
							() =>
							{
								context.SetDraftModeEnabled(from, true);
								// from.Kill();
								// from.Resurrect();
							}
						).AsStatic()
					);
					return noMorePicksReward;
				}

				noMorePicksReward.Add(
				context.DraftLevel == Constants.DRAFT_MAX_LEVEL
					? ActionReward.Create(
						false,
						AvatarShopGump.COST_NO_BUY,
						AvatarShopGump.NO_ITEM_ID,
						"No More Picks",
						"You have reached the maximum level for Draft.",
						() => { }
					).AsStatic()
					: ActionReward.Create(
						false,
						AvatarShopGump.COST_NO_BUY,
						AvatarShopGump.NO_ITEM_ID,
						"Next Pick",
						string.Format("You have no more picks available. Your next pick will be available at level {0}.",
							context.DraftLevel + Constants.DRAFT_LEVELS_PER_PICK - (context.DraftLevel % Constants.DRAFT_LEVELS_PER_PICK)
						),
						() => { }
					).AsStatic()
				);

				return noMorePicksReward;
			}

			var rewards = new List<IReward>();
			var allSkills = new List<SkillName>();
			foreach (var skillInfo in SkillInfo.Table)
			{
				var skillName = (SkillName)skillInfo.SkillID;
				if (skillName == SkillName.Mysticism) continue;
				if (skillName == SkillName.Imbuing) continue;
				if (skillName == SkillName.Throwing) continue;

				if (context.IsSkillDrafted(skillName)) continue;
				if (!context.HasPrerequisiteSkills(skillName)) continue;
				if (context.DraftBannedSkills.Contains(skillName)) continue;

				allSkills.Add(skillName);
			}

			var isInitialDraft = context.DraftPicksSpent < Constants.DRAFT_START_PICK_AMOUNT;
			var skills = context.DraftPicksSpent < Constants.DRAFT_START_PICK_AMOUNT
				? allSkills.Where(skillName => context.IsSmartSkill(skillName))
				: allSkills;
			foreach (var skillName in skills)
			{
				var skill = from.Skills[skillName];
				const int NEOPHYTE_SKILL_VALUE = 300;
				var archiveValue = context.Skills[skill.SkillName];
				if (isInitialDraft) archiveValue = Math.Max(archiveValue, NEOPHYTE_SKILL_VALUE);

				var maxValue = Math.Min(archiveValue / 10f, context.GetRecordedSkillCap());
				var maxValueFixedPoint = (int)(maxValue * 10);
				rewards.Add(
					ActionReward.Create(
						false,
						AvatarShopGump.COST_FREE,
						AvatarShopGump.NO_ITEM_ID, // TODO: Custom graphics for skills
						string.Format("{0}", skill.Name),
						isInGypsyEncampment
							? string.Format("Add this skill to your list of available skills and increase it to up to {0:n1}", maxValue)
							: "Add this skill to your list of available skills.",
						() =>
						{
							context.ClearRewardCache(Categories.Draft);
							context.AddDraftedSkill(skill.SkillName);
							skill.SetLockNoRelay(SkillLock.Up);
							skill.CanGain = true;
							from.Send(new SkillChange(skill));

							if (!isInGypsyEncampment) return;

							if (skill.IsSecondarySkill())
							{
								skill.BaseFixedPoint = maxValueFixedPoint;
							}
							else
							{
								var amountToGain = maxValueFixedPoint - skill.BaseFixedPoint;
								var amountAvailable = Math.Max(0, from.SkillsCap - from.SkillsTotal);

								var amountToIncrease = amountToGain;
								if (amountAvailable < amountToIncrease)
								{
									var amountRequired = amountToIncrease - amountAvailable;
									for (int i = 0; i < from.Skills.Length; ++i)
									{
										if (from.Skills[i].Lock != SkillLock.Down)
											continue;

										if (amountRequired >= from.Skills[i].BaseFixedPoint)
										{
											amountRequired -= from.Skills[i].BaseFixedPoint;
											from.Skills[i].Base = 0.0;
										}
										else
										{
											from.Skills[i].BaseFixedPoint -= amountRequired;
											amountRequired = 0;
											break;
										}
									}

									// Didn't get enough free points, so we'll just take what we can get
									if (0 < amountRequired)
										amountToIncrease -= amountRequired;
								}

								skill.BaseFixedPoint += amountToIncrease;
							}
						}
					).WithPrereq(
						context.DraftModeEnabled,
						"Requires Draft Mode to be enabled."
					).AllowSelectAnywhere()
				);
			}

			return rewards;
		}
	}
}