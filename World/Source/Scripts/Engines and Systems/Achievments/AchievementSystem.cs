using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Commands;
using Server.Misc;
using Scripts.Mythik.Systems.Achievements.Gumps;
using System.Globalization;
using System.Linq;

namespace Scripts.Mythik.Systems.Achievements
{
	public class AchievementSystem
	{
		private static readonly TextInfo m_TextInfo = new CultureInfo("en-US", false).TextInfo;

		public class AchievementCategory
		{
			public int ID { get; set; }
			public int Parent { get; set; }
			public string Name;


			public AchievementCategory(int id, int parent, string v3)
			{
				ID = id;
				Parent = parent;
				Name = v3;
			}
		}

		public static List<BaseAchievement> Achievements = new List<BaseAchievement>();
		public static List<AchievementCategory> Categories = new List<AchievementCategory>();
		private static Dictionary<string, Dictionary<int, AchieveData>> m_featData = new Dictionary<string, Dictionary<int, AchieveData>>();
		private static Dictionary<string, int> m_pointsTotal = new Dictionary<string, int>();

		private static string TryGetId(PlayerMobile player)
		{
			if (player == null || player.Account == null)
			{
				Console.WriteLine("Failed to ID for {0}", player != null ? player.Name : "Unknown Mobile");
				return null;
			}

			var username = player.Account.Username.Replace("$", "");
			var id = username;
			if (player.Avatar.Active) id += "-$AV";
			else if (player.Temptations.HasPermanentDeath) id += "-$HC";

			return id;
		}

		private static Dictionary<int, AchieveData> GetOrCreateFeatData(PlayerMobile from)
		{
			var id = TryGetId(from);
			if (id == null) return new Dictionary<int, AchieveData>();

			if (!m_featData.ContainsKey(id)) m_featData.Add(id, new Dictionary<int, AchieveData>());

			return m_featData[id];
		}

		private static int GetPlayerPointsTotal(PlayerMobile m)
		{
			var id = TryGetId(m);
			if (id == null) return 0;

			if (!m_pointsTotal.ContainsKey(id)) m_pointsTotal.Add(id, 0);

			return m_pointsTotal[id];
		}

		private static void AddPoints(PlayerMobile m, int points)
		{
			var id = TryGetId(m);
			if (id == null) return;

			if (!m_pointsTotal.ContainsKey(id)) m_pointsTotal.Add(id, 0);

			m_pointsTotal[id] += points;
		}

