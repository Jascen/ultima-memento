using System;
using Server;
using Server.Mobiles;
using Server.Network;
using System.Collections; 
using Server.Items;
using Server.Misc;
using Server.Gumps;

namespace Server.Items
{
	public class GateMoon : Item
	{
		[Constructable]
		public GateMoon() : base( 0x1B72 )
		{
			Movable = false;
			Visible = false;
			Name = "moongate";
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( m is PlayerMobile )
			{
				if ( m is PlayerMobile && m.Land == Land.Kuldar && !( Server.Misc.PlayerSettings.GetKeys( m, "VordoKey" ) ) )
				{
					m.SendMessage( "This magical gate doesn't seem to do anything." );
				}
				else if ( Worlds.AllowEscape( m, m.Map, m.Location, m.X, m.Y ) == false && m.Land != Land.Kuldar )
				{
					m.SendMessage( "This magical gate doesn't seem to do anything." );
				}
				else if ( Worlds.RegionAllowedRecall( m.Map, m.Location, m.X, m.Y ) == false && m.Land != Land.Ambrosia && m.Land != Land.Kuldar )
				{
					m.SendMessage( "This magical gate doesn't seem to do anything." );
				}
				else 
				{
					Timer.DelayCall( TimeSpan.FromSeconds( 0.5 ),( delegate
					{
						foreach ( Mobile pet in World.Mobiles.Values )
						{
							if ( pet is BaseCreature )
							{
								BaseCreature bc = (BaseCreature)pet;
								if ( bc.Controlled && bc.ControlMaster == m )
									pet.Hidden = true;
							}
						}
						
						if ( m.Alive )
							m.Hidden = true;
					} ) );

					m.PlaySound( 0x20E );
					m.CloseGump( typeof( MoonGateGump ) );
					m.SendGump( new MoonGateGump( m, false ) );
					m.SendMessage( "Choose a destination." );
				}
			}
			return true;
		}

		public class MoonGateGump : Gump
		{
			public MoonGateGump( Mobile from, bool IsBlackrock ): base( 50, 50 )
			{
				string color = "#9abdcf";

				this.Closable=true;
				this.Disposable=false;
				this.Dragable=true;
				this.Resizable=false;

				string mainTitle = "MAGICAL PORTAL DESTINATIONS";
				int img = 9583;
					if ( IsBlackrock ){ img = 9582; mainTitle = "BLACKROCK GATE DESTINATIONS"; } 

				AddPage(0);

				AddImage(0, 0, img, Server.Misc.PlayerSettings.GetGumpHue( from ));
				AddButton(571, 7, 4017, 4017, 0, GumpButtonType.Reply, 0);
				AddHtml( 7, 6, 243, 20, @"<BODY><BASEFONT Color=" + color + ">" + mainTitle + "</BASEFONT></BODY>", (bool)false, (bool)false);

				/////////////////////////////////////////////////////////////////////////////

				int GateAmount = 30; // THE AMOUNT OF MOONGATES IN THE GAME - MAX 30
				int GateNumber = 0;
				string sPlace = "";
				int counter = 0;

				while ( GateAmount > 0 )
				{
					GateAmount--;
					GateNumber++;

					sPlace = GetGateName( GateNumber, from );

					if ( sPlace != "" )
					{
						counter++;

						int x = 373;
						int y = 35 + (23*(counter-16));

						if ( counter < 16 )
						{
							x = 12;
							y = 35 + (23*(counter-1));
						}

						AddHtml( x+50, y, 180, 20, @"<BODY><BASEFONT Color=" + color + ">" + sPlace + "</BASEFONT></BODY>", (bool)false, (bool)false);
						AddButton(x, y, 4005, 4005, GateNumber, GumpButtonType.Reply, 0);
					}
				}
			}

