using System;
using Server.Network;
using Server.Accounting;
using Server.Mobiles;
using Server.Gumps;
using System.Linq;
using System.Collections.Generic;

namespace Server.Items
{
	[Flipable(0x577C, 0x577B)]
	public class AdvertiserVendor : Item
	{
		[Constructable]
		public AdvertiserVendor( ) : base( 0x577C )
		{
			Weight = 1.0;
			Name = "The Merchant Advertiser";
			Hue = 0xABE;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( "A Listing Of Player Vendors" );
		}

		public override void OnDoubleClick( Mobile e )
		{
			var player = e as PlayerMobile;
			if (player == null) return;

			var list = World.Mobiles.Values
				.Where(x => x is PlayerVendor)
				.Cast<PlayerVendor>()
				.Where(x => {
					var owner = x.Owner as PlayerMobile;
					if (owner == null) return false;

					// If the player is a special game mode, only show those vendors
					if (player.Avatar.Active) return owner.Avatar.Active;
					if (player.Temptations.HasPermanentDeath) return owner.Temptations.HasPermanentDeath;

					return true;
				})
				.ToList();

			e.SendGump( new FindPlayerVendorsGump( e, list, 1 ) );
		}

		public class FindPlayerVendorsGump : Gump
		{
			private const int GreenHue = 0x40;
			private const int RedHue = 0x20;
			private List<PlayerVendor> m_List;
			private int m_Page;
			private Mobile m_From;

			public void AddBlackAlpha( int x, int y, int width, int height )
			{
				AddImageTiled( x, y, width, height, 2624 );
				AddAlphaRegion( x, y, width, height );
			}

			public FindPlayerVendorsGump( Mobile from, List<PlayerVendor> list, int page ) : base( 50, 40 )
			{
				from.CloseGump( typeof( FindPlayerVendorsGump ) );
				int pvs = 0;
				m_Page = page;
				m_From = from;
				int pageCount = 0;
				m_List = list;

				AddPage( 0 );
				AddBackground( 0, 0, 645, 325, 3500 );
				AddBlackAlpha( 20, 20, 604, 277 );
				pvs = list.Count;
				if ( list.Count % 12 == 0 )
				{
					pageCount = (list.Count / 12);
				}
				else
				{
					pageCount = (list.Count / 12) + 1;
				}

				AddLabelCropped( 32, 20, 100, 20, 1152, "Shop Name" );
				AddLabelCropped( 250, 20, 120, 20, 1152, "Owner" );
				AddLabelCropped( 415, 20, 120, 20, 1152, "Location" );
				AddLabel( 27, 298, 32, String.Format( "" + MySettings.S_ServerName + "  -  Home Vendors                 There are {0} vendors in the world.", pvs ));

				if ( page > 1 )
					AddButton( 573, 22, 0x15E3, 0x15E7, 1, GumpButtonType.Reply, 0 );
				else
					AddImage( 573, 22, 0x25EA );

				if ( pageCount > page )
					AddButton( 590, 22, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0 );
				else
					AddImage( 590, 22, 0x25E6 );

				if ( m_List.Count == 0 )
					AddLabel( 180, 115, 1152, ".....::: There are no Vendors in world :::....." );

				if ( page == pageCount )
				{
					for ( int i = (page * 12) -12; i < pvs; ++i )
						AddDetails( i );
				}
				else
				{
					for ( int i = (page * 12) -12; i < page * 12; ++ i )
						AddDetails( i );
				}
			}

