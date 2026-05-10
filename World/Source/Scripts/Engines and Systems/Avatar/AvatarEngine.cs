using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.IO;

namespace Server.Engines.Avatar
{
	public class AvatarEngine
	{
		private static AvatarEngine m_Engine;
		private readonly Dictionary<Serial, PlayerContext> m_Context = new Dictionary<Serial, PlayerContext>();

		public static AvatarEngine Instance
		{
			get
			{
				if (m_Engine == null)
					m_Engine = new AvatarEngine() { IsEnabled = true };

				return m_Engine;
			}
		}

		public bool IsEnabled { get; set; }

		public static void Configure()
		{
		}

		public static void Initialize()
		{
			LoadData();

			if (Instance.IsEnabled)
			{
				EventSink.WorldSave += Instance.OnWorldSave;
				EventSink.OnKilledBy += Instance.OnKilledBy;
				EventSink.PlayerDeath += Instance.OnPlayerDeath;
				EventSink.SkillGain += Instance.OnSkillGain;
				CustomEventSink.CombatQuestCompleted += Instance.OnCombatQuestCompleted;
			}
		}

		public static void InitializePlayer(PlayerMobile player)
		{
			player.InitStats(10, 10, 10);

			if (player.Backpack == null)
				player.AddItem(new Backpack() { Movable = false });

			if (player.Backpack.FindItemByType<AvatarBook>() == null)
				player.Backpack.AddItem(new AvatarBook());
		}

		public void ApplyContext(PlayerMobile player, PlayerContext context)
		{
			if (!context.Active) return;

			player.StatCap = 100 + context.StatCapLevel * Constants.STAT_CAP_PER_LEVEL;

			// Skill cap could have changed
			player.RefreshSkillCap();

			SoulOrb.Create(player, SoulOrbType.PermadeathPlaceholder);
		}

		public PlayerContext GetContextOrDefault(Mobile mobile)
		{
			PlayerContext context;
			return mobile != null && mobile is PlayerMobile && m_Context.TryGetValue(mobile.Serial, out context) ? context : PlayerContext.Default;
		}

		public PlayerContext GetOrCreateContext(Mobile mobile)
		{
			var serial = mobile.Serial;

			PlayerContext context;
			if (m_Context.TryGetValue(serial, out context)) return context;

			return m_Context[serial] = new PlayerContext();
		}

		public void MigrateContext(Mobile oldMobile, Mobile newMobile)
		{
			var context = GetContextOrDefault(oldMobile);
			if (!context.Active) return;
			if (!m_Context.Remove(oldMobile.Serial)) return;

			m_Context.Add(newMobile.Serial, context);

			// Clear any cache
			context.RewardCache = null;
			context.BoostedTemplateCache = null;
			context.SelectedTemplate = AvatarStarterTemplates.None;

			context.GenerateRivalry();
		}

		private static void LoadData()
		{
			Persistence.Deserialize(
				"Saves//Player//Avatar.bin",
				reader =>
				{
					int version = reader.ReadInt();
					int count = reader.ReadInt();

					for (int i = 0; i < count; ++i)
					{
						var serial = reader.ReadInt();
						var context = new PlayerContext(reader);
						Instance.m_Context.Add(serial, context);
					}

					Console.WriteLine("Loaded Avatar data for '{0}' characters", Instance.m_Context.Count);
				}
			);

			foreach (var key in Instance.m_Context.Keys)
			{
				Mobile mobile;
				World.Mobiles.TryGetValue(key, out mobile);
				if (false == (mobile is PlayerMobile)) continue;

				var player = (PlayerMobile)mobile;
				if (!player.Avatar.Active) continue;

				Instance.ApplyContext(player, player.Avatar);
			}
		}

		private int GetBonusCoinsAmount(int value, PlayerContext context)
		{
			return (int)(value * context.PointGainRateLevel * Constants.POINT_GAIN_RATE_PER_LEVEL * 0.01);
		}

		private int GetValue<T>(int multiplier, Container corpse) where T : Item
		{
			var item = corpse.FindItemByType<T>();
			if (item == null) return 0;

			return item.Amount * multiplier;
		}

		private void GrantCoins(PlayerMobile player, int value, PlayerContext context)
		{
			context.PointsFarmed += value;
			CombatBar.Refresh(player);

			player.SendMessage("You have gained {0} coins.", value);
		}