			public bool NearGate( Mobile from )
			{
				foreach ( Item i in from.GetItemsInRange( 10 ) )
				{
					if ( (i is ObsidianGate || i is GateMoon) && i.Map == from.Map )
					{
						return true;
					}
				}
				return false;
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile;

				from.CloseGump( typeof( MoonGateGump ) );

				if ( info.ButtonID > 0 && NearGate( from ) )
				{
					bool gate1 = info.ButtonID == 1;
					bool gate2 = info.ButtonID == 2;
					bool gate3 = info.ButtonID == 3;
					bool gate4 = info.ButtonID == 4;
					bool gate5 = info.ButtonID == 5;
					bool gate6 = info.ButtonID == 6;
					bool gate7 = info.ButtonID == 7;
					bool gate8 = info.ButtonID == 8;
					bool gate9 = info.ButtonID == 9;
					bool gate10 = info.ButtonID == 10;
					bool gate11 = info.ButtonID == 11;
					bool gate12 = info.ButtonID == 12;
					bool gate13 = info.ButtonID == 13;
					bool gate14 = info.ButtonID == 14;
					bool gate15 = info.ButtonID == 15;
					bool gate16 = info.ButtonID == 16;
					bool gate17 = info.ButtonID == 17;
					bool gate18 = info.ButtonID == 18;
					bool gate19 = info.ButtonID == 19;
					bool gate20 = info.ButtonID == 20;
					bool gate21 = info.ButtonID == 21;
					bool gate22 = info.ButtonID == 22;
					bool gate23 = info.ButtonID == 23;
					bool gate24 = info.ButtonID == 24;
					bool gate25 = info.ButtonID == 25;
					bool gate26 = info.ButtonID == 26;
					bool gate27 = info.ButtonID == 27;
					bool gate28 = info.ButtonID == 28;
					bool gate29 = info.ButtonID == 29;
					bool gate30 = info.ButtonID == 30;

					int gX = 0; int gY = 0; int gZ = 0; Map map = Map.Sosaria;

					if (gate1){gX = 2518; gY = 1529; gZ = 3; map = Map.Sosaria;  }
					else if (gate2){gX = 3723; gY = 2155; gZ = 4; map = Map.Sosaria;  }
					else if (gate3){gX = 1779; gY = 1714; gZ = 6; map = Map.Sosaria;  }
					else if (gate4){gX = 3718; gY = 1136; gZ = 0; map = Map.Sosaria;  }
					else if (gate5){gX = 4970; gY = 1297; gZ = 4; map = Map.Sosaria;  }
					else if (gate6){gX = 2548; gY = 2685; gZ = 4; map = Map.Sosaria;  }
					else if (gate7){gX = 963; gY = 514; gZ = 4; map = Map.Sosaria;  }
					else if (gate8){gX = 1052; gY = 1570; gZ = 2; map = Map.Sosaria;  }
					else if (gate9){gX = 1792; gY = 913; gZ = 27; map = Map.Sosaria;  }
					else if (gate10){gX = 968; gY = 2726; gZ = 4; map = Map.Sosaria;  }
					else if (gate11){gX = 4038; gY = 179; gZ = 2; map = Map.Sosaria;  }
					else if (gate12){gX = 6092; gY = 3595; gZ = 4; map = Map.Sosaria;  }
					else if (gate13){gX = 1249; gY = 3815; gZ = 2; map = Map.Sosaria;  }
					else if (gate14){gX = 1017; gY = 546; gZ = 3; map = Map.IslesDread;  }
					else if (gate15){gX = 4199; gY = 2516; gZ = 7; map = Map.Lodor;  }
					else if (gate16){gX = 2497; gY = 1981; gZ = 5; map = Map.Lodor;  }
					else if (gate17){gX = 1045; gY = 2258; gZ = 6; map = Map.Lodor;  }
					else if (gate18){gX = 2350; gY = 3619; gZ = 6; map = Map.Lodor;  }
					else if (gate19){gX = 4276; gY = 1841; gZ = 16; map = Map.Lodor;  }
					else if (gate20){gX = 719; gY = 962; gZ = 6; map = Map.Lodor;  }
					else if (gate21){gX = 2876; gY = 733; gZ = 9; map = Map.Lodor;  }
					else if (gate22){gX = 1163; gY = 411; gZ = 5; map = Map.SerpentIsland;  }
					else if (gate23){gX = 1300; gY = 1372; gZ = 5; map = Map.SerpentIsland;  }
					else if (gate24){gX = 234; gY = 1333; gZ = 3; map = Map.SerpentIsland;  }
					else if (gate25){gX = 656; gY = 240; gZ = 3; map = Map.SavagedEmpire;  }
					else if (gate26){gX = 1112; gY = 1710; gZ = 20; map = Map.SavagedEmpire;  }
					else if (gate27){gX = 303; gY = 1269; gZ = 3; map = Map.SavagedEmpire;  }
					else if (gate28){gX = 6603; gY = 1082; gZ = 2; map = Map.Sosaria;  }
					else if (gate29){gX = 6377; gY = 302; gZ = 15; map = Map.Lodor;  }
					else if (gate30)
					{
						if ( PlayerSettings.GetDiscovered( from, "the Land of Sosaria" ) && from.Karma >= 0 && from.Kills < 1 && !Server.Items.BaseRace.IsEvil( from ) )
						{
							gX = 3907; gY = 3962; gZ = 5; map = Map.Sosaria;
						}
						else if ( Server.Items.BaseRace.IsEvilSeaCreature( from ) && from.RaceHomeLand == 2 )
						{
							gX = 5445; gY = 4052; gZ = 5; map = Map.Lodor;
						}
						else if ( Server.Items.BaseRace.IsEvilSeaCreature( from ) && from.RaceHomeLand == 1 )
						{
							gX = 7061; gY = 340; gZ = 5; map = Map.Sosaria;
						}
						else if ( Server.Items.BaseRace.IsEvilDeadCreature( from ) && from.RaceHomeLand == 2 )
						{
							gX = 5242; gY = 3665; gZ = 5; map = Map.Lodor;
						}
						else if ( Server.Items.BaseRace.IsEvilDeadCreature( from ) && from.RaceHomeLand == 1 )
						{
							gX = 5254; gY = 1063; gZ = 5; map = Map.Sosaria;
						}
						else if ( Server.Items.BaseRace.IsEvilDemonCreature( from ) )
						{
							gX = 2513; gY = 995; gZ = 5; map = Map.SerpentIsland;
						}
					}

					if ( gX > 0 )
					{
						Point3D loc = new Point3D( gX, gY, gZ );
						GateMoonTeleport( from, loc, map );
					}
				}
				else
				{
					from.SendSound( 0x0F7 );
				}
			}
		}

