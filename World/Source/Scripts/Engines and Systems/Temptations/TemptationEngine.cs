using Server.Commands.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

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

		public static void Configure()
		{
			EventSink.WorldSave += OnWorldSave;
		}

		public static void Initialize()
		{
			LoadData();

			if (Instance.IsEnabled)
			{
				TargetCommands.Register(new TemptationsCommand());
			}
		}

		public void ApplyContext(PlayerMobile player, PlayerContext context)
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

			WorldUtilities.DeleteAllItems<OldSwordTalisman>(item => item.Owner == player);
			if (context.Flags.HasFlag(TemptationFlags.Deathwish))
			{
				var knife = new OldSwordTalisman { Owner = player };
				player.AddToBackpack(knife);
			}

			// Skill cap could have changed (Titan or some other bonus could be reduced)
			player.RefreshSkillCap();

			if (context.HasPermanentDeath)
				SoulOrb.Create(player, SoulOrbType.PermadeathPlaceholder);
			else if (!player.Avatar.Active)
				WorldUtilities.DeleteAllItems<SoulOrb>(item => item.Owner == player);
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

					Console.WriteLine("Loaded Temptation data for '{0}' characters", Instance.m_Context.Count);
					Instance.IsEnabled = true;
				}
			);
		}

		private static void OnWorldSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(
				"Saves//Player//Temptations.bin",
				writer =>
				{
					writer.Write(0); // version

					writer.Write(Instance.m_Context.Count);
					foreach (var kv in Instance.m_Context)
					{
						writer.Write(kv.Key);
						kv.Value.Serialize(writer);
					}
				}
			);
		}
	}
}