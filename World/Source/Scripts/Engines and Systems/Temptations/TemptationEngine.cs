using Server.Commands.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Temptation
{
	public class TemptationEngine
	{
		private static TemptationEngine m_Engine;
		private readonly Dictionary<Serial, PlayerContext> m_Context = new Dictionary<Serial, PlayerContext>();

		public static TemptationEngine Instance
		{
			get
			{
				if (m_Engine == null)
					m_Engine = new TemptationEngine();

				return m_Engine;
			}
		}

		public bool IsEnabled { get; set; }

		public static void Initialize()
		{
			LoadData();

			if (Instance.IsEnabled)
			{
				TargetCommands.Register(new TemptationsCommand());

				EventSink.WorldSave += Instance.OnWorldSave;
				CustomEventSink.PlayerDeleted += Instance.OnPlayerDeleted;
			}
		}

		public void ApplyContext(PlayerMobile player, PlayerContext context, bool initialLoad = false)
		{
			Item pants = player.FindItemOnLayer(Layer.InnerLegs);
			if (!context.CanWearTightPants && pants != null)
			{
				player.RemoveItem(pants);
			}

			BaseRace playerRace = player.FindItemOnLayer(Layer.Special) as BaseRace;
			if (playerRace != null)
			{
				playerRace.Delete();
				BaseRace.SyncRace(player, true);
			}

			// Don't constantly recreate the item every restart
			if (!initialLoad)
			{
				WorldUtilities.DeleteAllItems<OldSwordTalisman>(item => item.Owner == player);
				if (context.Flags.HasFlag(TemptationFlags.Deathwish))
				{
					var knife = new OldSwordTalisman { Owner = player };
					player.AddToBackpack(knife);
				}
			}

			// Skill cap could have changed (Titan or some other bonus could be reduced)
			player.RefreshSkillCap();

			if (context.HasPermanentDeath)
				SoulOrb.Create(player, SoulOrbType.PermadeathPlaceholder);
			else if (!player.Avatar.Active)
				WorldUtilities.DeleteAllItems<SoulOrb>(item => item.Owner == player && item.OrbType == SoulOrbType.PermadeathPlaceholder);
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
			if (context == PlayerContext.Default) return;

			m_Context.Remove(oldMobile.Serial);
			m_Context.Add(newMobile.Serial, context);
		}

		public void ReplaceContext(Mobile mobile, PlayerContext context)
		{
			m_Context.Remove(mobile.Serial);
			if (context == PlayerContext.Default) return;

			m_Context.Add(mobile.Serial, context);
		}

		private static void LoadData()
		{
			Instance.IsEnabled = !File.Exists("Saves//Player//Temptations.bin");

			Persistence.Deserialize(
				"Saves//Player//Temptations.bin",
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

					Console.WriteLine("[Temptations] Loaded data for '{0}' characters", Instance.m_Context.Count);
					Instance.IsEnabled = true;
				}
			);
		}

		private void OnPlayerDeleted(PlayerDeletedArgs e)
		{
			var player = e.Mobile;
			if (player == null) return;

			var context = GetContextOrDefault(player);
			if (context.Active)
			{
				m_Context.Remove(player.Serial);
				Console.WriteLine("[Temptations] Removed context for player '{0}' ({1})", player.Name, player.Serial);
			}

			Prune();
		}

		private void OnWorldSave(WorldSaveEventArgs e)
		{
			Prune();
			Persistence.Serialize(
				"Saves//Player//Temptations.bin",
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

		private void Prune()
		{
			var toRemove = m_Context.Keys.Where(key =>
			{
				Mobile mobile;
				if (!World.Mobiles.TryGetValue(key, out mobile)) return true; // Doesn't exist

				var player = mobile as PlayerMobile;
				return player == null // Not a player
					|| player.Deleted // Deleted
					|| !player.Temptations.Active // Temptations no longer active
					|| player.Temptations.Flags == TemptationFlags.None; // Temptations no longer have any flags
			});

			if (toRemove.Any())
			{
				var removeList = toRemove.ToList();
				foreach (var key in removeList)
				{
					m_Context.Remove(key);
				}
				Console.WriteLine("[Temptations] Removed data for '{0}' characters. A total of '{1}' characters remain.", removeList.Count, m_Context.Count);
			}
		}
	}
}