		public static string GetGateName( int gate, Mobile m )
		{
			PlayerMobile pm = (PlayerMobile)m;

			string sGate = "";

			if ( m.Land == Land.Kuldar && !(PlayerSettings.GetDiscovered( m, "the Bottle World of Kuldar" )) ){}
			else if ( gate == 1 && PlayerSettings.GetDiscovered( m, "the Land of Sosaria" ) ){ sGate = "Sosaria - Central"; }
			else if ( gate == 2 && PlayerSettings.GetDiscovered( m, "the Land of Sosaria" )){ sGate = "Sosaria - Clues"; }
			else if ( gate == 3 && PlayerSettings.GetDiscovered( m, "the Land of Sosaria" )){ sGate = "Sosaria - Devil Guard"; }
			else if ( gate == 4 && PlayerSettings.GetDiscovered( m, "the Land of Sosaria" )){ sGate = "Sosaria - East"; }
			else if ( gate == 5 && PlayerSettings.GetDiscovered( m, "the Land of Sosaria" )){ sGate = "Sosaria - Frozen Isles"; }
			else if ( gate == 6 && PlayerSettings.GetDiscovered( m, "the Land of Sosaria" )){ sGate = "Sosaria - Montor"; }
			else if ( gate == 7 && PlayerSettings.GetDiscovered( m, "the Land of Sosaria" )){ sGate = "Sosaria - Moon"; }
			else if ( gate == 8 && PlayerSettings.GetDiscovered( m, "the Land of Sosaria" )){ sGate = "Sosaria - West"; }
			else if ( gate == 9 && PlayerSettings.GetDiscovered( m, "the Land of Sosaria" )){ sGate = "Sosaria - Yew"; }

			else if ( gate == 10 && PlayerSettings.GetDiscovered( m, "the Land of Ambrosia" ) ){ sGate = "Isle of Fire"; }
			else if ( gate == 11 && PlayerSettings.GetDiscovered( m, "the Island of Umber Veil" ) ){ sGate = "Lost Isle"; }

			else if ( gate == 12 && PlayerSettings.GetDiscovered( m, "the Land of Ambrosia" ) ){ sGate = "Land of Ambrosia"; }
			else if ( gate == 13 && PlayerSettings.GetDiscovered( m, "the Island of Umber Veil" ) ){ sGate = "Island of Umber Veil"; }
			else if ( gate == 14 && PlayerSettings.GetDiscovered( m, "the Isles of Dread" ) ){ sGate = "Isles of Dread"; }

			else if ( gate == 15 && PlayerSettings.GetDiscovered( m, "the Land of Lodoria" ) ){ sGate = "Lodoria - Greensky"; }
			else if ( gate == 16 && PlayerSettings.GetDiscovered( m, "the Land of Lodoria" ) ){ sGate = "Lodoria - Islegem"; }

			else if ( gate == 17 && PlayerSettings.GetDiscovered( m, "the Land of Lodoria" ) ){ sGate = "Lodoria - Portshine"; }
			else if ( gate == 18 && PlayerSettings.GetDiscovered( m, "the Land of Lodoria" ) ){ sGate = "Lodoria - South"; }
			else if ( gate == 19 && PlayerSettings.GetDiscovered( m, "the Land of Lodoria" ) ){ sGate = "Lodoria - Springvale"; }
			else if ( gate == 20 && PlayerSettings.GetDiscovered( m, "the Land of Lodoria" ) ){ sGate = "Lodoria - Whisper"; }
			else if ( gate == 21 && PlayerSettings.GetDiscovered( m, "the Land of Lodoria" ) ){ sGate = "Lodoria - Winterlands"; }

			else if ( gate == 22 && PlayerSettings.GetDiscovered( m, "the Serpent Island" ) ){ sGate = "Serpent Island - North"; }
			else if ( gate == 23 && PlayerSettings.GetDiscovered( m, "the Serpent Island" ) ){ sGate = "Serpent Island - South"; }
			else if ( gate == 24 && PlayerSettings.GetDiscovered( m, "the Serpent Island" ) ){ sGate = "Serpent Island - West"; }

			else if ( gate == 25 && PlayerSettings.GetDiscovered( m, "the Savaged Empire" ) ){ sGate = "Savaged Empire - North"; }
			else if ( gate == 26 && PlayerSettings.GetDiscovered( m, "the Savaged Empire" ) ){ sGate = "Savaged Empire - South"; }
			else if ( gate == 27 && PlayerSettings.GetDiscovered( m, "the Savaged Empire" ) ){ sGate = "Savaged Empire - West"; }

			else if ( gate == 28 && PlayerSettings.GetDiscovered( m, "the Bottle World of Kuldar" ) ){ sGate = "Bottle World of Kuldar"; }
			else if ( gate == 29 && PlayerSettings.GetDiscovered( m, "the Bottle World of Kuldar" ) ){ sGate = "Black Knight's Vault"; }

			else if ( gate == 30 )
			{
				if ( PlayerSettings.GetDiscovered( m, "the Land of Sosaria" ) && m.Karma >= 0 && m.Kills < 1 && !Server.Items.BaseRace.IsEvil( m ) )
				{
					sGate = "Woodlands - Druid's Glade";
				}
				else if ( Server.Items.BaseRace.IsEvilSeaCreature( m ) && m.RaceHomeLand == 2 )
				{
					sGate = "Vortex - Kraken Eye Cave";
				}
				else if ( Server.Items.BaseRace.IsEvilSeaCreature( m ) && m.RaceHomeLand == 1 )
				{
					sGate = "Vortex - Anchored Cave";
				}
				else if ( Server.Items.BaseRace.IsEvilDeadCreature( m ) && m.RaceHomeLand == 2 )
				{
					sGate = "Vortex - Ravendark Tomb";
				}
				else if ( Server.Items.BaseRace.IsEvilDeadCreature( m ) && m.RaceHomeLand == 1 )
				{
					sGate = "Vortex - Umbra Cave";
				}
				else if ( Server.Items.BaseRace.IsEvilDemonCreature( m ) )
				{
					sGate = "Vortex - Furnace Eye Cave";
				}
			}

			return sGate;
		}

		public static void GateMoonTeleport( Mobile m, Point3D loc, Map map )
		{
			BaseCreature.TeleportPets( m, loc, map, false );
			m.MoveToWorld ( loc, map );
			m.PlaySound( 0x1FE );
			foreach ( Mobile pet in World.Mobiles.Values )
			{
				if ( pet is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)pet;
					if ( bc.Controlled && bc.ControlMaster == m )
						pet.Hidden = true;
				}
			}
						
			if ( m.Alive )
				m.Hidden = true;
		}

		public GateMoon(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}