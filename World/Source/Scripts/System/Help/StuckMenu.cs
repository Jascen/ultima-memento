using System;
using Server.Network;
using Server.Gumps;
using Server.Regions;
using Server.Misc;

namespace Server.Menus.Questions
{
	public class StuckMenuEntry
	{
		private TextDefinition m_Name;
		private Point3D[] m_Locations;

		public TextDefinition Name{ get{ return m_Name; } }
		public Point3D[] Locations{ get{ return m_Locations; } }

		public StuckMenuEntry( TextDefinition name, Point3D[] locations )
		{
			m_Name = name;
			m_Locations = locations;
		}
	}

	public class StuckMenu : Gump
	{
		public void AddHtmlText( int x, int y, int width, int height, TextDefinition text, bool back, bool scroll )
		{
			if ( text != null && text.Number > 0 )
				AddHtmlLocalized( x, y, width, height, text.Number, back, scroll );
			else if ( text != null && text.String != null )
				AddHtml( x, y, width, height, text.String, back, scroll );
		}

		private static StuckMenuEntry[] m_Entries = new StuckMenuEntry[] { new StuckMenuEntry( "Sosaria", new Point3D[] { new Point3D( 3213, 3673, 0 ) } ) };
		private static StuckMenuEntry[] m_LunaEntries = new StuckMenuEntry[] { new StuckMenuEntry( "Luna", new Point3D[] { new Point3D( 5884, 2864, 0 ) } ) };
		private static StuckMenuEntry[] m_AmbrosiaEntries = new StuckMenuEntry[] { new StuckMenuEntry( "Ambrosia", new Point3D[] { new Point3D( 3325, 3934, 0 ) } ) };
		private static StuckMenuEntry[] m_UmberEntries = new StuckMenuEntry[] { new StuckMenuEntry( "Umber Veil", new Point3D[] { new Point3D( 2982, 3696, 0 ) } ) };
		private static StuckMenuEntry[] m_SerpentEntries = new StuckMenuEntry[] { new StuckMenuEntry( "Serpent Island", new Point3D[] { new Point3D( 2191, 315, 0 ) } ) };
		private static StuckMenuEntry[] m_LodorEntries = new StuckMenuEntry[] { new StuckMenuEntry( "Lodoria", new Point3D[] { new Point3D( 5727, 3467, 0 ) } ) };
		private static StuckMenuEntry[] m_DreadEntries = new StuckMenuEntry[] { new StuckMenuEntry( "Isles of Dread", new Point3D[] { new Point3D( 237, 324, 3 ) } ) };
		private static StuckMenuEntry[] m_SavageEntries = new StuckMenuEntry[] { new StuckMenuEntry( "Savaged Empire", new Point3D[] { new Point3D( 1110, 2541, -1 ) } ) };
		private static StuckMenuEntry[] m_KuldarEntries = new StuckMenuEntry[] { new StuckMenuEntry( "Bottle World", new Point3D[] { new Point3D( 6714, 705, 0 ) } ) };
		private static StuckMenuEntry[] m_BardEntries = new StuckMenuEntry[] { new StuckMenuEntry( "Skara Brae", new Point3D[] { new Point3D( 7041, 213, 0 ) } ) };
		private static StuckMenuEntry[] m_UnderworldEntries = new StuckMenuEntry[] { new StuckMenuEntry( "Underworld", new Point3D[] { new Point3D( 755, 393, 0 ) } ) };

		private Mobile m_Mobile, m_Sender;
		private bool m_MarkUse;
		private int m_Origin;
		private Map m_Map;

		private Timer m_Timer;

