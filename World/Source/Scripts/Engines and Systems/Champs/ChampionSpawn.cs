using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using System.Collections.Generic;
using Server.Utilities;
using System.Linq;
using Server.Network;

namespace Server.Engines.CannedEvil
{
	public class ChampionSpawn : Item
	{
		public const int MAX_SPAWN_SIZE_MOD = 12;
		private const int MIN_KILLS_PER_LEVEL = 20;

		[CommandProperty(AccessLevel.GameMaster)]
		public int SpawnSzMod
		{
			get
			{
				return (m_SPawnSzMod < 1 || m_SPawnSzMod > MAX_SPAWN_SIZE_MOD) ? MAX_SPAWN_SIZE_MOD : m_SPawnSzMod;
			}
			private set
			{
				m_SPawnSzMod = (value < 1 || value > MAX_SPAWN_SIZE_MOD) ? MAX_SPAWN_SIZE_MOD : value;
			}
		}

		private int m_SPawnSzMod;

		[CommandProperty(AccessLevel.GameMaster)]
		public Difficulty SpawnDifficulty { get; private set; }

		private bool m_Active;
		private bool m_RandomizeType;
		private ChampionSpawnType m_Type;
		private List<Mobile> m_Creatures;
		private List<Item> m_RedSkulls;
		private List<Item> m_WhiteSkulls;
		private ChampionPlatform m_Platform;
		private ChampionAltar m_Altar;
		private int m_Kills;
		private BaseChampion m_Champion;
		private Rectangle2D m_SpawnArea;
		private ChampionSpawnRegion m_Region;
		private TimeSpan m_ExpireDelay;
		private DateTime m_ExpireTime;
		private Timer m_Timer;
		private IdolOfTheChampion m_Idol;
		private bool m_HasBeenAdvanced;
		private bool m_ConfinedRoaming;
		private Dictionary<Mobile, int> m_DamageEntries;

