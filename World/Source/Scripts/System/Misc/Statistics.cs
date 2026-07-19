using System;
using System.Diagnostics;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;
using Server.Gumps;
using Server.Guilds;
using Server.Mobiles;
using Server.Network;
using Server.Commands;
using Server.Accounting;
using Server.Engines.PartySystem;
using System.IO;
using System.Text;

namespace Server
{
    class Statistics
    {
        public class Config
        {
            public static bool Enabled = true;                            // Is this system enabled?
            public static bool ConsoleReport = false;                     // Should we report statistics on console?
            public static int Interval = 1;                               // What's the statistics update interval, in minutes?
            public static AccessLevel CanSeeStats = AccessLevel.Player;   // What's the level required to see statistics in-game?
            public static AccessLevel CanUpdateStats = AccessLevel.Seer;  // What's the level required to update statistics in-game?
        }

        public static TimeSpan ShardAge { get { return m_ShardAge; } }
        public static TimeSpan Uptime { get { return m_Uptime; } }
        public static TimeSpan TotalGameTime { get { return m_TotalGameTime; } }
        public static DateTime LastRestart { get { return m_LastRestart; } }
        public static DateTime LastStatsUpdate { get { return m_LastStatsUpdate; } }
        public static int ActiveAccounts { get { return m_ActiveAccounts; } }
        public static int ActiveStaffMembers { get { return m_ActiveStaffMembers; } }
        public static int ActiveGuilds { get { return m_ActiveGuilds; } }
        public static int ActiveParties { get { return m_ActiveParties; } }
        public static int PlayersInParty { get { return m_PlayersInParty; } }
        public static int PlayerHouses { get { return m_PlayerHouses; } }
        public static int PlayerGold { get { return m_PlayerGold; } }
        public static int PlayersOnline { get { return m_PlayersOnline; } }
        public static int StaffOnline { get { return m_StaffOnline; } }

        private static TimeSpan m_ShardAge;
        private static TimeSpan m_Uptime;
        private static TimeSpan m_TotalGameTime;
        private static DateTime m_LastRestart;
        private static DateTime m_LastStatsUpdate;
        private static int m_ActiveAccounts;
        private static int m_ActiveStaffMembers;
        private static int m_ActiveGuilds;
        private static int m_ActiveParties;
        private static int m_PlayersInParty;
        private static int m_PlayerHouses;
        private static int m_PlayerGold;
        private static int m_PlayersOnline;
        private static int m_StaffOnline;

        private static List<String> StatsList = new List<String>();

        public static void Initialize()
        {
            if (Config.Enabled)
            {
                CommandSystem.Register("Statistics", Config.CanSeeStats, new CommandEventHandler(SeeStatistics_OnCommand));
                CommandSystem.Register("UpdateStatistics", Config.CanUpdateStats, new CommandEventHandler(UpdateStatistics_OnCommand));
                Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMinutes(Config.Interval), CollectStats);
            }
        }

