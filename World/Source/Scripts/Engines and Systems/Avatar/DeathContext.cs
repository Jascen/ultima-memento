using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.IO;

namespace Server.Engines.Avatar
{
	public partial class DeathContext
	{
		public DeathContext()
		{
		}

		public DeathContext(GenericReader reader)
		{
			int version = reader.ReadInt();

			CombatQuestCompletions = reader.ReadInt();
			DeathNumber = reader.ReadInt();
			GameTime = reader.ReadTimeSpan();
			LifetimeCreatureKills = reader.ReadInt();
			LifetimeEnemyFactionKills = reader.ReadInt();
			PlayerName = reader.ReadString();
			PointsFarmed = reader.ReadInt();
			RivalBonusPoints = reader.ReadInt();
			RivalFactionName = reader.ReadString();
		}

		public int CombatQuestCompletions { get; set; }
		public int DeathNumber { get; set; }
		public TimeSpan GameTime { get; set; }
		public int LifetimeCreatureKills { get; set; }
		public int LifetimeEnemyFactionKills { get; set; }
		public string PlayerName { get; set; }
		public int PointsFarmed { get; set; }
		public int RivalBonusPoints { get; set; }
		public string RivalFactionName { get; set; }

		public void Serialize(GenericWriter writer)
		{
			writer.Write(0); // version

			writer.Write(CombatQuestCompletions);
			writer.Write(DeathNumber);
			writer.Write(GameTime);
			writer.Write(LifetimeCreatureKills);
			writer.Write(LifetimeEnemyFactionKills);
			writer.Write(PlayerName);
			writer.Write(PointsFarmed);
			writer.Write(RivalBonusPoints);
			writer.Write(RivalFactionName);
		}
	}

	public partial class DeathContext
	{
		public static List<DeathContext> Load()
		{
			var records = new List<DeathContext>();

			foreach (var file in Directory.GetFiles("Saves//Player//AvatarDeaths//", "*.bin"))
			{
				Persistence.Deserialize(file, reader =>
				{
					try
					{
						var record = new DeathContext(reader);
						records.Add(record);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error recording death: {0}", ex);
					}
				});
			}

			return records;
		}

		public static void TrySave(PlayerMobile player, PlayerContext context)
		{
			try
			{
				var deathContext = new DeathContext
				{
					PlayerName = player.Name,
					DeathNumber = context.LifetimeDeaths + 1,
					GameTime = player.GameTime,
					PointsFarmed = context.PointsFarmed,
					LifetimeCreatureKills = context.LifetimeCreatureKills,
					LifetimeEnemyFactionKills = context.LifetimeEnemyFactionKills,
					RivalBonusPoints = context.RivalBonusPoints,
					RivalFactionName = context.RivalFactionName,
				};

				var filePath = string.Format("Saves//Player//AvatarDeaths//{0}_{1}.bin", deathContext.PlayerName, deathContext.DeathNumber);
				Persistence.Serialize(filePath, writer => deathContext.Serialize(writer));
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error recording death: {0}", ex);
			}
		}
	}
}