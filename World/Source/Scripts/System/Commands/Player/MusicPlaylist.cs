using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Misc;
using Server.Commands;

namespace Server.Gumps 
{
    public class MusicPlaylist : Gump
    {
		public int m_Origin;

		public static void Initialize()
		{
            CommandSystem.Register( "music", AccessLevel.Player, new CommandEventHandler( MyStats_OnCommand ) );
		}
		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "music" )]
		[Description( "Opens the music playlist and player." )]
		public static void MyStats_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( MusicPlaylist ) );
			from.SendGump( new MusicPlaylist( from, 0 ) );
        }

        public MusicPlaylist ( Mobile from, int origin ) : base ( 50, 50 )
        {
			m_Origin = origin;
            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			string color = "#90add7";
			int display = 70;
			int line = 0;

			AddPage(0);

			AddImage(0, 0, 9581, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddHtml( 15, 15, 200, 20, @"<BODY><BASEFONT Color=" + color + ">MUSIC PLAYLIST</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(737, 11, 4017, 4017, 0, GumpButtonType.Reply, 0);

			MusicPlaylistFunctions.InitializePlaylist( from );
			string MyServerSettings = ((PlayerMobile)from).Preferences.MusicPlaylist;

			int btn = button( 59, MyServerSettings );

			AddButton(422, 15, btn, btn, 59, GumpButtonType.Reply, 0);
			AddHtml( 462, 15, 106, 20, @"<BODY><BASEFONT Color=" + color + ">Use Playlist</BASEFONT></BODY>", (bool)false, (bool)false);

			while ( display > 0 )
			{
				display--;
				line++;

				GetLine( line, MyServerSettings );
			}
		}
    
		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( info.ButtonID > 0 && info.ButtonID < 100 ){ MusicPlaylistFunctions.UpdatePlaylist( from, info.ButtonID ); }
			else if ( info.ButtonID > 100 ){ Server.Misc.MusicPlaylistFunctions.PlayMusicFile( from, (info.ButtonID-100) ); }

			from.SendSound( 0x4A ); 

			if ( info.ButtonID < 1 && m_Origin > 0 ){ from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) ); }
			else if ( info.ButtonID > 0 ){ from.SendGump( new MusicPlaylist( from, m_Origin ) ); }
		}

		public int button( int line, string MyServerSettings )
		{
			string[] eachSong = MyServerSettings.Split('#');
			int nLine = 1;
			int button = 3609;
			foreach (string eachSongs in eachSong)
			{
				if ( line == nLine ){ if ( eachSongs == "1" ){ button = 4018; } }
				nLine++;
			}
			return button;
		}

		public void GetLine( int val, string MyServerSettings )
		{
			string color = "#90add7";
			string txt = "";
			int btn = 3609;
			int num = 0;

			if ( val == 1 ){ num = 1; txt = "Britain*"; }
			else if ( val == 2 ){ num = 2; txt = "Buccaneer's Den"; }
			else if ( val == 3 ){ num = 3; txt = "Castle British*"; }
			else if ( val == 4 ){ num = 4; txt = "Castle of Knowledge*"; }
			else if ( val == 5 ){ num = 5; txt = "Death Gulch"; }
			else if ( val == 6 ){ num = 6; txt = "Devil Guard"; }
			else if ( val == 7 ){ num = 7; txt = "Elidor"; }
			else if ( val == 8 ){ num = 8; txt = "Fawn"; }
			else if ( val == 9 ){ num = 9; txt = "Grey"; }
			else if ( val == 10 ){ num = 10; txt = "Luna"; }
			else if ( val == 11 ){ num = 11; txt = "Montor"; }
			else if ( val == 12 ){ num = 12; txt = "Moon"; }
			else if ( val == 13 ){ num = 13; txt = "Renika"; }
			else if ( val == 14 ){ num = 14; txt = "Skara Brae"; }
			else if ( val == 15 ){ num = 17; txt = "Yew"; }
			else if ( val == 16 ){ num = 30; txt = "Catacombs"; }
			else if ( val == 17 ){ num = 31; txt = "Clues"; }
			else if ( val == 18 ){ num = 32; txt = "Covetous"; }
			else if ( val == 19 ){ num = 33; txt = "Dardin's Pit"; }
			else if ( val == 20 ){ num = 34; txt = "Deceit"; }
			else if ( val == 21 ){ num = 35; txt = "Despise"; }
			else if ( val == 22 ){ num = 36; txt = "Destard"; }
			else if ( val == 23 ){ num = 37; txt = "Doom"; }
			else if ( val == 24 ){ num = 38; txt = "Exodus"; }
			else if ( val == 25 ){ num = 39; txt = "Fires of Hell"; }
			else if ( val == 26 ){ num = 40; txt = "Hythloth"; }
			else if ( val == 27 ){ num = 41; txt = "Mines of Morinia"; }
			else if ( val == 28 ){ num = 42; txt = "Perinian Depths"; }
			else if ( val == 29 ){ num = 43; txt = "Shame"; }
			else if ( val == 30 ){ num = 44; txt = "Time Awaits"; }
			else if ( val == 31 ){ num = 45; txt = "Wrong"; }
			else if ( val == 32 ){ num = 18; txt = "Adventure"; }
			else if ( val == 33 ){ num = 19; txt = "Expedition"; }
			else if ( val == 34 ){ num = 20; txt = "Explore"; }
			else if ( val == 35 ){ num = 21; txt = "Hunting"; }
			else if ( val == 36 ){ num = 22; txt = "Odyssey"; }
			else if ( val == 37 ){ num = 23; txt = "Quest"; }
			else if ( val == 38 ){ num = 24; txt = "Roaming"; }
			else if ( val == 39 ){ num = 25; txt = "Scouting"; }
			else if ( val == 40 ){ num = 26; txt = "Searching"; }
			else if ( val == 41 ){ num = 27; txt = "Seeking"; }
			else if ( val == 42 ){ num = 28; txt = "Traveling"; }
			else if ( val == 43 ){ num = 29; txt = "Wandering"; }
			else if ( val == 44 ){ num = 52; txt = "Alehouse"; }
			else if ( val == 45 ){ num = 53; txt = "Bar"; }
			else if ( val == 46 ){ num = 49; txt = "Cave"; }
			else if ( val == 47 ){ num = 46; txt = "Docks"; }
			else if ( val == 48 ){ num = 61; txt = "Dojo"; }
			else if ( val == 49 ){ num = 50; txt = "Grotto"; }
			else if ( val == 50 ){ num = 54; txt = "Guild"; }
			else if ( val == 51 ){ num = 60; txt = "Gypsy"; }
			else if ( val == 52 ){ num = 55; txt = "Inn"; }
			else if ( val == 53 ){ num = 56; txt = "Lodge"; }
			else if ( val == 54 ){ num = 51; txt = "Mines"; }
			else if ( val == 55 ){ num = 47; txt = "Pirates"; }
			else if ( val == 56 ){ num = 57; txt = "Pub"; }
			else if ( val == 57 ){ num = 48; txt = "Sailing"; }
			else if ( val == 58 ){ num = 58; txt = "Tavern"; }
			else if ( val == 59 ){ num = 15; txt = "Time Lord"; }
			else if ( val == 60 ){ num = 16; txt = "Wizard Den"; }

			if ( txt != "" )
			{
				int x; int y;

				if ( val < 21 ){ x = 77; y = 20 + (val*28); }
				else if ( val < 41 ){ x = 331; y = 20 + ((val-20)*28); }
				else { x = 584; y = 20 + ((val-40)*28); }

				btn = button( num, MyServerSettings );

				AddButton(x-61, y, btn, btn, num, GumpButtonType.Reply, 0);
				AddButton(x-24, y+4, 2117, 2117, num+100, GumpButtonType.Reply, 0);
				AddHtml( x, y, 166, 20, @"<BODY><BASEFONT Color=" + color + ">" + txt + "</BASEFONT></BODY>", (bool)false, (bool)false);
			}
		}
    }
}