        [Usage("Statistics")]
        [Description("Shows shard statistics.")]
        private static void SeeStatistics_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;
            m.CloseGump(typeof(StatisticsGump));
            m.SendGump(new StatisticsGump( m, 0 ));
        }

        [Usage("UpdateStatistics")]
        [Description("Updates shard statistics.")]
        private static void UpdateStatistics_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;
            Console.WriteLine("Statistics: {0} ({1}) has updated statistics in-game.", m.RawName, m.AccessLevel.ToString());
            CollectStats();
            m.SendMessage(68, "Statistics updated successful.");
            m.CloseGump(typeof(StatisticsGump));
            m.SendGump(new StatisticsGump( m, 0 ));
        }

        private static void CollectStats()
        {
            Stopwatch watch = Stopwatch.StartNew();

            m_StaffOnline = 0;
            m_PlayersOnline = 0;
            m_PlayersInParty = 0;
            m_PlayerHouses = 0;
            m_PlayerGold = 0;
            m_ActiveStaffMembers = 0;
            m_TotalGameTime = TimeSpan.Zero;
            StatsList.Clear();

            List<Party> parties = new List<Party>();
            DateTime shardCreation = DateTime.Now;

            foreach (Item i in World.Items.Values)
            {
                if (i is Multis.BaseHouse)
                    m_PlayerHouses++;
            }

            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is PlayerMobile)
                {
                    if (m.AccessLevel == AccessLevel.Player)
                    {
                        m_PlayerGold += m.TotalGold + GetPlayerInfo.GetBankedGold( m );
                    }
                    else
                    {
                        m_ActiveStaffMembers++;
                    }
                }
            }

            foreach (NetState ns in NetState.Instances)
            {
                Mobile m = ns.Mobile;

                if (m != null)
                {
                    if (m.AccessLevel == AccessLevel.Player)
                    {
                        m_PlayersOnline++;
                    }
                    else
                    {
                        m_StaffOnline++;
                    }

                    Party p = Party.Get(m);

                    if (p != null)
                    {
                        m_PlayersInParty++;

                        if (!parties.Contains(p))
                            parties.Add(p);
                    }
                }
            }

            foreach (Account a in Accounts.GetAccounts())
            {
                m_TotalGameTime += a.TotalGameTime;

                if (a.Created < shardCreation)
                    shardCreation = a.Created;
            }

            m_ShardAge = DateTime.Now - shardCreation;
            m_Uptime = DateTime.Now - Clock.ServerStart;
            m_LastRestart = Clock.ServerStart;
            m_LastStatsUpdate = DateTime.Now;
            m_ActiveAccounts = Accounts.Count;
            m_ActiveGuilds = Guild.List.Count;
            m_ActiveParties = parties.Count;

			string statUpdate = "Statistics Updated: " + m_LastStatsUpdate + "";

            StatsList.Add(String.Format(Server.Misc.ChangeLog.Version()));
			StatsList.Add(String.Format(statUpdate));
            StatsList.Add(String.Format("World Age: {0:n0} days, {1:n0} hours and {2:n0} minutes", m_ShardAge.Days, m_ShardAge.Hours, m_ShardAge.Minutes));
            StatsList.Add(String.Format("<BR>Total Game Time: {0:n0} hours and {1:n0} minutes", m_TotalGameTime.TotalHours, m_TotalGameTime.Minutes));
            StatsList.Add(String.Format("<BR>Last Restart: {0}", m_LastRestart));
            StatsList.Add(String.Format("<BR>Uptime: {0:n0} days, {1:n0} hours and {2:n0} minutes", m_Uptime.Days, m_Uptime.Hours, m_Uptime.Minutes));
            StatsList.Add(String.Format("<BR>Active Accounts: {0:n0} [{1:n0} Players Online]", m_ActiveAccounts, m_PlayersOnline));
            StatsList.Add(String.Format("Active Staff Members: {0:n0} [{1:n0} Online]", m_ActiveStaffMembers, m_StaffOnline));
            StatsList.Add(String.Format("Active Parties: {0:n0} [{1:n0} Players]", m_ActiveParties, m_PlayersInParty));
            StatsList.Add(String.Format("Active Guilds: {0:n0}", m_ActiveGuilds));
            StatsList.Add(String.Format("<BR>Player Houses: {0:n0}", m_PlayerHouses));
            StatsList.Add(String.Format("Player Gold: {0:n0}", m_PlayerGold));

            watch.Stop();

            if (Config.ConsoleReport)
            {
                Console.WriteLine("");
                Console.WriteLine("===========================================");
                Console.WriteLine("| Statistics Report | {0} |", m_LastStatsUpdate);
                Console.WriteLine("===========================================");
                Console.WriteLine("");
                foreach (String stat in StatsList)
                {
                    Console.WriteLine("* {0}", stat);
                }
                Console.WriteLine("");
                Console.WriteLine("=============================");
                Console.WriteLine("| Generated in {0:F2} seconds |", watch.Elapsed.TotalSeconds);
                Console.WriteLine("=============================");
                Console.WriteLine("");

            }
        }

        public class StatisticsGump : Gump
        {
			public int m_Origin;

            public StatisticsGump( Mobile from, int origin ) : base( 50, 50 )
            {
				m_Origin = origin;

				string color = "#9db1b7";

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 9549, Server.Misc.PlayerSettings.GetGumpHue( from ));
				AddHtml( 13, 12, 434, 20, @"<BODY><BASEFONT Color=" + color + ">" + MySettings.S_ServerName + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 14, 43, 581, 310, @"<BODY><BASEFONT Color=" + color + ">" + String.Join("<br>", StatsList.ToArray()) + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(567, 11, 4017, 4017, 0, GumpButtonType.Reply, 0);
            }

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				Mobile from = sender.Mobile;
				if ( m_Origin > 0 ){ from.SendSound( 0x4A ); from.SendGump( new Server.Engines.Help.HelpGump( from, m_Origin ) ); }
			}
        }
    }
}