		public StuckMenu( Mobile beholder, Mobile beheld, bool markUse, int origin = 0 ) : base( 50, 50 )
		{
			m_Sender = beholder;
			m_Mobile = beheld;
			m_MarkUse = markUse;
			m_Origin = origin;
			m_Map = Map.Sosaria;
			string color = "#ddbc4b";

			StuckMenuEntry[] entries = m_Entries;

			Land land = m_Mobile.Land;

			if ( land == Land.Luna ) { entries = m_LunaEntries; m_Map = Map.Sosaria; }
			else if ( land == Land.Ambrosia ) { entries = m_AmbrosiaEntries; m_Map = Map.Sosaria; }
			else if ( land == Land.UmberVeil ) { entries = m_UmberEntries; m_Map = Map.Sosaria; }
			else if ( land == Land.Kuldar ) { entries = m_KuldarEntries; m_Map = Map.Sosaria; }
			else if ( land == Land.SkaraBrae ) { entries = m_BardEntries; m_Map = Map.Lodor; }
			else if ( land == Land.Lodoria ) { entries = m_LodorEntries; m_Map = Map.Lodor; }
			else if ( land == Land.Sosaria ) { entries = m_Entries; m_Map = Map.Sosaria; }
			else if ( land == Land.Underworld ) { entries = m_UnderworldEntries; m_Map = Map.Underworld; }
			else if ( land == Land.Serpent ) { entries = m_SerpentEntries; m_Map = Map.SerpentIsland; }
			else if ( land == Land.IslesDread ) { entries = m_DreadEntries; m_Map = Map.IslesDread; }
			else if ( land == Land.Savaged ) { entries = m_SavageEntries; m_Map = Map.SavagedEmpire; }
			else if ( land == Land.Atlantis ) { entries = m_Entries; m_Map = Map.Sosaria; }

            this.Closable=true;
			this.Disposable=false;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			AddImage(0, 0, 9577, Server.Misc.PlayerSettings.GetGumpHue( m_Mobile ));
			AddHtml( 10, 11, 245, 20, @"<BODY><BASEFONT Color=" + color + ">STUCK IN THE WORLD</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(267, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

			for ( int i = 0; i < entries.Length; i++ )
			{
				StuckMenuEntry entry = entries[i];

				AddButton(14, 104, 4023, 4023, i+1, GumpButtonType.Reply, 0);
				AddHtml( 51, 104, 245, 104, @"<BODY><BASEFONT Color=" + color + ">Take me to a safe place in " + Server.Lands.LandName( land ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
			}

			AddButton(14, 232, 4020, 4020, 0, GumpButtonType.Reply, 0);
			AddHtml( 51, 233, 245, 20, @"<BODY><BASEFONT Color=" + color + ">Cancel</BASEFONT></BODY>", (bool)false, (bool)false);
		}

		public void BeginClose()
		{
			StopClose();

			m_Timer = new CloseTimer( m_Mobile );
			m_Timer.Start();

			m_Mobile.Frozen = true;
		}

		public void StopClose()
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			m_Mobile.Frozen = false;
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			StopClose();

			if ( info.ButtonID == 0 )
			{
				if ( m_Mobile == m_Sender )
				{
					m_Mobile.SendSound( 0x4A ); 
					m_Mobile.SendMessage( "You choose to remain where you are." );
					if ( m_Origin > 0 )
					{
						m_Mobile.CloseGump( typeof(Server.Engines.Help.HelpGump) );
						m_Mobile.SendGump( new Server.Engines.Help.HelpGump( m_Mobile, m_Origin ) );
					}
				}
			}
			else
			{
				int index = info.ButtonID - 1;

				StuckMenuEntry[] entries = m_Entries;

				Land land = m_Mobile.Land;

				if ( land == Land.Luna ) { entries = m_LunaEntries; }
				else if ( land == Land.Ambrosia ) { entries = m_AmbrosiaEntries; }
				else if ( land == Land.UmberVeil ) { entries = m_UmberEntries; }
				else if ( land == Land.Kuldar ) { entries = m_KuldarEntries; }
				else if ( land == Land.SkaraBrae ) { entries = m_BardEntries; }
				else if ( land == Land.Lodoria ) { entries = m_LodorEntries; }
				else if ( land == Land.Sosaria ) { entries = m_Entries; }
				else if ( land == Land.Serpent ) { entries = m_SerpentEntries; }
				else if ( land == Land.IslesDread ) { entries = m_DreadEntries; }
				else if ( land == Land.Savaged ) { entries = m_SavageEntries; }
				else if ( land == Land.Underworld ) { entries = m_UnderworldEntries; }

				if ( index >= 0 && index < entries.Length )
					Teleport( entries[index] );
			}
		}

		private void Teleport( StuckMenuEntry entry )
		{
			if ( m_MarkUse ) 
			{
				m_Mobile.SendLocalizedMessage( 1010589 ); // You will be teleported within the next two minutes.

				new TeleportTimer( m_Mobile, entry, TimeSpan.FromSeconds( 10.0 + (Utility.RandomDouble() * 110.0) ), m_Map ).Start();

				m_Mobile.UsedStuckMenu();
			}
			else
			{
				new TeleportTimer( m_Mobile, entry, TimeSpan.Zero, m_Map ).Start();
			}
		}

		private class CloseTimer : Timer
		{
			private Mobile m_Mobile;
			private DateTime m_End;

			public CloseTimer( Mobile m ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Mobile = m;
				m_End = DateTime.Now + TimeSpan.FromMinutes( 3.0 );
			}

			protected override void OnTick()
			{
				if ( m_Mobile.NetState == null || DateTime.Now > m_End )
				{
					m_Mobile.Frozen = false;
					m_Mobile.CloseGump( typeof( StuckMenu ) );

					Stop();
				}
				else
				{
					m_Mobile.Frozen = true;
				}
			} 
		} 

		private class TeleportTimer : Timer
		{
			private Map m_Map;
			private Mobile m_Mobile;
			private StuckMenuEntry m_Destination;
			private DateTime m_End;

			public TeleportTimer( Mobile mobile, StuckMenuEntry destination, TimeSpan delay, Map world ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.0 ) )
			{
				Priority = TimerPriority.TwoFiftyMS;

				m_Mobile = mobile;
				m_Map = world;
				m_Destination = destination;
				m_End = DateTime.Now + delay;
			}

			protected override void OnTick()
			{
				if ( DateTime.Now < m_End )
				{
					m_Mobile.Frozen = true;
				}
				else
				{
					m_Mobile.Frozen = false;
					Stop();

					int idx = Utility.Random( m_Destination.Locations.Length );
					Point3D dest = m_Destination.Locations[idx];

					Map destMap = m_Map;

					Mobiles.BaseCreature.TeleportPets( m_Mobile, dest, destMap );
					m_Mobile.MoveToWorld( dest, destMap );
				}
			}
		}
	}
}