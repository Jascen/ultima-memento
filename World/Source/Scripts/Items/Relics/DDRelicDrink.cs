using System;
using Server;
using System.Collections;
using Server.Network;
using Server.Targeting;
using Server.Prompts;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicDrink : Item, IRelic
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
		public DDRelicDrink() : base( 0x9C7 )
		{
			string sType = " bottle of ";

			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Potion;
			NotIDSkill = IDSkill.Tasting;

			int drinktype = Utility.RandomMinMax( 1, 4 );

			if ( drinktype == 1 )
			{
				Weight = 3;
				ItemID = Utility.RandomList( 0x9C7, 0x99B, 0x99F );
				Hue = Utility.RandomColor(0);
				sType = " bottle of ";
			}
			else if ( drinktype == 2 )
			{
				Weight = 5;
				ItemID = Utility.RandomList( 0x5447, 0x5448, 0x5449 );
				Hue = Utility.RandomColor(0);
				sType = " bottle of " + NameList.RandomName( "cultures" ) + " ";
			}
			else if ( drinktype == 3 )
			{
				Weight = 100;
				ItemID = 0xFAE;
				sType = " barrel of ";
			}
			else
			{
				Weight = 50;
				ItemID = Utility.RandomList( 0x1940, 0x1AD6, 0x1AD7 );
				Hue = 0x96D;
				sType = " keg of ";
			}

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

			string sLiquid = "ale";
			switch ( Utility.RandomMinMax( 0, 7 ) ) 
			{
				case 0: sLiquid = "ale"; break;
				case 1: sLiquid = "wine"; break;
				case 2: sLiquid = "rum"; break;
				case 3: sLiquid = "beer"; break;
				case 4: sLiquid = "grog"; break;
				case 5: sLiquid = "brandy"; break;
				case 6: sLiquid = "whiskey"; break;
				case 7: sLiquid = "brandy"; break;
			}

			Name = sLook + sType + sLiquid;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( NotIdentified )
			{
				if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID )
				{
					if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack ) 
					from.SendMessage( "This must be in your backpack to identify." );
					else 
						IDCommand( from );
				}
			}
			else
			{
				from.Stam = 20;
				from.Thirst = 20;
				this.Consume();
				from.PlaySound( Utility.RandomList( 0x30, 0x2D6 ) );

				if ( Weight > 75 )
				{
					from.AddToBackpack( new Barrel() );
					from.SendMessage( "You down the entire keg and are no longer thirsty." );
				}
				else if ( Weight > 25 )
				{
					from.AddToBackpack( new PotionKeg() );
					from.SendMessage( "You down the entire keg and are no longer thirsty." );
				}
				else
				{
					from.AddToBackpack( new Bottle() );
					from.SendMessage( "You drink the entire bottle and are no longer thirsty." );
				}
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

		public DDRelicDrink(Serial serial) : base(serial)
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