		public static void Initialize()
		{
			BaseAchievement achieve = null;
			const int DISCOVER_LAND_GRAPHIC = 0x14EB; // Map

			#region Exploration - 1 to 999

			Categories.Add(new AchievementCategory(1, 0, "Exploration"));

			#region Exploration - Facets (1 - 19)

			var discoverSosaria = AddLand(1, 1, "One Small Step", "Discover the World of Sosaria", Land.Sosaria);
			var discoverUmberVeil = AddLand(2, 1, "Through The Veil", "Discover the land of Umber Veil", Land.UmberVeil);
			var discoverAmbrosia = AddLand(3, 1, "The Lost Land", "Discover the land of Ambrosia", Land.Ambrosia);
			var discoverLodoria = AddLand(4, 1, "One Does Not Simply...", "Discover the Elven World of Lodoria", Land.Lodoria);
			var discoverSerpentIsland = AddLand(5, 1, "Hisstory In The Making", "Discover the Serpent Island", Land.Serpent);
			var discoverDreadIsles = AddLand(6, 1, "Dread The Unknown", "Discover the Isles of Dread", Land.IslesDread);
			var discoverSavagedEmpire = AddLand(7, 1, "This Party Is Savage", "Discover the Savaged Empire", Land.Savaged);
			var discoverBottleWorld = AddLand(8, 1, "Message In A Bottle", "Discover the Bottle World of Kuldar", Land.Kuldar);
			var discoverUnderworld = AddLand(9, 1, "Into The Darkness", "Discover the Underworld", Land.Underworld);
			var discoverLuna = AddLand(10, 1, "Blastoff!", "Discover the City of Luna", Land.Luna);
			var discoverSkara = AddLand(11, 1, "Help! I'm Trapped!", "Discover the Town of Skara Brae", Land.SkaraBrae);
			var discoverAtlantis = AddLand(12, 1, "The Lost City", "Discover the World of Atlantis", Land.Atlantis);
			discoverAtlantis.HiddenTillComplete = true;  // TODO: No way atm?
														 // 13
														 // 14
														 // 15
														 // 16

			#endregion Exploration - Facets (1 - 19)

			#region Exploration - Towns (20 - 119)

			Categories.Add(new AchievementCategory(2, 1, "Towns"));

			// Ambrosia
			// 20
			// 21
			// 22
			// 23

			// Isle of Dread
			AddTown(24, 2, discoverDreadIsles, "the Cimmeran Hold");
			AddTown(25, 2, discoverDreadIsles, "the Forgotten Lighthouse");
			// 26
			// 27
			// 28
			// 29

			// Lodoria
			AddTown(30, 2, discoverLodoria, "the Port of Dusk");
			AddTown(31, 2, discoverLodoria, "the City of Elidor");
			AddTown(32, 2, discoverLodoria, "the Town of Glacial Hills");
			AddTown(33, 2, discoverLodoria, "Greensky Village");
			AddTown(34, 2, discoverLodoria, "the Village of Islegem");
			AddTown(35, 2, discoverLodoria, "Kraken Reef Docks");
			AddTown(36, 2, discoverLodoria, "the City of Lodoria");
			AddTown(37, 2, discoverLodoria, "the Village of Portshine");
			AddTown(38, 2, discoverLodoria, "the Village of Ravendark");
			AddTown(39, 2, discoverLodoria, "the Village of Springvale");
			AddTown(40, 2, discoverLodoria, "the Port of Starguide");
			AddTown(41, 2, discoverLodoria, "the Village of Whisper");
			// 41
			// 42
			// 43
			// 44

			// Savaged Empire
			AddTown(45, 2, discoverSavagedEmpire, "the Barako Mines");
			AddTown(46, 2, discoverSavagedEmpire, "Savage Sea Docks");
			AddTown(47, 2, discoverSavagedEmpire, "the Village of Barako");
			AddTown(48, 2, discoverSavagedEmpire, "the Village of Kurak");
			// 49
			// 50
			// 51
			// 52

			// Serpent Island
			AddTown(53, 2, discoverSerpentIsland, "the City of Furnace");
			AddTown(54, 2, discoverSerpentIsland, "Serpent Sail Docks");
			// 55
			// 56
			// 57
			// 58

			// Sosaria
			AddTown(59, 2, discoverSosaria, "Anchor Rock Docks");
			AddTown(60, 2, discoverSosaria, "the City of Britain");
			AddTown(61, 2, discoverSosaria, "the Lunar City of Dawn"); // TODO: Redundant?
			AddTown(62, 2, discoverSosaria, "Death Gulch");
			AddTown(63, 2, discoverSosaria, "the Town of Devil Guard");
			AddTown(64, 2, discoverSosaria, "the Village of Fawn");
			AddTown(65, 2, discoverSosaria, "Glacial Coast Village");
			AddTown(66, 2, discoverSosaria, "the Village of Grey");
			AddTown(67, 2, discoverSosaria, "Iceclad Fisherman's Village");
			AddTown(68, 2, discoverSosaria, "the City of Montor");
			AddTown(69, 2, discoverSosaria, "the Town of Moon");
			AddTown(70, 2, discoverSosaria, "the Town of Mountain Crest");
			AddTown(71, 2, discoverSosaria, "the Fort of Stonewall");
			AddTown(72, 2, discoverSosaria, "the Undercity of Umbra");
			// 73
			// 74
			// 75
			// 76

			// Umber Veil
			AddTown(77, 2, discoverUmberVeil, "the Town of Renika");
			// 78
			// 79
			// 80
			// 81

			// Underworld
			AddTown(82, 2, discoverUnderworld, "the Fort of Tenebrae");
			// 83
			// 84
			// 85
			// 86

			#endregion Exploration - Towns (20 - 119)

			#region Exploration - Dungeons (120 - 219)

			Categories.Add(new AchievementCategory(3, 1, "Dungeons"));

			// Ambrosia
			AddDungeon(120, 3, discoverAmbrosia, "the City of the Dead");
			AddDungeon(121, 3, discoverAmbrosia, "the Dragon's Maw");
			AddDungeon(122, 3, discoverAmbrosia, "the Cave of the Zuluu");
			// ...
			// 130

			// Isle of Dread
			AddDungeon(131, 3, discoverDreadIsles, "the Blood Temple");
			AddDungeon(132, 3, discoverDreadIsles, "the Glacial Scar");
			AddDungeon(133, 3, discoverDreadIsles, "the Ice Queen Fortress");
			AddDungeon(134, 3, discoverDreadIsles, "the Sanctum of Saltmarsh");
			AddDungeon(135, 3, discoverDreadIsles, "the Scurvy Reef");
			AddDungeon(136, 3, discoverDreadIsles, "the Temple of Osirus");
			// ...
			// 145

			// Lodoria
			AddDungeon(146, 3, discoverLodoria, "the Lodoria Catacombs");
			AddDungeon(147, 3, discoverLodoria, "the City of Embers");
			AddDungeon(148, 3, discoverLodoria, "Dungeon Covetous");
			AddDungeon(149, 3, discoverLodoria, "Dungeon Despise");
			AddDungeon(150, 3, discoverLodoria, "Dungeon Deceit");
			AddDungeon(151, 3, discoverLodoria, "Dungeon Destard");
			AddDungeon(152, 3, discoverLodoria, "the Frozen Hells", false);
			AddDungeon(153, 3, discoverLodoria, "Dungeon Hythloth");
			AddDungeon(154, 3, discoverLodoria, "Dungeon Wrong", false);
			AddDungeon(155, 3, discoverLodoria, "the Lizardman Cave", false);
			AddDungeon(156, 3, discoverLodoria, "the Castle of Dracula", false);
			AddDungeon(157, 3, discoverLodoria, "the Crypts of Dracula");
			AddDungeon(158, 3, discoverLodoria, "Dungeon Shame");
			AddDungeon(159, 3, discoverLodoria, "Stonegate Castle");
			AddDungeon(160, 3, discoverLodoria, "Terathan Keep");
			AddDungeon(161, 3, discoverLodoria, "the Halls of Undermountain");
			AddDungeon(162, 3, discoverLodoria, "the Volcanic Cave");
			// ...
			// 175

			// Savaged Empire
			AddDungeon(176, 3, discoverSavagedEmpire, "the Cave of the Ancient Wyrm", false);
			AddDungeon(177, 3, discoverSavagedEmpire, "the Dungeon of the Mad Archmage");
			AddDungeon(178, 3, discoverSavagedEmpire, "the Corrupt Pass", false);
			AddDungeon(179, 3, discoverSavagedEmpire, "the Catacombs of Azerok");
			AddDungeon(180, 3, discoverSavagedEmpire, "the Dungeon of the Lich King");
			AddDungeon(181, 3, discoverSavagedEmpire, "the Halls of Ogrimar", false);
			AddDungeon(182, 3, discoverSavagedEmpire, "the Great Pyramid", false);
			AddDungeon(183, 3, discoverSavagedEmpire, "the Ratmen Mines", false);
			AddDungeon(184, 3, discoverSavagedEmpire, "Dungeon Rock");
			AddDungeon(185, 3, discoverSavagedEmpire, "the Spider Cave", false);
			AddDungeon(186, 3, discoverSavagedEmpire, "the Tomb of Kazibal");
			AddDungeon(187, 3, discoverSavagedEmpire, "the Tombs");
			AddDungeon(188, 3, discoverSavagedEmpire, "the Undersea Castle");
			AddDungeon(189, 3, discoverSavagedEmpire, "the Storm Giant Lair", false);
			// ...
			// 200

			// Serpent Island
			AddDungeon(200, 3, discoverSerpentIsland, "the Ancient Prison");
			AddDungeon(201, 3, discoverSerpentIsland, "the Vault of the Black Knight");
			AddDungeon(202, 3, discoverSerpentIsland, "the Cave of Fire");
			AddDungeon(203, 3, discoverSerpentIsland, "the Cave of Souls");
			AddDungeon(204, 3, discoverSerpentIsland, "Dungeon Ankh");
			AddDungeon(205, 3, discoverSerpentIsland, "Dungeon Bane");
			AddDungeon(206, 3, discoverSerpentIsland, "Dungeon Hate");
			AddDungeon(207, 3, discoverSerpentIsland, "Dungeon Scorn");
			AddDungeon(208, 3, discoverSerpentIsland, "Dungeon Torment");
			AddDungeon(209, 3, discoverSerpentIsland, "Dungeon Vile");
			AddDungeon(210, 3, discoverSerpentIsland, "Dungeon Wicked");
			AddDungeon(211, 3, discoverSerpentIsland, "Dungeon Wrath");
			AddDungeon(212, 3, discoverSerpentIsland, "the Flooded Temple");
			AddDungeon(213, 3, discoverSerpentIsland, "the Gargoyle Crypts");
			AddDungeon(214, 3, discoverSerpentIsland, "the Serpent Sanctum");
			AddDungeon(215, 3, discoverSerpentIsland, "the Tomb of the Fallen Wizard");
			// ...
			// 225

			// Sosaria
			AddDungeon(226, 3, discoverSosaria, "the Ancient Pyramid");
			AddDungeon(227, 3, discoverSosaria, "the Ruins of the Black Blade", false);
			AddDungeon(228, 3, discoverSosaria, "Dungeon Exodus");
			AddDungeon(229, 3, discoverSosaria, "the Cave of Banished Mages", false);
			AddDungeon(230, 3, discoverSosaria, "the Caverns of Poseidon");
			AddDungeon(231, 3, discoverSosaria, "Dungeon Clues");
			AddDungeon(232, 3, discoverSosaria, "Dardin's Pit");
			AddDungeon(233, 3, discoverSosaria, "Dungeon Doom");
			AddDungeon(234, 3, discoverSosaria, "the Fires of Hell");
			AddDungeon(235, 3, discoverSosaria, "the Forgotten Halls");
			AddDungeon(236, 3, discoverSosaria, "the Mines of Morinia");
			AddDungeon(237, 3, discoverSosaria, "the Montor Sewers", false);
			AddDungeon(238, 3, discoverSosaria, "the Perinian Depths");
			AddDungeon(239, 3, discoverSosaria, "the Dungeon of Time Awaits");
			// TODO: "Ice Island Dungeons"
			// ...
			// 250

			// Umber Veil
			AddDungeon(251, 3, discoverUmberVeil, "the Mausoleum");
			AddDungeon(252, 3, discoverUmberVeil, "the Tower of Brass");
			// ...
			// 265

			// Underworld
			AddDungeon(266, 3, discoverUnderworld, "the Ancient Sky Ship", false);
			AddDungeon(267, 3, discoverUnderworld, "Argentrock Castle", false);
			AddDungeon(268, 3, discoverUnderworld, "the Depths of Carthax Lake", false);
			AddDungeon(269, 3, discoverUnderworld, "the Daemon's Crag", false);
			AddDungeon(270, 3, discoverUnderworld, "the Hall of the Mountain King", false);
			AddDungeon(271, 3, discoverUnderworld, "Morgaelin's Inferno", false);
			AddDungeon(272, 3, discoverUnderworld, "the Stygian Abyss", false);
			AddDungeon(273, 3, discoverUnderworld, "the Zealan Tombs", false);
			// ...

			#endregion Exploration - Dungeons (120 - 219)

			#region Exploration - Points of Interest (220 - 419)

			Categories.Add(new AchievementCategory(4, 1, "Points of Interest"));

			#endregion Exploration - Points of Interest

			#endregion Exploration - 1 to 999

			#region Hunting - 1000 to 1999
			Categories.Add(new AchievementCategory(1000, 0, "Hunting"));
			#endregion Hunting - 1000 to 1999

			#region Resource Gathering - 2000 to 2999

			Categories.Add(new AchievementCategory(2000, 0, "Resource Gathering"));

			#region Resource Gathering - Lumberjacking (2000 - 2249)

			Categories.Add(new AchievementCategory(2001, 2000, "Lumberjacking"));

			const int LOG_GRAPHIC = 0x1BE0; // Log
			achieve = AddHarvest(2000, 2001, LOG_GRAPHIC, null, 1, item => item is BaseLog, "Chop Chop!", "Harvest your first Log");
			achieve = AddHarvest(2001, 2001, LOG_GRAPHIC, achieve, 100, item => item is BaseLog, "Wooden You Know", "Harvest 100 Logs");
			achieve = AddHarvest(2002, 2001, LOG_GRAPHIC, achieve, 1000, item => item is BaseLog, "Lumbering Along", "Harvest 1,000 Logs");
			achieve = AddHarvest(2003, 2001, LOG_GRAPHIC, achieve, 10000, item => item is BaseLog, "Chop It Like It's Hot", "Harvest 10,000 Logs");
			achieve = AddHarvest(2004, 2001, LOG_GRAPHIC, achieve, 100000, item => item is BaseLog, "Timber!", "Harvest 100,000 Logs");

			#endregion Resource Gathering - Lumberjacking (2000 - 2249)

			#region Resource Gathering - Mining (2250 - 2499)

			Categories.Add(new AchievementCategory(2002, 2000, "Mining"));

			const int ORE_GRAPHIC = 0x19B9; // Ore
			achieve = AddHarvest(2250, 2002, ORE_GRAPHIC, null, 1, item => item is BaseOre, "Finding The Vein", "Mine your first Ore");
			achieve = AddHarvest(2251, 2002, ORE_GRAPHIC, achieve, 100, item => item is BaseOre, "Minor Miner", "Mine 100 Ore");
			achieve = AddHarvest(2252, 2002, ORE_GRAPHIC, achieve, 1000, item => item is BaseOre, "Ya dig?", "Mine 1,000 Ore");
			achieve = AddHarvest(2253, 2002, ORE_GRAPHIC, achieve, 10000, item => item is BaseOre, "What's Yours I Mine", "Mine 10,000 Ore");
			achieve = AddHarvest(2254, 2002, ORE_GRAPHIC, achieve, 100000, item => item is BaseOre, "Mining My Own Business", "Mine 100,000 Ore");

			#endregion Resource Gathering - Mining (2250 - 2499)

			#endregion Resource Gathering - 2000 to 2999

			#region Crafting - 3000 to 3999
			Categories.Add(new AchievementCategory(3000, 0, "Crafting"));
			Categories.Add(new AchievementCategory(3001, 3000, "Alchemy"));
			Categories.Add(new AchievementCategory(3002, 3000, "Blacksmith"));
			Categories.Add(new AchievementCategory(3003, 3000, "Bowcraft"));
			Categories.Add(new AchievementCategory(3004, 3000, "Carpentry"));
			Categories.Add(new AchievementCategory(3005, 3000, "Cooking"));
			Categories.Add(new AchievementCategory(3006, 3000, "Inscription"));
			Categories.Add(new AchievementCategory(3007, 3000, "Tailoring"));
			Categories.Add(new AchievementCategory(3008, 3000, "Tinkering"));
			#endregion Crafting - 3000 to 3999

			#region Character Development - 4000 to 4999
			Categories.Add(new AchievementCategory(4000, 0, "Character Development"));
			#endregion Character Development - 4000 to 4999

			#region Feats of Strength - 5000 to 5999
			const int DEMON_SKULL = 0x2251;
			Categories.Add(new AchievementCategory(5000, 0, "Feats of Strength"));
			Achievements.Add(new HunterAchievement(5001, 5000, DEMON_SKULL, true, null, 1, "Domo Arigato", "Destroyed the mechanical being of Exodus, the demonic automaton", 5, typeof(Exodus)));
			Achievements.Add(new HunterAchievement(5002, 5000, DEMON_SKULL, true, null, 1, "Like Thor, but better", "You've bested Jormungandr, the mighty Serpent of Midgard", 5, typeof(Jormungandr)));
			#endregion Feats of Strength - 5000 to 5999

			CommandSystem.Register("feats", AccessLevel.Player, new CommandEventHandler(OpenGumpCommand));
			CommandSystem.Register("achievement", AccessLevel.Player, new CommandEventHandler(OpenGumpCommand));
			CommandSystem.Register("achievements", AccessLevel.Player, new CommandEventHandler(OpenGumpCommand));
			EventSink.WorldSave += EventSink_WorldSave;
			LoadData();
		}

