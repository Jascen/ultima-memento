using System;
using Server.Network;
using Server.Gumps;
using Server.Spells;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Misc;
using System.Collections;
using Server.Regions;

namespace Server.Items
{
	[FlipableAttribute( 0x6725, 0x6726 )]
	public class MysticSpellbook : Spellbook
	{
		public override string DefaultDescription{ get{ return "This book is used by mystics, in order for them to use various abilities akin to this ancient order of peaceful nomads. Dropping such scrolls onto this book will place the mystical abilities within its pages. The scrolls can lead a mystic far and wide, as they search for the places of worship and meditate to acquire the knowledge."; } }

		public override SpellbookType SpellbookType{ get{ return SpellbookType.Mystic; } }
		public override int BookOffset{ get{ return 250; } }
		public override int BookCount{ get{ return 10; } }

		[Constructable]
		public MysticSpellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public MysticSpellbook( ulong content ) : base( content, 0x6725 )
		{
			Name = "monk's tome";

			if ( owner == null )
			{
				int[] trammel = { 1, 2, 3, 4, 5, 6, 7, 8 };
				Shuffle(trammel);
				int t = 0;
				foreach (int tram in trammel)
				{
					t++;
					if ( t == 1 ){ SetPrayer( this, tram, 3 ); }
					else if ( t == 2 ){ SetPrayer( this, tram, 4 ); }
					else if ( t == 3 ){ SetPrayer( this, tram, 5 ); }
					else if ( t == 4 ){ SetPrayer( this, tram, 8 ); }
				}

				int[] felucca = { 9, 10, 11, 12, 13, 14, 15 };
				Shuffle(felucca);
				int f = 0;
				foreach (int fel in felucca)
				{
					f++;
					if ( f == 1 ){ SetPrayer( this, fel, 2 ); }
					else if ( f == 2 ){ SetPrayer( this, fel, 6 ); }
					else if ( f == 3 ){ SetPrayer( this, fel, 7 ); }
					else if ( f == 4 ){ SetPrayer( this, fel, 9 ); }
				}

				int[] other = { 16, 17, 18, 19 };
				Shuffle(other);
				int o = 0;
				foreach (int oth in other)
				{
					o++;
					if ( o == 1 ){ SetPrayer( this, oth, 1 ); }
					else if ( o == 2 ){ SetPrayer( this, oth, 10 ); }
				}

				switch ( Utility.Random( 4 ) )
				{
					case 0: PackShrine = "Shrine of Intelligence"; 	break;
					case 1: PackShrine = "Shrine of Strength"; 		break;
					case 2: PackShrine = "Shrine of Wisdom"; 		break;
					case 3: PackShrine = "Shrine of Dexterity"; 	break;
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			Container pack = from.Backpack;

			if ( Owner == null && from is PlayerMobile )
			{
				Owner = from;
				from.SendMessage( "You use your ki to bind the book to you." );
			}

			if ( from != owner )
			{
				from.SendMessage( "The book doesn't seem to open." );
			}
			else if ( PackTest( this, from ) )
			{
			}
			else if ( KiTest( this, from ) )
			{
			}
			else if ( Parent == from || ( pack != null && Parent == pack ) )
			{
				from.SendSound( 0x55 );
				from.CloseGump( typeof( MysticSpellbookGump ) );
				from.SendGump( new MysticSpellbookGump( from, this, 1 ) );
			}
			else from.SendLocalizedMessage(500207); // The spellbook must be in your backpack (and not in a container within) to open.
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			if ( owner != null ){ list.Add( 1070722, "Belongs to " + owner.Name + "" ); }
        }

		static Random _random = new Random();

		static void Shuffle<T>(T[] array)
		{
			int n = array.Length;
			for (int i = 0; i < n; i++)
			{
				int r = i + _random.Next(n - i);
				T t = array[r];
				array[r] = array[i];
				array[i] = t;
			}
		}

		public static bool PackTest( MysticSpellbook book, Mobile from )
		{
			if ( from.Region.Name == book.PackShrine && from.Skills[SkillName.FistFighting].Value >= 100 && Server.Misc.GetPlayerInfo.isMonk( from ) )
			{
				Item gem = null;

				if ( from.Backpack.FindItemByType( typeof ( Oyster ) ) != null )
				{
					gem = from.Backpack.FindItemByType( typeof ( Oyster ) );
				}

				if ( gem != null )
				{
					ArrayList targets = new ArrayList();
					foreach ( Item item in World.Items.Values )
					if ( item is MysticPack )
					{
						if ( ((MysticPack)item).owner == from )
						{
							targets.Add( item );
						}
					}
					for ( int i = 0; i < targets.Count; ++i )
					{
						Item item = ( Item )targets[ i ];
						item.Delete();
					}

					gem.Delete();
					MysticPack bag = new MysticPack();
					bag.owner = from;
					from.AddToBackpack( bag );
					from.SendMessage( "You summon a monk's rucksack from the astral plane." );

					string[] chant = new string[] {"Ahm", "Mu", "Ra", "Beh", "Cah", "Summ", "Om", "Lum"};
						string pray_chant_1 = chant[Utility.RandomMinMax( 0, (chant.Length-1) )];
						string pray_chant_2 = chant[Utility.RandomMinMax( 0, (chant.Length-1) )];
						string pray_chant_3 = chant[Utility.RandomMinMax( 0, (chant.Length-1) )];
						string pray_chant_4 = chant[Utility.RandomMinMax( 0, (chant.Length-1) )];

					string pray_chant = pray_chant_1 + " " + pray_chant_2 + " " + pray_chant_3 + " " + pray_chant_4;

					if ( from.Karma < 0 )
					{
						from.Say( pray_chant );
						from.PlaySound( 0x481 );
						return true;
					}
					else
					{
						from.Say( pray_chant );
						from.PlaySound( 0x24A );
						return true;
					}
				}
			}

			return false;
		}

		public static bool KiTest( MysticSpellbook book, Mobile from )
		{
			Item scroll = null;
			string speak = "";

			if ( from.Backpack.FindItemByType( typeof ( BlankScroll ) ) != null )
			{
				scroll = from.Backpack.FindItemByType( typeof ( BlankScroll ) );
			}

			if ( !book.HasSpell( 250 ) && from.Map == book.WritMap01 && from.X >= book.WritX101 && from.Y >= book.WritY101 && from.X <= book.WritX201 && from.Y <= book.WritY201 )
			{
				if ( scroll != null )
				{
					RemoveOldScroll( from, 250 );
					if ( scroll.Amount < 2 ){ scroll.Delete(); } else { scroll.Amount--; }
					AstralProjectionScroll paper = new AstralProjectionScroll();
					paper.owner = from;
					from.AddToBackpack( paper );
					speak = book.WritChant01;
					from.SendMessage( "You learn the secrets of astral projection." );
				}
				else
				{
					from.SendMessage( "You do not have a blank scroll to record what you learned." );
				}
			}
			else if ( !book.HasSpell( 251 ) && from.Map == book.WritMap02 && from.X >= book.WritX102 && from.Y >= book.WritY102 && from.X <= book.WritX202 && from.Y <= book.WritY202 )
			{
				if ( scroll != null )
				{
					RemoveOldScroll( from, 251 );
					if ( scroll.Amount < 2 ){ scroll.Delete(); } else { scroll.Amount--; }
					AstralTravelScroll paper = new AstralTravelScroll();
					paper.owner = from;
					from.AddToBackpack( paper );
					speak = book.WritChant02;
					from.SendMessage( "You learn the secrets of astral travel." );
				}
				else
				{
					from.SendMessage( "You do not have a blank scroll to record what you learned." );
				}
			}
			else if ( !book.HasSpell( 252 ) && from.Map == book.WritMap03 && from.X >= book.WritX103 && from.Y >= book.WritY103 && from.X <= book.WritX203 && from.Y <= book.WritY203 )
			{
				if ( scroll != null )
				{
					RemoveOldScroll( from, 252 );
					if ( scroll.Amount < 2 ){ scroll.Delete(); } else { scroll.Amount--; }
					CreateRobeScroll paper = new CreateRobeScroll();
					paper.owner = from;
					from.AddToBackpack( paper );
					speak = book.WritChant03;
					from.SendMessage( "You learn the secrets of creating mystical monk robes." );
				}
				else
				{
					from.SendMessage( "You do not have a blank scroll to record what you learned." );
				}
			}
			else if ( !book.HasSpell( 253 ) && from.Map == book.WritMap04 && from.X >= book.WritX104 && from.Y >= book.WritY104 && from.X <= book.WritX204 && from.Y <= book.WritY204 )
			{
				if ( scroll != null )
				{
					RemoveOldScroll( from, 253 );
					if ( scroll.Amount < 2 ){ scroll.Delete(); } else { scroll.Amount--; }
					GentleTouchScroll paper = new GentleTouchScroll();
					paper.owner = from;
					from.AddToBackpack( paper );
					speak = book.WritChant04;
					from.SendMessage( "You learn the secrets of the gentle touch." );
				}
				else
				{
					from.SendMessage( "You do not have a blank scroll to record what you learned." );
				}
			}
			else if ( !book.HasSpell( 254 ) && from.Map == book.WritMap05 && from.X >= book.WritX105 && from.Y >= book.WritY105 && from.X <= book.WritX205 && from.Y <= book.WritY205 )
			{
				if ( scroll != null )
				{
					RemoveOldScroll( from, 254 );
					if ( scroll.Amount < 2 ){ scroll.Delete(); } else { scroll.Amount--; }
					LeapScroll paper = new LeapScroll();
					paper.owner = from;
					from.AddToBackpack( paper );
					speak = book.WritChant05;
					from.SendMessage( "You learn the secrets of leaping." );
				}
				else
				{
					from.SendMessage( "You do not have a blank scroll to record what you learned." );
				}
			}
			else if ( !book.HasSpell( 255 ) && from.Map == book.WritMap06 && from.X >= book.WritX106 && from.Y >= book.WritY106 && from.X <= book.WritX206 && from.Y <= book.WritY206 )
			{
				if ( scroll != null )
				{
					RemoveOldScroll( from, 255 );
					if ( scroll.Amount < 2 ){ scroll.Delete(); } else { scroll.Amount--; }
					PsionicBlastScroll paper = new PsionicBlastScroll();
					paper.owner = from;
					from.AddToBackpack( paper );
					speak = book.WritChant06;
					from.SendMessage( "You learn the secrets of the psionic blast." );
				}
				else
				{
					from.SendMessage( "You do not have a blank scroll to record what you learned." );
				}
			}
			else if ( !book.HasSpell( 256 ) && from.Map == book.WritMap07 && from.X >= book.WritX107 && from.Y >= book.WritY107 && from.X <= book.WritX207 && from.Y <= book.WritY207 )
			{
				if ( scroll != null )
				{
					RemoveOldScroll( from, 256 );
					if ( scroll.Amount < 2 ){ scroll.Delete(); } else { scroll.Amount--; }
					PsychicWallScroll paper = new PsychicWallScroll();
					paper.owner = from;
					from.AddToBackpack( paper );
					speak = book.WritChant07;
					from.SendMessage( "You learn the secrets of the psychic wall." );
				}
				else
				{
					from.SendMessage( "You do not have a blank scroll to record what you learned." );
				}
			}
			else if ( !book.HasSpell( 257 ) && from.Map == book.WritMap08 && from.X >= book.WritX108 && from.Y >= book.WritY108 && from.X <= book.WritX208 && from.Y <= book.WritY208 )
			{
				if ( scroll != null )
				{
					RemoveOldScroll( from, 257 );
					if ( scroll.Amount < 2 ){ scroll.Delete(); } else { scroll.Amount--; }
					PurityOfBodyScroll paper = new PurityOfBodyScroll();
					paper.owner = from;
					from.AddToBackpack( paper );
					speak = book.WritChant08;
					from.SendMessage( "You learn the secrets of the purity of the body." );
				}
				else
				{
					from.SendMessage( "You do not have a blank scroll to record what you learned." );
				}
			}
			else if ( !book.HasSpell( 258 ) && from.Map == book.WritMap09 && from.X >= book.WritX109 && from.Y >= book.WritY109 && from.X <= book.WritX209 && from.Y <= book.WritY209 )
			{
				if ( scroll != null )
				{
					RemoveOldScroll( from, 258 );
					if ( scroll.Amount < 2 ){ scroll.Delete(); } else { scroll.Amount--; }
					QuiveringPalmScroll paper = new QuiveringPalmScroll();
					paper.owner = from;
					from.AddToBackpack( paper );
					speak = book.WritChant09;
					from.SendMessage( "You learn the secrets of the quivering palm." );
				}
				else
				{
					from.SendMessage( "You do not have a blank scroll to record what you learned." );
				}
			}
			else if ( !book.HasSpell( 259 ) && from.Map == book.WritMap10 && from.X >= book.WritX110 && from.Y >= book.WritY110 && from.X <= book.WritX210 && from.Y <= book.WritY210 )
			{
				if ( scroll != null )
				{
					RemoveOldScroll( from, 259 );
					if ( scroll.Amount < 2 ){ scroll.Delete(); } else { scroll.Amount--; }
					WindRunnerScroll paper = new WindRunnerScroll();
					paper.owner = from;
					from.AddToBackpack( paper );
					speak = book.WritChant10;
					from.SendMessage( "You learn the secrets of running like the wind." );
				}
				else
				{
					from.SendMessage( "You do not have a blank scroll to record what you learned." );
				}
			}

			if ( from.Karma < 0 && speak != "" )
			{
				from.Say( speak );
				from.PlaySound( 0x481 );
				return true;
			}
			else if ( speak != "" )
			{
				from.Say( speak );
				from.PlaySound( 0x24A );
				return true;
			}

			return false;
		}

		public static void RemoveOldScroll( Mobile from, int scroll )
		{
			if ( from is PlayerMobile )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				if ( item is AstralProjectionScroll && scroll == 250 )
				{
					if ( ((AstralProjectionScroll)item).owner == from )
						targets.Add( item );
				}
				else if ( item is AstralTravelScroll && scroll == 251 )
				{
					if ( ((AstralTravelScroll)item).owner == from )
						targets.Add( item );
				}
				else if ( item is CreateRobeScroll && scroll == 252 )
				{
					if ( ((CreateRobeScroll)item).owner == from )
						targets.Add( item );
				}
				else if ( item is GentleTouchScroll && scroll == 253 )
				{
					if ( ((GentleTouchScroll)item).owner == from )
						targets.Add( item );
				}
				else if ( item is LeapScroll && scroll == 254 )
				{
					if ( ((LeapScroll)item).owner == from )
						targets.Add( item );
				}
				else if ( item is PsionicBlastScroll && scroll == 255 )
				{
					if ( ((PsionicBlastScroll)item).owner == from )
						targets.Add( item );
				}
				else if ( item is PsychicWallScroll && scroll == 256 )
				{
					if ( ((PsychicWallScroll)item).owner == from )
						targets.Add( item );
				}
				else if ( item is PurityOfBodyScroll && scroll == 257 )
				{
					if ( ((PurityOfBodyScroll)item).owner == from )
						targets.Add( item );
				}
				else if ( item is QuiveringPalmScroll && scroll == 258 )
				{
					if ( ((QuiveringPalmScroll)item).owner == from )
						targets.Add( item );
				}
				else if ( item is WindRunnerScroll && scroll == 259 )
				{
					if ( ((WindRunnerScroll)item).owner == from )
						targets.Add( item );
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					item.Delete();
				}
			}
		}

		public static void SetPrayer( MysticSpellbook book, int pray, int page )
		{
			string pray_place = "";
			Map pray_map = Map.Sosaria;
			int pray_x1 = 0;
			int pray_y1 = 0;
			int pray_x2 = 0;
			int pray_y2 = 0;
			Point3D pray_location = new Point3D( 0, 0, 0 );

			if ( pray == 1 )
			{
				pray_place = "the Pool of the Twin Goddesses";
				pray_map = Map.Sosaria;
				pray_x1 = 1015;
				pray_y1 = 3437;
				pray_x2 = 1029;
				pray_y2 = 3450;
				pray_location = new Point3D( 1023, 3444, 15 );
			}
			else if ( pray == 2 )
			{
				pray_place = "the Fountain of the Druids";
				pray_map = Map.Sosaria;
				pray_x1 = 1484;
				pray_y1 = 538;
				pray_x2 = 1500;
				pray_y2 = 553;
				pray_location = new Point3D( 1492, 550, 0 );
			}
			else if ( pray == 3 )
			{
				pray_place = "the Lady of the Flame";
				pray_map = Map.Sosaria;
				pray_x1 = 4950;
				pray_y1 = 1314;
				pray_x2 = 4963;
				pray_y2 = 1328;
				pray_location = new Point3D( 4956, 1320, 0 );
			}
			else if ( pray == 4 )
			{
				pray_place = "the Monolith of Everfrost";
				pray_map = Map.Sosaria;
				pray_x1 = 4033;
				pray_y1 = 2933;
				pray_x2 = 4055;
				pray_y2 = 2951;
				pray_location = new Point3D( 4044, 2944, 0 );
			}
			else if ( pray == 5 )
			{
				pray_place = "the Shrine of Atonement";
				pray_map = Map.Sosaria;
				pray_x1 = 2251;
				pray_y1 = 2435;
				pray_x2 = 2259;
				pray_y2 = 2443;
				pray_location = new Point3D( 2255, 2439, 7 );
			}
			else if ( pray == 6 )
			{
				pray_place = "the Tree of Enlightened Tears";
				pray_map = Map.Sosaria;
				pray_x1 = 871;
				pray_y1 = 380;
				pray_x2 = 882;
				pray_y2 = 391;
				pray_location = new Point3D( 875, 384, 0 );
			}
			else if ( pray == 7 )
			{
				pray_place = "the Pass of Frostmarch";
				pray_map = Map.Sosaria;
				pray_x1 = 4768;
				pray_y1 = 1265;
				pray_x2 = 4798;
				pray_y2 = 1280;
				pray_location = new Point3D( 4782, 1273, 0 );
			}
			else if ( pray == 8 )
			{
				pray_place = "the Grave of Gargix Zul";
				pray_map = Map.Sosaria;
				pray_x1 = 3460;
				pray_y1 = 1897;
				pray_x2 = 3467;
				pray_y2 = 1904;
				pray_location = new Point3D( 3464, 1901, 0 );
			}
			else if ( pray == 9 )
			{
				pray_place = "the Tomb of Brun Stormaxe";
				pray_map = Map.Lodor;
				pray_x1 = 2620;
				pray_y1 = 2083;
				pray_x2 = 2637;
				pray_y2 = 2099;
				pray_location = new Point3D( 2628, 2092, 7 );
			}
			else if ( pray == 10 )
			{
				pray_place = "the Oasis of Faurel Xan";
				pray_map = Map.Lodor;
				pray_x1 = 1304;
				pray_y1 = 3007;
				pray_x2 = 1325;
				pray_y2 = 3026;
				pray_location = new Point3D( 1314, 3018, 0 );
			}
			else if ( pray == 11 )
			{
				pray_place = "the Thicket of Souls";
				pray_map = Map.Lodor;
				pray_x1 = 1274;
				pray_y1 = 2070;
				pray_x2 = 1299;
				pray_y2 = 2096;
				pray_location = new Point3D( 1288, 2083, 2 );
			}
			else if ( pray == 12 )
			{
				pray_place = "the Pixie Garden of Yal Sar";
				pray_map = Map.Lodor;
				pray_x1 = 4215;
				pray_y1 = 561;
				pray_x2 = 4227;
				pray_y2 = 572;
				pray_location = new Point3D( 4222, 567, 2 );
			}
			else if ( pray == 13 )
			{
				pray_place = "the Ruins of Hammerfell";
				pray_map = Map.Lodor;
				pray_x1 = 1474;
				pray_y1 = 3460;
				pray_x2 = 1490;
				pray_y2 = 3478;
				pray_location = new Point3D( 1482, 3470, 0 );
			}
			else if ( pray == 14 )
			{
				pray_place = "the Cairn of the Demon Eye";
				pray_map = Map.Lodor;
				pray_x1 = 2893;
				pray_y1 = 562;
				pray_x2 = 2911;
				pray_y2 = 580;
				pray_location = new Point3D( 2903, 573, 19 );
			}
			else if ( pray == 15 )
			{
				pray_place = "the Statue of the Inferno Lord";
				pray_map = Map.Lodor;
				pray_x1 = 1386;
				pray_y1 = 997;
				pray_x2 = 1409;
				pray_y2 = 1034;
				pray_location = new Point3D( 1398, 1017, 2 );
			}
			else if ( pray == 16 )
			{
				pray_place = "the Monument of Dulan Gor";
				pray_map = Map.SavagedEmpire;
				pray_x1 = 494;
				pray_y1 = 1110;
				pray_x2 = 502;
				pray_y2 = 1128;
				pray_location = new Point3D( 496, 1118, 2 );
			}
			else if ( pray == 17 )
			{
				pray_place = "the Statue of Vulkar the Ice King";
				pray_map = Map.IslesDread;
				pray_x1 = 412;
				pray_y1 = 451;
				pray_x2 = 430;
				pray_y2 = 473;
				pray_location = new Point3D( 424, 461, 34 );
			}
			else if ( pray == 18 )
			{
				pray_place = "the Ruins of Moonshade";
				pray_map = Map.SerpentIsland;
				pray_x1 = 166;
				pray_y1 = 1444;
				pray_x2 = 182;
				pray_y2 = 1467;
				pray_location = new Point3D( 175, 1456, 2 );
			}
			else if ( pray == 19 )
			{
				pray_place = "the Altar of Gru Bloodfist";
				pray_map = Map.SavagedEmpire;
				pray_x1 = 952;
				pray_y1 = 1076;
				pray_x2 = 966;
				pray_y2 = 1089;
				pray_location = new Point3D( 959, 1082, 5 );
			}

			int xLong = 0, yLat = 0;
			int xMins = 0, yMins = 0;
			bool xEast = false, ySouth = false;

			string pray_world = Server.Lands.LandName( Server.Lands.GetLand( pray_map, pray_location, pray_x1, pray_y1 ) );

			string pray_coords = "";
			if ( Sextant.Format( pray_location, pray_map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
			{
				pray_coords = String.Format( "{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W" );
			}

			string[] chant = new string[] {"Ahm", "Mu", "Ra", "Beh", "Cah", "Summ", "Om", "Lum"};
				string pray_chant_1 = chant[Utility.RandomMinMax( 0, (chant.Length-1) )];
				string pray_chant_2 = chant[Utility.RandomMinMax( 0, (chant.Length-1) )];
				string pray_chant_3 = chant[Utility.RandomMinMax( 0, (chant.Length-1) )];
				string pray_chant_4 = chant[Utility.RandomMinMax( 0, (chant.Length-1) )];

			string pray_chant = pray_chant_1 + " " + pray_chant_2 + " " + pray_chant_3 + " " + pray_chant_4;

			if ( page == 1 )
			{
				book.WritPlace01 = pray_place;
				book.WritWorld01 = pray_world;
				book.WritCoord01 = pray_coords;
				book.WritChant01 = pray_chant;
				book.WritMap01 = pray_map;
				book.WritX101 = pray_x1;
				book.WritY101 = pray_y1;
				book.WritX201 = pray_x2;
				book.WritY201 = pray_y2;
			}
			else if ( page == 2 )
			{
				book.WritPlace02 = pray_place;
				book.WritWorld02 = pray_world;
				book.WritCoord02 = pray_coords;
				book.WritChant02 = pray_chant;
				book.WritMap02 = pray_map;
				book.WritX102 = pray_x1;
				book.WritY102 = pray_y1;
				book.WritX202 = pray_x2;
				book.WritY202 = pray_y2;
			}
			else if ( page == 3 )
			{
				book.WritPlace03 = pray_place;
				book.WritWorld03 = pray_world;
				book.WritCoord03 = pray_coords;
				book.WritChant03 = pray_chant;
				book.WritMap03 = pray_map;
				book.WritX103 = pray_x1;
				book.WritY103 = pray_y1;
				book.WritX203 = pray_x2;
				book.WritY203 = pray_y2;
			}
			else if ( page == 4 )
			{
				book.WritPlace04 = pray_place;
				book.WritWorld04 = pray_world;
				book.WritCoord04 = pray_coords;
				book.WritChant04 = pray_chant;
				book.WritMap04 = pray_map;
				book.WritX104 = pray_x1;
				book.WritY104 = pray_y1;
				book.WritX204 = pray_x2;
				book.WritY204 = pray_y2;
			}
			else if ( page == 5 )
			{
				book.WritPlace05 = pray_place;
				book.WritWorld05 = pray_world;
				book.WritCoord05 = pray_coords;
				book.WritChant05 = pray_chant;
				book.WritMap05 = pray_map;
				book.WritX105 = pray_x1;
				book.WritY105 = pray_y1;
				book.WritX205 = pray_x2;
				book.WritY205 = pray_y2;
			}
			else if ( page == 6 )
			{
				book.WritPlace06 = pray_place;
				book.WritWorld06 = pray_world;
				book.WritCoord06 = pray_coords;
				book.WritChant06 = pray_chant;
				book.WritMap06 = pray_map;
				book.WritX106 = pray_x1;
				book.WritY106 = pray_y1;
				book.WritX206 = pray_x2;
				book.WritY206 = pray_y2;
			}
			else if ( page == 7 )
			{
				book.WritPlace07 = pray_place;
				book.WritWorld07 = pray_world;
				book.WritCoord07 = pray_coords;
				book.WritChant07 = pray_chant;
				book.WritMap07 = pray_map;
				book.WritX107 = pray_x1;
				book.WritY107 = pray_y1;
				book.WritX207 = pray_x2;
				book.WritY207 = pray_y2;
			}
			else if ( page == 8 )
			{
				book.WritPlace08 = pray_place;
				book.WritWorld08 = pray_world;
				book.WritCoord08 = pray_coords;
				book.WritChant08 = pray_chant;
				book.WritMap08 = pray_map;
				book.WritX108 = pray_x1;
				book.WritY108 = pray_y1;
				book.WritX208 = pray_x2;
				book.WritY208 = pray_y2;
			}
			else if ( page == 9 )
			{
				book.WritPlace09 = pray_place;
				book.WritWorld09 = pray_world;
				book.WritCoord09 = pray_coords;
				book.WritChant09 = pray_chant;
				book.WritMap09 = pray_map;
				book.WritX109 = pray_x1;
				book.WritY109 = pray_y1;
				book.WritX209 = pray_x2;
				book.WritY209 = pray_y2;
			}
			else if ( page == 10 )
			{
				book.WritPlace10 = pray_place;
				book.WritWorld10 = pray_world;
				book.WritCoord10 = pray_coords;
				book.WritChant10 = pray_chant;
				book.WritMap10 = pray_map;
				book.WritX110 = pray_x1;
				book.WritY110 = pray_y1;
				book.WritX210 = pray_x2;
				book.WritY210 = pray_y2;
			}
		}

		public Mobile owner;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner { get{ return owner; } set{ owner = value; InvalidateProperties(); } }

		public string PackShrine;
		[CommandProperty(AccessLevel.Owner)]
		public string Pack_Shrine { get { return PackShrine; } set { PackShrine = value; InvalidateProperties(); } }

		public string WritPlace01;
		[CommandProperty(AccessLevel.Owner)]
		public string WritPlace_01 { get { return WritPlace01; } set { WritPlace01 = value; InvalidateProperties(); } }

		public string WritWorld01;
		[CommandProperty(AccessLevel.Owner)]
		public string WritWorld_01 { get { return WritWorld01; } set { WritWorld01 = value; InvalidateProperties(); } }

		public string WritCoord01;
		[CommandProperty(AccessLevel.Owner)]
		public string WritCoord_01 { get { return WritCoord01; } set { WritCoord01 = value; InvalidateProperties(); } }

		public string WritChant01;
		[CommandProperty(AccessLevel.Owner)]
		public string WritChant_01 { get { return WritChant01; } set { WritChant01 = value; InvalidateProperties(); } }

		public Map WritMap01;
		[CommandProperty(AccessLevel.Owner)]
		public Map WritMap_01 { get { return WritMap01; } set { WritMap01 = value; InvalidateProperties(); } }

		public int WritX101;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX1_01 { get { return WritX101; } set { WritX101 = value; InvalidateProperties(); } }

		public int WritY101;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY1_01 { get { return WritY101; } set { WritY101 = value; InvalidateProperties(); } }

		public int WritX201;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX2_01 { get { return WritX201; } set { WritX201 = value; InvalidateProperties(); } }

		public int WritY201;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY2_01 { get { return WritY201; } set { WritY201 = value; InvalidateProperties(); } }

		public string WritPlace02;
		[CommandProperty(AccessLevel.Owner)]
		public string WritPlace_02 { get { return WritPlace02; } set { WritPlace02 = value; InvalidateProperties(); } }

		public string WritWorld02;
		[CommandProperty(AccessLevel.Owner)]
		public string WritWorld_02 { get { return WritWorld02; } set { WritWorld02 = value; InvalidateProperties(); } }

		public string WritCoord02;
		[CommandProperty(AccessLevel.Owner)]
		public string WritCoord_02 { get { return WritCoord02; } set { WritCoord02 = value; InvalidateProperties(); } }

		public string WritChant02;
		[CommandProperty(AccessLevel.Owner)]
		public string WritChant_02 { get { return WritChant02; } set { WritChant02 = value; InvalidateProperties(); } }

		public Map WritMap02;
		[CommandProperty(AccessLevel.Owner)]
		public Map WritMap_02 { get { return WritMap02; } set { WritMap02 = value; InvalidateProperties(); } }

		public int WritX102;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX1_02 { get { return WritX102; } set { WritX102 = value; InvalidateProperties(); } }

		public int WritY102;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY1_02 { get { return WritY102; } set { WritY102 = value; InvalidateProperties(); } }

		public int WritX202;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX2_02 { get { return WritX202; } set { WritX202 = value; InvalidateProperties(); } }

		public int WritY202;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY2_02 { get { return WritY202; } set { WritY202 = value; InvalidateProperties(); } }

		public string WritPlace03;
		[CommandProperty(AccessLevel.Owner)]
		public string WritPlace_03 { get { return WritPlace03; } set { WritPlace03 = value; InvalidateProperties(); } }

		public string WritWorld03;
		[CommandProperty(AccessLevel.Owner)]
		public string WritWorld_03 { get { return WritWorld03; } set { WritWorld03 = value; InvalidateProperties(); } }

		public string WritCoord03;
		[CommandProperty(AccessLevel.Owner)]
		public string WritCoord_03 { get { return WritCoord03; } set { WritCoord03 = value; InvalidateProperties(); } }

		public string WritChant03;
		[CommandProperty(AccessLevel.Owner)]
		public string WritChant_03 { get { return WritChant03; } set { WritChant03 = value; InvalidateProperties(); } }

		public Map WritMap03;
		[CommandProperty(AccessLevel.Owner)]
		public Map WritMap_03 { get { return WritMap03; } set { WritMap03 = value; InvalidateProperties(); } }

		public int WritX103;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX1_03 { get { return WritX103; } set { WritX103 = value; InvalidateProperties(); } }

		public int WritY103;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY1_03 { get { return WritY103; } set { WritY103 = value; InvalidateProperties(); } }

		public int WritX203;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX2_03 { get { return WritX203; } set { WritX203 = value; InvalidateProperties(); } }

		public int WritY203;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY2_03 { get { return WritY203; } set { WritY203 = value; InvalidateProperties(); } }

		public string WritPlace04;
		[CommandProperty(AccessLevel.Owner)]
		public string WritPlace_04 { get { return WritPlace04; } set { WritPlace04 = value; InvalidateProperties(); } }

		public string WritWorld04;
		[CommandProperty(AccessLevel.Owner)]
		public string WritWorld_04 { get { return WritWorld04; } set { WritWorld04 = value; InvalidateProperties(); } }

		public string WritCoord04;
		[CommandProperty(AccessLevel.Owner)]
		public string WritCoord_04 { get { return WritCoord04; } set { WritCoord04 = value; InvalidateProperties(); } }

		public string WritChant04;
		[CommandProperty(AccessLevel.Owner)]
		public string WritChant_04 { get { return WritChant04; } set { WritChant04 = value; InvalidateProperties(); } }

		public Map WritMap04;
		[CommandProperty(AccessLevel.Owner)]
		public Map WritMap_04 { get { return WritMap04; } set { WritMap04 = value; InvalidateProperties(); } }

		public int WritX104;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX1_04 { get { return WritX104; } set { WritX104 = value; InvalidateProperties(); } }

		public int WritY104;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY1_04 { get { return WritY104; } set { WritY104 = value; InvalidateProperties(); } }

		public int WritX204;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX2_04 { get { return WritX204; } set { WritX204 = value; InvalidateProperties(); } }

		public int WritY204;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY2_04 { get { return WritY204; } set { WritY204 = value; InvalidateProperties(); } }

		public string WritPlace05;
		[CommandProperty(AccessLevel.Owner)]
		public string WritPlace_05 { get { return WritPlace05; } set { WritPlace05 = value; InvalidateProperties(); } }

		public string WritWorld05;
		[CommandProperty(AccessLevel.Owner)]
		public string WritWorld_05 { get { return WritWorld05; } set { WritWorld05 = value; InvalidateProperties(); } }

		public string WritCoord05;
		[CommandProperty(AccessLevel.Owner)]
		public string WritCoord_05 { get { return WritCoord05; } set { WritCoord05 = value; InvalidateProperties(); } }

		public string WritChant05;
		[CommandProperty(AccessLevel.Owner)]
		public string WritChant_05 { get { return WritChant05; } set { WritChant05 = value; InvalidateProperties(); } }

		public Map WritMap05;
		[CommandProperty(AccessLevel.Owner)]
		public Map WritMap_05 { get { return WritMap05; } set { WritMap05 = value; InvalidateProperties(); } }

		public int WritX105;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX1_05 { get { return WritX105; } set { WritX105 = value; InvalidateProperties(); } }

		public int WritY105;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY1_05 { get { return WritY105; } set { WritY105 = value; InvalidateProperties(); } }

		public int WritX205;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX2_05 { get { return WritX205; } set { WritX205 = value; InvalidateProperties(); } }

		public int WritY205;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY2_05 { get { return WritY205; } set { WritY205 = value; InvalidateProperties(); } }

		public string WritPlace06;
		[CommandProperty(AccessLevel.Owner)]
		public string WritPlace_06 { get { return WritPlace06; } set { WritPlace06 = value; InvalidateProperties(); } }

		public string WritWorld06;
		[CommandProperty(AccessLevel.Owner)]
		public string WritWorld_06 { get { return WritWorld06; } set { WritWorld06 = value; InvalidateProperties(); } }

		public string WritCoord06;
		[CommandProperty(AccessLevel.Owner)]
		public string WritCoord_06 { get { return WritCoord06; } set { WritCoord06 = value; InvalidateProperties(); } }

		public string WritChant06;
		[CommandProperty(AccessLevel.Owner)]
		public string WritChant_06 { get { return WritChant06; } set { WritChant06 = value; InvalidateProperties(); } }

		public Map WritMap06;
		[CommandProperty(AccessLevel.Owner)]
		public Map WritMap_06 { get { return WritMap06; } set { WritMap06 = value; InvalidateProperties(); } }

		public int WritX106;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX1_06 { get { return WritX106; } set { WritX106 = value; InvalidateProperties(); } }

		public int WritY106;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY1_06 { get { return WritY106; } set { WritY106 = value; InvalidateProperties(); } }

		public int WritX206;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX2_06 { get { return WritX206; } set { WritX206 = value; InvalidateProperties(); } }

		public int WritY206;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY2_06 { get { return WritY206; } set { WritY206 = value; InvalidateProperties(); } }

		public string WritPlace07;
		[CommandProperty(AccessLevel.Owner)]
		public string WritPlace_07 { get { return WritPlace07; } set { WritPlace07 = value; InvalidateProperties(); } }

		public string WritWorld07;
		[CommandProperty(AccessLevel.Owner)]
		public string WritWorld_07 { get { return WritWorld07; } set { WritWorld07 = value; InvalidateProperties(); } }

		public string WritCoord07;
		[CommandProperty(AccessLevel.Owner)]
		public string WritCoord_07 { get { return WritCoord07; } set { WritCoord07 = value; InvalidateProperties(); } }

		public string WritChant07;
		[CommandProperty(AccessLevel.Owner)]
		public string WritChant_07 { get { return WritChant07; } set { WritChant07 = value; InvalidateProperties(); } }

		public Map WritMap07;
		[CommandProperty(AccessLevel.Owner)]
		public Map WritMap_07 { get { return WritMap07; } set { WritMap07 = value; InvalidateProperties(); } }

		public int WritX107;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX1_07 { get { return WritX107; } set { WritX107 = value; InvalidateProperties(); } }

		public int WritY107;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY1_07 { get { return WritY107; } set { WritY107 = value; InvalidateProperties(); } }

		public int WritX207;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX2_07 { get { return WritX207; } set { WritX207 = value; InvalidateProperties(); } }

		public int WritY207;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY2_07 { get { return WritY207; } set { WritY207 = value; InvalidateProperties(); } }

		public string WritPlace08;
		[CommandProperty(AccessLevel.Owner)]
		public string WritPlace_08 { get { return WritPlace08; } set { WritPlace08 = value; InvalidateProperties(); } }

		public string WritWorld08;
		[CommandProperty(AccessLevel.Owner)]
		public string WritWorld_08 { get { return WritWorld08; } set { WritWorld08 = value; InvalidateProperties(); } }

		public string WritCoord08;
		[CommandProperty(AccessLevel.Owner)]
		public string WritCoord_08 { get { return WritCoord08; } set { WritCoord08 = value; InvalidateProperties(); } }

		public string WritChant08;
		[CommandProperty(AccessLevel.Owner)]
		public string WritChant_08 { get { return WritChant08; } set { WritChant08 = value; InvalidateProperties(); } }

		public Map WritMap08;
		[CommandProperty(AccessLevel.Owner)]
		public Map WritMap_08 { get { return WritMap08; } set { WritMap08 = value; InvalidateProperties(); } }

		public int WritX108;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX1_08 { get { return WritX108; } set { WritX108 = value; InvalidateProperties(); } }

		public int WritY108;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY1_08 { get { return WritY108; } set { WritY108 = value; InvalidateProperties(); } }

		public int WritX208;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX2_08 { get { return WritX208; } set { WritX208 = value; InvalidateProperties(); } }

		public int WritY208;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY2_08 { get { return WritY208; } set { WritY208 = value; InvalidateProperties(); } }

		public string WritPlace09;
		[CommandProperty(AccessLevel.Owner)]
		public string WritPlace_09 { get { return WritPlace09; } set { WritPlace09 = value; InvalidateProperties(); } }

		public string WritWorld09;
		[CommandProperty(AccessLevel.Owner)]
		public string WritWorld_09 { get { return WritWorld09; } set { WritWorld09 = value; InvalidateProperties(); } }

		public string WritCoord09;
		[CommandProperty(AccessLevel.Owner)]
		public string WritCoord_09 { get { return WritCoord09; } set { WritCoord09 = value; InvalidateProperties(); } }

		public string WritChant09;
		[CommandProperty(AccessLevel.Owner)]
		public string WritChant_09 { get { return WritChant09; } set { WritChant09 = value; InvalidateProperties(); } }

		public Map WritMap09;
		[CommandProperty(AccessLevel.Owner)]
		public Map WritMap_09 { get { return WritMap09; } set { WritMap09 = value; InvalidateProperties(); } }

		public int WritX109;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX1_09 { get { return WritX109; } set { WritX109 = value; InvalidateProperties(); } }

		public int WritY109;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY1_09 { get { return WritY109; } set { WritY109 = value; InvalidateProperties(); } }

		public int WritX209;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX2_09 { get { return WritX209; } set { WritX209 = value; InvalidateProperties(); } }

		public int WritY209;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY2_09 { get { return WritY209; } set { WritY209 = value; InvalidateProperties(); } }

		public string WritPlace10;
		[CommandProperty(AccessLevel.Owner)]
		public string WritPlace_10 { get { return WritPlace10; } set { WritPlace10 = value; InvalidateProperties(); } }

		public string WritWorld10;
		[CommandProperty(AccessLevel.Owner)]
		public string WritWorld_10 { get { return WritWorld10; } set { WritWorld10 = value; InvalidateProperties(); } }

		public string WritCoord10;
		[CommandProperty(AccessLevel.Owner)]
		public string WritCoord_10 { get { return WritCoord10; } set { WritCoord10 = value; InvalidateProperties(); } }

		public string WritChant10;
		[CommandProperty(AccessLevel.Owner)]
		public string WritChant_10 { get { return WritChant10; } set { WritChant10 = value; InvalidateProperties(); } }

		public Map WritMap10;
		[CommandProperty(AccessLevel.Owner)]
		public Map WritMap_10 { get { return WritMap10; } set { WritMap10 = value; InvalidateProperties(); } }

		public int WritX110;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX1_10 { get { return WritX110; } set { WritX110 = value; InvalidateProperties(); } }

		public int WritY110;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY1_10 { get { return WritY110; } set { WritY110 = value; InvalidateProperties(); } }

		public int WritX210;
		[CommandProperty(AccessLevel.Owner)]
		public int WritX2_10 { get { return WritX210; } set { WritX210 = value; InvalidateProperties(); } }

		public int WritY210;
		[CommandProperty(AccessLevel.Owner)]
		public int WritY2_10 { get { return WritY210; } set { WritY210 = value; InvalidateProperties(); } }

		public MysticSpellbook( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
			writer.Write( (Mobile)owner);

            writer.Write( PackShrine );

            writer.Write( WritPlace01 );
            writer.Write( WritWorld01 );
            writer.Write( WritCoord01 );
            writer.Write( WritChant01 );
            writer.Write( WritMap01 );
            writer.Write( WritX101 );
            writer.Write( WritY101 );
            writer.Write( WritX201 );
            writer.Write( WritY201 );

            writer.Write( WritPlace02 );
            writer.Write( WritWorld02 );
            writer.Write( WritCoord02 );
            writer.Write( WritChant02 );
            writer.Write( WritMap02 );
            writer.Write( WritX102 );
            writer.Write( WritY102 );
            writer.Write( WritX202 );
            writer.Write( WritY202 );

            writer.Write( WritPlace03 );
            writer.Write( WritWorld03 );
            writer.Write( WritCoord03 );
            writer.Write( WritChant03 );
            writer.Write( WritMap03 );
            writer.Write( WritX103 );
            writer.Write( WritY103 );
            writer.Write( WritX203 );
            writer.Write( WritY203 );

            writer.Write( WritPlace04 );
            writer.Write( WritWorld04 );
            writer.Write( WritCoord04 );
            writer.Write( WritChant04 );
            writer.Write( WritMap04 );
            writer.Write( WritX104 );
            writer.Write( WritY104 );
            writer.Write( WritX204 );
            writer.Write( WritY204 );

            writer.Write( WritPlace05 );
            writer.Write( WritWorld05 );
            writer.Write( WritCoord05 );
            writer.Write( WritChant05 );
            writer.Write( WritMap05 );
            writer.Write( WritX105 );
            writer.Write( WritY105 );
            writer.Write( WritX205 );
            writer.Write( WritY205 );

            writer.Write( WritPlace06 );
            writer.Write( WritWorld06 );
            writer.Write( WritCoord06 );
            writer.Write( WritChant06 );
            writer.Write( WritMap06 );
            writer.Write( WritX106 );
            writer.Write( WritY106 );
            writer.Write( WritX206 );
            writer.Write( WritY206 );

            writer.Write( WritPlace07 );
            writer.Write( WritWorld07 );
            writer.Write( WritCoord07 );
            writer.Write( WritChant07 );
            writer.Write( WritMap07 );
            writer.Write( WritX107 );
            writer.Write( WritY107 );
            writer.Write( WritX207 );
            writer.Write( WritY207 );

            writer.Write( WritPlace08 );
            writer.Write( WritWorld08 );
            writer.Write( WritCoord08 );
            writer.Write( WritChant08 );
            writer.Write( WritMap08 );
            writer.Write( WritX108 );
            writer.Write( WritY108 );
            writer.Write( WritX208 );
            writer.Write( WritY208 );

            writer.Write( WritPlace09 );
            writer.Write( WritWorld09 );
            writer.Write( WritCoord09 );
            writer.Write( WritChant09 );
            writer.Write( WritMap09 );
            writer.Write( WritX109 );
            writer.Write( WritY109 );
            writer.Write( WritX209 );
            writer.Write( WritY209 );

            writer.Write( WritPlace10 );
            writer.Write( WritWorld10 );
            writer.Write( WritCoord10 );
            writer.Write( WritChant10 );
            writer.Write( WritMap10 );
            writer.Write( WritX110 );
            writer.Write( WritY110 );
            writer.Write( WritX210 );
            writer.Write( WritY210 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			owner = reader.ReadMobile();

			PackShrine = reader.ReadString();

			WritPlace01 = reader.ReadString();
			WritWorld01 = reader.ReadString();
			WritCoord01 = reader.ReadString();
			WritChant01 = reader.ReadString();
			WritMap01 = reader.ReadMap();
			WritX101 = reader.ReadInt();
			WritY101 = reader.ReadInt();
			WritX201 = reader.ReadInt();
			WritY201 = reader.ReadInt();

			WritPlace02 = reader.ReadString();
			WritWorld02 = reader.ReadString();
			WritCoord02 = reader.ReadString();
			WritChant02 = reader.ReadString();
			WritMap02 = reader.ReadMap();
			WritX102 = reader.ReadInt();
			WritY102 = reader.ReadInt();
			WritX202 = reader.ReadInt();
			WritY202 = reader.ReadInt();

			WritPlace03 = reader.ReadString();
			WritWorld03 = reader.ReadString();
			WritCoord03 = reader.ReadString();
			WritChant03 = reader.ReadString();
			WritMap03 = reader.ReadMap();
			WritX103 = reader.ReadInt();
			WritY103 = reader.ReadInt();
			WritX203 = reader.ReadInt();
			WritY203 = reader.ReadInt();

			WritPlace04 = reader.ReadString();
			WritWorld04 = reader.ReadString();
			WritCoord04 = reader.ReadString();
			WritChant04 = reader.ReadString();
			WritMap04 = reader.ReadMap();
			WritX104 = reader.ReadInt();
			WritY104 = reader.ReadInt();
			WritX204 = reader.ReadInt();
			WritY204 = reader.ReadInt();

			WritPlace05 = reader.ReadString();
			WritWorld05 = reader.ReadString();
			WritCoord05 = reader.ReadString();
			WritChant05 = reader.ReadString();
			WritMap05 = reader.ReadMap();
			WritX105 = reader.ReadInt();
			WritY105 = reader.ReadInt();
			WritX205 = reader.ReadInt();
			WritY205 = reader.ReadInt();

			WritPlace06 = reader.ReadString();
			WritWorld06 = reader.ReadString();
			WritCoord06 = reader.ReadString();
			WritChant06 = reader.ReadString();
			WritMap06 = reader.ReadMap();
			WritX106 = reader.ReadInt();
			WritY106 = reader.ReadInt();
			WritX206 = reader.ReadInt();
			WritY206 = reader.ReadInt();

			WritPlace07 = reader.ReadString();
			WritWorld07 = reader.ReadString();
			WritCoord07 = reader.ReadString();
			WritChant07 = reader.ReadString();
			WritMap07 = reader.ReadMap();
			WritX107 = reader.ReadInt();
			WritY107 = reader.ReadInt();
			WritX207 = reader.ReadInt();
			WritY207 = reader.ReadInt();

			WritPlace08 = reader.ReadString();
			WritWorld08 = reader.ReadString();
			WritCoord08 = reader.ReadString();
			WritChant08 = reader.ReadString();
			WritMap08 = reader.ReadMap();
			WritX108 = reader.ReadInt();
			WritY108 = reader.ReadInt();
			WritX208 = reader.ReadInt();
			WritY208 = reader.ReadInt();

			WritPlace09 = reader.ReadString();
			WritWorld09 = reader.ReadString();
			WritCoord09 = reader.ReadString();
			WritChant09 = reader.ReadString();
			WritMap09 = reader.ReadMap();
			WritX109 = reader.ReadInt();
			WritY109 = reader.ReadInt();
			WritX209 = reader.ReadInt();
			WritY209 = reader.ReadInt();

			WritPlace10 = reader.ReadString();
			WritWorld10 = reader.ReadString();
			WritCoord10 = reader.ReadString();
			WritChant10 = reader.ReadString();
			WritMap10 = reader.ReadMap();
			WritX110 = reader.ReadInt();
			WritY110 = reader.ReadInt();
			WritX210 = reader.ReadInt();
			WritY210 = reader.ReadInt();

			if ( ItemID != 0x6725 && ItemID != 0x6726 )
				ItemID = 0x6725;
			
			if (version < 1) Layer = Layer.Trinket;
		}
	}
}