			private void AddDetails( int index )
			{
				try{
					if ( index < m_List.Count )
					{
						int xSet = 1;
						int ySet = 1;
						Map mSet = Map.Sosaria;

						int btn;
						int row;
						btn = (index) + 101;
						row = index % 12;
						PlayerVendor pv = m_List[index] as PlayerVendor;
						Account a = pv.Owner.Account as Account;

						string vMap = "Sosaria";

						if ( ( pv.Map == Map.Lodor ) && ( pv.X > 5157 ) && ( pv.Y > 1095 ) && ( pv.X < 5296 ) && ( pv.Y < 1401 ) ) { vMap = "Ranger Outpost"; xSet = 1241; ySet = 1888; mSet = Map.Lodor; }
						else if ( ( pv.Map == Map.Lodor ) && ( pv.X > 6445 ) && ( pv.Y > 3054 ) && ( pv.X < 7007 ) && ( pv.Y < 3478 ) ) { vMap = "Ravendark Woods"; xSet = 466; ySet = 3801; mSet = Map.Lodor; }
						else if ( pv.Map == Map.Lodor ) { vMap = "Lodoria"; xSet = pv.X; ySet = pv.Y; mSet = Map.Lodor; }
						else if ( ( pv.Map == Map.Sosaria ) && ( pv.X > 5218 ) && ( pv.Y > 1036 ) && ( pv.X < 5414 ) && ( pv.Y < 1304 ) ) { vMap = "Umbra Cave"; xSet = 3370; ySet = 1553; mSet = Map.Sosaria; }
						else if ( ( pv.Map == Map.Sosaria ) && ( pv.X > 6548 ) && ( pv.Y > 3812 ) && ( pv.X < 6741 ) && ( pv.Y < 4071 ) ) { vMap = "Shipwreck Grotto"; xSet = 318; ySet = 1397; mSet = Map.Sosaria; }
						else if ( ( pv.Map == Map.Sosaria ) && ( pv.X > 860 ) && ( pv.Y > 3184 ) && ( pv.X < 2136 ) && ( pv.Y < 4090 ) ) { vMap = "Umber Veil"; xSet = pv.X; ySet = pv.Y; mSet = Map.Sosaria; }
						else if ( ( pv.Map == Map.Sosaria ) && ( pv.X > 5129 ) && ( pv.Y > 3062 ) ) { vMap = "Ambrosia"; xSet = pv.X; ySet = pv.Y; mSet = Map.Sosaria; }
						else if ( ( pv.Map == Map.Sosaria ) && ( pv.X > 5793 ) && ( pv.Y > 2738 ) && ( pv.X < 6095 ) && ( pv.Y < 3011 ) ) { vMap = "Moon of Luna"; xSet = 3696; ySet = 519; mSet = Map.Sosaria; }
						else if ( ( pv.Map == Map.SerpentIsland ) && ( pv.X > 1875 ) )
						{
							if ( ( pv.X > 1949 ) && ( pv.Y > 1393 ) && ( pv.X < 2061 ) && ( pv.Y < 1486 ) ){ xSet = 1863; ySet = 1129; vMap = "Sosaria"; mSet = Map.Sosaria; }
							else if ( ( pv.X > 2150 ) && ( pv.Y > 1401 ) && ( pv.X < 2270 ) && ( pv.Y < 1513 ) ){ xSet = 1861; ySet = 2747; vMap = "Lodoria"; mSet = Map.Lodor; }
							else if ( ( pv.X > 2375 ) && ( pv.Y > 1398 ) && ( pv.X < 2442 ) && ( pv.Y < 1467 ) ){ xSet = 466; ySet = 3801; vMap = "Lodoria"; mSet = Map.Lodor; }
							else if ( ( pv.X > 2401 ) && ( pv.Y > 1635 ) && ( pv.X < 2468 ) && ( pv.Y < 1703 ) ){ xSet = 254; ySet = 670; vMap = "Serpent Island"; mSet = Map.SerpentIsland; }
							else if ( ( pv.X > 2408 ) && ( pv.Y > 1896 ) && ( pv.X < 2517 ) && ( pv.Y < 2005 ) ){ xSet = 422; ySet = 398; vMap = "Savaged Empire"; mSet = Map.SavagedEmpire; }
							else if ( ( pv.X > 2181 ) && ( pv.Y > 1889 ) && ( pv.X < 2275 ) && ( pv.Y < 2003 ) ){ xSet = 251; ySet = 1249; vMap = "Dread Isles"; mSet = Map.IslesDread; }
							else if ( ( pv.X > 1930 ) && ( pv.Y > 1890 ) && ( pv.X < 2022 ) && ( pv.Y < 1997 ) ){ xSet = 3884; ySet = 2879; vMap = "Sosaria"; mSet = Map.Sosaria; }
						}
						else if ( pv.Map == Map.SerpentIsland ) { vMap = "Serpent Island"; xSet = pv.X; ySet = pv.Y; mSet = Map.SerpentIsland; }
						else if ( ( pv.Map == Map.Underworld ) && ( pv.X > 1630 ) )
						{
							if ( ( pv.X > 1644 ) && ( pv.Y > 35 ) && ( pv.X < 1818 ) && ( pv.Y < 163 ) ){ xSet = 4299; ySet = 3318; vMap = "Lodoria"; mSet = Map.Lodor; }
							else if ( ( pv.X > 1864 ) && ( pv.Y > 32 ) && ( pv.X < 2041 ) && ( pv.Y < 162 ) ){ xSet = 177; ySet = 961; vMap = "Savaged Empire"; mSet = Map.SavagedEmpire; }
							else if ( ( pv.X > 2098 ) && ( pv.Y > 27 ) && ( pv.X < 2272 ) && ( pv.Y < 156 ) ){ xSet = 766; ySet = 1527; vMap = "Savaged Empire"; mSet = Map.SavagedEmpire; }
							else if ( ( pv.X > 1647 ) && ( pv.Y > 184 ) && ( pv.X < 1810 ) && ( pv.Y < 305 ) ){ xSet = 1191; ySet = 1516; vMap = "Serpent Island"; mSet = Map.SerpentIsland; }
							else if ( ( pv.X > 1877 ) && ( pv.Y > 187 ) && ( pv.X < 2033 ) && ( pv.Y < 302 ) ){ xSet = 1944; ySet = 3377; vMap = "Umber Veil"; mSet = Map.Sosaria; }
							else if ( ( pv.X > 2108 ) && ( pv.Y > 190 ) && ( pv.X < 2269 ) && ( pv.Y < 305 ) ){ xSet = 1544; ySet = 1785; vMap = "Serpent Island"; mSet = Map.SerpentIsland; }
							else if ( ( pv.X > 1656 ) && ( pv.Y > 335 ) && ( pv.X < 1807 ) && ( pv.Y < 443 ) ){ xSet = 2059; ySet = 2406; vMap = "Sosaria"; mSet = Map.Sosaria; }
							else if ( ( pv.X > 1880 ) && ( pv.Y > 338 ) && ( pv.X < 2031 ) && ( pv.Y < 445 ) ){ xSet = 1558; ySet = 2861; vMap = "Lodoria"; mSet = Map.Lodor; }
							else if ( ( pv.X > 2111 ) && ( pv.Y > 335 ) && ( pv.X < 2266 ) && ( pv.Y < 446 ) ){ xSet = 755; ySet = 1093; vMap = "Dread Isles"; mSet = Map.IslesDread; }
							else if ( ( pv.X > 1657 ) && ( pv.Y > 496 ) && ( pv.X < 1807 ) && ( pv.Y < 606 ) ){ xSet = 2181; ySet = 1327; vMap = "Sosaria"; mSet = Map.Sosaria; }
							else if ( ( pv.X > 1879 ) && ( pv.Y > 498 ) && ( pv.X < 2031 ) && ( pv.Y < 605 ) ){ xSet = 752; ySet = 680; vMap = "Savaged Empire"; mSet = Map.SavagedEmpire; }
							else if ( ( pv.X > 2115 ) && ( pv.Y > 499 ) && ( pv.X < 2263 ) && ( pv.Y < 605 ) ){ xSet = 466; ySet = 3801; vMap = "Ravendark Woods"; mSet = Map.Lodor; }
							else if ( ( pv.X > 1657 ) && ( pv.Y > 641 ) && ( pv.X < 1808 ) && ( pv.Y < 748 ) ){ xSet = 2893; ySet = 2030; vMap = "Lodoria"; mSet = Map.Lodor; }
							else if ( ( pv.X > 1883 ) && ( pv.Y > 640 ) && ( pv.X < 2033 ) && ( pv.Y < 745 ) ){ xSet = 1050; ySet = 93; vMap = "Savaged Empire"; mSet = Map.SavagedEmpire; }
							else if ( ( pv.X > 2113 ) && ( pv.Y > 641 ) && ( pv.X < 2266 ) && ( pv.Y < 747 ) ){ xSet = 127; ySet = 85; vMap = "Dread Isles"; mSet = Map.IslesDread; }
							else if ( ( pv.X > 1657 ) && ( pv.Y > 795 ) && ( pv.X < 1811 ) && ( pv.Y < 898 ) ){ xSet = 145; ySet = 1434; vMap = "Serpent Island"; mSet = Map.SerpentIsland; }
							else if ( ( pv.X > 1883 ) && ( pv.Y > 794 ) && ( pv.X < 2034 ) && ( pv.Y < 902 ) ){ xSet = 2625; ySet = 823; vMap = "Lodoria"; mSet = Map.Lodor; }
							else if ( ( pv.X > 2112 ) && ( pv.Y > 794 ) && ( pv.X < 2267 ) && ( pv.Y < 898 ) ){ xSet = 740; ySet = 182; vMap = "Dread Isles"; mSet = Map.IslesDread; }
							else if ( ( pv.X > 1659 ) && ( pv.Y > 953 ) && ( pv.X < 1809 ) && ( pv.Y < 1059 ) ){ xSet = 5390; ySet = 3280; vMap = "Ambrosia"; mSet = Map.Sosaria; }
							else if ( ( pv.X > 1881 ) && ( pv.Y > 954 ) && ( pv.X < 2034 ) && ( pv.Y < 1059 ) ){ xSet = 922; ySet = 1775; vMap = "Hedge Maze"; mSet = Map.SavagedEmpire; }
							else if ( ( pv.X > 2113 ) && ( pv.Y > 952 ) && ( pv.X < 2268 ) && ( pv.Y < 1056 ) ){ xSet = 1036; ySet = 1162; vMap = "Savaged Empire"; mSet = Map.SavagedEmpire; }
						}
						else if ( pv.Map == Map.Underworld ) { vMap = "Underworld"; xSet = pv.X; ySet = pv.Y; mSet = Map.Underworld; }
						else if ( pv.Map == Map.IslesDread ) { vMap = "Dread Isles"; xSet = pv.X; ySet = pv.Y; mSet = Map.IslesDread; }
						else if ( ( pv.Map == Map.SavagedEmpire ) && ( pv.Y < 1800 ) ) { vMap = "Savaged Empire"; xSet = pv.X; ySet = pv.Y; mSet = Map.SavagedEmpire; }
						else if ( pv.Map == Map.Sosaria && pv.X > 5125 && pv.Y > 3038 && pv.X < 6124 && pv.Y < 4093 ){ vMap = "Ambrosia"; xSet = pv.X; ySet = pv.Y; mSet = Map.Sosaria; }
						else if ( pv.Map == Map.Sosaria && pv.X > 699 && pv.Y > 3129 && pv.X < 2272 && pv.Y < 4095 ){ vMap = "Umber Veil"; xSet = pv.X; ySet = pv.Y; mSet = Map.Sosaria; }
						else if ( pv.Map == Map.Sosaria && pv.X > 6127 && pv.Y > 828 && pv.X < 7168 && pv.Y < 2738 ){ vMap = "Bottle World"; xSet = pv.X; ySet = pv.Y; mSet = Map.Sosaria; }
						else { vMap = "Sosaria"; xSet = pv.X; ySet = pv.Y; mSet = Map.Sosaria; }

						int xLong = 0, yLat = 0;
						int xMins = 0, yMins = 0;
						bool xEast = false, ySouth = false;

						Point3D spot = new Point3D(xSet, ySet, 0);
						string my_location = pv.Location.ToString();

						if ( Sextant.Format( spot, mSet, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
						{
							my_location = String.Format( "{0}� {1}'{2}, {3}� {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W" );
						}

						AddLabel(32, 46 +(row * 20), 1152, String.Format( "{0}", pv.ShopName ));
						AddLabel(250, 46 +(row * 20), 1152, String.Format( "{0}", pv.Owner.Name ));
						AddLabel(415, 46 +(row * 20), 1152, String.Format( "{0} {1}", my_location, vMap));

						if ( pv == null )
						{
							Console.WriteLine("No Vendors In Shard...");
							return;
						}
					}
				}
				catch {}
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile;

				int buttonID = info.ButtonID;
				if ( buttonID == 2 )
				{
					m_Page ++;
					from.CloseGump( typeof( FindPlayerVendorsGump ) );
					from.SendGump( new FindPlayerVendorsGump( from, m_List, m_Page ) );
				}
				if ( buttonID == 1 )
				{
					m_Page --;
					from.CloseGump( typeof( FindPlayerVendorsGump ) );
					from.SendGump( new FindPlayerVendorsGump( from, m_List, m_Page ) );
				}
				if ( buttonID > 100 )
				{
					int index = buttonID - 101;
					PlayerVendor pv = m_List[index] as PlayerVendor;
					Point3D xyz = pv.Location;
					int x = xyz.X;
					int y = xyz.Y;
					int z = xyz.Z;

					Point3D dest = new Point3D( x, y, z );
					from.MoveToWorld( dest, pv.Map );
					from.SendGump( new FindPlayerVendorsGump( from, m_List, m_Page ) );
					
				}
			}
		}

		public AdvertiserVendor(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}