using System;
using Server;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicBanner : Item, IRelic
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

		public int RelicFlipID1;
		public int RelicFlipID2;

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_FlipID1 { get { return RelicFlipID1; } set { RelicFlipID1 = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_FlipID2 { get { return RelicFlipID2; } set { RelicFlipID2 = value; InvalidateProperties(); } }

		[Constructable]
		public DDRelicBanner() : base( 0x2D6F )
		{
			Weight = 30;
			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Merchant;
			NotIDSkill = IDSkill.Mercantile;
			Hue = Utility.RandomColor(0);
			int SuperRare = 0;

			string sLook = "a rare";
			switch ( Utility.RandomMinMax( 0, 19 ) )
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
				case 19:SuperRare = 1; break;
			}

			switch ( Utility.RandomMinMax( 0, 9 ) ) 
			{
				case 0: ItemID = 0x2D6F; RelicFlipID1 = 0x2D6F; RelicFlipID2 = 0x2D70; break;
				case 1: ItemID = 0x2D71; RelicFlipID1 = 0x2D71; RelicFlipID2 = 0x2D72; break;
				case 2: ItemID = 0x42C4; RelicFlipID1 = 0x42C4; RelicFlipID2 = 0x42C4; break;
				case 3: ItemID = 0x42C9; RelicFlipID1 = 0x42C9; RelicFlipID2 = 0x42CA; break;
				case 4: ItemID = 0x42CB; RelicFlipID1 = 0x42CB; RelicFlipID2 = 0x42CC; break;
				case 5: ItemID = 0x42CD; RelicFlipID1 = 0x42CD; RelicFlipID2 = 0x42CE; break;
				case 6: ItemID = 0x2D6F; RelicFlipID1 = 0x2D6F; RelicFlipID2 = 0x2D70; break;
				case 7: ItemID = 0x2D71; RelicFlipID1 = 0x2D71; RelicFlipID2 = 0x2D72; break;
				case 8: ItemID = 0x42C4; RelicFlipID1 = 0x42C4; RelicFlipID2 = 0x42C4; break;
				case 9: ItemID = 0x49A1; RelicFlipID1 = 0x49A1; RelicFlipID2 = 0x49B9; break;
			}

			Name = sLook + " tapestry";

			if ( SuperRare > 0 )
			{
				ItemID = 0x2886; RelicFlipID1 = 0x2886; RelicFlipID2 = 0x2887;
				Name = "a painting of " + Server.Misc.RandomThings.GetRandomScenePainting();
				Hue = 0;
				Weight = 100;
				CoinPrice = Utility.RandomMinMax( 1000, 5000 );
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
			else
				if ( this.ItemID == RelicFlipID1 ){ this.ItemID = RelicFlipID2; } else { this.ItemID = RelicFlipID1; }
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

		public static void MakeOriental( Item item )
		{
			DDRelicBanner relic = (DDRelicBanner)item;

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

			string OwnerName = Server.Misc.RandomThings.GetRandomOrientalName();
			string OwnerTitle = RandomThings.MagicItemAdj( "end", true, false, item.ItemID );
			string OwnerNation = Server.Misc.RandomThings.GetRandomOrientalNation();

			if ( Utility.RandomMinMax( 1, 3 ) > 1 )
			{
				relic.Name = sLook + " painting of " + OwnerName + " " + OwnerTitle;
			}
			else
			{
				relic.Hue = 0;
				string sDecon = "decorative";
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	sDecon = "decorative";		break;
					case 1:	sDecon = "ceremonial";		break;
					case 2:	sDecon = "ornamental";		break;
					case 3:	sDecon = Server.Misc.RandomThings.GetRandomOrientalNation();		break;
					case 4:	sDecon = Server.Misc.RandomThings.GetRandomOrientalNation();		break;
					case 5:	sDecon = Server.Misc.RandomThings.GetRandomOrientalNation();		break;
				}

				string sWorld = "land";
				switch ( Utility.RandomMinMax( 0, 6 ) )
				{
					case 0:	sWorld = "land";		break;
					case 1:	sWorld = "world";		break;
					case 2:	sWorld = "island";		break;
					case 3:	sWorld = "peninsula";	break;
					case 4:	sWorld = "continent";	break;
					case 5:	sWorld = "sea";			break;
					case 6:	sWorld = "ocean";		break;
				}

				string sScene = "a " + OwnerNation + " battle";
				switch ( Utility.RandomMinMax( 0, 10 ) )
				{
					case 0:		sScene = "a " + OwnerNation + " battle";		break;
					case 1:		sScene = "a " + OwnerNation + " castle";		break;
					case 2:		sScene = "a " + OwnerNation + " monestary";		break;
					case 3:		sScene = "a " + OwnerNation + " garden";		break;
					case 4:		sScene = "the " + OwnerName + " landscape";		break;
					case 5:		sScene = "the " + OwnerName + " sea";			break;
					case 6:		sScene = "a " + OwnerNation + " city";			break;
					case 7:		sScene = "a " + OwnerNation + " village";		break;
					case 8:		sScene = "a " + OwnerNation + " home";			break;
					case 9:		sScene = "the " + OwnerName + " palace";		break;
					case 10:	sScene = "a " + OwnerNation + " pagoda";		break;
				}

				string sSaying = Server.Misc.RandomThings.GetRandomJobTitle(0);
				switch ( Utility.RandomMinMax( 0, 3 ) )
				{
					case 0:	sSaying = Server.Misc.RandomThings.GetRandomJobTitle(0);		break;
					case 1:	sSaying = Server.Misc.RandomThings.GetRandomColorName(0) + " " + Server.Misc.RandomThings.GetRandomJobTitle(0);		break;
					case 2:	sSaying = Server.Misc.RandomThings.GetRandomThing(0);		break;
					case 3:	sSaying = Server.Misc.RandomThings.GetRandomColorName(0) + " " + Server.Misc.RandomThings.GetRandomThing(0);		break;
				}

				switch ( Utility.RandomMinMax( 0, 10 ) ) 
				{
					case 0: relic.Weight = 20.0;	relic.ItemID = 0x2D73; relic.RelicFlipID1 = 0x2D73; relic.RelicFlipID2 = 0x2D74; relic.Name = sLook + " map of the " + OwnerNation + " " + sWorld + " of " + OwnerName; break;
					case 1: relic.Weight = 20.0;	relic.ItemID = 0x2D73; relic.RelicFlipID1 = 0x2D73; relic.RelicFlipID2 = 0x2D74; relic.Name = sLook + " map of a " + sWorld + " during the " + Server.Misc.RandomThings.GetRandomColorName(0) + " Dynasty"; break;
					case 2: relic.Weight = 5.0;		relic.ItemID = 0x2409; relic.RelicFlipID1 = 0x2409; relic.RelicFlipID2 = 0x240A; relic.Name = sLook + ", " + sDecon + " fan"; break;
					case 3: relic.Weight = 5.0;		relic.ItemID = 0x240B; relic.RelicFlipID1 = 0x240B; relic.RelicFlipID2 = 0x240C; relic.Name = sLook + " set of " + sDecon + " fans"; break;
					case 4: relic.Weight = 10.0;	relic.ItemID = 0x240D; relic.RelicFlipID1 = 0x240D; relic.RelicFlipID2 = 0x240E; relic.Name = sLook + " painting of " + sScene; break;
					case 5: relic.Weight = 10.0;	relic.ItemID = 0x240F; relic.RelicFlipID1 = 0x240F; relic.RelicFlipID2 = 0x2410; relic.Name = sLook + " painting of " + sScene; break;
					case 6: relic.Weight = 10.0;	relic.ItemID = 0x2411; relic.RelicFlipID1 = 0x2411; relic.RelicFlipID2 = 0x2412; relic.Name = sLook + " painting of " + sScene; break;
					case 7: relic.Weight = 10.0;	relic.ItemID = 0x2413; relic.RelicFlipID1 = 0x2413; relic.RelicFlipID2 = 0x2414; relic.Name = sLook + " painting of " + sScene; break;
					case 8: relic.Weight = 50.0;	relic.ItemID = 0x2886; relic.RelicFlipID1 = 0x2886; relic.RelicFlipID2 = 0x2887; relic.Name = sLook + " painting of " + sScene; break;
					case 9: relic.Weight = 15.0;	relic.ItemID = 0x2415; relic.RelicFlipID1 = 0x2415; relic.RelicFlipID2 = 0x2416; relic.Name = "a painting of " + OwnerNation + " symbols for the " + sSaying; break;
					case 10: relic.Weight = 15.0;	relic.ItemID = 0x2417; relic.RelicFlipID1 = 0x2417; relic.RelicFlipID2 = 0x2418; relic.Name = "a painting of " + OwnerNation + " symbols for the " + sSaying; break;
				}
			}
		}

		public DDRelicBanner(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write( (int) 1 ); // version
            writer.Write( RelicFlipID1 );
            writer.Write( RelicFlipID2 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
            int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();

            RelicFlipID1 = reader.ReadInt();
            RelicFlipID2 = reader.ReadInt();
		}
	}
}