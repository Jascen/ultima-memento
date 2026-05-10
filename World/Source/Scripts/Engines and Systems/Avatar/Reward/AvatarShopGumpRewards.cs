using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Avatar
{
	public class RewardFactory
	{
		public const int ONE_HUNDRED_GOLD = 1000;
		public const int ONE_THOUSAND_GOLD = 10000;
		public const int TEN_GOLD = 100;

		public static List<IReward> CreateRewards(PlayerMobile m_From, Categories selectedCategory, PlayerContext context)
		{
			switch (selectedCategory)
			{
				default:
				case Categories.Information:
					{
						// Never reached
						return null;
					}

				case Categories.Ascensions:
					{
						var currentErudianBonus = context.GetRecordedSkillCap();
						int erudianCapCost;
						if (currentErudianBonus < 70) erudianCapCost = SecondOrderCost(100, context.RecordedSkillCapLevel + 1);
						else if (currentErudianBonus < 90) erudianCapCost = SecondOrderCost(200, context.RecordedSkillCapLevel + 1);
						else if (currentErudianBonus < 100) erudianCapCost = SecondOrderCost(400, context.RecordedSkillCapLevel + 1);
						else if (currentErudianBonus < 105) erudianCapCost = SecondOrderCost(800, context.RecordedSkillCapLevel + 1);
						else if (currentErudianBonus < 110) erudianCapCost = SecondOrderCost(1000, context.RecordedSkillCapLevel + 1);
						else if (currentErudianBonus < 115) erudianCapCost = SecondOrderCost(1200, context.RecordedSkillCapLevel + 1);
						else if (currentErudianBonus < 120) erudianCapCost = SecondOrderCost(2400, context.RecordedSkillCapLevel + 1);
						else erudianCapCost = SecondOrderCost(4800, context.RecordedSkillCapLevel + 1);

						var currentSkillCap = (Constants.SKILL_CAP_BASE / 10) + (context.SkillCapLevel * Constants.SKILL_CAP_PER_LEVEL);
						int skillCapCost;
						if (currentSkillCap < 400) skillCapCost = SecondOrderCost(200, 1);
						else if (currentSkillCap < 500) skillCapCost = SecondOrderCost(800, 1);
						else if (currentSkillCap < 600) skillCapCost = SecondOrderCost(1600, 1);
						else if (currentSkillCap < 700) skillCapCost = SecondOrderCost(3200, 1);
						else if (currentSkillCap < 800) skillCapCost = SecondOrderCost(6400, 1);
						else if (currentSkillCap < 900) skillCapCost = SecondOrderCost(12800, 1);
						else if (currentSkillCap < 1000) skillCapCost = SecondOrderCost(51200, 1);
						else skillCapCost = SecondOrderCost(4000, 1);

						int statCapCost;
						if (context.StatCapLevel < 10) statCapCost = SecondOrderCost(200, 1);
						else if (context.StatCapLevel < 20) statCapCost = SecondOrderCost(600, 1);
						else if (context.StatCapLevel < 30) statCapCost = SecondOrderCost(1200, 1);
						else if (context.StatCapLevel < 40) statCapCost = SecondOrderCost(2400, 1);
						else if (context.StatCapLevel < 50) statCapCost = SecondOrderCost(4800, 1);
						else if (context.StatCapLevel < 60) statCapCost = SecondOrderCost(9600, 1);
						else if (context.StatCapLevel < 70) statCapCost = SecondOrderCost(19200, 1);
						else if (context.StatCapLevel < 80) statCapCost = SecondOrderCost(38400, 1);
						else if (context.StatCapLevel < 90) statCapCost = SecondOrderCost(76800, 1);
						else if (context.StatCapLevel < 100) statCapCost = SecondOrderCost(153600, 1);
						else if (context.StatCapLevel < 110) statCapCost = SecondOrderCost(307200, 1);
						else if (context.StatCapLevel < 120) statCapCost = SecondOrderCost(614400, 1);
						else if (context.StatCapLevel < 130) statCapCost = SecondOrderCost(1228800, 1);
						else if (context.StatCapLevel < 140) statCapCost = SecondOrderCost(2457600, 1);
						else statCapCost = SecondOrderCost(4915200, 1);

						int pointGainRateCost = SecondOrderCost(50, context.PointGainRateLevel + 1);
						int skillGainRateCost = ExponentialCost(2000, context.SkillGainRateLevel + 1);

						return new List<IReward>
						{
							!context.HasSafetyDepositBox
								? ActionReward.Create(
									context.HasSafetyDepositBox,
									ONE_HUNDRED_GOLD,
									AvatarShopGump.NO_ITEM_ID,
									"Persistent Storage Container",
									"A safety deposit box is placed in your bankbox. Items in this container will persist through death.",
									() => {
										var box = context.GetOrCreateSafetyDepositBox(m_From);
										context.SafetyDepositBoxLevel = Math.Max(1, context.SafetyDepositBoxLevel);
										m_From.SendMessage("A safety deposit box has been placed in your bank box.");
									}
								)
								: ActionReward.Create(
									Constants.SAFETY_DEPOSIT_BOX_MAX_LEVEL <= context.SafetyDepositBoxLevel,
									SecondOrderCost(ONE_HUNDRED_GOLD, context.SafetyDepositBoxLevel + 1),
									AvatarShopGump.NO_ITEM_ID,
									string.Format("Safety Deposit Box ({0} of {1})", context.SafetyDepositBoxLevel, Constants.SAFETY_DEPOSIT_BOX_MAX_LEVEL),
									string.Format("Increase storage capacity of your safety deposit box. Current capacity: {0}", context.SafetyDepositBoxLevel),
									() => {
										var box = context.GetOrCreateSafetyDepositBox(m_From);
										context.SafetyDepositBoxLevel += 1;
										box.MaxItems = context.SafetyDepositBoxLevel;
										m_From.SendMessage("Your safety deposit box can now hold {0} items.", context.SafetyDepositBoxLevel);
									}
								),
							ActionReward.Create(
								context.UnlockRecordSkillCaps,
								ONE_HUNDRED_GOLD,
								AvatarShopGump.NO_ITEM_ID,
								"Erudian Teachings",
								"Reinforce your mind. Higher learning will become second nature.",
								() => {
									context.UnlockRecordSkillCaps = true;
									context.ClearRewardCache(Categories.PrimaryBoosts);
									context.ClearRewardCache(Categories.SecondaryBoosts);
									m_From.SendMessage("Your increased skill caps are now permanently unlocked.");
								}
							),
							ActionReward.Create(
								context.UnlockPrimarySkillBoost,
								2 * ONE_HUNDRED_GOLD,
								AvatarShopGump.NO_ITEM_ID,
								"Jack of No Trades",
								"Learn from the greatest masters. Unlock the ability to restore Primary skills.",
								() => {
									context.UnlockPrimarySkillBoost = true;
									context.ClearRewardCache(Categories.PrimaryBoosts);
									context.ClearRewardCache(Categories.SecondaryBoosts);
									m_From.SendMessage("Some of your Primary skills are now available in the Skill Archive.");
								}
							).WithPrereq(
								context.UnlockRecordSkillCaps,
								"Requires Erudian Knowledge to be unlocked."
							),
							ActionReward.Create(
								context.UnlockSecondarySkillBoost,
								2 * ONE_HUNDRED_GOLD,
								AvatarShopGump.NO_ITEM_ID,
								"Artisan's Mastery",
								"Master the crafts. Unlock the ability to restore Secondary skills.",
								() => {
									context.UnlockSecondarySkillBoost = true;
									context.ClearRewardCache(Categories.PrimaryBoosts);
									context.ClearRewardCache(Categories.SecondaryBoosts);
									m_From.SendMessage("Some of your Secondary skills are now available in the Skill Archive.");
								}
							).WithPrereq(
								context.UnlockRecordSkillCaps,
								"Requires Erudian Knowledge to be unlocked."
							),
							ActionReward.Create(
								context.UnlockRecordRecipes,
								ONE_THOUSAND_GOLD,
								AvatarShopGump.NO_ITEM_ID,
								"Crafter Lineage",
								"Record recipes that you have learned.",
								() => {
									context.UnlockRecordRecipes = true;
									m_From.SendMessage("Your recipes are now permanently unlocked.");
								}
							),
							ActionReward.Create(
								context.UnlockRecordDiscovered,
								5 * ONE_THOUSAND_GOLD,
								AvatarShopGump.NO_ITEM_ID,
								"World Class Cartographer",
								"Discover the world and its wonders. Permanently record your travels to every land.",
								() => {
									context.UnlockRecordDiscovered = true;
									m_From.SendMessage("Your facet discoveries are now permanently recorded.");
								}
							),
							ActionReward.Create(
								context.UnlockTemptations,
								10 * ONE_THOUSAND_GOLD,
								AvatarShopGump.NO_ITEM_ID,
								"Power Overwhelming",
								"Answer the seductive call of power. Gain strength through temptation and desire.",
								() => {
									context.UnlockTemptations = true;
									m_From.SendMessage("You have unlocked the ability to use Temptations.");
								}
							),
							ActionReward.Create(
								context.UnlockSavageRace,
								25 * ONE_THOUSAND_GOLD,
								AvatarShopGump.NO_ITEM_ID,
								"Primal Awakening",
								"Return to your untamed roots. Live life as a savage and embrace your barbaric heritage.",
								() => {
									context.UnlockSavageRace = true;
									m_From.SendMessage("You have unlocked a new tarot card for Humans.");
								}
							).WithPrereq(
								context.UnlockRecordDiscovered,
								"Requires World Class Cartographer to be unlocked."
							),
							ActionReward.Create(
								context.UnlockMonsterRaces,
								50 * ONE_THOUSAND_GOLD,
								AvatarShopGump.NO_ITEM_ID,
								"Bestial Transformation",
								"Embrace your monstrous nature. Live among the supernatural and the beastly races of Ultima.",
								() => {
									context.UnlockMonsterRaces = true;
									m_From.SendMessage("You have unlocked the option to select a non-human race.");
								}
							),
							ActionReward.Create(
								context.UnlockFugitiveMode,
								150 * ONE_THOUSAND_GOLD,
								AvatarShopGump.NO_ITEM_ID,
								"Outlaw's Mark",
								"Bear the mark of the hunted. Strengthen your core and live as an exile.",
								() => {
									context.UnlockFugitiveMode = true;
									m_From.SendMessage("You have unlocked a new tarot card for Monsters and Humans.");
								}
							),

							ActionReward.Create(
								Constants.IMPROVED_TEMPLATE_MAX_COUNT <= context.ImprovedTemplateCount,
								ONE_HUNDRED_GOLD * (context.ImprovedTemplateCount + 1),
								AvatarShopGump.NO_ITEM_ID,
								string.Format("Blessed Beginnings ({0} of {1})", context.ImprovedTemplateCount, Constants.IMPROVED_TEMPLATE_MAX_COUNT),
								string.Format("Awaken to your true potential. Ancestral relatives may enhance your template choices."),
								() => {
									context.ImprovedTemplateCount += 1;
									m_From.SendMessage("Your templates may now spawn as (Improved).");
								}
							),

							// Limits
							ActionReward.Create(
								Constants.RECORDED_SKILL_CAP_MAX_LEVEL <= context.RecordedSkillCapLevel,
								erudianCapCost,
								AvatarShopGump.NO_ITEM_ID,
								string.Format("Erudian Knowledge ({0} of {1})", context.RecordedSkillCapLevel, Constants.RECORDED_SKILL_CAP_MAX_LEVEL),
								string.Format("Increases the maximum of skill that your Skill Archive can provide by {0}. Current maximum: {1}", Constants.RECORDED_SKILL_CAP_INTERVAL, context.GetRecordedSkillCap()),
								() => {
									context.RecordedSkillCapLevel += 1;
									context.ClearRewardCache(Categories.PrimaryBoosts);
									context.ClearRewardCache(Categories.SecondaryBoosts);
								}
							).WithPrereq(
								context.UnlockPrimarySkillBoost || context.UnlockSecondarySkillBoost,
								"Requires Jack of No Trades or Artisan's Mastery to be unlocked."
							),
							ActionReward.Create(
								Constants.SKILL_CAP_MAX_LEVEL <= context.SkillCapLevel,
								skillCapCost,
								AvatarShopGump.NO_ITEM_ID,
								string.Format("Skill Cap ({0} of {1})", context.SkillCapLevel, Constants.SKILL_CAP_MAX_LEVEL),
								string.Format("Increases the skill cap by {0}. Current bonus: {1}", Constants.SKILL_CAP_PER_LEVEL, Constants.SKILL_CAP_PER_LEVEL * context.SkillCapLevel),
								() => context.SkillCapLevel += 1
							),
							ActionReward.Create(
								Constants.STAT_CAP_MAX_LEVEL <= context.StatCapLevel,
								statCapCost,
								AvatarShopGump.NO_ITEM_ID,
								string.Format("Stat Cap ({0} of {1})", context.StatCapLevel, Constants.STAT_CAP_MAX_LEVEL),
								string.Format("Increases the stat cap by {0}. Current bonus: {1}", Constants.STAT_CAP_PER_LEVEL, Constants.STAT_CAP_PER_LEVEL * context.StatCapLevel),
								() => context.StatCapLevel += 1
							),

							// Rates
							ActionReward.Create(
								Constants.POINT_GAIN_RATE_MAX_LEVEL <= context.PointGainRateLevel,
								pointGainRateCost,
								AvatarShopGump.NO_ITEM_ID,
								string.Format("Coins Gain Rate ({0} of {1})", context.PointGainRateLevel, Constants.POINT_GAIN_RATE_MAX_LEVEL),
								string.Format("Increases the coins gain rate by {0}%. Current bonus: {1}%", Constants.POINT_GAIN_RATE_PER_LEVEL, Constants.POINT_GAIN_RATE_PER_LEVEL * context.PointGainRateLevel),
								() => context.PointGainRateLevel += 1
							),
							ActionReward.Create(
								Constants.SKILL_GAIN_RATE_MAX_LEVEL <= context.SkillGainRateLevel,
								skillGainRateCost,
								AvatarShopGump.NO_ITEM_ID,
								string.Format("Skill Gain Rate ({0} of {1})", context.SkillGainRateLevel, Constants.SKILL_GAIN_RATE_MAX_LEVEL),
								string.Format("Increases the skill gain rate by {0}%. Current bonus: {1}%", Constants.SKILL_GAIN_RATE_PER_LEVEL, Constants.SKILL_GAIN_RATE_PER_LEVEL * context.SkillGainRateLevel),
								() => context.SkillGainRateLevel += 1
							),
						}.Where(r => r != null).ToList();
					}

				case Categories.Templates:
					{
						Action<Func<PlayerMobile, bool>> applyTemplate = action =>
						{
							if (m_From.NetState == null) return;

							var confirmation = new ConfirmationGump(
								m_From,
								"Select Template?",
								string.Format("Are you sure you wish to select this template? This is a {0} that will recreate your backpack, reduce existing stats, and change skills.", TextDefinition.GetColorizedText("destructive action", HtmlColors.RED)),
								() =>
								{
									SkillCheck.DisableSkillGains = true;

									// Auto-Lock Focus and Meditation to prevent them from naturally raising
									m_From.Skills.Focus.SetLockNoRelay(SkillLock.Locked);
									m_From.Skills.Meditation.SetLockNoRelay(SkillLock.Locked);

									// Reduce all skills to 0
									for (var i = 0; i < m_From.Skills.Length; i++)
									{
										var skill = m_From.Skills[i];
										if (0 < skill.Base)
											skill.Base = 0;
									}

									var boosted = action(m_From);

									// Boost skills if necessary
									for (var i = 0; i < m_From.Skills.Length; i++)
									{
										Skill skill = m_From.Skills[i];
										if (skill == null) continue;

										if (0 < skill.Value)
										{
											if (boosted)
												skill.BaseFixedPoint += 100; // +10 to each skill that was set
										}
									}

									SkillCheck.DisableSkillGains = false;

									AvatarEngine.Instance.ApplyContext(m_From, m_From.Avatar);
									m_From.OnSkillsQuery(m_From);
									m_From.SendMessage("Your skills have been set to the chosen template. Focus and Meditation have been set to Locked.");
								}
							);
							m_From.SendGump(confirmation);
						};
						var rewards = new List<IReward>
						{
							ActionReward.Create(
								AvatarShopGump.COST_FREE,
								AvatarShopGump.NO_ITEM_ID,
								"The Brute",
								"Starts with 60 strength, 10 dexterity, and 10 intelligence.",
								() =>
								{
									applyTemplate(
										player =>
										{
											m_From.InitStats(60, 10, 10);
											context.SelectedTemplate = AvatarStarterTemplates.Brute;
											return false;
										}
									);
								}
							),
							ActionReward.Create(
								AvatarShopGump.COST_FREE,
								AvatarShopGump.NO_ITEM_ID,
								"The Acrobat",
								"Starts with 10 strength, 60 dexterity, and 10 intelligence.",
								() =>
								{
									applyTemplate(
										player =>
										{
											m_From.InitStats(10, 60, 10);
											context.SelectedTemplate = AvatarStarterTemplates.Acrobat;
											return false;
										}
									);
								}
							),
							ActionReward.Create(
								AvatarShopGump.COST_FREE,
								AvatarShopGump.NO_ITEM_ID,
								"The Scholar",
								"Starts with 10 strength, 10 dexterity, and 60 intelligence.",
								() =>
								{
									applyTemplate(
										player =>
										{
											m_From.InitStats(10, 10, 60);
											context.SelectedTemplate = AvatarStarterTemplates.Scholar;

											return false;
										}
									);
								}
							),
						};

						var templates = new List<AvatarStarterTemplates>
						{
							// StarterProfessions.Custom,
							AvatarStarterTemplates.Ninja,
							AvatarStarterTemplates.Bard,
							AvatarStarterTemplates.Druid,
							AvatarStarterTemplates.Knight,
							AvatarStarterTemplates.Warrior,
							AvatarStarterTemplates.Mage,
							AvatarStarterTemplates.Archer,
						};

						HashSet<AvatarStarterTemplates> boostedTemplates = context.BoostedTemplateCache;
						if (boostedTemplates == null)
						{
							context.BoostedTemplateCache = boostedTemplates = new HashSet<AvatarStarterTemplates>();
						}

						if (0 < context.ImprovedTemplateCount && context.ImprovedTemplateCount <= templates.Count)
						{
							// Keep boosting a random profession until we reach our max
							while (boostedTemplates.Count != context.ImprovedTemplateCount)
							{
								boostedTemplates.Add(Utility.Random(templates));
							}
						}

						foreach (var template in templates.OrderBy(p => p.ToString()))
						{
							var boosted = 0 < boostedTemplates.Count && boostedTemplates.Contains(template);
							rewards.Add(ActionReward.Create(
								AvatarShopGump.COST_FREE,
								AvatarShopGump.NO_ITEM_ID,
								string.Format("The {0}{1}", template.ToString(), boosted ? " (Improved)" : ""),
								string.Format("Start with the stats, skills, and items of a {0}.", template.ToString()),
								() =>
								{
									applyTemplate(
										player =>
										{
											CharacterCreation.SetTemplateSkills(player, (StarterProfessions)template);
											context.SelectedTemplate = template;
											return boosted;
										}
									);
								}
							));
						}
						return rewards;
					}

				case Categories.FullSkillArchive:
					{
						var skills = new List<Skill>();
						for (var i = 0; i < m_From.Skills.Length; i++)
						{
							var skill = m_From.Skills[i];
							if (skill.SkillName == SkillName.Mysticism) continue;
							if (skill.SkillName == SkillName.Imbuing) continue;
							if (skill.SkillName == SkillName.Throwing) continue;

							var archiveValue = context.Skills[skill.SkillName];
							if (archiveValue < 1) continue;

							skills.Add(skill);
						}

						var rewards = new List<IReward>();
						foreach (var skill in skills.OrderBy(s => s.IsSecondarySkill()).ThenBy(s => m_From.Skills[s.SkillName].Name))
						{
							var value = context.Skills[skill.SkillName] / 10f;

							rewards.Add(
								ActionReward.Create(
									AvatarShopGump.COST_NO_BUY,
									AvatarShopGump.NO_ITEM_ID,
									skill.Name,
									string.Format("{0} skill. Your highest value was: {1:n1}", !skill.IsSecondarySkill() ? "Primary" : "Secondary", value),
									() => { }
								)
							);
						}

						return rewards;
					}

				case Categories.PrimaryBoosts:
				case Categories.SecondaryBoosts:
					{
						var rewards = new List<IReward>();
						var showPrimarySkills = selectedCategory == Categories.PrimaryBoosts;
						var showSecondarySkills = selectedCategory == Categories.SecondaryBoosts;

						// Skills
						if (showPrimarySkills || showSecondarySkills)
						{
							var skills = new List<Skill>();
							for (var i = 0; i < m_From.Skills.Length; i++)
							{
								var skill = m_From.Skills[i];
								if (skill.SkillName == SkillName.Mysticism) continue;
								if (skill.SkillName == SkillName.Imbuing) continue;
								if (skill.SkillName == SkillName.Throwing) continue;

								if (skill.IsSecondarySkill())
								{
									if (!showSecondarySkills) continue;
								}
								else
								{
									if (!showPrimarySkills) continue;
								}

								skills.Add(skill);
							}

							foreach (var skill in skills)
							{
								const int NEOPHYTE_SKILL_VALUE = 300;
								var archiveValue = context.Skills[skill.SkillName];
								if (archiveValue < NEOPHYTE_SKILL_VALUE) continue;

								var maxValue = Math.Min(archiveValue / 10f, context.GetRecordedSkillCap());
								var maxValueFixedPoint = (int)(maxValue * 10);
								rewards.Add(
									ActionReward.Create(
										maxValueFixedPoint <= skill.BaseFixedPoint,
										AvatarShopGump.COST_FREE,
										AvatarShopGump.NO_ITEM_ID,
										string.Format("{0}", skill.Name),
										string.Format("Raise your skill in {0} up to {1:n1}", skill.Name, maxValue),
										() =>
										{
											if (skill.IsSecondarySkill())
											{
												skill.BaseFixedPoint = maxValueFixedPoint;
											}
											else
											{
												var amountToGain = maxValueFixedPoint - skill.BaseFixedPoint;
												var amountAvailable = Math.Max(0, m_From.SkillsCap - m_From.SkillsTotal);

												var amountToIncrease = amountToGain;
												if (amountAvailable < amountToIncrease)
												{
													var amountRequired = amountToIncrease - amountAvailable;
													for (int i = 0; i < m_From.Skills.Length; ++i)
													{
														if (m_From.Skills[i].Lock != SkillLock.Down)
															continue;

														if (amountRequired >= m_From.Skills[i].BaseFixedPoint)
														{
															amountRequired -= m_From.Skills[i].BaseFixedPoint;
															m_From.Skills[i].Base = 0.0;
														}
														else
														{
															m_From.Skills[i].BaseFixedPoint -= amountRequired;
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
										showPrimarySkills ? context.UnlockPrimarySkillBoost : context.UnlockSecondarySkillBoost,
										string.Format("Requires {0} to be unlocked.", showPrimarySkills ? "Jack of No Trades" : "Artisan's Mastery")
									)
								);
							}
						}

						return rewards;
					}

				case Categories.Items:
					{
						return new List<IReward>
						{
							// Currency
							ItemReward.Create(
								ONE_THOUSAND_GOLD,
								true,
								() => { return new Gold(500); },
								amount: 500,
								graphicOverride: AvatarShopGump.GOLD_STACK_ITEM_ID
							),
							ItemReward.Create(
								5 * ONE_THOUSAND_GOLD,
								true,
								() => { return new Gold(5000); },
								amount: 5000,
								graphicOverride: AvatarShopGump.GOLD_STACK_ITEM_ID
							),

							// Resources
							ItemReward.Create(
								2 * ONE_HUNDRED_GOLD,
								true,
								() => { return new IronIngot(50); },
								50
							).WithDescription("A handful of ingots to get you started."),
							ItemReward.Create(
								ONE_HUNDRED_GOLD,
								true,
								() => { return new Fabric(50); },
								50
							).WithDescription("A handful of fabric to get you started."),
							ItemReward.Create(
								5 * TEN_GOLD,
								true,
								() => { return new Bottle(); },
								10
							).WithDescription("A handful of bottles to get you started."),

							// Tools
							ItemReward.Create(
								5 * ONE_HUNDRED_GOLD,
								true,
								() => { return new TinkerTools(); }
							),
							ItemReward.Create(
								5 * TEN_GOLD,
								true,
								() => { return new SmithHammer(); }
							),
							ItemReward.Create(
								5 * TEN_GOLD,
								true,
								() => { return new CarpenterTools(); }
							),
							ItemReward.Create(
								5 * TEN_GOLD,
								true,
								() => { return new SewingKit(); }
							),
							ItemReward.Create(
								5 * TEN_GOLD,
								true,
								() => { return new Hatchet(); }
							),
							ItemReward.Create(
								5 * TEN_GOLD,
								true,
								() => { return new Spade(); }
							),
							ItemReward.Create(
								5 * TEN_GOLD,
								true,
								() => { return new FishingPole(); }
							),
							ItemReward.Create(
								5 * TEN_GOLD,
								true,
								() => { return new Scissors(); }
							),

							// Equipment
							ItemReward.Create(
								ONE_THOUSAND_GOLD,
								true,
								() => { return new HikingBoots(); }
							),
							ItemReward.Create(
								5 * ONE_HUNDRED_GOLD,
								true,
								() => { return new BookOfChivalry(); }
							),
							ItemReward.Create(
								5 * ONE_HUNDRED_GOLD,
								true,
								() => { return new NecromancerSpellbook(); }
							).WithName("Necromancer Spellbook (Empty)"),
							ItemReward.Create(
								5 * ONE_HUNDRED_GOLD,
								true,
								() => { return new Spellbook(); }
							).WithName("Mage's Spellbook (Empty)"),
							ItemReward.Create(
								5 * ONE_HUNDRED_GOLD,
								true,
								() => { return new ElementalSpellbook(); }
							).WithName("Elementalist Spellbook (Empty)"),

							// Utility
							ItemReward.Create(
								5 * ONE_HUNDRED_GOLD,
								true,
								() => {
									var bag = new Bag();
									bag.AddItem(new Scissors());
									bag.AddItem(new Fabric(50));

									return bag;
								 }
							).WithName("Healer's Kit").WithDescription("Contains scissors and fabric."),
							ItemReward.Create(
								5 * ONE_HUNDRED_GOLD,
								true,
								() => {
									var bag = new Bag();
									bag.AddItem(new CurePotion() { Amount = 10 });
									bag.AddItem(new HealPotion() { Amount = 10 });
									bag.AddItem(new RefreshPotion() { Amount = 10 });

									return bag;
								 }
							).WithName("Warrior's Potion Bag").WithDescription("Contains healing, cure, and refresh potions."),
							ItemReward.Create(
								ONE_THOUSAND_GOLD,
								true,
								() => { return new BagOfReagents(); }
							).WithName("Bag of Reagents").WithDescription("Contains magery reagents for spells."),
							ItemReward.Create(
								5 * ONE_HUNDRED_GOLD,
								true,
								() => { return new BagOfNecroReagents(); }
							).WithName("Bag of Necro Reagents").WithDescription("Contains necromancy reagents for spells."),
							ItemReward.Create(
								10 * ONE_THOUSAND_GOLD,
								true,
								() => { return new SmallBoatDeed(); }
							).WithDescription("Hit the seas sailing!"),
							ItemReward.Create(
								30 * ONE_THOUSAND_GOLD,
								true,
								() => { return new MagicCarpetADeed(); }
							).WithDescription("I can show you the world!"),

							// Tames
							ItemReward.Create(
								ONE_HUNDRED_GOLD,
								true,
								() => { return new CagedHorse(); }
							).WithDescription("A horse is a great way to get around."),
							ItemReward.Create(
								ONE_THOUSAND_GOLD,
								true,
								() => { return new CagedPackHorse(); }
							).WithDescription("For when you're too weak to carry things yourself."),
						};
					}
			}
		}

		private static int ExponentialCost(int baseCost, int level)
		{
			var cost = baseCost;
			if (level <= 0) return cost;

			for (int i = 0; i < level; i++)
			{
				cost *= 2;
			}

			return cost;
		}

		private static int SecondOrderCost(double baseCost, int level)
		{
			return (int)(baseCost * Math.Pow(level, 2) + baseCost * level);
		}
	}
}