		public Dictionary<Mobile, int> DamageEntries { get { return m_DamageEntries; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Owner { get; private set; } // May be NULL

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ConfinedRoaming
		{
			get { return m_ConfinedRoaming; }
			set { m_ConfinedRoaming = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasBeenAdvanced
		{
			get { return m_HasBeenAdvanced; }
			set { m_HasBeenAdvanced = value; }
		}

		[Constructable]
		public ChampionSpawn(bool setInitialSpawnArea = true) : base(0xBD2)
		{
			Movable = false;
			Visible = false;

			m_Creatures = new List<Mobile>();
			m_RedSkulls = new List<Item>();
			m_WhiteSkulls = new List<Item>();

			m_Platform = new ChampionPlatform(this);
			m_Altar = new ChampionAltar(this);
			m_Idol = new IdolOfTheChampion(this);

			m_ExpireDelay = TimeSpan.FromMinutes(10.0);

			m_DamageEntries = new Dictionary<Mobile, int>();

			// Apparently this should only execute after Location changes
			if (setInitialSpawnArea)
				Timer.DelayCall(TimeSpan.Zero, new TimerCallback(SetInitialSpawnArea));
		}

		public static ChampionSpawn FindOwner(BaseCreature sourceMobile)
		{
			return WorldUtilities.ForEachItem<ChampionSpawn>(spawn => spawn.Active)
				.FirstOrDefault(spawn =>
				{
					if (sourceMobile is BaseChampion) return spawn.Champion == sourceMobile;

					return spawn.m_Creatures.Contains(sourceMobile);
				});
		}

		public void SetInitialSpawnArea()
		{
			//Previous default used to be 24;
			SpawnArea = new Rectangle2D(new Point2D(X - 24, Y - 24), new Point2D(X + 24, Y + 24));
		}

		public void UpdateRegion()
		{
			if (m_Region != null)
				m_Region.Unregister();

			if (!Deleted && this.Map != Map.Internal)
			{
				m_Region = new ChampionSpawnRegion(this);
				m_Region.Register();
			}

			/*
			if( m_Region == null )
			{
				m_Region = new ChampionSpawnRegion( this );
			}
			else
			{
				m_Region.Unregister();
				//Why doesn't Region allow me to set it's map/Area meself? ><
				m_Region = new ChampionSpawnRegion( this );
			}
			*/
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Kills
		{
			get
			{
				return m_Kills;
			}
			set
			{
				m_Kills = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Rectangle2D SpawnArea
		{
			get
			{
				return m_SpawnArea;
			}
			set
			{
				m_SpawnArea = value;
				InvalidateProperties();
				UpdateRegion();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan ExpireDelay
		{
			get
			{
				return m_ExpireDelay;
			}
			set
			{
				m_ExpireDelay = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime ExpireTime
		{
			get
			{
				return m_ExpireTime;
			}
			set
			{
				m_ExpireTime = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public ChampionSpawnType Type
		{
			get
			{
				return m_Type;
			}
			private set
			{
				m_Type = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Active
		{
			get
			{
				return m_Active;
			}
			set
			{
				if (value)
					Start();
				else
					Stop();

				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Champion
		{
			get
			{
				return m_Champion;
			}
			set
			{
				m_Champion = value as BaseChampion;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Level
		{
			get
			{
				return m_RedSkulls.Count;
			}
			set
			{
				for (int i = m_RedSkulls.Count - 1; i >= value; --i)
				{
					m_RedSkulls[i].Delete();
					m_RedSkulls.RemoveAt(i);
				}

				for (int i = m_RedSkulls.Count; i < value; ++i)
				{
					Item skull = new Item(0x1854);

					skull.Hue = 0x26;
					skull.Movable = false;
					skull.Light = LightType.Circle150;

					skull.MoveToWorld(GetRedSkullLocation(i), Map);

					m_RedSkulls.Add(skull);
				}

				InvalidateProperties();
			}
		}

		public int MaxKills
		{
			get
			{
				return Math.Max(MIN_KILLS_PER_LEVEL, (SpawnSzMod * (250 / MAX_SPAWN_SIZE_MOD)) - (Level * SpawnSzMod));
			}
		}

		public bool IsChampionSpawn(Mobile m)
		{
			return m is BaseCreature && ((BaseCreature)m).IsEphemeral && m_Creatures.Contains(m);
		}

		public void SetWhiteSkullCount(int val)
		{
			for (int i = m_WhiteSkulls.Count - 1; i >= val; --i)
			{
				m_WhiteSkulls[i].Delete();
				m_WhiteSkulls.RemoveAt(i);
			}

			for (int i = m_WhiteSkulls.Count; i < val; ++i)
			{
				Item skull = new Item(0x1854);

				skull.Movable = false;
				skull.Light = LightType.Circle150;

				skull.MoveToWorld(GetWhiteSkullLocation(i), Map);

				m_WhiteSkulls.Add(skull);

				Effects.PlaySound(skull.Location, skull.Map, 0x29);
				Effects.SendLocationEffect(new Point3D(skull.X + 1, skull.Y + 1, skull.Z), skull.Map, 0x3728, 10);
			}
		}

		public void Start(Mobile owner, ChampionSpawnType spawnType, int spawnSize, Difficulty monsterDifficulty)
		{
			if (!CanStart(owner)) return;

			Type = spawnType;
			SpawnSzMod = spawnSize;
			SpawnDifficulty = monsterDifficulty;

			Start(owner);
		}

		public void Start(Mobile owner = null)
		{
			if (!CanStart()) return;

			m_Active = true;
			m_HasBeenAdvanced = false;
			Owner = owner;

			if (m_Timer != null)
				m_Timer.Stop();

			m_DamageEntries.Clear();

			m_ExpireTime = DateTime.UtcNow + m_ExpireDelay;
			m_Timer = new SliceTimer(this);
			m_Timer.Start();

			if (m_Altar != null)
			{
				if (m_Champion != null)
					m_Altar.Hue = 0x26;
				else
					m_Altar.Hue = 0;
			}

			if (m_Platform != null)
				m_Platform.Hue = 0x452;
		}

		public bool CanStart(Mobile owner)
		{
			return CanStart() && owner != null;
		}

		public bool CanStart()
		{
			return !Active && !Deleted;
		}

		public bool CanStop(Mobile from)
		{
			if (!Active || Deleted) return false;

			return Owner == null || Owner == from;
		}

		public void Stop()
		{
			if (!m_Active || Deleted)
				return;

			m_Active = false;
			m_HasBeenAdvanced = false;
			Owner = null;
			m_Kills = 0;
			Level = 0;
			InvalidateProperties();
			SetWhiteSkullCount(0);

			if (m_Timer != null)
				m_Timer.Stop();

			m_Timer = null;

			if (m_Altar != null)
				m_Altar.Hue = 0;

			if (m_Platform != null)
				m_Platform.Hue = 0x497;
		}

		#region Scroll of Transcendence

		private ScrollofTranscendence CreateRandomSoT()
		{
			int level = Utility.RandomMinMax(1, 5);
			return ScrollofTranscendence.CreateRandom(level, level);
		}

		#endregion Scroll of Transcendence

		public static void GiveScrollTo(Mobile killer, SpecialScroll scroll)
		{
			if (scroll == null || killer == null)   //sanity
				return;

			if (scroll is ScrollofTranscendence)
				killer.SendLocalizedMessage(1094936); // You have received a Scroll of Transcendence!
			else
				killer.SendLocalizedMessage(1049524); // You have received a scroll of power!

			if (killer.Alive)
				killer.AddToBackpack(scroll);
			else
			{
				if (killer.Corpse != null && !killer.Corpse.Deleted)
					killer.Corpse.DropItem(scroll);
				else
					killer.AddToBackpack(scroll);
			}
		}

		public void OnSlice()
		{
			if (!m_Active || Deleted)
				return;

			if (m_Champion != null)
			{
				if (m_Champion.Deleted)
				{
					RegisterDamageTo(m_Champion);
					AwardArtifact(m_Champion.GetArtifact());

					if (m_Platform != null)
						m_Platform.Hue = 0x497;

					if (m_Altar != null)
					{
						m_Altar.Hue = 0;
					}

					m_Champion = null;
					Stop();
					Cleanup();

					// Broadcast the final gump to all players who have done damage.
					foreach (Mobile m in DamageEntries.Keys)
					{
						if (m.NetState == null) continue;

						SendGump(m);
					}
				}
			}
			else
			{
				int kills = m_Kills;

				for (int i = 0; i < m_Creatures.Count; ++i)
				{
					Mobile m = m_Creatures[i];

					if (m.Deleted)
					{
						if (m.Corpse != null && !m.Corpse.Deleted)
						{
							((Corpse)m.Corpse).BeginDecay(TimeSpan.FromMinutes(1));
						}
						m_Creatures.RemoveAt(i);
						--i;
						++m_Kills;

						Mobile killer = MobileUtilities.TryGetKillingPlayer(m);

						RegisterDamageTo(m);

						if (killer is PlayerMobile)
						{
							#region Scroll of Transcendence

							if (Utility.RandomDouble() < 0.001)
							{
								killer.SendLocalizedMessage(1094936); // You have received a Scroll of Transcendence!
								ScrollofTranscendence SoTT = CreateRandomSoT();
								killer.AddToBackpack(SoTT);
							}

							#endregion Scroll of Transcendence
						}
					}
				}

				// Only really needed once.
				if (m_Kills > kills)
					InvalidateProperties();

				double n = m_Kills / (double)MaxKills;
				int p = (int)(n * 100);

				if (p >= 90)
					AdvanceLevel();
				else if (p > 0)
					SetWhiteSkullCount(p / 20);

				if (DateTime.UtcNow >= m_ExpireTime)
					Expire();

				Respawn();
			}
		}

		public void AdvanceLevel()
		{
			m_ExpireTime = DateTime.UtcNow + m_ExpireDelay;

			if (Level < 16)
			{
				m_Kills = 0;
				++Level;
				InvalidateProperties();
				SetWhiteSkullCount(0);
				switch (Level)
				{
					case 4: AnnounceRegionMessage("The air grows heavy with an unnatural stillness..."); break; // 25%
					case 8: AnnounceRegionMessage("Thick clouds gather in the sky..."); break; // 50%
					case 12: AnnounceRegionMessage("The very ground trembles beneath your feet..."); break; // 75%
					case 15: AnnounceRegionMessage("The boundaries between worlds grow thin..."); break; // n - 1
				}

				if (m_Altar != null)
				{
					Effects.PlaySound(m_Altar.Location, m_Altar.Map, 0x29);
					Effects.SendLocationEffect(new Point3D(m_Altar.X + 1, m_Altar.Y + 1, m_Altar.Z), m_Altar.Map, 0x3728, 10);
				}
			}
			else
			{
				SpawnChampion();
				if (m_Champion != null)
					AnnounceRegionMessage("Reality fractures as " + m_Champion.Name + " awakens!");
			}
		}

		private void AnnounceRegionMessage(string message)
		{
			foreach (Mobile m in m_Region.GetMobiles())
			{
				if (m is PlayerMobile && m.NetState != null)
					m.LocalOverheadMessage(MessageType.Regular, LabelColors.PALE_RED, true, message);
			}
		}

		public void SpawnChampion()
		{
			if (m_Altar != null)
				m_Altar.Hue = 0x26;

			if (m_Platform != null)
				m_Platform.Hue = 0x452;

			m_Kills = 0;
			Level = 0;
			InvalidateProperties();
			SetWhiteSkullCount(0);

			try
			{
				m_Champion = Activator.CreateInstance(ChampionSpawnInfo.GetInfo(m_Type).Champion) as BaseChampion;
				m_Champion.GoldPercent = ChampionRewards.GetGoldPercent(SpawnSzMod, SpawnDifficulty);
				m_Champion.BossItemRewardChance = ChampionRewards.GetBossItemDropChance(SpawnSzMod, SpawnDifficulty);
				m_Champion.ArtifactRewardChance = ChampionRewards.GetArtifactDropChance(SpawnSzMod, SpawnDifficulty);
				m_Champion.TreasureChestRewardChance = ChampionRewards.GetTreasureChestDropChance(SpawnSzMod, SpawnDifficulty);
				m_Champion.PowerscrollRewardAmount = ChampionRewards.GetPowerscrollDropCount(SpawnSzMod, SpawnDifficulty);
				m_Champion.MoveToWorld(new Point3D(X, Y, Z - 15), Map);
				m_Champion.OnAfterSpawn();
				BaseCreature.BeefUp(m_Champion, SpawnDifficulty, false);
			}
			catch { }
		}

		public void Respawn()
		{
			if (!m_Active || Deleted || m_Champion != null)
				return;

			while (m_Creatures.Count < (SpawnSzMod * (200 / MAX_SPAWN_SIZE_MOD)) - (GetSubLevel() * SpawnSzMod * (40 / MAX_SPAWN_SIZE_MOD)))
			{
				Mobile m = Spawn();

				if (m == null)
					return;

				Point3D loc = GetSpawnLocation();

				// Allow creatures to turn into Paragons at Ilshenar champions.
				m.OnBeforeSpawn(loc, Map);

				m_Creatures.Add(m);
				m.MoveToWorld(loc, Map);

				m.OnAfterSpawn();

				if (m is BaseCreature)
				{
					BaseCreature bc = m as BaseCreature;
					BaseCreature.BeefUp(bc, SpawnDifficulty, false);
					bc.Tamable = false;
					bc.IsEphemeral = true;
					if (bc.FightMode == FightMode.Aggressor) bc.FightMode = FightMode.Closest;

					if (!m_ConfinedRoaming)
					{
						bc.Home = this.Location;
						bc.RangeHome = (int)(Math.Sqrt(m_SpawnArea.Width * m_SpawnArea.Width + m_SpawnArea.Height * m_SpawnArea.Height) / 2);
					}
					else
					{
						bc.Home = bc.Location;

						Point2D xWall1 = new Point2D(m_SpawnArea.X, bc.Y);
						Point2D xWall2 = new Point2D(m_SpawnArea.X + m_SpawnArea.Width, bc.Y);
						Point2D yWall1 = new Point2D(bc.X, m_SpawnArea.Y);
						Point2D yWall2 = new Point2D(bc.X, m_SpawnArea.Y + m_SpawnArea.Height);

						double minXDist = Math.Min(bc.GetDistanceToSqrt(xWall1), bc.GetDistanceToSqrt(xWall2));
						double minYDist = Math.Min(bc.GetDistanceToSqrt(yWall1), bc.GetDistanceToSqrt(yWall2));

						bc.RangeHome = (int)Math.Min(minXDist, minYDist);
					}
				}
			}
		}

		public Point3D GetSpawnLocation()
		{
			Map map = Map;

			if (map == null)
				return Location;

			// Try 20 times to find a spawnable location.
			for (int i = 0; i < 20; i++)
			{
				/*
				int x = Location.X + (Utility.Random( (m_SpawnRange * 2) + 1 ) - m_SpawnRange);
				int y = Location.Y + (Utility.Random( (m_SpawnRange * 2) + 1 ) - m_SpawnRange);
				*/

				int x = Utility.Random(m_SpawnArea.X, m_SpawnArea.Width);
				int y = Utility.Random(m_SpawnArea.Y, m_SpawnArea.Height);

				int z = Map.GetAverageZ(x, y);

				if (Map.CanSpawnMobile(new Point2D(x, y), z))
					return new Point3D(x, y, z);

				/* try @ platform Z if map z fails */
				else if (Map.CanSpawnMobile(new Point2D(x, y), m_Platform.Location.Z))
					return new Point3D(x, y, m_Platform.Location.Z);
			}

			return Location;
		}

		private const int Level1 = 4;  // First spawn level from 0-4 red skulls
		private const int Level2 = 8;  // Second spawn level from 5-8 red skulls
		private const int Level3 = 12; // Third spawn level from 9-12 red skulls

		public int GetSubLevel()
		{
			int level = this.Level;

			if (level <= Level1)
				return 0;
			else if (level <= Level2)
				return 1;
			else if (level <= Level3)
				return 2;

			return 3;
		}

		public Mobile Spawn()
		{
			Type[][] types = ChampionSpawnInfo.GetInfo(m_Type).SpawnTypes;

			int v = GetSubLevel();

			if (v >= 0 && v < types.Length)
				return Spawn(types[v]);

			return null;
		}

		public Mobile Spawn(params Type[] types)
		{
			try
			{
				return Activator.CreateInstance(types[Utility.Random(types.Length)]) as Mobile;
			}
			catch
			{
				return null;
			}
		}

		public void Expire()
		{
			m_Kills = 0;

			if (Level == 0 && m_WhiteSkulls.Count == 0)
			{
				Stop();
				Cleanup();
				return;
			}

			if (m_WhiteSkulls.Count == 0)
			{
				// They didn't even get 20%, go back a level

				if (Level > 0)
					--Level;

				InvalidateProperties();
			}
			else
			{
				SetWhiteSkullCount(0);
			}

			m_ExpireTime = DateTime.UtcNow + m_ExpireDelay;
		}

		public TimeSpan GetExpirationTimeRemaining()
		{
			return m_ExpireTime - DateTime.UtcNow;
		}

		public Point3D GetRedSkullLocation(int index)
		{
			int x, y;

			if (index < 5)
			{
				x = index - 2;
				y = -2;
			}
			else if (index < 9)
			{
				x = 2;
				y = index - 6;
			}
			else if (index < 13)
			{
				x = 10 - index;
				y = 2;
			}
			else
			{
				x = -2;
				y = 14 - index;
			}

			return new Point3D(X + x, Y + y, Z - 15);
		}

		public Point3D GetWhiteSkullLocation(int index)
		{
			int x, y;

			switch (index)
			{
				default:
				case 0: x = -1; y = -1; break;
				case 1: x = 1; y = -1; break;
				case 2: x = 1; y = 1; break;
				case 3: x = -1; y = 1; break;
			}

			return new Point3D(X + x, Y + y, Z - 15);
		}

		public override void AddNameProperty(ObjectPropertyList list)
		{
			list.Add("champion spawn");
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (m_Active)
			{
				list.Add(1060742); // active
				list.Add(1060658, "Type\t{0}", m_Type); // ~1_val~: ~2_val~
				list.Add(1060659, "Level\t{0}", Level); // ~1_val~: ~2_val~
				list.Add(1060660, "Kills\t{0} of {1} ({2:F1}%)", m_Kills, MaxKills, 100.0 * ((double)m_Kills / MaxKills)); // ~1_val~: ~2_val~
																														   //list.Add( 1060661, "Spawn Range\t{0}", m_SpawnRange ); // ~1_val~: ~2_val~
			}
			else
			{
				list.Add(1060743); // inactive
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			if (m_Active)
				LabelTo(from, "{0} (Active; Level: {1}; Kills: {2}/{3})", m_Type, Level, m_Kills, MaxKills);
			else
				LabelTo(from, "{0} (Inactive)", m_Type);
		}

		public override void OnDoubleClick(Mobile from)
		{
			from.SendGump(new PropertiesGump(from, this));
		}

		public override void OnLocationChange(Point3D oldLoc)
		{
			if (Deleted)
				return;

			if (m_Platform != null)
				m_Platform.Location = new Point3D(X, Y, Z - 20);

			if (m_Altar != null)
				m_Altar.Location = new Point3D(X, Y, Z - 15);

			if (m_Idol != null)
				m_Idol.Location = new Point3D(X, Y, Z - 15);

			if (m_RedSkulls != null)
			{
				for (int i = 0; i < m_RedSkulls.Count; ++i)
					m_RedSkulls[i].Location = GetRedSkullLocation(i);
			}

			if (m_WhiteSkulls != null)
			{
				for (int i = 0; i < m_WhiteSkulls.Count; ++i)
					m_WhiteSkulls[i].Location = GetWhiteSkullLocation(i);
			}

			m_SpawnArea.X += Location.X - oldLoc.X;
			m_SpawnArea.Y += Location.Y - oldLoc.Y;

			UpdateRegion();
		}

		public override void OnMapChange()
		{
			if (Deleted)
				return;

			if (m_Platform != null)
				m_Platform.Map = Map;

			if (m_Altar != null)
				m_Altar.Map = Map;

			if (m_Idol != null)
				m_Idol.Map = Map;

			if (m_RedSkulls != null)
			{
				for (int i = 0; i < m_RedSkulls.Count; ++i)
					m_RedSkulls[i].Map = Map;
			}

			if (m_WhiteSkulls != null)
			{
				for (int i = 0; i < m_WhiteSkulls.Count; ++i)
					m_WhiteSkulls[i].Map = Map;
			}

			UpdateRegion();
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if (m_Platform != null)
				m_Platform.Delete();

			if (m_Altar != null)
				m_Altar.Delete();

			if (m_Idol != null)
				m_Idol.Delete();

			Cleanup();

			Stop();

			UpdateRegion();
		}

		public void Cleanup()
		{
			if (m_RedSkulls != null)
			{
				for (int i = 0; i < m_RedSkulls.Count; ++i)
					m_RedSkulls[i].Delete();

				m_RedSkulls.Clear();
			}

			if (m_WhiteSkulls != null)
			{
				for (int i = 0; i < m_WhiteSkulls.Count; ++i)
					m_WhiteSkulls[i].Delete();

				m_WhiteSkulls.Clear();
			}

			if (m_Creatures != null)
			{
				for (int i = 0; i < m_Creatures.Count; ++i)
				{
					Mobile mob = m_Creatures[i];

					if (!mob.Player)
						mob.Delete();
				}

				m_Creatures.Clear();
			}

			if (m_Champion != null && !m_Champion.Player)
				m_Champion.Delete();
		}

		public ChampionSpawn(Serial serial) : base(serial)
		{
		}

		public virtual void RegisterDamageTo(Mobile m)
		{
			if (m == null)
				return;

			foreach (DamageEntry de in m.DamageEntries)
			{
				if (de.HasExpired)
					continue;

				Mobile damager = de.Damager;

				Mobile master = damager.GetDamageMaster(m);

				if (master != null)
					damager = master;

				RegisterDamage(damager, de.DamageGiven);
			}
		}

		public void RegisterDamage(Mobile from, int amount)
		{
			if (from == null || !from.Player)
				return;

			if (m_DamageEntries.ContainsKey(from))
				m_DamageEntries[from] += amount;
			else
				m_DamageEntries.Add(from, amount);
		}

		public void AwardArtifact(Item artifact)
		{
			if (artifact == null)
				return;

			int totalDamage = 0;

			Dictionary<Mobile, int> validEntries = new Dictionary<Mobile, int>();

			foreach (KeyValuePair<Mobile, int> kvp in m_DamageEntries)
			{
				if (IsEligible(kvp.Key, artifact))
				{
					validEntries.Add(kvp.Key, kvp.Value);
					totalDamage += kvp.Value;
				}
			}

			int randomDamage = Utility.RandomMinMax(1, totalDamage);

			totalDamage = 0;

			foreach (KeyValuePair<Mobile, int> kvp in validEntries)
			{
				totalDamage += kvp.Value;

				if (totalDamage >= randomDamage)
				{
					GiveArtifact(kvp.Key, artifact);
					return;
				}
			}

			artifact.Delete();
		}

		public void GiveArtifact(Mobile to, Item artifact)
		{
			if (to == null || artifact == null)
				return;

			Container pack = to.Backpack;

			if (pack == null || !pack.TryDropItem(to, artifact, false))
				artifact.Delete();
			else
				to.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
		}

		public bool IsEligible(Mobile m, Item Artifact)
		{
			return m.Player && m.Alive && m.Region != null && m.Region == m_Region && m.Backpack != null && m.Backpack.CheckHold(m, Artifact, false);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)8); // version

			writer.Write(Owner);
			writer.Write((int)SpawnDifficulty);
			writer.Write((int)m_SPawnSzMod);
			writer.Write(m_DamageEntries.Count);
			foreach (KeyValuePair<Mobile, int> kvp in m_DamageEntries)
			{
				writer.Write(kvp.Key);
				writer.Write(kvp.Value);
			}

			writer.Write(m_ConfinedRoaming);
			writer.WriteItem<IdolOfTheChampion>(m_Idol);
			writer.Write(m_HasBeenAdvanced);
			writer.Write(m_SpawnArea);

			writer.Write(m_RandomizeType);

			//			writer.Write( m_SpawnRange );
			writer.Write(m_Kills);

			writer.Write((bool)m_Active);
			writer.Write((int)m_Type);
			writer.Write(m_Creatures, true);
			writer.Write(m_RedSkulls, true);
			writer.Write(m_WhiteSkulls, true);
			writer.WriteItem<ChampionPlatform>(m_Platform);
			writer.WriteItem<ChampionAltar>(m_Altar);
			writer.Write(m_ExpireDelay);
			writer.WriteDeltaTime(m_ExpireTime);
			writer.Write(m_Champion);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			m_DamageEntries = new Dictionary<Mobile, int>();

			int version = reader.ReadInt();

			switch (version)
			{
				case 8:
					{
						Owner = reader.ReadMobile();
						goto case 7;
					}
				case 7:
					{
						SpawnDifficulty = (Difficulty)reader.ReadInt();
						goto case 6;
					}
				case 6:
					{
						m_SPawnSzMod = reader.ReadInt();
						goto case 5;
					}
				case 5:
					{
						int entries = reader.ReadInt();
						Mobile m;
						int damage;
						for (int i = 0; i < entries; ++i)
						{
							m = reader.ReadMobile();
							damage = reader.ReadInt();

							if (m == null)
								continue;

							m_DamageEntries.Add(m, damage);
						}

						goto case 4;
					}
				case 4:
					{
						m_ConfinedRoaming = reader.ReadBool();
						m_Idol = reader.ReadItem<IdolOfTheChampion>();
						m_HasBeenAdvanced = reader.ReadBool();

						goto case 3;
					}
				case 3:
					{
						m_SpawnArea = reader.ReadRect2D();

						goto case 2;
					}
				case 2:
					{
						m_RandomizeType = reader.ReadBool();

						goto case 1;
					}
				case 1:
					{
						if (version < 3)
						{
							int oldRange = reader.ReadInt();

							m_SpawnArea = new Rectangle2D(new Point2D(X - oldRange, Y - oldRange), new Point2D(X + oldRange, Y + oldRange));
						}

						m_Kills = reader.ReadInt();

						goto case 0;
					}
				case 0:
					{
						if (version < 1)
							m_SpawnArea = new Rectangle2D(new Point2D(X - 24, Y - 24), new Point2D(X + 24, Y + 24));    //Default was 24

						bool active = reader.ReadBool();
						m_Type = (ChampionSpawnType)reader.ReadInt();
						m_Creatures = reader.ReadStrongMobileList();
						m_RedSkulls = reader.ReadStrongItemList();
						m_WhiteSkulls = reader.ReadStrongItemList();
						m_Platform = reader.ReadItem<ChampionPlatform>();
						m_Altar = reader.ReadItem<ChampionAltar>();
						m_ExpireDelay = reader.ReadTimeSpan();
						m_ExpireTime = reader.ReadDeltaTime();
						m_Champion = reader.ReadMobile<BaseChampion>();

						if (version < 4)
						{
							m_Idol = new IdolOfTheChampion(this);
							m_Idol.MoveToWorld(new Point3D(X, Y, Z - 15), Map);
						}

						if (m_Platform == null || m_Altar == null || m_Idol == null)
							Delete();
						else if (active)
							Start(Owner);

						break;
					}
			}

			Timer.DelayCall(TimeSpan.Zero, new TimerCallback(UpdateRegion));
		}

		public void SendGump(Mobile from)
		{
			from.CloseGump(typeof(ChampionSpawnInfoGump));
			from.SendGump(new ChampionSpawnInfoGump(from, this));
		}
	}

	public class ChampionSpawnRegion : BaseRegion
	{
		public override bool YoungProtected
		{ get { return false; } }

		private ChampionSpawn m_Spawn;

		public ChampionSpawn ChampionSpawn
		{
			get { return m_Spawn; }
		}

		public ChampionSpawnRegion(ChampionSpawn spawn) : base(null, spawn.Map, Region.Find(spawn.Location, spawn.Map), spawn.SpawnArea)
		{
			m_Spawn = spawn;
		}

		public override bool AllowHousing(Mobile from, Point3D p)
		{
			return false;
		}

		public override bool OnBeforeDeath(Mobile m)
		{
			if (m is BaseCreature)
			{
				if (m_Spawn.IsChampionSpawn(m) && m is BaseChampion == false && MobileUtilities.TryGetMasterPlayer(m) == null)
				{
					// Delete spawned mobs instead of creating corpses
					m.Delete();
					return false;
				}
			}

			return base.OnBeforeDeath(m);
		}

		public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
		{
			base.AlterLightLevel(m, ref global, ref personal);
			global = Math.Max(global, 1 + m_Spawn.Level);   //This is a guesstimate.  TODO: Verify & get exact values // OSI testing: at 2 red skulls, light = 0x3 ; 1 red = 0x3.; 3 = 8; 9 = 0xD 8 = 0xD 12 = 0x12 10 = 0xD
		}
	}

	public class IdolOfTheChampion : Item
	{
		private ChampionSpawn m_Spawn;

		public ChampionSpawn Spawn
		{ get { return m_Spawn; } }

		public override string DefaultName
		{
			get { return "Idol of the Champion"; }
		}

		public IdolOfTheChampion(ChampionSpawn spawn) : base(0x1F18)
		{
			m_Spawn = spawn;
			Movable = false;
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if (m_Spawn != null)
				m_Spawn.Delete();
		}

		public IdolOfTheChampion(Serial serial) : base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			m_Spawn.SendGump(from);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version

			writer.Write(m_Spawn);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						m_Spawn = reader.ReadItem() as ChampionSpawn;

						if (m_Spawn == null)
							Delete();

						break;
					}
			}
		}
	}
}