namespace Server.Misc
{
    class MusicPlaylistFunctions
    {
		public static void UpdatePlaylist( Mobile m, int nChange )
		{
			m.SendSound( 0x4A ); 

			MusicPlaylistFunctions.InitializePlaylist( m );

			string PlaylistSetting = ((PlayerMobile)m).Preferences.MusicPlaylist;

			string[] eachSetting = PlaylistSetting.Split('#');
			int nLine = 1;
			string newSettings = "";

			foreach (string eachSettings in eachSetting)
			{
				if ( nLine == nChange )
				{
					string sChange = "0";
					if ( eachSettings == "0" ){ sChange = "1"; }
					newSettings = newSettings + sChange + "#";
				}
				else if ( nLine > 61 )
				{
				}
				else
				{
					newSettings = newSettings + eachSettings + "#";
				}
				nLine++;
			}

			((PlayerMobile)m).Preferences.MusicPlaylist = newSettings; 
		}

		public static void InitializePlaylist( Mobile m )
		{
			if ( ((PlayerMobile)m).Preferences.MusicPlaylist == "" || ((PlayerMobile)m).Preferences.MusicPlaylist == null )
				((PlayerMobile)m).Preferences.MusicPlaylist = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";
		}

		public static void PickRandomSong( Mobile m )
		{
			MusicPlaylistFunctions.InitializePlaylist( m );

			string PlaylistSetting = ((PlayerMobile)m).Preferences.MusicPlaylist;

			string[] eachSetting = PlaylistSetting.Split('#');
			int c = 0;
			int x = 1;

			ArrayList songs = new ArrayList();
			foreach (string eachSettings in eachSetting)
			{
				if ( eachSettings == "1" && x < 62 ){ songs.Add( x ); c++; } x++;
			}

			int o = Utility.RandomMinMax( 0, c );

			for ( int i = 0; i < songs.Count; ++i )
			{
				int tune = Convert.ToInt32(songs[ i ]);

				if ( i == o )
				{
					Server.Misc.MusicPlaylistFunctions.PlayMusicFile( m, tune );
				}
			}
		}

