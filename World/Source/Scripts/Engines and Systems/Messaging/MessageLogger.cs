using Scripts.Mythik.Systems.Achievements;
using Server.Misc;
using Server.Mobiles;
using Server.Temptation;
using System.Linq;
using System.Text;

namespace Server.Engines.Messaging
{
	public class MessageLogger
	{
		public static string ChatWebhookUrl;
		public static string EventWebhookUrl;

		private static MessageLogger m_Engine;
		private NotificationService ChatService;
		private NotificationService EventService;

		public static MessageLogger Instance
		{
			get
			{
				if (m_Engine == null)
					m_Engine = new MessageLogger();

				return m_Engine;
			}
		}

		public static void Initialize()
		{
			if (!string.IsNullOrWhiteSpace(ChatWebhookUrl))
			{
				Instance.ChatService = NotificationService.Start(ChatWebhookUrl);

				CustomEventSink.ChatMessage += new ChatMessageEventHandler(Instance.OnChatMessage);
			}

			if (!string.IsNullOrWhiteSpace(EventWebhookUrl))
			{
				Instance.EventService = NotificationService.Start(EventWebhookUrl);

				EventSink.Login += new LoginEventHandler(Instance.OnPlayerLogin);
				EventSink.Logout += new LogoutEventHandler(Instance.OnPlayerLogout);
				EventSink.PlayerDeath += new PlayerDeathEventHandler(Instance.OnDeath);
				CustomEventSink.LootPull += new LootPullEventHandler(Instance.OnLootPull);
				CustomEventSink.EventLogged += new EventLoggedHandler(Instance.OnEventLogged);
				CustomEventSink.BeginJourney += new BeginJourneyHandler(Instance.OnBeginJourney);
				CustomEventSink.AchievementObtained += new AchievementObtainedHandler(Instance.OnAchievementObtained);
				CustomEventSink.PlayerVendorSale += new PlayerVendorSaleEventHandler(Instance.OnPlayerVendorSale);
			}
		}

		private void OnPlayerVendorSale(PlayerVendorSaleEventArgs e)
		{
			if (AccessLevel.Counselor <= e.Mobile.AccessLevel) return;

			EventService.QueueMessage(string.Format("*{0}* has purchased *{1}* from *{2}*", e.Mobile.Name, e.Item.Name, e.VendorName));
		}

		private void OnAchievementObtained(AchievementObtainedArgs e)
		{
			var type = e.Achievement.GetType();
			if (AccessLevel.Counselor <= e.Mobile.AccessLevel) return;

			if (type == typeof(DiscoveryAchievement)) // town or dungeon
			{
				EventService.QueueMessage(string.Format("*{0}* has discovered *{1}*", e.Mobile.Name, e.Achievement.Title));
				return;
			}

			if (type == typeof(HarvestAchievement))
			{
				EventService.QueueMessage(string.Format("*{0}* has worked hard to unlock *{1}*", e.Mobile.Name, e.Achievement.Title));
				return;
			}

			if (type == typeof(HunterAchievement))
			{
				var achievement = (HunterAchievement)e.Achievement;
				if (achievement.EnemyType == typeof(Exodus) || achievement.EnemyType == typeof(Jormungandr))
				{
					EventService.QueueMessage(string.Format("*{0}* has completed the feat of strength *{1}*", e.Mobile.Name, achievement.Title));
					return;
				}
			}

			EventService.QueueMessage(string.Format("*{0}* has earned the achievement *{1}*", e.Mobile.Name, e.Achievement.Title));
		}

