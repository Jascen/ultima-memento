using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Items;

namespace Server.Misc
{
    class RegionMusic
    {
		public static void MusicRegion( Mobile from, Region reg )
		{
			if ( from is PlayerMobile )
			{
				string tunes = ((PlayerMobile)from).Preferences.CharMusical;
				MusicName toPlay = LandMusic[Utility.Random(LandMusic.Length)];

				bool switchSongs = false;

				if ( reg.IsPartOf( "the Tomb of Malak the Syth Lord" ) )
				{
					toPlay = SythMusic[Utility.Random(SythMusic.Length)]; switchSongs = true;
				}
				else if ( reg.IsPartOf( "the Tomb of Zoda the Jedi Master" ) )
				{
					toPlay = JediMusic[Utility.Random(JediMusic.Length)]; switchSongs = true;
				}
				else if ( reg.IsPartOf( "the Ancient Sky Ship" ) || reg.IsPartOf( "the Ancient Crash Site" ) )
				{
					toPlay = SpaceMusic[Utility.Random(SpaceMusic.Length)]; switchSongs = true;
				}
				else if ( reg.IsPartOf( "Shimazu's Dojo" ) || reg.IsPartOf( "the Dojo" ) )
				{
					toPlay = MusicName.Dojo; switchSongs = true;
				}
				else if ( Server.Misc.MusicPlaylistFunctions.GetPlaylistSetting( from, 59 ) > 0 )
				{
					Server.Misc.MusicPlaylistFunctions.PickRandomSong( from );
				}
				else
				{
					if ( reg.IsPartOf( "the Castle of Knowledge" ) )
					{
						toPlay = MusicName.CastleKnowledge; switchSongs = true;
					}
					else if ( reg.IsPartOf( "the Buccaneer's Den" ) || reg.IsPartOf( "Shipwreck Grotto" ) || reg.IsPartOf( "Barnacled Cavern" ) )
					{
						toPlay = PirateMusic[Utility.Random(PirateMusic.Length)];
					}
					else if ( Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) == "the Underworld" )
					{
						toPlay = UnderworldMusic[Utility.Random(UnderworldMusic.Length)]; switchSongs = true;
					}
					else if ( reg.IsPartOf( "the Lodoria Cemetery" ) )
					{
						toPlay = MusicName.Seeking; switchSongs = true;
					}
					else if ( reg.IsPartOf( "the British Family Cemetery" ) )
					{
						toPlay = MusicName.Seeking; switchSongs = true;
					}
					else if ( reg.IsPartOf( "the Tower" ) )
					{
						toPlay = MusicName.Adventure; switchSongs = true;
					}
					else if ( reg is PublicRegion )
					{
						toPlay = CaveMusic[Utility.Random(CaveMusic.Length)]; switchSongs = true;

						if ( reg.IsPartOf( "the Druid's Glade" ) ){ toPlay = MageList[Utility.Random(MageList.Length)]; }
						else if ( reg.IsPartOf( "the Black Magic Guild" ) ){ toPlay = NecromancerMusic[Utility.Random(NecromancerMusic.Length)]; }
						else if ( reg.IsPartOf( "the Tavern" ) ){ toPlay = InnList[Utility.Random(InnList.Length)]; }
						else if ( reg.IsPartOf( "the Thieves Guild" ) ){ toPlay = InnList[Utility.Random(InnList.Length)]; }
						else if ( reg.IsPartOf( "the Dojo" ) ){ toPlay = OrientalList[Utility.Random(OrientalList.Length)]; }
						else if ( reg.IsPartOf( "the Wizards Guild" ) ){ toPlay = MageList[Utility.Random(MageList.Length)]; }
						else if ( reg.IsPartOf( "the Lyceum" ) ){ toPlay = MageList[Utility.Random(MageList.Length)]; }
						else if ( reg.IsPartOf( "Xardok's Castle" ) ){ toPlay = InnList[Utility.Random(InnList.Length)]; }
						else if ( reg.IsPartOf( "the Camping Tent" ) ){ toPlay = InnList[Utility.Random(InnList.Length)]; }
						else if ( reg.IsPartOf( "the Ship's Lower Deck" ) ){ toPlay = InnList[Utility.Random(InnList.Length)]; }
						else if ( reg.IsPartOf( "the Hall of Legends" ) ){ toPlay = LandMusic[Utility.Random(LandMusic.Length)]; }
						else if ( reg.IsPartOf( "the Chamber of Tyball" ) ){ toPlay = MageList[Utility.Random(MageList.Length)]; }
						else if ( reg.IsPartOf( "the Tower of Stoneguard" ) ){ toPlay = CaveMusic[Utility.Random(CaveMusic.Length)]; }
						else if ( reg.IsPartOf( "the Ethereal Void" ) ){ toPlay = LandMusic[Utility.Random(LandMusic.Length)]; }
						else if ( reg.IsPartOf( "the Tower of Mondain" ) ){ toPlay = CaveMusic[Utility.Random(CaveMusic.Length)]; }
						else if ( reg.IsPartOf( "the Crypt of Morphius" ) ){ toPlay = CaveMusic[Utility.Random(CaveMusic.Length)]; }
						else if ( reg.IsPartOf( "the Castle of Shadowguard" ) ){ toPlay = CaveMusic[Utility.Random(CaveMusic.Length)]; }
						else if ( reg.IsPartOf( "the Guardian's Chamber" ) ){ toPlay = NecromancerMusic[Utility.Random(NecromancerMusic.Length)]; }
						else if ( reg.IsPartOf( "the Tomb of Lethe" ) ){ toPlay = CaveMusic[Utility.Random(CaveMusic.Length)]; }
						else if ( reg.IsPartOf( "Seggallions's Cave" ) ){ toPlay = LandMusic[Utility.Random(LandMusic.Length)]; }
						else if ( reg.IsPartOf( "Garamon's Castle" ) ){ toPlay = MageList[Utility.Random(MageList.Length)]; }
						else if ( reg.IsPartOf( "the Port" ) ){ toPlay = PirateMusic[Utility.Random(PirateMusic.Length)]; }
					}
					else if ( reg is SafeRegion )
					{
						toPlay = InnList[Utility.Random(InnList.Length)]; switchSongs = true;
						if ( reg.Name == "the Kraken Eye Cave" || reg.Name == "the Anchored Cave" || reg.Name == "the Ravendark Tomb" || reg.Name == "the Furnace Eye Cave" )
						{
							toPlay = CaveMusic[Utility.Random(CaveMusic.Length)]; 
						}
						if ( Server.Misc.Worlds.IsSeaTown( from.Location, from.Map ) ){ toPlay = PirateMusic[Utility.Random(PirateMusic.Length)]; }
					}
					else if ( reg is ProtectedRegion )
					{
						toPlay = InnList[Utility.Random(InnList.Length)]; switchSongs = true;
					}
					else if ( Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) ) ) 
					{
						toPlay = LandMusic[Utility.Random(LandMusic.Length)]; switchSongs = true;
					}
					else if ( reg is PirateRegion ) 
					{
						toPlay = PirateMusic[Utility.Random(PirateMusic.Length)]; switchSongs = true;
					}
					else if ( reg is OutDoorBadRegion || reg is DeadRegion )
					{
						toPlay = DangerMusic[Utility.Random(DangerMusic.Length)]; switchSongs = true;
						if ( tunes == "Forest" )
							toPlay = LandMusic[Utility.Random(LandMusic.Length)];
					}
					else if ( reg is NecromancerRegion || reg.IsPartOf( "the Dark Caves" ) )
					{
						toPlay = NecromancerMusic[Utility.Random(NecromancerMusic.Length)]; switchSongs = true;
					}
					else if ( reg is MoonCore || reg is CaveRegion || reg is WantedRegion || reg is SavageRegion )
					{
						toPlay = CaveMusic[Utility.Random(CaveMusic.Length)]; switchSongs = true;
					}
					else if ( reg is VillageRegion || reg is DawnRegion || reg is BardTownRegion )
					{
						toPlay = VillageMusic[Utility.Random(VillageMusic.Length)]; switchSongs = true;
					}
					else if ( reg is GargoyleRegion || reg is UmbraRegion )
					{
						toPlay = DungeonMusic[Utility.Random(DungeonMusic.Length)]; switchSongs = true;
					}
					else if ( reg is DungeonRegion || reg is BardDungeonRegion )
					{
						toPlay = DungeonMusic[Utility.Random(DungeonMusic.Length)]; switchSongs = true;
						if ( tunes == "Forest" )
							toPlay = LandMusic[Utility.Random(LandMusic.Length)];
					}
					else if ( reg is DungeonHomeRegion )
					{
						toPlay = HouseMusic[Utility.Random(HouseMusic.Length)]; switchSongs = true;
					}
					else if ( reg is StartRegion )
					{
						if ( reg.Name == "the Forest" )
							toPlay = MusicName.City; 
						else if ( reg.Name == "the Swamp" )
							toPlay = LandMusic[Utility.Random(LandMusic.Length)];
						else if ( reg.Name == "the Tomb" )
							toPlay = DungeonMusic[Utility.Random(DungeonMusic.Length)];
						else if ( reg.Name == "the Sea" )
							toPlay = LandMusic[Utility.Random(LandMusic.Length)];
						else if ( reg.Name == "the Pits" )
							toPlay = DungeonMusic[Utility.Random(DungeonMusic.Length)];
						else if ( reg.Name == "the Woods" )
							toPlay = LandMusic[Utility.Random(LandMusic.Length)];
						else if ( reg.Name == "the Cave" )
							toPlay = CaveMusic[Utility.Random(CaveMusic.Length)];
						else if ( reg.Name == "the Desert" )
							toPlay = LandMusic[Utility.Random(LandMusic.Length)];
						else if ( reg.Name == "the Tundra" )
							toPlay = LandMusic[Utility.Random(LandMusic.Length)];
						else if ( reg.Name == "the Mountains" )
							toPlay = LandMusic[Utility.Random(LandMusic.Length)];

						switchSongs = true;
					}
				}
				if ( switchSongs ){ from.Send(PlayMusic.GetInstance(toPlay)); }
			}
        }

        public static MusicName[] OrientalList = new MusicName[]
        {
            MusicName.Dojo
        };

        public static MusicName[] InnList = new MusicName[]
        {
            MusicName.Tavern,
            MusicName.Bar,
            MusicName.Alehouse,
            MusicName.Inn,
			MusicName.Pub,
			MusicName.DarkGuild,
			MusicName.SkaraBrae,
			MusicName.Guild
        };

        public static MusicName[] MageList = new MusicName[]
        {
            MusicName.Elidor,
            MusicName.WizardDen,
            MusicName.Mines,
            MusicName.DarkGuild,
			MusicName.Fawn,
			MusicName.SkaraBrae,
			MusicName.Guild
        };

        public static MusicName[] VillageMusic = new MusicName[]
        {
			MusicName.SkaraBrae,
			MusicName.TimeAwaits,
			MusicName.Fawn,
			MusicName.Britain,
			MusicName.Montor,
			MusicName.Yew,
			MusicName.Grey,
			MusicName.Moon,
			MusicName.Docks,
			MusicName.DevilGuard,
			MusicName.DeathGulch,
			MusicName.Luna,
			MusicName.Elidor,
			MusicName.BucsDen,
			MusicName.Montor,
			MusicName.Sailing,
			MusicName.Renika,
			MusicName.Guild,
            MusicName.City
        };

        public static MusicName[] CaveMusic = new MusicName[]
        {
            MusicName.Doom,
            MusicName.Mines,
            MusicName.Cave,
            MusicName.Grotto,
            MusicName.Scouting,
            MusicName.Wrong,
			MusicName.Deceit,
			MusicName.Odyssey
        };

        public static MusicName[] PirateMusic = new MusicName[]
        {
            MusicName.Pirates,
            MusicName.DeathGulch,
            MusicName.Docks,
            MusicName.DarkGuild,
            MusicName.Despise,
            MusicName.City,
            MusicName.Sailing,
            MusicName.BucsDen,
            MusicName.DevilGuard,
            MusicName.Elidor,
            MusicName.Pub
        };

        public static MusicName[] DangerMusic = new MusicName[]
        {
            MusicName.Exodus,
            MusicName.Clues,
            MusicName.DardinsPit,
            MusicName.Doom,
            MusicName.Hythloth,
            MusicName.WizardDen
        };

        public static MusicName[] DungeonMusic = new MusicName[]
        {
            MusicName.Exodus,
            MusicName.Clues,
            MusicName.DardinsPit,
            MusicName.Doom,
            MusicName.Hythloth,
            MusicName.PerinianDepths,
			MusicName.Shame,
			MusicName.Wrong,
			MusicName.Covetous,
			MusicName.Deceit,
			MusicName.Despise,
			MusicName.Destard,
			MusicName.FiresHell,
			MusicName.MinesMorinia,
			MusicName.TimeAwaits,
			MusicName.TimeLord,
			MusicName.Catacombs
        };

        public static MusicName[] LandMusic = new MusicName[]
        {
            MusicName.Traveling,
            MusicName.Explore,
            MusicName.Adventure,
            MusicName.Searching,
            MusicName.Scouting,
            MusicName.Wrong,
            MusicName.Hunting,
            MusicName.Seeking,
            MusicName.Despise,
            MusicName.Wandering,
            MusicName.Odyssey,
            MusicName.Expedition,
            MusicName.Roaming,
            MusicName.Quest
        };

        public static MusicName[] UnderworldMusic = new MusicName[]
        {
            MusicName.Mines,
            MusicName.Docks,
            MusicName.Hythloth,
            MusicName.Searching,
            MusicName.Wrong,
            MusicName.Despise,
            MusicName.FiresHell,
            MusicName.Catacombs
        };

        public static MusicName[] NecromancerMusic = new MusicName[]
        {
            MusicName.Exodus,
            MusicName.Clues,
            MusicName.DardinsPit,
            MusicName.Doom,
            MusicName.Hythloth,
            MusicName.PerinianDepths,
			MusicName.Shame,
			MusicName.Wrong,
			MusicName.Covetous,
			MusicName.Deceit,
			MusicName.Despise,
			MusicName.Destard,
			MusicName.FiresHell,
			MusicName.MinesMorinia,
			MusicName.TimeAwaits
        };

        public static MusicName[] HouseMusic = new MusicName[]
        {
            MusicName.Tavern,
            MusicName.Bar,
            MusicName.Alehouse,
            MusicName.Inn,
			MusicName.Pub,
			MusicName.DarkGuild,
			MusicName.SkaraBrae,
			MusicName.Guild
        };

        public static MusicName[] SythMusic = new MusicName[]
        {
            MusicName.SythCrypt,
			MusicName.SythTomb
        };

        public static MusicName[] JediMusic = new MusicName[]
        {
            MusicName.JediCrypt,
			MusicName.JediTomb
        };

        public static MusicName[] SpaceMusic = new MusicName[]
        {
            MusicName.Station1,
            MusicName.Station2,
            MusicName.Station3,
            MusicName.Station4,
			MusicName.Station5
        };
    }
}