		private static DiscoverLandAchievement AddLand(int achievementId, int categoryId, string title, string description, Land land)
		{
			var achievement = new DiscoverLandAchievement(achievementId, categoryId, 0, false, null, title, description, 5, land)
			{
				ItemIcon = 0x14EB, // Map
			};
			Achievements.Add(achievement);

			return achievement;
		}

		private static DiscoveryAchievement AddTown(int achievementId, int categoryId, DiscoverLandAchievement prerequisite, string region)
		{
			var title = m_TextInfo.ToTitleCase(region);
			var landName = Lands.LandNameShort(prerequisite.Land);

			// Get Sextant Location
			string location;
			switch (region)
			{
				// Manual overrides
				case "the Village of Ravendark": location = "154° 27'S, 147° 14'W"; break;
				case "the Barako Mines": location = "174° 57'S, 169° 6'W"; break;
				case "the Lunar City of Dawn": location = "120° 11'N, 79° 52'E"; break;
				case "the Fort of Stonewall": location = "90° 29'N, 51° 28'W"; break;
				case "the Undercity of Umbra": location = "1° 15'N, 56° 57'E"; break;
				case "the Forgotten Lighthouse": location = "51° 29'N, 2° 44'W"; break;
				case "Kraken Reef Docks": location = "107° 15'S, 157° 30'E"; break;
				case "Savage Sea Docks": location = "118° 2'N, 154° 20'E"; break;
				case "Serpent Sail Docks": location = "137° 10'S, 4° 37'W"; break;
				case "Anchor Rock Docks": location = "16° 34'N, 154° 11'W"; break;

				// Automatically resolvable
				default:
					{
						Map place;
						int xc;
						int yc;
						location = Worlds.GetTown(0, region, out place, out xc, out yc);
						if (place == Map.Internal)
							Console.WriteLine("Failed to detect location for: {0}", region);

						break;
					}
			}

			var achievement = new DiscoveryAchievement(achievementId, categoryId, 0, false, prerequisite, title, null, 5, region)
			{
				ItemIcon = 0x22C9, // Mini house
				HideDesc = true,
				HiddenDesc = string.Format("{0} | Discover {1}", landName, region),
				Desc = !string.IsNullOrWhiteSpace(location)
					? string.Format("{0} | {1}", landName, location)
					: landName
			};

			Achievements.Add(achievement);

			return achievement;
		}