		private void OnBeginJourney(BeginJourneyArgs e)
		{
			var player = e.Mobile;
			if (AccessLevel.Counselor <= player.AccessLevel) return;

			var name = player.Avatar.Active ? string.Format("{0} {1}", Icons.Dagger, player.Name) : player.Name;
			var message = string.Format("*{0}* has begun their journey", name);

			var temptationFlags = player.Temptations.Flags;
			if (temptationFlags != TemptationFlags.None)
			{
				var temptations = new string[]
				{
					temptationFlags.HasFlag(TemptationFlags.Puzzle_master) ? Icons.Jigsaw: null,
					temptationFlags.HasFlag(TemptationFlags.Strongest_Avenger) ? Icons.Jeans : null,
					temptationFlags.HasFlag(TemptationFlags.Famine) ? Icons.Stew : null,
					temptationFlags.HasFlag(TemptationFlags.I_can_take_it) ?Icons.Imp : null,
					// temptationFlags.HasFlag(TemptationFlags.This_is_just_a_tribute) ? Icons.Question  : null,
					temptationFlags.HasFlag(TemptationFlags.Deathwish) ? Icons.Skull_Crossbones : null,
				};
				message += string.Format(".\r\n{0} They have been tempted: {1}", Icons.Pepe_Yes, string.Join(" ", temptations.Where(x => x != null)));
			}

			EventService.QueueMessage(message);
		}

		private void OnChatMessage(ChatMessageEventArgs e)
		{
			var player = e.Mobile as PlayerMobile;
			if (player == null) return;
			if (!player.PublicInfo) return;

			ChatService.QueueMessage(string.Format("`{0}:` {1}", player.Name, e.Message));
		}

		private void OnDeath(PlayerDeathEventArgs args)
		{
			var player = args.Mobile as PlayerMobile;
			if (player == null) return;
			if (AccessLevel.Counselor <= player.AccessLevel) return;
			if (!player.PublicInfo) return;

			var builder = new StringBuilder();

			var killer = player.LastKiller;
			if (killer == player)
				builder.AppendFormat("*{0}* killed themselves", player.Name);
			else if (killer == null)
				builder.AppendFormat("*{0}* was killed", player.Name);
			else
				builder.AppendFormat("*{0}* was killed by *{1}*", player.Name, killer.Name);

			if (player.Temptations.HasPermanentDeath)
			{
				builder.AppendFormat(" after *{0}*. Rest in peace {1}",
					player.GameTime.TotalDays < 1
						? string.Format("{0}h {1}m", player.GameTime.Hours, player.GameTime.Minutes)
						: string.Format("{0}d {1}h {2}m", player.GameTime.Days, player.GameTime.Hours, player.GameTime.Minutes),
					Icons.Pepe_Heart
				);
			}
			else
				builder.Append(" !");

			EventService.QueueMessage(builder.ToString());
		}

		private void OnEventLogged(EventLoggedArgs e)
		{
			if (!e.Mobile.PublicInfo) return;
			if (AccessLevel.Counselor <= e.Mobile.AccessLevel) return;

			string @event = e.Event;
			if (!e.IsAnonymous)
				@event = string.Format("*{0}* {1}", e.Mobile.Name, @event.Trim());

			EventService.QueueMessage(@event);
		}

		private void OnLootPull(LootPullEventArgs e)
		{
			var player = e.Mobile as PlayerMobile;
			if (player == null) return;
			if (e.Item == null) return;
			if (AccessLevel.Counselor <= player.AccessLevel) return;

			EventService.QueueMessage(string.Format("*{0}* has acquired *{1}*!", player.RawName, e.Item.Name));
		}

		private void OnPlayerLogin(LoginEventArgs args)
		{
			var player = args.Mobile as PlayerMobile;
			if (player == null) return;
			if (AccessLevel.Counselor <= player.AccessLevel) return;
			if (!player.PublicInfo) return;

			EventService.QueueMessage(string.Format("*{0} {1}* has entered the realm", player.Name, player.Title != null ? player.Title : "the " + GetPlayerInfo.GetSkillTitle(player)));
		}

		private void OnPlayerLogout(LogoutEventArgs args)
		{
			var player = args.Mobile as PlayerMobile;
			if (player == null) return;
			if (AccessLevel.Counselor <= player.AccessLevel) return;
			if (!player.PublicInfo) return;

			EventService.QueueMessage(string.Format("*{0}* has left the realm", player.RawName));
		}
	}
}