		public static void PlayMusicFile( Mobile from, int song )
		{
			MusicName toPlay = MusicName.Adventure;

			switch ( song )
			{
				case 1: { toPlay = MusicName.Britain; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 2: { toPlay = MusicName.BucsDen; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 3: { toPlay = MusicName.CastleBritain; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 4: { toPlay = MusicName.CastleKnowledge; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 5: { toPlay = MusicName.DeathGulch; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 6: { toPlay = MusicName.DevilGuard; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 7: { toPlay = MusicName.Elidor; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 8: { toPlay = MusicName.Fawn; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 9: { toPlay = MusicName.Grey; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 10: { toPlay = MusicName.Luna; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 11: { toPlay = MusicName.Montor; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 12: { toPlay = MusicName.Moon; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 13: { toPlay = MusicName.Renika; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 14: { toPlay = MusicName.SkaraBrae; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 15: { toPlay = MusicName.TimeLord; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 16: { toPlay = MusicName.WizardDen; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 17: { toPlay = MusicName.Yew; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 18: { toPlay = MusicName.Adventure; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 19: { toPlay = MusicName.Expedition; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 20: { toPlay = MusicName.Explore; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 21: { toPlay = MusicName.Hunting; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 22: { toPlay = MusicName.Odyssey; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 23: { toPlay = MusicName.Quest; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 24: { toPlay = MusicName.Roaming; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 25: { toPlay = MusicName.Scouting; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 26: { toPlay = MusicName.Searching; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 27: { toPlay = MusicName.Seeking; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 28: { toPlay = MusicName.Traveling; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 29: { toPlay = MusicName.Wandering; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 30: { toPlay = MusicName.Catacombs; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 31: { toPlay = MusicName.Clues; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 32: { toPlay = MusicName.Covetous; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 33: { toPlay = MusicName.DardinsPit; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 34: { toPlay = MusicName.Deceit; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 35: { toPlay = MusicName.Despise; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 36: { toPlay = MusicName.Destard; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 37: { toPlay = MusicName.Doom; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 38: { toPlay = MusicName.Exodus; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 39: { toPlay = MusicName.FiresHell; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 40: { toPlay = MusicName.Hythloth; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 41: { toPlay = MusicName.MinesMorinia; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 42: { toPlay = MusicName.PerinianDepths; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 43: { toPlay = MusicName.Shame; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 44: { toPlay = MusicName.TimeAwaits; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 45: { toPlay = MusicName.Wrong; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 46: { toPlay = MusicName.Docks; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 47: { toPlay = MusicName.Pirates; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 48: { toPlay = MusicName.Sailing; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 49: { toPlay = MusicName.Cave; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 50: { toPlay = MusicName.Grotto; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 51: { toPlay = MusicName.Mines; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 52: { toPlay = MusicName.Alehouse; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 53: { toPlay = MusicName.Bar; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 54: { toPlay = MusicName.Guild; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 55: { toPlay = MusicName.Inn; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 56: { toPlay = MusicName.DarkGuild; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 57: { toPlay = MusicName.Pub; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 58: { toPlay = MusicName.Tavern; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 60: { toPlay = MusicName.City; from.Send(PlayMusic.GetInstance(toPlay)); break; }
				case 61: { toPlay = MusicName.Dojo; from.Send(PlayMusic.GetInstance(toPlay)); break; }
			}
		}

		public static int GetPlaylistSetting( Mobile m, int nSetting )
		{
			var player = m as PlayerMobile;
			if (player == null) return 0;

			string sSetting = "0";

			MusicPlaylistFunctions.InitializePlaylist( m );

			string PlaylistSetting = player.Preferences.MusicPlaylist;

			string[] eachSetting = PlaylistSetting.Split('#');
			int nLine = 1;

			foreach (string eachSettings in eachSetting)
			{
				if ( nLine == nSetting ){ sSetting = eachSettings; }
				nLine++;
			}

			int nValue = Convert.ToInt32(sSetting);

			return nValue;
		}
	}
}