		private static DiscoveryAchievement AddDungeon(int achievementId, int categoryId, DiscoverLandAchievement prerequisite, string region, bool hasRelic = true)
		{
			var title = m_TextInfo.ToTitleCase(region);
			var landName = Lands.LandNameShort(prerequisite.Land);

			var relicQuestItem = hasRelic ? SomeRandomNote.GetRelicItem(region) : null;
			if (!string.IsNullOrWhiteSpace(relicQuestItem))
				relicQuestItem = m_TextInfo.ToTitleCase(relicQuestItem);

			string location;
			switch (region)
			{
				default:
					{
						string world;
						Map placer;
						int xc;
						int yc;
						Worlds.GetDungeonListing(region, out world, out location, out placer, out xc, out yc);
						if (placer == Map.Internal)
							Console.WriteLine("Failed to detect location for: {0}", region);
						else
							location = string.Format("{0} | {1}", landName, location);
						break;
					}
			}

			var achievement = new DiscoveryAchievement(achievementId, categoryId, 0, false, prerequisite, title, null, 5, region)
			{
				ItemIcon = 0x1856, // Skull with candle
				HideDesc = true,
				HiddenDesc = landName,
				Desc = string.IsNullOrWhiteSpace(relicQuestItem)
					? location
					: string.Format("{0} | {1}", location, relicQuestItem)
			};

			Achievements.Add(achievement);

			return achievement;
		}

