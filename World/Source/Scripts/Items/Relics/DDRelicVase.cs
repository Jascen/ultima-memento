using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicVase : Item, IRelic
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
		public DDRelicVase() : base( 0x44F1 )
		{
			Weight = 20;
			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Merchant;
			NotIDSkill = IDSkill.Mercantile;
			Hue = Utility.RandomColor(0);

			switch ( Utility.RandomMinMax( 0, 8 ) ) 
			{
				case 0: ItemID = 0x44F1; break;
				case 1: ItemID = 0xB46; break;
				case 2: ItemID = 0x44EF; break;
				case 3: ItemID = 0xB48; break;
				case 4: ItemID = 0xB45; Weight = 40; CoinPrice = Utility.RandomMinMax( 20, 150 ); break;
				case 5: ItemID = 0xB47; Weight = 40; CoinPrice = Utility.RandomMinMax( 20, 150 ); break;
				case 6: ItemID = 0x42B2; Weight = 40; CoinPrice = Utility.RandomMinMax( 20, 150 ); break;
				case 7: ItemID = 0x42B3; Weight = 40; CoinPrice = Utility.RandomMinMax( 20, 150 ); break;
				case 8: ItemID = 0x44F0; Weight = 40; CoinPrice = Utility.RandomMinMax( 20, 150 ); break;
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
			Name = sLook + " vase";
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack && from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified ) 
				from.SendMessage( "This must be in your backpack to identify." );
			else if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified )
				IDCommand( from );
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

		public DDRelicVase(Serial serial) : base(serial)
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