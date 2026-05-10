using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicPainting : Item, IRelic
	{
		public override void ItemIdentified( bool id )
		{
			m_NotIdentified = id;
			if ( !id )
			{
				ColorHue3 = "FDC844";
				ColorText3 = "Worth " + CoinPrice + " Gold";
			}
		}

		[Constructable]
		public DDRelicPainting() : base( 0x3E20 )
		{
			Weight = 10;

			if ( CoinPrice < 1 )
			{
				CoinPrice = Utility.RandomMinMax( 80, 500 );
				NotIdentified = true;
				NotIDSource = Identity.Merchant;
				NotIDSkill = IDSkill.Mercantile;

				string sLook = "a rare";
				switch ( Utility.RandomMinMax( 0, 18 ) )
				{
					case 0:	sLook = "a rare";	break;
					case 1:	sLook = "a nice";	break;
					case 2:	sLook = "a pretty";	break;
					case 3:	sLook = "a superb";	break;
					case 4:	sLook = "a delightful";	break;
					case 5:	sLook = "an elegant";	break;
					case 6:	sLook = "an exquisite";	break;
					case 7:	sLook = "a fine";	break;
					case 8:	sLook = "a gorgeous";	break;
					case 9:	sLook = "a lovely";	break;
					case 10:sLook = "a magnificent";	break;
					case 11:sLook = "a marvelous";	break;
					case 12:sLook = "a splendid";	break;
					case 13:sLook = "a wonderful";	break;
					case 14:sLook = "an extraordinary";	break;
					case 15:sLook = "a strange";	break;
					case 16:sLook = "an odd";	break;
					case 17:sLook = "a unique";	break;
					case 18:sLook = "an unusual";	break;
				}

				switch ( Utility.RandomMinMax( 0, 80 ) ) 
				{
					case 0: ItemID = 0x3E20; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E21; } break;
					case 1: ItemID = 0x3E7; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0xC2C; } break;
					case 2: ItemID = 0x3E8; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0xEA0; } break;
					case 3: ItemID = 0x2A5D; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x2A61; } break;
					case 4: ItemID = 0x3EA; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0xEA7; } break;
					case 5: ItemID = 0x3EB; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0xEA6; } break;
					case 6: ItemID = 0x3EC; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0xEA5; } break;
					case 7: ItemID = 0x3ED; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0xE55; } break;
					case 8: ItemID = 0xDDF; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0xE9F; } break;
					case 9: ItemID = 0xEA3; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0xEA4; } break;
					case 10: ItemID = 0xEA1; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0xEA2; } break;
					case 11: ItemID = 0x2A65; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x2A68; } break;
					case 12: ItemID = 0x3308; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E0C; } break;
					case 13: ItemID = 0x3309; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E0D; } break;
					case 14: ItemID = 0x330A; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E0E; } break;
					case 15: ItemID = 0x330B; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E0F; } break;
					case 16: ItemID = 0x330C; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E10; } break;
					case 17: ItemID = 0x330D; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E11; } break;
					case 18: ItemID = 0x330E; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E12; } break;
					case 19: ItemID = 0x330F; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E13; } break;
					case 20: ItemID = 0x3310; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E14; } break;
					case 21: ItemID = 0x3311; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E15; } break;
					case 22: ItemID = 0x3312; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E16; } break;
					case 23: ItemID = 0x3313; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E17; } break;
					case 24: ItemID = 0x3314; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E18; } break;
					case 25: ItemID = 0x3315; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E19; } break;
					case 26: ItemID = 0x3316; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E1A; } break;
					case 27: ItemID = 0x3317; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E1B; } break;
					case 28: ItemID = 0x3318; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E1C; } break;
					case 29: ItemID = 0x3319; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E1D; } break;
					case 30: ItemID = 0x331A; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E1E; } break;
					case 31: ItemID = 0x331B; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E1F; } break;
					case 32: ItemID = 0x331C; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E22; } break;
					case 33: ItemID = 0x331D; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E23; } break;
					case 34: ItemID = 0x331E; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E24; } break;
					case 35: ItemID = 0x331F; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E25; } break;
					case 36: ItemID = 0x3320; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3E26; } break;
					case 37: ItemID = 0x3321; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DEC; } break;
					case 38: ItemID = 0x3322; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DED; } break;
					case 39: ItemID = 0x3323; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DEE; } break;
					case 40: ItemID = 0x3324; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DEF; } break;
					case 41: ItemID = 0x3325; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DF0; } break;
					case 42: ItemID = 0x3326; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DF1; } break;
					case 43: ItemID = 0x3327; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DF2; } break;
					case 44: ItemID = 0x3328; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DF3; } break;
					case 45: ItemID = 0x3329; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DF4; } break;
					case 46: ItemID = 0x332A; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DF5; } break;
					case 47: ItemID = 0x332B; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DF6; } break;
					case 48: ItemID = 0x332C; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DF7; } break;
					case 49: ItemID = 0x332D; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DF8; } break;
					case 50: ItemID = 0x332E; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DF9; } break;
					case 51: ItemID = 0x332F; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 0x3DFA; } break;
					case 52: ItemID = 21291; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21292; } break;
					case 53: ItemID = 21293; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21294; } break;
					case 54: ItemID = 21295; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21296; } break;
					case 55: ItemID = 21297; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21298; } break;
					case 56: ItemID = 21299; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21300; } break;
					case 57: ItemID = 21301; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21302; } break;
					case 58: ItemID = 21303; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21304; } break;
					case 59: ItemID = 21305; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21306; } break;
					case 60: ItemID = 21307; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21308; } break;
					case 61: ItemID = 21309; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21310; } break;
					case 62: ItemID = 21311; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21312; } break;
					case 63: ItemID = 21313; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21314; } break;
					case 64: ItemID = 21315; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21316; } break;
					case 65: ItemID = 21317; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21318; } break;
					case 66: ItemID = 21319; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21320; } break;
					case 67: ItemID = 21321; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21322; } break;
					case 68: ItemID = 21367; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21368; } break;
					case 69: ItemID = 21369; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21370; } break;
					case 70: ItemID = 21371; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21372; } break;
					case 71: ItemID = 21373; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21374; } break;
					case 72: ItemID = 21375; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21376; } break;
					case 73: ItemID = 21377; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21378; } break;
					case 74: ItemID = 21379; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21380; } break;
					case 75: ItemID = 21381; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21382; } break;
					case 76: ItemID = 21383; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21384; } break;
					case 77: ItemID = 21385; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21386; } break;
					case 78: ItemID = 21387; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21388; } break;
					case 79: ItemID = 21389; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21390; } break;
					case 80: ItemID = 21391; if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ItemID = 21392; } break;
				}

				string sPainting = Server.Misc.RandomThings.GetRandomScenePainting();
				if ( ItemID >= 0x5377 && ItemID <= 0x5390 ){ Hue = Utility.RandomList( 0xABE, 0x4A7, 0x747, 0x96C, 0x7DA, 0x415, 0x908, 0x712, 0x1CD, 0x9C2, 0x843, 0x750, 0xA94, 0x973, 0xA3A ); }

				switch ( Utility.RandomMinMax( 0, 7 ) ) 
				{
					case 0: sPainting = Server.Misc.RandomThings.GetRandomScenePainting(); break;
					case 1: sPainting = Server.Misc.RandomThings.GetRandomCity(); break;
					case 2: sPainting = "the " + Server.Misc.RandomThings.RandomMagicalItem(); break;
					case 3: sPainting = Server.Misc.RandomThings.GetRandomShipName( "", 0 ); break;
					case 4: sPainting = Server.Misc.RandomThings.GetRandomGirlName() + " the " + Server.Misc.RandomThings.GetBoyGirlJob( 1 ); break;
					case 5: sPainting = Server.Misc.RandomThings.GetRandomBoyName() + " the " + Server.Misc.RandomThings.GetBoyGirlJob( 0 ); break;
					case 6: sPainting = Server.Misc.RandomThings.GetRandomGirlName() + " the " + Server.Misc.RandomThings.GetRandomGirlNoble(); break;
					case 7: sPainting = Server.Misc.RandomThings.GetRandomBoyName() + " the " + Server.Misc.RandomThings.GetRandomBoyNoble(); break;
				}

				Name = sLook + " painting of " + sPainting;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack && from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified ) 
				from.SendMessage( "This must be in your backpack to identify." );
			else if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified )
				IDCommand( from );
			else if ( !IsChildOf( from.Backpack ) )
				from.SendMessage( "This must be in your backpack to flip." );
			else if ( this.Name.Contains(" (covered in muck)") )
			{
				from.SendMessage( "You clear the muck from the painting." );
				this.Hue = 0;
				if ( ItemID >= 0x5377 && ItemID <= 0x5390 ){ Hue = Utility.RandomList( 0xABE, 0x4A7, 0x747, 0x96C, 0x7DA, 0x415, 0x908, 0x712, 0x1CD, 0x9C2, 0x843, 0x750, 0xA94, 0x973, 0xA3A ); }
				this.Name = this.Name.Replace(" (covered in muck)", "");
			}
			else
			{
				if ( this.ItemID == 0x3E20 ){ this.ItemID = 0x3E21; }
				else if ( this.ItemID == 0x3E7 ){ this.ItemID = 0xC2C; }
				else if ( this.ItemID == 0x3E8 ){ this.ItemID = 0xEA0; }
				else if ( this.ItemID == 0x2A5D ){ this.ItemID = 0x2A61; }
				else if ( this.ItemID == 0x3EA ){ this.ItemID = 0xEA7; }
				else if ( this.ItemID == 0x3EB ){ this.ItemID = 0xEA6; }
				else if ( this.ItemID == 0x3EC ){ this.ItemID = 0xEA5; }
				else if ( this.ItemID == 0x3ED ){ this.ItemID = 0xE55; }
				else if ( this.ItemID == 0xDDF ){ this.ItemID = 0xE9F; }
				else if ( this.ItemID == 0xEA3 ){ this.ItemID = 0xEA4; }
				else if ( this.ItemID == 0xEA1 ){ this.ItemID = 0xEA2; }
				else if ( this.ItemID == 0x2A65 ){ this.ItemID = 0x2A68; }
				else if ( this.ItemID == 0x3308 ){ this.ItemID = 0x3E0C; }
				else if ( this.ItemID == 0x3309 ){ this.ItemID = 0x3E0D; }
				else if ( this.ItemID == 0x330A ){ this.ItemID = 0x3E0E; }
				else if ( this.ItemID == 0x330B ){ this.ItemID = 0x3E0F; }
				else if ( this.ItemID == 0x330C ){ this.ItemID = 0x3E10; }
				else if ( this.ItemID == 0x330D ){ this.ItemID = 0x3E11; }
				else if ( this.ItemID == 0x330E ){ this.ItemID = 0x3E12; }
				else if ( this.ItemID == 0x330F ){ this.ItemID = 0x3E13; }
				else if ( this.ItemID == 0x3310 ){ this.ItemID = 0x3E14; }
				else if ( this.ItemID == 0x3311 ){ this.ItemID = 0x3E15; }
				else if ( this.ItemID == 0x3312 ){ this.ItemID = 0x3E16; }
				else if ( this.ItemID == 0x3313 ){ this.ItemID = 0x3E17; }
				else if ( this.ItemID == 0x3314 ){ this.ItemID = 0x3E18; }
				else if ( this.ItemID == 0x3315 ){ this.ItemID = 0x3E19; }
				else if ( this.ItemID == 0x3316 ){ this.ItemID = 0x3E1A; }
				else if ( this.ItemID == 0x3317 ){ this.ItemID = 0x3E1B; }
				else if ( this.ItemID == 0x3318 ){ this.ItemID = 0x3E1C; }
				else if ( this.ItemID == 0x3319 ){ this.ItemID = 0x3E1D; }
				else if ( this.ItemID == 0x331A ){ this.ItemID = 0x3E1E; }
				else if ( this.ItemID == 0x331B ){ this.ItemID = 0x3E1F; }
				else if ( this.ItemID == 0x331C ){ this.ItemID = 0x3E22; }
				else if ( this.ItemID == 0x331D ){ this.ItemID = 0x3E23; }
				else if ( this.ItemID == 0x331E ){ this.ItemID = 0x3E24; }
				else if ( this.ItemID == 0x331F ){ this.ItemID = 0x3E25; }
				else if ( this.ItemID == 0x3320 ){ this.ItemID = 0x3E26; }
				else if ( this.ItemID == 0x3321 ){ this.ItemID = 0x3DEC; }
				else if ( this.ItemID == 0x3322 ){ this.ItemID = 0x3DED; }
				else if ( this.ItemID == 0x3323 ){ this.ItemID = 0x3DEE; }
				else if ( this.ItemID == 0x3324 ){ this.ItemID = 0x3DEF; }
				else if ( this.ItemID == 0x3325 ){ this.ItemID = 0x3DF0; }
				else if ( this.ItemID == 0x3326 ){ this.ItemID = 0x3DF1; }
				else if ( this.ItemID == 0x3327 ){ this.ItemID = 0x3DF2; }
				else if ( this.ItemID == 0x3328 ){ this.ItemID = 0x3DF3; }
				else if ( this.ItemID == 0x3329 ){ this.ItemID = 0x3DF4; }
				else if ( this.ItemID == 0x332A ){ this.ItemID = 0x3DF5; }
				else if ( this.ItemID == 0x332B ){ this.ItemID = 0x3DF6; }
				else if ( this.ItemID == 0x332C ){ this.ItemID = 0x3DF7; }
				else if ( this.ItemID == 0x332D ){ this.ItemID = 0x3DF8; }
				else if ( this.ItemID == 0x332E ){ this.ItemID = 0x3DF9; }
				else if ( this.ItemID == 0x332F ){ this.ItemID = 0x3DFA; }
				else if ( this.ItemID == 0x3E21 ){ this.ItemID = 0x3E20; }
				else if ( this.ItemID == 0xC2C ){ this.ItemID = 0x3E7; }
				else if ( this.ItemID == 0xEA0 ){ this.ItemID = 0x3E8; }
				else if ( this.ItemID == 0x2A61 ){ this.ItemID = 0x2A5D; }
				else if ( this.ItemID == 0xEA7 ){ this.ItemID = 0x3EA; }
				else if ( this.ItemID == 0xEA6 ){ this.ItemID = 0x3EB; }
				else if ( this.ItemID == 0xEA5 ){ this.ItemID = 0x3EC; }
				else if ( this.ItemID == 0xE55 ){ this.ItemID = 0x3ED; }
				else if ( this.ItemID == 0xE9F ){ this.ItemID = 0xDDF; }
				else if ( this.ItemID == 0xEA4 ){ this.ItemID = 0xEA3; }
				else if ( this.ItemID == 0xEA2 ){ this.ItemID = 0xEA1; }
				else if ( this.ItemID == 0x2A68 ){ this.ItemID = 0x2A65; }
				else if ( this.ItemID == 0x3E0C ){ this.ItemID = 0x3308; }
				else if ( this.ItemID == 0x3E0D ){ this.ItemID = 0x3309; }
				else if ( this.ItemID == 0x3E0E ){ this.ItemID = 0x330A; }
				else if ( this.ItemID == 0x3E0F ){ this.ItemID = 0x330B; }
				else if ( this.ItemID == 0x3E10 ){ this.ItemID = 0x330C; }
				else if ( this.ItemID == 0x3E11 ){ this.ItemID = 0x330D; }
				else if ( this.ItemID == 0x3E12 ){ this.ItemID = 0x330E; }
				else if ( this.ItemID == 0x3E13 ){ this.ItemID = 0x330F; }
				else if ( this.ItemID == 0x3E14 ){ this.ItemID = 0x3310; }
				else if ( this.ItemID == 0x3E15 ){ this.ItemID = 0x3311; }
				else if ( this.ItemID == 0x3E16 ){ this.ItemID = 0x3312; }
				else if ( this.ItemID == 0x3E17 ){ this.ItemID = 0x3313; }
				else if ( this.ItemID == 0x3E18 ){ this.ItemID = 0x3314; }
				else if ( this.ItemID == 0x3E19 ){ this.ItemID = 0x3315; }
				else if ( this.ItemID == 0x3E1A ){ this.ItemID = 0x3316; }
				else if ( this.ItemID == 0x3E1B ){ this.ItemID = 0x3317; }
				else if ( this.ItemID == 0x3E1C ){ this.ItemID = 0x3318; }
				else if ( this.ItemID == 0x3E1D ){ this.ItemID = 0x3319; }
				else if ( this.ItemID == 0x3E1E ){ this.ItemID = 0x331A; }
				else if ( this.ItemID == 0x3E1F ){ this.ItemID = 0x331B; }
				else if ( this.ItemID == 0x3E22 ){ this.ItemID = 0x331C; }
				else if ( this.ItemID == 0x3E23 ){ this.ItemID = 0x331D; }
				else if ( this.ItemID == 0x3E24 ){ this.ItemID = 0x331E; }
				else if ( this.ItemID == 0x3E25 ){ this.ItemID = 0x331F; }
				else if ( this.ItemID == 0x3E26 ){ this.ItemID = 0x3320; }
				else if ( this.ItemID == 0x3DEC ){ this.ItemID = 0x3321; }
				else if ( this.ItemID == 0x3DED ){ this.ItemID = 0x3322; }
				else if ( this.ItemID == 0x3DEE ){ this.ItemID = 0x3323; }
				else if ( this.ItemID == 0x3DEF ){ this.ItemID = 0x3324; }
				else if ( this.ItemID == 0x3DF0 ){ this.ItemID = 0x3325; }
				else if ( this.ItemID == 0x3DF1 ){ this.ItemID = 0x3326; }
				else if ( this.ItemID == 0x3DF2 ){ this.ItemID = 0x3327; }
				else if ( this.ItemID == 0x3DF3 ){ this.ItemID = 0x3328; }
				else if ( this.ItemID == 0x3DF4 ){ this.ItemID = 0x3329; }
				else if ( this.ItemID == 0x3DF5 ){ this.ItemID = 0x332A; }
				else if ( this.ItemID == 0x3DF6 ){ this.ItemID = 0x332B; }
				else if ( this.ItemID == 0x3DF7 ){ this.ItemID = 0x332C; }
				else if ( this.ItemID == 0x3DF8 ){ this.ItemID = 0x332D; }
				else if ( this.ItemID == 0x3DF9 ){ this.ItemID = 0x332E; }
				else if ( this.ItemID == 0x3DFA ){ this.ItemID = 0x332F; }
				else if ( this.ItemID == 21291 ){ this.ItemID = 21292; }
				else if ( this.ItemID == 21293 ){ this.ItemID = 21294; }
				else if ( this.ItemID == 21295 ){ this.ItemID = 21296; }
				else if ( this.ItemID == 21297 ){ this.ItemID = 21298; }
				else if ( this.ItemID == 21299 ){ this.ItemID = 21300; }
				else if ( this.ItemID == 21301 ){ this.ItemID = 21302; }
				else if ( this.ItemID == 21303 ){ this.ItemID = 21304; }
				else if ( this.ItemID == 21305 ){ this.ItemID = 21306; }
				else if ( this.ItemID == 21307 ){ this.ItemID = 21308; }
				else if ( this.ItemID == 21309 ){ this.ItemID = 21310; }
				else if ( this.ItemID == 21311 ){ this.ItemID = 21312; }
				else if ( this.ItemID == 21313 ){ this.ItemID = 21314; }
				else if ( this.ItemID == 21315 ){ this.ItemID = 21316; }
				else if ( this.ItemID == 21317 ){ this.ItemID = 21318; }
				else if ( this.ItemID == 21319 ){ this.ItemID = 21320; }
				else if ( this.ItemID == 21321 ){ this.ItemID = 21322; }
				else if ( this.ItemID == 21367 ){ this.ItemID = 21368; }
				else if ( this.ItemID == 21369 ){ this.ItemID = 21370; }
				else if ( this.ItemID == 21371 ){ this.ItemID = 21372; }
				else if ( this.ItemID == 21373 ){ this.ItemID = 21374; }
				else if ( this.ItemID == 21375 ){ this.ItemID = 21376; }
				else if ( this.ItemID == 21377 ){ this.ItemID = 21378; }
				else if ( this.ItemID == 21379 ){ this.ItemID = 21380; }
				else if ( this.ItemID == 21381 ){ this.ItemID = 21382; }
				else if ( this.ItemID == 21383 ){ this.ItemID = 21384; }
				else if ( this.ItemID == 21385 ){ this.ItemID = 21386; }
				else if ( this.ItemID == 21387 ){ this.ItemID = 21388; }
				else if ( this.ItemID == 21389 ){ this.ItemID = 21390; }
				else if ( this.ItemID == 21391 ){ this.ItemID = 21392; }
				else if ( this.ItemID == 21292 ){ this.ItemID = 21291; }
				else if ( this.ItemID == 21294 ){ this.ItemID = 21293; }
				else if ( this.ItemID == 21296 ){ this.ItemID = 21295; }
				else if ( this.ItemID == 21298 ){ this.ItemID = 21297; }
				else if ( this.ItemID == 21300 ){ this.ItemID = 21299; }
				else if ( this.ItemID == 21302 ){ this.ItemID = 21301; }
				else if ( this.ItemID == 21304 ){ this.ItemID = 21303; }
				else if ( this.ItemID == 21306 ){ this.ItemID = 21305; }
				else if ( this.ItemID == 21308 ){ this.ItemID = 21307; }
				else if ( this.ItemID == 21310 ){ this.ItemID = 21309; }
				else if ( this.ItemID == 21312 ){ this.ItemID = 21311; }
				else if ( this.ItemID == 21314 ){ this.ItemID = 21313; }
				else if ( this.ItemID == 21316 ){ this.ItemID = 21315; }
				else if ( this.ItemID == 21318 ){ this.ItemID = 21317; }
				else if ( this.ItemID == 21320 ){ this.ItemID = 21319; }
				else if ( this.ItemID == 21322 ){ this.ItemID = 21321; }
				else if ( this.ItemID == 21368 ){ this.ItemID = 21367; }
				else if ( this.ItemID == 21370 ){ this.ItemID = 21369; }
				else if ( this.ItemID == 21372 ){ this.ItemID = 21371; }
				else if ( this.ItemID == 21374 ){ this.ItemID = 21373; }
				else if ( this.ItemID == 21376 ){ this.ItemID = 21375; }
				else if ( this.ItemID == 21378 ){ this.ItemID = 21377; }
				else if ( this.ItemID == 21380 ){ this.ItemID = 21379; }
				else if ( this.ItemID == 21382 ){ this.ItemID = 21381; }
				else if ( this.ItemID == 21384 ){ this.ItemID = 21383; }
				else if ( this.ItemID == 21386 ){ this.ItemID = 21385; }
				else if ( this.ItemID == 21388 ){ this.ItemID = 21387; }
				else if ( this.ItemID == 21390 ){ this.ItemID = 21389; }
				else if ( this.ItemID == 21392 ){ this.ItemID = 21391; }
				else if ( this.ItemID == 0x52FE ){ this.ItemID = 0x52FF; }
				else if ( this.ItemID == 0x52FF ){ this.ItemID = 0x52FE; }
				else if ( this.ItemID == 0x52B3 ){ this.ItemID = 0x52B4; }
				else if ( this.ItemID == 0x52B4 ){ this.ItemID = 0x52B3; }
				else if ( this.ItemID == 0x53A6 ){ this.ItemID = 0x53A7; }
				else if ( this.ItemID == 0x53A7 ){ this.ItemID = 0x53A6; }
				else if ( this.ItemID == 0x49A0 ){ this.ItemID = 0x49B4; }
				else if ( this.ItemID == 0x49A2 ){ this.ItemID = 0x49BF; }
				else if ( this.ItemID == 0x49A3 ){ this.ItemID = 0x49C0; }
				else if ( this.ItemID == 0x49A7 ){ this.ItemID = 0x49BA; }
				else if ( this.ItemID == 0x49A8 ){ this.ItemID = 0x49B5; }
				else if ( this.ItemID == 0x49B2 ){ this.ItemID = 0x49BE; }
				else if ( this.ItemID == 0x49B3 ){ this.ItemID = 0x49BB; }
				else if ( this.ItemID == 0x49A1 ){ this.ItemID = 0x49B9; }
				else if ( this.ItemID == 0x49B4 ){ this.ItemID = 0x49A0; }
				else if ( this.ItemID == 0x49BF ){ this.ItemID = 0x49A2; }
				else if ( this.ItemID == 0x49C0 ){ this.ItemID = 0x49A3; }
				else if ( this.ItemID == 0x49BA ){ this.ItemID = 0x49A7; }
				else if ( this.ItemID == 0x49B5 ){ this.ItemID = 0x49A8; }
				else if ( this.ItemID == 0x49BE ){ this.ItemID = 0x49B2; }
				else if ( this.ItemID == 0x49BB ){ this.ItemID = 0x49B3; }
				else if ( this.ItemID == 0x49B9 ){ this.ItemID = 0x49A1; }
			}
		}

		public override void IDCommand( Mobile m )
		{
			if ( this.NotIDSkill == IDSkill.Tasting )
				RelicFunctions.IDItem( m, m, this, SkillName.Tasting );
			else if ( this.NotIDSkill == IDSkill.ArmsLore )
				RelicFunctions.IDItem( m, m, this, SkillName.ArmsLore );
			else
				RelicFunctions.IDItem( m, m, this, SkillName.Mercantile );
		}

		public DDRelicPainting(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
            int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();
		}
	}
}