		private static HarvestAchievement AddHarvest(int achievementId, int categoryId, int itemIcon, BaseAchievement prerequisite, int amount, Func<Item, bool> predicate, string title, string description)
		{
			var achievement = new HarvestAchievement(achievementId, categoryId, itemIcon, false, prerequisite, amount, title, description, 5, predicate);

			Achievements.Add(achievement);

			return achievement;
		}

		private static void LoadData()
		{
			Persistence.Deserialize(
				"Saves//Achievements//Achievements.bin",
				reader =>
				{
					int version = reader.ReadInt();

					if (version < 1)
					{
						int count = reader.ReadInt();
						for (int i = 0; i < count; ++i)
						{
							// Discard data, rebuild it later
							var serial = reader.ReadInt();
							var points = reader.ReadInt();
						}
					}
					else
					{
						int count = reader.ReadInt();
						for (int i = 0; i < count; ++i)
						{
							m_pointsTotal.Add(reader.ReadString(), reader.ReadInt());
						}
					}

					{
						int count = reader.ReadInt();

						for (int i = 0; i < count; ++i)
						{
							var id = reader.ReadString();
							var dict = new Dictionary<int, AchieveData>();
							int iCount = reader.ReadInt();
							if (iCount > 0)
							{
								for (int x = 0; x < iCount; x++)
								{
									dict.Add(reader.ReadInt(), new AchieveData(reader));
								}
							}

							m_featData.Add(id, dict);
						}
					}

					// Manually build points based off completed achievements
					if (version < 1)
					{
						var achievementsLookup = Achievements.ToDictionary(achievement => achievement.ID, achievement => achievement);

						foreach (var id in m_featData.Keys)
						{
							var dataByAchievementId = m_featData[id];
							var points = 0;
							foreach (var achievementId in dataByAchievementId.Keys)
							{
								var data = dataByAchievementId[achievementId];
								if (!data.IsComplete) continue;

								var achievement = achievementsLookup[achievementId];
								points += achievement.RewardPoints;
							}

							m_pointsTotal[id] = points;
						}
					}

					Console.WriteLine("[Achievements] Loaded: {0}", m_featData.Count);
				}
			);
		}