		private void OnCombatQuestCompleted(CombatQuestCompletedArgs e)
		{
			if (e == null) return;
			if (e.Award < 1) return;
			if (false == (e.Mobile is PlayerMobile)) return;

			var player = (PlayerMobile)e.Mobile;
			if (!player.Avatar.Active) return;

			var value = e.Award * 5;
			value += GetBonusCoinsAmount(value, player.Avatar);

			player.Avatar.LifetimeCombatQuestCompletions += 1;

			GrantCoins(player, value, player.Avatar);
		}

		private void OnKilledBy(OnKilledByArgs e)
		{
			if (e.Corpse == null) return;
			if (false == (e.Killed is BaseCreature)) return;

			var player = MobileUtilities.TryGetMasterPlayer(e.KilledBy);
			if (player == null) return;

			var context = GetContextOrDefault(player);
			if (!context.Active) return;

			var creature = (BaseCreature)e.Killed;
			if (creature.AI == AIType.AI_Vendor) return;
			if (
				creature.NoKillAwards // Idk, but seems reasonable
				|| (creature.IsEphemeral && false == creature is BaseChampion) // No temporary mobs
				|| MobileUtilities.TryGetMasterPlayer(creature) != null // No pets
			 ) return;

			// If the total damage is less than 10% of the creature's max hits, no coins!
			if (e.TotalDamage < (e.Killed.HitsMax / 10)) return;

			var corpse = e.Corpse;
			int value = GetValue<DDCopper>(1, corpse);
			value += GetValue<DDSilver>(2, corpse);
			value += GetValue<DDXormite>(30, corpse);
			value += GetValue<Gold>(10, corpse);
			value += GetValue<Crystals>(50, corpse);
			value += GetValue<DDGemstones>(20, corpse);
			value += GetValue<DDJewels>(20, corpse);
			value += GetValue<DDGoldNuggets>(10, corpse);
			if (value < 1) return;

			if (1 < e.DamagerCount) value /= 2;

			context.LifetimeCreatureKills += 1;

			// Apply bonus coin multiplier
			value += GetBonusCoinsAmount(value, context);

			// Apply rivalry bonus
			if (context.HasRivalFaction)
			{
				var slayer = SlayerGroup.GetEntryByName(context.RivalSlayerName);
				if (slayer != null && slayer.Slays(e.Killed))
				{
					context.LifetimeEnemyFactionKills += 1;

					if (context.RivalBonusEnabled)
					{
						var bonus = (int)(value * Constants.RIVAL_BONUS_PERCENT * 0.01);
						if (0 < bonus)
						{
							context.RivalBonusPoints += bonus;
							value += bonus;

							if (Constants.RIVAL_BONUS_MAX_POINTS <= context.RivalBonusPoints)
							{
								context.RivalBonusEnabled = false;
								player.SendMessage("You have avenged your family by vanquishing all members of '{0}'.", context.RivalFactionName);
							}
							else
							{
								player.SendMessage("You have eliminated another member of '{0}'.", context.RivalFactionName);
							}
						}
					}
				}
			}

			GrantCoins(player, value, context);
		}

		private void OnPlayerDeath(PlayerDeathEventArgs e)
		{
			var player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var context = GetContextOrDefault(player);
			if (!context.Active) return;

			DeathContext.TrySave(player, context);

			context.LifetimeDeaths += 1;
			context.LifetimePointsGained += context.PointsFarmed;
			context.PointsSaved += context.PointsFarmed + context.RivalBonusPoints;
			context.PointsFarmed = 0;
			context.RivalBonusPoints = 0;
		}

		private void OnSkillGain(SkillGainArgs e)
		{
			if (e == null) return;
			if (e.Skill == null) return;
			if (false == (e.From is PlayerMobile)) return;

			var player = (PlayerMobile)e.From;
			if (!player.Avatar.Active) return;

			player.Avatar.Skills[e.Skill.SkillID] = Math.Max(player.Avatar.Skills[e.Skill.SkillID], e.Skill.BaseFixedPoint);
		}

		private void OnWorldSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(
				"Saves//Player//Avatar.bin",
				writer =>
				{
					writer.Write(0); // version

					writer.Write(m_Context.Count);
					foreach (var kv in m_Context)
					{
						writer.Write(kv.Key);
						kv.Value.Serialize(writer);
					}
				}
			);
		}
	}
}