		private static void EventSink_WorldSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(
				"Saves//Achievements//Achievements.bin",
				writer =>
				{
					writer.Write(1); // version

					writer.Write(m_pointsTotal.Count);
					foreach (var kv in m_pointsTotal)
					{
						writer.Write(kv.Key);
						writer.Write(kv.Value);
					}

					writer.Write(m_featData.Count);
					foreach (var kv in m_featData)
					{
						writer.Write(kv.Key);

						writer.Write(kv.Value.Count);

						foreach (var ckv in kv.Value)
						{
							writer.Write(ckv.Key);
							ckv.Value.Serialize(writer);
						}
					}
				}
			);
		}

		public static void OpenGump(Mobile from, Mobile target)
		{
			var player = target as PlayerMobile;
			if (player == null) return;

			var id = TryGetId(player);
			if (id == null) return;

			var achieves = GetOrCreateFeatData(player);
			var total = GetPlayerPointsTotal(player);

			from.SendGump(new AchievementGump(achieves, total));
		}

		public static void OpenOtherGump(Mobile from, PlayerMobile target)
		{
			var fromPlayer = from as PlayerMobile;
			if (fromPlayer == null) return;

			var targetId = TryGetId(target);
			if (targetId == null) return;	

			var fromId = TryGetId(fromPlayer);
			if (fromId == null) return;

			var targetAchieves = GetOrCreateFeatData(target);
			var fromAchieves = GetOrCreateFeatData(fromPlayer);

			var total = GetPlayerPointsTotal(target);

			from.SendGump(new AchievementGump(targetAchieves, total, 1, -1, fromAchieves, target.Name));
		}

		[Usage("achievements"), Aliases("achievement", "feats")]
		[Description("Opens the Achievements gump for yourself or a targeted player")]
		private static void OpenGumpCommand(CommandEventArgs e)
		{
			e.Mobile.SendMessage("Target yourself or another player to view their feats.");
			e.Mobile.Target = new FeatsTarget();
		}

		internal static void SetAchievementStatus(PlayerMobile player, BaseAchievement ach, int progress)
		{
			if (player == null || player.Account == null) return;

			var id = TryGetId(player);
			if (id == null) return;

			var achieves = GetOrCreateFeatData(player);

			if (achieves.ContainsKey(ach.ID))
			{
				if (achieves[ach.ID].Progress >= ach.CompletionTotal) return;

				achieves[ach.ID].Progress += progress;
			}
			else
			{
				achieves.Add(ach.ID, new AchieveData() { Progress = progress });
			}

			if (achieves[ach.ID].Progress >= ach.CompletionTotal)
			{
				player.SendAsciiMessage("You have earned the achievement: {0}.", ach.Title);
				player.PlaySound(0x215); // Summon creature
				player.SendGump(new AchievementObtainedGump(ach), false);
				CustomEventSink.InvokeAchievementObtained(player, ach);
				achieves[ach.ID].CompletedOn = DateTime.UtcNow;

				AddPoints(player, ach.RewardPoints);

				if (ach.RewardItems != null && ach.RewardItems.Length > 0)
				{
					bool success = false;
					foreach (var itemType in ach.RewardItems)
					{
						try
						{
							var item = (Item)Activator.CreateInstance(itemType);
							if (!WeightOverloading.IsOverloaded(player))
								player.Backpack.DropItem(item);
							else
								player.BankBox.DropItem(item);
							success = true;
						}
						catch (Exception ex)
						{
							Console.WriteLine("Failed to create achievement reward ({0}). {1}", itemType, ex.Message);
						}
					}

					if (success)
						player.SendAsciiMessage("You have recieved an award for completing this achievment!");
				}
			}
		}
	}
}
